package kr.co.unes.aqm.config;

import kr.co.unes.aqm.servlet.AQMAverageDataWorker;
import kr.co.unes.aqm.view.AdminMapMultipartJjspView;
import kr.co.unes.aqm.view.AdminNodeJspView;
import kr.co.unes.aqm.view.AdminPostFtlView;
import kr.co.unes.aqm.view.HomeFtlView;
import kr.co.unes.aqm.view.ReviewFtlView;
import kr.co.unes.aqm.view.ReviewMainFtlView;
import kr.co.unes.aqm.view.SearchFtlView;
import kr.co.unes.aqm.webservice.AreaService;
import kr.co.unes.aqm.webservice.ConfigService;
import kr.co.unes.aqm.webservice.NodeService;
import kr.co.unes.aqm.webservice.SensorService;
import kr.co.unes.aqm.webservice.SiteService;
import kr.co.unes.aqm.view.InfoFtlView;
import kr.co.unes.aqm.view.MultiPartFtlView;

import org.glassfish.jersey.media.multipart.MultiPartFeature;
import org.glassfish.jersey.server.ResourceConfig;
import org.glassfish.jersey.server.mvc.MvcFeature;
import org.glassfish.jersey.server.mvc.jsp.JspMvcFeature;


public class AQMServiceConfig extends ResourceConfig
{

	public AQMServiceConfig()
	{
		
		property(MvcFeature.TEMPLATE_BASE_PATH, "/WEB-INF/ftl");
		property(AQMFreemarkerMvcFeature.ENCODING, "UTF-8");
		
		// FreemarkerViewProcess를 가장우선순위높도록 등록
		register(kr.co.unes.aqm.config.AQMFreemarkerViewProcessor.class, 1);		
		register(org.glassfish.jersey.server.mvc.MvcFeature.class);		
		register(kr.co.unes.aqm.config.AQMFreemarkerMvcFeature.class);		
		register(JspMvcFeature.class);
		//property(FreemarkerMvcFeature.CACHE_TEMPLATES, "true");
		
		
		// Register FTL View
		register(MultiPartFeature.class);
		register(MultiPartFtlView.class);		 
		register(SensorService.class);
		register(AreaService.class);
		register(NodeService.class);
		register(ConfigService.class);
		register(SiteService.class);		
		register(HomeFtlView.class);
		register(ReviewFtlView.class);
		register(ReviewMainFtlView.class);
		register(SearchFtlView.class);
		register(AdminPostFtlView.class);		
		register(InfoFtlView.class);
		
		// Register JSP View
		register(AdminNodeJspView.class);
		register(AdminMapMultipartJjspView.class);
		
		AQMSessionFactory.initSesseionFactory();
		
		AQMAverageDataWorker averageWorker = AQMAverageDataWorker.getInstance();
		averageWorker.beginWorker();
	}
}
