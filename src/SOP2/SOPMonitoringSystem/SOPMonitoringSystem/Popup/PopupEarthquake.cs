using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SOPMonitoringSystem
{

	public partial class PopupEarthquake : Form, AnnounceMessage
	{
		private Sections.Section mSection = null;

		private string m_strMode = "훈련 비상상황";

		private string szMessage = "";
		string AnnounceMessage.Message
		{
			get { return szMessage; }
			set { szMessage = value; }
		}
		private int nAnnoCount = 1;
		int AnnounceMessage.Count
		{
			get { return nAnnoCount; }
			set { nAnnoCount = value; }
		}
		private bool bUseSystemMsg = false;
		bool AnnounceMessage.UseSystemMessage
		{
			get { return bUseSystemMsg; }
			set { bUseSystemMsg = value; }
		}
		private string szSystemMsg = "";
		string AnnounceMessage.SystemMessage
		{
			get { return szSystemMsg; }
			set { szSystemMsg = value; }
		}
		private bool m_useSiren = true;
		bool AnnounceMessage.UseSiren
		{
			get { return m_useSiren; }
			set { m_useSiren = value; }
		}
		private bool m_useSenarioMessage = false;
		bool AnnounceMessage.UseSenarioMessage
		{
			get { return m_useSenarioMessage; }
			set { m_useSenarioMessage = value; }
		}
		private string m_szSenarioMessage = "";
		string AnnounceMessage.SenarioMessage
		{
			get { return m_szSenarioMessage; }
			set 
			{
				m_szSenarioMessage = value;
				if (m_szSenarioMessage != null && !m_szSenarioMessage.Equals(""))
				{
					m_comboDisaster.SelectedIndex = 3;
				}
			}
		}

		private string strMainText =
			"본부 재난안전대책본부에서 $1을 알려드립니다.\n" +
			"현재 지진발생이 감지되어 알려 드리오니 " +
			"전 직원은 휴대폰, 손전등, 마실 물, 소화기, 구급약품, 휴대용 라디오, 필기구 등을 \n" +
			"지참하시어 해안설비에서 먼 공터 또는 높은 지대나 튼튼한 건물 옥상으로 대피해" +
			"주시기 바랍니다.\n\n" +
			"미처 대피하지 못한 직원께서는 \n" +
			"책상이나 탁자 아래, 발전설비 내부, 내력벽이 있는 건물 공간으로 긴급 대피하시고, \n" +
			"부두, 방파제, 취․배수로, 고압가스 또는 위험물질 저장소, 매달린 물체 아래, 거울, \n" +
			"문, 액자, 발코니 근처는 피해주시기 바랍니다.";

		public PopupEarthquake(bool bVirtual)
			: base()
		{
			InitializeComponent();
			AdjustLocation(FormMain.Instance);
			RealMode(!bVirtual);
			szMessage = strMainText.Replace("$1", m_strMode);
			textMessage.Text = szMessage;
			textMessage.Enabled = true;
			textMessage.ReadOnly = true;
			cmbAnnoCount.SelectedIndex = 0;
			m_comboDisaster.SelectedIndex = 0;
			groupBox2.Hide();
		}

		private void AdjustLocation(Form parent)
		{
			Size size = parent.Size;
			Point p = parent.Location;
			int x = p.X + (size.Width / 2) - (this.Size.Width / 2);
			int y = p.Y + (size.Height / 2) - (this.Size.Height / 2);
			this.Location = new Point(x, y);
		}

		public void RealMode(bool isReal)
		{
			if (isReal)
				m_strMode = "실제 비상상황";
		}

		private void m_comboDisaster_SelectedIndexChanged(object sender, EventArgs e)
		{
			
		}

		private void cmbAnnoCount_SelectedIndexChanged(object sender, EventArgs e)
		{
			

		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if(SetBrodcast())
				DialogResult = DialogResult.OK;
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}

		private void btnBack_Click(object sender, EventArgs e)
		{
			if (btnNext.Text == "마침")
			{
				groupBox1.Show();
				groupBox2.Hide();

				btnNext.Text = "다음>";
			}
		}

		private void btnNext_Click(object sender, EventArgs e)
		{
			if(btnNext.Text == "다음>")
			{
				groupBox1.Hide();
				groupBox2.Show();

				btnNext.Text = "마침";
			}
			else if (btnNext.Text == "마침")
			{
				SetBrodcast();

				DialogResult = DialogResult.OK;
			}
		}

		private bool SetBrodcast()
		{
			m_useSiren = checkBoxSiren.Checked;

			if (cmbAnnoCount.SelectedIndex == -1)
				return false;

			if (bUseSystemMsg == true)
			{
				szMessage = szSystemMsg;
			}
			else
			{
				if (m_comboDisaster.SelectedIndex == -1)
					return false;
				int nSelect = m_comboDisaster.SelectedIndex;

				if (nSelect == 1 || nSelect == 2 || nSelect == 3)
				{
					szMessage = textMessage.Text;
				}
			}
			return true;
		}

		public void SetData(Sections.SectionState sectionState)
		{
			mSection = sectionState.Section;
			//this.Text = "외부상황전파 - " + mSection.Data.Title;
			btnBack.Visible = btnNext.Visible = btnCancel2.Visible = false;
			if (mSection.GetComponentType() == Sections.Section.ComponentType.TRANSMISSION)
			{
				btnBack.Visible = btnNext.Visible = btnCancel2.Visible = true;
				Sections.SectionDataTransmission sectionData = (Sections.SectionDataTransmission)mSection.Data;

				if (sectionData.DataExternal.UseSMS)
				{
					ArrayList list = sectionData.DataExternal.SMSReceivers;
					foreach (Sections.ExternalTeamData data in list)
					{
						DataGridViewRow row = new DataGridViewRow();
						DataGridViewCell cell1 = new DataGridViewTextBoxCell();
						cell1.Value = data.TeamName;
						row.Cells.Add(cell1);
						DataGridViewCell cell2 = new DataGridViewTextBoxCell();
						cell2.Value = data.PhoneNumber;
						row.Cells.Add(cell2);
						dataGridViewSMS.Rows.Add(row);
					}
					textBox1.Text = sectionData.DataExternal.SMSMessage;
					checkUseSMS.Checked = true;
				}

				if (sectionData.DataExternal.UseFax)
				{
					ArrayList list = sectionData.DataExternal.FaxReceivers;
					foreach (Sections.ExternalTeamData data in list)
					{
						DataGridViewRow row = new DataGridViewRow();
						DataGridViewCell cell1 = new DataGridViewTextBoxCell();
						cell1.Value = data.TeamName;
						row.Cells.Add(cell1);
						DataGridViewCell cell2 = new DataGridViewTextBoxCell();
						cell2.Value = data.PhoneNumber;
						row.Cells.Add(cell2);
						dataGridViewFax.Rows.Add(row);
					}
					textBox1.Text = sectionData.DataExternal.SMSMessage;
				}
			}
			
		}

		public string GetMessage()
		{
			return textBox1.Text;
		}

		private void m_comboDisaster_SelectedIndexChanged_1(object sender, EventArgs e)
		{
			ComboBox cbo = (ComboBox)sender;
			switch (cbo.SelectedIndex)
			{
				case 0:
					textMessage.Enabled = true;
					textMessage.ReadOnly = true;
					szMessage = strMainText.Replace("$1", m_strMode);
					textMessage.Text = strMainText.Replace("$1", m_strMode);
					textMessage.BackColor = Control.DefaultBackColor;
					bUseSystemMsg = false;
					m_useSenarioMessage = false;
					break;
				case 1:
					textMessage.ReadOnly = true;
					textMessage.Enabled = false;
					textMessage.Text = szSystemMsg;
					textMessage.BackColor = Control.DefaultBackColor;
					m_useSenarioMessage = false;
					bUseSystemMsg = true;
					break;
				case 2:
					textMessage.Enabled = true;
					textMessage.Text = "";
					textMessage.ReadOnly = false;
					textMessage.BackColor = Color.White;
					bUseSystemMsg = false;
					m_useSenarioMessage = false;
					break;
				case 3:
					textMessage.Enabled = true;
					textMessage.Text = m_szSenarioMessage;
					textMessage.ReadOnly = false;
					textMessage.BackColor = Color.White;
					bUseSystemMsg = false;
					m_useSenarioMessage = true;
					break;

			}
		}

		private void cmbAnnoCount_SelectedIndexChanged_1(object sender, EventArgs e)
		{
			ComboBox combo = (ComboBox)sender;
			string szText = combo.SelectedItem.ToString();
			if (szText == null || szText == "")
				return;

			if (szText == "상황 종료시까지 무한 반복")
			{
				nAnnoCount = -1;
			}
			else
			{
				nAnnoCount = int.Parse(szText);
			}
		}

	}
}
