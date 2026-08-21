using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;

namespace libSOPPolicy
{
    public class SOPUserFactory
    {
        /// <summary>
        /// 클라이언트용
        /// </summary>
        /// <param name="nSOPGenUserID"></param>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static BaseSOPUser CreateSOPUser(int nSOPGenUserID, WebDBManager dbMgr)
        {
            if (dbMgr != null)
            {
                if (dbMgr.SiteID == 201 || dbMgr.SiteID == 205)
                {
                    BaseSOPUser sopUser = new SOPUser_Parc1(dbMgr, nSOPGenUserID);
                    return sopUser;
                }
                else if (dbMgr.SiteID == 202)
                {
                    BaseSOPUser sopUser = new SOPUser_202(dbMgr, nSOPGenUserID);
                    return sopUser;
                }
            }

            BaseSOPUser user = new BaseSOPUser();
            user.ID = nSOPGenUserID;
            user.SiteID = dbMgr.SiteID;
            return user;
        }

        /// <summary>
        /// 서버용
        /// </summary>
        /// <param name="nSOPGenUserID"></param>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static BaseSOPUser CreateSOPUser(int nSOPGenUserID, DirectDBManager dbMgr)
        {
            if (dbMgr != null)
            {
                if (dbMgr.SiteID == 201 || dbMgr.SiteID == 205)
                {
                    BaseSOPUser sopUser = new SOPUser_Parc1(dbMgr, nSOPGenUserID);
                    return sopUser;
                }
                else if (dbMgr.SiteID == 202)
                {
                    BaseSOPUser sopUser = new SOPUser_202(dbMgr, nSOPGenUserID);
                    return sopUser;
                }
            }

            BaseSOPUser user = new BaseSOPUser();
            user.ID = nSOPGenUserID;
            user.SiteID = dbMgr.SiteID;
            return user;
        }
    }
}
