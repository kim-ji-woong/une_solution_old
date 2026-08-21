using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Collections.Generic;
using System.IO;

namespace SensorMaker.BLL.Excel.Writer
{
    using Models.Data.Sensor;

    public abstract class ExcelWriter
    {
        public ExcelWriter()
        {
        }

        public byte[] Run(out string strErrorMessage)
        {
            try
            {
                ICollection<SheetData> sheetDatas = ReadSheetDatas(out strErrorMessage);

                if (sheetDatas == null)
                {
                    System.Diagnostics.Trace.WriteLine(strErrorMessage);
                    return null;
                }

                HSSFWorkbook workbook = MakeWorkbook();

                if (workbook == null)
                    return null;

                WriteSheetDatas(workbook, sheetDatas);

                byte[] bytes = null;

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.Write(stream);
                    bytes = stream.ToArray();
                }

                workbook.Close();
                return bytes;
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                strErrorMessage = e.Message;
            }

            return null;
        }

        private void WriteSheetDatas(HSSFWorkbook workbook, ICollection<SheetData> sheetDatas)
        {
            foreach (SheetData sheetData in sheetDatas)
            {
                ISheet sheet = workbook.CreateSheet(sheetData.SheetName);

                if (sheet == null)
                    return;

                IRow row = sheet.CreateRow(0);

                int min, max;

                if (GetMinMax(sheetData.Titles, out max, out min) == false)
                    continue;

                string strTitle;

                for (int i = min; i <= max; i++)
                {
                    if (sheetData.Titles.TryGetValue(i, out strTitle))
                    {
                        ICell cell = row.CreateCell(i);

                        if (cell != null && strTitle != null)
                            cell.SetCellValue(strTitle);
                    }
                }

                List<string> values;
                // Key : Column Index
                Dictionary<int, IRow> dicColumnRows = new Dictionary<int, IRow>();

                for (int i = min; i <= max; i++)
                {
                    if (sheetData.ColumnDatas.TryGetValue(i, out values))
                    {
                        int nValueCount = values.Count;

                        for (int j = 0; j < nValueCount; j++)
                        {
                            if (dicColumnRows.TryGetValue(j, out row) == false)
                            {
                                row = sheet.CreateRow(j + 1);
                                dicColumnRows[j] = row;
                            }

                            string str = values[j];
                            ICell cell = row.CreateCell(i);

                            if (cell != null && str != null)
                                cell.SetCellValue(str);
                        }
                    }
                }
            }
        }

        private bool GetMinMax(Dictionary<int, string> dicTitles, out int max, out int min)
        {
            max = -1;
            min = 1;

            foreach (KeyValuePair<int, string> pair in dicTitles)
            {
                if (min > max)
                {
                    min = max = pair.Key;
                }
                else
                {
                    if (min > pair.Key)
                        min = pair.Key;

                    if (max < pair.Key)
                        max = pair.Key;
                }
            }

            return min <= max;
        }

        private HSSFWorkbook MakeWorkbook()
        {
            //string strCompany = ConfigurationManager.AppSettings.Get("company");
            string strCompany = "솔브레인";

            if (strCompany == null)
                strCompany = "";

            //book1.xls is an Excel-2007-generated file, so some new unknown BIFF records are added. 
            //stream = new FileStream(m_strFilePath, FileMode.Create);

            HSSFWorkbook hssfworkbook = new HSSFWorkbook(/*stream*/);

            //create a entry of DocumentSummaryInformation
            DocumentSummaryInformation dsi = PropertySetFactory.CreateDocumentSummaryInformation();
            dsi.Company = strCompany;
            hssfworkbook.DocumentSummaryInformation = dsi;

            //create a entry of SummaryInformation
            SummaryInformation si = PropertySetFactory.CreateSummaryInformation();
            si.Subject = GetSubject();
            hssfworkbook.SummaryInformation = si;

            return hssfworkbook;
        }

        protected abstract ICollection<SheetData> ReadSheetDatas(out string strErrorMessage);
        protected abstract string GetSubject();

        public static ExcelWriter MakeInstance(object sensorList)
        {
            if (sensorList.GetType() == typeof(List<FireSensor>))
                return new FireSensorWriter((List<FireSensor>)sensorList);
            else if (sensorList.GetType() == typeof(List<PSMSensor>))
                return new PSMSensorWriter((List<PSMSensor>)sensorList);
            else if (sensorList.GetType() == typeof(List<EtcSensor>))
                return new EtcSensorWriter((List<EtcSensor>)sensorList);
            else if (sensorList.GetType() == typeof(List<CCTVSensor>))
                return new CCTVWriter((List<CCTVSensor>)sensorList);

            return null;
        }

        protected string ListToString<DataType>(List<DataType> list)
        {
            string str = "";

            foreach (DataType data in list)
            {
                if (str.Length == 0)
                    str = data.ToString();
                else
                    str += "," + data.ToString();
            }

            return str;
        }
    }
}
