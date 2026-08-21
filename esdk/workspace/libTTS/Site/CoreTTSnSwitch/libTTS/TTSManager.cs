using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using DBUtility2;
using System.Diagnostics;
using CoreTTSDotNet;
using System.Threading;

namespace libTTS
{
    public static class TTSFactory
    {
        public static ITTSManager MakeInstance()
        {
            return new TTSManager();
        }
    }

    class SpeechEventArgs
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

    class TTSManager : ITTSManager, IDisposable
    {
        public delegate void SpeechStartEvent(object sender, SpeechEventArgs e);
        public delegate void SpeechEndEvent(object sender, SpeechEventArgs e);
        public delegate void SpeechPauseEvent(object sender, SpeechEventArgs e);

        public event SpeechStartEvent OnSpeechStarted;
        public event SpeechEndEvent OnSpeechEnded;
        public event SpeechPauseEvent OnSpeechPaused;

        private SpeechState mState = SpeechState.STANDBY;
        private MultiMode m_multiMode = MultiMode.STOP_N_NEW_PLAY;

        private HeartBeatManager m_hbMgr = new HeartBeatManager();
        private MessageManager m_msgMgr = new MessageManager();
        private SerialManager mSerial = null;

        private CoreTTS ts = new CoreTTS();

        private string m_strServerIP = "127.0.0.1";
        private string m_strServerPort = "20030";
        private int m_nSpeed = 100;
        private float m_fVolume = 1.0f;

        private int m_nPlayback = 0;
        private int m_nPlayCount = 0;
        private string m_strMsg = "";
        static bool m_playSiren = false;
        static int m_nAddNew = 0;

        private bool m_isProcessing = false;
        private bool m_isInitialized = false;
        private bool m_repeat = false;
        private string m_szMessage = "";
        private string m_szNOP = ",,,,,,";

        private string m_strSirenFile = "66084^air-raid-siren-alert.wav";

        public SpeechState State
        {
            get { return mState; }
            set { mState = value; }
        }

        public int PlaybackCount
        {
            get { return m_nPlayback; }
            set { m_nPlayback = value; }
        }

        public int PlayCount
        {
            get { return m_nPlayCount; }
            set { m_nPlayCount = value; }
        }

        public void Initialize()
        {
            string szPath = Assembly.GetEntryAssembly().Location;
            string szFullPath = Directory.GetParent(szPath).FullName;

            System.IO.StreamWriter file = new System.IO.StreamWriter(szFullPath + "//server.log");

            Utility util = new Utility();
            string szRunModule = util.getinivalue("TTS Server Info", "run_module");
            file.WriteLine("RUN MODULE : " + szRunModule);

            if (szRunModule.Equals("1"))
            {
                string szModulePath = util.getinivalue("TTS Server Info", "module_path");
                szModulePath = szModulePath.Replace("\\", "/");
                string strPath = szModulePath + "/ttsserver.exe";

                file.WriteLine("MODULE PATH: " + strPath);

                RunStartProcess(strPath, "", szModulePath, file);
            }

            file.Close();

            try
            {
                LoadIni();

                int nPort = int.Parse(m_strServerPort);

                ts.Create();
                ts.Volume = m_fVolume;
                ts.Speed = m_nSpeed;

                ts.Config(m_strServerIP, nPort);

                mState = SpeechState.STOP;

                mSerial = new SerialManager();
                bool bCheck = mSerial.CheckSwitch();
                if (bCheck == false)
                {
                    //TraceLog("TTSManager initialize fail");
                    return;
                }


                SwitchOff();
                m_isInitialized = true;
                //TraceLog("TTSManager initialize success");
            }
            catch (Exception)
            {
                //MessageBox.Show("TTS 생성이 실패하였습니다.\n방송 메세지 전파을 사용할 수 없습니다.");
            }
        }

        private void LoadIni()
        {
            Utility util = new Utility();

            string strSection = "TTS Server Info";
            m_strServerIP = util.getinivalue(strSection, "server_ip");
            m_strServerPort = util.getinivalue(strSection, "server_port");

            try
            {
                m_nSpeed = int.Parse(util.getinivalue(strSection, "tts_speed"));
                m_fVolume = float.Parse(util.getinivalue(strSection, "tts_volume"));
            }
            catch (Exception)
            {
            }
        }

        public void Speak()
        {
            //if (bAddNew != 1)
            //{
            //	ts.SpeakAsyncCancelAll();
            //}
            //TraceLog("Speak : " + msg);
            ts.SpeakAsync(m_strMsg, PlaybackCount);
            //TraceLog("Finish Speak");
            StopSpeech();

            m_nAddNew--;
        }

        public void SwitchOn()
        {
            try
            {
                mSerial.Start();
            }
            catch (System.SystemException)
            {
            }
        }

        public void SwitchOff()
        {
            try
            {
                if (mSerial != null)
                    mSerial.Stop();
            }
            catch (System.SystemException)
            {
            }
        }

        public void Dispose()
        {
            OnSpeechStarted = null;
            OnSpeechEnded = null;
            OnSpeechPaused = null;
            StopSpeech();

            SwitchOff();

            ts.ClearEngine();
        }

        // 새로운 메시지가 있는지 확인한다.
        public void CheckRequest(WebDBManager dbMgr)
        {
            // get tts server state
            SpeechState state = State;

            // check heartbeat
            m_hbMgr.HeartBeat(dbMgr, (int)state);

            if (m_isProcessing == true)
                return;

            m_isProcessing = true;

            List<BroadcastMessage> messages = m_msgMgr.ReadMessage(dbMgr);

            if (messages != null && messages.Count > 0)
                ProcessRequest(dbMgr, messages);

            m_isProcessing = false;
        }

        private void ProcessRequest(WebDBManager dbMgr, List<BroadcastMessage> messages)
        {
            foreach (BroadcastMessage message in messages)
            {
                if (message.PlayOption == BroadcastMessage.MesageOption.STOP)
                {
                    WaitUntilStop();
                }
                else if (message.PlayOption == BroadcastMessage.MesageOption.PLAY)
                {
                    if (State == SpeechState.STOP)
                        AddSpeech(dbMgr, message);
                    else
                    {
                        if (m_multiMode == MultiMode.STOP_N_NEW_PLAY)
                        {
                            WaitUntilStop();
                            AddSpeech(dbMgr, message);
                        }
                        else if (m_multiMode == MultiMode.WAIT_N_NEW_PLAY)
                        {
                            WaitUntilStop(false);
                            AddSpeech(dbMgr, message);
                        }
                        else// if (m_multiMode == MultiMode.IGNORE_NEW_PLAY)
                        {
                        }
                    }
                }
                else if (message.PlayOption == BroadcastMessage.MesageOption.PAUSE)
                    PauseSpeech();
                else if (message.PlayOption == BroadcastMessage.MesageOption.RESUME)
                    ResumeSpeech();
            }
        }

        private void WaitUntilStop(bool stopNow = true)
        {
            int nSleepTime = 100;
            int nMaxWaitTime = 3000;

            if (stopNow)
            {
                // 즉시 정지는 최대 3초 대기
                StopSpeech();
            }
            else
            {
                // 최대 5분 대기
                nMaxWaitTime = 300000;
            }

            for (int i = 0; i < nMaxWaitTime; i += nSleepTime)
            {
                if (State == SpeechState.STOP)
                    break;

                Thread.Sleep(nSleepTime);
            }
        }

        public void AddSpeech(WebDBManager dbMgr, BroadcastMessage message)
        {
            AddSpeech(message.Message, message.RepeatCount, message.UseSiren);
        }

        private void AddSpeech(string szMsg, int nPlayback, bool bUseSiren)
        {
            if (m_isInitialized == false)
                return;

            if (m_nAddNew > 0)
                return;

            if (mState == SpeechState.PAUSE)
            {
                ResumeSpeech();
            }

            m_nAddNew++;
            if (m_playSiren == true)
            {
                // 사이렌이 울리고 있음은 현재 다른 방송이 실행중임을 의미한다.
                m_nAddNew--;
                return;
                //Thread.Sleep(5000);
            }

            if (mState == SpeechState.PLAY || m_nAddNew > 1)
                StopSpeech();

            mState = SpeechState.PLAY;

            SwitchOn();

            // 스위치 켠후 대기시간을 0.4초에서 1초로 늘린다.
            Thread.Sleep(1000);

            if (bUseSiren == true)
            {
                try
                {
                    m_playSiren = true;
                    //TraceLog("PlaySiren");
                    SoundUtils.PlaySound(m_strSirenFile, false);
                    //TraceLog("FinishSiren");
                }
                catch (System.Exception ex)
                {
                }
            }

            if (nPlayback > 0)
            {
                PlaybackCount = nPlayback;
                PlayCount = 0;
                m_strMsg = "";

                for (int i = 0; i < PlaybackCount; i++)
                {
                    m_strMsg += szMsg;
                    m_strMsg += "\n \n";
                }
                Thread t = new Thread(Speak);
                t.Start();
            }
            else
                m_nAddNew--;

            m_playSiren = false;
        }

        public void StopSpeech()
        {
            try
            {
                if (m_isInitialized == false)
                    return;

                PlaybackCount = 0;
                m_repeat = false;
                if (mState == SpeechState.PAUSE)
                {
                    //ResumeSpeech();

                    mState = SpeechState.PLAY;
                    ts.Resume();
                }
                mState = SpeechState.STOP;

                //TraceLog("StopSpeech");
                ts.SpeakAsyncCancelAll();
            }
            catch (System.Exception)
            {
            }

            SwitchOff();

            Thread.Sleep(100);
        }

        public void PauseSpeech()
        {
            if (m_isInitialized == false)
                return;

            if (mState == SpeechState.STOP)
                return;

            try
            {
                mState = SpeechState.PAUSE;
                ts.Pause();
            }
            catch (System.SystemException)
            {
            }

            SwitchOff();
        }

        public void ResumeSpeech()
        {
            if (m_isInitialized == false)
                return;

            if (mState == SpeechState.STOP)
                return;

            SwitchOn();

            Thread.Sleep(300);
            try
            {
                mState = SpeechState.PLAY;
                ts.Resume();
            }
            catch (System.SystemException)
            {
            }
        }

        // Siren 음원
        public void SetSirenFile(string strFilePath)
        {
            m_strSirenFile = strFilePath;
        }

        public void SetSpeed(int nSpeed)
        {
            m_nSpeed = nSpeed;
        }

        public int GetSpeed()
        {
            return m_nSpeed;
        }

        public void SetVolume(int nVolume)
        {
            m_fVolume = nVolume / 10.0f;
        }

        public int GetVolume()
        {
            return (int)(m_fVolume * 10 + 0.1f);
        }

        private static Process process = null;
        private static void RunStartProcess(string strFileName, string args, string szPath, StreamWriter file)
        {
            try
            {
                using (process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo(strFileName, args);
                    process.StartInfo.WorkingDirectory = szPath;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.ErrorDialog = false;
                    process.StartInfo.RedirectStandardError = false;
                    process.StartInfo.RedirectStandardInput = false;
                    process.StartInfo.RedirectStandardOutput = false;
                    process.StartInfo.UseShellExecute = true;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                    bool bResult = process.Start();
                    file.WriteLine("LAUNCH PROCESS : " + process);
                    file.WriteLine("RESULT LAUNCH : " + bResult.ToString());
                }

            }
            catch (Exception ex)
            {
                file.WriteLine(ex.StackTrace);
            }
        }
    }
}
