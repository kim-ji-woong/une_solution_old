<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8"%>
<%@ page import="java.sql.*"%>
<%@ page import="javax.sql.*"%>
<%@ page import="javax.naming.*"%>
<%@ page import="javax.sql.DataSource"%>
<%@ page import="java.io.*"%>
<%@ page import="java.util.*"%>
<%@ page import="sun.misc.*"%>
<%
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
	
	String strSQL = "select ID, Name from tank where id not in ( select tankid from disconnectedtank) order by Name";

	String strTS = request.getParameter("Transaction");
	boolean isAutoCommit = true;

	if(strTS == null)
		strTS = "0";
	
	if(strTS.equals("1"))
		isAutoCommit = false;
	
	try
	{	
		if(isAutoCommit == false)	
			conn = (Connection)session.getAttribute("DBConnection");	
		else
			conn = null;	

		if( conn == null)
		{
			Class.forName(driver);
			conn = DriverManager.getConnection(url, id, pw);
			
			if(isAutoCommit == false)
				session.setAttribute("DBConnection", conn);
		}
		
		if(conn != null)
		{	
			conn.setAutoCommit(isAutoCommit);			
			stmt = conn.createStatement();
			
			String strTemp = "select";
			if( strSQL.length() >= 5)
				strTemp = strSQL.substring(0, 6).toLowerCase();
			
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
					out.println(colTypeName + ":[" + strValue + "]");
				}
				nRowCount++;
	 		}
			
			out.println("End Data");
			out.println("Begin Info :" + nCount + "," +nRowCount + " End Info" );
		}
	}
	catch(Exception e)
	{
	    log(url);
	    log("Notice " + strSQL);
        log(request.getRemoteAddr());
        
		out.println("JDBC 드라이브 연결 오류-"+e);
		e.printStackTrace();
		out.println("Begin Data");
		out.println("null_SQLError");
		out.println("End Data");
		
		
		
		try
		{
			if(!isAutoCommit)
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
			if(stmt!=null)
				stmt.close();
			
			if(isAutoCommit == true)
			{
				if(conn!=null)
					conn.close();
				session.setAttribute("DBConnection", null);
			}
 			out.println("연결끊음");
 		}
 		catch(Exception e)
 		{
 			e.printStackTrace();
 			if(!isAutoCommit)
	 		{
				//conn.rollback();
				out.println("INT:[0]");
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