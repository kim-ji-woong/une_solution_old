// 기본 DLL 파일입니다.

#include "stdafx.h"

#include "VirtoolsViewer.h"
#include <string.h>
#include "CustomPlayer.h"
#include < vcclr.h >

BEGIN_NS(VirtoolsViewer)

static char* SystemStringToCharArray(System::String^ strSrc)
{
	if (strSrc == nullptr)
		return 0;

	int nLen = strSrc->Length;
	wchar_t* tempW = new wchar_t[nLen+1];

	for (int i=0;i<nLen;i++)
		tempW[i] = strSrc[i];
	tempW[nLen] = 0;

	int nLen2 = nLen * 2 + 1;

	char* temp = new char[nLen2];

	BOOL defaultChar;
	WideCharToMultiByte(CP_ACP, 0, tempW, -1, temp, nLen2, 0, &defaultChar);

	delete [] tempW;
	return temp;
}

Player::Player()
{
	m_pPlayer = 0;
}

Player::~Player()
{
	if (m_pPlayer)
	{
		delete m_pPlayer;
	}
}

bool Player::InitPlayer(System::IntPtr iMainWindow, System::IntPtr iRenderWindow, System::String^ strFileName)
{
	delete m_pPlayer;
	m_pPlayer = new CCustomPlayer();

	char* fileName = SystemStringToCharArray(strFileName);
	if (fileName == 0) return false;

	if (m_pPlayer->InitPlayer((HWND)(__int64)iMainWindow, (HWND)(__int64)iRenderWindow, 0, fileName, 0) == 0)
	{
		delete [] fileName;
		return false;
	}

	delete [] fileName;

	m_pPlayer->Reset();
	return true;
}

bool Player::Process()
{
	if (m_pPlayer == 0)
		return false;

	if (!m_pPlayer->Process(0))
	{
		delete m_pPlayer;
		m_pPlayer = 0;
		return false;
	}

	return true;
}

int Player::GetWindowWidth()
{
	if (m_pPlayer == 0)
		return 0;

	return m_pPlayer->WindowedWidth();
}

int Player::GetWindowHeight()
{
	if (m_pPlayer == 0)
		return 0;

	return m_pPlayer->WindowedHeight();
}

bool Player::Resize(int xPos, int yPos, int nWidth, int nHeight)
{
	if (m_pPlayer == 0)
		return false;

	CKRenderContext* rc = m_pPlayer->GetRenderContext();
	if (rc == 0) return false;

	return rc->Resize(xPos, yPos, nWidth, nHeight) == CK_OK;
}

bool Player::SendMessage(System::String^ strObjectName, System::String^ strMsgName)
{
	if (m_pPlayer == 0)
		return false;

	char* _strObjectName = SystemStringToCharArray(strObjectName);
	char* _strMsgName = SystemStringToCharArray(strMsgName);

	CKContext* ct = m_pPlayer->GetCKContext();
	if (ct == 0) return false;

	CK3dObject* pObject = (CK3dObject*)ct->GetObjectByName(_strObjectName);
	if (pObject == 0) return false;

	CKMessageType nMsgType = m_pPlayer->GetMessageManager()->AddMessageType(_strMsgName);
	CKMessage* pMessage = m_pPlayer->GetMessageManager()->SendMessageSingle(nMsgType, (CKBeObject *)pObject);

	return pMessage == 0 ? false : true;
}

void Player::PauseInput(bool pause)
{
	if (m_pPlayer == 0)
		return;

	m_pPlayer->PauseInput(pause);
}

END_NS
