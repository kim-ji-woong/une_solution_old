package egovframework.weather.service.impl;

import egovframework.weather.service.*;

import org.springframework.stereotype.Repository;

@Repository("loginMapper")
public interface LoginMapper {
	/**
	 * Login을 진행한다.
	 * @param data - Login 정보(ID와 비밀번호)
	 * @return 성공할 경우 성공한 Login 정보
	 *         실패할 경우 null
	 * @exception Exception
	 */
	LoginVO actionLogin(LoginVO data) throws Exception;
	
	/**
	 * 비밀번호를 변경한다.
	 * @param data - 변경할 비밀번호가 담겨있다.
	 * @exception Exception
	 */
	void updatePassword(LoginVO data) throws Exception;
}
