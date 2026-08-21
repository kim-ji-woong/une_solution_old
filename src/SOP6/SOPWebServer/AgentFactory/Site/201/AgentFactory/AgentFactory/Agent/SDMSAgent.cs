using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;

namespace AgentFactory.Agent
{
    class SDMSAgent : BaseAgent
    {
        private const string FirstConnection = "ProcessFirstConnection";

        public override MethodProcessType CheckMethod(MethodType type, params object[] args)
        {
            if (type == MethodType.Etc)
            {
                int nArgumentCount = args.Count();

                if (nArgumentCount > 0 && args[0] != null)
                {
                    string strCommand = args[0].ToString();

                    if (strCommand == FirstConnection)
                        return MethodProcessType.PreProcess;
                }
            }

            return MethodProcessType.Default;
        }

        public override object RunMethod(MethodType type, params object[] args)
        {
            if (type == MethodType.Etc)
            {
                int nArgumentCount = args.Count();

                if (nArgumentCount > 0)
                {
                    string strCommand = args[0].ToString();

                    if (strCommand == FirstConnection)
                    {
                        if (nArgumentCount >= 4 && args[1] is DirectDBManager && args[2] is IBaseClient && args[3] is IAlarmManager)
                            return ProcessFirstConnection((DirectDBManager)args[1], (IBaseClient)args[2], (IAlarmManager)args[3]);
                    }
                }
            }

            return null;
        }

        private object ProcessFirstConnection(DirectDBManager dbMgr, IBaseClient client, IAlarmManager alarmManager)
        {
            List<AlarmData> alarms = alarmManager.CurrentAlarms;
            Dictionary<AlarmData, Alarm.AlarmManager.AlarmStep> dicAlarmSteps = new Dictionary<AlarmData, Alarm.AlarmManager.AlarmStep>();

            string strSensorZoneIDs = "";

            for (int i=alarms.Count -1;i>=0;i--)
            {
                AlarmData alarm = alarms[i];

                if (alarm.SensorType != UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR)
                {
                    alarms.RemoveAt(i);
                }
                else
                {
                    Alarm.AlarmManager.AlarmStep _step = Alarm.AlarmManager.GetAlarmStep(alarm);

                    if (_step != Alarm.AlarmManager.AlarmStep.None)
                    {
                        dicAlarmSteps[alarm] = _step;
                    }

                    if (strSensorZoneIDs.Length == 0)
                        strSensorZoneIDs = alarm.SensorZoneID.ToString();
                    else
                        strSensorZoneIDs += "," + alarm.SensorZoneID.ToString();
                }
            }

            Alarm.AlarmManager.AlarmStep step = Alarm.AlarmManager.AlarmStep.None;

            if (alarms.Count == dicAlarmSteps.Count)
            {
                if (alarms.Count == 0)
                    return null;

                if (dicAlarmSteps.TryGetValue(alarms[0], out step) == false)
                    return null;
            }
            else
            {
                step = Alarm.FireAlarmManager.GetAlarmStep(dbMgr, strSensorZoneIDs, alarms, -1);
            }

            if (step != Alarm.AlarmManager.AlarmStep.None)
            {
                ClientMessage message = Alarm.AlarmManager.MakeClientMessage(alarms, step, SOPWebServer.ClientType.SDMS, -1, false);
                client.SendClientData(message.Header, message.Bytes, message.ClientType, message.ClientSubType);
            }

            return null;
        }
    }
}
