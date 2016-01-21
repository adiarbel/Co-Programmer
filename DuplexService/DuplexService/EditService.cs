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
       string[] currChanges = new string[10];
       int place = 0;
        public void NormalFunction()
        {
            IEditServiceCallBack callback = OperationContext.Current.GetCallbackChannel<IEditServiceCallBack>();
            callback.CallBackFunction("Got it!");
        }
        public void SendCaretPosition(string location)
        {
            IEditServiceCallBack callback = OperationContext.Current.GetCallbackChannel<IEditServiceCallBack>();
            currChanges[place++] = location;
            callback.CallBackFunction(location+" From Service");
            
        }
        public void GetChanges()
        {
            IEditServiceCallBack callback = OperationContext.Current.GetCallbackChannel<IEditServiceCallBack>();
            callback.CallBackChanges(currChanges);
            currChanges = new string[10];
            place = 0;
        }

    }
}
