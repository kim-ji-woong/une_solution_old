<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8"%>
<%@ page import="java.sql.*"%>
<%@ page import="javax.sql.*"%>
<%@ page import="javax.naming.*"%>
<%@ page import="javax.sql.DataSource"%>
<%@ page import="java.io.*"%>
<%@ page import="sun.misc.*"%>
<%
	request.setCharacterEncoding("UTF-8");	
	{	
	  	session.setMaxInactiveInterval(5);
		Statement stmt=null;
		ResultSet result=null;	
		
		// Parsing Request Parameter		
		String strCmd = request.getParameter("Cmd");
		if( strCmd == null || strCmd.equals(""))
		{
			strCmd = "Batch";
		}
		
		String strSQL2 = request.getParameter("SQLQuery");
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
		
		String strDB = request.getParameter("DatabaseName");
		if( strDB == null)
			strDB = "EDU_100";
		
		boolean isTS = true;
		boolean bExcuteMode = false;
		
		// Find Connection in User Session	
		Connection conn=null;
		Connection dbConnection = (Connection)session.getAttribute("DBConnection");
		if( dbConnection != null && dbConnection.isClosed() == true)
		{
			session.removeAttribute("DBConnection");
			session.setAttribute("DBConnection", null);
			dbConnection = null;
		}
			
		if( dbConnection == null )//|| (strCmd != null && strCmd.equals("Batch"))) // Not Exist
		{
			// Create DB Connection & Add to Session Data
			try
			{
				String driver = "com.microsoft.sqlserver.jdbc.SQLServerDriver";
				String url = "jdbc:sqlserver://127.0.0.1:1433;DatabaseName="+strDB;
				
				String id = "sa";
				String pw = "9449966Ab";
				
				String strPort = request.getParameter("Port");
				String strHost = request.getParameter("Host");
				if(strHost == null)
					strHost = "127.0.0.1";
				
				String strType = request.getParameter("DatabaseType");
				if( strType == null)
					strType = "mysql";
				
				if(strType.equals("mysql"))
				{
					if(strPort == null)
						strPort = "3306";
					
					driver = "com.mysql.jdbc.Driver";
					url = "jdbc:mysql://"+ strHost + ":"+ strPort + "/"+strDB;
				}
				else if(strType.equals("sqlserver"))
				{
					if(strPort == null)
						strPort = "1433";
					
					driver = "com.microsoft.sqlserver.jdbc.SQLServerDriver";
					url = "jdbc:sqlserver://"+ strHost + ":" + strPort + ";DatabaseName="+strDB;
				}
				
				Class.forName(driver);
				conn = DriverManager.getConnection(url, id, pw);
				
				if(conn != null)
				{				  	
					//conn.setTransactionIsolation(0x1000);				
					conn.setAutoCommit(false);
					
					session.setAttribute("DBConnection", conn);
					
					
					if(strType.equals("mysql"))
					{
						try
						{
							Statement stmt2 = conn.createStatement();
							stmt2.executeQuery("START TRANSACTION READ WRITE;");							
						}
						catch(Exception ex)
						{
							
						}
					}
					
				}				
			}
			catch(Exception e)
			{	
				log("Create Connection : " + request.getRemoteAddr());	
				log("Create Connection : "+e);
					
				out.println("JDBC 드라이브 연결 오류-"+e);
				e.printStackTrace();
				out.println("Begin Data");
				out.println("null_SQLError");
				out.println("End Data");		
				return;
			}
		}
		else
		{			
			conn = dbConnection;
			//conn.setAutoCommit(false);
		}		
		
		if( conn != null)
		{
			// Command Rollback - Run Bacth rollback and close
			if( strCmd.equals("RollBack"))
			{
				try
				{
					//application.log("RollBack");				
					session.removeAttribute("DBConnection");
					session.setAttribute("DBConnection", null);				
					//conn.setTransactionIsolation(0x0010);	
				}
				catch(Exception e)
				{	
					log("Rollback Begin : " + request.getRemoteAddr());	
					log("Rollback Begin : "+e);				
				}
				
				try
				{
					conn.rollback();
					conn.close();
				}catch(Exception e)
				{
					log("Rollback End : " + request.getRemoteAddr());	
					log("Rollback End : "+e);	
					
					out.println("JDBC 드라이브 연결 오류-"+e);
					e.printStackTrace();
					out.println("Begin Data");
					out.println("null_SQLError");
					out.println("End Data");		
					return;		
				}
				//application.log("RollBack End");
				
				out.println("Begin Data");
				out.println("INT_*$#:[3]:#$*_");
				out.println("End Data");
				return;
			}		
			// Command Commit - Run Bacth Commit and close
			else if( strCmd.equals("Commit"))
			{			
				try
				{
					session.removeAttribute("DBConnection");
					conn.commit();
					//conn.setTransactionIsolation(0x0010);	
					conn.close();
				}
				catch(Exception e)
				{	
					log("Commit : " + request.getRemoteAddr());	
					log("Commit : "+e);
				}
				out.println("Begin Data");
				out.println("INT_*$#:[3]:#$*_");
				out.println("End Data");
				return;
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
							out.println("Begin Data");
							
							while(result.next())
					 		{
								for(int i = 0; i < nCount; i++)
								{
									String colTypeName = resultMetaData.getColumnTypeName(i+1 % (nCount+1));
									String strValue = result.getString(i+1);
									if(strValue!= null)
										strValue = strValue.trim();
									out.println(colTypeName + "_*$#:[" + strValue + "]:#$*_");
									//String strValue = result.getString(i+1);
									//out.println(strValue);
								}
								nRowCount++;
					 		}
							out.println("End Data");
							out.println("Begin Info :" + nCount + "," +nRowCount + " End Info" );
						}
						catch(Exception e)
						{							
							log("BatchQuery " + strSQL);
							log(request.getRemoteAddr());
							log("JDBC 드라이브 연결 오류-"+e);
							
							session.removeAttribute("DBConnection");
							
							out.println("JDBC 드라이브 연결 오류-"+e);
							e.printStackTrace();
							out.println("Begin Data");
							out.println("null_SQLError");
							out.println("End Data");
							
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
					 			out.println("연결끊음");
					 		}
					 		catch(Exception e)
					 		{
					 			session.removeAttribute("DBConnection");
					 			e.printStackTrace();
					 			conn.rollback();
								out.println("Begin Data");
								out.println("INT_*$#:[0]:#$*_");
								out.println("End Data");
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
								out.println("Begin Data");
								out.println("INT_*$#:[" + n + "]:#$*_");
								out.println("End Data");	
								return;							
								
							}
							else
							{
								result = stmt.executeQuery(strSQL);	
								ResultSetMetaData resultMetaData = result.getMetaData();
								int nCount = resultMetaData.getColumnCount();
								int nRowCount = 0;
								out.println("Begin Data");
								while(result.next())
						 		{
									for(int i = 0; i < nCount; i++)
									{
										String colTypeName = resultMetaData.getColumnTypeName(i+1 % (nCount+1));
										String strValue = result.getString(i+1);
										if(strValue!= null)
											strValue = strValue.trim();
										out.println(colTypeName + "_*$#:[" + strValue + "]:#$*_");
									}
									nRowCount++;
						 		}
								out.println("End Data");
								out.println("Begin Info :" + nCount + "," +nRowCount + " End Info" );
							}
						}
						catch(Exception e)
						{
							log("BatchQuery " + strSQL);
							log(request.getRemoteAddr());
							log("JDBC 드라이브 연결 오류-"+e);
							
							
							out.println("JDBC 드라이브 연결 오류-"+e);
							e.printStackTrace();
							out.println("Begin Data");
							out.println("null_SQLError");
							out.println("End Data");
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
						  //application.log("Finally");	
					 		try
					 		{	 			
					 			if(result!=null && bExcuteMode == false)
									result.close();
								if(stmt!=null)
									stmt.close();									
									
					 			out.println("연결끊음");
					 		}
					 		catch(Exception e)
					 		{
					 		  	application.log("예외");
					 		  	e.printStackTrace();
					 			
					 			try
					 			{
					 				conn.rollback();
					 			}
					 			catch(Exception es)
					 			{
					 				
					 			}
								
					 			out.println("Begin Data");
								out.println("INT_*$#:[0]:#$*_");
								out.println("End Data");
								
								session.removeAttribute("DBConnection");
					 		}
						}
					}					
				}			
			}		
		}	
	}	
%>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
<title>DBUtil</title>
</head>
<body>
<h1>BATCH</h1>
</body>
</html>