using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Company.VSPackage1
{
    /// <summary>
    /// Interaction logic for MyControl.xaml
    /// </summary>
    public partial class CoProExplorer : UserControl
    {
        private CoProNetwork cb = null;
        public CoProExplorer()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Sets the client network object
        /// </summary>
        /// <param name="cb"></param>
        /// 
        public void SetConnection(CoProNetwork cb)
        {
            this.cb=cb;
        }
        /// <summary>
        /// Updates info in the explorer
        /// </summary>
        public void UpdateInfo()
        {
            infoText.Text = cb.IPort[0] + " " + cb.IPort[1];
        }
    }
}