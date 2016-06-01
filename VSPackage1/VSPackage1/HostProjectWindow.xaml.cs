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
    /// Interaction logic for HostProjectWindow.xaml
    /// </summary>
    public partial class HostProjectWindow : Window
    {
        EnvDTE80.DTE2 dte;
        public HostProjectWindow(EnvDTE80.DTE2 dte)
        {
            InitializeComponent();
            this.dte = dte;
        }
        private void ChooseDirectory_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.ShowDialog();
            dirBlock.Text = dialog.SelectedPath;
            if (dialog.SelectedPath != "")
            {
                while (!(Directory.EnumerateFiles(dialog.SelectedPath).Any() && Directory.EnumerateDirectories(dialog.SelectedPath).Any()) && dialog.SelectedPath != "")
                {
                    System.Windows.MessageBox.Show("Please do not choose an empty folder for sharing");
                    dialog.ShowDialog();
                    dirBlock.Text = dialog.SelectedPath;
                }
            }
        }
        private void Done_Click(object sender, RoutedEventArgs e)
        {
            if (name.Text != "")
            {

                FileStream fs = File.Create(dirBlock.Text.Substring(0, dirBlock.Text.LastIndexOf('\\')) + "\\admin.txt");
                fs.Write(Encoding.ASCII.GetBytes(dirBlock.Text + "\n" + name.Text), 0, (dirBlock.Text + "\n" + name.Text).Length);
                //File.GetLastWriteTime();
                fs.Close();
                this.Close();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Pleases enter a name and try again");
            }
        }
    }
}
