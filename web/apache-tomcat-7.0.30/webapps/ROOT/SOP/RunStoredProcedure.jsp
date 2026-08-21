<%@ page language="java" contentType="text/html; charset=UTF-8"
    pageEncoding="UTF-8"%>
<%@ page import="java.sql.*"%>
<%@ page import="javax.sql.*"%>
<%@ page import="javax.naming.*"%>
<%@ page import="java.util.*" %>
<%@ page import="java.util.regex.*" %>


<%
//__GetUser
	request.setCharacterEncoding("UTF-8");	
	Connection conn=null;
	CallableStatement cstmt=null;
	ResultSet result=null;
	String strDB = request.getParameter("DatabaseName");
	if( strDB == null)
		strDB = "SOP3";
	String driver = "com.microsoft.sqlserver.jdbc.SQLServerDriver";
	String url = "jdbc:sqlserver://127.0.0.1:1433;DatabaseName="+strDB;
	String id = "sa";
	String pw = "9449966Ab";	
	String strSQL = request.getParameter("SQLQuery");
	//String strSQL = "sp_LatestVersion(une,0)";
	//String strSQL = "sp_LatestVersion";
	//String strParam = ",1";//"une,0";
	String strTS = request.getParameter("Transaction");
	String strFields = "";
	String strValues = "";
	boolean isTS = true;
	
	try{
		Class.forName(driver);
		conn = DriverManager.getConnection(url, id, pw);
		if(conn != null)
		{
			//log(strSQL);
			//out.println("JDBC 드라이브 연결 성공");
			//out.println("<br>");
			if(strTS.equals("1"))
				isTS = false;
			
			conn.setAutoCommit(isTS);
			
			
			//Pattern p = Pattern.compile(",");
			//Matcher m = p.matcher(strParam);
			//int count = 0;
			//while(m.find())
			//{
			//	count++;
			//}
			
			//for(int i=0; m.find(i); i = m.end())
			//{
				//count++;
			//}
			
			//StringTokenizer st = new StringTokenizer(strParam, ",", true);
			//int ntoken = st.countTokens();
			//int cnt = 1;
			//while (st.hasMoreTokens())
			//{
				
			//	String strParm1 = st.nextToken();
				//cstmt.setString(cnt, strParm1);
			//	cnt++;
			//}
			
			
			//cstmt = conn.prepareCall("{call " + strSQL + "(" + strParam + ")" + "}");
			cstmt = conn.prepareCall("exec " + strSQL);
			//cstmt = conn.prepareCall("exec sp_LatestVersion 'une','0'");
			result = cstmt.executeQuery();
			
			//stmt = conn.createStatement();
			
			//result = stmt.executeQuery(strSQL);
			//result = stmt.executeQuery("SELECT * FROM SOPGenUser");
			
			ResultSetMetaData resultMetaData = result.getMetaData();
			int nCount = resultMetaData.getColumnCount();
			
			out.println("Begin Data");
			
			while(result.next())
	 		{
				for(int i = 0; i < nCount; i++)
				{
					String strValue = result.getString(i+1);
					out.println(strValue);
				}
	 		}
			out.println("End Data");
			
			if(!isTS)
				conn.commit();
		}
		
	}catch(Exception e){
		out.println("JDBC 드라이브 연결 오류-"+e);
		e.printStackTrace();
		if(!isTS)
			conn.rollback();
	}
	finally{
 		try{
 			if(!isTS)
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
 			if(!isTS)
 				conn.rollback();
 		}
	}
	
	//RequestDispatcher dispatcher = request.getRequestDispatcher("LoginResult.jsp");
 	//dispatcher.forward(request, response);
%>

<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
<title>Insert title here</title>
</head>
<body>
<h1> TEST</h1>
</body>
</html>