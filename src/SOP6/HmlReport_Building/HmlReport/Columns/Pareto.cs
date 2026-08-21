using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmlReport.Columns
{
    
    /// <summary>
    /// 탐지 분석 (센서별)
    /// </summary>
    public class Pareto
    {
        public int ColumnCount = 7;
        public string No { get; set; }
        public string SensorName { get; set; }
        public string Building { get; set; }
        public string Floor { get; set; }
        public string Location { get; set; }
        public string HistoryCount { get; set; }
        public string Percent { get; set; }

        public Dictionary<int, TableColumns> Columns
        {
            get
            {
                Dictionary<int, TableColumns> dic = new Dictionary<int, TableColumns>();
                dic.Add(0, new TableColumns() { ColumnName = "No", ColumnWidthRatio = 3133 });
                dic.Add(1, new TableColumns() { ColumnName = "센서 이름", ColumnWidthRatio = 14841 });
                dic.Add(2, new TableColumns() { ColumnName = "건물", ColumnWidthRatio = 14016 });
                dic.Add(3, new TableColumns() { ColumnName = "층", ColumnWidthRatio = 3564 });
                dic.Add(4, new TableColumns() { ColumnName = "위치", ColumnWidthRatio = 21598 });
                dic.Add(5, new TableColumns() { ColumnName = "탐지횟수", ColumnWidthRatio = 4998 });
                dic.Add(6, new TableColumns() { ColumnName = "백분율(%)", ColumnWidthRatio = 4999 });

                return dic;
            }
        }
    }

    /// <summary>
    /// 탐지 분석 (위치별)
    /// </summary>
    public class ParetoEquipmentzone
    {
        public int ColumnCount = 6;
        public string No { get; set; }
        public string Location { get; set; }
        public string Building { get; set; }
        public string Floor { get; set; }
        public string HistoryCount { get; set; }
        public string Percent { get; set; }

        public Dictionary<int, TableColumns> Columns
        {
            get
            {
                Dictionary<int, TableColumns> dic = new Dictionary<int, TableColumns>();
                dic.Add(0, new TableColumns() { ColumnName = "No", ColumnWidthRatio = 3982 });
                dic.Add(1, new TableColumns() { ColumnName = "위치", ColumnWidthRatio = 29840 });
                dic.Add(2, new TableColumns() { ColumnName = "건물", ColumnWidthRatio = 18827 });
                dic.Add(3, new TableColumns() { ColumnName = "층", ColumnWidthRatio = 4130 });
                dic.Add(4, new TableColumns() { ColumnName = "탐지횟수", ColumnWidthRatio = 5088 });
                dic.Add(5, new TableColumns() { ColumnName = "백분율(%)", ColumnWidthRatio = 5089 });

                return dic;
            }
        }
    }

    ///
    /// 탐지 분석 (센서별) - 누출
    /// </summary>
    public class ParetoPSM
    {
        public int ColumnCount = 7;
        public string No { get; set; }
        public string SensorName { get; set; }
        public string Matter { get; set; }
        public string Building { get; set; }        
        public string Location { get; set; }
        public string HistoryCount { get; set; }
        public string Percent { get; set; }

        public Dictionary<int, TableColumns> Columns
        {
            get
            {
                Dictionary<int, TableColumns> dic = new Dictionary<int, TableColumns>();
                dic.Add(0, new TableColumns() { ColumnName = "No", ColumnWidthRatio = 3133 });
                dic.Add(1, new TableColumns() { ColumnName = "센서 이름", ColumnWidthRatio = 15124 });
                dic.Add(2, new TableColumns() { ColumnName = "물질", ColumnWidthRatio = 8073 });
                dic.Add(3, new TableColumns() { ColumnName = "건물", ColumnWidthRatio = 12054 });
                dic.Add(4, new TableColumns() { ColumnName = "누출 발생장소", ColumnWidthRatio = 15089 });
                dic.Add(5, new TableColumns() { ColumnName = "탐지횟수", ColumnWidthRatio = 6838 });
                dic.Add(6, new TableColumns() { ColumnName = "백분율(%)", ColumnWidthRatio = 6838 });

                return dic;
            }
        }
    }

    /// <summary>
    /// 탐지 분석 (탱크별) - 누출
    /// </summary>
    public class ParetoTankPSM
    {
        public int ColumnCount = 7;
        public string No { get; set; }
        public string TankName { get; set; }
        public string Material { get; set; }
        public string Building { get; set; }
        public string Location { get; set; }        
        public string HistoryCount { get; set; }
        public string Percent { get; set; }

        public Dictionary<int, TableColumns> Columns
        {
            get
            {
                Dictionary<int, TableColumns> dic = new Dictionary<int, TableColumns>();
                dic.Add(0, new TableColumns() { ColumnName = "No", ColumnWidthRatio = 3133 });
                dic.Add(1, new TableColumns() { ColumnName = "탱크 이름", ColumnWidthRatio = 14841 });
                dic.Add(2, new TableColumns() { ColumnName = "물질", ColumnWidthRatio = 7507 });
                dic.Add(3, new TableColumns() { ColumnName = "건물", ColumnWidthRatio = 12054 });
                dic.Add(4, new TableColumns() { ColumnName = "누출 발생장소", ColumnWidthRatio = 16504 });
                dic.Add(5, new TableColumns() { ColumnName = "탐지횟수", ColumnWidthRatio = 7121 });
                dic.Add(6, new TableColumns() { ColumnName = "백분율(%)", ColumnWidthRatio = 6272 });

                return dic;
            }
        }
    }

    /// <summary>
    /// 탐지 분석 (위치별) - 누출
    /// </summary>
    public class ParetoEquipmentzonePSM
    {
        public int ColumnCount = 5;
        public string No { get; set; }
        public string Location { get; set; }
        public string Building { get; set; }
        public string HistoryCount { get; set; }
        public string Percent { get; set; }

        public Dictionary<int, TableColumns> Columns
        {
            get
            {
                Dictionary<int, TableColumns> dic = new Dictionary<int, TableColumns>();
                dic.Add(0, new TableColumns() { ColumnName = "No", ColumnWidthRatio = 3982 });
                dic.Add(1, new TableColumns() { ColumnName = "누출 발생장소", ColumnWidthRatio = 35500 });
                dic.Add(2, new TableColumns() { ColumnName = "건물", ColumnWidthRatio = 17297 });
                dic.Add(3, new TableColumns() { ColumnName = "탐지횟수", ColumnWidthRatio = 5371 });
                dic.Add(4, new TableColumns() { ColumnName = "백분율(%)", ColumnWidthRatio = 5372 });

                return dic;
            }
        }
    }

    /// <summary>
    /// 탐지 분석 (물질별) - 누출
    /// </summary>
    public class ParetoMaterialPSM
    {
        public int ColumnCount = 4;
        public string No { get; set; }
        public string Material { get; set; }
        public string HistoryCount { get; set; }
        public string Percent { get; set; }

        public Dictionary<int, TableColumns> Columns
        {
            get
            {
                Dictionary<int, TableColumns> dic = new Dictionary<int, TableColumns>();
                dic.Add(0, new TableColumns() { ColumnName = "No", ColumnWidthRatio = 3982 });
                dic.Add(1, new TableColumns() { ColumnName = "물질", ColumnWidthRatio = 52797 });
                dic.Add(2, new TableColumns() { ColumnName = "탐지횟수", ColumnWidthRatio = 5371 });
                dic.Add(3, new TableColumns() { ColumnName = "백분율(%)", ColumnWidthRatio = 5372 });

                return dic;
            }
        }
    }
}
