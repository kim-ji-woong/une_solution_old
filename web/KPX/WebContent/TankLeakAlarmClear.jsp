<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8"%>
<%@ page import="java.sql.*"%>
<%@ page import="javax.sql.*"%>
<%@ page import="javax.naming.*"%>
<%@ page import="javax.sql.DataSource"%>
<%@ page import="java.io.*"%>
<%@ page import="java.util.*"%>
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
	String strTankID = request.getParameter("TankID"); 
	String strOccurType = request.getParameter("OccurType");
	String strComment = request.getParameter("Comment");
	
	/*	
	String strDeviceID = "dS5rmzaG9KQ:APA91bHPOpGm5oMy7esgOVxgRKesWYDvArndQ-p-MdBF8K10-YnC0ZH8i7AxRFFUBKT5djjLUh-9vY5YNiC0iQfyQQyE79FPYaJN0ZaH9auqYJwnHP84DiodE_F3FH5xoBnvDVupC_Ex";
	String strTankID = "13"; 
	String strOccurType = "0";
	String strComment = "aaabbb";
	*/
	 
	if (strDeviceID == null || strTankID == null)
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
				   
				strSQL = "SELECT HistoryID FROM TankLeak WHERE TankID = " + strTankID;  
				result = stmt.executeQuery(strSQL); 
				
				ResultSetMetaData resultMetaData = result.getMetaData();
				int nCount = resultMetaData.getColumnCount();  
				
				int nAlarmHistoryID = 0;
				if (nCount > 0)
				{
					if (result.next())
					{
						nAlarmHistoryID = result.getInt(1);
					}  
				}
				out.println("nAlarmHistoryID : " + nAlarmHistoryID);
				   
				strSQL = "Select ID, MobileUserLevel from User where DeviceID = '" + strDeviceID + "'";
				result = stmt.executeQuery(strSQL);
				 
				/*boolean firstRequest = true;*/
	 
				int nUserID = 0;
				int nMobileUserLevel = 0;
				if (result.next())
				{
					nUserID = result.getInt(1);
					nMobileUserLevel = result.getInt(2);
				} 
				 
				/*firstRequest = false;*/ 

				/*if (firstRequest)*/ 
				   
				if (nUserID > 0 && nMobileUserLevel == 0 && nAlarmHistoryID > 0)
				{  
					int nCommandID = 0;
					strSQL = "SELECT IFNULL(MAX(ID) + 1, 1) ID FROM Command";
					 
					result = stmt.executeQuery(strSQL);
					resultMetaData = result.getMetaData();
					
					nCount = resultMetaData.getColumnCount();
					  
					if (nCount > 0)
					{
						if (result.next())
						{
							nCommandID = result.getInt(1);
							
						}
					} 
					 
					out.println("Begin Data");
					 
					strSQL = "INSERT INTO Command (ID, CommandType, TimeStamp, TankID, UserID, CommandValue) ";
					strSQL += String.format("VALUES (%s, 11, now(), %s, %s, 1)", nCommandID, strTankID, nUserID);
					stmt.executeUpdate(strSQL);
					
					strSQL = "INSERT INTO CommandHistory (ID, CommandType, CommandMakeTime, CommandExecuteTime, UserID, CmdID, TankID, CommandValue, AlarmOccurType, AlarmComment, AlarmHistoryID) ";
					strSQL += String.format("VALUES ((SELECT ID FROM (SELECT IFNULL(MAX(ID) + 1, 1) ID FROM CommandHistory) X), 11, now(), NULL, %s, %s, %s, 1, %s, '%s', %s)", 
								nUserID, nCommandID, strTankID, strOccurType, strComment, nAlarmHistoryID);		
					stmt.executeUpdate(strSQL);  
					
					nCommandID++;
					
					strSQL = "INSERT INTO Command (ID, CommandType, TimeStamp, TankID, UserID, CommandValue) ";
					strSQL += String.format("VALUES (%s, 13, now(), %s, %s, 0)", nCommandID, strTankID, nUserID);
					stmt.executeUpdate(strSQL);
					
					strSQL = "INSERT INTO CommandHistory (ID, CommandType, CommandMakeTime, CommandExecuteTime, UserID, CmdID, TankID, CommandValue, AlarmHistoryID) ";
					strSQL += String.format("VALUES ((SELECT ID FROM (SELECT IFNULL(MAX(ID) + 1, 1) ID FROM CommandHistory) X), 13, now(), NULL, %s, %s, %s, 0, %s)", 
								nUserID, nCommandID, strTankID, strOccurType, strComment, nAlarmHistoryID);		
					stmt.executeUpdate(strSQL);  
					
					out.println("End Data"); 					
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
			out.println(" " + strSQL + " ");
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