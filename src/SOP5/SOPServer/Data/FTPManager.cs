using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DBUtility;

namespace SOPServer.Data
{
    public class FTPManager
    {
        private WebDBManager m_dbMgr = null;

        private string m_strLocalAddr = System.Windows.Forms.Application.StartupPath + @"\SOP3CCTV\ftp\";
        //private string m_strLocalAddr = @"C:\Users\Public\Pictures\Sample Pictures\";
        private string m_strFtpPath = "";

        private string m_strUserID = "ercctv";
        private string m_strPwd = "ercctv!2345";

        private string m_strFullPath = "";
        public string FullPath
        {
            get { return m_strFullPath; }
        }

        private string m_strFileName = "";
        public string FileName
        {
            get { return m_strFileName; }
        }

        /// <summary>
        /// 광교 누출
        /// </summary>
        public FTPManager()
        {
            m_dbMgr = new WebDBManager(0);
            //m_dbMgr.DatabaseHost = "210.182.116.102";
            m_dbMgr.DatabaseHost = "192.168.1.13";
            m_dbMgr.DatabasePort = "3306";
            m_dbMgr.DatabaseName = "eradams";

            m_dbMgr.DatabaseType = WebDBManager.DBType.mysql;
            m_dbMgr.WebServerURL = "http://127.0.0.1:8080/JUBIX";

            //m_strFtpPath = "ftp://210.182.116.102:40021/"; // 사내에서 test 할때
            m_strFtpPath = "ftp://192.168.1.13:40021/"; // 광교에 패치할때

            if (!Directory.Exists(m_strLocalAddr))
                Directory.CreateDirectory(m_strLocalAddr);
        }

        public void FindImage(SDMSServer.ServiceProvider provider, SDMSServer.SensorReactionLog log, string sensorName)
        {
            bool isSuc = GetFileName(sensorName);
            if (isSuc)
            {
                log.ImagePath = m_strFullPath;
                provider.SendSMS(log, SDMSServer.SMSManager.SMSMessageType.DETECT_PSM);
            }
            else // 1분동안 탐색
            {
                provider.SendSMS(log, SDMSServer.SMSManager.SMSMessageType.DETECT_PSM);

                isSuc = false;
                int nTimeout = 60;
                for (int i = 0; i < nTimeout; i++)
                {
                    isSuc = GetFileName(sensorName);

                    //test
                    //if (i == 10)
                    //    m_strFileName = "N_C1000_20180316050018_P1.jpg";

                    if (isSuc)
                        break;

                    System.Threading.Thread.Sleep(1000);
                }

                if (isSuc)
                {
                    log.ImagePath = m_strFullPath;
                    log.Message = " ";
                    provider.SendSMS(log, SDMSServer.SMSManager.SMSMessageType.DETECT_PSM);
                }
            }
        }

        public bool GetFileName(string sensorName)
        {
            this.m_strFullPath = "";

            if (sensorName.Contains('('))
            {
                int nIndex = sensorName.IndexOf('(');
                sensorName = sensorName.Substring(0, nIndex);
            }

            string strQuery = string.Format("select ss_id, ss_bigo from r_ss_dat where ss_id = (select ss_cctv_id from c_ss_info where ss_id='{0}') and ss_stat=22 order by ss_date desc limit 1", sensorName);

            ArrayList arrResult = m_dbMgr.GetResultData(strQuery, 0);
            if (arrResult == null || arrResult.Count != 2)
            {
                m_strFullPath = "";
                return false;
            }

            string strSSID = DBUtility.WebDBManager.GetStringField(arrResult[0]);
            string strSSBigo = DBUtility.WebDBManager.GetStringField(arrResult[1]);

            bool isSuc = FtpToLocal(strSSBigo);
            if (isSuc)
            {
                m_strFileName = strSSBigo;
                m_strFullPath = m_strLocalAddr + m_strFileName;
            }
            else
                m_strFullPath = "";

            return isSuc;
        }

        private bool isSearch = false;
        private bool FtpToLocal(string strFileName)
        {
            try
            {
                if (strFileName.Length == 0)
                    return false;

                FtpWebRequest req = (FtpWebRequest)WebRequest.Create(m_strFtpPath + strFileName);
                req.Credentials = new NetworkCredential(m_strUserID, m_strPwd);
                req.Method = WebRequestMethods.Ftp.DownloadFile;

                FtpWebResponse res = (FtpWebResponse)req.GetResponse();
                using (Stream stream = res.GetResponseStream())
                {
                    using (FileStream writeStream = new FileStream(m_strLocalAddr + strFileName, FileMode.Create, FileAccess.Write))
                    {
                        int length = 2048;
                        Byte[] buf = new Byte[length];
                        int bytesRead = stream.Read(buf, 0, length);

                        while (bytesRead > 0)
                        {
                            writeStream.Write(buf, 0, bytesRead);
                            bytesRead = stream.Read(buf, 0, length);
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
