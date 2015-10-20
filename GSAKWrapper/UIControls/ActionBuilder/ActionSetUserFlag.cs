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

        public override void FinalizeRun()
        {
            TotalProcessTime.Start();
            if ((long)DatabaseConnection.ExecuteScalar(string.Format("select count(1) from {0}", ActionInputTableName)) > 0)
            {
                DatabaseConnection.ExecuteNonQuery(string.Format("update Caches set UserFlag=1 where exists (select 1 from {0} where Caches.Code={0}.gccode)", ActionInputTableName));
            }
            TotalProcessTime.Stop();
            base.FinalizeRun();
        }
    }
}
