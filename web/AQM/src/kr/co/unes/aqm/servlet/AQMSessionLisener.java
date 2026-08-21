package kr.co.unes.aqm.servlet;

import javax.servlet.http.HttpSession;
import javax.servlet.http.HttpSessionEvent;
import javax.servlet.http.HttpSessionListener;

import org.slf4j.LoggerFactory;

public class AQMSessionLisener implements HttpSessionListener 
{
    private int sessionCount = 0;
    final org.slf4j.Logger logger = LoggerFactory.getLogger(AQMSessionLisener.class);
    
    @Override
    public void sessionCreated(HttpSessionEvent event)
    {
        synchronized (this)
        {
            sessionCount++;
        }     
 
        logger.debug("Session Created: " + event.getSession().getId());
        logger.debug("Total Sessions: " + sessionCount);
    }
 
    @Override
    public void sessionDestroyed(HttpSessionEvent event)
    {
        synchronized (this)
        {
            sessionCount--;
            if(sessionCount < 0)
            	sessionCount = 0;
        }
        
        HttpSession session = event.getSession();
        AQMLoginManager.getInstance().removeLoginSession(session);;
        
        logger.debug("Session Destroyed: " + session.getId());
        logger.debug("Total Sessions: " + sessionCount);
    }
}
