using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linear_Engine
{
    public class CellExtent
    {
        public double MinX { get; set; }
        public double MinY { get; set; }
        public double MaxX { get; set; }
        public double MaxY { get; set; }
        public double CellWidth { get; set; }
        public double CellHeight { get; set; }
        public double CellArea { get; set; }

        public async void CellProperties()
        {
            if (MinX != null && MinY != null)
            {
                CellWidth = MaxX - MinX;
                CellHeight = MaxY - MinY;
                CellArea = CellWidth * CellHeight;
            }
        }
    }
}
