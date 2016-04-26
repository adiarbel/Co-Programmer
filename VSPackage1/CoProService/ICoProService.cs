﻿using System;
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
        bool SendCaretPosition(string file, int position, string content);
        [OperationContract]
        int ShareProject(string path, string projName);
        [OperationContract]
        void GetProject();

        [OperationContract]
        bool SetAdmin(bool adm);
        [OperationContract]
        bool IsConnected();
        [OperationContract]
        int GetExpectedSeq();
        [OperationContract]
        bool SetProjectDir(string dir);
        [OperationContract]
        void UpdateProject();
        [OperationContract]
        void UpdateSpecificFile(string relPath);

    }

    public interface ICoProServiceCallback
    {
        [OperationContract]
        void AddCurrentEditors(string[] editors, string[] locations);
        [OperationContract]
        void NewEditorAdded(string file, int position, string editor, int seq);
        [OperationContract]
        void EditorDisconnected(string editor);
        [OperationContract]
        void ChangedCaret(string file, int position, string editor, int seq);
        [OperationContract]
        void NewAddedText(string file, int position, string editor, string content, int seq);
        [OperationContract]
        void NewRemovedText(string file, int position, string editor, string instruc, int seq);
        [OperationContract]
        void Save(string file);
        [OperationContract]
        void CloneProject(string fileName, byte[] zipFile);
        [OperationContract]
        void ApproveCloning(string[] idsToApprove);
        [OperationContract]
        string[] UpdateProjFilesCallback(string file);
        [OperationContract]
        void UpdateProjFilesContents(string[] files, byte[][] contents);
        [OperationContract]
        void UpdateSpecificFileCallback(byte[] content,string relPath);
        [OperationContract]
        void AdminFileOpen(string file);
    }
}
