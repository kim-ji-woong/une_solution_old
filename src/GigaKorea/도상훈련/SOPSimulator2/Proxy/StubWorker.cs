using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SOPMonitoringSystem
{
    public class StubWorker// : SDMS.IProxyMessenser
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

        private bool m_loadingSDMS = false;

        public bool SDMSisLoading
        {
            get { return m_loadingSDMS; }
        }

        private UnE.SOP.Workstate.IWorkflowStartOption m_workFlowStartOption = null;

        public UnE.SOP.Workstate.IWorkflowStartOption WorkFlowStartOption
        {
            get { return m_workFlowStartOption; }
            set { m_workFlowStartOption = value; }
        }

        private StubWorker()
        {
        }

        public bool OnlySDMS()
        {
            return FormSOP.Instance.OnlySDMS;
        }

        public void RunSOPSimulator()
        {
            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                //FormSOP.Instance.ShowMonitoringSystem();
                FormSOP.Instance.ShowMonitoringSystem(true);
            });
        }

        public void ShowSOPSimulator()
        {
            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                FormSOP.Instance.ShowMonitoringSystem(true);
            });
        }

        public void HideSOPSimulator()
        {
            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                FormSOP.Instance.ShowMonitoringSystem(false);
            });
        }

        public bool IsVisibleSOPSimulator()
        {
            bool visible = true;

            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                visible = FormSOP.Instance.Visible;
            });

            return visible;
        }

        public void ShowMissionStatus()
        {
            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                FormSOP.Instance.VisibleMissionStatus = true;
            });
        }

        public void HideMissionStatus()
        {
            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                FormSOP.Instance.VisibleMissionStatus = false;
            });
        }
            
        public bool IsVisibleMissionStatus()
        {
            bool visible = true;

            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                visible = FormSOP.Instance.VisibleMissionStatus;
            });

            return visible;
        }

        public void EnableCCTV()
        {
            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                FormSOP.Instance.ToggleCCTV();
            });
        }

        /*public void RunTrainingModeSOPSimulator(int nSensorID, int nEquipZoneID)
        {
            FormSOP.Instance.ShowMonitoringSystem(true);

            FireDetectSignal signal = new FireDetectSignal(nSensorID, 0, nEquipZoneID, DateTime.Now, 0.0f, 0.0f, 0.0f);
            SOPMonitoringSystem.Popup.PopupSensorOn popup = SOPMonitoringSystem.Popup.PopupSensorOn.Instance;

            if (popup.Visible == false)
            {
                SOPMonitoringSystem.Popup.PopupSensorOn.PopUpForm(FormSOP.Instance.DBManager, signal, FormSOP.Instance.HasControl);
            }
        }*/

        public void IgnoreSOP(int nSensorHistoryID)
        {
            SOPMonitoringSystem.Popup.PopupSensorOn popup = SOPMonitoringSystem.Popup.PopupSensorOn.Instance;

            if (popup.Visible == true)
            {
                if (popup.SensorHistoryID == nSensorHistoryID)
                {
                    FormSOP.Instance.Invoke((MethodInvoker)delegate
                    {
                        popup.Visible = false;
                        FormSOP.Instance.NetworkManager.RemoveSensorHistory(nSensorHistoryID);
                        FormSOP.Instance.NetworkManager.ShowDetectSignal();
                    });

                }

            }
        }

        public void CompleteLoading()
        {
            m_loadingSDMS = true;
            
            if (!FormSOP.Instance.OnlySDMS)
                FormSOP.Instance.MainFrame.Visible = true;
        }

        public void OpenSOP_Fire(int nZoneID, DateTime sopTime, int nSensorID, int nHistoryID)
        {
            if (UnE.SOP.ProxySOP.Instance.OpenSOPOnFireDetect == false)
                return;

            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                // SOP Version이 바뀌지 않았는지 검사한다.
                if (!FormSOP.Instance.GetPageHome().CheckSOPVersion())
                    return;

                if (FormSOP.Instance.HasControl == false)
                    return;

                /*if (FormSOP.Instance.IsReal == false)
                {
                    FormSOP.Instance.VirtualMode(false);
                }*/

                Zone zone = DataManager.Instance.GetZone(nZoneID);
                string strSOPFullPath = SOPMonitoringSystem.Popup.PopupSensorOn.GetLinkedSOPName(FormSOP.Instance.DBManager, zone);
                if( strSOPFullPath != null)
                {
                    if (!OpenSOP(sopTime, zone, strSOPFullPath, ID.ID_SOP_FIRE, nSensorID, nHistoryID))
                        PlaySOPonSensorDetect(zone.ID, nSensorID, nHistoryID);
                }
             
            });
        }

        private void PlaySOPonSensorDetect(int nZoneID, int nSensorID, int nHistoryID)
        {  
            if (FormSOP.Instance.SensorDetectLoadAndPlay == true)
            {
                FormSOP.Instance.ShowMonitoringSystem(true);

                if (FormSOP.Instance.HasControl == true)
                {
                    FormSOP.Instance.PlayWithDisasterPosition(nZoneID, nSensorID, nHistoryID);

                    bool isRealMode = true;
                    int nCurrentActionStepID = FormSOP.Instance.ReadCurrentActionStep(ref isRealMode);
                    if (nCurrentActionStepID >= 0)
                    {
                        SOPScenarioManager.Instance.SelectedScenario(nCurrentActionStepID, isRealMode);

                    }
                }
            }
        }

        private delegate void NoSensorDetectOption(UnE.SOP.Workstate.WorkflowOption option, params object[] args);

        private void SetEarthquakeOption(UnE.SOP.Workstate.WorkflowOption option, params object[] args)
        {
            try
            {
                UnE.SOP.Workstate.WorkflowOptionEarthquake earthOption = (UnE.SOP.Workstate.WorkflowOptionEarthquake)option;

                int nIntensity = (int)args[0];
                float fMagnitude = (float)args[1];
                string strPosition = (string)args[2];

                earthOption.Intensity = nIntensity;
                earthOption.Magnitude = fMagnitude;
            }
            catch(Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
        }

        private void PlaySOPonNoSensorDetect(DateTime sopTime, int nHistoryID, NoSensorDetectOption optionMethod, params object[] args)
        {
            FormSOP.Instance.ShowMonitoringSystem(true);

            if (FormSOP.Instance.HasControl == true)
            {
                UnE.SOP.Workstate.WorkflowOption option = FormSOP.Instance.Play(sopTime, nHistoryID);

                if (option != null && optionMethod != null)
                    optionMethod(option, args);

                bool isRealMode = true;
                int nCurrentActionStepID = FormSOP.Instance.ReadCurrentActionStep(ref isRealMode);
                if (nCurrentActionStepID >= 0)
                {
                    SOPScenarioManager.Instance.SelectedScenario(nCurrentActionStepID, isRealMode);

                }
            }
        }

        public void OpenSOP_Security(int nEquipZoneID, DateTime sopTime, int nSensorID, int nHistoryID, int nSensorType)
        {
            if (UnE.SOP.ProxySOP.Instance.OpenSOPOnFireDetect == false)
                return;

            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                // SOP Version이 바뀌지 않았는지 검사한다.
                if (!FormSOP.Instance.GetPageHome().CheckSOPVersion())
                    return;

                if (FormSOP.Instance.HasControl == false)
                    return;  

                EquipmentZone equipZone = DataManager.Instance.GetEquipZone(nEquipZoneID);

                if (equipZone == null || equipZone.LinkedZoneList.Count == 0)
                    return;

                /*if (FormSOP.Instance.IsReal == false)
                {
                    FormSOP.Instance.VirtualMode(false);
                }*/

                Zone zone = (Zone)equipZone.LinkedZoneList[0];

                string strSOPFullPath = SOPMonitoringSystem.Popup.PopupSensorOn.GetSecurityLinkedSOPName(FormSOP.Instance.DBManager, zone, nSensorType);
                if (strSOPFullPath != null)
                {
                    if (!OpenSOP(sopTime, zone, strSOPFullPath, ID.ID_SOP_SECURITY, nSensorID, nHistoryID))
                        PlaySOPonSensorDetect(zone.ID, nSensorID, nHistoryID);
                }
            });
        }


        public void OpenSOP_PSM(int nEquipZoneID, DateTime sopTime,int nSensorID, int nHistoryID)
        {
            if (UnE.SOP.ProxySOP.Instance.OpenSOPOnFireDetect == false)
                return;

            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                // SOP Version이 바뀌지 않았는지 검사한다.
                if (!FormSOP.Instance.GetPageHome().CheckSOPVersion())
                    return;

                if (FormSOP.Instance.HasControl == false)
                    return;


                EquipmentZone equipZone = DataManager.Instance.GetEquipZone(nEquipZoneID);

                if (equipZone == null || equipZone.LinkedZoneList.Count == 0)
                    return;

                /*if (FormSOP.Instance.IsReal == false)
                {
                    FormSOP.Instance.VirtualMode(false);
                }*/

                Zone zone = (Zone)equipZone.LinkedZoneList[0];

                string strSOPFullPath = SOPMonitoringSystem.Popup.PopupSensorOn.GetLinkedSOPName_PSM(FormSOP.Instance.DBManager, equipZone);
                //System.Diagnostics.Trace.WriteLine("EquipZoneID : " + equipZone.ID.ToString() + ", SOP Name : " + strSOPFullPath);
                if (strSOPFullPath != null)
                {
                    if (!OpenSOP(sopTime, zone, strSOPFullPath, ID.ID_SOP_POLLUTION, nSensorID, nHistoryID))
                        PlaySOPonSensorDetect(zone.ID, nSensorID, nHistoryID);
                }
            });
        }

        // strPosition : 진앙지
        public void OpenSOP_Earthquake(string strSOPFullPath, DateTime sopTime, int nHistoryID, int nIntensity, float fMagnitude, string strPosition)
        {
            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                // SOP Version이 바뀌지 않았는지 검사한다.
                if (!FormSOP.Instance.GetPageHome().CheckSOPVersion())
                    return;

                if (FormSOP.Instance.HasControl == false)
                    return;

                if (strSOPFullPath != null)
                {
                    if (!OpenSOP(sopTime, null, strSOPFullPath, ID.ID_SOP_EARTHQUAKE, -1, nHistoryID))
                        PlaySOPonNoSensorDetect(sopTime, nHistoryID, SetEarthquakeOption, nIntensity, fMagnitude, strPosition);
                }
            });
        }

        private bool OpenSOP(DateTime sopTime, Zone zone, string strSOPFullPath, int nDefaultSOPID, int nSensroID, int nSensroZoneHistoryID)
        {
            // 외부에서 사용중인 SOP의 Path 구분자는 '/' 이다
            if (strSOPFullPath == null || strSOPFullPath.Length == 0)
                strSOPFullPath = FormSOP.Instance.GetPageHome().GetQuickSOPFullPath(nDefaultSOPID);

            // 내부에 저장된 SOP Path의 구분자는 0x06이다
            string cmpSOPPath = strSOPFullPath.Replace('/', (char)0x06);

            // 모든로도되
            BarLevelTree tree = SOPScenarioManager.Instance.GetBarLevelTree();

            // 현재 정규모드인지 여부
            bool bIsNormal = tree.IsNormal;
            // 정규/비정규 전환여부
            bool bChangedTree = false;
            // 실행중인 SOP가 있는경우
            bool bFindRunSOP = false;

            // 전체 실행중인 SOP시나리오를 가져온다.
            System.Collections.ArrayList arScList = SOPScenarioManager.Instance.GetAllScenario();
            if (arScList != null && arScList.Count > 0)
            {

                foreach (UnE.SOP.Workstate.SOPScenario sc in arScList)
                {
                    String szCmp = sc.ActionStepFullPath;

                    // 현재 요청한 SOP가 실행중인지 여부를 검사
                    if (szCmp.StartsWith(strSOPFullPath) || szCmp.StartsWith(cmpSOPPath))
                    {
                        bFindRunSOP = true;
                        // 현재 실행중인 시나리오는 SelectedScenario로 선택한다.( 내부적으로 Tree와 TabPage처리가 된다.)
                        SOPScenarioManager.Instance.SelectedScenario(sc.ActionStepID, sc.RealMode);
                        break;
                    }
                }
            }

            // 요청한 SOP가 실행중이 아니면
            if (bFindRunSOP == false)
            {
                // SOP를 로드한다. ( SOP는 SOPTreeNode로 요청한다. )
                TreeNode node = LoadSOPTreeNode(tree, strSOPFullPath, true, tree.IsNormal, false);
                if (node == null)
                {
                    // 해당하는 정규모드의 SOP가 없는경우는 비정규 SOP로드를 시도한다.
                    node = LoadSOPTreeNode(tree, strSOPFullPath, true, !tree.IsNormal);
                    // 비정규모드로 전환
                    bChangedTree = true;
                }

                // 로드된 SOPNode가 있다면, (DB에서 로드되었음)
                if (node != null)
                {
                    //System.Diagnostics.Trace.WriteLine("node text : " + node.Text);

                    // SOP 선택하여 를 화면에 표시한다.
                    tree.SelectNode(node);

                    // 비정규모드로 전환하는 경우,  Tree전체를 갱신해준다.
                    if (bChangedTree == true)
                        tree.Load(FormSOP.Instance.SOPManager, true, bIsNormal);

                    // 최근에 로드된 SOP의 TabPage는 반드시 SelectedTab에 존재한다.
                    UnE.SOP.Sections.SectionTabPage tabPage = (UnE.SOP.Sections.SectionTabPage)FormSOP.Instance.GetPageHome().TabControls.SelectedTab;
                    if (tabPage != null)
                    {
                        // TabPage에 실행정보를 저장한다.
                        if (zone != null)
                        {
                            tabPage.LinkedZoneName = zone.BroadcastName;
                            tabPage.LinkedZoneID = zone.ID;
                        }

                        tabPage.LinkedTime = sopTime;

                        // Sensor로부터 로딩된 SOP는 SensorZoneHistoryID와 SensorID를 넣어준다.
                        tabPage.SensorZoneHistoryID = nSensroZoneHistoryID;
                        tabPage.SensorID = nSensroID;
                    }
                }
            }

            return bFindRunSOP;
        }

        /*public void OpenSOP(int nZoneID, DateTime sopTime)
        {
            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                // SOP Version이 바뀌지 않았는지 검사한다.
                if (!FormSOP.Instance.GetPageHome().CheckSOPVersion())
                    return;

                Zone zone = DataManager.Instance.GetZone(nZoneID);
                string strSOPFullPath = SOPMonitoringSystem.Popup.PopupSensorOn.GetLinkedSOPName(FormSOP.Instance.DBManager, zone);

                if (strSOPFullPath == null || strSOPFullPath.Length == 0)
                    strSOPFullPath = FormSOP.Instance.GetPageHome().GetQuickSOPFullPath(ID.ID_SOP_FIRE);

                BarLevelTree tree = SOPScenarioManager.Instance.GetBarLevelTree();

                bool bIsNormal = tree.IsNormal;
                bool bChangedTree = false;
                TreeNode node = LoadSOPTreeNode(tree, strSOPFullPath, true, tree.IsNormal, false);
                if (node == null)
                {
                    node = LoadSOPTreeNode(tree, strSOPFullPath, true, !tree.IsNormal);
                    bChangedTree = true;
                }

                if (node != null)
                {
                    tree.SelectNode(node);

                    if (bChangedTree == true)
                        tree.Load(FormSOP.Instance.SOPManager, true, bIsNormal);                   

                    UnE.SOP.Sections.SectionTabPage tabPage = (UnE.SOP.Sections.SectionTabPage)FormSOP.Instance.GetPageHome().TabControls.SelectedTab;

                    if (tabPage != null)
                    {
                        tabPage.LinkedZoneName = zone.BroadcastName;
                        tabPage.LinkedTime = sopTime;
                        tabPage.LinkedZoneID = nZoneID;
                    }
                }
            });
        }*/

        private TreeNode LoadSOPTreeNode(BarLevelTree tree, string strSOPFullPath, bool isRegular, bool isNormal, bool bLoadTree = true)
        {
            if (bLoadTree == true)
            {
                if (!tree.Load(FormSOP.Instance.SOPManager, isRegular, isNormal))
                    return null;

            }
           
            string[] strTokens = strSOPFullPath.Split('/');
            int nTokenCount = strTokens.Count();

            TreeNodeCollection nodes = null;
            TreeNode node = null;

            for (int i = 0; i < nTokenCount; i++)
            {
                node = tree.FindNode(strTokens[i], nodes);

                if (node == null)
                    return null;
                else
                    nodes = node.Nodes;
            }

            return node;
        }

        public void OnAfterLoadingCCTV()
        {
            /*if (FormSOP.Instance.Visible == true)
                if (FormSOP.Instance.GetAllSenario().Count == 0)
                {
                    FormSOP.Instance.SelectCCTVTab();
                }*/
                
        }
        /*** 같은 SensorZoneGroup을 가진 센서가 아직 동작중인지 아직 해제 되지 않았는지 체크 하도록 
         * 다음과 같은 함수를 internal message로 부터 받아서 등록하고 체크하도록 한다. 
         * by hypark, 2018.7.19 *****/
        public void RegisterSameSensorGroupRunning(int nSensorZoneID, int nSensorZoneHistoryID)
        {
            try
            {
                SupervisorSOPClose.RegisterSameSensorGroupRunning(nSensorZoneID, nSensorZoneHistoryID);
                //SDMS.ScriptProxy.Instance.UserObject.SupervisorSOPSensorClose.Invoke(nSensorZoneID);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
            }      
        }
        public void SensorClose(int nSensorZoneID, int nSensorZoneHistoryID)
        {
            try
            {
                SupervisorSOPClose.SupervisorSOPSensorClose(nSensorZoneID, nSensorZoneHistoryID);
                //SDMS.ScriptProxy.Instance.UserObject.SupervisorSOPSensorClose.Invoke(nSensorZoneID);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
            }      
        }

        public void ToggleSOPBulletin()
        {
            FormSOP.Instance.ToggleSOPBulletin();
        }

        public void AddLastHistoryDisasterPosition(string strDisasterName, string strPositionName, string strBroadcastName, string strBuildingID, float fFloorIndex, int nActionStepHistoryID, int nIconID, int nPSMDistance, string strPSMMaterial, float x, float y, float z, int nZoneID)
        {
            if (m_workFlowStartOption == null)
                return;

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

            Form form = m_workFlowStartOption.GetInvokeForm();

            form.Invoke((MethodInvoker)delegate
            {
                m_workFlowStartOption.AddLastHistoryDisasterPoistion(pos);
            });
        }

        public void SetSOPPositionName(string strPositionName)
        {
            if (m_workFlowStartOption == null)
                return;

            Form form = m_workFlowStartOption.GetInvokeForm();

            form.Invoke((MethodInvoker)delegate
            {
                m_workFlowStartOption.PositionName = strPositionName;
            });
        }

        public void SetWorkFlowOptionPosition(string strPositionName)
        {
            if (m_workFlowStartOption == null)
                return;

            Form form = m_workFlowStartOption.GetInvokeForm();

            form.Invoke((MethodInvoker)delegate
            {
                m_workFlowStartOption.PositionName = strPositionName;
            });
        }
    }
}
