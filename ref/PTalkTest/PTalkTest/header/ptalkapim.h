/*
 * Ptalk API : Copyright (c) 2014~2016, Netgenetech Co., Ltd All rights reserved.
 * uECC : Copyright (c) 2014, Kenneth MacKay All rights reserved.
 * OPUS : Copyright 2001-2011 Xiph.Org, Skype Limited, Octasic,
 *                  Jean-Marc Valin, Timothy B. Terriberry,
 *                  CSIRO, Gregory Maxwell, Mark Borgerding,
 *                  Erik de Castro Lopo
 * SHA3 : The Keccak sponge function, designed by Guido Bertoni, Joan Daemen,
 *		  Michael Peeters and Gilles Van Assche.
 * ARIA : 한국인터넷진흥원(KISA)
 */
#include <stdint.h>

#ifdef __cplusplus
extern "C" {
#endif

/** InitPtalkApi
 *  프로그램시작시 최초 한번만 호출하여야 하며
 *  OpenSession하기전에 호출되어야함.
 */
void InitPtalkApi(void);

/** OpenSession
 *  단말 하나당 한번씩 호출 되어야 함.
 *  dev_id는 0이 아니여야 하며, 각 session별로 다른 값을 가져야하고,
 *  재 시작시에도 같은 값을 유지해야 함.
 *  저장해둔 dev_id가 없으면 random값으로 하나를 생성하여 저장해 두고,
 *  프로그램이 재 시작시 저장해둔 dev_id를 사용하고, session별로는
 *  저장해둔 dev_id에 sid를 더해서 사용하기를 권고
 *  sid는 setFunc_xxx의 callback함수의 sid로 되돌아 오는 값으로
 *  각 session별로 다른 값을 사용해야 함. 연속적인 값을 사용하기를 권고함
 */
void *OpenSession(unsigned int dev_id, int sid);

/** CloseSession
 *  session을 더이상 사용하지 않을 때 사용
 *  OpenSession에서 사용된 resource를 회수함
 */
int  CloseSession(void *session_p);

/** setFunc_LogWrite
 *  api log 내용을 받을 수 있도록 callback 등록
 *  level : log level (RC_LOG_CTL에서의 각 bit number, 0=error, 1=warnning ...)
 *  log : 실제 log내용
 *  log_len : log length
 */
void setFunc_LogWrite(void (*func)(int sid, unsigned char level, const char *log, int log_len));

/** setFunc_rcv_msg
 *  api로부터 message를 수신할 callback 함수 등록
 */
void setFunc_rcv_msg(void (*func)(int sid,int msg,int arg1, int arg2));

/** setFunc_OpusWrite
 * setFunc_OpusWrite(func)에서 func가 NULL이 아니면 등록한 func
 * 으로 opus로 enconding된 voice가 호출되고 
 * NULL이면 setFunc_PlayerWrite(func)에서 등록된 func으로
 * PCM16 voice가 호출된다.
 */
void setFunc_OpusWrite(void (*func)(int sid, char *, int));
/** setFunc_PlayerWrite
 * setFunc_OpusWrite(func)에서 func가 NULL이 거나,
 * setFunc_OpusWrite를 호출하지 않았을 때
 * 등록된 func으로 PCM16 voice가 호출된다.
 */
void setFunc_PlayerWrite(void (*func)(int sid, short *, int));

/** setFunc_PlayerStart
 *  음성 수신이 시작됨을 알려 줌
 *  wideband == 1 : 16K PCM
 *  wideband == 0 : 8K PCM // 사용하지 않음
 */
void setFunc_PlayerStart(void (*func)(int sid, int wideband));

/** setFunc_PlayerStop
 *  음성 수신이 종료되었음을 알려 줌
 */
void setFunc_PlayerStop(void (*func)(int sid));

/** setFunc_RecorderStart
 *  음성 송신을 시작하라는 요구 : sendVoice를 통해 음성 전달
 */
void setFunc_RecorderStart(void (*func)(int sid));

/** setFunc_RecorderStop
 *  음성 송신을 중지하라는 요구
 */
void setFunc_RecorderStop(void (*func)(int sid));

/**
 * setBindIp : local ip를 특정한 ip로 설정 (PtalkStart전에 호출 하여야 함)
 * @param
 *  host_endian_ip : 0 = any ip,
 *                   0이 아니면 interface에 있는 특정 ip,
 *                   default 0
 */
void setBindIp(unsigned int host_endian_ip);


/** PtalkStart
 *  OpenSession후 ptalk을 활성화 할 때 호출
 */
void PtalkStart(void *sp);

/** PtalkStop
 *  PtalkStart후 ptalk을 비활성화 할 때 호출
 */
void PtalkStop(void *sp);

/** AuthReq
 *  PtalkStart후 서버에 등록할 때 사용
 */
void AuthReq(void *sp, char *url, char *userid, char *password);

/** PtalkEncrypt
 *  password를 암호화해서 저장할 때 사용.
 *  dlen(in)  : password길이
 *  data(in)  : password
 *  crypt(out): 암호화된 password가 저장될 위치 (32bytes 필요)
 *              crypt를 AuthReq시 password위치에 전달하면 됨.
 *              crypt는 string이 아니므로 32byte전부를 저장해 두어야 함
 */
int PtalkEncrypt(int dlen, char *data, char *crypt);

/** ChgGroup
 *  서버에 등록 후 그룹을 변경할 때 사용.
 *  MGRS기능을 사용시 PtalkCmd를 이용할 것
 */
void ChgGroup(void *sp, unsigned char group);

/** PtalkCmd
 *  API에 명령을 내릴때 사용
 *  msg는 "enum req_cmd"을 참고
 *  data는 msg의 종류에 따라서 정해짐
 *  dlen는 data의 length
 */
int PtalkCmd(void *sp, int msg, char *data, int dlen);

/** sendOpus
 * opus로 encodig된 data를 전송할 때 사용.
 */
void sendOpus(void *sp, char *data, int len);

/** sendVoice
 * PCM16 data를 전송할 때 사용
 */
void sendVoice(void *sp, short *data, int len);

#ifdef	WINVER	// windows
/**
 * send_video_fccp : h264 video data를 전송함
 * @param rtp : 표준 rtp header (12byte : sequence는 비 표준, 1byte는 frame number, 1byte는 frame내에서 seq. number)
 * @param len : rtp header size 12 + h264 data size (RFC3984 format)
 */
void send_video_fccp(void *sp, uint8_t *rtp, int len);
#else
/**
 * send_video_fccp : h264 video data를 전송함
 * @param rtp : 표준 rtp header (12byte : sequence는 비 표준, 1byte는 frame number, 1byte는 frame내에서 seq. number)
 * @param len1 : rtp header size 12 + h264 data size (RFC3984 format)
 * @param video : h264 video data
 * @param len2 : h264 data size (RFC3984 format)
 * 만약 rtp에 rtp header와 h264 data가 같이 있을 경우 len2를 0으로 설정하면 됨
 */
void send_video_fccp(void *sp, uint8_t *rtp, int len1, uint8_t *video, int len2);
#endif
/** reg_video_recv_func
 *  영상 수신 callback 등록
 */
void reg_video_recv_func(void (*func)(int sid, unsigned char *,int));

/** video_enable
 *  영상 수신 여부 설정
 *  en : bit0 == 1이면 영상 공유 통화 수신
 *       bit1 == 1이면 영상 통화 수신
 */
void video_enable(void *sp,int en);

/** get_video_on_delay
 *  @return video_on_res 수신 후 경과된 시간을 ms 단위로 return함
 */
int get_video_on_delay(void *sp);

/** get_video_rate
 *  @return 영상을 보낼 bitrate (Kbps)
 */
int get_video_rate(void *sp);

/** OpusEncode
 * pcm을 opus로 encode
 * @param pcm(in) : pcm data
 * @param len(in) : pcm data의 sample수 : 60ms기준 960
 * @param opus(out) : encoded opus data : 256 bytes 필요
 * @return : encoded opus data bytes
 */
int OpusEncode(void *sp, short *pcm, int len, uint8_t *opus);

/** OpusDecode
 * pcm을 opus로 decode
 * @param opus(in) : encoded opus data 
 * @param len(in) : opus data의 byte수 
 * @param pcm(out) : pcm data : 60ms기준 960 shorts필요
 * @return : decoded data sample수 : 60ms기준 960
 */
int OpusDecode(void *sp, uint8_t *opus, int len, short *pcm);

/*
 * session과 관계없는 opus codec
 */
/** CommOpenHdOpusEncoder()
 * open 고품질 음성 encoder
 */
void *CommOpenHdOpusEncoder();

/** CommOpenOpusEncoder()
 * open 일반 품질 음성 encoder
 */
void *CommOpenOpusEncoder();

/** OpusEncode
 * pcm을 opus로 encode
 * @param ep(in) : CommOpenHdOpusEncoder(),CommOpenOpusEncoder()에서 return된 값
 * @param pcm(in) : pcm data
 * @param len(in) : pcm data의 sample수 : 60ms기준 960
 * @param opus(out) : encoded opus data : 256 bytes 필요
 * @return : encoded opus data bytes
 */
int CommOpusEncode(void *ep, short *pcm, int len, uint8_t *opus);

/** CommCloseOpusEncoder
 * close encoder
 * @param ep(in) : CommOpenHdOpusEncoder(),CommOpenOpusEncoder()에서 return된 값
 */
void CommCloseOpusEncoder(void *ep);

/** CommOpenOpusDecoder
 * open decoder
 */
void *CommOpenOpusDecoder();

/** CommOpusDecode
 * pcm을 opus로 decode
 * @param dp(in) : CommOpenOpusDecoder()에서 return된 값
 * @param opus(in) : encoded opus data 
 * @param len(in) : opus data의 byte수 
 * @param pcm(out) : pcm data : 60ms기준 960 shorts필요
 * @return : decoded data sample수 : 60ms기준 960
 */
int CommOpusDecode(void *dp, uint8_t *opus, int len, short *pcm);

/** CommCloseOpusDecoder
 * close decoder
 * @param dp(in) : CommOpenOpusDecoder()에서 return된 값
 */
void CommCloseOpusDecoder(void *dp);

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
	 * UI_MSG_LMS_RES : LMS 전송 결과를 알려줌.
	 * @param 
	 *  arg1 : result
	 * @param 
	 *  arg2 : LMS를 수신한 peer수
	 */
	UI_MSG_LMS_RES=116,

	/**
	 * UI_MSG_LMS_NOTIFY : LMS를 수신 했음을 알려 줌.
	 * @param 
	 *  arg1 : reason, (call_type << 8), (msg_len << 16)
	 * @param 
	 *  arg2 : 발신자 SSID
	 */
	UI_MSG_LMS_NOTIFY=117,

	/**
	 * UI_MSG_PRESENCE_RES : PRESENCE에 대한 결과값
	 * @param 
	 *  arg1 : result
	 * @param 
	 *  arg2 : presence counter
	 */
	UI_MSG_PRESENCE_RES=118,

	/**
	 * UI_MSG_MGRS_NOTIFY : PTT 통화중 추가 그룹콜이 발생했음을 알려 줌.
	 * @param 
	 *  arg1 : call_type, (group << 8)
	 * @param 
	 *  arg2 : 발신자 SSID
	 */
	UI_MSG_MGRS_NOTIFY=119,

	/**
	 * @deleted
	 * UI_MSG_BOOSTER_NOTIFY : booster가 동작중인지를 알려줌.
	 * @param 
	 *  arg1 : on = 1, off = 0
	 */
	//UI_MSG_BOOSTER_NOTIFY=120,

	/**
	 * UI_MSG_ALMOST_HOLD_NOTIFY : 장시간 PTT_ON시 강제 종료 10초 전임을 알려 줌.
	 * @param 
	 *  arg1 : call_type
	 */
	UI_MSG_ALMOST_HOLD_NOTIFY=121,

	/**
	 * UI_MSG_SUB_INFO_CHG_NOTIFY : 가입자 정보가 변경되었음을 알려 줌. [현재는 가입자 속성(서비스) 정보 변경만 통보함]
	 * ApiDefines.java에 있는 AT_xxxx 참조
	 * @param 
	 *  arg1 : new attribute
	 *  arg2 : old attribute
	 */
	UI_MSG_SUB_INFO_CHG_NOTIFY=122,

	/**
	 * UI_MSG_MGRS_JOIN_RES : MGRS join에 대한 결과를 return함. 
	 * 2015.8월 package와 연동됨. system upgrade된 후 enable
	 * @param 
	 *  arg1 : result
	 * @param 
	 *  arg2 : call_type, (call_state << 8)
	 */
	UI_MSG_MGRS_JOIN_RES=123,

	/**
	 * UI_MSG_CALL_INFO_NOTIFY : 그룹 호에 참여중인 멤버수가 변경 되었음을 통보함
	 * @param 
	 *  arg1 : members (32bit)
	 * @param 
	 *  arg2 : call_type, (call_state << 8)
	 */
	UI_MSG_CALL_INFO_NOTIFY=124,

	/**
	 * UI_MSG_VIDEO_ON_RES : UI_MSG_VIDEO_ON_REQ에 대한 결과
	 * @param 
	 *  arg1 : result, (vcall_state << 8), (rate << 16) // rate는 16bits
	 * @param 
	 *  arg2 : call_type, (call_state << 8), (members << 16) // members는 16bits
	 */
	UI_MSG_VIDEO_ON_RES=130,
	/**
	 * UI_MSG_VIDEO_ON_NOTIFY : VIDEO_ON 수신을 알려줌.
	 * @param 
	 *  arg1 : call_type, (group << 8), (call_state << 16), (video_para << 24)
	 * @param 
	 *  arg2 : 발신자 SSID
	 */
	UI_MSG_VIDEO_ON_NOTIFY=131,
	/**
	 * UI_MSG_VIDEO_OFF_RES : VIDEO_OFF에 대한 결과
	 * @param 
	 *  arg1 : result
	 * @param 
	 *  arg2 : call_type, (call_state << 8)
	 */
	UI_MSG_VIDEO_OFF_RES=132,
	/**
	 * UI_MSG_VIDEO_OFF_NOTIFY : VIDEO_OFF 수신을 알려줌.
	 * @param 
	 *  arg1 : call_type, (group << 8), (reason << 16), (call_state << 24)
	 * @param 
	 *  arg2 : 발신자 SSID
	 */
	UI_MSG_VIDEO_OFF_NOTIFY=133,
	/**
	 * UI_MSG_VIDEO_JOIN_RES : VIDEO_JOIN결과를 알려줌
	 * @param 
	 *  arg1 : result
	 * @param 
	 *  arg2 :  call_type,  (call_state << 8)
	 */
	UI_MSG_VIDEO_JOIN_RES=134,

	/**
	 * UI_MSG_VIDEO_NOT_RCV_NOTIFY : VIDEO수신을 하지 못하는 경우 이유를 알려 줌
	 * @param 
	 *  arg1 : call_type, (call_state << 8), (reason << 16), (video_para << 24)
	 * @param 
	 *  arg2 : 영상 발신자 SSID
	 */
	UI_MSG_VIDEO_NOT_RCV_NOTIFY=135,
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
	 * RC_LMS : 메세지(Long Message Service)를 보냄
	 * 인증이 성공하고, 로그인된 경우에만 가능함
	 * RC_LMS 명령을 사용하기 전에
	 * RC_SET_LMS_GROUP 이나 RC_SET_LMS_PEER_SSID 를 미리 실행 해서 상대방을 설정해야 함
	 * @param
	 *  message(1000 bytes)
	 */
	RC_LMS=8,

	/**
	 * RC_PRESENCE : 상대방의 현재 상태를 체크 함
	 * 인증이 성공하고, 로그인된 경우에만 가능함
	 * ssid 를 1 개 부터 50(RC_GET_PRESENCE_SZ/4)개 까지 체크 할 수 있음
	 * 만약 ssid가 5개이면 5*4=20 bytes를 allocation하면 됨.
	 * @param
	 *  ssid1(4 bytes)
	 *  ssid2(4 bytes)
	 *  ...
	 *  ssidn(4 bytes)
	 */
	RC_PRESENCE=9,

	/**
	 * RC_WAKEUP_REQ : Sleep 상태인 상대편을 깨우기 위해 PTT를 시도하기 전에 미리 보내는 명령
	 * 화면 전환하기 전에 명령을 보내고, 화면 전환후에 PTT 를 시도하게 됨, 일명 fast call setup
	 * 인증이 성공하고, 로그인된 경우에만 가능함
	 * @param (group이나 ssid둘 중 하나만 있어야 함. 1 byte이면 group으로 인식하고, 4byte이면 private로 인식함)
	 *  group(1 byte) : group call인 경우
	 *  ssid (4 bytes) : provate call인 경우
	 */
	RC_WAKEUP_REQ=10,
	
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
	 * RC_VIDEO_JOIN : 영상 통화 중이면 영상 통화에 합류 (그룹,개인 그룹)
	 *
	 * @param
	 */
	RC_VIDEO_JOIN=13,

	/**
	 * RC_SET_GROUP : 그룹 통화 RC_PTT_ON 시도전에  통화를 원하는 그룹을 설정 함
	 *
	 * @param
	 *  group(1 bytes)
	 */
	RC_SET_GROUP=16,

	/**
	 * RC_SET_EMER_GROUP : 긴급 그룹 통화 RC_PTT_ON 시도전에  통화를 원하는 그룹을 설정 함
	 *
	 * @param
	 *  group(1 bytes)
	 */
	RC_SET_EMER_GROUP=17,

	/**
	 * RC_SET_LMS_GROUP : RC_LMS 시도전에  메세지를 보내길 원하는 그룹을 설정 함
	 *
	 * @param
	 *  group(1 bytes)
	 */
	RC_SET_LMS_GROUP=18,

	/**
	 * RC_SET_PEER : RC_PTT_ON 시도전에  통화를 원하는 상대방을 설정 함
	 *
	 * @param
	 *  ssid(4 bytes)
	 */
	RC_SET_PEER=19,

	/**
	 * RC_SET_UDG_MEMS : RC_PTT_ON 시도전에  그룹 통화를 원하는 상대방들을 설정 함
	 *
	 * ssid 를 1 개 부터 20(RC_GET_UDG_MEMS_SIZE/4)개 까지 추가할 수 있음
	 * @param
	 *  ssid1(4 bytes)
	 *  ssid2(4 bytes)
	 *  ...
	 *  ssidn(4 bytes)
	 */
	RC_SET_UDG_MEMS=20,

	/**
	 * RC_SET_LMS_PEER_SSID : RC_LMS 시도전에  호출을 원하는 상대방을 설정 함
	 *
	 * @param
	 *  ssid(4 bytes)
	 */
	RC_SET_LMS_PEER_SSID=22,

	/**
	 * RC_SET_NET_STATE : 백그라운드 서비스에서 네트웤 상태가 바뀌면 알려 줌
	 *
	 * @param
	 *  state(1 bytes) :  0 -> disconnect, 1 -> wifi,  2 -> mobile(3G), 3 -> LTE
 	 */
	RC_SET_NET_STATE=23,

	/**
	 * RC_SET_BLACK_LIST : 수신 차단 리스트를 설정 함
	 *
	 * ssid 를 1 개 부터 250(RC_GET_BLACK_LIST_SIZE/4)개 까지 추가할 수 있음
	 * @param
	 *  ssid1(4 bytes)
	 *  ssid2(4 bytes)
	 *  ...
	 *  ssidn(4 bytes)
	 */
	RC_SET_BLACK_LIST=25,

	/**
	 * RC_SET_PHONE_STATE : 백그라운드 서비스에서 현재 폰의 음성 통화 상태가 바뀌면 알려 줌
	 *
	 * @param
	 *  state(1 bytes) : 0 -> idle,  1 -> busy (음성통화중)
	 */
	RC_SET_PHONE_STATE=28,

	/**
	 * RC_SHOW_STATUS : 디버깅시에 필요한 정보값을 요청 함
	 * 현재 상태를 Log 에 출력 함
	 */
	RC_SHOW_STATUS = 31,


	/**
	 * @deprecated
	 * RC_HAS_ATTR_LMS : LMS (Long message service) 권한 있는지 체크 함
	 * Attribute 값은 인증 후에 받아 옴
	 * @return
	 *  attr (1 bytes) : 0 -> no attribute, 1 -> has attribute
	 */
	//RC_HAS_ATTR_LMS = 32,	// fccp_msg.h에 있는 ATTR과 순서가 같아야 함.
	/**
	 * @deprecated
	 * RC_HAS_ATTR_UDG : UDG (User defined group call) 권한 있는지 체크 함
	 * Attribute 값은 인증 후에 받아 옴
	 * @return
	 *  attr (1 bytes) : 0 -> no attribute, 1 -> has attribute
	 */
	//RC_HAS_ATTR_UDG,
	/**
	 * @deprecated
	 * RC_HAS_ATTR_EMER : 긴급 그룹 통화 권한 있는지 체크 함
	 * Attribute 값은 인증 후에 받아 옴
	 * @return
	 *  attr (1 bytes) : 0 -> no attribute, 1 -> has attribute
	 */
	//RC_HAS_ATTR_EMER,
	/**
	 * @deprecated
	 * RC_HAS_ATTR_MGRS : MGRS  권한 있는지 체크 함
	 * Attribute 값은 인증 후에 받아 옴
	 * @return
	 *  attr (1 bytes) : 0 -> no attribute, 1 -> has attribute
	 */
	//RC_HAS_ATTR_MGRS,
	/**
	 * @deprecated
	 * RC_HAS_ATTR_DISOCALL : 발신호 차단되어 있는지 체크 함
	 * Attribute 값은 인증 후에 받아 옴
	 * @return
	 *  attr (1 bytes) : 0 -> no attribute, 1 -> has attribute
	 */
	//RC_HAS_ATTR_DISOCALL,
	/**
	 * @deprecated
	 * RC_HAS_ATTR_DISTCALL : 수신호 차단되어 있는지 체크 함
	 * Attribute 값은 인증 후에 받아 옴
	 * @return
	 *  attr (1 bytes) : 0 -> no attribute, 1 -> has attribute
	 */
	//RC_HAS_ATTR_DISTCALL,
	/**
	 * @deprecated
	 * RC_HAS_ATTR_DISOPCALL : 개별 발신호 차단되어 있는지 체크 함
	 * Attribute 값은 인증 후에 받아 옴
	 * @return
	 *  attr (1 bytes) : 0 -> no attribute, 1 -> has attribute
	 */
	//RC_HAS_ATTR_DISOPCALL,
	/**
	 * @deprecated
	 * RC_HAS_ATTR_DISTPCALL : 개별 수신호 차단되어 있는지 체크 함
	 * Attribute 값은 인증 후에 받아 옴
	 * @return
	 *  attr (1 bytes) : 0 -> no attribute, 1 -> has attribute
	 */
	//RC_HAS_ATTR_DISTPCALL,
	/**
	 * @deprecated
	 * RC_HAS_ATTR_DISOGCALL : 그룹 발신호 차단되어 있는지 체크 함
	 * Attribute 값은 인증 후에 받아 옴
	 * @return
	 *  attr (1 bytes) : 0 -> no attribute, 1 -> has attribute
	 */
	//RC_HAS_ATTR_DISOGCALL,
	/**
	 * @deprecated
	 * RC_HAS_ATTR_DISTGCALL : 그룹 수신호 차단되어 있는지 체크 함
	 * Attribute 값은 인증 후에 받아 옴
	 * @return
	 *  attr (1 bytes) : 0 -> no attribute, 1 -> has attribute
	 */
	//RC_HAS_ATTR_DISTGCALL,
	/**
	 * @deprecated
	 * RC_HAS_ATTR_ALERT : CALL ALERT 사용 가능한지 체크 함
	 * Attribute 값은 인증 후에 받아 옴
	 * @return
	 *  attr (1 bytes) : 0 -> no attribute, 1 -> has attribute
	 */
	//RC_HAS_ATTR_ALERT,

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
	 * RC_GET_ALERT_PEER_SSID : 호출을 한 상대방의 SSID 를 읽어 옴
	 * @return
	 *  ssid (4 bytes)
	 */
	RC_GET_ALERT_PEER_SSID=70,

	/**
	 * RC_GET_LMS_PEER_SSID : 메세지를 보낸 상대방의 SSID 를 읽어 옴
	 * @return
	 *  ssid (4 bytes)
	 */
	RC_GET_LMS_PEER_SSID=71,

	/**
	 * RC_GET_GRP_OK_MEM_CNT : 그룹 통화시에 통화중인 멤버의 수를 읽어 옴
	 * @return
	 *  member_count (4 bytes)
	 */
	RC_GET_GRP_OK_MEM_CNT=72,

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
	 * RC_GET_LMS_MSG : LMS(Long message service) 메세지를  1000 bytes(RC_GET_LMS_MSG_SIZE) 까지 읽음
	 * @param
	 *  message (1000 bytes)
	 * @return
	 *  msg_length (4 bytes)
	 */
	RC_GET_LMS_MSG=75,

	/**
	 * RC_GET_UDG_MEM_CNT : UDG 통화중인 멤버의 수를 읽어 옴
	 * @return
	 *  member_count (4 bytes)
	 */
	RC_GET_UDG_MEM_CNT=76,

	/**
	 * @deprecated
	 * RC_GET_MGRS_PEER_SSID : MGRS 그룹 통화를 시도한 상대방 SSID 를 읽어 옴
	 * @return
	 *  ssid (4 bytes)
	 */
	RC_GET_MGRS_PEER_SSID=79,

	/**
	 * @deprecated
	 * RC_GET_MGRS_PEER_SSID : MGRS 그룹 통화를 시도한 그룹 정보를 읽어 옴
	 * @return
	 *  group (1 bytes)
	 */
	RC_GET_MGRS_GROUP=80,

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
	 * RC_RECORD_START : 백그라운드 서비스에서 레코딩을 시작하라고 명령을 함
	 */
	RC_RECORD_START= 82,

	/**
	 * RC_GET_BLACK_LIST : 수신 차단 리스트를 가져 옴
	 *
	 * ssid 를 1 개 부터 250(RC_GET_BLACK_LIST_SIZE/4)개 까지 가져옴
	 * @param
	 *  ssid1(4 bytes)
	 *  ssid2(4 bytes)
	 *  ...
	 *  ssidn(4 bytes)
	 */
	RC_GET_BLACK_LIST=83,

	/**
	 * RC_GET_SSID : 내 SSID를 문자열로 읽어 옴
	 *
	 * @param
	 *  ssid_string (16 bytes) 1001  ->  "0*0*1001"
	 */
	RC_GET_SSID=84,

	/**
	 * RC_GET_PRESENCE : 상대방의 현재 상태를 읽어 옴
	 * RC_PRESENCE 호출 후에 사용해야 함
	 * ssid 를 1 개 부터 50(RC_GET_PRESENCE_SZ/4)개 까지 읽을 수 있음
	 * state는 0 : 가입자 정보 없음, 1 : 가입자 정보는 있으나 등록되지 않음, 2: 등록되어 있음, 0x10 : 상태를 알 수 없음.
	 * @return
	 *  count (4 bytes) : presence가 존재하는 멤버수 (param의 n에 해당함)
	 * @param
	 *  ssid1 (4 bytes)
	 *  ssid2 (4 bytes)
	 *  ...
	 *  ssidn (4 bytes)
	 *  state1 (1 bytes)
	 *  state2 (1 bytes)
	 *  ...
	 *  staten (1 bytes)
	 */
	RC_GET_PRESENCE=86,

	/**
	 * RC_SET_BLOCK_PTT : 수신 거부 조건을 설정 함, 비트 별로 설정 값이 저장되어 있음
	 *
	 * @param
	 *  state(1 bytes) : 0 -> 수신 거부 해제
	 *                   1 -> 그룹 호 수신 거부
	 *                   2 -> UDG 호 수신 거부
	 *                   4 -> 개인 호 수신 거부
	 */
	RC_SET_BLOCK_PTT=89,

	/**
	 * RC_GET_BLOCK_CALL_TYPE : 현재 차단된  통화 종류를 읽어 옴
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
	RC_GET_BLOCK_CALL_TYPE=90,

	/**
	 * RC_GET_BLOCK_CALL_PEER_SSID : 차단된 상대방의 SSID 를 읽어 옴
	 * @return
	 *  ssid (4 bytes)
	 */
	RC_GET_BLOCK_PEER_SSID=91,

	/**
	 * RC_GET_BLOCK_GROUP : 차단된 상대 그룹의 번호를 읽어 옴
	 * @return
	 *  group (1 bytes)
	 */
	RC_GET_BLOCK_GROUP=92,

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
	 * @deleted
	 * RC_SET_BOOSTER : PTT Booster 기능을 위해서 시작,끝, 주기를 알려 줌
	 *
	 * @param 
	 *  start_time(4 bytes) : ptt booster start time (hour * 60 + min)
	 *  end_time(4 bytes) : ptt booster stop time (hour * 60 + min)
	 *  interval(4 bytes) : ptt booster interval (sec)
	 */
	//RC_SET_BOOSTER=94,

	/**
	 * @delete
	 * RC_SET_PUBKEY : set ECC my public_key (use RC_SET_ENCRYPTION)
	 *
	 * @param 
	 *  pub_key : public key
	 */
	//RC_SET_PUBKEY=96,

	/**
	 * RC_SET_ENCRYPTION : set encryption enable or disable
	 *
	 * @param 
	 *  enable[1byte] : encryption (0:  disable, 1: enable)
	 */
	RC_SET_ENCRYPTION=96,

	/**
	 * @delete
	 * RC_SET_DATAKEY : set data encoding, decoding 을 위한 ARIA key
	 *
	 * @param 
	 *  data_key : data encoding, decoding key
	 */
	// RC_SET_DATAKEY,

	/**
	 * RC_GET_STATUS : 현재 상태를 조회함.
	 *
	 * @param 
	 *  auth_state(1byte) : 인증 여부
	 *  reg_state(1byte) : 등록 여부
	 *  call_state(1byte) : 호 상태
	 *  call_type(1byte) : 호 종류
	 *  vcall_state(1byte) : video 호 상태
	 * @return
	 *  peer_ssid(4byte) : 개별호일 경우 상대 ssid
	 *  
	 */
	RC_GET_STATUS=99,
	
	/**
	 * RC_GET_BUNCH : bunch id를 return함. 인증이 성공한 후에 조회해야 함.
	 * 
	 * @return bunch_id(4 bytes)
	 */
	RC_GET_BUNCH=100,
	/**
	 * RC_LOG_CTL : api log on/off를 설정 (log off시에도 error message는 출력함)
	 * 
	 * @param 
	 * 	log_state(1byte) : 0 = log off, bit0 = error log, bit1 = warnning log, bit2 = timer 관련 log, bit3 = api core log,
	 *                                               bit4 = ptalk server와의 message log
	 * 	                   default=0x1f
	 *			error log는 off못함.
	 * @return 
	 */
	RC_LOG_CTL=101,
	
	/**
	 * RC_SET_MANUAL_RESPONSE : 수동 응대 기능
	 * @param
	 * 	on(1byte) : on = 1이면 수동 응대 기능 on, on = 0이면 자동 응대
	 */
	RC_SET_MANUAL_RESPONSE=102,
	
	/**
	 * RC_RESPONSE : 수동 응대시 응답을 해라는 의미 임. UI_MSG_CALL_RCV_NOTIFY 수신시 수신 준비가 완료되면 RC_RESPONSE를 호출하면 됨.
	 * @param
	 *  call_busy(1byte) : 3G, VOLTE, FMC call 중이면 1, 아니면 0
	 * @return
	 */
	RC_RESPONSE=103,

	/**
	 * RC_SET_BOOSTER_PARA : PTT Booster 기능을 위한 설정 값
	 *
	 * @param
	 *  resev(1byte) : 항상 0
	 *  len(1byte) : udp packet len(0~20)
	 *  port(2 bytes) : udp destination port number(bigendian)
	 *  server(4 bytes) : destination server ip address(bigendian)
	 *  data(0~20 bytes) : booster에 포함될 message
	 */
	RC_SET_BOOSTER_PARA=104,

	/**
	 * RC_SET_BOOSTER_INTV : PTT Booster packet주기를 설정 : 0이면 booster off
	 *
	 * @param
	 *  interval(1byte) : booster interval ( 0: off, 1~255 : on)
	 */
	RC_SET_BOOSTER_INTV=105,
	
	/**
	 * RC_SET_VIDEO_PARA : 영상 통화를 위한 파라미터 설정, video call start, stop (음성통화가 성립된 상태에서 호출 되어야 함)
	 *
	 * @param video_para : 
	 * video enable(1byte) : 0=stop, 1= 특수 영상 start, 2= 일반 영상 start
	 * video quality(1byte) : 0=저품질, 1=일반품질, 2=고품질
	 * @return : 0 : success, else fail 
	 */
	RC_SET_VIDEO=106,

	/**
	 * RC_SET_DEV_TYPE : ptalk client device의 종류를 지정함 (RC_AUTH를 호출하기 전에 호출하여야 함. 설정하지 않으면 전용폰으로 인식)
	 *
	 * @param dev_type(1byte): 0 : 전용폰, 1: 스마트폰
	 * @return :
	 */
	RC_SET_DEV_TYPE=107,

	
	/**
	 * RC_SET_SIGNAL_ENC : 암호화하여 접속할지 여부를 설정함
	 *
	 * @param enc(1byte): 0 : 암호화 하지 않음, 1: 인증시 암호화 함
	 * @return :
	 */
	RC_SET_SIGNAL_ENC=108,
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
	UI_RESULT_NOT_ENABLED		=27,
	UI_RESULT_OVER_TRAFFIC		=28,	// video traffic 과부하
	UI_RESULT_DECR_FAIL			=29,	// 암호해독 실패

	// result codes : local fail (in c)
	UI_RESULT_INVALID_USER_INFO = 50,
	UI_RESULT_AUTH_CONN_FAIL 	= 51,
	UI_RESULT_AUTH_DNS_FAIL 	= 52,
	UI_RESULT_AUTH_FAIL_RES 	= 53, // server에서 error return한 경우
	UI_RESULT_REG_CONN_FAIL 	= 54,
	UI_RESULT_REG_FAIL 			= 55, // confirm용
	UI_RESULT_ON_FAIL 			= 56, // fccp내부 fail
	UI_RESULT_PHONE_BUSY 		= 57, // 전화중인 경우
	UI_RESULT_NOT_REG 			= 58, // 등록이 되지 않은 경우
	UI_RESULT_CALLTYPE_ERR 		= 59,
	UI_RESULT_GROUP_ERR 		= 60, // MGRS속성이 없는데 group 2개 이상 등록 요청시
	UI_RESULT_TRY_REG 			= 61, // 등록 시도
	UI_RESULT_L_INVALID_PARA 	= 62, // api에서 parameter check error
	UI_RESULT_L_CALL_BUSY 		= 63, // api에서 호 중이여서 error
	UI_RESULT_NOT_IN_CALL 		= 64, // api에서 호 중이 아니여서 error
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
    UI_REASON_NO_VIDEO			=16,
    UI_REASON_INVALID_STATE     =17,
    UI_REASON_CHG_PASSWORD      =18,
	UI_REASON_NOT_ENABLED		=19,
	UI_REASON_GROUP_OVRFLOW		=20,

	UI_REASON_PHONE_BUSY 		= 50,
	UI_REASON_BLOCKED 		= 51,
	UI_REASON_TUNNEL_FAIL 		= 52,
	UI_REASON_FAIL 			= 53,
	UI_REASON_NETWORK_FAIL 		= 54,
	UI_REASON_NOT_ENC_PRI 		= 55,
	UI_REASON_NOT_ENC_EN 		= 56,
	UI_REASON_SERVER_NO_RES		= 57,
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
	AT_VIDEO_S_MASK = ( 3<<16), // 영상 공유 통화 VIDEO QUALITY Field (2bits), 0이면 영상 공유 통화 송신 기능 off
	AT_VIDEO_N_MASK = ( 3<<18), // 영상 통화 VIDEO QUALITY Field (2bits), 0이면 영상 통화 송신 기능 off
	AT_VIDEO_S_RX = (1<<20),    // 영상 공유 통화 수신
	AT_VIDEO_N_RX = (1<<21),    // 영상 통화 수신
};

enum PRESENCE_STATE {
        PS_NOT_PRE      = 0,    // 가입자 정보 없음
        PS_NOT_REG      = 1,    // 가입자 정보는 있으나 등록되지 않음
        PS_REG          = 2,    // 등록되어 있음
        PS_UNKNOWN      = 0x10, // 상태를 알 수 없음.
};

/* vcall state : vcall state가 VCS_DORMANT가 아니면 영상 호 중임. */
enum VIDEO_CALL_STATE {
	VCS_DORMANT	= 0,
	VCS_OUTGO_ING	= 1,
	VCS_OUTGO_BUSY	= 2,
	VCS_INCOM_BUSY	= 3,
};

/* 영상 품질 관련 */
#define VQ_LV0			0	// 영상 품질 레벨0
#define VQ_LV1			1	// 영상 품질 레벨1
#define VQ_LV2			2	// 영상 품질 레벨2
	
/* 영상 parameter bit의미 */
#define VP_QUALITY_MASK 	0x03	// Video 영상 품질
#define VP_NORMAL_VIDEO 	0x08	// Video parameter : 일반영상 모드
#define VP_RESOLUTION_MASK 	0x70	// 해상도 종류 mask : 0이면 각 영상 품질별 default해상도 (320x240, 640x480, 1280x720)
#define VP_BLOCK_NOTIFY 	0x80	// 착신 단말이 video 비 활성화시  UI_MSG_VIDEO_ON_NOTIFY의 video_para에 set됨.


#ifdef  __cplusplus
};  // extern "C"
#endif
