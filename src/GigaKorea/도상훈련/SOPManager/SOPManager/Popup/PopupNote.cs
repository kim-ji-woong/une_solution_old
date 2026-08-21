using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SOPManager
{
	
    public partial class PopupNote : Form
    {

		protected string m_szContent = "";
		public string Content
		{
			get
			{
				//szContent = textBox.Text;
				return m_szContent; 
			}
			set 
			{
				m_szContent = value;
				//textBox.Text = m_szContent;
			}
		}

        private PropertiesAnnotation m_propertiesAnnotation = null;
        public PropertiesAnnotation PropertiesAnnotation
        {
            get { return m_propertiesAnnotation; }
            set
            {
                m_propertiesEndPoint = null;
                m_propertiesAnnotation = value;
                if (m_propertiesAnnotation != null)
                {
                    m_szContent = m_propertiesAnnotation.Text;
                    InitText();
                }
                    
            }
        }

        private PropertiesEndPoint m_propertiesEndPoint = null;
        public PropertiesEndPoint PropertiesEndPoint
        {
            get { return m_propertiesEndPoint; }
            set
            {
                m_propertiesAnnotation = null;
                m_propertiesEndPoint = value;
                if (m_propertiesEndPoint != null)
                {
                    m_szContent = m_propertiesEndPoint.Text;
                    InitText();
                }
            }
        }

		protected bool m_ShowWarning = false;

		protected bool m_bLeftMouseDown = false;
		protected Point m_ptMove = new Point();

        public PopupNote()
        {
            InitializeComponent();
			btnStandard.Visible = false;

            UpdateControlSize();
        }

        public void UpdateControlSize()
        {
            Double[] dWindowRate = FormMain.Instance.GetCurWindowRate();
            double WindowRateWidth = dWindowRate[0];
            double WindowRateHeight = dWindowRate[1];

            this.Size = new System.Drawing.Size((int)(this.Size.Width * WindowRateWidth), (int)(this.Size.Height * WindowRateHeight));

            foreach (Control ctl in this.Controls)
            {
                HaveControl(ctl, WindowRateWidth, WindowRateHeight);
            }
        }

        private void HaveControl(Control pctl, double WindowRateWidth, double WindowRateHeight)
        {
            foreach (Control ctl in pctl.Controls)
            {
                if (ctl.Controls.Count > 0)
                    HaveControl(ctl, WindowRateWidth, WindowRateHeight);

                FormMain.Instance.UpdateWindowRate(ctl, WindowRateWidth, WindowRateHeight);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
		
        public void InitText()
        {				
            this.Text = "내용 작성";
           // labelNote.Text = "내용";               
            btnStandard.Visible = false;
			textBox.Text = m_szContent;

			if (!m_ShowWarning)
            {
				panelTop.Visible = false;
                labelWarning.Visible = labelWarning2.Visible = labelWarning3.Visible = false;

                int nNewHeight = 200;               
                this.Size = new Size(this.Size.Width, nNewHeight);

            }
        }

		protected void btnOK_Click(object sender, EventArgs e)
        {
			m_szContent = textBox.Text;

            SaveData();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

		protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

		protected void PopupNote_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = this.PointToScreen(new Point(e.X, e.Y));
            }
        }

		protected void PopupNote_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_bLeftMouseDown == true)
                {
                    Point pt = this.PointToScreen(new Point(e.X, e.Y));
                    int dx = pt.X - m_ptMove.X;
                    int dy = pt.Y - m_ptMove.Y;
                    if (!(dx == 0 && dy == 0))
                    {
                        Point ptCur = this.Location;
                        this.Location = new Point(ptCur.X + dx, ptCur.Y + dy);
                        m_ptMove.X += dx;
                        m_ptMove.Y += dy;
                    }
                }
            }
        }

		protected void PopupNote_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                m_bLeftMouseDown = false;
        }

        public void SaveData()
        {
            if( m_propertiesAnnotation != null)
            {
                m_szContent = textBox.Text;
                m_propertiesAnnotation.Text = m_szContent;
            }
            if( m_propertiesEndPoint != null)
            {
                m_szContent = textBox.Text;
                m_propertiesEndPoint.Text = m_szContent;
            }
        }

        private void btnShowSpecialMessage_Click(object sender, EventArgs e)
        {
            FormMain.Instance.ShowSpecialMessage();
        }
    }


	public class PopupNoteEx : PopupNote
	{
		public PopupNoteEx()
			: base()
		{
			m_ShowWarning = true;
		}
	}
}
