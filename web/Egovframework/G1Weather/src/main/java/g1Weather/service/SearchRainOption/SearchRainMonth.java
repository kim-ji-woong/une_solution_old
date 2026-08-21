package g1Weather.service.SearchRainOption;

import g1Weather.service.SearchRain;

public class SearchRainMonth extends SearchRain {
	private String rainDate = "";
	private double daySum = 0.0;
	private String locationID = "";
	private String locationName = "";
	
	public String getRainDate()
	{
		return rainDate;
	}
	
	public void setRainDate(String rainDate)
	{
		this.rainDate = rainDate;
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
