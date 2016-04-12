using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ServiceModel;
using Microsoft.VisualStudio.Text;
using System.IO;
using System.IO.Compression;

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
        Object locker = new Object();
        bool isAdmin;
        static string admin = "";
        public CoProService()
        {
            id = OperationContext.Current.SessionId;
            ids[id] = OperationContext.Current;

            //PrintIds();
            isAdmin = false;
        }
        public bool SetAdmin(bool adm)
        {
            if (admin != "" && adm)
            {
                return false;
            }
            isAdmin = adm;
            if (adm)
            {
                admin = id;
            }
            else
            {
                admin = "";
            }
            return true;
        }
        public bool IntializePosition(string file, int position)
        {
            string[] assa = carets.Keys.ToArray<string>();
            ICoProServiceCallback callback;
            OperationContext.Current.GetCallbackChannel<ICoProServiceCallback>().AddCurrentEditors(carets.Keys.ToArray<string>(), carets.Values.ToArray<string>());
            if (carets.ContainsKey(id))
            {
                SendCaretPosition(file, position, "click");
            }
            else
            {
                lock (carets)
                {
                    carets[id] = "" + file + " " + position;
                }
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
                        else if (content.Contains("DELETE"))
                        {
                            callback.NewRemovedText(file, position, id, content);
                        }
                        else if (content.Contains("save"))
                        {
                            if (content == "saveS")
                            {
                                callback.Save("all");
                            }
                            else
                            {
                                callback.Save(file);
                            }
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

        public int ShareProject(string path, string projName, string[] identifications)
        {
            if (isAdmin)
            {
                ZipProject(path);
                byte[] zipfile = File.ReadAllBytes(path.Substring(0, path.LastIndexOf('\\') + 1) + "\\proj.zip");
                for (int i = 0; i < identifications.Length; i++)
                {
                    ids[identifications[i]].GetCallbackChannel<ICoProServiceCallback>().CloneProject(projName, zipfile);
                }
                return 1;
            }
            return 0;
        }
        private void ZipProject(string path)
        {
            try
            {
                ZipFile.CreateFromDirectory(path, path.Substring(0, path.LastIndexOf('\\') + 1) + "\\proj.zip");
            }
            catch
            {
                File.Delete(path.Substring(0, path.LastIndexOf('\\') + 1) + "\\proj.zip");
                ZipFile.CreateFromDirectory(path, path.Substring(0, path.LastIndexOf('\\') + 1) + "\\proj.zip");
            }
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
            if (admin == id)
            {
                admin = "";
            }
            ids.Clear();
            carets.Clear();
        }
    }
}
