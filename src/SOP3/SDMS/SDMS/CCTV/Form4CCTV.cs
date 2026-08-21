using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SDMS
{
    public partial class Form4CCTV : Form
    {
        private static Zone NULL_ZONE = new Zone();
        private static Dictionary<Zone, CCTVList> m_dicZoneCCTVs = new Dictionary<Zone, CCTVList>();

        public enum CCTV_POSITION { TL = 0, BL, BR, TR, UNKNOWN }

        private BigCCTVCtrl[] m_arrCCTV = new BigCCTVCtrl[4] { null, null, null, null };
        private Control m_ctrlParent = null;
        private int m_nLineThick = 5;

        private CCTVList m_currentCCTVList = null;

        public Form4CCTV(Control ctrlParent)
        {
            InitializeComponent();

            m_arrCCTV[0] = BigCCTVCtrl.MakeInstance(null, this);//new BigCCTVCtrl(null, this);
            m_arrCCTV[1] = BigCCTVCtrl.MakeInstance(null, this);// new BigCCTVCtrl(null, this);
            m_arrCCTV[2] = BigCCTVCtrl.MakeInstance(null, this);// new BigCCTVCtrl(null, this);
            m_arrCCTV[3] = BigCCTVCtrl.MakeInstance(null, this);// new BigCCTVCtrl(null, this);

            ReadDefaultCCTV();

            m_ctrlParent = ctrlParent;
            ResizeForm(ctrlParent);
        }

        private void ReadDefaultCCTV()
        {
            string strPath = GetDefaultCCTVLogFilePath();

            if (!System.IO.File.Exists(strPath))
                return;

            System.IO.StreamReader reader = new System.IO.StreamReader(strPath, Encoding.UTF8);

            int nID;
            CCTVList list = new CCTVList();

            for (int i = 0; i < 4 && !reader.EndOfStream; i++)
            {
                string strLine = reader.ReadLine();

                if (int.TryParse(strLine, out nID))
                {
                    if (i == (int)CCTV_POSITION.TL)
                        list.TL = CCTVManager.Instance.GetCCTV(nID);
                    else if (i == (int)CCTV_POSITION.BL)
                        list.BL = CCTVManager.Instance.GetCCTV(nID);
                    else if (i == (int)CCTV_POSITION.BR)
                        list.BR = CCTVManager.Instance.GetCCTV(nID);
                    else if (i == (int)CCTV_POSITION.TR)
                        list.TR = CCTVManager.Instance.GetCCTV(nID);
                }
            }

            reader.Close();
            SetCCTVList(NULL_ZONE, list);
        }

        private void WriteDefaultCCTV()
        {
            CCTVList list = GetCCTVList(NULL_ZONE);

            if (list == null)
                return;

            string strPath = GetDefaultCCTVLogFilePath();
            System.IO.StreamWriter writer = new System.IO.StreamWriter(strPath, false, Encoding.UTF8);
            
            for (int i = 0; i < 4; i++)
            {
                CCTV cctv = null;

                if (i == (int)CCTV_POSITION.TL)
                    cctv = list.TL;
                else if (i == (int)CCTV_POSITION.BL)
                    cctv = list.BL;
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
            string strExePath =  Application.ExecutablePath;

            int nDotIndex = strExePath.LastIndexOf(".");
            string strLogFilePath = strExePath.Substring(0, nDotIndex) + "_DefCCTV.log";

            return strLogFilePath;
        }

        private void ResizeForm(Control ctrlParent)
        {
            this.Size = new Size(ctrlParent.Size.Width - 50, ctrlParent.Size.Height);

            int nIndex1 = (int)CCTV_POSITION.TL;
            int nIndex2 = (int)CCTV_POSITION.BL;
            int nIndex3 = (int)CCTV_POSITION.BR;
            int nIndex4 = (int)CCTV_POSITION.TR;

            int nSpace = m_nLineThick * 3 / 2;
            int nHSize = this.Size.Width / 2 - nSpace;
            int nVSize = this.Size.Height / 2 - nSpace;

            if (m_arrCCTV[nIndex1] != null)
            {
                m_arrCCTV[nIndex1].Size = new Size(nHSize, nVSize);
                m_arrCCTV[nIndex1].Location = new Point(m_nLineThick, m_nLineThick);
                m_arrCCTV[nIndex1].Show();
            }

            if (m_arrCCTV[nIndex2] != null)
            {
                m_arrCCTV[nIndex2].Size = new Size(nHSize, nVSize);
                m_arrCCTV[nIndex2].Location = new Point(m_nLineThick, this.Size.Height / 2 + m_nLineThick / 2);
                m_arrCCTV[nIndex2].Show();
            }

            if (m_arrCCTV[nIndex3] != null)
            {
                m_arrCCTV[nIndex3].Size = new Size(nHSize, nVSize);
                m_arrCCTV[nIndex3].Location = new Point(this.Size.Width / 2 + m_nLineThick / 2, this.Size.Height / 2 + m_nLineThick / 2);
                m_arrCCTV[nIndex3].Show();
            }

            if (m_arrCCTV[nIndex4] != null)
            {
                m_arrCCTV[nIndex4].Size = new Size(nHSize, nVSize);
                m_arrCCTV[nIndex4].Location = new Point(this.Size.Width / 2 + m_nLineThick / 2, m_nLineThick);
                m_arrCCTV[nIndex4].Show();
            }
        }

        public void SetCCTV(CCTV_POSITION pos, CCTV cctv)
        {
            int nIndex = (int)pos;
            //m_arrCCTV[nIndex] = new BigCCTVCtrl(cctv, this);
            m_arrCCTV[nIndex].CCTV = cctv;

            if (m_currentCCTVList != null)
            {
                if (pos == CCTV_POSITION.TL)
                    m_currentCCTVList.TL = cctv;
                else if (pos == CCTV_POSITION.BL)
                    m_currentCCTVList.BL = cctv;
                else if (pos == CCTV_POSITION.BR)
                    m_currentCCTVList.BR = cctv;
                else// if (pos == CCTV_POSITION.TR)
                    m_currentCCTVList.TR = cctv;
            }
        }

        public CCTV_POSITION SetCCTV(CCTV cctv)
        {
            int nCCTVCount = m_arrCCTV.Count();

            for (int i = 0; i < nCCTVCount; i++ )
            //foreach (BigCCTVCtrl ctrl in m_arrCCTV)
            {
                BigCCTVCtrl ctrl = (BigCCTVCtrl)m_arrCCTV[i];
                CCTV_POSITION pos = (CCTV_POSITION)i;

                if (ctrl != null && ctrl.IsSelected)
                {
                    FormMain.Instance.CCTVGuide.SetCCTV(cctv);
                    ctrl.CCTV = cctv;

                    if (m_currentCCTVList != null)
                    {
                        if (pos == CCTV_POSITION.TL)
                            m_currentCCTVList.TL = cctv;
                        else if (pos == CCTV_POSITION.BL)
                            m_currentCCTVList.BL = cctv;
                        else if (pos == CCTV_POSITION.BR)
                            m_currentCCTVList.BR = cctv;
                        else// if (pos == CCTV_POSITION.TR)
                            m_currentCCTVList.TR = cctv;
                    }

                    return pos;
                }
            }

            return CCTV_POSITION.UNKNOWN;
        }

        public void RemoveCCTV()
        {
            for (int i = 0; i < 4; i++ )
            {
                BigCCTVCtrl cctvCtrl = m_arrCCTV[i];

                if (cctvCtrl.IsSelected)
                {
                    CCTV_POSITION pos = (CCTV_POSITION)i;
                    FormMain.Instance.CCTVGuide.SetCCTV(null);

                    if (cctvCtrl.CCTV != null)
                    {
                        cctvCtrl.CCTV = null;

                        if (FormMain.Instance.ShowEquipZoneCCTV && FormMain.Instance.CurrentEquipZone != null)
                        {
                            EditEquipZoneCCTV equipZoneCCTV = new EditEquipZoneCCTV();
                            equipZoneCCTV.EquipmentZone = FormMain.Instance.CurrentEquipZone;

                            for (int j=0;j<4;j++)
                            {
                                if (i == j)
                                    equipZoneCCTV.SetCCTV(j, null);
                                else
                                    equipZoneCCTV.SetCCTV(j, m_arrCCTV[j].CCTV);
                            }

                            equipZoneCCTV.AddToManager(FormMain.Instance.PageHome);
                        }

                        if (m_currentCCTVList != null)
                        {
                            if (pos == CCTV_POSITION.TL)
                                m_currentCCTVList.TL = null;
                            else if (pos == CCTV_POSITION.BL)
                                m_currentCCTVList.BL = null;
                            else if (pos == CCTV_POSITION.BR)
                                m_currentCCTVList.BR = null;
                            else// if (pos == CCTV_POSITION.TR)
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
            FormContent frmContent = PageBackstageHome.Instance.ContentForm;

            m_nLayout = frmContent.NumLayout;
            BaseViewEx viewOutdoor = frmContent.OutdoorView;
            BaseViewEx viewIndoor = frmContent.IndoorView;

            m_arrOutdoorSelectedPOI.Clear();
            m_arrIndoorSelectedPOI.Clear();

            if (m_nLayout == 1)
            {
                foreach (int nPOIID in viewOutdoor.SelectedPOIList)
                {
                    POI poi = viewOutdoor.FindPOI(nPOIID);
                    if (poi == null)
                        continue;

                    m_arrOutdoorSelectedPOI.Add(poi);
                }
                /*foreach (POI poi in viewOutdoor.SelectedPOIList)
                    m_arrOutdoorSelectedPOI.Add(poi);*/
            }
            else if (m_nLayout == 2)
            {
                foreach (int nPOIID in viewOutdoor.SelectedPOIList)
                {
                    POI poi = viewOutdoor.FindPOI(nPOIID);
                    if (poi == null)
                        continue;

                    m_arrOutdoorSelectedPOI.Add(poi);
                }

                foreach (int nPOIID in viewIndoor.SelectedPOIList)
                {
                    POI poi = viewIndoor.FindPOI(nPOIID);
                    if (poi == null)
                        continue;

                    m_arrIndoorSelectedPOI.Add(poi);
                }
                /*foreach (POI poi in viewOutdoor.SelectedPOIList)
                    m_arrOutdoorSelectedPOI.Add(poi);

                foreach (POI poi in viewIndoor.SelectedPOIList)
                    m_arrIndoorSelectedPOI.Add(poi);*/
            }
            else if (m_nLayout == 3)
            {
                foreach (int nPOIID in viewIndoor.SelectedPOIList)
                {
                    POI poi = viewIndoor.FindPOI(nPOIID);
                    if (poi == null)
                        continue;

                    m_arrIndoorSelectedPOI.Add(poi);
                }
                /*foreach (POI poi in viewIndoor.SelectedPOIList)
                    m_arrIndoorSelectedPOI.Add(poi);*/
            }
        }

		private Zone m_zoneTarget = null;
		public SDMS.Zone ZoneTarget
		{
			get { return m_zoneTarget; }
		}
        public void SetCCTV(ArrayList arrCCTVs, Zone zoneTarget)
        {
			m_zoneTarget = zoneTarget;

            InitSelectedPOI();

            CCTVList cctvList = GetCCTVList(zoneTarget);

            if (cctvList == null)
            {
                cctvList = new CCTVList();
                SetCCTVList(zoneTarget, cctvList);
            }

            m_currentCCTVList = cctvList;

            if (cctvList.TL != null)
                SetCCTV(CCTV_POSITION.TL, cctvList.TL);

            if (cctvList.BL != null)
                SetCCTV(CCTV_POSITION.BL, cctvList.BL);

            if (cctvList.BR != null)
                SetCCTV(CCTV_POSITION.BR, cctvList.BR);

            if (cctvList.TR != null)
                SetCCTV(CCTV_POSITION.TR, cctvList.TR);

            int nCCTVCount = arrCCTVs.Count;

            for (int i = 0; i < nCCTVCount; i++)
            {
                CCTV_POSITION pos = (CCTV_POSITION)i;
                CCTV cctv = null;

                if (pos == CCTV_POSITION.TL)
                    cctv = cctvList.TL;
                else if (pos == CCTV_POSITION.BL)
                    cctv = cctvList.BL;
                else if (pos == CCTV_POSITION.BR)
                    cctv = cctvList.BR;
                else// if (pos == CCTV_POSITION.TR)
                    cctv = cctvList.TR;

                if (cctv == null)
                    cctv = (CCTV)arrCCTVs[i];

                SetCCTV(pos, cctv);
            }
        }

        private void Form4CCTV_Resize(object sender, EventArgs e)
        {
            ResizeForm(m_ctrlParent);
        }

        public void OnSelectCCTV(BigCCTVCtrl ctrl)
        {
            ctrl.IsSelected = !ctrl.IsSelected;

            if (ctrl.IsSelected)
            {
                ClearSelection(ctrl);

                if (ctrl.CCTV != null)
                {
                    FormContent frmContent = FormMain.Instance.PageHome.ContentForm;
                    POI poi = ctrl.CCTV.POI;

                    frmContent.ZoomTarget(poi.X, poi.Y, poi.Z, poi.IsIndoor);
                    frmContent.SelectPOI(poi, poi.IsIndoor);

                    if (ctrl.CCTV != null && FormMain.Instance.CCTVList != null)
                        FormMain.Instance.CCTVList.SelectCCTV(ctrl.CCTV.ID);
                }
                else
                    FormMain.Instance.PageHome.ContentForm.ClearPOISelection();

                FormMain.Instance.CCTVGuide.SetCCTV(ctrl.CCTV);
            }
            else
            {
                FormMain.Instance.PageHome.ContentForm.ClearPOISelection();
                FormMain.Instance.CCTVGuide.Clear();
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

        public static void SetCCTVList(Zone zone, CCTVList list)
        {
            if (zone == null)
                zone = NULL_ZONE;

            m_dicZoneCCTVs[zone] = list;
        }

        public static CCTVList GetCCTVList(Zone zone)
        {
            if (zone == null)
                zone = NULL_ZONE;

            if (m_dicZoneCCTVs.ContainsKey(zone))
                return m_dicZoneCCTVs[zone];

            return null;
        }

        public void OnFormClosing()
        {
            WriteDefaultCCTV();

            FormContent frmContent = PageBackstageHome.Instance.ContentForm;

            m_nLayout = frmContent.NumLayout;
            BaseViewEx viewOutdoor = frmContent.OutdoorView;
            BaseViewEx viewIndoor = frmContent.IndoorView;

            if (m_nLayout == 1)
            {
                viewOutdoor.ClearPOISelection();

                foreach (POI poi in m_arrOutdoorSelectedPOI)
                    viewOutdoor.SelectPOI(poi.ID);
            }
            else if (m_nLayout == 2)
            {
                viewOutdoor.ClearPOISelection();
                viewIndoor.ClearPOISelection();

                foreach (POI poi in m_arrOutdoorSelectedPOI)
                    viewOutdoor.SelectPOI(poi.ID);

                foreach (POI poi in m_arrIndoorSelectedPOI)
                    viewIndoor.SelectPOI(poi.ID);
            }
            else if (m_nLayout == 3)
            {
                viewIndoor.ClearPOISelection();

                foreach (POI poi in m_arrIndoorSelectedPOI)
                    viewIndoor.SelectPOI(poi.ID);
            }

            foreach (BigCCTVCtrl ctrl in m_arrCCTV)
            {
                ctrl.Dispose();
            }
        }

        public void UpdateCCTVGuide()
        {
            for (int i = 0; i < 4; i++)
            {
                if (m_arrCCTV[i].IsSelected)
                {
                    FormMain.Instance.CCTVGuide.SetCCTV(m_arrCCTV[i].CCTV);
                    return;
                }
            }

            FormMain.Instance.CCTVGuide.SetCCTV(null);
        }

        public void SetCCTVMode(CCTVMode mode)
        {
            for (int i = 0; i < 4; i++)
            {
                m_arrCCTV[i].SetCCTVMode(mode);
            }

        }
    }

    public class CCTVList
    {
        private CCTV m_cctvTL = null;
        private CCTV m_cctvBL = null;
        private CCTV m_cctvBR = null;
        private CCTV m_cctvTR = null;

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
