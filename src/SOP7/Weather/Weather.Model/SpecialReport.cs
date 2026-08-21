using System;

namespace Weather.Model
{
    public class SpecialReport
    {
        public enum Fields { WeatherSiteID, Url, ImageUrl, UpdateTime };

        private int m_nSiteID = -1;
        private string m_strURL = null;
        private string m_strImageUrl = null;
        private DateTime m_dtUpdate = new DateTime();

        public int WeatherSiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        public string Url
        {
            get { return m_strURL; }
            set { m_strURL = value; }
        }

        public string ImageUrl
        {
            get { return m_strImageUrl; }
            set { m_strImageUrl = value; }
        }

        public DateTime UpdateTime
        {
            get { return m_dtUpdate; }
            set { m_dtUpdate = value; }
        }

        public static string TableName
        {
            get { return "WeatherSpecialReport"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.Url ||
                field == Fields.ImageUrl)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }
    }
}
