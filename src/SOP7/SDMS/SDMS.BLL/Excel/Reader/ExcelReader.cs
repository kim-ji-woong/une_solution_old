using ExcelDataReader;
using SDMS.DAL;
using SDMS.IDAL;
using SDMS.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SDMS.BLL.Excel.Reader
{
    public abstract class ExcelReader
    {
        private string m_strFilePath = null;
        protected IDataManager m_dataManager = null;

        public ExcelReader(string strFilePath, IDataManager dataManager)
        {
            m_strFilePath = strFilePath;
            m_dataManager = dataManager;
        }

        public bool Run(out string strErrorMessage)
        {
            if (m_strFilePath == null)
            {
                strErrorMessage = "m_strFilePath 값이 null";
                return false;
            }

            try
            {
                List<SheetData> sheetDatas = new List<SheetData>();
                //StreamWriter writer = new StreamWriter("C:/Temp/aaa.txt", false, System.Text.Encoding.UTF8);

                using (var stream = File.Open(m_strFilePath, FileMode.Open, FileAccess.Read))
                //using (var stream = m_file)
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        do
                        {
                            SheetData sheet = new SheetData(reader.Name);
                            sheetDatas.Add(sheet);
                            //writer.WriteLine("[" + reader.Name + "]");

                            bool firstLine = true;
                            List<string> columnDatas = null;

                            while (reader.Read())
                            {
                                int nFieldCount = reader.FieldCount;
                                //string strLine = "";

                                for (int i = 0; i < nFieldCount; i++)
                                {
                                    object value = reader.GetValue(i);

                                    if (value == null)
                                    {
                                        /*if (strLine.Length == 0)
                                            strLine = string.Format("[{0}]()", i);
                                        else
                                            strLine += string.Format(", [{0}]()", i);*/

                                        if (firstLine)
                                            sheet.Titles[i] = null;
                                        else
                                        {
                                            if (sheet.ColumnDatas.TryGetValue(i, out columnDatas) == false)
                                            {
                                                columnDatas = new List<string>();
                                                sheet.ColumnDatas[i] = columnDatas;
                                            }

                                            columnDatas.Add(null);
                                        }
                                    }
                                    else
                                    {
                                        /*if (strLine.Length == 0)
                                            strLine = string.Format("[{0}]({1})", i, value.ToString());
                                        else
                                            strLine += string.Format(", [{0}]({1})", i, value.ToString());*/

                                        if (firstLine)
                                            sheet.Titles[i] = value.ToString();
                                        else
                                        {
                                            if (sheet.ColumnDatas.TryGetValue(i, out columnDatas) == false)
                                            {
                                                columnDatas = new List<string>();
                                                sheet.ColumnDatas[i] = columnDatas;
                                            }

                                            columnDatas.Add(value.ToString());
                                        }
                                    }
                                }

                                firstLine = false;
                                //System.Diagnostics.Trace.WriteLine(strLine);
                                //writer.WriteLine(strLine);
                            }
                        }
                        while (reader.NextResult());
                    }
                }

                //writer.Close();
                return UpdateData(sheetDatas, out strErrorMessage);
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                strErrorMessage = e.Message;
            }

            return false;
        }

        protected int[] GetColumnCounts(SheetData sheetData, int min, int max, out int maxColumnCount)
        {
            maxColumnCount = 0;

            foreach (KeyValuePair<int, List<string>> pair in sheetData.ColumnDatas)
            {
                int nColumnCount = pair.Value.Count;

                if (maxColumnCount < nColumnCount)
                    maxColumnCount = nColumnCount;
            }

            if (min > max)
                return null;

            List<string> datas;
            int[] arrColumnCount = new int[max - min + 1];

            for (int i = min; i <= max; i++)
            {
                if (sheetData.ColumnDatas.TryGetValue(i, out datas))
                    arrColumnCount[i - min] = datas.Count;
                else
                    arrColumnCount[i - min] = 0;
            }

            return arrColumnCount;
        }

        public static Dictionary<int, T> ToDictionary<T>(List<T> datas) where T : IIDObject
        {
            Dictionary<int, T> dicDatas = new Dictionary<int, T>();

            foreach (T data in datas)
            {
                dicDatas[data.ID] = data;
            }

            return dicDatas;
        }

        protected abstract bool UpdateData(List<SheetData> sheetDatas, out string strErrorMessage);

        public static ExcelReader MakeInstance(DataMode mode, string strFilePath, IDataManager dataManager)
        {
            if (mode == DataMode.FacilityInfo)
                return new FacilityInfoReader(strFilePath, dataManager);
            else if (mode == DataMode.BuildingData)
                return new BuildingReader(strFilePath, dataManager);
            else if (mode == DataMode.BuildingGroupData)
                return new BuildingGroupReader(strFilePath, dataManager);
            else if (mode == DataMode.RegularTeamData)
                return new RegularMemberReader(strFilePath, dataManager);

            return null;
        }
    }
}
