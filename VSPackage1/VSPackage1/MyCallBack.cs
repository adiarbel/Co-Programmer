using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Company.VSPackage1.ServiceReference1;
using System.ServiceModel;
using Microsoft.VisualStudio.Text;

namespace Company.VSPackage1
{
    /*TODO: define delegate*/
    public delegate void AddCurrentEditorsEventHandler(object sender, AddEditorsEventArgs e);
    public delegate void NewCaretEventHandler(object sender, ChangeCaretEventArgs e);
    public delegate void ChangeCaretEventHandler(object sender, ChangeCaretEventArgs e);
    /*TODO: define event for that delegate*/

    /*TODO: define the above for each event that might come from the server's callbacks*/
    [CallbackBehavior(UseSynchronizationContext = false)]
    class MyCallBack : ICoProServiceCallback, IDisposable
    {
        public event NewCaretEventHandler NewCaret;
        public event ChangeCaretEventHandler ChangeCaret;
        public event AddCurrentEditorsEventHandler AddAllEditors;
        InstanceContext context;
        EndpointAddress myEndPoint;
        NetTcpBinding mybinding;
        CoProServiceClient wcfclient;
        public MyCallBack()
        {
            context = new InstanceContext(this);
            mybinding = new NetTcpBinding();
            myEndPoint = new EndpointAddress("net.tcp://localhost:8090/CoProService");
            wcfclient = new ServiceReference1.CoProServiceClient(context, mybinding, myEndPoint);
        }
        public void IntializePosition(string file, int position)
        {
            wcfclient.IntializePosition(file, position);
        }
        public void SendCaretPosition(string file, int position, string command)
        {
            wcfclient.SendCaretPosition(file, position, command);
        }
        public void AddCurrentEditors(string[] editors, string[] locations)
        {
            if(AddAllEditors!=null)
            {
                AddAllEditors(this, new AddEditorsEventArgs(editors, locations));
            }
        }
        public void NewEditorAdded(string file, int position, string editor)
        {
            if (NewCaret != null)
            {
                NewCaret(this, new ChangeCaretEventArgs(editor, position.ToString(), file, " "));
            }
        }
        public void ChangedCaret(string file, int position, string editor)
        {
            if (ChangeCaret != null)
            {
                ChangeCaret(this, new ChangeCaretEventArgs(editor, position.ToString(), file, " "));
            }
        }
        public void EditorDisconnected(string editor)
        {
            throw new NotImplementedException();
        }
        public void NewAddedText(string file, int position, string editor, string content)
        {
            throw new NotImplementedException();
        }
        public void NewRemovedText(string file, int position, string editor, int end_position)
        {
            throw new NotImplementedException();
        }
        void IDisposable.Dispose()
        {
            wcfclient.Close();
        }
    }
    public class ChangeCaretEventArgs : EventArgs
    {
        // Fields
        private string m_sender = string.Empty;
        private string m_location = string.Empty;
        private string m_file = string.Empty;
        private string m_command = string.Empty;

        // Constructor
        public ChangeCaretEventArgs(string sender, string location, string file, string command)
        {
            m_sender = sender;
            m_location = location;
            m_file = file;
            m_command = command;
        }
        // Properties (read-only)
        public string Sender
        {
            get { return m_sender; }
        }
        public string Location
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

}
