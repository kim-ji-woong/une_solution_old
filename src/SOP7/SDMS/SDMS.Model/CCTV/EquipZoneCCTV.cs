using System;
using System.Collections.Generic;
using System.Text;

namespace SDMS.Model.CCTV
{
    public class EquipZoneCCTV : IIDObject
    {
        public enum Fields { ID, EquipZoneID, CCTV1, CCTV2, CCTV3, CCTV4, CCTV5, CCTV6, Preset1, Preset2, Preset3, Preset4, Preset5, Preset6, Description };

        private int m_nID = -1;
        private int m_nEquipZoneID = -1;
        private int? m_nCCTV1 = null;
        private int? m_nCCTV2 = null;
        private int? m_nCCTV3 = null;
        private int? m_nCCTV4 = null;
        private int? m_nCCTV5 = null;
        private int? m_nCCTV6 = null;
        private string m_strPreset1 = null;
        private string m_strPreset2 = null;
        private string m_strPreset3 = null;
        private string m_strPreset4 = null;
        private string m_strPreset5 = null;
        private string m_strPreset6 = null;
        private string m_strDescription = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int EquipZoneID
        {
            get { return m_nEquipZoneID; }
            set { m_nEquipZoneID = value; }
        }

        public int? CCTV1
        {
            get { return m_nCCTV1; }
            set { m_nCCTV1 = value; }
        }

        public int? CCTV2
        {
            get { return m_nCCTV2; }
            set { m_nCCTV2 = value; }
        }

        public int? CCTV3
        {
            get { return m_nCCTV3; }
            set { m_nCCTV3 = value; }
        }

        public int? CCTV4
        {
            get { return m_nCCTV4; }
            set { m_nCCTV4 = value; }
        }

        public int? CCTV5
        {
            get { return m_nCCTV5; }
            set { m_nCCTV5 = value; }
        }

        public int? CCTV6
        {
            get { return m_nCCTV6; }
            set { m_nCCTV6 = value; }
        }

        public string Preset1
        {
            get { return m_strPreset1; }
            set { m_strPreset1 = value; }
        }

        public string Preset2
        {
            get { return m_strPreset2; }
            set { m_strPreset2 = value; }
        }

        public string Preset3
        {
            get { return m_strPreset3; }
            set { m_strPreset3 = value; }
        }

        public string Preset4
        {
            get { return m_strPreset4; }
            set { m_strPreset4 = value; }
        }

        public string Preset5
        {
            get { return m_strPreset5; }
            set { m_strPreset5 = value; }
        }

        public string Preset6
        {
            get { return m_strPreset6; }
            set { m_strPreset6 = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.ID ||
                field == Fields.EquipZoneID)
                isNullable = false;
            else
                isNullable = true;

            return field.ToString();
        }

        public static string TableName
        {
            get { return "SdmsCCTVEquipZone"; }
        }
    }
}
