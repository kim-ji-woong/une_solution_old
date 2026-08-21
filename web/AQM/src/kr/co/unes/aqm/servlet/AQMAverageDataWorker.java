package kr.co.unes.aqm.servlet;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.HashMap;
import java.util.List;

import javax.servlet.ServletContext;

import org.slf4j.LoggerFactory;

import kr.co.unes.aqm.dto.SensorValueEx;
import kr.co.unes.aqm.dto.site.NodeLocation;
import kr.co.unes.aqm.dto.site.Site;
import kr.co.unes.aqm.model.NodeLocationDataAccessManager;
import kr.co.unes.aqm.model.SensorDataAccessManager;
import kr.co.unes.aqm.model.SiteDataAccessManager;

public class AQMAverageDataWorker {
	
	private final org.slf4j.Logger logger = LoggerFactory.getLogger(AQMAverageDataWorker.class);
	
	protected int m_timeWork = 60000;
	
	protected Thread mWorkerThread = null;
	
	protected SiteDataAccessManager siteManager = new SiteDataAccessManager();
	protected NodeLocationDataAccessManager nodeManager = new NodeLocationDataAccessManager();
	protected SensorDataAccessManager sensorManager = new SensorDataAccessManager();
		
	private static AQMAverageDataWorker instance = null;
	public static AQMAverageDataWorker getInstance()
	{
		return instance;
	}
	
	public static AQMAverageDataWorker createAQMAverageDataWorker(ServletContext ctx)
	{
		if( instance == null)
			instance = new AQMAverageDataWorker(ctx);
		return instance;
	}
	
	protected ServletContext context = null;
	public AQMAverageDataWorker(ServletContext ctx)
	{
		context = ctx;
		
		Integer sec = (Integer)context.getAttribute("AverageCronTime");
		if(  sec == null)
			m_timeWork = 1800 * 1000;
		else
			m_timeWork = sec * 1000;
	}
	
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
	
	public void cronJob() throws Exception
	{
		logger.debug("AverageCronWorker", "Begin Job");
		
		// 산소, 이산화탄소, TVOC, 미세먼지10, 폼알데히드, 라돈
		int [] sensorCodes = {21760,21248,22784,23040,36864,37120};
		
		int nSec = m_timeWork / 1000;
		
		long time = System.currentTimeMillis(); 
		SimpleDateFormat dayTime = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
		Date now = new Date(time);
		
		Calendar cal = Calendar.getInstance();
		cal.setTime(now);
		cal.add(Calendar.SECOND, -nSec);
		
		Date toTime= cal.getTime();
		String szToDate = dayTime.format(now);
		String szFormDate = dayTime.format(toTime);
		
		// 전체 도시 목록을 가져온다.
		for( int i = 0 ; i < 17 ; i++ )
		{
			
			boolean bUpdateData = false;
			// 도시멸 사이트를 가져온다.
			String cityName = mAreaList[i][0];
			// 사이트별 노드를 가져온다.
			
			float [] averageValue = new float[6];
			float [] totalValue = new float[6];
			int []   sensorCount = new int[6];
			
			List<Site> areaLoc = siteManager.getLocationForAreaDepth1(cityName);			
			for(int k = 0 ; k < areaLoc.size(); k++)
			{
				Site loc = areaLoc.get(k);
				int nSiteID = loc.getID();
				List<NodeLocation> nodeList = nodeManager.getNodeLocation(nSiteID);
				if( nodeList != null)
				{
					
					for(NodeLocation node : nodeList)
					{					
						// 노드의 실기간 데이터를 읽는다.
						HashMap<Integer, SensorValueEx> sensorValues = 
								sensorManager.getSensorValueForAverage(node.getNodeID(), sensorCodes, szFormDate, szToDate);
						
						for(int j = 0 ; j < sensorCodes.length; j++)
						{
							Integer key = new Integer(sensorCodes[j]);
							SensorValueEx value = sensorValues.get(key);
							if( value != null)
							{
								totalValue[j] += value.getSensorValue();
								sensorCount[j]++;
								
								bUpdateData = true;
							}							
						}						
					}					
				}				
			}	
			
			
			// Save averageValue
			if( bUpdateData == true)
			{
				logger.debug("AverageCronWorker", "Update Average '" + cityName + "'");
				for( int k = 0 ;  k < averageValue.length; k++)
				{
					if( sensorCount[k] == 0)
						averageValue[k] = 0;
					else
						averageValue[k] = totalValue[k] / (float)sensorCount[k];
				}				
				sensorManager.updateCityAverageData(cityName, averageValue);
			}
			else
			{
				logger.debug("AverageCronWorker", "No Data '" + cityName + "'");
			}
		}

		logger.debug("AverageCronWorker", "End Job");
	}
	
	
	public void beginWorker()
	{
		if( mWorkerThread == null)
		{
			mWorkerThread = new Thread()
			{
				public void run()
				{
					try
					{
						while(true)
						{
							cronJob();							
							Thread.sleep(m_timeWork);
						}						
					}
					catch(Exception ex)
					{
						logger.debug(ex.getMessage());
						logger.debug(ex.getStackTrace()[0].toString());
					}
				}
			};
			mWorkerThread.start();
		}		
	}
	
	public void stopWorker()
	{
		try
		{
			if(mWorkerThread != null && mWorkerThread.isAlive())
			{
				mWorkerThread.interrupt();
				mWorkerThread = null;
			}
		}
		catch(Exception ex)
		{
			logger.debug(ex.getMessage());
			logger.debug(ex.getStackTrace()[0].toString());
		}
	}
}
