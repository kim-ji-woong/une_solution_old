using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CheckSensorResult
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string strEvtID, strEquipCode, strEquipStatus;
            int isReal;

            if (WebServiceManager.GetParameter(args, out strEvtID, out isReal, out strEquipCode, out strEquipStatus))
            {
                // SOPWebServer에 전송
                WebServiceManager.SendSOPWebAPI(strEvtID, isReal);
                // 플럭시티에 전송
                WebServiceManager.SendPluxity(strEvtID, isReal, strEquipCode, strEquipStatus);
            }
        }
    }
}
