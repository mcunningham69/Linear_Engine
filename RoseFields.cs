using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linear_Engine
{
    public class RoseFields
    {
        public int fieldID { get; set; }
        public string FieldName { get; set; }
        public string FieldAlias { get; set; }
        public string FieldType { get; set; }
        public int FieldLength { get; set; }
        public int FieldPrecision { get; set; }
    }

    public class RasterFields : RoseFields
    { }

    public class MovingStatsFields : RoseFields
    { }

}
