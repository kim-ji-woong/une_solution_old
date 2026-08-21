using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JubixNetwork
{
    public class TablePartition
    {
        private static string szTableName = "PipeHistory";

        public static string GetTableNames(int nPipeID, DateTime dtTime)
        {
            int nBeginMonth = dtTime.Month;
            string szResult = string.Format("{0}_{1}_{2:D2}", szTableName, nPipeID, nBeginMonth);           
            return szResult;
        }

        public static string GetReportHistorySQL(int nPipeID, DateTime dtBegin, DateTime dtEnd)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT {0} as PipeID, TimeStamp, Pressure ");
            sb.Append("  FROM pipehistory_{1}_{2:D2}");
            sb.Append("   WHERE timestamp >= '" + dtBegin.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            sb.Append("   AND timestamp <= '" + dtEnd.ToString("yyyy-MM-dd HH:mm:ss") + "'");

            int nBeginMonth = dtBegin.Month;
            int nEndMont = dtEnd.Month;

            string szResult = "";
            for (int i = nBeginMonth; i <= nEndMont; i++)
            {
                if (szResult != "")
                {
                    szResult += " UNION ";
                }
                szResult += string.Format(sb.ToString(), nPipeID, nPipeID, i);                
            }
            return szResult;
        }

        public static string GetPipeHistorySQL(int nPipeID, DateTime dtBegin, DateTime dtEnd)
        {
            int nBeginMonth = dtBegin.Month;
            int nEndMont = dtEnd.Month;

            string szResult = "";
            for (int i = nBeginMonth; i <= nEndMont; i++)
            {               
                if (szResult != "")
                {
                    szResult += " UNION ";
                }

                szResult += string.Format("SELECT ID, {0} as PipeID,TimeStamp, Pressure FROM pipehistory_{1}_{2:D2}", nPipeID, nPipeID, i);
                szResult += " WHERE TimeStamp >= date_add(now(), interval - 30 minute) AND Pressure > 0.2 ";                
            }
            return szResult;
        }

        public static string GetMainQuerySQL(DateTime dtBegin, DateTime dtEnd)
        {
            int nBeginMonth = dtBegin.Month;
            int nEndMont = dtEnd.Month;
            string szResult = "";
            for (int i = nBeginMonth; i <= nEndMont; i++)
            {
                for (int j = 1; j < 10; j++)
                {
                    if (szResult != "")
                    {
                        szResult += " UNION ";
                    }

                    szResult += string.Format("SELECT ID, {0} as PipeID,TimeStamp, Pressure FROM pipehistory_{1}_{2:D2}", j, j, i);
                    szResult += " WHERE TimeStamp >= date_add(now(), interval - 30 minute) AND Pressure > 0.2 ";
                }
            }
            return szResult;
        }
    }
}
