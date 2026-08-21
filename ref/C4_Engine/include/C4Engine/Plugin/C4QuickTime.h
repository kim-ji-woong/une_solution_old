//=============================================================
//
// C4 Engine version 2.9.1
// Copyright 1999-2012, by Terathon Software LLC
//
// This file is part of the C4 Engine and is provided under the
// terms of the license agreement entered by the registed user.
//
// Unauthorized redistribution of source code is strictly
// prohibited. Violators will be prosecuted.
//
//=============================================================


#ifndef C4QuickTime_h
#define C4QuickTime_h


#if C4MACOS

	#define QT
	#define QTCallback

#else

	#define NewMovieDrawingCompleteUPP(name) name
	#define DisposeMovieDrawingCompleteUPP(name)
	
	#define NewMoviePrePrerollCompleteUPP(name) name
	#define DisposeMoviePrePrerollCompleteUPP(name)
	
	
	#define QTCallback __cdecl
	
	
	namespace QT
	{
		struct CGrafPort;
		struct GDevice;
		struct ColorTable;
		struct MovieType;
		struct TrackType;
		struct MediaType;
		struct __CFAllocator;
		struct __CFString;
		
		typedef char						*Ptr;
		typedef Ptr							*Handle;
		typedef long						Size;
		typedef long						Fixed;
		typedef short						OSErr;
		typedef unsigned long				OSType;
		typedef unsigned char				Boolean;
		typedef unsigned char				Str255[256];
		typedef unsigned char				*StringPtr;
		typedef struct CGrafPort			*CGrafPtr;
		typedef CGrafPtr					GWorldPtr;
		typedef unsigned long				GWorldFlags;
		typedef struct GDevice				*GDPtr;
		typedef GDPtr						*GDHandle;
		typedef struct ColorTable			*CTabPtr;
		typedef CTabPtr						*CTabHandle;
		typedef long						TimeValue;
		typedef long						TimeScale;
		typedef struct MovieType			**Movie;
		typedef struct TrackType			**Track;
		typedef struct MediaType			**Media;
		typedef unsigned long				QTPathStyle;
		typedef unsigned long				CFStringEncoding;
		typedef const void					*CFTypeRef;
		typedef const struct __CFAllocator	*CFAllocatorRef;
		typedef const struct __CFString		*CFStringRef;
		
		typedef OSErr (QTCallback *MovieDrawingCompleteUPP)(Movie, long);
		typedef void (QTCallback *MoviePrePrerollCompleteUPP)(Movie, OSErr, void *);
		
		
		enum
		{
			noErr							= 0,
			newMovieActive					= 1 << 0,
			movieDrawingCallWhenChanged		= 0,
			URLDataHandlerSubType			= 'url ',
			SoundMediaType					= 'soun',
			MusicMediaType					= 'musi',
			k32RGBAPixelFormat				= 'RGBA',
			kQTWindowsPathStyle				= 2,
			kCFStringEncodingUTF8			= 0x08000100
		};
		
		
		struct TimeRecord;
		
		
		struct Rect
		{
			short		top;
			short		left;
			short		bottom;
			short		right;
		};
	}
	
	
	#define kCFAllocatorDefault (*((const QT::CFAllocatorRef *) QTGetCFConstant("kCFAllocatorDefault")))
	#define kCFAllocatorNull (*((const QT::CFAllocatorRef *) QTGetCFConstant("kCFAllocatorNull")))
	
	
	extern "C"
	{
		QT::OSErr __cdecl InitializeQTML(long);
		void __cdecl TerminateQTML(void);
		QT::OSErr __cdecl EnterMovies(void);
		void __cdecl ExitMovies(void);
		
		void __cdecl MoviesTask(QT::Movie, long); 
		
		QT::OSErr __cdecl QTNewDataReferenceFromFullPathCFString(QT::CFStringRef, QT::QTPathStyle, unsigned long, QT::Handle *, QT::OSType *); 
		QT::OSErr __cdecl NewMovieFromDataRef(QT::Movie *, short, short *, QT::Handle, QT::OSType); 
		void __cdecl DisposeMovie(QT::Movie); 
		
		void __cdecl StartMovie(QT::Movie); 
		void __cdecl StopMovie(QT::Movie);
		QT::Boolean __cdecl IsMovieDone(QT::Movie);
		
		QT::OSErr __cdecl PrePrerollMovie(QT::Movie, QT::TimeValue, QT::Fixed, QT::MoviePrePrerollCompleteUPP, void *); 
		void __cdecl AbortPrePrerollMovie(QT::Movie, QT::OSErr);
		
		void __cdecl GoToBeginningOfMovie(QT::Movie);
		 
		void __cdecl GetMovieBox(QT::Movie, QT::Rect *);
		void __cdecl SetMovieBox(QT::Movie, const QT::Rect *);
		
		short __cdecl GetMoviePreferredVolume(QT::Movie);
		QT::Fixed __cdecl GetMoviePreferredRate(QT::Movie);
		
		void __cdecl SetMovieVolume(QT::Movie, short);
		void __cdecl SetMovieDrawingCompleteProc(QT::Movie, long, QT::MovieDrawingCompleteUPP, long);
		
		QT::TimeValue __cdecl GetMovieDuration(QT::Movie);
		QT::TimeScale __cdecl GetMovieTimeScale(QT::Movie);
		QT::TimeValue __cdecl GetMovieTime(QT::Movie, QT::TimeRecord *);
		void __cdecl SetMovieTimeValue(QT::Movie, QT::TimeValue);
		
		long __cdecl GetMovieTrackCount(QT::Movie);
		void __cdecl SetTrackEnabled(QT::Track, QT::Boolean);
		QT::Track __cdecl GetMovieIndTrack(QT::Movie, long);
		QT::Media __cdecl GetTrackMedia(QT::Track);
		void __cdecl GetMediaHandlerDescription(QT::Media, QT::OSType *, QT::Str255, QT::OSType *);
		
		void __cdecl SetMovieGWorld(QT::Movie, QT::CGrafPtr, QT::GDHandle);
		QT::OSErr __cdecl QTNewGWorldFromPtr(QT::GWorldPtr *, QT::OSType, const QT::Rect *, QT::CTabHandle, QT::GDHandle, QT::GWorldFlags, void *, long);
		void __cdecl DisposeGWorld(QT::GWorldPtr);
		
		QT::Handle __cdecl NewHandle(QT::Size);
		void __cdecl DisposeHandle(QT::Handle);
		
		void *__cdecl QTGetCFConstant(const char *);
		QT::CFStringRef __cdecl CFStringCreateWithCStringNoCopy(QT::CFAllocatorRef, const char *, QT::CFStringEncoding, QT::CFAllocatorRef);
		void __cdecl CFRelease(QT::CFTypeRef cf);
	}

#endif


#endif

// ZYURVUR
