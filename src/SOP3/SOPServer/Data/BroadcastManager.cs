using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Diagnostics;
using DBUtility;



namespace SDMSServer
{
	public enum SpeechState
	{
		STANDBY = 1,
		PLAY = 2,
		STOP = 3,
		PAUSE = 4,
		REPEAT = 5
	}

	public class BroadcastManager : IDisposable
	{
        public enum SituationType
        {
            DETECT_FIRE = 0,    // 화재 탐지
            REPORT_FIRE = 1     // 화재 신고
        }

		protected static BroadcastManager instance = null;
		public static BroadcastManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new BroadcastManager(NetworkServer.Instance.DBManager);
				}
				return instance;
			}
		}

		private SpeechState mState = SpeechState.STANDBY;
		public SpeechState State
		{
			get { return mState; }
			set { mState = value; }
		}

		private int nPlayback = 0;
		public int PlaybackCount
		{
			get { return nPlayback; }
			set { nPlayback = value; }
		}

		private int nPlayCount = 0;
		public int PlayCount
		{
			get { return nPlayCount; }
			set { nPlayCount = value; }
		}

		private bool bUseBroadcast = true;
		public bool UseBroadcast
		{
			get { return bUseBroadcast; }
			set { bUseBroadcast = value; }
		}
		
	

		private WebDBManager mDBMgr = null;
        private int m_nSiteID = 1;

		private BroadcastManager(WebDBManager DBMgr)
		{
            m_nSiteID = NetworkServer.Instance.SiteID;

			mDBMgr = DBMgr;
		}

		public void Dispose()
		{
		}

		public void SetState()
		{
			int nState = ReadHeartBeat();
			if (nState == -1)
			{
				mState = SpeechState.STANDBY;
			}
			else if (nState == 1)
			{
				mState = SpeechState.STANDBY;
			}
			else if (nState == 2)
			{
				mState = SpeechState.PLAY;
			}
			else if (nState == 3)
			{
				mState = SpeechState.STOP;
			}
			else if (nState == 4)
			{
				mState = SpeechState.PAUSE;
			}
			else if (nState == 5)
			{
				mState = SpeechState.REPEAT;
			}
		}

		private int ReadHeartBeat()
		{
			if (mDBMgr == null)
				return -1;

            string szSQL = "SELECT HOSTADDRESS, HEARTBEAT, BSTATE, BDescription from BroadcastState WHERE SiteID = " + m_nSiteID.ToString();

			ArrayList arResult = mDBMgr.GetResultData(szSQL, 0);

			if (arResult == null)
			{
				return -1;
			}

			int nResult = -1;

			DateTime nDate = DateTime.Now;

			int i = 0;
			if (arResult.Count == 4)
			{
				DateTime nLast = WebDBManager.GetDateTimeField(arResult[i + 1], nDate);

				int nState = WebDBManager.GetIntField(arResult[i + 2].ToString(), -1);

				TimeSpan nInt = nDate - nLast;

				if (nInt.TotalSeconds > 60)
				{
					nResult = 3;

				}
				else
				{
					nResult = nState;

				}
			}
			return nResult;
		}


		public void AddMessage(BroadcastMessage msg)
		{
			//if (FormMain.Instance.UseBroadcast == true)
			{
				if (mDBMgr == null || msg == null)
					return;

				DateTime nDate = DateTime.Now;

				string szSQL = string.Format("INSERT INTO Broadcast (Text, UseSiren, PlayOption, RepeatCount, AddTime, SiteID) VALUES('{0}', {1}, {2}, {3},'{4} {5:00}:{6:00}:{7:00}', {8})",
                    msg.Message, msg.UseSiren ? 1 : 0, msg.PlayOption, msg.RepeatCount, nDate.ToShortDateString(), nDate.Hour, nDate.Minute, nDate.Second, m_nSiteID);

				mDBMgr.GetResultData(szSQL, 0);

                string szSQL2 = string.Format("INSERT INTO BroadcastHistory (Text, UseSiren, PlayOption, RepeatCount, HostInfo, AddTime,SiteID) VALUES('{0}', {1}, {2}, {3}, '{4}', '{5} {6:00}:{7:00}:{8:00}', {9})",
					msg.Message, msg.UseSiren ? 1 : 0, msg.PlayOption, msg.RepeatCount, "", nDate.ToShortDateString(), nDate.Hour, nDate.Minute, nDate.Second, m_nSiteID);

				mDBMgr.GetResultData(szSQL2, 0);
			}
		}

		public void AddSpeech(string szMsg, int nPlayback, bool bUseSiren, SituationType type)
		{
            if (IsEnabled(type) == false)
                return;

			BroadcastMessage message = new BroadcastMessage();
			message.Message = szMsg;
			message.RepeatCount = nPlayback;
			message.UseSiren = bUseSiren;
			message.PlayOption = 1;
			AddMessage(message);
		}

		public void StopSpeech()
		{
			BroadcastMessage message = new BroadcastMessage();
			message.Message = "";
			message.RepeatCount = 1;
			message.UseSiren = false;
			message.PlayOption = 0;
			AddMessage(message);
		}

		public void PauseSpeech()
		{
			BroadcastMessage message = new BroadcastMessage();
			message.Message = "";
			message.RepeatCount = 0;
			message.UseSiren = false;
			message.PlayOption = 3;
			AddMessage(message);
		}

		public void ResumeSpeech()
		{
			BroadcastMessage message = new BroadcastMessage();
			message.Message = "";
			message.RepeatCount = 0;
			message.UseSiren = false;
			message.PlayOption = 2;
			AddMessage(message);
		}

        private bool m_bEnabled = false;
        public bool IsEnabled(SituationType type)
        {
            int nSituationType = (int)type;
            //string szSQL = "SELECT UseBroadcast FROM SDMSBroadcastConfig where SituationType = " + nSituationType.ToString();

            string szText = "SELECT UseBroadcast FROM SDMSBroadcastConfig WHERE SituationType = {0} and SiteID = {1}";
            string szSQL = string.Format(szText, nSituationType, m_nSiteID);

            ArrayList arResult = mDBMgr.GetResultData(szSQL, 0);
            if (arResult == null || arResult.Count == 0)
            {
                m_bEnabled = false;
            }
            else
            {
                int nTemp = WebDBManager.GetIntField(arResult[0].ToString(), -1);

                if (nTemp == 1)
                    m_bEnabled = true;
                else
                    m_bEnabled = false;
            }
            return m_bEnabled;
        }       
            
	}

}
