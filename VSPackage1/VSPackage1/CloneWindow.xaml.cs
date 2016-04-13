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
using System.IO;

namespace Company.VSPackage1
{
    /// <summary>
    /// Interaction logic for CloneWindow.xaml
    /// </summary>
    public partial class CloneWindow : Window
    {
        MyCallBack cb;

        public CloneWindow(MyCallBack calb)
        {
            InitializeComponent();
            cb = calb;
        }

        private void ChooseDirectory_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            while (Directory.EnumerateFiles(dialog.SelectedPath).Any())
            {
                dirBlock.Text = dialog.SelectedPath;
                System.Windows.MessageBox.Show("Please choose an empty folder for cloning the project");
                dialog.ShowDialog();
            }

            cb.SetProjPath(dialog.SelectedPath);
            cb.GetProject(@"C:\Users\Arbel\Documents\Visual Studio 2013\Projects\ConsoleApplication1", "ConsoleApplication1");
        }

        private void Done_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
