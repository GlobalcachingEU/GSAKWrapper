using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Database
{
    public class DBConSqlite : DBCon
    {
        public DBConSqlite(string filename)
        {
            Connection = new SQLiteConnection(string.Format("data source={0}", filename));
            Connection.Open();
            BindFunction(Connection as SQLiteConnection, new RegExSQLiteFunction());
            BindFunction(Connection as SQLiteConnection, new DistSQLiteFunction());
        }

        public override bool ColumnExists(string tableName, string columnName)
        {
            bool result = false;
            try
            {
                DbDataReader dr = ExecuteReader(string.Format("pragma table_info({0})", tableName));
                while (dr.Read())
                {
                    if (dr[1] is string)
                    {
                        if ((string)dr[1] == columnName)
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }
            catch
            {
            }
            return result;
        }

        public override bool TableExists(string tableName)
        {
            bool result = false;
            try
            {
                object o = ExecuteScalar(string.Format("SELECT name FROM sqlite_master WHERE type='table' AND name='{0}'", tableName));
                if (o == null || o.GetType() == typeof(DBNull))
                {
                    result = false;
                }
                else
                {
                    result = true;
                }
            }
            catch
            {
            }
            return result;
        }

        [SQLiteFunction(Name = "REGEXP", Arguments = 2, FuncType = FunctionType.Scalar)]
        public class RegExSQLiteFunction : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                return System.Text.RegularExpressions.Regex.IsMatch(Convert.ToString(args[1]), Convert.ToString(args[0]));
            }
        }

        [SQLiteFunction(Name = "DIST", Arguments = 4, FuncType = FunctionType.Scalar)]
        public class DistSQLiteFunction : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                var slat = args[0] as string;
                var slon = args[1] as string;
                if (!string.IsNullOrEmpty(slat) && !string.IsNullOrEmpty(slon))
                {
                    return Utils.Calculus.CalculateDistance(Utils.Conversion.StringToDouble(slat), Utils.Conversion.StringToDouble(slon), Convert.ToDouble(args[2]), Convert.ToDouble(args[3])).EllipsoidalDistance / 1000.0; ;
                }
                return false;
            }
        }

        public static void BindFunction(SQLiteConnection connection, SQLiteFunction function)
        {
            var attributes = function.GetType().GetCustomAttributes(typeof(SQLiteFunctionAttribute), true).Cast<SQLiteFunctionAttribute>().ToArray();
            if (attributes.Length == 0)
            {
                throw new InvalidOperationException("SQLiteFunction doesn't have SQLiteFunctionAttribute");
            }
            connection.BindFunction(attributes[0], function);
        }
    }
}
