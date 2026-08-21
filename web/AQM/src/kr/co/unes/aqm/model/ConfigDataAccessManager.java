package kr.co.unes.aqm.model;

import java.util.ArrayList;

import org.apache.ibatis.session.SqlSession;

import kr.co.unes.aqm.config.AQMSessionFactory;
import kr.co.unes.aqm.dao.ConfigDAO;
import kr.co.unes.aqm.dto.SensorGroup;
import kr.co.unes.aqm.dto.sensor.SensorCode;


public class ConfigDataAccessManager {

	//private DatabaseManager mDBManager = new DatabaseManager();
	//private ConfigDataAccess accessConfig = new ConfigDataAccess();

	
	public ArrayList<SensorGroup> getSensorGroups()
	{		
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			ConfigDAO dao = sqlSession.getMapper(ConfigDAO.class);			
			return dao.getSensorGroups();	
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
		
//		Connection con = mDBManager.getConnection();		
//		ArrayList<SensorGroup> value = accessConfig.getSensorGroups(con);
//		con.close();
//		return value;
	}
	
	public ArrayList<SensorCode> getSensorCodes()
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			ConfigDAO dao = sqlSession.getMapper(ConfigDAO.class);			
			return dao.getSensorCodes();	
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
		
//		Connection con = mDBManager.getConnection();		
//		ArrayList<SensorCode> value = accessConfig.getSensorCodes(con);
//		con.close();
//		return value;
	}
	

	public boolean setIntValue(int nSensorCode, String szFieldName, int value)
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			ConfigDAO dao = sqlSession.getMapper(ConfigDAO.class);			
			return dao.setConfigIntField(nSensorCode, szFieldName, value);	
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
		
//		Connection con = mDBManager.getConnection();	
//		boolean bResult = accessConfig.setConfigIntField(con, nSensorCode, szFieldName, value);	
//		con.close();
//		return bResult;
	}

	public int getIntValue(String szFieldName, int nSensorCode) throws Exception
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			ConfigDAO dao = sqlSession.getMapper(ConfigDAO.class);			
			return dao.getConfigIntField(nSensorCode, szFieldName);	
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
		
//		Connection con = mDBManager.getConnection();
//		int dValue = accessConfig.getConfigIntField(con, nSensorCode, szFieldName);
//		con.close();
//		return dValue;
	}	
	
	public boolean setDoubleValue(int nSensorCode, String szFileName, double value) throws Exception 
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			ConfigDAO dao = sqlSession.getMapper(ConfigDAO.class);			
			return dao.setConfigDoubleValue(nSensorCode,szFileName, value);	
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
		
//		Connection con = mDBManager.getConnection();	
//		boolean bResult = accessConfig.setConfigDoubleValue(con, szFileName, nSensorCode, value);
//		con.close();
//		return bResult;
	}

	public double getDoubleValue(String szFieldName, int nSensorID) throws Exception
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			ConfigDAO dao = sqlSession.getMapper(ConfigDAO.class);			
			return dao.getConfigDoubleValue(nSensorID, szFieldName);	
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
		
//		Connection con = mDBManager.getConnection();
//		double dValue = accessConfig.getConfigDoubleValue(con,szFieldName, nSensorID);
//		con.close();
//		return dValue;
	}
	
	public int addSensorCode(SensorCode code)
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			ConfigDAO dao = sqlSession.getMapper(ConfigDAO.class);			
			return dao.addSensorCode(code);	
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

	public int addSensorCode(String szSensorName, int nSensorCode, int nGroupID,
			float fLimitNotice, float fLimitAttention, float fLimitWarning,
			float fLimitValueLaw, String sensorUnit, int nLimitType, float fLimitNoticeBegin,
			float fLimitNoticeEnd, float fLimitAttentionBegin,
			float fLimitAttentionEnd, float fLimitWarningBegin,
			float fLimitWarningEnd, float fLimitValueLawBegin,
			float fLimitValueLawEnd, String szRemark)
	{
		
		
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			ConfigDAO dao = sqlSession.getMapper(ConfigDAO.class);			
			return dao.addSensorCodeData(szSensorName, nSensorCode,nGroupID,
					fLimitNotice, fLimitAttention, fLimitWarning,fLimitValueLaw, sensorUnit, nLimitType, fLimitNoticeBegin,fLimitNoticeEnd, 
					fLimitAttentionBegin,fLimitAttentionEnd, fLimitWarningBegin,fLimitWarningEnd, fLimitValueLawBegin,	fLimitValueLawEnd, szRemark);	
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
		
//		Connection con = mDBManager.getConnection();
//		int nID = accessConfig.addSensorCode(con,szSensorName, nSensorCode,nGroupID,
//				fLimitNotice, fLimitAttention, fLimitWarning,fLimitValueLaw, sensorUnit, nLimitType, fLimitNoticeBegin,fLimitNoticeEnd, 
//				fLimitAttentionBegin,fLimitAttentionEnd, fLimitWarningBegin,fLimitWarningEnd, fLimitValueLawBegin,	fLimitValueLawEnd, szRemark);
//		con.close();
//	
//		
//		return nID;
	}	

}
