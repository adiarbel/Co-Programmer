using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Text;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Company.VSPackage1
{
    /// <summary>
    /// The purpose of this class is to manage text view issues before and after use, manage connections between UI and network. 
    /// COllaboration of the network and UI managment class
    /// </summary>
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal class CoProFilterProvider : IVsTextViewCreationListener//classifies as an extension
    {
        [Export(typeof(AdornmentLayerDefinition))]
        [Name("MultiEditLayer")]
        [TextViewRole(PredefinedTextViewRoles.Editable)]
        internal AdornmentLayerDefinition m_multieditAdornmentLayer = null;
        [Import(typeof(IVsEditorAdaptersFactoryService))]
        internal IVsEditorAdaptersFactoryService editorFactory = null;
        static bool mySideCalling = true;// determines whether the invoke of an event is from our side or a network one 
        ProjectItemsEvents pie;// ProjectItems events object
        SolutionEvents se;// SolutionEvents events object
        DebuggerEvents de;// DebuggerEvents events object
        static CoProNetwork cb;// netowrk client object
        GraphicObjects gobj;// GraphicObjects object used to give access to dte objects and explorer
        public static bool isFirst = true; // flag for the load of the package for a specific project
        bool runFlag = false;// flag to determine if code is running or not

        /// <summary>
        /// Properties for members
        /// </summary>
        public static bool MySide
        {
            get { return mySideCalling; }
            set { mySideCalling = value; }
        }

        /// <summary>
        /// This function is called whenever a new item to edit is opened
        /// </summary>
        /// <param name="textViewAdapter">adapter of the text view (given by the VS itself [MEF] )</param>
        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            if (!runFlag)
            {
                cb = CoProgrammerPackage.cb;// gets current network object
                if (cb == null)
                {
                    cb = new CoProNetwork();//if null create new
                    CoProgrammerPackage.cb = cb;
                }
                ToolWindowPane window = CoProgrammerPackage.currRunning.FindToolWindow(typeof(CoProToolWindow), 0, true);
                if ((null == window) || (null == window.Frame))
                {
                    throw new NotSupportedException(Resources.CanNotCreateWindow);
                }
                gobj = new GraphicObjects(GetCurrentViewHost(textViewAdapter), cb, (CoProExplorer)window.Content);
                gobj.CoProExplorer.SetConnection(cb);//updates co pro explorer window 
                if (isFirst)// on opening the solution
                {
                    se = ((Events2)gobj.DTE2.Events).SolutionEvents;
                    de = ((Events2)gobj.DTE2.Events).DebuggerEvents;
                    se.Opened += SubscribeGlobalEvents;//solution opened event
                    de.OnEnterRunMode += new _dispDebuggerEvents_OnEnterRunModeEventHandler(OnRun);//when running the code event
                    bool setAdminConfiguraions = false;
                    var slnName = gobj.DTE2.Solution.FullName;
                    var adminFile = slnName.Substring(0, slnName.Substring(0, slnName.LastIndexOf('\\')).LastIndexOf('\\')) + "\\admin.txt";
                    //IF THERE IS ADMIN INFO
                    if (File.Exists(adminFile))//internal file that was created
                    {
                        StreamReader sr = new StreamReader(adminFile);
                        string dir = sr.ReadToEnd();
                        if (dir.Split('\n')[0] == slnName.Substring(0, slnName.LastIndexOf('\\')))
                        {
                            CoProgrammerPackage.service = new CoProServer();// runs the server
                            string filesDir = slnName.Substring(0, slnName.LastIndexOf('\\')) + "\\CoProFiles";
                            if (!Directory.Exists(filesDir))
                            {
                                Directory.CreateDirectory(slnName.Substring(0, slnName.LastIndexOf('\\')) + "\\CoProFiles");
                            }
                            string path = slnName.Substring(0, slnName.LastIndexOf('\\'));
                            XElement xe = CoProgrammerPackage.CreateFileSystemXmlTree(path, 1, path.Substring(path.LastIndexOf('\\') + 1));
                            XmlTextWriter xwr = new XmlTextWriter(path + "\\CoProFiles\\timestamps.xml", System.Text.Encoding.UTF8);// timestamps file creation
                            xwr.Formatting = Formatting.Indented;
                            xe.WriteTo(xwr);
                            xwr.Close();
                            FileStream fs = File.Create(slnName.Substring(0, slnName.LastIndexOf('\\')) + "\\CoProFiles\\client.txt");
                            string ipPort = "localhost:" + (CoProgrammerPackage.service.PortOfService() + 10).ToString() + ":" + dir.Split('\n')[1];
                            fs.Write(Encoding.ASCII.GetBytes(ipPort), 0, ipPort.Length);// creating client file if there is no one exsisting
                            fs.Close();
                            setAdminConfiguraions = true;
                        }
                    }
                    //IF THERE IS A CLIENT INFO
                    if (File.Exists(slnName.Substring(0, slnName.LastIndexOf('\\')) + "\\CoProFiles\\client.txt"))// internal client info file
                    {
                        StreamReader sr = new StreamReader(slnName.Substring(0, slnName.LastIndexOf('\\')) + "\\CoProFiles\\client.txt");
                        string iportName = sr.ReadToEnd();
                        cb.SetIpPort(iportName.Split(':')[0], iportName.Split(':')[1]);
                        cb.Name = iportName.Split(':')[2];// configurates the class accoridng to info in file
                        if (cb.Connect())// attempt to connect
                        {
                            cb.ProjPath = slnName.Substring(0, slnName.LastIndexOf('\\'));
                            if (setAdminConfiguraions)
                            {
                                if (cb.SetAdmin(true))
                                {
                                    cb.IsAdmin = true;// if is admin, he gets the access to some funcitons 
                                    cb.SetProjectDir(slnName.Substring(0, slnName.LastIndexOf('\\')));
                                }
                            }
                            else
                            {
                                cb.IsAdmin = false;
                                cb.UpdateProject();// updates project for regular clients
                            }
                            if (cb.IsAdmin)
                            {
                                cb.ExpectedSequence++;// +1 to current count toget next messages
                            }
                            gobj.CoProExplorer.UpdateInfo();// updates window info
                        }
                    }
                    else
                    {
                        cb = null;
                        gobj = null;
                    }
                    isFirst = false;
                }

                IWpfTextView textView = editorFactory.GetWpfTextView(textViewAdapter);//gets the text view
                if (textView == null)
                    return;
                AddCommandFilter(textViewAdapter, new CoProCommandFilter(textView, cb, gobj));//adds an instance of our command filter to the text view
            }
            runFlag = false;
        }

        /// <summary>
        /// Subscribes to golbal events of solutions
        /// </summary>
        private void SubscribeGlobalEvents()
        {
            pie = ((Events2)gobj.DTE2.Events).ProjectItemsEvents;
            pie.ItemAdded += ItemAdded;
            pie.ItemRemoved += ItemRemoved;
        }

        /// <summary>
        /// Handler of adding an item
        /// </summary>
        /// <param name="pi">the item that was added</param>
        private void ItemAdded(ProjectItem pi)
        {
            if (mySideCalling)
            {
                if (pi.Kind == "{6BB5F8EE-4483-11D3-8BCF-00C04F8EC28C}")
                {
                    string name = pi.FileNames[1];
                    byte[] content = File.ReadAllBytes(name);
                    string removeString = cb.ProjPath;
                    int index = name.IndexOf(removeString);
                    int length = removeString.Length;
                    String startOfString = name.Substring(0, index);
                    String endOfString = name.Substring(index + length);
                    String cleanPath = startOfString + endOfString;
                    if (cb.IsAdmin)
                    {
                        XElement xe = CoProgrammerPackage.CreateFileSystemXmlTree(cb.ProjPath, 1, cb.ProjPath.Substring(cb.ProjPath.LastIndexOf('\\') + 1));
                        XmlTextWriter xwr = new XmlTextWriter(cb.ProjPath + "\\CoProFiles\\timestamps.xml", System.Text.Encoding.UTF8);
                        xwr.Formatting = Formatting.Indented;
                        xe.WriteTo(xwr);
                        xwr.Close();
                    }
                    cb.NewItemAdded(cleanPath, content, name.Substring(name.LastIndexOf('\\') + 1), pi.ContainingProject.Name);
                }
            }
        }

        /// <summary>
        /// Hnadler of removing item
        /// </summary>
        /// <param name="pi">the item that was removed</param>
        private void ItemRemoved(ProjectItem pi)
        {
            if (mySideCalling)
            {
                if (pi.Kind == "{6BB5F8EE-4483-11D3-8BCF-00C04F8EC28C}")
                {
                    string name = pi.FileNames[1];
                    name = name.Substring(name.LastIndexOf('\\') + 1);
                    bool isDeleted = true;
                    if (File.Exists(pi.FileNames[1]))
                    {
                        isDeleted = true;
                    }
                    cb.NewItemRemoved(name, pi.ContainingProject.Name, isDeleted);
                }
            }
        }

        /// <summary>
        /// When the code is running
        /// </summary>
        /// <param name="d"></param>
        private void OnRun(dbgEventReason d)
        {
            runFlag = true;
        }

        /// <summary>
        /// gets the current text view for dte purposes
        /// </summary>
        /// <param name="vTextView"></param>
        /// <returns></returns>
        IWpfTextViewHost GetCurrentViewHost(IVsTextView vTextView)
        {
            IVsUserData userData = vTextView as IVsUserData;
            if (userData == null)
            {
                return null;
            }
            else
            {
                IWpfTextViewHost viewHost;
                object holder;
                Guid guidViewHost = DefGuidList.guidIWpfTextViewHost;
                userData.GetData(ref guidViewHost, out holder);
                viewHost = (IWpfTextViewHost)holder;
                return viewHost;
            }
        }

        /// <summary>
        /// adds the packge adornment layer the the entire structure of vs textview 
        /// </summary>
        /// <param name="viewAdapter"></param>
        /// <param name="commandFilter"></param>
        void AddCommandFilter(IVsTextView viewAdapter, CoProCommandFilter commandFilter)
        {
            if (commandFilter.m_added == false)
            {
                //get the view adapter from the editor factory
                IOleCommandTarget next;
                int hr = viewAdapter.AddCommandFilter(commandFilter, out next);

                if (hr == VSConstants.S_OK)
                {
                    commandFilter.m_added = true;
                    //you'll need the next target for Exec and QueryStatus
                    if (next != null)
                        commandFilter.m_nextTarget = next;
                }
            }
        }

    }
}
