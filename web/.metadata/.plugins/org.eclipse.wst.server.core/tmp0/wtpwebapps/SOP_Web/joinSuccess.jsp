<%@ page language="java" contentType="text/html; charset=EUC-KR"
    pageEncoding="EUC-KR"%>
<%@ page import="java.sql.*" %>
<%@ page import="javax.sql.*" %>
<%@ page import="javax.naming.*" %>

<%
	request.setCharacterEncoding("euc-kr");

	String strName = "";
	int nLevelID = 0;
	boolean bCheck = false;
	
	// 웹 페이지에 입력된 값을 받는다.
	String name=request.getParameter("name"); //비교
	String id=request.getParameter("id");
	String pw=request.getParameter("pw");
	int teamid=Integer.parseInt(request.getParameter("teamid")); //비교
	//int memberid=Integer.parseInt(request.getParameter("memberid")); //비교
	String memberid=request.getParameter("memberid"); //비교

	Connection conn=null;
 	Statement stmt=null;
 	ResultSet result=null;
 	
 	String driver ="sun.jdbc.odbc.JdbcOdbcDriver";
	String url="jdbc:odbc:MSAccessDB"; //local
	
	try{
		Class.forName(driver);
		conn = DriverManager.getConnection(url, "", "");
		
		if(conn != null)
		{
			out.println("DB연결");
	 		stmt=conn.createStatement();
	 		//result = stmt.executeQuery("SELECT * FROM SOP.CompanyMember Where id='"+id+"' AND pw ='"+pw+"'");
	 		result = stmt.executeQuery("SELECT * FROM CompanyMember");


	 		while(result.next())
	 		{
	 			String MemberID = result.getString("MemberID");
	 			int RegularTeamID = result.getInt("RegularTeamID");
	 			String MemberName = result.getString("MemberName");
	 			
	 			int TemporaryTeamID = result.getInt("TemporaryTeamID");
	 			int LevelID = result.getInt("LevelID");
	 			int PositionID = result.getInt("PositionID");
	 			int TemporaryPositionID = result.getInt("TemporaryPositionID");
	 			
	 			
	 			request.setAttribute("MemberID",  MemberID);
				request.setAttribute("RegularTeamID",  new Integer(RegularTeamID));
				request.setAttribute("MemberName", MemberName);
				request.setAttribute("TemporaryTeamID",  new Integer(TemporaryTeamID));
				request.setAttribute("LevelID", new Integer(LevelID));
				request.setAttribute("PositionID",  new Integer(PositionID));
				request.setAttribute("TemporaryPositionID", new Integer(TemporaryPositionID));

				
				if( teamid == RegularTeamID)
				{
					if( (name.equals(MemberName)) && (memberid.equals(MemberID)))
					{
 						strName = MemberName;
						nLevelID = LevelID; 
						bCheck = true;

						if(bCheck)
						{
							if(!strName.equals(""))
							{
								if(nLevelID != 0)
								{
									stmt=conn.createStatement();
									
								String command = String.format("INSERT INTO SOPGenUser"+"(MemberID,UserName,UserLevel,TeamID,Password,UserID) values ('%s','%s','%d','%d','%s','%s');",memberid,strName,nLevelID,teamid,pw,id);
								int rowNum = stmt.executeUpdate(command);
								out.println("사용자 등록 성공");
								out.println(command);
								}
							}
						}
					}
				}
	 		}
		}

	}
	finally{
 		try{
 			stmt.close();
 		}
 		catch(Exception e){
 			e.printStackTrace();
 		}
 		
 		try{
 			conn.close();
 		}
 		catch(Exception e){
 			e.printStackTrace();
 		}
 	}
 	
 	RequestDispatcher dispatcher = null; 
 	dispatcher.forward(request, response);
	
%>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=EUC-KR">
<title>Insert title here</title>
</head>
<script type="text/javascript">
function fncClose()
{
	alert("확인");
}
</script>
<body>
<!-- 	<h3> 회원가입이 완료되었습니다. </h3> -->
<!--     <form name ="result" action="main.jsp" > -->
<!--         <input type="submit" value="  완료  "/> -->
<!--     </form> -->
<center>
<%-- <%

if(bCheck)
{
	if(!strName.equals(""))
	{
		if(nLevelID != 0)
		{
			stmt=conn.createStatement();
			
		String command = String.format("INSERT INTO SOPGenUser"+"(MemberID,UserName,UserLevel,TeamID,Password,UserID) values ('%s','%s','%d','%d','%s','%s');",memberid,strName,nLevelID,teamid,pw,id);
		int rowNum = stmt.executeUpdate(command);
		out.println("사용자 등록 성공");
		}
	}
}
%> --%>
<br><br>
<a><input type="button" name="seccess" id="seccess" value=" 확인 " onclick="javascript:fncClose()"></a>
</center>
</body>
</html>