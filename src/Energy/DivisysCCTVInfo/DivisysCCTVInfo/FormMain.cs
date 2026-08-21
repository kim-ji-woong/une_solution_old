using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json.Linq;

namespace DivisysCCTVInfo
{
    public partial class FormMain : Form
    {
        private FormEventViewer m_eventViewer = new FormEventViewer();
        private FormCCTV m_frmCCTV = null;

        public FormMain()
        {
            InitializeComponent();

            string strPort = System.Configuration.ConfigurationManager.AppSettings.Get("Port");
            string strHost = System.Configuration.ConfigurationManager.AppSettings.Get("host");
            string strID = System.Configuration.ConfigurationManager.AppSettings.Get("id");
            string strPW = System.Configuration.ConfigurationManager.AppSettings.Get("pw");
            string strLoginData = string.Format("{0}:{1}:{2}:{3}", strHost, strPort, strID, strPW);

            textBoxID.Text = strID;
            textBoxPW.Text = strPW;

            m_frmCCTV = new FormCCTV(strHost, strPort, strID, strPW, m_eventViewer, this);
            labelStatus.Text = "";
        }

        private void btnToRTSP_Click(object sender, EventArgs e)
        {
            string strJson = "", strID = "", strPW = "";

            if (GetString(textBoxJson, "RTSP 데이터", ref strJson) == false)
                return;
            if (GetString(textBoxID, "아이디", ref strID) == false)
                return;
            if (GetString(textBoxPW, "비밀번호", ref strPW) == false)
                return;

            MakeRTSPList(strJson, strID, strPW);
        }

        private bool GetString(TextBox textBox, string strTag, ref string str)
        {
            str = textBox.Text.Trim();

            if (str.Length == 0)
            {
                textBox.Focus();
                MessageBox.Show(strTag + "를 입력하세요.");
                return false;
            }

            return true;
        }

        private bool MakeRTSPList(string strJson, string strID, string strPW)
        {
            textBoxRTSP.Text = "";

            List<JObject> objList = ReadJson(strJson, "sources");

            if (objList == null || objList.Count == 0)
            {
                MessageBox.Show("잘못된 Json 형식입니다.");
                return false;
            }

            string strRTSP = "";
            JToken tokenIP, tokenChanels, tokenChannelID, tokenName;

            foreach (JObject obj in objList)
            {
                if (obj.TryGetValue("address", out tokenIP) == false)
                    continue;

                string strIP = tokenIP.Value<string>();
                CheckIP(ref strIP);

                if (obj.TryGetValue("name", out tokenName) == false)
                    continue;

                string strCCTVName = tokenName.Value<string>();

                if (obj.TryGetValue("channels", out tokenChanels) == false)
                    continue;

                strRTSP += "[" + strCCTVName + "]\r\n";

                int nChannelCount = tokenChanels.Count();

                for (int i=0;i<nChannelCount;i++)
                {
                    JToken token = tokenChanels.ElementAt(i);

                    string strToken = token.ToString();
                    string str = GetJson(strToken);

                    JObject data = JObject.Parse(str);

                    if (data.TryGetValue("id", out tokenChannelID) == false)
                        continue;

                    int nChannelID = tokenChannelID.Value<int>();
                    string strRTSPURL = string.Format("rtsp://{0}:{1}@{2}/video{3}?profile=normal\r\n", strID, strPW, strIP, nChannelID);
                    strRTSP += strRTSPURL;
                }
            }

            textBoxRTSP.Text = strRTSP;
            return true;
        }

        private void CheckIP(ref string strIP)
        {
            int len = strIP.Length;

            for (int i=0;i<len;i++)
            {
                char ch = strIP.ElementAt(i);

                if (ch != '.' && (ch < '0' || ch > '9'))
                {
                    strIP = strIP.Substring(0, i);
                    break;
                }
            }
        }

        private List<JObject> ReadJson(string strJson, string strTag)
        {
            List<JObject> objectList = new List<JObject>();

            JObject obj = JObject.Parse(strJson);
            JToken token;

            if (obj.TryGetValue(strTag, out token))
            {
                try
                {
                    int nChildCount = token.Count();

                    for (int i = 0; i < nChildCount; i++)
                    {
                        JToken _token = token.ElementAt(i);

                        string strToken = _token.ToString();
                        string str = GetJson(strToken);

                        JObject data = JObject.Parse(str);
                        objectList.Add(data);
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                }

                return objectList;
            }

            return null;
        }

        private string GetJson(string str)
        {
            if (str.StartsWith("["))
            {
                str = str.Substring(1, str.Length - 2).Trim();
            }
            else if (str.StartsWith("{"))
                return str;
            else
            {
                str = "{" + str + "}";
            }

            return str;
        }

        public void SetStatus(string strText)
        {
            labelStatus.Text = strText;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            //DigestAuthFixer.HTTP_GET("http://demo.nvrsw.com", "guest", "guest");
            m_eventViewer.Show(this);
            m_frmCCTV.Show(this);
        }
    }

    public class EDNETP_ATTR_STATE_DATA
    {
        public const int EDNETP_STATE_OFFLINE = 0;
        public const int EDNETP_STATE_RESOLVE = 1;
        public const int EDNETP_STATE_CONNECT = 2;
        public const int EDNETP_STATE_INIT = 3;
        public const int EDNETP_STATE_LOGIN = 4;
        public const int EDNETP_STATE_PREPARE = 5;
        public const int EDNETP_STATE_ONLINE = 6;

        public static int Code
        {
            get { return 2; }
        }

        public static string GetStatusString(int nStatus)
        {
            if (nStatus == EDNETP_STATE_OFFLINE)
                return "연결안됨";
            else if (nStatus == EDNETP_STATE_RESOLVE)
                return "호스트 주소를 얻어오고 있음";
            else if (nStatus == EDNETP_STATE_CONNECT)
                return "호스트에 연결중인 상태";
            else if (nStatus == EDNETP_STATE_INIT)
                return "세션키 처리중";
            else if (nStatus == EDNETP_STATE_PREPARE)
                return "세션키 처리 성공후 로그인 시도중";
            else if (nStatus == EDNETP_STATE_ONLINE)
                return "로그인 성공";

            return "";
        }
    }

    public class EDNETP_EVENT_ATTR_DATA
    {
        public const int EDNETP_ATTR_CAMERA = 5;
    }
}
