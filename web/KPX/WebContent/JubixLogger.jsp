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
	  1 : 이미 요청한 상태
	  2 : Parameter 부족
	*/
	request.setCharacterEncoding("UTF-8");	
	Connection conn = null;
	Statement stmt = null;
	ResultSet result = null;
		
	String strSiren = request.getParameter("siren");
	
	/*
	String strDeviceID = "cJqU-V_v1rc:APA91bEWYFxTOjfddoKMKA1uyeziLq8FYotskGJgP956sbvCsq-uPVfkkob_0zNXo9THzsILdVtrHBGU0zr-L0bnMl4sm4TA8LkYVIogkp_xdbvIj3SQTCESpNabq9383zEs59qUcO7m";
	String strPipeID = "1";
	*/
		
	String szBtnStatus = "";
	String szSirenStatus = "";	
	 	
	String strDB = "KPX";
	String strType = "mysql";		

	String driver = "com.mysql.jdbc.Driver";
	String url = "jdbc:mysql://127.0.0.1:3306/" + strDB + "?useUnicode=true&characterEncoding=utf8"; 
	String id = "sa";
	String pw = "9449966Ab";
	
	String strPort = request.getParameter("Port");
	String strHost = request.getParameter("Host");
	if(strHost == null)
		strHost = "192.168.0.182";
	
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
			 
			strSQL = "SELECT PropertyName, PropertyValue FROM options WHERE PropertyName='SirenStatus' OR PropertyName='ButtonStatus'";
			result = stmt.executeQuery(strSQL);
			
			ResultSetMetaData resultMetaData = result.getMetaData();
			int nCount = resultMetaData.getColumnCount();
			int dd = 0;
			while (result.next())
			{
				
				String strName = result.getString(1);
				if( strName.equals("SirenStatus"))
				{
					szSirenStatus = result.getString(2);
				}
				else
				{
					szBtnStatus = result.getString(2);
				}
			}
			result.close();
			
			if (strSiren == null)
			{
			}
			else
			{
				String szSiren = "0";
				if(strSiren.equals("on"))
				{
					szSiren = "1";
				}
				
				try
				{           
					strSQL = "select date_format(now(), '%Y%m%d%H%i%s')";
					result = stmt.executeQuery(strSQL);
					String dtNow = "";					
					if (result.next())
					{
						dtNow = result.getString(1); 
					}
					result.close();
					       
					strSQL = "select MAX(ID) from command";
					result = stmt.executeQuery(strSQL);
					int nID = 0;				
					if (result.next())
					{
						nID = result.getInt(1) + 1;
					}
					result.close();
					        
					String szTemp2 = "INSERT INTO command (ID,CommandType,TimeStamp,PipeID,TankID,UserID,CommandName,CommandValue) " +
					        " VALUES ( %d, %s, now(), -1, -1, 1, NULL, NULL)";
					String szSQL1 = String.format(szTemp2, nID, szSiren);
					stmt.executeUpdate(szSQL1);
					
					strSQL = "select MAX(ID) from commandhistory";
					result = stmt.executeQuery(strSQL);			
					int nHistoryID = 0;
					if (result.next())
					{
						nHistoryID = result.getInt(1) + 1;
					}
					result.close();
					
					String szTemp1 = "INSERT INTO commandhistory (ID,CommandType,CommandMakeTime,CommandExecuteTime,UserID,CmdID, PipeID, TankID,CommandName,CommandValue,AlarmHistoryID) "+
					                 " VALUES ( %d, %s, now(), NULL, 1, %d, -1, -1, NULL, NULL, -1 )";
					
					String szSQL2 = String.format(szTemp1, nHistoryID, szSiren, nID);
					stmt.executeUpdate(szSQL2);

				}catch(Exception exx)
				{
					out.println("JDBC 드라이브 연결 오류-"+exx);
					exx.printStackTrace();
					out.println("Begin Data");
					out.println("null_SQLError");
					out.println("End Data");
				}
				finally
				{		
				}
			}
		}
	}
	catch(Exception e)
	{
	    log(url);
	    //log("RequestCertCode " + strSQL);
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
			
 			//out.println("연결끊음");
 		}
 		catch(Exception e)
 		{
 			e.printStackTrace();
 		}
	}
		
	if( szBtnStatus.equals("0"))
		szBtnStatus = "켜짐";
	else
		szBtnStatus = "꺼짐";
	
	if( szSirenStatus.equals("1"))
		szSirenStatus = "켜짐";
	else
		szSirenStatus = "꺼짐";	
	
%>
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
<title>Jubix 로거 상태</title>
</head>
<body>
  <div id='jubixInfo'>
      <p>사이렌 상태 : <%= szSirenStatus %></p>
      <p>함체버튼 상태 : <%= szBtnStatus %></p>
  </div>
  <br>
  <br>
  
	<div id='jubixCmd'>
    <form>
      <div>
        <input type="radio" id="sirenon" name="siren" value="on">사이렌켜기</input>
       <input type="radio" id="sirenoff" name="siren" value="off">사이렌끄기</input>
     </div>
     <br>
      <div>	
       <button type="submit">Submit</button>
     </div>   
    </form>
	</div>
</body>
</html>