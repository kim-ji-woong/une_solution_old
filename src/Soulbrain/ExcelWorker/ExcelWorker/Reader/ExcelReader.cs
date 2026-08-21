using System.IO;
using System.Collections.Generic;
using System.Configuration;
using ExcelDataReader;
using SDMS.DAL;
using SDMS.Model;

namespace ExcelWorker.Reader
{
    public abstract class ExcelReader
    {
        private string m_strFilePath = "";
        protected DataManager m_dataManager = null;

        public ExcelReader(string strFilePath)
        {
            m_strFilePath = strFilePath;
            m_dataManager = InitDataManager();
        }

        public static DataManager InitDataManager()
        {
            int nSiteID, nDBType;
            string strSiteID = ConfigurationManager.AppSettings.Get("siteid");
            string strDBType = ConfigurationManager.AppSettings.Get("dbType");

            if (strSiteID == null || strDBType == null)
                return null;

            if (int.TryParse(strSiteID, out nSiteID) == false || int.TryParse(strDBType, out nDBType) == false)
                return null;

            string strWebServerURL = ConfigurationManager.AppSettings.Get("webserverURL");
            string strDBName = ConfigurationManager.AppSettings.Get("dbName");

            if (strWebServerURL == null || strDBName == null)
                return null;

            DataManager dataManager = new DataManager(strDBName, nDBType, nSiteID, strWebServerURL);
            return dataManager;
        }

        public bool Run()
        {
            if (m_strFilePath == null || m_strFilePath.Length == 0)
                return false;

            try
            {
                List<SheetData> sheetDatas = new List<SheetData>();
                //StreamWriter writer = new StreamWriter("C:/Temp/aaa.txt", false, System.Text.Encoding.UTF8);

                using (var stream = File.Open(m_strFilePath, FileMode.Open, FileAccess.Read))
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
                return UpdateData(sheetDatas);
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
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

        protected abstract bool UpdateData(List<SheetData> sheetDatas);

        public static ExcelReader MakeInstance(DataMode mode, string strPath)
        {
            if (mode == DataMode.FacilityInfo)
                return new FacilityInfoReader(strPath);
            else if (mode == DataMode.BuildingData)
                return new BuildingReader(strPath);
            else if (mode == DataMode.BuildingGroupData)
                return new BuildingGroupReader(strPath);
            else if (mode == DataMode.RegularMember)
                return new RegularMemberReader(strPath);

            return null;
        }
    }
}
