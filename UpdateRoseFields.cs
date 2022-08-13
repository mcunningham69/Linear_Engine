using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Linear_Engine.DTO;

namespace Linear_Engine
{
    public static class UpdateRoseFields
    {
        public static async Task<bool> UpdateImportParameters(RoseDto roseDto, string previousSelection, string changeTo, string searchColumn, string strOldName)
        {
            bool bCheck = true;  //to implement

            ImportTableField queryUpdate = new ImportTableField();

            //Return row to be updated
            queryUpdate = (from column in roseDto.tableFields
                                            where column.columnHeader == searchColumn
                                            select new ImportTableField
                                            {
                                                columnHeader = column.columnHeader,
                                                columnImportAs = column.columnImportAs,
                                                columnImportName = column.columnImportName,
                                                groupName = column.groupName,
                                                fieldType = column.fieldType,
                                                keys = column.keys
                                            }).SingleOrDefault();
          
            switch (changeTo)
            {
                case RoseConstants.recIDName:
                    {
                        try
                        {
                            UpdateMandatoryFields(previousSelection, RoseConstants.recID,
                                RoseConstants.recIDName, queryUpdate, roseDto.tableFields, RoseConstants.GroupMapFields,
                                false, "Text", new KeyValuePair<bool, bool>(true, false));
                        }
                        catch
                        {
                            UpdateOptionalFields(RoseConstants.recIDName, RoseConstants.recID, queryUpdate, roseDto.tableFields, RoseConstants.GroupMapFields,
                                false, "Text", new KeyValuePair<bool, bool>(false, false));
                        }

                        break;
                    }
                case RoseConstants.xStartName:
                    {

                        try
                        {
                            UpdateMandatoryFields(previousSelection, RoseConstants.Coord,
                            RoseConstants.xStartName, queryUpdate, roseDto.tableFields, RoseConstants.GroupMapFields,
                            false, "Double", new KeyValuePair<bool, bool>(false, false));
                        }
                        catch
                        {
                            UpdateOptionalFields(RoseConstants.xStartName, RoseConstants.Coord, queryUpdate, roseDto.tableFields, RoseConstants.GroupMapFields,
                                false, "Double", new KeyValuePair<bool, bool>(false, false));
                        }
                        break;
                    }
                case RoseConstants.yStartName:
                    {
                        try
                        {
                            UpdateMandatoryFields(previousSelection, RoseConstants.Coord,
                                RoseConstants.yStartName, queryUpdate, roseDto.tableFields, RoseConstants.GroupMapFields,
                                false, "Double", new KeyValuePair<bool, bool>(false, false));
                        }
                        catch
                        {
                            UpdateOptionalFields(RoseConstants.yStartName, RoseConstants.Coord, queryUpdate, roseDto.tableFields, RoseConstants.GroupMapFields,
                                false, "Double", new KeyValuePair<bool, bool>(false, false));
                        }

                        break;

                    }
                case RoseConstants.xEndName:
                    {
                        try
                        {
                            UpdateMandatoryFields(previousSelection, RoseConstants.Coord,
                            RoseConstants.xEndName, queryUpdate, roseDto.tableFields, RoseConstants.GroupMapFields,
                            false, "Double", new KeyValuePair<bool, bool>(false, false));
                        }
                        catch
                        {
                            UpdateOptionalFields(RoseConstants.xEndName, RoseConstants.Coord, queryUpdate, roseDto.tableFields, RoseConstants.GroupMapFields,
                                false, "Double", new KeyValuePair<bool, bool>(false, false));
                        }
                        break;
                    }
                case RoseConstants.yEndName:
                    {
                        try
                        {
                            UpdateMandatoryFields(previousSelection, RoseConstants.Coord,
                                RoseConstants.yEndName, queryUpdate, roseDto.tableFields, RoseConstants.GroupMapFields,
                                false, "Double", new KeyValuePair<bool, bool>(false, false));
                        }
                        catch
                        {
                            UpdateOptionalFields(RoseConstants.yEndName, RoseConstants.Coord, queryUpdate, roseDto.tableFields, RoseConstants.GroupMapFields,
                                false, "Double", new KeyValuePair<bool, bool>(false, false));
                        }

                        break;

                    }
                case RoseConstants.orient:
                    {
                        try
                        {
                            UpdateMandatoryFields(previousSelection, RoseConstants.orient,
                                RoseConstants.orientName, queryUpdate, roseDto.tableFields, RoseConstants.GroupMapFields,
                                false, "Double", new KeyValuePair<bool, bool>(false, false));
                        }
                        catch
                        {
                            UpdateOptionalFields(RoseConstants.orientName, RoseConstants.orient, queryUpdate, roseDto.tableFields, RoseConstants.GroupMapFields,
                                false, "Double", new KeyValuePair<bool, bool>(false, false));
                        }

                        break;
                    }

                case RoseConstants.length:
                    {
                        try
                        {
                            UpdateMandatoryFields(previousSelection, RoseConstants.length,
                                RoseConstants.lengthName, queryUpdate, roseDto.tableFields, RoseConstants.GroupMapFields,
                                false, "Double", new KeyValuePair<bool, bool>(false, false));
                        }
                        catch
                        {
                            UpdateOptionalFields(RoseConstants.lengthName, RoseConstants.orient, queryUpdate, roseDto.tableFields, RoseConstants.GroupMapFields,
                                false, "Double", new KeyValuePair<bool, bool>(false, false));
                        }

                        break;
                    }


                case RoseConstants.notImported:
                    {
                        NotImportedFields(RoseConstants.notImported, queryUpdate, roseDto.tableFields,
                            RoseConstants.GroupOtherFields, true);

                        break;
                    }

                case RoseConstants._numeric:
                    {
                        UpdateOptionalFields(RoseConstants._numeric, queryUpdate, roseDto.tableFields,
                            RoseConstants.GroupOtherFields, true, "Double", new KeyValuePair<bool, bool>(false, false));
                        break;
                    }

                case RoseConstants._text:
                    UpdateOptionalFields(RoseConstants._text, queryUpdate, roseDto.tableFields,
                        RoseConstants.GroupOtherFields, true, "Text", new KeyValuePair<bool, bool>(false, false));
                    break;

                case RoseConstants._generic:
                    UpdateOptionalFields(RoseConstants._genericName, queryUpdate, roseDto.tableFields,
                        RoseConstants.GroupOtherFields, true, "Text", new KeyValuePair<bool, bool>(false, false));
                    break;

                default:
                    {
                        throw new Exception("There is a problem with changing field type");
                    }

            }

        
            return bCheck;

        }

        public static void UpdateMandatoryFields(string previousSelection, string changeTo, string changeToName,
    ImportTableField queryUpdate, ImportTableFields tableData, string groupFields, bool genericType,
    string fieldType, KeyValuePair<bool, bool> keys)
        {

            if (queryUpdate.columnImportAs == changeTo) //if selected field not changed, then just update values below
            {
                var queryTo = tableData.Where(v => v.columnImportAs == changeTo).First();

                queryTo.genericType = genericType;
                queryTo.groupName = groupFields;
            }
            else
            {
                //queryUpdate takes the existing field with the value that are being used to change the selected field. queryUpdate then 
                //takes the selected field's original values

                //this is the field that is being changed
                ImportTableField queryPrevious = (from column in tableData
                                                  where column.columnImportName == changeToName
                                                  select new ImportTableField
                                                  {
                                                      columnHeader = column.columnHeader,
                                                      columnImportAs = column.columnImportAs,
                                                      columnImportName = column.columnImportName,
                                                      groupName = column.groupName,
                                                      fieldType = column.fieldType,
                                                      keys = column.keys
                                                  }).FirstOrDefault();

                //take one to change and give to row that was previously assinged (i.e. reverse values)
                ImportTableField queryFrom = tableData.Where(v => v.columnImportAs == changeTo).FirstOrDefault();

                if (queryFrom == null)
                    queryFrom = new ImportTableField();

                string previousColumnImportAs = queryUpdate.columnImportAs;

                //if field being changed from is NotImported then use original column name for import as
                string previousColumnImportName = "";
                if (previousSelection == RoseConstants.notImported)
                {
                    previousColumnImportName = queryPrevious.columnHeader;
                }
                else
                {
                    previousColumnImportName = queryUpdate.columnImportName;
                }

                string previousGroupName = queryUpdate.groupName;
                bool previousType = queryUpdate.genericType;
                string previousFieldType = queryUpdate.fieldType;
                KeyValuePair<bool, bool> previousKeys = queryUpdate.keys;

                //the one to change
                var queryTo = tableData.Where(v => v.columnHeader == queryUpdate.columnHeader).First();

                //update changed from values
                queryFrom.columnImportAs = previousColumnImportAs;
                queryFrom.columnImportName = previousColumnImportName;
                queryFrom.groupName = previousGroupName;
                queryFrom.genericType = previousType;
                queryFrom.fieldType = previousFieldType;
                queryFrom.keys = previousKeys;

                //update changed to values
                queryTo.columnHeader.ToUpper(); //Make upper case
                queryTo.columnImportAs = changeTo;
                queryTo.columnImportName = changeToName;
                queryTo.groupName = groupFields;
                queryTo.fieldType = fieldType;  //check
                queryTo.genericType = genericType;
                queryTo.keys = keys;
            }

        }

        public static void UpdateOptionalFields(string changeTo, ImportTableField queryUpdate, ImportTableFields tableData,
    string groupFields, bool genericType, string fieldType, KeyValuePair<bool, bool> keys)
        {
            //the one to change
            var queryTo = tableData.Where(v => v.columnHeader == queryUpdate.columnHeader).First();

            string columnImportName = queryUpdate.columnHeader;

            //update changed to values
            queryTo.columnImportAs = changeTo;
            queryTo.columnImportName = columnImportName;
            queryTo.groupName = groupFields;
            queryTo.keys = keys;
            queryTo.fieldType = fieldType;

            queryTo.genericType = genericType;

        }

        public static void UpdateOptionalFields(string changeTo, string importAs, ImportTableField queryUpdate, ImportTableFields tableData,
    string groupFields, bool genericType, string fieldType, KeyValuePair<bool, bool> keys)
        {
            //the one to change
            var queryTo = tableData.Where(v => v.columnHeader == queryUpdate.columnHeader).First();

            //update changed to values
            queryTo.columnImportAs = importAs;
            queryTo.columnImportName = queryUpdate.columnHeader;
            queryTo.groupName = groupFields;
            queryTo.keys = keys;
            queryTo.fieldType = fieldType;

            queryTo.genericType = genericType;

        }

        public static void NotImportedFields(string changeTo, ImportTableField queryUpdate, ImportTableFields tableData,
    string groupFields, bool genericType)
        {

            if (queryUpdate.columnImportAs == changeTo)
            {
                var queryTo = tableData.Where(v => v.columnImportAs == changeTo).First();

                queryTo.genericType = genericType;
                queryTo.groupName = groupFields;
                queryTo.fieldType = queryUpdate.fieldType;
                queryTo.keys = new KeyValuePair<bool, bool>(false, false);
            }
            else if (queryUpdate.columnImportAs == RoseConstants._genericName)
            {
                var queryTo = tableData.Where(v => v.columnImportAs == RoseConstants._genericName && v.columnHeader == queryUpdate.columnHeader).First();

                queryTo.columnImportAs = RoseConstants.notImported;
                queryTo.columnImportName = queryTo.columnHeader;
                queryTo.groupName = groupFields;
                queryTo.genericType = true;
                queryTo.fieldType = "Text";  //NEED TODO 
                queryTo.keys = new KeyValuePair<bool, bool>(false, false);

            }
            else if (changeTo == RoseConstants.notImported && queryUpdate.columnImportAs == RoseConstants._numeric)
            {
                var queryTo = tableData.Where(v => v.columnImportAs == RoseConstants._numeric && v.columnHeader == queryUpdate.columnHeader).First();

                queryTo.columnImportAs = RoseConstants.notImported;
                queryTo.columnImportName = queryTo.columnHeader;
                queryTo.groupName = groupFields;
                queryTo.genericType = true;
                queryTo.fieldType = "Double";
                queryTo.keys = new KeyValuePair<bool, bool>(false, false);

            }
            else if (changeTo == RoseConstants.notImported && queryUpdate.columnImportAs == RoseConstants._text)
            {
                var queryTo = tableData.Where(v => v.columnImportAs == RoseConstants._text && v.columnHeader == queryUpdate.columnHeader).First();

                queryTo.columnImportAs = RoseConstants.notImported;
                queryTo.columnImportName = queryTo.columnHeader;
                queryTo.groupName = groupFields;
                queryTo.genericType = true;
                queryTo.fieldType = "Text";
                queryTo.keys = new KeyValuePair<bool, bool>(false, false);

            }
            else
            {
                //retrieve values to update row being changed from
                ImportTableField queryPrevious = (from column in tableData
                                                  where column.columnImportName == changeTo
                                                  select new ImportTableField
                                                  {
                                                      columnHeader = column.columnHeader,
                                                      columnImportAs = column.columnImportAs,
                                                      columnImportName = column.columnImportName,
                                                      groupName = column.groupName,
                                                      genericType = column.genericType,
                                                      fieldType = column.fieldType,
                                                      keys = column.keys
                                                  }).SingleOrDefault();

                //take one to change and give to row that was previously assinged (i.e. reverse values)
                var queryFrom = tableData.Where(v => v.columnImportAs == changeTo).First();

                string previousColumnImportAs = queryUpdate.columnImportAs;
                string previousColumnImportName = queryUpdate.columnImportName;
                string previousGroupName = queryUpdate.groupName;
                bool previousType = queryUpdate.genericType;

                string previousFieldType = queryUpdate.fieldType;
                KeyValuePair<bool, bool> previousKeys = queryUpdate.keys;

                //the one to change
                var queryTo = tableData.Where(v => v.columnHeader == queryUpdate.columnHeader).First();

                //update changed from values
                queryFrom.columnImportAs = previousColumnImportAs;
                queryFrom.columnImportName = previousColumnImportName;
                queryFrom.groupName = previousGroupName;
                queryFrom.genericType = previousType;
                queryFrom.fieldType = previousFieldType;
                queryFrom.keys = previousKeys;

                //update changed to values
                queryTo.columnImportAs = changeTo;
                queryTo.columnImportName = queryUpdate.columnHeader;
                queryTo.groupName = groupFields;
                queryTo.genericType = genericType;
                queryTo.fieldType = queryUpdate.fieldType;  //check
                queryTo.keys = new KeyValuePair<bool, bool>(false, false);
            }

        }

        public static async void ImportAllFieldsAsGeneric(RoseDto roseDto, bool bImport)
        {
            if (bImport)
            {
                var queryTo = roseDto.tableFields.Where(v => v.columnImportAs == RoseConstants.notImported);

                foreach (var _val in queryTo)
                {
                    _val.columnImportAs = RoseConstants._genericName;
                    _val.columnImportName = _val.columnHeader;
                    _val.groupName = RoseConstants.GroupOtherFields;
                    _val.genericType = true;
                    _val.fieldType = "Text"; //TODO
                    _val.keys = new KeyValuePair<bool, bool>(false, false);
                }

            }

            // return roseDto;
        }


    }
}
