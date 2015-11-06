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

namespace GSAKWrapper.Dialogs
{
    /// <summary>
    /// Interaction logic for WindowExportGPXSettings.xaml
    /// </summary>
    public partial class WindowExportGGZSettings : Window
    {
        public UIControls.ActionBuilder.ActionExportGGZ.ExportGPXSettings GPXSettings { get; set; }

        public WindowExportGGZSettings()
        {
            InitializeComponent();
        }

        public WindowExportGGZSettings(UIControls.ActionBuilder.ActionExportGGZ.ExportGPXSettings gpxSettings)
        {
            GPXSettings = gpxSettings;
            InitializeComponent();
            GPXVersion.Items.Add(Utils.GPXGenerator.V100);
            GPXVersion.Items.Add(Utils.GPXGenerator.V101);
            DataContext = GPXSettings;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (GPXSettings != null)
            {
                GPXSettings.FileName = FileName.Text;
                GPXSettings.Version = GPXVersion.SelectedItem as Version;
            }
            Close();
        }

        private void ButtonSelect_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = ""; // Default file name
            dlg.DefaultExt = ".ggz"; // Default file extension
            dlg.Filter = "GGZ (.ggz)|*.ggz"; // Filter files by extension 

            var result = dlg.ShowDialog();

            if (result == true)
            {
                FileName.Text = dlg.FileName;
            }
        }
    }
}
