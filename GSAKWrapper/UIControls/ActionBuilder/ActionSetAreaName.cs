using GSAKWrapper.Shapefiles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionSetAreaName : ActionImplementationAction
    {
        public const string STR_NAME = "SetAreaName";
        public ObservableCollection<string> AvailablePrefixes { get; set; }
        public ObservableCollection<string> AvailableFields { get; set; }

        private AreaType _level = AreaType.State;
        private string _prefix = "";
        private string _field = "";
        public ActionSetAreaName()
            : base(STR_NAME)
        {
        }

        public override UIElement GetUIElement()
        {
            if (AvailablePrefixes == null)
            {
                AvailablePrefixes = new ObservableCollection<string>();
            }
            if (AvailableFields == null)
            {
                AvailableFields = new ObservableCollection<string>();
            }

            if (Values.Count == 0)
            {
                Values.Add(true.ToString());
            }
            if (Values.Count < 2)
            {
                Values.Add("");
            }
            if (Values.Count < 3)
            {
                Values.Add("State");
            }
            StackPanel sp = new StackPanel();
            var opts = Enum.GetNames(typeof(AreaType)).ToArray();
            ComboBox cb = CreateComboBox(opts, Values[0]);
            cb.IsEditable = false;
            sp.Children.Add(cb);

            var opts2 = new string[] { };
            cb = CreateComboBox(opts2, Values[1]);
            cb.DropDownOpened += cb_DropDownOpened;
            sp.Children.Add(cb);

            Binding binding = new Binding();
            binding.Source = AvailablePrefixes;
            BindingOperations.SetBinding(cb, ComboBox.ItemsSourceProperty, binding);

            var opts3 = new string[] { };
            cb = CreateComboBox(opts3, Values[2]);
            cb.DropDownOpened += cb2_DropDownOpened;
            sp.Children.Add(cb);

            binding = new Binding();
            binding.Source = AvailableFields;
            BindingOperations.SetBinding(cb, ComboBox.ItemsSourceProperty, binding);

            return sp;
        }

        public override void CommitUIData(UIElement uiElement)
        {
            StackPanel sp = uiElement as StackPanel;
            ComboBox cb = sp.Children[0] as ComboBox;
            Values[0] = cb.Text;
            cb = sp.Children[1] as ComboBox;
            Values[1] = cb.Text;
            cb = sp.Children[2] as ComboBox;
            Values[2] = cb.Text;
        }

        void cb_DropDownOpened(object sender, EventArgs e)
        {
            AvailablePrefixes.Clear();
            var opt = Shapefiles.Manager.Instance.AvailablePrefixes;
            foreach (var c in opt)
            {
                AvailablePrefixes.Add(c);
            }
        }

        void cb2_DropDownOpened(object sender, EventArgs e)
        {
            AvailableFields.Clear();
            List<string> opt = new List<string>();
            var fn = System.IO.Path.Combine(Settings.Settings.Default.DatabaseFolderPath, Settings.Settings.Default.SelectedDatabase, "sqlite.db3");
            if (System.IO.File.Exists(fn))
            {
                using (var db = new Database.DBConSqlite(fn))
                {
                    opt = AvailableCustomFields(db);
                }
            }
            opt.Insert(0, "County");
            opt.Insert(0, "State");
            opt.Insert(0, "Country");
            foreach (var c in opt)
            {
                AvailableFields.Add(c);
            }
        }

        public override bool PrepareRun(Database.DBCon db, string tableName)
        {
            _level = AreaType.State;
            _prefix = "";
            _field = "";
            if (Values.Count > 0)
            {
                _level = (AreaType)Enum.Parse(typeof(AreaType), Values[0]);
            }
            if (Values.Count > 1)
            {
                _prefix = Values[1];
            }
            if (Values.Count > 2)
            {
                _field = Values[2];
            }
            return base.PrepareRun(db, tableName);
        }

        public override void FinalizeRun()
        {
            if (!string.IsNullOrEmpty(_field))
            {
                if (string.Compare(_field, "Country", true) == 0
                    || string.Compare(_field, "State", true) == 0
                    || string.Compare(_field, "County", true) == 0)
                {
                    UpdateCachesFromInputTable(string.Format("{0}=AREANAME(Latitude, Longitude, '{1}', '{2}')", _field, _level.ToString(), _prefix.Replace("'", "''")));
                }
                else
                {
                    TotalProcessTime.Start();
                    DatabaseConnection.ExecuteNonQuery(string.Format("update Custom set {1} = (select AREANAME(Latitude, Longitude, '{2}', '{3}') as {1} from Caches where Caches.Code=Custom.cCode) where exists (select 1 from {0} where {0}.gccode=Custom.cCode)", ActionInputTableName, _field, _level.ToString(), _prefix.Replace("'", "''")));
                    TotalProcessTime.Stop();
                }
            }
            base.FinalizeRun();
        }
    }
}
