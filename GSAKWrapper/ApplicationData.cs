﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GSAKWrapper
{
    public class ApplicationData: INotifyPropertyChanged
    {
        private static ApplicationData _uniqueInstance = null;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow MainWindow { get; set; }
        public List<DataTypes.GeocacheAttribute> GeocacheAttributes;
        public List<DataTypes.GeocacheType> GeocacheTypes;
        public List<DataTypes.GeocacheContainer> GeocacheContainers;
        public List<DataTypes.WaypointType> WaypointTypes;

        private int _activityCounter = 0;
        public void BeginActiviy()
        {
            _activityCounter++;
            if (_activityCounter == 1)
            {
                UIIsIdle = false;
            }
        }
        public void EndActiviy()
        {
            _activityCounter--;
            if (_activityCounter == 0)
            {
                UIIsIdle = true;
            }
        }

        private bool _uiIsIdle = true;
        public bool UIIsIdle
        {
            get { return _uiIsIdle; }
            set
            {
                if (_uiIsIdle != value)
                {
                    SetProperty(ref _uiIsIdle, value);
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        private string _statusText = "Ready";
        public string StatusText
        {
            get { return _statusText; }
            set
            {
                if (_statusText != value)
                {
                    SetProperty(ref _statusText, value);
                }
            }
        }

        public ApplicationData()
        {
#if DEBUG
            if (_uniqueInstance != null)
            {
                //you used the wrong binding
                //use: 
                //<properties:ApplicationData x:Key="ApplicationData" />
                //{Binding Databases, Source={x:Static p:ApplicationData.Instance}}
                //{Binding Source={x:Static p:ApplicationData.Instance}, Path=ActiveDatabase.GeocacheCollection}
                System.Diagnostics.Debugger.Break();
            }
#endif
            GeocacheAttributes = new List<DataTypes.GeocacheAttribute>();
            GeocacheTypes = new List<DataTypes.GeocacheType>();
            GeocacheContainers = new List<DataTypes.GeocacheContainer>();
            WaypointTypes = new List<DataTypes.WaypointType>();

            //addCacheType(0, "Not present");
            addCacheType(2, "Traditional Cache", 'T');
            addCacheType(3, "Multi-cache", 'M');
            addCacheType(4, "Virtual Cache", 'V');
            addCacheType(5, "Letterbox Hybrid", 'B');
            addCacheType(6, "Event Cache", 'E');
            addCacheType(8, "Unknown (Mystery) Cache", 'U', "Unknown Cache");
            addCacheType(9, "Project APE Cache", 'A');
            addCacheType(11, "Webcam Cache", 'W');
            addCacheType(12, "Locationless (Reverse) Cache", 'L');
            addCacheType(13, "Cache In Trash Out Event", 'C');
            addCacheType(137, "Earthcache", 'R');
            addCacheType(453, "Mega-Event Cache", 'Z');
            //addCacheType(605, "Geocache Course", ' ');
            addCacheType(1304, "GPS Adventures Exhibit", 'X');
            addCacheType(1858, "Wherigo Cache", 'I');
            addCacheType(3653, "Lost and Found Event Cache", 'F');
            addCacheType(3773, "Groundspeak HQ", 'H');
            addCacheType(3774, "Groundspeak Lost and Found Celebration", 'D');
            //addCacheType(4738, "Groundspeak Block Party", ' ');
            addCacheType(7005, "Giga-Event Cache", 'G');

            //addCacheContainer(0, "Unknown");
            addCacheContainer(1, "Not chosen");
            addCacheContainer(2, "Micro");
            addCacheContainer(3, "Regular");
            addCacheContainer(4, "Large");
            addCacheContainer(5, "Virtual");
            addCacheContainer(6, "Other");
            addCacheContainer(8, "Small");

            //addWaypointType(0, "Unknown");
            addWaypointType(217, "Parking Area");
            addWaypointType(220, "Final Location");
            //addWaypointType(218, "Question to Answer");
            addWaypointType(218, "Virtual Stage");
            addWaypointType(452, "Reference Point");
            //addWaypointType(219, "Stages of a Multicache");
            addWaypointType(219, "Physical Stage");
            addWaypointType(221, "Trailhead");

            //addCacheAttribute(0, "Unknown");
            addCacheAttribute(1, "Dogs");
            addCacheAttribute(2, "Access or parking fee");
            addCacheAttribute(3, "Climbing gear");
            addCacheAttribute(4, "Boat");
            addCacheAttribute(5, "Scuba gear");
            addCacheAttribute(6, "Recommended for kids");
            addCacheAttribute(7, "Takes less than an hour");
            addCacheAttribute(8, "Scenic view");
            addCacheAttribute(9, "Significant Hike");
            addCacheAttribute(10, "Difficult climbing");
            addCacheAttribute(11, "May require wading");
            addCacheAttribute(12, "May require swimming");
            addCacheAttribute(13, "Available at all times");
            addCacheAttribute(14, "Recommended at night");
            addCacheAttribute(15, "Available during winter");
            addCacheAttribute(16, "Cactus");
            addCacheAttribute(17, "Poison plants");
            addCacheAttribute(18, "Dangerous Animals");
            addCacheAttribute(19, "Ticks");
            addCacheAttribute(20, "Abandoned mines");
            addCacheAttribute(21, "Cliff / falling rocks");
            addCacheAttribute(22, "Hunting");
            addCacheAttribute(23, "Dangerous area");
            addCacheAttribute(24, "Wheelchair accessible");
            addCacheAttribute(25, "Parking available");
            addCacheAttribute(26, "Public transportation");
            addCacheAttribute(27, "Drinking water nearby");
            addCacheAttribute(28, "Public restrooms nearby");
            addCacheAttribute(29, "Telephone nearby");
            addCacheAttribute(30, "Picnic tables nearby");
            addCacheAttribute(31, "Camping available");
            addCacheAttribute(32, "Bicycles");
            addCacheAttribute(33, "Motorcycles");
            addCacheAttribute(34, "Quads");
            addCacheAttribute(35, "Off-road vehicles");
            addCacheAttribute(36, "Snowmobiles");
            addCacheAttribute(37, "Horses");
            addCacheAttribute(38, "Campfires");
            addCacheAttribute(39, "Thorns");
            addCacheAttribute(40, "Stealth required");
            addCacheAttribute(41, "Stroller accessible");
            addCacheAttribute(42, "Needs maintenance");
            addCacheAttribute(43, "Watch for livestock");
            addCacheAttribute(44, "Flashlight required");
            addCacheAttribute(45, "Lost And Found Tour");
            addCacheAttribute(46, "Truck Driver/RV");
            addCacheAttribute(47, "Field Puzzle");
            addCacheAttribute(48, "UV Light Required");
            addCacheAttribute(49, "Snowshoes");
            addCacheAttribute(50, "Cross Country Skis");
            addCacheAttribute(51, "Special Tool Required");
            addCacheAttribute(52, "Night Cache");
            addCacheAttribute(53, "Park and Grab");
            addCacheAttribute(54, "Abandoned Structure");
            addCacheAttribute(55, "Short hike (less than 1km)");
            addCacheAttribute(56, "Medium hike (1km-10km)");
            addCacheAttribute(57, "Long Hike (+10km)");
            addCacheAttribute(58, "Fuel Nearby");
            addCacheAttribute(59, "Food Nearby");
            addCacheAttribute(60, "Wireless Beacon");
            addCacheAttribute(61, "Partnership Cache");
            addCacheAttribute(62, "Seasonal Access");
            addCacheAttribute(63, "Tourist Friendly");
            addCacheAttribute(64, "Tree Climbing");
            addCacheAttribute(65, "Front Yard (Private Residence)");
            addCacheAttribute(66, "Teamwork Required");
            
            List<DataTypes.GeocacheAttribute> attrs = (from a in this.GeocacheAttributes select a).ToList();
            foreach (var a in attrs)
            {
                if (a.ID > 0)
                {
                    addCacheAttribute(-a.ID, a.Name);
                }
            }
        }

        public static ApplicationData Instance
        {
            get
            {
                if (_uniqueInstance == null)
                {
                    _uniqueInstance = new ApplicationData();
                }
                return _uniqueInstance;
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

        protected void addCacheAttribute(int id, string name)
        {
            var attr = new DataTypes.GeocacheAttribute();
            attr.ID = id;
            attr.Name = name;
            GeocacheAttributes.Add(attr);
        }

        private void addCacheType(int id, string name, char gsakId)
        {
            var ct = new DataTypes.GeocacheType();
            ct.ID = id;
            ct.Name = name;
            ct.GSAK = gsakId.ToString();
            GeocacheTypes.Add(ct);
        }
        private void addCacheType(int id, string name, char gsakId, string gpxTag)
        {
            var ct = new DataTypes.GeocacheType(gpxTag);
            ct.ID = id;
            ct.Name = name;
            ct.GSAK = gsakId.ToString();
            GeocacheTypes.Add(ct);
        }

        private void addCacheContainer(int id, string name)
        {
            var attr = new DataTypes.GeocacheContainer();
            attr.ID = id;
            attr.Name = name;
            GeocacheContainers.Add(attr);
        }

        protected void addWaypointType(int id, string name)
        {
            var attr = new DataTypes.WaypointType();
            attr.ID = id;
            attr.Name = name;
            WaypointTypes.Add(attr);
        }
    }
}
