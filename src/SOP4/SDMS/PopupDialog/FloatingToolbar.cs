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

namespace SDMS.PopupDialog
{
    public partial class FloatingToolbar : Form
    {
        #region Form 이동
        private bool m_bLeftMouseDown = false;
        private Point m_ptMove = new Point();
        #endregion

        private Point m_ptRelativePosition = new Point();
        public Point RelativePosition
        {
            get { return m_ptRelativePosition; }
        }

        protected int m_nMode = 1;
        public int Mode
        {
            get { return m_nMode; }
            set
            {
                m_nMode = value;
                ArrangeButton(m_nMode);
            }
        }

        public ToolStrip MainToolStrip
        {
            get { return mMainToolStrip;  }
        }

        private Dictionary<string, OptionButton> m_dicOptionButton = new Dictionary<string, OptionButton>();

        private Dictionary<string, OptionToolStripButton> m_dicToolstripOptionButton = new Dictionary<string, OptionToolStripButton>();

        public FloatingToolbar()
        {
            this.DoubleBuffered = true;

            InitializeComponent();

            InitToolbarTOption();
            ArrangeButton(m_nMode);


            // 버튼들의 레이블에 대해서도 마우스로 끌어서 폼을 이동할 수 있도록 이벤트 연결
            foreach (Control ctl in from ctls in this.Controls.Cast<Control>()
                                    where ctls is Label
                                    select ctls
                                    )
            {
                Label lb = ctl as Label;

                lb.MouseDown += FloatingToolbar_MouseDown;
                lb.MouseEnter += FloatingToolbar_MouseEnter;
                lb.MouseLeave += FloatingToolbar_MouseLeave;
                lb.MouseMove += FloatingToolbar_MouseMove;
                lb.MouseUp += FloatingToolbar_MouseUp;
            }

            this.Controls.Remove(mMainToolStrip);
        }

        

        private void InitToolbarTOption()
        {
            // init Button Object
            m_dicOptionButton.Add("초기화면", new OptionButton(btnFullScreen, lblFullScreen, ID.ID_VIEW_HOME_MAIN, false, false));
            m_dicOptionButton.Add("전체화면", new OptionButton(btnHome, lblHome, ID.ID_VIEW_HOME, false, false));
            m_dicOptionButton.Add("선택", new OptionButton(btnPick, lblPick, ID.ID_VIEW_PICK, false, false));
            m_dicOptionButton.Add("이동", new OptionButton(btnPanning, lblPanning, ID.ID_VIEW_PAN, false, false));
            m_dicOptionButton.Add("회전", new OptionButton(btnOrbit, lblOrbit, ID.ID_VIEW_ORBIT, false, false));
            m_dicOptionButton.Add("확대", new OptionButton(btnZoomIn, lblZoomIn, ID.ID_VIEW_ZOOMIN, false, false));
            m_dicOptionButton.Add("축소", new OptionButton(btnZoomOut, lblZoomOut, ID.ID_VIEW_ZOOMOUT, false, false));
            m_dicOptionButton.Add("3D", new OptionButton(btnOutside, lblOutside, ID.ID_VIEW_OUTSIDE, true, false));
            m_dicOptionButton.Add("3D/2D", new OptionButton(btnBoth, lblBoth, ID.ID_VIEW_BOTHSIDE, false, false));
            m_dicOptionButton.Add("2D", new OptionButton(btnInside, lblInside, ID.ID_VIEW_INSIDE, true, false));
            m_dicOptionButton.Add("화면캡쳐", new OptionButton(btnScreenShot, lblScreenShot, ID.ID_VIEW_SCREENSHOT, false, false));
            m_dicOptionButton.Add("기후정보", new OptionButton(btnWeatherInfo, lblWeatherInfo, ID.ID_VIEW_WEATHER_INFO, false, false));
            m_dicOptionButton.Add("테스트", new OptionButton(btnSimulator, lblSimulator, ID.ID_VIEW_SIMULATOR, false, false));
            m_dicOptionButton.Add("CCTV", new OptionButton(btnMultiCCTV, lblMultiCCTV, ID.ID_VIEW_CCTV, false, false));
            m_dicOptionButton.Add("유해물질", new OptionButton(btnPSMStatus, lblPSMStatus, ID.ID_VIEW_PSM, false, false));            
            m_dicOptionButton.Add("방재장비", new OptionButton(btnDisasterPrevention, lblDisasterPrevention, ID.ID_VIEW_DISASTER, false, false));

            m_dicToolstripOptionButton.Add("초기화면", new OptionToolStripButton(mMainToolStrip, tsBtnFullScreen, btnFullScreen, ID.ID_VIEW_HOME_MAIN, false, false));
            m_dicToolstripOptionButton.Add("전체화면", new OptionToolStripButton(mMainToolStrip, tsBtnHome, btnHome, ID.ID_VIEW_HOME, false, false));
            m_dicToolstripOptionButton.Add("선택", new OptionToolStripButton(mMainToolStrip, tsBtnPick, btnPick, ID.ID_VIEW_PICK, false, false));
            m_dicToolstripOptionButton.Add("이동", new OptionToolStripButton(mMainToolStrip, tsBtnPanning, btnPanning, ID.ID_VIEW_PAN, false, false));
            m_dicToolstripOptionButton.Add("회전", new OptionToolStripButton(mMainToolStrip, tsBtnOrbit, btnOrbit, ID.ID_VIEW_ORBIT, false, false));
            m_dicToolstripOptionButton.Add("확대", new OptionToolStripButton(mMainToolStrip, tsBtnZoomIn, btnZoomIn, ID.ID_VIEW_ZOOMIN, false, false));
            m_dicToolstripOptionButton.Add("축소", new OptionToolStripButton(mMainToolStrip, tsBtnZoomOut, btnZoomOut, ID.ID_VIEW_ZOOMOUT, false, false));
            m_dicToolstripOptionButton.Add("3D", new OptionToolStripButton(mMainToolStrip, tsBtnOutside, btnOutside, ID.ID_VIEW_OUTSIDE, true, false));
            m_dicToolstripOptionButton.Add("3D/2D", new OptionToolStripButton(mMainToolStrip, tsBtnBoth, btnBoth, ID.ID_VIEW_BOTHSIDE, false, false));
            m_dicToolstripOptionButton.Add("2D", new OptionToolStripButton(mMainToolStrip, tsBtnInside, btnInside, ID.ID_VIEW_INSIDE, true, false));
            m_dicToolstripOptionButton.Add("화면캡쳐", new OptionToolStripButton(mMainToolStrip, tsBtnScreenShot, btnScreenShot, ID.ID_VIEW_SCREENSHOT, false, false));
            m_dicToolstripOptionButton.Add("기후정보", new OptionToolStripButton(mMainToolStrip, tsBtnWeatherInfo, btnWeatherInfo, ID.ID_VIEW_WEATHER_INFO, false, false));
            m_dicToolstripOptionButton.Add("테스트", new OptionToolStripButton(mMainToolStrip, tsBtnSensorSimulator, btnSimulator, ID.ID_VIEW_SIMULATOR, false, false));
            m_dicToolstripOptionButton.Add("CCTV", new OptionToolStripButton(mMainToolStrip, tsBtnCCTVList, btnMultiCCTV, ID.ID_VIEW_CCTV, false, false));
            m_dicToolstripOptionButton.Add("유해물질", new OptionToolStripButton(mMainToolStrip, tsBtnPSMList, btnPSMStatus, ID.ID_VIEW_PSM, false, false));            
            m_dicToolstripOptionButton.Add("방재장비", new OptionToolStripButton(mMainToolStrip, tsBtnDisasterPrevention, btnDisasterPrevention, ID.ID_VIEW_DISASTER, false, false));


        //public const int ID_VIEW_HOME_MAIN = 5900;
        //public const int ID_VIEW_HOME_14 = 5901;
        //public const int ID_VIEW_HOME_56 = 5902;
        //public const int ID_VIEW_HOME_COAL = 5903;


            발ToolStripMenuItem.Tag = ID.ID_VIEW_HOME_14;
            발ToolStripMenuItem1.Tag = ID.ID_VIEW_HOME_56;
            저탄장ToolStripMenuItem.Tag = ID.ID_VIEW_HOME_COAL;
            전체ToolStripMenuItem.Tag = ID.ID_VIEW_HOME_MAIN;

            tsBtnFullScreen.Click += toolstrip_Click;
            //tsBtnHome.Click += toolstrip_Click;
            tsBtnPick.Click += toolstrip_Click;
            tsBtnPanning.Click += toolstrip_Click;
            tsBtnOrbit.Click += toolstrip_Click;
            tsBtnZoomIn.Click += toolstrip_Click;
            tsBtnZoomOut.Click += toolstrip_Click;
            tsBtnOutside.Click += toolstrip_Click;
            tsBtnBoth.Click += toolstrip_Click;
            tsBtnInside.Click += toolstrip_Click;
            tsBtnScreenShot.Click += toolstrip_Click;

            tsBtnWeatherInfo.Click += toolstrip_Click;
            tsBtnSensorSimulator.Click += toolstrip_Click;
            tsBtnCCTVList.Click += toolstrip_Click;
            tsBtnPSMList.Click += toolstrip_Click;
            tsBtnDisasterPrevention.Click += toolstrip_Click;


            tsBtnFullScreen.MouseEnter += mMainToolStrip_MouseEnter;
            //tsBtnHome.Click += toolstrip_Click;
            tsBtnPick.MouseEnter += mMainToolStrip_MouseEnter;
            tsBtnPanning.MouseEnter += mMainToolStrip_MouseEnter;
            tsBtnOrbit.MouseEnter += mMainToolStrip_MouseEnter;
            tsBtnZoomIn.MouseEnter += mMainToolStrip_MouseEnter;
            tsBtnZoomOut.MouseEnter += mMainToolStrip_MouseEnter;
            tsBtnOutside.MouseEnter += mMainToolStrip_MouseEnter;
            tsBtnBoth.MouseEnter += mMainToolStrip_MouseEnter;
            tsBtnInside.MouseEnter += mMainToolStrip_MouseEnter;
            tsBtnScreenShot.MouseEnter += mMainToolStrip_MouseEnter;
            tsBtnWeatherInfo.MouseEnter += mMainToolStrip_MouseEnter;
            tsBtnSensorSimulator.MouseEnter += mMainToolStrip_MouseEnter;
            tsBtnCCTVList.MouseEnter += mMainToolStrip_MouseEnter;
            tsBtnPSMList.MouseEnter += mMainToolStrip_MouseEnter;
            tsBtnDisasterPrevention.MouseEnter += mMainToolStrip_MouseEnter;


            // Option Read
            string strSQL = String.Format("SELECT PropertyValue FROM OptionSDMS WHERE PropertyName = 'ToolbarOption' AND SiteID = {0}", UnE.SOP.ProxySOP.Instance.SiteID);
            ArrayList arrResult = FormMain.Instance.DBManager.GetResultData(strSQL, 0);

            if (arrResult != null)
            {
                for (int i = 0; i < arrResult.Count; i++)
                {
                    string strOptionString = DBUtility.WebDBManager.GetStringField(arrResult[i]);

                    int nVisibleIndex = 0;

                    foreach (string strOption in strOptionString.Split(','))
                    {
                        nVisibleIndex++;

                        string strOptionName = strOption.Trim().Split('(')[0].Trim();
                        bool isVisible = strOption.Trim().Split('(')[1].Trim().Replace(")", "") != "0";

                        if (m_dicOptionButton.ContainsKey(strOptionName))
                        {
                            m_dicOptionButton[strOptionName].Visiblilty = isVisible;
                            m_dicOptionButton[strOptionName].VisibleIndex = nVisibleIndex;
                        }

                        if (m_dicToolstripOptionButton.ContainsKey(strOptionName))
                        {
                            m_dicToolstripOptionButton[strOptionName].Visiblilty = isVisible;
                            m_dicToolstripOptionButton[strOptionName].VisibleIndex = nVisibleIndex;
                        }
                    }
                }
            }

        }

        private void toolstrip_Click(object sender, EventArgs e)
        {
            ToolStripItem btn = (ToolStripItem)sender;

            if (btn.Tag == null)
                return;

            FormMain.Instance.OnClickToolbarButton((int)btn.Tag);
        }

        private void btn_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Tag == null)
                return;

            FormMain.Instance.OnClickToolbarButton((int)btn.Tag);
        }

        private void FloatingToolbar_Shown(object sender, EventArgs e)
        {
            SetRelativePosition();
        }

        private void FloatingToolbar_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void FloatingToolbar_MouseMove(object sender, MouseEventArgs e)
        {
            ProcessMouseMove(e, Control.MousePosition);
        }

        private void FloatingToolbar_MouseDown(object sender, MouseEventArgs e)
        {
            ProcessMouseDown(e, Control.MousePosition);
        }

        private void FloatingToolbar_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = false;
                SetRelativePosition();
            }
        }

        private void FloatingToolbar_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Arrow;
        }

        protected void ProcessMouseDown(MouseEventArgs e, Point ptMouse)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_bLeftMouseDown = true;
                m_ptMove = ptMouse;
            }
        }

        protected void ProcessMouseMove(MouseEventArgs e, Point ptMouse)
        {
            if (!m_bLeftMouseDown)
                return;

            Point ptScreen = ptMouse;

            int dx = ptScreen.X - m_ptMove.X;
            int dy = ptScreen.Y - m_ptMove.Y;

            if (dx == 0 && dy == 0)
                return;

            Point ptCur = this.Location;
            int x = ptCur.X + dx;
            int y = ptCur.Y + dy;

            // Toolbar가 FormMain 영역 밖으로 나가지 못하도록 한다.
            if (x < FormFrame.Instance.Location.X)
                x = FormFrame.Instance.Location.X;
            if (y < FormFrame.Instance.Location.Y)
                y = FormFrame.Instance.Location.Y;

            if (x + this.Size.Width > FormFrame.Instance.Location.X + FormFrame.Instance.Size.Width)
                x = FormFrame.Instance.Location.X + FormFrame.Instance.Size.Width - this.Size.Width;
            if (y + this.Size.Height > FormFrame.Instance.Location.Y + FormFrame.Instance.Size.Height)
                y = FormFrame.Instance.Location.Y + FormFrame.Instance.Size.Height - this.Size.Height;
            //////////////////////////////////////////////////////////////////////////////////////////

            this.Location = new Point(x, y);

            m_ptMove.X += dx;
            m_ptMove.Y += dy;
        }

        private void SetRelativePosition()
        {
            m_ptRelativePosition.X = this.Location.X - FormFrame.Instance.Location.X;
            m_ptRelativePosition.Y = this.Location.Y - FormFrame.Instance.Location.Y;
        }

        public void CheckButton(int nButtonID, bool isChecked)
        {
            ToolStripButton  tsbtn = GetToolstripgButton(nButtonID);
            Button btn = GetButton(nButtonID);

            if (btn != null)
                FormMain.Instance.CheckButton(btn, isChecked);

            if( tsbtn != null)
                tsbtn.Checked = isChecked;
        }

        private void ArrangeButton(int nMode)
        {
            int nIndex = 0;

            foreach (OptionToolStripButton item in from items in m_dicToolstripOptionButton.Values.AsEnumerable()
                                          where items.ToolStripButton != null
                                          orderby items.VisibleIndex ascending
                                          select items
                                         )
            {
                

                item.ApplyVisiblity((nMode == 2));

                if (UnE.SOP.ProxySOP.Instance.ShowCCTVForm == true && item.IsCCTVType == true)
                    continue;

                item.ApplyControlLocation(ref nIndex);
            }

            nIndex = 0;
            foreach (OptionButton item in from items in m_dicOptionButton.Values.AsEnumerable()
                                                   where items.Visiblilty == true
                                                   orderby items.VisibleIndex ascending
                                                   select items
                                         )
            {


                item.ApplyVisiblity((nMode == 2));

                if (UnE.SOP.ProxySOP.Instance.ShowCCTVForm == true && item.IsCCTVType == true)
                    continue;

                item.ApplyControlLocation(ref nIndex);
            }

            this.Size = new Size(25 + (48 * nIndex), this.Height);
            return;
        }

        public new void Show(IWin32Window owner)
        {
            //base.Show(owner);
        }

        public ToolStripButton GetToolstripgButton(int nButtonID)
        {
            if (tsBtnFullScreen.Tag != null && (int)tsBtnFullScreen.Tag == nButtonID)
                return tsBtnFullScreen;

            //if (tsBtnHome.Tag != null && (int)tsBtnHome.Tag == nButtonID)
            //    return tsBtnHome;

            if (tsBtnPick.Tag != null && (int)tsBtnPick.Tag == nButtonID)
                return tsBtnPick;

            if (tsBtnPanning.Tag != null && (int)tsBtnPanning.Tag == nButtonID)
                return tsBtnPanning;

            if (tsBtnOrbit.Tag != null && (int)tsBtnOrbit.Tag == nButtonID)
                return tsBtnOrbit;

            if (tsBtnZoomIn.Tag != null && (int)tsBtnZoomIn.Tag == nButtonID)
                return tsBtnZoomIn;

            if (tsBtnZoomOut.Tag != null && (int)tsBtnZoomOut.Tag == nButtonID)
                return tsBtnZoomOut;

            if (tsBtnOutside.Tag != null && (int)tsBtnOutside.Tag == nButtonID)
                return tsBtnOutside;

            if (tsBtnBoth.Tag != null && (int)tsBtnBoth.Tag == nButtonID)
                return tsBtnBoth;

            if (tsBtnInside.Tag != null && (int)tsBtnInside.Tag == nButtonID)
                return tsBtnInside;

            if (tsBtnCCTVList.Tag != null && (int)tsBtnCCTVList.Tag == nButtonID)
                return tsBtnCCTVList;

            if (tsBtnScreenShot.Tag != null && (int)tsBtnScreenShot.Tag == nButtonID)
                return tsBtnScreenShot;

            if (tsBtnWeatherInfo.Tag != null && (int)tsBtnWeatherInfo.Tag == nButtonID)
                return tsBtnWeatherInfo;

            if (tsBtnSensorSimulator.Tag != null && (int)tsBtnSensorSimulator.Tag == nButtonID)
                return tsBtnSensorSimulator;

            if (tsBtnPSMList.Tag != null && (int)tsBtnPSMList.Tag == nButtonID)
                return tsBtnPSMList;

            if (tsBtnDisasterPrevention.Tag != null && (int)tsBtnDisasterPrevention.Tag == nButtonID)
                return tsBtnDisasterPrevention;

            return null;
        }

        public Button GetButton(int nButtonID)
        {
            if (btnFullScreen.Tag != null && (int)btnFullScreen.Tag == nButtonID)
                return btnFullScreen;

            if (btnHome.Tag != null && (int)btnHome.Tag == nButtonID)
                return btnHome;

            if (btnPick.Tag != null && (int)btnPick.Tag == nButtonID)
                return btnPick;

            if (btnPanning.Tag != null && (int)btnPanning.Tag == nButtonID)
                return btnPanning;

            if (btnOrbit.Tag != null && (int)btnOrbit.Tag == nButtonID)
                return btnOrbit;

            if (btnZoomIn.Tag != null && (int)btnZoomIn.Tag == nButtonID)
                return btnZoomIn;

            if (btnZoomOut.Tag != null && (int)btnZoomOut.Tag == nButtonID)
                return btnZoomOut;

            if (btnOutside.Tag != null && (int)btnOutside.Tag == nButtonID)
                return btnOutside;

            if (btnBoth.Tag != null && (int)btnBoth.Tag == nButtonID)
                return btnBoth;

            if (btnInside.Tag != null && (int)btnInside.Tag == nButtonID)
                return btnInside;

            if (btnMultiCCTV.Tag != null && (int)btnMultiCCTV.Tag == nButtonID)
                return btnMultiCCTV;

            if (btnScreenShot.Tag != null && (int)btnScreenShot.Tag == nButtonID)
                return btnScreenShot;

            if (btnWeatherInfo.Tag != null && (int)btnWeatherInfo.Tag == nButtonID)
                return btnWeatherInfo;

            if (btnSimulator.Tag != null && (int)btnSimulator.Tag == nButtonID)
                return btnSimulator;

            if (btnPSMStatus.Tag != null && (int)btnPSMStatus.Tag == nButtonID)
                return btnPSMStatus;

            if (btnDisasterPrevention.Tag != null && (int)btnDisasterPrevention.Tag == nButtonID)
                return btnDisasterPrevention;

            return null;
        }

        private class OptionToolStripButton
        {
            private ToolStripItem m_button = null;
            public ToolStripItem ToolStripButton
            {
                get { return m_button; }
            }

            private Button m_btn = null;
            public Button Button
            {
                get { return m_btn; }
                set { m_btn = value; }
            }

            private int m_ID = -1;
            public int ID
            {
                get { return m_ID; }
                private set
                {
                    m_ID = value;
                    m_button.Tag = ID;
                }
            }

            private int m_nVisibleIndex = -1;
            public int VisibleIndex
            {
                get { return m_nVisibleIndex; }
                set { m_nVisibleIndex = value; }
            }

            private bool m_isVisiblilty = false;
            public bool Visiblilty
            {
                get { return m_isVisiblilty; }
                set { m_isVisiblilty = value; }
            }

            private bool m_isOnlyAdmin = false;
            public bool IsOnlyAdmin { get { return m_isOnlyAdmin; } }

            private bool m_isCCTVType = false;
            public bool IsCCTVType { get { return m_isCCTVType; } }

            private bool m_visible = false;

            private ToolStrip mParent = null;
            public OptionToolStripButton(ToolStrip parent, ToolStripItem tsbtn, Button btn, int id, bool isOnlyAdmin, bool isCCTVType)
            {
                mParent = parent;
                m_button = tsbtn;
                m_btn = btn;
                ID = id;
                m_isOnlyAdmin = isOnlyAdmin;
                m_isCCTVType = isCCTVType;
            }


            public void ApplyVisiblity(bool isAdminMode)
            {
                if ((isAdminMode == false && IsOnlyAdmin == true) ||
                    (IsCCTVType == true && UnE.SOP.ProxySOP.Instance.ShowCCTVForm == true))
                {
                    m_visible = false;

                }
                else
                {
                    m_visible = m_isVisiblilty;
                }

                m_button.Visible = m_visible;
                if(m_visible == false)
                {
                    ToolStrip ts = m_button.GetCurrentParent();
                    if (ts != null)
                    {
                        ts.Items.Remove(m_button);
                    }
                }
            }

            public void ApplyControlLocation(ref int nPositionIndex)
            {
                if (m_visible == false)
                    return;

                ToolStrip ts = m_button.GetCurrentParent();
                try
                {
                    if (ts != null)
                    {
                        ts.Items.Remove(m_button);
                        ts.Items.Insert(nPositionIndex, m_button);
                    }
                    else
                    {
                        mParent.Items.Insert(nPositionIndex, m_button);
                    }
                    nPositionIndex++;
                }
                catch(Exception)
                { }
                
            }
        }

        private class OptionButton
        {
            private static Point m_pDefaultButtonLocation = new Point(23, 0);
            private static Point m_pDefaultLabelLocation = new Point(20, 53);
            private static int m_nButtonSpace = 48;

            private Button m_button = null;
            public Button Button
            {
                get { return m_button; }
            }

            private Label m_label = null;
            public Label Label
            {
                get { return m_label; }
            }

            private int m_ID = -1;
            public int ID
            {
                get { return m_ID; }
                private set
                {
                    m_ID = value;
                    m_button.Tag = ID;
                }
            }

            private int m_nVisibleIndex = -1;
            public int VisibleIndex
            {
                get { return m_nVisibleIndex; }
                set { m_nVisibleIndex = value; }
            }

            private bool m_isVisiblilty = false;
            public bool Visiblilty
            {
                get { return m_isVisiblilty; }
                set { m_isVisiblilty = value; }
            }

            private bool m_isOnlyAdmin = false;
            public bool IsOnlyAdmin { get { return m_isOnlyAdmin; } }

            private bool m_isCCTVType = false;
            public bool IsCCTVType { get { return m_isCCTVType; } }

            private bool m_visible = false;

           
            public OptionButton(Button btn, Label lbl, int id, bool isOnlyAdmin, bool isCCTVType)
            {
 
                m_button = btn;
                m_label = lbl;
                ID = id;
                m_isOnlyAdmin = isOnlyAdmin;
                m_isCCTVType = isCCTVType;
            }


            public void ApplyVisiblity(bool isAdminMode)
            {
                if ((isAdminMode == false && IsOnlyAdmin == true) ||
                    (IsCCTVType == true && UnE.SOP.ProxySOP.Instance.ShowCCTVForm == true))
                {
                    m_visible = false;

                }
                else
                {
                    m_visible = m_isVisiblilty;
                }

                m_button.Visible = m_visible;
                m_label.Visible = m_visible;

            }

            public void ApplyControlLocation(ref int nPositionIndex)
            {
                if (m_visible == false)
                    return;


                Point pButton = new Point(m_pDefaultButtonLocation.X + (m_nButtonSpace * nPositionIndex), m_pDefaultButtonLocation.Y);
                Point plabel = new Point(m_pDefaultLabelLocation.X + (m_nButtonSpace * nPositionIndex), m_pDefaultLabelLocation.Y);

                m_button.Location = pButton;
                m_label.Location = plabel;

                nPositionIndex++;
            }
        }

        private void toolStripButton15_Click(object sender, EventArgs e)
        {

        }

        private void tsBtnFullScreen_Click(object sender, EventArgs e)
        {

        }

        private void tsBtnFullScreen_Click_1(object sender, EventArgs e)
        {

        }

        private void ToolStrip_MouseEnter(object sender, EventArgs e)
        {
            this.mMainToolStrip.Focus();
        }

        private void mMainToolStrip_MouseEnter(object sender, EventArgs e)
        {
            this.mMainToolStrip.Focus();
        }

        private void mMainToolStrip_MouseHover(object sender, EventArgs e)
        {
            this.mMainToolStrip.Focus();
        }

        private void mMainToolStrip_MouseMove(object sender, MouseEventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("TSBtn Mouse Up");
            this.mMainToolStrip.Focus();
        }

        public void SetHomeButtonText(int nIndex, string strText)
        {
            if (nIndex == 1)
                this.전체ToolStripMenuItem.Text = strText;
            else if (nIndex == 2)
                this.발ToolStripMenuItem.Text = strText;
            else if (nIndex == 3)
                this.발ToolStripMenuItem1.Text = strText;
            else if (nIndex == 4)
                this.저탄장ToolStripMenuItem.Text = strText;
        }

        public string GetHomeButtonText(int nIndex)
        {
            if (nIndex == 1)
                return this.전체ToolStripMenuItem.Text;
            else if (nIndex == 2)
                return this.발ToolStripMenuItem.Text;
            else if (nIndex == 3)
                return this.발ToolStripMenuItem1.Text;
            else if (nIndex == 4)
                return this.저탄장ToolStripMenuItem.Text;

            return "";
        }
    }

    public class ToolStripEx : ToolStrip
    {
        public ToolStripEx()
        {
            ToolStripManager.Renderer = new ToolStripProfessionalRenderer(new CustomProfessionalColors());


        }

        class CustomProfessionalColors : ProfessionalColorTable
        {
            public override Color ToolStripGradientBegin
            { get { return Color.FromArgb(227, 226, 226);} }

            public override Color ToolStripGradientMiddle
            {
                get { return Color.FromArgb(227, 226, 226); }
            }


            public override Color ButtonCheckedGradientBegin
            {
                get { return this.ButtonPressedGradientBegin; }
            }

            public override Color ButtonCheckedGradientEnd
            {
                get { return this.ButtonPressedGradientEnd; } 
            }
            public override Color ButtonCheckedGradientMiddle
            {
                get { return this.ButtonPressedGradientMiddle; } 
            }


            public override Color ToolStripGradientEnd
            { 
                get { return Color.FromArgb(227, 226, 226); }
            }

            public override Color MenuStripGradientBegin
            { 
                get { return Color.Salmon; }
            }

            public override Color MenuStripGradientEnd
            { get { return Color.OrangeRed; } }
        }
    }
}
