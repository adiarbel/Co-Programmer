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
        bool IntializePosition(string file, int position);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/IntializePosition", ReplyAction="http://tempuri.org/ICoProService/IntializePositionResponse")]
        System.Threading.Tasks.Task<bool> IntializePositionAsync(string file, int position);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/SendCaretPosition", ReplyAction="http://tempuri.org/ICoProService/SendCaretPositionResponse")]
        bool SendCaretPosition(string file, int position, string content);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/SendCaretPosition", ReplyAction="http://tempuri.org/ICoProService/SendCaretPositionResponse")]
        System.Threading.Tasks.Task<bool> SendCaretPositionAsync(string file, int position, string content);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/ShareProject", ReplyAction="http://tempuri.org/ICoProService/ShareProjectResponse")]
        int ShareProject(string path, string projName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/ShareProject", ReplyAction="http://tempuri.org/ICoProService/ShareProjectResponse")]
        System.Threading.Tasks.Task<int> ShareProjectAsync(string path, string projName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/GetProject", ReplyAction="http://tempuri.org/ICoProService/GetProjectResponse")]
        void GetProject();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/GetProject", ReplyAction="http://tempuri.org/ICoProService/GetProjectResponse")]
        System.Threading.Tasks.Task GetProjectAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/SetAdmin", ReplyAction="http://tempuri.org/ICoProService/SetAdminResponse")]
        bool SetAdmin(bool adm);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/SetAdmin", ReplyAction="http://tempuri.org/ICoProService/SetAdminResponse")]
        System.Threading.Tasks.Task<bool> SetAdminAsync(bool adm);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/IsConnected", ReplyAction="http://tempuri.org/ICoProService/IsConnectedResponse")]
        bool IsConnected();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/IsConnected", ReplyAction="http://tempuri.org/ICoProService/IsConnectedResponse")]
        System.Threading.Tasks.Task<bool> IsConnectedAsync();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ICoProServiceCallback {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/AddCurrentEditors", ReplyAction="http://tempuri.org/ICoProService/AddCurrentEditorsResponse")]
        void AddCurrentEditors(string[] editors, string[] locations);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/NewEditorAdded", ReplyAction="http://tempuri.org/ICoProService/NewEditorAddedResponse")]
        void NewEditorAdded(string file, int position, string editor);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/EditorDisconnected", ReplyAction="http://tempuri.org/ICoProService/EditorDisconnectedResponse")]
        void EditorDisconnected(string editor);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/ChangedCaret", ReplyAction="http://tempuri.org/ICoProService/ChangedCaretResponse")]
        void ChangedCaret(string file, int position, string editor);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/NewAddedText", ReplyAction="http://tempuri.org/ICoProService/NewAddedTextResponse")]
        void NewAddedText(string file, int position, string editor, string content);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/NewRemovedText", ReplyAction="http://tempuri.org/ICoProService/NewRemovedTextResponse")]
        void NewRemovedText(string file, int position, string editor, string instruc);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/Save", ReplyAction="http://tempuri.org/ICoProService/SaveResponse")]
        void Save(string file);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICoProService/CloneProject", ReplyAction="http://tempuri.org/ICoProService/CloneProjectResponse")]
        void CloneProject(string fileName, byte[] zipFile);
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
        
        public bool IntializePosition(string file, int position) {
            return base.Channel.IntializePosition(file, position);
        }
        
        public System.Threading.Tasks.Task<bool> IntializePositionAsync(string file, int position) {
            return base.Channel.IntializePositionAsync(file, position);
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
        
        public void GetProject() {
            base.Channel.GetProject();
        }
        
        public System.Threading.Tasks.Task GetProjectAsync() {
            return base.Channel.GetProjectAsync();
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
    }
}
