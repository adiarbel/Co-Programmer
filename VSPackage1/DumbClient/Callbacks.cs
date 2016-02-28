using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DumbClient.ServiceReference1;
using System.ServiceModel;

namespace DumbClient
{
    class Callbacks : IEditServiceCallback, IDisposable
    {
        EditServiceClient proxy;
        InstanceContext context;
        EndpointAddress myEndPoint;
        NetTcpBinding mybinding;
        IEditService wcfclient;
        public Callbacks()
        {
            context = new InstanceContext(this);
            mybinding = new NetTcpBinding();
            myEndPoint = new EndpointAddress("net.tcp://localhost:8090/EditService");
            wcfclient = new EditServiceClient(context,mybinding,myEndPoint);
        }
        public void CallBackFunction(string str,string file,string content)
        {
            Console.WriteLine(str + " " + file + " " + content);
        }
        public void CallBackChanges(string[] s)
        {
            string st = "";
            for (int i = 0; i < s.Length; i++)
            {
                st += s[i];
            }
            CallBackFunction(st,"","");
        }
        public void getChange()
        {
            wcfclient.GetChanges();
        }
        public void callService(string str,string file,string content)
        {
            wcfclient.SendCaretPosition(str,file,content);
        }

        void IDisposable.Dispose()
        {
            proxy.Close();
        }
    }
}
