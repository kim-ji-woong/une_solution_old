using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnE.Sensor;
using UnE.GUI;
using UnE.Util.Unity;

namespace SDMS_Building.PopupDialog.Controls
{
    public partial class uPoiVisible : UserControl
    {
        private static bool m_bVisiblePOIFire = true;
        public static bool bVisiblePOIFire
        {
            get { return m_bVisiblePOIFire; }
            set { m_bVisiblePOIFire = value; }
        }

        private static bool m_bVisiblePOICCTV = true;
        public static bool bVisiblePOICCTV
        {
            get { return m_bVisiblePOICCTV; }
            set { m_bVisiblePOICCTV = value; }
        }

        private static bool m_bVisiblelPOIDoor = true;
        public static bool bVisiblePOIDoor
        {
            get { return m_bVisiblelPOIDoor; }
            set { m_bVisiblelPOIDoor = value; }
        }

        private static bool m_bVisiblePOIFirewall = false;
        public static bool bVisiblePOIFirewall
        {
            get { return m_bVisiblePOIFirewall; }
            set { m_bVisiblePOIFirewall = value; }
        }

        private static bool m_bVisiblePOIPSM = false;
        public static bool bVisiblePOIPSM
        {
            get { return m_bVisiblePOIPSM; }
            set { m_bVisiblePOIPSM = value; }
        }

        private static Panel4Unity m_panelUnity = null;

        private List<RibbonButton> m_btns = null;

        public uPoiVisible()
        {
            InitializeComponent();
            
            rbtnFire.Tag = IFacility.FacilityType.FIRE_SENSOR;
            rbtnCCTV.Tag = IFacility.FacilityType.CCTV;
            rbtnDoor.Tag = IFacility.FacilityType.DOOR;
            rbtnFireWall.Tag = IFacility.FacilityType.FIREWALL;
            //rbtnPSM.Tag = IFacility.FacilityType.PSM_SENSOR;

            rbtnFire.IsChecked = true;
            rbtnCCTV.IsChecked = true;
            rbtnDoor.IsChecked = true;
        }

        public void SetButtons()
        {
            m_btns = new List<RibbonButton>();
            m_btns.Add(rbtnFire);
            m_btns.Add(rbtnCCTV);
            //m_btns.Add(rbtnPSM);
            m_btns.Add(rbtnDoor);
            m_btns.Add(rbtnFireWall);

            rbtnFire.Visible = true;
            rbtnCCTV.Visible = true;
            //if (UnE.SOP.ProxySOP.Instance.UsePSM)
            //    rbtnPSM.Visible = true;
            if (UnE.SOP.ProxySOP.Instance.UseDoor)
                rbtnDoor.Visible = true;
            if (UnE.SOP.ProxySOP.Instance.UseFirewall)
                rbtnFireWall.Visible = true;

            int empty = 6;
            //int beginX = btnMain.Location.X;
            //int beginY = btnMain.Location.Y + btnMain.Height + empty;

            //for (int i = 0; i < m_btns.Count; i++)
            //{
            //    if (!m_btns[i].Visible)
            //        continue;

            //    m_btns[i].Location = new Point(beginX, beginY);
            //    beginY = beginY + m_btns[i].Height + empty;

            //    if (m_btns.Count - 1 == i)
            //        m_nHeight = beginY;
            //}

            int beginX = 10;
            int beginY = 3;

            for (int i = 0; i < m_btns.Count; i++)
            {
                if (!m_btns[i].Visible)
                    continue;

                m_btns[i].Location = new Point(beginX, beginY);
                beginX = beginX + m_btns[i].Width + empty;

                if (m_btns.Count - 1 == i)
                    m_nWidth = beginX;
            }

            btnMain.Location = new Point(beginX - empty, beginY);

            this.Size = new Size(beginX + btnMain.Width, btnMain.Height + 6);

            Region = System.Drawing.Region.FromHrgn(FormMain.CreateRoundRectRgn(0, 0, this.Width, this.Height, 50, 50));
        }

        private int m_nWidth = 0;
        private int m_nHeight = 0;
        private void uPoiVisible_Load(object sender, EventArgs e)
        {
        }

        private Pen m_penBorder = new Pen(Color.FromArgb(0x2e, 0xa1, 0xff));
        private void uPoiVisible_Paint(object sender, PaintEventArgs e)
        {
            //Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            //e.Graphics.DrawRectangle(m_penBorder, 0, 0, this.Width - 1, this.Height - 1);
        }
                
        private void rbtnFire_Click(object sender, EventArgs e)
        {
            m_bVisiblePOIFire = rbtnFire.IsChecked = !rbtnFire.IsChecked;
            rbtnFire.Refresh();

            //FormMain.Instance.SetPOIVisible((IFacility.FacilityType)rbtnFire.Tag, rbtnFire.IsChecked);
            SetFireLayer();
        }

        private void rbtnCCTV_Click(object sender, EventArgs e)
        {
            m_bVisiblePOICCTV = rbtnCCTV.IsChecked = !rbtnCCTV.IsChecked;
            rbtnCCTV.Refresh();

            //FormMain.Instance.SetPOIVisible((IFacility.FacilityType)rbtnCCTV.Tag, rbtnCCTV.IsChecked);
            SetCCTVLayer();
        }

        private void rbtnDoor_Click(object sender, EventArgs e)
        {
            m_bVisiblelPOIDoor = rbtnDoor.IsChecked = !rbtnDoor.IsChecked;             
            rbtnDoor.Refresh();

            //FormMain.Instance.SetPOIVisible((IFacility.FacilityType)rbtnDoor.Tag, rbtnDoor.IsChecked);
            SetDoorLayer();
        }

        private void rbtnPSM_Click(object sender, EventArgs e)
        {
            m_bVisiblePOIPSM = rbtnPSM.IsChecked = !rbtnPSM.IsChecked;
            rbtnPSM.Refresh();

            //FormMain.Instance.SetPOIVisible((IFacility.FacilityType)rbtnPSM.Tag, rbtnPSM.IsChecked);
            SetPSMLayer();
        }

        private void rbtnFireWall_Click(object sender, EventArgs e)
        {
            m_bVisiblePOIFirewall = rbtnFireWall.IsChecked = !rbtnFireWall.IsChecked;
            rbtnFireWall.Refresh();

            //FormMain.Instance.SetPOIVisible((IFacility.FacilityType)rbtnFireWall.Tag, rbtnFireWall.IsChecked);
            SetFireWallLayer();
        }

        private void btnMain_Click(object sender, EventArgs e)
        {
            this.Visible = false;

            FormMain.Instance.SetVisible3DPopup(false);
        }

        public static void SetLayers(Panel4Unity panel)
        {
            if (panel != null)
                m_panelUnity = panel;

            SetCCTVLayer();
            SetFireLayer();
            SetFireWallLayer();
            SetDoorLayer();
            SetPSMLayer();
        }

        private static void SetLayer(string strLayerName, bool visible)
        {
            if (m_panelUnity == null)
                return;

            if (visible)
                m_panelUnity.ShowIconLayer(strLayerName);
            else
                m_panelUnity.HideIconLayer(strLayerName);
        }

        private static void SetCCTVLayer()
        {
            SetLayer("CCTV", m_bVisiblePOICCTV);
        }

        private static void SetFireLayer()
        {
            SetLayer("Fire", m_bVisiblePOIFire);
        }

        private static void SetFireWallLayer()
        {
            SetLayer("FireWall", m_bVisiblePOIFirewall);
        }

        private static void SetDoorLayer()
        {
            SetLayer("Door", m_bVisiblelPOIDoor);
        }

        private static void SetPSMLayer()
        {
            SetLayer("Gas", m_bVisiblePOIPSM);
        }
    }
}
