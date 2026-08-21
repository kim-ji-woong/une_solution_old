using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using SDMS.Model.CCTV;
using dnsData.Sensor;
//using System.Threading;

namespace SVMSServer
{
    public class TrayManager : ISVMSEventOwner
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct PointInter
        {
            public int X;
            public int Y;
            public static explicit operator Point(PointInter point)
            {
                return new Point(point.X, point.Y);
            }
        }

        private class Message
        {
            public DateTime EventTime;
            public string UniqueKey;
            public Facility.FacilityType SensorType;
            public string MessageString;

            public Message()
            {
            }

            public Message(DateTime eventTime, string uniqueKey, Facility.FacilityType sensorType, string message)
            {
                EventTime = eventTime;
                UniqueKey = uniqueKey;
                SensorType = sensorType;
                MessageString = message;
            }
        }

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out PointInter lpPoint);

        private NotifyIcon m_icon = null;
        private ContextMenuStrip m_contextMenu = null;
        private System.ComponentModel.IContainer components;

        private System.Windows.Forms.ToolStripMenuItem tsMenuClose;
        private System.Windows.Forms.ToolStripMenuItem tsMenuReload;
        private Timer m_timer = null;

        private List<SVMSEventReceiver> m_svmsEventReceivers = null;
        private List<SVMSEventReceiver> m_svmsTempEventReceivers = null;
        private CCTVManager m_cctvManager = null;
        private AlarmManager m_alarmManager = null;
        private DateTime? m_dtLastChanged = null;

        private System.Collections.Concurrent.ConcurrentQueue<Message> m_messageQueues = new System.Collections.Concurrent.ConcurrentQueue<Message>();
        private bool m_closeThread = false;

        // 마지막에 CCTV List를 확인한 날짜
        private DateTime m_dtLastChecked;

        public TrayManager()
        {
            CreateNotifyicon();
            m_dtLastChecked = DateTime.Now;

            m_svmsEventReceivers = SVMSEventReceiver.MakeInstances(this);
            //m_svmsEventReceiver = new SVMSEventReceiver(this);

            if (m_svmsEventReceivers != null && m_svmsEventReceivers.Count > 0)
            {
                bool isFirst = true;
                
                foreach (SVMSEventReceiver receiver in m_svmsEventReceivers)
                {
                    if (isFirst)
                    {
                        m_cctvManager = new CCTVManager(receiver.DataManager, receiver.CommonDataManager);
                        m_alarmManager = new AlarmManager(receiver.DataManager, receiver.CommonDataManager);
                    }
                    else
                        isFirst = false;

                    receiver.ConnectServer();
                }

                m_cctvManager.RestartProcess();

                m_timer = new Timer();
                // 1초 주기
                m_timer.Interval = 1000;
                m_timer.Tick += OnTimer;
                m_timer.Start();

                OnTimer(null, null);

                System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(MessageThread));
                t.Start();
            }
        }

        private void MessageThread()
        {
            Message message;

            while (m_closeThread == false)
            {
                if (m_messageQueues.TryDequeue(out message))
                {
                    m_cctvManager.SendEvent(message.EventTime, message.UniqueKey, message.SensorType);
                }

                System.Threading.Thread.Sleep(100);
            }
        }

        private void OnTimer(object sender, EventArgs e)
        {
            DateTime dtNow = DateTime.Now;
            DateTime? dtLastChanged = m_dtLastChanged;

            if (dtLastChanged != null)
            {
                TimeSpan span = dtNow - (DateTime)dtLastChanged;

                if (span.TotalMinutes >= 1.0)
                {
                    // 마지막 변경 이후로 1분 이상 지났다면...
                    m_dtLastChanged = null;

                    // svms로부터 받아야 한다.
                    ICollection<CCTV> svmsCCTVs = null;

                    if (m_svmsTempEventReceivers != null)
                    {
                        svmsCCTVs = SVMSEventReceiver.GetCCTVList(m_svmsTempEventReceivers);

                        WriteCCTVListLog("[TempEventReceiver List]", svmsCCTVs);

                        SVMSEventReceiver.DisposeInstances(m_svmsTempEventReceivers);
                        m_svmsTempEventReceivers = null;
                    }
                    else
                        svmsCCTVs = SVMSEventReceiver.GetCCTVList(m_svmsEventReceivers);

                    if (svmsCCTVs != null)
                    {
                        //ICollection<CCTV> svmsCCTVs = m_svmsEventReceiver.GetCCTVList();
                        m_cctvManager.Update(svmsCCTVs);
                    }
                }
            }

            m_alarmManager.CheckAutoClose();
            Logger.Instance.RemoveOldLogs();

            if (dtNow.Hour >= 1)
            {
                if (dtNow.Year != m_dtLastChecked.Year || dtNow.Month != m_dtLastChecked.Month || dtNow.Day != m_dtLastChecked.Day)
                {
                    m_dtLastChecked = dtNow;
                    // 변경된 CCTV List가 있는지 확인한다.
                    ReloadCCTVList();
                }
            }
        }

        private void CreateNotifyicon()
        {
            this.components = new System.ComponentModel.Container();
            this.m_contextMenu = new System.Windows.Forms.ContextMenuStrip();

            this.m_contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsMenuClose = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuReload = new System.Windows.Forms.ToolStripMenuItem();

            // Initialize contextMenu1
            this.m_contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuClose});
            this.m_contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuReload});
            this.m_contextMenu.Size = new System.Drawing.Size(181, 140);

            // Create the NotifyIcon.
            this.m_icon = new System.Windows.Forms.NotifyIcon(this.components);

            // The Icon property sets the icon that will appear
            // in the systray for this application.
            m_icon.Icon = global::SVMSServer.Properties.Resources.SDMS_BLUE;

            // The ContextMenu property sets the menu that will
            // appear when the systray icon is right clicked.
            m_icon.ContextMenuStrip = this.m_contextMenu;

            // The Text property sets the text that will be displayed,
            // in a tooltip, when the mouse hovers over the systray icon.
            m_icon.Text = "SVMSServer";
            m_icon.Visible = true;

            // Handle the DoubleClick event to activate the form.
            m_icon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.trayIcon_MouseClick);

            // 
            // tsMenuClose
            // 
            this.tsMenuClose.Name = "tsMenuClose";
            this.tsMenuClose.Size = new System.Drawing.Size(180, 22);
            this.tsMenuClose.Text = "종료";
            this.tsMenuClose.Click += new System.EventHandler(this.tsMenuClose_Click);

            // 
            // tsMenuReload
            // 
            this.tsMenuReload.Name = "tsMenuReload";
            this.tsMenuReload.Size = new System.Drawing.Size(180, 22);
            this.tsMenuReload.Text = "다시 불러오기";
            this.tsMenuReload.Click += new System.EventHandler(this.tsMenuReload_Click);
        }

        private void tsMenuClose_Click(object sender, EventArgs e)
        {
            m_closeThread = true;
            Application.Exit();
        }

        private void tsMenuReload_Click(object sender, EventArgs e)
        {
            Logger.Instance.Write("tsMenuReload_Click");

            ReloadCCTVList();
        }

        private void trayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                m_contextMenu.Show();
        }

        public void OnMessage(DateTime eventTime, string uniqueKey, Facility.FacilityType sensorType, string strMessage)
        {
            if (sensorType != Facility.FacilityType.NONE)
            {
                m_messageQueues.Enqueue(new Message(eventTime, uniqueKey, sensorType, strMessage));
                //MessageThread에서 처리
                //m_cctvManager.SendEvent(eventTime, uniqueKey, sensorType);
            }
        }

        public void OnModifiedCamera(CCTV cctv)
        {
            m_dtLastChanged = DateTime.Now;
            System.Diagnostics.Trace.WriteLine("OnModifiedCamera : " + cctv.CameraName);
        }

        public void OnAddCCTV(CCTV cctv)
        {
            m_dtLastChanged = DateTime.Now;
            System.Diagnostics.Trace.WriteLine("OnAddCCTV : " + cctv.CameraName);
        }

        private void ReloadCCTVList()
        {
            //if (m_svmsEventReceivers != null)
            //{
            //    m_svmsTempEventReceivers = SVMSEventReceiver.CloneInstances(m_svmsEventReceivers);

            //    foreach (SVMSEventReceiver receiver in m_svmsTempEventReceivers)
            //    {
            //        receiver.ConnectServer();
            //    }
            //}
            Logger.Instance.Write("ReloadCCTVList");

            // 프로그램 종료 >> 감시 프로세스에서 재시작하여 CCTV List 다시 불러오기
            m_closeThread = true;
            Application.Exit();

        }

        private void WriteCCTVListLog(string strTag, ICollection<CCTV> cctvs)
        {
            Logger.Instance.Write(strTag);

            if (cctvs == null)
            {
                Logger.Instance.Write("CCTV List is null");
                return;
            }

            Logger.Instance.Write("CCTV Count : " + cctvs.Count);

            foreach (CCTV cctv in cctvs)
            {
                Logger.Instance.Write(string.Format("{0} : {1}, {2}", cctv.UniqueKey, cctv.CameraName, cctv.URL));
            }
        }
    }
}
