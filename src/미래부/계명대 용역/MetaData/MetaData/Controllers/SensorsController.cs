using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MetaData.Models;

namespace MetaData.Controllers
{
    public class SensorsController : ApiController
    {
        // AddSensor
        public HttpResponseMessage Post(Sensor sensor)
        {
            sensor.ID = SensorRepository.LastSensorID;
            SensorRepository.AddSensor(sensor);

            var response = Request.CreateResponse<Sensor>(System.Net.HttpStatusCode.Created, sensor);
            return response;
        }
        
        /*public IEnumerable<Sensor> GetAllSensors()
        {
            return new List<Sensor>(SensorRepository.Sensors.Values);
        }*/

        // GetSensor
        public IHttpActionResult Get(int id)
        {
            Sensor sensor = SensorRepository.GetSensor(id);
            if (sensor == null)
                return NotFound();

            return Ok(sensor);
        }

        // RemoveSensor
        public void Delete(int id)
        {
            SensorRepository.RemoveSensor(id);
        }
    }
}
