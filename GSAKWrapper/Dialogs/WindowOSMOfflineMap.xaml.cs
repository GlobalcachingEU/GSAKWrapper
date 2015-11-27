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
    /// Interaction logic for WindowOSMOfflineMap.xaml
    /// </summary>
    public partial class WindowOSMOfflineMap : Window
    {
        private UIControls.Maps.Control _control = null;
        private List<GSAKWrapper.MapProviders.GeocachePoco> _gcList = null;
        private double? _cLat = null;
        private double? _cLon = null;
        private int _initialZoomLevel = 13;

        public WindowOSMOfflineMap()
            : this(null, null, null, 13)
        {
        }

        public WindowOSMOfflineMap(List<GSAKWrapper.MapProviders.GeocachePoco> gcList, double? cLat, double? cLon, int zoomLevel)
        {
            _initialZoomLevel = zoomLevel;
            _gcList = gcList;
            _cLat = cLat;
            _cLon = cLon;
            InitializeComponent();
            _control = new UIControls.Maps.Control(new MapProviders.MapControlFactoryOSMOffline());
            mapContainer.Children.Add(_control);
            _control.Loaded += _control_Loaded;

            ApplicationData.Instance.OpenWindows.Add(this);
            this.Closed += WindowOSMOfflineMap_Closed;
        }

        void _control_Loaded(object sender, RoutedEventArgs e)
        {
            _control.UpdateView(_gcList, _cLat, _cLon, Math.Max(_initialZoomLevel, _control.tileCanvas.MapControlFactory.TileGenerator.MaxZoom));
        }

        void WindowOSMOfflineMap_Closed(object sender, EventArgs e)
        {
            ApplicationData.Instance.OpenWindows.Remove(this);
            if (_control != null)
            {
                _control.Dispose();
                _control = null;
            }
        }
    }
}
