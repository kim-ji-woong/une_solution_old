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

#include "C4Application.h"
#include "C4AudioCapture.h"
#include "C4World.h"
#include "C4Plugins.h"
#include "C4ToolWindows.h"


using namespace C4;


//#if C4GAMECONSOLE

	extern "C"
	{
		C4::Application *ConstructApplication(void);
	}

//#endif


Engine *C4::TheEngine = nullptr;


namespace C4
{
	template <> Engine Manager<Engine>::managerObject(0);
	template <> Engine **Manager<Engine>::managerPointer = &TheEngine;
}


ResourceDescriptor ConfigResource::descriptor("cfg", kResourceExtraByte);


List<Reporter> Engine::reporterList;
List<Persistor> Engine::persistorList;

unsigned_int32 Engine::mouseButtonMask = 0;
Point3D Engine::lastMousePosition;


ConfigResource::ConfigResource(const char *name, ResourceCatalog *catalog) : Resource<ConfigResource>(name, catalog)
{
}

ConfigResource::~ConfigResource()
{
}

void ConfigResource::Preprocess(void)
{
	// Put a terminator byte at the end of the text.
	// This does not write past the end of the memory allocated because the kResourceExtraByte
	// flag is set in the resource descriptor, telling the Resource Manager to allocate an extra byte.
	
	static_cast<char *>(GetData())[GetSize()] = 0;
}


Reporter::Reporter(ReportProc *proc, void *cookie)
{
	reportProc = proc;
	reportCookie = cookie;
}


Persistor::Persistor(WriteProc *proc, void *cookie)
{
	persistProc = proc;
	persistCookie = cookie;
}


MouseEventHandler::MouseEventHandler(HandlerProc *proc, void *cookie)
{
	handlerProc = proc;
	handlerCookie = cookie;
}


KeyboardEventHandler::KeyboardEventHandler(HandlerProc *proc, void *cookie)
{
	handlerProc = proc;
	handlerCookie = cookie;
}


Engine::Engine(int) :
		logger(&Logger, this),
		
		#if C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

		#endif //]
		
		gameModuleObserver(this, &Engine::HandleGameModuleEvent),
		quitCommandObserver(this, &Engine::HandleQuitCommand),
		wireCommandObserver(this, &Engine::HandleWireCommand),
		normCommandObserver(this, &Engine::HandleNormCommand),
		tangCommandObserver(this, &Engine::HandleTangCommand),
		shadCommandObserver(this, &Engine::HandleShadCommand),
		sbndCommandObserver(this, &Engine::HandleSbndCommand), 
		lrgnCommandObserver(this, &Engine::HandleLrgnCommand),
		srgnCommandObserver(this, &Engine::HandleSrgnCommand), 
		doffCommandObserver(this, &Engine::HandleDoffCommand), 
		spthCommandObserver(this, &Engine::HandleSpthCommand), 
		bodyCommandObserver(this, &Engine::HandleBodyCommand),
		ctacCommandObserver(this, &Engine::HandleCtacCommand), 
		rateCommandObserver(this, &Engine::HandleRateCommand),
		statCommandObserver(this, &Engine::HandleStatCommand),
		smapCommandObserver(this, &Engine::HandleSmapCommand),
		netCommandObserver(this, &Engine::HandleNetCommand), 
		extCommandObserver(this, &Engine::HandleExtCommand),
		rsrcCommandObserver(this, &Engine::HandleRsrcCommand),
		heapCommandObserver(this, &Engine::HandleHeapCommand),
		dumpCommandObserver(this, &Engine::HandleDumpCommand), 
		visitCommandObserver(this, &Engine::HandleVisitCommand),
		shotCommandObserver(this, &Engine::HandleShotCommand),
		undefCommandObserver(this, &Engine::HandleUndefCommand),
		bindCommandObserver(this, &Engine::HandleBindCommand),
		unbindCommandObserver(this, &Engine::HandleUnbindCommand),
		sayCommandObserver(this, &Engine::HandleSayCommand),
		addressCommandObserver(this, &Engine::HandleAddressCommand),
		resolveCommandObserver(this, &Engine::HandleResolveCommand),
		disconnectCommandObserver(this, &Engine::HandleDisconnectCommand),
		execCommandObserver(this, &Engine::HandleExecCommand),
		importCommandObserver(this, &Engine::HandleImportCommand),
		cmdCommandObserver(this, &Engine::HandleCmdCommand),
		varCommandObserver(this, &Engine::HandleVarCommand),
		loadCommandObserver(this, &Engine::HandleLoadCommand),
		unloadCommandObserver(this, &Engine::HandleUnloadCommand)
{
}

Engine::~Engine()
{
}

EngineResult Engine::Construct(void)
{
	engineFlags = kEngineForeground | kEngineVisible;
	
	#if !C4GAMECONSOLE
	
		backgroundSleepTime = 0;
	
	#endif
	
	#if C4WINDOWS
	
		deadKeyFlag = false;
		wheelDeltaAccum = 0;
	
	#endif
	
	InitializeProcessorData();
	InstallReporter(&logger);
	
	#if C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

	#endif //]
	
	AddCommand(new Command("quit", &quitCommandObserver));
	AddCommand(new Command("wire", &wireCommandObserver));
	AddCommand(new Command("norm", &normCommandObserver));
	AddCommand(new Command("tang", &tangCommandObserver));
	AddCommand(new Command("shad", &shadCommandObserver));
	AddCommand(new Command("sbnd", &sbndCommandObserver));
	AddCommand(new Command("lrgn", &lrgnCommandObserver));
	AddCommand(new Command("srgn", &srgnCommandObserver));
	AddCommand(new Command("doff", &doffCommandObserver));
	AddCommand(new Command("spth", &spthCommandObserver));
	AddCommand(new Command("body", &bodyCommandObserver));
	AddCommand(new Command("ctac", &ctacCommandObserver));
	AddCommand(new Command("rate", &rateCommandObserver));
	AddCommand(new Command("stat", &statCommandObserver));
	AddCommand(new Command("smap", &smapCommandObserver));
	AddCommand(new Command("net", &netCommandObserver));
	AddCommand(new Command("ext", &extCommandObserver));
	AddCommand(new Command("rsrc", &rsrcCommandObserver));
	AddCommand(new Command("heap", &heapCommandObserver));
	AddCommand(new Command("dump", &dumpCommandObserver));
	AddCommand(new Command("visit", &visitCommandObserver));
	AddCommand(new Command("shot", &shotCommandObserver));
	AddCommand(new Command("undef", &undefCommandObserver));
	AddCommand(new Command("bind", &bindCommandObserver));
	AddCommand(new Command("unbind", &unbindCommandObserver));
	AddCommand(new Command("say", &sayCommandObserver));
	AddCommand(new Command("address", &addressCommandObserver));
	AddCommand(new Command("resolve", &resolveCommandObserver));
	AddCommand(new Command("disconnect", &disconnectCommandObserver));
	AddCommand(new Command("exec", &execCommandObserver));
	AddCommand(new Command("import", &importCommandObserver));
	AddCommand(new Command("cmd", &cmdCommandObserver));
	AddCommand(new Command("var", &varCommandObserver));
	AddCommand(new Command("load", &loadCommandObserver));
	AddCommand(new Command("unload", &unloadCommandObserver));
	
	return (kEngineOkay);
}

void Engine::Destruct(void)
{
	mouseEventHandlerList.RemoveAll();
	keyboardEventHandlerList.RemoveAll();
	
	reporterList.RemoveAll();
	persistorList.RemoveAll();
	
	variableMap.Purge();
	commandMap.Purge();
}


EngineResult Engine::InitializeSDI(const char *name, HINSTANCE instance, HWND hWnd, const char *commandLine)
{
	applicationName = name;

	multiaxisMouseActiveCount = 0;
	tabletActiveCount = 0;
	stylusPressure = 0.0F;

	wchar_t		wideName[256];

	engineInstance = instance;
	engineWindow = hWnd;

	{
		GetClassInfoExW(instance, L"C4ClientUNE", &windowClass);
	}
	//windowClass.cbSize = sizeof(WNDCLASSEXW);
	//windowClass.style = CS_OWNDC;
	//windowClass.lpfnWndProc = &WindowProc;
	//windowClass.cbClsExtra = 0;
	//windowClass.cbWndExtra = 0;
	//windowClass.hInstance = engineInstance;
	//windowClass.hIcon = LoadIconA(engineInstance, MAKEINTRESOURCE(1));
	//windowClass.hCursor = LoadCursorA(nullptr, IDC_ARROW);
	//windowClass.hbrBackground = nullptr;
	//windowClass.lpszMenuName = nullptr;
	//windowClass.lpszClassName = L"C4";
	//windowClass.hIconSm = nullptr;
	//RegisterClassExW(&windowClass);

	int32 len = Min(Text::GetTextLength(name), 255);
	for (machine a = 0; a < len; a++) wideName[a] = name[a];
	wideName[len] = 0;

	//engineWindow = CreateWindowExW(0, L"C4", wideName, WS_POPUP | WS_CLIPCHILDREN, 0, 0, 640, 480, nullptr, nullptr, engineInstance, nullptr);

	SetFocus(engineWindow);
	//SetCursor(windowClass.hCursor);

	


	EngineResult result = ConstructManagers(commandLine);
	if (result != kEngineOkay)
	{
		Report("<br/><br/>", kReportLog);
		LogResult(result);

		//Terminate();
		return (result);
	}
	
	result = LoadApplicationModule();
	if (result != kEngineOkay)
	{
	//Terminate();
		return (result);
	}
	
	return (kEngineOkay);
}

#if C4WINDOWS

	EngineResult Engine::Initialize(const char *name, HINSTANCE instance, const char *commandLine)

#else

	EngineResult Engine::Initialize(const char *name, const char *commandLine)

#endif

{
	applicationName = name;
	
	multiaxisMouseActiveCount = 0;
	tabletActiveCount = 0;
	stylusPressure = 0.0F;
	
	#if C4WINDOWS
	
		wchar_t		wideName[256];
		
		engineInstance = instance;
		
		windowClass.cbSize = sizeof(WNDCLASSEXW);
		windowClass.style = CS_OWNDC;
		windowClass.lpfnWndProc = &WindowProc;
		windowClass.cbClsExtra = 0;
		windowClass.cbWndExtra = 0;
		windowClass.hInstance = engineInstance;
		windowClass.hIcon = LoadIconA(engineInstance, MAKEINTRESOURCE(1));
		windowClass.hCursor = LoadCursorA(nullptr, IDC_ARROW);
		windowClass.hbrBackground = nullptr;
		windowClass.lpszMenuName = nullptr;
		windowClass.lpszClassName = L"C4";
		windowClass.hIconSm = nullptr;
		RegisterClassExW(&windowClass);
		
		int32 len = Min(Text::GetTextLength(name), 255);
		for (machine a = 0; a < len; a++) wideName[a] = name[a];
		wideName[len] = 0;
		
		engineWindow = CreateWindowExW(0, L"C4", wideName, WS_POPUP | WS_CLIPCHILDREN, 0, 0, 640, 480, nullptr, nullptr, engineInstance, nullptr);
		
		SetFocus(engineWindow);
		//SetCursor(windowClass.hCursor);
		
		//CoInitializeEx(nullptr, COINIT_APARTMENTTHREADED | COINIT_SPEED_OVER_MEMORY);//COINIT_MULTITHREADED | COINIT_SPEED_OVER_MEMORY);
	
	#elif C4MACOS
	
		EventTypeSpec	eventList[5];
		
		dispatcherTarget = GetEventDispatcherTarget();
		
		EventTargetRef eventTarget = GetApplicationEventTarget();
		
		eventList[0].eventClass = ::kEventClassApplication;
		eventList[0].eventKind = ::kEventAppActivated;
		eventList[1].eventClass = ::kEventClassApplication;
		eventList[1].eventKind = ::kEventAppDeactivated;
		eventList[2].eventClass = ::kEventClassVolume;
		eventList[2].eventKind = ::kEventVolumeMounted;
		eventList[3].eventClass = ::kEventClassVolume;
		eventList[3].eventKind = ::kEventVolumeUnmounted;
		
		applicationEventHandlerUPP = NewEventHandlerUPP(&HandleApplicationEvent);
		InstallEventHandler(eventTarget, applicationEventHandlerUPP, 4, eventList, this, &applicationEventHandlerRef);
		
		eventList[0].eventClass = ::kEventClassMouse;
		eventList[0].eventKind = ::kEventMouseDown;
		eventList[1].eventClass = ::kEventClassMouse;
		eventList[1].eventKind = ::kEventMouseUp;
		eventList[2].eventClass = ::kEventClassMouse;
		eventList[2].eventKind = ::kEventMouseMoved;
		eventList[3].eventClass = ::kEventClassMouse;
		eventList[3].eventKind = ::kEventMouseDragged;
		eventList[4].eventClass = ::kEventClassMouse;
		eventList[4].eventKind = ::kEventMouseWheelMoved;
		
		mouseEventHandlerUPP = NewEventHandlerUPP(&HandleMouseEvent);
		InstallEventHandler(eventTarget, mouseEventHandlerUPP, 5, eventList, this, &mouseEventHandlerRef);
		
		eventList[0].eventClass = ::kEventClassKeyboard;
		eventList[0].eventKind = ::kEventRawKeyDown;
		eventList[1].eventClass = ::kEventClassKeyboard;
		eventList[1].eventKind = ::kEventRawKeyUp;
		eventList[2].eventClass = ::kEventClassKeyboard;
		eventList[2].eventKind = ::kEventRawKeyRepeat;
		
		keyboardEventHandlerUPP = NewEventHandlerUPP(&HandleKeyboardEvent);
		InstallEventHandler(eventTarget, keyboardEventHandlerUPP, 3, eventList, this, &keyboardEventHandlerRef);
	
	#elif C4LINUX
	
		XSetWindowAttributes	attributes;
		
		engineDisplay = XOpenDisplay(nullptr);
		
		attributes.event_mask = ButtonPressMask | ButtonReleaseMask | PointerMotionMask | KeyPressMask | KeyReleaseMask;
		engineWindow = XCreateWindow(engineDisplay, DefaultRootWindow(engineDisplay), 0, 0, 640, 480, 0, CopyFromParent, InputOutput, CopyFromParent, CWEventMask, &attributes);
		XStoreName(engineDisplay, engineWindow, name);
		
		deleteWindowAtom = XInternAtom(engineDisplay, "WM_DELETE_WINDOW", false);
		XSetWMProtocols(engineDisplay, engineWindow, &deleteWindowAtom, 1);
	
	#elif C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

	#endif //]
	
	EngineResult result = ConstructManagers(commandLine);
	if (result != kEngineOkay)
	{
		Report("<br/><br/>", kReportLog);
		LogResult(result);
		
		Terminate();
		return (result);
	}
	
	result = LoadApplicationModule();
	if (result != kEngineOkay)
	{
		Terminate();
		return (result);
	}
	
	return (kEngineOkay);
}

void Engine::Terminate(void)
{
	delete TheApplication;
	if (ThePluginMgr)
		ThePluginMgr->PurgePlugins();
	
	UnloadApplicationModule();
	DestroyManagers();
	
	#if C4WINDOWS
	
		CoUninitialize();
		
		//DestroyWindow(engineWindow);
		//UnregisterClassW(L"C4", engineInstance);
	
	#elif C4MACOS
	
		RemoveEventHandler(keyboardEventHandlerRef);
		DisposeEventHandlerUPP(keyboardEventHandlerUPP);
		
		RemoveEventHandler(mouseEventHandlerRef);
		DisposeEventHandlerUPP(mouseEventHandlerUPP);
		
		RemoveEventHandler(applicationEventHandlerRef);
		DisposeEventHandlerUPP(applicationEventHandlerUPP);
	
	#elif C4LINUX
	
		XDestroyWindow(engineDisplay, engineWindow);
		XCloseDisplay(engineDisplay);
	
	#elif C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

	#endif //]
}

void Engine::InitializeProcessorData(void)
{
	processorFlags = 0;
	
	#if C4WINDOWS
	
		SYSTEM_INFO		systemInfo;
		
		GetSystemInfo(&systemInfo);
		processorCount = systemInfo.dwNumberOfProcessors;
		
		if (IsProcessorFeaturePresent(PF_XMMI_INSTRUCTIONS_AVAILABLE))
		{
			processorFlags |= kProcessorSSE;
			if (IsProcessorFeaturePresent(PF_XMMI64_INSTRUCTIONS_AVAILABLE))
			{
				processorFlags |= kProcessorSSE2;
				if (IsProcessorFeaturePresent(PF_SSE3_INSTRUCTIONS_AVAILABLE)) processorFlags |= kProcessorSSE3;
			}
		}
		
		if (IsProcessorFeaturePresent(PF_3DNOW_INSTRUCTIONS_AVAILABLE)) processorFlags |= kProcessor3DNow;
	
	#elif C4MACOS
	
		size_t		size;
		int			name[2];
		SInt32		result;
		
		size = 4;
		name[0] = CTL_HW;
		name[1] = HW_NCPU;
		if (sysctl(name, 2, &processorCount, &size, nullptr, 0) != 0) processorCount = 1;
		
		Gestalt(gestaltX86Features, &result);
		if (result & (1 << gestaltX86HasSSE))
		{
			processorFlags |= kProcessorSSE;
			if (result & (1 << gestaltX86HasSSE2))
			{
				processorFlags |= kProcessorSSE2;
				
				Gestalt(gestaltX86AdditionalFeatures, &result);
				if (result & (1 << gestaltX86HasSSE3)) processorFlags |= kProcessorSSE3;
			}
		}
	
	#elif C4LINUX
	
		int		eax, ebx, ecx, edx;
		
		processorCount = Max(sysconf(_SC_NPROCESSORS_ONLN), 1);
		
		__cpuid(1, eax, ebx, ecx, edx);
		if (edx & bit_SSE)
		{
			processorFlags |= kProcessorSSE;
			if (edx & bit_SSE2)
			{
				processorFlags |= kProcessorSSE2;
				if (ecx & bit_SSE3) processorFlags |= kProcessorSSE3;
			}
		}
	
	#elif C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

	#endif //]
}

EngineResult Engine::ConstructManagers(const char *commandLine)
{
	FileMgr::New();
	TimeMgr::New();
	
	#if C4MACOS
	
		FSRef	folderRef;
		
		openglBundle = nullptr;
		
		if (FSFindFolder(kSystemDomain, kFrameworksFolderType, kDontCreateFolder, &folderRef) == noErr)
		{
			CFURLRef folderURL = CFURLCreateFromFSRef(kCFAllocatorDefault, &folderRef);
			if (folderURL)
			{
				CFURLRef frameworkURL = CFURLCreateWithFileSystemPathRelativeToBase(kCFAllocatorDefault, CFSTR("OpenGL.framework"), kCFURLPOSIXPathStyle, false, folderURL);
				if (frameworkURL)
				{
					openglBundle = CFBundleCreate(kCFAllocatorDefault, frameworkURL);
					CFRelease(frameworkURL);
				}
				
				CFRelease(folderURL);
			}
		}
	
	#endif
	
	ResourceMgr::New();
	
	BeginLog();
	
	if (!ExecuteFile(C4_ENGINE_CONFIG_FILE, TheResourceMgr->GetConfigCatalog()))
		ExecuteFile(C4_ENGINE_CONFIG_FILE);
		
	ExecuteText(commandLine);
	
	EngineResult result = DisplayMgr::New();
	if (result != kEngineOkay)
		return (result);
	
	result = SoundMgr::New();
	//if (result != kEngineOkay)
	//	return (result);
	
	AudioCaptureMgr::New();
	
	result = InputMgr::New();
	if (result != kEngineOkay)
		return (result);
	
	InterfaceMgr::New();
	NetworkMgr::New();
	MessageMgr::New();
	JobMgr::New();
	WorldMgr::New();
	PluginMgr::New();
	ConsoleWindow::New();
	
	return (kEngineOkay);
}

void Engine::DestroyManagers(void)
{
	delete TheConsoleWindow;
	
	PluginMgr::Delete();
	WorldMgr::Delete();
	JobMgr::Delete();
	MessageMgr::Delete();
	NetworkMgr::Delete();
	InterfaceMgr::Delete();
	InputMgr::Delete();
	AudioCaptureMgr::Delete();
	SoundMgr::Delete();
	DisplayMgr::Delete();
	ResourceMgr::Delete();
	
	EndLog();
	
	TimeMgr::Delete();
	
	#if !C4MACOS || !C4LEAK_DETECTION
	
		FileMgr::Delete();
	
	#endif
}

void Engine::BeginLog(void)
{
	String<127>		date;
	String<127>		time;
	
	ResourcePath path(TheResourceMgr->GetSystemCatalog()->GetRootPath());
	logFile.Open(path += "UnELog.html", kFileCreate);
	
	Report( "<html>\r\n"
			"<head>\r\n<title>UnE Log File</title>\r\n"
				"<style type=\"text/css\">\r\n"
					"body {background-color: #F0F0F0; font-family: arial; font-size: 10pt;}\r\n"
					"table {background-color: white; border-top: solid 1px #444;}\r\n"
					"th {width: 240px; font-size: 10pt; font-weight: bold; text-align: left; vertical-align: top; padding: 4px 6px 4px 4px; border-left: solid 1px #444; border-right: solid 1px #444; border-bottom: solid 1px #444;}\r\n"
					"td {width: 512px; font-size: 10pt; vertical-align: top; padding: 4px 4px 4px 6px; border-right: solid 1px #444; border-bottom: solid 1px #444;}\r\n"
					"table.source {border: solid 1px #444; margin: 16px 0px 16px 0px;}\r\n"
					"td.line {vertical-align: top; text-align: right; font-family: 'courier new', fixed; background-color: #AAA; width: auto; border: 0px; padding: 1px 4px 1px 4px;}\r\n"
					"td.source {vertical-align: top; text-align: left; font-family: 'courier new', fixed; background-color: white; width: auto; border: 0px; padding: 1px 8px 1px 8px;}\r\n"
				"</style>\r\n"
			"</head>\r\n"
			"<body>\r\n"
				"<div style=\"width: 800px; font-size: 18pt; font-weight: bold; background-color: #C05050; padding: 0px 0px 2px 12px; margin-bottom: 20px;\">"
				"UnE Engine</div>\r\n",
		kReportLog);
	
	Report("<table cellspacing=\"0\" cellpadding=\"0\">\r\n", kReportLog);
	
	Report("<tr><th>Engine version</th><td>", kReportLog);
	Report(C4VERSION, kReportLog);
	
	#if C4DEBUG	
		Report(" (Debug)", kReportLog);	
	#endif
	
	Report("</td></tr>\r\n<tr><th>Application name</th><td>", kReportLog);
	Report(GetApplicationName(), kReportLog);
	
	Report("</td></tr>\r\n<tr><th>Time stamp</th><td>", kReportLog);
	TimeMgr::GetDateTimeStrings(&date, &time);
	Report(date += "<br/>", kReportLog);
	Report(time, kReportLog);
	
	Report("</td></tr>\r\n<tr><th>Operating system</th><td>", kReportLog);
		
	OSVERSIONINFO	versionInfo;		
	versionInfo.dwOSVersionInfoSize = sizeof(OSVERSIONINFO);
	GetVersionExA(&versionInfo);
		
	String<31> version("Windows ");
	version += (unsigned_int32) versionInfo.dwMajorVersion;
	version += ".";
	version += (unsigned_int32) versionInfo.dwMinorVersion;
	version += " ";
	Report(version, kReportLog);
	Report(versionInfo.szCSDVersion, kReportLog);
		
	Report("</td></tr>\r\n<tr><th>Hardware</th><td>", kReportLog);
		
	HKEY	keyHandle;		
	if (RegOpenKeyExA(HKEY_LOCAL_MACHINE, "HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0", 0, KEY_QUERY_VALUE, &keyHandle) == ERROR_SUCCESS)
	{
		DWORD	type;
		DWORD	size;
		BYTE	data[256];
			
		bool line = false;
			
		size = 255;
		if ((RegQueryValueExA(keyHandle, "ProcessorNameString", 0, &type, data, &size) == ERROR_SUCCESS) && (type == REG_SZ))
		{
			data[size] = 0;
			Report(reinterpret_cast<char *>(data), kReportLog);
			line = true;
		}
			
		size = 255;
		if ((RegQueryValueExA(keyHandle, "Identifier", 0, &type, data, &size) == ERROR_SUCCESS) && (type == REG_SZ))
		{
			data[size] = 0;
			if (line) Report("<br/>", kReportLog);
			Report(reinterpret_cast<char *>(data), kReportLog);
			line = true;
		}
			
		size = 255;
		if ((RegQueryValueExA(keyHandle, "~MHz", 0, &type, data, &size) == ERROR_SUCCESS) && (type == REG_DWORD))
		{
			if (line) Report("<br/>", kReportLog);
			Report(String<31>(*reinterpret_cast<unsigned_int32 *>(data)) += " MHz", kReportLog);
		}
	}	
	
	Report("</td></tr>\r\n<tr><th>Processor features</th><td>", kReportLog);
	
	if (processorFlags & kProcessorSSE) Report("SSE ", kReportLog);
	if (processorFlags & kProcessorSSE2) Report("SSE2 ", kReportLog);
	if (processorFlags & kProcessorSSE3) Report("SSE3 ", kReportLog);
	if (processorFlags & kProcessor3DNow) Report("3DNow! ", kReportLog);
	if (processorFlags & kProcessorAltivec) Report("Altivec ", kReportLog);
	
	Report("</td></tr>\r\n<tr><th>Processor count</th><td>", kReportLog);
	Report(Text::IntegerToString(processorCount), kReportLog);
	
	Report("</td></tr>\r\n<tr><th>Memory</th><td>", kReportLog);
	
		
	MEMORYSTATUSEX		memoryStatus;
		
	memoryStatus.dwLength = sizeof(MEMORYSTATUSEX);
	GlobalMemoryStatusEx(&memoryStatus);
	Report(String<31>(int64(((memoryStatus.ullTotalPhys >> 20) + 31) & ~31)) += " MB", kReportLog);
		
	Report("</td></tr>\r\n</table>\r\n", kReportLog);
}

void Engine::EndLog(void)
{
	Report("</body></html>\r\n", kReportLog);
	logFile.Close();
}

void Engine::LogResult(EngineResult result)
{
	if (result == kEngineOkay)
	{
		Report("<b>Success</b>", kReportLog | kReportSuccess);
	}
	else
	{
		String<63> message("<b>Error:</b> ");
		message += Text::IntegerToHexString8(result);
		Report(message += "<br/>", kReportLog);
		
		unsigned_int32 code = GetResultCode(result);
		switch (GetResultManager(result))
		{
			case kManagerEngine:
			{
				static const char *const string[] =
				{
					"Module failed to load",
					"Module failed to initialize",
					"<code>Construct()</code> function missing"
				};
				
				Report(string[code - 1], kReportLog | kReportError);
				break;
			}
			
			case kManagerFile:
			{
				static const char *const string[] =
				{
					"File open failed",
					"File is not open",
					"File read/write failed",
					"File locked",
					"File access denied",
					"File write protected",
					"File disk full",
					"File creation failed",
					"File deletion failed",
					"File async operation pending"
				};
				
				Report(string[code - 1], kReportLog | kReportError);
				break;
			}
			
			case kManagerResource:
			{
				static const char *const string[] =
				{
					"Resource not found",
					"Resource load failed",
					"Invalid pack file"
				};
				
				Report(string[code - 1], kReportLog | kReportError);
				break;
			}
			
			case kManagerDisplay:
				
				Report("DisplayMgr initialization failed", kReportLog | kReportError);
				break;
			
			case kManagerGraphics:
			{
				static const char *const string[] =
				{
					"Graphics context format failed",
					"Graphics context initialization failed",
					"Graphics hardware insufficient"
				};
				
				Report(string[code - 1], kReportLog | kReportError);
				break;
			}
			
			case kManagerSound:
			{
				static const char *const string[] =
				{
					"SoundMgr initialization failed",
					"Sound load failed",
					"Sound play failed",
					"Sound invalid format",
					"Sound too large for non-stream"
				};
				
				Report(string[code - 1], kReportLog | kReportError);
				break;
			}
			
			case kManagerAudioCapture:
				
				Report("Audio capture unavailable", kReportLog | kReportError);
				break;
			
			case kManagerInput:
				
				Report("InputMgr initialization failed", kReportLog | kReportError);
				break;
			
			case kManagerNetwork:
			{
				static const char *const string[] =
				{
					"NetworkMgr initialization failed",
					"Network no connection",
					"Network packet too large",
					"Network buffer full",
					"Network no packet available",
					"Network unresolved domain",
					"Network domain resolve pending",
					"Network domain resolve failed",
					"Network domain not found"
				};
				
				Report(string[code - 1], kReportLog | kReportError);
				break;
			}
			
			case kManagerWorld:
				
				Report("World failed to load", kReportLog | kReportError);
				break;
		}
	}
}

void Engine::LogSource(const char *source)
{
	String<>	string;
	
	Report("<table cellspacing=\"0\" cellpadding=\"0\" class=\"source\">\r\n", kReportLog);
	
	int32 line = 1;
	for (;;)
	{
		Report("<tr><td class=\"line\">", kReportLog);
		Report(Text::IntegerToString(line), kReportLog);
		Report("</td><td class=\"source\">", kReportLog);
		
		int32 len = Text::FindChar(source, 10);
		if (len < 0) len = Text::GetTextLength(source);
		
		int32 xpos = 1;
		int32 start = 0;
		for (machine a = 0; a < len; a++)
		{
			if (source[a] == 9)
			{
				static const char tabs[] = "&nbsp;&nbsp;&nbsp;&nbsp;";
				
				int32 size = a - start;
				xpos += size;
				Report(string.Set(source + start, size), kReportLog);
				
				int32 count = 4 - (xpos & 3);
				Report(&tabs[(4 - count) * 6], kReportLog);
				
				xpos += count;
				start = a + 1;
			}
		}
		
		if (start != len) Report(string.Set(source + start, len - start), kReportLog);
		Report("</td></tr>\r\n", kReportLog);
		
		if (source[len] == 0) break;
		
		line++;
		source += len + 1;
	}
	
	Report("</table>\r\n", kReportLog);
}

EngineResult Engine::LoadApplicationModule(void)
{
	String<kMaxFileNameLength>	moduleName;
	
	#ifdef GAME_MODULE_NAME
	
		moduleName = GAME_MODULE_NAME;
	
	#else
	
		Variable *variable = nullptr;
		for (machine a = 0; a < 2; a++)
		{
			variable = GetVariable("gameModuleName");
			if (variable)
			{
				variable->SetVariableFlags(kVariableNonpersistent);
				variable->AddObserver(&gameModuleObserver);
				moduleName = variable->GetValue();
				break;
			}
			
			ExecuteFile("game");
		}
		
		if (!variable) moduleName = "Game";
	
	#endif
	
	Report("Application Module", kReportLog | kReportHeading);
	
	Report("<table cellspacing=\"0\" cellpadding=\"0\">\r\n<tr><th>", kReportLog);
	Report(moduleName, kReportLog);
	Report("</th><td>\r\n", kReportLog);
	
	ApplicationModule *module = new ApplicationModule;
	EngineResult result = module->Load(moduleName);
	if (result == kEngineOkay)
	{
		applicationModule = module;
	}
	else
	{
		delete module;
		applicationModule = nullptr;
	}
	
	LogResult(result);
	Report("</table>\r\n", kReportLog);
	Report("", kReportLog | kReportHeading);
	
	if (result == kEngineOkay)
	{
		if (!ExecuteFile(C4_INPUT_CONFIG_FILE, TheResourceMgr->GetConfigCatalog())) ExecuteFile(C4_INPUT_CONFIG_FILE);
	}
	
	return (result);
}

void Engine::UnloadApplicationModule(void)
{
	delete applicationModule;
	applicationModule = nullptr;
}

void Engine::HandleGameModuleEvent(Variable *variable)
{
	DeferredTask *task = new DeferredTask(&ChangeGameModule, this);
	task->SetTaskFlags(kTaskNonpersistent);
	TheTimeMgr->AddTask(task);
}

void Engine::ChangeGameModule(DeferredTask *task, void *cookie)
{
	Engine *engine = static_cast<Engine *>(cookie);
	
	delete TheApplication;
	engine->UnloadApplicationModule();
	
	if (engine->LoadApplicationModule() != kEngineOkay) engine->Quit();
}

LRESULT CALLBACK Engine::WindowProc(HWND window, UINT message, WPARAM wparam, LPARAM lparam)
{
	switch (message)
	{
		case WM_ACTIVATEAPP:
		case WM_SHOWWINDOW:
		case WM_SYSCOMMAND:
		case WM_DEVICECHANGE:
		case WM_CLOSE:
				
			if ((window == TheEngine->engineWindow) && (TheEngine->HandleApplicationEvent(message, wparam, lparam))) return (0);
			break;
			
		case WM_LBUTTONDOWN:
		case WM_LBUTTONUP:
		case WM_RBUTTONDOWN:
		case WM_RBUTTONUP:
		case WM_MBUTTONDOWN:
		case WM_MBUTTONUP:
		case WM_MOUSEWHEEL:
		case WM_MOUSEMOVE:
				
			if (!(TheDisplayMgr->GetDisplayFlags() & kDisplayFullscreen)) TheDisplayMgr->HideCursor();
			TheEngine->HandleMouseEvent(window, message, wparam, lparam);
			return (0);
			
		case WM_NCMOUSEMOVE:
				
			if (!(TheDisplayMgr->GetDisplayFlags() & kDisplayFullscreen)) TheDisplayMgr->ShowCursor();
			break;
			
		case WM_KEYDOWN:
		case WM_KEYUP:
				
			TheEngine->HandleKeyboardEvent(window, message, wparam, lparam);
			return (0);
			
		case WM_CHAR:
				
			TheEngine->deadKeyFlag = false;
				
			if (wparam >= 0x00A0)
			{
				KeyboardEventData	eventData;
					
				eventData.eventType = kEventKeyDown;
				eventData.keyCode = wparam;
				eventData.modifierKeys = (((lparam >> 16) & 0xFF) != DIK_GRAVE) ? 0 : kModifierKeyConsole;
					
				const KeyboardEventHandler *handler = TheEngine->keyboardEventHandlerList.First();
				while (handler)
				{
					const KeyboardEventHandler *next = handler->Next();
					if (handler->HandleEvent(&eventData)) break;
					handler = next;
				}
					
				return (0);
			}
				
			break;
			
		case WM_DEADCHAR:
				
			TheEngine->deadKeyFlag = true;
			break;
			
		case WM_SYSKEYDOWN:
		case WM_SYSKEYUP:
				
			return (0);
			
		case WM_ERASEBKGND:
				
			return (1);
			
		case WM_INPUT:
				
			if (TheEngine->multiaxisMouseActiveCount > 0) TheEngine->HandleMultiaxisMouseEvent(wparam, lparam);
			break;
			
		case WT_PACKET:
		case WT_PROXIMITY:
				
			if (TheEngine->tabletActiveCount > 0) TheEngine->HandleTabletEvent(message, wparam, lparam);
			return (0);
			
		case kWindowsMessageResolve:
				
			DomainNameResolver::ResolveCallback((HANDLE) wparam, WSAGETASYNCERROR(lparam));
			return (0);
	}
		
	return (DefWindowProcW(window, message, wparam, lparam));
}
	
bool Engine::HandleApplicationEvent(UINT message, WPARAM wparam, LPARAM lparam)
{
	if (message == WM_ACTIVATEAPP)
	{
		if (wparam)
		{
			engineFlags |= kEngineForeground;
			//if ((TheDisplayMgr) && (TheDisplayMgr->GetDisplayFlags() & kDisplayFullscreen)) ShowWindow(engineWindow, SW_RESTORE);
			//if (TheInterfaceMgr) TheInterfaceMgr->ReadSystemClipboard();
		}
		else
		{
			engineFlags &= ~kEngineForeground;
			//if ((TheDisplayMgr) && (TheDisplayMgr->GetDisplayFlags() & kDisplayFullscreen)) ShowWindow(engineWindow, SW_MINIMIZE);
			//if (TheInterfaceMgr) TheInterfaceMgr->WriteSystemClipboard();
		}
	}
	else if (message == WM_SHOWWINDOW)
	{
		if (wparam) engineFlags |= kEngineVisible;
		else engineFlags &= ~(kEngineForeground | kEngineVisible);
	}
	else if (message == WM_SYSCOMMAND)
	{
		wparam &= 0xFFF0;
		if ((wparam != SC_SCREENSAVE) && (wparam != SC_MONITORPOWER)) return (false);
	}
	else if (message == WM_CLOSE)
	{
		engineFlags |= kEngineQuit;
	}
		
	return (true);
}
	
bool Engine::HandleWindowEvent(UINT message, WPARAM wparam, LPARAM lparam)
{
	return (false);
}
	
void Engine::HandleMouseEvent(HWND window, UINT message, WPARAM wparam, LPARAM lparam)
{
	if (!(InputMgr::GetInternalInputMode() & kInputMouseActive))
	{
		MouseEventData	eventData;
			
		eventData.mousePosition.Set((float) (int16) LOWORD(lparam), (float) (int16) HIWORD(lparam), 0.0F);
		lastMousePosition = eventData.mousePosition;
			
		eventData.eventType = kEventNone;
			
		switch (message)
		{
			case WM_LBUTTONDOWN:
					
				if ((mouseButtonMask & ~1) == 0)
				{
					mouseButtonMask = 1;
					eventData.eventType = kEventMouseDown;
					SetCapture(window);
				}
					
				break;
				
			case WM_LBUTTONUP:
					
				if (mouseButtonMask == 1)
				{
					mouseButtonMask = 0;
					eventData.eventType = kEventMouseUp;
					ReleaseCapture();
				}
					
				break;
				
			case WM_RBUTTONDOWN:
					
				if ((mouseButtonMask & ~2) == 0)
				{
					mouseButtonMask = 2;
					eventData.eventType = kEventRightMouseDown;
					SetCapture(window);
				}
					
				break;
				
			case WM_RBUTTONUP:
					
				if (mouseButtonMask == 2)
				{
					mouseButtonMask = 0;
					eventData.eventType = kEventRightMouseUp;
					ReleaseCapture();
				}
					
				break;
				
			case WM_MBUTTONDOWN:
					
				if ((mouseButtonMask & ~4) == 0)
				{
					mouseButtonMask = 4;
					eventData.eventType = kEventMiddleMouseDown;
					SetCapture(window);
				}
					
				break;
				
			case WM_MBUTTONUP:
					
				if (mouseButtonMask == 4)
				{
					mouseButtonMask = 0;
					eventData.eventType = kEventMiddleMouseUp;
					ReleaseCapture();
				}
					
				break;
				
			case WM_MOUSEWHEEL:
			{
				int32 delta = wheelDeltaAccum + GET_WHEEL_DELTA_WPARAM(wparam);
				int32 k = delta / WHEEL_DELTA;
				if (k != 0)
				{
					eventData.eventType = kEventMouseWheel;
					eventData.mousePosition.y = (float) k;
					delta -= k * WHEEL_DELTA;
				}
					
				wheelDeltaAccum = delta;
				break;
			}
				
			default:
					
				eventData.eventType = kEventMouseMoved;
				break;
		}
			
		if (eventData.eventType != kEventNone)
		{
			eventData.eventFlags = 0;
				
			const MouseEventHandler *handler = mouseEventHandlerList.First();
			while (handler)
			{
				const MouseEventHandler *next = handler->Next();
				if (handler->HandleEvent(&eventData)) break;
				handler = next;
			}
		}
	}
}
	
void Engine::HandleKeyboardEvent(HWND window, UINT message, WPARAM wparam, LPARAM lparam)
{
	if (!(InputMgr::GetInternalInputMode() & kInputKeyboardActive))
	{
		static const unsigned_int8 align_address(16) keyCodeMap[48] =
		{
			0, 0, 0, 0, 0, 0, 0, 0, kKeyCodeBackspace, kKeyCodeTab, 0, 0, 0, kKeyCodeReturn, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, kKeyCodeEscape, 0, 0, 0, 0,
			0, kKeyCodePageUp, kKeyCodePageDown, kKeyCodeEnd, kKeyCodeHome, kKeyCodeLeftArrow, kKeyCodeUpArrow, kKeyCodeRightArrow, kKeyCodeDownArrow, 0, 0, 0, 0, 0, kKeyCodeDelete, 0
		};
			
		KeyboardEventData	eventData;
		BYTE				keyState[256];
			
		GetKeyboardState(keyState);
		eventData.modifierKeys = (keyState[VK_SHIFT] & 0x80) ? kModifierKeyShift : 0;
			
		unsigned_int32 scan = (lparam >> 16) & 0xFF;
		if (scan == DIK_GRAVE) eventData.modifierKeys |= kModifierKeyConsole;
			
		bool command = false;
		if ((keyState[VK_CONTROL] & 0x80) && (!(keyState[VK_RMENU] & 0x80)))
		{
			command = true;
			keyState[VK_SHIFT] = 0;
			keyState[VK_LSHIFT] = 0;
			keyState[VK_RSHIFT] = 0;
			keyState[VK_CONTROL] = 0;
			keyState[VK_LCONTROL] = 0;
			keyState[VK_RCONTROL] = 0;
		}
			
		unsigned_int32 code = (wparam < 0x0030) ? keyCodeMap[wparam] : 0;
		if (code == 0)
		{
			WCHAR buffer = 0;
			if (ToUnicode(wparam, scan, keyState, &buffer, 1, 0) == 1) code = buffer;
		}
			
		if (code - 1 < 0x007FU)
		{
			eventData.eventType = kEventNone;
				
			if (message == WM_KEYDOWN)
			{
				if (command)
				{
					eventData.eventType = kEventKeyCommand;
					if (code - 0x0061 < 26U) code -= 0x0020;
				}
				else
				{
					if ((code < 0x0020) || (!deadKeyFlag)) eventData.eventType = kEventKeyDown;
				}
			}
			else
			{
				if (!command) eventData.eventType = kEventKeyUp;
			}
				
			if (eventData.eventType != kEventNone)
			{
				eventData.keyCode = code;
					
				const KeyboardEventHandler *handler = keyboardEventHandlerList.First();
				while (handler)
				{
					const KeyboardEventHandler *next = handler->Next();
					if (handler->HandleEvent(&eventData)) break;
					handler = next;
				}
			}
		}
	}
}
	
void Engine::HandleMultiaxisMouseEvent(WPARAM wparam, LPARAM lparam)
{
	char	data[256];
		
	UINT size = 256;
	if (GetRawInputData(reinterpret_cast<HRAWINPUT>(lparam), RID_INPUT, data, &size, sizeof(RAWINPUTHEADER)) - 1 < 256U)
	{
		const RAWINPUT *rawInput = reinterpret_cast<RAWINPUT *>(data);
		if (rawInput->header.dwType == RIM_TYPEHID)
		{
			RID_DEVICE_INFO		deviceInfo;
				
			deviceInfo.cbSize = sizeof(RID_DEVICE_INFO);
			size = sizeof(RID_DEVICE_INFO);
			if (GetRawInputDeviceInfoA(rawInput->header.hDevice, RIDI_DEVICEINFO, &deviceInfo, &size) - 1 < sizeof(RID_DEVICE_INFO))
			{
				if (deviceInfo.hid.dwVendorId == 0x046D)		// LOGITECH_VENDOR_ID
				{
					MouseEventData	eventData;
						
					eventData.eventType = kEventNone;
					eventData.eventFlags = 0;
						
					const BYTE *raw = rawInput->data.hid.bRawData;
					int32 type = raw[0];
					if (type == 1)
					{
						int32 dx = raw[1] | (reinterpret_cast<const char *>(raw)[2] << 8);
						int32 dy = raw[3] | (reinterpret_cast<const char *>(raw)[4] << 8);
						int32 dz = raw[5] | (reinterpret_cast<const char *>(raw)[6] << 8);
							
						eventData.eventType = kEventMultiaxisMouseTranslation;
						eventData.mousePosition.Set((float) dx * 3.2e-5F, (float) dy * 3.2e-5F, (float) dz * 3.2e-5F);
					}
					else if (type == 2)
					{
						int32 rx = raw[1] | (reinterpret_cast<const char *>(raw)[2] << 8);
						int32 ry = raw[3] | (reinterpret_cast<const char *>(raw)[4] << 8);
						int32 rz = raw[5] | (reinterpret_cast<const char *>(raw)[6] << 8);
							
						eventData.eventType = kEventMultiaxisMouseRotation;
						eventData.mousePosition.Set((float) rx * 8.0e-6F, (float) ry * 8.0e-6F, (float) rz * 8.0e-6F);
					}
					else if (type == 3)
					{
						eventData.eventType = kEventMultiaxisMouseButtonState;
						eventData.eventFlags = raw[1] | (raw[2] << 8) | (raw[3] << 16) | (raw[4] << 24);
					}
						
					if (eventData.eventType != kEventNone)
					{
						const MouseEventHandler *handler = mouseEventHandlerList.First();
						while (handler)
						{
							const MouseEventHandler *next = handler->Next();
							if (handler->HandleEvent(&eventData)) break;
							handler = next;
						}
					}
				}
			}
		}
	}
}
	
void Engine::HandleTabletEvent(UINT message, WPARAM wparam, LPARAM lparam)
{
	if (!(InputMgr::GetInternalInputMode() & kInputMouseActive))
	{
		if (message == WT_PACKET)
		{
			TabletPacket	packet[8];
				
			int32 count = WTPacketsGet((HCTX) lparam, 8, packet);
			if (count > 0)
			{
				unsigned_int32 pressure = packet[count - 1].normalPressure - stylusMinPressure;
				stylusPressure = (float) pressure * stylusPressureScale;
			}
		}
		else if (message == WT_PROXIMITY)
		{
			if (LOWORD(lparam) == 0) stylusPressure = 0.0F;
		}
	}
}
	
bool Engine::PluginFilter(const char *name, unsigned_int32 flags, void *cookie)
{
	if (name[0] == '.') return (false);
	if (flags & kFileDirectory) return (true);
		
	int32 length = Text::GetTextLength(name);
	if (length < 5) return (false);
		
	return (Text::CompareTextCaseless(&name[length - 4], ".dll"));
}
	
void Engine::GetPluginList(const char *directory, List<FileReference> *fileList) const
{
#if C4DEBUG
	String<kMaxFileNameLength> name("Plugins_d");
#else
	String<kMaxFileNameLength> name("Plugins");
#endif
	FileMgr::BuildFileList(name += directory, fileList, &PluginFilter);
}

void Engine::ResetMouseButtonMask(void)
{
	if (mouseButtonMask != 0)
	{
		MouseEventData	eventData;
		
		if (mouseButtonMask & 7)
		{
			if (mouseButtonMask == 1) eventData.eventType = kEventMouseUp;
			else if (mouseButtonMask == 2) eventData.eventType = kEventRightMouseUp;
			else if (mouseButtonMask == 4) eventData.eventType = kEventMiddleMouseUp;
			
			eventData.eventFlags = 0;
			eventData.mousePosition = lastMousePosition;
			
			const MouseEventHandler *handler = TheEngine->GetFirstMouseEventHandler();
			while (handler)
			{
				const MouseEventHandler *next = handler->Next();
				if (handler->HandleEvent(&eventData)) break;
				handler = next;
			}
		}
		
		mouseButtonMask = 0;
	}
}

Variable *Engine::InitVariable(const char *name, const char *value, unsigned_int32 flags, Variable::ObserverType *observer)
{
	Variable *variable = GetVariable(name);
	if (!variable)
	{
		variable = new Variable(name, flags, observer);
		variable->SetValue(value);
		AddVariable(variable);
	}
	else
	{
		variable->SetVariableFlags(flags);
		
		if (observer)
		{
			variable->AddObserver(observer);
			observer->InvokeCallback(variable);
		}
	}
	
	return (variable);
}

void Engine::ExecuteCommand(const char *text)
{
	String<kMaxCommandLength>	name;
	
	text += Text::ReadIdentifier(text, name, kMaxCommandLength);
	text += Text::GetWhitespaceLength(text);
	
	if (name[0] != '$')
	{
		Command *command = commandMap.Find(name);
		if (command)
		{
			command->PostEvent(text);
		}
		else
		{
			Report(String<kMaxCommandLength + 64>("[#FF8]Unrecognized command: [#FFF]") += name, kReportError);
		}
	}
	else
	{
		Variable *variable = GetVariable(&name[1]);
		if (*text == '=')
		{
			String<kMaxVariableValueLength>		value;
			
			if (!variable)
			{
				variable = new Variable(&name[1]);
				AddVariable(variable);
			}
			
			text++;
			text += Text::GetWhitespaceLength(text);
			Text::ReadString(text, value, kMaxVariableValueLength);
			variable->SetValue(value);
		}
		else
		{
			if (variable)
			{
				name += " = ";
				name += variable->GetValue();
				Report(name);
			}
			else
			{
				Report(String<kMaxCommandLength + 64>("[#FF8]Undefined variable: [#FFF]") += name, kReportError);
			}
		}
	}
}

void Engine::ExecuteText(const char *text)
{
	if (text)
	{
		for (;;)
		{
			text += Text::GetWhitespaceLength(text);
			if (*text == 0) return;
			
			int32 length = Text::FindUnquotedChar(text, ';');
			if (length < 0)
			{
				ExecuteCommand(text);
				break;
			}
			
			if (length > 0)
			{
				if (length > 63)
				{
					String<>	line;
					
					line.Set(text, length);
					ExecuteCommand(line);
				}
				else
				{
					String<63>	line;
					
					line.Set(text, length);
					ExecuteCommand(line);
				}
			}
			
			text += length + 1;
		}
	}
}

bool Engine::ExecuteFile(const char *name, ResourceCatalog *catalog)
{
	ConfigResource *config = ConfigResource::Get(name, 0, catalog);
	if (config)
	{
		ExecuteText(config->GetText());
		config->Release();
		return (true);
	}
	
	return (false);
}

void Engine::WriteEngineConfig(const char *name) const
{
	File			file;
	ResourcePath	path;
	
	TheResourceMgr->GetConfigCatalog()->GetResourcePath(ConfigResource::GetDescriptor(), name, &path);
	if (file.Open(path, kFileCreate) == kFileOkay)
	{
		Variable *variable = variableMap.First();
		while (variable)
		{
			if (!(variable->GetVariableFlags() & kVariableNonpersistent))
			{
				file << "$" << variable->GetName() << " = \"";
				WriteConfigString(file, variable->GetValue());
				file << "\";\r\n";
			}
			
			variable = variable->Next();
		}
	}
	
	const Persistor *persistor = persistorList.First();
	while (persistor)
	{
		file << "\r\n";
		persistor->WriteConfig(file);
		
		persistor = persistor->Next();
	}
}

void Engine::WriteInputConfig(const char *name) const
{
	File			file;
	ResourcePath	path;
	
	TheResourceMgr->GetConfigCatalog()->GetResourcePath(ConfigResource::GetDescriptor(), name, &path);
	if (file.Open(path, kFileCreate) == kFileOkay)
	{
		InputDevice *device = TheInputMgr->GetFirstDevice();
		while (device)
		{
			file << "$device = \"" << device->GetDeviceName() << "\";\r\n";
			
			InputControl *control = device->GetFirstControl();
			while (control)
			{
				Action *action = control->GetControlAction();
				if (action)
				{
					unsigned_int32 type = action->GetActionType();
					if (type != 0)
					{
						if (!(action->GetActionFlags() & kActionImmutable))
						{
							char	c[5];
							
							c[0] = (char) (type >> 24);
							c[1] = (char) ((type >> 16) & 255);
							c[2] = (char) ((type >> 8) & 255);
							c[3] = (char) (type & 255);
							c[4] = 0;
							
							file << "bind \"";
							WriteConfigString(file, control->GetControlName());
							file << "\" %" << c << ";\r\n";
						}
					}
					else
					{
						CommandAction *cmd = static_cast<CommandAction *>(action);
						file << "bind \"";
						WriteConfigString(file, control->GetControlName());
						file << "\" \"";
						WriteConfigString(file, cmd->GetCommand());
						file << "\";\r\n";
					}
				}
				
				control = device->GetNextControl(control);
			}
			
			file << "\r\n";
			device = device->Next();
		}
		
		file << "undef $device;\r\n";
	}
}

void Engine::WriteConfigString(File& file, const char *string)
{
	String<kMaxCommandLength>	output;
	
	for (machine x = 0;;)
	{
		unsigned_int32 c = *reinterpret_cast<const unsigned_int8 *>(string);
		if ((c == 34) || (c == 92))
		{
			if (x < kMaxCommandLength - 2)
			{
				output[x++] = 92;
				output[x++] = (char) c;
			}
			else
			{
				output[x] = 0;
				break;
			}
		}
		else
		{
			output[x++] = (char) c;
			if (c == 0) break;
		}
		
		string++;
		if (x == kMaxCommandLength - 1)
		{
			output[x] = 0;
			break;
		}
	}
	
	file << output;
}

void Engine::Logger(const char *text, unsigned_int32 flags, void *cookie)
{
	if (flags & kReportLog)
	{
		File& file = static_cast<Engine *>(cookie)->logFile;
		
		if (flags & kReportHeading)
		{
			file << "<div style=\"width: 800px; font-size: 18pt; font-weight: bold; border-top: solid 2px #808080; margin: 20px 0px 15px 0px;\">";
			file << text << "</div>\r\n";
		}
		else
		{
			if (flags & kReportFormatted)
			{
				file << "\r\n<br/><pre>\r\n" << text << "</pre>\r\n";
			}
			else
			{
				if (flags & kReportError) file << "<span style=\"color: #800000;\">" << text << "</span>";
				else if (flags & kReportSuccess) file << "<span style=\"color: #006000;\">" << text << "</span>";
				else file << text;
			}
		}
	}
}

void Engine::Report(const char *text, unsigned_int32 flags)
{
	Reporter *reporter = reporterList.Last();
	while (reporter)
	{
		reporter->Report(text, flags);
		reporter = reporter->Previous();
	}
}

void Engine::Run(void)
{
	//TheTimeMgr->ResetTime();
	
	//for (;;)
	//{
		TheTimeMgr->TimeTask();
		//if (engineFlags & kEngineQuit) break;

		//MSG		message;			
		//while (PeekMessageA(&message, nullptr, 0, 0, PM_REMOVE))
		//{
		//	TranslateMessage(&message);
		//	DispatchMessageA(&message);
		//}	
		
		TheInterfaceMgr->InterfaceTask();
		TheMessageMgr->ReceiveTask();
		
		if (engineFlags & kEngineForeground)
			TheInputMgr->InputTask();
		TheAudioCaptureMgr->AudioCaptureTask();
		
		TheApplication->ApplicationTask();
		ThePluginMgr->PluginTask();
		TheWorldMgr->Move();
		TheMessageMgr->SendTask();
		TheSoundMgr->SoundTask();
		
		if (engineFlags & kEngineVisible)
		{
			TheGraphicsMgr->BeginRendering();
			
			TheWorldMgr->Render();
			TheApplication->WorldRenderTask();
			TheInterfaceMgr->Render();
			TheApplication->InterfaceRenderTask();
			
			TheGraphicsMgr->EndRendering();
		}
		
		/*#if !C4GAMECONSOLE

		bool multiplayerServer = (TheMessageMgr->Multiplayer()) & (TheMessageMgr->Server());
		if ((!multiplayerServer) && (!(engineFlags & kEngineForeground)))
		{
			int32 dt = TheTimeMgr->GetDeltaTime();
			int32 sleepTime = Max(backgroundSleepTime + (40 - dt) / 2, 1);
			backgroundSleepTime = sleepTime;
			Thread::Sleep(sleepTime);
		}

		#endif*/
	//}
}

void Engine::Quit(void)
{
	TheMessageMgr->DisconnectAll();
	engineFlags |= kEngineQuit;
}

void Engine::StartMultiaxisMouse(void)
{
	if (++multiaxisMouseActiveCount == 1)
	{
		RAWINPUTDEVICE		device;
			
		device.usUsagePage = 1;
		device.usUsage = 8;
		device.dwFlags = 0;
		device.hwndTarget = engineWindow;
			
		RegisterRawInputDevices(&device, 1, sizeof(RAWINPUTDEVICE));
	}
}

void Engine::StopMultiaxisMouse(void)
{
	if (--multiaxisMouseActiveCount == 0)
	{		
		RAWINPUTDEVICE		device;
			
		device.usUsagePage = 1;
		device.usUsage = 8;
		device.dwFlags = RIDEV_REMOVE;
		device.hwndTarget = nullptr;
			
		RegisterRawInputDevices(&device, 1, sizeof(RAWINPUTDEVICE));
		
	}
}

void Engine::StartTablet(void)
{
	if (++tabletActiveCount == 1)
	{		
		tabletLibrary = LoadLibraryA("Wintab32.dll");
		if (tabletLibrary)
		{
			TabletLogContext	logicalContext;

			*(void **) &WTInfoA = GetProcAddress(tabletLibrary, "WTInfoA");
			*(void **) &WTOpenA = GetProcAddress(tabletLibrary, "WTOpenA");
			*(void **) &WTClose = GetProcAddress(tabletLibrary, "WTClose");
			*(void **) &WTPacketsGet = GetProcAddress(tabletLibrary, "WTPacketsGet");

			if (WTInfoA(WTI_DEFSYSCTX, 0, &logicalContext) != 0)
			{
				logicalContext.lcOptions = CXO_SYSTEM | CXO_MESSAGES;
				logicalContext.lcMsgBase = WT_DEFBASE;
				logicalContext.lcPktData = PK_NORMAL_PRESSURE;
				logicalContext.lcMoveMask = PK_NORMAL_PRESSURE;

				tabletContext = WTOpenA(engineWindow, &logicalContext, true);
				if (tabletContext)
				{
					TabletAxis	pressureRange;

					WTInfoA(WTI_DEVICES, DVC_NPRESSURE, &pressureRange);
					stylusMinPressure = pressureRange.axMin;
					stylusPressureScale = 1.0F / (float) (pressureRange.axMax - pressureRange.axMin);
				}
			}
			else
			{
				tabletContext = nullptr;
			}
		}
	}
}

void Engine::StopTablet(void)
{
	if (--tabletActiveCount == 0)
	{		
		if (tabletLibrary)
		{
			if (tabletContext) WTClose(tabletContext);
			FreeLibrary(tabletLibrary);
		}		
	}
	
	stylusPressure = 0.0F;
}

bool Engine::OpenExternalWebBrowser(const char *url)
{
	DWORD	code;
		
	HANDLE threadHandle = CreateThread(nullptr, 0, &ShellThread, &url, 0, nullptr);
	WaitForSingleObjectEx(threadHandle, INFINITE, false);
		
	bool result = ((GetExitCodeThread(threadHandle, &code)) && (code == 0));
		
	CloseHandle(threadHandle);
	return (result);
}



DWORD WINAPI Engine::ShellThread(void *cookie)
{
	SHELLEXECUTEINFOA	info;

	CoInitializeEx(nullptr, COINIT_APARTMENTTHREADED | COINIT_DISABLE_OLE1DDE);

	info.cbSize = sizeof(SHELLEXECUTEINFOA);
	info.fMask = SEE_MASK_NOASYNC | SEE_MASK_FLAG_NO_UI;
	info.hwnd = nullptr;
	info.lpVerb = "open";
	info.lpFile = *reinterpret_cast<const char **>(cookie);
	info.lpParameters = nullptr;
	info.lpDirectory = nullptr;
	info.nShow = SW_SHOWNORMAL;

	DWORD result = 1;
	if (ShellExecuteExA(&info)) result = ((int) info.hInstApp <= 32);

	CoUninitialize();
	return (result);
}



#if C4LEAK_DETECTION

	void Engine::DumpMemory(const char *filename)
	{
		File	file;
		
		if (file.Open(filename, kFileCreate) == kFileOkay)
		{
			const Heap *heap = MemoryMgr::GetFirstHeap();
			while (heap)
			{
				file << "Heap: " << heap->GetHeapName() << "\r\n";
				
				int32 poolCount = 0;
				const MemPoolHeader *pool = heap->GetFirstPool();
				while (pool)
				{
					file << "Pool #" << String<15>(poolCount) << "\r\n";
					
					const MemBlockHeader *block = pool->GetFirstBlock();
					do
					{
						if (block->blockFlags & kMemoryBlockUsed)
						{
							String<15> line(block->allocLine);
							String<15> size(block->logicalSize);
							file << "\t" << block->allocFile << "\t\tline " << (const char *) line << "\t\t" << (const char *) size << " bytes\r\n";
						}
						
						block = block->nextBlock;
					} while (block);
					
					poolCount++;
					pool = pool->nextPool;
				}
				
				file << "\r\n";
				heap = heap->GetNextHeap();
			}
			
			file << "System Blocks\r\n";
			const MemBlockHeader *block = MemoryMgr::GetFirstSystemBlock();
			while (block)
			{
				String<15> line(block->allocLine);
				String<15> size(block->logicalSize);
				file << "\t" << block->allocFile << "\t\tline " << (const char *) line << "\t\t" << (const char *) size << " bytes\r\n";
				block = block->nextBlock;
			}
		}
	}

#endif


Module::Module(ModuleType type)
{
	moduleType = type;
	moduleLoaded = false;
}

Module::~Module()
{
	#if !C4LEAK_DETECTION
	
		Unload();
	
	#endif
}

EngineResult Module::Load(const char *name)
{
#if C4DEBUG 
	String<kMaxFilePathLength> path((moduleType == kModulePlugin) ? "Plugins_d\\" : "");
#else
	String<kMaxFilePathLength> path((moduleType == kModulePlugin) ? "Plugins\\" : "");
#endif

	path += name;
	path += ".dll";

	moduleHandle = LoadLibraryA(path);
	if (!moduleHandle)
		return (kEngineModuleLoadFailed);
	
	moduleLoaded = true;
	return (kEngineOkay);
}

void Module::Unload(void)
{
	if (moduleLoaded)
	{
		moduleLoaded = false;
		
		FreeLibrary(moduleHandle);	
	}
}


ApplicationModule::ApplicationModule() : Module(kModuleApplication)
{
}

ApplicationModule::~ApplicationModule()
{
}

EngineResult ApplicationModule::Load(const char *name)
{
	ConstructProc *constructor = &ConstructApplication;
	
	if (!constructor)
		return (kEngineModuleConstructMissing);
	if (!(*constructor)())
		return (kEngineModuleInitFailed);
	return (kEngineOkay);
}

// ZYURVUR
