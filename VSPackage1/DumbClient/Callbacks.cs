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
        DuplexChannelFactory<IEditService> myChannelFactory;
        IEditService wcfclient;
        public Callbacks()
        {
            context = new InstanceContext(this);
            mybinding = new NetTcpBinding();
            myEndPoint = new EndpointAddress("net.tcp://localhost:8090/EditService");
            myChannelFactory = new DuplexChannelFactory<IEditService>(context, mybinding, myEndPoint);
            wcfclient = myChannelFactory.CreateChannel();
        }
        public void CallBackFunction(string str)
        {
            Console.WriteLine(str);
        }
        public void CallBackChanges(string[] s)
        {
            string st = "";
            for (int i = 0; i < s.Length; i++)
            {
                st += s[i];
            }
            CallBackFunction(st);
        }
        public void getChange()
        {
            wcfclient.GetChanges();
        }
        public void callService(string str)
        {
            wcfclient.SendCaretPosition(str);
        }
        public void Dispose()
        {
            proxy.Close();
        }
    }
}
