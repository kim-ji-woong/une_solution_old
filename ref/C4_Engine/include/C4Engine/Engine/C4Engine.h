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


#ifndef C4Engine_h
#define C4Engine_h


//# \component	System Utilities
//# \prefix		System/

//# \import		C4Types.h


#include "C4Resources.h"
#include "C4Variables.h"
#include "C4Commands.h"

#if C4WINDOWS

	#include "C4Wintab.h"

#endif


namespace C4
{
	enum
	{
		kEngineModuleLoadFailed			= (kManagerEngine << 16) | 0x0001,
		kEngineModuleInitFailed			= (kManagerEngine << 16) | 0x0002,
		kEngineModuleConstructMissing	= (kManagerEngine << 16) | 0x0003
	};
	
	
	//# \enum	ReportFlags
	
	enum
	{
		kReportLog				= 1 << 0,		//## The text is written to the standard log file.
		kReportError			= 1 << 1,		//## The text should be considered to be an error message.
		kReportSuccess			= 1 << 2,		//## The text should be considered to be a success message.
		kReportHeading			= 1 << 3,		//## The text should be formatted as a heading.
		kReportFormatted		= 1 << 4		//## The text is preformatted and should be output with no changes to its spacing.
	};
	
	
	//# \enum	EngineFlags
	
	enum
	{
		kEngineForeground		= 1 << 0,		//## The engine is currently the foreground process.
		kEngineVisible			= 1 << 1,		//## The engine is currently visible (not minimized or otherwise hidden).
		kEngineQuit				= 1 << 2		//## The engine is in the process of quitting.
	};
	
	
	enum
	{
		kProcessorSSE			= 1 << 0,
		kProcessorSSE2			= 1 << 1,
		kProcessorSSE3			= 1 << 2,
		kProcessor3DNow			= 1 << 3,
		kProcessorAltivec		= 1 << 4
	};
	
	
	enum ModuleType
	{
		kModuleApplication,
		kModulePlugin
	};
	
	
	class Application;
	class ApplicationModule;
	class DomainNameResolver;
	class DeferredTask;
	
	
	class C4_API ConfigResource : public Resource<ConfigResource>
	{
		friend class Resource<ConfigResource>;
		
		private:
			
			static ResourceDescriptor		descriptor;
			
			~ConfigResource();
			
			void Preprocess(void);
		
		public:
			
			ConfigResource(const char *name, ResourceCatalog *catalog);
			
			const char *GetText(void) const
			{
				return (static_cast<const char *>(GetData()));
			}
	};
	
	
	//# \class	Reporter	Handles messages passed to the system report chain.
	//
	//# The $Reporter$ class handles messages passed to the system report chain. 
	//
	//# \def	class Reporter : public ListElement<Reporter> 
	// 
	//# \ctor	Reporter(ReportProc *proc, void *cookie = nullptr); 
	//
	//# \param	proc	The procedure to invoke when a message is reported. 
	//# \param	cookie	The cookie that is passed to the report procedure as its last parameter.
	//
	//# \desc
	//# The $Reporter$ class encapsulates a procedure that is invoked when the 
	//# $@Engine::Report@$ function is called. Once an instance of the $Reporter$ class has
	//# been constructed, it can be installed by calling the $@Engine::InstallReporter@$ function.
	//# 
	//# When a message is reported, the procedures corresponding to all installed reporters are 
	//# invoked. The $ReportProc$ type is defined as follows.
	//
	//# \code	typedef void ReportProc(const char *text, unsigned_int32 flags, void *cookie);
	//
	//# The $text$ and $flags$ parameters passed to the procedure pointed to by the
	//# $proc$ parameter match those passed to the $@Engine::Report@$ function. The $cookie$
	//# parameter passed to the procedure pointed to by the $proc$ parameter matches the $cookie$
	//# parameter passed to the constructor of the $Reporter$ class.
	//# 
	//# A reporter is uninstalled by destroying its associated class instance.
	//
	//# \base	Utilities/ListElement<Reporter>		Used internally to store all instances of $Reporter$ in a list.
	
	
	class C4_API Reporter : public ListElement<Reporter>
	{
		public:
			
			typedef void ReportProc(const char *, unsigned_int32, void *);
		
		private:
			
			ReportProc		*reportProc;
			void			*reportCookie;
		
		public:
			
			Reporter(ReportProc *proc, void *cookie = nullptr);
			
			void Report(const char *text, unsigned_int32 flags) const
			{
				(*reportProc)(text, flags, reportCookie);
			}
	};
	
	
	class C4_API Persistor : public ListElement<Persistor>
	{
		public:
			
			typedef void WriteProc(File&, void *);
		
		private:
			
			WriteProc		*persistProc;
			void			*persistCookie;
		
		public:
			
			Persistor(WriteProc *proc, void *cookie = nullptr);
			
			void WriteConfig(File& file) const
			{
				(*persistProc)(file, persistCookie);
			}
	};
	
	
	//# \class	MouseEventHandler	Encapsulates a mouse event handler function.
	//
	//# The $MouseEventHandler$ class encapsulates a mouse event handler function.
	//
	//# \def	class MouseEventHandler : public ListElement<MouseEventHandler>
	//
	//# \ctor	MouseEventHandler(HandlerProc *proc, void *cookie = nullptr);
	//
	//# \param	proc	The procedure to invoke when a mouse event occurs.
	//# \param	cookie	The cookie that is passed to the event handler as its last parameter.
	//
	//# \desc
	//# The $MouseEventHandler$ class encapsulates a procedure that is invoked when a
	//# mouse event occurs. Once an instance of the $MouseEventHandler$ class has
	//# been constructed, it can be installed by calling the $@Engine::InstallMouseEventHandler@$ function.
	//# 
	//# When a mouse event occurs, the procedures corresponding to all installed mouse event handlers are
	//# invoked. The $HandlerProc$ type is defined as follows.
	//
	//# \code	typedef bool HandlerProc(const MouseEventData *eventData, void *cookie);
	//
	//# The $eventType$ field of the $@Utilities/MouseEventData@$ structure specifies what type of mouse event
	//# occurred and can be one of the following values.
	//
	//# \value	kEventMouseDown						The left mouse button was pressed.
	//# \value	kEventMouseUp						The left mouse button was released.
	//# \value	kEventRightMouseDown				The right mouse button was pressed.
	//# \value	kEventRightMouseUp					The right mouse button was released.
	//# \value	kEventMiddleMouseDown				The middle mouse button was pressed.
	//# \value	kEventMiddleMouseUp					The middle mouse button was released.
	//# \value	kEventMouseMoved					The mouse location was moved.
	//# \value	kEventMouseWheel					The mouse wheel was moved.
	//# \value	kEventMultiaxisMouseTranslation		The translation rate changed for a multiaxis mouse device.
	//# \value	kEventMultiaxisMouseRotation		The rotation rate changed for a multiaxis mouse device.
	//# \value	kEventMultiaxisMouseButtonState		The button state changed for a multiaxis mouse device.
	//
	//# \desc
	//# For mouse down, mouse up, and mouse moved events, the $mousePosition$ field of the $@Utilities/MouseEventData@$ structure
	//# specifies the screen coordinates at which the mouse event occurred. For the $kEventMouseWheel$ event, the $y$ member of the
	//# $mousePosition$ field specifies how far the wheel was moved (positive or negative), and the $x$ member is undefined.
	//#
	//# For the $kEventMultiaxisMouseTranslation$ and $kEventMultiaxisMouseRotation$ events, the $mousePosition$ field contains the
	//# 3D translation or rotation rate information. For the $kEventMultiaxisMouseButtonState$ event, the $eventFlags$ field of the
	//# $@Utilities/MouseEventData@$ structure contains the state of all 32 possible buttons, with the lowest bit representing button 1
	//# and the highest bit representing button 32.
	//#
	//# The $cookie$ parameter is the value passed to the $MouseEventHandler$ constructor.
	//#
	//# The value returned by the handler specifies whether the mouse event was successfully handled. If the handler
	//# returns $true$, then the mouse event is considered handled, and no further mouse event handlers will be called
	//# for the same event. If the handler returns $false$, then the event is passed to the next mouse event handler.
	//# 
	//# A mouse event handler is uninstalled by destroying its associated class instance.
	//
	//# \base	Utilities/ListElement<MouseEventHandler>	Used internally to store all instances of $MouseEventHandler$ in a list.
	
	
	class C4_API MouseEventHandler : public ListElement<MouseEventHandler>
	{
		public:
			
			typedef bool HandlerProc(const MouseEventData *, void *);
		
		private:
			
			HandlerProc		*handlerProc;
			void			*handlerCookie;
		
		public:
			
			MouseEventHandler(HandlerProc *proc, void *cookie = nullptr);
			
			bool HandleEvent(const MouseEventData *eventData) const
			{
				return ((*handlerProc)(eventData, handlerCookie));
			}
	};
	
	
	//# \class	KeyboardEventHandler	Encapsulates a keyboard event handler function.
	//
	//# The $KeyboardEventHandler$ class encapsulates a keyboard event handler function.
	//
	//# \def	class KeyboardEventHandler : public ListElement<KeyboardEventHandler>
	//
	//# \ctor	KeyboardEventHandler(HandlerProc *proc, void *cookie = nullptr);
	//
	//# \param	proc	The procedure to invoke when a keyboard event occurs.
	//# \param	cookie	The cookie that is passed to the event handler as its last parameter.
	//
	//# \desc
	//# The $KeyboardEventHandler$ class encapsulates a procedure that is invoked when a
	//# keyboard event occurs. Once an instance of the $KeyboardEventHandler$ class has
	//# been constructed, it can be installed by calling the $@Engine::InstallKeyboardEventHandler@$ function.
	//# 
	//# When a keyboard event occurs, the procedures corresponding to all installed keyboard event handlers are
	//# invoked. The $HandlerProc$ type is defined as follows.
	//
	//# \code	typedef bool HandlerProc(const KeyboardEventData *eventData, void *cookie);
	//
	//# The $eventType$ field of the $@Utilities/KeyboardEventData@$ structure specifies what type of keyboard event occurred
	//# and can be one of the following values.
	//
	//# \value	kEventKeyDown		A key was pressed. If a key is held down long enough to trigger auto-repeat, then this
	//#								event is received each time a character is generated.
	//# \value	kEventKeyUp			A key was released.
	//# \value	kEventKeyCommand	A command key combination was pressed. This means the user held in the control key
	//#								(under Windows) or the command key (under Mac OS) while pressing another key.
	//
	//# \desc
	//# The $keyCode$ field of the $@Utilities/KeyboardEventData@$ structure specifies the Unicode value corresponding to
	//# the key that was involved in the event. The $modifierKeys$ field specifies which modifier keys were held down when
	//# the event occurred. It can be zero or a combination (through logical OR) of the following value.
	//
	//# \table	KeyboardModifiers
	//
	//# The $cookie$ parameter is the value passed to the $KeyboardEventHandler$ constructor.
	//#
	//# The value returned by the handler specifies whether the keyboard event was successfully handled. If the handler
	//# returns $true$, then the keybaord event is considered handled, and no further keyboard event handlers will be called
	//# for the same event. If the handler returns $false$, then the event is passed to the next keyboard event handler.
	//# 
	//# A keyboard event handler is uninstalled by destroying its associated class instance.
	//
	//# \base	Utilities/ListElement<KeyboardEventHandler>		Used internally to store all instances of $KeyboardEventHandler$ in a list.
	
	
	class C4_API KeyboardEventHandler : public ListElement<KeyboardEventHandler>
	{
		public:
			
			typedef bool HandlerProc(const KeyboardEventData *, void *);
		
		private:
			
			HandlerProc		*handlerProc;
			void			*handlerCookie;
		
		public:
			
			KeyboardEventHandler(HandlerProc *proc, void *cookie = nullptr);
			
			bool HandleEvent(const KeyboardEventData *eventData) const
			{
				return ((*handlerProc)(eventData, handlerCookie));
			}
	};
	
	
	//# \class	Engine		The primary engine manager object.
	//
	//# The $Engine$ class is the primary engine manager object.
	//
	//# \def	class Engine : public Manager<Engine>
	//
	//# \desc
	//# The $Engine$ class is the primary engine manager object. It controls all of the other manager objects,
	//# provides low-level system utilities, and manages system commands and variables. The single instance of
	//# the $Engine$ class is automatically constructed during an application's initialization and
	//# destroyed at termination.
	//# 
	//# The $Engine$ class's member functions are accessed through the global pointer $TheEngine$.
	
	
	//# \function	Engine::GetEngineFlags		Returns flags indicating the current state.
	//
	//# \proto	unsigned_int32 GetEngineFlags(void) const;
	//
	//# \desc
	//# The $GetEngineFlags$ function returns a combination (through logical OR) of the following values.
	//
	//# \table	EngineFlags
	//
	//# \also	$@Reporter@$
	//# \also	$@Engine::Report@$
	
	
	//# \function	Engine::GetCommand			Returns a system command.
	//
	//# \proto	Command *GetCommand(const char *name) const;
	//
	//# \param	name	The name of the command.
	//
	//# \desc
	//# The $GetCommand$ function returns the system command matching the name specified by the $name$ parameter.
	//# If no such command exists, then the return value is $nullptr$.
	//
	//# \also	$@Engine::GetFirstCommand@$
	//# \also	$@Engine::AddCommand@$
	//# \also	$@Command@$
	
	
	//# \function	Engine::GetFirstCommand		Returns the first system command.
	//
	//# \proto	const Command *GetFirstCommand(void) const;
	//
	//# \desc
	//# The $GetFirstCommand$ function returns the first system command as determined by lexicographical ordering
	//# on the command name. The $@Utilities/MapElement::Next@$ function can be used to iterate through all commands
	//# currently registered with the engine.
	//
	//# \also	$@Engine::GetCommand@$
	//# \also	$@Engine::AddCommand@$
	//# \also	$@Command@$
	
	
	//# \function	Engine::AddCommand			Registers a system command.
	//
	//# \proto	bool AddCommand(Command *command);
	//
	//# \param	command		A pointer to the command to register.
	//
	//# \desc
	//# The $AddCommand$ function registers the system command specified by the $command$ parameter with the engine.
	//# If the command is successfully registered, then the return value is $true$. If another command with the same
	//# name has already been registered, then the return value is $false$, and the new command is not registered.
	//
	//# \also	$@Engine::GetCommand@$
	//# \also	$@Engine::GetFirstCommand@$
	//# \also	$@Command@$
	
	
	//# \function	Engine::GetVariable			Returns a system variable.
	//
	//# \proto	Variable *GetVariable(const char *name) const;
	//
	//# \param	name	The name of the variable.
	//
	//# \desc
	//# The $GetVariable$ function returns the system variable matching the name specified by the $name$ parameter.
	//# If no such variable exists, then the return value is $nullptr$.
	//
	//# \also	$@Engine::GetFirstVariable@$
	//# \also	$@Engine::AddVariable@$
	//# \also	$@Variable@$
	
	
	//# \function	Engine::GetFirstVariable	Returns the first system variable.
	//
	//# \proto	const Variable *GetFirstVariable(void) const;
	//
	//# \desc
	//# The $GetFirstVariable$ function returns the first system variable as determined by lexicographical ordering
	//# on the variable name. The $@Utilities/MapElement::Next@$ function can be used to iterate through all variables
	//# currently registered with the engine.
	//
	//# \also	$@Engine::GetVariable@$
	//# \also	$@Engine::AddVariable@$
	//# \also	$@Variable@$
	
	
	//# \function	Engine::AddVariable			Registers a system variable.
	//
	//# \proto	bool AddVariable(Variable *variable);
	//
	//# \param	variable	A pointer to the variable to register.
	//
	//# \desc
	//# The $AddVariable$ function registers the system variable specified by the $variable$ parameter with the engine.
	//# If the variable is successfully registered, then the return value is $true$. If another variable with the same
	//# name has already been registered, then the return value is $false$, and the new variable is not registered.
	//
	//# \also	$@Engine::GetVariable@$
	//# \also	$@Engine::GetFirstVariable@$
	//# \also	$@Variable@$
	
	
	//# \function	Engine::WriteEngineConfig		Writes the persistent engine configuration to a file.
	//
	//# \proto	void WriteEngineConfig(const char *name = C4_ENGINE_CONFIG_FILE) const;
	//
	//# \param	name	The name of the file.
	//
	//# \desc
	//# The $WriteEngineConfig$ function writes the values of all system variables to a file whose name is specified
	//# by the $name$ parameter. The file is written to a subfolder of the roaming application support directory for
	//# the current user, as defined by the operating system. The subfolder's name is that of the running application,
	//# defined by the $APPLICATION_NAME$ identifier.
	//
	//# \also	$@Engine::GetVariable@$
	//# \also	$@Engine::AddVariable@$
	//# \also	$@Variable@$
	//
	//# \wiki	File_Locations		File Locations
	
	
	//# \function	Engine::InstallReporter		Installs a report handler.
	//
	//# \proto	static void InstallReporter(Reporter *reporter);
	//
	//# \param	reporter	The reporter to install.
	//
	//# \desc
	//# The $InstallReporter$ function installs a reporter whose associated report procedure
	//# is invoked whenever the $@Engine::Report@$ function is called. Multiple reporters may
	//# be installed simultaneously, and each corresponding report procedure is called when a
	//# message is reported to the engine.
	//# 
	//# A reporter is uninstalled by destroying its associated class instance.
	//
	//# \also	$@Reporter@$
	//# \also	$@Engine::Report@$
	
	
	//# \function	Engine::Report		Reports a text message to the engine.
	//
	//# \proto	static void Report(const char *text, unsigned_int32 flags = 0);
	//
	//# \param	text	The text message to report.
	//# \param	flags	Flags pertaining to the message.
	//
	//# \desc
	//# The $Report$ function reports the text message pointed to by the $text$ parameter to the
	//# engine. This has the effect of invoking the report procedures for any instances of the
	//# $@Reporter@$ class that have been installed using the $@Engine::InstallReporter@$ function.
	//# 
	//# The $flags$ parameter specifies options that may be considered by any of the installed
	//# reporters. This parameter can be a combination (through logical OR) of the following values.
	//
	//# \table	ReportFlags
	//
	//# The engine installs its own reporter that ignores all messages except those that have the
	//# $kReportLog$ flag set. The console window installs a reporter that displays all messages
	//# that do <i>not</i> have the $kReportLog$ flag set.
	//
	//# \also	$@Reporter@$
	//# \also	$@Engine::InstallReporter@$
	
	
	//# \function	Engine::InstallMouseEventHandler		Installs a mouse event handler.
	//
	//# \proto	void InstallMouseEventHandler(MouseEventHandler *handler);
	//
	//# \param	handler		The event handler to install.
	//
	//# \desc
	//# The $InstallMouseEventHandler$ function installs a mouse event handler that is invoked whenever
	//# a mouse event occurs. Multiple mouse event handlers may be installed simultaneously, and each
	//# one is invoked when an event occurs.
	//# 
	//# A mouse event handler is uninstalled by destroying its associated class instance.
	//
	//# \also	$@MouseEventHandler@$
	//# \also	$@KeyboardEventHandler@$
	//# \also	$@Engine::InstallKeyboardEventHandler@$
	
	
	//# \function	Engine::InstallKeyboardEventHandler		Installs a keyboard event handler.
	//
	//# \proto	void InstallKeyboardEventHandler(KeyboardEventHandler *handler);
	//
	//# \param	handler		The event handler to install.
	//
	//# \desc
	//# The $InstallKeyboardEventHandler$ function installs a keyboard event handler that is invoked whenever
	//# a keyboard event occurs. If multiple keyboard event handlers are installed simultaneously, they are
	//# called in reverse order of installation. If any handler reports that it has successfully handled the
	//# keyboard event, then the remaining handlers are not called.
	//# 
	//# A keybaord event handler is uninstalled by destroying its associated class instance.
	//
	//# \also	$@KeyboardEventHandler@$
	//# \also	$@MouseEventHandler@$
	//# \also	$@Engine::InstallMouseEventHandler@$
	
	
	//# \function	Engine::Quit			Causes the engine to quit.
	//
	//# \proto	void Quit(void);
	//
	//# \desc
	//# The $Quit$ function causes the engine to quit at the end of the current game loop.
	
	
	//# \div
	//# \function	Engine::OpenExternalWebBrowser		Launches the default web browser and navigates to a URL.
	//
	//# \proto	bool OpenExternalWebBrowser(const char *url);
	//
	//# \param	url		The URL to which the web browser should navigate.
	//
	//# \desc
	//# The $OpenExternalWebBrowser$ function launches the default web browser on the local machine and navigates to
	//# the URL specified by the $url$ parameter. Upon success, this function returns $true$. If an error is reported
	//# by the operating system, then the return value is $false$.
	
	
	class C4_API Engine : public Manager<Engine>
	{
		friend class MouseDevice;		
		
	public:
		static LRESULT CALLBACK WindowProc(HWND window, UINT message, WPARAM wparam, LPARAM lparam);
		private:
			
			unsigned_int32					engineFlags;
			
			int32							processorCount;
			unsigned_int32					processorFlags;
			
			#if !C4GAMECONSOLE
			
				int32						backgroundSleepTime;
			
			#endif
			
			const char						*applicationName;
			ApplicationModule				*applicationModule;
			
			Map<Command>					commandMap;
			Map<Variable>					variableMap;
			
			File							logFile;
			Reporter						logger;
			
			static List<Reporter>		reporterList;
			static List<Persistor>	persistorList;
			
			List<MouseEventHandler>			mouseEventHandlerList;
			List<KeyboardEventHandler>		keyboardEventHandlerList;
			
			static unsigned_int32			mouseButtonMask;
			static Point3D					lastMousePosition;
			
			int32							multiaxisMouseActiveCount;
			int32							tabletActiveCount;
			float							stylusPressure;
			
			#if C4WINDOWS
				
				unsigned_int32				stylusMinPressure;
				float						stylusPressureScale;
				
				WNDCLASSEXW					windowClass;
				HINSTANCE					engineInstance;
				HWND						engineWindow;
				
				HMODULE						tabletLibrary;
				HCTX						tabletContext;
				
				bool						deadKeyFlag;
				
				
				int32						wheelDeltaAccum;
			
				
	public:
				bool GetDeadKeyFlag() const { return deadKeyFlag; }
				void SetDeadKeyFlag(bool val) { deadKeyFlag = val; }
				bool HandleApplicationEvent(UINT message, WPARAM wparam, LPARAM lparam);
				bool HandleWindowEvent(UINT message, WPARAM wparam, LPARAM lparam);
				void HandleMouseEvent(HWND window, UINT message, WPARAM wparam, LPARAM lparam);
				void HandleKeyboardEvent(HWND window, UINT message, WPARAM wparam, LPARAM lparam);
				void HandleMultiaxisMouseEvent(WPARAM wparam, LPARAM lparam);
				void HandleTabletEvent(UINT message, WPARAM wparam, LPARAM lparam);
	private:	
				static DWORD WINAPI ShellThread(void *cookie);
			
			#endif
			
			VariableObserver<Engine>		gameModuleObserver;
			
			CommandObserver<Engine>			quitCommandObserver;
			CommandObserver<Engine>			wireCommandObserver;
			CommandObserver<Engine>			normCommandObserver;
			CommandObserver<Engine>			tangCommandObserver;
			CommandObserver<Engine>			shadCommandObserver;
			CommandObserver<Engine>			sbndCommandObserver;
			CommandObserver<Engine>			lrgnCommandObserver;
			CommandObserver<Engine>			srgnCommandObserver;
			CommandObserver<Engine>			doffCommandObserver;
			CommandObserver<Engine>			spthCommandObserver;
			CommandObserver<Engine>			bodyCommandObserver;
			CommandObserver<Engine>			ctacCommandObserver;
			CommandObserver<Engine>			rateCommandObserver;
			CommandObserver<Engine>			statCommandObserver;
			CommandObserver<Engine>			smapCommandObserver;
			CommandObserver<Engine>			netCommandObserver;
			CommandObserver<Engine>			extCommandObserver;
			CommandObserver<Engine>			rsrcCommandObserver;
			CommandObserver<Engine>			heapCommandObserver;
			CommandObserver<Engine>			dumpCommandObserver;
			CommandObserver<Engine>			visitCommandObserver;
			CommandObserver<Engine>			shotCommandObserver;
			CommandObserver<Engine>			undefCommandObserver;
			CommandObserver<Engine>			bindCommandObserver;
			CommandObserver<Engine>			unbindCommandObserver;
			CommandObserver<Engine>			sayCommandObserver;
			CommandObserver<Engine>			addressCommandObserver;
			CommandObserver<Engine>			resolveCommandObserver;
			CommandObserver<Engine>			disconnectCommandObserver;
			CommandObserver<Engine>			execCommandObserver;
			CommandObserver<Engine>			importCommandObserver;
			CommandObserver<Engine>			cmdCommandObserver;
			CommandObserver<Engine>			varCommandObserver;
			CommandObserver<Engine>			loadCommandObserver;
			CommandObserver<Engine>			unloadCommandObserver;
			
			void InitializeProcessorData(void);
			
			EngineResult ConstructManagers(const char *commandLine);
			void DestroyManagers(void);
			
			void BeginLog(void);
			void EndLog(void);
			
			EngineResult LoadApplicationModule(void);
			void UnloadApplicationModule(void);
			
			void HandleGameModuleEvent(Variable *variable);
			static void ChangeGameModule(DeferredTask *task, void *cookie);
			
			static void Logger(const char *text, unsigned_int32 flags, void *cookie);
			
			static bool PluginFilter(const char *name, unsigned_int32 flags, void *cookie);
			
			static void ResetMouseButtonMask(void);
			
			static void ResolverComplete(DomainNameResolver *resolver, void *cookie);
		
		public:
			
			Engine(int);
			~Engine();
			
			EngineResult Construct(void);
			void Destruct(void);
			
			unsigned_int32 GetEngineFlags(void) const
			{
				return (engineFlags);
			}
			
			int32 GetProcessorCount(void) const
			{
				return (processorCount);
			}
			
			unsigned_int32 GetProcessorFlags(void) const
			{
				return (processorFlags);
			}
			
			const char *GetApplicationName(void) const
			{
				return (applicationName);
			}
			
			const Command *GetFirstCommand(void) const
			{
				return (commandMap.First());
			}
			
			Command *GetCommand(const char *name) const
			{
				return (commandMap.Find(name));
			}
			
			bool AddCommand(Command *command)
			{
				return (commandMap.Insert(command));
			}
			
			const Variable *GetFirstVariable(void) const
			{
				return (variableMap.First());
			}
			
			Variable *GetVariable(const char *name) const
			{
				return (variableMap.Find(name));
			}
			
			bool AddVariable(Variable *variable)
			{
				return (variableMap.Insert(variable));
			}
			
			static void InstallReporter(Reporter *reporter)
			{
				reporterList.Append(reporter);
			}
			
			static void InstallPersistor(Persistor *persistor)
			{
				persistorList.Append(persistor);
			}
			
			const MouseEventHandler *GetFirstMouseEventHandler(void) const
			{
				return (mouseEventHandlerList.First());
			}
			
			const KeyboardEventHandler *GetFirstKeyboardEventHandler(void) const
			{
				return (keyboardEventHandlerList.First());
			}
			
			void InstallMouseEventHandler(MouseEventHandler *handler)
			{
				mouseEventHandlerList.Prepend(handler);
			}
			
			void InstallKeyboardEventHandler(KeyboardEventHandler *handler)
			{
				keyboardEventHandlerList.Prepend(handler);
			}
			
			float GetStylusPressure(void)
			{
				return (stylusPressure);
			}
			
			#if C4WINDOWS
			
				HINSTANCE GetInstance(void) const
				{
					return (engineInstance);
				}
				
				HWND GetWindow(void) const
				{
					return (engineWindow);
				}
				
				EngineResult InitializeSDI(const char *name, HINSTANCE instance, HWND hWnd, const char *commandLine);

				EngineResult Initialize(const char *name, HINSTANCE instance, const char *commandLine);
				void Terminate(void);
				
			#elif C4MACOS
			
				unsigned_int32 GetSystemVersion(void) const
				{
					return (systemVersion);
				}
				
				CFBundleRef GetOpenGLBundle(void) const
				{
					return (openglBundle);
				}
				
				EngineResult Initialize(const char *name, const char *commandLine);
				void Terminate(void);
				
				static void *GetBundleFunctionAddress(CFBundleRef bundle, const char *name);
			
			#elif C4LINUX
			
				::Display *GetEngineDisplay(void) const
				{
					return (engineDisplay);
				}
				
				::Window GetEngineWindow(void) const
				{
					return (engineWindow);
				}
				
				EngineResult Initialize(const char *name, const char *commandLine);
				void Terminate(void);
			
			#elif C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

			#endif //]
			
			void GetPluginList(const char *directory, List<FileReference> *fileList) const;
			
			Variable *InitVariable(const char *name, const char *value, unsigned_int32 flags = 0, Variable::ObserverType *observer = nullptr);
			
			void ExecuteCommand(const char *text);
			void ExecuteText(const char *text);
			bool ExecuteFile(const char *name, ResourceCatalog *catalog = nullptr);
			
			void WriteEngineConfig(const char *name = C4_ENGINE_CONFIG_FILE) const;
			void WriteInputConfig(const char *name = C4_INPUT_CONFIG_FILE) const;
			static void WriteConfigString(File &file, const char *string);
			
			static void Report(const char *text, unsigned_int32 flags = 0);
			static void LogResult(EngineResult result);
			static void LogSource(const char *source);
			
			void Run(void);
			void Quit(void);
			
			void StartMultiaxisMouse(void);
			void StopMultiaxisMouse(void);
			
			void StartTablet(void);
			void StopTablet(void);
			
			bool OpenExternalWebBrowser(const char *url);
			
			#if C4LEAK_DETECTION
			
				static void DumpMemory(const char *filename);
			
			#endif
			
			void HandleQuitCommand(Command *command, const char *text);
			void HandleWireCommand(Command *command, const char *text);
			void HandleNormCommand(Command *command, const char *text);
			void HandleTangCommand(Command *command, const char *text);
			void HandleShadCommand(Command *command, const char *text);
			void HandleSbndCommand(Command *command, const char *text);
			void HandleLrgnCommand(Command *command, const char *text);
			void HandleSrgnCommand(Command *command, const char *text);
			void HandleDoffCommand(Command *command, const char *text);
			void HandleSpthCommand(Command *command, const char *text);
			void HandleBodyCommand(Command *command, const char *text);
			void HandleCtacCommand(Command *command, const char *text);
			void HandleRateCommand(Command *command, const char *text);
			void HandleStatCommand(Command *command, const char *text);
			void HandleSmapCommand(Command *command, const char *text);
			void HandleNetCommand(Command *command, const char *text);
			void HandleExtCommand(Command *command, const char *text);
			void HandleRsrcCommand(Command *command, const char *text);
			void HandleHeapCommand(Command *command, const char *text);
			void HandleDumpCommand(Command *command, const char *text);
			void HandleVisitCommand(Command *command, const char *text);
			void HandleShotCommand(Command *command, const char *text);
			void HandleUndefCommand(Command *command, const char *text);
			void HandleBindCommand(Command *command, const char *text);
			void HandleUnbindCommand(Command *command, const char *text);
			void HandleSayCommand(Command *command, const char *text);
			void HandleAddressCommand(Command *command, const char *text);
			void HandleResolveCommand(Command *command, const char *text);
			void HandleDisconnectCommand(Command *command, const char *text);
			void HandleExecCommand(Command *command, const char *text);
			void HandleImportCommand(Command *command, const char *text);
			void HandleCmdCommand(Command *command, const char *text);
			void HandleVarCommand(Command *command, const char *text);
			void HandleLoadCommand(Command *command, const char *text);
			void HandleUnloadCommand(Command *command, const char *text);
	};
	
	
	class C4_API Module
	{
		private:
			
			ModuleType			moduleType;
			bool				moduleLoaded;
			
			#if C4WINDOWS
			
				HMODULE			moduleHandle;
			
			#elif C4POSIX
			
				void			*moduleHandle;
			
			#endif
		
		protected:
			
			Module(ModuleType type);
			
			#if C4WINDOWS
			
				HMODULE GetModuleHandle(void) const
				{
					return (moduleHandle);
				}
				
				void *GetFunctionAddress(const char *name) const
				{
					return (GetProcAddress(moduleHandle, name));
				}
			
			#elif C4POSIX
			
				void *GetModuleHandle(void) const
				{
					return (moduleHandle);
				}
				
				void *GetFunctionAddress(const char *name) const
				{
					return (dlsym(moduleHandle, name));
				}
			
			#endif
		
		public:
			
			virtual ~Module();
			
			virtual EngineResult Load(const char *name);
			void Unload(void);
	};
	
	
	class C4_API ApplicationModule : public Module
	{
		private:
			
			typedef Application *ConstructProc(void);
		
		public:
			
			ApplicationModule();
			~ApplicationModule();
			
			EngineResult Load(const char *name);
	};
	
	
	C4_API extern Engine *TheEngine;
}


#endif

// ZYURVUR
