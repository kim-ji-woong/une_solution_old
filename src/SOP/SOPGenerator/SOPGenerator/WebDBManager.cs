using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.SqlClient;
using System.Collections;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace SOPGen
{
    public class WebDBManager
    {
        protected StringFile m_StringFile = new StringFile();
        private FormMain m_Main = null;
        private Utility m_ini = new Utility();
        private string m_strWebServerURL = "";

        public WebDBManager(FormMain main)
        {
            m_Main = main;
            Loadini_ServerConnectionInfo();
        }

        public void Loadini_ServerConnectionInfo()
        {
            string strSection = "Server Connection Info";

            m_strWebServerURL = m_ini.getinivalue(strSection, "webserver_url");
        }

        public string GetReadDB(string strSQLQuery, int nTransaction)
        {
            string resResult = string.Empty;
            //string m_sourceUrl = "http://localhost:8088/SOP/Login.jsp";
            string sourceUrl = m_strWebServerURL + "/DBQuery.jsp";
            string postData = "SQLQuery=" + strSQLQuery + "&"+ "Transaction=" + nTransaction;

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(postData);

            HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(sourceUrl);
            wReq.Method = "POST";
            //wReq.UserAgent = "Mozilla/4.0";
            wReq.ContentType = "application/x-www-form-urlencoded";
            wReq.ContentLength = bytes.Length;
            //wReq.CookieContainer = new CookieContainer();

            try
            {
                using (Stream writeStream = wReq.GetRequestStream())
                {
                    writeStream.Write(bytes, 0, bytes.Length);
                }

                HttpWebResponse wRes = (HttpWebResponse)wReq.GetResponse();

                // http 내용 추출
                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, Encoding.Default);

                resResult = readerPost.ReadToEnd();
            }
            catch (System.Net.WebException e)
            {
                MessageBox.Show(e.Message);
                return "";
            }

            return resResult;
        }

        public ArrayList GetResultData(string strSQLQuery, int nTransaction)
        {
            ArrayList arrResult = new ArrayList();
            string resResult = GetReadDB(strSQLQuery, nTransaction);

            m_StringFile.SetData(resResult);

            string strResult = "";
            bool isResult = true;
            bool isBegin = false;

            while (isResult)
            {
                isResult = m_StringFile.ReadLine(ref strResult);

                if (isResult)
                {
                    if (strResult == "Begin Data")
                    {
                        isBegin = true;
                        continue;
                    }

                    if (strResult == "End Data")
                        break;

                    if (isBegin)
                    {
                        arrResult.Add(strResult);
                    }
                }

                /*string[] strTemp = strResult.Split('\r');

                if (strTemp[0] == "End Data")
                {
                    break;
                }

                if (strTemp[0] != "")
                    arrResult.Add(strTemp[0]);*/
            }

            return arrResult;
        }
        
        //////////////////////////////////////////////////////////////////////////
        // StoredProcedure
        public string GetStoredProcedure(string strSQLQuery, int nTransaction)
        {
            string resResult = string.Empty;
            //string sourceUrl = "http://localhost:8088/SOP/RunStoredProcedure.jsp";
            string sourceUrl = m_strWebServerURL + "/RunStoredProcedure.jsp";
            string postData = "SQLQuery=" + strSQLQuery + "&" + "Transaction=" + nTransaction;

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = encoding.GetBytes(postData);

            HttpWebRequest wReq = (HttpWebRequest)WebRequest.Create(sourceUrl);
            wReq.Method = "POST";
            //wReq.UserAgent = "Mozilla/4.0";
            wReq.ContentType = "application/x-www-form-urlencoded";
            wReq.ContentLength = bytes.Length;
            //wReq.CookieContainer = new CookieContainer();

            using (Stream writeStream = wReq.GetRequestStream())
            {
                writeStream.Write(bytes, 0, bytes.Length);
            }

            HttpWebResponse wRes = (HttpWebResponse)wReq.GetResponse();

            // http 내용 추출
            Stream respPostStream = wRes.GetResponseStream();
            StreamReader readerPost = new StreamReader(respPostStream, Encoding.Default);

            resResult = readerPost.ReadToEnd();

            return resResult;
        }

        public ArrayList GetStoredProcedureData(string strSQLQuery, int nTransaction)
        {
            ArrayList arrResult = new ArrayList();
            string resResult = GetStoredProcedure(strSQLQuery, nTransaction);

            m_StringFile.SetData(resResult);

            string strResult = "";
            bool isResult = true;
            bool isBegin = false;

            while (isResult)
            {
                isResult = m_StringFile.ReadLine(ref strResult);

                if (isResult)
                {
                    if (strResult == "Begin Data")
                    {
                        isBegin = true;
                        continue;
                    }
                    if (strResult == "End Data")
                        break;

                    if (isBegin)
                        arrResult.Add(strResult);
                }

                /*string[] strTemp = strResult.Split('\r');

                if (strTemp[0] == "End Data")
                {
                    break;
                }

                if (strTemp[0] != "")
                    arrResult.Add(strTemp[0]);*/
            }

            return arrResult;
        }

        //////////////////////////////////////////////////////////////////////////
        // UserID를 가져온다 (strID:아이디, strPassword:비밀번호, return=-1:입력한UserID가DB에존재하지않음, 1:guest,2:member,3:leader,4:admin)
        public int GetUserID(string strID, string strPassword)
        {
            int nLevel = -1;

            ArrayList arrUser = new ArrayList();
            ReadDB_TableUsers(ref arrUser);

            for (int nList = 0; nList < arrUser.Count; nList++)
            {
                Data_SOPGenUser dataUser = (Data_SOPGenUser)arrUser[nList];
                if (dataUser == null) continue;

                if (dataUser.UserID == strID)
                {
                    if (dataUser.Password == strPassword)
                    {
                        nLevel = dataUser.UserLevel;
                    }

                    break;
                }
            }

            return nLevel;
        }

        //////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////
        // ReadDB_
        public void ReadDB_TableUsers(ref ArrayList arrUser)
        {
            arrUser.Clear();

            //string strSQL = "SELECT * FROM SOPGenUser";
            string strSQL = "SELECT us.id, us.MemberID, cm.MemberName, us.UserLevel, cm.RegularTeamID, us.Password, us.UserID FROM SOPGenUser as us, CompanyMember as cm WHERE us.MemberID = cm.ID";
            ArrayList arrResult = GetResultData(strSQL, 0);

            for (int i = 0; i < arrResult.Count - 6; i = i + 7)
            {
                Data_SOPGenUser dataNew = new Data_SOPGenUser();
                dataNew.ID = GetIntField(arrResult[i].ToString(), 0);
                dataNew.MemberID = GetIntField(arrResult[i + 1].ToString(), 0);
                dataNew.UserName = GetStringField(arrResult[i + 2].ToString(), "");
                dataNew.UserLevel = GetIntField(arrResult[i + 3].ToString(), 0);
                dataNew.TeamID = GetIntField(arrResult[i + 4].ToString(), 0);
                dataNew.Password = GetStringField(arrResult[i + 5].ToString(), "");
                dataNew.UserID = GetStringField(arrResult[i + 6].ToString(), "");


                arrUser.Add(dataNew);
            }
        }

        public void ReadDB_TableDisasterCategory(ref ArrayList arrCategory)
        {
            arrCategory.Clear();

            string strSQL = "SELECT * FROM DisasterCategory";
            ArrayList arrResult = GetResultData(strSQL, 0);

            for (int i = 0; i < arrResult.Count - 1; i = i + 2)
            {
                Data_DispasterCategory dataNew = new Data_DispasterCategory();
                dataNew.ID = GetIntField(arrResult[i].ToString(), 0);
                dataNew.CategoryName = GetStringField(arrResult[i + 1].ToString(), "");

                arrCategory.Add(dataNew);
            }
        }

        public void ReadDB_TableRegularTeam(ref ArrayList arrTeamName)
        {
            arrTeamName.Clear();

            string strSQL = "SELECT * FROM RegularTeam";
            ArrayList arrResult = GetResultData(strSQL, 0);

            for (int i = 0; i < arrResult.Count - 2; i = i + 3)
            {
                Data_RegularTeam dataNew = new Data_RegularTeam();
                dataNew.ID = GetIntField(arrResult[i].ToString(), 0);
                dataNew.TeamName = GetStringField(arrResult[i + 1].ToString(), "");
                dataNew.ParentTeamID = GetIntField(arrResult[i + 2].ToString(), 0);

                arrTeamName.Add(dataNew);
            }
        }

        public void ReadDB_TableCompanyMember(ref ArrayList arrMember)
        {
            arrMember.Clear();

            string strSQL = "SELECT CompanyMember.ID, CompanyMember.MemberName, RegularTeam.ID, RegularTeam.TeamName FROM CompanyMember, RegularTeam WHERE CompanyMember.RegularTeamID = RegularTeam.ID";
            ArrayList arrResult = GetResultData(strSQL, 0);

            for (int i = 0; i < arrResult.Count - 3; i = i + 4)
            {
                Data_SearchMember dataNew = new Data_SearchMember();
                dataNew.MemberID = GetIntField(arrResult[i].ToString(), 0);
                dataNew.MemberName = GetStringField(arrResult[i + 1].ToString(), "");
                dataNew.TeamID = GetIntField(arrResult[i + 2].ToString(), 0);
                dataNew.TeamName = GetStringField(arrResult[i + 3].ToString(), "");

                arrMember.Add(dataNew);
            }
        }


        public void Execute(string strSQL, SqlTransaction transaction = null)
        {
//             SqlCommand cmd = new SqlCommand(strSQL, m_dbConnection);
//             if (transaction != null) cmd.Transaction = transaction;
//             cmd.ExecuteNonQuery();
        }

        private void RemoveCheckTask(ArrayList arrTaskID, int transaction)
        {
            foreach (int nTaskID in arrTaskID)
            {
                string strSQL = string.Format("delete from CheckTask where TaskID = {0}", nTaskID);
                GetResultData(strSQL, transaction);
                //Execute(strSQL, transaction);
            }
        }

        private void RemoveTaskReport(ArrayList arrTaskID, int transaction)
        {
            foreach (int nTaskID in arrTaskID)
            {
                string strSQL = string.Format("delete from TaskReport where TaskID = {0}", nTaskID);
                GetResultData(strSQL, transaction);
                //Execute(strSQL, transaction);
            }
        }

        private void RemoveTask(ArrayList arrMissionInfoID, int transaction)
        {
            ArrayList arrTaskID = new ArrayList();

            foreach (int nMissionInfoID in arrMissionInfoID)
            {
                string strSQL = "SELECT id FROM Task WHERE MissionInfoID = " + nMissionInfoID.ToString();

                ArrayList arrResult = GetResultData(strSQL, transaction);

                for (int i = 0; i < arrResult.Count; i++)
                {
                    int nID = GetIntField(arrResult[i].ToString(), 0);

                    arrTaskID.Add(nID);
                }
            }

            int nTaskCount = arrTaskID.Count;

            if (nTaskCount > 0)
            {
                RemoveCheckTask(arrTaskID, transaction);
                RemoveTaskReport(arrTaskID, transaction);

                string strCondition = "(";

                for (int i = 0; i < nTaskCount; i++)
                {
                    if (i == 0)
                        strCondition += arrTaskID[i].ToString();
                    else
                        strCondition += string.Format(", {0}", (int)arrTaskID[i]);

                    if (i == nTaskCount - 1)
                        strCondition += ")";
                }

                string strSQL = "delete from Task where ID in " + strCondition;
                GetResultData(strSQL, transaction);
            }
        }

        private void RemoveMissionInfo(ArrayList arrStepMemberID, int transaction)
        {
            int nStepMemberCount = arrStepMemberID.Count;
            if (nStepMemberCount == 0) return;

            string strCondition = "(";

            for (int i = 0; i < nStepMemberCount; i++)
            {
                if (i == 0)
                    strCondition += arrStepMemberID[i].ToString();
                else
                    strCondition += ", " + arrStepMemberID[i].ToString();

                if (i == nStepMemberCount - 1)
                    strCondition += ")";
            }

            string strSQL = "select id from MissionInfo where StepMemberID in " + strCondition;

            ArrayList arrResult = GetResultData(strSQL, transaction);

            ArrayList arrMissionInfo = new ArrayList();

            for (int i = 0; i < arrResult.Count; i++)
            {
                int nID = GetIntField(arrResult[i].ToString(), 0);

                arrMissionInfo.Add(nID);
            }

            RemoveTask(arrMissionInfo, transaction);

            strSQL = "delete from MissionInfo where StepMemberID in " + strCondition;
            GetResultData(strSQL, transaction);
        }

        private void RemoveStepMember(string strVersion, int transaction)
        {
            ArrayList arrStepMemberID = new ArrayList();
            string strSQL = "select id from StepMember where VersionID = " + strVersion;

            ArrayList arrResult = GetResultData(strSQL, 0);

            for (int i = 0; i < arrResult.Count; i++)
            {
                int nID = GetIntField(arrResult[i].ToString(), 0);

                arrStepMemberID.Add(nID);
            }

            if (arrStepMemberID.Count > 0)
            {
                //RemoveTask(arrStepMemberID, transaction);
                RemoveMissionInfo(arrStepMemberID, transaction);

                strSQL = "delete from StepMember where VersionID = " + strVersion;
                //Execute(strSQL, transaction);
                GetResultData(strSQL, transaction);
            }
        }

        private void RemoveSubDisaster(string strVersion, int transaction)
        {
            string strSQL = "delete from SubDisasterCategory where VersionID = " + strVersion;
            //Execute(strSQL, transaction);
            GetResultData(strSQL, transaction);
        }

        private void RemoveActionStep(string strVersion, int transaction)
        {
            string strSQL = "delete from ActionStep where VersionID = " + strVersion;
            //Execute(strSQL, transaction);
            GetResultData(strSQL, transaction);
        }

        private void RemoveVersionTable(string strVersion, int transaction)
        {
            string strSQL = "delete from Version where ID = " + strVersion;
            //Execute(strSQL, transaction);
            GetResultData(strSQL, transaction);
        }
        
        // removeDataOnly : true이면 Version 정보는 남기고 Data들만 모두 지운다.
        //                  false이면 Version 정보까지 모두 지운다.
        private void RemoveVersion(string strVersionName, bool removeDataOnly, int transaction)
        {
            // 대소문자 구분 : collate Korean_Wansung_CS_AS
            string strSQL = string.Format("select ID from Version where VersionName collate Korean_Wansung_CS_AS = '{0}'", strVersionName);

            ArrayList arrResult = GetResultData(strSQL, 0);
            if (arrResult == null || arrResult.Count == 0)
                return;

            string strVersion = GetStringField(arrResult[0].ToString(), "");
            
            RemoveStepMember(strVersion, transaction);
            RemoveActionStep(strVersion, transaction);
            RemoveSubDisaster(strVersion, transaction);

            if (!removeDataOnly)
                RemoveVersionTable(strVersion, transaction);
            else
                UpdateVersionTime(strVersion, transaction);
        }

        private void UpdateVersionTime(string strVersion, int transaction)
        {
            DateTime tNow = DateTime.Now;
            string strLastAccessTime = tNow.ToShortDateString() + string.Format(" {0:00}:{1:00}:{2:00}", tNow.Hour, tNow.Minute, tNow.Second);
            string strSQL = "Update Version Set LastAccessTime = '" + strLastAccessTime + "' where ID = " + strVersion;
            //Execute(strSQL, transaction);
            GetResultData(strSQL, transaction);
        }

        private bool GetSaveVersion(string strLoginID, int transaction, out VersionData newVersionData, out bool isNewVersion, out int nOwnerID)
        {
            //strNewVersionName = strNewVersionDescription = "";
            newVersionData = null;
            isNewVersion = false;
            nOwnerID = -1;

            string strSQL = "select VersionName, SOPGenUser.UserID, CreateTime, LastAccessTime, Description, Version.OwnerID from Version, SOPGenUser where Version.OwnerID = SOPGenUser.ID";

            ArrayList arrResult = GetResultData(strSQL, 0);

            string strVersionName = "";
            string strOwner = "";
            DateTime dtCreate, dtLastAccess, dtDefault = new DateTime();
            string strDescription = "";

            FormVersion frm = new FormVersion();
            bool isFirst = true;
            DateTime dtLatestCreate = new DateTime();

            for (int i = 0; i < arrResult.Count - 5; i=i+6)
            {
                strVersionName = GetStringField(arrResult[i].ToString(), "");
                strOwner = GetStringField(arrResult[i+1].ToString(), "");
                dtCreate = GetDateTimeField(arrResult[i + 2], dtDefault);
                dtLastAccess = GetDateTimeField(arrResult[i + 3], dtDefault);
                strDescription = GetStringField(arrResult[i + 4].ToString(), "");
            
                if (strOwner == strLoginID)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        dtLatestCreate = dtCreate;
                        frm.AddVersionData(strVersionName, strOwner, dtCreate, dtLastAccess, strDescription);
                        nOwnerID = GetIntField(arrResult[i+5].ToString(), 0);
                    }
                    else
                    {
                        if (dtLatestCreate < dtCreate)
                        {
                            dtLatestCreate = dtCreate;
                            frm.ClearVersionData();
                            frm.AddVersionData(strVersionName, strOwner, dtCreate, dtLastAccess, strDescription);
                        }
                    }
                }

                frm.AddAllVersions(strVersionName, strOwner, dtCreate, dtLastAccess, strDescription);
            }

            if (frm.ShowDialog() == DialogResult.OK)
            {
                //frm.GetNewVersion(out strNewVersionName, out strNewVersionDescription, out isNewVersion);
                newVersionData = frm.GetNewVersion(out isNewVersion);
                newVersionData.Owner = strLoginID;

                if (nOwnerID < 0)
                {
                    // 대소문자 구분 :  collate Korean_Wansung_CS_AS
                    strSQL = "select id from SOPGenUser where UserID collate Korean_Wansung_CS_AS = '" + strLoginID + "'";
                    arrResult = GetResultData(strSQL, 0);

                    if(arrResult != null)
                    {
                        nOwnerID = GetIntField(arrResult[0].ToString(), 0);
                    }
                }

                return true;
            }

            return false;
        }

        public bool SaveSOP(FormProcess frmProcess, string strLoginID)
        {
            this.m_Main.Cursor = Cursors.WaitCursor;

            int transaction = 1;

            VersionData newVersionData;
            int nOwnerID;
            bool isNewVersion;
            if (!GetSaveVersion(strLoginID, transaction, out newVersionData, out isNewVersion, out nOwnerID))
            {
                //transaction.Rollback();
                this.m_Main.Cursor = Cursors.Arrow;
                return false;
            }

            if (!isNewVersion)
                RemoveVersion(newVersionData.VersionName, true, transaction);

            Dictionary<TreeNode, SubDisasterCategoryData> dicSubDisaster;
            int nVersionID;

            if (!m_Main.GetPaneLayer().SaveSubDisasterCategory(newVersionData, isNewVersion, nOwnerID, transaction, out nVersionID, out dicSubDisaster))
            {
                //transaction.Rollback();
                this.m_Main.Cursor = Cursors.Arrow;
                return false;
            }

            if (!m_Main.GetProcess().SaveSectionData(dicSubDisaster, nVersionID, transaction))
            {
                //transaction.Rollback();
                this.m_Main.Cursor = Cursors.Arrow;
                return false;
            }

            //transaction.Commit();
            this.m_Main.Cursor = Cursors.Arrow;
            return true;
        }

        public bool LoadSOP(string strVersionName)
        {
            this.m_Main.Cursor = Cursors.WaitCursor;

            // 대소문자 구분 : collate Korean_Wansung_CS_AS
            string strSQL = string.Format("select id from Version where VersionName collate Korean_Wansung_CS_AS = '{0}'", strVersionName);

            ArrayList arrResult = GetResultData(strSQL, 0);

            int nVersionID = 0;

            if(arrResult != null)
            {
                nVersionID = GetIntField(arrResult[0].ToString(), 0);
            }
            
            Dictionary<TreeNode, SubDisasterCategoryData> dicSubDisaster;
            Dictionary<int, TreeNode> dicSubNode;

            if (!m_Main.GetPaneLayer().LoadSubDisaster(nVersionID, out dicSubDisaster, out dicSubNode))
            {
                this.m_Main.Cursor = Cursors.Arrow;
                return false;
            }

            if (!m_Main.GetProcess().LoadActionStep(nVersionID, dicSubDisaster, dicSubNode))
            {
                this.m_Main.Cursor = Cursors.Arrow;
                return false;
            }

            SelectFirstSOP(m_Main.GetPaneLayer(), dicSubDisaster);
            this.m_Main.Cursor = Cursors.Arrow;

            return true;
        }

        private void SelectFirstSOP(FormPaneLayer frm, Dictionary<TreeNode, SubDisasterCategoryData> dicSubDisaster)
        {
            if (dicSubDisaster.Count == 0)
                return;

            frm.ExpandAllTreeView();

            foreach (KeyValuePair<TreeNode, SubDisasterCategoryData> pair in dicSubDisaster)
            {
                TreeNode node = pair.Key;

                if (node.Nodes.Count > 0)
                {
                    frm.SelectItem(node.Nodes[0]);
                    frm.treeViewSOP_AfterSelect(null, null);
                    return;
                }
            }
        }

        //public void RunStoredProcedure(string strProcName, ArrayList arrFields, ArrayList arrValues, int transaction, out SqlDataReader reader)
        public void RunStoredProcedure(string strProcName, ArrayList arrFields, ArrayList arrValues, int transaction, out ArrayList arrResult)
        {
            //reader = null;
            arrResult = null;

            int nFieldCount = arrFields.Count;
            int nValueCount = arrValues.Count;
            if (nFieldCount != nValueCount) return;

            //string strSQL = "exec " + strProcName;
            string strSQL = strProcName;

            for (int i = 0; i < nValueCount; i++)
            {
                if (i == 0)
                    //strSQL += "(" + (string)arrValues[i];
                    strSQL += " " + (string)arrValues[i];
                else
                    strSQL += "," + (string)arrValues[i];

                //if (i == nValueCount - 1)
                //    strSQL += ")";
            }

            arrResult = GetStoredProcedureData(strSQL, transaction);

            //SqlCommand cmd = new SqlCommand(strProcName, m_dbConnection);
            //cmd.CommandType = CommandType.StoredProcedure;
            //if (transaction != null) cmd.Transaction = transaction;

            //for (int i = 0; i < nFieldCount; i++)
            //{
            //    cmd.Parameters.Add(new SqlParameter((string)arrFields[i], (string)arrValues[i]));
            //}
            
            ////reader = cmd.ExecuteReader();

            //arrResult = GetResultData(strProcName, arrFields[0].ToString(), arrValues[0].ToString(), transaction);
            

        }







        public ArrayList GetCompanyMemberName()
        {
            ArrayList arrMemberName = new ArrayList();
            ReadDB_TableCompanyMember(ref arrMemberName);

            return arrMemberName;
        }

        public ArrayList GetDisasterCategoryName()
        {
            ArrayList arrCategory = new ArrayList();
            ReadDB_TableDisasterCategory(ref arrCategory);

            return arrCategory;
        }

        public ArrayList GetRegularTeamName()
        {
            ArrayList arrTeamName = new ArrayList();
            ReadDB_TableRegularTeam(ref arrTeamName);

            return arrTeamName;
        }

        public T GetField<T>(object dataSrc, T dataDefault)
        {
            T result;

            try
            {
                result = (T)dataSrc;
            }
            catch (Exception)
            {
                result = dataDefault;
            }

            return result;
        }

        // 문자열 앞뒤의 빈문자들을 제거한다.
        public string GetStringField(object dataSrc, string strDefault)
        {
            string result;

            try
            {
                result = (string)dataSrc;
                result = result.TrimStart(new char[] { ' ', '\t', '\r', '\n' });
                result = result.TrimEnd(new char[] { ' ', '\t', '\r', '\n' });
            }
            catch (Exception)
            {
                result = strDefault;
            }

            return result;
        }

        public DateTime GetDateTimeField(object dataSrc, DateTime dtDefault)
        {
            DateTime result;

            try
            {
                result = Convert.ToDateTime(dataSrc);
            }
            catch (Exception)
            {
                result = dtDefault;
            }

            return result;
        }

        public int GetIntField(string dataSrc, int nDefault)
        {
            int result;

            try
            {
                result = int.Parse(dataSrc);
            }
            catch (Exception)
            {
                result = nDefault;
            }

            return result;
        }
    }
}
