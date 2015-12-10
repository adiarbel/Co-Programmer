using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Company.VSPackage1.ServiceReference1;
using System.ServiceModel;

namespace Company.VSPackage1
{
    [CallbackBehavior(UseSynchronizationContext = false)]
    class MyCallBack : IEditServiceCallback, IDisposable
    {
        EditServiceClient proxy;
        public void CallBackFunction(string str)
        {
            System.Windows.Forms.MessageBox.Show(str);
        }
        public void callService(string str)
        {
            InstanceContext context = new InstanceContext(this);
            NetTcpBinding mybinding = new NetTcpBinding();
            EndpointAddress myEndPoint = new EndpointAddress("net.tcp://localhost:8090/EditService");
            DuplexChannelFactory<IEditService> myChannelFactory = new DuplexChannelFactory<IEditService>(context, mybinding, myEndPoint);
            IEditService wcfclient = myChannelFactory.CreateChannel();
            wcfclient.SendCaretPosition(str);
            
        }
        public void Dispose()
        {
            proxy.Close();
        }
    }
}
