using System;
using System.Linq;
using System.Windows.Forms;

namespace SDMS
{
	internal static class Program
	{
		private static log4net.ILog logger = null;

		/// <summary>
		/// 해당 응용 프로그램의 주 진입점입니다.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			try
			{
				log4net.Config.XmlConfigurator.Configure();
			}
			catch (System.Exception)
			{
			}

			ModuleManager.Instance.AddRelativePath(".");
			ModuleManager.Instance.RegisterModules();

			if (args.Count() < 2)
			{
				MessageBox.Show("시작할 수 없습니다.");
			}
			else
			{
				int nMonitor = 0;
				int nSOPGenUserID = -1;
				string strSOPGenUserRealName = args[1];
				if (args.Length >= 3)
				{
					int.TryParse(args[2], out nMonitor);
				}

				bool isSimulationMode = false;

				if (args.Length >= 4)
				{
					int nSimulationMode;

					if (int.TryParse(args[3], out nSimulationMode))
						isSimulationMode = nSimulationMode != 0;
				}

				try
				{
					nSOPGenUserID = int.Parse(args[0]);
				}
				catch (Exception)
				{
					MessageBox.Show("첫번째 Paramter는 정수 형태이어야 합니다.");
				}

				if (nSOPGenUserID >= 0)
				{
					logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
					AppDomain currentDomain = AppDomain.CurrentDomain;
					currentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionHandler);

					Application.EnableVisualStyles();
					Application.SetCompatibleTextRenderingDefault(false);
					Application.Run(new FormFrame(new FormMain(nSOPGenUserID, strSOPGenUserRealName, nMonitor, isSimulationMode)));
				}
			}
		}

		private static void ExceptionHandler(object sender, UnhandledExceptionEventArgs args)
		{
			Exception ex = (Exception)args.ExceptionObject;

			System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
			logger.Debug("프로그램 오류", ex);
			logger.Debug("Line: " + trace.GetFrame(0).GetFileLineNumber());
		}
	}
}