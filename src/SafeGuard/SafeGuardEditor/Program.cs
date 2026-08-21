using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SOPMonitoringSystem;

namespace SOPManager
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {           
			// 모듈이 들어있는 상대 경로 추가
			ModuleManager.Instance.AddRelativePath(".");
			// 하위 경로의 모듈을 등록
			ModuleManager.Instance.RegisterModules();

            int nSOPGenUserID = 1;
            string strSOPGenUserID = "";
            string strSOPGenUserRealName = "";
                
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
			FormMain mainForm = new FormMain(nSOPGenUserID, strSOPGenUserID, strSOPGenUserRealName);
			FormFrame frame = new FormFrame(mainForm, 1);
			frame.Size = new System.Drawing.Size(1688, 953);
			Application.Run(frame);
   
        }
    }
}
