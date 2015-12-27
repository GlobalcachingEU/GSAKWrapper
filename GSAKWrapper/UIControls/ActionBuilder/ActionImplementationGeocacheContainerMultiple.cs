using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionImplementationGeocacheContainerMultiple : ActionImplementationCondition
    {
        public const string STR_NAME = "GeocacheContainerMultiple";
        private string _value = "";
        public ActionImplementationGeocacheContainerMultiple()
            : base(STR_NAME)
        {
        }

        public override SearchType SearchTypeTarget { get { return SearchType.Other; } }

        public override UIElement GetUIElement()
        {
            var grid = new Grid();

            ColumnDefinition columnDefinition1 = new ColumnDefinition();
            ColumnDefinition columnDefinition2 = new ColumnDefinition();
            columnDefinition1.Width = new GridLength(1, GridUnitType.Auto);
            columnDefinition2.Width = new GridLength(1, GridUnitType.Star);

            grid.ColumnDefinitions.Add(columnDefinition1);
            grid.ColumnDefinitions.Add(columnDefinition2);

            foreach (var gt in ApplicationData.Instance.GeocacheContainers)
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = GridLength.Auto;
                grid.RowDefinitions.Add(rowDefinition);

                var cb = new CheckBox();
                cb.IsChecked = Values.Contains(gt.Name);
                grid.Children.Add(cb);
                Grid.SetRow(cb, grid.RowDefinitions.Count - 1);
                Grid.SetColumn(cb, 0);

                var txt = new TextBlock();
                txt.Text = gt.Name;
                grid.Children.Add(txt);
                Grid.SetRow(txt, grid.RowDefinitions.Count - 1);
                Grid.SetColumn(txt, 1);
            }

            return grid;
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
            Values.Clear();
            var grid = uiElement as Grid;
            for (int i = 0; i < ApplicationData.Instance.GeocacheContainers.Count; i++)
            {
                var cb = grid.Children[2*i] as CheckBox;
                if (cb.IsChecked==true)
                {
                    Values.Add(ApplicationData.Instance.GeocacheContainers[i].Name);
                }
            }
            }
        public override bool PrepareRun(Database.DBCon db, string tableName)
        {
            _value = "";
            if (Values.Count > 0)
            {
                _value = string.Format("'{0}'", string.Join("','", Values.ToArray()));
            }
            return base.PrepareRun(db, tableName);
        }

        public override void Process(Operator op, string inputTableName, string targetTableName)
        {
            if (op == Operator.Equal)
            {
                if (!string.IsNullOrEmpty(_value))
                {
                    SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("Container in ({0})", _value));
                }
            }
            else if (op == Operator.NotEqual)
            {
                if (!string.IsNullOrEmpty(_value))
                {
                    SelectGeocachesOnWhereClause(inputTableName, targetTableName, string.Format("Container not in ({0})", _value));
                }
                else
                {
                    SelectGeocachesOnWhereClause(inputTableName, targetTableName, "1=1");
                }
            }
        }
    }
}
