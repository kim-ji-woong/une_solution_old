using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using libSOPPolicy;
using System.Collections;
using DBUtility2;
using UnE.Sensor;
using System.Collections.Concurrent;
using System.Threading;

namespace SOPMonitoringSystem
{
    public class StubWorker// : SDMS.IProxyMessenser
    {
        // 센서신호를 받아서 SOP를 실행시키기 위한 데이터
        // 센서신호를 받으면 SOP 실행권한을 서버로부터 받아야 하는데, 그때까지 SOP 데이터를 보관하도록 한다.
        // Key : SensorZoneHistoryID
        private ConcurrentDictionary<int, DateTime> m_dicSensorSOPData = new ConcurrentDictionary<int, DateTime>();

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
                FormSOP.Instance.Visible = true;
        }

        public SOPProcessType OpenSOP_Fire(int nZoneID, DateTime sopTime, int nSensorID, int nHistoryID, int nSOPGenUserID, string strSOPFullPath, string strAlarmMessage)
        {
            if (UnE.SOP.ProxySOP.Instance.OpenSOPOnSensorDetect == false)
                return SOPProcessType.Igonore;

            SOPProcessType processType = SOPProcessType.None;
            BaseSOPUser sopUser = FormSOP.Instance.SOPUser;

            if (sopUser != null)
            {
                if (sopUser.ID == nSOPGenUserID)
                {
                    FormSOP.Instance.Invoke((MethodInvoker)delegate
                    {
                        processType = _OpenSOP_Fire(nZoneID, sopTime, nSensorID, nHistoryID, strSOPFullPath, strAlarmMessage);
                    });

                    return processType;
                }
            }

            return processType;
        }

        public SOPProcessType OpenSOP_Fire(int nZoneID, DateTime sopTime, int nSensorID, int nHistoryID, int nSOPGenUserID)
        {
            if (UnE.SOP.ProxySOP.Instance.OpenSOPOnSensorDetect == false)
                return SOPProcessType.Igonore;

            SOPProcessType processType = SOPProcessType.None;
            BaseSOPUser sopUser = FormSOP.Instance.SOPUser;

            if (sopUser != null)
            {
                if (nSOPGenUserID < 0)
                {
                    // 실행권한이 있는지 확인한다.
                    if (sopUser.AbletoAccess(nSensorID, FormSOP.Instance.DBManager.SiteID, FormSOP.Instance.DBManager) == false)
                    {
                        return SOPProcessType.None;
                    }
                    else
                    {
                        if (nHistoryID > 0)
                        {
                            if (m_dicSensorSOPData.ContainsKey(nHistoryID) == false)
                            {
                                m_dicSensorSOPData[nHistoryID] = sopTime;
                                SendRequestSensorSOP(nHistoryID);
                            }

                            return SOPProcessType.None;
                        }
                        else
                        {
                            FormSOP.Instance.Invoke((MethodInvoker)delegate
                            {
                                processType = _OpenSOP_Fire(nZoneID, sopTime, nSensorID, nHistoryID);
                            });

                            return processType;
                        }
                    }
                }
                else if (sopUser.ID == nSOPGenUserID)
                {
                    FormSOP.Instance.Invoke((MethodInvoker)delegate
                    {
                        processType = _OpenSOP_Fire(nZoneID, sopTime, nSensorID, nHistoryID);
                    });

                    return processType;
                }
            }

            return processType;
        }

        public SOPProcessType OpenReportSOP_Fire(int nDisasterID, int nZoneID, DateTime sopTime, int nSensorID, int nHistoryID)
        {
            if (UnE.SOP.ProxySOP.Instance.OpenSOPOnSensorDetect == false)
                return SOPProcessType.Igonore;

            SOPProcessType processType = SOPProcessType.None;
            
            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                processType = _OpenReportSOP_Fire(nDisasterID, nZoneID, sopTime, nSensorID, nHistoryID);
            });

            return processType;
        }

        /*public void SensorSOPPermit(Dictionary<int, StubWorker.SOPProcessType> dicSOPProcessTypes, int nSensorZoneHistoryID, int nSOPGenUserID)
        {
            SensorSOPStartData sopData;

            if (m_dicSensorSOPData.TryGetValue(nSensorZoneHistoryID, out sopData))
            {
                sopData.Processed = true;

                if (FormSOP.Instance.SOPUser.ID == nSOPGenUserID)
                {
                    SOPProcessType processType = SOPProcessType.None;

                    if (sopData is SensorSOPFire)
                    {
                        FormSOP.Instance.Invoke((MethodInvoker)delegate
                        {
                            SensorSOPFire sopFire = (SensorSOPFire)sopData;
                            processType = _OpenSOP_Fire(sopFire.ZoneID, sopFire.DetectTime, sopFire.SensorZoneID, sopFire.SensorZoneHistoryID);
                        });
                    }
                    else if (sopData is SensorSOPPSM)
                    {
                        FormSOP.Instance.Invoke((MethodInvoker)delegate
                        {
                            SensorSOPPSM sopPSM = (SensorSOPPSM)sopData;
                            processType = _OpenSOP_PSM(sopPSM.EquipZoneID, sopPSM.DetectTime, sopPSM.SensorZoneID, sopPSM.SensorZoneHistoryID);
                        });
                    }
                    else if (sopData is SensorSOPSecurity)
                    {
                        FormSOP.Instance.Invoke((MethodInvoker)delegate
                        {
                            SensorSOPSecurity sopSecurity = (SensorSOPSecurity)sopData;
                            processType = _OpenSOP_Security(sopSecurity.EquipZoneID, sopSecurity.DetectTime, sopSecurity.SensorZoneID, sopSecurity.SensorZoneHistoryID, sopSecurity.SensorType);
                        });
                    }
                    else if (sopData is SensorSOPEtc)
                    {
                        FormSOP.Instance.Invoke((MethodInvoker)delegate
                        {
                            SensorSOPEtc sopEtc = (SensorSOPEtc)sopData;
                            processType = _OpenSOP_ETC(sopEtc.ZoneID, sopEtc.EquipZoneID, sopEtc.DetectTime, sopEtc.SensorZoneID, sopEtc.SensorZoneHistoryID, sopEtc.SensorType);
                        });
                    }
                    else if (sopData is SensorSOPEarthquake)
                    {
                        FormSOP.Instance.Invoke((MethodInvoker)delegate
                        {
                            SensorSOPEarthquake sop = (SensorSOPEarthquake)sopData;

                            // SOP Version이 바뀌지 않았는지 검사한다.
                            if (!FormSOP.Instance.GetPageHome().CheckSOPVersion())
                                processType = SOPProcessType.None;
                            else
                            {
                                string strSOPFullPath = sop.SOPFullPath;

                                if (strSOPFullPath != null)
                                {
                                    if (!OpenSOP(sop.DetectTime, null, ref strSOPFullPath, ID.ID_SOP_EARTHQUAKE, sop.SensorZoneID, sop.SensorZoneHistoryID))
                                    {
                                        PlaySOPonNoSensorDetect(sop.DetectTime, sop.SensorZoneID, sop.SensorZoneHistoryID, SetEarthquakeOption, strSOPFullPath, sop.Intensity, sop.Magnitude, sop.Position);
                                        processType = SOPProcessType.Run;
                                    }
                                    else
                                        processType = SOPProcessType.Igonore;
                                }
                            }
                        });
                    }

                    if (processType != SOPProcessType.None)
                    {
                        dicSOPProcessTypes[nSensorZoneHistoryID] = processType;
                    }
                }
            }
        }*/

        private void SendRequestSensorSOP(int nSensorZoneHistoryID)
        {
            // WCF에 의한 동기화 문제를 피하기 위하여 Thread로 처리한다.
            Thread t = new Thread(new ParameterizedThreadStart(SendRequestSensorSOPThread));
            t.Start(nSensorZoneHistoryID);
        }

        private void SendRequestSensorSOPThread(object arg)
        {
            if (arg != null && arg is int)
            {
                int nSensorZoneHistoryID = (int)arg;

                ArrayList arrDatas = new ArrayList();
                arrDatas.Add(nSensorZoneHistoryID);

                byte[] bytes = SOPWebServer.BinaryHelper.MakeBytes(arrDatas);
                NetworkWebManager.Instance.SendMessage(SOPWebServer.Header.REQUEST_SENSOR_SOP_PERMIT, bytes);
            }
        }

        private SOPProcessType _OpenSOP_Fire(int nZoneID, DateTime sopTime, int nSensorID, int nHistoryID, string strSOPFullPath = null, string strAlarmMessage = null)
        {
            // SOP Version이 바뀌지 않았는지 검사한다.
            if (!FormSOP.Instance.GetPageHome().CheckSOPVersion())
                return SOPProcessType.None;

            Zone zone = DataManager.Instance.GetZone(nZoneID);

            if (strSOPFullPath == null)
                strSOPFullPath = SOPMonitoringSystem.Popup.PopupSensorOn.GetLinkedSOPName(FormSOP.Instance.DBManager, zone);

            if (strSOPFullPath != null)
            {
                UnE.SOP.Sections.SectionTabPage tabPage = OpenSOP(sopTime, zone, ref strSOPFullPath, ID.ID_SOP_FIRE, nSensorID, nHistoryID);

                // sensorData는 어떻게 전달?
                if (tabPage != null && tabPage.ActionStepHistoryID <= 0)
                //if (!OpenSOP(sopTime, zone, ref strSOPFullPath, ID.ID_SOP_FIRE, nSensorID, nHistoryID))
                {
                    // Server로부터 실행권한을 부여받았으므로 TabPage를 실행가능 상태로 만든다.
                    FormSOP.Instance.SetPageMode(tabPage);

                    if (PlaySOPonSensorDetect(tabPage, sopTime, zone.ID, nSensorID, nHistoryID, "", null, strAlarmMessage))
                        return SOPProcessType.Run;
                    else
                        return SOPProcessType.None;
                }
                else
                    return SOPProcessType.Igonore;
            }

            return SOPProcessType.Igonore;
        }

        private SOPProcessType _OpenReportSOP_Fire(int nDisasterID, int nZoneID, DateTime sopTime, int nSensorID, int nHistoryID)
        {
            Zone zone = DataManager.Instance.GetZone(nZoneID);
            string strSOPFullPath = GetSOPFullPath(nDisasterID, "화재", FormSOP.Instance.DBManager);
            if (strSOPFullPath != null)
            {
                UnE.SOP.Sections.SectionTabPage tabPage = OpenSOP(sopTime, zone, ref strSOPFullPath, ID.ID_SOP_FIRE, nSensorID, nHistoryID);

                // sensorData는 어떻게 전달?
                if (tabPage != null && tabPage.ActionStepHistoryID <= 0)
                {
                    // Server로부터 실행권한을 부여받았으므로 TabPage를 실행가능 상태로 만든다.
                    FormSOP.Instance.SetPageMode(tabPage);

                    if (PlaySOPonSensorDetect(tabPage, sopTime, zone.ID, nSensorID, nHistoryID))
                        return SOPProcessType.Run;
                    else
                        return SOPProcessType.None;
                }
                else
                    return SOPProcessType.Igonore;
            }

            return SOPProcessType.Igonore;
        }

        private string GetSOPFullPath(int nDisasterID, string strCategoryName, WebDBManager dbMgr)
        {
            string strSQL = "Select d.DisasterName, sdc.SubCategoryName, dc.CategoryName ";
            strSQL += "from Disaster as d, SubDisasterCategory as sdc, DisasterCategory as dc ";
            strSQL += "where d.SubDisasterID = sdc.ID and sdc.DisasterID = dc.ID ";
            strSQL += string.Format("and d.ID = {0} and dc.CategoryName like '%{1}%'", nDisasterID, strCategoryName);

            ArrayList arrResult = dbMgr.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count < 3)
                return null;

            string strDisasterName = WebDBManager.GetStringField(arrResult[0]);
            string strSubCategoryName = WebDBManager.GetStringField(arrResult[1]);
            string categoryName = WebDBManager.GetStringField(arrResult[2]);

            if (strDisasterName == null || strSubCategoryName == null || categoryName == null)
                return null;

            return categoryName + "/" + strSubCategoryName + "/" + strDisasterName;
        }

        private bool PlaySOPonSensorDetect(UnE.SOP.Sections.SectionTabPage page, DateTime sopTime, int nZoneID, int nSensorID, int nHistoryID, string strDisasterOption = "", VariousData<bool> isRealModeSOP = null, string strAlarmMessage = null)
        {  
            if (FormSOP.Instance.SensorDetectLoadAndPlay == true)
            {
                FormSOP.Instance.ShowMonitoringSystem(true);

                // 센서에 의한 알람이 발생하면 SOPWebServer는 하나의 SOPSimulator Client에게만 Sensor 신호를 보낸다.
                // 그 신호를 받은 Client는 SOP를 실행한다.
                //if (FormSOP.Instance.HasControl == true)
                {
                    bool isRealMode = isRealModeSOP == null ? !FormSOP.Instance.VirtualModeInSensor : isRealModeSOP.Data;
                    int nCurrentActionStepID = FormSOP.Instance.PlayWithDisasterPosition(page, sopTime, nZoneID, nSensorID, nHistoryID, strDisasterOption, isRealMode, strAlarmMessage);

                    //bool isRealMode = true;
                    //int nCurrentActionStepID = FormSOP.Instance.ReadCurrentActionStep(ref isRealMode);
                    if (nCurrentActionStepID >= 0)
                    {
                        SOPScenarioManager.Instance.SelectedScenario(nCurrentActionStepID, isRealMode);
                        return true;
                    }
                }
            }

            return false;
        }

        private bool PlaySOP(UnE.SOP.Sections.SectionTabPage page, DateTime sopTime, string strSOPFullPath, Dictionary<string, string> dicParameters, Dictionary<UnE.SOP.SOPParameter, string> dicUserDefinedParameters)
        {
            FormSOP.Instance.ShowMonitoringSystem(true);
            UnE.SOP.Workstate.WorkflowOption option = MakeWorkFlowOption(strSOPFullPath, sopTime, dicParameters, dicUserDefinedParameters);

            if (FormSOP.Instance.RunWorkflow(page, option, strSOPFullPath) != null)
                return true;
            
            return false;
        }

        private UnE.SOP.Workstate.WorkflowOption MakeWorkFlowOption(string strSOPFullPath, DateTime sopTime, Dictionary<string, string> dicParameters, Dictionary<UnE.SOP.SOPParameter, string> dicUserDefinedParameters)
        {
            string[] tokens = strSOPFullPath.Split('/');

            if (tokens.Count() < 3)
                return new UnE.SOP.Workstate.WorkflowOption();

            string strDisasterCategoryName = tokens[0].Trim().ToLower();
            string strSubDisasterCategoryName = tokens[1].Trim().ToLower();
            UnE.SOP.Workstate.WorkflowOption _option = null;

            if (strDisasterCategoryName.Contains("누출") ||
                strDisasterCategoryName.Contains("유출") ||
                strDisasterCategoryName.Contains("오염"))
            {
                UnE.SOP.Workstate.WorkflowOptionPSM option = new UnE.SOP.Workstate.WorkflowOptionPSM();
                _option = option;

                string strPSMMaterial;

                if (dicParameters.TryGetValue("{psmmaterial}", out strPSMMaterial))
                {
                    UnE.PSM.PSMMaterial material = GetPSMMaterialFromDB(strPSMMaterial);
                }
            }
            else if (strDisasterCategoryName.Contains("지진") ||
                strSubDisasterCategoryName.Contains("지진"))
            {
                UnE.SOP.Workstate.WorkflowOptionEarthquake option = new UnE.SOP.Workstate.WorkflowOptionEarthquake();
                _option = option;

                string strMagnit, strIntens, strEpic;

                if (dicParameters.TryGetValue("{earthq_magnit}", out strMagnit))
                {
                    float fMagnit;

                    if (float.TryParse(strMagnit, out fMagnit))
                    {
                        option.Magnitude = fMagnit;
                        option.Mode = UnE.SOP.Workstate.WorkflowOptionEarthquake.PowerMode.Magnitude;
                    }
                }

                if (dicParameters.TryGetValue("{earthq_intens}", out strIntens))
                {
                    int nIntens;

                    if (int.TryParse(strIntens, out nIntens))
                    {
                        option.Intensity = nIntens;
                        option.Mode = UnE.SOP.Workstate.WorkflowOptionEarthquake.PowerMode.Intensity;
                    }
                }

                if (dicParameters.TryGetValue("{earthq_epicenter}", out strEpic))
                {
                    option.PositionName = strEpic;
                }
            }
            else
            {
                _option = new UnE.SOP.Workstate.WorkflowOption();
            }

            string strPosition;

            if (dicParameters.TryGetValue("{location}", out strPosition))
            {
                _option.PositionName = strPosition;
            }

            if (_option.PositionName != null && _option.PositionName.Length > 0)
                _option.HasPosition = true;

            _option.DetectTime = new VariousData<DateTime>(sopTime);

            foreach (KeyValuePair<UnE.SOP.SOPParameter, string> pair in dicUserDefinedParameters)
            {
                _option.UserDefinedParameters[pair.Key] = pair.Value;
            }

            return _option;
        }

        private UnE.PSM.PSMMaterial GetPSMMaterialFromDB(string strPSMMaterial)
        {
            string strSQL = "Select ID, UOM, EvacInitDistance, EvacDayDistance, EvacNightDistance from PSMMaterial where MaterialName = '" + strPSMMaterial + "'";
            ArrayList arrResult = FormSOP.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count < 5)
                return null;

            VariousData<int> id = WebDBManager.GetIntField(arrResult[0].ToString());
            string strUOM = WebDBManager.GetStringField(arrResult[1]);
            VariousData<int> initDistance = WebDBManager.GetIntField(arrResult[2].ToString());
            VariousData<int> dayDistance = WebDBManager.GetIntField(arrResult[3].ToString());
            VariousData<int> nightDistance = WebDBManager.GetIntField(arrResult[4].ToString());

            if (id == null || strUOM == null)
                return null;

            UnE.PSM.PSMMaterial material = new UnE.PSM.PSMMaterial();
            material.ID = id.Data;
            material.Name = strPSMMaterial;
            material.UOM = strUOM;

            if (initDistance != null)
                material.InitEvacDistance = initDistance.Data;

            if (dayDistance != null)
                material.DayEvacDistance = dayDistance.Data;

            if (nightDistance != null)
                material.NightEvacDistance = nightDistance.Data;

            return material;
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

                if (nIntensity > 0)
                    earthOption.Mode = UnE.SOP.Workstate.WorkflowOptionEarthquake.PowerMode.Intensity;
                else if (fMagnitude > 0.0f)
                    earthOption.Mode = UnE.SOP.Workstate.WorkflowOptionEarthquake.PowerMode.Magnitude;

                earthOption.Intensity = nIntensity;
                earthOption.Magnitude = fMagnitude;
            }
            catch(Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
        }

        private void SetStrongWindOption(UnE.SOP.Workstate.WorkflowOption option, params object[] args)
        {
            try
            {
                UnE.SOP.Workstate.WorkflowOptionWind windOption = (UnE.SOP.Workstate.WorkflowOptionWind)option;

                float fWindSpeed = (float)args[0];
                string strPosition = (string)args[1];

                windOption.WindSpeed = fWindSpeed;

                if (strPosition.Length > 0)
                {
                    windOption.HasPosition = true;
                    windOption.PositionName = strPosition;
                }
                else
                    windOption.HasPosition = false;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
            }
        }

        private void PlaySOPonNoSensorDetect(DateTime sopTime, int nSensorZoneID, int nHistoryID, NoSensorDetectOption optionMethod, string strSOPFullPath, params object[] args)
        {
            FormSOP.Instance.ShowMonitoringSystem(true);

            // 센서에 의한 알람이 발생하면 SOPWebServer는 하나의 SOPSimulator Client에게만 Sensor 신호를 보낸다.
            // 그 신호를 받은 Client는 SOP를 실행한다.
            //if (FormSOP.Instance.HasControl == true)
            {
                int nCurrentActionStepID;
                bool isRealMode = !FormSOP.Instance.VirtualModeInSensor;
                UnE.SOP.Workstate.WorkflowOption option = FormSOP.Instance.Play(sopTime, nSensorZoneID, nHistoryID, strSOPFullPath, out nCurrentActionStepID, isRealMode);

                if (option != null && optionMethod != null)
                    optionMethod(option, args);

                //bool isRealMode = true;
                //int nCurrentActionStepID = FormSOP.Instance.ReadCurrentActionStep(ref isRealMode);
                if (nCurrentActionStepID >= 0)
                {
                    SOPScenarioManager.Instance.SelectedScenario(nCurrentActionStepID, isRealMode);

                }
            }
        }

        public SOPProcessType OpenSOP_Security(int nEquipZoneID, DateTime sopTime, int nSensorID, int nHistoryID, int nSensorType, int nSOPGenUserID)
        {
            if (UnE.SOP.ProxySOP.Instance.OpenSOPOnSensorDetect == false)
                return SOPProcessType.Igonore;

            SOPProcessType processType = SOPProcessType.None;
            BaseSOPUser sopUser = FormSOP.Instance.SOPUser;

            if (sopUser != null)
            {
                if (nSOPGenUserID < 0)
                {
                    // 실행권한이 있는지 확인한다.
                    if (sopUser.AbletoAccess(nSensorID, FormSOP.Instance.DBManager.SiteID, FormSOP.Instance.DBManager) == false)
                    {
                        return SOPProcessType.None;
                    }
                    else
                    {
                        if (nHistoryID > 0)
                        {
                            if (m_dicSensorSOPData.ContainsKey(nHistoryID) == false)
                            {
                                m_dicSensorSOPData[nHistoryID] = sopTime;
                                SendRequestSensorSOP(nHistoryID);
                            }

                            return SOPProcessType.None;
                        }
                        else
                        {
                            FormSOP.Instance.Invoke((MethodInvoker)delegate
                            {
                                processType = _OpenSOP_Security(nEquipZoneID, sopTime, nSensorID, nHistoryID, nSensorType);
                            });

                            return processType;
                        }
                    }
                }
                else if (sopUser.ID == nSOPGenUserID)
                {
                    FormSOP.Instance.Invoke((MethodInvoker)delegate
                    {
                        processType = _OpenSOP_Security(nEquipZoneID, sopTime, nSensorID, nHistoryID, nSensorType);
                    });

                    return processType;
                }
            }

            return processType;

            // 여기서 SOP를 바로 실행하도록 하면, 다른 SOPSimulator와 중복으로 실행시킬 우려가 있다.
            // 실행권한이 있다면 일단 Server에게 실행권한이 있음을 알리고, Server로부터 최종 허가가 떨어지면 그때 SOP를 실행하도록 한다.
            /*FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                processType = _OpenSOP_Security(nEquipZoneID, sopTime, nSensorID, nHistoryID, nSensorType);
            });

            return processType;*/
        }

        public SOPProcessType OpenSOP_Security(int nEquipZoneID, DateTime sopTime, int nSensorID, int nHistoryID, int nSensorType, int nSOPGenUserID, string strSOPFullPath, string strAlarmMessage)
        {
            if (UnE.SOP.ProxySOP.Instance.OpenSOPOnSensorDetect == false)
                return SOPProcessType.Igonore;

            SOPProcessType processType = SOPProcessType.None;
            BaseSOPUser sopUser = FormSOP.Instance.SOPUser;

            if (sopUser != null)
            {
                if (sopUser.ID == nSOPGenUserID)
                {
                    FormSOP.Instance.Invoke((MethodInvoker)delegate
                    {
                        processType = _OpenSOP_Security(nEquipZoneID, sopTime, nSensorID, nHistoryID, nSensorType);
                    });

                    return processType;
                }
            }

            return processType;
        }

        private SOPProcessType _OpenSOP_Security(int nEquipZoneID, DateTime sopTime, int nSensorID, int nHistoryID, int nSensorType, string strSOPFullPath = null, string strAlarmMessage = null)
        {
            // SOP Version이 바뀌지 않았는지 검사한다.
            if (!FormSOP.Instance.GetPageHome().CheckSOPVersion())
                return SOPProcessType.None;

            /*if (FormSOP.Instance.HasControl == false)
                return SOPProcessType.None;*/

            EquipmentZone equipZone = DataManager.Instance.GetEquipZone(nEquipZoneID);

            if (equipZone == null || equipZone.LinkedZoneList.Count == 0)
                return SOPProcessType.Igonore;

            /*if (FormSOP.Instance.IsReal == false)
            {
                FormSOP.Instance.VirtualMode(false);
            }*/

            Zone zone = (Zone)equipZone.LinkedZoneList[0];

            if (strSOPFullPath == null)
                strSOPFullPath = SOPMonitoringSystem.Popup.PopupSensorOn.GetSecurityLinkedSOPName(FormSOP.Instance.DBManager, zone, nSensorType);

            if (strSOPFullPath != null)
            {
                UnE.SOP.Sections.SectionTabPage tabPage = OpenSOP(sopTime, zone, ref strSOPFullPath, ID.ID_SOP_SECURITY, nSensorID, nHistoryID);

                if (tabPage != null && tabPage.ActionStepHistoryID <= 0)
                //if (!OpenSOP(sopTime, zone, ref strSOPFullPath, ID.ID_SOP_SECURITY, nSensorID, nHistoryID))
                {
                    // Server로부터 실행권한을 부여받았으므로 TabPage를 실행가능 상태로 만든다.
                    FormSOP.Instance.SetPageMode(tabPage);

                    PlaySOPonSensorDetect(tabPage, sopTime, zone.ID, nSensorID, nHistoryID, "", null, strAlarmMessage);
                    return SOPProcessType.Run;
                }
                else
                    return SOPProcessType.Igonore;
            }

            return SOPProcessType.Igonore;
        }

        public SOPProcessType OpenSOP_PSM(int nEquipZoneID, DateTime sopTime,int nSensorID, int nHistoryID, int nSOPGenUserID)
        {
            if (UnE.SOP.ProxySOP.Instance.OpenSOPOnSensorDetect == false)
                return SOPProcessType.Igonore;

            SOPProcessType processType = SOPProcessType.None;
            BaseSOPUser sopUser = FormSOP.Instance.SOPUser;

            if (sopUser != null)
            {
                if (nSOPGenUserID < 0)
                {
                    // 실행권한이 있는지 확인한다.
                    if (sopUser.AbletoAccess(nSensorID, FormSOP.Instance.DBManager.SiteID, FormSOP.Instance.DBManager) == false)
                    {
                        return SOPProcessType.None;
                    }
                    else
                    {
                        if (nHistoryID > 0)
                        {
                            if (m_dicSensorSOPData.ContainsKey(nHistoryID) == false)
                            {
                                m_dicSensorSOPData[nHistoryID] = sopTime;
                                SendRequestSensorSOP(nHistoryID);
                            }

                            return SOPProcessType.None;
                        }
                        else
                        {
                            FormSOP.Instance.Invoke((MethodInvoker)delegate
                            {
                                processType = _OpenSOP_PSM(nEquipZoneID, sopTime, nSensorID, nHistoryID);
                            });

                            return processType;
                        }
                    }
                }
                else if (sopUser.ID == nSOPGenUserID)
                {
                    FormSOP.Instance.Invoke((MethodInvoker)delegate
                    {
                        processType = _OpenSOP_PSM(nEquipZoneID, sopTime, nSensorID, nHistoryID);
                    });

                    return processType;
                }
            }

            return processType;

            // 여기서 SOP를 바로 실행하도록 하면, 다른 SOPSimulator와 중복으로 실행시킬 우려가 있다.
            // 실행권한이 있다면 일단 Server에게 실행권한이 있음을 알리고, Server로부터 최종 허가가 떨어지면 그때 SOP를 실행하도록 한다.
            /*SOPProcessType processType = SOPProcessType.None;

            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                processType = _OpenSOP_PSM(nEquipZoneID, sopTime, nSensorID, nHistoryID);
            });

            return processType;*/
        }

        public SOPProcessType OpenSOP_PSM(int nEquipZoneID, DateTime sopTime, int nSensorID, int nHistoryID, int nSOPGenUserID, string strSOPFullPath, string strAlarmMessage)
        {
            if (UnE.SOP.ProxySOP.Instance.OpenSOPOnSensorDetect == false)
                return SOPProcessType.Igonore;

            SOPProcessType processType = SOPProcessType.None;
            BaseSOPUser sopUser = FormSOP.Instance.SOPUser;

            if (sopUser != null)
            {
                if (sopUser.ID == nSOPGenUserID)
                {
                    FormSOP.Instance.Invoke((MethodInvoker)delegate
                    {
                        processType = _OpenSOP_PSM(nEquipZoneID, sopTime, nSensorID, nHistoryID, strSOPFullPath, strAlarmMessage);
                    });

                    return processType;
                }
            }

            return processType;
        }

        private SOPProcessType _OpenSOP_PSM(int nEquipZoneID, DateTime sopTime, int nSensorID, int nHistoryID, string strSOPFullPath = null, string strAlarmMessage = null)
        {
            // SOP Version이 바뀌지 않았는지 검사한다.
            if (!FormSOP.Instance.GetPageHome().CheckSOPVersion())
                return SOPProcessType.None;

            /*if (FormSOP.Instance.HasControl == false)
                return SOPProcessType.None;*/


            EquipmentZone equipZone = DataManager.Instance.GetEquipZone(nEquipZoneID);

            if (equipZone == null || equipZone.LinkedZoneList.Count == 0)
                return SOPProcessType.Igonore;

            /*if (FormSOP.Instance.IsReal == false)
            {
                FormSOP.Instance.VirtualMode(false);
            }*/

            Zone zone = (Zone)equipZone.LinkedZoneList[0];

            if (strSOPFullPath == null)
                strSOPFullPath = SOPMonitoringSystem.Popup.PopupSensorOn.GetLinkedSOPName_PSM(FormSOP.Instance.DBManager, equipZone);

            //System.Diagnostics.Trace.WriteLine("EquipZoneID : " + equipZone.ID.ToString() + ", SOP Name : " + strSOPFullPath);
            if (strSOPFullPath != null)
            {
                UnE.SOP.Sections.SectionTabPage tabPage = OpenSOP(sopTime, zone, ref strSOPFullPath, ID.ID_SOP_POLLUTION, nSensorID, nHistoryID);

                if (tabPage != null && tabPage.ActionStepHistoryID <= 0)
                //if (!OpenSOP(sopTime, zone, ref strSOPFullPath, ID.ID_SOP_POLLUTION, nSensorID, nHistoryID))
                {
                    // Server로부터 실행권한을 부여받았으므로 TabPage를 실행가능 상태로 만든다.
                    FormSOP.Instance.SetPageMode(tabPage);

                    PlaySOPonSensorDetect(tabPage, sopTime, zone.ID, nSensorID, nHistoryID, "", null, strAlarmMessage);
                    return SOPProcessType.Run;
                }
                else
                    return SOPProcessType.Igonore;
            }

            return SOPProcessType.Igonore;
        }

        // strPosition : 진앙지
        public SOPProcessType OpenSOP_Earthquake(string strSOPFullPath, DateTime sopTime, int nSensorZoneID, int nHistoryID, int nIntensity, float fMagnitude, string strPosition, int nSOPGenUserID)
        {
            SOPProcessType processType = SOPProcessType.None;

            BaseSOPUser sopUser = FormSOP.Instance.SOPUser;

            if (sopUser != null)
            {
                if (nSOPGenUserID < 0)
                {
                    // 지진은 실행권한 확인하지 않는다.
                    if (nHistoryID > 0)
                    {
                        if (m_dicSensorSOPData.ContainsKey(nHistoryID) == false)
                        {
                            m_dicSensorSOPData[nHistoryID] = sopTime;
                            SendRequestSensorSOP(nHistoryID);
                        }

                        return SOPProcessType.None;
                    }
                    else
                    {
                        return OpenSOP_Earthquake(strSOPFullPath, sopTime, nSensorZoneID, nHistoryID, nIntensity, fMagnitude, strPosition);
                    }
                }
                else if (sopUser.ID == nSOPGenUserID)
                {
                    return OpenSOP_Earthquake(strSOPFullPath, sopTime, nSensorZoneID, nHistoryID, nIntensity, fMagnitude, strPosition);
                }
            }

            /*FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                // SOP Version이 바뀌지 않았는지 검사한다.
                if (!FormSOP.Instance.GetPageHome().CheckSOPVersion())
                    processType = SOPProcessType.None;
                else
                {
                    if (strSOPFullPath != null)
                    {
                        if (!OpenSOP(sopTime, null, ref strSOPFullPath, ID.ID_SOP_EARTHQUAKE, nSensorZoneID, nHistoryID))
                        {
                            PlaySOPonNoSensorDetect(sopTime, nSensorZoneID, nHistoryID, SetEarthquakeOption, strSOPFullPath, nIntensity, fMagnitude, strPosition);
                            processType = SOPProcessType.Run;
                        }
                        else
                            processType = SOPProcessType.Igonore;
                    }
                }
            });*/

            return processType;
        }

        private SOPProcessType OpenSOP_Earthquake(string strSOPFullPath, DateTime sopTime, int nSensorZoneID, int nHistoryID, int nIntensity, float fMagnitude, string strPosition)
        {
            SOPProcessType processType = SOPProcessType.None;

            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                // SOP Version이 바뀌지 않았는지 검사한다.
                if (!FormSOP.Instance.GetPageHome().CheckSOPVersion())
                    processType = SOPProcessType.None;
                else
                {
                    if (strSOPFullPath != null)
                    {
                        UnE.SOP.Sections.SectionTabPage tabPage = OpenSOP(sopTime, null, ref strSOPFullPath, ID.ID_SOP_EARTHQUAKE, nSensorZoneID, nHistoryID);

                        if (tabPage != null && tabPage.ActionStepHistoryID <= 0)
                        //if (!OpenSOP(sopTime, null, ref strSOPFullPath, ID.ID_SOP_EARTHQUAKE, nSensorZoneID, nHistoryID))
                        {
                            // Server로부터 실행권한을 부여받았으므로 TabPage를 실행가능 상태로 만든다.
                            FormSOP.Instance.SetPageMode(tabPage);

                            PlaySOPonNoSensorDetect(sopTime, nSensorZoneID, nHistoryID, SetEarthquakeOption, strSOPFullPath, nIntensity, fMagnitude, strPosition);
                            processType = SOPProcessType.Run;
                        }
                        else
                            processType = SOPProcessType.Igonore;
                    }
                }
            });

            return processType;
        }

        public void OpenSOP_StrongWind(int nActionStepID, DateTime sopTime, int nHistoryID, int nSensorZoneID, int nAlarmLevel, string strSensorValue, EquipmentZone equipZone)
        {
            VariousData<bool> isNormal;
            string strSOPFullPath = ReadSOPFullPath(nActionStepID, out isNormal);

            Zone zone = GetZone(equipZone);

            string strBuildingName = "";

            if (zone != null && zone.Building != null)
                strBuildingName = zone.Building.BuildingName;

            float fWindSpeed;

            if (float.TryParse(strSensorValue, out fWindSpeed) == false)
                return;

            OpenSOP_StrongWind(strSOPFullPath, sopTime, nHistoryID, nSensorZoneID, nAlarmLevel, fWindSpeed, strBuildingName);
        }

        private Zone GetZone(EquipmentZone equipZone)
        {
            if (equipZone == null)
                return null;

            foreach (Zone zone in equipZone.LinkedZoneList)
            {
                return zone;
            }

            return null;
        }

        private string ReadSOPFullPath(int nActionStepID, out VariousData<bool> isNormal)
        {
            isNormal = null;

            string strSQL = "Select dc.CategoryName, sdc.SubCategoryName, d.DisasterName, step.StepName, v.isNormal ";
            strSQL += "from ActionStep as step, Disaster as d, SubDisasterCategory as sdc, DisasterCategory as dc, Version as v ";
            strSQL += "where step.DisasterID = d.ID and d.SubDisasterID = sdc.ID and sdc.DisasterID = dc.ID and d.VersionID = v.ID and step.ID = " + nActionStepID.ToString();

            ArrayList arrResult = FormSOP.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null || arrResult.Count < 5)
                return null;

            string strCategoryName = WebDBManager.GetStringField(arrResult[0]);
            string strSubCategoryName = WebDBManager.GetStringField(arrResult[1]);
            string strDisasterName = WebDBManager.GetStringField(arrResult[2]);
            string strActionStepName = WebDBManager.GetStringField(arrResult[3]);
            VariousData<int> normal = WebDBManager.GetIntField(arrResult[4].ToString());

            if (strCategoryName == null || strSubCategoryName == null ||
                strDisasterName == null || strActionStepName == null ||
                normal == null)
                return null;

            isNormal = new VariousData<bool>(normal.Data == 1);
            return strCategoryName + "/" + strSubCategoryName + "/" + strDisasterName + "/" + strActionStepName;
        }

        public void OpenSOP_StrongWind(string strSOPFullPath, DateTime sopTime, int nHistoryID, int nSensorZoneID, int nAlarmLevel, float fWindSpeed, string strPosition)
        {
            BaseSOPUser sopUser = FormSOP.Instance.SOPUser;

            if (sopUser != null)
            {
                //if (sopUser.AbletoAccess(nSensorZoneID, ))
                FormSOP.Instance.Invoke((MethodInvoker)delegate
                {
                // SOP Version이 바뀌지 않았는지 검사한다.
                if (!FormSOP.Instance.GetPageHome().CheckSOPVersion())
                        return;

                    if (strSOPFullPath != null)
                    {
                        UnE.SOP.Sections.SectionTabPage tabPage = OpenSOP(sopTime, null, ref strSOPFullPath, ID.ID_SOP_STRONGWIND, nSensorZoneID, nHistoryID);

                        if (tabPage != null && tabPage.ActionStepHistoryID <= 0)
                        {
                            // Server로부터 실행권한을 부여받았으므로 TabPage를 실행가능 상태로 만든다.
                            FormSOP.Instance.SetPageMode(tabPage);
                            PlaySOPonNoSensorDetect(sopTime, nSensorZoneID, nHistoryID, SetStrongWindOption, strSOPFullPath, fWindSpeed, strPosition);
                        }
                    }
                });
            }
        }

        /*public SOPProcessType OpenSOP_ETC(int nZoneID, int nEquipZoneID, DateTime sopTime, int nSensorID, int nHistoryID, int nSensorType, int nActionStepID, bool isRealMode, string strSensorValue, string strDisasterOption)
        {
            SOPProcessType processType = SOPProcessType.None;
            BaseSOPUser sopUser = FormSOP.Instance.SOPUser;

            if (sopUser != null)
            {
                // 실행권한이 있는지 확인한다.
                if (sopUser.AbletoAccess(nSensorID, FormSOP.Instance.DBManager.SiteID, FormSOP.Instance.DBManager) == false)
                {
                    return processType;
                }
            }

            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                processType = _OpenSOP_ETC(nZoneID, nEquipZoneID, sopTime, nSensorID, nHistoryID, nSensorType, nActionStepID, isRealMode, strSensorValue, strDisasterOption);
            });

            return processType;
        }*/

        public SOPProcessType OpenSOP_ETC(int nZoneID, int nEquipZoneID, DateTime sopTime, int nSensorID, int nHistoryID, int nSensorType, int nSOPGenUserID)
        {
            SOPProcessType processType = SOPProcessType.None;
            BaseSOPUser sopUser = FormSOP.Instance.SOPUser;

            if (sopUser != null)
            {
                if (nSOPGenUserID < 0)
                {
                    // 실행권한이 있는지 확인한다.
                    if (sopUser.AbletoAccess(nSensorID, FormSOP.Instance.DBManager.SiteID, FormSOP.Instance.DBManager) == false)
                    {
                        return processType;
                    }
                    else
                    {
                        if (nHistoryID > 0)
                        {
                            if (m_dicSensorSOPData.ContainsKey(nHistoryID) == false)
                            {
                                m_dicSensorSOPData[nHistoryID] = sopTime;
                                SendRequestSensorSOP(nHistoryID);
                            }

                            return SOPProcessType.None;
                        }
                        else
                        {
                            FormSOP.Instance.Invoke((MethodInvoker)delegate
                            {
                                processType = _OpenSOP_ETC(nZoneID, nEquipZoneID, sopTime, nSensorID, nHistoryID, nSensorType);
                            });

                            return processType;
                        }
                    }
                }
                else if (sopUser.ID == nSOPGenUserID)
                {
                    FormSOP.Instance.Invoke((MethodInvoker)delegate
                    {
                        processType = _OpenSOP_ETC(nZoneID, nEquipZoneID, sopTime, nSensorID, nHistoryID, nSensorType);
                    });

                    return processType;
                }
            }

            return processType;

            // 여기서 SOP를 바로 실행하도록 하면, 다른 SOPSimulator와 중복으로 실행시킬 우려가 있다.
            // 실행권한이 있다면 일단 Server에게 실행권한이 있음을 알리고, Server로부터 최종 허가가 떨어지면 그때 SOP를 실행하도록 한다.

            /*FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                processType = _OpenSOP_ETC(nZoneID, nEquipZoneID, sopTime, nSensorID, nHistoryID, nSensorType);
            });

            return processType;*/
        }

        public SOPProcessType OpenSOP_ETC(int nZoneID, int nEquipZoneID, DateTime sopTime, int nSensorID, int nHistoryID, int nSensorType, int nSOPGenUserID, string strSOPFullPath, string strAlarmMessage)
        {
            SOPProcessType processType = SOPProcessType.None;
            BaseSOPUser sopUser = FormSOP.Instance.SOPUser;

            if (sopUser != null)
            {
                if (sopUser.ID == nSOPGenUserID)
                {
                    FormSOP.Instance.Invoke((MethodInvoker)delegate
                    {
                        processType = _OpenSOP_ETC(nZoneID, nEquipZoneID, sopTime, nSensorID, nHistoryID, nSensorType, strSOPFullPath, strAlarmMessage);
                    });

                    return processType;
                }
            }

            return processType;
        }

        private SOPProcessType _OpenSOP_ETC(string strPosition, DateTime sopTime, int nSensorID, int nHistoryID, string strSOPFullPath, int nSOPType)
        {
            // SOP Version이 바뀌지 않았는지 검사한다.
            if (!FormSOP.Instance.GetPageHome().CheckSOPVersion())
                return SOPProcessType.None;

            /*if (FormSOP.Instance.HasControl == false)
                return SOPProcessType.None;*/

            string strArgument = strPosition;
        
            if (strSOPFullPath != null && strSOPFullPath.Length > 0)
            {
                UnE.SOP.Sections.SectionTabPage tabPage = OpenSOP(sopTime, null, ref strSOPFullPath, nSOPType, nSensorID, nHistoryID, strPosition);

                if (tabPage != null && tabPage.ActionStepHistoryID <= 0)
                //if (!OpenSOP(sopTime, null, ref strSOPFullPath, nSOPType, nSensorID, nHistoryID, strPosition))
                {
                    PlaySOPonSensorDetect(tabPage, sopTime, -1, nSensorID, nHistoryID);
                    return SOPProcessType.Run;
                }
                else
                    return SOPProcessType.Igonore;
            }

            return SOPProcessType.Igonore;
        }

        /*private SOPProcessType _OpenSOP_ETC(int nZoneID, int nEquipZoneID, DateTime sopTime, int nSensorID, int nHistoryID, int nSensorType, int nActionStepID, bool isRealMode, string strSensorValue, string strDisasterOption)
        {
            // SOP Version이 바뀌지 않았는지 검사한다.
            if (!FormSOP.Instance.GetPageHome().CheckSOPVersion())
                return SOPProcessType.None;

            Zone zone = null;
            int nSOPType = ID.ID_SOP_UNKNOWN;

            NoSensorDetectOption optionMethod = null;

            if (nSensorType == (int)UnE.Sensor.IFacility.FacilityType.STRONG_WIND ||
                nSensorType == (int)UnE.Sensor.IFacility.FacilityType.BLACKOUT)
            {
                if (nZoneID > 0)
                {
                    zone = DataManager.Instance.GetZone(nZoneID);
                }

                if (nSensorType == (int)UnE.Sensor.IFacility.FacilityType.STRONG_WIND)
                {
                    nSOPType = ID.ID_SOP_STRONGWIND;
                    optionMethod = SetStrongWindOption;
                }
                else if (nSensorType == (int)UnE.Sensor.IFacility.FacilityType.BLACKOUT)
                    nSOPType = ID.ID_SOP_POWEROFF;
            }

            object sensorData = GetEtcSensorData(nSensorType, strSensorValue);
            VariousData<bool> isNormal;
            string strSOPFullPath = ReadSOPFullPath(nActionStepID, out isNormal);

            if (strSOPFullPath != null)
            {
                if (!OpenSOP(sopTime, zone, strSOPFullPath, nSOPType, nSensorID, nHistoryID, null, isNormal))
                {
                    PlaySOPonSensorDetect(nZoneID, nSensorID, nHistoryID, strDisasterOption, new VariousData<bool>(isRealMode));
                    return SOPProcessType.Run;
                }
                else
                    return SOPProcessType.Igonore;
            }

            return SOPProcessType.Igonore;
        }*/

        private SOPProcessType _OpenSOP_ETC(int nZoneID, int nEquipZoneID, DateTime sopTime, int nSensorID, int nHistoryID, int nSensorType, string strSOPFullPath = null, string strAlarmMessage = null)
        {
            // SOP Version이 바뀌지 않았는지 검사한다.
            if (!FormSOP.Instance.GetPageHome().CheckSOPVersion())
                return SOPProcessType.None;

            /*if (FormSOP.Instance.HasControl == false)
                return SOPProcessType.None;*/

            string strArgument = "";
            Zone zone = nZoneID > 0 ? DataManager.Instance.GetZone(nZoneID) : null;
            int nSOPType = ID.ID_SOP_UNKNOWN;

            if (nSensorType == (int)UnE.Sensor.IFacility.FacilityType.STRONG_WIND ||
                nSensorType == (int)UnE.Sensor.IFacility.FacilityType.BLACKOUT)
            {
                if (nZoneID > 0)
                {
                    //zone = DataManager.Instance.GetZone(nZoneID);

                    if (zone != null)
                    {
                        if (zone.Building != null)
                            strArgument = zone.Building.DisplayText;
                        else
                            strArgument = zone.DisplayName;
                    }
                }

                if (nSensorType == (int)UnE.Sensor.IFacility.FacilityType.STRONG_WIND)
                    nSOPType = ID.ID_SOP_STRONGWIND;
                else if (nSensorType == (int)UnE.Sensor.IFacility.FacilityType.BLACKOUT)
                    nSOPType = ID.ID_SOP_POWEROFF;
            }

            string strDisasterOption;
            object sensorData = GetCurrentEtcSensorData(nHistoryID, nSensorType, out strDisasterOption);

            if (strSOPFullPath == null)
            {
                string strSOPName = GetEtcSensorSOPLink(nSensorType, zone);

                if (strSOPName == null)
                    return SOPProcessType.Igonore;

                if (sensorData == null)
                {
                    if (strSOPName != null)
                    {
                        string strActionStepName = FormSOP.Instance.SOPSupervisor.GetActionStepName(strSOPName, nSensorType);

                        if (strActionStepName != null && strActionStepName.Length > 0)
                            strSOPFullPath = strSOPName + "/" + strActionStepName;
                        else
                            strSOPFullPath = null;
                    }
                }
                else
                {
                    OptionEtcData data = SOPMonitoringSystem.Popup.PopupSensorOn.GetETCOptionData(FormSOP.Instance.DBManager, zone, nSensorType, sensorData);
                    //strSOPFullPath = SOPMonitoringSystem.Popup.PopupSensorOn.GetLinkedSOPName_ETC(FormSOP.Instance.DBManager, zone, nSensorType, sensorData);

                    if (data.AlarmDepth > 0)
                        strSOPFullPath = strSOPName + "/" + UnE.SOP.Sections.SectionTabControl.StandardActionStepNames[data.AlarmDepth - 1];
                    else
                        return SOPProcessType.Igonore;
                }
            }

            if (strSOPFullPath != null)
            {
                UnE.SOP.Sections.SectionTabPage tabPage = OpenSOP(sopTime, zone, ref strSOPFullPath, nSOPType, nSensorID, nHistoryID);

                // sensorData는 어떻게 전달?
                if (tabPage != null && tabPage.ActionStepHistoryID <= 0)
                //if (!OpenSOP(sopTime, zone, ref strSOPFullPath, nSOPType, nSensorID, nHistoryID))
                {
                    // Server로부터 실행권한을 부여받았으므로 TabPage를 실행가능 상태로 만든다.
                    FormSOP.Instance.SetPageMode(tabPage);
                    PlaySOPonSensorDetect(tabPage, sopTime, nZoneID, nSensorID, nHistoryID, strDisasterOption, null, strAlarmMessage);
                    return SOPProcessType.Run;
                }
                else
                    return SOPProcessType.Igonore;
            }

            return SOPProcessType.Igonore;
        }

        private string GetEtcSensorSOPLink(int nSensorType, Zone zone)
        {
            // 1. LinkedZone에 맞는 SOP가 있으면 먼저 선택한다.
            // 2. LinkedBuilding에 맟는 SOP가 있으면 그 다음 우선순위로 선택한다.
            // 3. 둘다 없을 경우 SensorType에 맞는 SOP를 선택한다.
            string strSQL = string.Format("Select SOPName, LinkedBuildingID, LinkedZoneID from ETCSensorSOPLink where Type = {0} and SiteID = {1}", nSensorType, FormSOP.Instance.DBManager.SiteID);
            ArrayList arrResult = FormSOP.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;
            string strBuildingSOP = null, strZoneSOP = null, strSOP = null;

            for (int i=0;i<nResultCount-2;i+=3)
            {
                string strSOPName = WebDBManager.GetStringField(arrResult[i]);
                string strBuildingIDs = WebDBManager.GetStringField(arrResult[i + 1]);
                string strZoneIDs = WebDBManager.GetStringField(arrResult[i + 2]);

                if (strSOPName == null)
                    continue;

                if (strBuildingIDs != null && zone.Building != null)
                {
                    if (IncludeID(strBuildingIDs, zone.Building.ID))
                    {
                        strBuildingSOP = strSOPName;
                    }
                }

                if (strZoneIDs != null)
                {
                    if (IncludeID(strBuildingIDs, zone.ID))
                    {
                        // Building보다 Zone SOP가 더 우선권을 갖는다.
                        strZoneSOP = strSOPName;
                        return strZoneSOP;
                    }
                }

                if (strBuildingIDs == null && strZoneIDs == null)
                    strSOP = strSOPName;
            }

            if (strBuildingSOP != null)
                return strBuildingSOP;

            if (strSOP != null)
                return strSOP;

            // Zone 이름과 비슷한 건물 SOP가 존재하는지 검사한다.
            for (int i = 0; i < nResultCount - 2; i += 3)
            {
                string strSOPName = WebDBManager.GetStringField(arrResult[i]);
                string strBuildingIDs = WebDBManager.GetStringField(arrResult[i + 1]);
                
                if (strSOPName == null)
                    continue;

                if (strBuildingIDs != null)
                {
                    string[] ids = strBuildingIDs.Split(',');
                    int nBuildingID;

                    foreach (string strID in ids)
                    {
                        if (int.TryParse(strID.Trim(), out nBuildingID))
                        {
                            Building building = DataManager.Instance.GetBuilding(nBuildingID);

                            if (building != null && zone.ZoneName.Contains(building.BuildingName))
                                return strSOPName;
                        }
                    }
                }
            }

            return null;
        }

        private bool IncludeID(string strIDs, int nID)
        {
            string[] ids = strIDs.Split(',');
            int id;

            foreach (string strID in ids)
            {
                if (int.TryParse(strID.Trim(), out id))
                {
                    if (id == nID)
                        return true;
                }
            }

            return false;
        }

        private object GetCurrentEtcSensorData(int nSensorZoneHistoryID, int nSensorType, out string strDisasterOption)
        {
            strDisasterOption = "";

            string strSQL = "SELECT ID, ReactionType, Param4 from SensorReactionHistory where ID in (";
            strSQL += string.Format("Select max(ID) from SensorReactionHistory where SensorHistoryID = {0} and ReactionType not in {1} group by ReactionType)",
                nSensorZoneHistoryID, GetAlarmOffReactionHistoryQueryString());

            ArrayList arrResult = FormSOP.Instance.DBManager.GetResultData(strSQL);

            if (arrResult == null)
                return null;

            int nResultCount = arrResult.Count;

            for (int i=nResultCount-3;i>=0;i-=3)
            {
                VariousData<int> id = WebDBManager.GetIntField(arrResult[i].ToString());
                VariousData<int> reactionType = WebDBManager.GetIntField(arrResult[i + 1].ToString());
                string strParam4 = WebDBManager.GetStringField(arrResult[i + 2]);

                if (id == null || reactionType == null || strParam4 == null)
                    continue;

                if (reactionType.Data == (int)libSensorProcess.ReactionType.BEGIN_STATUS ||
                    reactionType.Data == (int)libSensorProcess.ReactionType.CHANGE_ALARM_DEPTH)
                {
                    if (nSensorType == (int)IFacility.FacilityType.STRONG_WIND)
                    {
                        float fWindSpeed;

                        if (GetWindSpeed(strParam4, out fWindSpeed))
                        {
                            strDisasterOption = "[강풍:wSpeed/" + fWindSpeed.ToString() + "]";
                            return fWindSpeed;
                        }
                    }
                }
            }

            return null;
        }

        private object GetEtcSensorData(int nSensorType, string strSensorValue)
        {
            if (nSensorType == (int)IFacility.FacilityType.STRONG_WIND)
            {
                float fWindSpeed;

                if (GetWindSpeed(strSensorValue.Trim(), out fWindSpeed))
                    return fWindSpeed;
            }

            return null;
        }

        private bool GetWindSpeed(string strParam, out float fWindSpeed)
        {
            fWindSpeed = 0.0f;
            int nIndex = strParam.IndexOf(':');

            if (nIndex < 0)
                return false;

            string str = strParam.Substring(nIndex + 1).Trim();

            int len = str.Length;
            bool firstDot = true;
            int nEndIndex = -1;

            for (int i=0;i<len;i++)
            {
                char ch = str.ElementAt(i);

                if (ch >= '0' && ch <= '9')
                    continue;
                else if (ch == '.')
                {
                    if (firstDot)
                    {
                        firstDot = false;
                        continue;
                    }
                }

                nEndIndex = i;
                break;
            }

            string strWindSpeed = str.Substring(0, nEndIndex);
            return float.TryParse(strWindSpeed, out fWindSpeed);
        }

        private string GetAlarmOffReactionHistoryQueryString()
        {
            string strCondition = ((int)libSensorProcess.ReactionType.MALFUNCTION).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.IGNORE_SIGNAL).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.IGNORE_SOP).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.END_STATUS).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.USER_RESET).ToString();
            strCondition += ", " + ((int)libSensorProcess.ReactionType.TIME_OUT).ToString();

            return "(" + strCondition + ")";
        }

        private int GetActionStepIndex(string strActionStepName)
        {
            int nCount = UnE.SOP.Sections.SectionTabControl.StandardActionStepNames.Count();

            for (int i = 0; i < nCount; i++)
            {
                if (UnE.SOP.Sections.SectionTabControl.StandardActionStepNames[i] == strActionStepName)
                    return i + 1;
            }

            return -1;
        }

        private UnE.SOP.Sections.SectionTabPage OpenSOP(DateTime sopTime, Zone zone, ref string strSOPFullPath, int nDefaultSOPID, int nSensorZoneID, int nSensorZoneHistoryID, string strPositionName = null, VariousData<bool> isNormalSOP = null)
        {
            // 외부에서 사용중인 SOP의 Path 구분자는 '/' 이다
            if (strSOPFullPath == null || strSOPFullPath.Length == 0)
                strSOPFullPath = FormSOP.Instance.GetPageHome().GetQuickSOPFullPath(nDefaultSOPID);

            if (strSOPFullPath == null)
                return null;

            strSOPFullPath = strSOPFullPath.Replace('\\', '/');

            // 내부에 저장된 SOP Path의 구분자는 0x06이다
            string cmpSOPPath = strSOPFullPath.Replace('/', (char)0x06);

            string[] tokens = cmpSOPPath.Split((char)0x06);

            if (tokens.Count() < 4)
            {
                int nSensorType = GetSensorTypeFromSOP(nDefaultSOPID);
                string strActionStepName = FormSOP.Instance.SOPSupervisor.GetActionStepName(strSOPFullPath, nSensorType);

                if (strActionStepName.Length > 0)
                    strSOPFullPath += "/" + strActionStepName;
            }

            BarLevelTree tree = SOPScenarioManager.Instance.GetBarLevelTree();

            // 현재 평일모드인지 여부
            bool bIsNormal = isNormalSOP == null ? Popup.SOPLoader.IsDayLight_NoInvoke(sopTime) : isNormalSOP.Data;
            // 평일/휴일 전환여부
            bool bChangedTree = false;
            // 실행중인 SOP가 있는경우
            //bool bFindRunSOP = false;
            UnE.SOP.Sections.SectionTabPage openedPage = null;

            // 전체 실행중인 SOP시나리오를 가져온다.
            System.Collections.ArrayList arScList = SOPScenarioManager.Instance.GetAllScenario();
            List<UnE.SOP.Workstate.SOPScenario> currentScenarios = new List<UnE.SOP.Workstate.SOPScenario>();

            if (arScList != null && arScList.Count > 0)
            {
                List<int> removeActionStepHistoryIDs = new List<int>();

                foreach (UnE.SOP.Workstate.SOPScenario sc in arScList)
                {
                    String szCmp = sc.ActionStepFullPath;

                    // 현재 요청한 SOP가 실행중인지 여부를 검사
                    if ((szCmp.StartsWith(strSOPFullPath) || szCmp.StartsWith(cmpSOPPath)) && sc.NormalMode == bIsNormal)
                    {
                        if (sc.SensorZoneHistoryID > 0 && sc.SensorZoneHistoryID == nSensorZoneHistoryID)
                        {
                            openedPage = FormSOP.Instance.GetPageHome().ScenarioTab.GetTabPage(sc);
                            //bFindRunSOP = true;
                        }
                        else
                        {
                            UnE.SOP.Workstate.SOPScenario scenario = FormSOP.Instance.GetPageHome().GetSOPScenario(sc.ActionStepHistoryID);

                            if (scenario != null)
                            {
                                openedPage = FormSOP.Instance.GetPageHome().ScenarioTab.GetTabPage(scenario);
                                //bFindRunSOP = true;
                            }
                            else
                                removeActionStepHistoryIDs.Add(sc.ActionStepHistoryID);
                        }
                        // 현재 실행중인 시나리오는 SelectedScenario로 선택한다.( 내부적으로 Tree와 TabPage처리가 된다.)
                        //SOPScenarioManager.Instance.SelectedScenario(sc.ActionStepID, sc.RealMode);
                        //break;

                        /*if (currentScenarios == null)
                            currentScenarios = new List<UnE.SOP.Workstate.SOPScenario>();*/

                        currentScenarios.Add(sc);
                    }
                }

                foreach (int nActionStepHistoryID in removeActionStepHistoryIDs)
                {
                    SOPScenarioManager.Instance.RemoveScenario(nActionStepHistoryID);
                }
            }

            // 이미 같은 SOP가 실행중이지만 다른 알람이 또다시 감지되었으므로,
            // SOP 진행에 추가로 변화가 있을수 있는지 검토한다.
            if (/*currentScenarios != null && */FormSOP.Instance.SOPSupervisor != null)
            {
                int nSensorType = GetSensorTypeFromSOP(nDefaultSOPID);
                bool isChanged = FormSOP.Instance.SOPSupervisor.CheckOpenSOP(currentScenarios, ref strSOPFullPath, nSensorZoneID, nSensorZoneHistoryID, nSensorType);

                if (isChanged)
                {
                    openedPage = null;
                    //bFindRunSOP = false;
                }
            }

            bool isNormal = bIsNormal;

            // 요청한 SOP가 실행중이 아니면
            if (openedPage == null)
            //if (bFindRunSOP == false)
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

                    //UnE.SOP.Workstate.SOPScenario currentScenario = FormSOP.Instance.GetPageHome().ScenarioTab.CurrentScenario;
                    bool backgroundLoading = IsBackgroundLoading(/*zone, nSensorZoneID, nSensorZoneHistoryID*/);
                    FormSOP.Instance.GetPageHome().BackgroundLoading = backgroundLoading;

                    // SOP 선택하여 를 화면에 표시한다.
                    tree.SelectNode(node);

                    openedPage = FormSOP.Instance.GetPageHome().GetActionStepPage(node.Text);

                    // 모드가 바뀐 경우,  Tree전체를 갱신해준다.
                    if (bChangedTree == true)
                    {
                        tree.Load(FormSOP.Instance.SOPManager, true, isNormal/*bIsNormal*/);
                    }

                    FormSOP.Instance.GetPageHome().BackgroundLoading = false;

                    // 원래대로 되돌려 놓는다.
                    //if (backgroundLoading && currentScenario != null)
                    //{
                    //    FormSOP.Instance.GetPageHome().ScenarioTab.Select(currentScenario);
                    //}

                    // 최근에 로드된 SOP의 TabPage는 반드시 SelectedTab에 존재한다.
                    UnE.SOP.Sections.SectionTabPage tabPage = (UnE.SOP.Sections.SectionTabPage)FormSOP.Instance.GetPageHome().TabControls.SelectedTab;
                    if (tabPage != null)
                    {
                        // TabPage에 실행정보를 저장한다.
                        if (strPositionName != null)
                        {
                            tabPage.LinkedZoneName = strPositionName;

                            if (zone != null)
                                tabPage.LinkedZoneID = zone.ID;
                        }
                        else if (zone != null)
                        {
                            tabPage.LinkedZoneName = zone.BroadcastName;
                            tabPage.LinkedZoneID = zone.ID;
                        }

                        tabPage.LinkedTime = sopTime;

                        // Sensor로부터 로딩된 SOP는 SensorZoneHistoryID와 SensorID를 넣어준다.
                        tabPage.SensorZoneHistoryID = nSensorZoneHistoryID;
                        tabPage.SensorID = nSensorZoneID;
                    }
                }
            }

            return openedPage;
            //return bFindRunSOP;
        }

        private UnE.SOP.Sections.SectionTabPage OpenSOP(DateTime sopTime, string strSOPFullPath, Dictionary<string, string> dicParameters)
        {
            // 내부에 저장된 SOP Path의 구분자는 0x06이다
            string cmpSOPPath = strSOPFullPath.Replace('/', (char)0x06);

            BarLevelTree tree = SOPScenarioManager.Instance.GetBarLevelTree();

            // 현재 평일모드인지 여부
            bool bIsNormal = Popup.SOPLoader.IsDayLight_NoInvoke(sopTime);
            // 평일/휴일 전환여부
            bool bChangedTree = false;
            // 실행중인 SOP가 있는경우
            //bool bFindRunSOP = false;
            UnE.SOP.Sections.SectionTabPage openedPage = null;

            // 전체 실행중인 SOP시나리오를 가져온다.
            System.Collections.ArrayList arScList = SOPScenarioManager.Instance.GetAllScenario();
            List<UnE.SOP.Workstate.SOPScenario> currentScenarios = new List<UnE.SOP.Workstate.SOPScenario>();

            if (arScList != null && arScList.Count > 0)
            {
                List<int> removeActionStepHistoryIDs = new List<int>();

                foreach (UnE.SOP.Workstate.SOPScenario sc in arScList)
                {
                    String szCmp = sc.ActionStepFullPath;

                    // 현재 요청한 SOP가 실행중인지 여부를 검사
                    if ((szCmp.StartsWith(strSOPFullPath) || szCmp.StartsWith(cmpSOPPath)) && sc.NormalMode == bIsNormal)
                    {
                        UnE.SOP.Workstate.SOPScenario scenario = FormSOP.Instance.GetPageHome().GetSOPScenario(sc.ActionStepHistoryID);

                        if (scenario != null)
                        {
                            openedPage = FormSOP.Instance.GetPageHome().ScenarioTab.GetTabPage(scenario);
                        }
                        else
                            removeActionStepHistoryIDs.Add(sc.ActionStepHistoryID);
                        
                        currentScenarios.Add(sc);
                    }
                }

                foreach (int nActionStepHistoryID in removeActionStepHistoryIDs)
                {
                    SOPScenarioManager.Instance.RemoveScenario(nActionStepHistoryID);
                }
            }

            bool isNormal = bIsNormal;

            // 요청한 SOP가 실행중이 아니면
            if (openedPage == null)
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
                    bool backgroundLoading = IsBackgroundLoading(/*zone, nSensorZoneID, nSensorZoneHistoryID*/);
                    FormSOP.Instance.GetPageHome().BackgroundLoading = backgroundLoading;

                    // SOP 선택하여 를 화면에 표시한다.
                    tree.SelectNode(node);

                    openedPage = FormSOP.Instance.GetPageHome().GetActionStepPage(node.Text);

                    // 모드가 바뀐 경우,  Tree전체를 갱신해준다.
                    if (bChangedTree == true)
                    {
                        tree.Load(FormSOP.Instance.SOPManager, true, isNormal/*bIsNormal*/);
                    }

                    FormSOP.Instance.GetPageHome().BackgroundLoading = false;

                    // 원래대로 되돌려 놓는다.
                    //if (backgroundLoading && currentScenario != null)
                    //{
                    //    FormSOP.Instance.GetPageHome().ScenarioTab.Select(currentScenario);
                    //}

                    // 최근에 로드된 SOP의 TabPage는 반드시 SelectedTab에 존재한다.
                    UnE.SOP.Sections.SectionTabPage tabPage = (UnE.SOP.Sections.SectionTabPage)FormSOP.Instance.GetPageHome().TabControls.SelectedTab;
                    if (tabPage != null)
                    {
                        string strPositionName = null;

                        if (dicParameters.TryGetValue("{location}", out strPositionName) == false)
                            strPositionName = null;

                        // TabPage에 실행정보를 저장한다.
                        if (strPositionName != null)
                        {
                            tabPage.LinkedZoneName = strPositionName;
                        }
                        
                        tabPage.LinkedTime = sopTime;
                    }
                }
            }

            return openedPage;
        }

        private bool IsBackgroundLoading(/*Zone zone, int nSensorZoneID, int nSensorZoneHistoryID*/)
        {
            return FormSOP.Instance.GetPageHome().CurrentDisasterID > 0;
        }

        private int GetSensorTypeFromSOP(int nSOPID)
        {
            int nSensorType = (int)UnE.Sensor.IFacility.FacilityType.NONE;

            if (nSOPID == ID.ID_SOP_FIRE)
                nSensorType = (int)UnE.Sensor.IFacility.FacilityType.FIRE_SENSOR;
            else if (nSOPID == ID.ID_SOP_POLLUTION)
                nSensorType = (int)UnE.Sensor.IFacility.FacilityType.PSM_SENSOR;
            else if (nSOPID == ID.ID_SOP_SECURITY)
                nSensorType = (int)UnE.Sensor.IFacility.FacilityType.Security_Sensor;
            else if (nSOPID == ID.ID_SOP_POWEROFF)
                nSensorType = (int)UnE.Sensor.IFacility.FacilityType.BLACKOUT;
            else if (nSOPID == ID.ID_SOP_STRONGWIND)
                nSensorType = (int)UnE.Sensor.IFacility.FacilityType.STRONG_WIND;

            return nSensorType;
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
                FormSOP.Instance.SOPSupervisor.RegisterSameSensorGroupRunning(nSensorZoneID, nSensorZoneHistoryID);
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
                CloseSensorZoneHistory(nSensorZoneHistoryID);
                FormSOP.Instance.SOPSupervisor.SensorClose(nSensorZoneID, nSensorZoneHistoryID);
                //SDMS.ScriptProxy.Instance.UserObject.SupervisorSOPSensorClose.Invoke(nSensorZoneID);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.StackTrace);
            }      
        }

        public void CloseSensorZoneHistory(int nSensorZoneHistoryID)
        {
            DateTime temp;
            m_dicSensorSOPData.TryRemove(nSensorZoneHistoryID, out temp);
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

        public void RunSOP(bool isRealMode, string strSOPFullPath, int nSOPUserID, List<string> parameters)
        {
            BaseSOPUser sopUser = FormSOP.Instance.SOPUser;

            if (sopUser != null)
            {
                if (sopUser.ID == nSOPUserID)
                {
                    DateTime sopTime = DateTime.Now;
                    Dictionary<UnE.SOP.SOPParameter, string> dicUserDefinedParameters = new Dictionary<UnE.SOP.SOPParameter, string>();
                    Dictionary<string, string> dicParameters = MakeParameterDictionary(parameters, dicUserDefinedParameters);

                    FormSOP.Instance.Invoke((MethodInvoker)delegate
                    {
                        UnE.SOP.Sections.SectionTabPage tabPage = OpenSOP(sopTime, strSOPFullPath, dicParameters);

                        if (tabPage != null && tabPage.ActionStepHistoryID <= 0)
                        {
                            tabPage.VirtualMode = !isRealMode;

                            // Server로부터 실행권한을 부여받았으므로 TabPage를 실행가능 상태로 만든다.
                            FormSOP.Instance.SetPageMode(tabPage);

                            if (tabPage.FinishComponentContentsLoading)
                                PlaySOP(tabPage, sopTime, strSOPFullPath, dicParameters, dicUserDefinedParameters);
                            else
                            {
                                // ComponentContents가 모두 생성될때까지 기다린다.
                                ArrayList arrDatas = new ArrayList();
                                arrDatas.Add(tabPage);
                                arrDatas.Add(sopTime);
                                arrDatas.Add(strSOPFullPath);
                                arrDatas.Add(dicParameters);
                                arrDatas.Add(dicUserDefinedParameters);

                                Thread t = new Thread(new ParameterizedThreadStart(WaitNPlaySOP));
                                t.Start(arrDatas);
                            }
                        }
                    });
                }
            }
        }

        private void WaitNPlaySOP(object arg)
        {
            ArrayList arrDatas = (ArrayList)arg;

            UnE.SOP.Sections.SectionTabPage tabPage = (UnE.SOP.Sections.SectionTabPage)arrDatas[0];
            DateTime sopTime = (DateTime)arrDatas[1];
            string strSOPFullPath = (string)arrDatas[2];
            Dictionary<string, string> dicParameters = (Dictionary<string, string>)arrDatas[3];
            Dictionary<UnE.SOP.SOPParameter, string> dicUserDefinedParameters = (Dictionary<UnE.SOP.SOPParameter, string>)arrDatas[4];

            // ComponentContents가 모두 생성될때까지 기다린다.
            while (tabPage.FinishComponentContentsLoading == false && FormSOP.Instance.CloseThread == false)
            {
                Thread.Sleep(1000);
            }

            FormSOP.Instance.Invoke((MethodInvoker)delegate
            {
                PlaySOP(tabPage, sopTime, strSOPFullPath, dicParameters, dicUserDefinedParameters);
            });
        }

        // Return 값 : System Parameter
        private Dictionary<string, string> MakeParameterDictionary(List<string> parameters, Dictionary<UnE.SOP.SOPParameter, string> dicUserDefinedParameters)
        {
            Dictionary<string, string> dicParameters = new Dictionary<string, string>();

            foreach (string strParameter in parameters)
            {
                string[] tokens = strParameter.Split(':');

                if (tokens.Count() == 2)
                {
                    string strParameterName = tokens[0].Trim().ToLower();
                    string strParameterValue = tokens[1].Trim();
                    dicParameters[strParameterName] = strParameterValue;
                }
                else if (tokens.Count() > 2)
                {
                    int nIndex = strParameter.IndexOf(':');
                    string strParameterName = strParameter.Substring(0, nIndex).Trim().ToLower();

                    int nIndex2 = strParameter.IndexOf(':', nIndex + 1);
                    string strParameterType = strParameter.Substring(nIndex + 1, nIndex2 - nIndex - 1).Trim();

                    string strParameterValue = strParameter.Substring(nIndex2 + 1).Trim();

                    global::Sections.SectionDataDecision.VariableType type = global::Sections.SectionDataDecision.ToVariableType(strParameterType);

                    UnE.SOP.SOPParameter param = new UnE.SOP.SOPParameter();
                    param.Type = type;
                    param.VariableName = strParameterName.Substring(1, strParameterName.Length - 2);

                    dicUserDefinedParameters[param] = strParameterValue;
                }
            }

            return dicParameters;
        }
    }
}
