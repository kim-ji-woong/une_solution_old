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


#include "C4FilePicker.h"
#include "C4ToolWindows.h"
#include "C4Input.h"
#include "C4World.h"


using namespace C4;


namespace
{
	const float kStripHeight = 30.0F;
}


InterfaceMgr *C4::TheInterfaceMgr = nullptr;


namespace C4
{
	template <> InterfaceMgr Manager<InterfaceMgr>::managerObject(0);
	template <> InterfaceMgr **Manager<InterfaceMgr>::managerPointer = &TheInterfaceMgr;
}


#if C4LINUX

	InterfaceMgr::KeycodeList InterfaceMgr::keycodeList = {0};

#endif


ResourceDescriptor PanelResource::descriptor("pan");


PanelResource::PanelResource(const char *name, ResourceCatalog *catalog) : Resource<PanelResource>(name, catalog)
{
}

PanelResource::~PanelResource()
{
}

void PanelResource::Preprocess(void)
{
	PanelResourceHeader *header = static_cast<PanelResourceHeader *>(GetData());
	if (header->endian != 1)
	{
		Reverse(&header->version);
		Reverse(&header->widgetCount);
		Reverse(&header->auxiliaryDataSize);
	}
}


WindowEventHandler::WindowEventHandler(HandlerProc *proc, void *cookie)
{
	handlerProc = proc;
	handlerCookie = cookie;
}


C4::Cursor::Cursor() : Renderable(kRenderTriangleStrip)
{
	Initialize();
}

C4::Cursor::Cursor(const char *name) :
		Renderable(kRenderTriangleStrip),
		textureMap(name)
{
	Initialize();
}

C4::Cursor::~Cursor()
{
}

void C4::Cursor::Initialize(void)
{
	SetIdentityTransform();
	
	SetAmbientBlendState(kBlendInterpolate);
	SetTransformable(this);
	
	SetVertexCount(4);
	SetAttributeArray(kArrayVertex, cursorVertex);
	SetAttributeArray(kArrayTexture0, cursorTexcoord);
	
	attributeList.Append(&textureMap);
	SetMaterialAttributeList(&attributeList);
	
	cursorTexcoord[0].Set(0.0F, 1.0F);
	cursorTexcoord[1].Set(0.0F, 0.0F);
	cursorTexcoord[2].Set(1.0F, 1.0F);
	cursorTexcoord[3].Set(1.0F, 0.0F);
}

void C4::Cursor::Render(const Point3D& position, List<Renderable> *renderList)
{
	const Texture *texture = textureMap.GetTexture();
	if (texture) 
	{
		const Point2D& center = texture->GetImageCenter(); 
		float x = position.x - center.x; 
		float y = position.y - center.y; 
		
		float width = (float) texture->GetTextureWidth(); 
		float height = (float) texture->GetTextureHeight();
		
		cursorVertex[0].Set(x, y, 0.0F);
		cursorVertex[1].Set(x, y + height, 0.0F); 
		cursorVertex[2].Set(x + width, y, 0.0F);
		cursorVertex[3].Set(x + width, y + height, 0.0F);
		
		renderList->Append(this); 
	}
}


StripBoard::StripBoard(const Vector2D& size) :
		Board(size),
		stripWidget(size),
		menuButton(Vector2D(46.0F, 46.0F), Point2D(0.75F, 0.6875F), Point2D(0.875F, 0.8125F))
{
	menuButton.SetHiliteTexcoordOffset(Vector2D(0.0F, -0.1875F));
	menuButton.SetMenuPositionOffset(Vector2D(0.0F, 3.0F));
	menuButton.SetMenu(TheInterfaceMgr->GetToolsMenu());
	menuButton.SetBalloon(kBalloonResource, "C4/Menu");
	
	SetActiveUpdateFlags(GetActiveUpdateFlags() | kUpdateStructure);
}

StripBoard::~StripBoard()
{
}

void StripBoard::CalculateStructure(void)
{
	int32 totalCount = 0;
	int32 visibleCount = 0;
	
	WindowButtonWidget *widget = windowButtonList.First();
	while (widget)
	{
		totalCount++;
		visibleCount += widget->GetWindow()->Visible();
		
		widget = widget->Next();
	}
	
	if (totalCount > 0)
	{
		float width = Fmin(Floor((GetWidgetSize().x - 54.0F) / (float) visibleCount), 160.0F);
		float x = 54.0F;
		
		widget = windowButtonList.First();
		while (widget)
		{
			if (widget->GetWindow()->Visible())
			{
				widget->Show();
				widget->SetWidgetSize(Vector2D(width, widget->GetWidgetSize().y));
				widget->SetWidgetPosition(Point3D(x, 1.0F, 0.0F));
				widget->Invalidate();
				
				x += width;
			}
			else
			{
				widget->Hide();
			}
			
			widget = widget->Next();
		}
	}
}

void StripBoard::SetWidgetSize(const Vector2D& size)
{
	Board::SetWidgetSize(size);
	stripWidget.SetWidgetSize(size);
}

void StripBoard::Preprocess(void)
{
	AddSubnode(&stripWidget);
	
	menuButton.SetWidgetPosition(Point3D(4.0F, (stripWidget.GetWidgetSize().y - menuButton.GetWidgetSize().y) * 0.5F, 0.0F));
	AddSubnode(&menuButton);
	
	Board::Preprocess();
}

WindowButtonWidget *StripBoard::AddWindow(Window *window)
{
	WindowButtonWidget *widget = new WindowButtonWidget(window);
	windowButtonList.Append(widget);
	
	AddNewSubnode(widget);
	Invalidate();
	
	return (widget);
}

void StripBoard::HideEmpty(void)
{
	WindowButtonWidget *widget = windowButtonList.First();
	while (widget)
	{
		if (widget->GetWindow()->Visible()) return;
		widget = widget->Next();
	}
	
	Hide();
	TheInterfaceMgr->SetInputMode();
}


InterfaceMgr::InterfaceMgr(int) :
		widgetConstructor(&ConstructWidget),
		windowEventHandler(&HandleWindowEvent, this),
		mouseEventHandler(&HandleMouseEvent, this),
		keyboardEventHandler(&HandleKeyboardEvent, this),
		displayEventHandler(&HandleDisplayEvent, this),
		filePickerPersistor(&FilePicker::WritePersistentConfig),
		desktopColorObserver(this, &InterfaceMgr::HandleDesktopColorEvent),
		buttonColorObserver(this, &InterfaceMgr::HandleButtonColorEvent),
		hiliteColorObserver(this, &InterfaceMgr::HandleHiliteColorEvent),
		windowBackColorObserver(this, &InterfaceMgr::HandleWindowBackColorEvent),
		balloonBackColorObserver(this, &InterfaceMgr::HandleBalloonBackColorEvent),
		menuBackColorObserver(this, &InterfaceMgr::HandleMenuBackColorEvent),
		pageBackColorObserver(this, &InterfaceMgr::HandlePageBackColorEvent),
		windowFrameColorObserver(this, &InterfaceMgr::HandleWindowFrameColorEvent),
		pageFrameColorObserver(this, &InterfaceMgr::HandlePageFrameColorEvent),
		stripFrameColorObserver(this, &InterfaceMgr::HandleStripFrameColorEvent),
		windowTitleColorObserver(this, &InterfaceMgr::HandleWindowTitleColorEvent),
		menuTitleColorObserver(this, &InterfaceMgr::HandleMenuTitleColorEvent),
		pageTitleColorObserver(this, &InterfaceMgr::HandlePageTitleColorEvent),
		stripTitleColorObserver(this, &InterfaceMgr::HandleStripTitleColorEvent),
		stripButtonColorObserver(this, &InterfaceMgr::HandleStripButtonColorEvent)
{
}

InterfaceMgr::~InterfaceMgr()
{
}

EngineResult InterfaceMgr::Construct(void)
{
	Widget::InstallConstructor(&widgetConstructor);
	InstallWindowEventHandler(&windowEventHandler);
	TheEngine->InstallMouseEventHandler(&mouseEventHandler);
	TheEngine->InstallKeyboardEventHandler(&keyboardEventHandler);
	TheDisplayMgr->InstallDisplayEventHandler(&displayEventHandler);
	Engine::InstallPersistor(&filePickerPersistor);
	
	desktopSize.Set((float) TheDisplayMgr->GetDisplayWidth(), (float) TheDisplayMgr->GetDisplayHeight() - kStripHeight);
	
	interfaceCamera = new OrthoCameraObject;
	interfaceCamera->SetNearDepth(-1.0F);
	interfaceCamera->SetFarDepth(1.0F);
	cameraTransformable.SetWorldTransform(Transform4D(1.0F, 0.0F, 0.0F, 0.0F, 0.0F, -1.0F, 0.0F, 0.0F, 0.0F, 0.0F, -1.0F, 0.0F));
	
	arrowCursor = new Cursor("C4/arrow");
	stringTable = new StringTable("C4/engine");
	
	materialObject = new MaterialObject;
	materialObject->AddAttribute(new TextureMapAttribute("C4/gui1"));
	materialObject->AddAttribute(new EmissionMapAttribute("C4/gui2"));
	
	TheEngine->InitVariable("desktopColor", "404040", kVariablePermanent, &desktopColorObserver);
	TheEngine->InitVariable("buttonColor", "40D0FF", kVariablePermanent, &buttonColorObserver);
	TheEngine->InitVariable("hiliteColor", "00C080", kVariablePermanent, &hiliteColorObserver);
	TheEngine->InitVariable("windowBackColor", "A0A0A0", kVariablePermanent, &windowBackColorObserver);
	TheEngine->InitVariable("balloonBackColor", "EDDBB8", kVariablePermanent, &balloonBackColorObserver);
	TheEngine->InitVariable("menuBackColor", "C0C0C0F7", kVariablePermanent, &menuBackColorObserver);
	TheEngine->InitVariable("pageBackColor", "787878", kVariablePermanent, &pageBackColorObserver);
	TheEngine->InitVariable("windowFrameColor", "A0B0C0", kVariablePermanent, &windowFrameColorObserver);
	TheEngine->InitVariable("pageFrameColor", "A0B0C0", kVariablePermanent, &pageFrameColorObserver);
	TheEngine->InitVariable("stripFrameColor", "C0C0D0", kVariablePermanent, &stripFrameColorObserver);
	TheEngine->InitVariable("windowTitleColor", "FFFFFF", kVariablePermanent, &windowTitleColorObserver);
	TheEngine->InitVariable("menuTitleColor", "000000", kVariablePermanent, &menuTitleColorObserver);
	TheEngine->InitVariable("pageTitleColor", "FFFFFF", kVariablePermanent, &pageTitleColorObserver);
	TheEngine->InitVariable("stripTitleColor", "000000", kVariablePermanent, &stripTitleColorObserver);
	TheEngine->InitVariable("stripButtonColor", "40FFD0", kVariablePermanent, &stripButtonColorObserver);
	
	toolsMenu = new PulldownMenuWidget(nullptr);
	
	stripBoard = new StripBoard(Vector2D(desktopSize.x, kStripHeight));
	stripBoard->SetWidgetPosition(Point3D(0.0F, desktopSize.y, 0.0F));
	stripBoard->Preprocess();
	stripBoard->Hide();
	
	rootWidget = new Widget;
	windowRoot = new Widget;
	
	//currentCursor = arrowCursor;
	cursorPosition.Set(0.0F, 0.0F, 0.0F);
	cursorVisible = true;
	
	#if C4WINDOWS
	
		doubleClickTime = ::GetDoubleClickTime();
		caretBlinkTime = ::GetCaretBlinkTime();
	
	#elif C4MACOS
	
		doubleClickTime = ::GetDblTime() << 4;
		caretBlinkTime = ::GetCaretTime() << 4;
	
	#elif C4LINUX
	
		doubleClickTime = 200;
		caretBlinkTime = 350;
		
		::Display *display = TheEngine->GetEngineDisplay();
		keycodeList.leftShiftKeycode = XKeysymToKeycode(display, XK_Shift_L);
		keycodeList.rightShiftKeycode = XKeysymToKeycode(display, XK_Shift_R);
		keycodeList.leftAltKeycode = XKeysymToKeycode(display, XK_Alt_L);
		keycodeList.rightAltKeycode = XKeysymToKeycode(display, XK_Alt_R);
		keycodeList.leftControlKeycode = XKeysymToKeycode(display, XK_Control_L);
		keycodeList.rightControlKeycode = XKeysymToKeycode(display, XK_Control_R);
	
	#elif C4PLAYSTATION3
	
		doubleClickTime = 200;
		caretBlinkTime = 350;
	
	#endif
	
	previousClickTime = -doubleClickTime - 1;
	
	inputManagementMode = kInputManagementManual;
	
	ReadSystemClipboard();
	return (kEngineOkay);
}

void InterfaceMgr::Destruct(void)
{
	inputManagementMode = kInputManagementManual;
	
	delete activeMenu;
	delete windowRoot;
	delete rootWidget;
	delete stripBoard;
	delete toolsMenu;
	
	materialObject->Release();
	delete stringTable;
	delete arrowCursor;
	
	interfaceCamera->Release();
	
	filePickerPersistor.Detach();
	displayEventHandler.Detach();
	keyboardEventHandler.Detach();
	mouseEventHandler.Detach();
	Widget::RemoveConstructor(&widgetConstructor);
	
	WriteSystemClipboard();
	clipboard.Clear();
	
	FilePicker::PurgeVisits();
}

Widget *InterfaceMgr::ConstructWidget(Unpacker& data, unsigned_int32 unpackFlags)
{
	if (data.GetType() == kWidgetGeneric) return (new Widget);
	return (nullptr);
}

void InterfaceMgr::HandleDesktopColorEvent(Variable *variable)
{
	ColorRGB	color;
	
	color.SetHexString(variable->GetValue());
	interfaceColor[kInterfaceColorDesktop] = color;
	interfaceCamera->SetClearColor(color);
}

void InterfaceMgr::HandleButtonColorEvent(Variable *variable)
{
	interfaceColor[kInterfaceColorButton] = ColorRGBA().SetHexString(variable->GetValue());
}

void InterfaceMgr::HandleHiliteColorEvent(Variable *variable)
{
	interfaceColor[kInterfaceColorHilite] = ColorRGBA().SetHexString(variable->GetValue());
}

void InterfaceMgr::HandleWindowBackColorEvent(Variable *variable)
{
	interfaceColor[kInterfaceColorWindowBack] = ColorRGBA().SetHexString(variable->GetValue());
}

void InterfaceMgr::HandleBalloonBackColorEvent(Variable *variable)
{
	interfaceColor[kInterfaceColorBalloonBack] = ColorRGBA().SetHexString(variable->GetValue());
}

void InterfaceMgr::HandleMenuBackColorEvent(Variable *variable)
{
	interfaceColor[kInterfaceColorMenuBack] = ColorRGBA().SetHexString(variable->GetValue());
}

void InterfaceMgr::HandlePageBackColorEvent(Variable *variable)
{
	interfaceColor[kInterfaceColorPageBack] = ColorRGBA().SetHexString(variable->GetValue());
}

void InterfaceMgr::HandleWindowFrameColorEvent(Variable *variable)
{
	interfaceColor[kInterfaceColorWindowFrame] = ColorRGBA().SetHexString(variable->GetValue());
}

void InterfaceMgr::HandlePageFrameColorEvent(Variable *variable)
{
	interfaceColor[kInterfaceColorPageFrame] = ColorRGBA().SetHexString(variable->GetValue());
}

void InterfaceMgr::HandleStripFrameColorEvent(Variable *variable)
{
	interfaceColor[kInterfaceColorStripFrame] = ColorRGBA().SetHexString(variable->GetValue());
}

void InterfaceMgr::HandleWindowTitleColorEvent(Variable *variable)
{
	interfaceColor[kInterfaceColorWindowTitle] = ColorRGBA().SetHexString(variable->GetValue());
}

void InterfaceMgr::HandleMenuTitleColorEvent(Variable *variable)
{
	interfaceColor[kInterfaceColorMenuTitle] = ColorRGBA().SetHexString(variable->GetValue());
}

void InterfaceMgr::HandlePageTitleColorEvent(Variable *variable)
{
	interfaceColor[kInterfaceColorPageTitle] = ColorRGBA().SetHexString(variable->GetValue());
}

void InterfaceMgr::HandleStripTitleColorEvent(Variable *variable)
{
	interfaceColor[kInterfaceColorStripTitle] = ColorRGBA().SetHexString(variable->GetValue());
}

void InterfaceMgr::HandleStripButtonColorEvent(Variable *variable)
{
	interfaceColor[kInterfaceColorStripButton] = ColorRGBA().SetHexString(variable->GetValue());
}

bool InterfaceMgr::GetShiftKey(void)
{
	#if C4WINDOWS
	
		return (GetAsyncKeyState(VK_SHIFT) < 0);
	
	#elif C4MACOS
	
		KeyMap	keys;
		
		GetKeys(keys);
		return ((EndianU32_BtoN(keys[1].bigEndianValue) & 0x0001) != 0);
	
	#elif C4LINUX
	
		char	keymap[32];
		
		XQueryKeymap(TheEngine->GetEngineDisplay(), keymap);
		
		const KeycodeList *list = &keycodeList;
		int32 left = list->leftShiftKeycode;
		int32 right = list->rightShiftKeycode;
		return (((keymap[left >> 3] & (1 << (left & 7))) != 0) || ((keymap[right >> 3] & (1 << (right & 7))) != 0));
	
	#else
	
		return (false);
	
	#endif
}

bool InterfaceMgr::GetOptionKey(void)
{
	#if C4WINDOWS
	
		return (GetAsyncKeyState(VK_MENU) < 0);
	
	#elif C4MACOS
	
		KeyMap	keys;
		
		GetKeys(keys);
		return ((EndianU32_BtoN(keys[1].bigEndianValue) & 0x0004) != 0);
	
	#elif C4LINUX
	
		char	keymap[32];
		
		XQueryKeymap(TheEngine->GetEngineDisplay(), keymap);
		
		const KeycodeList *list = &keycodeList;
		int32 left = list->leftAltKeycode;
		int32 right = list->rightAltKeycode;
		return (((keymap[left >> 3] & (1 << (left & 7))) != 0) || ((keymap[right >> 3] & (1 << (right & 7))) != 0));
	
	#else
	
		return (false);
	
	#endif
}

bool InterfaceMgr::GetCommandKey(void)
{
	#if C4WINDOWS
	
		return (GetAsyncKeyState(VK_CONTROL) < 0);
	
	#elif C4MACOS
	
		KeyMap	keys;
		
		GetKeys(keys);
		return ((EndianU32_BtoN(keys[1].bigEndianValue) & 0x8000) != 0);
	
	#elif C4LINUX
	
		char	keymap[32];
		
		XQueryKeymap(TheEngine->GetEngineDisplay(), keymap);
		
		const KeycodeList *list = &keycodeList;
		int32 left = list->leftControlKeycode;
		int32 right = list->rightControlKeycode;
		return (((keymap[left >> 3] & (1 << (left & 7))) != 0) || ((keymap[right >> 3] & (1 << (right & 7))) != 0));
	
	#else
	
		return (false);
	
	#endif
}

unsigned_int32 InterfaceMgr::GetModifierKeys(void)
{
	#if C4WINDOWS
	
		unsigned_int32 flags = (GetAsyncKeyState(VK_SHIFT) < 0) ? kModifierKeyShift : 0;
		if (GetAsyncKeyState(VK_MENU) < 0) flags |= kModifierKeyOption;
		if (GetAsyncKeyState(VK_CONTROL) < 0) flags |= kModifierKeyCommand;
		return (flags);
	
	#elif C4MACOS
	
		KeyMap	keys;
		
		GetKeys(keys);
		unsigned_int32 flags = ((EndianU32_BtoN(keys[1].bigEndianValue) & 0x0001) != 0) ? kModifierKeyShift : 0;
		if ((EndianU32_BtoN(keys[1].bigEndianValue) & 0x0004) != 0) flags |= kModifierKeyOption;
		if ((EndianU32_BtoN(keys[1].bigEndianValue) & 0x8000) != 0) flags |= kModifierKeyCommand;
		return (flags);
	
	#elif C4LINUX
	
		char	keymap[32];
		
		XQueryKeymap(TheEngine->GetEngineDisplay(), keymap);
		
		const KeycodeList *list = &keycodeList;
		int32 left = list->leftShiftKeycode;
		int32 right = list->rightShiftKeycode;
		unsigned_int32 flags = (((keymap[left >> 3] & (1 << (left & 7))) != 0) || ((keymap[right >> 3] & (1 << (right & 7))) != 0)) ? kModifierKeyShift : 0;
		
		left = list->leftAltKeycode;
		right = list->rightAltKeycode;
		if (((keymap[left >> 3] & (1 << (left & 7))) != 0) || ((keymap[right >> 3] & (1 << (right & 7))) != 0)) flags |= kModifierKeyOption;
		
		left = list->leftControlKeycode;
		right = list->rightControlKeycode;
		if (((keymap[left >> 3] & (1 << (left & 7))) != 0) || ((keymap[right >> 3] & (1 << (right & 7))) != 0)) flags |= kModifierKeyCommand;
		
		return (flags);
	
	#else
	
		return (0);
	
	#endif
}

void InterfaceMgr::PostWindowEvent(const WindowEventData& eventData)
{
	const WindowEventHandler *handler = windowEventHandlerList.First();
	while (handler)
	{
		const WindowEventHandler *next = handler->Next();
		handler->HandleEvent(&eventData);
		handler = next;
	}
}

void InterfaceMgr::HandleWindowEvent(const WindowEventData *eventData, void *cookie)
{
	InterfaceMgr *interfaceMgr = static_cast<InterfaceMgr *>(cookie);
	
	switch (eventData->eventType)
	{
		case kEventWindowAdd:
			
			interfaceMgr->SetInputMode();
			break;
		
		case kEventWindowRemove:
			
			interfaceMgr->SetInputMode();
			break;
		
		case kEventWindowChange:
		{
			Window *window = eventData->eventWindow;
			
			if (window == interfaceMgr->GetActiveWindow())
			{
				if (!window->Visible()) interfaceMgr->SelectActiveWindow();
			}
			
			if (window->GetWindowButton()) interfaceMgr->stripBoard->Invalidate();
			
			interfaceMgr->SetInputMode();
			break;
		}
	}
}

void InterfaceMgr::HandleDisplayEvent(const DisplayEventData *eventData, void *cookie)
{
	InterfaceMgr *interfaceMgr = static_cast<InterfaceMgr *>(cookie);
	
	if (eventData->eventType == kEventDisplayChange)
	{
		StripBoard *stripBoard = interfaceMgr->stripBoard;
		float h = stripBoard->GetWidgetSize().y;
		
		Vector2D& desktopSize = interfaceMgr->desktopSize;
		desktopSize.Set((float) TheDisplayMgr->GetDisplayWidth(), (float) TheDisplayMgr->GetDisplayHeight() - h);
		
		stripBoard->SetWidgetSize(Vector2D(desktopSize.x, h));
		stripBoard->SetWidgetPosition(Point3D(0.0F, desktopSize.y, 0.0F));
		stripBoard->Invalidate();
		
		Window *window = interfaceMgr->windowList.First();
		while (window)
		{
			unsigned_int32 flags = window->GetWindowFlags();
			if (flags & (kWindowFullHorizontal | kWindowFullVertical))
			{
				window->ScaleWindow();
			}
			else if (flags & kWindowCenter)
			{
				window->CenterWindow();
			}
			else
			{
				const Point3D& position = window->GetWorldPosition();
				if ((position.x > desktopSize.x - 4.0F) || (position.y > desktopSize.y - 4.0F))
				{
					float right = position.x + window->GetWidgetSize().x + 8.0F;
					float bottom = position.y + window->GetWidgetSize().y + 8.0F;
					float dx = (right > desktopSize.x) ? desktopSize.x - right : 0.0F;
					float dy = (bottom > desktopSize.y) ? desktopSize.y - bottom : 0.0F;
					
					window->SetWidgetPosition(window->GetWidgetPosition() + Vector3D(dx, dy, 0.0F));
					window->Invalidate();
				}
			}
			
			window = window->Next();
		}
	}
}

void InterfaceMgr::AddWidget(Widget *widget)
{
	widget->Preprocess();
	
	if (widget->GetWidgetType() == kWidgetWindow)
	{
		Window *window = static_cast<Window *>(widget);
		if ((window->GetWidgetState() & (kWidgetDisabled | kWidgetHidden)) == 0)
		{
			SetActiveWindow(window);
		}
		else
		{
			windowRoot->AddSubnode(window);
			windowList.Append(window);
		}
		
		PostWindowEvent(WindowEventData(kEventWindowAdd, window));
	}
	else
	{
		rootWidget->AddSubnode(widget);
	}
}

void InterfaceMgr::RemoveWidget(Widget *widget)
{
	if (widget->GetWidgetType() == kWidgetWindow)
	{
		Window *window = static_cast<Window *>(widget);
		Window *owner = window->GetOwningWindow();
		window->Detach();
		
		if (GetActiveWindow() == window)
		{
			activeWindow = nullptr;
			
			if (owner) SetActiveWindow(owner);
			else SelectActiveWindow();
		}
		
		PostWindowEvent(WindowEventData(kEventWindowRemove, window));
	}
	else
	{
		widget->Detach();
	}
}

void InterfaceMgr::BringToFront(Window *window)
{
	Window *owner = window->GetOwningWindow();
	if (owner)
	{
		owner->BringToFront(window);
		BringToFront(owner);
	}
	else
	{
		windowRoot->AddSubnode(window);
		windowList.Append(window);
	}
}

void InterfaceMgr::SetActiveWindow(Window *window)
{
	Window *previousActiveWindow = GetActiveWindow();
	if (previousActiveWindow != window)
	{
		Widget *widget = balloonSourceWidget;
		if (widget)
		{
			delete widget->GetWidgetBalloon();
			balloonSourceWidget = nullptr;
		}
		
		if (previousActiveWindow)
		{
			previousActiveWindow->EnterBackground();
			activeWindow = nullptr;
		}
		
		if (window)
		{
			Window *subwindow = window->GetFirstSubwindow();
			while (subwindow)
			{
				if (subwindow->GetWindowFlags() & kWindowModal)
				{
					window = subwindow;
					subwindow = subwindow->GetFirstSubwindow();
					continue;
				}
				
				subwindow = subwindow->Next();
			}
			
			BringToFront(window);
			activeWindow = window;
			window->EnterForeground();
		}
	}
}

void InterfaceMgr::SelectActiveWindow(void)
{
	Window *window = windowList.Last();
	while (window)
	{
		if ((window->GetWidgetState() & (kWidgetDisabled | kWidgetHidden)) == 0)
		{
			SetActiveWindow(window);
			return;
		}
		
		window = window->Previous();
	}
	
	window = GetActiveWindow();
	if (window)
	{
		window->EnterBackground();
		activeWindow = nullptr;
	}
}

void InterfaceMgr::SetActiveMenu(Menu *menu)
{
	Menu *previousActiveMenu = GetActiveMenu();
	if (previousActiveMenu != menu)
	{
		delete previousActiveMenu;
		
		Widget *sourceWidget = balloonSourceWidget;
		if (sourceWidget)
		{
			delete sourceWidget->GetWidgetBalloon();
			balloonSourceWidget = nullptr;
		}
		
		if (menu)
		{
			menu->Preprocess();
			activeMenu = menu;
			trackWidget = menu;
		}
	}
}

void InterfaceMgr::SetInputMode(void)
{
	if (inputManagementMode == kInputManagementAutomatic)
	{
		if ((stripBoard->Visible()) || (EnabledInputWindow()))
		{
			TheInputMgr->SetInputMode(kInputInactive);
			ShowCursor();
		}
		else
		{
			if (!(TheEngine->GetEngineFlags() & kEngineQuit))
			{
				TheInputMgr->SetInputMode(kInputAllActive);
				HideCursor();
			}
		}
	}
}

bool InterfaceMgr::EnabledInputWindow(void) const
{
	Window *window = windowList.First();
	while (window)
	{
		if (((window->GetWidgetState() & (kWidgetDisabled | kWidgetHidden)) == 0) && (!(window->GetWindowFlags() & kWindowPassive)))
		{
			return (true);
		}
		
		window = window->Next();
	}
	
	return (false);
}

bool InterfaceMgr::HandleMouseDownEvent(const MouseEventData *eventData)
{
	PanelMouseEventData		panelEventData;
	
	EventType eventType = eventData->eventType;
	panelEventData.eventType = eventType;
	panelEventData.eventFlags = 0;
	
	cursorPosition.Set(Fmin(FmaxZero(eventData->mousePosition.x), (float) TheDisplayMgr->GetDisplayWidth()), Fmin(FmaxZero(eventData->mousePosition.y), (float) TheDisplayMgr->GetDisplayHeight()), 0.0F);
	
	unsigned_int32 time = TheTimeMgr->GetSystemAbsoluteTime();
	if (eventType == kEventMouseDown)
	{
		if (((int32) (time - previousClickTime) < doubleClickTime) && (SquaredMag(cursorPosition.GetVector2D() - previousClickPosition.GetVector2D()) < 144.0F))
		{
			panelEventData.eventFlags = kMouseDoubleClick;
			previousClickTime = time - doubleClickTime;
		}
		else
		{
			previousClickTime = time;
		}
	}
	else
	{
		previousClickTime = time - doubleClickTime;
	}
	
	previousClickPosition = cursorPosition;
	
	if (!trackWidget)
	{
		Widget *widget = (stripBoard->Visible()) ? stripBoard->DetectWidget(cursorPosition, 0, &panelEventData.widgetPart) : nullptr;
		if (!widget) widget = windowRoot->DetectWidget(cursorPosition, 0, &panelEventData.widgetPart);
		
		if ((widget) && (panelEventData.widgetPart != kWidgetPartNone))
		{
			unsigned_int32 usage = widget->GetWidgetUsage();
			
			Window *window = widget->GetOwningWindow();
			if (window)
			{
				if (GetActiveWindow() != window)
				{
					SetActiveWindow(window);
					if (GetActiveWindow() != window)
					{
						if ((widget->GetWidgetType() != kWidgetWindowFrame) || (panelEventData.widgetPart != kWidgetPartTitle)) return (true);
					}
				}
				
				if (usage & kWidgetKeyboardFocus) window->SetFocusWidget(widget);
			}
			
			if (!(usage & kWidgetTrackInhibit))
			{
				trackWidget = widget;
				trackPart = panelEventData.widgetPart;
			}
			
			panelEventData.mousePosition = widget->GetInverseWorldTransform() * cursorPosition;
			widget->HandleMouseEvent(&panelEventData);
			return (true);
		}
	}
	else if (trackWidget->GetWidgetType() == kWidgetMenu)
	{
		panelEventData.mousePosition = trackWidget->GetInverseWorldTransform() * cursorPosition;
		panelEventData.widgetPart = kWidgetPartInterior;
		trackWidget->HandleMouseEvent(&panelEventData);
		return (true);
	}
	
	return (false);
}

bool InterfaceMgr::HandleMouseUpEvent(const MouseEventData *eventData)
{
	cursorPosition.Set(Fmin(FmaxZero(eventData->mousePosition.x), (float) TheDisplayMgr->GetDisplayWidth()), Fmin(FmaxZero(eventData->mousePosition.y), (float) TheDisplayMgr->GetDisplayHeight()), 0.0F);
	
	Widget *widget = trackWidget;
	if (widget)
	{
		PanelMouseEventData		panelEventData;
		
		if (widget->GetWidgetType() != kWidgetMenu) trackWidget = nullptr;
		
		panelEventData.eventType = eventData->eventType;
		panelEventData.eventFlags = 0;
		panelEventData.mousePosition = widget->GetInverseWorldTransform() * cursorPosition;
		panelEventData.widgetPart = trackPart;
		widget->HandleMouseEvent(&panelEventData);
		return (true);
	}
	
	return (false);
}

bool InterfaceMgr::HandleMouseMovedEvent(const MouseEventData *eventData)
{
	cursorPosition.Set(Fmin(FmaxZero(eventData->mousePosition.x), (float) TheDisplayMgr->GetDisplayWidth()), Fmin(FmaxZero(eventData->mousePosition.y), (float) TheDisplayMgr->GetDisplayHeight()), 0.0F);
	
	Widget *widget = trackWidget;
	if (widget)
	{
		PanelMouseEventData		panelEventData;
		
		panelEventData.eventType = eventData->eventType;
		panelEventData.eventFlags = 0;
		panelEventData.mousePosition = widget->GetInverseWorldTransform() * cursorPosition;
		panelEventData.widgetPart = trackPart;
		widget->HandleMouseEvent(&panelEventData);
		
		const Widget *menu = activeMenu;
		if (menu)
		{
			MenuBarWidget *menuBar = static_cast<const Menu *>(menu)->GetMenuBar();
			if (menuBar)
			{
				panelEventData.mousePosition = menuBar->GetInverseWorldTransform() * cursorPosition;
				panelEventData.widgetPart = kWidgetPartInterior;
				menuBar->HandleMouseEvent(&panelEventData);
			}
		}
		
		return (true);
	}
	
	return (false);
}

bool InterfaceMgr::HandleMouseWheelEvent(const MouseEventData *eventData)
{
	Window *window = GetActiveWindow();
	if (window)
	{
		PanelMouseEventData		panelEventData;
		
		Widget *widget = window->DetectWidget(cursorPosition, kWidgetMouseWheel, &panelEventData.widgetPart);
		if ((widget) && (panelEventData.widgetPart != kWidgetPartNone))
		{
			panelEventData.eventType = kEventMouseWheel;
			panelEventData.eventFlags = 0;
			panelEventData.mousePosition = eventData->mousePosition;
			widget->HandleMouseEvent(&panelEventData);
			return (true);
		}
	}
	
	return (false);
}

bool InterfaceMgr::HandleMultiaxisMouseEvent(const MouseEventData *eventData)
{
	Window *window = GetActiveWindow();
	if (window)
	{
		PanelMouseEventData		panelEventData;
		
		Widget *widget = window->DetectWidget(cursorPosition, kWidgetMultiaxisMouse, &panelEventData.widgetPart);
		if ((widget) && (panelEventData.widgetPart != kWidgetPartNone))
		{
			panelEventData.eventType = eventData->eventType;
			
			if (eventData->eventType != kEventMultiaxisMouseButtonState)
			{
				panelEventData.eventFlags = 0;
				panelEventData.mousePosition = eventData->mousePosition;
			}
			else
			{
				panelEventData.eventFlags = eventData->eventFlags;
				panelEventData.mousePosition.Set(0.0F, 0.0F, 0.0F);
			}
			
			widget->HandleMouseEvent(&panelEventData);
			return (true);
		}
	}
	
	return (false);
}

bool InterfaceMgr::HandleMouseEvent(const MouseEventData *eventData, void *cookie)
{
	InterfaceMgr *interfaceMgr = static_cast<InterfaceMgr *>(cookie);
	
	switch (eventData->eventType)
	{
		case kEventMouseDown:
		case kEventRightMouseDown:
		case kEventMiddleMouseDown:
			
			return (interfaceMgr->HandleMouseDownEvent(eventData));
		
		case kEventMouseUp:
		case kEventRightMouseUp:
		case kEventMiddleMouseUp:
			
			return (interfaceMgr->HandleMouseUpEvent(eventData));
		
		case kEventMouseMoved:
			
			return (interfaceMgr->HandleMouseMovedEvent(eventData));
		
		case kEventMouseWheel:
			
			return (interfaceMgr->HandleMouseWheelEvent(eventData));
		
		case kEventMultiaxisMouseTranslation:
		case kEventMultiaxisMouseRotation:
		case kEventMultiaxisMouseButtonState:
			
			return (interfaceMgr->HandleMultiaxisMouseEvent(eventData));
	}
	
	return (false);
}

bool InterfaceMgr::HandleKeyboardEvent(const KeyboardEventData *eventData, void *cookie)
{
	InterfaceMgr *interfaceMgr = static_cast<InterfaceMgr *>(cookie);
	interfaceMgr->previousClickTime = TheTimeMgr->GetSystemAbsoluteTime() - interfaceMgr->doubleClickTime;
	
	Menu *menu = interfaceMgr->GetActiveMenu();
	if (!menu)
	{
		EventType eventType = eventData->eventType;
		
		if (eventData->modifierKeys & kModifierKeyConsole)
		{
			if (eventType == kEventKeyDown)
			{
				if (interfaceMgr->activeWindow != TheConsoleWindow) interfaceMgr->SetActiveWindow(TheConsoleWindow);
				else TheConsoleWindow->Close();
			}
			
			return (true);
		}
		
		Window *window = interfaceMgr->GetActiveWindow();
		if (window)
		{
			if (window->HandleKeyboardEvent(eventData)) return (true);
		}
		
		if (eventType == kEventKeyDown)
		{
			if ((eventData->keyCode == kKeyCodeEscape) && (!interfaceMgr->EnabledInputWindow()))
			{
				if (interfaceMgr->stripBoard->Visible())
				{
					interfaceMgr->stripBoard->Hide();
					interfaceMgr->SetInputMode();
					return (true);
				}
			}
		}
		else if (eventType == kEventKeyCommand)
		{
			if (interfaceMgr->toolsMenu->HandleKeyboardEvent(eventData)) return (true);
		}
	}
	else
	{
		return (menu->HandleKeyboardEvent(eventData));
	}
	
	return (false);
}

void InterfaceMgr::ReadSystemClipboard(void)
{
	enum
	{
		kMaxClipboardTextLength = 2048
	};
	
	#if C4WINDOWS
	
		if (OpenClipboard(nullptr))
		{
			HANDLE data = GetClipboardData(CF_UNICODETEXT);
			if (data)
			{
				const unsigned_int16 *wideText = static_cast<const unsigned_int16 *>(GlobalLock(data));
				
				int32 length = 0;
				for (machine a = 0;; a++)
				{
					unsigned_int32 code = wideText[a];
					if (code == 0) break;
					
					if (code <= 0x007F) length++;
					else if (code <= 0x07FF) length += 2;
					else if (code <= 0xFFFF) length += 3;
				}
				
				length = Min(length, kMaxClipboardTextLength);
				clipboard.SetLength(length);
				
				int32 x = 0;
				char *byte = clipboard;
				for (machine a = 0; x < length; a++) x += Text::WriteGlyphCodeUTF8(&byte[x], wideText[a]);
				
				GlobalUnlock(data);
			}
			
			CloseClipboard();
		}
	
	#elif C4MACOS
	
		PasteboardRef	pasteboard;
		
		if (PasteboardCreate(kPasteboardClipboard, &pasteboard) == noErr)
		{
			ItemCount	itemCount;
			
			PasteboardSynchronize(pasteboard);
			if (PasteboardGetItemCount(pasteboard, &itemCount) == noErr)
			{
				for (unsigned_machine itemIndex = 1; itemIndex <= itemCount; itemIndex++)
				{
					PasteboardItemID	identifier;
					CFDataRef			data;
					
					PasteboardGetItemIdentifier(pasteboard, itemIndex, &identifier);
					if (PasteboardCopyItemFlavorData(pasteboard, identifier, kUTTypeUTF8PlainText, &data) == noErr)
					{
						CFIndex len = CFDataGetLength(data);
						const void *text = CFDataGetBytePtr(data);
						clipboard.Set(static_cast<const char *>(text), Min(len, kMaxClipboardTextLength));
						CFRelease(data);
						break;
					}
				}
			}
			
			CFRelease(pasteboard);
		}
	
	#endif
}

void InterfaceMgr::WriteSystemClipboard(void)
{
	if (clipboard[0] != 0)
	{
		#if C4WINDOWS
		
			if (OpenClipboard(TheEngine->GetWindow()))
			{
				EmptyClipboard();
				
				int32 count = Text::GetGlyphCountUTF8(clipboard);
				
				HGLOBAL data = GlobalAlloc(GMEM_MOVEABLE, count * 2 + 2);
				if (data)
				{
					unsigned_int16 *wideText = static_cast<unsigned_int16 *>(GlobalLock(data));
					
					const char *byte = clipboard;
					for (machine a = 0; a < count; a++)
					{
						unsigned_int32	code;
						
						byte += Text::ReadGlyphCodeUTF8(byte, &code);
						wideText[a] = (unsigned_int16) code;
					}
					
					wideText[count] = 0;
					
					GlobalUnlock(data);
					SetClipboardData(CF_UNICODETEXT, data);
				}
				
				CloseClipboard();
			}
		
		#elif C4MACOS
		
			PasteboardRef	pasteboard;
			
			if (PasteboardCreate(kPasteboardClipboard, &pasteboard) == noErr)
			{
				PasteboardClear(pasteboard);
				
				const char *text = clipboard;
				CFDataRef data = CFDataCreate(kCFAllocatorDefault, reinterpret_cast<const UInt8 *>(text), clipboard.Length());
				if (data)
				{
					PasteboardPutItemFlavor(pasteboard, nullptr, kUTTypeUTF8PlainText, data, 0);
					CFRelease(data);
				}
				
				CFRelease(pasteboard);
			}
		
		#endif
	}
}

Widget *InterfaceMgr::FindHoverWidget(Widget **root) const
{
	if (stripBoard->Visible())
	{
		Widget *widget = stripBoard->DetectWidget(cursorPosition);
		if (widget)
		{
			*root = stripBoard;
			return (widget);
		}
	}
	
	Widget *window = activeWindow;
	if (window)
	{
		Widget *widget = window->DetectWidget(cursorPosition);
		if (widget)
		{
			*root = window;
			return (widget);
		}
	}
	
	return (nullptr);
}

void InterfaceMgr::DisplayBalloon(Widget *widget, Widget *root)
{
	BalloonType type = widget->GetBalloonType();
	if (type != kBalloonNone)
	{
		Balloon *balloon = new Balloon(type, widget->GetBalloonString());
		widget->SetWidgetBalloon(balloon);
		
		Point3D position = widget->GetWorldPosition();
		position.x += PositiveFloor(widget->GetWidgetSize().x * 0.5F) - 25.0F;
		
		float offset = 0.0F;
		if (position.x < 2.0F)
		{
			offset = position.x - 2.0F;
			position.x = 2.0F;
		}
		else
		{
			float right = position.x + balloon->GetWidgetSize().x;
			float maxRight = desktopSize.x - 2.0F;
			
			if (right > maxRight)
			{
				offset = right - maxRight;
				position.x -= offset;
			}
		}
		
		int32 location = 0;
		float height = balloon->GetWidgetSize().y + 12.0F;
		if (position.y + widget->GetWidgetSize().y + height > desktopSize.y - 2.0F)
		{
			location = 1;
			position.y -= height;
		}
		else
		{
			position.y += widget->GetWidgetSize().y + 12.0F;
		}
		
		balloon->SetWidgetPosition(root->GetInverseWorldTransform() * position);
		balloon->SetWedgeLocation(location, offset);
		root->AddNewSubnode(balloon);
	}
}

void InterfaceMgr::InterfaceTask(void)
{
	if (trackWidget)
	{
		trackWidget->TrackTask(trackPart, trackWidget->GetInverseWorldTransform() * cursorPosition);
	}
	else
	{
		Widget	*root;
		
		Widget *sourceWidget = balloonSourceWidget;
		Widget *widget = FindHoverWidget(&root);
		if (widget)
		{
			if (sourceWidget)
			{
				int32 time = balloonTime + TheTimeMgr->GetSystemDeltaTime();
				balloonTime = time;
				
				Widget *balloon = sourceWidget->GetWidgetBalloon();
				
				if (widget == sourceWidget)
				{
					if ((!balloon) && (time >= 500)) DisplayBalloon(widget, root);
				}
				else
				{
					balloonSourceWidget = widget;
					if (!balloon)
					{
						balloonTime = 0;
					}
					else
					{
						delete balloon;
						balloonTime = 300;
					}
				}
			}
			else
			{
				balloonSourceWidget = widget;
				balloonTime = 0;
			}
		}
		else
		{
			if (sourceWidget)
			{
				delete sourceWidget->GetWidgetBalloon();
				balloonSourceWidget = nullptr;
			}
		}
	}
	
	Widget *widget = rootWidget->GetFirstSubnode();
	while (widget)
	{
		Widget *next = widget->Next();
		widget->Move();
		widget = next;
	}
	
	widget = windowRoot->GetFirstSubnode();
	while (widget)
	{
		Widget *next = widget->Next();
		widget->Move();
		widget = next;
	}
}

void InterfaceMgr::Render(void)
{
	List<Renderable>	renderList;
	
	int32 width = TheDisplayMgr->GetDisplayWidth();
	int32 height = TheDisplayMgr->GetDisplayHeight();
	
	interfaceCamera->SetOrthoRect(0.0F, (float) width, (float) height, 0.0F);
	interfaceCamera->SetViewRect(Rect(0, 0, width, height));
	
	if (!TheWorldMgr->GetWorld()) interfaceCamera->SetClearFlags(kClearColorBuffer);
	TheGraphicsMgr->SetCamera(interfaceCamera, &cameraTransformable);
	interfaceCamera->SetClearFlags(0);
	
	rootWidget->Update();
	windowRoot->Update();
	stripBoard->Update();
	
	rootWidget->RenderTree(&renderList);
	windowRoot->RenderTree(&renderList);
	
	Widget *menu = activeMenu;
	if (menu)
	{
		menu->Update();
		menu->RenderTree(&renderList);
	}
	
	stripBoard->RenderTree(&renderList);
	
	//if (cursorVisible) currentCursor->Render(Point3D((float) cursorPosition.x, (float) cursorPosition.y, 0.0F), &renderList);
	
	if (!renderList.Empty())
	{
		TheGraphicsMgr->DrawRenderList(&renderList);
		renderList.RemoveAll();
	}
}

// ZYURVUR
