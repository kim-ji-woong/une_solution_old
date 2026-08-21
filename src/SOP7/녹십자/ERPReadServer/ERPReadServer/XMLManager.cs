using ERPReadServer.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ERPReadServer
{
    public class XMLManager
    {
        public DataTable ReadTeamXML(string strFilePath, out string strResultMessage)
        {
            DataTable dtTable = new DataTable();
            List<TeamInfoData> teamInfos = new List<TeamInfoData>();
            strResultMessage = "";

            bool stop = false;
            XmlTextReader reader = new XmlTextReader(strFilePath);

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "NewDataSet", true) == 0)
                        {
                            teamInfos = ReadTeamTables(reader);
                            reader.Close();

                            if (teamInfos == null)
                                return null;

                            dtTable = SetTeamTable(teamInfos);
                        }

                        PassElement(reader);
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            return dtTable;
        }

        private DataTable SetTeamTable(List<TeamInfoData> teamInfos)
        {
            DataTable dtTable = new DataTable();

            dtTable.Columns.Add("ORGEH");
            dtTable.Columns.Add("ORGTX");
            dtTable.Columns.Add("OWICH");
            dtTable.Columns.Add("UPORGEH");
            dtTable.Columns.Add("PARENT");
            dtTable.Columns.Add("CHILD");
            dtTable.Columns.Add("L_PERNR");
            dtTable.Columns.Add("KOSTL");
            dtTable.Columns.Add("OLEVEL");

            foreach (TeamInfoData data in teamInfos)
            {
                DataRow dr = dtTable.NewRow();

                dr[0] = data.ORGEH;
                dr[1] = data.ORGTX;
                dr[2] = data.OWICH;
                dr[3] = data.UPORGEH;
                dr[4] = data.PARENT;
                dr[5] = data.CHILD;
                dr[6] = data.L_PERNR;
                dr[7] = data.KOSTL;
                dr[8] = data.OLEVEL;

                dtTable.Rows.Add(dr);
            }

            return dtTable;
        }

        private List<TeamInfoData> ReadTeamTables(XmlTextReader reader)
        {
            List<TeamInfoData> teamInfos = null;

            try
            {
                if (reader.IsEmptyElement)
                    return teamInfos;

                bool stop = false;
                teamInfos = new List<TeamInfoData>();

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Table1", true) == 0)
                            {
                                TeamInfoData teamInfo = ReadTeamTable(reader);

                                if (teamInfo == null)
                                    return null;
                                else
                                    teamInfos.Add(teamInfo);
                            }
                            else
                                PassElement(reader);
                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                reader.Close();
                return null;
            }

            return teamInfos;
        }

        private TeamInfoData ReadTeamTable(XmlTextReader reader)
        {
            TeamInfoData teamInfo = null;

            try
            {
                if (reader.IsEmptyElement)
                    return teamInfo;

                teamInfo = new TeamInfoData();

                bool stop = false;
                string strORGEH = "";
                string strORGTX = "";
                string strOWICH = "";
                string strUPORGEH = "";
                string strPARENT = "";
                string strCHILD = "";
                string strL_PERNR = "";
                string strKOSTL = "";
                string strOLEVEL = "";

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "ORGEH", true) == 0)
                            {
                                if (reader.IsEmptyElement)
                                    continue;

                                if (ReadElementText(reader, ref strORGEH) == false)
                                {
                                    return null;
                                }
                                else
                                    teamInfo.ORGEH = strORGEH;
                            }
                            else if (string.Compare(reader.Name, "ORGTX", true) == 0)
                            {
                                if (reader.IsEmptyElement)
                                    continue;

                                if (ReadElementText(reader, ref strORGTX) == false)
                                {
                                    return null;
                                }
                                else
                                    teamInfo.ORGTX = strORGTX;
                            }
                            else if (string.Compare(reader.Name, "OWICH", true) == 0)
                            {
                                if (reader.IsEmptyElement)
                                    continue;

                                if (ReadElementText(reader, ref strOWICH) == false)
                                {
                                    return null;
                                }
                                else
                                    teamInfo.OWICH = strOWICH;
                            }
                            else if (string.Compare(reader.Name, "UPORGEH", true) == 0)
                            {
                                if (reader.IsEmptyElement)
                                    continue;

                                if (ReadElementText(reader, ref strUPORGEH) == false)
                                {
                                    return null;
                                }
                                else
                                    teamInfo.UPORGEH = strUPORGEH;
                            }
                            else if (string.Compare(reader.Name, "PARENT", true) == 0)
                            {
                                if (reader.IsEmptyElement)
                                    continue;

                                if (ReadElementText(reader, ref strPARENT) == false)
                                {
                                    return null;
                                }
                                else
                                    teamInfo.PARENT = strPARENT;
                            }
                            else if (string.Compare(reader.Name, "CHILD", true) == 0)
                            {
                                if (reader.IsEmptyElement)
                                    continue;

                                if (ReadElementText(reader, ref strCHILD) == false)
                                {
                                    return null;
                                }
                                else
                                    teamInfo.CHILD = strCHILD;
                            }
                            else if (string.Compare(reader.Name, "L_PERNR", true) == 0)
                            {
                                if (reader.IsEmptyElement)
                                    continue;

                                if (ReadElementText(reader, ref strL_PERNR) == false)
                                {
                                    return null;
                                }
                                else
                                    teamInfo.L_PERNR = strL_PERNR;
                            }
                            else if (string.Compare(reader.Name, "KOSTL", true) == 0)
                            {
                                if (reader.IsEmptyElement)
                                    continue;

                                if (ReadElementText(reader, ref strKOSTL) == false)
                                {
                                    return null;
                                }
                                else
                                    teamInfo.KOSTL = strKOSTL;
                            }
                            else if (string.Compare(reader.Name, "OLEVEL", true) == 0)
                            {
                                if (reader.IsEmptyElement)
                                    continue;

                                if (ReadElementText(reader, ref strOLEVEL) == false)
                                {
                                    return null;
                                }
                                else
                                    teamInfo.OLEVEL = strOLEVEL;
                            }
                            else
                                PassElement(reader);
                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

            }
            catch (Exception e)
            {
                reader.Close();
                return null;
            }

            return teamInfo;
        }

        public DataTable ReadMemberXML(string strFilePath, out string strResultMessage)
        {
            DataTable dtTable = new DataTable();
            List<MemberInfoData> memberInfos = new List<MemberInfoData>();
            strResultMessage = "";

            bool stop = false;
            XmlTextReader reader = new XmlTextReader(strFilePath);

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (string.Compare(reader.Name, "NewDataSet", true) == 0)
                        {
                            memberInfos = ReadMemberTables(reader);
                            reader.Close();

                            if (memberInfos == null)
                                return null;

                            dtTable = SetMemberTable(memberInfos);
                        }

                        PassElement(reader);
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            return dtTable;
        }

        private DataTable SetMemberTable(List<MemberInfoData> memberInfos)
        {
            DataTable dtTable = new DataTable();

            dtTable.Columns.Add("PERNR");
            dtTable.Columns.Add("ENAME");
            dtTable.Columns.Add("ORGEH");
            dtTable.Columns.Add("BTRTL");
            dtTable.Columns.Add("ZTITLE");
            dtTable.Columns.Add("ZGWID_NUM");
            dtTable.Columns.Add("BUKRS");
            dtTable.Columns.Add("ZHPON_NUM");
            dtTable.Columns.Add("ZOFFC_NUM");
            dtTable.Columns.Add("KOSTL");
            dtTable.Columns.Add("PERSK");
            dtTable.Columns.Add("GBDAT");
            dtTable.Columns.Add("BDATE");
            dtTable.Columns.Add("GESCH");
            dtTable.Columns.Add("FAMST");
            dtTable.Columns.Add("INDATA");
            dtTable.Columns.Add("BTEXT");


            foreach (MemberInfoData data in memberInfos)
            {
                DataRow dr = dtTable.NewRow();

                dr[0] = data.PERNR;
                dr[1] = data.ENAME;
                dr[2] = data.ORGEH;
                dr[3] = data.BTRTL;
                dr[4] = data.ZTITLE;
                dr[5] = data.ZGWID_NUM;
                dr[6] = data.BUKRS;
                dr[7] = data.ZHPON_NUM;
                dr[8] = data.ZOFFC_NUM;
                dr[9] = data.KOSTL;
                dr[10] = data.PERSK;
                dr[11] = data.GBDAT;
                dr[12] = data.BDATE;
                dr[13] = data.GESCH;
                dr[14] = data.FAMST;
                dr[15] = data.INDATA;
                dr[16] = data.BTEXT;

                dtTable.Rows.Add(dr);
            }

            return dtTable;
        }

        private List<MemberInfoData> ReadMemberTables(XmlTextReader reader)
        {
            List<MemberInfoData> memberInfos = new List<MemberInfoData>();

            try
            {
                if (reader.IsEmptyElement)
                    return memberInfos;

                bool stop = false;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "Table1", true) == 0)
                            {
                                MemberInfoData memberInfo = ReadMemberTable(reader);

                                if (memberInfo == null)
                                    return null;
                                else
                                    memberInfos.Add(memberInfo);
                            }
                            else
                                PassElement(reader);
                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }
            }
            catch (Exception e)
            {
                //m_strErrorMessage = e.Message;
                reader.Close();
                return null;
            }

            return memberInfos;
        }

        private MemberInfoData ReadMemberTable(XmlTextReader reader)
        {
            MemberInfoData memberInfo = null;

            try
            {
                if (reader.IsEmptyElement)
                    return memberInfo;

                memberInfo = new MemberInfoData();

                bool stop = false;
                string strPERNR = "";
                string strENAME = "";
                string strORGEH = "";
                string strBTRTL = "";
                string strPERSK = "";
                string strKOSTL = "";
                string strZDUTY = "";
                string strZRANK = "";
                string strZJKCOD = "";
                string strZJKCOT = "";
                string strZTITLE = "";
                string strTITEL = "";
                string strZGWID_NUM = "";
                string strBUKRS = "";
                string strZHPON_NUM = "";
                string strZOFFC_NUM = "";
                string strZCHIEF = "";
                string strGBDAT = "";
                string strBDATE = "";
                string strGESCH = "";
                string strFAMST = "";
                string strINDATA = "";
                string strBTEXT = "";

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (string.Compare(reader.Name, "PERNR", true) == 0)
                            {
                                if (reader.IsEmptyElement)
                                    continue;

                                if (ReadElementText(reader, ref strPERNR) == false)
                                {
                                    return null;
                                }
                                else
                                    memberInfo.PERNR = strPERNR;
                            }
                            else if (string.Compare(reader.Name, "ENAME", true) == 0)
                            {
                                if (reader.IsEmptyElement)
                                    continue;

                                if (ReadElementText(reader, ref strENAME) == false)
                                {
                                    return null;
                                }
                                else
                                    memberInfo.ENAME = strENAME;
                            }
                            else if (string.Compare(reader.Name, "ORGEH", true) == 0)
                            {
                                if (reader.IsEmptyElement)
                                    continue;

                                if (ReadElementText(reader, ref strORGEH) == false)
                                {
                                    return null;
                                }
                                else
                                    memberInfo.ORGEH = strORGEH;
                            }
                            else if (string.Compare(reader.Name, "BTRTL", true) == 0)
                            {
                                if (reader.IsEmptyElement)
                                    continue;

                                if (ReadElementText(reader, ref strBTRTL) == false)
                                {
                                    return null;
                                }
                                else
                                    memberInfo.BTRTL = strBTRTL;
                            }
                            else if (string.Compare(reader.Name, "PERSK", true) == 0)
                            {
                                if (reader.IsEmptyElement)
                                    continue;

                                if (ReadElementText(reader, ref strPERSK) == false)
                                {
                                    return null;
                                }
                                else
                                    memberInfo.PERSK = strPERSK;
                            }
                            else if (string.Compare(reader.Name, "KOSTL", true) == 0)
                            {
                                if (reader.IsEmptyElement)
                                    continue;

                                if (ReadElementText(reader, ref strKOSTL) == false)
                                {
                                    return null;
                                }
                                else
                                    memberInfo.KOSTL = strKOSTL;
                            }
                            else if (string.Compare(reader.Name, "ZDUTY", true) == 0)
                            {
                                if (reader.IsEmptyElement)
                                    continue;

                                if (ReadElementText(reader, ref strZDUTY) == false)
                                {
                                    return null;
                                }
                                else
                                    memberInfo.ZDUTY = strZDUTY;
                            }
                            else if (string.Compare(reader.Name, "ZRANK", true) == 0)
                            {
                                if (reader.IsEmptyElement)
                                    continue;

                                if (ReadElementText(reader, ref strZRANK) == false)
                                {
                                    return null;
                                }
                                else
                                    memberInfo.ZRANK = strZRANK;
                            }
                            else if (string.Compare(reader.Name, "ZJKCOD", true) == 0)
                            {
                                if (reader.IsEmptyElement)
                                    continue;

                                if (ReadElementText(reader, ref strZJKCOD) == false)
                                {
                                    return null;
                                }
                                else
                                    memberInfo.ZJKCOD = strZJKCOD;
                            }
                            else if (string.Compare(reader.Name, "ZJKCOT", true) == 0)
                            {
                                if (reader.IsEmptyElement)
                                    continue;

                                if (ReadElementText(reader, ref strZJKCOT) == false)
                                {
                                    return null;
                                }
                                else
                                    memberInfo.ZJKCOT = strZJKCOT;
                            }
                            else if (string.Compare(reader.Name, "ZTITLE", true) == 0)
                            {
                                if (reader.IsEmptyElement)
                                    continue;

                                if (ReadElementText(reader, ref strZTITLE) == false)
                                {
                                    return null;
                                }
                                else
                                    memberInfo.ZTITLE = strZTITLE;
                            }
                            else if (string.Compare(reader.Name, "TITEL", true) == 0)
                            {
                                if (reader.IsEmptyElement)
                                    continue;

                                if (ReadElementText(reader, ref strTITEL) == false)
                                {
                                    return null;
                                }
                                else
                                    memberInfo.TITEL = strTITEL;
                            }
                            else if (string.Compare(reader.Name, "ZGWID_NUM", true) == 0)
                            {
                                if (reader.IsEmptyElement)
                                    continue;

                                if (ReadElementText(reader, ref strZGWID_NUM) == false)
                                {
                                    return null;
                                }
                                else
                                    memberInfo.ZGWID_NUM = strZGWID_NUM;
                            }
                            else if (string.Compare(reader.Name, "BUKRS", true) == 0)
                            {
                                if (reader.IsEmptyElement)
                                    continue;

                                if (ReadElementText(reader, ref strBUKRS) == false)
                                {
                                    return null;
                                }
                                else
                                    memberInfo.BUKRS = strBUKRS;
                            }
                            else if (string.Compare(reader.Name, "ZHPON_NUM", true) == 0)
                            {
                                if (reader.IsEmptyElement)
                                    continue;

                                if (ReadElementText(reader, ref strZHPON_NUM) == false)
                                {
                                    return null;
                                }
                                else
                                    memberInfo.ZHPON_NUM = strZHPON_NUM;
                            }
                            else if (string.Compare(reader.Name, "ZOFFC_NUM", true) == 0)
                            {
                                if (reader.IsEmptyElement)
                                    continue;

                                if (ReadElementText(reader, ref strZOFFC_NUM) == false)
                                {
                                    return null;
                                }
                                else
                                    memberInfo.ZOFFC_NUM = strZOFFC_NUM;
                            }
                            else if (string.Compare(reader.Name, "ZCHIEF", true) == 0)
                            {
                                if (reader.IsEmptyElement)
                                    continue;

                                if (ReadElementText(reader, ref strZCHIEF) == false)
                                {
                                    return null;
                                }
                                else
                                    memberInfo.ZCHIEF = strZCHIEF;
                            }
                            else if (string.Compare(reader.Name, "GBDAT", true) == 0)
                            {
                                if (reader.IsEmptyElement)
                                    continue;

                                if (ReadElementText(reader, ref strGBDAT) == false)
                                {
                                    return null;
                                }
                                else
                                    memberInfo.GBDAT = strGBDAT;
                            }
                            else if (string.Compare(reader.Name, "BDATE", true) == 0)
                            {
                                if (reader.IsEmptyElement)
                                    continue;

                                if (ReadElementText(reader, ref strBDATE) == false)
                                {
                                    return null;
                                }
                                else
                                    memberInfo.BDATE = strBDATE;
                            }
                            else if (string.Compare(reader.Name, "GESCH", true) == 0)
                            {
                                if (reader.IsEmptyElement)
                                    continue;

                                if (ReadElementText(reader, ref strGESCH) == false)
                                {
                                    return null;
                                }
                                else
                                    memberInfo.GESCH = strGESCH;
                            }
                            else if (string.Compare(reader.Name, "FAMST", true) == 0)
                            {
                                if (reader.IsEmptyElement)
                                    continue;

                                if (ReadElementText(reader, ref strFAMST) == false)
                                {
                                    return null;
                                }
                                else
                                    memberInfo.FAMST = strFAMST;
                            }
                            else if (string.Compare(reader.Name, "INDATA", true) == 0)
                            {
                                if (reader.IsEmptyElement)
                                    continue;

                                if (ReadElementText(reader, ref strINDATA) == false)
                                {
                                    return null;
                                }
                                else
                                    memberInfo.INDATA = strINDATA;
                            }
                            else if (string.Compare(reader.Name, "BTEXT", true) == 0)
                            {
                                if (reader.IsEmptyElement)
                                    continue;

                                if (ReadElementText(reader, ref strBTEXT) == false)
                                {
                                    return null;
                                }
                                else
                                    memberInfo.BTEXT = strBTEXT;
                            }
                            else
                                PassElement(reader);
                            break;

                        case XmlNodeType.EndElement:
                            stop = true;
                            break;
                    }

                    if (stop)
                        break;
                }

            }
            catch (Exception e)
            {
                //m_strErrorMessage = e.Message;
                reader.Close();
                return null;
            }

            return memberInfo;
        }

        private bool ReadElementText(XmlTextReader reader, ref string strText)
        {
            bool stop = false;
            strText = "";

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Text:
                        strText = reader.Value;
                        break;

                    case XmlNodeType.EndElement:
                        stop = true;
                        break;
                }

                if (stop)
                    break;
            }

            return true;
        }

        private void PassElement(XmlTextReader reader)
        {
            if (reader.IsEmptyElement)
                return;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        PassElement(reader);
                        break;
                    case XmlNodeType.EndElement:
                        return;
                }
            }
        }
    }
}
