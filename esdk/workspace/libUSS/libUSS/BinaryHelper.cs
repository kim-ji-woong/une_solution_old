using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace libUSS
{
    public class BinaryHelper
    {
        public static byte[] MakeBytes(int data)
        {
            int nDataLength = sizeof(int);
            byte[] bytes = new byte[3 + nDataLength];

            bytes[0] = DataType.INTEGER;

            short nArraySize = 1;
            byte[] lengthBytes = BitConverter.GetBytes(nArraySize);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(int[] datas)
        {
            short nArraySize = (short)datas.Count();
            int IntSize = sizeof(int);
            int nDataLength = IntSize * nArraySize;
            byte[] bytes = new byte[3 + nDataLength];

            bytes[0] = DataType.INTEGER;

            byte[] lengthBytes = BitConverter.GetBytes(nArraySize);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            for (int i = 0; i < nArraySize; i++)
            {
                int data = datas[i];
                byte[] dataBytes = BitConverter.GetBytes(data);

                for (int j = 0; j < IntSize; j++)
                {
                    bytes[i * IntSize + j + 1 + nCount] = dataBytes[j];
                }
            }

            return bytes;
        }

        public static byte[] MakeBytes(long data)
        {
            int nDataLength = sizeof(long);
            byte[] bytes = new byte[3 + nDataLength];

            bytes[0] = DataType.LONG;

            short nArraySize = 1;
            byte[] lengthBytes = BitConverter.GetBytes(nArraySize);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(long[] datas)
        {
            short nArraySize = (short)datas.Count();
            int LongSize = sizeof(long);
            int nDataLength = LongSize * nArraySize;
            byte[] bytes = new byte[3 + nDataLength];

            bytes[0] = DataType.LONG;

            byte[] lengthBytes = BitConverter.GetBytes(nArraySize);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            for (int i = 0; i < nArraySize; i++)
            {
                long data = datas[i];
                byte[] dataBytes = BitConverter.GetBytes(data);

                for (int j = 0; j < LongSize; j++)
                {
                    bytes[i * LongSize + j + 1 + nCount] = dataBytes[j];
                }
            }

            return bytes;
        }

        public static byte[] MakeBytes(float data)
        {
            int nDataLength = sizeof(float);
            byte[] bytes = new byte[3 + nDataLength];

            bytes[0] = DataType.FLOAT;

            short nArraySize = 1;
            byte[] lengthBytes = BitConverter.GetBytes(nArraySize);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(float[] datas)
        {
            short nArraySize = (short)datas.Count();
            int FloatSize = sizeof(float);
            int nDataLength = FloatSize * nArraySize;
            byte[] bytes = new byte[3 + nDataLength];

            bytes[0] = DataType.FLOAT;

            byte[] lengthBytes = BitConverter.GetBytes(nArraySize);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            for (int i = 0; i < nArraySize; i++)
            {
                float data = datas[i];
                byte[] dataBytes = BitConverter.GetBytes(data);

                for (int j = 0; j < FloatSize; j++)
                {
                    bytes[i * FloatSize + j + 1 + nCount] = dataBytes[j];
                }
            }

            return bytes;
        }

        public static byte[] MakeBytes(double data)
        {
            int nDataLength = sizeof(double);
            byte[] bytes = new byte[3 + nDataLength];

            bytes[0] = DataType.DOUBLE;

            short nArraySize = 1;
            byte[] lengthBytes = BitConverter.GetBytes(nArraySize);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(double[] datas)
        {
            short nArraySize = (short)datas.Count();
            int DoubleSize = sizeof(double);
            int nDataLength = DoubleSize * nArraySize;
            byte[] bytes = new byte[3 + nDataLength];

            bytes[0] = DataType.DOUBLE;

            byte[] lengthBytes = BitConverter.GetBytes(nArraySize);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            for (int i = 0; i < nArraySize; i++)
            {
                double data = datas[i];
                byte[] dataBytes = BitConverter.GetBytes(data);

                for (int j = 0; j < DoubleSize; j++)
                {
                    bytes[i * DoubleSize + j + 1 + nCount] = dataBytes[j];
                }
            }

            return bytes;
        }

        public static byte[] MakeBytes(string data)
        {
            UTF8Encoding enc = new UTF8Encoding();
            byte[] datas = enc.GetBytes(data);

            short nDataLength = (short)datas.Length;

            byte[] bytes = new byte[3 + nDataLength];

            bytes[0] = DataType.STRING;

            byte[] lengthBytes = BitConverter.GetBytes(nDataLength);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = datas[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(short data)
        {
            int nDataLength = sizeof(short);
            byte[] bytes = new byte[3 + nDataLength];

            bytes[0] = DataType.SHORT;

            short nArraySize = 1;
            byte[] lengthBytes = BitConverter.GetBytes(nArraySize);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = BitConverter.GetBytes(data);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(short[] datas)
        {
            short nArraySize = (short)datas.Count();
            int ShortSize = sizeof(short);
            int nDataLength = ShortSize * nArraySize;
            byte[] bytes = new byte[3 + nDataLength];

            bytes[0] = DataType.SHORT;

            byte[] lengthBytes = BitConverter.GetBytes(nArraySize);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            for (int i = 0; i < nArraySize; i++)
            {
                short data = datas[i];
                byte[] dataBytes = BitConverter.GetBytes(data);

                for (int j = 0; j < ShortSize; j++)
                {
                    bytes[i * ShortSize + j + 1 + nCount] = dataBytes[j];
                }
            }

            return bytes;
        }

        private static byte[] DateTimeToBytes(DateTime time)
        {
            byte[] bytes = new byte[8];

            bytes[0] = (byte)(time.Year - 1900);
            bytes[1] = (byte)time.Month;
            bytes[2] = (byte)time.Day;
            bytes[3] = (byte)time.Hour;
            bytes[4] = (byte)time.Minute;
            bytes[5] = (byte)time.Second;

            byte[] milliSeconds = BitConverter.GetBytes((short)time.Millisecond);
            bytes[6] = milliSeconds[0];
            bytes[7] = milliSeconds[1];

            return bytes;
        }

        private static DateTime ToDateTime(byte[] bytes, int nIndex)
        {
            int nYear = (int)bytes[nIndex] + 1900;
            int nMon = (int)bytes[nIndex + 1];
            int nDay = (int)bytes[nIndex + 2];
            int nHour = (int)bytes[nIndex + 3];
            int nMin = (int)bytes[nIndex + 4];
            int nSec = (int)bytes[nIndex + 5];
            int nMilliSecond = (int)BitConverter.ToInt16(bytes, nIndex + 6);

            return new DateTime(nYear, nMon, nDay, nHour, nMin, nSec, nMilliSecond);
        }

        public static byte[] MakeBytes(DateTime time)
        {
            int nDataLength = 8;
            byte[] bytes = new byte[3 + nDataLength];

            bytes[0] = DataType.DATETIME;

            short nArraySize = 1;
            byte[] lengthBytes = BitConverter.GetBytes(nArraySize);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = DateTimeToBytes(time);

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(DateTime[] datas)
        {
            short nArraySize = (short)datas.Count();
            int DateTimeSize = 8;
            int nDataLength = DateTimeSize * nArraySize;
            byte[] bytes = new byte[3 + nDataLength];

            bytes[0] = DataType.DATETIME;

            byte[] lengthBytes = BitConverter.GetBytes(nArraySize);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            for (int i = 0; i < nArraySize; i++)
            {
                DateTime time = datas[i];
                byte[] dataBytes = DateTimeToBytes(time);

                for (int j = 0; j < DateTimeSize; j++)
                {
                    bytes[i * DateTimeSize + j + 1 + nCount] = dataBytes[j];
                }
            }

            return bytes;
        }

        public static byte[] MakeBytes(byte data)
        {
            int nDataLength = sizeof(byte);
            byte[] bytes = new byte[3 + nDataLength];

            bytes[0] = DataType.BYTE;

            short nArraySize = 1;
            byte[] lengthBytes = BitConverter.GetBytes(nArraySize);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            byte[] dataBytes = new byte[1] { data };

            for (int i = 0; i < nDataLength; i++)
            {
                bytes[i + 1 + nCount] = dataBytes[i];
            }

            return bytes;
        }

        public static byte[] MakeBytes(byte[] datas)
        {
            short nArraySize = (short)datas.Count();
            int ByteSize = sizeof(byte);
            int nDataLength = ByteSize * nArraySize;
            byte[] bytes = new byte[3 + nDataLength];

            bytes[0] = DataType.BYTE;

            byte[] lengthBytes = BitConverter.GetBytes(nArraySize);

            int nCount = lengthBytes.Length;

            for (int i = 0; i < nCount; i++)
                bytes[i + 1] = lengthBytes[i];

            for (int i = 0; i < nArraySize; i++)
            {
                byte data = datas[i];
                bytes[i + 1 + nCount] = data;
            }

            return bytes;
        }

        public static byte[] MakeBytes(short nHeader, ArrayList arrDatas)
        {
            short nChunkCount = arrDatas == null ? (short)0 : (short)arrDatas.Count;

            ArrayList arrBytes = new ArrayList();
            int nBytesCount = 0;

            for (int i = 0; i < nChunkCount; i++)
            {
                object data = arrDatas[i];
                Type type = data.GetType();
                byte[] bytes = null;

                if (type == typeof(int))
                    bytes = MakeBytes((int)data);
                else if (type == typeof(int[]))
                    bytes = MakeBytes((int[])data);
                else if (type == typeof(long))
                    bytes = MakeBytes((long)data);
                else if (type == typeof(long[]))
                    bytes = MakeBytes((long[])data);
                else if (type == typeof(float))
                    bytes = MakeBytes((float)data);
                else if (type == typeof(float[]))
                    bytes = MakeBytes((float[])data);
                else if (type == typeof(double))
                    bytes = MakeBytes((double)data);
                else if (type == typeof(double[]))
                    bytes = MakeBytes((double[])data);
                else if (type == typeof(short))
                    bytes = MakeBytes((short)data);
                else if (type == typeof(short[]))
                    bytes = MakeBytes((short[])data);
                else if (type == typeof(byte))
                    bytes = MakeBytes((byte)data);
                else if (type == typeof(byte[]))
                    bytes = MakeBytes((byte[])data);
                else if (type == typeof(DateTime))
                    bytes = MakeBytes((DateTime)data);
                else if (type == typeof(DateTime[]))
                    bytes = MakeBytes((DateTime[])data);
                else if (type == typeof(string))
                    bytes = MakeBytes((string)data);
                else
                    return null;

                nBytesCount += bytes.Length;
                arrBytes.Add(bytes);
            }

            byte[] _bytes = new byte[4 + nBytesCount];
            byte[] headerBytes = BitConverter.GetBytes(nHeader);
            byte[] lengthBytes = BitConverter.GetBytes(nChunkCount);

            _bytes[0] = headerBytes[0];
            _bytes[1] = headerBytes[1];
            _bytes[2] = lengthBytes[0];
            _bytes[3] = lengthBytes[1];

            int nIndex = 4;

            foreach (byte[] bytes in arrBytes)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    _bytes[nIndex + i] = bytes[i];
                }

                nIndex += bytes.Length;
            }

            return _bytes;
        }

        private static bool ReadType(byte[] bytes, int nBytesLength, ref int nIndex, int nDataSize, out short nArrSize, out bool isNullData)
        {
            isNullData = false;
            nArrSize = 0;

            if (nBytesLength < nIndex + 3)
                return false;

            nArrSize = BitConverter.ToInt16(bytes, nIndex + 1);

            if (nArrSize < 0)
                return false;
            else if (nArrSize > 0)
            {
                if (nBytesLength < nIndex + nDataSize * nArrSize)
                    return false;

                nIndex += 3 + nDataSize * nArrSize;
            }
            else
            {
                isNullData = true;
                nIndex += 3;
            }

            return true;
        }
        /*private static bool ReadType(byte[] bytes, int nBytesLength, ref int nIndex, int nTotalLength, out bool isNullData)
        {
            isNullData = false;

            if (nBytesLength < nIndex + 5)
                return false;

            short nDataLength = BitConverter.ToInt16(bytes, nIndex + 1);

            if (nDataLength < 0)
                return false;
            else if (nDataLength > 0)
            {
                if (nBytesLength < nIndex + nTotalLength)
                    return false;

                nIndex += nTotalLength;
            }
            else
            {
                isNullData = true;
                nIndex += 5;
            }

            return true;
        }*/

        public static ArrayList ReadBytes(byte[] bytes, out short nHeader)
        {
            nHeader = 0;

            int nLength = bytes.Length;

            if (nLength < 4)
                return null;

            nHeader = BitConverter.ToInt16(bytes, 0);
            short nChunkCount = BitConverter.ToInt16(bytes, 2);

            ArrayList arrResult = new ArrayList();
            int nIndex = 4;
            short nArrSize;
            bool isNullData;

            for (int i = 0; i < nChunkCount; i++)
            {
                if (nLength <= nIndex)
                    return null;

                byte type = bytes[nIndex];
                int nPrevIndex = nIndex + 3;

                if (type == DataType.INTEGER)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, sizeof(int), out nArrSize, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        if (nArrSize == 1)
                        {
                            int nData = BitConverter.ToInt32(bytes, nPrevIndex);
                            arrResult.Add(nData);
                        }
                        else if (nArrSize > 1)
                        {
                            int[] datas = new int[nArrSize];

                            for (int j=0;j<nArrSize;j++)
                            {
                                int nData = BitConverter.ToInt32(bytes, nPrevIndex + j * sizeof(int));
                                datas[j] = nData;
                            }

                            arrResult.Add(datas);
                        }
                    }
                }
                else if (type == DataType.FLOAT)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, sizeof(float), out nArrSize, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        if (nArrSize == 1)
                        {
                            float fData = BitConverter.ToSingle(bytes, nPrevIndex);
                            arrResult.Add(fData);
                        }
                        else if (nArrSize > 1)
                        {
                            float[] datas = new float[nArrSize];

                            for (int j = 0; j < nArrSize; j++)
                            {
                                float fData = BitConverter.ToSingle(bytes, nPrevIndex + j * sizeof(float));
                                datas[j] = fData;
                            }

                            arrResult.Add(datas);
                        }
                    }
                }
                else if (type == DataType.DOUBLE)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, sizeof(double), out nArrSize, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        if (nArrSize == 1)
                        {
                            double dData = BitConverter.ToDouble(bytes, nPrevIndex);
                            arrResult.Add(dData);
                        }
                        else if (nArrSize > 1)
                        {
                            double[] datas = new double[nArrSize];

                            for (int j = 0; j < nArrSize; j++)
                            {
                                double dData = BitConverter.ToDouble(bytes, nPrevIndex + j * sizeof(double));
                                datas[j] = dData;
                            }

                            arrResult.Add(datas);
                        }
                    }
                }
                else if (type == DataType.LONG)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, sizeof(long), out nArrSize, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        if (nArrSize == 1)
                        {
                            long lData = BitConverter.ToInt64(bytes, nPrevIndex);
                            arrResult.Add(lData);
                        }
                        else if (nArrSize > 1)
                        {
                            long[] datas = new long[nArrSize];

                            for (int j = 0; j < nArrSize; j++)
                            {
                                long lData = BitConverter.ToInt64(bytes, nPrevIndex + j * sizeof(long));
                                datas[j] = lData;
                            }

                            arrResult.Add(datas);
                        }
                    }
                }
                else if (type == DataType.SHORT)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, sizeof(short), out nArrSize, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        if (nArrSize == 1)
                        {
                            short sData = BitConverter.ToInt16(bytes, nPrevIndex);
                            arrResult.Add(sData);
                        }
                        else if (nArrSize > 1)
                        {
                            short[] datas = new short[nArrSize];

                            for (int j = 0; j < nArrSize; j++)
                            {
                                short sData = BitConverter.ToInt16(bytes, nPrevIndex + j * sizeof(short));
                                datas[j] = sData;
                            }

                            arrResult.Add(datas);
                        }
                    }
                }
                else if (type == DataType.BYTE)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, sizeof(byte), out nArrSize, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        if (nArrSize == 1)
                        {
                            byte data = bytes[nPrevIndex];
                            arrResult.Add(data);
                        }
                        else if (nArrSize > 1)
                        {
                            byte[] datas = new byte[nArrSize];

                            for (int j = 0; j < nArrSize; j++)
                            {
                                byte data = bytes[nPrevIndex + j];
                                datas[j] = data;
                            }

                            arrResult.Add(datas);
                        }
                    }
                }
                else if (type == DataType.DATETIME)
                {
                    if (!ReadType(bytes, nLength, ref nIndex, 8, out nArrSize, out isNullData))
                        return null;

                    if (!isNullData)
                    {
                        if (nArrSize == 1)
                        {
                            DateTime time = ToDateTime(bytes, nPrevIndex);
                            arrResult.Add(time);
                        }
                        else if (nArrSize > 1)
                        {
                            DateTime[] times = new DateTime[nArrSize];

                            for (int j = 0; j < nArrSize; j++)
                            {
                                DateTime time = ToDateTime(bytes, nPrevIndex + j * 8);
                                times[j] = time;
                            }

                            arrResult.Add(times);
                        }
                    }
                }
                else if (type == DataType.STRING)
                {
                    if (nLength < nIndex + 3)
                        return null;

                    short nDataLength = BitConverter.ToInt16(bytes, nIndex + 1);

                    if (nDataLength < 0)
                        return null;
                    else if (nDataLength > 0)
                    {
                        if (nLength < nIndex + 3 + nDataLength)
                            return null;

                        string strData = Encoding.UTF8.GetString(bytes, nIndex + 3, nDataLength);
                        arrResult.Add(strData);

                        nIndex += 3 + nDataLength;
                    }
                    else
                    {
                        arrResult.Add("");
                        nIndex += 3;
                    }
                }
                else
                    return null;
            }

            return arrResult;
        }
    }
}
