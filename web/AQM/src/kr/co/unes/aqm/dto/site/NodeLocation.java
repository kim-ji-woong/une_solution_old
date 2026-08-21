package kr.co.unes.aqm.dto.site;

public class NodeLocation {

	private int  id;
    private int locationID;
    private int nodeID;
    private String name;
    private int mapImage;
    private String description;
    
	public int getID() {
		return id;
	}
	public void setID(int id) {
		this.id = id;
	}
	
	public int getLocationID() {
		return locationID;
	}
	public void setLocationID(int locationID) {
		this.locationID = locationID;
	}
	
	public int getNodeID() {
		return nodeID;
	}
	public void setNodeID(int nodeID) {
		this.nodeID = nodeID;
	}
	
	public String getName() {
		return name;
	}
	public void setName(String name) {
		this.name = name;
	}
	
	public int getMapImage() {
		return mapImage;
	}
	public void setMapImage(int mapImage) {
		this.mapImage = mapImage;
	}
	
	public String getDescription() {
		return description;
	}
	public void setDescription(String description) {
		this.description = description;
	}
}
