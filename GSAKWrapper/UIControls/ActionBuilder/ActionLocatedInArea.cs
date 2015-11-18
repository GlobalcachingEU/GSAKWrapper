﻿using System;
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
    public class ActionLocatedInArea : ActionImplementationCondition
    {
        public const string STR_NAME = "LocatedInArea";

        public ObservableCollection<string> AvailableAreas { get; set; }

        private Shapefiles.AreaType _option = Shapefiles.AreaType.State;
        private string _areaname = "";
        public ActionLocatedInArea()
            : base(STR_NAME)
        {
        }

        public override SearchType SearchTypeTarget { get { return SearchType.Extra; } }

        public override UIElement GetUIElement()
        {
            if (Values.Count == 0)
            {
                Values.Add(Shapefiles.AreaType.State.ToString());
            }
            if (Values.Count < 2)
            {
                Values.Add("");
            }
            if (AvailableAreas == null)
            {
                AvailableAreas = new ObservableCollection<string>();
            }
            StackPanel sp = new StackPanel();
            var opts = Enum.GetNames(typeof(Shapefiles.AreaType)).ToList();
            ComboBox cb = CreateComboBox(opts.ToArray(), Values[0]);
            cb.IsEditable = false;
            sp.Children.Add(cb);
            var opts2 = new string[] { };
            cb = CreateComboBox(opts2, Values[1]);
            cb.DropDownOpened += cb_DropDownOpened;
            sp.Children.Add(cb);

            Binding binding = new Binding();
            binding.Source = AvailableAreas;  // view model?
            BindingOperations.SetBinding(cb, ComboBox.ItemsSourceProperty, binding);

            return sp;
        }

        void cb_DropDownOpened(object sender, EventArgs e)
        {
            AvailableAreas.Clear();
            var opt = (Shapefiles.AreaType)Enum.Parse(typeof(Shapefiles.AreaType), ((UIActionControl.ActionContent.Children[0] as StackPanel).Children[0] as ComboBox).Text);
            var cf = Shapefiles.Manager.Instance.GetAreasByLevel(opt).OrderBy(x => x.Name).ToList();
            foreach (var c in cf)
            {
                AvailableAreas.Add(c.Name);
            }
        }
        public override ActionImplementation.Operator AllowOperators
        {
            get
            {
                return ActionImplementation.Operator.Equal | ActionImplementation.Operator.NotEqual;
            }
        }
        public override void CommitUIData(UIElement uiElement)
        {
            StackPanel sp = uiElement as StackPanel;
            ComboBox cb = sp.Children[0] as ComboBox;
            Values[0] = cb.Text;
            cb = sp.Children[1] as ComboBox;
            Values[1] = cb.Text;
        }
        public override bool PrepareRun(Database.DBCon db, string tableName)
        {
            _option = Shapefiles.AreaType.State;
            _areaname = "";
            if (Values.Count > 0)
            {
                _option = (Shapefiles.AreaType)Enum.Parse(typeof(Shapefiles.AreaType), Values[0]);
            }
            if (Values.Count > 1)
            {
                _areaname = Values[1];
            }
            return base.PrepareRun(db, tableName);
        }

        public override void Process(Operator op, string inputTableName, string targetTableName)
        {
            if (!string.IsNullOrEmpty(_areaname))
            {
                if (op == Operator.Equal)
                {
                    SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("INAREA(Latitude, Longitude, '{0}', '{1}') = 1", _option.ToString(), _areaname.Replace("'","''")));
                }
                else if (op == Operator.NotEqual)
                {
                    SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("INAREA(Latitude, Longitude, '{0}', '{1}') = 0", _option.ToString(), _areaname.Replace("'", "''")));
                }
            }
        }
    }
}
