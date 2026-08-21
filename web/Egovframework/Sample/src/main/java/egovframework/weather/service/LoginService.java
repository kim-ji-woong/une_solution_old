package egovframework.weather.service;

import egovframework.weather.service.LoginVO;

public interface LoginService {

	/**
	 * 일반 로그인을 처리한다
	 * @return LoginVO
	 *
	 * @param vo    LoginVO
	 * @exception Exception Exception
	 */
	LoginVO actionLogin(LoginVO vo) throws Exception;

	/**
	 * 비밀번호를 변경한다.
	 *
	 * @param vo    LoginVO
	 * @exception Exception Exception
	 */
	void updatePassword(LoginVO vo) throws Exception;
}