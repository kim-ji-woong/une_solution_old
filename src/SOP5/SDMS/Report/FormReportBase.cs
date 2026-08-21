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

namespace SDMS.Report
{
    public partial class FormReportBase : Form
    {
        public FormReportBase()
        {
            InitializeComponent();
        }

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
                    if (imgBtn.ImageNormal != null)
                        m_CtrlSize.Add(childCtrl, new System.Drawing.Size(imgBtn.ImageNormal.Size.Width / 2, imgBtn.ImageNormal.Size.Height / 2));
                        //m_CtrlSize.Add(childCtrl, imgBtn.ImageNormal.Size);                    
                }
                else if (childCtrl is Panel)
                { 
                    if (childCtrl.BackgroundImage == null) 
                        m_CtrlSize.Add(childCtrl, childCtrl.Size); 
                    else
                        m_CtrlSize.Add(childCtrl, childCtrl.BackgroundImage.Size);
                }
                else if (childCtrl is Label || childCtrl is CheckBox)
                {
                    m_CtrlFontSize.Add(childCtrl, childCtrl.Font);
                }
                else if (childCtrl is PictureBox)
                {
                    PictureBox pic = childCtrl as PictureBox;
                    if (pic.Image == null)
                        m_CtrlSize.Add(childCtrl, childCtrl.Size);
                    else
                        m_CtrlSize.Add(childCtrl, new System.Drawing.Size(pic.Image.Size.Width, pic.Image.Size.Height));
                }
                else if (childCtrl is TextBox || childCtrl is RichTextBox || childCtrl is Button || childCtrl is RadioButton || childCtrl is ImageComboBox || childCtrl is TreeView || childCtrl is ComboBox)
                {
                    m_CtrlSize.Add(childCtrl, childCtrl.Size);
                    m_CtrlFontSize.Add(childCtrl, childCtrl.Font);
                }
                else if (childCtrl is DataGridView || childCtrl is ChartDirector.WinChartViewer)
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

            if (ctrl != this)
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
                    if (FormMain.Instance.Resolution == Resolution.FullHD)
                    {
                        if (FormMain.Instance.UseNanumFont)
                            fontFamily = new FontFamily(Program.prgFont);
                        else
                            fontFamily = new FontFamily("굴림");
                    }
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
