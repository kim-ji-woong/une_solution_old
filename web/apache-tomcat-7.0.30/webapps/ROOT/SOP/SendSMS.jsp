<%@ page language="java" contentType="text/html; charset=UTF-8"
    pageEncoding="UTF-8"%>
<%@ page import="java.sql.*"%>
<%@ page import="javax.sql.*"%>
<%@ page import="javax.naming.*"%>
<%@ page import="javax.sql.DataSource"%>
<%@ page import="java.io.*"%>
<%@ page import="sun.misc.*"%>


<%
//__GetUser

	request.setCharacterEncoding("UTF-8");
	Connection conn=null;
	Statement stmt=null;
	ResultSet result=null;	  

	String driver = "com.mysql.jdbc.Driver";
	String url = "jdbc:mysql://192.168.0.210:3306/pamts_sms2?useUnicode=true&characterEncoding=UTF8";
	String id = "smsuser";
	String pw = "smsnd";	
	String strSender = request.getParameter("Sender");
	String strReciver = request.getParameter("Reciver");
	String strMsg = request.getParameter("Msg");
	
	if( strSender == null || strReciver == null || strMsg == null)
	{
		out.println("Begin Data");
		out.println("null_SQLError");
		out.println("End Data");
		return;
	}

	String strTemp;
	strTemp = new String(strMsg.getBytes("8859_1"), "ksc5601");	
	strMsg = strTemp;

	try
	{
       
			 String strSQL = "insert into LOG_SMS(CLASS,CLIENT,WRITE_TIME,DESTINATION,CALLBACK,BODY,SEND_FLAG) values('TMS','SP',now(),'";
			 strSQL = strSQL + strReciver + "','";
			 strSQL = strSQL + strSender + "','";
			 strSQL = strSQL + strMsg + "','1')";
//strSQL = new String(strSQL.getBytes("KSC5601"), "KSC5601");
			 out.println(strSQL);

		Class.forName(driver);
		conn = DriverManager.getConnection(url, id, pw);
		if(conn != null)
		{			
			 conn.setAutoCommit(false);
			
			stmt = conn.createStatement();
			
			stmt.executeUpdate(strSQL);
			
			conn.commit();
			out.println(1);			
		}
	}catch(Exception e){
		out.println("JDBC 드라이브 연결 오류-"+e);
		e.printStackTrace();
		out.println("Begin Data");
		out.println("null_SQLError");
		out.println("End Data");
		try
		{
			conn.rollback();
		}
		catch(Exception ex){
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
			if(conn!=null)
				conn.close();
 			out.println("연결끊음");
 		}
 		catch(Exception e){
 			e.printStackTrace();
 			conn.rollback();
			out.println(0);			
 		}
	}
%>

<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
<title>SendSMS</title>
</head>
<body>
<h1>SMS</h1>
</body>
</html>