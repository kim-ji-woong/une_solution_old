using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using SpeechLib;
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

    class TTSManager : ITTSManager
    {
        private HeartBeatManager m_hbMgr = new HeartBeatManager();
        private MessageManager m_msgMgr = new MessageManager();

        private bool m_isProcessing = false;

        private SpVoice m_voice = new SpVoice();
        private bool m_isPaused = false;
        private BroadcastMessage m_currentMessage = null;
        private bool m_soundOn = false;

        // 속도 : -10 ~ 10
        private int m_nSpeed = 0;
        // 음량 : 0 ~ 100
        private int m_nVolume = 100;

        private string m_strSirenFile = "66084^air-raid-siren-alert.mp3";
        //private string m_strSirenFile = "66084^air-raid-siren-alert.wav";

        private MultiMode m_multiMode = MultiMode.STOP_N_NEW_PLAY;

        public SpeechState State
        {
            get
            {
                if (m_isPaused)
                    return SpeechState.PAUSE;

                if (m_voice.Status.RunningState == SpeechRunState.SRSEIsSpeaking)
                    return SpeechState.PLAY;
                else if (m_voice.Status.RunningState == SpeechRunState.SRSEDone)
                    return SpeechState.STOP;

                return SpeechState.STANDBY;
            }
        }

        public MultiMode MultiMode
        {
            get { return m_multiMode; }
            set { m_multiMode = value; }
        }

        public void Initialize()
        {

        }

        // 새로운 메시지가 있는지 확인한다.
        public void CheckRequest(WebDBManager dbMgr)
        {
            // get tts server state
            SpeechState state = State;

            if (m_soundOn && state != SpeechState.PLAY)
            {
                SystemSound(false);
            }

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
            m_nVolume = nVolume;
        }

        public int GetVolume()
        {
            return m_nVolume;
        }

        private void SystemSound(bool on)
        {
            NAudio.CoreAudioApi.MMDeviceEnumerator devEnum = new NAudio.CoreAudioApi.MMDeviceEnumerator();
            NAudio.CoreAudioApi.MMDevice defaultDevice = devEnum.GetDefaultAudioEndpoint(NAudio.CoreAudioApi.DataFlow.Render, NAudio.CoreAudioApi.Role.Multimedia);
            //defaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar = 0.20f;
            defaultDevice.AudioEndpointVolume.Mute = !on;

            if (on == false)
                m_soundOn = false;
        }

        public void AddSpeech(WebDBManager dbMgr, BroadcastMessage message)
        {
            m_isPaused = false;
            m_currentMessage = message;
            SystemSound(true);

            // 사이렌일경우 울리기전에 먼저 상태 정보를 갱신해준다.(사이렌동안 업데이트가 안됨)
            if (message.UseSiren == true)
            {
                if (dbMgr != null)
                    m_hbMgr.HeartBeat(dbMgr, (int)SpeechState.PLAY);

                SoundUtils.PlaySound(m_strSirenFile, false);
            }

            if (message.RepeatCount < 0 || message.Message.Length == 0)
            {
                SystemSound(false);
                return;
            }

            m_soundOn = true;
            string strMessage = "";

            int nRepeatCount = message.RepeatCount;

            if (nRepeatCount <= 0)
                nRepeatCount = 1;

            for (int i=0;i<nRepeatCount;i++)
            {
                strMessage += message.Message;
                strMessage += "\n \n";
            }

            try
            {
                m_voice.Rate = m_nSpeed;
                m_voice.Volume = m_nVolume;
                m_voice.Speak(strMessage, SpeechVoiceSpeakFlags.SVSFlagsAsync);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("AddSpeech Error : " + e.Message);
                m_voice = new SpVoice();
            }

            m_currentMessage = null;
        }

        public void StopSpeech()
        {
            try
            {
                if (m_voice.Status.RunningState != SpeechRunState.SRSEDone)
                    m_voice.Skip("sentence", 1000000);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("StopSpeech Error : " + e.Message);
                m_voice = new SpVoice();
            }

            BroadcastMessage currentMessage = m_currentMessage;

            // 사이렌이 울리는 동안은 방송 정지를 시킬수가 없다.
            // CurrentMessage의 RepeatCount를 0으로 두어 사이렌이 울린후 방송이 나가지 않도록 한다.
            if (currentMessage != null)
                currentMessage.RepeatCount = 0;

            m_isPaused = false;
        }

        public void PauseSpeech()
        {
            try
            {
                if (m_voice.Status.RunningState != SpeechRunState.SRSEDone)
                {
                    m_voice.Pause();
                    m_isPaused = true;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("PauseSpeech Error : " + e.Message);
                m_voice = new SpVoice();
            }
        }

        public void ResumeSpeech()
        {
            try
            {
                m_isPaused = false;
                m_voice.Resume();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("ResumeSpeech Error : " + e.Message);
                m_voice = new SpVoice();
            }
        }
    }
}
