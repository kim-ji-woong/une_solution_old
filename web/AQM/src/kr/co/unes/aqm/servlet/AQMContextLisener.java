package kr.co.unes.aqm.servlet;

import javax.servlet.*;

import org.slf4j.LoggerFactory;


public class AQMContextLisener implements ServletContextListener {

	final org.slf4j.Logger logger = LoggerFactory.getLogger(AQMContextLisener.class);
	
	private AQMLoginManager manager = null;
	
	private AQMAverageDataWorker averageWorker = null;
	
	@Override
	public void contextDestroyed(ServletContextEvent arg0) {
		if( averageWorker != null)
		{
			averageWorker.stopWorker();
			averageWorker = null;
		}		
	}

	@Override
	public void contextInitialized(ServletContextEvent arg0)
	{		
			
		ServletContext ctx = arg0.getServletContext();
		if( ctx != null)
		{
			manager = AQMLoginManager.createAQMLoginManager(ctx);
			logger.debug("LoginManager " + manager);
			
			averageWorker = AQMAverageDataWorker.createAQMAverageDataWorker(ctx);
			
		}
	}
}
