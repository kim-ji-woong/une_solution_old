using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility2;

namespace UnE.SOP
{
    public class SupervisorFactory
    {
        /// <summary>
        /// Client 용
        /// </summary>
        /// <param name="nSiteID"></param>
        /// <returns></returns>
        public static ISupervisor MakeInstance(int nSiteID)
        {
            if (nSiteID == 201)
                return new Site.Supervisor_Parc1();
            else if (nSiteID == 205)
                return new Site.Supervisor_Urbanbrix();

            return new Supervisor();
        }

        /// <summary>
        /// Server 용
        /// </summary>
        /// <param name="dbMgr"></param>
        /// <returns></returns>
        public static ISupervisor MakeInstance(DirectDBManager dbMgr)
        {
            if (dbMgr.SiteID == 201)
                return new Site.Supervisor_Parc1(dbMgr);
            else if (dbMgr.SiteID == 205)
                return new Site.Supervisor_Urbanbrix(dbMgr);

            return new Supervisor(dbMgr);
        }
    }
}
