package egovframework.weather.service;

import java.io.Serializable;

public class LoginVO implements Serializable{
	
	public enum LoginStatus{ UNKNOWN, NOT_EXIST_ID, INVALID_PW, SUCCESS };
	
	public enum UserGrade
	{
		ADMIN(0), NORMAL_USER(1);
		
		private int value;
		
		private UserGrade(int value)
		{
			this.value = value;
		}
		
		public int getValue()
		{
			return this.value;
		}
		
		public static UserGrade toUserGrade(int grade)
		{
			for (UserGrade userGrade : UserGrade.values())
			{
				if (userGrade.value == grade)
					return userGrade;
			}
			
			return UserGrade.NORMAL_USER;
		}
	};
	
	/**
	 * 
	 */
	private static final long serialVersionUID = 82740044207618049L;
	
	/** 아이디 */
	private String id;
	/** 비밀번호 */
	private String password;
	/** 시군 */
	private String cityName;
	/** 소속명 */
	private String workPlace;
	/** 로그인 결과*/
	private LoginStatus status = LoginStatus.UNKNOWN;
	/** 로그인한 계정 등급*/
	private UserGrade userGrade = UserGrade.NORMAL_USER;
	private int nGrade = UserGrade.NORMAL_USER.getValue();

	/**
	 * id attribute 를 리턴한다.
	 * @return String
	 */
	public String getId() {
		return id;
	}
	/**
	 * id attribute 값을 설정한다.
	 * @param id String
	 */
	public void setId(String id) {
		this.id = id;
	}
	/**
	 * password attribute 를 리턴한다.
	 * @return String
	 */
	public String getPassword() {
		return password;
	}
	/**
	 * password attribute 값을 설정한다.
	 * @param password String
	 */
	public void setPassword(String password) {
		this.password = password;
	}
	/**
	 * 시군 값을 리턴한다.
	 * @return String
	 */
	public String getCityName() {
		return cityName;
	}
	/**
	 * 시군 값을 설정한다.
	 * @param cityName String
	 */
	public void setCityName(String cityName) {
		this.cityName = cityName;
	}
	/**
	 * 소속명을 리턴한다.
	 * @return String
	 */
	public String getWorkPlace() {
		return workPlace;
	}
	/**
	 * 소속명 값을 설정한다.
	 * @param workPlace String
	 */
	public void setWorkPlace(String workPlace) {
		this.workPlace = workPlace;
	}
	/**
	 * 로그인 상태를 리턴한다.
	 * @return LoginStatus
	 */
	public LoginStatus getStatus() {
		return status;
	}
	/**
	 * 로그인 상태를 설정한다.
	 * @param status LoginStatus
	 */
	public void setStatus(LoginStatus status) {
		this.status = status;
	}
	/**
	 * 로그인한 계정등급을 리턴한다.
	 * @return UserGrade
	 */
	public UserGrade getGrade() {
		userGrade = UserGrade.toUserGrade(nGrade);
		return userGrade;
	}
	/**
	 * 로그인한 계정등급을 설정한다.
	 * @param grade UserGrade
	 */
	public void setGrade(UserGrade grade) {
		this.userGrade = grade;
		this.nGrade = this.userGrade.getValue();
	}
	/**
	 * 로그인한 계정등급을 리턴한다.
	 * @return Integer
	 */
	public int getNGrade() {
		nGrade = userGrade.getValue();
		return nGrade;
	}
	/**
	 * 로그인한 계정등급을 설정한다.
	 * @param grade Integer
	 */
	public void setNGrade(int grade) {
		this.nGrade = grade;
		this.userGrade = UserGrade.toUserGrade(grade);
	}
}
