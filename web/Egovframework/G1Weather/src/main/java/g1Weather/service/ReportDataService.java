package g1Weather.service;

import java.util.List;

public interface ReportDataService {
	/**
	 * 보고서 출력용 기간별 강우량을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 VO
	 * @return 기간별 강우량
	 * @exception Exception
	 */
	List<?> selectReportRainDataPeriodList(PageVO searchVO) throws Exception; 
	
	/**
	 * 보고서 출력용 기간별 적설량을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 VO
	 * @return 기간별 적설량
	 * @exception Exception
	 */
	List<?> selectReportSnowDataPeriodList(PageVO searchVO) throws Exception;
}
