package g1Weather.service.impl;

import java.util.List;

import javax.annotation.Resource;

import egovframework.rte.fdl.cmmn.EgovAbstractServiceImpl;
import g1Weather.service.PageVO;
import g1Weather.service.SearchWaterLevelService;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Service;

@Service("searchWaterLevelService")
public class SearchWaterLevelServiceImpl extends EgovAbstractServiceImpl implements SearchWaterLevelService {
	private static final Logger LOGGER = LoggerFactory.getLogger(SearchWaterLevelServiceImpl.class);

	/** SearchWaterLevelDAO */
	// TODO ibatis 사용
	@Resource(name = "searchWaterLevelDAO")
	private SearchWaterLevelDAO searchWaterLevelDAO;
	
	/**
	 * 일일 수위를 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 일일 수위 목록
	 * @exception Exception
	 */
	@Override
	public List<?> selectSearchWaterLevelDayList(PageVO searchVO) throws Exception
	{
		return searchWaterLevelDAO.selectSearchWaterLevelDayList(searchVO);
	}
	
	/**
	 * 기간별 수위를 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 기간별 수위 목록
	 * @exception Exception
	 */
	@Override
	public List<?> selectSearchWaterLevelPeriodList(PageVO searchVO) throws Exception
	{
		return searchWaterLevelDAO.selectSearchWaterLevelPeriodList(searchVO);
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
		return searchWaterLevelDAO.selectCityTownList(searchVO);
	}
}
