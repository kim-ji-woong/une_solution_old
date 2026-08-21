
import javax.servlet.http.HttpSessionEvent;
import javax.servlet.http.HttpSessionListener;

import java.sql.Connection;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;


public class SessionListener implements HttpSessionListener 
{
	private static final Logger logger = LoggerFactory.getLogger(SessionListener.class);
	
    private int sessionCount = 0;
 
    public void sessionCreated(HttpSessionEvent event)
    {
        synchronized (this)
        {
            sessionCount++;
        }
 
        System.out.println("SOPSession Created: " + event.getSession().getId());
        System.out.println("SOPTotal Sessions: " + sessionCount);
        
        logger.info("SOP Session Created: " + event.getSession().getId());
        logger.info("SOP Total Sessions: " + sessionCount);
    }
 
    public void sessionDestroyed(HttpSessionEvent event)
    {
        synchronized (this)
        {
            sessionCount--;
        }
        Connection con = (Connection)event.getSession().getAttribute("DBConnection");
        if( con != null)
        {
        	try
        	{
        		
        		con.rollback();
        	}
        	catch(Exception e)
        	{        		
        	}  
        	
        	try
        	{
            	con.close();
        	}
        	catch(Exception e)
        	{        		
        	}   
        }

        System.out.println("SOP Session Destroyed: " + event.getSession().getId());
        System.out.println("SOP Total Sessions: " + sessionCount);
        
        logger.info("SOP Session Destroyed: " + event.getSession().getId());
        logger.info("SOP Total Sessions: " + sessionCount);
    }
}
