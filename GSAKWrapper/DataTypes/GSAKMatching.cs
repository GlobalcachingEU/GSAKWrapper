using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.DataTypes
{
    public class GSAKMatching
    {
        public string Exact { get; set; }
        public string Regex { get; set; }
        public string Wild { get; set; }
        public string Id { get; set; }
        public string GeoName { get; set; }
    }
}
