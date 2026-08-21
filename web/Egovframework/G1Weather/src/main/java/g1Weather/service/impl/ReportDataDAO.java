package g1Weather.service.impl;

import java.util.List;

import javax.annotation.Resource;

import org.springframework.stereotype.Repository;

import egovframework.rte.fdl.property.EgovPropertyService;
import egovframework.rte.psl.dataaccess.EgovAbstractDAO;
import g1Weather.service.PageVO;

@Repository("reportDataDAO")
public class ReportDataDAO extends EgovAbstractDAO {
	/** EgovPropertyService */
	@Resource(name = "propertiesService")
	protected EgovPropertyService propertiesService;
	
	/**
	 * 보고서 출력용 기간별 강우량을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 VO
	 * @return 기간별 강우량
	 * @exception Exception
	 */
	public List<?> selectReportRainDataPeriodList(PageVO searchVO) throws Exception {
		int cityCode = propertiesService.getInt("cityCode");
		searchVO.setCityCode(cityCode);
		
		return list("reportDataDAO.selectReportRainDataPeriodList", searchVO);
	} 
	
	/**
	 * 보고서 출력용 기간별 적설량을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 VO
	 * @return 기간별 적설량
	 * @exception Exception
	 */
	List<?> selectReportSnowDataPeriodList(PageVO searchVO) throws Exception {
		int cityCode = propertiesService.getInt("cityCode");
		searchVO.setCityCode(cityCode);
		
		return list("reportDataDAO.selectReportSnowDataPeriodList", searchVO);
	}
}
