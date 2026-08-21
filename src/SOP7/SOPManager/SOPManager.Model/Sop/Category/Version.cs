using System.Text.Json;

namespace SOPManager.Model.Sop.Category
{
    public class Version
    {
        public enum Fields { ID, IsNormal, CreateTime, LastAccessTime, VersionName, OwnerID, SiteID, Description };

        private int m_nID = -1;
        // true이면 평일/주간, false이면 야간/휴일
        private bool m_isNormal = true;
        private System.DateTime m_dtCreate = new System.DateTime();
        private System.DateTime m_dtLastAccess = new System.DateTime();
        private string m_strVersionName = "";
        private int m_nOwnerID = -1;
        private int m_nSiteID = -1;
        private string m_strDescription = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public bool IsNormal
        {
            get { return m_isNormal; }
            set { m_isNormal = value; }
        }

        public System.DateTime CreateTime
        {
            get { return m_dtCreate; }
            set { m_dtCreate = value; }
        }

        public System.DateTime LastAccessTime
        {
            get { return m_dtLastAccess; }
            set { m_dtLastAccess = value; }
        }

        public string VersionName
        {
            get { return m_strVersionName; }
            set { m_strVersionName = value; }
        }

        public int OwnerID
        {
            get { return m_nOwnerID; }
            set { m_nOwnerID = value; }
        }

        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public static string TableName
        {
            get { return "SopCategoryVersion"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.Description)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }

        /*public static Version FromJson(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.Read() == false || reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();

            string strPropertyName = reader.GetString();

            if (strPropertyName != "version")
                throw new JsonException();

            Version version = null;

            if (reader.Read() == false)
                throw new JsonException();

            if (reader.TokenType == JsonTokenType.Null)
                return version;

            version = new Version();

            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            bool readID = false, readNormal = false, readCreate = false;
            bool readLastAccess = false, readVersionName = false, readOwnerID = false;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    strPropertyName = reader.GetString();

                    if (string.Compare(strPropertyName, "id", true) == 0)
                    {
                        if (reader.Read() == false || reader.TokenType != JsonTokenType.Number)
                            throw new JsonException();

                        version.ID = reader.GetInt32();
                        readID = true;
                    }
                    else if (string.Compare(strPropertyName, "isNormal", true) == 0)
                    {
                        if (reader.Read() == false)
                            throw new JsonException();

                        if (reader.TokenType == JsonTokenType.Number)
                        {
                            int normal = reader.GetInt32();

                            if (normal == 1)
                                version.IsNormal = true;
                            else if (normal == 0)
                                version.IsNormal = false;
                            else
                                throw new JsonException();
                        }
                        else if (reader.TokenType == JsonTokenType.True ||
                            reader.TokenType == JsonTokenType.False)
                        {
                            version.IsNormal = reader.GetBoolean();
                        }
                        else
                            throw new JsonException();

                        readNormal = true;
                    }
                    else if (string.Compare(strPropertyName, "createTime", true) == 0)
                    {
                        if (reader.Read() == false || reader.TokenType != JsonTokenType.String)
                            throw new JsonException();

                        version.CreateTime = System.Convert.ToDateTime(reader.GetString());
                        readCreate = true;
                    }
                    else if (string.Compare(strPropertyName, "lastAccessTime", true) == 0)
                    {
                        if (reader.Read() == false || reader.TokenType != JsonTokenType.String)
                            throw new JsonException();

                        version.LastAccessTime = System.Convert.ToDateTime(reader.GetString());
                        readLastAccess = true;
                    }
                    else if (string.Compare(strPropertyName, "versionName", true) == 0)
                    {
                        if (reader.Read() == false || reader.TokenType != JsonTokenType.String)
                            throw new JsonException();

                        version.VersionName = reader.GetString();
                        readVersionName = true;
                    }
                    else if (string.Compare(strPropertyName, "ownerID", true) == 0)
                    {
                        if (reader.Read() == false || reader.TokenType != JsonTokenType.Number)
                            throw new JsonException();

                        version.OwnerID = reader.GetInt32();
                        readOwnerID = true;
                    }
                    else if (string.Compare(strPropertyName, "description", true) == 0)
                    {
                        if (reader.Read() == false)
                            throw new JsonException();

                        if (reader.TokenType != JsonTokenType.Null)
                        {
                            if (reader.TokenType != JsonTokenType.String)
                                throw new JsonException();
                            else
                                version.Description = reader.GetString();
                        }
                    }
                }
                else if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }
            }

            if (readID && readNormal && readCreate &&
                readLastAccess && readVersionName &&
                readOwnerID)
                return version;

            return null;
        }*/
    }
}
