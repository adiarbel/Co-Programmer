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
    public interface ICoProService
    {
        [OperationContract]
        bool IntializePosition(string file, int position);
        [OperationContract]
        bool SendCaretPosition(string file, int position,string content);
        [OperationContract]
        int ShareProject(string path,string projName);
        [OperationContract]
        void GetProject();

        [OperationContract]
        bool SetAdmin(bool adm);
        [OperationContract]
        bool IsConnected();
    }

    public interface ICoProServiceCallback
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
        void NewRemovedText(string file, int position, string editor, string instruc);
        [OperationContract]
        void Save(string file);
        [OperationContract]
        void CloneProject(string fileName,byte[] zipFile);
    }
}
