using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.GUI;

namespace ImageEditor
{
    public partial class FormMain : Form, IRibbonButtonOwner
    {
        private string m_ImagePath = "";


        private Image m_CurrentImage = null;
        public Image CurrentImage
        {
            get { return m_CurrentImage; }
        }

        private static FormMain m_instance = null;
        public static FormMain Instance
        {
            get { return m_instance; }
        }

        private ArrayList m_arRibbonButtons = new ArrayList();

        private FormContent m_contentForm = null;
        public FormContent ContentForm
        {
            get { return m_contentForm; }
        }

        private FormInfo m_formProperties = null;
        public FormInfo PropertiesForm
        {
            get { return m_formProperties; }
        }

        private FormNewImage m_formNewImage = null;
        public FormNewImage NewImageForm
        {
            get { return m_formNewImage; }
        }

        private FormRotation m_formRotation = null;
        public FormRotation RotationForm
        {
            get { return m_formRotation; }
            set { m_formRotation = value; }
        }

        public FormMain()
        {
            InitializeComponent();

            m_instance = this;

            SetCommandID();

            m_contentForm = new FormContent();
            m_contentForm.TopLevel = false;
            m_contentForm.Dock = DockStyle.Fill;
            panelRight.Controls.Add(m_contentForm);

            m_contentForm.Show();

            m_formProperties = new FormInfo();
            m_formProperties.TopLevel = false;
            m_formProperties.Dock = DockStyle.Fill;
            panelLeft.Controls.Add(m_formProperties);
            m_formProperties.Show();

            SaveImageToolStripMenuItem.Enabled = false;
            이미지다른이름으로저장ToolStripMenuItem.Enabled = false;
            SettingButton();

        }

        private void SetCommandID()
        {
            rbCopy.Owner = this;
            rbCut.Owner = this;
            rbPaste.Owner = this;
            rbDelete.Owner = this;
            rbSizeSetup.Owner = this;
            rbRotate.Owner = this;
            rbAllSelect.Owner = this;
            rbReverse.Owner = this;
            rbTransparent.Owner = this;
            rbSelectCut.Owner = this;

            m_arRibbonButtons.Add(rbCopy);
            m_arRibbonButtons.Add(rbCut);
            m_arRibbonButtons.Add(rbPaste);
            m_arRibbonButtons.Add(rbDelete);
            m_arRibbonButtons.Add(rbSizeSetup);
            m_arRibbonButtons.Add(rbRotate);
            m_arRibbonButtons.Add(rbAllSelect);
            m_arRibbonButtons.Add(rbReverse);
            m_arRibbonButtons.Add(rbTransparent);
            m_arRibbonButtons.Add(rbSelectCut);

            rbCopy.ID = ID.EDIT_COPY;
            rbCut.ID = ID.EDIT_CUT;
            rbPaste.ID = ID.EDIT_PASTE;
            rbDelete.ID = ID.EDIT_DELETE;
            rbSizeSetup.ID = ID.EDIT_SIZESETUP;
            rbRotate.ID = ID.EDIT_ROTATE;
            rbAllSelect.ID = ID.EDIT_ALLSELECT;
            rbReverse.ID = ID.EDIT_REVERSE;
            rbTransparent.ID = ID.EDIT_TRANSPARENT;
            rbSelectCut.ID = ID.EDIT_SELECTCUT;
        }

        //처음실행할때 버튼 enable처리
        private void SettingButton()
        {
            rbCopy.Enabled = false;
            rbCut.Enabled = false;
            rbPaste.Enabled = false;
            rbDelete.Enabled = false;
            rbSizeSetup.Enabled = false;
            rbRotate.Enabled = false;
            rbAllSelect.Enabled = false;
            rbReverse.Enabled = false;
            rbTransparent.Enabled = false;
            rbSelectCut.Enabled = false;
        }

        //이미지 불러오거나 새이미지 가져올때
        private void LoadButton()
        {
            rbCopy.Enabled = true;
            rbCut.Enabled = true;
            rbPaste.Enabled = true;
            rbDelete.Enabled = true;

            //rbSizeSetup.Enabled = false;
            rbRotate.Enabled = true;
            rbAllSelect.Enabled = true;
            //rbReverse.Enabled = false;
            //rbTransparent.Enabled = false;
            rbSelectCut.Enabled = true;
        }


        public void OnRibbonButtonMouseDown(object sender, MouseEventArgs e)
        {
        }

        public void OnRibbonButtonMouseUp(object sender, MouseEventArgs e)
        {
            RibbonButton rbtn = (RibbonButton)sender;
            int nCmdID = rbtn.ID;
            if (rbtn.CheckButton)
            {
                bool bChecked = !rbtn.IsChecked;
                CheckedChanged(nCmdID, bChecked);
            }
            else
            {
                RunCommand(nCmdID);
            }
        }

        public ToolStripStatusLabel GetStatusLabel()
        {
            //return mStatusWork;
            return null;
        }      

        public void CheckedChanged(int nCommand, bool bChecked)
        {

        }

        public void RunCommand(int nCommand)
        {
            switch(nCommand)
            {
                case ID.EDIT_COPY :
                    Bitmap b = (Bitmap)m_contentForm.GetImageFile2(true);
                    if (b == null)
                        return;
                    Clipboard.SetImage(b);
                    break;

                case ID.EDIT_CUT :
                    //이미지 클립보드에 저장 후 삭제
                    Bitmap b1 = (Bitmap)m_contentForm.GetImageFile2(false);
                    if (b1 == null)
                        return;
                    Clipboard.SetImage(b1);

                    //m_contentForm.DeleteIamge();
                    break;

                case ID.EDIT_PASTE:
                    Image image = Clipboard.GetImage();
                    if (image == null)
                        return;

                   m_contentForm.ProcessPaste(image);
                    break;

                case ID.EDIT_DELETE:
                    m_contentForm.DeleteIamge();
                    break;

                case ID.EDIT_SIZESETUP:
                    break;

                case ID.EDIT_ROTATE:
                    {
                        m_formRotation = new FormRotation();
                        m_formRotation.StartPosition = FormStartPosition.CenterParent;
                        m_formRotation.Dock = DockStyle.Fill;
                        if (m_formRotation.ShowDialog(this) == DialogResult.OK)
                        {

                        }
                    }
                    break;

                case ID.EDIT_ALLSELECT:
                    m_contentForm.AllSelect();
                    //영역선택 버튼이 눌리게
                    m_contentForm.ToolBar.ButtonChecked(ID.TOOLBAR_SELECT_AREA);
                    break;

                case ID.EDIT_REVERSE:
                    break;

                case ID.EDIT_TRANSPARENT:
                    break;

                case ID.EDIT_SELECTCUT:
                    if (MessageBox.Show("정말 자르시겠습니까?", "Confirm delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        // YES 일경우의 이벤트 내용....

                        //자를 이미지정보
                        Bitmap b2 = (Bitmap)m_contentForm.GetImageFile2(true);
                        if (b2 == null)
                            return;
                        //잘라낸 이미지 적용
                        m_CurrentImage = b2;
                        //panel에도 이미지크기 적용
                        m_contentForm.ImageCut(b2.Width, b2.Height);
                        m_formProperties.SetImageGrid(0, 0);
                    }
                    break;
            }
        }

        private void mNewImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_formNewImage = new FormNewImage();
            m_formNewImage.StartPosition = FormStartPosition.CenterParent;
            m_formNewImage.Dock = DockStyle.Fill;
            if(m_formNewImage.ShowDialog(this) == DialogResult.OK)
            {
                SaveImageToolStripMenuItem.Enabled = true;
                이미지다른이름으로저장ToolStripMenuItem.Enabled = true;
                LoadButton();
                m_CurrentImage = null;
            }
        }

        private void OpenImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //openFileDialog1.Filter = "이미지(*.png)|*.png|모든 파일(*.*)|*.*";
            openFileDialog1.Filter = "Jpeg files (*.jpg)|*.jpg|Png files (*.png)|*.png|Bmp files (*.bmp)|*.bmp|All files (*.*)|*.*";
            

            if(openFileDialog1.ShowDialog(this) ==DialogResult.OK)
            {
                try
                {
                    string strFilePath = openFileDialog1.FileName;
    
                    m_ImagePath = strFilePath;
                    string strFileNmae = strFilePath.Substring(strFilePath.LastIndexOf("\\") + 1);
                    
                    Image img;
                    img = Image.FromFile(strFilePath);
                    m_CurrentImage = img;
                   
                    //이미지 정보 갱신
                    m_formProperties.SetImageGrid(img.Size.Width, img.Size.Height, strFileNmae);
                    m_contentForm.SetPaintSize(img.Size.Width, img.Size.Height, img);

                    SaveImageToolStripMenuItem.Enabled = true;
                    이미지다른이름으로저장ToolStripMenuItem.Enabled = true;
                    LoadButton();
                }
                catch(Exception ex)
                {
                    UnE.Utility.UMessageBox.Show(ex.Message, "에러", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void SaveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_ImagePath == "")
            {
                이미지다른이름으로저장ToolStripMenuItem_Click(sender, e);
                return;
            }

            try
            {
                Bitmap b = m_contentForm.GetImageFile3();
                m_CurrentImage.Dispose();
                b.Save(m_ImagePath);



                m_CurrentImage = b;

                string strFileNmae = m_ImagePath.Substring(m_ImagePath.LastIndexOf("\\") + 1);

                //이미지 정보 갱신
                m_formProperties.SetImageGrid(b.Size.Width, b.Size.Height, strFileNmae);
                //m_contentForm.SetPaintSize(b.Size.Width, b.Size.Height);
            }
            catch(Exception ex)
            {
                UnE.Utility.UMessageBox.Show(ex.Message, "에러", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void 이미지다른이름으로저장ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Jpeg files (*.jpg)|*.jpg|Png files (*.png)|*.png|Bmp files (*.bmp)|*.bmp|All files (*.*)|*.*";
            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    string strFilePath = saveFileDialog1.FileName;
                    Bitmap b = (Bitmap)m_contentForm.GetImageFile3();

                    if (m_CurrentImage != null)
                    {
                        //원본 이미지는 닫고
                        m_CurrentImage.Dispose();
                    }
                    //저장
                    b.Save(strFilePath);

                    //바뀐 이미지 새로 갱신
                    m_CurrentImage = b;

                    string strFileNmae = strFilePath.Substring(strFilePath.LastIndexOf("\\") + 1);
                    //이미지 정보 갱신
                    m_formProperties.SetImageGrid(b.Size.Width, b.Size.Height, strFileNmae);
                   // m_contentForm.SetPaintSize(b.Size.Width, b.Size.Height);

                    //이미지 경로 갱신
                    m_ImagePath = strFilePath;

                }
                catch (Exception ex)
                {
                    UnE.Utility.UMessageBox.Show(ex.Message, "에러", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void 끝내기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormFrame.Instance.Close();
        }

        private void 확대ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_contentForm.ZoomIn();
        }

        private void 축소ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_contentForm.ZoomOut();
        }

        private void 보기ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (!base.ProcessCmdKey(ref msg, keyData))
            {
                Keys key = keyData & ~(Keys.Shift | Keys.Control);

                //switch (key)
                //{
                //    case Keys.F:
                //        파일ToolStripMenuItem.ShowDropDown();
                //        break;
                //    case Keys.V:
                //        보기ToolStripMenuItem.ShowDropDown();
                //        break;
                //}
            }

            return false;
        }
       
    }
}
