package egovframework.weather.service.impl;

import egovframework.weather.service.LoginVO;
import egovframework.weather.service.LoginService;
import egovframework.rte.fdl.cmmn.EgovAbstractServiceImpl;

import javax.annotation.Resource;

import org.springframework.stereotype.Service;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;

@Service("loginService")
public class LoginServiceImpl extends EgovAbstractServiceImpl implements LoginService {

	@Resource(name = "loginDAO")
	private LoginDAO loginDAO;
	
	private BCryptPasswordEncoder passwordEncoder = new BCryptPasswordEncoder();

	/**
	 * 일반 로그인을 처리한다
	 * @param vo LoginVO
	 * @return LoginVO
	 * @exception Exception
	 */
	@Override
	public LoginVO actionLogin(LoginVO vo) throws Exception {

		// 1. 입력된 ID를 사용하여 DB로부터 데이터를 얻어온다.
		LoginVO result = loginDAO.actionLogin(vo);
		
		if (result == null)
		{
			result = new LoginVO();
			result.setStatus(LoginVO.LoginStatus.NOT_EXIST_ID);
		}
		else
		{
			// 2. 입력된 비밀번호와 DB값이 일치하는지 검사한다.
			//    DB값은 단방향으로 암호화 되어있다.
			boolean isSuccess = passwordEncoder.matches(vo.getPassword(), result.getPassword());
			// 3. 암호를 초기화시켜 다른곳에서 사용되지 못하도록 한 다.
			result.setPassword("");
			
			if (isSuccess)
				result.setStatus(LoginVO.LoginStatus.SUCCESS);
			else
				result.setStatus(LoginVO.LoginStatus.INVALID_PW);
		}

		return result;
		// 1. 입력한 비밀번호를 암호화한다.
		/*String enpassword = EgovFileScrty.encryptPassword(vo.getPassword(), vo.getId());
		vo.setPassword(enpassword);

		// 2. 아이디와 암호화된 비밀번호가 DB와 일치하는지 확인한다.
		LoginVO loginVO = loginDAO.actionLogin(vo);

		// 3. 결과를 리턴한다.
		if (loginVO != null && !loginVO.getId().equals("") && !loginVO.getPassword().equals("")) {
			return loginVO;
		} else {
			loginVO = new LoginVO();
		}

		return loginVO;*/
	}

	/**
	 * 비밀번호를 변경한다.
	 * @param vo LoginVO
	 * @exception Exception
	 */
	@Override
	public void updatePassword(LoginVO vo) throws Exception {

	}
}