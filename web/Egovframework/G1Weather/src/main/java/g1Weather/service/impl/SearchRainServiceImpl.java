package g1Weather.service.impl;

import java.util.List;

import javax.annotation.Resource;

import egovframework.rte.fdl.cmmn.EgovAbstractServiceImpl;
import g1Weather.service.PageVO;
import g1Weather.service.SearchRainService;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Service;

@Service("searchRainService")
public class SearchRainServiceImpl extends EgovAbstractServiceImpl implements SearchRainService {
	private static final Logger LOGGER = LoggerFactory.getLogger(SearchRainServiceImpl.class);

	/** SearchRainDAO */
	// TODO ibatis 사용
	@Resource(name = "searchRainDAO")
	private SearchRainDAO searchRainDAO;
	
	/**
	 * 금일 강우를 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 금일 강우 목록
	 * @exception Exception
	 */
	@Override
	public List<?> selectSearchRainTodayList(PageVO searchVO) throws Exception
	{
		return searchRainDAO.selectSearchRainTodayList(searchVO);
	}
	
	/**
	 * 금일 강우 총 갯수를 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 금일 강우 총 갯수
	 * @exception
	 */
	@Override
	public int selectSearchRainTodayListTotCnt(PageVO searchVO)
	{
		return searchRainDAO.selectSearchRainTodayListTotCnt(searchVO);
	}
	
	/**
	 * 월간 강우를 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 월간 강우 목록
	 * @exception Exception
	 */
	@Override
	public List<?> selectSearchRainMonthList(PageVO searchVO) throws Exception
	{
		return searchRainDAO.selectSearchRainMonthList(searchVO);
	}
	
	/**
	 * 연간 강우를 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 연간 강우 목록
	 * @exception Exception
	 */
	@Override
	public List<?> selectSearchRainYearList(PageVO searchVO) throws Exception
	{
		return searchRainDAO.selectSearchRainYearList(searchVO);
	}
	
	/**
	 * 기간별 강우를 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 기간별 강우 목록
	 * @exception Exception
	 */
	@Override
	public List<?> selectSearchRainPeriodList(PageVO searchVO) throws Exception
	{
		return searchRainDAO.selectSearchRainPeriodList(searchVO);
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
		return searchRainDAO.selectCityTownList(searchVO);
	}
	
	/**
	 * 도시의 이름을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 도시의 이름
	 * @exception
	 */
	@Override
	public String selectCityName(PageVO searchVO) throws Exception
	{
		return searchRainDAO.selectCityName(searchVO);
	}
}
