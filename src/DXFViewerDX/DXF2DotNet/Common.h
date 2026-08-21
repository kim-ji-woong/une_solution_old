#pragma once

namespace DXFDotNet
{
	wchar_t* ToWcharArray(System::String^ str);
	System::String^ ToSystemString(wchar_t* str);

	public enum class UnitOfLength
	{
		MILLIMETER = 0, CENTIMETER, METER, INCH, FEET 
	};
}

#define	OUT							[System::Runtime::InteropServices::OutAttribute]
