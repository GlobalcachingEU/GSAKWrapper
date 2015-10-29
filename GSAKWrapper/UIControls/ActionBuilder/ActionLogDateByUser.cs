using System;
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
    public class ActionLogDateByUser : ActionImplementationCondition
    {
        public const string STR_NAME = "LogDateByUser";
        private DateTime _value = DateTime.Now.Date;
        private string _user = "user name";
        public ActionLogDateByUser()
            : base(STR_NAME)
        {
        }
        public override SearchType SearchTypeTarget { get { return SearchType.Logs; } }

        public override UIElement GetUIElement()
        {
            if (Values.Count == 0)
            {
                Values.Add(DateTime.Now.Date.ToString("yyyy-MM-dd"));
            }
            if (Values.Count < 2)
            {
                Values.Add("user name");
            }
            StackPanel sp = new StackPanel();
            DatePicker dp = new DatePicker();
            dp.HorizontalAlignment = HorizontalAlignment.Center;
            dp.SelectedDate = DateTime.ParseExact(Values[0], "yyyy-MM-dd", CultureInfo.InvariantCulture).Date;
            sp.Children.Add(dp);
            TextBox tb = new TextBox();
            tb.Text = Values[1];
            sp.Children.Add(tb);
            return sp;
        }
        public override void CommitUIData(UIElement uiElement)
        {
            StackPanel sp = uiElement as StackPanel;
            DatePicker dp = sp.Children[0] as DatePicker;
            Values[0] = (dp.SelectedDate != null ? ((DateTime)dp.SelectedDate).Date : DateTime.Now.Date).ToString("yyyy-MM-dd");
            TextBox tb = sp.Children[1] as TextBox;
            Values[1] = tb.Text;
        }
        public override bool PrepareRun(Database.DBCon db, string tableName)
        {
            _value = DateTime.Now.Date;
            _user = "user name";
            if (Values.Count > 0)
            {
                _value = DateTime.ParseExact(Values[0], "yyyy-MM-dd", CultureInfo.InvariantCulture).Date;
            }
            if (Values.Count > 1)
            {
                _user = Values[1];
            }
            return base.PrepareRun(db, tableName);
        }

        public override void Process(Operator op, string inputTableName, string targetTableName)
        {
            if (op == Operator.Equal)
            {
                SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("lDate = '{0}' and lBy like '{1}'", _value.ToString("yyyy-MM-dd"), _user.Replace("'", "''")), innerJoins: "inner join Logs on Caches.Code=Logs.lParent");
            }
            else if (op == Operator.NotEqual)
            {
                SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("lDate <> '{0}' and lBy like '{1}'", _value.ToString("yyyy-MM-dd"), _user.Replace("'", "''")), innerJoins: "inner join Logs on Caches.Code=Logs.lParent");
            }
            else if (op == Operator.Larger)
            {
                SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("lDate > '{0}' and lBy like '{1}'", _value.ToString("yyyy-MM-dd"), _user.Replace("'", "''")), innerJoins: "inner join Logs on Caches.Code=Logs.lParent");
            }
            else if (op == Operator.LargerOrEqual)
            {
                SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("lDate >= '{0}' and lBy like '{1}'", _value.ToString("yyyy-MM-dd"), _user.Replace("'", "''")), innerJoins: "inner join Logs on Caches.Code=Logs.lParent");
            }
            else if (op == Operator.Less)
            {
                SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("lDate < '{0}' and lBy like '{1}'", _value.ToString("yyyy-MM-dd"), _user.Replace("'", "''")), innerJoins: "inner join Logs on Caches.Code=Logs.lParent");
            }
            else if (op == Operator.LessOrEqual)
            {
                SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("lDate <= '{0}' and lBy like '{1}'", _value.ToString("yyyy-MM-dd"), _user.Replace("'", "''")), innerJoins: "inner join Logs on Caches.Code=Logs.lParent");
            }
        }
    }
}
