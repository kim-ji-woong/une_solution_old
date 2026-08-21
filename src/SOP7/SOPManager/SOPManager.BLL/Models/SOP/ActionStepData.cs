using System.Collections.Generic;
using SOPManager.Model.Sop.Category;
using SOPManager.Model.Sop.Component;
using System;
using System.Text.Json;
using Common.Model.History;

namespace SOPManager.BLL.Models.SOP
{
    public class ActionStepData : ICloneable
    {
        private string m_strStepName = "";
        private ActionStep m_actionStep = null;
        private List<StepMemberData> m_stepMemberDatas = new List<StepMemberData>();

        public string StepName
        {
            get { return m_strStepName; }
            set { m_strStepName = value; }
        }

        public ActionStep ActionStep
        {
            get { return m_actionStep; }
            set { m_actionStep = value; }
        }

        public List<StepMemberData> StepMemberDatas
        {
            get { return m_stepMemberDatas; }
            set { m_stepMemberDatas = value; }
        }



        /*public static List<ActionStepData> FromJson(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.Read() == false || reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException();

            string strPropertyName = reader.GetString();

            if (string.Compare(strPropertyName, "actionSteps", true) != 0)
                throw new JsonException();

            List<ActionStepData> actionStepDatas = new List<ActionStepData>();

            if (reader.Read() == false || reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;

                if (reader.TokenType == JsonTokenType.EndObject)
                    continue;

                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    ActionStepData actionStepData = ReadActionStepData(ref reader, typeToConvert, options);

                    if (actionStepData == null)
                        throw new JsonException();

                    actionStepDatas.Add(actionStepData);
                }
                else if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string strProperty = reader.GetString();
                    System.Diagnostics.Trace.WriteLine(strProperty);
                }
            }

            return actionStepDatas;
        }

        private static ActionStepData ReadActionStepData(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            ActionStepData actionStepData = new ActionStepData();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    break;

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    string strProperty = reader.GetString();

                    if (string.Compare(strProperty, "stepName", true) == 0)
                    {
                        if (reader.Read() == false || reader.TokenType != JsonTokenType.String)
                            throw new JsonException();
                        else
                            actionStepData.StepName = reader.GetString();
                    }
                    else if (string.Compare(strProperty, "actionStep", true) == 0)
                    {
                        if (reader.Read() == false)
                            throw new JsonException();

                        if (reader.TokenType == JsonTokenType.Null)
                            continue;
                        else if (reader.TokenType != JsonTokenType.StartObject)
                            throw new JsonException();

                        ActionStep actionStep = ActionStep.FromJson(ref reader, typeToConvert, options);

                        if (actionStep == null)
                            throw new JsonException();

                        actionStepData.ActionStep = actionStep;
                    }
                    else if (string.Compare(strProperty, "stepMemberDatas", true) == 0)
                    {
                        List<StepMemberData> stepMembers = StepMemberData.FromJson(ref reader, typeToConvert, options);

                        if (stepMembers == null)
                            throw new JsonException();

                        actionStepData.StepMemberDatas.AddRange(stepMembers);
                    }
                }
            }

            return actionStepData;
        }*/

        // SOP Simulrator에서 사용

        private ActionStepHistory m_actionStepHistory = null;
        public ActionStepHistory _ActionStepHistory
        {
            get { return m_actionStepHistory; }
            set { m_actionStepHistory = value; }
        }

        private SectionData m_currentSection = null;
        public SectionData CurrentSection
        {
            get { return m_currentSection; }
            set { m_currentSection = value; }
        }

        private List<ComponentHistoryData> m_componentHistoryData = null;
        public List<ComponentHistoryData> ComponentHistoryData
        {
            get { return m_componentHistoryData; }
            set { m_componentHistoryData = value; }
        }

        public object Clone()
        {
            ActionStepData data = new ActionStepData();
            data.m_actionStepHistory = m_actionStepHistory;
            data.m_currentSection = m_currentSection;
            data.m_strStepName = m_strStepName;
            data.m_actionStep = m_actionStep;
            data.m_stepMemberDatas = m_stepMemberDatas;

            if (m_componentHistoryData != null)
            {
                foreach (ComponentHistoryData item in m_componentHistoryData)
                {
                    if (data.m_componentHistoryData == null)
                        data.m_componentHistoryData = new List<ComponentHistoryData>();

                    data.m_componentHistoryData.Add(item);
                }
            }

            return data;
        }
    }

    public class ComponentHistoryData
    {
        private ComponentHistory m_componentHistory = null;
        public ComponentHistory ComponentHistory
        {
            get { return m_componentHistory; }
            set { m_componentHistory = value; }
        }

        private ComponentHistoryDetail m_componentHistoryDetails = null;
        public ComponentHistoryDetail _ComponentHistoryDetails
        {
            get { return m_componentHistoryDetails; }
            set { m_componentHistoryDetails = value; }
        }
    }
}
