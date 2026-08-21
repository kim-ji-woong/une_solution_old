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


#include "C4Main.h"
#include "C4Engine.h"
#include "C4Graphics.h"
#include "C4Threads.h"


#ifndef APPLICATION_NAME

	#define APPLICATION_NAME "C4 Engine"

#endif


using namespace C4;


#if C4WINDOWS

	int WINAPI WinMain(HINSTANCE instance, HINSTANCE prevInstance, LPSTR commandLine, int cmdShow)
	{
		HANDLE mutex = CreateMutexA(nullptr, true, APPLICATION_NAME);
		if (GetLastError() == ERROR_SUCCESS)
		{
			#if C4DEBUG
			
				WIN32_FIND_DATA		fileData;
				
				HANDLE handle = FindFirstFileA("Data", &fileData);
				if (handle != INVALID_HANDLE_VALUE) FindClose(handle);
				
				if ((handle == INVALID_HANDLE_VALUE) || ((fileData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) == 0))
				{
					const char *message =	"The Data directory could not be found.\r\n"
											"Make sure you have set the working directory to \"..\\..\\..\" in the Debugging properties.";
					
					MessageBoxA(nullptr, message, APPLICATION_NAME, MB_OK | MB_ICONINFORMATION);
					return (0);
				}
			
			#endif
			
			Engine::New();
			
			EngineResult result = TheEngine->Initialize(APPLICATION_NAME, instance, commandLine);
			if (result == kEngineOkay)
			{
				TheEngine->Run();
				TheEngine->Terminate();
			}
			else
			{
				unsigned_int32 manager = GetResultManager(result);
				if ((manager == kManagerDisplay) || (manager == kManagerGraphics))
				{
					const char *message =	"The engine was unable to create a suitable graphics context.\r\n\r\n"
											"Please make sure that your graphics hardware meets the minimum requirements "
											"and that you have the latest display drivers installed.\r\n\r\n"
											"If you're running in windowed mode for the first time and you see this message, "
											"make sure your desktop color depth is set to 32-bit color.";
					
					MessageBoxA(nullptr, message, APPLICATION_NAME, MB_OK | MB_ICONINFORMATION);
				}
				else if (manager == kManagerSound)
				{
					const char *message =	"The engine was unable to initialize the sound system.\r\n\r\n"
											"Please make sure that you have the June 2010 (or later) version of DirectX installed.\r\n\r\n"
											"Also make sure that you have a sound driver installed.";
					
					MessageBoxA(nullptr, message, APPLICATION_NAME, MB_OK | MB_ICONINFORMATION);
				}
			}
			
			Engine::Delete();
			
			#if C4LEAK_DETECTION
			
				Engine::DumpMemory("Leaks.txt");
			
			#endif
			
			ReleaseMutex(mutex);
		}
		
		return (0);
	}

#elif C4MACOS

	#ifdef C4ENGINEMODULE
	
		void MacMain(int argc, const char **argv)
		{
			String<>	commandLine;
			
			for (machine a = 1; a < argc; a++)
			{
				commandLine += argv[a];
				commandLine += ' ';
			}
			
			Engine::New();
			 
			EngineResult result = TheEngine->Initialize(APPLICATION_NAME, commandLine);
			if (result == kEngineOkay) 
			{ 
				TheEngine->Run(); 
				TheEngine->Terminate();
			} 
			else if (GetResultManager(result) == kManagerDisplay)
			{
				SInt16		itemIndex;
				 
				const unsigned_int8 *message =	"\pThe engine was unable to create a suitable graphics context.\r"
												"Please make sure your graphics hardware meets the minimum requirements.";
				
				StandardAlert(kAlertNoteAlert, "\pC4 Engine could not be initialized.", message, nullptr, &itemIndex); 
			}
			
			Engine::Delete();
			
			#if C4LEAK_DETECTION
			
				Engine::DumpMemory("Leaks.txt");
			
			#endif
		}
	
	#else
	
		int main(int argc, const char **argv)
		{
			MacMain(argc, argv);
			return (0);
		}
	
	#endif

#elif C4LINUX

	int main(int argc, const char **argv)
	{
		String<>	commandLine;
		
		for (machine a = 1; a < argc; a++)
		{
			commandLine += argv[a];
			commandLine += ' ';
		}
		
		Engine::New();
		
		EngineResult result = TheEngine->Initialize(APPLICATION_NAME, commandLine);
		if (result == kEngineOkay)
		{
			TheEngine->Run();
			TheEngine->Terminate();
		}
		
		Engine::Delete();
		
		#if C4LEAK_DETECTION
		
			Engine::DumpMemory("Leaks.txt");
		
		#endif
		
		return (0);
	}

#elif C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

#endif //]

// ZYURVUR
