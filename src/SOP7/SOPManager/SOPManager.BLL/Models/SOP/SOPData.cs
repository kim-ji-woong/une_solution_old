using System;
using System.Collections.Generic;
using SOPManager.Model.Sop.Category;
using SOPManager.Model.Sop.Component;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SOPManager.BLL.Models.SOP
{
    //[JsonConverter(typeof(SOPDataJsonConverter))]
    public class SOPData : ICloneable
    {
        private DisasterCategory m_disasterCategory = null;
        private SubDisasterCategory m_subDisasterCategory = null;
        private Disaster m_disaster = null;
        private global::SOPManager.Model.Sop.Category.Version m_version = null;
        private List<ActionStepData> m_actionStepDatas = new List<ActionStepData>();

        public DisasterCategory DisasterCategory
        {
            get { return m_disasterCategory; }
            set { m_disasterCategory = value; }
        }

        public SubDisasterCategory SubDisasterCategory
        {
            get { return m_subDisasterCategory; }
            set { m_subDisasterCategory = value; }
        }

        public Disaster Disaster
        {
            get { return m_disaster; }
            set { m_disaster = value; }
        }

        public global::SOPManager.Model.Sop.Category.Version Version
        {
            get { return m_version; }
            set { m_version = value; }
        }

        public List<ActionStepData> ActionStepDatas
        {
            get { return m_actionStepDatas; }
            set { m_actionStepDatas = value; }
        }

        public object Clone()
        {
            SOPData data = new SOPData();
            data.m_disasterCategory = m_disasterCategory;
            data.m_subDisasterCategory = m_subDisasterCategory;
            data.m_disaster = m_disaster;
            data.m_version = m_version;

            if (m_actionStepDatas != null)
            {
                foreach (ActionStepData item in m_actionStepDatas)
                {
                    data.m_actionStepDatas.Add((ActionStepData)item.Clone());
                }
            }

            return data;
        }

        /*public static SOPData FromJson(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            DisasterCategory dc = DisasterCategory.FromJson(ref reader, typeToConvert, options);

            if (dc == null)
                throw new JsonException();

            SubDisasterCategory sdc = SubDisasterCategory.FromJson(ref reader, typeToConvert, options);

            if (sdc == null)
                throw new JsonException();

            Disaster disaster = Disaster.FromJson(ref reader, typeToConvert, options);

            if (disaster == null)
                throw new JsonException();

            global::SOPManager.Model.Sop.Category.Version version = global::SOPManager.Model.Sop.Category.Version.FromJson(ref reader, typeToConvert, options);
            List<ActionStepData> actionStepDatas = ActionStepData.FromJson(ref reader, typeToConvert, options);

            SOPData data = new SOPData();
            data.DisasterCategory = dc;
            data.SubDisasterCategory = sdc;
            data.Disaster = disaster;
            data.Version = version;

            if (actionStepDatas != null)
                data.ActionStepDatas.AddRange(actionStepDatas);

            return data;
        }

        public Dictionary<ActionStep, List<StepMember>> GetActionSteps(Dictionary<StepMember, List<Section>> dicStepMemberSections, Dictionary<StepMember, List<Arrow>> dicStepMemberArrows)
        {
            Dictionary<ActionStep, List<StepMember>> dicActionSteps = new Dictionary<ActionStep, List<StepMember>>();

            foreach (ActionStepData actionStepData in m_actionStepDatas)
            {
                if (actionStepData == null || actionStepData.ActionStep == null)
                    continue;

                List<StepMember> stepMembers = new List<StepMember>();
                dicActionSteps[actionStepData.ActionStep] = stepMembers;

                foreach (StepMemberData stepMemberData in actionStepData.StepMemberDatas)
                {
                    stepMembers.Add(stepMemberData.StepMember);

                    List<Section> sections = new List<Section>();
                    List<Arrow> arrows = new List<Arrow>();

                    dicStepMemberSections[stepMemberData.StepMember] = sections;
                    dicStepMemberArrows[stepMemberData.StepMember] = arrows;

                    foreach (Section section in stepMemberData.Sections)
                    {
                        sections.Add(section);
                    }

                    foreach (Arrow arrow in stepMemberData.Arrows)
                    {
                        arrows.Add(arrow);
                    }
                }
            }

            return dicActionSteps;
        }*/
    }

    /*public class SOPDataJsonConverter : JsonConverter<SOPData>
    {
        public override SOPData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            SOPData data = SOPData.FromJson(ref reader, typeToConvert, options);

            int nCount = 0;

            // 남은 부분이 있으면 마저 읽도록 한다.
            while (reader.Read())
            {
                nCount++;

                if (nCount > 1000)
                    break;
            }

            return data;
        }

        public override void Write(Utf8JsonWriter writer, SOPData value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            JsonSerializer.Serialize(writer, value, options);
            writer.WriteEndObject();
        }
    }*/
}
