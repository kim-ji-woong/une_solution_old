package kr.co.unes.aqm.dao;

import java.util.List;

import org.apache.ibatis.annotations.Param;

import kr.co.unes.aqm.dto.post.AttachFile;
import kr.co.unes.aqm.dto.post.FileInPost;

public interface AttachFileDAO {

	public void saveUploadFile(AttachFile file);
	
	public void setAttachFileInPost(@Param("postid") int nPostID,@Param("fileID") int fileID);
	public void setUsePost(int fileID);	
	public void setUnusePost(int fileID);	
	public List<FileInPost> getFilesInPost(int nPostID);
	
	public int getAttachedFileID(String uuid);
	public AttachFile getAttachedFile(String uuid);
	public AttachFile getAttachedFileFromID(int nID);	
	public List<AttachFile> getAttachedFiles(int postID);
	
	public void deleteUnlinkedFiles();	
	public void deleteAttachedFile(int nPostID);
	public void deleteFileInPost(int nPostID);
	
}
