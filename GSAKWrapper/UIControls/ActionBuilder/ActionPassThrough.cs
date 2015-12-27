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
    public class ActionPassThrough : ActionImplementationCondition
    {
        public const string STR_NAME = "PassThrough";

        public ActionPassThrough()
            : base(STR_NAME)
        {
        }

        public override SearchType SearchTypeTarget { get { return SearchType.Extra; } }

        public override ActionImplementation.Operator AllowOperators
        {
            get
            {
                return ActionImplementation.Operator.Equal;
            }
        }

        public override void Process(Operator op, string inputTableName, string targetTableName)
        {
            DatabaseConnection.ExecuteNonQuery(string.Format("insert or ignore into {0} select distinct gccode from {1}", targetTableName, inputTableName));
        }

    }
}
