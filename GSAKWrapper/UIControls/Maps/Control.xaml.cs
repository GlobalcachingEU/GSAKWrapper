using GSAKWrapper.Commands;
using GSAKWrapper.MapProviders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GSAKWrapper.UIControls.Maps
{
    /// <summary>
    /// Interaction logic for Control.xaml
    /// </summary>
    public partial class Control : UserControl, INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static MapControlFactory MapControlFactoryToUse { get; private set; }
        private int _maxDownload = -1;
        private MapControlFactory _mapControlFactory;
        private MarkerClusterer _clusterGeocaches = null;
        private List<GeocachePoco> _gcList = null;
        private List<UIElement> _waypointMarkers = new List<UIElement>();
        private List<UIElement> _geocacheMarkers = new List<UIElement>();
        private int _initialZoomLevel = 13;

        private string _selectedMap;
        public string SelectedMap
        {
            get { return _selectedMap; }
            set { SetProperty(ref _selectedMap, value); }
        }

        public MapControlFactory MapFactory { get { return _mapControlFactory; } }

        public Control()
        {
            InitializeComponent();
        }

        public Control(MapControlFactory mcf)
        {
            MapControlFactoryToUse = mcf;
            _mapControlFactory = mcf;

            if (_mapControlFactory != null)
            {
                _mapControlFactory.TileGenerator.DownloadCountChanged += this.OnDownloadCountChanged;
                _mapControlFactory.TileGenerator.DownloadError += this.OnDownloadError;

                DataContext = this;

                this.InitializeComponent();
                CommandManager.AddPreviewExecutedHandler(this, this.PreviewExecuteCommand); // We're going to do some effects when zooming.
                if (_mapControlFactory != null && _mapControlFactory.ID == "GoogleCache")
                {
                    creditsinfo.Visibility = System.Windows.Visibility.Hidden;
                }

                tileCanvas.ZoomChanged += tileCanvas_ZoomChanged;

                Task.Run(() =>
                {
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        this.tileCanvas.Zoom = _initialZoomLevel;
                    }));
                });
            }
        }

        public override string ToString()
        {
            if (_mapControlFactory != null)
            {
                return _mapControlFactory.ToString();
            }
            else
            {
                return "";
            }
        }

        private RelayCommand _removeSelectedMapCommand;
        public RelayCommand RemoveSelectedMapCommand
        {
            get
            {
                if (_removeSelectedMapCommand == null)
                {
                    _removeSelectedMapCommand = new RelayCommand(param => RemoveSelectedMap(), param => !string.IsNullOrEmpty(SelectedMap));
                }
                return _removeSelectedMapCommand;
            }
        }
        public void RemoveSelectedMap()
        {
            if (!string.IsNullOrEmpty(SelectedMap))
            {
                _mapControlFactory.OSMBinFiles.Remove(SelectedMap);
            }
        }

        private RelayCommand _addMapCommand;
        public RelayCommand AddMapCommand
        {
            get
            {
                if (_addMapCommand == null)
                {
                    _addMapCommand = new RelayCommand(param => AddMap());
                }
                return _addMapCommand;
            }
        }
        public void AddMap()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = ""; // Default file name
            dlg.DefaultExt = ".map"; // Default file extension
            dlg.Filter = "OSM Bin files (.map)|*.map"; // Filter files by extension 

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                if (!_mapControlFactory.OSMBinFiles.Contains(dlg.FileName))
                {
                    _mapControlFactory.OSMBinFiles.Add(dlg.FileName);
                }
            }
        }

        void tileCanvas_ZoomChanged(object sender, EventArgs e)
        {
            clusterGeocaches(true);
        }

        public void Dispose()
        {
            if (_mapControlFactory != null)
            {
                if (_mapControlFactory is IDisposable)
                {
                    (_mapControlFactory as IDisposable).Dispose();
                }
                _mapControlFactory = null;
            }
        }

        private void clusterGeocaches(bool recluster)
        {
            List<Marker> mlist = new List<Marker>();
            if (_gcList != null)
            {
                double lat1 = this.tileCanvas.MapControlFactory.TileGenerator.GetLatitude(0, this.tileCanvas.Zoom);
                double lat2 = this.tileCanvas.MapControlFactory.TileGenerator.GetLatitude(1, this.tileCanvas.Zoom);
                double deltaLat = 200 * (lat1 - lat2) / 256.0; //200px
                double lon1 = this.tileCanvas.MapControlFactory.TileGenerator.GetLongitude(0, this.tileCanvas.Zoom);
                double lon2 = this.tileCanvas.MapControlFactory.TileGenerator.GetLongitude(1, this.tileCanvas.Zoom);
                double deltaLon = 200 * (lon2 - lon1) / 256.0; //200px
                if (_clusterGeocaches == null || recluster)
                {
                    _clusterGeocaches = new MarkerClusterer(deltaLat, deltaLon);

                    bool doCluster = this.tileCanvas.Zoom < 12;
                    foreach (var gc in _gcList)
                    {
                        _clusterGeocaches.AddMarker(gc, doCluster);
                    }
                }
                //add markers
                foreach (var b in _clusterGeocaches.Buckets)
                {
                    Marker m = new Marker();
                    m.Latitude = b.Latitude;
                    m.Longitude = b.Longitude;
                    if (b.Count == 1)
                    {
                        m.ImagePath = string.Format("/Resources/Map/{0}.png", (from a in ApplicationData.Instance.GeocacheTypes where a.GSAK == b.Geocache.CacheType select a.ID).FirstOrDefault());
                        m.Tag = b.Geocache;
                    }
                    else
                    {
                        m.ImagePath = "";
                        m.Tag = b.Count;
                    }
                    mlist.Add(m);
                }
            }
            this.SetGeocacheMarkers(mlist);
        }

        public void UpdateView(List<GeocachePoco> gcList, double? cLat, double? cLon, int zoomLevel)
        {
            _initialZoomLevel = zoomLevel;
            _gcList = gcList;
            clusterGeocaches(true);
            this.tileCanvas.CheckMassMarkers();
            this.tileCanvas.RepositionChildren();
            if (cLat != null && cLon != null)
            {
                this.tileCanvas.Center((double)cLat, (double)cLon, zoomLevel);
            }
        }

        protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string name = "")
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                var handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
            }
        }

        public MapCanvas MapCanvas
        {
            get { return this.tileCanvas; }
        }

        private void OnHyperlinkRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri); // Launch the site in the user's default browser.
        }

        private void OnDownloadCountChanged(object sender, EventArgs e)
        {
            if (this.Dispatcher.Thread != Thread.CurrentThread)
            {
                this.Dispatcher.BeginInvoke(new Action(() => this.OnDownloadCountChanged(sender, e)), null);
                return;
            }
            if (_mapControlFactory != null)
            {
                if (_mapControlFactory.TileGenerator.DownloadCount == 0)
                {
                    this.label.Visibility = Visibility.Hidden;
                    this.progress.Visibility = Visibility.Hidden;
                    _maxDownload = -1;
                }
                else
                {
                    this.errorBar.Visibility = Visibility.Collapsed;

                    if (_maxDownload < _mapControlFactory.TileGenerator.DownloadCount)
                    {
                        _maxDownload = _mapControlFactory.TileGenerator.DownloadCount;
                    }
                    this.progress.Value = 100 - (_maxDownload == 0 ? 0 : (_mapControlFactory.TileGenerator.DownloadCount * 100.0 / _maxDownload));
                    this.progress.Visibility = Visibility.Visible;
                    this.label.Text = string.Format(
                        CultureInfo.CurrentUICulture,
                        "Downloading {0} item{1}",
                        _mapControlFactory.TileGenerator.DownloadCount,
                        _mapControlFactory.TileGenerator.DownloadCount != 1 ? 's' : ' ');
                    this.label.Visibility = Visibility.Visible;
                }
            }
        }

        private void OnDownloadError(object sender, EventArgs e)
        {
            if (this.Dispatcher.Thread != Thread.CurrentThread)
            {
                this.Dispatcher.BeginInvoke(new Action(() => this.OnDownloadError(sender, e)), null);
                return;
            }

            this.errorBar.Text = "Unable to contact the server to download map data.";
            this.errorBar.Visibility = Visibility.Visible;
        }

        public void SetGeocacheMarkers(List<Marker> markers)
        {
            _geocacheMarkers.Clear();

            if (markers != null && markers.Count > 0)
            {
                System.Windows.Media.RadialGradientBrush rb1 = new RadialGradientBrush(Color.FromArgb(0xFF, 0x0C, 0x5A, 0xA6), Color.FromArgb(0x66, 0x0C, 0x5A, 0xA6));
                System.Windows.Media.RadialGradientBrush rb2 = new RadialGradientBrush(Color.FromArgb(0xFF, 0xFF, 0xAE, 0x00), Color.FromArgb(0x66, 0xFF, 0xAE, 0x00));
                System.Windows.Media.RadialGradientBrush rb3 = new RadialGradientBrush(Color.FromArgb(0xFF, 0xA6, 0x0C, 0x00), Color.FromArgb(0x66, 0xA6, 0x0C, 0x00));

                System.Windows.Media.Effects.DropShadowEffect effect = new System.Windows.Media.Effects.DropShadowEffect();
                effect.Color = Colors.Black;
                effect.BlurRadius = 3;
                effect.ShadowDepth = 2;

                foreach (var m in markers)
                {
                    Grid rootElement = new Grid();
                    rootElement.Height = 34;
                    rootElement.Width = 34;
                    rootElement.Margin = new Thickness(-17, -34, 0, 0);
                    rootElement.Visibility = System.Windows.Visibility.Hidden;

                    if (m.ImagePath.Length > 0)
                    {
                        Image imgElement;
                        BitmapImage bi3;
                        try
                        {
                            imgElement = new Image();
                            bi3 = new BitmapImage();
                            bi3.BeginInit();
                            bi3.UriSource = Utils.ResourceHelper.GetResourceUri(m.ImagePath);
                            bi3.EndInit();
                        }
                        catch
                        {
                            imgElement = new Image();
                            bi3 = new BitmapImage();
                            bi3.BeginInit();
                            bi3.UriSource = Utils.ResourceHelper.GetResourceUri("/Resources/Map/0.png");
                            bi3.EndInit();
                        }
                        imgElement.Stretch = Stretch.None;
                        imgElement.Source = bi3;
                        rootElement.Children.Add(imgElement);
                        var gc = m.Tag as GeocachePoco;
                        if (gc != null)
                        {
                            imgElement.ToolTip = string.Format("{0}-{1}", gc.Code, gc.Name);
                        }
                    }
                    else
                    {
                        System.Windows.Shapes.Ellipse el = new Ellipse();
                        el.Width = 34;
                        el.Height = 34;
                        int cnt = int.Parse(m.Tag.ToString());
                        if (cnt < 10)
                        {
                            el.Fill = rb1;
                        }
                        else if (cnt < 50)
                        {
                            el.Fill = rb2;
                        }
                        else
                        {
                            el.Fill = rb3;
                        }
                        el.StrokeThickness = 2;

                        TextBlock elT = new TextBlock();
                        elT.Foreground = System.Windows.Media.Brushes.White;
                        elT.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                        elT.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                        elT.Text = m.Tag.ToString();
                        elT.FontSize = 18;

                        /*
                         *                           <TextBlock.Effect>
                            <DropShadowEffect BlurRadius="3" Color="#C222" ShadowDepth="2" />
                          </TextBlock.Effect>

                         */
                        elT.Effect = effect;

                        rootElement.Children.Add(el);
                        rootElement.Children.Add(elT);
                    }

                    rootElement.DataContext = m;
                    rootElement.MouseDown += new MouseButtonEventHandler(rootElement_MouseDown);
                    _geocacheMarkers.Add(rootElement);
                }
            }
            tileCanvas.SetMassMarkers(_geocacheMarkers);
        }

        void rootElement_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Marker m = (sender as Grid).DataContext as Marker;
            if (m.Tag is GeocachePoco)
            {
                if (Utils.GSAK.IsGSAKRunning && !Settings.Settings.Default.ExecutedWithParameters)
                {
                    try
                    {
                        System.Diagnostics.Process.Start(System.IO.Path.Combine(Settings.Settings.Default.GSAKExecutablePath, "gsak.exe"), string.Format("gsak://%FF/Search/{0}", (m.Tag as GeocachePoco).Code));
                    }
                    catch
                    {
                    }
                }
            }
            else
            {
                if (tileCanvas.Zoom < _mapControlFactory.TileGenerator.MaxZoom)
                {
                    tileCanvas.Center(m.Latitude, m.Longitude, tileCanvas.Zoom + 1);
                }
            }
        }

        public void SetWaypointMarkers(List<Marker> markers)
        {
            foreach (var m in _waypointMarkers)
            {
                tileCanvas.Children.Remove(m);
            }
            _waypointMarkers.Clear();

            if (markers != null && markers.Count > 0)
            {
                foreach (var m in markers)
                {
                    Grid rootElement = new Grid();
                    rootElement.Height = 34;
                    rootElement.Width = 34;
                    rootElement.Margin = new Thickness(-17, -34, 0, 0);
                    rootElement.Visibility = System.Windows.Visibility.Visible;

                    Image imgElement = new Image();
                    BitmapImage bi3 = new BitmapImage();
                    bi3.BeginInit();
                    bi3.UriSource = Utils.ResourceHelper.GetResourceUri(m.ImagePath);
                    bi3.EndInit();
                    imgElement.Stretch = Stretch.None;
                    imgElement.Source = bi3;

                    rootElement.Children.Add(imgElement);
                    rootElement.DataContext = m;

                    _waypointMarkers.Add(rootElement);
                    this.tileCanvas.Children.Add(rootElement);
                }
            }
        }

        private void OnSearchControlNavigate(object sender, NavigateEventArgs e)
        {
            if (e.Result == null) // The results have been cleared - hide the marker.
            {
                this.searchMarker.Visibility = Visibility.Hidden;
            }
            else
            {
                this.searchMarker.Visibility = Visibility.Visible;

                this.tileCanvas.Focus();
                if (e.Result.Size.IsEmpty)
                {
                    this.tileCanvas.Center(e.Result.Latitude, e.Result.Longitude, this.tileCanvas.Zoom);
                }
                else
                {
                    this.tileCanvas.Center(e.Result.Latitude, e.Result.Longitude, e.Result.Size);
                }
            }
            this.searchMarker.DataContext = e.Result;
        }

        private void OnZoomStoryboardCompleted(object sender, EventArgs e)
        {
            this.zoomGrid.Visibility = Visibility.Hidden;
            this.zoomImage.Source = null;
        }

        private void PreviewExecuteCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == NavigationCommands.DecreaseZoom)
            {
                if (this.tileCanvas.Zoom > 0) // Make sure we can actualy zoom out
                {
                    this.StartZoom("zoomOut", 1);
                }
            }
            else if (e.Command == NavigationCommands.IncreaseZoom)
            {
                if (this.tileCanvas.Zoom < _mapControlFactory.TileGenerator.MaxZoom)
                {
                    this.StartZoom("zoomIn", 0.5);
                }
            }
        }

        private void StartZoom(string name, double scale)
        {
            this.zoomImage.Source = this.tileCanvas.CreateImage();
            this.zoomRectangle.Height = this.tileCanvas.ActualHeight * scale;
            this.zoomRectangle.Width = this.tileCanvas.ActualWidth * scale;

            this.zoomGrid.RenderTransform = new ScaleTransform(); // Clear the old transform
            this.zoomGrid.Visibility = Visibility.Visible;
            ((Storyboard)this.zoomGrid.FindResource(name)).Begin();
        }

        private void gotoCurpos_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Marker m = curposMarker.DataContext as Marker;
            if (m != null)
            {
                this.tileCanvas.Center(m.Latitude, m.Longitude, this.tileCanvas.Zoom);
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                try
                {
                    string[] dirs = System.IO.Directory.GetDirectories(_mapControlFactory.TileGenerator.CacheFolder);
                    foreach (string d in dirs)
                    {
                        System.IO.Directory.Delete(d, true);
                    }
                }
                catch
                {
                }
            });
        }

        private void Hyperlink_Navigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

    }
}
