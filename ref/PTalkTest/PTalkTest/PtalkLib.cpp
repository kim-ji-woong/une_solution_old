#include "stdafx.h"
#include "PtalkLib.h"

#include "ptalkapi.h"
#include "PtalkSession.h"


#include <string>


namespace UnE
{
	namespace TRS
	{
		PtalkSession* gSession;

		PTalkLib::PTalkLib(void)
		{
			trsNumber = 100150003;
			szServerName = nullptr;
			szLoginID = nullptr;
			szPass = nullptr;

		}

		PTalkLib::~PTalkLib()
		{

		}

		void PTalkLib::SetTrsNumber(long nLong)
		{
			trsNumber = nLong;
		}
	
		void PTalkLib::SetLoginInfo(String^ szServer, String^ szID, String^ szPass)
		{
			szServerName = szServer;
			szLoginID = szID;
			this->szPass = szPass;
		}

		void LogWrite(const void *msg, int len)
		{

		}

		// 메시지 함수
		// Session Index, Message Type
		void RecvMessage(int msg, int arg1, unsigned int arg2)
		{
			if (gSession != NULL)
			{
				gSession->RecvMessage(msg, arg1, arg2);
			}
		}

		// PTT 음성 수신시에 Raw PCM 데이터를 받을 CallBack 함수
		// Session Index, PCM16 Voice, 배열의 크기

		void PlayerWrite(short *data, int len)
		{
		}

		// PTT 음성 수신이 시작되었음
		// Session Index, 음성 품질
		void PlayerStart(int wideband)
		{
		}
		
		// PTT 음성 수신이 중단되었음
		// Session Index
		void PlayerStop()
		{
		}

		// PTT 음성 송신이 준비되었음
		void RecorderStart()
		{
			if (gSession != NULL)
			{
				gSession->PlayTalk();
			}
			
		}

		// PTT 음성 송신이 중지되었음
		void RecorderStop()
		{
			if (gSession != NULL)
			{
				gSession->StopTalk();
			}
		}

		void PTalkLib::RegisterCallBack()
		{
			setFunc_LogWrite(LogWrite);
			setFunc_rcv_msg(RecvMessage);
			setFunc_PlayerWrite(PlayerWrite);
			setFunc_PlayerStart(PlayerStart);
			setFunc_PlayerStop(PlayerStop);
			setFunc_RecorderStart(RecorderStart);
			setFunc_RecorderStop(RecorderStop);
		}
		
		bool PTalkLib::InitPtalk()
		{
			if (szServerName == nullptr || szLoginID == nullptr || szPass == nullptr)
				return false;
			if (trsNumber == -1)
				return false;

			RegisterCallBack();
				

			char buf[4096];
			wchar_t * t = ToWcharArray(szServerName);
			WideToMulti(buf, t, CP_ACP);
			std::string strServer = std::string(buf);
			delete[] t;

			t = ToWcharArray(szLoginID);
			WideToMulti(buf, t, CP_ACP);
			std::string strID = std::string(buf);
			delete[] t;

			t = ToWcharArray(szPass);
			WideToMulti(buf, t, CP_ACP);
			std::string strPass = std::string(buf);
			delete[] t;

			
			gSession = new PtalkSession();
			PtalkStart(0, trsNumber);
			gSession->DoLogin((char*)strServer.c_str(), (char*)strID.c_str(), (char*)strPass.c_str());

			return true;
		}

		void PTalkLib::PttOff()
		{
			if (gSession != NULL)
			{
				gSession->PTTOff();
			}
		}

		void PTalkLib::CallEnd()
		{
			if (gSession != NULL)
			{
				gSession->CallEnd();
			}
		}

		void PTalkLib::CallPrivate(long id)
		{
			if (gSession != NULL)
			{
				gSession->CallPrivatePTT(id, 1);
			}
		}

		void PTalkLib::CallGroup(int nGroup)
		{
			if (gSession != NULL)
			{
				gSession->CallGroupPTT(1);
			}
		}

		void PTalkLib::SendLMS(long id, String^ szMSG)
		{
			if (gSession != NULL)
			{
				char buf[4096];
				wchar_t * t = ToWcharArray(szMSG);
				WideToMulti(buf, t, CP_ACP);
				std::string strMsg = std::string(buf);
				delete[] t;
				gSession->SendLMS(id, strMsg);
			}			
		}

		void PTalkLib::SendTTS(long id, String^ szMsg)
		{
			if (gSession != NULL)
			{
				char buf[4096];
				wchar_t * t = ToWcharArray(szMsg);
				WideToMulti(buf, t, CP_ACP);
				std::string strMsg = std::string(buf);
				delete[] t;
				gSession->PlayTTS(id, strMsg);
			}		
		}
	}
}
