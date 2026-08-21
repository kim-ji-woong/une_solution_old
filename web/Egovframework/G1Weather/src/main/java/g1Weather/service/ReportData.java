package g1Weather.service;

import java.util.ArrayList;
import java.util.List;

import g1Weather.service.PageVO;

public class ReportData extends PageVO {
private static final String UnknownResult = "";
	
	/**
	 *  시군코드
	 */
	private int cityCode = 0;
	/**
	 *  시군
	 */
	private String cityName = "";
	
	/**
	 *  지점번호
	 */
	private String locationNumber = "";
	
	/**
	 *  지점명
	 */
	private String locationName = "";
	
	private String timeStamp = "";
	
	/**
	 *  관측값
	 */
	private double daySum = 0.0;
	private String daySumString = "0.0";
	
	/**
	 *  일일 최대 관측값
	 */
	private double dayMax = 0.0;
	
	/**
	 * 구분
	 */
	private String mountName = "";
	
	private List<String> printItems = new ArrayList();
	private List<String> printDetailItems = new ArrayList();	
	
	/**
	 *  비고
	 */
	private String description = "";
	
	private double todayVal = 0.0;
	private String todayValString = "0.0";
	private double yesterdayVal = 0.0;
	private String yesterdayValString = "0.0";
	
	public void setCityCode(int cityCode)
	{
		this.cityCode = cityCode;
	}
	
	public int getCityCode()
	{
		return this.cityCode;
	}
	
	public void setCityName(String cityName)
	{
		this.cityName = cityName;
	}
	
	public String getCityName()
	{
		return this.cityName;
	}
	
	public void setLocationNumber(String locationNumber)
	{
		this.locationNumber = locationNumber;
	}
	
	public String getLocationNumber()
	{
		return this.locationNumber;
	}
	
	public void setLocationName(String locationName)
	{
		this.locationName = locationName;
	}
	
	public String getLocationName()
	{
		return this.locationName;
	}
	
	public void setTimeStamp(String timeStamp)
	{
		int len = timeStamp.length();
		
		if (len == 14)
		{
			String strYear = timeStamp.substring(0, 4);
			String strMonth = timeStamp.substring(4, 6);
			String strDay = timeStamp.substring(6, 8);
			String strHour = timeStamp.substring(8, 10);
			String strMin = timeStamp.substring(10, 12);
			String strSecond = timeStamp.substring(12);
			
			this.timeStamp = strYear + "-" + strMonth + "-" + strDay + " " + strHour + ":" + strMin + ":" + strSecond;
		}
		else if (len == 12)
		{
			String strYear = timeStamp.substring(0, 4);
			String strMonth = timeStamp.substring(4, 6);
			String strDay = timeStamp.substring(6, 8);
			String strHour = timeStamp.substring(8, 10);
			String strMin = timeStamp.substring(10, 12);
			
			this.timeStamp = strYear + "-" + strMonth + "-" + strDay + " " + strHour + ":" + strMin + ":00";
		}
		else if (len == 8)
		{
			String strYear = timeStamp.substring(0, 4);
			String strMonth = timeStamp.substring(4, 6);
			String strDay = timeStamp.substring(6, 8);
			
			this.timeStamp = strYear + "-" + strMonth + "-" + strDay;
		}
		else
			this.timeStamp = timeStamp;
	}
	
	public String getTimeStamp()
	{
		return this.timeStamp;
	}
	
	public void setDaySum(double daySum)
	{
		this.daySum = daySum;
		this.daySumString = SearchRain.DoubleToString(this.daySum);
	}
	
	public double getDaySum()
	{
		return this.daySum;
	}
	
	public void setTodayVal(double todayVal)
	{
		this.todayVal = todayVal;
		this.todayValString = SearchRain.DoubleToString(this.todayVal);
	}
	
	public String getTodayVal()
	{
		return this.todayValString;
	}
	public void setYesterdayVal(double yesterdayVal)
	{
		this.yesterdayVal = yesterdayVal;
		this.yesterdayValString = SearchRain.DoubleToString(this.yesterdayVal);
	}
	
	public String getYesterdayVal()
	{
		return this.yesterdayValString;
	}
	
	public String getDaySumString()
	{
		return this.daySumString;
	}
	
	public void setDayMax(double max)
	{
		this.dayMax = max;
	}
	
	public double getDayMax()
	{
		return this.dayMax;
	}
	
	public void setMountName(String mountName)
	{
		this.mountName = mountName;
	}
	
	public String getMountName()
	{
		return this.mountName;
	}
	
	public List<String> getPrintItems()
	{
		return this.printItems;
	}
	
	public List<String> getPrintDetailItems()
	{
		return this.printDetailItems;
	}
	
	public void setDescription(String description)
	{
		this.description = description;
	}
	
	public String getDescription()
	{
		return this.description;
	}
}
