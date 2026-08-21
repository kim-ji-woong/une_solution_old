package kr.co.unes.aqm.dto;

import java.sql.Timestamp;

public class NetNode {

	private int id;
	private String nodeName;
	private float nodePosX;
	private float nodePosY;
	private Timestamp regDate;
	private String nodeAddress;
	private String nodeDataTable;
	private String nodeLastTable;
	private int aeraID;
	private String homePage;
	
	public int getID() {
		return id;
	}
	public void setID(int id) {
		this.id = id;
	}
	public String getNodeName() {
		return nodeName;
	}
	public void setNodeName(String nodeName) {
		this.nodeName = nodeName;
	}
	public float getNodePosX() {
		return nodePosX;
	}
	public void setNodePosX(float nodePosX) {
		this.nodePosX = nodePosX;
	}
	public float getNodePosY() {
		return nodePosY;
	}
	public void setNodePosY(float nodePosY) {
		this.nodePosY = nodePosY;
	}
	protected Timestamp getRegDate() {
		return regDate;
	}
	protected void setRegDate(Timestamp regDate) {
		this.regDate = regDate;
	}
	public String getNodeAddress() {
		return nodeAddress;
	}
	public void setNodeAddress(String nodeAddress) {
		this.nodeAddress = nodeAddress;
	}
	public String getNodeDataTable() {
		return nodeDataTable;
	}
	public void setNodeDataTable(String nodeDataTable) {
		this.nodeDataTable = nodeDataTable;
	}
	public String getNodeLastTable() {
		return nodeLastTable;
	}
	public void setNodeLastTable(String nodeLastTable) {
		this.nodeLastTable = nodeLastTable;
	}
	public int getAeraID() {
		return aeraID;
	}
	public void setAeraID(int aeraID) {
		this.aeraID = aeraID;
	}
	public String getHomePage() {
		return homePage;
	}
	public void setHomePage(String homePage) {
		this.homePage = homePage;
	}
}
