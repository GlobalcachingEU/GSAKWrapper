using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionCompareTwoDates : ActionImplementationCondition
    {
        public const string STR_NAME = "CompareTwoDates";
        public const string STR_COMPARETO = "CompareTo";
        private DateFields _dateField1 = DateFields.Unknown;
        private DateFields _dateField2 = DateFields.Unknown;
        public ActionCompareTwoDates()
            : base(STR_NAME)
        {
        }
        public override SearchType SearchTypeTarget { get { return SearchType.Dates; } }

        public enum DateFields
        {
            Unknown,
            DidNotFind,
            Created,
            Changed,
            FoundByMe,
            LastFound,
            LastGPX,
            LastLog,
            LastUser,
            Placed,
            UserNote
        }
        public override UIElement GetUIElement()
        {
            if (Values.Count == 0)
            {
                Values.Add(DateFields.Placed.ToString());
            }
            if (Values.Count < 2)
            {
                Values.Add(DateFields.FoundByMe.ToString());
            }
            StackPanel sp = new StackPanel();
            var opts = Enum.GetNames(typeof(DateFields)).Skip(1).ToArray();
            ComboBox cb = CreateComboBox(opts, Values[0]);
            cb.IsEditable = false;
            sp.Children.Add(cb);
            TextBlock tb = new TextBlock();
            tb.Text = Localization.TranslationManager.Instance.Translate(STR_COMPARETO) as string;
            sp.Children.Add(tb);
            ComboBox cb1 = CreateComboBox(opts, Values[1]);
            cb1.IsEditable = false;
            sp.Children.Add(cb1);
            return sp;
        }

        public override void CommitUIData(UIElement uiElement)
        {
            StackPanel sp = uiElement as StackPanel;
            ComboBox cb = sp.Children[0] as ComboBox;
            Values[0] = cb.Text;
            cb = sp.Children[2] as ComboBox;
            Values[1] = cb.Text;
        }
        public override bool PrepareRun(Database.DBCon db, string tableName)
        {
            if (Values.Count > 0)
            {
                _dateField1 = (DateFields)Enum.Parse(typeof(DateFields), Values[0]);
            }
            if (Values.Count > 1)
            {
                _dateField2 = (DateFields)Enum.Parse(typeof(DateFields), Values[1]);
            }
            return base.PrepareRun(db, tableName);
        }

        public override void Process(Operator op, string inputTableName, string targetTableName)
        {
            var f1 = GetDateField(_dateField1);
            var f2 = GetDateField(_dateField2);
            if (!string.IsNullOrEmpty(f1) && !string.IsNullOrEmpty(f2))
            {
                if (op == Operator.Equal)
                {
                    SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} = {1}", f1, f2));
                }
                else if (op == Operator.NotEqual)
                {
                    SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} <> {1}", f1, f2));
                }
                else if (op == Operator.Larger)
                {
                    SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} > {1}", f1, f2));
                }
                else if (op == Operator.LargerOrEqual)
                {
                    SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} >= {1}", f1, f2));
                }
                else if (op == Operator.Less)
                {
                    SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} < {1}", f1, f2));
                }
                else if (op == Operator.LessOrEqual)
                {
                    SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("{0} <= {1}", f1, f2));
                }
            }
        }

        private string GetDateField(DateFields f)
        {
            string result = null;
            switch (f)
            {
                case DateFields.Changed:
                    result = "ChangedDate";
                    break;
                case DateFields.Created:
                    result = "Created";
                    break;
                case DateFields.DidNotFind:
                    result = "DNFDate";
                    break;
                case DateFields.FoundByMe:
                    result = "FoundByMeDate";
                    break;
                case DateFields.LastFound:
                    result = "LastFoundDate";
                    break;
                case DateFields.LastGPX:
                    result = "LastGPXDate";
                    break;
                case DateFields.LastLog:
                    result = "LastLog";
                    break;
                case DateFields.LastUser:
                    result = "LastUserDate";
                    break;
                case DateFields.Placed:
                    result = "PlacedDate";
                    break;
                case DateFields.UserNote:
                    result = "UserNoteDate";
                    break;
            }
            return result;
        }
    }

}
