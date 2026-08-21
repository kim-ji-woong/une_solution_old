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
using System.Collections;

namespace SDMS_Building.PopupDialog.Controls
{
    public partial class uBroadcast : UserControl
    {
        private bool m_bFireBroadcast = false;
        private bool m_bPSMBroadcast = false;
        private bool m_bBlackoutBroadcast = false;
        private bool m_bSubmergencyBroadcast = false;
        private bool m_bTerrorBroadcast = false;
        private bool m_bCoronaBroadcast = false;
        private bool m_bEarthquakeBroadcast = false;

        private static Panel4Unity m_panelUnity = null;

        private List<RibbonButton> m_btns = null;

        public uBroadcast()
        {
            InitializeComponent();
            
            rbtnFire.Tag = IFacility.FacilityType.FIRE_SENSOR;
            rbtnPSM.Tag = IFacility.FacilityType.PSM_SENSOR;
            rbtnBlackout.Tag = IFacility.FacilityType.BLACKOUT;
            rbtnSubmergency.Tag = IFacility.FacilityType.SUBMERGENCY;
            rbtnTerror.Tag = IFacility.FacilityType.TERROR;
            rbtnCorona.Tag = IFacility.FacilityType.CORONA;
            rbtnEarthquake.Tag = IFacility.FacilityType.Earthquake;

            rbtnFire.IsChecked = false;
            rbtnPSM.IsChecked = false;
            rbtnBlackout.IsChecked = false;
            rbtnSubmergency.IsChecked = false;
            rbtnTerror.IsChecked = false;
            rbtnCorona.IsChecked = false;
            rbtnEarthquake.IsChecked = false;
        }

        public void SetButtons()
        {
            m_btns = new List<RibbonButton>();
            m_btns.Add(rbtnFire);
            m_btns.Add(rbtnPSM);
            m_btns.Add(rbtnBlackout);
            m_btns.Add(rbtnSubmergency);
            m_btns.Add(rbtnTerror);
            m_btns.Add(rbtnCorona);
            m_btns.Add(rbtnEarthquake);

            //rbtnFire.Visible = true;
            //rbtnCCTV.Visible = true;
            ////if (UnE.SOP.ProxySOP.Instance.UsePSM)
            ////    rbtnPSM.Visible = true;
            //if (UnE.SOP.ProxySOP.Instance.UseDoor)
            //    rbtnDoor.Visible = true;
            //if (UnE.SOP.ProxySOP.Instance.UseFirewall)
            //    rbtnFireWall.Visible = true;

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
        private void uBroadcast_Load(object sender, EventArgs e)
        {
        }

        private void rbtn_Click(object sender, EventArgs e)
        {
            RibbonButton rbtn = sender as RibbonButton;
            if (rbtn == null || rbtn.Tag == null)
                return;

            IFacility.FacilityType facilityType = (IFacility.FacilityType)rbtn.Tag;
            string strMsg = IFacility.GetFacilityTypeString(facilityType).Replace(" ", "").Replace("센서", "");

            strMsg += " 방송을 실행 또는 중지할 수 있습니다.";

            FormMessageBox msg = new FormMessageBox(strMsg, MessageBoxButtons.YesNoCancel);
            msg.YesText = "실행";
            msg.NoText = "중지";
            msg.StartPosition = FormStartPosition.CenterParent;

            bool isBegin = false;
            DialogResult result = msg.ShowDialog();
            if (result == DialogResult.Yes)
            {
                isBegin = true;
            }
            else if (result == DialogResult.No)
            {
                isBegin = false;
            }
            else
                return;

            StringBuilder sb = new StringBuilder();
            sb.Append("Insert into BroadcastCommand(ID, timeStamp, FacilityType, IsBegin) ");
            sb.AppendFormat("Values((Select isnull(Max(ID) + 1, 1) from BroadcastCommand), getDate(), {0}, {1})"
                , (int)facilityType, (isBegin) ? 1 : 0);

            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(sb.ToString());
        }

        private void btnMain_Click(object sender, EventArgs e)
        {
            this.Visible = false;

            FormMain.Instance.SetVisible3DPopup(false);
        }
    }
}
