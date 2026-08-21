package kr.co.unes.aqm.servlet;


import java.util.HashMap;

import javax.inject.Named;
import javax.servlet.ServletContext;
import javax.servlet.http.HttpSession;

import org.slf4j.LoggerFactory;

@Named("LoginManager")
public class AQMLoginManager {

	private static AQMLoginManager instance = null;
	public static AQMLoginManager getInstance()
	{
		return instance;
	}
	
	public static AQMLoginManager createAQMLoginManager(ServletContext ctx)
	{
		if( instance == null)
			instance = new AQMLoginManager(ctx);
		return instance;
	}
	
	final org.slf4j.Logger logger = LoggerFactory.getLogger(AQMLoginManager.class);
	
	private HashMap<String, HttpSession> mLoginUserSessionList = new HashMap<String, HttpSession>();
	private ServletContext servletContext;
	private String mLoginString = "loginInfo#sdf234234sdfwerwer";
	private String mLoginName = "AqmAdminLoginInfoString2342893476093245";
	
	public boolean checkLogin(HttpSession session)
	{	
		if( isValidLoginSession(session))
		{
			String szLoginInfo = (String)session.getAttribute(mLoginName);
			if( szLoginInfo != null)
			{
				if( szLoginInfo.compareTo(mLoginString) == 0)
				{
					session.setMaxInactiveInterval(180);
					

					StackTraceElement[] stackTrace = new Exception().getStackTrace();
					String szFName = stackTrace[2].getMethodName();
					logger.debug("Check Success Session {"+szFName+"}: " + session.getId());
					return true;
				}
			}
		}
		session.removeAttribute(mLoginName);
		return false;
	}
	
	public void logoutAdmin(HttpSession session)
	{		
		logger.debug("Logout Session : " + session.getId());		
		session.removeAttribute(mLoginName);
		removeLoginSession(session);
	}
	
	private AQMLoginManager(ServletContext servletContext)
	{
		instance = this;
	}	
	
	public ServletContext getServletContext() {
		return servletContext;
	}
	
	public int getLoginCount()
	{
		return mLoginUserSessionList.size();
	}
	
	public boolean adminLogin(HttpSession session, int nPass)
	{
		if( nPass == 1111)
		{			
			logger.debug("Login Admin Session : " + session.getId());
			session.setAttribute(mLoginName, mLoginString );
			addLoginSession(session);
			return true;
		}
		return false;
	}

	public void addLoginSession(HttpSession session)
	{
		String key = session.getId();
		if( mLoginUserSessionList.containsKey(key))
		{
			mLoginUserSessionList.remove(key);
			mLoginUserSessionList.put(key,  session);			
		}
		else
		{
			mLoginUserSessionList.put(key,  session);
		}
		logger.debug("Add Admin Session : " + key);
		
		// 로그인시 최대시간은 3분으로 변경
		session.setMaxInactiveInterval(180);
	}
	
	public boolean isValidLoginSession(HttpSession session)
	{
		String key = session.getId();
		if(mLoginUserSessionList.containsKey(key))
			return true;		
		return false;
	}
	
	public void removeLoginSession(HttpSession session)
	{
		String key = session.getId();
		if(mLoginUserSessionList.containsKey(key))
		{
			mLoginUserSessionList.remove(key);
			logger.debug("Remove Admin Session : " + key);
		}
	}
}
