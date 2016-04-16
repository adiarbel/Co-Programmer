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

namespace Company.VSPackage1
{
    /*TODO: define delegate*/
    public delegate void RemovedTextEventHandler(object sender, EditedTextEventArgs e);
    public delegate void NewTextEventHandler(object sender, EditedTextEventArgs e);
    public delegate void EditorDisconnectedEventHandler(object sender, EditorDisEventArgs e);
    public delegate void AddCurrentEditorsEventHandler(object sender, AddEditorsEventArgs e);
    public delegate void NewCaretEventHandler(object sender, ChangeCaretEventArgs e);
    public delegate void ChangeCaretEventHandler(object sender, ChangeCaretEventArgs e);
    public delegate void SaveEventHandler(object sender, ChangeCaretEventArgs e);
    /*TODO: define event for that delegate*/

    /*TODO: define the above for each event that might come from the server's callbacks*/
    [CallbackBehavior(UseSynchronizationContext = false, ConcurrencyMode=ConcurrencyMode.Reentrant)]
    public class MyCallBack : ICoProServiceCallback, IDisposable
    {
        public event NewCaretEventHandler NewCaret;
        public event RemovedTextEventHandler RemovedText;
        public event ChangeCaretEventHandler ChangeCaret;
        public event AddCurrentEditorsEventHandler AddAllEditors;
        public event EditorDisconnectedEventHandler EditorDisc;
        public event NewTextEventHandler NewText;
        public event SaveEventHandler save;
        public static Object locker = new Object();
        InstanceContext context;
        EndpointAddress myEndPoint;
        NetTcpBinding mybinding;
        CoProServiceClient wcfclient;
        int expecSeq;
        string proj;
        string[] iport = new string[2];
        public int ExpectedSequence
        {
            get { return expecSeq; }
            set { expecSeq = value; }
        }
        public MyCallBack()
        {
            context = new InstanceContext(this);
            mybinding = new NetTcpBinding();
            mybinding.PortSharingEnabled = true;
            mybinding.Security.Mode = SecurityMode.None;
        }
        public void SetProjPath(string path)
        {
            proj = path;

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
                ExpectedSequence = wcfclient.GetExpectedSeq(); //insert function that gets the id
                return wcfclient.IsConnected();
            }
            catch
            {
                return false;
            }
           
        }
        public void IntializePosition(string file, int position)
        {
            wcfclient.IntializePosition(file, position);
        }
        public void SendCaretPosition(string file, int position, string command)
        {
            wcfclient.SendCaretPosition(file, position, command);
        }
        public void GetProject()
        {
            wcfclient.GetProject();
            wcfclient.Abort();
        }
        public void AddCurrentEditors(string[] editors, string[] locations)
        {
            if (AddAllEditors != null)
            {
                AddAllEditors(this, new AddEditorsEventArgs(editors, locations));
            }
        }
        public void NewEditorAdded(string file, int position, string editor)
        {
            if (NewCaret != null)
            {
                NewCaret(this, new ChangeCaretEventArgs(editor, position, file, " "));
            }
        }
        public void ChangedCaret(string file, int position, string editor)
        {
            if (ChangeCaret != null)
            {
                ChangeCaret(this, new ChangeCaretEventArgs(editor, position, file, " "));
            }
        }
        public void EditorDisconnected(string editor)
        {
            if (EditorDisc != null)
            {
                EditorDisc(this, new EditorDisEventArgs(editor));
            }
        }
        public void NewAddedText(string file, int position, string editor, string content,int seq)
        {
            if (NewText != null)
            {
                NewText(this, new EditedTextEventArgs(editor, position, file, content,seq));
            }
        }
        public void NewRemovedText(string file, int position, string editor, string instruc,int seq)
        {
            if (RemovedText != null)
            {

                RemovedText(this, new EditedTextEventArgs(editor, position, file, instruc,seq));

            }
        }
        public void Save(string file)
        {
            if (save != null)
            {

                save(this, new ChangeCaretEventArgs(" ", 0, file, " "));

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
            FileStream fs =  File.Create(proj +"\\"+ fileName +"\\client.txt");
            fs.Write(Encoding.ASCII.GetBytes(iport[0]+':'+iport[1]),0,(iport[0]+':'+iport[1]).Length);
            fs.Close();
        }
        public void ApproveCloning(string[] idsToApprove)
        {
            string st="";
            for (int i = 0; i < idsToApprove.Length; i++)
            {
                st += idsToApprove[i] + '\n';
            }
            System.Windows.Forms.DialogResult result = System.Windows.Forms.MessageBox.Show("Do you want to approve cloning for the following?:\n"+st, "Confirmation", System.Windows.Forms.MessageBoxButtons.YesNoCancel);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                wcfclient.ShareProject(proj, proj.Substring(proj.LastIndexOf('\\')+1));
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
        public EditedTextEventArgs(string editor, int location, string file, string command,int seq):base(editor,location,file,command)
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
}
