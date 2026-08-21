using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MetaData.Models;

namespace MetaData.Controllers
{
    public class SensorDataController : ApiController
    {
        // SetSensorData
        public HttpResponseMessage Post(SensorData data)
        {
            Sensor sensor = SensorRepository.GetSensor(data.SensorID);
            
            if (sensor == null)
            {
                return Request.CreateResponse<SensorData>(HttpStatusCode.NotFound, data);
            }

            SensorValue value = null;

            if (sensor.SensorDataType == Sensor.DataType.INTEGER)
            {
                int nData;
                if (!int.TryParse(data.Data, out nData))
                {
                    return Request.CreateResponse<SensorData>(HttpStatusCode.NotAcceptable, data);
                }

                data.ID = SensorRepository.LastIntegerValueID;
                value = new SensorValuei(nData);
            }
            else if (sensor.SensorDataType == Sensor.DataType.FLOAT)
            {
                float fData;
                if (!float.TryParse(data.Data, out fData))
                {
                    return Request.CreateResponse<SensorData>(HttpStatusCode.NotAcceptable, data);
                }

                data.ID = SensorRepository.LastFloatValueID;
                value = new SensorValuef(fData);
            }
            else if (sensor.SensorDataType == Sensor.DataType.STRING)
            {
                data.ID = SensorRepository.LastStringValueID;
                value = new SensorValues(data.Data);
            }
            else
                return Request.CreateResponse<SensorData>(HttpStatusCode.NotAcceptable, data);

            value.ID = data.ID;
            value.Description = data.Description;
            value.Latitude = data.Latitude;
            value.Longitude = data.Longitude;
            value.Sensor = sensor;
            value.Time = data.Time;

            SensorRepository.AddSensorValue(sensor, value);
            
            var response = Request.CreateResponse<SensorData>(System.Net.HttpStatusCode.Created, data);
            return response;
        }

        // GetSensorData
        public IHttpActionResult Get(int id)
        {
            SensorValue value = SensorRepository.GetSensorValue(id);

            if (value == null)
                return NotFound();

            return Ok(value);
        }
    }
}
