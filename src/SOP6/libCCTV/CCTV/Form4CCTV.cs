using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UnE.CCTV
{
    public enum CCTVMode { CCTV_ONLY = 0, NORMAL }

    public partial class Form4CCTV : Form
    {
        private Zone NULL_ZONE = new Zone();
        private Dictionary<Zone, CCTVList> m_dicZoneCCTVs = new Dictionary<Zone, CCTVList>();

        //public enum CCTV_POSITION { TL = 0, TM = 1, BM = 2, BL = 3, BR = 4, TR = 5, UNKNOWN }        
        public enum CCTV_POSITION { TL = 0, TM = 1, BM = 4, BL = 3, BR = 5, TR = 2, UNKNOWN = 6 }


        private BigCCTVCtrl[] m_arrCCTV = new BigCCTVCtrl[6] { null, null, null, null, null, null };

        private System.Windows.Forms.Control m_ctrlParent = null;
        public System.Windows.Forms.Control ParentCtrl
        {
            get { return m_ctrlParent; }
            set { m_ctrlParent = value; }
        }

        private int m_nLineThick = 5;

        private CCTVList m_currentCCTVList = null;

        private string m_szDefaultCCTVFileName = "";

        private ICCTVFormOwner m_FormOwner = null;
        private ICCTVControl m_resizeCtrl = null, m_bigCtrl = null;
        private bool m_closeForm = false;

        public ICCTVFormOwner GetOwner()
        {
            return m_FormOwner;
        }

        public void SetOwner(ICCTVFormOwner owner)
        {
            m_FormOwner = owner;
        }


        public Form4CCTV(System.Windows.Forms.Control ctrlParent, string szDefFileName)
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

            for (int i = 0; i < m_arrCCTV.Count();i++ )
            {
                m_arrCCTV[i].PositionIndex = i;
            }

            SetPanel((int)CCTV_POSITION.TM, m_arrCCTV[(int)CCTV_POSITION.TM]);
            SetPanel((int)CCTV_POSITION.TR, m_arrCCTV[(int)CCTV_POSITION.TR]);
            SetPanel((int)CCTV_POSITION.BM, m_arrCCTV[(int)CCTV_POSITION.BM]);
            SetPanel((int)CCTV_POSITION.BR, m_arrCCTV[(int)CCTV_POSITION.BR]);

            SetPanel((int)CCTV_POSITION.TL, m_arrCCTV[(int)CCTV_POSITION.TL]);
            SetPanel((int)CCTV_POSITION.BL, m_arrCCTV[(int)CCTV_POSITION.BL]);

            //ReadDefaultCCTV();
            m_ctrlParent = ctrlParent;
            ResizeForm(ctrlParent);



            m_PictureBox1.BackColor = Color.Black;
            m_PictureBox2.BackColor = Color.Black;
            m_PictureBox3.BackColor = Color.Black;
            m_PictureBox4.BackColor = Color.Black;

            m_PictureBox1.Owner = this;
            m_PictureBox2.Owner = this;
            m_PictureBox3.Owner = this;
            m_PictureBox4.Owner = this;

            m_arrPictureBox = new TitlePictureBox[4] { m_PictureBox1, m_PictureBox2, m_PictureBox3, m_PictureBox4 };

            //System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(ConnectionThread));
            //t.Start();
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
            string strLogFilePath = strExePath.Substring(0, nDotIndex) + "_" + m_szDefaultCCTVFileName + "_DefCCTV.log";
            return strLogFilePath;
        }

        private void ResizeForm(System.Windows.Forms.Control ctrlParent)
        {
            this.Size = new Size(ctrlParent.Size.Width, ctrlParent.Size.Height);
            OnFitSize();
        }


        private int nAsyncType = 1;
        private void AsyncPreset()
        {
            int nType = nAsyncType;
            System.Threading.Thread.Sleep(1500);
            
            try
            {
                this.Invoke(new Action(() =>
                {

                    for (int i = 0; i < m_arPanels.Length; i++)
                    {
                        if (m_arPanels[i] != null)
                        {
                            Panel p = m_arPanels[i];
                            if (p.Controls.Count > 0)
                            {
                                System.Windows.Forms.Control preControl = p.Controls[0];
                                if (preControl is BigCCTVCtrl)
                                {
                                    ((BigCCTVCtrl)preControl).SetPreset(nType);
                                }
                            }
                        }
                    }
                }));            
            }
            catch(Exception)
            {
            }           
        }

        public void SetPreset(int nType)
        {
            nAsyncType = nType;
            System.Threading.Thread t = new System.Threading.Thread(AsyncPreset);
            t.Start();
        }

        public void ClearCCTV()
        {
            for(Int32 index = 0; index < m_arrCCTV.Length ; index++)
            {
                m_arrCCTV[index].CCTV = null;
            }           
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

        public void SelectCCTV(Int32 pCCTVIndex)
        {
            if (pCCTVIndex >= 0)
            {
                for (Int32 index = 0; index < m_arrCCTV.Count(); index++)
                {
                    m_arrCCTV[index].IsSelected = false;
                }
                m_arrCCTV[pCCTVIndex].IsSelected = true;
            }
        }

        public CCTV_POSITION SetCCTV(CCTV cctv, Int32 pCCTVIndex, bool writeDefCCTV = true)            
        {
            SelectCCTV(pCCTVIndex);            

            return SetCCTV(cctv, writeDefCCTV);
        }

        public CCTV_POSITION SetCCTV(CCTV cctv, bool writeDefCCTV = true)
        {
            int nCCTVCount = m_arrCCTV.Count();
            for (int i = 0; i < nCCTVCount; i++)
            {
                BigCCTVCtrl ctrl = (BigCCTVCtrl)m_arrCCTV[i];
                CCTV_POSITION pos = (CCTV_POSITION)i;                

                if (ctrl != null && ctrl.IsSelected)
                {
                    System.Windows.Forms.Control c = GetContent((int)pos);
                    if (c == null || c.GetType() != typeof(BigCCTVCtrl))
                        continue;

                    if (m_currentCCTVList != null)
                    {
                        if (pos == CCTV_POSITION.TL)
                        {                           
                            m_currentCCTVList.TL = cctv;
                            ctrl.CCTV = cctv;                            
                        }
                        else if (pos == CCTV_POSITION.BL)
                        {
                            m_currentCCTVList.BL = cctv;  
                            ctrl.CCTV = cctv;                            
                        }
                        else if (pos == CCTV_POSITION.TM)
                        {
                            m_currentCCTVList.TM = cctv;                            
                            ctrl.CCTV = cctv;
                        }
                        else if (pos == CCTV_POSITION.BM)
                        {
                            m_currentCCTVList.BM = cctv;                            
                            ctrl.CCTV = cctv;
                        }
                        else if (pos == CCTV_POSITION.BR)
                        {
                            m_currentCCTVList.BR = cctv;                           
                            ctrl.CCTV = cctv;
                        }
                        else if (pos == CCTV_POSITION.TR)
                        {
                            m_currentCCTVList.TR = cctv;                            
                            ctrl.CCTV = cctv;
                        }
                        if (!ProxyCCTV.Instance.EquipZoneCCTVMode && writeDefCCTV)
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
                        if (ProxyCCTV.Instance.ShowEquipZoneCCTV && ProxyCCTV.Instance.CurrentEquipZone != null)
                        {
                            if (i == 0 || i == 3)
                                continue;

                            EditEquipZoneCCTV equipZoneCCTV = new EditEquipZoneCCTV();
                            equipZoneCCTV.EquipmentZone = ProxyCCTV.Instance.CurrentEquipZone;

                            //	public enum CCTV_POSITION { TL = 0, TM = 1, BM = 4, BL = 3, BR = 5, TR = 2, UNKNOWN }

                            for (int j = 0; j < 6; j++)
                            {
                                if (i == j)
                                    equipZoneCCTV.SetCCTV(j, null);
                                else
                                    equipZoneCCTV.SetCCTV(j, m_arrCCTV[j].CCTV);
                            }
                            
                            // 영역별 CCTV변경의 경우 즉시 변경하도록 수정 ( 영흥버전)
                            // skkim. 2015-10-08 
                            equipZoneCCTV.Update(FormMain.Instance.DBManager);
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

            CCTVList cctvList = GetCCTVList(zoneTarget);

            if (cctvList == null)
            {
                cctvList = new CCTVList();
                SetCCTVList(zoneTarget, cctvList);
            }

            m_currentCCTVList = cctvList;

            if (arrCCTVs != null)
            {
                int nCCTVCount = arrCCTVs.Count;

                // TL = 0, TM = 1, BM = 2, BL = 3, BR = 4, TR = 5,           

                if (0 < nCCTVCount)
                {
                    CCTV cctv = (CCTV)arrCCTVs[0];
                    //if (cctv != null)
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
                    //if (cctv != null)
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
                    //if (cctv != null)
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
                    //if (cctv != null)
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
                    //if (cctv != null)
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
                    //if (cctv != null)
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

        public void OnSelectCCTV(ICCTVControl ctrl)
        {
            for (int i = 0; i < m_arrCCTV.Length; i++)
            {
                if (m_arrCCTV[i] != ctrl)
                    m_arrCCTV[i].IsSelected = false;
                else
                    m_arrCCTV[i].IsSelected = !m_arrCCTV[i].IsSelected;
            }
            CCTVSelectionManager.Instance.SetCurrent(this);

            foreach (TitlePictureBox pictureBox in m_arrPictureBox)
            {
                if (pictureBox == ctrl)
                    pictureBox.IsSelected = !pictureBox.IsSelected;
                else
                    pictureBox.IsSelected = false;
            }
            
            //if (ctrl.IsSelected)
            //{
            //    ClearSelection(ctrl);

            //    if (ctrl.CCTV != null)
            //    {
            //        //FormContent2D frmContent = FormMain.Instance.PageHome.ContentForm;
            //        //POI poi = ctrl.CCTV.POI;

            //        ////frmContent.ZoomTarget(poi.X, poi.Y, poi.Z, poi.IsIndoor);
            //        //frmContent.SelectPOILoadZone(poi, poi.IsIndoor);

            //        //if (ctrl.CCTV != null && FormMain.Instance.CCTVList != null)
            //        //{
            //        //    FormMain.Instance.CCTVList.SelectCCTV(ctrl.CCTV.ID);
            //        //}
            //    }
            //    else
            //        FormMain.Instance.PageHome.ContentForm.ClearPOISelection();

            //    FormMain.Instance.CCTVGuide.SetCCTV(ctrl.CCTV);
            //}
            //else
            //{
            //    FormMain.Instance.ContentForm.ClearPOISelection();
            //    FormMain.Instance.CCTVGuide.Clear();
            //}
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
            //FormContent2D frmContent = PageBackstageHome.Instance.ContentForm;

            //m_nLayout = frmContent.NumLayout;
            //ISensorTooltipOwner viewOutdoor = frmContent.OutdoorView;
            //ISensorTooltipOwner viewIndoor = frmContent.IndoorView;

            //if (m_nLayout == 1)
            //{
            //    viewOutdoor.ClearPOISelection();

            //    foreach (POI poi in m_arrOutdoorSelectedPOI)
            //        viewOutdoor.SelectPOI(poi.ID);
            //}
            //else if (m_nLayout == 2)
            //{
            //    viewOutdoor.ClearPOISelection();
            //    viewIndoor.ClearPOISelection();

            //    foreach (POI poi in m_arrOutdoorSelectedPOI)
            //        viewOutdoor.SelectPOI(poi.ID);

            //    foreach (POI poi in m_arrIndoorSelectedPOI)
            //        viewIndoor.SelectPOI(poi.ID);
            //}
            //else if (m_nLayout == 3)
            //{
            //    viewIndoor.ClearPOISelection();

            //    foreach (POI poi in m_arrIndoorSelectedPOI)
            //        viewIndoor.SelectPOI(poi.ID);
            //}

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
            for (int i = 0; i < m_nHeightCount; i++)
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
                        m_arPanels[nCount].MouseDown += Form4CCTV_MouseDown;
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

        void Form4CCTV_MouseDown(object sender, MouseEventArgs e)
        {
            //System.Diagnostics.Trace.WriteLine("MouseDown" + sender.ToString());
            MessageBox.Show("4CCTV Click");
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
            m_closeForm = true;
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
            for (int i = 0; i < m_nHeightCount; i++)
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

        public virtual System.Windows.Forms.Control GetContent(int nIdxPane)
        {
            if (m_arPanels == null)
                return null;

            if (nIdxPane < 0 || nIdxPane >= m_arPanels.Length)
                return null;

            if (m_arPanels[nIdxPane] != null && m_arPanels[nIdxPane].Controls.Count > 0)
            {
                System.Windows.Forms.Control preControl = m_arPanels[nIdxPane].Controls[0];
                return preControl;
            }
            return null;
        }


        private TitlePictureBox m_PictureBox1 = new TitlePictureBox();
        public TitlePictureBox PictureBox1
        {
            get { return m_PictureBox1; }
        }
        private TitlePictureBox m_PictureBox2 = new TitlePictureBox();
        public TitlePictureBox PictureBox2
        {
            get { return m_PictureBox2; }
            set { m_PictureBox2 = value; }
        }

        private TitlePictureBox m_PictureBox3 = new TitlePictureBox();
        public TitlePictureBox PictureBox3
        {
            get { return m_PictureBox3; }
        }
        private TitlePictureBox m_PictureBox4 = new TitlePictureBox();
        public TitlePictureBox PictureBox4
        {
            get { return m_PictureBox4; }
            set { m_PictureBox4 = value; }
        }

        private TitlePictureBox[] m_arrPictureBox = null;

        public virtual void SetPanel(int nIdxPane, System.Windows.Forms.Control c, bool bRemoveOnlyPrevCtrl = false)
        {
            if (m_arPanels == null)
                return;

            if (nIdxPane < 0 || nIdxPane >= m_arPanels.Length)
                return;

            if (c == null)
            {
                SetPanel((int)nIdxPane, m_arrCCTV[nIdxPane]);
                return;
            }

            if (nIdxPane == 0 && c is PictureBox)
            {
                m_PictureBox1 = (TitlePictureBox)c;
            }
            if (nIdxPane == 3 && c is PictureBox)
            {
                m_PictureBox2 = (TitlePictureBox)c;
            }

            try
            {
                if (m_arPanels[nIdxPane] != null && m_arPanels[nIdxPane].Controls.Count > 0)
                {
                    System.Windows.Forms.Control preControl = m_arPanels[nIdxPane].Controls[0];
                    m_arPanels[nIdxPane].Controls.Clear();
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
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
            }
        }


        private void Form4CCTV_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible == false)
            {
                int i = 0;
                i++;
            }

        }

        public void OnMouseDoubleClick(ICCTVControl ctrl)
        {
            if (ctrl.EnableDoubleClickEvent == false)
                return;
            /*if (ctrl is BigCCTVCtrl)
            {
                BigCCTVCtrl control = (BigCCTVCtrl)ctrl;

                if (control.CCTV == null)
                {
                    // CCTV 없음
                    return;
                }
                else
                {
                    // CCTV
#if _IDIS_NVR_ || _ITX_NVR_
                    return;
#endif
                }
            }
            else
            {
                // 이미지
            }*/

            if (ctrl.LargeMode == false)
            {
                int d = ctrl.LineThick * 2;
                ctrl.LargeMode = true;
                ctrl.SaveSize = ctrl.Size;
                ctrl.SaveLoc = ctrl.Location;
                ctrl.ParentControl = ctrl.Parent;
                ctrl.ParentForm = ctrl.Parent.Parent;

                if (UnE.SOP.ProxySOP.Instance.ShowCCTVForm == true)
                {
                    ctrl.ParentControl.Controls.Remove(ctrl.ThisControl);
                    ctrl.ParentForm.Controls.Add(ctrl.ThisControl);
                    ctrl.Location = new Point(ctrl.LineThick, ctrl.LineThick);
                    ctrl.Size = new Size(ctrl.Parent.Parent.Width - d, ctrl.Parent.Parent.Height - d);
                }
                else
                {
                    ctrl.Location = new Point(ctrl.LineThick, ctrl.LineThick);
                    ctrl.Size = new Size(ctrl.Parent.Width - d, ctrl.Parent.Height - d);
                }

                ctrl.BringToFront();
            }
            else
            {
                if (UnE.SOP.ProxySOP.Instance.ShowCCTVForm == true)
                {
                    ctrl.ParentForm.Controls.Remove(ctrl.ThisControl);
                    ctrl.ParentControl.Controls.Add(ctrl.ThisControl);
                }
                ctrl.LargeMode = false;
                ctrl.Size = ctrl.SaveSize;
                ctrl.Location = ctrl.SaveLoc;
            }

            m_resizeCtrl = ctrl;
        }

        private void ConnectionThread()
        {
            // nRefreshTime(초) 만큼 경과하면 재접속 하도록 한다.
            // 중간에 접속이 끊어졌을수도 있으니 미리 조치한다.
            int nRefreshTime = 6 * 3600;

            while (m_closeForm == false)
            {
                foreach (BigCCTVCtrl cctvCtrl in m_arrCCTV)
                {
                    // Ctrl마다 재접속 시간이 조금씩 다르도록 한다.
                    int nTimeValue = cctvCtrl.PositionIndex * 60;

                    if (cctvCtrl == null || cctvCtrl.ConnectionTime == null)
                        continue;

                    TimeSpan span = DateTime.Now - cctvCtrl.ConnectionTime.Data;

                    if (span.TotalSeconds >= (nRefreshTime + nTimeValue))
                    {
                        // 접속유지 시간이 경과한 경우
                        cctvCtrl.Reload();
                    }
                    // 일반모드에서 cctvCtrl이 접속정보가 있는데도 Process가 종료되었거나
                    // 전체화면모드에서 cctvCtrl이(전체화면 ctrl) 접속정보가 있는데도 Process가 종료되었을 경우
                    else if ((m_bigCtrl == null && cctvCtrl.IsAlive() == false) ||
                        (m_bigCtrl != null && cctvCtrl == m_bigCtrl && cctvCtrl.IsAlive() == false))
                    {
                        // 접속유지 시간이 경과하지 않았으나 Process가 종료된 경우
                        cctvCtrl.Reload();
                    }

                    if (m_resizeCtrl != null)
                    {
                        ProcessResize();
                        break;
                    }
                }

                for (int i=0;i<10;i++)
                {
                    if (m_closeForm)
                        break;

                    if (m_resizeCtrl != null)
                        ProcessResize();

                    System.Threading.Thread.Sleep(1000);
                }
                /*foreach (BigCCTVCtrl cctvCtrl in m_arrCCTV)
                {
                    if (cctvCtrl == null)
                        continue;

                    if (cctvCtrl.IsAlive)
                    {
                        // 접속이 끊어져 있으면 다시 연결하도록 한다.
                        cctvCtrl.CheckConnection();
                    }
                }

                for (int i=0;i<10;i++)
                {
                    if (m_closeForm)
                        break;

                    System.Threading.Thread.Sleep(1000);
                }*/
            }
        }

        private void ProcessResize()
        {
            if (m_resizeCtrl == null)
                return;

            m_bigCtrl = m_resizeCtrl;

            // 특정 Ctrl이 최대화되면 나머지 CCTV들은 화면에 나타나지 않으므로 Process를 멈춘다.
            // 최대화되었던 Ctrl이 원래대로 돌아가면 멈추었던 Process들을 재가동한다.
            foreach (BigCCTVCtrl cctvCtrl in m_arrCCTV)
            {
                if (cctvCtrl != m_resizeCtrl)
                {
                    if (m_resizeCtrl.LargeMode)
                        cctvCtrl.Pause();
                    else
                        cctvCtrl.Resume();
                }
            }

            if (m_resizeCtrl.LargeMode == false)
                m_bigCtrl = null;

            m_resizeCtrl = null;
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

    public class TitlePictureBox : PictureBox, ICCTVControl
    {
        private Form4CCTV m_owner = null;
        private Label lbTitle = new Label();
        private bool m_isSelected = false;

        private Size mSaveSize = new Size();
        private Point mSaveLoc = new Point();
        private bool m_bLargeMode = false;
        private int m_nLineThick = 5;
        private System.Windows.Forms.Control mParentContorl = null;
        private System.Windows.Forms.Control mParentForm = null;

        public Form4CCTV Owner
        {
            get { return m_owner; }
            set { m_owner = value; }
        }

        public string Title
        {
            get { return lbTitle.Text; }
            set { lbTitle.Text = value; }
        }

        public bool LargeMode
        {
            get { return m_bLargeMode; }
            set { m_bLargeMode = value; }
        }

        public int LineThick
        {
            get { return m_nLineThick; }
            set { m_nLineThick = value; }
        }

        public Size SaveSize
        {
            get { return mSaveSize; }
            set { mSaveSize = value; }
        }

        public Point SaveLoc
        {
            get { return mSaveLoc; }
            set { mSaveLoc = value; }
        }

        public System.Windows.Forms.Control ParentControl
        {
            get { return mParentContorl; }
            set { mParentContorl = value; }
        }

        public System.Windows.Forms.Control ParentForm
        {
            get { return mParentForm; }
            set { mParentForm = value; }
        }

        public System.Windows.Forms.Control ThisControl
        {
            get { return this; }
        }

        public bool EnableDoubleClickEvent
        {
            get { return true; }
        }

        public bool IsSelected
        {
            get { return m_isSelected; }
            set
            {
                m_isSelected = value;

                if (m_isSelected)
                {
                    this.BackColor = Color.FromArgb(109, 155, 206);
                    this.lbTitle.ForeColor = Color.Orange;
                }
                else
                {
                    this.BackColor = System.Windows.Forms.Control.DefaultBackColor;
                    this.lbTitle.ForeColor = Color.White;
                }
            }
        }

        public TitlePictureBox()
        {
            this.lbTitle.AutoSize = true;
            this.lbTitle.BackColor = System.Drawing.Color.Black;
            this.lbTitle.Font = new System.Drawing.Font("맑은 고딕", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lbTitle.ForeColor = System.Drawing.Color.White;
            this.lbTitle.Location = new System.Drawing.Point(12, 9);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(143, 25);
            this.lbTitle.Text = "";

            this.lbTitle.MouseDoubleClick += new MouseEventHandler(this.TitlePictureBox_MouseDoubleClick);
            this.lbTitle.MouseDown += new MouseEventHandler(this.TitlePictureBox_MouseDown);
            this.MouseDoubleClick += new MouseEventHandler(this.TitlePictureBox_MouseDoubleClick);
            this.MouseDown += new MouseEventHandler(this.TitlePictureBox_MouseDown);

            this.Controls.Add(this.lbTitle);
        }

        private void TitlePictureBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (m_owner != null)
            {
                m_owner.OnMouseDoubleClick(this);
            }
        }

        private void TitlePictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (m_owner != null)
                {
                    m_owner.OnSelectCCTV(this);
                }
            }
        }
    }

    public interface ICCTVControl
    {
        bool LargeMode
        {
            get;
            set;
        }

        int LineThick
        {
            get;
            set;
        }

        Size SaveSize
        {
            get;
            set;
        }

        Point SaveLoc
        {
            get;
            set;
        }

        System.Windows.Forms.Control ParentControl
        {
            get;
            set;
        }

        System.Windows.Forms.Control ParentForm
        {
            get;
            set;
        }

        System.Windows.Forms.Control Parent
        {
            get;
            set;
        }

        System.Windows.Forms.Control ThisControl
        {
            get;
        }

        Size Size
        {
            get;
            set;
        }

        Point Location
        {
            get;
            set;
        }

        bool EnableDoubleClickEvent
        {
            get;
        }

        void BringToFront();
    }
}