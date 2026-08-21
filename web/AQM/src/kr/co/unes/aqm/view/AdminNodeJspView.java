package kr.co.unes.aqm.view;

import java.io.BufferedReader;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;
import java.net.URLEncoder;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
import java.util.StringTokenizer;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;
import javax.ws.rs.Consumes;
import javax.ws.rs.DefaultValue;
import javax.ws.rs.FormParam;
import javax.ws.rs.GET;
import javax.ws.rs.POST;
import javax.ws.rs.Path;
import javax.ws.rs.PathParam;
import javax.ws.rs.Produces;
import javax.ws.rs.core.Context;
import javax.ws.rs.core.HttpHeaders;
import javax.ws.rs.core.MultivaluedMap;
import javax.ws.rs.core.Response;
import javax.ws.rs.core.Response.Status;

import org.glassfish.jersey.media.multipart.FormDataContentDisposition;
import org.glassfish.jersey.media.multipart.FormDataParam;
import org.glassfish.jersey.server.mvc.Viewable;
import org.slf4j.LoggerFactory;

import com.google.gson.Gson;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;

import kr.co.unes.aqm.dto.area.AreaDepth1;
import kr.co.unes.aqm.dto.post.PostItem;
import kr.co.unes.aqm.dto.site.NodeLinkImageMap;
import kr.co.unes.aqm.dto.site.NodeLocation;
import kr.co.unes.aqm.dto.site.Site;
import kr.co.unes.aqm.model.AreaDataAccessManager;
import kr.co.unes.aqm.model.AttachedFileDataAccessManager;
import kr.co.unes.aqm.model.NodeImageMapDataAccessManager;
import kr.co.unes.aqm.model.NodeLocationDataAccessManager;
import kr.co.unes.aqm.model.SiteDataAccessManager;
import kr.co.unes.aqm.model.PostDataAccessManager;
import kr.co.unes.aqm.servlet.AQMLoginManager;

@Path("/Admin")
public class AdminNodeJspView {

	
	
	@Context
	private HttpServletRequest request;
	
	@Context
	private HttpServletResponse response;

	private final org.slf4j.Logger logger = LoggerFactory.getLogger(AdminNodeJspView.class);
	
	private SiteDataAccessManager locationManager = new SiteDataAccessManager();
	private AreaDataAccessManager areaManager = new AreaDataAccessManager();
	private NodeLocationDataAccessManager nodeManager = new NodeLocationDataAccessManager();
	private NodeImageMapDataAccessManager mapManager = new NodeImageMapDataAccessManager();
	
	
	private boolean checkLogin()
	{
		HttpSession session = request.getSession();
		AQMLoginManager manager = AQMLoginManager.getInstance();	
		return manager.checkLogin(session);
	}
	
	public Response requestLogin()
	{		
		return requestLogin(true);
	}
	
	public Response requestLogin(boolean bSavedReferer)
	{
		if( bSavedReferer == true)
		{
			String referrer = request.getHeader("Referer");
		    request.getSession().setAttribute("prevPage", referrer);
		}
		Map<String, Object> map = new LinkedHashMap<String, Object>();
		Viewable view = new Viewable("/admin/admin_login.ftl", map);
		return Response.ok(view).build();
	}
	
	@GET
	@Path("/manage")	
	@Produces("text/html; charset=UTF-8")
	public Response adminManageNode()
	{
		if(!checkLogin())
			return requestLogin(false);		
    
        return viewAdminManage();		
	}
	
	private Response viewAdminManage()
	{
		Map<String, Object> map = new LinkedHashMap<String, Object>();
		List<AreaDepth1> depth1s = locationManager.getAreaForNode();
		if( depth1s != null && depth1s.size() > 0)
		{
			map.put("UseArea", depth1s);			
			String activeName = depth1s.get(0).getDetph();
			map.put("Active", activeName);
			List<Site> areaLoc = locationManager.getLocationForAreaDepth1(activeName);			
			map.put("LocationList", areaLoc);
		}
		
        map.put("user", "usul");   
        Viewable view = new Viewable("/admin/admin_manage_main.jsp",map );        
        return Response.ok(view).build();	
	}
	
	@POST
	@Path("/manage")	
	@Produces("text/html; charset=UTF-8")
	public Response adminManageNodePost(@FormParam("tabname") String szLocName)
	{
		if(!checkLogin())
			return requestLogin(false);		
		
		szLocName = szLocName.trim();
		szLocName = szLocName.replace("\n", "");
		szLocName = szLocName.replace("\r", "");
		
		logger.debug("Tab Move : " + szLocName);
		
		String activeName = "";
		Map<String, Object> map = new LinkedHashMap<String, Object>();
		List<AreaDepth1> depth1s = locationManager.getAreaForNode();
		if( depth1s != null && depth1s.size() > 0)
		{
			map.put("UseArea", depth1s);
			
			if( szLocName == null)
			{
				activeName = depth1s.get(0).getDetph();				
			}
			else
			{
				activeName = szLocName;
			}
			map.put("Active", activeName);
			
			List<Site> areaLoc = locationManager.getLocationForAreaDepth1(activeName);
			map.put("LocationList", areaLoc);
		}
		
        map.put("user", "usul");   
        Viewable view = new Viewable("/admin/admin_manage_main.jsp",map );        
        return Response.ok(view).build();		
	}
	

	@GET
	@Path("/manage/new")	
	@Produces("text/html; charset=UTF-8")
	public Response adminManageCreateNode()
	{
		if(!checkLogin())
			return requestLogin(false);
		
		Map<String, Object> map = new LinkedHashMap<String, Object>();
        map.put("user", "usul");   
        Viewable view = new Viewable("/admin/admin_manage_create.jsp",map );        
        return Response.ok(view).build();		
	}
	
	@POST
	@Path("/manage/new")
	@Produces("text/html; charset=UTF-8")
	public Response adminManageCreateNode(
			@FormParam("Name") String name,
			@FormParam("Phone") String phone,
			@FormParam("Address") String address,
			@FormParam("DetailAddress") String detailAddress,
			@FormParam("Description") String description,
			@FormParam("attach_link") List<Integer> links)
	{
		if(!checkLogin())
			return requestLogin(false);	
		
		Map<String, Object> map = new LinkedHashMap<String, Object>();
		
		
		float x = 0.0f;
		float y = 0.0f;
		
		// Get Geo Location
		try
		{
			String szLocation = getGeoCodingLocation(address);
			StringTokenizer token = new StringTokenizer(szLocation, ",", false);
			String szX = token.nextToken();
			String szY = token.nextToken();
			
			x = Float.parseFloat(szX);
			y = Float.parseFloat(szY);
			logger.debug("Geocoding Result: " + szLocation);
		}
		catch(Exception ex)
		{	
			logger.debug("GeoCoding Error : ", ex);
			x = 0.0f;
			y = 0.0f;
		}	
		
		int nAreaID = -1;
		String szDepth1 = null, szDepth2 = null, szDepth3 = null, szDepth4 = null;
		String[] splited = address.split("\\s+");
		if(splited != null)
		{
			if(splited.length > 0)
			{
				szDepth1 = splited[0];
				szDepth2 = null;
				szDepth3 = null;
				szDepth4 = null;
				if(splited.length > 1)
				{
					szDepth1 = splited[0];
					szDepth2 = splited[1];
					szDepth3 = null;
					szDepth4 = null;
					if(splited.length > 2)
					{
						szDepth1 = splited[0];
						szDepth2 = splited[1];
						szDepth3 = splited[2];
						szDepth4 = null;
						if(splited.length > 3)
						{
							szDepth1 = splited[0];
							szDepth2 = splited[1];
							szDepth3 = splited[2];
							szDepth4 = splited[3];
						}
					}
				}
			}
			
			try {
				nAreaID = areaManager.getAreaID(szDepth1, szDepth2, szDepth3, szDepth4);
			} catch (Exception e) {
				logger.debug("GetAreaID Exception", e);
			}
			
			
			// Add Location
			if(nAreaID > 0)
			{
				Site loc = new Site();
				loc.setName(name);
				loc.setPhone(phone);
				loc.setAddress(address);
				loc.setDetailAddress(detailAddress);
				loc.setArea(nAreaID);
				loc.setDescription(description);
				loc.setLocationX(x);
				loc.setLocationY(y);
				
				locationManager.addLocation(loc);
				
				if( links!= null)
				{
					for(Integer linkID : links)
					{
						int link = linkID.intValue();
						NodeLocation node = nodeManager.getNodeLocationByNodeID(link);
						node.setLocationID(loc.getID());
						nodeManager.updateNodeLocation(node);
					}
				}
			}
			
			
		}
		return viewAdminManage();	
	}	
	
	@GET
	@Path("/manage/detail/{id}")	
	@Produces("text/html; charset=UTF-8")
	public Response adminManageDetail(@DefaultValue("-1") @PathParam("id") int nID)
	{		
		
		if(!checkLogin())
			return requestLogin(false);
		
		Map<String, Object> map = new LinkedHashMap<String, Object>();
		if( nID >= 0)
		{
			Site loc = locationManager.getLocation(nID);
			if(loc != null)
			{
				map.put("targetLocation", loc);   
				
				List<NodeLocation> nodeList = nodeManager.getNodeLocation(nID);
				map.put("linkNodes", nodeList);	
				
				map.put("user", "usul");   
		        Viewable view = new Viewable("/admin/admin_manage_detail.jsp",map );        
		        return Response.ok(view).build();		
			}			
		}
		
		String activeName = "";		
		List<AreaDepth1> depth1s = locationManager.getAreaForNode();
		if( depth1s != null && depth1s.size() > 0)
		{
			map.put("UseArea", depth1s);			
			activeName = depth1s.get(0).getDetph();	
			map.put("Active", activeName);
			List<Site> areaLoc = locationManager.getLocationForAreaDepth1(activeName);
			map.put("LocationList", areaLoc);
		}
		
        map.put("user", "usul");   
        Viewable view = new Viewable("/admin/admin_manage_main.jsp",map );        
        return Response.ok(view).build();	
       
	}
	
	@POST
	@Path("/manage/modify/{id}")	
	@Produces("text/html; charset=UTF-8")
	public Response adminManageEditNode(
	@DefaultValue("-1") @PathParam("id") int nID,
						@FormParam("Name") String name,
						@FormParam("Phone") String phone,
						@FormParam("Address") String address,
						@FormParam("DetailAddress") String detailAddress,
						@FormParam("Description") String description,
						@FormParam("attach_link") List<Integer> links	
						)
	{	
		
		if(!checkLogin())
			return requestLogin(false);

		Map<String, Object> map = new LinkedHashMap<String, Object>();
		if( nID >= 0)
		{
			Site loc = locationManager.getLocation(nID);
			
			float x = 0.0f;
			float y = 0.0f;
			
			// Get Geo Location
			try
			{
				String szLocation = getGeoCodingLocation(address);
				StringTokenizer token = new StringTokenizer(szLocation, ",", false);
				String szX = token.nextToken();
				String szY = token.nextToken();
				
				x = Float.parseFloat(szX);
				y = Float.parseFloat(szY);
				logger.debug("Geocoding Result: " + szLocation);
			}
			catch(Exception ex)
			{	
				logger.debug("GeoCoding Error : ", ex);
				x = 0.0f;
				y = 0.0f;
			}	
			
			int nAreaID = -1;
			String szDepth1 = null, szDepth2 = null, szDepth3 = null, szDepth4 = null;
			String[] splited = address.split("\\s+");
			if(splited != null)
			{
				if(splited.length > 0)
				{
					szDepth1 = splited[0];
					szDepth2 = null;
					szDepth3 = null;
					szDepth4 = null;
					if(splited.length > 1)
					{
						szDepth1 = splited[0];
						szDepth2 = splited[1];
						szDepth3 = null;
						szDepth4 = null;
						if(splited.length > 2)
						{
							szDepth1 = splited[0];
							szDepth2 = splited[1];
							szDepth3 = splited[2];
							szDepth4 = null;
							if(splited.length > 3)
							{
								szDepth1 = splited[0];
								szDepth2 = splited[1];
								szDepth3 = splited[2];
								szDepth4 = splited[3];
							}
						}
					}
				}
				
				try {
					nAreaID = areaManager.getAreaID(szDepth1, szDepth2, szDepth3, szDepth4);
				} catch (Exception e) {
					logger.debug("GetAreaID Exception", e);
				}
				// Add Location
				if(nAreaID > 0)
				{				
					loc.setName(name);
					loc.setPhone(phone);
					loc.setAddress(address);
					loc.setDetailAddress(detailAddress);
					loc.setArea(nAreaID);
					loc.setDescription(description);
					loc.setLocationX(x);;
					loc.setLocationY(y);
					locationManager.updateLocation(loc);
				}
				
				if( links!= null)
				{
					for(Integer linkID : links)
					{
						int link = linkID.intValue();
						NodeLocation node = nodeManager.getNodeLocationByNodeID(link);
						node.setLocationID(loc.getID());
						nodeManager.updateNodeLocation(node);
					}
				}
			}			
			map.put("targetLocation", loc);
			
			List<NodeLocation> nodeList = nodeManager.getNodeLocation(loc.getID());
			map.put("linkNodes", nodeList);	
		}
		
        map.put("user", "usul");   
        Viewable view = new Viewable("/admin/admin_manage_detail.jsp",map );        
        return Response.ok(view).build();		
	}
	

	@GET
	@Path("/manage/modify/{id}")	
	@Produces("text/html; charset=UTF-8")
	public Response adminManageEditNode(@DefaultValue("-1") @PathParam("id") int nID)
	{	
		
		if(!checkLogin())
			return requestLogin(false);

		Map<String, Object> map = new LinkedHashMap<String, Object>();
		if( nID >= 0)
		{
			Site loc = locationManager.getLocation(nID);
			map.put("targetLocation", loc);  
			
			List<NodeLocation> nodeList = nodeManager.getNodeLocation(nID);
			map.put("linkNodes", nodeList);			
		}
		
        map.put("user", "usul");   
        Viewable view = new Viewable("/admin/admin_manage_edit.jsp",map );        
        return Response.ok(view).build();		
	}
	
	@POST
	@Path("/manage/delete/{id}")	
	@Produces("text/html; charset=UTF-8")
	public Response adminManageDeleteNode(@DefaultValue("-1") @PathParam("id") int nID)
	{
		if(!checkLogin())
			return requestLogin(false);

		if( nID >= 0)
		{
			List<NodeLocation> nodeList = nodeManager.getNodeLocation(nID);
			for(NodeLocation node : nodeList)
			{
				int nMapID = node.getMapImage();				
				nodeManager.deleteNodeLocation(node.getID());				
				if(nMapID > 0)
					mapManager.deleteImageMap(nMapID);
			}
			
			locationManager.deleteLocation(nID);
		}
		
		String activeName = "";
		Map<String, Object> map = new LinkedHashMap<String, Object>();
		List<AreaDepth1> depth1s = locationManager.getAreaForNode();
		if( depth1s != null && depth1s.size() > 0)
		{
			map.put("UseArea", depth1s);			
			activeName = depth1s.get(0).getDetph();	
			map.put("Active", activeName);
			List<Site> areaLoc = locationManager.getLocationForAreaDepth1(activeName);
			map.put("LocationList", areaLoc);
		}
		
        map.put("user", "usul");   
        Viewable view = new Viewable("/admin/admin_manage_main.jsp",map );        
        return Response.ok(view).build();		
	}	
	
	private final String USER_AGENT = "Mozilla/5.0";
	private String getGeoCodingLocation(String address) throws Exception {
		if(address == null)
			return "";
					
					
		String sbuURL =  URLEncoder.encode(address, "UTF-8");
		String queryURL = "http://maps.googleapis.com/maps/api/geocode/xml?address=" + sbuURL + "&language=ko&sensor=false";
		
		URL obj = new URL(queryURL);
		HttpURLConnection con = (HttpURLConnection) obj.openConnection();

		con.setRequestMethod("GET");
		con.setRequestProperty("User-Agent", USER_AGENT);
		int responseCode = con.getResponseCode();
		logger.debug("Google Geo Coding Server Return Code : " + responseCode);
		BufferedReader in = new BufferedReader(new InputStreamReader(con.getInputStream()));
		String inputLine;
		
		StringBuffer response = new StringBuffer();
		while ((inputLine = in.readLine()) != null) {
			response.append(inputLine);
		}
		in.close();	
		
		String szString = response.toString();		
		logger.debug("GeoCoding Response : " + szString );
		String szStart = "<location>";
		int nStartIdx = szString.indexOf(szStart) + szStart.length();
		int nEndIdx = szString.indexOf("</location>");
		if( nStartIdx >= 0 && nEndIdx > nStartIdx)
		{
			String szTarget = szString.substring(nStartIdx, nEndIdx);
			if( szTarget != null)
			{
				szTarget.trim();
				szTarget = szTarget.replace("\n\r", "");
				szTarget = szTarget.replace("\n", "");
				szTarget = szTarget.replace("\r", "");
				szTarget = szTarget.replace("\t", "");
				szTarget = szTarget.replace("<lat>", "");
				szTarget = szTarget.replace("</lat>", ",");
				szTarget = szTarget.replace("<lng>", "");
				szTarget = szTarget.replace("</lng>", "");
			}		
			else
			{
				szTarget= "";
			}		
			return szTarget;
		}
		return "";
	}

	@POST
	@Produces("application/json; charset=UTF-8")
	@Path("/nodelink/new")
	public Response adminNodeLinkAdd(@Context HttpHeaders headers,
			MultivaluedMap<String, String> formFields,
			@DefaultValue("-1") @FormParam("SiteID") int nSiteID,
			@FormParam("NodeID") int nodeID,
			@FormParam("NodeAddName") String szName,
			@FormParam("MapID") int fileID)
	{		
		if(!checkLogin())
			return requestLogin();	
		
		String szResultJson = "{\"NodeLinks\":{\"NodeLink\":[{}]}, \"Result\":-1}";
		try
		{
			if(szName != null)
			{
				NodeLocation item = new NodeLocation();
				item.setLocationID(nSiteID);
				item.setNodeID(nodeID);
				item.setMapImage(fileID);
				item.setName(szName);
				nodeManager.addNodeLocation(item);		
				
				NodeLocation target = item;
				if( target != null)
				{
					Map<String, Object> map = new LinkedHashMap<String, Object>();
					map.put("LinkID", "" + target.getID());
					map.put("SiteID", "" + target.getLocationID());
					map.put("NodeID", "" + target.getNodeID());
					
					map.put("MapID",  "" + target.getMapImage());
					
					if(target.getMapImage() > 0)
					{
						mapManager.setImageMapLink(target.getID(), target.getMapImage());
						NodeLinkImageMap img = mapManager.getImageMap(target.getMapImage());
						if( img != null)
						{
							map.put("MapURL",  "" + img.getUrl());						 
							logger.debug("Find NodeLinkImageMap : " + img.getUrl());
						}
						else
						{
							map.put("MapURL",  "");
						}
					}
					else
					{
						map.put("MapURL",  "");
					}
					
					logger.debug("Find NodeLocation : " + target.getID());
					
					Gson gson = new Gson();
					Map<String, Object> result = new LinkedHashMap<String, Object>();
					result.put("NodeLink", map);	
					JsonElement el = gson.toJsonTree(result);		
					JsonObject obj = new JsonObject ();
					obj.add("NodeLinks", el);
					obj.addProperty("Result", 1);
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
	@Produces("application/json; charset=UTF-8")
	@Path("/nodelink/modify/{id}")
	public Response adminNodeLinkWrite(@Context HttpHeaders headers,
			MultivaluedMap<String, String> formFields,
			@FormParam("LinkID") int nLinkID,
			@FormParam("SiteID") int nSiteID,
			@FormParam("NodeID") int nodeID,
			@FormParam("NodeAddName") String szName,
			@FormParam("MapID") int fileID)
	{		
		if(!checkLogin())
			return requestLogin();	
		
		String szResultJson = "{\"NodeLinks\":{\"NodeLink\":[{}]}, \"Result\":-1}";
		try
		{
			if(szName != null)
			{
				NodeLocation target = null;
				
				NodeLocation node = nodeManager.getNodeLocationByNodeID(nLinkID);
				if( node != null)
				{
					mapManager.deleteNodeLinkMap(node.getID(), node.getMapImage());
					node.setNodeID(nodeID);
					node.setLocationID(nSiteID);
					node.setMapImage(fileID);
					node.setName(szName);
					nodeManager.updateNodeLocation(node);
					target = node;
				}
				
				if( target != null)
				{
					Map<String, Object> map = new LinkedHashMap<String, Object>();
					map.put("LinkID", "" + target.getID());
					map.put("SiteID", "" + target.getLocationID());
					map.put("NodeID", "" + target.getNodeID());
					
					map.put("MapID",  "" + target.getMapImage());
					
					if(target.getMapImage() > 0)
					{
						mapManager.setImageMapLink(target.getID(), target.getMapImage());
						NodeLinkImageMap img = mapManager.getImageMap(target.getMapImage());
						if( img != null)
						{
							map.put("MapURL",  "" + img.getUrl()); 
							logger.debug("Find NodeLinkImageMap : " + img.getUrl());
						}
						else
						{
							map.put("MapURL",  "");
						}
					}
					else
					{
						map.put("MapURL",  "");
					}
					
					logger.debug("Find NodeLocation : " + target.getID());
					
					Gson gson = new Gson();
					Map<String, Object> result = new LinkedHashMap<String, Object>();
					result.put("NodeLink", map);	
					JsonElement el = gson.toJsonTree(result);		
					JsonObject obj = new JsonObject ();
					obj.add("NodeLinks", el);
					obj.addProperty("Result", 1);
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
	@Produces("application/json; charset=UTF-8")
	@Path("/nodelink/delete/{id}")
	public Response adminNodeLinkDelete(@Context HttpHeaders headers,
			MultivaluedMap<String, String> formFields,
			@PathParam("id") int nLinkID
			// @FormParam("SiteID") int nSiteID
			)
	{		
		if(!checkLogin())
			return requestLogin();	
		
		String szResultJson = "{\"NodeLinks\":{\"NodeLink\":[{}]}, \"Result\":-1}";
		try
		{
			int nDeleteMapID = -1;
			NodeLocation target = null;
			
			NodeLocation node = nodeManager.getNodeLocationByNodeID(nLinkID);
			if( node != null)
			{
				nDeleteMapID = node.getMapImage();
				// Set Unlink
				mapManager.deleteNodeLinkMap(node.getID(), nDeleteMapID);
				// delete link
				nodeManager.deleteNodeLocation(nLinkID);
				// delete map
				mapManager.deleteImageMap(node.getMapImage());
				
				target = node;
			}
			
			if( target != null)
			{
				Map<String, Object> map = new LinkedHashMap<String, Object>();
				map.put("LinkID", "" + target.getID());
				map.put("SiteID", "" + target.getLocationID());
				map.put("NodeID", "" + target.getNodeID());				
				map.put("MapID",  "" + target.getMapImage());
				map.put("MapURL",  "");			
				
				logger.debug("delete NodeLocation : " + target.getID());
				
				Gson gson = new Gson();
				Map<String, Object> result = new LinkedHashMap<String, Object>();
				result.put("NodeLink", map);	
				JsonElement el = gson.toJsonTree(result);		
				JsonObject obj = new JsonObject ();
				obj.add("NodeLinks", el);
				obj.addProperty("Result", 1);
				szResultJson = gson.toJson(obj);
			}
					
		}
		catch(Exception ex)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	@GET
	@Produces("application/json; charset=UTF-8")
	@Path("/nodelink/detail/{id}")
	public Response adminNodeLinkRead(@PathParam("id") int linkID)
	{		
		if(!checkLogin())
			return requestLogin();
		
		String szResultJson = "{\"NodeLinks\":{\"NodeLink\":[{}]}, \"Result\":-1}";
		try
		{			
			NodeLocation node = nodeManager.getNodeLocationByNodeID(linkID);
			if( node != null)
			{
				Map<String, Object> map = new LinkedHashMap<String, Object>();
				map.put("LinkID", "" + node.getID());
				map.put("SiteID", "" + node.getLocationID());
				map.put("NodeID", "" + node.getNodeID());
				
				map.put("MapID",  "" + node.getMapImage());
				
				if(node.getMapImage() > 0)
				{
					 NodeLinkImageMap img = mapManager.getImageMap(node.getMapImage());
					 if( img != null)
					 {
						 map.put("MapURL",  "" + img.getUrl());
						 
						 logger.debug("Find NodeLinkImageMap : " + img.getUrl());
					 }
					 else
					 {
						 map.put("MapURL",  "");
					 }
				}
				else
				{
					map.put("MapURL",  "");
				}
				
				logger.debug("Find NodeLocation : " + linkID);
				
				Gson gson = new Gson();
				Map<String, Object> result = new LinkedHashMap<String, Object>();
				result.put("NodeLink", map);	
				JsonElement el = gson.toJsonTree(result);		
				JsonObject obj = new JsonObject ();
				obj.add("NodeLinks", el);
				obj.addProperty("Result", 1);
				szResultJson = gson.toJson(obj);	
			}
		}
		catch(Exception ex)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
}
