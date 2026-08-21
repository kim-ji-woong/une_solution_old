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
using System.Collections;

namespace MashupImage
{
    public partial class FormMain : Form, IShapeOwner
    {
        private const int SC_RESTORE = 0xF120;
        private const int SC_RESTORE2 = 0xF122;
        private const int SC_MAXIMIZE = 0xF030;
        private const int SC_MAXIMIZE2 = 0xF032;
        private const int SC_MINIMIZE = 0xF020;

        private const string Title = "Mashup Image";
        private const uint MaxAddPixel = 10;

        private Project m_project = null;
        private string m_strPrevPath = "";
        private string m_strIni = "log.dat";

        private bool m_systemInput = false;

        public FormMain()
        {
            InitializeComponent();

            EnableControl(false);
            InitText();
            ReadFile();

            panelImage.Owner = this;
        }

        private void ReadFile()
        {
            if (File.Exists(m_strIni))
            {
                StreamReader reader = new StreamReader(m_strIni, Encoding.UTF8);
                m_strPrevPath = reader.ReadLine().Trim();
                reader.Close();
            }
        }

        private void WriteFile()
        {
            string strPath = textBoxFolderPath.Text.Trim();

            if (strPath.Length == 0)
            {
                if (Directory.Exists(strPath))
                    Directory.Delete(strPath, true);
            }
            else
            {
                StreamWriter writer = new StreamWriter(m_strIni, false, Encoding.UTF8);
                writer.Write(strPath);
                writer.Close();
            }
        }

        private void tsMenuNewProject_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();

            dlg.Filter = "프로젝트 파일|*.prj";
            dlg.Title = "Mashup 프로젝트 파일 만들기";
            
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                m_project = new Project();
                m_project.ProjectPath = dlg.FileName;

                int nIndex = m_project.ProjectPath.LastIndexOf('\\');
                string strFileName = nIndex > 0 ? m_project.ProjectPath.Substring(nIndex + 1) : m_project.ProjectPath;

                this.Text = Title + " - " + strFileName;
                EnableControl(true);
                tsMenuSave.Enabled = true;
            }
        }

        private void EnableControl(bool enabled)
        {
            textBoxFolderPath.Enabled = btnFolderPath.Enabled = enabled;
            cboLOD.Enabled = btnAddLOD.Enabled = btnApply.Enabled = enabled;
            cboAddPixel.Enabled = enabled;

            textBoxShapeName.Enabled = cboShapes.Enabled = enabled;
            btnAddShape.Enabled = btnApplyShape.Enabled = enabled;
            cboShapeLOD.Enabled = textBoxShapeFilePath.Enabled = enabled;
            btnShapeFilePath.Enabled = textBoxShapeX.Enabled = textBoxShapeY.Enabled = enabled;
        }

        private void btnFolderPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();

            dlg.Description = "LOD 이미지 경로 선택";
            dlg.ShowNewFolderButton = false;

            if (m_strPrevPath.Length > 0)
                dlg.SelectedPath = m_strPrevPath;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBoxFolderPath.Text = dlg.SelectedPath;
                m_strPrevPath = dlg.SelectedPath;
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            string strFolderPath = textBoxFolderPath.Text.Trim();

            if (Directory.Exists(strFolderPath) == false)
            {
                textBoxFolderPath.Focus();
                MessageBox.Show("존재하지 않는 폴더입니다.");
                return;
            }

            LOD lod = GetLOD(strFolderPath);
            SetLOD(lod);
            SetLODRatio();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            WriteFile();
        }

        private LOD GetLOD(string strFolderPath)
        {
            string[] files = Directory.GetFiles(strFolderPath);
            int nFileCount = files.Count();

            if (nFileCount == 0)
                return null;

            LOD lod = null;

            if (cboLOD.SelectedItem == null)
            {
                lod = new LOD();
                lod.SetIndex(cboLOD.Items.Count);
                int nIndex = cboLOD.Items.Add(lod);
                btnRemoveLOD.Enabled = true;
                btnArrangeLOD.Enabled = true;

                m_systemInput = true;
                cboLOD.SelectedIndex = nIndex;
                m_systemInput = false;
            }
            else
                lod = (LOD)cboLOD.SelectedItem;

            lod.ImageHCount = lod.ImageVCount = 0;
            int nTotalImageWidth = 0, nTotalImageHeight = 0;

            bool first = true;
            int nFileIndex = 0;

            for (int i = 0; i < nFileCount; i++)
            {
                string strFilePath = files[i];
                Image image = Image.FromFile(strFilePath);

                if (first)
                {
                    lod.ImageWidth = image.Width;
                    lod.ImageHeight = image.Height;
                    first = false;
                }

                nTotalImageWidth += image.Width;

                image.Dispose();

                nFileIndex = i;

                if (GetNextFile(files, ref nFileIndex) == null)
                {
                    lod.ImageHCount = i + 1;
                    break;
                }
            }

            if (lod.ImageHCount == 0)
                return null;

            for (int i = 0; i < nFileCount; i += lod.ImageHCount)
            {
                string strFilePath = files[i];
                Image image = Image.FromFile(strFilePath);

                nTotalImageHeight += image.Height;

                lod.ImageVCount++;
                image.Dispose();
            }

            if (lod.ImageVCount == 0)
                return null;

            SetAddPixel(lod);

            lod.ImageTotalWidth = nTotalImageWidth - (int)lod.AddPixel * (lod.ImageHCount - 1);
            lod.ImageTotalHeight = nTotalImageHeight - (int)lod.AddPixel * (lod.ImageVCount - 1);
            lod.FolderName = strFolderPath;

            return lod;
        }

        private void SetAddPixel(LOD lod)
        {
            if (cboAddPixel.SelectedIndex >= 0)
            {
                uint addPixel;

                if (uint.TryParse(cboAddPixel.SelectedItem.ToString(), out addPixel))
                {
                    lod.AddPixel = addPixel;
                }
            }
        }

        // 같은 행에 해당하는 다음 이미지 파일을 얻어온다.
        // nFileIndex가 이번행의 마지막 이미지 파일을 가르키고 있다면 null을 리턴한다.
        private string GetNextFile(string[] files, ref int nFileIndex)
        {
            if (files.Count() == nFileIndex + 1)
                return null;

            string strCurrent = files[nFileIndex];
            string strNext = files[nFileIndex + 1];

            int nIndexCurrent_ = strCurrent.LastIndexOf('_');
            int nIndexNext_ = strNext.LastIndexOf('_');

            string strCurrentPrev = strCurrent.Substring(0, nIndexCurrent_);
            string strNextPrev = strNext.Substring(0, nIndexNext_);

            // nFileIndex가 이번행의 마지막 이미지 파일이다.
            if (strCurrentPrev != strNextPrev)
                return null;

            nFileIndex++;
            return strNext;
        }

        private void SetLOD(LOD lod)
        {
            if (lod == null)
            {
                lblImageWidth.Text = lblImageHeight.Text = "";
                lblImageHCount.Text = lblImageVCount.Text = "";
                lblImageTotalWidth.Text = lblImageTotalHeight.Text = "";
                cboAddPixel.SelectedIndex = 0;
            }
            else
            {
                lblImageWidth.Text = string.Format("{0}px", lod.ImageWidth);
                lblImageHeight.Text = string.Format("{0}px", lod.ImageHeight);
                lblImageHCount.Text = string.Format("{0}개", lod.ImageHCount);
                lblImageVCount.Text = string.Format("{0}개", lod.ImageVCount);
                lblImageTotalWidth.Text = string.Format("{0}px", lod.ImageTotalWidth);
                lblImageTotalHeight.Text = string.Format("{0}px", lod.ImageTotalHeight);
                textBoxFolderPath.Text = lod.FolderName;

                if (lod.AddPixel > MaxAddPixel)
                {
                    if (cboAddPixel.Items.Count == (int)(MaxAddPixel + 1))
                        cboAddPixel.Items.Add(lod.AddPixel);
                    else
                        cboAddPixel.Items[cboAddPixel.Items.Count - 1] = lod.AddPixel;

                    cboAddPixel.SelectedIndex = cboAddPixel.Items.Count - 1;
                }
                else
                    cboAddPixel.SelectedIndex = (int)lod.AddPixel;
            }

            panelImage.SetLOD(lod);
        }

        private void cboLOD_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_systemInput)
                return;

            if (cboLOD.SelectedIndex < 0)
                SetLOD(null);
            else
            {
                LOD lod = (LOD)cboLOD.SelectedItem;
                SetLOD(lod);
            }
        }

        private void btnAddLOD_Click(object sender, EventArgs e)
        {
            LOD lod = new LOD();
            lod.SetIndex(cboLOD.Items.Count);

            int nIndex = cboLOD.Items.Add(lod);
            cboLOD.SelectedIndex = nIndex;
            btnRemoveLOD.Enabled = true;
            btnArrangeLOD.Enabled = true;

            SetAddPixel(lod);
            SetLOD(lod);
        }

        private void SetLODRatio()
        {
            ArrayList arrLODs = new ArrayList();
            arrLODs.AddRange(cboLOD.Items);

            arrLODs.Sort();

            int nLODCount = arrLODs.Count;

            if (nLODCount == 0)
                return;

            LOD lodPrev = (LOD)arrLODs[0];
            lodPrev.Ratio = null;

            for (int i=1;i<nLODCount;i++)
            {
                LOD lod = (LOD)arrLODs[i];

                Ratio ratio = new Ratio();
                ratio.BaseLOD = lodPrev;
                ratio.BaseWidth = lodPrev.ImageTotalWidth;
                ratio.BaseHeight = lodPrev.ImageTotalHeight;
                ratio.CurrentWidth = lod.ImageTotalWidth;
                ratio.CurrentHeight = lod.ImageTotalHeight;
                ratio.UsePercent = false;

                lod.Ratio = ratio;
                lodPrev = lod;
            }
        }

        private void btnRemoveLOD_Click(object sender, EventArgs e)
        {
            if (cboLOD.SelectedIndex < 0)
                return;

            cboLOD.Items.RemoveAt(cboLOD.SelectedIndex);

            LOD lod = (LOD)cboLOD.SelectedItem;
            SetLOD(lod);
            SetLODRatio();

            if (lod == null)
            {
                btnRemoveLOD.Enabled = false;
                btnArrangeLOD.Enabled = false;
            }
        }

        private void btnArrangeLOD_Click(object sender, EventArgs e)
        {
            m_systemInput = true;

            ArrayList lods = new ArrayList();
            lods.AddRange(cboLOD.Items);

            LOD selectedLOD = (LOD)cboLOD.SelectedItem;
            cboLOD.Items.Clear();

            lods.Sort();
            int nIndex = 0;
            
            foreach (LOD lod in lods)
            {
                lod.SetIndex(nIndex++);
                cboLOD.Items.Add(lod);
            }

            m_systemInput = false;
            cboLOD.SelectedItem = selectedLOD;
        }

        private void tsMenuSave_Click(object sender, EventArgs e)
        {
            if (m_project != null)
            {
                m_project.LODs.Clear();
                m_project.Shapes.Clear();
                
                foreach (LOD lod in cboLOD.Items)
                {
                    m_project.LODs.Add(lod);
                }

                foreach (Shape shape in cboShapes.Items)
                {
                    m_project.Shapes.Add(shape);
                }

                string strErrorMessage;

                if (m_project.Save(out strErrorMessage) == false)
                    MessageBox.Show("프로젝트 저장 실패\r\n" + strErrorMessage);
                else
                    MessageBox.Show("프로젝트를 저장하였습니다.");
            }
        }

        private void tsMenuOpenProject_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "프로젝트 파일|*.prj";
            dlg.Title = "Mashup 프로젝트 파일 열기";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                XMLManager mgr = new XMLManager();
                Project project = mgr.Read(dlg.FileName);

                if (project == null)
                {
                    MessageBox.Show("프로젝트 열기 실패\r\n" + mgr.ErrorMessage);
                }
                else
                {
                    foreach (LOD lod in project.LODs)
                    {
                        SetLODTotalImage(lod);
                    }

                    SetProject(project);
                    project.ProjectPath = dlg.FileName;
                    tsMenuSave.Enabled = true;
                }
            }
        }

        private void InitText()
        {
            lblImageWidth.Text = lblImageHeight.Text = "";
            lblImageHCount.Text = lblImageVCount.Text = "";
            lblImageTotalWidth.Text = lblImageTotalHeight.Text = "";
            textBoxFolderPath.Text = "";
        }

        private void SetLODTotalImage(LOD lod)
        {
            if (Directory.Exists(lod.FolderName) == false)
                return;

            string[] files = Directory.GetFiles(lod.FolderName);
            int nFileCount = files.Count();

            if (nFileCount != lod.ImageHCount * lod.ImageVCount)
                return;

            int nWidth = 0, nHeight = 0;

            for (int i=0;i<lod.ImageHCount;i++)
            {
                Image img = Image.FromFile(files[i]);
                nWidth += img.Size.Width;
            }

            for (int i = 0; i < nFileCount; i += lod.ImageHCount)
            {
                Image img = Image.FromFile(files[i]);
                nHeight += img.Size.Height;
            }

            lod.ImageTotalWidth = nWidth - (int)lod.AddPixel * (lod.ImageHCount - 1);
            lod.ImageTotalHeight = nHeight - (int)lod.AddPixel * (lod.ImageVCount - 1);
        }

        private void SetProject(Project project)
        {
            m_project = project;

            EnableControl(true);
            InitText();

            m_systemInput = true;
            cboLOD.Items.Clear();
            cboShapes.Items.Clear();
            panelImage.ClearShapes();
            m_systemInput = false;

            panelImage.SetLOD(null, false);
            int nIndex = 0;

            foreach (LOD lod in m_project.LODs)
            {
                lod.SetIndex(nIndex++);
                cboLOD.Items.Add(lod);
            }

            if (cboLOD.Items.Count > 0)
                cboLOD.SelectedIndex = 0;

            foreach (Shape shape in m_project.Shapes)
            {
                cboShapes.Items.Add(shape);
                panelImage.AddShape(shape);
            }

            if (cboShapes.Items.Count > 0)
                cboShapes.SelectedIndex = 0;

            panelImage.Refresh();
        }

        // FormMain의 크기가 변경될때 Split Distance가 바뀌지 않도록 한다.
        private void FixSplitDistance()
        {
            splitContainerBody.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            splitContainerLeft.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
        }

        private void UnFixSplitDistance()
        {
            splitContainerBody.FixedPanel = System.Windows.Forms.FixedPanel.None;
            splitContainerLeft.FixedPanel = System.Windows.Forms.FixedPanel.None;
        }

        private void FormMain_ResizeBegin(object sender, EventArgs e)
        {
            FixSplitDistance();
        }

        private void FormMain_ResizeEnd(object sender, EventArgs e)
        {
            UnFixSplitDistance();
        }

        protected override void WndProc(ref Message m)
        {
            // WM_SYSCOMMAND
            if (m.Msg == 0x0112)
            {
                int wParam = (int)m.WParam;

                if (wParam == SC_RESTORE || wParam == SC_RESTORE2 ||
                    wParam == SC_MAXIMIZE || wParam == SC_MINIMIZE ||
                    wParam == SC_MAXIMIZE2)
                {
                    FixSplitDistance();
                }
            }

            base.WndProc(ref m);
        }

        private void cboShapes_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkBoxAllLod.Enabled = cboShapes.SelectedIndex >= 0;

            if (cboShapes.SelectedIndex >= 0)
            {
                Shape shape = (Shape)cboShapes.SelectedItem;
                textBoxShapeName.Text = shape.Name;
                textBoxShapeFilePath.Text = shape.ImagePath;
                textBoxShapeX.Text = string.Format("{0:F1}", shape.Position.X);
                textBoxShapeY.Text = string.Format("{0:F1}", shape.Position.Y);

                btnRenameShape.Enabled = btnRemoveShape.Enabled = true;
            }
            else
            {
                textBoxShapeName.Text = "";
                textBoxShapeFilePath.Text = "";
                textBoxShapeX.Text = "";
                textBoxShapeY.Text = "";

                btnRenameShape.Enabled = btnRemoveShape.Enabled = false;
            }
        }

        private void btnAddShape_Click(object sender, EventArgs e)
        {
            string strShapeName = textBoxShapeName.Text.Trim();

            if (strShapeName.Length == 0)
            {
                textBoxShapeName.Focus();
                MessageBox.Show("추가할 Shape 이름을 입력하세요.");
                return;
            }

            foreach (Shape _shape in cboShapes.Items)
            {
                if (_shape.Name == strShapeName)
                {
                    textBoxShapeName.Focus();
                    MessageBox.Show(strShapeName + "은 이미 사용중인 이름입니다.");
                    return;
                }
            }

            Shape shape = new Shape();
            shape.Name = strShapeName;

            int nIndex = cboShapes.Items.Add(shape);
            cboShapes.SelectedIndex = nIndex;

            panelImage.ClearShapes();

            foreach (Shape _shape in cboShapes.Items)
            {
                panelImage.AddShape(_shape);
            }

            panelImage.Refresh();
        }

        public void OnSelectShape(Shape shape)
        {
            if (shape == null)
                cboShapes.SelectedIndex = -1;
            else
            {
                foreach (Shape _shape in cboShapes.Items)
                {
                    if (shape == _shape)
                    {
                        if (cboShapes.SelectedItem == _shape)
                            return;
                        else
                        {
                            cboShapes.SelectedItem = shape;
                            return;
                        }
                    }
                }
            }
        }

        public void OnMoveShape(Shape shape, float x, float y)
        {
            textBoxShapeX.Text = string.Format("{0:F1}", x);
            textBoxShapeY.Text = string.Format("{0:F1}", y);
        }

        private void btnApplyShape_Click(object sender, EventArgs e)
        {
            Shape shape = (Shape)cboShapes.SelectedItem;

            if (shape == null)
                return;

            string strFilePath = textBoxShapeFilePath.Text.Trim();

            if (File.Exists(strFilePath))
            {
                try
                {
                    Image img = Image.FromFile(strFilePath);
                    shape.Image = img;
                    shape.ImagePath = strFilePath;
                }
                catch (Exception ex)
                {
                    shape.Image = null;
                    shape.ImagePath = "";
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                }
            }
            else
            {
                shape.Image = null;
                shape.ImagePath = "";
            }

            string strX = textBoxShapeX.Text.Trim();
            string strY = textBoxShapeY.Text.Trim();

            float x, y;

            if (float.TryParse(strX, out x) && float.TryParse(strY, out y))
                shape.Position = new PointF(x, y);
            else
                shape.Position = new PointF(0, 0);

            panelImage.Refresh();
        }

        private void btnShapeFilePath_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            dlg.Filter = "이미지 파일|*.bmp;*.jpg;*.png;*.gif;*.tga|모든 파일|*.*";
            dlg.Title = "이미지 파일 열기";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBoxShapeFilePath.Text = dlg.FileName;
            }
        }

        private void btnRemoveShape_Click(object sender, EventArgs e)
        {
            Shape shape = (Shape)cboShapes.SelectedItem;

            if (shape != null)
            {
                panelImage.RemoveShape(shape);
                cboShapes.Items.Remove(shape);

                if (cboShapes.Items.Count == 0)
                    cboShapes_SelectedIndexChanged(null, null);
                else
                {
                    cboShapes.SelectedIndex = cboShapes.Items.Count - 1;
                }

                panelImage.Refresh();
            }
        }

        private void btnRenameShape_Click(object sender, EventArgs e)
        {
            Shape shape = (Shape)cboShapes.SelectedItem;

            if (shape == null)
                return;

            string strShapeName = textBoxShapeName.Text.Trim();

            if (strShapeName.Length == 0)
            {
                textBoxShapeName.Focus();
                MessageBox.Show("Shape 이름을 입력해야 합니다.");
                return;
            }

            foreach (Shape _shape in cboShapes.Items)
            {
                if (_shape == shape)
                    continue;

                if (_shape.Name == strShapeName)
                {
                    textBoxShapeName.Focus();
                    MessageBox.Show(strShapeName + "은 이미 사용중인 이름입니다.");
                    return;
                }
            }

            shape.Name = strShapeName;
            List<Shape> shapes = new List<Shape>();

            foreach (Shape _shape in cboShapes.Items)
            {
                shapes.Add(_shape);
            }

            cboShapes.Items.Clear();

            foreach (Shape _shape in shapes)
            {
                cboShapes.Items.Add(_shape);
            }

            shapes.Clear();
            cboShapes.SelectedItem = shape;
        }
    }
}
