package kr.co.unes.aqm.view;

import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import javax.ws.rs.DefaultValue;
import javax.ws.rs.FormParam;
import javax.ws.rs.GET;
import javax.ws.rs.POST;
import javax.ws.rs.Path;
import javax.ws.rs.Produces;
import javax.ws.rs.core.Response;
import javax.ws.rs.core.Response.Status;

import kr.co.unes.aqm.dto.site.Site;
import kr.co.unes.aqm.model.SiteDataAccessManager;

import org.glassfish.jersey.server.mvc.Viewable;

import com.google.gson.Gson;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;

@Path("/Search")
public class SearchFtlView {
	
	private SiteDataAccessManager mLocManager = new SiteDataAccessManager();
	@GET
	@Produces("text/html; charset=UTF-8")
	@Path("/")
    public Response search() 
	{			
		Map<String, Object> map = new LinkedHashMap<String, Object>();
        Viewable view = new Viewable("/search.ftl",map );        
        return Response.ok(view).build();
    }	
	
	@POST
	@Produces("text/html; charset=UTF-8")
	@Path("/nodeAverage")
	public Response getAverageForSensor(
			@DefaultValue("-1") @FormParam("LocationID") int nLocationID, 
			@DefaultValue("") @FormParam("LocationName") String strNodeName)
	{
		Map<String, Object> map = new LinkedHashMap<String, Object>();
		
		map.put("LocationID", nLocationID);
		map.put("LocationName", strNodeName);
		
		if( mLocManager != null)
		{
			try
			{
				List<List<String>> searchResult = mLocManager.getLocationAirQuality(nLocationID);
				if(searchResult.size() == 0)
					return Response.ok().build();
				map.put("SensorValues", searchResult);		
			}catch(Exception e)
			{				
			}
		}		
		Viewable view = new Viewable("/sensor/sensorDetail.ftl",map );        
        return Response.ok(view).build();
	}
	
	@POST
	@Produces("application/json; charset=UTF-8")
	@Path("/nodeList/json")
    public Response jsonSearchResult(
    		@DefaultValue("-1") @FormParam("SearchType") int nSearchType, 
			@DefaultValue("") @FormParam("Name") String strNodeName,
			@DefaultValue("") @FormParam("depth1") String szDepth1,
			@DefaultValue("") @FormParam("depth2") String szDepth2,
			@DefaultValue("") @FormParam("depth3") String szDepth3,
			@DefaultValue("") @FormParam("depth4") String szDepth4) {
		
		String szResultJson = "{\"SensorList\":{\"Sensors\":[{}]}, \"Result\":-1}";
		Map<String, Object> map = new LinkedHashMap<String, Object>();
		try
		{
			if( nSearchType == 1)
			{
				if(strNodeName != "")
				{
					if( mLocManager != null)
					{
						List<Site> searchResult = mLocManager.nameSearch(strNodeName);				
						if(searchResult.size() == 0)
							return Response.status(Response.Status.OK).entity(szResultJson).build();
						Gson gson = new Gson();				
						map.put("Sensors", searchResult);	
						JsonElement el = gson.toJsonTree(map);		
						JsonObject obj = new JsonObject ();
						obj.add("SensorList", el);
						obj.addProperty("Result", searchResult.size());
						szResultJson = gson.toJson(obj);
					}				
				}			
			}
			else if( nSearchType == 2)
			{
				if( mLocManager != null)
				{
					List<Site> searchResult = mLocManager.regionSearch(szDepth1, szDepth2, szDepth3, szDepth4);
					if(searchResult.size() == 0)
						return Response.status(Response.Status.OK).entity(szResultJson).build();
					Gson gson = new Gson();				
					map.put("Sensors", searchResult);	
					JsonElement el = gson.toJsonTree(map);		
					JsonObject obj = new JsonObject ();
					obj.add("SensorList", el);
					obj.addProperty("Result", searchResult.size());
					szResultJson = gson.toJson(obj);
				}	
			}
		}
		catch(Exception ex)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
    }
	
	@POST
	@Produces("text/html; charset=UTF-8")
	@Path("/resultList")
    public Response searchList(
    		@DefaultValue("-1") @FormParam("SearchType") int nSearchType, 
			@DefaultValue("") @FormParam("Name") String strNodeName,
			@DefaultValue("") @FormParam("selecteddepth1") String szDepth1,
			@DefaultValue("") @FormParam("selecteddepth2") String szDepth2,
			@DefaultValue("") @FormParam("selecteddepth3") String szDepth3,
			@DefaultValue("") @FormParam("selecteddepth4") String szDepth4) {
		
		Map<String, Object> map = new LinkedHashMap<String, Object>();
		
		// Default
		// Name Search
		if( nSearchType == 1)
		{
			if(strNodeName != "")
			{
				if( mLocManager != null)
				{
					List<Site> searchResult = mLocManager.nameSearch(strNodeName);
					if(searchResult.size() == 0)
						return Response.ok().build();
					map.put("LocationList", searchResult);
					
					makePageNavigationInfo(map, searchResult, 8);
				}				
			}			
		}
		else if( nSearchType == 2)
		{
			if( mLocManager != null)
			{
				List<Site> searchResult = mLocManager.regionSearch(szDepth1, szDepth2, szDepth3, szDepth4);
				map.put("LocationList", searchResult);
				
				if( szDepth1 != "")
					map.put("AreaDepth1", szDepth1);
				if( szDepth2 != "")
					map.put("AreaDepth2", szDepth2);
				if( szDepth3 != "")
					map.put("AreaDepth3", szDepth3);
				if( szDepth4 != "")
					map.put("AreaDepth4", szDepth4);	
				
				makePageNavigationInfo(map, searchResult, 8);
			}	
		}
        Viewable view = new Viewable("/common/search_list.ftl",map );        
        return Response.ok(view).build();
    }
	
	private int makePageNavigationInfo(Map<String, Object> map, List<Site> locationList, int nPagePerItem)
	{
		int nItemCount = locationList.size();
		int nExtraCount = nItemCount % nPagePerItem;
		int nPage = nItemCount / nPagePerItem;
		
		if(nExtraCount > 0)
			nPage = nPage + 1;
		
		map.put("PageCount", nPage);
		map.put("ExtraCount",  nExtraCount);
		
		LinkedHashMap<String, Object> mapPage = new LinkedHashMap<String,Object>();
		for( int j = 0 ; j < nPage; j++)
		{
			List<Site> itemList = new ArrayList<Site>();
			for(int i = 0 ; i < nPagePerItem; i++)
			{
				int index = j * nPagePerItem + i;
				if( index < nItemCount)
				{
					Site item = locationList.get(index);
					itemList.add(item);
				}
				else
				{
					break;
				}				
			}
			mapPage.put("Page" + (j+1), itemList);
		}
		if(mapPage.size() > 0)
			map.put("PageList", mapPage);
		return nPage;
	}
}
