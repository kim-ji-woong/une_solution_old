using System;
using System.Text.Json;

namespace SOPManager.Model.Sop.Component
{
    public class Decision : Section
    {
        public enum Fields { ID, GridID, GridRowIndex, GridColumnIndex, Width, Height, Text, TeamID, TeamType, ComponentID, StepMemberID, VAlign, HAlign, FontName, FontStyle, FontSize, LineSpace, FontColor, AutoRunScript, AutoRunScriptVariableTypes, SectionNumber, Description };

        private string m_strText = "";
        // 판단을 누가 해야하는가?
        private int? m_nTeamID = null;
        // m_nTeamID의 Team Type
        // StepMember.MemberTeamType
        private int? m_nTeamType = null;

        private string m_strAutoRunScript = null;
        //autoRunScript에서 사용되는 변수들의 Type에 관한 정보.(Unknown, boolean, double, integer, string)
        private string m_strAutoRunScriptVariableTypes = null;
        private string m_strDescription = null;

        public string Text
        {
            get { return m_strText; }
            set { m_strText = value; }
        }

        public int? TeamID
        {
            get { return m_nTeamID; }
            set { m_nTeamID = value; }
        }

        // m_nTeamID의 Team Type
        // StepMember.MemberTeamType
        public int? TeamType
        {
            get { return m_nTeamType; }
            set { m_nTeamType = value; }
        }

        public string AutoRunScript
        {
            get { return m_strAutoRunScript; }
            set { m_strAutoRunScript = value; }
        }

        public string AutoRunScriptVariableTypes
        {
            get { return m_strAutoRunScriptVariableTypes; }
            set { m_strAutoRunScriptVariableTypes = value; }
        }

        public string Description
        {
            get { return m_strDescription; }
            set { m_strDescription = value; }
        }

        public static string TableName
        {
            get { return "SopComponentDecision"; }
        }

        public override int ComponentType
        {
            get { return (int)SectionType.Decision; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.TeamID ||
                field == Fields.TeamType ||
                field == Fields.VAlign ||
                field == Fields.HAlign ||
                field == Fields.FontName ||
                field == Fields.FontStyle ||
                field == Fields.FontSize ||
                field == Fields.LineSpace ||
                field == Fields.FontColor ||
                field == Fields.AutoRunScript ||
                field == Fields.AutoRunScriptVariableTypes ||
                field == Fields.SectionNumber ||
                field == Fields.Description)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }

        /*public override void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
        {
            base.WriteJson(writer, options);
            writer.WriteString("text", Text);

            if (TeamID == null)
                writer.WriteNull("teamID");
            else
                writer.WriteNumber("teamID", (int)TeamID);

            if (TeamType == null)
                writer.WriteNull("teamType");
            else
                writer.WriteNumber("teamType", (int)TeamType);

            if (AutoRunScript == null)
                writer.WriteNull("autoRunScript");
            else
                writer.WriteString("autoRunScript", AutoRunScript);

            if (AutoRunScriptVariableTypes == null)
                writer.WriteNull("autoRunScriptVariableTypes");
            else
                writer.WriteString("autoRunScriptVariableTypes", AutoRunScriptVariableTypes);
        }

        public override void FromJson(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options, string strPropertyName)
        {
            if (string.Compare(strPropertyName, "text", true) == 0)
            {
                if (reader.Read() == false || reader.TokenType != JsonTokenType.String)
                    throw new Exception();

                this.Text = reader.GetString();
            }
            else if (string.Compare(strPropertyName, "teamID", true) == 0)
            {
                if (reader.Read() == false)
                    throw new Exception();

                if (reader.TokenType != JsonTokenType.Null)
                {
                    if (reader.TokenType != JsonTokenType.Number)
                        throw new Exception();
                    else
                        this.TeamID = reader.GetInt32();
                }
            }
            else if (string.Compare(strPropertyName, "teamType", true) == 0)
            {
                if (reader.Read() == false)
                    throw new Exception();

                if (reader.TokenType != JsonTokenType.Null)
                {
                    if (reader.TokenType != JsonTokenType.Number)
                        throw new Exception();
                    else
                        this.TeamType = reader.GetInt32();
                }
            }
            else if (string.Compare(strPropertyName, "autoRunScript", true) == 0)
            {
                if (reader.Read() == false)
                    throw new Exception();

                if (reader.TokenType != JsonTokenType.Null)
                {
                    if (reader.TokenType != JsonTokenType.String)
                        throw new Exception();
                    else
                        this.AutoRunScript = reader.GetString();
                }
            }
            else if (string.Compare(strPropertyName, "autoRunScriptVariableTypes", true) == 0)
            {
                if (reader.Read() == false)
                    throw new Exception();

                if (reader.TokenType != JsonTokenType.Null)
                {
                    if (reader.TokenType != JsonTokenType.String)
                        throw new Exception();
                    else
                        this.AutoRunScriptVariableTypes = reader.GetString();
                }
            }
            else
            {
                base.FromJson(ref reader, typeToConvert, options, strPropertyName);
            }
        }*/
    }
}
