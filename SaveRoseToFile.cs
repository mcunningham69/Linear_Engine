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
        public static async void SaveRoseAsCSV(string outputFilename, List<FlapParameter> parameters, bool bCSV, string metadata)
        {
            char delimiter = '\t';

            if (bCSV)
                delimiter = ',';

            //Create output file

            using StreamWriter sw = new(outputFilename, append: true);
            await sw.WriteLineAsync(metadata);


            //Metadata. Rose creation date and time. Rose filename and type
            /*
             * Rose diagram Created:
             * Input data: 
             * Rose type:
             * Rose geometry:
             * 
             */
            string header = "PetalID, OriginX, OriginY, X1, Y1, X2, Y2, Count";

            await sw.WriteLineAsync(header);

            foreach (FlapParameter parameter in parameters)
            {
                foreach (RosePetalCoordinates coordinates in parameter.Rose.petals)
                {
                    string line = coordinates.PetalID.ToString() + ", " +
                    coordinates.OriginX.ToString() + ", " +
                    coordinates.OriginY.ToString() + ", " +
                    coordinates.x1.ToString() + ", " +
                    coordinates.y1.ToString() + ", " +
                    coordinates.x2.ToString() + ", " +
                    coordinates.y2.ToString();

                    await sw.WriteLineAsync(line);
                }

            }

        }


    }
}
