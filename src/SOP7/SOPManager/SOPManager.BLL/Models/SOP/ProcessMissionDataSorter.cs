using System.Collections.Generic;
using SOPManager.Model.Sop.Component;

namespace SOPManager.BLL.Models.SOP
{
    public class ProcessMissionDataSorter
    {
        public static bool Sort(List<ProcessMissionData> missionDatas)
        {
            if (missionDatas == null)
                return true;

            int nMissionCount = missionDatas.Count;
            List<ProcessMissionData> tempList = new List<ProcessMissionData>();

            for (int i = 0; i < nMissionCount; i++)
            {
                tempList.Add(null);
            }

            // Key : OrderIndex
            Dictionary<int, ProcessMissionData> dicExternalMissionDatas = new Dictionary<int, ProcessMissionData>();
            List<ProcessMission> missionList = new List<ProcessMission>();

            int nIgnoreCount = 0;

            foreach (ProcessMissionData data in missionDatas)
            {
                if (data.MissionType == ProcessMissionData.MissionDataType.External)
                {
                    ProcessExternalMission externalMission = ToExternalMission(data);

                    if (externalMission == null)
                        return false;

                    int nIndex = externalMission.OrderIndex - 1;

                    if (nIndex < 0 || nIndex >= nMissionCount)
                        return false;

                    ProcessMissionData externalMissionData;

                    if (dicExternalMissionDatas.TryGetValue(nIndex, out externalMissionData) == false)
                    {
                        externalMissionData = new ProcessMissionData();

                        externalMissionData.MissionType = ProcessMissionData.MissionDataType.External;
                        externalMissionData.ProcessID = data.ProcessID;
                        externalMissionData.OrderIndex = data.OrderIndex;
                        externalMissionData.ProgramID = data.ProgramID;
                        externalMissionData.ProgramName = data.ProgramName;
                        externalMissionData.Parameters = new List<string>();

                        dicExternalMissionDatas[nIndex] = externalMissionData;

                        if (tempList[nIndex] != null)
                            return false;
                        else
                            tempList[nIndex] = externalMissionData;
                    }
                    else
                        nIgnoreCount++;

                    externalMissionData.Parameters.Add(externalMission.Value);
                }
                else if (data.MissionType == ProcessMissionData.MissionDataType.Normal)
                {
                    ProcessMission mission = ToMission(data);

                    if (mission == null)
                        return false;

                    missionList.Add(mission);
                }
            }

            missionList.Sort();
            nMissionCount -= nIgnoreCount;

            for (int i = 0; i < nMissionCount; i++)
            {
                ProcessMissionData data = tempList[i];

                if (data is null)
                {
                    if (missionList.Count > 0)
                    {
                        tempList[i] = ToMissionData(missionList[0]);
                        missionList.RemoveAt(0);
                    }
                    else
                        return false;
                }
            }

            if (missionList.Count > 0)
                return false;

            missionDatas.Clear();

            for (int i=0;i<nMissionCount;i++)
            {
                ProcessMissionData missionData = tempList[i];
                missionDatas.Add(missionData);
            }

            return true;
        }

        public static ProcessMissionData ToMissionData(ProcessMission mission)
        {
            ProcessMissionData data = new ProcessMissionData();

            data.MissionType = ProcessMissionData.MissionDataType.Normal;
            data.ID = mission.ID;
            data.ProcessID = mission.ProcessID;
            data.MissionText = mission.MissionText;

            return data;
        }

        public static ProcessMissionData ToMissionData(ProcessExternalMission externalMission)
        {
            ProcessMissionData data = new ProcessMissionData();

            data.MissionType = ProcessMissionData.MissionDataType.External;
            data.OrderIndex = externalMission.OrderIndex;
            data.ProcessID = externalMission.ParameterIndex;
            data.ProcessID = externalMission.ProcessID;
            data.ProgramID = externalMission.ProgramID;
            data.Value = externalMission.Value;

            return data;
        }

        public static ProcessExternalMission ToExternalMission(ProcessMissionData data)
        {
            if (data.OrderIndex == null || data.ProgramID == null)
                return null;

            ProcessExternalMission mission = new ProcessExternalMission();

            mission.ProcessID = data.ProcessID;
            mission.OrderIndex = (int)data.OrderIndex;
            mission.ProgramID = (int)data.ProgramID;
            mission.Value = data.Value;

            if (data.ParameterIndex != null)
                mission.ParameterIndex = (int)data.ParameterIndex;

            return mission;
        }

        public static ProcessMission ToMission(ProcessMissionData data)
        {
            if (data.ID == null || data.MissionText == null)
                return null;

            ProcessMission mission = new ProcessMission();

            mission.ID = (int)data.ID;
            mission.MissionText = data.MissionText;
            mission.ProcessID = data.ProcessID;

            return mission;
        }

        public static List<ProcessExternalMission> GetExternalMissions(ProcessMissionData data)
        {
            if (data.OrderIndex == null || data.ProgramID == null)
                return null;

            int nIndex = 0;
            List<ProcessExternalMission> externalMissions = new List<ProcessExternalMission>();

            foreach (string strParam in data.Parameters)
            {
                ProcessExternalMission externalMission = new ProcessExternalMission();

                externalMission.OrderIndex = (int)data.OrderIndex;
                externalMission.ProcessID = data.ProcessID;
                externalMission.ProgramID = (int)data.ProgramID;
                externalMission.ParameterIndex = nIndex++;
                externalMission.Value = strParam;

                externalMissions.Add(externalMission);
            }

            return externalMissions;
        }
    }
}
