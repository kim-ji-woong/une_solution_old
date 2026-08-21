using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Timers;
using System.Reflection;
using System.IO;

namespace BroadcastServer
{
    public partial class Broadcast : ServiceBase
    {
        private static TTSManager mTtsManager = null;
        private static DBManager mDBManager = null;
        private static bool bProcess = false;

        private static bool bFirstRun = true;
		private bool bExit = false;

        private int m_nSiteID = 1;

        private System.Timers.Timer tmrTimer = null;
		private System.Diagnostics.Process process = null;
        public Broadcast()
        {
            InitializeComponent();     
        }

        protected override void OnStart(string[] args)
        {
			string szPath = Assembly.GetEntryAssembly().Location;
			string szFullPath = Directory.GetParent(szPath).FullName;

			//System.IO.StreamWriter file = new System.IO.StreamWriter(szFullPath + "//server.log");


			mDBManager = new DBManager();

            m_nSiteID = mDBManager.SiteID;
                                    
            string szRunModule = mDBManager.Ini.getinivalue("TTS Server Info", "run_module");
            
			//file.WriteLine("RUN MODULE : " + szRunModule);
			if (szRunModule.Equals("1"))
			{

				string szModulePath = mDBManager.Ini.getinivalue("TTS Server Info", "module_path");
				szModulePath = szModulePath.Replace("\\", "/");
				string strPath = szModulePath + "/ttsserver.exe";
				//file.WriteLine("MODULE PATH: " + strPath);

				bool bRunning = RunCheckProcess("ttsserver");
				//file.WriteLine("TTSSErver Running : " + bRunning.ToString());

				if (!bRunning)
					RunStartProcess(strPath, "", szModulePath);//, file);
			}

			//file.Close();
			mTtsManager = TTSManager.Instance;


			tmrTimer = new System.Timers.Timer();
			tmrTimer.Interval = 2000;
			tmrTimer.Elapsed += tmrTimer_Elapsed;
			tmrTimer.Start();
           
        }


		protected virtual void OnSessionChange(SessionChangeDescription changeDescription)
		{
			
			
		}

		private void RunStartProcess(string strFileName, string args, string szPath)//, System.IO.StreamWriter file)
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
					//file.WriteLine("LAUNCH PROCESS : " + process);
					//file.WriteLine("RESULT LAUNCH : " + bResult.ToString());
					//process.WaitForExit();
					//file.WriteLine("EXIT CODE : " + process.ExitCode);
				}

			}
			catch (Exception)
			{
				//file.WriteLine(ex.StackTrace);
				//MessageBox.Show(ex.Message);
			}
		}

		private void AbortProcess(string strProcessName)
		{
			System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcesses();

			foreach (System.Diagnostics.Process process in processList)
			{
				if (process.ProcessName == (strProcessName))
				{
					process.Kill();
				}
			}
		}

        //strProcessName을 가진 프로그램이 실행중인지 체크
        private bool RunCheckProcess(string strProcessName)
        {
            System.Diagnostics.Process[] processList = System.Diagnostics.Process.GetProcesses();

            foreach (System.Diagnostics.Process process in processList)
            {
                if (process.ProcessName == (strProcessName))
                    return true;
            }
            return false;
        }

        protected override void OnStop()
        {
			bExit = true;
            if (tmrTimer != null)
            {
                tmrTimer.Elapsed -= tmrTimer_Elapsed;
                tmrTimer.Stop();
            }
            mTtsManager.Dispose();
            mDBManager = null;

			AbortProcess("ttsserver");
        }

		void tmrTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
            try
            {
                // get tts server state
                SpeechState state = mTtsManager.State;

                int nState = (int)state;

                // check heartbeat
                mDBManager.HeartBeat(nState);

                if (bProcess == true || bExit == true)
                    return;

                //if (mTtsManager.IsPlay == false && mTtsManager.RequestPlay == true)
                //{
                //	return;
                //}

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
                                //Thread.Sleep(1000);
                                mTtsManager.StopSpeech();
                                break;
                            case 1:
                                // 사이렌일경우 울리기전에 먼저 상태 정보를 갱신해준다.(사이렌동안업데이트가 안됨)
                                //if (msg.UseSiren == true)
                                //{
                                //	mDBManager.HeartBeat(2);
                                //}
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
            }
            catch (System.Exception)
            {
            	
            }		

		} // method      
	}
}
