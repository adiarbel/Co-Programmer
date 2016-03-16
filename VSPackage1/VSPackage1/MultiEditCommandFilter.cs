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
namespace Company.VSPackage1
{
    class MultiEditCommandFilter : IOleCommandTarget///helps to get IO of clicked buttons 
    {
        private IWpfTextView m_textView;//The text view we work on
        internal IOleCommandTarget m_nextTarget;//The next task to be called by the visual studio
        internal bool m_added;
        private IAdornmentLayer m_adornmentLayer;//Our adornment layer to work on
        private bool requiresHandling = false;
        List<ITrackingPoint> trackList = new List<ITrackingPoint>();
        private MyCallBack cb;
        private Carets crts;
        static Dispatcher uiDisp;
        public MultiEditCommandFilter(IWpfTextView textView, MyCallBack mcb, Carets cs)
        {
            m_textView = textView;
            m_adornmentLayer = m_textView.GetAdornmentLayer("MultiEditLayer");
            m_added = false;
            m_textView.LayoutChanged += m_textView_LayoutChanged;
            cb = mcb;
            crts = cs;
            cb.NewCaret += new NewCaretEventHandler(my_NewCaret);//how to send by parameter the NetworkClass ref
            cb.ChangeCaret += new ChangeCaretEventHandler(my_ChangedCaret);//how to send by parameter the NetworkClass ref
            uiDisp = Dispatcher.CurrentDispatcher;
            ITextDocument textDoc;
            var rc = m_textView.TextBuffer.Properties.TryGetProperty<ITextDocument>(
              typeof(ITextDocument), out textDoc);
            string st = crts.DTE2.Solution.FullName;
            st = st.Substring(st.LastIndexOf('\\') + 1);
            st = st.Split('.')[0];
            st = textDoc.FilePath.Substring(textDoc.FilePath.IndexOf(st));
            cb.callService(st, m_textView.Caret.Position.BufferPosition.Position);
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
            var curTrackPoint = m_textView.TextSnapshot.CreateTrackingPoint(int.Parse(e.Location),
            PointTrackingMode.Positive);
            trackList.Add(curTrackPoint);
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
            if(trackList.Count>0)
            trackList[0] = m_textView.TextSnapshot.CreateTrackingPoint(int.Parse(e.Location),
            PointTrackingMode.Positive);
        }
        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            RedrawScreen();
            requiresHandling = false;
            // When Alt Clicking, we need to add Edit points.
            //Debug.WriteLine("=====" + nCmdID + " " + pguidCmdGroup.ToString() + nCmdexecopt + " " + pvaIn.ToString() + " " + pvaOut.ToString(), "adi");
            if (pguidCmdGroup == VSConstants.VSStd2K && nCmdID == (uint)VSConstants.VSStd2KCmdID.ECMD_LEFTCLICK)
            {
                requiresHandling = true;
                RedrawScreen();
                ITextDocument textDoc;
                var rc = m_textView.TextBuffer.Properties.TryGetProperty<ITextDocument>(
                  typeof(ITextDocument), out textDoc);
                string st = crts.DTE2.Solution.FullName;
                st = st.Substring(st.LastIndexOf('\\') + 1);
                st = st.Split('.')[0];
                st = textDoc.FilePath.Substring(textDoc.FilePath.IndexOf(st));
                cb.SendCurrPos(st, m_textView.Caret.Position.BufferPosition.Position);

            }
            else if (pguidCmdGroup == VSConstants.VSStd2K && trackList.Count > 0 && (nCmdID == (uint)VSConstants.VSStd2KCmdID.TYPECHAR ||
                    nCmdID == (uint)VSConstants.VSStd2KCmdID.BACKSPACE ||
                    nCmdID == (uint)VSConstants.VSStd2KCmdID.TAB ||
                    nCmdID == (uint)VSConstants.VSStd2KCmdID.UP ||
                    nCmdID == (uint)VSConstants.VSStd2KCmdID.DOWN ||
                    nCmdID == (uint)VSConstants.VSStd2KCmdID.LEFT ||
                    nCmdID == (uint)VSConstants.VSStd2KCmdID.RIGHT
            ))
                requiresHandling = true;
            if (requiresHandling == true)
            {

                if (pguidCmdGroup == VSConstants.VSStd2K && nCmdID == (uint)VSConstants.VSStd2KCmdID.TYPECHAR)
                {
                    var typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
                    //InsertSyncedChar(typedChar.ToString());
                    RedrawScreen();
                }
                else if (pguidCmdGroup == VSConstants.VSStd2K && nCmdID == (uint)VSConstants.VSStd2KCmdID.BACKSPACE)
                {
                    var typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
                    // SyncedBackSpace();
                    RedrawScreen();
                }
                else if (pguidCmdGroup == VSConstants.VSStd2K && nCmdID == (uint)VSConstants.VSStd2KCmdID.DELETE)
                {
                    var typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
                    // SyncedDelete();
                    RedrawScreen();
                }
            }
            return m_nextTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            return m_nextTarget.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }

        private void RedrawScreen()
        {
            m_adornmentLayer.RemoveAllAdornments();
            for (int i = 0; i < trackList.Count; i++)
            {
                var curTrackPoint = trackList[i];
                DrawSingleSyncPoint(curTrackPoint);
            }
        }

        private void AddSyncPoint()
        {
            CaretPosition curPosition = m_textView.Caret.Position;
            var curTrackPoint = m_textView.TextSnapshot.CreateTrackingPoint(curPosition.BufferPosition.Position,
            PointTrackingMode.Positive);
            trackList.Add(curTrackPoint);
        }
        private void DrawSingleSyncPoint(ITrackingPoint curTrackPoint)
        {
            SnapshotSpan span;
            span = new SnapshotSpan(curTrackPoint.GetPoint(m_textView.TextSnapshot), 1);
            var brush = new SolidColorBrush(Colors.Plum);
            var g = m_textView.TextViewLines.GetLineMarkerGeometry(span);
            GeometryDrawing drawing = new GeometryDrawing(brush, null, g);
            if (drawing.Bounds.IsEmpty)
                return;

            System.Windows.Shapes.Rectangle r = new System.Windows.Shapes.Rectangle()
            {
                Fill = brush,
                Width = drawing.Bounds.Width / 2,
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
            trackList.Clear();
            m_adornmentLayer.RemoveAllAdornments();
        }
        private void InsertSyncedChar(string inputString)
        {
            // Avoiding inserting the character for the last edit point, as the Caret is there and
            // the default IDE behavior will insert the text as expected.
            uiDisp.Invoke(new Action(() =>
                {
                    ITextEdit edit = m_textView.TextBuffer.CreateEdit();
                    for (int i = 0; i < trackList.Count - 1; i++)
                    {
                        var curTrackPoint = trackList[i];
                        edit.Insert(curTrackPoint.GetPosition(m_textView.TextSnapshot), inputString);
                    }
                    edit.Apply();
                    edit.Dispose();
                }));
        }
    }
}