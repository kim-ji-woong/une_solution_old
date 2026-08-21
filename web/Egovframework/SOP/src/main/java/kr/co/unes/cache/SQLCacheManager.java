package kr.co.unes.cache;

import java.util.Calendar;
import java.util.Date;
import java.util.HashMap;
import java.util.Map;

import org.springframework.beans.factory.annotation.Value;

public class SQLCacheManager {
	
	private static SQLCacheManager m_Instance = new SQLCacheManager();
	
	public static SQLCacheManager getInstance() {
		return m_Instance;
	}
	
	@Value("${db.catchtime}")
	private int nCacheTime = 1;
	
	private Map<String, QueryCache> queryMap = null;
	private SQLCacheManager()
	{		
		queryMap = new HashMap<String,QueryCache>();
	}

	public QueryCache FindCache(String strSQL) {

		if( queryMap.containsKey(strSQL))
		{
			QueryCache result = queryMap.get(strSQL);
			if( checkCache(result))
				return result;			
		}
		return null;
	}
	
	private boolean checkCache(QueryCache result)
	{
		long time = System.currentTimeMillis(); 
		
		Date now = new Date(time);
		Calendar cal = Calendar.getInstance();
		cal.setTime(now);
		cal.add(Calendar.SECOND, -(nCacheTime));
		
		Date toTime= cal.getTime();
		if( toTime.after(result.getQueryTime()))
		{
			return true;
		}
		return false;
	}

	public QueryCache AddQueryCache(String strSQL) {
		
		if( queryMap.containsKey(strSQL))
		{
			QueryCache result = queryMap.get(strSQL);
			result.setQuery(strSQL);
			return result;
		}			 
		return null;
	}

	public void AddQueryCache(QueryCache cache) {
		queryMap.put(cache.getQuery(),  cache);
	}
}
