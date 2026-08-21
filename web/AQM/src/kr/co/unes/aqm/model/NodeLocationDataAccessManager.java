package kr.co.unes.aqm.model;

import java.util.List;

import org.apache.ibatis.session.SqlSession;

import kr.co.unes.aqm.config.AQMSessionFactory;

import kr.co.unes.aqm.dao.NodeLocationDAO;
import kr.co.unes.aqm.dto.site.NodeLocation;


public class NodeLocationDataAccessManager
{
	
	public List<NodeLocation> getNodeLocation(int nSiteID)
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);			
			NodeLocationDAO dao = sqlSession.getMapper(NodeLocationDAO.class);			
			return dao.getNodeLocation(nSiteID);	
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
	
	public void addNodeLocation(NodeLocation loc)
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(false);			
			NodeLocationDAO dao = sqlSession.getMapper(NodeLocationDAO.class);			
			dao.addNodeLocation(loc);	
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
	
	public void updateNodeLocation(NodeLocation loc)
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(false);			
			NodeLocationDAO dao = sqlSession.getMapper(NodeLocationDAO.class);			
			dao.updateNodeLocation(loc);	
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
	
	public void deleteNodeLocation(int nNodeID)
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(false);			
			NodeLocationDAO dao = sqlSession.getMapper(NodeLocationDAO.class);			
			dao.deleteNodeLocation(nNodeID);	
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

	public NodeLocation getNodeLocationByNodeID(int linkID) {
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);			
			NodeLocationDAO dao = sqlSession.getMapper(NodeLocationDAO.class);			
			return dao.getNodeLocationByLinkID(linkID);

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
	
	public NodeLocation getNodeLocationByNetNodeID(int netNodeID) {
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);			
			NodeLocationDAO dao = sqlSession.getMapper(NodeLocationDAO.class);			
			return dao.getNodeLocationByNodeID(netNodeID);

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
