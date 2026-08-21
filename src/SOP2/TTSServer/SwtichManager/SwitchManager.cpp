// 기본 DLL 파일입니다.

#include "stdafx.h"

#include "AlrimiSwitchProtect.h"

#include "SwtichManager.h"

#include "MMSystem.h"


char szSwitchProtectFileName[256];

#define	TIMER_PROTECT		999

void __stdcall CheckTimer(HWND, UINT, UINT_PTR idEvent, DWORD)
{
	//if( idEvent == TIMER_PROTECT)
		AlrimiSwitchProtect();
}

SwitchManager::AudioManager::AudioManager()
{
	m_nInterval = 2000;
}

bool SwitchManager::AudioManager::InitAudio()
{


	 if(InitAudioOut() != AUD_OK)
		return false;

	// 2) 응용프로그램이 실행된 경우에 외부 감시를 중단하도록 명령한다.
	FILE *fp = NULL;
	fp = fopen("./AlrimiSwitchProtect.cfg", "r"); 
	if(fp != NULL)
	{
		fscanf(fp, "%s", szSwitchProtectFileName);
		fclose(fp);
		
		fp = fopen(szSwitchProtectFileName, "w"); // "AlrimiSwitchProtect.exe에서 인식할 수 있는 파일이름을 만든다
		fclose(fp);
	}
	AlrimiSwitchProtect();
	// 3) 알리미 스위치를 응용프로그램 내부에서 직접 감시한다.  2초간격(변경 가능함)
	SetTimer(NULL, TIMER_PROTECT, m_nInterval, CheckTimer);
	return true;
}

bool SwitchManager::AudioManager::Start()
{

	// 방송하기
	SwitchControl(1); // 1 : start
	return true;
}

bool SwitchManager::AudioManager::Stop()
{
	// 방송 중지
	SwitchControl(0); // 0 : stop
	return true;
}

bool SwitchManager::AudioManager::ClearAudio()
{
	// 1) 오디오 해제
	UninitAudioOut();

	// 2) 내부 감시를 중단한다
	KillTimer(NULL, TIMER_PROTECT);

	// 3) 외부 감시 시작. 감시 중지 파일을 삭제하여 외부 감시가 시작되도록 한다
	DeleteFileA((LPCSTR)szSwitchProtectFileName);
	

	return true;
}

void SwitchManager::AudioManager::SetInterval( int nInterval )
{
	m_nInterval = nInterval;
}

bool SwitchManager::AudioManager::CheckSwitch()
{
	//AlrimiSwitchProtect();
	return true;
}
