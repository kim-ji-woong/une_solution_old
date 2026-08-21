package kr.co.unes.data;

import java.util.Date;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpSession;

public class ClientData {

	public Date LastQueryTime;
		
	public ClientData(HttpServletRequest request, HttpSession session) {
		this.request = request;
		this.session = session;
	}

	private boolean bAutoCommit = true;
	public boolean IsTransaction() {
		return !bAutoCommit;
	}
	
	public void setAutoCommit(boolean isAutoCommit) {
		bAutoCommit = isAutoCommit;	
	}

	private HttpServletRequest request;
	public HttpServletRequest getRequest() {
		return request;
	}
	public void setRequest(HttpServletRequest request) {
		this.request = request;		
	}

	private HttpSession session;
	public HttpSession getSession() {
		return session;
	}

	private String driverName = "";
	public String getDriverClassName() {
		return driverName;
	}
	public void setDriverClassName(String szName)
	{
		driverName = szName;
	}	
	
	private String szUserID = "";
	public void setDBUser(String szUser) {
		szUserID = szUser;
	}
	
	public String getDBUser() {
		return szUserID;
	}

	private String jdbcURL = "";
	public String getJdbcURL() {
		return jdbcURL;
	}

	public void setJdbcURL(String jdbcURL) {
		this.jdbcURL = jdbcURL;
	}

	private String dbPass = "";
	public String getDbPass() {
		return dbPass;
	}

	public void setDbPass(String dbPass) {
		this.dbPass = dbPass;
	}

	private String batchCmd = "";
	public void setBatchCmd(String strCMD) {
		batchCmd = strCMD;		
	}

	public String getBatchCmd() {
		return batchCmd;
	}
	
}
