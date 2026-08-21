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
using System.Text.RegularExpressions;
using UnE.Utility.Print;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Office.Core;
using Excel = Microsoft.Office.Interop.Excel;

namespace RoadMan
{
	public partial class FormSearch : Form
	{

		private Dictionary<string, string> m_FindResult = new Dictionary<string, string>();
		private string szExtraText = "대로";

		private bool m_bUseRate = false;
		private bool m_bUseWidth = false;
		private bool m_bUseLength = false;
		private bool m_bUseCost = false;
		private bool m_bUseArea = false;
		private bool m_bUsePeriod = false;

		// 시작날짜 사용
		private bool m_bUsePeriod1 = false;
		// 한계날짜 사용
		private bool m_bUsePeriod2 = false;

		private bool m_bUseLotNum = false;

		// 도로폭
		private string m_szWidth1 = "";
		private string m_szWidth2 = "";

		// 공정율
		// 공정율 계산방법 : 1 (영역) , 2 (길이)
		private int m_nProcessRateMethod = 1;

		private string m_szRate1 = "";
		private string m_szRate2 = "";

		// 구간길이
		private string m_szLength1 = "";
		private string m_szLength2 = "";

		// 사업비
		private string m_szCost1 = "";
		private string m_szCost2 = "";

		// 구간면적
		private string m_szArea1 = "";
		private string m_szArea2 = "";

		// 지번
		//private string m_szLotNumber = "";
		private ArrayList m_arLotNumbers = new ArrayList();
        private ArrayList m_arLotNumberString = new ArrayList();

		// 사업 기간
		private DateTime m_dtPeriod1;
		private DateTime m_dtPeriod2;

		private bool m_bUseRateDown = false;
		private double m_fRateDown = 0.0f;
		private bool m_bUseRateUp = false;
		private double m_fRateUp = 0.0f;

		private bool m_bUseWidthDown = false;
		private float m_fWidthDown = 0.0f;
		private bool m_bUseWidthUp = false;
		private float m_fWidthUp = 0.0f;

		private bool m_bUseLengthDown = false;
		private float m_fLengthDown = 0.0f;
		private bool m_bUseLengthUp = false;
		private float m_fLengthUp = 0.0f;

		private bool m_bUseCostDown = false;
		private long m_nCostDown = 0L;
		private bool m_bUseCostUp = false;
		private long m_nCostUp = 0L;

		private bool m_bUseAreaDown = false;
		private float m_fAreaDown = 0.0f;
		private bool m_bUseAreaUp = false;
		private float m_fAreaUp = 0.0f;

        // Key : StreetName Tag
        // Value : Major List
        private Dictionary<string, List<AddrString>> m_dicStreetMajors = new Dictionary<string, List<AddrString>>();
        // Key : StreetName + Major
        // Value : Minor List
        private Dictionary<string, List<AddrString>> m_dicStreetMinors = new Dictionary<string, List<AddrString>>();

        // Key : 읍/면/동 이름
        // Value : 리 이름 List
        private Dictionary<string, List<AddrString>> m_dicLandAddrTowns = new Dictionary<string, List<AddrString>>();
        // Key : 읍/면/동 + 리
        // Value : Major List
        private Dictionary<string, List<AddrString>> m_dicLandAddrMajors = new Dictionary<string, List<AddrString>>();
        // Key : 읍/면/동 + 리 + Major
        // Value : Minor List
        private Dictionary<string, List<AddrString>> m_dicLandAddrMinors = new Dictionary<string, List<AddrString>>();

		public FormSearch()
		{
			InitializeComponent();
			tbLoadName.AutoCompleteMode = AutoCompleteMode.Suggest;
			tbLoadName.AutoCompleteSource = AutoCompleteSource.CustomSource;
		}


		private string m_szSearchName = "";
		private string m_szExtraName = "";
		private bool FindStreetForName(ScheduleProperty data)
		{
			string szName = data.StreetName;
			if (szName.IndexOf(m_szExtraName) == -1)
				return false;

			if (szName.Contains(m_szSearchName))
				return true;
			return false;
		}

		private void FindStreet(string szExtra, string szStreet)
		{			
			PanelDXFViewer panel = FormMain.Instance.CurrentPanel;
			if (panel == null)
				return;
					
			ArrayList arList = new ArrayList();

			
			List<ProcessSchedule> results = panel.ProcessSchedules;
			m_szSearchName = szStreet;
			m_szExtraName = szExtra;

			foreach (ProcessSchedule result in results)
			{
				
				List<ScheduleProperty> findList = ((List<ScheduleProperty>)result.Properties).FindAll(FindStreetForName);

				if (findList != null)
				{
					foreach (ScheduleProperty prop in findList)
					{
						if (!arList.Contains(prop))
							arList.Add(prop);
					}
				}
			}
			SetResultGrid(arList);				

		}

        private void MakeLandAddressData()
        {
            m_dicLandAddrTowns.Clear();
            m_dicLandAddrMajors.Clear();
            m_dicLandAddrMinors.Clear();

            cboDong.Items.Clear();
            cboRi.Items.Clear();
            cboLandMajor.Items.Clear();
            cboLandMinor.Items.Clear();

            PanelDXFViewer panel = FormMain.Instance.CurrentPanel;

            if (panel == null)
                return;

            List<AddrString> ris = new List<AddrString>();
            List<AddrString> majors = new List<AddrString>();
            List<AddrString> minors = new List<AddrString>();

            foreach (KeyValuePair<string, LandAddressData2> pair in panel.DataManager.LandAddressDatas)
            {
                LandAddressData2 data = pair.Value;
                string strTownName = data.Dong;

                if (!m_dicLandAddrTowns.TryGetValue(data.Dong, out ris))
                {
                    ris = new List<AddrString>();
                    m_dicLandAddrTowns[data.Dong] = ris;
                }

                if (data.Ri != null && !AddrString.Contains(ris, data.Ri))
                {
                    ris.Add(new AddrString(data.Ri));
                }

                if (data.Ri != null && data.Ri.Length > 0)
                    strTownName += " " + data.Ri;

                if (!m_dicLandAddrMajors.TryGetValue(strTownName, out majors))
                {
                    majors = new List<AddrString>();
                    m_dicLandAddrMajors[strTownName] = majors;
                }

                if (!AddrString.Contains(majors, data.MajorAddr))
                    majors.Add(new AddrString(data.MajorAddr));

                string strTownMajor = strTownName + " " + data.MajorAddr;

                if (!m_dicLandAddrMinors.TryGetValue(strTownMajor, out minors))
                {
                    minors = new List<AddrString>();
                    m_dicLandAddrMinors[strTownMajor] = minors;
                }

                if (!AddrString.Contains(minors, data.MinorAddr))
                    minors.Add(new AddrString(data.MinorAddr));
            }

            foreach (KeyValuePair<string, List<AddrString>> pair in m_dicLandAddrTowns)
            {
                cboDong.Items.Add(pair.Key);
                pair.Value.Sort();
            }

            foreach (KeyValuePair<string, List<AddrString>> pair in m_dicLandAddrMajors)
            {
                pair.Value.Sort();
            }

            foreach (KeyValuePair<string, List<AddrString>> pair in m_dicLandAddrMinors)
            {
                pair.Value.Sort();
            }

            string strPrev = cboDong.Text;

            if (cboDong.Items.Count > 0)
                cboDong.Text = (string)cboDong.Items[0];

            // cboDong.Text가 변하지 않으면 TextChanged 이벤트가 호출되지 않으므로 강제로 함수를 호출해준다.
            if (strPrev == cboDong.Text)
                cboDong_TextChanged(null, null);
        }

        private void SetStreetNames(PanelDXFViewer panel, string strSubCategory)
        {
            string strName = null, strMajor = null, strMinor = null;
            List<AddrString> majors = null;
            List<AddrString> minors = null;

            foreach (ProcessSchedule schedule in panel.ProcessSchedules)
            {
                foreach (ScheduleProperty prop in schedule.Properties)
                {
                    if (strSubCategory != null && prop.SubCategory != strSubCategory)
                        continue;

                    ParseStreetName(prop.StreetName, out strName, out strMajor, out strMinor);

                    if (!m_dicStreetMajors.TryGetValue(strName, out majors))
                    {
                        majors = new List<AddrString>();
                        m_dicStreetMajors[strName] = majors;
                    }

                    if (strMajor != null)
                    {
                        if (!AddrString.Contains(majors, strMajor))
                            majors.Add(new AddrString(strMajor));

                        if (!m_dicStreetMinors.TryGetValue(strName + strMajor, out minors))
                        {
                            minors = new List<AddrString>();
                            m_dicStreetMinors[strName + strMajor] = minors;
                        }

                        if (strMinor != null && !AddrString.Contains(minors, strMinor))
                            minors.Add(new AddrString(strMinor));
                    }
                }
            }

            List<string> streetNames = new List<string>();
            string strBig = null, strMiddle = null, strSmall = null;

            foreach (KeyValuePair<string, List<AddrString>> pair in m_dicStreetMajors)
            {
                if (pair.Key == "대로")
                    strBig = pair.Key;
                else if (pair.Key == "중로")
                    strMiddle = pair.Key;
                else if (pair.Key == "소로")
                    strSmall = pair.Key;
                else
                    streetNames.Add(pair.Key);

                pair.Value.Sort();
            }

            foreach (KeyValuePair<string, List<AddrString>> pair in m_dicStreetMinors)
            {
                pair.Value.Sort();
            }

            streetNames.Sort();

            if (strBig != null)
                cboStreetName.Items.Add(strBig);
            if (strMiddle != null)
                cboStreetName.Items.Add(strMiddle);
            if (strSmall != null)
                cboStreetName.Items.Add(strSmall);

            foreach (string strStreetName in streetNames)
            {
                cboStreetName.Items.Add(strStreetName);
            }

            if (strSubCategory == null)
            {
            }
            else if (cboStreetName.Items.Contains(strSubCategory))
                cboStreetName.Text = strSubCategory;
        }

        private void ParseStreetName(string strStreetName, out string strName, out string strMajor, out string strMinor)
        {
            strName = strMajor = strMinor = null;

            int nIndex = strStreetName.IndexOf('-');

            if (nIndex < 0)
            {
                strName = strStreetName.Trim();
                return;
            }

            for (int i=0;i<nIndex;i++)
            {
                char ch = strStreetName.ElementAt(i);

                if (ch >= '0' && ch <= '9')
                {
                    strName = strStreetName.Substring(0, i);
                    break;
                }
            }

            if (strName == null)
                strName = strStreetName.Substring(0, nIndex);
            else
            {
                int nLen = strName.Length;

                if (nLen < nIndex)
                    strMajor = strStreetName.Substring(nLen, nIndex - nLen);
            }

            if (nIndex < strStreetName.Length - 1)
                strMinor = strStreetName.Substring(nIndex + 1);

            strName = strName.Trim();

            if (strMajor != null)
                strMajor = strMajor.Trim();

            if (strMinor != null)
                strMinor = strMinor.Trim();
        }
		
		private void MakeAutoCompleteData()
		{
            PanelDXFViewer panel = FormMain.Instance.CurrentPanel;
            if (panel == null)
                return;

            m_dicStreetMajors.Clear();
            m_dicStreetMinors.Clear();
            cboStreetName.Items.Clear();
            cboStreetMajor.Items.Clear();
            cboStreetMinor.Items.Clear();

            if (rbAllStreet.Checked)
                SetStreetNames(panel, null);
            else if (rbLargeStreet.Checked)
                SetStreetNames(panel, "대로");
            else if (rbMiddleStreet.Checked)
                SetStreetNames(panel, "중로");
            else if (rbSmallStreet.Checked)
                SetStreetNames(panel, "소로");

            //List<string> streetList = panel.DataManager.StreetShapes.Keys.ToList<string>();

			//if (rbLargeStreet.Checked == true)
			//{
			//	szExtraText = "대로";
			//}
			//else if (rbMiddleStreet.Checked == true)
			//{
			//	szExtraText = "중로";
			//}
			//else if (rbSmallStreet.Checked == true)
			//{
			//	szExtraText = "소로";
			//}
			//else if (rbAllStreet.Checked == true)
			//{
			//	szExtraText = "";
			//}
			
			//tbLoadName.AutoCompleteCustomSource.Clear();
			//foreach (string szName in streetList)
			//{
			//	if (szExtraText != "")
			//	{
			//		if (szName.IndexOf(szExtraText) != -1)
			//		{
			//			string szStreet = szName.Replace(szExtraText, "");
			//			szStreet = szStreet.Trim();
			//			if (!tbLoadName.AutoCompleteCustomSource.Contains(szStreet))
			//				tbLoadName.AutoCompleteCustomSource.Add(szStreet);
			//		}
			//	}
			//	else
			//	{
			//		if (!tbLoadName.AutoCompleteCustomSource.Contains(szName))
			//			tbLoadName.AutoCompleteCustomSource.Add(szName);
			//	}
			//}

			//mGridResult.ClearSelection();
			//mGridResult.Rows.Clear();
			//tbLoadName.Text = "";
		}


		private void FormSearch_Activated(object sender, EventArgs e)
		{
		}

		private void FormSearch_Load(object sender, EventArgs e)
		{
            PanelDXFViewer panel = FormMain.Instance.CurrentPanel;
            if (panel == null)
                return;

            //MakeAutoCompleteData();
            //MakeLandAddressData();

            EnableWidth(false);

            EnableProcessRate(false);

            EnableLength(false);

            EnableCost(false);

            EnableArea(false);

            EnablePeriod(false);

            EnableStreetName(false);

            EnableLotNum(false);

            ShowOptionPane();

            dtPeriod2.ShowCheckBox = true;
            dtPeriod2.Checked = false;
            dtPeriod1.ShowCheckBox = true;
            dtPeriod1.Checked = false;
		}

		private void rbAllStreet_CheckedChanged(object sender, EventArgs e)
		{
			if (rbAllStreet.Checked == true)
			{
				ClearSelection();
				MakeAutoCompleteData();
			}		
		}

		private void rbSmallStreet_CheckedChanged(object sender, EventArgs e)
		{
			if (rbSmallStreet.Checked == true)
			{
				ClearSelection();
				MakeAutoCompleteData();
			}
		}

		private void rbMiddleStreet_CheckedChanged(object sender, EventArgs e)
		{
			if (rbMiddleStreet.Checked == true)
			{
				ClearSelection();
				MakeAutoCompleteData();
			}
		}

		private void rbLargeStreet_CheckedChanged(object sender, EventArgs e)
		{
			if (rbLargeStreet.Checked == true)
			{
				ClearSelection();
				MakeAutoCompleteData();
			}
		}

		private void ClearSelection()
		{
			PanelDXFViewer panel = FormMain.Instance.CurrentPanel;
			if (panel != null)
			{
                ClearLandAddrLayer();
				panel.ClearFixedSelection();
				panel.DXFControl.Refresh();
			}
		}

        private DXFViewer.Layer ClearLandAddrLayer()
        {
            PanelDXFViewer panel = FormMain.Instance.CurrentPanel;
            
            if (panel != null)
            {
                DXFExternPainter painter = (DXFExternPainter)panel.DXFControl.ExternalPainter;
                DXFViewer.Layer layer = painter.GetLayer(DXFExternPainter.LayerType.LAND_ADDRESS);

                if (layer != null)
                    layer.RemoveAll();

                return layer;
            }

            return null;
        }

		/*private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			int nIdx = e.RowIndex;
			if( nIdx != -1)
			{
				DataGridViewRow row = mGridResult.Rows[nIdx];
				if( row != null)
				{
					string szName = (string)row.Tag;
					PanelDXFViewer panel = FormMain.Instance.CurrentPanel;
					if( panel != null)
						panel.DataManager.ObjectZoom(szName, panel);
				}
			}
		}*/

		private void FormSearch_FormClosing(object sender, FormClosingEventArgs e)
		{
			ClearSelection();

			FormMain.Instance.EndLoadSearch();

			e.Cancel = true;
		}		

		private void btnClose_Click(object sender, EventArgs e)
		{
			tbLoadName.AutoCompleteCustomSource.Clear();
			m_FindResult.Clear();
			mGridResult.Rows.Clear();

			ClearSelection();

			FormMain.Instance.EndLoadSearch();
		}
		
		private void ParsingValues()
		{
			m_bUseWidthDown = false;
			m_fWidthDown = 0.0f;
			m_bUseWidthUp = false;
			m_fWidthUp = 0.0f;

			if(m_bUseWidth == true)
			{
				if(m_szWidth1 != null && m_szWidth1 != "")
				{
					if(float.TryParse(m_szWidth1, out m_fWidthDown))
					{
						m_bUseWidthDown = true;
					}
				}
				if(m_szWidth2 != null && m_szWidth2 != "")
				{
					if(float.TryParse(m_szWidth2, out m_fWidthUp))
					{
						m_bUseWidthUp = true;
					}
				}		
			}

			m_bUseLengthDown = false;
			m_fLengthDown = 0.0f;
			m_bUseLengthUp = false;
			m_fLengthUp = 0.0f;

			if(m_bUseLength == true)
			{
				if(m_szLength1 != null && m_szLength1 != "")
				{
					if(float.TryParse(m_szLength1, out m_fLengthDown))
					{
						m_bUseLengthDown = true;
					}
				}
				if(m_szLength2 != null && m_szLength2 != "")
				{
					if(float.TryParse(m_szLength2, out m_fLengthUp))
					{
						m_bUseLengthUp = true;
					}
				}		
			}	

			m_bUseCostDown = false;
			m_nCostDown = 0L;
			m_bUseCostUp = false;
			m_nCostUp = 0L;

			if(m_bUseCost == true)
			{
				if(m_szCost1 != null && m_szCost1 != "")
				{
					if(long.TryParse(m_szCost1, out m_nCostDown))
					{
						m_bUseCostDown = true;
					}
				}
				if(m_szCost2 != null && m_szCost2 != "")
				{
					if(long.TryParse(m_szCost2, out m_nCostUp))
					{
						m_bUseCostUp = true;
					}
				}		
			}
	
			m_bUseAreaDown = false;
			m_fAreaDown = 0.0f;
			m_bUseAreaUp = false;
			m_fAreaUp = 0.0f;

			if(m_bUseArea == true)
			{
				if(m_szArea1 != null && m_szArea1 != "")
				{
					if(float.TryParse(m_szArea1, out m_fAreaDown))
					{
						m_bUseAreaDown = true;
					}
				}
				if(m_szArea2 != null && m_szArea2 != "")
				{
					if(float.TryParse(m_szArea2, out m_fAreaUp))
					{
						m_bUseAreaUp = true;
					}
				}		
			}

			m_bUseRateDown = false;
			m_fRateDown = 0.0;
			m_bUseRateUp = false;
			m_fRateUp = 0.0;

			if (m_bUseArea == true)
			{
				if (m_szRate1 != null && m_szRate1 != "")
				{
					if (double.TryParse(m_szRate1, out m_fRateDown))
					{
						m_bUseRateDown = true;
					}
				}
				if (m_szRate2 != null && m_szRate2 != "")
				{
					if (double.TryParse(m_szRate2, out m_fRateUp))
					{
						m_bUseRateUp = true;
					}
				}
			}	
		}

        private void ParseLotNumbers()
        {
            m_arLotNumberString.Clear();
            string[] arrNumbers = tbLotNum.Text.Split(',');

            foreach (string strLotNumber in arrNumbers)
            {
                string strNumber = strLotNumber.Trim();

                if (strNumber.Length > 0)
                    m_arLotNumberString.Add(strNumber);
            }
        }

		private void btnSearch_Click(object sender, EventArgs e)
		{
            ParseLotNumbers();

			// 검색 값을 파싱한다.
			ParsingValues();

			PanelDXFViewer panel = FormMain.Instance.CurrentPanel;
			if(panel != null)
			{
				ArrayList arList = new ArrayList();

				List<ProcessSchedule> results = panel.ProcessSchedules;
				foreach (ProcessSchedule result in results)
				{
					List<ScheduleProperty> findList = ((List<ScheduleProperty>)result.Properties).FindAll(FindScheduleProperty);

					if (findList != null)
					{
						foreach (ScheduleProperty prop in findList)
						{
							if (!arList.Contains(prop))
								arList.Add(prop);
						}
					}
				}
	
				SetResultGrid(arList);			

			}

			ShowResultPane();
		}

		private void SetResultGrid(ArrayList arDatas)
		{
            arDatas.Sort();

			int nCount = 1;
			mGridResult.ClearSelection();
			mGridResult.Rows.Clear();
			foreach (ScheduleProperty prop in arDatas)
			{
				DataGridViewRow row = new DataGridViewRow();

				DataGridViewTextBoxCell cell1 = new DataGridViewTextBoxCell();
				cell1.Value = nCount;
				cell1.ValueType = typeof(int);
				nCount++;
				row.Cells.Add(cell1);
				row.Tag = prop;

				// 도로명
				DataGridViewTextBoxCell cell8 = new DataGridViewTextBoxCell();
				cell8.Value = prop.StreetName;
				row.Cells.Add(cell8);

				// 내용
				DataGridViewTextBoxCell cell9 = new DataGridViewTextBoxCell();
				cell9.Value = prop.SubCategory;
				row.Cells.Add(cell9);


				// 도로폭
				DataGridViewTextBoxCell cell2 = new DataGridViewTextBoxCell();
				if (prop.Width != null)
					cell2.Value = prop.Width.Data.ToString();
				else
					cell2.Value = "";
				row.Cells.Add(cell2);

				// 공정율
				DataGridViewTextBoxCell cell3 = new DataGridViewTextBoxCell();
				double rate = 0.0;
				ResultProperty data = FindResultProperty(prop);
				if (data != null)
				{					
					if (m_nProcessRateMethod == 1)
					{						
						if (prop.Area != null)
						{
							double dResult = 0.0;
							double dSchedule = 0.0;

							if (data != null)
							{
								dResult = data.TotalArea;
								dSchedule = prop.Area.Data;
								if (dResult < 0.001 && dResult > -0.001)
									rate = 0.0;
								else
									rate = dSchedule / dResult * 100;
							}							
						}						
					}
					else
					{
						if (prop.Length == null)
						{
							double dResult = 0.0;
							double dSchedule = 0.0;							
							if (data != null)
							{
								dResult = data.TotalLength;
								dSchedule = prop.Length.Data;
								if (dResult < 0.001 && dResult > -0.001)
									rate = 0.0;
								else
									rate = dSchedule / dResult * 100;
							}							
						}
					}					
				}

                if (rate > 100.0)
                    rate = 100.0;
                else if (rate < 0.0)
                    rate = 0.0;

				cell3.Value = rate;
				cell3.ValueType = typeof(double);
				row.Cells.Add(cell3);

				// 공사길이
				DataGridViewTextBoxCell cell4 = new DataGridViewTextBoxCell();
				if (prop.Length != null)
				{
					cell4.Value = string.Format("{0,12:N0}", prop.Length.Data);
				}
				else
					cell4.Value = "";
				row.Cells.Add(cell4);

				// 공사면적
				DataGridViewTextBoxCell cell5 = new DataGridViewTextBoxCell();
				if (prop.Area != null)
					cell5.Value = string.Format("{0,12:N0}", prop.Area.Data);
				else
					cell5.Value = "";
				row.Cells.Add(cell5);

				// 사업비
				DataGridViewTextBoxCell cell6 = new DataGridViewTextBoxCell();
				cell6.Value = prop.TotalCost;
				row.Cells.Add(cell6);

				// 사업기간
				DataGridViewTextBoxCell cell7 = new DataGridViewTextBoxCell();
				string szText = "";

                // 집행계획의 기간을 사용
                szText = prop.Schedule.Description;
                // 집행진행상황의 기간을 사용
				/*ResultProperty data2 = FindResultProperty(prop);
				if (data2 != null)
				{
					DateTime dtStart;
					DateTime dtEnd;

					if (data != null || data.PropertyDatas.Count > 0)
					{
						bool bMin, bMax;
						GetBeginMinDate(data, out dtStart, out bMin, out dtEnd, out bMax );

						if (bMin == true)
							szText += dtStart.ToShortDateString() + "부터 ";
						if (bMax == true)
						{
							szText += dtEnd.ToShortDateString() + "까지";
						}
					}
				}*/

				cell7.Value = szText;
				row.Cells.Add(cell7);


				mGridResult.Rows.Add(row);
			}
		}


		private ScheduleProperty mFindSchedule = null;
		private bool FindResult(ResultProperty prop)
		{
			if (mFindSchedule == null)
				return false;

			if (prop.ScheduleProperty == mFindSchedule)
				return true;
			return false;

		}

		private ResultProperty FindResultProperty(ScheduleProperty data)
		{
			PanelDXFViewer panel = FormMain.Instance.CurrentPanel;
			if( panel == null)
				return null;
			mFindSchedule = data;
			List<ProcessResult> results = panel.ProcessResults;
			foreach (ProcessResult result in results)
			{
				
				ResultProperty find = ((List<ResultProperty>)result.ResultProperties).Find(FindResult);
				if (find != null)
				{
					return find;
				}
			}
			return null;
		}

        private bool CheckStreetName(ScheduleProperty prop)
        {
            string strName, strMajor, strMinor;
            ParseStreetName(prop.StreetName, out strName, out strMajor, out strMinor);

            if (cboStreetName.Text.Length == 0)
                return true;
            else
            {
                if (strName == null)
                    return false;
                else if (cboStreetName.Text != strName)
                    return false;
            }

            if (cboStreetMajor.Text.Length == 0)
                return true;
            else
            {
                if (strMajor == null)
                    return false;
                else if (cboStreetMajor.Text != strMajor)
                    return false;
            }

            if (cboStreetMinor.Text.Length == 0)
                return true;
            else
            {
                if (strMinor == null)
                    return false;
                else if (cboStreetMinor.Text != strMinor)
                    return false;
            }

            return true;
        }

		private bool FindScheduleProperty(ScheduleProperty dd)
		{		
			bool bResult = true;
			ScheduleProperty prop = dd;
			if (prop == null)
				return false;


            if (checkStreetName.Checked)
            {
                if (!CheckStreetName(prop))
                    return false;
            }
			/*if (rbLargeStreet.Checked == true)
			{
				szExtraText = "대로";
			}
			else if (rbMiddleStreet.Checked == true)
			{
				szExtraText = "중로";
			}
			else if (rbSmallStreet.Checked == true)
			{
				szExtraText = "소로";
			}
			else if (rbAllStreet.Checked == true)
			{
				szExtraText = "";
			}
			// 도로명 검색
			{
				if( szExtraText != "")
				{
					if (!prop.StreetName.Contains(szExtraText))
						return false;
				}
				
			}*/

			// 도로폭 검색
			if( m_bUseWidth == true)
			{
				if (prop.Width == null)
				{
					return false;
				}

				if(m_bUseWidthDown == true)
				{
					if( prop.Width.Data <  m_fWidthDown)
					{
						return false;
					}

				}
				if( m_bUseWidthUp == true)
				{
					if (prop.Width.Data >= m_fWidthUp)
					{
						return false;
					}
				}
			}

			// 도로길이 검색
			if (m_bUseLength == true)
			{
				if (prop.Length == null)
				{
					return false;
				}

				if (m_bUseLengthDown == true)
				{
					if (prop.Length.Data < m_fLengthDown)
					{
						return false;
					}

				}
				if (m_bUseLengthUp == true)
				{
					if (prop.Length.Data >= m_fLengthUp)
					{
						return false;
					}
				}
			}

			// 공정율 검색
			if (m_bUseRate == true)
			{
				ResultProperty data = FindResultProperty(prop);
				if (m_nProcessRateMethod == 1)
				{
					if (prop.Area == null)
						return false;

					double dResult = 0.0;
					double dSchedule =0.0;

					double rate = 0.0;
					if( data != null)
					{
						dResult = data.TotalArea;
						dSchedule = prop.Area.Data;

						if (dResult < 0.001 && dResult > -0.001)
							rate = 0.0;
						else
							rate = dSchedule / dResult * 100;
					
					}
					else
					{
						rate = 0.0;
					}
					

					if (m_bUseRateDown == true)
					{
						if (rate < m_fRateDown)
							return false;
					}
					if (m_bUseRateUp == true)
					{
						if (rate >= m_fRateUp)
							return false;
					}
				}
				else
				{
					if (prop.Length == null)
						return false;

					double dResult = 0.0;
					double dSchedule = 0.0;

					double rate = 0.0;
					if (data != null)
					{
						dResult = data.TotalLength;
						dSchedule = prop.Length.Data;

						if (dResult < 0.001 && dResult > -0.001)
							rate = 0.0;
						else
							rate = dSchedule / dResult * 100;
					}
					else
					{
						rate = 0.0;
					}

					if (m_bUseRateDown == true)
					{
						if (rate < m_fRateDown)
							return false;
					}
					if (m_bUseRateUp == true)
					{
						if (rate >= m_fRateUp)
							return false;
					}
				}
			}
					

			// 사업비 검색
			if (m_bUseCost == true)
			{
				if (prop.LandCost == null && prop.ObjectCost == null && prop.AroundCost == null)
					return false;

				long nTotalCost = 0;

				if (prop.LandCost != null)
					nTotalCost += prop.LandCost.Data;

				if (prop.ObjectCost != null)
					nTotalCost += prop.ObjectCost.Data;

				if (prop.AroundCost != null)
					nTotalCost += prop.AroundCost.Data;

	 
				if (m_bUseCostDown == true)
				{
					if (nTotalCost < m_nCostDown)
					{
						return false;
					}

				}
				if (m_bUseCostUp == true)
				{
					if (nTotalCost >= m_nCostUp)
					{
						return false;
					}
				}
			}

			// 도로면적 검색
			if (m_bUseArea == true)
			{
				if (prop.Area == null)
				{
					return false;
				}

				if (m_bUseAreaDown == true)
				{
					if (prop.Area.Data < m_fAreaDown)
					{
						return false;
					}

				}
				if (m_bUseAreaUp == true)
				{
					if (prop.Area.Data >= m_fAreaUp)
					{
						return false;
					}
				}
			}

			// 사업기간 검색
			if( m_bUsePeriod == true)
			{
                DateTime dtStart = new DateTime(), dtEnd = new DateTime();
                bool bMin, bMax;

                // 집행계획의 기간을 사용
                if (prop.Schedule.BeginYear == null)
                    bMin = false;
                else
                {
                    bMin = true;
                    dtStart = new DateTime(prop.Schedule.BeginYear.Data, 1, 1);
                }

                if (prop.Schedule.EndYear == null)
                    bMax = false;
                else
                {
                    bMax = true;
                    dtEnd = new DateTime(prop.Schedule.EndYear.Data, 12, 31);
                }
                // 집행진행상황의 기간을 사용
				/*ResultProperty data = FindResultProperty(prop);
				DateTime dtStart;
				DateTime dtEnd;

				if (data == null || data.PropertyDatas.Count == 0)
				{
					return false;
				}

				bool bMin, bMax;
				GetBeginMinDate(data, out dtStart,out bMin, out dtEnd, out bMax);*/


				if (m_bUsePeriod1 == true && m_bUsePeriod2 == false)
				{
					if (bMax == false)
						return false;

					if(dtEnd < m_dtPeriod1)
					{
						return false;
					}
				}

				if (m_bUsePeriod2 == true && m_bUsePeriod1 == false)
				{
					if (bMin == false)
						return false;

					if (dtStart >= m_dtPeriod2)
					{
						return false;
					}
				}

				if (m_bUsePeriod2 == true && m_bUsePeriod1 == true)
				{
					if (bMin == false || bMax == false)
						return false;

					if(dtEnd < m_dtPeriod1 && dtStart < m_dtPeriod1)
					{
						return false;
					}
					if (dtEnd < m_dtPeriod2 && dtStart < m_dtPeriod2)
					{
						return false;
					}
					
				}
			}

			// 지번 검색
			if( m_bUseLotNum == true)
			{
                string strLandAddr = GetLandAddressString();
                bool bFind = false;
                
                foreach (LandAddressData land in prop.LandAddressDatas)
                {
                    if (land.ToString() == strLandAddr)
                    {
                        bFind = true;
                        break;
                    }
                }

                if (bFind == false)
                    return false;
				/*if( m_arLotNumbers.Count > 0)
				{
					bool bFind = false;
					foreach (LandAddressData land in prop.LandAddressDatas)
					{
						if( m_arLotNumbers.Contains(land))
						{
							bFind = true;
						}
					}
					if (bFind == false)
						return false;
				}*/
			}

			return bResult;
		}

        private string GetLandAddressString()
        {
            string strLandAddr = "";

            if (cboDong.Text.Length == 0)
                return strLandAddr;
            else
                strLandAddr = cboDong.Text;

            if (cboRi.Text.Length == 0)
                return strLandAddr;
            else
                strLandAddr += " " + cboRi.Text;

            if (cboLandMajor.Text.Length == 0)
                return strLandAddr;
            else
                strLandAddr += " " + cboLandMajor.Text;

            if (cboLandMinor.Text.Length == 0)
                return strLandAddr;
            else
                strLandAddr += "-" + cboLandMinor.Text;

            return strLandAddr;
        }

		private void GetBeginMinDate(ResultProperty data, out DateTime min, out bool minuse, out DateTime max, out bool maxuse)
		{
			bool bFirst = true;
			max = new DateTime();
			min = new DateTime();
			minuse = false;
			maxuse = false;
			foreach(ResultPropertyData prop in data.PropertyDatas)
			{
				if(bFirst == true)
				{
					if (prop.BeginTime != null)
					{
						min = prop.BeginTime.Data;
						minuse = true;
					}
					if (prop.EndTime != null)
					{
						max = prop.EndTime.Data;
						maxuse = true;
					}
				}
				else
				{
					if (prop.EndTime != null)
					{
						if (max < prop.EndTime.Data)
						{
							max = prop.EndTime.Data;
							maxuse = true;
						}
					}

					if (prop.BeginTime != null)
					{
						if (min > prop.BeginTime.Data)
						{
							min = prop.BeginTime.Data;
							minuse = true;
						}
					}	
				}				
			}
		}
		
		private void btnNameSearch_Click(object sender, EventArgs e)
		{
			PanelDXFViewer panel = FormMain.Instance.CurrentPanel;

			string szText = tbLoadName.Text;
			if (szText == "")
				return;

			FindStreet(szExtraText, szText);

			mGridResult.ClearSelection();

			ShowResultPane();
		}

		private void btnToExcel_Click(object sender, EventArgs e)
		{
			saveFileDialog1.Title = "Excel 내보내기";			
			if(saveFileDialog1.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				string szExcelFileName = saveFileDialog1.FileName;

				bool bCreated = true;
				this.Enabled = false;
				Cursor temp = Cursor.Current;
				Cursor.Current = Cursors.WaitCursor;
				try
				{

					Excel.Application xlApp;
					Excel.Workbook xlWorkBook;
					Excel.Worksheet xlWorkSheet;
					object misValue = System.Reflection.Missing.Value;

					xlApp = new Excel.Application();
					xlApp.DisplayAlerts = false;
					xlWorkBook = xlApp.Workbooks.Add(misValue);
					xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
					xlWorkSheet.Name = "도로검색결과";
					int i = 0;
					int j = 0;

					Excel.Style style = xlWorkBook.Styles.Add("LoadResult");
					style.Font.Name = mGridResult.Font.Name;
					style.Font.Size = 9;
					style.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
					
					for (j = 0; j <= mGridResult.ColumnCount - 1; j++)
					{
						DataGridViewColumn colume = mGridResult.Columns[j];

						xlWorkSheet.Cells[1, j + 1] = colume.HeaderText;
						xlWorkSheet.Cells[1, j + 1].EntireColumn.AutoFit();
						xlWorkSheet.Cells[1, j + 1].Style = style;
						xlWorkSheet.Cells[1, j + 1].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

						xlWorkSheet.Cells[1, j + 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
						xlWorkSheet.Cells[1, j + 1].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Gray);
						xlWorkSheet.Cells[1, j + 1].Interior.Pattern = Excel.XlPattern.xlPatternSolid;						
						xlWorkSheet.Cells[1, j + 1].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;					
					}

					for (i = 0; i <= mGridResult.RowCount - 1; i++)
					{
						for (j = 0; j <= mGridResult.ColumnCount - 1; j++)
						{
							DataGridViewColumn colume = mGridResult.Columns[j];
							DataGridViewCell cell = mGridResult[j, i];

							bool bNumberCell = false;
							if (colume.HeaderText.Contains("구간"))
								bNumberCell = true;

							xlWorkSheet.Cells[i + 2, j + 1] = cell.Value;
							xlWorkSheet.Cells[i + 2, j + 1].EntireColumn.AutoFit();
							xlWorkSheet.Cells[i + 2, j + 1].Style = style;
							xlWorkSheet.Cells[i + 2, j + 1].VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

							if (bNumberCell == true)
							{
								xlWorkSheet.Cells[i + 2, j + 1].NumberFormat = "#,##0";
							}
							xlWorkSheet.Cells[i + 2, j + 1].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
							//xlWorkSheet.Cells[i + 1, j + 1].bordersRange.Interior.Color = 0xFF00;

							if (colume.DefaultCellStyle.Alignment == DataGridViewContentAlignment.MiddleCenter)
							{
								xlWorkSheet.Cells[i + 2, j + 1].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
							}
							else if (colume.DefaultCellStyle.Alignment == DataGridViewContentAlignment.MiddleRight)
							{
								xlWorkSheet.Cells[i + 2, j + 1].HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
							}
							else if (colume.DefaultCellStyle.Alignment == DataGridViewContentAlignment.NotSet)
							{
								xlWorkSheet.Cells[i + 2, j + 1].HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
							}
						}
					}
					xlWorkSheet.PageSetup.PrintTitleRows = "$1:$1";
					
					releaseObject(xlWorkSheet);

					xlWorkBook.SaveAs(szExcelFileName, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
					xlWorkBook.Close(true, misValue, misValue);
					releaseObject(xlWorkBook);

					xlApp.Workbooks.Close();
					releaseObject(xlApp.Workbooks);
					xlApp.Quit();
					releaseObject(xlApp);

				}
				catch (Exception ex)
				{
					bCreated = false;
					UnE.Utility.UMessageBox.Show(this, "Excel문서로 출력중 오류가 발생하였습니다.\r\n오류내용 : " + ex.Message, "Excel 내보내기", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}

				this.Enabled = true;
				Cursor.Current = temp;

				if (bCreated == true)
				{
                    UnE.Utility.UMessageBox.Show(this, "Excel문서로 출력이 완료 되었습니다.\r\n" + szExcelFileName, "Excel 내보내기", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}

			
		}

		private void releaseObject(object obj)
		{
			try
			{
				Marshal.FinalReleaseComObject(obj);
				obj = null;
			}
			catch (Exception ex)
			{
				obj = null;				
			}
			finally
			{
				GC.Collect();
				GC.WaitForPendingFinalizers();

			}
		}


		private void btnOption_Click(object sender, EventArgs e)
		{
			ShowOptionPane();
		}

		private void btnAllClear_Click(object sender, EventArgs e)
		{
			tbWidth1.Clear();
			tbWidth2.Clear();
		
			tbRate1.Clear();
			tbRate2.Clear();
		
			tbLength1.Clear();
			tbLength2.Clear();
		
			tbCost1.Clear();
			tbCost2.Clear();
		
			tbArea1.Clear();
			tbArea2.Clear();
		
			tbLotNum.Clear();
		
			dtPeriod1.Value = DateTime.Now;
			dtPeriod2.Value = DateTime.Now;

			tbLoadName.Clear();
		}
		
		public void ShowResultPane()
		{
			mLBColumn.Visible = true;
			mCkbColumnWidth.Visible = true;
			mCkbColumnProcessRate.Visible = true;
			mCkbColumnPriod.Visible = true;
			mCkbColumnLength.Visible = true;
			mCkbColumnCost.Visible = true;
			mCkbColumnArea.Visible = true;

			mOptionPane.Visible = false;
			mResultPane.Dock = DockStyle.Fill;
			mResultPane.Visible = true;

			btnOption.Visible = true;
			btnAllClear.Visible = false;
			btnToExcel.Visible = true;
			btnPrint.Visible = true;

            if (mGridResult.SelectedRows.Count > 0)
                SelectRow(mGridResult.SelectedRows[0]);
            else if (mCkbLot.Checked)
                SelectLandAddress(FormMain.Instance.CurrentPanel);
		}

		public void ShowOptionPane()
		{
			mLBColumn.Visible = false;
			mCkbColumnWidth.Visible = false;
			mCkbColumnProcessRate.Visible = false;
			mCkbColumnPriod.Visible = false;
			mCkbColumnLength.Visible = false;
			mCkbColumnCost.Visible = false;
			mCkbColumnArea.Visible = false;

			mOptionPane.Visible = true;
			mResultPane.Dock = DockStyle.Fill;
			mResultPane.Visible = false;

			btnOption.Visible = false;
			btnAllClear.Visible = true;
			btnToExcel.Visible = false;
			btnPrint.Visible = false;

            ClearSelection();

            MakeAutoCompleteData();
            MakeLandAddressData();

            FormMain.Instance.CurrentDXFControl.Refresh();
		}		

		private string ToStringLotNumbers()
		{
			if (m_arLotNumbers == null)
				return "";

			string szText = "";

			foreach (LandAddressData lot in m_arLotNumbers)
			{
				szText += lot.ToString();
				szText += ",";
			}
			return szText;
		}

		private void btnFindLotNumber_Click(object sender, EventArgs e)
		{
			FormLotNumberSearch lotSearch = new FormLotNumberSearch();
			lotSearch.SetData((ArrayList)m_arLotNumbers.Clone());
			DialogFormFrame frame = new DialogFormFrame(lotSearch);
			frame.StartPosition = FormStartPosition.CenterParent;
			if(frame.ShowDialog(this) == DialogResult.OK)
			{
				m_arLotNumbers = lotSearch.GetData();
			}
			else
			{

			}

			tbLotNum.Text = ToStringLotNumbers();
		}
		
		private void btnClearWidth_Click(object sender, EventArgs e)
		{
			tbWidth1.Clear();
			tbWidth2.Clear();
		}

		private void btnClearRate_Click(object sender, EventArgs e)
		{
			tbRate1.Clear();
			tbRate2.Clear();
		}

		private void btnClearLength_Click(object sender, EventArgs e)
		{
			tbLength1.Clear();
			tbLength2.Clear();
		}

		private void btnClearCost_Click(object sender, EventArgs e)
		{
			tbCost1.Clear();
			tbCost2.Clear();
		}

		private void btnClearArea_Click(object sender, EventArgs e)
		{
			tbArea1.Clear();
			tbArea2.Clear();
		}

		private void btnClearLot_Click(object sender, EventArgs e)
		{
			tbLotNum.Clear();
			m_arLotNumbers.Clear();
		}

		private void btnClearPriod_Click(object sender, EventArgs e)
		{
			dtPeriod1.Value = DateTime.Now;
			dtPeriod2.Value = DateTime.Now;

			dtPeriod1.Checked = false;
			dtPeriod2.Checked = false;

			m_bUsePeriod1 = false;
			m_bUsePeriod1 = false;
		}
		
		private void EnableWidth(bool bEnabled)
		{
			m_bUseWidth = bEnabled;

			tbWidth1.Enabled = bEnabled;
			tbWidth2.Enabled = bEnabled;

			btnClearWidth.Enabled = bEnabled;
		}

		private void EnableProcessRate(bool bEnabled)
		{
			m_bUseRate = bEnabled;

			tbRate1.Enabled = bEnabled;
			tbRate2.Enabled = bEnabled;

			rdRateArea.Enabled = bEnabled;
			rdRateLength.Enabled = bEnabled;

			btnClearRate.Enabled = bEnabled;
		}

		private void rdRateLength_CheckedChanged(object sender, EventArgs e)
		{
			if (rdRateLength.Checked == true)
			{
				m_nProcessRateMethod = 2;
			}
		}

		private void rdRateArea_CheckedChanged(object sender, EventArgs e)
		{
			if (rdRateArea.Checked == true)
			{
				m_nProcessRateMethod = 1;
			}
		}
		
		private void EnableLength(bool bEnabled)
		{
			m_bUseLength = bEnabled;

			tbLength1.Enabled = bEnabled;
			tbLength2.Enabled = bEnabled;

			btnClearLength.Enabled = bEnabled;
		}

		private void EnableCost(bool bEnabled)
		{
			m_bUseCost = bEnabled;

			tbCost1.Enabled = bEnabled;
			tbCost2.Enabled = bEnabled;

			btnClearCost.Enabled = bEnabled;
		}

		private void EnableArea(bool bEnabled)
		{
			m_bUseArea = bEnabled;

			tbArea1.Enabled = bEnabled;
			tbArea2.Enabled = bEnabled;

			btnClearArea.Enabled = bEnabled;
		} 

		private void EnablePeriod(bool bEnabled)
		{
			m_bUsePeriod = bEnabled;

			dtPeriod1.Enabled = bEnabled;
			dtPeriod2.Enabled = bEnabled;

			btnClearPriod.Enabled = bEnabled;		
		}

        private void EnableStreetName(bool bEnabled)
        {
            cboStreetName.Enabled = cboStreetMajor.Enabled = cboStreetMinor.Enabled = bEnabled;
        }

		private void EnableLotNum(bool bEnabled)
		{
			m_bUseLotNum = bEnabled;

			tbLotNum.Enabled = bEnabled;

			btnFindLotNumber.Enabled = bEnabled;
			btnClearLot.Enabled = bEnabled;

            cboDong.Enabled = cboRi.Enabled = cboLandMajor.Enabled = cboLandMinor.Enabled = bEnabled;
		}

		private void mCkbWidth_CheckedChanged(object sender, EventArgs e)
		{
			EnableWidth(mCkbWidth.Checked);	
		}

		private void mCkbRate_CheckedChanged(object sender, EventArgs e)
		{
			EnableProcessRate(mCkbRate.Checked);
		}

		private void mCkbLength_CheckedChanged(object sender, EventArgs e)
		{
			EnableLength(mCkbLength.Checked);
		}

		private void mCkbCost_CheckedChanged(object sender, EventArgs e)
		{
			EnableCost(mCkbCost.Checked);
		}

		private void mCkbArea_CheckedChanged(object sender, EventArgs e)
		{
			EnableArea(mCkbArea.Checked);
		}

		private void mCkbPeriod_CheckedChanged(object sender, EventArgs e)
		{
			EnablePeriod(mCkbPeriod.Checked);
		}
		
		private void mCkbLot_CheckedChanged(object sender, EventArgs e)
		{
			EnableLotNum(mCkbLot.Checked);
		}

		private void mCkbColumnWidth_CheckedChanged(object sender, EventArgs e)
		{			
			colWidth.Visible = mCkbColumnWidth.Checked;			
		}

		private void mCkbColumnProcessRate_CheckedChanged(object sender, EventArgs e)
		{
			colRate.Visible = mCkbColumnProcessRate.Checked;
		}

		private void mCkbColumnLength_CheckedChanged(object sender, EventArgs e)
		{
			colLength.Visible = mCkbColumnLength.Checked;
		}

		private void mCkbColumnArea_CheckedChanged(object sender, EventArgs e)
		{
			colArea.Visible = mCkbColumnArea.Checked;
		}

		private void mCkbColumnCost_CheckedChanged(object sender, EventArgs e)
		{
			colCost.Visible = mCkbColumnCost.Checked;
		}

		private void mCkbColumnPriod_CheckedChanged(object sender, EventArgs e)
		{
			colPeriod.Visible = mCkbColumnPriod.Checked;
		}

		private void FormSearch_VisibleChanged(object sender, EventArgs e)
		{
			if( this.Visible == false)
			{
				FormMain.Instance.EndLoadSearch();
			}
		}


		private bool CheckNumberText(string szText, string szName)
		{			
			if( szText.IndexOf(" ") != -1)
			{
				UnE.Utility.UMessageBox.Show(this,szName + "(은)는 공백이 포함될 수 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			
			Regex r = new Regex("^[0-9.]*$");
			if (!r.IsMatch(szText))
			{
				UnE.Utility.UMessageBox.Show(this,szName + "(은)는 숫자만 입력됩니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			return true;		
		}

		public void SetTextBox(TextBox tbEdit, ref string szOrgText, string szNewText, string szName)
		{
			if(!CheckNumberText(szNewText, szName))
			{
				tbEdit.Text = szOrgText;
			}
			else
			{
				szOrgText = szNewText;
			}
		}
		

		private void tbWidth1_TextChanged(object sender, EventArgs e)
		{
			string szText = tbWidth1.Text;
			string szName = "도로폭";

			this.BeginInvoke(new Action(() => SetTextBox(tbWidth1, ref m_szWidth1, szText, szName)));
		}

		private void tbWidth2_TextChanged(object sender, EventArgs e)
		{
			string szText = tbWidth2.Text;
			string szName = "도로폭";
			this.BeginInvoke(new Action(() => SetTextBox(tbWidth2, ref m_szWidth2, szText, szName)));
		}
				
		private void tbRate1_TextChanged(object sender, EventArgs e)
		{
			string szText = tbRate1.Text;
			string szName = "공정율";
			this.BeginInvoke(new Action(() => SetTextBox(tbRate1, ref m_szRate1, szText, szName)));
		}

		private void tbRate2_TextChanged(object sender, EventArgs e)
		{
			string szText = tbRate2.Text;
			string szName = "공정율";
			this.BeginInvoke(new Action(() => SetTextBox(tbRate2, ref m_szRate2, szText, szName)));
		}
				
		private void tbLength1_TextChanged(object sender, EventArgs e)
		{
			string szText = tbLength1.Text;
			string szName = "구간길이";
			this.BeginInvoke(new Action(() => SetTextBox(tbLength1, ref m_szLength1, szText, szName)));
		}

		private void tbLength2_TextChanged(object sender, EventArgs e)
		{
			string szText = tbLength2.Text;
			string szName = "구간길이";
			this.BeginInvoke(new Action(() => SetTextBox(tbLength2, ref m_szLength2, szText, szName)));
		}

		private void tbCost1_TextChanged(object sender, EventArgs e)
		{
			string szText = tbCost1.Text;
			string szName = "사업비용";
			this.BeginInvoke(new Action(() => SetTextBox(tbCost1, ref m_szCost1, szText, szName)));
		}

		private void tbCost2_TextChanged(object sender, EventArgs e)
		{
			string szText = tbCost2.Text;
			string szName = "사업비용";
			this.BeginInvoke(new Action(() => SetTextBox(tbCost2, ref m_szCost2, szText, szName)));
		}
		
		private void tbArea1_TextChanged(object sender, EventArgs e)
		{
			string szText = tbArea1.Text;
			string szName = "구간면적";
			this.BeginInvoke(new Action(() => SetTextBox(tbArea1, ref m_szArea1, szText, szName)));
		}

		private void tbArea2_TextChanged(object sender, EventArgs e)
		{
			string szText = tbArea2.Text;
			string szName = "구간면적";
			this.BeginInvoke(new Action(() => SetTextBox(tbArea1, ref m_szArea2, szText, szName)));
		}
				
		private void dtPeriod1_ValueChanged(object sender, EventArgs e)
		{
			if(dtPeriod1.Checked == true)
			{
				DateTime dtValue = dtPeriod1.Value;
				m_dtPeriod1 = dtValue;
				m_bUsePeriod1 = true;
			}
			else
			{
				m_bUsePeriod1 = false;
			}		
		}

		private void dtPeriod2_ValueChanged(object sender, EventArgs e)
		{
			if (dtPeriod2.Checked == true)
			{
				DateTime dtValue = dtPeriod2.Value;
				m_dtPeriod2 = dtValue;
				m_bUsePeriod2 = true;
			}
			else
			{
				m_bUsePeriod2 = false;
			}	
		}

		private bool m_bShowHeader = false;
		public bool PrintShowHeader
		{
			get { return m_bShowHeader; }
			set { m_bShowHeader = value; }
		}
		private bool m_bShowDate = false;
		public bool PrintShowDate
		{
			get { return m_bShowDate; }
			set { m_bShowDate = value; }
		}
		private string m_szHeaderText = "";
		public string PrintHeaderText
		{
			get { return m_szHeaderText; }
			set { m_szHeaderText = value; }
		}

		

		private DialogFormFrame frame = null;
		private FormGridPrintPageSetup formSetup = null;		
		private void btnPrint_Click(object sender, EventArgs e)
		{
			formSetup = new FormGridPrintPageSetup();
			formSetup.Text = "인쇄 설정";
			formSetup.PrinterSettings = new System.Drawing.Printing.PrinterSettings();
			formSetup.Document = mPrintDocument;
			formSetup.Document.DefaultPageSettings.Landscape = true;
			formSetup.ShowHeader = m_bShowHeader;
			formSetup.ShowDate = m_bShowDate;
			formSetup.HeaderText = m_szHeaderText;
			
			frame = new DialogFormFrame(formSetup);
			frame.Text = "인쇄 설정";
			frame.StartPosition = FormStartPosition.CenterParent;
			if (frame.ShowDialog(this) == DialogResult.OK)
			{
				mPrintDocument.Print();
			}

			m_bShowHeader = formSetup.ShowHeader;
			m_bShowDate = formSetup.ShowDate;
			m_szHeaderText = formSetup.HeaderText;
		}

		// 인쇄에 필요한 정보
		private StringFormat mFormatNormal;
		private StringFormat mFormatMiddleCenter;
		private StringFormat mFormatRightCenter;

		private ArrayList mArrrColumnLefts = new ArrayList();
		private ArrayList mArColumnWidths = new ArrayList();

		private int m_nCellHeight = 0;
		private int m_nTotalWidth = 0;
		private int m_nRow = 0;
		private bool m_bFirstPage = false;
		private bool m_bNewPage = false;
		private int m_nHeaderHeight = 0; 
				
		private void printDocument1_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
		{
			try
			{
				mFormatNormal = new StringFormat();
				mFormatNormal.Alignment = StringAlignment.Near;
				mFormatNormal.LineAlignment = StringAlignment.Center;
				mFormatNormal.Trimming = StringTrimming.EllipsisCharacter;

				mFormatMiddleCenter = new StringFormat();
				mFormatMiddleCenter.Alignment = StringAlignment.Center;
				mFormatMiddleCenter.LineAlignment = StringAlignment.Center;
				mFormatMiddleCenter.Trimming = StringTrimming.EllipsisCharacter;

				mFormatRightCenter = new StringFormat();
				mFormatRightCenter.Alignment = StringAlignment.Far;
				mFormatRightCenter.LineAlignment = StringAlignment.Center;
				mFormatRightCenter.Trimming = StringTrimming.EllipsisCharacter;

				mArrrColumnLefts.Clear();
				mArColumnWidths.Clear();
				m_nCellHeight = 0;
				m_nRow = 0;
				m_bFirstPage = true;
				m_bNewPage = true;


				m_nTotalWidth = 0;
				foreach (DataGridViewColumn dgvGridCol in mGridResult.Columns)
				{
					m_nTotalWidth += dgvGridCol.Width;
				}
			}
			catch (Exception ex)
			{
				UnE.Utility.UMessageBox.Show(this, "출력오류가 발생하였습니다.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
		{
			try
			{
				// 인쇄 미리 보기 에서 현재 프린트 설정을 임시로 가져온다.
				if (formSetup != null && !formSetup.IsDisposed)
				{
					if (formSetup.Visible == true)
					{
						m_bShowHeader = formSetup.ShowHeader;
						m_bShowDate = formSetup.ShowDate;
						m_szHeaderText = formSetup.HeaderText;
					}
				}


				// 왼쪽 Margin 지정
				int nLeftMargin = e.MarginBounds.Left;
				// 상단 Margin 지정
				int nTopMargin = e.MarginBounds.Top;
				
				// 추가 페이지가 있는지 여부를 나타내는 변수
				bool bMorePagesToPrint = false;
				int nTmpWidth = 0;

				// 첫번째 페이지의 Width와 Height을 계산
				if (m_bFirstPage)
				{
					foreach (DataGridViewColumn GridCol in mGridResult.Columns)
					{
						nTmpWidth = (int)(Math.Floor((double)((double)GridCol.Width /
									   (double)m_nTotalWidth * (double)m_nTotalWidth *
									   ((double)e.MarginBounds.Width / (double)m_nTotalWidth))));

						m_nHeaderHeight = (int)(e.Graphics.MeasureString(GridCol.HeaderText,
									GridCol.InheritedStyle.Font, nTmpWidth).Height) + 11;

						// Save width and height of headres
						mArrrColumnLefts.Add(nLeftMargin);
						mArColumnWidths.Add(nTmpWidth);
						nLeftMargin += nTmpWidth;
					}
				}

				// Grid의 모든 행을 출력할 때까지 Loop를 돌면서 출력한다.
				while (m_nRow <= mGridResult.Rows.Count - 1)
				{
					DataGridViewRow GridRow = mGridResult.Rows[m_nRow];
					
					// Cell의 높이 설정
					m_nCellHeight = GridRow.Height + 5;
					int nCount = 0;

					// 전체 페이지의 Bound를 넘기는지 검사하여 새Page처리
					if (nTopMargin + m_nCellHeight >= e.MarginBounds.Height + e.MarginBounds.Top)
					{
						m_bNewPage = true;
						m_bFirstPage = false;
						bMorePagesToPrint = true;
						break;
					}
					else
					{
						if (m_bNewPage)
						{
							

							if( m_bShowHeader == true)
							{
								// 헤더 Text 출력
								e.Graphics.DrawString(m_szHeaderText, new Font(mGridResult.Font, FontStyle.Bold),
										Brushes.Black, e.MarginBounds.Left, e.MarginBounds.Top -
										e.Graphics.MeasureString(m_szHeaderText, new Font(mGridResult.Font,
										FontStyle.Bold), e.MarginBounds.Width).Height - 13);
							}
							

							if( m_bShowDate == true)
							{
								String strDate = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToShortTimeString();

								// 날짜 출력
								e.Graphics.DrawString(strDate, new Font(mGridResult.Font, FontStyle.Bold),
										Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width -
										e.Graphics.MeasureString(strDate, new Font(mGridResult.Font,
										FontStyle.Bold), e.MarginBounds.Width).Width), e.MarginBounds.Top -
										e.Graphics.MeasureString(m_szHeaderText, new Font(new Font(mGridResult.Font,
										FontStyle.Bold), FontStyle.Bold), e.MarginBounds.Width).Height - 13);
							}

							

							// 컬럼 헤더 출력              
							nTopMargin = e.MarginBounds.Top;
							foreach (DataGridViewColumn GridCol in mGridResult.Columns)
							{
								e.Graphics.FillRectangle(new SolidBrush(Color.LightGray),
									new Rectangle((int)mArrrColumnLefts[nCount], nTopMargin,
									(int)mArColumnWidths[nCount], m_nHeaderHeight));

								e.Graphics.DrawRectangle(Pens.Black,
									new Rectangle((int)mArrrColumnLefts[nCount], nTopMargin,
									(int)mArColumnWidths[nCount], m_nHeaderHeight));

								StringFormat format = mFormatNormal;

								if (mGridResult.ColumnHeadersDefaultCellStyle.Alignment == DataGridViewContentAlignment.MiddleCenter)
								{
									format = mFormatMiddleCenter;
								}
								if (mGridResult.ColumnHeadersDefaultCellStyle.Alignment == DataGridViewContentAlignment.MiddleRight)
								{
									format = mFormatRightCenter;
								}

								e.Graphics.DrawString(GridCol.HeaderText, GridCol.InheritedStyle.Font,
									new SolidBrush(GridCol.InheritedStyle.ForeColor),
									new RectangleF((int)mArrrColumnLefts[nCount], nTopMargin,
									(int)mArColumnWidths[nCount], m_nHeaderHeight), format);
								nCount++;
							}
							m_bNewPage = false;
							nTopMargin += m_nHeaderHeight;
						}
						nCount = 0;
						
						// 각 Cell의 내용을 출력               
						foreach (DataGridViewCell Cel in GridRow.Cells)
						{
							if (Cel.Value != null)
							{
								int nColumIdx = Cel.ColumnIndex;
								DataGridViewColumn GridCol = mGridResult.Columns[nColumIdx];
								StringFormat format = mFormatNormal;
								if (GridCol.DefaultCellStyle.Alignment == DataGridViewContentAlignment.MiddleCenter)
								{
									format = mFormatMiddleCenter;
								}
								if (GridCol.DefaultCellStyle.Alignment == DataGridViewContentAlignment.MiddleRight)
								{
									format = mFormatRightCenter;
								}

								e.Graphics.DrawString(Cel.Value.ToString(), Cel.InheritedStyle.Font,
											new SolidBrush(Cel.InheritedStyle.ForeColor),
											new RectangleF((int)mArrrColumnLefts[nCount], (float)nTopMargin,
											(int)mArColumnWidths[nCount], (float)m_nCellHeight), format);
							}

							
							// Cell의 테투리를 출력 
							e.Graphics.DrawRectangle(Pens.Black, new Rectangle((int)mArrrColumnLefts[nCount],
									nTopMargin, (int)mArColumnWidths[nCount], m_nCellHeight));

							nCount++;
						}
					}
					m_nRow++;
					nTopMargin += m_nCellHeight;
				}

				// 새 페이지가 필요한 경우
				if (bMorePagesToPrint)
					e.HasMorePages = true;
				else
					e.HasMorePages = false;
			}
			catch (Exception)
			{
				UnE.Utility.UMessageBox.Show(this, "출력오류가 발생하였습니다.", "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

        private void mGridResult_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
                FormMain.Instance.ShowHelp();
        }

        private void mGridResult_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = mGridResult.Rows[e.RowIndex];

            if (row.Tag != null && row.Tag is ScheduleProperty)
            {
                ScheduleProperty prop = (ScheduleProperty)row.Tag;
                PanelDXFViewer panel = FormMain.Instance.CurrentPanel;

                if (panel.ScheduleDetailForm != null)
                {
                    // 이미 같은 창이 떠있으면 다시 띄우지 않는다.
                    if (panel.ScheduleDetailForm.ScheduleProperty != prop)
                    {
                        panel.ScheduleDetailForm.Close();
                        DataManager.ShowScheduleDetail(prop, panel);
                    }
                }
                else
                    DataManager.ShowScheduleDetail(prop, panel);
            }
        }

        private void cboStreetName_TextChanged(object sender, EventArgs e)
        {
            cboStreetMajor.Items.Clear();
            string strName = cboStreetName.Text.Trim();

            if (strName.Length > 0)
            {
                List<AddrString> majors = new List<AddrString>();

                if (m_dicStreetMajors.TryGetValue(strName, out majors))
                {
                    foreach (AddrString strMajor in majors)
                    {
                        cboStreetMajor.Items.Add(strMajor.Data);
                    }
                }
            }

            string major = cboStreetMajor.Text.Trim();

            if (!cboStreetMajor.Items.Contains(major))
                cboStreetMajor.Text = "";

            cboStreetMajor_TextChanged(null, null);
        }

        private void cboStreetMajor_TextChanged(object sender, EventArgs e)
        {
            cboStreetMinor.Items.Clear();

            string strName = cboStreetName.Text.Trim();
            string strMajor = cboStreetMajor.Text.Trim();
            string strTag = strName + strMajor;

            if (strTag.Length > 0)
            {
                List<AddrString> minors = new List<AddrString>();

                if (m_dicStreetMinors.TryGetValue(strTag, out minors))
                {
                    foreach (AddrString strMinor in minors)
                    {
                        cboStreetMinor.Items.Add(strMinor.Data);
                    }
                }
            }

            string minor = cboStreetMinor.Text.Trim();

            if (!cboStreetMinor.Items.Contains(minor))
                cboStreetMinor.Text = "";
        }

        private void cboDong_TextChanged(object sender, EventArgs e)
        {
            cboRi.Items.Clear();
            string strDong = cboDong.Text.Trim();

            if (strDong.Length == 0)
            {
                cboRi.Text = "";
                return;
            }

            List<AddrString> ris = new List<AddrString>();

            if (!m_dicLandAddrTowns.TryGetValue(strDong, out ris))
            {
                cboRi.Text = "";
                return;
            }

            foreach (AddrString strRi in ris)
            {
                cboRi.Items.Add(strRi.Data);
            }

            if (!cboRi.Items.Contains(cboRi.Text))
                cboRi.Text = "";

            cboRi_TextChanged(null, null);
        }

        private void cboRi_TextChanged(object sender, EventArgs e)
        {
            cboLandMajor.Items.Clear();
            string strTownName = cboDong.Text + " " + cboRi.Text;

            if (strTownName.Length == 0)
            {
                cboLandMajor.Text = "";
                return;
            }

            List<AddrString> majors = new List<AddrString>();

            if (!m_dicLandAddrMajors.TryGetValue(strTownName, out majors))
            {
                cboLandMajor.Text = "";
                return;
            }

            foreach (AddrString strMajor in majors)
            {
                cboLandMajor.Items.Add(strMajor.Data);
            }

            if (!cboLandMajor.Items.Contains(cboLandMajor.Text))
                cboLandMajor.Text = "";

            cboLandMajor_TextChanged(null, null);
        }

        private void cboLandMajor_TextChanged(object sender, EventArgs e)
        {
            cboLandMinor.Items.Clear();
            string strTownMajor = cboDong.Text + " " + cboRi.Text + " " + cboLandMajor.Text;

            if (strTownMajor.Length == 0)
            {
                cboLandMinor.Text = "";
                return;
            }

            List<AddrString> minors = new List<AddrString>();

            if (!m_dicLandAddrMinors.TryGetValue(strTownMajor, out minors))
            {
                cboLandMinor.Text = "";
                return;
            }

            foreach (AddrString strMinor in minors)
            {
                cboLandMinor.Items.Add(strMinor.Data);
            }

            if (!cboLandMinor.Items.Contains(cboLandMinor.Text))
                cboLandMinor.Text = "";
        }

        private void checkStreetName_CheckedChanged(object sender, EventArgs e)
        {
            EnableStreetName(checkStreetName.Checked);
        }

        private void mGridResult_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = mGridResult.Rows[e.RowIndex];
            SelectRow(row);
        }

        private int SelectLandAddress(PanelDXFViewer panel)
        {
            string strLandAddr = GetLandAddressString();

            LandAddressData2 data;
            DXFViewer.Layer layer = ((DXFExternPainter)panel.DXFControl.ExternalPainter).GetLayer(DXFExternPainter.LayerType.LAND_ADDRESS);

            if (layer != null && panel.DataManager.LandAddressDatas.TryGetValue(strLandAddr, out data))
            {
                List<DXFViewer.Shape> shapes = new List<DXFViewer.Shape>();

                foreach (DXFViewer.Hatch hatch in data.Hatchs)
                {
                    layer.Add(hatch);
                    shapes.Add(hatch);
                }

                if (Options.Instance.ZoomOnSelectStreet == true)
                {
                    panel.DataManager.ObjectZoom(shapes, panel, true);
                }

                return shapes.Count;
            }

            return 0;
        }

        private void SelectRow(DataGridViewRow row)
        {
            if (row == null)
                return;

            ClearSelection();

            PanelDXFViewer panel = FormMain.Instance.CurrentPanel;
            bool hatchZoom = false;

            if (mCkbLot.Checked)
            {
                hatchZoom = SelectLandAddress(panel) > 0 ? true : false;
            }

            if (row.Tag != null && row.Tag is ScheduleProperty)
            {
                ScheduleProperty prop = (ScheduleProperty)row.Tag;

                List<DXFViewer.Shape> shapes;

                if (panel.DataManager.StreetShapes.TryGetValue(prop.StreetName, out shapes))
                {
                    if (Options.Instance.ZoomOnSelectStreet == true)
                    {
                        // Hatch가 Zoom 되었으면 도로는 그냥 선택만 한다.
                        if (hatchZoom == false)
                            panel.DataManager.ObjectZoom(prop.StreetName, panel);
                        else
                            FormSettingStreetName.SelectShapes(panel, shapes, true, false);
                    }
                    else
                    {
                        FormSettingStreetName.SelectShapes(panel, shapes, true, false);
                    }
                }
            }

            panel.DXFControl.Refresh();
        }
	}
}
