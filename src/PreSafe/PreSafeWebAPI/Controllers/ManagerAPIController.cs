using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApplication1.Controllers
{
    public class ManagerAPIController : ApiController
    {
        /// <summary>
        /// API를 실행하기 위한 Token을 생성하거나 가져옵니다.
        /// </summary>
        /// <param name="UserID">사용할 ID</param>
        /// <param name="Password">AES암호화된 비밀번호</param>
        /// <returns>Access Token</returns>
        [Route("GetAccessToken")]       
        public string PostAccessToken(string UserID, string Password)
        {
            return "value";
        }

        /// <summary>
        /// 특정 센서의 ID를 가져옵니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="uid">TargetUserID</param>
        /// <returns>AES 암호화된 SensorID</returns>
        [Route("GetSensorID")]
        public string PostSensorID(string token, string uid)
        {
            return "value";
        }

        /// <summary>
        /// 특정 센서 착용자의 ID를 가져옵니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="registrationNumber">AES 암호화된 주민번호</param>
        /// <param name="name">AES 암호화된 착용자 이름</param>
        /// <returns>AES 암호화된 TargetUserID</returns>
        [Route("GetTargetUserID")]
        public string PostAccessToken(string token, string registrationNumber, string name)
        {
            return "value";
        }

        /// <summary>
        /// 모든 센서 착용자의 정보를 가져옵니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <returns>AES 암호화된 SensorID</returns>
        [Route("GetAllTargetUsers")]
        public string[] PostAllTargetUsers(string token)
        {
            string [] a = new string[2];
            a[0] = "value1";
            a[1] = "value2";
            return a;
        }

        /// <summary>
        /// 모든 센서 ID를 가져 옵니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <returns>AES 암호화된 SensorID</returns>
        [Route("GetAllSensorIDs")]
        public string[] PostAllSensorIDs(string token)
        {
            string [] a = new string[2];
            a[0] = "value1";
            a[1] = "value2";
            return a;
        }

        /// <summary>
        /// 특정 착용자의 센서 ID를 변경합니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="uid">TargetUserID</param>
        /// <param name="string">SensorID</param>
        /// <returns>AES 암호화된 TargetUserID</returns>
        [Route("SetSensorToUser")]
        public string PostSensorToUser(string token, string uid, string sid)
        {
            return "value";
        }
    }
}
