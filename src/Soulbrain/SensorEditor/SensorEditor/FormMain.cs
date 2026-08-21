using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using UnE.Geometry;

namespace SensorEditor
{
    public partial class FormMain : Form
    {
        private Vertex2D m_vScreen1 = null, m_vScreen2 = null, m_vScreen3 = null;
        private Vertex2D m_vGlobal1 = null, m_vGlobal2 = null, m_vGlobal3 = null;
        private FormMenu m_frmMenu = null;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            ReadCoordinate();
            this.pictureBox1.Size = new Size(this.pictureBox1.BackgroundImage.Size.Width, this.pictureBox1.BackgroundImage.Size.Height);

            m_frmMenu = new FormMenu(this);
            m_frmMenu.Show();

            this.Size = new Size(1920, 1080);
        }

        private bool ReadCoordinate()
        {
            string strPoint1 = ConfigurationManager.AppSettings["Point1"].ToString();
            string strPoint2 = ConfigurationManager.AppSettings["Point2"].ToString();
            string strPoint3 = ConfigurationManager.AppSettings["Point3"].ToString();

            Point pt1, pt2, pt3;
            Vertex2D v1, v2, v3;

            if (GetCoord(strPoint1, out pt1, out v1) == false)
                return false;
            if (GetCoord(strPoint2, out pt2, out v2) == false)
                return false;
            if (GetCoord(strPoint3, out pt3, out v3) == false)
                return false;

            m_vScreen1 = new Vertex2D(pt1.X, pt1.Y);
            m_vScreen2 = new Vertex2D(pt2.X, pt2.Y);
            m_vScreen3 = new Vertex2D(pt3.X, pt3.Y);

            m_vGlobal1 = v1;
            m_vGlobal2 = v2;
            m_vGlobal3 = v3;

            return true;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            Vertex2D vGlobal = GetVertex(e.Location);
            /*System.Diagnostics.Trace.WriteLine("Screen : " + e.Location.X.ToString() + ", " + e.Location.Y.ToString());
            System.Diagnostics.Trace.WriteLine("Click : " + vGlobal.x.ToString() + ", " + vGlobal.y.ToString());*/

            btnMark.Location = e.Location;
            btnMark.Visible = true;

            m_frmMenu.SetSensorLocation(vGlobal);
        }

        private Vertex2D GetVertex(Point pt)
        {
            Vertex2D vertex = new Vertex2D(pt.X, pt.Y);
            Vertex2D v1 = UnE.Geometry.Math.GetNearestVertex(vertex, m_vScreen1, m_vScreen2, true);
            Vertex2D _v1 = ToGlobal(m_vScreen1, m_vScreen2, m_vGlobal1, m_vGlobal2, v1);

            Vertex2D v2 = UnE.Geometry.Math.GetNearestVertex(vertex, m_vScreen2, m_vScreen3, true);
            Vertex2D _v2 = ToGlobal(m_vScreen2, m_vScreen3, m_vGlobal2, m_vGlobal3, v2);

            return _v1 - m_vGlobal2 + _v2;
        }

        private Vertex2D ToGlobal(Vertex2D vScreen1, Vertex2D vScreen2, Vertex2D vGlobal1, Vertex2D vGlobal2, Vertex2D vSrc)
        {
            double dLen1 = vScreen1.GetDistance(vSrc);
            double dLenScreen = vScreen1.GetDistance(vScreen2);
            double dLenGlobal = vGlobal1.GetDistance(vGlobal2);

            Line2D line = new Line2D(vScreen1, vScreen2);

            if (line.IsInclude(vSrc))
            {
                return UnE.Geometry.Math.GetLinearVertex(vGlobal1, vGlobal2, dLen1 * dLenGlobal / dLenScreen);
            }

            double dLen2 = vScreen2.GetDistance(vSrc);

            if (dLen1 < dLen2)
            {
                // vScreen1쪽에 더 가까운 경우...
                return UnE.Geometry.Math.GetLinearVertex(vGlobal1, vGlobal2, -dLen1 * dLenGlobal / dLenScreen);
            }

            // vScreen2쪽에 더 가까운 경우...
            return UnE.Geometry.Math.GetLinearVertex(vGlobal2, vGlobal1, -dLen2 * dLenGlobal / dLenScreen);
        }

        private bool GetCoord(string strPoint, out Point pt, out Vertex2D vertex)
        {
            pt = new Point();
            vertex = null;

            int nIndex = strPoint.IndexOf(':');

            if (nIndex < 0)
                return false;

            string str1 = strPoint.Substring(0, nIndex).Trim();
            string str2 = strPoint.Substring(nIndex + 1).Trim();

            if (GetPoint(str1, ref pt) == false)
                return false;

            if (GetVertex(str2, ref vertex) == false)
                return false;

            return true;
        }

        private bool GetPoint(string str, ref Point pt)
        {
            int nIndex = str.IndexOf(',');

            if (nIndex < 0)
                return false;

            string strX = str.Substring(0, nIndex).Trim();
            string strY = str.Substring(nIndex + 1).Trim();

            int x, y;

            if (int.TryParse(strX, out x) && int.TryParse(strY, out y))
            {
                pt = new Point(x, y);
                return true;
            }

            return false;
        }

        private bool GetVertex(string str, ref Vertex2D vertex)
        {
            int nIndex = str.IndexOf(',');

            if (nIndex < 0)
                return false;

            string strX = str.Substring(0, nIndex).Trim();
            string strY = str.Substring(nIndex + 1).Trim();

            double x, y;

            if (double.TryParse(strX, out x) && double.TryParse(strY, out y))
            {
                vertex = new Vertex2D(x, y);
                return true;
            }

            return false;
        }

        public void ShowSensor(float x, float z)
        {
            Point pt = GetPoint(new Vertex2D(x, z));
            btnMark.Location = pt;
            btnMark.Visible = true;
        }

        private Point GetPoint(Vertex2D vertex)
        {
            Vertex2D v1 = UnE.Geometry.Math.GetNearestVertex(vertex, m_vGlobal1, m_vGlobal2, true);
            Vertex2D _v1 = ToGlobal(m_vGlobal1, m_vGlobal2, m_vScreen1, m_vScreen2, v1);

            Vertex2D v2 = UnE.Geometry.Math.GetNearestVertex(vertex, m_vGlobal2, m_vGlobal3, true);
            Vertex2D _v2 = ToGlobal(m_vGlobal2, m_vGlobal3, m_vScreen2, m_vScreen3, v2);

            Vertex2D vTarget = _v1 - m_vScreen2 + _v2;
            return new Point((int)vTarget.x, (int)vTarget.y);
        }
    }
}
