using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Linear_Engine.DTO;
using System.Xml.Linq;

namespace Linear_Engine
{
    public static class ImportRoseData
    {
        //TODO - don't need to return RoseDto strictly speaking!
        public async static Task<RoseDto> RetrieveTableFieldnames(RoseDto roseDto, RoseGeom roseGeom)
        {

            roseDto.tableIsValid = true;

            if (roseDto.tableIsValid)
            {
                roseDto.tableFields = new ImportTableFields();

                roseDto.fields = new List<string>();

                List<string> roseFields = new List<string>();

                bool bCSV = true;

                switch (roseDto.tableFormat)
                {

                    case ImportTableFormat.text_csv:
                        {
                            roseFields = await RetrieveFieldnames(true, roseDto);
                            break;
                        }

                    case ImportTableFormat.text_txt:
                        {
                            roseFields = await RetrieveFieldnames(false, roseDto);

                            break;
                        }
                    case ImportTableFormat.other:
                        {
                            throw new Exception("Table format currently not supported");
                        }

                    default:
                        throw new Exception("Generic error with previewing table");
                }


                foreach (string field in roseFields)
                {
                    roseDto.fields.Add(field);
                }

                if (RoseGeom.Line == roseGeom)
                    PopulateLineFields(roseDto);
                else
                    PopulatePointFields(roseDto);
            }

            return roseDto;
        }

        private async static void PopulatePointFields(RoseDto roseDto)
        {
            int index = 0;
            int fieldIncrement = 4;

            if (roseDto.fields.Count < fieldIncrement)
                throw new Exception("Problem with fields in Rose table. Minimum of 3 fields required");

            //OID
            roseDto.tableFields.Add(new ImportTableField
            {
                columnHeader = roseDto.fields[index],
                columnImportAs = RoseConstants.recID,
                groupName = RoseConstants.GroupMapFields,
                columnImportName = RoseConstants.recIDName,
                genericType = false,
                fieldType = "Integer",
                keys = new KeyValuePair<bool, bool>(true, false) //Primary key
            });

            index++;
            //X
            roseDto.tableFields.Add(new ImportTableField
            {
                columnHeader = roseDto.fields[index],
                columnImportAs = RoseConstants.Startx,
                groupName = RoseConstants.GroupMapFields,
                columnImportName = RoseConstants.xStartName,
                genericType = false,
                fieldType = "Double",
                keys = new KeyValuePair<bool, bool>(false, false)
            });
            index++;
            //Y
            roseDto.tableFields.Add(new ImportTableField
            {
                columnHeader = roseDto.fields[index],
                columnImportAs = RoseConstants.Starty,
                groupName = RoseConstants.GroupMapFields,
                columnImportName = RoseConstants.yStartName,
                genericType = false,
                fieldType = "Double",
                keys = new KeyValuePair<bool, bool>(false, false)
            });
            index++;
            //Orient
            roseDto.tableFields.Add(new ImportTableField
            {
                columnHeader = roseDto.fields[index],
                columnImportAs = RoseConstants.orient,
                groupName = RoseConstants.GroupMapFields,
                columnImportName = RoseConstants.orientName,
                genericType = false,
                fieldType = "Double",
                keys = new KeyValuePair<bool, bool>(false, false)
            });
            index++;

            for (int i = fieldIncrement; i < roseDto.fields.Count; i++)
            {
                roseDto.tableFields.Add(new ImportTableField
                {
                    columnHeader = roseDto.fields[i],
                    columnImportName = roseDto.fields[i],
                    columnImportAs = "Not Imported",
                    groupName = RoseConstants.GroupOtherFields,
                    genericType = true,
                    fieldType = "Text",
                    keys = new KeyValuePair<bool, bool>(false, false)
                });
            }
        }

        private async static void PopulateLineFields(RoseDto roseDto)
        {
            int index = 0;
            int fieldIncrement = 6;

            if (roseDto.fields.Count < fieldIncrement)
                throw new Exception("Problem with fields in Rose table. Minimum of 5 fields required");

            //OID
            roseDto.tableFields.Add(new ImportTableField
            {
                columnHeader = roseDto.fields[index],
                columnImportAs = RoseConstants.recID,
                groupName = RoseConstants.GroupMapFields,
                columnImportName = RoseConstants.recIDName,
                genericType = false,
                fieldType = "Integer",
                keys = new KeyValuePair<bool, bool>(true, false) //Primary key
            });

            index++;
            //StartX
            roseDto.tableFields.Add(new ImportTableField
            {
                columnHeader = roseDto.fields[index],
                columnImportAs = RoseConstants.Startx,
                groupName = RoseConstants.GroupMapFields,
                columnImportName = RoseConstants.xStartName,
                genericType = false,
                fieldType = "Double",
                keys = new KeyValuePair<bool, bool>(false, false)
            });
            index++;
            //StartY
            roseDto.tableFields.Add(new ImportTableField
            {
                columnHeader = roseDto.fields[index],
                columnImportAs = RoseConstants.Starty,
                groupName = RoseConstants.GroupMapFields,
                columnImportName = RoseConstants.yStartName,
                genericType = false,
                fieldType = "Double",
                keys = new KeyValuePair<bool, bool>(false, false)
            });
            index++;
            //EndX
            roseDto.tableFields.Add(new ImportTableField
            {
                columnHeader = roseDto.fields[index],
                columnImportAs = RoseConstants.Endx,
                groupName = RoseConstants.GroupMapFields,
                columnImportName = RoseConstants.xEndName,
                genericType = false,
                fieldType = "Double",
                keys = new KeyValuePair<bool, bool>(false, false)
            });
            index++;
            //EndY
            roseDto.tableFields.Add(new ImportTableField
            {
                columnHeader = roseDto.fields[index],
                columnImportAs = RoseConstants.Endy,
                groupName = RoseConstants.GroupMapFields,
                columnImportName = RoseConstants.yEndName,
                genericType = false,
                fieldType = "Double",
                keys = new KeyValuePair<bool, bool>(false, false)
            });
            index++;
            //Orient
            roseDto.tableFields.Add(new ImportTableField
            {
                columnHeader = roseDto.fields[index],
                columnImportAs = RoseConstants.orient,
                groupName = RoseConstants.GroupMapFields,
                columnImportName = RoseConstants.orientName,
                genericType = false,
                fieldType = "Double",
                keys = new KeyValuePair<bool, bool>(false, false)
            });
            index++;

            for (int i = fieldIncrement; i < roseDto.fields.Count; i++)
            {
                roseDto.tableFields.Add(new ImportTableField
                {
                    columnHeader = roseDto.fields[i],
                    columnImportName = roseDto.fields[i],
                    columnImportAs = "Not Imported",
                    groupName = RoseConstants.GroupOtherFields,
                    genericType = true,
                    fieldType = "Text",
                    keys = new KeyValuePair<bool, bool>(false, false)
                });
            }
        }

        private async static Task<List<string>> RetrieveFieldnames(bool bCSV, RoseDto roseDto)
        {
            List<string> roseFields = new List<string>();

        char delimiter = '\t';

            if (bCSV)
                delimiter = ',';

            bool bExists = File.Exists(roseDto.tablePath + "\\" + roseDto.tableName);

            if (!bExists)
                throw new Exception("File does not exist");

            using (var reader = new StreamReader(roseDto.tablePath + "\\" + roseDto.tableName))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();

                    if (line == null)
                        break;

                    
                    var values = line.Split(delimiter);

                    //first row only for columns
                    foreach (string column in values)
                    {
                        roseFields.Add(column);

                    }
                    break;
                }
            }

            return roseFields;
        }

        public async static void ImportData(RoseDto roseDto, bool bCSV)
        {
            if (roseDto.tableIsValid)
            {

                switch (roseDto.tableFormat)
                {

                    case ImportTableFormat.text_csv:
                        {
                            FormatAsXmlData(roseDto,true);
                            break;
                        }

                    case ImportTableFormat.other:
                        {
                            throw new Exception("Table format currently not supported");

                        }

                    case ImportTableFormat.text_txt:
                        {
                            FormatAsXmlData(roseDto, false);
                            break;

                        }

                    default:
                        throw new Exception("Generic error with formatting data");
                }
            }


          //  return roseDto;
        }

        private async static void FormatAsXmlData(RoseDto roseDto, bool bCSV)
        {

            roseDto.xPreview = new XElement("Rose");

            char delimiter = '\t';

            if (bCSV)
                delimiter = ',';

            List<CsvRow> rows = new List<CsvRow>();

            XElement mFieldItems = null;

            int counter = 0;

                try
            {
                using (var reader = new StreamReader(roseDto.tablePath + "\\" + roseDto.tableName))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(delimiter);

                        if (counter > 0)
                        {
                            foreach (string row in values)
                            {
                                string strValue = row;
                                if (row == "")
                                    strValue = "-";

                                rows.Add(new CsvRow { results = strValue });
                            }

                            mFieldItems = new XElement("Rose", new XAttribute("ID", (counter - 1).ToString()));

                            for (int i = 0; i < roseDto.fields.Count; i++)
                            {
                                XElement mNode = new XElement(roseDto.fields[i], rows[i].results);
                                mFieldItems.Add(mNode);
                            }

                            roseDto.xPreview.Add(mFieldItems);
                            rows.Clear();
                        }

                        counter++;
                    }
                }
            }
            catch
            {
                throw new Exception("There is a problem with table format. Irrecoverable error at line: " + counter--.ToString());
            }

        }


    }

    public class CsvRow
    {
        public string results { get; set; }

    }


}
