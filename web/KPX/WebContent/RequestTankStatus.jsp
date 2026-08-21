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
			strSQL = "SELECT t.id, Name, LiquidType, Level, Temperature, Density, Mass, Flow, Capacity, Type, Status, HighLevel, MinTemp, MaxTemp ";   
			strSQL +="     , (select count(AlarmHistoryID2) from alarmrecenthistory as arh where t.id=arh.tankid) as AlarmHistoryID2 ";
			strSQL +="     , (select count(AlarmHistoryID3) from alarmrecenthistory as arh where t.id=arh.tankid) as AlarmHistoryID3 ";
			strSQL +="     , (select count(*) from LastWorkHistory where EndTime is null and tankid=t.id) as iswork ";
			strSQL +="     , lwh.StandardFlow, ifnull(TankStableType, -1) as TankStableType, ifnull(TankStableRatio, -1) as TankStableRatio, ifnull(TankStableAbsolute, -1) as TankStableAbsolute ";
			strSQL +="     , IsLeakStatus, IsLeakMonitoring, lwh.BeginTime ";
			strSQL +="	FROM Tank as t LEFT OUTER JOIN (select * from lastworkhistory where endtime is null) as lwh ON T.id=lwh.tankid ";
			strSQL +="		     		    INNER JOIN AlarmOptions as ao ON t.id=ao.tankid "; 
			
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
						
						String colTypeNameLiquidType = resultMetaData.getColumnTypeName(i+3 % (nCount+1));
						String strLiquidType = result.getString(i+3);
						
						String colTypeNameLevel = resultMetaData.getColumnTypeName(i+4 % (nCount+1));
						double strLevel = result.getDouble(i+4);
						
						String colTypeNameTemp = resultMetaData.getColumnTypeName(i+5 % (nCount+1));
						double strTemp = result.getDouble(i+5);
						
						String colTypeNameDensity = resultMetaData.getColumnTypeName(i+6 % (nCount+1));
						double strDensity = result.getDouble(i+6);
						
						String colTypeNameMass = resultMetaData.getColumnTypeName(i+7 % (nCount+1));
						double strMass = result.getDouble(i+7);
						
						String colTypeNameFlow = resultMetaData.getColumnTypeName(i+8 % (nCount+1));
						double strFlow = result.getDouble(i+8);
						
						String colTypeNameCapacity = resultMetaData.getColumnTypeName(i+9 % (nCount+1));
						double capacity = result.getDouble(i+9);
						
						String colTypeNameTankType = resultMetaData.getColumnTypeName(i+10 % (nCount+1));
						String strTankType = result.getString(i+10);
						
						String colTypeNameStatus = resultMetaData.getColumnTypeName(i+11 % (nCount+1));
						int nStatus = result.getInt(i+11);
						
						String colTypeNameHighLevel = resultMetaData.getColumnTypeName(i+12 % (nCount+1));
						double nHighLevel = result.getDouble(i+12);
						
						String colTypeNameMinTemp = resultMetaData.getColumnTypeName(i+13 % (nCount+1));
						double nMinTemp = result.getDouble(i+13);
						
						String colTypeNameMaxTemp = resultMetaData.getColumnTypeName(i+14 % (nCount+1));
						double nMaxTemp = result.getDouble(i+14);
						
						String colTypeNameAlarmHisotryID1 = resultMetaData.getColumnTypeName(i+15 % (nCount+1));
						int nAlarmHisotryID1 = result.getInt(i+15);
						
						String colTypeNameAlarmHisotryID2 = resultMetaData.getColumnTypeName(i+16 % (nCount+1));
						int nAlarmHisotryID2 = result.getInt(i+16);
						
						String colTypeNameIsWork = resultMetaData.getColumnTypeName(i+17 % (nCount+1));
						int nIsWork = result.getInt(i+17);
						
						String colTypeNameStandardFlow = resultMetaData.getColumnTypeName(i+18 % (nCount+1));
						double strStandardFlow = result.getDouble(i+18);
						
						String colTypeNameTankStableType = resultMetaData.getColumnTypeName(i+19 % (nCount+1));
						int nTankStableType = result.getInt(i+19);
						
						String colTypeNameTankStableRatio = resultMetaData.getColumnTypeName(i+20 % (nCount+1));
						double nTankStableRatio = result.getDouble(i+20);
						
						String colTypeNameTankStableAbsolute = resultMetaData.getColumnTypeName(i+21 % (nCount+1));
						double nTankStableAbsolute = result.getDouble(i+21);
						
						String colTypeNameIsLeakStatus = resultMetaData.getColumnTypeName(i+22 % (nCount+1));
						int nIsLeakStatus = result.getInt(i+22);
						
						String colTypeNameIsLeakMonitoring = resultMetaData.getColumnTypeName(i+23 % (nCount+1));
						int nIsLeakMonitoring = result.getInt(i+23);
						
						String colBeginTime = resultMetaData.getColumnTypeName(i+24 % (nCount+1));
						String strBeginTime = result.getString(i+24);
						
						out.println(colTypeNameID + ":[" + strID + "]");
						out.println(colTypeNameName + ":[" + strName + "]");
						out.println(colTypeNameLiquidType + ":[" + strLiquidType + "]");
						out.println(colTypeNameLevel + ":[" + strLevel + "]");
						out.println(colTypeNameTemp + ":[" + strTemp + "]");
						out.println(colTypeNameDensity + ":[" + strDensity + "]");
						out.println(colTypeNameMass + ":[" + strMass + "]");
						out.println(colTypeNameFlow + ":[" + strFlow + "]");
						out.println(colTypeNameCapacity + ":[" + capacity + "]");
						out.println(colTypeNameTankType + ":[" + strTankType + "]"); 
						out.println(colTypeNameStatus + ":[" + nStatus + "]");
						out.println(colTypeNameHighLevel + ":[" + nHighLevel + "]");
						out.println(colTypeNameMinTemp + ":[" + nMinTemp + "]");
						out.println(colTypeNameMaxTemp + ":[" + nMaxTemp + "]");
						out.println(colTypeNameAlarmHisotryID1 + ":[" + nAlarmHisotryID1 + "]");
						out.println(colTypeNameAlarmHisotryID2 + ":[" + nAlarmHisotryID2 + "]");
						out.println(colTypeNameIsWork + ":[" + nIsWork + "]");
						out.println(colTypeNameStandardFlow + ":[" + strStandardFlow + "]");
						out.println(colTypeNameTankStableType + ":[" + nTankStableType + "]");  
						out.println(colTypeNameTankStableRatio + ":[" + nTankStableRatio + "]");  
						out.println(colTypeNameTankStableAbsolute + ":[" + nTankStableAbsolute + "]");  
						out.println(colTypeNameIsLeakStatus + ":[" + nIsLeakStatus + "]");  
						out.println(colTypeNameIsLeakMonitoring + ":[" + nIsLeakMonitoring + "]");
						out.println(colBeginTime + ":[" + strBeginTime + "]");
 
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