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


#ifndef C4Defines_h
#define C4Defines_h


#define C4VERSION					"2.9.1"

#define C4LEGACY					1
#define C4DIAGNOSTICS				0

#if defined(EXPORT_C4ENGINE)
#define C4_API __declspec(dllexport)
#else
#define C4_API __declspec(dllimport)
#endif



#if defined(C4SERVER)

	#undef C4SERVER
	
	#define C4SERVER				1

#else

	#define C4SERVER				0

#endif


#if defined(C4WINDOWS)

	#undef C4WINDOWS
	
	#define C4WINDOWS				1
	#define C4MACOS					0
	#define C4IOS					0
	#define C4LINUX					0
	#define C4PLAYSTATION3 			0
	
	#define C4POWERPC				0
	#define C4INTEL					1
	#define C4SIMD					0
	
	#define C4BIGENDIAN				0
	#define C4LITTLEENDIAN			1
	
	#define C4POSIX					0
	#define C4GAMECONSOLE			0
	#define C4XAUDIO				1
	#define C4CGSHADER				0


	#define C4OPENAL				0
	
	#define C4OPENGL				1
	#define C4XINPUT				0
	
	#define C4_ENGINE_CONFIG_FILE	"engine"
	#define C4_INPUT_CONFIG_FILE	"input"

#elif defined(C4MACOS)

	#undef C4MACOS
	
	#define C4WINDOWS				0
	#define C4MACOS					1
	#define C4IOS					0
	#define C4LINUX					0
	#define C4PLAYSTATION3 			0
	
	#define C4POWERPC				0
	#define C4INTEL					1
	#define C4SIMD					1
	
	#define C4BIGENDIAN				0
	#define C4LITTLEENDIAN			1
	
	#define C4POSIX					1
	#define C4GAMECONSOLE			0
	#define C4XAUDIO				0
	#define C4CGSHADER				0
	
	#define C4OPENGL				1
	#define C4XINPUT				0
	
	#define C4_ENGINE_CONFIG_FILE	"engine"
	#define C4_INPUT_CONFIG_FILE	"input"

#elif defined(C4LINUX)

	#undef C4LINUX
	
	#define C4WINDOWS				0
	#define C4MACOS					0
	#define C4IOS					0
	#define C4LINUX					1
	#define C4PLAYSTATION3 			0
	
	#define C4POWERPC				0
	#define C4INTEL					1
	#define C4SIMD					1
	
	#define C4BIGENDIAN				0
	#define C4LITTLEENDIAN			1
	
	#define C4POSIX					1
	#define C4GAMECONSOLE			0
	#define C4XAUDIO				0
	#define C4CGSHADER				0
	
	#define C4OPENGL				1 
	#define C4XINPUT				0
	 
	#define C4_ENGINE_CONFIG_FILE	"engine" 
	#define C4_INPUT_CONFIG_FILE	"input" 

#elif defined(C4PLAYSTATION3) 

	#undef C4PLAYSTATION3
	
	#define C4WINDOWS				0 
	#define C4MACOS					0
	#define C4IOS					0
	#define C4LINUX					0
	#define C4PLAYSTATION3 			1 
	
	#define C4POWERPC				1
	#define C4INTEL					0
	#define C4SIMD					1
	
	#define C4BIGENDIAN				1
	#define C4LITTLEENDIAN			0
	
	#define C4POSIX					0
	#define C4GAMECONSOLE			1
	#define C4XAUDIO				0
	#define C4CGSHADER				1
	
	#define C4OPENGL				0
	#define C4XINPUT				0
	
	#define C4_ENGINE_CONFIG_FILE	"engine_ps3"
	#define C4_INPUT_CONFIG_FILE	"input_ps3"

#else

	#error	One of C4WINDOWS, C4MACOS, C4LINUX, or C4PLAYSTATION3 must be defined.

#endif


#if defined(C4OPTIMIZED)

	#undef C4OPTIMIZED
	
	#define C4OPTIMIZED				1
	#define C4DEBUG					0

#elif defined(C4DEBUG)

	#undef C4DEBUG
	
	#define C4OPTIMIZED				0
	#define C4DEBUG					1

#else

	#error	Either C4OPTIMIZED or C4DEBUG must be defined.

#endif


#if defined(_MSC_VER)
	
	#define C4VISUALC				1
	#define C4GCC					0
	
#if defined(C4WININCLUDE)
//	#include "C4PrefixWindows.h"

#pragma warning(disable: 4201)		// nonstandard extension used : nameless struct/union
#define _WIN32_WINNT			0x0502
#define DIRECTINPUT_VERSION		0x0800

#undef UNICODE

#define WIN32_LEAN_AND_MEAN
#define NOGDICAPMASKS
//#define NOMENUS
//#define NOICONS
#define NORASTEROPS
#define OEMRESOURCE
#define NOATOM
#define NOCOLOR
#define NOMETAFILE
#define NOMINMAX
#define NOSCROLL
#define NOSERVICE
#define NOSOUND
#define NOWH
#define NOCOMM
#define NOKANJI
#define NOHELP
#define NOPROFILER
#define NODEFERWINDOWPOS
#define NOMCX
#include <windows.h>

#include <windows.h>
#include <winuser.h>
#include <winsock2.h>
#include <mmsystem.h>
#include <dinput.h>
#include <dsound.h>
#include <xinput.h>
#include <xaudio2.h>
#include <shlobj.h>
#include <wbemcli.h>
#include <oleauto.h>
#include <shellapi.h>
#include <math.h>
#include <intrin.h>
#include <gl/gl.h>

#undef CopyMemory
#undef FillMemory
#undef DeleteFile
#undef CreateDirectory
#undef GetObject
#undef AppendMenu
#undef SendMessage
#undef GetCurrentTime
#undef Yield

#ifdef _WIN64

typedef unsigned __int64 size_t;
typedef __int64 ptrdiff_t;

#else

typedef unsigned int size_t;
typedef int ptrdiff_t;

#endif

namespace std
{
	using ::ptrdiff_t;
	using ::size_t;
}

void *__cdecl operator new(size_t);
void *__cdecl operator new[](size_t);
void __cdecl operator delete(void *);
void __cdecl operator delete[](void *);

inline void *__cdecl operator new(size_t, void *ptr)
{
	return (ptr);
}

inline void *__cdecl operator new[](size_t, void *ptr)
{
	return (ptr);
}

inline void __cdecl operator delete(void *, void *)
{
}

inline void __cdecl operator delete[](void *, void *)
{
}

#endif

	
	#if defined(_WIN64)
	
		#define C4PTR64				1
	
	#else
	
		#define C4PTR64				0
	
	#endif
	
	#if (_MSC_VER >= 1600)
	
		#define C4RVALUEREF			1
	
	#else
	
		#define C4RVALUEREF			0
		
		#define nullptr 0
	
	#endif
	
	typedef signed char				int8;
	typedef unsigned char			unsigned_int8;
	
	typedef short					int16;
	typedef unsigned short			unsigned_int16;
	
	typedef int						int32;
	typedef unsigned int			unsigned_int32;
	
	typedef __int64					int64;
	typedef unsigned __int64		unsigned_int64;
	
	typedef long					machine;
	typedef unsigned long			unsigned_machine;
	
	typedef long					machine_int;
	typedef unsigned long			unsigned_machine_int;
	
	#define asm _asm
	
	#define align_address(n) __declspec(align(n))
	
	#define __attribute__(x)
	#define restrict __restrict
	
	#pragma pointers_to_members(full_generality, multiple_inheritance)
	
	#pragma warning(3: 4706)			// assignment within conditional expression
	#pragma warning(disable: 4100)		// unreferenced formal parameter
	#pragma warning(disable: 4244)		// conversion, possible loss of data
	#pragma warning(disable: 4245)		// conversion, signed/unsigned mismatch
	#pragma warning(disable: 4310)		// cast truncates constant value
	#pragma warning(disable: 4355)		// this used in base member initializer list
	#pragma warning(disable: 4389)		// '==' : signed/unsigned mismatch
	#pragma warning(disable: 4481)		// nonstandard extension used : override
	#pragma warning(disable: 4505)		// unreferenced local function has been removed
	#pragma warning(disable: 4701)		// potentially uninitialized local variable used
	#pragma warning(disable: 4800)		// forcing value to bool 'true' or 'false'
	#pragma warning(disable: 4804)		// unsafe use of type 'bool' in operation
	#pragma warning(disable: 4805)		// unsafe mix of type 'unsigned long' and type 'bool' in operation

#elif defined(__GNUC__)

	#define C4VISUALC				0
	#define C4GCC					1
	
	#if C4MACOS
	
		#include "C4PrefixMacOS.h"
	
	#elif C4LINUX
	
		#include "C4PrefixLinux.h"
	
	#elif C4PLAYSTATION3
	
		#include "C4PrefixPS3.h"
	
	#endif
	
	#if defined(__LP64__)
	
		#define C4PTR64				1
	
	#else
	
		#define C4PTR64				0
	
	#endif
	
	#define GCCVER ((__GNUC__ << 8) | __GNUC_MINOR__)
	
	#if (GCCVER >= 0x0403)
	
		#define C4RVALUEREF			1
	
	#else
	
		#define C4RVALUEREF			0
	
	#endif
	
	#if (GCCVER < 0x0406)
	
		#define nullptr 0
	
	#endif
	
	#if (GCCVER < 0x0407)
	
		#define override
	
	#endif
	
	typedef signed char				int8;
	typedef unsigned char			unsigned_int8;
	
	typedef short					int16;
	typedef unsigned short			unsigned_int16;
	
	typedef int						int32;
	typedef unsigned int			unsigned_int32;
	
	typedef long long				int64;
	typedef unsigned long long		unsigned_int64;
	
	#if C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

	#else //]
	
		typedef long				machine;
		typedef unsigned long		unsigned_machine;
		
		typedef long				machine_int;
		typedef unsigned long		unsigned_machine_int;
	
	#endif
	
	#define align_address(n) __attribute__((aligned(n)))
	
	#define restrict __restrict__
	#define __cdecl

#else

	#error	The only supported compilers are MSVC and GCC.

#endif


#if C4PTR64

	typedef unsigned_int64			machine_address;

#else

	typedef unsigned_int32			machine_address;

#endif


#if !C4GCC

	#ifdef C4ENGINEMODULE

		#define C4API __declspec(dllexport)

	#else

		#define C4API __declspec(dllimport)

	#endif
	
	#define C4MODULEEXPORT __declspec(dllexport)
	#define C4MODULEIMPORT __declspec(dllimport)

#else

	#ifdef C4ENGINEMODULE
		
		#define C4API __attribute__((visibility("default")))
	
	#else
	
		#define C4API
	
	#endif
	
	#define C4MODULEEXPORT __attribute__((visibility("default")))
	#define C4MODULEIMPORT

#endif


#endif

// ZYURVUR
