using System;
using System.Collections.Generic;
using System.Text.Json;

namespace SOPManager.Model.Sop.Component
{
    public class Process : Section
    {
        public enum Fields { ID, GridID, GridRowIndex, GridColumnIndex, Width, Height, Text, TeamList, ComponentID, OnlyTeamLeader, StepMemberID, VAlign, HAlign, FontName, FontStyle, FontSize, LineSpace, FontColor, AutoRun, SectionNumber };

        private string m_strText = "";
        private List<Receiver> m_teamList = new List<Receiver>();
        private bool? m_onlyTeamLeader = null;
        private bool m_autoRun = false;
        private List<ProcessMissionData> m_missions = new List<ProcessMissionData>();

        public string Text
        {
            get { return m_strText; }
            set { m_strText = value; }
        }

        public List<Receiver> TeamList
        {
            get { return m_teamList; }
        }

        public bool? OnlyTeamLeader
        {
            get { return m_onlyTeamLeader; }
            set { m_onlyTeamLeader = value; }
        }

        public bool AutoRun
        {
            get { return m_autoRun; }
            set { m_autoRun = value; }
        }

        public List<ProcessMissionData> Missions
        {
            get { return m_missions; }
        }

        public void AddTeam(int nTeamType, int nTeamID)
        {
            m_teamList.Add(new Receiver(nTeamType, nTeamID));
        }

        public void AddMission(ProcessMissionData mission)
        {
            if (mission == null)
                return;

            if (m_missions.Contains(mission) == false)
                m_missions.Add(mission);
        }

        public void RemoveMission(ProcessMissionData mission)
        {
            if (mission != null)
                m_missions.Remove(mission);
        }

        public static string TableName
        {
            get { return "SopComponentProcess"; }
        }

        public override int ComponentType
        {
            get { return (int)SectionType.Process; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.OnlyTeamLeader ||
                field == Fields.VAlign ||
                field == Fields.HAlign ||
                field == Fields.FontName ||
                field == Fields.FontStyle ||
                field == Fields.FontSize ||
                field == Fields.LineSpace ||
                field == Fields.FontColor ||
                field == Fields.SectionNumber)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }

        /*public override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
        {
            base.WriteJson(writer, options);
            writer.WriteString("text", Text);
            
            writer.WriteStartArray("teamList");

            foreach (KeyValuePair<int, int> teamData in TeamList)
            {
                writer.WriteStartObject();

                writer.WriteNumber("teamType", teamData.Key);
                writer.WriteNumber("teamID", teamData.Value);

                writer.WriteEndObject();
            }

            writer.WriteEndArray();

            if (OnlyTeamLeader == null)
                writer.WriteNull("onlyTeamLeader");
            else
                writer.WriteNumber("onlyTeamLeader", (bool)OnlyTeamLeader ? 1 : 0);

            writer.WriteNumber("AutoRun", (bool)AutoRun ? 1 : 0);

            writer.WriteStartArray("missions");

            foreach (ProcessMission mission in Missions)
            {
                JsonSerializer.Serialize(writer, mission, options);
            }

            writer.WriteEndArray();
        }

        public override void FromJson(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options, string strPropertyName)
        {
            if (string.Compare(strPropertyName, "text", true) == 0)
            {
                if (reader.Read() == false || reader.TokenType != JsonTokenType.String)
                    throw new JsonException();

                this.Text = reader.GetString();
            }
            else if (string.Compare(strPropertyName, "teamList", true) == 0)
            {
                if (reader.Read() == false || reader.TokenType != JsonTokenType.StartArray)
                    throw new JsonException();

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndArray)
                        break;

                    if (reader.TokenType == JsonTokenType.StartObject)
                    {
                        int nTeamType = -1, nTeamID = -1;
                        bool readTeamType = false, readTeamID = false;

                        while (reader.Read())
                        {
                            if (reader.TokenType == JsonTokenType.EndObject)
                                break;

                            if (reader.TokenType == JsonTokenType.PropertyName)
                            {
                                string propertyName = reader.GetString();

                                if (string.Compare(propertyName, "teamType", true) == 0)
                                {
                                    if (reader.Read() == false || reader.TokenType != JsonTokenType.Number)
                                        throw new JsonException();

                                    nTeamType = reader.GetInt32();
                                    readTeamType = true;
                                }
                                else if (string.Compare(propertyName, "teamID", true) == 0)
                                {
                                    if (reader.Read() == false || reader.TokenType != JsonTokenType.Number)
                                        throw new JsonException();

                                    nTeamID = reader.GetInt32();
                                    readTeamID = true;
                                }
                            }
                        }

                        if (readTeamType == false || readTeamID == false)
                            throw new JsonException();

                        this.TeamList.Add(new KeyValuePair<int, int>(nTeamType, nTeamID));
                    }
                }
            }
            else if (string.Compare(strPropertyName, "onlyTeamLeader", true) == 0)
            {
                if (reader.Read() == false)
                    throw new JsonException();

                if (reader.TokenType != JsonTokenType.Null)
                {
                    if (reader.TokenType == JsonTokenType.Number)
                    {
                        int teamLeader = reader.GetInt32();

                        if (teamLeader == 1)
                            this.OnlyTeamLeader = true;
                        else if (teamLeader == 0)
                            this.OnlyTeamLeader = false;
                        else
                            throw new JsonException();
                    }
                    else if (reader.TokenType == JsonTokenType.True ||
                        reader.TokenType == JsonTokenType.False)
                    {
                        this.OnlyTeamLeader = reader.GetBoolean();
                    }
                    else
                        throw new JsonException();
                }
            }
            else if (string.Compare(strPropertyName, "autoRun", true) == 0)
            {
                if (reader.Read() == false)
                    throw new JsonException();

                if (reader.TokenType == JsonTokenType.Number)
                {
                    int autoRun = reader.GetInt32();

                    if (autoRun == 1)
                        this.AutoRun = true;
                    else if (autoRun == 0)
                        this.AutoRun = false;
                    else
                        throw new JsonException();
                }
                else if (reader.TokenType == JsonTokenType.True ||
                        reader.TokenType == JsonTokenType.False)
                {
                    this.AutoRun = reader.GetBoolean();
                }
                else
                    throw new JsonException();
            }
            else if (string.Compare(strPropertyName, "missions", true) == 0)
            {
                if (reader.Read() == false || reader.TokenType != JsonTokenType.StartArray)
                    throw new JsonException();

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndArray)
                        break;

                    ProcessMission mission = ProcessMission.FromJson(ref reader, typeToConvert, options);

                    if (mission == null)
                        throw new JsonException();
                    else
                        this.Missions.Add(mission);
                }
            }
            else
            {
                base.FromJson(ref reader, typeToConvert, options, strPropertyName);
            }
        }*/
    }
}
