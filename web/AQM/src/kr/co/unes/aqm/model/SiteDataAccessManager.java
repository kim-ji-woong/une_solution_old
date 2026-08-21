package kr.co.unes.aqm.model;

import java.util.ArrayList;
import java.util.List;

import org.apache.ibatis.session.SqlSession;

import kr.co.unes.aqm.config.AQMSessionFactory;
import kr.co.unes.aqm.dao.SiteDAO;
import kr.co.unes.aqm.dto.NetNode;
import kr.co.unes.aqm.dto.area.AreaDepth1;
import kr.co.unes.aqm.dto.sensor.CityStatus;
import kr.co.unes.aqm.dto.site.Site;

public class SiteDataAccessManager 
{
	private String [][] mAreaList = {
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
	
	public List<CityStatus> getAllState() throws Exception
	{	
		/*List<List<String>> result = new ArrayList<List<String>>();
		for( int i = 0 ; i < 17 ; i++ )
		{
			List<String> temp1 = new ArrayList<String>();
			for(int j = 0 ; j < 2 ; j++)
			{
				temp1.add(mAreaList[i][j]);
			}	
			result.add(temp1);
		}	
		return result;*/
		
		
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			SiteDAO dao = sqlSession.getMapper(SiteDAO.class);			
			return dao.getAllCityState();	
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
	
	public CityStatus getCityState(String strNodeName) {
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			SiteDAO dao = sqlSession.getMapper(SiteDAO.class);			
			return dao.getCityState(strNodeName);	
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


	public List<AreaDepth1> getAreaForNode() 
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			SiteDAO dao = sqlSession.getMapper(SiteDAO.class);			
			return dao.getAreaForNode();	
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
	
	public Site getLocation(int id)
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			SiteDAO dao = sqlSession.getMapper(SiteDAO.class);			
			return dao.getLocation(id);	
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
	
	public List<Site> getAllLocation() 
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			SiteDAO dao = sqlSession.getMapper(SiteDAO.class);			
			return dao.getAllLocation();	
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
	
	public List<Site> 	getLocationForAreaDepth1(String depth1)
	{		
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			SiteDAO dao = sqlSession.getMapper(SiteDAO.class);			
			return dao.getLocationForAreaDepth1(depth1);	
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
	
	public NetNode getNetNode(int nNodeID)
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);
			SiteDAO dao = sqlSession.getMapper(SiteDAO.class);	
			return dao.getNetNode(nNodeID);			
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
	
	public List<NetNode> getUnlinkNodes(int nID)
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			SiteDAO dao = sqlSession.getMapper(SiteDAO.class);	
			if( nID == -1)
				return dao.getUnlinkNode();
			
			return dao.getUnlinkNodeIncludeNode(nID);
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
	
	public List<Site> nameSearch(String nodeName) {

		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			SiteDAO dao = sqlSession.getMapper(SiteDAO.class);			
			return dao.nameSearch(nodeName);	
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
	

	public List<Site> regionSearch(String szDepth1, String szDepth2, String szDepth3, String szDepth4) {
		
		if( szDepth2 == null || szDepth2.isEmpty())
			return regionSearch1(szDepth1);
		
		if( szDepth3 == null || szDepth3.isEmpty())
			return regionSearch2(szDepth1, szDepth2);
		
		if( szDepth4 == null || szDepth4.isEmpty())
			return regionSearch3(szDepth1, szDepth2, szDepth3);
		
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			SiteDAO dao = sqlSession.getMapper(SiteDAO.class);			
			return dao.regionSearch(szDepth1, szDepth2, szDepth3, szDepth4);	
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
	
	public List<Site> regionSearch1(String szDepth1)
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			SiteDAO dao = sqlSession.getMapper(SiteDAO.class);			
			return dao.regionSearch1(szDepth1);	
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
	public List<Site> regionSearch2(String szDepth1, String szDepth2)
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			SiteDAO dao = sqlSession.getMapper(SiteDAO.class);			
			return dao.regionSearch2(szDepth1, szDepth2);	
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
	public List<Site> regionSearch3(String szDepth1, String szDepth2, String szDepth3)
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			SiteDAO dao = sqlSession.getMapper(SiteDAO.class);			
			return dao.regionSearch3(szDepth1, szDepth2, szDepth3);	
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
	

	public void updateLocation(Site loc)
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			SiteDAO dao = sqlSession.getMapper(SiteDAO.class);			
			dao.updateLocation(loc);	
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

	public void addLocation(Site loc) 
	{
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			SiteDAO dao = sqlSession.getMapper(SiteDAO.class);			
			dao.addLocation(loc);	
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
	
	public void deleteLocation(int nID) {
		SqlSession sqlSession = null;
		try
		{
			sqlSession = AQMSessionFactory.getSqlSessionFactory().openSession(true);	
			SiteDAO dao = sqlSession.getMapper(SiteDAO.class);			
			dao.deleteLocation(nID);
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

	
	
//	public List<Site> getStateAverage(String szLocName) throws Exception
//	{
//		// get state all location
//		List<Site> areaLoc = getLocationForAreaDepth1(szLocName);		
//		List<Site> result = new ArrayList<Site>();
//		// average location air quality		
//		for(int i = 0 ; i < areaLoc.size(); i++)
//		{
//			Site loc = areaLoc.get(i);
//			int nLocationID = loc.getID();
//
//			result.add(loc);
//		}		
//		return result;
//	}
	

	public List<List<String>> getLocationAirQuality(int nID) {
		List<List<String>> result = new ArrayList<List<String>>();
		Site loc = new Site();
		loc.setName("서울대학교");
		loc.setAddress("");
		loc.setDetailAddress("");
		loc.setPhone("");
		loc.setLocationX(37.514737f);
		loc.setLocationY(127.109455f);
		result.add(loc.toFTL());
		return result;
	}

	
	

}
