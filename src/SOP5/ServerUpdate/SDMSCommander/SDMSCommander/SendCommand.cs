using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility;
using SDMSAgent;

namespace SDMSCommander
{
    public class SendCommand
    {
        public SendCommand() { }

        public bool Execute(WebDBManager dbMgr, CommandItem cmd)
        {
            if (cmd == null)
                return false;

            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO SDMSCommand (ID, Command, TimeStamp, SearchPath, IsStop, IsStopService, StopName, IsUpdate, UpdateName, IsStart, IsStartService, StartName) ");
            sb.AppendFormat("           VALUES ((select isnull(max(id)+1,1) from sdmscommand), {0}, '{1}', '{2}', {3}, {4}, '{5}', {6}, '{7}', {8}, {9}, '{10}')"
                , (int)cmd.CmdType
                , cmd.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss")
                , cmd.SearchPath
                , (cmd.IsStop) ? 1 : 0, (cmd.IsStopService) ? 1 : 0, cmd.StopName                
                , (cmd.IsUpdate) ? 1 : 0, cmd.UpdateName
                , (cmd.IsStart) ? 1 : 0, (cmd.IsStartService) ? 1 : 0, cmd.StartName);

            if (dbMgr.GetResultData(sb.ToString(), 0) == null)
                return false; 

            return true;
        }       
    }
}
