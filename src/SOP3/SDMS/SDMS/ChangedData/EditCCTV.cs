using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

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

        public new bool IsDeleting
        {
            get { return m_isDeleting; }
            set { m_isDeleting = value; }
        }

        public int ID
        {
            get { return m_cctv == null ? -1 : m_cctv.ID; }
        }

        public SDMS.CCTV CCTV
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

        public SDMS.Zone Zone
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

        private bool Insert(DBUtility.WebDBManager dbMgr)
        {
            POI poi = m_cctv.POI;
            if (poi == null)
                return false;

            Zone zone = poi.Zone;
            if (zone == null)
                return false;

            string strSQL = "Select max(id) from CCTV";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nID = -1;

            if (arrResult.Count == 0)
                nID = 1;
            else
                nID = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;

            strSQL = string.Format("Insert into CCTV (ID, CameraName, IPAddr, Port, PositionName, X, Y, Z, ZoneID, IsIndoor, LOD, Description) values ({0}, '{1}', '{2}', {3}, '{4}', {5}, {6}, {7}, {8}, {9}, {10}, NULL)",
                nID, m_cctv.AccessKey, m_cctv.IPAddress, m_cctv.PortNo, zone.BroadcastName, poi.X, poi.Y, poi.Z, zone.ID, poi.IsIndoor ? 1 : 0, (int)m_cctv.LODType);

            if (dbMgr.GetResultData(strSQL, 0) != null)
            {
                m_cctv.ID = nID;
                poi.UpdateDBData();
                return true;
            }

            return false;
        }

        public override bool Update(DBUtility.WebDBManager dbMgr)
        {
            if (m_cctv == null)
                return false;

			if (m_cctv.ID < 0 && IsDeleting == false)
                return Insert(dbMgr);

            if (m_isDeleting)
            {
                string strSQL = string.Format("Delete from CCTV where ID = {0}", m_cctv.ID);
                return dbMgr.GetResultData(strSQL, 0) != null;
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
                strValue = string.Format("PositionName = '{0}', ZoneID = {1}, IsIndoor = {2}", m_zone.BroadcastName, m_zone.ID, m_cctv.POI.IsIndoor ? 1 : 0);
                AddQueryString(ref strField, strValue);
            }

            if (m_pos != null)
            {
                strValue = string.Format("X = {0}, Y = {1}, Z = {2}", m_pos.x, m_pos.y, m_pos.z);
                AddQueryString(ref strField, strValue);
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

                if (m_lod.Data == 0)
                {
                    Core.Layer layer = FormMain.Instance.PageHome.ContentForm.Layers.GetLayer(SDMS.ID.ID_LAYER_CCTV);

                    if (layer != null)
                        layer.Remove(m_cctv.POI.ID);

                    layer = FormMain.Instance.PageHome.ContentForm.Layers.GetLayer(SDMS.ID.ID_LAYER_CCTVLOW);

                    if (layer != null)
                        layer.Add(m_cctv.POI.ID);
                }
                else
                {
                    Core.Layer layer = FormMain.Instance.PageHome.ContentForm.Layers.GetLayer(SDMS.ID.ID_LAYER_CCTVLOW);

                    if (layer != null)
                        layer.Remove(m_cctv.POI.ID);

                    layer = FormMain.Instance.PageHome.ContentForm.Layers.GetLayer(SDMS.ID.ID_LAYER_CCTV);

                    if (layer != null)
                        layer.Add(m_cctv.POI.ID);
                }
            }

            if (strField.Length == 0)
                return false;

            string strSQL2 = string.Format("Update CCTV set {0} where id = {1}", strField, m_cctv.ID);

            if (dbMgr.GetResultData(strSQL2, 0) != null)
            {
                m_cctv.POI.UpdateDBData();
                return true;
            }

            return false;
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
        private bool IsOriginStatus()
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

            return true;
        }
    }
}
