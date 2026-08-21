using dnsCommunicateSopServer;
using dnsData.Sensor;
using SDMS.BLL.Models.Request;
using SDMS.IDAL;
using SDMS.Model.CCTV;
using SDMS.Model.Sensor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SDMS.BLL
{
    /// <summary>
    /// 알람 처리 (오작동, 상황 전파)
    /// </summary>
    public class AlarmManager
    {
        private IDataManager m_dataManager = null;
        private ProcessManager m_processManager = null;

        private SopQueryManager m_sopQueryManager = null;

        public AlarmManager(IDataManager dataManager, ProcessManager processManager)
        {
            this.m_dataManager = dataManager;
            this.m_processManager = processManager;
        }

        /// <summary>
        /// 알람 오작동
        /// </summary>
        public void Malfunction(RequestMalfunction data)
        {
            try
            {
                string strErrorMessage = null;
                
                Dictionary<TagInfo.Fields, object> dicCondition = new Dictionary<TagInfo.Fields, object>();
                dicCondition.Add(TagInfo.Fields.SensorZoneID, data.SensorZoneID);
                List<TagInfo> tagInfo = m_dataManager.GetSelectManager().SelectSensorTagInfo(dicCondition, "", out strErrorMessage);
                if (tagInfo == null || tagInfo.Count == 0 || tagInfo[0].TagNo < 0)
                    return;

                m_sopQueryManager = new SopQueryManager();

                bool isMalfunction = data.IsMalfunction;
                
                if (Facility.IsFireSensorType(Facility.ToFacilityType(data.SensorType)))
                {
                    ArrayList arrDatas = new ArrayList();
                    arrDatas.Add(data.SensorType);
                    arrDatas.Add(tagInfo[0].TagNo);
                    arrDatas.Add(data.SensorZoneID);
                    arrDatas.Add(false);

                    m_sopQueryManager.SendAlarmMalfunctionQuery(isMalfunction, arrDatas, "POST", m_processManager.SOPWebServerURL + "/api/FireSensor");
                }
                else if (Facility.IsPSMSensorType(Facility.ToFacilityType(data.SensorType)))
                {
                    ArrayList arrDatas = new ArrayList();
                    arrDatas.Add(data.SensorZoneID);
                    arrDatas.Add(data.AccessedUserID);

                    m_sopQueryManager.SendAlarmUserResetQuery(!isMalfunction, arrDatas, "POST", m_processManager.SOPWebServerURL + "/api/PSMSensor");
                }
                else if (Facility.IsETCSensorType(Facility.ToFacilityType(data.SensorType)))
                {
                    if (m_dataManager.SiteID == 11)
                    {
                        ArrayList arrDatas = new ArrayList();
                        arrDatas.Add(data.SensorType);
                        arrDatas.Add(tagInfo[0].TagNo);
                        arrDatas.Add(data.SensorZoneID);
                        arrDatas.Add("");
                        arrDatas.Add(""); //arrDatas.Add(cctv.UniqueKey);
                        arrDatas.Add(DateTime.Now);
                        arrDatas.Add(-1);
                        arrDatas.Add("");

                        m_sopQueryManager.SendAlarmMalfunctionQueryNST(isMalfunction, arrDatas, "POST", m_processManager.SOPWebServerURL + "/api/EtcSensor");
                    }
                    else
                    {
                        ArrayList arrDatas = new ArrayList();
                        arrDatas.Add(data.SensorType);
                        arrDatas.Add(tagInfo[0].TagNo);
                        arrDatas.Add(data.SensorZoneID);
                        arrDatas.Add(false);

                        m_sopQueryManager.SendAlarmMalfunctionQuery(isMalfunction, arrDatas, "POST", m_processManager.SOPWebServerURL + "/api/EtcSensor");
                    }
                }
                else if (Facility.IsSVMSSensorType(Facility.ToFacilityType(data.SensorType)))
                {
                    ArrayList arrDatas = new ArrayList();
                    arrDatas.Add(data.SensorType);
                    arrDatas.Add(tagInfo[0].TagNo);
                    arrDatas.Add(data.SensorZoneID);
                    arrDatas.Add(false);

                    m_sopQueryManager.SendAlarmMalfunctionQuery(isMalfunction, arrDatas, "POST", m_processManager.SOPWebServerURL + "/api/FireSensor");
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        public void SituationNotice(RequestSituationNotice data)
        {
            try
            {
                m_sopQueryManager = new SopQueryManager(m_processManager.SOPWebServerURL + "/api/Sop");

                ArrayList arrDatas = new ArrayList();
                arrDatas.Add(0); // 0: 화재
                arrDatas.Add(data.SensorZoneID);

                m_sopQueryManager.SendSituationNotice(arrDatas, "POST");
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        public bool ManualReport(RequestManualReport data)
        {
            try
            {
                string url = m_processManager.SOPWebServerURL + "/api";
                Facility.FacilityType facilityType = Facility.ToFacilityType(data.SensorType);
                if (Facility.IsFireSensorType(facilityType))
                    url += "/FireSensor";
                else if (Facility.IsPSMSensorType(facilityType))
                    url += "/PSMSensor";
                else if (Facility.IsETCSensorType(facilityType))
                    url += "/EtcSensor";
                else
                    return false;

                m_sopQueryManager = new SopQueryManager(url);

                DateTime dt;
                if (!DateTime.TryParse(data.DateTime, out dt))
                    return false;

                ArrayList arrDatas = new ArrayList();
                arrDatas.Add(data.SensorType);
                arrDatas.Add(data.SensorZoneID);
                arrDatas.Add(data.ZoneID);
                arrDatas.Add(dt);
                arrDatas.Add(data.AlarmDepth);
                arrDatas.Add(data.ReportPerson);
                arrDatas.Add(data.Memo);

                bool result = m_sopQueryManager.SendManualReport(arrDatas, "POST");
                return result;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return false;
            }
        }

        public bool ClearManualReport(RequestClearManualReport data)
        {
            try
            {
                string url = m_processManager.SOPWebServerURL + "/api";
                Facility.FacilityType facilityType = Facility.ToFacilityType(data.SensorType);
                if (Facility.IsFireSensorType(facilityType))
                    url += "/FireSensor";
                else if (Facility.IsPSMSensorType(facilityType))
                    url += "/PSMSensor";
                else if (Facility.IsETCSensorType(facilityType))
                    url += "/EtcSensor";
                else
                    return false;

                m_sopQueryManager = new SopQueryManager(url);

                ArrayList arrDatas = new ArrayList();
                arrDatas.Add(data.SensorType);
                arrDatas.Add(data.SensorZoneID);
                arrDatas.Add(data.SensorZoneHistoryID);
                arrDatas.Add(data.AccessedUserID);

                bool result = m_sopQueryManager.SendClearManualReport(arrDatas, "POST");
                return result;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
