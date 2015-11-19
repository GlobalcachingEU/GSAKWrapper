using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionScriptFilter : ActionImplementationCondition
    {
        public const string STR_NAME = "ScriptFilter";

        private string _script = "";
        private Script.IFilterScript _scriptObject = null;
        public ActionScriptFilter()
            : base(STR_NAME)
        {
        }

        public override SearchType SearchTypeTarget { get { return SearchType.Extra; } }

        public override UIElement GetUIElement()
        {
            if (Values.Count == 0)
            {
                Values.Add("");
            }
            StackPanel sp = new StackPanel();
            var opts = (from a in Settings.Settings.Default.GetScriptItems() select a.Name).ToArray();
            ComboBox cb = CreateComboBox(opts.ToArray(), Values[0]);
            cb.IsEditable = true;
            sp.Children.Add(cb);
            return sp;
        }
        public override void CommitUIData(UIElement uiElement)
        {
            StackPanel sp = uiElement as StackPanel;
            ComboBox cb = sp.Children[0] as ComboBox;
            Values[0] = cb.Text;
        }
        public override bool PrepareRun(Database.DBCon db, string tableName)
        {
            _script = "";
            _scriptObject = null;
            if (Values.Count > 0)
            {
                _script = Values[0];
            }
            if (!string.IsNullOrEmpty(_script))
            {
                var scr = Settings.Settings.Default.GetScriptItem(_script);
                if (scr != null)
                {
                    _scriptObject = Script.Manager.Instance.LoadFilterScript(scr.Code);
                    _scriptObject.PrepareRun(this, db, tableName);
                }
            }
            return base.PrepareRun(db, tableName);
        }

        public override void Process(Operator op, string inputTableName, string targetTableName)
        {
            if (_scriptObject != null)
            {
                _scriptObject.Process(this, op, inputTableName, targetTableName);
            }
        }

        public override void FinalizeRun()
        {
            if (_scriptObject != null)
            {
                _scriptObject.FinalizeRun(this);
            }
            base.FinalizeRun();
        }
    }
}
