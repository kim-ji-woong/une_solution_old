
package kr.co.unes.controller;

import java.util.List;
import java.io.UnsupportedEncodingException;
import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;
import java.util.Locale;
import java.util.StringTokenizer;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpSession;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.SessionAttributes;
import org.springframework.web.context.request.RequestContextHolder;
import org.springframework.web.context.request.ServletRequestAttributes;

import kr.co.unes.data.ClientData;
import kr.co.unes.data.ClientDataManager;
import kr.co.unes.query.QueryExcutor;
import util.AES256Cipher;


@Controller
@SessionAttributes("sessionId")
public class DBQueryController {
	
	@SuppressWarnings("unused")
	private static final Logger logger = LoggerFactory.getLogger(DBQueryController.class);
	
	@Value("${db.ip}")
	private String szDbIP;
	
	@Value("${db.port}")
	private int nDBPort;
	
	@Value("${db.name}")
	private String szDatabaseName;	
	
	@Value("${db.type}")
	private String szDBType;
	
	@Value("${db.con}")
	private String szConString = "";
	
	private String szLoginID = "";
	private String szPasswd = "";
		

	private boolean bReadConnectionInfo = false;
	
	private ClientDataManager clientManager; 
	
	public DBQueryController()
	{		
		clientManager = ClientDataManager.getInstance();
	}
		
	private void ReadDBConnection()
	{		
		if(bReadConnectionInfo == true)
			return;
		
		try {
			String szTemp = AES256Cipher.AES_Decode(szConString);
			StringTokenizer token = new StringTokenizer(szTemp, "|", false);

			szLoginID = token.nextToken();
			szPasswd = token.nextToken();
			bReadConnectionInfo = true;
		} catch (Exception e) {
		} 
	}
	
	@RequestMapping(value = "/DBQuery2", method = RequestMethod.GET )
	public String DBQueryGet(Locale locale, Model model, final HttpSession session) {
		return DBQueryPost(locale, model, session);
	}
	
	@RequestMapping(value = "/DBQuery2", method = RequestMethod.POST)
	public String DBQueryPost(Locale locale, Model model, final HttpSession session) {
		
		// DB 커넥션 정보를 읽는다.
		if(bReadConnectionInfo == false)
			ReadDBConnection();		
		HttpServletRequest request = ((ServletRequestAttributes) RequestContextHolder
		        .getRequestAttributes()).getRequest();		
		try 
		{
			request.setCharacterEncoding("UTF-8");
		} catch (UnsupportedEncodingException e) {
			e.printStackTrace();
		}
				
		// Find Client Data
		String szSessionID = session.getId();		
		ClientData data = clientManager.FindClientData(szSessionID);
		if( data == null)
		{
			data = clientManager.AddClientData(request, session);
		}
		
		// Set Client Data
		if(!MakeClientData(request, session, data))
		{
			logger.info("MakeClientData Error");
			model.addAttribute("errorMsg", "Unsupport Database");
			return "ErrorPage";
		}
		
		// Get SQL Query
		String strSQL = request.getParameter("SQLQuery");
		if( strSQL == null || strSQL.equals(""))
		{
			model.addAttribute("errorMsg", "No SQL Statement");
			return "ErrorPage";
		}
		
	
		// Select Result Type		
		String strResultType = request.getParameter("ResultType");
		if( strResultType == null)
		{
			strResultType = "DBUTIL";
		}
		strResultType = strResultType.toUpperCase();
				
		// Excute Query
		QueryExcutor excutor = new QueryExcutor(data);
		List<String> queryResult = excutor.ExcuteQuery(strSQL);
				
		// Set Query Result
		String timeStamp = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss").format(data.LastQueryTime);
		model.addAttribute("locale", locale );
		model.addAttribute("serverTime", timeStamp );
		model.addAttribute("sessionId", session.getId());
		model.addAttribute("DBResult", queryResult);	

		return ("DBQuery" + strResultType);			
	}
	
	private boolean MakeClientData(HttpServletRequest request, HttpSession session, ClientData data)
	{
		data.setRequest(request);
		
		String strDB = request.getParameter("DatabaseName");
		if( strDB == null)
		{
			strDB = szDatabaseName;
		}
		String strType = request.getParameter("DatabaseType");
		if( strType == null)
		{
			strType = szDBType;
		}
		
		String strTS = request.getParameter("Transaction");		
		boolean isAutoCommit = true;
		if(strTS == null)
			strTS = "0";
		
		data.setAutoCommit(isAutoCommit);		
		
		Date dtNow = Calendar.getInstance().getTime();
		data.LastQueryTime = dtNow;
		
		String strPort = request.getParameter("Port");
		if(strPort == null)
		{
			strPort = ""+ nDBPort;
		}
		
		String strHost = request.getParameter("Host");
		if(strHost == null)
		{
			strHost = szDbIP;
		}		
		
		String driver = "com.microsoft.sqlserver.jdbc.SQLServerDriver";
		String url = "jdbc:sqlserver://"+ strHost + ":" + strPort + ";DatabaseName="+strDB;	
		String id = szLoginID;
		String pw = szPasswd;
		
		if(strType.equals("mysql"))
		{			
			driver = "com.mysql.jdbc.Driver";
			url = "jdbc:mysql://"+ strHost + ":"+ strPort + "/"+strDB + "?useUnicode=true&characterEncoding=utf8";
		}
		else if(strType.equals("sqlserver"))
		{
			driver = "com.microsoft.sqlserver.jdbc.SQLServerDriver";
			url = "jdbc:sqlserver://"+ strHost + ":" + strPort + ";DatabaseName="+strDB;
		}
		else
		{
			return false;
		}
				
		data.setDriverClassName(driver);
		data.setJdbcURL( url );			
		data.setDBUser(id);
		data.setDbPass(pw);	
		
		return true;
	}	
}
