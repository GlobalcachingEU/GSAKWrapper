using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionUserDataNumber : ActionImplementationCondition
    {
        public const string STR_NAME = "UserDataNumber";

        public enum Option
        {
            Value,
            IsEmpty
        }

        public enum UserDataField
        {
            UserData,
            UserData2,
            UserData3,
            UserData4,
        }

        private double _value = 0.0;
        private Option _option = Option.Value;
        private UserDataField _userDataField = UserDataField.UserData;

        public ActionUserDataNumber()
            : base(STR_NAME)
        {
        }

        public override UIElement GetUIElement()
        {
            if (Values.Count == 0)
            {
                Values.Add("0.0");
            }
            if (Values.Count < 2)
            {
                Values.Add(Option.Value.ToString());
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
            _value = 0.0;
            _option = Option.Value;
            _userDataField = UserDataField.UserData;
            if (Values.Count > 0)
            {
                _value = Utils.Conversion.StringToDouble(Values[0]);
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
                case Option.Value:
                    if (op == Operator.Equal)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} = {1}", fieldName, _value.ToString(CultureInfo.InvariantCulture)));
                    }
                    else if (op == Operator.NotEqual)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} <> {1}", fieldName, _value.ToString(CultureInfo.InvariantCulture)));
                    }
                    else if (op == Operator.Larger)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} > {1}", fieldName, _value.ToString(CultureInfo.InvariantCulture)));
                    }
                    else if (op == Operator.LargerOrEqual)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} >= {1}", fieldName, _value.ToString(CultureInfo.InvariantCulture)));
                    }
                    else if (op == Operator.Less)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} < {1}", fieldName, _value.ToString(CultureInfo.InvariantCulture)));
                    }
                    else if (op == Operator.LessOrEqual)
                    {
                        SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} <= {1}", fieldName, _value.ToString(CultureInfo.InvariantCulture)));
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
