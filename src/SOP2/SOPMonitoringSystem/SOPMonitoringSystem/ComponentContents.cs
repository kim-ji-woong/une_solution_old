using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SOPDisasterSystem;
using System.Collections;
using Sections;
using SDMS;

namespace SOPMonitoringSystem
{
    public partial class ComponentContents : Form
    {
        private bool m_isFlag = false;
        private int m_nComponentHistoryID = -1;
        private int m_nComponentID = -1;
        private Sections.State m_state = Sections.State.NORMAL;
        private DataLogGridViewRow m_logGridRow = null;


		public ComponentContents()
        {
            InitializeComponent();

            //AddGridData();
            pictureBox1.Image = GetImage(m_isFlag);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (label.Text.Contains("시작")) return;
            //if (label.Text.Contains("-")) return;
            if ((label.Text.Length - 1) == label.Text.LastIndexOf('-')) return;

            m_isFlag = !m_isFlag;
            pictureBox1.Image = GetImage(m_isFlag);
            pictureBox1.Tag = m_isFlag;

            if (m_isFlag)
                dataGridView.Hide();
            else
                dataGridView.Show();

            ReSizeForm(m_isFlag);
        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView grid = (DataGridView)sender;

			if (e.ColumnIndex == 0 || e.ColumnIndex == 1)
			{
				DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)grid.Rows[e.RowIndex].Cells[e.ColumnIndex];

				string strValue = grid.Rows[e.RowIndex].Cells[2].Value.ToString();

				if (grid.Rows[e.RowIndex].Cells.Count == 5)
					strValue = grid.Rows[e.RowIndex].Cells[4].Value.ToString();
				bool isChecked = (bool)checkCell.EditedFormattedValue;

				checkCell.Value = isChecked;

				Sections.Section section = (Sections.Section)dataGridView.Tag;
				Sections.SectionTabPage page = (Sections.SectionTabPage)FormMain.Instance.GetPageHome().TabControls.SelectedTab;
                if (page == null)
                {
                    PanelSectionEx panel = (PanelSectionEx)section.GetParent();
                    page = (Sections.SectionTabPage)panel.Parent;
                    FormMain.Instance.GetPageHome().TabControls.SelectedTab = page;
                }
				Sections.SectionState state = Sections.WorkFlowManager.Instance.Find(section, !page.VirtualMode);

				if (state == null)
					return;

				if (state.GetType() == typeof(Sections.TSectionState))
				{
					Sections.SectionDataTransmission data1 = (Sections.SectionDataTransmission)section.Data;

					if (strValue.Contains("팝업") && strValue.Contains("내부"))
					{
						data1.DataInternal.UsePopupMessage = isChecked;
						state.CheckNotify1 = state.CheckNotify1 & (~1);

						if (isChecked)
							state.CheckNotify1 = state.CheckNotify1 & 1;
					}
					else if (strValue.Contains("사내방송") && strValue.Contains("내부"))
					{
						data1.DataInternal.UseBroadcast = isChecked;
						state.CheckNotify1 = state.CheckNotify1 & (~4);

						if (isChecked)
							state.CheckNotify1 = state.CheckNotify1 & 4;
					}
					else if (strValue.Contains("모바일") && strValue.Contains("내부"))
					{
						data1.DataInternal.UseMobileApp = isChecked;

						state.CheckNotify1 = state.CheckNotify1 & (~2);

						if (isChecked)
							state.CheckNotify1 = state.CheckNotify1 & 2;
					}
					else if (strValue.Contains("메세지") && strValue.Contains("외부"))
					{
						int nIdx = 3;
						int nBit = 0;
						foreach (Sections.ExternalTeamData exTeam in data1.DataExternal.SMSReceivers)
						{
							if (strValue.Contains(exTeam.TeamName))
							{
								nBit = 1 << nIdx;
								state.CheckNotify1 = state.CheckNotify1 & (~nBit);
								if (isChecked)
									state.CheckNotify1 = state.CheckNotify1 & nBit;

							}
							nIdx++;
							if (nIdx == 16)
								break;
						}
					}
					else if (strValue.Contains("팩스") && strValue.Contains("외부"))
					{
						int nIdx = 0;
						int nBit = 0;
						foreach (Sections.ExternalTeamData exTeam in data1.DataExternal.FaxReceivers)
						{
							if (strValue.Contains(exTeam.TeamName))
							{
								nBit = 1 << nIdx;
								state.CheckNotify2 = state.CheckNotify2 & (~nBit);
								if (isChecked)
									state.CheckNotify2 = state.CheckNotify2 & nBit;
							}
							nIdx++;
							if (nIdx == 16)
								break;
						}
					}

				}
				if (state.GetType() == typeof(Sections.ESectionState))
				{
					Sections.SectionDataExternal data = (Sections.SectionDataExternal)section.Data;
					if (strValue.Contains("문자"))
					{
						int nIdx = 0;
						int nBit = 0;
						foreach (Sections.ExternalTeamData exTeam in data.SMSReceivers)
						{
							if (strValue.Contains(exTeam.TeamName))
							{
								nBit = 1 << nIdx;
								state.CheckNotify1 = state.CheckNotify1 & (~nBit);
								if (isChecked)
									state.CheckNotify1 = state.CheckNotify1 & nBit;

							}
							nIdx++;
							if (nIdx == 16)
								break;
						}
					}
					else if (strValue.Contains("팩스"))
					{
						int nIdx = 0;
						int nBit = 0;
						foreach (Sections.ExternalTeamData exTeam in data.FaxReceivers)
						{
							if (strValue.Contains(exTeam.TeamName))
							{
								nBit = 1 << nIdx;
								state.CheckNotify2 = state.CheckNotify2 & (~nBit);
								if (isChecked)
									state.CheckNotify2 = state.CheckNotify2 & nBit;
							}
							nIdx++;
							if (nIdx == 16)
								break;
						}
					}
				}
				else if (state.GetType() == typeof(Sections.ISectionState))
				{
					Sections.SectionDataInternal data = (Sections.SectionDataInternal)section.Data;
					if (strValue.Contains("팝업"))
					{
						data.UsePopupMessage = isChecked;
						state.CheckNotify1 = state.CheckNotify1 & (~1);

						if (isChecked)
							state.CheckNotify1 = state.CheckNotify1 & 1;
					}
					else if (strValue.Contains("사내방송"))
					{
						data.UseBroadcast = isChecked;
						state.CheckNotify1 = state.CheckNotify1 & (~4);

						if (isChecked)
							state.CheckNotify1 = state.CheckNotify1 & 4;
					}
					else if (strValue.Contains("모바일"))
					{
						data.UseMobileApp = isChecked;

						state.CheckNotify1 = state.CheckNotify1 & (~2);

						if (isChecked)
							state.CheckNotify1 = state.CheckNotify1 & 2;
					}
				}
				else if (state.GetType() == typeof(Sections.PSectionState))
				{
					Sections.SectionDataProcess data = (Sections.SectionDataProcess)section.Data;

					for (int i = 0; i < data.MissionItems.Count; i++)
					{
						Sections.MissionItem item = (Sections.MissionItem)data.MissionItems[i];
						if (item.Mission == strValue)
						{
							MissionItemInfo info = FormMain.Instance.GetMissionInfo(item);

							if (info == null)
							{
								info = new MissionItemInfo();

								if (e.ColumnIndex == 0)
								{
									info.UseSMS = isChecked;
									info.UseBroadcast = (bool)grid.Rows[e.RowIndex].Cells[1].EditedFormattedValue;
								}
								else
								{
									info.UseBroadcast = isChecked;
									info.UseSMS = (bool)grid.Rows[e.RowIndex].Cells[0].EditedFormattedValue;
								}

								FormMain.Instance.SetMissionInfo(item, info);
							}
							else
							{
								if (e.ColumnIndex == 0)
									info.UseSMS = isChecked;
								else
									info.UseBroadcast = isChecked;
							}

							if (!info.UseSMS && !info.UseBroadcast)
								item.CheckItem = false;
							else
								item.CheckItem = true;

							int nBitFlag = 1 << i;
							state.CheckNotify1 = state.CheckNotify1 & (~nBitFlag);
							state.CheckNotify2 = state.CheckNotify2 & (~nBitFlag);

							if (info.UseSMS)
								state.CheckNotify1 = state.CheckNotify1 | nBitFlag;

							if (info.UseBroadcast)
								state.CheckNotify2 = state.CheckNotify2 | nBitFlag;

							//item.CheckItem = isChecked;
							break;
						}
						if (i == 15)
							break;
					}
				}

				if (LogGridRow != null)
				{
					LogGridRow.Cells[5].Tag = state.CheckNotify1;
					LogGridRow.Cells[6].Tag = state.CheckNotify2;
				}
			}

			else
			{
				
			}
        }


		private void dataGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
		{
			DataGridView grid = (DataGridView)sender;
			DataGridViewCell cell = (DataGridViewCell)(grid.Rows[e.RowIndex].Cells[e.ColumnIndex]);
			if (cell != null)
			{
				DataGridViewRow row = cell.OwningRow;
				if (row.Selected == true)
				{
					m_bSelectedGridRow = true;
				}
				else
				{
					m_bSelectedGridRow = false;
				}
			}
		}

		private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			DataGridView grid = (DataGridView)sender;
			if (grid.SelectedRows.Count > 0)
			{

				DataGridViewCell cell = (DataGridViewCell)(grid.Rows[e.RowIndex].Cells[e.ColumnIndex]);
				if (cell != null)
				{
					DataGridViewRow row = cell.OwningRow;
					if (m_bSelectedGridRow == true)
					{
						row.Selected = false;
					}
				}
			}
		}
		private bool m_bSelectedGridRow = false;

		  

        public void SetTitle(string strTitle, DateTime time, string strStatus)
        {
            label.Text = strTitle + "/" + time +"/" + strStatus;
        }

        public string GetTitle()
        {
            return label.Text;
        }

        public void ChangeTitle(string strTitle)
        {
            label.Text = strTitle;
        }

        public void HideGrid()
        {
            dataGridView.Hide();
            m_isFlag = true;
            ReSizeForm(m_isFlag);
        }


        public void AddGridData(Sections.Section section, string strStatus, int nCheckNotify1, int nCheckNotify2)
        {
            Sections.Section.ComponentType sectionType = section.GetComponentType();

			if (sectionType == Sections.Section.ComponentType.PROCESS/* && strStatus != "실행 완료"*/)
            {
                Sections.SectionDataProcess dataSection = (Sections.SectionDataProcess)section.Data;
                Sections.SectionTabPage page = (Sections.SectionTabPage)FormMain.Instance.GetPageHome().TabControls.SelectedTab;
                if (page == null)
                {
                    PanelSectionEx panel = (PanelSectionEx)section.GetParent();
                    page = (Sections.SectionTabPage)panel.Parent;
                    FormMain.Instance.GetPageHome().TabControls.SelectedTab = page;
                }

                Sections.SectionState state = Sections.WorkFlowManager.Instance.Find(section, !page.VirtualMode);

                int nCount = dataSection.MissionItems.Count;
                string[] strMission = null;
                string[] strTransmissionType = null;
                string[] strToNotify = null;
                bool[] useSMS = null;
                bool[] useBroadcast = null;

                if (nCount > 0)
                {
                    strMission = new string[nCount];
                    strTransmissionType = new string[nCount];
                    strToNotify = new string[nCount];
                    useSMS = new bool[nCount];
                    useBroadcast = new bool[nCount];

                    int i = 0;
                    int nBit = 0;
                    foreach (Sections.MissionItem data in dataSection.MissionItems)
                    {
                        string szType = "무전기";
                        switch (data.TransmissionType)
                        {
                            case 0:
                                szType = "구두";
                                break;
                            case 1:
                                szType = "전화";
                                break;
                            case 2:
                                szType = "무전기";
                                break;
                            case 3:
                                szType = "기타";
                                break;

                        }


                        strToNotify[i] = data.Target;
                        //strTransmissionType[i] = "[" + szType + "]";
                        strTransmissionType[i] = szType;
                        strMission[i] = data.Mission;

                        nBit = 1 << i;
                        useSMS[i] = (nCheckNotify1 & nBit) == nBit;
                        useBroadcast[i] = (nCheckNotify2 & nBit) == nBit;

                        MissionItemInfo info = new MissionItemInfo();
                        info.UseBroadcast = useBroadcast[i];
                        info.UseSMS = useSMS[i];

                        if (!info.UseBroadcast && !info.UseSMS)
                            data.CheckItem = false;
                        else
                            data.CheckItem = true;

                        FormMain.Instance.SetMissionInfo(data, info);

                        i++;
                        if (i == 16)
                            break;
                    }
                }
                //AddGridData  string[] strTransType,  string[] strTarget, string[] strMission, 
                AddGridData(nCount, useSMS, useBroadcast, strTransmissionType,strToNotify, strMission, section, false, nCheckNotify1, nCheckNotify2);
            }
            else if (sectionType == Sections.Section.ComponentType.TRANSMISSION/* && strStatus != "실행 완료"*/)
            {
                Sections.SectionDataTransmission dataSection = (Sections.SectionDataTransmission)section.Data;

                bool isPopupMessage = dataSection.DataInternal.UsePopupMessage;
                bool isMobileApp = dataSection.DataInternal.UseMobileApp;
                bool isBrodcast = dataSection.DataInternal.UseBroadcast;
                bool isSMS = dataSection.DataExternal.UseSMS;
                bool isFax = dataSection.DataExternal.UseFax;
                
                int nCnt = dataSection.DataExternal.SMSReceivers.Count + dataSection.DataExternal.FaxReceivers.Count + 3;

                string[] str = new string[nCnt];
                bool[] bUse = new bool[nCnt];

                int i = 0;
                
                str[i] = "(내부상황전파) 팝업메시지 사용";                
                int nBit = 1 << i;
                bUse[i] = (nCheckNotify1 & nBit) == nBit;
                i++;

                str[i] = "(내부상황전파) 모바일메시지 사용";
                nBit = 1 << i;
                bUse[i] = (nCheckNotify1 & nBit) == nBit;
                i++;
                
                str[i] = "(내부상황전파) 사내방송 사용";
                nBit = 1 << i;
                bUse[i] = (nCheckNotify1 & nBit) == nBit;

                // nIdx = 3
                i = 3;

                if (dataSection.DataExternal.UseSMS)
                {
                    foreach (Sections.ExternalTeamData data in dataSection.DataExternal.SMSReceivers)
                    {
                        str[i] = "(외부상황전파) " + data.TeamName + " 문자메시지 전송";
                        nBit = 1 << i;
                        bUse[i++] = (nCheckNotify1 & nBit) == nBit;
                        if (i == 16)
                            break;
                    }
                }
                else
                {
                    foreach (Sections.ExternalTeamData data in dataSection.DataExternal.SMSReceivers)
                    {
                        str[i] = "(외부상황전파) " + data.TeamName + " 문자메시지 전송";
                        nBit = 1 << i;
                        bUse[i++] = (nCheckNotify1 & nBit) == nBit;
                        if (i == 16)
                            break;
                    }
                }
                // nIdx = 0
                int j = 0;
                if (dataSection.DataExternal.UseFax)
                {

                    foreach (Sections.ExternalTeamData data in dataSection.DataExternal.FaxReceivers)
                    {
                        str[i] = "(외부상황전파) " + data.TeamName + " 팩스 전송";
                        nBit = 1 << j;
                        bUse[i++] = (nCheckNotify2 & nBit) == nBit;
                        j++;
                        if (j == 16)
                            break;
                    }
                }
                else
                {
                    foreach (Sections.ExternalTeamData data in dataSection.DataExternal.FaxReceivers)
                    {
                        str[i] = "(외부상황전파) " + data.TeamName + " 팩스 전송";
                        nBit = 1 << j;
                        bUse[i++] = (nCheckNotify2 & nBit) == nBit;
                        j++;
                        if (j == 16)
                            break;
                    }
                }
                AddGridData(nCnt, bUse, str, section, nCheckNotify1, nCheckNotify2);
            }
            else if (sectionType == Sections.Section.ComponentType.INTERNAL/* && strStatus != "실행 완료"*/)
            {
                Sections.SectionDataInternal dataSection = (Sections.SectionDataInternal)section.Data;

                bool[] bUse = new bool[3];
                string[] str = new string[3];
                int i = 0;

                str[i] = "(내부상황전파) 팝업메시지";
                int nBit = 1 << i;
                bUse[i++] = (nCheckNotify1 & nBit) == nBit;

                str[i] = "(내부상황전파) 모바일메시지";
                nBit = 1 << i;
                bUse[i++] = (nCheckNotify1 & nBit) == nBit;
                                
                str[i] = "(내부상황전파) 사내방송";
                nBit = 1 << i;
                bUse[i++] = (nCheckNotify1 & nBit) == nBit;

                AddGridData(3, bUse, str, section, nCheckNotify1, nCheckNotify2);
            }                
            else if (sectionType == Sections.Section.ComponentType.EXTERNAL/* && strStatus != "실행 완료"*/)
            {
                Sections.SectionDataExternal dataSection = (Sections.SectionDataExternal)section.Data;
                Sections.SectionTabPage page = (Sections.SectionTabPage)FormMain.Instance.GetPageHome().TabControls.SelectedTab;                
                if (page == null)
                {
                    PanelSectionEx panel = (PanelSectionEx)section.GetParent();
                    page = (Sections.SectionTabPage)panel.Parent;
                    FormMain.Instance.GetPageHome().TabControls.SelectedTab = page;
                }

                Sections.SectionState state = Sections.WorkFlowManager.Instance.Find(section, !page.VirtualMode);

                if (page == null || state == null)
                    return;

                bool isSMS = dataSection.UseSMS;
                bool isFax = dataSection.UseFax;

                int nCnt = dataSection.SMSReceivers.Count + dataSection.FaxReceivers.Count;
                string[] str = new string[nCnt];
                bool[] bUse = new bool[nCnt];
                int i = 0;
                int nBit = 0;
                if (dataSection.UseSMS)
                {
                    foreach (Sections.ExternalTeamData data in dataSection.SMSReceivers)
                    {
                        str[i] = "(외부상황전파) " + data.TeamName + " 문자메시지 전송";
                        nBit = 1 << i;
                        bUse[i++] = (nCheckNotify1 & nBit) == nBit;
                        if( i == 16 )
                            break;
                    }
                }
                else
                {
                    foreach (Sections.ExternalTeamData data in dataSection.SMSReceivers)
                    {
                        str[i] = "(외부상황전파) " + data.TeamName + " 문자메시지 전송";
                        nBit = 1 << i;
                        bUse[i++] = (nCheckNotify1 & nBit) == nBit;
                        if (i == 16)
                            break;
                    }
                }
                // nIdx = 0
                int j = 0;
                if (dataSection.UseFax)
                {
                    
                    foreach (Sections.ExternalTeamData data in dataSection.FaxReceivers)
                    {
                        str[i] = "(외부상황전파) " + data.TeamName + " 팩스 전송";
                        nBit = 1 << j;
                        bUse[i++] = (nCheckNotify2 & nBit) == nBit;
                        j++;
                        if (j == 16)
                            break;
                    }
                }
                else
                {
                    foreach (Sections.ExternalTeamData data in dataSection.FaxReceivers)
                    {
                        str[i] = "(외부상황전파) " + data.TeamName + " 팩스 전송";
                        nBit = 1 << j;
                        bUse[i++] = (nCheckNotify2 & nBit) == nBit;
                        j++;
                        if (j == 16)
                            break;
                    }
                }
                AddGridData(nCnt, bUse, str, section, nCheckNotify1, nCheckNotify2);
            }
            else
            {
                dataGridView.Tag = section;
                HideGrid();
            }
        }

        private void AddGridData(int nRowCount, 
            bool[] useSMS, 
            bool[] useBroadcast, 
            string[] strTransType, 
            string[] strTarget, 
            string[] strMission, 
            Sections.Section section, 
            bool checkBoxReadOnly, 
            int nCheckNotify1,
            int nCheckNotify2)
        {
            dataGridView.Show();
            m_isFlag = false;

            int nRowHeight = 0;
            for (int i = 0; i < nRowCount; i++)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCheckBoxCell checkCell = new DataGridViewCheckBoxCell();
                checkCell.Value = useSMS[i];
                checkCell.ReadOnly = checkBoxReadOnly;
                checkCell.Tag = nCheckNotify1;
                gridRow.Cells.Add(checkCell);

                checkCell = new DataGridViewCheckBoxCell();
                checkCell.Value = useBroadcast[i];
                checkCell.ReadOnly = checkBoxReadOnly;
                checkCell.Tag = nCheckNotify2;
                gridRow.Cells.Add(checkCell);

                DataGridViewCell cell1 = new DataGridViewTextBoxCell();
                cell1.Value = strTransType[i]; // i.ToString() + ". 상황전파";
                gridRow.Cells.Add(cell1);
                  

                DataGridViewCell cell2 = new DataGridViewTextBoxCell();
                cell2.Value = strTarget[i]; // i.ToString() + ". 상황전파";
                cell2.ToolTipText = strTarget[i];           
                gridRow.Cells.Add(cell2);

                DataGridViewCell cell3 = new DataGridViewTextBoxCell();
                cell3.Value = strMission[i]; // i.ToString() + ". 상황전파";
                gridRow.Cells.Add(cell3);
               
                nRowHeight = gridRow.Height;

                dataGridView.Rows.Add(gridRow);
            }

            dataGridView.Columns[2].Width = 60;
            dataGridView.Columns[3].Width = 100;  

            dataGridView.Tag = section;
            dataGridView.Size = new Size(dataGridView.Width, nRowHeight * nRowCount);
            this.columnBroadcast.Visible = true;

            ReSizeForm(m_isFlag);
        }

        private void AddGridData(int nCount, bool[] bChecked, string[] szItem, Sections.Section secTarget, int nCheckNotify1, int nCheckNotify2)
        {
            dataGridView.Show();
            m_isFlag = false;
            int nRowHeight = 0;

            for (int i = 0; i < nCount; i++)
            {
                DataGridViewRow gridRow = new DataGridViewRow();
                DataGridViewCheckBoxCell checkCell = new DataGridViewCheckBoxCell();
                
                checkCell.Value = bChecked[i];
                checkCell.Tag = nCheckNotify1;
                gridRow.Cells.Add(checkCell);

                checkCell = new DataGridViewCheckBoxCell();
                gridRow.Cells.Add(checkCell);
                checkCell.Tag = nCheckNotify2;

                DataGridViewCell cellNull1 = new DataGridViewTextBoxCell();
                cellNull1.Value = "";
                gridRow.Cells.Add(cellNull1);

                DataGridViewCell cellNull2 = new DataGridViewTextBoxCell();
                cellNull2.Value = "";
                gridRow.Cells.Add(cellNull2);

                DataGridViewCell cell = new DataGridViewTextBoxCell();
                cell.Value = szItem[i]; 
                gridRow.Cells.Add(cell);
                
                nRowHeight = gridRow.Height;
                dataGridView.Rows.Add(gridRow);
            }
            dataGridView.Tag = secTarget;
            dataGridView.Size = new Size(dataGridView.Width, nRowHeight * nCount);

            this.columnBroadcast.Visible = false;
            this.Column2.Visible = false;
            this.Column3.Visible = false;
            label.Location = new Point(pictureBoxSMS.Location.X, label.Location.Y);
            pictureBoxSMS.Visible = false;
            pictureBoxBroadcast.Visible = false;

            ReSizeForm(m_isFlag);
        }

        public void UpdateContents(int nCheckedNotify1, int nCheckedNotify2)
        {
            if (pictureBoxSMS.Visible)  // Process
            {
                int nRowCount = dataGridView.Rows.Count;

                for (int i=0;i<nRowCount;i++)
                {
                    DataGridViewRow row = dataGridView.Rows[i];
                    int nBitFlag = 1 << i;

                    row.Cells[0].Value = (nCheckedNotify1 & nBitFlag) == nBitFlag;
                    row.Cells[1].Value = (nCheckedNotify2 & nBitFlag) == nBitFlag;
                }
            }
            else
            {
                int nRowCount = dataGridView.Rows.Count;

                for (int i = 0; i < nRowCount; i++)
                {
                    DataGridViewRow row = dataGridView.Rows[i];
                    int nBitFlag = 1 << i;

                    row.Cells[0].Value = (nCheckedNotify1 & nBitFlag) == nBitFlag;
                }
            }
        }

        public Image GetImage(bool isFlag)
        {
            Bitmap bmp = new Bitmap(global::SOPMonitoringSystem.Properties.Resources.btn_arrow2);

            ImageList imgList = new ImageList();
            imgList.ImageSize = new Size(32, 32);
            imgList.Images.AddStrip(bmp);

            int nFlag = 0;
            if (!isFlag) nFlag = 1;

            Image img = imgList.Images[nFlag];

            return img;
        }

        public void ReSizeForm(bool isFlag)
        {
            Size szPanel = panel.Size;
            Size szGrid = dataGridView.Size;

            if (!isFlag)
                this.Size = new Size(this.Width, szPanel.Height + szGrid.Height);
            else
                this.Size = new Size(this.Width, szPanel.Height);

            FormMain.Instance.GetPageHome().ReLocation();
        }

        public Panel GetPanel()
        {
            return panel;
        }

        public DataGridView gridView
        {
            get { return dataGridView; }
        }

        private void pictureBoxBroadcast_MouseClick(object sender, MouseEventArgs e)
        {
            pictureBoxClicked(1);
        }

        private void pictureBoxSMS_MouseClick(object sender, MouseEventArgs e)
        {
            pictureBoxClicked(0);           
        }

        private void pictureBoxClicked(int nColumnIndex)
        {
            int nRowCount = dataGridView.Rows.Count;

            if (nRowCount == 0)
                return;

            DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)dataGridView.Rows[0].Cells[nColumnIndex];
            bool isChecked = !(bool)checkCell.EditedFormattedValue;

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                row.Cells[nColumnIndex].Value = isChecked;
            }

            Sections.Section section = (Sections.Section)dataGridView.Tag;
            Sections.SectionTabPage page = (Sections.SectionTabPage)FormMain.Instance.GetPageHome().TabControls.SelectedTab;
            if (page == null)
            {
                PanelSectionEx panel = (PanelSectionEx)section.GetParent();
                page = (Sections.SectionTabPage)panel.Parent;
                FormMain.Instance.GetPageHome().TabControls.SelectedTab = page;
            }
            Sections.SectionState state = Sections.WorkFlowManager.Instance.Find(section, !page.VirtualMode);

            if (section.GetComponentType() == Sections.Section.ComponentType.PROCESS/* ||
                section.GetComponentType() == Sections.Section.ComponentType.INTERNAL*/)
            {
                int nCheckedNotify = 0;

                if (isChecked)
                {
                    for (int i = 0; i < nRowCount; i++)
                    {
                        nCheckedNotify |= (1 << i);
                    }
                }

                Sections.SectionDataProcess data = (Sections.SectionDataProcess)section.Data;

                for (int i = 0; i < data.MissionItems.Count; i++)
                {
                    Sections.MissionItem item = (Sections.MissionItem)data.MissionItems[i];

                    MissionItemInfo info = FormMain.Instance.GetMissionInfo(item);

                    if (info == null)
                    {
                        info = new MissionItemInfo();

                        if (nColumnIndex == 0)
                        {
                            info.UseSMS = isChecked;
                            info.UseBroadcast = (bool)dataGridView.Rows[i].Cells[1].EditedFormattedValue;
                        }
                        else
                        {
                            info.UseBroadcast = isChecked;
                            info.UseSMS = (bool)dataGridView.Rows[i].Cells[0].EditedFormattedValue;
                        }

                        FormMain.Instance.SetMissionInfo(item, info);
                    }
                    else
                    {
                        if (nColumnIndex == 0)
                            info.UseSMS = isChecked;
                        else
                            info.UseBroadcast = isChecked;
                    }

                    if (!info.UseSMS && !info.UseBroadcast)
                        item.CheckItem = false;
                    else
                        item.CheckItem = true;

                    int nBitFlag = 1 << i;
                    state.CheckNotify1 = state.CheckNotify1 & (~nBitFlag);
                    state.CheckNotify2 = state.CheckNotify2 & (~nBitFlag);

                    if (info.UseSMS)
                        state.CheckNotify1 = state.CheckNotify1 | nBitFlag;

                    if (info.UseBroadcast)
                        state.CheckNotify2 = state.CheckNotify2 | nBitFlag;

                    if (i == 15)
                        break;
                }

                if (nColumnIndex == 0)
                    state.CheckNotify1 = nCheckedNotify;
                else
                    state.CheckNotify2 = nCheckedNotify;

                if (LogGridRow != null)
                {
                    LogGridRow.Cells[5].Tag = state.CheckNotify1;
                    LogGridRow.Cells[6].Tag = state.CheckNotify2;
                }
            }
        }

        public bool GetItem(int nIndex, out bool check1, out bool check2, out string strMessenger, out string strTeamName, out string strItem)
        {
            check1 = check2 = false;
            strItem = "";
            strMessenger = "";
            strTeamName = "";

            if (nIndex < 0 || nIndex >= ItemCount)
                return false;

            if (dataGridView.Rows[nIndex].Cells[2].Value == null)
                return false;

            check1 = (bool)dataGridView.Rows[nIndex].Cells[0].Value;
            check2 = dataGridView.Rows[nIndex].Cells[1].Value == null ? false : (bool)dataGridView.Rows[nIndex].Cells[1].Value;
            strMessenger = (string)dataGridView.Rows[nIndex].Cells[2].Value;
            strTeamName = (string)dataGridView.Rows[nIndex].Cells[3].Value;
            strItem = (string)dataGridView.Rows[nIndex].Cells[4].Value;

            Sections.Section section = this.Section;

            if (section != null)
            {
                Sections.Section.ComponentType type = section.GetComponentType();

                if (type == Sections.Section.ComponentType.INTERNAL ||
                    type == Sections.Section.ComponentType.EXTERNAL)
                {
                    if (check1)
                        strItem = strItem + " 사용";
                    else
                        strItem = strItem + " 사용안함";
                }
            }

            int nLen = strMessenger.Length;
            string strTemp = strMessenger;
            strMessenger = "";

            for (int i = 0; i < nLen; i++)
            {
                if (i < nLen - 1)
                    strMessenger += strTemp[i] + "\r\n";
                else
                    strMessenger += strTemp[i];
            }

            return true;
        }

        public void EnableGrid(bool enabled)
        {
            int nColumnCount = gridView.Columns.Count;
            if (nColumnCount == 0)
                return;

            Color disabledGridColor = Color.LightGray;

            foreach (DataGridViewRow row in gridView.Rows)
            {
                for (int i = 0; i < nColumnCount - 1; i++)
                {
                    row.Cells[i].ReadOnly = !enabled;
                }

                if (!enabled)
                {
                    for (int i=0;i<nColumnCount;i++)
                    {
                        row.Cells[i].Style.BackColor = disabledGridColor;
                    }
                }
            }

            pictureBoxSMS.Enabled = enabled;
            pictureBoxBroadcast.Enabled = enabled;
        }

        public int ComponentHistoryID
        {
            get { return m_nComponentHistoryID; }
            set { m_nComponentHistoryID = value; }
        }

        public int ComponentID
        {
            get { return m_nComponentID; }
            set { m_nComponentID = value; }
        }

        public Sections.State State
        {
            get { return m_state; }
            set { m_state = value; }
        }

        public string Title
        {
            get { return label.Text; }
        }

        public DataLogGridViewRow LogGridRow
        {
            get { return m_logGridRow; }
            set { m_logGridRow = value; }
        }

        public bool UseSMS
        {
            get { return pictureBoxSMS.Visible; }
        }

        public bool UseBroadcast
        {
            get { return pictureBoxBroadcast.Visible; }
        }

        public Sections.Section.ComponentType ContentsType
        {
            get
            {
                if (m_logGridRow == null)
                    return Sections.Section.ComponentType.NONE;

                Sections.Section section = m_logGridRow.Section;
                if (section == null)
                    return Sections.Section.ComponentType.NONE;

                return section.GetComponentType();
            }
        }

        public int ItemCount
        {
            get { return dataGridView.Rows.Count; }
        }

        public Sections.Section Section
        {
            get { return (Sections.Section)dataGridView.Tag; }
        }


		public void SelectRow(int nRow)
		{
			if (gridView.RowCount <= nRow)
				return;
			
			gridView.ClearSelection();
			m_bSelectedGridRow = false;

			gridView.Rows[nRow].Selected = true;
		}
		

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            ArrayList arrSelectedRows = new ArrayList();

            foreach (DataGridViewCell cell in gridView.SelectedCells)
            {
                if (arrSelectedRows.Contains(cell.RowIndex))
                    continue;

                arrSelectedRows.Add(cell.RowIndex);
            }

            if (FormMain.Instance.FrmMain3 != null)
                FormMain.Instance.FrmMain3.SelectRows(arrSelectedRows, this);

            int nCount = arrSelectedRows.Count;
            if (nCount > 0)
            {
                DataGridView grid = (DataGridView)sender;

                int nRowIdx = (int)arrSelectedRows[nCount - 1];
                if (grid.Rows[nRowIdx].Cells.Count == 5)
                {
					DataGridViewRow row = grid.Rows[nRowIdx];
					
					DataGridViewCell cell1 = row.Cells[4];
					string szMissionText = (string)cell1.Value;

					DataGridViewCell cell2 = row.Cells[3];
					string szToTarget = (string)cell2.Value;

					string szValueMedium = (string)row.Cells[2].Value;

					if (szMissionText == null)
						szMissionText = "";
					if (szToTarget == null)
						szToTarget = "";
                    PopupMissionText form = PopupMissionText.Instance;
					form.SetText(szMissionText, szToTarget, szValueMedium);

					if (FormMain.Instance.HasControl == true)
					{
						if (this.ContentsType == Sections.Section.ComponentType.PROCESS)
						{
							int nRow = nRowIdx;
							int nCompID = this.m_nComponentHistoryID;

							if (Section != null && Section.GetParent() != null)
							{
								Sections.PanelSectionEx panel = (Sections.PanelSectionEx)this.Section.GetParent();
								SectionTabPage tabPage = (SectionTabPage)panel.Parent;
								if (tabPage != null)
								{
									int bRealMode = ((!tabPage.VirtualMode) == true) ? 1 : 0;
									int nActionSID = tabPage.ActionStepID;

									if( WorkFlowManager.Instance.Get(nActionSID, (bRealMode == 1 ? true : false)) != null)
									{
										WorkFlow work = WorkFlowManager.Instance.Get(nActionSID, (bRealMode == 1 ? true : false));
										if( work.State == WorkFlowState.RUN)
										{
											if (nActionSID > 0 && this.Visible == true)
											{
												NetworkManager.Instance.SendSelectMission(nActionSID, bRealMode, nCompID, nRow);
											}
										}
									}
								}
							}
						}
					}
					
                }
            }
        }


    }
}
