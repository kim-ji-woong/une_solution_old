using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SDMS.Proxy
{
    internal class StubWorker
    {
        private static StubWorker m_instance = null;

        public static StubWorker Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new StubWorker();

                return m_instance;
            }
        }

        private StubWorker()
        {
        }

        public void SetCheckPosition(string strDisasterName, string strPositionName, string strBroadcastName, string strBuildingID, float fFloorIndex, int nActionStepHistoryID, int nIconID, int nPSMDistance, string strPSMMaterial, float x, float y, float z, int nZoneID, bool isChecked)
        {
            UnE.SOP.IDisasterContainer disContainer = (UnE.SOP.IDisasterContainer)FormMain.Instance.PageHome.ContentForm;

            if (disContainer != null)
            {
                WorkflowStartOption form = new WorkflowStartOption(strDisasterName, strPositionName, strBroadcastName, strBuildingID, fFloorIndex, nActionStepHistoryID, nIconID, nPSMDistance, strPSMMaterial, x, y, z, nZoneID);
                disContainer.SetCheckPoistion(form, isChecked);
            }
        }

        public void SetLastPosition(string strDisasterName, string strPositionName, string strBroadcastName, string strBuildingID, float fFloorIndex, int nActionStepHistoryID, int nIconID, int nPSMDistance, string strPSMMaterial, float x, float y, float z, int nZoneID)
        {
            UnE.SOP.IDisasterContainer disContainer = (UnE.SOP.IDisasterContainer)FormMain.Instance.PageHome.ContentForm;

            if (disContainer != null)
            {
                UnE.SOP.HistoryDisasterPosition pos = new UnE.SOP.HistoryDisasterPosition();
                pos.BroadcastName = strBroadcastName;
                pos.BuildingID = strBuildingID;
                pos.DisasterName = strDisasterName;
                pos.FloorIndex = fFloorIndex;
                pos.HistoryActionStepID = nActionStepHistoryID;
                pos.IconID = nIconID;
                pos.PoistionName = strPositionName;
                pos.PSMDistance = nPSMDistance;
                pos.PSMMaterial = strPSMMaterial;
                pos.X = x;
                pos.Y = y;
                pos.Z = z;
                pos.ZoneID = nZoneID;

                disContainer.LastPos = pos;
            }
        }

        public void RemoveDisasterPos()
        {
            UnE.SOP.IDisasterContainer disContainer = (UnE.SOP.IDisasterContainer)FormMain.Instance.PageHome.ContentForm;

            if (disContainer != null)
            {
                disContainer.LastPos = null;
            }
        }

        public void NullLastPosition()
        {
            UnE.SOP.IDisasterContainer disContainer = (UnE.SOP.IDisasterContainer)FormMain.Instance.PageHome.ContentForm;

            if (disContainer != null)
            {
                disContainer.RemoveDisasterPos();
            }
        }

        public void Update3DView()
        {
            FormMain.Instance.Invoke((MethodInvoker)delegate
            {
                FormMain.Instance.Update3DView();
            });
        }

        public void ToggleMinimumWindow()
        {
            FormMain.Instance.Invoke((MethodInvoker)delegate
            {
                // 작업표시줄에서 Show/Hide
                if (FormMain.Instance.Visible)
                    FormMain.Instance.ToggleWindow(false);
                else
                    FormMain.Instance.ToggleWindow(true);
                // 최대/최소화 반복
                //FormMain.Instance.ToggleMinimumWindow();
            });
        }

        public void ShowWindow()
        {
            FormMain.Instance.Invoke((MethodInvoker)delegate
            {
                FormMain.Instance.ToggleWindow(true);
            });
        }

        public void EarthquakeEvent(int nIntensity, float fMagnitude, string strPosition, bool isRealMode)
        {
            UnE.View.Content.IFormContent content = FormMain.Instance.PageHome.ContentForm;

            if (content != null)
            {
                content.EarthquakeEvent(nIntensity, fMagnitude, strPosition, isRealMode);
            }
        }

        public void Ask_EarthquakeEventIsFinished()
        {
            UnE.View.Content.IFormContent content = FormMain.Instance.PageHome.ContentForm;

            if (content != null)
            {
                bool finished = content.EarthquakeEventIsFinished();
                FormMain.Instance.ProxyMessenger.Reply_EarthquakeEventIsFinished(finished);
            }
        }

        public void OnCheckPositionEnd(bool bResult)
        {
            UnE.SOP.IDisasterContainer disContainer = (UnE.SOP.IDisasterContainer)FormMain.Instance.PageHome.ContentForm;

            if (disContainer != null)
            {
                disContainer.OnCheckEnd(bResult);
            }
        }

        public void ShowBuildingCollapse(string szBuildingID, string szDisplayName)
        {
            UnE.View.Content.IFormContent content = FormMain.Instance.PageHome.ContentForm;

            if (content != null)
            {
                content.ShowBuildingCollapse(szBuildingID, szDisplayName);
            }
        }

        public void CloseBuildingCollapse(string szBuildingID)
        {
            UnE.View.Content.IFormContent content = FormMain.Instance.PageHome.ContentForm;

            if (content != null)
            {
                content.CloseBuilingCollapse(szBuildingID);
            }
        }

        public void ToggleCCTV()
        {
            FormMain.Instance.Invoke((MethodInvoker)delegate
            {
                FormMain.Instance.ShowCCTVForm();
            });
        }

        internal class WorkflowStartOption : UnE.SOP.Workstate.IWorkflowStartOption
        {
            public WorkflowStartOption(string strDisasterName, string strPositionName, string strBroadcastName, string strBuildingID, float fFloorIndex, int nActionStepHistoryID, int nIconID, int nPSMDistance, string strPSMMaterial, float x, float y, float z, int nZoneID)
            {
                DisasterName = strDisasterName;
                PositionName = strPositionName;

                UnE.SOP.HistoryDisasterPosition pos = new UnE.SOP.HistoryDisasterPosition();
                pos.BroadcastName = strBroadcastName;
                pos.BuildingID = strBuildingID;
                pos.DisasterName = strDisasterName;
                pos.FloorIndex = fFloorIndex;
                pos.HistoryActionStepID = nActionStepHistoryID;
                pos.IconID = nIconID;
                pos.PoistionName = strPositionName;
                pos.PSMDistance = nPSMDistance;
                pos.PSMMaterial = strPSMMaterial;
                pos.X = x;
                pos.Y = y;
                pos.Z = z;
                pos.ZoneID = nZoneID;

                LastPosition = pos;
            }

            public string DisasterName { get; set; }
            public UnE.SOP.HistoryDisasterPosition LastPosition { get; set; }

            private string m_strPositionName = "";
            public string PositionName
            {
                get { return m_strPositionName; }
                set
                {
                    m_strPositionName = value;
                    FormMain.Instance.ProxyMessenger.SOPPositionName(m_strPositionName);
                }
            }

            public virtual event UnE.SOP.Workstate.EndCheckPosition OnCheckPositionEnd;

            public void AddLastHistoryDisasterPoistion(UnE.SOP.HistoryDisasterPosition pos)
            {
                FormMain.Instance.ProxyMessenger.AddLastHistoryDisasterPoistion(pos.DisasterName, pos.PoistionName, pos.BroadcastName, pos.BuildingID, pos.FloorIndex, pos.HistoryActionStepID, pos.IconID, pos.PSMDistance, pos.PSMMaterial, pos.X, pos.Y, pos.Z, pos.ZoneID);
            }

            public System.Windows.Forms.Form GetInvokeForm()
            {
                return FormMain.Instance;
                //return null;
            }

            public bool IsHandleCreated()
            {
                return true;
            }
        }
    }
}
