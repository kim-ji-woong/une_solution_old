package g1Weather.service.impl;

import java.util.List;

import javax.annotation.Resource;

import g1Weather.service.PageVO;
import egovframework.rte.fdl.property.EgovPropertyService;
import egovframework.rte.psl.dataaccess.EgovAbstractDAO;

import org.springframework.stereotype.Repository;

@Repository("realTimeRainFallDAO")
public class RealTimeRainFallDAO extends EgovAbstractDAO {
	/** EgovPropertyService */
	@Resource(name = "propertiesService")
	protected EgovPropertyService propertiesService;
	
	/**
	 * 실시간 강우 목록을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 실시간 강우 목록
	 * @exception Exception
	 */
	public List<?> selectRealTimeRainFallList(PageVO searchVO) throws Exception {
		int cityCode = propertiesService.getInt("cityCode");
		searchVO.setCityCode(cityCode);
		
		return list("realTimeRainFallDAO.selectRealTimeRainFallList", searchVO);
	}
	
	/**
	 * 실시간 강우 총 갯수를 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 실시간 강우 총 갯수
	 * @exception
	 */
	public int selectRealTimeRainFallListTotCnt(PageVO searchVO) {
		int cityCode = propertiesService.getInt("cityCode");
		searchVO.setCityCode(cityCode);
		
		return (Integer) select("realTimeRainFallDAO.selectRealTimeRainFallListTotCnt", searchVO);
	}
}
