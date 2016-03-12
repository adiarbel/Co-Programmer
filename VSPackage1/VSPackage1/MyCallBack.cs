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
    public delegate void ChangeCaretEventHandler(object sender, ChangeCaretEventArgs e);
    /*TODO: define event for that delegate*/

    /*TODO: define the above for each event that might come from the server's callbacks*/
    [CallbackBehavior(UseSynchronizationContext = false)]
    class MyCallBack : IEditServiceCallback, IDisposable
    {
        public event ChangeCaretEventHandler ChangeCaret;
        InstanceContext context;
        EndpointAddress myEndPoint;
        NetTcpBinding mybinding;
        EditServiceClient wcfclient;
        public void CallBackFunction(string file, int line, int char_off)
        {

        }
        public MyCallBack()
        {
            context = new InstanceContext(this);
            mybinding = new NetTcpBinding();
            myEndPoint = new EndpointAddress("net.tcp://localhost:8090/EditService");
            wcfclient = new ServiceReference1.EditServiceClient(context, mybinding, myEndPoint);
            PrintIds();
        }
        public void callService(string file, int line, int char_off)
        {
            wcfclient.IntializePosition(file, line, char_off);
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
        private void OnCaretChanged(string file, int line, int char_off,string sender)
        {
            if (ChangeCaret != null)
            {
                ChangeCaret(this, new ChangeCaretEventArgs( sender,line.ToString(),file, char_off.ToString()));
            }
        }




        public void AddNewEditor(string file, int line, int char_off, string sender)
        {
            OnCaretChanged(file, line, char_off, sender);
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
