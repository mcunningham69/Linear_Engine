using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Linear_Engine.DTO;

namespace Linear_Engine
{
    public static class SaveRoseToFile
    {
        public static async void SaveRoseAsCSV(string outputFilename, List<FlapParameter> parameters, bool bCSV, string metadata, bool bStatistics, bool bLength)
        {
            char delimiter = '\t';

            if (bCSV)
                delimiter = ',';

            //Create output file

            using StreamWriter sw = new(outputFilename, append: true);
            await sw.WriteLineAsync(metadata);

            string header = "index, RoseID, PetalID, OriginX, OriginY, X1, Y1, X2, Y2";

            await sw.WriteLineAsync(header);

            var createCells = parameters.Where(a => a.CreateCell).Select(b => b).ToList();
            int counter = 0;
            int index = 0;

            foreach (FlapParameter parameter in createCells)
            {
                counter = 0;
                foreach (RosePetalCoordinates coordinates in parameter.Rose.petals)
                {
                    string line = index.ToString() + ", " +
                    parameter.CellID.ToString() + ", " +
                    coordinates.PetalID.ToString() + ", " +
                    coordinates.OriginX.ToString() + ", " +
                    coordinates.OriginY.ToString() + ", " +
                    coordinates.x1.ToString() + ", " +
                    coordinates.y1.ToString() + ", " +
                    coordinates.x2.ToString() + ", " +
                    coordinates.y2.ToString();

                    if (bStatistics)
                    {
                        line = line + ", " + parameter.Rose.BinCount[counter].ToString() + ", " +
                        parameter.Rose.AvgAzi[counter].ToString() + ", " +
                        parameter.Rose.StdAzi[counter].ToString();

                        if (bLength)
                        {
                            line = line + ", " + parameter.Rose.AvgLength[counter] + ", " +
                            parameter.Rose.StdLength[counter].ToString();
                        }
                    }

                    counter++;
                    index++;

                    await sw.WriteLineAsync(line);

                }

                counter++;

            }

        }

        public static async void SaveFishnetAsCSV(string outputFilename, List<FlapParameter> parameters, bool bCSV, bool bLength)
        {
            char delimiter = '\t';

            if (bCSV)
                delimiter = ',';

            //Create output file
            int length = outputFilename.Count() - 4;

            string fishExtension = outputFilename.Substring(length, 4);
            string origname = outputFilename.Substring(0, length);
            string fishname = origname + "_fish" + fishExtension;

            using StreamWriter sw = new(fishname, append: true);

            bool bLine = false;
            int index = 0;

            string header = "index, RoseID, X1, Y1, X2, Y2, X3, Y3, X4, Y4, Count, Average DIR, Std Dev. DIR";

            if (bLength)
                header = header + ", Average LEN, Std Dev. LEN";


            await sw.WriteLineAsync(header);

            var createCells = parameters.Where(a => a.CreateCell).Select(b => b).ToList();

            foreach (FlapParameter parameter in createCells)
            {
                string line = index.ToString() + ", " +
                parameter.CellID.ToString() + ", " +
                parameter.RoseExtent.MinX.ToString() + ", " +
                parameter.RoseExtent.MinY.ToString() + ", " +
                parameter.RoseExtent.MinX.ToString() + ", " +
                parameter.RoseExtent.MaxY.ToString() + ", " +
                parameter.RoseExtent.MaxX.ToString() + ", " +
                parameter.RoseExtent.MaxY.ToString() + ", " +
                parameter.RoseExtent.MaxX.ToString() + ", " +
                parameter.RoseExtent.MinY.ToString() + ", " +
                parameter.FishStats.Count.ToString() + ", " +
                parameter.FishStats.AziAvg.ToString() + ", " +
                parameter.FishStats.AziStd.ToString();

                if (bLength)
                {
                    line = line + ", " + parameter.FishStats.LenAvg + ", " +
                         parameter.FishStats.LenStd.ToString();
                }

                index++;
                await sw.WriteLineAsync(line);

            }

        }

        public static async void SaveRoseStatistics(string outputFilename, List<FlapParameter> parameters, bool bCSV, bool bLength)
        {
            char delimiter = '\t';

            if (bCSV)
                delimiter = ',';

            //Create output file
            int length = outputFilename.Count() - 4;

            string origExtension = outputFilename.Substring(length, 4);
            string origname = outputFilename.Substring(0, length);
            string statsname = origname + "_stats" + origExtension;

            using StreamWriter sw = new(statsname, append: true);

            bool bLine = false;

            string header = "RoseID, Bin, Count, Average DIR, Std Dev. DIR";

            if (bLength)
                header = header + ", Average LEN, Std Dev. LEN";


            await sw.WriteLineAsync(header);

            int counter = 0;

            var createCells = parameters.Where(a => a.CreateCell).Select(b => b).ToList();

            foreach (FlapParameter parameter in createCells)
            {
                counter = 0;

                foreach (RosePetalCoordinates coordinates in parameter.Rose.petals)
                {
                    string line = parameter.CellID.ToString() + ", " +
                    parameter.Rose.BinCount[counter].ToString() + ", " +
                    parameter.Rose.AvgAzi[counter].ToString() + ", " +
                    parameter.Rose.StdAzi[counter].ToString();

                    if (bLength)
                    {
                        line = line + ", " + parameter.Rose.AvgLength[counter] + ", " +
                                 parameter.Rose.StdLength[counter].ToString();
                    }

                    counter++;

                    await sw.WriteLineAsync(line);

                }

            }

        }

    }
}
