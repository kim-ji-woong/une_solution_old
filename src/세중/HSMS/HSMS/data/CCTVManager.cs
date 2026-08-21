using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSMS
{
    public class CCTVManager
    {
        private static CCTVManager m_Instance = null;
        public static CCTVManager Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new CCTVManager();
                return m_Instance;
            }
        }


        // 화면 캡처가 저장되는 경로
        private string m_strCapturePath = string.Empty;
        /// <summary>
        /// 화면 캡처가 저장되는 경로
        /// </summary>
        public string CapturePath
        {
            get { return m_strCapturePath; }
            set { m_strCapturePath = value; }
        }

        // 유효한 CCTV 정보(DB에서 사용되고 있는 CCTV정보)
        public List<CCTVViewer.CCTV> m_liValidCCTV = new List<CCTVViewer.CCTV>();

        // CCTV목록
        private Dictionary<int, CCTVViewer.CCTV> m_dicCCTVList = new Dictionary<int, CCTVViewer.CCTV>();
        /// <summary>
        /// CCTV목록
        /// </summary>
        public Dictionary<int, CCTVViewer.CCTV> DicCCTVList
        {
            get { return m_dicCCTVList; }
        }

        // 현재 사용자가 시청중인 CCTV목록
        private List<CCTVViewer.CCTVCrtl> m_liLiveCCTV = new List<CCTVViewer.CCTVCrtl>();
        /// <summary>
        /// 현재 사용자가 시청중인 CCTV목록
        /// </summary>
        public List<CCTVViewer.CCTVCrtl> ListLiveCCTV
        {
            get { return m_liLiveCCTV; }
        }


        /// <summary>
        /// CCTV 정보 등록 (ID값을 Key값으로 하여 기존에 있던 CCTV 정보인 경우에는 데이터를 수정하도록 함.)
        /// </summary>
        /// <param name="cctv"></param>
        public void AddCCTV(CCTVViewer.CCTV cctv)
        {
            CCTVViewer.CCTV cctvSrc = cctv;

            if (m_dicCCTVList.ContainsKey(cctv.ID))
            {
                cctvSrc = m_dicCCTVList[cctv.ID];

                cctvSrc.ID = cctv.ID;
                cctvSrc.CameraName = cctv.CameraName;
                cctvSrc.ServerIP = cctv.ServerIP;
                cctvSrc.ServerControlPort = cctv.ServerControlPort;
                cctvSrc.ServerVideoPort = cctv.ServerVideoPort;
                cctvSrc.ServerAudioTransmitPort = cctv.ServerAudioTransmitPort;
                cctvSrc.ServerAudioReceivePort = cctv.ServerAudioReceivePort;
                cctvSrc.X = cctv.X;
                cctvSrc.Y = cctv.Y;
                cctvSrc.Z = cctv.Z;
                cctvSrc.IsInDoor = cctv.IsInDoor;
                cctvSrc.HTTPPort = cctv.HTTPPort;
                cctvSrc.Type = cctv.Type;
                cctvSrc.Stream = cctv.Stream;
                cctvSrc.VideoChannel = cctv.VideoChannel;
                cctvSrc.UserID = cctv.UserID;
                cctvSrc.UserPassword = cctv.UserPassword;
                cctvSrc.URL = cctv.URL;
                cctvSrc.ChipVersion = cctv.ChipVersion;
                cctvSrc.Description = cctv.Description;
            }
            else
            {
                CCTVManager.Instance.DicCCTVList.Add(cctvSrc.ID, cctvSrc);
            }

            m_liValidCCTV.Add(cctvSrc);
        }

        /// <summary>
        /// CCTV 추가작업 종료
        /// </summary>
        public void EndCCTVLoad()
        {
            List<int> liRemoveCCTV = new List<int>();

            foreach (CCTVViewer.CCTV item in from items in m_dicCCTVList.Values
                                             where m_liValidCCTV.Contains(items) == false
                                             select items
                                            )
            {
                liRemoveCCTV.Add(item.ID);
            }

            foreach (int nID in liRemoveCCTV)
            {
                m_dicCCTVList.Remove(nID);
            }

            m_liValidCCTV.Clear();
        }

        /// <summary>
        /// 사용자가 시청중인 모든 CCTV에 대해서 CCTV정보 갱신 및 재연결
        /// </summary>
        public void ReConnectCCTV()
        {
            foreach (CCTVViewer.CCTVCrtl ctl in m_liLiveCCTV)
            {
                ctl.ReConnectCCTV();
            }
        }
        
    }
}
