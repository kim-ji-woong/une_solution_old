using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;
using CrisisAlertAPI.BLL.Models;
using dnsDBUtil;
using CrisisAlertAPI.BLL;
using CrisisAlertAPI.BLL.Response;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CrisisAlertAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SMSController : ControllerBase
    {
        private SMSManager m_smsManager = null;

        public SMSController(WebDBManager dbManager)
        {
            m_smsManager = new SMSManager(dbManager);
        }

        // GET: api/<SMSController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<SMSController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<SMSController>
        [HttpPost]
        /*
        public void Post([FromBody] string value)
        {
        }
        */
        public IActionResult Post(SmsParameter param)
        {
            MessageResult result = new MessageResult();

            if (param.Caller == null || param.Message == null || param.PhoneNumbers == null)
            {
                result.Success = false;
                result.Message = "제대로 된 데이터가 아닙니다.";
                return Ok(result);
            }
               
            result = m_smsManager.SendSMS(param);

            return Ok(result);
        }

        // PUT api/<SMSController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SMSController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        
    }
}
