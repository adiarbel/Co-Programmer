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
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceReference1.IEditService", CallbackContract=typeof(Company.VSPackage1.ServiceReference1.IEditServiceCallback))]
    public interface IEditService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEditService/SendCaretPosition", ReplyAction="http://tempuri.org/IEditService/SendCaretPositionResponse")]
        void SendCaretPosition(string location);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEditService/SendCaretPosition", ReplyAction="http://tempuri.org/IEditService/SendCaretPositionResponse")]
        System.Threading.Tasks.Task SendCaretPositionAsync(string location);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEditService/NormalFunction", ReplyAction="http://tempuri.org/IEditService/NormalFunctionResponse")]
        void NormalFunction();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEditService/NormalFunction", ReplyAction="http://tempuri.org/IEditService/NormalFunctionResponse")]
        System.Threading.Tasks.Task NormalFunctionAsync();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IEditServiceCallback {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IEditService/CallBackFunction", ReplyAction="http://tempuri.org/IEditService/CallBackFunctionResponse")]
        void CallBackFunction(string str);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IEditServiceChannel : Company.VSPackage1.ServiceReference1.IEditService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class EditServiceClient : System.ServiceModel.DuplexClientBase<Company.VSPackage1.ServiceReference1.IEditService>, Company.VSPackage1.ServiceReference1.IEditService {
        
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
        
        public void SendCaretPosition(string location) {
            base.Channel.SendCaretPosition(location);
        }
        
        public System.Threading.Tasks.Task SendCaretPositionAsync(string location) {
            return base.Channel.SendCaretPositionAsync(location);
        }
        
        public void NormalFunction() {
            base.Channel.NormalFunction();
        }
        
        public System.Threading.Tasks.Task NormalFunctionAsync() {
            return base.Channel.NormalFunctionAsync();
        }
    }
}
