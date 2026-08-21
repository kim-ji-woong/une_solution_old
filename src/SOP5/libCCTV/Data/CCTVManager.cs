using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace UnE.CCTV
{
    public class CCTVManager
    {
        // 한 화면에 표시되는 CCTV의 갯수
        private int m_nScrCCTV = 6;

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


        private int m_nSiteID = 1;
        private CCTVManager()
        {
            m_nSiteID = UnE.SOP.ProxySOP.Instance.SiteID;
        }

        public CCTV GetCCTV(int nID)
        {
            if (!m_dicCCTVs.ContainsKey(nID))
                return null;

            return m_dicCCTVs[nID];
        }

        public bool LoadEquipZoneCCTV(int nEquipZone = -1)
        {
            // CCTV의 갯수가 m_nScrCCTV에 따라 가져오도록 수정. 사용시 EquipZoneCCTV 테이블에 CCTV필드 확인 바람
            // 2015-10-05 skkim
            DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;

            string strSQL = "Select ezc.ID, ezc.EquipZoneID";
            for (int i = 0; i < m_nScrCCTV; i++)
            {
                strSQL += ", ezc.CCTV";
                strSQL += (i + 1).ToString();
            }

            for (int i = 0; i < m_nScrCCTV; i++)
            {
                strSQL += ", ezc.PRESET";
                strSQL += (i + 1).ToString();
            }

            strSQL += ", ezc.description from EquipZoneCCTV as ezc";

            if (nEquipZone != -1)
            {
                strSQL += string.Format(" where ezc.EquipZoneID = {0}", nEquipZone);
            }
            else
            {
                strSQL += string.Format(", EquipmentZone as ez where ez.ID = ezc.EquipZoneID and ez.SiteID = {0}", m_nSiteID);
            }
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - (2 + (m_nScrCCTV*2)); i += (3 + (m_nScrCCTV*2)))
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nEquipZoneID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);

                int[] nCCTV = new int[m_nScrCCTV];
                string[] nPRESET = new string[m_nScrCCTV];

                for (int j = 0; j < m_nScrCCTV; j++)
                {
                    nCCTV[j] = DBUtility.WebDBManager.GetIntField(arrResult[i + 2 + j].ToString(), -1);
                }

                for (int k = 0; k < m_nScrCCTV; k++)
                {
                    nPRESET[k] = DBUtility.WebDBManager.GetStringField(arrResult[i + 2 + m_nScrCCTV + k].ToString(), "").Replace("null","");
                }

                if (nID <= 0)
                    continue;

                EquipmentZone equipZone = ZoneManager.Instance.GetEquipZone(nEquipZoneID);

                if (equipZone == null)
                    continue;

                equipZone.Preset = nPRESET;

                CCTV[] arrCCTV = new CCTV[m_nScrCCTV];
                m_dicEquipZoneCCTVs[equipZone] = arrCCTV;

                CCTV[] arrDBCCTV = new CCTV[m_nScrCCTV];
                m_dicDBEquipZoneCCTVs[equipZone] = arrDBCCTV;

                for (int j = 0; j < m_nScrCCTV; j++)
                {
                    arrDBCCTV[j] = arrCCTV[j] = GetCCTV(nCCTV[j]);
                }
            }
            return true;
        }

        private int GetCCTVType(string szType)
        {
            if (szType == "Axis")
                return 1;
            else if (szType == "NVS")
                return 2;
            else if (szType == "XpressStrm")
                return 3;
            else if (szType == "UDP")
                return 4;
            else if (szType == "Panasonic")
                return 5;
            else if (szType == "iPolis")
                return 6;
            else if (szType == "IPVideo")
                return 7;
            else if (szType == "HIK")
                return 8;
            else if (szType == "NVT")
                return 9;
            else if (szType == "MediaPlayer")
                return 10;
            else if (szType == "IDIS")
                return 11;
            else if (szType == "RTSP")
                return 12;
            else if (szType == "IDIS_NVR")
                return 13;
            else if (szType == "ITX_NVR")
                return 14;
            else if (szType == "RTSPONVIF")
                return 15;
            return 0;
        }

        public bool LoadCCTV(bool isIndoor)
        {
            DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;

            //string strSQL = "Select ID, CameraName, IPAddr, Port, X, Y, Z, ZoneID, LOD from CCTV where IsIndoor = " + (isIndoor ? "1" : "0");

            // SiteID를 사용하도록 변경. skkim 2015.01.23
            StringBuilder sb = new StringBuilder();
            //sb.Append("SELECT cv.ID, cv.CameraName, cv.IPAddr, cv.Port, cv.X, cv.Y, cv.Z, cv.ZoneID, cv.LOD ");
            //sb.Append(" FROM CCTV as cv JOIN Zone as z ON z.ID = cv.ZoneID ");
            //sb.AppendFormat(" WHERE z.SiteID = {0} AND IsIndoor = {1} ORDER BY cv.Id", m_nSiteID, (isIndoor ? "1" : "0"));


            sb.Append("SELECT cv.ID, cv.CameraName, cv.IPAddr, cv.Port, cv.X, cv.Y, cv.Z, cv.ZoneID, cv.LOD ");
            sb.Append(",cv.HTTPPort, cv.Type,cv.Stream,cv.Channel,cv.UserID,cv.Password,cv.URL ");
            sb.Append(" FROM CCTV as cv JOIN Zone as z ON z.ID = cv.ZoneID ");
            sb.AppendFormat(" WHERE cv.LOD > -1 AND z.SiteID = {0} AND IsIndoor = {1} ORDER BY cv.Id", m_nSiteID, (isIndoor ? "1" : "0"));

            string strSQL = sb.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 15; i += 16)
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

                int nHttpPort = DBUtility.WebDBManager.GetIntField(arrResult[i + 9].ToString(), 80);
                string szType = DBUtility.WebDBManager.GetStringField(arrResult[i + 10].ToString(), "");
                int nStream = DBUtility.WebDBManager.GetIntField(arrResult[i + 11].ToString(), 0);
                int nChannel = DBUtility.WebDBManager.GetIntField(arrResult[i + 12].ToString(), 0);
                string szUserName = DBUtility.WebDBManager.GetStringField(arrResult[i + 13], "guest");
                string szPassword = DBUtility.WebDBManager.GetStringField(arrResult[i + 14], "");
                string szURL = DBUtility.WebDBManager.GetStringField(arrResult[i + 15], "");

                //if (szType == "Axis")
                //continue;
                //if (szType == "IPVideo")
                //   continue;
                //if (szType == "Panasonic")
                //    continue;
                //if(szType == "UDP")
                //   continue;
                int nType = GetCCTVType(szType);

                if (nLOD < (int)CCTV.LOD.DISCONNECTED || nLOD > (int)CCTV.LOD.VERY_IMPORTANT)
                    continue;

                if (nZoneID == -1)
                    continue;

                Zone zone = ZoneManager.Instance.GetZone(nZoneID);

                CCTV cctv = new CCTV();

                cctv.ID = nID;
                cctv.AccessKey = strCameraName;
                cctv.IPAddress = strIPAddr;
                cctv.PortNo = (short)nPort;
                cctv.POI = new POI();
                cctv.POI.X = x / 1000;
                cctv.POI.Y = z;
                cctv.POI.Z = -y / 1000;
                cctv.POI.Zone = zone;
                cctv.POI.IsIndoor = isIndoor;
                cctv.LODType = (CCTV.LOD)nLOD;
                cctv.Channel = nChannel;
                cctv.Stream = nStream;
                cctv.UserName = szUserName;
                cctv.Password = szPassword;
                cctv.CCTVType = nType;

                /// <summary>
                /// DB에 저장되어 있는 값을 기억시키기 위한 데이터
                cctv.AccessKeyDB = strCameraName;
                cctv.IPAddressDB = strIPAddr;
                cctv.PortNoDB = (short)nPort;
                cctv.POI.XDB = x / 1000;
                cctv.POI.YDB = 10.0f;
                cctv.POI.ZDB = -y / 1000;
                cctv.POI.ZoneDB = zone;
                cctv.LODTypeDB = (CCTV.LOD)nLOD;

                cctv.ChannelDB = nChannel;
                cctv.StreamDB = nStream;
                cctv.UserNameDB = szUserName;
                cctv.PasswordDB = szPassword;
                cctv.CCTVTypeDB = nType;
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
                if (cctv.POI != null && cctv.POI.Zone != null)
                {
                    if (nZoneID == cctv.POI.Zone.ID)
                    {
                        cctvInZone.Add(cctv);
                    }
                }

            }
            return cctvInZone;
        }

        public ArrayList AutoPopupCCTV(Zone zone)
        {
            ArrayList arResult = new ArrayList();

            int nICount = m_IndoorCCTVList.Count;
            int nOCount = m_OutdoorCCTVList.Count;

            // 전체 CCVT가 m_nScrCCTV 개이상인지
            // 이하이면 모두 리턴
            if (nICount + nOCount <= m_nScrCCTV)
            {
                arResult.AddRange(m_IndoorCCTVList);
                arResult.AddRange(m_OutdoorCCTVList);
                return arResult;
            }

            // 화재 발생 존에 있는 CCTV를 가져온다.
            int nZoneID = zone.ID;
            ArrayList cctvInZone = FindCCTV(nZoneID);

            // CCTV 가 m_nScrCCTV개 이상인지?
            if (cctvInZone.Count >= m_nScrCCTV)
            {
                // m_nScrCCTV 개를 랜덤하게 선택하여 리턴
                for (int i = 0; i < m_nScrCCTV; i++)
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

                if (arZones == null)
                    return arResult;

                foreach (Zone ozone in arZones)
                {
                    // 있는 경우 CCTV가 4 일때까지 반복하여 인접구역을 찾는다.
                    ArrayList cctvs = FindCCTV(ozone.ID);

                    if ((cctvs.Count + arResult.Count) >= m_nScrCCTV)
                    {
                        int nCount = m_nScrCCTV - arResult.Count;
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

                if (arZones == null)
                    return arResult;

                foreach (Zone ozone in arZones)
                {
                    // 있는 경우 CCTV가 4 일때까지 반복하여 인접구역을 찾는다.
                    ArrayList cctvs = FindCCTV(ozone.ID);

                    if ((cctvs.Count + arResult.Count) >= m_nScrCCTV)
                    {
                        int nCount = m_nScrCCTV - arResult.Count;
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
                    if ((arCCTV.Count + arResult.Count) >= m_nScrCCTV)
                    {
                        int nCount = m_nScrCCTV - arResult.Count;
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
                    if ((arCCTV.Count + arResult.Count) >= m_nScrCCTV)
                    {
                        int nCount = m_nScrCCTV - arResult.Count;
                        for (int i = 0; i < nCount; i++)
                            arResult.Add(arCCTV[i]);
                        return arResult;
                    }
                    else
                    {
                        arResult.AddRange(arCCTV);
                    }
                }

                // 외부에서 가까운것을 찾는다. m_nScrCCTV개를 찾을 때까지 반복
                ArrayList arOutZones = ZoneManager.Instance.FindNearZone(zone.ID, false, 10);
                foreach (Zone ozone in arOutZones)
                {
                    // 있는 경우 CCTV가 4 일때까지 반복하여 인접구역을 찾는다.
                    ArrayList cctvs = FindCCTV(ozone.ID);

                    if ((cctvs.Count + arResult.Count) >= m_nScrCCTV)
                    {
                        int nCount = m_nScrCCTV - arResult.Count;
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

            // m_nScrCCTV 개를 찾았거나 못찾았거나 리턴한다.
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
            if (nCCTVIndex < 0 || nCCTVIndex > m_nScrCCTV)
                return null;

            CCTV cctv = GetCCTV(nCCTVID);
            //if (cctv == null)
            //	return null;

            if (equipZone == null)
                return null;

            CCTV[] arrCCTV = null;

            if (!m_dicEquipZoneCCTVs.ContainsKey(equipZone))
            {
                arrCCTV = new CCTV[m_nScrCCTV];
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

            for (int i = 0; i < m_nScrCCTV; i++)
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
                CCTV[] arrNewCCTVs = new CCTV[m_nScrCCTV];
                dicEquipZoneCCTVs[equipZone] = arrNewCCTVs;

                for (int i = 0; i < m_nScrCCTV; i++)
                {
                    arrNewCCTVs[i] = arrCCTVs[i];
                }
            }
            else
            {
                CCTV[] arrOrigins = dicEquipZoneCCTVs[equipZone];

                for (int i = 0; i < m_nScrCCTV; i++)
                {
                    arrOrigins[i] = arrCCTVs[i];
                }
            }
        }

        /// <summary>
        /// EquipmentZone에 지정된 CCTV의 DB정보와 동일한지 비교
        /// </summary>
        /// <param name="equipZone">대상 EquipmentZone</param>
        /// <param name="arrCCTVs">비교 CCTV's</param>
        /// <returns>동일하면 true, 다르면 false</returns>
        public bool IsOriginStatus(EquipmentZone equipZone, CCTV[] arrCCTVs)
        {
            if (!m_dicDBEquipZoneCCTVs.ContainsKey(equipZone))
                return false;

            CCTV[] arrDBCCTVs = m_dicDBEquipZoneCCTVs[equipZone];

            for (int i = 0; i < m_nScrCCTV; i++)
            {
                if (arrDBCCTVs[i] != arrCCTVs[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// EquipmentZone에 지정된 CCTV리스트를 가져온다.
        /// </summary>
        /// <param name="equipZone"></param>
        /// <returns></returns>
        public CCTV[] GetCCTVArray(EquipmentZone equipZone)
        {
            if (!m_dicEquipZoneCCTVs.ContainsKey(equipZone))
                return null;

            return m_dicEquipZoneCCTVs[equipZone];
        }
    }
}