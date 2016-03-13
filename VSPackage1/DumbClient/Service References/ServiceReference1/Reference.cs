﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DumbClient.ServiceReference1 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceReference1.IEditService", CallbackContract=typeof(DumbClient.ServiceReference1.IEditServiceCallback))]
    public interface IEditService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEditService/IntializePosition", ReplyAction="http://tempuri.org/IEditService/IntializePositionResponse")]
        void IntializePosition(string file, int position);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEditService/IntializePosition", ReplyAction="http://tempuri.org/IEditService/IntializePositionResponse")]
        System.Threading.Tasks.Task IntializePositionAsync(string file, int position);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEditService/SendCaretPosition", ReplyAction="http://tempuri.org/IEditService/SendCaretPositionResponse")]
        void SendCaretPosition(string file, int position, string content);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEditService/SendCaretPosition", ReplyAction="http://tempuri.org/IEditService/SendCaretPositionResponse")]
        System.Threading.Tasks.Task SendCaretPositionAsync(string file, int position, string content);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEditService/GetChanges", ReplyAction="http://tempuri.org/IEditService/GetChangesResponse")]
        void GetChanges();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEditService/GetChanges", ReplyAction="http://tempuri.org/IEditService/GetChangesResponse")]
        System.Threading.Tasks.Task GetChangesAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEditService/printIds", ReplyAction="http://tempuri.org/IEditService/printIdsResponse")]
        void printIds();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEditService/printIds", ReplyAction="http://tempuri.org/IEditService/printIdsResponse")]
        System.Threading.Tasks.Task printIdsAsync();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IEditServiceCallback {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEditService/CallBackFunction", ReplyAction="http://tempuri.org/IEditService/CallBackFunctionResponse")]
        void CallBackFunction(string file, int position, string sender);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEditService/AddNewEditor", ReplyAction="http://tempuri.org/IEditService/AddNewEditorResponse")]
        void AddNewEditor(string file, int position, string sender);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEditService/CallBackChanges", ReplyAction="http://tempuri.org/IEditService/CallBackChangesResponse")]
        void CallBackChanges(string[] s);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IEditServiceChannel : DumbClient.ServiceReference1.IEditService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class EditServiceClient : System.ServiceModel.DuplexClientBase<DumbClient.ServiceReference1.IEditService>, DumbClient.ServiceReference1.IEditService {
        
        public EditServiceClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public EditServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public EditServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public EditServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public EditServiceClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public void IntializePosition(string file, int position) {
            base.Channel.IntializePosition(file, position);
        }
        
        public System.Threading.Tasks.Task IntializePositionAsync(string file, int position) {
            return base.Channel.IntializePositionAsync(file, position);
        }
        
        public void SendCaretPosition(string file, int position, string content) {
            base.Channel.SendCaretPosition(file, position, content);
        }
        
        public System.Threading.Tasks.Task SendCaretPositionAsync(string file, int position, string content) {
            return base.Channel.SendCaretPositionAsync(file, position, content);
        }
        
        public void GetChanges() {
            base.Channel.GetChanges();
        }
        
        public System.Threading.Tasks.Task GetChangesAsync() {
            return base.Channel.GetChangesAsync();
        }
        
        public void printIds() {
            base.Channel.printIds();
        }
        
        public System.Threading.Tasks.Task printIdsAsync() {
            return base.Channel.printIdsAsync();
        }
    }
}
