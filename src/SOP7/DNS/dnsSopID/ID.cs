namespace dnsSopID
{
    public class Header
    {
        // 탐지신호
        public const int SENSOR_DATA = 100;
        // 탐지신호(테스트)
        public const int SENSOR_DATA_TEST = 101;
        // 오동작처리
        public const int SENSOR_MALFUNCTION = 102;
        // 신호복구
        public const int SENSOR_USER_RESET = 103;
        // 재난신고
        public const int MANUAL_REPORT = 104;
        // 재난신고 해제
        public const int CLEAR_MANUAL_REPORT = 105;
        // 모든 신호 해제
        public const int CLEAR_DETECT_ALL = 109;

        // 기타 센서
        public const int ETC_SENSOR_DETECT = 125;
        public const int ETC_SENSOR_DATA_INT = 126;
        public const int ETC_SENSOR_DATA_DOUBLE = 127;
        public const int ETC_SENSOR_DATA_STRING = 128;

        // 상황 전파
        public const int SITUATION_NOTICE = 129;
        // SOP 진행 상황
        public const int SOP_RUN = 130;


        // 수동신고를 위한 Zone ID
        // ex) ManualReportDefaultID + FacilityType
        //     화재 : ManualReportDefaultID + FacilityType.FIRE_SENSOR = 1000000
        //     누출 : ManualReportDefaultID + FacilityType.PSM_SENSOR = 1000011
        public const int ManualReportDefaultID = 1000000;
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
        public const int NO_SUCH_ALARM = 17;

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

    public class DATA_TYPE
    {
        public const byte NULL = 0;
        public const byte INT = 1;
        public const byte INT_LIST = 2;
        public const byte FLOAT = 3;
        public const byte FLOAT_LIST = 4;
        public const byte DOUBLE = 5;
        public const byte DOUBLE_LIST = 6;
        public const byte STRING = 7;
        public const byte STRING_LIST_BEGIN = 8;
        public const byte STRING_LIST_END = 9;
        public const byte LONG = 10;
        public const byte LONG_LIST = 11;
        public const byte BOOLEAN = 12;
        public const byte BOOLEAN_LIST = 13;
        public const byte SHORT = 14;
        public const byte SHORT_LIST = 15;
        public const byte BYTE = 16;
        public const byte BYTE_ARRAY = 17;
        public const byte DATETIME = 18;
    }
}
