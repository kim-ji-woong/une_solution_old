#pragma once

namespace UnE
{
	namespace Hardware
	{
		public ref class MonitorInfo
		{
		public:
			MonitorInfo();

		public:
			static int GetUID(int nIndex, System::String^% strDeviceName, System::String^% strContainerID, System::Drawing::Point% position, System::Drawing::Size% size);

		private:
			static System::String^ GetContainerID(int uid, System::String^ strMonitorName);
			static System::Collections::Generic::List<System::String^>^ ReadRegKeys(System::String^ strRegPath);
			static System::String^ ReadRegValue(System::String^ strRegPath, System::String^ strKey);
		};
	}
}
