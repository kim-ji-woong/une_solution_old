using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDMS
{
    public class CCTVManager
    {
        private static CCTVManager m_Instance = null;
        public static CCTVManager Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new CCTVManager();
                return m_Instance;
            }
        }

        private ArrayList m_IndoorCCTVList = new ArrayList();
        public System.Collections.ArrayList IndoorCCTVList
        {
            get { return m_IndoorCCTVList; }
        }

        private ArrayList m_OutdoorCCTVList = new ArrayList();
        public System.Collections.ArrayList OutdoorCCTVList
        {
            get { return m_OutdoorCCTVList; }
        }

        // 설비영역별 4개의 CCTV
        private Dictionary<EquipmentZone, CCTV[]> m_dicEquipZoneCCTVs = new Dictionary<EquipmentZone, CCTV[]>();
        // 현재 DB에 저장되어 있는 설비영역별 4개의 CCTV
        private Dictionary<EquipmentZone, CCTV[]> m_dicDBEquipZoneCCTVs = new Dictionary<EquipmentZone, CCTV[]>();
        // ID별 CCTV
        private Dictionary<int, CCTV> m_dicCCTVs = new Dictionary<int, CCTV>();
        
        private CCTVManager()
        {

        }

        public CCTV GetCCTV(int nID)
        {
            if (!m_dicCCTVs.ContainsKey(nID))
                return null;

            return m_dicCCTVs[nID];
        }

        public bool LoadEquipZoneCCTV(int nEquipZone = -1)
        {
            DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;


            string strSQL = "Select ID, EquipZoneID, CCTV1, CCTV2, CCTV3, CCTV4, description from EquipZoneCCTV";

			if (nEquipZone != -1)
			{
				strSQL += string.Format(" where EquipZoneID = {0}", nEquipZone);
			}
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 6; i += 7)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nEquipZoneID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                int nCCTV1 = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nCCTV2 = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nCCTV3 = DBUtility.WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nCCTV4 = DBUtility.WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                //string strDescription = DBUtility.WebDBManager.GetStringField(arrResult[i + 6].ToString(), "");

                if (nID <= 0)
                    continue;

                EquipmentZone equipZone = ZoneManager.Instance.GetEquipZone(nEquipZoneID);

                if (equipZone == null)
                    continue;

                CCTV[] arrCCTV = new CCTV[4];
                m_dicEquipZoneCCTVs[equipZone] = arrCCTV;

                CCTV[] arrDBCCTV = new CCTV[4];
                m_dicDBEquipZoneCCTVs[equipZone] = arrDBCCTV;

                arrDBCCTV[0] = arrCCTV[0] = GetCCTV(nCCTV1);
                arrDBCCTV[1] = arrCCTV[1] = GetCCTV(nCCTV2);
                arrDBCCTV[2] = arrCCTV[2] = GetCCTV(nCCTV3);
                arrDBCCTV[3] = arrCCTV[3] = GetCCTV(nCCTV4);
            }

            return true;
        }

        public bool LoadCCTV(BaseViewEx view, bool isIndoor)
        {
            DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;

            string strSQL = "Select ID, CameraName, IPAddr, Port, X, Y, Z, ZoneID, LOD from CCTV where IsIndoor = " + (isIndoor ? "1" : "0");
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 8; i += 9)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                string strCameraName = DBUtility.WebDBManager.GetStringField(arrResult[i + 1], "");
                string strIPAddr = DBUtility.WebDBManager.GetStringField(arrResult[i + 2], "");
                int nPort = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                float x = DBUtility.WebDBManager.GetFloatField(arrResult[i + 4].ToString(), 0.0f);
                float y = DBUtility.WebDBManager.GetFloatField(arrResult[i + 5].ToString(), 0.0f);
                float z = DBUtility.WebDBManager.GetFloatField(arrResult[i + 6].ToString(), 0.0f);
                int nZoneID = DBUtility.WebDBManager.GetIntField(arrResult[i + 7].ToString(), -1);
                int nLOD = DBUtility.WebDBManager.GetIntField(arrResult[i + 8].ToString(), -1);

                if (nLOD < (int)CCTV.LOD.LOW || nLOD > (int)CCTV.LOD.VERY_IMPORTANT)
                    continue;

                Zone zone = ZoneManager.Instance.GetZone(nZoneID);

                CCTV cctv = new CCTV();

                cctv.ID = nID;
                cctv.AccessKey = strCameraName;
                cctv.IPAddress = strIPAddr;
                cctv.PortNo = (short)nPort;
                cctv.POI = new POI();
                cctv.POI.X = x;
                cctv.POI.Y = y;
                cctv.POI.Z = z;
                cctv.POI.Zone = zone;
                cctv.POI.IsIndoor = isIndoor;
                cctv.LODType = (CCTV.LOD)nLOD;

                /// <summary>
                /// DB에 저장되어 있는 값을 기억시키기 위한 데이터
                cctv.AccessKeyDB = strCameraName;
                cctv.IPAddressDB = strIPAddr;
                cctv.PortNoDB = (short)nPort;
                cctv.POI.XDB = x;
                cctv.POI.YDB = y;
                cctv.POI.ZDB = z;
                cctv.POI.ZoneDB = zone;
                cctv.LODTypeDB = (CCTV.LOD)nLOD;
                /// </summary>

                if (isIndoor == true)
                {
                    m_IndoorCCTVList.Add(cctv);
                }
                else
                {
                    m_OutdoorCCTVList.Add(cctv);
                }
                if (cctv.LODType == CCTV.LOD.LOW)
                {
                    int dd = 0;
                    dd++;
                }
                view.AddPOI(cctv.POI );
                if( cctv.LODType == CCTV.LOD.LOW)
                    view.ShowIconPOI(cctv.POI.ID, false);

                m_dicCCTVs[nID] = cctv;
            }

            return true;
        }

        // Zone 내부에 CCTV 를 검색
        private ArrayList FindCCTV(int nZoneID)
        {
            ArrayList cctvInZone = new ArrayList();

            foreach (CCTV cctv in m_OutdoorCCTVList)
            {
                if (nZoneID == cctv.POI.Zone.ID)
                {
                    cctvInZone.Add(cctv);
                }
            }
          
            foreach (CCTV cctv in m_IndoorCCTVList)
            {
                if (nZoneID == cctv.POI.Zone.ID)
                {
                    cctvInZone.Add(cctv);
                }
            }
            return  cctvInZone;
        }

        /*public ArrayList GetAutoCCTVList(Zone zone)
        {
            int nCameraCount = 15;
            ArrayList arResult = new ArrayList();

            // 인접 존을 찾는다. 
            if (zone.IsOutdoor == true)
            {
                // 해당 존이 1층인지?  -> 인접존을 탐색한다. 최대 10개까지( 중심거리탐색 )
                ArrayList arZones = ZoneManager.Instance.FindNearZone(zone.ID, false, nCameraCount);
                foreach (Zone ozone in arZones)
                {
                    // 있는 경우 CCTV가 4 일때까지 반복하여 인접구역을 찾는다.
                    ArrayList cctvs = FindCCTV(ozone.ID);

                    if ((cctvs.Count + arResult.Count) >= nCameraCount)
                    {
                        int nCount = nCameraCount - arResult.Count;
                        for (int i = 0; i < nCount; i++)
                            arResult.Add(cctvs[i]);
                        return arResult;
                    }
                    else
                    {
                        arResult.AddRange(cctvs);
                    }
                }
            }
            else
            {
                // 해당 존이 1층이 아니면  같은 층에 인접존이 있는지 여부를 검색
                ArrayList arZones = ZoneManager.Instance.FindNearZone(zone.ID, true, nCameraCount);
                foreach (Zone ozone in arZones)
                {
                    // 있는 경우 CCTV가 4 일때까지 반복하여 인접구역을 찾는다.
                    ArrayList cctvs = FindCCTV(ozone.ID);

                    if ((cctvs.Count + arResult.Count) >= nCameraCount)
                    {
                        int nCount = nCameraCount - arResult.Count;
                        for (int i = 0; i < nCount; i++)
                            arResult.Add(cctvs[i]);
                        return arResult;
                    }
                    else
                    {
                        arResult.AddRange(cctvs);
                    }
                }

                // 인접 존이 없는 경우 위층과 아래층을 구한다.
                Building building = zone.Building;
                Zone zFirst = null;
                Zone zLast = null;

                Zone zoneUnder = ZoneManager.Instance.GetZone(building.BuildingID, zone.FloorIndex - 1);
                Zone zoneUp = ZoneManager.Instance.GetZone(building.BuildingID, zone.FloorIndex + 1);
                if (zone.FloorIndex < 0)
                {
                    zFirst = zoneUp;
                    zLast = zoneUnder;
                }
                else
                {
                    zFirst = zoneUnder;
                    zLast = zoneUp;
                }

                if (zFirst != null)
                {
                    ArrayList arCCTV = FindCCTV(zFirst.ID);
                    if ((arCCTV.Count + arResult.Count) >= nCameraCount)
                    {
                        int nCount = nCameraCount - arResult.Count;
                        for (int i = 0; i < nCount; i++)
                            arResult.Add(arCCTV[i]);
                        return arResult;
                    }
                    else
                    {
                        arResult.AddRange(arCCTV);
                    }
                }

                if (zLast != null)
                {
                    ArrayList arCCTV = FindCCTV(zLast.ID);
                    if ((arCCTV.Count + arResult.Count) >= nCameraCount)
                    {
                        int nCount = nCameraCount - arResult.Count;
                        for (int i = 0; i < nCount; i++)
                            arResult.Add(arCCTV[i]);
                        return arResult;
                    }
                    else
                    {
                        arResult.AddRange(arCCTV);
                    }
                }

                // 외부에서 가까운것을 찾는다. 4개를 찾을 때까지 반복
                ArrayList arOutZones = ZoneManager.Instance.FindNearZone(zone.ID, false, 50);
                foreach (Zone ozone in arOutZones)
                {
                    // 있는 경우 CCTV가 4 일때까지 반복하여 인접구역을 찾는다.
                    ArrayList cctvs = FindCCTV(ozone.ID);

                    if ((cctvs.Count + arResult.Count) >= nCameraCount)
                    {
                        int nCount = nCameraCount - arResult.Count;
                        for (int i = 0; i < nCount; i++)
                            arResult.Add(cctvs[i]);
                        return arResult;
                    }
                    else
                    {
                        arResult.AddRange(cctvs);
                    }
                }
            }

            return arResult;
        }*/

        public ArrayList AutoPopupCCTV(Zone zone)        
        {
            ArrayList arResult = new ArrayList();

            int nICount = m_IndoorCCTVList.Count;
            int nOCount = m_OutdoorCCTVList.Count;

            // 전체 CCVT가 4개이상인지 
            // 이하이면 모두 리턴
            if (nICount + nOCount <= 4)
            {
                arResult.AddRange(m_IndoorCCTVList);
                arResult.AddRange(m_OutdoorCCTVList);
                return arResult;
            }
            
            // 화재 발생 존에 있는 CCTV를 가져온다.
            int nZoneID = zone.ID;
            ArrayList cctvInZone = FindCCTV(nZoneID);
           
            // CCTV 가 4개 이상인지?            
            if (cctvInZone.Count >= 4)
            {
                // 4 개를 랜덤하게 선택하여 리턴
                for (int i = 0; i < 4; i++)
                    arResult.Add(cctvInZone[i]);
                return arResult;
            }
            else
            {
                arResult.AddRange(cctvInZone);
            }
            
            // 인접 존을 찾는다. 
            if (zone.IsOutdoor == true)
            {
                // 해당 존이 1층인지?  -> 인접존을 탐색한다. 최대 10개까지( 중심거리탐색 )
                ArrayList arZones = ZoneManager.Instance.FindNearZone(zone.ID, false, 10);
                foreach ( Zone ozone in arZones)
                {
                    // 있는 경우 CCTV가 4 일때까지 반복하여 인접구역을 찾는다.
                    ArrayList cctvs = FindCCTV(ozone.ID);
                    
                    if ((cctvs.Count + arResult.Count) >= 4)
                    {
                        int nCount = 4 - arResult.Count;
                        for (int i = 0; i < nCount; i++)
                            arResult.Add(cctvs[i]);
                        return arResult;
                    }
                    else
                    {
                        arResult.AddRange(cctvs);
                    }
                }
            }
            else
            {
                // 해당 존이 1층이 아니면  같은 층에 인접존이 있는지 여부를 검색
                ArrayList arZones = ZoneManager.Instance.FindNearZone(zone.ID, true, 10);
                foreach (Zone ozone in arZones)
                {
                    // 있는 경우 CCTV가 4 일때까지 반복하여 인접구역을 찾는다.
                    ArrayList cctvs = FindCCTV(ozone.ID);

                    if ((cctvs.Count + arResult.Count) >= 4)
                    {
                        int nCount = 4 - arResult.Count;
                        for (int i = 0; i < nCount; i++)
                            arResult.Add(cctvs[i]);
                        return arResult;
                    }
                    else
                    {
                        arResult.AddRange(cctvs);
                    }
                }

                // 인접 존이 없는 경우 위층과 아래층을 구한다.
                Building building = zone.Building;
                Zone zFirst = null;
                Zone zLast = null;
                
                Zone zoneUnder = ZoneManager.Instance.GetZone(building.BuildingID, zone.FloorIndex - 1);
                Zone zoneUp = ZoneManager.Instance.GetZone(building.BuildingID, zone.FloorIndex + 1);
                if (zone.FloorIndex < 0)
                {
                    zFirst = zoneUp;
                    zLast = zoneUnder;
                }
                else
                {
                    zFirst = zoneUnder;
                    zLast = zoneUp;
                }

                if (zFirst != null)
                {
                    ArrayList arCCTV = FindCCTV(zFirst.ID);
                    if ((arCCTV.Count + arResult.Count) >= 4)
                    {
                        int nCount = 4 - arResult.Count;
                        for (int i = 0; i < nCount; i++)
                            arResult.Add(arCCTV[i]);
                        return arResult;
                    }
                    else
                    {
                        arResult.AddRange(arCCTV);
                    }
                }
                
                if (zLast != null)
                {
                    ArrayList arCCTV = FindCCTV(zLast.ID);
                    if ((arCCTV.Count + arResult.Count) >= 4)
                    {
                        int nCount = 4 - arResult.Count;
                        for (int i = 0; i < nCount; i++)
                            arResult.Add(arCCTV[i]);
                        return arResult;
                    }
                    else
                    {
                        arResult.AddRange(arCCTV);
                    }
                }               
                
                // 외부에서 가까운것을 찾는다. 4개를 찾을 때까지 반복
                ArrayList arOutZones = ZoneManager.Instance.FindNearZone(zone.ID, false, 10);
                foreach (Zone ozone in arOutZones)
                {
                    // 있는 경우 CCTV가 4 일때까지 반복하여 인접구역을 찾는다.
                    ArrayList cctvs = FindCCTV(ozone.ID);

                    if ((cctvs.Count + arResult.Count) >= 4)
                    {
                        int nCount = 4 - arResult.Count;
                        for (int i = 0; i < nCount; i++)
                            arResult.Add(cctvs[i]);
                        return arResult;
                    }
                    else
                    {
                        arResult.AddRange(cctvs);
                    }
                }
            }
            
            // 4 개를 찾았거나 못찾았거나 리턴한다.
            return arResult;
        }

        public ArrayList AutoPopupCCTV(int nZoneID)
        {
            Zone zone = ZoneManager.Instance.GetZone(nZoneID);
            if (zone == null)
                return null;

            return AutoPopupCCTV(zone);
        }

        public EditEquipZoneCCTV UpdateEquipZoneCCTV(int nCCTVIndex, int nCCTVID, EquipmentZone equipZone)
        {
            if (nCCTVIndex < 0 || nCCTVIndex > 3)
                return null;

            CCTV cctv = GetCCTV(nCCTVID);
            if (cctv == null)
                return null;

            CCTV[] arrCCTV = null;

            if (!m_dicEquipZoneCCTVs.ContainsKey(equipZone))
            {
                arrCCTV = new CCTV[4] { null, null, null, null };
                arrCCTV[nCCTVIndex] = cctv;
                m_dicEquipZoneCCTVs[equipZone] = arrCCTV;
            }
            else
            {
                arrCCTV = m_dicEquipZoneCCTVs[equipZone];
                arrCCTV[nCCTVIndex] = cctv;
            }

            EditEquipZoneCCTV editEquipZoneCCTV = new EditEquipZoneCCTV();
            editEquipZoneCCTV.EquipmentZone = equipZone;

            for (int i=0;i<4;i++)
            {
                editEquipZoneCCTV.SetCCTV(i, arrCCTV[i]);
            }

            return editEquipZoneCCTV;
        }

        public void UpdateDBEquipZoneCCTV(CCTV[] arrCCTVs, EquipmentZone equipZone)
        {
            UpdateEquipZoneCCTV(arrCCTVs, equipZone, m_dicEquipZoneCCTVs);
            UpdateEquipZoneCCTV(arrCCTVs, equipZone, m_dicDBEquipZoneCCTVs);
        }

        private void UpdateEquipZoneCCTV(CCTV[] arrCCTVs, EquipmentZone equipZone, Dictionary<EquipmentZone, CCTV[]> dicEquipZoneCCTVs)
        {
            if (!dicEquipZoneCCTVs.ContainsKey(equipZone))
            {
                CCTV[] arrNewCCTVs = new CCTV[4];
                dicEquipZoneCCTVs[equipZone] = arrNewCCTVs;

                for (int i = 0; i < 4; i++)
                {
                    arrNewCCTVs[i] = arrCCTVs[i];
                }
            }
            else
            {
                CCTV[] arrOrigins = dicEquipZoneCCTVs[equipZone];

                for (int i = 0; i < 4; i++)
                {
                    arrOrigins[i] = arrCCTVs[i];
                }
            }
        }

        public bool IsOriginStatus(EquipmentZone equipZone, CCTV[] arrCCTVs)
        {
            if (!m_dicDBEquipZoneCCTVs.ContainsKey(equipZone))
                return false;

            CCTV[] arrDBCCTVs = m_dicDBEquipZoneCCTVs[equipZone];

            for (int i = 0; i < 4; i++)
            {
                if (arrDBCCTVs[i] != arrCCTVs[i])
                    return false;
            }

            return true;
        }

        public CCTV[] GetCCTVArray(EquipmentZone equipZone)
        {
            if (!m_dicEquipZoneCCTVs.ContainsKey(equipZone))
                return null;

            return m_dicEquipZoneCCTVs[equipZone];
        }
    }
}
