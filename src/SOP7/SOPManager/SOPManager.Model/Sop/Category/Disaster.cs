using System;
using System.Text.Json;
using System.Collections.Generic;

namespace SOPManager.Model.Sop.Category
{
    public class Disaster
    {
        public enum Fields { ID, DisasterName, SubDisasterCategoryID, VersionID, UserLevelIDs, Description };

        private int m_nID = -1;
        private string m_strDisasterName = "";
        private int m_nSubDisasterCategoryID = -1;
        private int m_nVersionID = -1;
        // 이 SOP가 특정 타입의 계정에서만 사용되어야 할 경우 사용되는 Field.
        // Account.Level의 ID가 담긴다.
        // 이 값이 null일 경우 모든 계정에서 사용 가능하며, null이 아니면서 비어있을 경우 누구도 사용할 수 없는 SOP가 된다.
        private List<int> m_userLevelIDs = null;
        private string m_strDescription = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string DisasterName
        {
            get { return m_strDisasterName; }
            set { m_strDisasterName = value; }
        }

        public int SubDisasterCategoryID
        {
            get { return m_nSubDisasterCategoryID; }
            set { m_nSubDisasterCategoryID = value; }
        }

        public int VersionID
        {
            get { return m_nVersionID; }
            set { m_nVersionID = value; }
        }

        /// <summary>
        /// 이 SOP가 특정 타입의 계정에서만 사용되어야 할 경우 사용되는 Field.
        /// Account.Level의 ID가 담긴다.
        /// 이 값이 null일 경우 모든 계정에서 사용 가능하며, null이 아니면서 비어있을 경우 누구도 사용할 수 없는 SOP가 된다.
        /// </summary>
        public List<int> UserLevelIDs
        {
            get { return m_userLevelIDs; }
            set { m_userLevelIDs = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public static string TableName
        {
            get { return "SopCategoryDisaster"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.UserLevelIDs ||
                field == Fields.Description)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }

        /*public static Disaster FromJson(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.Read() == false || reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();

            string strPropertyName = reader.GetString();

            if (strPropertyName != "disaster")
                throw new JsonException();

            Disaster disaster = new Disaster();

            if (reader.Read() == false || reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            bool readID = false, readDisasterName = false, readVersionID = false;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    strPropertyName = reader.GetString();

                    if (strPropertyName == "id")
                    {
                        if (reader.Read() == false || reader.TokenType != JsonTokenType.Number)
                            throw new JsonException();

                        disaster.ID = reader.GetInt32();
                        readID = true;
                    }
                    else if (strPropertyName == "disasterName")
                    {
                        if (reader.Read() == false || reader.TokenType != JsonTokenType.String)
                            throw new JsonException();

                        disaster.DisasterName = reader.GetString();
                        readDisasterName = true;
                    }
                    else if (strPropertyName == "versionID")
                    {
                        if (reader.Read() == false || reader.TokenType != JsonTokenType.Number)
                            throw new JsonException();

                        disaster.VersionID = reader.GetInt32();
                        readVersionID = true;
                    }
                }
                else if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }
            }

            if (readID && readDisasterName && readVersionID)
                return disaster;

            return null;
        }*/
    }
}
