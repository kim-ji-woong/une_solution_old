using System;
using System.Collections.Generic;
using System.Text.Json;

namespace SOPManager.Model.Sop.Component
{
    public class InternalTransmission : Section
    {
        public enum Fields { ID, GridID, GridRowIndex, GridColumnIndex, Width, Height, Text, ComponentID, UseSMS, UseBroadcast, UseEmail, StepMemberID, Message, TeamList, OnlyTeamLeader, VAlign, HAlign, FontName, FontStyle, FontSize, LineSpace, FontColor, AutoRun, UseSiren, SectionNumber };

        private string m_strText = "";
        private bool m_useSMS = false;
        private bool m_useBroadcast = false;
        private bool? m_useEmail = null;
        private string m_strMessage = null;
        private List<Receiver> m_teamList = new List<Receiver>();
        // 팀장에게만 전송할 것인가?
        private bool? m_onlyTeamLeader = null;
        private bool m_autoRun = false;
        private bool? m_useSiren = null;

        public string Text
        {
            get { return m_strText; }
            set { m_strText = value; }
        }

        public bool UseSMS
        {
            get { return m_useSMS; }
            set { m_useSMS = value; }
        }

        public bool UseBroadcast
        {
            get { return m_useBroadcast; }
            set { m_useBroadcast = value; }
        }

        public bool? UseEmail
        {
            get { return m_useEmail; }
            set { m_useEmail = value; }
        }

        // 방송 또는 문자전송시 사용하는 메시지
        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
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

        public bool? UseSiren
        {
            get { return m_useSiren; }
            set { m_useSiren = value; }
        }

        public void AddTeam(int nTeamType, int nTeamID)
        {
            m_teamList.Add(new Receiver(nTeamType, nTeamID));
        }

        public static string TableName
        {
            get { return "SopComponentInternalTransmission"; }
        }

        public override int ComponentType
        {
            get { return (int)SectionType.Internal; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.UseEmail ||
                field == Fields.Message ||
                field == Fields.TeamList ||
                field == Fields.OnlyTeamLeader ||
                field == Fields.VAlign ||
                field == Fields.HAlign ||
                field == Fields.FontName ||
                field == Fields.FontStyle ||
                field == Fields.FontSize ||
                field == Fields.LineSpace ||
                field == Fields.FontColor ||
                field == Fields.UseSiren ||
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
            writer.WriteNumber("useSMS", UseSMS ? 1 : 0);
            writer.WriteNumber("useBroadcast", UseBroadcast ? 1 : 0);

            if (Message == null)
                writer.WriteNull("message");
            else
                writer.WriteString("message", Message);

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

            if (UseSiren == null)
                writer.WriteNull("useSiren");
            else
                writer.WriteNumber("useSiren", (bool)UseSiren ? 1 : 0);
        }

        public override void FromJson(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options, string strPropertyName)
        {
            if (string.Compare(strPropertyName, "text", true) == 0)
            {
                if (reader.Read() == false || reader.TokenType != JsonTokenType.String)
                    throw new JsonException();

                this.Text = reader.GetString();
            }
            else if (string.Compare(strPropertyName, "useSMS", true) == 0)
            {
                if (reader.Read() == false)
                    throw new JsonException();

                if (reader.TokenType == JsonTokenType.Number)
                {
                    int sms = reader.GetInt32();

                    if (sms == 1)
                        this.UseSMS = true;
                    else if (sms == 0)
                        this.UseSMS = false;
                    else
                        throw new JsonException();
                }
                else if (reader.TokenType == JsonTokenType.True ||
                    reader.TokenType == JsonTokenType.False)
                {
                    this.UseSMS = reader.GetBoolean();
                }
                else
                    throw new JsonException();
            }
            else if (string.Compare(strPropertyName, "useBroadcast", true) == 0)
            {
                if (reader.Read() == false)
                    throw new JsonException();

                if (reader.TokenType == JsonTokenType.Number)
                {
                    int broadcast = reader.GetInt32();

                    if (broadcast == 1)
                        this.UseBroadcast = true;
                    else if (broadcast == 0)
                        this.UseBroadcast = false;
                    else
                        throw new JsonException();
                }
                else if (reader.TokenType == JsonTokenType.True ||
                    reader.TokenType == JsonTokenType.False)
                {
                    this.UseBroadcast = reader.GetBoolean();
                }
                else
                    throw new JsonException();
            }
            else if (string.Compare(strPropertyName, "message", true) == 0)
            {
                if (reader.Read() == false)
                    throw new JsonException();

                if (reader.TokenType != JsonTokenType.Null)
                {
                    if (reader.TokenType != JsonTokenType.String)
                        throw new JsonException();
                    else
                        this.Message = reader.GetString();
                }
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
            else if (string.Compare(strPropertyName, "useSiren", true) == 0)
            {
                if (reader.Read() == false)
                    throw new JsonException();

                if (reader.TokenType != JsonTokenType.Null)
                {
                    if (reader.TokenType == JsonTokenType.Number)
                    {
                        int siren = reader.GetInt32();

                        if (siren == 1)
                            this.UseSiren = true;
                        else if (siren == 0)
                            this.UseSiren = false;
                        else
                            throw new JsonException();
                    }
                    else if (reader.TokenType == JsonTokenType.True ||
                        reader.TokenType == JsonTokenType.False)
                    {
                        this.UseSiren = reader.GetBoolean();
                    }
                    else
                        throw new JsonException();
                }
            }
            else
            {
                base.FromJson(ref reader, typeToConvert, options, strPropertyName);
            }
        }*/
    }
}
