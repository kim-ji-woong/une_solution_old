using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnE.GUI;
using DBUtility;
using System.Diagnostics;


namespace SOPMonitoringSystem
{
	public partial class FormNewSOP : Form , IRibbonButtonOwner
	{

		private ArrayList m_arrAddedUserType = new ArrayList();
		private ArrayList m_arAddedUserTeam = new ArrayList();
		private ArrayList m_arAddedExternalTeam = new ArrayList();
		protected IOManager m_ioMgr = new IOManager();

		private PopupUserDisaster m_popupUserDisaster = new PopupUserDisaster();

		private Dictionary<int, Data_SubDisasterCategory> m_dicSubCategory = new Dictionary<int, Data_SubDisasterCategory>();

		//재난 카테고리 리스트(하위 및 상세 카테고리 포함)
		private ArrayListEx m_arrDisaster = new ArrayListEx();

		private ArrayListEx m_arrSubCategoryButton = new ArrayListEx();
		private ArrayListEx m_arrDetailCategoryButton = new ArrayListEx();


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

		/// <summary>
		/// Team List 
		/// </summary>
		private PopupUserDisaster m_popupUserTeam = new PopupUserDisaster();

		private ArrayList m_arrSelectedTeam = new ArrayList();
		public ArrayList SelectedTeamList
		{
			get { return m_arrSelectedTeam; }
			set { m_arrSelectedTeam = value; }
		}

		private ArrayList m_arrNormalTeam = new ArrayList();
		private ArrayList m_arrEmergencyTeam = new ArrayList();

		protected WebDBManager m_dbMgr = null;

		public FormNewSOP()
		{
			m_dbMgr = FormMain.Instance.DBManager;
			InitializeComponent();			
			InitRibbonButton();			
			TopLevel = false;
			StartPosition = FormStartPosition.Manual;
			ShowInTaskbar = false;	
			InitData();
		}
		
		private void FormNewSOP_Load(object sender, EventArgs e)
		{
			m_bSelectedTeamGridRow = false;
			m_bSelectedEtcTeamGridRow = false;
			m_bSelectedExternalTeamGridRow = false;
			ClearDataGridViewSelection(dataGridViewTeam);
			ClearDataGridViewSelection(dataGridViewTeamETC);
			ClearDataGridViewSelection(dataGridViewExternalTeam);

            SetRadioImage();

            if (rdoWeekday.Checked)
                SetTeamLabelText("평일 비상 조직 선택");
            else
                SetTeamLabelText("야간 및 휴일 비상 조직 선택");
		}		

		public void OnRibbonButtonMouseDown(object sender, MouseEventArgs e)
		{
		}

		public void OnRibbonButtonMouseUp(object sender, MouseEventArgs e)
		{
			RibbonButton rb = (RibbonButton)sender;
			OnCommandExcute(rb.ID, rb.IsChecked);
		}

		public void OnCommandExcute(int nID, bool bChecked)
		{
			switch(nID)
			{
				case ID.ID_BUTTON_EXPLOSION:
					
					break;
				case ID.ID_BUTTON_SAVING:
					break;
				case ID.ID_BUTTON_TYPOON:
					break;
				case ID.ID_BUTTON_TERROR:
					break;
				case ID.ID_BUTTON_SPILL:
					break;
				case ID.ID_BUTTON_FIRE:
					break;
				case ID.ID_BUTTON_NATURAL:
					break;
				default:
					break;
			}
		}

		private void InitRibbonButton()
		{
			btnCategroyExplosion.ID = ID.ID_BUTTON_EXPLOSION;
			btnCategroyExplosion.IsChecked = false;
			btnCategroyExplosion.Owner = this;

			btnCategorySaving.ID = ID.ID_BUTTON_SAVING;
			btnCategorySaving.IsChecked = false;		
			btnCategorySaving.Owner = this;

			btnCategoryTypoon.ID = ID.ID_BUTTON_TYPOON;
			btnCategoryTypoon.IsChecked = false;
			btnCategoryTypoon.Owner = this;	
	
			btnCategoryTerror.ID = ID.ID_BUTTON_TERROR;			
			btnCategoryTerror.IsChecked = false;
			btnCategoryTerror.Owner = this;		

			btnCategorySpill.ID = ID.ID_BUTTON_SPILL;
			btnCategorySpill.IsChecked = false;
			btnCategorySpill.Owner = this;
		
			btnCategoryFire.ID = ID.ID_BUTTON_FIRE;
			btnCategoryFire.IsChecked = false;			
			btnCategoryFire.Owner = this;

			btnCategoryNetural.ID = ID.ID_BUTTON_NATURAL;
			btnCategoryNetural.IsChecked = false;
			this.btnCategoryNetural.Owner = this;
		}

		private void InitData()
		{
			btnDelUserType.Enabled = false;

			m_arrDisaster.Clear();
			ArrayList arrDisasterCategory = FormMain.Instance.DisasterCategory;
			foreach (Data_DisasterCategory data in arrDisasterCategory)
			{
				switch (data.ID)
				{
					case 1:
						btnCategoryNetural.Tag = data;
						break;
					case 2:
						btnCategoryFire.Tag = data;
						break;
					case 3:
						btnCategorySpill.Tag = data;
						break;
					case 4:
						btnCategoryTerror.Tag = data;
						break;
					case 5:
						btnCategorySaving.Tag = data;
						break;
					case 7:
						btnCategroyExplosion.Tag = data;
						break;
					case 8:
						btnCategoryTypoon.Tag = data;
						break;
				}

				SetCategory(data);
			}

			InitTeamPane();
		}
		
		protected void OnCategorySelect(object sender, EventArgs e)
		{
			RibbonButton btnRB = (RibbonButton)sender;
			btnRB.IsChecked = true;

			Data_DisasterCategory data = (Data_DisasterCategory)btnRB.Tag;
			if (data != null)
			{
				SelectedCategory = data.CategoryName;
				SetSubCategoryRow(data.ID);
			}
			
			if (btnRB != btnCategoryFire)
				btnCategoryFire.IsChecked = false;
			if (btnRB != btnCategorySpill)
				btnCategorySpill.IsChecked = false;
			if (btnRB != btnCategoryTerror)
				btnCategoryTerror.IsChecked = false;
			if (btnRB != btnCategoryTypoon)
				btnCategoryTypoon.IsChecked = false;
			if (btnRB != btnCategorySaving)
				btnCategorySaving.IsChecked = false;
			if (btnRB != btnCategroyExplosion)
				btnCategroyExplosion.IsChecked = false;
			if (btnRB != btnCategoryNetural)
				btnCategoryNetural.IsChecked = false;
			
			Refresh();
		}  
			
		private void dataGridViewDisaster_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (sender == null)
				return;

			DataGridView grid = (DataGridView)sender;
			if (e.ColumnIndex == 1)
			{
				DataGridViewTextBoxCell cell = (DataGridViewTextBoxCell)(grid.Rows[e.RowIndex].Cells[e.ColumnIndex]);
				if (cell != null)
				{
					string szSubCategoryName = (string)cell.Value;
					SetDetailCategoryRow();
					SelectedSubCategory = szSubCategoryName;
				}
				else
				{
					SelectedSubCategory = "";
				}
			}
			if (e.ColumnIndex == 0)
			{
				DataGridViewImageCell cell = (DataGridViewImageCell)(grid.Rows[e.RowIndex].Cells[e.ColumnIndex]);
				if (cell != null)
				{
					string szSubCategoryName = (string)(cell.Tag);
					SetDetailCategoryRow();
					SelectedSubCategory = szSubCategoryName;
				}
				else
				{
					SelectedSubCategory = "";
				}
			}
		}

		private void dataGridViewSubDisaster_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (sender == null)
				return;

			DataGridView grid = (DataGridView)sender;
			DataGridViewCell cell = (DataGridViewCell)(grid.Rows[e.RowIndex].Cells[e.ColumnIndex]);
			if (cell != null)
			{
				string szSubCategoryName = (string)(cell.Tag);				
			}	
		}

		private void dataGridViewDisaster_SelectionChanged(object sender, EventArgs e)
		{
			btnDelUserType.Enabled = true;
			DataGridView grid = (DataGridView)sender;

			DataGridViewSelectedRowCollection rows = grid.SelectedRows;
			if (rows == null || rows.Count == 0)
				return;

			DataGridViewRow row = rows[0];
			Data_SubDisasterCategory data = (Data_SubDisasterCategory)row.Tag;
			if (data != null)
			{
				SelectedSubCategory = data.CategoryName;
				SetDetailCategoryRow();		
			}
			else
			{
				SelectedSubCategory = "";
			}
		}

		private void dataGridViewSubDisaster_SelectionChanged(object sender, EventArgs e)
		{
			btnDelUserType.Enabled = true;
			DataGridView grid = (DataGridView)sender;

			DataGridViewSelectedRowCollection rows = grid.SelectedRows;
			if (rows == null || rows.Count == 0)
				return;

			DataGridViewRow row = rows[0];
			Data_Disaster data = (Data_Disaster)row.Tag;
			if (data != null)
			{
				m_strDetailCategory = data.DisasterName;
				richTextBoxDisasterDescription.Text = data.Description;	
			}
			else
			{
				m_strDetailCategory = "";
				richTextBoxDisasterDescription.Text = "";
			}

			ClearDataGridViewSelection(dataGridViewTeam);
			ClearDataGridViewSelection(dataGridViewTeamETC);
			ClearDataGridViewSelection(dataGridViewExternalTeam);
		}

        private void SetSubCategoryRow(int nCategoryID)
        {
            m_arrSubCategoryButton.Clear();
			
			dataGridViewDisaster.ClearSelection();
			dataGridViewDisaster.Rows.Clear();

			dataGridViewSubDisaster.ClearSelection();
			dataGridViewSubDisaster.Rows.Clear();

            for (int i = 1; i < m_dicSubCategory.Count + 1; i++)
            {
                if (m_dicSubCategory[i].DisasterID == nCategoryID)
                {
					DataGridViewRow row = new DataGridViewRow();
					row.Tag = m_dicSubCategory[i];

					DataGridViewImageCell imgCell = new DataGridViewImageCell();
					imgCell.Value = SetSubCategoryImage(m_dicSubCategory[i].CategoryName);
					imgCell.Tag = m_dicSubCategory[i].CategoryName;
					row.Cells.Add(imgCell);

					DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
					cell.Value = m_dicSubCategory[i].CategoryName;
					cell.Tag = m_dicSubCategory[i].ID;
					cell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
					row.Cells.Add(cell);

					row.Height = 50;

					m_arrSubCategoryButton.Add(row);
					dataGridViewDisaster.Rows.Add(row);
                }
            }
        }
        
        public void SetDetailCategoryRow(string strCategoryName)
        {
            m_arrDetailCategoryButton.Clear();
			
			dataGridViewSubDisaster.ClearSelection();
			dataGridViewSubDisaster.Rows.Clear();

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
            foreach(Data_Disaster data in arrDetail)
            {
                if (data.SubDisasterID == nCategoryID)
                {
					DataGridViewRow row = new DataGridViewRow();
					row.Tag = data.ID;
					DataGridViewImageCell imgCell = new DataGridViewImageCell();
					imgCell.Value = SetSubCategoryImage(data.DisasterName);
					imgCell.Tag = data.DisasterName;
					row.Cells.Add(imgCell);
					DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
					cell.Value = data.DisasterName;
					cell.Tag = data.DisasterName;
					cell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
					row.Cells.Add(cell);
					row.Height = 50;

					m_arrDetailCategoryButton.Add(row);
					
					dataGridViewSubDisaster.Rows.Add(row);
                }
            }
        }

		public void SetDetailCategoryRow()
		{
			m_arrDetailCategoryButton.Clear();

			dataGridViewSubDisaster.ClearSelection();
			dataGridViewSubDisaster.Rows.Clear();


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
								DataGridViewRow row = new DataGridViewRow();
								row.Tag = data;

								DataGridViewImageCell imgCell = new DataGridViewImageCell();
								imgCell.Value = SetSubCategoryImage(data.DisasterName);
								imgCell.Tag = data.DisasterName;
								row.Cells.Add(imgCell);
								
								DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
								cell.Value = data.DisasterName;
								cell.Tag = data.DisasterName;
								cell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
								row.Cells.Add(cell);
								
								row.Height = 50;

								m_arrDetailCategoryButton.Add(row);

								dataGridViewSubDisaster.Rows.Add(row);
							}
						}
					}
				}
			}
		}

        private void AddUserType(string strValue)
        {
            if (strValue == "")
            {
                MessageBox.Show("재난 이름을 설정하십시오.");
                return;
            }

			if (FindSameCategory() == false)
            {
                DataGridViewRow row = new DataGridViewRow();								

				SetDetailCategory(strValue, row);

				DataGridViewImageCell imgCell = new DataGridViewImageCell();
				imgCell.Value = SetSubCategoryImage(strValue);
				imgCell.Tag = strValue;
				row.Cells.Add(imgCell);

				DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
				cell.Value = strValue;
				cell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
				row.Cells.Add(cell);

				row.Height = 50;
				dataGridViewSubDisaster.Rows.Add(row);

				m_arrAddedUserType.Add(strValue);
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
        private void RemoveDetailCategory(Data_Disaster data2)
        {
            foreach (ArrayListEx arrCategory in m_arrDisaster)
            {
                if (arrCategory.Title == SelectedCategory)
                {
                    foreach (ArrayListEx arrSub in arrCategory)
                    {
                        if(arrSub.Title == SelectedSubCategory)
                        {
							bool bFind = false;
							foreach (Data_Disaster data in arrSub)
							{

								if (data == data2)
								{
									bFind = true;
									break;
								}
							}

							if (bFind == true)
								arrSub.Remove(data2);
                        }
                    }
                }
            }
        }

		private void SetDetailCategory(string strValue, DataGridViewRow row)
		{
			foreach (ArrayListEx arrCategory in m_arrDisaster)
			{
				if (arrCategory.Title == SelectedCategory)
				{
					foreach (ArrayListEx arrSub in arrCategory)
					{
						if (arrSub.Title == SelectedSubCategory)
						{
							Data_Disaster data = new Data_Disaster();

							data.DisasterName = strValue;
							data.SubDisasterID = GetCurrentSubDisasterID();
							data.ID = -1;
							data.VersionID = -1;

							arrSub.Add(data);
							row.Tag = data;
							return;
						}
					}
				}
			}
		}

		private int GetCurrentSubDisasterID()
		{
			DataGridViewSelectedRowCollection rows = dataGridViewDisaster.SelectedRows;
			if (rows == null || rows.Count == 0)
				return -1;

			DataGridViewRow row = rows[0];
			Data_SubDisasterCategory data = (Data_SubDisasterCategory)row.Tag;
			return data.DisasterID;
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
                            {
                                if (data.DisasterName == m_popupUserDisaster.DisasterCaption)
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

		object[] m_arSubCategorys = 
		{
			"태풍", global::SOPManager.Properties.Resources.btn_sub_typoon,
			"지진", global::SOPManager.Properties.Resources.btn_sub_earthquake,
			"폭설", global::SOPManager.Properties.Resources.btn_sub_snowfall,
			"침수", global::SOPManager.Properties.Resources.btn_sub_flooding,
			"일반재해", global::SOPManager.Properties.Resources.btnEtc_User,
			"화재", global::SOPManager.Properties.Resources.btn_sub_fire,
			"산불", global::SOPManager.Properties.Resources.btn_sub_fire,
			"오염", global::SOPManager.Properties.Resources.btn_sub_spill,
			"누출", global::SOPManager.Properties.Resources.btn_sub_spill,
			"유출", global::SOPManager.Properties.Resources.btn_sub_spill,
			"암모니아", global::SOPManager.Properties.Resources.btn_sub_spill,
			"테러", global::SOPManager.Properties.Resources.btn_sub_terror,
			"폭발", global::SOPManager.Properties.Resources.btn_sub_volcano,
			"119상황", global::SOPManager.Properties.Resources.btn_sub_119,
			"SOP상황", global::SOPManager.Properties.Resources.btnEtc_User,
			"무장", global::SOPManager.Properties.Resources.btn_sub_terror,
			"괴선박", global::SOPManager.Properties.Resources.btn_sub_terror,
			"폭탄", global::SOPManager.Properties.Resources.btn_sub_terror,
			"침입", global::SOPManager.Properties.Resources.btn_sub_terror,
			"폭약", global::SOPManager.Properties.Resources.btn_sub_terror
		};
		

        private Image SetSubCategoryImage(string strValue)
        {
			for( int i = 0 ; i < m_arSubCategorys.Length; i+= 2)
			{
	            if( strValue == (string)m_arSubCategorys[i] || strValue.Contains((string)m_arSubCategorys[i]))
					return (Image)m_arSubCategorys[i+1];
			}
			return global::SOPManager.Properties.Resources.btnEtc_User;
        }

		private Image GetTeamImage(int nType)
		{
			if(nType == 0)
				return global::SOPManager.Properties.Resources.btnEtc_User;
			
			return global::SOPManager.Properties.Resources.btnDot;
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
        }

        private void rdoRegular_CheckedChanged(object sender, EventArgs e)
        {
            FormMain.Instance.EnableControlLevel(false);
        }

		private void btnUserType_Click(object sender, EventArgs e)
		{
			FormMain.Instance.EnableControlLevel(false);
			if (SelectedCategory == "" || SelectedSubCategory == "")
				return;

			m_popupUserDisaster.ChangeTitle(1);

			if (m_popupUserDisaster.ShowDialog() == DialogResult.OK)
			{
				AddUserType(m_popupUserDisaster.DisasterCaption);
			}
		}

		private void btnDeleteUserType_Click(object sender, EventArgs e)
		{
			if (sender == null)
				return;

			DataGridView grid = dataGridViewSubDisaster;
			DataGridViewSelectedRowCollection rows = grid.SelectedRows;
			if (rows == null || rows.Count == 0)
				return;

			foreach (DataGridViewRow row in rows)
			{
				Data_Disaster data = (Data_Disaster)row.Tag;
				if (data != null)
				{
					if (m_arrAddedUserType.Contains(data.DisasterName))
					{
						RemoveDetailCategory(data);
						dataGridViewSubDisaster.Rows.Remove(row);
						m_arrAddedUserType.Remove(data.DisasterName);
					}			
				}
				
			}			
		}
		

        public void InitTeamPane()
        {
			m_arrNormalTeam.Clear();
			m_arrNormalTeam.AddRange(FormMain.Instance.TemporaryNormalTeam);      

            InitNormalTeam();

            //InitEmergencyTeam();

            InitUserTeam();
            InitExternalTeam();

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
			dataGridViewTeam.ClearSelection();
			dataGridViewTeam.Rows.Clear();

            foreach (Data_NormalTeam data in m_arrNormalTeam)
            {
                //TemporaryTeamFullPath path = (TemporaryTeamFullPath)FormMain.Instance.FullPath[nButtonCount-1];
                foreach (TemporaryTeamFullPath path in FormMain.Instance.FullPath)
                {
                    if (data.ID == path.ID)
                    {
						DataGridViewRow row = new DataGridViewRow();
						row.Tag = data;

						DataGridViewImageCell imgCell = new DataGridViewImageCell();
						imgCell.Value = GetTeamImage(1);
						imgCell.Tag = data.ID;
						row.Cells.Add(imgCell);

						DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
						cell.Value = data.TeamName;
						cell.Tag = data.TeamName;
						cell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
						row.Cells.Add(cell);

						row.Height = 50;


						dataGridViewTeam.Rows.Add(row);
				
                        break;
                    }
                }
            }
			ClearDataGridViewSelection(dataGridViewTeam);

        }

        private void InitEmergencyTeam()
        {
			dataGridViewTeam.ClearSelection();
			dataGridViewTeam.Rows.Clear();

            foreach (Data_EmergencyTeam data in m_arrEmergencyTeam)
            {
                //TemporaryTeamFullPath path = (TemporaryTeamFullPath)FormMain.Instance.FullPath[nButtonCount-1];
                foreach (TemporaryTeamFullPath path in FormMain.Instance.FullPath)
                {
                    if (data.ID == path.ID)
                    {
						DataGridViewRow row = new DataGridViewRow();
						row.Tag = data;

						DataGridViewImageCell imgCell = new DataGridViewImageCell();
						imgCell.Value = GetTeamImage(1);
						imgCell.Tag = data.ID;
						row.Cells.Add(imgCell);

						DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
						cell.Value = data.TeamName;
						cell.Tag = data.TeamName;
						cell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
						row.Cells.Add(cell);

						row.Height = 50;
						dataGridViewTeam.Rows.Add(row);

                    }
                }
            }
			ClearDataGridViewSelection(dataGridViewTeam);
        }

        private void InitUserTeam()
        {
			dataGridViewTeamETC.ClearSelection();
			dataGridViewTeamETC.Rows.Clear();

            foreach (Data_UserDefinedTeam data in FormMain.Instance.UserDefinedTeam)
            {
				DataGridViewRow row = new DataGridViewRow();
				row.Tag = data;

				DataGridViewImageCell imgCell = new DataGridViewImageCell();
				imgCell.Value = GetTeamImage(1);
				imgCell.Tag = data.ID;
				row.Cells.Add(imgCell);

				DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
				cell.Value = data.TeamName;
				cell.Tag = data.TeamName;
				cell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
				row.Cells.Add(cell);

				row.Height = 50;
				dataGridViewTeamETC.Rows.Add(row);

            }

			ClearDataGridViewSelection(dataGridViewTeamETC);
        }

        private void InitExternalTeam()
        {
			dataGridViewExternalTeam.ClearSelection();
			dataGridViewExternalTeam.Rows.Clear();
            foreach (Data_ExternalTeam data in FormMain.Instance.ExternalTeam)
            {
				DataGridViewRow row = new DataGridViewRow();
				row.Tag = data;

				DataGridViewImageCell imgCell = new DataGridViewImageCell();
				imgCell.Value = GetTeamImage(0);
				imgCell.Tag = data.ID;
				row.Cells.Add(imgCell);

				DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
				cell.Value = data.TeamName;
				cell.Tag = data.TeamName;
				cell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
				row.Cells.Add(cell);

				row.Height = 50;
				dataGridViewExternalTeam.Rows.Add(row);

            }

			ClearDataGridViewSelection(dataGridViewExternalTeam);

        }

        public void SetTeamLabelText(string strValue)
        {
            LabelTeamList.Text = strValue;

            if (strValue.Substring(0, 2) == "평일")
                InitNormalTeam();
            else
                InitEmergencyTeam();
        }
		
		private void AddTeamRow(string strValue, bool isUser)
        {
            if (strValue == "")
            {
                MessageBox.Show("조직(기관) 이름을 설정하십시오.");
                return;
            }

            bool isCheck = false;
            if (isUser)
            {
                foreach (DataGridViewRow row in dataGridViewTeamETC.Rows)
                {
					Data_UserDefinedTeam team = (Data_UserDefinedTeam)row.Tag;
					if (team.TeamName == strValue)
                    {
						
                        isCheck = true;
                        break;
                    }
                }
            }
            else
            {
				foreach (DataGridViewRow row in dataGridViewExternalTeam.Rows)
				{
					Data_ExternalTeam team = (Data_ExternalTeam)row.Tag;
					if (team.TeamName == strValue)
					{
						isCheck = true;
						break;
					}
				}
            }

            if (!isCheck)
            {
                if (isUser)
                {
					
					IOManager mgr = new IOManager();
					int nID = mgr.AddUserDefinedTeam(m_dbMgr, strValue, false);
					if( nID == -1)
						return;

					Data_UserDefinedTeam data = new Data_UserDefinedTeam();
					data.TeamName = strValue;
					data.ID = nID;					
					DataGridViewRow row = new DataGridViewRow();
					row.Tag = data;
					DataGridViewImageCell imgCell = new DataGridViewImageCell();
					imgCell.Value = GetTeamImage(1);
					imgCell.Tag = data.ID;
					row.Cells.Add(imgCell);

					DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
					cell.Value = data.TeamName;
					cell.Tag = data.TeamName;
					cell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
					row.Cells.Add(cell);

					row.Height = 50;
					dataGridViewTeamETC.Rows.Add(row);

					m_arAddedUserTeam.Add(row);
                }
                else
                {
					IOManager mgr = new IOManager();
					int nID = mgr.AddExternalTeam(m_dbMgr, strValue, false);
					if (nID == -1)
						return;

					Data_ExternalTeam data = new Data_ExternalTeam();
					data.TeamName = strValue;
					data.ID = nID;
					DataGridViewRow row = new DataGridViewRow();
					row.Tag = data;

					DataGridViewImageCell imgCell = new DataGridViewImageCell();
					imgCell.Value = GetTeamImage(0);
					imgCell.Tag = data.ID;
					row.Cells.Add(imgCell);

					DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
					cell.Value = data.TeamName;
					cell.Tag = data.TeamName;
					cell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
					row.Cells.Add(cell);

					row.Height = 50;
					dataGridViewExternalTeam.Rows.Add(row);

					m_arAddedExternalTeam.Add(row);
                }
            }
            else
            {
                MessageBox.Show("같은 이름의 조직(기관)을 사용할 수 없습니다.");
            }
        }		     

        public void SelectedTeams()
        {
			m_arrSelectedTeam.Clear();

			DataGridView grid = dataGridViewTeam;
			DataGridViewSelectedRowCollection rows = grid.SelectedRows;
			if (rows != null && rows.Count > 0)
			{
				foreach (DataGridViewRow row in rows)
				{
					m_arrSelectedTeam.Add(row);
				}
			}		

			DataGridViewSelectedRowCollection rows2 = dataGridViewTeamETC.SelectedRows;
			if (rows2 != null && rows2.Count > 0)
			{
				foreach (DataGridViewRow row in rows2)
				{
					m_arrSelectedTeam.Add(row);
				}
			}			

			DataGridViewSelectedRowCollection rows3 = dataGridViewExternalTeam.SelectedRows;
			if (rows3 != null && rows3.Count > 0)
			{
				foreach (DataGridViewRow row in rows3)
				{
					m_arrSelectedTeam.Add(row);
				}
			}	
        }

        public void EnabledPage(bool isEnabled)
        {
            ////panelTeamList.Enabled = isEnabled;
            //panelEtc1.Enabled = isEnabled;
            //panelEtc2.Enabled = isEnabled;
            //axBackstageBtnUser.Enabled = isEnabled;
        }

        private void comboBoxSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            FormNewSOP frmNew = FormMain.Instance.GetPageDisaster();

            if (frmNew == null)
                return;

            ComboBox cbo = (ComboBox)sender;
            if (frmNew.IsWeekMode())
                NormalTeamSort(cbo.SelectedIndex);
            else
                EmergencyTeamSort(cbo.SelectedIndex);
        }

		private void ClearDataGridViewSelection(DataGridView grid)
		{
			DataGridViewSelectionMode oldmode = grid.SelectionMode;
			grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			grid.ClearSelection();
			grid.SelectionMode = oldmode;

			if (grid == dataGridViewTeam)
			{
				m_bSelectedTeamGridRow = false;				
			}
			else if (grid == dataGridViewTeamETC)
			{
				m_bSelectedEtcTeamGridRow = false;
			}
			else if (grid == dataGridViewExternalTeam)
			{
				m_bSelectedExternalTeamGridRow = false;
			}		
		}

		private void dataGridViewTeam_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			DataGridView grid = (DataGridView)sender;
			DataGridViewCell cell = (DataGridViewCell)(grid.Rows[e.RowIndex].Cells[e.ColumnIndex]);
			if (cell != null)
			{
				DataGridViewRow row = cell.OwningRow;
				if (m_bSelectedTeamGridRow == true)
				{
					row.Selected = false;
				}				
			}			
		}

		bool m_bSelectedTeamGridRow = false;
		private void dataGridViewTeam_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
		{
			DataGridView grid = (DataGridView)sender;
			DataGridViewCell cell = (DataGridViewCell)(grid.Rows[e.RowIndex].Cells[e.ColumnIndex]);
			if (cell != null)
			{
				DataGridViewRow row = cell.OwningRow;
				if (row.Selected == true)
				{
					m_bSelectedTeamGridRow = true;
				}
				else
				{
					m_bSelectedTeamGridRow = false;
				}
			}
		}
		private void dataGridViewTeam_SelectionChanged(object sender, EventArgs e)
		{
		}


		bool m_bSelectedEtcTeamGridRow = false;
		private void dataGridViewTeamETC_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			DataGridView grid = (DataGridView)sender;
			DataGridViewCell cell = (DataGridViewCell)(grid.Rows[e.RowIndex].Cells[e.ColumnIndex]);
			if (cell != null)
			{
				DataGridViewRow row = cell.OwningRow;
				if (m_bSelectedEtcTeamGridRow == true)
				{
					row.Selected = false;
				}
			}
		}
		private void dataGridViewTeamETC_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
		{
			DataGridView grid = (DataGridView)sender;
			DataGridViewCell cell = (DataGridViewCell)(grid.Rows[e.RowIndex].Cells[e.ColumnIndex]);
			if (cell != null)
			{
				DataGridViewRow row = cell.OwningRow;
				if (row.Selected == true)
				{
					m_bSelectedEtcTeamGridRow = true;
				}
				else
				{
					m_bSelectedEtcTeamGridRow = false;
				}
			}
		}	
		
		private void dataGridViewTeamETC_SelectionChanged(object sender, EventArgs e)
		{
		}
		
		private bool m_bSelectedExternalTeamGridRow = false;
		private void dataGridViewExternalTeam_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			DataGridView grid = (DataGridView)sender;
			DataGridViewCell cell = (DataGridViewCell)(grid.Rows[e.RowIndex].Cells[e.ColumnIndex]);
			if (cell != null)
			{
				DataGridViewRow row = cell.OwningRow;
				if (m_bSelectedExternalTeamGridRow == true)
				{
					row.Selected = false;
				}
			}
		}

		private void dataGridViewExternalTeam_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
		{
			DataGridView grid = (DataGridView)sender;
			DataGridViewCell cell = (DataGridViewCell)(grid.Rows[e.RowIndex].Cells[e.ColumnIndex]);
			if (cell != null)
			{
				DataGridViewRow row = cell.OwningRow;
				if (row.Selected == true)
				{
					m_bSelectedExternalTeamGridRow = true;
				}
				else
				{
					m_bSelectedExternalTeamGridRow = false;
				}
			}
		}

		private void dataGridViewExternalTeam_SelectionChanged(object sender, EventArgs e)
		{
		}


		private void btnAddExternalTeam_Click(object sender, EventArgs e)
		{
			m_popupUserTeam.ChangeTitle(3);
			if (m_popupUserTeam.ShowDialog() == DialogResult.OK)
			{
				AddTeamRow(m_popupUserTeam.DisasterCaption, false);
			}
		}

		private void btnDeleteExternalTeam_Click(object sender, EventArgs e)
		{
			WebDBManager dbMgr = FormMain.Instance.DBManager;

			btnDelUserType.Enabled = true;
			DataGridView grid = dataGridViewExternalTeam;

			DataGridViewSelectedRowCollection rows = grid.SelectedRows;
			if (rows == null || rows.Count == 0)
				return;

			foreach (DataGridViewRow row in rows)
			{
				Data_ExternalTeam data = (Data_ExternalTeam)row.Tag;
				if (data != null)
				{					
					if (m_arAddedExternalTeam.Contains(row))
					{
						if (m_ioMgr.DeleteExternalTeam(dbMgr, data.TeamName, false))
						{
							grid.Rows.Remove(row);
						}
					}
					else
					{
						MessageBox.Show("현재 사용 중인 조직은 삭제할 수 없습니다.");
					}				
				}
			}

			ClearDataGridViewSelection(grid);
		}

		private void btnAddUserTeam_Click(object sender, EventArgs e)
		{
			m_popupUserTeam.ChangeTitle(2);
			if (m_popupUserTeam.ShowDialog() == DialogResult.OK)
			{
				AddTeamRow(m_popupUserTeam.DisasterCaption, true);
			}
		}		

		private void btnDeleteUserTeam_Click(object sender, EventArgs e)
		{
			WebDBManager dbMgr = FormMain.Instance.DBManager;

			btnDelUserType.Enabled = true;
			DataGridView grid = dataGridViewTeamETC;

			DataGridViewSelectedRowCollection rows = grid.SelectedRows;
			if (rows == null || rows.Count == 0)
				return;
			foreach (DataGridViewRow row in rows)
			{
				Data_UserDefinedTeam data = (Data_UserDefinedTeam)row.Tag;
				if (data != null)
				{
					if (m_arAddedUserTeam.Contains(row))
					{
						if (m_ioMgr.DeleteUserDefinedTeam(dbMgr, data.TeamName, false))
						{
							grid.Rows.Remove(row);
						}
					}					
					else
					{
						MessageBox.Show("현재 사용 중인 조직은 삭제할 수 없습니다.");
					}					
				}
			}

			ClearDataGridViewSelection(grid);
		}

		private void btnCreateSOP_Click(object sender, EventArgs e)
		{
			if( m_strCategory == "")
			{
				MessageBox.Show("재난 카테고리를 먼저 선택해야 합니다.");
				return;
			}

			if (dataGridViewDisaster.SelectedRows.Count == 0)
			{
				MessageBox.Show("재난 유형을 먼저 선택해야 합니다.");
				return;
			}

			if (dataGridViewSubDisaster.SelectedRows.Count == 0)
			{
				MessageBox.Show("재난 상세정의를 먼저 선택해야 합니다.\n재난 상세정의가 없는 경우 사용자 정의 추가를 이용하여 추가하세요");
				return;
			}
				
			SelectedTeams();

			if (m_arrSelectedTeam == null || m_arrSelectedTeam.Count == 0)
			{
				MessageBox.Show("SOP를 수행할 조직을 한개 이상 선택해야 합니다.");				
			}
			else
			{
				m_arAddedExternalTeam.Clear();
				m_arAddedUserTeam.Clear();
				m_arrAddedUserType.Clear();

				FormMain.Instance.CreateSOP();
			}
			
		}

		private void btnReload_Click(object sender, EventArgs e)
		{
			m_arrSelectedTeam.Clear();
						
			ClearDataGridViewSelection(dataGridViewSubDisaster);
			dataGridViewSubDisaster.Rows.Clear();

			ClearDataGridViewSelection(dataGridViewDisaster);
			dataGridViewDisaster.Rows.Clear();

			ClearDataGridViewSelection(dataGridViewTeam);
			ClearDataGridViewSelection(dataGridViewTeamETC);
			ClearDataGridViewSelection(dataGridViewExternalTeam);

			SelectedCategory = "";
			SelectedSubCategory = "";

			btnCategroyExplosion.IsChecked = false;
			btnCategorySaving.IsChecked = false;
			btnCategoryTypoon.IsChecked = false;			
			btnCategoryTerror.IsChecked = false;			
			btnCategorySpill.IsChecked = false;		
			btnCategoryFire.IsChecked = false;			
			btnCategoryNetural.IsChecked = false;


			btnCategroyExplosion.Refresh();
			btnCategorySaving.Refresh();
			btnCategoryTypoon.Refresh();
			btnCategoryTerror.Refresh();
			btnCategorySpill.Refresh();
			btnCategoryFire.Refresh();
			btnCategoryNetural.Refresh();

			richTextBoxDisasterDescription.Clear();
			richTextBoxDisasterDescription.ClearUndo();
		}

        #region Image Check 박스 처리 영역
        private void SetRadioImage()
        {
            if (rdoRegular.Checked == true)
            {
                rdPictureBox1.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_black;
            }
            else
            {
                rdPictureBox1.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_gray;
            }

            if (rdoDev.Checked == true)
            {
                rdPictureBox2.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_black;
            }
            else
            {
                rdPictureBox2.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_gray;
            }

            if (rdoWeekday.Checked == true)
            {
                rdPictureBox3.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_black;
            }
            else
            {
                rdPictureBox3.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_gray;
            }

            if (rdoWeekend.Checked == true)
            {
                rdPictureBox4.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_black;
            }
            else
            {
                rdPictureBox4.BackgroundImage = global::SOPManager.Properties.Resources.rdselect_gray;
            }
        }

        private void rdLabel1_Click(object sender, EventArgs e)
        {
            if (rdoRegular.Checked == false)
            {
                rdoRegular.Checked = !rdoRegular.Checked;
                SetRadioImage();
            }
        }

        private void rdLabel2_Click(object sender, EventArgs e)
        {
            if (rdoDev.Checked == false)
            {
                rdoDev.Checked = !rdoDev.Checked;
                SetRadioImage();
            }
        }

        private void rdLabel3_Click(object sender, EventArgs e)
        {
            if (rdoWeekday.Checked == false)
            {
                SetTeamLabelText("평일 비상 조직 선택");
                rdoWeekday.Checked = !rdoWeekday.Checked;
                SetRadioImage();
            }
        }

        private void rdLabel4_Click(object sender, EventArgs e)
        {
            if (rdoWeekend.Checked == false)
            {
                SetTeamLabelText("야간 및 휴일 비상 조직 선택");
                rdoWeekend.Checked = !rdoWeekend.Checked;
                SetRadioImage();
            }
        }

        private void rdPictureBox1_Click(object sender, EventArgs e)
        {
            rdLabel1_Click(null, null);
        }

        private void rdPictureBox2_Click(object sender, EventArgs e)
        {
            rdLabel2_Click(null, null);
        }

        private void rdPictureBox3_Click(object sender, EventArgs e)
        {
            rdLabel3_Click(null, null);
        }

        private void rdPictureBox4_Click(object sender, EventArgs e)
        {
            rdLabel4_Click(null, null);
        }
        #endregion
    }  


    public class ArrayListEx : ArrayList
    {
        private string m_strTitle = "";
        private DataGridViewRow m_titleButton = null;

        public string Title
        {
            get { return m_strTitle; }
            set { m_strTitle = value; } 
        }

		public DataGridViewRow Row
        {
            get { return m_titleButton; }
            set { m_titleButton = value; }
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


