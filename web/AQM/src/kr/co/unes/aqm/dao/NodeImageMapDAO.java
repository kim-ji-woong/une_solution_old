package kr.co.unes.aqm.dao;

import org.apache.ibatis.annotations.Param;

import kr.co.unes.aqm.dto.site.NodeLinkImageMap;


public interface NodeImageMapDAO {

	public void saveUploadImage(NodeLinkImageMap file);
	
	public void unlinkNodeImageMap(@Param("nodelocationid") int nPostID,@Param("mapID") int fileID);
	public void setNodeImageMap(@Param("nodelocationid") int nPostID,@Param("mapID") int fileID);
	
	public void setUseNodeLocation(int fileID);	
	public void setUnuseNodeLocation(int fileID);		
	
	public int getImageMapID(String uuid);
	public NodeLinkImageMap getImageMap(String uuid);
	public NodeLinkImageMap getImageMapFormID(int nID);	
	
	public void deleteUnlinkedMaps();	
	public void deleteImageMap(int nPostID);	
}
