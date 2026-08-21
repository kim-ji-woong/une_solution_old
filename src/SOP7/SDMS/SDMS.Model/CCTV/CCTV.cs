using System;
using System.Collections.Generic;
using System.Text;

namespace SDMS.Model.CCTV
{
    public class CCTV : IIDObject
    {
        public enum Fields { ID, CameraName, PositionName, UniqueKey, X, Y, Z, ZoneID, IsIndoor, Type, Channel, UserID, Password, URL, BigURL, SmallURL, Enabled, CameraIP, CameraCompanyName, CameraModelName, Description };

        private int m_nID = -1;
        private string m_strCameraName = "";
        private string m_strPositionName = null;
        private string m_strUniqueKey = "";
        private float? x = null;
        private float? y = null;
        private float? z = null;
        private int? m_nZoneID = null;
        private bool m_isIndoor = false;
        private string m_strCCTVType = "";
        private int? m_nChannel = null;
        private string m_strUserID = null;
        private string m_strPassword = null;
        private string m_strURL = "";
        private string m_strBigURL = null;
        private string m_strSmallURL = null;
        private bool? m_enabled = null;
        private string m_strDescription = null;
        private string m_strCameraIP = null;
        private string m_strCameraCompanyName = null;
        private string m_strCameraModelName = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string CameraName
        {
            get { return m_strCameraName; }
            set { m_strCameraName = value; }
        }

        public string PositionName
        {
            get { return m_strPositionName; }
            set { m_strPositionName = value; }
        }

        public string UniqueKey
        {
            get { return m_strUniqueKey; }
            set { m_strUniqueKey = value; }
        }

        public float? X
        {
            get { return x; }
            set { x = value; }
        }

        public float? Y
        {
            get { return y; }
            set { y = value; }
        }

        public float? Z
        {
            get { return z; }
            set { z = value; }
        }

        public int? ZoneID
        {
            get { return m_nZoneID; }
            set { m_nZoneID = value; }
        }

        public bool IsIndoor
        {
            get { return m_isIndoor; }
            set { m_isIndoor = value; }
        }

        public string Type
        {
            get { return m_strCCTVType; }
            set { m_strCCTVType = value; }
        }

        public int? Channel
        {
            get { return m_nChannel; }
            set { m_nChannel = value; }
        }

        public string UserID
        {
            get { return m_strUserID; }
            set { m_strUserID = value; }
        }

        public string Password
        {
            get { return m_strPassword; }
            set { m_strPassword = value; }
        }

        public string URL
        {
            get { return m_strURL; }
            set { m_strURL = value; }
        }

        public string BigURL
        {
            get { return m_strBigURL; }
            set { m_strBigURL = value; }
        }

        public string SmallURL
        {
            get { return m_strSmallURL; }
            set { m_strSmallURL = value; }
        }

        public bool? Enabled
        {
            get { return m_enabled; }
            set { m_enabled = value; }
        }

        public string CameraIP
        {
            get { return m_strCameraIP; }
            set { m_strCameraIP = value; }
        }

        public string CameraCompanyName
        {
            get { return m_strCameraCompanyName; }
            set { m_strCameraCompanyName = value; }
        }

        public string CameraModelName
        {
            get { return m_strCameraModelName; }
            set { m_strCameraModelName = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.ID ||
                field == Fields.CameraName ||
                field == Fields.IsIndoor ||
                field == Fields.Type ||
                field == Fields.URL ||
                field == Fields.UniqueKey)
                isNullable = false;
            else
                isNullable = true;

            return field.ToString();
        }

        public static string TableName
        {
            get { return "SdmsCCTV"; }
        }
    }
}
