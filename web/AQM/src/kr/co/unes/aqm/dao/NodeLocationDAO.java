package kr.co.unes.aqm.dao;

import java.util.List;
import org.apache.ibatis.annotations.Param;

import kr.co.unes.aqm.dto.site.NodeLocation;


public interface NodeLocationDAO {

	public List<NodeLocation> getNodeLocation(@Param("siteID") int nNodeID);
	
	
	public void addNodeLocation(NodeLocation loc);
	
	public void updateNodeLocation(NodeLocation loc);
	
	public void deleteNodeLocation(@Param("nodeID") int nNodeID);


	public NodeLocation getNodeLocationByLinkID(int linkID);
	public NodeLocation getNodeLocationByNodeID(@Param("netNodeID") int nodeID);
}
