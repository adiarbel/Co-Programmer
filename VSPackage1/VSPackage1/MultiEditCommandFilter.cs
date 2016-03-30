using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Text.Editor;
using System.Windows.Input;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using System.Windows.Media;
using System.Windows.Controls;
using System.Diagnostics;
using System.IO;
using System.Windows.Threading;
using EnvDTE;
using EnvDTE80;
using EnvDTE100;
namespace Company.VSPackage1
{
    class MultiEditCommandFilter : IOleCommandTarget///helps to get IO of clicked buttons 
    {
        private IWpfTextView m_textView;//The text view we work on
        internal IOleCommandTarget m_nextTarget;//The next task to be called by the visual studio
        internal bool m_added;
        private IAdornmentLayer m_adornmentLayer;//Our adornment layer to work on
        private bool requiresHandling = false;
        Dictionary<string, ITrackingPoint> trackDict = new Dictionary<string, ITrackingPoint>();
        List<SolidColorBrush> brushes = new List<SolidColorBrush>();
        internal MyCallBack cb;
        internal Carets crts;
        static Dispatcher uiDisp;
        static bool isFirst = true;
        int currSizeBuffer;
        public MultiEditCommandFilter(IWpfTextView textView, MyCallBack mcb, Carets cs)
        {
            m_textView = textView;
            m_adornmentLayer = m_textView.GetAdornmentLayer("MultiEditLayer");
            m_added = false;
            m_textView.LayoutChanged += m_textView_LayoutChanged;
            cb = mcb;
            crts = cs;
            //crts.DTE2.Events.TextEditorEvents.LineChanged += new _dispTextEditorEvents_LineChangedEventHandler();
            if (isFirst)
            {
                cb.NewCaret += new NewCaretEventHandler(my_NewCaret);//how to send by parameter the NetworkClass ref
                cb.ChangeCaret += new ChangeCaretEventHandler(my_ChangedCaret);//how to send by parameter the NetworkClass ref
                cb.AddAllEditors += new AddCurrentEditorsEventHandler(my_AddEditors);
                cb.EditorDisc += new EditorDisconnectedEventHandler(my_EditorDisc);
                cb.NewText += new NewTextEventHandler(my_AddedText);
                cb.RemovedText += new RemovedTextEventHandler(my_RemovedText);
                isFirst = false;
            }
            InitBrushes();
            uiDisp = Dispatcher.CurrentDispatcher;
            ITextDocument textDoc;
            var rc = m_textView.TextBuffer.Properties.TryGetProperty<ITextDocument>(
              typeof(ITextDocument), out textDoc);
            string st = crts.DTE2.Solution.FullName;
            st = st.Substring(st.LastIndexOf('\\') + 1);
            st = st.Split('.')[0];
            st = textDoc.FilePath.Substring(textDoc.FilePath.IndexOf(st));
            cb.IntializePosition(st, m_textView.Caret.Position.BufferPosition.Position);

            currSizeBuffer = m_textView.TextSnapshot.Length;
        }
        private void my_NewCaret(object sender, ChangeCaretEventArgs e)
        {
            //ITextDocument textDoc;
            //var rc = m_textView.TextBuffer.Properties.TryGetProperty<ITextDocument>(
            //  typeof(ITextDocument), out textDoc);
            //string s = textDoc.FilePath.Substring(textDoc.FilePath.LastIndexOf('\\'));//gets the file only
            //if (rc == true)
            //    if (e.File == textDoc.FilePath.Substring(textDoc.FilePath.LastIndexOf('\\') + 1))
            //        crts.my_CaretChange(sender, e);//helps me to find which file the caret is in
            var curTrackPoint = m_textView.TextSnapshot.CreateTrackingPoint(e.Location,
            PointTrackingMode.Positive);
            trackDict[e.Editor] = curTrackPoint;
        }
        private void my_ChangedCaret(object sender, ChangeCaretEventArgs e)
        {
            //ITextDocument textDoc;
            //var rc = m_textView.TextBuffer.Properties.TryGetProperty<ITextDocument>(
            //  typeof(ITextDocument), out textDoc);
            //string s = textDoc.FilePath.Substring(textDoc.FilePath.LastIndexOf('\\'));//gets the file only
            //if (rc == true)
            //    if (e.File == textDoc.FilePath.Substring(textDoc.FilePath.LastIndexOf('\\') + 1))
            //        crts.my_CaretChange(sender, e);//helps me to find which file the caret is in
            if (trackDict.Count > 0)
            {
                if(e.Location==1||e.Location==-1)
                {
                    trackDict[e.Editor] = m_textView.TextSnapshot.CreateTrackingPoint(e.Location + trackDict[e.Editor].GetPosition(m_textView.TextSnapshot),
                    PointTrackingMode.Positive);
                }
                else
                {
                    trackDict[e.Editor] = m_textView.TextSnapshot.CreateTrackingPoint(e.Location,
                    PointTrackingMode.Positive);
                }
            }
        }
        private void my_AddEditors(object sender, AddEditorsEventArgs e)
        {
            string s;
            for (int i = 0; i < e.Editors.Length; i++)
            {
                s = crts.DTE2.ActiveDocument.FullName;
                s = e.Locations[i].Split(' ')[1];
                trackDict[e.Editors[i]] = m_textView.TextSnapshot.CreateTrackingPoint(int.Parse(s),
                PointTrackingMode.Positive);//Have to change textview when it is another file!!!
            }
        }
        private void my_EditorDisc(object sender, EditorDisEventArgs e)
        {
            trackDict.Remove(e.Editor);
            RedrawScreen();
        }
        private void my_AddedText(object sender, ChangeCaretEventArgs e)
        {
            my_ChangedCaret(sender, e);
            uiDisp.Invoke(new Action(() =>
                {
                    ITextEdit edit = m_textView.TextBuffer.CreateEdit();

                    var curTrackPoint = trackDict[e.Editor];
                    edit.Insert(curTrackPoint.GetPosition(m_textView.TextSnapshot), e.Command);
                    edit.Apply();
                    edit.Dispose();
                }));
        }
        private void my_RemovedText(object sender, ChangeCaretEventArgs e)
        {
            my_ChangedCaret(sender, e);
            uiDisp.Invoke(new Action(() =>
            {
                ITextEdit edit = m_textView.TextBuffer.CreateEdit();

                var curTrackPoint = trackDict[e.Editor];
                if(e.Command.Contains("sel"))
                {
                    edit.Delete(e.Location, int.Parse(e.Command.Split(';')[2]));
                }
                else if (e.Command.Contains("DELETE"))
                {
                    edit.Delete(e.Location, int.Parse(e.Command.Split(';')[1]));
                }
                else if(e.Command.Contains("BACKSPACE")&&e.Location!=0)
                {
                    edit.Delete(e.Location - int.Parse(e.Command.Split(';')[1]), int.Parse(e.Command.Split(';')[1]));
                }
                edit.Apply();
                edit.Dispose();
            }));
        }
        
        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            ITextDocument textDoc;
            var rc = m_textView.TextBuffer.Properties.TryGetProperty<ITextDocument>(
              typeof(ITextDocument), out textDoc);
            string filename = crts.DTE2.Solution.FullName;
            filename = filename.Substring(filename.LastIndexOf('\\') + 1);
            filename = filename.Split('.')[0];
            filename = textDoc.FilePath.Substring(textDoc.FilePath.IndexOf(filename));
            RedrawScreen();
            requiresHandling = false;
            // When Alt Clicking, we need to add Edit points.
            Debug.WriteLine("=====" + nCmdID + " " + pguidCmdGroup.ToString() + nCmdexecopt + " " + pvaIn.ToString() + " " + pvaOut.ToString(), "adi");
            Debug.WriteLine((uint)VSConstants.VSStd2KCmdID.RETURN);
            if (pguidCmdGroup == VSConstants.VSStd2K && nCmdID == (uint)VSConstants.VSStd2KCmdID.ECMD_LEFTCLICK)
            {
                requiresHandling = true;
                RedrawScreen();

                cb.SendCaretPosition(filename, m_textView.Caret.Position.BufferPosition.Position, "click");

            }
            else if(pguidCmdGroup == VSConstants.VSStd2K && 
                    nCmdID == (uint)VSConstants.VSStd2KCmdID.UP ||
                    nCmdID == (uint)VSConstants.VSStd2KCmdID.DOWN ||
                    nCmdID == (uint)VSConstants.VSStd2KCmdID.LEFT ||
                    nCmdID == (uint)VSConstants.VSStd2KCmdID.RIGHT)
            {
                cb.SendCaretPosition(filename, m_textView.Caret.Position.BufferPosition.Position, "click");

            }
            else if (pguidCmdGroup == VSConstants.VSStd2K && trackDict.Count > 0 && (nCmdID == (uint)VSConstants.VSStd2KCmdID.TYPECHAR ||
                    nCmdID == (uint)VSConstants.VSStd2KCmdID.BACKSPACE ||
                    nCmdID == (uint)VSConstants.VSStd2KCmdID.TAB ||
                    nCmdID == (uint)VSConstants.VSStd2KCmdID.RETURN
            ))
            {
                requiresHandling = true;
                crts.KeyPress = true;
            }
            else if (nCmdID == 17)//DELETE pressed
            {
                requiresHandling = true;
            }
            if (requiresHandling == true)
            {

                if (pguidCmdGroup == VSConstants.VSStd2K && nCmdID == (uint)VSConstants.VSStd2KCmdID.TYPECHAR)
                {
                    var typedChar = ((char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn)).ToString();
                    //if (Math.Abs(m_textView.Selection.End.Position - m_textView.Selection.Start.Position) > 0)
                    //{
                    //    typedChar = typedChar + ";del;" + m_textView.Selection.Start.Position + ";" + m_textView.Selection.End.Position + ";";
                    //    cb.SendCaretPosition(filename, m_textView.Selection.Start.Position, typedChar);
                    //}
                    //else
                    //{
                    //    cb.SendCaretPosition(filename, m_textView.Caret.Position.BufferPosition.Position, typedChar);
                    //}
                    cb.SendCaretPosition(filename, m_textView.Caret.Position.BufferPosition.Position, typedChar);
                    //InsertSyncedChar(typedChar.ToString());
                    RedrawScreen();
                }
                else if (pguidCmdGroup == VSConstants.VSStd2K && nCmdID == (uint)VSConstants.VSStd2KCmdID.BACKSPACE)
                {
                    if (Math.Abs(m_textView.Selection.End.Position - m_textView.Selection.Start.Position) > 0)
                    {

                        cb.SendCaretPosition(filename, m_textView.Selection.Start.Position, "BACKSPACE;sel;" + (m_textView.Selection.End.Position - m_textView.Selection.Start.Position) + ";");
                    }
                    else
                    {
                        if((crts.DTE2.ActiveDocument.Selection as TextSelection).ActivePoint.LineCharOffset==1)
                        {
                            cb.SendCaretPosition(filename, m_textView.Caret.Position.BufferPosition.Position, "BACKSPACE;2;");
                        }
                        else
                        {
                            cb.SendCaretPosition(filename, m_textView.Caret.Position.BufferPosition.Position, "BACKSPACE;1;");
                        }
                    }
                    // SyncedBackSpace();
                    RedrawScreen();
                }
                else if (nCmdID == 17)
                {
                    if (Math.Abs(m_textView.Selection.End.Position - m_textView.Selection.Start.Position) > 0)
                    {

                        cb.SendCaretPosition(filename, m_textView.Selection.Start.Position, "DELETE;sel" +(m_textView.Selection.End.Position - m_textView.Selection.Start.Position)+ ";");
                    }
                    else
                    {
                        cb.SendCaretPosition(filename, m_textView.Caret.Position.BufferPosition.Position, "DELETE;1;");
                    }
                    // SyncedDelete();
                    RedrawScreen();
                }
                else if (pguidCmdGroup == VSConstants.VSStd2K && nCmdID == (uint)VSConstants.VSStd2KCmdID.RETURN)
                {
                    //var typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
                    cb.SendCaretPosition(filename, m_textView.Caret.Position.BufferPosition.Position, "\r\n");
                    //InsertSyncedChar(typedChar.ToString());
                   
                    RedrawScreen();
                }
            } 
            if (currSizeBuffer != m_textView.TextSnapshot.Length)
            {
                currSizeBuffer = m_textView.TextSnapshot.Length;
            }
            return m_nextTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            return m_nextTarget.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }

        private void RedrawScreen()
        {
            uiDisp.Invoke(new Action(() =>
                {
                    m_adornmentLayer.RemoveAllAdornments();
                    int i = 0;
                    foreach (KeyValuePair<string, ITrackingPoint> entry in trackDict)
                    {
                        var curTrackPoint = trackDict[entry.Key];
                        DrawSingleSyncPoint(curTrackPoint, brushes[i]);
                        i++;
                    }
                }));
           
        }

        private void AddSyncPoint()
        {
            CaretPosition curPosition = m_textView.Caret.Position;
            var curTrackPoint = m_textView.TextSnapshot.CreateTrackingPoint(curPosition.BufferPosition.Position,
            PointTrackingMode.Positive);
            //trackDict.Add(curTrackPoint);
        }
        private void DrawSingleSyncPoint(ITrackingPoint curTrackPoint, SolidColorBrush brush)
        {
            SnapshotSpan span;
            SnapshotPoint tempSnapPoint =curTrackPoint.GetPoint(m_textView.TextSnapshot);
            if (tempSnapPoint.Position == m_textView.TextSnapshot.Length)
            {
                m_textView.TextSnapshot.TextBuffer.Insert(tempSnapPoint.Position, " ");
                tempSnapPoint = tempSnapPoint.Subtract(1);
            }

            span = new SnapshotSpan(tempSnapPoint, 1);
            var g = m_textView.TextViewLines.GetLineMarkerGeometry(span);
            GeometryDrawing drawing = new GeometryDrawing(brush, null, g);
            if (drawing.Bounds.IsEmpty)
                return;

            System.Windows.Shapes.Rectangle r = new System.Windows.Shapes.Rectangle()
            {
                Fill = brush,
                Width = drawing.Bounds.Width / 4,
                Height = drawing.Bounds.Height
            };
            Canvas.SetLeft(r, g.Bounds.Left);
            Canvas.SetTop(r, g.Bounds.Top);
            m_adornmentLayer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, "MultiEditLayer", r, null);

        }
        private void m_textView_LayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            RedrawScreen();
        }
        private void ClearSyncPoints()
        {
            trackDict.Clear();
            m_adornmentLayer.RemoveAllAdornments();
        }
        private void InsertSyncedChar(string inputString)
        {
            // Avoiding inserting the character for the last edit point, as the Caret is there and
            // the default IDE behavior will insert the text as expected.
            uiDisp.Invoke(new Action(() =>
                {
                    ITextEdit edit = m_textView.TextBuffer.CreateEdit();
                    foreach (KeyValuePair<string, ITrackingPoint> entry in trackDict)
                    {
                        var curTrackPoint = trackDict[entry.Key];
                        edit.Insert(curTrackPoint.GetPosition(m_textView.TextSnapshot), inputString);
                    }
                    edit.Apply();
                    edit.Dispose();
                }));
        }
        private void InitBrushes()
        {
            brushes.Add(new SolidColorBrush(Colors.Aqua));
            brushes.Add(new SolidColorBrush(Colors.LimeGreen));
            brushes.Add(new SolidColorBrush(Colors.Plum));
            brushes.Add(new SolidColorBrush(Colors.Coral));
            brushes.Add(new SolidColorBrush(Colors.Gold));
            brushes.Add(new SolidColorBrush(Colors.Fuchsia));
            brushes.Add(new SolidColorBrush(Colors.Blue));

        }
    }
}