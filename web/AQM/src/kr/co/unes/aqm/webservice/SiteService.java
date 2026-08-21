package kr.co.unes.aqm.webservice;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import javax.ws.rs.DefaultValue;
import javax.ws.rs.FormParam;
import javax.ws.rs.GET;
import javax.ws.rs.POST;
import javax.ws.rs.Path;
import javax.ws.rs.PathParam;
import javax.ws.rs.Produces;
import javax.ws.rs.core.Response;
import javax.ws.rs.core.Response.Status;

import com.google.gson.Gson;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;

import kr.co.unes.aqm.dto.NetNode;
import kr.co.unes.aqm.dto.site.NodeLinkImageMap;
import kr.co.unes.aqm.dto.site.NodeLocation;
import kr.co.unes.aqm.model.NodeImageMapDataAccessManager;
import kr.co.unes.aqm.model.NodeLocationDataAccessManager;
import kr.co.unes.aqm.model.SiteDataAccessManager;


@Path("/Site")
public class SiteService 
{
	private NodeLocationDataAccessManager dataManager = new NodeLocationDataAccessManager();
	private SiteDataAccessManager siteManager = new SiteDataAccessManager();
	private NodeImageMapDataAccessManager imgManager = new NodeImageMapDataAccessManager();
	@GET
	@Path("/nodes/{siteID}")
	@Produces("application/json; charset=UTF-8")
	public Response getSiteNodes(@DefaultValue("-1") @PathParam("siteID") int nSiteID)	
	{
		String szResultJson = "{\"Site\":{\"values\":[{}]}, \"Result\":-1}";
		try
		{			
			if( nSiteID >= 0)
			{
				ArrayList<Map<String, Object>> ar = new ArrayList<Map<String, Object>>(); 
				List<NodeLocation> nodeList = dataManager.getNodeLocation(nSiteID);
				for(NodeLocation node : nodeList)
				{		
					Map<String, Object> map2 = new HashMap<String, Object>();
					map2.put("nodeID", ""+ node.getNodeID());
					map2.put("nodeName", node.getName());
					
					int nImg = node.getMapImage();
					if( nImg > 0)
					{
						NodeLinkImageMap img = imgManager.getImageMap(nImg);
						if( img != null)
							map2.put("ImageKey", img.getUrl());	
						
					}
					
					ar.add(map2);
				}
				
				Gson gson = new Gson();
				Map<String, Object> map = new HashMap<String, Object>();
				map.put("values", ar);	
				JsonElement el = gson.toJsonTree(map);		
				JsonObject obj = new JsonObject ();
				obj.add("Site", el);
				obj.addProperty("Result", 2);			

				szResultJson = gson.toJson(obj);
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();		
	}
	

	@GET
	@Path("/node/available")
	@Produces("application/json; charset=UTF-8")
	public Response getUnlinkNodes()	
	{
		String szResultJson = "{\"Site\":{\"values\":[{}]}, \"Result\":-1}";
		try
		{
			ArrayList<Map<String, Object>> ar = new ArrayList<Map<String, Object>>(); 
			List<NetNode> nodeList = siteManager.getUnlinkNodes(-1);
			for(NetNode node : nodeList)
			{		
				Map<String, Object> map2 = new HashMap<String, Object>();
				map2.put("nodeID", ""+ node.getID());
				map2.put("nodeName", node.getNodeName());
				ar.add(map2);
			}
			
			Gson gson = new Gson();
			Map<String, Object> map = new HashMap<String, Object>();
			map.put("values", ar);	
			JsonElement el = gson.toJsonTree(map);		
			JsonObject obj = new JsonObject ();
			obj.add("Site", el);
			obj.addProperty("Result", 2);			

			szResultJson = gson.toJson(obj);
			
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();		
	}
	
	@GET
	@Path("/node/available/{id}")
	@Produces("application/json; charset=UTF-8")
	public Response getUnlinkNodes(@PathParam("id") int nNodeID)	
	{
		String szResultJson = "{\"Site\":{\"values\":[{}]}, \"Result\":-1}";
		try
		{
			
			ArrayList<Map<String, Object>> ar = new ArrayList<Map<String, Object>>(); 
			List<NetNode> nodeList = siteManager.getUnlinkNodes(nNodeID);
			for(NetNode node : nodeList)
			{		
				Map<String, Object> map2 = new HashMap<String, Object>();
				map2.put("nodeID", ""+ node.getID());
				map2.put("nodeName", node.getNodeName());
				ar.add(map2);
			}
			
			Gson gson = new Gson();
			Map<String, Object> map = new HashMap<String, Object>();
			map.put("values", ar);	
			JsonElement el = gson.toJsonTree(map);		
			JsonObject obj = new JsonObject ();
			obj.add("Site", el);
			obj.addProperty("Result", 2);			

			szResultJson = gson.toJson(obj);
			
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();		
	}
	
	
	@POST
	@Path("/new")
	@Produces("application/json; charset=UTF-8")	
	public Response addNetNode(
			@FormParam("NodeID") int nNodeID, 
			@FormParam("NodeName") String strNodeName,
			@FormParam("NodePosX") float fNodePosX,
			@FormParam("NodePosY") float fNodePosY,
			@DefaultValue("-1") @FormParam("Area") int nAreaID,
			@DefaultValue("") @FormParam("Materials") String szMaterialCode)
	{
		return Response.status(Response.Status.OK).build();
	}
}
