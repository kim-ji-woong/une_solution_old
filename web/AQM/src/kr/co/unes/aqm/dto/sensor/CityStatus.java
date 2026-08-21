package kr.co.unes.aqm.dto.sensor;

import com.google.gson.annotations.Expose;

public class CityStatus {


	private int ID;
	@Expose
	private String cityName;
	@Expose
	private double locX;
	@Expose
	private double locY;
	private int Status;
	public float SensorValue1;
	
	private float getValue1() {
		return SensorValue1;
	}
	private void setSensorValue1(float v) {
		this.SensorValue1 = v;
	}
	
	private float getSensorValue1() {
		return SensorValue2;
	}
	private void setSensorValue2(float v) {
		this.SensorValue1 = v;
	}
	private float getSensorValue3() {
		return SensorValue3;
	}
	private void setSensorValue3(float v) {
		this.SensorValue3 = v;
	}
	private float getSensorValue4() {
		return SensorValue4;
	}
	private void setSensorValue4(float v) {
		this.SensorValue4 = v;
	}
	private float getSensorValue5() {
		return SensorValue5;
	}
	private void setSensorValue5(float v) {
		this.SensorValue5 = v;
	}
	private float getSensorValue6() {
		return SensorValue6;
	}
	private void setSensorValue6(float v) {
		this.SensorValue6 = v;
	}
	
	public float SensorValue2;
	public float SensorValue3;
	public float SensorValue4;
	public float SensorValue5;
	public float SensorValue6;
	
	
	public int getID() {
		return ID;
	}
	public void setID(int iD) {
		ID = iD;
	}
	public String getCityName() {
		return cityName;
	}
	public void setCityName(String cityName) {
		this.cityName = cityName;
	}
	public double getLocX() {
		return locX;
	}
	public void setLocX(double locX) {
		this.locX = locX;
	}
	public double getLocY() {
		return locY;
	}
	public void setLocY(double locY) {
		this.locY = locY;
	}
	public int getStatus() {
		return Status;
	}
	public void setStatus(int status) {
		Status = status;
	}
	
	public double getValue(int nType) {
		if( nType == 0)
			return SensorValue1;
		else if(nType == 1)
			return SensorValue2;
		else if(nType == 2)
			return SensorValue3;
		else if(nType == 3)
			return SensorValue4;
		else if(nType == 4)
			return SensorValue5;
		else if(nType == 5)
			return SensorValue6;
		
		return getValue1();
	}
	
	
}
