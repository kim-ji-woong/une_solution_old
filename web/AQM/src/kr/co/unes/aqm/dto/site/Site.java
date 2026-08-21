package kr.co.unes.aqm.dto.site;

import java.util.ArrayList;
import java.util.List;

public class Site {
	
	private int ID;	
	private String locationName = "";	
	private String mainAddress = "";	
	private String detailAddress = "";	
	private String phone = "";
	private float locationX;	
	private float locationY;
	private String description;
	private int area;
	private int map;
	
	public int getID() {
		return ID;
	}

	public void setID(int m_nID) {
		this.ID = m_nID;
	}

	public String getName() {
		return locationName;
	}

	public void setName(String m_szName) {
		this.locationName = m_szName;
	}

	public String getAddress() {
		return mainAddress;
	}

	public void setAddress(String m_szAddress) {
		this.mainAddress = m_szAddress;
	}

	public String getDetailAddress() {
		return detailAddress;
	}

	public void setDetailAddress(String m_szDetailAddress) {
		this.detailAddress = m_szDetailAddress;
	}

	public String getPhone() {
		return phone;
	}

	public void setPhone(String m_szPhone) {
		this.phone = m_szPhone;
	}

	public float getLocationX() {
		return locationX;
	}

	public void setLocationX(float m_fLocationX) {
		this.locationX = m_fLocationX;
	}

	public float getLocationY() {
		return locationY;
	}

	public void setLocationY(float m_fLocationY) {
		this.locationY = m_fLocationY;
	}
	
	

	public String getDescription() {
		return description;
	}

	public void setDescription(String description) {
		this.description = description;
	}
	

	public List<String> toFTL()
	{		
		List<String> result = new ArrayList<String>();
		result.add(""+ ID);
		result.add(""+locationName);
		result.add(""+phone);
		result.add(""+mainAddress + detailAddress);	
		result.add(""+locationX + "," + locationY);
	
		return result;
	}

	public int getArea() {
		return area;
	}

	public void setArea(int area) {
		this.area = area;
	}

	public int getMap() {
		return map;
	}

	public void setMap(int map) {
		this.map = map;
	}
}
