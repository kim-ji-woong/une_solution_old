using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SOPManager.Model.Sop.Component
{
    //[JsonConverter(typeof(SectionJsonConverter))]
    public abstract class Section
    {
        public enum SectionType { Process = 0, Decision, Annotation, Endpoint, Link, TransSOP, Internal, External, None };

        private int m_nID = -1;
        private int m_nGridID = -1;
        private int m_nColumnIndex = -1;
        private int m_nRowIndex = -1;
        private float m_fWidth = 0.0f;
        private float m_fHeight = 0.0f;
        private string m_strComponentID = "";
        private int m_nStepMemberID = -1;
        // 0(None), 1(위쪽), 2(가운데), 3(아래쪽)
        private int? m_nVAlign = null;
        // 0(None), 1(왼쪽), 2(가운데), 3(오른쪽)
        private int? m_nHAlign = null;
        private string m_strFontName = null;
        // 1(굵게), 2(기울임), 4(밑줄), 8(취소선). Bit연산으로 복합 속성을 가질수 있다.
        private int? m_nFontStyle = null;
        private float? m_fFontSize = null;
        private float? m_fLineSpace = null;
        private int? m_nFontColor = null;
        private int? m_nSectionNumber = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public abstract int ComponentType
        {
            get;
        }

        // SectionGrid의 ID
        public int GridID
        {
            get { return m_nGridID; }
            set { m_nGridID = value; }
        }

        // SectionGridColumn의 ID
        public int GridColumnIndex
        {
            get { return m_nColumnIndex; }
            set { m_nColumnIndex = value; }
        }

        // SectionGridRow의 ID
        public int GridRowIndex
        {
            get { return m_nRowIndex; }
            set { m_nRowIndex = value; }
        }

        public float Width
        {
            get { return m_fWidth; }
            set { m_fWidth = value; }
        }

        public float Height
        {
            get { return m_fHeight; }
            set { m_fHeight = value; }
        }

        public string ComponentID
        {
            get { return m_strComponentID; }
            set { m_strComponentID = value; }
        }

        public int StepMemberID
        {
            get { return m_nStepMemberID; }
            set { m_nStepMemberID = value; }
        }

        // 0(None), 1(위쪽), 2(가운데), 3(아래쪽)
        public int? VAlign
        {
            get { return m_nVAlign; }
            set { m_nVAlign = value; }
        }

        // 0(None), 1(왼쪽), 2(가운데), 3(오른쪽)
        public int? HAlign
        {
            get { return m_nHAlign; }
            set { m_nHAlign = value; }
        }

        public string FontName
        {
            get { return m_strFontName; }
            set { m_strFontName = value; }
        }

        // 1(굵게), 2(기울임), 4(밑줄), 8(취소선). Bit연산으로 복합 속성을 가질수 있다.
        public int? FontStyle
        {
            get { return m_nFontStyle; }
            set { m_nFontStyle = value; }
        }

        public float? FontSize
        {
            get { return m_fFontSize; }
            set { m_fFontSize = value; }
        }

        public float? LineSpace
        {
            get { return m_fLineSpace; }
            set { m_fLineSpace = value; }
        }

        public int? FontColor
        {
            get { return m_nFontColor; }
            set { m_nFontColor = value; }
        }

        public int? SectionNumber
        {
            get { return m_nSectionNumber; }
            set { m_nSectionNumber = value; }
        }

        /*public virtual void WriteJson(Utf8JsonWriter writer, JsonSerializerOptions options)
        {
            writer.WriteNumber("id", ID);
            writer.WriteNumber("componentType", ComponentType);
            writer.WriteNumber("gridID", GridID);
            writer.WriteNumber("gridColumnIndex", GridColumnIndex);
            writer.WriteNumber("gridRowIndex", GridRowIndex);
            writer.WriteNumber("width", Width);
            writer.WriteNumber("height", Height);
            writer.WriteString("componentID", ComponentID);
            writer.WriteNumber("stepMemberID", StepMemberID);

            if (VAlign == null)
                writer.WriteNull("vAlign");
            else
                writer.WriteNumber("vAlign", (int)VAlign);

            if (HAlign == null)
                writer.WriteNull("hAlign");
            else
                writer.WriteNumber("hAlign", (int)HAlign);

            if (FontName == null)
                writer.WriteNull("fontName");
            else
                writer.WriteString("fontName", (string)FontName);

            if (FontStyle == null)
                writer.WriteNull("fontStyle");
            else
                writer.WriteNumber("fontStyle", (int)FontStyle);

            if (FontSize == null)
                writer.WriteNull("fontSize");
            else
                writer.WriteNumber("fontSize", (float)FontSize);

            if (LineSpace == null)
                writer.WriteNull("lineSpace");
            else
                writer.WriteNumber("lineSpace", (float)LineSpace);

            if (FontColor == null)
                writer.WriteNull("fontColor");
            else
                writer.WriteNumber("fontColor", (int)FontColor);
        }

        public static List<Section> ListFromJson(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.Read() == false || reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();

            string strPropertyName = reader.GetString();

            if (string.Compare(strPropertyName, "sections", true) != 0)
                throw new JsonException();

            List<Section> sections = new List<Section>();

            if (reader.Read() == false || reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;

                if (reader.TokenType != JsonTokenType.StartObject)
                    throw new JsonException();

                Section section = Section.FromJson(ref reader, typeToConvert, options);

                if (section == null)
                    throw new JsonException();

                sections.Add(section);
            }

            return sections;
        }

        public static Section FromJson(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            int nID = -1;
            Section section = null;
            bool readID = false;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string strPropertyName = reader.GetString();

                    if (string.Compare(strPropertyName, "id", true) == 0)
                    {
                        if (reader.Read() == false || reader.TokenType != JsonTokenType.Number)
                            throw new JsonException();

                        nID = reader.GetInt32();
                        readID = true;

                        if (section != null)
                            section.ID = nID;
                    }
                    else if (string.Compare(strPropertyName, "componentType", true) == 0)
                    {
                        if (reader.Read() == false)
                            throw new JsonException();

                        if (reader.TokenType == JsonTokenType.Number)
                        {
                            int nComponentType = reader.GetInt32();

                            if (nComponentType == (int)SectionType.Process)
                                section = new Process();
                            else if (nComponentType == (int)SectionType.Decision)
                                section = new Decision();
                            else if (nComponentType == (int)SectionType.Annotation)
                                section = new Annotation();
                            else if (nComponentType == (int)SectionType.Endpoint)
                                section = new EndPoint();
                            else if (nComponentType == (int)SectionType.Link)
                                section = new Link();
                            else if (nComponentType == (int)SectionType.Internal)
                                section = new InternalTransmission();
                            else
                                throw new JsonException();
                        }
                        else if (reader.TokenType == JsonTokenType.String)
                        {
                            string strComponentType = reader.GetString();

                            if (string.Compare(strComponentType, "process", true) == 0)
                                section = new Process();
                            else if (string.Compare(strComponentType, "decision", true) == 0)
                                section = new Decision();
                            if (string.Compare(strComponentType, "annotation", true) == 0)
                                section = new Annotation();
                            if (string.Compare(strComponentType, "endpoint", true) == 0)
                                section = new EndPoint();
                            if (string.Compare(strComponentType, "link", true) == 0)
                                section = new Link();
                            if (string.Compare(strComponentType, "internal", true) == 0)
                                section = new InternalTransmission();
                        }
                        else
                            throw new JsonException();

                        section.ID = nID;
                    }
                    else if (section != null)
                    {
                        section.FromJson(ref reader, typeToConvert, options, strPropertyName);
                    }
                }
                else if (reader.TokenType == JsonTokenType.EndObject)
                    break;
            }

            if (readID && section != null)
                return section;

            return null;
        }

        public virtual void FromJson(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options, string strPropertyName)
        {
            if (string.Compare(strPropertyName, "gridID", true) == 0)
            {
                if (reader.Read() == false || reader.TokenType != JsonTokenType.Number)
                    throw new Exception();

                this.GridID = reader.GetInt32();
            }
            else if (string.Compare(strPropertyName, "gridColumnIndex", true) == 0)
            {
                if (reader.Read() == false || reader.TokenType != JsonTokenType.Number)
                    throw new Exception();

                this.GridColumnIndex = reader.GetInt32();
            }
            else if (string.Compare(strPropertyName, "gridRowIndex", true) == 0)
            {
                if (reader.Read() == false || reader.TokenType != JsonTokenType.Number)
                    throw new Exception();

                this.GridRowIndex = reader.GetInt32();
            }
            else if (string.Compare(strPropertyName, "width", true) == 0)
            {
                if (reader.Read() == false || reader.TokenType != JsonTokenType.Number)
                    throw new Exception();

                this.Width = reader.GetSingle();
            }
            else if (string.Compare(strPropertyName, "height", true) == 0)
            {
                if (reader.Read() == false || reader.TokenType != JsonTokenType.Number)
                    throw new Exception();

                this.Height = reader.GetSingle();
            }
            else if (string.Compare(strPropertyName, "componentID", true) == 0)
            {
                if (reader.Read() == false || reader.TokenType != JsonTokenType.String)
                    throw new Exception();

                this.ComponentID = reader.GetString();
            }
            else if (string.Compare(strPropertyName, "vAlign", true) == 0)
            {
                if (reader.Read() == false)
                    throw new Exception();

                if (reader.TokenType != JsonTokenType.Null)
                {
                    if (reader.TokenType != JsonTokenType.Number)
                        throw new Exception();
                    else
                        this.VAlign = reader.GetInt32();
                }
            }
            else if (string.Compare(strPropertyName, "hAlign", true) == 0)
            {
                if (reader.Read() == false)
                    throw new Exception();

                if (reader.TokenType != JsonTokenType.Null)
                {
                    if (reader.TokenType != JsonTokenType.Number)
                        throw new Exception();
                    else
                        this.HAlign = reader.GetInt32();
                }
            }
            else if (string.Compare(strPropertyName, "fontName", true) == 0)
            {
                if (reader.Read() == false)
                    throw new Exception();

                if (reader.TokenType != JsonTokenType.Null)
                {
                    if (reader.TokenType != JsonTokenType.String)
                        throw new Exception();
                    else
                        this.FontName = reader.GetString();
                }
            }
            else if (string.Compare(strPropertyName, "fontStyle", true) == 0)
            {
                if (reader.Read() == false)
                    throw new Exception();

                if (reader.TokenType != JsonTokenType.Null)
                {
                    if (reader.TokenType != JsonTokenType.Number)
                        throw new Exception();
                    else
                        this.FontStyle = reader.GetInt32();
                }
            }
            else if (string.Compare(strPropertyName, "fontSize", true) == 0)
            {
                if (reader.Read() == false)
                    throw new Exception();

                if (reader.TokenType != JsonTokenType.Null)
                {
                    if (reader.TokenType != JsonTokenType.Number)
                        throw new Exception();
                    else
                        this.FontSize = reader.GetSingle();
                }
            }
            else if (string.Compare(strPropertyName, "lineSpace", true) == 0)
            {
                if (reader.Read() == false)
                    throw new Exception();

                if (reader.TokenType != JsonTokenType.Null)
                {
                    if (reader.TokenType != JsonTokenType.Number)
                        throw new Exception();
                    else
                        this.LineSpace = reader.GetSingle();
                }
            }
            else if (string.Compare(strPropertyName, "fontColor", true) == 0)
            {
                if (reader.Read() == false)
                    throw new Exception();

                if (reader.TokenType != JsonTokenType.Null)
                {
                    if (reader.TokenType != JsonTokenType.Number)
                        throw new Exception();
                    else
                        this.FontColor = reader.GetInt32();
                }
            }
            else
            {
                if (reader.Read() == false)
                    throw new Exception();
            }
        }*/
    }

    /*public class SectionJsonConverter : JsonConverter<Section>
    {
        public override Section Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string str = (string)reader.GetString();

            if (str == null)
                return null;

            return null;
        }

        public override void Write(Utf8JsonWriter writer, Section value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            value.WriteJson(writer, options);
            writer.WriteEndObject();
        }
    }*/
}
