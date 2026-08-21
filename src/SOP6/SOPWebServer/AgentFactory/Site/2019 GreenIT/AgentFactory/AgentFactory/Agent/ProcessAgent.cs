using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Collections;
using DBUtility2;
using System.Diagnostics;
using System.IO;

namespace AgentFactory.Agent
{
    internal class ProcessAgent : BaseProcessAgent
    {
        private Utility m_util = new Utility();

        public override List<ClientMessage> PostNewAlarm(DirectDBManager dbMgr, AlarmData alarm, IAlarmManager alarmManager)
        {
            RunProcess("alarm_on");
            return base.PostNewAlarm(dbMgr, alarm, alarmManager);
        }

        public override List<ClientMessage> PostClearAlarm(DirectDBManager dbMgr, AlarmData alarm, IAlarmManager alarmManager)
        {
            RunProcess("alarm_off");
            return base.PostClearAlarm(dbMgr, alarm, alarmManager);
        }

        private string GetFilePath()
        {
            return m_util.getinivalue("GreenIT", "file_path");
        }

        private void RunProcess(string strKey)
        {
            string strParameter = m_util.getinivalue("GreenIT", strKey);

            if (strParameter.Length == 0)
                return;

            string strFilePath = GetFilePath();

            if (strFilePath.Length == 0)
                return;

            if (File.Exists(strFilePath) == false)
                return;

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = strFilePath;
            startInfo.ErrorDialog = true;
            startInfo.Arguments = strParameter;

            Process process;
            try
            {
                process = System.Diagnostics.Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }
    }
}
