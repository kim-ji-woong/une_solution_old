package kr.co.unes.aqm.config;

import java.io.IOException;
import java.io.OutputStream;
import java.io.OutputStreamWriter;
import java.io.Reader;
import java.nio.charset.Charset;
import java.util.HashMap;
import java.util.Map;

import javax.ws.rs.core.MediaType;
import javax.ws.rs.core.MultivaluedMap;

import javax.inject.Inject;
import javax.servlet.ServletContext;

import org.glassfish.jersey.internal.util.collection.Value;
import org.glassfish.jersey.internal.util.collection.Values;
import org.glassfish.jersey.server.ContainerException;
import org.glassfish.jersey.server.mvc.Viewable;
import org.glassfish.jersey.server.mvc.freemarker.FreemarkerConfigurationFactory;
import org.glassfish.jersey.server.mvc.freemarker.FreemarkerDefaultConfigurationFactory;
import org.glassfish.jersey.server.mvc.spi.AbstractTemplateProcessor;

import org.glassfish.hk2.api.ServiceLocator;

import org.jvnet.hk2.annotations.Optional;

import freemarker.template.Configuration;
import freemarker.template.Template;
import freemarker.template.TemplateException;


final class AQMFreemarkerViewProcessor extends AbstractTemplateProcessor<Template> {

    private final FreemarkerConfigurationFactory factory;
    private final javax.ws.rs.core.Configuration mConfig;
    private final ServletContext mServletContext;
    
    @Inject
    public AQMFreemarkerViewProcessor(final javax.ws.rs.core.Configuration config, final ServiceLocator serviceLocator,
                                   @Optional final ServletContext servletContext) {
        super(config, servletContext, "freemarker", "ftl");

        mConfig = config;
        mServletContext = servletContext;
        
        this.factory = getTemplateObjectFactory(serviceLocator, FreemarkerConfigurationFactory.class,
                new Value<FreemarkerConfigurationFactory>() {
                    @Override
                    public FreemarkerConfigurationFactory get() {
                        Configuration configuration = getTemplateObjectFactory(serviceLocator, Configuration.class,
                                Values.<Configuration>empty());
                        if (configuration == null) {
                            return new FreemarkerDefaultConfigurationFactory(servletContext);
                        } else {
                            return new FreemarkerSuppliedConfigurationFactory(configuration);
                        }
                    }
                });

    }

    @Override
    protected Template resolve(final String templateReference, final Reader reader) throws Exception {
        return factory.getConfiguration().getTemplate(templateReference);
    }

    @Override
    public void writeTo(final Template template, final Viewable viewable, final MediaType mediaType,
                        final MultivaluedMap<String, Object> httpHeaders, final OutputStream out) throws IOException {
        try {
            Object model = viewable.getModel();
            if (!(model instanceof Map)) {
                model = new HashMap<String, Object>() {{
                    put("model", viewable.getModel());                    
                }};               
            }           
            
            // Add Servlet Context
            Map tMap = (Map)model;
            if( tMap != null)
            {
            	tMap.put("Context", mServletContext);
            }            
           
            Charset encoding = setContentType(mediaType, httpHeaders);

            template.process(model, new OutputStreamWriter(out, encoding));
        } catch (TemplateException te) {
            throw new ContainerException(te);
        }
    }
}

final class FreemarkerSuppliedConfigurationFactory implements FreemarkerConfigurationFactory {
    private final Configuration configuration;
    public FreemarkerSuppliedConfigurationFactory(Configuration configuration) {
        this.configuration = configuration;
    }
    @Override
    public Configuration getConfiguration() {
        return configuration;
    }
}