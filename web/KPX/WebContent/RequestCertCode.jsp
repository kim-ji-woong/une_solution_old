<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8"%>
<%@ page import="java.sql.*"%>
<%@ page import="javax.sql.*"%>
<%@ page import="javax.naming.*"%>
<%@ page import="javax.sql.DataSource"%>
<%@ page import="java.io.*"%>
<%@ page import="java.util.*"%>
<%@ page import="sun.misc.*"%>
<%@ page import="util.AES256Cipher" %>
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
	String strSerialNumber = request.getParameter("SerialNumber");
	String strTeamName = request.getParameter("TeamName");
	String strUserName = request.getParameter("UserName");
	String strPhoneNumber = request.getParameter("PhoneNumber");
	
	//String szSS = "Ihsz9f/HBfTYQ3vyZfaw8Q==";
	//out.println(szSS);
    //String ssPhoneNumber = AES256Cipher.AES_Encode(strPhoneNumber);
    //out.println(ssPhoneNumber);
    
    //out.println(ddddd);

   
	
	
	if (strDeviceID == null || strSerialNumber == null || strTeamName == null || strUserName == null || strPhoneNumber == null)
	{
		out.println("Begin Data");
		// Parameter 부족
		out.println("ErrorCode:[2]");
		out.println("End Data");
		out.println("Begin Info :1,1 End Info" );
	}
	else
	{
		String encPhoneNumber = AES256Cipher.AES_Encode(strPhoneNumber);
		
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
			strSQL = "Select ID from CertRequest where SerialNumber = '" + strSerialNumber + "'";
			
			Class.forName(driver);
			conn = DriverManager.getConnection(url, id, pw);
			
			if(conn != null)
			{	
				conn.setAutoCommit(true);			
				stmt = conn.createStatement();
				
				result = stmt.executeQuery(strSQL);
				
				boolean firstRequest = true;
				
				ResultSetMetaData resultMetaData = result.getMetaData();
				int nCount = resultMetaData.getColumnCount();

				while(result.next())
		 		{
					for(int i = 0; i < nCount; i++)
					{
						String colTypeName = resultMetaData.getColumnTypeName(i+1 % (nCount+1));
						String strID = result.getString(i+1);
						if(strID!= null)
							strID = strID.trim();
						
						strSQL = "Update CertRequest set DeviceID = '" + strDeviceID + "' where ID = " + strID;
						stmt.executeUpdate(strSQL);
						
						out.println("Begin Data");
						// 이미 요청한 상태
						out.println("ErrorCode:[1]");
						out.println("End Data");
						out.println("Begin Info :1,1 End Info" );
						firstRequest = false;
						break;
					}
		 		}

				if (firstRequest)
				{
					int nID = 1;
					strSQL = "Select max(ID) from CertRequest";
					
					result = stmt.executeQuery(strSQL);
					resultMetaData = result.getMetaData();
					
					nCount = resultMetaData.getColumnCount();
					
					if (nCount > 0)
					{
						if (result.next())
						{
							String strValue = result.getString(1);
							if(strValue!= null)
								strValue = strValue.trim();
							
							try
							{
								int num = Integer.parseInt(strValue);
								nID = num + 1;
							}
							catch (NumberFormatException e)
							{
							}
						}
					}
					
					strSQL = "Insert into CertRequest (ID, TeamName, UserName, PhoneNumber, DeviceID, SerialNumber, CertCode, CertCodeLifeTime, MobileUserLevel) ";
					strSQL += String.format("values (%d, '%s', '%s', '%s', '%s', '%s', NULL, NULL, NULL)", nID, strTeamName, strUserName, encPhoneNumber, strDeviceID, strSerialNumber);
				
					stmt.executeUpdate(strSQL);
					
					out.println("Begin Data");
					// 이미 요청한 상태
					out.println("INT:[0]");
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