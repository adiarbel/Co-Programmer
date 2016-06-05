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
    class CoProCommandFilter : IOleCommandTarget///helps to get IO of clicked buttons 
    {
        private IWpfTextView m_textView;//The text view we work on
        internal IOleCommandTarget m_nextTarget;//The next task to be called by the visual studio
        internal bool m_added;
        private IAdornmentLayer m_adornmentLayer;//adornment layer to work on
        Dictionary<string, ITrackingPoint> trackDict = new Dictionary<string, ITrackingPoint>();//tracking point of current text view editors
        List<SolidColorBrush> brushes = new List<SolidColorBrush>();//brushes of the editors' carets
        internal CoProNetwork cb;// client's object
        internal GraphicObjects gobj;//graphic objects item
        static Dispatcher uiDisp;// dispatcher of main thread in order for other threads do ui manipulations
        static bool isFirstTime = true;// flag for first load of a textview
        bool mySideCalling = true;// flag to determine which side the event/function is called from
        bool delayFixer = false;// delay fixer flag for sending caret positions
        int currBufferSize;// current buffer size of the file
        string filename;// the name of the file
        Events events;// DTE events object 
        DocumentEvents saveEvent;//DocumentEvents object used for saving events

        /// <summary>
        /// Initialization of UI and editing related events and members
        /// </summary>
        /// <param name="textView"></param>
        /// <param name="mcb"></param>
        /// <param name="cs"></param>
        public CoProCommandFilter(IWpfTextView textView, CoProNetwork mcb, GraphicObjects cs)
        {
            m_textView = textView;
            m_adornmentLayer = m_textView.GetAdornmentLayer("MultiEditLayer");
            m_added = false;
            m_textView.LayoutChanged += m_textView_LayoutChanged;
            cb = mcb;
            gobj = cs;
            //crts.DTE2.Events.TextEditorEvents.LineChanged += new _dispTextEditorEvents_LineChangedEventHandler();
            events = gobj.DTE2.Events;
            saveEvent = events.DocumentEvents;
            saveEvent.DocumentSaved += new _dispDocumentEvents_DocumentSavedEventHandler(my_DocWasSaved);
            cb.NewCaret += new NewCaretEventHandler(my_NewCaret);
            cb.ChangeCaret += new ChangeCaretEventHandler(my_ChangedCaret);
            cb.EditorDisc += new EditorDisconnectedEventHandler(my_EditorDisc);
            cb.NewText += new NewTextEventHandler(my_AddedText);
            cb.RemovedText += new RemovedTextEventHandler(my_RemovedText);
            cb.SaveEvent += new SaveEventHandler(my_Save);
            cb.AddAllEditors += new AddCurrentEditorsEventHandler(my_AddEditors);
            cb.ItemRemoved += new ItemRemovedEventHandler(my_RemovedItem);
            cb.ItemAdded += new NewItemAddedEventHandler(my_ItemAdded);
            textView.Caret.PositionChanged += new EventHandler<CaretPositionChangedEventArgs>(my_PositionChanged);
            textView.TextBuffer.Changed += TextBuffer_Changed;
            InitBrushes();
            uiDisp = Dispatcher.CurrentDispatcher;
            ITextDocument textDoc;
            var rc = m_textView.TextBuffer.Properties.TryGetProperty<ITextDocument>(
              typeof(ITextDocument), out textDoc);
            filename = gobj.DTE2.Solution.FullName;
            filename = filename.Substring(filename.LastIndexOf('\\') + 1);
            filename = filename.Split('.')[0];
            filename = textDoc.FilePath.Substring(textDoc.FilePath.IndexOf(filename));
            if (cb.IsAdmin)
            {
                cb.AdminEvent += my_AdminCallback;
            }
            if (isFirstTime)
            {
                isFirstTime = false;
            }
            else
            {
                if (!cb.IsAdmin)
                {
                    cb.UpdateSpecificFile(filename);
                }
            }
            filename = filename.Substring(filename.LastIndexOf('\\') + 1);
            cb.IntializePosition(filename, m_textView.Caret.Position.BufferPosition.Position, cb.Name);

            currBufferSize = m_textView.TextSnapshot.Length;
        }

        /// <summary>
        /// Handler for changing position in the text file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void my_PositionChanged(object sender, CaretPositionChangedEventArgs e)
        {
            RedrawScreen();
            //https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.text.editor.itextcaret.positionchanged.aspx
            cb.SendCaretPosition(filename, e.NewPosition.BufferPosition.Position, "click");
            cb.ExpectedSequence++;
        }

        /// <summary>
        /// Handler for saving the text file
        /// </summary>
        /// <param name="target"></param>
        void my_DocWasSaved(Document target)
        {
            if (mySideCalling)
            {
                cb.SendCaretPosition(filename, 0, "save");
            }
            mySideCalling = true;
        }

        /// <summary>
        /// Handler for admin open file action - in order for the project items to be 
        /// synced the admin should always open the file when even one user uses them 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void my_AdminCallback(object sender, AdminEventArgs e)
        {
            if (!gobj.DTE2.Solution.Projects.Item(gobj.DTE2.ActiveDocument.ProjectItem.ContainingProject.UniqueName).ProjectItems.Item(e.File).IsOpen)
            {
                Window w = null;
                uiDisp.Invoke(new Action(() =>
                        {
                            w = gobj.DTE2.Solution.Projects.Item(gobj.DTE2.ActiveDocument.ProjectItem.ContainingProject.UniqueName).ProjectItems.Item(e.File).Open(EnvDTE.Constants.vsViewKindTextView);

                        }));
                w.Activate();
                w.Visible = false;
            }
        }

        /// <summary>
        /// Handler for changes on the buffer deletion/insertion of text 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TextBuffer_Changed(object sender, TextContentChangedEventArgs e)
        {
            if (mySideCalling)
            {

                for (int i = 0; i < e.Changes.Count; i++)
                {
                    if (e.Changes[i].OldText != "")
                    {
                        cb.SendCaretPosition(filename, e.Changes[i].NewPosition, "DELETE;" + e.Changes[i].OldSpan.Length + ";");
                        cb.ExpectedSequence++;
                    }
                    if (e.Changes[i].NewText != "")
                    {
                        cb.SendCaretPosition(filename, e.Changes[i].NewPosition, e.Changes[i].NewText);
                        cb.ExpectedSequence++;
                    }
                }
            }
            mySideCalling = true;
        }

        /// <summary>
        /// Handler for net side saving
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void my_Save(object sender, ChangeCaretEventArgs e)
        {
            if (e.File == filename)
            {
                if (e.File == "all")
                {
                    gobj.DTE2.ActiveWindow.Project.Save();
                }
                else
                {
                    var b = gobj.DTE2.ActiveWindow.Project.ProjectItems.Item(e.File).IsOpen;
                    if (b)
                    {
                        gobj.DTE2.ActiveWindow.Project.ProjectItems.Item(e.File).Save();
                    }
                }
                mySideCalling = false;
            }
        }

        /// <summary>
        /// Handler for new editor on the file 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void my_NewCaret(object sender, EditedTextEventArgs e)
        {

            if (e.File == filename)
            {
                lock (CoProNetwork.locker)
                {
                    while (e.Seq != cb.ExpectedSequence)//if excpected id is the id i got
                    {
                        System.Threading.Monitor.Wait(CoProNetwork.locker);
                        Debug.WriteLine("Recieved seq : " + e.Seq + " Expected seq : " + cb.ExpectedSequence);
                    }
                    var curTrackPoint = m_textView.TextSnapshot.CreateTrackingPoint(e.Location,
                    PointTrackingMode.Positive);
                    trackDict[e.Editor] = curTrackPoint;
                    System.Threading.Monitor.PulseAll(CoProNetwork.locker);
                }
            }
            else
            {
                trackDict[e.Editor] = null;
            }
        }

        /// <summary>
        /// Handler for changing caret of an editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void my_ChangedCaret(object sender, EditedTextEventArgs e)
        {

            if (e.File == filename)
            {
                lock (CoProNetwork.locker)
                {
                    while (e.Seq != cb.ExpectedSequence)//if excpected id is the id i got
                    {
                        System.Threading.Monitor.Wait(CoProNetwork.locker);
                        Debug.WriteLine("Recieved seq : " + e.Seq + " Expected seq : " + cb.ExpectedSequence);
                    }

                    if (e.Location == 1 || e.Location == -1)
                    {
                        trackDict[e.Editor] = m_textView.TextSnapshot.CreateTrackingPoint(e.Location + trackDict[e.Editor].GetPosition(m_textView.TextSnapshot),
                        PointTrackingMode.Positive);
                    }
                    else
                    {
                        trackDict[e.Editor] = m_textView.TextSnapshot.CreateTrackingPoint(e.Location,
                        PointTrackingMode.Positive);
                    }

                    System.Threading.Monitor.PulseAll(CoProNetwork.locker);
                }
            }
            else
            {
                trackDict[e.Editor] = null;
            }

        }

        /// <summary>
        /// Handler for adding current editors of file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void my_AddEditors(object sender, AddEditorsEventArgs e)
        {
            string s;
            for (int i = 0; i < e.Editors.Length; i++)
            {
                s = gobj.DTE2.ActiveDocument.FullName;
                s = e.Locations[i].Split(' ')[1];
                if (e.Locations[i].Split(' ')[0] == filename)
                {
                    trackDict[e.Editors[i]] = m_textView.TextSnapshot.CreateTrackingPoint(int.Parse(s),
                    PointTrackingMode.Positive);//Have to change textview when it is another file!!!
                }
            }
        }

        /// <summary>
        /// Handler for an editor disconnection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void my_EditorDisc(object sender, EditorDisEventArgs e)
        {
            trackDict.Remove(e.Editor);
            RedrawScreen();
        }

        /// <summary>
        /// Handler for added text of other editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void my_AddedText(object sender, EditedTextEventArgs e)
        {
            if (e.File == filename)
            {
                lock (CoProNetwork.locker)
                {
                    while (e.Seq != cb.ExpectedSequence)//if excpected id is the id i got
                    {
                        System.Threading.Monitor.Wait(CoProNetwork.locker);
                        Debug.WriteLine("Recieved seq : " + e.Seq + " Expected seq : " + cb.ExpectedSequence);
                    }

                    trackDict[e.Editor] = m_textView.TextSnapshot.CreateTrackingPoint(e.Location,
                                            PointTrackingMode.Positive);
                    uiDisp.Invoke(new Action(() =>
                        {
                            ITextEdit edit = m_textView.TextBuffer.CreateEdit();

                            var curTrackPoint = trackDict[e.Editor];


                            edit.Insert(curTrackPoint.GetPosition(m_textView.TextSnapshot), e.Command);

                            mySideCalling = false;
                            edit.Apply();
                            edit.Dispose();


                        }));
                    System.Threading.Monitor.PulseAll(CoProNetwork.locker);
                }
            }
        }

        /// <summary>
        ///  Handler for removed text by other editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void my_RemovedText(object sender, EditedTextEventArgs e)
        {
            if (e.File == filename)
            {
                lock (CoProNetwork.locker)
                {
                    while (e.Seq != cb.ExpectedSequence)//if excpected id is the id i got
                    {
                        System.Threading.Monitor.Wait(CoProNetwork.locker);
                        Debug.WriteLine("Recieved seq : " + e.Seq + " Expected seq : " + cb.ExpectedSequence);
                    }


                    trackDict[e.Editor] = m_textView.TextSnapshot.CreateTrackingPoint(e.Location,
                                            PointTrackingMode.Positive);
                    uiDisp.Invoke(new Action(() =>
                    {
                        ITextEdit edit = m_textView.TextBuffer.CreateEdit();

                        var curTrackPoint = trackDict[e.Editor];

                        edit.Delete(e.Location, int.Parse(e.Command.Split(';')[1]));

                        mySideCalling = false;
                        edit.Apply();
                        edit.Dispose();
                    }));
                    System.Threading.Monitor.PulseAll(CoProNetwork.locker);
                }
            }
        }

        /// <summary>
        /// Handler for a removed item by other editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void my_RemovedItem(object sender, ItemRemovedEventArgs e)
        {

            CoProFilterProvider.MySide = false;
            if (e.IsDeleted)
            {
                EnvDTE.Projects ps = gobj.DTE2.Solution.Projects;
                foreach (EnvDTE.Project p in ps)
                {
                    string pname = p.Name;
                    if (p.Name.Contains(e.Project))
                    {
                        p.ProjectItems.Item(e.Name).Delete();
                        break;
                    }
                }
            }
            else
            {
                EnvDTE.Projects ps = gobj.DTE2.Solution.Projects;
                foreach (EnvDTE.Project p in ps)
                {
                    string pname = p.Name;
                    if (p.Name.Contains(e.Project))
                    {
                        p.ProjectItems.Item(e.Name).Remove();
                        break;
                    }
                }
            }
            CoProFilterProvider.MySide = true;
        }

        /// <summary>
        /// Handler for item added by other editor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void my_ItemAdded(object sender, NewItemAddedEventArgs e)
        {

            File.WriteAllBytes(cb.ProjPath + e.RelPath, e.Content);
            CoProFilterProvider.MySide = false;
            EnvDTE.Projects ps = gobj.DTE2.Solution.Projects;
            foreach (EnvDTE.Project p in ps)
            {
                string pname = p.Name;
                if (p.Name.Contains(e.Project))
                {
                    p.ProjectItems.AddFromTemplate(cb.ProjPath + e.RelPath, e.Name);
                    break;
                }
            }
            CoProFilterProvider.MySide = true;
        }

        /// <summary>
        /// Exec fucntion - called every time as scheduled by VS system to do reapted checks and actions
        /// </summary>
        /// <param name="pguidCmdGroup"></param>
        /// <param name="nCmdID"></param>
        /// <param name="nCmdexecopt"></param>
        /// <param name="pvaIn"></param>
        /// <param name="pvaOut"></param>
        /// <returns></returns>
        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            RedrawScreen();
            return m_nextTarget.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }

        /// <summary>
        /// Inner interface function for the COM object, to query the object for
        /// the status of one or more commands generated by user interface events.
        /// </summary>
        /// <param name="pguidCmdGroup"></param>
        /// <param name="cCmds"></param>
        /// <param name="prgCmds"></param>
        /// <param name="pCmdText"></param>
        /// <returns></returns>
        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            return m_nextTarget.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }

        /// <summary>
        /// Redraw screens and carets
        /// </summary>
        private void RedrawScreen()
        {
            try
            {
                uiDisp.Invoke(new Action(() =>
                    {
                        m_adornmentLayer.RemoveAllAdornments();
                        int i = 0;
                        foreach (KeyValuePair<string, ITrackingPoint> entry in trackDict)
                        {
                            var curTrackPoint = trackDict[entry.Key];
                            if (curTrackPoint != null)
                            {
                                DrawSingleSyncPoint(curTrackPoint, brushes[i]);
                                i++;
                            }
                        }
                    }));
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// Draw a caret in its position
        /// </summary>
        /// <param name="curTrackPoint"></param>
        /// <param name="brush"></param>
        private void DrawSingleSyncPoint(ITrackingPoint curTrackPoint, SolidColorBrush brush)
        {
            SnapshotSpan span;
            SnapshotPoint tempSnapPoint = curTrackPoint.GetPoint(m_textView.TextSnapshot);
            if (tempSnapPoint.Position != m_textView.TextSnapshot.Length)
            {
                //m_textView.TextSnapshot.TextBuffer.Insert(tempSnapPoint.Position, " ");
                //tempSnapPoint = tempSnapPoint.Subtract(1);
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

        }

        /// <summary>
        /// Handler for layout changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_textView_LayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            RedrawScreen();
        }

        /// <summary>
        /// Initializes brush list with colors
        /// </summary>
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