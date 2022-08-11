using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linear_Engine
{
    public class LinearValues
    {
        public double startX { get; set; }
        public double startY { get; set; }
        public double endX { get; set; }
        public double endY { get; set; }
        public double Direction { get; set; }
        public double LineLength { get; set; }
        public Int16 LookupAttribute { get; set; }
        public int oid { get; set; }
        public RoseGeom mGeom { get; set; }

        public LinearValues()
        {
            startX = 0.0;
            startY = 0.0;
            endX = 0.0;
            endY = 0.0;
            oid = 0;
           // Direction = CalcOrient();
         //   LineLength = CalcLength();
        

        }

        public LinearValues(int _oid)
        {
            startX = 0.0;
            startY = 0.0;
            endX = 0.0;
            endY = 0.0;
            oid = _oid;
         //   LineAzimuth = CalcOrient();
         //   LineLength = CalcLength();

        }

        //return orientation between two points
        private double CalcOrient()
        {

            double dblStartX = startX;
            double dblStartY = startY;

            double dblEndX = endX;
            double dblEndY = endY;

            double dblDx = dblEndX - dblStartX;
            double dblDy = dblEndY - dblStartY;

            if (dblDy == 0.0)
            {
                return 90.0;
  
            }

            double dblOrient = dblDx / dblDy;

            //get the inverse tangent
            double dblInverse = Math.Atan(dblOrient);

            //as Atan returns radians, convert to degress
            double dblDeg = dblInverse * (180 / Math.PI);

            if (dblDeg < 0)
                return dblDeg + 180;
            else
                return dblDeg;
        }
   
        //return length between two points
        private double CalcLength()
        {
            double dblStartX = startX;
            double dblStartY = startY;

            double dblEndX = endX;
            double dblEndY = endY;

            double dblDx = dblEndX - dblStartX;
            double dblDy = dblEndY - dblStartY;

            return Math.Sqrt( Math.Pow(dblDx,2) + Math.Pow(dblDy,2));

        }

    }

    public class PointValues
    {
        public double startX { get; set; }
        public double startY { get; set; }

        public double PointOrient { get; set; }
        public int oid { get; set; }

        public PointValues()
        {
            startX = 0.0;
            startY = 0.0;
            oid = 0;
            PointOrient = 0.0;

        }

        public PointValues(double dblOrient)
        {
            startX = 0.0;
            startY = 0.0;
            oid = 0;
            PointOrient = dblOrient;

        }

        public PointValues(double dblOrient, int _oid)
        {
            startX = 0.0;
            startY = 0.0;
            oid = _oid;
            PointOrient = dblOrient;

        }

    }
}
