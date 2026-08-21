
using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.IO;



namespace libTTS
{
    internal delegate void SpeechStartEvent(object sender, SpeechEventArgs e);
    internal delegate void SpeechEndEvent(object sender, SpeechEventArgs e);
    internal delegate void SpeechPauseEvent(object sender, SpeechEventArgs e);

	internal enum SpeechState
	{
		STANDBY = 1,
		PLAY = 2,
		STOP = 3,
		PAUSE = 4,
		REPEAT = 5
	}

    internal class SpeechEventArgs
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

    sealed class LocalTTS : IDisposable
	{		
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

		private CoreTTSDotNet.CoreTTS ts = new CoreTTSDotNet.CoreTTS();


		private string m_strServerIP = "127.0.0.1";
		private string m_strServerPort = "20030";
		private int m_nSpeed = 100;
		private float m_fVolume = 1.0f;
		

		public LocalTTS()
        {

        }

        public void ConnectServer(string szServerIP, string port)
        {
            try
            {
                if (bInit == true)
                    return;

                m_strServerIP = szServerIP;
                m_strServerPort = port;            

                ts.PlayCallBack += new CoreTTSDotNet.SpeakPlayCallBack(BeginPlayTTS);

                int nPort = int.Parse(port);

                ts.Create();
                ts.Volume = m_fVolume;
                ts.Speed = m_nSpeed;

                ts.Config(szServerIP, nPort);

                mState = SpeechState.STOP;

                SwitchOff();
                bInit = true;
            }
            catch (Exception)
            {
                //MessageBox.Show("TTS 생성이 실패하였습니다.\n방송 메세지 전파을 사용할 수 없습니다.");
            }
        }

		private bool m_bRequestPlay = false;
		public bool RequestPlay
		{
			get { return m_bRequestPlay; }
			set { m_bRequestPlay = value; }
		}
		private bool m_bIsPlay = false;
		public bool IsPlay
		{
			get { return m_bIsPlay; }
			set { m_bIsPlay = value; }
		}

		public void BeginPlayTTS()
		{
		

			m_bRequestPlay = false;
			m_bIsPlay = true;

			//string szPath = Assembly.GetEntryAssembly().Location;
			//string szFullPath = Directory.GetParent(szPath).FullName;
			//System.IO.StreamWriter file = new System.IO.StreamWriter(szFullPath + "//callback.log",true);
			//using (file)
			//{
			//    file.WriteLine("Callback");
			//}
			//file.Close();
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
			m_bIsPlay = false;
			m_bRequestPlay = false;

			if (bInit == false)
				return;

			if (mState == SpeechState.PAUSE)
			{
				ResumeSpeech();
			}
			
			bAddNew++;
			if (bPlaySiren == true)
			{
				Thread.Sleep(5000);
			}

			if (mState == SpeechState.PLAY || bAddNew > 1)
			{
				StopSpeech();
			}			
			SwitchOn();

			Thread.Sleep(400);

            string szPath = Assembly.GetEntryAssembly().Location;
            string szFullPath = Directory.GetParent(szPath).FullName;
       
			if (bUseSiren == true)
			{
				try
				{
					bPlaySiren = true;
                              
                    string szFileName = szFullPath + "\\66084^air-raid-siren-alert.mp3";
                    szFileName = szFileName.Replace("\\", "/");
                   
                    SoundUtils.PlaySound(szFileName, false);
				}
				catch (System.Exception)
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
				//mState = SpeechState.PLAY;

				m_bRequestPlay = true;
				//Thread t = new Thread(Speak);
				//t.Start();

                Speak();
			}
			
			bPlaySiren = false;

		}

		public void Speak()
		{
			mState = SpeechState.PLAY;

			ts.SpeakAsync(msg, PlaybackCount);			
			
			StopSpeech();

			bAddNew--;
		}

		public void SwitchOn()
		{
		}

		public void SwitchOff()
		{			
		}

		//////////////////////////////////////////////////////////////////////////
		public void StopSpeech()
		{
			m_bRequestPlay = false;
			m_bIsPlay = false;
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
			m_bIsPlay = false;

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
			m_bIsPlay = true;

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


