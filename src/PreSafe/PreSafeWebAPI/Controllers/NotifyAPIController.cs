using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApplication1.Controllers
{
    public class NotifyAPIController : ApiController
    {

        /// <summary>
        /// 특정 센서의 관리 경찰서에 출동신호을 보냅니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sid">Sensor ID</param>
        /// <returns>작업이 성공하면 true, 실패하면 false</returns>
        [Route("SendMoveOrder")]
        public bool PostSendMoveOrder(string token, string sid)
        {
            return true;
        }

        /// <summary>
        /// 특정 센서의 관리자에게 Email을 전송합니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sid">Sensor ID</param>
        /// <param name="message">이메일 내용</param>
        /// <returns>작업이 성공하면 true, 실패하면 false</returns>
        [Route("SendEmail")]
        public bool PostSendEmail(string token, string sid, [FromBody]string message)
        {
            return true;
        }

        /// <summary>
        /// 	특정 센서의 관리자에게 Fax를 전송합니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sid">Sensor ID</param>
        /// <param name="message">이메일 내용</param>
        /// <returns>작업이 성공하면 true, 실패하면 false</returns>
        [Route("SendFax")]
        public bool PostSendFax(string token, string sid, [FromBody]string message)
        {
            return true;
        }

        /// <summary>
        /// 특정 센서의 관리 PC에 음성을 출력합니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sid">Sensor ID</param>
        /// <param name="message">TTS 내용</param>
        /// <returns>작업이 성공하면 true, 실패하면 false</returns>
        [Route("SendVoice")]
        public bool PostSendVoice(string token, string sid, [FromBody]string message)
        {
            return true;
        }

        /// <summary>
        /// 특정 센서의 관리자에게 전화를 요청합니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sid">Sensor ID</param>
        /// <returns>작업이 성공하면 true, 실패하면 false</returns>
        [Route("CallPhone")]
        public bool PostCallPhone(string token, string sid)
        {
            return true;
        }


        /// <summary>
        /// 특정 센서의 관리자에게 문자메세지를 전송합니다. 
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sid">Sensor ID</param>
        /// <param name="message">문자 내용</param>
        /// <returns>작업이 성공하면 true, 실패하면 false</returns>
        [Route("SendShortMessage")]
        public bool PostSendShortMessage(string token, string sid, [FromBody]string message)
        {
            return true;
        }
    }
}
