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
			strSQL =  "SELECT p.id, Name, p.pressure, lwh.StandardPressure, ";
			strSQL += "	      Status, (select count(*) from LastWorkHistory where EndTime is null and pipeid=p.id) as iswork, Type, TypeBefore  "; 
			strSQL += "     , ifnull(lwh.tankid, -1) as connectTankID, ifnull(PipeStableType, -1) as PipeStableType, ifnull(PipeStableRatio, -1) as PipeStableRatio, ifnull(PipeStableAbsolute, -1) as PipeStableAbsolute "; 
			strSQL += "     , alarm.id as alarmhistoryid, alarm.alarmtype, lwh.BeginTime, now()";
			strSQL += "  FROM Pipe as p LEFT OUTER JOIN (select * from lastworkhistory where endtime is null) as lwh ON p.id=lwh.pipeid "; 
            strSQL += "                 LEFT OUTER JOIN AlarmPipeOptions as ao ON ao.pipeid=lwh.pipeid ";
			strSQL += "                 LEFT OUTER JOIN (select ah.id, ah.pipeid, alarmtype  ";
			strSQL += "                    				   from alarmhistory as ah INNER JOIN alarmrecenthistory as arh  ";
			strSQL += "                    				                    		   ON (arh.AlarmHistoryID1=ah.id OR  ";
			strSQL += "                    				                    			   arh.AlarmHistoryID2=ah.id OR  ";
			strSQL += "                    				                    			   arh.AlarmHistoryID3=ah.id OR  ";
			strSQL += "                    				                    			   arh.AlarmHistoryID4=ah.id)   ";
			strSQL += "                    				                    		  AND (arh.PipeID=ah.PipeID OR arh.PipeID = -1 OR arh.PipeID IS NULL)) as alarm ON alarm.pipeid=p.id ";
			
			Class.forName(driver);
			conn = DriverManager.getConnection(url, id, pw);
			
			if(conn != null)
			{	
				conn.setAutoCommit(true);			
				stmt = conn.createStatement();
				
				result = stmt.executeQuery(strSQL);
				 				
				ResultSetMetaData resultMetaData = result.getMetaData();
				int nCount = resultMetaData.getColumnCount();
				int nRowCount = 0;
				out.println("Begin Data");
				while(result.next())
		 		{ 	
					for(int i = 0; i < nCount; i++)
					{						
						String colTypeNameID = resultMetaData.getColumnTypeName(i+1 % (nCount+1));
						int strID = result.getInt(i+1); 
						
						String colTypeNameName = resultMetaData.getColumnTypeName(i+2 % (nCount+1));
						String strName = result.getString(i+2);
						
						String colTypeNamePressure = resultMetaData.getColumnTypeName(i+3 % (nCount+1));
						double strPressure = result.getDouble(i+3);
						
						String colTypeNameStandardPressure = resultMetaData.getColumnTypeName(i+4 % (nCount+1));
						double strStandardPressure = result.getDouble(i+4);
						  
						String colTypeNameStatus = resultMetaData.getColumnTypeName(i+5 % (nCount+1));
						String strStatus = result.getString(i+5);
						 
						String colTypeNameIsWork = resultMetaData.getColumnTypeName(i+6 % (nCount+1));
						int nIsWork = result.getInt(i+6);
						 
						String colTypeNamePipeType = resultMetaData.getColumnTypeName(i+7 % (nCount+1));
						String strPipeType = result.getString(i+7);
						
						String colTypeNamePipeTypeBefore = resultMetaData.getColumnTypeName(i+8 % (nCount+1));
						String strPipeTypeBefore = result.getString(i+8);
						 
						String colTypeNameConnectTankID = resultMetaData.getColumnTypeName(i+9 % (nCount+1));
						int nConnectTankID = result.getInt(i+9);
						
						String colTypeNamePipeStableType = resultMetaData.getColumnTypeName(i+10 % (nCount+1));
						int nPipeStableType = result.getInt(i+10);
						
						String colTypeNamePipeStableRatio = resultMetaData.getColumnTypeName(i+11 % (nCount+1));
						double nPipeStableRatio = result.getDouble(i+11);
						
						String colTypeNamePipeStableAbsolute = resultMetaData.getColumnTypeName(i+12 % (nCount+1));
						double nPipeStableAbsolute = result.getDouble(i+12);
												 
						String colTypeNameAlarmHistoryID = resultMetaData.getColumnTypeName(i+13 % (nCount+1));
						int nAlarmHistoryID = result.getInt(i+13);
						
						String colTypeNameAlarmType = resultMetaData.getColumnTypeName(i+14 % (nCount+1));
						int nAlarmType = result.getInt(i+14);
						
						String colBeginTime = resultMetaData.getColumnTypeName(i+15 % (nCount+1));
						String strBeginTime = result.getString(i+15);
						
						String colNow = resultMetaData.getColumnTypeName(i+16 % (nCount+1));
						String strNow = result.getString(i+16);
						
						out.println(colTypeNameID + ":[" + strID + "]");
						out.println(colTypeNameName + ":[" + strName + "]");
						out.println(colTypeNamePressure + ":[" + strPressure + "]");
						out.println(colTypeNameStandardPressure + ":[" + strStandardPressure + "]");						 
						out.println(colTypeNameStatus + ":[" + strStatus + "]"); 
						out.println(colTypeNameIsWork + ":[" + nIsWork + "]");  						 
						out.println(colTypeNamePipeType + ":[" + strPipeType + "]");  
						out.println(colTypeNamePipeTypeBefore + ":[" + strPipeTypeBefore + "]");   
						
						out.println(colTypeNameConnectTankID + ":[" + nConnectTankID + "]");  
						out.println(colTypeNamePipeStableType + ":[" + nPipeStableType + "]");  
						out.println(colTypeNamePipeStableRatio + ":[" + nPipeStableRatio + "]");  
						out.println(colTypeNamePipeStableAbsolute + ":[" + nPipeStableAbsolute + "]");  
						 
						out.println(colTypeNameAlarmHistoryID + ":[" + nAlarmHistoryID + "]");  
						out.println(colTypeNameAlarmType + ":[" + nAlarmType + "]");
						out.println(colBeginTime + ":[" + strBeginTime + "]");						
						out.println(colNow + ":[" + strNow + "]");						
						
						nRowCount++;
						break;						
					}					
		 		} 
				// 이미 요청한 상태
				/*out.println("ErrorCode:[1]");*/
				out.println("End Data");
				out.println("Begin Info :" + nCount + "," +nRowCount + " End Info" );
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