package g1Weather.service.impl;

import java.util.List;

import javax.annotation.Resource;

import egovframework.rte.fdl.cmmn.EgovAbstractServiceImpl;
import g1Weather.service.PageVO;
import g1Weather.service.SearchSnowService;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Service;

@Service("searchSnowService")
public class SearchSnowServiceImpl extends EgovAbstractServiceImpl implements SearchSnowService {
	private static final Logger LOGGER = LoggerFactory.getLogger(SearchSnowServiceImpl.class);

	/** SearchSnowDAO */
	// TODO ibatis 사용
	@Resource(name = "searchSnowDAO")
	private SearchSnowDAO searchSnowDAO;
	
	/**
	 * 금일 적설량을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 금일 적설량 목록
	 * @exception Exception
	 */
	@Override
	public List<?> selectSearchSnowTodayList(PageVO searchVO) throws Exception
	{
		return searchSnowDAO.selectSearchSnowTodayList(searchVO);
	}
		
	/**
	 * 월간 적설량을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 월간 적설량 목록
	 * @exception Exception
	 */
	@Override
	public List<?> selectSearchSnowMonthList(PageVO searchVO) throws Exception
	{
		return searchSnowDAO.selectSearchSnowMonthList(searchVO);
	}
	
	/**
	 * 연간 적설량을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 연간 적설량 목록
	 * @exception Exception
	 */
	@Override
	public List<?> selectSearchSnowYearList(PageVO searchVO) throws Exception
	{
		return searchSnowDAO.selectSearchSnowYearList(searchVO);
	}
	
	/**
	 * 기간별 적설량을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 기간별 적설량 목록
	 * @exception Exception
	 */
	@Override
	public List<?> selectSearchSnowPeriodList(PageVO searchVO) throws Exception
	{
		return searchSnowDAO.selectSearchSnowPeriodList(searchVO);
	}
	
	/**
	 * 특정도시의 지역들을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 특정도시의 지역 목록
	 * @exception
	 */
	@Override
	public List<?> selectCityTownList(PageVO searchVO) throws Exception
	{
		return searchSnowDAO.selectCityTownList(searchVO);
	}
}
