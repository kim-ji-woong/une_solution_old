using System;
using System.Text.Json;

namespace SOPManager.Model.Sop.Component
{
    public class Annotation : Section
    {
        public enum Fields { ID, GridID, GridRowIndex, GridColumnIndex, Width, Height, Text, ComponentID, StepMemberID, VAlign, HAlign, FontName, FontStyle, FontSize, LineSpace, FontColor, SectionNumber };

        private string m_strText = "";

        public string Text
        {
            get { return m_strText; }
            set { m_strText = value; }
        }

        public static string TableName
        {
            get { return "SopComponentAnnotation"; }
        }

        public override int ComponentType
        {
            get { return (int)SectionType.Annotation; }
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
        }

        public override void FromJson(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options, string strPropertyName)
        {
            if (string.Compare(strPropertyName, "text", true) == 0)
            {
                if (reader.Read() == false || reader.TokenType != JsonTokenType.String)
                    throw new Exception();

                this.Text = reader.GetString();
            }
            else
            {
                base.FromJson(ref reader, typeToConvert, options, strPropertyName);
            }
        }*/
    }
}
