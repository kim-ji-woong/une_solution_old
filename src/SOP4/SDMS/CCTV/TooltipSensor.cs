using System;
using System.Drawing;
using System.Windows.Forms;
using UnE.Sensor;


namespace SDMS
{
    public partial class TooltipSensor : Form, IPOIPopup
	{
		// Target과의 거리
		static private int m_nTargetSpaceX = 30;

		static private int m_nTargetSpaceY = 50;

        private ISensorTooltipOwner m_viewOwner = null;

		private bool m_bVisible = false;

		private bool m_isConnected = false;

		public bool Connected
		{
			get { return m_isConnected; }
		}

		private ISensor m_Sensor = null;

		public ISensor Sensor
		{
			get { return m_Sensor; } 
			set { m_Sensor = value; }
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

		private IFacility.FacilityType m_nSenstorType = IFacility.FacilityType.NONE;

        public TooltipSensor(ISensorTooltipOwner view, ISensor sensor, int nType)
		{
            this.TopLevel = false;
            m_bVisible = false;

            
            InitializeComponent();
			this.BringToFront();
			m_viewOwner = view;
            view.AddToolTipControl(this);
			m_Sensor = sensor;
			m_nSenstorType = IFacility.ToFacilityType(nType);
			base.Hide();

			SetStatus();
		}


		public void SetStatus()
		{
			if (m_Sensor == null)
			{
				m_TextStatus.Text = "연결 안됨";
				m_TextStatus.ForeColor = Color.Orange;
				return;
			}
			if (m_Sensor.Connected == true)
			{
				m_viewOwner.EnablePOI(m_Sensor.POI.ID, m_Sensor.IconPath, true);
				if (m_nSenstorType == IFacility.FacilityType.FIRE_SENSOR)
				{
					if (m_Sensor.SensorData != 1)
					{
						m_TextStatus.Text = "정상 운영";
						m_TextStatus.ForeColor = Color.LimeGreen;
					}
					else
					{
						m_TextStatus.Text = "화재 감지";
						m_TextStatus.ForeColor = Color.Red;
					}
				}
				else if (m_nSenstorType == IFacility.FacilityType.COOLER_SENSOR)
				{
					if (m_Sensor.SensorData != 1)
					{
						m_TextStatus.Text = "정지 상태";
						m_TextStatus.ForeColor = Color.LimeGreen;
					}
					else
					{
						m_TextStatus.Text = "스프링쿨러 동작중";
						m_TextStatus.ForeColor = Color.Red;
					}
				}
				else if (m_nSenstorType == IFacility.FacilityType.PRESSURE_SENSOR)
				{
					if (m_Sensor.SensorData != 1)
					{
						m_TextStatus.Text = "정상 압력";
						m_TextStatus.ForeColor = Color.LimeGreen;
					}
					else
					{
						m_TextStatus.Text = "비정상 압력";
						m_TextStatus.ForeColor = Color.Red;
					}
				}
				else if (m_nSenstorType == IFacility.FacilityType.CCTV)
				{
					if (m_Sensor.SensorData != 1)
					{
						m_TextStatus.Text = "정상 동작";
						m_TextStatus.ForeColor = Color.LimeGreen;
					}
					else
					{
						m_TextStatus.Text = "연결 끊김";
						m_TextStatus.ForeColor = Color.Red;
					}
				}
			}
			else
			{
				m_TextStatus.Text = "신호 두절";
				m_TextStatus.ForeColor = Color.Red;

				if (m_Sensor.POI.ID != -1)
				{
					m_viewOwner.EnablePOI(m_Sensor.POI.ID, m_Sensor.IconPath, false);
				}
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

			if (m_Sensor == null)
				return false;

			if (m_Sensor.POI == null)
				return false;
          
			return m_viewOwner.IsTemporaryHiddenPOI(m_Sensor.POI);
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

		private void label2_Click(object sender, EventArgs e)
		{
			if (m_Sensor != null)
			{
				MessageBox.Show("센서 고장신고");
			}
		}

		private void label3_Click(object sender, EventArgs e)
		{
			if (m_Sensor != null)
			{
				MessageBox.Show("센서 배치도 그림 보기");
			}
		}
	}	
}