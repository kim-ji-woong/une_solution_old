using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace DXFUtility
{
    public class DXFFireEquipmentTemp
    {
        private WebDBManager m_dbMgr = null;

        // Key : RFIDTag
        private Dictionary<string, FireEquipmentDBDataEx> m_dicFireEquipment = new Dictionary<string, FireEquipmentDBDataEx>();
        private Dictionary<string, FireEquipmentDBDataEx> m_dicFireEquipmentTemp = new Dictionary<string, FireEquipmentDBDataEx>();
        // Key : FireEquipment ID
        // Value : FireEquipmentTemp ID
        private Dictionary<int, int> m_dicOriginToNewEquip = new Dictionary<int, int>();

        public DXFFireEquipmentTemp(WebDBManager dbMgr)
        {
            m_dbMgr = dbMgr;
        }

        public void Run()
        {
            if (ReadFireEquipment())
            {
                if (ReadFireEquipmentTemp())
                {
                    if (UpdateFireEquipmentTemp())
                    {
                        if (UpdateFireEquipmentGroup())
                        {
                            MessageBox.Show("DB Update 성공");
                            return;
                        }
                    }
                }
            }

            MessageBox.Show("DB Update 실패");
        }

        private bool UpdateFireEquipmentGroup()
        {
            string strSQL = "select id, linkedEquipID from FireEquipmentGroup";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;
            int nID = 1;
            ArrayList arrIgnores = new ArrayList();

            for (int i = 0; i < nResultCount - 1; i += 2)
            {
                int nGroupID = m_dbMgr.GetIntField(arrResult[i].ToString(), -1);
                int nLinkedEquipID = m_dbMgr.GetIntField(arrResult[i + 1].ToString(), -1);

                if (nGroupID < 0 || nLinkedEquipID < 0)
                    continue;

                if (!m_dicOriginToNewEquip.ContainsKey(nLinkedEquipID))
                {
                    arrIgnores.Add(nGroupID);
                    continue;
                }

                int nEquipTempID = m_dicOriginToNewEquip[nLinkedEquipID];

                string strQuery = string.Format("Insert into FireEquipmentGroupTemp (id, linkedEquipID, x, y, z) values ({0}, {1}, NULL, NULL, NULL)",
                    nID++, nEquipTempID);

                if (m_dbMgr.GetResultData(strQuery, 0) == null)
                    return false;
            }

            return true;
        }

        private bool UpdateFireEquipmentTemp()
        {
            foreach (KeyValuePair<string, FireEquipmentDBDataEx> pair in m_dicFireEquipmentTemp)
            {
                FireEquipmentDBDataEx equip = pair.Value;

                string strSQL = string.Format("Update FireEquipmentTemp set DxfObjID = {0}, ZoneID = {1}, X = {2}, Y = {3}, Z = {4} where ID = {5}",
                    string.Compare(equip.DXFObjID, "NULL", true) == 0 ? "NULL" : "'" + equip.DXFObjID + "'",
                    equip.ZoneID,
                    equip.X, equip.Y, equip.Z,
                    equip.ID);

                if (m_dbMgr.GetResultData(strSQL, 0) == null)
                    return false;
            }

            return true;
        }

        private bool ReadFireEquipment()
        {
            string strSQL = "select id, RFIDTag, EquipID, DxfObjID, EquipType, EquipSubType, ZoneID, X, Y, Z, CreateDate, Duration, Description from FireEquipment";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;
            
            for (int i = 0; i < nResultCount - 12; i += 13)
            {
                int nID = m_dbMgr.GetIntField(arrResult[i].ToString(), -1);
                string strRFIDTag = m_dbMgr.GetStringField(arrResult[i + 1], "NULL");
                string strEquipID = m_dbMgr.GetStringField(arrResult[i + 2], "NULL");
                string strDXFObjID = m_dbMgr.GetStringField(arrResult[i + 3], "NULL");
                int nEquipType = m_dbMgr.GetIntField(arrResult[i + 4].ToString(), -1);
                int nEquipSubType = m_dbMgr.GetIntField(arrResult[i + 5].ToString(), -1);
                int nZoneID = m_dbMgr.GetIntField(arrResult[i + 6].ToString(), -1);
                float x = m_dbMgr.GetFloatField(arrResult[i + 7].ToString(), 0.0f);
                float y = m_dbMgr.GetFloatField(arrResult[i + 8].ToString(), 0.0f);
                float z = m_dbMgr.GetFloatField(arrResult[i + 9].ToString(), 0.0f);
                string strCreateDate = m_dbMgr.GetStringField(arrResult[i + 10], "NULL");
                int nDuration = m_dbMgr.GetIntField(arrResult[i + 11].ToString(), -1);
                string strDescription = m_dbMgr.GetStringField(arrResult[i + 12], "NULL");

                if (nID < 0)
                    continue;

                FireEquipmentDBDataEx equip = new FireEquipmentDBDataEx();

                equip.ID = nID;
                equip.EquipID = strEquipID;
                equip.CreateDate = strCreateDate;
                equip.Description = strDescription;
                equip.Duration = nDuration < 0 ? "NULL" : nDuration.ToString();
                equip.EquipSubType = nEquipSubType < 0 ? "NULL" : nEquipSubType.ToString();
                equip.EquipType = nEquipType;
                equip.RFIDTag = strRFIDTag;
                equip.ZoneID = nZoneID;
                equip.DXFObjID = strDXFObjID;
                equip.X = x;
                equip.Y = y;
                equip.Z = z;

                m_dicFireEquipment[strRFIDTag] = equip;
            }

            return true;
        }

        private bool ReadFireEquipmentTemp()
        {
            string strSQL = "select id, RFIDTag, EquipID, DxfObjID, EquipType, EquipSubType, ZoneID, X, Y, Z, CreateDate, Duration, Description from FireEquipmentTemp";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;
            ArrayList arrUnknownTags = new ArrayList();

            for (int i = 0; i < nResultCount - 12; i += 13)
            {
                int nID = m_dbMgr.GetIntField(arrResult[i].ToString(), -1);
                string strRFIDTag = m_dbMgr.GetStringField(arrResult[i + 1], "NULL");
                string strEquipID = m_dbMgr.GetStringField(arrResult[i + 2], "NULL");
                string strDXFObjID = m_dbMgr.GetStringField(arrResult[i + 3], "NULL");
                int nEquipType = m_dbMgr.GetIntField(arrResult[i + 4].ToString(), -1);
                int nEquipSubType = m_dbMgr.GetIntField(arrResult[i + 5].ToString(), -1);
                int nZoneID = m_dbMgr.GetIntField(arrResult[i + 6].ToString(), -1);
                float x = m_dbMgr.GetFloatField(arrResult[i + 7].ToString(), 0.0f);
                float y = m_dbMgr.GetFloatField(arrResult[i + 8].ToString(), 0.0f);
                float z = m_dbMgr.GetFloatField(arrResult[i + 9].ToString(), 0.0f);
                string strCreateDate = m_dbMgr.GetStringField(arrResult[i + 10], "NULL");
                int nDuration = m_dbMgr.GetIntField(arrResult[i + 11].ToString(), -1);
                string strDescription = m_dbMgr.GetStringField(arrResult[i + 12], "NULL");

                if (nID < 0 || string.Compare(strRFIDTag, "null", true) == 0)
                    continue;

                FireEquipmentDBDataEx equip = new FireEquipmentDBDataEx();

                if (!m_dicFireEquipment.ContainsKey(strRFIDTag))
                {
                    arrUnknownTags.Add(nID);
                    continue;
                }

                FireEquipmentDBDataEx equipOrg = m_dicFireEquipment[strRFIDTag];

                equip.ID = nID;
                equip.EquipID = strEquipID;
                equip.CreateDate = strCreateDate;
                equip.Description = strDescription;
                equip.Duration = nDuration < 0 ? "NULL" : nDuration.ToString();
                equip.EquipSubType = nEquipSubType < 0 ? "NULL" : nEquipSubType.ToString();
                equip.EquipType = nEquipType;
                equip.RFIDTag = strRFIDTag;
                equip.ZoneID = equipOrg.ZoneID;
                equip.DXFObjID = equipOrg.DXFObjID;
                equip.X = equipOrg.X;
                equip.Y = equipOrg.Y;
                equip.Z = equipOrg.Z;

                if (equip.EquipType != equipOrg.EquipType || equip.EquipSubType != equipOrg.EquipSubType)
                    continue;

                m_dicFireEquipmentTemp[strRFIDTag] = equip;
                m_dicOriginToNewEquip[equipOrg.ID] = equip.ID;
            }

            return true;
        }
    }

    class FireEquipmentDBDataEx : FireEquipmentDBData
    {
        private int m_nID = -1;
        private string m_strDxfObjID = "NULL";
        private float x = 0.0f;
        private float y = 0.0f;
        private float z = 0.0f;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string DXFObjID
        {
            get { return m_strDxfObjID; }
            set { m_strDxfObjID = value; }
        }

        public float X
        {
            get { return x; }
            set { x = value; }
        }

        public float Y
        {
            get { return y; }
            set { y = value; }
        }

        public float Z
        {
            get { return z; }
            set { z = value; }
        }
    }
}
