using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;

namespace libTTS
{
    class MessageManager
    {
        public List<BroadcastMessage> ReadMessage(WebDBManager dbMgr)
        {
            string strSQL = "SELECT Text,UseSiren,PlayOption,RepeatCount,AddTime from Broadcast";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            List<BroadcastMessage> messages = new List<BroadcastMessage>();

            int nResultCount = arrResult.Count;

            for (int i = 0; i < nResultCount - 4; i += 5)
            {
                string strMessage = WebDBManager.GetStringField(arrResult[0]);
                VariousData<int> useSiren = WebDBManager.GetIntField(arrResult[1].ToString());
                VariousData<int> playOption = WebDBManager.GetIntField(arrResult[2].ToString());
                VariousData<int> repeatCount = WebDBManager.GetIntField(arrResult[3].ToString());
                VariousData<DateTime> addTime = WebDBManager.GetDateTimeField(arrResult[4].ToString());

                if (strMessage == null || useSiren == null || playOption == null || repeatCount == null || addTime == null)
                    continue;

                if (playOption.Data != -1)
                {
                    BroadcastMessage data = new BroadcastMessage();

                    data.Message = strMessage;
                    data.UseSiren = useSiren.Data == 1;
                    data.PlayOption = (BroadcastMessage.MesageOption)playOption.Data;
                    data.RepeatCount = repeatCount.Data;
                    data.AddTime = addTime.Data;

                    messages.Add(data);
                }
            }

            ClearMessage(dbMgr);
            return messages;
        }

        private bool ClearMessage(WebDBManager dbMgr)
        {
            string strSQL = " DELETE from Broadcast";
            return dbMgr.GetResultData(strSQL) != null;
        }
    }
}
