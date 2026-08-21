using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmlReport.Columns
{
    /// <summary>
    /// 상황판
    /// </summary>
    public class Bulletin
    {
        public int ColumnCount = 6;
        public string No { get; set; }
        public string Date { get; set; }
        public string Caller { get; set; }
        public string Receiver { get; set; }
        public string Mission { get; set; }
        public string Status { get; set; }

        public Dictionary<int, TableColumns> Columns
        {
            get
            {
                Dictionary<int, TableColumns> dic = new Dictionary<int, TableColumns>();
                dic.Add(0, new TableColumns() { ColumnName = "번호", ColumnWidthRatio = 4717 });
                dic.Add(1, new TableColumns() { ColumnName = "시간", ColumnWidthRatio = 10061 });
                dic.Add(2, new TableColumns() { ColumnName = "발신자", ColumnWidthRatio = 9333 });
                dic.Add(3, new TableColumns() { ColumnName = "수신자", ColumnWidthRatio = 8767 });
                dic.Add(4, new TableColumns() { ColumnName = "임무", ColumnWidthRatio = 26314 });
                dic.Add(5, new TableColumns() { ColumnName = "상황", ColumnWidthRatio = 7341 });

                return dic;
            }
        }
    }
}
