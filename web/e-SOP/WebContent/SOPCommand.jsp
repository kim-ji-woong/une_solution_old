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
	  4 : 연결된 SOP 없음
	*/
	request.setCharacterEncoding("UTF-8");	
	Connection conn = null;
	Statement stmt = null;
	ResultSet result = null;

	String strProcessID = request.getParameter("ProcessID");
	String strChecked = request.getParameter("Checked");
	
	if (strProcessID == null || strChecked == null)
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

				strSQL = "Select ash.ActionStepID, ash.RealMode from MobileAppUsingActionStep as ma, ActionStepHistory as ash where ma.ID = 1 and ma.ActionStepID = ash.ActionStepID and ash.EndTime is null and ash.CancelTime is null order by ash.BeginTime desc";
				result = stmt.executeQuery(strSQL);

				ResultSetMetaData resultMetaData = result.getMetaData();
				String strActionStepID = null;
				String strRealMode = null;
				int nColumnCount = resultMetaData.getColumnCount();
				
				if (nColumnCount >= 2)
				{
					if (result.next())
					{
						try
						{
							strActionStepID = result.getString(1);
							strRealMode = result.getString(2);
						}
						catch (Exception e)
						{
						}
					}
				}
				
				if (strActionStepID != null)
				{
					strSQL = "Insert into MobileAppCommand (ActionStepID, RealMode, ProcessID, Checked, Processed) values (" + strActionStepID + ", " + strRealMode + ", " + strProcessID + ", " + strChecked + ", 0)";
					stmt.executeUpdate(strSQL);

					out.println("Begin Data");
					// Command 요청 성공
					out.println("INT:[0]");
					out.println("End Data");
					out.println("Begin Info :1,1 End Info" );
				}
				else
				{
					out.println("Begin Data");
					// 연결된 SOP 없음
					out.println("INT:[4]");
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