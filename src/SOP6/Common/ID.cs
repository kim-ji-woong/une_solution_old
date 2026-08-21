namespace SOPWebServer
{
	public class Header
	{
		public const int CLOSE_CONNECTION = 0;
        public const int ARE_YOU_THERE = 1;
        public const int I_AM_HERE = 2;

        // 로그인 요청
        public const int LOGIN_USER = 10;
        // 로그인 성공
        public const int ACCEPT_LOGIN = 11;
        // 로그인 실패
        public const int REJECT_LOGIN = 12;
        // 로그인 상태 체크
        public const int CHECK_LOGIN = 13;
        // 사용자 로그아웃
        public const int LOGOUT_USER = 14;
        // 사용자 등록
        public const int JOIN_USER = 15;
        // 로그인된 사용자 비번 변경
        public const int CHANGE_PASSWORD = 16;
        // 사용자 이름과 사번으로 사용자 비번 변경
        public const int SET_PASSWORD = 17;
        // 로그인된 사용자 별명 변경
        public const int CHANGE_NICKNAME = 18;
        public const int CHANGE_SOPGENUSER_COMMANDER = 19;

        // 설정 변경
        public const int CHANGE_CONFIG = 25;

        // 통합관리자와 통신하는 내부 메시지
        public const int INTERNAL_MESSAGE = 30;

        public const int RUN_SOP = 41;
        public const int RUN_N_CANCEL_SOP = 42;
        public const int FINISH_SOP = 43;
        public const int IGNORE_SOP = 44;
        public const int ALARM_SOP_RESULT = 45;

        // 복원 요청
        public const int REQUEST_RESTORE = 70;
        // 복원 요청 거절
        public const int REJECT_RESTORE = 71;
        // 복원 요청 승인
        public const int ACCEPT_RESTORE = 72;
        // 복원 작업 시작
        public const int BEGEIN_RESTORE = 73;
        // 복원 작업 종료, 모두 재시작
        public const int END_RESTORE = 74;

        // SOP 미션 선택 전송
        public const int SOP_SELECT_MISSION = 80;
        public const int CHANGE_WORK_MEMBER = 81;
        // SOP 현재 미션 선택 전송
        public const int SOP_CURRENT_SELECT_MISSION = 82;
        // 새로운 ComponentHistory 요청
        public const int REQUEST_COMPONENT_HISTORY_LIST = 83;
        // REQUEST_COMPONENT_HISTORY_LIST에 대한 응답
        public const int RESPONSE_COMPONENT_HISTORY_LIST = 84;
        // 실행중인 SOP의 특정 Component 선택
        public const int SELECT_SOP_COMPONENT = 85;
        // SOP에(ActionStepHistory) 대한 제어권 부여
        public const int SEND_SOP_CONTROL= 87;
        // SOP에 대한 제어권을 받았음을 확인
        public const int CONFIRM_SOP_CONTROL = 88;
        // 특정 사용자에게만 제어권 부여
        public const int GIVE_CONTROL_KEY = 89;
        // 제어권 요청취소
        public const int CANCEL_REQUEST_CONTROL = 90;
        // 제어권 요청
        public const int REQUEST_CONTROL = 91;
        // 제어권 반납
        public const int RETURN_CONTROL = 92;
        // 제어권 부여
        public const int GIVE_CONTROL = 93;
        // 제어권 취득 확인
        public const int CONFIRM_HAS_CONTROL = 94;
        // 제어권 소유자 알림
        public const int CONTROL_CLIENT = 95;
        // 제어권 요청 거부
        public const int REJECT_REQUEST_CONTROL = 96;
        // 제어권 뺏기
        public const int STEAL_CONTROL = 97;

        // 클라이언트 정보 요청
        public const int REQUEST_CLIENT_INFO = 98;
        // 클라이언트 정보 응답
        public const int REPLY_CLIENT_INFO = 99;

        // 탐지신호
        public const int SENSOR_DATA = 100;
        // 탐지신호(테스트)
        public const int SENSOR_DATA_TEST = 101;
        // 오동작처리
        public const int SENSOR_MALFUNCTION = 102;
        // 신호복구
        public const int SENSOR_USER_RESET = 103;
        // 재난신고
        public const int NOTIFY_DISASTER = 104;

        // 상황해제
        public const int CLEAR_DETECT_REPORT = 105;
        // SOPSimulator에게 전달하는 센서신호
        public const int SENSOR_SIGNAL_FOR_SOP = 106;
        // SensorZone 정보
        public const int SENSOR_ZONE_DATA = 107;
        // 화재신호 꺼짐
        public const int IGNORE_DETECT_REPORT = 108;

        // 모든 신호 해제
        public const int CLEAR_DETECT_ALL = 109;

        // 현재의 알람정보 요청
        public const int REQUEST_SENSOR_REACTION_HISTORY_DATA_LIST = 110;
        // 현재의 알람정보 전송
        public const int SENSOR_REACTION_HISTORY_DATA_LIST = 111;
        public const int EDIT_SENSOR_ZONE = 112;
        public const int SENSOR_REACTION_HISTORY_DATA = 113;
        // SOP Simulator에게 전달하기 위한 알람정보
        public const int ALARM_DATA_LIST = 114;
        // SOP 생성기에 의하여 ActionStepHistory가 삭제되었다.
        public const int DELETE_ACTIONSTEP_HISTORY = 115;
        // 센서신호에 의한 SOP 실행허가를 요청(SOPSimulator => Server)
        public const int REQUEST_SENSOR_SOP_PERMIT = 116;
        // REQUEST_SENSOR_SOP_PERMIT에 대한 응답(Server => SOPSimulator)
        public const int RESPONSE_SENSOR_SOP_PERMIT = 117;
        public const int ALARM_DATA_LIST2 = 118;

        public const int RESET_MAX_ACTIONSTEP_HISTORY_ID = 119;

        // 기후 정보
        public const int WEATHER_INFO = 120;        

        // 지진 정보
        public const int EARTHQUAKE_SENSOR_DETECT = 121;
        // 건물붕괴
        public const int COLLAPSE_BUILDING_DETECT = 122;

        public const int SENSOR_DATA_WITH_TAG = 124;
        // 기타 센서
        public const int ETC_SENSOR_DETECT = 125;
        public const int ETC_SENSOR_DATA_INT = 126;
        public const int ETC_SENSOR_DATA_DOUBLE = 127;
        public const int ETC_SENSOR_DATA_STRING = 128;

        // 탐지신호(여러개의 센서신호를 한꺼번에 묶어서 보낸다.)
        public const int SENSOR_DATAS = 130;
        // 탐지신호(테스트, 여러개의 센서신호를 한꺼번에 묶어서 보낸다.)
        public const int SENSOR_DATAS_TEST = 131;

        public const int START_TTS_SERVER = 150;
        public const int STOP_TTS_SERVER = 151;

        public const int RECEIVER_CONNECT = 200;
        public const int RECEIVER_DISCONNECT = 201;
        public const int ALL_RECEIVER_STATE = 204;

        public const int START_SERVER_FROM_MONITOR = 238;
        public const int STOP_SERVER_FROM_MONITOR = 239;

        public const int CHECK_ALL_SERVER = 240;
        public const int SERVER_STATE = 241;

        public const int START_SOP_SERVER = 242;
        public const int STOP_SOP_SERVER = 243;

        public const int START_SENSOR_MONITOR = 246;
        public const int STOP_SENSOR_MONITOR = 247;

        public const int START_BACKUP_LOG = 248;
        public const int GET_BACKUP_LOG = 249;

        // ServerCommandType과 조합
        public const int SERVER_COMMAND = 250;
        // SDMSCommandType과 조합
        public const int SDMS_COMMAND = 251;
        // SOPSimulatorCommandType과 조합
        public const int SOP_SIMULATOR_COMMAND = 252;
        // TrainingSimulatorCommandType과 조합
        public const int TRAINING_SIMULATOR_COMMAND = 253; 
        public const int PSM_BUZZER_STOP = 254;

        public const int ALARM_STEP = 300;

        // 문자메시지
        public const int SEND_SMS = 10000;

        // 수동신고를 위한 Zone ID
        // ex) ManualReportDefaultID + FacilityType
        //     화재 : ManualReportDefaultID + FacilityType.FIRE_SENSOR = 1000000
        //     누출 : ManualReportDefaultID + FacilityType.PSM_SENSOR = 1000011
        public const int ManualReportDefaultID = 1000000;
    }

    public class ClientType
    {
        public const int FIRE_SENSOR_SERVER = 1;
        public const int PSM_SENSOR_SERVER = 2;
        public const int SECURITY_SENSOR_SERVER = 3;
        public const int EARTHQUAKE_SENSOR_SERVER = 4;
        public const int LOGIN_SERVER = 5;
        public const int TEMPERATURE_HUMIDITY_SERVER = 6;
        public const int BLACKOUT_SERVER = 7;

        public const int SDMS = 100;
        public const int SOP_SIMULATOR = 101;
        public const int SOP_MANAGER = 102;
        public const int SOP_COMMANDER = 103;

        public const int ETC = 200;

        public static string ToString(int nClientType)
        {
            switch (nClientType)
            {
                case FIRE_SENSOR_SERVER:
                    return "FireSensorServer";

                case PSM_SENSOR_SERVER:
                    return "PSMSensorServer";

                case SECURITY_SENSOR_SERVER:
                    return "SecuritySensorServer";

                case EARTHQUAKE_SENSOR_SERVER:
                    return "EarthquakeSensorServer";

                case LOGIN_SERVER:
                    return "LoginServer";

                case TEMPERATURE_HUMIDITY_SERVER:
                    return "온/습도 서버";
                case BLACKOUT_SERVER:
                    return "정전 서버";

                case SDMS:
                    return "SDMS";

                case SOP_SIMULATOR:
                    return "SOP 시스템";

                case SOP_MANAGER:
                    return "SOP 생성기";
                case SOP_COMMANDER:
                    return "SOP Commander";

                case ETC:
                    return "기타";
            }

            return nClientType.ToString();
        }
    }

    public class ClientSubType
    {
        public const int SIMULATOR = 1;
        // 태흥전자(삼천포)
        public const int TH = 2;
        // 지멘스
        public const int SIEMENS = 3;
        public const int JUBIX = 4;
        public const int SENKO = 5;
        // 아신(서울대)
        public const int ASIN = 6;
        public const int S1_SVMS = 7;
        public const int S1_ACCESS = 8;
        public const int S1_SECOM = 9;
        // 서울대 외부비상벨
        public const int EMPOLL = 10;
        // 동방전자
        public const int JOHNSON_CONTROLS = 11;
        public const int SHINHAN = 12;
        public const int WOORIZEN = 13;

        public const int POWER_PLANT = 100;
        public const int UNIVERSITY = 101;
        public const int OFFICE_BUILDING = 102;

        public const int INTEGRATED_MANAGER = 110;
        public const int SOP_SIMULATOR = 111;
        public const int SOP_MANAGER = 112;
        public const int SOP_COMMANDER = 113;

        public const int EARTHQUAKE = 114;
        public const int SMS_SENDER = 115;

        public const int AIR_QUALITY = 120;
        public const int Parc1 = 121;
        public const int SKT_DT = 122;
        public const int UrbanBricks = 123;

        public static string ToString(int nClientSubType)
        {
            switch (nClientSubType)
            {
                case SIMULATOR:
                    return "시뮬레이터";

                case TH:
                    return "태흥전자";

                case SIEMENS:
                    return "지멘스";

                case JUBIX:
                    return "주빅스";

                case SENKO:
                    return "센코";

                case ASIN:
                    return "아신";

                case S1_SVMS:
                    return "에스원(SVMS)";

                case S1_ACCESS:
                    return "에스원(Access)";

                case S1_SECOM:
                    return "에스원(세콤)";

                case EMPOLL:
                    return "EmPoll";

                case JOHNSON_CONTROLS:
                    return "동방전자";

                case SHINHAN:
                    return "신한은행";

                case WOORIZEN:
                    return "우리젠";

                case POWER_PLANT:
                    return "발전소";

                case UNIVERSITY:
                    return "대학교";

                case OFFICE_BUILDING:
                    return "오피스 건물";

                case INTEGRATED_MANAGER:
                    return "통합관리자";

                case SOP_SIMULATOR:
                    return "SOP 시스템";

                case SOP_MANAGER:
                    return "SOP 생성기";
                case SOP_COMMANDER:
                    return "SOP Commander";
                case EARTHQUAKE:
                    return "Earthquake";
                case SMS_SENDER:
                    return "문자 발신기";

                case AIR_QUALITY:
                    return "공기질";

                case Parc1:
                    return "파크원";
                case SKT_DT:
                    return "SKT 디지털트윈";
                case UrbanBricks:
                    return "어반브릭스";
            }

            return nClientSubType.ToString();
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

    public class SOPSimulatorCommandType
    {
        public const byte RESET_USER_DEFINED_TEAM_NAMES = 1;
    }

    public class ErrorMessageType
    {
        public const int SUCCESS = 0;
        public const int SERVICE_IS_CLOSED = 1;
        public const int NULL_CLIENT_CONTEXT = 2;
        public const int UNKNOWN_CLIENT = 3;
        public const int UNKNOWN_HEADER = 4;
        public const int INVALID_MESSAGE = 5;
        public const int UNKNOWN_SENSOR_ID = 6;
        public const int DB_EXCEPTION = 7;
        public const int CAN_NOT_SEND_SMS = 8;
        public const int NO_SENSORZONE_HISTORY_ALARM = 9;
        public const int ALREADY_PROCESSED = 10;
        public const int INVALID_ID_OR_PASSWORD = 11;
        public const int ALREADY_USING_ID = 12;
        public const int UNKNOWN_CONFIG = 13;
        public const int NO_PERMISSION = 14;
        public const int NO_OTHER_CLIENTS = 15;
        public const int UNKNOWN_COMMAND = 16;

        public static string ToMessage(int nErrorType)
        {
            switch (nErrorType)
            {
                case SUCCESS:
                    return "성공";

                case SERVICE_IS_CLOSED:
                    return "서비스가 종료되었습니다.";

                case NULL_CLIENT_CONTEXT:
                    return "Null Client Context";

                case UNKNOWN_CLIENT:
                    return "알려지지 않은 클라이언트 타입입니다.";

                case UNKNOWN_HEADER:
                    return "알려지지 않은 메시지 헤더입니다.";

                case INVALID_MESSAGE:
                    return "형식에 맞지않는 메시지입니다.";

                case UNKNOWN_SENSOR_ID:
                    return "알수없는 센서 ID 입니다.";

                case DB_EXCEPTION:
                    return "Database 예외가 발생하였습니다.";

                case CAN_NOT_SEND_SMS:
                    return "문자메시지를 발송할 수 없습니다.";

                case NO_SENSORZONE_HISTORY_ALARM:
                    return "SensorZoneHistory ID에 해당하는 알람이 존재하지 않습니다.";

                case ALREADY_PROCESSED:
                    return "이미 처리되었습니다.";

                case INVALID_ID_OR_PASSWORD:
                    return "잘못된 아이디 혹은 비밀번호입니다.";

                case ALREADY_USING_ID:
                    return "이미 사용중인 ID입니다.";

                case UNKNOWN_CONFIG:
                    return "알수없는 설정값입니다.";

                case NO_PERMISSION:
                    return "권한이 없습니다.";

                case NO_OTHER_CLIENTS:
                    return "다른 클라이언트가 존재하지 않습니다.";

                case UNKNOWN_COMMAND:
                    return "알려지지 않은 command 입니다.";
            }

            return "";
        }
    }

    public class ServerPort
    {
        public const string SOP_WEB_SERVER = "SOPWebServer";
        public const string SOP_WEB_SERVER_MEX = "SOPWebServerMex";
    }
}