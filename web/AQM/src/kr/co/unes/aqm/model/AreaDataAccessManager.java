package kr.co.unes.aqm.model;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

import org.apache.ibatis.session.SqlSession;

import kr.co.unes.aqm.config.AQMSessionFactory;
import kr.co.unes.aqm.dto.area.AreaDepth1;
import kr.co.unes.aqm.dto.area.AreaDepth2;
import kr.co.unes.aqm.dto.area.AreaDepth3;
import kr.co.unes.aqm.dto.area.AreaDepth4;
import kr.co.unes.aqm.dto.area.AreaName;

public class AreaDataAccessManager {
	
	public ArrayList<AreaDepth1> getAreaDepth1List() throws Exception
	{	
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);			
			List<AreaDepth1> depth1s = sqlSession.selectList("kr.co.unes.aqm.dao.AreaDAO.getAreaDepth1");
			
			return (ArrayList<AreaDepth1>) depth1s;
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
	
	public ArrayList<AreaDepth2> getAreaDepth2List(String depth1) throws Exception
	{		
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			HashMap<String, String> map = new HashMap<String, String>();
			map.put("area1", depth1);
			List<AreaDepth2> depth2s = sqlSession.selectList("kr.co.unes.aqm.dao.AreaDAO.getAreaDepth2", depth1);	
			return (ArrayList<AreaDepth2>) depth2s;
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
	
	public ArrayList<AreaDepth3> getAreaDepth3List(String depth1, String depth2) throws Exception
	{		
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			HashMap<String, String> map = new HashMap<String, String>();
			map.put("depth1", depth1);
			map.put("depth2", depth2);
			List<AreaDepth3> depth3s = sqlSession.selectList("kr.co.unes.aqm.dao.AreaDAO.getAreaDepth3", map);	
			return (ArrayList<AreaDepth3>) depth3s;
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
	
	public ArrayList<AreaDepth4> getAreaDepth4List(String szDepth1, String szDepth2, String szDepth3) throws Exception
	{	
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			HashMap<String, String> map = new HashMap<String, String>();
			map.put("depth1", szDepth1);
			map.put("depth2", szDepth2);
			map.put("depth3", szDepth3);
			List<AreaDepth4> depth3s = sqlSession.selectList("kr.co.unes.aqm.dao.AreaDAO.getAreaDepth4", map);	
			return (ArrayList<AreaDepth4>) depth3s;
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
	
	public int getAreaID(String szDepth1, String szDepth2, String szDepth3, String szDepth4)  throws Exception
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			HashMap<String, String> map = new HashMap<String, String>();
			map.put("depth1", szDepth1);
			map.put("depth2", szDepth2);
			map.put("depth3", szDepth3);
			
			if(szDepth4 == null)
				szDepth4 = "";
			map.put("depth4", szDepth4);		
			int nID = sqlSession.selectOne("kr.co.unes.aqm.dao.AreaDAO.getAreaID", map);	
			return nID;
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
	
	public AreaName getAreaName(int areaID) throws Exception
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			AreaName name = sqlSession.selectOne("kr.co.unes.aqm.dao.AreaDAO.getAreaName", areaID);				
			return name;
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
