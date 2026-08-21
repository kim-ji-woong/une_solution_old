using System;
using System.Collections.Generic;
using System.Text.Json;

namespace SOPManager.Model.Sop.Category
{
    using Component;

    public class ActionStep
    {
        public enum Fields { ID, StepName, DisasterID, UserDefinedConfigID };

        private int m_nID = -1;
        private string m_strStepName = "";
        private int m_nDisasterID = -1;
        private int? m_nUserDefinedConfigID = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string StepName
        {
            get { return m_strStepName; }
            set { m_strStepName = value; }
        }

        public int DisasterID
        {
            get { return m_nDisasterID; }
            set { m_nDisasterID = value; }
        }

        public int? UserDefinedConfigID
        {
            get { return m_nUserDefinedConfigID; }
            set { m_nUserDefinedConfigID = value; }
        }

        public static string TableName
        {
            get { return "SopCategoryActionStep"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.UserDefinedConfigID)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }

        /*public static ActionStep FromJson(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            bool readID = false, readName = false;
            ActionStep actionStep = new ActionStep();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string strPropertyName = reader.GetString();

                    if (string.Compare(strPropertyName, "id", true) == 0)
                    {
                        if (reader.Read() == false || reader.TokenType != JsonTokenType.Number)
                            throw new JsonException();

                        actionStep.ID = reader.GetInt32();
                        readID = true;
                    }
                    else if (string.Compare(strPropertyName, "stepName", true) == 0)
                    {
                        if (reader.Read() == false || reader.TokenType != JsonTokenType.String)
                            throw new JsonException();

                        actionStep.StepName = reader.GetString();
                        readName = true;
                    }
                }
                else if (reader.TokenType == JsonTokenType.EndObject)
                    break;
            }

            if (readID && readName)
                return actionStep;

            return null;
        }*/
    }
}
