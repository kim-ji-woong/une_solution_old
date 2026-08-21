using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.GUI;

namespace SDMS
{
    public partial class PopupFormBase : Form
    {
        #region Form 이동
        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();
        private bool m_isClicked = false;
        private Point m_ptOrigin = new Point();
        private bool m_useFrmMove = true;
        public bool UseFrmMove
        {
            get { return m_useFrmMove; }
            set { m_useFrmMove = value; }
        }
        #endregion

        public PopupFormBase()
        {
            InitializeComponent(); 
        }

        #region 폼 이동
        public void PopupFormBase_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left && m_useFrmMove)
            {
                m_bLeftMouseDown = true;
                m_ptMove = Control.MousePosition;
                m_ptOrigin = this.Location;
            }

            m_isClicked = true;
        }

        public void PopupFormBase_MouseMove(object sender, MouseEventArgs e)
        {
            if (!m_useFrmMove)
                return;

            if (!m_isClicked)
                return;

            if (!m_bLeftMouseDown)
                return;

            Point ptScreen = Control.MousePosition;

            int dx = ptScreen.X - m_ptMove.X;
            int dy = ptScreen.Y - m_ptMove.Y;

            if (dx == 0 && dy == 0)
                return;

            Point ptCur = this.Location;
            this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
            m_ptMove.X += dx;
            m_ptMove.Y += dy;
        }

        public void PopupFormBase_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;

            m_isClicked = false;
        } 
        #endregion

        private Dictionary<Control, Point> m_CtrlLoction = new Dictionary<Control, Point>();
        private Dictionary<Control, Size> m_CtrlSize = new Dictionary<Control, Size>();
        private Dictionary<Control, Font> m_CtrlFontSize = new Dictionary<Control, Font>();

        protected void InitCtrlSize(Control ctrl)
        {
            foreach (Control childCtrl in ctrl.Controls)
            {
                if (childCtrl is ImageButton)
                {
                    ImageButton imgBtn = childCtrl as ImageButton;
                    if (imgBtn.ImageNormal == null)
                        m_CtrlSize.Add(childCtrl, imgBtn.Size);  
                    else
                        m_CtrlSize.Add(childCtrl, new Size(imgBtn.ImageNormal.Size.Width / 2, imgBtn.ImageNormal.Size.Height / 2));  
                }
                else if (childCtrl is Panel)
                {
                    if (childCtrl.BackgroundImage == null)
                        m_CtrlSize.Add(childCtrl, childCtrl.Size);
                    else
                        m_CtrlSize.Add(childCtrl, new Size(childCtrl.BackgroundImage.Size.Width / 2, childCtrl.BackgroundImage.Size.Height / 2));
                }
                else if (childCtrl is Label || childCtrl is CheckBox)
                {
                    m_CtrlFontSize.Add(childCtrl, childCtrl.Font);
                }
                else if (childCtrl is PictureBox)
                {
                    PictureBox pic = childCtrl as PictureBox;
                    if (pic.Image == null)
                        m_CtrlSize.Add(childCtrl, pic.Size);
                    else
                        m_CtrlSize.Add(childCtrl, new Size(pic.Image.Size.Width / 2, pic.Image.Size.Height / 2));
                }
                else if (childCtrl is TextBox || childCtrl is RichTextBox || childCtrl is Button || childCtrl is RadioButton || childCtrl is ImageComboBox || childCtrl is TreeView || childCtrl is DateTimePicker)
                {
                    m_CtrlSize.Add(childCtrl, childCtrl.Size);
                    m_CtrlFontSize.Add(childCtrl, childCtrl.Font);
                }
                else if (childCtrl is DataGridView)
                {
                    m_CtrlSize.Add(childCtrl, childCtrl.Size);
                } 

                m_CtrlLoction.Add(childCtrl, childCtrl.Location);
                InitCtrlSize(childCtrl);
            }
        }

        public void SetChildCtrlResize(Control ctrl, int width, int height)
        { 
            float sizePer = 1f; 
            if (FormMain.Instance.Resolution == Resolution.Other) 
                sizePer = 1.5f; 
            else if (FormMain.Instance.Resolution == Resolution.FourK)
                sizePer = 2.0f;  

            ctrl.Size = new Size(Convert.ToInt32(width * sizePer), Convert.ToInt32(height * sizePer));

            if (m_CtrlLoction.ContainsKey(ctrl)) 
                ctrl.Location = new Point((int)(m_CtrlLoction[ctrl].X * sizePer), (int)(m_CtrlLoction[ctrl].Y * sizePer)); 

            foreach (Control childCtrl in ctrl.Controls)
            {
                int width2 = -1;
                int height2 = -1;

                if (m_CtrlSize.ContainsKey(childCtrl))
                {
                    width2 = m_CtrlSize[childCtrl].Width;
                    height2 = m_CtrlSize[childCtrl].Height;
                }

                if (m_CtrlFontSize.ContainsKey(childCtrl))
                {
                    Font font = m_CtrlFontSize[childCtrl];
                    FontFamily fontFamily = font.FontFamily;
                    //if (FormMain.Instance.Resolution == Resolution.FullHD)
                    //{
                        //if (FormMain.Instance.UseNanumFont)
                        //    fontFamily = new FontFamily(Program.prgFont);
                        //else
                            fontFamily = new FontFamily(Program.prgFont);
                    //}
                    float fontSize = font.Size;
                    FontStyle fontStyle = font.Style;
                    //if (FormMain.Instance.Resolution == Resolution.FullHD)
                    //    fontStyle = FontStyle.Regular;

                    childCtrl.Font = new Font(fontFamily, fontSize * sizePer, fontStyle, System.Drawing.GraphicsUnit.Point, ((byte)(129)));

                    // 예외 : Label 일 경우 입력 Text마다 크기가 달라진다
                    if (width2 == -1 && height2 == -1)
                    {
                        width2 = childCtrl.Width;
                        height2 = childCtrl.Height;
                    }
                }

                if (width2 < 0 || height2 < 0)
                    continue;

                SetChildCtrlResize(childCtrl, width2, height2);
            }  
        }
    }
}
