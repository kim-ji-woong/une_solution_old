namespace SDMS
{
    //public class SERIAL_ID
    //{
    //    public const byte STX = 0x02;
    //    public const byte ETX = 0x03;

    //    public const byte POLL = 0x03;
    //    public const byte ACK = 0x06;
    //    public const byte NACK = 0x15;
    //}

	public class TCP_ID
	{
		public const byte ARE_YOU_THERE = 1;
		public const byte I_AM_HERE = 2;

		public const byte SENSOR_DATA = 3;
		public const byte SENSOR_ZONE_DATA = 4;
		//public const byte SENSOR_CONNECTION_DATA = 4;

		public const byte FIRE_DETECT_REPORT = 5;   // 화재 신고
		public const byte SENSOR_FAIL_REPORT = 6;
		public const byte MALFUNCTION_REPORT = 7;   // 오동작
        public const byte IGNORE_DETECT_REPORT = 8; // 화재신호 꺼짐
		public const byte CLEAR_DETECT_REPORT = 9;  // 상황 해제

		public const byte FIRE_DETECT_TRAINNING = 10;

		public const byte ALL_SENSOR_DATA_IN_RECIVER = 11;

        public const byte PSM_SENSOR_DATA = 12;
        public const byte PSM_DETECT_REPORT = 13;
        public const byte PSM_SENSOR_RESET = 14;
        public const byte PSM_BUZZER_STOP = 15;

        public const byte PSM_DETECT_BROADCAST = 16;
        public const byte PSM_REPORT_BROADCAST = 17;

        public const byte TEST_SENSOR_DATA = 18;
        public const byte TEST_PSM_SENSOR_DATA = 19;

        
        public const byte EDIT_SENSOR_ZONE = 21;

		//public const byte SENSOR_HISTORY_ID = 31;
		public const byte SENSOR_REACTION_HISTORY_DATA = 32;

		public const byte SENSOR_REACTION_HISTORY_DATA_LIST = 33;
		public const byte REQUEST_SENSOR_REACTION_HISTORY_DATA_LIST = 34;

		//public const byte SENSOR_HISTORY_ID_LIST = 33;

		public const byte RUN_SOP = 41;
		public const byte RUN_N_CANCEL_SOP = 42;
		public const byte FINISH_SOP = 43;
		public const byte IGNORE_SOP = 44;

		public const byte FIRE_SENSOR_SIGNAL = 50;

		public const byte RECIVER_CONNECT = 52;
		public const byte RECIVER_DISCONNECT = 53;
		public const byte ALL_RECIVER_STATE = 54;

		//public const byte CHANGE_FACILITY_MANAGER = 60; // 관리자 정보 변경
		//public const byte CHANGE_EQUIPZONE_CCTV = 61;   // CCTV 정보 변경

		public const byte REQUEST_RESTORE = 70;   // 복원 요청
		public const byte REJECT_RESTORE = 71;    // 복원 요청 거절
		public const byte ACCEPT_RESTORE = 72;	  // 복원 요청 승인
		public const byte BEGEIN_RESTORE = 73;    // 복원 작업 시작
		public const byte END_RESTORE = 74;       // 복원 작업 종료	, 모두 재시작

        public const byte CHANGE_SOPGENUSER_COMMANDER = 80; //SOPGenUserCommander 수정

		public const byte LOGIN_USER = 81;          // 사용자 로그인
		public const byte ACCEPT_LOGIN = 82;		// 로그인 성공
		public const byte REJECT_LOGIN = 83;		// 로그인 실패
		public const byte CHECK_LOGIN = 84;		    // 로그인 상태 체크
		public const byte LOGOUT_USER = 85;			// 사용자 로그아웃
		public const byte JOIN_USER = 86;			// 사용자 등록
		public const byte CHNAGE_PASSWORD = 87;	    // 로그인된 사용자 비번 변경
		public const byte SET_PASSWORD = 88;		// 사용자 이름과 사번으로 사용자 비번 변경
		public const byte CHANGE_NICKNAME = 89;	    // 로그인된 사용자 별명 변경

        public const byte CANCEL_REQUEST_CONTROL = 90;// 제어권 요청취소
		public const byte REQUEST_CONTROL = 91;     // 제어권 요청
		public const byte RETURN_CONTROL = 92;      // 제어권 반납
		public const byte GIVE_CONTROL = 93;        // 제어권 부여
		public const byte CONFIRM_GIVE_CONTROL = 94;// 제어권 취득 확인
		public const byte TAKE_CONTROL = 95;        // 제어권 상실
		public const byte CONFIRM_TAKE_CONTROL = 96;// 제어권 상실 확인
		public const byte REJECT_REQUEST_CONTROL = 97;  // 제어권 요청 거부
		public const byte STEAL_CONTROL = 98;       // 제어권 뺏기
		public const byte GIVE_CONTROL_KEY = 99;    // 특정 사용자에게만 제어권 부여

		public const byte WHO_ARE_YOU = 100;
		public const byte WHO_I_AM = 101;

		public const byte CHANGE_CONFIG = 110;       // 설정 변경

        public const byte WEATHER_INFO = 120;        //  기후 정보
        public const byte EARTHQUAKE_SENSOR_DETECT = 121;   // 지진 정보
        public const byte COLLAPSE_BUILDING_DETECT = 122;   // 건물붕괴

        public const byte SENSOR_DATA_WITH_TAG = 124;
        public const byte SECURITY_DETECT_REPORT = 125;

		public const byte SOP_SELECT_MISSION = 200;  // SOP 미션 선택 전송
        public const byte SOP_CURRENT_SELECT_MISSION = 201; // SOP 현재 미션 선택 전송

        public const byte CHAGNE_WORK_MEMBER = 210;  // 근무조 변경

        public const byte SOP_SIMULATOR_COMMAND = 220;  // SOPSimulatorCommandType과 조합
        public const byte SDMS_COMMAND = 221;           // SDMSCommandType과 조합

        public const byte START_SERVER_FROM_MONITOR = 238;
        public const byte STOP_SERVER_FROM_MONITOR = 239;

		public const byte CHECK_ALL_SERVER = 240;
		public const byte SERVER_STATE = 241;

		public const byte START_SOP_SERVER = 242;
		public const byte STOP_SOP_SERVER = 243;

		public const byte START_TTS_SERVER = 244;
		public const byte STOP_TTS_SERVER = 245;

		public const byte START_SENSOR_MONITOR = 246;
		public const byte STOP_SENSOR_MONITOR = 247;

		public const byte START_BACKUP_LOG = 248;
		public const byte GET_BACKUP_LOG = 249;

		public const byte SERVER_COMMAND = 250;             // ServerCommandType과 조합
		public const byte INTERNAL_MESSAGE = 251;           // 통합관리자와 로컬 PC 내부간 통신
		public const byte TRAINING_SIMULATOR_COMMAND = 252; // TrainingSimulatorCommandType과 조합
	}

    //public class TCP_TYPE
    //{
    //    public const byte INTEGER = 20;
    //    public const byte FLOAT = 21;
    //    public const byte DOUBLE = 22;
    //    public const byte STRING = 23;
    //    public const byte LONG = 24;
    //    public const byte BOOLEAN = 25;
    //    public const byte SHORT = 26;
    //    public const byte BYTE = 27;
    //}

	public class TCP_CLIENT
	{
        public const byte ALL = 0;
		public const byte SDMS_CLIENT = 1;
		public const byte SOP_SIMULATOR = 2;
		public const byte SENSOR_SIMULATOR = 3;
		public const byte SENSOR_MONITOR = 4;
		public const byte SOP_RESTORE = 5;
		public const byte INTEGRATE_MANAGE = 6;
		public const byte SDMS_CLIENT_SECOND = 7;
		public const byte SERVER_MONITOR = 8;
		public const byte SENSOR_MONITOR2 = 9;
		public const byte SERVER_COMMANDER = 10;
		public const byte TRAINING_SIMULATOR = 11;  // 연습용 모드
        public const byte SOP_WEATHER = 12;
        public const byte PSM_SENSOR_SERVER = 13;
        public const byte PSM_LEVEL_SERVER = 14;
        public const byte EARTHQUAKE_SENSOR_SERVER = 15;

        public const byte SVMS_EVENT_RECIVER = 16;
        public const byte ACCESS_EVENT_RECIVER = 17;
        public const byte SAINTOP_EVENT_RECIVER = 18;
        public const byte ASIN_EVENT_RECIVER = 19;
        public const byte S1_TEST_SENSOR_SERVER = 20;
        public const byte S1_SECOM_EVENT_RECEIVER = 21;

        public const byte SOP_MANAGER = 22;

        public const byte UNKNOWN = 255;

        public static string GetClientTypeString(byte clientType)
        {
            switch (clientType)
            {
                case SDMS_CLIENT:
                    return "SDMS Client";

                case SOP_SIMULATOR:
                    return "SOP Simulator";

                case SENSOR_SIMULATOR:
                    return "Sensor Simulator";

                case SENSOR_MONITOR:
                    return "SENSOR_MONITOR";

                case SOP_RESTORE:
                    return "Restore Manager";

                case INTEGRATE_MANAGE:
                    return "통합관리자";

                case SDMS_CLIENT_SECOND:
                    return "SDMS Client Sub Line";

                case SERVER_MONITOR:
                    return "SERVER_MONITOR";

                case SENSOR_MONITOR2:
                    return "Sensor Monitor";

                case SERVER_COMMANDER:
                    return "Server Commander";

                case TRAINING_SIMULATOR:
                    return "TRAINING_SIMULATOR";

                case SOP_WEATHER:
                    return "기후정보 입력기";

                case PSM_SENSOR_SERVER:
                    return "PSM Server";

                case PSM_LEVEL_SERVER:
                    return "PSM_LEVEL_SERVER";

                case EARTHQUAKE_SENSOR_SERVER:
                    return "Earthquake Sensor Server";

                case SVMS_EVENT_RECIVER:
                    return "SVMS Event Receiver";

                case ACCESS_EVENT_RECIVER:
                    return "Access(S1) Event Receiver";

                case SAINTOP_EVENT_RECIVER:
                    return "EMPoll 비상벨 서버";

                case ASIN_EVENT_RECIVER:
                    return "아신 화재 서버";

                case S1_TEST_SENSOR_SERVER:
                    return "S1 Sensor Server";

                case S1_SECOM_EVENT_RECEIVER:
                    return "S1 Secom Event Receiver";

                case SOP_MANAGER:
                    return "SOP 생성기";
            }

            return "UNKNOWN";
        }
	}

	public class ServerCommandType
	{
		public const byte RUN_SDMS = 1;
		public const byte UPDATE_SYSTEM = 2;
        public const byte REQUEST_PSM_SENSOR_ALARM = 3;
        public const byte REQUEST_PSM_SENSOR_RESET = 4;
        public const byte REQUEST_PSM_BUZZER = 5;
        public const byte DELETE_SENSOR_TAG_HISTORY = 6;
        public const byte EQUIPMENTZONE_CHANGE_NAME = 7;
        public const byte REQUEST_PSM_TEST_ALARM = 8;
	}

    public class SOPSimulatorCommandType
    {
        public const byte RESET_USER_DEFINED_TEAM_NAMES = 1;
    }

	public class TrainingSimulatorCommandType
	{
		public const byte SEND_SDMS_SMS = 1;
	}

    public class SDMSCommandType
    {
        public const byte CHANGE_PSM_SENSOR_STATUS = 1;
        public const byte PSM_SENSOR_DATA = 2;
        public const byte REFRESH_PSM_SENSOR_LIFE_TIME = 3;
        public const byte SDMS_PUBLIC_MESSAGE = 4;
        public const byte SDMS_PUBLIC_MESSAGE_ID = 5;
        public const byte PSM_SENSOR_ALARM_LEVEL = 6;
        public const byte SET_VIEW = 7;
        public const byte CHANGE_TAG_ACTIVATION = 8;

        
    }
}