<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8"%>
<%@ page import="java.sql.*"%>
<%@ page import="javax.sql.*"%>
<%@ page import="javax.naming.*"%>
<%@ page import="javax.sql.DataSource"%>
<%@ page import="java.io.*"%>
<%@ page import="java.util.*"%>
<%@ page import="sun.misc.*"%> 
<%
	/*ERROR_CODE
	  1 : 이미 요청한 상태
	  2 : Parameter 부족
	*/
	request.setCharacterEncoding("UTF-8");	
	Connection conn = null;
	Statement stmt = null;
	ResultSet result = null;
	 	
	String strDeviceID = request.getParameter("DeviceID");
	String strPipeID = request.getParameter("PipeID"); 
	String strTankID = request.getParameter("TankID"); 
	String strWorkType = request.getParameter("WorkType");  // 4:작업 시작, 5:작업 종료 (CommandType)
	
	/*	
	String strDeviceID = "cpLoLd4cNvo:APA91bE__kWnuWa91zVnA3sa4S07sFhlWu4x39ZTiTnXe_FV-TT4AtHo44aulLlkHPKXTiqUXRxSjOA9mv8qR5nmGQBN2NGdHkYx97vj-FMFPQfm7KMvYx_hiePfBw8x-g5k0QBLeFhG";
	String strPipeID = "NULL";
	String strTankID = "1";
	String strWorkType = "4";
	*/ 
	
	if (strDeviceID == null || strPipeID == null || strTankID == null || strWorkType == null)
	{ 	
		out.println("Begin Data");
		// Parameter 부족
		out.println("ErrorCode:[2]");
		out.println("End Data");
		out.println("Begin Info :1,1 End Info" );
	}
	else
	{		
		String strDB = "KPX";
		String strType = "mysql";	
		
		String driver = "com.microsoft.sqlserver.jdbc.SQLServerDriver";
		String url = "jdbc:sqlserver://127.0.0.1:1433;DatabaseName="+strDB;
		String id = "sa";
		String pw = "9449966Ab";
		  
		String strPort = request.getParameter("Port");
		String strHost = request.getParameter("Host");
		
		if(strHost == null)
			strHost = "127.0.0.1";
		
		if(strType.equals("mysql"))
		{
			if(strPort == null)
				strPort = "3306";
			
			driver = "com.mysql.jdbc.Driver";
			url = "jdbc:mysql://"+ strHost + ":"+ strPort + "/"+strDB + "?useUnicode=true&characterEncoding=utf8"; 
		}
		else if(strType.equals("sqlserver"))
		{
			if(strPort == null)
				strPort = "1433";
			driver = "com.microsoft.sqlserver.jdbc.SQLServerDriver";
			url = "jdbc:sqlserver://"+ strHost + ":" + strPort + ";DatabaseName="+strDB;
		}
		
		String strSQL = "";
		
		try
		{ 
			Class.forName(driver);
			conn = DriverManager.getConnection(url, id, pw);
			
			if(conn != null)
			{	
				conn.setAutoCommit(true);			
				stmt = conn.createStatement();
				 
				strSQL = "Select ID from User where DeviceID = '" + strDeviceID + "'";
				result = stmt.executeQuery(strSQL);
				
				ResultSetMetaData resultMetaData = result.getMetaData();
				int nCount = resultMetaData.getColumnCount(); 
				int nUserID = 0; 
				if (result.next())
				{
					nUserID = result.getInt(1);
				}
				  				 
				if (nUserID > 0)
				{  		
					int nCommandID = 0;
					strSQL = "SELECT IFNULL(MAX(ID) + 1, 1) ID FROM Command";
					
					result = stmt.executeQuery(strSQL);
					resultMetaData = result.getMetaData();
					
					nCount = resultMetaData.getColumnCount();
					if (result.next())
					{
						nCommandID = result.getInt(1); 
					}
					
					if (strWorkType.equals("4")) //작업 시작
					{ 
					    // 이미 작업중인지 판단
						strSQL = "SELECT Count(PipeID) FROM LastWorkHistory WHERE EndTime IS NULL AND TankID = " + strTankID;
						result = stmt.executeQuery(strSQL);
						resultMetaData = result.getMetaData();
						
						nCount = resultMetaData.getColumnCount();
						if (result.next())
						{
							int nConnectedTankCnt = result.getInt(1); 
							if (nConnectedTankCnt < 2)
							{
								out.println("Begin Data"); 
								if (nCommandID > 0)
								{
									strSQL = "INSERT INTO Command (ID, CommandType, TimeStamp, PipeID, TankID, UserID) ";
									strSQL += String.format("VALUES (%s, 4, now(), %s, %s, %s)", nCommandID, strPipeID, strTankID, nUserID);
									 
									stmt.executeUpdate(strSQL);
									
									strSQL = "INSERT INTO CommandHistory (ID, CommandType, CommandMakeTime, CommandExecuteTime, UserID, CmdID, PipeID, TankID) ";
									strSQL += String.format("VALUES ((SELECT ID FROM (SELECT IFNULL(MAX(ID) + 1, 1) ID FROM CommandHistory) X), 4, now(), NULL, %s, %s, %s, %s)"
											, nUserID, nCommandID, strPipeID, strTankID);
								
									stmt.executeUpdate(strSQL); 
								} 								
								out.println("End Data"); 
							}							
						} 
					}	
					else if (strWorkType.equals("5")) //작업 종료
					{
						out.println("Begin Data");
						  
						if (nCommandID > 0)
						{
							strSQL = "INSERT INTO Command (ID, CommandType, TimeStamp, PipeID, TankID, UserID) ";
							strSQL += String.format("VALUES (%s, 5, now(), %s, %s, %s)", nCommandID, strPipeID, strTankID, nUserID);
							 
							stmt.executeUpdate(strSQL);
							
							strSQL = "INSERT INTO CommandHistory (ID, CommandType, CommandMakeTime, CommandExecuteTime, UserID, CmdID, PipeID, TankID) ";
							strSQL += String.format("VALUES ((SELECT ID FROM (SELECT IFNULL(MAX(ID) + 1, 1) ID FROM CommandHistory) X), 5, now(), NULL, %s, %s, %s, %s)"
									, nUserID, nCommandID, strPipeID, strTankID);
						
							stmt.executeUpdate(strSQL); 
						} 
						
						out.println("End Data");
					}
				}   
			}
		}
		catch(Exception e)
		{
		    log(url);
		    log("RequestCertCode " + strSQL);
	        log(request.getRemoteAddr());
	        
			out.println("JDBC 드라이브 연결 오류-"+e);
			e.printStackTrace();
			out.println("Begin Data");
			out.println("null_SQLError");
			out.println("End Data");
		}
		finally
		{
	 		try
	 		{ 	
	 			if(result!=null)
					result.close();
				if(stmt!=null)
					stmt.close();
				
				if(conn!=null)
					conn.close();
				session.setAttribute("DBConnection", null);
				
	 			out.println("연결끊음");
	 		}
	 		catch(Exception e)
	 		{
	 			e.printStackTrace();
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
<h1>DBQuery Result</h1>
</body>
</html>