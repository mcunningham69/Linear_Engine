using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Linear_Engine.DTO;

namespace Linear_Engine
{
    public class ImportRoseData
    {
        private RoseDto roseDto { get; set; }

        public async Task<RoseDto> RetrieveTableFieldnames(string tablePath, string tableName, ImportTableFormat tableFormat)
        {
            roseDto = new RoseDto();

            roseDto.tablePath = tablePath;
            roseDto.tableIsValid = true;
            roseDto.tableName = tableName;

            if (roseDto.tableIsValid)
            {
                roseDto.tableData = new ImportTableFields();

                roseDto.fields = new List<string>();

                List<string> roseFields = new List<string>();

                bool bCSV = true;

                switch (tableFormat)
                {

                    case ImportTableFormat.text_csv:
                        {
                            roseFields = await RetrieveFieldnames(true);
                            break;
                        }

                    case ImportTableFormat.text_txt:
                        {
                            roseFields = await RetrieveFieldnames(false);

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
                int fieldIncrement = 3;

                if (roseDto.fields[0].ToUpper() == "OBJECTID")
                {
                    index = index + 1;
                    fieldIncrement = fieldIncrement + 1;
                }

                if (roseDto.fields.Count < fieldIncrement)
                    throw new Exception("Problem with fields in Rose table. Minimum of 3 fields required");

                //BHID
                roseDto.tableData.Add(new ImportTableField
                {
                    columnHeader = roseDto.fields[index],
                    columnImportAs = "OID",
                    groupName = "Mandatory Fields",
                    columnImportName = "Row ID",
                    genericType = false,
                    fieldType = "Text",
                    keys = new KeyValuePair<bool, bool>(true, false) //Primary key
                });

                index++;
                //X
                roseDto.tableData.Add(new ImportTableField
                {
                    columnHeader = roseDto.fields[index],
                    columnImportAs = "X",
                    groupName = "Mandatory Fields",
                    columnImportName = "Easting",
                    genericType = false,
                    fieldType = "Double",
                    keys = new KeyValuePair<bool, bool>(false, false)
                });
                index++;
                //Y
                roseDto.tableData.Add(new ImportTableField
                {
                    columnHeader = roseDto.fields[index],
                    columnImportAs = "Y",
                    groupName = "Mandatory Fields",
                    columnImportName = "Northing",
                    genericType = false,
                    fieldType = "Double",
                    keys = new KeyValuePair<bool, bool>(false, false)
                });
                index++;
                //Z
                roseDto.tableData.Add(new ImportTableField
                {
                    columnHeader = roseDto.fields[index],
                    columnImportAs = "Orient",
                    groupName = "Mandatory Fields",
                    columnImportName = "Orientation",
                    genericType = false,
                    fieldType = "Double",
                    keys = new KeyValuePair<bool, bool>(false, false)
                });
                index++;

                for (int i = fieldIncrement; i < roseDto.fields.Count; i++)
                {
                    roseDto.tableData.Add(new ImportTableField
                    {
                        columnHeader = roseDto.fields[i],
                        columnImportName = roseDto.fields[i],
                        columnImportAs = "Not Imported",
                        groupName = "Other Fields",
                        genericType = true,
                        fieldType = "Text",
                        keys = new KeyValuePair<bool, bool>(false, false)
                    });
                }
            }

            return roseDto;
        }

        private async Task<List<string>> RetrieveFieldnames(bool bCSV)
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
