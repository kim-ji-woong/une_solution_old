using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility;
using System.Collections;
using SpeechLib;
using System.Threading;

namespace SOPMonitoringSystem.TTS
{
    public class TTSManager
    {
        public enum SpeechState
        {
            STANDBY = 1,
            PLAY = 2,
            STOP = 3,
            PAUSE = 4,
            REPEAT = 5
        }

        private static TTSManager m_instance = null;

        private WebDBManager m_dbMgr = null;
        private int m_nSiteID = 1;

        private SpeechState m_state = SpeechState.STANDBY;
        private bool m_runThread = false;
        //private List<BroadcastMessage> m_messages = new List<BroadcastMessage>();

        private SpVoice m_voice = new SpVoice();

        public static TTSManager Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new TTSManager(FormSOP.Instance.DBManager, UnE.SOP.ProxySOP.Instance.SiteID);

                return m_instance;
            }
        }

        private TTSManager(WebDBManager dbMgr, int nSiteID)
        {
            m_dbMgr = dbMgr;
            m_nSiteID = nSiteID;
            m_state = SpeechState.STOP;
        }

        public void Run()
        {
            if (m_runThread)
                return;

            m_runThread = true;

            Thread t = new Thread(new ThreadStart(RunThread));
            t.Start();
        }

        private void RunThread()
        {
            HeartBeat();
            BroadcastMessage message = ReadMessage();
            RunBroadcast(message);

            m_runThread = false;
        }

        private void HeartBeat()
		{
			DateTime nDate = DateTime.Now;
			string szSQL = string.Format("UPDATE BroadcastState SET HEARTBEAT= '{0} {1:00}:{2:00}:{3:00}', BSTATE ={4} WHERE ID = 1 and SiteID = {5}"
				, nDate.ToShortDateString(), nDate.Hour, nDate.Minute, nDate.Second, (int)m_state, m_nSiteID);

            m_dbMgr.GetResultData(szSQL, 0);
		}

        private BroadcastMessage ReadMessage()
        {
            //m_messages.Clear();

            string szSQL = "SELECT Text,UseSiren,PlayOption,RepeatCount,AddTime from Broadcast WHERE SiteID = " + m_nSiteID.ToString();
            ArrayList arrResult = m_dbMgr.GetResultData(szSQL, 0);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            BroadcastMessage lastMessage = null;

            for (int i=0;i<nResultCount-4;i+=5)
            {
                string strMessage = WebDBManager.GetStringField(arrResult[i]);
                VariousData<int> useSiren = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                VariousData<int> playOption = WebDBManager.GetIntField(arrResult[i + 2].ToString());
                VariousData<int> repeatCount = WebDBManager.GetIntField(arrResult[i + 3].ToString());
                VariousData<DateTime> addTime = WebDBManager.GetDateTimeField(arrResult[i + 4].ToString());

                if (strMessage == null || useSiren == null || playOption == null || repeatCount == null || addTime == null)
                    continue;

                BroadcastMessage data = new BroadcastMessage();

                data.Message = strMessage;
                data.UseSiren = useSiren.Data == 0 ? false : true;
                data.PlayOption = playOption.Data;
                data.RepeatCount = repeatCount.Data;
                data.AddTime = addTime.Data;

                if (data.PlayOption != -1)
                {
                    if (data.PlayOption == 1)
                    {
                        lastMessage = data;
                    }
                    //m_messages.Add(data);
                }
            }

            ClearMessage();

            return lastMessage;
        }

        private void ClearMessage()
        {
            string szSQL = " DELETE from Broadcast WHERE SiteID = " + m_nSiteID.ToString();
            m_dbMgr.GetResultData(szSQL, 0);
        }

        private void RunBroadcast(BroadcastMessage message)
        {
            if (message == null)
                return;

            if (FormSOP.Instance.UseBroadcast == false)
                return;

            try
            {
                m_state = SpeechState.PLAY;

                if (message.UseSiren)
                {
                    string szPath = System.Reflection.Assembly.GetEntryAssembly().Location;
                    string szFullPath = System.IO.Directory.GetParent(szPath).FullName;
                    string szFileName = szFullPath + "\\66084^air-raid-siren-alert.wav";
                    szFileName = szFileName.Replace("\\", "/");

                    SoundUtils.PlaySound(szFileName, false);
                }

                m_voice.Rate = 0;
                m_voice.Speak(message.Message, SpeechVoiceSpeakFlags.SVSFlagsAsync);

                m_state = SpeechState.STOP;
            }
            catch (Exception ex)
            {
                m_state = SpeechState.STOP;
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
    }

    class BroadcastMessage
    {
        protected int mID;
        public int ID
        {
            get { return mID; }
            set { mID = value; }
        }
        protected string message;
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        protected bool bUseSiren;
        public bool UseSiren
        {
            get { return bUseSiren; }
            set { bUseSiren = value; }
        }
        protected int mplayOption;
        public int PlayOption
        {
            get { return mplayOption; }
            set { mplayOption = value; }
        }
        protected int mRepeatCount;
        public int RepeatCount
        {
            get { return mRepeatCount; }
            set { mRepeatCount = value; }
        }

        protected DateTime mAddedTime;
        public System.DateTime AddTime
        {
            get { return mAddedTime; }
            set { mAddedTime = value; }
        }
    }
}
