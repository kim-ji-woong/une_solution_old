using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1
{
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