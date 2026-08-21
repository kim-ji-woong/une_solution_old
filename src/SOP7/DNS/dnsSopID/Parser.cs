using System;
using System.Collections;
using System.Collections.Generic;

namespace dnsSopID
{
    public class Parser
    {
        public ArrayList ToArrayList(List<string> values)
        {
            ArrayList arrDatas = new ArrayList();
            int nDataType;
            List<string> strList = null;

            foreach (string strValue in values)
            {
                int nIndex = strValue.IndexOf(',');

                if (nIndex < 0)
                    continue;

                string strDataType = strValue.Substring(0, nIndex).Trim();
                string strDataValue = strValue.Substring(nIndex + 1).Trim();

                if (int.TryParse(strDataType, out nDataType) == false)
                    continue;

                ParseData(arrDatas, nDataType, strDataValue, ref strList);
            }

            return arrDatas;
        }

        public List<string> ToList(ArrayList arrDatas)
        {
            List<string> values = new List<string>();

            foreach (object data in arrDatas)
            {
                AddValue(data, values);
            }

            return values;
        }

        private void AddValue(object data, List<string> values)
        {
            if (data == null)
                AddValue(DATA_TYPE.NULL, "", values);
            else if (data is int)
                AddValue(DATA_TYPE.INT, ((int)data).ToString(), values);
            else if (data is List<int>)
                AddValue(DATA_TYPE.INT_LIST, MakeString<int>((List<int>)data), values);
            else if (data is float)
                AddValue(DATA_TYPE.FLOAT, ((float)data).ToString(), values);
            else if (data is List<float>)
                AddValue(DATA_TYPE.FLOAT_LIST, MakeString<float>((List<float>)data), values);
            else if (data is double)
                AddValue(DATA_TYPE.DOUBLE, ((double)data).ToString(), values);
            else if (data is List<double>)
                AddValue(DATA_TYPE.DOUBLE_LIST, MakeString<double>((List<double>)data), values);
            else if (data is string)
                AddValue(DATA_TYPE.STRING, (string)data, values);
            else if (data is List<string>)
                AddValue((List<string>)data, values);
            else if (data is long)
                AddValue(DATA_TYPE.LONG, ((long)data).ToString(), values);
            else if (data is List<long>)
                AddValue(DATA_TYPE.LONG_LIST, MakeString<long>((List<long>)data), values);
            else if (data is bool)
                AddValue(DATA_TYPE.BOOLEAN, ((bool)data).ToString(), values);
            else if (data is List<bool>)
                AddValue(DATA_TYPE.LONG_LIST, MakeString<bool>((List<bool>)data), values);
            else if (data is short)
                AddValue(DATA_TYPE.SHORT, ((short)data).ToString(), values);
            else if (data is List<short>)
                AddValue(DATA_TYPE.SHORT_LIST, MakeString<short>((List<short>)data), values);
            else if (data is byte)
                AddValue(DATA_TYPE.BYTE, ((byte)data).ToString(), values);
            else if (data is byte[])
                AddValue((byte[])data, values);
            else if (data is DateTime)
                AddValue((DateTime)data, values);
        }

        private string MakeString<Type>(List<Type> datas)
        {
            string strValue = "";

            foreach (Type data in datas)
            {
                if (strValue.Length == 0)
                    strValue = data.ToString();
                else
                    strValue += "\t" + data.ToString();
            }

            return strValue;
        }

        private void AddValue(DateTime time, List<string> values)
        {
            string strValue = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}",
                time.Year, time.Month, time.Day,
                time.Hour, time.Minute, time.Second);

            AddValue(DATA_TYPE.DATETIME, strValue, values);
        }

        private void AddValue(byte[] bytes, List<string> values)
        {
            string strValue = "";

            foreach (byte b in bytes)
            {
                if (strValue.Length == 0)
                    strValue = b.ToString("X2");
                else
                    strValue += "\t" + b.ToString("X2");
            }

            AddValue(DATA_TYPE.BYTE_ARRAY, strValue, values);
        }

        private void AddValue(List<string> strDatas, List<string> values)
        {
            AddValue(DATA_TYPE.STRING_LIST_BEGIN, "", values);

            foreach (string strData in strDatas)
            {
                AddValue(DATA_TYPE.STRING, strData, values);
            }

            AddValue(DATA_TYPE.STRING_LIST_END, "", values);
        }

        private void AddValue(int nDataType, string strValue, List<string> values)
        {
            values.Add(nDataType.ToString() + "," + strValue);
        }

        private bool ParseData(ArrayList arrDatas, int nDataType, string strDataValue, ref List<string> strList)
        {
            if (nDataType == DATA_TYPE.NULL)
            {
                arrDatas.Add(null);
            }
            else if (nDataType == DATA_TYPE.INT)
            {
                return ParseInt(arrDatas, strDataValue);
            }
            else if (nDataType == DATA_TYPE.INT_LIST)
            {
                return ParseIntList(arrDatas, strDataValue);
            }
            else if (nDataType == DATA_TYPE.FLOAT)
            {
                return ParseFloat(arrDatas, strDataValue);
            }
            else if (nDataType == DATA_TYPE.FLOAT_LIST)
            {
                return ParseFloatList(arrDatas, strDataValue);
            }
            else if (nDataType == DATA_TYPE.DOUBLE)
            {
                return ParseDouble(arrDatas, strDataValue);
            }
            else if (nDataType == DATA_TYPE.DOUBLE_LIST)
            {
                return ParseDoubleList(arrDatas, strDataValue);
            }
            else if (nDataType == DATA_TYPE.STRING)
            {
                if (strList == null)
                    arrDatas.Add(strDataValue);
                else
                    strList.Add(strDataValue);
            }
            else if (nDataType == DATA_TYPE.STRING_LIST_BEGIN)
            {
                strList = new List<string>();
                arrDatas.Add(strList);
            }
            else if (nDataType == DATA_TYPE.STRING_LIST_END)
            {
                strList = null;
            }
            else if (nDataType == DATA_TYPE.LONG)
            {
                return ParseLong(arrDatas, strDataValue);
            }
            else if (nDataType == DATA_TYPE.LONG_LIST)
            {
                return ParseLongList(arrDatas, strDataValue);
            }
            else if (nDataType == DATA_TYPE.BOOLEAN)
            {
                return ParseBoolean(arrDatas, strDataValue);
            }
            else if (nDataType == DATA_TYPE.BOOLEAN_LIST)
            {
                return ParseBooleanList(arrDatas, strDataValue);
            }
            else if (nDataType == DATA_TYPE.SHORT)
            {
                return ParseShort(arrDatas, strDataValue);
            }
            else if (nDataType == DATA_TYPE.SHORT_LIST)
            {
                return ParseShortList(arrDatas, strDataValue);
            }
            else if (nDataType == DATA_TYPE.BYTE)
            {
                return ParseByte(arrDatas, strDataValue);
            }
            else if (nDataType == DATA_TYPE.BYTE_ARRAY)
            {
                return ParseByteArray(arrDatas, strDataValue);
            }
            else if (nDataType == DATA_TYPE.DATETIME)
            {
                return ParseDateTime(arrDatas, strDataValue);
            }
            else
                return false;

            return true;
        }

        private bool ParseInt(ArrayList arrDatas, string strDataValue)
        {
            int data;

            if (int.TryParse(strDataValue, out data) == false)
                return false;

            arrDatas.Add(data);
            return true;
        }

        private bool ParseIntList(ArrayList arrDatas, string strDataValue)
        {
            string[] tokens = strDataValue.Split('\t');
            List<int> dataList = new List<int>();

            foreach (string strToken in tokens)
            {
                int data;

                if (int.TryParse(strToken.Trim(), out data) == false)
                    return false;

                dataList.Add(data);
            }

            arrDatas.Add(dataList);
            return true;
        }

        private bool ParseLong(ArrayList arrDatas, string strDataValue)
        {
            long data;

            if (long.TryParse(strDataValue, out data) == false)
                return false;

            arrDatas.Add(data);
            return true;
        }

        private bool ParseLongList(ArrayList arrDatas, string strDataValue)
        {
            string[] tokens = strDataValue.Split('\t');
            List<long> dataList = new List<long>();

            foreach (string strToken in tokens)
            {
                long data;

                if (long.TryParse(strToken.Trim(), out data) == false)
                    return false;

                dataList.Add(data);
            }

            arrDatas.Add(dataList);
            return true;
        }

        private bool ParseShort(ArrayList arrDatas, string strDataValue)
        {
            short data;

            if (short.TryParse(strDataValue, out data) == false)
                return false;

            arrDatas.Add(data);
            return true;
        }

        private bool ParseShortList(ArrayList arrDatas, string strDataValue)
        {
            string[] tokens = strDataValue.Split('\t');
            List<short> dataList = new List<short>();

            foreach (string strToken in tokens)
            {
                short data;

                if (short.TryParse(strToken.Trim(), out data) == false)
                    return false;

                dataList.Add(data);
            }

            arrDatas.Add(dataList);
            return true;
        }

        private bool ParseFloat(ArrayList arrDatas, string strDataValue)
        {
            float data;

            if (float.TryParse(strDataValue, out data) == false)
                return false;

            arrDatas.Add(data);
            return true;
        }

        private bool ParseFloatList(ArrayList arrDatas, string strDataValue)
        {
            string[] tokens = strDataValue.Split('\t');
            List<float> dataList = new List<float>();

            foreach (string strToken in tokens)
            {
                float data;

                if (float.TryParse(strToken.Trim(), out data) == false)
                    return false;

                dataList.Add(data);
            }

            arrDatas.Add(dataList);
            return true;
        }

        private bool ParseDouble(ArrayList arrDatas, string strDataValue)
        {
            double data;

            if (double.TryParse(strDataValue, out data) == false)
                return false;

            arrDatas.Add(data);
            return true;
        }

        private bool ParseDoubleList(ArrayList arrDatas, string strDataValue)
        {
            string[] tokens = strDataValue.Split('\t');
            List<double> dataList = new List<double>();

            foreach (string strToken in tokens)
            {
                double data;

                if (double.TryParse(strToken.Trim(), out data) == false)
                    return false;

                dataList.Add(data);
            }

            arrDatas.Add(dataList);
            return true;
        }

        private bool ParseBoolean(ArrayList arrDatas, string strDataValue)
        {
            if (strDataValue == "1")
                arrDatas.Add(true);
            else if (strDataValue == "0")
                arrDatas.Add(false);
            else
                return false;

            return true;
        }

        private bool ParseBooleanList(ArrayList arrDatas, string strDataValue)
        {
            string[] tokens = strDataValue.Split('\t');
            List<bool> dataList = new List<bool>();

            foreach (string strToken in tokens)
            {
                string str = strToken.Trim();

                if (str == "1")
                    dataList.Add(true);
                else if (str == "0")
                    dataList.Add(false);
                else
                    return false;
            }

            arrDatas.Add(dataList);
            return true;
        }

        private bool ParseByte(ArrayList arrDatas, string strDataValue)
        {
            byte b;

            if (byte.TryParse(strDataValue, System.Globalization.NumberStyles.HexNumber, null, out b) == false)
                return false;

            arrDatas.Add(b);
            return true;
        }

        private bool ParseByteArray(ArrayList arrDatas, string strDataValue)
        {
            string[] tokens = strDataValue.Split('\t');
            int nTokenCount = tokens.Length;

            if (nTokenCount == 0)
            {
                arrDatas.Add(null);
                return true;
            }

            byte[] bytes = new byte[nTokenCount];

            for (int i=0;i<nTokenCount;i++)
            {
                string strToken = tokens[i].Trim();
                byte b;

                if (byte.TryParse(strToken, System.Globalization.NumberStyles.HexNumber, null, out b) == false)
                    return false;

                bytes[i] = b;
            }

            arrDatas.Add(bytes);
            return true;
        }

        private bool ParseDateTime(ArrayList arrDatas, string strDataValue)
        {
            DateTime time;

            if (DateTime.TryParse(strDataValue, out time) == false)
                return false;

            arrDatas.Add(time);
            return true;
        }
    }
}
