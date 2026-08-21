#pragma once

#include <string>
#include "Buffer.h"

class PtalkSession
{
public:
	PtalkSession();
	virtual ~PtalkSession();

	void RecvMessage(int msg, int arg1, int arg2);
	void DoLogin(char* url, char * _usrID, char *_usrPw);
	void DoLogout();

	int CallGroupPTT(int nGroupID);
	int CallPrivatePTT(unsigned int nSid, int nType);
	void PTTOff();
	void CallEnd();

	void PlayTalk();
	void StopTalk();
	void SendLMS(long sid, std::string msg);
	void PlayTTS(long sid, std::string msg);

	static void DataFromSoundIn(CBuffer* buffer, void* Owner);
	static void DataFromSoundFile();


private:


	int m_nTalkType;
};