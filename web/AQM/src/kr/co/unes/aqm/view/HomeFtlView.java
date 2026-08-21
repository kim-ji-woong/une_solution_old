package kr.co.unes.aqm.view;

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

import kr.co.unes.aqm.dto.sensor.CityStatus;
import kr.co.unes.aqm.dto.site.Site;
import kr.co.unes.aqm.model.SiteDataAccessManager;
import kr.co.unes.aqm.servlet.AQMAverageDataWorker;

import org.glassfish.jersey.server.mvc.Viewable;
import org.slf4j.LoggerFactory;

import com.google.gson.Gson;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;

@Path("/")
public class HomeFtlView 
{
	private SiteDataAccessManager mLocManager = new SiteDataAccessManager();
	private final org.slf4j.Logger logger = LoggerFactory.getLogger(HomeFtlView.class);
	
	public HomeFtlView()
	{
	}
	
	
	@GET
	@Produces("text/html; charset=UTF-8")
	@Path("/jsp")
    public Response index2() {
        Map<String, Object> map = new LinkedHashMap<String, Object>();
        map.put("user", "usul");   
        Viewable view = new Viewable("/sample.jsp",map );
        
        return Response.ok(view).build();
    }
	
	@GET
	@Produces("text/html; charset=UTF-8")
	@Path("/")
    public Response index() {
        Map<String, Object> map = new LinkedHashMap<String, Object>();
        map.put("user", "usul");   
        Viewable view = new Viewable("/index.ftl",map );
        
        return Response.ok(view).build();
    }

	
	@GET
	@Produces("text/html; charset=UTF-8")
	@Path("/Home")
    public Response home() {
        Map<String, Object> map = new LinkedHashMap<String, Object>();
        map.put("user", "usul");   
        Viewable view = new Viewable("/index.ftl",map );
        
        return Response.ok(view).build();
    }
	
	@GET
	@Produces("text/html; charset=UTF-8")
	@Path("/Marker/Average")
	public Response getMarkers() 
	{
		Map<String, Object> map = new LinkedHashMap<String, Object>();
		map.put("LocationName", "");
		
		if( mLocManager != null)
		{
			try
			{
				List<CityStatus> searchResult = mLocManager.getAllState();
				if(searchResult.size() == 0)
					return Response.ok().build();
				map.put("LocationList", searchResult);
			}catch(Exception e)
			{
				logger.debug("getMarkers", e.getMessage());
			}
		}
		Viewable view = new Viewable("/common/markerAverage.ftl",map );        
        return Response.ok(view).build();
	}
	
	@POST
	@Produces("application/json; charset=UTF-8")
	@Path("/Marker/Average/json")
	public Response getStateMarkersJSON(@DefaultValue("") @FormParam("ID")  String szID) 
	{
		String szResultJson = "{\"SensorList\":{\"Sensors\":[{}]}, \"Result\":-1}";
		try
		{
			if( mLocManager != null)
			{
				try
				{
					List<CityStatus> searchResult = mLocManager.getAllState();
					if(searchResult.size() == 0)
						return Response.status(Response.Status.OK).entity(szResultJson).build();
					Gson gson = new Gson();
					Map<String, Object> map = new LinkedHashMap<String, Object>();
					map.put("Sensors", searchResult);	
					JsonElement el = gson.toJsonTree(map);		
					JsonObject obj = new JsonObject ();
					obj.add("SensorList", el);
					obj.addProperty("Result", searchResult.size());
					szResultJson = gson.toJson(obj);
				}
				catch(Exception e)
				{			
					logger.debug("getMarkers", e.getMessage());
				}
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();	
	}	
	
	@POST
	@Produces("text/html; charset=UTF-8")
	@Path("/Average")
	public Response getStateAverage(@DefaultValue("") @FormParam("LocationName") String strNodeName) 
	{
		Map<String, Object> map = new LinkedHashMap<String, Object>();
		map.put("LocationName", strNodeName);
		
		if( mLocManager != null)
		{
			try
			{
				CityStatus searchResult = mLocManager.getCityState(strNodeName);
				if(searchResult != null)
				{					
					map.put("SensorValue", searchResult);
				}
			}catch(Exception e)
			{		
				logger.debug("getMarkers", e.getMessage());
			}
		}
        Viewable view = new Viewable("/sensor/sensorDetail.ftl",map );        
        return Response.ok(view).build();
	}
}

