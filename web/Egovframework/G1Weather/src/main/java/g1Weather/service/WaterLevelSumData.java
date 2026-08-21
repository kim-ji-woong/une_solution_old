package g1Weather.service;

public class WaterLevelSumData extends PageVO {
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
	
	/**
	 *  관측시각
	 */
	private String timeStamp = "";
	
	/**
	 *  해발표고
	 */
	private String standardWaterLevel = UnknownResult;
	
	/**
	 *  현재수위
	 */
	private String waterLevel = UnknownResult;
	
	/**
	 *  비고
	 */
	private String description = "";
	
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
	
	public void setStandardWaterLevel(String level)
	{
		SomeType<Double> swl = new SomeType<Double>(0.0);
		
		if (tryParseDouble(level, swl) == false)
			this.standardWaterLevel = UnknownResult;
		else
		{
			if (swl.getValue() < 0.0)
				this.standardWaterLevel = UnknownResult;
			else
				this.standardWaterLevel = Double.toString(swl.getValue() / 10);
		}
	}
	
	public String getStandardWaterLevel()
	{
		return this.standardWaterLevel;
	}
	
	public void setWaterLevel(String level)
	{
		SomeType<Double> cwl = new SomeType<Double>(0.0);
		
		if (tryParseDouble(level, cwl) == false)
			this.waterLevel = UnknownResult;
		else
		{
			if (cwl.getValue() < 0.0)
				this.waterLevel = UnknownResult;
			else
				this.waterLevel = Double.toString(cwl.getValue() / 10);
		}
	}
	
	public String getWaterLevel()
	{
		return this.waterLevel;
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
