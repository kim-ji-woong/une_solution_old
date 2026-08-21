package kr.co.unes.aqm.dto;

import com.google.gson.annotations.Expose;

public class SensorGroup {

	@Expose
	private int id = -1;
	
	@Expose
	private String GroupName = "";

	public int getId()
	{
		return id;
	}

	public void setId(int id)
	{
		this.id = id;
	}

	public String getGroupName() 
	{
		return GroupName;
	}

	public void setGroupName(String groupName) 
	{
		GroupName = groupName;
	}
}
