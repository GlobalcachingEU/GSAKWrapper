using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionCollection : ActionImplementationAction
    {
        public const string STR_NAME = "Collection";

        public enum Option
        {
            Add,
            Remove,
        }

        private Option _option = Option.Add;
        private string _collection = "";
        public ActionCollection()
            : base(STR_NAME)
        {
        }

        public override UIElement GetUIElement()
        {
            if (Values.Count == 0)
            {
                Values.Add(Option.Add.ToString());
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
            sp.Children.Add(cb);
            return sp;
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
            _option = Option.Add;
            _collection = "";
            if (Values.Count > 0)
            {
                _option = (Option)Enum.Parse(typeof(Option), Values[0]);
            }
            if (Values.Count > 1)
            {
                _collection = Values[1];
            }
            return base.PrepareRun(db, tableName);
        }

        public override void FinalizeRun()
        {
            TotalProcessTime.Start();
            if (!string.IsNullOrEmpty(_collection))
            {
                var col = Collections.Manager.Instance.GetCollection(_collection, createIfNotExists: _option == Option.Add);
                if (col != null)
                {
                    string target = "target";
                    DatabaseConnection.ExecuteNonQuery(string.Format("ATTACH DATABASE '{0}' as {1}", Collections.Manager.Instance.DatabaseFilePath, target));
                    switch (_option)
                    {
                        case Option.Add:
                            DatabaseConnection.ExecuteNonQuery(string.Format("insert or ignore into {1}.GeocacheCollectionItem (CollectionID, GeocacheCode, Name) select {2} as CollectionID, main.Caches.Code as GeocacheCode, main.Caches.Name as Name from main.Caches inner join {0} on main.Caches.Code = {0}.gccode", ActionInputTableName, target, col.CollectionID));
                            break;
                        case Option.Remove:
                            DatabaseConnection.ExecuteNonQuery(string.Format("delete from {1}.GeocacheCollectionItem where exists (select 1 from {0} where {0}.gccode={1}.GeocacheCollectionItem.GeocacheCode) and GeocacheCollectionItem.CollectionID = {2}", ActionInputTableName, target, col.CollectionID));
                            break;
                    }
                    DatabaseConnection.ExecuteNonQuery(string.Format("DETACH DATABASE {0}", target));
                }
            }
            TotalProcessTime.Stop();
            base.FinalizeRun();
        }
    }
}
