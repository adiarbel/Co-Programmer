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
            var addr = textBox1.Text;
            cb.SetIpPort(addr.ToString().Split(':')[0], addr.ToString().Split(':')[1]);
            if(!cb.Connect())
            {
                System.Windows.MessageBox.Show("Try again connection failed...");
            }
            else
            {
                System.Windows.MessageBox.Show("You are now connected!");
                this.Close();
            }
        }
    }
}
