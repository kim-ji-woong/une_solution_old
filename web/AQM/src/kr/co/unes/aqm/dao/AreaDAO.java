package kr.co.unes.aqm.dao;

import java.util.List;

import kr.co.unes.aqm.dto.area.AreaDepth1;
import kr.co.unes.aqm.dto.area.AreaDepth2;
import kr.co.unes.aqm.dto.area.AreaDepth3;
import kr.co.unes.aqm.dto.area.AreaDepth4;
import kr.co.unes.aqm.dto.area.AreaName;


public interface AreaDAO {

	public List<AreaDepth1> getAreaDepth1();
	public List<AreaDepth2> getAreaDepth2(String depth1);		
	public List<AreaDepth3> getAreaDepth3(String depth1,String depth2);
	public List<AreaDepth4> getAreaDepth4(String depth1,String depth2, String depth3);
	
	public int getAreaID(String depth1,String depth2, String depth3, String depth4);		
	public AreaName getAreaName(int areaID);

}
