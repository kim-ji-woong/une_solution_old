package g1Weather.service.impl;

import java.util.List;

import javax.annotation.Resource;

import egovframework.rte.fdl.cmmn.EgovAbstractServiceImpl;
import g1Weather.service.PageVO;
import g1Weather.service.ReportDataService;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Service;

@Service("reportDataService")
public class ReportDataServiceImpl extends EgovAbstractServiceImpl implements ReportDataService {
	private static final Logger LOGGER = LoggerFactory.getLogger(ReportDataServiceImpl.class);

	/** ReportDataDAO */
	// TODO ibatis 사용
	@Resource(name = "reportDataDAO")
	private ReportDataDAO reportDataDAO;
	
	/**
	 * 보고서 출력용 기간별 강우량을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 VO
	 * @return 기간별 강우량
	 * @exception Exception
	 */
	public List<?> selectReportRainDataPeriodList(PageVO searchVO) throws Exception
	{
		return reportDataDAO.selectReportRainDataPeriodList(searchVO);
	}
	
	/**
	 * 보고서 출력용 기간별 적설량을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 VO
	 * @return 기간별 적설량
	 * @exception Exception
	 */
	public List<?> selectReportSnowDataPeriodList(PageVO searchVO) throws Exception
	{
		return reportDataDAO.selectReportSnowDataPeriodList(searchVO);
	} 
}
