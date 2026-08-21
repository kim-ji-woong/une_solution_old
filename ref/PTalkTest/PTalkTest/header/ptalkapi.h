/*
 * Ptalk API : Copyright (c) 2014~2015, Netgenetech Co., Ltd All rights reserved.
 * uECC : Copyright (c) 2014, Kenneth MacKay All rights reserved.
 * OPUS : Copyright 2001-2011 Xiph.Org, Skype Limited, Octasic,
 *                  Jean-Marc Valin, Timothy B. Terriberry,
 *                  CSIRO, Gregory Maxwell, Mark Borgerding,
 *                  Erik de Castro Lopo
 * SHA3 : The Keccak sponge function, designed by Guido Bertoni, Joan Daemen,
 *		  Michael Peeters and Gilles Van Assche.
 * ARIA : 한국인터넷진흥원(KISA)
 */



#ifdef __cplusplus
extern "C" {
#endif

void setFunc_rcv_msg(void (*func)(int,int, unsigned int));
void setFunc_PlayerWrite(void (*func)(short *, int));
void setFunc_PlayerStart(void (*func)(int));
void setFunc_PlayerStop(void (*func)(void));
void setFunc_RecorderStart(void (*func)(void));
void setFunc_RecorderStop(void (*func)(void));
void setFunc_LogWrite(void (*func)(const void *, int));
int PtalkCmd(int msg, char *data, int dlen);
void sendVoice(short *data, int len);
void PtalkStart(int task, long long ptalkdevid);
void PtalkStop();
void AuthReq( char *url, char *userid, char *password);


enum GUI_MSG {
	UI_MSG_NONE=0,

	/**
	 * UI_MSG_AUTH_STATE_RES : 인증에 실패 했을 경우 결과 값을 보내 줌.
	 * @param 
	 *  arg1 : result
	 */
	UI_MSG_AUTH_STATE_RES=100,

	/**
	 * UI_MSG_REG_STATE_RES : 등록 결과를 보내 줌.
	 * @param 
	 *  arg1 : result
	 * @param 
	 *  arg2 : my SSID
	 */
	UI_MSG_REG_STATE_RES=101,

	/**
	 * UI_MSG_DEREG_RES : 등록 해제 요청이 완료 되었음을 의미.
	 * @param 
	 */
	UI_MSG_DEREG_RES=102,

	/**
	 * UI_MSG_DEREG_NOTIFY : 서버에 의해 등록이 해제 되었음을 알려줌.
	 * @param 
	 *  arg1 : reason
	 */
	UI_MSG_DEREG_NOTIFY=103,

	/**
	 * UI_MSG_CHG_GROUP_RES : 그룹 등록 결과를 알려줌.
	 * @param 
	 *  arg1 : result
	 * @param 
	 *  arg2 : 등록된 그룹 list (1 byte가 1 group을 표시, 최대 4개 까지 표시)
	 */
	UI_MSG_CHG_GROUP_RES=104,

	/**
	 * UI_MSG_CHG_GROUP_NOTIFY : 서버에 의해 group 정보가 변경 되었음을 알려줌.
	 * @param 
	 *  arg1 : 0
	 * @param
	 *  arg2 : 등록된 그룹 list (1 byte가 1 group을 표시, 최대 4개 까지 표시)
	 */
	UI_MSG_CHG_GROUP_NOTIFY=105,

	/**
	 * UI_MSG_PTT_ON_REQ : 서버로 PTT_ON_REQ가 전송 되었음을 알려줌.
	 * @param 
	 *  arg1 : call_type, (group << 8)
	 * @param 
	 *  arg2 : peer SSID
	 */
	UI_MSG_PTT_ON_REQ=106,

	/**
	 * UI_MSG_PTT_ON_RES : PTT_ON_REQ에 대한 결과
	 * @param 
	 *  arg1 : result
	 * @param 
	 *  arg2 : call_type, (call_state << 8), (encryption << 16)
	 */
	UI_MSG_PTT_ON_RES=107,

	/**
	 * UI_MSG_PTT_ON_NOTIFY : PTT_ON 수신을 알려줌.
	 * @param 
	 *  arg1 : call_type, (group << 8), (call_state << 16), (encryption << 24)
	 * @param 
	 *  arg2 : 발신자 SSID
	 */
	UI_MSG_PTT_ON_NOTIFY=108,

	/**
	 * UI_MSG_PTT_OFF_RES : PTT_OFF에 대한 결과
	 * @param 
	 *  arg1 : result
	 * @param 
	 *  arg2 : call_type, (call_state << 8)
	 */
	UI_MSG_PTT_OFF_RES=109,

	/**
	 * UI_MSG_PTT_OFF_NOTIFY : PTT_OFF 수신을 알려줌.
	 * @param 
	 *  arg1 : call_type, (group << 8), (reason << 16), (call_state << 24)
	 * @param 
	 *  arg2 : 발신자 SSID
	 */
	UI_MSG_PTT_OFF_NOTIFY=110,

	/**
	 * UI_MSG_PTT_END_RES : PTT_END에 대한 결과
	 * @param 
	 *  arg1 : result
	 * @param 
	 *  arg2 : call_type, (call_state << 8)
	 */
	UI_MSG_PTT_END_RES=111,

	/**
	 * UI_MSG_PTT_END_NOTIFY : PTT_END 수신을 알려줌.
	 * @param 
	 *  arg1 : reason
	 * @param 
	 *  arg2 : call_type, (call_state << 8) // call_state는 UI_MSG_PTT_END_NOTIFY메시지를 보내기 전 호 상태를 보냄.
	 */
	UI_MSG_PTT_END_NOTIFY=112,

	/**
	 * UI_MSG_CALL_RCV_NOTIFY : 수신호 setup이 시작되었음을 알려 줌.
	 * @param 
	 *  arg1 : call_type, (group <<8), (reason << 16)
	 * @param 
	 *  arg2 : 발신자 SSID
	 */
	UI_MSG_CALL_RCV_NOTIFY=113,

	/**
	 * UI_MSG_ALERT_RES : CALL_ALERT에 대한 결과값
	 * @param 
	 *  arg1 : result
	 * @param 
	 *  arg2 : peer SSID
	 */
	UI_MSG_ALERT_RES=114,

	/**
	 * UI_MSG_ALERT_NOTIFY : CALL_ALERT를 수신했음을 알려줌.
	 * @param 
	 *  arg1 : reason
	 * @param 
	 *  arg2 : 발신자 SSID
	 */
	UI_MSG_ALERT_NOTIFY=115,	
	
	/**
	 * UI_MSG_SUB_INFO_CHG_NOTIFY : 가입자 정보가 변경되었음을 알려 줌. [현재는 가입자 속성(서비스) 정보 변경만 통보함]
	 * ApiDefines.java에 있는 AT_xxxx 참조
	 * @param 
	 *  arg1 : new attribute
	 *  arg2 : old attribute
	 */
	UI_MSG_SUB_INFO_CHG_NOTIFY=122,
};

enum req_cmd
{
	/*
	 * RC_AUTH : 인증(authentication)을 요청함
	 *
	 * @param
	 * BUNCH (4 bytes)는 기본값으로 0
	 * SERVER(64 bytes) 는 기본값으로 www.ptalk20.com
	 * USERID(16 bytes) 는 사용자 Id.
	 * PASSWORD(32 bytes) 는  preference에 암호화 해서 저장해야 함
	 * SIM(32 bytes) SIM 번호
	 * GROUP(4 bytes) 각 1byte가 등록할 그룹 번호. 등록하지 않을 경우 0
	 *  struct auth_arg {
	 *  	int 		bunch; // bigendian
	 *  	char 		server[64];
	 *  	char 		userid[16];
	 *  	char 		password[32];
	 *  	char 		sim[32]; // usim number
	 *  	unsigned char 	group[4];
	 *  }:
	 */
	RC_AUTH = 0,
	/**
	 * RC_REG : 로그인(registration)을 함
	 * 인증이 성공하면 자동으로 로그인을 시도함
	 */
	RC_REG =1,
	/**
	 * RC_DEREG : 로그아웃(deregistration)을 함
	 * 로그아웃을 한 경우 다시 로그인을 하려고 하면 인증 절차를 거치게 해야 함
	 */
	RC_DEREG=2,
	/**
	 * @deprecated
	 * RC_PTT_ON : PTT 버튼을 누르고 발신을 시도 함
	 * 인증이 성공하고, 로그인된 경우에만 가능함
	 * RC_PTT_ON 명령을 사용하기 전에 아래 명령중 하나를 미리 실행 해야 함
	 * RC_SET_PEER (private call)
	 * RC_SET_GROUP (group call)
	 * RC_SET_EMER_GROUP (emergency group call)
	 * RC_SET_UDG_MEMS (udg call)
	 */
	//RC_PTT_ON=3,
	/**
	 * RC_PTT_OFF : PTT 버튼을 떼고 발신을 끝냄
	 * 인증이 성공하고, 로그인된 경우에만 가능함
	 */
	RC_PTT_OFF=4,

	/**
	 * RC_CALL_END : PTT 통화를 종료 함
	 * 인증이 성공하고, 로그인된 경우에만 가능함
	 */
	RC_CALL_END=5,

	/**
	 * RC_CHG_GRP : 그룹을 바꿈
	 * 인증이 성공하고, 로그인된 경우에만 가능함
	 * 내가 속한 그룹은 인증이 성공한 후에  RC_GET_GROUP 으로 읽어 올 수 있음
	 * 그룹은 1~255 까지 사용 가능함
	 * @param
	 *  group1(1 bytes)
	 *  group2(1 bytes)
	 *  group3(1 bytes)
	 *  group4(1 bytes)
	 * @return
	 *  result(int) : 0 = success , 1 = fail (MGRS속성이 없고 잘못된 그룹이 있음), 
	 *                2 = fail (MGRS속성이 있고 잘못된 그룹이 있음), 3 = fail (MGRS속성이 없고 그룹수가 1보다 큼)
	 *
	 */

	RC_CHG_GRP=6,

	/**
	 * RC_CALL_ALERT : 호출을 시도 함
	 * 인증이 성공하고, 로그인된 경우에만 가능함
	 * @param
	 *  peer_ssid(4 bytes)
	 */
	RC_CALL_ALERT=7,
	
	/**
	 * RC_JOIN_GROUP : 기존 호를 종료하고 그룹호에 합류함.
	 *
	 * @param
	 *  group(1 bytes)
	 */
	RC_JOIN_GROUP=11,

	/**
	 * RC_NEW_PTT_ON : PTT 버튼을 누르고 발신을 시도 함
	 * 인증이 성공하고, 로그인된 경우에만 가능함
	 * @return
	 *  result(1 byte) :  RESULT_NOT_IN_CALL 또는 RESULT_SUCCESS
	 */
	RC_NEW_PTT_ON=12,

	/**
	 * RC_PCALL_REQ : Private call을 시도함. (PTT_ON을 포함함)
	 *
	 * @param
	 *  ssid(4 bytes)
	 * @return
	 *  result(1 byte) : RESULT_SUCCESS, RESULT_L_INVALID_PARA, RESULT_L_CALL_BUSY, RESULT_PHONE_BUSY
	 */
	RC_PCALL_REQ=50,

	/**
	 * RC_GCALL_REQ : 그룹 통화를 시도함. (PTT_ON을 포함함)
	 *
	 * @param
	 *  group(1 bytes)
	 * @return
	 *  result(1 byte) : RESULT_SUCCESS, RESULT_L_INVALID_PARA, RESULT_L_CALL_BUSY, RESULT_PHONE_BUSY
	 */
	RC_GCALL_REQ=51,
	
	/**
	 * RC_EGCALL_REQ : 긴급 그룹 통화를 시도함. (PTT_ON을 포함함)
	 *
	 * @param
	 *  group(1 bytes)
	 * @return
	 *  result(1 byte) : RESULT_SUCCESS, RESULT_L_INVALID_PARA, RESULT_L_CALL_BUSY, RESULT_PHONE_BUSY
	 */
	RC_EGCALL_REQ=52,

	/**
	 * RC_SET_UDG_MEMS : UDG 통화를 시도함. (PTT_ON을 포함함)
	 *
	 * ssid 를 1 개 부터 20(RC_GET_UDG_MEMS_SIZE/4)개 까지 추가할 수 있음
	 * @param
	 *  ssid1(4 bytes)
	 *  ssid2(4 bytes)
	 *  ...
	 *  ssidn(4 bytes)
	 * @return
	 *  result(1 byte) : RESULT_SUCCESS, RESULT_L_INVALID_PARA, RESULT_L_CALL_BUSY, RESULT_PHONE_BUSY
	 */
	RC_UDGCALL_REQ=53,

	/**
	 * RC_GET_ATTR_ALL : 한번에 attribute전부 받아옴.
	 * Attribute 값은 인증 후에 받아 옴
	 * @return
	 *  AT_LMS = (1<<0),        // 비 활성화시 착 발신 금지
	 *  AT_UDG = (1<<1),        // 비 활성화시 발신 금지
	 *  AT_EMER = (1<<2),       // 비 활성화시 발신 금지
	 *  AT_MGRS = (1<<3),       // 비 활성화시 single그룹만 지원
	 *  AT_DISOCALL = (1<<4),   // 전체 발신호 차단
	 *  AT_DISTCALL = (1<<5),   // 전체 수신호 차단
	 *  AT_DISOPCALL = (1<<6),  // private 발신호 차단
	 *  AT_DISTPCALL = (1<<7),  // private 수신호 차단
	 *  AT_DISOGCALL = (1<<8),  // group 발신호 차단
	 *  AT_DISTGCALL = (1<<9),  // group 수신호 차단
	 *  AT_ALERT = (1<<10),     // 비 활성화시 착 발신 금지
	 *  AT_CROSS_BUNCH = (1<<11), // bunch간 개별통화, UDG통화 가능하게.
	 *  AT_HD_VOICE = (1<<12),  // 고품질 codec사용
	 *  AT_ENCRYPTION = (1<<13), // 암호화
	 *  AT_RCV_SEND = (1<<14),  // 개별호 수신중 송신 기능
	 *  AT_RECORD = (1<<15),  // 녹음 기능 on/off
	 */
	RC_GET_ATTR_ALL=63,

	/**
	 * RC_GET_GROUP : 나의 그룹 정보를 읽어 옴
	 * 그룹 정보는 인증 후에 받아 옴
	 * @param
	 *  group_info (256 bytes)
	 */
	RC_GET_GROUP = 64,


	/**
	 * RC_GET_AUTH_STATE : 현재 인증되어 있는지 아닌지를 체크 함
	 * @return
	 *  state (1 bytes) : 0 -> 인증 되지 않음,  1 -> 인증 됨
	 */
	RC_GET_AUTH_STATE=65,

	/**
	 * RC_GET_REG_STATE : 현재 로그인되어 있는지 아닌지를 체크 함
	 * @return
	 *  state (1 bytes) : 0 -> 로그인 되지 않음,  1 -> 로그인 됨
	 */
	RC_GET_REG_STATE=66,

	/**
	 * RC_GET_CALL_STATE : 현재 PTT 통화 상태를 체크 함
	 * @return
	 *  state (1 bytes) : 0 -> 통화중이 아님,  1~6 -> 통화 중
	 *
	 * enum CALL_STATE {
	 * 	CS_DORMANT = 0,
	 * 	CS_IDLE,		// pause
	 * 	CS_OUTGO_ING,	// outgoing processing
	 * 	CS_OUTGO_BUSY,	// voice sending
	 * 	CS_INCOM_ING,	// incoming processing
	 * 	CS_INCOM_BUSY,	// voice receiving
	 * 	CS_STOP_ING,	// call stopping
	 *  CS_INOUT_BUSY,	// 송수신 중
	 * };
	 */
	RC_GET_CALL_STATE=67,

	/**
	 * RC_GET_CALL_TYPE : 현재 PTT 통화 종류를 읽어 옴
	 * @return
	 *  type (1 bytes) : 0 -> 통화중이 아님, 1 -> 개인 통화, 2 -> 그룹 통화, 3 -> 긴급 통화, 4 -> UDG 통화
	 *
	 * enum CALL_TYPE {
	 * CT_UNKNOWN = 0,
	 * CT_PRIVATE = 1,
	 * CT_GROUP,
	 * CT_EMERGENCY,
	 * CT_UDG,
	 * };
	 */
	RC_GET_CALL_TYPE=68,

	/**
	 * RC_GET_CALL_PEER_SSID : 개인 통화중인 상대방의 SSID 를 읽어 옴
	 * @return
	 *  ssid (4 bytes)
	 */
	RC_GET_CALL_PEER_SSID=69,

	/**
	 * RC_GET_CALL_GROUP : 통화중인 상대 그룹의 번호를 읽어 옴
	 * @return
	 *  group (1 bytes)
	 */
	RC_GET_CALL_GROUP=73,	

	/**
	 * RC_GET_UDG_MEMS : UDG 통화중인 멤버의 ssid를 읽어 옴
	 * ssid 를 1 개 부터 20(RC_GET_UDG_MEMS_SIZE/4)개까지 읽어 올 수 있음
	 * @param
	 *  ssid1(4 bytes)
	 *  ssid2(4 bytes)
	 *  ...
	 *  ssidn(4 bytes)
	 * @return
	 *  member_count (4 bytes)
	 */
	RC_GET_UDG_MEMS=74,

	/**
	 * RC_GET_REG_GROUP : 현재 등록된 그룹 정보를 가져 옴
	 * @param
	 *  group1 (1 bytes)
	 *  group2 (1 bytes)
	 *  group3 (1 bytes)
	 *  group4 (1 bytes)
	 * @return
	 *  reg_group_cnt (4 bytes) : 등록된 그룹 수
	 */
	RC_GET_REG_GROUP=81,

	/**
	 * RC_GET_SSID : 내 SSID를 문자열로 읽어 옴
	 *
	 * @param
	 *  ssid_string (16 bytes) 1001  ->  "0*0*1001"
	 */
	RC_GET_SSID=84,

	/**
	 * RC_RECORD_START : 백그라운드 서비스에서 레코딩을 시작하라고 명령을 함
	 */
	RC_RECORD_START= 82,

	/**
	 * RC_GET_BLOCK_UDG_MEMS : 차단된  UDG 멤버의 ssid를 읽어 옴
	 * ssid 를 1 개 부터 20(RC_GET_UDG_MEMS_SIZE/4)개까지 읽어 올 수 있음
	 * @param
	 *  ssid1(4 bytes)
	 *  ssid2(4 bytes)
	 *  ...
	 *  ssidn(4 bytes)
	 * @return
	 *  member_count (4 bytes)
	 */
	RC_GET_BLOCK_UDG_MEMS=93,

	/**
	 * RC_SET_ENCRYPTION : set encryption enable or disable
	 *
	 * @param 
	 *  enable[1byte] : encryption (0:  disable, 1: enable)
	 */
	RC_SET_ENCRYPTION=96,

	/**
	 * RC_GET_STATUS : 현재 상태를 조회함.
	 *
	 * @param 
	 *  auth_state(1byte) : 인증 여부
	 *  reg_state(1byte) : 등록 여부
	 *  call_state(1byte) : 호 상태
	 *  call_type(1byte) : 호 종류
	 * @return
	 *  peer_ssid(4byte) : 개별호일 경우 상대 ssid
	 *  
	 */
	RC_GET_STATUS=99,
};

enum {
	UI_RESULT_SUCCESS = 0,
	UI_RESULT_INVALID_SSID 		= 1,
	UI_RESULT_NO_RESOURCE 		= 2,
	// 3 not used
	UI_RESULT_INVALID_USER		= 4,
	UI_RESULT_INVALID_PARA		= 5,
	UI_RESULT_INVALID_STATE		= 6,
	UI_RESULT_INVALID_GROUP		= 7,
	UI_RESULT_INVALID_CALLID	= 8,
	UI_RESULT_INVALID_CALLTYPE	= 9,
	UI_RESULT_NOT_PRIVILEGE		=10,
	UI_RESULT_PEER_NOT_READY	=11,
	UI_RESULT_OTHER_OWNER		=12,
	// 13 not used
	UI_RESULT_CALL_BUSY		=14,
	UI_RESULT_PEER_BUSY		=15,
	UI_RESULT_OTHER_MASTER		=16,
	UI_RESULT_PLEASE_RETRY		=17,
	UI_RESULT_NO_MEMBER		=18,
	UI_RESULT_GROUP_OVRFLOW		=19,
	UI_RESULT_NOT_TALK		=20,
	UI_RESULT_NO_RESPONSE		=22,
	UI_RESULT_LMS_MSG_TOO_BIG	=23,
	UI_RESULT_OVER_LOAD		=23,
	UI_RESULT_PEER_NOT_PRIVILEGE	=24,
	UI_RESULT_NOT_ENC_PRIVILEGE	=25,
	UI_RESULT_NOT_ENC_ENABLED	=26,

	UI_RESULT_INVALID_USER_INFO 	= 50,
	UI_RESULT_AUTH_CONN_FAIL 	= 51,
	UI_RESULT_AUTH_DNS_FAIL 	= 52,
	UI_RESULT_AUTH_FAIL_RES 	= 53,
	UI_RESULT_REG_CONN_FAIL 	= 54,
	UI_RESULT_REG_FAIL 		= 55,
	UI_RESULT_ON_FAIL 		= 56,
	UI_RESULT_PHONE_BUSY 		= 57,
	UI_RESULT_NOT_REG 		= 58, // 등록이 되지 않은 경우
	UI_RESULT_CALLTYPE_ERR 		= 59,
	UI_RESULT_GROUP_ERR 		= 60,
	UI_RESULT_TRY_REG 		= 61,
	RESULT_L_INVALID_PARA 		= 62, // api에서 parameter check error
	RESULT_L_CALL_BUSY 		= 63, // api에서 호 중이여서 error
	RESULT_NOT_IN_CALL 		= 64, // api에서 호 중이 아니여서 error
};

enum {
	UI_REASON_NORMAL_CALL = 0,

	UI_REASON_EMERGENCY_CALL	= 1,
	UI_REASON_READY_TO_LISTEN	= 2,
	UI_REASON_EXIT_SPEECH		= 3,
	UI_REASON_TIME_OUT		= 4,
	UI_REASON_HANG_UP_TIME_OUT	= 5,
	UI_REASON_TALKER_CALL_END	= 6,
	UI_REASON_NO_MORE_PEERS		= 7,
	UI_REASON_LONG_SPEECH		= 8,
	UI_REASON_NO_VOICE		= 9,
	UI_REASON_OTHER_REGISTRATION	=10,
	UI_REASON_RE_AUTH		=11,
	UI_REASON_DEL_SSID		=12,
	UI_REASON_CONN_FAIL		=13,
	UI_REASON_NOT_PRIVILEGE		=14,
	UI_REASON_CANCEL_CALL		=15,
    UI_REASON_INVALID_STATE         =16,
    UI_REASON_CHG_PASSWORD          =17,
	UI_REASON_PHONE_BUSY 		= 50,
	UI_REASON_BLOCKED 		= 51,
	UI_REASON_TUNNEL_FAIL 		= 52,
	UI_REASON_FAIL 			= 53,
	UI_REASON_NETWORK_FAIL 		= 54,
	UI_REASON_NOT_ENC_PRI 		= 55,
	UI_REASON_NOT_ENC_EN 		= 56,
	UI_REASON_SERVER_NO_RES		= 57,
};

enum SERVICE_ATTR {
	AT_LMS = (1<<0),        // 비 활성화시 착 발신 금지
	AT_UDG = (1<<1),        // 비 활성화시 발신 금지
	AT_EMER = (1<<2),       // 비 활성화시 발신 금지
	AT_MGRS = (1<<3),       // 비 활성화시 single그룹만 지원
	AT_DISOCALL = (1<<4),   // 전체 발신호 차단
	AT_DISTCALL = (1<<5),   // 전체 수신호 차단
	AT_DISOPCALL = (1<<6),  // private 발신호 차단
	AT_DISTPCALL = (1<<7),  // private 수신호 차단
	AT_DISOGCALL = (1<<8),  // group 발신호 차단
	AT_DISTGCALL = (1<<9),  // group 수신호 차단
	AT_ALERT = (1<<10),     // 비 활성화시 착 발신 금지
	AT_CROSS_BUNCH = (1<<11), // bunch간 개별통화, UDG통화 가능하게.
	AT_HD_VOICE = (1<<12),  // 고품질 codec사용
	AT_ENCRYPTION = (1<<13), // 암호화
	AT_RCV_SEND = (1<<14),  // 개별호 수신중 송신 기능
	AT_RECORD = (1<<15),  // 녹음 기능 on/off
};

enum CALL_TYPE {
	CT_UNKNOWN	= 0,
	CT_PRIVATE 	= 1,
	CT_GROUP 	= 2,
	CT_EMERGENCY 	= 3,
	CT_UDG		= 4,
};

enum CALL_STATE {
	CS_DORMANT	= 0,	// 호 중이 아님을 나타냄
	CS_IDLE		= 1,	// pause
	CS_OUTGO_ING	= 2,	// outgoing processing
	CS_OUTGO_BUSY	= 3,	// voice sending
	CS_INCOM_ING	= 4,	// incoming processing
	CS_INCOM_BUSY	= 5,	// voice receiving
	CS_STOP_ING	= 6,	// call stopping
	CS_INOUT_BUSY	= 7,	// 송수신 중
};


#ifdef  __cplusplus
};  // extern "C"
#endif
