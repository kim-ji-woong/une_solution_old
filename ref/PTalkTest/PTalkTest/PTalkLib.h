
#pragma once


using namespace System;

namespace UnE
{
	namespace TRS
	{
		public ref class PTalkLib
		{
		private:
			long trsNumber;
						
			String^ szServerName;
			String^ szLoginID;
			String^ szPass;

			void RegisterCallBack();

		public:
			PTalkLib();

			~PTalkLib();


			void SetTrsNumber(long nLong);
			void SetLoginInfo(String^ szServer, String^ szID, String^ szPass);

			bool InitPtalk();
					
			void CallEnd();
			void PttOff();
			void CallPrivate(long id);
			void CallGroup(int nGroup);
			void SendLMS(long id, String^ szMSG);
			void SendTTS(long id, String^ szMsg);

		};
	}
}

