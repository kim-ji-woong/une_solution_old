<%@ page language="java" contentType="text/html; charset=UTF-8" pageEncoding="UTF-8"%>
<%@ page import="java.sql.*"%>
<%@ page import="javax.sql.*"%>
<%@ page import="javax.naming.*"%>
<%@ page import="javax.sql.DataSource"%>
<%@ page import="java.io.*"%>
<%@ page import="java.util.*"%>
<%@ page import="sun.misc.*"%>
<%@ page import="util.AES256Cipher" %>
<%
	/*ERROR_CODE
	  2 : Parameter 부족
	  3 : 승인되지 않은 인증코드
	  4 : 유효기간이 지난 인증코드
	  5 : 이미 인증된 사용자
	*/
	request.setCharacterEncoding("UTF-8");	
	Connection conn = null;
	Statement stmt = null;
	ResultSet result = null;
	
	String strSerialNumber = request.getParameter("SerialNumber");
	String strDeviceID = request.getParameter("DeviceID");
	String strCertCode = request.getParameter("CertCode");
	
	//String szSS = "Ihsz9f/HBfTYQ3vyZfaw8Q==";
    //String ssPhoneNumber = AES256Cipher.AES_Decode(szSS);
	
	
	if (strSerialNumber == null || strDeviceID == null || strCertCode == null)
	{
		out.println("Begin Data");
		// Parameter 부족
		out.println("ErrorCode:[2]");
		out.println("End Data");
		out.println("Begin Info :1,1 End Info" );
	}
	else
	{
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
		
		String strSQL = "";
		
		try
		{
			java.text.DateFormat format = new java.text.SimpleDateFormat("yyyyMMddHHmmss");
			Calendar cal = Calendar.getInstance();
			
			String currentTime = format.format(cal.getTime());
			
			Class.forName(driver);
			conn = DriverManager.getConnection(url, id, pw);
			
			if(conn != null)
			{	
				conn.setAutoCommit(true);			
				stmt = conn.createStatement();
				
				strSQL = "Select ID from User where Mobile = 1 and SerialNumber = '" + strSerialNumber + "'";
				result = stmt.executeQuery(strSQL);
				
				ResultSetMetaData resultMetaData = result.getMetaData();
				boolean alreadyExistUser = false;

				if (resultMetaData.getColumnCount() > 0)
				{
					if (result.next())
					{
						try
						{
							String strID = result.getString(1);
							Integer.parseInt(strID);
							
							strSQL = "Update User set DeviceID = '" + strDeviceID + "' where ID = " + strID;
							stmt.executeUpdate(strSQL);
							
							alreadyExistUser = true;
						}
						catch (Exception e)
						{
						}
					}
				}
				
				if (alreadyExistUser)
				{
					out.println("Begin Data");
					// 이미 인증된 사용자
					out.println("ErrorCode:[5]");
					out.println("End Data");
					out.println("Begin Info :1,1 End Info" );
				}
				else
				{
					strSQL = "Select ID, UserName, PhoneNumber, CertCode, CertCodeLifeTime, MobileUserLevel, IsSms, UserGroupID from CertRequest where SerialNumber = '" + strSerialNumber + "'";
					result = stmt.executeQuery(strSQL);
					
					resultMetaData = result.getMetaData();
					int nCount = resultMetaData.getColumnCount();
	
					if (nCount == 8)
					{
						String strCertRequestID = "", strUserName = "", strDBCertCode = "";
						String strCertCodeLifeTime = "", strPhoneNumber = "", strMobileUserLevel = "", strIsSms = "";
						String strUserGroupID = "";
						
						if (result.next())
				 		{
							strCertRequestID = result.getString(1);
							strUserName = result.getString(2);
							strPhoneNumber = result.getString(3);
							strDBCertCode = result.getString(4);
							strCertCodeLifeTime = result.getString(5);
							strMobileUserLevel = result.getString(6);
							strIsSms = result.getString(7);
							strUserGroupID = result.getString(8);

							if (strUserGroupID == null)
								strUserGroupID = "NULL";
							
							if (strUserName == null || strDBCertCode == null || strCertCodeLifeTime == null
									|| strCertCode.equals(strDBCertCode) == false || strMobileUserLevel == null
									|| strCertRequestID == null || strIsSms == null)
							{
								out.println("Begin Data");
								// 승인되지 않은 인증코드
								out.println("ErrorCode:[3]");
								out.println("End Data");
								out.println("Begin Info :1,1 End Info" );
							}
							else
							{
								int nCompareResult = currentTime.compareTo(strCertCodeLifeTime);
								
								if (nCompareResult > 0)
								//if (currentTime > strCertCodeLifeTime)
								{
									strSQL = "Delete from CertRequest where ID = " + strCertRequestID;
									stmt.executeUpdate(strSQL);
									
									out.println("Begin Data");
									// 유효기간이 지난 인증코드
									out.println("ErrorCode:[4]");
									out.println("End Data");
									out.println("Begin Info :1,1 End Info" );
								}
								else
								{
									///// User Table의 Max ID 얻어오기
									int nID = 1;
									strSQL = "Select max(ID) from User";
									
									result = stmt.executeQuery(strSQL);
									resultMetaData = result.getMetaData();
									
									nCount = resultMetaData.getColumnCount();
									
									if (nCount > 0)
									{
										if (result.next())
										{
											String strValue = result.getString(1);
											if(strValue!= null)
												strValue = strValue.trim();
											
											try
											{
												int num = Integer.parseInt(strValue);
												nID = num + 1;
											}
											catch (NumberFormatException e)
											{
											}
										}
									}
									///////////////////////////////////////////////////
									// 전화번호를 이용하여  CompanyMemberID 또는 ExternalCompanyMemberID 얻어오기
									String strCompanyMemberID = "NULL";
									String strExternalMemberID = "NULL";
									
									String query1 = "SELECT cm.ID FROM companymember as cm WHERE cm.PhoneNumber = '" + strPhoneNumber+"'";
									ResultSet rs1 = stmt.executeQuery(query1);
									if( rs1.next())
									{
										strCompanyMemberID = rs1.getString(0);
									}
									rs1.close();
									
									String query2 = "SELECT ecm.ID FROM externalcompanymember as ecm WHERE ecm.PhoneNumber = '" + strPhoneNumber+"'";
									ResultSet rs2 = stmt.executeQuery(query2);
									if( rs2.next())
									{
										strExternalMemberID = rs2.getString(0);
									}
									rs2.close();
									
									if(strCompanyMemberID == null)
									{
										strCompanyMemberID = "NULL";
									}
									if(strExternalMemberID == null)
									{
										strExternalMemberID = "NULL";
									}
									
									///////////////////////////////////////////////////
									
									// User Table에 데이터 추가하기
									strSQL = "Insert into User (ID, UserName, Mobile, CompanyMemberID, ExternalMemberID, DeviceID, MobileUserLevel, PhoneNumber, SerialNumber, IsSms, UserGroup) ";
									strSQL += String.format("values (%d, '%s', 1, %s, %s, '%s', %s, '%s', '%s', %s, %s)",
											nID, strUserName, strCompanyMemberID, strExternalMemberID, strDeviceID, strMobileUserLevel, strPhoneNumber, strSerialNumber, strIsSms, strUserGroupID);
									stmt.executeUpdate(strSQL);
									///////////////////////////////////////////////////
									
									// CertRequest 데이터 삭제하기
									strSQL = "Delete from CertRequest where ID = " + strCertRequestID;
									stmt.executeUpdate(strSQL);
									///////////////////////////////////////////////////
									
									out.println("Begin Data");
									// 성공
									out.println("INT:[0]");
									out.println("End Data");
									out.println("Begin Info :1,1 End Info" );
								}
							}
				 		}	
					}
					else
					{
						out.println("Begin Data");
						// 승인되지 않은 인증코드
						out.println("ErrorCode:[3]");
						out.println("End Data");
						out.println("Begin Info :1,1 End Info" );
					}
				}
			}
		}
		catch(Exception e)
		{
		    log(url);
		    log("RequestCertCode " + strSQL);
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
				
	 			out.println("연결끊음");
	 		}
	 		catch(Exception e)
	 		{
	 			e.printStackTrace();
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