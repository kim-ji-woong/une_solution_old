using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Collections.Concurrent;
using System.Xml.Linq;
using DBUtility2;
using System.Collections;

namespace TestWebService
{
    public partial class FormMain : Form
    {
        // ETRI
        //private const string SERVER_URL = "http://39.119.118.190:9001";
        // 노아
        private const string SERVER_URL = "http://61.105.196.70:10003";
        //private const string SERVER_URL = "http://61.105.196.70:9001";
        private const string XML_HEADER = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>";

        private Dictionary<string, string> m_strPOITable = new Dictionary<string, string>();

        public FormMain()
        {
            //DumpFile(@"C:\Users\kimjw\Downloads\abcFE0.poi", @"C:\temp\normal.txt");
            //DumpFile(@"C:\Users\kimjw\Downloads\abcFE0__.poi", @"C:\temp\big.txt");
            InitializeComponent();
            //JsonLogin();
            //XMLLogin();

            m_strPOITable["H1110"] = "아염소산염류";
            m_strPOITable["H1120"] = "염소산염류";
            m_strPOITable["H1130"] = "과염소산염류";
            m_strPOITable["H1140"] = "무기과산화물";
            m_strPOITable["H1150"] = "브롬산염류";
            m_strPOITable["H1160"] = "질산염류";
            m_strPOITable["H1170"] = "요오드산염류";
            m_strPOITable["H1180"] = "과망간산염류";
            m_strPOITable["H1190"] = "중크롬산염류";
            m_strPOITable["H11A0"] = "기타1";
            m_strPOITable["H11B0"] = "기타2";


            m_strPOITable["H1210"] = "황화린";
            m_strPOITable["H1220"] = "적린";
            m_strPOITable["H1230"] = "유황";
            m_strPOITable["H1240"] = "철분";
            m_strPOITable["H1250"] = "금속분";
            m_strPOITable["H1260"] = "마그네슘";
            m_strPOITable["H1270"] = "기타1";
            m_strPOITable["H1280"] = "기타2";
            m_strPOITable["H1290"] = "인화성고체";


            m_strPOITable["H1310"] = "칼륨";
            m_strPOITable["H1320"] = "나트륨";
            m_strPOITable["H1330"] = "알킬알루미늄";
            m_strPOITable["H1340"] = "알킬리튬";
            m_strPOITable["H1350"] = "황린";
            m_strPOITable["H1360"] = "알칼리금속(칼륨 및 나트륨을 제외) 및 알칼리토금속";
            m_strPOITable["H1370"] = "유기금속화합물(알킬알루미늄 및 알킬리튬을 제외)";
            m_strPOITable["H1380"] = "금속의 수소화물";
            m_strPOITable["H1390"] = "금속의 인화물";
            m_strPOITable["H13A0"] = "칼슘 또는 알루미늄의 탄화물";
            m_strPOITable["H13B0"] = "기타1";
            m_strPOITable["H13C0"] = "기타2";


            m_strPOITable["H1410"] = "특수인화물";
            m_strPOITable["H1420"] = "제1석유류(비수용성액체)";
            m_strPOITable["H1430"] = "제1석유류(수용성액체)";
            m_strPOITable["H1440"] = "알코올류";
            m_strPOITable["H1450"] = "제2석유류(비수용성액체)";
            m_strPOITable["H1460"] = "제2석유류(수용성액체)";
            m_strPOITable["H1470"] = "제3석유류(비수용성액체)";
            m_strPOITable["H1480"] = "제3석유류(수용성액체)";
            m_strPOITable["H1490"] = "제4석유류";
            m_strPOITable["H14A0"] = "동식물유류";


            m_strPOITable["H1510"] = "유기과산화물";
            m_strPOITable["H1520"] = "질산에스테르류";
            m_strPOITable["H1530"] = "니트로화합물";
            m_strPOITable["H1540"] = "니트로소화합물";
            m_strPOITable["H1550"] = "아조화합물";
            m_strPOITable["H1560"] = "디아조화합물";
            m_strPOITable["H1570"] = "히드라진 유도체";
            m_strPOITable["H1580"] = "히드록실아민";
            m_strPOITable["H1590"] = "히드록실아민염류";
            m_strPOITable["H15A0"] = "기타1";
            m_strPOITable["H15B0"] = "기타2";


            m_strPOITable["H1610"] = "과염소산";
            m_strPOITable["H1620"] = "과산화수소";
            m_strPOITable["H1630"] = "질산";
            m_strPOITable["H1640"] = "기타1";
            m_strPOITable["H1650"] = "기타2";


            m_strPOITable["H2100"] = "고압가스";
            m_strPOITable["H2200"] = "액화석유가스(LPG)";
            m_strPOITable["H2300"] = "도시가스";


            m_strPOITable["H3100"] = "면화류";
            m_strPOITable["H3200"] = "나무껍질 및 대팻밥";
            m_strPOITable["H3300"] = "넝마 및 종이부스러기";
            m_strPOITable["H3400"] = "사류(絲類)";
            m_strPOITable["H3500"] = "볏짚류";
            m_strPOITable["H3600"] = "가연성고체류";
            m_strPOITable["H3700"] = "석탄 목탄류";
            m_strPOITable["H3800"] = "가연성액체류";
            m_strPOITable["H3900"] = "목재가공품 및 나무부스러기";
            m_strPOITable["H3A00"] = "합성수지류(발포시킨 것)";
            m_strPOITable["H3B00"] = "합성수지류(그 밖의 것)";

        }

        private void DumpFile(string strFilePath, string strFileName)
        {
            StreamWriter writer = new StreamWriter(strFileName, false, System.Text.Encoding.UTF8);

            long nFileSize = (new FileInfo(strFilePath)).Length;
            FileStream fileStream = new FileStream(strFilePath, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[nFileSize];
            int bytesRead = 0;
            bool first = true;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                for (int i=0;i<bytesRead;i++)
                {
                    byte b = buffer[i];

                    if (first)
                        writer.Write("{0:x2}", (int)b);
                    else
                        writer.Write(" {0:x2}", (int)b);

                    first = false;
                }

            }

            fileStream.Close();
            writer.Close();
        }

        private void JsonLogin()
        {
            string resResult = string.Empty;
            string strURL = SERVER_URL + "/edesignApi/login/json";

            string strJson = "{\"user_id\":\"userSI\",\"user_pwd\":\"1q2w3e4r\"}";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(strURL));
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = strJson.Length;

            try
            {
                StreamWriter writer = new StreamWriter(request.GetRequestStream());
                writer.Write(strJson);
                writer.Close();

                HttpWebResponse wRes = (HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                resResult = readerPost.ReadToEnd();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();
                System.Diagnostics.Trace.WriteLine("Success : " + resResult);
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Trace.WriteLine("Fail : " + ex.Message);
            }
        }

        private string XMLLogin()
        {
            string resResult = string.Empty;
            string strURL = SERVER_URL + "/sdesignApi/login";

            string strXML = XML_HEADER;
            strXML += "<login>";
            strXML += "<user_id>user_spatial</user_id>";
            strXML += "<user_pwd>spatial1234</user_pwd>";
            strXML += "</login>";

            byte[] bytes = Encoding.UTF8.GetBytes(strXML);
            int len = bytes.Count();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(strURL));
            request.Method = "POST";
            request.ContentType = "application/xml; charset=utf-8";
            request.ContentLength = len + 3;
            //request.ContentLength = strXML.Length;

            try
            {
                StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.UTF8);
                writer.Write(strXML);
                writer.Close();

                HttpWebResponse wRes = (HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                resResult = readerPost.ReadToEnd();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();
                System.Diagnostics.Trace.WriteLine("Success : " + resResult);

                return resResult;
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Trace.WriteLine("Fail : " + ex.Message);
            }

            return "";
        }

        private string SendQuery(string strXML, string strURL, out string strErrorMessage)
        {
            strErrorMessage = "";
            string url = SERVER_URL + "/sdesignApi";

            if (strURL.StartsWith("/"))
                url += strURL;
            else
                url += "/" + strURL;

            strXML = XML_HEADER + strXML;

            byte[] bytes = Encoding.UTF8.GetBytes(strXML);
            int len = bytes.Count();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.Method = "POST";
            request.ContentType = "application/xml; charset=utf-8";
            request.ContentLength = len + 3;
            //request.ContentLength = strXML.Length;

            string strResult = "";

            try
            {
                StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.UTF8);
                writer.Write(strXML);
                writer.Close();

                HttpWebResponse wRes = (HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                strResult = readerPost.ReadToEnd().Trim();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();
                
                if (strResult.StartsWith("<") == false)
                {
                    strErrorMessage = strResult;
                    return "";
                }

                XElement xml = XElement.Parse(strResult);
                XElement code = FindElement(xml, "rsCode");

                if (code == null)
                {
                    strErrorMessage = strResult;
                    return "";
                }
                else if (code.Value == "RS101")
                {
                    return strResult;
                }
                else if (code.Value == "RS401")
                {
                    strErrorMessage = "사용자 없음";
                    return "";
                }
                else if (code.Value == "RS402")
                {
                    strErrorMessage = "사용자 삭제";
                    return "";
                }
                else
                {
                    strErrorMessage = strResult;
                }
            }
            catch (System.Net.WebException ex)
            {
                strErrorMessage =  ex.Message;
            }

            return "";
        }

        private string RemoveXMLHeader(string strXML)
        {
            if (strXML.StartsWith("<?"))
            {
                int nIndex = strXML.IndexOf("?>");

                if (nIndex < 0)
                    return strXML;

                return strXML.Substring(nIndex + 2);
            }

            return strXML;
        }

        private XElement FindElement(XElement node, string strNodeName)
        {
            if (node.Name == strNodeName)
                return node;

            foreach (XElement element in node.Elements())
            {
                XElement _element = FindElement(element, strNodeName);

                if (_element != null)
                    return _element;
            }

            return null;
        }

        private bool CheckEmpty(string str, TextBox textBox, string strTag)
        {
            if (str.Length == 0)
            {
                textBox.Focus();
                MessageBox.Show(strTag + " 입력하세요");
                return false;
            }

            return true;
        }

        private bool CheckPassword(string strPassword)
        {
            for (int i = 0; i < strPassword.Length; i++)
            {
                char ch = strPassword.ElementAt(i);

                if ((ch >= '0' && ch <= '9') ||
                    (ch >= 'a' && ch <= 'z') ||
                    (ch >= 'A' && ch <= 'Z'))
                    continue;

                textBoxPW.Focus();
                MessageBox.Show("PW는 영어와 숫자를 사용하여 8에서 16 사이의 길이를 가져야만 합니다.");
                return false;
            }

            if (strPassword.Length < 8 || strPassword.Length > 16)
            {
                textBoxPW.Focus();
                MessageBox.Show("PW는 영어와 숫자를 사용하여 8에서 16 사이의 길이를 가져야만 합니다.");
                return false;
            }

            return true;
        }

        private bool CheckPhoneNumber(ref string strPhoneNumber)
        {
            string str = "";

            for (int i=0;i<strPhoneNumber.Length;i++)
            {
                char ch = strPhoneNumber.ElementAt(i);

                if (ch >= '0' && ch <= '9')
                    str += ch;
            }

            if (str.Length == 10)
            {
                strPhoneNumber = str.Substring(0, 3) + "-" + str.Substring(3, 3) + "-" + str.Substring(6);
            }
            else if (str.Length == 11)
            {
                strPhoneNumber = str.Substring(0, 3) + "-" + str.Substring(3, 4) + "-" + str.Substring(7);
            }
            else
            {
                textBoxPhoneNumber.Focus();
                MessageBox.Show("휴대전화번호가 형식에 맞지 않습니다.");
                return false;
            }

            return true;
        }

        private bool CheckBirth(string strBirth)
        {
            if (strBirth.Length != 8)
            {
                textBoxBirth.Focus();
                MessageBox.Show("생년월일은 숫자로 된 8자리여야 합니다");
                return false;
            }

            for (int i = 0; i < strBirth.Length; i++)
            {
                char ch = strBirth.ElementAt(i);

                if (ch < '0' || ch > '9')
                {
                    textBoxBirth.Focus();
                    MessageBox.Show("생년월일은 숫자로 된 8자리여야 합니다");
                    return false;
                }
            }

            return true;
        }

        private void btnRegist_Click(object sender, EventArgs e)
        {
            string strName = textBoxName.Text.Trim();
            string strPassword = textBoxPW.Text.Trim();
            string strPhoneNumber = textBoxPhoneNumber.Text.Trim();
            string strBirth = textBoxBirth.Text.Trim();

            if (CheckEmpty(strName, textBoxName, "이름을") == false)
                return;
            if (CheckEmpty(strPassword, textBoxPW, "비밀번호를") == false)
                return;
            if (CheckEmpty(strPhoneNumber, textBoxPhoneNumber, "전화번호를") == false)
                return;
            if (CheckEmpty(strBirth, textBoxBirth, "생년월일을") == false)
                return;

            if (CheckPassword(strPassword) == false)
                return;

            if (CheckPhoneNumber(ref strPhoneNumber) == false)
                return;

            if (CheckBirth(strBirth) == false)
                return;

            string strXML = XMLLogin().Trim();

            if (strXML.Length == 0)
                return;

            if (strXML.StartsWith("<") == false)
                return;

            XElement xml = XElement.Parse(strXML);

            XElement id = FindElement(xml, "user_id");
            XElement key = FindElement(xml, "key_id");
            XElement code = FindElement(xml, "rsCode");

            if (id == null || key == null || code == null)
                return;

            if (code.Value != "RS101")
                return;

            string strID = id.Value;
            string strKey = key.Value;

            string strNewID, strNewKey;
            SendRegist(strID, strKey, strPassword, strName, strPhoneNumber, strBirth, out strNewID, out strNewKey);
        }

        private bool SendRegist(string strID, string strKey, string strPassword, string strName, string strPhoneNumber, string strBirth, out string strNewID, out string strNewKey)
        {
            strNewID = strNewKey = "";

            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xUserPW = MakeElement("user_pwd", strPassword);
            XElement xUserName = MakeElement("user_name", strName);
            XElement xUserPhone = MakeElement("user_phone", strPhoneNumber);
            XElement xUserEmail = MakeElement("user_email", "");
            XElement xUserMajor = MakeElement("user_major", "유엔이");
            XElement xUserDept = MakeElement("user_dept", "");
            XElement xUserBirth = MakeElement("user_birth", strBirth);
            XElement xUserRoles = MakeElement("user_roles", "ROLE_101");

            XElement xUser = new XElement("user");
            xUser.Add(xUserPW);
            xUser.Add(xUserName);
            xUser.Add(xUserPhone);
            xUser.Add(xUserEmail);
            xUser.Add(xUserMajor);
            xUser.Add(xUserDept);
            xUser.Add(xUserBirth);
            xUser.Add(xUserRoles);

            XElement xCommon = new XElement("common");
            xCommon.Add(xState);
            xCommon.Add(xUser);

            string strErrorMessage;
            string strXML = xCommon.ToString();
            string strResult = SendQuery(strXML, "user", out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("사용자 등록 실패 : " + strErrorMessage);
                return false;
            }

            XElement xml = XElement.Parse(strResult);
            XElement user = FindElement(xml, "user");

            if (user != null)
            {
                XElement id = FindElement(user, "user_id");
                XElement key = FindElement(xml, "key_id");

                if (id != null && key != null)
                {
                    strNewID = id.Value;
                    strNewKey = key.Value;

                    string str = string.Format("사용자 등록 성공 : ID({0}, PW({1}), Key({2})", strNewID, strPassword, strNewKey);
                    System.Diagnostics.Trace.WriteLine(str);
                    return true;
                }
            }

            return false;
        }

        private XElement MakeElement(string strElementName, string strValue)
        {
            XElement x = new XElement(strElementName);
            x.SetValue(strValue);
            return x;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string strName = textBoxName.Text.Trim();
            
            if (CheckEmpty(strName, textBoxName, "ID를") == false)
                return;
            
            string strXML = XMLLogin().Trim();

            if (strXML.Length == 0)
                return;

            if (strXML.StartsWith("<") == false)
                return;

            XElement xml = XElement.Parse(strXML);

            XElement id = FindElement(xml, "user_id");
            XElement key = FindElement(xml, "key_id");
            XElement code = FindElement(xml, "rsCode");

            if (id == null || key == null || code == null)
                return;

            if (code.Value != "RS101")
                return;

            string strID = id.Value;
            string strKey = key.Value;
            SendDelete(strID, strKey, strName);
        }

        private bool SendDelete(string strID, string strKey, string strName)
        {
            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xUserName = MakeElement("user_id", strName);
            
            XElement xUser = new XElement("user");
            xUser.Add(xUserName);

            XElement xCommon = new XElement("common");
            xCommon.Add(xState);
            xCommon.Add(xUser);

            string strErrorMessage;
            string strXML = xCommon.ToString();
            string strResult = SendQuery(strXML, "user", out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("사용자 삭제 실패 : " + strErrorMessage);
                return false;
            }

            XElement xml = XElement.Parse(strResult);
            XElement user = FindElement(xml, "user");

            if (user != null)
            {
                XElement id = FindElement(user, "user_id");
                XElement key = FindElement(xml, "key_id");

                if (id != null && key != null)
                {
                    string str = string.Format("사용자 삭제 성공");
                    System.Diagnostics.Trace.WriteLine(str);
                    return true;
                }
            }

            return false;
        }

        private void btnPOIFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "POI 파일 (*.poi)|*.poi|모든 파일 (*.*)|*.*";
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBoxPOIFilePath.Text = dlg.FileName;
            }
        }

        private void btnRegistPOIFile_Click(object sender, EventArgs e)
        {
            string strPOIFile = textBoxPOIFilePath.Text.Trim();

            if (strPOIFile.Length == 0)
            {
                textBoxPOIFilePath.Focus();
                MessageBox.Show("업로드할 POI 파일을 입력하세요");
                return;
            }
            
            string strXML = XMLLogin().Trim();

            if (strXML.Length == 0)
                return;

            if (strXML.StartsWith("<") == false)
                return;

            XElement xml = XElement.Parse(strXML);

            XElement id = FindElement(xml, "user_id");
            XElement key = FindElement(xml, "key_id");
            XElement code = FindElement(xml, "rsCode");

            if (id == null || key == null || code == null)
                return;

            if (code.Value != "RS101")
                return;

            string strID = id.Value;
            string strKey = key.Value;

            string strErrorMessage;

            SendPOIFile(strID, strKey, strPOIFile, out strErrorMessage);
        }

        private void btnUpdatePOIFile_Click(object sender, EventArgs e)
        {
            string strPOIFile = textBoxPOIFilePath.Text.Trim();

            if (strPOIFile.Length == 0)
            {
                textBoxPOIFilePath.Focus();
                MessageBox.Show("업로드할 POI 파일을 입력하세요");
                return;
            }

            string strXML = XMLLogin().Trim();

            if (strXML.Length == 0)
                return;

            if (strXML.StartsWith("<") == false)
                return;

            XElement xml = XElement.Parse(strXML);

            XElement id = FindElement(xml, "user_id");
            XElement key = FindElement(xml, "key_id");
            XElement code = FindElement(xml, "rsCode");

            if (id == null || key == null || code == null)
                return;

            if (code.Value != "RS101")
                return;

            string strID = id.Value;
            string strKey = key.Value;

            string strErrorMessage;

            UpdatePOIFile(strID, strKey, strPOIFile, out strErrorMessage);
        }

        private bool GetFileInfo(string strFilePath, out string strCode, out string strFileName, out string strPOIName)
        {
            strCode = strFileName = strPOIName = "";

            int nDotIndex = strFilePath.LastIndexOf('.');
            int nSlashIndex = strFilePath.LastIndexOf('\\');

            if (nSlashIndex > 0 && nDotIndex > nSlashIndex)
            {
                strFileName = strFilePath.Substring(nSlashIndex + 1);
                strCode = strFilePath.Substring(nSlashIndex + 1, nDotIndex - nSlashIndex - 1);

                WebDBManager dbMgr = new WebDBManager(1);

                string strSQL = "Select Name from POIType where Code = '" + strCode + "'";
                ArrayList arrResult = dbMgr.GetResultData(strSQL, "UnE_BIM");

                if (arrResult == null || arrResult.Count == 0)
                    return false;

                strPOIName = WebDBManager.GetStringField(arrResult[0]);

                // 임시로 직접 데이터 입력
                //if (m_strPOITable.ContainsKey(strCode))
                //    strPOIName = m_strPOITable[strCode];
                return strPOIName != null;
            }

            return false;
        }

        private string SendPOIFile(string strID, string strKey, string strFilePath, out string strErrorMessage)
        {
            strErrorMessage = "";

            string strCode, strFileName, strPOIName;

            if (GetFileInfo(strFilePath, out strCode, out strFileName, out strPOIName) == false)
                return "";

            string url = SERVER_URL + "/sdesignApi/file/equipType";

            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.KeepAlive = true;
            request.Credentials = System.Net.CredentialCache.DefaultCredentials;

            string strResult = "";

            try
            {
                Stream rs = request.GetRequestStream();

                AddStringContents(rs, "state.user_id", strID, boundarybytes);
                AddStringContents(rs, "state.key_id", strKey, boundarybytes);
                AddStringContents(rs, "equip_type_code", strCode, boundarybytes);
                AddFileContents(rs, "equip_type_img", strFileName, "poi", boundarybytes);
                //AddStringContents(rs, "equip_type_name", strPOIName, boundarybytes);

                long nFileSize = (new FileInfo(strFilePath)).Length;
                FileStream fileStream = new FileStream(strFilePath, FileMode.Open, FileAccess.Read);
                byte[] buffer = new byte[nFileSize];
                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    rs.Write(buffer, 0, bytesRead);
                }
                fileStream.Close();

                AddStringContents(rs, "equip_type_name", strPOIName, boundarybytes);

                byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                rs.Write(trailer, 0, trailer.Length);
                rs.Close();

                HttpWebResponse wRes = (HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                strResult = readerPost.ReadToEnd().Trim();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();

                if (strResult.StartsWith("<") == false)
                {
                    strErrorMessage = strResult;
                    System.Diagnostics.Trace.WriteLine("FileUpload Error : " + strErrorMessage);
                    return "";
                }

                XElement xml = XElement.Parse(strResult);
                XElement code = FindElement(xml, "rsCode");

                if (code == null)
                {
                    strErrorMessage = strResult;
                    return "";
                }
                else if (code.Value == "RS101")
                {
                    return strResult;
                }
                else
                {
                    strErrorMessage = strResult;
                }
            }
            catch (System.Net.WebException ex)
            {
                strErrorMessage = ex.Message;
            }

            return "";
        }

        private string SendHazardPOIFile(string strID, string strKey, string strFilePath, out string strErrorMessage)
        {
            strErrorMessage = "";

            string strCode, strFileName, strPOIName;

            if (GetFileInfo(strFilePath, out strCode, out strFileName, out strPOIName) == false)
                return "";

            string url = SERVER_URL + "/sdesignApi/file/hazardType";

            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.KeepAlive = true;
            request.Credentials = System.Net.CredentialCache.DefaultCredentials;

            string strResult = "";

            try
            {
                Stream rs = request.GetRequestStream();

                AddStringContents(rs, "state.user_id", strID, boundarybytes);
                AddStringContents(rs, "state.key_id", strKey, boundarybytes);
                AddStringContents(rs, "hazard_type_code", strCode, boundarybytes);
                AddFileContents(rs, "hazard_type_img", strFileName, "poi", boundarybytes);
                //AddStringContents(rs, "equip_type_name", strPOIName, boundarybytes);

                long nFileSize = (new FileInfo(strFilePath)).Length;
                FileStream fileStream = new FileStream(strFilePath, FileMode.Open, FileAccess.Read);
                byte[] buffer = new byte[nFileSize];
                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    rs.Write(buffer, 0, bytesRead);
                }
                fileStream.Close();

                AddStringContents(rs, "hazard_type_name", strPOIName, boundarybytes);

                byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                rs.Write(trailer, 0, trailer.Length);
                rs.Close();

                HttpWebResponse wRes = (HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                strResult = readerPost.ReadToEnd().Trim();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();

                if (strResult.StartsWith("<") == false)
                {
                    strErrorMessage = strResult;
                    System.Diagnostics.Trace.WriteLine("FileUpload Error : " + strErrorMessage);
                    return "";
                }

                XElement xml = XElement.Parse(strResult);
                XElement code = FindElement(xml, "rsCode");

                if (code == null)
                {
                    strErrorMessage = strResult;
                    return "";
                }
                else if (code.Value == "RS101")
                {
                    return strResult;
                }
                else
                {
                    strErrorMessage = strResult;
                }
            }
            catch (System.Net.WebException ex)
            {
                strErrorMessage = ex.Message;
            }

            return "";
        }

        private string UpdatePOIFile(string strID, string strKey, string strFilePath, out string strErrorMessage)
        {
            strErrorMessage = "";

            string strCode, strFileName, strPOIName;

            if (GetFileInfo(strFilePath, out strCode, out strFileName, out strPOIName) == false)
                return "";

            string url = SERVER_URL + "/sdesignApi/file/equipTypeUpdate";

            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.KeepAlive = true;
            request.Credentials = System.Net.CredentialCache.DefaultCredentials;

            string strResult = "";

            try
            {
                Stream rs = request.GetRequestStream();

                AddStringContents(rs, "state.user_id", strID, boundarybytes);
                AddStringContents(rs, "state.key_id", strKey, boundarybytes);
                AddStringContents(rs, "equip_type_code", strCode, boundarybytes);
                AddFileContents(rs, "equip_type_img", strFileName, "poi", boundarybytes);
                
                long nFileSize = (new FileInfo(strFilePath)).Length;
                FileStream fileStream = new FileStream(strFilePath, FileMode.Open, FileAccess.Read);
                byte[] buffer = new byte[nFileSize];
                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    rs.Write(buffer, 0, bytesRead);
                }
                fileStream.Close();

                byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                rs.Write(trailer, 0, trailer.Length);
                rs.Close();

                HttpWebResponse wRes = (HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                strResult = readerPost.ReadToEnd().Trim();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();

                if (strResult.StartsWith("<") == false)
                {
                    strErrorMessage = strResult;
                    System.Diagnostics.Trace.WriteLine("FileUpdate Error : " + strErrorMessage);
                    return "";
                }

                XElement xml = XElement.Parse(strResult);
                XElement code = FindElement(xml, "rsCode");

                if (code == null)
                {
                    strErrorMessage = strResult;
                    return "";
                }
                else if (code.Value == "RS101")
                {
                    System.Diagnostics.Trace.WriteLine("UpdatePOI 성공 : " + strFileName);
                    return strResult;
                }
                else
                {
                    strErrorMessage = strResult;
                }
            }
            catch (System.Net.WebException ex)
            {
                strErrorMessage = ex.Message;
            }

            return "";
        }

        private string UpdateHazardPOIFile(string strID, string strKey, string strFilePath, out string strErrorMessage)
        {
            strErrorMessage = "";

            string strCode, strFileName, strPOIName;

            if (GetFileInfo(strFilePath, out strCode, out strFileName, out strPOIName) == false)
                return "";

            string url = SERVER_URL + "/sdesignApi/file/hazardTypeUpdate";

            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.KeepAlive = true;
            request.Credentials = System.Net.CredentialCache.DefaultCredentials;

            string strResult = "";

            try
            {
                Stream rs = request.GetRequestStream();

                AddStringContents(rs, "state.user_id", strID, boundarybytes);
                AddStringContents(rs, "state.key_id", strKey, boundarybytes);
                AddStringContents(rs, "hazard_type_code", strCode, boundarybytes);
                AddFileContents(rs, "hazard_type_img", strFileName, "poi", boundarybytes);

                long nFileSize = (new FileInfo(strFilePath)).Length;
                FileStream fileStream = new FileStream(strFilePath, FileMode.Open, FileAccess.Read);
                byte[] buffer = new byte[nFileSize];
                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    rs.Write(buffer, 0, bytesRead);
                }
                fileStream.Close();

                byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                rs.Write(trailer, 0, trailer.Length);
                rs.Close();

                HttpWebResponse wRes = (HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                strResult = readerPost.ReadToEnd().Trim();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();

                if (strResult.StartsWith("<") == false)
                {
                    strErrorMessage = strResult;
                    System.Diagnostics.Trace.WriteLine("FileUpdate Error : " + strErrorMessage);
                    return "";
                }

                XElement xml = XElement.Parse(strResult);
                XElement code = FindElement(xml, "rsCode");

                if (code == null)
                {
                    strErrorMessage = strResult;
                    return "";
                }
                else if (code.Value == "RS101")
                {
                    System.Diagnostics.Trace.WriteLine("UpdatePOI 성공 : " + strFileName);
                    return strResult;
                }
                else
                {
                    strErrorMessage = strResult;
                }
            }
            catch (System.Net.WebException ex)
            {
                strErrorMessage = ex.Message;
            }

            return "";
        }

        /*private void AddStringContents(StreamWriter stream, string strName, string strValue, string strBoundary)
        {
            stream.WriteLine(strBoundary);

            string strFormData = string.Format("Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}", strName, strValue);
            stream.WriteLine(strFormData);
        }*/

        private void AddStringContents(Stream stream, string strName, string strValue, byte[] boundaryBytes)
        {
            stream.Write(boundaryBytes, 0, boundaryBytes.Length);

            string strFormData = string.Format("Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}", strName, strValue);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(strFormData);
            stream.Write(bytes, 0, bytes.Length);
        }

        /*private void AddFileContents(StreamWriter stream, string strName, string strFileName, string strContentType, string strBoundary)
        {
            stream.WriteLine(strBoundary);

            string strFormData = string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n",
                strName, strFileName, strContentType);
            stream.WriteLine(strFormData);
        }*/

        private void AddFileContents(Stream stream, string strName, string strFileName, string strContentType, byte[] boundaryBytes)
        {
            stream.Write(boundaryBytes, 0, boundaryBytes.Length);

            string strFormData = string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n",
                strName, strFileName, strContentType);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(strFormData);
            stream.Write(bytes, 0, bytes.Length);
        }

        private void btnSearchPOIFile_Click(object sender, EventArgs e)
        {
            string strPOICode = textBoxPOICode.Text.Trim();

            if (strPOICode.Length == 0)
            {
                textBoxPOICode.Focus();
                MessageBox.Show("POI 코드를 입력하세요");
                return;
            }

            string strXML = XMLLogin().Trim();

            if (strXML.Length == 0)
                return;

            if (strXML.StartsWith("<") == false)
                return;

            XElement xml = XElement.Parse(strXML);

            XElement id = FindElement(xml, "user_id");
            XElement key = FindElement(xml, "key_id");
            XElement code = FindElement(xml, "rsCode");

            if (id == null || key == null || code == null)
                return;

            if (code.Value != "RS101")
                return;

            string strID = id.Value;
            string strKey = key.Value;

            string strPOIName, strSequenceNumber, strFileName, strErrorMessage;

            SendSearchPOI(strID, strKey, strPOICode, out strPOIName, out strSequenceNumber, out strFileName, out strErrorMessage);
        }

        private void btnDownloadPOIFile_Click(object sender, EventArgs e)
        {
            string strPOICode = textBoxPOICode.Text.Trim();

            if (strPOICode.Length == 0)
            {
                textBoxPOICode.Focus();
                MessageBox.Show("POI 코드를 입력하세요");
                return;
            }

            string strXML = XMLLogin().Trim();

            if (strXML.Length == 0)
                return;

            if (strXML.StartsWith("<") == false)
                return;

            XElement xml = XElement.Parse(strXML);

            XElement id = FindElement(xml, "user_id");
            XElement key = FindElement(xml, "key_id");
            XElement code = FindElement(xml, "rsCode");

            if (id == null || key == null || code == null)
                return;

            if (code.Value != "RS101")
                return;

            string strID = id.Value;
            string strKey = key.Value;

            string strPOIName, strSequenceNumber, strFileName, strErrorMessage;

            if (SendSearchPOI(strID, strKey, strPOICode, out strPOIName, out strSequenceNumber, out strFileName, out strErrorMessage) == false)
                return;

            string strLocalFolder = GetDownloadFolder();
            DownloadPOIFile(strSequenceNumber, strFileName, strLocalFolder);
        }

        private void DownloadPOIFile(string strSequenceNumber, string strFileName, string strLocalFolder)
        {
            string strURL = SERVER_URL + "/file/download/" + strSequenceNumber;

            try
            {
                string strLocalFilePath = strLocalFolder + "\\" + strFileName;

                WebClient client = new WebClient();
                client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                client.DownloadFile(strURL, strLocalFilePath);

                System.Diagnostics.Trace.WriteLine("File Download 성공 : " + strLocalFilePath);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine("File Download 실패 : " + strURL + ", " + e.Message);
            }
        }

        private string GetDownloadFolder()
        {
            string strDocumentFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            int nSlashIndex = strDocumentFolder.LastIndexOf('\\');

            if (nSlashIndex < 0)
                return strDocumentFolder;

            string strFolder = strDocumentFolder.Substring(0, nSlashIndex + 1);
            string strDownloadFolder = strFolder + "Downloads";

            if (Directory.Exists(strDownloadFolder))
                return strDownloadFolder;

            return strDocumentFolder;
        }

        private bool SendSearchPOI(string strID, string strKey, string strPOICode, out string strPOIName, out string strSequenceNumber, out string strFileName, out string strErrorMessage)
        {
            strPOIName = "";
            strSequenceNumber = "";
            strFileName = "";

            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xEquipTypeCode = MakeElement("paramEquipCode", strPOICode);

            XElement xCommon = new XElement("convergence");
            xCommon.Add(xState);
            xCommon.Add(xEquipTypeCode);

            strErrorMessage = "";
            string strXML = xCommon.ToString();
            string strResult = SendQuery(strXML, "convergence/equipTypeSearch", out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("POI Type 조회 실패 : " + strErrorMessage);
                return false;
            }

            XElement xml = XElement.Parse(strResult);
            XElement equipType = FindElement(xml, "equipType");

            if (equipType != null)
            {
                XElement poiName = FindElement(equipType, "equip_type_name");

                if (poiName != null)
                {
                    strPOIName = poiName.Value;
                    return SendSearchPOIDetail(strID, strKey, strPOICode, strPOIName, out strSequenceNumber, out strFileName, out strErrorMessage);
                }
                
                /*if (poiName != null && seq != null && fileName != null)
                {
                    strPOIName = poiName.Value;
                    strSequenceNumber = seq.Value;
                    strFileName = fileName.Value;

                    string str = string.Format("POI 조회 성공 : Code({0}, Name({1}), Seq({2})", strPOICode, strPOIName, strSequenceNumber);
                    System.Diagnostics.Trace.WriteLine(str);
                    return true;
                }*/
            }

            System.Diagnostics.Trace.WriteLine("POI 조회 실패 : 존재하지 않는 POI(" + strPOICode + ")");
            return false;
        }

        private bool SendSearchPOIDetail(string strID, string strKey, string strPOICode, string strPOIName, out string strSequenceNumber, out string strFileName, out string strErrorMessage)
        {
            strSequenceNumber = "";
            strFileName = "";

            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xEquipTypeCode = MakeElement("equip_type_code", strPOICode);
            XElement xEquipTypeName = MakeElement("equip_type_name", strPOICode);

            XElement xEquipType = new XElement("equipType");
            xEquipType.Add(xEquipTypeCode);
            xEquipType.Add(xEquipTypeName);

            XElement xCommon = new XElement("convergence");
            xCommon.Add(xState);
            xCommon.Add(xEquipType);

            strErrorMessage = "";
            string strXML = xCommon.ToString();
            string strResult = SendQuery(strXML, "convergence/equipTypeDetail", out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("POI Type Detail 조회 실패 : " + strErrorMessage);
                return false;
            }

            XElement xml = XElement.Parse(strResult);
            XElement equipType = FindElement(xml, "equipType");

            if (equipType != null)
            {
                XElement seq = FindElement(equipType, "file_seq");
                XElement fileName = FindElement(equipType, "file_org_name");

                if (seq != null && fileName != null)
                {
                    strSequenceNumber = seq.Value;
                    strFileName = fileName.Value;

                    string str = string.Format("POI 조회 성공 : Code({0}, Name({1}), Seq({2})", strPOICode, strPOIName, strSequenceNumber);
                    System.Diagnostics.Trace.WriteLine(str);
                    return true;
                }
            }

            System.Diagnostics.Trace.WriteLine("POI 조회 실패 : 존재하지 않는 POI(" + strPOICode + ")");
            return false;
        }

        private bool SendSearchHazardPOI(string strID, string strKey, string strPOICode, out string strPOIName, out string strSequenceNumber, out string strFileName, out string strErrorMessage)
        {
            strPOIName = "";
            strSequenceNumber = "";
            strFileName = "";

            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xEquipTypeCode = MakeElement("paramHazardCode", strPOICode);

            XElement xCommon = new XElement("convergence");
            xCommon.Add(xState);
            xCommon.Add(xEquipTypeCode);

            strErrorMessage = "";
            string strXML = xCommon.ToString();
            // 위험물 조회
            string strResult = SendQuery(strXML, "convergence/hazardTypeSearch", out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("hazard POI Type 조회 실패 : " + strErrorMessage);
                return false;
            }

            XElement xml = XElement.Parse(strResult);
            XElement equipType = FindElement(xml, "hazardType");

            if (equipType != null)
            {
                XElement poiName = FindElement(equipType, "hazard_type_name");

                if (poiName != null)
                {
                    strPOIName = poiName.Value;
                    return SendSearchHazardPOIDetail(strID, strKey, strPOICode, strPOIName, out strSequenceNumber, out strFileName, out strErrorMessage);
                }
            }

            System.Diagnostics.Trace.WriteLine("hazard POI 조회 실패 : 존재하지 않는 POI(" + strPOICode + ")");
            return false;
        }

        private bool SendSearchHazardPOIDetail(string strID, string strKey, string strPOICode, string strPOIName, out string strSequenceNumber, out string strFileName, out string strErrorMessage)
        {
            strSequenceNumber = "";
            strFileName = "";

            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xEquipTypeCode = MakeElement("hazard_type_code", strPOICode);
            XElement xEquipTypeName = MakeElement("hazard_type_name", strPOICode);

            XElement xEquipType = new XElement("hazardType");
            xEquipType.Add(xEquipTypeCode);
            xEquipType.Add(xEquipTypeName);

            XElement xCommon = new XElement("convergence");
            xCommon.Add(xState);
            xCommon.Add(xEquipType);

            strErrorMessage = "";
            string strXML = xCommon.ToString();
            string strResult = SendQuery(strXML, "convergence/hazardTypeDetail", out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("hazard POI Type Detail 조회 실패 : " + strErrorMessage);
                return false;
            }

            XElement xml = XElement.Parse(strResult);
            XElement equipType = FindElement(xml, "hazardType");

            if (equipType != null)
            {
                XElement seq = FindElement(equipType, "file_grp_id");
                XElement fileName = FindElement(equipType, "hazard_type_name");

                if (seq != null && fileName != null)
                {
                    strSequenceNumber = seq.Value;
                    strFileName = fileName.Value;

                    string str = string.Format("hazard POI 조회 성공 : Code({0}, Name({1}), Seq({2})", strPOICode, strPOIName, strSequenceNumber);
                    System.Diagnostics.Trace.WriteLine(str);
                    return true;
                }
            }

            System.Diagnostics.Trace.WriteLine("hazard POI 조회 실패 : 존재하지 않는 POI(" + strPOICode + ")");
            return false;
        }

        // Return 값
        // Registed : 이미 등록됨
        // None : 조회는 되나 파일이 없음.
        // Fail : 조회 실패
        private string CheckSearchPOIDetail(string strID, string strKey, string strPOICode, string strPOIName, out string strSequenceNumber, out string strFileName, out string strErrorMessage)
        {
            strSequenceNumber = "";
            strFileName = "";

            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xEquipTypeCode = MakeElement("equip_type_code", strPOICode);
            XElement xEquipTypeName = MakeElement("equip_type_name", strPOICode);

            XElement xEquipType = new XElement("equipType");
            xEquipType.Add(xEquipTypeCode);
            xEquipType.Add(xEquipTypeName);

            XElement xCommon = new XElement("convergence");
            xCommon.Add(xState);
            xCommon.Add(xEquipType);

            strErrorMessage = "";
            string strXML = xCommon.ToString();
            string strResult = SendQuery(strXML, "convergence/equipTypeDetail", out strErrorMessage);

            if (strResult.Length == 0)
            {
                //System.Diagnostics.Trace.WriteLine("POI Type Detail 조회 실패 : " + strErrorMessage);
                return "Fail";
            }

            XElement xml = XElement.Parse(strResult);
            XElement equipType = FindElement(xml, "equipType");

            if (equipType != null)
            {
                XElement seq = FindElement(equipType, "file_seq");
                XElement fileName = FindElement(equipType, "file_org_name");

                if (seq != null && fileName != null)
                {
                    strSequenceNumber = seq.Value;
                    strFileName = fileName.Value;

                    //string str = string.Format("POI 조회 성공 : Code({0}, Name({1}), Seq({2})", strPOICode, strPOIName, strSequenceNumber);
                    //System.Diagnostics.Trace.WriteLine(str);
                    return "Registed";
                }
            }

            //System.Diagnostics.Trace.WriteLine("POI 조회 실패 : 존재하지 않는 POI(" + strPOICode + ")");
            return "None";
        }
        private string CheckSearchHazardPOIDetail(string strID, string strKey, string strPOICode, string strPOIName, out string strSequenceNumber, out string strFileName, out string strErrorMessage)
        {
            strSequenceNumber = "";
            strFileName = "";

            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xEquipTypeCode = MakeElement("hazard_type_code", strPOICode);
            XElement xEquipTypeName = MakeElement("hazard_type_name", strPOICode);

            XElement xEquipType = new XElement("hazardType");
            xEquipType.Add(xEquipTypeCode);
            xEquipType.Add(xEquipTypeName);

            XElement xCommon = new XElement("convergence");
            xCommon.Add(xState);
            xCommon.Add(xEquipType);

            strErrorMessage = "";
            string strXML = xCommon.ToString();
            string strResult = SendQuery(strXML, "convergence/hazardTypeDetail", out strErrorMessage);

            if (strResult.Length == 0)
            {
                //System.Diagnostics.Trace.WriteLine("POI Type Detail 조회 실패 : " + strErrorMessage);
                return "Fail";
            }

            XElement xml = XElement.Parse(strResult);
            XElement equipType = FindElement(xml, "hazardType");

            if (equipType != null)
            {
                XElement seq = FindElement(equipType, "hazard_type_code");
                XElement fileName = FindElement(equipType, "hazard_type_name");

                if (seq != null && fileName != null)
                {
                    strSequenceNumber = seq.Value;
                    strFileName = fileName.Value;

                    //string str = string.Format("POI 조회 성공 : Code({0}, Name({1}), Seq({2})", strPOICode, strPOIName, strSequenceNumber);
                    //System.Diagnostics.Trace.WriteLine(str);
                    return "Registed";
                }
            }

            //System.Diagnostics.Trace.WriteLine("POI 조회 실패 : 존재하지 않는 POI(" + strPOICode + ")");
            return "None";
        }

        private void btnSearchAllPOIs_Click(object sender, EventArgs e)
        {
            /*string strPOICode = textBoxPOICode.Text.Trim();

            if (strPOICode.Length == 0)
            {
                textBoxPOICode.Focus();
                MessageBox.Show("POI 코드를 입력하세요");
                return;
            }

            string strXML = XMLLogin().Trim();

            if (strXML.Length == 0)
                return;

            if (strXML.StartsWith("<") == false)
                return;

            XElement xml = XElement.Parse(strXML);

            XElement id = FindElement(xml, "user_id");
            XElement key = FindElement(xml, "key_id");
            XElement code = FindElement(xml, "rsCode");

            if (id == null || key == null || code == null)
                return;

            if (code.Value != "RS101")
                return;

            string strID = id.Value;
            string strKey = key.Value;

            string strPOIName, strSequenceNumber, strFileName, strErrorMessage;
            SendSearchPOI(strID, strKey, strPOICode, out strPOIName, out strSequenceNumber, out strFileName, out strErrorMessage);*/
        }

        private void btnPOIFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.ShowNewFolderButton = true;
            dlg.Description = "POI 파일이 있는 폴더를 선택하세요";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBoxPOIFolderPath.Text = dlg.SelectedPath;
            }
        }

        private void btnRegistPOIFolder_Click(object sender, EventArgs e)
        {
            int nTotal = 0;
            int nFail = 0;
            int nRegisted = 0;
            int nModifity = 0;
            string strPath = textBoxPOIFolderPath.Text.Trim();

            if (strPath.Length == 0)
            {
                textBoxPOIFolderPath.Focus();
                MessageBox.Show("POI 파일이 있는 폴더를 지정하세요.");
                return;
            }

            if (Directory.Exists(strPath) == false)
            {
                textBoxPOIFolderPath.Focus();
                MessageBox.Show("잘못된 경로입니다.");
                return;
            }


            string strXML = XMLLogin().Trim();

            if (strXML.Length == 0)
                return;

            if (strXML.StartsWith("<") == false)
                return;

            XElement xml = XElement.Parse(strXML);

            XElement id = FindElement(xml, "user_id");
            XElement key = FindElement(xml, "key_id");
            XElement code = FindElement(xml, "rsCode");

            if (id == null || key == null || code == null)
                return;

            if (code.Value != "RS101")
                return;

            string strID = id.Value;
            string strKey = key.Value;


            int nFileCount = 0;
            string[] files = Directory.GetFiles(strPath, "*.poi");
            nTotal = files.Count();

            if (nTotal == 0)
            {
                MessageBox.Show("해당 폴더에 poi 파일이 없습니다.");
                return;
            }


            foreach (string strFile in files)
            {
                string strSequenceNumber = "", strFileName = "", strErrorMessage = "";
                string strPOICode = "", strPOIName = "";
                string strCheck = "Fail";

                int nIndex = strFile.LastIndexOf("\\");
                strPOICode = strFile.Substring(nIndex + 1);

                nIndex = strPOICode.IndexOf(".");
                strPOICode = strPOICode.Substring(0, nIndex);

                // 폴더의 POI 파일 업로드 기능
                // 조회
                // Registed : 이미 등록됨
                // None : 조회는 되나 파일이 없음.
                // Fail : 조회 실패
                strCheck = CheckSearchPOIDetail(strID, strKey, strPOICode, strPOIName, out strSequenceNumber, out strFileName, out strErrorMessage);

                if (strCheck == "Registed")
                {
                    // 이미 등록되어 있음 >> 수정
                    UpdatePOIFile(strID, strKey, strFile, out strErrorMessage);
                    nModifity++;
                }
                else if (strCheck == "None")
                {
                    // 등록되어 있지 않음 >> 등록
                    SendPOIFile(strID, strKey, strFile, out strErrorMessage);
                    nRegisted++;
                }
                else
                {
                    // 실패
                    // 출력 창에 표시
                    System.Diagnostics.Trace.WriteLine(strPOICode + " POI Type Detail 조회 실패 : " + strErrorMessage);
                    nFail++;
                }
            }

            MessageBox.Show(string.Format("총 {0}개, 등록: {1}개, 수정: {2}개, 실패: {3}개", nTotal, nRegisted, nModifity, nFail));
            return;
        }

        private void btnRegistHazardPOIFile_Click(object sender, EventArgs e)
        {
            string strPOIFile = textBoxHazardPOIFilePath.Text.Trim();

            if (strPOIFile.Length == 0)
            {
                textBoxHazardPOIFilePath.Focus();
                MessageBox.Show("업로드할 POI 파일을 입력하세요");
                return;
            }

            string strXML = XMLLogin().Trim();

            if (strXML.Length == 0)
                return;

            if (strXML.StartsWith("<") == false)
                return;

            XElement xml = XElement.Parse(strXML);

            XElement id = FindElement(xml, "user_id");
            XElement key = FindElement(xml, "key_id");
            XElement code = FindElement(xml, "rsCode");

            if (id == null || key == null || code == null)
                return;

            if (code.Value != "RS101")
                return;

            string strID = id.Value;
            string strKey = key.Value;
            string strErrorMessage;

            SendHazardPOIFile(strID, strKey, strPOIFile, out strErrorMessage);
        }

        private void btnHazardPOIFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "POI 파일 (*.poi)|*.poi|모든 파일 (*.*)|*.*";
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBoxHazardPOIFilePath.Text = dlg.FileName;
            }
        }

        private void btnUpdateHazardPOIFile_Click(object sender, EventArgs e)
        {
            string strPOIFile = textBoxHazardPOIFilePath.Text.Trim();

            if (strPOIFile.Length == 0)
            {
                textBoxHazardPOIFilePath.Focus();
                MessageBox.Show("업로드할 POI 파일을 입력하세요");
                return;
            }

            string strXML = XMLLogin().Trim();

            if (strXML.Length == 0)
                return;

            if (strXML.StartsWith("<") == false)
                return;

            XElement xml = XElement.Parse(strXML);

            XElement id = FindElement(xml, "user_id");
            XElement key = FindElement(xml, "key_id");
            XElement code = FindElement(xml, "rsCode");

            if (id == null || key == null || code == null)
                return;

            if (code.Value != "RS101")
                return;

            string strID = id.Value;
            string strKey = key.Value;

            string strErrorMessage;

            UpdateHazardPOIFile(strID, strKey, strPOIFile, out strErrorMessage);
        }

        private void btnHazardPOIFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.ShowNewFolderButton = true;
            dlg.Description = "POI 파일이 있는 폴더를 선택하세요";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                textBoxHazardPOIFolderPath.Text = dlg.SelectedPath;
            }
        }

        private void btnRegistHazardPOIFolder_Click(object sender, EventArgs e)
        {
            int nTotal = 0;
            int nFail = 0;
            int nRegisted = 0;
            int nModifity = 0;
            string strPath = textBoxHazardPOIFolderPath.Text.Trim();

            if (strPath.Length == 0)
            {
                textBoxHazardPOIFolderPath.Focus();
                MessageBox.Show("POI 파일이 있는 폴더를 지정하세요.");
                return;
            }

            if (Directory.Exists(strPath) == false)
            {
                textBoxHazardPOIFolderPath.Focus();
                MessageBox.Show("잘못된 경로입니다.");
                return;
            }


            string strXML = XMLLogin().Trim();

            if (strXML.Length == 0)
                return;

            if (strXML.StartsWith("<") == false)
                return;

            XElement xml = XElement.Parse(strXML);

            XElement id = FindElement(xml, "user_id");
            XElement key = FindElement(xml, "key_id");
            XElement code = FindElement(xml, "rsCode");

            if (id == null || key == null || code == null)
                return;

            if (code.Value != "RS101")
                return;

            string strID = id.Value;
            string strKey = key.Value;


            int nFileCount = 0;
            string[] files = Directory.GetFiles(strPath, "*.poi");
            nTotal = files.Count();

            if (nTotal == 0)
            {
                MessageBox.Show("해당 폴더에 poi 파일이 없습니다.");
                return;
            }


            foreach (string strFile in files)
            {
                string strSequenceNumber = "", strFileName = "", strErrorMessage = "";
                string strPOICode = "", strPOIName = "";
                string strCheck = "Fail";

                int nIndex = strFile.LastIndexOf("\\");
                strPOICode = strFile.Substring(nIndex + 1);

                nIndex = strPOICode.IndexOf(".");
                strPOICode = strPOICode.Substring(0, nIndex);

                // 폴더의 POI 파일 업로드 기능
                // 조회
                // Registed : 이미 등록됨
                // None : 조회는 되나 파일이 없음.
                // Fail : 조회 실패
                strCheck = CheckSearchHazardPOIDetail(strID, strKey, strPOICode, strPOIName, out strSequenceNumber, out strFileName, out strErrorMessage);

                if (strCheck == "Registed")
                {
                    // 이미 등록되어 있음 >> 수정
                    UpdateHazardPOIFile(strID, strKey, strFile, out strErrorMessage);
                    nModifity++;
                }
                else if (strCheck == "None")
                {
                    // 등록되어 있지 않음 >> 등록
                    SendHazardPOIFile(strID, strKey, strFile, out strErrorMessage);
                    nRegisted++;
                }
                else
                {
                    // 실패
                    // 출력 창에 표시
                    System.Diagnostics.Trace.WriteLine(strPOICode + " POI Type Detail 조회 실패 : " + strErrorMessage);
                    nFail++;
                }
            }

            MessageBox.Show(string.Format("총 {0}개, 등록: {1}개, 수정: {2}개, 실패: {3}개", nTotal, nRegisted, nModifity, nFail));
            return;
        }

        private void btnSearchHazardPOI_Click(object sender, EventArgs e)
        {
            string strPOICode = textBoxHazardPOICode.Text.Trim();

            if (strPOICode.Length == 0)
            {
                textBoxHazardPOICode.Focus();
                MessageBox.Show("POI 코드를 입력하세요");
                return;
            }

            string strXML = XMLLogin().Trim();

            if (strXML.Length == 0)
                return;

            if (strXML.StartsWith("<") == false)
                return;

            XElement xml = XElement.Parse(strXML);

            XElement id = FindElement(xml, "user_id");
            XElement key = FindElement(xml, "key_id");
            XElement code = FindElement(xml, "rsCode");

            if (id == null || key == null || code == null)
                return;

            if (code.Value != "RS101")
                return;

            string strID = id.Value;
            string strKey = key.Value;

            string strPOIName, strSequenceNumber, strFileName, strErrorMessage;

            SendSearchHazardPOI(strID, strKey, strPOICode, out strPOIName, out strSequenceNumber, out strFileName, out strErrorMessage);
        }

        private void btnDownloadHazardPOIFile_Click(object sender, EventArgs e)
        {
            string strPOICode = textBoxHazardPOICode.Text.Trim();

            if (strPOICode.Length == 0)
            {
                textBoxHazardPOICode.Focus();
                MessageBox.Show("POI 코드를 입력하세요");
                return;
            }

            string strXML = XMLLogin().Trim();

            if (strXML.Length == 0)
                return;

            if (strXML.StartsWith("<") == false)
                return;

            XElement xml = XElement.Parse(strXML);

            XElement id = FindElement(xml, "user_id");
            XElement key = FindElement(xml, "key_id");
            XElement code = FindElement(xml, "rsCode");

            if (id == null || key == null || code == null)
                return;

            if (code.Value != "RS101")
                return;

            string strID = id.Value;
            string strKey = key.Value;

            string strPOIName, strSequenceNumber, strFileName, strErrorMessage;

            if (SendSearchHazardPOI(strID, strKey, strPOICode, out strPOIName, out strSequenceNumber, out strFileName, out strErrorMessage) == false)
                return;

            string strLocalFolder = GetDownloadFolder();
            DownloadPOIFile(strSequenceNumber, strFileName, strLocalFolder);
        }

        /*private bool SendSearchPOI(string strID, string strKey, string strPOICode, out string strPOIName, out string strSequenceNumber, out string strFileName, out string strErrorMessage)
        {
            strPOIName = "";
            strSequenceNumber = "";
            strFileName = "";

            XElement xUserID = MakeElement("user_id", strID);
            XElement xKeyID = MakeElement("key_id", strKey);

            XElement xState = new XElement("state");
            xState.Add(xUserID);
            xState.Add(xKeyID);

            XElement xEquipTypeCode = MakeElement("equip_type_code", strPOICode);
            
            XElement xUser = new XElement("equipType");
            xUser.Add(xEquipTypeCode);
            
            XElement xCommon = new XElement("convergence");
            xCommon.Add(xState);
            xCommon.Add(xUser);

            strErrorMessage = "";
            string strXML = xCommon.ToString();
            string strResult = SendQuery(strXML, "convergence/fileEquipTypeDetail", out strErrorMessage);

            if (strResult.Length == 0)
            {
                System.Diagnostics.Trace.WriteLine("POI Type 조회 실패 : " + strErrorMessage);
                return false;
            }

            XElement xml = XElement.Parse(strResult);
            XElement equipType = FindElement(xml, "equipType");

            if (equipType != null)
            {
                XElement poiName = FindElement(equipType, "equip_type_name");
                XElement seq = FindElement(equipType, "file_seq");
                XElement fileName = FindElement(equipType, "file_org_name");

                if (poiName != null && seq != null && fileName != null)
                {
                    strPOIName = poiName.Value;
                    strSequenceNumber = seq.Value;
                    strFileName = fileName.Value;

                    string str = string.Format("POI 조회 성공 : Code({0}, Name({1}), Seq({2})", strPOICode, strPOIName, strSequenceNumber);
                    System.Diagnostics.Trace.WriteLine(str);
                    return true;
                }
            }

            System.Diagnostics.Trace.WriteLine("POI 조회 실패 : 존재하지 않는 POI(" + strPOICode + ")");
            return false;
        }*/
    }
}
