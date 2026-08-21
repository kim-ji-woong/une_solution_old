using System;
using System.Collections.Generic;

namespace SOPManager.Model.Sop.Component
{
    public class ProcessMission : IComparable
    {
        public enum Fields { ID, MissionText, ProcessID };

        private int m_nID = -1;
        private string m_strMissionText = "";
        private int m_nProcessID = -1;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string MissionText
        {
            get { return m_strMissionText; }
            set { m_strMissionText = value; }
        }

        public int ProcessID
        {
            get { return m_nProcessID; }
            set { m_nProcessID = value; }
        }

        public static string TableName
        {
            get { return "SopComponentProcessMission"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            isNullable = false;
            return field.ToString();
        }

        public int CompareTo(object obj)
        {
            ProcessMission mission = (ProcessMission)obj;

            if (this.m_nID > mission.m_nID)
                return 1;
            else if (this.m_nID < mission.m_nID)
                return -1;

            return 0;
        }

        /*public static ProcessMission FromJson(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException();

            ProcessMission mission = new ProcessMission();
            bool readID = false, readText = false;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    break;

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string strPropertyName = reader.GetString();

                    if (string.Compare(strPropertyName, "id", true) == 0)
                    {
                        if (reader.Read() && reader.TokenType == JsonTokenType.Number)
                        {
                            mission.ID = reader.GetInt32();
                            readID = true;
                        }
                    }
                    else if (string.Compare(strPropertyName, "missionText", true) == 0)
                    {
                        if (reader.Read() && reader.TokenType == JsonTokenType.String)
                        {
                            mission.MissionText = reader.GetString();
                            readText = true;
                        }
                    }
                }
            }

            if (readID && readText)
                return mission;

            return null;
        }*/
    }
}
