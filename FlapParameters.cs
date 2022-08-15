using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linear_Engine
{
    public class FlapParameters
    {
        public RoseType _RoseType { get; set; }
        public RoseGeom _RoseGeom { get; set; }
        public int SubCellsize { get; set; }
        public int NoOfColumns { get; set; }
        public int NoOfRows { get; set; }
        public int Interval { get; set; }
        public bool SelectedFeatures { get; set; }
        public int RangeFrom { get; set; }
        public int RangeTo { get; set; }
        public string FieldNameForAzi { get; set; }
        public int XBlocks { get; set; }
        public int YBlocks { get; set; }
        public int TotalBlocks { get; set; }
        public double SearchWindow { get; set; }
        public bool Bearing { get; set; }
        public bool SaveFishnet { get; set; }
        public bool SaveStatistics { get; set; }

        public List<FlapParameter> flapParameters { get; set; }

        public FlapParameters()
        {

        }

        public void SetProperties()
        {
            double dblColumns = 0.0; //divide by subcellsize 
            double dblRows = 0.0;

            NoOfColumns = Convert.ToInt16(Math.Floor(dblColumns) + 1);
            NoOfRows = Convert.ToInt16(Math.Floor(dblRows) + 1);
        }

        public int CalculateTotalBlocks(int _XBlocks, int _YBlocks)
        {
            XBlocks = _XBlocks;
            YBlocks = _YBlocks;

            if (_XBlocks > 0 && _YBlocks > 0)
                TotalBlocks = ((_XBlocks * 2) + 1) * ((_YBlocks * 2) + 1);
            else if (_XBlocks == 0)
                TotalBlocks = (_YBlocks * 2) + 1;
            else
                TotalBlocks = (_XBlocks * 2) + 1;

            return TotalBlocks;
        }
    }

    public class FlapParameter
    {
        public int CellID { get; set; }
        public List<double> ArrayValues { get; set; }
        public int Count { get; set; }
        public double SearchCount { get; set; }
        public List<FreqLen> LenAzi { get; set; } //raw input from lines
        public List<BinRangeClass> BinRange { get; set; }
        public double GridValue { get; set; }
        public double MeanValue { get; set; }
        public double MinValue { get; set; } //Min
        public double MaxValue { get; set; }
        public double SumValue { get; set; }
        public double StdValue { get; set; }
        public double AdjustedValue { get; set; } //adjusted gridvalue
        public CellExtent RoseExtent { get; set; }
        //public double ExtentHeight { get; set; }
        //public double ExtentWidth { get; set; }
        //public double ExtentArea { get; set; }
        //public double CentreX { get; set; }
        //public double CentreY { get; set; }
        //public double XMin { get; set; }
        //public double XMax { get; set; }
        //public double YMin { get; set; }
        //public double YMax { get; set; }
        public bool CreateCell { get; set; }
        public FishnetStatistics FishStats { get; set; }
        public RoseDiagramParameters Rose { get; set; }

        public FlapParameter()
        {
            CellID = 0;
            ArrayValues = new List<double>();
            Count = 0;
            LenAzi = new List<FreqLen>();
            BinRange = new List<BinRangeClass>();

            GridValue = 0.0;
            MinValue = 0.0;
            MaxValue = 0.0;
            SumValue = 0.0;
            StdValue = 0.0;

            this.CreateCell = true;
        }

        public void SetProperties()
        {

        }

        public void CalcArrayValues(bool bDirection, int NoOfElements, RoseType _roseType, int binSize)
        {
            int counter = NoOfElements;

            //counter = counter * 2;

            for (int i = 0; i <= counter - 1; i++)
            {

                this.ArrayValues.Add(0);

                this.ArrayValues.Add(0);
            }

            double dblElement = 0.0;

            for (int i = 0; i < this.LenAzi.Count; i++)
            {
                int nElement = 0;

                dblElement = LenAzi[i].Azimuth / Convert.ToDouble(binSize);

                nElement = (int)Math.Floor(dblElement);

                if (_roseType == RoseType.Length)
                {
                    ArrayValues[nElement] = ArrayValues[nElement] + LenAzi[i].Length;
                }
                else
                {
                    ArrayValues[nElement] = ArrayValues[nElement] + 1;
                }

            }

            /*
            //Run second iteration for symmetry if not point
            if (!bDirection)
            {
                int nIncrement = NoOfElements;

                for (int i = 0; i <= NoOfElements - 1; i++)
                {
                    FreqArray[nIncrement] = FreqArray[i];

                    nIncrement++; ;
                }

            }
            */
        }

    }
}
