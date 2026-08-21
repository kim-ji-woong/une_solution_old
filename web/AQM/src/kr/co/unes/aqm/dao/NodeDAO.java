package kr.co.unes.aqm.dao;

import java.sql.Timestamp;


public interface NodeDAO {
	
	public int getAreaID(int nNodeID);	
	public String getNodeName(int nNodeID);	
	public int getNodeEnabled(int nNodeID);
	public boolean setNodeEnabled(int nNodeID, boolean bEnabled);
	public boolean addNetNode(int nNodeID, String strNodeName, float fNodePosX, float fNodePosY, Timestamp timestamp, String szArea, int nAreaID);
	public boolean existSensor(int nNodeID, String szCode);
	public int addSensor(int nNodeID, String szCode);	
	
	public boolean removeNode(int nNodeID, String lastDataTable, String newDataTable);
	
}
