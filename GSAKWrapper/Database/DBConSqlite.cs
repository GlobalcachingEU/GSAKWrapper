using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Globalization;
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
            BindFunction(Connection as SQLiteConnection, new InAreaSQLiteFunction());
            BindFunction(Connection as SQLiteConnection, new AreaNameSQLiteFunction());
            BindFunction(Connection as SQLiteConnection, new GSAKNoCaseCollation());
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

        [SQLiteFunction(Name = "AREANAME", Arguments = 4, FuncType = FunctionType.Scalar)]
        public class AreaNameSQLiteFunction : SQLiteFunction
        {
            public override object Invoke(object[] args)
            {
                var slevel = args[2] as string;
                var sprefix = args[3] as string;
                if (args[0] != null && args[1] != null)
                {
                    var dlat = Convert.ToDouble(args[0], CultureInfo.InvariantCulture);
                    var dlon = Convert.ToDouble(args[1], CultureInfo.InvariantCulture);
                    var area = Shapefiles.Manager.Instance.GetAreaNameOfLocation(dlat, dlon, (Shapefiles.AreaType)Enum.Parse(typeof(Shapefiles.AreaType), slevel), sprefix);
                    if (!string.IsNullOrEmpty(area))
                    {
                        return area;
                    }
                    else
                    {
                        return "";
                    }
                }
                return "";
            }
        }

        [SQLiteFunction(Name = "INAREA", Arguments = 4, FuncType = FunctionType.Scalar)]
        public class InAreaSQLiteFunction : SQLiteFunction
        {
            private Hashtable _bufferedGetAreasByName = new Hashtable();
            public override object Invoke(object[] args)
            {
                var slevel = args[2] as string;
                var sname = args[3] as string;
                if (args[0] != null && args[1]!=null)
                {
                    var dlat = Convert.ToDouble(args[0], CultureInfo.InvariantCulture);
                    var dlon = Convert.ToDouble(args[1], CultureInfo.InvariantCulture);
                    List<Shapefiles.AreaInfo> areas = _bufferedGetAreasByName[sname] as List<Shapefiles.AreaInfo>;
                    if (areas == null)
                    {
                        areas = Shapefiles.Manager.Instance.GetAreasByName(sname, (Shapefiles.AreaType)Enum.Parse(typeof(Shapefiles.AreaType), slevel));
                        _bufferedGetAreasByName.Add(sname, areas);
                    }
                    foreach (var a in areas)
                    {
                        if (dlat >= a.MinLat && dlon >= a.MinLon && dlat <= a.MaxLat && dlon <= a.MaxLon)
                        {
                            Shapefiles.Manager.Instance.GetPolygonOfArea(a);
                            foreach (var p in a.Polygons)
                            {
                                if (dlat >= p.MinLat && dlon >= p.MinLon && dlat <= p.MaxLat && dlon <= p.MaxLon)
                                {
                                    if (Utils.Calculus.PointInPolygon(p, dlat, dlon))
                                    {
                                        return 1;
                                    }
                                }
                            }
                        }
                    }
                }
                return 0;
            }
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
            private static double?[] d = new double?[] { null, null, null, null };
            public override object Invoke(object[] args)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (args[i] != null)
                    {
                        d[i] = Convert.ToDouble(args[i], CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        d[i] = null;
                    }
                }
                if (d[0] != null && d[1] != null && d[2] != null && d[3] != null)
                {
                    return Utils.Calculus.CalculateDistance((double)d[0], (double)d[1], (double)d[2], (double)d[3]).EllipsoidalDistance / 1000.0;
                }
                return null;
            }
        }

        [SQLiteFunction(Name = "gsaknocase", Arguments = 2, FuncType = FunctionType.Collation)]
        public class GSAKNoCaseCollation : SQLiteFunction
        {
            public override int Compare(string x, string y)
            {
                return string.Compare(x, y, CultureInfo.InvariantCulture, CompareOptions.IgnoreCase);
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
