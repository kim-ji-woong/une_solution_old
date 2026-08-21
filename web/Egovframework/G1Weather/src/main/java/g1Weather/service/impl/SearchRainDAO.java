package g1Weather.service.impl;

import java.util.List;

import javax.annotation.Resource;

import org.springframework.stereotype.Repository;

import egovframework.rte.fdl.property.EgovPropertyService;
import egovframework.rte.psl.dataaccess.EgovAbstractDAO;
import g1Weather.service.PageVO;

@Repository("searchRainDAO")
public class SearchRainDAO extends EgovAbstractDAO {
	/** EgovPropertyService */
	@Resource(name = "propertiesService")
	protected EgovPropertyService propertiesService;
	
	/**
	 * 금일 강우를 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 금일 강우 목록
	 * @exception Exception
	 */
	public List<?> selectSearchRainTodayList(PageVO searchVO) throws Exception {
		int cityCode = propertiesService.getInt("cityCode");
		searchVO.setCityCode(cityCode);
		
		java.text.SimpleDateFormat formatter = new java.text.SimpleDateFormat("yyyyMMdd");
		String now = formatter.format(new java.util.Date());
		searchVO.setDateCondition(now);
		
		return list("searchRainDAO.selectSearchRainTodayList", searchVO);
	}
	
	/**
	 * 금일 강우 총 갯수를 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 금일 강우 총 갯수
	 * @exception
	 */
	public int selectSearchRainTodayListTotCnt(PageVO searchVO) {
		int cityCode = propertiesService.getInt("cityCode");
		searchVO.setCityCode(cityCode);
		
		java.text.SimpleDateFormat formatter = new java.text.SimpleDateFormat("yyyyMMdd");
		String now = formatter.format(new java.util.Date());
		searchVO.setDateCondition(now);
		
		return (Integer) select("searchRainDAO.selectSearchRainTodayListTotCnt", searchVO);
	}
	
	/**
	 * 월간 강우를 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 월간 강우 목록
	 * @exception Exception
	 */
	public List<?> selectSearchRainMonthList(PageVO searchVO) throws Exception {
		int cityCode = propertiesService.getInt("cityCode");
		searchVO.setCityCode(cityCode);
		
		return list("searchRainDAO.selectSearchRainMonthList", searchVO);
	}
	
	/**
	 * 연간 강우를 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 연간 강우 목록
	 * @exception Exception
	 */
	public List<?> selectSearchRainYearList(PageVO searchVO) throws Exception {
		int cityCode = propertiesService.getInt("cityCode");
		searchVO.setCityCode(cityCode);
		
		return list("searchRainDAO.selectSearchRainYearList", searchVO);
	}
	
	/**
	 * 기간별 강우를 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 기간별 강우 목록
	 * @exception Exception
	 */
	public List<?> selectSearchRainPeriodList(PageVO searchVO) throws Exception {
		int cityCode = propertiesService.getInt("cityCode");
		searchVO.setCityCode(cityCode);
		
		return list("searchRainDAO.selectSearchRainPeriodList", searchVO);
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
		
		return list("searchRainDAO.selectCityTownList", searchVO);
	}
	
	/**
	 * 도시의 이름을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 도시의 이름
	 * @exception
	 */
	public String selectCityName(PageVO searchVO) throws Exception
	{
		int cityCode = propertiesService.getInt("cityCode");
		searchVO.setCityCode(cityCode);
		
		return (String) select("searchRainDAO.selectCityName", searchVO);
	}
}
