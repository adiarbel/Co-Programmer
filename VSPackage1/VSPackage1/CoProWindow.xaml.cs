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
        private EnvDTE.DTE DTE;
        public CoProWindow(EnvDTE.DTE dte)
        {
            this.DTE = dte;
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {

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
