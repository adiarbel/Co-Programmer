using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace DuplexService
{
    [ServiceContract(CallbackContract = typeof(IEditServiceCallBack))]
    public interface IEditService
    {

        [OperationContract]
        void SendCaretPosition(string location);
        [OperationContract]
        void NormalFunction();
    }
    public interface IEditServiceCallBack
    {
        [OperationContract]
        void CallBackFunction(string str);
    }
}
