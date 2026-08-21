using System;
using System.Text.Json;
using System.Collections.Generic;

namespace SOPManager.Model.Sop.Component
{
    public class Arrow
    {
        public enum Fields { ID, Text, BeginComponentID, BeginComponentPosition, EndComponentID, EndComponentPosition, StepMemberID };

        public enum PositionType { Top = 0, Right, Bottom, Left, None };
        public enum ComponentType { Process = 0, Decision, Annotation, Endpoint, Link, TransSOP, Internal, External, None };

        private int m_nID = -1;
        private string m_strText = null;
        private int m_nBeginComponentID = -1;
        //BeginComponent의 어느쪽에 화살표가 붙어 있는가? (0 : Top, 1 : Right, 2 : Bottom, 3 : Left, 4 : 알수 없음)
        private int m_nBeginComponentPosition = 4;
        private int m_nEndComponentID = -1;
        //EndComponent의 어느쪽에 화살표가 붙어 있는가? (0 : Top, 1 : Right, 2 : Bottom, 3 : Left, 4 : 알수 없음)
        private int m_nEndComponentPosition = 4;
        private int m_nStepMemberID = -1;
        private Section m_sectionBegin = null;
        private Section m_sectionEnd = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public string Text
        {
            get { return m_strText; }
            set { m_strText = value; }
        }

        // 처음 1바이트는 ComponentType
        // 나머지 3바이트가 실제 ID
        public int BeginComponentID
        {
            get { return m_nBeginComponentID; }
            set { m_nBeginComponentID = value; }
        }

        public int BeginComponentPosition
        {
            get { return m_nBeginComponentPosition; }
            set { m_nBeginComponentPosition = value; }
        }

        public Section BeginSection
        {
            get { return m_sectionBegin; }
            set { m_sectionBegin = value; }
        }

        // 처음 1바이트는 ComponentType
        // 나머지 3바이트가 실제 ID
        public int EndComponentID
        {
            get { return m_nEndComponentID; }
            set { m_nEndComponentID = value; }
        }

        public int EndComponentPosition
        {
            get { return m_nEndComponentPosition; }
            set { m_nEndComponentPosition = value; }
        }

        public Section EndSection
        {
            get { return m_sectionEnd; }
            set { m_sectionEnd = value; }
        }

        public int StepMemberID
        {
            get { return m_nStepMemberID; }
            set { m_nStepMemberID = value; }
        }

        public static string TableName
        {
            get { return "SopComponentArrow"; }
        }

        public static string GetFieldName(Fields field, out bool isNullable)
        {
            if (field == Fields.Text)
                isNullable = true;
            else
                isNullable = false;

            return field.ToString();
        }

        public bool MakeComponentID()
        {
            if (MakeComponentID(m_sectionBegin, ref m_nBeginComponentID) == false)
                return false;
            if (MakeComponentID(m_sectionEnd, ref m_nEndComponentID) == false)
                return false;

            return true;
        }

        private bool MakeComponentID(Section section, ref int nComponentID)
        {
            if (section == null || section.ID < 0)
                return false;

            nComponentID = ((section.ComponentType << 24) | section.ID);
            return true;
        }

        /*public static List<Arrow> ListFromJson(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options, List<Section> sections)
        {
            if (reader.Read() == false || reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();

            string strPropertyName = reader.GetString();

            if (string.Compare(strPropertyName, "arrows", true) != 0)
                throw new JsonException();

            List<Arrow> arrows = new List<Arrow>();

            if (reader.Read() == false || reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;

                if (reader.TokenType != JsonTokenType.StartObject)
                    throw new JsonException();

                Section beginSection = null;
                Section endSection = null;

                Arrow arrow = Arrow.FromJson(ref reader, typeToConvert, options, sections, out beginSection, out endSection);

                if (arrow == null || beginSection == null || endSection == null)
                    throw new JsonException();

                arrow.BeginSection = beginSection;
                arrow.EndSection = endSection;
                arrows.Add(arrow);
            }

            return arrows;
        }

        public static Arrow FromJson(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options, List<Section> sections, out Section beginSection, out Section endSection)
        {
            beginSection = endSection = null;

            Arrow arrow = new Arrow();
            int nBeginComponentID = -1, nEndComponentID = -1;
            int nBeginColumnIndex = -1, nBeginRowIndex = -1;
            int nEndColumnIndex = -1, nEndRowIndex = -1;
            bool readID = false, readBeginComponentID = false, readBeginComponentPosition = false;
            bool readEndComponentID = false, readEndComponentPosition = false;
            bool readBeginColumnIndex = false, readBeginRowIndex = false, readEndColumnIndex = false, readEndRowIndex = false;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string strPropertyName = reader.GetString();

                    if (string.Compare(strPropertyName, "id", true) == 0)
                    {
                        if (reader.Read() == false || reader.TokenType != JsonTokenType.Number)
                            throw new JsonException();

                        arrow.ID = reader.GetInt32();
                        readID = true;
                    }
                    else if (string.Compare(strPropertyName, "text", true) == 0)
                    {
                        if (reader.Read() == false)
                            throw new Exception();

                        if (reader.TokenType != JsonTokenType.Null)
                        {
                            if (reader.TokenType != JsonTokenType.String)
                                throw new Exception();
                            else
                                arrow.Text = reader.GetString();
                        }
                    }
                    else if (string.Compare(strPropertyName, "beginComponentID", true) == 0)
                    {
                        if (reader.Read() == false || reader.TokenType != JsonTokenType.Number)
                            throw new Exception();

                        nBeginComponentID = reader.GetInt32();
                        readBeginComponentID = true;
                    }
                    else if (string.Compare(strPropertyName, "beginComponentColumnIndex", true) == 0)
                    {
                        if (reader.Read() == false || reader.TokenType != JsonTokenType.Number)
                            throw new Exception();

                        nBeginColumnIndex = reader.GetInt32();
                        readBeginColumnIndex = true;
                    }
                    else if (string.Compare(strPropertyName, "beginComponentRowIndex", true) == 0)
                    {
                        if (reader.Read() == false || reader.TokenType != JsonTokenType.Number)
                            throw new Exception();

                        nBeginRowIndex = reader.GetInt32();
                        readBeginRowIndex = true;
                    }
                    else if (string.Compare(strPropertyName, "beginComponentPosition", true) == 0)
                    {
                        if (reader.Read() == false || reader.TokenType != JsonTokenType.Number)
                            throw new Exception();

                        arrow.BeginComponentPosition = reader.GetInt32();
                        readBeginComponentPosition = true;
                    }
                    else if (string.Compare(strPropertyName, "endComponentID", true) == 0)
                    {
                        if (reader.Read() == false || reader.TokenType != JsonTokenType.Number)
                            throw new Exception();

                        nEndComponentID = reader.GetInt32();
                        readEndComponentID = true;
                    }
                    else if (string.Compare(strPropertyName, "endComponentColumnIndex", true) == 0)
                    {
                        if (reader.Read() == false || reader.TokenType != JsonTokenType.Number)
                            throw new Exception();

                        nEndColumnIndex = reader.GetInt32();
                        readEndColumnIndex = true;
                    }
                    else if (string.Compare(strPropertyName, "endComponentRowIndex", true) == 0)
                    {
                        if (reader.Read() == false || reader.TokenType != JsonTokenType.Number)
                            throw new Exception();

                        nEndRowIndex = reader.GetInt32();
                        readEndRowIndex = true;
                    }
                    else if (string.Compare(strPropertyName, "endComponentPosition", true) == 0)
                    {
                        if (reader.Read() == false || reader.TokenType != JsonTokenType.Number)
                            throw new Exception();

                        arrow.EndComponentPosition = reader.GetInt32();
                        readEndComponentPosition = true;
                    }
                }
                else if (reader.TokenType == JsonTokenType.EndObject)
                    break;
            }

            if (readBeginColumnIndex && readBeginRowIndex)
            {
                beginSection = GetSection(sections, nBeginColumnIndex, nBeginRowIndex);
            }
            else if (readBeginComponentID && nBeginComponentID > 0)
            {
                beginSection = GetSection(sections, nBeginComponentID);
            }

            if (beginSection == null)
                return null;

            if (readEndColumnIndex && readEndRowIndex)
            {
                endSection = GetSection(sections, nEndColumnIndex, nEndRowIndex);
            }
            else if (readEndComponentID && nEndComponentID > 0)
            {
                endSection = GetSection(sections, nEndComponentID);
            }

            if (endSection == null)
                return null;

            if (readID == false || readBeginComponentPosition == false || readEndComponentPosition == false)
                return null;

            return arrow;
        }*/

        private static Section GetSection(List<Section> sections, int nComponentID)
        {
            foreach (Section section in sections)
            {
                if (section.ID == nComponentID)
                    return section;
            }

            return null;
        }

        private static Section GetSection(List<Section> sections, int nColumnIndex, int nRowIndex)
        {
            foreach (Section section in sections)
            {
                if (section.GridColumnIndex == nColumnIndex && section.GridRowIndex == nRowIndex)
                    return section;
            }

            return null;
        }
    }
}
