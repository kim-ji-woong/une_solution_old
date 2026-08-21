// 기본 DLL 파일입니다.

#include "stdafx.h"
#include "VolumeMaster.h"
#include "VolumeManager.h"
//#include <mmdeviceapi.h>
//#include <endpointvolume.h>

namespace VolumeMaster
{
	bool Volume::IsMute()
	{
		return VolumeManager::IsMute();
	}

	// nVolume : 0 ~ 100
	void Volume::ChangeVolume(int nVolume, bool mute)
	{
		return VolumeManager::ChangeVolume(nVolume, mute);
	}

	void Volume::SetMute(bool mute)
	{
		return VolumeManager::SetMute(mute);
	}

	int Volume::GetVolume()
	{
		return VolumeManager::GetVolume();
	}
}