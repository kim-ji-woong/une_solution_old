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
	  2 : Parameter 부족
	  3 : 등록되지 않은 사용자
	*/
	request.setCharacterEncoding("UTF-8");	
	Connection conn = null;
	Statement stmt = null;
	ResultSet result = null;
	
	String strDeviceID = request.getParameter("DeviceID");
	String strSerialNumber = request.getParameter("SerialNumber");
	
	if (strSerialNumber == null || strDeviceID == null)
	{
		out.println("Begin Data");
		// Parameter 부족
		out.println("ErrorCode:[2]");
		out.println("End Data");
		out.println("Begin Info :1,1 End Info" );
	}
	else
	{
		String strDB = "SOP_3";
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
				
				strSQL = "Select ID, DeviceID from MobileAppUser where SerialNumber = '" + strSerialNumber + "'";
				result = stmt.executeQuery(strSQL);
				
				ResultSetMetaData resultMetaData = result.getMetaData();
				boolean registeredUser = false;
				
				if (resultMetaData.getColumnCount() > 0)
				{
					if (result.next())
					{
						try
						{
							String strID = result.getString(1);
							String strOriginDeviceID = result.getString(2);
							
							Integer.parseInt(strID);
							
							if (strDeviceID.equals(strOriginDeviceID) == false || strDeviceID.equals("temp"))
							{
								strSQL = "Update MobileAppUser set DeviceID = '" + strDeviceID + "' where ID = " + strID;
								stmt.executeUpdate(strSQL);
							}

							registeredUser = true;
						}
						catch (Exception e)
						{
						}
					}
				}
				
				if (registeredUser)
				{
					strSQL = "Select ID, Title, Message, SensorZoneHistoryID from MobileAppLastNotification where Title is not null and Message is not null order by ID desc";
					result = stmt.executeQuery(strSQL);
					resultMetaData = result.getMetaData();
					
					boolean alarm = false;
					int nColumnCount = 4;
					
					if (resultMetaData.getColumnCount() == nColumnCount)
					{
						while (result.next())
						{
							try
							{
								String strID = result.getString(1);
								String strTitle = result.getString(2);
								String strMessage = result.getString(3);
								String strSensorZoneHistoryID = result.getString(4);
								
								strSQL = "Select ID from SensorReactionHistory where ReactionType = 50 and SensorHistoryID = " + strSensorZoneHistoryID;
								ResultSet result2 = stmt.executeQuery(strSQL);
								ResultSetMetaData resultMetaData2 = result2.getMetaData();

								boolean releaseAlarm = false;
								
								if (resultMetaData2.getColumnCount() > 0)
								{
									if (result2.next())
									{
										try
										{
											String strID2 = result2.getString(1);
											Integer.parseInt(strID2);

											releaseAlarm = true;
										}
										catch (Exception e)
										{
										}
									}
								}
								
								if (releaseAlarm)
								{
									strSQL = "Update MobileAppLastNotification set Title = NULL, Message = NULL where ID = " + strID;
									stmt.executeUpdate(strSQL);

									//alarm = false;
								}
								else
								{
									// 가장 나중에 발생한 알람만 알린다.
									if (alarm == false)
									{
										// 현재 진행중인 알람이 존재한다.
										out.println("Begin Data");
										// 등록된 사용자
										out.println("INT:[1]");
										out.println("VARCHAR:[" + strTitle + "]");
										out.println("VARCHAR:[" + strMessage + "]");
										out.println("End Data");
										out.println("Begin Info :1,3 End Info" );
									}
									
									alarm = true;
								}
							}
							catch (Exception e)
							{
							}
						}
					}
					
					if (alarm == false)
					{
						out.println("Begin Data");
						// 등록된 사용자
						out.println("INT:[0]");
						out.println("End Data");
						out.println("Begin Info :1,1 End Info" );
					}
				}
				else
				{
					out.println("Begin Data");
					// 등록되지 않은 사용자
					out.println("ErrorCode:[3]");
					out.println("End Data");
					out.println("Begin Info :1,1 End Info" );
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