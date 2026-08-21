using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace SOPMonitoringSystem.Process
{
    public class SMSManagerEx : UnE.SOP.SMS.SMSManager
    {
        public static void SetManager(bool isSimulationMode)
        {
            if (isSimulationMode)
                m_instance = new SMSManagerEx();
            else
                m_instance = new UnE.SOP.SMS.SMSManager();

            m_instance.WebServerURL = FormSOP.Instance.DBManager.WebServerURL;
        }

        /*protected override ArrayList GetSimulationPhoneNumbers()
        {
            if (SDMS.FormManager_Simulation.ManagerPhoneNumbers == null)
                return null;

            ArrayList arrPhoneNumbers = new ArrayList();

            foreach (KeyValuePair<string, string> pair in SDMS.FormManager_Simulation.ManagerPhoneNumbers)
            {
                arrPhoneNumbers.Add(pair.Value);
            }

            return arrPhoneNumbers;
        }*/
    }
}
