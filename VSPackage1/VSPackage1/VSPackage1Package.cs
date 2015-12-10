using EnvDTE;
using EnvDTE80;
using EnvDTE100;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using System.Threading;
using System.IO;
using Microsoft.Win32;
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


namespace Company.VSPackage1
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidVSPackage1PkgString)]



    public sealed class VSPackage1Package : Package
    {
        
        private DTE2 dte;
        private IWpfTextViewHost iwpf;
        TextDocumentKeyPressEvents tde;
        TextEditorEvents te;
        char[] text = new char[50];
        int place = 0;
        private DTE2 DTE2
        {
            get { return dte ?? (dte = ServiceProvider.GlobalProvider.GetService(typeof(DTE)) as DTE2); }
        }
        private IWpfTextViewHost IWPF
        {
            get { return iwpf ?? (iwpf = ServiceProvider.GlobalProvider.GetService(typeof(IWpfTextViewHost)) as IWpfTextViewHost); }
        }


        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public VSPackage1Package()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));

        }



        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                // Create the command for the menu item.

                CommandID menuCommandID = new CommandID(GuidList.guidTopLevelMenuCmdSet, (int)PkgCmdIDList.cmdidMyCommand);
                MenuCommand menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
                mcs.AddCommand(menuItem);
            }
        }
        #endregion

        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            //CoProWindow c = new CoProWindow(DTE);
            // c.Show();      
            if (DTE2.ActiveWindow != null)
            {

                
                tde = ((Events2)DTE2.Events).TextDocumentKeyPressEvents;
                tde.BeforeKeyPress += new _dispTextDocumentKeyPressEvents_BeforeKeyPressEventHandler(CallBack);
                te = ((Events2)DTE2.Events).TextEditorEvents;
                te.LineChanged += new _dispTextEditorEvents_LineChangedEventHandler(CallBack2);
                IWpfTextViewHost h = GetCurrentViewHost();
                ITextCaret c = h.TextView.Caret;
                MyCallBack obj = new MyCallBack();
                obj.callService(c.Position.ToString());

                //ts.NewLine();
                //ts.Insert("a");
            }

        }
        bool twice = false;
        private void CallBack(string st, TextSelection ts, bool b, ref bool br)
        {
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
        }
        private void CallBack2(TextPoint a, TextPoint b, int i)
        {
            //MessageBox.Show( Clipboard.GetText());   
            EnvDTE.TextSelection ts = dte.ActiveDocument.Selection as EnvDTE.TextSelection;
            //   EnvDTE.VirtualPoint vp = ts.ActivePoint;
            // System.Windows.Forms.MessageBox.Show(st);
            //DTE2.ActiveDocument.Save();
            MessageBox.Show("a:" + a.Line + "," + a.LineCharOffset + "," + a.DTE.ActiveDocument.Name + "\n b:" + b.Line + "," + b.LineCharOffset + b.DTE.ActiveDocument.Name + "\n" + i);
            int line = ts.ActivePoint.Line;
            int charoff = ts.ActivePoint.LineCharOffset;
            TextSelection ts2 = null;
            ts2 = DTE2.ActiveWindow.Project.ProjectItems.Item("Class2.cs").Document.Selection as TextSelection;
            ts2.MoveToLineAndOffset(line, charoff, false);
            DTE2.ActiveDocument.Save();
            
        }
        IWpfTextViewHost GetCurrentViewHost()
        {
            // code to get access to the editor's currently selected text cribbed from
            // http://msdn.microsoft.com/en-us/library/dd884850.aspx
            IVsTextManager txtMgr = (IVsTextManager)GetService(typeof(SVsTextManager));
            IVsTextView vTextView = null;
            int mustHaveFocus = 1;
            txtMgr.GetActiveView(mustHaveFocus, null, out vTextView);
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


       
    }
}
