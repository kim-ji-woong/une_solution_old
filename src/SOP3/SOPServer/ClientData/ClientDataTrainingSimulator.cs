using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpLib2;
using System.Collections;
using SDMS;
using System.Threading;

namespace SDMSServer
{
    public class ClientDataTrainingSimulator : ClientData
    {
        public ClientDataTrainingSimulator(ServiceProvider provider)
        {
            m_provider = provider;
            Type = ClientType.TRAINING_SIMULATOR;
        }

        // bytes는 length byte가 제거되었음
        protected override bool OnReceive(ConnectionState state, byte[] bytes, int nHeader, ArrayList arrDatas)
        {
            if (nHeader == TCP_ID.TRAINING_SIMULATOR_COMMAND)
            {
                ProcessCommand(state, bytes, arrDatas);
            }

            return true;
        }

        private void ProcessCommand(ConnectionState state, byte[] bytes, ArrayList arrDatas)
        {
            if (arrDatas.Count == 0 || !(arrDatas[0] is byte))
                return;

            byte command = (byte)arrDatas[0];

            switch (command)
            {
                case TrainingSimulatorCommandType.SEND_SDMS_SMS:
                    SendSDMSSMS(arrDatas);
                    break;
            }
        }

        private void SendSDMSSMS(ArrayList arrDatas)
        {
            if (arrDatas.Count < 5)
                return;

            if ((arrDatas[1] is int) == false)
                return;
            if ((arrDatas[2] is int) == false)
                return;
            if ((arrDatas[3] is int) == false)
                return;
            if ((arrDatas[4] is string) == false)
                return;

            int nSituationType = (int)arrDatas[1];
            int nEquipZoneID = (int)arrDatas[2];
            int nPhoneNumberCount = (int)arrDatas[3];
            string strTag = (string)arrDatas[4];

            string strMessage = "";

            if (nSituationType == (int)BroadcastManager.SituationType.DETECT_FIRE)
                strMessage = ClientDataSensorSimulator.GetFireDetectString(nEquipZoneID);
            else if (nSituationType == (int)BroadcastManager.SituationType.REPORT_FIRE)
                strMessage = ClientDataSDMS.GetFireReportString(nEquipZoneID, null);
            else
                return;

            if (nPhoneNumberCount <= 0 || nPhoneNumberCount != arrDatas.Count - 5)
                return;

            string szMsg = strTag + strMessage;
            // 발신자 번호 가져오기
            string szSendNum = m_provider.GetSendPhoneNumber();

            // 문자 메세지 보내기
            if (szMsg != "")
            {       
                // Send SMS
                // 메세지 전송 부분만 쓰레드로 처리해야 함. (중요), skkim 2014.12.16
                new Thread(() =>
                {
                    SMSManager.Instance.SendSMSForPhoneNumber(arrDatas, 5, szSendNum, szMsg);
                }).Start();

            }
        }
    }
}
