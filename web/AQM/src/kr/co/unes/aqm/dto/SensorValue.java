package kr.co.unes.aqm.dto;

import java.sql.Timestamp;
import java.text.SimpleDateFormat;

import com.google.gson.annotations.Expose;

public class SensorValue 
{	
	@Expose
	protected float SensorValue;
	@Expose
	protected float ExtraValue;
	@Expose
	protected Timestamp TimeStamp;
	
	
	protected String timeStampString;
	protected int nodeID;
	protected int sensorCode;
	
	private int qualityGrade = -1;
	private float percentValue = 0.0f;
	
	
	public float getSensorValue() {
		return SensorValue;
	}

	public void setSensorValue(float sensorValue) {
		SensorValue = sensorValue;
	}

	public float getExtraValue() {
		return ExtraValue;
	}

	public void setExtraValue(float extraValue) {
		ExtraValue = extraValue;
	}

	public Timestamp getTimeStamp() {
		return TimeStamp;
	}

	public void setTimeStamp(Timestamp timeStamp) {
		try
		{
			SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
			timeStampString = sdf.format(timeStamp); 
		}
		catch(Exception e)
		{			
		}
		
		TimeStamp = timeStamp;
	}

	public String getTimeStampString() {
		return timeStampString;
	}

	public int getNodeID() {
		return nodeID;
	}

	public void setNodeID(int nodeID) {
		this.nodeID = nodeID;
	}

	public int getSensorCode() {
		return sensorCode;
	}

	public void setSensorCode(int sensorCode) {
		this.sensorCode = sensorCode;
	}

	public int getQualityGrade() {
		return qualityGrade;
	}

	public void setQualityGrade(int qualityGrade) {
		this.qualityGrade = qualityGrade;
	}

	public float getPercentValue() {
		return percentValue;
	}

	public void setPercentValue(float percentValue) {
		this.percentValue = percentValue;
	}
}
