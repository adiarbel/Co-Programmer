using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace DuplexService
{
    [ServiceBehavior(ConcurrencyMode=ConcurrencyMode.Reentrant)]
    public class EditService : IEditService
    {
        public void NormalFunction()
        {
            IEditServiceCallBack callback = OperationContext.Current.GetCallbackChannel<IEditServiceCallBack>();
            callback.CallBackFunction("Got it!");
        }
        public void SendCaretPosition(string location)
        {
            IEditServiceCallBack callback = OperationContext.Current.GetCallbackChannel<IEditServiceCallBack>();
            callback.CallBackFunction(location+" From Service");
        }
    }
}
