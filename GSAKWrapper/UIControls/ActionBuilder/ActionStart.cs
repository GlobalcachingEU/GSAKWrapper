using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.IO;
using System.Text.RegularExpressions;

namespace GSAKWrapper.UIControls.ActionBuilder
{
    public class ActionStart : ActionImplementationCondition
    {
        public const string STR_NAME = "Start";
        public ActionStart()
            : base(STR_NAME)
        {
        }
        public override bool AllowEntryPoint
        {
            get { return false; }
        }
        public override Operator AllowOperators
        {
            get { return Operator.Equal; }
        }
        public override void Process(Operator op, string inputTableName, string targetTableName)
        {
            DatabaseConnection.ExecuteNonQuery(string.Format("insert into {0} select distinct Code as gccode from Caches", targetTableName));
        }
        public override bool PrepareRun(Database.DBCon db, string tableName)
        {
            base.PrepareRun(db, tableName);
            CreateTableInDatabase(TempTableName);
            CreateTableInDatabase(TempTableName2);
            return true;
        }
    }

}
