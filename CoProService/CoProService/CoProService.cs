using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ServiceModel;
using Microsoft.VisualStudio.Text;


namespace CoProService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class CoProService : ICoProService, IDisposable
    {
        string[] currChanges = new string[10];
        static Dictionary<string, OperationContext> ids = new Dictionary<string, OperationContext>();
        static Dictionary<string, string> carets = new Dictionary<string, string>();
        string id;
        int place;
        public CoProService()
        {
            id = OperationContext.Current.SessionId;
            ids[id] = OperationContext.Current;
        }
        public bool IntializePosition(string file, int position)
        {
            string[] assa = carets.Keys.ToArray<string>();
            ICoProServiceCallback callback;
            OperationContext.Current.GetCallbackChannel<ICoProServiceCallback>().AddCurrentEditors(carets.Keys.ToArray<string>(), carets.Values.ToArray<string>());
            if (carets.ContainsKey(id))
            {
                SendCaretPosition(file, position, "");
            }
            else
            {
                carets[id] = "" + file + " " + position;

                foreach (KeyValuePair<string, OperationContext> entry in ids)
                {
                    if (entry.Value.SessionId != id)
                    {
                        try
                        {
                            callback = entry.Value.GetCallbackChannel<ICoProServiceCallback>();
                            callback.NewEditorAdded(file, position, id);
                        }
                        catch
                        {
                            ids.Remove(entry.Key);

                        }
                    }

                }
            }
            return true;
        }


        public bool SendCaretPosition(string file, int position, string content)
        {
            ICoProServiceCallback callback;
            carets[id] = "" + file + " " + position;
            foreach (KeyValuePair<string, OperationContext> entry in ids)
            {
                if (entry.Value.SessionId != id)
                {
                    try
                    {

                        callback = entry.Value.GetCallbackChannel<ICoProServiceCallback>();
                        if (content == "click")
                        {
                            callback.ChangedCaret(file, position, id);
                        }
                        else if (content.Contains("BACKSPACE") || content.Contains("DELETE"))
                        {
                            callback.NewRemovedText(file, position, id, content);
                        }
                        else
                        {
                            callback.NewAddedText(file, position, id, content);
                        }
                    }
                    catch
                    {
                        ids.Remove(entry.Key);

                    }
                }

            }
            return true;
        }
        private void PrintIds()
        {
            foreach (KeyValuePair<string, OperationContext> entry in ids)
            {
                Console.WriteLine(entry.Value.SessionId);
            }
        }

        void IDisposable.Dispose()
        {
            ICoProServiceCallback callback;
            ids.Remove(id);
            carets.Remove(id);
            foreach (KeyValuePair<string, OperationContext> entry in ids)
            {

                try
                {
                    callback = entry.Value.GetCallbackChannel<ICoProServiceCallback>();
                    callback.EditorDisconnected(id);
                }
                catch
                {
                    ids.Remove(id);
                    carets.Remove(id);
                }


            }
        }
    }
}
