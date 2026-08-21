package kr.co.unes.query;

import kr.co.unes.data.*;

import java.sql.CallableStatement;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.ResultSet;
import java.sql.ResultSetMetaData;
import java.sql.SQLException;
import java.sql.Statement;
import java.util.ArrayList;
import java.util.List;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpSession;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public class BatchQueryExcutor {


	private ClientData clientData = null;
	
	@SuppressWarnings("unused")
	private boolean isAutoCommit;
	
	private HttpServletRequest request;
	
	private HttpSession session;
	private String driver;
	private String url;
	private String id;
	private String pw;
	
	private String strCmd;
	
	private static final Logger logger = LoggerFactory.getLogger(BatchQueryExcutor.class);
	
	public BatchQueryExcutor(ClientData data) {
		clientData = data;
		
		isAutoCommit = !clientData.IsTransaction();
		
		request = clientData.getRequest();
		session = clientData.getSession();
		
		driver = clientData.getDriverClassName();
		url = clientData.getJdbcURL();
		id = clientData.getDBUser();
		pw = clientData.getDbPass();
		
		strCmd = clientData.getBatchCmd();
	}

	public List<String> ExcuteBatch(String batchSQL)
	{
		logger.info(strCmd + " : " + batchSQL );
		
		List<String> arResult = new ArrayList<String>();
		Statement stmt=null;
		ResultSet result=null;			
		
		String strSQL2 = batchSQL;
		if( strSQL2 == null || strSQL2.equals("") == true)
		{
			session.setAttribute("Query", "null");			
		}
		else
		{
			int nQueryCount = 1;
			if(session.getAttribute("QueryCount") != null)
			{
				nQueryCount = ((Integer)session.getAttribute("QueryCount")).intValue();
				nQueryCount += 1;
			}
			session.setAttribute("Query"+nQueryCount, strSQL2);
			session.setAttribute("QueryCount", new Integer(nQueryCount));
		}		
		
		//boolean isTS = true;
		boolean bExcuteMode = false;
		
		// Find Connection in User Session	
		Connection conn=null;
		Connection dbConnection = (Connection)session.getAttribute("DBConnection");
		try {
			if( dbConnection != null && dbConnection.isClosed() == true)
			{
				session.removeAttribute("DBConnection");
				session.setAttribute("DBConnection", null);
				dbConnection = null;
			}
		} catch (SQLException e1) {
			e1.printStackTrace();
		}
			
		if( dbConnection == null )//|| (strCmd != null && strCmd.equals("Batch"))) // Not Exist
		{
			// Create DB Connection & Add to Session Data
			try
			{				
				String strType = request.getParameter("DatabaseType");
				if( strType == null)
					strType = "mysql";				
				
				Class.forName(driver);
				conn = DriverManager.getConnection(url, id, pw);
				
				if(conn != null)
				{
					conn.setAutoCommit(false);					
					session.setAttribute("DBConnection", conn);										
					if(strType.equals("mysql"))
					{
						try
						{
							//Statement stmt2 = conn.createStatement();
							//stmt2.executeQuery("START TRANSACTION READ WRITE;");							
						}
						catch(Exception ex)
						{
							
						}
					}
					else if(strType.equals("sqlserver"))
					{
						try
						{
							Statement stmt2 = conn.createStatement();
							stmt2.executeQuery("SET TRANSACTION ISOLATION LEVEL READ COMMITTED;");							
						}
						catch(Exception ex)
						{
							
						}
						
					}
				}				
			}
			catch(Exception e)
			{	
				logger.debug("Create Connection : " + request.getRemoteAddr());	
				logger.debug("Create Connection : "+e);
					
				
				arResult.add("JDBC 드라이브 연결 오류-"+e);
				e.printStackTrace();
				arResult.add("Begin Data");
				arResult.add("null_SQLError");
				arResult.add("End Data");		
				return arResult;
			}
		}
		else
		{			
			conn = dbConnection;
		}		
		
		if( conn != null)
		{
			
			// Command Rollback - Run Bacth rollback and close
			if( strCmd.equals("RollBack"))
			{
				try
				{							
					session.removeAttribute("DBConnection");
					session.setAttribute("DBConnection", null);				
				}
				catch(Exception e)
				{	
					logger.debug("Rollback Begin : " + request.getRemoteAddr());	
					logger.debug("Rollback Begin : "+e);				
				}
				
				try
				{
					conn.rollback();
					conn.close();
				}
				catch(Exception e)
				{
					logger.debug("Rollback End : " + request.getRemoteAddr());	
					logger.debug("Rollback End : "+e);	
					
					arResult.add("JDBC 드라이브 연결 오류-"+e);
					e.printStackTrace();
					arResult.add("Begin Data");
					arResult.add("null_SQLError");
					arResult.add("End Data");		
					return arResult;		
				}			
				arResult.add("Begin Data");
				arResult.add("INT_*$#:[3]:#$*_");
				arResult.add("End Data");
				return arResult;
			}		
			
			// Command Commit - Run Bacth Commit and close
			else if( strCmd.equals("Commit"))
			{			
				try
				{
					session.removeAttribute("DBConnection");
					session.setAttribute("DBConnection", null);		
				}
				catch(Exception e)
				{	
					logger.debug("Commit : " + request.getRemoteAddr());	
					logger.debug("Commit : "+e);
				}
				
				try
				{					
					conn.commit();
					conn.close();
				}
				catch(Exception e)
				{	
					logger.debug("Commit : " + request.getRemoteAddr());	
					logger.debug("Commit : "+e);
				}
				
				arResult.add("Begin Data");
				arResult.add("INT_*$#:[3]:#$*_");
				arResult.add("End Data");
				return arResult;
			}
			// Command Batch - Add Batch and Continues Use
			else if(strCmd.equals("Batch"))
			{
				boolean bProcMode = false;
				String strPrepared = request.getParameter("Proc");
				if( strPrepared != null)
				{
					if( strPrepared.equals("true"))
					{
						bProcMode = true;
					}
				}
				
				String strSQL = request.getParameter("SQLQuery");				
				if( strSQL != null && !strSQL.equals(""))
				{
					if( bProcMode == true)
					{
						CallableStatement cstmt=null;
						try
						{							
							cstmt = conn.prepareCall(strSQL);
							result = cstmt.executeQuery();
							
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
										strValue = strValue.trim();
									arResult.add(colTypeName + "_*$#:[" + strValue + "]:#$*_");
								}
								nRowCount++;
					 		}
							arResult.add("End Data");
							arResult.add("Begin Info :" + nCount + "," +nRowCount + " End Info" );
						}
						catch(Exception e)
						{							
							logger.debug("BatchQuery " + strSQL);
							logger.debug(request.getRemoteAddr());
							logger.debug("JDBC 드라이브 연결 오류-"+e);
							
							session.removeAttribute("DBConnection");
							
							arResult.add("JDBC 드라이브 연결 오류-"+e);
							e.printStackTrace();
							arResult.add("Begin Data");
							arResult.add("null_SQLError");
							arResult.add("End Data");
							
							try
							{	
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
								if(cstmt!=null)
									cstmt.close();				
								arResult.add("연결끊음");
					 		}
					 		catch(Exception e)
					 		{
					 			session.removeAttribute("DBConnection");
					 			e.printStackTrace();
					 			
					 			try {
									conn.rollback();
								} catch (SQLException e1) {
								}
					 			
					 			arResult.add("Begin Data");
					 			arResult.add("INT_*$#:[0]:#$*_");
					 			arResult.add("End Data");
					 		}
						}						
					}
					else
					{					 
						try
						{	
							if( strSQL.length() < 6)
							{
								strSQL = "select";
							}
							stmt = conn.createStatement();			
							String strTemp = strSQL.substring(0, 6).toLowerCase();
							if(!strTemp.equals("select"))
							{
							  	bExcuteMode = true;
						  
								int n = stmt.executeUpdate(strSQL);
								arResult.add("Begin Data");
								arResult.add("INT_*$#:[" + n + "]:#$*_");
								arResult.add("End Data");	
								return arResult;							
								
							}
							else
							{
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
											strValue = strValue.trim();
										arResult.add(colTypeName + "_*$#:[" + strValue + "]:#$*_");
									}
									nRowCount++;
						 		}
								arResult.add("End Data");
								arResult.add("Begin Info :" + nCount + "," +nRowCount + " End Info" );
							}
						}
						catch(Exception e)
						{
							logger.debug("BatchQuery " + strSQL);
							logger.debug(request.getRemoteAddr());
							logger.debug("JDBC 드라이브 연결 오류-"+e);
														
							arResult.add("JDBC 드라이브 연결 오류-"+e);
							e.printStackTrace();
							arResult.add("Begin Data");
							arResult.add("null_SQLError");
							arResult.add("End Data");
							session.removeAttribute("DBConnection");
							
							try
							{	
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
					 			if(result!=null && bExcuteMode == false)
									result.close();
								if(stmt!=null)
									stmt.close();									
									
								arResult.add("연결끊음");
					 		}
					 		catch(Exception e)
					 		{
					 			logger.debug("예외");
					 		  	e.printStackTrace();
					 			
					 			try
					 			{
					 				conn.rollback();
					 			}
					 			catch(Exception es)
					 			{					 				
					 			}
								
					 			arResult.add("Begin Data");
					 			arResult.add("INT_*$#:[0]:#$*_");
					 			arResult.add("End Data");
								
								session.removeAttribute("DBConnection");
					 		}
						}
					}
				}
			}
		}
		return arResult;
	}
}
