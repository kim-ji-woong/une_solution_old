using System.Collections.Generic;
using SOPManager.Model.Sop.Component;
using System;
using System.Text.Json;

namespace SOPManager.BLL.Models.SOP
{
    public class StepMemberData
    {
        private StepMember m_stepMember = null;
        private string m_strStepMemberName = "";
        private List<SectionData> m_sections = new List<SectionData>();
        private List<ArrowData> m_arrows = new List<ArrowData>();
        //private List<Section> m_sections = new List<Section>();
        //private List<Arrow> m_arrows = new List<Arrow>();
        private List<int> m_gridColumnWidth = new List<int>();
        private List<int> m_gridRowHeight = new List<int>();

        public StepMember StepMember
        {
            get { return m_stepMember; }
            set { m_stepMember = value; }
        }

        public string StepMemberName
        {
            get { return m_strStepMemberName; }
            set { m_strStepMemberName = value; }
        }

        public List<SectionData> Sections
        {
            get { return m_sections; }
            set { m_sections = value; }
        }

        public List<ArrowData> Arrows
        {
            get { return m_arrows; }
            set { m_arrows = value; }
        }

        public List<int> GridColumnWidth
        {
            get { return m_gridColumnWidth; }
            set { m_gridColumnWidth = value; }
        }

        public List<int> GridRowHeight
        {
            get { return m_gridRowHeight; }
            set { m_gridRowHeight = value; }
        }

        /*public static List<StepMemberData> FromJson(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            List<StepMemberData> stepMemberDatas = new List<StepMemberData>();

            if (reader.Read() == false || reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;

                if (reader.TokenType == JsonTokenType.EndObject)
                    continue;

                if (reader.TokenType != JsonTokenType.StartObject)
                    throw new JsonException();

                StepMember stepMember = StepMember.FromJson(ref reader, typeToConvert, options);

                if (stepMember == null)
                    throw new JsonException();

                StepMemberData stepMemberData = new StepMemberData();
                stepMemberData.StepMember = stepMember;

                if (reader.Read() == false || reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException();

                string strPropertyName = reader.GetString();

                if (string.Compare(strPropertyName, "stepMemberName", true) != 0)
                    throw new JsonException();

                if (reader.Read() == false || reader.TokenType != JsonTokenType.String)
                    throw new JsonException();

                stepMemberData.StepMemberName = reader.GetString();

                List<Section> sections = Section.ListFromJson(ref reader, typeToConvert, options);

                if (sections == null)
                    throw new JsonException();

                List<Arrow> arrows = Arrow.ListFromJson(ref reader, typeToConvert, options, sections);

                if (arrows == null)
                    throw new JsonException();

                stepMemberData.Sections.AddRange(sections);
                stepMemberData.Arrows.AddRange(arrows);
                stepMemberDatas.Add(stepMemberData);
            }

            return stepMemberDatas;
        }*/
    }
}
