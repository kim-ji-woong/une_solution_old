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


#ifndef C4Main_h
#define C4Main_h


#include "C4Types.h"


#if C4WINDOWS

	int WINAPI WinMain(HINSTANCE, HINSTANCE, LPSTR, int);

#elif C4MACOS

	C4API void MacMain(int argc, const char **argv);
	int main(int argc, const char **argv);

#elif C4LINUX

	int main(int argc, const char **argv);

#elif C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

#endif //]


#endif

// ZYURVUR
