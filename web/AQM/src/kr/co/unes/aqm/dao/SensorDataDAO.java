package kr.co.unes.aqm.dao;

import java.lang.String;
import java.sql.Timestamp;
import java.util.ArrayList;
import java.util.List;

import org.apache.ibatis.annotations.Param;

import kr.co.unes.aqm.dto.SensorValue;
import kr.co.unes.aqm.dto.SensorValueEx;
import kr.co.unes.aqm.dto.sensor.SensorInfo;

public interface SensorDataDAO 
{		
	public SensorInfo getSensorInfo(@Param("sensorID") int nSensorID);
	public List<SensorInfo> getAllSensorInfo(@Param("nodeID") int nNodeID);
	
	public List<Integer> getAllNode();
	
	public String getDataTableNameBySensor(@Param("sensorID") int nSensorID);
	public String getLastTableNameBySensor(@Param("sensorID") int nSensorID);

	public String getDataTableNameByNode(@Param("nodeID") int nNodeID);
	public String getLastTableNameByNode(@Param("nodeID") int nNodeID);
	
	public ArrayList<SensorValueEx> getAllSensorValue(@Param("tableName") String szTableName);

	public ArrayList<SensorValue> getSensorValuesBySensor(
			@Param("tableName") String szTableName,
			@Param("sensorID") int nSensorID, 
			@Param("maxCount") int nMaxCount,
			@Param("timeFrom") Timestamp szFormDate,
			@Param("timeTo") Timestamp szToDate);
	
	public ArrayList<SensorValue> getSensorValuesByNode(
			@Param("tableName") String szTableName,
			@Param("nodeID") int nNodeID, 
			@Param("sensorCode") int nSensorCode,
			@Param("maxCount") int nMaxCount,
			@Param("timeFrom") Timestamp tsFrom,
			@Param("timeTo") Timestamp tsTo);
	
	public SensorValue getMaxSensorValuesByNode(
			@Param("tableName") String szTableName,
			@Param("nodeID") int nNodeID, 
			@Param("sensorCode") int nSensorCode,
			@Param("timeFrom") Timestamp tsFrom,
			@Param("timeTo") Timestamp tsTo);	
	
	public List<SensorValueEx> getSensorTimeSeries(
			@Param("tableName") String szTableName,
			@Param("nodeID") int nNodeID, 
			@Param("CodeList") int [] nSensorCodes,
			@Param("TimeList") List<String> timeList,
			@Param("timeFrom") Timestamp tsFrom,
			@Param("timeTo") Timestamp tsTo);	


	public SensorValue getLastSensorValueBySensor(
			@Param("tableName") String szTableName,
			@Param("sensorID") int sensorID);
	
	public SensorValue getLastSensorValueByNode(
			@Param("tableName") String szTableName,
			@Param("nodeID") int nodeID, 
			@Param("sensorCode") int sensorCode);
	
	public void addSensorValue(
			@Param("tableName") String szTableName, 
			@Param("nodeID") int nNodeID,
			@Param("sensorCode") int nSensorCode,
			@Param("sensorValue") float sensorValue,
			@Param("extraValue") float extraValue,
			@Param("timeStampString") String timeStamp);
	
	public void addLastSensorValue(
			@Param("tableName") String szTableName, 
			@Param("nodeID") int nNodeID,
			@Param("sensorCode") int nSensorCode,
			@Param("sensorValue") float sensorValue,
			@Param("extraValue") float extraValue,
			@Param("timeStampString") String timeStamp);
	
	public void updateLastSensorValue(
			@Param("tableName") String szTableName, 
			@Param("nodeID") int nNodeID,
			@Param("sensorCode") int nSensorCode,
			@Param("sensorValue") float sensorValue,
			@Param("extraValue") float extraValue,
			@Param("timeStampString") String timeStamp);
	
	public List<SensorValueEx> getSensorValueForAverage(
			@Param("tableName") String szTableName,
			@Param("nodeID") int nNodeID, 
			@Param("CodeList") int [] nSensorCodes,
			@Param("timeFrom") Timestamp tsFrom,
			@Param("timeTo") Timestamp tsTo);
	
	public void updateCityAverageData(
			@Param("cityName") String cityName,
			@Param("valueList") float[] averageValue);	

	
	//public void addSensorValue(
	//		@Param("tableName") String szTableName, 
	//		SensorValue value);
	
	//public void addLastSensorValue(
	//		@Param("tableName") String szTableName, 
	//		SensorValue value);
	
}
