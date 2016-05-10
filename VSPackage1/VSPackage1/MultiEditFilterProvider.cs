using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio;
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
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal class MultiEditFilterProvider : IVsTextViewCreationListener//classifies as an extension
    {
        [Export(typeof(AdornmentLayerDefinition))]
        [Name("MultiEditLayer")]
        [TextViewRole(PredefinedTextViewRoles.Editable)]
        internal AdornmentLayerDefinition m_multieditAdornmentLayer = null;
        [Import(typeof(IVsEditorAdaptersFactoryService))]
        internal IVsEditorAdaptersFactoryService editorFactory = null;
        static bool mySide = true;
        ProjectItemsEvents pie;
        SolutionEvents se;
        static MyCallBack cb = VSPackage1Package.cb;
        Carets cs;
        static bool isFirst = true;
        public static bool MySide
        {
            get { return mySide; }
            set { mySide = value; }
        }
        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            if (cb == null)
            {
                cb = new MyCallBack();
                VSPackage1Package.cb = cb;
            }
            cs = new Carets(GetCurrentViewHost(textViewAdapter), cb);
            cb.DTE2 = cs.DTE2;
            if (isFirst)
            {
                se = ((Events2)cs.DTE2.Events).SolutionEvents;
                se.Opened += SubscribeGlobalEvents;
                se.BeforeClosing += UnSubscribeGlobalEvents;
                bool setAdminConfiguraions = false;
                var slnName = cs.DTE2.Solution.FullName;
                var adminFile = slnName.Substring(0, slnName.Substring(0, slnName.LastIndexOf('\\')).LastIndexOf('\\')) + "\\admin.txt";
                if (File.Exists(adminFile))
                {
                    StreamReader sr = new StreamReader(adminFile);
                    string dir = sr.ReadToEnd();
                    if (dir.Split('\n')[0] == slnName.Substring(0, slnName.LastIndexOf('\\')))
                    {
                        VSPackage1Package.service = new Service();
                        string filesDir = slnName.Substring(0, slnName.LastIndexOf('\\')) + "\\CoProFiles";
                        if (!Directory.Exists(filesDir))
                        {
                            Directory.CreateDirectory(slnName.Substring(0, slnName.LastIndexOf('\\')) + "\\CoProFiles");
                        }
                        string path = slnName.Substring(0, slnName.LastIndexOf('\\'));
                        XElement xe = VSPackage1Package.CreateFileSystemXmlTree(path, 1, path.Substring(path.LastIndexOf('\\') + 1));
                        XmlTextWriter xwr = new XmlTextWriter(path + "\\CoProFiles\\timestamps.xml", System.Text.Encoding.UTF8);
                        xwr.Formatting = Formatting.Indented;
                        xe.WriteTo(xwr);
                        xwr.Close();
                        FileStream fs = File.Create(slnName.Substring(0, slnName.LastIndexOf('\\')) + "\\CoProFiles\\client.txt");
                        string ipPort = "localhost:" + (VSPackage1Package.service.PortOfService() + 10).ToString() + ":" + dir.Split('\n')[1];
                        fs.Write(Encoding.ASCII.GetBytes(ipPort), 0, ipPort.Length);
                        fs.Close();
                        setAdminConfiguraions = true;
                    }
                }
                //if (File.Exists(slnName.Substring(0, slnName.LastIndexOf('\\')) + "\\CoProFiles\\client.txt"))
                //{
                //    StreamReader sr = new StreamReader(slnName.Substring(0, slnName.LastIndexOf('\\')) + "\\CoProFiles\\client.txt");
                //    string iportName = sr.ReadToEnd();
                //    cb.SetIpPort(iportName.Split(':')[0], iportName.Split(':')[1]);
                //    cb.Name = iportName.Split(':')[2];
                //    if (cb.Connect())
                //    {
                //        cb.ProjPath = slnName.Substring(0, slnName.LastIndexOf('\\'));
                //        if (setAdminConfiguraions)
                //        {
                //            if (cb.SetAdmin(true))
                //            {
                //                cb.IsAdmin = true;
                //                cb.SetProjectDir(slnName.Substring(0, slnName.LastIndexOf('\\')));
                //            }
                //        }
                //        else
                //        {
                //            cb.IsAdmin = false;
                //            cb.UpdateProject();
                //        }
                //        if (cb.IsAdmin)
                //        {
                //            cb.ExpectedSequence++;
                //        }

                //    }
                //}
                //else
                //{
                //    cb = null;
                //    cs = null;
                //}
                //isFirst = false;
            }
            //IWpfTextView textView = editorFactory.GetWpfTextView(textViewAdapter);//gets the text view
            //if (textView == null)
            //    return;
            //AddCommandFilter(textViewAdapter, new MultiEditCommandFilter(textView, cb, cs));//adds an instance of our command filter to the text view
        }
        private void SubscribeGlobalEvents()
        {
            pie = ((Events2)cs.DTE2.Events).ProjectItemsEvents;
            pie.ItemAdded += ItemAdded;
            pie.ItemRemoved += ItemRemoved;
        }
        private void UnSubscribeGlobalEvents()
        {
            pie.ItemAdded -= ItemAdded;
            pie.ItemRemoved -= ItemRemoved;
        }
        private void ItemAdded(ProjectItem pi)
        {
            if (mySide)
            {

                string name = pi.FileNames[1];
                byte[] content = File.ReadAllBytes(name);
                string removeString = cb.ProjPath;
                int index = name.IndexOf(removeString);
                int length = removeString.Length;
                String startOfString = name.Substring(0, index);
                String endOfString = name.Substring(index + length);
                String cleanPath = startOfString + endOfString;
                cb.NewItemAdded(cleanPath, content, name, pi.ContainingProject.Name);
            }
        }
        private void ItemRemoved(ProjectItem pi)
        {
            if (mySide)
            {

                string name = pi.FileNames[1];
                name = name.Substring(name.LastIndexOf('\\')+1);
                
                //cs.DTE2.Solution.Projects.Item(1).ProjectItems.Item("ASD").Remove();
                //cb.NewItemAdded(cleanPath, content, name, pi.ContainingProject.Name);
            }
        }
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
        void AddCommandFilter(IVsTextView viewAdapter, MultiEditCommandFilter commandFilter)
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
