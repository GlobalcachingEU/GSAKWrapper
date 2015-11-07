using System;
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
        public List<DataTypes.LogType> LogTypes;

        public List<DataTypes.GSAKCustomGlobal> GSAKCustomGlobals { get; set; }

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
            LogTypes = new List<DataTypes.LogType>();
            GSAKCustomGlobals = new List<DataTypes.GSAKCustomGlobal>();

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

            //addLogType(0, "Unknown", false);
            addLogType(1, "Unarchive", false);
            addLogType(2, "Found it", true);
            addLogType(3, "Didn't find it", false);
            addLogType(4, "Write note", false);
            addLogType(5, "Archive", false);
            addLogType(6, "Archive", false);
            addLogType(7, "Needs Archived", false);
            addLogType(8, "Mark Destroyed", false);
            addLogType(9, "Will Attend", false);
            addLogType(10, "Attended", true);
            addLogType(11, "Webcam Photo Taken", true);
            addLogType(12, "Unarchive", false);
            addLogType(13, "Retrieve It from a Cache", false);
            addLogType(14, "Dropped Off", false);
            addLogType(15, "Transfer", false);
            addLogType(16, "Mark Missing", false);
            addLogType(17, "Recovered", false);
            addLogType(18, "Post Reviewer Note", false);
            addLogType(19, "Grab It (Not from a Cache)", false);
            addLogType(20, "Write Jeep 4x4 Contest Essay", false);
            addLogType(21, "Upload Jeep 4x4 Contest Photo", false);
            addLogType(22, "Temporarily Disable Listing", false);
            addLogType(23, "Enable Listing", false);
            addLogType(24, "Publish Listing", false);
            addLogType(25, "Retract Listing", false);
            addLogType(30, "Uploaded Goal Photo for \"A True Original\"", false);
            addLogType(31, "Uploaded Goal Photo for \"Yellow Jeep Wrangler\"", false);
            addLogType(32, "Uploaded Goal Photo for \"Construction Site\"", false);
            addLogType(33, "Uploaded Goal Photo for \"State Symbol\"", false);
            addLogType(34, "Uploaded Goal Photo for \"American Flag\"", false);
            addLogType(35, "Uploaded Goal Photo for \"Landmark/Memorial\"", false);
            addLogType(36, "Uploaded Goal Photo for \"Camping\"", false);
            addLogType(37, "Uploaded Goal Photo for \"Peaks and Valleys\"", false);
            addLogType(38, "Uploaded Goal Photo for \"Hiking\"", false);
            addLogType(39, "Uploaded Goal Photo for \"Ground Clearance\"", false);
            addLogType(40, "Uploaded Goal Photo for \"Water Fording\"", false);
            addLogType(41, "Uploaded Goal Photo for \"Traction\"", false);
            addLogType(42, "Uploaded Goal Photo for \"Tow Package\"", false);
            addLogType(43, "Uploaded Goal Photo for \"Ultimate Makeover\"", false);
            addLogType(44, "Uploaded Goal Photo for \"Paint Job\"", false);
            addLogType(45, "Needs Maintenance", false);
            addLogType(46, "Owner Maintenance", false);
            addLogType(47, "Update Coordinates", false);
            addLogType(48, "Discovered It", false);
            addLogType(49, "Uploaded Goal Photo for \"Discovery\"", false);
            addLogType(50, "Uploaded Goal Photo for \"Freedom\"", false);
            addLogType(51, "Uploaded Goal Photo for \"Adventure\"", false);
            addLogType(52, "Uploaded Goal Photo for \"Camaraderie\"", false);
            addLogType(53, "Uploaded Goal Photo for \"Heritage\"", false);
            addLogType(54, "Reviewer Note", false);
            addLogType(55, "Lock User (Ban)", false);
            addLogType(56, "Unlock User (Unban)", false);
            addLogType(57, "Groundspeak Note", false);
            addLogType(58, "Uploaded Goal Photo for \"Fun\"", false);
            addLogType(59, "Uploaded Goal Photo for \"Fitness\"", false);
            addLogType(60, "Uploaded Goal Photo for \"Fighting Diabetes\"", false);
            addLogType(61, "Uploaded Goal Photo for \"American Heritage\"", false);
            addLogType(62, "Uploaded Goal Photo for \"No Boundaries\"", false);
            addLogType(63, "Uploaded Goal Photo for \"Only in a Jeep\"", false);
            addLogType(64, "Uploaded Goal Photo for \"Discover New Places\"", false);
            addLogType(65, "Uploaded Goal Photo for \"Definition of Freedom\"", false);
            addLogType(66, "Uploaded Goal Photo for \"Adventure Starts Here\"", false);
            addLogType(67, "Needs Attention", false);
            addLogType(68, "Post Reviewer Note", false);
            addLogType(69, "Move To Collection", false);
            addLogType(70, "Move To Inventory", false);
            addLogType(71, "Throttle User", false);
            addLogType(72, "Enter CAPTCHA", false);
            addLogType(73, "Change Username", false);
            addLogType(74, "Announcement", false);
            addLogType(75, "Visited", false);

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

        protected void addLogType(int id, string name, bool asFound)
        {
            var lt = new DataTypes.LogType();
            lt.ID = id;
            lt.Name = name;
            lt.AsFound = asFound;
            LogTypes.Add(lt);
        }
    }
}
