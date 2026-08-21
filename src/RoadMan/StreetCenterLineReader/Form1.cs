using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DXFViewer;
using System.Xml;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private UnE.Geometry.Vertex2D m_vMovedVertex = null;
        float m_fMoveX = 0.0f, m_fMoveY = 0.0f;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            colNo.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colStreetName.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colShapeID.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colPolygonID.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colLineOrder.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colMethod.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void btnOpenDXF_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "DXF Files|*.dxf|All FIles|*.*";
            dlg.FilterIndex = 0;
            dlg.Title = "DXF 파일 열기";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ReadDXF(dlg.FileName);
            }
        }

        private bool ReadDXF(string strPath)
        {
            DXFControl ctrl = new DXFControl();
            ctrl.OpenNRefresh = false;

            if (!ctrl.OpenDXF(strPath))
                return false;

            foreach (Layer layer in ctrl.Layers)
            {
                string strStreetName = "";

                if (textBoxStreetHeader.Text.Length == 0)
                    strStreetName = layer.LayerName;
                else if (layer.LayerName.StartsWith(textBoxStreetHeader.Text))
                    strStreetName = layer.LayerName.Substring(textBoxStreetHeader.Text.Length);
                else
                    continue;

                foreach (Shape shape in layer.Shapes)
                {
                    AddRow(strStreetName, shape, layer.Shapes.Count == 1 ? 0 : -1);
                }
            }

            m_vMovedVertex = new UnE.Geometry.Vertex2D(ctrl.MovedVertex.x, ctrl.MovedVertex.y);
            return true;
        }

        private void AddRow(string strStreetName, Shape shape, int nLineOrder)
        {
            DataGridViewRow row = MakeNewRow();

            row.Cells[0].Value = row.Index + 1;
            row.Cells[1].Value = strStreetName;
            row.Cells[2].Value = string.Format("{0:X}", shape.ID);

            if (nLineOrder >= 0)
                row.Cells[4].Value = nLineOrder;

            row.Tag = shape;
        }

        private void AddRow(string strStreetName, int nSourceID, int nLineOrder, PolyLineOption opt)
        {
            DataGridViewRow row = MakeNewRow();

            row.Cells[0].Value = row.Index + 1;
            row.Cells[1].Value = strStreetName;
            row.Cells[2].Value = string.Format("{0:X}", nSourceID);
            row.Cells[3].Value = string.Format("{0:X}", opt.TargetShapeID);

            if (nLineOrder >= 0)
                row.Cells[4].Value = nLineOrder;

            row.Cells[5].Value = PolyLineOption.GetBeginOptionString(opt.BegionOption);
            row.Tag = opt;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            List<DataGridViewRow> removeRows = new List<DataGridViewRow>();

            foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
            {
                DataGridViewRow row = dataGridView1.Rows[cell.RowIndex];

                if (!removeRows.Contains(row))
                    removeRows.Add(row);
            }

            foreach (DataGridViewRow row in removeRows)
            {
                dataGridView1.Rows.Remove(row);
            }

            int nIndex = 1;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Cells[0].Value = nIndex++;
            }
        }

        private DataGridViewRow MakeNewRow()
        {
            int nIndex = 0;

            if (dataGridView1.AllowUserToAddRows)
            {
                nIndex = dataGridView1.Rows.Count - 1;
                DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[dataGridView1.Rows.Count - 1].Clone();
                dataGridView1.Rows.Add(row);
            }
            else
            {
                dataGridView1.AllowUserToAddRows = true;

                nIndex = dataGridView1.Rows.Count - 1;
                DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[dataGridView1.Rows.Count - 1].Clone();
                dataGridView1.Rows.Add(row);

                dataGridView1.AllowUserToAddRows = false;
            }

            return dataGridView1.Rows[nIndex];
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            double x = 0, y = 0;
            bool readX = false, readY = false;

            if (textBoxTargetX.Text.Length > 0)
            {
                if (double.TryParse(textBoxTargetX.Text, out x))
                    readX = true;
            }

            if (textBoxTargetY.Text.Length > 0)
            {
                if (double.TryParse(textBoxTargetY.Text, out y))
                    readY = true;
            }

            UnE.Geometry.Vertex2D vMovedTarget = readX && readY ? new UnE.Geometry.Vertex2D(x, y) : null;

            SaveFileDialog dlg = new SaveFileDialog();

            dlg.Filter = "XML Files|*.xml|All FIles|*.*";
            dlg.FilterIndex = 0;
            dlg.Title = "XML 내보내기";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SaveXML(dlg.FileName, vMovedTarget);
            }
        }

        private void SaveXML(string strPath, UnE.Geometry.Vertex2D vMovedTarget)
        {
            if (vMovedTarget != null)
            {
                m_fMoveX = (float)(-m_vMovedVertex.x + vMovedTarget.x);
                m_fMoveY = (float)(-m_vMovedVertex.y + vMovedTarget.y);
            }
            else
            {
                m_fMoveX = m_fMoveY = 0.0f;
                vMovedTarget = m_vMovedVertex;
            }

            XmlTextWriter writer = new XmlTextWriter(strPath, Encoding.UTF8);

            writer.Formatting = Formatting.Indented;
            writer.WriteStartDocument();
            writer.WriteStartElement("StreetCenterLines");

            writer.WriteStartAttribute("movedX");
            writer.WriteString(vMovedTarget.x.ToString());
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("movedY");
            writer.WriteString(vMovedTarget.y.ToString());
            writer.WriteEndAttribute();

            SaveCenterLines(writer);

            writer.WriteEndElement();
            writer.WriteEndDocument();

            writer.Close();
        }

        private void SaveCenterLines(XmlTextWriter writer)
        {
            Dictionary<string, List<DataGridViewRow>> dicStreetRows = new Dictionary<string, List<DataGridViewRow>>();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                List<DataGridViewRow> rows = null;

                if (dicStreetRows.ContainsKey(row.Cells[1].Value.ToString()))
                    rows = dicStreetRows[row.Cells[1].Value.ToString()];
                else
                {
                    rows = new List<DataGridViewRow>();
                    dicStreetRows[row.Cells[1].Value.ToString()] = rows;
                }

                // rows에 담겨질 행들은 Line 순서에 맞게 정렬되어야 한다.
                int nOrder = int.Parse(row.Cells[4].Value.ToString());
                int nRowCount = rows.Count;
                bool added = false;

                for (int i = 0; i < nRowCount;i++)
                {
                    DataGridViewRow row2 = rows[i];
                    int nOrder2 = int.Parse(row2.Cells[4].Value.ToString());

                    if (nOrder2 < nOrder)
                    {
                        if (i == nRowCount - 1)
                        {
                            rows.Add(row);
                            added = true;
                            break;
                        }
                        else
                        {
                            DataGridViewRow row3 = rows[i + 1];
                            int nOrder3 = int.Parse(row3.Cells[4].Value.ToString());

                            if (nOrder < nOrder3)
                            {
                                rows.Insert(i + 1, row);
                                added = true;
                                break;
                            }
                        }
                    }
                    else if (nOrder < nOrder2)
                    {
                        rows.Insert(i, row);
                        added = true;
                        break;
                    }
                }

                if (!added)
                    rows.Add(row);
            }

            SaveCenterLines(writer, dicStreetRows);
        }

        private void SaveCenterLines(XmlTextWriter writer, Dictionary<string, List<DataGridViewRow>> dicStreetRows)
        {
            foreach (KeyValuePair<string, List<DataGridViewRow>> pair in dicStreetRows)
            {
                writer.WriteStartElement("StreetCenterLine");

                writer.WriteStartElement("StreetName");
                writer.WriteString(pair.Key);
                writer.WriteEndElement();

                writer.WriteStartElement("PolyLines");

                SavePolyLines(writer, pair.Value);

                // PolyLines
                writer.WriteEndElement();
                // StreetCenterLine
                writer.WriteEndElement();
            }
        }

        private void SavePolyLines(XmlTextWriter writer, List<DataGridViewRow> rows)
        {
            foreach (DataGridViewRow row in rows)
            {
                PolyLine line = null;

                if (row.Tag == null)
                    continue;

                if (row.Tag.GetType() == typeof(PolyLineOption))
                {
                    PolyLineOption option = (PolyLineOption)row.Tag;
                    line = option.PolyLine;
                }
                else
                {
                    Shape shape = (Shape)row.Tag;
                    if (shape.GetShapeType() != Shape.ShapeType.POLYLINE)
                        continue;

                    line = (PolyLine)shape;
                }

                int nVertexCount = line.GetVertexSize();

                if (nVertexCount < 2)
                    continue;

                writer.WriteStartElement("PolyLine");

                int nSourceShapeID, nTargetShapeID;

                if (int.TryParse(row.Cells[2].Value.ToString(), System.Globalization.NumberStyles.AllowHexSpecifier, null, out nSourceShapeID))
                {
                    writer.WriteStartElement("SourceShapeID");
                    writer.WriteString(nSourceShapeID.ToString());
                    writer.WriteEndElement();
                }

                if (int.TryParse(row.Cells[3].Value.ToString(), System.Globalization.NumberStyles.AllowHexSpecifier, null, out nTargetShapeID))
                {
                    writer.WriteStartElement("TargetShapeID");
                    writer.WriteString(nTargetShapeID.ToString());
                    writer.WriteEndElement();
                }

                writer.WriteStartElement("Vertices");

                int nDirOpt;
                bool isPositiveDirection = IsPositiveDirection(line, nVertexCount, row.Cells[5].Value.ToString(), out nDirOpt);

                writer.WriteStartAttribute("beginOpt");
                writer.WriteString(nDirOpt.ToString());
                writer.WriteEndAttribute();

                SaveVertices(writer, line, nVertexCount, isPositiveDirection);

                // Vertices
                writer.WriteEndElement();
                // PolyLine
                writer.WriteEndElement();
            }
        }

        private string HexToInt(string strHex)
        {
            int nResult = 0;

            if (!int.TryParse(strHex, System.Globalization.NumberStyles.AllowHexSpecifier, null, out nResult))
                return nResult.ToString();

            return nResult.ToString();
        }

        private void SaveVertices(XmlTextWriter writer, PolyLine line, int nVertexCount, bool isPositiveDirection)
        {
            if (isPositiveDirection)
            {
                for (int i=0;i<nVertexCount;i++)
                {
                    SaveVertex(writer, line, i);
                }
            }
            else
            {
                for (int i=nVertexCount-1;i>=0;i--)
                {
                    SaveVertex(writer, line, i);
                }
            }
        }

        private void SaveVertex(XmlTextWriter writer, PolyLine line, int nVertexIndex)
        {
            PointF pt = line.GetVertex(nVertexIndex);

            writer.WriteStartElement("Vertex");

            writer.WriteStartAttribute("x");
            writer.WriteString((pt.X + m_fMoveX).ToString());
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("y");
            writer.WriteString((pt.Y + m_fMoveY).ToString());
            writer.WriteEndAttribute();

            writer.WriteEndElement();
        }

        private bool IsPositiveDirection(PolyLine line, int nVertexCount, string strType, out int nDirectionOption)
        {
            PointF ptBegin = line.GetVertex(0);
            PointF ptEnd = line.GetVertex(nVertexCount - 1);

            if (strType == "Y값이 가장 큰 Vertex")
            {
                nDirectionOption = 0;

                if (ptBegin.Y > ptEnd.Y)
                    return true;
            }
            else if (strType == "Y값이 가장 작은 Vertex")
            {
                nDirectionOption = 1;

                if (ptBegin.Y < ptEnd.Y)
                    return true;
            }
            else if (strType == "X값이 가장 큰 Vertex")
            {
                nDirectionOption = 2;

                if (ptBegin.X > ptEnd.X)
                    return true;
            }
            else if (strType == "X값이 가장 작은 Vertex")
            {
                nDirectionOption = 3;

                if (ptBegin.X < ptEnd.X)
                    return true;
            }
            else
                throw new Exception();

            return false;
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "XML Files|*.xml|All FIles|*.*";
            dlg.FilterIndex = 0;
            dlg.Title = "XML 파일 열기";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Dictionary<string, StreetCenterLine> dicStreetCenterLine = ReadXML(dlg.FileName);

                if (dicStreetCenterLine != null)
                    ResetGrid(dicStreetCenterLine);
            }
        }

        private void ResetGrid(Dictionary<string, StreetCenterLine> dicStreetCenterLine)
        {
            dataGridView1.Rows.Clear();

            foreach (KeyValuePair<string, StreetCenterLine> pair in dicStreetCenterLine)
            {
                int nOrder = 0;

                foreach (KeyValuePair<int, PolyLineOption> pair2 in pair.Value.PolyLineOptions)
                {
                    AddRow(pair.Key, pair2.Key, nOrder++, pair2.Value);
                }
            }
        }

        private Dictionary<string, StreetCenterLine> ReadXML(string strPath)
        {
            XmlTextReader reader = new XmlTextReader(strPath);
            Dictionary<string, StreetCenterLine> dicStreetCenterLine = null;
            bool stop = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "StreetCenterLines", true) != 0)
                        {
                            MessageBox.Show("다른 형식의 파일입니다.");
                            reader.Close();
                            return null;
                        }
                        else
                        {
                            dicStreetCenterLine = ReadStreetCenterLines(reader);

                            if (dicStreetCenterLine == null)
                                stop = true;
                        }

                        break;
                }

                if (stop)
                    break;
            }

            reader.Close();
            return dicStreetCenterLine;
        }

        private Dictionary<string, StreetCenterLine> ReadStreetCenterLines(XmlTextReader reader)
        {
            bool stop = false;
            Dictionary<string, StreetCenterLine> dicStreetCenterLines = new Dictionary<string, StreetCenterLine>();

            if (reader.IsEmptyElement)
                return dicStreetCenterLines;

            try
            {
                double x = 0.0, y = 0.0;
                bool findX = false, findY = false;

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "movedX", true) == 0)
                    {
                        if (!double.TryParse(reader.Value.ToString(), out x))
                            return null;
                        else
                            findX = true;
                    }
                    else if (string.Compare(reader.Name, "movedY", true) == 0)
                    {
                        if (!double.TryParse(reader.Value.ToString(), out y))
                            return null;
                        else
                            findY = true;
                    }
                }

                if (findX && findY)
                {
                    m_vMovedVertex = new UnE.Geometry.Vertex2D(x, y);
                }

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "StreetCenterLine", true) == 0)
                            {
                                if (!ReadStreetCenterLine(reader, dicStreetCenterLines))
                                    return null;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                return null;
            }

            return dicStreetCenterLines;
        }

        private void PassElement(XmlTextReader reader)
        {
            if (reader.IsEmptyElement)
                return;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.EndElement:
                        return;
                }
            }
        }

        private bool ReadText(XmlTextReader reader, ref string strText, bool allowEmpty = false)
        {
            if (reader.IsEmptyElement)
            {
                strText = "";
                return allowEmpty;
            }

            if (!ReadElementText(reader, ref strText))
                strText = "";

            return true;
        }

        private bool ReadElementText(XmlTextReader reader, ref string strText)
        {
            bool stop = false, readText = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Text:
                        strText = reader.Value;
                        readText = true;
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            return readText;
        }

        private bool ReadStreetCenterLine(XmlTextReader reader, Dictionary<string, StreetCenterLine> dicStreetCenterLines)
        {
            bool stop = false;

            if (reader.IsEmptyElement)
                return false;

            string strStreetName = "";
            // Key : Source(중심선) Shape의 ID
            Dictionary<int, PolyLineOption> dicPolyLineOptions = null;

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "StreetName", true) == 0)
                            {
                                if (!ReadText(reader, ref strStreetName, true))
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "PolyLines", true) == 0)
                            {
                                dicPolyLineOptions = ReadPolyLines(reader);

                                if (dicPolyLineOptions == null)
                                    return false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                return false;
            }

            StreetCenterLine centerLine = new StreetCenterLine();
            centerLine.StreetName = strStreetName;
            centerLine.PolyLineOptions = dicPolyLineOptions;

            dicStreetCenterLines[strStreetName] = centerLine;
            return true;
        }

        // Key : Source(중심선) Shape의 ID
        private Dictionary<int, PolyLineOption> ReadPolyLines(XmlTextReader reader)
        {
            bool stop = false;

            if (reader.IsEmptyElement)
                return null;

            Dictionary<int, PolyLineOption> dicPolyLineOptions = new Dictionary<int, PolyLineOption>();

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "PolyLine", true) == 0)
                            {
                                if (!ReadPolyLine(reader, dicPolyLineOptions))
                                    return null;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                return null;
            }

            return dicPolyLineOptions;
        }

        private bool ReadPolyLine(XmlTextReader reader, Dictionary<int, PolyLineOption> dicPolyLineOptions)
        {
            bool stop = false;

            if (reader.IsEmptyElement)
                return false;

            int nTargetShapeID = -1;
            int nSourceShapeID = -1;
            int nOption = -1;
            PolyLine polyLine = null;

            try
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "TargetShapeID", true) == 0)
                            {
                                string strID = "";

                                if (!ReadText(reader, ref strID, false))
                                    return false;

                                if (!int.TryParse(strID, out nTargetShapeID))
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "SourceShapeID", true) == 0)
                            {
                                string strID = "";

                                if (!ReadText(reader, ref strID, false))
                                    return false;

                                if (!int.TryParse(strID, out nSourceShapeID))
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Vertices", true) == 0)
                            {
                                nOption = ReadPolyLine(reader, out polyLine);
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                return false;
            }

            if (nTargetShapeID <= 0 || nSourceShapeID <= 0 || nOption < 0)
            {
                return false;
            }

            if (nOption >= 4)
                return false;

            PolyLineOption.Begin_Option opt = (PolyLineOption.Begin_Option)nOption;

            PolyLineOption polyLineOption = new PolyLineOption();
            polyLineOption.TargetShapeID = nTargetShapeID;
            polyLineOption.BegionOption = opt;
            polyLineOption.PolyLine = polyLine;

            dicPolyLineOptions[nSourceShapeID] = polyLineOption;
            return true;
        }

        private int ReadPolyLine(XmlTextReader reader, out DXFViewer.PolyLine polyLine)
        {
            bool stop = false;

            polyLine = null;

            if (reader.IsEmptyElement)
                return -1;

            int nOption = -1;
            System.Collections.ArrayList arrVertices = new System.Collections.ArrayList();

            try
            {
                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "beginOpt", true) == 0)
                    {
                        string strOption = reader.Value.ToString();

                        if (!int.TryParse(strOption, out nOption))
                            return -1;
                    }
                }

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Vertex", true) == 0)
                            {
                                UnE.Geometry.Vertex2D vertex = ReadVertex2D(reader);

                                if (vertex == null)
                                    return -1;
                                else
                                    arrVertices.Add(vertex);
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                return -1;
            }

            polyLine = new PolyLine();
            polyLine.SetVertex(arrVertices);

            return nOption;
        }

        private UnE.Geometry.Vertex2D ReadVertex2D(XmlTextReader reader)
        {
            bool stop = false;
            bool isEmpty = reader.IsEmptyElement;
            double x = 0.0, y = 0.0;
            bool findX = false, findY = false;

            while (reader.MoveToNextAttribute())
            {
                if (string.Compare(reader.Name, "x", true) == 0)
                {
                    if (!double.TryParse(reader.Value.ToString(), System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.Number, null, out x))
                    {
                        return null;
                    }
                    else
                        findX = true;
                }
                else if (string.Compare(reader.Name, "y", true) == 0)
                {
                    if (!double.TryParse(reader.Value.ToString(), System.Globalization.NumberStyles.AllowExponent | System.Globalization.NumberStyles.Number, null, out y))
                    {
                        return null;
                    }
                    else
                        findY = true;
                }
            }

            if (!isEmpty)
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            PassElement(reader);
                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }

            if (!findX || !findY)
                return null;

            return new UnE.Geometry.Vertex2D(x, y);
        }
    }

    public class StreetCenterLine
    {
        private string m_strStreetName = "";
        // Key : Source(중심선) Shape의 ID
        private Dictionary<int, PolyLineOption> m_dicPolyLineOptions = new Dictionary<int, PolyLineOption>();

        public string StreetName
        {
            get { return m_strStreetName; }
            set { m_strStreetName = value; }
        }

        public Dictionary<int, PolyLineOption> PolyLineOptions
        {
            get { return m_dicPolyLineOptions; }
            set { m_dicPolyLineOptions = value; }
        }
    }

    public class PolyLineOption
    {
        public enum Begin_Option { MAX_Y = 0, MIN_Y, MAX_X, MIN_X };

        private int m_nTargetShapeID = -1;
        private Begin_Option m_beginOption = Begin_Option.MAX_X;
        private DXFViewer.PolyLine m_polyLine = null;

        public int TargetShapeID
        {
            get { return m_nTargetShapeID; }
            set { m_nTargetShapeID = value; }
        }

        public Begin_Option BegionOption
        {
            get { return m_beginOption; }
            set { m_beginOption = value; }
        }

        public DXFViewer.PolyLine PolyLine
        {
            get { return m_polyLine; }
            set { m_polyLine = value; }
        }

        public static string GetBeginOptionString(Begin_Option opt)
        {
            if (opt == Begin_Option.MAX_Y)
                return "Y값이 가장 큰 Vertex";
            else if (opt == Begin_Option.MIN_Y)
                return "Y값이 가장 작은 Vertex";
            else if (opt == Begin_Option.MAX_X)
                return "X값이 가장 큰 Vertex";
            else if (opt == Begin_Option.MIN_X)
                return "X값이 가장 작은 Vertex";

            return "";
        }
    }
}
