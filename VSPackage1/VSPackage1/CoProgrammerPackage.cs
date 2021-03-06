﻿using EnvDTE;
using EnvDTE80;
using EnvDTE100;
using System;
using System.Collections.Generic;
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
using System.Xml;
using System.Xml.Linq;


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
    [ProvideToolWindow(typeof(CoProToolWindow))]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidVSPackage1PkgString)]
    [ProvideAutoLoad("ADFC4E64-0397-11D1-9F4E-00A0C911004F")]

    public sealed class CoProgrammerPackage : Package
    {

        private DTE2 dte;//dte object for solution and graphic manipulations
        private IWpfTextViewHost iwpf;//the current iwpf text view window
        public static CoProServer service = null;// service that is running if there is any
        List<object> events = new List<object>(); //events of dte to be used (must be pointed to be called)
        SolutionEvents se;// solution events object
        public static CoProNetwork cb = null;// Client object if there it is in use
        public static CoProExplorer coproExplorer;// object that controls the explorer of the package
        public static CoProgrammerPackage currRunning;// the current running instance of this class
        static Dictionary<string, MenuCommand> cmds = new Dictionary<string, MenuCommand>();// commands of the menu

        /// <summary>
        /// Members properties
        /// </summary>
        private DTE2 DTE2
        {
            get { return dte ?? (dte = ServiceProvider.GlobalProvider.GetService(typeof(DTE)) as DTE2); }
        }
        private IWpfTextViewHost IWPF
        {
            get { return iwpf ?? (iwpf = ServiceProvider.GlobalProvider.GetService(typeof(IWpfTextViewHost)) as IWpfTextViewHost); }
        }
        public static Dictionary<string, MenuCommand> MenuCommands
        {
            get { return cmds; }
        }

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public CoProgrammerPackage()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }

        /// <summary>
        /// Gets the current ipwf host for dte purposes
        /// </summary>
        /// <returns>Current viewhost for dte object</returns>
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


        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            currRunning = this;
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();
            se = ((Events2)DTE2.Events).SolutionEvents;//Events for opening and shutdown of solutions
            se.AfterClosing += ShutDown;
            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                // Create the command for the menu items.
                CommandID toolwndCommandID = new CommandID(GuidList.guidVSPackage1CmdSet, (int)PkgCmdIDList.cmdidMyTool);
                MenuCommand menuToolWin = new MenuCommand(ShowToolWindow, toolwndCommandID);
                mcs.AddCommand(menuToolWin);
                CommandID menuCommandID = new CommandID(GuidList.guidTopLevelMenuCmdSet, (int)PkgCmdIDList.connectToServer);
                MenuCommand menuItem = new MenuCommand(ConnectCallback, menuCommandID);
                cmds["connectToServer"] = menuItem;
                mcs.AddCommand(menuItem);
                menuCommandID = new CommandID(GuidList.guidTopLevelMenuCmdSet, (int)PkgCmdIDList.hostProject);
                menuItem = new MenuCommand(HostCallback, menuCommandID);
                menuItem.Enabled = true;
                cmds["hostProject"] = menuItem;
                mcs.AddCommand(menuItem);
            }
        }


        private void ShowToolWindow(object sender, EventArgs e)
        {
            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            ToolWindowPane window = this.FindToolWindow(typeof(CoProToolWindow), 0, true);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException(Resources.CanNotCreateWindow);
            }
            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }
        #endregion
        /// <summary>
        /// Helper function to get window content when needed
        /// </summary>
        /// <returns>the window object inside the ToolWindowPane</returns>
        public object GetWindowCotnent()
        {
            ToolWindowPane window = this.FindToolWindow(typeof(CoProToolWindow), 0, true);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException(Resources.CanNotCreateWindow);
            }
            return window.Content;
        }

        /// <summary>
        /// These functions are the callbacks used to execute commands when the a menu items are clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        /// 

        /// <summary>
        /// Callback function for connection option on the menu to open the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectCallback(object sender, EventArgs e)
        {
            cb = new CoProNetwork();
            CoProWindow c = new CoProWindow(cb);
            c.Show();
            //Carets cs = new Carets(GetCurrentViewHost());


        }

        /// <summary>
        /// Callback function for hosting project option on the menu to open the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HostCallback(object sender, EventArgs e)
        {
            HostProjectWindow hpw = new HostProjectWindow(DTE2);
            hpw.ShowDialog();
            //service = new Service();
        }

        /// <summary>
        /// reseting the package objects
        /// </summary>
        private void ShutDown()
        {
            if (cb != null)
            {
                //FileStream fs = File.Create(cb.ProjPath + "\\CoProFiles\\timestamps.txt");
                //fs.Close();
                //StreamWriter sw = new StreamWriter(cb.ProjPath + "\\CoProFiles\\timestamps.txt");
                //sw.Write(TimeStampDirectory(cb.ProjPath, 1, cb.ProjPath.Substring(cb.ProjPath.LastIndexOf('\\'))));
                //sw.Close();
                if (cb.IsAdmin)
                {
                    XElement xe = CreateFileSystemXmlTree(cb.ProjPath, 1, cb.ProjPath.Substring(cb.ProjPath.LastIndexOf('\\') + 1));
                    XmlTextWriter xwr = new XmlTextWriter(cb.ProjPath + "\\CoProFiles\\timestamps.xml", System.Text.Encoding.UTF8);
                    xwr.Formatting = Formatting.Indented;
                    xe.WriteTo(xwr);
                    xwr.Close();
                }
                cb.Abort();
                cb = null;
                CoProFilterProvider.isFirst = true;
            }
            if (CoProgrammerPackage.service != null)
            {
                CoProgrammerPackage.service.Close();
                CoProgrammerPackage.service = null;
            }
        }

        /// <summary>
        /// Helper function to create xml file for the fiel tree in a directory
        /// </summary>
        /// <param name="source">path of the original directory</param>
        /// <param name="level">level of the directory relatively to the first </param>
        /// <param name="relPath">relative path of the current directory relatively to the first</param>
        /// <returns></returns>
        public static XElement CreateFileSystemXmlTree(string source, int level, string relPath)
        {
            DirectoryInfo dir = new DirectoryInfo(source);
            var info = new XElement("Directory",
                   new XAttribute("Name", dir.Name), new XAttribute("Level", level));
            foreach (var file in dir.GetFiles())
            {
                if (!file.FullName.ToLower().EndsWith(".suo"))
                {
                    info.Add(new XElement("File",
                                 new XAttribute("Name", file.Name), new XAttribute("TimeChanged", file.LastWriteTimeUtc), new XAttribute("RelativePath", relPath)));
                }
            }
            foreach (var subDir in dir.GetDirectories())
            {
                if (!(subDir.FullName.Contains("bin") || subDir.FullName.Contains("obj") || subDir.FullName.Contains("CoProFiles")))
                    info.Add(CreateFileSystemXmlTree(subDir.FullName, level + 1, relPath + '\\' + subDir.FullName.Substring(subDir.FullName.LastIndexOf('\\') + 1)));
            }

            return info;
        }


    }
}
