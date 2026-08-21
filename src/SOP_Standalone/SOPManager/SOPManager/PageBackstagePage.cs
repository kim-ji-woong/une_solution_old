using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Runtime.InteropServices;

namespace SOPManager
{
    public partial class PageBackstagePage : Form
    {
        private PopupUserDisaster m_popupUserDisaster = new PopupUserDisaster();

        private FormBacksatgeButton m_BackstageButton;

        private ArrayList m_arrTeamList = new ArrayList();
        public ArrayList TeamList
        {
            get { return m_arrTeamList; }
            set { m_arrTeamList = value; }
        }

        private ArrayList m_arrEtc1 = new ArrayList();
        public ArrayList Etc1
        {
            get { return m_arrEtc1; }
            set { m_arrEtc1 = value; }
        }

        private ArrayList m_arrEtc2 = new ArrayList();
        public ArrayList Etc2
        {
            get { return m_arrEtc2; }
            set { m_arrEtc2 = value; }
        }

        private ArrayList m_arrSelectedTeam = new ArrayList();
        public ArrayList SelectedTeamList
        {
            get { return m_arrSelectedTeam; }
            set { m_arrSelectedTeam = value; }
        }

        private int m_nUserBtnCount = 1;
        private int m_nExternalBtnCount = 1;

        private ArrayList m_arrNormalTeam = new ArrayList();
        private ArrayList m_arrEmergencyTeam = new ArrayList();

        public PageBackstagePage()
        {
            InitializeComponent();

            InitNormalTeam();
            //InitEmergencyTeam();
            InitUserTeam();
            InitExternalTeam();
            InitImage();

            comboBoxSort.SelectedIndex = 0;

            m_arrNormalTeam.Clear();
            m_arrNormalTeam.AddRange(FormMain.Instance.TemporaryNormalTeam);       
            m_arrEmergencyTeam.Clear();
            m_arrEmergencyTeam.AddRange(FormMain.Instance.TemporaryEmergencyTeam);
        }

        private void NormalTeamSort(int nIndex)
        {
            NormalTeamComparer myComparer = new NormalTeamComparer();

            switch (nIndex)
            {
                case 0:
                    m_arrNormalTeam.Clear();
                    m_arrNormalTeam.AddRange(FormMain.Instance.TemporaryNormalTeam);          
                    break;
                case 1:        
                    myComparer.Direct = true;
                    m_arrNormalTeam.Sort(myComparer);
                    break;
                case 2:
                    myComparer.Direct = false;
                    m_arrNormalTeam.Sort(myComparer);
                    break;
            }
            InitNormalTeam();
        }

        private void EmergencyTeamSort(int nIndex)
        {
            EmergencyTeamComparer myComparer = new EmergencyTeamComparer();

            switch (nIndex)
            {
                case 0:
                    m_arrEmergencyTeam.Clear();
                    m_arrEmergencyTeam.AddRange(FormMain.Instance.TemporaryEmergencyTeam);
                    break;
                case 1:
                    myComparer.Direct = true;
                    m_arrEmergencyTeam.Sort(myComparer);
                    break;
                case 2:
                    myComparer.Direct = false;
                    m_arrEmergencyTeam.Sort(myComparer);
                    break;
            }
            InitEmergencyTeam();
        }

        private void InitNormalTeam()
        {
            panelTeamList.Controls.Clear();

            int nButtonCount = 1;
            Point pt = new Point(3, 3);

            foreach (Data_NormalTeam data in m_arrNormalTeam)
            {
                //TemporaryTeamFullPath path = (TemporaryTeamFullPath)FormMain.Instance.FullPath[nButtonCount-1];
                foreach (TemporaryTeamFullPath path in FormMain.Instance.FullPath)
                {
                    if (data.ID == path.ID)
                    {
                        m_BackstageButton = new FormBacksatgeButton();

                        m_BackstageButton.Location = new Point(3, pt.Y);
                        m_BackstageButton.Size = new System.Drawing.Size(415, 66);
                        m_BackstageButton.TopLevel = false;
                        m_BackstageButton.Parent = this;
                        m_BackstageButton.axBackstageBtnUser.Tag = data.ID;
                        m_BackstageButton.SetButtonCaption2(data.TeamName, path.FullPath);
                        m_BackstageButton.axBackstageBtnUser.Icon = GetListImage();
                        panelTeamList.Controls.Add(m_BackstageButton);

                        m_BackstageButton.Show();
                        m_arrTeamList.Add(m_BackstageButton);

                        pt.Y = m_BackstageButton.Height * nButtonCount + 3 + nButtonCount;
                        nButtonCount++;
                        break;
                    }
                }
            }
        }

        private void InitEmergencyTeam()
        {
            panelTeamList.Controls.Clear();

            int nButtonCount = 1;
            Point pt = new Point(3, 3);

            foreach (Data_EmergencyTeam data in m_arrEmergencyTeam)
            {
                //TemporaryTeamFullPath path = (TemporaryTeamFullPath)FormMain.Instance.FullPath[nButtonCount-1];
                foreach (TemporaryTeamFullPath path in FormMain.Instance.FullPath)
                {
                    if (data.ID == path.ID)
                    {
                        m_BackstageButton = new FormBacksatgeButton();

                        m_BackstageButton.Location = new Point(3, pt.Y);
                        m_BackstageButton.Size = new System.Drawing.Size(415, 66);
                        m_BackstageButton.TopLevel = false;
                        m_BackstageButton.Parent = this;
                        m_BackstageButton.axBackstageBtnUser.Tag = data.ID;
                        m_BackstageButton.SetButtonCaption2(data.TeamName, path.FullPath);
                        m_BackstageButton.axBackstageBtnUser.Icon = GetListImage();
                        panelTeamList.Controls.Add(m_BackstageButton);

                        m_BackstageButton.Show();
                        m_arrTeamList.Add(m_BackstageButton);

                        pt.Y = m_BackstageButton.Height * nButtonCount + 3 + nButtonCount;
                        nButtonCount++;
                    }
                }
            }
        }

        private void InitUserTeam()
        {
            panelEtc1.Controls.Clear();

            //m_nUserBtnCount++;
            //int nHeight = m_BackstageButton.Height * m_nUserBtnCount + 3 + m_nUserBtnCount;
            Point pt = new Point(3, 3);

            foreach (Data_UserDefinedTeam data in FormMain.Instance.UserDefinedTeam)
            {  
                m_BackstageButton = new FormBacksatgeButton();

                m_BackstageButton.Location = new Point(3, pt.Y);
                m_BackstageButton.Size = new System.Drawing.Size(415, 66);
                m_BackstageButton.TopLevel = false;
                m_BackstageButton.Parent = this;
                m_BackstageButton.axBackstageBtnUser.Tag = data.ID;
                m_BackstageButton.SetButtonCaption(data.TeamName/*, path.FullPath*/);
                //m_BackstageButton.axBackstageBtnUser.Icon = FormMain.Instance.GetUserImage();
                m_BackstageButton.TeamType = 3;
                panelEtc1.Controls.Add(m_BackstageButton);

                m_BackstageButton.Show();
                m_arrEtc1.Add(m_BackstageButton);

                pt.Y = m_BackstageButton.Height * m_nUserBtnCount + 3 + m_nUserBtnCount;
                m_nUserBtnCount++;
            }
        }

        private void InitExternalTeam()
        {
            panelEtc2.Controls.Clear();
            panelEtc2.Controls.Add(axBackstageBtnExternal);

            m_nExternalBtnCount++;
            Point pt = new Point(3, 70);

            foreach (Data_ExternalTeam data in FormMain.Instance.ExternalTeam)
            {
                m_BackstageButton = new FormBacksatgeButton();

                m_BackstageButton.Location = new Point(3, pt.Y);
                m_BackstageButton.Size = new System.Drawing.Size(415, 66);
                m_BackstageButton.TopLevel = false;
                m_BackstageButton.Parent = this;
                m_BackstageButton.axBackstageBtnUser.Tag = data.ID;
                m_BackstageButton.SetButtonCaption(data.TeamName);
                //m_BackstageButton.axBackstageBtnUser.Icon = FormMain.Instance.GetUserImage();

                m_BackstageButton.TeamType = 2;
                panelEtc2.Controls.Add(m_BackstageButton);

                m_BackstageButton.Show();
                m_arrEtc2.Add(m_BackstageButton);

                pt.Y = m_BackstageButton.Height * m_nExternalBtnCount + 3 + m_nExternalBtnCount;
                m_nExternalBtnCount++;
            }
        }

        private void InitImage()
        {
            //axBackstageBtnUser.Icon = FormMain.Instance.GetUserImage();

            //string resDir = FormMain.Instance.ResourcePath();

			//FormMain.Instance.AddBitmapFormRes("btnEtc_Accident", new int[] { ID.ID_BUTTON_ACCIDENT });
            
           // FormMain.Instance.axCommandBars.Icons.LoadBitmap(resDir + "btnEtc_Accident.png", new int[] { ID.ID_BUTTON_ACCIDENT }, XtremeCommandBars.XTPImageState.xtpImageNormal);
            //axBackstageBtnAccident.Icon =  FormMain.Instance.axCommandBars.Icons.GetImage(ID.ID_BUTTON_ACCIDENT, 32);

			//FormMain.Instance.AddBitmapFormRes("btnEtc_External", new int[] { ID.ID_BUTTON_EXTERNAL_ENGINE });
            //FormMain.Instance.axCommandBars.Icons.LoadBitmap(resDir + "btnEtc_External.png", new int[] { ID.ID_BUTTON_EXTERNAL_ENGINE }, XtremeCommandBars.XTPImageState.xtpImageNormal);
            //axBackstageBtnExternal.Icon = FormMain.Instance.axCommandBars.Icons.GetImage(ID.ID_BUTTON_EXTERNAL_ENGINE, 32);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetAsyncKeyState(int vKey);

        const int VK_SHIFT = 0x10;
        const int VK_CONTROL = 0x11;
        const int VK_MENU = 0x12;

        private void axBackstageBtnAccident_ClickEvent(object sender, EventArgs e)
        {
            bool isDown = false;

            if (GetAsyncKeyState(VK_CONTROL) == 0)
                isDown = false;
            else
                isDown = true;

            axBackstageBtnAccident.Checked = !axBackstageBtnAccident.Checked;
            if(!isDown)
            {

                for (int i = 0; i < Etc1.Count; i++)
                {
                    FormBacksatgeButton BackstageButton = (FormBacksatgeButton)Etc1[i];
                    BackstageButton.axBackstageBtnUser.Checked = false;
                }
            }

        }

        public void SetTeamLabelText(string strValue)
        {
            LabelTeamList.Text = strValue;

            if (strValue.Substring(0, 2) == "평일")
                InitNormalTeam();
            else
                InitEmergencyTeam();
        }

        private void axBackstageBtnUser_ClickEvent(object sender, EventArgs e)
        {
            m_popupUserDisaster.ChangeTitle(2);
            if (m_popupUserDisaster.ShowDialog() == DialogResult.OK)
            {
                AddDisasterButton(m_popupUserDisaster.DisasterCaption, m_nUserBtnCount, true);
                m_nUserBtnCount++;
            }
        }

        private void axBackstageBtnExternal_ClickEvent(object sender, EventArgs e)
        {
            m_popupUserDisaster.ChangeTitle(3);
            if (m_popupUserDisaster.ShowDialog() == DialogResult.OK)
            {
                AddDisasterButton(m_popupUserDisaster.DisasterCaption, m_nExternalBtnCount, false);
                m_nExternalBtnCount++;
            }
        }

        private void AddDisasterButton(string strValue, int nButtonCount, bool isUser)
        {
            if (strValue == "")
            {
                MessageBox.Show("조직(기관) 이름을 설정하십시오.");
                return;
            }

            bool isCheck = false;
            if (isUser)
            {
                foreach (FormBacksatgeButton btn in m_arrEtc1)
                {
                    string strCaption = FormMain.Instance.ParseCaption(btn.axBackstageBtnUser.Caption);
                    if (strCaption == strValue)
                    {
                        isCheck = true;
                        break;
                    }
                }
            }
            else
            {
                foreach (FormBacksatgeButton btn in m_arrEtc2)
                {
                    string strCaption = FormMain.Instance.ParseCaption(btn.axBackstageBtnUser.Caption);
                    if (strCaption == strValue)
                    {
                        isCheck = true;
                        break;
                    }
                }
            }

            if (!isCheck)
            {
                m_BackstageButton = new FormBacksatgeButton();

                Point pt = SetUserLocation(nButtonCount-1);
                m_BackstageButton.Location = new Point(3, pt.Y);

                m_BackstageButton.Size = new System.Drawing.Size(415, 66);
                m_BackstageButton.TopLevel = false;
                m_BackstageButton.Parent = this;
                m_BackstageButton.SetButtonCaption(strValue);
                //m_BackstageButton.axBackstageBtnUser.Icon = FormMain.Instance.GetUserImage();
                m_BackstageButton.axBackstageBtnUser.Tag = -1;
                
                if (isUser)
                {
                    m_BackstageButton.TeamType = 3;
                    panelEtc1.Controls.Add(m_BackstageButton);
                    m_arrEtc1.Add(m_BackstageButton);
                    m_BackstageButton.AlignButton(panelEtc1);
                }
                else
                {
                    m_BackstageButton.TeamType = 2;
                    panelEtc2.Controls.Add(m_BackstageButton);
                    m_arrEtc2.Add(m_BackstageButton);
                    m_BackstageButton.AlignButton(panelEtc2);
                }
                m_BackstageButton.Show();
            }
            else
            {
                MessageBox.Show("같은 이름의 조직(기관)을 사용할 수 없습니다.");

                if (isUser)
                    m_nUserBtnCount--;
                else
                    m_nExternalBtnCount--;
            }
        }

        private Point SetUserLocation(int nButtonCount)
        {
            Point pt = new Point();
            pt.X = 3;
            if (m_nUserBtnCount == 1)
                pt.Y = m_BackstageButton.Height * nButtonCount + 4;
            else
                pt.Y = m_BackstageButton.Height * nButtonCount + 3 + nButtonCount;

            return pt;
        }

        private XtremeCommandBars.ImageManagerIcon GetListImage()
        {
            //string resDir = FormMain.Instance.ResourcePath();

			//F//ormMain.Instance.AddBitmapFormRes("btnDot", new int[] { ID.ID_BUTTON_TEAMLIST });
            //FormMain.Instance.axCommandBars.Icons.LoadBitmap(resDir + "btnDot.png", new int[] { ID.ID_BUTTON_TEAMLIST }, XtremeCommandBars.XTPImageState.xtpImageNormal);

			XtremeCommandBars.ImageManagerIcon axIcon = null;//FormMain.Instance.axCommandBars.Icons.GetImage(ID.ID_BUTTON_TEAMLIST, 32);

            return axIcon;
        }

        public AxXtremeCommandBars.AxBackstageButton GetAccidentButton()
        {
            return axBackstageBtnAccident;
        }

        public Panel GetPanelTeamList()
        {
            return panelTeamList;
        }

        public Panel GetPanelEtc1()
        {
            return panelEtc1;
        }

        public Panel GetPanelEtc2()
        {
            return panelEtc2;
        }

        public void SelectedTeamButton()
        {
            m_arrSelectedTeam.Clear();

            foreach (FormBacksatgeButton btn in panelTeamList.Controls)
            {
                if (btn.axBackstageBtnUser.Checked)
                {
                    m_arrSelectedTeam.Add(btn.axBackstageBtnUser);
                }
            }

            for (int i = 0; i < panelEtc1.Controls.Count; i++)
            {
//                 if (i < 2)
//                 {
//                     AxXtremeCommandBars.AxBackstageButton btn = (AxXtremeCommandBars.AxBackstageButton)panelEtc1.Controls[i];
//                     if (btn.Checked)
//                     {
//                         m_arrSelectedTeam.Add(btn);
//                     }
//                 }
//                 else
                {
                    FormBacksatgeButton btn = (FormBacksatgeButton)panelEtc1.Controls[i];
                    if (btn.axBackstageBtnUser.Checked)
                    {
                        m_arrSelectedTeam.Add(btn.axBackstageBtnUser);
                    }
                }
            }

            for (int i = 0; i < panelEtc2.Controls.Count; i++)
            {
                if (i < 1)
                {
                    AxXtremeCommandBars.AxBackstageButton btn = (AxXtremeCommandBars.AxBackstageButton)panelEtc2.Controls[i];
                    if (btn.Checked)
                    {
                        m_arrSelectedTeam.Add(btn);
                    }
                }
                else
                {
                    FormBacksatgeButton btn = (FormBacksatgeButton)panelEtc2.Controls[i];
                    if (btn.axBackstageBtnUser.Checked)
                    {
                        m_arrSelectedTeam.Add(btn.axBackstageBtnUser);
                    }
                }
            }
        }

        public void EnabledPage(bool isEnabled)
        {
            panelTeamList.Enabled = isEnabled;
            panelEtc1.Enabled = isEnabled;
            panelEtc2.Enabled = isEnabled;
            axBackstageBtnUser.Enabled = isEnabled;
        }

        private void comboBoxSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cbo = (ComboBox)sender;
            if (FormMain.Instance.GetPageDisaster().IsWeekMode())
                NormalTeamSort(cbo.SelectedIndex);
            else
                EmergencyTeamSort(cbo.SelectedIndex);
        }

    }

    public class NormalTeamComparer : IComparer
    {
        bool direct = true;

        public bool Direct
        {
            get { return direct; }
            set { direct = value; }
        }

        int IComparer.Compare(Object x, Object y)
        {
            Data_NormalTeam a = (Data_NormalTeam)x;
            Data_NormalTeam b = (Data_NormalTeam)y;
            if (Direct)
            {
                return a.TeamName.CompareTo(b.TeamName);
            }
            else
            {
                return b.TeamName.CompareTo(a.TeamName);
            }
        }
    }

    public class EmergencyTeamComparer : IComparer
    {
        bool direct = true;

        public bool Direct
        {
            get { return direct; }
            set { direct = value; }
        }

        int IComparer.Compare(Object x, Object y)
        {
            Data_EmergencyTeam a = (Data_EmergencyTeam)x;
            Data_EmergencyTeam b = (Data_EmergencyTeam)y;
            if (Direct)
            {
                return a.TeamName.CompareTo(b.TeamName);
            }
            else
            {
                return b.TeamName.CompareTo(a.TeamName);
            }
        }
    }
  
}
