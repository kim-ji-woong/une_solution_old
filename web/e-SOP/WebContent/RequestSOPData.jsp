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
	  2 : Parameter 부족
	  4 : 연결된 SOP 없음
	*/
	request.setCharacterEncoding("UTF-8");	
	Connection conn = null;
	Statement stmt = null;
	ResultSet result = null;

	String strQuickButtonID = request.getParameter("QuickButtonID");
	String strActionStepID = request.getParameter("ActionStepID");
	String strSerialNumber = request.getParameter("SerialNumber");
	String strSiteID = request.getParameter("SiteID");
	
	if (strSerialNumber == null || strActionStepID == null || strQuickButtonID == null)
	{
		out.println("Begin Data");
		// Parameter 부족
		out.println("ErrorCode:[2]");
		out.println("End Data");
		out.println("Begin Info :1,1 End Info" );
	}
	else
	{
		String strDB = "SOP_3";
		String strType = "mysql";	
		
		String driver = "com.microsoft.sqlserver.jdbc.SQLServerDriver";
		String url = "jdbc:sqlserver://127.0.0.1:1433;DatabaseName="+strDB;
		String id = "sa";
		String pw = "9449966Ab";
		
		String strPort = request.getParameter("Port");
		String strHost = request.getParameter("Host");
		if(strHost == null)
			strHost = "127.0.0.1";

		String FunctionLength = "length";
		
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

			FunctionLength = "len";
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
				
				strSQL = "Select TemporaryNormalTeamID, TemporaryEmergencyTeamID from MobileAppUser where SerialNumber = '" + strSerialNumber + "'";
				result = stmt.executeQuery(strSQL);
				
				ResultSetMetaData resultMetaData = result.getMetaData();
				String strNormalTeamID = null, strEmergencyTeamID = null;
				
				if (resultMetaData.getColumnCount() > 0)
				{
					if (result.next())
					{
						try
						{
							strNormalTeamID = result.getString(1);
							strEmergencyTeamID = result.getString(2);
						}
						catch (Exception e)
						{
						}
					}
				}

				if (strActionStepID.equals("-1"))
				{
					strSQL = "Select DisasterName from OptionQuickButton where isNormal = 1 and ButtonID = " + strQuickButtonID + " and SiteID = " + strSiteID;
					result = stmt.executeQuery(strSQL);
					resultMetaData = result.getMetaData();

					String strSOPName = null;

					if (resultMetaData.getColumnCount() > 0)
					{
						if (result.next())
						{
							try
							{
								strSOPName = result.getString(1);
							}
							catch (Exception e)
							{
								strSOPName = null;
							}
						}
					}

					if (strSOPName != null)
					{
						String[] tokens = strSOPName.split("/");

						if (tokens.length >= 3)
						{
							String strCategoryName = tokens[0].trim();
							String strSubCategoryName = tokens[1].trim();
							String strDisasterName = tokens[2].trim();

							strSQL = "Select Disaster.ID, Disaster.VersionID from Disaster, subdisastercategory, DisasterCategory ";
							strSQL += "where Disaster.SubDisasterID = subdisastercategory.ID and subdisastercategory.DisasterID = DisasterCategory.ID ";
							strSQL += String.format("and DisasterCategory.CategoryName = '%s' and subdisastercategory.SubCategoryName = '%s' and Disaster.DisasterName = '%s'", strCategoryName, strSubCategoryName, strDisasterName);

							result = stmt.executeQuery(strSQL);
							resultMetaData = result.getMetaData();

							int nDisasterID = -1, nVersionID = -1;

							if (resultMetaData.getColumnCount() > 0)
							{
								while (result.next())
								{
									try
									{
										String strID = result.getString(1);
										String strVersionID = result.getString(2);

										int disasterID = Integer.parseInt(strID);
										int versionID = Integer.parseInt(strVersionID);

										if (versionID > nVersionID)
										{
											nVersionID = versionID;
											nDisasterID = disasterID;
										}
									}
									catch (Exception e)
									{
										break;
									}
								}
							}

							if (nDisasterID > 0)
							{
								strSQL = "Select ID from ActionStep where DisasterID = " + Integer.toString(nDisasterID);
								result = stmt.executeQuery(strSQL);
								resultMetaData = result.getMetaData();

								if (resultMetaData.getColumnCount() > 0)
								{
									if (result.next())
									{
										try
										{
											strActionStepID = result.getString(1);
										}
										catch (Exception e)
										{
											strActionStepID = "-1";
										}
									}
								}
							}
						}
					}
				}

				if (strActionStepID == null || strActionStepID.equals("-1"))
				{
					strSQL = "Update MobileAppUsingActionStep set ActionStepID = -1 where ID = 1";
					stmt.executeUpdate(strSQL);

					out.println("Begin Data");
					// 연결된 SOP 없음
					out.println("ErrorCode:[4]");
					out.println("End Data");
					out.println("Begin Info :1,1 End Info" );
				}
				else
				{
					strSQL = "Update MobileAppUsingActionStep set ActionStepID = " + strActionStepID + " where ID = 1";
					stmt.executeUpdate(strSQL);

					strSQL = "Select ID from UserDefinedTeam where TeamName = '전 부서' or TeamName = '전부서'";
					result = stmt.executeQuery(strSQL);
					resultMetaData = result.getMetaData();

					List<String> userDefinedTeams = new ArrayList();
					List<String> missionList = new ArrayList();
					String strNormalTeamCodition = strNormalTeamID == null ? "" : strNormalTeamID + "(0)";
					String _strNormalTeamCodition = strNormalTeamID == null ? "" : "-" + strNormalTeamID + "(0)";
					String strEmergencyTeamCodition = strEmergencyTeamID == null ? "" : strEmergencyTeamID + "(1)";
					String _strEmergencyTeamCodition = strEmergencyTeamID == null ? "" : "-" + strEmergencyTeamID + "(1)";

					if (resultMetaData.getColumnCount() > 0)
					{
						while (result.next())
						{
							try
							{
								String strID = result.getString(1);
								userDefinedTeams.add(strID + "(3)"); 
							}
							catch (Exception e)
							{
								break;
							}
						}
					}

					strSQL = "Select p.ID, p.text, p.TeamList from ActionStep as _as, StepMember as sm, Process as p ";
					strSQL += "where sm.ActionStepID = _as.ID and p.StepMemberID = sm.ID and _as.ID = " + strActionStepID + " and " + FunctionLength + "(p.TeamList) > 0";

					result = stmt.executeQuery(strSQL);
					resultMetaData = result.getMetaData();

					if (resultMetaData.getColumnCount() > 0)
					{
						while (result.next())
						{
							try
							{
								String strProcessID = result.getString(1);
								String strText = result.getString(2);
								String strTeamList = result.getString(3);

								String[] tokens = strTeamList.split(",");
								boolean find = false;

								for (int i=0;i<tokens.length;i++)
								{
									String strTeamID = tokens[i].trim();

									for (String strUserDefinedTeamID : userDefinedTeams)
									{
										if (strTeamID.equals(strUserDefinedTeamID))
										{
											find = true;
											break;
										}
									}

									if (find)
										break;

									if ((strNormalTeamCodition.length() > 0 && strNormalTeamCodition.equals(strTeamID)) ||
									    (_strNormalTeamCodition.length() > 0 && _strNormalTeamCodition.equals(strTeamID)))
									{
										find = true;
										break;
									}

									if ((strEmergencyTeamCodition.length() > 0 && strEmergencyTeamCodition.equals(strTeamID)) ||
									    (_strEmergencyTeamCodition.length() > 0 && _strEmergencyTeamCodition.equals(strTeamID)))
									{
										find = true;
										break;
									}
								}

								if (find)
									missionList.add(strProcessID + "_" + strText);
							}
							catch (Exception e)
							{
								break;
							}
						}
					}

					out.println("Begin Data");
					out.println("INT:[" + Integer.toString(missionList.size()) + "]");

					for (String strMission : missionList)
					{
						out.println("VARCHAR:[" + strMission + "]");
					}

					out.println("End Data");
					out.println("Begin Info :1," + Integer.toString(missionList.size() + 1) + " End Info" );
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