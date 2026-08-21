using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MetaData
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

    public class Vertex2F
    {
        private float x = 0.0f;
        private float y = 0.0f;

        public float X
        {
            get { return x; }
            set { x = value; }
        }

        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        public Vertex2F()
        {
        }

        public Vertex2F(float x, float y)
        {
            SetVertex(x, y);
        }

        public void SetVertex(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
