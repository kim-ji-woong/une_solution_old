using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SDMS
{
    public partial class FormViewIndoor : Form
    {
        private BaseViewEx m_view = null;

        public FormViewIndoor()
        {
            InitializeComponent();
            MouseWheel += new MouseEventHandler(OnMouseWheel);
        }

        public void AttachView(BaseViewEx view)
        {
            this.Controls.Add(view);
            view.Dock = DockStyle.Fill;
            m_view = view;
            view.Show();
        }

        public BaseViewEx DetachView()
        {
            this.Controls.Remove(m_view);

            BaseViewEx view = m_view;
            m_view = null;

            return view;
        }

        public void OnMouseWheel(object sender, MouseEventArgs e)
        {
            if (m_view != null)
            {
                m_view.OnMouseWheel(e.X, e.Y, e.Delta);
            }
        }
    }
}
