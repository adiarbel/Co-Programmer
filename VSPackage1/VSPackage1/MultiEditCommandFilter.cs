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
        public MultiEditCommandFilter(IWpfTextView textView)
        {
            m_textView = textView;
            m_adornmentLayer = m_textView.GetAdornmentLayer("MultiEditLayer");
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            requiresHandling = false;
            // When Alt Clicking, we need to add Edit points.
            Debug.WriteLine("=====" + nCmdID + " " + pguidCmdGroup.ToString() + nCmdexecopt + " " + pvaIn.ToString() + " " + pvaOut.ToString(), "adi");
            if (pguidCmdGroup == VSConstants.VSStd2K && nCmdID == (uint)VSConstants.VSStd2KCmdID.ECMD_LEFTCLICK && Keyboard.Modifiers == ModifierKeys.Alt)
            {
                requiresHandling = true;

            }
            if (requiresHandling == true)
            {
                // Capture Alt Left Click, only when the Box Selection mode hasn't been used (After Drag-selecting)
                if (pguidCmdGroup == VSConstants.VSStd2K && nCmdID == (uint)VSConstants.VSStd2KCmdID.ECMD_LEFTCLICK &&
                                                            Keyboard.Modifiers == ModifierKeys.Alt)
                {
                    // Add a Edit point, show it Visually 
                    AddSyncPoint();
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
        }

        private void AddSyncPoint()
        {
            CaretPosition curPosition = m_textView.Caret.Position;
            var curTrackPoint = m_textView.TextSnapshot.CreateTrackingPoint(curPosition.BufferPosition.Position,
            PointTrackingMode.Positive);
            trackList.Add(curTrackPoint);
        }
    }
}
