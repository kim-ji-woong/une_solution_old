using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KpxPipeMonitoring
{
    public enum HistoryQueryType
    {
        압력 = 1,
        유량 = 2,
        작업중 = 3,
        NONE
    }

    public interface IHistoryManager
    {
        //List<CommonFunction.PipeChartField> ReadFile(string[] paths);
        //void WriteRealTimePipeHistory();  

        List<CommonFunction.ChartField> ReadHistory(List<HistoryQuery> historyQueries);
    }
    
    public class HistoryQuery
    {
        private int nPipeID;
        public int TargetID
        {
            get { return nPipeID; }
            set { nPipeID = value; }
        }

        private string y;
        public string Year
        {
            get { return y; }
            set { y = value; }
        }

        private string m;
        public string Month
        {
            get { return m; }
            set { m = value; }
        }

        private string d;
        public string Day
        {
            get { return d; }
            set { d = value; }
        }

        private HistoryQueryType historyQueryType;
        internal HistoryQueryType QueryType
        {
            get { return historyQueryType; }
            set { historyQueryType = value; }
        }

        public HistoryQuery(int targetID, string y, string m, string d, HistoryQueryType historyQueryType)
        {
            this.nPipeID = targetID;
            this.y = y;
            this.m = m;
            this.d = d;
            this.historyQueryType = historyQueryType;
        }
    }        
}
