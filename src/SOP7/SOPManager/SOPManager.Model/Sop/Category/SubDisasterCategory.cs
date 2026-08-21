using System;
using System.Text.Json;

namespace SOPManager.Model.Sop.Category
{
    public class SubDisasterCategory
    {
        public enum Fields { ID, DisasterCategoryID, SubCategoryName };

        private int m_nID = -1;
        private int m_nDisasterCategoryID = -1;
        private string m_strSubCategoryName = "";

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int DisasterCategoryID
        {
            get { return m_nDisasterCategoryID; }
            set { m_nDisasterCategoryID = value; }
        }

        public string SubCategoryName
        {
            get { return m_strSubCategoryName; }
            set { m_strSubCategoryName = value; }
        }

        public static string TableName
        {
            get { return "SopCategorySubDisasterCategory"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            isNullable = false;
            return field.ToString();
        }

        /*public static SubDisasterCategory FromJson(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.Read() == false || reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();

            string strPropertyName = reader.GetString();

            if (strPropertyName != "subDisasterCategory")
                throw new JsonException();

            SubDisasterCategory sdc = new SubDisasterCategory();

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
                        if (reader.Read() == false || reader.TokenType != JsonTokenType.Number)
                            throw new JsonException();

                        sdc.ID = reader.GetInt32();
                        readID = true;
                    }
                    else if (strPropertyName == "subCategoryName")
                    {
                        if (reader.Read() == false || reader.TokenType != JsonTokenType.String)
                            throw new JsonException();

                        sdc.SubCategoryName = reader.GetString();
                        readCategoryName = true;
                    }
                }
                else if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }
            }

            if (readID && readCategoryName)
                return sdc;

            return null;
        }*/
    }
}
