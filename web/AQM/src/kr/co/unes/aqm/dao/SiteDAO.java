package kr.co.unes.aqm.dao;

import java.util.List;

import org.apache.ibatis.annotations.Param;

import kr.co.unes.aqm.dto.NetNode;
import kr.co.unes.aqm.dto.area.AreaDepth1;
import kr.co.unes.aqm.dto.sensor.CityStatus;
import kr.co.unes.aqm.dto.site.Site;

public interface SiteDAO
{	
	final String [][] mAreaList = {
			{ "서울특별시", "37.550434,126.969393" },
			{"경기도",  "37.274712, 127.009632" },
			{"강원도", "37.885395, 127.729786" },
			{"충청북도", "36.635413, 127.491419" },
			{"충청남도", "36.658889, 126.672876" },
			{"전라남도", "34.816219, 126.462913" },
			{"전라북도", "35.820343, 127.108727" },
			{"경상북도", "36.576051, 128.505761" },
			{"경상남도", "35.238229, 128.692344" },
			{"광주광역시", "35.160112, 126.851265" },
			{"대구광역시", "35.871468, 128.601285" },
			{"대전광역시 ","36.350406, 127.384510" },
			{"부산광역시", "35.179755, 129.075002" },
			{"세종특별자치시", "36.480107, 127.289025" },
			{"울산광역시 ", "35.538774,129.311348" },
			{"인천광역시", "37.456131, 126.705250" },
			{"제주특별자치도","33.488987, 126.49835" }
	};
	
	public List<AreaDepth1> getAreaForNode();
	public List<Site>	getAllLocation();
	public List<Site> 	getLocationForAreaDepth1(@Param("depth1") String depth1);
	public List<Site> 	nameSearch(@Param("nodeName") String nodeName);
		
	public List<Site> regionSearch1(@Param("depth1") String szDepth1);
	public List<Site> regionSearch2(@Param("depth1") String szDepth1, @Param("depth2")String szDepth2);
	public List<Site> regionSearch3(@Param("depth1") String szDepth1, @Param("depth2")String szDepth2, @Param("depth3" )String szDepth3);	
	public List<Site> regionSearch( @Param("depth1") String szDepth1, @Param("depth2")String szDepth2, @Param("depth3") String szDepth3, @Param("depth4") String szDepth4);

	public void addLocation(Site loc);
	public Site getLocation(int id);
	public void updateLocation(Site loc);
	
	public List<NetNode> getUnlinkNode();
	public List<NetNode> getUnlinkNodeIncludeNode(@Param("nodeID") int nodeID);
	public NetNode getNetNode(@Param("nodeID") int nodeID);
	
	
	public void deleteLocation(@Param("siteID") int id);
	
	
	public List<CityStatus> getAllCityState();
	public CityStatus getCityState(@Param("cityName")String strNodeName);
}
