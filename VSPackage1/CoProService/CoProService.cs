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
        
        static Dictionary<string, OperationContext> ids = new Dictionary<string, OperationContext>(); //ids of editors
        static Dictionary<string, string> carets = new Dictionary<string, string>();// locations of editors
        static List<string> EditorsNames = new List<string>(); // editors names
        static int seqId = 0;// current message sequence
        static string projPath;// the path of the project on the computer
        string name;// the name of the current editor
        string id;// the id of the current editor
        Object locker = new Object();// locker for mutex
        bool isAdmin;// whether the editor is admin or not
        static string admin = "";// the id of the admin for public use
        /// <summary>
        /// 
        /// </summary>
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
            string[][] filesToSend = OperationContext.Current.GetCallbackChannel<ICoProServiceCallback>().UpdateProjFilesCallback(content);
            string absolutePath = projPath.Substring(0, projPath.LastIndexOf('\\'));
            byte[][] filesContents = new byte[filesToSend[0].Length + 1][];
            byte[][] newFilesContents = new byte[filesToSend[1].Length][];
            for (int i = 0; i < filesToSend[0].Length; i++)
            {
                filesContents[i] = File.ReadAllBytes(absolutePath + '\\' + filesToSend[0][i]);

            } for (int i = 0; i < filesToSend[1].Length; i++)
            {
                newFilesContents[i] = File.ReadAllBytes(absolutePath + '\\' + filesToSend[1][i]);
            }
            filesContents[filesToSend[0].Length] = File.ReadAllBytes(projPath + "\\CoProFiles\\timestamps.xml");
            OperationContext.Current.GetCallbackChannel<ICoProServiceCallback>().UpdateProjFilesContents(filesToSend[0], filesContents, filesToSend[1], newFilesContents);

        }
        public bool IntializePosition(string file, int position, string name)
        {
            string[] assa = carets.Keys.ToArray<string>();
            ICoProServiceCallback callback;
            if (carets.ContainsKey(id))
            {
                List<string> keys = new List<string>();
                List<string> vals = new List<string>();
                for (int i = 0; i < carets.Values.Count; i++)
                {
                    string t_key = carets.Keys.ElementAt(i);
                    string t_val = carets.Values.ElementAt(i);

                    if (t_key != id)
                    {
                        if (t_val.Contains(file))
                        {
                            keys.Add(t_key);
                            vals.Add(t_val);
                        }
                    }
                }
                if (!isAdmin)
                {
                    ids[admin].GetCallbackChannel<ICoProServiceCallback>().AdminFileOpen(file);

                }
                OperationContext.Current.GetCallbackChannel<ICoProServiceCallback>().AddCurrentEditors(keys.ToArray<string>(), vals.ToArray<string>(), null);

                SendCaretPosition(file, position, "click");

            }
            else
            {
                this.name = name;
                EditorsNames.Add(name);
                if (!isAdmin)
                {
                    ids[admin].GetCallbackChannel<ICoProServiceCallback>().AdminFileOpen(file);
                }
                OperationContext.Current.GetCallbackChannel<ICoProServiceCallback>().AddCurrentEditors(carets.Keys.ToArray<string>(), carets.Values.ToArray<string>(), EditorsNames.ToArray());
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
                            lock (locker)
                            {
                                callback.NewEditorAdded(file, position, id, seqId, name);
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
        public bool GetProject(string name)
        {
            return ids[admin].GetCallbackChannel<ICoProServiceCallback>().ApproveCloning(name, id);

        }
        public int ShareProject(string path, string projName, string id)
        {
            if (isAdmin)
            {
                ZipProject(path);
                byte[] zipfile = File.ReadAllBytes(path.Substring(0, path.LastIndexOf('\\') + 1) + "\\proj.zip");

                ids[id].GetCallbackChannel<ICoProServiceCallback>().CloneProject(projName, zipfile);
                File.Delete(path.Substring(0, path.LastIndexOf('\\') + 1) + "\\proj.zip");
                
                return 1;
            }
            return 0;
        }

        public void UpdateSpecificFile(string relPath)
        {
            var filePath = projPath + relPath.Substring(relPath.IndexOf('\\'));
            if (File.Exists(filePath))
            {
                byte[] content = File.ReadAllBytes(filePath);
                OperationContext.Current.GetCallbackChannel<ICoProServiceCallback>().UpdateSpecificFileCallback(content, relPath);
            }
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

        public void NewItemAdded(string relpath, byte[] content, string name, string project)
        {
            ICoProServiceCallback callback;
            string[] idsKeys = ids.Keys.ToArray();
            OperationContext[] idsArr = ids.Values.ToArray();
            for (int i = 0; i < idsArr.Length; i++)
            {
                if (idsKeys[i] != id)
                {
                    try
                    {
                        callback = idsArr[i].GetCallbackChannel<ICoProServiceCallback>();
                        lock (locker)
                        {
                            callback.NewItemAddedCallback(relpath, content, name, project);
                        }
                    }
                    catch
                    {

                    }
                }

            }
        }

        public void NewItemRemoved(string name, string project, bool isDeleted)
        {
            ICoProServiceCallback callback;
            string[] idsKeys = ids.Keys.ToArray();
            OperationContext[] idsArr = ids.Values.ToArray();
            for (int i = 0; i < idsArr.Length; i++)
            {
                if (idsKeys[i] != id)
                {
                    try
                    {
                        callback = idsArr[i].GetCallbackChannel<ICoProServiceCallback>();
                        lock (locker)
                        {
                            callback.NewItemRemovedCallback(name, project, isDeleted);
                        }
                    }
                    catch
                    {

                    }
                }

            }
        }
        /// <summary>
        /// 
        /// </summary>
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
