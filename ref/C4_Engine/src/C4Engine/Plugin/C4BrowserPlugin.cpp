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


#include "C4BrowserPlugin.h"
#include "C4Configuration.h"


using namespace C4;


BrowserPlugin *C4::TheBrowserPlugin = nullptr;


C4::Plugin *ConstructPlugin(void)
{
	return (new BrowserPlugin);
}


BrowserWidget::BrowserWidget() : ImageWidget(kWidgetBrowser)
{
	webView = nullptr;
	
	browserWidth = 640;
	browserHeight = 480;
	
	textWidgetKey[0] = 0;
	homePageAddress[0] = 0;
	
	SetWidgetUsage(kWidgetKeyboardFocus | kWidgetMouseWheel | kWidgetGeneratedImage);
	SetImageBlendState(kBlendReplace);
}

BrowserWidget::BrowserWidget(const Vector2D& size, int32 width, int32 height) : ImageWidget(kWidgetBrowser, size)
{
	webView = nullptr;
	
	browserWidth = width;
	browserHeight = height;
	
	textWidgetKey[0] = 0;
	homePageAddress[0] = 0;
	
	SetWidgetUsage(kWidgetKeyboardFocus | kWidgetMouseWheel | kWidgetGeneratedImage);
	SetImageBlendState(kBlendReplace);
}

BrowserWidget::BrowserWidget(const BrowserWidget& browserWidget) : ImageWidget(browserWidget)
{
	webView = nullptr;
	
	browserWidth = browserWidget.browserWidth;
	browserHeight = browserWidget.browserHeight;
	
	textWidgetKey = browserWidget.textWidgetKey;
	homePageAddress = browserWidget.homePageAddress;
}

BrowserWidget::~BrowserWidget()
{
	if (webView) TheBrowserPlugin->ReleaseWebView(this, webView);
}

Widget *BrowserWidget::Replicate(void) const
{
	return (new BrowserWidget(*this));
}

void BrowserWidget::Pack(Packer& data, unsigned_int32 packFlags) const
{
	ImageWidget::Pack(data, packFlags);
	
	data << ChunkHeader('SIZE', 8);
	data << browserWidth;
	data << browserHeight;
	
	if (textWidgetKey[0] != 0)
	{
		PackHandle handle = data.BeginChunk('TWKY');
		data << textWidgetKey;
		data.EndChunk(handle);
	}
	
	if (homePageAddress[0] != 0)
	{
		PackHandle handle = data.BeginChunk('HOME');
		data << homePageAddress;
		data.EndChunk(handle);
	}
	
	data << TerminatorChunk;
}

void BrowserWidget::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	ImageWidget::Unpack(data, unpackFlags);
	UnpackChunkList<BrowserWidget>(data, unpackFlags);
}

bool BrowserWidget::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'SIZE':
			 
			data >> browserWidth;
			data >> browserHeight; 
			return (true); 
		 
		case 'TWKY':
			 
			data >> textWidgetKey;
			return (true);
		
		case 'HOME': 
			
			data >> homePageAddress;
			return (true);
	} 
	
	return (false);
}

void *BrowserWidget::BeginSettingsUnpack(void)
{
	textWidgetKey[0] = 0;
	homePageAddress[0] = 0;
	
	return (ImageWidget::BeginSettingsUnpack());
}

int32 BrowserWidget::GetSettingCount(void) const
{
	return (Widget::GetSettingCount() + 6);
}

Setting *BrowserWidget::GetSetting(int32 index) const
{
	int32 count = Widget::GetSettingCount();
	if (index < count) return (Widget::GetSetting(index));
	
	const StringTable *table = TheBrowserPlugin->GetStringTable();
	
	if (index == count)
	{
		const char *title = table->GetString(StringID('WDGT', kWidgetBrowser, 'SETT'));
		return (new HeadingSetting(kWidgetBrowser, title));
	}
	
	if (index == count + 1)
	{
		const char *title = table->GetString(StringID('WDGT', kWidgetBrowser, 'WIDE'));
		return (new TextSetting('BRWX', Text::IntegerToString(browserWidth), title, 4, &EditTextWidget::NumberFilter));
	}
	
	if (index == count + 2)
	{
		const char *title = table->GetString(StringID('WDGT', kWidgetBrowser, 'HIGH'));
		return (new TextSetting('BRWY', Text::IntegerToString(browserHeight), title, 4, &EditTextWidget::NumberFilter));
	}
	
	if (index == count + 3)
	{
		const char *title = table->GetString(StringID('WDGT', kWidgetBrowser, 'ALFA'));
		return (new BooleanSetting('BRBL', (GetImageBlendState() != kBlendReplace), title));
	}
	
	if (index == count + 4)
	{
		const char *title = table->GetString(StringID('WDGT', kWidgetBrowser, 'HOME'));
		return (new TextSetting('HOME', homePageAddress, title, kMaxBrowserAddressLength));
	}
	
	if (index == count + 5)
	{
		const char *title = table->GetString(StringID('WDGT', kWidgetBrowser, 'TWKY'));
		return (new TextSetting('TWKY', textWidgetKey, title, kMaxWidgetKeyLength));
	}
	
	return (nullptr);
}

void BrowserWidget::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'BRWX')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		browserWidth = Min(Max(Text::StringToInteger(text), 32), 1600);
	}
	else if (identifier == 'BRWY')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		browserHeight = Min(Max(Text::StringToInteger(text), 32), 1600);
	}
	else if (identifier == 'BRBL')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		unsigned_int32 blendState = (b) ? kBlendInterpolate : kBlendReplace;
		SetImageBlendState(blendState);
		SetAmbientBlendState(blendState);
	}
	else if (identifier == 'HOME')
	{
		homePageAddress = static_cast<const TextSetting *>(setting)->GetText();
	}
	else if (identifier == 'TWKY')
	{
		textWidgetKey = static_cast<const TextSetting *>(setting)->GetText();
	}
	else
	{
		Widget::SetSetting(setting);
	}
}

void BrowserWidget::Preprocess(void)
{
	ImageWidget::Preprocess();
	
	if (!GetManipulator())
	{
		const PanelController *controller = GetPanelController();
		if ((!controller) || (!controller->GetTargetNode()->GetManipulator()))
		{
			if (TheBrowserPlugin->Initialize() == kBrowserOkay)
			{
				if (webView) awe_webview_destroy(webView);
				webView = TheBrowserPlugin->NewWebView(this, browserWidth, browserHeight, (GetImageBlendState() != kBlendReplace) ? kBrowserTransparentBackground : 0);
				
				browserTexture.textureType = kTextureRectangle;
				browserTexture.textureFlags = kTextureExternalStorage;
				browserTexture.colorSemantic = kTextureSemanticNone;
				browserTexture.alphaSemantic = kTextureSemanticTransparency;
				browserTexture.imageFormat = kTextureBGRA8;
				browserTexture.imageWidth = browserWidth;
				browserTexture.imageHeight = browserHeight;
				browserTexture.imageDepth = 1;
				browserTexture.wrapMode[0] = kTextureClamp;
				browserTexture.wrapMode[1] = kTextureClamp;
				browserTexture.wrapMode[2] = kTextureClamp;
				browserTexture.mipmapCount = 1;
				browserTexture.mipmapDataOffset = 0;
				browserTexture.auxiliaryDataSize = 0;
				browserTexture.auxiliaryDataOffset = 0;
				SetTexture(0, &browserTexture);
				
				if (homePageAddress[0] != 0) NavigateAddress(homePageAddress);
			}
		}
	}
	
	if (!webView) SetTexture(0, "C4/checker");
}

void BrowserWidget::SetWidgetState(unsigned_int32 state)
{
	if (((GetWidgetState() ^ state) & kWidgetFocus) && (webView))
	{
		if (state & kWidgetFocus) awe_webview_focus(webView);
		else awe_webview_unfocus(webView);
	}
	
	ImageWidget::SetWidgetState(state);
}

void BrowserWidget::Build(void)
{
	if (webView)
	{
		SetImageOffset(Vector2D(0.0F, 1.0F));
		SetImageScale(Vector2D(1.0F, -1.0F));
	}
	else
	{
		const Vector2D& size = GetWidgetSize();
		SetImageScale(Vector2D(size.x * 0.03125F, size.y * 0.03125F));
	}
	
	ImageWidget::Build();
}

void BrowserWidget::Render(List<Renderable> *renderList)
{
	if ((webView) && (awe_webview_is_dirty(webView)))
	{
		awe_rect rect = awe_webview_get_dirty_bounds(webView);
		int32 left = rect.x;
		int32 top = rect.y;
		int32 right = left + rect.width;
		int32 bottom = top + rect.height;
		
		const awe_renderbuffer *renderBuffer = awe_webview_render(webView);
		int32 pitch = awe_renderbuffer_get_rowspan(renderBuffer) / sizeof(Color4C);
		const Color4C *image = reinterpret_cast<const Color4C *>(awe_renderbuffer_get_buffer(renderBuffer));
		
		GetTexture()->Update(Rect(left, top, right, bottom), pitch, image);
	}
	
	ImageWidget::Render(renderList);
}

void BrowserWidget::HandleMouseEvent(const PanelMouseEventData *eventData)
{
	if (webView)
	{
		EventType eventType = eventData->eventType;
		if ((eventType == kEventMouseDown) || (eventType == kEventMouseUp) || (eventType == kEventMouseMoved))
		{
			const Vector2D& size = GetWidgetSize();
			int32 x = (int32) ((float) browserWidth * eventData->mousePosition.x / size.x);
			int32 y = (int32) ((float) browserHeight * eventData->mousePosition.y / size.y);
			
			awe_webview_inject_mouse_move(webView, x, y);
			
			if (eventType == kEventMouseDown)
			{
				PanelController *controller = GetPanelController();
				if (controller) controller->BeginKeyboardInteraction(this);
				
				awe_webview_inject_mouse_down(webView, AWE_MB_LEFT);
			}
			else if (eventType == kEventMouseUp)
			{
				awe_webview_inject_mouse_up(webView, AWE_MB_LEFT);
			}
		}
		else if (eventType == kEventMouseWheel)
		{
			awe_webview_inject_mouse_wheel(webView, (int32) eventData->mousePosition.y * 16, 0);
		}
	}
}

bool BrowserWidget::HandleKeyboardEvent(const KeyboardEventData *eventData)
{
	if (webView)
	{
		EventType eventType = eventData->eventType;
		if ((eventType == kEventKeyDown) || (eventType == kEventKeyUp))
		{
			static const unsigned_int8 virtualCode[32] =
			{
				0x00, 0x24, 0x00, 0x00, 0x23, 0x00, 0x00, 0x00, 0x08, 0x09, 0x00, 0x21, 0x22, 0x00, 0x00, 0x00,
				0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1B, 0x25, 0x27, 0x26, 0x28
			};
			
			awe_webkeyboardevent	awe_event;
			
			awe_event.modifiers = (eventData->modifierKeys & kModifierKeyShift) ? AWE_WKM_SHIFT_KEY : 0;
			awe_event.native_key_code = 0;
			awe_event.is_system_key = false;
			
			unsigned_int32 code = eventData->keyCode;
			if ((code < 32U) || (code == 127U))
			{
				if (code < 32U) code = virtualCode[code];
				else code = 0x2E;
				
				if (code != 0)
				{
					awe_event.type = (eventType == kEventKeyDown) ? AWE_WKT_KEYDOWN : AWE_WKT_KEYUP;
					awe_event.virtual_key_code = code;
					awe_event.text[0] = 0;
					awe_event.unmodified_text[0] = 0;
					
					awe_webview_inject_keyboard_event(webView, awe_event);
					return (true);
				}
			}
			else
			{
				if (eventType == kEventKeyDown)
				{
					awe_event.type = AWE_WKT_CHAR;
					awe_event.virtual_key_code = 0;
					awe_event.text[0] = code;
					awe_event.text[1] = 0;
					awe_event.unmodified_text[0] = code;
					awe_event.unmodified_text[1] = 0;
					
					awe_webview_inject_keyboard_event(webView, awe_event);
					return (true);
				}
			}
		}
	}
	
	return (false);
}

void BrowserWidget::UpdateAddress(const char *address)
{
	if (textWidgetKey[0] != 0)
	{
		const RootWidget *root = GetRootWidget();
		if (root)
		{
			Widget *widget = root->FindWidget(textWidgetKey);
			if ((widget) && (widget->GetBaseWidgetType() == kWidgetText))
			{
				static_cast<TextWidget *>(widget)->SetText(address);
			}
		}
	}
}

void BrowserWidget::NavigateBack(void)
{
	if (webView) awe_webview_go_to_history_offset(webView, -1);
}

void BrowserWidget::NavigateForward(void)
{
	if (webView) awe_webview_go_to_history_offset(webView, 1);
}

void BrowserWidget::NavigateAddress(const char *address)
{
	if (webView)
	{
		const awe_string *empty = awe_string_empty();
		awe_string *awe_address = awe_string_create_from_ascii(address, Text::GetTextLength(address));
		awe_webview_load_url(webView, awe_address, empty, empty, empty);
		awe_string_destroy(awe_address);
	}
}

void BrowserWidget::Reload(void)
{
	if (webView) awe_webview_reload(webView);
}

void BrowserWidget::Stop(void)
{
	if (webView) awe_webview_stop(webView);
}


NavigateBrowserWidgetBackFunction::NavigateBrowserWidgetBackFunction() : WidgetFunction(kFunctionNavigateBrowserWidgetBack)
{
}

NavigateBrowserWidgetBackFunction::NavigateBrowserWidgetBackFunction(const NavigateBrowserWidgetBackFunction& navigateBrowserWidgetBackFunction) : WidgetFunction(navigateBrowserWidgetBackFunction)
{
}

NavigateBrowserWidgetBackFunction::~NavigateBrowserWidgetBackFunction()
{
}

Function *NavigateBrowserWidgetBackFunction::Replicate(void) const
{
	return (new NavigateBrowserWidgetBackFunction(*this));
}

void NavigateBrowserWidgetBackFunction::Execute(Controller *controller, FunctionMethod *method, const ScriptState *state)
{
	const Panel *root = static_cast<PanelController *>(controller)->GetRootWidget();
	
	Widget *widget = root->FindWidget(GetWidgetKey());
	while (widget)
	{
		if (widget->GetWidgetType() == kWidgetBrowser) static_cast<BrowserWidget *>(widget)->NavigateBack();
		widget = widget->GetNextWidgetWithSameKey();
	}
	
	CallCompletionProc();
}


NavigateBrowserWidgetForwardFunction::NavigateBrowserWidgetForwardFunction() : WidgetFunction(kFunctionNavigateBrowserWidgetForward)
{
}

NavigateBrowserWidgetForwardFunction::NavigateBrowserWidgetForwardFunction(const NavigateBrowserWidgetForwardFunction& navigateBrowserWidgetForwardFunction) : WidgetFunction(navigateBrowserWidgetForwardFunction)
{
}

NavigateBrowserWidgetForwardFunction::~NavigateBrowserWidgetForwardFunction()
{
}

Function *NavigateBrowserWidgetForwardFunction::Replicate(void) const
{
	return (new NavigateBrowserWidgetForwardFunction(*this));
}

void NavigateBrowserWidgetForwardFunction::Execute(Controller *controller, FunctionMethod *method, const ScriptState *state)
{
	const Panel *root = static_cast<PanelController *>(controller)->GetRootWidget();
	
	Widget *widget = root->FindWidget(GetWidgetKey());
	while (widget)
	{
		if (widget->GetWidgetType() == kWidgetBrowser) static_cast<BrowserWidget *>(widget)->NavigateForward();
		widget = widget->GetNextWidgetWithSameKey();
	}
	
	CallCompletionProc();
}


NavigateBrowserWidgetAddressFunction::NavigateBrowserWidgetAddressFunction() : WidgetFunction(kFunctionNavigateBrowserWidgetAddress)
{
	browserAddress[0] = 0;
}

NavigateBrowserWidgetAddressFunction::NavigateBrowserWidgetAddressFunction(const NavigateBrowserWidgetAddressFunction& navigateBrowserWidgetAddressFunction) : WidgetFunction(navigateBrowserWidgetAddressFunction)
{
	browserAddress = navigateBrowserWidgetAddressFunction.browserAddress;
}

NavigateBrowserWidgetAddressFunction::~NavigateBrowserWidgetAddressFunction()
{
}

Function *NavigateBrowserWidgetAddressFunction::Replicate(void) const
{
	return (new NavigateBrowserWidgetAddressFunction(*this));
}

void NavigateBrowserWidgetAddressFunction::Pack(Packer& data, unsigned_int32 packFlags) const
{
	WidgetFunction::Pack(data, packFlags);
	
	data << browserAddress;
}

void NavigateBrowserWidgetAddressFunction::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	WidgetFunction::Unpack(data, unpackFlags);
	
	data >> browserAddress;
}

void NavigateBrowserWidgetAddressFunction::Compress(Compressor& data) const
{
	WidgetFunction::Compress(data);
	
	data << browserAddress;
}

bool NavigateBrowserWidgetAddressFunction::Decompress(Decompressor& data)
{
	if (WidgetFunction::Decompress(data))
	{
		data >> browserAddress;
		return (true);
	}
	
	return (false);
}

int32 NavigateBrowserWidgetAddressFunction::GetSettingCount(void) const
{
	return (WidgetFunction::GetSettingCount() + 1);
}

Setting *NavigateBrowserWidgetAddressFunction::GetSetting(int32 index) const
{
	int32 count = WidgetFunction::GetSettingCount();
	if (index < count) return (WidgetFunction::GetSetting(index));
	
	if (index == count)
	{
		const StringTable *table = TheBrowserPlugin->GetStringTable();
		const char *title = table->GetString(StringID('FUNC', kFunctionNavigateBrowserWidgetAddress, 'ADDR'));
		return (new TextSetting('ADDR', browserAddress, title, kMaxBrowserAddressLength));
	}
	
	return (nullptr);
}

void NavigateBrowserWidgetAddressFunction::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'ADDR')
	{
		browserAddress = static_cast<const TextSetting *>(setting)->GetText();
	}
	else
	{
		WidgetFunction::SetSetting(setting);
	}
}

bool NavigateBrowserWidgetAddressFunction::OverridesFunction(const Function *function) const
{
	FunctionType type = function->GetFunctionType();
	if ((type == kFunctionNavigateBrowserWidgetAddress) || (type == kFunctionNavigateBrowserWidgetText))
	{
		return (static_cast<const WidgetFunction *>(function)->GetWidgetKey() == GetWidgetKey());
	}
	
	return (false);
}

void NavigateBrowserWidgetAddressFunction::Execute(Controller *controller, FunctionMethod *method, const ScriptState *state)
{
	const Panel *root = static_cast<PanelController *>(controller)->GetRootWidget();
	
	Widget *widget = root->FindWidget(GetWidgetKey());
	while (widget)
	{
		if (widget->GetWidgetType() == kWidgetBrowser) static_cast<BrowserWidget *>(widget)->NavigateAddress(browserAddress);
		widget = widget->GetNextWidgetWithSameKey();
	}
	
	CallCompletionProc();
}


NavigateBrowserWidgetTextFunction::NavigateBrowserWidgetTextFunction() : WidgetFunction(kFunctionNavigateBrowserWidgetText)
{
	textWidgetKey[0] = 0;
}

NavigateBrowserWidgetTextFunction::NavigateBrowserWidgetTextFunction(const NavigateBrowserWidgetTextFunction& navigateBrowserWidgetTextFunction) : WidgetFunction(navigateBrowserWidgetTextFunction)
{
	textWidgetKey = navigateBrowserWidgetTextFunction.textWidgetKey;
}

NavigateBrowserWidgetTextFunction::~NavigateBrowserWidgetTextFunction()
{
}

Function *NavigateBrowserWidgetTextFunction::Replicate(void) const
{
	return (new NavigateBrowserWidgetTextFunction(*this));
}

void NavigateBrowserWidgetTextFunction::Pack(Packer& data, unsigned_int32 packFlags) const
{
	WidgetFunction::Pack(data, packFlags);
	
	data << textWidgetKey;
}

void NavigateBrowserWidgetTextFunction::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	WidgetFunction::Unpack(data, unpackFlags);
	
	data >> textWidgetKey;
}

void NavigateBrowserWidgetTextFunction::Compress(Compressor& data) const
{
	WidgetFunction::Compress(data);
	
	data << textWidgetKey;
}

bool NavigateBrowserWidgetTextFunction::Decompress(Decompressor& data)
{
	if (WidgetFunction::Decompress(data))
	{
		data >> textWidgetKey;
		return (true);
	}
	
	return (false);
}

int32 NavigateBrowserWidgetTextFunction::GetSettingCount(void) const
{
	return (WidgetFunction::GetSettingCount() + 1);
}

Setting *NavigateBrowserWidgetTextFunction::GetSetting(int32 index) const
{
	int32 count = WidgetFunction::GetSettingCount();
	if (index < count) return (WidgetFunction::GetSetting(index));
	
	if (index == count)
	{
		const StringTable *table = TheBrowserPlugin->GetStringTable();
		const char *title = table->GetString(StringID('FUNC', kFunctionNavigateBrowserWidgetText, 'TWKY'));
		return (new TextSetting('TWKY', textWidgetKey, title, kMaxWidgetKeyLength));
	}
	
	return (nullptr);
}

void NavigateBrowserWidgetTextFunction::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'TWKY')
	{
		textWidgetKey = static_cast<const TextSetting *>(setting)->GetText();
	}
	else
	{
		WidgetFunction::SetSetting(setting);
	}
}

bool NavigateBrowserWidgetTextFunction::OverridesFunction(const Function *function) const
{
	FunctionType type = function->GetFunctionType();
	if ((type == kFunctionNavigateBrowserWidgetAddress) || (type == kFunctionNavigateBrowserWidgetText))
	{
		return (static_cast<const WidgetFunction *>(function)->GetWidgetKey() == GetWidgetKey());
	}
	
	return (false);
}

void NavigateBrowserWidgetTextFunction::Execute(Controller *controller, FunctionMethod *method, const ScriptState *state)
{
	const Panel *root = static_cast<PanelController *>(controller)->GetRootWidget();
	
	const Widget *textWidget = root->FindWidget(textWidgetKey);
	if ((textWidget) && (textWidget->GetBaseWidgetType() == kWidgetText))
	{
		const char *address = static_cast<const TextWidget *>(textWidget)->GetText();
		
		Widget *widget = root->FindWidget(GetWidgetKey());
		while (widget)
		{
			if (widget->GetWidgetType() == kWidgetBrowser) static_cast<BrowserWidget *>(widget)->NavigateAddress(address);
			widget = widget->GetNextWidgetWithSameKey();
		}
	}
	
	CallCompletionProc();
}


ReloadBrowserWidgetFunction::ReloadBrowserWidgetFunction() : WidgetFunction(kFunctionReloadBrowserWidget)
{
}

ReloadBrowserWidgetFunction::ReloadBrowserWidgetFunction(const ReloadBrowserWidgetFunction& reloadBrowserWidgetFunction) : WidgetFunction(reloadBrowserWidgetFunction)
{
}

ReloadBrowserWidgetFunction::~ReloadBrowserWidgetFunction()
{
}

Function *ReloadBrowserWidgetFunction::Replicate(void) const
{
	return (new ReloadBrowserWidgetFunction(*this));
}

void ReloadBrowserWidgetFunction::Execute(Controller *controller, FunctionMethod *method, const ScriptState *state)
{
	const Panel *root = static_cast<PanelController *>(controller)->GetRootWidget();
	
	Widget *widget = root->FindWidget(GetWidgetKey());
	while (widget)
	{
		if (widget->GetWidgetType() == kWidgetBrowser) static_cast<BrowserWidget *>(widget)->Reload();
		widget = widget->GetNextWidgetWithSameKey();
	}
	
	CallCompletionProc();
}


StopBrowserWidgetFunction::StopBrowserWidgetFunction() : WidgetFunction(kFunctionStopBrowserWidget)
{
}

StopBrowserWidgetFunction::StopBrowserWidgetFunction(const StopBrowserWidgetFunction& stopBrowserWidgetFunction) : WidgetFunction(stopBrowserWidgetFunction)
{
}

StopBrowserWidgetFunction::~StopBrowserWidgetFunction()
{
}

Function *StopBrowserWidgetFunction::Replicate(void) const
{
	return (new StopBrowserWidgetFunction(*this));
}

void StopBrowserWidgetFunction::Execute(Controller *controller, FunctionMethod *method, const ScriptState *state)
{
	const Panel *root = static_cast<PanelController *>(controller)->GetRootWidget();
	
	Widget *widget = root->FindWidget(GetWidgetKey());
	while (widget)
	{
		if (widget->GetWidgetType() == kWidgetBrowser) static_cast<BrowserWidget *>(widget)->Stop();
		widget = widget->GetNextWidgetWithSameKey();
	}
	
	CallCompletionProc();
}


BrowserPlugin::BrowserPlugin() :
		Singleton<BrowserPlugin>(TheBrowserPlugin),
		stringTable("Browser/strings"),
		browserWidgetReg(kWidgetBrowser, stringTable.GetString(StringID('WDGT', kWidgetBrowser)), "Browser/Browser")
{
	awesomiumActive = false;
	
	ControllerRegistration *registration = Controller::FindRegistration(kControllerPanel);
	static FunctionReg<NavigateBrowserWidgetBackFunction> navigateBrowserWidgetBackRegistration(registration, kFunctionNavigateBrowserWidgetBack, stringTable.GetString(StringID('FUNC', kFunctionNavigateBrowserWidgetBack)), kFunctionRemote);
	static FunctionReg<NavigateBrowserWidgetForwardFunction> navigateBrowserWidgetForwardRegistration(registration, kFunctionNavigateBrowserWidgetForward, stringTable.GetString(StringID('FUNC', kFunctionNavigateBrowserWidgetForward)), kFunctionRemote);
	static FunctionReg<NavigateBrowserWidgetAddressFunction> navigateBrowserWidgetAddressRegistration(registration, kFunctionNavigateBrowserWidgetAddress, stringTable.GetString(StringID('FUNC', kFunctionNavigateBrowserWidgetAddress)), kFunctionRemote | kFunctionJournaled);
	static FunctionReg<NavigateBrowserWidgetTextFunction> navigateBrowserWidgetTextRegistration(registration, kFunctionNavigateBrowserWidgetText, stringTable.GetString(StringID('FUNC', kFunctionNavigateBrowserWidgetText)), kFunctionRemote | kFunctionJournaled);
	static FunctionReg<ReloadBrowserWidgetFunction> reloadBrowserWidgetRegistration(registration, kFunctionReloadBrowserWidget, stringTable.GetString(StringID('FUNC', kFunctionReloadBrowserWidget)), kFunctionRemote);
	static FunctionReg<StopBrowserWidgetFunction> stopBrowserWidgetRegistration(registration, kFunctionStopBrowserWidget, stringTable.GetString(StringID('FUNC', kFunctionStopBrowserWidget)), kFunctionRemote);
}

BrowserPlugin::~BrowserPlugin()
{
	Terminate();
}

BrowserResult BrowserPlugin::Initialize(void)
{
	if (!awesomiumActive)
	{
		awesomiumActive = true;
		
		String<kMaxFilePathLength> path(TheResourceMgr->GetSystemCatalog()->GetRootPath());
		path += "Awesomium";
		
		const awe_string *empty = awe_string_empty();
		awe_string *awe_path = awe_string_create_from_ascii(path, path.Length());
		awe_webcore_initialize(true, true, false, awe_path, empty, awe_path, AWE_LL_NONE, false, empty, true, empty, empty, empty, empty, empty, empty, true, 0, false, false, empty);
		awe_string_destroy(awe_path);
	}
	
	return (kBrowserOkay);
}

void BrowserPlugin::Terminate(void)
{
	if (awesomiumActive)
	{
		awesomiumActive = false;
		awe_webcore_shutdown();
	}
}

awe_webview *BrowserPlugin::NewWebView(BrowserWidget *widget, int32 browserWidth, int32 browserHeight, unsigned_int32 flags)
{
	browserList.Append(widget);
	
	awe_webview *webView = awe_webcore_create_webview(browserWidth, browserHeight, false);
	if (flags & kBrowserTransparentBackground) awe_webview_set_transparent(webView, true);
	awe_webview_set_callback_begin_navigation(webView, &NavigationCallback);
	return (webView);
}

void BrowserPlugin::ReleaseWebView(BrowserWidget *widget, awe_webview *webView)
{
	browserList.Remove(widget);
	awe_webview_destroy(webView);
}

void BrowserPlugin::NavigationCallback(awe_webview *webView, const awe_string *url, const awe_string *frameName)
{
	BrowserWidget *widget = TheBrowserPlugin->browserList.First();
	while (widget)
	{
		if (widget->webView == webView)
		{
			int32 len = awe_string_to_utf8(url, nullptr, 0);
			char *buffer = new char[len + 1];
			awe_string_to_utf8(url, buffer, len);
			buffer[len] = 0;
			widget->UpdateAddress(buffer);
			delete[] buffer;
			break;
		}
		
		widget = widget->Next();
	}
}

void BrowserPlugin::PluginTask(void)
{
	if (awesomiumActive) awe_webcore_update();
}

// ZYURVUR
