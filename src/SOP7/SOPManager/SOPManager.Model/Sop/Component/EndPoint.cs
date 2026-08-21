using System;
using System.Text.Json;

namespace SOPManager.Model.Sop.Component
{
    public class EndPoint : Section
    {
        public enum Fields { ID, GridID, GridRowIndex, GridColumnIndex, Width, Height, Text, ComponentID, IsBegin, StepMemberID, VAlign, HAlign, FontName, FontStyle, FontSize, LineSpace, FontColor, SectionNumber };

        private string m_strText = "";
        private bool m_isBegin = true;

        public string Text
        {
            get { return m_strText; }
            set { m_strText = value; }
        }

        public bool IsBegin
        {
            get { return m_isBegin; }
            set { m_isBegin = value; }
        }

        public static string TableName
        {
            get { return "SopComponentEndPoint"; }
        }

        public override int ComponentType
        {
            get { return (int)SectionType.Endpoint; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.VAlign ||
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
            writer.WriteNumber("isBegin", IsBegin ? 1 : 0);
        }

        public override void FromJson(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options, string strPropertyName)
        {
            if (string.Compare(strPropertyName, "text", true) == 0)
            {
                if (reader.Read() == false || reader.TokenType != JsonTokenType.String)
                    throw new JsonException();

                this.Text = reader.GetString();
            }
            else if (string.Compare(strPropertyName, "isBegin", true) == 0)
            {
                if (reader.Read() == false)
                    throw new JsonException();

                if (reader.TokenType == JsonTokenType.Number)
                {
                    int nBegin = reader.GetInt32();

                    if (nBegin == 1)
                        this.IsBegin = true;
                    else if (nBegin == 0)
                        this.IsBegin = false;
                    else
                        throw new JsonException();
                }
                else if (reader.TokenType == JsonTokenType.True ||
                    reader.TokenType == JsonTokenType.False)
                {
                    this.IsBegin = reader.GetBoolean();
                }
                else
                    throw new JsonException();
            }
            else
            {
                base.FromJson(ref reader, typeToConvert, options, strPropertyName);
            }
        }*/
    }
}
