using EnvDTE;
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
        public bool KeyWasPressed = false;
        public DTE2 DTE2
        {
            get { return dte ?? (dte = ServiceProvider.GlobalProvider.GetService(typeof(DTE)) as DTE2); }
        }
        public bool KeyPress
        {
            get { return KeyWasPressed; }
            set { KeyWasPressed = value; }
        }
        public Carets(IWpfTextViewHost h, MyCallBack cb)
        {
            if (DTE2.ActiveWindow != null)
            {
                iwpf = h;
                //tde = ((Events2)DTE2.Events).TextDocumentKeyPressEvents;
                //tde.BeforeKeyPress += new _dispTextDocumentKeyPressEvents_BeforeKeyPressEventHandler(KeyPress_EventHandler);
                te = ((Events2)DTE2.Events).TextEditorEvents;
                te.LineChanged += new _dispTextEditorEvents_LineChangedEventHandler(EnterFix);
                this.cb = cb;
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
        public void my_CaretChange(object sender, ChangeCaretEventArgs e)
        {

            //TextSelection ts2 = null;
            //ts2 = DTE2.ActiveWindow.Project.ProjectItems.Item("Class2.cs").Document.Selection as TextSelection;
            //string s = e.Location.Split(',')[3];
            //int line = ts2.ActivePoint.Line;
            //int offs = ts2.ActivePoint.LineCharOffset;
            //ts2.MoveToLineAndOffset(int.Parse(e.Location.Split(',')[3]), int.Parse(e.Location.Split(',')[4]));
            //ts2.Insert("oved");//TODO: Check event calling by insert and not by pressing
            //ts2.MoveToLineAndOffset(line, offs);


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
        //private void LineChanged_EventHandler(TextPoint a, TextPoint b, int i)
        //{
        //    //MessageBox.Show( Clipboard.GetText());   
        //    EnvDTE.TextSelection ts = dte.ActiveDocument.Selection as EnvDTE.TextSelection;
        //    //   EnvDTE.VirtualPoint vp = ts.ActivePoint;
        //    // System.Windows.Forms.MessageBox.Show(st);
        //    //DTE2.ActiveDocument.Save();
        //    string s = ts.Text;
        //    ITextCaret c = iwpf.TextView.Caret;
        //    MessageBox.Show("a:" + a.Line + "," + a.LineCharOffset + "," + a.DTE.ActiveDocument.Name + "\n b:" + b.Line + "," + b.LineCharOffset + b.DTE.ActiveDocument.Name + "\n" + i);
        //    int line = ts.ActivePoint.Line;
        //    int charoff = ts.ActivePoint.LineCharOffset;
        //    //cb.PrintIds();
        //    //cb.callService(a.Line + "," + a.LineCharOffset + "," + a.DTE.ActiveDocument.Name + "," + b.Line + "," + b.LineCharOffset + "," + b.DTE.ActiveDocument.Name);
        //    TextSelection ts2 = null;
        //    ts2 = DTE2.ActiveWindow.Project.ProjectItems.Item("Class2.cs").Document.Selection as TextSelection;
        //    ts2.MoveToLineAndOffset(line, charoff, false);
        //    DTE2.ActiveDocument.Save();
        //}
        private void EnterFix(TextPoint a, TextPoint b, int hint)
        {

            if (KeyWasPressed)
            {
                if ((b.AbsoluteCharOffset - a.AbsoluteCharOffset-1)%4 == 0)
                {
                    string st = "";
                    for (int i = 0; i < b.AbsoluteCharOffset - a.AbsoluteCharOffset-1; i++)
                    {
                        st += ' ';
                    }
                    cb.SendCaretPosition(dte.ActiveDocument.FullName, b.AbsoluteCharOffset-2, st);
                    cb.SendCaretPosition(dte.ActiveDocument.FullName, b.AbsoluteCharOffset+st.Length, "click");
                }
                KeyWasPressed = false;
                
            }
            //ts.Select(ts.AnchorPoint, ts.ActivePoint);
            // EnterWasPressed = false;

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

