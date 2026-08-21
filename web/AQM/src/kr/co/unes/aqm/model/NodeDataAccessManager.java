package kr.co.unes.aqm.model;

import java.util.Date;
import java.util.HashMap;

import org.apache.ibatis.session.SqlSession;

import java.sql.Timestamp;

import kr.co.unes.aqm.config.AQMSessionFactory;
import kr.co.unes.aqm.dto.area.AreaName;
import kr.co.unes.aqm.dto.sensor.SensorCode;

public class NodeDataAccessManager
{
	
	public int getNodeAreaID(int nodeID) throws Exception 
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);			
			int nAreaID = sqlSession.selectOne("kr.co.unes.aqm.dao.NodeDAO.getNodeAreaID", nodeID);			
			return nAreaID;
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
//		int nID = accessNode.getAreaID(con, nNodeID);
//		con.close();
//		return nID;
	}
	
	public int getNodeEanbled(int nodeID) throws Exception 
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);			
			int nEabled = sqlSession.selectOne("kr.co.unes.aqm.dao.NodeDAO.getNodeEnabled", nodeID);			
			return nEabled;
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
//		int nID = accessNode.getNodeEnabled(con, nNodeID);
//		con.close();
//		return nID;
	}
	
	public boolean setNodeEanbled(int nodeID, boolean bUse) throws Exception
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);		
			HashMap<String, String> map = new HashMap<String, String>();
			map.put("nodeID", ""+nodeID);
			map.put("use", ""+bUse);
			sqlSession.update("kr.co.unes.aqm.dao.NodeDAO.setNodeEanbled", map);			
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
//		Connection con = mDBManager.getConnection();		
//		boolean bResult = accessNode.setNodeEnabled(con, nNodeID, bEnabled);
//		con.close();
//		return bResult;
	}
	
	public String getNodeName(int nodeID) throws Exception
	{
		
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);		
			String szName = sqlSession.selectOne("kr.co.unes.aqm.dao.NodeDAO.getNodeName", nodeID);			
			return szName;
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
//		String szName = accessNode.getNodeName(con, nNodeID);
//		con.close();
//		return szName;
	}

	private String mDataTable = "DataNetRaw";
	private String mDataLastTable = "DataNetLast";
	public boolean addNetNode(int nNodeID, String strNodeName, float fNodePosX, float fNodePosY, int areaID, String szMaterialCode) throws Exception {
		
		AreaName name = null;
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			name = sqlSession.selectOne("kr.co.unes.aqm.dao.AreaDAO.getAreaName", areaID);				

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
		if( name == null)
			return false;
		
		String szArea = name.getDetph1() + " " + name.getDetph2() + " " + name.getDetph3();
		Date now = new Date();
	    Timestamp timestamp = new Timestamp(now.getTime());
		
	    boolean bResult = true;	    
	    try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(false);	
			HashMap<String,String> map = new HashMap<String,String>();			
			map.put("nodeID",  ""+ nNodeID);
			map.put("nodeName", strNodeName);
			map.put("nodePosX", "" + fNodePosX);
			map.put("nodePosY", "" + fNodePosY);
			map.put("timeStamp", timestamp.toString());
			map.put("area", szArea);
			map.put("areaID", ""+ areaID);
			
			map.put("dataNetRaw", mDataTable);
			map.put("dataNetLast", mDataLastTable);

			int nInsert = sqlSession.insert("kr.co.unes.aqm.dao.NodeDAO.addNetNode", map);	
			if(nInsert > 0)
			{
				String [] codes = szMaterialCode.split("[,]");
				if( codes != null && codes.length > 0)
				{
					for(int i = 0 ; i < codes.length ; i++)
					{
						String szCode = codes[i];
						SensorCode code = sqlSession.selectOne("kr.co.unes.aqm.dao.NodeDAO.getSensorCode", szCode);	
						
						HashMap<String, Object> map2 = new HashMap<String,Object>();
						map2.put("nodeID", "" + nNodeID);
						map2.put("materialCode", szCode );
						map2.put("SensorCode", code);
						
						if(sqlSession.insert("kr.co.unes.aqm.dao.NodeDAO.addSensor", map2) < 0)						
						{
							bResult = false;
						}
					}
				}	
			}
			else
			{
				bResult = false;				
			}			
			
			if(bResult == true)
				sqlSession.commit();
			else
				sqlSession.rollback();
			
			return bResult;
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
//		AreaName name = accessArea.getAreaName(con, nAreaID);
//		String szArea = name.getDetph1() + " " + name.getDetph2() + " " + name.getDetph3();
//		
//		Date now = new Date();
//	    Timestamp timestamp = new Timestamp(now.getTime());
//		boolean bResult = accessNode.addNetNode(con, nNodeID, strNodeName, fNodePosX, fNodePosY, timestamp, szArea, nAreaID);
//		if(bResult == true)
//		{
//			String [] codes = szMaterialCode.split("[,]");
//			if( codes != null && codes.length > 0)
//			{
//				for(int i = 0 ; i < codes.length ; i++)
//				{
//					String szCode = codes[i];
//					if(accessNode.addSensor(con, nNodeID, szCode) < 0)
//					{
//						bResult = false;
//					}
//				}
//			}			
//		}	
//		if( bResult == true)
//			con.commit();
//		else
//			con.rollback();		
//		con.close();		
//		return bResult;
	    
	}

	public boolean removeNode(int nodeID) throws Exception 
	{		
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);		
			HashMap<String, String> map = new HashMap<String,String>();
			map.put("nodeID", ""+ nodeID);
			map.put("dataTable", mDataTable);
			map.put("lastTable", mDataLastTable);
			int nResult = sqlSession.delete("kr.co.unes.aqm.dao.NodeDAO.removeNode", map);
			if( nResult > 0)
				return true;
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
		
//		Connection con = mDBManager.getConnection();		
//		boolean bResult = accessNode.removeNode(con, nNodeID);
//		if( bResult == true)
//			con.commit();
//		con.close();
//		return bResult;

	}
}
