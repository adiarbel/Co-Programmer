﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace DuplexService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class EditService : IEditService,IDisposable
    {
        string[] currChanges = new string[10];
        static List<OperationContext> ids = new List<OperationContext>();
        string id;
        int place = 0;
        public EditService()
        {
            ids.Add(OperationContext.Current);
            id = OperationContext.Current.SessionId;
        }
        public void NormalFunction()
        {
            IEditServiceCallBack callback = OperationContext.Current.GetCallbackChannel<IEditServiceCallBack>();
            callback.CallBackFunction("Got it!","","");
        }
        public void SendCaretPosition(string location,string file,string content)
        {
            IEditServiceCallBack callback;
            currChanges[place++] = location;
            for (int i = 0; i < ids.Count; i++)
            {
                if(ids[i].SessionId!=id)
                {
                    try
                    {
                        callback = ids[i].GetCallbackChannel<IEditServiceCallBack>();
                        callback.CallBackFunction(location + ",From Service",file,content);
                    }
                    catch
                    {
                        ids.Remove(ids[i]);
                    }
                }
            }

        }
        public void GetChanges()
        {
            IEditServiceCallBack callback = OperationContext.Current.GetCallbackChannel<IEditServiceCallBack>();
            callback.CallBackChanges(currChanges);
            currChanges = new string[10];
            place = 0;
        }
        public void printIds()
        {
            for (int i = 0; i < ids.Count; i++)
            {
                Console.WriteLine(ids[i].SessionId);
            }
        }
        void IDisposable.Dispose()
        {
            ids.Remove(OperationContext.Current);
        }

    }
    
}
