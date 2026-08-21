package g1Weather.service.impl;

import java.util.List;

import javax.annotation.Resource;

import org.springframework.stereotype.Repository;

import egovframework.rte.fdl.property.EgovPropertyService;
import egovframework.rte.psl.dataaccess.EgovAbstractDAO;
import g1Weather.service.PageVO;

@Repository("waterLevelSumDataDAO")
public class WaterLevelSumDataDAO extends EgovAbstractDAO {
	/** EgovPropertyService */
	@Resource(name = "propertiesService")
	protected EgovPropertyService propertiesService;
	
	/**
	 * 수위현황 목록을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 수위현황 목록
	 * @exception Exception
	 */
	public List<?> selectWaterLevelSumDataList(PageVO searchVO) throws Exception {
		int cityCode = propertiesService.getInt("cityCode");
		searchVO.setCityCode(cityCode);
		
		return list("waterLevelSumDataDAO.selectWaterLevelSumDataList", searchVO);
	}
	
	/**
	 * 수위현황 총 갯수를 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 수위현황 총 갯수
	 * @exception
	 */
	public int selectWaterLevelSumDataListTotCnt(PageVO searchVO) {
		int cityCode = propertiesService.getInt("cityCode");
		searchVO.setCityCode(cityCode);
		
		return (Integer) select("waterLevelSumDataDAO.selectWaterLevelSumDataListTotCnt", searchVO);
	}
}
