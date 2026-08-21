using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Web;

namespace WebApplication1.Controllers
{
    public class SenarioAPIController : ApiController
    {
        /// <summary>
        /// 새로운 시나리오를 추가합니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sname">Senario 이름</param>
        /// <param name="conetent">시나리오 파일의 Binary Steam</param>
        /// <returns>작업이 성공하면 true, 실패하면 false</returns>
        [Route("AddSenario")]
        public bool PostAddSenario(string token, string sname, [FromBody]HttpPostedFile conetent)
        {
            return true;
        }

        /// <summary>
        /// 기존의 시나리오를 삭제합니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sname">Senario 이름</param>
        /// <returns>작업이 성공하면 true, 실패하면 false</returns>
        [Route("DeleteSenario")]
        public bool PostDeleteSenario(string token, string sname)
        {
            return true;
        }

        /// <summary>
        /// 기존의 시나리오를 사용가능 상태로 변경합니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sname">Senario 이름</param>
        /// <returns>작업이 성공하면 true, 실패하면 false</returns>
        [Route("EnableSenario")]
        public bool PostEnableSenario(string token, string sname)
        {
            return true;
        }


        /// <summary>
        /// 기존의 시나리오를 사용불가능 상태로 변경합니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sname">Senario 이름</param>
        /// <returns>작업이 성공하면 true, 실패하면 false</returns>
        [Route("DisableSenario")]
        public bool PostDisableSenario(string token, string sname)
        {
            return true;
        }

        /// <summary>
        /// 기존의 시나리오를 덮어씁니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sname">Senario 이름</param>
        /// <param name="conetent">시나리오 파일</param>
        /// <returns>작업이 성공하면 true, 실패하면 false</returns>
        [Route("UpdateSenario")]
        public bool PostUpdateSenario(string token, string sname, [FromBody]HttpPostedFile conetent)
        {
            return true;
        }

        /// <summary>
        /// 기존의 시나리오를 다운로드 받습니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sname">Senario 이름</param>
        /// <returns>작업이 성공하면 true, 실패하면 false</returns>
        [Route("GetSenario")]
        public HttpResponseMessage PostSenario(string token, string sname)
        {
            HttpResponseMessage result = null;
            var localFilePath = HttpContext.Current.Server.MapPath("App_Data/XmlDocument.xml");
            if (!File.Exists(localFilePath))
            {
                result = Request.CreateResponse(HttpStatusCode.Gone);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.OK);
                result.Content = new StreamContent(new FileStream(localFilePath, FileMode.Open, FileAccess.Read));
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = "XmlDocument.xml";
            }
            return result;

        }
    }
}
