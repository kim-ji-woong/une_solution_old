using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SOPManager
{
    public partial class PageBackstageDisaster : Form
    {
        private PopupUserDisaster m_popupUserDisaster = new PopupUserDisaster();

        private FormBacksatgeButton m_BackstageButton;

        private Dictionary<int, Data_SubDisasterCategory> m_dicSubCategory = new Dictionary<int, Data_SubDisasterCategory>();

        //재난 카테고리 리스트(하위 및 상세 카테고리 포함)
        private ArrayListEx m_arrDisaster = new ArrayListEx();

        
        private ArrayListEx m_arrSubCategoryButton = new ArrayListEx();
        private ArrayListEx m_arrDetailCategoryButton = new ArrayListEx();

        private int m_nButtonCount = 0;


        public ArrayListEx SubCategoryButton
        {
            get { return m_arrSubCategoryButton; }
            set { m_arrSubCategoryButton = value; }
        }

        public ArrayListEx DetailCategoryButton
        {
            get { return m_arrDetailCategoryButton; }
            set { m_arrDetailCategoryButton = value; }
        }

        public ArrayListEx DisasterCategory
        {
            get { return m_arrDisaster; }
            set { m_arrDisaster = value; }
        }

        private string m_strCategory = "";
        public string SelectedCategory
        {
            get { return m_strCategory; }
            set { m_strCategory = value; }
        }

        private string m_strSubCategory = "";
        public string SelectedSubCategory
        {
            get { return m_strSubCategory; }
            set { m_strSubCategory = value; }
        }

        private string m_strDetailCategory = "";
        public string SelectedDetailCategory
        {
            get { return m_strDetailCategory; }
            set { m_strDetailCategory = value; }
        }

        public string DisasterDescription
        {
            get { return richTextBoxDisasterDescription.Text; }
            set { richTextBoxDisasterDescription.Text = value; }
        }

        public PageBackstageDisaster()
        {
            InitializeComponent();

            InitCategoryImage();
            Init();

            //axBackstageBtnUser.Icon = FormMain.Instance.GetUserImage();
        }

        public void DecreaseButtonCount(Form frmButton)
        {
            if (m_arrDetailCategoryButton.Contains(frmButton))
                m_arrDetailCategoryButton.Remove(frmButton);

            if (m_nButtonCount > 0)
                m_nButtonCount--;
        }

        private void InitCategoryImage()
        {
            //string resDir = FormMain.Instance.ResourcePath();

			//FormMain.Instance.AddBitmapFormRes("btnDisasterCategory", new int[] { ID.ID_BUTTON_NATURAL, ID.ID_BUTTON_FIRE, ID.ID_BUTTON_SPILL, ID.ID_BUTTON_TERROR, ID.ID_BUTTON_SAVING, ID.ID_BUTTON_ETC });
            //FormMain.Instance.axCommandBars.Icons.LoadBitmap(resDir + "btnDisasterCategory.png", new int[] { ID.ID_BUTTON_NATURAL, ID.ID_BUTTON_FIRE, ID.ID_BUTTON_SPILL, ID.ID_BUTTON_TERROR, ID.ID_BUTTON_SAVING, ID.ID_BUTTON_ETC }, XtremeCommandBars.XTPImageState.xtpImageNormal);

            //btnNatural.Icon = FormMain.Instance.axCommandBars.Icons.GetImage(ID.ID_BUTTON_NATURAL, 48);
            //btnFire.Icon = FormMain.Instance.axCommandBars.Icons.GetImage(ID.ID_BUTTON_FIRE, 48);
            //btnSpill.Icon = FormMain.Instance.axCommandBars.Icons.GetImage(ID.ID_BUTTON_SPILL, 48);
            //btnTerror.Icon = FormMain.Instance.axCommandBars.Icons.GetImage(ID.ID_BUTTON_TERROR, 48);
            //btnSaving.Icon = FormMain.Instance.axCommandBars.Icons.GetImage(ID.ID_BUTTON_SAVING, 48);
            //btnEtc.Icon = FormMain.Instance.axCommandBars.Icons.GetImage(ID.ID_BUTTON_ETC, 48);
        }

        private void Init()
        {
            ArrayList arrDisasterCategory = FormMain.Instance.DisasterCategory;
            foreach (Data_DisasterCategory data in arrDisasterCategory)
            {
                switch(data.ID)
                {
                    case 1:
                        btnNatural.Caption = data.CategoryName;
                        break;
                    case 2:
                        btnFire.Caption = data.CategoryName;
                        break;
                    case 3:
                        btnSpill.Caption = data.CategoryName;
                        break;
                    case 4:
                        btnTerror.Caption = data.CategoryName;
                        break;
                    case 5:
                        btnSaving.Caption = data.CategoryName;
                        break;
                    case 6:
                        btnEtc.Caption = data.CategoryName;
                        break;
                }
                
                SetCategory(data);
            }
        }

        private void btnDisaster_Enter(object sender, EventArgs e)
        {
            FormMain.Instance.EnableControlLevel(false);
            AxXtremeCommandBars.AxBackstageButton btn = (AxXtremeCommandBars.AxBackstageButton)sender;
            
            btnNatural.Checked = false;
            btnFire.Checked = false;
            btnSpill.Checked = false;
            btnTerror.Checked = false;
            btnSaving.Checked = false;
            btnEtc.Checked = false;

            btn.Checked = true;
            SelectedCategory = btn.Caption;

            LoadAnnotationImage(btn.Caption);

            if (btnNatural == btn)
            {
                SetSubCategoryButton(1);
            }
            else if (btnFire == btn)
            {
                SetSubCategoryButton(2);
            }
            else if (btnSpill == btn)
            {
                SetSubCategoryButton(3);
            }
            else if (btnTerror == btn)
            {
                SetSubCategoryButton(4);
            }
            else if (btnSaving == btn)
            {
                SetSubCategoryButton(5);
            }
            else if (btnEtc == btn)
            {
                SetSubCategoryButton(6);
            }

            Invalidate();
        }

        private void LoadAnnotationImage(string strValue)
        {
            labelTitle.Text = strValue;
            switch (strValue)
            {
                case "자연재해":
                    labelAnnotation.Text = strValue;
                    pictureBox.Image = new Bitmap(global::SOPManager.Properties.Resources.btnCategoryNatural);
                    break;
                case "화재":
                    labelAnnotation.Text = strValue;
                    pictureBox.Image = new Bitmap(global::SOPManager.Properties.Resources.btnCategoryFire);
                    break;
                case "유출사고":
                    labelAnnotation.Text = strValue;
                    pictureBox.Image = new Bitmap(global::SOPManager.Properties.Resources.btnCategorySpill);
                    break;
                case "테러":
                    labelAnnotation.Text = strValue;
                    pictureBox.Image = new Bitmap(global::SOPManager.Properties.Resources.btnCategoryTerror);
                    break;
                case "인명구조 및 의료지원":
                    labelAnnotation.Text = strValue;
                    pictureBox.Image = new Bitmap(global::SOPManager.Properties.Resources.btnCategorySaving);
                    break;
                case "기타":
                    labelAnnotation.Text = strValue;
                    pictureBox.Image = new Bitmap(global::SOPManager.Properties.Resources.btnCategoryTypoon);
                    break;
            }
        }

        private void SetSubCategoryButton(int nCategoryID)
        {
            m_arrSubCategoryButton.Clear();
            panelDisasterType.Controls.Clear();

            Point pt = new Point(3, 3);
            for (int i = 1; i < m_dicSubCategory.Count + 1; i++)
            {
                if (m_dicSubCategory[i].DisasterID == nCategoryID)
                {
                    m_BackstageButton = new FormBacksatgeButton();

                    m_BackstageButton.Location = new Point(3, pt.Y);
                    m_BackstageButton.Size = new System.Drawing.Size(274, 66);
                    m_BackstageButton.TopLevel = false;
                    m_BackstageButton.Parent = this;
                    m_BackstageButton.axBackstageBtnUser.Tag = m_dicSubCategory[i].ID;
                    m_BackstageButton.SetButtonCaption(m_dicSubCategory[i].CategoryName);
                    m_BackstageButton.axBackstageBtnUser.Icon = SetSubCategoryImage(m_dicSubCategory[i].CategoryName);
                    panelDisasterType.Controls.Add(m_BackstageButton);

                    m_BackstageButton.Show();

                    m_arrSubCategoryButton.Add(m_BackstageButton);
                    pt.Y = m_BackstageButton.Height * i + 3 + i;
                }
            }
        }
        
        public void SetDetailCategoryButton(string strCategoryName)
        {
            m_arrDetailCategoryButton.Clear();
            panelUserType.Controls.Clear();

            int nCategoryID = 0;
            for (int i = 1; i < m_dicSubCategory.Count + 1; i++)
            {
                if (m_dicSubCategory[i].CategoryName == strCategoryName)
                {
                    nCategoryID = m_dicSubCategory[i].ID;
                    break;
                }
            }

            ArrayList arrDetail = FormMain.Instance.DetailDisasterCategory;
            int nButtonCount = 1;
            Point pt = new Point(3, 3);
            foreach(Data_Disaster data in arrDetail)
            {
                if (data.SubDisasterID == nCategoryID)
                {
                    m_BackstageButton = new FormBacksatgeButton();

                    m_BackstageButton.Location = new Point(3, pt.Y);
                    m_BackstageButton.Size = new System.Drawing.Size(274, 66);
                    m_BackstageButton.TopLevel = false;
                    m_BackstageButton.Parent = this;
                    m_BackstageButton.SetButtonCaption(data.DisasterName);
                    //m_BackstageButton.axBackstageBtnUser.Icon = FormMain.Instance.GetUserImage();
                    panelDisasterType.Controls.Add(m_BackstageButton);

                    m_BackstageButton.Show();

                    m_arrSubCategoryButton.Add(m_BackstageButton);
                    pt.Y = m_BackstageButton.Height * nButtonCount + 3 + nButtonCount;
                    nButtonCount++;
                }
            }
        }

        public void SetDetailCategoryButton(AxXtremeCommandBars.AxBackstageButton btn)
        {
            m_arrDetailCategoryButton.Clear();
            panelUserType.Controls.Clear();

            int nButtonCount = 1;
            Point pt = new Point(3, 3);
            foreach (ArrayListEx arrCategory in m_arrDisaster)
            {
                if (arrCategory.Title == SelectedCategory)
                {
                    foreach (ArrayListEx arrSub in arrCategory)
                    {
                        if (arrSub.Title == SelectedSubCategory)
                        {
                            foreach (Data_Disaster data in arrSub)
                            {
                                m_BackstageButton = new FormBacksatgeButton();

                                m_BackstageButton.Location = new Point(3, pt.Y);
                                m_BackstageButton.Size = new System.Drawing.Size(274, 66);
                                m_BackstageButton.TopLevel = false;
                                m_BackstageButton.Parent = this;
                                m_BackstageButton.SetButtonCaption(data.DisasterName);
                                //m_BackstageButton.axBackstageBtnUser.Icon = FormMain.Instance.GetUserImage();
                                panelUserType.Controls.Add(m_BackstageButton);

                                m_BackstageButton.Show();

                                m_arrDetailCategoryButton.Add(m_BackstageButton);
                                pt.Y = m_BackstageButton.Height * nButtonCount + 3 + nButtonCount;
                                nButtonCount++;
                            }
                        }
                    }
                }
            }
        }

        public ArrayListEx GetDetailArrayList(DataGridViewRow row)
        {
            foreach (ArrayListEx arr in m_arrSubCategoryButton)
            {
				if (arr.Row == row)
                    return arr;
            }

            return null;
        }

        private void axBackstageBtnUser_ClickEvent(object sender, EventArgs e)
        {
            FormMain.Instance.EnableControlLevel(false);
            if (SelectedCategory == "" || SelectedSubCategory == "") return;

            m_popupUserDisaster.ChangeTitle(1);

            if (m_popupUserDisaster.ShowDialog() == DialogResult.OK)
            {
                AddUserButton(m_popupUserDisaster.DisasterCaption);
                m_nButtonCount++;
            }
        }

        private int GetCurrentSubDisasterID()
        {
            foreach (FormBacksatgeButton btn in panelDisasterType.Controls)
            {
                if (btn.axBackstageBtnUser.Checked)
                {
                    return (int)btn.axBackstageBtnUser.Tag;
                }
            }

            return -1;
        }

        private void AddUserButton(string strValue)
        {
            if (strValue == "")
            {
                MessageBox.Show("재난 이름을 설정하십시오.");
                return;
            }

            m_arrDetailCategoryButton.Clear();
            if (FindSameCategory() == false)
            {
                // 실시간 버튼 생성
                m_BackstageButton = new FormBacksatgeButton();

                Point pt = SetUserLocation();
                m_BackstageButton.Location = new Point(3, pt.Y);

                m_BackstageButton.Size = new System.Drawing.Size(274, 66);
                m_BackstageButton.TopLevel = false;
                m_BackstageButton.Parent = this;
                m_BackstageButton.SetButtonCaption(strValue);
                //m_BackstageButton.axBackstageBtnUser.Icon = FormMain.Instance.GetUserImage();
                panelUserType.Controls.Add(m_BackstageButton);

                SetDetailCategory(strValue, m_BackstageButton.axBackstageBtnUser);
                m_arrDetailCategoryButton.Add(m_BackstageButton);
                m_BackstageButton.Show();

                m_BackstageButton.AlignButton(panelUserType);
            }
            else
            {
                MessageBox.Show("같은 이름의 재난을 사용할 수 없습니다.");
            }
        }

        private int FindCategoryID(string strCategoryName)
        {
            foreach (Data_DisasterCategory data in FormMain.Instance.DisasterCategory)
            {
                if (data.CategoryName == strCategoryName)
                    return data.ID;
            }

            return -1;
        }

        private ArrayListEx FindDetailList(string strSubCategoryName, ArrayListEx arrSubCategory)
        {
            foreach (ArrayListEx arrDetail in arrSubCategory)
            {
                if (arrDetail.Title == strSubCategoryName)
                    return arrDetail;
            }

            return null;
        }

        // 재난 카테고리, 재난유형 카테고리를 ArrayList에 담음.
        private void SetCategory(Data_DisasterCategory data)
        {
            ArrayListEx arr = new ArrayListEx();
            arr.Title = data.CategoryName;
            m_arrDisaster.Add(arr);

            //재난 유형을 ArrayList에서 하나씩 읽음
            foreach (Data_SubDisasterCategory subData in FormMain.Instance.SubDisasterCategory)
            {
                if (data.ID == subData.DisasterID)
                {
                    m_dicSubCategory[subData.ID] = subData;

                    ArrayListEx arrDetail = FindDetailList(subData.CategoryName, arr);

                    if (arrDetail == null)
                    {
                        arrDetail = new ArrayListEx();
                        arrDetail.Title = subData.CategoryName;
                        arr.Add(arrDetail);
                    }
                        
                    // DB로부터 로딩
                    //재난 유형에 재난 상세를 넣는다.
                    foreach (Data_Disaster detailData in FormMain.Instance.DetailDisasterCategory)
                    {
                        if (detailData.SubDisasterID == subData.ID)
                        {
                            if (!FindDetailCategory(arrDetail, detailData.DisasterName))
                                arrDetail.Add(detailData);
                        }
                    }
                }
            }
        }

        private bool FindDetailCategory(ArrayListEx arr, string strDisasterName)
        {
            foreach(Data_Disaster data in arr)
            {
                if (data.DisasterName == strDisasterName)
                    return true;
            }

            return false;
        }

        // 재난 상세 정의를 재난 카테고리 및 유형별 카테고리를 검색하여 해당 ArrayList에 담음
        private void SetDetailCategory(string strValue, AxXtremeCommandBars.AxBackstageButton btn)
        {
            foreach (ArrayListEx arrCategory in m_arrDisaster)
            {
                if (arrCategory.Title == SelectedCategory)
                {
                    foreach (ArrayListEx arrSub in arrCategory)
                    {
                        if(arrSub.Title == SelectedSubCategory)
                        {
                            //ArrayListEx arrDetail = new ArrayListEx();
                            //arrDetail.Title = strValue;
                            //arrDetail.Button = btn;
                            //arrSub.Add(arrDetail);
                            Data_Disaster data = new Data_Disaster();

                            data.DisasterName = strValue;
                            data.SubDisasterID = GetCurrentSubDisasterID();
                            data.ID = -1;
                            data.VersionID = -1;

                            arrSub.Add(data);
                            return;
                        }
                    }
                }
            }
        }

        private int GetDetailCount()
        {
            foreach (ArrayListEx arrCategory in m_arrDisaster)
            {
                if (arrCategory.Title == SelectedCategory)
                {
                    foreach (ArrayListEx arrSub in arrCategory)
                    {
                        if (arrSub.Title == SelectedSubCategory)
                        {
                            return arrSub.Count;
                        }
                    }
                }
            }

            return 0;
        }

        private Point SetUserLocation()
        {
            int nButtonCount = GetDetailCount();
            Point pt = new Point();
            pt.X = 3;
            //if (m_nButtonCount == 1)
            //    pt.Y = m_BackstageButton.Height * m_nButtonCount + 4;
            //else
            //    pt.Y = m_BackstageButton.Height * m_nButtonCount + 3 + m_nButtonCount;

            //if (m_nButtonCount == 0)
            //    pt.Y = 3;
            //else
            //    pt.Y = m_BackstageButton.Height * m_nButtonCount + 3 + m_nButtonCount;
            
            if (nButtonCount == 0)
                pt.Y = 3;
            else
                pt.Y = m_BackstageButton.Height * nButtonCount + 3 + nButtonCount;

            return pt;
        }

        private bool FindSameCategory()
        {
            foreach (ArrayListEx arrCategory in m_arrDisaster)
            {
                if (arrCategory.Title == SelectedCategory)
                {
                    foreach (ArrayListEx arrSub in arrCategory)
                    {
                        if (arrSub.Title == SelectedSubCategory)
                        {
                            foreach (Data_Disaster data in arrSub)
                            //foreach (ArrayListEx arrDetail in arrSub)
                            {
                                if (data.DisasterName == m_popupUserDisaster.DisasterCaption)
                                //if (arrDetail.Title == m_popupUserDisaster.DisasterCaption)
                                {
                                    return true;
                                }
                            }
                            return false;
                        }
                    }
                }
            }
            return false;
        }

        private XtremeCommandBars.ImageManagerIcon SetSubCategoryImage(string strValue)
        {
            //string resDir = FormMain.Instance.ResourcePath();
            //FormMain.Instance.axCommandBars.Icons.LoadBitmap(resDir + "btnDisasterSub.png", new int[] { ID.ID_BUTTON_SUB_DUST, ID.ID_BUTTON_SUB_ERUPTION, ID.ID_BUTTON_SUB_TERROR, ID.ID_BUTTON_SUB_OIL, ID.ID_BUTTON_SUB_FIRE, ID.ID_BUTTON_SUB_TYPHOON, ID.ID_BUTTON_SUB_TYPHOON1, ID.ID_BUTTON_SUB_QUAKE, ID.ID_BUTTON_SUB_SNOW, ID.ID_BUTTON_SUB_FLOODING, ID.ID_BUTTON_SUB_FALLINGROCK, ID.ID_BUTTON_SUB_119 }, XtremeCommandBars.XTPImageState.xtpImageNormal);

			//FormMain.Instance.AddBitmapFormRes("btnDisasterSub", new int[] { ID.ID_BUTTON_SUB_DUST, ID.ID_BUTTON_SUB_ERUPTION, ID.ID_BUTTON_SUB_TERROR, ID.ID_BUTTON_SUB_OIL, ID.ID_BUTTON_SUB_FIRE, ID.ID_BUTTON_SUB_TYPHOON, ID.ID_BUTTON_SUB_TYPHOON1, ID.ID_BUTTON_SUB_QUAKE, ID.ID_BUTTON_SUB_SNOW, ID.ID_BUTTON_SUB_FLOODING, ID.ID_BUTTON_SUB_FALLINGROCK, ID.ID_BUTTON_SUB_119 });

            XtremeCommandBars.ImageManagerIcon axIcon = null;
            switch (strValue)
            {
                case "태풍":
                    //axIcon = FormMain.Instance.axCommandBars.Icons.GetImage(ID.ID_BUTTON_SUB_TYPHOON1, 32);
                    break;
                case "지진":
                    //axIcon = FormMain.Instance.axCommandBars.Icons.GetImage(ID.ID_BUTTON_SUB_QUAKE, 32);
                    break;
                case "폭설":
                    //axIcon = FormMain.Instance.axCommandBars.Icons.GetImage(ID.ID_BUTTON_SUB_SNOW, 32);
                    break;
                case "침수":
                    //axIcon = FormMain.Instance.axCommandBars.Icons.GetImage(ID.ID_BUTTON_SUB_FLOODING, 32);
                    break;
                case "일반재해":
                    //axIcon = FormMain.Instance.axCommandBars.Icons.GetImage(ID.ID_BUTTON_USERDEFINED, 32);
                    break;
                case "화재":
                    //axIcon = FormMain.Instance.axCommandBars.Icons.GetImage(ID.ID_BUTTON_SUB_FIRE, 32);
                    break;
                case "오염":
                    //axIcon = FormMain.Instance.axCommandBars.Icons.GetImage(ID.ID_BUTTON_SUB_OIL, 32);
                    break;
                case "테러":
                    //axIcon = FormMain.Instance.axCommandBars.Icons.GetImage(ID.ID_BUTTON_SUB_TERROR, 32);
                    break;
                case "폭발":
                    //axIcon = FormMain.Instance.axCommandBars.Icons.GetImage(ID.ID_BUTTON_SUB_ERUPTION, 32);
                    break;
                case "119상황":
                    //axIcon = FormMain.Instance.axCommandBars.Icons.GetImage(ID.ID_BUTTON_SUB_119, 32);
                    break;
                case "SOP상황":
                    //axIcon = FormMain.Instance.axCommandBars.Icons.GetImage(ID.ID_BUTTON_USERDEFINED, 32);
                    break;
            }

             return axIcon;
        }

        public Panel GetPanelDisasterSub()
        {
            return panelDisasterType;
        }

        public Panel GetPanelDisasterDetail()
        {
            return panelUserType;
        }

        public string GetLevelName()
        {
            string strCategory = FormMain.Instance.GetPageDisaster().SelectedCategory;
            string strSubCategory = FormMain.Instance.GetPageDisaster().SelectedSubCategory;
            string strDetailCategory = FormMain.Instance.GetPageDisaster().SelectedDetailCategory;
            string strLevelName = FormMain.Instance.GetPageLevel().GetTabPageName();

            return strCategory + "/" + strSubCategory + "/" + strDetailCategory + "/" + strLevelName;
        }

        // return 값이 true : 평일 false : 휴일 및 야간
        public bool IsWeekMode()
        {
            return rdoWeekend.Checked ? false : true;
        }

        // Return 값 : true이면 등록모드, false이면 미등록모드
        public bool IsRegularMode()
        {
            return rdoDev.Checked ? false : true;
        }

        public void SetWeekMode(bool isCheck)
        {
            rdoWeekday.Checked = isCheck;
            rdoWeekend.Checked = !isCheck;
        }
        
        public void SetRegularMode(bool isCheck)
        {
            rdoRegular.Checked = isCheck;
            rdoDev.Checked = !isCheck;
        }

        private void rdoWeekday_CheckedChanged(object sender, EventArgs e)
        {
            FormMain.Instance.EnableControlLevel(false);
            if (rdoWeekday.Checked)
                FormMain.Instance.GetPagePage().SetTeamLabelText(rdoWeekday.Text + "비상 조직 리스트");
            else
                FormMain.Instance.GetPagePage().SetTeamLabelText(rdoWeekend.Text + "비상 조직 리스트");
        }

        private void rdoRegular_CheckedChanged(object sender, EventArgs e)
        {
            FormMain.Instance.EnableControlLevel(false);
        }

		private void btnNatural_DropDown(object sender, EventArgs e)
		{

		}

    }

}
