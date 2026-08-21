using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BuildingSMS
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args == null)
            {
                //MessageBox.Show("args is null");
                return;
            }

            //string strLog = "";
            int nArgumentCount = args.Count();

            /*for (int i=0;i<nArgumentCount;i++)
            {
                string str = "arg[" + i.ToString() + "] : " + args[i];

                if (i == 0)
                    strLog = str;
                else
                    strLog += "\r\n" + str;
            }*/

            if (nArgumentCount < 2)
                return;

            string strLocation = args[0].Trim();
            string strMessage = args[1].Trim();

            if (strLocation.StartsWith("'"))
                strLocation = strLocation.Substring(1);

            if (strLocation.EndsWith("'"))
                strLocation = strLocation.Substring(0, strLocation.Length - 1);

            Building building = ZoneManager.GetBuilding(strLocation);

            if (building == null)
                return;

            int nFloorIndex;

            if (GetFloorIndex(strLocation, out nFloorIndex) == false)
                return;

            string strTag = "{location}";
            string strLower = strMessage.ToLower();

            int nIndex = strLower.IndexOf(strTag);

            while (nIndex >= 0)
            {
                if (nIndex == 0)
                    strMessage = strLocation + strMessage.Substring(strTag.Length);
                else
                    strMessage = strMessage.Substring(0, nIndex) + strLocation + strMessage.Substring(nIndex + strTag.Length);

                strLower = strMessage.ToLower();
                nIndex = strLower.IndexOf(strTag);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new FormMain());
            Application.Run(new FormMessage(building, nFloorIndex, strMessage));
        }

        private static bool GetFloorIndex(string strLocation, out int nFloorIndex)
        {
            nFloorIndex = 0;
            bool underground = false;

            if (strLocation.Contains("지하"))
                underground = true;

            int nIndex = strLocation.LastIndexOf("층");

            if (nIndex < 0)
                return false;

            int num = 0;
            int nTimes = 1;

            for (int i = nIndex - 1; i >= 0; i--)
            {
                char ch = strLocation.ElementAt(i);

                if (ch >= '0' && ch <= '9')
                {
                    num += nTimes * (int)(ch - '0');
                    nTimes *= 10;
                }
                else
                    break;
            }

            if (underground)
                nFloorIndex = -num;
            else
                nFloorIndex = num - 1;

            return true;
        }
    }
}
