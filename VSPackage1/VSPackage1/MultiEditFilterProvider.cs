using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Text;
using System.IO;
using System.Text;

namespace Company.VSPackage1
{
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    internal class MultiEditFilterProvider : IVsTextViewCreationListener//classifies as an extension
    {
        [Export(typeof(AdornmentLayerDefinition))]
        [Name("MultiEditLayer")]
        [TextViewRole(PredefinedTextViewRoles.Editable)]
        internal AdornmentLayerDefinition m_multieditAdornmentLayer = null;
        [Import(typeof(IVsEditorAdaptersFactoryService))]
        internal IVsEditorAdaptersFactoryService editorFactory = null;
        static MyCallBack cb = VSPackage1Package.cb;
        Carets cs;
        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            if (cb == null)
            {
                cb = new MyCallBack();
                VSPackage1Package.cb = cb;
            }
            bool setAdmin = false;
            cs = new Carets(GetCurrentViewHost(textViewAdapter), cb);
            var slnName = cs.DTE2.Solution.FullName;
            var adminFile = slnName.Substring(0, slnName.Substring(0, slnName.LastIndexOf('\\')).LastIndexOf('\\')) + "\\admin.txt";
            if (File.Exists(adminFile))
            {
                StreamReader sr = new StreamReader(adminFile);
                string dir = sr.ReadToEnd();
                if (dir == slnName.Substring(0, slnName.LastIndexOf('\\')))
                {
                    VSPackage1Package.service = new Service();
                    string filesDir=slnName.Substring(0, slnName.LastIndexOf('\\')) + "\\CoProFiles";
                    if (!Directory.Exists(filesDir))
                    {
                        Directory.CreateDirectory(slnName.Substring(0, slnName.LastIndexOf('\\')) + "\\CoProFiles");
                    }
                    FileStream fs = File.Create(slnName.Substring(0, slnName.LastIndexOf('\\')) + "\\CoProFiles\\client.txt");
                    string ipPort = "localhost:" + (VSPackage1Package.service.PortOfService() + 10).ToString();
                    fs.Write(Encoding.ASCII.GetBytes(ipPort), 0, ipPort.Length);
                    fs.Close();
                    setAdmin = true;
                }
            }
            if (File.Exists(slnName.Substring(0, slnName.LastIndexOf('\\')) + "\\CoProFiles\\client.txt"))
            {
                StreamReader sr = new StreamReader(slnName.Substring(0, slnName.LastIndexOf('\\')) + "\\CoProFiles\\client.txt");
                string iport = sr.ReadToEnd();
                cb.SetIpPort(iport.Split(':')[0], iport.Split(':')[1]);
                if (cb.Connect())
                {
                    cb.ProjPath = slnName.Substring(0, slnName.LastIndexOf('\\'));
                    if (setAdmin)
                    {
                        cb.SetAdmin(true);
                    }
                    else
                    {

                    }
                    IWpfTextView textView = editorFactory.GetWpfTextView(textViewAdapter);//gets the text view
                    if (textView == null)
                        return;
                    AddCommandFilter(textViewAdapter, new MultiEditCommandFilter(textView, cb, cs));//adds an instance of our command filter to the text view
                }
            }
            else
            {
                cb = null;
                cs = null;
            }
        }
        IWpfTextViewHost GetCurrentViewHost(IVsTextView vTextView)
        {
            IVsUserData userData = vTextView as IVsUserData;
            if (userData == null)
            {
                return null;
            }
            else
            {
                IWpfTextViewHost viewHost;
                object holder;
                Guid guidViewHost = DefGuidList.guidIWpfTextViewHost;
                userData.GetData(ref guidViewHost, out holder);
                viewHost = (IWpfTextViewHost)holder;
                return viewHost;
            }
        }
        void AddCommandFilter(IVsTextView viewAdapter, MultiEditCommandFilter commandFilter)
        {
            if (commandFilter.m_added == false)
            {
                //get the view adapter from the editor factory
                IOleCommandTarget next;
                int hr = viewAdapter.AddCommandFilter(commandFilter, out next);

                if (hr == VSConstants.S_OK)
                {
                    commandFilter.m_added = true;
                    //you'll need the next target for Exec and QueryStatus
                    if (next != null)
                        commandFilter.m_nextTarget = next;
                }
            }
        }

    }
}
