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
using System.Xml.Linq;
using System.Xml;
using Microsoft.Win32;


namespace Company.VSPackage1
{
    class GraphicObjects
    {
        DTE2 dte;
        IWpfTextViewHost iwpf;
        WindowEvents we;
        CoProNetwork cb;
        CoProExplorer coproExplorer;

        /// <summary>
        /// Properties of members
        /// </summary>
        public DTE2 DTE2
        {
            get { return dte ?? (dte = ServiceProvider.GlobalProvider.GetService(typeof(DTE)) as DTE2); }
        }
        public CoProExplorer CoProExplorer
        {
            get { return (CoProExplorer)(coproExplorer); }
        }

        /// <summary>
        /// sets the objects that would be accessed later by other classes
        /// </summary>
        /// <param name="h">iwpftextviewhost of the textview</param>
        /// <param name="cb">the client network object</param>
        /// <param name="cpe">the explorer management object</param>
        public GraphicObjects(IWpfTextViewHost h, CoProNetwork cb, CoProExplorer cpe)
        {
            coproExplorer = cpe;
            if (DTE2.ActiveWindow != null)
            {
                iwpf = h;                
                we = ((Events2)DTE2.Events).WindowEvents;
                this.cb = cb;
                //pie.ItemAdded += ItemAdded;
                //((Events2)DTE2.Events).WindowEvents.WindowClosing += new _dispWindowEvents_WindowClosingEventHandler(ClosedWindow);
                //cb = new MyCallBack();
                //cb.ChangeCaret += new ChangeCaretEventHandler(my_CaretChange);
                //twice = false;
                //TODO: register to cb's events
                //TODO: add a different handler function for each of the events
                // examples: http://www.codeproject.com/Articles/20550/C-Event-Implementation-Fundamentals-Best-Practices

                //ts.NewLine();
                //ts.Insert("a");
                //tde = ((Events2)DTE2.Events).TextDocumentKeyPressEvents;
                //tde.BeforeKeyPress += new _dispTextDocumentKeyPressEvents_BeforeKeyPressEventHandler(KeyPress_EventHandler);

                //te.LineChanged += new _dispTextEditorEvents_LineChangedEventHandler(EnterFix);
                //DTE2.Events.CommandEvents.BeforeExecute+= new _dispCommandEvents_BeforeExecuteEventHandler()
                //te.LineChanged += new _dispTextEditorEvents_LineChangedEventHandler(IntelisenseFix);

            }
        }

        /// <summary>
        /// Getter for TextViewHost - helps for current document dte objects
        /// </summary>
        /// <returns></returns>
        IWpfTextViewHost GetTextViewHost()
        {
            return iwpf;
        }

        /// <summary>
        /// Setter for current document text view host for dte object
        /// </summary>
        /// <param name="h"></param>
        void SetTextViewHost(IWpfTextViewHost h)
        {
            iwpf = h;
        }
        
    }
}

