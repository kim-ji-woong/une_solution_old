using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DBUtility;
using UnE.GUI;

namespace SOPManager
{
	public partial class FormNewSOP2 : Form, UnE.GUI.IRibbonButtonOwner
	{
        // "새 SOP" 인지 변경인지 체크
        private bool m_bNew = false;
		private ArrayList m_arrAddedUserType = new ArrayList();
		private ArrayList m_arAddedUserTeam = new ArrayList();
		private ArrayList m_arAddedExternalTeam = new ArrayList();
		protected IOManager m_ioMgr = new IOManager();

		private PopupUserDisaster m_popupUserDisaster = new PopupUserDisaster();

		//private Dictionary<int, Data_SubDisasterCategory> m_dicSubCategory = new Dictionary<int, Data_SubDisasterCategory>();
        private List<Data_SubDisasterCategory> m_listSubCategory = new List<Data_SubDisasterCategory>();

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

		private ArrayList m_arrNormalTeam = new ArrayList();
		private ArrayList m_arrEmergencyTeam = new ArrayList();

		protected WebDBManager m_dbMgr = null;

		public FormNewSOP2(bool bNew = true)
		{
            m_bNew = bNew;
			m_dbMgr = FormMain.Instance.DBManager;
			InitializeComponent();
			InitRibbonButton();
			TopLevel = false;
			StartPosition = FormStartPosition.Manual;
			ShowInTaskbar = false;
			InitData();
		}

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
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

            if (!FormMain.Instance.UseStepMember)
                NoStepMemberOption();
		}

        private void NoStepMemberOption()
        {
            int nNewWidth = 945;
            //int nNewWidth = this.Width;
            //int nDiff = this.Size.Width - nNewWidth;

            this.Size = new Size(nNewWidth, this.Size.Height);
            //btnReload.Location = new Point(btnReload.Location.X - nDiff, btnReload.Location.Y);
            //btnCreateSOP.Location = new Point(btnCreateSOP.Location.X - nDiff, btnCreateSOP.Location.Y);
        }

		public ToolStripStatusLabel GetStatusLabel()
		{
			return null;
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
			switch (nID)
			{
				case ID.ID_BUTTON_EXPLOSION:

					break;

				case ID.ID_BUTTON_ETC:
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

                case ID.ID_BUTTON_SECURITY:
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

			btnCategoryEtc.ID = ID.ID_BUTTON_ETC;
			btnCategoryEtc.IsChecked = false;
			btnCategoryEtc.Owner = this;

			//btnCategoryTypoon.ID = ID.ID_BUTTON_TYPOON;
			//btnCategoryTypoon.IsChecked = false;
			//btnCategoryTypoon.Owner = this;

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

            btnCategorySecurity.ID = ID.ID_BUTTON_SECURITY;
            btnCategorySecurity.IsChecked = false;
            this.btnCategorySecurity.Owner = this;
		}

		private void InitData()
		{
			//nCount = 1;

            Enable(btnEditDisaster, false);
            Enable(btnDelDisaster, false);
            Enable(btnEditSubDisaster, false);
            Enable(btnDelSubDisaster, false);

			m_arrDisaster.Clear();
			ArrayList arrDisasterCategory = FormMain.Instance.DisasterCategory;
			foreach (Data_DisasterCategory data in arrDisasterCategory)
			{
                int nData = data.ID % 10;
                switch (nData)
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

					case 6:
						btnCategoryEtc.Tag = data;
						break;

					case 7:
						btnCategroyExplosion.Tag = data;
						break;

                    //case 8:
                    //    btnCategorySecurity.Tag = data;
                    //    break;
				}

				SetCategory(data);
			}

			InitTeamPane();
		}

		protected void OnCategorySelect(object sender, EventArgs e)
		{
			RibbonButton btnRB = (RibbonButton)sender;
			btnRB.IsChecked = true;

            m_strCategory = m_strSubCategory = m_strDetailCategory = "";

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
			//if (btnRB != btnCategoryTypoon)
			//	btnCategoryTypoon.IsChecked = false;
			if (btnRB != btnCategoryEtc)
				btnCategoryEtc.IsChecked = false;
			if (btnRB != btnCategroyExplosion)
				btnCategroyExplosion.IsChecked = false;
			if (btnRB != btnCategoryNetural)
				btnCategoryNetural.IsChecked = false;
            if (btnRB != btnCategorySecurity)
                btnCategorySecurity.IsChecked = false;

			Refresh();
		}

		private void dataGridViewDisaster_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			/*if (sender == null)
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
			}*/
		}

		private void dataGridViewSubDisaster_CellClick(object sender, DataGridViewCellEventArgs e)
		{
            //if (sender == null)
            //    return;

            //DataGridView grid = (DataGridView)sender;
            //DataGridViewCell cell = (DataGridViewCell)(grid.Rows[e.RowIndex].Cells[e.ColumnIndex]);
            //if (cell != null)
            //{
            //    string szSubCategoryName = (string)(cell.Tag);
            //}
		}

		private void dataGridViewDisaster_SelectionChanged(object sender, EventArgs e)
		{
			DataGridView grid = (DataGridView)sender;

			DataGridViewSelectedRowCollection rows = grid.SelectedRows;
            if (rows == null || rows.Count == 0)
            {
                Enable(btnEditDisaster, false);
                Enable(btnDelDisaster, false);
                return;
            }

			DataGridViewRow row = rows[0];
			Data_SubDisasterCategory data = (Data_SubDisasterCategory)row.Tag;
			if (data != null)
			{
				SelectedSubCategory = data.CategoryName;
				SetDetailCategoryRow();

                string query = string.Format("select UserCategory from subDisasterCategory where ID={0}", data.ID);
                ArrayList arrRes = m_dbMgr.GetResultData(query, 0);
                if (arrRes != null && WebDBManager.GetIntField(arrRes[0].ToString(), 0) == 1)
                {
                    Enable(btnEditDisaster, true);

                    if (dataGridViewSubDisaster.Rows.Count == 0)
                        Enable(btnDelDisaster, true);
                }
                else
                {
                    Enable(btnEditDisaster, false);
                    Enable(btnDelDisaster, false);
                }
			}
			else
			{
				SelectedSubCategory = "";
                Enable(btnEditDisaster, false);
                Enable(btnDelDisaster, false);
			}
		}

		private void dataGridViewSubDisaster_SelectionChanged(object sender, EventArgs e)
		{
			DataGridView grid = (DataGridView)sender;

			DataGridViewSelectedRowCollection rows = grid.SelectedRows;
            if (rows == null || rows.Count == 0)
            {
                Enable(btnEditSubDisaster, false);
                Enable(btnDelSubDisaster, false);
                return;
            }

			DataGridViewRow row = rows[0];
			Data_Disaster data = (Data_Disaster)row.Tag;
			if (data != null)
			{
				m_strDetailCategory = data.DisasterName;
				richTextBoxDisasterDescription.Text = data.Description;

                Enable(btnEditSubDisaster, true);
                Enable(btnDelSubDisaster, true);
			}
			else
			{
				m_strDetailCategory = "";
				richTextBoxDisasterDescription.Text = "";
                Enable(btnEditSubDisaster, false);
                Enable(btnDelSubDisaster, false);
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

            bool bFind = false;
            for (int i = 0; i < m_listSubCategory.Count; i++)
			{
                Data_SubDisasterCategory subCategory = m_listSubCategory[i];

                if (subCategory.DisasterID == nCategoryID)
				{
                    bFind = true;
					DataGridViewRow row = new DataGridViewRow();
                    row.Tag = m_listSubCategory[i];

					DataGridViewImageCell imgCell = new DataGridViewImageCell();
                    imgCell.Value = SOPCategory.GetSubCategoryImage(nCategoryID, subCategory.CategoryName);
                    imgCell.ToolTipText = subCategory.CategoryName;
                    imgCell.Tag = subCategory.CategoryName;
					row.Cells.Add(imgCell);

					DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                    cell.Value = subCategory.CategoryName;
                    cell.ToolTipText = subCategory.CategoryName;
                    cell.Tag = subCategory.ID;
					cell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
					row.Cells.Add(cell);

					row.Height = 50;

					m_arrSubCategoryButton.Add(row);
					dataGridViewDisaster.Rows.Add(row);
				}
			}
            if(!bFind)
            {
                Enable(btnEditDisaster, false);
                Enable(btnDelDisaster, false);
                Enable(btnEditSubDisaster, false);
                Enable(btnDelSubDisaster, false);
            }
		}

		public void SetDetailCategoryRow(string strCategoryName)
		{
			m_arrDetailCategoryButton.Clear();

			dataGridViewSubDisaster.ClearSelection();
			dataGridViewSubDisaster.Rows.Clear();

			int nCategoryID = 0;
            for (int i = 0; i < m_listSubCategory.Count; i++)
			{
                if (m_listSubCategory[i].CategoryName == strCategoryName)
				{
                    nCategoryID = m_listSubCategory[i].ID;
					break;
				}
			}

			ArrayList arrDetail = FormMain.Instance.DetailDisasterCategory;
			foreach (Data_Disaster data in arrDetail)
			{
				if (data.SubDisasterID == nCategoryID)
				{
                    Data_SubDisasterCategory subCategory = null;
                    //if (m_listSubCategory.TryGetValue(data.SubDisasterID, out subCategory) == false)
                    //    continue;
                    subCategory = GetSubCategory(data.SubDisasterID);
                    if (subCategory == null)
                        continue;

					DataGridViewRow row = new DataGridViewRow();
					row.Tag = data.ID;
					DataGridViewImageCell imgCell = new DataGridViewImageCell();
                    imgCell.Value = SOPCategory.GetSubCategoryImage(subCategory.DisasterID, subCategory.CategoryName);
                    //imgCell.Value = SetSubCategoryImage(subCategory.DisasterID, strCategoryName);
					imgCell.ToolTipText = data.DisasterName;
					imgCell.Tag = data.DisasterName;
					row.Cells.Add(imgCell);
					DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
					cell.Value = data.DisasterName;
					cell.Tag = data.DisasterName;
					cell.ToolTipText = data.DisasterName;
					cell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
					row.Cells.Add(cell);
					row.Height = 50;

					m_arrDetailCategoryButton.Add(row);

					dataGridViewSubDisaster.Rows.Add(row);
				}
			}
		}

        private Data_SubDisasterCategory GetSubCategory(int nDisasterID)
        {
            for(int i=0; i<m_listSubCategory.Count; ++i)
            {
                if (m_listSubCategory[i].ID == nDisasterID)
                    return m_listSubCategory[i];
            }
            return null;
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

                                Data_SubDisasterCategory subCategory = null;
                                subCategory = GetSubCategory(data.SubDisasterID);
                                if (subCategory == null)
                                    continue;
                                //if (m_dicSubCategory.TryGetValue(data.SubDisasterID, out subCategory) == false)
                                //    continue;

								DataGridViewImageCell imgCell = new DataGridViewImageCell();
                                imgCell.Value = SOPCategory.GetSubCategoryImage(subCategory.DisasterID, subCategory.CategoryName);
                                //imgCell.Value = SetSubCategoryImage(subCategory.DisasterID, SelectedSubCategory);
								imgCell.Tag = data.DisasterName;

								imgCell.ToolTipText = data.DisasterName;
								row.Cells.Add(imgCell);

								DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
								cell.Value = data.DisasterName;
								cell.Tag = data.DisasterName;
								cell.ToolTipText = data.DisasterName;
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

		private Data_Disaster AddUserType(string strValue)
		{
			if (strValue == "")
			{
				UnE.Utility.UMessageBoxRibbon.Show("재난 이름을 설정하십시오.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return null;
			}

			if (FindSameCategory(strValue) == false)
			{
				DataGridViewRow row = new DataGridViewRow();

				Data_Disaster disaster = SetDetailCategory(strValue, row);
                Data_SubDisasterCategory subCategory = FormMain.Instance.GetSubDisasterCategory(disaster);

				DataGridViewImageCell imgCell = new DataGridViewImageCell();

                if (subCategory != null)
                    imgCell.Value = SOPCategory.GetSubCategoryImage(subCategory.DisasterID, subCategory.CategoryName);
                else
                    imgCell.Value = SOPCategory.GetDefaultSubCategoryImage();

                //imgCell.Value = SetSubCategoryImage(SelectedSubCategory);
				imgCell.Tag = strValue;
				imgCell.ToolTipText = strValue;
				row.Cells.Add(imgCell);

				DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
				cell.Value = strValue;
				cell.ToolTipText = strValue;
				cell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
				row.Cells.Add(cell);

				row.Height = 50;
				dataGridViewSubDisaster.Rows.Add(row);

				dataGridViewSubDisaster.ClearSelection();
				dataGridViewSubDisaster.CurrentCell = row.Cells[0];

				m_arrAddedUserType.Add(strValue);

				return (Data_Disaster)row.Tag;
			}
			else
			{
				UnE.Utility.UMessageBoxRibbon.Show("같은 이름의 재난을 사용할 수 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			return null;
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
		//private int nCount = 1;
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
					//m_dicSubCategory[nCount] = subData;
                    m_listSubCategory.Add(subData);
					//nCount++;
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
							{
								arrDetail.Add(detailData);
								//System.Diagnostics.Trace.WriteLine("find : " + detailData.DisasterName);
							}
							else
							{
								System.Diagnostics.Trace.WriteLine("Not find : " + detailData.DisasterName);
							}
						}
					}
				}
			}
		}

		private bool FindDetailCategory(ArrayListEx arr, string strDisasterName)
		{
			foreach (Data_Disaster data in arr)
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
						if (arrSub.Title == SelectedSubCategory)
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

        private Data_Disaster SetDetailCategory(string strValue, DataGridViewRow row)
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
							return data;
						}
					}
				}
			}

            return null;
		}

		private int GetCurrentSubDisasterID()
		{
			DataGridViewSelectedRowCollection rows = dataGridViewDisaster.SelectedRows;
			if (rows == null || rows.Count == 0)
				return -1;

			DataGridViewRow row = rows[0];
			Data_SubDisasterCategory data = (Data_SubDisasterCategory)row.Tag;
			return data.ID;
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

		private bool FindSameCategory(string szName)
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
								if (data.DisasterName == szName)
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

        // 첫번째 요소 : DisasterCategory ID
		/*private object[] m_arSubCategorys =
		{
			1, "태풍", global::SOPManager.Properties.Resources.btn_sub_typoon,
			1, "지진", global::SOPManager.Properties.Resources.btn_sub_earthquake,
			1, "폭설", global::SOPManager.Properties.Resources.btn_sub_snowfall,
			1, "침수", global::SOPManager.Properties.Resources.btn_sub_flooding,
			6, "일반재해", global::SOPManager.Properties.Resources.btnEtc_User,
			2, "화재", global::SOPManager.Properties.Resources.btn_sub_fire,
			2, "산불", global::SOPManager.Properties.Resources.btn_sub_fire,
			3, "오염", global::SOPManager.Properties.Resources.btn_sub_spill,
			3, "누출", global::SOPManager.Properties.Resources.btn_sub_spill,
			3, "유출", global::SOPManager.Properties.Resources.btn_sub_spill,
			3, "암모니아", global::SOPManager.Properties.Resources.btn_sub_spill,
			4, "테러", global::SOPManager.Properties.Resources.btn_sub_terror,
			7, "폭발", global::SOPManager.Properties.Resources.btn_sub_volcano,
			5, "119상황", global::SOPManager.Properties.Resources.btn_sub_119,
			5, "SOP상황", global::SOPManager.Properties.Resources.btnEtc_User,
			4, "무장", global::SOPManager.Properties.Resources.btn_sub_terror,
			4, "괴선박", global::SOPManager.Properties.Resources.btn_sub_terror,
			4, "폭탄", global::SOPManager.Properties.Resources.btn_sub_terror,
			4, "침입", global::SOPManager.Properties.Resources.btn_sub_terror,
			4, "폭약", global::SOPManager.Properties.Resources.btn_sub_terror,
            8, "침입", global::SOPManager.Properties.Resources.btn_sub_security,
            8, "배회", global::SOPManager.Properties.Resources.btn_sub_security,
            8, "쓰러짐", global::SOPManager.Properties.Resources.btn_sub_security,
            8, "도난", global::SOPManager.Properties.Resources.btn_sub_security,
            8, "방치", global::SOPManager.Properties.Resources.btn_sub_security,
            8, "가상펜스", global::SOPManager.Properties.Resources.btn_sub_security,
            8, "내부비상벨", global::SOPManager.Properties.Resources.btn_sub_security,
            8, "외부비상벨", global::SOPManager.Properties.Resources.btn_sub_security,
            8, "기타", global::SOPManager.Properties.Resources.btn_sub_security,
            8, "방범", global::SOPManager.Properties.Resources.btn_sub_security
		};

        private Image SetSubCategoryImage(int nDisasterCategoryID, string strValue)
        {
            for (int i = 0; i < m_arSubCategorys.Length; i += 3)
            {
                if (nDisasterCategoryID == (int)m_arSubCategorys[i])
                {
                    if (strValue == (string)m_arSubCategorys[i + 1] || strValue.Contains((string)m_arSubCategorys[i + 1]))
                        return (Image)m_arSubCategorys[i + 2];
                }
            }
            return global::SOPManager.Properties.Resources.btnEtc_User;
        }

		private Image SetSubCategoryImage(string strValue)
		{
			for (int i = 0; i < m_arSubCategorys.Length; i += 3)
			{
				if (strValue == (string)m_arSubCategorys[i + 1] || strValue.Contains((string)m_arSubCategorys[i + 1]))
					return (Image)m_arSubCategorys[i + 2];
			}
			return global::SOPManager.Properties.Resources.btnEtc_User;
		}*/

		private Image GetTeamImage(int nType)
		{
			if (nType == 0)
				return global::SOPManager.Properties.Resources.btnEtc_User;

			return global::SOPManager.Properties.Resources.btnDot;
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
		}

		private void btnUserType_Click(object sender, EventArgs e)
		{
			if (SelectedCategory == "" || SelectedSubCategory == "")
				return;

			PopupUserDisaster disasterName = new PopupUserDisaster();
			disasterName.ChangeTitle(1);
            UnE.GUI.DialogFormFrameRibbon frame = new DialogFormFrameRibbon(disasterName);
			if (frame.ShowDialog() == DialogResult.OK)
			{
				string szValue = disasterName.DisasterCaption;
				try
				{
					Data_Disaster data = AddUserType(szValue);
					if (data != null)
					{
						string strSQL = string.Format("select max(id) from DisasterType");
						ArrayList arrResult = m_dbMgr.GetResultData(strSQL, 0);

						int nDisasterID;

						if (arrResult == null || arrResult.Count == 0)
							nDisasterID = 0;
						else
							nDisasterID = WebDBManager.GetIntField(arrResult[0].ToString(), 0);

						strSQL = string.Format("INSERT INTO DisasterType (ID, Name, SubDisasterID) VALUES ({0}, '{1}', {2})",
							++nDisasterID, data.DisasterName, data.SubDisasterID);

						m_dbMgr.GetResultData(strSQL, 0);

						data.ID = -1;
						data.VersionID = -1;
						FormMain.Instance.DetailDisasterCategory.Add(data);
					}
				}
				catch (Exception)
				{

				}
			}
		}

		private void btnDeleteUserType_Click(object sender, EventArgs e)
		{
			if (SelectedCategory == "" || SelectedSubCategory == "" || SelectedDetailCategory == "")
			{
				string szMsg2 = "재난종류를 먼저 선택하십시요";
				UnE.Utility.UMessageBoxRibbon.Show(this, szMsg2, "확인", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			string szOrgSelectName = SelectedDetailCategory;
			if (szOrgSelectName == SopDocManager.Instance.DisasterName)
			{
				if (SelectedSubCategory == SopDocManager.Instance.SubCategoryName)
				{
					if (SelectedCategory == SopDocManager.Instance.CategoryName)
					{
						string szMsg2 = "현재 사용중인 재난은 삭제할 수 없습니다.";
						UnE.Utility.UMessageBoxRibbon.Show(this, szMsg2, "확인", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
				}
			}

			string szMsg = "이 작업은 선택된 재난과 연결된 모든 SOP가 삭제됩니다.\n계속하시겠습니까?";
			if (UnE.Utility.UMessageBoxRibbon.Show(this, szMsg, "확인", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
			{
				return;
			}

			ArrayList arrDeleteDisaster = new ArrayList();
			int nID = GetCurrentSubDisasterID();

			// DB Data 업데이트
			bool bResult = false;
			try
			{
				// Batch 모드
				m_dbMgr.BeginBatch();

				// DisasterType 삭제
				string szSQL1 = string.Format("delete from DisasterType where name= '{0}' and SubDisasterID = {1}", szOrgSelectName, nID);

				m_dbMgr.GetBatchData(szSQL1);

				// 해당 DisasterType의 이름을 가지는 Disaster의 모든 버전을 삭제
				foreach (Data_Disaster data in FormMain.Instance.DetailDisasterCategory)
				{
					if (data.DisasterName == szOrgSelectName && data.SubDisasterID == nID)
					{
						arrDeleteDisaster.Add(data);

						if (data.VersionID != -1 && nID != -1)
						{
							using (IOManager ioMgr = new IOManager())
							{
								if (!ioMgr.DeleteSOPVersion(m_dbMgr, data.VersionID, true, true, true))
								{
									m_dbMgr.BatchRollback();

									string szMsg2 = "아래의 SOP가 사용 중 이어서 삭제가 취소됩니다.\n모니터링 시스템에서 중지 후 삭제 해 주세요\n사용 중인 SOP : {0}";
									string szMsg1 = string.Format(szMsg2, data.DisasterName);

									UnE.Utility.UMessageBoxRibbon.Show(this, szMsg1, "삭제 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);

									return;
								}
							}
						}
					}
				}
				m_dbMgr.BatchCommit();
				bResult = true;
			}
			catch (Exception)
			{
				m_dbMgr.BatchRollback();
			}

			// DB 작업이 성공한경우 데이터 변경
			if (bResult == true)
			{
				// MainData에서 삭제
				foreach (Data_Disaster data in arrDeleteDisaster)
				{
					FormMain.Instance.DetailDisasterCategory.Remove(data);

                    foreach (ArrayListEx arrCategory in m_arrDisaster)
                    {
                        if (arrCategory.Title == SelectedCategory)
                        {
                            foreach (ArrayListEx arrSub in arrCategory)
                            {
                                if (arrSub.Title == SelectedSubCategory)
                                {
                                    arrSub.Remove(data);                                   
                                }
                            }
                        }
                    }

				}

				SelectedDetailCategory = "";
				// UI 업데이트
				DataGridViewSelectedRowCollection rows = dataGridViewSubDisaster.SelectedRows;
				if (rows.Count > 0)
				{
					dataGridViewSubDisaster.ClearSelection();
					DataGridViewRow row = rows[0];
					dataGridViewSubDisaster.Rows.Remove(row);
				}

                Enable(btnEditSubDisaster, false);
                Enable(btnDelSubDisaster, false);
			}
		}

		public void InitTeamPane()
		{
			m_arrNormalTeam.Clear();
			m_arrNormalTeam.AddRange(FormMain.Instance.TemporaryNormalTeam);

			//InitNormalTeam();
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
				UnE.Utility.UMessageBoxRibbon.Show("조직(기관) 이름을 설정하십시오.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
					if (nID == -1)
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
				UnE.Utility.UMessageBoxRibbon.Show("같은 이름의 조직(기관)을 사용할 수 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}

		private List<SelectTeamInfo> m_arSelectTeamInfo = new List<SelectTeamInfo>();

		public List<SelectTeamInfo> SelectTeamInfoList
		{
			get { return m_arSelectTeamInfo; }
		}

        private void SelectedTeamsNoStepMember()
        {
            // 사용하지는 않지만 다른처리와 통일하기 위해 팀을 한개 더해준다.
            m_arrSelectedTeam.Clear();

            if (dataGridViewTeam.Rows.Count > 0)
                m_arrSelectedTeam.Add(dataGridViewTeam.Rows[0]);
            if (m_arrSelectedTeam.Count == 0 && dataGridViewTeamETC.Rows.Count > 0)
                m_arrSelectedTeam.Add(dataGridViewTeam.Rows[0]);

            if (m_arrSelectedTeam.Count == 0 && dataGridViewExternalTeam.Rows.Count > 0)
                m_arrSelectedTeam.Add(dataGridViewTeam.Rows[0]);
            //////////////////////////////////////////////////////////////

            ArrayList arList = FormMain.Instance.RegularTeam;
            Data_RegularTeam team = (Data_RegularTeam)arList[0];

            m_arSelectTeamInfo.Clear();
            foreach (DataGridViewRow row in m_arrSelectedTeam)
            {
                string strTeamName = team.TeamName;
                int nTeamID = team.ID;
                Sections.SOPTeam.SOPTeamType nTeamType = Sections.SOPTeam.SOPTeamType.Regular;

                SelectTeamInfo info = new SelectTeamInfo(nTeamID, nTeamType, strTeamName);
                info.TeamObject = team;
                m_arSelectTeamInfo.Add(info);
            }
        }

		private void SelectedTeams()
		{
            if (!FormMain.Instance.UseStepMember)
            {
                SelectedTeamsNoStepMember();
                return;
            }

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

            PostSelectedTeams();
		}

        private void PostSelectedTeams()
        {
            m_arSelectTeamInfo.Clear();
            foreach (DataGridViewRow row in m_arrSelectedTeam)
            {
                string strTeamName = row.Cells[1].Value.ToString();
                int nTeamID = (int)(row.Cells[0].Tag);
                Sections.SOPTeam.SOPTeamType nTeamType = Sections.SOPTeam.SOPTeamType.Normal;
                //0(평일 비상 조직, TemporaryNormalTeam), 1(휴일 비상 조직, TemporaryEmergencyTeam), 2(외부 기관, ExternalTeam), 3(사용자 정의 조직, UserDefinedTeam), 4(정규 조직, RegularTeam)
                object obj = row.Tag;
                if (obj.GetType() == typeof(Data_NormalTeam))
                {
                    nTeamType = Sections.SOPTeam.SOPTeamType.Normal;
                }
                else if (obj.GetType() == typeof(Data_EmergencyTeam))
                {
                    nTeamType = Sections.SOPTeam.SOPTeamType.Holiday;
                }
                else if (obj.GetType() == typeof(Data_ExternalTeam))
                {
                    nTeamType = Sections.SOPTeam.SOPTeamType.External;
                }
                else if (obj.GetType() == typeof(Data_UserDefinedTeam))
                {
                    nTeamType = Sections.SOPTeam.SOPTeamType.UserDefined;
                }
                else if (obj.GetType() == typeof(Data_RegularTeam))
                {
                    nTeamType = Sections.SOPTeam.SOPTeamType.Regular;
                }

                SelectTeamInfo info = new SelectTeamInfo(nTeamID, nTeamType, strTeamName);
                info.TeamObject = obj;
                m_arSelectTeamInfo.Add(info);
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
			bool bWeek = SopDocManager.Instance.WeekMode;

			ComboBox cbo = (ComboBox)sender;
			if (bWeek)
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

		private bool m_bSelectedTeamGridRow = false;

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

		private bool m_bSelectedEtcTeamGridRow = false;

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

			//btnDelSubDisaster.Enabled = true;
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
						UnE.Utility.UMessageBoxRibbon.Show("현재 사용 중인 조직은 삭제할 수 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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

			//btnDelSubDisaster.Enabled = true;
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
						UnE.Utility.UMessageBoxRibbon.Show("현재 사용 중인 조직은 삭제할 수 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					}
				}
			}

			ClearDataGridViewSelection(grid);
		}

		private void btnCreateSOP_Click(object sender, EventArgs e)
		{
			if (m_strCategory == "")
			{
				UnE.Utility.UMessageBoxRibbon.Show("재난분야를 먼저 선택해야 합니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}

			if (dataGridViewDisaster.SelectedRows.Count == 0)
			{
				UnE.Utility.UMessageBoxRibbon.Show("재난종류를 먼저 선택해야 합니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}

			if (dataGridViewSubDisaster.SelectedRows.Count == 0)
			{
				UnE.Utility.UMessageBoxRibbon.Show("재난상황을 먼저 선택해야 합니다.\n재난 상세정의가 없는 경우 추가버튼을 이용하여 추가하세요", "오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}

			SelectedTeams();

			if (m_arrSelectedTeam == null || m_arrSelectedTeam.Count == 0)
			{
				UnE.Utility.UMessageBoxRibbon.Show("SOP를 수행할 조직을 한개 이상 선택해야 합니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			else
			{
				m_arAddedExternalTeam.Clear();
				m_arAddedUserTeam.Clear();
				m_arrAddedUserType.Clear();

				SopDocManager.Instance.InitMode();
				SopDocManager.Instance.CategoryName = m_strCategory;
				SopDocManager.Instance.SubCategoryName = m_strSubCategory;
				SopDocManager.Instance.DisasterName = m_strDetailCategory;
				SopDocManager.Instance.DisasterDescription = DisasterDescription;
				SopDocManager.Instance.FilePath = "";

				SopDocManager.Instance.WeekMode = IsWeekMode();
				SopDocManager.Instance.RegularMode = IsRegularMode();

                if(m_bNew)
				    FormMain.Instance.CreateSOP(m_arSelectTeamInfo);

                DialogResult = System.Windows.Forms.DialogResult.OK;
			}
            Close();
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
			btnCategoryEtc.IsChecked = false;
			//btnCategoryTypoon.IsChecked = false;
			btnCategoryTerror.IsChecked = false;
			btnCategorySpill.IsChecked = false;
			btnCategoryFire.IsChecked = false;
			btnCategoryNetural.IsChecked = false;

			btnCategroyExplosion.Refresh();
			btnCategoryEtc.Refresh();
			//btnCategoryTypoon.Refresh();
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

		#endregion Image Check 박스 처리 영역

		private void dataGridViewDisaster_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{

		}

        private void btnEditSubDisaster_Click(object sender, EventArgs e)
        {
            if (m_strCategory == "" || m_strSubCategory == "" || m_strDetailCategory == "")
            {
                string szMsg2 = "재난분야/재난종류/재난상황을 선택하십시요";
                UnE.Utility.UMessageBoxRibbon.Show(this, szMsg2, "확인", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }


            string szOrgSelectName = m_strDetailCategory;
            string szMsg = "이 작업은 선택된 재난상황과 연결된 모든 SOP에 영향을 미칩니다.\n계속하시겠습니까?";
            if (UnE.Utility.UMessageBoxRibbon.Show(this, szMsg, "확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            PopupUserDisaster disasterName = new PopupUserDisaster();
            disasterName.ChangeTitle(1);
            disasterName.DisasterCaption = szOrgSelectName;
            UnE.GUI.DialogFormFrameRibbon frame = new DialogFormFrameRibbon(disasterName);
            if (frame.ShowDialog() == DialogResult.OK)
            {
                bool bResult = false;
                string szValue = disasterName.DisasterCaption;
                if (szValue != szOrgSelectName)
                {
                    int nID = GetCurrentSubDisasterID();

                    m_strDetailCategory = szValue;

                    // DB Data 업데이트
                    try
                    {
                        m_dbMgr.BeginBatch();
                        string szSQL1 = string.Format("update DisasterType set name = '{0}' where name= '{1}' and SubDisasterID = {2}", szValue, szOrgSelectName, nID);
                        m_dbMgr.GetBatchData(szSQL1);
                        string szSQL2 = string.Format("update Disaster set DisasterName = '{0}' where DisasterName= '{1}' and SubDisasterID = {2}", szValue, szOrgSelectName, nID);

                        m_dbMgr.GetBatchData(szSQL2);
                        m_dbMgr.BatchCommit();
                        bResult = true;
                    }
                    catch (Exception)
                    {
                        m_dbMgr.BatchRollback();

                    }

                    if (bResult == true)
                    {
                        // Main Data 업데이트
                        foreach (Data_Disaster data in FormMain.Instance.DetailDisasterCategory)
                        {
                            if (data.DisasterName == szOrgSelectName && data.SubDisasterID == nID)
                            {
                                data.DisasterName = szValue;
                            }
                        }

                        // UI 업데이트						
                        DataGridViewSelectedRowCollection rows = dataGridViewSubDisaster.SelectedRows;
                        if (rows.Count > 0)
                        {
                            DataGridViewRow row = rows[0];
                            row.Cells[1].Value = szValue;
                        }

                        FormMain.Instance.ChangeDisasterName(szOrgSelectName, szValue);
                    }
                }
            }
        }

        private void btnAddDisaster_Click(object sender, EventArgs e)
        {
            if (m_strCategory == "")
            {
                string szMsg2 = "재난분야를 먼저 선택하십시요";
                UnE.Utility.UMessageBoxRibbon.Show(this, szMsg2, "확인", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            PopupUserDisaster disasterName = new PopupUserDisaster();
            disasterName.ChangeTitle(4);
            UnE.GUI.DialogFormFrameRibbon frame = new DialogFormFrameRibbon(disasterName);
            if (frame.ShowDialog() == DialogResult.OK)
            {
                string szValue = disasterName.DisasterCaption;
                AddNewSubCategory(szValue);
            }
        }

        private void AddNewSubCategory(string strValue)
        {
            if (strValue == "")
            {
                UnE.Utility.UMessageBoxRibbon.Show("재난종류 이름을 설정하십시오.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (FindSameSubCategory(strValue) == false)
            {
                DataGridViewRow row = new DataGridViewRow();

                Data_SubDisasterCategory subCategory = SetSubCategory(strValue, row);


                DataGridViewImageCell imgCell = new DataGridViewImageCell();

                if (subCategory != null)
                    imgCell.Value = SOPCategory.GetSubCategoryImage(subCategory.DisasterID, subCategory.CategoryName);
                else
                    imgCell.Value = SOPCategory.GetDefaultSubCategoryImage();

                imgCell.Tag = strValue;
                imgCell.ToolTipText = strValue;
                row.Cells.Add(imgCell);

                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                cell.Value = strValue;
                cell.ToolTipText = strValue;
                cell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                row.Cells.Add(cell);

                row.Height = 50;
                dataGridViewDisaster.Rows.Add(row);

                m_arrSubCategoryButton.Add(row);

                dataGridViewDisaster.ClearSelection();
                dataGridViewDisaster.CurrentCell = row.Cells[0];
            }
            else
            {
                UnE.Utility.UMessageBoxRibbon.Show("같은 이름의 재난종류를 사용할 수 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private bool FindSameSubCategory(string szValue)
        {
            foreach (ArrayListEx arrCategory in m_arrDisaster)
            {
                if (arrCategory.Title == m_strCategory)
                {
                    foreach (ArrayListEx arrSub in arrCategory)
                    {
                        if (arrSub.Title == szValue)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private Data_SubDisasterCategory SetSubCategory(string szValue, DataGridViewRow row)
        {
            foreach (ArrayListEx arrCategory in m_arrDisaster)
            {
                if (arrCategory.Title == m_strCategory)
                {
                    Data_SubDisasterCategory category = FormMain.Instance.AddSubCategory(szValue, m_strCategory);
                    row.Tag = category;
                    FormMain.Instance.SubDisasterCategory.Add(category);


                    //m_dicSubCategory[nCount] = category;
                    //nCount++;
                    m_listSubCategory.Add(category);

                    ArrayListEx arrDetail = new ArrayListEx();
                    arrDetail.Title = category.CategoryName;
                    arrCategory.Add(arrDetail);


                    return category;
                }
            }

            return null;
        }

        private void btnEditDisaster_Click(object sender, EventArgs e)
        {
            if (m_strCategory == "" || m_strSubCategory == "")
            {
                string szMsg2 = "재난종류를 먼저 선택하십시요";
                UnE.Utility.UMessageBoxRibbon.Show(this, szMsg2, "확인", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string szMsg = "이 작업은 해당 재난종류와 연결된 모든 SOP에 영향을 미칩니다.\n계속하시겠습니까?";
            if (UnE.Utility.UMessageBoxRibbon.Show(this, szMsg, "확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            PopupUserDisaster disasterName = new PopupUserDisaster();
            disasterName.ChangeTitle(4);

            string szOrgSubCategory = m_strSubCategory;
            disasterName.DisasterCaption = szOrgSubCategory;
            UnE.GUI.DialogFormFrameRibbon frame = new DialogFormFrameRibbon(disasterName);
            if (frame.ShowDialog() == DialogResult.OK)
            {
                string szValue = disasterName.DisasterCaption;
                if (szValue != szOrgSubCategory)
                {
                    int nID = GetCurrentSubDisasterID();

                    bool bResult = false;

                    // Main Data 업데이트
                    foreach (Data_SubDisasterCategory data in FormMain.Instance.SubDisasterCategory)
                    {
                        if (data.ID == nID)
                        {
                            if (data.CategoryName != szValue)
                            {


                                // DB Data 업데이트
                                try
                                {
                                    string szSQL = string.Format("UPDATE SubDisasterCategory SET SubCategoryName = '{0}' WHERE ID = {1}", szValue, nID);
                                    m_dbMgr.GetResultData(szSQL, 0);

                                    bResult = true;
                                }
                                catch (Exception)
                                {
                                }

                                if (bResult == true)
                                    data.CategoryName = szValue;
                            }
                            break;
                        }
                    }

                    // DB작업이 성공한경우 데이터 업데이트
                    if (bResult == true)
                    {
                        m_strSubCategory = szValue;
                        // UI 업데이트
                        DataGridViewSelectedRowCollection rows = dataGridViewDisaster.SelectedRows;
                        if (rows.Count > 0)
                        {
                            DataGridViewRow row = rows[0];
                            row.Cells[1].Value = szValue;
                        }
                    }
                }
            }
        }

        private void btnDelDisaster_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedRowCollection rows = dataGridViewDisaster.SelectedRows;
            if (rows == null || rows.Count == 0)
                return;

            DataGridViewRow row = rows[0];
            Data_SubDisasterCategory data = (Data_SubDisasterCategory)row.Tag;
            if (data != null)
            {
                m_strSubCategory = data.CategoryName;

                ArrayList arDeleteList = null;
                foreach (ArrayListEx arrCategory in m_arrDisaster)
                {
                    if (arrCategory.Title == m_strCategory)
                    {
                        foreach (ArrayListEx arrSub in arrCategory)
                        {
                            if (arrSub.Title == m_strSubCategory)
                            {
                                arDeleteList = arrSub;
                            }
                        }
                    }
                }

                if (arDeleteList != null && arDeleteList.Count > 0)
                {
                    UnE.Utility.UMessageBoxRibbon.Show("하위에 재난상황이 존재합니다.\n재난상황이 존재하는경우 삭제할 수없습니다.", "삭제오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                FormMain.Instance.RemoveSubCategory(data, arDeleteList);
                FormMain.Instance.SubDisasterCategory.Remove(data);

                RemoveSubCategory(data);

                dataGridViewDisaster.ClearSelection();
                dataGridViewDisaster.Rows.Remove(row);

                Enable(btnDelDisaster, false);
                Enable(btnDelSubDisaster, false);
                Enable(btnEditDisaster, false);
                Enable(btnEditSubDisaster, false);
            }
            else
            {
                m_strSubCategory = "";
            }
        }

        private void RemoveSubCategory(Data_SubDisasterCategory data2)
        {
            foreach (ArrayListEx arrCategory in m_arrDisaster)
            {
                if (arrCategory.Title == m_strCategory)
                {
                    bool bFind = false;
                    ArrayListEx delete = null;
                    foreach (ArrayListEx arrSub in arrCategory)
                    {
                        if (arrSub.Title == data2.CategoryName)
                        {
                            bFind = true;
                            delete = arrSub;
                        }
                    }
                    if (bFind == true)
                    {
                        arrCategory.Remove(delete);
                        m_listSubCategory.Remove(data2);
                        //int nDeleteKey = -1;
                        //foreach (KeyValuePair<int, Data_SubDisasterCategory> pair in m_dicSubCategory)
                        //{
                        //    if (pair.Value == data2)
                        //    {
                        //        nDeleteKey = pair.Key;
                        //        break;
                        //    }
                        //}
                        //if (nDeleteKey > 0)
                        //    m_dicSubCategory.Remove(nDeleteKey);
                    }
                }
            }
        }

        private void Enable(Control ctrl, bool bEnable)
        {
            ctrl.Enabled = bEnable;
            if (bEnable)
                ctrl.ForeColor = Color.Black;
            else
                ctrl.ForeColor = Color.LightGray;

            Refresh();
        }
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
        private bool direct = true;

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
        private bool direct = true;

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

    public class SelectTeamInfo
    {
        public SelectTeamInfo(int nTeamID, Sections.SOPTeam.SOPTeamType nTeamType, string szTeamName)
        {
            m_nTeamID = nTeamID;
            m_nTeamType = nTeamType;
            m_szTeamName = szTeamName;
        }

        private string m_szTeamName = "";

        public string TeamName
        {
            get { return m_szTeamName; }
            set { m_szTeamName = value; }
        }

        private int m_nTeamID = -1;

        public int TeamID
        {
            get { return m_nTeamID; }
            set { m_nTeamID = value; }
        }

        private Sections.SOPTeam.SOPTeamType m_nTeamType = Sections.SOPTeam.SOPTeamType.None;

        public Sections.SOPTeam.SOPTeamType TeamType
        {
            get { return m_nTeamType; }
            set { m_nTeamType = value; }
        }

        private object m_objTeam = null;

        public object TeamObject
        {
            get { return m_objTeam; }
            set { m_objTeam = value; }
        }
    }
}
