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
	  6 : 승인되지 않은 사용자
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
				
				strSQL = "Select u.ID, u.MobileUserLevel, ug.PipeAccess, ug.PipeItems, ug.TankAccess, ug.TankItems from User as u, UserGroup as ug where u.UserGroup = ug.ID and Mobile = 1 and SerialNumber = '" + strSerialNumber + "'";
				result = stmt.executeQuery(strSQL);
				
				ResultSetMetaData resultMetaData = result.getMetaData();
				boolean certificatedUser = false;
				String strUserLevel = "", strPipeAccess = "", strPipeItems = "", strTankAccess = "", strTankItems = "";

				if (resultMetaData.getColumnCount() > 1)
				{
					if (result.next())
					{
						try
						{
							String strID = result.getString(1);
							strUserLevel = result.getString(2);
							strPipeAccess = result.getString(3);
							strPipeItems = result.getString(4);
							strTankAccess = result.getString(5);
							strTankItems = result.getString(6);
							
							Integer.parseInt(strID);
							
							strSQL = "Update User set DeviceID = '" + strDeviceID + "' where ID = " + strID;
							stmt.executeUpdate(strSQL);
							
							certificatedUser = true;
						}
						catch (Exception e)
						{
						}
					}
				}
				
				if (certificatedUser)
				{
					out.println("Begin Data");
					// 승인된 사용자
					out.println("INT:[" + strUserLevel + "]");
					out.println("INT:[" + strPipeAccess + "]");
					out.println("VARCHAR:[" + strPipeItems + "]");
					out.println("VARCHAR:[" + strTankAccess + "]");
					out.println("VARCHAR:[" + strTankItems + "]");
					out.println("End Data");
					out.println("Begin Info :4,1 End Info" );
				}
				else
				{
					strSQL = "Select ID, CertCode, CertCodeLifeTime, MobileUserLevel from CertRequest where SerialNumber = '" + strSerialNumber + "'";
					result = stmt.executeQuery(strSQL);
					
					boolean find = false;
					String strCertCode = null, strCertCodeLifeTime = null, strMobileUserLevel = null;
					resultMetaData = result.getMetaData();
					
					if (resultMetaData.getColumnCount() > 0)
					{
						if (result.next())
						{
							try
							{
								String strID = result.getString(1);
								Integer.parseInt(strID);
								
								strCertCode = result.getString(2);
								strCertCodeLifeTime = result.getString(3);
								strMobileUserLevel = result.getString(4);
								
								strSQL = "Update CertRequest set DeviceID = '" + strDeviceID + "' where ID = " + strID;
								stmt.executeUpdate(strSQL);
								
								find = true;
							}
							catch (Exception e)
							{
								e.printStackTrace();
							}
						}
					}
					
					if (find)
					{
						out.println("Begin Data");
						
						if (strCertCode != null && strCertCodeLifeTime != null && strMobileUserLevel != null &&
								strCertCode.equals("null") == false &&
										strCertCodeLifeTime.equals("null") == false &&
												strMobileUserLevel.equals("null") == false)
						{
							// 승인 코드가 발급되었으나 확인하지 않은 사용자
							out.println("ErrorCode:[7]");
						}
						else
						{
							// 이미 인증 요청을 한 사용자
							out.println("ErrorCode:[1]");
						}
						
						out.println("End Data");
						out.println("Begin Info :1,1 End Info" );
					}
					else
					{
						out.println("Begin Data");
						// 승인되지 않은 사용자
						out.println("ErrorCode:[6]");
						out.println("End Data");
						out.println("Begin Info :1,1 End Info" );
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