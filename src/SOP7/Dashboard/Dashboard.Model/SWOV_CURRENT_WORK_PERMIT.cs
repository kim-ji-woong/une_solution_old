using System;

namespace Dashboard.Model
{
    public class SWOV_CURRENT_WORK_PERMIT
    {
        public enum Fields { GENERAL_CNT, FIRE_CNT, HIGH_CNT, ELEC_CNT, CLOSENESS_CNT, CRANE_CNT, DIGG_CNT, RADI_CNT, TOTAL_CNT, PLANT_PRCS_ID };

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

        public static string TableName
        {
            get { return "SWOV_CURRENT_WORK_PERMIT"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            isNullable = false;

            return field.ToString();
        }
    }
}
