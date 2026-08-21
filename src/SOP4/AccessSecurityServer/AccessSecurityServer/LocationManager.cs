using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using DBUtility;
using System.Collections;

namespace AccessSecurityServer
{
    public class LocationManager
    {
        private Dictionary<int, string> m_dicLocationName = null;
        private int m_nSOPLocationCount = 0;

        private static LocationManager m_instance = null;

        public static LocationManager Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new LocationManager();

                return m_instance;
            }
        }

        private LocationManager()
        {
        }

        // 새로운 Location이 추가되었을 경우 관련정보를 로그에 기록한다.
        public bool CheckLocation(string strS1AccessDBConnection, WebDBManager dbMgr, LocationManagerOwner owner)
        {
            lock (this)
            {
                try
                {
                    SqlConnection accessDBConnection = new SqlConnection();
                    accessDBConnection.ConnectionString = strS1AccessDBConnection;
                    accessDBConnection.Open();

                    if (accessDBConnection.State != System.Data.ConnectionState.Open)
                        return false;

                    bool isFirst = false;
                    int nChangedCount = 0;
                    Dictionary<int, string> newLocationNames = null;
                    Dictionary<int, string> changedLocationNames = null;

                    if (!ReadAccessLocation(accessDBConnection, ref isFirst, ref newLocationNames, ref changedLocationNames))
                    {
                        accessDBConnection.Close();
                        return false;
                    }

                    // 바뀐 정보가 없으면 SOP DB는 읽지 않아도 무방하다.
                    if (isFirst == false && changedLocationNames == null && m_nSOPLocationCount == m_dicLocationName.Count)
                    {
                        SetStatusLog("변동사항 없음", owner);
                        accessDBConnection.Close();
                        return true;
                    }

                    if (!ReadSOPLocation(dbMgr, isFirst, ref newLocationNames, ref changedLocationNames, ref nChangedCount))
                    {
                        accessDBConnection.Close();
                        return false;
                    }

                    if (WriteLogNewLocation(newLocationNames, owner) == false)
                    {
                        string strStatus = "";

                        if (nChangedCount > 0)
                            strStatus += nChangedCount.ToString() + "개의 영역정보가 변경되었습니다.";
                        else
                            strStatus += "변동사항 없음";

                        SetStatusLog(strStatus, owner);
                        accessDBConnection.Close();
                        return true;
                    }

                    accessDBConnection.Close();
                }
                catch (Exception e)
                {
                    ConnectionLogEx.Instance.WriteLine(e.Message);
                    return false;
                }
            }

            return true;
        }

        private void SetStatusLog(string strLog, LocationManagerOwner owner)
        {
            DateTime now = DateTime.Now;
            string strStatus = string.Format("마지막 Location DB 비교({0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}) : ",
                now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);

            ConnectionLogEx.Instance.WriteLine(strStatus + strLog);

            if (owner != null)
            {
                owner.SetStatus(strStatus + strLog, System.Drawing.Color.Black);
            }
        }

        // Return 값 : 새로운 영역정보가 있으면 true를 리턴한다.
        private bool WriteLogNewLocation(Dictionary<int, string> newLocationNames, LocationManagerOwner owner)
        {
            if (newLocationNames == null)
            {
                return false;
            }

            foreach (KeyValuePair<int, string> pair in newLocationNames)
            {
                ConnectionLogEx.Instance.WriteLine("[S1 Access DB 신규 Location 추가] : LocationID(" + pair.Key.ToString() + "), " + pair.Value);
            }

            if (newLocationNames.Count > 0)
            {
                DateTime now = DateTime.Now;
                string strStatus = string.Format("마지막 Location DB 비교({0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00} : 새로운 영역정보가 추가되었습니다.",
                    now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);

                ConnectionLogEx.Instance.WriteLine(strStatus);

                if (owner != null)
                {
                    owner.SetStatus(strStatus, System.Drawing.Color.Red);

                    return true;
                }
            }

            return false;
        }

        private bool ReadSOPLocation(WebDBManager dbMgr, bool isFirst, ref Dictionary<int, string> newLocationNames, ref Dictionary<int, string> changedLocationNames, ref int nChangedCount)
        {
            string strSQL = "select LocationID, EquipZoneID, ez.DisplayText ";
            strSQL += "from AccessLink_View_External_Location as el, EquipmentZone as ez ";
            strSQL += "where el.EquipZoneID = ez.ID";

            ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            List<int> newLocationIDs = null;

            if (isFirst || m_nSOPLocationCount != m_dicLocationName.Count)
            {
                newLocationIDs = new List<int>();

                foreach (KeyValuePair<int, string> pair in m_dicLocationName)
                {
                    newLocationIDs.Add(pair.Key);
                }
            }

            string strLocationName = "";
            int nResultCount = arrResult.Count;

            int nSOPLocationCount = 0;

            for (int i=0;i<nResultCount-2;i+=3)
            {
                VariousData<int> locationID = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> equipZoneID = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                string strDisplayText = WebDBManager.GetStringField(arrResult[i + 2]);

                if (locationID == null || equipZoneID == null)
                    continue;

                nSOPLocationCount++;

                if (newLocationIDs != null)
                    newLocationIDs.Remove(locationID.Data);

                if (isFirst)
                {
                    if (m_dicLocationName.TryGetValue(locationID.Data, out strLocationName) == false)
                    {
                        // 해당 영역의 정보가 삭제되었다.
                        // 삭제된 정보는 무시한다.
                    }
                    else if (strLocationName != strDisplayText)
                    {
                        UpdateEquipZoneDisplayText(dbMgr, equipZoneID.Data, strLocationName);
                        nChangedCount++;

                        if (changedLocationNames == null)
                            changedLocationNames = new Dictionary<int, string>();

                        changedLocationNames[locationID.Data] = strLocationName;
                    }
                }
                else if (changedLocationNames != null)
                {
                    if (changedLocationNames.TryGetValue(locationID.Data, out strLocationName))
                    {
                        UpdateEquipZoneDisplayText(dbMgr, equipZoneID.Data, strLocationName);
                        nChangedCount++;
                    }
                }
            }

            m_nSOPLocationCount = nSOPLocationCount;

            // 추가된 영역정보가 존재한다.
            if (newLocationIDs != null && newLocationIDs.Count > 0)
            {
                if (newLocationNames == null)
                    newLocationNames = new Dictionary<int, string>();

                foreach (int nLocationID in newLocationIDs)
                {
                    newLocationNames[nLocationID] = m_dicLocationName[nLocationID];
                }
            }

            return true;
        }

        private bool UpdateEquipZoneDisplayText(WebDBManager dbMgr, int nEquipZoneID, string strLocationName)
        {
            string strSQL = string.Format("Update EquipmentZone set DisplayText = '{0}' where ID = {1}", strLocationName, nEquipZoneID);
            return dbMgr.GetResultData(strSQL, 0) != null;
        }

        private bool ReadAccessLocation(SqlConnection accessDBConnection, ref bool isFirst, ref Dictionary<int, string> newLocationNames, ref Dictionary<int, string> changedLocationNames)
        {
            try
            {
                string strSQL = "Select LocationID, LocationName from View_External_Location";
                SqlCommand cmd = new SqlCommand(strSQL, accessDBConnection);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader == null)
                    return false;

                if (m_dicLocationName == null)
                {
                    isFirst = true;
                    m_dicLocationName = new Dictionary<int, string>();
                }

                string strTempName = "";

                while (reader.Read())
                {
                    if (reader.IsDBNull(0) || reader.IsDBNull(1))
                        continue;

                    int nLocationID = (int)reader[0];
                    string strLocationName = reader[1].ToString();

                    if (isFirst)
                    {
                        m_dicLocationName[nLocationID] = strLocationName;
                    }
                    else
                    {
                        if (m_dicLocationName.TryGetValue(nLocationID, out strTempName) == false)
                        {
                            if (newLocationNames == null)
                                newLocationNames = new Dictionary<int, string>();

                            // 새로운 영역정보가 생성되었다.
                            newLocationNames[nLocationID] = strTempName;
                            m_dicLocationName[nLocationID] = strTempName;
                        }
                        else if (strTempName != strLocationName)
                        {
                            if (changedLocationNames == null)
                                changedLocationNames = new Dictionary<int, string>();

                            // 영역정보가 변경되었다.
                            changedLocationNames[nLocationID] = strTempName;
                            m_dicLocationName[nLocationID] = strTempName;
                        }
                    }
                }

                reader.Close();
            }
            catch (Exception e)
            {
                ConnectionLogEx.Instance.WriteLine(e.Message);
                return false;
            }

            return true;
        }
    }

    public interface LocationManagerOwner
    {
        void SetStatus(string strStatus, System.Drawing.Color fontColor);
    }
}
