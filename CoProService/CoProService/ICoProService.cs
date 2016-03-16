using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace CoProService
{
    [ServiceContract(CallbackContract = typeof(ICoProServiceCallback))]
    interface ICoProService
    {
        [OperationContract]
        bool IntializePosition(string file, int position);
        
    }

    interface ICoProServiceCallback
    {
        [OperationContract]
        void AddCurrentEditors(string[] editors,string[] locations);
        [OperationContract]
        void NewEditorAdded(string file,int position,string editor);
        [OperationContract]
        void EditorDisconnected(string editor);
        [OperationContract]
        void ChangedCaret(string file, int position, string editor);
        [OperationContract]
        void NewAddedText(string file, int position, string editor, string content);
        [OperationContract]
        void NewRemovedText(string file, int position, string editor, int end_position);
    }
}
