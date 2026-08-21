package kr.co.unes.cache;

import java.util.ArrayList;
import java.util.Date;
import java.util.List;

public class QueryCache {
	
	private String strQuery;	
	private Date queryTime;	
	private boolean bExcuteQuery = false;	
	private List<String> arQueryResult = new ArrayList<String>();	
		
	public QueryCache()
	{	
	}	
	
	@Override
	public String toString()
	{
		return strQuery;
	}

	public String getQuery() {
		return strQuery;
	}

	public void setQuery(String strQuerry) {
		this.strQuery = strQuerry;
	}

	public Date getQueryTime() {
		return queryTime;
	}

	public void setQueryTime(Date queryTime) {
		this.queryTime = queryTime;
	}

	public boolean isbExcuteQuery() {
		return bExcuteQuery;
	}

	public void setbExcuteQuery(boolean bExcuteQuery) {
		this.bExcuteQuery = bExcuteQuery;
	}

	public List<String> getQueryResult() {
		return arQueryResult;
	}

	public void setQueryResult(List<String> arQueryResult) {
		this.arQueryResult = arQueryResult;
	}

}
