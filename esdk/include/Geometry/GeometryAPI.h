#ifndef __GEOMETRY_API_HEADER_INLCUDED__
#define __GEOMETRY_API_HEADER_INLCUDED__

#pragma once

#ifdef UNE_GEOMETRY_EXPORTS    
#	ifdef DOTNET
#		define GEOMETRY_DECLARE_EXPORT_CLASS(ClassName)		public ref class ClassName
#		define GEOMETRY_API
#	else
#		define GEOMETRY_DECLARE_EXPORT_CLASS(ClassName)		class __declspec(dllexport) ClassName
#		define GEOMETRY_API __declspec(dllexport)
#	endif	
#else
#	ifdef DOTNET
#		define GEOMETRY_DECLARE_EXPORT_CLASS(ClassName)		public ref class ClassName
#		define GEOMETRY_API
#	else		
#		define GEOMETRY_DECLARE_EXPORT_CLASS(ClassName)		class __declspec(dllimport) ClassName
#		define GEOMETRY_API __declspec(dllimport)
#	endif
#endif


//////////////////////////////////////////////////////////////////////////
// SET ENDIAN

#ifndef UNE_ENDIAN_BIG
#	define UNE_ENDIAN_BIG 1
#endif

#ifndef UNE_ENDIAN_LITTLE
#	define UNE_ENDIAN_LITTLE 2
#endif

#ifdef UNE_ENDIAN
#	undef UNE_ENDIAN
#endif
#define UNE_ENDIAN UNE_ENDIAN_LITTLE

//////////////////////////////////////////////////////////////////////////
// SET COMPILER & VERSION
#ifndef UNE_COMPILER_MSVC
#	define UNE_COMPILER_MSVC 1
#endif

#ifndef UNE_COMPILER_DOTNET
#	define UNE_COMPILER_DOTNET 2
#endif

#if defined( _MSC_VER )
#	ifdef UNE_COMPILER
#		undef UNE_COMPILER
#	endif

#	ifdef DOTNET	
#		define UNE_COMPILER UNE_COMPILER_DOTNET
#	else
#		define UNE_COMPILER UNE_COMPILER_MSVC
#	endif

#	ifdef UNE_COMP_VER
#		undef UNE_COMP_VER
#	endif
#	define UNE_COMP_VER _MSC_VER
#endif


//////////////////////////////////////////////////////////////////////////
// SET PLATFORM
#ifndef UNE_PLATFORM_WIN32 
#	define UNE_PLATFORM_WIN32  1
#endif

#if defined( __WIN32__ ) || defined( _WIN32 )
#	ifdef UNE_PLATFORM
#		undef UNE_PLATFORM
#	endif
#   define UNE_PLATFORM UNE_PLATFORM_WIN32
#endif

//////////////////////////////////////////////////////////////////////////
// SET ARCHITECHER

#ifndef UNE_ARCHITECTURE_32
#	define UNE_ARCHITECTURE_32 1
#endif

#ifndef UNE_ARCHITECTURE_64
#	define UNE_ARCHITECTURE_64 2
#endif

#ifdef UNE_ARCH_TYPE
#	undef UNE_ARCH_TYPE
#endif

#if defined(__x86_64__) || defined(_M_X64) || defined(__powerpc64__) || defined(__alpha__) || defined(__ia64__) || defined(__s390__) || defined(__s390x__)
#   define UNE_ARCH_TYPE UNE_ARCHITECTURE_64
#else
#   define UNE_ARCH_TYPE UNE_ARCHITECTURE_32
#endif


//////////////////////////////////////////////////////////////////////////
// set basic type
typedef unsigned int uint32;
typedef unsigned short uint16;
typedef unsigned char uint8;
typedef int int32;
typedef short int16;
typedef char int8;

typedef unsigned __int64 uint64;
typedef __int64 int64;

#include "UnEUtility/Common.h"

namespace UnE 
{
	namespace Geometry 
	{
		typedef float Real;
	}
}

//////////////////////////////////////////////////////////////////////////
// EXPORT
#define GEOMETRY_EXPORT_CLASS(ClassName)		DECLARE_CLASS(ClassName)	\
	GEOMETRY_DECLARE_EXPORT_CLASS(ClassName)

#endif // UMath_h__
