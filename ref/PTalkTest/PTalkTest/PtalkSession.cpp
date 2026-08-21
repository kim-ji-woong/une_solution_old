
#include "stdafx.h"
#include "PtalkSession.h"
#include "ptalkapi.h"

#include "SoundIn.h"
#include "SoundFile.h"

#include <mmsystem.h>
#pragma comment (lib, "winmm.lib")


using namespace std;

CSoundIn soundInMic;

PtalkSession::PtalkSession()
{
	soundInMic.DataFromSoundIn = DataFromSoundIn;
	m_nTalkType = 1;
}

PtalkSession::~PtalkSession()
{

}

void PtalkSession::RecvMessage(int nMsg, int arg1, int arg2)
{
	System::Diagnostics::Trace::WriteLine("RecvMesg :" + nMsg + " arg1 :" + arg1 + " arg2 :" + arg2);
	if (nMsg == UI_MSG_AUTH_STATE_RES)
	{
		int ret = arg1 & 0xff;
		if (ret != UI_RESULT_SUCCESS)
		{
			System::Diagnostics::Trace::WriteLine("인증 실패");
		}
		else

		{
			System::Diagnostics::Trace::WriteLine("인증 성공");
		}
	}
	else if (nMsg == UI_MSG_REG_STATE_RES)
	{
		System::Diagnostics::Trace::WriteLine("서버 등록");
	}
	else if (nMsg == UI_MSG_DEREG_RES || nMsg == UI_MSG_DEREG_NOTIFY)
	{
		System::Diagnostics::Trace::WriteLine("인증해제 등록");
	}
	else if (nMsg == UI_MSG_PTT_ON_RES)
	{
		int ret = (arg1 & 0xff);
		if (ret == UI_RESULT_SUCCESS)
		{
			System::Diagnostics::Trace::WriteLine("전송 시작");
			PtalkCmd(RC_RECORD_START, NULL, 0);
		}
	}
}

void PtalkSession::DoLogin(char* url, char * _usrID, char *_usrPw)
{
	char net_stat[1] = { 0x01 };
	//PtalkCmd(RC_SET_NET_STATE, net_stat, sizeof(net_stat));
	AuthReq( url, _usrID, _usrPw);
}

int PtalkSession::CallPrivatePTT(unsigned int nSid, int nType)
{
	m_nTalkType = nType;
	int ret = 0;
	unsigned int peer = htonl(nSid);
	
	ret = PtalkCmd( RC_PCALL_REQ, (char*)&peer, sizeof(peer));
	return ret;
}

int PtalkSession::CallGroupPTT(int nGroupID)
{
	char peer[1] = { 0 };

	peer[0] = nGroupID & 0xff;
	int ret = PtalkCmd( RC_GCALL_REQ, (char*)&peer, sizeof(peer));
	return ret;
}

void PtalkSession::PTTOff()
{
	PtalkCmd( RC_PTT_OFF, NULL, 0);
}

void PtalkSession::CallEnd()
{
	PtalkCmd( RC_CALL_END, NULL, 0);
}

void PtalkSession::DoLogout()
{
	PtalkCmd( RC_DEREG, NULL, 0);
}

void PtalkSession::PlayTalk()
{
	if (m_nTalkType == 1)
		soundInMic.Start();
	else if (m_nTalkType == 2)
		DataFromSoundFile();
}

void PtalkSession::StopTalk()
{
	if (m_nTalkType == 1)
		soundInMic.Stop();
	else if (m_nTalkType == 2)
		;//soundInTTS->Stop();
}

void PtalkSession::PlayTTS(long sid, std::string msg)
{
	System::Speech::Synthesis::SpeechSynthesizer^ ss = gcnew System::Speech::Synthesis::SpeechSynthesizer();
	//ss->Volume = 100;
	ss->SelectVoiceByHints(System::Speech::Synthesis::VoiceGender::Female, System::Speech::Synthesis::VoiceAge::Adult);

	array<byte>^ formatSpecificData = gcnew array<byte>(0);
	System::Speech::AudioFormat::SpeechAudioFormatInfo^ synthFormat =
		gcnew System::Speech::AudioFormat::SpeechAudioFormatInfo(System::Speech::AudioFormat::EncodingFormat::Pcm,
		16000, 16, 1, 32000, 2, formatSpecificData);

	ss->SetOutputToWaveFile("c:/temp/test.wav", synthFormat);

	std::string sss = "      " + msg;
	System::String^ szText = gcnew System::String(sss.c_str());
	ss->Speak(szText);
	m_nTalkType = 2;

	delete ss;

	unsigned int peer = htonl(sid);

	int ret = PtalkCmd(RC_PCALL_REQ, (char*)&peer, sizeof(peer));
}

void PtalkSession::SendLMS(long sid, std::string msg)
{
	
}

void PtalkSession::DataFromSoundFile()
{	
	int m_data_size;
	HGLOBAL mh_read_data = NULL;

	// test.wav 파일을 읽기모드로 연다.
	HMMIO hmmio = mmioOpenA("c:/temp/test.wav", NULL, MMIO_READ);

	// 파일을 성공적으로 연 경우...
	if (hmmio != NULL){
		// 부모 청크 정보를 저장할 구조체
		MMCKINFO chunk_info_parent;

		// 부모 청크에 명시되어 있는 "WAVE" 값을 구조체에 기록한다.
		chunk_info_parent.fccType = mmioFOURCC('W', 'A', 'V', 'E');
		// 부모 청크를 찾는다.
		// 성공적으로 부모 청크를 찾은 경우...
		if (mmioDescend(hmmio, &chunk_info_parent, NULL, MMIO_FINDRIFF) == MMSYSERR_NOERROR){
			// 자식 청크와 데이터 청크를 저장할 구조체
			MMCKINFO chunk_info_child;

			// 자식 청크에 명시되어 있는 "fmt" 값을 구조체에 기록한다.
			chunk_info_child.ckid = mmioFOURCC('f', 'm', 't', ' ');
			// 자식 청크를 찾는다. 세번째 인자에 위에서 얻은 부모 청크의 주소를 넘겨서 해당 자식 청크를
			// 얻을 수 있도록 한다. 성공적으로 자식 청크를 찾은 경우...
			if (mmioDescend(hmmio, &chunk_info_child, &chunk_info_parent, MMIO_FINDCHUNK)
				== MMSYSERR_NOERROR){
				// 웨이브 포맷 정보를 저장할 구조체
				WAVEFORMATEX wave_record;
				// chunk_info_child.cksize 크기의 정보를 파일에서 읽어서 wave_record 구조체에 저장한다.
				::mmioRead(hmmio, (HPSTR)&wave_record, chunk_info_child.cksize);

				// 데이터 청크에 명시되어 있는 "data" 값을 구조체에 기록한다.
				chunk_info_child.ckid = mmioFOURCC('d', 'a', 't', 'a');
				// 데이터 청크를 찾는다.
				if (mmioDescend(hmmio, &chunk_info_child, &chunk_info_parent, MMIO_FINDCHUNK)
					== MMSYSERR_NOERROR){
					// 데이터의 크기를 구한다.
					m_data_size = chunk_info_child.cksize;

					// 글로벌 메모리 영역을 할당받았다면 할당을 해제한다.
					if (mh_read_data != NULL)
						::GlobalFree(mh_read_data);

					// 데이터의 크기만큼 글로벌 메모리 영역을 할당받는다.
					mh_read_data = ::GlobalAlloc(GMEM_MOVEABLE, m_data_size);
					// 메모리를 사용할 수 있게 p_data와 연결시킨다.
					char *p_data = (char *)::GlobalLock(mh_read_data);

					// 해당 메모리 영역에 웨이브 파일에서 읽은 데이터를 읽어서 저장한다.
					mmioRead(hmmio, p_data, m_data_size);
						
					BYTE * pt = (BYTE*)p_data;
					for (int i = 0; i < m_data_size; i += 1920)
					{
						int nOutSize = m_data_size - i;
						if (nOutSize > 1920)
							nOutSize = 1920;

						sendVoice((short*)pt, nOutSize);
						pt += nOutSize;
						Sleep(60);
					}
					// p_data와 글로벌 메모리의 연결을 해제한다.
					::GlobalUnlock(p_data);
				}
			}
		}
		// 웨이브 파일을 닫는다.
		mmioClose(hmmio, 0);
	}
}

void PtalkSession::DataFromSoundIn(CBuffer* buffer, void* Owner)
{
	sendVoice((short*)(buffer->ptr.s), buffer->ByteLen);
}