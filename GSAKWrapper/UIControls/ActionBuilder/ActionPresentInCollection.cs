using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionPresentInCollection : ActionImplementationCondition
    {
        public const string STR_NAME = "PresentInCollection";

        public enum Option
        {
            Yes,
            No,
        }

        private Option _option = Option.Yes;
        private string _collection = "";
        public ActionPresentInCollection()
            : base(STR_NAME)
        {
        }

        public override SearchType SearchTypeTarget { get { return SearchType.Extra; } }

        public override UIElement GetUIElement()
        {
            if (Values.Count == 0)
            {
                Values.Add(Option.Yes.ToString());
            }
            if (Values.Count < 2)
            {
                Values.Add("");
            }
            StackPanel sp = new StackPanel();
            var opts = Enum.GetNames(typeof(Option)).ToList();
            ComboBox cb = CreateComboBox(opts.ToArray(), Values[0]);
            cb.IsEditable = false;
            sp.Children.Add(cb);
            var opts2 = (from a in Collections.Manager.Instance.GetGeocacheCollections() select a.Name).ToArray();
            //cb = CreateComboBox(opts2, (from a in opts2 where string.Compare(a, Values[1], true)==0 select a).FirstOrDefault() ?? "");
            cb = CreateComboBox(opts2, Values[1]);
            cb.DropDownOpened += cb_DropDownOpened;
            sp.Children.Add(cb);
            return sp;
        }

        void cb_DropDownOpened(object sender, EventArgs e)
        {
            var cb = sender as ComboBox;
            var items = new List<ComboBoxItem>();
            foreach (ComboBoxItem item in cb.Items)
            {
                if (item.Content as string != cb.Text)
                {
                    items.Add(item);
                }
            }
            foreach (var item in items)
            {
                cb.Items.Remove(item);
            }
            var cf = Collections.Manager.Instance.GetGeocacheCollections();
            foreach (var c in cf)
            {
                if (string.Compare(cb.Text, c.Name, true) != 0)
                {
                    ComboBoxItem cboxitem = new ComboBoxItem();
                    cboxitem.Content = c.Name;
                    cb.Items.Add(cboxitem);
                }
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
            _option = Option.Yes;
            _collection = "";
            if (Values.Count > 0)
            {
                _option = (Option)Enum.Parse(typeof(Option), Values[0]);
            }
            if (Values.Count > 1)
            {
                _collection = Values[1];
            }
            if (!string.IsNullOrEmpty(_collection))
            {
                var opts = (from a in Collections.Manager.Instance.GetGeocacheCollections() select a.Name).ToArray();
                _collection = (from a in opts where string.Compare(a, _collection, true) == 0 select a).FirstOrDefault() ?? "";
            }
            return base.PrepareRun(db, tableName);
        }

        public override void Process(Operator op, string inputTableName, string targetTableName)
        {
            if (!string.IsNullOrEmpty(_collection))
            {
                var col = Collections.Manager.Instance.GetCollection(_collection);
                if (col != null)
                {
                    string target = "target";
                    DatabaseConnection.ExecuteNonQuery(string.Format("ATTACH DATABASE '{0}' as {1}", Collections.Manager.Instance.DatabaseFilePath, target));
                    switch (_option)
                    {
                        case Option.Yes:
                            if (op == Operator.Equal)
                            {
                                SelectGeocachesOnWhereClause(inputTableName, targetTableName, "1=1", innerJoins: string.Format("inner join {0}.GeocacheCollectionItem on {1}.gccode={0}.GeocacheCollectionItem.GeocacheCode and {0}.GeocacheCollectionItem.CollectionID = {2}", target, inputTableName, col.CollectionID));
                            }
                            else if (op == Operator.NotEqual)
                            {
                                SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0}.GeocacheCollectionItem.GeocacheCode is null", target), innerJoins: string.Format("inner join {0}.GeocacheCollectionItem on {1}.gccode={0}.GeocacheCollectionItem.GeocacheCode and {0}.GeocacheCollectionItem.CollectionID = {2}", target, inputTableName, col.CollectionID));
                            }
                            break;
                        case Option.No:
                            if (op == Operator.Equal)
                            {
                                SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0}.GeocacheCollectionItem.GeocacheCode is null", target), innerJoins: string.Format("inner join {0}.GeocacheCollectionItem on {1}.gccode={0}.GeocacheCollectionItem.GeocacheCode and {0}.GeocacheCollectionItem.CollectionID = {2}", target, inputTableName, col.CollectionID));
                            }
                            else if (op == Operator.NotEqual)
                            {
                                SelectGeocachesOnWhereClause(inputTableName, targetTableName, "1=1", innerJoins: string.Format("inner join {0}.GeocacheCollectionItem on {1}.gccode={0}.GeocacheCollectionItem.GeocacheCode and {0}.GeocacheCollectionItem.CollectionID = {2}", target, inputTableName, col.CollectionID));
                            }
                            break;
                    }
                    DatabaseConnection.ExecuteNonQuery(string.Format("DETACH DATABASE {0}", target));
                }
            }
        }
    }
}
