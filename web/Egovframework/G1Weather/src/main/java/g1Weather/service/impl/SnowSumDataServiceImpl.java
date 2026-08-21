package g1Weather.service.impl;

import java.util.List;

import javax.annotation.Resource;

import egovframework.rte.fdl.cmmn.EgovAbstractServiceImpl;
import g1Weather.service.PageVO;
import g1Weather.service.SnowSumDataService;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Service;

@Service("snowSumDataService")
public class SnowSumDataServiceImpl extends EgovAbstractServiceImpl implements SnowSumDataService {
	private static final Logger LOGGER = LoggerFactory.getLogger(SnowSumDataServiceImpl.class);

	/** SnowSumDataDAO */
	// TODO ibatis 사용
	@Resource(name = "snowSumDataDAO")
	private SnowSumDataDAO snowSumDataDAO;
	// TODO mybatis 사용
	//  @Resource(name="snowSumDataMapper")
	//	private SnowSumDataMapper snowSumDataDAO;

	/**
	 * 실시간 강설 목록을 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 VO
	 * @return 실시간 강설 목록
	 * @exception Exception
	 */
	@Override
	public List<?> selectSnowSumDataList(PageVO searchVO) throws Exception {
		return snowSumDataDAO.selectSnowSumDataList(searchVO);
	}
	
	/**
	 * 실시간 강설 총 갯수를 조회한다.
	 * @param searchVO - 조회할 정보가 담긴 VO
	 * @return 실시간 강설 총 갯수
	 * @exception
	 */
	@Override
	public int selectSnowSumDataListTotCnt(PageVO searchVO) {
		return snowSumDataDAO.selectSnowSumDataListTotCnt(searchVO);
	}
}
