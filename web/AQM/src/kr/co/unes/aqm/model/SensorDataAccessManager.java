package kr.co.unes.aqm.model;

import java.sql.Timestamp;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.apache.ibatis.annotations.Param;
import org.apache.ibatis.session.SqlSession;

import kr.co.unes.aqm.config.AQMSessionFactory;
import kr.co.unes.aqm.dao.QualityEvaluationDAO;
import kr.co.unes.aqm.dao.SensorDataDAO;
import kr.co.unes.aqm.dto.QualityEvaluation;
import kr.co.unes.aqm.dto.SensorValue;
import kr.co.unes.aqm.dto.SensorValueEx;
import kr.co.unes.aqm.dto.sensor.SensorInfo;

public class SensorDataAccessManager
{
	public SensorInfo getSensorInfo(int nSensorID)
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			SensorDataDAO dao = sqlSession.getMapper(SensorDataDAO.class);		
			return dao.getSensorInfo(nSensorID);
		}
		catch(Exception e)
		{
			e.printStackTrace();
			throw e;
		}
		finally
		{
			if(sqlSession != null)
				sqlSession.close();
		}
	}
	
	public List<SensorInfo> getAllSensorInfo(int nNodeID)
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			SensorDataDAO dao = sqlSession.getMapper(SensorDataDAO.class);		
			return dao.getAllSensorInfo(nNodeID);
		}
		catch(Exception e)
		{
			e.printStackTrace();
			throw e;
		}
		finally
		{
			if(sqlSession != null)
				sqlSession.close();
		}
	}	
	
	public ArrayList<SensorValueEx> getAllSensorValue() throws Exception  
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			SensorDataDAO dao = sqlSession.getMapper(SensorDataDAO.class);			
			
			ArrayList<SensorValueEx> arResult = new ArrayList<SensorValueEx>();
			List<Integer> nodeList = dao.getAllNode();
			for(Integer nodeID : nodeList)
			{
				String szTableName = dao.getLastTableNameByNode(nodeID.intValue());
				arResult.addAll(dao.getAllSensorValue(szTableName));
			}			
			return arResult;
		}
		catch(Exception e)
		{
			e.printStackTrace();
			throw e;
		}
		finally
		{
			if(sqlSession != null)
				sqlSession.close();
		}
	}

	public ArrayList<SensorValue> getSensorValues(int nSensorID, int nMaxCount, String szFormDate, String szToDate) throws Exception  
	{	
	    
		SqlSession sqlSession = null;
		try
		{
			SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");	
			Date dtFrom  = new Date();
			if( szFormDate == null)
			{
				szFormDate = "1970-01-01 00:00:00";				
				dtFrom = sdf.parse(szFormDate);			
			}
			else
			{						
				dtFrom = sdf.parse(szFormDate);	
			}	
			
		    Timestamp tsFrom = new Timestamp(dtFrom.getTime());
		    
		    Date dtTo = new Date();
		    if( szToDate != null)			
			{						
				dtTo = sdf.parse(szToDate);	
			}	
		    
		    Timestamp tsTo = new Timestamp(dtTo.getTime());	
		    
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			SensorDataDAO dao = sqlSession.getMapper(SensorDataDAO.class);
			String szTableName = dao.getDataTableNameBySensor(nSensorID);
			return dao.getSensorValuesBySensor(szTableName, nSensorID, nMaxCount, tsFrom , tsTo );
		}
		catch(Exception e)
		{
			e.printStackTrace();
			throw e;
		}
		finally
		{
			if(sqlSession != null)
				sqlSession.close();
		}	
	}
	
	public ArrayList<SensorValue> getSensorValues(int nNodeID, int nSensorCode, int nMaxCount, String szFormDate, String szToDate) throws Exception  
	{
		SqlSession sqlSession = null;
		try
		{
			SimpleDateFormat sdf = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");			
			Date dtFrom  = new Date();
			if( szFormDate == null)
			{
				szFormDate = "1970-01-01 00:00:00";				
				dtFrom = sdf.parse(szFormDate);			
			}
			else
			{					
				dtFrom = sdf.parse(szFormDate);		
				
			}	
			
		    Timestamp tsFrom = new Timestamp(dtFrom.getTime());
		    
		    Date dtTo = new Date();
		    if( szToDate != null)			
			{						
				dtTo = sdf.parse(szToDate);	
			} 
		    Timestamp tsTo = new Timestamp(dtTo.getTime());	
		    
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			SensorDataDAO dao = sqlSession.getMapper(SensorDataDAO.class);
			String szTableName = dao.getDataTableNameByNode(nNodeID);
			return dao.getSensorValuesByNode(szTableName, nNodeID, nSensorCode, nMaxCount, tsFrom , tsTo );
		}
		catch(Exception e)
		{
			e.printStackTrace();
			throw e;
		}
		finally
		{
			if(sqlSession != null)
				sqlSession.close();
		}	
	}	
	
	
	public QualityEvaluation getQualityEvalTable(int nSensorCode)
	{
		SqlSession sqlSession = null;
		try
		{			
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			QualityEvaluationDAO dao = sqlSession.getMapper(QualityEvaluationDAO.class);			
			return dao.getQualityEvalution(nSensorCode);
		}
		catch(Exception e)
		{
			e.printStackTrace();
			throw e;
		}
		finally
		{
			if(sqlSession != null)
				sqlSession.close();
		}	
	}
	
	public SensorValue getMaxSensorValue(int nNodeID, int nSensorCode, String szFormDate, String szToDate) throws Exception  
	{
		SqlSession sqlSession = null;
		try	
		{
			SimpleDateFormat dayTime = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");		

			Date dtFrom  = new Date();
			if( szFormDate == null)
			{
				long time = System.currentTimeMillis(); 
				dtFrom = new Date(time);
			}
			else
			{
				dtFrom = dayTime.parse(szFormDate);		
			}	
			
		    Timestamp tsFrom = new Timestamp(dtFrom.getTime());
		    
		    Date dtTo = new Date();
		    if(szToDate != null)
		    {
		    	dtTo = dayTime.parse(szToDate);		
		    } 
		    
		    Timestamp tsTo = new Timestamp(dtTo.getTime());	
			
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			SensorDataDAO dao = sqlSession.getMapper(SensorDataDAO.class);
			String szTableName = dao.getDataTableNameByNode(nNodeID);
			return dao.getMaxSensorValuesByNode(szTableName, nNodeID, nSensorCode, tsFrom , tsTo );
		}
		catch(Exception e)
		{
			e.printStackTrace();
			throw e;
		}
		finally
		{
			if(sqlSession != null)
				sqlSession.close();
		}	
	}
	
	public HashMap<Integer, HashMap<Integer, SensorValueEx>> getTimeSeriesSensorValue(int nNodeID, int [] nSensorCode, String szFormDate, String szToDate) throws Exception  
	{
		SqlSession sqlSession = null;
		try
		{
			SimpleDateFormat dayTime = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");		

			Date dtFrom  = new Date();
			if( szFormDate == null)
			{
				long time = System.currentTimeMillis(); 
				dtFrom = new Date(time);
			}
			else
			{
				dtFrom = dayTime.parse(szFormDate);		
			}	
			
		    Timestamp tsFrom = new Timestamp(dtFrom.getTime());
		    
		    Date dtTo = new Date();
		    if(szToDate != null)
		    {
		    	dtTo = dayTime.parse(szToDate);		
		    } 
		    Timestamp tsTo = new Timestamp(dtTo.getTime());	
		    
		    HashMap<Integer, HashMap<Integer, SensorValueEx>> resultMap = new HashMap<Integer, HashMap<Integer, SensorValueEx>>();
		    ArrayList<String> dtList = new ArrayList<String>();
			for( int i = 0 ; i < 24; i++)
			{
				Calendar cal = Calendar.getInstance();
				cal.setTime(dtTo);
				cal.add(Calendar.HOUR, -1 * i);
				Date oneTimeBefore= cal.getTime();
				cal.add(Calendar.HOUR, -1);
				Date twoTimeBefore= cal.getTime();
		
				dtList.add(dayTime.format(twoTimeBefore));
				dtList.add(dayTime.format(oneTimeBefore));
				HashMap<Integer, SensorValueEx> list = new HashMap<Integer, SensorValueEx>();				
				resultMap.put(new Integer(i+1),  list);
			}		   
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			SensorDataDAO dao = sqlSession.getMapper(SensorDataDAO.class);
			String szTableName = dao.getDataTableNameByNode(nNodeID);
			List<SensorValueEx> result = dao.getSensorTimeSeries(szTableName, nNodeID,nSensorCode, dtList, tsFrom , tsTo );

			for(SensorValueEx value : result)
			{
				Integer key = new Integer(value.getTimeIdx());
				if( resultMap.containsKey(key))
				{
					HashMap<Integer, SensorValueEx> list = resultMap.get(key);
					list.put(value.getSensorCode(), value);
				}
				else
				{
					HashMap<Integer, SensorValueEx> list = new HashMap<Integer, SensorValueEx>();
					list.put(value.getSensorCode(), value);
					resultMap.put(key,  list);					
				}
			}
			return resultMap;
		}
		catch(Exception e)
		{
			e.printStackTrace();
			throw e;
		}
		finally
		{
			if(sqlSession != null)
				sqlSession.close();
		}	
	}	
	
	public HashMap<Integer, SensorValueEx> getSensorValueForAverage(int nNodeID, int [] nSensorCode, String szFormDate, String szToDate) throws Exception  
	{
		SqlSession sqlSession = null;
		try
		{
			
			SimpleDateFormat dayTime = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");		

			Date dtFrom  = new Date();
			if( szFormDate == null)
			{
				long time = System.currentTimeMillis(); 
				dtFrom = new Date(time);
			}
			else
			{
				dtFrom = dayTime.parse(szFormDate);		
			}	
			
		    Timestamp tsFrom = new Timestamp(dtFrom.getTime());
		    
		    Date dtTo = new Date();
		    if(szToDate != null)
		    {
		    	dtTo = dayTime.parse(szToDate);		
		    } 
		    Timestamp tsTo = new Timestamp(dtTo.getTime());	
		    
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			SensorDataDAO dao = sqlSession.getMapper(SensorDataDAO.class);
			String szTableName = dao.getLastTableNameByNode(nNodeID);
			List<SensorValueEx> result = dao.getSensorValueForAverage(szTableName, nNodeID,nSensorCode, tsFrom, tsTo);

			HashMap<Integer, SensorValueEx> list = new HashMap<Integer, SensorValueEx>();
			for(SensorValueEx value : result)
			{							
				list.put(value.getSensorCode(), value);	
			}
			return list;
		}
		catch(Exception e)
		{
			e.printStackTrace();
			throw e;
		}
		finally
		{
			if(sqlSession != null)
				sqlSession.close();
		}	
	}	
		
	public SensorValue getSensorValue(int nNodeID, int nSensorCode)  throws Exception 
	{
		SqlSession sqlSession = null;
		try
		{ 
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			SensorDataDAO dao = sqlSession.getMapper(SensorDataDAO.class);
			String szTableName = dao.getLastTableNameByNode(nNodeID);
			return dao.getLastSensorValueByNode(szTableName, nNodeID, nSensorCode);
		}
		catch(Exception e)
		{
			e.printStackTrace();
			throw e;
		}
		finally
		{
			if(sqlSession != null)
				sqlSession.close();
		}	
	}

	public SensorValue getSensorValue(int nSensorID) throws Exception 
	{
		SqlSession sqlSession = null;
		try
		{ 
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			SensorDataDAO dao = sqlSession.getMapper(SensorDataDAO.class);
			String szTableName = dao.getLastTableNameBySensor(nSensorID);
			return dao.getLastSensorValueBySensor(szTableName, nSensorID);
		}
		catch(Exception e)
		{
			e.printStackTrace();
			throw e;
		}
		finally
		{
			if(sqlSession != null)
				sqlSession.close();
		}	
	}

	public boolean addSensorValue(int nNodeID, int nSensorCode, float fValue, float fExtraValue) throws Exception 
	{
		SqlSession sqlSession = null;
		try
		{ 
			Date now = new Date();
		    Timestamp timestamp = new Timestamp(now.getTime());
			SensorValue value = new SensorValue();
			value.setNodeID(nNodeID);
			value.setSensorCode(nSensorCode);
			value.setSensorValue(fValue);
			value.setExtraValue(fExtraValue);
			value.setTimeStamp(timestamp);
			
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			SensorDataDAO dao = sqlSession.getMapper(SensorDataDAO.class);
			String szDataTableName = dao.getDataTableNameByNode(nNodeID);			
			String szLastTableName = dao.getLastTableNameByNode(nNodeID);
						
			dao.addSensorValue(szDataTableName, nNodeID, nSensorCode, fValue, fExtraValue, value.getTimeStampString() );
			
			SensorValue exist = dao.getLastSensorValueByNode(szLastTableName, nNodeID, nSensorCode);
			if(exist == null)
				dao.addLastSensorValue(szLastTableName, nNodeID, nSensorCode, fValue, fExtraValue, value.getTimeStampString() );
			else
				dao.updateLastSensorValue(szLastTableName, nNodeID, nSensorCode, fValue, fExtraValue, value.getTimeStampString() );
			return true;
		}
		catch(Exception e)
		{
			e.printStackTrace();
			throw e;
		}
		finally
		{
			if(sqlSession != null)
				sqlSession.close();
		}	
	}

	public boolean addSensorValue(int nSensorID, float fValue, float fExtraValue) throws Exception 
	{
		
		SqlSession sqlSession = null;
		try
		{ 			
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			SensorDataDAO dao = sqlSession.getMapper(SensorDataDAO.class);
			SensorInfo info = dao.getSensorInfo(nSensorID);
			if( info != null)
			{
				Date now = new Date();
			    Timestamp timestamp = new Timestamp(now.getTime());
				SensorValue value = new SensorValue();
			
				value.setSensorValue(fValue);
				value.setExtraValue(fExtraValue);
				value.setTimeStamp(timestamp);
				int nNodeID = info.getNodeID();
				int nSensorCode = info.getSensorCode();
				value.setNodeID(nNodeID);
				value.setSensorCode(nSensorCode);
				
				String szDataTableName = dao.getDataTableNameBySensor(nSensorID);			
				String szLastTableName = dao.getLastTableNameBySensor(nSensorID);
							
				dao.addSensorValue(szDataTableName, nNodeID, nSensorCode, fValue, fExtraValue, value.getTimeStampString() );
				SensorValue exist = dao.getLastSensorValueByNode(szLastTableName, nNodeID, nSensorCode);
				if(exist == null)
					dao.addLastSensorValue(szLastTableName, nNodeID, nSensorCode, fValue, fExtraValue, value.getTimeStampString() );
				else
					dao.updateLastSensorValue(szLastTableName, nNodeID, nSensorCode, fValue, fExtraValue, value.getTimeStampString() );
				
				return true;
			}
			return false;
		}
		catch(Exception e)
		{
			e.printStackTrace();
			throw e;
		}
		finally
		{
			if(sqlSession != null)
				sqlSession.close();
		}
	}

	public void updateCityAverageData(String cityName, float[] averageValue) {
		SqlSession sqlSession = null;
		try
		{ 			
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			SensorDataDAO dao = sqlSession.getMapper(SensorDataDAO.class);
			dao.updateCityAverageData(cityName, averageValue);		
		}
		catch(Exception e)
		{
			e.printStackTrace();
			throw e;
		}
		finally
		{
			if(sqlSession != null)
				sqlSession.close();
		}	
		
	}	
}
