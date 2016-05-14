﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Company.VSPackage1.ServiceReference1 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceReference1.ICoProService", CallbackContract=typeof(Company.VSPackage1.ServiceReference1.ICoProServiceCallback))]
    public interface ICoProService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/IntializePosition", ReplyAction="http://tempuri.org/ICoProService/IntializePositionResponse")]
        bool IntializePosition(string file, int position, string name);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/IntializePosition", ReplyAction="http://tempuri.org/ICoProService/IntializePositionResponse")]
        System.Threading.Tasks.Task<bool> IntializePositionAsync(string file, int position, string name);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/SendCaretPosition", ReplyAction="http://tempuri.org/ICoProService/SendCaretPositionResponse")]
        bool SendCaretPosition(string file, int position, string content);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/SendCaretPosition", ReplyAction="http://tempuri.org/ICoProService/SendCaretPositionResponse")]
        System.Threading.Tasks.Task<bool> SendCaretPositionAsync(string file, int position, string content);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/ShareProject", ReplyAction="http://tempuri.org/ICoProService/ShareProjectResponse")]
        int ShareProject(string path, string projName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/ShareProject", ReplyAction="http://tempuri.org/ICoProService/ShareProjectResponse")]
        System.Threading.Tasks.Task<int> ShareProjectAsync(string path, string projName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/GetProject", ReplyAction="http://tempuri.org/ICoProService/GetProjectResponse")]
        void GetProject(string name);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/GetProject", ReplyAction="http://tempuri.org/ICoProService/GetProjectResponse")]
        System.Threading.Tasks.Task GetProjectAsync(string name);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/SetAdmin", ReplyAction="http://tempuri.org/ICoProService/SetAdminResponse")]
        bool SetAdmin(bool adm);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/SetAdmin", ReplyAction="http://tempuri.org/ICoProService/SetAdminResponse")]
        System.Threading.Tasks.Task<bool> SetAdminAsync(bool adm);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/IsConnected", ReplyAction="http://tempuri.org/ICoProService/IsConnectedResponse")]
        bool IsConnected();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/IsConnected", ReplyAction="http://tempuri.org/ICoProService/IsConnectedResponse")]
        System.Threading.Tasks.Task<bool> IsConnectedAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/GetExpectedSeq", ReplyAction="http://tempuri.org/ICoProService/GetExpectedSeqResponse")]
        int GetExpectedSeq();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/GetExpectedSeq", ReplyAction="http://tempuri.org/ICoProService/GetExpectedSeqResponse")]
        System.Threading.Tasks.Task<int> GetExpectedSeqAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/SetProjectDir", ReplyAction="http://tempuri.org/ICoProService/SetProjectDirResponse")]
        bool SetProjectDir(string dir);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/SetProjectDir", ReplyAction="http://tempuri.org/ICoProService/SetProjectDirResponse")]
        System.Threading.Tasks.Task<bool> SetProjectDirAsync(string dir);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/UpdateProject", ReplyAction="http://tempuri.org/ICoProService/UpdateProjectResponse")]
        void UpdateProject();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/UpdateProject", ReplyAction="http://tempuri.org/ICoProService/UpdateProjectResponse")]
        System.Threading.Tasks.Task UpdateProjectAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/UpdateSpecificFile", ReplyAction="http://tempuri.org/ICoProService/UpdateSpecificFileResponse")]
        void UpdateSpecificFile(string relPath);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/UpdateSpecificFile", ReplyAction="http://tempuri.org/ICoProService/UpdateSpecificFileResponse")]
        System.Threading.Tasks.Task UpdateSpecificFileAsync(string relPath);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/NewItemAdded", ReplyAction="http://tempuri.org/ICoProService/NewItemAddedResponse")]
        void NewItemAdded(string relpath, byte[] content, string name, string project);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/NewItemAdded", ReplyAction="http://tempuri.org/ICoProService/NewItemAddedResponse")]
        System.Threading.Tasks.Task NewItemAddedAsync(string relpath, byte[] content, string name, string project);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/NewItemRemoved", ReplyAction="http://tempuri.org/ICoProService/NewItemRemovedResponse")]
        void NewItemRemoved(string name, string project, bool isDeleted);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/NewItemRemoved", ReplyAction="http://tempuri.org/ICoProService/NewItemRemovedResponse")]
        System.Threading.Tasks.Task NewItemRemovedAsync(string name, string project, bool isDeleted);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ICoProServiceCallback {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/AddCurrentEditors", ReplyAction="http://tempuri.org/ICoProService/AddCurrentEditorsResponse")]
        void AddCurrentEditors(string[] editors, string[] locations, string[] names);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/NewEditorAdded", ReplyAction="http://tempuri.org/ICoProService/NewEditorAddedResponse")]
        void NewEditorAdded(string file, int position, string editor, int seq, string name);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/EditorDisconnected", ReplyAction="http://tempuri.org/ICoProService/EditorDisconnectedResponse")]
        void EditorDisconnected(string editor);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/ChangedCaret", ReplyAction="http://tempuri.org/ICoProService/ChangedCaretResponse")]
        void ChangedCaret(string file, int position, string editor, int seq);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/NewAddedText", ReplyAction="http://tempuri.org/ICoProService/NewAddedTextResponse")]
        void NewAddedText(string file, int position, string editor, string content, int seq);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/NewRemovedText", ReplyAction="http://tempuri.org/ICoProService/NewRemovedTextResponse")]
        void NewRemovedText(string file, int position, string editor, string instruc, int seq);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/Save", ReplyAction="http://tempuri.org/ICoProService/SaveResponse")]
        void Save(string file);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/CloneProject", ReplyAction="http://tempuri.org/ICoProService/CloneProjectResponse")]
        void CloneProject(string fileName, byte[] zipFile);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/ApproveCloning", ReplyAction="http://tempuri.org/ICoProService/ApproveCloningResponse")]
        void ApproveCloning(string[] idsToApprove);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/UpdateProjFilesCallback", ReplyAction="http://tempuri.org/ICoProService/UpdateProjFilesCallbackResponse")]
        string[][] UpdateProjFilesCallback(string file);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/UpdateProjFilesContents", ReplyAction="http://tempuri.org/ICoProService/UpdateProjFilesContentsResponse")]
        void UpdateProjFilesContents(string[] files, byte[][] contents, string[] n_files, byte[][] n_contents);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/UpdateSpecificFileCallback", ReplyAction="http://tempuri.org/ICoProService/UpdateSpecificFileCallbackResponse")]
        void UpdateSpecificFileCallback(byte[] content, string relPath);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/AdminFileOpen", ReplyAction="http://tempuri.org/ICoProService/AdminFileOpenResponse")]
        void AdminFileOpen(string file);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/NewItemAddedCallback", ReplyAction="http://tempuri.org/ICoProService/NewItemAddedCallbackResponse")]
        void NewItemAddedCallback(string relpath, byte[] content, string name, string project);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/NewItemRemovedCallback", ReplyAction="http://tempuri.org/ICoProService/NewItemRemovedCallbackResponse")]
        void NewItemRemovedCallback(string name, string project, bool isDeleted);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ICoProServiceChannel : Company.VSPackage1.ServiceReference1.ICoProService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CoProServiceClient : System.ServiceModel.DuplexClientBase<Company.VSPackage1.ServiceReference1.ICoProService>, Company.VSPackage1.ServiceReference1.ICoProService {
        
        public CoProServiceClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public CoProServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public CoProServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public CoProServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public CoProServiceClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public bool IntializePosition(string file, int position, string name) {
            return base.Channel.IntializePosition(file, position, name);
        }
        
        public System.Threading.Tasks.Task<bool> IntializePositionAsync(string file, int position, string name) {
            return base.Channel.IntializePositionAsync(file, position, name);
        }
        
        public bool SendCaretPosition(string file, int position, string content) {
            return base.Channel.SendCaretPosition(file, position, content);
        }
        
        public System.Threading.Tasks.Task<bool> SendCaretPositionAsync(string file, int position, string content) {
            return base.Channel.SendCaretPositionAsync(file, position, content);
        }
        
        public int ShareProject(string path, string projName) {
            return base.Channel.ShareProject(path, projName);
        }
        
        public System.Threading.Tasks.Task<int> ShareProjectAsync(string path, string projName) {
            return base.Channel.ShareProjectAsync(path, projName);
        }
        
        public void GetProject(string name) {
            base.Channel.GetProject(name);
        }
        
        public System.Threading.Tasks.Task GetProjectAsync(string name) {
            return base.Channel.GetProjectAsync(name);
        }
        
        public bool SetAdmin(bool adm) {
            return base.Channel.SetAdmin(adm);
        }
        
        public System.Threading.Tasks.Task<bool> SetAdminAsync(bool adm) {
            return base.Channel.SetAdminAsync(adm);
        }
        
        public bool IsConnected() {
            return base.Channel.IsConnected();
        }
        
        public System.Threading.Tasks.Task<bool> IsConnectedAsync() {
            return base.Channel.IsConnectedAsync();
        }
        
        public int GetExpectedSeq() {
            return base.Channel.GetExpectedSeq();
        }
        
        public System.Threading.Tasks.Task<int> GetExpectedSeqAsync() {
            return base.Channel.GetExpectedSeqAsync();
        }
        
        public bool SetProjectDir(string dir) {
            return base.Channel.SetProjectDir(dir);
        }
        
        public System.Threading.Tasks.Task<bool> SetProjectDirAsync(string dir) {
            return base.Channel.SetProjectDirAsync(dir);
        }
        
        public void UpdateProject() {
            base.Channel.UpdateProject();
        }
        
        public System.Threading.Tasks.Task UpdateProjectAsync() {
            return base.Channel.UpdateProjectAsync();
        }
        
        public void UpdateSpecificFile(string relPath) {
            base.Channel.UpdateSpecificFile(relPath);
        }
        
        public System.Threading.Tasks.Task UpdateSpecificFileAsync(string relPath) {
            return base.Channel.UpdateSpecificFileAsync(relPath);
        }
        
        public void NewItemAdded(string relpath, byte[] content, string name, string project) {
            base.Channel.NewItemAdded(relpath, content, name, project);
        }
        
        public System.Threading.Tasks.Task NewItemAddedAsync(string relpath, byte[] content, string name, string project) {
            return base.Channel.NewItemAddedAsync(relpath, content, name, project);
        }
        
        public void NewItemRemoved(string name, string project, bool isDeleted) {
            base.Channel.NewItemRemoved(name, project, isDeleted);
        }
        
        public System.Threading.Tasks.Task NewItemRemovedAsync(string name, string project, bool isDeleted) {
            return base.Channel.NewItemRemovedAsync(name, project, isDeleted);
        }
    }
}
