package g1Weather.common;

import javax.servlet.ServletContext;

import org.springframework.web.context.ServletContextAware;

import egovframework.rte.ptl.mvc.tags.ui.pagination.AbstractPaginationRenderer;

public class G1ImgPaginationRenderer extends AbstractPaginationRenderer implements ServletContextAware {
	private ServletContext servletContext;

	public G1ImgPaginationRenderer() {
	}

	/**
	* PaginationRenderer
	*
	* @see 개발프레임웍크 실행환경 개발팀
	*/
	public void initVariables() {

		firstPageLabel = "<li><a href=\"#\" class=\"btnBlueMini\" onclick=\"{0}({1}); return false;\"><input type=\"submit\" value=\"처음\" /></a></li>&#160;";
		previousPageLabel = "<li><a href=\"#\" class=\"btnBlueMini\" onclick=\"{0}({1}); return false;\"><input type=\"submit\" value=\"< 이전\" /></a></li>&#160;";
		currentPageLabel = "<li><label class=\"numTxt\"><strong>{0}</strong></label></li>";
		otherPageLabel = "<li><a href=\"#\" class=\"numTxt\" onclick=\"{0}({1}); return false;\">{2}</a></li>";
		nextPageLabel = "&#160;<li><a href=\"#\" class=\"btnBlueMini\" onclick=\"{0}({1}); return false;\"><input type=\"submit\" value=\"다음  >\" /></a></li>&#160;";
		lastPageLabel = "<li><a href=\"#\" class=\"btnBlueMini\" onclick=\"{0}({1}); return false;\"><input type=\"submit\" value=\"마지막\" /></a></li>";
		/*firstPageLabel = "<a href=\"#\" onclick=\"{0}({1}); return false;\">" + "<image src='" + servletContext.getContextPath() + "/images/common/btn_page_pre10.gif' border=0/></a>&#160;";
		previousPageLabel = "<a href=\"#\" onclick=\"{0}({1}); return false;\">" + "<image src='" + servletContext.getContextPath() + "/images/common/btn_page_pre1.gif' border=0/></a>&#160;";
		currentPageLabel = "<strong>{0}</strong>&#160;";
		otherPageLabel = "<a href=\"#\" onclick=\"{0}({1}); return false;\">{2}</a>&#160;";
		nextPageLabel = "<a href=\"#\" onclick=\"{0}({1}); return false;\">" + "<image src='" + servletContext.getContextPath() + "/images/common/btn_page_next1.gif' border=0/></a>&#160;";
		lastPageLabel = "<a href=\"#\" onclick=\"{0}({1}); return false;\">" + "<image src='" + servletContext.getContextPath() + "/images/common/btn_page_next10.gif' border=0/></a>&#160;";*/
	}

	@Override
	public void setServletContext(ServletContext servletContext) {
		this.servletContext = servletContext;
		initVariables();
	}
}
