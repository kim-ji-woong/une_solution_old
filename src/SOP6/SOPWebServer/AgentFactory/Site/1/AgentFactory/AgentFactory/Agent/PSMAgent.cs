using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Collections;

namespace AgentFactory.Agent
{
    internal class PSMAgent : BaseAgent
    {
        public override MethodProcessType CheckMethod(MethodType type, params object[] args)
        {
            if (type == MethodType.OnReceive)
            {
                return CheckOnReceive(args);
            }

            return MethodProcessType.Default;
        }

        public override object RunMethod(MethodType type, params object[] args)
        {
            if (type == MethodType.OnReceive)
            {
                return OnReceiveData(args);
            }

            throw null;
        }

        private MethodProcessType CheckOnReceive(params object[] args)
        {
            if (args.Count() < 1)
                return MethodProcessType.Default;

            if (args[0] is int)
            {
                int nHeader = (int)args[0];

                if (nHeader == SOPWebServer.Header.SENSOR_USER_RESET)
                    return MethodProcessType.PreProcess;
            }

            return MethodProcessType.Default;
        }

        private int OnReceiveData(params object[] args)
        {
            if (args.Count() < 5)
                return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;

            if (args[0] is int && args[1] is OperationContext && args[2] is int && args[3] is byte[] && args[4] is ArrayList)
            {
                return OnReceive((int)args[0], (OperationContext)args[1], (int)args[2], (byte[])args[3], (ArrayList)args[4]);
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        private int OnReceive(int nClientSubType, OperationContext ctx, int nHeader, byte[] messages, ArrayList arrDatas)
        {
            if (nClientSubType == SOPWebServer.ClientSubType.SENKO)
            {
                if (nHeader == SOPWebServer.Header.SENSOR_USER_RESET)
                {
                    return ProcessUserReset(ctx, arrDatas);
                }
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        private int ProcessUserReset(OperationContext ctx, ArrayList arrDatas)
        {
            if (arrDatas.Count >= 4 && arrDatas[0] is int && arrDatas[1] is int && arrDatas[2] is int && arrDatas[3] is string)
            {
                int nSensorZoneHistoryID = (int)arrDatas[0];
                int nSensorZoneID = (int)arrDatas[1];
                int nSOPGenUserID = (int)arrDatas[2];
                string strDescription = (string)arrDatas[3];

                RequestPSMSensorReset(ctx, nSensorZoneID);
                return SOPWebServer.ErrorMessageType.SUCCESS;
            }

            return SOPWebServer.ErrorMessageType.INVALID_MESSAGE;
        }

        private void RequestPSMSensorReset(OperationContext ctx, int nSensosrZoneID)
        {
            ArrayList arrDatas = new ArrayList();
            arrDatas.Add(SOPWebServer.ServerCommandType.REQUEST_PSM_SENSOR_RESET);
            arrDatas.Add(nSensosrZoneID);

            byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
            arrDatas.Clear();

            // 다음번 Timer 이벤트에서 발송하도록 한다.
            arrDatas.Add(ctx);
            arrDatas.Add(SOPWebServer.Header.SERVER_COMMAND);
            arrDatas.Add(bytes);

            TimerDatas.Enqueue(arrDatas);
        }
    }
}
