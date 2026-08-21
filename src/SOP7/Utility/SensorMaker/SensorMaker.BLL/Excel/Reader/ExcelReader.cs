using System.Collections.Generic;
using System.IO;
using System.Text;
using ExcelDataReader;

namespace SensorMaker.BLL.Excel.Reader
{
    public enum DataMode { Fire = 0, PSM, Etc, CCTV };

    public abstract class ExcelReader
    {

        private string m_strFilePath = null;

        public abstract object Result
        {
            get;
        }
        
        public ExcelReader(string strFilePath)
        {
            m_strFilePath = strFilePath;
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
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                List<SheetData> sheetDatas = new List<SheetData>();

                using (var stream = File.Open(m_strFilePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        do
                        {
                            SheetData sheet = new SheetData(reader.Name);
                            sheetDatas.Add(sheet);

                            bool firstLine = true;
                            List<string> columnDatas = null;

                            while (reader.Read())
                            {
                                int nFieldCount = reader.FieldCount;

                                for (int i = 0; i < nFieldCount; i++)
                                {
                                    object value = reader.GetValue(i);

                                    if (value == null)
                                    {
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
                            }
                        }
                        while (reader.NextResult());
                    }
                }

                return UpdateData(sheetDatas, out strErrorMessage);
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                strErrorMessage = e.Message;
            }

            return false;
        }

        protected bool CheckDataFile(SheetData sheet, int nColumnCount, out int nRowCount, out string strErrorMessage)
        {
            strErrorMessage = null;
            nRowCount = 0;

            if (sheet.ColumnDatas.Count != nColumnCount)
            {
                strErrorMessage = "잘못된 형식의 데이터 파일입니다.";
                return false;
            }

            foreach (KeyValuePair<int, List<string>> pair in sheet.ColumnDatas)
            {
                if (nRowCount == 0)
                    nRowCount = pair.Value.Count;
                else
                {
                    if (pair.Value.Count != nRowCount)
                    {
                        strErrorMessage = "형식에 맞지않는 데이터 파일입니다.";
                        return false;
                    }
                }
            }

            return true;
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

        protected abstract bool UpdateData(List<SheetData> sheetDatas, out string strErrorMessage);

        public static ExcelReader MakeInstance(DataMode mode, string strFilePath)
        {
            if (mode == DataMode.Fire)
                return new FireSensorReader(strFilePath);
            else if (mode == DataMode.PSM)
                return new PSMSensorReader(strFilePath);
            else if (mode == DataMode.Etc)
                return new EtcSensorReader(strFilePath);
            else if (mode == DataMode.CCTV)
                return new CCTVReader(strFilePath);

            return null;
        }

        protected bool GetNotNullString(string strData, int row, int column, out string result, out string strErrorMessage)
        {
            strErrorMessage = null;
            result = null;

            if (strData == null)
            {
                strErrorMessage = string.Format("{0}번째 행, {1}번째 열의 데이터는 null이 되어서는 안됩니다.", row + 1, column + 1);
                return false;
            }

            strData = strData.Trim();

            if (strData.Length == 0)
            {
                strErrorMessage = string.Format("{0}번째 행, {1}번째 열의 데이터는 null이 되어서는 안됩니다.", row + 1, column + 1);
                return false;
            }

            result = strData;
            return true;
        }

        protected bool GetNullableString(string strData, int row, int column, out string result, out string strErrorMessage)
        {
            strErrorMessage = null;
            result = null;

            if (strData == null)
                return true;

            strData = strData.Trim();

            if (strData.Length == 0)
                return true;

            result = strData;
            return true;
        }

        // result가 null일 경우 빈문자열("")로 바꿔준다.
        protected bool GetEmptyString(string strData, int row, int column, out string result, out string strErrorMessage)
        {
            if (GetNullableString(strData, row, column, out result, out strErrorMessage) == false)
                return false;

            if (result == null)
                result = "";

            return true;
        }

        protected bool GetNotNullInt(string strData, int row, int column, out int result, out string strErrorMessage)
        {
            strErrorMessage = null;
            result = -1;

            if (strData == null)
            {
                strErrorMessage = string.Format("{0}번째 행, {1}번째 열의 데이터는 null이 되어서는 안됩니다.", row + 1, column + 1);
                return false;
            }

            strData = strData.Trim();

            if (strData.Length == 0)
            {
                strErrorMessage = string.Format("{0}번째 행, {1}번째 열의 데이터는 null이 되어서는 안됩니다.", row + 1, column + 1);
                return false;
            }

            int data;

            if (int.TryParse(strData, out data))
            {
                result = data;
                return true;
            }

            strErrorMessage = string.Format("{0}번째 행, {1}번째 열의 데이터는 정수 형태로 변환할 수 없습니다.({2})", row + 1, column + 1, strData);
            return false;
        }

        protected bool GetNullableInt(string strData, int row, int column, out int? result, out string strErrorMessage)
        {
            strErrorMessage = null;
            result = null;

            if (strData == null)
                return true;

            strData = strData.Trim();

            if (strData.Length == 0)
                return true;

            int data;

            if (int.TryParse(strData, out data))
            {
                result = data;
                return true;
            }

            strErrorMessage = string.Format("{0}번째 행, {1}번째 열의 데이터는 정수 형태로 변환할 수 없습니다.({2})", row + 1, column + 1, strData);
            return false;
        }

        protected void StringToIntList(string str, List<int> list)
        {
            if (str == null)
                return;

            string[] arr = str.Split(',');
            int nCount = arr.Length;

            int data;

            for (int i=0;i<nCount;i++)
            {
                string strData = arr[i].Trim();

                if (strData.Length == 0)
                    continue;

                if (int.TryParse(strData, out data))
                    list.Add(data);
            }
        }
    }
}
