using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApplication1.Controllers
{
    public class SensorAPIController : ApiController
    {
        /// <summary>
        /// 특정 센서의 심장 박동수를 가져옵니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sid">조회할 Sensor ID</param>
        /// <returns>심장 박동수(회/분)</returns>
        [Route("GetHeartBeat")]
        public int PostHeartBeat(string token, string sid)
        {
            return 0;
        }

        /// <summary>
        /// 특정 센서의 알코올 수치를 가져옵니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sid">조회할 Sensor ID</param>
        /// <returns>알콜수치(%)</returns>
        [Route("GetAlcoholDensity")]
        public double PostAlcoholDensity(string token, string sid)
        {
            return 0.0;
        }

        /// <summary>
        /// 특정 센서 소음 정도를 가져옵니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sid">조회할 Sensor ID</param>
        /// <returns>소음크기(dB)</returns>
        [Route("GetSoundLevel")]
        public double PostSoundLevel(string token, string sid)
        {
            return 0.0;
        }

        /// <summary>
        /// 특정 센서의 체온을 가져옵니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sid">조회할 Sensor ID</param>
        /// <returns>체온(Cº)</returns>
        [Route("GetBodyHeat")]
        public double PostBodyHeat(string token, string sid)
        {
            return 0.0;
        }

        /// <summary>
        /// 특정 센서의 위치를 가져옵니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sid">조회할 Sensor ID</param>
        /// <returns>현재위치</returns>
        [Route("GetLocation")]
        public string PostLocation(string token, string sid)
        {
            return "xy";
        }

        /// <summary>
        /// 특정 센서의 가속도를 가져옵니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sid">조회할 Sensor ID</param>
        /// <returns>가속도(m/s2)</returns>
        [Route("GetAcceleration")]
        public double PostAcceleration(string token, string sid)
        {
            return 0.0;
        }

        /// <summary>
        /// 특정 센서의 충격 여부를 가져옵니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sid">조회할 Sensor ID</param>
        /// <returns>1분이내 충격있는경우 true, 없는경우 false</returns>
        [Route("GetImpact")]
        public bool PosImpact(string token, string sid)
        {
            return false;
        }
        
        /////////////////////////////////////////////////////////////

        /// <summary>
        /// 일정 시간동안 특정 센서의 최대 심장 박동수를 가져옵니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sid">조회할 Sensor ID</param>
        /// <param name="period">입력된 시간(분) 이전부터</param>
        /// <returns>심장 박동수(회/분)</returns>
        [Route("GetPeakHeartBeat")]
        public int PostPeakHeartBeat(string token, string sid, int period)
        {
            return 0;
        }

        /// <summary>
        /// 일정 시간동안 특정 센서의 최대 알코올 수치를 가져옵니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sid">조회할 Sensor ID</param>
        /// <param name="period">입력된 시간(분) 이전부터</param>
        /// <returns>알콜수치(%)</returns>
        [Route("GetPeakAlcoholDensity")]
        public double PostPeakAlcoholDensity(string token, string sid, int period)
        {
            return 0.0;
        }

        /// <summary>
        /// 일정 시간동안 특정 센서 최대 소음 정도를 가져옵니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sid">조회할 Sensor ID</param>
        /// <param name="period">입력된 시간(분) 이전부터</param>
        /// <returns>소음크기(dB)</returns>
        [Route("GetPeakSoundLevel")]
        public double PostPeakSoundLevel(string token, string sid, int period)
        {
            return 0.0;
        }

        /// <summary>
        /// 일정 시간동안 특정 센서의 최대 체온을 가져옵니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sid">조회할 Sensor ID</param>
        /// <param name="period">입력된 시간(분) 이전부터</param>
        /// <returns>체온(Cº)</returns>
        [Route("GetPeakBodyHeat")]
        public double PostPeakBodyHeat(string token, string sid, int period)
        {
            return 0.0;
        }


        /// <summary>
        /// 일정 시간동안 특정 센서의 최대 가속도를 가져옵니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sid">조회할 Sensor ID</param>
        /// <param name="period">입력된 시간(분) 이전부터</param>
        /// <returns>가속도(m/s2)</returns>
        [Route("GetPeakAcceleration")]
        public double PostPeakAcceleration(string token, string sid, int period)
        {
            return 0.0;
        }

        /// <summary>
        /// 일정 시간동안 특정 센서의 충격 여부를 가져옵니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sid">조회할 Sensor ID</param>
        /// <param name="period">입력된 시간(분) 이전부터</param>
        /// <returns>일정시간안에 충격있는경우 true, 없는경우 false</returns>
        [Route("GetImpactPeriod")]
        public bool PostImpactPeriod(string token, string sid, int period)
        {
            return false;
        }


        // <summary>
        /// 일정 시간동안 특정 센서의 최소 심장 박동수를 가져옵니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sid">조회할 Sensor ID</param>
        /// <param name="period">입력된 시간(분) 이전부터</param>
        /// <returns>심장 박동수(회/분)</returns>
        [Route("GetLeastHeartBeat")]
        public int PostLeastHeartBeat(string token, string sid, int period)
        {
            return 0;
        }

        /// <summary>
        /// 일정 시간동안 특정 센서의 최소 알코올 수치를 가져옵니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sid">조회할 Sensor ID</param>
        /// <param name="period">입력된 시간(분) 이전부터</param>
        /// <returns>알콜수치(%)</returns>
        [Route("GetLeastAlcoholDensity")]
        public double PostLeastAlcoholDensity(string token, string sid, int period)
        {
            return 0.0;
        }

        /// <summary>
        /// 일정 시간동안 특정 센서 최소 소음 정도를 가져옵니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sid">조회할 Sensor ID</param>
        /// <param name="period">입력된 시간(분) 이전부터</param>
        /// <returns>소음크기(dB)</returns>
        [Route("GetLeastSoundLevel")]
        public double PostLeastSoundLevel(string token, string sid, int period)
        {
            return 0.0;
        }

        /// <summary>
        /// 일정 시간동안 특정 센서의 최소 체온을 가져옵니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sid">조회할 Sensor ID</param>
        /// <param name="period">입력된 시간(분) 이전부터</param>
        /// <returns>체온(Cº)</returns>
        [Route("GetLeastBodyHeat")]
        public double PostLeastBodyHeat(string token, string sid, int period)
        {
            return 0.0;
        }

        /// <summary>
        /// 일정 시간동안 특정 센서의 최소 가속도를 가져옵니다.
        /// </summary>
        /// <param name="token">AccessToken</param>
        /// <param name="sid">조회할 Sensor ID</param>
        /// <param name="period">입력된 시간(분) 이전부터</param>
        /// <returns>가속도(m/s2)</returns>
        [Route("GetLeastAcceleration")]
        public double PostLeastAcceleration(string token, string sid, int period)
        {
            return 0.0;
        }
    }
}
