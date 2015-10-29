﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionLogType : ActionImplementationCondition
    {
        public const string STR_NAME = "LogType";
        private string _value = "Found it";
        private double _count = 0;
        public ActionLogType()
            : base(STR_NAME)
        {
        }
        public override SearchType SearchTypeTarget { get { return SearchType.Logs; } }

        public override UIElement GetUIElement()
        {
            if (Values.Count == 0)
            {
                Values.Add("Found it");
            }
            if (Values.Count < 2)
            {
                Values.Add("0");
            }
            StackPanel sp = new StackPanel();
            var opts = (from a in ApplicationData.Instance.LogTypes select a.Name).ToArray();
            ComboBox cb = CreateComboBox(opts, (from a in ApplicationData.Instance.LogTypes where a.Name == Values[0] select a.Name).FirstOrDefault() ?? "Found it");
            cb.IsEditable = false;
            sp.Children.Add(cb);
            TextBox tb = new TextBox();
            tb.Text = Values[1];
            sp.Children.Add(tb);
            return sp;
        }
        public override void CommitUIData(UIElement uiElement)
        {
            StackPanel sp = uiElement as StackPanel;
            ComboBox cb = sp.Children[0] as ComboBox;
            var s = cb.Text;
            Values[0] = (from a in ApplicationData.Instance.LogTypes where a.Name == s select a.Name).FirstOrDefault() ?? "Found it";
            TextBox tb = sp.Children[1] as TextBox;
            Values[1] = tb.Text;
        }
        public override bool PrepareRun(Database.DBCon db, string tableName)
        {
            _value = "Found it";
            _count = 0;
            if (Values.Count > 0)
            {
                _value = Values[0];
            }
            if (Values.Count > 1)
            {
                _count = Utils.Conversion.StringToDouble(Values[1]);
            }
            return base.PrepareRun(db, tableName);
        }

        public override void Process(Operator op, string inputTableName, string targetTableName)
        {
            if (op == Operator.Equal)
            {
                SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("(select count(1) from Logs where Logs.lParent=Caches.Code and lType='{0}') = {1}", _value.Replace("'", "''"), _count.ToString(CultureInfo.InvariantCulture)));
            }
            else if (op == Operator.NotEqual)
            {
                SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("(select count(1) from Logs where Logs.lParent=Caches.Code and lType='{0}') <> {1}", _value.Replace("'", "''"), _count.ToString(CultureInfo.InvariantCulture)));
            }
            else if (op == Operator.Larger)
            {
                SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("(select count(1) from Logs where Logs.lParent=Caches.Code and lType='{0}') > {1}", _value.Replace("'", "''"), _count.ToString(CultureInfo.InvariantCulture)));
            }
            else if (op == Operator.LargerOrEqual)
            {
                SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("(select count(1) from Logs where Logs.lParent=Caches.Code and lType='{0}') >= {1}", _value.Replace("'", "''"), _count.ToString(CultureInfo.InvariantCulture)));
            }
            else if (op == Operator.Less)
            {
                SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("(select count(1) from Logs where Logs.lParent=Caches.Code and lType='{0}') < {1}", _value.Replace("'", "''"), _count.ToString(CultureInfo.InvariantCulture)));
            }
            else if (op == Operator.LessOrEqual)
            {
                SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("(select count(1) from Logs where Logs.lParent=Caches.Code and lType='{0}') <= {1}", _value.Replace("'", "''"), _count.ToString(CultureInfo.InvariantCulture)));
            }
        }
    }
}
