using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SDMS
{
    public class EditFacilityManager : ChangedData
    {
        // m_isDeleting이 true이면 다른 인자는 모두 무시하고 m_nID에 해당하는 FacilityManager를 삭제한다.
        private bool m_isDeleting = false;
        private VariousData<int> m_memberID = null;
        private VariousData<int> m_memberType = null;
        private VariousData<int> m_facilityType = null;
        private VariousData<int> m_levelLimit = null;
        private VariousData<string> m_description = null;
		private VariousData<int> m_nUpperLimit = null;
        private FacilityManager m_mgr = null;

        public new bool IsDeleting
        {
            get { return m_isDeleting; }
            set { m_isDeleting = value; }
        }

        public FacilityManager Manager
        {
            get { return m_mgr; }
            set { m_mgr = value; }
        }

        public int ID
        {
            get { return m_mgr == null ? -1 : m_mgr.ID; }
        }

        public int MemberID
        {
            set { m_memberID = new VariousData<int>(value); }
        }

        public int MemberType
        {
            set { m_memberType = new VariousData<int>(value); }
        }

        public int FacilityType
        {
            set { m_facilityType = new VariousData<int>(value); }
        }

        public int LevelLimit
        {
            set { m_levelLimit = new VariousData<int>(value); }
        }

		public int UpperLimit
		{
			set { m_nUpperLimit = new VariousData<int>(value); }
		}

        public string Description
        {
            set { m_description = new VariousData<string>(value); }
        }

        public override bool Update(DBUtility.WebDBManager dbMgr)
        {
            int nBuildingID = 0;
            string strTableName = "BuildingFacilityManager";

            if (m_mgr.Group == null)
                return false;

			int nEquipZoneID = 0;
			if( m_mgr.Group.EquipZone != null)
			{
				nEquipZoneID = m_mgr.Group.EquipZone.ID;
				strTableName = "EquipZoneFacilityManager";
			}
			else
			{
				if (m_mgr.Group.Building != null)
					nBuildingID = m_mgr.Group.Building.ID;
				else if (m_mgr.Group.Zone != null)
					nBuildingID = -m_mgr.Group.Zone.ID;
				else
					strTableName = "FacilityManager";
			}
          

            /*if (m_mgr.Building != null)
                nBuildingID = m_mgr.Building.ID;
            else if (m_mgr.Zone != null)
                nBuildingID = -m_mgr.Zone.ID;
            else
                strTableName = "FacilityManager";*/

            if (m_isDeleting)
            {
                if (ID < 0)
                    return false;

                string strSQL = string.Format("Delete from {0} where ID = {1}", strTableName, m_mgr.ID);
                bool isSuccess = dbMgr.GetResultData(strSQL, 0) != null;

                if (isSuccess)
                    PageBackstageHome.Instance.IsChangedFacilityManager = true;

                return isSuccess;
            }

            if (ID < 0)
            {
				bool isSuccess = false;
				if( nEquipZoneID > 0)
					isSuccess = InsertEquipZoneFacilityManager(dbMgr, nEquipZoneID);

				if( isSuccess == false )
					isSuccess = nBuildingID != 0 ? InsertBuildingFacilityManager(dbMgr, nBuildingID) : InsertFacilityManager(dbMgr);

                if (isSuccess)
                    PageBackstageHome.Instance.IsChangedFacilityManager = true;

                return isSuccess;
            }

            string strSetting = "";

            if (m_memberID != null)
                AddQueryString(ref strSetting, "MemberID = " + m_memberID.Data.ToString());
            
            if (m_memberType != null)
                AddQueryString(ref strSetting, "MemberType = " + m_memberType.Data.ToString());

            if (m_facilityType != null)
                AddQueryString(ref strSetting, "FacilityType = " + m_facilityType.Data.ToString());

            if (m_levelLimit != null)
                AddQueryString(ref strSetting, "LevelLimit = " + m_levelLimit.Data.ToString());

			if( m_nUpperLimit != null)
				AddQueryString(ref strSetting, "UpperLimit = " + m_nUpperLimit.Data.ToString());

            if (m_description != null)
                AddQueryString(ref strSetting, "Description = '" + m_description.Data.ToString() + "'");

            if (strSetting.Length != 0)
            {
                string strSQL = string.Format("Update {0} set {1} where ID = {2}", strTableName, strSetting, ID);
                bool isSuccess = dbMgr.GetResultData(strSQL, 0) != null;

                if (isSuccess)
                    PageBackstageHome.Instance.IsChangedFacilityManager = true;

                return isSuccess;
            }

            return true;
        }

        private bool InsertFacilityManager(DBUtility.WebDBManager dbMgr)
        {
            ArrayList arrResult = dbMgr.GetResultData("Select max(id) from FacilityManager", 0);
            if (arrResult == null)
                return false;

            int nID = arrResult.Count == 0 ? 1 : DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;

			string strFormat = "Insert into FacilityManager (ID, MemberID, MemberType, FacilityType, LevelLimit, UpperLimit, Description) values ";
            strFormat += "({0}, {1}, {2}, {3}, {4}, {5}, '{6}')";

			string strSQL = string.Format(strFormat, nID, m_mgr.MemberID, m_mgr.MemberType, m_facilityType.Data, m_mgr.LevelLimit, m_mgr.UpperLimit,  m_mgr.Description);

            if (dbMgr.GetResultData(strSQL, 0) == null)
                return false;

            m_mgr.ID = nID;

            FormMain.Instance.DataManager.AddFacilityManager(m_mgr, m_mgr.Type);

            return true;
        }

		private bool InsertEquipZoneFacilityManager(DBUtility.WebDBManager dbMgr, int nEquipZone)
		{
			ArrayList arrResult = dbMgr.GetResultData("Select max(id) from EquipZoneFacilityManager", 0);
			if (arrResult == null)
				return false;

			int nID = arrResult.Count == 0 ? 1 : DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;

			string strFormat = "Insert into EquipZoneFacilityManager (ID, MemberID, MemberType, FacilityType, LevelLimit, EquipZoneID, UpperLimit, Description) values ";
			strFormat += "({0}, {1}, {2}, {3}, {4}, {5}, {6}, '{7}')";

			string strSQL = string.Format(strFormat, nID, m_mgr.MemberID, m_mgr.MemberType, m_facilityType.Data, m_mgr.LevelLimit, nEquipZone, m_mgr.UpperLimit, m_mgr.Description);

			if (dbMgr.GetResultData(strSQL, 0) == null)
				return false;

			m_mgr.ID = nID;

			FormMain.Instance.DataManager.AddEquipZoneFacilityManager(m_mgr, m_mgr.EquipZone, m_mgr.Type);

			return true;
		}

        private bool InsertBuildingFacilityManager(DBUtility.WebDBManager dbMgr, int nBuildingID)
        {
            ArrayList arrResult = dbMgr.GetResultData("Select max(id) from BuildingFacilityManager", 0);
            if (arrResult == null)
                return false;

            int nID = arrResult.Count == 0 ? 1 : DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;

            string strFormat = "Insert into BuildingFacilityManager (ID, MemberID, MemberType, FacilityType, LevelLimit, BuildingID, UpperLimit, Description) values ";
            strFormat += "({0}, {1}, {2}, {3}, {4}, {5}, {6}, '{7}')";

            string strSQL = string.Format(strFormat, nID, m_mgr.MemberID, m_mgr.MemberType, m_facilityType.Data, m_mgr.LevelLimit, nBuildingID, m_mgr.UpperLimit, m_mgr.Description);

            if (dbMgr.GetResultData(strSQL, 0) == null)
                return false;

            m_mgr.ID = nID;

            if (m_mgr.Building != null)
                FormMain.Instance.DataManager.AddBuildingFacilityManager(m_mgr, m_mgr.Building, m_mgr.Type);
            else// if (m_mgr.Zone != null)
                FormMain.Instance.DataManager.AddOutdoorFacilityManager(m_mgr, m_mgr.Zone, m_mgr.Type);

            return true;
        }

        // 이미 같은 데이터가 편집되었으면, 해당 데이터의 내용과 합친다.
        public override void AddToManager(IChangedDataManager mgr)
        {
            ArrayList arrDatas = mgr.GetDataList();
            Type type = typeof(EditFacilityManager);

            foreach (ChangedData data in arrDatas)
            {
                if (data.GetType() == type)
                {
                    EditFacilityManager facility = (EditFacilityManager)data;


					bool bSameEquipZone = false;
					if (facility.Manager.EquipZone != null)
					{
						if (this.Manager.EquipZone != null)
						{
							if (facility.Manager.EquipZone.ID == this.Manager.EquipZone.ID)
								bSameEquipZone = true;
						}
					}
					else
					{
						if (this.Manager.EquipZone == null)
						{
							bSameEquipZone = true;
						}
					}

                    if (facility.Manager.Building == this.Manager.Building && 
                        facility.Manager.Zone == this.Manager.Zone &&						
						bSameEquipZone == true &&
                        facility.Manager.Type == this.Manager.Type &&
                        facility.Manager.MemberID == this.Manager.MemberID &&
                        facility.Manager.MemberType == this.Manager.MemberType &&
                        facility.Manager.LevelLimit == this.Manager.LevelLimit &&
                        facility.Manager.UpperLimit == this.Manager.UpperLimit)
                        //facility.Manager.Tag == this.Manager.Tag)
                    {
                        facility.m_isDeleting = this.m_isDeleting;

                        if (facility.ID < 0)
                        {
                            if (facility.m_isDeleting)
                            {
                                arrDatas.Remove(data);
                                mgr.SomethingChanged(null);
                                return;
                            }
                        }

                        if (this.m_memberID != null)
                            facility.m_memberID = this.m_memberID;
                        if (this.m_memberType != null)
                            facility.m_memberType = this.m_memberType;
                        if (this.m_facilityType != null)
                            facility.m_facilityType = this.m_facilityType;
                        if (this.m_levelLimit != null)
                            facility.m_levelLimit = this.m_levelLimit;
                        if (this.m_description != null)
                            facility.m_description = this.m_description;
						if (this.m_nUpperLimit != null)
							facility.m_nUpperLimit = this.m_nUpperLimit;
                        return;
                    }
                }
            }

            // DB에 저장된 것과 동일한 상태인가?
            //if (!IsOriginStatus())
                mgr.SomethingChanged(this);
        }
    }
}
