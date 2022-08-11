using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linear_Engine
{
    public class FishnetStatistics
    {
        public double AziMin { get; set; }
        public double AziMax { get; set; }
        public double AziAvg { get; set; }
        public double AziStd { get; set; }
        public double Count { get; set; }

        public double LenMin { get; set; }
        public double LenMax { get; set; }
        public double LenAvg { get; set; }
        public double LenStd { get; set; }

        public double TotalLength { get; set; }

        public List<double> RoseAzimuth { get; set; }
        public List<double> RoseLength { get; set; }

        public FishnetStatistics()
        {
            RoseAzimuth = new List<double>();
            RoseLength = new List<double>();
        }

        public bool CalculateFishnetStatistics(List<FreqLen> freqLength)
        {

            Count = freqLength.Count;
            AziAvg = freqLength.Select(a => a.Azimuth).Average();
            AziMin = freqLength.Select(a => a.Azimuth).Min();
            AziMax = freqLength.Select(a => a.Azimuth).Max();

            var queryAzi = freqLength.Select(a => a.Azimuth);

            double sumOfDerivation = 0;
            foreach (double azi in queryAzi)
            {
                sumOfDerivation += (azi) * (azi);
            }

            double sumOfDerivationAverage = sumOfDerivation / Count;
            AziStd = Math.Sqrt(sumOfDerivationAverage - (AziAvg * AziAvg));

            LenMin = freqLength.Select(a => a.Length).Min();
            LenMax = freqLength.Select(a => a.Length).Max();
            LenAvg = freqLength.Select(a => a.Length).Average();
            TotalLength = freqLength.Select(a => a.Length).Sum();

            var queryLen = freqLength.Select(a => a.Length);

            sumOfDerivation = 0;
            foreach (double len in queryLen)
            {
                sumOfDerivation += (len) * (len);
            }
            sumOfDerivationAverage = sumOfDerivation / Count;
            LenStd = Math.Sqrt(sumOfDerivationAverage - (LenAvg * LenAvg));

            return true;

        }
    }
}
