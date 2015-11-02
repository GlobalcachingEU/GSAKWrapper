using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.DataTypes
{
    public class LogType
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool AsFound { get; set; }

        public LogType()
        {
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
