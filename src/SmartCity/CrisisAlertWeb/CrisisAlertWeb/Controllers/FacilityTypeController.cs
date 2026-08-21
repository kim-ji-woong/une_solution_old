using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartCity.BLL.Models.Request;
using SmartCity.BLL.Models.Response;
using SmartCity.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CrisisAlertWeb.Controllers
{
    public class FacilityTypeController : Controller
    {
        private global::SmartCity.BLL.ProcessManager m_processManager = null;
        public FacilityTypeController(global::SmartCity.IDAL.IDataManager dataManager)
        {
            m_processManager = new global::SmartCity.BLL.ProcessManager(dataManager);
        }

        [HttpPost]
        public IActionResult RequestData([FromBody] RequestData data)
        {
            if (data == null)
                return BadRequest();

            if (data.RequestFirstSensor != null)
                return RequestFirstSensor(data.RequestFirstSensor);
            else if (data.RequestSensorInfo != null)
                return RequestSensorInfo(data.RequestSensorInfo);
            else if (data.RequestFacilityTypeSensors != null)
                return RequestFacilityTypeSensors(data.RequestFacilityTypeSensors);
            else if (data.RequestAlarmList != null)
                return RequestAlarmList(data.RequestAlarmList);
            else if (data.RequestManualList != null)
                return RequestManualList(data.RequestManualList);

            return null;
        }

        private IActionResult RequestFirstSensor(RequestFirstSensor data)
        {
            ResponseSensorInfo result = null;

            if (data.FacilityType != -1)
            {
                try
                {
                    int nFacilityType = data.FacilityType;
                    result = m_processManager.GetLoadManager().GetFirstSensor(nFacilityType);
                }
                catch (Exception e)
                {
                    result = new ResponseSensorInfo();
                    result.Message = e.Message;
                    result.Success = false;
                }
            }
            else
            {
                result = new ResponseSensorInfo();
                result.Success = false;
            }

            return Ok(result);
        }

        private IActionResult RequestSensorInfo(RequestSensorInfo data)
        {
            ResponseSensorInfo result = null;

            if (data.ID != -1 && data.FacilityType != -1)
            {
                try
                {
                    int nID = data.ID;
                    int nFacilityType = data.FacilityType;
                    result = m_processManager.GetLoadManager().GetSensorInfo(nID, nFacilityType);
                }
                catch (Exception e)
                {
                    result = new ResponseSensorInfo();
                    result.Message = e.Message;
                    result.Success = false;
                }
            }
            else
            {
                result = new ResponseSensorInfo();
                result.Success = false;
            }

            return Ok(result);
        }

        private IActionResult RequestAlarmList(RequestAlarmList data)
        {
            ResponseAlarmList result = null;

            if (data.FacilityType != -1)
            {
                try
                {
                    int nFacilityType = data.FacilityType;

                    result = m_processManager.GetLoadManager().GetAlarmList(nFacilityType);
                }
                catch (Exception e)
                {
                    result = new ResponseAlarmList();
                    result.Message = e.Message;
                    result.Success = false;
                }
            }
            else
            {
                result = new ResponseAlarmList();
                result.Success = false;
            }

            return Ok(result);
        }

        private IActionResult RequestManualList(RequestManualList data)
        {
            ResponseManualList result = null;

            if (data.FacilityType != -1)
            {
                try
                {
                    int nFacilityType = data.FacilityType;

                    result = m_processManager.GetLoadManager().GetManualList(nFacilityType);
                }
                catch (Exception e)
                {
                    result = new ResponseManualList();
                    result.Message = e.Message;
                    result.Success = false;
                }
            }
            else
            {
                result = new ResponseManualList();
                result.Success = false;
            }

            return Ok(result);
        }

        private IActionResult RequestFacilityTypeSensors(RequestFacilityTypeSensors data)
        {
            ResponseFacilityTypeSensors result = null;

            if (data.FacilityType != -1)
            {
                try
                {
                    int nFacilityType = data.FacilityType;
                    List<Sensor> listSensor = new List<Sensor>();

                    if (nFacilityType == (int)FacilityType.FIRE_SENSOR)
                    {
                         Dictionary<int, SmartCity.Model.FireSensor> dicFireSensors = m_processManager.GetLoadManager().FireSensors;

                        foreach (KeyValuePair<int, SmartCity.Model.FireSensor> pair in dicFireSensors)
                        {
                            SmartCity.Model.FireSensor fire = pair.Value;

                            Sensor sensor = new Sensor();
                            sensor.ID = fire.ID;
                            sensor.SensorID = fire.SensorID;
                            sensor.Addr = fire.Addr;
                            sensor.State = fire.State;

                            listSensor.Add(sensor);
                        }
                    }
                    else if (nFacilityType == (int)FacilityType.FLOOD_SENSOR)
                    {
                        Dictionary<int, SmartCity.Model.FloodSensor> dicFloodSensors = m_processManager.GetLoadManager().FloodSensors;

                        foreach (KeyValuePair<int, SmartCity.Model.FloodSensor> pair in dicFloodSensors)
                        {
                            SmartCity.Model.FloodSensor flood = pair.Value;

                            Sensor sensor = new Sensor();
                            sensor.ID = flood.ID;
                            sensor.SensorID = flood.SensorID;
                            sensor.Addr = flood.Addr;
                            sensor.State = flood.State;

                            listSensor.Add(sensor);
                        }
                    }
                    else if (nFacilityType == (int)FacilityType.HEAT_SENSOR)
                    {
                        Dictionary<int, SmartCity.Model.HeatSensor> dicHeatSensors = m_processManager.GetLoadManager().HeatSensors;

                        foreach (KeyValuePair<int, SmartCity.Model.HeatSensor> pair in dicHeatSensors)
                        {
                            SmartCity.Model.HeatSensor heat = pair.Value;

                            Sensor sensor = new Sensor();
                            sensor.ID = heat.ID;
                            sensor.SensorID = heat.SensorID;
                            sensor.Addr = heat.Addr;
                            sensor.State = heat.State;

                            listSensor.Add(sensor);
                        }
                    }
                    else if (nFacilityType == (int)FacilityType.COLLAPSE_SENSOR)
                    {
                        Dictionary<int, SmartCity.Model.CollapseSensor> dicFCollapseSensors = m_processManager.GetLoadManager().CollapseSensors;

                        foreach (KeyValuePair<int, SmartCity.Model.CollapseSensor> pair in dicFCollapseSensors)
                        {
                            SmartCity.Model.CollapseSensor collapse = pair.Value;

                            Sensor sensor = new Sensor();
                            sensor.ID = collapse.ID;
                            sensor.SensorID = collapse.SensorID;
                            sensor.Addr = collapse.Addr;
                            sensor.State = collapse.State;

                            listSensor.Add(sensor);
                        }
                    }

                    result = new ResponseFacilityTypeSensors();

                    result.FacilityTypeSensors = listSensor;
                    result.Success = true;
                }
                catch (Exception e)
                {
                    result = new ResponseFacilityTypeSensors();
                    result.Message = e.Message;
                    result.Success = false;
                }
            }
            else
            {
                result = new ResponseFacilityTypeSensors();
                result.Message = "센서 타입이 잘못 전달 되었습니다.";
                result.Success = false;
            }

            return Ok(result);
        }
    }
}
