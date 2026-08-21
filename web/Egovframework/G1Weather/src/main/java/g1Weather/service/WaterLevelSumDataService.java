package g1Weather.service;

import java.util.List;

public interface WaterLevelSumDataService {
	/**
	 * 수위현황 목록을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 VO
	 * @return 수위현황 목록
	 * @exception Exception
	 */
	List<?> selectWaterLevelSumDataList(PageVO searchVO) throws Exception;
	
	/**
	 * 수위현황 총 갯수를 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 VO
	 * @return 수위현황 총 갯수
	 * @exception
	 */
	int selectWaterLevelSumDataListTotCnt(PageVO searchVO);
}
