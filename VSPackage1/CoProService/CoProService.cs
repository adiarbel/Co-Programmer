using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.VisualStudio.Text;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Xml.Linq;

namespace CoProService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class CoProService : ICoProService, IDisposable
    {
        string[] currChanges = new string[10];
        static Dictionary<string, OperationContext> ids = new Dictionary<string, OperationContext>();
        static Dictionary<string, string> carets = new Dictionary<string, string>();
        static List<string> ShareProjectIDs = new List<string>();
        static List<string> ShareProjectIPs = new List<string>();
        static int seqId = 0;
        static string projPath;
        int place;
        string id;
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
        public bool SetProjectDir(string dir)
        {
            if (isAdmin)
            {
                projPath = dir;
                return true;
            }
            return false;
        }
        public void UpdateProject()
        {
            XDocument xd = XDocument.Load(projPath + "\\CoProFiles\\timestamps.xml");
            string content = xd.ToString();
            string[] filesToSend = OperationContext.Current.GetCallbackChannel<ICoProServiceCallback>().UpdateProjFilesCallback(content);
            string absolutePath = projPath.Substring(0, projPath.LastIndexOf('\\'));
            byte[][] filesContents = new byte[filesToSend.Length + 1][];
            for (int i = 0; i < filesToSend.Length; i++)
            {
                filesContents[i] = File.ReadAllBytes(absolutePath + '\\' + filesToSend[i]);
            }
            filesContents[filesToSend.Length] = File.ReadAllBytes(projPath + "\\CoProFiles\\timestamps.xml");
            OperationContext.Current.GetCallbackChannel<ICoProServiceCallback>().UpdateProjFilesContents(filesToSend, filesContents);

        }
        public bool IntializePosition(string file, int position)
        {
            string[] assa = carets.Keys.ToArray<string>();
            ICoProServiceCallback callback;
            if (carets.ContainsKey(id))
            {
                SendCaretPosition(file, position, "click");
                List<string> keys = new List<string>();
                List<string> vals = new List<string>();
                for (int i = 0; i < carets.Values.Count; i++)
                {
                    string t_key = carets.Keys.ElementAt(i);
                    string t_val = carets.Values.ElementAt(i);

                    if(t_key!=id)
                    {
                        if (t_val.Contains(file))
                        {
                            keys.Add(t_key);
                            vals.Add(t_val);
                        }
                    }
                }
                OperationContext.Current.GetCallbackChannel<ICoProServiceCallback>().AddCurrentEditors(keys.ToArray<string>(), vals.ToArray<string>());
                if (!isAdmin)
                {
                    ids[admin].GetCallbackChannel<ICoProServiceCallback>().AdminFileOpen(file);
                }

            }
            else
            {
                OperationContext.Current.GetCallbackChannel<ICoProServiceCallback>().AddCurrentEditors(carets.Keys.ToArray<string>(), carets.Values.ToArray<string>());
                lock (carets)
                {
                    carets[id] = "" + file + " " + position;
                }
                OperationContext[] idsArr = ids.Values.ToArray();
                string[] idsKeys = ids.Keys.ToArray(); 
                if (!isAdmin)
                {
                    ids[admin].GetCallbackChannel<ICoProServiceCallback>().AdminFileOpen(file);
                }
                for (int i = 0; i < idsArr.Length; i++)
                {
                    if (idsKeys[i] != id)
                    {
                        try
                        {
                            callback = idsArr[i].GetCallbackChannel<ICoProServiceCallback>();
                            lock (locker)
                            {
                                callback.NewEditorAdded(file, position, id, seqId);
                            }

                        }
                        catch
                        {
                            //ids.Remove(entry.Key);

                        }
                    }

                }
                seqId++;
            }
            return true;
        }
        public bool SendCaretPosition(string file, int position, string content)
        {
            ICoProServiceCallback callback;
            lock (carets)
            {
                carets[id] = "" + file + " " + position;
            }
            OperationContext[] idsArr = ids.Values.ToArray();
            string[] idsKeys = ids.Keys.ToArray(); 
            for (int i = 0; i < idsArr.Length; i++)
            {
                if (idsKeys[i] != id)
                {
                    try
                    {
                        callback = idsArr[i].GetCallbackChannel<ICoProServiceCallback>();
                        if (content == "click")
                        {
                            lock (locker)
                            {
                                callback.ChangedCaret(file, position, id, seqId);
                                seqId++;
                            }
                        }
                        else if (content.Contains("DELETE"))
                        {
                            lock (locker)
                            {
                                callback.NewRemovedText(file, position, id, content, seqId);
                                seqId++;
                            }
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
                            lock (locker)
                            {
                                callback.NewAddedText(file, position, id, content, seqId);
                                seqId++;
                            }
                        }
                    }
                    catch
                    {
                        //ids.Remove(entry.Key);
                    }
                }

            }
            return true;
        }
        public void GetProject()
        {
            lock (ShareProjectIDs)
            {
                ShareProjectIDs.Add(id);
            }
            lock (ShareProjectIPs)
            {
                string ip="";
                var prop = OperationContext.Current.IncomingMessageProperties;
                if (OperationContext.Current.IncomingMessageProperties.ContainsKey(System.ServiceModel.Channels.RemoteEndpointMessageProperty.Name))
                {
                    var endpoint = prop[System.ServiceModel.Channels.RemoteEndpointMessageProperty.Name]
                        as System.ServiceModel.Channels.RemoteEndpointMessageProperty;
                    if (endpoint != null)
                    {
                        ip = endpoint.Address;
                    }
                }
                ShareProjectIPs.Add(ip);
            }
            ids[admin].GetCallbackChannel<ICoProServiceCallback>().ApproveCloning(ShareProjectIPs.ToArray());
        }
        public int ShareProject(string path, string projName)
        {
            if (isAdmin)
            {
                ZipProject(path);
                byte[] zipfile = File.ReadAllBytes(path.Substring(0, path.LastIndexOf('\\') + 1) + "\\proj.zip");
                for (int i = 0; i < ShareProjectIDs.Count; i++)
                {
                    ids[ShareProjectIDs[i]].GetCallbackChannel<ICoProServiceCallback>().CloneProject(projName, zipfile);
                }
                File.Delete(path.Substring(0, path.LastIndexOf('\\') + 1) + "\\proj.zip");
                ShareProjectIDs.Clear();
                return 1;
            }
            return 0;
        }

        public void UpdateSpecificFile(string relPath)
        {
            var filePath = projPath + relPath.Substring(relPath.IndexOf('\\'));
            byte[] content = File.ReadAllBytes(filePath);
            OperationContext.Current.GetCallbackChannel<ICoProServiceCallback>().UpdateSpecificFileCallback(content, relPath);
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
        public bool IsConnected()
        {
            return true;
        }

        public int GetExpectedSeq()
        {
            int seq;
            lock (locker)
            {
                seq = seqId;
            }
            return seq;
        }
        void IDisposable.Dispose()
        {
            ICoProServiceCallback callback;
            lock (ids)
            {
                ids.Remove(id);
            }
            lock (carets)
            {
                carets.Remove(id);
            }
            if (admin == id)
            {
                admin = "";
            }
            OperationContext[] ocarr = ids.Values.ToArray<OperationContext>();
            for (int i = 0; i < ocarr.Length; i++)
            {
                try
                {
                    callback = ocarr[i].GetCallbackChannel<ICoProServiceCallback>();
                    callback.EditorDisconnected(id);
                }
                catch
                {
                    //removal
                }
            }
        }

    }
}
