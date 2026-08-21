package kr.co.unes.aqm.model;

import java.util.HashMap;
import java.util.List;

import org.apache.ibatis.annotations.Param;
import org.apache.ibatis.session.SqlSession;

import kr.co.unes.aqm.config.AQMSessionFactory;
import kr.co.unes.aqm.dao.NodeImageMapDAO;
import kr.co.unes.aqm.dto.site.NodeLinkImageMap;


public class NodeImageMapDataAccessManager {


	public void saveUploadImage(NodeLinkImageMap file) 
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			NodeImageMapDAO dao = sqlSession.getMapper(NodeImageMapDAO.class);			
			dao.saveUploadImage(file);		
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
	
	public void deleteNodeLinkMap(int nNodeLinkID, int nMapID)
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(false);	
			NodeImageMapDAO dao = sqlSession.getMapper(NodeImageMapDAO.class);	
			//dao.unlinkNodeImageMap(nNodeLinkID, nMapID);
			dao.setUnuseNodeLocation(nMapID);	
	
			sqlSession.commit();
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
	
	public void setImageMapLink(int nNodeLinkID, int nMapID) {

		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(false);	
			NodeImageMapDAO dao = sqlSession.getMapper(NodeImageMapDAO.class);
			//dao.setNodeImageMap(nNodeLinkID, nMapID);
			dao.setUseNodeLocation(nMapID);						
			sqlSession.commit();
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
	
	public NodeLinkImageMap getImageMap(String uuid) {
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			NodeImageMapDAO dao = sqlSession.getMapper(NodeImageMapDAO.class);			
			NodeLinkImageMap file = dao.getImageMap(uuid);
			return file;
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
	
	public NodeLinkImageMap getImageMap(int nID) {
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			NodeImageMapDAO dao = sqlSession.getMapper(NodeImageMapDAO.class);			
			NodeLinkImageMap file = dao.getImageMapFormID(nID);
			return file;
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

	public void deleteUnlinkedMaps() {
		// TODO Auto-generated method stub
		
	}
		
	public void deleteImageMap(int mapID) 
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(false);	
			NodeImageMapDAO dao = sqlSession.getMapper(NodeImageMapDAO.class);	
			dao.deleteImageMap(mapID);			
			sqlSession.commit();			
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
