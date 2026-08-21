// 기본 DLL 파일입니다.

#include "stdafx.h"

#include "CoreTTSDotNet.h"

#include "CoreTtsApi.h"	




wchar_t* ToWcharArray(System::String^ str)
{
	if (str == nullptr)
		return 0;

	int nLen = str->Length;
	wchar_t* wstr = new wchar_t[nLen + 1];

	array<wchar_t>^ arr = str->ToCharArray();

	for (int i=0; i < nLen; i++)
		wstr[i] = arr[i];
	wstr[nLen] = 0;

	return wstr;
}

System::String^ ToSystemString(wchar_t* str)
{
	if (str == 0)
		return nullptr;

	System::String^ _str = gcnew System::String(L"");

	for (int i=0;str[i] != 0;i++)
	{
		_str += str[i];
	}

	return _str;
}

namespace CoreTTSDotNet
{
	//////////////////////////////////////////////////////////////////////////
	// Global Variable
	char gTtsServer[] =			"192.168.10.112";
	char gTtsBuffer[] =			"";
	bool gAllowAutoChannel =	true;
	//////////////////////////////////////////////////////////////////////////
	CoreTTSClient *client;		// 클라이언트 포인터 저장
	int  nChannel;		

	unsigned int TtsCallback(int nChannel, unsigned char* samples, int nSamples)
	{
		FILE *fp;
		char szFileName[_MAX_PATH];	// 합성음을 저장하는 파일이름을 위한 배열

		// 합성음 저장파일을 지정한다
		sprintf(szFileName, "voice%03d.dat", nChannel);

		// 전달받은 합성음을 저장한다
		if ((fp=fopen(szFileName, "ab")) == NULL) {
			//printf("File open error..\n", szFileName);
			return 0;
		}

		fwrite(samples, sizeof(char), nSamples, fp);
		fclose(fp);

		if( nSamples == 0)
		{
			TTSPlayFile(client, szFileName, WF_M16);
			if( CoreTTS::Instance != nullptr)
			{
				CoreTTS::Instance->PlayStarted();
			}
		}
		return nSamples;
	}

	CoreTTS::CoreTTS()
	{
		Instance = this;
		
		m_nSoundFormat = WF_M16;
		m_LastMsg = gcnew System::String("");
		//m_nServerPort = 20030;
		//m_TtsServer = gcnew System::String("127.0.0.1");

		m_nServerPort = 23456;
		m_TtsServer = gcnew System::String("218.235.67.30");

		m_szTempFile = gcnew System::String("temp.dat");	
		m_nSecTimeOut = 0;
		m_fTTSVolume = 2.0f;
		m_nTTSSpeed = 100;
	}

	bool CoreTTS::Create()
	{
		StartWSA();
		return true;
	}

	bool CoreTTS::Config( System::String ^ szServerIP, int nPort )
	{
		m_TtsServer = gcnew System::String(szServerIP);
		m_nServerPort = nPort;
		InitEngine(szServerIP, nPort);
		return true;
	}

	bool CoreTTS::InitEngine( System::String^ szServer, int nPort )
	{
		m_TtsServer = gcnew System::String(szServer);
		wchar_t * t = ToWcharArray(szServer);
		USES_CONVERSION;
		char * gTtsServer = W2A(t);
		delete [] t;	

		m_nServerPort = nPort;
		// 음성합성엔진과 접속하기 위한 클라이언트를 생성한다	
		client = TTSCreate(gTtsServer, nPort, m_nSoundFormat);		
		if (client == 0)
		{			
			CleanWSA();
			return false;
		}
		int nRet = TTSSetCallback(client, TtsCallback);		
		return true;
	}

	bool CoreTTS::ClearEngine()
	{	
		// 음성합성을 위해서 생성했던 클라이언트를 삭제한다
		int nRet = TTSDelete(client);
		if (nRet != TTS_OK)
		{
			CleanWSA();
			return false;			
		}
		return true;
	}

	bool CoreTTS::SpeakAsync( System::String^ szMsg , int nCount)
	{
		// 음성합성엔진 서버와 연결을 시도한다
		if (gAllowAutoChannel)
		{
			//client = NULL;
			// 음성합성엔진 내부에서 임의로 채널을 지정한다
			int nRet = TTSOpen(client);
			if (nRet != TTS_OK)
			{		
				if( client != NULL)
				{
					TTSDelete(client);
				}				
				CleanWSA();
				return false;
			}
		}
		else
		{
			// 사용하고자 하는 합성엔진 채널을 지정한다
			nChannel = 1;
			int nRet = TTSOpenChannel(client, nChannel);
			if (nRet != TTS_OK)
			{				
				if( client != NULL)
				{
					TTSDelete(client);
				}	
				CleanWSA();
				return false;
			}
		}
		if (gAllowAutoChannel)
		{
			nChannel = TTSGetChannel(client);
			if (nChannel == TTSERR_CHANNEL_ASSIGN) {
				//printf("TTSGetChannel Error[%X]: can't get channel number..\n", nChannel);
				
				if( client != NULL)
				{
					TTSClose(client);
					TTSDelete(client);
				}	
				CleanWSA();
				return false;
			}
		}

		//////////////////////////////////////////////////////////////////////////
		// option 설정
		//TTSSetTimeOut(client, m_nSecTimeOut);
		TTSSetVolume(client, (double)m_fTTSVolume);
		TTSSetSpeed(client, m_nTTSSpeed);

		m_LastMsg = gcnew System::String(szMsg);
		wchar_t * t2 = ToWcharArray(m_LastMsg);
		USES_CONVERSION;
		char * gTtsBuffer = W2A(t2);
		delete [] t2;

		char szFileName[512];
		sprintf(szFileName, "voice%03d.dat", nChannel);
		remove(szFileName);
		
		//TTSSetPlaySound(client, 1);

		// 합성하고자 하는 문장을 서버로 전송하고 합성음을 전달받는다
		int nRet = TTSGetSpeechStream(client, gTtsBuffer);
		if (nRet != TTS_OK)
		{	
			if( client != NULL)
			{
				TTSClose(client);
				TTSDelete(client);
			}	
			
			CleanWSA();
			return false;
		}

		/*int nRet = TTSGetSpeech(client, gTtsBuffer, szFileName);
		if (nRet != TTS_OK)
		{
		TTSClose(client);
		TTSDelete(client);
		CleanWSA();
		return false;
		}*/
		
		nRet = TTSClose(client);
		if (nRet != TTS_OK)
		{
			if( client != NULL)
			{
				TTSDelete(client);
			}		
			CleanWSA();
			return false;
		}	
		return true;
	}

	bool CoreTTS::Resume()
	{
		return false;
	}

	bool CoreTTS::Pause()
	{
		return false;
	}

	bool CoreTTS::SpeakAsyncCancelAll()
	{
		if( client != NULL)
			int nRet = TTSStopPlay(client);	
		return false;
	}

	bool CoreTTS::PlayStarted()
	{
		if( PlayCallBack != nullptr)
		{
			PlayCallBack();
			return true;
		}
		return false;
	}


	
}




//
//
//
//// =======================================================================
//
//int main(int argc, char* argv[])
//{
//	char szFileName[_MAX_PATH];	// 합성음을 저장하는 파일이름을 위한 배열
//	
//	
//	int  nRet;					// 함수의 리턴값
//
//	// 윈도우즈 소켓을 초기화 한다
//	StartWSA();
//
//	// 합성을 저장 파일을 지정한다
//	sprintf(szFileName, "test.dat");
//
//	// 음성합성엔진과 접속하기 위한 클라이언트를 생성한다	
//	client = TTSCreate(gTtsServer, gTtsServePort, gSpeechFormat);
//	if (client == NULL) {
//		printf("TTSCreate Error: can't create client..\n");
//		CleanWSA();
//		exit(0);
//	}
//	printf("TTSCreate: ok..\n");
//
//	// 음성합성엔진 서버와 연결을 시도한다
//	if (gAllowAutoChannel) {
//		// 음성합성엔진 내부에서 임의로 채널을 지정한다
//		nRet = TTSOpen(client);
//		if (nRet != TTS_OK) {
//			printf("TTSOpen Error[%X]: can't connect to %s..\n", 
//				nRet, gTtsServer);
//			TTSDelete(client);
//			CleanWSA();
//			exit(0);
//		}
//		printf("TTSOpen: ok..\n");
//	}
//	else {
//		// 사용하고자 하는 합성엔진 채널을 지정한다
//		nChannel = 1;
//		nRet = TTSOpenChannel(client, nChannel);
//		if (nRet != TTS_OK) {
//			printf("TTSOpenChannel Error[%X]: can't connect to %s/%03d..\n", 
//				nRet, nChannel, gTtsServer);
//			TTSDelete(client);
//			CleanWSA();
//			exit(0);
//		}
//		printf("TTSOpenChannel: ok..\n");
//	}
//
//	// 합성하고자 하는 문장을 서버로 전송하고 합성음을 전달받는다
//	nRet = TTSGetSpeech(client, gTtsBuffer, szFileName);
//	if (nRet != TTS_OK) {
//		printf("TTSGetSpeech Error[%X]: can't get speech..\n", nRet);
//		TTSClose(client);
//		TTSDelete(client);
//		CleanWSA();
//		exit(0);
//
//	}
//	printf("TTSGetSpeech: ok..\n");
//
//
//	TTSPlayFile(client, szFileName, WF_M16);
//
//
//	// 음성합성엔진 서버와 연결을 해제한다
//	nRet = TTSClose(client);
//	if (nRet != TTS_OK) {
//		printf("TTSClose Error[%X]: can't close connection with %s..\n", nRet, gTtsServer);
//		TTSDelete(client);
//		CleanWSA();
//		exit(0);
//	}
//	printf("TTSClose: ok..\n");
//
//
//
//
//
//	// 음성합성을 위해서 생성했던 클라이언트를 삭제한다
//	nRet = TTSDelete(client);
//	if (nRet != TTS_OK) {
//		printf("TTSDelete Error[%X]: can't delete TTS client..\n", nRet);
//		CleanWSA();
//		exit(0);
//	}
//	printf("TTSDelete: ok..\n");
//
//	// 윈도우즈 소켓 사용을 해제한다
//	CleanWSA();
//
//	return 0;
//}
//
