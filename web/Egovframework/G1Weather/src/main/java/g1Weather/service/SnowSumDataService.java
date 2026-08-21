package g1Weather.service;

import java.util.List;

public interface SnowSumDataService {
	/**
	 * 실시간 강설 목록을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 VO
	 * @return 실시간 강설 목록
	 * @exception Exception
	 */
	List<?> selectSnowSumDataList(PageVO searchVO) throws Exception;
	
	/**
	 * 실시간 강설 총 갯수를 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 VO
	 * @return 실시간 강설 총 갯수
	 * @exception
	 */
	int selectSnowSumDataListTotCnt(PageVO searchVO);
}
