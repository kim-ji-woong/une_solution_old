package g1Weather.service;

import java.util.List;

public interface SearchRainService {
	/**
	 * 금일 강우를 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 금일 강우 목록
	 * @exception Exception
	 */
	List<?> selectSearchRainTodayList(PageVO searchVO) throws Exception;
	
	/**
	 * 금일 강우 총 갯수를 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 금일 강우 총 갯수
	 * @exception
	 */
	int selectSearchRainTodayListTotCnt(PageVO searchVO);
	
	/**
	 * 월간 강우를 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 월간강우 목록
	 * @exception Exception
	 */
	List<?> selectSearchRainMonthList(PageVO searchVO) throws Exception;
	
	/**
	 * 연간 강우를 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 연간 강우 목록
	 * @exception Exception
	 */
	List<?> selectSearchRainYearList(PageVO searchVO) throws Exception;
	
	/**
	 * 기간별 강우를 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 기간별 강우 목록
	 * @exception Exception
	 */
	List<?> selectSearchRainPeriodList(PageVO searchVO) throws Exception;
	
	/**
	 * 특정도시의 지역들을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 특정도시의 지역 목록
	 * @exception
	 */
	List<?> selectCityTownList(PageVO searchVO) throws Exception;
	
	/**
	 * 도시의 이름을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 도시의 이름
	 * @exception
	 */
	String selectCityName(PageVO searchVO) throws Exception;
}
