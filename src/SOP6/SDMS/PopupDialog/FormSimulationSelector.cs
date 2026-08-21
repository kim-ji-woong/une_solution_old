using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SDMS
{
    public partial class FormSimulationSelector : Form
    {
        public EventHandler ButtonClickEvent;

        private List<Button> m_liButton = new List<Button>();


        public FormSimulationSelector()
        {
            InitializeComponent();
        }


        public void ClearButtons()
        {
            Button[] btns = m_liButton.ToArray();

            foreach (Button btn in btns)
            {
                btn.Dispose();
            }

            m_liButton.Clear();
        }

        public Button AddButton(string strButtonName, string strRunProcessName)
        {
            int nDefaultLocatiopX = 8;
            int nPrevButtonWidth = 0;

            if (m_liButton.Count != 0)
                nPrevButtonWidth = (m_liButton.Count  * 5);

            foreach (Button btn in m_liButton)
            {
                nPrevButtonWidth += btn.Size.Width;
            }

            int nStringWidth = -1;
            Font font = new Font("굴림", 9.75F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(129)));

            using (Graphics g = this.CreateGraphics())
            {
                nStringWidth = Convert.ToInt32(g.MeasureString(strButtonName, font).Width);
            }

            System.Windows.Forms.Button btnNew = new Button()
            {
                Font = font,
                Location = new Point(nDefaultLocatiopX + nPrevButtonWidth, 7),
                Name = strButtonName,
                Size = new System.Drawing.Size(nStringWidth + 25, 32),
                TabIndex = 3,
                Text = strButtonName,
                UseVisualStyleBackColor = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Tag = strRunProcessName
            };

            this.Controls.Add(btnNew);
            m_liButton.Add(btnNew);

            this.Size = new Size((nDefaultLocatiopX * 2) + nPrevButtonWidth + btnNew.Size.Width, this.Size.Height);

            btnNew.Click += btnNew_Click;

            return btnNew;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (ButtonClickEvent != null)
            {
                ButtonClickEvent(sender, e);
            }
        }

    }
}
