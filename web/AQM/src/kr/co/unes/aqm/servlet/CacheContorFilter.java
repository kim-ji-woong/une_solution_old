package kr.co.unes.aqm.servlet;

import java.io.IOException;
import java.util.Enumeration;

import javax.servlet.Filter;
import javax.servlet.FilterChain;
import javax.servlet.FilterConfig;
import javax.servlet.ServletException;
import javax.servlet.ServletRequest;
import javax.servlet.ServletResponse;
import javax.servlet.http.HttpServletResponse;

public class CacheContorFilter implements Filter {
	
	private FilterConfig fc;
	@Override
	public void doFilter(ServletRequest req,
	                       ServletResponse res,
	                       FilterChain chain)
	                       throws IOException,
	                              ServletException {
		HttpServletResponse response =
		  (HttpServletResponse) res;
		// set the provided HTTP response parameters
		response.setHeader("Cache-Control", "no-cache, no-store, must-revalidate"); // HTTP 1.1.
        response.setHeader("Pragma", "no-cache"); // HTTP 1.0.
        response.setDateHeader("Expires", 0);
        
		// pass the request/response on
    	chain.doFilter(req, response);
	}
	@Override
	public void init(FilterConfig filterConfig) {
		this.fc = filterConfig;
	}
	
	@Override
	public void destroy() {
		this.fc = null;
	}

}
