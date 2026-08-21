using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.ServiceProcess;
using System.Text;

namespace MessageServer
{
	static class Program
	{
		private static log4net.ILog logger = null;

#if WIN
		///<summary>
		///해당 응용 프로그램의 주 진입점입니다.
		///</summary>
		static void Main()
		{
			try
			{
				log4net.Config.DOMConfigurator.Configure();
			}
			catch (System.Exception)
			{
			}

			logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
			MessageService.Logger = logger;
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new FormMain());
		}
#else
		/// <summary>
		/// 해당 응용 프로그램의 주 진입점입니다.
		/// </summary>
		static void Main()
		{
			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[] 
			{ 
				new MessageService()
			};
			ServiceBase.Run(ServicesToRun);
		}		
#endif
	}
}


