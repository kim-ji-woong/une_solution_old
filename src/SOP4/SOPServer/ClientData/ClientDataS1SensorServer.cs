﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TcpLib2;
using SDMS;
using System.Threading;
using UnE.Sensor;

namespace SDMSServer
{
    public class ClientDataS1SensorServer : ClientData
    {

        private object m_LockObj = new object();
        private ClientDataAsinFireMonitor mAsinProcessor = null;
        private ClientDataEMPollEventReciver mEmpollProcessor = null;
        private ClientDataS1AccessEventReciver mAccessProcessor = null;
        private ClientDataSVMSEventReciver mSVMSProcessor = null;
        private ClientDataPSMMonitor mPSMProcessor = null;
        private ClientDataS1SecomEventReceiver mSecomProcessor = null;

        public ClientDataS1SensorServer(ServiceProvider provider)
        {
            m_provider = provider;
            ClientType = TCP_CLIENT.S1_TEST_SENSOR_SERVER;

            mAsinProcessor = new ClientDataAsinFireMonitor(provider);
            mEmpollProcessor = new ClientDataEMPollEventReciver(provider);
            mAccessProcessor = new ClientDataS1AccessEventReciver(provider);
            mSVMSProcessor = new ClientDataSVMSEventReciver(provider);

            mPSMProcessor = new ClientDataPSMMonitor(provider);
            mSecomProcessor = new ClientDataS1SecomEventReceiver(provider);
        }

        // 다른 ClientData에서 수신된 Data를 이용하여 SVMS이벤트를 처리하는 경우에 사용함
        // 주의점 : 내부에서 Lock을 사용하므로 동일 Lock루틴중에 사용하면 Deadlock이 발생함
        public bool ProcessSensorData(ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
            return OnReceive(state, bytes, nHeader, arrDatas);
        }

        //private int m_nCurrentHeader = -1;
        // bytes는 length byte가 제거되었음
        // arrDatas : 0(SensorType, int), 1(SensorTagInfoID, int), 2(SensorZoneID, int), 3(SensorData, int)
        protected override bool OnReceive(ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
            PingCount = 0;
            bool bResult = true;
            if(nHeader == TCP_ID.TEST_SENSOR_DATA || nHeader == TCP_ID.SENSOR_DATA)
            {
                int nSensorType = -1;

                if (arrDatas.Count > 0 && arrDatas[0] is int)
                    nSensorType = (int)arrDatas[0];
                //int nSensorType = BitConverter.ToInt32(bytes, 11);
                //int nSensorZoneID = BitConverter.ToInt32(bytes, 20);
                //int nSensorData = BitConverter.ToInt32(bytes, 29);
                IFacility.FacilityType sensorType = IFacility.ToFacilityType(nSensorType);

                if (sensorType >= IFacility.FacilityType.FIRE_SENSOR  && sensorType < IFacility.FacilityType.PSM_SENSOR || 
                    (sensorType >= IFacility.FacilityType.FireSensor_TypeA && sensorType <= IFacility.FacilityType.FireSensor_MonitoringType))
                {
                    bResult = mAsinProcessor.ProcessSensorData(state, bytes, nHeader, arrDatas);
                }
                else if (sensorType == IFacility.FacilityType.ExternalAlarmBell)
                {
                    bResult = mEmpollProcessor.ProcessSensorData(state, bytes, nHeader, arrDatas);
                }
                else if (sensorType >= IFacility.FacilityType.Intrusion_S1 && sensorType <= IFacility.FacilityType.EmergencyBell_S1)
                {
                    mSVMSProcessor.ProcessSensorData(state, bytes, nHeader, arrDatas);
                    bResult = true;
                }
                else if (sensorType >= IFacility.FacilityType.GeneralIntrusionT1_S1 && sensorType <= IFacility.FacilityType.SynthesisAlertAbnormalityU8_S1)
                {
                    bResult = mAccessProcessor.ProcessSensorData(state, bytes, nHeader, arrDatas);
                }
                else if (sensorType == IFacility.FacilityType.PSM_SENSOR)
                {
                    bResult = mPSMProcessor.ProcessSensorData(state, bytes, nHeader, arrDatas);
                }
                else if (sensorType >= IFacility.FacilityType.SecomFire && sensorType <= IFacility.FacilityType.SecomWomenAlarmBell)
                {
                    bResult = mSecomProcessor.ProcessSensorData(state, bytes, nHeader, arrDatas);
                }
            }   
         
            else if( nHeader == TCP_ID.TEST_PSM_SENSOR_DATA || nHeader == TCP_ID.PSM_SENSOR_DATA)
            {
                int nSensorType = BitConverter.ToInt32(bytes, 11);
                //int nSensorZoneID = BitConverter.ToInt32(bytes, 20);
                //int nSensorData = BitConverter.ToInt32(bytes, 29);
                IFacility.FacilityType sensorType = IFacility.ToFacilityType(nSensorType);

                if (sensorType == IFacility.FacilityType.PSM_SENSOR)
                {
                    bResult = mPSMProcessor.ProcessSensorData(state, bytes, nHeader, arrDatas);
                }
            }
            return bResult;       
        }
    }
}

