using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace CCTVChecker
{
    public partial class Form4CCTV : Form
    {
        private static Zone NULL_ZONE = new Zone();

        private BigCCTVCtrl[] m_arrCCTV = new BigCCTVCtrl[15];
        private Control m_ctrlParent = null;
        private int m_nLineThick = 5;

        public Form4CCTV(Control ctrlParent)
        {
            InitializeComponent();

            for (int i = 0; i < 15; i++)
            {
                m_arrCCTV[i] = BigCCTVCtrl.MakeInstance(null, this);//new BigCCTVCtrl(null, this);
            }
            
            m_ctrlParent = ctrlParent;
            ResizeForm(ctrlParent);
            FormCCTVInfo.Instance.Set4CCTV(this);
        }
        
        private void ResizeForm(Control ctrlParent)
        {
            for (int i = 0; i < 15; i++)
            {
                int x = this.Size.Width / 5 * (i % 5);
                int y = this.Size.Height / 3 * (i / 5);

                m_arrCCTV[i].Location = new Point(x, y);
                m_arrCCTV[i].Size = new Size(this.Size.Width / 5, this.Size.Height / 3);
                m_arrCCTV[i].Show();
            }
        }

        public void SetCCTV(int nIndex, CCTV cctv)
        {
            m_arrCCTV[nIndex].CCTV = cctv;
        }

        public int SetCCTV(CCTV cctv)
        {
            int nCCTVCount = m_arrCCTV.Count();
            BigCCTVCtrl selectedCtrl = null;
            int nSelectedIndex = -1;

            for (int i = 0; i < nCCTVCount; i++ )
            {
                BigCCTVCtrl ctrl = (BigCCTVCtrl)m_arrCCTV[i];

                if (ctrl != null && ctrl.IsSelected)
                {
                    if (selectedCtrl != null)
                        return -1;

                    selectedCtrl = ctrl;
                    nSelectedIndex = i;
                }
            }

            if (selectedCtrl != null)
            {
                FormMain.Instance.CCTVGuide.SetCCTV(cctv);
                selectedCtrl.CCTV = cctv;
            }

            return nSelectedIndex;
        }

        public void RemoveCCTV()
        {
            int nCCTVCount = m_arrCCTV.Count();

            for (int i = 0; i < nCCTVCount; i++)
            {
                BigCCTVCtrl cctvCtrl = m_arrCCTV[i];

                if (cctvCtrl != null && cctvCtrl.IsSelected)
                {
                    FormMain.Instance.CCTVGuide.SetCCTV(null);

                    if (cctvCtrl.CCTV != null)
                    {
                        cctvCtrl.CCTV = null;
                    }

                    break;
                }
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
                FormMain.Instance.CCTVGuide.SetCCTV(ctrl.CCTV);
            }
        }

        public void OnMultiSelectCCTV(BigCCTVCtrl ctrl)
        {
            ctrl.IsSelected = !ctrl.IsSelected;

            if (ctrl.IsSelected)
            {
                FormMain.Instance.CCTVGuide.SetCCTV(ctrl.CCTV);
            }
        }

        public int GetSelectedCount()
        {
            int nCount = 0;

            foreach (BigCCTVCtrl ctrl in m_arrCCTV)
            {
                if (ctrl.IsSelected)
                    nCount++;
            }

            return nCount;
        }

        public bool IsAbleToSelect()
        {
            return GetSelectedCount() < 4;
        }

        public CCTV[] GetSelectedCCTVs()
        {
            ArrayList arr = new ArrayList();

            foreach (BigCCTVCtrl ctrl in m_arrCCTV)
            {
                if (ctrl.IsSelected && ctrl.CCTV != null)
                    arr.Add(ctrl.CCTV);
            }

            int nCount = arr.Count;

            if (nCount == 0)
                return null;

            CCTV[] arrCCTVs = new CCTV[4];

            for (int i = 0; i < nCount; i++)
            {
                CCTV cctv = (CCTV)arr[i];
                arrCCTVs[i] = cctv;
            }

            for (int i = nCount; i < 4; i++)
            {
                arrCCTVs[i] = null;
            }

            return arrCCTVs;
        }

        public void ClearSelection(BigCCTVCtrl exceptCtrl)
        {
            foreach (BigCCTVCtrl ctrl in m_arrCCTV)
            {
                if (ctrl != null && ctrl != exceptCtrl)
                    ctrl.IsSelected = false;
            }
        }

        public void OnFormClosing()
        {
            //WriteDefaultCCTV();

            //FormContent frmContent = PageBackstageHome.Instance.ContentForm;

            //m_nLayout = frmContent.NumLayout;
            //BaseViewEx viewOutdoor = frmContent.OutdoorView;
            //BaseViewEx viewIndoor = frmContent.IndoorView;

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

            //foreach (BigCCTVCtrl ctrl in m_arrCCTV)
            //{
            //    ctrl.Dispose();
            //}
        }

        public void UpdateCCTVGuide()
        {
            int nCCTVCount = m_arrCCTV.Count();

            for (int i = 0; i < nCCTVCount; i++)
            {
                if (m_arrCCTV[i].IsSelected)
                {
                    FormMain.Instance.CCTVGuide.SetCCTV(m_arrCCTV[i].CCTV);
                    return;
                }
            }

            FormMain.Instance.CCTVGuide.SetCCTV(null);
        }

        public void LoadCCTV(ArrayList arrCCTVs)
        {
            int nCCTVCount = arrCCTVs.Count;

            for (int i = 0; i < nCCTVCount; i++)
            {
                m_arrCCTV[i].CCTV = (CCTV)arrCCTVs[i];
            }
        }

        public void LoadCCTV(CCTV[] cctvs)
        {
            for (int i = 0; i < cctvs.Length; i++)
            {               
                m_arrCCTV[i].CCTV = (CCTV)cctvs[i];
            }
        }

        public void Reload()
        {
            for (int i = 0; i < 4; i++)
            {
                if (m_arrCCTV[i] != null)
                    m_arrCCTV[i].Reload();
            }
        }
    }
}
