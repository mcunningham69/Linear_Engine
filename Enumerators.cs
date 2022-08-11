using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linear_Engine
{

    public enum RasterLineamentAnalysis
    {
        DensityLength,
        DensityFrequency,
        GroupMeansFrequency,
        GroupMeansLength,
        RelativeEntropy,
        GroupDominanceLength,
        GroupDominanceFrequency
    }

    public enum RoseLineamentAnalysis
    {
        RoseCells,
        RoseRegional,
        RoseRegionalPoint,
        Fishnet
    }

    public enum ImportTableFormat
    {
        excel_table,
        egdb_table,
        fgdb_table,
        pgdb_table,
        text_csv,
        text_txt,
        other
    }
}
