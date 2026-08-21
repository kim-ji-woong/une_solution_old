using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data.SQLite;

namespace SmartEye
{
    /// <summary>
    /// Service1의 요약 설명입니다.
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // ASP.NET AJAX를 사용하여 스크립트에서 이 웹 서비스를 호출하려면 다음 줄의 주석 처리를 제거합니다. 
    [System.Web.Script.Services.ScriptService]
    public class SmartEyeService : System.Web.Services.WebService
    {
        private string m_strServerURL = "";
        private static char SEPARATOR = (char)6;
        /*[WebMethod(Description = "Start Action")]
        public string StartAction(int nJobID, int nActionID, DateTime dtActTime, string strDescription)
        {
            WriteLog("StartAction");
            // true : Success.
            // flase : Fail.

            // DB Call..

            return String.Format("Start Action... {4}EventID = {0}{4}ActionID = {1}{4}Act Time = {2}{4}Description = {3}",
                nJobID, nActionID, dtActTime, strDescription, Environment.NewLine);

            //return true;
        }

        [WebMethod(Description = "End Action")]
        public bool EndAction(int nJobID, int nActionID, DateTime dtActTime, string strDescription)
        {
            WriteLog("EndAction");
            // true : Success.
            // flase : Fail.

            // DB Call..

            return true;
        }*/

        [WebMethod(MessageName = "SendActionData", Description = "현재 진행 단계를 서버에 전달합니다.<br>ActionID(0) : 수집단계<br>ActionID(1) : 분석 단계<br>ActionID(2) : 예측 단계<br>ActionID(3) : 시각화 단계<br>ActionID(4) : 경보 단계<br>ActionID(5) : 대응 단계")]
        public bool SendActionData(int disasterID, int actionID, string description)
        {
            WriteLogParam("SendActionData", disasterID, actionID, description);

            SQLiteConnection connection = MakeConnection();

            if (connection == null)
            {
                WriteLog("SendActionData false, MakeConnection fail");
                return false;
            }

            if (actionID == 5)
            {
                if (description == null || description.Length == 0)
                    description = GetReactionText();
                else
                    description += ";" + GetReactionText();
            }

            SQLiteTransaction tr = connection.BeginTransaction();

            DateTime dtNow = DateTime.Now;
            string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);

            // disasterID가 0일 경우 가장 마지막에 생성된 Disaster의 ID를 얻어오거나 새로 생성한다.
            if (!CheckDisasterID(connection, tr, ref disasterID, strTime))
            {
                WriteLog("SendActionData, CheckDisasterID Error");
                tr.Rollback();
                connection.Close();
                return false;
            }

            // disasterID에 해당하는 DB 데이터가 존재하는지 검사한다.
            string strSQL = "Select ActionID, Description from ActionStep where DisasterID = " + disasterID.ToString();
            SQLiteDataReader reader = null;

            try
            {
                SQLiteCommand cmd = new SQLiteCommand(strSQL, connection);
                cmd.Transaction = tr;

                reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string strValue = reader.GetValue(0).ToString();
                    string strOldDescription = reader.GetValue(1).ToString();
                    int nID;

                    if (int.TryParse(strValue, out nID))
                    {
                        reader.Close();

                        // actionID가 현재 단계보다 낮은 값이 오면 무시한다.
                        if (nID >= actionID)
                        {
                            WriteLog("Ignore ActionData, nID : " + nID.ToString() + ", actionID : " + actionID.ToString());
                            tr.Rollback();
                            connection.Close();
                            return false;
                        }

                        // DB 데이터가 존재하므로 업데이트 시킨다.
                        if (UpdateActionStep(connection, tr, disasterID, actionID, description, strOldDescription))
                        {
                            tr.Commit();
                            connection.Close();

                            // 경보단계일 경우 10초뒤 대응으로 바꾼다.
                            // 대응단계일 경우 10초뒤 ActionStep을 초기화한다.
                            SetReactionStepAfter10Seconds(disasterID, actionID);
                            return true;
                        }
                        else
                        {
                            WriteLog("UpdateActionStep Fail : ");
                            tr.Rollback();
                            connection.Close();
                            return false;
                        }
                    }
                }

                reader.Close();
            }
            catch (Exception e)
            {
                reader.Close();
            }

            // DB 데이터가 존재하지 않으므로 새로운 ActionStep을 생성시킨다.
            if (InsertActionStep(connection, tr, disasterID, actionID, description))
            {
                tr.Commit();
                connection.Close();

                // 경보단계일 경우 10초뒤 대응으로 바꾼다.
                // 대응단계일 경우 10초뒤 ActionStep을 초기화한다.
                SetReactionStepAfter10Seconds(disasterID, actionID);
                return true;
            }

            WriteLog("InsertActionStep Fail");
            tr.Rollback();
            connection.Close();
            return false;
        }

        // 경보단계일 경우 10초뒤 대응 단계로 바꾼다.
        // 대응단계일 경우 10초뒤 ActionStep을 초기화한다.
        private void SetReactionStepAfter10Seconds(int nDisasterID, int nOriginalActionStep)
        {
            if (nOriginalActionStep != 4 && nOriginalActionStep != 5)
                return;

            long argument = (((long)nDisasterID) << 32) | nOriginalActionStep;

            // 동기화 문제로 인하여 10초뒤 실행되도록 한다.
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(SetReactionStepAfter10SecondsThread));
            t.Start(argument);
        }

        private void SetReactionStepAfter10SecondsThread(object param)
        {
            // 10초간 대기...
            System.Threading.Thread.Sleep(10000);

            if (param != null && (param is long))
            {
                long argument = (long)param;
                int nDisasterID = (int)(argument >> 32);
                int nOriginalActionStep = (int)(argument & 0xffffffff);

                WriteLogParam("SetReactionStepAfter10SecondsThread", nDisasterID, nOriginalActionStep);

                SQLiteConnection connection = MakeConnection();

                if (connection == null)
                {
                    WriteLog("SetReactionStepAfter10SecondsThread false, MakeConnection fail");
                    return;
                }

                string strSQL = "";
                SQLiteTransaction tr = connection.BeginTransaction();

                try
                {
                    strSQL = "Select ActionID, Description from ActionStep where DisasterID = " + nDisasterID.ToString();

                    SQLiteCommand cmd = new SQLiteCommand(strSQL, connection);
                    cmd.Transaction = tr;

                    SQLiteDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        string strValue = reader.GetValue(0).ToString();
                        string strDescription = reader.GetValue(1).ToString();
                        int nID;

                        if (int.TryParse(strValue, out nID))
                        {
                            reader.Close();

                            // 10초동안 ActionStep에 변동이 없는지 확인한다.
                            if (nID == nOriginalActionStep)
                            {
                                if (nOriginalActionStep == 4)
                                {
                                    strDescription = MakeActionStepDescription(5, "", strDescription);
                                    strSQL = "Update ActionStep set ActionID = 5, Description = '" + strDescription + "' where DisasterID = " + nDisasterID.ToString();
                                    //strSQL = "Update ActionStep set ActionID = 5, Description = '" + GetReactionText() + "' where DisasterID = " + nDisasterID.ToString();
                                }
                                else// if (nOriginalActionStep == 5)
                                    strSQL = "Delete from ActionStep where DisasterID = " + nDisasterID.ToString();

                                cmd = new SQLiteCommand(strSQL, connection);
                                cmd.Transaction = tr;

                                cmd.ExecuteNonQuery();
                                tr.Commit();
                                connection.Close();

                                if (nOriginalActionStep == 4)
                                {
                                    // [대응] 단계로 바뀐 10초후 ActionStep을 초기화시킨다.
                                    SetReactionStepAfter10Seconds(nDisasterID, 5);
                                }

                                return;
                            }
                        }
                    }

                    reader.Close();
                }
                catch (Exception e)
                {
                    WriteLog("SQL : " + strSQL);
                    WriteLog("Caught exception : " + e.Message);
                }

                tr.Rollback();
                connection.Close();
            }
        }

        private string GetServerURL()
        {
            if (m_strServerURL != null && m_strServerURL.Length > 0)
                return m_strServerURL;

            string strFilePath = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\config.ini";
            System.IO.StreamReader reader = new System.IO.StreamReader(strFilePath, System.Text.Encoding.UTF8);

            string strResult = "";

            while (!reader.EndOfStream)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                // 주석
                if (strLine.StartsWith("#"))
                    continue;

                int nIndex = strLine.IndexOf(':');

                if (nIndex < 0)
                    continue;

                string strTag = strLine.Substring(0, nIndex).Trim();

                if (string.Compare(strTag, "Server_URL", true) != 0)
                    continue;

                strResult = strLine.Substring(nIndex + 1).Trim();
                break;
            }

            reader.Close();
            m_strServerURL = strResult;

            return m_strServerURL;
        }

        private string GetReactionText()
        {
            string strServerURL = GetServerURL();

            if (strServerURL == null || strServerURL.Length == 0)
                return "";

            if (!strServerURL.EndsWith("/"))
                strServerURL += "/";

            return "SmartEyeReactionImageLink:" + strServerURL + "ReactionImage.png";
        }

        // Return 값 : string[3]
        //             [0] : Action ID
        //             [1] : Disaster ID
        //             [2] : Description
        //             Action Data가 존재하지 않을 경우 배열의 크기는 1개이며, 그 값은 FAIL
        [WebMethod(MessageName = "GetActionData", Description = "현재 진행 단계정보를 얻어옵니다.")]
        public string[] GetActionData(int disasterID)
        {
            SQLiteConnection connection = MakeConnection();

            if (connection == null)
            {
                WriteLog("GetActionData false, MakeConnection fail");
                return null;
            }

            SQLiteTransaction tr = connection.BeginTransaction();

            if (!CheckValidDisasterID(connection, tr, ref disasterID))
            {
                tr.Rollback();
                connection.Close();
                return null;
            }

            string[] results = null;
            SQLiteDataReader reader = null;
            string strSQL = "Select ActionID, Description from ActionStep where DisasterID = " + disasterID.ToString();

            try
            {
                SQLiteCommand cmd = new SQLiteCommand(strSQL, connection);
                cmd.Transaction = tr;

                reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string strValue = reader.GetValue(0).ToString();
                    int nActionID;

                    if (!int.TryParse(strValue, out nActionID))
                    {
                        reader.Close();
                        tr.Rollback();
                        connection.Close();
                        return null;
                    }

                    string strDescription = reader.GetValue(1).ToString();
                    results = new string[3];

                    // (char)6은 xml 형태의 WebService로 전달할 수가 없다.
                    string strOldValue = SEPARATOR + "";
                    string strNewValue = "!@#$%^&*()";

                    results[0] = strValue;
                    results[1] = disasterID.ToString();
                    results[2] = strDescription.Replace(strOldValue, strNewValue);

                    reader.Close();
                    tr.Commit();
                    connection.Close();

                    return results;
                }

                reader.Close();
                tr.Commit();
                connection.Close();
            }
            catch (Exception e)
            {
                reader.Close();
                tr.Rollback();
                connection.Close();
                return null;
            }

            results = new string[1] { "FAIL" };
            return results;
        }

        private bool InsertActionStep(SQLiteConnection connection, SQLiteTransaction tr, int nDisasterID, int nActionID, string strDescription)
        {
            strDescription = MakeActionStepDescription(nActionID, strDescription, "");

            string strSQL = string.Format("Insert into ActionStep (ActionID, DisasterID, Description) values ({0}, {1}, '{2}')",
                nActionID, nDisasterID, strDescription);

            try
            {
                SQLiteCommand cmd = new SQLiteCommand(strSQL, connection);
                cmd.Transaction = tr;
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                WriteLog("Caught exception : " + e.Message);
                return false;
            }

            return true;
        }

        private string MakeActionStepDescription(int nActionID, string strDescription, string strOldDescription)
        {
            if (strOldDescription.Length == 0)
                return "[" + nActionID.ToString() + "]" + strDescription;

            string[] tokens = strOldDescription.Split(SEPARATOR);

            bool added = false;
            int nID;
            string strResult = "";

            foreach (string strToken in tokens)
            {
                string str = strToken.Trim();

                if (str.Length == 0)
                    continue;

                int nIndex1 = str.IndexOf('[');
                int nIndex2 = str.IndexOf(']');

                if (nIndex1 < 0 || nIndex2 < 0 || nIndex2 <= nIndex1)
                    continue;

                string strActionID = str.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);

                if (!int.TryParse(strActionID, out nID))
                    continue;

                if (nID == nActionID)
                {
                    AddDescription(ref strResult, nActionID, strDescription, SEPARATOR);
                    added = true;
                }
                else
                    AddDescription(ref strResult, nID, str.Substring(nIndex2 + 1), SEPARATOR);
            }

            if (!added)
                AddDescription(ref strResult, nActionID, strDescription, SEPARATOR);

            return strResult;
        }

        private void AddDescription(ref string strDescription, int nActionID, string str, char separator)
        {
            if (strDescription.Length == 0)
                strDescription = "[" + nActionID.ToString() + "]" + str;
            else
                strDescription += separator + "[" + nActionID.ToString() + "]" + str;
        }

        private bool UpdateActionStep(SQLiteConnection connection, SQLiteTransaction tr, int nDisasterID, int nActionID, string strDescription, string strOldDescription)
        {
            strDescription = MakeActionStepDescription(nActionID, strDescription, strOldDescription);

            string strSQL = string.Format("Update ActionStep set ActionID = {0}, Description = '{1}' where DisasterID = {2}",
                nActionID, strDescription, nDisasterID);
            
            try
            {
                SQLiteCommand cmd = new SQLiteCommand(strSQL, connection);
                cmd.Transaction = tr;
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                WriteLog("Caught exception : " + e.Message);
                return false;
            }

            return true;
        }

        [WebMethod(MessageName = "SendImageData", Description = "실시간 현장 이미지를 서버에 전달합니다.")]
        public bool SendImageData(string imageURL, double latitude, double longitude, string time_from_uav, string time, string description)
        {
            WriteLogParam("SendImageData", imageURL, latitude, longitude, time_from_uav, time, description);

            if (imageURL.Length == 0)
            {
                WriteLog("SendImageData false, imageURL.Length == 0");
                return false;
            }

            CheckTime(ref time_from_uav);
            CheckTime(ref time);

            SQLiteConnection connection = MakeConnection();

            if (connection == null)
            {
                WriteLog("SendImageData false, MakeConnection fail");
                return false;
            }

            //DateTime dtNow = DateTime.Now;
            //string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);
            string strTime = time;
            
            string strFormat = "Update RealTimeImage set ImageURL = '{0}', Latitude = {1}, Longitude = {2}, time_from_uav = '{3}', DateTime = '{4}', Description = {5}";
            string strSQL = string.Format(strFormat, imageURL, latitude, longitude, time_from_uav, strTime,
                description == null ? "NULL" : "'" + description + "'");
        
            try
            {
                SQLiteCommand cmd = new SQLiteCommand(strSQL, connection);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                WriteLog("Caught exception : " + e.Message);
                connection.Close();
                return false;
            }

            connection.Close();
            WriteLog("SendImageData success");
            return true;
        }

        private void WriteLog(string strLog)
        {
            DateTime dtNow = DateTime.Now;
            string strDate = string.Format("{0}_{1:00}_{2:00}", dtNow.Year, dtNow.Month, dtNow.Day);
            string strTime = string.Format("{0:00}:{1:00}:{2:00}, ", dtNow.Hour, dtNow.Minute, dtNow.Second);

            string szPath = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\log_" + strDate + ".txt";
            //string szPath = AppDomain.CurrentDomain.BaseDirectory + "log_" + strDate + ".txt";

            System.IO.StreamWriter writer = new System.IO.StreamWriter(szPath, true, System.Text.Encoding.UTF8);
            writer.WriteLine(strTime + strLog);
            writer.Close();
        }

        private void WriteLogParam(string strLog, params object[] args)
        {
            string strArguments = "";

            if (args == null)
            {
                WriteLog(strLog);
                return;
            }

            foreach (object obj in args)
            {
                string strObj = obj == null ? "null" : obj.ToString();

                if (strArguments.Length == 0)
                    strArguments = strObj;
                else
                    strArguments += ", " + strObj;
            }

            strArguments = "(" + strArguments + ")";
            WriteLog(strLog + strArguments);
        }

        // time : null이거나 빈문자열일 경우 현재 시간으로 설정한다.
        //        날짜가 없이 시간만 입력될 경우 날짜는 오늘 날짜로 한다.
        //        시간이 없이 날짜만 입력될 경우 시간은 0시 0분으로 한다.
        [WebMethod(MessageName = "BeginDisaster", Description = "재난이 발생하였음을 서버에 전달합니다.<br>리턴값은 재난의 ID가 되는데, 리턴값이 0이면 재난 생성에 실패한 것입니다.")]
        public int BeginDisaster(string station, string location, string etc, string time)
        {
            WriteLogParam("BeginDisaster", station, location, etc, time);
            CheckTime(ref time);

            if (station == null)
                station = "";

            if (location == null)
                location = "";

            SQLiteConnection connection = MakeConnection();

            if (connection == null)
            {
                WriteLog("BeginDisaster fail, MakeConnection Fail");
                return 0;
            }

            SQLiteTransaction tr = connection.BeginTransaction();

            int nID = GetMaxID(connection, tr, "Disaster") + 1;

            string strFormat = "Insert into Disaster (ID, StationName, LocationName, Etc, Time) values ({0}, '{1}', '{2}', '{3}', '{4}')";
            string strSQL = string.Format(strFormat, nID, station, location, etc, time);

            try
            {
                SQLiteCommand cmd = new SQLiteCommand(strSQL, connection);
                cmd.Transaction = tr;
                cmd.ExecuteNonQuery();
                tr.Commit();
            }
            catch (Exception e)
            {
                WriteLog("Caught exception : " + e.Message);
                tr.Rollback();
                connection.Close();
                return 0;
            }

            connection.Close();
            WriteLog("BeginDisaster Success");
            return nID;
        }

        // time : null이거나 빈문자열일 경우 현재 시간으로 설정한다.
        //        날짜가 없이 시간만 입력될 경우 날짜는 오늘 날짜로 한다.
        //        시간이 없이 날짜만 입력될 경우 시간은 0시 0분으로 한다.
        private void CheckTime(ref string strTime)
        {
            DateTime time = new DateTime();
            bool validate = false;

            if (strTime != null && strTime.Length > 0)
            {
                strTime = strTime.Trim();
                validate = DateTime.TryParse(strTime, out time);
            }

            if (!validate)
                time = DateTime.Now;

            strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
        }

        [WebMethod(MessageName = "EndDisaster", Description = "재난이 종료되었음을 서버에 전달합니다.")]
        public bool EndDisaster(int disasterID)
        {
            WriteLogParam("EndDisaster", disasterID);

            SQLiteConnection connection = MakeConnection();

            if (connection == null)
            {
                WriteLog("EndDisaster false, MakeConnection fail");
                return false;
            }

            SQLiteTransaction tr = connection.BeginTransaction();

            // disasterID가 0일 경우 가장 마지막에 생성된 Disaster의 ID를 얻어온다.
            if (!CheckValidDisasterID(connection, tr, ref disasterID))
            {
                tr.Rollback();
                connection.Close();
                return false;
            }

            string strSQL = "Delete from Disaster where ID = " + disasterID.ToString();

            try
            {
                // DisasterImage를 먼저 삭제한다.
                DeleteDisasterImage(connection, tr, disasterID);
                // ActionStep을 삭제한다.
                DeleteActionStep(connection, tr, disasterID);

                SQLiteCommand cmd = new SQLiteCommand(strSQL, connection);
                cmd.Transaction = tr;
                cmd.ExecuteNonQuery();

                // 다른 재난상황에 대한 이미지가 없다면 실시간 이미지도 초기화한다.
                /*int nFieldCount = GetMaxCount(connection, tr, "DisasterImage", "ImageURL");

                if (nFieldCount == 0)
                    ClearRealTimeImage(connection, tr);*/

                tr.Commit();
            }
            catch (Exception e)
            {
                WriteLog("Caught exception : " + e.Message);
                connection.Close();
                return false;
            }

            connection.Close();
            WriteLog("EndDisaster Success");
            return true;
        }

        private void DeleteActionStep(SQLiteConnection connection, SQLiteTransaction tr, int nDisasterID)
        {
            string strSQL = "Delete from ActionStep where DisasterID = " + nDisasterID.ToString();

            SQLiteCommand cmd = new SQLiteCommand(strSQL, connection);
            cmd.Transaction = tr;
            cmd.ExecuteNonQuery();
        }

        private void DeleteActionStep(SQLiteConnection connection, SQLiteTransaction tr)
        {
            string strSQL = "Delete from ActionStep";

            SQLiteCommand cmd = new SQLiteCommand(strSQL, connection);
            cmd.Transaction = tr;
            cmd.ExecuteNonQuery();
        }

        private void DeleteDisasterImage(SQLiteConnection connection, SQLiteTransaction tr, int nDisasterID)
        {
            string strSQL = "Delete from DisasterImage where DisasterID = " + nDisasterID.ToString();

            SQLiteCommand cmd = new SQLiteCommand(strSQL, connection);
            cmd.Transaction = tr;
            cmd.ExecuteNonQuery();
        }

        private void DeleteDisasterImage(SQLiteConnection connection, SQLiteTransaction tr)
        {
            string strSQL = "Delete from DisasterImage";

            SQLiteCommand cmd = new SQLiteCommand(strSQL, connection);
            cmd.Transaction = tr;
            cmd.ExecuteNonQuery();
        }

        private void ClearRealTimeImage(SQLiteConnection connection, SQLiteTransaction tr)
        {
            string strSQL = "Update RealTimeImage set ImageURL = '', Latitude = 0, Longitude = 0, DateTime = '', Description = NULL";

            SQLiteCommand cmd = new SQLiteCommand(strSQL, connection);
            cmd.Transaction = tr;
            cmd.ExecuteNonQuery();
        }

        [WebMethod(MessageName = "CloseAllDisasters", Description = "진행중인 모든 재난을 종료시킵니다.")]
        public bool CloseAllDisasters()
        {
            WriteLog("CloseAllDisasters");
            SQLiteConnection connection = MakeConnection();

            if (connection == null)
            {
                WriteLog("CloseAllDisasters fail, MakeConnection fail");
                return false;
            }

            SQLiteTransaction tr = connection.BeginTransaction();

            string strSQL = "Delete from Disaster";

            try
            {
                // DisasterImage를 먼저 삭제한다.
                DeleteDisasterImage(connection, tr);
                // ActionStep을 삭제한다.
                DeleteActionStep(connection, tr);

                SQLiteCommand cmd = new SQLiteCommand(strSQL, connection);
                cmd.Transaction = tr;
                cmd.ExecuteNonQuery();

                // 실시간 이미지도 초기화한다.
                //ClearRealTimeImage(connection, tr);

                tr.Commit();
            }
            catch (Exception e)
            {
                WriteLog("Caught exception : " + e.Message);
                tr.Rollback();
                connection.Close();
                return false;
            }

            connection.Close();
            WriteLog("CloseAllDisasters Success");
            return true;
        }

        // nDisasterID : 이 값이 0이면 가장 최근에 발생한 재난의 ID를 사용한다.
        //               최근에 발생한 재난이 없다면 재난 ID를 새로 생성하여 사용한다.
        // time : null이거나 빈문자열일 경우 현재 시간으로 설정한다.
        //        날짜가 없이 시간만 입력될 경우 날짜는 오늘 날짜로 한다.
        //        시간이 없이 날짜만 입력될 경우 시간은 0시 0분으로 한다.
        [WebMethod(MessageName = "SendDisasterImageData", Description = "재난 발생 이미지를 서버에 전달합니다.")]
        public int SendDisasterImageData(int disasterID, string imageURL, double latitude, double longitude, string time, string description)
        {
            WriteLogParam("SendDisasterImageData", disasterID, imageURL, latitude, longitude, time, description);

            if (imageURL.Length == 0)
            {
                WriteLog("SendDisasterImageData fail, imageURL.Length == 0");
                return 0;
            }

            CheckTime(ref time);

            SQLiteConnection connection = MakeConnection();

            if (connection == null)
            {
                WriteLog("SendDisasterImageData fail, MakeConnection fail");
                return 0;
            }

            SQLiteTransaction tr = connection.BeginTransaction();
            
            if (!CheckDisasterID(connection, tr, ref disasterID, time))
            {
                connection.Close();
                tr.Rollback();
                WriteLog("CheckDisasterID Fail");
                return 0;
            }

            string strSQL = "";

            try
            {
                strSQL = string.Format("Insert into DisasterImage (ImageURL, Latitude, Longitude, Time, DisasterID, Description) values ('{0}', {1}, {2}, '{3}', {4}, {5})",
                    imageURL, latitude, longitude, time, disasterID,
                    description == null ? "NULL" : "'" + description + "'");

                SQLiteCommand cmd = new SQLiteCommand(strSQL, connection);
                cmd.Transaction = tr;
                cmd.ExecuteNonQuery();
                tr.Commit();
            }
            catch (Exception e)
            {
                WriteLog("Caught exception : " + e.Message);
                tr.Rollback();
                connection.Close();
                return 0;
            }

            connection.Close();
            WriteLog("SendDisasterImageData Success");
            return disasterID;
        }

        private bool CheckValidDisasterID(SQLiteConnection connection, SQLiteTransaction tr, ref int nDisasterID)
        {
            if (nDisasterID == 0)
            {
                nDisasterID = GetLastDisasterID(connection, tr);
                return nDisasterID > 0;
            }

            // nDisasterID가 유효한 값인지 검사한다.
            string strSQL = "Select ID from Disaster where ID = " + nDisasterID.ToString();

            try
            {
                SQLiteCommand cmd = new SQLiteCommand(strSQL, connection);
                cmd.Transaction = tr;

                SQLiteDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string strValue = reader.GetValue(0).ToString();
                    int nID;

                    if (int.TryParse(strValue, out nID) && nID > 0)
                    {
                        reader.Close();
                        return true;
                    }
                }

                reader.Close();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("Caught exception : " + e.Message);
            }

            return false;
        }

        private bool CheckDisasterID(SQLiteConnection connection, SQLiteTransaction tr, ref int nDisasterID, string strTime)
        {
            if (nDisasterID > 0)
                return true;

            nDisasterID = GetMaxID(connection, tr, "Disaster");

            // nDisasterID가 0보다 크면 nDisasterID의 유효성을 검사하지 않는다.
            // nDisasterID가 유효하지 않을 경우 이후 Query에서 오류가 발생하여 걸러질 것이다.
            if (nDisasterID > 0)
                return true;

            // 가장 마지막에 발생한 Disaster의 ID를 얻어온다.
            nDisasterID = GetLastDisasterID(connection, tr);

            if (nDisasterID > 0)
                return true;

            // 현재 Disaster가 하나도 없을 경우 새로운 Disaster를 만든다.
            string strSQL = "Insert into Disaster (ID, StationName, LocationName, Etc, Time) values (1, '', '', '', '" + strTime + "')";

            try
            {
                SQLiteCommand cmd = new SQLiteCommand(strSQL, connection);
                cmd.Transaction = tr;
                cmd.ExecuteNonQuery();
                nDisasterID = 1;
            }
            catch (Exception e)
            {
                WriteLog("Caught exception : " + e.Message);
                return false;
            }

            return true;
        }

        private int GetLastDisasterID(SQLiteConnection connection, SQLiteTransaction tr)
        {
            // 가장 마지막에 DisasterImage가 등록된 Disaster의 ID를 얻어온다.
            string strSQL = "Select d.ID from DisasterImage as di, Disaster as d where di.Time = (Select max(Time) from DisasterImage) and di.DisasterID = d.ID";

            try
            {
                SQLiteCommand cmd = new SQLiteCommand(strSQL, connection);
                cmd.Transaction = tr;
                SQLiteDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string strValue = reader.GetValue(0).ToString();
                    int nID;

                    if (int.TryParse(strValue, out nID) && nID > 0)
                    {
                        reader.Close();
                        return nID;
                    }
                }

                reader.Close();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("Caught exception : " + e.Message);
            }

            // DisasterImage가 등록된 Disaster가 없을 경우, 가장 마지막에 등록된 Disaster의 ID를 얻어온다.
            return GetMaxID(connection, tr, "Disaster");
        }

        private int GetMaxID(SQLiteConnection connection, SQLiteTransaction tr, string strTableName)
        {
            int nID = 0;
            string strSQL = "Select max(ID) from " + strTableName;

            try
            {
                SQLiteCommand cmd = new SQLiteCommand(strSQL, connection);
                cmd.Transaction = tr;

                SQLiteDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string strID = reader.GetValue(0).ToString();
                    int.TryParse(strID, out nID);
                }

                reader.Close();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("Caught exception : " + e.Message);
            }

            return nID;
        }

        private int GetMaxCount(SQLiteConnection connection, SQLiteTransaction tr, string strTableName, string strFieldName)
        {
            int nCount = 0;
            string strSQL = "Select count(" + strFieldName + ") from " + strTableName;

            try
            {
                SQLiteCommand cmd = new SQLiteCommand(strSQL, connection);
                cmd.Transaction = tr;

                SQLiteDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string strCount = reader.GetValue(0).ToString();
                    int.TryParse(strCount, out nCount);
                }

                reader.Close();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("Caught exception : " + e.Message);
            }

            return nCount;
        }

        private SQLiteConnection MakeConnection()
        {
            string szPath = AppDomain.CurrentDomain.BaseDirectory;
            string szFilePath = szPath + "App_Data\\SmartEye.db";

            string strConnection = "Data Source=" + szFilePath;
            SQLiteConnection connection = null;

            try
            {
                connection = new SQLiteConnection(strConnection);
                connection.Open();
            }
            catch (Exception e)
            {
                WriteLog(e.Message);
                return null;
            }

            return connection;
        }

        // Return 값 : string[10]
        //             [0] : Image URL
        //             [1] : Latitude
        //             [2] : Longitude
        //             [3] : 촬영 시각
        //             [4] : Description
        //             [5] : 관측소 이름
        //             [6] : 재난발생 장소 이름
        //             [7] : 기타
        //             [8] : 재난발생 시각
        //             [9] : Disaster ID
        //             재난 Image가 존재하지 않을 경우 배열의 크기는 1개이며, 그 값은 FAIL
        [WebMethod(MessageName = "GetDisasterImageData", Description = "재난 발생 이미지를 받아옵니다.")]
        public string[] GetDisasterImageData()
        {
            //WriteLog("GetDisasterImageData");
            SQLiteConnection connection = MakeConnection();

            if (connection == null)
            {
                WriteLog("GetDisasterImageData fail, MakeConnection fail");
                return null;
            }

            string[] results = null;
            string strSQL = "Select di.ImageURL, di.Latitude, di.Longitude, di.Time, di.Description, d.StationName, d.LocationName, d.Etc, d.Time, d.ID from DisasterImage as di, Disaster as d where di.Time = (Select max(Time) from DisasterImage) and di.DisasterID = d.ID";

            try
            {
                SQLiteCommand cmd = new SQLiteCommand(strSQL, connection);
                SQLiteDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    if (reader.FieldCount != 10)
                    {
                        reader.Close();
                        connection.Close();
                        return null;
                    }

                    results = new string[10];

                    results[0] = reader.GetValue(0).ToString();
                    results[1] = reader.GetValue(1).ToString();
                    results[2] = reader.GetValue(2).ToString();
                    results[3] = reader.GetValue(3).ToString();

                    object objDescription = reader.GetValue(4);

                    if (objDescription == null)
                        results[4] = "";
                    else
                        results[4] = objDescription.ToString();

                    results[5] = reader.GetValue(5).ToString();
                    results[6] = reader.GetValue(6).ToString();
                    results[7] = reader.GetValue(7).ToString();
                    results[8] = reader.GetValue(8).ToString();
                    results[9] = reader.GetValue(9).ToString();
                }

                reader.Close();
            }
            catch (Exception e)
            {
                WriteLog("GetDisasterImageData Fail : " + e.Message);
                connection.Close();
                return null;
            }

            connection.Close();

            if (results == null)
                results = new string[1] { "FAIL" };

            //WriteLog("GetDisasterImageData Success, results Count : " + results.Length.ToString());
            return results;
        }

        // Return 값 : string[5]
        //             [0] : Image URL
        //             [1] : Latitude
        //             [2] : Longitude
        //             [3] : 촬영 시각
        //             [4] : Description
        //             Image가 존재하지 않을 경우 배열의 크기는 1개이며, 그 값은 FAIL
        [WebMethod(MessageName = "GetImageData", Description = "실시간 현장 이미지를 받아옵니다.")]
        public string[] GetImageData()
        {
            //WriteLog("GetImageData");
            SQLiteConnection connection = MakeConnection();

            if (connection == null)
            {
                WriteLog("GetImageData fail, MakeConnection fail");
                return null;
            }

            string[] results = null;
            string strSQL = "Select ImageURL, Latitude, Longitude, time_from_uav, Description from RealTimeImage";

            try
            {
                SQLiteCommand cmd = new SQLiteCommand(strSQL, connection);
                SQLiteDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    if (reader.FieldCount != 5)
                    {
                        reader.Close();
                        connection.Close();
                        return null;
                    }

                    string strImageURL = reader.GetValue(0).ToString();

                    if (strImageURL == null || strImageURL.Length == 0)
                    {
                        reader.Close();
                        connection.Close();

                        results = new string[1] { "FAIL" };
                        return results;
                    }

                    results = new string[5];

                    results[0] = strImageURL;
                    results[1] = reader.GetValue(1).ToString();
                    results[2] = reader.GetValue(2).ToString();
                    results[3] = reader.GetValue(3).ToString();

                    object objDescription = reader.GetValue(4);

                    if (objDescription == null)
                        results[4] = "";
                    else
                        results[4] = objDescription.ToString();
                }

                reader.Close();
            }
            catch (Exception e)
            {
                WriteLog("GetImageData Fail : " + e.Message);
                connection.Close();
                return null;
            }

            connection.Close();

            if (results == null)
                results = new string[1] { "FAIL" };

            //WriteLog("GetImageData Success, results Count : " + results.Length.ToString());
            return results;
        }
    }
}