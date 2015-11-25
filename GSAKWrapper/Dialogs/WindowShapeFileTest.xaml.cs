using GSAKWrapper.Shapefiles;
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
    /// Interaction logic for WindowShapeFileTest.xaml
    /// </summary>
    public partial class WindowShapeFileTest : Window
    {
        private UIControls.WebBrowserControl _browser = null;
        private ShapeFile _sf;

        public List<string> Areas { get; set; }

        private string _selectedArea;
        public string SelectedArea
        {
            get { return _selectedArea; }
            set
            {
                _selectedArea = value;
                if (!string.IsNullOrEmpty(_selectedArea))
                {
                    try
                    {
                        var area = (from a in _sf.AreaInfos where a.Name == _selectedArea select a).FirstOrDefault();
                        if (area != null)
                        {
                            _sf.GetPolygonOfArea(area);
                            if (area.Polygons != null && area.Polygons.Count > 0)
                            {
                                string html = Utils.ResourceHelper.GetEmbeddedTextFile("/Resources/WindowShapeFileTest.html");
                                StringBuilder sb = new StringBuilder();
                                sb.Append("addPolygons ([");
                                bool firstPoly = true;
                                bool firstPoint;
                                foreach (var ps in area.Polygons)
                                {
                                    if (!firstPoly)
                                    {
                                        sb.Append(",");
                                    }
                                    else
                                    {
                                        firstPoly = false;
                                    }
                                    sb.Append("{points:[");
                                    firstPoint = true;
                                    foreach (Utils.Location l in ps)
                                    {
                                        if (!firstPoint)
                                        {
                                            sb.Append(",");
                                        }
                                        else
                                        {
                                            firstPoint = false;
                                        }
                                        sb.AppendFormat("{{lat: {0}, lon: {1}}}", l.Lat.ToString().Replace(',', '.'), l.Lon.ToString().Replace(',', '.'));
                                    }
                                    sb.Append("]}");
                                }
                                sb.Append("]);");
                                html = html.Replace("//addpolygon", sb.ToString());
                                _browser.DocumentText = html;
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        public WindowShapeFileTest()
        {
            InitializeComponent();
        }

        public WindowShapeFileTest(ShapeFileInfo shapeFileInfo)
        {
            Areas = new List<string>();
            try
            {
                _sf = new ShapeFile(shapeFileInfo.Filename);
                if (_sf.Initialize(shapeFileInfo.TableName,
                    (ShapeFile.CoordType)Enum.Parse(typeof(ShapeFile.CoordType), shapeFileInfo.TCoord),
                    (AreaType)Enum.Parse(typeof(AreaType), shapeFileInfo.TArea),
                    shapeFileInfo.Prefix ?? "",
                    shapeFileInfo.Encoding))
                {
                    var ail = _sf.AreaInfos;
                    Areas = (from a in ail orderby a.Name select a.Name).ToList();
                }
            }
            catch
            {
            }
            InitializeComponent();
            _browser = new UIControls.WebBrowserControl();
            this.webBrowser.Children.Add(_browser);
            DataContext = this;

            this.Closed += WindowShapeFileTest_Closed;
        }

        void WindowShapeFileTest_Closed(object sender, EventArgs e)
        {
            if (_sf != null)
            {
                _sf.Dispose();
                _sf = null;
            }
        }
    }
}
