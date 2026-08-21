using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamEditor.Command
{
    public abstract class CommandEx : UnE.Command.Command
    {
        // dir이 true이면 redo방향으로 DB 저장
        //       false이면 undo방향으로 DB 저장
        public abstract void SaveDB(DBUtility.WebDBManager dbMgr, bool dir);

        // TeamEditor에서 변경사항이 생겼음을 SOP Server에게 알린다.
        protected void UpdateConfig(DBUtility.WebDBManager dbMgr, SOP.SDMSConfig.ConfigType type)
        {
            // 모든 Command들의 SaveDB() 과정이 끝난후 한번만 호출되도록 하기 위하여
            // CommandManager에게 config 정보를 넘겨준다.
            FormMain.Instance.CommandManager.SetSDMSConfig((int)type);
        }

        // TeamEditor에서 변경사항이 생겼음을 SOP Server에게 알린다.
        /*protected bool UpdateConfig(DBUtility.WebDBManager dbMgr, SOP.SDMSConfig.ConfigType type)
        {
            string strTableName = "OptionSDMS", strPropertyName = "SDMSConfig";

            string strSQL = string.Format("Select PropertyValue from {0} where PropertyName = '{1}'", strTableName, strPropertyName);
            System.Collections.ArrayList arrResult = dbMgr.GetResultData(strSQL, 0);

            if (arrResult == null)
                return false;

            if (arrResult.Count == 0)
            {
                strSQL = string.Format("Insert into {0} (PropertyName, PropertyValue, Description, SiteID) values ('{1}', '{2}', NULL, {3})",
                    strTableName, strPropertyName, (int)type, FormMain.Instance.SiteID);

                arrResult = dbMgr.GetResultData(strSQL, 0);
            }
            else
            {
                int nConfig = 0;
                DBUtility.VariousData<int> config = DBUtility.WebDBManager.GetIntField(arrResult[0].ToString());

                if (config == null)
                    nConfig = (int)type;
                else
                    nConfig = config.Data | (int)type;

                strSQL = string.Format("Update {0} set PropertyValue = '{1}' where PropertyName = '{2}'", strTableName, nConfig, strPropertyName);
                arrResult = dbMgr.GetResultData(strSQL, 0);
            }

            return arrResult != null;
        }*/
    }
}
