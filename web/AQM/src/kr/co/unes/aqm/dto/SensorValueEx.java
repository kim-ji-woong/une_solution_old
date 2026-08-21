package kr.co.unes.aqm.dto;

import com.google.gson.annotations.Expose;

public class SensorValueEx extends SensorValue
{
	@Expose
	protected int ID = -1;
	
	public int getID() {
		return ID;
	}

	public void setID(int iD) {
		ID = iD;
	}
	
	
	protected int TimeIdx;
	public int getTimeIdx() {
		return TimeIdx;
	}

	public void setTimeIdx(int timeIdx) {
		TimeIdx = timeIdx;
	}

	
	
}
