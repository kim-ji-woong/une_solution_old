using System;
using System.Collections.Generic;
using System.Threading;

namespace SDMS.BLL.Models.Data
{
    using Model.CCTV;
    using IDAL;

    // 새로운 CCTV가 추가되었는지를 확인하기 위한 클래스
    public static class CCTVManager
    {
        private class QueryData
        {
            private DateTime m_dtQuery = new DateTime();
            private List<CCTV> m_cctvList = null;
            private string m_strErrorMessage = null;

            public DateTime QueryTime
            {
                get { return m_dtQuery; }
                set { m_dtQuery = value; }
            }

            public List<CCTV> CCTVList
            {
                get { return m_cctvList; }
                set { m_cctvList = value; }
            }

            public string ErrorMessage
            {
                get { return m_strErrorMessage; }
                set { m_strErrorMessage = value; }
            }
        }

        // 얼마 이전에 같은 쿼리가 있었으면 해당 쿼리를 결과를 사용한다.(milli second)
        private static int m_nIgnoreTime = 3 * 1000;
        private static QueryData m_lastQueryData = null;

        public static List<CCTV> GetNewCCTVList(IDataManager dataManager, out string strErrorMessage)
        {
            strErrorMessage = null;
            List<CCTV> cctvList = GetLastNewCCTVList(ref strErrorMessage);

            if (cctvList != null)
                return cctvList;
            else if (strErrorMessage != null)
                return null;

            QueryData newData = new QueryData();
            newData.QueryTime = DateTime.Now;
            m_lastQueryData = newData;

            cctvList = ReadNewCCTVList(dataManager, out strErrorMessage);

            if (cctvList != null)
                newData.CCTVList = cctvList;
            else
                newData.ErrorMessage = strErrorMessage;

            return cctvList;
        }

        private static List<CCTV> ReadNewCCTVList(IDataManager dataManager, out string strErrorMessage)
        {
            bool isNullable;

            /*string strAdditionalCondition = string.Format("{0} is NULL",
                CCTV.GetFieldName(CCTV.Fields.ZoneID, out isNullable));*/
            string strAdditionalCondition = string.Format("{0} is NULL or {1} is NULL or {2} is NULL or {3} is NULL",
                CCTV.GetFieldName(CCTV.Fields.X, out isNullable),
                CCTV.GetFieldName(CCTV.Fields.Y, out isNullable),
                CCTV.GetFieldName(CCTV.Fields.Z, out isNullable),
                CCTV.GetFieldName(CCTV.Fields.ZoneID, out isNullable));

            return dataManager.GetSelectManager().SelectCCTVs(null, strAdditionalCondition, out strErrorMessage);
        }

        // 최근에 NewCCTVList 조회한 결과가 있는가?
        private static List<CCTV> GetLastNewCCTVList(ref string strErrorMessage)
        {
            QueryData lastQueryData = m_lastQueryData;

            if (lastQueryData != null)
            {
                DateTime dtNow = DateTime.Now;
                TimeSpan span = dtNow - lastQueryData.QueryTime;

                if (span.TotalMilliseconds <= m_nIgnoreTime)
                {
                    if (lastQueryData.CCTVList != null)
                        return lastQueryData.CCTVList;
                    else if (lastQueryData.ErrorMessage != null)
                    {
                        strErrorMessage = lastQueryData.ErrorMessage;
                        return null;
                    }
                    else
                    {
                        // 최대 3초간 결과값 대기한다.
                        for (int i=0;i<30;i++)
                        {
                            Thread.Sleep(100);

                            if (lastQueryData.CCTVList != null)
                                return lastQueryData.CCTVList;
                            else if (lastQueryData.ErrorMessage != null)
                            {
                                strErrorMessage = lastQueryData.ErrorMessage;
                                return null;
                            }
                        }
                    }
                }
            }

            return null;
        }
    }
}
