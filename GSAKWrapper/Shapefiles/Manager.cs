using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GSAKWrapper.Shapefiles
{
    public class Manager: INotifyPropertyChanged
    {
        private static Manager _uniqueInstance = null;
        private static object _lockObject = new object();

        private List<ShapeFile> _shapeFiles = new List<ShapeFile>();

        public Manager()
        {
#if DEBUG
            if (_uniqueInstance != null)
            {
                //you used the wrong binding
                System.Diagnostics.Debugger.Break();
            }
#endif
            Initialize();
        }

        public static Manager Instance
        {
            get
            {
                if (_uniqueInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_uniqueInstance == null)
                        {
                            _uniqueInstance = new Manager();
                        }
                    }
                }
                return _uniqueInstance;
            }
        }

        public bool Initialize()
        {
            Clear();

            var items = Settings.Settings.Default.GetShapeFileItems();
            foreach (var item in items)
            {
                try
                {
                    if (item.Enabled != 0)
                    {
                        if (System.IO.File.Exists(item.FileName))
                        {
                            ShapeFile sf = new ShapeFile(item.FileName);
                            if (sf.Initialize(item.TableName,
                                (ShapeFile.CoordType)Enum.Parse(typeof(ShapeFile.CoordType), item.CoordType),
                                (AreaType)Enum.Parse(typeof(AreaType), item.AreaType),
                                item.NamePrefix??"",
                                item.Encoding))
                            {
                                _shapeFiles.Add(sf);
                            }
                        }
                    }
                }
                catch
                {
                }
            }

            if (PropertyChanged!=null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(""));
            }

            return true;
        }

        private void Clear()
        {
            foreach (var sf in _shapeFiles)
            {
                sf.Dispose();
            }
            _shapeFiles.Clear();
        }

        public List<string> AvailablePrefixes
        {
            get
            {
                return (from a in _shapeFiles select a.NamePrefix).Distinct().ToList();
            }
        }

        public string GetAreaNameOfLocation(double lat, double lon, AreaType areaType, string prefix)
        {
            string result = null;
            foreach (var sf in _shapeFiles)
            {
                result = sf.GetAreaNameOfLocation(lat, lon, areaType, prefix);
                if (!string.IsNullOrEmpty(result))
                {
                    break;
                }
            }
            return result;
        }

        public List<AreaInfo> GetAreasOfLocation(Utils.Location loc)
        {
            List<AreaInfo> result = new List<AreaInfo>();
            foreach (var sf in _shapeFiles)
            {
                List<AreaInfo> areas = sf.GetAreasOfLocation(loc);
                if (areas != null)
                {
                    result.AddRange(areas);
                }
            }
            return result;
        }

        public List<AreaInfo> GetAreasOfLocation(Utils.Location loc, List<AreaInfo> inAreas)
        {
            List<AreaInfo> result = new List<AreaInfo>();
            foreach (var sf in _shapeFiles)
            {
                List<AreaInfo> areas = sf.GetAreasOfLocation(loc, inAreas);
                if (areas != null)
                {
                    result.AddRange(areas);
                }
            }
            return result;
        }

        public List<AreaInfo> GetEnvelopAreasOfLocation(Utils.Location loc)
        {
            List<AreaInfo> result = new List<AreaInfo>();
            foreach (var sf in _shapeFiles)
            {
                List<AreaInfo> areas = sf.GetEnvelopAreasOfLocation(loc);
                if (areas != null)
                {
                    result.AddRange(areas);
                }
            }
            return result;
        }

        public List<AreaInfo> GetEnvelopAreasOfLocation(Utils.Location loc, List<AreaInfo> inAreas)
        {
            List<AreaInfo> result = new List<AreaInfo>();
            foreach (var sf in _shapeFiles)
            {
                List<AreaInfo> areas = sf.GetEnvelopAreasOfLocation(loc, inAreas);
                if (areas != null)
                {
                    result.AddRange(areas);
                }
            }
            return result;
        }

        public List<AreaInfo> GetAreasByName(string name)
        {
            List<AreaInfo> result = new List<AreaInfo>();
            foreach (var sf in _shapeFiles)
            {
                result.AddRange((from a in sf.AreaInfos where string.Compare(a.Name, name, true) == 0 select a).ToList());
            }
            return result;
        }

        public List<AreaInfo> GetAreasByName(string name, AreaType level)
        {
            List<AreaInfo> result = new List<AreaInfo>();
            foreach (var sf in _shapeFiles)
            {
                result.AddRange((from a in sf.AreaInfos where string.Compare(a.Name, name, true) == 0 && a.Level == level select a).ToList());
            }
            return result;
        }

        public List<AreaInfo> GetAreasByID(object id)
        {
            List<AreaInfo> result = new List<AreaInfo>();
            foreach (var sf in _shapeFiles)
            {
                result.AddRange((from a in sf.AreaInfos where a.ID == id select a).ToList());
            }
            return result;
        }

        public List<AreaInfo> GetAreasByLevel(AreaType level)
        {
            List<AreaInfo> result = new List<AreaInfo>();
            foreach (var sf in _shapeFiles)
            {
                result.AddRange((from a in sf.AreaInfos where a.Level == level select a).ToList());
            }
            return result;
        }

        public void GetPolygonOfArea(AreaInfo area)
        {
            foreach (var sf in _shapeFiles)
            {
                sf.GetPolygonOfArea(area);
                if (area.Polygons != null)
                {
                    break;
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
