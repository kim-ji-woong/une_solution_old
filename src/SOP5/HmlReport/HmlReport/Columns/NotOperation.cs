using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmlReport.Columns
{
    /// <summary>
    /// 처리 이력
    /// </summary>
    public class NotOperation
    {
        public int ColumnCount = 10;
        public string No { get; set; }
        public string Type { get; set; }
        public string BuildingGroup { get; set; }
        public string Building { get; set; }
        public string Floor { get; set; }
        public string Detect { get; set; }
        public string Fire { get; set; }
        public string Malfunction { get; set; }
        public string UnHandling { get; set; }
        public string MalfunctionRate { get; set; }

        public Dictionary<int, TableColumns> Columns
        {
            get
            {
                Dictionary<int, TableColumns> dic = new Dictionary<int, TableColumns>();
                dic.Add(0, new TableColumns() { ColumnName = "No", ColumnWidth = 3459 });
                dic.Add(1, new TableColumns() { ColumnName = "유형", ColumnWidth = 5520 });
                dic.Add(2, new TableColumns() { ColumnName = "건물 그룹", ColumnWidth = 5543 });
                dic.Add(3, new TableColumns() { ColumnName = "건물", ColumnWidth = 8607 });
                dic.Add(4, new TableColumns() { ColumnName = "층", ColumnWidth = 3355 });
                dic.Add(5, new TableColumns() { ColumnName = "탐지", ColumnWidth = 2911 });
                dic.Add(6, new TableColumns() { ColumnName = "화재", ColumnWidth = 2911 });
                dic.Add(7, new TableColumns() { ColumnName = "오작동", ColumnWidth = 2911 });
                dic.Add(8, new TableColumns() { ColumnName = "처리 안됨", ColumnWidth = 2911 });
                dic.Add(9, new TableColumns() { ColumnName = "오작동률", ColumnWidth = 3387 });

                return dic;
            }
        }
    }

    /// <summary>
    /// 처리 이력 - 누출
    /// </summary>
    public class NotOperationPSM
    {
        public int ColumnCount = 8;
        public string No { get; set; }
        public string Material { get; set; }
        public string Building { get; set; }
        public string Location { get; set; }
        public string Detect { get; set; }
        public string Psm { get; set; }
        public string SystemRestore { get; set; }
        public string PlaceRestore { get; set; }

        public Dictionary<int, TableColumns> Columns
        {
            get
            {
                Dictionary<int, TableColumns> dic = new Dictionary<int, TableColumns>();
                dic.Add(0, new TableColumns() { ColumnName = "No", ColumnWidth = 3176 });
                dic.Add(1, new TableColumns() { ColumnName = "물질", ColumnWidth = 5803 });
                dic.Add(2, new TableColumns() { ColumnName = "건물", ColumnWidth = 6958 });
                dic.Add(3, new TableColumns() { ColumnName = "누출 발생장소", ColumnWidth = 11154 });
                dic.Add(4, new TableColumns() { ColumnName = "탐지", ColumnWidth = 3760 });
                dic.Add(5, new TableColumns() { ColumnName = "누출 신고", ColumnWidth = 3760 });
                dic.Add(6, new TableColumns() { ColumnName = "시스템복구", ColumnWidth = 3760 });
                dic.Add(7, new TableColumns() { ColumnName = "현장 복구", ColumnWidth = 3760 });

                return dic;
            }
        }
    }

    /// <summary>
    /// 처리 이력
    /// </summary>
    public class NotOperationSecurity
    {
        public int ColumnCount = 10;
        public string No { get; set; }
        public string Type { get; set; }
        public string BuildingGroup { get; set; }
        public string Building { get; set; }
        public string Floor { get; set; }
        public string Detect { get; set; }
        public string Fire { get; set; }
        public string Malfunction { get; set; }
        public string UnHandling { get; set; }
        public string MalfunctionRate { get; set; }

        public Dictionary<int, TableColumns> Columns
        {
            get
            {
                Dictionary<int, TableColumns> dic = new Dictionary<int, TableColumns>();
                dic.Add(0, new TableColumns() { ColumnName = "No", ColumnWidth = 3459 });
                dic.Add(1, new TableColumns() { ColumnName = "유형", ColumnWidth = 5520 });
                dic.Add(2, new TableColumns() { ColumnName = "건물 그룹", ColumnWidth = 5543 });
                dic.Add(3, new TableColumns() { ColumnName = "건물", ColumnWidth = 8607 });
                dic.Add(4, new TableColumns() { ColumnName = "층", ColumnWidth = 3355 });
                dic.Add(5, new TableColumns() { ColumnName = "탐지", ColumnWidth = 2911 });
                dic.Add(6, new TableColumns() { ColumnName = "화재", ColumnWidth = 2911 });
                dic.Add(7, new TableColumns() { ColumnName = "오작동", ColumnWidth = 2911 });
                dic.Add(8, new TableColumns() { ColumnName = "처리 안됨", ColumnWidth = 2911 });
                dic.Add(9, new TableColumns() { ColumnName = "오작동률", ColumnWidth = 3387 });

                return dic;
            }
        }
    }
}
