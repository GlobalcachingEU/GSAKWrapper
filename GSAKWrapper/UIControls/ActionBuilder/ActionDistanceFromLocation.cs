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
        public const string STR_NAME = "ActionDistanceFromLocationKm";
        private double _value = 0.0;
        private Utils.Location _loc = null;
        public ActionDistanceFromLocation()
            : base(STR_NAME)
        {
        }
        public override UIElement GetUIElement()
        {
            /*
             * ------------------------
             * |  N51.33.... | [...]  |
             * |         10           |
             * ------------------------
             */
            StackPanel sp = new StackPanel();

            Grid g = new Grid();
            TextBox tb = new TextBox();
            tb.HorizontalAlignment = HorizontalAlignment.Left;
            tb.Width = 180;
            if (Values.Count == 0)
            {
                Values.Add(Utils.Conversion.GetCoordinatesPresentation(new Utils.Location(51.5, 5.5)));
            }
            tb.Text = Values[0];
            g.Children.Add(tb);

            Button b = new Button();
            b.Content = "check";
            b.HorizontalAlignment = HorizontalAlignment.Right;
            b.Click += new RoutedEventHandler(bActionDistanceToLocation_Click);
            g.Children.Add(b);

            sp.Children.Add(g);

            tb = new TextBox();
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
            var l = Utils.Conversion.StringToLocation((((sender as Button).Parent as Grid).Children[0] as TextBox).Text);
            if (l != null)
            {
                (((sender as Button).Parent as Grid).Children[0] as TextBox).Text = Utils.Conversion.GetCoordinatesPresentation(l);
            }
        }

        public override void CommitUIData(UIElement uiElement)
        {
            Values[0] = (((uiElement as StackPanel).Children[0] as Grid).Children[0] as TextBox).Text;
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
                double minLat, minLon, maxLat, maxLon;
                Utils.Calculus.GetEnvelope(_loc.Lat, _loc.Lon, _value, out minLat, out minLon, out maxLat, out maxLon);
                if (op == Operator.Larger)
                {
                    SelectGeocachesOnWhereClause(inputTableName, targetTableName
                        , string.Format("Latitude>{0} or Latitude<{1} or Longitude>{2} or Longitude<{3}"
                        , maxLat.ToString(CultureInfo.InvariantCulture)
                        , minLat.ToString(CultureInfo.InvariantCulture)
                        , maxLon.ToString(CultureInfo.InvariantCulture)
                        , minLon.ToString(CultureInfo.InvariantCulture)
                    ));
                }

                var dr = DatabaseConnection.ExecuteReader(string.Format("select Code, Latitude, Longitude from Caches inner join {0} on Caches.Code = {0}.gccode where Latitude<={1} and Latitude>={2} and Longitude<={3} and Longitude>={4}"
                        , inputTableName
                        , maxLat.ToString(CultureInfo.InvariantCulture)
                        , minLat.ToString(CultureInfo.InvariantCulture)
                        , maxLon.ToString(CultureInfo.InvariantCulture)
                        , minLon.ToString(CultureInfo.InvariantCulture)
                    ));
                List<string> codes = new List<string>();
                while (dr.Read())
                {
                    var slat = dr.GetString(1);
                    var slon = dr.GetString(2);
                    if (!string.IsNullOrEmpty(slat) && !string.IsNullOrEmpty(slon))
                    {
                        double dist = Utils.Calculus.CalculateDistance(Utils.Conversion.StringToDouble(slat), Utils.Conversion.StringToDouble(slon), _loc.Lat, _loc.Lon).EllipsoidalDistance / 1000.0;
                        if ((op == Operator.Larger && dist > _value) || (op == Operator.LessOrEqual && dist <= _value))
                        {
                            codes.Add(dr.GetString(0));
                        }
                    }
                }
                InsertGeocacheCodes(targetTableName, codes);
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
