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
        private DTE2 DTE2
        {
            get { return dte ?? (dte = ServiceProvider.GlobalProvider.GetService(typeof(DTE)) as DTE2); }
        }
        public Carets(IWpfTextViewHost h)
        {
            if (DTE2.ActiveWindow != null)
            {
                iwpf = h;
                tde = ((Events2)DTE2.Events).TextDocumentKeyPressEvents;
                tde.BeforeKeyPress += new _dispTextDocumentKeyPressEvents_BeforeKeyPressEventHandler(KeyPress_EventHandler);
                te = ((Events2)DTE2.Events).TextEditorEvents;
                te.LineChanged += new _dispTextEditorEvents_LineChangedEventHandler(LineChanged_EventHandler);
                cb = new MyCallBack();
                cb.ChangeCaret += new ChangeCaretEventHandler(my_CaretChange);
                twice = false;
                //TODO: register to cb's events
                //TODO: add a different handler function for each of the events
                // examples: http://www.codeproject.com/Articles/20550/C-Event-Implementation-Fundamentals-Best-Practices
                
                //ts.NewLine();
                //ts.Insert("a");
            }
        }
        private void my_CaretChange(object sender, ChangeCaretEventArgs e)
        {

            if (twice == false)
            {
                twice = true;
                TextSelection ts2 = null;
                ts2 = DTE2.ActiveWindow.Project.ProjectItems.Item("Class2.cs").Document.Selection as TextSelection;
                string s = e.Location.Split(',')[3];
                ts2.MoveToLineAndOffset(int.Parse(e.Location.Split(',')[3]), int.Parse(e.Location.Split(',')[4]));
                ts2.Insert("oved");//TODO: Check event calling by insert and not by pressing
                
            }
            else
            {
                twice = false;
            }
        }
        private void KeyPress_EventHandler(string st, TextSelection ts, bool b, ref bool br)
        {
            int i = 1;
            /*
            int line = ts.ActivePoint.Line;
            int charoff = ts.ActivePoint.LineCharOffset;
            int stn = (int)st[0];
            TextSelection ts2 = null;
            
            /* Projects ps = DTE2.Solution.Projects;
             ProjectItem pi=null;
             foreach(Project p in ps)
             {
                 try
                 {

                     pi= p.ProjectItems.Item("Class2.cs");
                     pi.Open();
                     pi.Document.Activate();
                     ts2 = pi.Document.Selection as TextSelection;
                 }
                 catch
                 {
                    
                 }
             }*/
            /*
            ts2 = DTE2.ActiveWindow.Project.ProjectItems.Item("Class2.cs").Document.Selection as TextSelection;
            ts2.MoveToLineAndOffset(line, charoff, false);

            if (twice == false)
            {
                if (st[0] == '\b')
                {
                    twice = true;
                    ts2.DeleteLeft();
                }

                else if (st[0] == '\r')
                {
                    twice = true;
                    ts2.NewLine();
                }
                else if (st[0] == '\t')
                {
                    twice = true;
                    ts2.Indent(1);
                }
                else if (st[0] == 127)
                {
                    twice = true;
                    ts2.Delete();
                }
                else
                {
                    ts2.Insert(st);
                }
                // pi.Save();
                //  pi.Document.Close();
            }
            DTE2.ActiveDocument.Save();
            twice = false;
            */
            //countCalls++;
            //if (countCalls % 10 == 0)
            //{
            //    cb.getChange();
            //}
        }
        int countCalls = 0;
        private void LineChanged_EventHandler(TextPoint a, TextPoint b, int i)
        {
            //MessageBox.Show( Clipboard.GetText());   
            EnvDTE.TextSelection ts = dte.ActiveDocument.Selection as EnvDTE.TextSelection;
            //   EnvDTE.VirtualPoint vp = ts.ActivePoint;
            // System.Windows.Forms.MessageBox.Show(st);
            //DTE2.ActiveDocument.Save();
            string s = ts.Text;
            ITextCaret c = iwpf.TextView.Caret;
            MessageBox.Show("a:" + a.Line + "," + a.LineCharOffset + "," + a.DTE.ActiveDocument.Name + "\n b:" + b.Line + "," + b.LineCharOffset + b.DTE.ActiveDocument.Name + "\n" + i);
            int line = ts.ActivePoint.Line;
            int charoff = ts.ActivePoint.LineCharOffset;
            //cb.PrintIds();
            //cb.callService(a.Line + "," + a.LineCharOffset + "," + a.DTE.ActiveDocument.Name + "," + b.Line + "," + b.LineCharOffset + "," + b.DTE.ActiveDocument.Name);
            TextSelection ts2 = null;
            ts2 = DTE2.ActiveWindow.Project.ProjectItems.Item("Class2.cs").Document.Selection as TextSelection;
            ts2.MoveToLineAndOffset(line, charoff, false);
            DTE2.ActiveDocument.Save();
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

