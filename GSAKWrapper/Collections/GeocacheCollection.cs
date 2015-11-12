using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.Collections
{
    [NPoco.TableName("GeocacheCollection")]
    public class GeocacheCollection
    {
        public int CollectionID { get; set; }
        public string Name { get; set; }
    }
}
