using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace MonitoringForestFire.WS
{
    /// <summary>
    /// MonitoringForestFireWS의 요약 설명입니다.
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // ASP.NET AJAX를 사용하여 스크립트에서 이 웹 서비스를 호출하려면 다음 줄의 주석 처리를 제거합니다. 
    [System.Web.Script.Services.ScriptService]
    public class MonitoringForestFireWS : System.Web.Services.WebService
    {
        [WebMethod(Description = "Start Action")]
        public string StartAction(int nJobID, int nActionID, DateTime dtActTime, string strDescription)
        {
            // true : Success.
            // flase : Fail.

            // DB Call..

            return String.Format("Start Action... {4}EventID = {0}{4}ActionID = {1}{4}Act Time = {2}{4}Description = {3}",
                nJobID, nActionID, dtActTime, strDescription, Environment.NewLine);

            //return true;
        }

        [WebMethod(Description = "End Action")]
        public bool EndAction(int nJobID, int nActionID, DateTime dtActTime, string strDescription)
        {
            // true : Success.
            // flase : Fail.

            // DB Call..

            return true;
        }

    }
}
