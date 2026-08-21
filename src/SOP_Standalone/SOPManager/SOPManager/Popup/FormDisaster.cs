using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBUtility;
using UnE.GUI;

namespace SOPManager
{
	public partial class FormDisaster : Form, IRibbonButtonOwner
	{
		private ArrayList m_arrAddedUserType = new ArrayList();

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

		protected WebDBManager m_dbMgr = null;
        
		public FormDisaster()
		{
			m_dbMgr = FormMain.Instance.DBManager;
			
			InitializeComponent();
			
			InitRibbonButton();
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

		private void SetDefaultValue(string szCategoryName, string szSubCategoryName, string szDisasterName)
		{
			RibbonButton btn = GetCategoryBtn(szCategoryName);
			if (btn != null)
			{
				btn.PerformClick();

				DataGridViewRow row = GetSubCategoryBtn(szSubCategoryName);
				if (row != null)
				{
					dataGridViewDisaster.ClearSelection();
					row.Selected = true;
					dataGridViewDisaster.CurrentCell = row.Cells[0];

					DataGridViewRow rowDisaster = GetDisasterBtn(szDisasterName);
					if( rowDisaster != null)
					{
						dataGridViewSubDisaster.ClearSelection();
						rowDisaster.Selected = true;
						dataGridViewSubDisaster.CurrentCell = rowDisaster.Cells[0];
					}
				}
			}			
		}

		private RibbonButton GetCategoryBtn(string szName)
		{
			foreach(RibbonButton btn in m_btnCategoryList)
			{
				if (btn.ToolTipText == szName)
					return btn;
			}
			return null;
		}

		private DataGridViewRow GetSubCategoryBtn(string szName)
		{
			foreach(DataGridViewRow row in m_arrSubCategoryButton)
			{
				if (row.Cells[1].Value.ToString() == szName)
					return row;
			}
			return null;

		}

		private DataGridViewRow GetDisasterBtn(string szName)
		{
			foreach (DataGridViewRow row in m_arrDetailCategoryButton)
			{
				if (row.Cells[1].Value.ToString() == szName)
					return row;
			}			
			return null;
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

				default:
					break;
			}
		}


		private ArrayList m_btnCategoryList = new ArrayList();

		private void InitRibbonButton()
		{
			m_btnCategoryList.Clear();

			btnCategroyExplosion.ID = ID.ID_BUTTON_EXPLOSION;
			btnCategroyExplosion.IsChecked = false;
			btnCategroyExplosion.Owner = this;

			m_btnCategoryList.Add(btnCategroyExplosion);

			btnCategoryEtc.ID = ID.ID_BUTTON_ETC;
			btnCategoryEtc.IsChecked = false;
			btnCategoryEtc.Owner = this;

			m_btnCategoryList.Add(btnCategoryEtc);

			btnCategoryTypoon.ID = ID.ID_BUTTON_TYPOON;
			btnCategoryTypoon.IsChecked = false;
			btnCategoryTypoon.Owner = this;

			m_btnCategoryList.Add(btnCategoryTypoon);

			btnCategoryTerror.ID = ID.ID_BUTTON_TERROR;
			btnCategoryTerror.IsChecked = false;
			btnCategoryTerror.Owner = this;

			btnCategorySpill.ID = ID.ID_BUTTON_SPILL;
			btnCategorySpill.IsChecked = false;
			btnCategorySpill.Owner = this;

			m_btnCategoryList.Add(btnCategorySpill);

			btnCategoryFire.ID = ID.ID_BUTTON_FIRE;
			btnCategoryFire.IsChecked = false;
			btnCategoryFire.Owner = this;

			m_btnCategoryList.Add(btnCategoryFire);

			btnCategoryNetural.ID = ID.ID_BUTTON_NATURAL;
			btnCategoryNetural.IsChecked = false;
			btnCategoryNetural.Owner = this;

			m_btnCategoryList.Add(btnCategoryNetural);

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
			if (btnRB != btnCategoryEtc)
				btnCategoryEtc.IsChecked = false;
			if (btnRB != btnCategroyExplosion)
				btnCategroyExplosion.IsChecked = false;
			if (btnRB != btnCategoryNetural)
				btnCategoryNetural.IsChecked = false;

			Refresh();
		}

		private void InitData()
		{
			nCount = 1;
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

					case 6:
						btnCategoryEtc.Tag = data;
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
		}

		private void SetSubCategoryRow(int nCategoryID)
		{
			m_arrSubCategoryButton.Clear();

			dataGridViewDisaster.ClearSelection();
			dataGridViewDisaster.Rows.Clear();

			dataGridViewSubDisaster.ClearSelection();
			dataGridViewSubDisaster.Rows.Clear();

            m_strSubCategory = "";
            m_strDetailCategory = "";
            

			for (int i = 1; i < nCount + 1; i++)
			{
                if (m_dicSubCategory.ContainsKey(i))
                {
                    if (m_dicSubCategory[i].DisasterID == nCategoryID)
                    {
                        DataGridViewRow row = new DataGridViewRow();
                        row.Tag = m_dicSubCategory[i];

                        DataGridViewImageCell imgCell = new DataGridViewImageCell();
                        imgCell.Value = SOPCategory.GetSubCategoryImage(nCategoryID, m_dicSubCategory[i].CategoryName);
                        //imgCell.Value = SetSubCategoryImage(m_dicSubCategory[i].CategoryName);
                        imgCell.ToolTipText = m_dicSubCategory[i].CategoryName;
                        imgCell.Tag = m_dicSubCategory[i].CategoryName;
                        row.Cells.Add(imgCell);

                        DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                        cell.Value = m_dicSubCategory[i].CategoryName;
                        cell.ToolTipText = m_dicSubCategory[i].CategoryName;
                        cell.Tag = m_dicSubCategory[i].ID;
                        cell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                        row.Cells.Add(cell);

                        row.Height = 50;

                        m_arrSubCategoryButton.Add(row);
                        dataGridViewDisaster.Rows.Add(row);
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

				//imgCell.Value = SetSubCategoryImage(m_strSubCategory);
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

				m_arrAddedUserType.Add(strValue);

				dataGridViewSubDisaster.ClearSelection();
				dataGridViewSubDisaster.CurrentCell = row.Cells[0];

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

		private int nCount = 1;
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
					m_dicSubCategory[nCount] = subData;
					nCount++;
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

							if(detailData.ID == -1)
							{
								if (!FindDetailCategory(arrDetail, detailData.DisasterName))
								{
									arrDetail.Add(detailData);
									m_arrAddedUserType.Add(detailData);
								}
							}
							else
							{
								if (!FindDetailCategory(arrDetail, detailData.DisasterName))
									arrDetail.Add(detailData);
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

        private void RemoveSubCategory(Data_SubDisasterCategory data2)
        {
            foreach (ArrayListEx arrCategory in m_arrDisaster)
            {
                if (arrCategory.Title == SelectedCategory)
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
                        int nDeleteKey = -1;
                        foreach(KeyValuePair<int, Data_SubDisasterCategory> pair in m_dicSubCategory)
                        {
                            if( pair.Value == data2)
                            {
                                nDeleteKey = pair.Key;
                                break;
                            }
                        }
                        if (nDeleteKey > 0)
                            m_dicSubCategory.Remove(nDeleteKey);
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

        private Data_SubDisasterCategory SetSubCategory(string szValue, DataGridViewRow row)
		{
			foreach (ArrayListEx arrCategory in m_arrDisaster)
			{
				if (arrCategory.Title == SelectedCategory)
				{
					Data_SubDisasterCategory category = FormMain.Instance.AddSubCategory(szValue, SelectedCategory);
					row.Tag = category;
					FormMain.Instance.SubDisasterCategory.Add(category);


                    m_dicSubCategory[nCount] = category;
                    nCount++;

                    ArrayListEx arrDetail = new ArrayListEx();
                    arrDetail.Title = category.CategoryName;
                    arrCategory.Add(arrDetail);


                    return category;
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


		private int GetCurrentDisasterID()
		{
			ArrayList arrDisasterCategory = FormMain.Instance.DisasterCategory;
			foreach(Data_DisasterCategory data in arrDisasterCategory)
			{
				if( data.CategoryName == SelectedCategory)
				{
					return data.ID;
				}
			}
			return -1;
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

		private bool FindSameCategory(string szValue)
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
								if (data.DisasterName == szValue)
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

		private bool FindSameSubCategory(string szValue)
		{
			foreach (ArrayListEx arrCategory in m_arrDisaster)
			{
				if (arrCategory.Title == SelectedCategory)
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


		/*private object[] m_arSubCategorys =
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
			for (int i = 0; i < m_arSubCategorys.Length; i += 2)
			{
				if (strValue == (string)m_arSubCategorys[i] || strValue.Contains((string)m_arSubCategorys[i]))
					return (Image)m_arSubCategorys[i + 1];
			}
			return global::SOPManager.Properties.Resources.btnEtc_User;
		}*/

		private Image GetTeamImage(int nType)
		{
			if (nType == 0)
				return global::SOPManager.Properties.Resources.btnEtc_User;

			return global::SOPManager.Properties.Resources.btnDot;
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
                                imgCell.Value = SOPCategory.GetSubCategoryImage(SelectedCategory, SelectedSubCategory);
                                //imgCell.Value = SetSubCategoryImage(SelectedSubCategory);
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

		private void btnCategoryNetural_Click(object sender, EventArgs e)
		{
			OnCategorySelect(sender, e);
		}

		private void btnCategoryFire_Click(object sender, EventArgs e)
		{
			OnCategorySelect(sender, e);
		}

		private void btnCategorySpill_Click(object sender, EventArgs e)
		{
			OnCategorySelect(sender, e);
		}

		private void btnCategoryTerror_Click(object sender, EventArgs e)
		{
			OnCategorySelect(sender, e);
		}

		private void btnCategoryTypoon_Click(object sender, EventArgs e)
		{
			OnCategorySelect(sender, e);
		}

		private void btnCategorySaving_Click(object sender, EventArgs e)
		{
			OnCategorySelect(sender, e);
		}

		private void btnCategroyExplosion_Click(object sender, EventArgs e)
		{
			OnCategorySelect(sender, e);
		}

		private void dataGridViewDisaster_SelectionChanged(object sender, EventArgs e)
		{
			btnDelUserType.Enabled = true;
			DataGridView grid = (DataGridView)sender;

			DataGridViewSelectedRowCollection rows = grid.SelectedRows;
			if (rows == null || rows.Count == 0)
            {
                SelectedDetailCategory = "";
                SelectedSubCategory = "";
                return;
            }
				

			DataGridViewRow row = rows[0];
			Data_SubDisasterCategory data = (Data_SubDisasterCategory)row.Tag;
			if (data != null)
			{
				SelectedSubCategory = data.CategoryName;
				SelectedDetailCategory = "";
				SetDetailCategoryRow();
			}
			else
			{
				SelectedDetailCategory = "";
				SelectedSubCategory = "";
			}
		}

		private void FormDisaster_Load(object sender, EventArgs e)
		{
			SelectedCategory = SopDocManager.Instance.CategoryName;
			SelectedSubCategory = SopDocManager.Instance.SubCategoryName;
			SelectedDetailCategory = SopDocManager.Instance.DisasterName;


			SetDefaultValue(SelectedCategory, SelectedSubCategory, SelectedDetailCategory);
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
				catch(Exception)
				{

				}				
			}
		}

		private void dataGridViewSubDisaster_SelectionChanged(object sender, EventArgs e)
		{
			btnDelUserType.Enabled = true;
			DataGridView grid = (DataGridView)sender;

			DataGridViewSelectedRowCollection rows = grid.SelectedRows;
            if (rows == null || rows.Count == 0)
            {
                SelectedDetailCategory = "";
                return;
            }

			DataGridViewRow row = rows[0];
			Data_Disaster data = (Data_Disaster)row.Tag;
			if (data != null)
			{
				m_strDetailCategory = data.DisasterName;				
			}
			else
			{
				m_strDetailCategory = "";				
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

				//imgCell.Value = SetSubCategoryImage(strValue);
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

		private void btnAddSubCategroy_Click(object sender, EventArgs e)
		{
			if (SelectedCategory == "")
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

		private void btnChnageSubCateogryName_Click(object sender, EventArgs e)
		{
			if (SelectedCategory == "" || SelectedSubCategory == "")
			{
				string szMsg2 = "재난종류를 먼저 선택하십시요";
				UnE.Utility.UMessageBoxRibbon.Show(this, szMsg2, "확인", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}	

			string szMsg = "이 작업은 해당 재난종류와 연결된 모든 SOP에 영향을 미칩니다.\n계속하시겠습니까?";
			if( UnE.Utility.UMessageBoxRibbon.Show(this,szMsg, "확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}

			PopupUserDisaster disasterName = new PopupUserDisaster();
			disasterName.ChangeTitle(4);

			string szOrgSubCategory = SelectedSubCategory;
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
						SelectedSubCategory = szValue;
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

		private void btnDeleteSubCategory_Click(object sender, EventArgs e)
		{
			DataGridViewSelectedRowCollection rows = dataGridViewDisaster.SelectedRows;
			if (rows == null || rows.Count == 0)
				return;

			DataGridViewRow row = rows[0];
			Data_SubDisasterCategory data = (Data_SubDisasterCategory)row.Tag;
			if (data != null)
			{
				SelectedSubCategory = data.CategoryName;

				ArrayList arDeleteList = null;
				foreach (ArrayListEx arrCategory in m_arrDisaster)
				{
					if (arrCategory.Title == SelectedCategory)
					{
						foreach (ArrayListEx arrSub in arrCategory)
						{
							if (arrSub.Title == SelectedSubCategory)
							{
								arDeleteList = arrSub;
							}
						}
					}
				}

                if (arDeleteList.Count > 0)
                {
                    UnE.Utility.UMessageBoxRibbon.Show("하위에 재난상황이 존재합니다.\n재난상황이 존재하는경우 삭제할 수없습니다.", "삭제오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
				FormMain.Instance.RemoveSubCategory(data, arDeleteList);
				FormMain.Instance.SubDisasterCategory.Remove(data);


                RemoveSubCategory(data);
                

				dataGridViewDisaster.ClearSelection();
				dataGridViewDisaster.Rows.Remove(row);
			}
			else
			{
				SelectedSubCategory = "";
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			SopDocManager.Instance.CategoryName = SelectedCategory;
			SopDocManager.Instance.SubCategoryName = SelectedSubCategory;
			SopDocManager.Instance.DisasterName = SelectedDetailCategory;

            if (SelectedCategory == "" || SelectedSubCategory == "" || SelectedDetailCategory == "")
            {
                string szMsg2 = "재난분야/재난종류/재난상황을 선택하십시요";
                UnE.Utility.UMessageBoxRibbon.Show(this, szMsg2, "확인", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

			DialogResult = System.Windows.Forms.DialogResult.OK;
			Close();
		}

		private void btnChangeDisaster_Click(object sender, EventArgs e)
		{
			if (SelectedCategory == "" || SelectedSubCategory == "" || SelectedDetailCategory == "")
			{
                string szMsg2 = "재난분야/재난종류/재난상황을 선택하십시요";
				UnE.Utility.UMessageBoxRibbon.Show(this, szMsg2, "확인", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}


			string szOrgSelectName = SelectedDetailCategory;

            //if( szOrgSelectName == SopDocManager.Instance.DisasterName)
            //{
            //    if( SelectedSubCategory != SopDocManager.Instance.SubCategoryName)
            //    {
            //        if(SelectedCategory != SopDocManager.Instance.CategoryName)
            //        {
            //            string szMsg2 = "재난분야/재난종류/재난상황을 선택하십시요";
            //            UnE.Utility.UMessageBoxRibbon.Show(this, szMsg2, "확인", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //            return;
            //            //string szMsg2 = "현재 사용중인 재난은 변경할 수 없습니다.";
            //            //UnE.Utility.UMessageBoxRibbon.Show(this, szMsg2, "확인", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //            //return;
            //        }
            //    }				
            //}

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

					SelectedDetailCategory = szValue;
					
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

	
		private void btnDelUserType_Click(object sender, EventArgs e)
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
						string szMsg2 = "현재 사용중인 재난상황은 삭제할 수 없습니다.";
						UnE.Utility.UMessageBoxRibbon.Show(this, szMsg2, "확인", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}
				}
			}

			string szMsg = "이 작업은 선택된 재난상황과 연결된 모든 SOP가 삭제됩니다.\n계속하시겠습니까?";
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
								if(!ioMgr.DeleteSOPVersion(m_dbMgr, data.VersionID, true, true, true))
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

                    RemoveDetailCategory(data);
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
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
			Close();
		}
	}
}
