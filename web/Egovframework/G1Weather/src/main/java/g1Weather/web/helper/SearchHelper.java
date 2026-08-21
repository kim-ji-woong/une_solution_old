package g1Weather.web.helper;

import g1Weather.service.ReportData;
import g1Weather.service.SearchRain;
import g1Weather.service.SearchSnow;
import g1Weather.service.SearchWaterLevel;
import g1Weather.service.SearchRainOption.SearchRainMonth;
import g1Weather.service.SearchRainOption.SearchRainToday;
import g1Weather.service.SearchRainOption.SearchRainYear;
import g1Weather.service.SearchSnowOption.SearchSnowToday;
import g1Weather.service.SearchSnowOption.SearchSnowMonth;
import g1Weather.service.SearchSnowOption.SearchSnowYear;
import g1Weather.service.SearchWaterLevelOption.SearchWaterLevelDay;
import g1Weather.service.data.CityTown;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.GregorianCalendar;
import java.util.HashMap;
import java.util.List;

public class SearchHelper {
	public static SearchRainToday getAverageSearchRainToday(List<SearchRainToday> totalRainList)
	{
		int nCount = 0;
		double h00 = 0.0, h01 = 0.0, h02 = 0.0, h03 = 0.0, h04 = 0.0, h05 = 0.0, h06 = 0.0;
		double h07 = 0.0, h08 = 0.0, h09 = 0.0, h10 = 0.0, h11 = 0.0, h12 = 0.0;
		double h13 = 0.0, h14 = 0.0, h15 = 0.0, h16 = 0.0, h17 = 0.0, h18 = 0.0;
		double h19 = 0.0, h20 = 0.0, h21 = 0.0, h22 = 0.0, h23 = 0.0;
		
		for (Object item : totalRainList)
		{
			SearchRainToday search = (SearchRainToday)item;
			
			h00 += toDouble(search.getH00());
			h01 += toDouble(search.getH01());
			h02 += toDouble(search.getH02());
			h03 += toDouble(search.getH03());
			h04 += toDouble(search.getH04());
			h05 += toDouble(search.getH05());
			h06 += toDouble(search.getH06());
			h07 += toDouble(search.getH07());
			h08 += toDouble(search.getH08());
			h09 += toDouble(search.getH09());
			h10 += toDouble(search.getH10());
			h11 += toDouble(search.getH11());
			h12 += toDouble(search.getH12());
			h13 += toDouble(search.getH13());
			h14 += toDouble(search.getH14());
			h15 += toDouble(search.getH15());
			h16 += toDouble(search.getH16());
			h17 += toDouble(search.getH17());
			h18 += toDouble(search.getH18());
			h19 += toDouble(search.getH19());
			h20 += toDouble(search.getH20());
			h21 += toDouble(search.getH21());
			h22 += toDouble(search.getH22());
			h23 += toDouble(search.getH23());
			
			nCount++;
		}
		
		SearchRainToday result = new SearchRainToday();
		result.setSumData(true);
		result.setLocationName("평균");
		
		if (nCount == 0)
			nCount = 1;
		
		// 조회값에 0.1을 곱해서 보여주므로 미리 10을 곱해서 원래값이 나오도록 한다.
		int times = 10;
		
		result.setH00(SearchRain.DoubleToString(h00 * times / nCount));
		result.setH01(SearchRain.DoubleToString(h01 * times / nCount));
		result.setH02(SearchRain.DoubleToString(h02 * times / nCount));
		result.setH03(SearchRain.DoubleToString(h03 * times / nCount));
		result.setH04(SearchRain.DoubleToString(h04 * times / nCount));
		result.setH05(SearchRain.DoubleToString(h05 * times / nCount));
		result.setH06(SearchRain.DoubleToString(h06 * times / nCount));
		result.setH07(SearchRain.DoubleToString(h07 * times / nCount));
		result.setH08(SearchRain.DoubleToString(h08 * times / nCount));
		result.setH09(SearchRain.DoubleToString(h09 * times / nCount));
		result.setH10(SearchRain.DoubleToString(h10 * times / nCount));
		result.setH11(SearchRain.DoubleToString(h11 * times / nCount));
		result.setH12(SearchRain.DoubleToString(h12 * times / nCount));
		result.setH13(SearchRain.DoubleToString(h13 * times / nCount));
		result.setH14(SearchRain.DoubleToString(h14 * times / nCount));
		result.setH15(SearchRain.DoubleToString(h15 * times / nCount));
		result.setH16(SearchRain.DoubleToString(h16 * times / nCount));
		result.setH17(SearchRain.DoubleToString(h17 * times / nCount));
		result.setH18(SearchRain.DoubleToString(h18 * times / nCount));
		result.setH19(SearchRain.DoubleToString(h19 * times / nCount));
		result.setH20(SearchRain.DoubleToString(h20 * times / nCount));
		result.setH21(SearchRain.DoubleToString(h21 * times / nCount));
		result.setH22(SearchRain.DoubleToString(h22 * times / nCount));
		result.setH23(SearchRain.DoubleToString(h23 * times / nCount));
		
		result.setMax("");
		return result;
	}
	
	public static SearchSnowToday getAverageSearchSnowToday(List<SearchSnowToday> totalSnowList)
	{
		int nCount = 0;
		double h00 = 0.0, h01 = 0.0, h02 = 0.0, h03 = 0.0, h04 = 0.0, h05 = 0.0, h06 = 0.0;
		double h07 = 0.0, h08 = 0.0, h09 = 0.0, h10 = 0.0, h11 = 0.0, h12 = 0.0;
		double h13 = 0.0, h14 = 0.0, h15 = 0.0, h16 = 0.0, h17 = 0.0, h18 = 0.0;
		double h19 = 0.0, h20 = 0.0, h21 = 0.0, h22 = 0.0, h23 = 0.0;
		
		for (Object item : totalSnowList)
		{
			SearchSnowToday search = (SearchSnowToday)item;
			
			h00 += toDouble(search.getH00());
			h01 += toDouble(search.getH01());
			h02 += toDouble(search.getH02());
			h03 += toDouble(search.getH03());
			h04 += toDouble(search.getH04());
			h05 += toDouble(search.getH05());
			h06 += toDouble(search.getH06());
			h07 += toDouble(search.getH07());
			h08 += toDouble(search.getH08());
			h09 += toDouble(search.getH09());
			h10 += toDouble(search.getH10());
			h11 += toDouble(search.getH11());
			h12 += toDouble(search.getH12());
			h13 += toDouble(search.getH13());
			h14 += toDouble(search.getH14());
			h15 += toDouble(search.getH15());
			h16 += toDouble(search.getH16());
			h17 += toDouble(search.getH17());
			h18 += toDouble(search.getH18());
			h19 += toDouble(search.getH19());
			h20 += toDouble(search.getH20());
			h21 += toDouble(search.getH21());
			h22 += toDouble(search.getH22());
			h23 += toDouble(search.getH23());
			
			nCount++;
		}
		
		SearchSnowToday result = new SearchSnowToday();
		result.setSumData(true);
		result.setLocationName("평균");
		
		if (nCount == 0)
			nCount = 1;
		
		// 조회값에 0.1을 곱해서 보여주므로 미리 10을 곱해서 원래값이 나오도록 한다.
		int times = 10;
				
		result.setH00(SearchSnow.DoubleToString(h00 * times / nCount));
		result.setH01(SearchSnow.DoubleToString(h01 * times / nCount));
		result.setH02(SearchSnow.DoubleToString(h02 * times / nCount));
		result.setH03(SearchSnow.DoubleToString(h03 * times / nCount));
		result.setH04(SearchSnow.DoubleToString(h04 * times / nCount));
		result.setH05(SearchSnow.DoubleToString(h05 * times / nCount));
		result.setH06(SearchSnow.DoubleToString(h06 * times / nCount));
		result.setH07(SearchSnow.DoubleToString(h07 * times / nCount));
		result.setH08(SearchSnow.DoubleToString(h08 * times / nCount));
		result.setH09(SearchSnow.DoubleToString(h09 * times / nCount));
		result.setH10(SearchSnow.DoubleToString(h10 * times / nCount));
		result.setH11(SearchSnow.DoubleToString(h11 * times / nCount));
		result.setH12(SearchSnow.DoubleToString(h12 * times / nCount));
		result.setH13(SearchSnow.DoubleToString(h13 * times / nCount));
		result.setH14(SearchSnow.DoubleToString(h14 * times / nCount));
		result.setH15(SearchSnow.DoubleToString(h15 * times / nCount));
		result.setH16(SearchSnow.DoubleToString(h16 * times / nCount));
		result.setH17(SearchSnow.DoubleToString(h17 * times / nCount));
		result.setH18(SearchSnow.DoubleToString(h18 * times / nCount));
		result.setH19(SearchSnow.DoubleToString(h19 * times / nCount));
		result.setH20(SearchSnow.DoubleToString(h20 * times / nCount));
		result.setH21(SearchSnow.DoubleToString(h21 * times / nCount));
		result.setH22(SearchSnow.DoubleToString(h22 * times / nCount));
		result.setH23(SearchSnow.DoubleToString(h23 * times / nCount));
		
		result.setMax("");
		return result;
	}
	
	public static SearchWaterLevelDay getAverageSearchWaterLevelDay(List<SearchWaterLevelDay> totalWaterLevelList)
	{
		int nCount = 0;
		double h00 = 0.0, h01 = 0.0, h02 = 0.0, h03 = 0.0, h04 = 0.0, h05 = 0.0, h06 = 0.0;
		double h07 = 0.0, h08 = 0.0, h09 = 0.0, h10 = 0.0, h11 = 0.0, h12 = 0.0;
		double h13 = 0.0, h14 = 0.0, h15 = 0.0, h16 = 0.0, h17 = 0.0, h18 = 0.0;
		double h19 = 0.0, h20 = 0.0, h21 = 0.0, h22 = 0.0, h23 = 0.0;
		
		for (Object item : totalWaterLevelList)
		{
			SearchWaterLevelDay search = (SearchWaterLevelDay)item;
			
			h00 += toDouble(search.getH00());
			h01 += toDouble(search.getH01());
			h02 += toDouble(search.getH02());
			h03 += toDouble(search.getH03());
			h04 += toDouble(search.getH04());
			h05 += toDouble(search.getH05());
			h06 += toDouble(search.getH06());
			h07 += toDouble(search.getH07());
			h08 += toDouble(search.getH08());
			h09 += toDouble(search.getH09());
			h10 += toDouble(search.getH10());
			h11 += toDouble(search.getH11());
			h12 += toDouble(search.getH12());
			h13 += toDouble(search.getH13());
			h14 += toDouble(search.getH14());
			h15 += toDouble(search.getH15());
			h16 += toDouble(search.getH16());
			h17 += toDouble(search.getH17());
			h18 += toDouble(search.getH18());
			h19 += toDouble(search.getH19());
			h20 += toDouble(search.getH20());
			h21 += toDouble(search.getH21());
			h22 += toDouble(search.getH22());
			h23 += toDouble(search.getH23());
			
			nCount++;
		}
		
		SearchWaterLevelDay result = new SearchWaterLevelDay();
		result.setSumData(true);
		result.setLocationName("평균");
		
		if (nCount == 0)
			nCount = 1;
		
		// 조회값에 0.1을 곱해서 보여주므로 미리 10을 곱해서 원래값이 나오도록 한다.
		int times = 10;
		
		result.setH00(SearchWaterLevel.DoubleToString(h00 * times / nCount));
		result.setH01(SearchWaterLevel.DoubleToString(h01 * times / nCount));
		result.setH02(SearchWaterLevel.DoubleToString(h02 * times / nCount));
		result.setH03(SearchWaterLevel.DoubleToString(h03 * times / nCount));
		result.setH04(SearchWaterLevel.DoubleToString(h04 * times / nCount));
		result.setH05(SearchWaterLevel.DoubleToString(h05 * times / nCount));
		result.setH06(SearchWaterLevel.DoubleToString(h06 * times / nCount));
		result.setH07(SearchWaterLevel.DoubleToString(h07 * times / nCount));
		result.setH08(SearchWaterLevel.DoubleToString(h08 * times / nCount));
		result.setH09(SearchWaterLevel.DoubleToString(h09 * times / nCount));
		result.setH10(SearchWaterLevel.DoubleToString(h10 * times / nCount));
		result.setH11(SearchWaterLevel.DoubleToString(h11 * times / nCount));
		result.setH12(SearchWaterLevel.DoubleToString(h12 * times / nCount));
		result.setH13(SearchWaterLevel.DoubleToString(h13 * times / nCount));
		result.setH14(SearchWaterLevel.DoubleToString(h14 * times / nCount));
		result.setH15(SearchWaterLevel.DoubleToString(h15 * times / nCount));
		result.setH16(SearchWaterLevel.DoubleToString(h16 * times / nCount));
		result.setH17(SearchWaterLevel.DoubleToString(h17 * times / nCount));
		result.setH18(SearchWaterLevel.DoubleToString(h18 * times / nCount));
		result.setH19(SearchWaterLevel.DoubleToString(h19 * times / nCount));
		result.setH20(SearchWaterLevel.DoubleToString(h20 * times / nCount));
		result.setH21(SearchWaterLevel.DoubleToString(h21 * times / nCount));
		result.setH22(SearchWaterLevel.DoubleToString(h22 * times / nCount));
		result.setH23(SearchWaterLevel.DoubleToString(h23 * times / nCount));
		
		return result;
	}
	
	public static List<String> getSearchRainTodayHeader()
	{
		List<String> searchHeader = new ArrayList();
		searchHeader.add("지점명");
		
		for (int i=0;i<=23;i++)
		{
			searchHeader.add(String.format("%d시", i));
		}
		
		searchHeader.add("금일우량");
		searchHeader.add("시간최대");
		return searchHeader;
	}
	
	public static List<String> getSearchRainMonthHeader(int year, int month)
	{
		Calendar cal = new GregorianCalendar(year, month - 1, 1);
		int daysInMonth = cal.getActualMaximum(Calendar.DAY_OF_MONTH);
		
		List<String> searchHeader = new ArrayList();
		searchHeader.add("지점명");
		
		for (int i=1;i<=daysInMonth;i++)
		{
			searchHeader.add(String.format("%d일", i));
		}
		
		searchHeader.add("합계");
		return searchHeader;
	}
	
	public static List<String> getSearchRainYearHeader()
	{
		List<String> searchHeader = new ArrayList();
		searchHeader.add("지점명");
		
		for (int i=1;i<=12;i++)
		{
			searchHeader.add(String.format("%d월", i));
		}
		
		searchHeader.add("합계");
		return searchHeader;
	}
	
	public static List<String> getSearchRainPeriodHeader(String locationID, List<CityTown> cityTowns)
	{
		String locationName = "";
		
		for (CityTown town : cityTowns)
		{
			if (locationID.equals(town.getLocationID()))
			{
				locationName = town.getLocationName();
				break;
			}
		}
		
		List<String> searchHeader = new ArrayList();
		searchHeader.add(locationName);
		
		for (int i=0;i<=23;i++)
		{
			searchHeader.add(String.format("%d시", i));
		}
		
		searchHeader.add("합계");
		return searchHeader;
	}
	
	public static List<String> getSearchSnowTodayHeader()
	{
		List<String> searchHeader = new ArrayList();
		searchHeader.add("지점명");
		
		for (int i=0;i<=23;i++)
		{
			searchHeader.add(String.format("%d시", i));
		}
		
		searchHeader.add("신적설");
		return searchHeader;
	}
	
	public static List<String> getSearchSnowMonthHeader(int year, int month)
	{
		Calendar cal = new GregorianCalendar(year, month - 1, 1);
		int daysInMonth = cal.getActualMaximum(Calendar.DAY_OF_MONTH);
		
		List<String> searchHeader = new ArrayList();
		searchHeader.add("지점명");
		
		for (int i=1;i<=daysInMonth;i++)
		{
			searchHeader.add(String.format("%d일", i));
		}
		
		searchHeader.add("합계");
		return searchHeader;
	}
	
	public static List<String> getSearchSnowYearHeader()
	{
		List<String> searchHeader = new ArrayList();
		searchHeader.add("지점명");
		
		for (int i=1;i<=12;i++)
		{
			searchHeader.add(String.format("%d월", i));
		}
		
		searchHeader.add("합계");
		return searchHeader;
	}
	
	public static List<String> getSearchSnowPeriodHeader(String locationID, List<CityTown> cityTowns)
	{
		String locationName = "";
		
		for (CityTown town : cityTowns)
		{
			if (locationID.equals(town.getLocationID()))
			{
				locationName = town.getLocationName();
				break;
			}
		}
		
		List<String> searchHeader = new ArrayList();
		searchHeader.add(locationName);
		
		for (int i=0;i<=23;i++)
		{
			searchHeader.add(String.format("%d시", i));
		}
		
		searchHeader.add("합계");
		return searchHeader;
	}
	
	public static List<String> getSearchWaterLevelDayHeader()
	{
		List<String> searchHeader = new ArrayList();
		searchHeader.add("지점명");
		
		for (int i=0;i<=23;i++)
		{
			searchHeader.add(String.format("%d시", i));
		}
		
		return searchHeader;
	}
	
	public static List<String> getSearchWaterLevelPeriodHeader(String locationID, List<CityTown> cityTowns)
	{
		String locationName = "";
		
		for (CityTown town : cityTowns)
		{
			if (locationID.equals(town.getLocationID()))
			{
				locationName = town.getLocationName();
				break;
			}
		}
		
		List<String> searchHeader = new ArrayList();
		searchHeader.add(locationName);
		
		for (int i=0;i<=23;i++)
		{
			searchHeader.add(String.format("%d시", i));
		}
		
		return searchHeader;
	}
	
	public static List<String> getPrintReportDataHeader(boolean isRain)
	{
		List<String> headers = new ArrayList();
		
		SimpleDateFormat date = new SimpleDateFormat("MM-dd"); 
		Calendar cal = Calendar.getInstance();
		String todayStr = date.format(cal.getTime());		
		cal.add(cal.DATE, -1);
		String yesterdayStr = date.format(cal.getTime());
		
		if (isRain)
		{
			headers.add("시군");
			headers.add(yesterdayStr);
			headers.add(todayStr);
			headers.add("누적강수량 (mm)"); 
			headers.add("비고");
		}
		else
		{
			headers.add("시군");
			headers.add("구분");
			headers.add(yesterdayStr);
			headers.add(todayStr);
			headers.add("누적적설 (cm)");
			headers.add("최대값");
		}
		
		return headers;
	}
	
	public static void getPrintReportDataHeader2(boolean isRain, String beginDateCondition, String endDateCondition, List<String> headers1, List<String> headers2, List<String> headers3, List<String> headersUp)
	{
		SimpleDateFormat date = new SimpleDateFormat("MM-dd");
		
		Calendar cal = Calendar.getInstance();
		String todayStr = date.format(cal.getTime());
		
		cal.add(cal.DATE, -1);
		String yesterdayStr = date.format(cal.getTime());
		
		String beginDate = beginDateCondition.substring(0, 4) + "-" + beginDateCondition.substring(4, 6) + "-" + beginDateCondition.substring(6, 8);
		String endDate = endDateCondition.substring(0, 4) + "-" + endDateCondition.substring(4, 6) + "-" + endDateCondition.substring(6, 8);
		
		String upHeader = "누적강수량 (" + beginDate + "~" + endDate + ")";
		
		if (isRain)
		{
			headers1.add("시군");
			headersUp.add("누적강수량 (" + beginDate + "~" + endDate + ")");
			headers2.add("비고");
			headers3.add(yesterdayStr);
			headers3.add(todayStr);
			headers3.add("계"); 
		}
		else
		{
			headers1.add("시군");
			headers1.add("구분");
			headersUp.add("누적적설량 (" + beginDate + "~" + endDate + ")");
			headers2.add("비고");
			headers3.add(yesterdayStr);
			headers3.add(todayStr);
			headers3.add("계"); 
		}
	}
	
	public static List<String> getSearchYearList(int firstYear, boolean asc)
	{
		List<String> years = new ArrayList();
		int currentYear = Calendar.getInstance().get(Calendar.YEAR);
		
		if (asc)
		{
			for (int i=currentYear;i>=firstYear;i--)
			{
				String year = Integer.toString(i);
				years.add(year);
			}
		}
		else
		{
			for (int i=firstYear;i<=currentYear;i++)
			{
				String year = Integer.toString(i);
				years.add(year);
			}
		}
		
		return years;
	}
	
	// totalRainList에 towns에 존재하는 지역 가운데 없는 것이 있으면 새로 채워 넣는다.
	public static void addCityTowns(List<SearchRainToday> totalRainList, List<CityTown> towns)
	{
		List<SearchRainToday> newList = new ArrayList();
		
		for (CityTown town : towns)
		{
			SearchRainToday searchRain = findSearchRainToday(town.getLocationID(), totalRainList);
			
			if (searchRain == null)
			{
				searchRain = new SearchRainToday();
				searchRain.setLocationID(town.getLocationID());
				searchRain.setLocationName(town.getLocationName());
				newList.add(searchRain);
			}
		}
		
		for (SearchRainToday rain : newList)
		{
			totalRainList.add(rain);
		}
	}
	
	private static SearchRainToday findSearchRainToday(String locationID, List<SearchRainToday> totalRainList)
	{
		for (SearchRainToday searchRain : totalRainList)
		{
			if (searchRain.getLocationID().equals(locationID))
				return searchRain;
		}
		
		return null;
	}
	
	private static double toDouble(String data)
	{
		try
		{
			double num = Double.parseDouble(data);
			return num;
		}
		catch (NumberFormatException e)
		{
		}
		
		return 0.0;
	}
	
	public static List<SearchRain> makeSearchRainMonthList(int year, int month, List<SearchRainMonth> totalRainList)
	{
		Calendar cal = new GregorianCalendar(year, month - 1, 1);
		int daysInMonth = cal.getActualMaximum(Calendar.DAY_OF_MONTH);
		
		// Key : Location ID
		HashMap<String, SearchRain> locationMap = new HashMap<String, SearchRain>();
		
		for (SearchRainMonth searchRain : totalRainList)
		{
			SearchRain rain = locationMap.get(searchRain.getLocationID());
			
			if (rain == null)
			{
				rain = makeNewSearchRainMonth(daysInMonth, searchRain);
				locationMap.put(searchRain.getLocationID(), rain);
			}
			
			setSearchRainMonthData(rain, searchRain, daysInMonth);
		}
		
		return new ArrayList(locationMap.values());
	}
	
	public static List<SearchSnow> makeSearchSnowMonthList(int year, int month, List<SearchSnowMonth> totalSnowList)
	{
		Calendar cal = new GregorianCalendar(year, month - 1, 1);
		int daysInMonth = cal.getActualMaximum(Calendar.DAY_OF_MONTH);
		
		// Key : Location ID
		HashMap<String, SearchSnow> locationMap = new HashMap<String, SearchSnow>();
		
		for (SearchSnowMonth searchSnow : totalSnowList)
		{
			SearchSnow snow = locationMap.get(searchSnow.getLocationID());
			
			if (snow == null)
			{
				snow = makeNewSearchSnowMonth(daysInMonth, searchSnow);
				locationMap.put(searchSnow.getLocationID(), snow);
			}
			
			setSearchSnowMonthData(snow, searchSnow, daysInMonth);
		}
		
		return new ArrayList(locationMap.values());
	}
	
	public static void setSearchRainMonthData(SearchRain rain, SearchRainMonth month, int daysInMonth)
	{
		String rainDate = month.getRainDate().trim();
		
		if (rainDate.length() != 8)
			return;
		
		String strDay = rainDate.substring(6, 8);
		int day;
		
		try
		{
			day = Integer.parseInt(strDay);
		}
		catch (NumberFormatException e)
		{
			return;
		}
		
		List<SearchRain.RainData> items = rain.getItemValues();
		SearchRain.RainData item = items.get(day - 1);
		rain.setRainDataValue(item, SearchRain.DoubleToString(month.getDaySum()));
		
		double daySum = 0.0;
		SearchRain.RainData itemSum = items.get(daysInMonth);
		
		try
		{
			daySum = Double.parseDouble(itemSum.getValue());
		}
		catch (NumberFormatException e)
		{
		}
		
		daySum += month.getDaySum();
		rain.setRainDataValue(itemSum, SearchRain.DoubleToString(daySum));
	}
	
	public static void setSearchSnowMonthData(SearchSnow snow, SearchSnowMonth month, int daysInMonth)
	{
		String snowDate = month.getSnowDate().trim();
		
		if (snowDate.length() != 8)
			return;
		
		String strDay = snowDate.substring(6, 8);
		int day;
		
		try
		{
			day = Integer.parseInt(strDay);
		}
		catch (NumberFormatException e)
		{
			return;
		}
		
		List<SearchSnow.SnowData> items = snow.getItemValues();
		SearchSnow.SnowData item = items.get(day - 1);
		snow.setSnowDataValue(item, SearchSnow.DoubleToString(month.getDaySum()));
		
		double daySum = 0.0;
		SearchSnow.SnowData itemSum = items.get(daysInMonth);
		
		try
		{
			daySum = Double.parseDouble(itemSum.getValue());
		}
		catch (NumberFormatException e)
		{
		}
		
		daySum += month.getDaySum();
		snow.setSnowDataValue(itemSum, SearchSnow.DoubleToString(daySum));
	}
	
	public static SearchRain makeNewSearchRainMonth(int daysInMonth, SearchRainMonth month)
	{
		SearchRain rain = new SearchRain();
		
		rain.setLocationID(month.getLocationID());
		rain.setLocationName(month.getLocationName());
		
		String defData = SearchRain.DoubleToString(0.0);

		// 합계를 위하여 실제 날짜보다 하나 더 추가한다.
		for (int i=0;i<=daysInMonth;i++)
		{
			SearchRain.RainData data = rain.makeRainData();
			rain.setRainDataValue(data, defData);
			rain.getItemValues().add(data);
			
			if (i == daysInMonth)
			{
				data.setSumData(true);
			}
		}
		
		return rain;
	}
	
	public static SearchSnow makeNewSearchSnowMonth(int daysInMonth, SearchSnowMonth month)
	{
		SearchSnow snow = new SearchSnow();
		
		snow.setLocationID(month.getLocationID());
		snow.setLocationName(month.getLocationName());
		
		String defData = SearchSnow.DoubleToString(0.0);

		// 합계를 위하여 실제 날짜보다 하나 더 추가한다.
		for (int i=0;i<=daysInMonth;i++)
		{
			SearchSnow.SnowData data = snow.makeSnowData();
			snow.setSnowDataValue(data, defData);
			snow.getItemValues().add(data);
			
			if (i == daysInMonth)
			{
				data.setSumData(true);
			}
		}
		
		return snow;
	}
	
	public static List<SearchRain> makeSearchRainYearList(int year, List<SearchRainYear> totalRainList)
	{
		// Key : Location ID
		HashMap<String, SearchRain> locationMap = new HashMap<String, SearchRain>();
		
		for (SearchRainYear searchRain : totalRainList)
		{
			SearchRain rain = locationMap.get(searchRain.getLocationID());
			
			if (rain == null)
			{
				rain = makeNewSearchRainYear(searchRain);
				locationMap.put(searchRain.getLocationID(), rain);
			}
			
			setSearchRainYearData(rain, searchRain);
		}
		
		return new ArrayList(locationMap.values());
	}
	
	public static List<SearchSnow> makeSearchSnowYearList(int year, List<SearchSnowYear> totalSnowList)
	{
		// Key : Location ID
		HashMap<String, SearchSnow> locationMap = new HashMap<String, SearchSnow>();
		
		for (SearchSnowYear searchSnow : totalSnowList)
		{
			SearchSnow snow = locationMap.get(searchSnow.getLocationID());
			
			if (snow == null)
			{
				snow = makeNewSearchSnowYear(searchSnow);
				locationMap.put(searchSnow.getLocationID(), snow);
			}
			
			setSearchSnowYearData(snow, searchSnow);
		}
		
		return new ArrayList(locationMap.values());
	}
	
	public static SearchRain makeNewSearchRainYear(SearchRainYear year)
	{
		SearchRain rain = new SearchRain();
		
		rain.setLocationID(year.getLocationID());
		rain.setLocationName(year.getLocationName());
		
		String defData = SearchRain.DoubleToString(0.0);

		// 합계를 위하여 12개월보다 하나 더 추가한다.
		for (int i=0;i<=12;i++)
		{
			SearchRain.RainData data = rain.makeRainData();
			rain.setRainDataValue(data, defData);
			rain.getItemValues().add(data);
			
			if (i == 12)
			{
				data.setSumData(true);
			}
		}
		
		return rain;
	}
	
	public static SearchSnow makeNewSearchSnowYear(SearchSnowYear year)
	{
		SearchSnow snow = new SearchSnow();
		
		snow.setLocationID(year.getLocationID());
		snow.setLocationName(year.getLocationName());
		
		String defData = SearchSnow.DoubleToString(0.0);

		// 합계를 위하여 12개월보다 하나 더 추가한다.
		for (int i=0;i<=12;i++)
		{
			SearchSnow.SnowData data = snow.makeSnowData();
			snow.setSnowDataValue(data, defData);
			snow.getItemValues().add(data);
			
			if (i == 12)
			{
				data.setSumData(true);
			}
		}
		
		return snow;
	}
	
	public static void setSearchRainYearData(SearchRain rain, SearchRainYear year)
	{
		String rainDate = year.getRainDate().trim();
		
		if (rainDate.length() != 8)
			return;
		
		String strMonth = rainDate.substring(4, 6);
		int month;
		
		try
		{
			month = Integer.parseInt(strMonth);
		}
		catch (NumberFormatException e)
		{
			return;
		}
		
		List<SearchRain.RainData> items = rain.getItemValues();
		SearchRain.RainData item = items.get(month - 1);
		
		// 월간 합계
		double monthSum = 0.0;
		
		try
		{
			monthSum = Double.parseDouble(item.getValue());
		}
		catch (NumberFormatException e)
		{
		}
		
		monthSum += year.getDaySum();
		rain.setRainDataValue(item, SearchRain.DoubleToString(monthSum));
		
		// 연간 합계
		double totalSum = 0.0;
		SearchRain.RainData itemSum = items.get(12);
		
		try
		{
			totalSum = Double.parseDouble(itemSum.getValue());
		}
		catch (NumberFormatException e)
		{
		}
		
		totalSum += year.getDaySum();
		rain.setRainDataValue(itemSum, SearchRain.DoubleToString(totalSum));
	}
	
	public static void setSearchSnowYearData(SearchSnow snow, SearchSnowYear year)
	{
		String snowDate = year.getSnowDate().trim();
		
		if (snowDate.length() != 8)
			return;
		
		String strMonth = snowDate.substring(4, 6);
		int month;
		
		try
		{
			month = Integer.parseInt(strMonth);
		}
		catch (NumberFormatException e)
		{
			return;
		}
		
		List<SearchSnow.SnowData> items = snow.getItemValues();
		SearchSnow.SnowData item = items.get(month - 1);
		
		// 월간 합계
		double monthSum = 0.0;
		
		try
		{
			monthSum = Double.parseDouble(item.getValue());
		}
		catch (NumberFormatException e)
		{
		}
		
		monthSum += year.getDaySum();
		snow.setSnowDataValue(item, SearchSnow.DoubleToString(monthSum));
		
		// 연간 합계
		double totalSum = 0.0;
		SearchSnow.SnowData itemSum = items.get(12);
		
		try
		{
			totalSum = Double.parseDouble(itemSum.getValue());
		}
		catch (NumberFormatException e)
		{
		}
		
		totalSum += year.getDaySum();
		snow.setSnowDataValue(itemSum, SearchSnow.DoubleToString(totalSum));
	}
	
	public static List<ReportData> todayReportDataList = new ArrayList();
	private static void addTodayReportDataList(ReportData report)
	{
		todayReportDataList.add(report);
	}
	public static List<ReportData> yesterdayReportDataList = new ArrayList();
	private static void addYesterdayReportDataList(ReportData report)
	{
		yesterdayReportDataList.add(report);
	}
	
	// Key : Location Number
	// 
	public static List<ReportData> makeReportData(List<ReportData> totalReportDataList, List<ReportData> minMaxList, boolean isRain, boolean isToday, boolean isYesterday)
	{
		HashMap<String, ReportData> mapReport = new HashMap<String, ReportData>();
		
		ReportData maxReport = null;
		ReportData minReport = null;
	
		SimpleDateFormat date = new SimpleDateFormat("yyyyMMdd"); 
		Calendar cal = Calendar.getInstance();
		String todayStr = date.format(cal.getTime());		
		cal.add(cal.DATE, -1);
		String yesterdayStr = date.format(cal.getTime());
		//test
//		todayStr = "20170105";
//		yesterdayStr = "20170104";
		
		for (ReportData report : totalReportDataList)
		{  			
			if (report.getDaySum() > 0.0)
			{
				if (maxReport == null)
					maxReport = report;
				
				if (minReport == null)
					minReport = report;
				
				if (maxReport.getDaySum() < report.getDaySum())
					maxReport = report;
				
				if (minReport.getDaySum() > report.getDaySum())
					minReport = report;
			}
					
			ReportData data = mapReport.get(report.getLocationNumber());
			
			if (data == null)
			{
				data = new ReportData();
				mapReport.put(report.getLocationNumber(), data);
				
				data.setLocationNumber(report.getLocationNumber());
				data.setLocationName(report.getLocationName());
				data.setMountName(report.getMountName()); 
			}
			 
			String reportTimeStamp = report.getTimeStamp().replace("-", "");
			 
			//조회기간에 전일, 금일이 포함되어 있지 않은 경우는 daySum에 합쳐지면 안됨  			
			if (reportTimeStamp.equals(todayStr) || reportTimeStamp.equals(yesterdayStr))
			{
				if (reportTimeStamp.equals(todayStr) && isToday == true)
					data.setDaySum(report.getDaySum() + data.getDaySum());
				else if (reportTimeStamp.equals(yesterdayStr) && isYesterday == true)
					data.setDaySum(report.getDaySum() + data.getDaySum());
			}
			else
				data.setDaySum(report.getDaySum() + data.getDaySum());
			  			 
			// 현재 for값이 전일, 금일 데이터일때			
			if (reportTimeStamp.equals(todayStr) && data != null)  
				data.setTodayVal(report.getDaySum());  
			if (reportTimeStamp.equals(yesterdayStr) && data != null)  
				data.setYesterdayVal(report.getDaySum()); 	 
			
			if (data.getDayMax() < report.getDaySum())
			{
				data.setDayMax(report.getDaySum());
				data.setDescription(report.getTimeStamp());
			}			
		}
		
		if (minReport != null && maxReport != null)
		{
			minMaxList.add(minReport);
			minMaxList.add(maxReport);
		}
		
		List<ReportData> reportDataList = new ArrayList(mapReport.values());
		
		for (ReportData report : reportDataList)
		{
			if (report.getDescription().length() > 0)
			{
				report.setDescription(SearchRain.DoubleToString(report.getDayMax()) + "(" + report.getDescription() + ")");
			}
			
			if (isRain)
			{
				report.getPrintItems().add(report.getLocationName());
				report.getPrintItems().add(report.getDaySumString());
				report.getPrintItems().add(report.getDescription()); 
				
				report.getPrintDetailItems().add(report.getLocationName());
				report.getPrintDetailItems().add(report.getYesterdayVal());
				report.getPrintDetailItems().add(report.getTodayVal());				
				report.getPrintDetailItems().add(report.getDaySumString());
				report.getPrintDetailItems().add(report.getDescription());				
			}
			else
			{
				report.getPrintItems().add(report.getLocationName());
				report.getPrintItems().add(report.getMountName());
				report.getPrintItems().add(report.getDaySumString());
				report.getPrintItems().add(report.getDescription()); 
				
				report.getPrintDetailItems().add(report.getLocationName());
				report.getPrintDetailItems().add(report.getMountName());
				report.getPrintDetailItems().add(report.getYesterdayVal());
				report.getPrintDetailItems().add(report.getTodayVal());
				report.getPrintDetailItems().add(report.getDaySumString());
				report.getPrintDetailItems().add(report.getDescription()); 
			}
		}
		
		return reportDataList; 
	}
}
