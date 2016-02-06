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
        [OperationContract]
        void GetChanges();
        [OperationContract]
        void printIds();
    }
    public interface IEditServiceCallBack
    {
        [OperationContract]
        void CallBackFunction(string str);
        [OperationContract]
        void CallBackChanges(string[] s);
    }
}
