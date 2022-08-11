using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Linear_Engine.DTO;

namespace Linear_Engine
{
    public static class ImportRoseData
    {
        public async static Task<RoseDto> RetrieveTableFieldnames(RoseDto roseDto)
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

                int index = 0;
                int fieldIncrement = 4;

                //if (roseDto.fields[0].ToUpper() == "OBJECTID")
               // {
                    //index = index + 1;
                    //fieldIncrement = fieldIncrement + 1;
                //}

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
                    columnImportAs = RoseConstants.x,
                    groupName = RoseConstants.GroupMapFields,
                    columnImportName = RoseConstants.xName,
                    genericType = false,
                    fieldType = "Double",
                    keys = new KeyValuePair<bool, bool>(false, false)
                });
                index++;
                //Y
                roseDto.tableFields.Add(new ImportTableField
                {
                    columnHeader = roseDto.fields[index],
                    columnImportAs = RoseConstants.y,
                    groupName = RoseConstants.GroupMapFields,
                    columnImportName = RoseConstants.yName,
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

            return roseDto;
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

    }


}
