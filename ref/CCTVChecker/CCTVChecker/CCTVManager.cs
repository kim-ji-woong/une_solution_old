using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CCTVChecker
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


        // 설비영역별 15개의 Temp CCTV
        private Dictionary<EquipmentZone, CCTV[]> m_dicEquipZoneTempCCTVs = new Dictionary<EquipmentZone, CCTV[]>();
       
        
        private CCTVManager()
        {

        }

        public CCTV GetCCTV(int nID)
        {
            if (!m_dicCCTVs.ContainsKey(nID))
            {
                return null;
            }
            return m_dicCCTVs[nID];
        }

        public bool LoadEquipZoneTempCCTV()
        {
            DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;

            string strSQL = "Select ID, EquipZoneID, CCTV1, CCTV2, CCTV3, CCTV4,  CCTV5, CCTV6, CCTV7, CCTV8,  CCTV9,CCTV10,CCTV11, CCTV12, CCTV13, CCTV14,CCTV15 from EquipZoneCCTVTemp";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 16; i += 17)
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                int nEquipZoneID = DBUtility.WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                
                int nCCTV1 = DBUtility.WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                int nCCTV2 = DBUtility.WebDBManager.GetIntField(arrResult[i + 3].ToString(), -1);
                int nCCTV3 = DBUtility.WebDBManager.GetIntField(arrResult[i + 4].ToString(), -1);
                int nCCTV4 = DBUtility.WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);                
                int nCCTV5 = DBUtility.WebDBManager.GetIntField(arrResult[i + 6].ToString(), -1);

                int nCCTV6 = DBUtility.WebDBManager.GetIntField(arrResult[i + 7].ToString(), -1);
                int nCCTV7 = DBUtility.WebDBManager.GetIntField(arrResult[i + 8].ToString(), -1);
                int nCCTV8 = DBUtility.WebDBManager.GetIntField(arrResult[i + 9].ToString(), -1);
                int nCCTV9 = DBUtility.WebDBManager.GetIntField(arrResult[i + 10].ToString(), -1);
                int nCCTV10 = DBUtility.WebDBManager.GetIntField(arrResult[i + 11].ToString(), -1);

                int nCCTV11 = DBUtility.WebDBManager.GetIntField(arrResult[i + 12].ToString(), -1);
                int nCCTV12 = DBUtility.WebDBManager.GetIntField(arrResult[i + 13].ToString(), -1);
                int nCCTV13 = DBUtility.WebDBManager.GetIntField(arrResult[i + 14].ToString(), -1);
                int nCCTV14 = DBUtility.WebDBManager.GetIntField(arrResult[i + 15].ToString(), -1);
                int nCCTV15 = DBUtility.WebDBManager.GetIntField(arrResult[i + 16].ToString(), -1);

                if (nID <= 0)
                    continue;

                EquipmentZone equipZone = ZoneManager.Instance.GetEquipZone(nEquipZoneID);

                if (equipZone == null)
                    continue;

                CCTV[] arrCCTV = new CCTV[15];
                m_dicEquipZoneTempCCTVs[equipZone] = arrCCTV;


                arrCCTV[0] = GetCCTV(nCCTV1);
                arrCCTV[1] = GetCCTV(nCCTV2);
                arrCCTV[2] = GetCCTV(nCCTV3);
                arrCCTV[3] = GetCCTV(nCCTV4);
                arrCCTV[4] = GetCCTV(nCCTV5);
                arrCCTV[5] = GetCCTV(nCCTV6);
                arrCCTV[6] = GetCCTV(nCCTV7);
                arrCCTV[7] = GetCCTV(nCCTV8);
                arrCCTV[8] = GetCCTV(nCCTV9);
                arrCCTV[9] = GetCCTV(nCCTV10);
                arrCCTV[10] = GetCCTV(nCCTV11);
                arrCCTV[11] = GetCCTV(nCCTV12);
                arrCCTV[12] = GetCCTV(nCCTV13);
                arrCCTV[13] = GetCCTV(nCCTV14);
                arrCCTV[14] = GetCCTV(nCCTV15);
            }

            return true;
        }

        public bool LoadEquipZoneCCTV()
        {
            DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;

            string strSQL = "Select ID, EquipZoneID, CCTV1, CCTV2, CCTV3, CCTV4, description from EquipZoneCCTV";
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

        public bool LoadCCTV( bool isIndoor)
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

        //public EditEquipZoneCCTV UpdateEquipZoneCCTV(int nCCTVIndex, int nCCTVID, EquipmentZone equipZone)
        //{
        //    if (nCCTVIndex < 0 || nCCTVIndex > 3)
        //        return null;

        //    CCTV cctv = GetCCTV(nCCTVID);
        //    if (cctv == null)
        //        return null;

        //    CCTV[] arrCCTV = null;

        //    if (!m_dicEquipZoneCCTVs.ContainsKey(equipZone))
        //    {
        //        arrCCTV = new CCTV[4] { null, null, null, null };
        //        arrCCTV[nCCTVIndex] = cctv;
        //        m_dicEquipZoneCCTVs[equipZone] = arrCCTV;
        //    }
        //    else
        //    {
        //        arrCCTV = m_dicEquipZoneCCTVs[equipZone];
        //        arrCCTV[nCCTVIndex] = cctv;
        //    }

        //    EditEquipZoneCCTV editEquipZoneCCTV = new EditEquipZoneCCTV();
        //    editEquipZoneCCTV.EquipmentZone = equipZone;

        //    for (int i=0;i<4;i++)
        //    {
        //        editEquipZoneCCTV.SetCCTV(i, arrCCTV[i]);
        //    }

        //    return editEquipZoneCCTV;
        //}

        public void UpdateDBEquipZoneCCTV(CCTV[] arrCCTVs, EquipmentZone equipZone)
        {
            UpdateEquipZoneCCTV(arrCCTVs, equipZone, m_dicEquipZoneCCTVs);
            UpdateEquipZoneCCTV(arrCCTVs, equipZone, m_dicDBEquipZoneCCTVs);
            WriteToDB(arrCCTVs, equipZone);
        }

        private void WriteToDB(CCTV[] arrCCTVs, EquipmentZone equipZone)
        {
            DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;

            string strSQL = "Select id from EquipZoneCCTV where equipZoneID = " + equipZone.ID.ToString();
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                InsertDB(arrCCTVs, equipZone);
            else
            {
                int nID = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), -1);
                UpdateDB(arrCCTVs, equipZone, nID);
            }
        }

        private void UpdateDB(CCTV[] arrCCTVs, EquipmentZone equipZone, int nID)
        {
            DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;

            string strSQL = "Update EquipZoneCCTV set ";

            int nCCTVCount = arrCCTVs.Count();

            for (int i = 1; i <= nCCTVCount; i++)
            {
                CCTV cctv = arrCCTVs[i-1];

                if (cctv == null)
                {
                    if (i == nCCTVCount)
                        strSQL += "CCTV" + i.ToString() + " = NULL";
                    else
                        strSQL += "CCTV" + i.ToString() + " = NULL, ";
                }
                else
                {
                    if (i == nCCTVCount)
                        strSQL += "CCTV" + i.ToString() + " = " + cctv.ID.ToString();
                    else
                        strSQL += "CCTV" + i.ToString() + " = " + cctv.ID.ToString() + ", ";
                }
            }

            strSQL += " where ID = " + nID.ToString();

            if (dbMgr.GetResultData(strSQL, 0) == null)
                return;
        }

        private void InsertDB(CCTV[] arrCCTVs, EquipmentZone equipZone)
        {
            DBUtility.WebDBManager dbMgr = FormMain.Instance.DBManager;

            string strSQL = "Select max(id) from EquipZoneCCTV";
            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            int nID = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString(), 0) + 1;
            strSQL = "Insert into EquipZoneCCTV (id, EquipZoneID, CCTV1, CCTV2, CCTV3, CCTV4, description)";
            strSQL += " values (" + nID.ToString() + ", " + equipZone.ID.ToString() + ", ";

            int nCCTVCount = arrCCTVs.Count();

            for (int i = 0; i < nCCTVCount; i++)
            {
                CCTV cctv = arrCCTVs[i];
                strSQL += cctv.ID.ToString() + ", ";
            }

            for (int i = nCCTVCount; i < 4; i++)
            {
                strSQL += "NULL, ";
            }

            strSQL += "NULL)";

            if (dbMgr.GetResultData(strSQL, 0) == null)
                return;
        }

        private void UpdateEquipZoneCCTV(CCTV[] arrCCTVs, EquipmentZone equipZone, Dictionary<EquipmentZone, CCTV[]> dicEquipZoneCCTVs)
        {
            if (!dicEquipZoneCCTVs.ContainsKey(equipZone))
            {
                CCTV[] arrNewCCTVs = new CCTV[4];
                dicEquipZoneCCTVs[equipZone] = arrNewCCTVs;

                for (int i = 0; i < 4; i++)
                {
                    if (i < arrCCTVs.Count())
                        arrNewCCTVs[i] = arrCCTVs[i];
                }
            }
            else
            {
                CCTV[] arrOrigins = dicEquipZoneCCTVs[equipZone];

                for (int i = 0; i < 4; i++)
                {
                    if (i < arrCCTVs.Count())
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
        public CCTV[] GetTempCCTVArray(EquipmentZone equipZone)
        {
            if (!m_dicEquipZoneTempCCTVs.ContainsKey(equipZone))
                return null;

            return m_dicEquipZoneTempCCTVs[equipZone];
        }

        // 처음 4개는 GetCCTVArray, 나머지 11개는 GetTempCCTVArray
        public CCTV[] GetMixCCTVArray(EquipmentZone equipZone)
        {
            CCTV[] arr1 = GetCCTVArray(equipZone);
            CCTV[] arr2 = GetTempCCTVArray(equipZone);

            /*if (arr1 == null)
                return arr2;
            else if (arr2 == null)
                return arr1;*/

            if (arr1 == null)
            {
                arr1 = new CCTV[4] { null, null, null, null };
            }

            if (arr2 == null)
            {
                arr2 = new CCTV[11] { null, null, null, null, null, null, null, null, null, null, null };
            }

            int nCount1 = arr1.Count();
            int nCount2 = arr2.Count();
            int nArrayCount = nCount1 + nCount2;

            if (nArrayCount > 15)
                nArrayCount = 15;

            CCTV[] arrCCTVs = new CCTV[nArrayCount];

            for (int i = 0; i < nCount1; i++)
            {
                arrCCTVs[i] = arr1[i];
            }

            for (int i = nCount1; i < nArrayCount; i++)
            {
                arrCCTVs[i] = arr2[i - nCount1];
            }

            return arrCCTVs;
        }

        // Null인 CCTV를 Temp CCTV에서 모두 채워넣는다.
        public void FillCCTVs()
        {
            foreach (KeyValuePair<EquipmentZone, CCTV[]> pair in m_dicDBEquipZoneCCTVs)
            {
                if (pair.Value[0] == null || pair.Value[1] == null || pair.Value[2] == null || pair.Value[3] == null)
                {
                    FillEquipZoneCCTV(pair.Key, pair.Value);

                    UpdateEquipZoneCCTV(pair.Value, pair.Key, m_dicEquipZoneCCTVs);
                    WriteToDB(pair.Value, pair.Key);
                }
            }
        }

        private void FillEquipZoneCCTV(EquipmentZone equipZone, CCTV[] arrCCTVs)
        {
            for (int i = 0; i < 4; i++)
            {
                CCTV cctv = arrCCTVs[i];

                if (cctv == null)
                {
                    CCTV[] arrTempCCTVs = GetTempCCTVArray(equipZone);
                    arrCCTVs[i] = GetTempCCTVArray(arrTempCCTVs, arrCCTVs);
                }
            }
        }

        // arrTempCCTVs 가운데 arrCCTVs에 속하지 않은 CCTV 객체 하나를 얻어온다.
        private CCTV GetTempCCTVArray(CCTV[] arrTempCCTVs, CCTV[] arrCCTVs)
        {
            foreach (CCTV temp in arrTempCCTVs)
            {
                if (temp == null)
                    continue;

                foreach (CCTV cctv in arrCCTVs)
                {
                    if (temp == cctv)
                        goto CONTINUE;
                }

                return temp;

            CONTINUE:
                continue;
            }

            return null;
        }
    }
}
