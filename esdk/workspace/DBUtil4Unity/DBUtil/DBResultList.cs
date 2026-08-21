using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace DBUtility
{
    public class DBResultList : ArrayList, IDisposable
    {
        public void Dispose()
        {
        }

        private int m_nRowCount = 0;        
        public int Row
        {
            get { return m_nRowCount; }
            set { m_nRowCount = value; }
        }

        private int m_nColCount = 0;
        public int Column
        {
            get { return m_nColCount; }
            set { m_nColCount = value; }
        }

        private string m_szQuery = "";
        public string Query
        {
            get { return m_szQuery; }
            set { m_szQuery = value; }
        }
    }

    // struct와 같이 null이 허용되지 않는 데이터를 위한 Wrapper 클래스
    public class VariousData<DataType>
    {
        private DataType data;

        public DataType Data
        {
            get { return data; }
            set { data = value; }
        }

        public VariousData()
        {
        }

        public VariousData(DataType data)
        {
            this.data = data;
        }
    }
}
