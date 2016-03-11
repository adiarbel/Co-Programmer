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
        void IntializePosition(string file, int line, int char_off);
        void SendCaretPosition(string file,int line,int char_off,string content);
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
        void CallBackFunction(string file, int line, int char_off);
        [OperationContract]
        void AddNewEditor(string file, int line, int char_off);
        [OperationContract]
        void CallBackChanges(string[] s);
    }
    
}
