using System;
using System.Collections;
using DBUtility2;
using UnE.Spatial;
using UnE.Sensor;

namespace SDMS
{
	public class EditCCTV : ChangedData
	{
		// m_isDeleting이 true이면 다른 인자는 모두 무시하고 m_nID에 해당하는 CCTV를 삭제한다.
		private bool m_isDeleting = false;

		private VariousData<string> m_ipAddr = null;
		private VariousData<int> m_port = null;
		private Zone m_zone = null;
		private UnE.Geometry.Vertex3F m_pos = null;
		private VariousData<string> m_description = null;
		private VariousData<string> m_cameraName = null;
		private VariousData<int> m_lod = null;
		private CCTV m_cctv = null;

        private VariousData<string> m_UserName = null;
        private VariousData<string> m_Password = null;
        private VariousData<int> m_Channel = null;
        private VariousData<int> m_Stream = null;
        private VariousData<int> m_cctyType = null;

        public int CCTVType
        {
             set { m_cctyType = new VariousData<int>(value); }
        }


        public string UserName
        {
            set { m_UserName = new VariousData<string>(value); }
        }

        public string Password
        {
            set { m_Password = new VariousData<string>(value); }
        }
        
        public int Channel
        {
            set { m_Channel = new VariousData<int>(value); }
        }

        public int Stream
        {
            set { m_Stream = new VariousData<int>(value); }
        }

		public new bool IsDeleting
		{
			get { return m_isDeleting; }
			set { m_isDeleting = value; }
		}

		public int ID
		{
			get { return m_cctv == null ? -1 : m_cctv.ID; }
		}

		public CCTV CCTV
		{
			get { return m_cctv; }
			set { m_cctv = value; }
		}

		public string IPAddr
		{
			set { m_ipAddr = new VariousData<string>(value); }
		}

		public int Port
		{
			set { m_port = new VariousData<int>(value); }
		}

		public Zone Zone
		{
			get { return m_zone; }
			set { m_zone = value; }
		}

		public UnE.Geometry.Vertex3F Position
		{
			get { return m_pos; }
			set { m_pos = value; }
		}

		public string Description
		{
			set { m_description = new VariousData<string>(value); }
		}

		public string CameraName
		{
			set { m_cameraName = new VariousData<string>(value); }
		}

		public int LOD
		{
			set { m_lod = new VariousData<int>(value); }
		}

		public EditCCTV(CCTV cctv)
		{
			m_cctv = cctv;

			m_zone = cctv.POI.Zone;
			Position = new UnE.Geometry.Vertex3F(cctv.POI.X, cctv.POI.Y, cctv.POI.Z);
		}

		private bool Insert(WebDBManager dbMgr)
		{
			POI poi = m_cctv.POI;
			if (poi == null)
				return false;

			Zone zone = poi.Zone;
			if (zone == null)
				return false;

			string strSQL = "Select max(id) from CCTV";
			ArrayList arrResult = dbMgr.GetResultData(strSQL);

			if (arrResult == null)
				return false;

			int nID = -1;

			if (arrResult.Count == 0)
				nID = 1;
			else
				nID = WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;

            try
            {
                strSQL = string.Format("Insert into CCTV (ID, CameraName, IPAddr, Port, PositionName, X, Y, Z, ZoneID, IsIndoor, LOD, Description , HTTPPort, Type,  Stream, Channel, UserID,  Password ) " +
                   "values ({0}, '{1}', '{2}', {3}, '{4}', {5}, {6}, {7}, {8}, {9}, {10}, NULL, {11}, '{12}', {13}, {14}, '{15}','{16}')",
                   nID,                    // 0
                   m_cctv.AccessKey,       // 1
                   m_cctv.IPAddress,       // 2 
                   m_cctv.PortNo,          // 3
                   zone.DisplayText,       // 4
                   (poi.X * 1000),         // 5
                   (-poi.Z * 1000),        // 6
                   poi.Z,                  // 7
                   zone.ID,                // 8
                   (poi.IsIndoor ? 1 : 0), // 9
                   ((int)m_cctv.LODType),  // 10
                   80,                     // 11
                   m_cctv.CCTVType,        // 12
                   m_cctv.Stream,          // 13
                   m_cctv.Channel,         // 14
                   m_cctv.UserName,        // 15
                   m_cctv.Password);       // 16
            }
            catch(Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
            }
           

			if (dbMgr.GetResultData(strSQL) != null)
			{
				m_cctv.ID = nID;
				poi.UpdateDBData();
				return true;
			}

			return false;
		}

		public override bool Update(WebDBManager dbMgr)
		{
			if (m_cctv == null)
				return false;

			if (m_cctv.ID < 0 && IsDeleting == false)
				return Insert(dbMgr);

			if (m_isDeleting)
			{
				string strSQL = string.Format("Delete from CCTV where ID = {0}", m_cctv.ID);
				return dbMgr.GetResultData(strSQL) != null;
			}

			string strField = "", strValue = "";

			if (m_ipAddr != null)
			{
				strValue = string.Format("IPAddr = '{0}'", m_ipAddr.Data);
				AddQueryString(ref strField, strValue);
			}

			if (m_port != null)
			{
				strValue = string.Format("Port = {0}", m_port.Data);
				AddQueryString(ref strField, strValue);
			}
      
			if (m_zone != null)
			{
                strValue = string.Format("PositionName = '{0}', ZoneID = {1}, IsIndoor = {2}", m_zone.DisplayText, m_zone.ID, m_cctv.POI.IsIndoor ? 1 : 0);
				AddQueryString(ref strField, strValue);
			}

			if (m_pos != null)
			{
                
                if (UnE.SOP.ProxySOP.Instance.SiteID == 1)
                {
                    strValue = string.Format("X = {0}, Y = {1}, Z = {2}", m_pos.x * 1000, m_pos.z * -1000, m_pos.y);
                    AddQueryString(ref strField, strValue);
                }
                else if (UnE.SOP.ProxySOP.Instance.SiteID == 2)
                {
                    strValue = string.Format("X = {0}, Y = {1}, Z = {2}", m_pos.x * 1000, m_pos.z * -1000, m_pos.y);
                    AddQueryString(ref strField, strValue);
                }
                else if ((UnE.SOP.ProxySOP.Instance.SiteID == 100) || (UnE.SOP.ProxySOP.Instance.SiteID == 101))
                {
                    strValue = string.Format("X = {0}, Y = {1}, Z = {2}", m_pos.x , m_pos.y , m_pos.z);
                    AddQueryString(ref strField, strValue);
                }
                else if(UnE.SOP.ProxySOP.Instance.SiteID == 3)
                {
                    strValue = string.Format("X = {0}, Y = {1}, Z = {2}", m_pos.x , m_pos.y , m_pos.z);
                    AddQueryString(ref strField, strValue);
                }	
                else
                {
                    strValue = string.Format("X = {0}, Y = {1}, Z = {2}", m_pos.x * 1000, m_pos.z * -1000, m_pos.y);
                    AddQueryString(ref strField, strValue);
                }				
			}

			if (m_description != null)
			{
				strValue = string.Format("Description = '{0}'", m_description.Data);
				AddQueryString(ref strField, strValue);
			}

			if (m_cameraName != null)
			{
				strValue = string.Format("CameraName = '{0}'", m_cameraName.Data);
				AddQueryString(ref strField, strValue);
			}

			if (m_lod != null)
			{
				strValue = string.Format("LOD = {0}", m_lod.Data);
				AddQueryString(ref strField, strValue);

				/*if (m_lod.Data == 0)
				{
					Core.Layer layer = FormMain.Instance.PageHome.ContentForm.Layers.GetLayer(SDMS.ID.ID_LAYER_CCTV);

					if (layer != null)
						layer.Remove(m_cctv.POI.ID);

					layer = FormMain.Instance.PageHome.ContentForm.Layers.GetLayer(SDMS.ID.ID_LAYER_CCTVLOW);

					if (layer != null)
						layer.Add(m_cctv.POI.ID);
				}
				else*/
				{
                    UnE.View.Content.ILayer layer = UnE.View.Content.ViewUtils.GetContentView().Layers.GetLayer(SDMS.ID.ID_LAYER_CCTVLOW);

					if (layer != null)
						layer.Remove(m_cctv.POI.ID);

                    layer = UnE.View.Content.ViewUtils.GetContentView().Layers.GetLayer(SDMS.ID.ID_LAYER_CCTV);

					if (layer != null)
                        layer.Remove(m_cctv.POI.ID);

                    layer = UnE.View.Content.ViewUtils.GetContentView().Layers.GetLayer(SDMS.ID.ID_LAYER_CCTV_DISCONNECTED);

                    if (layer != null)
                        layer.Remove(m_cctv.POI.ID);


                    if (m_cctv.LODType == CCTV.LOD.DEFAULT)
                    {
                        layer = UnE.View.Content.ViewUtils.GetContentView().Layers.GetLayer(SDMS.ID.ID_LAYER_CCTV);
                    }

                    if (layer != null)
                    {
                        layer.Add(m_cctv.POI.ID);
                    }



				}
			}

            if( m_UserName != null)
            {
                strValue = string.Format("UserName = '{0}'", m_UserName.Data);
				AddQueryString(ref strField, strValue);
            }
            if( m_Password != null)
            {
                strValue = string.Format("Password = '{0}'", m_Password.Data);
				AddQueryString(ref strField, strValue);
            }
            if( m_Channel != null)
            {
                strValue = string.Format("Channel = '{0}'", m_Channel.Data);
				AddQueryString(ref strField, strValue);
            }
            if( m_Stream != null)
            {
                strValue = string.Format("Stream = '{0}'", m_Stream.Data);
				AddQueryString(ref strField, strValue);
            }
            if( m_cctyType != null)
            {
                strValue = string.Format("Type = '{0}'", GetCCTVType(m_cctyType.Data));
				AddQueryString(ref strField, strValue);
            }

			if (strField.Length == 0)
				return false;

			string strSQL2 = string.Format("Update CCTV set {0} where id = {1}", strField, m_cctv.ID);

			if (dbMgr.GetResultData(strSQL2) != null)
			{
				m_cctv.POI.UpdateDBData();
				return true;
			}

			return false;
		}

        private string GetCCTVType(int cctvType)
        {
            string strCCTVType = String.Empty;

            switch (cctvType)
            {
                case 1 :
                    strCCTVType = "Axis";
                    break;
                case 2:
                    strCCTVType = "NVS";
                    break;
                case 3:
                    strCCTVType = "XpressStrm";
                    break;
                case 4:
                    strCCTVType = "UDP";
                    break;
                case 5:
                    strCCTVType = "Panasonic";
                    break;
                case 6:
                    strCCTVType = "iPolis";
                    break;
                case 7:
                    strCCTVType = "IPVideo";
                    break;
                case 8:
                    strCCTVType = "HIK";
                    break;
                case 9:
                    strCCTVType = "NVT";
                    break;
                case 10:
                    strCCTVType = "IDIS";
                    break;
                default :
                    break;
            }

            return strCCTVType;
        }

		// 이미 같은 데이터가 편집되었으면, 해당 데이터의 내용과 합친다.
		public override void AddToManager(IChangedDataManager mgr)
		{
			ArrayList arrDatas = mgr.GetDataList();
			Type type = typeof(EditCCTV);

			foreach (ChangedData data in arrDatas)
			{
				if (data.GetType() == type)
				{
					EditCCTV cctv = (EditCCTV)data;

					if (cctv.m_cctv == this.m_cctv)
					{
						cctv.m_isDeleting = this.m_isDeleting;

						if (cctv.ID < 0)
						{
							if (cctv.m_isDeleting)
							{
								arrDatas.Remove(data);
								mgr.SomethingChanged(null);
								return;
							}
						}

						if (this.m_ipAddr != null)
							cctv.m_ipAddr = this.m_ipAddr;
						if (this.m_port != null)
							cctv.m_port = this.m_port;
						if (this.m_zone != null)
							cctv.m_zone = this.m_zone;
						if (this.m_pos != null)
							cctv.m_pos = this.m_pos;
						if (this.m_description != null)
							cctv.m_description = this.m_description;
						if (this.m_cameraName != null)
							cctv.m_cameraName = this.m_cameraName;
						if (this.m_lod != null)
							cctv.m_lod = this.m_lod;
                        if (this.m_UserName != null)
                            cctv.m_UserName = this.m_UserName;
                        if (this.m_Password != null)
                            cctv.m_Password = this.m_Password;
                        if (this.m_Channel != null)
                            cctv.m_Channel = this.m_Channel;
                        if (this.m_Stream != null)
                            cctv.m_Stream = this.m_Stream;
                        if (this.m_cctyType != null)
                            cctv.m_cctyType = this.m_cctyType;

						/*if (!cctv.m_isDeleting && cctv.IsOriginStatus())
							arrDatas.Remove(data);*/

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

		// DB에 저장된 것과 동일한 상태인가?
        public override bool IsOriginStatus()
		{
			if (m_isDeleting)
				return false;

			if (m_cctv == null)
				return false;

			if (this.m_ipAddr != null)
			{
				if (this.m_ipAddr.Data != m_cctv.IPAddressDB)
					return false;
			}

			if (this.m_port != null)
			{
				if (this.m_port.Data != m_cctv.PortNoDB)
					return false;
			}

			if (this.m_zone != null)
			{
				if (this.m_zone != m_cctv.POI.ZoneDB)
					return false;
			}

			if (this.m_pos != null)
			{
				if (this.m_pos.GetDistance(new UnE.Geometry.Vertex3F(m_cctv.POI.XDB, m_cctv.POI.YDB, m_cctv.POI.ZDB)) >
					UnE.Geometry.Math.HALF_TOLERANCE())
					return false;
			}

			if (this.m_description != null)
			{
				return false;
			}

			if (this.m_cameraName != null)
			{
				if (this.m_cameraName.Data != m_cctv.AccessKeyDB)
					return false;
			}

			if (this.m_lod != null)
			{
				if (this.m_lod.Data != (int)m_cctv.LODTypeDB)
					return false;
			}

            if (this.m_UserName != null)
            {
                if (this.m_UserName.Data != m_cctv.UserNameDB)
                    return false;
            }
            if (this.m_Password != null)
            {
                if (this.m_Password.Data != m_cctv.PasswordDB)
                    return false;
            }
            if (this.m_Channel != null)
            {
                if (this.m_Channel.Data != m_cctv.ChannelDB)
                    return false;
            }
            if (this.m_Stream != null)
            {
                if (this.m_Stream.Data != m_cctv.StreamDB)
                    return false;
            }
            if (this.m_cctyType != null)
            {
                if (this.m_cctyType.Data != m_cctv.CCTVTypeDB)
                    return false;
            }
			return true;
		}
	}
}