// TTSManager.h

#pragma once

using namespace System;

namespace SwitchManager
{

	public ref class AudioManager
	{
	protected:
		int m_nInterval;
	public:
		AudioManager();

		void SetInterval(int nInterval);

		bool InitAudio();

		bool Start();

		bool Stop();

		bool ClearAudio();

		bool CheckSwitch();

	};
}
