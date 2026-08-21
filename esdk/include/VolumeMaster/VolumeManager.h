#pragma once
class VolumeManager
{
public:
	VolumeManager();
	virtual ~VolumeManager();

	static bool IsMute();
	static void SetMute(bool mute);
	// nVolume : 0 ~ 100
	static void ChangeVolume(int nVolume, bool mute);
	static int GetVolume();
};
