package kr.co.unes.aqm.dao;

import java.util.ArrayList;

import org.apache.ibatis.annotations.Param;

import kr.co.unes.aqm.dto.SensorGroup;
import kr.co.unes.aqm.dto.sensor.SensorCode;

public interface ConfigDAO {

	public ArrayList<SensorGroup> getSensorGroups();	
	public ArrayList<SensorCode> getSensorCodes();
	
	public int getConfigIntField(
			@Param("sensorCode") int nSensorCode, 
			@Param("fieldName") String szFieldName);
	
	public boolean setConfigIntField(
			@Param("sensorCode") int nSensorCode, 
			@Param("fieldName") String szFileName,
			@Param("fieldValue") int nValue);
	
	public boolean setConfigDoubleValue(
			@Param("sensorCode") int nSensorCode, 
			@Param("fieldName") String szFileName,
			@Param("fieldValue") double dValue);	
	
	public double getConfigDoubleValue(
			@Param("sensorCode") int nSensorCode, 
			@Param("fieldName") String  szFieldName);	
		
	public int addSensorCode(SensorCode code);
	public int addSensorCodeData(
			@Param("name") String szSensorName, 
			@Param("sensorCode") int nSensorCode, 
			@Param("groupID") int nGroupID,
			@Param("limitNotice") float fLimitNotice,
			@Param("limitAttention") float fLimitAttention,
			@Param("limitWarning") float fLimitWarning, 
			@Param("limitValueLaw") float fLimitValueLaw, 
			@Param("sensorUnit") String sensorUnit, 
			@Param("limitType") int nLimitType,
			@Param("limitNoticeBegin") float fLimitNoticeBegin, 
			@Param("limitNoticeEnd") float fLimitNoticeEnd,
			@Param("limitAttentionBegin") float fLimitAttentionBegin,
			@Param("limitAttentionEnd") float fLimitAttentionEnd,
			@Param("limitWarningBegin") float fLimitWarningBegin, 
			@Param("limitWarningEnd") float fLimitWarningEnd,
			@Param("limitValueLawBegin") float fLimitValueLawBegin,
			@Param("limitValueLawEnd") float fLimitValueLawEnd, 
			@Param("remark") String szRemark);

}
