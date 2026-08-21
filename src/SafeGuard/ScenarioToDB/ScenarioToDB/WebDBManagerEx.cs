using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace ScenarioToDB
{
    public class WebDBManagerEx : DBUtility.WebDBManager
    {
        private int m_nDisasterID = -1;
        private int m_nScenarioID = -1;

        public WebDBManagerEx()
            : base(0)
        {
        }

        public override ArrayList GetResultData(string strSQLQuery, int nTransaction, string szDBName = null)
        {
            return base.GetResultData(strSQLQuery, nTransaction, "SafeGuard");
        }

        // distance : km
        public bool InsertDisaster(string strChemistry, string strAccident, string strMixedFactor, string strWeather, bool isDayLight, double distance, string strDamage)
        {
            int nChemistryID = GetChemistryID(strChemistry);
            int nAccidentID = GetAccidentID(strAccident);
            int nMixedFactorID = GetMixedFactorID(strMixedFactor);
            int nWeatherID = GetWeatherID(strWeather, isDayLight);

            if (nChemistryID < 0 || nAccidentID < 0 || nMixedFactorID < 0 || nWeatherID < 0)
                return false;

            string strSQL = string.Format("Select ID from Disaster where ChemistryID = {0} and AccidentID = {1} and MixedFactorID = {2} and WeatherID = {3}",
                nChemistryID, nAccidentID, nMixedFactorID < 0 ? "NULL" : nMixedFactorID.ToString(), nWeatherID);

            ArrayList arrResult = GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return InsertDisaster(nChemistryID, nAccidentID, nMixedFactorID, nWeatherID, distance, strDamage);
            else
                m_nDisasterID = GetIntField(arrResult[0].ToString(), -1);

            return true;
        }

        public bool InsertScenario(string strFilePath, ArrayList arrActionSteps)
        {
            int nIndex1 = strFilePath.LastIndexOf('\\');
            int nIndex2 = strFilePath.LastIndexOf('.');
            string strScenarioName = strFilePath.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);

            if (arrActionSteps == null || arrActionSteps.Count == 0)
                return false;

            ActionStep actionStep = (ActionStep)arrActionSteps[0];

            string strSQL = string.Format("select id from Scenario where ScenarioName = '{0}' and DisasterID = {1}", strScenarioName, m_nDisasterID);
            ArrayList arrResult = GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                m_nScenarioID = InsertScenario(strScenarioName, actionStep);
            else
                m_nScenarioID = GetIntField(arrResult[0].ToString(), -1);

            return true;
        }

        private int InsertScenario(string strScenarioName, ActionStep actionStep)
        {
            DateTime dtNow = DateTime.Now;
            string strCurrentTime = dtNow.ToShortDateString() + string.Format(" {0:00}:{1:00}:{2:00}", dtNow.Hour, dtNow.Minute, dtNow.Second);

            int nVersionID = GetLastID("Version") + 1;

            string strSQL = string.Format("Insert into Version (ID, CreateTime, LastAccessTime, VersionName, Description) values ({0}, '{1}', '{2}', '{3}', NULL)",
                nVersionID, strCurrentTime, strCurrentTime, "V1.0");

            if (GetResultData(strSQL, 0) == null)
                return -1;

            int nScenarioID = GetLastID("Scenario") + 1;

            strSQL = string.Format("Insert into Scenario (ID, ScenarioName, DisasterID, VersionID, Description) values ({0}, '{1}', {2}, {3}, NULL)",
                nScenarioID, strScenarioName, m_nDisasterID, nVersionID);

            if (GetResultData(strSQL, 0) == null)
                return -1;

            int nActionStepID = InsertActionStep(actionStep, nScenarioID);

            if (nActionStepID < 0)
            {
                RemoveScenario(nScenarioID, nVersionID);
                return -1;
            }

            return nScenarioID;
        }

        private void RemoveScenario(int nScenarioID, int nVersionID)
        {
            if (!RemoveID(nVersionID, "Scenario"))
                return;

            RemoveID(nScenarioID, "Version");
        }

        private bool RemoveID(int nID, string strTableName)
        {
            string strSQL = "Delete from " + strTableName + " where ID = " + nID.ToString();
            return GetResultData(strSQL, 0) != null;
        }

        private int InsertActionStep(ActionStep actionStep, int nScenarioID)
        {
            if (actionStep.StepMemberList == null || actionStep.StepMemberList.Count == 0)
                return -1;

            StepMember stepMember = (StepMember)actionStep.StepMemberList[0];

            int nActionStepID = GetLastID("ActionStep") + 1;
            string strSQL = string.Format("Insert into ActionStep (ID, StepName, ScenarioID, Description) values ({0}, '{1}', {2}, NULL)",
                nActionStepID, actionStep.StepName, nScenarioID);

            if (GetResultData(strSQL, 0) == null)
                return -1;

            int nStepMemberID = InsertStepMember(stepMember, nActionStepID);

            if (nStepMemberID < 0)
            {
                RemoveID(nActionStepID, "ActionStep");
                return -1;
            }

            return nActionStepID;
        }

        private int InsertStepMember(StepMember stepMember, int nActionStepID)
        {
            int nStepMemberID = GetLastID("StepMember") + 1;

            string strSQL = string.Format("Insert into StepMember (ID, TeamName, ActionStepID) values ({0}, '{1}', {2})",
                nStepMemberID, stepMember.TeamName, nActionStepID);

            if (GetResultData(strSQL, 0) == null)
                return -1;

            foreach (Component component in stepMember.ComponentList)
            {
                if (!AddComponent(component, nStepMemberID))
                {
                    RemoveComponents(nStepMemberID);
                    RemoveID(nStepMemberID, "StepMember");
                    return -1;
                }
            }

            int nArrowID = GetLastID("Arrow") + 1;

            foreach (Arrow arrow in stepMember.ArrowList)
            {
                if (!AddArrow(arrow, nStepMemberID, nArrowID++))
                {
                    RemoveComponents(nStepMemberID);
                    RemoveID(nStepMemberID, "StepMember");
                    return -1;
                }
            }

            return nStepMemberID;
        }

        private bool AddArrow(Arrow arrow, int nStepMemberID, int nArrowID)
        {
            int nBeginComponentID = (((int)arrow.BeginComponent.Property.Type) << 24) | arrow.BeginComponent.ID;
            int nEndComponentID = (((int)arrow.EndComponent.Property.Type) << 24) | arrow.EndComponent.ID;
            string strSQL = string.Format("insert into Arrow (ID, Text, BeginComponentID, BeginComponentPosition, EndComponentID, EndComponentPosition, StepMemberID) values ({0}, '{1}', {2}, {3}, {4}, {5}, {6})",
                    nArrowID, ChangeSpecialCharacter(arrow.Text), nBeginComponentID, arrow.BeginComponentPosition, nEndComponentID, arrow.EndComponentPosition, nStepMemberID);

            return GetResultData(strSQL, 0) != null;
        }

        private void RemoveComponents(int nStepMemberID)
        {
            RemoveComponent(nStepMemberID, "Arrow");
            RemoveComponent(nStepMemberID, "Decision");
            RemoveComponent(nStepMemberID, "EndPoint");
            RemoveComponent(nStepMemberID, "Annotation");

            RemoveProcess(nStepMemberID);
        }

        private void RemoveProcess(int nStepMemberID)
        {
            string strSQL = "Delete from ProcessMission where processID in (select id from Process where StepMemberID = " + nStepMemberID.ToString() + ")";

            if (GetResultData(strSQL, 0) == null)
                return;

            RemoveComponent(nStepMemberID, "Process");
        }

        private void RemoveComponent(int nStepMemberID, string strTableName)
        {
            string strSQL = "Delete from " + strTableName + " where StepMemberID = " + nStepMemberID.ToString();
            GetResultData(strSQL, 0);
        }

        private bool AddComponent(Component component, int nStepMemberID)
        {
            if (component.Property.Type == Sections.Section.ComponentType.ANNOTATION)
                return AddAnnotation(component, nStepMemberID);
            else if (component.Property.Type == Sections.Section.ComponentType.ENDPOINT)
                return AddEndPoint(component, nStepMemberID);
            else if (component.Property.Type == Sections.Section.ComponentType.DECISION)
                return AddDecision(component, nStepMemberID);
            else if (component.Property.Type == Sections.Section.ComponentType.PROCESS)
                return AddProcess(component, nStepMemberID);

            return false;
        }

        private bool AddProcess(Component component, int nStepMemberID)
        {
            int nComponentID = GetLastID("Process") + 1;

            string strFormat = "insert into Process (ID, x, y, width, height, text, TeamList, ComponentID, useMissionMessage, StepMemberID) values ";
            strFormat += "({0}, {1}, {2}, {3}, {4}, '{5}', '{6}', '{7}', {8}, {9})";

            PropertyProcess prop = (PropertyProcess)component.Property;

            string strTeamList = GetTeamList(prop.TeamList);

            string strSQL = string.Format(strFormat,
                nComponentID, component.X, component.Y, component.Width, component.Height,
                component.Text, strTeamList, component.ComponentID, prop.UseMissionMessage ? 1 : 0, nStepMemberID);

            if (GetResultData(strSQL, 0) == null)
                return false;

            if (!AddProcessMission(prop, nComponentID))
                return false;

            component.ID = nComponentID;
            return true;
        }

        private bool AddProcessMission(PropertyProcess prop, int nProcessID)
        {
            int nMissionID = GetLastID("ProcessMission") + 1;

            foreach (Sections.MissionItem mission in prop.Missions)
            {
                string strSQL = string.Format("insert into ProcessMission (ID, missionText, ProcessID) values ({0}, '{1}', {2})",
                    nMissionID++, ChangeSpecialCharacter(mission.Mission), nProcessID);

                if (GetResultData(strSQL, 0) == null)
                    return false;
            }

            return true;
        }

        // strText에 따옴표(')가 있을 경우 DB에서 인식할 수 있도록 (******)로 치환시킨다.
        private string ChangeSpecialCharacter(string strText)
        {
            return strText.Replace("'", "******");
        }

        // TeamID(TeamType), ... 형태로 되어 있는 strTeamList를 분석하여 Team 이름들을 얻어온다.
        // ex) 1(0), 1(2), 2(3), 5(0)
        private string GetTeamList(string strTeamList)
        {
            int nBeginIndex = 0;
            int nLen = strTeamList.Length;

            string strTeamNameList = "";

            while (nBeginIndex < nLen)
            {
                int nDotIndex = strTeamList.IndexOf(',', nBeginIndex);
                if (nDotIndex < 0) break;

                if (!GetTeamName(ref strTeamNameList, strTeamList, nBeginIndex, nDotIndex))
                    return "";

                nBeginIndex = nDotIndex + 1;
            }

            if (!GetTeamName(ref strTeamNameList, strTeamList, nBeginIndex, nLen))
                return "";

            return strTeamNameList;
        }

        private bool GetTeamName(ref string strTeamNameList, string strTeamList, int nBeginIndex, int nEndIndex)
        {
            return true;
            if (nBeginIndex == nEndIndex)
                return true;

            string strToken = strTeamList.Substring(nBeginIndex, nEndIndex - nBeginIndex);

            int nIndex1 = strTeamList.IndexOf('(', nBeginIndex);
            int nIndex2 = strTeamList.IndexOf(')', nBeginIndex);

            if (nIndex1 < 0 || nIndex2 < 0)
                return false;

            string strTeamID = strTeamList.Substring(nBeginIndex, nIndex1 - nBeginIndex);
            string strTeamType = strTeamList.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);

            strTeamID = strTeamID.Trim();
            strTeamType = strTeamType.Trim();

            int nTeamID = -1;

            if (!int.TryParse(strTeamID, out nTeamID))
                return false;

            string strTeamName = null;

            if (strTeamType == "0")
            {
                strTeamName = GetTeamName(nTeamID, "TemporaryNormalTeam");
            }
            else if (strTeamType == "1")
            {
                strTeamName = GetTeamName(nTeamID, "TemporaryEmergencyTeam");
            }
            else if (strTeamType == "2")
            {
                strTeamName = GetTeamName(nTeamID, "ExternalCompanyTeam");
            }
            else if (strTeamType == "3")
            {
                strTeamName = GetTeamName(nTeamID, "UserDefinedTeam");
            }
            else if (strTeamType == "4")
            {
                strTeamName = GetTeamName(nTeamID, "RegularTeam");
            }
            else
                return false;

            if (strTeamName == null)
                return false;

            if (strTeamNameList.Length == 0)
                strTeamNameList = strTeamName;
            else
                strTeamNameList += ", " + strTeamName;

            return true;
        }

        private string GetTeamName(int nTeamID, string strTableName)
        {
            string strSQL = "select TeamName from " + strTableName + " where ID = " + nTeamID.ToString();
            ArrayList arrResult = base.GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return null;

            return GetStringField(arrResult[0], "");
        }

        private bool AddDecision(Component component, int nStepMemberID)
        {
            int nComponentID = GetLastID("Decision") + 1;

            string strFormat = "insert into Decision (ID, x, y, width, height, text, ComponentID, StepMemberID) values ";
            strFormat += "({0}, {1}, {2}, {3}, {4}, '{5}', '{6}', {7})";

            string strSQL = string.Format(strFormat,
                nComponentID, component.X, component.Y, component.Width, component.Height,
                component.Text, component.ComponentID, nStepMemberID);

            if (GetResultData(strSQL, 0) == null)
                return false;

            component.ID = nComponentID;
            return true;
        }

        private bool AddEndPoint(Component component, int nStepMemberID)
        {
            int nComponentID = GetLastID("EndPoint") + 1;

            string strFormat = "insert into EndPoint (ID, x, y, width, height, text, ComponentID, isBegin, StepMemberID) values ";
            strFormat += "({0}, {1}, {2}, {3}, {4}, '{5}', '{6}', {7}, {8})";

            PropertyEndPoint prop = (PropertyEndPoint)component.Property;

            string strSQL = string.Format(strFormat,
                nComponentID, component.X, component.Y, component.Width, component.Height,
                component.Text, component.ComponentID, prop.IsBegin ? 1 : 0, nStepMemberID);

            if (GetResultData(strSQL, 0) == null)
                return false;

            component.ID = nComponentID;
            return true;
        }

        private bool AddAnnotation(Component component, int nStepMemberID)
        {
            int nComponentID = GetLastID("Annotation") + 1;

            string strFormat = "Insert into Annotation (ID, x, y, width, height, text, ComponentID, StepMemberID) values ";
            strFormat += "({0}, {1}, {2}, {3}, {4}, '{5}', '{6}', {7})";

            string strSQL = string.Format(strFormat,
                nComponentID, component.X, component.Y, component.Width, component.Height,
                component.Text, component.ComponentID, nStepMemberID);

            if (GetResultData(strSQL, 0) == null)
                return false;

            component.ID = nComponentID;
            return true;
        }

        private int GetLastID(string strTableName)
        {
            string strSQL = "select max(ID) from " + strTableName;
            ArrayList arrResult = GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return 0;

            return GetIntField(arrResult[0].ToString(), 0);
        }

        private bool InsertDisaster(int nChemistryID, int nAccidentID, int nMixedFactorID, int nWeatherID, double distance, string strDamage)
        {
            int nDisasterID = GetLastID("Disaster") + 1;

            string strFormat = "Insert into Disaster (ID, ChemistryID, AccidentID, MixedFactorID, WeatherID, SafetyDistance, EstimatedDamage, Description)";
            strFormat += " values ({0}, {1}, {2}, {3}, {4}, {5}, '{6}', NULL)";
            string strSQL = string.Format(strFormat,
                nDisasterID, nChemistryID, nAccidentID,
                nMixedFactorID < 0 ? "NULL" : nMixedFactorID.ToString(),
                nWeatherID,
                distance <= 0 ? "NULL" : ((int)(distance * 1000)).ToString(),
                strDamage);

            if (GetResultData(strSQL, 0) == null)
                return false;

            m_nDisasterID = nDisasterID;
            return true;
        }

        private int GetChemistryID(string strChemistry)
        {
            string strSQL = "select id from HarmfulChemistry where Name = '" + strChemistry + "'";
            ArrayList arrResult = GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            return GetIntField(arrResult[0].ToString(), -1);
        }

        private int GetAccidentID(string strAccident)
        {
            string strSQL = "select id from Accident where Name = '" + strAccident + "'";
            ArrayList arrResult = GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            return GetIntField(arrResult[0].ToString(), -1);
        }

        private int GetMixedFactorID(string strMixedFactor)
        {
            string strSQL = "select id from MixedFactor where Name = '" + strMixedFactor + "'";
            ArrayList arrResult = GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            return GetIntField(arrResult[0].ToString(), -1);
        }

        private int GetWeatherID(string strWeather, bool isDayLight)
        {
            string strCondition = "";

            if (strWeather == "비")
                strCondition = "Rain = 1 and StrongWind = 0";
            else if (strWeather == "맑음")
                strCondition = "Rain = 0 and StrongWind = 0";
            else if (strWeather == "강풍")
                strCondition = "Rain = 0 and StrongWind = 1";
            else
                return -1;

            if (isDayLight)
                strCondition += " and DayLight = 1";
            else
                strCondition += " and DayLight = 0";

            string strSQL = "select id from Weather where " + strCondition;
            ArrayList arrResult = GetResultData(strSQL, 0);

            if (arrResult == null || arrResult.Count == 0)
                return -1;

            return GetIntField(arrResult[0].ToString(), -1);
        }
    }
}
