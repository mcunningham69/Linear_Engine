using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Linear_Engine
{
    //Helper Class for abstract reference
    public class RoseFactoryPreview
    {
        public TypeOfAnalysis _factoryType;

        public RoseFactoryPreview(RoseLineamentAnalysis value)
        {

            SetAnalysisDataset(value);
        }

        public void SetAnalysisDataset(RoseLineamentAnalysis value)
        {
            _factoryType = TypeOfAnalysis.RoseStatistics(value);
        }


        public async Task<FlapParameters> PrepareInputForProcessing(List<LinearValues> orientValues, CellExtent customEnvelope, int subCellsize,
    int nInterval, bool selectedFeatures, RoseDiagramParameters rose, bool bRegional, string fieldName, RoseType roseType, RoseGeom roseGeom)
        {
            return await _factoryType.PrepareInputForProcessing(orientValues, customEnvelope, subCellsize, nInterval,
                selectedFeatures, rose, bRegional, fieldName, roseType, roseGeom);
        }

        public async Task<bool> CalculateValues(FlapParameters _parameters)
        {
            bool bCheck = true;

            bCheck = await _factoryType.CalculateValues(_parameters);

            return bCheck;
        }

        public async Task<string> SaveFeatures(FlapParameters _parameters)
        {
            string message = "";
            message = await _factoryType.SaveFeatures(_parameters);

            return message;
        }

    }

    //abstract class 
    public abstract class TypeOfAnalysis
    {
        public static TypeOfAnalysis RoseStatistics(RoseLineamentAnalysis value)
        {
            switch (value)
            {
                case RoseLineamentAnalysis.RoseCells:
                    return new RoseCellPlots();

                case RoseLineamentAnalysis.RoseRegional:
                    return new RoseRegionalPlot();

                //case RoseLineamentAnalysis.Fishnet:
                //    return new FishnetCoverage();

                default:
                    throw new Exception("Generic error with rose analysis");
            }
        }


        #region Abstract methods
        public abstract Task<FlapParameters> PrepareInputForProcessing(List<LinearValues> orientValues, CellExtent customEnvelope, int subCellsize,
    int nInterval, bool selectedFeatures, RoseDiagramParameters rose, bool bRegional, string fieldName, RoseType roseType, RoseGeom roseGeom);

        public abstract Task<bool> CalculateValues(FlapParameters _parameters);

        public abstract Task<string> SaveFeatures(FlapParameters _parameters);

        /*
         public abstract Task<FeatureClass> CreateFeatureClass(string suffix, string roseName, bool bExists, string databasePath,
     FeatureLayer InputLayer, bool bStatistics, RoseGeom roseGeom, SpatialReference thisSpatRef);

                 public abstract Task<string> SaveAsVectorFeatures(FeatureClass OuputFeatureClass, SpatialReference mySpatialRef,
             FlapParameters _parameters, bool bExtentOnly);

         */
        #endregion
    }

    public class RoseCellPlots : TypeOfAnalysis
    {
        public override async Task<FlapParameters> PrepareInputForProcessing(List<LinearValues> orientValues, CellExtent customEnvelope, int subCellsize,
    int nInterval, bool selectedFeatures, RoseDiagramParameters rose, bool bRegional, string fieldName, RoseType roseType, RoseGeom roseGeom)
        {
            int cellID = 1;

            //calculate centre of cell
            customEnvelope.CentreOfCell();

            FlapParameters subCellsRose = new FlapParameters
            {
                Interval = nInterval,
                SubCellsize = subCellsize,
                SelectedFeatures = selectedFeatures,
                flapParameters = new List<FlapParameter>(),
                _RoseType = roseType,
                _RoseGeom = roseGeom,
                RangeFrom = 0,
                RangeTo = 0,
                NoOfColumns = (int)Math.Abs(customEnvelope.CellWidth / subCellsize),
                NoOfRows = (int)Math.Abs(customEnvelope.CellHeight / subCellsize)
            };

            double dblMinX = Math.Floor(customEnvelope.MinX);
            double dblMaxY = Math.Floor(customEnvelope.MaxY);
            double dblMaxX = dblMinX + subCellsize;
            double dblMinY = dblMaxY - subCellsize;

            double dblCentreX = dblMinX + ((dblMaxX - dblMinX) / 2);
            double dblCentreY = dblMinY + ((dblMaxY - dblMinY) / 2);

            for (int y = 0; y < subCellsRose.NoOfRows; y++)
            {
                for (int x = 0; x < subCellsRose.NoOfColumns; x++)
                {

                    dblCentreX = dblMinX + ((dblMaxX - dblMinX) / 2);
                    dblCentreY = dblMinY + ((dblMaxY - dblMinY) / 2);

                    subCellsRose.flapParameters.Add(new FlapParameter
                    {
                        RoseExtent = new CellExtent
                        {
                            MinX = dblMinX,
                            MinY = dblMinY,
                            MaxX = dblMaxX,
                            MaxY = dblMaxY,
                            CellWidth = subCellsize,
                            CellHeight = subCellsize,
                            CellArea = subCellsize * subCellsize,
                            CentreX = dblCentreX,
                            CentreY = dblCentreY
                        },
                        CellID = cellID

                    });

                    subCellsRose.flapParameters.Last().CellID = cellID;

                    int featCount = 0;

                    //Query within extent
                    List<LinearValues> subSelValues = await ReturnPointFeautresWithinExtent(subCellsRose.flapParameters.Last().RoseExtent, orientValues);
                    featCount = subSelValues.Count;

                    if (featCount > 0)
                    {
                        subCellsRose.flapParameters.Last().CreateCell = true;
                        subCellsRose.flapParameters.Last().Count = featCount;

                        await PopulateAziLenArray(roseType, subSelValues, subCellsRose.flapParameters.Last());
                    }
                    else
                    {
                        subCellsRose.flapParameters.Last().CreateCell = false;
                    }

                    cellID++;

                    //CHANGE to X
                    dblMinX = dblMinX + subCellsize;
                    dblMaxX = dblMaxX + subCellsize;

                    dblCentreX = dblMinX + ((dblMaxX - dblMinX) / 2);
                    dblCentreY = dblMinY + ((dblMaxY - dblMinY) / 2);
                }

                //Reset after changing Y
                dblMinX = Math.Floor(Math.Floor(customEnvelope.MinX));
                dblMaxX = dblMinX + subCellsize;

                dblMinY = dblMinY - subCellsize;
                dblMaxY = dblMaxY - subCellsize;

                dblCentreX = dblMinX + ((dblMaxX - dblMinX) / 2);
                dblCentreY = dblMinY + ((dblMaxY - dblMinY) / 2);
            }

            return subCellsRose;
        }

        private async Task<string> PopulateAziLenArray(RoseType roseType, List<LinearValues> orientValues, FlapParameter parameter)
        {
            if (roseType == RoseType.Frequency)
            {
                foreach (var val in orientValues)
                {
                    parameter.LenAzi.Add(new FreqLen
                    {
                        Azimuth = val.Direction,
                        Length = 0.0
                    });
                }
            }
            else
            {
                foreach (var val in orientValues)
                {
                    parameter.LenAzi.Add(new FreqLen
                    {
                        Azimuth = val.Direction,
                        Length = val.LineLength
                    });
                }
            }

            return ""; //TODO

        }

        private async Task<List<LinearValues>> ReturnPointFeautresWithinExtent(CellExtent cell, List<LinearValues> orientValues)
        {
            List<LinearValues> cellResults = orientValues.Where(a => a.startX >= cell.MinX && a.startX <= cell.MaxX
           && a.startY <= cell.MaxY && a.startY >= cell.MinY)
   .Select(b => new LinearValues()
   {
        startX = b.startX,
        startY = b.startY,
        Direction = b.Direction
    }).ToList();

            var minx1 = orientValues.Select(a => a.startX).Min();
            var miny1 = orientValues.Select(a => a.startY).Min();
            var maxx1 = orientValues.Select(a => a.startX).Max();
            var maxy1 = orientValues.Select(a => a.startX).Max();

            if (cellResults.Count > 0)
            {
                var minx = cellResults.Select(a => a.startX).Min();
                var miny = cellResults.Select(a => a.startY).Min();
                var maxx = cellResults.Select(a => a.startX).Max();
                var maxy = cellResults.Select(a => a.startY).Max();
            }

            return cellResults;
        }

        public override async Task<bool> CalculateValues(FlapParameters _parameters)
        {
            int NoOfElements = 180 / _parameters.Interval;

            RoseType roseType = _parameters._RoseType;
            bool bStats = _parameters.SaveStatistics;
            bool bBearing = _parameters.Bearing;
            int nInterval = _parameters.Interval;

            var createCells = _parameters.flapParameters.Where(a => a.CreateCell).Select(b => b).ToList();

            foreach(var cell in createCells)
            {
                if (cell.LenAzi.Count > 0)
                {
                    cell.FishStats = new FishnetStatistics();
                    cell.FishStats.CalculateFishnetStatistics(cell.LenAzi, roseType, cell);

                    cell.Rose = new RoseDiagramParameters();

                    await cell.Rose.RoseArrayValues(bBearing, NoOfElements, nInterval, cell.LenAzi, roseType);
                    await cell.Rose.CalculateRosePetals(nInterval, cell.RoseExtent.CellWidth, cell.RoseExtent.CellHeight);

                    if (bStats)
                        await cell.Rose.CalculateRoseStatistics(cell.LenAzi, _parameters.Interval, _parameters.Bearing);
                }
            }
            return true;
        }

        public override async Task<string> SaveFeatures(FlapParameters _parameters)
        {
            string message = String.Empty;

            var createCells = _parameters.flapParameters.Where(a => a.CreateCell).Select(b => b).ToList();

            foreach (var cell in createCells)
            {
                cell.Rose.petals = new List<RosePetalCoordinates>();

                if (cell.LenAzi.Count > 0)
                {
                    int counter = cell.Rose.RoseArrayBin.GetUpperBound(0);

                    for (int i = 0; i < counter + 1; i++)
                    {
                        cell.Rose.petals.Add(new RosePetalCoordinates()
                        {
                            PetalID = i,
                            OriginX = cell.RoseExtent.CentreX,
                            OriginY = cell.RoseExtent.CentreY,
                            x1 = cell.Rose.RoseArrayBin[i, 0] + cell.RoseExtent.CentreX,
                            y1 = cell.Rose.RoseArrayBin[i, 1] + cell.RoseExtent.CentreY,
                            x2 = cell.Rose.RoseArrayBin[i, 2] + cell.RoseExtent.CentreX,
                            y2 = cell.Rose.RoseArrayBin[i, 3] + cell.RoseExtent.CentreY
                        });
                    }
                }
            }

            return message;
        }
    }

    /*  public class FishnetCoverage : TypeOfAnalysis
    {



            public override async Task<FlapParameters> PrepareInputForProcessing(FeatureLayer InputLayer,
                Envelope customEnvelope, int subCellsize, int nInterval, bool selectedFeatures, RoseGeom roseGeom,
                bool bRegional, string fieldName)
            {
                if (!bRegional)
                {
                    if (!selectedFeatures)
                        return await VectorFunctions.PrepareInputForCellFishnet(InputLayer, customEnvelope, subCellsize,
                            nInterval, 0, 0, roseGeom, fieldName);
                    else
                        return await VectorFunctions.PrepareInputForCellFishnet(InputLayer, customEnvelope, subCellsize,
                        nInterval, 0, 0, roseGeom, fieldName, true);
                }
                else
                    return await VectorFunctions.PrepareInputForRegionalFishnet(InputLayer, customEnvelope,
                        nInterval, roseGeom, fieldName);

            }

            public override async Task<string> SaveAsVectorFeatures(FeatureClass OuputFeatureClass, SpatialReference mySpatialRef, FlapParameters _parameters, bool bExtentOnly)
            {
                return await VectorFunctions.SaveFishnetVectorFeatures(OuputFeatureClass, mySpatialRef, _parameters, bExtentOnly);
            }

            public override async Task<FeatureClass> CreateFeatureClass(string suffix, string fishnetName, bool bExists,
                string databasePath, FeatureLayer InputLayer, bool bStatistics, RoseGeom roseGeom, SpatialReference thisSpatRef)
            {
                return await VectorFunctions.CreateRoseNetFeatClass(suffix, fishnetName, bExists, databasePath, InputLayer,
                 bStatistics, roseGeom, true, thisSpatRef);
            }

        }*/

    public class RoseRegionalPlot : TypeOfAnalysis
    {
        public override async Task<FlapParameters> PrepareInputForProcessing(List<LinearValues> orientValues, CellExtent customEnvelope, int subCellsize,
int nInterval, bool selectedFeatures, RoseDiagramParameters rose, bool bRegional, string fieldName, RoseType roseType, RoseGeom roseGeom)
        {
            int cellID = 1;

            //calculate centre of cell
            customEnvelope.CentreOfCell();

            FlapParameters regionalRose = new FlapParameters
            {
                Interval = nInterval,
                SelectedFeatures = selectedFeatures,
                flapParameters = new List<FlapParameter>(),
                _RoseType = roseType,
                _RoseGeom = roseGeom
            };

            regionalRose.flapParameters.Add(new FlapParameter
            {
                RoseExtent = customEnvelope,
                CreateCell = true,  //default
                CellID = cellID,
                Rose = rose
            }) ;

            if (regionalRose._RoseType == RoseType.Frequency)
            {
                foreach (var val in orientValues)
                {
                    regionalRose.flapParameters.Last().LenAzi.Add(new FreqLen
                    {
                        Azimuth = val.Direction,
                        Length = 0.0
                    });
                }
            }
            else
            {
                foreach (var val in orientValues)
                {
                    regionalRose.flapParameters.Last().LenAzi.Add(new FreqLen
                    {
                        Azimuth = val.Direction,
                        Length = val.LineLength
                    });
                }
            }

            return regionalRose;


        }

        public override async Task<bool> CalculateValues(FlapParameters _parameters)
        {
            int NoOfElements = 180 / _parameters.Interval;

            RoseType roseType = _parameters._RoseType;
            bool bStats = _parameters.SaveStatistics;
            bool bBearing = _parameters.Bearing;
            int nInterval = _parameters.Interval;


            FlapParameter parameter = _parameters.flapParameters[0];

            if (parameter.CreateCell)
            {
                parameter.Rose = parameter.Rose;// new RoseDiagramParameters();

                await parameter.Rose.RoseArrayValues(bBearing, NoOfElements, nInterval, parameter.LenAzi, roseType);

                await parameter.Rose.CalculateRosePetals(nInterval, parameter.RoseExtent.CellWidth, parameter.RoseExtent.CellHeight);

                if (bStats)
                    await parameter.Rose.CalculateRoseStatistics(parameter.LenAzi, _parameters.Interval, _parameters.Bearing);

                parameter.FishStats = new FishnetStatistics();
                parameter.FishStats.CalculateFishnetStatistics(parameter.LenAzi, roseType, parameter);
            }

            return true;
        }

        public override async Task<string> SaveFeatures(FlapParameters _parameters)
        {
            string message = String.Empty;

            foreach (FlapParameter parameter in _parameters.flapParameters)
            {
                if (parameter.CreateCell)
                {
                    parameter.Rose.petals = new List<RosePetalCoordinates>();

                    if (parameter.LenAzi.Count > 0)
                    {
                        int counter = parameter.Rose.RoseArrayBin.GetUpperBound(0);

                        for (int i = 0; i < counter + 1; i++)
                        {
                            parameter.Rose.petals.Add(new RosePetalCoordinates()
                            {
                                PetalID = i,
                                OriginX = parameter.RoseExtent.CentreX,
                                OriginY = parameter.RoseExtent.CentreY,
                                x1 = parameter.Rose.RoseArrayBin[i, 0] + parameter.RoseExtent.CentreX,
                                y1 = parameter.Rose.RoseArrayBin[i, 1] + parameter.RoseExtent.CentreY,
                                x2 = parameter.Rose.RoseArrayBin[i, 2] + parameter.RoseExtent.CentreX,
                                y2 = parameter.Rose.RoseArrayBin[i, 3] + parameter.RoseExtent.CentreY
                            });

                        }
                    }
                }
            }

            return message;

        }

    }


}
