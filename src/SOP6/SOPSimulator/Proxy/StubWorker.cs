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
        // 알람에 대한 SOP 처리여부
        // SOP의 진행상태에 대한 정보는 없고, 알람에 대하여 SOP가 실행되었는가 여부를 나타낸다.
        // None : 아직 결정되지 않았다.
        // Run : SOP가 실행되었다.
        // Ignore : 알람에 대하여 SOP를 실행시키지 않기로 하였다.
        public enum SOPProcessType { None, Run, Igonore };

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
                        NetworkWebManager.Instance.RemoveSensorHistory(nSensorHistoryID);
                        //NetworkWebManager.Instance.ShowDetectSignal();
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

        public SOPProcessType OpenSOP_Fire(int nZoneID, DateTime sopTime, int nSensorID, int nHistoryID)
        {
            if (UnE.SOP.ProxySOP.Instance.OpenSOPOnSensorDetect == false)
                return SOPProcessType.Igonore;

            SOPProcessType processType = SOPProcessType.None;

            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                processType = _OpenSOP_Fire(nZoneID, sopTime, nSensorID, nHistoryID);
            });

            return processType;
        }

        private SOPProcessType _OpenSOP_Fire(int nZoneID, DateTime sopTime, int nSensorID, int nHistoryID, string strAlarmMessage = null)
        {
            // SOP Version이 바뀌지 않았는지 검사한다.
            if (!FormSOP.Instance.GetPageHome().CheckSOPVersion())
                return SOPProcessType.None;

            if (FormSOP.Instance.HasControl == false)
                return SOPProcessType.None;

            /*if (FormSOP.Instance.IsReal == false)
            {
                FormSOP.Instance.VirtualMode(false);
            }*/

            Zone zone = DataManager.Instance.GetZone(nZoneID);
            string strSOPFullPath = SOPMonitoringSystem.Popup.PopupSensorOn.GetLinkedSOPName(FormSOP.Instance.DBManager, zone);
            if (strSOPFullPath != null)
            {
                if (!OpenSOP(sopTime, zone, strSOPFullPath, ID.ID_SOP_FIRE, nSensorID, nHistoryID))
                {
                    PlaySOPonSensorDetect(sopTime, zone.ID, nSensorID, nHistoryID, strAlarmMessage);
                    return SOPProcessType.Run;
                }
                else
                    return SOPProcessType.Igonore;
            }

            return SOPProcessType.Igonore;
        }

        private void PlaySOPonSensorDetect(DateTime sopTime, int nZoneID, int nSensorID, int nHistoryID, string strAlarmMessage = null)
        {  
            if (FormSOP.Instance.SensorDetectLoadAndPlay == true)
            {
                FormSOP.Instance.ShowMonitoringSystem(true);

                if (FormSOP.Instance.HasControl == true)
                {
                    bool isRealMode = !FormSOP.Instance.VirtualModeInSensor;
                    FormSOP.Instance.PlayWithDisasterPosition(sopTime, nZoneID, nSensorID, nHistoryID, isRealMode, strAlarmMessage);

                    //bool isRealMode = true;
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

        public SOPProcessType OpenSOP_Security(int nEquipZoneID, DateTime sopTime, int nSensorID, int nHistoryID, int nSensorType)
        {
            if (UnE.SOP.ProxySOP.Instance.OpenSOPOnSensorDetect == false)
                return SOPProcessType.Igonore;

            SOPProcessType processType = SOPProcessType.None;

            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                processType = _OpenSOP_Security(nEquipZoneID, sopTime, nSensorID, nHistoryID, nSensorType);
            });

            return processType;
        }

        private SOPProcessType _OpenSOP_Security(int nEquipZoneID, DateTime sopTime, int nSensorID, int nHistoryID, int nSensorType, string strAlarmMessage = null)
        {
            // SOP Version이 바뀌지 않았는지 검사한다.
            if (!FormSOP.Instance.GetPageHome().CheckSOPVersion())
                return SOPProcessType.None;

            if (FormSOP.Instance.HasControl == false)
                return SOPProcessType.None;

            EquipmentZone equipZone = DataManager.Instance.GetEquipZone(nEquipZoneID);

            if (equipZone == null || equipZone.LinkedZoneList.Count == 0)
                return SOPProcessType.Igonore;

            /*if (FormSOP.Instance.IsReal == false)
            {
                FormSOP.Instance.VirtualMode(false);
            }*/

            Zone zone = (Zone)equipZone.LinkedZoneList[0];

            string strSOPFullPath = SOPMonitoringSystem.Popup.PopupSensorOn.GetSecurityLinkedSOPName(FormSOP.Instance.DBManager, zone, nSensorType);
            if (strSOPFullPath != null)
            {
                if (!OpenSOP(sopTime, zone, strSOPFullPath, ID.ID_SOP_SECURITY, nSensorID, nHistoryID))
                {
                    PlaySOPonSensorDetect(sopTime, zone.ID, nSensorID, nHistoryID, strAlarmMessage);
                    return SOPProcessType.Run;
                }
                else
                    return SOPProcessType.Igonore;
            }

            return SOPProcessType.Igonore;
        }

        public SOPProcessType OpenSOP_PSM(int nEquipZoneID, DateTime sopTime,int nSensorID, int nHistoryID)
        {
            if (UnE.SOP.ProxySOP.Instance.OpenSOPOnSensorDetect == false)
                return SOPProcessType.Igonore;

            SOPProcessType processType = SOPProcessType.None;

            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                processType = _OpenSOP_PSM(nEquipZoneID, sopTime, nSensorID, nHistoryID);
            });

            return processType;
        }

        private SOPProcessType _OpenSOP_PSM(int nEquipZoneID, DateTime sopTime, int nSensorID, int nHistoryID, string strAlarmMessage = null)
        {
            // SOP Version이 바뀌지 않았는지 검사한다.
            if (!FormSOP.Instance.GetPageHome().CheckSOPVersion())
                return SOPProcessType.None;

            if (FormSOP.Instance.HasControl == false)
                return SOPProcessType.None;


            EquipmentZone equipZone = DataManager.Instance.GetEquipZone(nEquipZoneID);

            if (equipZone == null || equipZone.LinkedZoneList.Count == 0)
                return SOPProcessType.Igonore;

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
                {
                    PlaySOPonSensorDetect(sopTime, zone.ID, nSensorID, nHistoryID, strAlarmMessage);
                    return SOPProcessType.Run;
                }
                else
                    return SOPProcessType.Igonore;
            }

            return SOPProcessType.Igonore;
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

            // 현재 평일모드인지 여부
            bool bIsNormal = Popup.SOPLoader.IsDayLight_NoInvoke(sopTime);//tree.IsNormal;
            // 평일/휴일 전환여부
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
                    if ((szCmp.StartsWith(strSOPFullPath) || szCmp.StartsWith(cmpSOPPath)) && sc.NormalMode == bIsNormal)
                    {
                        bFindRunSOP = true;
                        // 현재 실행중인 시나리오는 SelectedScenario로 선택한다.( 내부적으로 Tree와 TabPage처리가 된다.)
                        SOPScenarioManager.Instance.SelectedScenario(sc.ActionStepID, sc.RealMode);
                        break;
                    }
                }
            }

            bool isNormal = bIsNormal;

            // 요청한 SOP가 실행중이 아니면
            if (bFindRunSOP == false)
            {
                // SOP를 로드한다. ( SOP는 SOPTreeNode로 요청한다. )
                TreeNode node = LoadSOPTreeNode(tree, strSOPFullPath, true, bIsNormal/*tree.IsNormal*/, bIsNormal != tree.IsNormal);
                if (node == null)
                {
                    // 해당하는 평일모드의 SOP가 없는경우는 휴일 SOP로드를 시도한다.
                    node = LoadSOPTreeNode(tree, strSOPFullPath, true, !bIsNormal/*!tree.IsNormal*/);
                    // 모드 전환
                    bChangedTree = bIsNormal == tree.IsNormal;
                    //bChangedTree = true;
                    isNormal = !bIsNormal;
                }
                else
                {
                    // 모드 전환
                    bChangedTree = bIsNormal != tree.IsNormal;
                }

                // 로드된 SOPNode가 있다면, (DB에서 로드되었음)
                if (node != null)
                {
                    //System.Diagnostics.Trace.WriteLine("node text : " + node.Text);

                    // SOP 선택하여 를 화면에 표시한다.
                    tree.SelectNode(node);

                    // 모드가 바뀐 경우,  Tree전체를 갱신해준다.
                    if (bChangedTree == true)
                        tree.Load(FormSOP.Instance.SOPManager, true, isNormal/*bIsNormal*/);

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
