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
    public partial class FormHomeView : PopupFormBase
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

        public FormHomeView()
        {
            InitializeComponent();

            InitCtrlSize(this);

            m_manualManager = new ManualManager(this);
            SetManualID();
        }

        private void FormHomeView_Load(object sender, EventArgs e)
        {
            SetChildCtrlResize(this, 225, 219);
        }

        public void SetFont()
        {
            float sizePer = 1.0f;
            if (FormMain.Instance.Resolution == Resolution.FullHD)
                sizePer = 0.5f;
            else if (FormMain.Instance.Resolution == Resolution.Other)
                sizePer = 0.75f;

            Font font = new System.Drawing.Font(Program.prgFont, 21.75F * sizePer, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            btnHome.TextFont = font;
            btn14Home.TextFont = font;
            btn56Home.TextFont = font;
            btnCoalHome.TextFont = font;
        }

        private void SetManualID()
        {
            m_manualManager.Handle = this.Handle;

            m_manualManager.Clear();

            m_manualManager.SetID(btnHome, "Toolbar_HomeButton");
            m_manualManager.SetID(btn14Home, "Toolbar_HomeButton");
            m_manualManager.SetID(btn56Home, "Toolbar_HomeButton");
            m_manualManager.SetID(btnCoalHome, "Toolbar_HomeButton");
            m_manualManager.ProcessEvent();
        }
    }
}
