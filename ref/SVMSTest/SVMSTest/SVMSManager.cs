using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using S1SVMSSDKv2.Info;
using S1SVMSSDKv2.Network;
using S1SVMSSDKv2.Model;
using S1SVMSSDKv2.Model.Device;
using S1SVMSSDKv2.Model.Etc;

namespace SVMSTest
{
    public class SVMSManager
    {
        private ManagementServer m_svmsMgr = null;
        private ISVMSClient m_owner = null;

        public SVMSManager(ISVMSClient owner)
        {
            m_owner = owner;
        }

        public void Connect(string strSVMSServerIP, int nPort, string strUserID, string strUserPW)
        {
            m_svmsMgr = new ManagementServer(strSVMSServerIP, nPort, strUserID, strUserPW, false, 1, SVMSClientType.externalclient);

            m_svmsMgr.ClientTypeCompleted += OnClientTypeCompleted;
            m_svmsMgr.LoginCompleted += OnLoginCompleted;
            m_svmsMgr.Disconnected += OnDisconnected;
            m_svmsMgr.Reconnected += OnReconnected;

            #region DeviceCamera
            m_svmsMgr.DeviceCameraListCompleted += OnDeviceCameraListCompleted;
            m_svmsMgr.AddDeviceCameraNotified += OnAddDeviceCameraNotified;
            m_svmsMgr.ModifyDeviceCameraNotified += OnModifyDeviceCameraNotified;
            m_svmsMgr.RemoveDeviceCameraNotified += OnRemoveDeviceCameraNotified;
            #endregion

            m_svmsMgr.Launch(OnNetworkManagerCompleted);
        }

        public void Disconnect()
        {
            m_svmsMgr.Cleanup();
        }

        public void RequestCameraList()
        {
            m_svmsMgr.RequestDeviceCameraList();
        }

        private void OnRemoveDeviceCameraNotified(string managementServerKey, bool isSuccess, DeviceCamera deviceCamera, System.Xml.XmlNode originalActionStructure)
        {
            if (m_owner != null && isSuccess)
                m_owner.OnRemoveCamera(deviceCamera);
        }

        private void OnModifyDeviceCameraNotified(string managementServerKey, bool isSuccess, DeviceCamera deviceCamera, System.Xml.XmlNode originalActionStructure)
        {
            if (m_owner != null && isSuccess)
                m_owner.OnModifyCamera(deviceCamera);
        }

        private void OnAddDeviceCameraNotified(string managementServerKey, bool isSuccess, DeviceCamera deviceCamera, System.Xml.XmlNode originalActionStructure)
        {
            if (m_owner != null && isSuccess)
                m_owner.OnAddCamera(deviceCamera);
        }

        private void OnDeviceCameraListCompleted(string managementServerKey, bool isSuccess, bool isFinished, List<DeviceCamera> deviceCameras, System.Xml.XmlNode originalActionStructure)
        {
            if (m_owner != null)
                m_owner.OnCameraList(isSuccess, isFinished, deviceCameras);
            /*if (isSuccess == true)
            {
                if (isFinished != true)
                {
                    foreach (var deviceCameraItem in deviceCameras)
                    {
                        System.Diagnostics.Trace.WriteLine("DeviceCamera Read : " + deviceCameraItem.CameraName);

                        string deviceCameraGUID = deviceCameraItem.CameraGUID;
                        if (deviceCameraGUID != null)
                        {
                            ListViewItem lvi = new ListViewItem(deviceCameraItem.CameraGUID);
                            lvi.SubItems.Add(deviceCameraItem.CameraName);
                            lvi.SubItems.Add(deviceCameraItem.IsPTZ == true ? "O" : "X");

                            DeviceCameraAdded(lvi);
                        }

                        if (string.IsNullOrEmpty(deviceCameraGUID) == false)
                        {
                            //managementServer.GetIntelligentConfigurationInformation(deviceCameraGUID);
                        }

                        Console.WriteLine("[DeviceCamera] " + deviceCameraItem.CameraGUID);
                    }
                }
                else
                {
                    System.Diagnostics.Trace.WriteLine("DeviceCamera list up completed.");
                }
            }*/
        }

        private void OnReconnected(string managementServerKey, bool isSuccess)
        {
            if (m_owner != null)
                m_owner.OnReconnect();
        }

        private void OnDisconnected(string managementServerKey)
        {
            if (m_owner != null)
                m_owner.OnDisconnect();
        }

        private void OnLoginCompleted(string managementServerKey, bool isSuccess, bool isAdministrator, System.Xml.XmlNode originalActionStructure)
        {
            if (m_owner != null)
                m_owner.OnLogin(isSuccess);
        }

        private void OnClientTypeCompleted(string managementServerKey, bool isSuccess, string clientGUID, System.Xml.XmlNode originalActionStructure)
        {
            if (m_owner != null && isSuccess)
                m_owner.OnClientType(clientGUID);
        }

        private void OnNetworkManagerCompleted(string managementServerKey, bool isSuccess)
        {
            if (m_owner != null)
            {
                m_owner.OnConnection(isSuccess);
            }
        }
    }

    public interface ISVMSClient
    {
        void OnConnection(bool isSuccess);
        void OnClientType(string strClientGUID);
        void OnLogin(bool isSuccess);
        void OnDisconnect();
        void OnReconnect();
        void OnCameraList(bool isSuccess, bool isFinished, List<DeviceCamera> deviceCameras);
        void OnAddCamera(DeviceCamera deviceCamera);
        void OnModifyCamera(DeviceCamera deviceCamera);
        void OnRemoveCamera(DeviceCamera deviceCamera);
    }
}
