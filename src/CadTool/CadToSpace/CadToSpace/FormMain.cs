using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using DXFViewer;
using UnE.Geometry;

namespace CadToSpace
{
    public partial class FormMain : Form
    {
        private const string m_strConfigFile = "c2s.ini";

        private List<Space> m_spaces = new List<Space>();
        private List<Layer> m_hiddenLayers = new List<Layer>();

        public FormMain()
        {
            InitializeComponent();
            ReadConfig();
        }

        private void ReadConfig()
        {
            if (File.Exists(m_strConfigFile))
            {
                checkBoxRemember.Checked = true;

                StreamReader reader = new StreamReader(m_strConfigFile, Encoding.UTF8);

                if (reader.EndOfStream == false)
                    textBoxPolyLineLayerName.Text = reader.ReadLine().Trim();

                if (reader.EndOfStream == false)
                    textBoxTextLayerName.Text = reader.ReadLine().Trim();

                if (reader.EndOfStream == false)
                    textBoxExportFilePath.Text = reader.ReadLine().Trim();

                reader.Close();
            }
        }

        private void dxfControl_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Count() == 1)
            {
                string strFileName = files[0].ToLower();

                if (strFileName.EndsWith("dxf"))
                {
                    this.Cursor = Cursors.WaitCursor;

                    dxfControl.OpenDXF(strFileName);
                    ReadSpaces();

                    if (checkBoxAll.Checked == false)
                        ShowLayer(false);

                    this.Cursor = Cursors.Arrow;
                }
            }
        }

        private void ShowLayer(bool all)
        {
            if (all)
            {
                foreach (Layer layer in m_hiddenLayers)
                {
                    layer.Hidden = false;
                }
            }
            else
            {
                string strPolyLineLayerName = textBoxPolyLineLayerName.Text.Trim();
                string strTextLayerName = textBoxTextLayerName.Text.Trim();

                if (m_hiddenLayers.Count == 0)
                {
                    foreach (Layer layer in dxfControl.Layers)
                    {
                        if (layer.LayerName == strPolyLineLayerName || layer.LayerName == strTextLayerName)
                            continue;

                        if (layer.Hidden == false)
                        {
                            m_hiddenLayers.Add(layer);
                            layer.Hidden = true;
                        }
                    }
                }
                else
                {
                    foreach (Layer layer in m_hiddenLayers)
                    {
                        layer.Hidden = true;
                    }
                }
            }

            dxfControl._Refresh();
        }

        private void dxfControl_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Count() == 1)
                {
                    string strFileName = files[0].ToLower();

                    if (strFileName.EndsWith("dxf"))
                    {
                        e.Effect = DragDropEffects.Copy;
                        return;
                    }
                }
            }

            e.Effect = DragDropEffects.None;
        }

        private void ReadSpaces()
        {
            m_spaces.Clear();
            m_hiddenLayers.Clear();

            string strPolyLineLayerName = textBoxPolyLineLayerName.Text.Trim();
            string strTextLayerName = textBoxTextLayerName.Text.Trim();

            foreach (Layer layer in dxfControl.Layers)
            {
                if (layer.LayerName == strPolyLineLayerName)
                {
                    foreach (Shape shape in layer.Shapes)
                    {
                        if (shape is PolyLine)
                        {
                            Space space = new Space((PolyLine)shape);
                            m_spaces.Add(space);
                        }
                    }

                    break;
                }
            }

            foreach (Layer layer in dxfControl.Layers)
            {
                if (layer.LayerName == strTextLayerName)
                {
                    string strOutTexts = "";

                    foreach (Shape shape in layer.Shapes)
                    {
                        if (shape is Text)
                        {
                            Text text = (Text)shape;
                            Vertex2D vCenter = (text.BoundaryTL + text.BoundaryBR) / 2;
                            Space space = FindSpace(vCenter);

                            if (space != null)
                            {
                                space.Text = text;
                                string strSpaceID = SetSpaceName(text);
                                space.ID = strSpaceID;
                            }
                            else
                            {
                                if (strOutTexts.Length == 0)
                                    strOutTexts = text.Title;
                                else
                                    strOutTexts += ", " + text.Title;
                            }
                        }
                    }

                    if (strOutTexts.Length > 0)
                    {
                        MessageBox.Show("공간 외부에 쓰여진 Text가 있습니다.\r\n" + strOutTexts);
                    }

                    break;
                }
            }
        }

        private string SetSpaceName(Text text)
        {
            string strText = text.Title;

            foreach (DataGridViewRow row in gridSpaceID.Rows)
            {
                if (row.Cells[0].Value.ToString() == strText)
                {
                    row.Cells[0].Style.BackColor = Color.Yellow;
                    //text.Title = row.Cells[1].Value.ToString();
                    return (string)row.Tag;
                }
            }

            // strText가 번호가 아닐 경우
            foreach (DataGridViewRow row in gridSpaceID.Rows)
            {
                if (row.Cells[1].Value.ToString().Trim() == strText)
                {
                    row.Cells[1].Style.BackColor = Color.Yellow;
                    //text.Title = row.Cells[1].Value.ToString();
                    return (string)row.Tag;
                }
            }

            return null;
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (checkBoxRemember.Checked == false)
            {
                File.Delete(m_strConfigFile);
                return;
            }

            StreamWriter writer = new StreamWriter(m_strConfigFile, false, Encoding.UTF8);
            writer.WriteLine(textBoxPolyLineLayerName.Text.Trim());
            writer.WriteLine(textBoxTextLayerName.Text.Trim());
            writer.WriteLine(textBoxExportFilePath.Text.Trim());
            writer.Close();
        }

        private Space FindSpace(Vertex2D vCenter)
        {
            foreach (Space space in m_spaces)
            {
                if (space.HitTest(vCenter))
                {
                    // 영역이 겹치는 경우
                    if (space.ID != null)
                    {
                        Space _space = new Space(space);
                        m_spaces.Add(_space);
                        return _space;
                    }

                    return space;
                }
            }

            return null;
        }

        private void gridSpaceID_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Count() == 1)
            {
                string strFileName = files[0].ToLower();

                if (strFileName.EndsWith("txt"))
                {
                    ReadSpaceID(strFileName);
                }
            }
        }

        private void gridSpaceID_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Count() == 1)
                {
                    string strFileName = files[0].ToLower();

                    if (strFileName.EndsWith("txt"))
                    {
                        e.Effect = DragDropEffects.Copy;
                        return;
                    }
                }
            }

            e.Effect = DragDropEffects.None;
        }

        private void ReadSpaceID(string strFileName)
        {
            gridSpaceID.Rows.Clear();
            StreamReader reader = new StreamReader(strFileName, Encoding.Default);

            while (reader.EndOfStream == false)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                string[] tokens = strLine.Split('\t');

                if (tokens.Count() < 2)
                    continue;

                /*int nID;

                if (int.TryParse(tokens[0].Trim(), out nID) == false)
                    continue;*/

                string strSpaceName = tokens[1].Trim();
                string strID = null;

                if (tokens.Count() >= 3)
                {
                    strID = tokens[2].Trim();
                }

                int nRowIndex = gridSpaceID.Rows.Add();
                DataGridViewRow row = gridSpaceID.Rows[nRowIndex];

                row.Cells[0].Value = tokens[0].Trim();
                //row.Cells[0].Value = nID;
                row.Cells[1].Value = strSpaceName;
                row.Tag = strID;
            }

            reader.Close();
        }

        private void checkBoxAll_CheckedChanged(object sender, EventArgs e)
        {
            ShowLayer(checkBoxAll.Checked);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            dxfControl.CloseDXF();
            gridSpaceID.Rows.Clear();
            dxfControl._Refresh();
        }

        private void btnExportFilePath_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();

            dlg.Filter = "TXT Files|*.txt|All FIles|*.*";
            dlg.FilterIndex = 0;
            dlg.Title = "Polyline 파일 지정";

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBoxExportFilePath.Text = dlg.FileName;
            }
        }

        private void btnExportFile_Click(object sender, EventArgs e)
        {
            string strFilePath = textBoxExportFilePath.Text.Trim();

            try
            {
                StreamWriter writer = new StreamWriter(strFilePath, false, Encoding.UTF8);

                foreach (Space space in m_spaces)
                {
                    space.Write(writer);
                }

                writer.Close();
                MessageBox.Show("파일이 저장되었습니다.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

    public class Space
    {
        private Polygon m_polygon = null;
        private Text m_text = null;
        private string m_strID = null;

        public string ID
        {
            get { return m_strID; }
            set { m_strID = value; }
        }

        public Text Text
        {
            get { return m_text; }
            set { m_text = value; }
        }

        public string Name
        {
            get { return m_text == null ? "" : m_text.Title; }
        }

        public Space(PolyLine polyline)
        {
            m_polygon = polyline.GetPolygon();
        }

        public Space(Space space)
        {
            m_polygon = space.m_polygon;
        }

        public bool HitTest(Vertex2D vPos)
        {
            if (m_polygon == null)
                return false;

            return m_polygon.HitTest(vPos) != 0;
        }

        public void Write(StreamWriter writer)
        {
            if (m_text == null && m_strID == null)
                return;

            string strPolygon = "";
            int nVertexCount = m_polygon.GetVertexCount();

            for (int i=0;i<nVertexCount;i++)
            {
                Vertex2D vertex = m_polygon.GetVertex(i);
                string strVertex = string.Format("{0:F1},{1:F1}", vertex.x, vertex.y);

                if (strPolygon.Length == 0)
                    strPolygon = strVertex;
                else
                    strPolygon += "," + strVertex;
            }

            if (m_strID != null)
                writer.WriteLine(m_strID + "\t" + strPolygon);
            else
                writer.WriteLine(m_text.Title + "\t" + strPolygon);
        }
    }
}
