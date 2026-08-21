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


#ifndef C4BrowserPlugin_h
#define C4BrowserPlugin_h


//# \component	Browser Plugin
//# \prefix		BrowserPlugin/


#include "C4Plugins.h"
#include "C4Panels.h"
#include "C4Image.h"
#include "C4Awesomium.h"


#ifdef C4BROWSER

	#define C4BROWSERAPI C4MODULEEXPORT
	
	
	extern "C"
	{
		C4MODULEEXPORT C4::Plugin *ConstructPlugin(void);
	}

#else

	#define C4BROWSERAPI C4MODULEIMPORT

#endif


namespace C4
{
	typedef EngineResult BrowserResult;
	
	
	enum
	{
		kManagerBrowser			= 'BR',
		
		kBrowserOkay			= kEngineOkay,
		kBrowserInitFailed		= (kManagerBrowser << 16) | 0x0001
	};
	
	
	enum
	{
		kWidgetBrowser			= 'brow'
	};
	
	
	//# \enum	BrowserFlags
	
	enum
	{
		kBrowserTransparentBackground	= 1 << 0		//## Enables a transparent background if specified by the style of the web page being displayed.
	};
	
	
	enum
	{
		kMaxBrowserAddressLength	= kMaxWidgetFunctionMessageSize - 1
	};
	
	
	typedef String<kMaxBrowserAddressLength> BrowserAddressString;
	
	
	enum
	{
		kFunctionNavigateBrowserWidgetBack		= 'nvbk',
		kFunctionNavigateBrowserWidgetForward	= 'nvfd',
		kFunctionNavigateBrowserWidgetAddress	= 'nvad',
		kFunctionNavigateBrowserWidgetText		= 'nvtx',
		kFunctionReloadBrowserWidget			= 'rlbr',
		kFunctionStopBrowserWidget				= 'stbr'
	};
	
	
	//# \class	BrowserWidget		The interface widget that displays a web browser.
	//
	//# The $BrowserWidget$ class represents an interface widget that displays a web browser.
	//
	//# \def	class BrowserWidget : public ImageWidget, public ListElement<BrowserWidget>
	//
	//# \ctor	BrowserWidget(const Vector2D& size, int32 width, int32 height);
	//
	//# \param	size		The size of the quad to which the browser is scaled.
	//# \param	width		The internal widget of the web browser texture map, in pixels.
	//# \param	height		The internal height of the web browser texture map, in pixels.
	//
	//# \desc
	//# The $BrowserWidget$ class displays a web browser in a widget. Web pages are rendered into a
	//# texture map whose dimensions are specified by the $width$ and $height$ parameters. As far as
	//# the web browser is concerned, this is the size of the virtual window into which it can render.
	//# The texture map is then scaled to the physical size of the widget, specified by the $size$
	//# parameter, when it is rendered in a user interface panel.
	//
	//# \base	InterfaceMgr/ImageWidget				A $BrowserWidget$ is a specialized image widget.
	//# \base	Utilities/ListElement<BrowserWidget>	Used internally by the Browser Plugin.
	//
	//# \wiki	Browser_Plugin		Browser Plugin 
	
	 
	class BrowserWidget : public ImageWidget, public ListElement<BrowserWidget> 
	{ 
		friend class WidgetReg<BrowserWidget>;
		friend class BrowserPlugin; 
		
		private:
			
			awe_webview				*webView; 
			
			int32					browserWidth;
			int32					browserHeight;
			 
			WidgetKey				textWidgetKey;
			BrowserAddressString	homePageAddress;
			
			TextureHeader			browserTexture;
			
			BrowserWidget();
			BrowserWidget(const BrowserWidget& browserWidget);
			
			Widget *Replicate(void) const;
		
		public:
			
			BrowserWidget(const Vector2D& size, int32 width, int32 height);
			~BrowserWidget();
			
			using ListElement<BrowserWidget>::Previous;
			using ListElement<BrowserWidget>::Next;
			
			int32 GetBrowserWidth(void) const
			{
				return (browserWidth);
			}
			
			int32 GetBrowserHeight(void) const
			{
				return (browserHeight);
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			void *BeginSettingsUnpack(void);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			void Preprocess(void);
			
			void SetWidgetState(unsigned_int32 state);
			
			void Build(void);
			void Render(List<Renderable> *renderList);
			
			void HandleMouseEvent(const PanelMouseEventData *eventData);
			bool HandleKeyboardEvent(const KeyboardEventData *eventData);
			
			void UpdateAddress(const char *address);
			
			void NavigateBack(void);
			void NavigateForward(void);
			void NavigateAddress(const char *address);
			
			void Reload(void);
			void Stop(void);
	};
	
	
	class NavigateBrowserWidgetBackFunction : public WidgetFunction
	{
		private:
			
			NavigateBrowserWidgetBackFunction(const NavigateBrowserWidgetBackFunction& navigateBrowserWidgetBackFunction);
			
			Function *Replicate(void) const;
		
		public:
			
			NavigateBrowserWidgetBackFunction();
			~NavigateBrowserWidgetBackFunction();
			
			void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};
	
	
	class NavigateBrowserWidgetForwardFunction : public WidgetFunction
	{
		private:
			
			NavigateBrowserWidgetForwardFunction(const NavigateBrowserWidgetForwardFunction& navigateBrowserWidgetForwardFunction);
			
			Function *Replicate(void) const;
		
		public:
			
			NavigateBrowserWidgetForwardFunction();
			~NavigateBrowserWidgetForwardFunction();
			
			void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};
	
	
	class NavigateBrowserWidgetAddressFunction : public WidgetFunction
	{
		private:
			
			BrowserAddressString	browserAddress;
			
			NavigateBrowserWidgetAddressFunction(const NavigateBrowserWidgetAddressFunction& navigateBrowserWidgetAddressFunction);
			
			Function *Replicate(void) const;
		
		public:
			
			NavigateBrowserWidgetAddressFunction();
			~NavigateBrowserWidgetAddressFunction();
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			void Compress(Compressor& data) const;
			bool Decompress(Decompressor& data);
			
			bool OverridesFunction(const Function *function) const;
			
			void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};
	
	
	class NavigateBrowserWidgetTextFunction : public WidgetFunction
	{
		private:
			
			WidgetKey		textWidgetKey;
			
			NavigateBrowserWidgetTextFunction(const NavigateBrowserWidgetTextFunction& navigateBrowserWidgetTextFunction);
			
			Function *Replicate(void) const;
		
		public:
			
			NavigateBrowserWidgetTextFunction();
			~NavigateBrowserWidgetTextFunction();
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			void Compress(Compressor& data) const;
			bool Decompress(Decompressor& data);
			
			bool OverridesFunction(const Function *function) const;
			
			void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};
	
	
	class ReloadBrowserWidgetFunction : public WidgetFunction
	{
		private:
			
			ReloadBrowserWidgetFunction(const ReloadBrowserWidgetFunction& reloadBrowserWidgetFunction);
			
			Function *Replicate(void) const;
		
		public:
			
			ReloadBrowserWidgetFunction();
			~ReloadBrowserWidgetFunction();
			
			void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};
	
	
	class StopBrowserWidgetFunction : public WidgetFunction
	{
		private:
			
			StopBrowserWidgetFunction(const StopBrowserWidgetFunction& stopBrowserWidgetFunction);
			
			Function *Replicate(void) const;
		
		public:
			
			StopBrowserWidgetFunction();
			~StopBrowserWidgetFunction();
			
			void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};
	
	
	class BrowserPlugin : public Plugin, public Singleton<BrowserPlugin>
	{
		friend class BrowserWidget;
		
		private:
			
			bool							awesomiumActive;
			List<BrowserWidget>				browserList;
			
			StringTable						stringTable;
			
			WidgetReg<BrowserWidget>		browserWidgetReg;
			
			awe_webview *NewWebView(BrowserWidget *widget, int32 browserWidth, int32 browserHeight, unsigned_int32 flags = 0);
			void ReleaseWebView(BrowserWidget *widget, awe_webview *webView);
			
			static void NavigationCallback(awe_webview *webView, const awe_string *url, const awe_string *frameName);
		
		public:
			
			BrowserPlugin();
			~BrowserPlugin();
			
			const StringTable *GetStringTable(void) const
			{
				return (&stringTable);
			}
			
			C4BROWSERAPI BrowserResult Initialize(void);
			C4BROWSERAPI void Terminate(void);
			
			void PluginTask(void);
	};
	
	
	C4BROWSERAPI extern BrowserPlugin *TheBrowserPlugin;
}


#endif

// ZYURVUR
