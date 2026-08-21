using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Timers;
using System.Reflection;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;

namespace TTSServerDotNetCmd
{
	class Program
	{
        #region unmanaged
        // Declare the SetConsoleCtrlHandler function
        // as external and receiving a delegate. 
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);
        
        // A delegate type to be used as the handler routine 
        // for SetConsoleCtrlHandler.
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        // An enumerated type for the control messages
        // sent to the handler routine.
        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }
        #endregion

		private static TTSManager mTtsManager = null;
		private static DBManager mDBManager = null;
		private static bool bProcess = false;
        private static System.Timers.Timer tmrTimersTimer = new System.Timers.Timer();
		private static bool bExit = false;
        private static bool bFirstRun = true;
        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            switch (ctrlType)
            {
                case CtrlTypes.CTRL_C_EVENT:
                case CtrlTypes.CTRL_BREAK_EVENT:
                case CtrlTypes.CTRL_CLOSE_EVENT:
                case CtrlTypes.CTRL_LOGOFF_EVENT:
                case CtrlTypes.CTRL_SHUTDOWN_EVENT:
                    tmrTimersTimer.Stop();                    
                    if (mTtsManager != null)
                        mTtsManager.Dispose();
                    bExit = true;
                    break;
            }
            return true;
        }
        
		static void Main(string[] args)
		{
            SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);

			//mDBManager = new DBManager();
			

			string szPath = Assembly.GetEntryAssembly().Location;
			string szFullPath = Directory.GetParent(szPath).FullName;

			System.IO.StreamWriter file = new System.IO.StreamWriter(szFullPath + "//server.log");


			mDBManager = new DBManager();
			string szRunModule = mDBManager.Ini.getinivalue("TTS Server Info", "run_module");
			file.WriteLine("RUN MODULE : " + szRunModule);
			if (szRunModule.Equals("1"))
			{

				string szModulePath = mDBManager.Ini.getinivalue("TTS Server Info", "module_path");
				szModulePath = szModulePath.Replace("\\", "/");
				string strPath = szModulePath + "/ttsserver.exe";

				file.WriteLine("MODULE PATH: " + strPath);

				//bool bRunning = RunCheckProcess("ttsserver");
				//file.WriteLine("TTSSErver Running : " + bRunning.ToString());

				//if (!bRunning)
				RunStartProcess(strPath, "", szModulePath, file);
			}

			file.Close();

			mTtsManager = TTSManager.Instance;
			tmrTimersTimer.Interval = 2000;
			tmrTimersTimer.Elapsed += new ElapsedEventHandler(tmrTimersTimer_Elapsed);
			tmrTimersTimer.Start();

			while (bExit == false)
			{                
				Thread.Sleep(1000);
			}           
		}
		private static System.Diagnostics.Process process = null;
		private static void RunStartProcess(string strFileName, string args,string szPath, System.IO.StreamWriter file)
		{

			try
			{
				using (process = new System.Diagnostics.Process())
				{

					process.StartInfo = new System.Diagnostics.ProcessStartInfo(strFileName, args);
					process.StartInfo.WorkingDirectory = szPath;
					process.StartInfo.CreateNoWindow = true;
					process.StartInfo.ErrorDialog = false;
					process.StartInfo.RedirectStandardError = false;
					process.StartInfo.RedirectStandardInput = false;
					process.StartInfo.RedirectStandardOutput = false;
					process.StartInfo.UseShellExecute = true;
					process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
					bool bResult = process.Start();
					file.WriteLine("LAUNCH PROCESS : " + process);
					file.WriteLine("RESULT LAUNCH : " + bResult.ToString());
					//process.WaitForExit();
					//file.WriteLine("EXIT CODE : " + process.ExitCode);
				}

			}
			catch (Exception ex)
			{
				file.WriteLine(ex.StackTrace);
				//MessageBox.Show(ex.Message);
			}
		}

		static void tmrTimersTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			// get tts server state
			SpeechState state = mTtsManager.State;

			int nState = (int)state;

			// check heartbeat
			mDBManager.HeartBeat(nState);

			if (bProcess == true || bExit == true)
				return;

			bProcess = true;
			if (mDBManager.LastMessege == null)
				// check db message
				mDBManager.ReadMessage();

			// get db last message
			if (mDBManager.RecvMsg == true)
			{
				mDBManager.RecvMsg = false;
				mDBManager.ClearMessage();

				BroadcastMessage msg = mDBManager.LastMessege;
				if (bFirstRun == true)
				{
					msg = null;
					mDBManager.LastMessege = null;
				}

				if (msg != null)
				{

					string szMssage = msg.Message;

					if (mDBManager.Mode == 1)
					{
						szMssage = mDBManager.TMsg + msg.Message;
					}
					switch (msg.PlayOption)
					{
						//0(방송 끝), 1(방송 실행), 2(방송 중지), 3(일시 정지)
						case 0:
							mTtsManager.StopSpeech();
							break;
						case 1:
							// 사이렌일경우 울리기전에 먼저 상태 정보를 갱신해준다.(사이렌동안업데이트가 안됨)
							if (msg.UseSiren == true)
							{
								mDBManager.HeartBeat(2);
							}
							mTtsManager.AddSpeech(szMssage, msg.RepeatCount, msg.UseSiren);
							break;
						case 2:// resume
							mTtsManager.ResumeSpeech();
							break;
						case 3: // pause
							mTtsManager.PauseSpeech();
							break;
					}
					mDBManager.LastMessege = null;
				}
			}
			bProcess = false;
			bFirstRun = false;

		} // method            
	} // class
} // namespace
