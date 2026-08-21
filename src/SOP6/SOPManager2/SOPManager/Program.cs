using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SOPMonitoringSystem;

namespace SOPManager
{
    static class Program
    {
        public static string prgFont = "나눔스퀘어";
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //args = new string[3];
            //args[0] = "2";
            //args[1] = "112";
            //args[2] = "알수없음";

            if (args.Count() < 2)
            {
                //MessageBox.Show("전달된 Parameter가 3개 이하입니다.\r\nTeamID 계정ID 사용자이름 순으로 입력되어야 합니다.");
                MessageBox.Show("시작할 수 없습니다.");
            }
            else
            {
                // 모듈이 들어있는 상대 경로 추가
                /*ModuleManager.Instance.AddRelativePath(".");
                // 하위 경로의 모듈을 등록
                ModuleManager.Instance.RegisterModules();*/

                int nSOPGenUserID = -1;
                string strSOPGenUserID = args[1];
                //string strSOPGenUserRealName = args[2];

                int nTargetMonitor = 1;
                try
                {
                    nSOPGenUserID = int.Parse(args[0]);

                    if (args.Length > 3)
                        nTargetMonitor = int.Parse(args[2]);
                }
                catch (Exception)
                {
                    MessageBox.Show("첫번째 Paramter는 정수 형태이어야 합니다.");
                }

                if (nSOPGenUserID >= 0)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    FormMain mainForm = new FormMain(nSOPGenUserID, strSOPGenUserID/*, strSOPGenUserRealName*/);
                    FormFrame frame = new FormFrame(mainForm, nTargetMonitor);
                    frame.Size = new System.Drawing.Size(1688, 953);
                    Application.Run(frame);
                }
            }
        }
    }
}
