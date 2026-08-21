using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SDMS
{
    public class EditEquipZoneCCTV : ChangedData
    {
        private EquipmentZone m_equipZone = null;
        private CCTV[] m_arrCCTVs = new CCTV[4] { null, null, null, null };

        public SDMS.EquipmentZone EquipmentZone
        {
            get { return m_equipZone; }
            set { m_equipZone = value; }
        }

        public new bool IsDeleting
        {
            get { return false; }
            set {}
        }

        public void SetCCTV(int nIndex, CCTV cctv)
        {
            if (nIndex < 0 || nIndex > 3)
                return;

            m_arrCCTVs[nIndex] = cctv;
        }

        public CCTV GetCCTV(int nIndex)
        {
            if (nIndex < 0 || nIndex > 3)
                return null;

            return m_arrCCTVs[nIndex];
        }

        public override bool Update(DBUtility.WebDBManager dbMgr)
        {
            if (m_equipZone == null || m_equipZone.ID < 0)
                return false;

            string strSQL = "Select id from EquipZoneCCTV where EquipZoneID = " + m_equipZone.ID.ToString();
            ArrayList arrResult = dbMgr.GetBatchData(strSQL);

            if (arrResult == null)
            {
                dbMgr.BatchRollback();
                return false;
            }

            int nID = arrResult.Count == 0 ? -1 : DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);

            if (nID < 0)
            {
                strSQL = "select max(id) from EquipZoneCCTV";
                arrResult = dbMgr.GetBatchData(strSQL);

                if (arrResult == null)
                {
                    dbMgr.BatchRollback();
                    return false;
                }

                int nNewID = arrResult.Count == 0 ? 1 : DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;

                strSQL = string.Format("Insert into EquipZoneCCTV (id, EquipZoneID, CCTV1, CCTV2, CCTV3, CCTV4, description) values ({0}, {1}, {2}, {3}, {4}, {5}, NULL)",
                    nNewID, m_equipZone.ID,
                    m_arrCCTVs[0] == null ? "NULL" : m_arrCCTVs[0].ID.ToString(),
                    m_arrCCTVs[1] == null ? "NULL" : m_arrCCTVs[1].ID.ToString(),
                    m_arrCCTVs[2] == null ? "NULL" : m_arrCCTVs[2].ID.ToString(),
                    m_arrCCTVs[3] == null ? "NULL" : m_arrCCTVs[3].ID.ToString());

				
            }
            else
            {
                strSQL = string.Format("Update EquipZoneCCTV set CCTV1 = {0}, CCTV2 = {1}, CCTV3 = {2}, CCTV4 = {3} where id = {4}",
                    m_arrCCTVs[0] == null ? "NULL" : m_arrCCTVs[0].ID.ToString(),
                    m_arrCCTVs[1] == null ? "NULL" : m_arrCCTVs[1].ID.ToString(),
                    m_arrCCTVs[2] == null ? "NULL" : m_arrCCTVs[2].ID.ToString(),
                    m_arrCCTVs[3] == null ? "NULL" : m_arrCCTVs[3].ID.ToString(),
                    nID);
            }

            if (dbMgr.GetBatchData(strSQL) == null)
            {
                dbMgr.BatchRollback();
                return false;
            }

            dbMgr.BatchCommit();
            CCTVManager.Instance.UpdateDBEquipZoneCCTV(m_arrCCTVs, m_equipZone);
			PageBackstageHome.Instance.IsChangedEquipZoneCCTV = true;
            return true;
        }

        // 이미 같은 데이터가 편집되었으면, 해당 데이터의 내용과 합친다.
        public override void AddToManager(IChangedDataManager mgr)
        {
            if (m_equipZone == null)
                return;

            ArrayList arrDatas = mgr.GetDataList();
            Type type = typeof(EditEquipZoneCCTV);

            foreach (ChangedData data in arrDatas)
            {
                if (data.GetType() == type)
                {
                    EditEquipZoneCCTV cctv = (EditEquipZoneCCTV)data;

                    if (cctv.m_equipZone == this.m_equipZone)
                    {
                        cctv.m_arrCCTVs = this.m_arrCCTVs;

                        if (IsOriginStatus())
                            mgr.RemoveData(cctv);

                        return;
                    }
                }
            }

            // DB에 저장된 것과 동일한 상태인가?
            if (!IsOriginStatus())
                mgr.SomethingChanged(this);
        }

        private bool IsOriginStatus()
        {
            return CCTVManager.Instance.IsOriginStatus(m_equipZone, m_arrCCTVs);
        }
    }
}
