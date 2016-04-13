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
    public delegate void RemovedTextEventHandler(object sender, ChangeCaretEventArgs e);
    public delegate void NewTextEventHandler(object sender, ChangeCaretEventArgs e);
    public delegate void EditorDisconnectedEventHandler(object sender, EditorDisEventArgs e);
    public delegate void AddCurrentEditorsEventHandler(object sender, AddEditorsEventArgs e);
    public delegate void NewCaretEventHandler(object sender, ChangeCaretEventArgs e);
    public delegate void ChangeCaretEventHandler(object sender, ChangeCaretEventArgs e);
    public delegate void SaveEventHandler(object sender, ChangeCaretEventArgs e);
    /*TODO: define event for that delegate*/

    /*TODO: define the above for each event that might come from the server's callbacks*/
    [CallbackBehavior(UseSynchronizationContext = false)]
    public class MyCallBack : ICoProServiceCallback, IDisposable
    {
        public event NewCaretEventHandler NewCaret;
        public event RemovedTextEventHandler RemovedText;
        public event ChangeCaretEventHandler ChangeCaret;
        public event AddCurrentEditorsEventHandler AddAllEditors;
        public event EditorDisconnectedEventHandler EditorDisc;
        public event NewTextEventHandler NewText;
        public event SaveEventHandler save;
        InstanceContext context;
        EndpointAddress myEndPoint;
        NetTcpBinding mybinding;
        CoProServiceClient wcfclient;
        string proj;
        string[] iport = new string[2];
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
        public void GetProject(string path, string name)
        {
            wcfclient.GetProject();
            wcfclient.ShareProject(path, name);
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
        public void NewAddedText(string file, int position, string editor, string content)
        {
            if (NewText != null)
            {
                NewText(this, new ChangeCaretEventArgs(editor, position, file, content));
            }
        }
        public void NewRemovedText(string file, int position, string editor, string instruc)
        {
            if (RemovedText != null)
            {

                RemovedText(this, new ChangeCaretEventArgs(editor, position, file, instruc));

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

        }
    }
    public class ChangeCaretEventArgs : EventArgs
    {
        // Fields
        private string m_editor = string.Empty;
        private int m_location = -1;
        private string m_file = string.Empty;
        private string m_command = string.Empty;

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
