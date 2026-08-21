//=============================================================
//
// C4 Engine version 2.9.1
// Copyright 1999-2012, by Terathon Software LLC
//
// This copy is licensed to the following:
//
//     Registered user: Soo Ki Kim
//     Maximum number of users: 1
//     License #C4T0035002
//
// License is granted under terms of the license agreement
// entered by the registed user.
//
// Unauthorized redistribution of source code is strictly
// prohibited. Violators will be prosecuted.
//
//=============================================================


#include "C4Base.h"


using namespace C4;


#if C4DEBUG

	void C4::Assert(bool condition, const char *message)
	{
		if (!condition)
		{
			#if C4WINDOWS
			
				__debugbreak();
			
			#elif C4MACOS
			
				Debugger();
			
			#elif C4LINUX
			
				raise(SIGTRAP);
			
			#elif C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

			#endif //]
		}
	}

#endif

// ZYURVUR
