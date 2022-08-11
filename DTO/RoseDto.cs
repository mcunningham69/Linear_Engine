using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linear_Engine.DTO
{
    public class RoseDto
    {
        public string tablePath { get; set; }
        public string tableName { get; set; }
        public ImportTableFormat tableFormat { get; set; }
        public bool tableIsValid { get; set; }
        public List<string> fields { get; set; }
        public ImportTableFields tableFields { get; set; }
        public string roseKey { get; set; }
        public RoseType roseType { get; set; }
        public RoseGeom geomType { get; set; }

    }
}
