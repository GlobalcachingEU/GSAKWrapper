using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Excel
{
    public class GeocacheData
    {
        public DataTypes.GSAKCaches Caches { get; set; }
        public DataTypes.GSAKCacheMemo CacheMemo { get; set; }
        public DataTypes.GSAKCorrected Corrected { get; set; }
    }
}
