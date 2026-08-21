using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using DBUtility;
using System.Windows.Forms;

namespace SDMS
{
    namespace Report
    {
        public class ReactionManager
        {
            private ArrayList m_arrDectectList = null;
            public ArrayList DectectList
            {
                get { return m_arrDectectList; }
                set { m_arrDectectList = value; }
            }
            private ArrayList m_arrMulFunctionList = null;
            public ArrayList MulFunctionList
            {
                get { return m_arrMulFunctionList; }
                set { m_arrMulFunctionList = value; }
            }

            private ArrayList m_arrReactionHistory = new ArrayList();

            //HistoryID,ReactionLog
            private Dictionary<int, ArrayList> m_dicHistoryLog = new Dictionary<int, ArrayList>();
            //SensorID,HistoryID List
            private Dictionary<int, ArrayList> m_dicSensorHistorys = new Dictionary<int, ArrayList>();

            //HistoryID, Zone
            private Dictionary<int, Zone> m_dicZoneHistorys = new Dictionary<int, Zone>();

            //HistoryID, ReactionType
            private Dictionary<int, int> m_dicHistoryType = new Dictionary<int, int>();

            //MulFunctionLog, SensorReactionLogList
            private Dictionary<MulFunctionLog, ArrayList> m_dicMulFuctionSrLog = new Dictionary<MulFunctionLog, ArrayList>();
            internal Dictionary<MulFunctionLog, ArrayList> DicMulFuctionSrLog
            {
                get { return m_dicMulFuctionSrLog; }
                set { m_dicMulFuctionSrLog = value; }
            }

            private ArrayList arrAllReactionLog = new ArrayList();

            //private static ReactionManager m_Instance = null;
            //public static ReactionManager Instance
            //{
            //    get
            //    {
            //        if (m_Instance == null)
            //            m_Instance = new ReactionManager();
            //        return m_Instance;
            //    }
            //}

            //화재를 신고한곳<MemberID, NicName>
            private Dictionary<string, string> m_dicGenUserIDDNicName = new Dictionary<string, string>();
            public Dictionary<string, string> DicGenUserIDDNicName
            {
                get { return m_dicGenUserIDDNicName; }
                set { m_dicGenUserIDDNicName = value; }
            }

            //HistoryID, Param3(MemberID)
            private Dictionary<int, string> m_dicHistoryMember = new Dictionary<int, string>();


            public ReactionManager()
            {
                m_arrDectectList = new ArrayList();
            }

            public void DataClear()
            {
                if(m_arrDectectList != null)
                    m_arrDectectList.Clear();
                if(m_arrMulFunctionList != null)
                    m_arrMulFunctionList.Clear();
                if(m_dicHistoryLog != null)
                    m_dicHistoryLog.Clear();
                if(m_dicSensorHistorys != null)
                    m_dicSensorHistorys.Clear();
                if (m_dicZoneHistorys != null)
                    m_dicZoneHistorys.Clear();
                if(m_dicHistoryType != null)
                    m_dicHistoryType.Clear();
                if(arrAllReactionLog != null)
                    arrAllReactionLog.Clear();
                if (m_dicMulFuctionSrLog != null)
                    m_dicMulFuctionSrLog.Clear();
                if (m_arrReactionHistory != null)
                    m_arrReactionHistory.Clear();
            }

            private ArrayList AddReactionHistoryLog(ArrayList arrManualReactionHistory, ArrayList arrReactionList)
            {
                ArrayList arrAllReactionLog = new ArrayList();

                if (arrReactionList != null)
                    arrAllReactionLog.AddRange(arrReactionList);
                if (arrManualReactionHistory != null)
                    arrAllReactionLog.AddRange(arrManualReactionHistory);

                return arrAllReactionLog;
            }

            //탐지, 처리이력
            public void ZoneSubmit(ArrayList arrZoneList, DateTime startDate, DateTime endDate, int pageType = 1)//pageType이 1이면 탐지/처리 2이면 대응이력
            {
                //initControl();

                LoadSOPGenUser();

                string strNowDate = "";
                string strBeforeDate = string.Format("{0} {1}:{2}:{3}", startDate.ToShortDateString(), "00", "00", "00");

                if (pageType == 2)//대응이력은 시작날과 종료날이 같을경우 시간까지 조절해야하므로 ..
                {
                    if (startDate.ToShortDateString() == endDate.ToShortDateString())
                    {
                        strNowDate = string.Format("{0} {1}:{2}:{3}", endDate.ToShortDateString(), endDate.Hour, endDate.Minute, endDate.Second);
                    }
                    else
                    {
                        strNowDate = string.Format("{0} {1}:{2}:{3}", endDate.ToShortDateString(), 23, 59, 59);
                    }
                }
                else
                {
                    //검색에 오늘날짜가 들어가면 현재 시간까지만 검사
                    if (endDate.ToShortDateString() == DateTime.Now.ToShortDateString())
                    {
                        strNowDate = string.Format("{0} {1}:{2}:{3}", endDate.ToShortDateString(), DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                    }
                    else//아니면 23시 59분59분까지 검사
                    {
                        strNowDate = string.Format("{0} {1}:{2}:{3}", endDate.ToShortDateString(), 23, 59, 59);
                    }
                }
                //ZoneID 리스트로 ReactionHistory의 수동신고의 log를 가져온다.
                ArrayList arrManualReactionHistory = GetManualReactionHistory(arrZoneList, strBeforeDate, strNowDate);

                //선택한 ZoneID 리스트로 EquipmentZoneID를 찾는다.
                ArrayList arrEquipmentZoneList = FindEquipZone(arrZoneList);
                //가져온 EquipmentZoneID 리스트로 SensorID를 찾아온다.
                ArrayList arrSensorZoneList = FindSensorZone(arrEquipmentZoneList);
                //SensorID리스트로 SensorHistoryID를 찾아옴
                ArrayList arrZoneHistoryList = GetSensorZoneHistoryID(arrSensorZoneList, strBeforeDate, strNowDate);
                //ReactionLog를 가져옴
                ArrayList arrReactionList = GetReactionHistory(arrZoneHistoryList);

                //수동신고와 자탐의 SensorReactionLog를 합친다.
                arrAllReactionLog = new ArrayList();
                arrAllReactionLog = AddReactionHistoryLog(arrManualReactionHistory, arrReactionList);

                //오작동이력 로그 저장
                m_arrMulFunctionList = GetMulFunctionLog(arrZoneList, strBeforeDate, strNowDate);


                //전체 ReactionLog중에 화재 탐지 된 로그만 가져와서 저장함
                //화재신고 된 로그만 가져옴(ReactionType=0 -> 자탐 / reactionLog.ReactionType == 22 && reactionLog.Param2 == "0" -> 수동
                m_arrDectectList = GetDetectLog(arrAllReactionLog);
                m_arrDectectList.Sort();


            }

            public void LoadSOPGenUser()
            {
                WebDBManager webDB = FormMain.Instance.DBManager;

                string strSQL = "select ID, NickName From SOPGenUser";

                ArrayList arrResult = webDB.GetResultData(strSQL, 0);
                if (arrResult == null)
                    return;

                int nResultCount = arrResult.Count;
                DateTime dt = DateTime.Now;

                for (int i = 0; i < nResultCount-1; i += 2)
                {
                    int nMemberID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    string strNicName = WebDBManager.GetStringField(arrResult[i + 1], "");

                    if (!m_dicGenUserIDDNicName.ContainsKey(nMemberID.ToString()))
                        m_dicGenUserIDDNicName[nMemberID.ToString()] = strNicName;
                }
            }

            public ArrayList HistorySubmit(DateTime startDate, DateTime endDate)
            {
                m_arrReactionHistory.Clear();


                endDate = endDate.AddDays(1);
                foreach (KeyValuePair<int,ArrayList> pair in m_dicHistoryLog)
                {
                    int nHistoryID = pair.Key;
                    ArrayList log = pair.Value;
                    int nReactionType = 0;
                    Zone zone = null;
                    string strMemberID = "";



                    if (m_dicZoneHistorys.ContainsKey(nHistoryID))
                        zone = m_dicZoneHistorys[nHistoryID];

                    ReactionLog reactionLog = new ReactionLog();
                    reactionLog.HistoryID = nHistoryID;
                    reactionLog.ArrLogList = log;

                    //자탐 ReactionType가져옴
                    if (m_dicHistoryType.ContainsKey(nHistoryID))
                    {
                        nReactionType = m_dicHistoryType[nHistoryID];
                        reactionLog.SensorType = 1;
                    }
                    else
                        reactionLog.SensorType = 0;

                    if (reactionLog.SensorType == 0)
                    {
                        //오작동인지 화재인지, 무시된 신호인지 구분하기위함(수동신고)
                        if (reactionLog.SensorType == 0)
                        {
                            foreach (SensorReactionLog Typelog in log)
                            {
                                if (Typelog.ReactionType == 22)
                                {
                                    nReactionType = 22;
                                    break;
                                }
                                else if (Typelog.ReactionType == 21)
                                {
                                    nReactionType = 21;
                                    break;
                                }
                                else if (Typelog.ReactionType == 23)
                                {
                                    nReactionType = 23;
                                    break;
                                }
                                nReactionType = Typelog.ReactionType;
                            }
                        }
                    }


                    //가장 맨 처음 발생한 ReactionLog를 Comobox로 보여줘야 하므로 log배열의 가장 첫번째 값을 가져온다
                    SensorReactionLog sensorreactionLog = (SensorReactionLog)log[0];

                    if (!(sensorreactionLog.Time >= startDate && sensorreactionLog.Time <= endDate))
                        continue;

                    
                    if (m_dicHistoryMember.ContainsKey(nHistoryID))
                        strMemberID = m_dicHistoryMember[nHistoryID];

                    if (m_dicGenUserIDDNicName.ContainsKey(strMemberID))
                        reactionLog.UserName = m_dicGenUserIDDNicName[strMemberID];
                    
                    reactionLog.Time = sensorreactionLog.Time;
                    reactionLog.SensorType = sensorreactionLog.SensorType;
                    reactionLog.Zone = zone;
                    reactionLog.ManagerName = FindManagerName(zone);
                    reactionLog.Type = nReactionType;

                    m_arrReactionHistory.Add(reactionLog);
                }
                m_arrReactionHistory.Sort();
                return m_arrReactionHistory;
            }

            public ArrayList GetReactionLog(int nHistoryID)
            {
                ArrayList arrReactLog = new ArrayList();

                foreach (KeyValuePair<int, ArrayList> pair in m_dicHistoryLog)
                {
                    int nSensorHistoryID = pair.Key;
                    ArrayList log = pair.Value;
                    string strMemberID = "";

                    if (nSensorHistoryID == nHistoryID)
                    {
                        Zone zone = null;
                        if (m_dicZoneHistorys.ContainsKey(nHistoryID))
                            zone = m_dicZoneHistorys[nHistoryID];

                        foreach (SensorReactionLog srLog in log)
                        {
                            ReactionLog reactionLog = new ReactionLog();
                            reactionLog.HistoryID = nHistoryID;
                            reactionLog.ArrLogList = log;

                            if (m_dicHistoryMember.ContainsKey(nHistoryID))
                                strMemberID = m_dicHistoryMember[nHistoryID];

                            if (m_dicGenUserIDDNicName.ContainsKey(strMemberID))
                                reactionLog.UserName = m_dicGenUserIDDNicName[strMemberID];


                            //자탐은 param1
                            reactionLog.equipZone = ZoneManager.Instance.GetEquipZone(srLog.Param1);


                            reactionLog.Time = srLog.Time;
                            reactionLog.SensorType = srLog.SensorType;
                            reactionLog.Zone = zone;
                            reactionLog.ManagerName = FindManagerName(zone);
                            reactionLog.Type = srLog.ReactionType;

                            

                            arrReactLog.Add(reactionLog);
                        }
                        break;
                    }
                }

               // arrReactLog.Sort();
                return arrReactLog;
            }


            private Zone GetZone(int nHistoryID)
            {
                if (m_dicZoneHistorys.ContainsKey(nHistoryID))
                    return m_dicZoneHistorys[nHistoryID];

                return null;
            }


            private ArrayList GetDetectLog(ArrayList arrAllLog)
            {
                ArrayList arrDetectLog = new ArrayList();
                ArrayList arrComboBoxDate = new ArrayList();
                //ArrayList arrReactionLog = new ArrayList();
                
                foreach (SensorReactionLog reactionLog in arrAllLog)
                {
                    if (reactionLog.ReactionType == 0 || (reactionLog.ReactionType == 22 && reactionLog.Param2 == "0"))
                    {
                        DetectLog detect = new DetectLog();

                        detect.HistoryID = reactionLog.SensorHistoryID;
                        detect.Time = reactionLog.Time;

                        Zone zone = null;

                        if (reactionLog.Param2 == "0")
                        {
                            zone = ZoneManager.Instance.GetZone(reactionLog.Param1);
                            detect.zoneID = reactionLog.Param1;
                        }
                        else
                        {
                            if (m_dicZoneHistorys.ContainsKey(reactionLog.SensorHistoryID))
                                zone = m_dicZoneHistorys[reactionLog.SensorHistoryID];

                            detect.zoneID = zone.ID;
                        }

                        string szBuildingName = zone.Building != null ? zone.Building.BuildingName : "";
                        string szGroupName = szBuildingName != "" ? zone.Building.BuildingGroup.BuildingGroupName : "";
                        string strFloorIndex = zone.Floor != null ? zone.Floor.ToString() : "";


                        //EquipZone표시는 자탐일때만.. 수동신고일때는 알 수 없다.
                        if (reactionLog.ReactionType == 0)
                        {
                            //EquipZone구하기
                            detect.EquipZone = ZoneManager.Instance.GetEquipZone(reactionLog.Param1);
                        }


                        if (szGroupName == "")
                            detect.BuildingGroup = "외부 영역";
                        else
                            detect.BuildingGroup = szBuildingName;

                        if (szBuildingName == "")
                            detect.BuildingName = zone.ZoneName;
                        else
                            detect.BuildingName = szBuildingName;

                        detect.FloorName = strFloorIndex;

                        string strManagerName = FindManagerName(zone);
                        detect.ManagerName = strManagerName;

                        //자탐
                        if (reactionLog.ReactionType == 0)
                            detect.DetectType = GetReactionString(1);
                        else//수동신고
                            detect.DetectType = GetReactionString(4);

                        arrDetectLog.Add(detect);


                    }
                }
                //
                //arrDetectLog.Sort();
                return arrDetectLog;
            }


            
            private ArrayList GetMulFunctionLog(ArrayList arrZoneList, string strStartDate, string strEndDate)
            {
                ArrayList arrMulFunction = new ArrayList();

                //Zone별로 Log에서 탐지,화재,오작동,처리되지않은신호의 갯수, 오작동률 등을 구함
                foreach (Zone zone in arrZoneList)
                {
                    ArrayList arrHistoryList = FindHistoryID(zone.ID);
                    if (arrHistoryList == null)
                        continue;

                    //오작동이력 클래스 생성
                    MulFunctionLog mulfuction = new MulFunctionLog();

                    int nFireCount = 0;
                    int nMulFunctionCount = 0;
                    int nNotprocessCount = 0;

                    foreach (int nHistoryID in arrHistoryList)
                    {
                        ArrayList arrLog = new ArrayList();

                        if (m_dicHistoryLog.ContainsKey(nHistoryID))
                            arrLog = m_dicHistoryLog[nHistoryID];

                        int nType = 23;

                        foreach (SensorReactionLog log in arrLog)
                        {
                            if (log.ReactionType == 0)
                            {
                                ArrayList arrSensorLog = null;


                                //<MulFunctionLog, SensorReactionLog> Dictionary에 값 추가
                                if (m_dicMulFuctionSrLog.ContainsKey(mulfuction))
                                    arrSensorLog = m_dicMulFuctionSrLog[mulfuction];
                                else
                                {
                                    arrSensorLog = new ArrayList();
                                    m_dicMulFuctionSrLog[mulfuction] = arrSensorLog;
                                }
                                arrSensorLog.Add(log);
                            }

                            if (log.ReactionType == 22)
                            {
                                nFireCount++;
                                nType = 22;

                                break;
                            }
                            else if (log.ReactionType == 21)
                            {
                                nMulFunctionCount++;
                                nType = 21;

                                break;
                            }
                        }

                        if (!m_dicHistoryType.ContainsKey(nHistoryID))
                        {
                            m_dicHistoryType.Add(nHistoryID, nType);
                        }

                    }

                    if (arrHistoryList.Count == 0)
                        continue;

                    mulfuction.ReactionCount = arrHistoryList.Count;
                    //mulfuction.ReactionCount = mulfuction.ReactionCount - nMinusCount;

                    //처리되지 않음
                    nNotprocessCount = arrHistoryList.Count - (nFireCount + nMulFunctionCount);
    
                    double PercentMulFunction = (nMulFunctionCount * 100) / arrHistoryList.Count;

                    mulfuction.HistoryIDList = arrHistoryList;
                    mulfuction.DetectType = GetReactionString(1);
                    
                    mulfuction.FireCount = nFireCount;
                    mulfuction.MulFunctionCount = nMulFunctionCount;
                    mulfuction.Zone = zone;
                    mulfuction.ManagerName = FindManagerName(zone);
                    mulfuction.Notprocess = nNotprocessCount;
                    mulfuction.PercentMulFunction = PercentMulFunction;
                    //mulfuction.GroupName = zone.Building.BuildingGroup.BuildingGroupName;
                    //mulfuction.BuildingName = zone.Building.BuildingName;

                    string szBuildingName = zone.Building != null ? zone.Building.BuildingName : "";
                    string szGroupName = szBuildingName != "" ? zone.Building.BuildingGroup.BuildingGroupName : "";
                    string strFloorIndex = zone.Floor != null ? zone.Floor.ToString() : "";

                    if (szGroupName == "")
                        mulfuction.GroupName = "외부 영역";
                    else
                        mulfuction.GroupName = szGroupName;

                    if (szBuildingName == "")
                        mulfuction.BuildingName = zone.ZoneName;
                    else
                        mulfuction.BuildingName = szBuildingName;

                    mulfuction.FloorName = strFloorIndex;

                    arrMulFunction.Add(mulfuction);
                }
                //오작동이력로그들을 배열에 저장
                return arrMulFunction;
            }



            //ZoneID로 ReactionHistory의 수동신고Log를 가져온다
            private ArrayList GetManualReactionHistory(ArrayList arrZoneList, string startDate, string endDate)
            {
                //수동신고 목록을 저장 할 배열
                ArrayList arrManualReactionLog = new ArrayList();

                string strZoneList = "";
                int nCount = 1;
                foreach (Zone zone in arrZoneList)
                {
                    strZoneList += zone.ID.ToString();
                    if (nCount != arrZoneList.Count)
                        strZoneList += ",";

                    nCount++;
                }

                WebDBManager webDB = FormMain.Instance.DBManager;

                string strSQL = "select ID,SensorHistoryID,ReactionType, Time, Message, Param1, Param2, Param3, Param4, Param5 From SensorReactionHistory where SensorHistoryID in "
                         + "(select SensorHistoryID from SensorReactionHistory where param1 in(" + strZoneList + ") And ReactionType = 22 And Param2 = 0 And Time Between '" + startDate + "' and '" + endDate + "')";

                ArrayList arrResult = webDB.GetResultData(strSQL, 0);
                if (arrResult == null)
                    return null;

                int nResultCount = arrResult.Count;
                DateTime dt = DateTime.Now;

                for (int i = 0; i < nResultCount - 9; i += 10)
                {
                    int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    int nSensorHistoryID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                    int nReactionType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                    DateTime time = WebDBManager.GetDateTimeField(arrResult[i + 3], dt);
                    string strMessage = WebDBManager.GetStringField(arrResult[i + 4], "");
                    int Param1 = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                    string Param2 = WebDBManager.GetStringField(arrResult[i + 6].ToString(), "");
                    string Param3 = WebDBManager.GetStringField(arrResult[i + 7].ToString(), "");
                    string Param4 = WebDBManager.GetStringField(arrResult[i + 8].ToString(), "");
                    string Param5 = WebDBManager.GetStringField(arrResult[i + 9].ToString(), "");

                    SensorReactionLog reactionLog = new SensorReactionLog();
                    reactionLog.ID = nID;
                    reactionLog.SensorHistoryID = nSensorHistoryID;
                    reactionLog.ReactionType = nReactionType;
                    reactionLog.Time = time;
                    reactionLog.Param1 = Param1;
                    reactionLog.Message = strMessage;
                    reactionLog.Param2 = Param2;
                    reactionLog.Param3 = Param3;
                    reactionLog.Param4 = Param4;
                    reactionLog.Param5 = Param5;
                    reactionLog.SensorType = 0;

                    if (nReactionType == 22 || nReactionType == 21 || nReactionType == 23)
                    {
                        if (!m_dicHistoryMember.ContainsKey(nSensorHistoryID))
                            m_dicHistoryMember.Add(nSensorHistoryID, Param3);
                    }


                    //사내방송실시, 메세지(탐지/신고 여부)
                    //사내방송실시(탐지)
                    if (nReactionType == 10 && Param3 == "")
                    {
                        reactionLog.ReactionType = 101;
                    }
                    else if (nReactionType == 10 && Param3 != "") //사내방송실시(신고)
                    {
                        reactionLog.ReactionType = 102;
                    }

                    //문자메세지(탐지)
                    if (nReactionType == 11 && strMessage.Contains("탐지"))
                    {
                        reactionLog.ReactionType = 111;
                    }
                    else if (nReactionType == 11 && strMessage.Contains("신고"))
                    {
                        reactionLog.ReactionType = 112;
                    }

                    Zone zone = ZoneManager.Instance.GetZone(Param1);

                    if (zone != null)
                    {
                        if (!m_dicZoneHistorys.ContainsKey(nSensorHistoryID))
                            m_dicZoneHistorys.Add(nSensorHistoryID, zone);
                    }

                    reactionLog.SensorType = 0;

                    arrManualReactionLog.Add(reactionLog);

                    ArrayList arrLogs = null;

                    if (m_dicHistoryLog.ContainsKey(nSensorHistoryID))
                        arrLogs = m_dicHistoryLog[nSensorHistoryID];
                    else
                    {
                        arrLogs = new ArrayList();
                        m_dicHistoryLog[nSensorHistoryID] = arrLogs;
                    }
                    arrLogs.Add(reactionLog);
                }
                return arrManualReactionLog;
            }

            private ArrayList GetReactionHistory(ArrayList arrSensorHistoryID)
            {
                if (arrSensorHistoryID == null)
                    return null;

                string strSensorList = "";
                int nCount = 1;
                foreach (int nHistoryID in arrSensorHistoryID)
                {
                    strSensorList += nHistoryID.ToString();
                    if (nCount != arrSensorHistoryID.Count)
                        strSensorList += ",";

                    nCount++;
                }

                ArrayList arrReactionLog = new ArrayList();

                WebDBManager webDB = FormMain.Instance.DBManager;

                string strSQL = "select id, SensorHistoryID, ReactionType, Time, Message, Param1, Param2, Param3, Param4, Param5 from SensorReactionHistory ";
                strSQL += "where SensorHistoryID in (" + strSensorList + ")";

                ArrayList arrResult = webDB.GetResultData(strSQL, 0);
                if (arrResult == null)
                    return null;

                int nResultCount = arrResult.Count;
                DateTime dt = DateTime.Now;

                for (int i = 0; i < nResultCount - 9; i += 10)
                {
                    int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    int nSensorHistoryID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                    int nReactionType = WebDBManager.GetIntField(arrResult[i + 2].ToString(), -1);
                    DateTime time = WebDBManager.GetDateTimeField(arrResult[i + 3], dt);
                    string strMessage = WebDBManager.GetStringField(arrResult[i + 4], "");
                    int Param1 = WebDBManager.GetIntField(arrResult[i + 5].ToString(), -1);
                    string Param2 = WebDBManager.GetStringField(arrResult[i + 6].ToString(), "");
                    string Param3 = WebDBManager.GetStringField(arrResult[i + 7].ToString(), "");
                    string Param4 = WebDBManager.GetStringField(arrResult[i + 8].ToString(), "");
                    string Param5 = WebDBManager.GetStringField(arrResult[i + 9].ToString(), "");

                    SensorReactionLog reactionLog = new SensorReactionLog();
                    reactionLog.ID = nID;
                    reactionLog.SensorHistoryID = nSensorHistoryID;
                    reactionLog.ReactionType = nReactionType;
                    reactionLog.Time = time;

                    reactionLog.SensorType = 1;

                    if (nReactionType == 22 || nReactionType == 21 || nReactionType == 23)
                    {
                        if (!m_dicHistoryMember.ContainsKey(nSensorHistoryID))
                            m_dicHistoryMember.Add(nSensorHistoryID, Param3);
                    }

                    reactionLog.Param1 = Param1;

                    //자탐은 Param1이 EquipZoneID임


                    //Message에서 

                    reactionLog.Message = strMessage;
                    reactionLog.Param2 = Param2;
                    reactionLog.Param3 = Param3;
                    reactionLog.Param4 = Param4;
                    reactionLog.Param5 = Param5;


                    //사내방송실시, 메세지(탐지/신고 여부)
                    //사내방송실시(탐지)
                    if (nReactionType == 10 && Param3 == "")
                    {
                        reactionLog.ReactionType = 101;
                    }
                    else if (nReactionType == 10 && Param3 != "") //사내방송실시(신고)
                    {
                        reactionLog.ReactionType = 102;
                    }
                     
                    //문자메세지(탐지)
                    if (nReactionType == 11 && strMessage.Contains("탐지"))
                    {
                        reactionLog.ReactionType = 111;
                    }
                    else if (nReactionType == 11 && strMessage.Contains("신고"))
                    {
                        reactionLog.ReactionType = 112;
                    }

                    arrReactionLog.Add(reactionLog);

                    ArrayList arrLogs = null;

                    //
                    if (m_dicHistoryLog.ContainsKey(nSensorHistoryID))
                        arrLogs = m_dicHistoryLog[nSensorHistoryID];
                    else
                    {
                        arrLogs = new ArrayList();
                        m_dicHistoryLog[nSensorHistoryID] = arrLogs;
                    }
                    arrLogs.Add(reactionLog);
                }

                return arrReactionLog;
            }



            //SensorID로 SensorHistoryID를 찾아옴
            private ArrayList GetSensorZoneHistoryID(ArrayList arrSensorZoneID, string startDate, string endDate)
            {
                ArrayList arrSensorZoneHistoryID = new ArrayList();

                string strSensorList = "";
                int nCount = 1;
                foreach (int sensorID in arrSensorZoneID)
                {
                    strSensorList += sensorID.ToString();
                    if (nCount != arrSensorZoneID.Count)
                        strSensorList += ",";

                    nCount++;
                }

                WebDBManager webDB = FormMain.Instance.DBManager;

                string strSQL = "select id,SensorID from SensorZoneHistory where SensorID in (" + strSensorList + ") And Time Between '" + startDate + "' and '" + endDate + "' and (Connected = 1 and Data =1)";

                ArrayList arrResult = webDB.GetResultData(strSQL, 0);
                if (arrResult == null)
                    return null;

                int nResultCount = arrResult.Count;

                for (int i = 0; i < nResultCount - 1; i += 2)
                {
                    int nID = WebDBManager.GetIntField(arrResult[i].ToString(), -1);
                    int nSensorID = WebDBManager.GetIntField(arrResult[i + 1].ToString(), -1);
                    arrSensorZoneHistoryID.Add(nID);

                    ArrayList arrLogs = null;

                    if (m_dicSensorHistorys.ContainsKey(nSensorID))
                        arrLogs = m_dicSensorHistorys[nSensorID];
                    else
                    {
                        arrLogs = new ArrayList();
                        m_dicSensorHistorys[nSensorID] = arrLogs;
                    }
                    arrLogs.Add(nID);
                }
                return arrSensorZoneHistoryID;
            }

            private ArrayList FindHistoryID(int nZoneID)
            {
                ArrayList arrHistoryIDList = new ArrayList();
                Zone zone = ZoneManager.Instance.GetZone(nZoneID);
                //자탐Log
                ArrayList arrEquipmentZoneList = ZoneManager.Instance.GetEquipmentZoneList(zone);
                ArrayList arrSensorZoneList = FindSensorZone(arrEquipmentZoneList);

                if (arrSensorZoneList == null)
                    return null;

                foreach (int nSensorID in arrSensorZoneList)
                {
                    if (m_dicSensorHistorys.ContainsKey(nSensorID))
                        arrHistoryIDList.AddRange(m_dicSensorHistorys[nSensorID]);
                }

                foreach (int nHistoryID in arrHistoryIDList)
                {
                    if (!m_dicZoneHistorys.ContainsKey(nHistoryID))
                        m_dicZoneHistorys.Add(nHistoryID, zone);
                }

                return arrHistoryIDList;
            }

            //선택한 ZoneID로 EquipmentZoneID를 찾는다
            private ArrayList FindEquipZone(ArrayList arrZoneList)
            {
                ArrayList arrEquipZoneList = new ArrayList();
                if (arrZoneList == null)
                    return null;

                foreach (Zone zone in arrZoneList)
                {
                    if (ZoneManager.Instance.GetEquipmentZoneList(zone) == null)
                        continue;

                    arrEquipZoneList.AddRange(ZoneManager.Instance.GetEquipmentZoneList(zone));
                }

                //중복제거
                ArrayList arTemp = new ArrayList();
                foreach (EquipmentZone equipZone in arrEquipZoneList)
                {
                    if (!arTemp.Contains(equipZone))
                    {
                        arTemp.Add(equipZone);
                    }
                }
                arrEquipZoneList = arTemp;

                return arrEquipZoneList;

            }

            //EquipmentZoneID로 SensorID를 찾아온다
            private ArrayList FindSensorZone(ArrayList arrEquipZoneList)
            {
                ArrayList arrSensorZoneList = new ArrayList();
                if (arrEquipZoneList == null)
                    return null;

                foreach (EquipmentZone equip in arrEquipZoneList)
                {
                    if (SensorManager.Instance.FindZoneInSensor(equip.ID, Facility.FacilityType.FIRE_SENSOR) == -1)
                        continue;

                    //SensorZoneID 구함
                    arrSensorZoneList.Add(SensorManager.Instance.FindZoneInSensor(equip.ID, Facility.FacilityType.FIRE_SENSOR));
                }
                return arrSensorZoneList;
            }

            //담당자 찾아옴
            private string FindManagerName(Zone zone)
            {
                EquipmentZone equipZone = null;

                ArrayList arrEquipZoneList = new ArrayList();
                if (ZoneManager.Instance.GetEquipmentZoneList(zone) == null)
                    return null;

                arrEquipZoneList = ZoneManager.Instance.GetEquipmentZoneList(zone);
                if (arrEquipZoneList != null && arrEquipZoneList.Count > 0)
                {
                    equipZone = (EquipmentZone)arrEquipZoneList[0];
                }

                FacilityManagerGroup ManagerGroup = null;
                Building buildingFind = zone.Building;

                if (equipZone != null)
                {
                    ManagerGroup = FormMain.Instance.DataManager.GetEquipZoneFacilityManagerGroup(Facility.FacilityType.FIRE_SENSOR, equipZone);
                }
                if (ManagerGroup == null)
                {
                    //EquipmentZone으로 담당자를 못찾으면 Building으로 찾음
                    ManagerGroup = FormMain.Instance.DataManager.GetBuildingFacilityManagerGroup(Facility.FacilityType.FIRE_SENSOR, buildingFind);
                }
                if (ManagerGroup == null)
                {
                    ManagerGroup = FormMain.Instance.DataManager.GetEntireFacilityManagerGroup(Facility.FacilityType.FIRE_SENSOR);
                }

                string strPhoneNumber = "";
                string strManagerName = FormMain.Instance.DataManager.GetFacilityManagerName(ManagerGroup, ref strPhoneNumber);

                return strManagerName;
            }

            private string GetReactionString(int nType)
            {
                string strType = "";
                switch (nType)
                {
                    case 1: strType = "자탐 센서";
                        break;
                    case 2: strType = "소화 센서";
                        break;
                    case 3: strType = "압력 센서";
                        break;
                    case 4: strType = "수동 신고";
                        break;
                    default:
                        break;
                }

                return strType;
            }
        }

        public class SOPGenUSer
        {
            private int nMemberID = -1;

            public int MemberID
            {
                get { return nMemberID; }
                set { nMemberID = value; }
            }
            private string strNicName = "";

            public string NicName
            {
                get { return strNicName; }
                set { strNicName = value; }
            }
        }

        public class SensorReactionLog
        {
            private int nID = -1;
            private int nSensorHistoryID = -1;
            private int nReactionType = -1;
            private DateTime time;
            private int param1 = -1;
            private string strMessage = "";
            private string param2 = "";
            private string param3 = "";
            private string param4 = "";
            private string param5 = "";
            private int nSensorType = -1;

            public string Param2
            {
                get { return param2; }
                set { param2 = value; }
            }

            public string Param3
            {
                get { return param3; }
                set { param3 = value; }
            }

            public string Param4
            {
                get { return param4; }
                set { param4 = value; }
            }

            public string Param5
            {
                get { return param5; }
                set { param5 = value; }
            }

            public int ID
            {
                get { return nID; }
                set { nID = value; }
            }

            public int SensorHistoryID
            {
                get { return nSensorHistoryID; }
                set { nSensorHistoryID = value; }
            }

            public int ReactionType
            {
                get { return nReactionType; }
                set { nReactionType = value; }
            }

            public DateTime Time
            {
                get { return time; }
                set { time = value; }
            }

            public int Param1
            {
                get { return param1; }
                set { param1 = value; }
            }

            public string Message
            {
                get { return strMessage; }
                set { strMessage = value; }
            }

            public int SensorType
            {
                get { return nSensorType; }
                set { nSensorType = value; }
            }
        }


        class DetectLog : IComparable
        {
            private int nHistoryID = -1;
            private DateTime time;
            private string strDetectType = "";
            private string strManagerName = "";
            private int nZoneID = -1;
            private string strBuildingGroup = "";
            private string strBuildingName = "";
            private string strFloor = "";
            private EquipmentZone equipZone = null;

            public int HistoryID
            {
                get { return nHistoryID; }
                set { nHistoryID = value; }
            }

            public DateTime Time
            {
                get { return time; }
                set { time = value; }
            }

            public string DetectType
            {
                get { return strDetectType; }
                set { strDetectType = value; }
            }

            public string ManagerName
            {
                get { return strManagerName; }
                set { strManagerName = value; }
            }

            public int zoneID
            {
                get { return nZoneID; }
                set { nZoneID = value; }
            }

            public string BuildingGroup
            {
                get { return strBuildingGroup; }
                set { strBuildingGroup = value; }
            }


            public string BuildingName
            {
                get { return strBuildingName; }
                set { strBuildingName = value; }
            }

            public string FloorName
            {
                get { return strFloor; }
                set { strFloor = value; }
            }

            public EquipmentZone EquipZone
            {
                get { return equipZone; }
                set { equipZone = value; }
            }


            public DetectLog()
            {

            }

            public int CompareTo(object b)
            {
                DetectLog data = this;
                DetectLog data2 = (DetectLog)b;

                if (data.time > data2.time)
                    return 1;
                else if (data.time < data2.time)
                    return -1;
                else
                {
                    if (data.nHistoryID < data2.nHistoryID)
                        return -1;
                    else if (data.nHistoryID > data2.nHistoryID)
                        return 1;
                }

                return 0;
            }
        }

        class MulFunctionLog
        {
            private ArrayList nHistoryIDList = new ArrayList();

            public ArrayList HistoryIDList
            {
                get { return nHistoryIDList; }
                set { nHistoryIDList = value; }
            }
            private string strDetectType = "";

            public string DetectType
            {
                get { return strDetectType; }
                set { strDetectType = value; }
            }

            //탐지 횟수
            private int nReactionCount = 0;

            public int ReactionCount
            {
                get { return nReactionCount; }
                set { nReactionCount = value; }
            }
            //오작동 횟수
            private int nMulFunctionCount = 0;

            public int MulFunctionCount
            {
                get { return nMulFunctionCount; }
                set { nMulFunctionCount = value; }
            }
            //화재신고 횟수
            private int nFireCount = 0;

            public int FireCount
            {
                get { return nFireCount; }
                set { nFireCount = value; }
            }
            //처리되지 않음
            private int nNotprocess = 0;

            public int Notprocess
            {
                get { return nNotprocess; }
                set { nNotprocess = value; }
            }
            //오작동률
            private double nPercentMulFunction = 0;

            public double PercentMulFunction
            {
                get { return nPercentMulFunction; }
                set { nPercentMulFunction = value; }
            }
            private string strGroupName = "";

            public string GroupName
            {
                get { return strGroupName; }
                set { strGroupName = value; }
            }
            private string strBuildingName = "";

            public string BuildingName
            {
                get { return strBuildingName; }
                set { strBuildingName = value; }
            }
            private string strFloorName = "";

            public string FloorName
            {
                get { return strFloorName; }
                set { strFloorName = value; }
            }
            private string strManagerName = "";

            public string ManagerName
            {
                get { return strManagerName; }
                set { strManagerName = value; }
            }
            private Zone zone = null;

            public Zone Zone
            {
                get { return zone; }
                set { zone = value; }
            }


        }

        class ReactionLog : IComparable
        {
            private int nHistoryID = -1;
            public int HistoryID
            {
                get { return nHistoryID; }
                set { nHistoryID = value; }
            }
            private DateTime time;

            public DateTime Time
            {
                get { return time; }
                set { time = value; }
            }
            private string strManagerName = "";

            public string ManagerName
            {
                get { return strManagerName; }
                set { strManagerName = value; }
            }
            private int nSensorType = -1;

            public int SensorType
            {
                get { return nSensorType; }
                set { nSensorType = value; }
            }
            private string strBuildingName = "";

            public string BuildingName
            {
                get { return strBuildingName; }
                set { strBuildingName = value; }
            }
            private string strFloorName = "";

            public string FloorName
            {
                get { return strFloorName; }
                set { strFloorName = value; }
            }
            private int nReactionType = -1;

            public int Type
            {
                get { return nReactionType; }
                set { nReactionType = value; }
            }
            private Zone zone;

            public Zone Zone
            {
                get { return zone; }
                set { zone = value; }
            }

            public EquipmentZone equipZone;

            public EquipmentZone EquipZone
            {
                get { return equipZone; }
                set { equipZone = value; }
            }

            private ArrayList arrLogList = new ArrayList();
            public ArrayList ArrLogList
            {
                get { return arrLogList; }
                set { arrLogList = value; }
            }
            //화재신고위치
            private string strUserName = "";
            public string UserName
            {
                get { return strUserName; }
                set { strUserName = value; }
            }

            public override string ToString()
            {
                string strReactionType = "";
                if (nReactionType == 22)
                    strReactionType = "화재 발생";
                else if (nReactionType == 21)
                    strReactionType = "오작동 처리";
                else if (nReactionType == 23)
                    strReactionType = "화재탐지 후 상황해제";

                if (nSensorType == 0)
                    return time.ToString() + "    [ 수동 신고 ] " + strReactionType;
                else
                    return time.ToString() + "    [ 자탐 ] " + strReactionType;
            }

            public int CompareTo(object obj)
            {
                ReactionLog data = (ReactionLog)obj;

                if (this.time > data.time)
                    return 1;
                else if (this.time < data.time)
                    return -1;
                else
                {
                    if (this.nHistoryID < data.nHistoryID)
                        return -1;
                    else if (this.nHistoryID > data.nHistoryID)
                        return 1;
                }

                return 0;
            }

        }

    }
}