using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SDMS.Help;
using UnE.GUI;

namespace SDMS
{
    public partial class FormSaveHomeView : PopupFormBase
    {
        public ImageButton BtnMainHome
        {
            get { return btnHome; }
            set { btnHome = value; }
        }

        public ImageButton Btn14Home
        {
            get { return btn14Home; }
            set { btn14Home = value; }
        }

        public ImageButton Btn56Home
        {
            get { return btn56Home; }
            set { btn56Home = value; }
        }

        public ImageButton BtnCoalHome
        {
            get { return btnCoalHome; }
            set { btnCoalHome = value; }
        }

        private ManualManager m_manualManager = null;

        public FormSaveHomeView()
        {
            InitializeComponent();

            InitCtrlSize(this);

            m_manualManager = new ManualManager(this);
            SetManualID();
        }

        private void FormSaveHomeView_Load(object sender, EventArgs e)
        {
            SetChildCtrlResize(this, this.Width, this.Height);
        }

        public void SetButtonVisible()
        {
            ImageButton[] buttons = new ImageButton[] { btn14Home, btn56Home, btnCoalHome };
            int pos = 0;

            foreach (ImageButton btn in buttons)
            {
                if (btn.Text == "사용안함")
                {
                    btn.Visible = false;
                    pos += btn.Size.Height;
                }
                else
                {
                    btn.Location = new Point(btn.Location.X, btn.Location.Y - pos);
                    btn.Visible = true;
                }
            }

            this.Size = new Size(this.Size.Width, this.Size.Height - pos);
        }

        public void SetFont()
        {
            float sizePer = 1.0f;
            if (FormMain.Instance.Resolution == Resolution.FullHD)
                sizePer = 0.5f;
            else if (FormMain.Instance.Resolution == Resolution.Other)
                sizePer = 0.75f;

            Font font = new System.Drawing.Font(Program.prgFont, 21.75F * sizePer, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            btnHome.TextFont = font;
            btn14Home.TextFont = font;
            btn56Home.TextFont = font;
            btnCoalHome.TextFont = font;
        }

        private void SetManualID()
        {
            m_manualManager.Handle = this.Handle;

            m_manualManager.Clear();

            m_manualManager.SetID(btnHome, "Layer_화면설정");
            m_manualManager.SetID(btn14Home, "Layer_화면설정");
            m_manualManager.SetID(btn56Home, "Layer_화면설정");
            m_manualManager.SetID(btnCoalHome, "Layer_화면설정");
            m_manualManager.ProcessEvent();
        }
    }
}
