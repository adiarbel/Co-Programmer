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
    /*TODO: define delegate*/
    public delegate void RemovedTextEventHandler(object sender, EditedTextEventArgs e);
    public delegate void NewTextEventHandler(object sender, EditedTextEventArgs e);
    public delegate void EditorDisconnectedEventHandler(object sender, EditorDisEventArgs e);
    public delegate void AddCurrentEditorsEventHandler(object sender, AddEditorsEventArgs e);
    public delegate void NewCaretEventHandler(object sender, EditedTextEventArgs e);
    public delegate void ChangeCaretEventHandler(object sender, EditedTextEventArgs e);
    public delegate void SaveEventHandler(object sender, ChangeCaretEventArgs e);
    public delegate void AdminEventHandler(object sender, AdminEventArgs e);
    public delegate void ExplorerInfoEventHandler(object sender, ExplorerInfoEventArgs e);
    /*TODO: define event for that delegate*/

    /*TODO: define the above for each event that might come from the server's callbacks*/
    [CallbackBehavior(UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class MyCallBack : ICoProServiceCallback, IDisposable
    {
        public event NewCaretEventHandler NewCaret;
        public event RemovedTextEventHandler RemovedText;
        public event ChangeCaretEventHandler ChangeCaret;
        public event AddCurrentEditorsEventHandler AddAllEditors;
        public event EditorDisconnectedEventHandler EditorDisc;
        public event NewTextEventHandler NewText;
        public event SaveEventHandler SaveEvent;
        public event AdminEventHandler AdminEvent;
        public event ExplorerInfoEventHandler ExplorerInfoEvent;


        public static Object locker = new Object();
        InstanceContext context;
        EndpointAddress myEndPoint;
        NetTcpBinding mybinding;
        CoProServiceClient wcfclient;
        int expecSeq;
        bool isAdmin;
        string name;
        string proj;
        EnvDTE80.DTE2 dte;
        string[] iport = new string[2];
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
        public EnvDTE80.DTE2 DTE2
        {
            get { return dte; }
            set { dte = value; }
        }
        public MyCallBack()
        {
            context = new InstanceContext(this);
            mybinding = new NetTcpBinding();
            mybinding.PortSharingEnabled = true;
            //mybinding.ReceiveTimeout = new TimeSpan(0, 10, 0);
            //mybinding.SendTimeout = new TimeSpan(0, 10, 0);
            mybinding.Security.Mode = SecurityMode.None;
        }
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
        public bool SetAdmin(bool adm)
        {
            return wcfclient.SetAdmin(adm);
        }
        public void SetProjectDir(string dir)
        {
            wcfclient.SetProjectDir(dir);
        }
        public void SetIpPort(string ip, string port)
        {
            iport[0] = ip;
            iport[1] = port;
        }
        public bool Connect()
        {
            myEndPoint = new EndpointAddress("net.tcp://" + iport[0] + ":" + iport[1] + "/CoProService");
            wcfclient = new ServiceReference1.CoProServiceClient(context, mybinding, myEndPoint);
            try
            {
                ExpectedSequence = wcfclient.GetExpectedSeq() - 1; //insert function that gets the id
                return wcfclient.IsConnected();
            }
            catch
            {
                return false;
            }

        }
        public void IntializePosition(string file, int position, string name)
        {
            wcfclient.IntializePosition(file, position, name);
        }
        public void SendCaretPosition(string file, int position, string command)
        {
            wcfclient.SendCaretPosition(file, position, command);
        }
        public void NewItemAdded(string relpath, byte[] content, string name, string project)
        {
            wcfclient.NewItemAdded(relpath, content, name, project);
        }
        public void GetProject(string name)
        {
            wcfclient.GetProject(name);
            wcfclient.Abort();
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
        public void ApproveCloning(string[] idsToApprove)
        {
            string st = "";
            for (int i = 0; i < idsToApprove.Length; i++)
            {
                st += idsToApprove[i] + '\n';
            }
            System.Windows.Forms.DialogResult result = System.Windows.Forms.MessageBox.Show("Do you want to approve cloning for the following?:\n" + st, "Confirmation", System.Windows.Forms.MessageBoxButtons.YesNoCancel);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                wcfclient.ShareProject(proj, proj.Substring(proj.LastIndexOf('\\') + 1));
            }

        }
        public void Abort()
        {
            wcfclient.Abort();
        }
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
                //for (int i = 0; i < myDictionary.Keys.Count; i++)
                //{
                //    string tempKey = myDictionary.Keys.ElementAt(i);
                //    if (!serverDictionary.ContainsKey(tempKey))
                //    {

                //    }
                //}
                int dsa = 1;
            }
            string[][] arrs = { filesToRequest.ToArray(), newFilesToRequest.ToArray() };
            return arrs;
        }
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
                //project = n_files[i].Substring(n_files[i].IndexOf('\\') + 1);
                //project = project.Substring(0, project.LastIndexOf('\\'));
                //File.WriteAllBytes(absolutePath + "\\" + n_files[i], n_contents[i]);
                //name = absolutePath + "\\" + n_files[i];
                //MultiEditFilterProvider.MySide = false;
                //EnvDTE.Projects ps = DTE2.Solution.Projects;
                //foreach(EnvDTE.Project p in ps)
                //{
                //    string pname = p.Name;
                //    if(p.Name.Contains(project))
                //    {
                //        p.ProjectItems.AddFromTemplate(name, name.Substring(name.LastIndexOf('\\') + 1));
                //    }
                //}
                //MultiEditFilterProvider.MySide = true; ;

            }
            File.WriteAllBytes(ProjPath + "\\CoProFiles\\timestamps.xml", contents[files.Length]);
            ExpectedSequence++;
        }
        public void UpdateSpecificFileCallback(byte[] content, string relPath)
        {
            File.WriteAllBytes(ProjPath + relPath.Substring(relPath.IndexOf('\\')), content);
        }

        public void NewItemRemovedCallback(string name, string project)
        {
            DTE2.Solution.Projects.Item(project).ProjectItems.Item(name).Remove();
        }

        public void NewItemAddedCallback(string relpath, byte[] content, string name, string project)
        {
            File.WriteAllBytes(ProjPath + relpath, content);
            MultiEditFilterProvider.MySide = false;
            EnvDTE.Projects ps = DTE2.Solution.Projects;
            foreach (EnvDTE.Project p in ps)
            {
                string pname = p.Name;
                if (p.Name.Contains(project))
                {
                    p.ProjectItems.AddFromTemplate(ProjPath + relpath, name);
                    break;
                }
            }
            MultiEditFilterProvider.MySide = true;
        }

        public void AdminFileOpen(string file)
        {
            if (AdminEvent != null)
            {
                AdminEvent(this, new AdminEventArgs(file));
            }
        }


    }
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

}
