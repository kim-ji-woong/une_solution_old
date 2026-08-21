package kr.co.unes.aqm.webservice;

import javax.ws.rs.DefaultValue;
import javax.ws.rs.FormParam;
import javax.ws.rs.GET;
import javax.ws.rs.POST;
import javax.ws.rs.Path;
import javax.ws.rs.PathParam;
import javax.ws.rs.Produces;
import javax.ws.rs.core.Response;
import javax.ws.rs.core.Response.Status;

import kr.co.unes.aqm.model.NodeDataAccessManager;

import com.google.gson.Gson;
import com.google.gson.JsonObject;


@Path("Node")
public class NodeService {

	@GET
	@Path("/areaID/{nodeID}")
	@Produces("application/json; charset=UTF-8")
	public Response getNodeAreaID(@PathParam("nodeID") int nNodeID)
	{
		String szResultJson = "{\"Node\":{\"area\":-1}, \"Result\":-1}";
		try
		{
			int nID = new NodeDataAccessManager().getNodeAreaID(nNodeID);			
								
			JsonObject obj1 = new JsonObject ();
			obj1.addProperty("area", nID);
			
			JsonObject obj = new JsonObject ();
			obj.add("Node", obj1);
			obj.addProperty("Result", (nID >= 0 ? nID : -1));			

			Gson gson = new Gson();
			szResultJson = gson.toJson(obj);
			
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	@GET
	@Path("/enabled/{nodeID}")
	@Produces("application/json; charset=UTF-8")
	public Response getNodeEnabled(@PathParam("nodeID") int nNodeID)
	{
		String szResultJson = "{\"Node\":{\"enabled\":-1}, \"Result\":-1}";
		try
		{
			int bEnabled = new NodeDataAccessManager().getNodeEanbled(nNodeID);			
			if( bEnabled > 0)
			{
				JsonObject obj1 = new JsonObject ();
				obj1.addProperty("enabled", (bEnabled > 0 ? "True" : "False"));
				
				JsonObject obj = new JsonObject ();
				obj.add("Node", obj1);
				obj.addProperty("Result", 0);			

				Gson gson = new Gson();
				szResultJson = gson.toJson(obj);
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	@POST
	@Path("/enable/{nodeID}")
	@Produces("application/json; charset=UTF-8")
	public Response setNodeEnabled(@PathParam("nodeID") int nNodeID, @FormParam("NodeID") boolean bEnabled )
	{
		String szResultJson = "{\"Node\":{\"enable\":-1}, \"Result\":-1}";
		try
		{
			boolean bResult = new NodeDataAccessManager().setNodeEanbled(nNodeID, bEnabled);			
			if( bResult == true)
			{
				JsonObject obj1 = new JsonObject ();
				obj1.addProperty("enable", bEnabled);
				
				JsonObject obj = new JsonObject ();
				obj.add("Node", obj1);
				obj.addProperty("Result", 0);			

				Gson gson = new Gson();
				szResultJson = gson.toJson(obj);
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	
	@GET
	@Path("/name/{nodeID}")
	@Produces("application/json; charset=UTF-8")
	public Response getNodeName(@PathParam("nodeID") int nNodeID)
	{
		String szResultJson = "{\"Node\":{\"name\":\"\"}, \"Result\":-1}";
		try
		{
			String szName = new NodeDataAccessManager().getNodeName(nNodeID);
			if( szName != null)
			{
				JsonObject obj1 = new JsonObject ();
				obj1.addProperty("name", szName);
				
				JsonObject obj = new JsonObject ();
				obj.add("Node", obj1);
				obj.addProperty("Result", szName.length());			

				Gson gson = new Gson();
				szResultJson = gson.toJson(obj);
			}		
		} 
		catch (Exception e)
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
		String szResultJson = "{\"Node\":{\"id\":-1}, \"Result\":-1}";
		try
		{
			boolean bResult = new NodeDataAccessManager().addNetNode(nNodeID, strNodeName, fNodePosX, fNodePosY, nAreaID, szMaterialCode);
			if( bResult == true)
			{
				JsonObject obj1 = new JsonObject ();
				obj1.addProperty("id", nNodeID);
				
				JsonObject obj = new JsonObject ();
				obj.add("Node", obj1);
				obj.addProperty("Result", nNodeID);			

				Gson gson = new Gson();
				szResultJson = gson.toJson(obj);
			}		
		} 
		catch (Exception e)
		{
			e.printStackTrace();
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}	
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	@POST
	@Path("/remove/{nodeID}")
	@Produces("application/json; charset=UTF-8")	
	public Response removeNode(@PathParam("nodeID") int nNodeID)
	{
		String szResultJson = "{\"Node\":{\"id\":-1}, \"Result\":-1}";
		try
		{
			boolean bResult = new NodeDataAccessManager().removeNode(nNodeID);
			if( bResult == true)
			{
				JsonObject obj1 = new JsonObject ();
				obj1.addProperty("id", nNodeID);
				
				JsonObject obj = new JsonObject ();
				obj.add("Node", obj1);
				obj.addProperty("Result", nNodeID);			

				Gson gson = new Gson();
				szResultJson = gson.toJson(obj);
			}		
		} 
		catch (Exception e)
		{
			e.printStackTrace();
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}	
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}

}
