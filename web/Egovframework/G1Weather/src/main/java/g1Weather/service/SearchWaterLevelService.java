package g1Weather.service;

import java.util.List;

public interface SearchWaterLevelService {
	/**
	 * 일일 수위를 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 일일 수위 목록
	 * @exception Exception
	 */
	List<?> selectSearchWaterLevelDayList(PageVO searchVO) throws Exception;
	
	/**
	 * 기간별 수위를 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 기간별 수위 목록
	 * @exception Exception
	 */
	List<?> selectSearchWaterLevelPeriodList(PageVO searchVO) throws Exception;
	
	/**
	 * 특정도시의 지역들을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 특정도시의 지역 목록
	 * @exception
	 */
	List<?> selectCityTownList(PageVO searchVO) throws Exception;
}
