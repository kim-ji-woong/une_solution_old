package g1Weather.service.impl;

import java.util.List;

import javax.annotation.Resource;

import org.springframework.stereotype.Repository;

import egovframework.rte.fdl.property.EgovPropertyService;
import egovframework.rte.psl.dataaccess.EgovAbstractDAO;
import g1Weather.service.PageVO;

@Repository("searchSnowDAO")
public class SearchSnowDAO extends EgovAbstractDAO {
	/** EgovPropertyService */
	@Resource(name = "propertiesService")
	protected EgovPropertyService propertiesService;
	
	/**
	 * 금일 적설량을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 금일 적설량 목록
	 * @exception Exception
	 */
	public List<?> selectSearchSnowTodayList(PageVO searchVO) throws Exception {
		int cityCode = propertiesService.getInt("cityCode");
		searchVO.setCityCode(cityCode);
		
		java.text.SimpleDateFormat formatter = new java.text.SimpleDateFormat("yyyyMMdd");
		String now = formatter.format(new java.util.Date());
		searchVO.setDateCondition(now);
		
		return list("searchSnowDAO.selectSearchSnowTodayList", searchVO);
	}
	
	/**
	 * 월간 적설량을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 월간 적설량 목록
	 * @exception Exception
	 */
	public List<?> selectSearchSnowMonthList(PageVO searchVO) throws Exception {
		int cityCode = propertiesService.getInt("cityCode");
		searchVO.setCityCode(cityCode);
		
		return list("searchSnowDAO.selectSearchSnowMonthList", searchVO);
	}
	
	/**
	 * 연간 적설량을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 연간 적설량 목록
	 * @exception Exception
	 */
	public List<?> selectSearchSnowYearList(PageVO searchVO) throws Exception {
		int cityCode = propertiesService.getInt("cityCode");
		searchVO.setCityCode(cityCode);
		
		return list("searchSnowDAO.selectSearchSnowYearList", searchVO);
	}
	
	/**
	 * 기간별 적설량을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 기간별 적설량 목록
	 * @exception Exception
	 */
	public List<?> selectSearchSnowPeriodList(PageVO searchVO) throws Exception {
		int cityCode = propertiesService.getInt("cityCode");
		searchVO.setCityCode(cityCode);
		
		return list("searchSnowDAO.selectSearchSnowPeriodList", searchVO);
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
		
		return list("searchSnowDAO.selectCityTownList", searchVO);
	}
}
