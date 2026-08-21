using DBUtility2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOPMonitoringSystem.Data
{
    public class OptionManager
    {
        public void ReadUsageStatus()
        {
            string[] types = new string[] { "'UseFire'", "'UsePSM'", "'UseIntrusion'", "'UseEarthquake'", "'UseFirewall'", "'UseDoor'", "'UseBlackout'", "'UseStrongWind'", "'UseTerror'", "'UseSubmergency'", "'UseCorona'" };

            // 정전, 강풍 사용하는 센서 종류 읽어오기
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT PropertyName, PropertyValue ");
            sb.Append("  FROM OptionSOPSimulator ");
            sb.AppendFormat("WHERE PropertyName IN ({0})", string.Join(", ", types));
            sb.AppendFormat("  AND SiteID = {0}", UnE.SOP.ProxySOP.Instance.SiteID);

            string strSQL = sb.ToString();
            ArrayList arrResult = FormSOP.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                string strSensorName = WebDBManager.GetStringField(arrResult[i]);
                VariousData<int> useFlag = WebDBManager.GetIntField(arrResult[i + 1].ToString());

                if (strSensorName == "UsePSM")
                {
                    if (useFlag.Data == 1)
                        UnE.SOP.ProxySOP.Instance.UsePSM = true;
                    else
                        UnE.SOP.ProxySOP.Instance.UsePSM = false;
                }

                if (strSensorName == "UseIntrusion")
                {
                    if (useFlag.Data == 1)
                        UnE.SOP.ProxySOP.Instance.UseIntrusion = true;
                    else
                        UnE.SOP.ProxySOP.Instance.UseIntrusion = false;
                }

                if (strSensorName == "UseEarthquake")
                {
                    if (useFlag.Data == 1)
                        UnE.SOP.ProxySOP.Instance.UseEarthquake = true;
                    else
                        UnE.SOP.ProxySOP.Instance.UseEarthquake = false;
                }

                if (strSensorName == "UseFirewall")
                {
                    if (useFlag.Data == 1)
                        UnE.SOP.ProxySOP.Instance.UseFirewall = true;
                    else
                        UnE.SOP.ProxySOP.Instance.UseFirewall = false;
                }

                if (strSensorName == "UseDoor")
                {
                    if (useFlag.Data == 1)
                        UnE.SOP.ProxySOP.Instance.UseDoor = true;
                    else
                        UnE.SOP.ProxySOP.Instance.UseDoor = false;
                }

                if (strSensorName == "UseBlackout")
                {
                    if (useFlag.Data == 1)
                        UnE.SOP.ProxySOP.Instance.UseBlackout = true;
                    else
                        UnE.SOP.ProxySOP.Instance.UseBlackout = false;
                }

                if (strSensorName == "UseStrongWind")
                {
                    if (useFlag.Data == 1)
                        UnE.SOP.ProxySOP.Instance.UseStrongWind = true;
                    else
                        UnE.SOP.ProxySOP.Instance.UseStrongWind = false;
                }

                if (strSensorName == "UseTerror")
                {
                    if (useFlag.Data == 1)
                        UnE.SOP.ProxySOP.Instance.UseTerror = true;
                    else
                        UnE.SOP.ProxySOP.Instance.UseTerror = false;
                }

                if (strSensorName == "UseSubmergency")
                {
                    if (useFlag.Data == 1)
                        UnE.SOP.ProxySOP.Instance.UseSubmergency = true;
                    else
                        UnE.SOP.ProxySOP.Instance.UseSubmergency = false;
                }

                if (strSensorName == "UseCorona")
                {
                    if (useFlag.Data == 1)
                        UnE.SOP.ProxySOP.Instance.UseCorona = true;
                    else
                        UnE.SOP.ProxySOP.Instance.UseCorona = false;
                }
            }
        }
    }
}
