package egovframework.weather.service;

import java.util.List;

import egovframework.weather.service.WeatherDefaultVO;

public interface RealTimeRainFallService {
	/**
	 * 실시간 강수 목록을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 VO
	 * @return 실시간 강수 목록
	 * @exception Exception
	 */
	List<?> selectRealTimeRainFallList(WeatherDefaultVO searchVO) throws Exception;
	
	/**
	 * 실시간 강수 총 갯수를 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 VO
	 * @return 실시간 강수 총 갯수
	 * @exception
	 */
	int selectRealTimeRainFallListTotCnt(WeatherDefaultVO searchVO);
}
