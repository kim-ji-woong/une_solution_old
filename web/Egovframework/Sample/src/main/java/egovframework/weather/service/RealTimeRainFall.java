package egovframework.weather.service;

import egovframework.weather.service.WeatherDefaultVO;

public class RealTimeRainFall extends WeatherDefaultVO {
	/**
	 *  serialVersion UID
	 */
	private static final long serialVersionUID = 10L;
	
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
	 *  강수유무.
	 */
	private String raining = "";
	//private int raining = -1;
	
	/**
	 *  이동 15분 강수량
	 *  0보다 작으면 알수 없음
	 */
	private String rain15M = "-";
	//private int rain15M = -1;
	
	/**
	 *  이동 60분 강수량
	 *  0보다 작으면 알수 없음
	 */
	private String rain60M = "-";
	//private int rain60M = -1;
	
	/**
	 *  오늘 강수량
	 *  0보다 작으면 알수 없음
	 */
	private String rainToday = "-";
	//private int rainToday = -1;
	
	/**
	 *  어제 강수량
	 *  0보다 작으면 알수 없음
	 */
	private String rainYesterday = "-";
	//private int rainYesterday = -1;
	
	/**
	 *  기온
	 *  0보다 작으면 알수 없음
	 */
	private String temperature = "-";
	//private int temperature = -1;
	
	/**
	 *  풍향1M
	 */
	private String windDirection1M = "-";
	//private int windDirection1M = -1;
	
	/**
	 *  풍속1M
	 *  0보다 작으면 알수 없음
	 */
	private String windSpeed1M = "-";
	//private int windSpeed1M = -1;
	
	/**
	 *  습도
	 *  0보다 작으면 알수 없음
	 */
	private String humidity = "-";
	//private int humidity = -1;
	
	/**
	 *  비고
	 */
	private String description = "";
	
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
	
	public void setRaining(String raining)
	{
		SomeType<Integer> rainFlag = new SomeType<Integer>(0);
		
		if (tryParseInt(raining, rainFlag) == false)
			this.raining = "";
		else
		{
			if (rainFlag.getValue() == 1)
				this.raining = "O";
			else
				this.raining = "-";
		}
	}
	
	public String getRaining()
	{
		return this.raining;
	}
	
	public void setRain15M(String rain)
	{
		SomeType<Double> rain15 = new SomeType<Double>(0.0);
		
		if (tryParseDouble(rain, rain15) == false)
			this.rain15M = "-";
		else
		{
			this.rain15M = Double.toString(rain15.getValue() / 10);
		}
	}
	
	public String getRain15M()
	{
		return this.rain15M;
	}
	
	public void setRain60M(String rain)
	{
		SomeType<Double> rain60 = new SomeType<Double>(0.0);
		
		if (tryParseDouble(rain, rain60) == false)
			this.rain60M = "-";
		else
		{
			this.rain60M = Double.toString(rain60.getValue() / 10);
		}
	}
	
	public String getRain60M()
	{
		return this.rain60M;
	}
	
	public void setRainToday(String rain)
	{
		SomeType<Double> rainToday = new SomeType<Double>(0.0);
		
		if (tryParseDouble(rain, rainToday) == false)
			this.rainToday = "-";
		else
		{
			this.rainToday = Double.toString(rainToday.getValue() / 10);
		}
	}
	
	public String getRainToday()
	{
		return this.rainToday;
	}
	
	public void setRainYesterday(String rain)
	{
		this.rainYesterday = rain;
	}
	
	public String getRainYesterday()
	{
		return this.rainYesterday;
	}
	
	public void setTemperature(String temperature)
	{
		this.temperature = temperature;
	}
	
	public String getTemperature()
	{
		return this.temperature;
	}
	
	public void setWindDirection1M(String windDirection1M)
	{
		this.windDirection1M = windDirection1M;
	}
	
	public String getWindDirection1M()
	{
		return this.windDirection1M;
	}
	
	public void setWindSpeed1M(String windSpeed1M)
	{
		this.windSpeed1M = windSpeed1M;
	}
	
	public String getWindSpeed1M()
	{
		return this.windSpeed1M;
	}
	
	public void setHumidity(String humidity)
	{
		this.humidity = humidity;
	}
	
	public String getHumidity()
	{
		return this.humidity;
	}
	
	/*public void setRaining(int raining)
	{
		this.raining = raining;
	}
	
	public int getRaining()
	{
		return this.raining;
	}
	
	public void setRain15M(int rain)
	{
		this.rain15M = rain;
	}
	
	public int getRain15M()
	{
		return this.rain15M;
	}
	
	public void setRain60M(int rain)
	{
		this.rain60M = rain;
	}
	
	public int getRain60M()
	{
		return this.rain60M;
	}
	
	public void setRainToday(int rain)
	{
		this.rainToday = rain;
	}
	
	public int getRainToday()
	{
		return this.rainToday;
	}
	
	public void setRainYesterday(int rain)
	{
		this.rainYesterday = rain;
	}
	
	public int getRainYesterday()
	{
		return this.rainYesterday;
	}
	
	public void setTemperature(int temperature)
	{
		this.temperature = temperature;
	}
	
	public int getTemperature()
	{
		return this.temperature;
	}
	
	public void setWindDirection1M(int windDirection1M)
	{
		this.windDirection1M = windDirection1M;
	}
	
	public int getWindDirection1M()
	{
		return this.windDirection1M;
	}
	
	public void setWindSpeed1M(int windSpeed1M)
	{
		this.windSpeed1M = windSpeed1M;
	}
	
	public int getWindSpeed1M()
	{
		return this.windSpeed1M;
	}
	
	public void setHumidity(int humidity)
	{
		this.humidity = humidity;
	}
	
	public int getHumidity()
	{
		return this.humidity;
	}*/
	
	public void setDescription(String description)
	{
		this.description = description;
	}
	
	public String getDescription()
	{
		return this.description;
	}
}
