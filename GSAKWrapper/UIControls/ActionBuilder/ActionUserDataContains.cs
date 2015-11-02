using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionUserDataContains : ActionImplementationCondition
    {
        public const string STR_NAME = "UserDataText";

        public enum Option
        {
            Contains,
            EqualsTo,
            RegEx,
            IsEmpty
        }

        public enum UserDataField
        {
            UserData,
            UserData2,
            UserData3,
            UserData4,
        }

        private string _value = "";
        private Option _option = Option.Contains;
        private UserDataField _userDataField = UserDataField.UserData;

        public ActionUserDataContains()
            : base(STR_NAME)
        {
        }

        public override UIElement GetUIElement()
        {
            if (Values.Count == 0)
            {
                Values.Add("-");
            }
            if (Values.Count < 2)
            {
                Values.Add(Option.Contains.ToString());
            }
            if (Values.Count < 3)
            {
                Values.Add(UserDataField.UserData.ToString());
            }
            StackPanel sp = new StackPanel();
            var opts2 = Enum.GetNames(typeof(UserDataField));
            ComboBox cb = CreateComboBox(opts2, Values[2]);
            cb.IsEditable = false;
            sp.Children.Add(cb);
            var opts = Enum.GetNames(typeof(Option));
            cb = CreateComboBox(opts, Values[1]);
            cb.IsEditable = false;
            sp.Children.Add(cb);
            TextBox tb = new TextBox();
            tb.HorizontalAlignment = HorizontalAlignment.Center;
            if (Values.Count == 0)
            {
                Values.Add("-");
            }
            tb.Text = Values[0];
            sp.Children.Add(tb);
            return sp;
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
            ComboBox cb = sp.Children[1] as ComboBox;
            Values[1] = cb.Text;
            TextBox tb = sp.Children[2] as TextBox;
            Values[0] = tb.Text;
            cb = sp.Children[0] as ComboBox;
            Values[2] = cb.Text;
        }
        public override bool PrepareRun(Database.DBCon db, string tableName)
        {
            _value = "";
            _option = Option.Contains;
            _userDataField = UserDataField.UserData;
            if (Values.Count > 0)
            {
                _value = Values[0];
            }
            if (Values.Count > 1)
            {
                _option = (Option)Enum.Parse(typeof(Option), Values[1]);
            }
            if (Values.Count > 2)
            {
                _userDataField = (UserDataField)Enum.Parse(typeof(UserDataField), Values[2]);
            }
            return base.PrepareRun(db, tableName);
        }

        public override void Process(Operator op, string inputTableName, string targetTableName)
        {
            var fieldName = "";
            switch (_userDataField)
            {
                case UserDataField.UserData:
                    fieldName = "UserData";
                    break;
                case UserDataField.UserData2:
                    fieldName = "User2";
                    break;
                case UserDataField.UserData3:
                    fieldName = "User3";
                    break;
                case UserDataField.UserData4:
                    fieldName = "User4";
                    break;
            }
            switch (_option)
            {
                case Option.Contains:
                    if (op == Operator.Equal)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} like '%{1}%'", fieldName, _value.Replace("'", "''")));
                    }
                    else if (op == Operator.NotEqual)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} not like '%{1}%'", fieldName, _value.Replace("'", "''")));
                    }
                    break;
                case Option.EqualsTo:
                    if (op == Operator.Equal)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} like '{1}'", fieldName, _value.Replace("'", "''")));
                    }
                    else if (op == Operator.NotEqual)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} not like '{1}'", fieldName, _value.Replace("'", "''")));
                    }
                    break;
                case Option.RegEx:
                    if (op == Operator.Equal)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} REGEXP '{1}'", fieldName, _value.Replace("'", "''")));
                    }
                    else if (op == Operator.NotEqual)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} not REGEXP '{1}'", fieldName, _value.Replace("'", "''")));
                    }
                    break;
                case Option.IsEmpty:
                    if (op == Operator.Equal)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} is NULL", fieldName));
                    }
                    else if (op == Operator.NotEqual)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} is not NULL", fieldName));
                    }
                    break;
            }
        }

    }
}
