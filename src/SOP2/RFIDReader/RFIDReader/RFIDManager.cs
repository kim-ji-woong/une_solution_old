using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Ubists.NET;
using System.Windows.Forms;

namespace RFIDReader
{
    public class RFIDManager
    {
        private CUbistsLib UbistsLib = new CUbistsLib();
        private int m_hUbistsHandle = -1;
        private bool m_isWorkingThread = false;
        private Thread m_thread = null;

        private bool m_isUSBConnect = true;
        private IReaderOwner m_owner = null;

        private string m_strTagUID = "";

        public RFIDManager()
        {
            m_hUbistsHandle = UbistsLib.InstanceOpen();
        }

        public void FinishReading()
        {
            m_isWorkingThread = false;
            m_thread = null;
            UbistsLib.FinishReadTag(m_hUbistsHandle);

            if (m_isUSBConnect)
                UbistsLib.SerialDisconnect(m_hUbistsHandle);
            else
                UbistsLib.EtherDisconnect(m_hUbistsHandle);
        }

        public bool IsConnected()
        {
            if (!m_isWorkingThread)
                return false;

            if (UbistsLib.IsOpenComm(m_hUbistsHandle))
            {
                //System.Diagnostics.Trace.WriteLine("이미 연결되어 있음");
                return true;
            }
            //else
            //    System.Diagnostics.Trace.WriteLine("새로운 연결이 필요함");

            return false;
        }

        public bool StartReading()
        {
            if (IsConnected())
                return true;

            int nPortNo = GetComPortNo();
            if (nPortNo <= 0)
            {
                m_isWorkingThread = false;
                MessageBox.Show("RFID Reader 장비가 연결되어 있지 않습니다.\r\n장비가 꺼져있지 않은지, 연결 상태는 올바른지 확인하여 주십시오.");
                return false;
            }

            if (!ConnectUSB(nPortNo))
            {
                MessageBox.Show("RFID Reader와의 통신 연결에 실패하였습니다.");
                return false;
            }

            UbistsLib.BeginReadTag(m_hUbistsHandle);

            if (m_thread == null)
            {
                m_isWorkingThread = true;
                m_thread = new Thread(new ThreadStart(ListenThread));
                m_thread.Start();
            }

            return true;
        }

        public bool ConnectUSB(int nComportNo)
        {
            string strComPort = string.Format("COM{0}", nComportNo);
            string strBaudRate = "115200";

            if (UbistsLib.SerialConnect(m_hUbistsHandle, strComPort, strBaudRate))
            {
                m_isUSBConnect = true;
                return true;
            }

            return false;
        }

        private int GetComPortNo()
        {
            var usbDevices = GetUSBDevices();

            foreach (var usbDevice in usbDevices)
            {
                if (usbDevice.Name.Contains("Silicon Labs CP210x USB to UART Bridge"))
                {
                    try
                    {
                        string strID = usbDevice.DeviceID.Substring(3);
                        int nID = int.Parse(strID);
                        return nID;
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            return 0;
        }

        public string GetTag()
        {
            return m_strTagUID;
        }

        private void OnReadRFIDTag(string strTag)
        {
            m_owner.OnReadTag(strTag);
        }

        delegate void SendRFIDTag(string strTag);

        private void ListenThread()
        {
            SendRFIDTag SendMessage = new SendRFIDTag(OnReadRFIDTag);

            while (m_isWorkingThread)
            {
                int nTagCount = UbistsLib.GetTagUIDCount(m_hUbistsHandle);

                for (int i = 0; i < nTagCount; i++)
                {
                    string strTagUID = UbistsLib.GetTagUIDString(m_hUbistsHandle, i).ToString();

                    if (ValidTag(strTagUID))
                    {
                        TrimString(ref strTagUID);
                        m_strTagUID = strTagUID;

                        if (m_owner != null)
                        {
                            FormMain.Instance.Invoke(SendMessage, strTagUID);
                            //m_owner.OnReadTag(m_strTagUID);
                        }
                    }
                }

                UbistsLib.DeleteAllDB(m_hUbistsHandle);
                Thread.Sleep(200);
            }

            m_thread = null;
        }

        private void TrimString(ref string str)
        {
            str = str.TrimStart(new char[] { ' ', '\t', '\r', '\n' });
            str = str.TrimEnd(new char[] { ' ', '\t', '\r', '\n' });
        }

        private bool ValidTag(string strTag)
        {
            int nBeginIndex = 0;
            int nLen = strTag.Length;

            for (int i = 0; nBeginIndex < nLen || i > 12; i++)
            {
                string strByte = strTag.Substring(nBeginIndex, 2);

                if (i >= 2 && strByte != "00")
                    return true;

                nBeginIndex += 3;
            }

            return false;
        }

        static List<USBDeviceInfo> GetUSBDevices()
        {
            List<USBDeviceInfo> devices = new List<USBDeviceInfo>();

            var searcher = new System.Management.ManagementObjectSearcher(@"Select * From Win32_SerialPort");

            foreach (var device in searcher.Get())
            {
                devices.Add(new USBDeviceInfo(
                (string)device.GetPropertyValue("DeviceID"),
                (string)device.GetPropertyValue("PNPDeviceID"),
                (string)device.GetPropertyValue("Description"),
                (string)device.GetPropertyValue("Name")
                ));
            }

            return devices;
        }

        public IReaderOwner Owner
        {
            get { return m_owner; }
            set { m_owner = value; }
        }
    }

    public interface IReaderOwner
    {
        void OnReadTag(string strTag);
    }

    class USBDeviceInfo
    {
        public USBDeviceInfo(string deviceID, string pnpDeviceID, string description, string name)
        {
            this.DeviceID = deviceID;
            this.PnpDeviceID = pnpDeviceID;
            this.Description = description;
            this.Name = name;
        }
        public string DeviceID { get; private set; }
        public string PnpDeviceID { get; private set; }
        public string Description { get; private set; }
        public string Name { get; private set; }

    }
}
