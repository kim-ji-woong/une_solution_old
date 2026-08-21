using System.Collections.Generic;
using Common.Model.Option;
using Common.IDAL;
using System.Linq;
using SOPManager.BLL.Models.Response;
using SOPManager.Model.Sop.Account;
using SOPManager.BLL.Models.Request;

namespace SOPManager.BLL
{
    public class OptionManager
    {
        private const string AccountLevelDisasterTag = "UseAccountLevelDisaster";
        private const string AccountLevelDisasterItemsTag = "AccountLevelDisasterItems";

        private const string SOPSimulatorOptionTag = "OptionSOPSimulator";

        private SOPManager.IDAL.IDataManager m_sopDataManager = null;
        private Common.IDAL.IDataManager m_commonDataManager = null;
        private ProcessManager m_processManager = null;

        // 로그인한 사용자 계정에 따라 각기 사용할수 있는 SOP가 다른가?
        private bool m_useAccountLevelDisaster = false;
        // m_useAccountLevelDisaster가 true일때 SOP는 몇개의 타입으로 구분되는가?
        // Key : SOP Type 이름
        // Value : 해당 SOP Type을 사용하는 AccountLevel ID들
        private Dictionary<string, List<int>> m_dicLevelItems = new Dictionary<string, List<int>>();

        public bool UseAccountLevelDisaster
        {
            get { return m_useAccountLevelDisaster; }
        }

        public List<string> AccountLevelItemNames
        {
            get { return m_dicLevelItems.Keys.ToList(); }
        }

        public List<int> GetAccountLevelItemIDs(string strItemName)
        {
            List<int> results;

            if (m_dicLevelItems.TryGetValue(strItemName, out results))
                return results;

            return null;
        }

        public OptionManager(Common.IDAL.IDataManager commonDataManager, SOPManager.IDAL.IDataManager sopDataManager, ProcessManager processManager)
        {
            m_sopDataManager = sopDataManager;
            m_commonDataManager = commonDataManager;
            m_processManager = processManager;

            ReadAccountLevelDisaster();
        }

        private void ReadAccountLevelDisaster()
        {
            //if (m_commonDataManager == null)
            //    return;
            //
            //string strErrorMessage;
            //Options option = m_commonDataManager.GetSelectManager().SelectOption(SOPSimulatorOptionTag, AccountLevelDisasterTag, out strErrorMessage);
            //
            //if (option == null || option.PropertyValue == null)
            //    m_useAccountLevelDisaster = false;
            //else
            //{
            //    string strValue = option.PropertyValue.ToLower();
            //
            //    if (strValue == "0" || strValue == "false")
            //        m_useAccountLevelDisaster = false;
            //    else if (strValue == "1" || strValue == "true")
            //        m_useAccountLevelDisaster = true;
            //}
            //
            //if (m_useAccountLevelDisaster)
            //{
            //    ReadAccountLevelDisasterItems();
            //}
        }

        // 예) [시,도 SOP(0, 1)], [일반 SOP(2)]
        private void ReadAccountLevelDisasterItems()
        {
            //m_dicLevelItems.Clear();
            //
            //if (m_commonDataManager == null)
            //    return;
            //
            //string strErrorMessage;
            //Options option = m_commonDataManager.GetSelectManager().SelectOption(SOPSimulatorOptionTag, AccountLevelDisasterItemsTag, out strErrorMessage);
            //
            //if (option != null && option.PropertyValue != null)
            //{
            //    List<string> items = SplitLevelItems(option.PropertyValue);
            //
            //    List<int> ids;
            //    string strItemName;
            //
            //    foreach (string strItem in items)
            //    {
            //        if (ParseLevelItems(strItem, out strItemName, out ids) == false)
            //        {
            //            m_useAccountLevelDisaster = false;
            //            return;
            //        }
            //
            //        m_dicLevelItems[strItemName] = ids;
            //    }
            //}
        }

        private bool ParseLevelItems(string strValue, out string strItemName, out List<int> ids)
        {
            strItemName = null;
            ids = new List<int>();

            int nIndex2 = strValue.LastIndexOf(')');
            int nIndex1 = strValue.LastIndexOf('(');

            if (nIndex1 > 0 && nIndex2 > nIndex1)
            {
                string strIDs = strValue.Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1);
                string[] tokens = strIDs.Split(',');

                int nID;

                foreach (string strToken in tokens)
                {
                    if (int.TryParse(strToken.Trim(), out nID) == false)
                        return false;

                    ids.Add(nID);
                }

                strItemName = strValue.Substring(0, nIndex1).Trim();
                return true;
            }

            return false;
        }

        private List<string> SplitLevelItems(string str)
        {
            int nBeginIndex = -1, nEndIndex = -1;
            List<string> items = new List<string>();

            int len = str.Length;

            for (int i = 0; i < len; i++)
            {
                char ch = str[i];

                if (nBeginIndex < 0)
                {
                    if (ch == '[')
                        nBeginIndex = i;
                }
                else if (nEndIndex < 0)
                {
                    if (ch == ']')
                        nEndIndex = i;
                }
                else
                {
                    if (ch == ',')
                    {
                        string strItem = str.Substring(nBeginIndex + 1, nEndIndex - nBeginIndex - 1);
                        items.Add(strItem);

                        nBeginIndex = nEndIndex = -1;
                    }
                }
            }

            if (nBeginIndex >= 0 && nEndIndex > nBeginIndex)
            {
                string strItem = str.Substring(nBeginIndex + 1, nEndIndex - nBeginIndex - 1);
                items.Add(strItem);
            }

            return items;
        }
    }
}
