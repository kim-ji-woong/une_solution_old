<%@ page language="java" contentType="text/html; charset=EUC-KR"
    pageEncoding="EUC-KR"%>
<%@ page import="java.sql.*"%>
<%@ page import="javax.sql.*"%>
<%@ page import="javax.naming.*"%>

<%!
	private String toUnicode(String str)
	{
		try{
			byte[] b = str.getBytes("ISO-8859-1");
			return new String(b);
		}
		catch(java.io.UnsupportedEncodingException uee)
		{
			System.out.println(uee.getMessage());
			return null;
		}
	}
%>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=EUC-KR">
<title>ID Check</title>
</head>
<body>

<table width="800" cellspacing="0" cellpadding="0" border="1" bordercolor="gray" align="center">
	<tr>
		<td colspan=2>
		<table border="0" width="99%">
			<tr>
				<!-- <td height="22" bgcolor="#6699FF"> -->
				<td><h1 align="center">사용자 입력정보</h1>
				</td>
			</tr>
			<tr>
				<td height="63"><table width="374" border="0" align="center">
				  <tr>
				    <td><p align="right"><font size=2><a href="./joinSuccess.jsp">등록</a></font></p></td>
			      </tr>
				  </table>
				</td>
			</tr>
			<tr>
				<td>
				<form name=itemform method="post"><p align="center">
				  <table border="1" align="center">
			
	<%
	request.setCharacterEncoding("euc-kr");	
	Connection conn=null;
	Statement stmt=null;
	ResultSet result=null;
	RequestDispatcher dispatcher = null;
 	
	String strName = "";
	int nLevelID = 0;
	
	// 웹 페이지에 입력된 값을 받는다.
	String UserName = request.getParameter("UserName"); //비교
	String UserID = request.getParameter("UserID");
	String UserPW = request.getParameter("UserPW");
	int MemberID = Integer.parseInt(request.getParameter("MemberID")); //비교
	String MemberTeamID = request.getParameter("MemberTeamID"); //비교
	
	
	String driver = "com.microsoft.jdbc.sqlserver.SQLServerDriver";
	String url = "jdbc:microsoft:sqlserver:192.168.0.207:3306;databasename=SOP";
	String id = "sa";
	String pw = "9449966Ab";
	
	try{
		Class.forName(driver);
		conn = DriverManager.getConnection(url, id, pw);
		
		if(conn != null)
		{
	 		stmt=conn.createStatement();
	 		//result = stmt.executeQuery("SELECT * FROM SOP.CompanyMember Where id='"+id+"' AND pw ='"+pw+"'");
	 		result = stmt.executeQuery("SELECT * FROM SOPGenUser");

	 		while(result.next())
	 		{
	 			String UserName1 = result.getString("UserName");
	 			String UserID1 = result.getString("UserID");
	 			int MemberID1 = result.getInt("MemberID");
	 			int MemberTeamID1 = result.getInt("MemberTeamID");

//				String strMemberID = Integer.toString(MemberID1); //int를 String 형 변환
	 			if(UserID.equals(UserID1)) 
	 			{
	 				if( MemberID == MemberID1)
	 				{
						if(UserName.equals(UserName1)) 
	 					{
							dispatcher = request.getRequestDispatcher("joinSuccess.jsp");
				
	 %>
	 
		 <tr>
			<td width="150"><p align="center"><font size=2>사원번호 </font></p></td>
			<td width="150"><p align="center"><%= MemberID%></p></td>
		</tr>
		
			<tr>		 
			<td width="150"><p align="center"><font size=2>부서</font></p></td>
			<td width="150"> <%= MemberTeamID%></td>
			</tr>
			
			<tr>
			<td width="150"><p align="center"><font size=2>이름</font></p></td>
			<td width="150"> <%= UserName%></td>
			</tr>
			
			<tr>
			<td width="150"><p align="center">아이디</p></td>
			<td width="150"><%= UserID%></td>
			</tr>
		
	<%
		
	request.setAttribute("MemberID",  new Integer(MemberID));
	request.setAttribute("MemberTeamID",  new Integer(MemberTeamID));
	request.setAttribute("UserName", UserName);
	request.setAttribute("UserID",  UserID);

	
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
/* 	 	
	 	RequestDispatcher dispatcher = null; //request.getRequestDispatcher("admin_item_list.jsp");
	 	dispatcher.forward(request, response); */
	 	dispatcher.forward(request, response);
	%>		
		
		</table>
				  <p>&nbsp;</p>
				  </p>
		        </form>
		
		</td>
		</tr>
		</table>
		</td>
		
	</tr>
</table>


</body>
</html>