using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.DataTypes
{
    public class GSAKWaypoints
    {
        public string cParent { get; set; }
        public string cCode { get; set; }
        public string cPrefix { get; set; }
        public string cName { get; set; }
        public string cType { get; set; }
        public string cLat { get; set; }
        public string cLon { get; set; }
        public int cByuser { get; set; }
        public DateTime cDate { get; set; }
        public int cFlag { get; set; }
        public int sB1 { get; set; }
    }
}
