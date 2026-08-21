using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using System.Collections;
using System.Data.SqlClient;
using HSMS;


namespace HSMSServer2
{
    public class EditAlarm : EditData
    {
        public static void ProcessAlarmIgnoreOptions(ConnectionState state, ArrayList arrDatas)
        {
            int nDataCount = arrDatas.Count;

            for (int i = 1; i < nDataCount; i += 4)
            {
                ProcessAlarmIgnoreOption(state, arrDatas, i);
            }
        }

        private static bool ProcessAlarmIgnoreOption(ConnectionState state, ArrayList arrDatas, int nIndex)
        {
            DataManager dataMgr = NetworkServer.Instance.DataManager;
            DBConn dbMgr = NetworkServer.Instance.DBManager;

            int nDataCount = arrDatas.Count;

            if (nIndex + 4 > nDataCount)
                return false;

            try
            {
                int nType = (int)arrDatas[nIndex];
                int nOption = (int)arrDatas[nIndex + 1];
                int nDistance = (int)arrDatas[nIndex + 2];
                int nTime = (int)arrDatas[nIndex + 3];

                if (nOption < (int)AlarmManager.AlarmIgnoreOption.NONE || nOption >= (int)AlarmManager.AlarmIgnoreOption.TYPE_COUNT)
                    return false;

                AlarmManager alarmMgr = NetworkServer.Instance.AlarmManager;

                if (nType == (int)NetworkClient.ObjectType.VEHICLE)
                {                   
                    if (alarmMgr.IgnoreOptionCar != (AlarmManager.AlarmIgnoreOption)nOption)
                    {
                        if (DBOptionHelper.UpdateOption(dbMgr, "AlarmIgnoreOptionCar", nOption.ToString()))
                        {
                            alarmMgr.IgnoreOptionCar = (AlarmManager.AlarmIgnoreOption)nOption;
                        }
                    }
                    
                    if (alarmMgr.IgnoreDistanceCar != nDistance)
                    {
                        if (DBOptionHelper.UpdateOption(dbMgr, "AlarmIgnoreDistanceCar", nDistance.ToString()))
                        {
                            alarmMgr.IgnoreDistanceCar = nDistance;
                        }
                    }
                    
                    if (alarmMgr.IgnoreTimeCar != nTime)
                    {
                        if (DBOptionHelper.UpdateOption(dbMgr, "AlarmIgnoreTimeCar", nTime.ToString()))
                        {
                            alarmMgr.IgnoreTimeCar = nDistance;                            
                        }
                    }
                }
                else if (nType == (int)NetworkClient.ObjectType.EQUIPMENT)
                { 
                    if (alarmMgr.IgnoreOptionEquip != (AlarmManager.AlarmIgnoreOption)nOption)
                    {
                        if (DBOptionHelper.UpdateOption(dbMgr, "AlarmIgnoreOptionEquip", nOption.ToString()))
                        {
                            alarmMgr.IgnoreOptionEquip = (AlarmManager.AlarmIgnoreOption)nOption;                           
                        }
                    }

                    if (alarmMgr.IgnoreDistanceEquip != nDistance)
                    {
                        if (DBOptionHelper.UpdateOption(dbMgr, "AlarmIgnoreDistanceEquip", nDistance.ToString()))
                        {
                            alarmMgr.IgnoreDistanceEquip = nDistance;                           
                        }
                    }

                    if (alarmMgr.IgnoreTimeEquip != nTime)
                    {
                        if (DBOptionHelper.UpdateOption(dbMgr, "AlarmIgnoreTimeEquip", nTime.ToString()))
                        {
                            alarmMgr.IgnoreTimeEquip = nDistance;                           
                        }
                    }
                }
                else if (nType == (int)NetworkClient.ObjectType.ZONE)
                {
                    if (alarmMgr.IgnoreOptionZone != (AlarmManager.AlarmIgnoreOption)nOption)
                    {
                        if (DBOptionHelper.UpdateOption(dbMgr, "AlarmIgnoreOptionZone", nOption.ToString()))
                        {
                            alarmMgr.IgnoreOptionZone = (AlarmManager.AlarmIgnoreOption)nOption;                            
                        }
                    }

                    if (alarmMgr.IgnoreDistanceZone != nDistance)
                    {
                        if (DBOptionHelper.UpdateOption(dbMgr, "AlarmIgnoreDistanceZone", nDistance.ToString()))
                        {
                            alarmMgr.IgnoreDistanceZone = nDistance;                           
                        }
                    }

                    if (alarmMgr.IgnoreTimeZone != nTime)
                    {
                        if (DBOptionHelper.UpdateOption(dbMgr, "AlarmIgnoreTimeZone", nTime.ToString()))
                        {
                            alarmMgr.IgnoreTimeZone = nDistance;                            
                        }
                    }
                }
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
