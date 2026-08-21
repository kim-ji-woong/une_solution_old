using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApplication1.Controllers
{
    public class StatisticAPIController : ApiController
    {
        /// <summary>
        /// 지정된 시간동안 특정 센서의 범죄율 통계를 가져옵니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sid">조회할 Sensor ID</param>
        /// <param name="begin">조회시작일자</param>
        /// <param name="end">조회종료일자</param>
        /// <returns>평균 범죄율(%)</returns>
        [Route("GetStsCrimeRate")]
        public double PostStsCrimeRate(string token, string sid, DateTime begin, DateTime end)
        {
            return 0.0;
        }

        /// <summary>
        /// 지정된 시간동안 특정 범죄율 이상의 기록이 있는 센서 사용자를 가져옵니다.	
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="rate">범죄율(%)</param>
        /// <param name="begin">조회시작일자</param>
        /// <param name="end">조회종료일자</param>
        /// <returns>사용자ID 리스트</returns>
        [Route("GetStsAllCrimeRateUsers")]
        public string[] PostStsAllCrimeRateUsers(string token, double rate, DateTime begin, DateTime end)
        {
            string[] a = new string[2];
            a[0] = "AA";
            a[1] = "bB";
            return a;
        }

       
        /// <summary>
        /// 지정된 시간동안 접근 금지 구역 위반 내역이 있는 센서 사용자를 가져옵니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="begin">조회시작일자</param>
        /// <param name="end">조회종료일자</param>
        /// <returns>사용자ID 리스트</returns>
        [Route("GetStsAllViolationUsers")]
        public string[] PostStsAllViolationUsers(string token, DateTime begin, DateTime end)
        {
            string[] a = new string[2];
            a[0] = "AA";
            a[1] = "bB";
            return a;
        }

        /// <summary>
        /// 지정된 시간동안 특정 센서 사용자의 접근 금지 구역 위반 횟수를 가져옵니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sid">조회할 Sensor ID</param>
        /// <param name="begin">조회시작일자</param>
        /// <param name="end">조회종료일자</param>
        /// <returns>위반 횟수</returns>
        [Route("GetStsViolationCountForID")]
        public int PostStsViolationCountForID(string token, string sid, DateTime begin, DateTime end)
        {
            return 0;
        }

        /// <summary>
        /// 지정된 시간동안 특정 센서 사용자의 알코올 수치를 가져옵니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sid">조회할 Sensor ID</param>
        /// <param name="begin">조회시작일자</param>
        /// <param name="end">조회종료일자</param>
        /// <returns>알콜 수치(%) 리스트</returns>
        [Route("GetStsAlcoholeDensityForID")]
        public double[] PostStsAlcoholeDensityForID(string token, string sid, DateTime begin, DateTime end)
        {
            double[] a = new double[2];
            a[0] = 0.0;
            a[1] = 0.0;
            return a;
        }

        /// <summary>
        /// 지정된 시간동안 특정 센서 사용자의 충격 회수를 가져옵니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sid">조회할 Sensor ID</param>
        /// <param name="begin">조회시작일자</param>
        /// <param name="end">조회종료일자</param>
        /// <returns>충격 횟수</returns>
        [Route("GetStsImportCountForID")]
        public int PostStsImportCountForID(string token, string sid, DateTime begin, DateTime end)
        {
            return 0;
        }

        /// <summary>
        /// 지정된 시간동안 특정 센서 사용자의 가속도를 가져옵니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sid">조회할 Sensor ID</param>
        /// <param name="begin">조회시작일자</param>
        /// <param name="end">조회종료일자</param>
        /// <returns>알콜 수치(%) 리스트</returns>
        [Route("GetStsAcceleationForID")]
        public double[] PostStsAcceleationForID(string token, string sid, DateTime begin, DateTime end)
        {
            double[] a = new double[2];
            a[0] = 0.0;
            a[1] = 0.0;
            return a;
        }

        /// <summary>
        /// 지정된 시간동안 특정 센서 사용자의 소음 수치를 가져옵니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sid">조회할 Sensor ID</param>
        /// <param name="begin">조회시작일자</param>
        /// <param name="end">조회종료일자</param>
        /// <returns>소음 수치(dB) 리스트</returns>
        [Route("GetStsSoundLevelForID")]
        public double[] PostStsSoundLevelForID(string token, string sid, DateTime begin, DateTime end)
        {
            double[] a = new double[2];
            a[0] = 0.0;
            a[1] = 0.0;
            return a;
        }

        /// <summary>
        /// 지정된 시간동안 알코올 수치가 범위 이상인 사용자를 가져옵니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="alocholeDensity">알콜수치(%)</param>
        /// <param name="begin">조회시작일자</param>
        /// <param name="end">조회종료일자</param>
        /// <returns>사용자ID 리스트</returns>
        [Route("GetStsAlocholeDensity")]
        public string[] PostStsAlocholeDensity(string token, double alocholeDensity, DateTime begin, DateTime end)
        {
            string[] a = new string[2];
            a[0] = "AA";
            a[1] = "bB";
            return a;
        }

        /// <summary>
        /// 지정된 시간동안 충격 회수가 범위 이상인 사용자를 가져옵니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="impactCount">충격횟수</param>
        /// <param name="begin">조회시작일자</param>
        /// <param name="end">조회종료일자</param>
        /// <returns>사용자ID 리스트</returns>
        [Route("GetStsImpactCount")]
        public string[] PostStsImpactCount(string token, int impactCount, DateTime begin, DateTime end)
        {
            string[] a = new string[2];
            a[0] = "AA";
            a[1] = "bB";
            return a;
        }

        /// <summary>
        /// 지정된 시간동안 충격 회수가 범위 이상인 사용자를 가져옵니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="violation">위반횟수</param>
        /// <param name="begin">조회시작일자</param>
        /// <param name="end">조회종료일자</param>
        /// <returns>사용자ID 리스트</returns>
        [Route("GetStsViolationCount")]
        public string[] PostStsViolationCount(string token, int violation, DateTime begin, DateTime end)
        {
            string[] a = new string[2];
            a[0] = "AA";
            a[1] = "bB";
            return a;
        }

    }
}
