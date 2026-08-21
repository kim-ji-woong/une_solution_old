#pragma once
#include <string>

namespace UnE
{
#ifdef UNICODE
	typedef std::wstring tstring;
#else
	typedef std::string tstring;
#endif
}
