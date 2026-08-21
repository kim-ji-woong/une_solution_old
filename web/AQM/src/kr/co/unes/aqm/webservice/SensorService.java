package kr.co.unes.aqm.webservice;

import kr.co.unes.aqm.dto.*;
import kr.co.unes.aqm.model.SensorDataAccessManager;

import java.util.HashMap;
import java.util.Map;

import java.util.ArrayList;
import javax.ws.rs.DefaultValue;
import javax.ws.rs.FormParam;
import javax.ws.rs.GET;
import javax.ws.rs.POST;
import javax.ws.rs.Path;
import javax.ws.rs.PathParam;
import javax.ws.rs.Produces; 
import javax.ws.rs.QueryParam;
import javax.ws.rs.core.Response;
import javax.ws.rs.core.Response.Status;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;
 
@Path("/Sensor")
public class SensorService
{	
	
	private SensorDataAccessManager sensorData = new SensorDataAccessManager();

	@GET
	@Path("/values")
	@Produces("application/json; charset=UTF-8")
	public Response getSensorValues()
	{
		String szResultJson = "{\"Sensor\":{\"values\":[{}]}, \"Result\":-1}";
		try
		{
			ArrayList<SensorValueEx> values = sensorData.getAllSensorValue();
			if( values != null && values.size() > 0)
			{
				Gson gson = new GsonBuilder().excludeFieldsWithoutExposeAnnotation().setDateFormat("yyyy-MM-dd HH:mm:ss").create();	
				Map<String, Object> map = new HashMap<String, Object>();
				map.put("values", values);	
				JsonElement el = gson.toJsonTree(map);		
				JsonObject obj = new JsonObject ();
				obj.add("Sensor", el);
				obj.addProperty("Result", values.size());		

				szResultJson = gson.toJson(obj);
			}		
		} catch (Exception e)
		{
			
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}	
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	@POST
	@Path("/value/new")
	@Produces("application/json; charset=UTF-8")
	public Response addSensorValue(
			@DefaultValue("-1") @FormParam("Node") int nNodeID,
			@DefaultValue("-1") @FormParam("SensorCode") int nMaterialCode,
			@FormParam("Value") float fValue,
			@FormParam("ExtraValue") float fExtraValue)
	{
		
		String szResultJson = "{\"Result\":-1}";
		try
		{
			boolean bResult = false;

			if( nNodeID != -1 && nMaterialCode != -1)
			{
				bResult = sensorData.addSensorValue(nNodeID, nMaterialCode, fValue, fExtraValue );
			}
			
			if( bResult == true)
			{			
				JsonObject obj = new JsonObject ();
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
	@Path("/value/new1")
	@Produces("application/json; charset=UTF-8")
	public Response addSensorValue(
			@DefaultValue("-1") @FormParam("Sensor") int nSensorID,
			@FormParam("Value") float fValue,
			@FormParam("ExtraValue") float fExtraValue)
	{
		
		String szResultJson = "{\"Result\":-1}";
		try
		{
			boolean bResult = false;
			bResult = sensorData.addSensorValue(nSensorID, fValue, fExtraValue);
			
			if( bResult == true)
			{			
				JsonObject obj = new JsonObject ();
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
	@Path("/value/{sensorID}")
	@Produces("application/json; charset=UTF-8")
	public Response getSensorValue(@PathParam("sensorID") int nSensorID)
	{
		String szResultJson = "{\"Sensor\":{\"value\":-1}, \"Result\":-1}";
		try
		{
			SensorValue value = sensorData.getSensorValue(nSensorID);
			if( value != null)
			{				
				Gson gson = new GsonBuilder().excludeFieldsWithoutExposeAnnotation().setDateFormat("yyyy-MM-dd HH:mm:ss").create();
				JsonElement el = gson.toJsonTree(value);	
				JsonObject obj1 = new JsonObject ();
				obj1.add("value", el);
				
				JsonObject obj = new JsonObject ();
				obj.add("Sensor", obj1);
				obj.addProperty("Result", 0);	
				szResultJson = gson.toJson(obj);
			}		
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}	
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	@GET
	@Path("/value/{sensorCode}/{nodeID}")
	@Produces("application/json; charset=UTF-8")
	public Response getSensorValue( @PathParam("sensorCode") int nSensorCode, @PathParam("nodeID") int nNodeID)
	{
		String szResultJson = "{\"Sensor\":{\"value\":-1}, \"Result\":-1}";
		try
		{
			SensorValue value = sensorData.getSensorValue(nNodeID, nSensorCode);
			if( value != null)
			{				
				Gson gson = new GsonBuilder().excludeFieldsWithoutExposeAnnotation().setDateFormat("yyyy-MM-dd HH:mm:ss").create();
				JsonElement el = gson.toJsonTree(value);	
				JsonObject obj1 = new JsonObject ();
				obj1.add("value", el);
				
				JsonObject obj = new JsonObject ();
				obj.add("Sensor", obj1);
				obj.addProperty("Result", 0);	
				szResultJson = gson.toJson(obj);
			}		
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}	
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	@GET
	@Path("/values/{sensorID}")
	@Produces("application/json; charset=UTF-8")
	public Response getSensorValues(
			@PathParam("sensorID") int nSensorID, 
			@DefaultValue("0") @QueryParam("MaxQueryCount") int nMaxCount,
			@QueryParam("From") String szFormDate,
			@QueryParam("To") String szToDate)
	{
		String szResultJson = "{\"Sensor\":{\"values\":[{}]}, \"Result\":-1}";
		try
		{			
			ArrayList<SensorValue> values = sensorData.getSensorValues(nSensorID, nMaxCount, szFormDate, szToDate);
			if( values != null && values.size() > 0)
			{
				Gson gson = new GsonBuilder().excludeFieldsWithoutExposeAnnotation().setDateFormat("yyyy-MM-dd HH:mm:ss").create();
				Map<String, Object> map = new HashMap<String, Object>();
				map.put("values", values);	
				JsonElement el = gson.toJsonTree(map);		
				JsonObject obj = new JsonObject ();
				obj.add("Sensor", el);
				obj.addProperty("Result", values.size());		

				szResultJson = gson.toJson(obj);
			}		
		} catch (Exception e)
		{
			e.printStackTrace();
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}	
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	@GET
	@Path("/values/{sensorCode}/{nodeId}")
	@Produces("application/json; charset=UTF-8")
	public Response getSensorValues(
			@PathParam("sensorCode") int nSensorCode, 
			@PathParam("nodeId") int nNodeID, 
			@DefaultValue("0") @QueryParam("MaxQueryCount") int nMaxCount,
			@QueryParam("From") String szFormDate,
			@QueryParam("To") String szToDate)
	{
		String szResultJson = "{\"Sensor\":{\"values\":[{}]}, \"Result\":-1}";
		try
		{
			ArrayList<SensorValue> values = sensorData.getSensorValues(nNodeID, nSensorCode, nMaxCount, szFormDate, szToDate);
			if( values != null && values.size() > 0)
			{
				Gson gson = new GsonBuilder().excludeFieldsWithoutExposeAnnotation().setDateFormat("yyyy-MM-dd HH:mm:ss").create();				
				
				Map<String, Object> map = new HashMap<String, Object>();
				map.put("values", values);	
				JsonElement el = gson.toJsonTree(map);		
				JsonObject obj = new JsonObject ();
				obj.add("Sensor", el);
				obj.addProperty("Result", values.size());		

				szResultJson = gson.toJson(obj);
			}		
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}	
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
}
