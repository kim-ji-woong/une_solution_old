using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SateImageLOD
{
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

    public class Rect
    {
        public float TLx { get; set; }
        public float TLy { get; set; }
        public float BLx { get; set; }
        public float BLy { get; set; }
        public float BRx { get; set; }
        public float BRy { get; set; }
    }

    public class LODImage
    {
        private string m_strTime = "";

        public string ImageName { get; set; }
        public int LODIndex { get; set; }
        public string Time
        {
            get { return m_strTime; }
            set { m_strTime = value; }
        }
    }
}
