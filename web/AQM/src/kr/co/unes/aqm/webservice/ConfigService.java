package kr.co.unes.aqm.webservice;

import java.util.ArrayList;
import java.util.HashMap;
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

import kr.co.unes.aqm.dto.SensorGroup;
import kr.co.unes.aqm.dto.sensor.SensorCode;
import kr.co.unes.aqm.model.ConfigDataAccessManager;

import com.google.gson.Gson;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;

@Path("/Config")
public class ConfigService 
{	
	private ConfigDataAccessManager mConfigManager = new ConfigDataAccessManager();
	
	@GET
	@Path("/sensorGroups")
	@Produces("application/json; charset=UTF-8")
	public Response getSensorGroups()
	{
		String szResultJson = "{\"Config\":{\"group\":-1}, \"Result\":-1}";
		try
		{
			ArrayList<SensorGroup> groups = mConfigManager.getSensorGroups();
			if(groups != null && groups.size() > 0)
			{
				Gson gson = new Gson();
				Map<String, Object> map = new HashMap<String, Object>();
				map.put("group", groups);	
				JsonElement el = gson.toJsonTree(map);	
				
				JsonObject obj1 = new JsonObject ();
				obj1.add("Config", el);
				obj1.addProperty("Result", 0);
				
				szResultJson = gson.toJson(obj1);
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	@GET
	@Path("/sensorCodes")
	@Produces("application/json; charset=UTF-8")
	public Response getSensorCodes()
	{
		String szResultJson = "{\"Config\":{\"sensors\":-1}, \"Result\":-1}";
		try
		{
			ArrayList<SensorCode> groups = mConfigManager.getSensorCodes();
			if(groups != null && groups.size() > 0)
			{
				Gson gson = new Gson();
				Map<String, Object> map = new HashMap<String, Object>();
				map.put("sensors", groups);	
				JsonElement el = gson.toJsonTree(map);	
				
				JsonObject obj1 = new JsonObject ();
				obj1.add("Config", el);
				obj1.addProperty("Result", groups.size());
				
				szResultJson = gson.toJson(obj1);
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	
	
	@POST
	@Path("/sensor/new")
	@Produces("application/json; charset=UTF-8")
	public Response addSensorCode(@FormParam("SensorName") String szSensorName,
								  @FormParam("GroupID") int nGroupID,
				                  @FormParam("SensorCode") int nSensorCode,
				                  @FormParam("LimitNotice") float fLimitNotice,
				                  @FormParam("LimitAttention") float fLimitAttention,
				                  @FormParam("LimitWarning") float fLimitWarning,
				                  @FormParam("LimitValueLaw") float fLimitValueLaw,
				                  @FormParam("SensorUnit") String SensorUnit,
				                  @DefaultValue("-1") @FormParam("LimitType") int nLimitType,
				                  @DefaultValue("-1") @FormParam("LimitNoticeBegin") float fLimitNoticeBegin,
				                  @DefaultValue("-1") @FormParam("LimitNoticeEnd") float fLimitNoticeEnd,
				                  @DefaultValue("-1") @FormParam("LimitAttentionBegin") float fLimitAttentionBegin,
				                  @DefaultValue("-1") @FormParam("LimitAttentionEnd") float fLimitAttentionEnd,
				                  @DefaultValue("-1") @FormParam("LimitWarningBegin") float fLimitWarningBegin,
				                  @DefaultValue("-1") @FormParam("LimitWarningEnd") float fLimitWarningEnd,
				                  @DefaultValue("-1") @FormParam("LimitValueLawBegin") float fLimitValueLawBegin,
				                  @DefaultValue("-1") @FormParam("LimitValueLawEnd") float fLimitValueLawEnd,			                  
				                  @FormParam("Remark") String szRemark)
	{
		String szResultJson = "{\"Config\":{\"id\":-1}, \"Result\":-1}";
		try
		{
			SensorCode sensorCode = new SensorCode();
			sensorCode.setName(szSensorName);
			sensorCode.setID(nSensorCode);
			sensorCode.setGroupID(nGroupID);
			sensorCode.setSensorUnit(SensorUnit);
			sensorCode.setLimitType(nLimitType);
			sensorCode.setRemark(szRemark);
						
			sensorCode.setLimitAttention(fLimitAttention);
			sensorCode.setLimitAttentionBegin(fLimitAttentionBegin);
			sensorCode.setLimitAttentionEnd(fLimitAttentionEnd);
			
			sensorCode.setLimitNotice(fLimitNotice);
			sensorCode.setLimitNoticeBegin(fLimitNoticeBegin);
			sensorCode.setLimitNoticeEnd(fLimitNoticeEnd);
			
			sensorCode.setLimitWarning(fLimitWarning);
			sensorCode.setLimitWarningBegin(fLimitWarningBegin);
			sensorCode.setLimitWarningEnd(fLimitWarningEnd);
						
			sensorCode.setLimitValueLaw(fLimitValueLaw);
			sensorCode.setLimitValueLawBegin(fLimitValueLawBegin);
			sensorCode.setLimitValueLawEnd(fLimitValueLawEnd);
			
			
//			
//			int nSensorID = new ConfigDataAccessManager().addSensorCode(
//					szSensorName, nSensorCode, nGroupID, fLimitNotice, fLimitAttention, 
//					fLimitWarning, fLimitValueLaw, SensorUnit,nLimitType,
//					fLimitNoticeBegin,fLimitNoticeEnd,fLimitAttentionBegin,
//					fLimitAttentionEnd,fLimitWarningBegin,fLimitWarningEnd,
//					fLimitValueLawBegin,fLimitValueLawEnd, szRemark);
//			
			if(sensorCode.getID() > 0)
			{
				mConfigManager.addSensorCode(sensorCode);
				
				szResultJson = getIntJson("Config", "id", sensorCode.getID());
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}	
	
	@GET
	@Path("/groupId/{sensorCode}")
	@Produces("application/json; charset=UTF-8")
	public Response getSensorCodeGroupID(@PathParam("sensorCode") int nSensorCode)
	{
		String szResultJson = "{\"Config\":{\"groupId\":-1}, \"Result\":-1}";
		try
		{
			int nGroupID = mConfigManager.getIntValue("GroupID", nSensorCode);
			if(nGroupID > 0)
			{				
				szResultJson = getIntJson("Config", "groupID", nGroupID);				
			}			
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}	
	
	@POST
	@Path("/groupId/{sensorCode}")
	@Produces("application/json; charset=UTF-8")
	public Response setSensorCodeGroupID(@PathParam("sensorCode") int nSensorCode, @FormParam("GroupID") int nGroupID)
	{
		String szResultJson = "{\"Config\":{\"groupId\":-1}, \"Result\":-1}";
		try
		{
			boolean bResult = mConfigManager.setIntValue(nSensorCode,"GroupID", nGroupID);
			if(bResult == true)
			{
				szResultJson = getIntJson("Config", "groupId", nGroupID);					
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}	
		
	@GET
	@Path("/limitType/{sensorCode}")
	@Produces("application/json; charset=UTF-8")
	public Response getLimitType(@PathParam("sensorCode") int nSensorCode)
	{
		String szResultJson = "{\"Config\":{\"limitType\":-1}, \"Result\":-1}";
		try
		{
			int nLimit = mConfigManager.getIntValue("LimitType", nSensorCode);
			if(nLimit > 0)
			{				
				szResultJson = getIntJson("Config", "limitType", nLimit);				
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}	
	
	@POST
	@Path("/limitType/{sensorCode}")
	@Produces("application/json; charset=UTF-8")
	public Response setLimitType(@PathParam("sensorCode") int nSensorCode, @FormParam("LimitType") int nLimit)
	{
		String szResultJson = "{\"Config\":{\"limitType\":-1}, \"Result\":-1}";
		try
		{
			boolean bResult = mConfigManager.setIntValue(nSensorCode, "LimitType", nLimit);
			if(bResult == true)
			{
				szResultJson = getIntJson("Config", "limitType", 0);				
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}	
	
	private String getIntJson(String szMain, String szField, int value)
	{
		JsonObject obj1 = new JsonObject ();
		obj1.addProperty(szField, value);
		
		JsonObject obj = new JsonObject ();
		obj.add(szMain, obj1);
		obj.addProperty("Result", 0);			

		Gson gson = new Gson();
		return gson.toJson(obj);
	}
	
	private String getDobuelJson(String szMain, String szField, double value)
	{
		JsonObject obj1 = new JsonObject ();
		obj1.addProperty(szField, value);
		
		JsonObject obj = new JsonObject ();
		obj.add(szMain, obj1);
		obj.addProperty("Result", 0);			

		Gson gson = new Gson();
		return gson.toJson(obj);
	}
	
	@GET
	@Path("/limitNotice/{sensorCode}")
	@Produces("application/json; charset=UTF-8")
	public Response getSensorLimitNotice(@PathParam("sensorCode") int nSensorID)
	{
		String szResultJson = "{\"Config\":{\"limitNotice\":-1}, \"Result\":-1}";
		try
		{
			double dValue = mConfigManager.getDoubleValue("LimitNotice", nSensorID);
			if(dValue > 0)
			{			
				szResultJson = getDobuelJson("Config","limitNotice", dValue );
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	@POST
	@Path("/limitNotice/{sensorCode}")
	@Produces("application/json; charset=UTF-8")
	public Response setSensorLimitNotice(@PathParam("sensorCode") int nSensorID, @FormParam("LimitNotice") double dLimit)
	{
		String szResultJson = "{\"Config\":{\"limitNotice\":-1}, \"Result\":-1}";
		try
		{
			boolean bResult = mConfigManager.setDoubleValue(nSensorID, "LimitNotice",dLimit);
			if(bResult == true)
			{			
				szResultJson = getIntJson("Config","limitNotice", 0 );
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	@GET
	@Path("/limitAttention/{sensorCode}")
	@Produces("application/json; charset=UTF-8")
	public Response getSensorLimitAttention(@PathParam("sensorCode") int nSensorID)
	{
		String szResultJson = "{\"Config\":{\"limitAttention\":-1}, \"Result\":-1}";
		try
		{
			double dValue = mConfigManager.getDoubleValue("LimitAttention", nSensorID);
			if(dValue > 0)
			{
				szResultJson = getDobuelJson("Config","limitAttention", dValue );
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	@POST
	@Path("/limitAttention/{sensorCode}")
	@Produces("application/json; charset=UTF-8")
	public Response setSensorLimitAttention(@PathParam("sensorCode") int nSensorID, @FormParam("LimitAttention") double dLimit)
	{
		String szResultJson = "{\"Config\":{\"limitAttention\":-1}, \"Result\":-1}";
		try
		{
			boolean dValue = mConfigManager.setDoubleValue(nSensorID, "LimitAttention", dLimit);
			if(dValue == true)
			{
				szResultJson = getIntJson("Config","limitAttention", 0 );
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	@GET
	@Path("/limitWarning/{sensorCode}")
	@Produces("application/json; charset=UTF-8")
	public Response getSensorLimitWarning(@PathParam("sensorCode") int nSensorID)
	{
		String szResultJson = "{\"Config\":{\"limitWarning\":-1}, \"Result\":-1}";
		try
		{
			double dValue = mConfigManager.getDoubleValue("LimitWarning", nSensorID);
			if(dValue > 0)
			{
				szResultJson = getDobuelJson("Config","limitWarning", dValue );
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	@POST
	@Path("/limitWarning/{sensorCode}")
	@Produces("application/json; charset=UTF-8")
	public Response setSensorLimitWarning(@PathParam("sensorCode") int nSensorID, @FormParam("LimitWarning") double dLimit)
	{
		String szResultJson = "{\"Config\":{\"limitWarning\":-1}, \"Result\":-1}";
		try
		{
			boolean dValue = mConfigManager.setDoubleValue(nSensorID, "LimitWarning", dLimit);
			if(dValue == true)
			{
				szResultJson = getIntJson("Config","limitWarning", 0 );
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	@GET
	@Path("/limitValueLaw/{sensorCode}")
	@Produces("application/json; charset=UTF-8")
	public Response getSensorLimitValueLaw(@PathParam("sensorCode") int nSensorID)
	{
		String szResultJson = "{\"Config\":{\"limitValueLaw\":-1}, \"Result\":-1}";
		try
		{
			double dValue = mConfigManager.getDoubleValue("LimitValueLaw", nSensorID);
			if(dValue > 0)
			{
				szResultJson = getDobuelJson("Config","limitValueLaw", dValue );
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	

	@POST
	@Path("/limitValueLaw/{sensorCode}")
	@Produces("application/json; charset=UTF-8")
	public Response setSensorLimitValueLaw(@PathParam("sensorCode") int nSensorID, @FormParam("LimitValueLaw") double dLimit)
	{
		String szResultJson = "{\"Config\":{\"limitValueLaw\":-1}, \"Result\":-1}";
		try
		{
			boolean dValue = mConfigManager.setDoubleValue(nSensorID, "LimitValueLaw", dLimit);
			if(dValue == true)
			{
				szResultJson = getIntJson("Config","limitValueLaw", 0 );
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	
	
	
	@GET
	@Path("/limitNoticeBegin/{sensorCode}")
	@Produces("application/json; charset=UTF-8")
	public Response getSensorLimitNoticeBegin(@PathParam("sensorCode") int nSensorID)
	{
		String szResultJson = "{\"Config\":{\"limitNoticeBegin\":-1}, \"Result\":-1}";
		try
		{
			double dValue = mConfigManager.getDoubleValue("LimitNoticeBegin", nSensorID);
			if(dValue > 0)
			{
				szResultJson = getDobuelJson("Config","limitNoticeBegin", dValue );
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	@POST
	@Path("/limitNoticeBegin/{sensorCode}")
	@Produces("application/json; charset=UTF-8")
	public Response setSensorLimitNoticeBegin(@PathParam("sensorCode") int nSensorID, @FormParam("LimitNoticeBegin") double dLimit)
	{
		String szResultJson = "{\"Config\":{\"limitNoticeBegin\":-1}, \"Result\":-1}";
		try
		{
			boolean dValue = mConfigManager.setDoubleValue(nSensorID, "LimitNoticeBegin", dLimit);
			if(dValue == true)
			{
				szResultJson = getIntJson("Config","limitNoticeBegin", 0 );
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	
	@GET
	@Path("/limitNoticeEnd/{sensorCode}")
	@Produces("application/json; charset=UTF-8")
	public Response getSensorLimitNoticeEnd(@PathParam("sensorCode") int nSensorID)
	{
		String szResultJson = "{\"Config\":{\"limitNoticeEnd\":-1}, \"Result\":-1}";
		try
		{
			double dValue = mConfigManager.getDoubleValue("LimitNoticeEnd", nSensorID);
			if(dValue > 0)
			{
				szResultJson = getDobuelJson("Config","limitNoticeEnd", dValue );
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	@POST
	@Path("/limitNoticeEnd/{sensorCode}")
	@Produces("application/json; charset=UTF-8")
	public Response setSensorLimitNoticeEnd(@PathParam("sensorCode") int nSensorID, @FormParam("LimitNoticeEnd") double dLimit)
	{
		String szResultJson = "{\"Config\":{\"limitNoticeBegin\":-1}, \"Result\":-1}";
		try
		{
			boolean dValue = mConfigManager.setDoubleValue(nSensorID, "LimitNoticeEnd", dLimit);
			if(dValue == true)
			{
				szResultJson = getIntJson("Config","limitNoticeBegin", 0 );
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	
	@GET
	@Path("/limitAttentionBegin/{sensorCode}")
	@Produces("application/json; charset=UTF-8")
	public Response getSensorLimitAttentionBegin(@PathParam("sensorCode") int nSensorID)
	{
		String szResultJson = "{\"Config\":{\"limitAttentionBegin\":-1}, \"Result\":-1}";
		try
		{
			double dValue = mConfigManager.getDoubleValue("LimitAttentionBegin", nSensorID);
			if(dValue > 0)
			{
				szResultJson = getDobuelJson("Config","limitAttentionBegin", dValue );
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	@POST
	@Path("/limitAttentionBegin/{sensorCode}")
	@Produces("application/json; charset=UTF-8")
	public Response setSensorLimitAttentionBegin(@PathParam("sensorCode") int nSensorID, @FormParam("LimitAttentionBegin") double dLimit)
	{
		String szResultJson = "{\"Config\":{\"limitAttentionBegin\":-1}, \"Result\":-1}";
		try
		{
			boolean dValue = mConfigManager.setDoubleValue(nSensorID, "LimitAttentionBegin", dLimit);
			if(dValue == true)
			{
				szResultJson = getIntJson("Config","limitAttentionBegin", 0 );
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	@GET
	@Path("/limitAttentionEnd/{sensorCode}")
	@Produces("application/json; charset=UTF-8")
	public Response getSensorLimitAttentionEnd(@PathParam("sensorCode") int nSensorID)
	{
		String szResultJson = "{\"Config\":{\"limitAttentionEnd\":-1}, \"Result\":-1}";
		try
		{
			double dValue = mConfigManager.getDoubleValue("LimitAttentionEnd", nSensorID);
			if(dValue > 0)
			{
				szResultJson = getDobuelJson("Config","limitAttentionEnd", dValue );
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	@POST
	@Path("/limitAttentionEnd/{sensorCode}")
	@Produces("application/json; charset=UTF-8")
	public Response setSensorLimitAttentionEnd(@PathParam("sensorCode") int nSensorID, @FormParam("LimitAttentionEnd") double dLimit)
	{
		String szResultJson = "{\"Config\":{\"limitAttentionEnd\":-1}, \"Result\":-1}";
		try
		{
			boolean dValue = mConfigManager.setDoubleValue(nSensorID, "LimitAttentionEnd", dLimit);
			if(dValue == true)
			{
				szResultJson = getIntJson("Config","limitAttentionEnd", 0 );
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	@GET
	@Path("/limitWarningBegin/{sensorCode}")
	@Produces("application/json; charset=UTF-8")
	public Response getSensorLimitWarningBegin(@PathParam("sensorCode") int nSensorID)
	{
		String szResultJson = "{\"Config\":{\"limitWarningBegin\":-1}, \"Result\":-1}";
		try
		{
			double dValue = mConfigManager.getDoubleValue("LimitWarningBegin", nSensorID);
			if(dValue > 0)
			{
				szResultJson = getDobuelJson("Config","limitWarningBegin", dValue );
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	@POST
	@Path("/limitWarningBegin/{sensorCode}")
	@Produces("application/json; charset=UTF-8")
	public Response setSensorLimitWarningBegin(@PathParam("sensorCode") int nSensorID, @FormParam("LimitWarningBegin") double dLimit)
	{
		String szResultJson = "{\"Config\":{\"limitWarningBegin\":-1}, \"Result\":-1}";
		try
		{
			boolean dValue = mConfigManager.setDoubleValue(nSensorID, "LimitWarningBegin", dLimit);
			if(dValue == true)
			{
				szResultJson = getIntJson("Config","limitWarningBegin", 0 );
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	@GET
	@Path("/limitWarningEnd/{sensorCode}")
	@Produces("application/json; charset=UTF-8")
	public Response getSensorLimitWarningEnd(@PathParam("sensorCode") int nSensorID)
	{
		String szResultJson = "{\"Config\":{\"limitWarningEnd\":-1}, \"Result\":-1}";
		try
		{
			double dValue = mConfigManager.getDoubleValue("LimitWarningEnd", nSensorID);
			if(dValue > 0)
			{
				szResultJson = getDobuelJson("Config","limitWarningEnd", dValue );
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	@POST
	@Path("/limitWarningEnd/{sensorCode}")
	@Produces("application/json; charset=UTF-8")
	public Response setSensorLimitWarningEnd(@PathParam("sensorCode") int nSensorID, @FormParam("LimitWarningEnd") double dLimit)
	{
		String szResultJson = "{\"Config\":{\"limitWarningEnd\":-1}, \"Result\":-1}";
		try
		{
			boolean dValue = mConfigManager.setDoubleValue(nSensorID, "LimitWarningEnd", dLimit);
			if(dValue == true)
			{
				szResultJson = getIntJson("Config","limitWarningEnd", 0 );
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	@GET
	@Path("/limitValueLawBegin/{sensorCode}")
	@Produces("application/json; charset=UTF-8")
	public Response getSensorLimitValueLawBegin(@PathParam("sensorCode") int nSensorID)
	{
		String szResultJson = "{\"Config\":{\"limitValueLawBegin\":-1}, \"Result\":-1}";
		try
		{
			double dValue = mConfigManager.getDoubleValue("LimitValueLawBegin", nSensorID);
			if(dValue > 0)
			{
				szResultJson = getDobuelJson("Config","limitValueLawBegin", dValue );
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}

	@POST
	@Path("/limitValueLawBegin/{sensorCode}")
	@Produces("application/json; charset=UTF-8")
	public Response setSensorLimitValueLawBegin(@PathParam("sensorCode") int nSensorID, @FormParam("LimitValueLawBegin") double dLimit)
	{
		String szResultJson = "{\"Config\":{\"limitValueLawBegin\":-1}, \"Result\":-1}";
		try
		{
			boolean dValue = mConfigManager.setDoubleValue(nSensorID, "LimitValueLawBegin", dLimit);
			if(dValue == true)
			{
				szResultJson = getIntJson("Config","limitValueLawBegin", 0 );
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	@GET
	@Path("/limitValueLawEnd/{sensorCode}")
	@Produces("application/json; charset=UTF-8")
	public Response getSensorLimitValueLawEnd(@PathParam("sensorCode") int nSensorID)
	{
		String szResultJson = "{\"Config\":{\"limitValueLawEnd\":-1}, \"Result\":-1}";
		try
		{
			double dValue = mConfigManager.getDoubleValue("LimitValueLawEnd", nSensorID);
			if(dValue > 0)
			{
				szResultJson = getDobuelJson("Config","limitValueLawEnd", dValue );
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
	
	@POST
	@Path("/limitValueLawEnd/{sensorCode}")
	@Produces("application/json; charset=UTF-8")
	public Response setSensorLimitValueLawEnd(@PathParam("sensorCode") int nSensorID, @FormParam("LimitValueLawEnd") double dLimit)
	{
		String szResultJson = "{\"Config\":{\"limitValueLawEnd\":-1}, \"Result\":-1}";
		try
		{
			boolean dValue = mConfigManager.setDoubleValue(nSensorID, "LimitValueLawEnd", dLimit);
			if(dValue == true)
			{
				szResultJson = getIntJson("Config","limitValueLawEnd", 0 );
			}
		} catch (Exception e)
		{
			return Response.status(Status.NOT_FOUND).entity(szResultJson).build();
		}		
		return Response.status(Response.Status.OK).entity(szResultJson).build();
	}
}
