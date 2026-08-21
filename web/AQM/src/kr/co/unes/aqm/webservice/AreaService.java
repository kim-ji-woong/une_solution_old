package kr.co.unes.aqm.webservice;

import kr.co.unes.aqm.dto.*;
import kr.co.unes.aqm.dto.area.AreaDepth1;
import kr.co.unes.aqm.dto.area.AreaDepth2;
import kr.co.unes.aqm.dto.area.AreaDepth3;
import kr.co.unes.aqm.dto.area.AreaDepth4;
import kr.co.unes.aqm.dto.area.AreaName;
import kr.co.unes.aqm.model.AreaDataAccessManager;

import java.util.HashMap;
import java.util.Map;
import java.util.ArrayList;

import javax.ws.rs.GET;
import javax.ws.rs.Path;
import javax.ws.rs.PathParam;
import javax.ws.rs.Produces; 
import javax.ws.rs.core.Response;
import javax.ws.rs.core.Response.Status;

import com.google.gson.Gson;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;

@Path("Area")
public class AreaService
{
	@GET
	@Path("/depth1s")
	@Produces("application/json; charset=UTF-8")
	public Response getAreaDepth1List()
	{
		String szResultJson = "{\"Area\":{\"depth1s\":[{}]}, \"Result\":-1}";
		try
		{	
			ArrayList<AreaDepth1> depth1s = new AreaDataAccessManager().getAreaDepth1List();
			int nCount = -1;
			if(depth1s != null)
			{
				nCount = depth1s.size();
			}
			else
			{
				depth1s = new ArrayList<AreaDepth1>();
			}				
			
			Gson gson = new Gson();
			Map<String, Object> map = new HashMap<String, Object>();
			map.put("depth1s", depth1s);	
			JsonElement el = gson.toJsonTree(map);		
			JsonObject obj = new JsonObject ();
			obj.add("Area", el);
			obj.addProperty("Result", nCount);			

			szResultJson = gson.toJson(obj);
			
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();		
	}
	
	@GET
	@Path("/depth2s/{depth1}")
	@Produces("application/json; charset=UTF-8")
	public Response getAreaDepth2List(@PathParam("depth1") String szDepth1)
	{
		String szResultJson = "{\"Area\":{\"depth2s\":[{}]}, \"Result\":-1}";
		try
		{
			ArrayList<AreaDepth2> depth2s = new AreaDataAccessManager().getAreaDepth2List(szDepth1);			
			//ArrayList<AreaDepth2> depth2s = null;		
			
			int nCount = -1;
			if(depth2s != null)
			{
				nCount = depth2s.size();
			}
			else
			{
				depth2s = new ArrayList<AreaDepth2>();
			}				
			
			Gson gson = new Gson();
			Map<String, Object> map = new HashMap<String, Object>();
			map.put("depth2s", depth2s);	
			JsonElement el = gson.toJsonTree(map);		
			JsonObject obj = new JsonObject ();
			obj.add("Area", el);
			obj.addProperty("Result", nCount);			

			szResultJson = gson.toJson(obj);
			
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}	
	
	@GET
	@Path("depth3s/{depth1}/{depth2}")
	@Produces("application/json; charset=UTF-8")
	public Response getAreaDepth3List(@PathParam("depth1") String szDepth1, @PathParam("depth2") String szDepth2)
	{
		String szResultJson = "{\"Area\":{\"depth3s\":[{}]}, \"Result\":-1}";
		try
		{
			ArrayList<AreaDepth3> depth3s = new AreaDataAccessManager().getAreaDepth3List(szDepth1,szDepth2);			
			//ArrayList<AreaDepth2> depth2s = null;		
			
			int nCount = -1;
			if(depth3s != null)
			{
				nCount = depth3s.size();
			}
			else
			{
				depth3s = new ArrayList<AreaDepth3>();
			}				
			
			Gson gson = new Gson();
			Map<String, Object> map = new HashMap<String, Object>();
			map.put("depth3s", depth3s);	
			JsonElement el = gson.toJsonTree(map);		
			JsonObject obj = new JsonObject ();
			obj.add("Area", el);
			obj.addProperty("Result", nCount);			

			szResultJson = gson.toJson(obj);
			
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	
	@GET
	@Path("depth4s/{depth1}/{depth2}/{depth3}")
	@Produces("application/json; charset=UTF-8")
	public Response getAreaDepth4List(@PathParam("depth1") String szDepth1, @PathParam("depth2") String szDepth2 , @PathParam("depth3") String szDepth3)
	{
		String szResultJson = "{\"Area\":{\"depth4s\":[{}]}, \"Result\":-1}";
		try
		{
			ArrayList<AreaDepth4> depth4s = new AreaDataAccessManager().getAreaDepth4List(szDepth1,szDepth2, szDepth3);			
			//ArrayList<AreaDepth2> depth2s = null;		
			
			int nCount = -1;
			if(depth4s != null)
			{
				nCount = depth4s.size();
			}
			else
			{
				depth4s = new ArrayList<AreaDepth4>();
			}				
			
			Gson gson = new Gson();
			Map<String, Object> map = new HashMap<String, Object>();
			map.put("depth4s", depth4s);	
			JsonElement el = gson.toJsonTree(map);		
			JsonObject obj = new JsonObject ();
			obj.add("Area", el);
			obj.addProperty("Result", nCount);			

			szResultJson = gson.toJson(obj);
			
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	@GET
	@Path("id/{depth1}/{depth2}/{depth3}/{depth4}")
	@Produces("application/json; charset=UTF-8")
	public Response getAreaID(
			@PathParam("depth1") String szDepth1,
			@PathParam("depth2") String szDepth2, 
			@PathParam("depth3") String szDepth3,
			@PathParam("depth4") String szDepth4
			)
	{
		String szResultJson = "{\"Area\":{\"id\":-1}, \"Result\":-1}";
		try
		{
			int nID = new AreaDataAccessManager().getAreaID(szDepth1, szDepth2, szDepth3, szDepth4);			
								
			JsonObject obj1 = new JsonObject ();
			obj1.addProperty("id", nID);
			
			JsonObject obj = new JsonObject ();
			obj.add("Area", obj1);
			obj.addProperty("Result", (nID >= 0 ? 0 : -1));			

			Gson gson = new Gson();
			szResultJson = gson.toJson(obj);
			
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	@GET
	@Path("id/{depth1}/{depth2}/{depth3}")
	@Produces("application/json; charset=UTF-8")
	public Response getAreaID(
			@PathParam("depth1") String szDepth1,
			@PathParam("depth2") String szDepth2, 
			@PathParam("depth3") String szDepth3
			)
	{
		String szResultJson = "{\"Area\":{\"id\":-1}, \"Result\":-1}";
		try
		{
			int nID = new AreaDataAccessManager().getAreaID(szDepth1, szDepth2, szDepth3, null);			
								
			JsonObject obj1 = new JsonObject ();
			obj1.addProperty("id", nID);
			
			JsonObject obj = new JsonObject ();
			obj.add("Area", obj1);
			obj.addProperty("Result", (nID >= 0 ? 0 : -1));			

			Gson gson = new Gson();
			szResultJson = gson.toJson(obj);
			
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	@GET
	@Path("name/{AreaID}")
	@Produces("application/json; charset=UTF-8")
	public Response getAreaName(@PathParam("AreaID") int nAreaID)
	{
		String szResultJson = "{\"Area\":{\"name\":{}}, \"Result\":-1}";
		try
		{
			AreaName name = new AreaDataAccessManager().getAreaName(nAreaID);			
			if( name != null)
			{
				Gson gson = new Gson();
				JsonElement el = gson.toJsonTree(name);	
				
				JsonObject obj1 = new JsonObject ();
				obj1.add("name", el);
				
				JsonObject obj = new JsonObject ();
				obj.add("Area",obj1);
				obj.addProperty("Result", 0);
				szResultJson = gson.toJson(obj);
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
}
