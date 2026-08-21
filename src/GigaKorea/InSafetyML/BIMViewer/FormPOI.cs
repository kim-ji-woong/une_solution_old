using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using System.Data.SqlClient;
using BIMViewer.DB;
using UnE.Geometry;
using System.IO;
using System.Drawing.Drawing2D;

namespace BIMViewer
{
    using Shapes;
    using BIM;

    public partial class FormPOI : Form
    {
        //private _SqlConnection m_connection = null;
        private string m_strErrorMessage = "";
        private Dictionary<int, POIType> m_dicPOITypes = null;

        public FormPOI(Dictionary<int, POIType> poiTypes)
        //public FormPOI(_SqlConnection connection, Dictionary<int, POIType> poiTypes)
        {
            InitializeComponent();
            //m_connection = connection;
            m_dicPOITypes = poiTypes;
        }

        private void FormPOI_Load(object sender, EventArgs e)
        {
            LoadTreeItems();
            poiTree.ExpandAll();
        }

        private bool LoadTreeItems()
        {
            Dictionary<int, TreeNode> dicItems = new Dictionary<int, TreeNode>();
            TreeNodeCollection nodes = poiTree.Nodes;
            TreeNode node = null;

            foreach (KeyValuePair<int, POIType> pair in m_dicPOITypes)
            {
                if (pair.Value.Parent == null)
                {
                    nodes = poiTree.Nodes;
                }
                else
                {
                    if (dicItems.TryGetValue(pair.Value.Parent.ID, out node) == false)
                        return false;

                    nodes = node.Nodes;
                }

                node = nodes.Add(pair.Value.Name);
                node.Tag = pair.Value;
                dicItems[pair.Value.ID] = node;
            }

            return true;
        }

        /*private bool LoadTreeItems()
        {
            string strSQL = "Select ID, IsGroup, ParentID, Name, Code, IsUserDefined from POIType";

            try
            {
                _SqlDataReader reader = ReadQuery(strSQL, m_connection, null);
                Dictionary<int, TreeNode> dicItems = new Dictionary<int, TreeNode>();
                TreeNodeCollection nodes = poiTree.Nodes;
                TreeNode node = null;

                if (reader != null)
                {
                    while (reader.Read())
                    {
                        int nID = reader.GetInt32(0);
                        bool isGroup = reader.GetBoolean(1);
                        
                        if (reader.IsDBNull(2))
                        {
                            nodes = poiTree.Nodes;
                        }
                        else
                        {
                            int nParentID = reader.GetInt32(2);

                            if (dicItems.TryGetValue(nParentID, out node) == false)
                                return false;

                            nodes = node.Nodes;
                        }

                        string strPOIName = reader.GetString(3);
                        string strCode = reader.IsDBNull(4) ? "" : reader.GetString(4);
                        bool isUserDefined = reader.GetBoolean(5);
                        POIType poiType = null;

                        if (isGroup == false)
                        {
                            if (m_dicPOITypes == null || m_dicPOITypes.TryGetValue(nID, out poiType) == false)
                            {
                                poiType = new POIType();
                                poiType.ID = nID;
                                poiType.Name = strPOIName;
                                poiType.UserDefined = isUserDefined;
                                poiType.Code = strCode;
                            }
                        }

                        node = nodes.Add(strPOIName);
                        node.Tag = poiType;
                        dicItems[nID] = node;
                    }

                    reader.Close();
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private _SqlDataReader ReadQuery(string strSQL, _SqlConnection connection, _SqlTransaction transaction)
        {
            try
            {
                _SqlCommand cmd = new _SqlCommand(strSQL, connection, transaction);
                return cmd.ExecuteReader();
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return null;
        }*/

        private void poiTree_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                TreeViewHitTestInfo info = poiTree.HitTest(e.X, e.Y);

                if (info != null && info.Node != null)
                {
                    poiTree.SelectedNode = info.Node;

                    if (info.Node.Tag != null)
                    {
                        POIType poiType = (POIType)info.Node.Tag;
                        DoDragDrop(poiType, DragDropEffects.Move);
                    }
                }
                else
                    poiTree.SelectedNode = null;
            }
        }
    }

    public class POIType
    {
        private int m_nID = 0;
        private string m_strXMLID = "";
        private string m_strName = "";
        private bool m_userDefined = false;
        private int m_nParentID = 0;
        private POIType m_parent = null;
        private List<POIType> m_childTypes = new List<POIType>();
        private string m_strCode = "";
        private bool m_bIsGroup = false;
        private Color m_color = Color.Yellow;
        private string m_strDefaultHeight = null;

        // 선형
        private LinkedPath m_path = new LinkedPath();
        // 채움
        private List<LinkedPath> m_listPolygons = new List<LinkedPath>();
        private List<TextData> m_listText = new List<TextData>();

        private bool m_hasIcon = false;
        private bool m_bVisible = true;

        private List<Property> m_properties = new List<Property>();

        public const string POITypeIDTag = "pt";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string XMLID
        {
            get { return m_strXMLID; }
            set { m_strXMLID = value; }
        }

        public string Name
        {
            get { return m_strName; }
            set { m_strName = value; }
        }

        public bool UserDefined
        {
            get { return m_userDefined; }
            set { m_userDefined = value; }
        }

        public int ParentID
        {
            get { return m_nParentID; }
            set { m_nParentID = value; }
        }

        public POIType Parent
        {
            get { return m_parent; }
            set
            {
                m_parent = value;

                if (m_parent != null && m_parent.m_childTypes.Contains(this) == false)
                    m_parent.m_childTypes.Add(this);
            }
        }

 
        public List<POIType> ChildTypes
        {
            get { return m_childTypes; }
        }

        public string DefaultHeight
        {
            get { return m_strDefaultHeight; }
            set { m_strDefaultHeight = value; }
        }

        public LinkedPath Path
        {
            get { return m_path; }
        }

        public List<LinkedPath> Polygons
        {
            get { return m_listPolygons; }
        }

        public List<TextData> TextDatas
        {
            get { return m_listText; }
        }

        public bool HasIcon
        {
            get { return m_hasIcon; }
        }
                
        public bool POIVisible
        {
            get { return m_bVisible; }
            set { m_bVisible = value; }
        }

        public string Code
        {
            get { return m_strCode; }
            set { m_strCode = value; }
        }

        public bool IsGroup
        {
            get { return m_bIsGroup; }
            set { m_bIsGroup = value; }
        }
        
        public Color Color
        {
            get { return m_color; }
            set { m_color = value; }
        }

        public List<Property> Properties
        {
            get { return m_properties; }
        }

        public void CopyFrom(POIType poi)
        {
            m_path.Path.Clear();
            m_path.Path.AddRange(poi.m_path.Path);

            m_listPolygons.Clear();
            m_listPolygons.AddRange(poi.m_listPolygons);

            m_listText.Clear();
            m_listText.AddRange(poi.m_listText);

            if (m_path.Path.Count > 0 || m_listPolygons.Count > 0 || m_listText.Count > 0)
                m_hasIcon = true;
            else
                m_hasIcon = false;
        }

        public static POIType ReadPOI(BinaryReader reader, double dScale = 1.0)
        {
            POIType poiType = new POIType();

            string strPOIName = reader.ReadString();
            poiType.Name = strPOIName;

            if (ReadPath(poiType.m_path, reader, dScale) == false)
                return null;

            if (ReadPolygons(poiType.m_listPolygons, reader, dScale) == false)
                return null;

            if (ReadTextDatas(poiType.TextDatas, reader, dScale) == false)
                return null;

            return poiType;
        }

        private static bool ReadTextDatas(List<TextData> textDatas, BinaryReader reader, double dScale)
        {
            try
            {
                int nTextCount = reader.ReadInt32();

                for (int i = 0; i < nTextCount; i++)
                {
                    string strText = reader.ReadString();
                    double x = reader.ReadDouble() * dScale;
                    double y = reader.ReadDouble() * dScale;
                    float fFontSize = (float)(reader.ReadSingle() * dScale);
                    double dTextAngle = reader.ReadDouble();

                    TextData data = new TextData();

                    data.Text = strText;
                    data.Position = new Vertex2D(x, y);
                    data.FontSize = fFontSize;
                    data.TextAngle = dTextAngle;

                    textDatas.Add(data);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private static bool ReadPolygons(List<LinkedPath> polygons, BinaryReader reader, double dScale)
        {
            int nPolygonCount = reader.ReadInt32();

            for (int i=0;i<nPolygonCount;i++)
            {
                LinkedPath polygon = new LinkedPath();

                if (ReadPath(polygon, reader, dScale) == false)
                    return false;

                polygons.Add(polygon);
            }

            return true;
        }

        private static bool ReadPath(LinkedPath path, BinaryReader reader, double dScale)
        {
            int nPathCount = reader.ReadInt32();

            for (int i = 0; i < nPathCount; i++)
            {
                int nType = reader.ReadInt32();
                PathItem item = null;

                if (nType == (int)PathItem.DrawType.Line)
                    item = ReadLinePath(reader, dScale);
                else if (nType == (int)PathItem.DrawType.Arc)
                    item = ReadArcPath(reader, dScale);
                else if (nType == (int)PathItem.DrawType.EArc)
                    item = ReadEArcPath(reader, dScale);

                if (item == null)
                    return false;

                path.Path.Add(item);
            }

            return true;
        }

        private static PathItem ReadEArcPath(BinaryReader reader, double dScale)
        {
            try
            {
                double dTLX = reader.ReadDouble() * dScale;
                double dTLY = reader.ReadDouble() * dScale;
                double dBLX = reader.ReadDouble() * dScale;
                double dBLY = reader.ReadDouble() * dScale;
                double dBRX = reader.ReadDouble() * dScale;
                double dBRY = reader.ReadDouble() * dScale;
                double dBeginAngle = reader.ReadDouble();
                double dEArcAngle = reader.ReadDouble();
                bool isClockWise = reader.ReadBoolean();

                Vertex2D vTL = new Vertex2D(dTLX, dTLY);
                Vertex2D vBL = new Vertex2D(dBLX, dBLY);
                Vertex2D vBR = new Vertex2D(dBRX, dBRY);
                EArc2D earc = new EArc2D(vTL, vBL, vBR, dBeginAngle, dEArcAngle, isClockWise);

                PathItem item = new PathItem();
                item.SetEArc(earc);
                return item;
            }
            catch (Exception)
            {
            }

            return null;
        }

        private static PathItem ReadArcPath(BinaryReader reader, double dScale)
        {
            try
            {
                double dCenterX = reader.ReadDouble() * dScale;
                double dCenterY = reader.ReadDouble() * dScale;
                double dRadius = reader.ReadDouble() * dScale;
                double dBeginAngle = reader.ReadDouble();
                double dArcAngle = reader.ReadDouble();
                bool isClockWise = reader.ReadBoolean();

                Vertex2D vCenter = new Vertex2D(dCenterX, dCenterY);
                Arc2D arc = new Arc2D(vCenter, dRadius, dBeginAngle, dArcAngle, isClockWise);

                PathItem item = new PathItem();
                item.SetArc(arc);
                return item;
            }
            catch (Exception)
            {
            }

            return null;
        }

        private static PathItem ReadLinePath(BinaryReader reader, double dScale)
        {
            try
            {
                double dBeginX = reader.ReadDouble() * dScale;
                double dBeginY = reader.ReadDouble() * dScale;
                double dEndX = reader.ReadDouble() * dScale;
                double dEndY = reader.ReadDouble() * dScale;

                Vertex2D vBegin = new Vertex2D(dBeginX, dBeginY);
                Vertex2D vEnd = new Vertex2D(dEndX, dEndY);

                PathItem item = new PathItem();
                item.SetLine(new Line2D(vBegin, vEnd));
                return item;
            }
            catch (Exception)
            {
            }

            return null;
        }

        public List<GraphicsPath> MakePath(double x, double y, ref Vertex2D vTL, ref Vertex2D vBR)
        {
            if (m_path.Path.Count == 0)
                return null;

            List<GraphicsPath> pathList = new List<GraphicsPath>();

            foreach (PathItem item in m_path.Path)
            {
                GraphicsPath path = new GraphicsPath();
                Space.AddPath(path, item, x, y);
                pathList.Add(path);

                item.CheckBoundary(x, y, ref vTL, ref vBR);
            }

            return pathList;
        }

        public List<GraphicsPath> MakePolygons(double x, double y, ref Vertex2D vTL, ref Vertex2D vBR)
        {
            List<GraphicsPath> paths = new List<GraphicsPath>();

            foreach (LinkedPath polygon in m_listPolygons)
            {
                if (polygon.Path.Count > 0)
                {
                    GraphicsPath path = Space.MakeGraphicsPath(polygon.Path, x, y);

                    if (path != null)
                        paths.Add(path);

                    foreach (PathItem item in polygon.Path)
                    {
                        item.CheckBoundary(x, y, ref vTL, ref vBR);
                    }
                }
            }

            return paths;
        }

        public class TextData
        {
            private Vertex2D m_vPos = new Vertex2D();
            private Vertex2D m_vBoundaryTL = null;
            private Vertex2D m_vBoundaryBR = null;
            private string m_strText = "";
            private float m_fFontSize = 10;
            // Degree
            private double m_dTextAngle = 0.0;

            public Vertex2D Position
            {
                get { return m_vPos; }
                set { m_vPos = value; }
            }

            public string Text
            {
                get { return m_strText; }
                set { m_strText = value; }
            }

            public float FontSize
            {
                get { return m_fFontSize; }
                set { m_fFontSize = value; }
            }

            // Degree
            public double TextAngle
            {
                get { return m_dTextAngle; }
                set { m_dTextAngle = value; }
            }

            public Vertex2D BoundaryTL
            {
                get { return m_vBoundaryTL; }
                set { m_vBoundaryTL = value; }
            }

            public Vertex2D BoundaryBR
            {
                get { return m_vBoundaryBR; }
                set { m_vBoundaryBR = value; }
            }

            public Font GetFont()
            {
                return new Font("돋움", m_fFontSize);
            }

            public void Render(Graphics g, float x, float y, Color color, float fScaleY)
            {
                g.ScaleTransform(1.0f, -1.0f);
                y = -y;

                // 현재 Y축 Scale값을 가져온다.
                float x1 = fScaleY;
                //float x1 = g.Transform.Elements[3];
                // 폰트의 길이와 Y축의 곱이 실제 픽셀당 거리
                float h = x1 * m_fFontSize;

                // 1 픽셀미만이면 의미없으므로 Cutoff를 1로 한다.
                // 자간이 좁아지면 Graphics에서 예외가 발생하므로 작은값은 피한다.
                if (h > 1.0f || h < -1.0)
                {
                    Font font = GetFont();//new Font("돋움", m_fFontSize);
                    Brush brush = new SolidBrush(color);

                    g.DrawString(m_strText, font, brush, x, y);
                    //SizeF size = g.MeasureString(m_strText, font);
                    //g.DrawString(m_strText, font, brush, x - size.Width / 2, y - size.Height / 2 + 13);

                    brush.Dispose();
                    font.Dispose();
                }

                g.ScaleTransform(1.0f, -1.0f);
            }
        }

        public class LinkedPath
        {
            private List<PathItem> m_listPath = new List<PathItem>();
            private Vertex2D m_vTL = null;
            private Vertex2D m_vBR = null;

            public Vertex2D BoundaryTL
            {
                get { return m_vTL; }
            }

            public Vertex2D BoundaryBR
            {
                get { return m_vBR; }
            }

            public List<PathItem> Path
            {
                get { return m_listPath; }
            }

            public PathItem AddLine(Line2D line)
            {
                PathItem item = new PathItem();
                item.SetLine(line);
                m_listPath.Add(item);
                SetBoundary(ref m_vTL, ref m_vBR, line.GetVertex(true));
                SetBoundary(ref m_vTL, ref m_vBR, line.GetVertex(false));
                return item;
            }

            public PathItem AddLine(Vertex2D vBegin, Vertex2D vEnd)
            {
                PathItem item = new PathItem();
                item.SetLine(new Line2D(vBegin, vEnd));
                m_listPath.Add(item);
                SetBoundary(ref m_vTL, ref m_vBR, vBegin);
                SetBoundary(ref m_vTL, ref m_vBR, vEnd);
                return item;
            }

            public PathItem AddArc(Arc2D arc)
            {
                PathItem item = new PathItem();
                item.SetArc(arc);
                m_listPath.Add(item);
                SetBoundary(ref m_vTL, ref m_vBR, arc.GetTL());
                SetBoundary(ref m_vTL, ref m_vBR, arc.GetBR());
                return item;
            }

            public PathItem AddEArc(EArc2D earc)
            {
                PathItem item = new PathItem();
                item.SetEArc(earc);
                m_listPath.Add(item);
                SetBoundary(ref m_vTL, ref m_vBR, earc.GetTL());
                SetBoundary(ref m_vTL, ref m_vBR, earc.GetBR());
                return item;
            }

            public static void SetBoundary(ref Vertex2D vTL, ref Vertex2D vBR, Vertex2D vertex)
            {
                if (vTL == null)
                {
                    vTL = new Vertex2D(vertex);
                    vBR = new Vertex2D(vertex);
                }
                else
                {
                    if (vTL.x > vertex.x)
                        vTL.x = vertex.x;
                    if (vTL.y < vertex.y)
                        vTL.y = vertex.y;
                    if (vBR.x < vertex.x)
                        vBR.x = vertex.x;
                    if (vBR.y > vertex.y)
                        vBR.y = vertex.y;
                }
            }
        }
    }
}
