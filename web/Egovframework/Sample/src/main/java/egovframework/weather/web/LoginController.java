package egovframework.weather.web;

import javax.annotation.Resource;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.stereotype.Controller;
import org.springframework.ui.ModelMap;
import org.springframework.web.bind.annotation.ModelAttribute;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;

import egovframework.weather.service.LoginVO;
import egovframework.weather.service.LoginService;
import egovframework.weather.common.EgovMessageSource;

@Controller
public class LoginController {
	/** EgovLoginService */
	@Resource(name = "loginService")
	private LoginService loginService;

	/** EgovMessageSource */
	@Resource(name = "egovMessageSource")
	EgovMessageSource egovMessageSource;
	
	private String getUserID(HttpServletRequest request)
	{
		return (String)request.getSession().getAttribute("userID");
	}
	
	/**
	 * 로그인 화면으로 들어간다
	 * @param vo - 로그인후 이동할 URL이 담긴 LoginVO
	 * @return 로그인 페이지
	 * @exception Exception
	 */
	@RequestMapping(value = "/loginMain.do")
	public String loginMain(@ModelAttribute("loginVO") LoginVO loginVO, HttpServletRequest request, HttpServletResponse response, ModelMap model) throws Exception {		
		//request.getSession().setAttribute("userID", "");
		//System.out.println("loginUsrView, userID : " + getUserID(request) + ", " + request.getSession().getId());
		//String userID = getUserID(request);
		
		//if (userID != null && userID != "")
		//	return "/main.do";
		
		return "/login/g1LoginSwitch";
	}
	
	/**
	 * 로그인 화면으로 들어간다
	 * @param vo - 로그인후 이동할 URL이 담긴 LoginVO
	 * @return 로그인 페이지
	 * @exception Exception
	 */
	@RequestMapping(value = "/loginSwitch.do")
	public String loginSwitch(@ModelAttribute("loginVO") LoginVO loginVO2, HttpServletRequest request, HttpServletResponse response, ModelMap model) throws Exception {
		//request.getSession().setAttribute("userID", "");
		//System.out.println("loginUsrView, userID : " + getUserID(request) + ", " + request.getSession().getId());
		//String userID = getUserID(request);
		
		//if (userID != null && userID != "")
		//	return "/main.do";
		LoginVO loginVO = (LoginVO)request.getSession().getAttribute("LoginVO"); 
		
	    if(loginVO == null || loginVO.getStatus() != LoginVO.LoginStatus.SUCCESS)
	    {
	    	return "forward:/loginPage.do";
	    }
		
	    return "forward:/main.do";
		//return "/login/g1LoginSwitch";
	}
	
	/**
	 * 로그인 화면으로 들어간다
	 * @param vo - 로그인후 이동할 URL이 담긴 LoginVO
	 * @return 로그인 페이지
	 * @exception Exception
	 */
	@RequestMapping(value = "/loginPage.do")
	public String loginUserView(@ModelAttribute("loginVO") LoginVO loginVO, HttpServletRequest request, HttpServletResponse response, ModelMap model) throws Exception {
		//request.getSession().setAttribute("userID", "");
		//System.out.println("loginUsrView, userID : " + getUserID(request) + ", " + request.getSession().getId());
		//String userID = getUserID(request);
		
		//if (userID != null && userID != "")
		//	return "/main.do";
		
		return "/login/g1Login";
		//return "forward:/login/g1Login";
	}

	/**
	 * 일반 로그인을 처리한다
	 * @param vo - 아이디, 비밀번호가 담긴 LoginVO
	 * @param request - 세션처리를 위한 HttpServletRequest
	 * @return result - 로그인결과(세션정보)
	 * @exception Exception
	 */
	@RequestMapping(value = "/actionLogin.do")
	public String actionLogin(@ModelAttribute("loginVO") LoginVO loginVO, HttpServletRequest request, ModelMap model) throws Exception {

		System.out.println("actionLogin Input : " + loginVO.getStatus());
		LoginVO result = loginService.actionLogin(loginVO);
		
		request.getSession().setAttribute("LoginVO", result);
		
		if (result.getStatus() == LoginVO.LoginStatus.SUCCESS)
		{
			request.getSession().setAttribute("userID", result.getId());
			System.out.println("actionLogin, userID : " + getUserID(request) + ", " + request.getSession().getId());
			request.getSession().setAttribute("userGrade", result.getGrade().toString());
			request.getSession().setAttribute("LoginVO", result);
			return "forward:/main.do";
		}
		
		request.getSession().setAttribute("userID", "");
		System.out.println("actionLogin, userID : " + getUserID(request));
		model.addAttribute("message", egovMessageSource.getMessage("fail.login"));
		return "forward:/loginPage.do";

		/*boolean loginPolicyYn = true;

		if (resultVO != null && resultVO.getId() != null && !resultVO.getId().equals("") && loginPolicyYn) {

			request.getSession().setAttribute("LoginVO", resultVO);
			return "forward:/cmm/main/mainPage.do";
		} else {

			model.addAttribute("message", egovMessageSource.getMessage("fail.login"));
			return "cmm/uat/uia/EgovLoginUsr";
		}*/

	}

	/**
	 * 로그아웃한다.
	 * @return String
	 * @exception Exception
	 */
	@RequestMapping(value = "/actionLogout.do")
	public String actionLogout(HttpServletRequest request, ModelMap model) throws Exception {

		request.getSession().removeAttribute("LoginVO");
		request.getSession().removeAttribute("userID");
		request.getSession().invalidate();
		System.out.println("actionLogout, userID : " + getUserID(request) + ", " + request.getSession().getId());
		//RequestContextHolder.getRequestAttributes().removeAttribute("LoginVO", RequestAttributes.SCOPE_SESSION);

		return "forward:/loginPage.do";
	}
}
