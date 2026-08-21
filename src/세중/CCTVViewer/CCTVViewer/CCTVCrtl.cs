using AxNVS4Viewer2Lib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCTVViewer
{
    public partial class CCTVCrtl : AxNVS4Viewer2
    {
        CCTV m_cctv = null;


        public CCTVCrtl()
        {
            InitializeComponent();
        }

        public CCTVCrtl(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }


        /// <summary>
        /// CCTV정보 컴포넌트에 적용
        /// </summary>
        /// <param name="cctvInfo"></param>
        public void SetCCTVInfo(CCTV cctvInfo)
        {
            if (object.Equals(cctvInfo, m_cctv) == false)
                m_cctv = cctvInfo;

            this.ServerIP                = m_cctv.ServerIP;
            this.ServerControlPort       = m_cctv.ServerControlPort;
            this.ServerVideoPort         = m_cctv.ServerVideoPort;
            this.ServerAudioTransmitPort = m_cctv.ServerAudioTransmitPort;
            this.ServerAudioReceivePort  = m_cctv.ServerAudioReceivePort;
            this.VideoChannel            = m_cctv.VideoChannel;
            this.UserID                  = m_cctv.UserID;
            this.UserPassword            = m_cctv.UserPassword;
            this.ChipVersion             = m_cctv.ChipVersion;
        }

        /// <summary>
        /// CCTV 정보 갱신 및 연결 해제후 재연결
        /// </summary>
        public void ReConnectCCTV()
        {
            if (this.Parent != null
                && this.Parent is System.Windows.Forms.Form)
            {
                (this.Parent as System.Windows.Forms.Form).Text = m_cctv.CameraName;
            }

            SetCCTVInfo(m_cctv);

            DisconnectCCTV();
        }

        /// <summary>
        /// 연결
        /// </summary>
        public void ConnectCCTV()
        {
            if (this.ConnectStatus == NVS4Viewer2Lib._WinsockStatus_Type.wDisconnected)
            {
                if (this.ServerConnect())
                {
                    this.ChannelView = false;
                    this.ImageStretch = true;
                    this.ImageZoomIn = false;
                }
            }
        }

        /// <summary>
        /// 연결해제
        /// </summary>
        public void DisconnectCCTV()
        {
            if (this.ConnectStatus != NVS4Viewer2Lib._WinsockStatus_Type.wDisconnected)
            {
                this.ServerDisconnect();
            }
        }

        /// <summary>
        /// CCTV Screen Capture
        /// </summary>
        /// <param name="strFileName">파일명</param>
        /// <param name="isIgnoreBeforeFile">중복된 파일이 있는 경우 무시하고 덮어 저장할 것인지.</param>
        /// <returns></returns>
        public string CaptureScreen(string strFilePath, string strFileName, bool isIgnoreBeforeFile = true)
        {
            string strFullPath = null;

            if (String.IsNullOrWhiteSpace(strFileName))
            {
                return strFullPath;
            }

            // 지정된 폴더가 없는 경우 만들어줌.
            if (Directory.Exists(strFilePath) == false)
                Directory.CreateDirectory(strFilePath);

            // 파일의 확장자는 무조건 대문자 JPG 만 가능함.
            strFullPath = String.Format(@"{0}{1}.JPG", new DirectoryInfo(strFilePath).FullName, strFileName);

            // 중복된 이름이 있을 때 덮어쓰기 옵션이 False 인 경우 파일명 뒤에 숫자를 붙여줌.
            if (File.Exists(strFullPath) && isIgnoreBeforeFile == false)
                strFullPath = SetFileNumbering(strFilePath, strFileName);

            // 캡쳐 이미지 저장
            if (this.SetStillImage(strFullPath) == false)
            {
                strFullPath = null;
            }


            return strFullPath;
        }

        private string SetFileNumbering(string strFilePath, string strFileName)
        {
            string strRtnFileName = null;
            int nSeq = 0;

            foreach (string fileName in from fileNames in Directory.GetFiles(strFilePath, "*.JPG", SearchOption.TopDirectoryOnly)
                                        where fileNames.Contains(@"\" + strFileName)
                                        select fileNames)
            {
                FileInfo file = new FileInfo(fileName);
                int nParseNumber = 0;

                if (int.TryParse(file.Name.Replace(strFileName, "").Replace(".JPG", ""), out nParseNumber))
                {
                    if (nSeq < nParseNumber)
                        nSeq = nParseNumber;
                }
            }

            nSeq++;

            strRtnFileName = String.Format(@"{0}{1}{2}.JPG", new DirectoryInfo(strFilePath).FullName, strFileName, nSeq);
            
            return strRtnFileName;
        }

    }
}
