using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FireManagement
{
    public partial class FormEquipDeletePopup : Form
    {
        private DXFViewer.Shape m_shapeDel = null;
        private UnE.Geometry.Vertex2D m_vPos = null;

        public FormEquipDeletePopup(DXFViewer.Shape shape, UnE.Geometry.Vertex2D vPos)
        {
            InitializeComponent();
            m_shapeDel = shape;
            m_vPos = vPos;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            FormMain2 frmMain = FormMain2.Instance;
            //frmMain.DeleteEquipment(frmMain.ViewControl.FrmEquipHistory.FindEquipment(m_shapeDel));
            frmMain.DeleteEquipment(frmMain.ViewControl.LeftBar.FindEquipment(m_shapeDel));
            frmMain.Refresh();
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            FormMain2.Instance.ViewControl.LeftBar.SelectedEquipment.Move(m_vPos);
            FormMain2.Instance.Refresh();

            FormMain2.Instance.ViewControl.LeftBar.ClearSelection(true);
            FormMain2.Instance.ViewControl.FrmEquipHistory.ClearSelection(true);
            this.Close();
        }
    }
}
