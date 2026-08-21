using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace libShapeFile
{
    public class ShapeInfo
    {
        #region Language Driver To Code Page Utils

        private struct LD_CP
        {
            public int ldid;
            public int codepage;
            public LD_CP(int ld, int cp) { ldid = ld; codepage = cp; }
        }
        private static LD_CP[] LD_CP_LU = new LD_CP[]{
            new LD_CP(0x00,-1),     new LD_CP(0x01,437),
            new LD_CP(0x02,850),    new LD_CP(0x03,1252),
            new LD_CP(0x04,-1),     new LD_CP(0x05,-1),
            new LD_CP(0x06,-1),     new LD_CP(0x07,-1),
            new LD_CP(0x08,865),    new LD_CP(0x09,437),
            new LD_CP(0x0A,850),    new LD_CP(0x0B,437),
            new LD_CP(0x0C,-1),     new LD_CP(0x0D,437),
            new LD_CP(0x0E,850),    new LD_CP(0x0F,437),
            
            new LD_CP(0x10,850),    new LD_CP(0x11,437),
            new LD_CP(0x12,850),    new LD_CP(0x13,932),
            new LD_CP(0x14,850),    new LD_CP(0x15,437),
            new LD_CP(0x16,850),    new LD_CP(0x17,865),
            new LD_CP(0x18,437),    new LD_CP(0x19,437),
            new LD_CP(0x1A,850),    new LD_CP(0x1B,437),
            new LD_CP(0x1C,863),    new LD_CP(0x1D,850),
            new LD_CP(0x1E,-1),     new LD_CP(0x1F,852),
            
            new LD_CP(0x20,-1),     new LD_CP(0x21,-1),
            new LD_CP(0x22,852),    new LD_CP(0x23,852),
            new LD_CP(0x24,860),    new LD_CP(0x25,850),
            new LD_CP(0x26,866),    new LD_CP(0x27,-1),
            new LD_CP(0x28,-1),     new LD_CP(0x29,-1),
            new LD_CP(0x2A,-1),     new LD_CP(0x2B,-1),
            new LD_CP(0x2C,-1),     new LD_CP(0x2D,-1),
            new LD_CP(0x2E,-1),     new LD_CP(0x2F,-1),

            new LD_CP(0x30,-1),     new LD_CP(0x31,-1),
            new LD_CP(0x32,-1),     new LD_CP(0x33,-1),
            new LD_CP(0x34,-1),     new LD_CP(0x35,-1),
            new LD_CP(0x36,-1),     new LD_CP(0x37,850),
            new LD_CP(0x38,-1),     new LD_CP(0x39,-1),
            new LD_CP(0x3A,-1),     new LD_CP(0x3B,-1),
            new LD_CP(0x3C,-1),     new LD_CP(0x3D,-1),
            new LD_CP(0x3E,-1),     new LD_CP(0x3F,-1),

            new LD_CP(0x40,852),    new LD_CP(0x41,-1),
            new LD_CP(0x42,-1),     new LD_CP(0x43,-1),
            new LD_CP(0x44,-1),     new LD_CP(0x45,-1),
            new LD_CP(0x46,-1),     new LD_CP(0x47,-1),
            new LD_CP(0x48,-1),     new LD_CP(0x49,-1),
            new LD_CP(0x4A,-1),     new LD_CP(0x4B,-1),
            new LD_CP(0x4C,-1),     new LD_CP(0x4D,936),
            new LD_CP(0x4E,949),    new LD_CP(0x4F,950),

            new LD_CP(0x50,874),    new LD_CP(0x51,-1),
            new LD_CP(0x52,-1),     new LD_CP(0x53,-1),
            new LD_CP(0x54,-1),     new LD_CP(0x55,-1),
            new LD_CP(0x56,-1),     new LD_CP(0x57,1252),
            new LD_CP(0x58,1252),   new LD_CP(0x59,1252),
            new LD_CP(0x5A,-1),     new LD_CP(0x5B,-1),
            new LD_CP(0x5C,-1),     new LD_CP(0x5D,936),
            new LD_CP(0x5E,949),    new LD_CP(0x5F,950),

            new LD_CP(0x60,-1),     new LD_CP(0x61,-1),
            new LD_CP(0x62,-1),     new LD_CP(0x63,-1),
            new LD_CP(0x64,852),    new LD_CP(0x65,866),
            new LD_CP(0x66,865),    new LD_CP(0x67,861),
            //new LD_CP(0x68,895),    new LD_CP(0x69,620),    /*??*/
            new LD_CP(0x68,-1),    new LD_CP(0x69,-1),
            new LD_CP(0x6A,737),    new LD_CP(0x6B,857),
            new LD_CP(0x6C,863),    new LD_CP(0x6D,-1),
            new LD_CP(0x6E,-1),     new LD_CP(0x6F,-1),

            new LD_CP(0x70,-1),     new LD_CP(0x71,-1),
            new LD_CP(0x72,-1),     new LD_CP(0x73,-1),
            new LD_CP(0x74,-1),     new LD_CP(0x75,-1),
            new LD_CP(0x76,-1),     new LD_CP(0x77,-1),
            new LD_CP(0x78,950),    new LD_CP(0x79,949),
            new LD_CP(0x7A,936),    new LD_CP(0x7B,932),
            new LD_CP(0x7C,874),    new LD_CP(0x7D,1255),   /*??*/
            new LD_CP(0x7E,1256),   new LD_CP(0x7F,-1),     /*??*/

            new LD_CP(0x80,-1),     new LD_CP(0x81,-1),
            new LD_CP(0x82,-1),     new LD_CP(0x83,-1),
            new LD_CP(0x84,-1),     new LD_CP(0x85,-1),
            new LD_CP(0x86,737),    new LD_CP(0x87,852),
            new LD_CP(0x88,857),    new LD_CP(0x89,-1),
            new LD_CP(0x8A,-1),     new LD_CP(0x8B,-1),
            new LD_CP(0x8C,-1),     new LD_CP(0x8D,-1),
            new LD_CP(0x8E,-1),     new LD_CP(0x8F,-1),

            new LD_CP(0x90,-1),     new LD_CP(0x91,-1),
            new LD_CP(0x92,-1),     new LD_CP(0x93,-1),
            new LD_CP(0x94,-1),     new LD_CP(0x95,-1),
            new LD_CP(0x96,-1),     new LD_CP(0x97,10029),  /*??*/
            new LD_CP(0x98,-1),     new LD_CP(0x99,-1),
            new LD_CP(0x9A,-1),     new LD_CP(0x9B,-1),
            new LD_CP(0x9C,-1),     new LD_CP(0x9D,-1),
            new LD_CP(0x9E,-1),     new LD_CP(0x9F,-1),

            new LD_CP(0xA0,-1),     new LD_CP(0xA1,-1),
            new LD_CP(0xA2,-1),     new LD_CP(0xA3,-1),
            new LD_CP(0xA4,-1),     new LD_CP(0xA5,-1),
            new LD_CP(0xA6,-1),     new LD_CP(0xA7,-1),
            new LD_CP(0xA8,-1),     new LD_CP(0xA9,-1),
            new LD_CP(0xAA,-1),     new LD_CP(0xAB,-1),
            new LD_CP(0xAC,-1),     new LD_CP(0xAD,-1),
            new LD_CP(0xAE,-1),     new LD_CP(0xAF,-1),

            new LD_CP(0xB0,-1),     new LD_CP(0xB1,-1),
            new LD_CP(0xB2,-1),     new LD_CP(0xB3,-1),
            new LD_CP(0xB4,-1),     new LD_CP(0xB5,-1),
            new LD_CP(0xB6,-1),     new LD_CP(0xB7,-1),
            new LD_CP(0xB8,-1),     new LD_CP(0xB9,-1),
            new LD_CP(0xBA,-1),     new LD_CP(0xBB,-1),
            new LD_CP(0xBC,-1),     new LD_CP(0xBD,-1),
            new LD_CP(0xBE,-1),     new LD_CP(0xBF,-1),

            new LD_CP(0xC0,-1),     new LD_CP(0xC1,-1),
            new LD_CP(0xC2,-1),     new LD_CP(0xC3,-1),
            new LD_CP(0xC4,-1),     new LD_CP(0xC5,-1),
            new LD_CP(0xC6,-1),     new LD_CP(0xC7,-1),
            new LD_CP(0xC8,1250),   new LD_CP(0xC9,1251),
            new LD_CP(0xCA,1254),   new LD_CP(0xCB,1253),
            new LD_CP(0xCC,1257),   new LD_CP(0xCD,-1),
            new LD_CP(0xCE,-1),     new LD_CP(0xCF,-1),
            
            new LD_CP(0xD0,-1),     new LD_CP(0xD1,-1),
            new LD_CP(0xD2,-1),     new LD_CP(0xD3,-1),
            new LD_CP(0xD4,-1),     new LD_CP(0xD5,-1),
            new LD_CP(0xD6,-1),     new LD_CP(0xD7,-1),
            new LD_CP(0xD8,-1),     new LD_CP(0xD9,-1),
            new LD_CP(0xDA,-1),     new LD_CP(0xDB,-1),
            new LD_CP(0xDC,-1),     new LD_CP(0xDD,-1),
            new LD_CP(0xDE,-1),     new LD_CP(0xDF,-1),

            new LD_CP(0xE0,-1),     new LD_CP(0xE1,-1),
            new LD_CP(0xE2,-1),     new LD_CP(0xE3,-1),
            new LD_CP(0xE4,-1),     new LD_CP(0xE5,-1),
            new LD_CP(0xE6,-1),     new LD_CP(0xE7,-1),
            new LD_CP(0xE8,-1),     new LD_CP(0xE9,-1),
            new LD_CP(0xEA,-1),     new LD_CP(0xEB,-1),
            new LD_CP(0xEC,-1),     new LD_CP(0xED,-1),
            new LD_CP(0xEE,-1),     new LD_CP(0xEF,-1),

            new LD_CP(0xF0,-1),     new LD_CP(0xF1,-1),
            new LD_CP(0xF2,-1),     new LD_CP(0xF3,-1),
            new LD_CP(0xF4,-1),     new LD_CP(0xF5,-1),
            new LD_CP(0xF6,-1),     new LD_CP(0xF7,-1),
            new LD_CP(0xF8,-1),     new LD_CP(0xF9,-1),
            new LD_CP(0xFA,-1),     new LD_CP(0xFB,-1),
            new LD_CP(0xFC,-1),     new LD_CP(0xFD,-1),
            new LD_CP(0xFE,-1),     new LD_CP(0xFF,-1)
        };

        #endregion

        private class FieldDatas
        {
            private List<string> m_fieldDatas = new List<string>();

            public void Add(string strFieldData)
            {
                m_fieldDatas.Add(strFieldData);
            }

            public string Get(int nIndex)
            {
                if (nIndex >= m_fieldDatas.Count)
                    return null;

                return m_fieldDatas[nIndex];
            }

            public void Set(int nIndex, string strValue)
            {
                if (nIndex >= m_fieldDatas.Count)
                    return;

                m_fieldDatas[nIndex] = strValue;
            }

            public int GetFieldCount()
            {
                return m_fieldDatas.Count;
            }

            public void Clear()
            {
                m_fieldDatas.Clear();
            }
        }

        private int[] m_arrFieldOffset = null;
        private int[] m_arrFieldSize = null;
        private int[] m_arrFieldDecimal = null;
        private char[] m_arrFieldType = null;

        private int m_nRecordCount = -1;
        private int m_nHeaderLength = -1;
        private int m_nRecordLength = -1;
        private int m_nFieldCount = -1;

        List<string> m_fieldNames = new List<string>();
        private List<FieldDatas> m_fieldDatas = new List<FieldDatas>();

        public int GetFieldCount()
        {
            return m_fieldNames.Count;
        }

        public string GetFieldName(int nIndex)
        {
            if (nIndex >= m_fieldNames.Count)
                return null;

            return m_fieldNames[nIndex];
        }

        public string GetFieldData(int nShapeID, int nFieldIndex)
        {
            if (nShapeID >= m_fieldDatas.Count)
                return null;

            FieldDatas fieldDatas = m_fieldDatas[nShapeID];
            return fieldDatas.Get(nFieldIndex);
        }

        public void SetFieldData(int nShapeID, int nFieldIndex, string strValue)
        {
            if (nShapeID >= m_fieldDatas.Count)
                return;

            FieldDatas fieldDatas = m_fieldDatas[nShapeID];
            fieldDatas.Set(nFieldIndex, strValue);
        }
        
        public void Clear()
        {
            m_fieldNames.Clear();
            m_fieldDatas.Clear();
        }

        public void AddFieldName(string strFieldName)
        {
            m_fieldNames.Add(strFieldName);
        }

        public void AddFieldDatas(List<string> fieldDatas)
        {
            FieldDatas datas = new FieldDatas();

            foreach (string data in fieldDatas)
            {
                datas.Add(data);
            }

            m_fieldDatas.Add(datas);
        }

        public ShapeInfo()
        {
        }

        public static ShapeInfo Load(string strPath)
        {
            FileStream reader = new FileStream(strPath, FileMode.Open, FileAccess.Read, FileShare.Read);

            byte[] buffer = new byte[500];
            reader.Read(buffer, 0, 32);

            int nRecordCount = buffer[4] + buffer[5] * 256 + buffer[6] * 256 * 256 + buffer[7] * 256 * 256 * 256;
            int nHeaderLength = buffer[8] + buffer[9] * 256;
            int nRecordLength = buffer[10] + buffer[11] * 256;
            int nFieldCount = (nHeaderLength - 32) / 32;
            int LD_CP_LU = buffer[29 - 12];

            if (nHeaderLength <= 0 || nFieldCount <= 0)
            {
                reader.Close();
                return null;
            }

            System.Text.Encoding encoding = Encoding.Default;
            int codePage = ShapeInfo.LD_CP_LU[LD_CP_LU].codepage;

            if (codePage > 0)
            {
                encoding = System.Text.Encoding.GetEncoding(codePage);

                if (encoding == null)
                    encoding = Encoding.Default;
            }

            buffer = new byte[nHeaderLength];
            reader.Read(buffer, 0, nHeaderLength);

            ShapeInfo info = new ShapeInfo();

            info.m_arrFieldOffset = new int[nFieldCount];
            info.m_arrFieldSize = new int[nFieldCount];
            info.m_arrFieldDecimal = new int[nFieldCount];
            info.m_arrFieldType = new char[nFieldCount];

            info.m_nRecordCount = nRecordCount;
            info.m_nHeaderLength = nHeaderLength;
            info.m_nRecordLength = nRecordLength;
            info.m_nFieldCount = nFieldCount;

            for (int i=0;i<nFieldCount;i++)
            {
                int nIndex = i * 32;
                int nCount = GetNotNullCount(buffer, nIndex);

                string strFieldName = Encoding.ASCII.GetString(buffer, nIndex, nCount);
                info.m_fieldNames.Add(strFieldName);

                if (buffer[nIndex + 11] == (byte)'N')
                {
                    info.m_arrFieldSize[i] = (int)buffer[nIndex + 16];
                    info.m_arrFieldDecimal[i] = (int)buffer[nIndex + 17];
                }
                else
                {
                    info.m_arrFieldSize[i] = (int)buffer[nIndex + 16] + (int)buffer[nIndex + 17] * 256;
                    info.m_arrFieldDecimal[i] = 0;
                }

                info.m_arrFieldType[i] = (char)buffer[nIndex + 11];

                if (i == 0)
                    info.m_arrFieldOffset[i] = 1;
                else
                    info.m_arrFieldOffset[i] = info.m_arrFieldOffset[i - 1] + info.m_arrFieldSize[i - 1];
            }

            info.LoadFieldDatas(reader, encoding);
            reader.Close();
            return info;
        }

        private void LoadFieldDatas(FileStream reader, Encoding encoding)
        {
            byte[] buffer = Shape.SharedBuffer;
            
            for (int i = 0; i < this.m_nRecordCount; i++)
            {
                int nRecordOffset = m_nRecordLength * i + m_nHeaderLength;
                
                reader.Seek(nRecordOffset, SeekOrigin.Begin);
                reader.Read(buffer, 0, m_nRecordLength);

                FieldDatas fieldDatas = new FieldDatas();

                for (int j=0;j<m_nFieldCount;j++)
                {
                    string strFieldData = encoding.GetString(buffer, m_arrFieldOffset[j], m_arrFieldSize[j]).Trim();
                    //string strFieldData = Encoding.ASCII.GetString(buffer, m_arrFieldOffset[j], m_arrFieldSize[j]).Trim();
                    fieldDatas.Add(strFieldData);
                }

                m_fieldDatas.Add(fieldDatas);
            }
        }

        private static int GetNotNullCount(byte[] bytes, int nIndex)
        {
            for (int i=nIndex;i<bytes.Length;i++)
            {
                if (bytes[i] == 0)
                {
                    return i - nIndex;
                }
            }

            return bytes.Length - nIndex;
        }
    }
}
