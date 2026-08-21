using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility;

namespace PSensorServer
{
    public class KPXSimulator
    {
        WebDBManager m_dbManager = null;

        public KPXSimulator()
        {
            m_dbManager = new WebDBManager(500);
            m_dbManager.WebServerURL = "http://127.0.0.1:8080/SOP";
            m_dbManager.DatabaseHost = "127.0.0.1";
            m_dbManager.DatabaseType = WebDBManager.DBType.mysql;
            m_dbManager.DatabasePort = "3306";
            m_dbManager.DatabaseName = "KPX";
        }

        public void SetSimulation()
        {
            // stop timer
            
            //  
        }

        private void ReadPipeInfo(DateTime dtTime)
        {
            string szTemp = "";
            
            List<JubixNetwork.PipeSensor> pipeList = KPXAlarmChecker.Instance.PipeList;
            
            foreach(JubixNetwork.PipeSensor sensor in pipeList)
            {

                string szTableName = string.Format("pipehistory_{0}_{1:00}", sensor.PipeID, dtTime.Month);
                string szKeyTimeString = DBUtil.GetKeyTimeString(dtTime);
                string szSQL = string.Format("select Pressure, KeyTime FROM {0} WHERE KeyTime > {1} order by KeyTime ASC limit 1", szTableName, szKeyTimeString);

                ArrayList arResult = m_dbManager.GetResultData(szSQL, 0);
                if( arResult != null && arResult.Count > 0)
                {
                    for(int i = 0 ; i < arResult.Count; i++)
                    {

                    }
                }
            }
        }


        private void ReadTankInfo()
        {
           

        }


        private int m_nLastWorkHistoryID;
        private int m_nLastCmdHistoryID;
        
        public void Simulation()
        {
            
            // set time
            DateTime dtTime = m_dtStartDate;




            // get kpx data
            
            

            // read work history

            // read command history

            // set sensor data
            
            // calc nextTime

            // wait 
        }

        internal void Start()
        {
           
        }

        internal void Stop()
        {
            throw new NotImplementedException();
        }

        private DateTime m_dtStartDate;
        public DateTime StartDate 
        {
            get { return m_dtStartDate; }
            set { m_dtStartDate = value; }
        }

        private int m_nSpeed = 1;
        public int Speed
        {
            get { return m_nSpeed; }
            set { m_nSpeed = value; }
        }

        private bool m_bUseFlow = true;
        public bool UseFlow 
        {
            get { return m_bUseFlow; }
            set { m_bUseFlow = value; }
        }

        private bool m_bUsePressure = true;
        public bool UsePressure
        {
            get { return m_bUsePressure; }
            set { m_bUsePressure = value; }
        }

        private bool m_bUseOptions = true;
        public bool UseOptions
        {
            get { return m_bUseOptions; }
            set { m_bUseOptions = value; }
        }

        private bool m_bUseWork = true;
        public bool UseWork
        {
            get { return m_bUseWork; }
            set { m_bUseWork = value; }
        }

    }
}
