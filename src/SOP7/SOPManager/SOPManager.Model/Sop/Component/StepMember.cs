using System;
using System.Text.Json;

namespace SOPManager.Model.Sop.Component
{
    using Category;

    public class StepMember
    {
        public enum Fields { ID, TeamID, TeamType, ActionStepID };

        public enum MemberTeamType { TemporaryNormalTeam = 0, TemporaryEmergencyTeam, RegularTeam };

        private int m_nID = -1;
        private int m_nTeamID = -1;
        private int m_nTeamType = -1;
        private int m_nActionStepID = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int TeamID
        {
            get { return m_nTeamID; }
            set { m_nTeamID = value; }
        }

        // MemberTeamType
        public int TeamType
        {
            get { return m_nTeamType; }
            set { m_nTeamType = value; }
        }

        public int ActionStepID
        {
            get { return m_nActionStepID; }
            set { m_nActionStepID = value; }
        }

        public static string TableName
        {
            get { return "SopComponentStepMember"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            isNullable = false;
            return field.ToString();
        }

        /*public static StepMember FromJson(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.Read() == false || reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();

            string strPropertyName = reader.GetString();

            if (string.Compare(strPropertyName, "stepMember", true) != 0)
                throw new JsonException();

            bool readID = false, readTeamID = false, readTeamType = false;
            StepMember stepMember = new StepMember();

            if (reader.Read() == false || reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    strPropertyName = reader.GetString();

                    if (string.Compare(strPropertyName, "id", true) == 0)
                    {
                        if (reader.Read() == false || reader.TokenType != JsonTokenType.Number)
                            throw new JsonException();

                        stepMember.ID = reader.GetInt32();
                        readID = true;
                    }
                    else if (string.Compare(strPropertyName, "teamID", true) == 0)
                    {
                        if (reader.Read() == false || reader.TokenType != JsonTokenType.Number)
                            throw new JsonException();

                        stepMember.TeamID = reader.GetInt32();
                        readTeamID = true;
                    }
                    else if (string.Compare(strPropertyName, "teamType", true) == 0)
                    {
                        if (reader.Read() == false || reader.TokenType != JsonTokenType.Number)
                            throw new JsonException();

                        stepMember.TeamType = reader.GetInt32();
                        readTeamType = true;
                    }
                }
                else if (reader.TokenType == JsonTokenType.EndObject)
                    break;
            }

            if (readID && readTeamID && readTeamType)
                return stepMember;

            return null;
        }*/
    }
}
