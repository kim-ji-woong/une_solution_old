using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageEditor
{
    public partial class FormContent : Form
    {
        private TempImage m_TempImage = null;

        private ArrayList arrShape = new ArrayList();
        private enum ToggleMode { SelectArea = 0, SelectColor, Translate, StraightLine, Curve, Text, None };
        private Matrix mTransform = new Matrix();
        private Matrix mTemp = new Matrix();

        private Point m_ptStart;
        private Point m_ptEnd;
        private Pen m_PenTemp;
        private Pen m_Pen;
        private Boolean m_bDrag;

        private TextBox mTextBox;

        //선택된 영역이 있는지없는지
        private bool bSelected = false;

        //복사 붙여넣기할때 클립보드에서 가져올 이미지
        private Image m_img = null;
        //private ArrayList m_ClipBoardImages = new ArrayList();

        private bool m_bBold = false;
        public bool Bold
        {
            get { return m_bBold; }
            set { m_bBold = value; }
        }

        private bool m_bLean = false;
        public bool Lean
        {
            get { return m_bLean; }
            set { m_bLean = value; }
        }

        private bool m_bUnderLine = false;
        public bool UnderLine
        {
            get { return m_bUnderLine; }
            set { m_bUnderLine = value; }
        }

        //선택영역
        private Rectangle m_Rect;
        public Rectangle Rect
        {
            get { return m_Rect; }
            set { m_Rect = value; }
        }

        private FormImageToolBar m_formToolBar = null;
        public FormImageToolBar ToolBar
        {
            get { return m_formToolBar; }
            set { m_formToolBar = value; }            
        }

        public FormContent()
        {
            InitializeComponent();
            panelPaint.Visible = false;

            m_formToolBar = new FormImageToolBar();
            m_formToolBar.TopLevel = false;
            m_formToolBar.Dock = DockStyle.Fill;
            
            // mTextBox
            this.mTextBox = new System.Windows.Forms.TextBox();

            this.mTextBox.BackColor = System.Drawing.Color.White;
            this.mTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mTextBox.Font = new System.Drawing.Font("맑은 고딕", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.mTextBox.Location = new System.Drawing.Point(0, 0);
            this.mTextBox.Multiline = true;
            this.mTextBox.Name = "mTextBox";
            this.mTextBox.Size = new System.Drawing.Size(100, 21);
            this.mTextBox.TabIndex = 0;
            this.mTextBox.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.mTextBox_PreviewKeyDown);

            panelToolBar.Controls.Add(m_formToolBar);

            mTextBox.Visible = false;
            panelPaint.Controls.Add(mTextBox);

            //m_formToolBar.Show();
            

        }

        private void mTextBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                //m_tempText = null;
                CancelTextInput();
            }
        }

        private void panel1_Resize(object sender, EventArgs e)
        {

        }

        private void FormContent_Resize(object sender, EventArgs e)
        {
            //int nWidth = label1.Size.Width;
            //int nHeight = label1.Size.Height;
            //int x = panelToolBar.Width / 2 - nWidth / 2;
            //int y = panelToolBar.Height / 2 - nHeight / 2;
            //label1.SetBounds(x, y, nWidth, nHeight);
        }

        public void SetPaintSize(int width, int height, Image img = null)
        {

            panelPaint.Width = width;
            panelPaint.Height = height;

            mTransform.Reset();

            //m_ClipBoardImages.Clear();
            m_img = img;

            Point pt = new Point(0, 0);
            m_TempImage = new TempImage(null, pt);

            panelPaint.Invalidate();

            m_Scale = 1.0f;

            arrShape.Clear();

            panelPaint.Visible = true;
            m_formToolBar.Show();
        }

        private void panelPaint_MouseDown(object sender, MouseEventArgs e)
        {
            CancelTextInput();


            m_bDrag = true;

            m_ptStart.X = e.X;
            m_ptStart.Y = e.Y;

            
            if (m_TempImage.Img != null)
            {
                if (bSelected == false)
                {
                    m_TempImage.Selected = false;

                    m_Rect = new Rectangle();

                    Graphics g = Graphics.FromImage(FormMain.Instance.CurrentImage);
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.DrawImage(m_TempImage.Img, m_TempImage.DrawPt);         

                    m_TempImage.Img = null;
         

                    //이동작업이 끝났으면 다시 영역선택으로 돌아감
                    if (m_formToolBar.ToggleType == (int)ToggleMode.Translate)
                        m_formToolBar.ButtonChecked(ID.TOOLBAR_SELECT_AREA);

                    panelPaint.Invalidate();
                }
                else
                {
                    if( cutRect.Width > 0 && cutRect.Height > 0)
                    {
                        Graphics g = Graphics.FromImage(FormMain.Instance.CurrentImage);
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        using (SolidBrush brush = new SolidBrush(Color.White))
                        {
                            g.FillRectangle(brush, cutRect);
                        }
                    }
                   
                }
            }
        }


        private void panelPaint_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_formToolBar.ToggleType == (int)ToggleMode.Translate)
            {
                if (m_Rect.Contains(e.X, e.Y))
                {
                    this.Cursor = Cursors.Hand;
                    bSelected = true;
                }
                else
                {
                    this.Cursor = Cursors.Default;
                    bSelected = false;
                }
            }

            if (m_bDrag)
            {
                if (e.Button == MouseButtons.Left)
                {
                    m_ptEnd = e.Location;

                    PointF fpt1 = ScreenToGlobal(m_ptStart);
                    PointF fpt2 = ScreenToGlobal(m_ptEnd);

                    if (m_formToolBar.ToggleType == (int)ToggleMode.SelectArea)
                    {
                        //if (bSelected)
                        {
                            int mMinX = Math.Min(m_ptStart.X, m_ptEnd.X);
                            int mMaxX = Math.Max(m_ptStart.X, m_ptEnd.X);

                            int mMinY = Math.Min(m_ptStart.Y, m_ptEnd.Y);
                            int mMaxY = Math.Max(m_ptStart.Y, m_ptEnd.Y);

                            int nWidth = 0;
                            int nHeight = 0;

                            if (mMinX < 0)
                                mMinX = 0;

                            if (mMinY < 0)
                                mMinY = 0;

                            if (panelPaint.Width < mMaxX)
                            {
                                nWidth = panelPaint.Width - mMinX;
                            }
                            else
                            {
                                nWidth = mMaxX - mMinX;
                            }

                            if (panelPaint.Height < mMaxY)
                            {
                                nHeight = panelPaint.Height - mMinY;
                            }
                            else
                            {
                                nHeight = mMaxY - mMinY;
                            }

                            m_Rect = new Rectangle(mMinX, mMinY, nWidth, nHeight);

                        }

                        panelPaint.Invalidate();
                    }
                    else if (m_formToolBar.ToggleType == (int)ToggleMode.Translate)
                    {
                        if (bSelected)
                        {
                            //그림좌표
                            Rectangle r = new Rectangle(0, 0, panelPaint.Width, panelPaint.Height);
                            Rectangle rCropArea = m_Rect;
                            rCropArea.Intersect(r);

                            float dx2 = m_ptEnd.X - m_ptStart.X;
                            float dy2 = m_ptEnd.Y - m_ptStart.Y;                    
                            if (rCropArea.Width < 30)
                            {
                                m_Rect.X = m_Rect.X-1;
                                m_Rect.Y = (int)e.Y - (m_Rect.Height / 2);
                            }
                            else if(rCropArea.Height < 30)
                            {
                                m_Rect.X = (int)e.X - (m_Rect.Width / 2);
                                m_Rect.Y = (int)m_Rect.Y-1;
                            }
                            else if(rCropArea.Width < 30 && rCropArea.Height < 30)
                            {
                                m_Rect.X = m_Rect.X - 1;
                                m_Rect.Y = (int)m_Rect.Y - 1;
                            }
                            else
                            {
                                m_Rect.X += (int)dx2;
                                m_Rect.Y += (int)dy2;
                            }

                            m_ptStart = e.Location;
                            if (m_TempImage.Img != null)
                            {
                              
                                m_TempImage.DrawPt = m_Rect.Location;
                            }                            

                        }
                        panelPaint.Invalidate();

                    }
                    //직선
                    else if (m_formToolBar.ToggleType == (int)ToggleMode.StraightLine)
                    {                
                        panelPaint.Invalidate();
                    }
                    //곡선
                    else if (m_formToolBar.ToggleType == (int)ToggleMode.Curve)
                    {
                        if (m_ptEnd != this.m_ptStart)
                        {
                            Color currentColor = FormMain.Instance.PropertiesForm.LineColor;
                            using(Pen P = new Pen(currentColor, 4))
                            {
                                P.StartCap = P.EndCap = LineCap.Round;
                                int nThick = m_formToolBar.CurrentThick;

                                Graphics g = Graphics.FromImage(FormMain.Instance.CurrentImage);
                                g.SmoothingMode = SmoothingMode.AntiAlias;
                                g.DrawLine(P, (int)fpt1.X, (int)fpt1.Y, (int)fpt2.X, (int)fpt2.Y);                               
                            }
                            panelPaint.Invalidate();
                        }                     
                        m_ptStart.X = e.X;
                        m_ptStart.Y = e.Y;
                    }
                    else if (m_formToolBar.ToggleType == (int)ToggleMode.Text)
                    {
                        panelPaint.Invalidate();
                    }
                }
            }
        }

        private Rectangle cutRect = new Rectangle();
        private void panelPaint_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                m_ptEnd = new Point(e.X, e.Y);

                PointF fpt1 = ScreenToGlobal(m_ptStart);
                PointF fpt2 = ScreenToGlobal(m_ptEnd);


                if (m_formToolBar.ToggleType == (int)ToggleMode.SelectArea)
                {
                    if (m_Rect.Width != 0 && m_Rect.Height != 0)
                    {
                        //영역이 선택되어있으면 버튼활성화
                        m_formToolBar.SelectArea();

                        if (m_TempImage.Img == null)
                        {
                            Image img = GetImageFile2(true);
                            m_TempImage.Img = img;
                            m_TempImage.orgImage = img;
                            m_TempImage.DrawPt = m_Rect.Location;
                        }

                        if (m_TempImage.Selected == false)
                        {
                            
                            PointF ptf = ScreenToGlobal(m_Rect.Location);
                            cutRect.X = (int)ptf.X;
                            cutRect.Y = (int)ptf.Y;
                            cutRect.Width = (int)((m_Rect.Width - 1) / m_Scale);
                            cutRect.Height = (int)((m_Rect.Height - 1) / m_Scale);

                                                   
                            m_TempImage.Selected = true;
                        }                        
                    }
                    else
                    {
                        m_formToolBar.SelectAreaClear();
                    }

                    FormMain.Instance.PropertiesForm.SetSelectInfoGrid(m_Rect.X, m_Rect.Y, m_Rect.Width, m_Rect.Height);
                    panelPaint.Invalidate();
                }

                else if (m_formToolBar.ToggleType == (int)ToggleMode.SelectColor)
                {
                    //선 색상 선택시.....
                    Bitmap b = GetImageFile2();
                    Color c = b.GetPixel(e.X, e.Y);

                    FormMain.Instance.PropertiesForm.SetInfoGridColor(c);
                    m_formToolBar.ToggleType = 0;
                    this.Cursor = Cursors.Default;
                    m_formToolBar.ButtonToggle();                 
                }
                else if (m_formToolBar.ToggleType == (int)ToggleMode.Translate)
                {
                    //m_Rect = new Rectangle();
                    FormMain.Instance.PropertiesForm.SetSelectInfoGrid(m_Rect.X, m_Rect.Y, m_Rect.Width, m_Rect.Height);
                    panelPaint.Invalidate();
                }             
                else if (m_formToolBar.ToggleType == (int)ToggleMode.StraightLine)
                {                   
                    Graphics g = Graphics.FromImage(FormMain.Instance.CurrentImage);
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    Color currentColor = FormMain.Instance.PropertiesForm.LineColor;
                    int nThick = m_formToolBar.CurrentThick;
                    Pen P = new Pen(currentColor, nThick);
                    g.DrawLine(P, (int)fpt1.X, (int)fpt1.Y, (int)fpt2.X, (int)fpt2.Y);
                    panelPaint.Invalidate();
                }
                else if (m_formToolBar.ToggleType == (int)ToggleMode.Text)
                {
                    int x = Math.Min(m_ptStart.X, e.Location.X);
                    int y = Math.Min(m_ptStart.Y, e.Location.Y);

                    int width = Math.Abs(e.Location.X - m_ptStart.X);
                    int height = Math.Abs(e.Location.Y - m_ptStart.Y);

                    if (width < 40)
                    {
                        width += 40;
                    }

                    if (height < 20)
                    {
                        height += 20;
                    }

                    string strFont = m_formToolBar.TextFont;
                    int nSize = m_formToolBar.TextSize;
                    FontStyle fontStyle = FontStyle.Regular;
                    if (m_bBold)
                    {
                        fontStyle = FontStyle.Bold;
                    }
                    if (m_bLean)
                    {
                        fontStyle |= FontStyle.Italic;
                    }
                    if (m_bUnderLine)
                    {
                        fontStyle |= FontStyle.Underline;
                    }

                    mTextBox.Font = new Font(strFont, nSize, fontStyle);
                    mTextBox.SetBounds(x, y, width, height);
                    mTextBox.Location = new Point(x + 1, y + 1);
                    mTextBox.Text = "";
                    mTextBox.Visible = true;
                    mTextBox.BackColor = Color.White;
                    mTextBox.ForeColor = Color.Black;
                    mTextBox.Multiline = true;
                    mTextBox.Focus();
                }
            }
            m_bDrag = false;
        }
        private Pen rectPane = new Pen(Color.WhiteSmoke, 2);
        private void panelPaint_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            try
            {
                g.Transform = mTransform;
            }
            catch (Exception ex)
            {
                UnE.Utility.UMessageBox.Show(ex.Message, "에러", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            Image img = FormMain.Instance.CurrentImage;
            if (img != null)
            {
                g.DrawImage(img, 0,0, img.Width, img.Height);
            }

            if (m_formToolBar.ToggleType == (int)ToggleMode.StraightLine)
            {
                Color currentColor = FormMain.Instance.PropertiesForm.LineColor;
                int nThick = m_formToolBar.CurrentThick;                
                using(Pen P = new Pen(currentColor, nThick))
                {
                    g.DrawLine(P, m_ptStart, m_ptEnd);
                }
            }        

            if (m_TempImage.Img != null)
            {
                // 회전을 적용
                g.DrawImage(m_TempImage.Img, m_TempImage.DrawPt);
            }

            if (m_bDrag == true && m_formToolBar.ToggleType == (int)ToggleMode.Text)
            {
                int mMinX = Math.Min(m_ptStart.X, m_ptEnd.X);
                int mMaxX = Math.Max(m_ptStart.X, m_ptEnd.X);

                int mMinY = Math.Min(m_ptStart.Y, m_ptEnd.Y);
                int mMaxY = Math.Max(m_ptStart.Y, m_ptEnd.Y);

                int nWidth = 0;
                int nHeight = 0;

                if (mMinX < 0)
                    mMinX = 0;

                if (mMinY < 0)
                    mMinY = 0;

                if (panelPaint.Width < mMaxX)
                {
                    nWidth = panelPaint.Width - mMinX;
                }
                else
                {
                    nWidth = mMaxX - mMinX;
                }

                if (panelPaint.Height < mMaxY)
                {
                    nHeight = panelPaint.Height - mMinY;
                }
                else
                {
                    nHeight = mMaxY - mMinY;
                }

                g.ResetTransform();

                Rectangle rect = new Rectangle(mMinX, mMinY, nWidth, nHeight);

                Pen pen = new Pen(Color.Gray, 1);
                pen.DashCap = System.Drawing.Drawing2D.DashCap.Round;
                pen.DashStyle = DashStyle.Dash;

                g.DrawRectangle(pen, rect);
            }

            g.ResetTransform();

            rectPane.DashCap = System.Drawing.Drawing2D.DashCap.Round;
            rectPane.DashStyle = DashStyle.Dash;
            g.DrawRectangle(rectPane, m_Rect);            
        }
 

        //현재 panel에 그려진 이미지를 Bitmap형식으로 변환하여 반환
        public Bitmap GetImageFile2()
        {
            Bitmap b = new Bitmap(this.panelPaint.Width, this.panelPaint.Height);
            this.panelPaint.DrawToBitmap(b, new Rectangle(0, 0, this.panelPaint.Width, this.panelPaint.Height));
            return b;
        }

        //저장할 이미지를 가져오는
        public Bitmap GetImageFile3()
        {                     
            PointF ptf = new PointF(0, 0);
            PointF p1 = ScreenToGlobal(ptf);
            
            ////저장하기전에 원래이미지의 scale을 매김
            //mTransform.Translate(p1.X, p1.Y);
            //mTransform.Scale((1 / m_Scale), (1 / m_Scale));
            //mTransform.Translate(-p1.X, -p1.Y);

            //SizeF scale = new SizeF(1 / m_Scale, 1 / m_Scale);
            //panelPaint.Scale(scale);
            //panelPaint.SetBounds(0, panelToolBar.Height, panelPaint.Size.Width, panelPaint.Size.Height);

            //panelPaint.Invalidate();

            //AllSelect();
            //Image img = GetImageFile2(true);
            //Bitmap b = (Bitmap)img;

            ////scale 다시 복구
            //mTransform.Translate(p1.X, p1.Y);
            //mTransform.Scale(m_Scale, m_Scale);
            //mTransform.Translate(-p1.X, -p1.Y);

            //scale = new SizeF(m_Scale, m_Scale);
            //panelPaint.Scale(scale);
            //panelPaint.SetBounds(0, panelToolBar.Height, panelPaint.Size.Width, panelPaint.Size.Height);

            //m_Rect = new Rectangle(0, 0, 0, 0);

            

            return null;
        }

        private Image CropImage(Image img, Rectangle cropArea)
        {
            try
            {
                if (img == null)
                    return null;

                if (cropArea.Width == 0 || cropArea.Height == 0)
                    return null;

                //Rectangle r = new Rectangle(0,0,panelPaint.Width,panelPaint.Height);
                //Rectangle rCropArea = cropArea;
                //rCropArea.Intersect(r);

                Bitmap bmpImage = (Bitmap)img;
                //Bitmap bmpCrop = bmpImage.Clone(rCropArea, bmpImage.PixelFormat);
                Bitmap bmpCrop = bmpImage.Clone(cropArea, bmpImage.PixelFormat);

                return bmpCrop;
            }
            catch(Exception e)
            {
                UnE.Utility.UMessageBox.Show(e.Message, "에러", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }

        }

        //복사하기, false일땐 잘라내기
        public Image GetImageFile2(bool bType)
        {
            if (m_Rect.Width == 0 || m_Rect.Height == 0)
                return null;

            //복사할때 테두리 잔상 안남게하기위해 임시사각형을 만든다.
            Rectangle temp = new Rectangle();
            temp.X = m_Rect.X;
            temp.Y = m_Rect.Y;
            temp.Width = m_Rect.Width;
            temp.Height = m_Rect.Height;

            m_Rect = new Rectangle(0, 0, 0, 0);

            Bitmap bitmap = new Bitmap(panelPaint.Size.Width, panelPaint.Size.Height, PixelFormat.Format32bppPArgb);
            panelPaint.DrawToBitmap(bitmap, new Rectangle(0, 0, panelPaint.Size.Width, panelPaint.Size.Height));

            Image image = bitmap;

            Image CapImage = CropImage(image, temp);

            image.Dispose();

            m_Rect = temp;
            if (bType == false)
            {
                //잘라내기
                DeleteIamge();
                
            }

            //복사하기
            //m_Rect = new Rectangle();
            return CapImage;
        }

        //붙여넣기
        public void ProcessPaste(Image image)
        {
            m_TempImage.Img = image;
            m_TempImage.DrawPt = new Point(0, 0);
            m_TempImage.Selected = true;

            //선택영역의 위치가 바뀜
            m_Rect.X = 0;
            m_Rect.Y = 0;
            m_Rect.Width = image.Width;
            m_Rect.Height = image.Height;

            //붙여넣었으면 이미지를 이동하기쉽게 이동모드로 변경
            if (m_formToolBar.ToggleType == (int)ToggleMode.SelectArea)
                m_formToolBar.ButtonChecked(ID.TOOLBAR_TRANSLATE);

            cutRect = new Rectangle();

            panelPaint.Invalidate();
        }

        //삭제
        public void DeleteIamge()
        {
            if (cutRect.Width == 0 || cutRect.Height == 0)
                return;          
            Graphics g = Graphics.FromImage(FormMain.Instance.CurrentImage);
            g.SmoothingMode = SmoothingMode.HighQuality;
            using (SolidBrush brush = new SolidBrush(Color.White))
            {
                g.FillRectangle(brush, cutRect);
            }

            cutRect = new Rectangle();
            m_TempImage.Img = null;
            m_TempImage.Selected = false;

            panelPaint.Invalidate();
        }

        //자르기(잘라내기아님)
        public void ImageCut(int width, int height)
        {
            if (m_Rect.Width == 0 || m_Rect.Height == 0)
                return;

            //m_ClipBoardImages.RemoveAt(m_ClipBoardImages.Count - 1);
            panelPaint.Width = width;
            panelPaint.Height = height;

            //mTransform.Reset();

            //arrShape.Clear();
            //m_ClipBoardImages.Clear();
            m_img = null;
            m_TempImage.Img = null;
            m_TempImage.Selected = false;

            m_Rect = new Rectangle(0, 0, 0, 0);

            m_formToolBar.ButtonChecked(ID.TOOLBAR_SELECT_AREA);
            m_formToolBar.SelectAreaClear();

            panelPaint.Invalidate();
        }

        //전체선택
        public void AllSelect()
        {
            m_Rect = new Rectangle(0, 0, panelPaint.Width, panelPaint.Height);
            FormMain.Instance.PropertiesForm.SetSelectInfoGrid(m_Rect.X, m_Rect.Y, m_Rect.Width, m_Rect.Height);

            Image img = GetImageFile2(true);
            m_TempImage.Img = img;
            m_TempImage.DrawPt = m_Rect.Location;

            //영역이 선택되어있으면 버튼활성화
                        m_formToolBar.SelectArea();

            panelPaint.Invalidate();
        }

        private void panelMain_MouseUp(object sender, MouseEventArgs e)
        {
            if (m_formToolBar.ToggleType == (int)ToggleMode.SelectColor)
            {
                m_formToolBar.ToggleType = (int)ToggleMode.SelectArea;
                this.Cursor = Cursors.Default;
                m_formToolBar.ButtonToggle();
            }

        }

        private void FormContent_Load(object sender, EventArgs e)
        {
            m_ptStart = new Point(0, 0);
            m_ptEnd = new Point(0, 0);
            m_PenTemp = new Pen(Color.Black, 1);
            m_Pen = new Pen(Color.Black, 1);
            m_bDrag = false;

            panel1.SetBounds(0, panelToolBar.Height, panelMain.Width, panelMain.Height - panelToolBar.Height);
        }

        //선택영역 확대
        public void SelectZoomIn()
        {
            float fScale = 1.1f;
            PointF ptf = new PointF(0, 0);
            PointF p1 = ScreenToGlobal(ptf);

            if(m_TempImage.Img == null)
            {
                Image img = GetImageFile2(true);
                m_TempImage.Img = img;
                m_TempImage.DrawPt = m_Rect.Location;
            }

            Image img2 = m_TempImage.orgImage;
            Image img3 = m_TempImage.Img;
            Bitmap returnBitmap = new Bitmap(img2, new Size((int)(img3.Width * fScale), (int)(img3.Height * fScale)));

            Point pt = new Point(m_Rect.X, m_Rect.Y);
            m_TempImage.Img = returnBitmap;
            m_TempImage.DrawPt = m_Rect.Location;

            m_Rect = new Rectangle(m_Rect.X, m_Rect.Y, (int)(m_Rect.Width * fScale), (int)(m_Rect.Height * fScale));

            Graphics g = Graphics.FromImage(FormMain.Instance.CurrentImage);
            g.SmoothingMode = SmoothingMode.HighQuality;
            using (SolidBrush brush = new SolidBrush(Color.White))
            {
                g.FillRectangle(brush, m_Rect);
            }


            panelPaint.Invalidate();
        }

        //선택영역 축소
        public void SelectZoomOut()
        {
            float fScale = 0.9f;
            PointF ptf = new PointF(0, 0);
            PointF p1 = ScreenToGlobal(ptf);

            if (m_TempImage.Img == null)
            {
                Image img = GetImageFile2(true);
                m_TempImage.Img = img;
                m_TempImage.DrawPt = m_Rect.Location;
            }

            Image img2 = m_TempImage.orgImage;
            Image img3 = m_TempImage.Img;
            Bitmap returnBitmap = new Bitmap(img2, new Size((int)(img3.Width * fScale), (int)(img3.Height * fScale)));
         
            Point pt = new Point(m_Rect.X, m_Rect.Y);
            m_TempImage.Img = returnBitmap;
            m_TempImage.DrawPt = m_Rect.Location;


            m_Rect = new Rectangle(m_Rect.X, m_Rect.Y, (int)(m_Rect.Width * fScale), (int)(m_Rect.Height * fScale));

            Graphics g = Graphics.FromImage(FormMain.Instance.CurrentImage);
            g.SmoothingMode = SmoothingMode.HighQuality;
            using (SolidBrush brush = new SolidBrush(Color.White))
            {
                g.FillRectangle(brush, m_Rect);
            }

            panelPaint.Invalidate();
        }

        //전체 확대
        private float m_Scale = 1.0f;
        public void ZoomIn()
        {
            if (m_Scale > Math.Pow(2.0f,1))
                return;
            //float x = mTransform.OffsetX;
            //float y = mTransform.OffsetY;

            //float width = panelPaint.Width;
            //float height = panelPaint.Height;

            //float tx = width - x;
            //float ty = height - y;

            //mTransform.Scale(tx, ty);
            PointF ptf = new PointF(0, 0);
            PointF p1 = ScreenToGlobal(ptf);

            float fScale = 2.0f;
            mTransform.Translate(p1.X, p1.Y);
            mTransform.Scale(fScale, fScale);
            //mTransform.Scale(1.0f, 1.0f);
            mTransform.Translate(-p1.X, -p1.Y);

            m_Scale *= fScale;

            SizeF scale = new SizeF(fScale, fScale);
            panelPaint.Scale(scale);
            panelPaint.SetBounds(0, 0, panelPaint.Size.Width, panelPaint.Size.Height);

            m_Rect = new Rectangle();

            //정보 갱신
            FormMain.Instance.PropertiesForm.SetSelectInfoGrid(m_Rect.X, m_Rect.Y, m_Rect.Width, m_Rect.Height);



            panelPaint.Invalidate();
            //int nWidth = panelPaint.Size.Width * fScale;
            //int nHeight = panelPaint.Size.Height * fScale;
            //panelPaint.Size = new Size(nWidth, nHeight);

        }


        //전체 축소
        public void ZoomOut()
        {
            if (m_Scale < Math.Pow(0.5f, 1))
                return;

            float fScale = 0.5f;
            PointF ptf = new PointF(0, 0);
            PointF p1 = ScreenToGlobal(ptf);

            mTransform.Translate(p1.X, p1.Y);
            mTransform.Scale(fScale, fScale);
            mTransform.Translate(-p1.X, -p1.Y);

            SizeF scale = new SizeF(fScale, fScale);

            panelPaint.Scale(scale);
            panelPaint.SetBounds(0, 0, panelPaint.Size.Width, panelPaint.Size.Height);

            m_Scale *= fScale;

            m_Rect = new Rectangle();

            //정보 갱신
            FormMain.Instance.PropertiesForm.SetSelectInfoGrid(m_Rect.X, m_Rect.Y, m_Rect.Width, m_Rect.Height);



            panelPaint.Invalidate();
            //int nWidth = panelPaint.Size.Width / 2;
            //int nHeight = panelPaint.Size.Height / 2;

            //panelPaint.Size = new Size(nWidth, nHeight);

        }

        public void SelectRotation(int nType)
        {
            //Image img = GetImageFile2(true);
            PointF ptf = new PointF(0, 0);
            PointF p1 = ScreenToGlobal(ptf);
            if(m_TempImage.Img == null)
            {
                Image img = GetImageFile2(true);
                m_TempImage.Img = img;
                m_TempImage.DrawPt = m_Rect.Location;
            }

            if (nType == 0)
            {
                Image img2 = m_TempImage.Img;

                //Bitmap returnBitmap = new Bitmap(img2.Width, img2.Height);
                Graphics g = Graphics.FromImage(img2);
           
                //g.TranslateTransform(img2.Height * 0.5f, img2.Width * 0.5f);
                g.TranslateTransform(m_Rect.Width * 0.5f, m_Rect.Height * 0.5f);
                g.RotateTransform(90);
                g.TranslateTransform(-m_Rect.Width * 0.5f, -m_Rect.Height * 0.5f);
                //g.TranslateTransform(-img2.Height * 0.5f, -img2.Width * 0.5f);

                //g = Graphics.FromImage(m_Rect);

                g.DrawImage(img2, new Point(0, 0));



                //returnBitmap.Save("C:\\Text.png");

                Point pt = new Point(m_Rect.X, m_Rect.Y);

               // 

                //EditImage editimg = new EditImage(returnBitmap, pt);
                //arrShape.Add(editimg);

                //m_TempImage.Img = returnBitmap;
                m_TempImage.DrawPt = m_Rect.Location;
            }
            else if(nType == 3)
            {
                Image img2 = m_TempImage.Img;
                Graphics g = Graphics.FromImage(img2);

                g.TranslateTransform(m_Rect.Width * 0.5f, m_Rect.Height * 0.5f);
                g.ScaleTransform(1,-1);
                g.TranslateTransform(-m_Rect.Width * 0.5f, -m_Rect.Height * 0.5f);

                g.DrawImage(img2, new Point(0, 0));

                //mTransform.Scale(1, -1);
            }

            panelPaint.Invalidate();
        }

        private int nRotationCount = 0;
        public void Rotation(int nType)
        {
            if (nType == 0)
            {
                PointF ptf1 = new PointF(panelPaint.Width * 0.5f, panelPaint.Height * 0.5f);
                PointF p1 = ScreenToGlobal(ptf1);

                int tmp = panelPaint.Width;
                panelPaint.Width = panelPaint.Height;
                panelPaint.Height = tmp;

                PointF ptf2 = new PointF(panelPaint.Width * 0.5f, panelPaint.Height * 0.5f);
                PointF p2 = ScreenToGlobal(ptf2);

                float tx = p2.X - p1.X;
                float ty = p2.Y - p1.Y;
                mTransform.Translate(tx, ty);

                mTransform.Translate(p1.X, p1.Y);
                mTransform.Rotate(90);

                mTransform.Translate(-p1.X, -p1.Y);

                nRotationCount++;

            }
            else if (nType == 1)
            {
                PointF ptf1 = new PointF(panelPaint.Width * 0.5f, panelPaint.Height * 0.5f);
                PointF p1 = ScreenToGlobal(ptf1);

                int tmp = panelPaint.Width;
                panelPaint.Width = panelPaint.Height;
                panelPaint.Height = tmp;

                PointF ptf2 = new PointF(panelPaint.Width * 0.5f, panelPaint.Height * 0.5f);
                PointF p2 = ScreenToGlobal(ptf2);

                float tx = p2.X - p1.X;
                float ty = p2.Y - p1.Y;
                mTransform.Translate(tx, ty);

                mTransform.Translate(p1.X, p1.Y);
                mTransform.Rotate(-90);

                mTransform.Translate(-p1.X, -p1.Y);

                nRotationCount--;
            }
            else if (nType == 2)
            {
                PointF ptf = new PointF(panelPaint.Width * 0.5f, panelPaint.Height * 0.5f);
                PointF p1 = ScreenToGlobal(ptf);

                mTransform.Translate(p1.X, p1.Y);
                mTransform.Rotate(180);
                mTransform.Translate(-p1.X, -p1.Y);

            }
            else if (nType == 3)
            {
                PointF ptf = new PointF(panelPaint.Width * 0.5f, panelPaint.Height * 0.5f);
                PointF p1 = ScreenToGlobal(ptf);
                if (nRotationCount == 0 || nRotationCount % 2 == 0)
                {
                    mTransform.Translate(p1.X, p1.Y);
                    mTransform.Scale(1, -1);
                    mTransform.Translate(-p1.X, -p1.Y);
                }
                else
                {
                    mTransform.Translate(p1.X, p1.Y);
                    mTransform.Scale(-1, 1);
                    mTransform.Translate(-p1.X, -p1.Y);
                }
            }
            else if (nType == 4)
            {
                PointF ptf = new PointF(panelPaint.Width * 0.5f, panelPaint.Height * 0.5f);
                PointF p1 = ScreenToGlobal(ptf);
                if (nRotationCount == 0 || nRotationCount % 2 == 0)
                {
                    mTransform.Translate(p1.X, p1.Y);
                    mTransform.Scale(-1, 1);
                    mTransform.Translate(-p1.X, -p1.Y);
                }
                else
                {
                    mTransform.Translate(p1.X, p1.Y);
                    mTransform.Scale(1, -1);
                    mTransform.Translate(-p1.X, -p1.Y);
                }
            }

            panelPaint.Invalidate();
        }


        public void CancelTextInput()
        {
            string szText = mTextBox.Text;
            szText = szText.Trim();
            if (szText != "")
            {
                int x = Math.Min(m_ptStart.X, m_ptEnd.X);
                int y = Math.Min(m_ptStart.Y, m_ptEnd.Y);
                Point ptOrigin = new Point(x, y);

                PointF fpt = ScreenToGlobal(ptOrigin);

                Color currentColor = FormMain.Instance.PropertiesForm.LineColor;
                Pen P = new Pen(currentColor, 4);
                string strTextFont = m_formToolBar.TextFont;
                int nTextSize = m_formToolBar.TextSize;

                FontStyle fontStyle = FontStyle.Regular;
                if (m_bBold)
                {
                    fontStyle = FontStyle.Bold;
                }
                if (m_bLean)
                {
                    fontStyle |= FontStyle.Italic;
                }
                if (m_bUnderLine)
                {
                    fontStyle |= FontStyle.Underline;
                }

                Graphics g = Graphics.FromImage(FormMain.Instance.CurrentImage);
                g.SmoothingMode = SmoothingMode.HighQuality;
                using (SolidBrush brush = new SolidBrush(currentColor))
                {
                    using(Font f = new System.Drawing.Font(strTextFont, nTextSize, fontStyle))
                    {
                        g.DrawString(szText, f, brush, fpt.X, fpt.Y);
                    }
                }

                mTextBox.Text = "";
                panelPaint.Invalidate();
            }

            mTextBox.Visible = false;

            Focus();
        }

        private Point GlabalToScreen(PointF fpt)
        {

            Matrix mTemp = mTransform.Clone();


            PointF[] myArray =
            {
                fpt
            };
            mTemp.TransformPoints(myArray);

            int x = (int)myArray[0].X;
            int y = (int)myArray[0].Y;

            return new Point(x, y);
        }

        private PointF ScreenToGlobal(Point pt)
        {
            Matrix mTemp = mTransform.Clone();

            try
            {
                mTemp.Invert();

            }
            catch (Exception)
            {

            }

            PointF ff = new PointF(pt.X, pt.Y);

            PointF[] myArray =
            {
                ff
            };
            mTemp.TransformPoints(myArray);

            float x = myArray[0].X;
            float y = myArray[0].Y;

            return new PointF(x, y);
        }
        private PointF ScreenToGlobal(PointF fpt)
        {
            Matrix mTemp = mTransform.Clone();
            mTemp.Invert();


            PointF[] myArray =
            {
                fpt
            };
            mTemp.TransformPoints(myArray);

            float x = myArray[0].X;
            float y = myArray[0].Y;

            return new PointF(x, y);
        }

        private void panelToolBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (m_formToolBar.ToggleType == (int)ToggleMode.SelectColor)
            {
                m_formToolBar.ToggleType = (int)ToggleMode.SelectArea;
                this.Cursor = Cursors.Default;
                m_formToolBar.ButtonToggle();
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (!base.ProcessCmdKey(ref msg, keyData))
            {
                Keys key = keyData & ~(Keys.Shift | Keys.Control);

                switch (key)
                {
                    case Keys.Z:
                        //if((keyData & Keys.Control) != 0)
                        //{
                        //    if (arrShape.Count > 0)
                        //    {
                        //        if (arrShape[arrShape.Count - 1].GetType() == typeof(ShapeCurve))
                        //        {
                        //            for (int i = arrShape.Count - 1; i >= 0; i--)
                        //            {
                        //                if (arrShape[i].GetType() == typeof(ShapeCurve))
                        //                    arrShape.RemoveAt(i);
                        //                else
                        //                    break;
                        //            }
                        //        }
                        //        else
                        //        {
                        //            arrShape.RemoveAt(arrShape.Count - 1);
                        //        }
                        //        panelPaint.Invalidate();
                        //        return true;
                        //    }
                        //}
                        break;
                          
                
                }

            }
            return false;
        }
    }
}
