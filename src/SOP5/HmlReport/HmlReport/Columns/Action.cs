using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmlReport.Columns
{
    /// <summary>
    /// 대응 이력
    /// </summary>
    public class Action
    {
        public int ColumnCount = 4;
        public string No { get; set; }
        public string Date { get; set; }
        public string Manager { get; set; }
        public string Category { get; set; }

        public Dictionary<int, TableColumns> Columns
        {
            get
            {
                Dictionary<int, TableColumns> dic = new Dictionary<int, TableColumns>();
                dic.Add(0, new TableColumns() { ColumnName = "No", ColumnWidth = 3868 });
                dic.Add(1, new TableColumns() { ColumnName = "일시", ColumnWidth = 11476 });
                dic.Add(2, new TableColumns() { ColumnName = "담당자", ColumnWidth = 5397 });
                dic.Add(3, new TableColumns() { ColumnName = "분류", ColumnWidth = 21402 });

                return dic;
            }
        }
    }

    /// <summary>
    /// 대응 이력 - 누출
    /// </summary>
    public class ActionPSM
    {
        public int ColumnCount = 5;
        public string No { get; set; }
        public string Date { get; set; }
        public string Material { get; set; }
        public string Manager { get; set; }
        public string Category { get; set; }

        public Dictionary<int, TableColumns> Columns
        {
            get
            {
                Dictionary<int, TableColumns> dic = new Dictionary<int, TableColumns>();
                dic.Add(0, new TableColumns() { ColumnName = "No", ColumnWidth = 2453 });
                dic.Add(1, new TableColumns() { ColumnName = "일시", ColumnWidth = 10910 });
                dic.Add(2, new TableColumns() { ColumnName = "물질", ColumnWidth = 6812 });
                dic.Add(3, new TableColumns() { ColumnName = "담당자", ColumnWidth = 7535 });
                dic.Add(4, new TableColumns() { ColumnName = "분류", ColumnWidth = 14133 });

                return dic;
            }
        }
    }
}
