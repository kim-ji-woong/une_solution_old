<%@ page language="java" contentType="text/html; charset=EUC-KR"
    pageEncoding="EUC-KR"%>
<%@ page import="java.sql.*"%>
<%@ page import="javax.sql.*"%>
<%@ page import="javax.naming.*"%>

<%
	request.setCharacterEncoding("euc-kr");	
	Connection conn=null;
	Statement stmt=null;
	ResultSet result=null;
	RequestDispatcher dispatcher = null;
	
	String driver = "com.microsoft.sqlserver.jdbc.SQLServerDriver";
	String url = "jdbc:microsoft:sqlserver:192.168.0.207:3306;databasename=SOP";
	String id = "sa";
	String pw = "9449966Ab";	
	
	try{
		Class.forName("com.microsoft.sqlserver.jdbc.SQLServerDriver");
		conn = DriverManager.getConnection(url, id, pw);
		stmt = conn.createStatement();
		
	}catch(Exception e){
 			e.printStackTrace();
 	}
	

	
%>    

<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=EUC-KR">
<title>Insert title here</title>
</head>
<body>

</body>
</html>