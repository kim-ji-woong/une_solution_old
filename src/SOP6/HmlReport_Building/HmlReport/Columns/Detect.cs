using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmlReport.Columns
{
    /// <summary>
    /// 탐지 이력
    /// </summary>
    public class Detect
    {
        public int ColumnCount = 9;
        public string No { get; set; }
        public string Date { get; set; }
        public string SensorType { get; set; }
        public string SensorName { get; set; }
        public string Building { get; set; }
        public string Floor { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public string Memo { get; set; }

        public Dictionary<int, TableColumns> Columns
        {
            get
            {
                Dictionary<int, TableColumns> dic = new Dictionary<int, TableColumns>();
                dic.Add(0, new TableColumns() { ColumnName = "No", ColumnWidthRatio = 3982 });
                dic.Add(1, new TableColumns() { ColumnName = "일시", ColumnWidthRatio = 7483 });
                dic.Add(2, new TableColumns() { ColumnName = "유형", ColumnWidthRatio = 7483 });
                dic.Add(3, new TableColumns() { ColumnName = "센서 이름", ColumnWidthRatio = 7655 });
                dic.Add(4, new TableColumns() { ColumnName = "건물", ColumnWidthRatio = 12680 });
                dic.Add(5, new TableColumns() { ColumnName = "층", ColumnWidthRatio = 4394 });
                dic.Add(6, new TableColumns() { ColumnName = "위치", ColumnWidthRatio = 11771 });
                dic.Add(7, new TableColumns() { ColumnName = "상태", ColumnWidthRatio = 4381 });
                dic.Add(8, new TableColumns() { ColumnName = "메모", ColumnWidthRatio = 7211 });

                return dic;
            }
        }
    }

    /// <summary>
    /// 탐지 이력 - 누출
    /// </summary>
    public class DetectPSM
    {
        public int ColumnCount = 8;
        public string No { get; set; }
        public string Date { get; set; }        
        public string Material { get; set; }
        public string SensorName { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public string AlarmLevel { get; set; }
        public string Memo { get; set; }

        public Dictionary<int, TableColumns> Columns
        {
            get
            {
                Dictionary<int, TableColumns> dic = new Dictionary<int, TableColumns>();
                dic.Add(0, new TableColumns() { ColumnName = "No", ColumnWidthRatio = 3416 });
                dic.Add(1, new TableColumns() { ColumnName = "일시", ColumnWidthRatio = 6351 });
                dic.Add(2, new TableColumns() { ColumnName = "물질", ColumnWidthRatio = 5957 });
                dic.Add(3, new TableColumns() { ColumnName = "센서 이름", ColumnWidthRatio = 10416 });
                dic.Add(4, new TableColumns() { ColumnName = "누출 발생장소", ColumnWidthRatio = 12601 });
                dic.Add(5, new TableColumns() { ColumnName = "상태", ColumnWidthRatio = 4524 });
                dic.Add(6, new TableColumns() { ColumnName = "알람단계", ColumnWidthRatio = 4524 });
                dic.Add(7, new TableColumns() { ColumnName = "메모", ColumnWidthRatio = 19241 });

                return dic;
            }
        }
    }

    /// <summary>
    /// 탐지 이력 - 지진
    /// </summary>
    public class DetectEarthquake
    {
        public int ColumnCount = 6;
        public string No { get; set; }
        public string Date { get; set; }
        public string Magnitude { get; set; }
        public string AlarmLevel { get; set; }        
        public string Status { get; set; }
        public string Memo { get; set; }        

        public Dictionary<int, TableColumns> Columns
        {
            get
            {   //67030
                Dictionary<int, TableColumns> dic = new Dictionary<int, TableColumns>();
                dic.Add(0, new TableColumns() { ColumnName = "No", ColumnWidthRatio = 3416 });
                dic.Add(1, new TableColumns() { ColumnName = "일시", ColumnWidthRatio = 12294 });
                dic.Add(2, new TableColumns() { ColumnName = "진도", ColumnWidthRatio = 5963 });
                dic.Add(3, new TableColumns() { ColumnName = "알람단계", ColumnWidthRatio = 5397 });
                dic.Add(4, new TableColumns() { ColumnName = "상태", ColumnWidthRatio = 5656 });
                dic.Add(5, new TableColumns() { ColumnName = "메모", ColumnWidthRatio = 34000 });
                return dic;
            }
        }
    }

    /// <summary>
    /// 탐지 이력 - 온도/습도
    /// </summary>
    public class DetectTH
    {
        public int ColumnCount = 8;
        public string No { get; set; }
        public string Date { get; set; }
        public string SensorType { get; set; }
        public string AlarmType { get; set; }
        public string SensorName { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public string Memo { get; set; }

        public Dictionary<int, TableColumns> Columns
        {
            get
            {
                Dictionary<int, TableColumns> dic = new Dictionary<int, TableColumns>();
                dic.Add(0, new TableColumns() { ColumnName = "No", ColumnWidthRatio = 3982 });
                dic.Add(1, new TableColumns() { ColumnName = "일시", ColumnWidthRatio = 7483 });
                dic.Add(2, new TableColumns() { ColumnName = "유형", ColumnWidthRatio = 9483 });
                dic.Add(3, new TableColumns() { ColumnName = "알람 타입", ColumnWidthRatio = 9483 });
                dic.Add(4, new TableColumns() { ColumnName = "센서 이름", ColumnWidthRatio = 11655 });
                dic.Add(5, new TableColumns() { ColumnName = "알람 발생 장소", ColumnWidthRatio = 12680 });
                dic.Add(6, new TableColumns() { ColumnName = "상태", ColumnWidthRatio = 4381 });
                dic.Add(7, new TableColumns() { ColumnName = "메모", ColumnWidthRatio = 7211 });

                return dic;
            }
        }
    }
}
