using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;


namespace Linear_Engine
{
    public enum RoseType
    {
        Frequency,
        Length,
        Other
    }

    public enum RoseGeom
    {
        Line,
        Point,
        CellOnly,
        Other
    }

    public class RoseDiagramParameters
    {
        #region Properties
        public int CellID { get; set; }
        public List<double> RoseArray { get; set; }
        public double[,] RoseArrayBin { get; set; }
        public List<double> MinAzi { get; set; }
        public List<double> MaxAzi { get; set; }
        public List<double> AvgAzi { get; set; }
        public List<double> StdAzi { get; set; }
        public List<int> BinCount { get; set; }
        public List<double> MinLength { get; set; }
        public List<double> MaxLength { get; set; }
        public List<double> AvgLength { get; set; }
        public List<double> StdLength { get; set; }
        public List<double> RoseAzimuth { get; set; } //raw input from lines
        public List<double> RoseLength { get; set; }  //raw input from lines
        public List<BinRangeClass> BinRange { get; set; }
        public List<RosePetalCoordinates> petals { get; set; }
        #endregion

        public RoseDiagramParameters()
        {
            CellID = 0;
            MinAzi = new List<double>();
            MaxAzi = new List<double>(); ;
            AvgAzi = new List<double>();
            BinCount = new List<int>();
            MinLength = new List<double>();
            MaxLength = new List<double>(); ;
            AvgLength = new List<double>();
            RoseAzimuth = new List<double>(); //captures all values from input for statistics
            RoseLength = new List<double>(); //captures all values from input for statistics
            BinCount = new List<int>();
            BinRange = new List<BinRangeClass>();
            StdAzi = new List<double>();
            StdLength = new List<double>();
            RoseArray = new List<double>();
        }

        public bool CalculateRoseStatistics(List<FreqLen> freqLength, int binSize)
        {
            int nElements = 180 / binSize;
            int lowerRange = 0;
            int upperRange = binSize;

            int count = nElements * 2;

            //Create temp list
            List<FreqLen> tempList = new List<FreqLen>();
            foreach (FreqLen _value in freqLength)
            {
                tempList.Add(new FreqLen { Azimuth = _value.Azimuth + 180, Length = _value.Length });
            }

            freqLength = freqLength.Concat(tempList).ToList();


            for (int p = 0; p < count; p++)
            {
                int Count = 0;
                var query = freqLength.Where(i => i.Azimuth > lowerRange).Where(b => b.Azimuth <= upperRange);

                if (query.Count() == 0)
                {
                    this.BinRange.Add(new BinRangeClass { LowerRange = lowerRange, UpperRange = upperRange });

                    Count = 0;
                    MaxAzi.Add(-1);
                    MinAzi.Add(-1);
                    AvgAzi.Add(-1);
                    BinCount.Add(-1);
                    MaxLength.Add(-1);
                    MinLength.Add(-1);
                    AvgLength.Add(-1);
                    StdAzi.Add(-1);
                    StdLength.Add(-1);
                }
                else
                {
                    this.BinRange.Add(new BinRangeClass { LowerRange = lowerRange, UpperRange = upperRange });
                    this.MaxAzi.Add(freqLength.Where(i => i.Azimuth > lowerRange).Where(b => b.Azimuth <= upperRange).Max(a => a.Azimuth));
                    this.MinAzi.Add(freqLength.Where(i => i.Azimuth > lowerRange).Where(b => b.Azimuth <= upperRange).Min(a => a.Azimuth));
                    double AziAvg = freqLength.Where(i => i.Azimuth > lowerRange).Where(b => b.Azimuth <= upperRange).Average(a => a.Azimuth);
                    this.AvgAzi.Add(AziAvg);

                    Count = freqLength.Where(i => i.Azimuth > lowerRange).Where(b => b.Azimuth <= upperRange).Count();
                    this.BinCount.Add(Count);

                    this.MaxLength.Add(freqLength.Where(i => i.Azimuth > lowerRange).Where(b => b.Azimuth <= upperRange).Max(a => a.Length));
                    this.MinLength.Add(freqLength.Where(i => i.Azimuth > lowerRange).Where(b => b.Azimuth <= upperRange).Min(a => a.Length));

                    double LenAvg = freqLength.Where(i => i.Azimuth > lowerRange).Where(b => b.Azimuth <= upperRange).Average(a => a.Length);
                    this.AvgLength.Add(LenAvg);

                    double sumOfDerivation = 0;
                    foreach (FreqLen _freq in query)
                    {

                        sumOfDerivation += (_freq.Azimuth) * (_freq.Azimuth);

                    }
                    double sumOfDerivationAverage = sumOfDerivation / Count;
                    this.StdAzi.Add(Math.Sqrt(sumOfDerivationAverage - (AziAvg * AziAvg)));

                    sumOfDerivation = 0;
                    foreach (FreqLen _freq in query)
                    {

                        sumOfDerivation += (_freq.Length) * (_freq.Length);

                    }
                    sumOfDerivationAverage = sumOfDerivation / Count;
                    this.StdLength.Add(Math.Sqrt(sumOfDerivationAverage - (LenAvg * LenAvg)));

                }

                lowerRange = upperRange;
                upperRange = upperRange + binSize;
            }

            return true;
        }

        public void RoseArrayValues(bool bDirection, int NoOfElements, int binSize, List<FreqLen> freqLen, RoseType roseType)
        {
            int counter = NoOfElements;

            counter = counter * 2;

            for (int i = 0; i <= counter - 1; i++)
            {
                this.RoseArray.Add(0);
            }

            double dblElement = 0.0;

            for (int i = 0; i < freqLen.Count; i++)
            {
                int nElement = 0;

                dblElement = freqLen[i].Azimuth / Convert.ToDouble(binSize);

                nElement = (int)Math.Floor(dblElement);

                if (roseType == RoseType.Length)
                {
                    RoseArray[nElement] = RoseArray[nElement] + freqLen[i].Length;
                }
                else
                {
                    RoseArray[nElement] = RoseArray[nElement] + 1;
                }

            }

            if (!bDirection)
            {
                int nIncrement = NoOfElements;

                for (int i = 0; i <= NoOfElements - 1; i++)
                {
                    RoseArray[nIncrement] = RoseArray[i];

                    nIncrement++; ;
                }
            }

        }

        public void CalculateRosePetals(int nInterval, double ExtentWidth, double ExtentHeight)
        {
            int j2 = RoseArray.Count;

            //NEED Extent from envelope or local if not regional rose

            //scale roseArray
            double dblScale = ScalePlots(RoseArray, ExtentWidth, ExtentHeight);
            double ScaleFactor = dblScale;

            for (int i = 0; i < RoseArray.Count; i++)
            {
                RoseArray[i] = (RoseArray[i] * dblScale) * 0.8;
            }

            int start = 180 / nInterval;
            RoseArrayBin = new double[j2, 4];

            for (int j = 0; j != j2; j++)
            {
                //Create rose diagram bins
                int alphaDeg = j * nInterval;
                int betaDeg = (j + 1) * nInterval;

                double dblAlphaRad = (double)alphaDeg * (Math.PI / 180);  //Convert to radians
                double dblBetaRad = (double)betaDeg * (Math.PI / 180);

                RoseArrayBin[j, 0] = Math.Sin(dblAlphaRad) * RoseArray[j];  //x2
                RoseArrayBin[j, 1] = Math.Cos(dblAlphaRad) * RoseArray[j];  //y2
                RoseArrayBin[j, 2] = Math.Sin(dblBetaRad) * RoseArray[j];  //x3
                RoseArrayBin[j, 3] = Math.Cos(dblBetaRad) * RoseArray[j];  //y3
            }
        }



        private double ScalePlots(List<double> RoseArray, double ExtentWidth, double ExtentHeight)
        {
            double dblLength = RoseArray.Max(); //greatest length

            double dblWidth = ExtentWidth / 2;
            double dblHeight = ExtentHeight / 2;

            double cellSize = dblWidth;

            if (cellSize < dblHeight)
                cellSize = dblHeight;

            return cellSize / dblLength;

        }

        public async Task<string> IntervalErrorChecking(string strInterval)
        {

            if (strInterval == "")
            {
                return "Please enter an integer value";
            }

            if (!Information.IsNumeric(strInterval))
            {
                return "The value must be an integer";
            }

            int interval = Convert.ToInt32(strInterval);

            if (interval <= 4)
            {
                return "The value must be greater than 4";
            }

            if (180 % interval > 0)
            {
                return "The value must be divisible into 180";
            }

            return "";
        }

        public string SubcellErrorChecking(string strInterval)
        {
            if (!Information.IsNumeric(strInterval))
            {
                return "The value must be an integer";
            }

            return "";
        }

    }

    public class RosePetalCoordinates
    {
        public int PetalID { get; set; }
        public double OriginX { get; set; }
        public double OriginY { get; set; }
        public double x1 { get; set; }
        public double y1 { get; set; }
        public double x2 { get; set; }
        public double y2 { get; set; }

    }

    public class BinRangeClass
    {
        public int LowerRange { get; set; }
        public int UpperRange { get; set; }

        public BinRangeClass()
        {
            LowerRange = 0;
            UpperRange = 0;
        }
    }

    public class FreqLen
    {
        public double Azimuth { get; set; }
        public double Length { get; set; }
        int Element { get; set; }
    }
}
