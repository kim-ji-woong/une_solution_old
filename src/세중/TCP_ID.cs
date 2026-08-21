using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HSMS
{
	public class SERIAL_ID
	{
		public const byte STX = 0x02;
		public const byte ETX = 0x03;

		public const byte POLL = 0x03;
		public const byte ACK = 0x06;
		public const byte NACK = 0x15;
	}

	public class TCP_ID
	{
		public const byte ARE_YOU_THERE = 1;
		public const byte I_AM_HERE = 2;

        public const byte SENSOR_DATA = 10;
        public const byte SENSOR_DATA_LIST = 11;
        public const byte REMOVE_SENSORS = 12;

        public const byte ALARM_HISTORY = 20;
        public const byte ALARM_PROCESS_HISTORY = 21;
        // Client 최초 접속시 전달
        public const byte ALARM_PROCESS_HISTORY_LIST = 22;
        public const byte FINISH_ALARM = 23;

        public const byte GAS_ALARM = 24;
        public const byte FINISH_GAS_ALARM = 25;

        public const byte WHO_ARE_YOU = 100;
        public const byte WHO_I_AM = 101;

        public const byte CHANGE_DB_TIME = 199;
        public const byte CHANGE_DB_DATA = 200;
        public const byte CHANGE_DB_DATA_LIST = 201;


        public const byte LOGIN_USER = 81;          // 사용자 로그인
        public const byte ACCEPT_LOGIN = 82;		// 로그인 성공
        public const byte REJECT_LOGIN = 83;		// 로그인 실패
        public const byte CHECK_LOGIN = 84;		    // 로그인 상태 체크
        public const byte LOGOUT_USER = 85;			// 사용자 로그아웃
        public const byte JOIN_USER = 86;			// 사용자 등록
        public const byte CHNAGE_PASSWORD = 87;	    // 로그인된 사용자 비번 변경
        //public const byte SET_PASSWORD = 88;		// 사용자 이름과 사번으로 사용자 비번 변경
        //public const byte REQUEST_CODE = 89;	    // 로그인 사용자 등록키 발송
        public const byte DELETE_USER = 90;         // 사용자 삭제

	}

	public class TCP_TYPE
	{
		public const byte INTEGER = 20;
		public const byte FLOAT = 21;
		public const byte DOUBLE = 22;
		public const byte STRING = 23;
		public const byte LONG = 24;
        public const byte BOOLEAN = 25;
        public const byte SHORT = 26;
        public const byte BYTE = 27;
        // 년(2),월(1),일(1),시(1),분(1),초(1),milli Second(2)
        public const byte DATETIME = 28;
	}

    public class TCP_CLIENT
    {
        public const byte HSMS_CLIENT = 1;
    }

    public class ServerCommandType
    {
        //public const byte RUN_SDMS = 1;
    }
}
