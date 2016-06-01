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
        CoProNetwork cb;
        string m_name;
        public CloneWindow(CoProNetwork calb, string name)
        {
            InitializeComponent();
            cb = calb;
            m_name = name;
        }
        /// <summary>
        /// Choosing directory button handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChooseDirectory_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            dirBlock.Text = dialog.SelectedPath;
            if (dialog.SelectedPath != "")
            {

                while (Directory.EnumerateFiles(dialog.SelectedPath).Any() && Directory.EnumerateDirectories(dialog.SelectedPath).Any())
                {
                    System.Windows.MessageBox.Show("Please choose an empty folder for cloning the project");
                    dialog.ShowDialog();
                    dirBlock.Text = dialog.SelectedPath;
                }
                cb.ProjPath = dialog.SelectedPath;
            }
            
        }
        /// <summary>
        /// "When done chosing directory" handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Done_Click(object sender, RoutedEventArgs e)
        {
            if(cb.GetProject(m_name))
            {
                System.Windows.Forms.MessageBox.Show("Success you can now open your project!");
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Cloning failed :/ try again");
            }
            this.Close();
        }
    }
}
