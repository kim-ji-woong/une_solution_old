using DBUtility2;
using System;
using System.Collections;

namespace UnE.CCTV
{
	public class EditEquipZoneCCTV : ChangedData
	{
        private static int m_nScrCnt = 6;
		private EquipmentZone m_equipZone = null;
        private CCTV[] m_arrCCTVs = new CCTV[m_nScrCnt];

		public EquipmentZone EquipmentZone
		{
			get { return m_equipZone; }
			set { m_equipZone = value; }
		}

		public new bool IsDeleting
		{
			get { return false; }
			set { }
		}

		public void SetCCTV(int nIndex, CCTV cctv)
		{
            if (nIndex < 0 || nIndex > (m_nScrCnt -1))
				return;

			m_arrCCTVs[nIndex] = cctv;
		}

		public CCTV GetCCTV(int nIndex)
		{
            if (nIndex < 0 || nIndex > (m_nScrCnt - 1))
				return null;

			return m_arrCCTVs[nIndex];
		}

		public override bool Update(WebDBManager dbMgr)
		{
			if (m_equipZone == null || m_equipZone.ID < 0)
				return false;

			string strSQL = "Select id from EquipZoneCCTV where EquipZoneID = " + m_equipZone.ID.ToString();
			ArrayList arrResult = dbMgr.GetResultData(strSQL);

			if (arrResult == null)
			{
				//dbMgr.BatchRollback();
				return false;
			}

            
			int nID = arrResult.Count == 0 ? -1 : WebDBManager.GetIntField(arrResult[0].ToString(), -1);
            // EquipmentZone에 지정된 CCTV가 없는 경우 추가
			if (nID < 0)
			{
				strSQL = "select max(id) from EquipZoneCCTV";
                arrResult = dbMgr.GetResultData(strSQL);

				if (arrResult == null)
				{
					//dbMgr.BatchRollback();
					return false;
				}

				int nNewID = arrResult.Count == 0 ? 1 : WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;
                String[] strPreset = ProxyCCTV.Instance.CurrentEquipZone.Preset;

				string strTemp = "Insert into EquipZoneCCTV (id, EquipZoneID";
                string strVal = " ,description) values ({0}, {1}";
                for(int i = 0; i < m_nScrCnt; i++)
                {
                    strTemp += ",CCTV"+ (i+1).ToString();
                    strVal += ","+ (m_arrCCTVs[i] == null ? "NULL" : m_arrCCTVs[i].ID.ToString());
                }

                for (int i = 0; i < m_nScrCnt; i++)
                {
                    strTemp += ",PRESET" + (i + 1).ToString();
                    strVal += "," + (strPreset[i] == "" ? "NULL" : "'" + strPreset[i] + "'");
                }

                strVal += ", NULL)";
                strTemp += strVal;
                strSQL = string.Format(strTemp, nNewID, m_equipZone.ID);
			}
			else // 있는경우 갱신한다.
			{
                //strSQL = string.Format("Update EquipZoneCCTV set CCTV1 = {0}, CCTV2 = {1}, CCTV3 = {2}, CCTV4 = {3},CCTV5 = {4},CCTV6 = {5} where id = {6}",                    
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendFormat("Update EquipZoneCCTV set CCTV1 = {0}, CCTV2 = {1}, CCTV3 = {2}, CCTV4 = {3},CCTV5 = {4},CCTV6 = {5} ",
                    m_arrCCTVs[0] == null ? "NULL" : m_arrCCTVs[0].ID.ToString(),
                    m_arrCCTVs[1] == null ? "NULL" : m_arrCCTVs[1].ID.ToString(),
                    m_arrCCTVs[2] == null ? "NULL" : m_arrCCTVs[2].ID.ToString(),
                    m_arrCCTVs[3] == null ? "NULL" : m_arrCCTVs[3].ID.ToString(),
                    m_arrCCTVs[4] == null ? "NULL" : m_arrCCTVs[4].ID.ToString(),
                    m_arrCCTVs[5] == null ? "NULL" : m_arrCCTVs[5].ID.ToString());

                if (ProxyCCTV.Instance.EquipZoneCCTVMode == true)
                {
                    String[] strPreset = ProxyCCTV.Instance.CurrentEquipZone.Preset;
                    sb.AppendFormat(",PRESET1 = {0},PRESET2 = {1},PRESET3 = {2},PRESET4 = {3},PRESET5 = {4},PRESET6 = {5} ",
                        strPreset[0] == "" ? "NULL" : "'" + strPreset[0] + "'",
                        strPreset[1] == "" ? "NULL" : "'" + strPreset[1] + "'",
                        strPreset[2] == "" ? "NULL" : "'" + strPreset[2] + "'",
                        strPreset[3] == "" ? "NULL" : "'" + strPreset[3] + "'",
                        strPreset[4] == "" ? "NULL" : "'" + strPreset[4] + "'",
                        strPreset[5] == "" ? "NULL" : "'" + strPreset[5] + "'");
                }

                sb.Append("where id = " + nID.ToString());
                strSQL = sb.ToString();					
			}

			if (dbMgr.GetResultData(strSQL) == null)
			{
				//dbMgr.BatchRollback();
				return false;
			}

			//dbMgr.BatchCommit();

            
            // DB에서 로드된 원본을 갱신한다.
            CCTVManager.Instance.UpdateDBEquipZoneCCTV(m_arrCCTVs, m_equipZone);
            
            // EquipmentZone에 지정된 CCTV가 변경되었음
			//PageBackstageHome.Instance.IsChangedEquipZoneCCTV = true;
			return true;
		}

        /// <summary>
        /// 이미 같은 데이터가 편집되었으면, 해당 데이터의 내용과 합친다.
        /// </summary>
        /// <param name="mgr"></param>
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

        /// <summary>
        /// DB에 저장된것과 다른지 검사
        /// </summary>
        /// <returns>동일하면 true, 다르면 false</returns>
		private bool IsOriginStatus()
		{
			return CCTVManager.Instance.IsOriginStatus(m_equipZone, m_arrCCTVs);
		}
	}
}