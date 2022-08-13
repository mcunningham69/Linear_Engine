using System;
using System.Collections.Generic;
using System.Linq;
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
    int nInterval, bool selectedFeatures, RoseDiagramParameters rose, bool bRegional, string fieldName)
        {
            return await _factoryType.PrepareInputForProcessing(orientValues, customEnvelope, subCellsize, nInterval,
                selectedFeatures, rose, bRegional, fieldName);
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

        /*

         public async Task<FeatureClass> CreateFeatureClass(string suffix, string roseName, bool bExists, string databasePath,
     FeatureLayer InputLayer, bool bStatistics, RoseGeom roseGeom, SpatialReference thisSpatRef)
         {
             return await _factoryType.CreateFeatureClass(suffix, roseName, bExists, databasePath, InputLayer,
                 bStatistics, roseGeom, thisSpatRef);

         }

         public async Task<string> SaveAsVectorFeatures(FeatureClass OuputFeatureClass, SpatialReference mySpatialRef,
             FlapParameters _parameters, bool bExtentOnly)
         {
             return await _factoryType.SaveAsVectorFeatures(OuputFeatureClass, mySpatialRef, _parameters, bExtentOnly);
         }


         */
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


        public abstract Task<FlapParameters> PrepareInputForProcessing(List<LinearValues> orientValues, CellExtent customEnvelope, int subCellsize,
    int nInterval, bool selectedFeatures, RoseDiagramParameters rose, bool bRegional, string fieldName);

        public abstract Task<bool> CalculateValues(FlapParameters _parameters);

        public abstract Task<string> SaveFeatures(FlapParameters _parameters);

        /*
         public abstract Task<FeatureClass> CreateFeatureClass(string suffix, string roseName, bool bExists, string databasePath,
     FeatureLayer InputLayer, bool bStatistics, RoseGeom roseGeom, SpatialReference thisSpatRef);

                 public abstract Task<string> SaveAsVectorFeatures(FeatureClass OuputFeatureClass, SpatialReference mySpatialRef,
             FlapParameters _parameters, bool bExtentOnly);

         */
    }

    public class RoseCellPlots : TypeOfAnalysis
    {

        public override async Task<FlapParameters> PrepareInputForProcessing(List<LinearValues> orientValues, CellExtent customEnvelope, int subCellsize,
    int nInterval, bool selectedFeatures, RoseDiagramParameters rose, bool bRegional, string fieldName)
        {
            int cellID = 1;

            //calculate centre of cell
            customEnvelope.CentreOfCell();

            FlapParameters regionalRose = new FlapParameters
            {
                Interval = nInterval,
                SelectedFeatures = selectedFeatures,
                flapParameters = new List<FlapParameter>()
            };

            regionalRose.flapParameters.Add(new FlapParameter
            {
                CentreX = customEnvelope.CentreX,
                CentreY = customEnvelope.CentreY,
                CreateCell = true,  //default
                ExtentArea = customEnvelope.CellArea,
                ExtentHeight = customEnvelope.CellHeight,
                ExtentWidth = customEnvelope.CellWidth,
                XMin = customEnvelope.MinX,
                XMax = customEnvelope.MaxX,
                YMin = customEnvelope.MinY,
                YMax = customEnvelope.MaxY,
                CellID = cellID
            });

            foreach (var val in orientValues)
            {
                regionalRose.flapParameters.Last().LenAzi.Add(new FreqLen
                {
                    Azimuth = val.Direction,
                    Length = 0.0
                });
            }

            return regionalRose;

            /*
           public override void CalculateVectorValues(FlapParameters _parameters, bool Statistics, RoseType roseType, bool bDirection)
           {
               int NoOfElements = 180 / _parameters.Interval;

               foreach (FlapParameter parameter in _parameters.flapParameters)
               {
                   if (parameter.CreateCell)
                   {
                       parameter.Rose = new RoseDiagramParameters();
                       parameter.Rose.RoseArrayValues(bDirection, NoOfElements, _parameters.Interval, parameter.LenAzi, roseType);
                       parameter.Rose.CalculateRosePetals(_parameters.Interval, parameter.ExtentWidth, parameter.ExtentHeight);
                       parameter.Rose.CalculateRoseStatistics(parameter.LenAzi, _parameters.Interval);
                   }
               }
           }



           public override async Task<string> SaveAsVectorFeatures(FeatureClass OuputFeatureClass,
               SpatialReference mySpatialRef, FlapParameters _parameters, bool bExtentOnly)
           {
               return await VectorFunctions.SaveRoseToVectorFeatures(OuputFeatureClass, mySpatialRef, _parameters);
           }

           public override async Task<FeatureClass> CreateFeatureClass(string suffix, string roseName, bool bExists,
               string databasePath, FeatureLayer InputLayer, bool bStatistics, RoseGeom roseGeom, SpatialReference thisSpatRef)
           {
               return await VectorFunctions.CreateRoseNetFeatClass(suffix, roseName, bExists, databasePath, InputLayer,
               bStatistics, roseGeom, false, thisSpatRef);
           }

           */
        }

        public override async Task<bool> CalculateValues(FlapParameters _parameters)
        {
            foreach (FlapParameter parameter in _parameters.flapParameters)
            {
                if (parameter.CreateCell)
                {
                    if (parameter.LenAzi.Count > 0)
                    {
                        parameter.FishStats = new FishnetStatistics();

                        parameter.FishStats.CalculateFishnetStatistics(parameter.LenAzi);
                    }
                }
            }

            return true;
        }

        public override async Task<string> SaveFeatures(FlapParameters _parameters)
        {
            return await SaveFeatures(_parameters);
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
int nInterval, bool selectedFeatures, RoseDiagramParameters rose, bool bRegional, string fieldName)
        {
            int cellID = 1;

            //calculate centre of cell
            customEnvelope.CentreOfCell();

            FlapParameters regionalRose = new FlapParameters
            {
                Interval = nInterval,
                SelectedFeatures = selectedFeatures,
                flapParameters = new List<FlapParameter>()

            };

            regionalRose.flapParameters.Add(new FlapParameter
            {
                CentreX = customEnvelope.CentreX,
                CentreY = customEnvelope.CentreY,
                CreateCell = true,  //default
                ExtentArea = customEnvelope.CellArea,
                ExtentHeight = customEnvelope.CellHeight,
                ExtentWidth = customEnvelope.CellWidth,
                XMin = customEnvelope.MinX,
                XMax = customEnvelope.MaxX,
                YMin = customEnvelope.MinY,
                YMax = customEnvelope.MaxY,
                CellID = cellID,
                Rose = rose
            });

            foreach (var val in orientValues)
            {
                regionalRose.flapParameters.Last().LenAzi.Add(new FreqLen
                {
                    Azimuth = val.Direction,
                    Length = 0.0
                });
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

                parameter.Rose.RoseArrayValues(bBearing, NoOfElements, nInterval, parameter.LenAzi, roseType);

                parameter.Rose.CalculateRosePetals(nInterval, parameter.ExtentWidth, parameter.ExtentHeight);

                if (bStats)
                    parameter.Rose.CalculateRoseStatistics(parameter.LenAzi, _parameters.Interval);
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
                                OriginX = parameter.CentreX,
                                OriginY = parameter.CentreY,
                                x1 = parameter.Rose.RoseArrayBin[i, 0] + parameter.CentreX,
                                y1 = parameter.Rose.RoseArrayBin[i, 1] + parameter.CentreY,
                                x2 = parameter.Rose.RoseArrayBin[i, 2] + parameter.CentreX,
                                y2 = parameter.Rose.RoseArrayBin[i, 3] + parameter.CentreY
                            });

                            //TODO - for statistics
                                /*
                                 * 
                                // use the builder constructor
                                Polygon poly = null;
                                using (PolygonBuilder pb = new PolygonBuilder(list))
                                {
                                    pb.SpatialReference = mySpatialRef;
                                    poly = pb.ToGeometry();
                                    rowBuffer[_definition.GetShapeField()] = poly;
                                }
                                int checkField = -1;

                                checkField = _definition.FindField("CellID");
                                if (checkField > -1)
                                    rowBuffer["CellID"] = parameter.CellID;

                                checkField = _definition.FindField("AvgAzi");
                                if (checkField > -1)
                                    rowBuffer["AvgAzi"] = parameter.Rose.AvgAzi[i];

                                checkField = _definition.FindField("MinAzi");
                                if (checkField > -1)
                                    rowBuffer["MinAzi"] = parameter.Rose.MinAzi[i];

                                checkField = _definition.FindField("MaxAzi");
                                if (checkField > -1)
                                    rowBuffer["MaxAzi"] = parameter.Rose.MaxAzi[i];

                                checkField = _definition.FindField("StdAzi");
                                if (checkField > -1)
                                    rowBuffer["StdAzi"] = parameter.Rose.StdAzi[i];

                                checkField = _definition.FindField("BinCount");
                                if (checkField > -1)
                                    rowBuffer["BinCount"] = parameter.Rose.BinCount[i];

                                checkField = _definition.FindField("BinAzi");
                                if (checkField > -1)
                                    rowBuffer["BinAzi"] = parameter.Rose.BinRange[i].LowerRange.ToString() + " - " + parameter.Rose.BinRange[i].UpperRange.ToString();


                                checkField = _definition.FindField("AvgLength");
                                if (checkField > -1)
                                    rowBuffer["AvgLength"] = parameter.Rose.AvgLength[i];

                                checkField = _definition.FindField("MinLength");
                                if (checkField > -1)
                                    rowBuffer["MinLength"] = parameter.Rose.MinLength[i];

                                checkField = _definition.FindField("MaxLength");
                                if (checkField > -1)
                                    rowBuffer["MaxLength"] = parameter.Rose.MaxLength[i];

                                checkField = _definition.FindField("StdLen");
                                if (checkField > -1)
                                    rowBuffer["StdLen"] = parameter.Rose.StdLength[i];

                                checkField = _definition.FindField("Created");
                                if (checkField > -1)
                                    rowBuffer["Created"] = "Created on: " + DateTime.Now.ToShortDateString() + " at: " + DateTime.Now.ToShortTimeString();
                                */

                        }
                    }
                }
            }

            return message;

        }

    }


}
