#ifndef _CORETTS_API_H_
#define _CORETTS_API_H_

#ifndef CORETTS_API
#ifdef _WIN32
#define CORETTS_API __declspec(dllimport)
#define TTS_CALLING __cdecl
#else
#define CORETTS_API extern 
#define TTS_CALLING
#endif
#endif

#ifdef __cplusplus
extern "C" {
#endif

/* 합성음의 포맷을 정의한다 */
typedef enum {
	WF_M8=1,
	WF_M8_WAV,
	WF_L16,
	WF_L16_WAV,
	WF_M16,
	WF_M16_WAV,
	WF_A8,
	WF_A8_WAV,
	WF_L8,
	WF_L8_WAV,
	WF_M16L,
	WF_M16L_WAV,
	WF_M8L,
	WF_M8L_WAV,
	WF_M8L_VOX,
	WF_A8L,
	WF_A8L_WAV
} WF_TYPE;

/* API의 리턴 타입을 정의한다 */
#define TTS_OK						0

/* 오류 타입을 정의한다 */
typedef enum {
	TTSERR_WSA_START=0x1001,
	TTSERR_WSA_CLEAN,
	TTSERR_SOCKET_CONNECTION,
	TTSERR_SOCKET_MAX_RETRY,
	TTSERR_SOCKET_INVALID,
	TTSERR_SOCKET_ERROR,
	TTSERR_SOCKET_SHUTDOWN,
	TTSERR_HOST_INVALID,
	TTSERR_NO_CLIENT,
	TTSERR_PORT_RANGE,
	TTSERR_CHANNEL_RANGE,
	TTSERR_ALREADY_INIT,
	TTSERR_NOT_OPENED,
	TTSERR_REJECTED,
	TTSERR_CHANNEL_ASSIGN,
	TTSERR_NOT_SYN_PKT,
	TTSERR_SYN_OPEN,
	TTSERR_SYN_READ,
	TTSERR_SYN_WRITE,
	TTSERR_WAVE_OPEN,
	TTSERR_WAVE_FORMAT,
	TTSERR_WAVE_HEADER,
	TTSERR_LIP_OPEN,
	TTSERR_LIP_READ,
	TTSERR_LIP_WRITE,
	TTSERR_MAX_WAIT_TIME,
	TTSERR_STATUS_READ,
	TTSERR_NO_RESPONSE,
	TTSERR_INVALID_RANGE,
	TTSERR_MEM_ALLOC
} TTSERR_TYPE;

typedef void CoreTTSClient;

/* ====================================================================
윈도우즈 소켓을 초기화하는 함수이다. 음성합성엔진 API를 사용하는 서비스
프로그램에서 윈도우즈 소켓을 초기화하지 않은 경우 한번만 이 함수를 호출
하면 된다. 
1. 리턴값: 정상적이면 TTS_OK 를, 그렇지 않으면 오류 값을 리턴한다.
==================================================================== */
CORETTS_API int TTS_CALLING StartWSA();

/* ====================================================================
윈도우즈 소켓 사용을 해제하는 함수이다. 음성합성엔진 API를 사용하는 
서비스 프로그램에서 윈도우즈 소켓 사용을 해제하지 않은 경우, 서비스 
프로그램 종료시 한번만 이 함수를 호출하면 된다. 
1. 리턴값: 정상적이면 TTS_OK 를, 그렇지 않으면 오류 값을 리턴한다.
==================================================================== */
CORETTS_API int TTS_CALLING CleanWSA();

/* ====================================================================
음성합성엔진을 프로세서로 구동하는 경우 음성합성엔진의 구동상태를 파악
하는 함수이다. 이 함수를 사용하기 위해서는 음성합성엔진과 클라이언트 API가
같은 시스템에서 동작하고 있어야 한다.
1. 입력파라미터
     - pStatus: 음성합성엔진의 구동상태를 나타내는 것으로서, 0 에서 100 
	   사이의 값을 가진다. 음성합성엔진이 정상적으로 구동 완료 되면 이값은 
	   100 을 가리킨다.
	 - pszMsg: 음성합성엔진의 상태 메세지를 받는 것으로서, 메세지의 최대 
	   크기는 255 bytes 이다.
	 - nMaxWaitTime: 음성합성엔진의 상태를 확인하는 최대 시간으로서, 단위는 
	   초 이다. 함수는 이 시간 동안 상태를 확인할 수 없으면 오류 값을 리턴한다. 
2. 리턴값: 정상적이면 TTS_OK 를, 그렇지 않으면 오류 값을 리턴한다.
==================================================================== */
CORETTS_API int TTS_CALLING GetStatusMsgFromTTS(
	int *pStatus, char *pszMsg, unsigned int nMaxWaitTime);

/* ====================================================================
음성합성엔진을 프로세서로 구동하는 경우 음성합성엔진이 구동완료 될 때까지
기다리는 함수이다. 이 함수를 사용하기 위해서는 음성합성엔진과 클라이언트 
API가 같은 시스템에서 동작하고 있어야 한다.
1. 입력파라미터
	 - nMaxWaitTime: 음성합성엔진의 구동완료를 기다리는 최대 시간으로서, 
	   단위는 초 이다. 함수는 이 시간 동안 상태를 확인할 수 없으면 오류 
	   값을 리턴한다. 
2. 리턴값: 정상적이면 TTS_OK 를, 그렇지 않으면 오류 값을 리턴한다.
==================================================================== */
CORETTS_API int TTS_CALLING GetWaitMsgFromTTS(
	unsigned int nMaxWaitTime);

/* ====================================================================
음성합성엔진 서버와 접속하기 위한 클라이언트를 생성하는 함수이다.
1. 입력파라미터: 
     - pszRemoteIP: 음성합성엔진이 동작하는 서버 주소이며, 숫자로 구성된
       dotted format IP주소이다. 예) 210.107.136.51
	 - nPort: 음성합성엔진이 동작하는 포트번호이며, -1 을 입력하면
	   디폴트 포트번호로 동작한다. 입력가능범위는 1 에서 65535 이다.
     - nFileFormat: TTSGetSpeech() 함수를 사용하는 경우, 저장하고자 하는 
	   음성파일의 포맷을 지정하는 값으로서, 현재 지원하는 포맷은 다음과 같다.
	   [PC용 엔진인 경우]
		 - WF_L16     : 16kHz/16bit/mono 인 linear PCM 포맷
	     - WF_L16_WAV : 16kHz/16bit/mono 인 linear PCM WAV 포맷
		 - WF_M16     : 16kHz/8bit/mono 인 mu-Law PCM 포맷
		 - WF_M16_WAV : 16kHz/8bit/mono 인 mu-Law PCM WAV 포맷
		 - WF_M16L    : 16kHz/16bit/mono 인 linear PCM 포맷
						(8bit->16bit conversion)
	     - WF_M16L_WAV: 16kHz/16bit/mono 인 linear PCM WAV 포맷
						(8bit->16bit conversion)
	   [전화망용 엔진인 경우]
		 - WF_M8      : 8kHz/8bit/mono 인 mu-Law PCM 포맷
		 - WF_M8_WAV  : 8kHz/8bit/mono 인 mu-Law PCM WAV 포맷
		 - WF_A8      : 8kHz/8bit/mono 인 a-Law PCM 포맷
		 - WF_A8_WAV  : 8kHz/8bit/mono 인 a-Law PCM WAV 포맷
		 - WF_L8      : 8kHz/16bit/mono 인 linear PCM 포맷
		 - WF_L8_WAV  : 8kHz/16bit/mono 인 linear PCM WAV 포맷
		 - WF_M8L     : 8kHz/16bit/mono 인 linear PCM 포맷
						(8bit->16bit conversion)
		 - WF_M8L_WAV : 8kHz/16bit/mono 인 linear PCM WAV 포맷
						(8bit->16bit conversion)
		 - WF_M8L_VOX : 8kHz/4bit/mono 인 Dialogic ADPCM 포맷
						(8bit->4bit conversion)
		 - WF_A8L     : 8kHz/16bit/mono 인 linear PCM 포맷
						(8bit->16bit conversion)
		 - WF_A8L_WAV : 8kHz/16bit/mono 인 linear PCM WAV 포맷
						(8bit->16bit conversion)
2. 리턴값: 정상적이면 클라이언트 포인터를, 그렇지 않으면 NULL을 리턴한다.
==================================================================== */
CORETTS_API CoreTTSClient* TTS_CALLING TTSCreate(
	char *pszRemoteIP, int nPort, int nFileFormat);

/* ====================================================================
음성합성엔진 서버와 접속하기 위해서 생성했던 클라이언트를 삭제하는 
함수이다.
1. 입력파라미터
     - client: 클라이언트 포인터.
2. 리턴값: 정상적이면 TTS_OK 를, 그렇지 않으면 오류 값을 리턴한다.
==================================================================== */
CORETTS_API int TTS_CALLING TTSDelete(
	CoreTTSClient *client);

/* ====================================================================
TTSCreate()함수에서 넘겨받은 서버주소를 이용하여 음성합성엔진 서버와 
접속을 수행하는 함수이다. 
1. 입력파라미터
     - client: 클라이언트 포인터.
2. 리턴값: 정상적이면 TTS_OK 를, 그렇지 않으면 오류 값을 리턴한다.
==================================================================== */
CORETTS_API int TTS_CALLING TTSOpen(
	CoreTTSClient *client);

/* ====================================================================
TTSCreate()함수에서 넘겨받은 서버주소를 이용하여 사용자가 지정한 채널로
음성합성엔진 서버와 접속을 수행하는 함수이다.
1. 입력파라미터
     - client: 클라이언트 포인터.
	 - nChannel: 사용자 지정 채널 번호로서, 1 부터 최대채널 사이의 값.
2. 리턴값: 정상적이면 TTS_OK 를, 그렇지 않으면 오류 값을 리턴한다.
==================================================================== */
CORETTS_API int TTS_CALLING TTSOpenChannel(
	CoreTTSClient *client, int nChannel);

/* ====================================================================
음성합성엔진 서버와 연결을 해제하는 함수이다.
1. 입력파라미터
     - client: 클라이언트 포인터.
2. 리턴값: 정상적이면 TTS_OK 를, 그렇지 않으면 오류 값을 리턴한다.
==================================================================== */
CORETTS_API int TTS_CALLING TTSClose(
	CoreTTSClient *client);

/* ====================================================================
합성하고자 하는 문장을 음성합성엔진 서버로 전송하고, 그 결과로 합성음을 
전달받는 함수이다.
1. 입력파라미터: 
     - client: 클라이언트 포인터.
     - pszBuffer: 합성하고자 하는 문장을 저장한 텍스트 버퍼로서 버퍼의 
	   끝에는 NULL이 있어야 함.
	 - pszFileName: 음성합성엔진 서버로 부터 전송받은 합성음을 저장하는
	   파일이름을 지정하는 것으로서 최대 255 bytes 까지 가능함.
2. 리턴값: 정상적이면 TTS_OK 를, 그렇지 않으면 오류 값을 리턴한다.
==================================================================== */
CORETTS_API int TTS_CALLING TTSGetSpeech(
	CoreTTSClient *client, const char *pszBuffer, const char *pszFileName);

/* ====================================================================
합성하고자 하는 문장을 음성합성엔진 서버로 전송하고, 그 결과로 합성음을 
전달받는 함수로서 TTSGetSpeech()와는 달리 합성음을 파일로 저장하지 않고, 
사용자의 callback함수에게 합성음을 전달한다.
1. 입력파라미터: 
     - client: 클라이언트 포인터.
     - pszBuffer: 합성하고자 하는 문장을 저장한 텍스트 버퍼로서 버퍼의 
	   끝에는 NULL이 있어야 함. 
2. 리턴값: 정상적이면 TTS_OK 를, 그렇지 않으면 오류 값을 리턴한다.
==================================================================== */
CORETTS_API int TTS_CALLING TTSGetSpeechStream(
	CoreTTSClient *client, const char *pszBuffer);

/* ====================================================================
클라이언트가 서버로 부터 합성음을 전송받은 경우, 사용자 프로그램에서 
이를 처리할 수 있도록 callback을 지원하는 함수이다.
1. 입력파라미터: 
     - client: 클라이언트 포인터.
     - pfn: 사용자 callback 함수.
	 - nId: 클라이언트 ID 이며, 1 부터 최대채널 사이의 값.
	 - pSamples: 합성음 데이터를 저장한 버퍼.
	 - nSamples: 합성음 버퍼에 저장된 데이터의 갯수이며 byte단위로 계산됨.
2. 리턴값: 정상적이면 TTS_OK 를, 그렇지 않으면 오류 값을 리턴한다.
==================================================================== */
CORETTS_API int TTS_CALLING TTSSetCallback(
	CoreTTSClient *client, 
	unsigned int (TTS_CALLING *pFn)(
		int nId, unsigned char *pSamples, int nSamples));

/* ====================================================================
클라이언트가 TTSOpen()을 이용하여 접속한 경우, 음성합성 엔진으로 부터
할당받은 채널번호를 얻는 함수이다.
1. 입력파라미터: 
     - client: 클라이언트 포인터.
2. 리턴값: 정상적이면 1 부터 최대채널 사이의 값을, 그렇지 않으면 
   오류 값을 리턴한다.
==================================================================== */
CORETTS_API int TTS_CALLING TTSGetChannel(
	CoreTTSClient *client);

/* ====================================================================
클라이언트가 정상적으로 서버에 접속한 경우, 합성기 엔진에서 생성하는
합성음의 피치값을 조절하는 함수이다.
1. 입력파라미터: 
     - client: 클라이언트 포인터.
	 - nPitch: 변경하고자 하는 피치레벨로서, 1-7 사이의 값(기본값은 3)을
	 가진다.
2. 리턴값: 정상적이면 TTS_OK 를, 그렇지 않으면 오류 값을 리턴한다.
==================================================================== */
CORETTS_API int TTS_CALLING TTSSetPitch(
	CoreTTSClient *client, int nPitch);

/* ====================================================================
클라이언트가 정상적으로 서버에 접속한 경우, 합성기 엔진에서 생성하는
합성음의 속도를 조절하는 함수이다.
1. 입력파라미터: 
     - client: 클라이언트 포인터.
	 - nSpeed: 변경하고자 하는 속도레벨로서, 1-5 사이의 값(기본값은 3)을
	 가진다.
2. 리턴값: 정상적이면 TTS_OK 를, 그렇지 않으면 오류 값을 리턴한다.
==================================================================== */
CORETTS_API int TTS_CALLING TTSSetSpeed(
	CoreTTSClient *client, int nPitch);

/* ====================================================================
클라이언트가 서버에 접속하는 경우 최대 접속 시도 횟수를 지정하는 함수
로서, TTSOpen() 또는 TTSOpenChannel() 함수를 호출하기 전에 사용한다.
1. 입력파라미터: 
     - client: 클라이언트 포인터.
	 - nRetry: 지정하고자 하는 접속 시도 횟수로서, 1보다 큰 값을 가지며 
	 기본값은 1이다.
2. 리턴값: 정상적이면 TTS_OK 를, 그렇지 않으면 오류 값을 리턴한다.
==================================================================== */
CORETTS_API int TTS_CALLING TTSSetMaxConRetry(
	CoreTTSClient *client, int nRetry);

/* ====================================================================
합성기 엔진에서 립싱크 정보를 생성하도록 지정하는 함수로서, TTSGetSpeech()
함수를 호출하기 전에 사용한다.
1. 입력파라미터: 
     - client: 클라이언트 포인터.
	 - nMode: TRUE 이면 립싱크 정보를 생성하고, FALSE 이면 립싱크 정보를
	 생성하지 않으며 기본값은 TRUE 이다.
2. 리턴값: 정상적이면 TTS_OK 를, 그렇지 않으면 오류 값을 리턴한다.
==================================================================== */
CORETTS_API int TTS_CALLING TTSSetLipSync(
	CoreTTSClient *client, int nMode);

/* ====================================================================
합성기 엔진으로 부터 립싱크 정보를 가져오는 함수로서, TTSGetSpeech() 함수가
성공한 다음에 사용한다.
1. 입력파라미터: 
     - client: 클라이언트 포인터.
	 - pszFileName: 음성합성엔진 서버로 부터 전송받은 립싱크 정보를 저장하는
	   파일이름을 지정하는 것으로서 최대 255 bytes 까지 가능함.
2. 리턴값: 정상적이면 TTS_OK 를, 그렇지 않으면 오류 값을 리턴한다.
==================================================================== */
CORETTS_API int TTS_CALLING TTSGetLipSync(
	CoreTTSClient *client, const char *pszFileName);

/* ====================================================================
TTSCreate()함수에서 넘겨받은 자원관리기 서버주소를 이용하여 음성합성엔진 
서버와 접속을 수행하는 함수이다. 이 함수는 자원관리기가 부하분산을 수행
하면서 항상 최적의 음성합성엔진 서버로 연결시켜준다.
1. 입력파라미터
     - client: 클라이언트 포인터.
2. 리턴값: 정상적이면 TTS_OK 를, 그렇지 않으면 오류 값을 리턴한다.
==================================================================== */
CORETTS_API int TTS_CALLING TTSOpenRM(
	CoreTTSClient *client);

/* ====================================================================
자원관리기와 연결을 해제하는 함수이다.
1. 입력파라미터
     - client: 클라이언트 포인터.
2. 리턴값: 정상적이면 TTS_OK 를, 그렇지 않으면 오류 값을 리턴한다.
==================================================================== */
CORETTS_API int TTS_CALLING TTSCloseRM(
	CoreTTSClient *client);

/* ====================================================================
합성기 엔진에서 음색변환된 음성을 생성하도록 지정하는 함수로서, 
TTSGetSpeech() 또는 TTSGetSpeechStream() 함수를 호출하기 전에 사용한다.
1. 입력파라미터: 
     - client: 클라이언트 포인터.
     - nMode: TRUE 이면 음색변환된 음성을 생성하고, FALSE 이면 정상적인
       합성음을 생성한다. 기본값은 FALSE 이다.
     - fVtl: 성도길이를 지정하는 것으로서, 0.8 에서 1.2 사이의 값을 지정할
       수 있다. 기본 값은 1.0 이다.
     - fPitch: 억양을 지정하는 것으로서, 0.8 에서 1.2 사이의 값을 지정할
       수 있다. 기본 값은 1.0 이다.
2. 리턴값: 정상적이면 TTS_OK 를, 그렇지 않으면 오류 값을 리턴한다.
==================================================================== */
CORETTS_API int TTS_CALLING TTSSetVoiceConversion(
	CoreTTSClient *client, int nVtl, int nPitch);

/* ====================================================================
합성기 엔진에서 사용하는 탐색공간을 지정하는 함수로서, TTSGetSpeech() 
또는 TTSGetSpeechStream() 함수를 호출하기 전에 사용한다.
1. 입력파라미터: 
     - client: 클라이언트 포인터.
     - nmaxCand: 탐색공간을 설정하는 값으로서, 50 에서 1000 사이의 값을 
       지정할 수 있다. 기본값은 150 이다.
2. 리턴값: 정상적이면 TTS_OK 를, 그렇지 않으면 오류 값을 리턴한다.
==================================================================== */
CORETTS_API int TTS_CALLING TTSSetMaxCand(
	CoreTTSClient *client, int nMaxCand);

/* ====================================================================
생성하고자 하는 합성음의 앞 뒤에 묵음을 삽입하는 함수로서, TTSGetSpeech() 
함수를 호출하기 전에 사용한다.
1. 입력파라미터: 
     - client: 클라이언트 포인터.
     - nStart: 합성음의 앞부분에 묵음을 삽입하고자 하는 경우에 초 단위로 
       값을 지정한다. 기본 값은 0 이다.
     - nEnd: 합성음의 뒷부분에 묵음을 삽입하고자 하는 경우에 초 단위로 
       값을 지정한다. 기본 값은 0 이다.
2. 리턴값: 정상적이면 TTS_OK 를, 그렇지 않으면 오류 값을 리턴한다.
==================================================================== */
CORETTS_API int TTS_CALLING TTSSetSpeechMargin(
	CoreTTSClient *client, int nStart, int nEnd);

/* ====================================================================
합성음 생성시 문장과 문장 사이에 묵음을 삽입하는 함수로서, TTSGetSpeech() 
또는 TTSGetSpeechStream()함수를 호출하기 전에 사용한다.
1. 입력파라미터: 
     - client: 클라이언트 포인터.
     - nMargin: 삽입하고자 하는 묵음의 길이를 1/1000 초 단위로 
       값을 지정한다. 기본 값은 150 이다.
2. 리턴값: 정상적이면 TTS_OK 를, 그렇지 않으면 오류 값을 리턴한다.
==================================================================== */
CORETTS_API int TTS_CALLING TTSSetSentenceMargin(
	CoreTTSClient *client, int nMargin);

/* ====================================================================
합성하고자 하는 문장에 대해서 띄어쓰기 유무를 지정하는 함수로서, 
TTSGetSpeech() 또는 TTSGetSpeechStream()함수를 호출하기 전에 사용한다.
1. 입력파라미터: 
     - client: 클라이언트 포인터.
     - nEnable: 자동으로 띄어쓰기를 하고자 하면 1, 그렇지 않으면 0 을
       지정한다. 기본 값은 1 이다.
2. 리턴값: 정상적이면 TTS_OK 를, 그렇지 않으면 오류 값을 리턴한다.
==================================================================== */
CORETTS_API int TTS_CALLING TTSSetWordSpacing(
	CoreTTSClient *client, int nEnable);

/* ====================================================================
합성하고자 하는 문장에 대해서 괄호읽기 유무를 지정하는 함수로서, 
TTSGetSpeech() 또는 TTSGetSpeechStream()함수를 호출하기 전에 사용한다.
1. 입력파라미터: 
     - client: 클라이언트 포인터.
     - nEnable: 괄호안의 내용을 읽어주고자 하면 1, 그렇지 않으면 0 을
       지정한다. 기본 값은 0 이다.
2. 리턴값: 정상적이면 TTS_OK 를, 그렇지 않으면 오류 값을 리턴한다.
==================================================================== */
CORETTS_API int TTS_CALLING TTSSetParenReading(
	CoreTTSClient *client, int nEnable);

/* ====================================================================
합성하고자 하는 문장에 대해서 줄바꿈기호 무시 유무를 지정하는 함수로서, 
TTSGetSpeech() 또는 TTSGetSpeechStream()함수를 호출하기 전에 사용한다.
1. 입력파라미터: 
     - client: 클라이언트 포인터.
     - nEnable: 줄바꿈기호를 무시하고자 하면 0, 그렇지 않으면 1 을
       지정한다. 기본 값은 1 이다.
2. 리턴값: 정상적이면 TTS_OK 를, 그렇지 않으면 오류 값을 리턴한다.
==================================================================== */
CORETTS_API int TTS_CALLING TTSSetReturnBreaking(
	CoreTTSClient *client, int nEnable);

/* ====================================================================
합성하고자 하는 문장의 일정부분에 대해서 음절단위 읽기 유무를 지정하는 
함수로서, TTSGetSpeech() 또는 TTSGetSpeechStream()함수를 호출하기 전에 
사용한다.
1. 입력파라미터: 
     - client: 클라이언트 포인터.
     - pszStart: 음절단위 읽기 시작을 나타내는 태그 문자열을 지정한다. 
       합성기엔진은 이 태그 분자열을 제외한 다음 문자부터 또박또박 읽는다. 
       입력가능한 문자열의 최대 길이는 8 바이트 이며, 기본 문자열은 << 이다.
	 - pszEnd: 음절단위 읽기 중지를 나타내는 태그 문자열을 지정한다. 
       합성기엔진은 이 태그 분자열의 앞까지 또박또박 읽는다. 
       입력가능한 문자열의 최대 길이는 8 바이트 이며, 기본 문자열은 >> 이다.
2. 리턴값: 정상적이면 TTS_OK 를, 그렇지 않으면 오류 값을 리턴한다.
==================================================================== */
CORETTS_API int TTS_CALLING TTSSetSyllableReading(
	CoreTTSClient *client, const char *pszStart, const char *pszEnd);

/* ====================================================================
클라이언트가 정상적으로 서버에 접속한 경우, 합성기 엔진에서 생성하는
합성음의 크기를 조절하는 함수이다.
1. 입력파라미터: 
     - client: 클라이언트 포인터.
	 - fVolume: 변경하고자 하는 볼륨 레벨로서, 0.10 에서 2.00 사이의 값
       (기본값은 1.00)을 가진다.
2. 리턴값: 정상적이면 TTS_OK 를, 그렇지 않으면 오류 값을 리턴한다.
==================================================================== */
CORETTS_API int TTS_CALLING TTSSetVolume(
	CoreTTSClient *client, double fVolume);


CORETTS_API int TTSPlayFile(CoreTTSClient *client, const char *pszFileName, int nFileFormat);
CORETTS_API int TTSStopPlay(CoreTTSClient *client);
CORETTS_API int TTSSetPlaySound(CoreTTSClient * client, int nMode);

#ifdef __cplusplus
}
#endif

#endif /* _CORETTS_API_H_ */
