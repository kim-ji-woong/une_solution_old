using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XtremeDockingPane;
using System.Collections;

namespace FireManagement
{
    public partial class FormPanel : Form, Ubists.IReaderOwner
    {
        private Form[] m_arrDocking = new Form[2];
        private DockingLeftBar m_dockLeft = new DockingLeftBar();
        private bool m_isConnectedRFIDReader = false;

        public FormPanel()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            labelZoneName.Text = "";

            CreatePane();

            if (!FormMain2.Instance.IsPCMode)
                dxfControl1.PanningMouseButton = System.Windows.Forms.MouseButtons.Left;
            
        }

        private void CreatePane()
        {
            Pane paneLayer = axDockingPane.CreatePane(0, 280, 190, DockingDirection.DockLeftOf, null);
            paneLayer.Title = "Layer";
            paneLayer.Options = PaneOptions.PaneNoCloseable;

            m_dockLeft = new DockingLeftBar();
            m_arrDocking[0] = m_dockLeft;

            axDockingPane.VisualTheme = VisualTheme.ThemeVisualStudio2010;

            
        }

        public DXFViewer.DXFControl DXFControl
        {
            get { return dxfControl1; }
        }

        private void FormPanel_Resize(object sender, EventArgs e)
        {
            ResizeControls();
        }

        public void ResizeControls()
        {
            int x = m_dockLeft.GridWidth + 4;//m_dockLeft.Location.X + m_dockLeft.Size.Width;
            int nDXFWidth = this.Size.Width - x + 2;

            labelZoneName.Location = new Point(x, 0);
            labelZoneName.Size = new Size(nDXFWidth, labelZoneName.Size.Height);

            dxfControl1.Location = new Point(x, labelZoneName.Size.Height);
            dxfControl1.Size = new Size(nDXFWidth, this.Size.Height - labelZoneName.Size.Height);
        }

        private void axDockingPane_AttachPaneEvent(object sender, AxXtremeDockingPane._DDockingPaneEvents_AttachPaneEvent e)
        {
            int nIndex = e.item.Id;

            if (nIndex == 0)
                e.item.Handle = m_arrDocking[0].Handle.ToInt32();
        }

        public DockingLeftBar LeftBar
        {
            get { return m_dockLeft; }
        }

        private void dxfControl1_MouseDown(object sender, MouseEventArgs e)
        {
            FormMain2 frmMain = FormMain2.Instance;

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (frmMain.NeedScreenInput())
                {
                    UnE.Geometry.Vertex2D vPos = dxfControl1.ScreenToGlobal(e.X, e.Y);
                    UnE.Geometry.Vertex2D vMove = dxfControl1.MovedVertex;

                    float fUnitFlag = frmMain.GetUnitFlag(DXFViewer.UnitOfLength.METER);
                    frmMain.ScreenInput((vPos.x - vMove.x) * fUnitFlag, (vPos.y - vMove.y) * fUnitFlag);
                }
                else if (m_dockLeft.IsOpened)
                {
                    UnE.Geometry.Vertex2D vPos = dxfControl1.ScreenToGlobal(e.X, e.Y);
                    DXFViewer.Shape shape = dxfControl1.PickObject(vPos.x, vPos.y);

                    if (shape != null)
                    {
                        if (frmMain.IsDeletingMode)
                        {
                            frmMain.DeleteEquipment(m_dockLeft.FindEquipment(shape));
                            frmMain.Refresh();
                        }
                        else
                        {
                            m_dockLeft.SelectShape(shape);
                            dxfControl1.Refresh();
                        }
                    }
                    else
                    {
                        //if (frmMain.IsEditingMode)
                        //{
                        //    if (m_dockLeft.SelectedEquipment != null)
                        //    {
                        //        m_dockLeft.SelectedEquipment.Move(vPos);
                        //        frmMain.Refresh();
                        //    }
                        //}
                        //else
                            m_dockLeft.ClearSelection(true);
                    }
                }
            }
        }

        public void SetLabelText(string strText)
        {
            labelZoneName.Text = strText;
        }

        private void dxfControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (dxfControl1.IsOpened)
            {
                UnE.Geometry.Vertex2D vertex = dxfControl1.ScreenToGlobal(e.X, e.Y);

                if (vertex != null)
                {
                    UnE.Geometry.Vertex2D vMove = dxfControl1.MovedVertex;
                    float fFlag = FormMain2.Instance.GetUnitFlag(DXFViewer.UnitOfLength.METER);
                    FormMain2.Instance.StatusText = string.Format("{0}, {1}, 단위(m)", (vertex.x - vMove.x)* fFlag, (vertex.y - vMove.y) * fFlag);
                }
            }
            else
                FormMain2.Instance.StatusText = "";
        }

        public void SetRFIDOwner()
        {
            if (FormMain2.Instance.IsPCMode)
                return;

            FormMain2.Instance.RFIDReader.Owner = this;
            m_isConnectedRFIDReader = FormMain2.Instance.RFIDReader.StartReading();
        }

        public void OnReadTag(string strTag)
        {
            if (m_dockLeft.SelectedEquipment == null)
                return;

            if (m_dockLeft.SelectedEquipment.RFIDTag == strTag)
                return;

            m_dockLeft.SetRFID(m_dockLeft.SelectedEquipment, strTag);
        }

        private void dxfControl1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!FormMain2.Instance.TagInputMode)
                return;

            if (m_dockLeft.SelectedEquipment == null)
                return;

            m_dockLeft.InputEquipID(m_dockLeft.SelectedEquipment, ((char)e.KeyValue).ToString());
        }
    }
}
