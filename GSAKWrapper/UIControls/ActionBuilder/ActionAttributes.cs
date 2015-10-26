using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionAttributes : ActionImplementationCondition
    {
        public const string STR_NAME = "Attributes";
        public const string STR_ONOFSELECTED = "AtLeastOneOfSelected";
        private bool _all = false;
        private List<int> _ids = new List<int>();
        public ActionAttributes()
            : base(STR_NAME)
        {
        }
        public override SearchType SearchTypeTarget { get { return SearchType.Attributes; } }

        public override ActionImplementation.Operator AllowOperators
        {
            get
            {
                return ActionImplementation.Operator.Equal | ActionImplementation.Operator.NotEqual;
            }
        }
        public override UIElement GetUIElement()
        {
            if (Values.Count == 0)
            {
                Values.Add(true.ToString());
            }

            StackPanel sp = new StackPanel();
            CheckBox cb = new CheckBox();
            cb.Content = Localization.TranslationManager.Instance.Translate(STR_ONOFSELECTED);
            cb.IsChecked = bool.Parse(Values[0]);
            cb.IsEnabled = false;
            sp.Children.Add(cb);
            ScrollViewer sv = new ScrollViewer();
            UniformGrid g = new UniformGrid();
            g.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            sv.Content = g;
            sv.CanContentScroll = true;
            sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            sv.Width = 200;
            sv.Height = 120;
            foreach (var attr in ApplicationData.Instance.GeocacheAttributes)
            {
                StackPanel ga = new StackPanel();
                ga.Width = 40;
                ga.Height = 60;
                Image img = new Image();
                img.ToolTip = Localization.TranslationManager.Instance.Translate(attr.Name);
                if (Math.Abs(attr.ID) < 100)
                {
                    img.Source = new BitmapImage(Utils.ResourceHelper.GetResourceUri(string.Format("/Resources/Attributes/{0}.gif", attr.ID.ToString().Replace('-', '_'))));
                }
                else
                {
                    img.Source = new BitmapImage(Utils.ResourceHelper.GetResourceUri(string.Format("/Resources/Attributes/{0}.png", attr.ID.ToString().Replace('-', '_'))));
                }
                img.Width = 30;
                img.Height = 30;
                img.HorizontalAlignment = HorizontalAlignment.Center;
                img.VerticalAlignment = VerticalAlignment.Top;
                CheckBox acb = new CheckBox();
                acb.HorizontalAlignment = HorizontalAlignment.Center;
                acb.VerticalAlignment = VerticalAlignment.Top;
                acb.Tag = attr.ID;
                acb.IsChecked = Values.Contains(acb.Tag.ToString());
                ga.Children.Add(img);
                ga.Children.Add(acb);
                g.Children.Add(ga);
            }
            sp.Children.Add(sv);

            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            sv.MouseEnter += new System.Windows.Input.MouseEventHandler(sv_MouseEnter);
            sv.MouseLeave += new System.Windows.Input.MouseEventHandler(sv_MouseLeave);

            return sp;
        }

        void sv_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is ScrollViewer)
            {
                (sender as ScrollViewer).VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                (sender as ScrollViewer).HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                (sender as ScrollViewer).Width = 200;
                (sender as ScrollViewer).Height = 120;
            }
        }

        void sv_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is ScrollViewer)
            {
                (sender as ScrollViewer).VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                (sender as ScrollViewer).HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                (sender as ScrollViewer).Width = 400;
                (sender as ScrollViewer).Height = 400;
            }
        }

        public override void CommitUIData(UIElement uiElement)
        {
            Values.Clear();
            Values.Add(((uiElement as StackPanel).Children[0] as CheckBox).IsChecked.ToString());
            UniformGrid g = ((uiElement as StackPanel).Children[1] as ScrollViewer).Content as UniformGrid;
            foreach (StackPanel sp in g.Children)
            {
                CheckBox cb = sp.Children[1] as CheckBox;
                if (cb.IsChecked == true)
                {
                    Values.Add(cb.Tag.ToString());
                }
            }
        }
        public override bool PrepareRun(Database.DBCon db, string tableName)
        {
            if (Values.Count > 0)
            {
                _all = bool.Parse(Values[0]);
                _ids.Clear();
                for (int i = 1; i < Values.Count; i++)
                {
                    _ids.Add(int.Parse(Values[i]));
                }
            }
            return base.PrepareRun(db, tableName);
        }

        public override void Process(Operator op, string inputTableName, string targetTableName)
        {
            StringBuilder sb = new StringBuilder();
            if (_ids.Count > 0)
            {
                //sb.Append(string.Format("where aId={0} and aInc={1}", Math.Abs(_ids[0]), _ids[0] < 0 ? 0 : 1));
                var posList = (from a in _ids where a > 0 select a).ToArray();
                var negList = (from a in _ids where a < 0 select a).ToArray();
                if (posList.Length > 0)
                {
                    sb.AppendFormat("(Attributes.aId in ({0}) and aInc=1) ", string.Join(",", posList));
                }
                if (negList.Length > 0)
                {
                    if (sb.Length > 0)
                    {
                        sb.Append(" or ");
                    }
                    sb.AppendFormat("(Attributes.aId in ({0}) and aInc=0) ", string.Join(",", negList));
                }
                DatabaseConnection.ExecuteNonQuery(string.Format("insert or ignore into {0} select distinct Code as gccode from Caches inner join {1} on Caches.Code = {1}.gccode inner join Attributes on Caches.Code = Attributes.aCode and ({2})", targetTableName, inputTableName, sb.ToString()));
            }
        }
    }
}
