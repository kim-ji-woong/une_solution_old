package kr.co.unes.query;

import kr.co.unes.data.*;
import kr.co.unes.cache.*;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.ResultSet;
import java.sql.ResultSetMetaData;
import java.sql.Statement;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpSession;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public class QueryExcutor {

	
	private ClientData clientData = null;
	
	private boolean isAutoCommit;
	
	private HttpServletRequest request;
	
	private HttpSession session;
	private String driver;
	private String url;
	private String id;
	private String pw;
	
	private boolean bUseCache = false;
	
	private static final Logger logger = LoggerFactory.getLogger(QueryExcutor.class);
	
	public QueryExcutor(ClientData data) {
		clientData = data;
		
		isAutoCommit = !clientData.IsTransaction();
		
		request = clientData.getRequest();
		session = clientData.getSession();
		
		driver = clientData.getDriverClassName();
		url = clientData.getJdbcURL();
		id = clientData.getDBUser();
		pw = clientData.getDbPass();		
	}

	public List<String> ExcuteQuery(String strSQL)
	{
		if( strSQL.toLowerCase().startsWith("update"))
		{
			logger.info("DBQuery2 : "+ strSQL);
		}
		
		
		List<String> arResult = new ArrayList<String>();
		Connection conn = null;
		Statement stmt = null;
		ResultSet result = null;
		try
		{	
			if(isAutoCommit == false)	
				conn = (Connection)session.getAttribute("DBConnection");	
			else
				conn = null;	
			
			if(strSQL.equals("close"))
			{	
				if(conn!=null && conn.isClosed() == false)
					conn.close();
				session.setAttribute("DBConnection", null);
				
				session.setMaxInactiveInterval(1);
				System.out.println("Close");
			}
			else			
			{
				if( conn == null)
				{
					Class.forName(driver);
					conn = DriverManager.getConnection(url, id, pw);
					
					if(isAutoCommit == false)
						session.setAttribute("DBConnection", conn);
				}
				
				if(conn != null)
				{	
					conn.setAutoCommit(isAutoCommit);			
					
					
					String strTemp = "select";
					if( strSQL.length() >= 5)
						strTemp = strSQL.trim().substring(0, 6).toLowerCase();

					if(!strTemp.equals("select"))
					{
						if(strTemp.equals("commit"))
						{
							conn.commit();
							arResult.add("INT_*$#:[1]:#$*_");
						}
						else if(strTemp.equals("rollba"))
						{
							conn.rollback();
							arResult.add("INT_*$#:[1]:#$*_");
						}	
						else					
						{
							stmt = conn.createStatement();
							int nResult = stmt.executeUpdate(strSQL);
							
							arResult.add("Begin Data");
							arResult.add("End Data");
							arResult.add("INT_*$#:[" + nResult + "]:#$*_");
							arResult.add("Begin Info :" + 1 + "," +1 + " End Info" );
						}				
					}
					else
					{		
						boolean bAddCache = false;
						if(bUseCache == true)
						{
							SQLCacheManager qm = SQLCacheManager.getInstance();
							QueryCache cache = qm.FindCache(strSQL);
							if( cache != null && cache.isbExcuteQuery() == true )
							{
								List<String> cacheResult = cache.getQueryResult();
								if(cacheResult != null)
								{
									try
									{									
										if(isAutoCommit == true)
										{
											if(conn!=null)
												conn.close();
											session.setAttribute("DBConnection", null);
										}
									}
									catch(Exception ex)
									{
									}
									logger.info("Cache Hit : " + strSQL);
									return cacheResult;
								}
							}
							bAddCache = true;
						}
						
						stmt = conn.createStatement();
						result = stmt.executeQuery(strSQL);
						ResultSetMetaData resultMetaData = result.getMetaData();
						int nCount = resultMetaData.getColumnCount();
						int nRowCount = 0;
						
						arResult.add("Begin Data");
						while(result.next())
				 		{
							for(int i = 0; i < nCount; i++)
							{
								String colTypeName = resultMetaData.getColumnTypeName(i+1 % (nCount+1));
								String strValue = result.getString(i+1);
								if(strValue!= null)
								{
									strValue = strValue.trim();

									String typeName = colTypeName.toLowerCase();

									if(typeName.indexOf("char") >= 0 || typeName.indexOf("text") >= 0)
									{
										strValue = strValue.replace('\n', (char)6 );
										strValue = strValue.replace('\r', (char)7 );
										strValue = strValue.replace('\'', (char)8 );
									}
								}
															
								String szTemp = colTypeName + "_*$#:[" + strValue + "]:#$*_";
								arResult.add(szTemp);
							}
							nRowCount++;
				 		}
						arResult.add("End Data");
						arResult.add("Begin Info :" + nCount + "," +nRowCount + " End Info" );
						
						if(bUseCache == true && bAddCache == true)
						{
							SQLCacheManager qm = SQLCacheManager.getInstance();
							QueryCache cache = qm.AddQueryCache(strSQL);
							if(cache == null)
							{
								cache = new QueryCache();
								cache.setbExcuteQuery(true);
								cache.setQuery(strSQL);
								qm.AddQueryCache(cache);
							}
							// set cache time
							long time = System.currentTimeMillis(); 							
							Date now = new Date(time);	
							cache.setQueryTime(now);
							cache.setbExcuteQuery(true);
							cache.setQueryResult(arResult);
						}
					}
				}
			}	
		}
		catch(Exception e)
		{
			logger.info(url);
			logger.info("DBQuery2 " + strSQL);
			logger.info(request.getRemoteAddr());
	        
			arResult.clear();
	        arResult.add("JDBC 드라이브 연결 오류-"+e);
			e.printStackTrace();
			arResult.add("Begin Data");
			arResult.add("null_SQLError");
			arResult.add("End Data");					
			
			try
			{
				if(!isAutoCommit)
					conn.rollback();
			}
			catch(Exception ex)
			{
			}
		}
		finally
		{
	 		try
	 		{ 	
	 			if(result!=null)
					result.close();
				if(stmt!=null)
					stmt.close();
				
				if(isAutoCommit == true)
				{
					if(conn!=null)
						conn.close();
					session.setAttribute("DBConnection", null);
				}
				arResult.add("연결끊음");
	 		}
	 		catch(Exception e)
	 		{
	 			e.printStackTrace();
	 			if(!isAutoCommit)
		 		{	 				
	 				arResult.add("INT_*$#:[0]:#$*_");
				}
	 		}
		}		
		return arResult;
	}
	
	

}
