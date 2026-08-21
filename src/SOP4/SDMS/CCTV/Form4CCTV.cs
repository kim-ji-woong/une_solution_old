using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UnE.Spatial;
using UnE.Sensor;
using UnE.View.Content;

namespace SDMS
{

	public partial class Form4CCTV : Form
	{
		private Zone NULL_ZONE = new Zone();
		private Dictionary<Zone, CCTVList> m_dicZoneCCTVs = new Dictionary<Zone, CCTVList>();

		public enum CCTV_POSITION { TL = 0, TM = 1, BM = 4, BL = 3, BR = 5, TR = 2, UNKNOWN = 6 }

		private BigCCTVCtrl[] m_arrCCTV = new BigCCTVCtrl[6] { null, null, null, null, null, null };
        
        private Control m_ctrlParent = null;
        public Control ParentCtrl
        {
            get { return m_ctrlParent; }
            set { m_ctrlParent = value; }
        }

		private int m_nLineThick = 5;

		private CCTVList m_currentCCTVList = null;

        private string m_szDefaultCCTVFileName = "";

        private ICCTVFormOwner m_FormOwner = null;
        public ICCTVFormOwner GetOwner()
        {
            return m_FormOwner;
        }

        public void SetOwner(ICCTVFormOwner owner)
        {
            m_FormOwner = owner;
        }


		public Form4CCTV(Control ctrlParent, string szDefFileName)
		{
            m_szDefaultCCTVFileName = szDefFileName;

			InitializeComponent();

            InitPanel();

			m_arrCCTV[0] = BigCCTVCtrl.MakeInstance(null, this);
			m_arrCCTV[1] = BigCCTVCtrl.MakeInstance(null, this);
			m_arrCCTV[2] = BigCCTVCtrl.MakeInstance(null, this);
			m_arrCCTV[3] = BigCCTVCtrl.MakeInstance(null, this);
            m_arrCCTV[4] = BigCCTVCtrl.MakeInstance(null, this);
            m_arrCCTV[5] = BigCCTVCtrl.MakeInstance(null, this);

            SetPanel((int)CCTV_POSITION.TM, m_arrCCTV[(int)CCTV_POSITION.TM]);
            SetPanel((int)CCTV_POSITION.TR, m_arrCCTV[(int)CCTV_POSITION.TR]);
            SetPanel((int)CCTV_POSITION.BM, m_arrCCTV[(int)CCTV_POSITION.BM]);
            SetPanel((int)CCTV_POSITION.BR, m_arrCCTV[(int)CCTV_POSITION.BR]);

            SetPanel((int)CCTV_POSITION.TL, m_arrCCTV[(int)CCTV_POSITION.TL]);
            SetPanel((int)CCTV_POSITION.BL, m_arrCCTV[(int)CCTV_POSITION.BL]);

            //ReadDefaultCCTV();
			m_ctrlParent = ctrlParent;            
            ResizeForm(ctrlParent);
		}

        public void SetDefaultCCTV()
        {
            ReadDefaultCCTV();
        }

		private void ReadDefaultCCTV()
		{
			string strPath = GetDefaultCCTVLogFilePath();
            CCTVList list = new CCTVList();
            if (System.IO.File.Exists(strPath))
            {
                System.IO.StreamReader reader = new System.IO.StreamReader(strPath, Encoding.UTF8);
                int nID;
                
                for (int i = 0; i < 6 && !reader.EndOfStream; i++)
                {
                    string strLine = reader.ReadLine();

                    if (int.TryParse(strLine, out nID))
                    {
                        if (i == (int)CCTV_POSITION.TL)
                            list.TL = CCTVManager.Instance.GetCCTV(nID);
                        else if (i == (int)CCTV_POSITION.BL)
                            list.BL = CCTVManager.Instance.GetCCTV(nID);
                        else if (i == (int)CCTV_POSITION.TM)
                            list.TM = CCTVManager.Instance.GetCCTV(nID);
                        else if (i == (int)CCTV_POSITION.BM)
                            list.BM = CCTVManager.Instance.GetCCTV(nID);
                        else if (i == (int)CCTV_POSITION.BR)
                            list.BR = CCTVManager.Instance.GetCCTV(nID);
                        else if (i == (int)CCTV_POSITION.TR)
                            list.TR = CCTVManager.Instance.GetCCTV(nID);
                    }
                }
                reader.Close();
            } 
            else
            {               
                list.TM = null;
                list.BM = null;
                list.BR = null;
                list.TR = null;
                list.TL = null;
                list.BL = null;
            }

        	SetCCTVList(NULL_ZONE, list);            
		}

		private void WriteDefaultCCTV()
		{
			CCTVList list = GetCCTVList(NULL_ZONE);

			if (list == null)
				return;

			string strPath = GetDefaultCCTVLogFilePath();
			System.IO.StreamWriter writer = new System.IO.StreamWriter(strPath, false, Encoding.UTF8);

			for (int i = 0; i < 6; i++)
			{
				CCTV cctv = null;
                if (i == (int)CCTV_POSITION.TL)
                    cctv = list.TL;
                else if (i == (int)CCTV_POSITION.BL)
                    cctv = list.BL;
				else if (i == (int)CCTV_POSITION.TM)
					cctv = list.TM;
				else if (i == (int)CCTV_POSITION.BM)
					cctv = list.BM;
				else if (i == (int)CCTV_POSITION.BR)
					cctv = list.BR;
				else if (i == (int)CCTV_POSITION.TR)
					cctv = list.TR;
                
				if (cctv == null)
					writer.WriteLine();
				else
					writer.WriteLine(cctv.ID.ToString());
			}

			writer.Close();
		}

		private string GetDefaultCCTVLogFilePath()
		{
			string strExePath = Application.ExecutablePath;
			int nDotIndex = strExePath.LastIndexOf(".");
            string strLogFilePath = strExePath.Substring(0, nDotIndex) +"_"+ m_szDefaultCCTVFileName + "_DefCCTV.log";
			return strLogFilePath;
		}

		private void ResizeForm(Control ctrlParent)
		{
			this.Size = new Size(ctrlParent.Size.Width, ctrlParent.Size.Height);
            OnFitSize();
		}

		public void SetCCTV(CCTV_POSITION pos, CCTV cctv)
		{
			int nIndex = (int)pos;			
			m_arrCCTV[nIndex].CCTV = cctv;

			if (m_currentCCTVList != null)
			{
                if (pos == CCTV_POSITION.TL)
                    m_currentCCTVList.TL = cctv;
                else if (pos == CCTV_POSITION.BL)
                    m_currentCCTVList.BL = cctv;
				else if (pos == CCTV_POSITION.TM)
					m_currentCCTVList.TM = cctv;
				else if (pos == CCTV_POSITION.BM)
					m_currentCCTVList.BM = cctv;
				else if (pos == CCTV_POSITION.BR)
					m_currentCCTVList.BR = cctv;
				else if (pos == CCTV_POSITION.TR)
					m_currentCCTVList.TR = cctv;
			}
		}

		public CCTV_POSITION SetCCTV(CCTV cctv)
		{
			int nCCTVCount = m_arrCCTV.Count();
			for (int i = 0; i < nCCTVCount; i++)
			{
				BigCCTVCtrl ctrl = (BigCCTVCtrl)m_arrCCTV[i];
				CCTV_POSITION pos = (CCTV_POSITION)i;

				if (ctrl != null && ctrl.IsSelected)
				{
                    Control c = GetContent((int)pos);
                    if (c == null || c.GetType() != typeof(BigCCTVCtrl))
                        continue;
					
					if (m_currentCCTVList != null)
					{
                        if (pos == CCTV_POSITION.TL)
                        {
                            //if (!FormMain.Instance.EquipZoneCCTVMode)

                            
                            {
                                m_currentCCTVList.TL = cctv;
                                //FormMain.Instance.CCTVGuide.SetCCTV(cctv);
                                ctrl.CCTV = cctv;
                            }
                        }
                        else if (pos == CCTV_POSITION.BL)
                        {
                            //if (!FormMain.Instance.EquipZoneCCTVMode)
                            {
                                m_currentCCTVList.BL = cctv;
                                //FormMain.Instance.CCTVGuide.SetCCTV(cctv);
                                ctrl.CCTV = cctv;
                            }
                        }
                        else if (pos == CCTV_POSITION.TM)
                        {
                            m_currentCCTVList.TM = cctv;
                            //FormMain.Instance.CCTVGuide.SetCCTV(cctv);
                            ctrl.CCTV = cctv;
                        }
                        else if (pos == CCTV_POSITION.BM)
                        {
                            m_currentCCTVList.BM = cctv;
                            //FormMain.Instance.CCTVGuide.SetCCTV(cctv);
                            ctrl.CCTV = cctv;
                        }
                        else if (pos == CCTV_POSITION.BR)
                        {
                            m_currentCCTVList.BR = cctv;
                            //FormMain.Instance.CCTVGuide.SetCCTV(cctv);
                            ctrl.CCTV = cctv;
                        }
                        else if (pos == CCTV_POSITION.TR)
                        {
                            m_currentCCTVList.TR = cctv;
                            //FormMain.Instance.CCTVGuide.SetCCTV(cctv);
                            ctrl.CCTV = cctv;
                        }
                        if(!FormMain.Instance.EquipZoneCCTVMode)
                            WriteDefaultCCTV();
					}


                    ctrl.IsSelected = false;

					return pos;
				}
			}

			return CCTV_POSITION.UNKNOWN;
		}

		public void RemoveCCTV()
		{
			for (int i = 0; i < 6; i++)
			{
				BigCCTVCtrl cctvCtrl = m_arrCCTV[i];
                if (cctvCtrl != null && cctvCtrl.IsSelected)
				{
					CCTV_POSITION pos = (CCTV_POSITION)i;
					if (cctvCtrl.CCTV != null)
					{
						cctvCtrl.CCTV = null;
						if (FormMain.Instance.CurrentEquipZone != null)
						{
                            if (i == 0 || i == 3)
                                continue;

							EditEquipZoneCCTV equipZoneCCTV = new EditEquipZoneCCTV();
							equipZoneCCTV.EquipmentZone = FormMain.Instance.CurrentEquipZone;

                            //	public enum CCTV_POSITION { TL = 0, TM = 1, BM = 4, BL = 3, BR = 5, TR = 2, UNKNOWN }

							for (int j = 0; j < 6; j++)
							{
								if(i == j)
									equipZoneCCTV.SetCCTV(j, null);
                                else
								    equipZoneCCTV.SetCCTV(j, m_arrCCTV[j].CCTV);
							}
                            // 영역별 CCTV변경의 경우 즉시 변경하도록 수정 ( 영흥버전)
                            // skkim. 2015-10-08 
                            equipZoneCCTV.Update(FormMain.Instance.DBManager);
							//equipZoneCCTV.AddToManager(FormMain.Instance.PageHome);
						}

						if (m_currentCCTVList != null)
						{
                            if (pos == CCTV_POSITION.TL)
                                m_currentCCTVList.TL = null;
                            else if (pos == CCTV_POSITION.BL)
                                m_currentCCTVList.BL = null;
							else if (pos == CCTV_POSITION.TM)
								m_currentCCTVList.TM = null;
							else if (pos == CCTV_POSITION.BM)
								m_currentCCTVList.BM = null;
							else if (pos == CCTV_POSITION.BR)
								m_currentCCTVList.BR = null;
							else if (pos == CCTV_POSITION.TR)
								m_currentCCTVList.TR = null;
						}
					}
					break;
				}
			}
		}

		private ArrayList m_arrOutdoorSelectedPOI = new ArrayList();
		private ArrayList m_arrIndoorSelectedPOI = new ArrayList();
		private int m_nLayout = -1;

		private void InitSelectedPOI()
		{
            IFormContent frmContent = PageBackstageHome.Instance.ContentForm;

			m_nLayout = frmContent.NumLayout;
            ISensorTooltipOwner viewOutdoor = frmContent.OutdoorView;
            ISensorTooltipOwner viewIndoor = frmContent.IndoorView;

			m_arrOutdoorSelectedPOI.Clear();
			m_arrIndoorSelectedPOI.Clear();

			if (m_nLayout == 1)
			{
				foreach (string szPOIID in viewOutdoor.SelectedPOIList)
				{
                    POI poi = viewOutdoor.FindPOI(szPOIID);
					if (poi == null)
						continue;

					m_arrOutdoorSelectedPOI.Add(poi);
				}
			}
			else if (m_nLayout == 2)
			{
                foreach (string szPOIID in viewOutdoor.SelectedPOIList)
				{
                    POI poi = viewOutdoor.FindPOI(szPOIID);
					if (poi == null)
						continue;

					m_arrOutdoorSelectedPOI.Add(poi);
				}

                foreach (string szPOIID in viewIndoor.SelectedPOIList)
				{
                    POI poi = viewIndoor.FindPOI(szPOIID);
					if (poi == null)
						continue;

					m_arrIndoorSelectedPOI.Add(poi);
				}
			}
			else if (m_nLayout == 3)
			{
                foreach (string szPOIID in viewIndoor.SelectedPOIList)
				{
                    POI poi = viewIndoor.FindPOI(szPOIID);
					if (poi == null)
						continue;

					m_arrIndoorSelectedPOI.Add(poi);
				}
			}
		}

		private Zone m_zoneTarget = null;

		public Zone ZoneTarget
		{
			get { return m_zoneTarget; }
		}

        private ArrayList m_lastCCTVs = null;
        public ArrayList LastCCTVList
        {
            get { return m_lastCCTVs; }
            set { m_lastCCTVs = value; }
        }

        public void SetCCTV(ArrayList arrCCTVs, Zone zoneTarget)
        {
            if (zoneTarget != null)
            {
                m_zoneTarget = zoneTarget;
                m_lastCCTVs = arrCCTVs;
            }

            InitSelectedPOI();

            CCTVList cctvList = GetCCTVList(zoneTarget);

            if (cctvList == null)
            {
                cctvList = new CCTVList();
                SetCCTVList(zoneTarget, cctvList);
            }

            m_currentCCTVList = cctvList;

            if(arrCCTVs != null)
            {
                int nCCTVCount = arrCCTVs.Count;

                // TL = 0, TM = 1, BM = 2, BL = 3, BR = 4, TR = 5,           

                if (0 < nCCTVCount)
                {
                    CCTV cctv = (CCTV)arrCCTVs[0];
                    if (cctv != null)
                        SetCCTV(CCTV_POSITION.TM, cctv);
                }
                else
                {
                    //if (cctvList.TM != null)
                        SetCCTV(CCTV_POSITION.TM, cctvList.TM);
                }

                if (1 < nCCTVCount)
                {
                    CCTV cctv = (CCTV)arrCCTVs[1];
                    if (cctv != null)
                        SetCCTV(CCTV_POSITION.BM, cctv);
                }
                else
                {
                    //if (cctvList.BM != null)
                        SetCCTV(CCTV_POSITION.BM, cctvList.BM);
                }

                if (2 < nCCTVCount)
                {
                    CCTV cctv = (CCTV)arrCCTVs[2];
                    if (cctv != null)
                        SetCCTV(CCTV_POSITION.BR, cctv);
                }
                else
                {
                   // if (cctvList.BR != null)
                        SetCCTV(CCTV_POSITION.BR, cctvList.BR);
                }

                if (3 < nCCTVCount)
                {
                    CCTV cctv = (CCTV)arrCCTVs[3];
                    if (cctv != null)
                        SetCCTV(CCTV_POSITION.TR, cctv);
                }
                else
                {
                    //if (cctvList.TR != null)
                        SetCCTV(CCTV_POSITION.TR, cctvList.TR);
                }

                if (4 < nCCTVCount)
                {
                    CCTV cctv = (CCTV)arrCCTVs[4];
                    if (cctv != null)
                        SetCCTV(CCTV_POSITION.TL, cctv);
                }
                else
                {
                    //if (cctvList.TR != null)
                        SetCCTV(CCTV_POSITION.TL, cctvList.TL);
                }

                if (5 < nCCTVCount)
                {
                    CCTV cctv = (CCTV)arrCCTVs[5];
                    if (cctv != null)
                        SetCCTV(CCTV_POSITION.BL, cctv);
                }
                else
                {
                    //if (cctvList.TR != null)
                        SetCCTV(CCTV_POSITION.BL, cctvList.BL);
                   // else
                    //    SetCCTV(CCTV_PO
                }          
            }
            else
            {
                SetCCTV(CCTV_POSITION.TM, null);
                SetCCTV(CCTV_POSITION.BM, null);
                SetCCTV(CCTV_POSITION.BR, null);
                SetCCTV(CCTV_POSITION.TR, null);
                SetCCTV(CCTV_POSITION.TL, null);
                SetCCTV(CCTV_POSITION.BL, null);
            }
            
        }

		private void Form4CCTV_Resize(object sender, EventArgs e)
		{
			ResizeForm(m_ctrlParent);
		}

		public void OnSelectCCTV(BigCCTVCtrl ctrl)
		{
            CCTVSelectionManager.Instance.SetCurrent(this);

			ctrl.IsSelected = !ctrl.IsSelected;

			if (ctrl.IsSelected)
			{
				ClearSelection(ctrl);

				if (ctrl.CCTV != null)
				{
                    IFormContent frmContent = FormMain.Instance.PageHome.ContentForm;
					POI poi = ctrl.CCTV.POI;

					//frmContent.ZoomTarget(poi.X, poi.Y, poi.Z, poi.IsIndoor);
                    frmContent.SelectPOILoadZone(poi, poi.IsIndoor);

                    if (ctrl.CCTV != null && FormMain.Instance.CCTVList != null)
                    {
                        FormMain.Instance.CCTVList.SelectCCTV(ctrl.CCTV.ID);                        
                    }
				}
				else
					FormMain.Instance.PageHome.ContentForm.ClearPOISelection();

				//FormMain.Instance.CCTVGuide.SetCCTV(ctrl.CCTV);
			}
			else
			{
				FormMain.Instance.PageHome.ContentForm.ClearPOISelection();
				//FormMain.Instance.CCTVGuide.Clear();
			}
		}

		private void ClearSelection(BigCCTVCtrl exceptCtrl)
		{
			foreach (BigCCTVCtrl ctrl in m_arrCCTV)
			{
				if (ctrl != null && ctrl != exceptCtrl)
					ctrl.IsSelected = false;
			}
		}

		public void SetCCTVList(Zone zone, CCTVList list)
		{
			if (zone == null)
				zone = NULL_ZONE;

			m_dicZoneCCTVs[zone] = list;
		}

		public CCTVList GetCCTVList(Zone zone)
		{
			if (zone == null)
				zone = NULL_ZONE;

			if (m_dicZoneCCTVs.ContainsKey(zone))
				return m_dicZoneCCTVs[zone];

			return null;
		}

		public void OnFormClosing()
		{
            IFormContent frmContent = PageBackstageHome.Instance.ContentForm;

			m_nLayout = frmContent.NumLayout;
            ISensorTooltipOwner viewOutdoor = frmContent.OutdoorView;
            ISensorTooltipOwner viewIndoor = frmContent.IndoorView;

			if (m_nLayout == 1)
			{
				viewOutdoor.ClearPOISelection();

				foreach (POI poi in m_arrOutdoorSelectedPOI)
					viewOutdoor.SelectPOI(poi.ID, poi.Facility.IconPath);
			}
			else if (m_nLayout == 2)
			{
				viewOutdoor.ClearPOISelection();
				viewIndoor.ClearPOISelection();

				foreach (POI poi in m_arrOutdoorSelectedPOI)
                    viewOutdoor.SelectPOI(poi.ID, poi.Facility.IconPath);

				foreach (POI poi in m_arrIndoorSelectedPOI)
                    viewIndoor.SelectPOI(poi.ID, poi.Facility.IconPath);
			}
			else if (m_nLayout == 3)
			{
				viewIndoor.ClearPOISelection();

				foreach (POI poi in m_arrIndoorSelectedPOI)
                    viewIndoor.SelectPOI(poi.ID, poi.Facility.IconPath);
			}

			foreach (BigCCTVCtrl ctrl in m_arrCCTV)
			{                
				ctrl.Dispose();
			}
		}

		public void UpdateCCTVGuide()
		{
            //for (int i = 0; i < 4; i++)
            //{
            //    if (m_arrCCTV[i].IsSelected)
            //    {
            //        FormMain.Instance.CCTVGuide.SetCCTV(m_arrCCTV[i].CCTV);
            //        return;
            //    }
            //}

            //FormMain.Instance.CCTVGuide.SetCCTV(null);
		}

		public void SetCCTVMode(CCTVMode mode)
		{
			for (int i = 0; i < 4; i++)
			{
				m_arrCCTV[i].SetCCTVMode(mode);
			}
		}

        protected Panel[] m_arPanels = null;

        private int m_nWGap = 3;
        protected int WGap
        {
            get { return m_nWGap; }
            set { m_nWGap = value; }
        }

        private int m_nHGap = 3;
        protected int HGap
        {
            get { return m_nHGap; }
            set { m_nHGap = value; }
        }

        private int m_nWidthCount = 3;
        protected int WidthCount
        {
            get { return m_nWidthCount; }
            set { m_nWidthCount = value; }
        }

        private int m_nHeightCount = 2;
        protected int HeightCount
        {
            get { return m_nHeightCount; }
            set { m_nHeightCount = value; }
        }
        
    
        protected virtual bool InitPanel()
        {
            ClearPanel();

            int width = this.ClientSize.Width;
            int height = this.ClientSize.Height;

            if (width == 0 || height == 0)
                return false;

            if (m_nWidthCount == 0 || m_nHeightCount == 0)
                return false;

            if (m_arPanels == null)
                m_arPanels = new Panel[m_nWidthCount * m_nHeightCount];

            int scCount = m_arPanels.Length;

            // 가로 Gap의 갯수는 가로칸 + 1
            int nWGapCount = m_nWidthCount + 1;
            // 세로 Gap의 갯수는 세로칸 + 1
            int nHGapCount = m_nHeightCount + 1;

            // 한개 패널의 가로길이는 (전체 길이 - 전체 가로 갭)/가로칸수
            int oneWidth = (width - (nWGapCount * m_nWGap)) / m_nWidthCount;
            // 한개 패널의 세로길이는 (전체 길이 - 전체 세로 갭)/세로칸수
            int oneHeight = (height - (nHGapCount * m_nHGap)) / m_nHeightCount;
                       
            int nLocX = 0;
            int nLocY = 0;
            int nCount = 0;


            bool bExit = false;
            for(int i = 0 ; i < m_nHeightCount; i++ )
            {
                // 패널의 Y위치
                nLocY = (i * (oneHeight + m_nHGap)) + m_nHGap;

                for (int j = 0; j < m_nWidthCount; j++)
                {
                    // 패널의 X 위치
                    nLocX = (j * (oneWidth + m_nWGap)) + m_nWGap;

                    if (m_arPanels != null && m_arPanels[nCount] == null)
                    {
                        m_arPanels[nCount] = new Panel();
                        m_arPanels[nCount].Location = new Point(nLocX, nLocY);
                        m_arPanels[nCount].Margin = new System.Windows.Forms.Padding(0);
                        m_arPanels[nCount].Size = new System.Drawing.Size(oneWidth, oneHeight);
                        m_arPanels[nCount].TabIndex = nCount + 1;
                        m_arPanels[nCount].MinimumSize = new System.Drawing.Size(20, 20);
                        //m_arPanels[nCount].BackColor = Color.Blue;
                        m_arPanels[nCount].Visible = true;

                        Controls.Add(this.m_arPanels[nCount]);
                    }                   
                    nCount++;
                    if (nCount >= scCount)
                    {
                        bExit = true;
                        break;
                    }
                }

                if (bExit == true)
                    break;
            }
            return true;
        }

        protected virtual void ClearPanel()
        {
            if (m_arPanels != null)
            {
                for (int i = 0; i < m_arPanels.Length; i++)
                    SetPanel(i, null);

                m_arPanels = null;
            }            
        }

        protected void OnFormLoad(object sender, EventArgs e)
        {
            OnFitSize();
        }

        protected void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            ClearPanel();
        }

        private void Form4CCTV_FormClosing(object sender, FormClosingEventArgs e)
        {
            ClearPanel();

            if (m_arrCCTV != null)
            {
                for (int i = 0; i < m_arrCCTV.Length; i++)
                {
                    BigCCTVCtrl ctrl = (BigCCTVCtrl)m_arrCCTV[i];
                    if (ctrl != null && ctrl.IsDisposed == false)
                        ctrl.Close();
                }
            }
        }
        
        protected void OnFitSize()
        {
            int width = this.ClientSize.Width;
            int height = this.ClientSize.Height;
                       
            if (width == 0 || height == 0)
                return;

            if (m_arPanels == null)
                return;

            int scCount = m_arPanels.Length;

            // 가로 Gap의 갯수는 가로칸 + 1
            int nWGapCount = m_nWidthCount + 1;
            // 세로 Gap의 갯수는 세로칸 + 1
            int nHGapCount = m_nHeightCount + 1;

            // 한개 패널의 가로길이는 (전체 길이 - 전체 가로 갭)/가로칸수
            int oneWidth = (width - (nWGapCount * m_nWGap)) / m_nWidthCount;
            // 한개 패널의 세로길이는 (전체 길이 - 전체 세로 갭)/세로칸수
            int oneHeight = (height - (nHGapCount * m_nHGap)) / m_nHeightCount;
                       
            int nLocX = 0;
            int nLocY = 0;
            int nCount = 0;


            bool bExit = false;
            for(int i = 0 ; i < m_nHeightCount; i++ )
            {
                // 패널의 Y위치
                nLocY = (i * (oneHeight + m_nHGap)) + m_nHGap;

                for (int j = 0; j < m_nWidthCount; j++)
                {
                    // 패널의 X 위치
                    nLocX = (j * (oneWidth + m_nWGap)) + m_nWGap;

                    if (m_arPanels != null && m_arPanels[nCount] != null)
                    {
                        m_arPanels[nCount].Location = new Point(nLocX, nLocY);
                        m_arPanels[nCount].Size = new System.Drawing.Size(oneWidth, oneHeight);
                    }                   
                    nCount++;
                    if (nCount >= scCount)
                    {
                        bExit = true;
                        break;
                    }
                }

                if (bExit == true)
                    break;
            }
        }

        public virtual Control GetContent(int nIdxPane)
        {
            if (m_arPanels == null)
                return null;

            if (nIdxPane < 0 || nIdxPane >= m_arPanels.Length)
                return null;

            if (m_arPanels[nIdxPane] != null && m_arPanels[nIdxPane].Controls.Count > 0)
            {
                Control preControl = m_arPanels[nIdxPane].Controls[0];
                return preControl;
            }
            return null;
        }


        private PictureBox m_PictureBox1 = null;
        public PictureBox PictureBox1
        {
            get { return m_PictureBox1; }
        }
        private PictureBox m_PictureBox2 = null;
        public PictureBox PictureBox2
        {
            get { return m_PictureBox2; }
            set { m_PictureBox2 = value; }
        }
        
        public virtual void SetPanel(int nIdxPane, Control c, bool bRemoveOnlyPrevCtrl = false)
        {
            if (m_arPanels == null)
                return;

            if (nIdxPane < 0 || nIdxPane >= m_arPanels.Length)
                return;

            if( c == null)
            {
                SetPanel((int)nIdxPane, m_arrCCTV[nIdxPane]);
                return;
            }

            if( nIdxPane == 0 && c.GetType() == typeof(PictureBox))
            {
                m_PictureBox1 = (PictureBox)c;                
            }
            if (nIdxPane == 3 && c.GetType() == typeof(PictureBox))
            {
                m_PictureBox2 = (PictureBox)c;
            }

            try
            {
                if (m_arPanels[nIdxPane] != null && m_arPanels[nIdxPane].Controls.Count > 0)
                {
                    Control preControl = m_arPanels[nIdxPane].Controls[0];
                    m_arPanels[nIdxPane].Controls.Clear();
                    //if (preControl.IsDisposed == false)
                    //{
                    //    if (bRemoveOnlyPrevCtrl == false)
                    //    {
                    //        preControl.Visible = false;
                    //        preControl.Dispose();
                    //    }                        
                    //}
                }

                if (c != null && c.IsDisposed == false)
                {
                    c.Dock = DockStyle.Fill;
                    m_arPanels[nIdxPane].Controls.Add(c);
                    if (c.Visible == false)
                    {
                        c.Visible = true;
                    }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
            }            
        }

        
        private void Form4CCTV_VisibleChanged(object sender, EventArgs e)
        {
            if(this.Visible == false)
            {
                int i = 0;
                i++;
            }

        }
	}

	public class CCTVList
	{
		private CCTV m_cctvTL = null;
		private CCTV m_cctvBL = null;
        private CCTV m_cctvTM = null;
        private CCTV m_cctvBM = null;
		private CCTV m_cctvBR = null;
		private CCTV m_cctvTR = null;

        public ArrayList GetAllCCTV()
        {
            ArrayList arCCTV = new ArrayList();
            // 
            arCCTV.Add(m_cctvTM);
            arCCTV.Add(m_cctvBM);
            arCCTV.Add(m_cctvBR);
            arCCTV.Add(m_cctvTR);
            arCCTV.Add(m_cctvTL);
            arCCTV.Add(m_cctvBL);

            return arCCTV;
        }


        public CCTV TL
        {
            get { return m_cctvTL; }
            set { m_cctvTL = value; }
        }

        public CCTV BL
        {
            get { return m_cctvBL; }
            set { m_cctvBL = value; }
        }


		public CCTV TM
		{
            get { return m_cctvTM; }
            set { m_cctvTM = value; }
		}

		public CCTV BM
		{
            get { return m_cctvBM; }
            set { m_cctvBM = value; }
		}

		public CCTV BR
		{
			get { return m_cctvBR; }
			set { m_cctvBR = value; }
		}

		public CCTV TR
		{
			get { return m_cctvTR; }
			set { m_cctvTR = value; }
		}
	}
}