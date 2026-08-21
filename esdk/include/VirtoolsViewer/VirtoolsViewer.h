// VirtoolsViewer.h

#pragma once
#include "UnEUtility/Common.h"

//using namespace System;

class CCustomPlayer;

namespace VirtoolsViewer {

	//EXPORT_CLASS(Player)
	public ref class Player
	{
		// TODO: 여기에 이 클래스에 대한 메서드를 추가합니다.
	public:
		Player();
		virtual ~Player();

	public:
		bool InitPlayer(System::IntPtr iMainWindow, System::IntPtr iRenderWindow, System::String^ strFileName);
		bool Process();
		int GetWindowWidth();
		int GetWindowHeight();
		bool Resize(int xPos, int yPos, int nWidth, int nHeight);
		bool SendMessage(System::String^ strObjectName, System::String^ strMsgName);
		void PauseInput(bool pause);

	protected:
		CCustomPlayer* m_pPlayer;
	};
}
