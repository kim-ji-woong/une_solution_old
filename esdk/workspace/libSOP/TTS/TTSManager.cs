using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using DBUtility;

namespace UnE
{
    namespace SOP
    {
        namespace TTS
        {
            public enum SpeechState
            {
                ERROR = -1,
                STANDBY = 1,
                PLAY = 2,
                STOP = 3,
                PAUSE = 4,
                REPEAT = 5
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

                private SpeechState mState = SpeechState.ERROR;
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
                public WebDBManager DBMgr
                {
                    get { return mDBMgr; }
                    set { mDBMgr = value; }
                }

                private int m_nSiteID = 1;
                private TTSManager()
                {
                    m_nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;
                }

                public void Dispose()
                {
                }

                public void SetState()
                {
                    m_nReadState = ReadHeartBeat();
                    if (m_nReadState == -1)
                    {
                        mState = SpeechState.ERROR;
                    }
                    else if (m_nReadState == 1)
                    {
                        mState = SpeechState.STANDBY;
                    }
                    else if (m_nReadState == 2)
                    {
                        mState = SpeechState.PLAY;
                    }
                    else if (m_nReadState == 3)
                    {
                        mState = SpeechState.STOP;
                    }
                    else if (m_nReadState == 4)
                    {
                        mState = SpeechState.PAUSE;
                    }
                    else if (m_nReadState == 5)
                    {
                        mState = SpeechState.REPEAT;
                    }
                }

                // 가장 마지막에 ReadHeartBeat()을 호출했던 시간
                private DateTime m_dtPrevReadHeartBeat = new DateTime();
                private bool m_bReadHeartBeat = false;
                private int m_nReadState = -1;
                private int ReadHeartBeat()
                {
                    int nResult = m_nReadState;
                    if (mDBMgr == null)
                        return -1;

                    // 서울대의 경우 방송서버가 없으므로 체크할 필요가 없다.
                    if (m_nSiteID == 100)
                        return -1;

                    DateTime dtNow = DateTime.Now;
                    TimeSpan span = dtNow - m_dtPrevReadHeartBeat;

                    // 마지막에 호출한 이후 1초가 지나지 않았으면 지난번 읽은 값을 리턴한다.
                    if (span.TotalSeconds < 1.0)
                    {
                        return nResult;
                    }

                    m_dtPrevReadHeartBeat = dtNow;

                    if (m_bReadHeartBeat == false)
                    {
                        nResult = -1;

                        m_bReadHeartBeat = true;
                        //string szSQL = "SELECT HOSTADDRESS, HEARTBEAT, BSTATE, BDescription from BroadcastState where id = 1";
                        string szText = "SELECT HOSTADDRESS, HEARTBEAT, BSTATE, BDescription FROM BroadcastState " +
                                        " WHERE id in (SELECT min(id) FROM BroadcastState WHERE SiteID = {0})";

                        string szSQL = string.Format(szText, m_nSiteID);
                        ArrayList arResult = mDBMgr.GetResultData(szSQL, 0);

                        if (arResult == null)
                        {
                            m_bReadHeartBeat = false;
                            return -1;
                        }                                               

                        DateTime nDate = DateTime.Now;

                        int i = 0;
                        if (arResult.Count == 4)
                        {
                            DateTime nLast = WebDBManager.GetDateTimeField(arResult[i + 1], nDate);

                            m_nReadState = WebDBManager.GetIntField(arResult[i + 2].ToString(), -1);

                            TimeSpan nInt = nDate - nLast;

                            if (nInt.TotalSeconds > 60)
                            {
                                nResult = -1;
                            }
                            else
                            {
                                nResult = m_nReadState;
                            }
                        }
                        m_bReadHeartBeat = false;
                    }                   
                    return nResult;
                }

                private void AddMessage(BroadcastMessage msg)
                {
                    if (bUseBroadcast == true)
                    {
                        if (mDBMgr == null || msg == null)
                            return;

                        DateTime nDate = DateTime.Now;

                        string szSQL2 = string.Format("INSERT INTO BroadcastHistory (Text, UseSiren, PlayOption, RepeatCount, HostInfo, AddTime, SiteID) VALUES('{0}', {1}, {2}, {3},'{4}', '{5} {6:00}:{7:00}:{8:00}', {9})",
                            msg.Message, msg.UseSiren ? 1 : 0, msg.PlayOption, msg.RepeatCount, "", nDate.ToShortDateString(), nDate.Hour, nDate.Minute, nDate.Second, m_nSiteID);

                        mDBMgr.GetResultData(szSQL2, 0);

                        string szSQL = string.Format("INSERT INTO Broadcast (Text, UseSiren, PlayOption, RepeatCount, AddTime, SiteID) VALUES('{0}', {1}, {2}, {3},'{4} {5:00}:{6:00}:{7:00}', {8})",
                            msg.Message, msg.UseSiren ? 1 : 0, msg.PlayOption, msg.RepeatCount, nDate.ToShortDateString(), nDate.Hour, nDate.Minute, nDate.Second, m_nSiteID);

                        mDBMgr.GetResultData(szSQL, 0);
                    }
                }

                public void AddSpeech(string szMsg, int nPlayback, bool bUseSiren)
                {
                    if (bUseBroadcast == false)
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
                    if (bUseBroadcast == false)
                        return;

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
}
