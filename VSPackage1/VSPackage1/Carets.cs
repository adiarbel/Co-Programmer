﻿using EnvDTE;
using EnvDTE80;
using EnvDTE100;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Xaml;
using System.ComponentModel.Design;
using System.Threading;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using Microsoft.Win32;


namespace Company.VSPackage1
{
    class Carets
    {
        DTE2 dte;
        IWpfTextViewHost iwpf;
        TextDocumentKeyPressEvents tde;
        TextEditorEvents te;
        MyCallBack cb;
        bool twice;
        public DTE2 DTE2
        {
            get { return dte ?? (dte = ServiceProvider.GlobalProvider.GetService(typeof(DTE)) as DTE2); }
        }
        public Carets(IWpfTextViewHost h, MyCallBack cb)
        {
            if (DTE2.ActiveWindow != null)
            {
                iwpf = h;
                //tde = ((Events2)DTE2.Events).TextDocumentKeyPressEvents;
                //tde.BeforeKeyPress += new _dispTextDocumentKeyPressEvents_BeforeKeyPressEventHandler(KeyPress_EventHandler);
                te = ((Events2)DTE2.Events).TextEditorEvents;
                //te.LineChanged += new _dispTextEditorEvents_LineChangedEventHandler(EnterFix);
                //te.LineChanged += new _dispTextEditorEvents_LineChangedEventHandler(IntelisenseFix);
                this.cb = cb;
                DTE2.Events.SolutionEvents.BeforeClosing += ShutDown;
                //cb = new MyCallBack();
                //cb.ChangeCaret += new ChangeCaretEventHandler(my_CaretChange);
                //twice = false;
                //TODO: register to cb's events
                //TODO: add a different handler function for each of the events
                // examples: http://www.codeproject.com/Articles/20550/C-Event-Implementation-Fundamentals-Best-Practices

                //ts.NewLine();
                //ts.Insert("a");


            }
        }

        IWpfTextViewHost GetTextViewHost()
        {
            return iwpf;
        }
        void GetTextViewHost(IWpfTextViewHost h)
        {
            iwpf = h;
        }
        private void ShutDown()
        {


            if (cb != null)
            {
                //FileStream fs = File.Create(cb.ProjPath + "\\CoProFiles\\timestamps.txt");
                //fs.Close();
                //StreamWriter sw = new StreamWriter(cb.ProjPath + "\\CoProFiles\\timestamps.txt");
                //sw.Write(TimeStampDirectory(cb.ProjPath, 1, cb.ProjPath.Substring(cb.ProjPath.LastIndexOf('\\'))));
                //sw.Close();
                XElement xe = CreateFileSystemXmlTree(cb.ProjPath,1);
                XmlTextWriter xwr = new XmlTextWriter(cb.ProjPath + "\\CoProFiles\\timestamps.xml", System.Text.Encoding.UTF8);
                xwr.Formatting = Formatting.Indented;
                xe.WriteTo(xwr);
                xwr.Close();
                cb.Abort();
            }
            if (VSPackage1Package.service != null)
            {
                VSPackage1Package.service.Close();
            }
        }
        private string TimeStampDirectory(string target_dir, int lev, string relPath)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);
            string filesInfo = "";
            string currDir;
            foreach (string dir in dirs)
            {
                currDir = dir.Substring(dir.LastIndexOf('\\'));
                if (currDir != "\\bin" && currDir != "\\obj" && currDir != "\\CoProFiles")
                    filesInfo += lev + relPath + ' ' + TimeStampDirectory(dir, lev + 1, relPath + currDir);
            }

            foreach (string file in files)
            {
                filesInfo += ";" + file.Substring(file.LastIndexOf('\\')) + "," + File.GetLastWriteTimeUtc(file) + "," + relPath;
            }

            return filesInfo;
        }
        private XElement CreateFileSystemXmlTree(string source,int level)
        {
            DirectoryInfo dir = new DirectoryInfo(source);
            var info = new XElement("Directory",
                   new XAttribute("Name", dir.Name), new XAttribute("Level", level));

            foreach (var file in dir.GetFiles())
                info.Add(new XElement("File",
                             new XAttribute("Name", file.Name), new XAttribute("TimeChanged", file.LastWriteTimeUtc)));

            foreach (var subDir in dir.GetDirectories())
            {
                if (!(subDir.FullName.Contains("bin") || subDir.FullName.Contains("obj") || subDir.FullName.Contains("CoProFiles")))
                    info.Add(CreateFileSystemXmlTree(subDir.FullName,level+1));
            }

            return info;
        }
    }
}

