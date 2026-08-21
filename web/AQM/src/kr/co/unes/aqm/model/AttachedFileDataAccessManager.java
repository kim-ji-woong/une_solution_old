package kr.co.unes.aqm.model;

import java.util.HashMap;
import java.util.List;

import org.apache.ibatis.session.SqlSession;

import kr.co.unes.aqm.config.AQMSessionFactory;
import kr.co.unes.aqm.dao.AttachFileDAO;
import kr.co.unes.aqm.dto.post.AttachFile;
import kr.co.unes.aqm.dto.post.FileInPost;

public class AttachedFileDataAccessManager {


	public void saveUploadFile(AttachFile file) 
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			AttachFileDAO dao = sqlSession.getMapper(AttachFileDAO.class);			
			dao.saveUploadFile(file);		
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
	
	public void deleteAttachFileInPost(int postID)
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			AttachFileDAO dao = sqlSession.getMapper(AttachFileDAO.class);	
			
			// get fileInpost postID
			List<FileInPost> files = dao.getFilesInPost(postID);				
			for(FileInPost post : files)
			{
				// setusepost = false
				dao.setUnusePost(post.getFileID());	
				
			}
			dao.deleteFileInPost(postID);
			
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

	public void modifyAttachFileInPost(int nPostID,List<String> files) {
		SqlSession sqlSession = null;
		try
		{			
			deleteAttachFileInPost(nPostID);
			
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			AttachFileDAO dao = sqlSession.getMapper(AttachFileDAO.class);				
			for(String uuid : files)
			{
				int fileID = dao.getAttachedFileID(uuid);
				dao.setAttachFileInPost(nPostID, fileID);
				dao.setUsePost(fileID);
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
	public void setAttachFileInPost(int nPostID, List<String> files) {

		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(false);	
			AttachFileDAO dao = sqlSession.getMapper(AttachFileDAO.class);		
			for(String uuid : files)
			{
				int fileID = dao.getAttachedFileID(uuid);
				dao.setAttachFileInPost(nPostID, fileID);
				dao.setUsePost(fileID);
			}
			
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

	public AttachFile getAttachedFile(String uuid) {
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			AttachFileDAO dao = sqlSession.getMapper(AttachFileDAO.class);			
			AttachFile file = dao.getAttachedFile(uuid);
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

	public AttachFile getAttachedFile(int nID) {
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			AttachFileDAO dao = sqlSession.getMapper(AttachFileDAO.class);			
			AttachFile file = dao.getAttachedFileFromID(nID);
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

	public List<AttachFile> getAttachedFiles(int postID) {
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			AttachFileDAO dao = sqlSession.getMapper(AttachFileDAO.class);			
			List<AttachFile> files = dao.getAttachedFiles(postID);
			return files;
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

	public void deleteUnlinkedFiles() {
		// TODO Auto-generated method stub
		
	}

	public void deleteAttachedFile(int postID) {
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(false);	
			AttachFileDAO dao = sqlSession.getMapper(AttachFileDAO.class);	
			
			// get fileInpost postID
			List<FileInPost> files = dao.getFilesInPost(postID);		
			dao.deleteFileInPost(postID);
			
			for(FileInPost post : files)
			{
				dao.deleteAttachedFile(post.getFileID());				
			}
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
