using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections;

namespace HSMS
{
    public class EditOptions : ChangedData
    {
        private AlarmManager.AlarmIgnoreOption m_optCar = AlarmManager.AlarmIgnoreOption.NONE;
        public AlarmManager.AlarmIgnoreOption OptCar
        {
            get { return m_optCar; }
            set { m_optCar = value; }
        }
        private AlarmManager.AlarmIgnoreOption m_optEquip = AlarmManager.AlarmIgnoreOption.NONE;
        public AlarmManager.AlarmIgnoreOption OptEquip
        {
            get { return m_optEquip; }
            set { m_optEquip = value; }
        }
        private AlarmManager.AlarmIgnoreOption m_optZone = AlarmManager.AlarmIgnoreOption.NONE;
        public AlarmManager.AlarmIgnoreOption OptZone
        {
            get { return m_optZone; }
            set { m_optZone = value; }
        }

        int m_nDistanceCar, m_nDayCar, m_nHourCar, m_nMinCar, m_nSecCar;
        int m_nDistanceEquip, m_nDayEquip, m_nHourEquip, m_nMinEquip, m_nSecEquip;
        int m_nDistanceZone, m_nDayZone, m_nHourZone, m_nMinZone, m_nSecZone;

        public int DistanceCar
        {
            get { return m_nDistanceCar; }
            set { m_nDistanceCar = value; }
        }
        public int DayCar
        {
            get { return m_nDayCar; }
            set { m_nDayCar = value; }
        }
        public int HourCar
        {
            get { return m_nHourCar; }
            set { m_nHourCar = value; }
        }
        public int MinCar
        {
            get { return m_nMinCar; }
            set { m_nMinCar = value; }
        }
        public int SecCar
        {
            get { return m_nSecCar; }
            set { m_nSecCar = value; }
        }
        public int DistanceEquip
        {
            get { return m_nDistanceEquip; }
            set { m_nDistanceEquip = value; }
        }
        public int DayEquip
        {
            get { return m_nDayEquip; }
            set { m_nDayEquip = value; }
        }
        public int HourEquip
        {
            get { return m_nHourEquip; }
            set { m_nHourEquip = value; }
        }
        public int MinEquip
        {
            get { return m_nMinEquip; }
            set { m_nMinEquip = value; }
        }
        public int SecEquip
        {
            get { return m_nSecEquip; }
            set { m_nSecEquip = value; }
        }

        public int DistanceZone
        {
            get { return m_nDistanceZone; }
            set { m_nDistanceZone = value; }
        }
        public int DayZone
        {
            get { return m_nDayZone; }
            set { m_nDayZone = value; }
        }

        public int HourZone
        {
            get { return m_nHourZone; }
            set { m_nHourZone = value; }
        }
        public int MinZone
        {
            get { return m_nMinZone; }
            set { m_nMinZone = value; }
        }
        public int SecZone
        {
            get { return m_nSecZone; }
            set { m_nSecZone = value; }
        }
        
        public override bool Update(DBConn conn)
        {
            ArrayList arrDatas = new ArrayList();
            CheckChangedData(ClientProvider.ObjectType.VEHICLE, m_nDistanceCar, m_nDayCar, m_nHourCar, m_nMinCar, m_nSecCar, arrDatas);
            CheckChangedData(ClientProvider.ObjectType.EQUIPMENT, m_nDistanceEquip, m_nDayEquip, m_nHourEquip, m_nMinEquip, m_nSecEquip, arrDatas);
            CheckChangedData(ClientProvider.ObjectType.ZONE, m_nDistanceZone, m_nDayZone, m_nHourZone, m_nMinZone, m_nSecZone, arrDatas);

            if (arrDatas.Count > 0)
            {
                arrDatas.Insert(0, (int)ChangeDataType.ALARM_IGNORE_OPTIONS);
                FormMain.Instance.NetMgr.ClientProvider.SendChangeAlarmIgnoreOptions(arrDatas);
                return true;
            }
            return false;
        }

        private void CheckChangedData(ClientProvider.ObjectType type, int nDistance, int nDay, int nHour, int nMin, int nSec, ArrayList arrDatas)
        {
            bool isChanged = false;
            AlarmManager alarmMgr = FormMain.Instance.AlarmManager;
            int nOption = 0, nTime = 0;

            if (type == ClientProvider.ObjectType.VEHICLE)
            {
                if (alarmMgr.IgnoreOptionCar != m_optCar)
                {
                    isChanged = true;
                }

                if (alarmMgr.IgnoreDistanceCar != nDistance)
                {
                    isChanged = true;
                }

                nTime = nDay * 24 * 3600 + nHour * 3600 + nMin * 60 + nSec;
                if (alarmMgr.IgnoreTimeCar != nTime)
                {
                    isChanged = true;
                }

                nOption = (int)m_optCar;
            }
            else if (type == ClientProvider.ObjectType.EQUIPMENT)
            {
                if (alarmMgr.IgnoreOptionEquip != m_optEquip)
                {
                    isChanged = true;
                }

                if (alarmMgr.IgnoreDistanceEquip != nDistance)
                {
                    isChanged = true;
                }

                nTime = nDay * 24 * 3600 + nHour * 3600 + nMin * 60 + nSec;
                if (alarmMgr.IgnoreTimeEquip != nTime)
                {
                    isChanged = true;
                }

                nOption = (int)m_optEquip;
            }
            else if (type == ClientProvider.ObjectType.ZONE)
            {
                if (alarmMgr.IgnoreOptionZone != m_optZone)
                {
                    isChanged = true;
                }

                if (alarmMgr.IgnoreDistanceZone != nDistance)
                {
                    isChanged = true;
                }

                nTime = nDay * 24 * 3600 + nHour * 3600 + nMin * 60 + nSec;

                if (alarmMgr.IgnoreTimeZone != nTime)
                {
                    isChanged = true;
                }

                nOption = (int)m_optZone;
            }

            if (isChanged)
            {
                arrDatas.Add((int)type);
                arrDatas.Add(nOption);
                arrDatas.Add(nDistance);
                arrDatas.Add(nTime);
            }
        }

        public override void AddToManager(IChangedDataManager mgr)
        {
            throw new NotImplementedException();
        }
    }
}
