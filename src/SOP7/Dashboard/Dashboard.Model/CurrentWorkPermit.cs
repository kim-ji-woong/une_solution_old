using System;

namespace Dashboard.Model
{
    public class CurrentWorkPermit
    {
        public enum Fields { GENERAL_CNT, FIRE_CNT, HIGH_CNT, ELEC_CNT, CLOSENESS_CNT, CRANE_CNT, DIGG_CNT, RADI_CNT, TOTAL_CNT, PLANT_PRCS_ID, UpdateTime };

        public int GENERAL_CNT { get; set; }
        public int FIRE_CNT { get; set; }
        public int HIGH_CNT { get; set; }
        public int ELEC_CNT { get; set; }
        public int CLOSENESS_CNT { get; set; }
        public int CRANE_CNT { get; set; }
        public int DIGG_CNT { get; set; }
        public int RADI_CNT { get; set; }
        public int TOTAL_CNT { get; set; }
        public string PLANT_PRCS_ID { get; set; }
        public DateTime UpdateTime { get; set; }

        public static string TableName
        {
            get { return "DashboardCurrentWorkPermit"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            isNullable = false;

            return field.ToString();
        }
    }
}
