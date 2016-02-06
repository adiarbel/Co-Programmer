using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Company.VSPackage1.ServiceReference1;
using System.ServiceModel;

namespace Company.VSPackage1
{
    //TODO: define delegate
    //TODO: define event for that delegate
    //TODO: define the above for each event that might come from the server's callbacks
    [CallbackBehavior(UseSynchronizationContext = false)]
    class MyCallBack : IEditServiceCallback, IDisposable
    {
        EditServiceClient proxy;
        InstanceContext context;
        EndpointAddress myEndPoint;
        NetTcpBinding mybinding;
        DuplexChannelFactory<IEditService> myChannelFactory;
        IEditService wcfclient;
        public void CallBackFunction(string str)
        {
            System.Windows.Forms.MessageBox.Show(str);
        }
        public MyCallBack()
        {
            context = new InstanceContext(this);
            mybinding = new NetTcpBinding();
            myEndPoint = new EndpointAddress("net.tcp://localhost:8090/EditService");
            myChannelFactory = new DuplexChannelFactory<IEditService>(context, mybinding, myEndPoint);
            wcfclient = myChannelFactory.CreateChannel();            
        }
        public void callService(string str)
        {
            wcfclient.SendCaretPosition(str);
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
            CallBackFunction(st);
        }
        public void PrintIds()
        {
            wcfclient.printIds();
        }

        void IDisposable.Dispose()
        {
            proxy.Close();
        }
    }
}
