using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Concurrent;
using System.IO;
using System.Collections;

namespace FireSimulator
{
    public class TrayManager : IListener
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

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out PointInter lpPoint);

        private NotifyIcon m_icon = null;
        private ContextMenuStrip m_contextMenu = null;
        private System.ComponentModel.IContainer components;

        private System.Windows.Forms.ToolStripMenuItem tsMenuClose;
        private System.Windows.Forms.ToolStripMenuItem tsMenuOpen;

        private string m_strXMLFile = "data.xml";
        private Project m_project = null;
        private Level m_selectedLevel = null;
        private Space m_spaceFire = null;
        private Space m_spaceEarthquake4 = null;
        private Space m_spaceEarthquake5 = null;
        private Space m_spaceSequrity = null;
        private Space m_spaceFineDust1 = null;
        private Space m_spaceFineDust2 = null;

        //private ConcurrentDictionary<Alarm, Alarm> m_alarms = new ConcurrentDictionary<Alarm, Alarm>();

        private NetworkServer m_netServer = null;
        //private StreamWriter m_writer = new StreamWriter("FireSimulator.log", false, Encoding.UTF8);

        private FormMain m_formMain = null;

        private void WriteLog(string strLog)
        {
            //m_writer.WriteLine(strLog);
            //m_writer.Flush();
        }

        public TrayManager(string[] args)
        {
            CreateNotifyicon();

            //if (File.Exists(m_strXMLFile) == false)
            //    m_strXMLFile = "XML/" + m_strXMLFile;

            //if (File.Exists(m_strXMLFile))
            //{
            //    Project project = ReadXML(m_strXMLFile);

            //    if (project != null)
            //    {
            //        SetProject(project);
            //    }
            //}

            //m_netServer = new NetworkServer(this);

            bool bChk = false;

            if (args.Count() == 1)
            {
                bool.TryParse(args[0], out bChk);
            }

            if (bChk)
                m_formMain = new FormMain(bChk);
            else
                m_formMain = new FormMain();

        }

        private Project ReadXML(string strPath)
        {
            StreamReader reader = new StreamReader(strPath, Encoding.UTF8);
            string strXML = reader.ReadToEnd();
            reader.Close();

            XElement xml = XElement.Parse(strXML);

            if (xml.Name != "IndoorModelFile")
                return null;

            return Project.Read(xml);
        }

        private void SetProject(Project project)
        {
            m_project = project;

            foreach (Level level in m_project.Levels)
            {
                m_selectedLevel = level;
                
                foreach (Space space in level.Spaces)
                {
                    if (m_spaceFire == null)
                        m_spaceFire = space;
                    else if (m_spaceEarthquake4 == null)
                        m_spaceEarthquake4 = space;
                    else if (m_spaceEarthquake5 == null)
                        m_spaceEarthquake5 = space;
                    else if (m_spaceSequrity == null)
                        m_spaceSequrity = space;
                    else if (m_spaceFineDust1 == null)
                        m_spaceFineDust1 = space;
                    else if (m_spaceFineDust2 == null)
                        m_spaceFineDust2 = space;
                    else
                        break;
                }

                break;
            }
        }

        private void CreateNotifyicon()
        {
            this.components = new System.ComponentModel.Container();
            this.m_contextMenu = new System.Windows.Forms.ContextMenuStrip();

            this.m_contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsMenuClose = new System.Windows.Forms.ToolStripMenuItem();
            this.tsMenuOpen = new System.Windows.Forms.ToolStripMenuItem();

            // Initialize contextMenu1
            this.m_contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuOpen});
            this.m_contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsMenuClose});
            this.m_contextMenu.Size = new System.Drawing.Size(181, 70);

            // Create the NotifyIcon.
            this.m_icon = new System.Windows.Forms.NotifyIcon(this.components);

            // The Icon property sets the icon that will appear
            // in the systray for this application.
            m_icon.Icon = (Icon)typeof(Form).GetProperty("DefaultIcon", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).GetValue(null, null);
            //m_icon.Icon = global::SOPWebServer.Properties.Resources.SDMS_BLUE;

            // The ContextMenu property sets the menu that will
            // appear when the systray icon is right clicked.
            m_icon.ContextMenuStrip = this.m_contextMenu;

            // The Text property sets the text that will be displayed,
            // in a tooltip, when the mouse hovers over the systray icon.
            m_icon.Text = "SOP Mini Server";
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
            // tsMenuOpen
            // 
            this.tsMenuOpen.Name = "tsMenuOpen";
            this.tsMenuOpen.Size = new System.Drawing.Size(180, 22);
            this.tsMenuOpen.Text = "열기";
            this.tsMenuOpen.Click += new System.EventHandler(this.tsMenuOpen_Click);
        }

        private void tsMenuClose_Click(object sender, EventArgs e)
        {
            m_formMain.FormClosed();

            Application.Exit();
        }

        private void tsMenuOpen_Click(object sender, EventArgs e)
        {
            if (m_formMain == null)
                m_formMain = new FormMain();

            m_formMain.Visible = true;
        }

        private void trayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                m_contextMenu.Show();
        }

        public void ProcessAlarm(int nHeader, ArrayList arrDatas)
        {
            WriteLog("ProcessAlarm, nHeader : " + nHeader.ToString());

            Alarm alarm = new Alarm();
            alarm.Level = m_selectedLevel;
            alarm.TimeStamp = DateTime.Now;

            if (nHeader == TCP_ID.REPORT_FIRE)
            {
                if (m_spaceFire == null)
                {
                    WriteLog("m_spaceFire is null");
                    return;
                }

                alarm.Space = m_spaceFire;
            }
            //else if (nHeader == TCP_ID.REPORT_EARTHQUAKE4)
            //{
            //    if (m_spaceEarthquake4 == null)
            //    {
            //        WriteLog("m_spaceEarthquake4 is null");
            //        return;
            //    }

            //    alarm.Space = m_spaceEarthquake4;
            //}
            else if (nHeader == TCP_ID.REPORT_EARTHQUAKE5)
            {
                if (m_spaceEarthquake5 == null)
                {
                    WriteLog("m_spaceEarthquake5 is null");
                    return;
                }

                alarm.Space = m_spaceEarthquake5;
            }
            else if (nHeader == TCP_ID.REPORT_SEQURITY)
            {
                if (m_spaceSequrity == null)
                {
                    WriteLog("m_spaceSequrity is null");
                    return;
                }

                alarm.Space = m_spaceSequrity;
            }
            else if (nHeader == TCP_ID.REPORT_FINEDUST1)
            {
                if (m_spaceFineDust1 == null)
                {
                    WriteLog("m_spaceFineDust1 is null");
                    return;
                }

                alarm.Space = m_spaceFineDust1;
            }
            else if (nHeader == TCP_ID.REPORT_FINEDUST2)
            {
                if (m_spaceFineDust2 == null)
                {
                    WriteLog("m_spaceFineDust2 is null");
                    return;
                }

                alarm.Space = m_spaceFineDust2;
            }
            else
                return;

            WriteLog("ProcessAlarm : " + nHeader.ToString());
            m_netServer.SendAlarm(alarm, m_project, TCP_ID.REPORT_FIRE);
        }

        public void ClearAlarm(int nHeader, ArrayList arrDatas)
        {
            WriteLog("ClearAlarm, nHeader : " + nHeader.ToString());

            Alarm alarm = new Alarm();
            alarm.Level = m_selectedLevel;
            alarm.TimeStamp = DateTime.Now;

            if (nHeader == TCP_ID.CLEAR_FIRE)
            {
                if (m_spaceFire == null)
                    return;

                alarm.Space = m_spaceFire;
            }
            //else if (nHeader == TCP_ID.CLEAR_EARTHQUAKE4)
            //{
            //    if (m_spaceEarthquake4 == null)
            //        return;

            //    alarm.Space = m_spaceEarthquake4;
            //}
            else if (nHeader == TCP_ID.CLEAR_EARTHQUAKE5)
            {
                if (m_spaceEarthquake5 == null)
                    return;

                alarm.Space = m_spaceEarthquake5;
            }
            else if (nHeader == TCP_ID.CLEAR_SEQURITY)
            {
                if (m_spaceSequrity == null)
                    return;

                alarm.Space = m_spaceSequrity;
            }
            else if (nHeader == TCP_ID.CLEAR_FINEDUST1)
            {
                if (m_spaceFineDust1 == null)
                    return;

                alarm.Space = m_spaceFineDust1;
            }
            else if (nHeader == TCP_ID.CLEAR_FINEDUST2)
            {
                if (m_spaceFineDust2 == null)
                    return;

                alarm.Space = m_spaceFineDust2;
            }
            else
                return;

            WriteLog("ClearAlarm : " + nHeader.ToString());
            m_netServer.SendClear(alarm, m_project, TCP_ID.CLEAR_FIRE);
        }
    }

    public interface IListener
    {
        void ProcessAlarm(int nHeader, ArrayList arrDatas);
        void ClearAlarm(int nHeader, ArrayList arrDatas);
    }
}
