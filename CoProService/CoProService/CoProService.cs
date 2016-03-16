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
    class CoProService : ICoProService, IDisposable
    {
        string[] currChanges = new string[10];
        static List<OperationContext> ids = new List<OperationContext>();
        static Dictionary<string, string> carets = new Dictionary<string, string>();
        string id;
        int place = 0;
        public void Dispose()
        {
        }

        public bool IntializePosition(string file, int position)
        {
            string[] assa = carets.Keys.ToArray<string>();
            ICoProServiceCallback callback;
            if (carets.ContainsKey(id))
            {
                SendCaretPosition(file, position,"");
            }
            else
            {
                carets[id] = "" + file + " " + position;

                for (int i = 0; i < ids.Count; i++)
                {
                    if (ids[i].SessionId != id)
                    {
                        try
                        {
                            callback = ids[i].GetCallbackChannel<ICoProServiceCallback>();
                            callback.NewEditorAdded(file, position, id);
                        }
                        catch
                        {
                            ids.Remove(ids[i]);

                        }
                    }

                }
            }
            OperationContext.Current.GetCallbackChannel<ICoProServiceCallback>().AddCurrentEditors(carets.Keys.ToArray<string>(), carets.Values.ToArray<string>());
            return true;
        }


        public bool SendCaretPosition(string file, int position, string content)
        {
            ICoProServiceCallback callback;
            carets[id] = "" + file + " " + position;
            for (int i = 0; i < ids.Count; i++)
            {
                if (ids[i].SessionId != id)
                {
                    try
                    {
                        callback = ids[i].GetCallbackChannel<ICoProServiceCallback>();
                        callback.ChangedCaret(file, position, id);
                    }
                    catch
                    {
                        ids.Remove(ids[i]);

                    }
                }

            }
            return true;
        }
    }
}
