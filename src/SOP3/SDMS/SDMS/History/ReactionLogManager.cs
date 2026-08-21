using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace SDMS
{
	

	public class ReactionLogManager
	{
		private static ReactionLogManager m_Instance = null;
		public static ReactionLogManager Instance
		{
			get
			{
				if (m_Instance == null)
					m_Instance = new ReactionLogManager();
				return m_Instance;
			}
		}

		public Int32 Count
		{
			get
			{
				mMutex.WaitOne();
				Int32 result = m_arLogHistory.Count;
				mMutex.ReleaseMutex();
				return result;
			}
		}

		private ArrayList m_arLogHistory = new ArrayList();
		public System.Collections.ArrayList LogHistory
		{
			get { return (ArrayList)m_arLogHistory.Clone(); }
		}

		private ArrayList m_arLogList = new ArrayList();
		private Mutex mMutex = new Mutex(false);
		
		protected ReactionLogManager()
		{
		}

		public void ClearLog()
		{
			try
			{
				mMutex.WaitOne();
				m_arLogHistory.Clear();
			}
			finally
			{
				mMutex.ReleaseMutex();
			}
		}

		public void ProcessLog(ReactionLog log, bool bNewLog = false)
		{
            if (log == null || log.SensorHistoryID == -1)
                return;

			if (log.ReactionType == (int)(ReactionType.MALFUNCTION) ||
				(log.ReactionType == (int)(ReactionType.FINISH_SOP)) ||
				(log.ReactionType == (int)(ReactionType.IGNORE_SOP)))
			{
				if (bNewLog == true)
				{
					// 화재 상황 종료
					FormMain.Instance.Invoke((MethodInvoker)delegate
					{
						FormMain.Instance.EndFireProcess(log);
					});
				}				
			}

			if (log.ReactionType == (int)(ReactionType.BEGIN_STATUS))
			{
				FormMain.Instance.Invoke((MethodInvoker)delegate
				{
					// 화재 탐지 모드
					FormMain.Instance.SetFireDetectMode(log);
				});
			}
			else if (log.ReactionType == (int)(ReactionType.NOTIFY_FIRE))
			{
				if (bNewLog == true)
				{
					FormMain.Instance.Invoke((MethodInvoker)delegate
					{
                        int nSensorID = SensorHistoryManager.Instance.GetSensorID(log.SensorHistoryID);
                        if( nSensorID != -1)
                        {
                            // 화재 신호시 Select 처리 추가 skkim 2014-03-03
                            FireDetectProcess process = (FireDetectProcess)ProcessManager.Instance.GetProcess(nSensorID);
                            FormMain.Instance.BeginFireProcess(log);

                            if (process != null)
                                process.Select();

                            FormMain.Instance.SendFireDetectMessageToSOPSimulator();
                        }
                        
					});
				}
			}
			else if (log.ReactionType == (int)(ReactionType.TRAINNING_FIRE))
			{
				if (bNewLog == true)
				{
					FormMain.Instance.Invoke((MethodInvoker)delegate
					{
						FormMain.Instance.BeginFireProcess(log);
					});
				}
			}
			else if (log.ReactionType == (int)(ReactionType.RUN_SOP))
			{
				FormMain.Instance.Invoke((MethodInvoker)delegate
				{
					FormMain.Instance.SetRunSOPMode(log);
                    int nSensorID = SensorHistoryManager.Instance.GetSensorID(log.SensorHistoryID);
                    if (nSensorID != -1)
                    {
                        // 화재 신호시 Select 처리 추가 skkim 2014-03-03
                        FireDetectProcess process = (FireDetectProcess)ProcessManager.Instance.GetProcess(nSensorID);
                        if (process != null)
                            process.Select();
                    }
				});
			}
			else if (log.ReactionType == (int)(ReactionType.RUN_N_CANCEL_SOP))
			{
				FormMain.Instance.Invoke((MethodInvoker)delegate
				{
					FormMain.Instance.SetRunNCancelSOPMode(log);
                    int nSensorID = SensorHistoryManager.Instance.GetSensorID(log.SensorHistoryID);
                    if (nSensorID != -1)
                    {
                        // 화재 신호시 Select 처리 추가 skkim 2014-03-03
                        FireDetectProcess process = (FireDetectProcess)ProcessManager.Instance.GetProcess(nSensorID);
                        if (process != null)
                            process.Select();
                    }
				});
			}
			else if (log.ReactionType == (int)(ReactionType.FINISH_SOP))
			{
				FormMain.Instance.Invoke((MethodInvoker)delegate
				{
					FormMain.Instance.SetFinishSOPMode(log);

                    int nSensorID = SensorHistoryManager.Instance.GetSensorID(log.SensorHistoryID);
                    if (nSensorID != -1)
                    {
                        // 화재 신호시 Select 처리 추가 skkim 2014-03-03
                        FireDetectProcess process = (FireDetectProcess)ProcessManager.Instance.GetProcess(nSensorID);
                        if (process != null)
                            process.Select();
                    }
				});
			}
			else if (log.ReactionType == (int)(ReactionType.IGNORE_SOP))
			{
				FormMain.Instance.Invoke((MethodInvoker)delegate
				{
					FormMain.Instance.SetIgnoreSOPMode(log);
                    int nSensorID = SensorHistoryManager.Instance.GetSensorID(log.SensorHistoryID);
                    if (nSensorID != -1)
                    {
                        // 화재 신호시 Select 처리 추가 skkim 2014-03-03
                        FireDetectProcess process = (FireDetectProcess)ProcessManager.Instance.GetProcess(nSensorID);
                        if (process != null)
                            process.Select();
                    }
				});
			}
			else if (log.ReactionType != (int)(ReactionType.END_STATUS))
			{
				FormMain.Instance.Invoke((MethodInvoker)delegate
				{
					FormMain.Instance.SetNormalMode(log);
				});
			}

            if (log.ReactionType != (int)(ReactionType.END_STATUS))
            {
                FormMain.Instance.Invoke((MethodInvoker)delegate
                {
                    FormMain.Instance.AddLogMessage(log);
                });
            }
		}

		public void AddLog(ReactionLog log)
		{
			try
			{
				mMutex.WaitOne();
				if (!m_arLogHistory.Contains(log))
				{
					m_arLogHistory.Add(log);
				}
			}
			finally
			{
				mMutex.ReleaseMutex();
			}
			
			ProcessLog(log, true);
			
			FireDetectProcess process = (FireDetectProcess)ProcessManager.Instance.FindProcess(log.SensorHistoryID);
			if (process != null)
			{
				process.LastLog = log;
			}
		}
	}

	enum ReactionType
	{
		BEGIN_STATUS = 0,
		RUN_BROADCAST = 10,
		SEND_SMS = 11,
		MALFUNCTION = 21,
		NOTIFY_FIRE = 22,
		IGNORE_FIRE = 23,
		TRAINNING_FIRE = 24,
		RUN_SOP = 30,
		RUN_N_CANCEL_SOP = 31,
		FINISH_SOP = 32,
		IGNORE_SOP = 33,
        END_STATUS = 50,
		ETC = 100
	}

	public class ReactionLog
	{
		

		private int m_nID = -1;
		public int ID
		{
			get { return m_nID; }
			set { m_nID = value; }
		}
		private int m_nSensorHistoryID = -1;
		public int SensorHistoryID
		{
			get { return m_nSensorHistoryID; }
			set { m_nSensorHistoryID = value; }
		}
		private int m_nReactionType = 0;
		public int ReactionType
		{
			get { return m_nReactionType; }
			set { m_nReactionType = value; }
		}
		private DateTime m_LogTime;
		public System.DateTime LogTime
		{
			get { return m_LogTime; }
			set { m_LogTime = value; }
		}
		private string m_szMessage = "";
		public string Message
		{
			get { return m_szMessage; }
			set { m_szMessage = value; }
		}
		private string m_szParameter1 = "";
		public string Parameter1
		{
			get { return m_szParameter1; }
			set { m_szParameter1 = value; }
		}
		private string m_nParam2 = "";
		public string Parameter2
		{
			get { return m_nParam2; }
			set { m_nParam2 = value; }
		}
        private string m_szParameter3= "";
        public string Parameter3
        {
            get { return m_szParameter3; }
            set { m_szParameter3 = value; }
        }
        private string m_nParam4 = "";
        public string Parameter4
        {
            get { return m_nParam4; }
            set { m_nParam4 = value; }
        }
        private string m_nParam5 = "";
        public string Parameter5
        {
            get { return m_nParam5; }
            set { m_nParam5 = value; }
        }	
		
		public override string ToString()
		{
			return String.Format("{0} {1}", m_LogTime.ToLongTimeString(), m_szMessage);
		}
	}
}
