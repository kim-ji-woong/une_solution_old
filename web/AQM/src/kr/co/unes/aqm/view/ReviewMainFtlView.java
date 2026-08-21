package kr.co.unes.aqm.view;

import java.sql.Timestamp;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.HashMap;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
import java.util.Set;

import javax.ws.rs.DefaultValue;
import javax.ws.rs.FormParam;
import javax.ws.rs.GET;
import javax.ws.rs.POST;
import javax.ws.rs.Path;
import javax.ws.rs.PathParam;
import javax.ws.rs.Produces;
import javax.ws.rs.core.Response;

import org.apache.ibatis.annotations.Param;
import org.glassfish.jersey.server.mvc.Viewable;
import org.slf4j.LoggerFactory;

import kr.co.unes.aqm.dto.NetNode;
import kr.co.unes.aqm.dto.QualityEvaluation;
import kr.co.unes.aqm.dto.SensorValue;
import kr.co.unes.aqm.dto.SensorValueEx;
import kr.co.unes.aqm.dto.sensor.SensorInfo;
import kr.co.unes.aqm.model.SensorDataAccessManager;
import kr.co.unes.aqm.model.SiteDataAccessManager;


@Path("/ReviewMain")
public class ReviewMainFtlView {

	private SiteDataAccessManager siteManager = new SiteDataAccessManager();
	private SensorDataAccessManager sensorManager = new SensorDataAccessManager();
	
	private final org.slf4j.Logger logger = LoggerFactory.getLogger(ReviewMainFtlView.class);
	
	@GET
	@Produces("text/html; charset=UTF-8")
	@Path("/")
    public Response review() 
	{			
		Map<String, Object> map = new LinkedHashMap<String, Object>();
		map.put("SiteName", "");
		map.put("SiteID", -1);
        Viewable view = new Viewable("/reviewMain.ftl",map );        
        return Response.ok(view).build();
    }
	
	@POST
	@Produces("text/html; charset=UTF-8")
	@Path("/")
    public Response review(@DefaultValue("") @FormParam("SiteName") String strNodeName,
    		@DefaultValue("-1") @FormParam("SiteID") int nSiteID
    		) 
	{			
		
		Map<String, Object> map = new LinkedHashMap<String, Object>();
		map.put("SiteName", strNodeName);
		map.put("SiteID", nSiteID);
        Viewable view = new Viewable("/reviewMain.ftl",map );        
        return Response.ok(view).build();
    }
	
	@POST
	@Produces("text/html; charset=UTF-8")
	@Path("/node/{nodeID}")
	public Response reviewNodeTimeSeries(@PathParam("nodeID") int nNetNodeID,
			@DefaultValue("") @FormParam("SiteName") String strSiteName,
    		@DefaultValue("-1") @FormParam("SiteID") int nSiteID)
	{
		Map<String, Object> map = new LinkedHashMap<String, Object>();
		map.put("NodeID", ""+ nNetNodeID);
		map.put("SiteName", ""+ strSiteName);
		map.put("SiteID", ""+ nSiteID);
				
		NetNode node = siteManager.getNetNode(nNetNodeID);
		if( node != null)
		{			
			int [] sensorCodes = {20992,21248,22274,23040,36864,37120};
			long time = System.currentTimeMillis(); 
			SimpleDateFormat dayTime = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
			Date now = new Date(time);
			
			Calendar cal = Calendar.getInstance();
			cal.setTime(now);
			cal.add(Calendar.HOUR, -24);
			Date toTime= cal.getTime();

			String szToDate = dayTime.format(now);
			String szFormDate = dayTime.format(toTime);
			
			List<List<SensorValue>> dataMap = new ArrayList<List<SensorValue>>();
			HashMap<Integer, QualityEvaluation> qeMap = new HashMap<Integer, QualityEvaluation>();
			
			try {	
				
				HashMap<Integer, HashMap<Integer,SensorValueEx>> resultMap = sensorManager.getTimeSeriesSensorValue(nNetNodeID, sensorCodes, szFormDate, szToDate);

				for(int i = 1; i <= 24 ; i++)
				
				//for(Integer timeIdx : keyset)
				{
					Integer timeIdx = new Integer(i);
					HashMap<Integer,SensorValueEx> valueMap = resultMap.get(timeIdx);				
				
					ArrayList<SensorValue> arList = new ArrayList<SensorValue>();	
					for(int j = 0 ; j < sensorCodes.length ; j++)
					{
						int nSensorCode = sensorCodes[j];
						SensorValue value = null;
						if( valueMap.containsKey(nSensorCode))
						{							
							value = valueMap.get(nSensorCode);
							if( value != null)
							{			
								QualityEvaluation qe = null;
								if(qeMap.containsKey(nSensorCode))
								{
									qe = qeMap.get(nSensorCode);
								}
								else
								{
									QualityEvaluation qeTemp = sensorManager.getQualityEvalTable(nSensorCode);
									qeMap.put(nSensorCode,qeTemp);
									qe = qeTemp;
								}
								
								if( qe != null)
								{
									int nGrade = qe.getQualityEvalution(value.getSensorValue());
									value.setQualityGrade(nGrade);
									float percentValue = qe.getPercentEvaluation(value.getSensorValue());
									value.setPercentValue(percentValue);
								}
								else
								{
									value.setQualityGrade(0);
									value.setPercentValue(0.0f);
								}
							}						
						}
						
						if( value != null)
							arList.add(value);
						else
						{
							SensorValue v = new SensorValue();
							v.setSensorValue(-1.0f);
							v.setQualityGrade(0);
							v.setPercentValue(0.0f);
							arList.add(v);
						}
					}				
					dataMap.add(arList);
				}
				map.put("SensorDataList", dataMap);
				
			} catch (Exception e) {
				logger.debug("LoadSensorValue", e.getMessage());
				logger.debug("LoadSensorValue" , e.getStackTrace()[0]);
			}
		}		
		Viewable view = new Viewable("/sensor/sensorTimeSeries.ftl",map );        
        return Response.ok(view).build();
	}
	
	
	@POST
	@Produces("text/html; charset=UTF-8")
	@Path("/realtime/{nodeID}")
	public Response reviewNodeValue(@PathParam("nodeID") int netNodeID,
			@DefaultValue("") @FormParam("SiteName") String strSiteName,
			@DefaultValue("-1") @FormParam("SiteID") int nSiteID)
	{
		Map<String, Object> map = new LinkedHashMap<String, Object>();
		map.put("NodeID", ""+ netNodeID);
		map.put("SiteName", ""+ strSiteName);
		map.put("SiteID", ""+ nSiteID);		
				
		// netnode 가져오기
		// datatable에서 6가지 센서 정보 읽어오기		// data가 없는 경우 0으로 읽어
		       
		NetNode node = siteManager.getNetNode(netNodeID);
		if( node != null)
		{			
			int [] sensorCodes = {37120,22274,36864,20992,21248,23040};		
		
			ArrayList<SensorValue> arList = new ArrayList<SensorValue>();	
			for(int j = 0 ; j < sensorCodes.length ; j++)
			{
				int nSensorCode = sensorCodes[j];
				SensorValue value = null;
				try {
					value = sensorManager.getSensorValue(netNodeID, nSensorCode);
					if(value != null)
					{
						QualityEvaluation qe = sensorManager.getQualityEvalTable(nSensorCode);
						if( qe != null)
						{
							int nGrade = qe.getQualityEvalution(value.getSensorValue());
							value.setQualityGrade(nGrade);
							float percentValue = qe.getPercentEvaluation(value.getSensorValue());
							value.setPercentValue(percentValue);
						}
						else
						{
							value.setQualityGrade(0);
							value.setPercentValue(0);
						}						
					}
				} catch (Exception e) {
					logger.debug("LoadSensorValue", e.getMessage());
					logger.debug("LoadSensorValue" , e.getStackTrace()[0]);
				}
				if( value != null)
					arList.add(value);
				else
				{
					SensorValue v = new SensorValue();
					v.setSensorValue(-1.0f);
					v.setQualityGrade(0);
					value.setPercentValue(0);
					arList.add(v);
				}
			}
			map.put("SensorDataList", arList);
		}		
		Viewable view = new Viewable("/sensor/sensorValues.ftl",map );        
        return Response.ok(view).build();
	}	
}
