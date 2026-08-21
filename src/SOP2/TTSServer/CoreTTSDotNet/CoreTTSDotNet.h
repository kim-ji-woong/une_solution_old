

#pragma once

using namespace System;

namespace CoreTTSDotNet
{
	public ref class CoreTTS
	{
	protected:

		// Is Init Core TTS Engine
		bool	m_bInit;

		// Last Message
		System::String^ m_LastMsg;

		// Message Count
		int				m_nMsgCount;
		
		// Core TTS - MA Server ip주소,  -> cmd창에서 netstat -anb 의 결과로 ttsserver를 확인한 ip를 입력할것
		System::String^ m_TtsServer;

		// Core TTS - MA Server 포트번호
		int		m_nServerPort;

		// TTS Sound 포맷
		int		m_nSoundFormat;

		// Buffer용 Temp File 이름
		System::String^ m_szTempFile;

		// TTS 대기 타임아웃 시간(s)
		int		m_nSecTimeOut;

		// TTS 음성 볼륨
		float	m_fTTSVolume;

		// TTS 음성 속도
		int		m_nTTSSpeed;

	public:
		property bool Init
		{
			bool get() { return m_bInit; }
		}
		
		property int Speed
		{
			int get() { return m_nTTSSpeed; }
			void set(int value){ m_nTTSSpeed = value; }
		}
	
		property float Volume
		{
			float get() { return m_fTTSVolume; }
			void set(float value) { m_fTTSVolume = value; }
		}

		property int TimeOut
		{
			int get() { return m_nSecTimeOut; }
			void set(int value) { m_nSecTimeOut = value; }
		}

		property System::String^ TempFile
		{
			System::String^ get() { return m_szTempFile; }
			void set(System::String^ value) { m_szTempFile = gcnew System::String(value); }
		}
		
		//////////////////////////////////////////////////////////////////////////
		CoreTTS();
	
		bool InitEngine(System::String ^ szServerIP, int nPort);

		bool SpeakAsync(System::String^ szMsg, int nCount);

		bool Resume();
		bool Pause();
	
		bool SpeakAsyncCancelAll();

		bool ClearEngine();

		bool Create();
		bool Config(System::String ^ szServerIP, int nPort);
		
	};
}
