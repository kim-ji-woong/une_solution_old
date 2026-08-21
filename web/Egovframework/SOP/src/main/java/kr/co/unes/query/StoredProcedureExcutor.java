package kr.co.unes.query;

import kr.co.unes.data.*;

import java.sql.CallableStatement;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.ResultSet;
import java.sql.ResultSetMetaData;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpSession;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

public class StoredProcedureExcutor {

	
	private ClientData clientData = null;
	
	private boolean isAutoCommit;
	@SuppressWarnings("unused")
	private HttpServletRequest request;
	@SuppressWarnings("unused")
	private HttpSession session;
	private String driver;
	private String url;
	private String id;
	private String pw;
	
	@SuppressWarnings("unused")
	private static final Logger logger = LoggerFactory.getLogger(QueryExcutor.class);
	
	public StoredProcedureExcutor(ClientData data) {
		clientData = data;
		
		isAutoCommit = !clientData.IsTransaction();
		
		request = clientData.getRequest();
		session = clientData.getSession();
		
		driver = clientData.getDriverClassName();
		url = clientData.getJdbcURL();
		id = clientData.getDBUser();
		pw = clientData.getDbPass();		
	}

	public List<String> ExcuteQuery(String strSQL)
	{
		List<String> arResult = new ArrayList<String>();
		Connection conn = null;
		CallableStatement cstmt=null;
		ResultSet result = null;
	
		try{
			Class.forName(driver);
			conn = DriverManager.getConnection(url, id, pw);
			if(conn != null)
			{			
				conn.setAutoCommit(isAutoCommit);

				if(driver.contains("mysql"))
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
				
				arResult.add("Begin Data");			
				while(result.next())
		 		{
					for(int i = 0; i < nCount; i++)
					{					
						String colTypeName = resultMetaData.getColumnTypeName(i+1 % (nCount+1));
						String strValue = result.getString(i+1);
						if(strValue!= null)
							strValue = strValue.trim();
						arResult.add(colTypeName + "_*$#:[" + strValue + "]:#$*_");
					}
		 		}
				arResult.add("End Data");

				if(!isAutoCommit)
					conn.commit();
			}			
		}catch(Exception e){
			arResult.add("JDBC 드라이브 연결 오류-"+e);
			e.printStackTrace();
			if(!isAutoCommit)
			{
				try {
					conn.rollback();
				} catch (SQLException e1) {					
				}
			}
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
				arResult.add("연결끊음");
	 		}
	 		catch(Exception e){
	 			e.printStackTrace();
	 			if(!isAutoCommit)
	 			{
	 				try {
						conn.rollback();
					} catch (SQLException e1) {						
					}
	 				arResult.add("INT_*$#:[0]:#$*_");
	 			}
				try {
					conn.close();
				} catch (SQLException e1) {					
				}
	 		}
		}		
		return arResult;
	}
}
