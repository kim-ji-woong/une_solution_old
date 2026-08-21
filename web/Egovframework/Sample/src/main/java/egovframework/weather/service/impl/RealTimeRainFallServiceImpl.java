package egovframework.weather.service.impl;

import java.util.List;

import javax.annotation.Resource;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Service;

import egovframework.weather.service.RealTimeRainFallService;
import egovframework.weather.service.WeatherDefaultVO;
import egovframework.rte.fdl.cmmn.EgovAbstractServiceImpl;

@Service("realTimeRainFallService")
public class RealTimeRainFallServiceImpl extends EgovAbstractServiceImpl implements RealTimeRainFallService {
	private static final Logger LOGGER = LoggerFactory.getLogger(RealTimeRainFallServiceImpl.class);

	/** RealTimeRainFallDAO */
	// TODO ibatis 사용
	@Resource(name = "realTimeRainFallDAO")
	private RealTimeRainFallDAO realTimeRainFallDAO;
	// TODO mybatis 사용
	//  @Resource(name="realTimeRainFallMapper")
	//	private RealTimeRainFallMapper realTimeRainFallDAO;

	/**
	 * 실시간 강수 목록을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 VO
	 * @return 실시간 강수 목록
	 * @exception Exception
	 */
	@Override
	public List<?> selectRealTimeRainFallList(WeatherDefaultVO searchVO) throws Exception {
		return realTimeRainFallDAO.selectRealTimeRainFallList(searchVO);
	}
	
	/**
	 * 실시간 강수 총 갯수를 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 VO
	 * @return 실시간 강수 총 갯수
	 * @exception
	 */
	@Override
	public int selectRealTimeRainFallListTotCnt(WeatherDefaultVO searchVO) {
		return realTimeRainFallDAO.selectRealTimeRainFallListTotCnt(searchVO);
	}
}
