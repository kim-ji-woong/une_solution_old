using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Collections;
using System.Data.SqlClient;
using System.Data.SQLite;
using DBUtility;
using System.Threading;

namespace IntegratedManagement2
{
    public class SimulationDBManager
    {
        class DataType
        {
            private string m_strFieldName = "";
            private bool m_isTextFormat = false;
            private bool m_isNullable = false;

            public string FieldName
            {
                get { return m_strFieldName; }
                set { m_strFieldName = value; }
            }

            public bool TextFormat
            {
                get { return m_isTextFormat; }
                set { m_isTextFormat = value; }
            }

            public bool IsNullable
            {
                get { return m_isNullable; }
                set { m_isNullable = value; }
            }
        }

        public enum LocalDBMode { CANNOT_USE = 0, PROCESSING, PREPARED };

        private WebDBManager m_dbMgr = null;
        private LocalDBMode m_mode = LocalDBMode.CANNOT_USE;
        private List<string> m_queries = new List<string>();
        private int m_nProcessedQueryCount = 0;
        private string m_strDBFilePath = "";
        private string m_strPW = "";
        private bool m_isActivated = false;

        public string DBFilePath
        {
            get { return m_strDBFilePath; }
        }

        public string DBPassword
        {
            get { return m_strPW; }
        }

        public LocalDBMode PrepareStatus
        {
            get { return m_mode; }
        }

        public int TotalQueryCount
        {
            get { return m_queries.Count; }
        }

        public int ProcessedQueryCount
        {
            get { return m_nProcessedQueryCount; }
        }

        public bool IsActivated
        {
            get { return m_isActivated; }
        }

        public SimulationDBManager(WebDBManager dbMgr)
        {
#if SIMULATION_MODE
            m_isActivated = true;
#endif

            m_dbMgr = dbMgr;

            if (m_isActivated)
            {
                SQLiteConnection connection = MakeConnection();

                if (connection != null)
                {
                    // 새로 만들지 않고 기존에 만들어진 것을 사용한다.
                    m_mode = LocalDBMode.PREPARED;
                    //Thread t = new Thread(MakeLocalDB);
                    //t.Start(connection);
                }
            }
        }

        private SQLiteConnection MakeConnection()
        {
            m_strDBFilePath = System.Windows.Forms.Application.StartupPath + "\\SOPSimulation.db3";

            /*try
            {
                if (System.IO.File.Exists(m_strDBFilePath))
                    System.IO.File.Delete(m_strDBFilePath);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                return null;
            }*/

            m_strPW = new string(new char[] { '9', '4', '4', '9', '9', '6', '6', 'A', 'b' });
            //m_strPW = "";
            string strConnection = "";

            if (m_strPW.Length == 0)
                strConnection = "Data Source=" + m_strDBFilePath;
            else
                strConnection = "Data Source=" + m_strDBFilePath + ";Password=" + m_strPW;

            SQLiteConnection connection = null;
            
            try
            {
                connection = new SQLiteConnection(strConnection);
                connection.Open();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                return null;
            }

            return connection;
        }

        private void MakeLocalDB(object param)
        {
            SQLiteConnection connection = (SQLiteConnection)param;

            System.Diagnostics.Trace.WriteLine("MakeBeginTime : " + DateTime.Now.ToLongTimeString());
            m_mode = LocalDBMode.PROCESSING;
            m_nProcessedQueryCount = 0;

            ArrayList arrResult = m_dbMgr.GetResultData("select name from sys.Tables", 0);

            string[] arrDataTables = new string[] { "ActionStep", "Annotation", "Arrow", "BluePrint", 
                "Building", "BuildingGroup", "CCTV", "CheckTask", "CompanyMember", "Decision", "Disaster",
                "DisasterCategory", "EndPoint", "EquipmentZone", "EquipZoneCCTV", "EquipZoneCCTVTeamp",
                "ExternalCompanyMember", "ExternalCompanyTeam", "ExternalMemberList", "ExternalTeam", "ExternalTransmission",
                "FireEquipment", "FireEquipmentGroup", "FireSensor", "InternalTransmission",
                "InternalTransmissionMessageType", "JobLevel", "JobPosition", "Link", "MonitoringSensor",
                "OptionSDMS", "OptionSOPSimulator", "Process", "ProcessMission", "PumpPressureSensor", "RegularMemberList",
                "RegularTeam", "SDMSBroadcastConfig", "SDMSEditPassword", "SDMSSMSConfig", "SDMSServerPort",
                "SDMSSMSConfig", "SectionGroup", "SensorServerInfo", "SensorTagInfo", "SensorZone",
                "Site", "SOPGenLevel", "SOPGenUser", "SpringCooler", "StepMember", "SubDisasterCategory",
                "TeamVersion", "TemporaryEmergencyTeam", "TemporaryEmergencyTeamHistory", "TemporaryMemberList",
                "TemporaryNormalTeam", "TemporaryNormalTeamHistory", "Transmission", "TransSOP",
                "UserDefinedTeam", "Version", "Zone"};

            int nDataTableCount = arrDataTables.Count();

            for (int i = 0; i < nDataTableCount;i++)
            {
                string strDataTable = arrDataTables[i];
                arrDataTables[i] = strDataTable.ToLower();
            }

            string strConn = @"Data Source=:memory:";
            SQLiteConnection connectionMemory = null;

            try
            {
                connectionMemory = new SQLiteConnection(strConn);
                connectionMemory.Open();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                connection.Close();
                m_mode = LocalDBMode.CANNOT_USE;
                System.Diagnostics.Trace.WriteLine("MakeFailTime : " + DateTime.Now.ToLongTimeString());
                return;
            }

            foreach (string strTableName in arrResult)
            {
                if (!MakeLocalDB(connectionMemory, strTableName, arrDataTables, m_queries))
                {
                    connection.Close();
                    m_mode = LocalDBMode.CANNOT_USE;
                    System.Diagnostics.Trace.WriteLine("MakeFailTime : " + DateTime.Now.ToLongTimeString());
                    return;
                }
            }

            System.Diagnostics.Trace.WriteLine("ReadDBTime : " + DateTime.Now.ToLongTimeString());

            if (InsertSQLite(connection, connectionMemory, m_queries))
                m_mode = LocalDBMode.PREPARED;
            else
                m_mode = LocalDBMode.CANNOT_USE;

            System.Diagnostics.Trace.WriteLine("MakeEndTime : " + DateTime.Now.ToLongTimeString());
        }

        private bool InsertSQLite(SQLiteConnection connection, SQLiteConnection connectionMemory, List<string> queries)
        {
            string strSQL = "";

            try
            {
                int len = queries.Count;

                for (int i = 0; i < len; i++)
                {
                    strSQL = queries[i];
                    SQLiteCommand cmd = new SQLiteCommand(strSQL, connectionMemory);
                    cmd.ExecuteNonQuery();
                    m_nProcessedQueryCount++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Caught exception: " + e.Message);
                if (connectionMemory != null)
                {
                    connectionMemory.Close();
                    connection.Close();
                    return false;
                }
            }
            finally
            {

            }

            connectionMemory.BackupDatabase(connection, "main", "main", -1, null, 0);
            connection.Close();
            connectionMemory.Close();

            return true;
        }

        private bool MakeLocalDB(SQLiteConnection connection, string strTableName, string[] arrDataTables, List<string> queries)
        {
            //string strSQL = "Select COLUMN_NAME, DATA_TYPE, IS_NULLABLE from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = N'" + strTableName + "'";
            string strSQL = "SELECT COLUMN_NAME, data_type, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '" + strTableName + "'";
            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            string strIdentityID = "";
            //ID 자동증가라면
            bool isIdentity = IsAutoID(strTableName, ref strIdentityID);

            string strLowerTableName = strTableName.ToLower();
            List<DataType> datas = new List<DataType>();

            strSQL = "";
            int nResultCount = arrResult.Count;

            for (int i=0;i<nResultCount-3;i+=4)
            {
                string strFieldName = WebDBManager.GetStringField(arrResult[i], "");
                string strType = WebDBManager.GetStringField(arrResult[i + 1], "");
                string strCharLength = WebDBManager.GetStringField(arrResult[i + 2], "");
                string strNullCheck = WebDBManager.GetStringField(arrResult[i + 3], "");

                if (strFieldName.Length == 0 || strType.Length == 0 || strNullCheck.Length == 0)
                    continue;

                DataType type = new DataType();
                type.FieldName = strFieldName;
                type.TextFormat = false;

                if (strType.Contains("char"))
                {
                    if (strCharLength != "null")
                        strType += "(" + strCharLength + ")";
                    type.TextFormat = true;
                }
                else if (strType == "int")
                    strType = "integer";
                else
                {
                    string strLowerType = strType.ToLower();

                    if (strLowerType == "text" || strLowerType == "datetime")
                        type.TextFormat = true;
                }

                if (strSQL.Length != 0)
                    strSQL += ", ";

                if (string.Compare(strNullCheck, "YES", true) == 0)
                {
                    if (isIdentity && strFieldName == strIdentityID)
                        strSQL += "[" + strFieldName + "] [" + strType + "] NULL PRIMARY KEY AUTOINCREMENT";
                    else
                        strSQL += "[" + strFieldName + "] [" + strType + "] NULL";

                    type.IsNullable = true;
                }
                else
                {
                    if (isIdentity && strFieldName == strIdentityID)
                        strSQL += "[" + strFieldName + "] [" + strType + "] NOT NULL PRIMARY KEY AUTOINCREMENT";
                    else
                        strSQL += "[" + strFieldName + "] [" + strType + "] NOT NULL";

                    type.IsNullable = false;
                }

                datas.Add(type);
            }

            if (strSQL.Length > 0)
            {
                strSQL = "CREATE TABLE " + strTableName + " (" + strSQL + ")";

                try
                {
                    SQLiteCommand command = new SQLiteCommand(strSQL, connection);
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                    return false;
                }
            }

            if (arrDataTables.Contains(strLowerTableName))
            {
                strSQL = "Select * from " + strTableName;
                arrResult = m_dbMgr.GetResultData(strSQL, 0);

                nResultCount = arrResult.Count;
                int nFieldCount = datas.Count();

                for (int i=0;i<nResultCount-(nFieldCount - 1);i+=nFieldCount)
                {
                    List<string> fieldDatas = new List<string>();

                    for (int j = 0; j < nFieldCount; j++)
                    {
                        string strFieldData = WebDBManager.GetStringField(arrResult[i + j], "");

                        if (strFieldData.Length == 0 || strFieldData == "null")
                        {
                            if (datas[j].IsNullable)
                                fieldDatas.Add("NULL");
                            else if (datas[j].TextFormat)
                                fieldDatas.Add("''");
                            else
                                fieldDatas.Add("0");
                        }
                        else
                        {
                            if (datas[j].TextFormat)
                            {
                                int len = strFieldData.Length;
                                string strData = "";

                                for (int k = 0; k < len; k++)
                                {
                                    char ch = strFieldData.ElementAt(k);

                                    if (ch == '\'')
                                        strData += "''";
                                    else
                                        strData += ch;
                                }

                                fieldDatas.Add("'" + strData + "'");
                            }
                            else
                                fieldDatas.Add(strFieldData);
                        }
                    }

                    if (fieldDatas.Count == 0)
                        continue;

                    string strHeader = "Insert into " + strTableName + " (";
                    string strTail = ") values (";

                    for (int j=0;j<nFieldCount;j++)
                    {
                        if (j == 0)
                        {
                            strHeader += datas[j].FieldName;
                            strTail += fieldDatas[j];
                        }
                        else
                        {
                            strHeader += ", " + datas[j].FieldName;
                            strTail += ", " + fieldDatas[j];
                        }
                    }

                    queries.Add(strHeader + strTail + ");");
                    fieldDatas.Clear();
                }
            }

            return true;
        }

        private bool IsAutoID(string strTableName, ref string strIdentityID)
        {
            string strSQL = "select ic.name, tb.name from sys.identity_columns as ic ";
            strSQL += "join sys.tables as tb on ic.object_id = tb.object_id where tb.name = '" + strTableName + "'";

            ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;
            if (arrResult.Count == 0)
                return false;

            strIdentityID = WebDBManager.GetStringField(arrResult[0], "");
            return true;
        }
    }
}
