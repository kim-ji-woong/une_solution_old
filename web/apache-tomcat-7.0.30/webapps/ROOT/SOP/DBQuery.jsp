<%@ page language="java" contentType="text/html; charset=EUC-KR" pageEncoding="EUC-KR"%>
<%@ page import="java.sql.*"%>
<%@ page import="javax.sql.*"%>
<%@ page import="javax.naming.*"%>
<%@ page import="java.io.*"%>
<%

	request.setCharacterEncoding("UTF-8");	
	Connection conn=null;
	Statement stmt=null;
	ResultSet result=null;
	
	String driver = "com.microsoft.sqlserver.jdbc.SQLServerDriver";
	String url = "jdbc:sqlserver://192.168.0.207:1433;DatabaseName=SOP";

	String id = "sa";
	String pw = "9449966Ab";	
	String strSQL = request.getParameter("SQLQuery");
	String strTS = request.getParameter("Transaction");
	
	boolean isTS = true;
	
	try
	{
		Class.forName(driver);
		conn = DriverManager.getConnection(url, id, pw);
		if(conn != null)
		{
			//out.println("JDBC 드라이브 연결 성공");
			//out.println("<br>");
			
			if(strTS.equals("1"))
				isTS = false;
			
			conn.setAutoCommit(isTS);			
			stmt = conn.createStatement();			
			
			String strTemp = strSQL.substring(0, 6).toLowerCase();			
			if(!strTemp.equals("select"))
			{
				stmt.executeUpdate(strSQL);
				if(!isTS)
					conn.commit();
			}
			else
			{
				result = stmt.executeQuery(strSQL);
				ResultSetMetaData resultMetaData = result.getMetaData();
				int nCount = resultMetaData.getColumnCount();
				out.println("Begin Data");
				while(result.next())
		 		{
					for(int i = 0; i < nCount; i++)
					{
						String colTypeName = resultMetaData.getColumnTypeName(i+1 % nCount);
						String strValue = result.getString(i+1);
						out.println(colTypeName + ":[" + strValue + "]");
						
						//String strValue = result.getString(i+1);
						//out.println(strValue);
					}
		 		}
				out.println("End Data");
				
				if(!isTS)
					conn.commit();
			}
		}
	}
	catch(IOException e)
	{
		out.println("JDBC 드라이브 연결 오류-"+e);
		e.printStackTrace();
		if(isTS)
			conn.rollback();
	}
	catch(Exception e)
	{
		out.println("JDBC 드라이브 연결 오류-"+e);
		e.printStackTrace();
		if(isTS)
			conn.rollback();
	}
	finally
	{
 		try
 		{
 			if(!isTS)
	 			conn.setAutoCommit(true); 			
 			if(result!=null)
				result.close();
			if(stmt!=null)
				stmt.close();
			if(conn!=null)
				conn.close();
 			out.println("연결끊음");
 		}
 		catch(Exception e)
 		{
 			e.printStackTrace();
 			if(!isTS)
 				conn.rollback();
 		}
	}
%>

<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=EUC-KR">
<title>DBQery JSP</title>
</head>
<body>
<h1>Query</h1>
</body>
</html>