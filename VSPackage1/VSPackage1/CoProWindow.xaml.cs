using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Company.VSPackage1
{
    /// <summary>
    /// Interaction logic for CoProWindow.xaml
    /// </summary>
    public partial class CoProWindow : Window
    {
        MyCallBack cb;
        public CoProWindow(MyCallBack calb)
        {
            InitializeComponent();
            cb = calb;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            var addr = textBox1.Text;
            cb.SetIpPort(addr.ToString().Split(',')[0], addr.ToString().Split(',')[1]);
            cb.Connect();
            cb.SetProjPath(dialog.SelectedPath);
            cb.GetProject(@"C:\Users\Arbel\Documents\Visual Studio 2013\Projects\ConsoleApplication1", "ConsoleApplication1");
            //if (DTE.ActiveDocument != null)
            //{
            //    EnvDTE.TextSelection ts = DTE.ActiveDocument.Selection as EnvDTE.TextSelection;
            //    EnvDTE.VirtualPoint vp = ts.ActivePoint;
            //    ts.GotoLine(15, true);
            //    ts.Insert("         Console.WriteLine(\""+textBox1.Text.ToString()+"\");");
            //}
        }
    }
}
