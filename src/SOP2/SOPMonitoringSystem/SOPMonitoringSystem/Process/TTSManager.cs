using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;



namespace SOPMonitoringSystem
{
    namespace Process
    {
        
        public delegate void SpeechStartEvent(object sender, SpeechEventArgs e);
        public delegate void SpeechEndEvent(object sender, SpeechEventArgs e);
        public delegate void SpeechPauseEvent(object sender, SpeechEventArgs e);

        public enum SpeechState
        {
            STANDBY = 1,
            PLAY = 2,
            STOP = 3,
            PAUSE = 4,
            REPEAT = 5
        }

        public class SpeechEventArgs
        {
            int nTotalCount = 0;
            public int Count
            {
                get { return nTotalCount; }
                set { nTotalCount = value; }
            }

            int nPlayCount = 0;
            public int PlayCount
            {
                get { return nPlayCount; }
                set { nPlayCount = value; }
            }

            string szMessage = "";
            public string Message
            {
                get { return szMessage; }
                set { szMessage = value; }
            }

            private SpeechState mState = SpeechState.STANDBY;
            public SpeechState State
            {
                get { return mState; }
                set { mState = value; }
            }

            public SpeechEventArgs()
            {
            }          
        }

        public class TTSManager : IDisposable
        {
            protected static TTSManager instance = null;
            public static TTSManager Instance
            {
                get
                {
                    if (instance == null)
                    {
                        instance = new TTSManager();
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


            //private SpeechSynthesizer ts = new SpeechSynthesizer();

            //public event SpeechStartEvent OnSpeechStarted;
            //public event SpeechEndEvent   OnSpeechEnded;
            //public event SpeechPauseEvent OnSpeechPaused;

           // private bool bInit = false;
            //private bool bRepeat = false;
            //private string szMessage = "";
            //private string szNOP = ",,,,,,";

            private WebDBManager mDBMgr = null;
            
            public SOPMonitoringSystem.WebDBManager DBMgr
            {
                get { return mDBMgr; }
                set { mDBMgr = value; }
            }

            private TTSManager()
            {                   
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

                string szSQL = "SELECT HOSTADDRESS, HEARTBEAT, BSTATE, BDescription from BroadcastState where id = 1";

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
                if (FormMain.Instance.UseBroadcast == true)
                {
                    if (mDBMgr == null || msg == null)
                        return;

                    DateTime nDate = DateTime.Now;

                    string szSQL = string.Format("INSERT INTO Broadcast (Text, UseSiren, PlayOption, RepeatCount, AddTime) VALUES('{0}','{1}','{2}','{3}','{4} {5:00}:{6:00}:{7:00}')",
                        msg.Message, msg.UseSiren, msg.PlayOption, msg.RepeatCount, nDate.ToShortDateString(), nDate.Hour, nDate.Minute, nDate.Second);

                    mDBMgr.GetResultData(szSQL, 0);

                    string szSQL2 = string.Format("INSERT INTO BroadcastHistory (Text, UseSiren, PlayOption, RepeatCount, HostInfo, AddTime) VALUES('{0}','{1}','{2}','{3}','{4}', '{5} {6:00}:{7:00}:{8:00}')",
                       msg.Message, msg.UseSiren, msg.PlayOption, msg.RepeatCount, "", nDate.ToShortDateString(), nDate.Hour, nDate.Minute, nDate.Second);

                    mDBMgr.GetResultData(szSQL2, 0);         
                }                      
            }

            public void AddSpeech(string szMsg, int nPlayback, bool bUseSiren)
            {
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
        }
    }       
}
