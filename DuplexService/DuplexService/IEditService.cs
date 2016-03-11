using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Uml.Classes;
namespace DuplexService
{
    [ServiceContract(CallbackContract = typeof(IEditServiceCallBack))]
    public interface IEditService
    {

        [OperationContract]
        void SendCaretPosition(ITrackingPoint itp);
        //[OperationContract]
       // void NormalFunction();
        [OperationContract]
        void GetChanges();
        [OperationContract]
        void printIds();
    }
    public interface IEditServiceCallBack
    {
        [OperationContract]
        void CallBackFunction(ITrackingPoint itp, string content);
        [OperationContract]
        void AddNewEditor(ITrackingPoint itp);
        [OperationContract]
        void CallBackChanges(string[] s);
    }
    
}
