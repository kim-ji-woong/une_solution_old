package egovframework.weather.service.impl;

import java.util.List;

import egovframework.weather.service.WeatherDefaultVO;
import egovframework.rte.psl.dataaccess.EgovAbstractDAO;

import org.springframework.stereotype.Repository;

@Repository("realTimeRainFallDAO")
public class RealTimeRainFallDAO extends EgovAbstractDAO {
	/**
	 * 실시간 강수 목록을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 실시간 강수 목록
	 * @exception Exception
	 */
	public List<?> selectRealTimeRainFallList(WeatherDefaultVO searchVO) throws Exception {
		return list("realTimeRainFallDAO.selectRealTimeRainFallList", searchVO);
	}
	
	/**
	 * 실시간 강수 총 갯수를 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 실시간 강수 총 갯수
	 * @exception
	 */
	public int selectRealTimeRainFallListTotCnt(WeatherDefaultVO searchVO) {
		return (Integer) select("realTimeRainFallDAO.selectRealTimeRainFallListTotCnt", searchVO);
	}
}
