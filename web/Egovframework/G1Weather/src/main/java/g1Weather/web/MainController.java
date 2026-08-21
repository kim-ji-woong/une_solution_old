package g1Weather.web;

import java.sql.Date;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.GregorianCalendar;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import javax.annotation.Resource;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import org.springframework.stereotype.Controller;
import org.springframework.ui.ModelMap;
import org.springframework.web.bind.annotation.ModelAttribute;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.servlet.ModelAndView;

import egovframework.rte.fdl.property.EgovPropertyService;
import egovframework.rte.ptl.mvc.tags.ui.pagination.PaginationInfo;
import g1Weather.service.RealTimeRainFallService;
import g1Weather.service.ReportData;
import g1Weather.service.ReportDataService;
import g1Weather.service.SearchRain;
import g1Weather.service.SearchRain.RainData;
import g1Weather.service.SearchSnow;
import g1Weather.service.SearchSnowService;
import g1Weather.service.SearchWaterLevelService;
import g1Weather.service.SnowSumData;
import g1Weather.service.SnowSumDataService;
import g1Weather.service.WaterLevelSumDataService;
import g1Weather.service.SearchRainService;
import g1Weather.service.PageVO;
import g1Weather.service.SearchRainOption.SearchRainMonth;
import g1Weather.service.SearchRainOption.SearchRainPeriod;
import g1Weather.service.SearchRainOption.SearchRainToday;
import g1Weather.service.SearchRainOption.SearchRainYear;
import g1Weather.service.SearchSnowOption.SearchSnowMonth;
import g1Weather.service.SearchSnowOption.SearchSnowPeriod;
import g1Weather.service.SearchSnowOption.SearchSnowToday;
import g1Weather.service.SearchSnowOption.SearchSnowYear;
import g1Weather.service.SearchWaterLevelOption.SearchWaterLevelDay;
import g1Weather.service.SearchWaterLevelOption.SearchWaterLevelPeriod;
import g1Weather.service.data.CityTown;
import g1Weather.web.helper.SearchHelper;
import g1Weather.webService.RadarImageURLParser;
import g1Weather.webService.SpecialNews;
import g1Weather.webService.SpecialNewsXMLParser;
import g1Weather.common.*;

@Controller
public class MainController {
	private int m_nRRFPageIndex = 1;
	private final int MAX_ITEM_COUNT_PER_PAGE = 10000; 
	
	private String m_strPrevSearchRainOption = "today";
	private String m_strPrevSearchSnowOption = "today";
	private String m_strPrevSearchWaterLevelOption = "day";
	private String m_strPrevSearchReportOption = "rain";
	
	private List<MenuItem> m_menus = new ArrayList();
	private MenuItem m_currentMenu = null;
	
	/** realTimeRainFallService */
	@Resource(name = "realTimeRainFallService")
	private RealTimeRainFallService realTimeRainFallService;
	
	/** snowSumDataService */
	@Resource(name = "snowSumDataService")
	private SnowSumDataService snowSumDataService;
	
	/** waterLevelSumDataService */
	@Resource(name = "waterLevelSumDataService")
	private WaterLevelSumDataService waterLevelSumDataService;
	
	/** searchRainService */
	@Resource(name = "searchRainService")
	private SearchRainService searchRainService;
	
	/** searchSnowService */
	@Resource(name = "searchSnowService")
	private SearchSnowService searchSnowService;
	
	/** searchWaterLevelService */
	@Resource(name = "searchWaterLevelService")
	private SearchWaterLevelService searchWaterLevelService;
	
	/** reportDataService */
	@Resource(name = "reportDataService")
	private ReportDataService reportDataService;
	
	/** EgovPropertyService */
	@Resource(name = "propertiesService")
	protected EgovPropertyService propertiesService;
	
	public MainController()
	{ 
		MenuItem menu = new MenuItem("메인화면", "mainPageList.do", "mainPage.do");
		menu.setFirstMenu(true);
		SetCurrentMenu(menu, null);
		m_menus.add(menu);
		
		menu = new MenuItem("강우현황", "realTimeRainFallList.do", "rainList.do");
		m_menus.add(menu);
		
		menu = new MenuItem("적설현황", "snowSumDataList.do", "snowList.do");
		m_menus.add(menu);
		
		menu = new MenuItem("수위현황", "waterLevelSumDataList.do", "waterLevelList.do");
		m_menus.add(menu);
		
		menu = new MenuItem("강우조회", "searchRainDataList.do", "searchRainList.do");
		m_menus.add(menu);
		
		menu = new MenuItem("적설조회", "searchSnowDataList.do", "searchSnowList.do");
		m_menus.add(menu);
		
		menu = new MenuItem("수위조회", "searchWaterLevelDataList.do", "searchWaterLevelList.do");
		m_menus.add(menu);
		
		menu = new MenuItem("보고서출력", "printReportDataList.do", "printReportList.do");
		m_menus.add(menu);
		
		menu = new MenuItem("CCTV영상", "showCCTV.do", "showCCTV.do");
		m_menus.add(menu);
	}
	
	private void SetMenuVisible(int nMenuIndex, String menuString)
	{
		int visibleMenu = propertiesService.getInt(menuString);
		
		if (visibleMenu == 0)
			m_menus.get(nMenuIndex).setVisible(false);
		else
			m_menus.get(nMenuIndex).setVisible(true);
	}
	
	private void SetTitleNVersion(ModelMap model)
	{
		if (propertiesService != null)
		{
			String title = propertiesService.getString("Title");
			String appName = propertiesService.getString("AppName");
			
			if (title != null)
				model.addAttribute("Title", title);
			else
				model.addAttribute("Title", "강원 기상 웹");
			
			if (appName != null)
				model.addAttribute("AppName", appName);
			else
				model.addAttribute("AppName", "기상통합 웹조회 - v1.0.0");
		}
	}
	
	private void SetFirstSearchYear(ModelMap model)
	{
		if (propertiesService != null)
		{
			int nFirstSearchYear = propertiesService.getInt("FirstSearchYear");
			
			if (nFirstSearchYear > 1900)
				model.addAttribute("FirstSearchYear", nFirstSearchYear);
			else
				model.addAttribute("FirstSearchYear", 1980);
		}
	}
	
	private void SetCurrentMenu(MenuItem menu, ModelMap model)
	{
		if (propertiesService != null)
		{
			SetMenuVisible(0, "메인화면");
			SetMenuVisible(1, "강우현황");
			SetMenuVisible(2, "적설현황");
			SetMenuVisible(3, "수위현황");
			SetMenuVisible(4, "강우조회");
			SetMenuVisible(5, "적설조회");
			SetMenuVisible(6, "수위조회");
			SetMenuVisible(7, "보고서출력");
			SetMenuVisible(8, "CCTVOption");
						  			
			MenuItem firstMenu = null;
			
			for (MenuItem menuItem : m_menus)
			{
				if (menuItem.getVisible())
				{
					if (firstMenu == null)
					{
						firstMenu = menuItem;
						menuItem.setFirstMenu(true);
					}
					else
						menuItem.setFirstMenu(false);
				}
			}
		}
		
		if (menu.getVisible() == false)
		{
			for (MenuItem menuItem : m_menus)
			{
				if (menuItem.getVisible())
				{
					menu = menuItem;
					break;
				}
			}
		}
		
		if (m_currentMenu != menu)
		{
			if (m_currentMenu != null)
				m_currentMenu.setSelected(false);
			
			if (menu != null)
				menu.setSelected(true);
			
			m_currentMenu = menu;
		}
		
		if (model != null)
		{
			if (m_currentMenu == null)
				model.remove("currentMenu");
			else
				model.addAttribute("currentMenu", m_currentMenu);
		}
		
		SetTitleNVersion(model);
		SetFirstSearchYear(model);
	}
	 
	@RequestMapping(value = "/mainPageList.do")
	public String selectMainPage(@ModelAttribute("searchVO") PageVO searchVO, ModelMap model, HttpServletRequest request) throws Exception
	{
		m_nRRFPageIndex = searchVO.getPageIndex(); //
		//searchVO.setPageIndex(m_nRRFPageIndex);
		
		// EgovPropertyService.sample
		searchVO.setPageUnit(MAX_ITEM_COUNT_PER_PAGE);
		//searchVO.setPageUnit(propertiesService.getInt("pageUnit"));
		searchVO.setPageSize(propertiesService.getInt("pageSize"));
		
		int autoUpdateSeconds = propertiesService.getInt("AutoUpdateTime");
		model.addAttribute("AutoUpdateTime", "" + autoUpdateSeconds);

		// pageing setting
		PaginationInfo paginationInfo = new PaginationInfo();
		paginationInfo.setCurrentPageNo(searchVO.getPageIndex());
		paginationInfo.setRecordCountPerPage(searchVO.getPageUnit());
		paginationInfo.setPageSize(searchVO.getPageSize());

		searchVO.setFirstIndex(paginationInfo.getFirstRecordIndex());
		searchVO.setLastIndex(paginationInfo.getLastRecordIndex());
		searchVO.setRecordCountPerPage(paginationInfo.getRecordCountPerPage());
		
		int nCurrentPage = paginationInfo.getCurrentPageNo();
		int nPageSize = paginationInfo.getPageSize();
		int nRecordCountPerPage = paginationInfo.getRecordCountPerPage(); //
		
		try
		{
			HttpSession session = request.getSession();
			String cityName = searchRainService.selectCityName(searchVO);
			session.setAttribute("cityName", cityName);
			session.setAttribute("excelView", "mainPageExcelView");
			
			//강우현황
			int totCnt = realTimeRainFallService.selectRealTimeRainFallListTotCnt(searchVO);
			paginationInfo.setTotalRecordCount(totCnt);
			
			int from = nRecordCountPerPage * (m_nRRFPageIndex - 1);
			int to = nRecordCountPerPage * m_nRRFPageIndex;
			
			if (totCnt <= to)
				to = totCnt;
			
			List<?> totalRainList = realTimeRainFallService.selectRealTimeRainFallList(searchVO);
			List<?> rainList = totalRainList.subList(from,  to);
			//List<?> rainList = realTimeRainFallService.selectRealTimeRainFallList(searchVO);
			model.addAttribute("rainResultList", rainList);
			session.setAttribute("rainResultList", rainList);
			
			//특보리스트, 레이더 이미지
			String serviceKey = propertiesService.getString("SNewsServiceKey");
			
			String urlString = "http://newsky2.kma.go.kr/service/WetherSpcnwsInfoService/SpecialNewsCode?ServiceKey=";
			urlString += serviceKey + "&areaCode=&warninType=&numOfRows=999&pageNo=1";			
			//test
			//String urlString = "http://newsky2.kma.go.kr/service/WetherSpcnwsInfoService/SpecialNewsCode?ServiceKey=6lIn11vBfAG7cH0zgE5Skyqh5bClzO%2Fhi%2Fub12cL6TvJdmp6QC4QQW0vbhzWUK%2B7OEVfqQ92SI9MNfHophhP4g%3D%3D&areaCode=&warninType=&numOfRows=999&pageNo=1&fromTmFc=20170101";
			
			java.net.URL url = new java.net.URL(urlString);
			java.net.HttpURLConnection connection = (java.net.HttpURLConnection)url.openConnection();
			
			//connection.setRequestProperty("Content-Type", "application/json");
			//connection.setRequestProperty("Accept", "application/json");
			//connection.setRequestMethod("GET");
			
            java.io.InputStream is = connection.getInputStream();
            java.util.Scanner scan = new java.util.Scanner(is);
            
            String result = "";
            int line = 1;
            while (scan.hasNext())
            {
            	String str = scan.nextLine();
            	result += str;
            }
            scan.close();
            is.close();
            
            List<SpecialNews> specialNewsList = SpecialNewsXMLParser.parse(result);
            
            if (specialNewsList != null)
            {
            	model.addAttribute("newsList", specialNewsList);
            	session.setAttribute("newsList", specialNewsList);
            }
            
            String radarImageURL = RadarImageURLParser.getCurrentURL(serviceKey);
            model.addAttribute("radarImageURL", radarImageURL); 			
            session.setAttribute("radarImageURL", radarImageURL);
             
			MenuItem menu = (MenuItem)m_menus.get(0);
			SetCurrentMenu(menu, model);
		}
		catch (Exception e)
		{
			//trace(e.toString());
		}
		
		/*trace("Current Page : " + Integer.toString(nCurrentPage));
		trace("Page Size : " + Integer.toString(nPageSize));
		trace("Record Count per Page : " + Integer.toString(nRecordCountPerPage));*/
		
		model.addAttribute("paginationInfo", paginationInfo);
		
		return "/weather/mainPage";
	}
	
	@RequestMapping(value = "/realTimeRainFallList.do")
	public String selectRealTimeRainFall(@ModelAttribute("searchVO") PageVO searchVO, ModelMap model) throws Exception
	{
		m_nRRFPageIndex = searchVO.getPageIndex();
		//searchVO.setPageIndex(m_nRRFPageIndex);
		
		// EgovPropertyService.sample
		searchVO.setPageUnit(MAX_ITEM_COUNT_PER_PAGE);
		//searchVO.setPageUnit(propertiesService.getInt("pageUnit"));
		searchVO.setPageSize(propertiesService.getInt("pageSize"));
		
		int autoUpdateSeconds = propertiesService.getInt("AutoUpdateTime");
		model.addAttribute("AutoUpdateTime", "" + autoUpdateSeconds);

		// pageing setting
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
			
			MenuItem menu = (MenuItem)m_menus.get(1);
			SetCurrentMenu(menu, model);
		}
		catch (Exception e)
		{
			//trace(e.toString());
		}
		
		/*trace("Current Page : " + Integer.toString(nCurrentPage));
		trace("Page Size : " + Integer.toString(nPageSize));
		trace("Record Count per Page : " + Integer.toString(nRecordCountPerPage));*/
		
		model.addAttribute("paginationInfo", paginationInfo);
		
		return "/weather/realTimeRainFall";
	}
	
	@RequestMapping(value = "/snowSumDataList.do")
	public String selectSnowSumData(@ModelAttribute("searchVO") PageVO searchVO, ModelMap model) throws Exception
	{
		m_nRRFPageIndex = searchVO.getPageIndex();
		//searchVO.setPageIndex(m_nRRFPageIndex);
		
		/** EgovPropertyService.sample */
		searchVO.setPageUnit(MAX_ITEM_COUNT_PER_PAGE);
		//searchVO.setPageUnit(propertiesService.getInt("pageUnit"));
		searchVO.setPageSize(propertiesService.getInt("pageSize"));
		
		int autoUpdateSeconds = propertiesService.getInt("AutoUpdateTime");
		model.addAttribute("AutoUpdateTime", "" + autoUpdateSeconds);

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
			List<?> totalSnowList = snowSumDataService.selectSnowSumDataList(searchVO);
			int totCnt = totalSnowList.size();
			
			//int totCnt = snowSumDataService.selectSnowSumDataListTotCnt(searchVO);
			paginationInfo.setTotalRecordCount(totCnt);
			
			int from = nRecordCountPerPage * (m_nRRFPageIndex - 1);
			int to = nRecordCountPerPage * m_nRRFPageIndex;
			
			if (totCnt <= to)
				to = totCnt;
			
			//List<?> totalSnowList = snowSumDataService.selectSnowSumDataList(searchVO);
			List<?> snowList = totalSnowList.subList(from,  to);
			model.addAttribute("resultList", snowList);
			
			MenuItem menu = (MenuItem)m_menus.get(2);
			SetCurrentMenu(menu, model);
		}
		catch (Exception e)
		{
			//trace(e.toString());
		}
		
		model.addAttribute("paginationInfo", paginationInfo);
		
		return "/weather/snowSumData";
	}
	
	@RequestMapping(value = "/waterLevelSumDataList.do")
	public String selectWaterLevelSumData(@ModelAttribute("searchVO") PageVO searchVO, ModelMap model) throws Exception
	{
		m_nRRFPageIndex = searchVO.getPageIndex();
		//searchVO.setPageIndex(m_nRRFPageIndex);
		
		/** EgovPropertyService.sample */
		searchVO.setPageUnit(MAX_ITEM_COUNT_PER_PAGE);
		//searchVO.setPageUnit(propertiesService.getInt("pageUnit"));
		searchVO.setPageSize(propertiesService.getInt("pageSize"));
		
		int autoUpdateSeconds = propertiesService.getInt("AutoUpdateTime");
		model.addAttribute("AutoUpdateTime", "" + autoUpdateSeconds);

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
			int totCnt = waterLevelSumDataService.selectWaterLevelSumDataListTotCnt(searchVO);
			paginationInfo.setTotalRecordCount(totCnt);
			
			int from = nRecordCountPerPage * (m_nRRFPageIndex - 1);
			int to = nRecordCountPerPage * m_nRRFPageIndex;
			
			if (totCnt <= to)
				to = totCnt;
			
			List<?> totalWaterLevelList = waterLevelSumDataService.selectWaterLevelSumDataList(searchVO);
			List<?> waterLevelList = totalWaterLevelList.subList(from,  to);
			model.addAttribute("resultList", waterLevelList);
			
			MenuItem menu = (MenuItem)m_menus.get(3);
			SetCurrentMenu(menu, model);
		}
		catch (Exception e)
		{
			//trace(e.toString());
		}
		
		model.addAttribute("paginationInfo", paginationInfo);
		
		return "/weather/waterLevelSumData";
	}
	
	@RequestMapping(value = "/searchRainDataList.do")
	public String selectSearchRainData(@ModelAttribute("searchVO") PageVO searchVO, ModelMap model, HttpServletRequest request) throws Exception
	{
		String searchOptions = request.getParameter("param");
		String pageIndex = request.getParameter("pageIndex");
		
		if (searchOptions == null)
			searchOptions = "today";
		
		m_strPrevSearchRainOption = "today";
		
		MenuItem menu = (MenuItem)m_menus.get(4);
		menu.setLinkedPage(menu.getLinkedPageOrigin());
		SetCurrentMenu(menu, model);
		
		m_nRRFPageIndex = searchVO.getPageIndex();
		//searchVO.setPageIndex(m_nRRFPageIndex);
		
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
		
		List<String> searchYearList = SearchHelper.getSearchYearList(propertiesService.getInt("FirstSearchYear"), true);
		model.addAttribute("searchYearList", searchYearList);
		
		try
		{
			HttpSession session = request.getSession();
			
			List<CityTown> cityTowns = (List<CityTown>)searchRainService.selectCityTownList(searchVO);
			model.addAttribute("cityTowns", cityTowns);
			
			String cityName = searchRainService.selectCityName(searchVO);
			session.setAttribute("cityName", cityName);
			
			if (searchOptions.equals("today"))
				selectSearchRainToday(session, cityTowns, paginationInfo, nRecordCountPerPage, searchVO, model);
			else if (searchOptions.startsWith("month"))
				selectSearchRainMonth(session, searchOptions, cityTowns, paginationInfo, nRecordCountPerPage, searchVO, model);
			else if (searchOptions.startsWith("year"))
				selectSearchRainYear(session, searchOptions, cityTowns, paginationInfo, nRecordCountPerPage, searchVO, model);
			else if (searchOptions.startsWith("period"))
				selectSearchRainPeriod(session, searchOptions, cityTowns, paginationInfo, nRecordCountPerPage, searchVO, model);
		}
		catch (Exception e)
		{
			//trace(e.toString());
		}
		
		model.addAttribute("paginationInfo", paginationInfo);
		
		return "/search/rain";
	}
	
	@RequestMapping(value = "/searchSnowDataList.do")
	public String selectSearchSnowData(@ModelAttribute("searchVO") PageVO searchVO, ModelMap model, HttpServletRequest request) throws Exception
	{
		String searchOptions = request.getParameter("param");
		
		if (searchOptions == null)
			searchOptions = "today";
		
		m_strPrevSearchSnowOption = "today";
		
		MenuItem menu = (MenuItem)m_menus.get(5);
		menu.setLinkedPage(menu.getLinkedPageOrigin());
		SetCurrentMenu(menu, model);
		
		m_nRRFPageIndex = searchVO.getPageIndex();
		//searchVO.setPageIndex(m_nRRFPageIndex);
		
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
		
		List<String> searchYearList = SearchHelper.getSearchYearList(propertiesService.getInt("FirstSearchYear"), true);
		model.addAttribute("searchYearList", searchYearList);
		
		try
		{
			HttpSession session = request.getSession();
			
			List<CityTown> cityTowns = (List<CityTown>)searchSnowService.selectCityTownList(searchVO);
			model.addAttribute("cityTowns", cityTowns);
			
			String cityName = searchRainService.selectCityName(searchVO);
			session.setAttribute("cityName", cityName);
			
			if (searchOptions.equals("today"))
				selectSearchSnowToday(session, cityTowns, paginationInfo, nRecordCountPerPage, searchVO, model);
			else if (searchOptions.startsWith("month"))
				selectSearchSnowMonth(session, searchOptions, cityTowns, paginationInfo, nRecordCountPerPage, searchVO, model);
			else if (searchOptions.startsWith("year"))
				selectSearchSnowYear(session, searchOptions, cityTowns, paginationInfo, nRecordCountPerPage, searchVO, model);
			else if (searchOptions.startsWith("period"))
				selectSearchSnowPeriod(session, searchOptions, cityTowns, paginationInfo, nRecordCountPerPage, searchVO, model);
		}
		catch (Exception e)
		{
			//trace(e.toString());
		}
		
		model.addAttribute("paginationInfo", paginationInfo);
		
		return "/search/snow";
	}
	
	@RequestMapping(value = "/searchWaterLevelDataList.do")
	public String selectSearchWaterLevelData(@ModelAttribute("searchVO") PageVO searchVO, ModelMap model, HttpServletRequest request) throws Exception
	{
		//if( true)
		//	throw new java.io.IOException("에러 메세지 테스트");
		
		String searchOptions = request.getParameter("param");
		
		if (searchOptions == null)
			searchOptions = "day";
		
		m_strPrevSearchWaterLevelOption = "day";
		
		MenuItem menu = (MenuItem)m_menus.get(6);
		menu.setLinkedPage(menu.getLinkedPageOrigin());
		SetCurrentMenu(menu, model);
		
		m_nRRFPageIndex = searchVO.getPageIndex();
		//searchVO.setPageIndex(m_nRRFPageIndex);
		
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
		
		List<String> searchYearList = SearchHelper.getSearchYearList(propertiesService.getInt("FirstSearchYear"), true);
		model.addAttribute("searchYearList", searchYearList);
		
		try
		{
			HttpSession session = request.getSession();
			
			List<CityTown> cityTowns = (List<CityTown>)searchWaterLevelService.selectCityTownList(searchVO);
			model.addAttribute("cityTowns", cityTowns);
			
			String cityName = searchRainService.selectCityName(searchVO);
			session.setAttribute("cityName", cityName);
			
			if (searchOptions.startsWith("day"))
				selectSearchWaterLevelDay(session, searchOptions, cityTowns, paginationInfo, nRecordCountPerPage, searchVO, model);
			else if (searchOptions.startsWith("period"))
				selectSearchWaterLevelPeriod(session, searchOptions, cityTowns, paginationInfo, nRecordCountPerPage, searchVO, model);
		}
		catch (Exception e)
		{
			//trace(e.toString());
		}
		
		model.addAttribute("paginationInfo", paginationInfo);
		
		return "/search/waterLevel";
	}
	
	@RequestMapping(value = "/printReportDataList.do")
	public String selectPrintReportDataList(@ModelAttribute("searchVO") PageVO searchVO, ModelMap model, HttpServletRequest request) throws Exception
	{
		String searchOptions = request.getParameter("param");
		
		if (searchOptions == null)
		{
			searchOptions = "";
			//searchOptions = "rain";
		}
		
		m_strPrevSearchReportOption = "rain";
		
		MenuItem menu = (MenuItem)m_menus.get(7);
		menu.setLinkedPage(menu.getLinkedPageOrigin());
		SetCurrentMenu(menu, model);
		
		m_nRRFPageIndex = searchVO.getPageIndex();
		//searchVO.setPageIndex(m_nRRFPageIndex);
		
		/** EgovPropertyService.sample */
		searchVO.setPageUnit(MAX_ITEM_COUNT_PER_PAGE);
		//searchVO.setPageUnit(propertiesService.getInt("pageUnit"));
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
		
		if (searchOptions.length() > 0)
		{
			try
			{
				HttpSession session = request.getSession();
				
				String cityName = searchRainService.selectCityName(searchVO);
				session.setAttribute("cityName", cityName);
				
				if (searchOptions.startsWith("rain"))
					selectPrintReportData(session, searchOptions, paginationInfo, nRecordCountPerPage, searchVO, model, true);
				else if (searchOptions.startsWith("snow"))
					selectPrintReportData(session, searchOptions, paginationInfo, nRecordCountPerPage, searchVO, model, false);
			}
			catch (Exception e)
			{
				//trace(e.toString());
			}
		}
		else
			InitEmptyReportData(request.getSession(), model);
		
		model.addAttribute("paginationInfo", paginationInfo);
		
		return "/print/reportData";
	}
		
	private void InitEmptyReportData(HttpSession session, ModelMap model)
	{
		model.addAttribute("radarImageURL", "./images/common/radarNoimg.png");
		
		List<SpecialNews> emptyNewsList = new ArrayList();
		
		SpecialNews emptyNews = new SpecialNews();
		emptyNews.setEmptyData(true);
		
		emptyNewsList.add(emptyNews);
		model.addAttribute("newsList", emptyNewsList);
		model.addAttribute("searchingTime", "");
		
//		List<ReportData> resultList = new ArrayList();
//		model.addAttribute("resultList", resultList);
		
	}
	
	private void selectPrintReportData(HttpSession session, String param, PaginationInfo paginationInfo, int nRecordCountPerPage, PageVO searchVO, ModelMap model, boolean isRain) throws Exception
	{
		String[] arrParams = param.split(";");

		// 값이 없을 경우 올해의 1월 1일부터 오늘까지의 검색을 수행한다.
		if (arrParams.length < 3)
		{
			java.text.DateFormat format = new java.text.SimpleDateFormat("yyyy-MM-dd");
			Calendar cal = Calendar.getInstance();
			
			String today = format.format(cal.getTime());
			String firstDay = today.substring(0, 4) + "-01-01";
			String reportType = arrParams.length >= 1 && arrParams[0].length() > 0 ? arrParams[0] : "rain";
			
			param = reportType + ";" + firstDay + ";" + today;
			arrParams = param.split(";");
		}
		
		if (arrParams[1].length() < 10 || arrParams[2].length() < 10)
			return;
	
		String beginDateCondition = getOnlyNumbers(arrParams[1]);
		String endDateCondition = getOnlyNumbers(arrParams[2]);

		searchVO.setBeginDateCodition(beginDateCondition);
		searchVO.setEndDateCondition(endDateCondition);
		
		List<ReportData> totalReportDataList = getTotalReportDataList(searchVO, isRain); 
		
		// totalReportDataList 결과에 전일과 금일 데이터가 포함됐는지 여부
		Boolean isToday = new Boolean(true);
		Boolean isYesterday = new Boolean(true);
		
		int beforeDate = Integer.parseInt(beginDateCondition);
		int afterDate = Integer.parseInt(endDateCondition);
		
		SimpleDateFormat date = new SimpleDateFormat("yyyyMMdd");
		Calendar cal = Calendar.getInstance();		
		String todayStr = date.format(cal.getTime());
		cal.add(cal.DATE, -1);
		String yesterdayStr = date.format(cal.getTime());
		//test
		//todayStr = "20170105";
		//yesterdayStr = "20170104";
		int todayInt = Integer.parseInt(todayStr);
		int yesterDayInt = Integer.parseInt(yesterdayStr);
		
		if (todayInt >= beforeDate & todayInt <= afterDate)
			isToday = true;
		else
			isToday = false;
		
		if (yesterDayInt >= beforeDate & yesterDayInt <= afterDate)
			isYesterday = true;
		else
			isYesterday = false;
	    
		for(int i = 0; i < totalReportDataList.size() ; i++)
		{ 
			totalReportDataList.get(i).setDaySum(totalReportDataList.get(i).getDaySum() * 0.1);
		} 
		
		SearchHelper.todayReportDataList.clear();
		SearchHelper.yesterdayReportDataList.clear();
		
		// 첫번째 값이 최소값, 두번째값이 최대값
		List<ReportData> minMaxList = new ArrayList();
		List<ReportData> reportDataList = SearchHelper.makeReportData(totalReportDataList, minMaxList, isRain, isToday, isYesterday); 
		
		int totCnt = reportDataList.size();
		paginationInfo.setTotalRecordCount(totCnt);
		
		int from = nRecordCountPerPage * (m_nRRFPageIndex - 1);
		int to = nRecordCountPerPage * m_nRRFPageIndex;
		
		if (totCnt <= to)
			to = totCnt;
		
		//List<String> headers = SearchHelper.getPrintReportDataHeader(isRain);
		//model.addAttribute("reportDataHeader", headers);
		
		List<String> headers1 = new ArrayList();
		List<String> headers2 = new ArrayList();
		List<String> headers3 = new ArrayList();
		List<String> headersUp = new ArrayList();
		SearchHelper.getPrintReportDataHeader2(isRain, beginDateCondition, endDateCondition, headers1, headers2, headers3, headersUp);
		
		model.addAttribute("reportDataHeader1", headers1);
		model.addAttribute("reportDataHeader2", headers2);
		model.addAttribute("reportDataHeader3", headers3);
		model.addAttribute("reportDataHeaderUp", headersUp);
		
		session.setAttribute("excelParam", beginDateCondition + "-" + endDateCondition);
					
		List<?> reportList = reportDataList.subList(from,  to);
		model.addAttribute("resultList", reportList);
		
		List<String> avgList = getReportDataAverageList(reportList, isRain);
		model.addAttribute("resultAverageList", avgList);
		
		model.addAttribute("printReportDataParam", param);
		
		try {
			String serviceKey = propertiesService.getString("SNewsServiceKey");
			
			String urlString = "http://newsky2.kma.go.kr/service/WetherSpcnwsInfoService/SpecialNewsCode?ServiceKey=";
			urlString += serviceKey + "&areaCode=&warninType=&numOfRows=999&pageNo=1";
			//test
			//String urlString = "http://newsky2.kma.go.kr/service/WetherSpcnwsInfoService/SpecialNewsCode?ServiceKey=6lIn11vBfAG7cH0zgE5Skyqh5bClzO%2Fhi%2Fub12cL6TvJdmp6QC4QQW0vbhzWUK%2B7OEVfqQ92SI9MNfHophhP4g%3D%3D&areaCode=&warninType=&numOfRows=999&pageNo=1&fromTmFc=20170101";
			java.net.URL url = new java.net.URL(urlString);
			java.net.HttpURLConnection connection = (java.net.HttpURLConnection)url.openConnection();
			
			//connection.setRequestProperty("Content-Type", "application/json");
			//connection.setRequestProperty("Accept", "application/json");
			//connection.setRequestMethod("GET");
			
            java.io.InputStream is = connection.getInputStream();
            java.util.Scanner scan = new java.util.Scanner(is);
            
            String result = "";
            int line = 1;
            while (scan.hasNext())
            {
            	String str = scan.nextLine();
            	result += str;
            }
            scan.close();
            is.close();
            
            List<SpecialNews> specialNewsList = SpecialNewsXMLParser.parse(result);
            
            if (specialNewsList != null)
            {
            	model.addAttribute("newsList", specialNewsList);
            }
            
            String radarImageURL = RadarImageURLParser.getCurrentURL(serviceKey);
            model.addAttribute("radarImageURL", radarImageURL);
            
            session.setAttribute("radarImageURL", radarImageURL);
            session.setAttribute("newsList", specialNewsList);
            session.setAttribute("currentResult", reportDataList); 
            session.setAttribute("minMaxList", minMaxList);
    		session.setAttribute("excelView", isRain ? "prdRainExcelView" : "prdSnowExcelView");
    		
    		DateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
    		String now = dateFormat.format(new java.util.Date());
    		session.setAttribute("excelParam", beginDateCondition + ";" + endDateCondition + ";" + now);
    		
    		String searchingTime = getSearchingTime(now);
    		model.addAttribute("searchingTime", searchingTime);
            
        } catch (Exception e) {
            System.err.println("Error occurred while sending SOAP Request to Server");
            e.printStackTrace();
        }

	}
	
	private List<String> getReportDataAverageList(List<?> reportList, boolean isRain)
	{
		List<String> avgList = new ArrayList();
		
		avgList.add("평균");
		avgList.add("");
		avgList.add("");
		avgList.add("");
		avgList.add("");
		
		int nIndex = 1;
		
		if (isRain == false)
		{
			avgList.add("");
			nIndex = 2;
		}
		
		double sum1 = 0.0, sum2 = 0.0, sum3 = 0.0;
		int nCount = 0;
		
		for (Object data : reportList)
		{
			ReportData reportData = (ReportData)data;
			
			List<String> items = reportData.getPrintDetailItems();

			try
			{
				double num1 = Double.parseDouble(items.get(nIndex));
				double num2 = Double.parseDouble(items.get(nIndex + 1));
				double num3 = Double.parseDouble(items.get(nIndex + 2));
				
				sum1 += num1;
				sum2 += num2;
				sum3 += num3;
				nCount++;
			}
			catch (NumberFormatException e)
			{
			}
		}
		
		if (nCount > 0)
		{
			avgList.set(nIndex, SearchRain.DoubleToString(sum1 / nCount));
			avgList.set(nIndex + 1, SearchRain.DoubleToString(sum2 / nCount));
			avgList.set(nIndex + 2, SearchRain.DoubleToString(sum3 / nCount));
		}
		
		return avgList;
	}
	
	private String getSearchingTime(String time)
	{
		String year = time.substring(0, 4);
		String month = time.substring(5, 7);
		String day = time.substring(8, 10);
		String hour = time.substring(11, 13);
		String min = time.substring(14, 16);
		String sec = time.substring(17, 19);
		
		int hourValue = 0;
		
		try
		{
			hourValue = Integer.parseInt(hour);
		}
		catch (Exception e)
		{
			return time;
		}
		
		String hourString = "";
		
		if (hourValue >= 12)
		{
			if (hourValue == 12)
				hourString = "오후 12";
			else
				hourString = "오후 " + (hourValue - 12);
			//hourString = "오후 " + (hourValue - 12);
		}
		else
			hourString = "오전 " + toShortTime(Integer.toString(hourValue));  
		
		String result = year + "년 " + toShortTime(month) + "월 " + toShortTime(day) + "일 / ";
		result += hourString + "시 " + toShortTime(min) + "분 " + toShortTime(sec) + "초";
		return result;
	}
	
	private String toShortTime(String time)
	{
		if (time.startsWith("0"))
			return time.substring(1);
		
		return time;
	}
	
	private List<ReportData> getTotalReportDataList(PageVO searchVO, boolean isRain) throws Exception
	{
		if (isRain)
			return (List<ReportData>)reportDataService.selectReportRainDataPeriodList(searchVO);
		
		return (List<ReportData>)reportDataService.selectReportSnowDataPeriodList(searchVO);
	} 
	
	private void selectSearchWaterLevelDay(HttpSession session, String param, List<CityTown> cityTowns, PaginationInfo paginationInfo, int nRecordCountPerPage, PageVO searchVO, ModelMap model) throws Exception
	{
		String[] arrParams = param.split(";");
		String date = "";
		
		if (arrParams.length < 2)
		{
			java.text.DateFormat format = new java.text.SimpleDateFormat("yyyy-MM-dd");
			Calendar cal = Calendar.getInstance();
			date = format.format(cal.getTime());
			//return;
		}
		else
			date = arrParams[1];
		
		if (date.length() < 10)
			return;
		
		String dateCondition = getOnlyNumbers(date);
		searchVO.setDateCondition(dateCondition);
		
		List<SearchWaterLevelDay> totalWaterLevelList = (List<SearchWaterLevelDay>)searchWaterLevelService.selectSearchWaterLevelDayList(searchVO);
		// DB에 강우 수치가 없는 지역들도 추가한다.
		//SearchHelper.addCityTowns(totalRainList, cityTowns);
		
		// 평균값 추가
		SearchWaterLevelDay avgSearch = SearchHelper.getAverageSearchWaterLevelDay(totalWaterLevelList);
		totalWaterLevelList.add(0, avgSearch);
		
		session.setAttribute("currentResult", totalWaterLevelList);
		session.setAttribute("excelView", "wllDayExcelView");
		session.setAttribute("excelParam", date);
		
		int totCnt = totalWaterLevelList.size();
		paginationInfo.setTotalRecordCount(totCnt);
		
		int from = nRecordCountPerPage * (m_nRRFPageIndex - 1);
		int to = nRecordCountPerPage * m_nRRFPageIndex;
		
		if (totCnt <= to)
			to = totCnt;
		
		List<String> searchHeader = SearchHelper.getSearchWaterLevelDayHeader();
		model.addAttribute("searchHeader", searchHeader);
		
		List<?> waterLevelList = totalWaterLevelList.subList(from,  to);
		model.addAttribute("resultList", waterLevelList);
		
		trace("selectSearchWaterLevelDay Param : " + date);
		model.addAttribute("searchWaterLevelParam", "day;" + date);
	}
	
	private void selectSearchWaterLevelPeriod(HttpSession session, String param, List<CityTown> cityTowns, PaginationInfo paginationInfo, int nRecordCountPerPage, PageVO searchVO, ModelMap model) throws Exception
	{
		String[] arrParams = param.split(";");
		
		if (arrParams.length < 4)
			return;
		
		if (arrParams[1].length() < 10 || arrParams[2].length() < 10)
			return;
	
		String beginDateCondition = getOnlyNumbers(arrParams[1]);
		String endDateCondition = getOnlyNumbers(arrParams[2]);
		String locationID = arrParams[3];
				
		searchVO.setBeginDateCodition(beginDateCondition);
		searchVO.setEndDateCondition(endDateCondition);
		searchVO.setTownCode(locationID);
		List<SearchWaterLevelPeriod> totalWaterLevelList = (List<SearchWaterLevelPeriod>)searchWaterLevelService.selectSearchWaterLevelPeriodList(searchVO);
		
		for (SearchWaterLevelPeriod period : totalWaterLevelList)
		{
			// HTML 표기를 위하여 지역명에 날짜를 넣는다.
			period.setLocationName(period.getWaterLevelDate());
		}
		
		session.setAttribute("currentResult", totalWaterLevelList);
		session.setAttribute("excelView", "wllPeriodExcelView");
		
		int totCnt = totalWaterLevelList.size();
		paginationInfo.setTotalRecordCount(totCnt);
		
		int from = nRecordCountPerPage * (m_nRRFPageIndex - 1);
		int to = nRecordCountPerPage * m_nRRFPageIndex;
		
		if (totCnt <= to)
			to = totCnt;
		
		List<String> searchHeader = SearchHelper.getSearchWaterLevelPeriodHeader(locationID, cityTowns);
		model.addAttribute("searchHeader", searchHeader);
		
		String locationName = searchHeader.get(0);
		
		if (locationName != null)
			session.setAttribute("excelParam", locationName + ";" + beginDateCondition + "-" + endDateCondition);
		else
			session.setAttribute("excelParam", ";" + beginDateCondition + "-" + endDateCondition);
					
		List<?> snowList = totalWaterLevelList.subList(from,  to);
		model.addAttribute("resultList", snowList);
		
		model.addAttribute("searchWaterLevelParam", param);
	}
	
	private void selectSearchSnowPeriod(HttpSession session, String param, List<CityTown> cityTowns, PaginationInfo paginationInfo, int nRecordCountPerPage, PageVO searchVO, ModelMap model) throws Exception
	{
		String[] arrParams = param.split(";");
		
		if (arrParams.length < 4)
			return;
		
		if (arrParams[1].length() < 10 || arrParams[2].length() < 10)
			return;
	
		String beginDateCondition = getOnlyNumbers(arrParams[1]);
		String endDateCondition = getOnlyNumbers(arrParams[2]);
		String locationID = arrParams[3];
				
		searchVO.setBeginDateCodition(beginDateCondition);
		searchVO.setEndDateCondition(endDateCondition);
		searchVO.setTownCode(locationID);
		List<SearchSnowPeriod> totalSnowList = (List<SearchSnowPeriod>)searchSnowService.selectSearchSnowPeriodList(searchVO);
		
		for (SearchSnowPeriod period : totalSnowList)
		{
			// HTML 표기를 위하여 지역명에 날짜를 넣는다.
			period.setLocationName(period.getSnowDate());
		}
		
		session.setAttribute("currentResult", totalSnowList);
		session.setAttribute("excelView", "sslPeriodExcelView");
		
		int totCnt = totalSnowList.size();
		paginationInfo.setTotalRecordCount(totCnt);
		
		int from = nRecordCountPerPage * (m_nRRFPageIndex - 1);
		int to = nRecordCountPerPage * m_nRRFPageIndex;
		
		if (totCnt <= to)
			to = totCnt;
		
		List<String> searchHeader = SearchHelper.getSearchSnowPeriodHeader(locationID, cityTowns);
		model.addAttribute("searchHeader", searchHeader);
		
		String locationName = searchHeader.get(0);
		
		if (locationName != null)
			session.setAttribute("excelParam", locationName + ";" + beginDateCondition + "-" + endDateCondition);
		else
			session.setAttribute("excelParam", ";" + beginDateCondition + "-" + endDateCondition);
					
		List<?> snowList = totalSnowList.subList(from,  to);
		model.addAttribute("resultList", snowList);
		
		model.addAttribute("searchSnowParam", param);
	}
	
	private void selectSearchSnowYear(HttpSession session, String param, List<CityTown> cityTowns, PaginationInfo paginationInfo, int nRecordCountPerPage, PageVO searchVO, ModelMap model) throws Exception
	{
		String[] arrParams = param.split(";");
		
		if (arrParams.length < 2)
			return;
		
		int year;
		
		try
		{
			year = Integer.parseInt(arrParams[1]);
		}
		catch (NumberFormatException e)
		{
			return;
		}
		
		String dateCondition = Integer.toString(year) + "%";
				
		searchVO.setDateCondition(dateCondition);
		List<SearchSnowYear> totalYearSnowList = (List<SearchSnowYear>)searchSnowService.selectSearchSnowYearList(searchVO);
		
		// 지역별 일간 데이터를 이용하여 지역별 연간 데이터를 만든다.
		List<SearchSnow> totalSnowList = SearchHelper.makeSearchSnowYearList(year, totalYearSnowList);
		
		session.setAttribute("currentResult", totalSnowList);
		session.setAttribute("excelView", "sslYearExcelView");
		session.setAttribute("excelParam", arrParams[1]);
		
		int totCnt = totalSnowList.size();
		paginationInfo.setTotalRecordCount(totCnt);
		
		int from = nRecordCountPerPage * (m_nRRFPageIndex - 1);
		int to = nRecordCountPerPage * m_nRRFPageIndex;
		
		if (totCnt <= to)
			to = totCnt;
		
		List<String> searchHeader = SearchHelper.getSearchSnowYearHeader();
		model.addAttribute("searchHeader", searchHeader);
					
		List<?> snowList = totalSnowList.subList(from,  to);
		model.addAttribute("resultList", snowList);
		
		model.addAttribute("searchSnowParam", param);
	}
	
	private void selectSearchSnowMonth(HttpSession session, String param, List<CityTown> cityTowns, PaginationInfo paginationInfo, int nRecordCountPerPage, PageVO searchVO, ModelMap model) throws Exception
	{
		String[] arrParams = param.split(";");
		
		if (arrParams.length < 3)
			return;
		
		int year, month;
		
		try
		{
			year = Integer.parseInt(arrParams[1]);
			month = Integer.parseInt(arrParams[2]);
		}
		catch (NumberFormatException e)
		{
			return;
		}
		
		String dateCondition = Integer.toString(year);
		
		if (month < 10)
			dateCondition += "0" + Integer.toString(month) + "%";
		else
			dateCondition += Integer.toString(month) + "%";
				
		searchVO.setDateCondition(dateCondition);
		List<SearchSnowMonth> totalMonthSnowList = (List<SearchSnowMonth>)searchSnowService.selectSearchSnowMonthList(searchVO);
		
		// 지역별 일간 데이터를 이용하여 지역별 월간 데이터를 만든다.
		List<SearchSnow> totalSnowList = SearchHelper.makeSearchSnowMonthList(year, month, totalMonthSnowList);
		
		Calendar cal = new GregorianCalendar(year, month - 1, 1);
		int daysInMonth = cal.getActualMaximum(Calendar.DAY_OF_MONTH);
		
		session.setAttribute("currentResult", totalSnowList);
		session.setAttribute("excelView", "sslMonthExcelView");
		session.setAttribute("excelParam", arrParams[1] + arrParams[2] + ";" + Integer.toString(daysInMonth));
		
		int totCnt = totalSnowList.size();
		paginationInfo.setTotalRecordCount(totCnt);
		
		int from = nRecordCountPerPage * (m_nRRFPageIndex - 1);
		int to = nRecordCountPerPage * m_nRRFPageIndex;
		
		if (totCnt <= to)
			to = totCnt;
		
		List<String> searchHeader = SearchHelper.getSearchSnowMonthHeader(year, month);
		model.addAttribute("searchHeader", searchHeader);
					
		List<?> rainList = totalSnowList.subList(from,  to);
		model.addAttribute("resultList", rainList);
		
		model.addAttribute("searchSnowParam", param);
	}
	
	private void selectSearchSnowToday(HttpSession session, List<CityTown> cityTowns, PaginationInfo paginationInfo, int nRecordCountPerPage, PageVO searchVO, ModelMap model) throws Exception
	{
		List<SearchSnowToday> totalSnowList = (List<SearchSnowToday>)searchSnowService.selectSearchSnowTodayList(searchVO);
		// DB에 강우 수치가 없는 지역들도 추가한다.
		//SearchHelper.addCityTowns(totalRainList, cityTowns);
		
		// 평균값 추가
		SearchSnowToday avgSearch = SearchHelper.getAverageSearchSnowToday(totalSnowList);
		totalSnowList.add(0, avgSearch);
		
		session.setAttribute("currentResult", totalSnowList);
		session.setAttribute("excelView", "sslTodayExcelView");
		
		int totCnt = totalSnowList.size();
		paginationInfo.setTotalRecordCount(totCnt);
		
		int from = nRecordCountPerPage * (m_nRRFPageIndex - 1);
		int to = nRecordCountPerPage * m_nRRFPageIndex;
		
		if (totCnt <= to)
			to = totCnt;
		
		List<String> searchHeader = SearchHelper.getSearchSnowTodayHeader();
		model.addAttribute("searchHeader", searchHeader);
		
		
		List<?> snowList = totalSnowList.subList(from,  to);
		model.addAttribute("resultList", snowList);
		
		model.addAttribute("searchSnowParam", "today");
	}
	
	private void selectSearchRainPeriod(HttpSession session, String param, List<CityTown> cityTowns, PaginationInfo paginationInfo, int nRecordCountPerPage, PageVO searchVO, ModelMap model) throws Exception
	{
		String[] arrParams = param.split(";");
		
		if (arrParams.length < 4)
			return;
		
		if (arrParams[1].length() < 10 || arrParams[2].length() < 10)
			return;
	
		String beginDateCondition = getOnlyNumbers(arrParams[1]);
		String endDateCondition = getOnlyNumbers(arrParams[2]);
		String locationID = arrParams[3];
				
		searchVO.setBeginDateCodition(beginDateCondition);
		searchVO.setEndDateCondition(endDateCondition);
		searchVO.setTownCode(locationID);
		List<SearchRainPeriod> totalRainList = (List<SearchRainPeriod>)searchRainService.selectSearchRainPeriodList(searchVO);
		
		for (SearchRainPeriod period : totalRainList)
		{
			// HTML 표기를 위하여 지역명에 날짜를 넣는다.
			period.setLocationName(period.getRainDate());
		}
		
		session.setAttribute("currentResult", totalRainList);
		session.setAttribute("excelView", "srlPeriodExcelView");
		
		int totCnt = totalRainList.size();
		paginationInfo.setTotalRecordCount(totCnt);
		
		int from = nRecordCountPerPage * (m_nRRFPageIndex - 1);
		int to = nRecordCountPerPage * m_nRRFPageIndex;
		
		if (totCnt <= to)
			to = totCnt;
		
		List<String> searchHeader = SearchHelper.getSearchRainPeriodHeader(locationID, cityTowns);
		model.addAttribute("searchHeader", searchHeader);
		
		String locationName = searchHeader.get(0);
		
		if (locationName != null)
			session.setAttribute("excelParam", locationName + ";" + beginDateCondition + "-" + endDateCondition);
		else
			session.setAttribute("excelParam", ";" + beginDateCondition + "-" + endDateCondition);
					
		List<?> rainList = totalRainList.subList(from,  to);
		model.addAttribute("resultList", rainList);
		
		model.addAttribute("searchRainParam", param);
	}
	
	// 숫자부분만 가져온다.
	private String getOnlyNumbers(String date)
	{
		String result = "";
		int len = date.length();
		
		for (int i=0;i<len;i++)
		{
			char ch = date.charAt(i);
			
			if (ch >= '0' && ch <= '9')
				result += ch;
		}
		
		return result;
	}
	
	private void selectSearchRainYear(HttpSession session, String param, List<CityTown> cityTowns, PaginationInfo paginationInfo, int nRecordCountPerPage, PageVO searchVO, ModelMap model) throws Exception
	{
		String[] arrParams = param.split(";");
		
		if (arrParams.length < 2)
			return;
		
		int year;
		
		try
		{
			year = Integer.parseInt(arrParams[1]);
		}
		catch (NumberFormatException e)
		{
			return;
		}
		
		String dateCondition = Integer.toString(year) + "%";
				
		searchVO.setDateCondition(dateCondition);
		List<SearchRainYear> totalYearRainList = (List<SearchRainYear>)searchRainService.selectSearchRainYearList(searchVO);
		
		// 지역별 일간 데이터를 이용하여 지역별 연간 데이터를 만든다.
		List<SearchRain> totalRainList = SearchHelper.makeSearchRainYearList(year, totalYearRainList);
		
		session.setAttribute("currentResult", totalRainList);
		session.setAttribute("excelView", "srlYearExcelView");
		session.setAttribute("excelParam", arrParams[1]);
		
		int totCnt = totalRainList.size();
		paginationInfo.setTotalRecordCount(totCnt);
		
		int from = nRecordCountPerPage * (m_nRRFPageIndex - 1);
		int to = nRecordCountPerPage * m_nRRFPageIndex;
		
		if (totCnt <= to)
			to = totCnt;
		
		List<String> searchHeader = SearchHelper.getSearchRainYearHeader();
		model.addAttribute("searchHeader", searchHeader);
					
		List<?> rainList = totalRainList.subList(from,  to);
		model.addAttribute("resultList", rainList);
		
		model.addAttribute("searchRainParam", param);
	}
	
	private void selectSearchRainMonth(HttpSession session, String param, List<CityTown> cityTowns, PaginationInfo paginationInfo, int nRecordCountPerPage, PageVO searchVO, ModelMap model) throws Exception
	{
		String[] arrParams = param.split(";");
		
		if (arrParams.length < 3)
			return;
		
		int year, month;
		
		try
		{
			year = Integer.parseInt(arrParams[1]);
			month = Integer.parseInt(arrParams[2]);
		}
		catch (NumberFormatException e)
		{
			return;
		}
		
		String dateCondition = Integer.toString(year);
		
		if (month < 10)
			dateCondition += "0" + Integer.toString(month) + "%";
		else
			dateCondition += Integer.toString(month) + "%";
				
		searchVO.setDateCondition(dateCondition);
		List<SearchRainMonth> totalMonthRainList = (List<SearchRainMonth>)searchRainService.selectSearchRainMonthList(searchVO);
		
		// 지역별 일간 데이터를 이용하여 지역별 월간 데이터를 만든다.
		List<SearchRain> totalRainList = SearchHelper.makeSearchRainMonthList(year, month, totalMonthRainList);
		
		Calendar cal = new GregorianCalendar(year, month - 1, 1);
		int daysInMonth = cal.getActualMaximum(Calendar.DAY_OF_MONTH);
		
		session.setAttribute("currentResult", totalRainList);
		session.setAttribute("excelView", "srlMonthExcelView");
		session.setAttribute("excelParam", arrParams[1] + arrParams[2] + ";" + Integer.toString(daysInMonth));
		
		int totCnt = totalRainList.size();
		paginationInfo.setTotalRecordCount(totCnt);
		
		int from = nRecordCountPerPage * (m_nRRFPageIndex - 1);
		int to = nRecordCountPerPage * m_nRRFPageIndex;
		
		if (totCnt <= to)
			to = totCnt;
		
		List<String> searchHeader = SearchHelper.getSearchRainMonthHeader(year, month);
		model.addAttribute("searchHeader", searchHeader);
					
		List<?> rainList = totalRainList.subList(from,  to);
		model.addAttribute("resultList", rainList);
		
		model.addAttribute("searchRainParam", param);
	}
	
	private void selectSearchRainToday(HttpSession session, List<CityTown> cityTowns, PaginationInfo paginationInfo, int nRecordCountPerPage, PageVO searchVO, ModelMap model) throws Exception
	{
		List<SearchRainToday> totalRainList = (List<SearchRainToday>)searchRainService.selectSearchRainTodayList(searchVO);
		// DB에 강우 수치가 없는 지역들도 추가한다.
		//SearchHelper.addCityTowns(totalRainList, cityTowns);
		
		// 평균값 추가
		SearchRainToday avgSearch = SearchHelper.getAverageSearchRainToday(totalRainList);
		totalRainList.add(0, avgSearch);
		
		session.setAttribute("currentResult", totalRainList);
		session.setAttribute("excelView", "srlTodayExcelView");
		
		int totCnt = totalRainList.size();
		//int totCnt = searchRainService.selectSearchRainTodayListTotCnt(searchVO);
		paginationInfo.setTotalRecordCount(totCnt);
		
		int from = nRecordCountPerPage * (m_nRRFPageIndex - 1);
		int to = nRecordCountPerPage * m_nRRFPageIndex;
		
		if (totCnt <= to)
			to = totCnt;
		
		List<String> searchHeader = SearchHelper.getSearchRainTodayHeader();
		model.addAttribute("searchHeader", searchHeader);
		
		//List<?> totalRainList = searchRainService.selectSearchRainTodayList(searchVO);			
		List<?> rainList = totalRainList.subList(from,  to);
		model.addAttribute("resultList", rainList);
		
		model.addAttribute("searchRainParam", "today");
	}
	  	
	@RequestMapping(value = "/main.do")
	public String showWeatherFrame(@ModelAttribute("searchVO") PageVO searchVO, ModelMap model) throws Exception
	{
		SetCurrentMenu((MenuItem)m_menus.get(0), model);
		model.addAttribute("menus", m_menus);
		m_nRRFPageIndex = searchVO.getPageIndex();
		return "/weather/home";
	}
	
	@RequestMapping(value = "/mainPage.do")
	public String showMainPage(@ModelAttribute("searchVO") PageVO searchVO, ModelMap model) throws Exception
	{
		SetCurrentMenu((MenuItem)m_menus.get(0), model);
		model.addAttribute("menus", m_menus);
		m_nRRFPageIndex = searchVO.getPageIndex();
		return "/weather/home";
	}
	
	@RequestMapping(value = "/rainList.do")
	public String showRainList(@ModelAttribute("searchVO") PageVO searchVO, ModelMap model) throws Exception
	{
		SetCurrentMenu((MenuItem)m_menus.get(1), model);
		model.addAttribute("menus", m_menus);
		m_nRRFPageIndex = searchVO.getPageIndex();
		return "/weather/home";
	}
	
	@RequestMapping(value = "/snowList.do")
	public String showSnowList(@ModelAttribute("searchVO") PageVO searchVO, ModelMap model) throws Exception
	{
		SetCurrentMenu((MenuItem)m_menus.get(2), model);
		model.addAttribute("menus", m_menus);
		m_nRRFPageIndex = searchVO.getPageIndex();
		return "/weather/home";
	}
	
	@RequestMapping(value = "/waterLevelList.do")
	public String showWaterLevelList(@ModelAttribute("searchVO") PageVO searchVO, ModelMap model) throws Exception
	{
		SetCurrentMenu((MenuItem)m_menus.get(3), model);
		model.addAttribute("menus", m_menus);
		m_nRRFPageIndex = searchVO.getPageIndex();
		return "/weather/home";
	}
	
	@RequestMapping(value = "/searchRainList.do")
	public String showSearchRainList(@ModelAttribute("searchVO") PageVO searchVO, ModelMap model, HttpServletRequest request) throws Exception
	{
		String searchOptions = request.getParameter("param");
		
	    java.util.Enumeration<String> parameterNames = request.getParameterNames();		
	    while (parameterNames.hasMoreElements()) {		
	        String paramName = parameterNames.nextElement();
	        String[] paramValues = request.getParameterValues(paramName);		
	        for (int i = 0; i < paramValues.length; i++) {		
	            String paramValue = paramValues[i];	
	            
	        }
	    }
	    
		if (searchOptions == null)
			searchOptions = m_strPrevSearchRainOption;
		
		m_strPrevSearchRainOption = searchOptions;
		
		MenuItem menu = (MenuItem)m_menus.get(4);
		menu.setLinkedPage(menu.getLinkedPageOrigin() + "?param=" + searchOptions);
		
		SetCurrentMenu(menu, model);
		model.addAttribute("menus", m_menus);
		m_nRRFPageIndex = searchVO.getPageIndex();
		return "/weather/home";
	}
	
	@RequestMapping(value = "/searchSnowList.do")
	public String showSearchSnowList(@ModelAttribute("searchVO") PageVO searchVO, ModelMap model, HttpServletRequest request) throws Exception
	{
		String searchOptions = request.getParameter("param");
		
		if (searchOptions == null)
			searchOptions = m_strPrevSearchSnowOption;
		
		m_strPrevSearchSnowOption = searchOptions;
		
		MenuItem menu = (MenuItem)m_menus.get(5);
		menu.setLinkedPage(menu.getLinkedPageOrigin() + "?param=" + searchOptions);
		
		SetCurrentMenu(menu, model);
		model.addAttribute("menus", m_menus);
		m_nRRFPageIndex = searchVO.getPageIndex();
		return "/weather/home";
	}
	
	@RequestMapping(value = "/searchWaterLevelList.do")
	public String showSearchWaterLevelList(@ModelAttribute("searchVO") PageVO searchVO, ModelMap model, HttpServletRequest request) throws Exception
	{
		String searchOptions = request.getParameter("param");
		
		if (searchOptions == null)
			searchOptions = m_strPrevSearchWaterLevelOption;
		
		m_strPrevSearchWaterLevelOption = searchOptions;
		
		MenuItem menu = (MenuItem)m_menus.get(6);
		menu.setLinkedPage(menu.getLinkedPageOrigin() + "?param=" + searchOptions);
		
		trace(menu.getLinkedPageOrigin() + "?param=" + searchOptions);
		
		SetCurrentMenu(menu, model);
		model.addAttribute("menus", m_menus);
		m_nRRFPageIndex = searchVO.getPageIndex();
		return "/weather/home";
	}
	
	@RequestMapping(value = "/showCCTV.do")
	public String showCCTV(@ModelAttribute("searchVO") PageVO searchVO, ModelMap model, HttpServletRequest request) throws Exception
	{
		MenuItem menu = (MenuItem)m_menus.get(8);
		SetCurrentMenu(menu, model);
		model.addAttribute("menus", m_menus);
		
		// cctvOption : 0(사용안함), 1(이미지 사용), 2(URL)
		int cctvOption = propertiesService.getInt("CCTVOption");
		
		if (cctvOption == 0 || cctvOption == 1)
			model.addAttribute("CCTVOption", Integer.toString(cctvOption));
		else if (cctvOption == 2)
		{
			String url = propertiesService.getString("CCTV_URL");
			
			if (url == null)
				model.addAttribute("CCTVOption", "0");
			else
				model.addAttribute("CCTVOption", Integer.toString(cctvOption) + ";" + url);
		}

		return "/cctv/cctv";
	}
	 
	@RequestMapping(value = "/error404Page.do")
	public String showError404Page(@ModelAttribute("searchVO") PageVO searchVO, ModelMap model, HttpServletRequest request) throws Exception
	{
		try
		{
			model.addAttribute("errorText", "페이지를 찾을 수 없습니다.");
		}
		catch (Exception e)
		{
		}
		
		return "/error/dataError";
	}
	
	@RequestMapping(value = "/error500Page.do")
	public String showError500Page(@ModelAttribute("searchVO") PageVO searchVO, ModelMap model, HttpServletRequest request) throws Exception
	{
		try
		{
			model.addAttribute("errorText", "서버 내부 오류");
		}
		catch (Exception e)
		{
		}
		
		return "/error/dataError";
	}
	
	@RequestMapping(value = "/errorExcpetionPage.do")
	public String showErrorExceptionPage(@ModelAttribute("searchVO") PageVO searchVO, ModelMap model, HttpServletRequest request) throws Exception
	{
		try
		{
			model.addAttribute("errorText", "서버 내부 오류");
		}
		catch (Exception e)
		{
		}
		
		return "/error/dataError";
	}
	
	@RequestMapping(value = "/downloadExcelMainPage.do")
	public ModelAndView downloadExcelMainPage(@ModelAttribute("searchVO") PageVO searchVO, HttpServletRequest request) throws Exception
	{
		try
		{
			HttpSession session = request.getSession();
			
			String radarImageURL = (String)session.getAttribute("radarImageURL");
			List<SpecialNews> specialNewsList = (List<SpecialNews>)session.getAttribute("newsList");
			Object reportDataList = session.getAttribute("rainResultList"); 
			String excelViewName = (String)session.getAttribute("excelView");
			String excelParam = (String)session.getAttribute("excelParam"); 
			String cityName = (String)session.getAttribute("cityName");
			
			if (specialNewsList == null || reportDataList == null)
				return null;

			Map<String, Object> map = new HashMap<String, Object>();
			map.put("radarImageURL", radarImageURL);
			map.put("newsList", specialNewsList);
			map.put("totalRRF", reportDataList);
			map.put("cityName", cityName); 
			
			if (excelParam != null)
				map.put("excelParam", excelParam);
			
			return new ModelAndView(excelViewName, map); 
		}
		catch (Exception e)
		{
		}
		
		return null;
	}
	
	@RequestMapping(value = "/downloadExcelRainList.do")
	public ModelAndView downloadExcelRealTimeRainFall(@ModelAttribute("searchVO") PageVO searchVO) throws Exception
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
		}
		
		return null;
	}
	
	@RequestMapping(value = "/downloadExcelSnowList.do")
	public ModelAndView downloadExcelSnowSumData(@ModelAttribute("searchVO") PageVO searchVO) throws Exception
	{
		try
		{	
			List<?> totalSnowList = snowSumDataService.selectSnowSumDataList(searchVO);

			Map<String, Object> map = new HashMap<String, Object>();
			map.put("totalSnowSumData", totalSnowList);
			
			return new ModelAndView("ssdExcelView", map);
		}
		catch (Exception e)
		{
		}
		
		return null;
	}
	
	@RequestMapping(value = "/downloadExcelWaterLevelList.do")
	public ModelAndView downloadExcelWaterLevelSumData(@ModelAttribute("searchVO") PageVO searchVO) throws Exception
	{
		try
		{	
			List<?> totalWaterLevelList = waterLevelSumDataService.selectWaterLevelSumDataList(searchVO);

			Map<String, Object> map = new HashMap<String, Object>();
			map.put("totalWaterLevelSumData", totalWaterLevelList);
			
			return new ModelAndView("wllExcelView", map);
		}
		catch (Exception e)
		{
		}
		
		return null;
	}
	
	@RequestMapping(value = "/downloadExcelSearchRainList.do")
	public ModelAndView downloadExcelSearchRainList(@ModelAttribute("searchVO") PageVO searchVO, HttpServletRequest request) throws Exception
	{
		try
		{
			HttpSession session = request.getSession();
			
			Object currentResult = session.getAttribute("currentResult");
			String cityName = (String)session.getAttribute("cityName");
			String excelViewName = (String)session.getAttribute("excelView");
			String excelParam = (String)session.getAttribute("excelParam");
			
			if (currentResult == null || cityName == null || excelViewName == null)
				return null;

			Map<String, Object> map = new HashMap<String, Object>();
			map.put("totalSearchRain", currentResult);
			map.put("cityName", cityName);
			
			if (excelParam != null)
				map.put("excelParam", excelParam);
			
			return new ModelAndView(excelViewName, map);
		}
		catch (Exception e)
		{
		}
		
		return null;
	}
	
	@RequestMapping(value = "/downloadExcelSearchSnowList.do")
	public ModelAndView downloadExcelSearchSnowList(@ModelAttribute("searchVO") PageVO searchVO, HttpServletRequest request) throws Exception
	{
		try
		{
			HttpSession session = request.getSession();
			
			Object currentResult = session.getAttribute("currentResult");
			String cityName = (String)session.getAttribute("cityName");
			String excelViewName = (String)session.getAttribute("excelView");
			String excelParam = (String)session.getAttribute("excelParam");
			
			if (currentResult == null || cityName == null || excelViewName == null)
				return null;

			Map<String, Object> map = new HashMap<String, Object>();
			map.put("totalSearchSnow", currentResult);
			map.put("cityName", cityName);
			
			if (excelParam != null)
				map.put("excelParam", excelParam);
			
			return new ModelAndView(excelViewName, map);
		}
		catch (Exception e)
		{
		}
		
		return null;
	}
	
	@RequestMapping(value = "/downloadExcelSearchWaterLevelList.do")
	public ModelAndView downloadExcelSearchWaterLevelList(@ModelAttribute("searchVO") PageVO searchVO, HttpServletRequest request) throws Exception
	{
		try
		{
			HttpSession session = request.getSession();
			
			Object currentResult = session.getAttribute("currentResult");
			String cityName = (String)session.getAttribute("cityName");
			String excelViewName = (String)session.getAttribute("excelView");
			String excelParam = (String)session.getAttribute("excelParam");
			
			if (currentResult == null || cityName == null || excelViewName == null)
				return null;

			Map<String, Object> map = new HashMap<String, Object>();
			map.put("totalSearchWaterLevel", currentResult);
			map.put("cityName", cityName);
			
			if (excelParam != null)
				map.put("excelParam", excelParam);
			
			return new ModelAndView(excelViewName, map);
		}
		catch (Exception e)
		{
		}
		
		return null;
	}
	
	@RequestMapping(value = "/downloadExcelPrintReport.do")
	public ModelAndView downloadExcelPrintReport(@ModelAttribute("searchVO") PageVO searchVO, HttpServletRequest request) throws Exception
	{
		try
		{
			HttpSession session = request.getSession();
			
			String radarImageURL = (String)session.getAttribute("radarImageURL");
			List<SpecialNews> specialNewsList = (List<SpecialNews>)session.getAttribute("newsList");
			List<ReportData> reportDataList = (List<ReportData>)session.getAttribute("currentResult"); 
			List<ReportData> minMaxList = (List<ReportData>)session.getAttribute("minMaxList");
			String excelViewName = (String)session.getAttribute("excelView");
			String excelParam = (String)session.getAttribute("excelParam");
			String cityName = (String)session.getAttribute("cityName");
			
			if (specialNewsList == null || reportDataList == null || minMaxList == null || cityName == null)
				return null;

			Map<String, Object> map = new HashMap<String, Object>();
			map.put("radarImageURL", radarImageURL);
			map.put("newsList", specialNewsList);
			map.put("currentResult", reportDataList); 
			map.put("minMaxList", minMaxList);
			map.put("cityName", cityName);
			
			if (excelParam != null)
				map.put("excelParam", excelParam);
			
			return new ModelAndView(excelViewName, map);
		}
		catch (Exception e)
		{
		}
		
		return null;
	}
	
	@RequestMapping(value = "/errorPage.do")
	public String showErrorPage(@ModelAttribute("searchVO") PageVO searchVO, ModelMap model) throws Exception
	{
		return "/error/dataError";
	}
	
	private void trace(String strMessage)
	{
		System.out.println(strMessage);
	}
}
