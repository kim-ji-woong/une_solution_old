using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;
using System.Collections;
using SOP;

namespace ServerProcess.Data
{
    /// <summary>
    /// DB변경을 감시하여 클라이언트에 변경을 전송하는 class
    /// 현재는 조직관리툴에만 사용중이나 필요시 추가 함수를 작성하여 사용한다.
    /// </summary>
    public class SDMSConfigWatcher
    {
        private static SDMSConfigWatcher m_instance = new SDMSConfigWatcher();

        public static SDMSConfigWatcher Instance
        {
            get { return m_instance; }
        }

        private SDMSConfigWatcher()
        {
        }

        /// <summary>
        /// SDMSConfig는 조직관리툴에서 조직이 변경된 경우에, 0이 아닌값으로 셋팅 되므로
        /// 변경사항을 확인하여 클라이언트에 전송 후 0으로 변경하는 함수. 상시감시
        /// </summary>ㅣ
        public void Watch(DirectDBManager dbMgr)
        {
            string strTableName = "OptionSDMS", strPropertyName = "SDMSConfig";

            string strSQL = string.Format("Select PropertyValue from {0} where PropertyName = '{1}' and SiteID = {2}", strTableName, strPropertyName, dbMgr.SiteID);
            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count == 0)
                return;

            VariousData<int> config = WebDBManager.GetIntField(arrResult[0].ToString());

            if (config == null)
                return;

            if (config.Data == 0)
                return;

            strSQL = string.Format("Update {0} set PropertyValue = '0' where PropertyName = '{1}' and SiteID = {2}", strTableName, strPropertyName, dbMgr.SiteID);

            if (dbMgr.GetResultData(strSQL) == null)
                return;

            UpdateConfig(dbMgr, config.Data);
        }

        private void UpdateConfig(DirectDBManager dbMgr, int nConfigValue)
        {
            if (nConfigValue == 0)
                return;

            using (DdMonitor.Lock(MemberManager.Instance.MemberCriticalSection))
            {
                bool changeMembers = false;

                if (((nConfigValue & (int)SDMSConfig.ConfigType.COMPANY_MEMBER) == (int)SDMSConfig.ConfigType.COMPANY_MEMBER) ||
                    ((nConfigValue & (int)SDMSConfig.ConfigType.REGULAR_TEAM) == (int)SDMSConfig.ConfigType.REGULAR_TEAM))
                {
                    MemberManager.Instance.ReloadRegularMembers(dbMgr);
                    changeMembers = true;
                }

                if (((nConfigValue & (int)SDMSConfig.ConfigType.EXTERNAL_MEMBER) == (int)SDMSConfig.ConfigType.EXTERNAL_MEMBER) ||
                    ((nConfigValue & (int)SDMSConfig.ConfigType.EXTERNAL_TEAM) == (int)SDMSConfig.ConfigType.EXTERNAL_TEAM))
                {
                    MemberManager.Instance.ReloadExternalMembers(dbMgr);
                    changeMembers = true;
                }

                if (changeMembers)
                {
                    nConfigValue |= (int)SDMSConfig.ConfigType.EQUIPZONE_FACILITY_MANAGER | (int)SDMSConfig.ConfigType.BUILDING_FACILITY_MANAGER | (int)SDMSConfig.ConfigType.ENTIRE_FACILITY_MANAGER;
                    MemberManager.Instance.ReloadControlRoomTeams(dbMgr);
                    MemberManager.Instance.LoadFacilityManager(dbMgr);
                }

                // Data가 바뀌었으니 Client들에게 알려준다.
                Client.SDMSServer.Instance.SendChangedConfig(nConfigValue);
                SOPSimulatorManager.ServerInstance.SendChangedConfig(nConfigValue);
                //Client.SOPSimulatorServer.Instance.SendChangedConfig(nConfigValue);
                Client.SOPManagerServer.Instance.SendChangedConfig(nConfigValue);
            }
        }
    }
}
