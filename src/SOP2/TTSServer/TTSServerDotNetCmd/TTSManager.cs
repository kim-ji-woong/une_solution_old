
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



namespace TTSServerDotNetCmd
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
		private SerialManager mSerial = null;

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

		public event SpeechStartEvent OnSpeechStarted;
		public event SpeechEndEvent OnSpeechEnded;
		public event SpeechPauseEvent OnSpeechPaused;

		private bool bInit = false;
		private bool bRepeat = false;
		private string szMessage = "";
		private string szNOP = ",,,,,,";

		private Utility m_ini = new Utility();

		private CoreTTSDotNet.CoreTTS ts = new CoreTTSDotNet.CoreTTS();


		private string m_strServerIP = "127.0.0.1";
		private string m_strServerPort = "20030";
		private int m_nSpeed = 100;
		private float m_fVolume = 1.0f;
		private void LoadIni()
		{
			string strSection = "TTS Server Info";
			m_strServerIP = m_ini.getinivalue(strSection, "server_ip");
			m_strServerPort = m_ini.getinivalue(strSection, "server_port");

			try
			{
				m_nSpeed = int.Parse(m_ini.getinivalue(strSection, "tts_speed"));
				m_fVolume = float.Parse(m_ini.getinivalue(strSection, "tts_volume"));
			}
			catch (Exception)
			{
			}
		}
		private TTSManager()
		{
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
				bInit = true;
                //TraceLog("TTSManager initialize success");
			}
			catch (Exception)
			{
				//MessageBox.Show("TTS 생성이 실패하였습니다.\n방송 메세지 전파을 사용할 수 없습니다.");
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

		private string msg = "";
		static bool bPlaySiren = false;
		static int bAddNew = 0;
		public void AddSpeech(string szMsg, int nPlayback, bool bUseSiren)
		{
			if (bInit == false)
				return;

            if (bAddNew > 0)
                return;

			if (mState == SpeechState.PAUSE)
			{
				ResumeSpeech();
			}


			bAddNew++;
			if (bPlaySiren == true)
			{
                // 사이렌이 울리고 있음은 현재 다른 방송이 실행중임을 의미한다.
                bAddNew--;
                return;
				//Thread.Sleep(5000);
			}

			if (mState == SpeechState.PLAY || bAddNew > 1)
				StopSpeech();

			mState = SpeechState.PLAY;

			SwitchOn();

            // 스위치 켠후 대기시간을 0.4초에서 1초로 늘린다.
			Thread.Sleep(1000);

			if (bUseSiren == true)
			{
				try
				{
					bPlaySiren = true;
					string szFileName = Application.StartupPath + "\\" + "66084^air-raid-siren-alert.wav";
                    //TraceLog("PlaySiren");
					SoundUtils.PlaySound(szFileName, false);
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
                msg = "";
                for (int i = 0; i < PlaybackCount; i++)
                {
                    msg += szMsg;
                    msg += "\n \n";
                }
                Thread t = new Thread(Speak);
                t.Start();
            }
            else
                bAddNew--;

			bPlaySiren = false;

		}

        /*private System.IO.StreamWriter m_writer = null;

        private void TraceLog(string strLog)
        {
            if (m_writer == null)
                m_writer = new System.IO.StreamWriter("D:\\DxfRoot\\Update\\tts.log", false, System.Text.Encoding.UTF8);

            DateTime dtNow = DateTime.Now;
            string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);
            m_writer.WriteLine(strTime + ", " + strLog);
            m_writer.Flush();
            //System.Diagnostics.Trace.WriteLine(strTime + ", " + strLog);
        }*/

		public void Speak()
		{
			//if (bAddNew != 1)
			//{
			//	ts.SpeakAsyncCancelAll();
			//}
            //TraceLog("Speak : " + msg);
			ts.SpeakAsync(msg, PlaybackCount);
            //TraceLog("Finish Speak");
			StopSpeech();

			bAddNew--;
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

		//////////////////////////////////////////////////////////////////////////
		public void StopSpeech()
		{
			try
			{
				if (bInit == false)
					return;
				PlaybackCount = 0;
				bRepeat = false;
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

		//////////////////////////////////////////////////////////////////////////
		public void PauseSpeech()
		{
			if (bInit == false)
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

		//////////////////////////////////////////////////////////////////////////
		public void ResumeSpeech()
		{
			if (bInit == false)
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
	}

}


