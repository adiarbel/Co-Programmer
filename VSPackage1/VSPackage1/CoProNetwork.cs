using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Company.VSPackage1.ServiceReference1;
using System.ServiceModel;
using Microsoft.VisualStudio.Text;
using NetFwTypeLib;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Xml.Linq;



namespace Company.VSPackage1
{
    public delegate void RemovedTextEventHandler(object sender, EditedTextEventArgs e);
    public delegate void NewTextEventHandler(object sender, EditedTextEventArgs e);
    public delegate void EditorDisconnectedEventHandler(object sender, EditorDisEventArgs e);
    public delegate void AddCurrentEditorsEventHandler(object sender, AddEditorsEventArgs e);
    public delegate void NewCaretEventHandler(object sender, EditedTextEventArgs e);
    public delegate void ChangeCaretEventHandler(object sender, EditedTextEventArgs e);
    public delegate void SaveEventHandler(object sender, ChangeCaretEventArgs e);
    public delegate void AdminEventHandler(object sender, AdminEventArgs e);
    public delegate void ExplorerInfoEventHandler(object sender, ExplorerInfoEventArgs e);
    public delegate void NewItemAddedEventHandler(object sender, NewItemAddedEventArgs e);
    public delegate void ItemRemovedEventHandler(object sender, ItemRemovedEventArgs e);


    [CallbackBehavior(UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class CoProNetwork : ICoProServiceCallback, IDisposable
    {
        public event NewCaretEventHandler NewCaret; //Event for new editors
        public event RemovedTextEventHandler RemovedText; //Event for removed text while editing
        public event ChangeCaretEventHandler ChangeCaret; // Event for changing the position of an editor
        public event AddCurrentEditorsEventHandler AddAllEditors; //Event for adding the current editors
        public event EditorDisconnectedEventHandler EditorDisc; //Event for new disconnected editor
        public event NewTextEventHandler NewText; //Event for new added text
        public event SaveEventHandler SaveEvent;//Event for saving action
        public event AdminEventHandler AdminEvent; //Event for adming calling actions
        public event ExplorerInfoEventHandler ExplorerInfoEvent;// Event for changing explorer info due to network changes 
        public event NewItemAddedEventHandler ItemAdded;// Event for adding new items
        public event ItemRemovedEventHandler ItemRemoved;//  Event for removing items


        public static Object locker = new Object();//Locker for public resources
        InstanceContext context;//Service client infrastructure objects
        EndpointAddress myEndPoint;//
        NetTcpBinding mybinding;//
        CoProServiceClient wcfclient;//service client object
        int expecSeq;// expected message sequence number
        bool isAdmin;// determines access level of user
        string name;// editor name
        string proj;// project path on the current endpoint
        string[] iport = new string[2];// ip and port of the server

        /// <summary>
        /// Properties of members
        /// </summary>
        public string ProjPath
        {
            get { return proj; }
            set { proj = value; }

        }
        public string GetId()
        {
            string st = wcfclient.InnerDuplexChannel.SessionId;
            return st;
        }
        public int ExpectedSequence
        {
            get { return expecSeq; }
            set { expecSeq = value; }
        }
        public bool IsAdmin
        {
            get { return isAdmin; }
            set { isAdmin = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string[] IPort
        {
            get { return iport; }
        }

        /// <summary>
        /// Initialize the main members of the client 
        /// </summary>
        public CoProNetwork()
        {
            context = new InstanceContext(this);
            mybinding = new NetTcpBinding();
            mybinding.PortSharingEnabled = true;
            //mybinding.ReceiveTimeout = new TimeSpan(0, 10, 0);
            //mybinding.SendTimeout = new TimeSpan(0, 10, 0);
            mybinding.Security.Mode = SecurityMode.None;
        }
        /// <summary>
        /// Mediator funciton for service SetAdmin function
        /// </summary>
        /// <param name="adm">is admin or not </param>
        /// <returns>if setting was successful</returns>
        public bool SetAdmin(bool adm)
        {
            return wcfclient.SetAdmin(adm);
        }

        /// <summary>
        /// Mediator funciton for service SetProjectDir function
        /// </summary>
        /// <param name="dir">the project directory</param>
        public void SetProjectDir(string dir)
        {
            wcfclient.SetProjectDir(dir);
        }

        /// <summary>
        /// Setter for ip and port
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void SetIpPort(string ip, string port)
        {
            iport[0] = ip;
            iport[1] = port;
        }

        /// <summary>
        /// Connection to the service and checking for response
        /// </summary>
        /// <returns>true if suceeds, otherwise false</returns>
        public bool Connect()
        {
            myEndPoint = new EndpointAddress("net.tcp://" + iport[0] + ":" + iport[1] + "/CoProService");
            wcfclient = new ServiceReference1.CoProServiceClient(context, mybinding, myEndPoint);
            try
            {
                ExpectedSequence = wcfclient.GetExpectedSeq() - 1; //insert function that gets the id
                return wcfclient.IsConnected();
            }
            catch (Exception e)
            {
                return false;
            }

        }

        /// <summary>
        /// Mediator function for service InitializePosition function
        /// </summary>
        /// <param name="file">current file of editor</param>
        /// <param name="position">position within the file</param>
        /// <param name="name">the name of the editor</param>
        public void IntializePosition(string file, int position, string name)
        {
            wcfclient.IntializePosition(file, position, name);
        }

        /// <summary>
        /// Mediator function for service SendCaretPosition function
        /// </summary>
        /// <param name="file">current file of editor</param>
        /// <param name="position">position within the file</param>
        /// <param name="command">command that was done</param>
        public void SendCaretPosition(string file, int position, string command)
        {
            wcfclient.SendCaretPosition(file, position, command);
        }

        /// <summary>
        /// Mediator function for service NewItemAdded function
        /// </summary>
        /// <param name="relpath"></param>
        /// <param name="content"></param>
        /// <param name="name"></param>
        /// <param name="project"></param>
        public void NewItemAdded(string relpath, byte[] content, string name, string project)
        {
            wcfclient.NewItemAdded(relpath, content, name, project);
        }

        /// <summary>
        /// Mediator function for service NewItemRemoved function
        /// </summary>
        /// <param name="name"></param>
        /// <param name="project"></param>
        /// <param name="isDeleted"></param>
        public void NewItemRemoved(string name, string project, bool isDeleted)
        {
            wcfclient.NewItemRemoved(name, project, isDeleted);
        }

        /// <summary>
        /// Mediator function for service GetProject function
        /// </summary>
        /// <param name="name"></param>
        /// <returns>result of attempt to get the project</returns>
        public bool GetProject(string name)
        {
            bool ret = wcfclient.GetProject(name);
            wcfclient.Abort();
            return ret;
        }

        
        public void AddCurrentEditors(string[] editors, string[] locations, string[] names)
        {
            if (AddAllEditors != null)
            {
                AddAllEditors(this, new AddEditorsEventArgs(editors, locations));
                ExpectedSequence++;
            }
            if (ExplorerInfoEvent != null)
            {
                ExplorerInfoEvent(this, new ExplorerInfoEventArgs("", names));
            }
        }
        public void NewEditorAdded(string file, int position, string editor, int seq, string name)
        {
            if (NewCaret != null)
            {
                NewCaret(this, new EditedTextEventArgs(editor, position, file, " ", seq));
                ExpectedSequence++;
            }
            if (ExplorerInfoEvent != null)
            {
                ExplorerInfoEvent(this, new ExplorerInfoEventArgs(name, null));
            }
        }
        public void ChangedCaret(string file, int position, string editor, int seq)
        {
            if (ChangeCaret != null)
            {
                ChangeCaret(this, new EditedTextEventArgs(editor, position, file, " ", seq));
                ExpectedSequence++;
            }
        }
        public void EditorDisconnected(string editor)
        {
            if (EditorDisc != null)
            {
                EditorDisc(this, new EditorDisEventArgs(editor));
            }
        }
        public void NewAddedText(string file, int position, string editor, string content, int seq)
        {
            if (NewText != null)
            {
                NewText(this, new EditedTextEventArgs(editor, position, file, content, seq)); //timeout exception
                ExpectedSequence++;
            }

        }
        public void NewRemovedText(string file, int position, string editor, string instruc, int seq)
        {
            if (RemovedText != null)
            {

                RemovedText(this, new EditedTextEventArgs(editor, position, file, instruc, seq));
                ExpectedSequence++;
            }

        }
        public void Save(string file)
        {
            if (SaveEvent != null)
            {
                SaveEvent(this, new ChangeCaretEventArgs(" ", 0, file, " "));
            }
        }

        /// <summary>
        /// disposes the client object
        /// </summary>
        void IDisposable.Dispose()
        {
            wcfclient.Close();
        }

        public void CloneProject(string fileName, byte[] zipFile)
        {
            File.WriteAllBytes(proj + "\\proj.zip", zipFile);
            ZipFile.ExtractToDirectory(proj + "\\proj.zip", proj + "\\" + fileName);
            File.Delete(proj + "\\proj.zip");
            if (!Directory.Exists(proj + "\\" + fileName + "\\CoProFiles"))
            {
                Directory.CreateDirectory(proj + "\\" + fileName + "\\CoProFiles");
            }
            FileStream fs = File.Create(proj + "\\" + fileName + "\\CoProFiles" + "\\client.txt");
            fs.Write(Encoding.ASCII.GetBytes(iport[0] + ':' + iport[1] + ':' + name), 0, (iport[0] + ':' + iport[1] + ':' + name).Length);
            fs.Close();
        }
        public bool ApproveCloning(string nameToApprove, string idToApprove)
        {
            System.Windows.Forms.DialogResult result = System.Windows.Forms.MessageBox.Show("Do you want to approve cloning for the following?:\n" + nameToApprove, "Confirmation", System.Windows.Forms.MessageBoxButtons.YesNoCancel);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                wcfclient.ShareProject(proj, proj.Substring(proj.LastIndexOf('\\') + 1), idToApprove);
                return true;
            }
            return false;

        }
        /// <summary>
        /// aborts client object when needed
        /// </summary>
        public void Abort()
        {
            wcfclient.Abort();
        }

        /// <summary>
        /// Mediator function for service funciton UpdateProject
        /// </summary>
        public void UpdateProject()
        {
            wcfclient.UpdateProject();
        }

        
        public string[][] UpdateProjFilesCallback(string file)
        {
            List<string> filesToRequest = new List<string>();
            List<string> newFilesToRequest = new List<string>();
            XDocument xd = XDocument.Load(ProjPath + "\\CoProFiles\\timestamps.xml");
            bool cond = xd.ToString() != file;
            if (cond)
            {
                XmlTextReader xr = new XmlTextReader(new System.IO.StringReader(file));
                Dictionary<string, string[]> serverDictionary = new Dictionary<string, string[]>();
                while (xr.Read())
                {
                    xr.MoveToContent();
                    if (xr.NodeType == System.Xml.XmlNodeType.Element && xr.Name == "File")
                    {
                        serverDictionary[xr.GetAttribute(0)] = new string[2];
                        serverDictionary[xr.GetAttribute(0)][0] = xr.GetAttribute(1);
                        serverDictionary[xr.GetAttribute(0)][1] = xr.GetAttribute(2);
                    }
                }
                xr.Close();
                xr = new XmlTextReader(ProjPath + "\\CoProFiles\\timestamps.xml");
                Dictionary<string, string[]> myDictionary = new Dictionary<string, string[]>();
                while (xr.Read())
                {
                    xr.MoveToContent();
                    if (xr.NodeType == System.Xml.XmlNodeType.Element && xr.Name == "File")
                    {
                        myDictionary[xr.GetAttribute(0)] = new string[2];
                        myDictionary[xr.GetAttribute(0)][0] = xr.GetAttribute(1);
                        myDictionary[xr.GetAttribute(0)][1] = xr.GetAttribute(2);
                    }
                }
                xr.Close();
                for (int i = 0; i < serverDictionary.Keys.Count; i++)
                {
                    string tempKey = serverDictionary.Keys.ElementAt(i);
                    if (!myDictionary.ContainsKey(tempKey))
                    {
                        newFilesToRequest.Add(serverDictionary[tempKey][1] + "\\" + tempKey);
                    }
                    else if (myDictionary[tempKey][0] != serverDictionary[tempKey][0])
                    {
                        filesToRequest.Add(serverDictionary[tempKey][1] + "\\" + tempKey);
                    }
                }

            }
            string[][] arrs = { filesToRequest.ToArray(), newFilesToRequest.ToArray() };
            return arrs;
        }

        /// <summary>
        /// Mediator function to service UpdateSpecificFile function
        /// </summary>
        /// <param name="relPath">relative path of the file to update</param>
        public void UpdateSpecificFile(string relPath)
        {
            wcfclient.UpdateSpecificFile(relPath);
        }

        public void UpdateProjFilesContents(string[] files, byte[][] contents, string[] n_files, byte[][] n_contents)
        {
            string absolutePath = ProjPath.Substring(0, ProjPath.LastIndexOf('\\'));
            string project;
            string name;
            for (int i = 0; i < files.Length; i++)
            {
                if (File.Exists(absolutePath + "\\" + files[i]))
                {
                    using (StreamWriter sr = new StreamWriter(absolutePath + "\\" + files[i], false))
                    {
                        sr.Write(System.Text.Encoding.UTF8.GetString(contents[i]));
                        sr.Flush();
                    }
                }
                else
                {
                    File.WriteAllBytes(absolutePath + "\\" + files[i], contents[i]);
                }

            }
            for (int i = 0; i < n_files.Length; i++)
            {
                File.WriteAllBytes(absolutePath + "\\" + n_files[i], n_contents[i]);
            }
            File.WriteAllBytes(ProjPath + "\\CoProFiles\\timestamps.xml", contents[files.Length]);
            ExpectedSequence++;
        }

        public void UpdateSpecificFileCallback(byte[] content, string relPath)
        {
            File.WriteAllBytes(ProjPath + relPath.Substring(relPath.IndexOf('\\')), content);
        }

        public void NewItemRemovedCallback(string name, string project, bool isDeleted)
        {
            if(ItemRemoved!=null)
            {
                ItemRemoved(this, new ItemRemovedEventArgs(name, project, isDeleted));
            }
        }

        public void NewItemAddedCallback(string relpath, byte[] content, string name, string project)
        {
            if(ItemAdded!=null)
            {
                ItemAdded(this, new NewItemAddedEventArgs(relpath, content, name, project));
            }
        }

        public void AdminFileOpen(string file)
        {
            if (AdminEvent != null)
            {
                AdminEvent(this, new AdminEventArgs(file));
            }
        }


    }

    /// <summary>
    /// Events arguments classes
    /// </summary>
    public class ChangeCaretEventArgs : EventArgs
    {
        // Fields
        protected string m_editor = string.Empty;
        protected int m_location = -1;
        protected string m_file = string.Empty;
        protected string m_command = string.Empty;

        // Constructor
        public ChangeCaretEventArgs(string editor, int location, string file, string command)
        {
            m_editor = editor;
            m_location = location;
            m_file = file;
            m_command = command;
        }
        // Properties (read-only)
        public string Editor
        {
            get { return m_editor; }
        }
        public int Location
        {
            get { return m_location; }
        }
        public string File
        {
            get { return m_file; }
        }
        public string Command
        {
            get { return m_command; }
        }

    }
    public class EditedTextEventArgs : ChangeCaretEventArgs
    {
        // Fields
        private int m_seq = -1;
        // Constructor
        public EditedTextEventArgs(string editor, int location, string file, string command, int seq)
            : base(editor, location, file, command)
        {
            m_seq = seq;
        }
        // Properties (read-only)

        public int Seq
        {
            get { return m_seq; }
        }
    }
    public class AddEditorsEventArgs : EventArgs
    {
        private string[] m_editors = null;
        private string[] m_locations = null;

        public AddEditorsEventArgs(string[] editors, string[] locations)
        {
            m_editors = editors;
            m_locations = locations;
        }
        public string[] Editors
        {
            get { return m_editors; }
        }
        public string[] Locations
        {
            get { return m_locations; }
        }
    }
    public class EditorDisEventArgs : EventArgs
    {

        private string m_editor = string.Empty;
        public EditorDisEventArgs(string editor)
        {
            m_editor = editor;
        }
        public string Editor
        {
            get { return m_editor; }
        }
    }
    public class AdminEventArgs : EventArgs
    {
        private string m_file = string.Empty;
        public AdminEventArgs(string file)
        {
            m_file = file;
        }
        public string File
        {
            get { return m_file; }
        }
    }
    public class ExplorerInfoEventArgs : EventArgs
    {
        private string m_name = null;
        private string[] m_names = null;

        public ExplorerInfoEventArgs(string name, string[] names)
        {
            m_name = name;
            m_names = names;
        }
        public string Name
        {
            get { return m_name; }
        }
        public string[] Names
        {
            get { return m_names; }
        }
    }
    public class NewItemAddedEventArgs : EventArgs
    {
        string m_relpath;
        byte[] m_content;
        string m_name;
        string m_project;

        public NewItemAddedEventArgs(string relpath, byte[] content, string name, string project)
        {
            m_relpath = relpath;
            m_content = content;
            m_name = name;
            m_project = project;
        }
        public string RelPath
        {
            get { return m_relpath; }
        }
        public byte[] Content
        {
            get { return m_content; }
        }
        public string Name
        {
            get { return m_name; }
        }
        public string Project
        {
            get { return m_project; }
        }

    }
    public class ItemRemovedEventArgs : EventArgs
    {
        string m_name;
        string m_project;
        bool m_isDeleted;

        public ItemRemovedEventArgs(string name, string project,bool isDeleted)
        {
            m_isDeleted = isDeleted;
            m_name = name;
            m_project = project;
        }
        public bool IsDeleted
        {
            get { return m_isDeleted; }
        }
        public string Name
        {
            get { return m_name; }
        }
        public string Project
        {
            get { return m_project; }
        }

    }
}
