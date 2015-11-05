using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionExport : ActionImplementationAction
    {
        public const string STR_SETTINGS = "Settings";
        private object _settings = null;
        public ActionExport(string name)
            : base(name)
        {
        }

        protected virtual string SetSettings(string currentSettings)
        {
            return currentSettings;
        }

        protected virtual object PrepareSettings(string settings)
        {
            return settings;
        }

        protected virtual void PerformExport(object settings)
        {
        }

        public override UIElement GetUIElement()
        {
            if (Values.Count == 0)
            {
                Values.Add("");
            }
            Button b = new Button();
            b.Content = Localization.TranslationManager.Instance.Translate(STR_SETTINGS);
            b.Click += b_Click;
            return b;
        }

        void b_Click(object sender, RoutedEventArgs e)
        {
            Values[0] = SetSettings(Values[0]) ?? "";
        }

        public override bool PrepareRun(Database.DBCon db, string tableName)
        {
            if (Values.Count == 0)
            {
                Values.Add("");
            }
            _settings = PrepareSettings(Values[0]);
            return base.PrepareRun(db, tableName);
        }

        public override void FinalizeRun()
        {
            TotalProcessTime.Start();
            try
            {
                PerformExport(_settings);
            }
            finally
            {
                TotalProcessTime.Stop();
            }
            base.FinalizeRun();
        }
    }
}
