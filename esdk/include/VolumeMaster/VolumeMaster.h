// VolumeMaster.h

#pragma once

using namespace System;

namespace VolumeMaster {

	public ref class Volume
	{
		// TODO: 여기에 이 클래스에 대한 메서드를 추가합니다.
	public:
		static bool IsMute();
		static void SetMute(bool mute);
		// nVolume : 0 ~ 100
		static void ChangeVolume(int nVolume, bool mute);
		static int GetVolume();
	};
}
