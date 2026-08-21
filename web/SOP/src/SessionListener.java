
import javax.servlet.http.HttpSessionEvent;
import javax.servlet.http.HttpSessionListener;

import java.sql.Connection;

public class SessionListener implements HttpSessionListener 
{
    private int sessionCount = 0;
 
    public void sessionCreated(HttpSessionEvent event)
    {
        synchronized (this)
        {
            sessionCount++;
        }
 
        System.out.println("Session Created: " + event.getSession().getId());
        System.out.println("Total Sessions: " + sessionCount);
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
        
        System.out.println("Session Destroyed: " + event.getSession().getId());
        System.out.println("Total Sessions: " + sessionCount);
    }
}
