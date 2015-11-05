using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.DataTypes
{
    public class GSAKLogs
    {
        public string lParent { get; set; }
        public int lLogId { get; set; }
        public string lType { get; set; }
        public string lBy { get; set; }
        public DateTime? lDate { get; set; }
        public string lLat { get; set; }
        public string lLon { get; set; }
        public int? lEncoded { get; set; }
        public int? lownerid { get; set; }
        public int? lHasHtml { get; set; }
        public string lTime { get; set; }
    }
}
