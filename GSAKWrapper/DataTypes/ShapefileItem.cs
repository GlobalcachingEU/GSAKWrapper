using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.DataTypes
{
    public class ShapefileItem
    {
        public string FileName { get; set; }
        public string TableName { get; set; }
        public string CoordType { get; set; }
        public string AreaType { get; set; }
        public string NamePrefix { get; set; }
        public string Encoding { get; set; }
        public int Enabled { get; set; }
    }
}
