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
                ((Events2)DTE2.Events).ProjectItemsEvents.ItemAdded += ItemAdded;
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
        private void ItemAdded(ProjectItem pi)
        {
        }
        IWpfTextViewHost GetTextViewHost()
        {
            return iwpf;
        }
        void GetTextViewHost(IWpfTextViewHost h)
        {
            iwpf = h;
        }
        
    }
}

