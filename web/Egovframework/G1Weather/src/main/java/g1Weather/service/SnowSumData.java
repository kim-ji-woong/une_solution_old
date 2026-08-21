package g1Weather.service;

public class SnowSumData extends PageVO {
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
	 *  강설유무.
	 */
	private String snow = "";
	
	/**
	 *  신적설
	 */
	private String snowNew = UnknownResult;
	
	/**
	 *  오늘 적설
	 */
	private String snowNow = UnknownResult;
	
	/**
	 *  어제 적설
	 */
	private String snowYesterday = UnknownResult;
	
	/**
	 *  어제 강수량
	 *  0보다 작으면 알수 없음
	 */
	private String rainYesterday = UnknownResult;
	
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
	
	public void setSnow(String snow)
	{
		SomeType<Integer> snowFlag = new SomeType<Integer>(0);
		
		if (tryParseInt(snow, snowFlag) == false)
			this.snow = UnknownResult;
		else
		{
			if (snowFlag.getValue() == 1)
				this.snow = "O";
			else
				this.snow = UnknownResult;
		}
	}
	
	public String getSnow()
	{
		return this.snow;
	}
	
	public void setSnowNew(String snow)
	{
		SomeType<Double> newSnow = new SomeType<Double>(0.0);
		
		if (tryParseDouble(snow, newSnow) == false)
			this.snowNew = UnknownResult;
		else
		{
			if (newSnow.getValue() < 0.0)
				this.snowNew = UnknownResult;
			else
				this.snowNew = Double.toString(newSnow.getValue() / 10);
		}
	}
	
	public String getSnowNew()
	{
		return this.snowNew;
	}
	
	public void setSnowNow(String snow)
	{
		SomeType<Double> nowSnow = new SomeType<Double>(0.0);
		
		if (tryParseDouble(snow, nowSnow) == false)
			this.snowNow = UnknownResult;
		else
		{
			if (nowSnow.getValue() < 0.0)
				this.snowNow = UnknownResult;
			else
				this.snowNow = Double.toString(nowSnow.getValue() / 10);
		}
	}
	
	public String getSnowNow()
	{
		return this.snowNow;
	}
	
	public void setSnowYesterday(String snow)
	{
		SomeType<Double> yesterSnow = new SomeType<Double>(0.0);
		
		if (tryParseDouble(snow, yesterSnow) == false)
			this.snowYesterday = UnknownResult;
		else
		{
			if (yesterSnow.getValue() < 0.0)
				this.snowYesterday = UnknownResult;
			else
				this.snowYesterday = Double.toString(yesterSnow.getValue() / 10);
		}
	}
	
	public String getSnowYesterday()
	{
		return this.snowYesterday;
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
