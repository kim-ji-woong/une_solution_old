using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility;

namespace DidDisasterViewer
{
    public enum Mode { Normal, Fire, Psm }
    public partial class FormMain : Form
    {
        //private WebDBManager m_dbMgr = null;
        //private int m_nSiteID = -1;
        private Timer m_timer = null;
        //private Mode m_curMode = Mode.Normal;

        private Image m_img1 = global::DidDisasterViewer.Properties.Resources.wait;
        private Image m_img2 = global::DidDisasterViewer.Properties.Resources.fire;
        private Image m_img3 = global::DidDisasterViewer.Properties.Resources.leack;
        private int m_nRefreshSec = 5;

        public FormMain()
        {
            InitializeComponent();

            m_nRefreshSec = DidDisasterViewer.Properties.Settings.Default.Sec;

            this.Size = new Size(1920, 1080);
            this.Location = new Point(0, 0);
            this.pictureBox1.Parent = this;
            this.pictureBox1.Size = new Size(1920, 1080);
            this.pictureBox1.Location = new Point(0, 0);
            this.pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

            this.pictureBox1.Image = m_img1;            

            //m_nSiteID = ReadSiteID();
            //m_dbMgr = new WebDBManager(m_nSiteID);

            m_timer = new Timer();
            m_timer.Interval = 1000;
            m_timer.Tick += M_timer_Tick;
            m_timer.Start();

            this.pictureBox1.PreviewKeyDown += PictureBox1_PreviewKeyDown;
            this.KeyUp += FormMain_KeyUp;
        }

        private void FormMain_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.m_timer.Stop();
                this.m_timer.Dispose();
                this.Close();
            }
        }

        private void PictureBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private int ReadSiteID()
        {
            Utility util = new Utility();
            string szSiteID = util.getinivalue("Server Connection Info", "siteid");
            if (szSiteID != null && szSiteID.Length > 0)
            {
                int nSiteId = 1;
                if (int.TryParse(szSiteID, out nSiteId))
                    return nSiteId;
            }
            return -1;
        }

        private void RefreshDisaster()
        {
            //StringBuilder sb = new StringBuilder();
            //sb.Append("SELECT* FROM ");
            //sb.Append("( ");
            ////sb.Append("SELECT srh.id, srh.SensorHistoryID, srh.ReactionType, srh.Time, srh.Message, srh.Param1, srh.Param2, srh.Param3, srh.Param4, srh.Param5, szh.SensorID ");
            //sb.Append("SELECT srh.ReactionType ");
            //sb.Append("  FROM SensorReactionHistory as srh INNER JOIN  SensorZoneHistory as szh on srh.SensorHistoryID = szh.ID ");
            //sb.Append(" WHERE SensorHistoryID in (SELECT srh2.SensorHistoryID ");
            //sb.Append("                             FROM SensorReactionHistory as srh2 ");
            //sb.Append("                            WHERE srh2.ReactionType in (0, 60, 62, 898, 899, 921, 961))   ");
            //sb.Append("   AND SensorHistoryID not in (SELECT srh3.SensorHistoryID ");
            //sb.Append("                                 FROM SensorReactionHistory as srh3 ");
            //sb.Append("                                WHERE srh3.ReactionType in (21, 23, 33, 50, 61, 70, 919, 920, 939, 940, 969, 970, 1000))   ");
            //sb.Append("   AND szh.SiteID = " + m_nSiteID);
            //sb.Append(" ORDER BY srh.Time DESC, szh.SensorID ");
            //sb.Append(" ) as tt WHERE ReactionType IN (0, 60) limit 1");

            //ArrayList arrResult = m_dbMgr.GetResultData(sb.ToString(), 0);

            //if (arrResult == null)
            //    return;

            //int nResultCount = arrResult.Count;
            //if (nResultCount == 0)
            //{
            //    m_curMode = Mode.Normal;
            //    return;
            //}

            //int nReactionType = WebDBManager.GetIntField(arrResult[0].ToString(), -1);

            //if (nReactionType == 0) // 화재
            //{
            //    m_curMode = Mode.Fire;
            //}
            //else if (nReactionType == 60) // 누출
            //{
            //    m_curMode = Mode.Psm;
            //}
        }

        private int m_curSec = 1;        

        private void M_timer_Tick(object sender, EventArgs e)
        {
            m_curSec++;
            if (m_curSec == m_nRefreshSec * 1)
            {
                this.pictureBox1.Image = m_img1;
            }
            else if (m_curSec == m_nRefreshSec * 2)
            {
                this.pictureBox1.Image = m_img2;
            }
            else if (m_curSec == m_nRefreshSec * 3)
            {
                this.pictureBox1.Image = m_img3;
                m_curSec = 0;
            }
            //RefreshDisaster();

            //if (m_curMode == Mode.Normal)
            //{
            //    this.pictureBox1.Image = m_img1;
            //}
            //else if (m_curMode == Mode.Fire)
            //{
            //    this.pictureBox1.Image = m_img2;
            //}
            //else if (m_curMode == Mode.Psm)
            //{
            //    this.pictureBox1.Image = m_img3;
            //}


        }
    }
}
