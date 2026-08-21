package kr.co.unes.aqm.dao;

import java.util.List;

import org.apache.ibatis.annotations.Param;

import kr.co.unes.aqm.dto.post.PostItem;

public interface PostDAO {

	PostItem getPost(int nID);
	
	List<PostItem> getAllPost();
	
	boolean writePost(PostItem item);	
	
	boolean modifyPost(PostItem item);
	
	boolean deletePost(int nID);	
	
	List<PostItem> searchTitleInPost(@Param("text") String text);	
	List<PostItem> searchContentInPost(@Param("text") String text);
	List<PostItem> searchContentTitleInPost(@Param("text") String text);
	List<PostItem> searchAllInPost(@Param("text") String text);
}
