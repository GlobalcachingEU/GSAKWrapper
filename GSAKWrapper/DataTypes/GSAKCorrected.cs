using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.DataTypes
{
    public class GSAKCorrected
    {
        public string kCode { get; set; }
        public string kBeforeLat { get; set; }
        public string kBeforeLon { get; set; }
        public string kBeforeState { get; set; }
        public string kBeforeCounty { get; set; }
        public string kAfterLat { get; set; }
        public string kAfterLon { get; set; }
        public string kAfterState { get; set; }
        public string kAftercounty { get; set; }
        public string kType { get; set; }
    }
}
