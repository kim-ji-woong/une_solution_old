using System.Collections;

namespace SDMS
{
	internal class SimulationBroadcastOption
	{
		private static SimulationBroadcastOption m_instance = null;

		public static SimulationBroadcastOption Instance
		{
			get
			{
				if (m_instance == null)
					m_instance = new SimulationBroadcastOption();
				return m_instance;
			}
			set { m_instance = value; }
		}

		private SimulationBroadcastOption()
		{
			m_instance = this;

			LoadData();
		}

		private void LoadData()
		{
			FormBroadcastConfig frmBroadcastConfig = new FormBroadcastConfig();
			ArrayList arrOption = frmBroadcastConfig.GetBroadcastConfigOption();

			m_bUseBroadcast = (bool)arrOption[0];
			m_bUseBroadcast2 = (bool)arrOption[1];
			m_bUseSiren = (bool)arrOption[2];

			m_nRadioRepeat = 1;
			int.TryParse(arrOption[3].ToString(), out m_nRadioRepeat);

			m_strDetectMessage = arrOption[4].ToString();
			m_strReportMessage = arrOption[5].ToString();
		}

		public void SetData(bool useBroadcastDetect, bool useBroadcastReport, bool bUseSiren, int nRadioRepeat, string strDetectMessage, string strReportMessage)
		{
			m_bUseBroadcast = useBroadcastDetect;
			m_bUseBroadcast2 = useBroadcastReport;
			m_bUseSiren = bUseSiren;
			m_nRadioRepeat = nRadioRepeat;
			m_strDetectMessage = strDetectMessage;
			m_strReportMessage = strReportMessage;
		}

		private bool m_bUseBroadcast = false;
		private bool m_bUseBroadcast2 = false;
		private bool m_bUseSiren = false;
		private int m_nRadioRepeat = 0;
		private string m_strDetectMessage = null;
		private string m_strReportMessage = null;

		// 화재탐지시 방송할 것인가?
		public bool UseBroadcast
		{
			get { return m_bUseBroadcast; }
			set { m_bUseBroadcast = value; }
		}

		// 화재신고시 방송할 것인가?
		public bool UseBroadcast2
		{
			get { return m_bUseBroadcast2; }
			set { m_bUseBroadcast2 = value; }
		}

		public bool UseSiren
		{
			get { return m_bUseSiren; }
			set { m_bUseSiren = value; }
		}

		public int RadioRepeat
		{
			get { return m_nRadioRepeat; }
			set { m_nRadioRepeat = value; }
		}

		// 화재탐지 메시지
		public string DetectMessage
		{
			get { return m_strDetectMessage; }
			set { m_strDetectMessage = value; }
		}

		// 화재신고 메시지
		public string ReportMessage
		{
			get { return m_strReportMessage; }
			set { m_strReportMessage = value; }
		}
	}
}