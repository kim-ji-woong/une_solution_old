// UZip.h

#pragma once

using namespace System;

namespace Core
{

	public ref class UZip
	{

	public:
		static bool ExtractFile( System::String^ arName, System::String^ path );
		static bool CompressFile( System::String^ arName, System::Collections::ArrayList^ arFileOrDirList);

		static bool  CompressRecusive( System::String^ arName, System::Collections::ArrayList^ arFileOrDirList );
		
	};
}
