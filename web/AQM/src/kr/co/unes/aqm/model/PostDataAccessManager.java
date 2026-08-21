package kr.co.unes.aqm.model;

import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import org.apache.ibatis.session.SqlSession;

import kr.co.unes.aqm.config.AQMSessionFactory;
import kr.co.unes.aqm.dao.AttachFileDAO;
import kr.co.unes.aqm.dao.PostDAO;
import kr.co.unes.aqm.dto.post.FileInPost;
import kr.co.unes.aqm.dto.post.PostItem;

public class PostDataAccessManager {


	public PostItem getPost(int id)
	{
		SqlSession sqlSession = null;
		try
		{
			updateReadCount(id);			
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);			
			PostItem item = sqlSession.selectOne("kr.co.unes.aqm.dao.PostDAO.getPost", id);		
			return item;
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
	
	public int updateReadCount(int id)
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);			
			int count = sqlSession.update("kr.co.unes.aqm.dao.PostDAO.updateReadCount", id);
			return count;
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
	
	public List<PostItem> getAllPost()
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);			
			List<PostItem> items = sqlSession.selectList("kr.co.unes.aqm.dao.PostDAO.getAllPost");			
			return (ArrayList<PostItem>)items;	
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
	
	public int writePost(PostItem post)
	{

		SqlSession sqlSession = null;
		try
		{
			//DateFormat dateFormat = new SimpleDateFormat("yyyy/MM/dd HH:mm:ss");
			Date date = new Date();
			post.setTimeStamp(date);
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);			
			int nItem = sqlSession.insert("kr.co.unes.aqm.dao.PostDAO.writePost", post);			
			return nItem;
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
	
	public boolean modifyPost(PostItem post)
	{
		SqlSession sqlSession = null;
		try
		{
			//DateFormat dateFormat = new SimpleDateFormat("yyyy/MM/dd HH:mm:ss");
			Date date = new Date();
			post.setTimeStamp(date);
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);			
			int nItem = sqlSession.update("kr.co.unes.aqm.dao.PostDAO.modifyPost", post);			
			return (nItem > 0);
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
	
	public boolean deletePost(int id)
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);			
			int nItem = sqlSession.delete("kr.co.unes.aqm.dao.PostDAO.deletePost", id);			
			return (nItem > 0);
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

	public ArrayList<PostItem> searchPost(int type, String text)
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			PostDAO dao = sqlSession.getMapper(PostDAO.class);
			if( type == 0)
			{
				return (ArrayList<PostItem>) dao.searchTitleInPost(text);	
			}
			else if(type == 1)
			{
				return (ArrayList<PostItem>) dao.searchContentInPost(text);
			}
			else if(type == 2)
			{
				return (ArrayList<PostItem>) dao.searchContentTitleInPost(text);
			}
			else 
			{
				return (ArrayList<PostItem>) dao.searchAllInPost(text);
			}	
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
