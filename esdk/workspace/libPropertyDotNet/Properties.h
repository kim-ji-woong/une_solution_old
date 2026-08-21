// libPropertyDotNet.h

#pragma once

#include "libProperty/Properties.h"

using namespace System;

namespace UnE
{
	namespace Utility
	{
		public ref class Properties
		{
		private:
			Properties(){};
			
		public:
			~Properties(){};

			static void SetProperty(System::String^ strKey, int nValue);
			static void SetProperty(System::String^ strKey, float nValue);
			static void SetProperty(System::String^ strKey, double nValue);
			static void SetProperty(System::String^ strKey, System::String^ nValue);

			static bool GetProperty(System::String^ strKey, System::String^ %value);
			static bool GetProperty(System::String^ strKey, int %value);	
			static bool GetProperty(System::String^ strKey, float %value);
			static bool GetProperty(System::String^ strKey, double %value);
		};
	}
}
