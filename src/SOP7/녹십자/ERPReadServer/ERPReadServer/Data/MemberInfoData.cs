using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERPReadServer.Data
{
    public class MemberInfoData
    {
        private string m_PERNR = "";
        public string PERNR
        {
            get { return m_PERNR; }
            set { m_PERNR = value; }
        }

        private string m_ENAME = "";
        public string ENAME
        {
            get { return m_ENAME; }
            set { m_ENAME = value; }
        }

        private string m_ORGEH = "";
        public string ORGEH
        {
            get { return m_ORGEH; }
            set { m_ORGEH = value; }
        }

        private string m_BTRTL = "";
        public string BTRTL
        {
            get { return m_BTRTL; }
            set { m_BTRTL = value; }
        }

        private string m_PERSK = "";
        public string PERSK
        {
            get { return m_PERSK; }
            set { m_PERSK = value; }
        }

        private string m_KOSTL = "";
        public string KOSTL
        {
            get { return m_KOSTL; }
            set { m_KOSTL = value; }
        }

        private string m_ZDUTY = "";
        public string ZDUTY
        {
            get { return m_ZDUTY; }
            set { m_ZDUTY = value; }
        }

        private string m_ZRANK = "";
        public string ZRANK
        {
            get { return m_ZRANK; }
            set { m_ZRANK = value; }
        }

        private string m_ZJKCOD = "";
        public string ZJKCOD
        {
            get { return m_ZJKCOD; }
            set { m_ZJKCOD = value; }
        }

        private string m_ZJKCOT = "";
        public string ZJKCOT
        {
            get { return m_ZJKCOT; }
            set { m_ZJKCOT = value; }
        }

        private string m_ZTITLE = "";
        public string ZTITLE
        {
            get { return m_ZTITLE; }
            set { m_ZTITLE = value; }
        }

        private string m_TITEL = "";
        public string TITEL
        {
            get { return m_TITEL; }
            set { m_TITEL = value; }
        }

        private string m_ZGWID_NUM = "";
        public string ZGWID_NUM
        {
            get { return m_ZGWID_NUM; }
            set { m_ZGWID_NUM = value; }
        }

        private string m_BUKRS = "";
        public string BUKRS
        {
            get { return m_BUKRS; }
            set { m_BUKRS = value; }
        }

        private string m_ZHPON_NUM = "";
        public string ZHPON_NUM
        {
            get { return m_ZHPON_NUM; }
            set { m_ZHPON_NUM = value; }
        }

        private string m_ZOFFC_NUM = "";
        public string ZOFFC_NUM
        {
            get { return m_ZOFFC_NUM; }
            set { m_ZOFFC_NUM = value; }
        }

        private string m_ZCHIEF = "";
        public string ZCHIEF
        {
            get { return m_ZCHIEF; }
            set { m_ZCHIEF = value; }
        }

        private string m_GBDAT = "";
        public string GBDAT
        {
            get { return m_GBDAT; }
            set { m_GBDAT = value; }
        }

        private string m_BDATE = "";
        public string BDATE
        {
            get { return m_BDATE; }
            set { m_BDATE = value; }
        }

        private string m_GESCH = "";
        public string GESCH
        {
            get { return m_GESCH; }
            set { m_GESCH = value; }
        }

        private string m_FAMST = "";
        public string FAMST
        {
            get { return m_FAMST; }
            set { m_FAMST = value; }
        }

        private string m_INDATA = "";
        public string INDATA
        {
            get { return m_INDATA; }
            set { m_INDATA = value; }
        }

        private string m_BTEXT = "";
        public string BTEXT
        {
            get { return m_BTEXT; }
            set { m_BTEXT = value; }
        }
    }

    public class RegularMemberData : TeamEditor.Model.Sop.Team.RegularMember
    {
        bool m_bCheck = false;

        public bool Check
        {
            get { return m_bCheck; }
            set { m_bCheck = value; }
        }
    }
}
