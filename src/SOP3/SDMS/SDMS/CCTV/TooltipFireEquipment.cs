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
    public partial class TooltipFireEquipment : Form, IPOIPopup
    {
        // Target과의 거리
        static private int m_nTargetSpaceX = 30;
        static private int m_nTargetSpaceY = 50;

        private BaseViewEx m_viewOwner = null;

        private bool m_bVisible = false;
       
		private SDMS.FireEquipment m_equip = null;
		public SDMS.FireEquipment FireEquipment
        {
			get { return m_equip; }
			set { m_equip = value; }
        }

        private bool m_bLayerVisible = true;
        public bool LayerVisible
        {
            get { return m_bLayerVisible; }
            set 
            { 
                m_bLayerVisible = value;
                if (m_bLayerVisible == false)
                {
                    Visible = false;
                }
                else
                {
                    if (m_bVisible == true)
                    {
                        //base.Show();
                    }
                }
            }
        }


		private Facility.FacilityType m_type = Facility.FacilityType.NONE;

		public TooltipFireEquipment(BaseViewEx view, SDMS.FireEquipment equip, Facility.FacilityType type)
		{
			InitializeComponent();

			this.TopLevel = false;
			view.Controls.Add(this);
			this.BringToFront();
			m_viewOwner = view;
			m_bVisible = false;
            m_equip = equip;
            m_type = type;
			base.Hide();

			SetStatus();
		}

		public void SetStatus()			
		{
            if (m_equip == null || m_equip.Status == SDMS.FireEquipment.EquipmentStatus.UNKNOWN)
            {
                m_TextStatus.Text = "상태정보 없음";
                m_TextStatus.ForeColor = Color.Orange;

                labelTime.Visible = false;
                labelLastCheckedTime.Visible = false;

                return;
            }
            else
            {
                m_viewOwner.EnablePOI(m_equip.POI.ID, true);

                if (m_equip.Status == SDMS.FireEquipment.EquipmentStatus.NORMAL)
                    m_TextStatus.ForeColor = Color.LimeGreen;
                else
                    m_TextStatus.ForeColor = Color.Red;

                m_TextStatus.Text = m_equip.StatusString;
                labelLastCheckedTime.Text = string.Format("{0} {1:00}:{2:00}:{3:00}",
                    m_equip.LastCheckedTime.ToShortDateString(),
                    m_equip.LastCheckedTime.Hour,
                    m_equip.LastCheckedTime.Minute,
                    m_equip.LastCheckedTime.Second);

                labelTime.Visible = true;
                labelLastCheckedTime.Visible = true;
            }
		}

		// xTarget, yTarget : Target POI의 좌표
		public void Show(int xTarget, int yTarget)
		{
			int x = xTarget + m_nTargetSpaceX;
			int y = yTarget - m_nTargetSpaceY;

			this.Location = new Point(x, y);
			m_bVisible = true;

			SetStatus();

			this.Show();
		}


		// Panning이나 Orbit같은 동작을 위하여 잠시동안 임시로 꺼두는 것인가?
		private bool IsTemporaryHidden()
		{
			if (m_viewOwner == null)
				return false;

			if (m_equip == null)
				return false;

			if (m_equip.POI == null)
				return false;

			return m_viewOwner.IsTemporaryHiddenPOI(m_equip.POI);
		}

		public void Hide(bool absolutely)
		{
			//if (absolutely)
			//{
				base.Hide();
				m_bVisible = false;
			//}
		}

		public void MoveTarget(int xTarget, int yTarget)
		{
			int x = xTarget + m_nTargetSpaceX;
			int y = yTarget - m_nTargetSpaceY;

			this.Location = new Point(x, y);
		}

		public bool IsVisible()
		{
			if (m_bLayerVisible == true && m_bVisible == true)
				return true;
			return Visible;
		}

		public new void Close()
		{
			m_bLayerVisible = false;
			m_bVisible = false;
			Visible = false;
			base.Close();
		}
    }

    public partial class FireEquipment : Facility
    {
        public override IPOIPopup CreatePopup(BaseViewEx view)
        {
            return new TooltipFireEquipment(view, this, m_type);
        }
    }
}
