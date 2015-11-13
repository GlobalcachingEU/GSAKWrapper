using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSAKWrapper.DataTypes
{
    [NPoco.TableName("GeocacheCollectionItem")]
    public class GeocacheCollectionItem
    {
        public int CollectionID { get; set; }
        public string GeocacheCode { get; set; }
        public string Name { get; set; }
    }
}
