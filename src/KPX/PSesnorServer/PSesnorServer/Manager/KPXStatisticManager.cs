using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSensorServer
{
    public class KPXStatisticManager
    {
        private static object m_lock = new object();
        private static KPXStatisticManager m_Instance = null;
        public static KPXStatisticManager Instance
        {
            get
            {
                lock (m_lock)
                {
                    if (m_Instance == null)
                        m_Instance = new KPXStatisticManager();
                    return m_Instance;
                }

            }
        }

        private SortedList<int, StatisticInfo> m_workStatistics = new SortedList<int, StatisticInfo>();

        public void SaveStatistic(int nWorkHistoryID)
        {
            if(m_workStatistics.ContainsKey(nWorkHistoryID))
            {
                StatisticInfo info = m_workStatistics[nWorkHistoryID];

                SaveStatisticInfo(info);

                m_workStatistics.Remove(nWorkHistoryID);
            }
        }

        private KPXStatisticManager()
        {
        }

        private void SaveStatisticInfo(StatisticInfo info)
        {
            // ToDo : 1분 평균, 3분 평균, 10분 평균 저장 : pipeaverage

            string szTemp = "INSERT INTO ";
        }

        public void CalcStatisticPipeInfo(int nWorkHistoryID, int nPipeID, float fPressure, DateTime dtTime)
        {
            // ToDo : 1분 평균, 3분 평균, 10분 평균 작성
        }

        public void CalcStatisticTankInfo(int nWorkHistoryID, int nTankID, float fFlow, DateTime dtTime)
        {
        }

        

    }
}
