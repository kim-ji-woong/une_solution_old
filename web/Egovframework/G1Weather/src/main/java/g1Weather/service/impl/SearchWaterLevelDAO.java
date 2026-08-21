package g1Weather.service.impl;

import java.util.List;

import javax.annotation.Resource;

import org.springframework.stereotype.Repository;

import egovframework.rte.fdl.property.EgovPropertyService;
import egovframework.rte.psl.dataaccess.EgovAbstractDAO;
import g1Weather.service.PageVO;

@Repository("searchWaterLevelDAO")
public class SearchWaterLevelDAO extends EgovAbstractDAO {
	/** EgovPropertyService */
	@Resource(name = "propertiesService")
	protected EgovPropertyService propertiesService;
	
	/**
	 * 일일 수위를 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 일일 수위 목록
	 * @exception Exception
	 */
	public List<?> selectSearchWaterLevelDayList(PageVO searchVO) throws Exception {
		int cityCode = propertiesService.getInt("cityCode");
		searchVO.setCityCode(cityCode);
		
		return list("searchWaterLevelDAO.selectSearchWaterLevelDayList", searchVO);
	}
	
	/**
	 * 기간별 수위를 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 기간별 수위 목록
	 * @exception Exception
	 */
	public List<?> selectSearchWaterLevelPeriodList(PageVO searchVO) throws Exception {
		int cityCode = propertiesService.getInt("cityCode");
		searchVO.setCityCode(cityCode);
		
		return list("searchWaterLevelDAO.selectSearchWaterLevelPeriodList", searchVO);
	}
	
	/**
	 * 특정도시의 지역들을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 특정도시의 지역 목록
	 * @exception
	 */
	public List<?> selectCityTownList(PageVO searchVO) throws Exception
	{
		int cityCode = propertiesService.getInt("cityCode");
		searchVO.setCityCode(cityCode);
		
		return list("searchWaterLevelDAO.selectCityTownList", searchVO);
	}
}
