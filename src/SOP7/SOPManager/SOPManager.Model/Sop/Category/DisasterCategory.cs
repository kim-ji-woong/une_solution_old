using System;
using System.Text.Json;

namespace SOPManager.Model.Sop.Category
{
    public class DisasterCategory
    {
        public enum Fields { ID, CategoryName, SiteID };

        private int m_nID = -1;
        private string m_strCategoryName = "";
        private int m_nSiteID = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string CategoryName
        {
            get { return m_strCategoryName; }
            set { m_strCategoryName = value; }
        }

        public int SiteID
        {
            get { return m_nSiteID; }
            set { m_nSiteID = value; }
        }

        public static string TableName
        {
            get { return "SopCategoryDisasterCategory"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            isNullable = false;
            return field.ToString();
        }

        /*public static DisasterCategory FromJson(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.Read() == false || reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();

            string strPropertyName = reader.GetString();

            if (strPropertyName != "disasterCategory")
                throw new JsonException();

            DisasterCategory dc = new DisasterCategory();

            if (reader.Read() == false || reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            bool readID = false, readCategoryName = false;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    strPropertyName = reader.GetString();

                    if (strPropertyName == "id")
                    {
                        if (reader.Read() == false ||  reader.TokenType != JsonTokenType.Number)
                            throw new JsonException();

                        dc.ID = reader.GetInt32();
                        readID = true;
                    }
                    else if (strPropertyName == "categoryName")
                    {
                        if (reader.Read() == false || reader.TokenType != JsonTokenType.String)
                            throw new JsonException();

                        dc.CategoryName = reader.GetString();
                        readCategoryName = true;
                    }
                }
                else if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }
            }

            if (readID && readCategoryName)
                return dc;

            return null;
        }*/
    }
}
