using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections;
using System.IO;

namespace LoginKeyMaker
{
    public partial class FormMain : Form
    {
        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder refval, int size, string filepath);

        private string m_strDBName = "";
        private string m_strMacAddr = "";

        public enum CertOption
        {
            NEW_CREATE = 0, // 새로운 계정을 생성한다.
                            // 기존에 같은 이름의 계정이 존재하면 계정 생성이 실패한다.
            UPDATE,         // 기존에 존재하는 계정이 사용할 Mac Address List를 바꾼다.
                            // 정보 수정시 기존에 존재하는 계정과 비밀번호를 그대로 입력하여야 한다.
                            // 기존에 사용하던 Mac Address List는 모두 사라지고 새로운 Mac Address List가 DB에 저장된다.
            INSERT          // 기존에 존재하는 계정이 추가로 사용할 Mac Address List를 입력한다.
                            // 정보 수정시 기존에 존재하는 계정과 비밀번호를 그대로 입력하여야 한다.
                            // 기존에 사용하던 Mac Address List는 그대로 남아있는 상태에서 새로 입력받은 Mac Address List만 추가된다.
        }

        private CertOption m_option = CertOption.NEW_CREATE;

        public FormMain()
        {
            InitializeComponent();
        }

        private string GetIniValue(string section, string key)
        {
            StringBuilder temp = new StringBuilder(255);
            string strPath = Application.StartupPath + "\\LoginConfig.ini";
            GetPrivateProfileString(section, key, "", temp, 255, strPath);

            return temp.ToString();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            m_strDBName = GetIniValue("ServerInfo", "db_name");
            //m_strMacAddr = GetIniValue("ServerInfo", "mac_addr").ToUpper();
            textMacAddress.Text = m_strMacAddr;
            textBoxDBName.Text = m_strDBName;
            radioUser.Checked = true;

            radioNew.Checked = true;
            textBoxCode.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\cert.dat";
        }

        private string GetMacAddressListString(ArrayList arrMacAddrList)
        {
            char[] arrParams = new char[2] { ',', ';' };
            string[] arrList = m_strMacAddr.Split(arrParams);

            foreach (string strMacAddress in arrList)
            {
                string str = FormMacAddrList.TrimString(strMacAddress);
                str = str.ToUpper();

                if (str.Length > 0)
                {
                    arrMacAddrList.Add(str);
                }
            }

            arrMacAddrList.Sort();
            string strMacAddrList = "";

            foreach (string strMacAddress in arrMacAddrList)
            {
                strMacAddrList += strMacAddress;
            }

            return strMacAddrList;
        }

        private void btnCreateCode_Click(object sender, EventArgs e)
        {
            m_strDBName = textBoxDBName.Text;
            m_strMacAddr = textMacAddress.Text;

            if (m_strDBName.Length == 0)
            {
                MessageBox.Show("DB 이름을 입력하세요.");
                textBoxDBName.Focus();
                return;
            }

            if (m_strMacAddr.Length == 0)
            {
                MessageBox.Show("계정을 사용할 Mac Address들을 입력하세요.");
                textMacAddress.Focus();
                return;
            }

            if (textBoxCode.Text.Length == 0)
            {
                MessageBox.Show("인증 파일을 저장할 경로를 입력하세요");
                textBoxCode.Focus();
                return;
            }

            ArrayList arrMacAddrList = new ArrayList();
            string strMacAddrList = GetMacAddressListString(arrMacAddrList);

            if (strMacAddrList.Length == 0)
                return;

            StreamWriter writer = null;

            try
            {
                writer = new StreamWriter(textBoxCode.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                textBoxCode.Focus();
                return;
            }

            int hash1 = m_strDBName.GetHashCode();
            //int hash2 = m_strMacAddr.GetHashCode();
            int hash2 = strMacAddrList.GetHashCode();

            DateTime dtNow = DateTime.Now;
            string strCurrentTime = string.Format("{0}-{1}-{2} {3:00}:{4:00}:{5:00}:{6}",
                dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second, dtNow.Millisecond);

            int hash3 = strCurrentTime.GetHashCode();

            byte[] bytes = new byte[14];

            byte[] bytes1 = BitConverter.GetBytes(hash1);
            byte[] bytes2 = BitConverter.GetBytes(hash2);
            byte[] bytes3 = BitConverter.GetBytes(hash3);

            System.Buffer.BlockCopy(bytes1, 0, bytes, 0, 4);
            System.Buffer.BlockCopy(bytes2, 0, bytes, 4, 4);
            System.Buffer.BlockCopy(bytes1, 0, bytes, 8, 4);
            bytes[12] = radioAdmin.Checked ? (byte)1 : (byte)0;
            bytes[13] = (byte)m_option;

            // 14개 바이트의 순서를 임의로 조작한다.
            long nOrder = RandomOrder(ref bytes);

            string strCodeText = MakeCodeString(bytes, nOrder);
            //textBoxCode.Text = strCodeText;

            WriteCertFile(strCodeText, arrMacAddrList, writer);
        }

        private void WriteCertFile(string strCodeText, ArrayList arrMacAddrList, System.IO.StreamWriter writer)
        {
            string strData = strCodeText;

            foreach (string strMacAddr in arrMacAddrList)
            {
                if (strData.Length == 0)
                    strData = strMacAddr;
                else
                    strData += "\r\n" + strMacAddr;
            }

            string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });
            string strEncrypt = DBUtility.AES256Cipher.AES_encrypt(strData, key);

            writer.Write(strEncrypt);
            writer.Close();

            MessageBox.Show("파일이 생성되었습니다.");
        }

        private string MakeBytesString(byte[] bytes)
        {
            string str = "";

            foreach (byte b in bytes)
            {
                if (str.Length == 0)
                    str = ((int)b).ToString();
                else
                    str = "," + ((int)b).ToString();
            }

            return str;
        }

        private string MakeCodeString(byte[] bytes, long nOrder)
        {
            string strCode = BitConverter.ToString(bytes).Replace("-", "");

            if (nOrder >= 0)
                strCode += nOrder.ToString() + "!";
            else
                strCode += (-nOrder).ToString() + "~";

            return strCode;
        }

        // 바이트의 순서를 임의로 조작한다.
        // 배열의 개수는 16을 넘을수 없다.
        private long RandomOrder(ref byte[] bytes)
        {
            // 배열의 순서를 임의로 섞어놓은 Index Array를 얻어온다.
            ArrayList arrResults = GetRandomArray(bytes);

            if (arrResults == null || arrResults.Count != 16)
                return 0x0123456789abcdef;

            int nBytesCount = bytes.Count();
            byte[] bytesTemp = new byte[nBytesCount];

            for (int i = 0; i < nBytesCount; i++)
                bytesTemp[i] = bytes[i];

            byte[] orderBytes = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };

            for (int i = 0, j = 0; i < 16; i++)
            {
                orderBytes[i / 2] = (byte)((orderBytes[i / 2] << 4) | (int)arrResults[i]);

                int nIndex = (int)arrResults[i];

                if (nIndex < nBytesCount)
                {
                    bytes[j++] = bytesTemp[nIndex];
                }
            }

            return BitConverter.ToInt64(orderBytes, 0);
        }

        // 배열의 순서를 임의로 섞어놓은 Index Array를 얻어온다.
        private ArrayList GetRandomArray(byte[] bytes)
        {
            int nBytesCount = bytes.Count();

            if (nBytesCount > 16)
                return null;

            int nPaddingCount = 16 - nBytesCount;
            int nTotalCount = nPaddingCount > 0 ? nBytesCount + nPaddingCount : nBytesCount;

            bool[] arrFlags = new bool[nTotalCount];

            for (int i = 0; i < nTotalCount; i++)
                arrFlags[i] = false;

            ArrayList arrResults = new ArrayList();
            System.Random random = new Random(DateTime.Now.GetHashCode());

            for (int i = 0; i < nTotalCount; i++)
            {
                int nIndex = GetRandom(arrFlags);

                // nBytesCount 보다 큰 숫자는 Padding 값이며, 이 값은 한번 더 난수를 발생시킨다.
                if (nIndex >= nBytesCount)
                {
                    nIndex = nBytesCount + random.Next(nPaddingCount);
                }

                if (nIndex < 0)
                    return null;

                arrResults.Add(nIndex);
            }

            return arrResults;
        }

        private int GetRandom(bool[] arrFlags)
        {
            int nArrCount = arrFlags.Count();

            System.Random random = new Random(DateTime.Now.GetHashCode());

            for (int i = 0; i < nArrCount * 10; i++)
            {
                int nIndex = random.Next(nArrCount);

                if (!arrFlags[nIndex])
                {
                    arrFlags[nIndex] = true;
                    return nIndex;
                }
            }

            for (int i = 0; i < nArrCount; i++)
            {
                if (!arrFlags[i])
                    return i;
            }

            return -1;
        }

        private void btnMacAddrDetail_Click(object sender, EventArgs e)
        {
            FormMacAddrList frm = new FormMacAddrList(textMacAddress.Text);

            if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                textMacAddress.Text = frm.MacAddressList;
        }

        private void btnFilePath_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();

            if (textBoxCode.Text.Length > 0)
            {
                string strPath = textBoxCode.Text.Replace('/', '\\');

                if (Directory.Exists(strPath))
                {
                    dlg.InitialDirectory = strPath;
                }
                else
                {
                    int nIndex = strPath.LastIndexOf('\\');

                    if (nIndex < 0)
                        dlg.InitialDirectory = Application.StartupPath;
                    else
                        dlg.InitialDirectory = strPath.Substring(0, nIndex);
                }
            }
            else
                dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            dlg.Filter = "인증 Files|*.dat";
            dlg.FilterIndex = 0;
            dlg.Title = "인증 파일로 저장";

            if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                textBoxCode.Text = dlg.FileName;
            }
        }

        private void radioNew_CheckedChanged(object sender, EventArgs e)
        {
            m_option = CertOption.NEW_CREATE;
        }

        private void radioUpdate_CheckedChanged(object sender, EventArgs e)
        {
            m_option = CertOption.UPDATE;
        }

        private void radioInsert_CheckedChanged(object sender, EventArgs e)
        {
            m_option = CertOption.INSERT;
        }
    }
}
