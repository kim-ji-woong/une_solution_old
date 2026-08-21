using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using S1SVMSSDKv2.Model.Device;
using System.IO;

namespace SVMSTest
{
    public partial class FormMain : Form, ISVMSClient
    {
        private SVMSManager m_svmsMgr = null;

        public FormMain()
        {
            InitializeComponent();
            m_svmsMgr = new SVMSManager(this);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string strSVMSServerIP = textBoxIP.Text.Trim();
            string strSVMSServerPort = textBoxPort.Text.Trim();
            string strUserID = textBoxID.Text.Trim();
            string strUserPW = textBoxPW.Text.Trim();

            if (strSVMSServerIP.Length == 0)
            {
                textBoxIP.Focus();
                MessageBox.Show("SVMS Server IP를 입력하세요.");
                return;
            }

            if (strUserID.Length == 0)
            {
                textBoxID.Focus();
                MessageBox.Show("SVMS 사용자 ID를 입력하세요.");
                return;
            }

            if (strUserPW.Length == 0)
            {
                textBoxPW.Focus();
                MessageBox.Show("SVMS 사용자 비밀번호를 입력하세요.");
                return;
            }

            int nPort;

            if (int.TryParse(strSVMSServerPort, out nPort) == false || nPort <= 0)
            {
                textBoxPort.Focus();
                MessageBox.Show("SVMS Server Port는 0보다 큰 양의 정수이어야 합니다.");
                return;
            }

            m_svmsMgr.Connect(strSVMSServerIP, nPort, strUserID, strUserPW);
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            // 재접속시 Camera List를 받아오지 않음
            m_svmsMgr.Disconnect();
        }

        private void AddCamera(DeviceCamera camera)
        {
            int nRowIndex = gridCameras.Rows.Add();

            if (nRowIndex >= 0)
            {
                DataGridViewRow row = gridCameras.Rows[nRowIndex];

                row.Cells[0].Value = nRowIndex + 1;
                row.Cells[1].Value = camera.Guid;
                row.Cells[2].Value = camera.CameraName;

                row.Tag = camera;
            }
        }

        private DeviceCamera m_prevCamera = null;
        private int m_nPrevCameraIndex = -1;

        private void ConnectCamera(DeviceCamera camera)
        {
            string strCCTVIP = camera.CameraIPAddress;
            string strRTSP = "rtsp://";
            string strLower = camera.ConnectURL.ToLower();

            int nIndex1 = strLower.IndexOf(strRTSP);
            int nIndex2 = strLower.IndexOf(strCCTVIP);
            string strConnection = camera.ConnectURL;

            if (nIndex1 >= 0 && nIndex2 > nIndex1)
            {
                string strServer = strConnection.Substring(strRTSP.Length, nIndex2 - strRTSP.Length);

                if (strServer.Contains(':') == false)
                {
                    if (strServer.EndsWith("/"))
                        strConnection = strRTSP + strServer.Substring(0, strServer.Length - 1) + ":" + camera.CameraRTSPPort.ToString() + "/" + strCCTVIP;
                    else
                        strConnection = strRTSP + strServer + ":" + camera.CameraRTSPPort.ToString() + "/" + strCCTVIP;
                }
            }
            else
                return;

            if (m_prevCamera == camera)
            {
                System.Diagnostics.Trace.WriteLine(camera.CameraName + " close");
                axRTSPLiveScreen1.CloseRTSPLiveScreen(m_nPrevCameraIndex);
                m_prevCamera = null;
                m_nPrevCameraIndex = -1;
                axRTSPLiveScreen1.Refresh();
                btnExpand.Enabled = false;
            }
            else
            {
                if (m_nPrevCameraIndex >= 0)
                {
                    System.Diagnostics.Trace.WriteLine(m_prevCamera.CameraName + " close & " + camera.CameraName + " open");
                    axRTSPLiveScreen1.CloseRTSPLiveScreen(m_nPrevCameraIndex);
                }
                else
                    System.Diagnostics.Trace.WriteLine(camera.CameraName + " open");

                m_nPrevCameraIndex = axRTSPLiveScreen1.OpenRTSPLiveScreen(strConnection, (short)camera.CameraRTSPPort, "", "", 1);
                m_prevCamera = camera;
                btnExpand.Enabled = true;
            }
        }

        private void gridCameras_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            DataGridViewRow row = gridCameras.Rows[e.RowIndex];
            DeviceCamera camera = (DeviceCamera)row.Tag;

            if (camera != null)
                ConnectCamera(camera);
        }

        private void btnExpand_Click(object sender, EventArgs e)
        {
            FormCCTV frm = new FormCCTV(m_prevCamera);
            frm.ShowDialog();
        }

        private void btnExportCCTVList_Click(object sender, EventArgs e)
        {
            string strPath = "CCTVList.txt";
            StreamWriter writer = new StreamWriter(strPath, false, Encoding.UTF8);

            foreach (DataGridViewRow row in gridCameras.Rows)
            {
                DeviceCamera cctv = (DeviceCamera)row.Tag;

                int nID = row.Index + 1;
                string strName = cctv.CameraName;
                string strURL = cctv.ConnectURL;

                writer.WriteLine("{0}\t{1}\t{2}", nID, strName, strURL);
            }

            writer.Close();
            MessageBox.Show(strPath + " 파일이 작성되었습니다.");
            
        }

        #region ISVMSClient
        public void OnConnection(bool isSuccess)
        {
            if (isSuccess)
            {
                System.Diagnostics.Trace.WriteLine("Connection Success");
            }
            else
            {
                System.Diagnostics.Trace.WriteLine("Connection Fail");
            }
        }

        public void OnClientType(string strClientGUID)
        {
            System.Diagnostics.Trace.WriteLine("Client GUID : " + strClientGUID);
        }

        public void OnLogin(bool isSuccess)
        {
            if (isSuccess)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    btnConnect.Enabled = false;
                    gridCameras.Rows.Clear();
                });

                m_svmsMgr.RequestCameraList();
            }
            else
            {
                this.Invoke((MethodInvoker)delegate
                {
                    btnConnect.Enabled = true;
                    gridCameras.Rows.Clear();
                });
            }
        }

        public void OnDisconnect()
        {
            this.Invoke((MethodInvoker)delegate
            {
                btnConnect.Enabled = true;
                gridCameras.Rows.Clear();
            });
        }

        public void OnReconnect()
        {

        }

        public void OnCameraList(bool isSuccess, bool isFinished, List<DeviceCamera> deviceCameras)
        {
            if (isSuccess == true)
            {
                if (isFinished != true)
                {
                    foreach (var deviceCameraItem in deviceCameras)
                    {
                        string deviceCameraGUID = deviceCameraItem.CameraGUID;
                        if (deviceCameraGUID != null)
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                AddCamera(deviceCameraItem);
                            });
                        }
                    }

                    this.Invoke((MethodInvoker)delegate
                    {
                        btnExportCCTVList.Enabled = true;
                    });

                    System.Diagnostics.Trace.WriteLine("OnCameraList false : " + deviceCameras.Count.ToString());
                }
                else
                {
                    if (deviceCameras != null)
                        System.Diagnostics.Trace.WriteLine("OnCameraList true : " + deviceCameras.Count.ToString());
                    else
                        System.Diagnostics.Trace.WriteLine("OnCameraList true : null");
                }
            }
        }

        public void OnAddCamera(DeviceCamera deviceCamera)
        {

        }

        public void OnModifyCamera(DeviceCamera deviceCamera)
        {

        }

        public void OnRemoveCamera(DeviceCamera deviceCamera)
        {

        }
        #endregion
    }
}
