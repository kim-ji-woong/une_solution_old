package g1Weather.service.impl;

import java.util.List;

import javax.annotation.Resource;

import org.springframework.stereotype.Repository;

import egovframework.rte.fdl.property.EgovPropertyService;
import egovframework.rte.psl.dataaccess.EgovAbstractDAO;
import g1Weather.service.PageVO;

@Repository("snowSumDataDAO")
public class SnowSumDataDAO extends EgovAbstractDAO {
	/** EgovPropertyService */
	@Resource(name = "propertiesService")
	protected EgovPropertyService propertiesService;
	
	/**
	 * 실시간 강설 목록을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 실시간 강설 목록
	 * @exception Exception
	 */
	public List<?> selectSnowSumDataList(PageVO searchVO) throws Exception {
		int cityCode = propertiesService.getInt("cityCode");
		searchVO.setCityCode(cityCode);
		
		return list("snowSumDataDAO.selectSnowSumDataList", searchVO);
	}
	
	/**
	 * 실시간 강설 총 갯수를 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 Map
	 * @return 실시간 강설 총 갯수
	 * @exception
	 */
	public int selectSnowSumDataListTotCnt(PageVO searchVO) {
		int cityCode = propertiesService.getInt("cityCode");
		searchVO.setCityCode(cityCode);
		
		return (Integer) select("snowSumDataDAO.selectSnowSumDataListTotCnt", searchVO);
	}
}
