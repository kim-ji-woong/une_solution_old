using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Data.SqlClient;

namespace XmlToDB
{
    public class DBManager
    {
        public enum UnitType { Unknown = -1, MM = 0, CM, M, KM };

        private string m_strErrorMessage = "";
        // Key : XML ComponentID
        // Value : DB Component ID
        private Dictionary<string, int> m_dicComponentID = new Dictionary<string, int>();
        private Dictionary<string, int> m_dicWallID = new Dictionary<string, int>();
        private Dictionary<string, int> m_dicGridID = new Dictionary<string, int>();

        private Dictionary<string, int> m_dicPOITypeID = new Dictionary<string, int>();

        private const string TARGET_VERSION = "1.3";

        public string ErrorMessage
        {
            get { return m_strErrorMessage; }
        }

        public bool XmlToDB(string strFilePath, SqlConnection connection)
        {
            try
            {
                m_strErrorMessage = "";

                // 트랜잭션시작
                SqlTransaction transaction = connection.BeginTransaction();

                m_dicComponentID.Clear();
                m_dicWallID.Clear();
                m_dicGridID.Clear();
                m_dicPOITypeID.Clear();

                int nProjectID = ReadCommonDatas(strFilePath, connection, transaction);

                if (nProjectID < 0)
                {
                    transaction.Rollback();
                    return false;
                }

                if (ReadAllExceptCommon(strFilePath, nProjectID, connection, transaction) == false)
                {
                    transaction.Rollback();
                    return false;
                }

                transaction.Commit();
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool ReadAllExceptCommon(string strFilePath, int nProjectID, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                bool stop = false;

                XmlTextReader reader = new XmlTextReader(strFilePath);

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "IndoorModelFile", true) == 0)
                            {
                                if (ReadIndoorModelFile(reader, false, nProjectID, connection, transaction) <= 0)
                                {
                                    reader.Close();
                                    return false;
                                }
                            }
                            PassElement(reader);
                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

                reader.Close();
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private int ReadCommonDatas(string strFilePath, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                bool stop = false;
                int nProjectID = 0;

                XmlTextReader reader = new XmlTextReader(strFilePath);

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "IndoorModelFile", true) == 0)
                            {
                                nProjectID = ReadIndoorModelFile(reader, true, nProjectID, connection, transaction);

                                if (nProjectID <= 0)
                                {
                                    reader.Close();
                                    return -1;
                                }
                            }
                            PassElement(reader);
                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

                reader.Close();
                return nProjectID;
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return -1;
        }

        private int ReadIndoorModelFile(XmlTextReader reader, bool commonRead, int nProjectID, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                bool stop = false;
                string strVersion = "";

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "version", true) == 0)
                    {
                        strVersion = reader.Value;
                    }
                }

                if (strVersion != TARGET_VERSION)
                {
                    m_strErrorMessage = "문서의 버전이 현재버전과 다릅니다.\r\n문서버전 : " + strVersion + ", 타겟버전 : " + TARGET_VERSION;
                    return -1;
                }

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (commonRead)
                            {
                                if (string.Compare(reader.Name, "ProjectInfo", true) == 0)
                                {
                                    if (!ReadProject(reader, connection, transaction, out nProjectID))
                                        return -1;
                                }
                                /*else if (string.Compare(reader.Name, "Levels", true) == 0)
                                {
                                    if (reader.ReadToNextSibling("Common"))
                                    {
                                        if (!ReadCommon(reader, connection, transaction, nProjectID))
                                            return -1;
                                    }
                                }*/
                                else if (string.Compare(reader.Name, "Common", true) == 0)
                                {
                                    if (!ReadCommon(reader, connection, transaction, nProjectID))
                                        return -1;
                                }
                                else
                                    PassElement(reader);
                            }
                            else
                            {
                                if (string.Compare(reader.Name, "Levels", true) == 0)
                                {
                                    if (!ReadLevels(reader, connection, transaction, nProjectID))
                                        return -1;
                                }
                                else
                                    PassElement(reader);
                            }

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return -1;
            }

            return nProjectID;
        }

        private bool ReadCommon(XmlTextReader reader, SqlConnection connection, SqlTransaction transaction, int nProjectID)
        {
            try
            {
                bool stop = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Components", true) == 0)
                            {
                                if (!ReadComponents(reader, connection, transaction, nProjectID))
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "POITypes", true) == 0)
                            {
                                if (!ReadPOITypes(reader, connection, transaction, nProjectID))
                                    return false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool ReadPOITypes(XmlTextReader reader, SqlConnection connection, SqlTransaction transaction, int nProjectID)
        {
            try
            {
                bool stop = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "POITypeGroup", true) == 0)
                            {
                                int nComponentID = ReadPOITypeGroup(reader, connection, transaction, -1);

                                if (nComponentID <= 0)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "POIType", true) == 0)
                            {
                                int nTypeID = ReadPOIType(reader, connection, transaction, -1);

                                if (nTypeID <= 0)
                                    return false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private int ReadPOITypeGroup(XmlTextReader reader, SqlConnection connection, SqlTransaction transaction, int nParentID)
        {
            try
            {
                bool stop = false;
                string strID = null, strName = null, strUserDefined = null;

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "id", true) == 0)
                    {
                        strID = reader.Value;
                    }
                    else if (string.Compare(reader.Name, "name", true) == 0)
                    {
                        strName = reader.Value;
                    }
                    else if (string.Compare(reader.Name, "userDefined", true) == 0)
                    {
                        strUserDefined = reader.Value;
                    }
                }

                if (strID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", POITypeGroup Element에 id 속성이 존재하지 않습니다.";
                    return -1;
                }

                if (strName == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", POITypeGroup Element에 name 속성이 존재하지 않습니다.";
                    return -1;
                }

                if (strUserDefined == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", POITypeGroup Element에 userDefined 속성이 존재하지 않습니다.";
                    return -1;
                }

                strUserDefined = strUserDefined.ToLower();
                bool isUserDefined = false;

                if (strUserDefined == "true" || strUserDefined == "1")
                    isUserDefined = true;
                else if (strUserDefined == "false" || strUserDefined == "0")
                    isUserDefined = false;
                else
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", POITypeGroup Element에 userDefined 속성이 잘못된 데이터를 가집니다.";
                    return -1;
                }

                int nPOITypeID = GetPOITypeID(true, nParentID, strName, isUserDefined, connection, transaction);

                if (nPOITypeID < 0)
                {
                    nPOITypeID = InsertPOIType(strName, nParentID, isUserDefined, true, null, null, connection, transaction);

                    if (nPOITypeID < 0)
                        return -1;
                }

                m_dicPOITypeID[strID] = nPOITypeID;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "POITypeGroup", true) == 0)
                            {
                                int nTypeID = ReadPOITypeGroup(reader, connection, transaction, nPOITypeID);

                                if (nTypeID <= 0)
                                    return -1;
                            }
                            else if (string.Compare(reader.Name, "POIType", true) == 0)
                            {
                                int nTypeID = ReadPOIType(reader, connection, transaction, nPOITypeID);

                                if (nTypeID <= 0)
                                    return -1;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

                return nPOITypeID;
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return -1;
        }

        private int ReadPOIType(XmlTextReader reader, SqlConnection connection, SqlTransaction transaction, int nParentID)
        {
            try
            {
                bool stop = false;
                string strID = null, strName = null, strUserDefined = null, strCode = null, strDefaultHeight = null;

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "id", true) == 0)
                    {
                        strID = reader.Value;
                    }
                    else if (string.Compare(reader.Name, "name", true) == 0)
                    {
                        strName = reader.Value;
                    }
                    else if (string.Compare(reader.Name, "userDefined", true) == 0)
                    {
                        strUserDefined = reader.Value;
                    }
                    else if (string.Compare(reader.Name, "code", true) == 0)
                    {
                        strCode = reader.Value;
                    }
                    else if (string.Compare(reader.Name, "defaultHeight", true) == 0)
                    {
                        strDefaultHeight = reader.Value;
                    }
                }

                if (strID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", POIType Element에 id 속성이 존재하지 않습니다.";
                    return -1;
                }

                if (strName == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", POIType Element에 name 속성이 존재하지 않습니다.";
                    return -1;
                }

                if (strUserDefined == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", POIType Element에 userDefined 속성이 존재하지 않습니다.";
                    return -1;
                }

                strUserDefined = strUserDefined.ToLower();
                bool isUserDefined = false;

                if (strUserDefined == "true" || strUserDefined == "1")
                    isUserDefined = true;
                else if (strUserDefined == "false" || strUserDefined == "0")
                    isUserDefined = false;
                else
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", POIType Element에 userDefined 속성이 잘못된 데이터를 가집니다.";
                    return -1;
                }

                int nPOITypeID = GetPOITypeID(false, nParentID, strName, isUserDefined, connection, transaction);

                if (nPOITypeID < 0)
                {
                    nPOITypeID = InsertPOIType(strName, nParentID, isUserDefined, false, strCode, strDefaultHeight, connection, transaction);

                    if (nPOITypeID < 0)
                        return -1;
                }

                m_dicPOITypeID[strID] = nPOITypeID;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            PassElement(reader);
                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

                return nPOITypeID;
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return -1;
        }

        private int InsertPOIType(string strName, int nParentID, bool isUserDefined, bool isGroup, string strCode, string strDefaultHeight, SqlConnection connection, SqlTransaction transaction)
        {
            string strSQL = "Select max(ID) from POIType";
            SqlDataReader sqlReader = ReadQuery(strSQL, connection, transaction);

            if (sqlReader == null)
                return -1;

            int nPOITypeID = 1;

            if (sqlReader.Read())
            {
                if (sqlReader.IsDBNull(0) == false)
                    nPOITypeID = sqlReader.GetInt32(0) + 1;
            }

            sqlReader.Close();

            strSQL = "Insert into POIType (ID, IsGroup, ParentID, Name, Code, IsUserDefined, DefaultHeight) values (";
            strSQL += string.Format("{0}, {1}, {2}, '{3}', {4}, {5}, {6})",
                nPOITypeID,
                isGroup ? 1 : 0,
                nParentID < 0 ? "NULL" : nParentID.ToString(),
                strName,
                strCode == null ? "NULL" : "'" + strCode + "'",
                isUserDefined ? 1 : 0,
                strDefaultHeight);

            if (ExecuteQuery(strSQL, connection, transaction) == false)
                return -1;

            return nPOITypeID;
        }

        private int GetPOITypeID(bool isGroup, int nParentID, string strName, bool userDefined, SqlConnection connection, SqlTransaction transaction)
        {
            string strSQL = string.Format("Select ID from POIType where IsGroup = {0} and ParentID = {1} and Name = '{2}' and IsUserDefined = {3}",
                isGroup ? 1 : 0,
                nParentID < 0 ? "NULL" : nParentID.ToString(),
                strName,
                userDefined ? 1 : 0);

            SqlDataReader sqlReader = ReadQuery(strSQL, connection, transaction);

            if (sqlReader == null)
                return -1;

            if (sqlReader.Read())
            {
                if (sqlReader.IsDBNull(0) == false)
                {
                    int nTypeID = sqlReader.GetInt32(0);
                    sqlReader.Close();
                    return nTypeID;
                }
            }

            sqlReader.Close();
            return -1;
        }

        private bool ReadComponents(XmlTextReader reader, SqlConnection connection, SqlTransaction transaction, int nProjectID)
        {
            try
            {
                bool stop = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Component", true) == 0)
                            {
                                int nComponentID = ReadComponent(reader, connection, transaction, nProjectID);

                                if (nComponentID <= 0)
                                    return false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool ReadLevels(XmlTextReader reader, SqlConnection connection, SqlTransaction transaction, int nProjectID)
        {
            try
            {
                bool stop = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Level", true) == 0)
                            {
                                if (!ReadLevel(reader, connection, transaction, nProjectID))
                                    return false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private string GetLineCountString(XmlTextReader reader)
        {
            return "Line : " + reader.LineNumber.ToString();
        }

        private bool ReadLevel(XmlTextReader reader, SqlConnection connection, SqlTransaction transaction, int nProjectID)
        {
            try
            {
                if (reader.IsEmptyElement)
                    return false;

                bool stop = false;
                /*string strID = null;

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "id", true) == 0)
                    {
                        strID = reader.Value;
                    }
                }

                if (strID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Level Element에 id 속성이 존재하지 않습니다.";
                    return false;
                }*/

                int nLevelID = -1;
                bool readGridCollection = false;
                string strName = null, strElevation = null;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Name", true) == 0)
                            {
                                if (ReadElementText(reader, ref strName) == false)
                                    return false;

                                if (strName != null && strElevation != null)
                                {
                                    nLevelID = InsertLevel(/*strID, */strName, strElevation, nProjectID, reader, connection, transaction);

                                    if (nLevelID <= 0)
                                        return false;
                                }
                            }
                            else if (string.Compare(reader.Name, "Elevation", true) == 0)
                            {
                                if (ReadElementText(reader, ref strElevation) == false)
                                    return false;

                                if (strName != null && strElevation != null)
                                {
                                    nLevelID = InsertLevel(/*strID, */strName, strElevation, nProjectID, reader, connection, transaction);

                                    if (nLevelID <= 0)
                                        return false;
                                }
                            }
                            else if (string.Compare(reader.Name, "GridCollection", true) == 0)
                            {
                                if (nLevelID <= 0)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", GridCollection 이전에 먼저 name과 Elevation이 선언되어야만 합니다.";
                                    return false;
                                }

                                if (ReadGridCollection(reader, connection, transaction, nLevelID) == false)
                                    return false;

                                readGridCollection = true;
                            }
                            else if (string.Compare(reader.Name, "ElementCollection", true) == 0)
                            {
                                if (readGridCollection == false)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", ElementCollection 이전에 먼저 GridCollection이 선언되어야만 합니다.";
                                    return false;
                                }

                                if (ReadElementCollection(reader, connection, transaction, nLevelID, nProjectID) == false)
                                    return false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool ReadElementCollection(XmlTextReader reader, SqlConnection connection, SqlTransaction transaction, int nLevelID, int nProjectID)
        {
            try
            {
                bool stop = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Wall", true) == 0)
                            {
                                if (ReadWall(reader, connection, transaction, nLevelID, nProjectID) == false)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Space", true) == 0)
                            {
                                if (ReadSpace(reader, connection, transaction, nLevelID) == false)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Door", true) == 0)
                            {
                                if (ReadDoor(reader, connection, transaction, nLevelID) == false)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Window", true) == 0)
                            {
                                if (ReadWindow(reader, connection, transaction, nLevelID) == false)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Column", true) == 0)
                            {
                                if (ReadColumn(reader, connection, transaction, nLevelID) == false)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Topology", true) == 0)
                            {
                                if (ReadTopology(reader, connection, transaction, nLevelID) == false)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Poi", true) == 0)
                            {
                                if (ReadPOI(reader, connection, transaction, nLevelID) == false)
                                    return false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool ReadPOI(XmlTextReader reader, SqlConnection connection, SqlTransaction transaction, int nLevelID)
        {
            try
            {
                if (reader.IsEmptyElement)
                    return false;

                bool stop = false;
                string strID = null, strType = null;

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "id", true) == 0)
                    {
                        strID = reader.Value;
                    }
                    else if (string.Compare(reader.Name, "type", true) == 0)
                    {
                        strType = reader.Value;
                    }
                }

                if (strID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", POI Element에 id 속성이 존재하지 않습니다.";
                    return false;
                }

                if (strType == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", POI Element에 type 속성이 존재하지 않습니다.";
                    return false;
                }

                int nPOITypeID;

                if (m_dicPOITypeID.TryGetValue(strType, out nPOITypeID) == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + strType + "은 잘못 참조된 POIType ID입니다.";
                    return false;
                }

                string strName = null, strHeight = null, strAngle = null;
                float x = 0.0f, y = 0.0f, angle = 0.0f;
                bool readPoint = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Name", true) == 0)
                            {
                                if (ReadElementText(reader, ref strName) == false)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Point", true) == 0)
                            {
                                if (ReadPoint(reader, out x, out y))
                                    readPoint = true;
                            }
                            else if (string.Compare(reader.Name, "Height", true) == 0)
                            {
                                if (ReadElementText(reader, ref strHeight) == false)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Angle", true) == 0)
                            {
                                if (ReadElementText(reader, ref strAngle) == false)
                                    return false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

                if (strName == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", POI Element에 Name이 없습니다.";
                    return false;
                }

                if (readPoint == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", POI Element에 Point 정보를 찾을수 없습니다.";
                    return false;
                }

                if (strAngle == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", POI Element에 Angle 정보를 찾을수 없습니다.";
                    return false;
                }

                if (strHeight != null)
                {
                    float fHeight;

                    if (float.TryParse(strHeight, out fHeight) == false)
                    {
                        m_strErrorMessage = GetLineCountString(reader) + strHeight + "은 잘못된 Angle값 입니다.";
                        return false;
                    }
                }

                if (float.TryParse(strAngle, out angle) == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + strAngle + "은 잘못된 Angle값 입니다.";
                    return false;
                }

                int nID = GetMaxTableID(connection, transaction, "POI");

                string strSQL = "Insert into POI (ID, TypeID, Name, x, y, Angle, Height, LevelID) values (";
                strSQL += string.Format("{0}, {1}, '{2}', {3}, {4}, {5}, {6}, {7})",
                    nID, nPOITypeID, strName, x, y, angle, 
                    strHeight == null ? "NULL" : strHeight,
                    nLevelID);

                if (ExecuteQuery(strSQL, connection, transaction) == false)
                    return false;
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool ReadTopology(XmlTextReader reader, SqlConnection connection, SqlTransaction transaction, int nLevelID)
        {
            try
            {
                if (reader.IsEmptyElement)
                    return false;

                bool stop = false;
                /*string strTopologyID = null;

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "id", true) == 0)
                    {
                        strTopologyID = reader.Value;
                    }
                }

                if (strTopologyID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Topology Element에 id가 존재하지 않습니다.";
                    return false;
                }*/

                int nID = GetMaxTableID(connection, transaction, "Topology");

                string strSQL = string.Format("Insert into Topology (ID, LevelID) values ({0}, {1})", nID, nLevelID);

                if (ExecuteQuery(strSQL, connection, transaction) == false)
                    return false;

                Dictionary<string, List<string>> dicLinkedNode = new Dictionary<string, List<string>>();
                Dictionary<string, int> dicNodeID = new Dictionary<string, int>();

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Node", true) == 0)
                            {
                                if (ReadTopologyNode(reader, connection, transaction, nID, dicLinkedNode, dicNodeID) == false)
                                    return false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

                foreach (KeyValuePair<string, List<string>> pair in dicLinkedNode)
                {
                    if (pair.Value.Count == 0)
                        continue;

                    int nNodeID, nLinkID;

                    if (dicNodeID.TryGetValue(pair.Key, out nNodeID) == false)
                        continue;

                    foreach (string strLink in pair.Value)
                    {
                        if (dicNodeID.TryGetValue(strLink, out nLinkID) == false)
                            continue;

                        strSQL = "Insert into TopologyNodeLink (NodeID, LinkedNodeID, TopologyID) values (";
                        strSQL += string.Format("{0}, {1}, {2})", nNodeID, nLinkID, nID);

                        if (ExecuteQuery(strSQL, connection, transaction) == false)
                            return false;
                    }
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool ReadTopologyNode(XmlTextReader reader, SqlConnection connection, SqlTransaction transaction, int nTopologyID, Dictionary<string, List<string>> dicLinkedNode, Dictionary<string, int> dicNodeID)
        {
            try
            {
                if (reader.IsEmptyElement)
                    return false;

                bool stop = false;
                string strNodeID = null;

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "id", true) == 0)
                    {
                        strNodeID = reader.Value;
                    }
                }

                if (strNodeID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Node Element에 id가 존재하지 않습니다.";
                    return false;
                }

                List<string> linkedIDs = null;

                if (dicLinkedNode.TryGetValue(strNodeID, out linkedIDs) == false)
                {
                    linkedIDs = new List<string>();
                    dicLinkedNode[strNodeID] = linkedIDs;
                }

                float x = 0.0f, y = 0.0f;
                bool readPoint = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Target", true) == 0)
                            {
                                if (ReadLinkedNode(reader, linkedIDs) == false)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Point", true) == 0)
                            {
                                if (ReadPoint(reader, out x, out y) == false)
                                    return false;
                                else
                                    readPoint = true;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

                if (readPoint == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Node Element에 Point 정보가 없습니다.";
                    return false;
                }

                int nID = GetMaxTableID(connection, transaction, "TopologyNode");

                string strSQL = "Insert into TopologyNode (ID, X, Y, TopologyID) values (";
                strSQL += string.Format("{0}, {1}, {2}, {3})", nID, x, y, nTopologyID);

                if (ExecuteQuery(strSQL, connection, transaction) == false)
                    return false;

                dicNodeID[strNodeID] = nID;
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private int GetMaxTableID(SqlConnection connection, SqlTransaction transaction, string strTableName)
        {
            string strSQL = "Select max(ID) from " + strTableName;
            SqlDataReader reader = ReadQuery(strSQL, connection, transaction);

            if (reader == null)
                return 0;

            int nID = 1;

            if (reader.Read())
            {
                if (reader.IsDBNull(0) == false)
                    nID = reader.GetInt32(0) + 1;
            }

            reader.Close();

            return nID;
        }

        private bool ReadLinkedNode(XmlTextReader reader, List<string> linkedNodeIDs)
        {
            try
            {
                string strNodeID = null;

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "id", true) == 0)
                    {
                        strNodeID = reader.Value;
                    }
                }

                if (strNodeID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Target Element에 id가 존재하지 않습니다.";
                    return false;
                }

                linkedNodeIDs.Add(strNodeID);
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool ReadColumn(XmlTextReader reader, SqlConnection connection, SqlTransaction transaction, int nLevelID)
        {
            try
            {
                if (reader.IsEmptyElement)
                    return false;

                bool stop = false;
                string strColumnID = null;

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "id", true) == 0)
                    {
                        strColumnID = reader.Value;
                    }
                }

                if (strColumnID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Column Element에 id가 존재하지 않습니다.";
                    return false;
                }

                Column column = null;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Rect", true) == 0)
                            {
                                column = ReadRectColumn(reader);

                                if (column == null)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Circle", true) == 0)
                            {
                                column = ReadCircleColumn(reader);

                                if (column == null)
                                    return false;
                            }
                            else
                                PassElement(reader);
                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

                if (column == null)
                    return false;

                string strSQL = "";

                if (column is RectColumn)
                {
                    RectColumn rect = (RectColumn)column;

                    strSQL = string.Format("Insert into Column (ID, ColumnType, TLx, TLy, BLx, BLy, BRx, BRy) values ((select isnull(max(id), 0) + 1 from Column), 0, {0}, {1}, {2}, {3}, {4}, {5})",
                        rect.TopLeft.X, rect.TopLeft.Y,
                        rect.BottomLeft.X, rect.BottomLeft.Y,
                        rect.BottomRight.X, rect.BottomRight.Y);
                }
                else if (column is CircleColumn)
                {
                    CircleColumn circle = (CircleColumn)column;

                    strSQL = string.Format("Insert into Column (ID, ColumnType, TLx, TLy, BLx, BLy, BRx, BRy) values ((select isnull(max(id), 0) + 1 from Column), 1, {0}, {1}, {2}, NULL, NULL, NULL)",
                        circle.Center.X, circle.Center.Y, circle.Radius);
                }
                else
                    return false;

                if (ExecuteQuery(strSQL, connection, transaction) == false)
                    return false;
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private Column ReadCircleColumn(XmlTextReader reader)
        {
            try
            {
                if (reader.IsEmptyElement)
                    return null;

                CircleColumn column = new CircleColumn();
                bool readCenter = false, readRadius = false;
                bool stop = false;
                float x, y;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Center", true) == 0)
                            {
                                if (ReadPoint(reader, out x, out y) == false)
                                    return null;
                                else
                                {
                                    column.Center = new System.Drawing.PointF(x, y);
                                    readCenter = true;
                                }
                            }
                            else if (string.Compare(reader.Name, "Radius", true) == 0)
                            {
                                string strRadius = "";
                                double dRadius;

                                if (ReadElementText(reader, ref strRadius) == false)
                                    return null;

                                if (double.TryParse(strRadius, out dRadius))
                                {
                                    readRadius = true;
                                    column.Radius = dRadius;
                                }
                                else
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Radius에 잘못된 값이 들어 있습니다.";
                                    return null;
                                }
                            }
                            else
                                PassElement(reader);
                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

                if (readCenter == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Column/Circle Element에 Center 정보가 없습니다.";
                    return null;
                }

                if (readRadius == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Column Element에 Radius가 없습니다.";
                    return null;
                }

                return column;
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return null;
        }

        private Column ReadRectColumn(XmlTextReader reader)
        {
            try
            {
                if (reader.IsEmptyElement)
                    return null;

                RectColumn column = new RectColumn();
                bool readTL = false, readBL = false, readBR = false;
                bool stop = false;
                float x, y;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "TL", true) == 0)
                            {
                                if (ReadPoint(reader, out x, out y) == false)
                                    return null;
                                else
                                {
                                    column.TopLeft = new System.Drawing.PointF(x, y);
                                    readTL = true;
                                }
                            }
                            else if (string.Compare(reader.Name, "BL", true) == 0)
                            {
                                if (ReadPoint(reader, out x, out y) == false)
                                    return null;
                                else
                                {
                                    column.BottomLeft = new System.Drawing.PointF(x, y);
                                    readBL = true;
                                }
                            }
                            else if (string.Compare(reader.Name, "BR", true) == 0)
                            {
                                if (ReadPoint(reader, out x, out y) == false)
                                    return null;
                                else
                                {
                                    column.BottomRight = new System.Drawing.PointF(x, y);
                                    readBR = true;
                                }
                            }
                            else
                                PassElement(reader);
                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

                if (readTL == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Column/Rect Element에 TL 정보가 없습니다.";
                    return null;
                }

                if (readBL == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Column/Rect Element에 BL 정보가 없습니다.";
                    return null;
                }

                if (readBR == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Column/Rect Element에 BR 정보가 없습니다.";
                    return null;
                }

                return column;
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return null;
        }

        private bool ReadWindow(XmlTextReader reader, SqlConnection connection, SqlTransaction transaction, int nLevelID)
        {
            try
            {
                if (reader.IsEmptyElement)
                    return false;

                bool stop = false;
                string strWindowID = null, strWallID = null;

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "id", true) == 0)
                    {
                        strWindowID = reader.Value;
                    }
                    else if (string.Compare(reader.Name, "attachedWall", true) == 0)
                    {
                        strWallID = reader.Value;
                    }
                }

                /*if (strWindowID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Window Element에 id가 존재하지 않습니다.";
                    return false;
                }*/

                if (strWallID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Window Element에 attachedWall 존재하지 않습니다.";
                    return false;
                }

                float x = 0.0f, y = 0.0f;
                float fWidth = 0.0f, fHeight = 0.0f, fElevation = 0.0f;
                bool readPoint = false;
                bool readWidth = false, readHeight = false, readElevation = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Point", true) == 0)
                            {
                                if (ReadPoint(reader, out x, out y) == false)
                                    return false;
                                else
                                    readPoint = true;
                            }
                            else if (string.Compare(reader.Name, "Width", true) == 0)
                            {
                                string strWidth = "";

                                if (ReadElementText(reader, ref strWidth) == false)
                                    return false;

                                if (float.TryParse(strWidth, out fWidth))
                                    readWidth = true;
                                else
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Width에 잘못된 값이 들어 있습니다.";
                                    return false;
                                }
                            }
                            else if (string.Compare(reader.Name, "Height", true) == 0)
                            {
                                string strHeight = "";

                                if (ReadElementText(reader, ref strHeight) == false)
                                    return false;

                                if (float.TryParse(strHeight, out fHeight))
                                    readHeight = true;
                                else
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Height에 잘못된 값이 들어 있습니다.";
                                    return false;
                                }
                            }
                            else if (string.Compare(reader.Name, "Elevation", true) == 0)
                            {
                                string strElevation = "";

                                if (ReadElementText(reader, ref strElevation) == false)
                                    return false;

                                if (float.TryParse(strElevation, out fElevation))
                                    readElevation = true;
                                else
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Elevation에 잘못된 값이 들어 있습니다.";
                                    return false;
                                }
                            }
                            else
                                PassElement(reader);
                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

                if (readPoint == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Window Element에 Point 정보가 없습니다.";
                    return false;
                }

                if (readWidth == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Window Element에 Width 정보가 없습니다.";
                    return false;
                }

                if (readHeight == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Window Element에 Height 정보가 없습니다.";
                    return false;
                }

                if (readElevation == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Window Element에 Elevation 정보가 없습니다.";
                    return false;
                }

                int nWallID;
                int nID = GetMaxTableID(connection, transaction, "Window");

                if (m_dicWallID.TryGetValue(strWallID, out nWallID) == false)
                    return false;

                string strSQL = "Insert into Window (ID, WallID, X, Y, Width, Height, Elevation, LevelID) values (";
                strSQL += string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7})", nID, nWallID, x, y, fWidth, fHeight, fElevation, nLevelID);

                if (ExecuteQuery(strSQL, connection, transaction) == false)
                    return false;
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool ReadDoor(XmlTextReader reader, SqlConnection connection, SqlTransaction transaction, int nLevelID)
        {
            try
            {
                if (reader.IsEmptyElement)
                    return false;

                bool stop = false;
                string strDoorID = null, strWallID = null;

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "id", true) == 0)
                    {
                        strDoorID = reader.Value;
                    }
                    else if (string.Compare(reader.Name, "attachedWall", true) == 0)
                    {
                        strWallID = reader.Value;
                    }
                }

                if (strDoorID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Door Element에 id가 존재하지 않습니다.";
                    return false;
                }

                if (strWallID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Door Element에 attachedWall 존재하지 않습니다.";
                    return false;
                }

                float x = 0.0f, y = 0.0f;
                float hinge1X = 0.0f, hinge1Y = 0.0f, hinge2X = 0.0f, hinge2Y = 0.0f;
                //double direction = 0.0;
                float fWidth = 0.0f, fHeight = 0.0f, fElevation = 0.0f;
                int nDoorType = 0;
                bool readPoint = false/*, readDirection = false*/, readHinge1 = false, readHinge2 = false;
                bool readWidth = false, readHeight = false, readElevation = false;
                bool readDoorType = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Point", true) == 0)
                            {
                                if (ReadPoint(reader, out x, out y) == false)
                                    return false;
                                else
                                    readPoint = true;
                            }
                            /*else if (string.Compare(reader.Name, "Direction", true) == 0)
                            {
                                string strDirection = "";

                                if (ReadElementText(reader, ref strDirection) == false)
                                    return false;

                                if (double.TryParse(strDirection, out direction))
                                    readDirection = true;
                                else
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Direction에 잘못된 값이 들어 있습니다.";
                                    return false;
                                }
                            }*/
                            else if (string.Compare(reader.Name, "Width", true) == 0)
                            {
                                string strWidth = "";

                                if (ReadElementText(reader, ref strWidth) == false)
                                    return false;

                                if (float.TryParse(strWidth, out fWidth))
                                    readWidth = true;
                                else
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Width에 잘못된 값이 들어 있습니다.";
                                    return false;
                                }
                            }
                            else if (string.Compare(reader.Name, "Height", true) == 0)
                            {
                                string strHeight = "";

                                if (ReadElementText(reader, ref strHeight) == false)
                                    return false;

                                if (float.TryParse(strHeight, out fHeight))
                                    readHeight = true;
                                else
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Height에 잘못된 값이 들어 있습니다.";
                                    return false;
                                }
                            }
                            else if (string.Compare(reader.Name, "Elevation", true) == 0)
                            {
                                string strElevation = "";

                                if (ReadElementText(reader, ref strElevation) == false)
                                    return false;

                                if (float.TryParse(strElevation, out fElevation))
                                    readElevation = true;
                                else
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", Elevation에 잘못된 값이 들어 있습니다.";
                                    return false;
                                }
                            }
                            else if (string.Compare(reader.Name, "DoorType", true) == 0)
                            {
                                string strDoorType = "";

                                if (ReadElementText(reader, ref strDoorType) == false)
                                    return false;

                                if (int.TryParse(strDoorType, out nDoorType))
                                    readDoorType = true;
                                else
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", DoorType에 잘못된 값이 들어 있습니다.";
                                    return false;
                                }
                            }
                            else if (string.Compare(reader.Name, "Hinge1", true) == 0)
                            {
                                if (ReadPoint(reader, out hinge1X, out hinge1Y) == false)
                                    return false;
                                else
                                    readHinge1 = true;
                            }
                            else if (string.Compare(reader.Name, "Hinge2", true) == 0)
                            {
                                if (ReadPoint(reader, out hinge2X, out hinge2Y) == false)
                                    return false;
                                else
                                    readHinge2 = true;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

                if (readPoint == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Door Element에 Point 정보가 없습니다.";
                    return false;
                }

                /*if (readDirection == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Door Element에 Direction 정보가 없습니다.";
                    return false;
                }*/

                if (readWidth == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Door Element에 Width 정보가 없습니다.";
                    return false;
                }

                if (readHeight == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Door Element에 Height 정보가 없습니다.";
                    return false;
                }

                if (readElevation == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Door Element에 Elevation 정보가 없습니다.";
                    return false;
                }

                if (readDoorType == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Door Element에 DoorType 정보가 없습니다.";
                    return false;
                }

                int nWallID;
                int nID = GetMaxTableID(connection, transaction, "Door");

                if (m_dicWallID.TryGetValue(strWallID, out nWallID) == false)
                    return false;

                string strHinge1 = "NULL, NULL", strHinge2 = "NULL, NULL";

                if (readHinge1)
                    strHinge1 = string.Format("{0}, {1}", hinge1X, hinge1Y);

                if (readHinge2)
                    strHinge2 = string.Format("{0}, {1}", hinge2X, hinge2Y);

                string strSQL = "Insert into Door (ID, WallID, X, Y, Width, Height, Elevation, DoorType, LevelID, Hinge1X, Hinge1Y, Hinge2X, Hinge2Y) values (";
                strSQL += string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10})",
                    nID, nWallID, x, y, fWidth, fHeight, fElevation, nDoorType, nLevelID, strHinge1, strHinge2);

                if (ExecuteQuery(strSQL, connection, transaction) == false)
                    return false;
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool ReadSpace(XmlTextReader reader, SqlConnection connection, SqlTransaction transaction, int nLevelID)
        {
            try
            {
                if (reader.IsEmptyElement)
                    return false;

                bool stop = false;
                string strSpaceID = null, strSpaceName = null;

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "id", true) == 0)
                    {
                        strSpaceID = reader.Value;
                    }
                    else if (string.Compare(reader.Name, "name", true) == 0)
                    {
                        strSpaceName = reader.Value;
                    }
                }

                /*if (strSpaceID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Space Element에 id가 존재하지 않습니다.";
                    return false;
                }*/

                if (strSpaceName == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Space Element에 name이 존재하지 않습니다.";
                    return false;
                }

                int nID = GetMaxTableID(connection, transaction, "Space");

                string strSQL = "Insert into Space (ID, Name, LevelID) values (";
                strSQL += string.Format("{0}, '{1}', {2})", nID, strSpaceName, nLevelID);

                if (ExecuteQuery(strSQL, connection, transaction) == false)
                    return false;

                int nWallIndex = 1;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "LinkedWall", true) == 0)
                            {
                                if (ReadLinkedWall(reader, connection, transaction, nID, nLevelID, ref nWallIndex) == false)
                                    return false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool ReadLinkedWall(XmlTextReader reader, SqlConnection connection, SqlTransaction transaction, int nSpaceID, int nLevelID, ref int nWallIndex)
        {
            try
            {
                string strWallID = null;

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "link", true) == 0)
                    {
                        strWallID = reader.Value;
                    }
                }

                if (strWallID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", LinkedWall Element에 link가 존재하지 않습니다.";
                    return false;
                }

                int nWallID;

                if (m_dicWallID.TryGetValue(strWallID, out nWallID) == false)
                    return false;

                string strSQL = "Insert into SpaceWallLink (SpaceID, WallID, LevelID, WallIndex) values (";
                strSQL += string.Format("{0}, {1}, {2}, {3})", nSpaceID, nWallID, nLevelID, nWallIndex);

                if (ExecuteQuery(strSQL, connection, transaction) == false)
                    return false;

                nWallIndex++;
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool ReadWall(XmlTextReader reader, SqlConnection connection, SqlTransaction transaction, int nLevelID, int nProjectID)
        {
            try
            {
                if (reader.IsEmptyElement)
                    return false;

                bool stop = false;
                string strWallID = null, strGridID = null, strComponentID = null;

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "id", true) == 0)
                    {
                        strWallID = reader.Value;
                    }
                    else if (string.Compare(reader.Name, "grid", true) == 0)
                    {
                        strGridID = reader.Value;
                    }
                    else if (string.Compare(reader.Name, "component", true) == 0)
                    {
                        strComponentID = reader.Value;
                    }
                }

                if (strWallID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Wall Element에 id가 존재하지 않습니다.";
                    return false;
                }

                if (strGridID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Wall Element에 grid가 존재하지 않습니다.";
                    return false;
                }

                if (strComponentID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Wall Element에 component가 존재하지 않습니다.";
                    return false;
                }

                string strThick = null, strHeight = null;
                int nComponentID = -1;

                if (m_dicComponentID.TryGetValue(strComponentID, out nComponentID) == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + strComponentID + "는 정의되지 않은 component ID 입니다.";
                    return false;
                }

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Thickness", true) == 0)
                            {
                                if (ReadElementText(reader, ref strThick) == false)
                                    return false;
                            }
                            else if (string.Compare(reader.Name, "Height", true) == 0)
                            {
                                if (ReadElementText(reader, ref strHeight) == false)
                                    return false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

                if (strThick == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Wall Element에 Thickness가 존재하지 않습니다.";
                    return false;
                }

                if (strHeight == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Wall Element에 Height가 존재하지 않습니다.";
                    return false;
                }

                if (nComponentID <= 0)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Wall Element에서 Component 정보를 읽어올 수 없습니다.";
                    return false;
                }

                float fThick = 0.0f, fHeight = 0.0f;

                if (float.TryParse(strThick.Trim(), out fThick) == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Thickness는 숫자이어야 합니다.";
                    return false;
                }

                if (float.TryParse(strHeight.Trim(), out fHeight) == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Height는 숫자이어야 합니다.";
                    return false;
                }

                int nGridID = 0;
                int nID = GetMaxTableID(connection, transaction, "Wall");

                if (m_dicGridID.TryGetValue(strGridID, out nGridID) == false)
                    return false;

                string strSQL = "Insert into Wall (ID, Thick, Height, ComponentID, GridID, LevelID) values (";
                strSQL += string.Format("{0}, {1}, {2}, {3}, {4}, {5})", nID, fThick, fHeight, nComponentID, nGridID, nLevelID);

                if (ExecuteQuery(strSQL, connection, transaction) == false)
                    return false;

                m_dicWallID[strWallID] = nID;
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private int ReadComponent(XmlTextReader reader, SqlConnection connection, SqlTransaction transaction, int nProjectID)
        {
            try
            {
                string strTypeName = null, strComponentID = null;

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "id", true) == 0)
                    {
                        strComponentID = reader.Value;
                    }
                    else if (string.Compare(reader.Name, "type", true) == 0)
                    {
                        strTypeName = reader.Value;
                    }
                }

                if (strComponentID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Component Element에 id가 존재하지 않습니다.";
                    return 0;
                }

                if (strTypeName == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Component Element에 type이 존재하지 않습니다.";
                    return 0;
                }

                string strComponentName = "";
                ReadElementText(reader, ref strComponentName);

                int nComponentID = GetComponentID(nProjectID, strTypeName, strComponentName, connection, transaction);

                if (nComponentID < 0)
                {
                    string strSQL = "Select max(ID) from Component";
                    SqlDataReader sqlReader = ReadQuery(strSQL, connection, transaction);

                    if (sqlReader == null)
                        return 0;

                    nComponentID = 1;

                    if (sqlReader.Read())
                    {
                        if (sqlReader.IsDBNull(0) == false)
                            nComponentID = sqlReader.GetInt32(0) + 1;
                    }

                    sqlReader.Close();

                    strSQL = "Insert into Component (ID, TypeName, ComponentName, ProjectID) values (";
                    strSQL += string.Format("{0}, '{1}', '{2}', {3})", nComponentID, strTypeName, strComponentName, nProjectID);

                    if (ExecuteQuery(strSQL, connection, transaction) == false)
                        return 0;
                }

                m_dicComponentID[strComponentID] = nComponentID;
                return nComponentID;
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return 0;
        }

        private int GetComponentID(int nProjectID, string strTypeName, string strComponentName, SqlConnection connection, SqlTransaction transaction)
        {
            string strSQL = string.Format("Select ID from Component where TypeName = '{0}' and ComponentName = '{1}' and ProjectID = {2}",
                strTypeName, strComponentName, nProjectID);

            SqlDataReader sqlReader = ReadQuery(strSQL, connection, transaction);

            if (sqlReader == null)
                return -1;

            if (sqlReader.Read())
            {
                if (sqlReader.IsDBNull(0) == false)
                {
                    int nComponentID = sqlReader.GetInt32(0);
                    sqlReader.Close();
                    return nComponentID;
                }
            }

            sqlReader.Close();
            return -1;
        }

        private bool ReadGridCollection(XmlTextReader reader, SqlConnection connection, SqlTransaction transaction, int nLevelID)
        {
            try
            {
                bool stop = false;
                
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Grid", true) == 0)
                            {
                                if (ReadGrid(reader, connection, transaction, nLevelID) == false)
                                    return false;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool ReadGrid(XmlTextReader reader, SqlConnection connection, SqlTransaction transaction, int nLevelID)
        {
            try
            {
                if (reader.IsEmptyElement)
                    return false;

                bool stop = false;
                string strID = null;

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "id", true) == 0)
                    {
                        strID = reader.Value;
                    }
                }

                if (strID == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Grid Element에 id 속성이 존재하지 않습니다.";
                    return false;
                }

                float fBeginX, fBeginY, fEndX, fEndY;
                VariousData<float> thirdX = null, thirdY = null, beginAngle = null, angle = null;
                VariousData<bool> isClockwise = null;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Line", true) == 0)
                            {
                                if (ReadLine(reader, out fBeginX, out fBeginY, out fEndX, out fEndY) == false)
                                    return false;
                                else
                                {
                                    if (InsertGrid(nLevelID, strID, fBeginX, fBeginY, fEndX, fEndY, null, null, null, null, null, connection, transaction) == false)
                                        return false;
                                }
                            }
                            else if (string.Compare(reader.Name, "Arc", true) == 0)
                            {
                                if (ReadArc(reader, out fBeginX, out fBeginY, out fEndX, out fEndY, out thirdX, out thirdY, out beginAngle, out angle, out isClockwise) == false)
                                    return false;
                                else
                                {
                                    if (InsertGrid(nLevelID, strID, fBeginX, fBeginY, fEndX, fEndY, thirdX, thirdY, beginAngle, angle, isClockwise, connection, transaction) == false)
                                        return false;
                                }
                            }
                            else if (string.Compare(reader.Name, "EArc", true) == 0)
                            {
                                if (ReadArc(reader, out fBeginX, out fBeginY, out fEndX, out fEndY, out thirdX, out thirdY, out beginAngle, out angle, out isClockwise) == false)
                                    return false;
                                else
                                {
                                    if (InsertGrid(nLevelID, strID, fBeginX, fBeginY, fEndX, fEndY, thirdX, thirdY, beginAngle, angle, isClockwise, connection, transaction) == false)
                                        return false;
                                }
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool ReadPoint(XmlTextReader reader, out float x, out float y)
        {
            x = y = 0.0f;

            try
            {
                bool stop = false;
                bool read = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Pos", true) == 0)
                            {
                                string strPos = "";

                                if (ReadElementText(reader, ref strPos) == false)
                                    return false;

                                if (ReadPos(strPos, out x, out y) == false)
                                {
                                    m_strErrorMessage = GetLineCountString(reader) + ", 좌표 정보에 잘못된 값이 들어있습니다.";
                                    return false;
                                }
                                else
                                    read = true;
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

                if (read == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Point 정보가 충분하지 않습니다.";
                    return false;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool ReadArc(XmlTextReader reader, out float beginX, out float beginY, out float endX, out float endY, out VariousData<float> thirdX, out VariousData<float> thirdY, out VariousData<float> beginAngle, out VariousData<float> angle, out VariousData<bool> clockwise)
        {
            beginX = beginY = endX = endY = 0.0f;
            thirdX = thirdY = beginAngle = angle = null;
            clockwise = null;

            bool readCenter = false;

            try
            {
                bool stop = false;
                
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Center", true) == 0)
                            {
                                if (ReadPoint(reader, out beginX, out beginY) == false)
                                    return false;

                                readCenter = true;
                            }
                            else if (string.Compare(reader.Name, "Radius", true) == 0)
                            {
                                string strRadius = "";
                                float fRadius;

                                if (ReadElementText(reader, ref strRadius) == false)
                                    return false;

                                if (float.TryParse(strRadius, out fRadius) && fRadius > 0.0f)
                                    thirdX = new VariousData<float>(fRadius);
                            }
                            else if (string.Compare(reader.Name, "BeginAngle", true) == 0)
                            {
                                string strAngle = "";
                                float fAngle;

                                if (ReadElementText(reader, ref strAngle) == false)
                                    return false;

                                if (float.TryParse(strAngle, out fAngle))
                                    beginAngle = new VariousData<float>(fAngle);
                            }
                            else if (string.Compare(reader.Name, "Angle", true) == 0)
                            {
                                string strAngle = "";
                                float fAngle;

                                if (ReadElementText(reader, ref strAngle) == false)
                                    return false;

                                if (float.TryParse(strAngle, out fAngle))
                                    angle = new VariousData<float>(fAngle);
                            }
                            else if (string.Compare(reader.Name, "ClockWise", true) == 0)
                            {
                                string strClockwise = "";

                                if (ReadElementText(reader, ref strClockwise) == false)
                                    return false;

                                strClockwise = strClockwise.ToLower();

                                if (strClockwise == "true" || strClockwise == "1")
                                    clockwise = new VariousData<bool>(true);
                                else if (strClockwise == "false" || strClockwise == "0")
                                    clockwise = new VariousData<bool>(false);
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

                if (readCenter == false || thirdX == null || beginAngle == null || angle == null || clockwise == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Arc 정보가 충분하지 않습니다.";
                    return false;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool ReadEArc(XmlTextReader reader, out float beginX, out float beginY, out float endX, out float endY, out VariousData<float> thirdX, out VariousData<float> thirdY, out VariousData<float> beginAngle, out VariousData<float> angle, out VariousData<bool> clockwise)
        {
            beginX = beginY = endX = endY = 0.0f;
            thirdX = thirdY = beginAngle = angle = null;
            clockwise = null;

            bool readTL = false, readBL = false;

            try
            {
                bool stop = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "TL", true) == 0)
                            {
                                if (ReadPoint(reader, out beginX, out beginY) == false)
                                    return false;

                                readTL = true;
                            }
                            else if (string.Compare(reader.Name, "BL", true) == 0)
                            {
                                if (ReadPoint(reader, out endX, out endY) == false)
                                    return false;

                                readBL = true;
                            }
                            else if (string.Compare(reader.Name, "BR", true) == 0)
                            {
                                float x, y;

                                if (ReadPoint(reader, out x, out y) == false)
                                    return false;

                                thirdX = new VariousData<float>(x);
                                thirdY = new VariousData<float>(y);
                            }
                            else if (string.Compare(reader.Name, "BeginAngle", true) == 0)
                            {
                                string strAngle = "";
                                float fAngle;

                                if (ReadElementText(reader, ref strAngle) == false)
                                    return false;

                                if (float.TryParse(strAngle, out fAngle))
                                    beginAngle = new VariousData<float>(fAngle);
                            }
                            else if (string.Compare(reader.Name, "Angle", true) == 0)
                            {
                                string strAngle = "";
                                float fAngle;

                                if (ReadElementText(reader, ref strAngle) == false)
                                    return false;

                                if (float.TryParse(strAngle, out fAngle))
                                    angle = new VariousData<float>(fAngle);
                            }
                            else if (string.Compare(reader.Name, "ClockWise", true) == 0)
                            {
                                string strClockwise = "";

                                if (ReadElementText(reader, ref strClockwise) == false)
                                    return false;

                                strClockwise = strClockwise.ToLower();

                                if (strClockwise == "true" || strClockwise == "1")
                                    clockwise = new VariousData<bool>(true);
                                else if (strClockwise == "false" || strClockwise == "0")
                                    clockwise = new VariousData<bool>(false);
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

                if (readTL == false || readBL == false || thirdX == null || thirdY == null || beginAngle == null || angle == null || clockwise == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", EArc 정보가 충분하지 않습니다.";
                    return false;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool ReadLine(XmlTextReader reader, out float beginX, out float beginY, out float endX, out float endY)
        {
            beginX = beginY = endX = endY = 0.0f;

            try
            {
                bool stop = false;
                bool readBegin = false, readEnd = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Pos", true) == 0)
                            {
                                string strPos = "";

                                if (ReadElementText(reader, ref strPos) == false)
                                    return false;

                                if (readBegin)
                                {
                                    if (ReadPos(strPos, out endX, out endY) == false)
                                    {
                                        m_strErrorMessage = GetLineCountString(reader) + ", 좌표 정보에 잘못된 값이 들어있습니다.";
                                        return false;
                                    }
                                    else
                                        readEnd = true;
                                }
                                else
                                {
                                    if (ReadPos(strPos, out beginX, out beginY) == false)
                                    {
                                        m_strErrorMessage = GetLineCountString(reader) + ", 좌표 정보에 잘못된 값이 들어있습니다.";
                                        return false;
                                    }
                                    else
                                        readBegin = true;
                                }
                            }
                            else
                                PassElement(reader);

                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

                if (readBegin == false || readEnd == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Line 정보가 충분하지 않습니다.";
                    return false;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool ReadPos(string strVertex, out float x, out float y)
        {
            x = y = 0.0f;

            string[] tokens = strVertex.Split(',');

            if (tokens.Count() != 2)
                return false;

            if (float.TryParse(tokens[0].Trim(), out x) == false)
                return false;

            if (float.TryParse(tokens[1].Trim(), out y) == false)
                return false;

            return true;
        }

        private bool InsertGrid(int nLevelID, string strGridID, float fBeginX, float fBeginY, float fEndX, float fEndY, VariousData<float> thirdX, VariousData<float> thirdY, VariousData<float> beginAngle, VariousData<float> angle, VariousData<bool> isClockwise, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                string strClockwise = "NULL";

                if (isClockwise != null)
                {
                    if (isClockwise.Data)
                        strClockwise = "1";
                    else
                        strClockwise = "0";
                }

                int nGridType = 0;

                if (thirdX != null)
                {
                    if (thirdY == null)
                        nGridType = 1;
                    else
                        nGridType = 2;
                }

                int nID = GetMaxTableID(connection, transaction, "Grid");

                string strSQL = "Insert into Grid (ID, GridType, BeginX, BeginY, EndX, EndY, ThirdX, ThirdY, BeginAngle, Angle, ClockWise, LevelID) values (";
                strSQL += string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {7}, {8}, {9}, {10}, {11}, {6})",
                    nID, nGridType, fBeginX, fBeginY, fEndX, fEndY, nLevelID,
                    thirdX == null ? "NULL" : thirdX.Data.ToString(),
                    thirdY == null ? "NULL" : thirdY.Data.ToString(),
                    beginAngle == null ? "NULL" : beginAngle.Data.ToString(),
                    angle == null ? "NULL" : angle.Data.ToString(),
                    strClockwise);

                if (ExecuteQuery(strSQL, connection, transaction) == false)
                    return false;

                m_dicGridID[strGridID] = nID;
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private int InsertLevel(/*string strLevelID, */string strName, string strElevation, int nProjectID, XmlTextReader reader, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                float fElevation = 0.0f;

                if (float.TryParse(strElevation, out fElevation) == false)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", Level Element의 Elevation 속성이 잘못되었습니다.";
                    return -1;
                }

                int nID = GetMaxTableID(connection, transaction, "Level");

                string strSQL = "Insert into Level (ID, Name, Elevation, ProjectID) values (";
                strSQL += string.Format("{0}, '{1}', {2}, {3})", nID, strName, fElevation, nProjectID);

                if (ExecuteQuery(strSQL, connection, transaction) == false)
                    return -1;

                return nID;
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return -1;
        }

        private bool ReadProject(XmlTextReader reader, SqlConnection connection, SqlTransaction transaction, out int nProjectID)
        {
            nProjectID = 0;

            try
            {
                if (reader.IsEmptyElement)
                    return false;

                bool stop = false;
                string strName = null, strUnit = null, strDate = null, strAuthor = null;

                while (reader.MoveToNextAttribute())
                {
                    if (string.Compare(reader.Name, "name", true) == 0)
                    {
                        strName = reader.Value;
                    }
                    else if (string.Compare(reader.Name, "unit", true) == 0)
                    {
                        strUnit = reader.Value;
                    }
                    else if (string.Compare(reader.Name, "datetime", true) == 0)
                    {
                        strDate = reader.Value;
                    }
                    else if (string.Compare(reader.Name, "author", true) == 0)
                    {
                        strAuthor = reader.Value;
                    }
                }

                if (strName == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", ProjectInfo Element에 name 속성이 존재하지 않습니다.";
                    return false;
                }

                if (strUnit == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", ProjectInfo Element에 unit 속성이 존재하지 않습니다.";
                    return false;
                }

                if (strDate == null)
                {
                    m_strErrorMessage = GetLineCountString(reader) + ", ProjectInfo Element에 datetime 속성이 존재하지 않습니다.";
                    return false;
                }

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            PassElement(reader);
                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

                string strSQL = "Select ID from Project where Name = '" + strName + "'";
                SqlDataReader sqlReader = ReadQuery(strSQL, connection, transaction);

                if (sqlReader == null)
                    return false;

                if (sqlReader.Read() == false)
                {
                    sqlReader.Close();
                }
                else
                {
                    int nID = sqlReader.GetInt32(0);
                    sqlReader.Close();

                    // 같은 이름의 프로젝트가 이미 존재하면 먼저 저장되어 있는 프로젝트를 삭제한다.
                    DeleteProject(nID, connection, transaction);
                }

                nProjectID = InsertProject(strName, strUnit, strDate, strAuthor, connection, transaction);

                if (nProjectID == 0)
                {
                    if (m_strErrorMessage.Length > 0)
                        m_strErrorMessage = GetLineCountString(reader) + ", " + m_strErrorMessage;
                    else
                        m_strErrorMessage = GetLineCountString(reader) + ", Error";

                    return false;
                }
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                reader.Close();
                return false;
            }

            return true;
        }

        private int InsertProject(string strName, string strUnit, string strDate, string strAuthor, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                string strSQL = "Select max(ID) from Project";
                SqlDataReader reader = ReadQuery(strSQL, connection, transaction);

                if (reader == null)
                    return 0;

                int nID = 1;

                if (reader.Read())
                {
                    if (reader.IsDBNull(0) == false)
                        nID = reader.GetInt32(0) + 1;
                }

                reader.Close();

                UnitType unit = ToUnitType(strUnit);

                if (unit == UnitType.Unknown)
                {
                    m_strErrorMessage = "ProjectInfo Element의 unit 속성에 잘못된 값이 들어있습니다.";
                    return 0;
                }

                string strTime = strDate;
                /*string[] tokens = strDate.Split(':');

                if (tokens.Count() != 5)
                {
                    m_strErrorMessage = "ProjectInfo Element의 date 속성에 잘못된 값이 들어있습니다.";
                    return 0;
                }

                string strYear = tokens[0].Trim();
                string strMonth = tokens[1].Trim();
                string strDay = tokens[2].Trim();
                string strHour = tokens[3].Trim();
                string strMinute = tokens[4].Trim();
                string strTime = string.Format("{0}-{1}-{2} {3}:{4}:00", strYear, strMonth, strDay, strHour, strMinute);*/

                if (strAuthor != null)
                    strAuthor = "'" + strAuthor + "'";
                else
                    strAuthor = "NULL";

                strSQL = "Insert into Project (ID, Name, UnitOfLength, TimeStamp, Author) values (";
                strSQL += string.Format("{0}, '{1}', {2}, '{3}', {4})", nID, strName, (int)unit, strTime, strAuthor);

                if (ExecuteQuery(strSQL, connection, transaction) == false)
                    return 0;

                return nID;
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return 0;
        }

        private UnitType ToUnitType(string strUnit)
        {
            if (strUnit == "mm")
                return UnitType.MM;
            else if (strUnit == "cm")
                return UnitType.CM;
            else if (strUnit == "meter")
                return UnitType.M;
            else if (strUnit == "km")
                return UnitType.KM;

            return UnitType.Unknown;
        }

        private bool DeleteProject(int nProjectID, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                SqlDataReader reader = ReadQuery("Select ID from Level where ProjectID = " + nProjectID.ToString(), connection, transaction);
                List<int> levelIDs = new List<int>();

                while (reader.Read())
                {
                    levelIDs.Add(reader.GetInt32(0));
                }

                reader.Close();

                foreach (int nLevelID in levelIDs)
                {
                    if (DeleteTopology(nLevelID, connection, transaction) == false)
                        return false;

                    if (DeleteSpace(nLevelID, connection, transaction) == false)
                        return false;

                    if (DeleteWall(nLevelID, connection, transaction) == false)
                        return false;

                    string strSQL = string.Format("Delete from POI where LevelID = '{0}'", nLevelID);

                    if (ExecuteQuery(strSQL, connection, transaction) == false)
                        return false;

                    if (ExecuteQuery("Delete from Grid where LevelID = " + nLevelID.ToString(), connection, transaction) == false)
                        return false;
                }

                if (ExecuteQuery("Delete from Component where ProjectID = " + nProjectID.ToString(), connection, transaction) == false)
                    return false;

                if (ExecuteQuery("Delete from Level where ProjectID = " + nProjectID.ToString(), connection, transaction) == false)
                    return false;

                if (ExecuteQuery("Delete from Project where ID = " + nProjectID.ToString(), connection, transaction) == false)
                    return false;
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool DeleteWall(int nLevelID, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                string strSQL = string.Format("Delete from Door where LevelID = {0}", nLevelID);

                if (ExecuteQuery(strSQL, connection, transaction) == false)
                    return false;

                strSQL = string.Format("Delete from Window where LevelID = {0}", nLevelID);

                if (ExecuteQuery(strSQL, connection, transaction) == false)
                    return false;

                strSQL = string.Format("Delete from Wall where LevelID = {0}", nLevelID);

                if (ExecuteQuery(strSQL, connection, transaction) == false)
                    return false;

                strSQL = string.Format("Delete from Grid where LevelID = {0}", nLevelID);

                if (ExecuteQuery(strSQL, connection, transaction) == false)
                    return false;
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool DeleteSpace(int nLevelID, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                string strSQL = string.Format("Delete from SpaceWallLink where LevelID = {0}", nLevelID.ToString());

                if (ExecuteQuery(strSQL, connection, transaction) == false)
                    return false;

                strSQL = string.Format("Delete from Space where LevelID = {0}", nLevelID.ToString());

                if (ExecuteQuery(strSQL, connection, transaction) == false)
                    return false;
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool DeleteTopology(int nLevelID, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                SqlDataReader reader = ReadQuery("Select ID from Topology where LevelID = " + nLevelID, connection, transaction);
                List<int> topologyIDs = new List<int>();

                while (reader.Read())
                {
                    topologyIDs.Add(reader.GetInt32(0));
                }

                reader.Close();

                foreach (int nTopologyID in topologyIDs)
                {
                    if (DeleteTopologyNode(nTopologyID, connection, transaction) == false)
                        return false;
                }

                string strSQL = string.Format("Delete from Topology where LevelID = '{0}'", nLevelID);

                if (ExecuteQuery(strSQL, connection, transaction) == false)
                    return false;
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private bool DeleteTopologyNode(int nTopologyID, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                string strSQL = string.Format("Delete from TopologyNodeLink where TopologyID = {0}", nTopologyID);

                if (ExecuteQuery(strSQL, connection, transaction) == false)
                    return false;

                strSQL = string.Format("Delete from TopologyNode where TopologyID = {0}", nTopologyID);

                if (ExecuteQuery(strSQL, connection, transaction) == false)
                    return false;
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private SqlDataReader ReadQuery(string strSQL, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(strSQL, connection, transaction);
                return cmd.ExecuteReader();
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
            }

            return null;
        }

        private bool ExecuteQuery(string strSQL, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(strSQL, connection, transaction);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                m_strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        private void PassElement(XmlTextReader reader)
        {
            if (reader.IsEmptyElement)
                return;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        PassElement(reader);
                        break;
                    case XmlNodeType.EndElement:
                        return;
                }
            }
        }

        private bool ReadElementText(XmlTextReader reader, ref string strText)
        {
            bool stop = false, readText = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Text:
                        strText = reader.Value;
                        readText = true;
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            return readText;
        }
    }

    public class VariousData<DataType>
    {
        private DataType m_data;

        public VariousData()
        {
        }

        public VariousData(DataType data)
        {
            m_data = data;
        }

        public DataType Data
        {
            get { return m_data; }
            set { m_data = value; }
        }
    }

    public class Column
    {
        private string m_strID = "";

        public string ID
        {
            get { return m_strID; }
            set { m_strID = value; }
        }
    }

    public class RectColumn : Column
    {
        private System.Drawing.PointF m_ptTL;
        private System.Drawing.PointF m_ptBL;
        private System.Drawing.PointF m_ptBR;

        public System.Drawing.PointF TopLeft
        {
            get { return m_ptTL; }
            set { m_ptTL = value; }
        }

        public System.Drawing.PointF BottomLeft
        {
            get { return m_ptBL; }
            set { m_ptBL = value; }
        }

        public System.Drawing.PointF BottomRight
        {
            get { return m_ptBR; }
            set { m_ptBR = value; }
        }
    }

    public class CircleColumn : Column
    {
        private System.Drawing.PointF m_ptCenter;
        private double m_dRadius = 0.0;

        public System.Drawing.PointF Center
        {
            get { return m_ptCenter; }
            set { m_ptCenter = value; }
        }

        public double Radius
        {
            get { return m_dRadius; }
            set { m_dRadius = value; }
        }
    }
}
