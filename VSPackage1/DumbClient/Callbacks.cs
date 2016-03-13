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
       
        public void CallBackChanges(string[] s)
        {
            string st = "";
            for (int i = 0; i < s.Length; i++)
            {
                st += s[i];
            }
            CallBackFunction(st,0,"");
        }
        public void getChange()
        {
            wcfclient.GetChanges();
        }
        public void callService(string file, int position)
        {
            wcfclient.IntializePosition(file, position);
        }
        void IDisposable.Dispose()
        {
            proxy.Close();
        }


        public void CallBackFunction(string file, int position, string sender)
        {
            
        }

        public void AddNewEditor(string file, int position, string sender)
        {
            
        }
    }
}
