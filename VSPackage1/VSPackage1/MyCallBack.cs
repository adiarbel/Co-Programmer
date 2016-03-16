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
    public delegate void NewCaretEventHandler(object sender, ChangeCaretEventArgs e);
    public delegate void ChangeCaretEventHandler(object sender, ChangeCaretEventArgs e);
    /*TODO: define event for that delegate*/

    /*TODO: define the above for each event that might come from the server's callbacks*/
    [CallbackBehavior(UseSynchronizationContext = false)]
    class MyCallBack : IEditServiceCallback, IDisposable
    {
        public event NewCaretEventHandler NewCaret;
        public event ChangeCaretEventHandler ChangeCaret;
        InstanceContext context;
        EndpointAddress myEndPoint;
        NetTcpBinding mybinding;
        EditServiceClient wcfclient;
        public void CallBackFunction(string file, int position, string sender)
        {

            OnCaretChanged(file, position, sender);
        }
        public MyCallBack()
        {
            context = new InstanceContext(this);
            mybinding = new NetTcpBinding();
            myEndPoint = new EndpointAddress("net.tcp://localhost:8090/EditService");
            wcfclient = new ServiceReference1.EditServiceClient(context, mybinding, myEndPoint);
            PrintIds();
        }
        public void callService(string file, int position)
        {
            wcfclient.IntializePosition(file, position);
        }
        public void SendCurrPos(string file,int position)
        {
            wcfclient.SendCaretPosition(file, position, "click");
        }
        public void getChange()
        {
            wcfclient.GetChanges();
        }
        public void CallBackChanges(string[] s)
        {
            string st = "";
            for (int i = 0; i < s.Length; i++)
            {
                st += s[i];
            }
        }
        public void PrintIds()
        {
            wcfclient.printIds();
        }

        void IDisposable.Dispose()
        {
            wcfclient.Close();
        }
        private void OnCaretChanged(string file, int position,string sender)
        {
            if (ChangeCaret != null)
            {
                ChangeCaret(this, new ChangeCaretEventArgs(sender, position.ToString(), file, " "));
            }
        }
        private void OnNewCaret(string file, int position, string sender)
        {
            if (NewCaret != null)
            {
                NewCaret(this, new ChangeCaretEventArgs(sender, position.ToString(), file, " "));
            }
        }
        public void AddNewEditor(string file, int position, string sender)
        {
            OnNewCaret(file, position, sender);
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
}
