package g1Weather.service.SearchSnowOption;

import g1Weather.service.SearchSnow;

public class SearchSnowMonth extends SearchSnow {
	private String snowDate = "";
	private double daySum = 0.0;
	private String locationID = "";
	private String locationName = "";
	
	public String getSnowDate()
	{
		return snowDate;
	}
	
	public void setSnowDate(String snowDate)
	{
		this.snowDate = snowDate;
	}
	
	public double getDaySum()
	{
		return daySum;
	}
	
	public void setDaySum(double daySum)
	{
		this.daySum = daySum * 0.1;
	}
	
	public String getLocationID()
	{
		return this.locationID;
	}
	
	public void setLocationID(String id)
	{
		this.locationID = id;
	}
	
	public String getLocationName()
	{
		return this.locationName;
	}
	
	public void setLocationName(String name)
	{
		this.locationName = name;
	}
}
