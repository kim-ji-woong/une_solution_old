using System;
using System.Collections.Generic;
using System.Collections;
using dnsDBUtil;

namespace WifiSensorService.Data
{
    using Request;
    using Response;

    public class AccountManager
    {
        public static ResponseManagerList GetManagerList(WebDBManager dbMgr)
        {
            string strSQL = "select id, name, type, note, pass from Manager";
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return new ResponseManagerList(false, dbMgr.LastErrorMessage);

            int nResultCount = arrResult.Count;
            ResponseManagerList response = new ResponseManagerList(true, "");

            for (int i=0;i<nResultCount-4;i+=5)
            {
                string strID = WebDBManager.GetStringField(arrResult[i].ToString());
                string strName = WebDBManager.GetStringField(arrResult[i + 1].ToString());
                string strType = WebDBManager.GetStringField(arrResult[i + 2].ToString());
                string strNote = WebDBManager.GetStringField(arrResult[i + 3].ToString());
                string strPass = WebDBManager.GetStringField(arrResult[i + 4].ToString());

                if (strID == null || strType == null || strPass == null)
                    continue;

                RequestCreateManager manager = new RequestCreateManager();

                manager.Id = strID;
                manager.Name = strName;
                manager.Type = strType;
                manager.Note = strNote;
                manager.Pass = strPass;

                response.ManagerList.Add(manager);
            }

            return response;
        }

        public static MessageResult CreateManager(WebDBManager dbMgr, RequestCreateManager data)
        {
            string strName = data.Name == null ? "NULL" : "'" + data.Name + "'";
            string strNote = data.Note == null ? "NULL" : "'" + data.Note + "'";
            string strSQL = string.Format("Insert into Manager (id, name, type, note, pass) values ('{0}', {1}, '{2}', {3}, '{4}')", data.Id, strName, data.Type, strNote, data.Pass);

            if (dbMgr.GetResultData(strSQL) == null)
                return new MessageResult(false, dbMgr.LastErrorMessage);

            return new MessageResult(true, "");
        }

        public static MessageResult UpdatePassword(WebDBManager dbMgr, RequestUpdatePassword data)
        {
            string strSQL = string.Format("Select pass from Manager where id = '{0}'", data.Id);
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return new MessageResult(false, dbMgr.LastErrorMessage);

            if (arrResult.Count == 0)
                return new MessageResult(false, string.Format("id {0}에 해당하는 계정정보가 존재하지 않습니다.", data.Id));

            string strPassword = WebDBManager.GetStringField(arrResult[0]);

            if (data.OldPass != strPassword)
                return new MessageResult(false, "oldPass가 일치하지 않습니다.");

            strSQL = string.Format("Update Manager set pass = '{0}' where id = '{1}'", data.NewPass, data.Id);

            if (dbMgr.GetResultData(strSQL) == null)
                return new MessageResult(false, dbMgr.LastErrorMessage);

            return new MessageResult(true, "");
        }

        public static MessageResult RemoveManager(WebDBManager dbMgr, RequestRemoveManager data)
        {
            string strSQL = string.Format("Delete from Manager where id = '{0}'", data.Id);
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null)
                return new MessageResult(false, dbMgr.LastErrorMessage);

            return new MessageResult(true, "");
        }
    }
}
