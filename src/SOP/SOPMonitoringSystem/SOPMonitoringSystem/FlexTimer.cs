using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SOPMonitoringSystem
{
    public class FlexTimer
    {
        static private bool m_isInit = false;
        static private DateTime m_timeFirst;
        static private int m_speed = 4; // 원래 시간보다 4배 빠르게...

        static public DateTime Now
        {
            get
            {
                if (m_isInit)
                {
                    DateTime t = DateTime.Now;
                    TimeSpan ts = t - m_timeFirst;

                    return t.AddMilliseconds(ts.TotalMilliseconds * m_speed);
                }

                m_isInit = true;
                m_timeFirst = DateTime.Now;
                return m_timeFirst;
            }
        }
    }
}
