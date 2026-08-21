using System.Windows.Forms;
using UnE.Sensor;
using System.Collections.Generic;

namespace SDMS
{
	public partial class DockingFormLocation : Form
	{
        public enum Mode { Location = 0, PSM, CCTV, DISASTER };

        private static string m_strOriginTitle = "위치 정보";
        public static string OriginTitle
        {
            get { return m_strOriginTitle; }
        }

        private IChildControl m_ctrlAdded = null;

        private List<IChildControl> m_CtrlList = new List<IChildControl>();

        //private int m_nOriginSplitterDistance = 0;
        private Dictionary<Mode, int> m_dicModeDistance = new Dictionary<Mode, int>();

		public DockingFormLocation()
		{
            this.DoubleBuffered = true;

			InitializeComponent();
			SetPOI(null);
		}

		public void SetPOI(POI poi)
		{
            if (poi == null || poi.Zone == null)
                labelLocation.Text = "";
            else
            {
                if (GetCurrentMode() == Mode.Location)
                    labelLocation.Text = poi.Zone.DisplayText;
            }
		}

		public void SetTitle(string szText)
		{
			this.lbTitle.Text = szText;
			this.Text = szText;
		}

        public string GetTitle()
        {
            return this.Text;
        }

        public void AddControl(Control ctrl)
        {
            panelBody.Controls.Add(ctrl);
            ctrl.Dock = DockStyle.Fill;
            ctrl.Show();
            ctrl.Visible = false;
            ctrl.Visible = true;

            if (ctrl is IChildControl)
            {
                m_ctrlAdded = (IChildControl)ctrl;
                m_ctrlAdded.OnAdded(this);
            }
        }

        public void RemoveControl()
        {
            if (m_ctrlAdded != null)
            {
                if (m_ctrlAdded is Control)
                {
                    panelBody.Controls.Remove((Control)m_ctrlAdded);
                    m_ctrlAdded.OnRemoved(this);
                    m_bPSMMode = false;
                    m_bDisasterMgrMode = false;
                    m_bCCTVMode = false;
                }
            }

            m_ctrlAdded = null;
        }

        public Mode GetCurrentMode()
        {
            if (m_bPSMMode == true)
                return Mode.PSM;

            if (m_bDisasterMgrMode == true)
                return Mode.DISASTER;

            if (m_bCCTVMode == true)
               return Mode.CCTV;
             
            return Mode.Location;
        }

        private bool m_bPSMMode = false;
        public bool PSMMode
        {
            get { return m_bPSMMode; }
            set 
            {
                m_bPSMMode = value;

                if (m_bPSMMode == true)
                {
                    m_bCCTVMode = false;
                    m_bDisasterMgrMode = false;
                }
            }
        }

        private bool m_bDisasterMgrMode = false;
        public bool DisasterMgrMode
        {
            get { return m_bDisasterMgrMode; }
            set
            {
                m_bDisasterMgrMode = value;

                if (m_bDisasterMgrMode == true)
                {
                    m_bPSMMode = false;
                    m_bCCTVMode = false;
                }
            }
        }


        private bool m_bCCTVMode = false;
        public bool CCTVMode
        {
            get { return m_bCCTVMode; }
            set 
            {
                m_bCCTVMode = value;
                if (m_bCCTVMode == true)
                {
                    m_bPSMMode = false;
                    m_bDisasterMgrMode = false;
                }
            }
        }
        

        public int GetSplitDistance(SplitContainer splitter)
        {
            Mode mode = GetCurrentMode();
            int nDistance = 0;

            if (m_dicModeDistance.TryGetValue(mode, out nDistance))
            {
                return splitter.Width - nDistance;
            }

            // Default 값
            return splitter.Width - 300;
        }

        public void SetSplitDistance(Mode mode, int nDistance)
        {
            m_dicModeDistance[mode] = nDistance;
        }
	}

    public interface IChildControl
    {
        // 부모컨트롤에 추가된 이후에 호출
        void OnAdded(Control parent);
        // 부모컨트롤에서 제거된 이후에 호출
        void OnRemoved(Control parent);
    }
}
