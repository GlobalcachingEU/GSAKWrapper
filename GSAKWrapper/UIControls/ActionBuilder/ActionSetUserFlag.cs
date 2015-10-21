using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionSetUserFlag : ActionImplementationAction
    {
        public const string STR_NAME = "SetUserFlag";
        private string _value = "";
        public ActionSetUserFlag()
            : base(STR_NAME)
        {
        }
        public override UIElement GetUIElement()
        {
            if (Values.Count == 0)
            {
                Values.Add(true.ToString());
            }
            ComboBox cb = CreateComboBox(new string[] { true.ToString(), false.ToString() }, Values[0]);
            return cb;
        }

        public override void CommitUIData(UIElement uiElement)
        {
            ComboBox cb = uiElement as ComboBox;
            Values[0] = cb.Text;
        }

        public override bool PrepareRun(Database.DBCon db, string tableName)
        {
            _value = true.ToString();
            if (Values.Count > 0)
            {
                bool flag;
                if (bool.TryParse(Values[0], out flag))
                {
                    _value = flag ? "1" : "0";
                }
            }
            return base.PrepareRun(db, tableName);
        }

        public override void FinalizeRun()
        {
            UpdateCachesFromInputTable(string.Format("UserFlag={0}", _value));
            base.FinalizeRun();
        }
    }
}
