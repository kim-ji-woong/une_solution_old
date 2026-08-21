package egovframework.weather.web;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

import egovframework.rte.fdl.property.EgovPropertyService;
import egovframework.rte.ptl.mvc.tags.ui.pagination.PaginationInfo;
import egovframework.weather.service.*;

import javax.annotation.Resource;

import egovframework.weather.service.WeatherDefaultVO;

import org.springframework.stereotype.Controller;
import org.springframework.ui.ModelMap;
import org.springframework.web.bind.annotation.ModelAttribute;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.servlet.ModelAndView;

@Controller
public class WeatherController {
	
	/** realTimeRainFallService */
	@Resource(name = "realTimeRainFallService")
	private RealTimeRainFallService realTimeRainFallService;
	
	/** EgovPropertyService */
	@Resource(name = "propertiesService")
	protected EgovPropertyService propertiesService;
	
	private int m_nRRFPageIndex = 1;
	
	@RequestMapping(value = "/main.do")
	public String showWeatherFrame(@ModelAttribute("searchVO") WeatherDefaultVO searchVO, ModelMap model) throws Exception
	{
		m_nRRFPageIndex = searchVO.getPageIndex();		
		return "/weather/g1Weather";
	}
	
	@RequestMapping(value = "/realTimeRainFallList.do")
	public String selectRealTimeRainFall(@ModelAttribute("searchVO") WeatherDefaultVO searchVO, ModelMap model) throws Exception
	{
		searchVO.setPageIndex(m_nRRFPageIndex);
		
		/** EgovPropertyService.sample */
		searchVO.setPageUnit(propertiesService.getInt("pageUnit"));
		searchVO.setPageSize(propertiesService.getInt("pageSize"));

		/** pageing setting */
		PaginationInfo paginationInfo = new PaginationInfo();
		paginationInfo.setCurrentPageNo(searchVO.getPageIndex());
		paginationInfo.setRecordCountPerPage(searchVO.getPageUnit());
		paginationInfo.setPageSize(searchVO.getPageSize());

		searchVO.setFirstIndex(paginationInfo.getFirstRecordIndex());
		searchVO.setLastIndex(paginationInfo.getLastRecordIndex());
		searchVO.setRecordCountPerPage(paginationInfo.getRecordCountPerPage());
		
		int nCurrentPage = paginationInfo.getCurrentPageNo();
		int nPageSize = paginationInfo.getPageSize();
		int nRecordCountPerPage = paginationInfo.getRecordCountPerPage();
		
		try
		{
			int totCnt = realTimeRainFallService.selectRealTimeRainFallListTotCnt(searchVO);
			paginationInfo.setTotalRecordCount(totCnt);
			
			int from = nRecordCountPerPage * (m_nRRFPageIndex - 1);
			int to = nRecordCountPerPage * m_nRRFPageIndex;
			
			if (totCnt <= to)
				to = totCnt;
			
			List<?> totalRainList = realTimeRainFallService.selectRealTimeRainFallList(searchVO);
			List<?> rainList = totalRainList.subList(from,  to);
			//List<?> rainList = realTimeRainFallService.selectRealTimeRainFallList(searchVO);
			model.addAttribute("resultList", rainList);
		}
		catch (Exception e)
		{
			trace(e.toString());
		}
		
		/*trace("Current Page : " + Integer.toString(nCurrentPage));
		trace("Page Size : " + Integer.toString(nPageSize));
		trace("Record Count per Page : " + Integer.toString(nRecordCountPerPage));*/
		
		model.addAttribute("paginationInfo", paginationInfo);
		
		return "/weather/g1RealTimeRainFall";
	}
	
	@RequestMapping(value = "/downloadExcelRRF.do")
	public ModelAndView downloadExcelRealTimeRainFall(@ModelAttribute("searchVO") WeatherDefaultVO searchVO) throws Exception
	{
		try
		{	
			List<?> totalRainList = realTimeRainFallService.selectRealTimeRainFallList(searchVO);

			Map<String, Object> map = new HashMap<String, Object>();
			map.put("totalRRF", totalRainList);
			
			return new ModelAndView("rrfExcelView", map);
		}
		catch (Exception e)
		{
			trace(e.toString());
		}
		
		return null;
	}
	
	private void trace(String strMessage)
	{
		System.out.println(strMessage);
	}
}
