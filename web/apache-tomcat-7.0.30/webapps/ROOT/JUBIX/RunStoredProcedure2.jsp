<%@ page language="java" contentType="text/html; charset=UTF-8"
    pageEncoding="UTF-8"%>
<%@ page import="java.sql.*"%>
<%@ page import="javax.sql.*"%>
<%@ page import="javax.naming.*"%>
<%@ page import="javax.sql.DataSource"%>
<%@ page import="java.util.*" %>
<%@ page import="java.util.regex.*" %>
<%@ page import="java.io.*"%>
<%@ page import="sun.misc.*"%>

<%
//__GetUser
	request.setCharacterEncoding("UTF-8");	
	Connection conn=null;
	CallableStatement cstmt=null;
	ResultSet result=null;
	
	String strDB = request.getParameter("DatabaseName");
	if( strDB == null)
		strDB = "SOP_3";	

	String strType = request.getParameter("DatabaseType");
	if( strType == null)
		strType = "mysql";
	
	
	String driver = "com.microsoft.sqlserver.jdbc.SQLServerDriver";
	String url = "jdbc:sqlserver://127.0.0.1:1433;DatabaseName="+strDB;
	String id = "etadams";
	String pw = "et!2345";
	
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
	
	String strSQL = request.getParameter("SQLQuery");
	if( strSQL == null || strSQL.equals("") == true)
	{
		session.setAttribute("Query", "null");
		return;
	}
	else
	{
		int nQueryCount = 1;
		if(session.getAttribute("QueryCount") != null)
		{
			nQueryCount = ((Integer)session.getAttribute("QueryCount")).intValue();
			nQueryCount += 1;
		}
		session.setAttribute("Query"+nQueryCount, strSQL);
		session.setAttribute("QueryCount", new Integer(nQueryCount));
	}
	
	String strTS = request.getParameter("Transaction");
	String strFields = "";
	String strValues = "";
	boolean isAutoCommit = true;
	
	try{
		Class.forName(driver);
		conn = DriverManager.getConnection(url, id, pw);
		if(conn != null)
		{
			if(strTS == null)
				strTS = "0";

			if(strTS.equals("1"))
				isAutoCommit = false;
			
			conn.setAutoCommit(isAutoCommit);

			if(strType.equals("mysql"))
			{
				cstmt = conn.prepareCall(strSQL);
				cstmt.execute();	
				result =  cstmt.getResultSet();
			}
			else
			{
				cstmt = conn.prepareCall(strSQL);
				result = cstmt.executeQuery();		
			}
			
			ResultSetMetaData resultMetaData = result.getMetaData();
			int nCount = resultMetaData.getColumnCount();
			
			out.println("Begin Data");			
			while(result.next())
	 		{
				for(int i = 0; i < nCount; i++)
				{					
					String colTypeName = resultMetaData.getColumnTypeName(i+1 % (nCount+1));
					String strValue = result.getString(i+1);
					if(strValue!= null)
						strValue = strValue.trim();
					out.println(colTypeName + "_*$#:[" + strValue + "]:#$*_");
				}
	 		}
			out.println("End Data");
			
			//result.close();
			//cstmt.close();
			//conn.close();

			if(!isAutoCommit)
				conn.commit();
		}
		
	}catch(Exception e){
		out.println("JDBC 드라이브 연결 오류-"+e);
		e.printStackTrace();
		if(!isAutoCommit)
			conn.rollback();
	}
	finally{
 		try{
 			if(!isAutoCommit)
	 			conn.setAutoCommit(true);
 			
			if(result!=null)
				result.close();
			if(cstmt!=null)
				cstmt.close();
			if(conn!=null)
				conn.close();
 			out.println("연결끊음");
 		}
 		catch(Exception e){
 			e.printStackTrace();
 			if(!isAutoCommit)
 				conn.rollback();

			conn.close();
 		}
	}
	
	//RequestDispatcher dispatcher = request.getRequestDispatcher("LoginResult.jsp");
 	//dispatcher.forward(request, response);
%>

<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
<title>StoredProcedure</title>
</head>
<body>
<h1>Procedure</h1>
</body>
</html>