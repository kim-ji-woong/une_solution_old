package g1Weather.service;

import java.util.List;

public interface SearchSnowService {
	/**
	 * 금일 적설량을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 금일 적설량 목록
	 * @exception Exception
	 */
	List<?> selectSearchSnowTodayList(PageVO searchVO) throws Exception;
	
	/**
	 * 월간 적설량을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 월간 적설량 목록
	 * @exception Exception
	 */
	List<?> selectSearchSnowMonthList(PageVO searchVO) throws Exception;
	
	/**
	 * 연간 적설량을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 연간 적설량 목록
	 * @exception Exception
	 */
	List<?> selectSearchSnowYearList(PageVO searchVO) throws Exception;
	
	/**
	 * 기간별 적설량을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 기간별 적설량 목록
	 * @exception Exception
	 */
	List<?> selectSearchSnowPeriodList(PageVO searchVO) throws Exception;
	
	/**
	 * 특정도시의 지역들을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 특정도시의 지역 목록
	 * @exception
	 */
	List<?> selectCityTownList(PageVO searchVO) throws Exception;
}
