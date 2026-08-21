package g1Weather.service.impl;

import java.util.List;

import javax.annotation.Resource;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Service;

import egovframework.rte.fdl.cmmn.EgovAbstractServiceImpl;
import g1Weather.service.PageVO;
import g1Weather.service.WaterLevelSumDataService;

@Service("waterLevelSumDataService")
public class WaterLevelSumDataServiceImpl extends EgovAbstractServiceImpl implements WaterLevelSumDataService {
	private static final Logger LOGGER = LoggerFactory.getLogger(WaterLevelSumDataServiceImpl.class);

	/** WaterLevelSumDataDAO */
	// TODO ibatis 사용
	@Resource(name = "waterLevelSumDataDAO")
	private WaterLevelSumDataDAO waterLevelSumDataDAO;

	/**
	 * 수위현황 목록을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 VO
	 * @return 수위현황 목록
	 * @exception Exception
	 */
	@Override
	public List<?> selectWaterLevelSumDataList(PageVO searchVO) throws Exception {
		return waterLevelSumDataDAO.selectWaterLevelSumDataList(searchVO);
	}
	
	/**
	 * 수위현황 총 갯수를 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 VO
	 * @return 수위현황 총 갯수
	 * @exception
	 */
	@Override
	public int selectWaterLevelSumDataListTotCnt(PageVO searchVO) {
		return waterLevelSumDataDAO.selectWaterLevelSumDataListTotCnt(searchVO);
	}
}
