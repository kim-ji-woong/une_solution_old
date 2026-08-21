using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DBUtility2;

namespace GasLevelServer
{
    public class LevelMeterManager
    {
        private int m_nSiteID = 1;
        private WebDBManager mDBMgr = null;
        private Thread m_LevelCheckThread = null;

        private GasDetector.LevelMeterManager dm = new GasDetector.LevelMeterManager();
        public GasDetector.LevelMeterManager Detector
        {
            get { return dm; }
        }

        private static LevelMeterManager m_instance = null;
        public static LevelMeterManager Instance
        {
            get { return m_instance; }
        }

        private NetworkClient m_mgr = null;
        public LevelMeterManager(NetworkClient client)
        {
            m_nSiteID = LevelMeterNetworkServer.Instance.SiteID;
            mDBMgr = new WebDBManager(m_nSiteID);         
            m_mgr = client;
            m_instance = this;
        }
              

        private Action<int, int, float, int, int> mNotifyAction = null;
        public void BeginServer(Action<int, int, float, int, int> onNotify)
        {
            try
            {
                mNotifyAction = onNotify;
                dm.OnNotifyAlarm += GasLevelMeter_OnNotifyAlarm;
                dm.Start();

            }
            catch(Exception)
            { }

            LoadPSMLevelMeter();

            m_LevelCheckThread = new Thread(CheckValue);
            m_LevelCheckThread.Name = "Level value Check";
            m_LevelCheckThread.Start();
        }

        private void GasLevelMeter_OnNotifyAlarm(int nComm, int nAlarmUnit, float fValue, int nChannel, int nStatus)
        {
            if(mNotifyAction != null)
            {
                mNotifyAction.Invoke(nComm, nAlarmUnit, fValue, nChannel, nStatus);
            }           
        }

        public void StopServer()
        {
            try
            {
                m_bReleaseThread = true;
                if (m_LevelCheckThread != null)
                    m_LevelCheckThread.Join(2000);
            }
            catch(Exception)
            {}            

            dm.OnNotifyAlarm -= GasLevelMeter_OnNotifyAlarm;
            dm.End();

            mNotifyAction = null;
        }

        private List<LevelMeterInfo> m_arPSMMeters = new List<LevelMeterInfo>();
        private void LoadPSMLevelMeter()
        {
            m_arPSMMeters.Clear();

            string szSQL = "SELECT ID, LevelMeterServerID, SlaveID, TagNo, LevelMeterName, Tank1, Tank2, Tank3, Tank4 FROM LevelMeterTagInfo";

            string strSQL = string.Format(szSQL, m_nSiteID);

            ArrayList arrResult = mDBMgr.GetResultData(strSQL);
           
            if (arrResult == null)
                return;

            int nResultCount = arrResult.Count;
            for (int i = 0; i < nResultCount - 8; i += 9)
            {
                int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nReciverID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nSlaveID = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nTagNo = WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                string szLevelMeterName = WebDBManager.GetStringField(arrResult[i + 4], "");                

                int nTank1 = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                int nTank2 = WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);
                int nTank3 = WebDBManager.GetIntField(arrResult[i + 7].ToString(), -1);
                int nTank4 = WebDBManager.GetIntField(arrResult[i + 8].ToString(), -1);

                LevelMeterInfo info = new LevelMeterInfo();
                info.ReciverID = nReciverID;
                info.TagID = nTagNo;
                info.SlaveID = nSlaveID;
                info.Name = szLevelMeterName;
               
                info.Tank1 = nTank1;
                info.Tank2 = nTank2;
                info.Tank3 = nTank3;
                info.Tank4 = nTank4;

                m_arPSMMeters.Add(info);
            }
        }

        public void SaveAllSensorServerInfo(bool isConnected)
        {
            ArrayList arReciverList = LevelMeterNetworkServer.Instance.IOManager.GetReciverList();
            foreach (Reciver reciver in arReciverList)
            {
                SaveLevelServerInfo(reciver.ID, isConnected);
            }
        }

        private void SaveLevelServerInfo(int nReciverID, bool bOnline)
        {
            DateTime dtNow = DateTime.Now;
            string strDateTimeField = string.Format("{0} {1}:{2}:{3}", dtNow.ToShortDateString(), dtNow.Hour, dtNow.Minute, dtNow.Second);
            string szText = "UPDATE LevelServerInfo SET ConnectionState = {0}, ConnectionTime={1} WHERE ID = {2}";
            string szSQL = string.Format(szText, (bOnline == true ? 1 : 0), strDateTimeField, nReciverID);
            mDBMgr.GetResultData(szSQL);
        }

        private void SavePSMLevel(LevelMeterInfo info)
        {
            if (info.Value == -999)
                return;

            DateTime dtNow = DateTime.Now;
            string strDateTimeField = string.Format("{0} {1}:{2}:{3}", dtNow.ToShortDateString(), dtNow.Hour, dtNow.Minute, dtNow.Second);

            if(info.Tank1 > 0 )
            {
                string szText = "UPDATE PSMTank SET Remains = {0}, RemainUpdateTime = '{1}' WHERE ID = {2} ";
                string szSQL = string.Format(szText, info.Value, strDateTimeField, info.Tank1);
                mDBMgr.GetResultData(szSQL);
            }

            if (info.Tank2 > 0)
            {
                string szText = "UPDATE PSMTank SET Remains = {0}, RemainUpdateTime = '{1}' WHERE ID = {2} ";
                string szSQL = string.Format(szText, info.Value, strDateTimeField, info.Tank2);
                mDBMgr.GetResultData(szSQL);
            }

            if (info.Tank3 > 0)
            {
                string szText = "UPDATE PSMTank SET Remains = {0}, RemainUpdateTime = '{1}' WHERE ID = {2} ";
                string szSQL = string.Format(szText, info.Value, strDateTimeField, info.Tank3);
                mDBMgr.GetResultData(szSQL);
            }

            if (info.Tank4 > 0)
            {
                string szText = "UPDATE PSMTank SET Remains = {0}, RemainUpdateTime = '{1}' WHERE ID = {2} ";
                string szSQL = string.Format(szText, info.Value, strDateTimeField, info.Tank4);
                mDBMgr.GetResultData(szSQL);
            }
        }

        private bool m_bReleaseThread = false;
        private void CheckValue()
        {
            while (!m_bReleaseThread)
            {
                foreach (LevelMeterInfo sensor in m_arPSMMeters)
                {
                    bool bOnline = dm.GetOnline(sensor.ReciverID);
                    float fValue = dm.GetLevel(sensor.ReciverID, (sensor.TagID - 1));

                    sensor.Value = (int)fValue;

                    if (bOnline == true)
                        SavePSMLevel(sensor);

                    if (m_bReleaseThread == true)
                        break;
                }

                ArrayList arReciverList = LevelMeterNetworkServer.Instance.IOManager.GetReciverList();
                foreach(Reciver reciver in arReciverList)
                {
                    bool bOnline = dm.GetOnline(reciver.ReciverID);
                    SaveLevelServerInfo(reciver.ID, bOnline);
                }                

                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(100);
                    if (m_bReleaseThread == true)
                        break;
                }
            }
        }       

        internal class LevelMeterInfo
        {
            private int m_nID = -1;
            internal int ID
            {
                get { return m_nID; }
                set { m_nID = value; }
            }

            private int m_nReciverID = -1;
            internal int ReciverID
            {
                get { return m_nReciverID; }
                set { m_nReciverID = value; }
            }

            private int m_nTagID = -1;
            internal int TagID
            {
                get { return m_nTagID; }
                set { m_nTagID = value; }
            }

            private int m_nSlaveID = -1;
            internal int SlaveID
            {
                get { return m_nSlaveID; }
                set { m_nSlaveID = value; }
            }

            private int m_nValue = 0;
            internal int Value
            {
                get { return m_nValue; }
                set { m_nValue = value; }
            }

            private int m_nUpperBound = 180;
            internal int UpperBound
            {
                get { return m_nUpperBound; }
                set { m_nUpperBound = value; }
            }

            private int m_nLowerBound = 0;
            internal int LowerBound
            {
                get { return m_nLowerBound; }
                set { m_nLowerBound = value; }
            }

            private int m_nTankCount = 0;
            internal int TankCount
            {
                get { return m_nTankCount; }
                set { m_nTankCount = value; }
            }

            private int m_nTank1 = -1;
            internal int Tank1
            {
                get { return m_nTank1; }
                set { m_nTank1 = value; }
            }

            private int m_nTank2 = -1;
            internal int Tank2
            {
                get { return m_nTank2; }
                set { m_nTank2 = value; }
            }

            private int m_nTank3 = -1;
            internal int Tank3
            {
                get { return m_nTank3; }
                set { m_nTank3 = value; }
            }

            private int m_nTank4 = -1;
            internal int Tank4
            {
                get { return m_nTank4; }
                set { m_nTank4 = value; }
            }

            private string m_szName = "";
            public string Name
            {
                get { return m_szName; }
                set { m_szName = value; }
            }
        }
    }
}
