using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Microsoft.VisualStudio.Text;

namespace DuplexService
{

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class EditService : IEditService, IDisposable
    {
        string[] currChanges = new string[10];
        static List<OperationContext> ids = new List<OperationContext>();
        static Dictionary<string,string> carets = new Dictionary<string,string>();
        string id;
        int place = 0;
        public EditService()
        {
            id = OperationContext.Current.SessionId;
            ids.Add(OperationContext.Current);
        }

        public void IntializePosition(string file, int line, int char_off)
        {
            IEditServiceCallBack callback;
            carets[id] = "" + file + " " + line + " " + char_off;
            
            for (int i = 0; i < ids.Count;i++ )
            {
                if (ids[i].SessionId != id)
                {
                    try
                    {
                        callback = ids[i].GetCallbackChannel<IEditServiceCallBack>();
                        callback.AddNewEditor(file, line, char_off);
                    }
                    catch
                    {
                        ids.Remove(ids[i]);

                    }
                }

            }

        }
        public void SendCaretPosition(string file, int line, int char_off, string content)
        {
            IEditServiceCallBack callback;
            carets[id] = "" + file + " " + line + " " + char_off;
            currChanges[place++] = file.ToString();
            for (int i = 0; i < ids.Count; i++)
            {

                try
                {
                    callback = ids[i].GetCallbackChannel<IEditServiceCallBack>();
                    callback.AddNewEditor(file, line, char_off);
                }
                catch
                {
                    ids.Remove(ids[i]);

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
