package g1Weather.service;

import g1Weather.service.PageVO.SomeType;

import java.text.DecimalFormat;
import java.util.ArrayList;
import java.util.List;

public class SearchWaterLevel extends PageVO {
	public class WaterLevelData
	{
		// 평균값인가?
		private boolean sumData = false;
		private String value = "";
		
		public boolean getSumData()
		{
			return sumData;
		}
		
		public void setSumData(boolean isSumData)
		{
			this.sumData = isSumData;
		}
		
		public String getValue()
		{
			return value;
		}
		
		public void setValue(String value)
		{
			this.value = value;
		}
	}
	
	protected static final String UnknownResult = "";
	
	private String locationID = "";
	private String locationName = "";
	protected boolean sumData = false;
	protected String itemName = "";
	protected List<WaterLevelData> itemValues = new ArrayList();
	
	public WaterLevelData makeWaterLevelData()
	{
		return new WaterLevelData();
	}
	
	public void setWaterLevelDataValue(WaterLevelData data, String value)
	{
		data.setValue(value);
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
	
	public boolean getSumData()
	{
		return sumData;
	}
	
	public void setSumData(boolean isSumData)
	{
		this.sumData = isSumData;
	}
	
	public String getItemName()
	{
		return itemName;
	}
	
	public void setItemName(String name)
	{
		this.itemName = name;
	}
	
	public List<WaterLevelData> getItemValues()
	{
		return itemValues;
	}
	
	public static String DoubleToString(double data)
	{
		// 소수점 첫째자리까지만 표시한다.
		return String.format("%.1f", data);
	}
}