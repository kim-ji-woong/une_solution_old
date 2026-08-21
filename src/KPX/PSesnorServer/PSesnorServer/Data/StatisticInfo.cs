using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSensorServer
{

    /// <summary>
    /// 작업 중인 배관/탱크의 압력, 유량 데이터의 평균 데이터
    /// </summary>
    public class StatisticInfo
    {
        // ToDo : 1분 평균, 3분 평균, 10분 평균 작성

        private int m_nHistoryID = -1;

        private long m_nKey = 0;

        private float m_fAvgPressure1Min = 0.0f;
        private float m_fAvgPressure3Min = 0.0f;
        private float m_fAvgPressure10Min = 0.0f;

        private int m_nAvgPressure1Min = 0;
        private int m_nAvgPressure3Min = 0;
        private int m_nAvgPressure10Min = 0;
        

        private float m_fAvgFlow1Min = 0.0f;
        private float m_fAvgFlow3Min = 0.0f;
        private float m_fAvgFlow10Min = 0.0f;

        private int m_nAvgFlow1MinCnt = 0;
        private int m_nAvgFlow3MinCnt = 0;
        private int m_nAvgFlow10MinCnt = 0;



    }
}
