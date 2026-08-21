package g1Weather.service.data;

import g1Weather.service.PageVO;

public class CityTown extends PageVO {
	private String locationID = "";
	private String locationName = "";
	
	public String getLocationID()
	{
		return locationID;
	}
	
	public void setLocationID(String locationID)
	{
		this.locationID = locationID;
	}
	
	public String getLocationName()
	{
		return locationName;
	}
	
	public void setLocationName(String name)
	{
		this.locationName = name;
	}
}
