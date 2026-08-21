<%@ page language="java" contentType="text/html; charset=EUC-KR"
    pageEncoding="EUC-KR"%>
<%@ page import="java.sql.*"%>
<%@ page import="javax.sql.*"%>
<%@ page import="javax.naming.*"%>
<%@ page import="javax.sql.DataSource"%>
<%@ page import="java.io.*"%>
<%@ page import="sun.misc.*"%>

<%
	request.setCharacterEncoding("UTF-8");		
	synchronized(this)
	{
		String strCmd = request.getParameter("Cmd");
		if( strCmd != null)
		{
			String user = request.getParameter("User");
			session.setAttribute("UserName", user);
			if( user == null)
			{
				out.println("Begin Data");
				out.println("0");				
				out.println("End Data");
			}
			else
			{
				if( strCmd.equals("Login"))
				{			
	
						application.setAttribute(user, session);
					
					out.println("Begin Data");
					out.println("INT_*$#:[1]:#$*_");				
					out.println("End Data");
				}
				else if( strCmd.equals("Logout"))
				{
					HttpSession ses = (HttpSession)application.getAttribute(user);
					application.removeAttribute(user);
					
					if( ses != null)
						ses.invalidate();
					
					out.println("Begin Data");
					out.println("INT_*$#:[1]:#$*_");						
					out.println("End Data");
					
					
				}
				else if( strCmd.equals("Check"))
				{
					HttpSession ses = (HttpSession)application.getAttribute(user);
					if( ses == null)
					{
						out.println("Begin Data");
						out.println("INT_*$#:[0]:#$*_");						
						out.println("End Data");
					}
					else
					{
						try
						{
							long nCurrentTime = session.getLastAccessedTime();
							long nAccessTime = ses.getLastAccessedTime();
							
							if( nCurrentTime - nAccessTime > 15000 )
							{						
								application.removeAttribute(user);
								ses.invalidate();
								
								out.println("Begin Data");
								out.println("INT_*$#:[0]:#$*_");					
								out.println("End Data");
							}
							else
							{
								out.println("Begin Data");
								out.println("INT_*$#:[1]:#$*_");					
								out.println("End Data");
							}	
						}
						catch(Exception e)
						{
							out.println("Begin Data");
							out.println("INT_*$#:[0]:#$*_");						
							out.println("End Data");
						}
											
					}
				}
			}		
		}
	}
%>

<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" "http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<meta http-equiv="Content-Type" content="text/html; charset=EUC-KR">
<title>Insert title here</title>
</head>
<body>
<h1> Users</h1>
</body>
</html>