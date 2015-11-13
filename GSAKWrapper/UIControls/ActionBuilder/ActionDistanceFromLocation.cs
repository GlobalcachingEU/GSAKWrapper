using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionDistanceFromLocation : ActionImplementationCondition
    {
        public const string STR_NAME = "DistanceFromLocationKm";
        public const string STR_CHECK = "Check";
        private double _value = 0.0;
        private Utils.Location _loc = null;
        public ActionDistanceFromLocation()
            : base(STR_NAME)
        {
        }
        public override UIElement GetUIElement()
        {
            if (Values.Count == 0)
            {
                Values.Add(Utils.Conversion.GetCoordinatesPresentation(new Utils.Location(51.5, 5.5)));
            }

            /*
             * ------------------------
             * |  N51.33.... | [...]  |
             * |         10           |
             * ------------------------
             */
            StackPanel sp = new StackPanel();

            var g = new StackPanel();
            g.Orientation = Orientation.Horizontal;
            var opts = ApplicationData.Instance.GSAKLocations.ToArray();
            ComboBox cb = CreateComboBox(opts, Values[0]);
            cb.Width = 180;
            g.Children.Add(cb);

            Button b = new Button();
            b.Content = Localization.TranslationManager.Instance.Translate(STR_CHECK);
            b.HorizontalAlignment = HorizontalAlignment.Right;
            b.Click += new RoutedEventHandler(bActionDistanceToLocation_Click);
            g.Children.Add(b);

            sp.Children.Add(g);

            var tb = new TextBox();
            tb.HorizontalAlignment = HorizontalAlignment.Center;
            if (Values.Count < 2)
            {
                Values.Add("10.0");
            }
            tb.Text = Values[1];
            sp.Children.Add(tb);
            return sp;
        }

        void bActionDistanceToLocation_Click(object sender, RoutedEventArgs e)
        {
            var l = Utils.Conversion.StringToLocation((((sender as Button).Parent as StackPanel).Children[0] as ComboBox).Text);
            if (l != null)
            {
                (((sender as Button).Parent as StackPanel).Children[0] as ComboBox).Text = Utils.Conversion.GetCoordinatesPresentation(l);
            }
        }

        public override void CommitUIData(UIElement uiElement)
        {
            Values[0] = (((uiElement as StackPanel).Children[0] as StackPanel).Children[0] as ComboBox).Text;
            Values[1] = ((uiElement as StackPanel).Children[1] as TextBox).Text;
        }
        public override bool PrepareRun(Database.DBCon db, string tableName)
        {
            if (Values.Count > 1)
            {
                try
                {
                    _loc = Utils.Conversion.StringToLocation(Values[0]);
                    _value = Utils.Conversion.StringToDouble(Values[1]);
                }
                catch
                {
                }
            }
            return base.PrepareRun(db, tableName);
        }

        public override void Process(Operator op, string inputTableName, string targetTableName)
        {
            if (_loc != null)
            {
                if (op == Operator.LessOrEqual)
                {
                    SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("DIST(Latitude, Longitude, {0}, {1}) <= {2}", _loc.Lat.ToString(CultureInfo.InvariantCulture), _loc.Lon.ToString(CultureInfo.InvariantCulture), _value.ToString(CultureInfo.InvariantCulture)));
                }
                else if (op == Operator.Larger)
                {
                    SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("DIST(Latitude, Longitude, {0}, {1}) > {2}", _loc.Lat.ToString(CultureInfo.InvariantCulture), _loc.Lon.ToString(CultureInfo.InvariantCulture), _value.ToString(CultureInfo.InvariantCulture)));
                }
            }
        }

        public override ActionImplementation.Operator AllowOperators
        {
            get
            {
                return ActionImplementation.Operator.LessOrEqual | ActionImplementation.Operator.Larger;
            }
        }
    }
}
