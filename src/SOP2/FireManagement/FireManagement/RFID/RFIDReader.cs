using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Ubists.NET;
using System.Windows.Forms;
using System.IO;

namespace Ubists
{
    public class RFIDReader
    {
        private CUbistsLib UbistsLib = new CUbistsLib();
        private int m_hUbistsHandle = -1;
        private bool m_isWorkingThread = false;
        private Thread m_thread = null;

        private bool m_isUSBConnect = true;
        private IReaderOwner m_owner = null;
        private bool m_isConnected = false;

        // RFID Reader를 직접 사용하는 상황이 아니라 Text 파일을 통해서 RFID Tag를 입력받는 모드인가?
        private bool m_isFakeMode = false;

        public bool FakeMode
        {
            get { return m_isFakeMode; }
            set { m_isFakeMode = value; }
        }

        private string m_strTagUID = "";
        /*private static StreamWriter m_writer = new StreamWriter("c:/UNE/fire.log", false, Encoding.UTF8);

        public static void WriteLog(string str)
        {
            DateTime dtNow = DateTime.Now;

            string strLine = string.Format("{0:00}:{1:00}:{2:00} {3}",
                dtNow.Hour, dtNow.Minute, dtNow.Second, str);

            m_writer.WriteLine(strLine);
            m_writer.Flush();
        }*/

        public RFIDReader()
        {
            m_hUbistsHandle = UbistsLib.InstanceOpen();
        }

        public void FinishReading(bool instanceClose = false)
        {
            m_isWorkingThread = false;
            m_thread = null;
            UbistsLib.FinishReadTag(m_hUbistsHandle);

            if (m_isUSBConnect)
                UbistsLib.SerialDisconnect(m_hUbistsHandle);
            else
                UbistsLib.EtherDisconnect(m_hUbistsHandle);

            if (instanceClose)
            {
                UbistsLib.InstanceClose(m_hUbistsHandle);
            }
        }

        public bool IsConnected()
        {
            return m_isWorkingThread && m_isConnected;
            /*if (!m_isWorkingThread)
                return false;

            if (UbistsLib.IsOpenComm(m_hUbistsHandle))
            {
                //System.Diagnostics.Trace.WriteLine("이미 연결되어 있음");
                return true;
            }
            //else
            //    System.Diagnostics.Trace.WriteLine("새로운 연결이 필요함");

            return false;*/
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
                //return false;
            }
            else if (!ConnectUSB(nPortNo))
            {
                MessageBox.Show("RFID Reader와의 통신 연결에 실패하였습니다.");
                //return false;
            }

            UbistsLib.BeginReadTag(m_hUbistsHandle);

            if (m_thread == null)
            {
                m_isWorkingThread = true;

                //FireManagement.FormMain frmMain = FireManagement.FormMain2.Instance;
                //frmMain.ViewControl.SetLabelText(frmMain.CurrentZone.ZoneName + "(RFID Reader 연결중...)");

                m_thread = new Thread(new ThreadStart(ListenThread));
                m_thread.Start();

                Thread connectionThread = new Thread(new ThreadStart(ConnectionThread));
                connectionThread.Start();
            }

            return true;
        }

        private void ConnectionThread()
        {
            m_isConnected = ConfirmConnection();
            bool isFirst = true;

            while (m_isWorkingThread)
            {
                if (m_isConnected)
                {
                    m_isConnected = ConfirmConnection(3000);
                    
                    if (!m_isConnected)
                    {
                        UbistsLib.SerialDisconnect(m_hUbistsHandle);
                        FireManagement.EventManager.Instance.OnDisconnectRFIDReader(true);
                        System.Diagnostics.Trace.WriteLine("접속 끊김");
                    }
                }
                else
                    FireManagement.EventManager.Instance.OnDisconnectRFIDReader(false);

                if (!m_isConnected)
                {
                    int nPortNo = GetComPortNo();

                    if (nPortNo > 0)
                    {
                        if (ConnectUSB(nPortNo))
                        {
                            m_isConnected = ConfirmConnection(3000);

                            if (m_isConnected)
                            {
                                UbistsLib.BeginReadTag(m_hUbistsHandle);
                                FireManagement.EventManager.Instance.OnConnectRFIDReader();
                                System.Diagnostics.Trace.WriteLine("접속 재개");
                            }
                            else
                                UbistsLib.SerialDisconnect(m_hUbistsHandle);
                        }
                    }
                }

                if (m_isConnected)
                {
                    if (isFirst)
                    {
                        FireManagement.EventManager.Instance.OnConnectRFIDReader();
                        isFirst = false;
                    }
                }

                Thread.Sleep(1000);
            }
        }

        private bool ConfirmConnection(int nTimeout = 0)
        {
            if (m_isFakeMode)
                return true;

            bool result = UbistsLib.GetReaderAlive(m_hUbistsHandle);
            int nSleepTime = 0;

            while (nSleepTime < nTimeout)
            {
                if (result)
                    break;

                nSleepTime += 1000;
                Thread.Sleep(1000);

                try
                {
                    result = UbistsLib.GetReaderAlive(m_hUbistsHandle);
                }
                catch (System.AccessViolationException e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                }
            }

            /*if (result)
                System.Diagnostics.Trace.WriteLine(string.Format("Result is true"));*/

            return result;
            //return UbistsLib.GetReaderAlive(m_hUbistsHandle);
        }

        public bool ConnectUSB(int nComportNo)
        {
            if (m_isFakeMode)
                return true;

            string strComPort = string.Format("COM{0}", nComportNo);
            string strBaudRate = "115200";

            try
            {
                if (UbistsLib.SerialConnect(m_hUbistsHandle, strComPort, strBaudRate))
                {
                    //UbistsLib.SetReaderAliveTimeOut(m_hUbistsHandle, 1);
                    m_isUSBConnect = true;
                    return true;
                }
            }
            catch (System.AccessViolationException)
            {
                return false;
            }

            return false;
        }

        private int GetComPortNo()
        {
            /*var usbDevices = GetUSBDevices();

            foreach (var usbDevice in usbDevices)
            {
                if (usbDevice.Name.Contains("Silicon Labs CP210x USB to UART Bridge"))
                {
                    try
                    {
                         //USB로 연결되었음
                        string strID = usbDevice.DeviceID.Substring(3);
                        int nID = int.Parse(strID);
                        return nID;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return 0;*/
            // Bluetooth로 연결되었음
            return FireManagement.FormMain2.Instance.BluetoothComport;
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

            try
            {
                while (m_isWorkingThread)
                {
                    if (m_isFakeMode)
                    {
                        string strTempLogFilePath = Application.StartupPath + "\\Logs\\test.txt";

                        if (File.Exists(strTempLogFilePath))
                        {
                            System.IO.StreamReader reader = new StreamReader(strTempLogFilePath);
                            bool read = false;

                            while (!reader.EndOfStream)
                            {
                                string strLine = reader.ReadLine();

                                if (strLine.Length > 1)
                                {
                                    read = true;
                                    FireManagement.FormMain2.Instance.Invoke(SendMessage, strLine);
                                }
                            }

                            reader.Close();

                            if (read)
                            {
                                // 읽었으면 내용을 모두 삭제
                                StreamWriter writer = new StreamWriter(strTempLogFilePath);
                                writer.Close();
                            }
                        }
                    }
                    else
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
                                    FireManagement.FormMain2.Instance.Invoke(SendMessage, strTagUID);
                                    m_owner.OnReadTag(m_strTagUID);
                                }
                            }
                        }

                        UbistsLib.DeleteAllDB(m_hUbistsHandle);
                    }
                    Thread.Sleep(200);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
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