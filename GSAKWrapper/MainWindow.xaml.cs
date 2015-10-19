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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GSAKWrapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal delegate void ProcessArgDelegate(String arg);
        internal static ProcessArgDelegate ProcessArg;
        
        public MainWindow()
        {
            ProcessArg = delegate(String arg)
            {
                //process arguments
            };

            this.Initialized += delegate(object sender, EventArgs e)
            {
                //process arguments
                //ArgsRun.Text = (String)Application.Current.Resources[WpfSingleInstance.StartArgKey];
                try
                {
                    Application.Current.Resources.Remove(WpfSingleInstance.StartArgKey);
                }
                catch
                {

                }
            };

            InitializeComponent();
        }

    }
}
