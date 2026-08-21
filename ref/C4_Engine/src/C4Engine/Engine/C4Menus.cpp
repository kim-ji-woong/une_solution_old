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


#include "C4Configuration.h"
#include "C4Time.h"


using namespace C4;


const Triangle MenuFrameWidget::frameTriangle[18] =
{
	{{ 0,  3,  4}},
	{{ 0,  4,  1}},
	{{ 1,  4,  5}},
	{{ 1,  5,  2}},
	{{ 3,  6,  7}},
	{{ 3,  7,  4}},
	{{ 4,  7,  8}},
	{{ 4,  8,  5}},
	{{ 9, 11, 12}},
	{{ 9, 12, 10}},
	{{11, 13, 14}},
	{{11, 14, 12}},
	{{15, 18, 19}},
	{{15, 19, 16}},
	{{16, 19, 20}},
	{{16, 20, 17}},
	{{21, 23, 24}},
	{{21, 24, 22}}
};


bool Shortcut::GetShortcutText(ShortcutString& text) const
{
	enum
	{
		kShortcutTextControl,
		kShortcutTextShift,
		kShortcutTextLeftArrow,
		kShortcutTextRightArrow,
		kShortcutTextUpArrow,
		kShortcutTextDownArrow,
		kShortcutTextDelete,
		kShortcutTextCount
	};
	
	static const unsigned_int32 shortcutTextIdentifier[kShortcutTextCount] =
	{
		#if !C4MACOS
		
			'CTRL', 'SHFT', 'LTAR', 'RTAR', 'UPAR', 'DNAR', 'DELT'
		
		#else
		
			'COMM', 'SHFT', 'LTAR', 'RTAR', 'UPAR', 'DNAR', 'DELT'
		
		#endif
	};
	
	static const char *shortcutText[kShortcutTextCount];
	
	static bool init = false;
	if (!init)
	{
		init = true;
		
		const StringTable *stringTable = TheInterfaceMgr->GetStringTable();
		for (machine a = 0; a < kShortcutTextCount; a++) shortcutText[a] = stringTable->GetString(StringID('SHRT', shortcutTextIdentifier[a]));
	}
	
	if (keyCode != 0)
	{
		if (!(shortcutFlags & kShortcutUnmodified))
		{
			text = shortcutText[kShortcutTextControl];
			if (shortcutFlags & kShortcutShift) text += shortcutText[kShortcutTextShift];
		}
		else
		{
			text[0] = 0;
		}
		
		if (keyCode - 0x20 < 0x5FU)
		{
			text += (char) keyCode;
		}
		else
		{
			switch (keyCode)
			{
				case kKeyCodeLeftArrow:
					
					text += shortcutText[kShortcutTextLeftArrow];
					break;
				
				case kKeyCodeRightArrow:
					
					text += shortcutText[kShortcutTextRightArrow];
					break;
				
				case kKeyCodeUpArrow:
					
					text += shortcutText[kShortcutTextUpArrow];
					break; 
				
				case kKeyCodeDownArrow: 
					 
					text += shortcutText[kShortcutTextDownArrow]; 
					break;
				 
				case kKeyCodeDelete:
					
					text += shortcutText[kShortcutTextDelete];
					break; 
			}
		}
		
		return (true); 
	}
	
	return (false);
}


MenuItemWidget::MenuItemWidget() :
		Widget(kWidgetMenuItem),
		menuItemObserver(nullptr)
{
	textWidget = nullptr;
	shortcutWidget = nullptr;
	bulletWidget = nullptr;
	lineWidget = nullptr;
}

MenuItemWidget::MenuItemWidget(const MenuItemWidget& menuItemWidget) :
		Widget(menuItemWidget),
		menuItemObserver(nullptr)
{
	if (menuItemWidget.textWidget) textWidget = static_cast<TextWidget *>(menuItemWidget.textWidget->Clone());
	else textWidget = nullptr;
	
	if (menuItemWidget.shortcutWidget) shortcutWidget = static_cast<TextWidget *>(menuItemWidget.shortcutWidget->Clone());
	else shortcutWidget = nullptr;
	
	if (menuItemWidget.bulletWidget) bulletWidget = static_cast<TextWidget *>(menuItemWidget.bulletWidget->Clone());
	else bulletWidget = nullptr;
	
	if (menuItemWidget.lineWidget) lineWidget = static_cast<LineWidget *>(menuItemWidget.lineWidget->Clone());
	else lineWidget = nullptr;
}

MenuItemWidget::MenuItemWidget(const char *text) :
		Widget(kWidgetMenuItem),
		menuItemObserver(nullptr)
{
	textWidget = new TextWidget(text);
	AddSubnode(textWidget);
	
	shortcutWidget = nullptr;
	bulletWidget = nullptr;
	lineWidget = nullptr;
}

MenuItemWidget::MenuItemWidget(const char *text, const ObserverType& observer) :
		Widget(kWidgetMenuItem),
		menuItemObserver(&observer)
{
	textWidget = new TextWidget(text);
	AddSubnode(textWidget);
	
	shortcutWidget = nullptr;
	bulletWidget = nullptr;
	lineWidget = nullptr;
	
	SetObserver(&menuItemObserver);
}

MenuItemWidget::MenuItemWidget(const char *text, const ObserverType& observer, const Shortcut& shortcut) :
		Widget(kWidgetMenuItem),
		menuItemObserver(&observer),
		menuItemShortcut(shortcut)
{
	Shortcut::ShortcutString	shortcutString;
	
	textWidget = new TextWidget(text);
	AddSubnode(textWidget);
	
	if (menuItemShortcut.GetShortcutText(shortcutString))
	{
		TextWidget *widget = new TextWidget(shortcutString);
		widget->SetTextAlignment(kTextAlignRight);
		AddSubnode(widget);
		shortcutWidget = widget;
	}
	else
	{
		shortcutWidget = nullptr;
	}
	
	bulletWidget = nullptr;
	lineWidget = nullptr;
	
	SetObserver(&menuItemObserver);
}

MenuItemWidget::MenuItemWidget(int32 lineStyle) :
		Widget(kWidgetMenuItem),
		menuItemObserver(nullptr)
{
	lineWidget = new LineWidget(Vector2D(1.0F, 1.0F), lineStyle, ColorRGBA(0.0F, 0.0F, 0.0F, 0.5F));
	AddSubnode(lineWidget);
	Disable();
	
	textWidget = nullptr;
	shortcutWidget = nullptr;
	bulletWidget = nullptr;
}

MenuItemWidget::~MenuItemWidget()
{
}

void MenuItemWidget::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Widget::Pack(data, packFlags);
	
	if (textWidget)
	{
		PackHandle handle = data.BeginChunk('TEXT');
		data << textWidget->GetText();
		data.EndChunk(handle);
	}
	
	data << TerminatorChunk;
}

void MenuItemWidget::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Widget::Unpack(data, unpackFlags);
	UnpackChunkList<MenuItemWidget>(data, unpackFlags);
}

bool MenuItemWidget::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'TEXT':
			
			textWidget = new TextWidget(data.ReadString());
			AddSubnode(textWidget);
			return (true);
	}
	
	return (false);
}

void *MenuItemWidget::BeginSettingsUnpack(void)
{
	delete textWidget;
	textWidget = nullptr;
	
	return (Widget::BeginSettingsUnpack());
}

void MenuItemWidget::ShowBullet(void)
{
	if (!bulletWidget)
	{
		bulletWidget = new TextWidget("\xE2\x80\xA2");		// U+2022
		bulletWidget->SetTextAlignment(kTextAlignRight);
		AddSubnode(bulletWidget);
	}
}

void MenuItemWidget::HideBullet(void)
{
	delete bulletWidget;
	bulletWidget = nullptr;
}


PopupMenuWidget::PopupMenuWidget() :
		TextWidget(kWidgetPopupMenu),
		menuObserver(this, &PopupMenuWidget::HandleMenuEvent),
		diffuseAttribute(kAttributeMutable),
		menuRenderable(kRenderQuads)
{
	buttonColor.Set(1.0F, 1.0F, 1.0F);
}

PopupMenuWidget::PopupMenuWidget(const Vector2D& size, const char *font) :
		TextWidget(kWidgetPopupMenu, size, nullptr, font),
		menuObserver(this, &PopupMenuWidget::HandleMenuEvent),
		diffuseAttribute(kAttributeMutable),
		menuRenderable(kRenderQuads)
{
	popupMenuFlags = 0;
	popupMenuSelection = kWidgetValueNone;
	
	itemSpacing = 13.0F;
	buttonColor.Set(1.0F, 1.0F, 1.0F);
}

PopupMenuWidget::PopupMenuWidget(const PopupMenuWidget& popupMenuWidget) :
		TextWidget(popupMenuWidget),
		menuObserver(this, &PopupMenuWidget::HandleMenuEvent),
		diffuseAttribute(kAttributeMutable),
		menuRenderable(kRenderQuads)
{
	popupMenuFlags = popupMenuWidget.popupMenuFlags;
	popupMenuSelection = popupMenuWidget.popupMenuSelection;
	itemSpacing = popupMenuWidget.itemSpacing;
	buttonColor = popupMenuWidget.buttonColor;
	
	const MenuItemWidget *widget = popupMenuWidget.menuItemList.First();
	while (widget)
	{
		AppendMenuItem(new MenuItemWidget(*widget));
		widget = widget->Next();
	}
}

PopupMenuWidget::~PopupMenuWidget()
{
}

Widget *PopupMenuWidget::Replicate(void) const
{
	return (new PopupMenuWidget(*this));
}

void PopupMenuWidget::Pack(Packer& data, unsigned_int32 packFlags) const
{
	TextWidget::Pack(data, packFlags);
	
	data << ChunkHeader('FLAG', 4);
	data << popupMenuFlags;
	
	data << ChunkHeader('SLCT', 4);
	data << popupMenuSelection;
	
	data << ChunkHeader('SPAC', 4);
	data << itemSpacing;
	
	data << ChunkHeader('BTTC', sizeof(ColorRGBA));
	data << buttonColor;
	
	const MenuItemWidget *widget = menuItemList.First();
	while (widget)
	{
		PackHandle handle = data.BeginChunk('ITEM');
		widget->Pack(data, packFlags);
		data.EndChunk(handle);
		
		widget = widget->Next();
	}
	
	data << TerminatorChunk;
}

void PopupMenuWidget::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	TextWidget::Unpack(data, unpackFlags);
	UnpackChunkList<PopupMenuWidget>(data, unpackFlags);
}

bool PopupMenuWidget::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'FLAG':
			
			data >> popupMenuFlags;
			return (true);
		
		case 'SLCT':
			
			data >> popupMenuSelection;
			return (true);
		
		case 'SPAC':
			
			data >> itemSpacing;
			return (true);
		
		case 'BTTC':
			
			data >> buttonColor;
			return (true);
		
		case 'ITEM':
		{
			MenuItemWidget *widget = new MenuItemWidget;
			widget->Unpack(data, unpackFlags);
			menuItemList.Append(widget);
			return (true);
		}
	}
	
	return (false);
}

void *PopupMenuWidget::BeginSettingsUnpack(void)
{
	menuItemList.Purge();
	return (TextWidget::BeginSettingsUnpack());
}

int32 PopupMenuWidget::GetSettingCount(void) const
{
	return (TextWidget::GetSettingCount() + 6);
}

Setting *PopupMenuWidget::GetSetting(int32 index) const
{
	int32 count = TextWidget::GetSettingCount();
	if (index < count) return (TextWidget::GetSetting(index));
	
	const StringTable *table = TheInterfaceMgr->GetStringTable();
	
	if (index == count)
	{
		const char *title = table->GetString(StringID('WDGT', kWidgetPopupMenu, 'SETT'));
		return (new HeadingSetting(kWidgetPopupMenu, title));
	}
	
	if (index == count + 1)
	{
		const char *title = table->GetString(StringID('WDGT', kWidgetPopupMenu, 'REND'));
		return (new BooleanSetting('PMRP', ((popupMenuFlags & kPopupMenuRenderPlain) != 0), title));
	}
	
	if (index == count + 2)
	{
		const char *title = table->GetString(StringID('WDGT', kWidgetPopupMenu, 'SPAC'));
		return (new TextSetting('MISP', itemSpacing, title));
	}
	
	if (index == count + 3)
	{
		String<>	string;
		
		const MenuItemWidget *widget = menuItemList.First();
		while (widget)
		{
			if (string[0] != 0) string += ';';
			string += widget->GetTextWidget()->GetText();
			
			widget = widget->Next();
		}
		
		const char *title = table->GetString(StringID('WDGT', kWidgetPopupMenu, 'ITEM'));
		return (new TextSetting('MITL', string, title, 1023));
	}
	
	if (index == count + 4)
	{
		const char *title = table->GetString(StringID('WDGT', kWidgetPopupMenu, 'SLCT'));
		return (new TextSetting('MSLT', Text::IntegerToString(Max(popupMenuSelection, -1)), title, 2, &EditTextWidget::SignedNumberFilter));
	}
	
	if (index == count + 5)
	{
		const char *title = table->GetString(StringID('WDGT', kWidgetPopupMenu, 'BTTC'));
		const char *picker = table->GetString(StringID('WDGT', kWidgetPopupMenu, 'BTTP'));
		return (new ColorSetting('PMBC', buttonColor, title, picker));
	}
	
	return (nullptr);
}

void PopupMenuWidget::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'PMRP')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) popupMenuFlags |= kPopupMenuRenderPlain;
		else popupMenuFlags &= ~kPopupMenuRenderPlain;
	}
	else if (identifier == 'MISP')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		itemSpacing = Fmax(Text::StringToFloat(text), 1.0F);
	}
	else if (identifier == 'MITL')
	{
		menuItemList.Purge();
		
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		if (text[0] != 0)
		{
			for (;;)
			{
				int32 len = Text::FindChar(text, ';');
				if (len >= 0)
				{
					AppendMenuItem(new MenuItemWidget(String<>(text, len)));
					text += len + 1;
				}
				else
				{
					AppendMenuItem(new MenuItemWidget(text));
					break;
				}
			}
		}
	}
	else if (identifier == 'MSLT')
	{
		int32 selection = Text::StringToInteger(static_cast<const TextSetting *>(setting)->GetText());
		if (selection < 0) selection = kWidgetValueNone;
		
		if (!GetManipulator()) SetSelection(selection);
		else popupMenuSelection = selection;
	}
	else if (identifier == 'PMBC')
	{
		buttonColor = static_cast<const ColorSetting *>(setting)->GetColor();
	}
	else
	{
		TextWidget::SetSetting(setting);
	}
}

const ColorRGBA& PopupMenuWidget::GetWidgetColor(WidgetColorType type) const
{
	if (type == kWidgetColorButton) return (buttonColor);
	return (TextWidget::GetWidgetColor(type));
}

void PopupMenuWidget::SetWidgetColor(const ColorRGBA& color, WidgetColorType type)
{
	if (type == kWidgetColorButton) buttonColor = color;
	TextWidget::SetWidgetColor(color, type);
}

void PopupMenuWidget::SetDynamicWidgetColor(const ColorRGBA& color, WidgetColorType type)
{
	if (type == kWidgetColorButton)
	{
		float a = (GetGlobalWidgetState() & kWidgetDisabled) ? 0.5F : 1.0F;
		diffuseAttribute.SetDiffuseColor(ColorRGBA(color.GetColorRGB(), a));
	}
	else
	{
		TextWidget::SetDynamicWidgetColor(color, type);
	}
}

void PopupMenuWidget::SetSelection(int32 selection, bool post)
{
	if (popupMenuSelection != selection)
	{
		popupMenuSelection = selection;
		if (selection >= 0)
		{
			const MenuItemWidget *widget = menuItemList[selection];
			if (widget)
			{
				SetText(widget->GetTextWidget()->GetText());
				goto end;
			}
		}
		
		SetText(nullptr);
		
		end:
		if (post) PostWidgetEvent(WidgetEventData(kEventWidgetChange));
	}
}

void PopupMenuWidget::Preprocess(void)
{
	TextWidget::Preprocess();
	
	InitRenderable(&menuRenderable);
	menuRenderable.SetVertexCount(12);
	menuRenderable.SetAttributeArray(kArrayVertex, menuVertex);
	menuRenderable.SetAttributeArray(kArrayTexture0, menuTexcoord);
	
	attributeList.Append(&diffuseAttribute);
	menuRenderable.SetMaterialAttributeList(&attributeList);
	menuRenderable.SetMaterialObjectPointer(TheInterfaceMgr->GetMaterialObjectPointer());
	
	if ((!GetManipulator()) && (popupMenuSelection >= 0))
	{
		const MenuItemWidget *widget = menuItemList[popupMenuSelection];
		if (widget) SetText(widget->GetTextWidget()->GetText());
	}
}

void PopupMenuWidget::Build(void)
{
	float width = GetWidgetSize().x;
	float height = GetWidgetSize().y;
	
	const Font *font = GetFont();
	if (font)
	{
		float dx = 0.0F;
		TextAlignment alignment = GetTextAlignment();
		if (alignment == kTextAlignLeft) dx = 4.0F;
		else if (alignment == kTextAlignRight) dx = -4.0F;
		
		float h = font->GetFontHeader()->fontHeight;
		SetTextRenderOffset(Vector3D(dx, FmaxZero(Floor((height - h) * 0.5F)), 0.0F));
	}
	
	float y1 = -1.0F;
	float y2 = height + 1.0F;
	float x1 = -1.0F;
	float x2 = Fmin(height * 0.5F, width * 0.5F);
	float x3 = width - x2;
	float x4 = width + 1.0F;
	
	menuVertex[0].Set(x1, y1);
	menuVertex[1].Set(x1, y2);
	menuVertex[2].Set(x2, y2);
	menuVertex[3].Set(x2, y1);
	
	menuVertex[4].Set(x2, y1);
	menuVertex[5].Set(x2, y2);
	menuVertex[6].Set(x3, y2);
	menuVertex[7].Set(x3, y1);
	
	menuVertex[8].Set(x3, y1);
	menuVertex[9].Set(x3, y2);
	menuVertex[10].Set(x4, y2);
	menuVertex[11].Set(x4, y1);
	
	menuTexcoord[0].Set(0.0F, 0.25F);
	menuTexcoord[1].Set(0.0F, 0.125F);
	menuTexcoord[2].Set(0.0625F, 0.125F);
	menuTexcoord[3].Set(0.0625F, 0.25F);
	
	float ds = Fmin((x3 - x2) / (y2 - y1) * 0.125F, 0.375F);
	
	menuTexcoord[4].Set(0.0625F, 0.25F);
	menuTexcoord[5].Set(0.0625F, 0.125F);
	menuTexcoord[6].Set(0.0625F + ds, 0.125F);
	menuTexcoord[7].Set(0.0625F + ds, 0.25F);
	
	menuTexcoord[8].Set(0.4375F, 0.25F);
	menuTexcoord[9].Set(0.4375F, 0.125F);
	menuTexcoord[10].Set(0.5F, 0.125F);
	menuTexcoord[11].Set(0.5F, 0.25F);
	
	PopupMenuWidget::SetDynamicWidgetColor(buttonColor, kWidgetColorButton);
	TextWidget::SetWidgetAlpha(diffuseAttribute.GetDiffuseColor().alpha);
	
	TextWidget::Build();
}

void PopupMenuWidget::Render(List<Renderable> *renderList)
{
	if (!(popupMenuFlags & kPopupMenuRenderPlain)) renderList->Append(&menuRenderable);
	TextWidget::Render(renderList);
}

void PopupMenuWidget::HandleMouseEvent(const PanelMouseEventData *eventData)
{
	if (eventData->eventType == kEventMouseDown)
	{
		Menu *menu = new Menu(kMenuPopup, &menuItemList, popupMenuSelection, GetWidgetSize().x, itemSpacing);
		menu->SetObserver(&menuObserver);
		
		const Point3D& position = GetWorldPosition();
		menu->SetWidgetPosition(Point3D(position.x, position.y + GetTextRenderOffset().y - menu->GetMenuPadding() - (float) Max(popupMenuSelection, -1) * itemSpacing, 0.0F));
		TheInterfaceMgr->SetActiveMenu(menu);
	}
}

void PopupMenuWidget::HandleMenuEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetSelect)
	{
		int32 selection = static_cast<const MenuEventData *>(eventData)->menuSelection;
		if (popupMenuSelection != selection)
		{
			SetSelection(selection);
			PostWidgetEvent(WidgetEventData(kEventWidgetChange));
		}
	}
}


PulldownMenuWidget::PulldownMenuWidget() : TextWidget(kWidgetPulldownMenu)
{
	RenderableWidget::SetWidgetColor(TheInterfaceMgr->GetInterfaceColor(kInterfaceColorMenuTitle));
}

PulldownMenuWidget::PulldownMenuWidget(const char *text, float spacing, const char *font) : TextWidget(kWidgetPulldownMenu, text)
{
	itemSpacing = spacing;
	fontName = font;
	
	RenderableWidget::SetWidgetColor(TheInterfaceMgr->GetInterfaceColor(kInterfaceColorMenuTitle));
}

PulldownMenuWidget::PulldownMenuWidget(const PulldownMenuWidget& pulldownMenuWidget) : TextWidget(pulldownMenuWidget)
{
	itemSpacing = pulldownMenuWidget.itemSpacing;
	fontName = pulldownMenuWidget.fontName;
}

PulldownMenuWidget::~PulldownMenuWidget()
{
}

Widget *PulldownMenuWidget::Replicate(void) const
{
	return (new PulldownMenuWidget(*this));
}

Menu *PulldownMenuWidget::DisplayMenu(const Point3D& position)
{
	Menu *menu = new Menu(kMenuPulldown, &menuItemList, -1, 0.0F, itemSpacing, fontName);
	menu->SetPulldownMenu(this);
	
	menu->SetWidgetPosition(position);
	TheInterfaceMgr->SetActiveMenu(menu);
	return (menu);
}

bool PulldownMenuWidget::HandleKeyboardEvent(const KeyboardEventData *eventData)
{
	if (eventData->eventType == kEventKeyCommand)
	{
		bool shift = ((eventData->modifierKeys & kModifierKeyShift) != 0);
		
		MenuItemWidget *widget = menuItemList.First();
		while (widget)
		{
			const Shortcut *shortcut = widget->GetShortcut();
			if (shortcut->GetKeyCode() == eventData->keyCode)
			{
				bool shortcutShift = ((shortcut->GetShortcutFlags() & kShortcutShift) != 0);
				if (shortcutShift == shift)
				{
					if (widget->Enabled()) widget->Activate();
					return (true);
				}
			}
			
			widget = widget->Next();
		}
	}
	
	return (false);
}


MenuButtonWidget::MenuButtonWidget() :
		GuiButtonWidget(kWidgetMenuButton),
		menuObserver(this, &MenuButtonWidget::HandleMenuEvent)
{
}

MenuButtonWidget::MenuButtonWidget(const Vector2D& size, const Point2D& minTex, const Point2D& maxTex) :
		GuiButtonWidget(kWidgetMenuButton, size, minTex, maxTex),
		menuObserver(this, &MenuButtonWidget::HandleMenuEvent)
{
	hiliteTexcoordOffset.Set(0.0F, 0.0F);
	menuPositionOffset.Set(0.0F, 0.0F);
}

MenuButtonWidget::MenuButtonWidget(const MenuButtonWidget& menuButtonWidget) :
		GuiButtonWidget(menuButtonWidget),
		menuObserver(this, &MenuButtonWidget::HandleMenuEvent)
{
	hiliteTexcoordOffset = menuButtonWidget.hiliteTexcoordOffset;
	menuPositionOffset = menuButtonWidget.menuPositionOffset;
}

MenuButtonWidget::~MenuButtonWidget()
{
}

Widget *MenuButtonWidget::Replicate(void) const
{
	return (new MenuButtonWidget(*this));
}

void MenuButtonWidget::Pack(Packer& data, unsigned_int32 packFlags) const
{
	GuiButtonWidget::Pack(data, packFlags);
	
	data << ChunkHeader('HTOF', sizeof(Vector2D));
	data << hiliteTexcoordOffset;
	
	data << ChunkHeader('MPOF', sizeof(Vector2D));
	data << menuPositionOffset;
	
	data << TerminatorChunk;
}

void MenuButtonWidget::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	GuiButtonWidget::Unpack(data, unpackFlags);
	UnpackChunkList<MenuButtonWidget>(data, unpackFlags);
}

bool MenuButtonWidget::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'HTOF':
			
			data >> hiliteTexcoordOffset;
			return (true);
		
		case 'MPOF':
			
			data >> menuPositionOffset;
			return (true);
	}
	
	return (false);
}

void MenuButtonWidget::SetMenu(PulldownMenuWidget *widget)
{
	menuWidget = widget;
}

void MenuButtonWidget::HandleMouseEvent(const PanelMouseEventData *eventData)
{
	if (eventData->eventType == kEventMouseDown)
	{
		PulldownMenuWidget *widget = GetMenu();
		if (widget)
		{
			SetTexcoordOffset(hiliteTexcoordOffset);
			
			const Point3D& position = GetWorldPosition();
			Menu *menu = widget->DisplayMenu(position);
			menu->SetWidgetPosition(Point3D(position.x + menuPositionOffset.x, position.y - menu->GetWidgetSize().y + menuPositionOffset.y, 0.0F));
			menu->SetObserver(&menuObserver);
		}
	}
}

bool MenuButtonWidget::HandleKeyboardEvent(const KeyboardEventData *eventData)
{
	PulldownMenuWidget *widget = GetMenu();
	if ((widget) && (widget->PulldownMenuWidget::HandleKeyboardEvent(eventData))) return (true);
	return (false);
}

void MenuButtonWidget::HandleMenuEvent(Widget *widget, const WidgetEventData *eventData)
{
	SetTexcoordOffset(Vector2D(0.0F, 0.0F));
	
	if (eventData->eventType == kEventWidgetSelect)
	{
		const MenuEventData *menuEventData = static_cast<const MenuEventData *>(eventData);
		menuEventData->menuItemWidget->Activate();
	}
}


MenuBarWidget::MenuBarWidget() :
		RenderableWidget(kWidgetMenuBar, kRenderQuads),
		menuObserver(this, &MenuBarWidget::HandleMenuEvent)
{
	preprocessFlag = false;
	menuBarUpdateFlags = kMenuBarUpdatePlacement;
	
	colorOverrideFlags = 0;
	hiliteColor.Set(0.0F, 0.0F, 0.0F);
	
	SetDefaultColorType(kWidgetColorBackground);
	SetActiveUpdateFlags(GetActiveUpdateFlags() | kUpdateStructure);
}

MenuBarWidget::MenuBarWidget(const Vector2D& size, const char *font) :
		RenderableWidget(kWidgetMenuBar, kRenderQuads, size),
		menuObserver(this, &MenuBarWidget::HandleMenuEvent)
{
	menuSpacing = 12.0F;
	menuOffset = 0.0F;
	
	colorOverrideFlags = 0;
	hiliteColor.Set(0.0F, 0.0F, 0.0F);
	
	fontName = font;
	
	preprocessFlag = false;
	menuBarUpdateFlags = kMenuBarUpdatePlacement;
	
	SetDefaultColorType(kWidgetColorBackground);
	SetActiveUpdateFlags(GetActiveUpdateFlags() | kUpdateStructure);
}

MenuBarWidget::MenuBarWidget(const MenuBarWidget& menuBarWidget) :
		RenderableWidget(menuBarWidget),
		menuObserver(this, &MenuBarWidget::HandleMenuEvent)
{
	menuSpacing = menuBarWidget.menuSpacing;
	menuOffset = menuBarWidget.menuOffset;
	
	colorOverrideFlags = menuBarWidget.colorOverrideFlags;
	hiliteColor = menuBarWidget.hiliteColor;
	
	fontName = menuBarWidget.fontName;
	
	preprocessFlag = false;
	menuBarUpdateFlags = kMenuBarUpdatePlacement;
	
	SetActiveUpdateFlags(GetActiveUpdateFlags() | kUpdateStructure);
}

MenuBarWidget::~MenuBarWidget()
{
}

Widget *MenuBarWidget::Replicate(void) const
{
	return (new MenuBarWidget(*this));
}

void MenuBarWidget::Pack(Packer& data, unsigned_int32 packFlags) const
{
	RenderableWidget::Pack(data, packFlags);
	
	data << ChunkHeader('SPAC', 4);
	data << menuSpacing;
	
	data << ChunkHeader('OFST', 4);
	data << menuOffset;
	
	data << ChunkHeader('OVER', 4);
	data << colorOverrideFlags;
	
	if (colorOverrideFlags & kWidgetOverrideHiliteColor)
	{
		data << ChunkHeader('HILC', sizeof(ColorRGBA));
		data << hiliteColor;
	}
	
	PackHandle handle = data.BeginChunk('FONT');
	data << fontName;
	data.EndChunk(handle);
	
	data << TerminatorChunk;
}

void MenuBarWidget::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	RenderableWidget::Unpack(data, unpackFlags);
	UnpackChunkList<MenuBarWidget>(data, unpackFlags);
}

bool MenuBarWidget::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'SPAC':
			
			data >> menuSpacing;
			return (true);
		
		case 'OFST':
			
			data >> menuOffset;
			return (true);
		
		case 'OVER':
			
			data >> colorOverrideFlags;
			return (true);
		
		case 'HILC':
			
			data >> hiliteColor;
			return (true);
		
		case 'FONT':
			
			data >> fontName;
			return (true);
	}
	
	return (false);
}

void *MenuBarWidget::BeginSettingsUnpack(void)
{
	colorOverrideFlags = 0;
	return (RenderableWidget::BeginSettingsUnpack());
}

int32 MenuBarWidget::GetSettingCount(void) const
{
	return (RenderableWidget::GetSettingCount() + 5);
}

Setting *MenuBarWidget::GetSetting(int32 index) const
{
	int32 count = RenderableWidget::GetSettingCount();
	if (index < count) return (RenderableWidget::GetSetting(index));
	
	const StringTable *table = TheInterfaceMgr->GetStringTable();
	
	if (index == count)
	{
		const char *title = table->GetString(StringID('WDGT', kWidgetMenuBar, 'SETT'));
		return (new HeadingSetting(kWidgetMenuBar, title));
	}
	
	if (index == count + 1)
	{
		const char *title = table->GetString(StringID('WDGT', kWidgetMenuBar, 'SPAC'));
		return (new TextSetting('MESP', menuSpacing, title));
	}
	
	if (index == count + 2)
	{
		const char *title = table->GetString(StringID('WDGT', kWidgetMenuBar, 'OFFY'));
		return (new TextSetting('MOFF', menuOffset, title));
	}
	
	if (index == count + 3)
	{
		const char *title = table->GetString(StringID('WDGT', kWidgetMenuBar, 'FONT'));
		const char *picker = table->GetString(StringID('WDGT', kWidgetMenuBar, 'PICK'));
		return (new ResourceSetting('MFNT', fontName, title, picker, FontResource::GetDescriptor()));
	}
	
	if (index == count + 4)
	{
		const char *title = table->GetString(StringID('WDGT', kWidgetMenuBar, 'HILC'));
		const char *picker = table->GetString(StringID('WDGT', kWidgetMenuBar, 'HILP'));
		return (new CheckColorSetting('MBHC', ((colorOverrideFlags & kWidgetOverrideHiliteColor) != 0), hiliteColor, title, picker, kColorPickerAlpha));
	}
	
	return (nullptr);
}

void MenuBarWidget::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'MESP')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		menuSpacing = FmaxZero(Text::StringToFloat(text));
	}
	else if (identifier == 'MOFF')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		menuOffset = Text::StringToFloat(text);
	}
	else if (identifier == 'MFNT')
	{
		fontName = static_cast<const ResourceSetting *>(setting)->GetResourceName();
	}
	else if (identifier == 'MBHC')
	{
		const CheckColorSetting *checkColorSetting = static_cast<const CheckColorSetting *>(setting);
		if (checkColorSetting->GetCheckValue() != 0)
		{
			colorOverrideFlags |= kWidgetOverrideHiliteColor;
			hiliteColor = checkColorSetting->GetColor();
		}
		else
		{
			colorOverrideFlags &= ~kWidgetOverrideHiliteColor;
			SetDefaultHiliteColor();
		}
	}
	else
	{
		RenderableWidget::SetSetting(setting);
	}
}

const ColorRGBA& MenuBarWidget::GetWidgetColor(WidgetColorType type) const
{
	if (type == kWidgetColorHilite) return (hiliteColor);
	return (RenderableWidget::GetWidgetColor(type));
}

void MenuBarWidget::SetWidgetColor(const ColorRGBA& color, WidgetColorType type)
{
	if (type == kWidgetColorHilite)
	{
		hiliteColor = color;
		colorOverrideFlags |= kWidgetOverrideHiliteColor;
		SetBuildFlag();
	}
	
	RenderableWidget::SetWidgetColor(color, type);
}

void MenuBarWidget::SetWidgetAlpha(float alpha, WidgetColorType type)
{
	if (type == kWidgetColorHilite)
	{
		hiliteColor.alpha = alpha;
		colorOverrideFlags |= kWidgetOverrideHiliteColor;
		SetBuildFlag();
	}
	
	RenderableWidget::SetWidgetAlpha(alpha, type);
}

void MenuBarWidget::SetDynamicWidgetColor(const ColorRGBA& color, WidgetColorType type)
{
	if ((type == kWidgetColorDefault) || (type == kWidgetColorBackground))
	{
		for (machine a = 0; a < 4; a++) menuBarColor[a] = color;
	}
	else if (type == kWidgetColorHilite)
	{
		for (machine a = 4; a < 8; a++) menuBarColor[a] = color;
	}
	else
	{
		RenderableWidget::SetDynamicWidgetColor(color, type);
	}
}

void MenuBarWidget::SetDynamicWidgetAlpha(float alpha, WidgetColorType type)
{
	if ((type == kWidgetColorDefault) || (type == kWidgetColorBackground))
	{
		for (machine a = 0; a < 4; a++) menuBarColor[a].alpha = alpha;
	}
	else if (type == kWidgetColorHilite)
	{
		for (machine a = 4; a < 8; a++) menuBarColor[a].alpha = alpha;
	}
	else
	{
		RenderableWidget::SetDynamicWidgetAlpha(alpha, type);
	}
}

void MenuBarWidget::PrependMenu(PulldownMenuWidget *widget)
{
	menuList.Prepend(widget);
	AddSubnode(widget);
	
	widget->Disable();
	widget->TextWidget::SetFont(fontName);
	if (preprocessFlag) widget->Preprocess();
	
	SetMenuBarUpdateFlags(kMenuBarUpdatePlacement);
}

void MenuBarWidget::AppendMenu(PulldownMenuWidget *widget)
{
	menuList.Append(widget);
	AddSubnode(widget);
	
	widget->Disable();
	widget->TextWidget::SetFont(fontName);
	if (preprocessFlag) widget->Preprocess();
	
	SetMenuBarUpdateFlags(kMenuBarUpdatePlacement);
}

void MenuBarWidget::InsertMenuBefore(PulldownMenuWidget *widget, PulldownMenuWidget *before)
{
	menuList.InsertBefore(widget, before);
	AddSubnode(widget);
	
	widget->Disable();
	widget->TextWidget::SetFont(fontName);
	if (preprocessFlag) widget->Preprocess();
	
	SetMenuBarUpdateFlags(kMenuBarUpdatePlacement);
}

void MenuBarWidget::InsertMenuAfter(PulldownMenuWidget *widget, PulldownMenuWidget *after)
{
	menuList.InsertAfter(widget, after);
	AddSubnode(widget);
	
	widget->Disable();
	widget->TextWidget::SetFont(fontName);
	if (preprocessFlag) widget->Preprocess();
	
	SetMenuBarUpdateFlags(kMenuBarUpdatePlacement);
}

void MenuBarWidget::RemoveMenu(PulldownMenuWidget *widget)
{
	menuList.Remove(widget);
	RemoveSubnode(widget);
	
	SetMenuBarUpdateFlags(kMenuBarUpdatePlacement);
}

void MenuBarWidget::SetDefaultHiliteColor(void)
{
	hiliteColor = TheInterfaceMgr->GetInterfaceColor(kInterfaceColorHilite);
}

void MenuBarWidget::Preprocess(void)
{
	if (!(colorOverrideFlags & kWidgetOverrideHiliteColor)) SetDefaultHiliteColor();
	
	RenderableWidget::Preprocess();
	
	Window *window = GetOwningWindow();
	if (window) window->SetCommandWidget(this);
	
	SetAttributeArray(kArrayVertex, menuBarVertex);
	SetAttributeArray(kArrayColor0, menuBarColor);
	SetAttributeArray(kArrayTexture0, menuBarTexcoord);
	
	SetMaterialObjectPointer(TheInterfaceMgr->GetMaterialObjectPointer());
	
	preprocessFlag = true;
}

void MenuBarWidget::CalculateStructure(void)
{
	unsigned_int32 flags = menuBarUpdateFlags;
	if (flags & kMenuBarUpdatePlacement)
	{
		menuBarUpdateFlags = flags & ~kMenuBarUpdatePlacement;
		
		float height = GetWidgetSize().y;
		float x = menuSpacing * 0.5F;
		
		PulldownMenuWidget *widget = menuList.First();
		while (widget)
		{
			widget->SetWidgetPosition(Point3D(x, menuOffset, 0.0F));
			
			float width = widget->GetFormattedTextWidth();
			widget->SetWidgetSize(Vector2D(width, height));
			
			x += width + menuSpacing;
			widget = widget->Next();
		}
	}
}

void MenuBarWidget::Build(void)
{
	float w = GetWidgetSize().x;
	float h = GetWidgetSize().y;
	
	menuBarVertex[0].Set(0.0F, 0.0F);
	menuBarVertex[1].Set(0.0F, h);
	menuBarVertex[2].Set(w, h);
	menuBarVertex[3].Set(w, 0.0F);
	
	float s = Fmin(w, 128.0F) * 0.0009765625F;
	menuBarTexcoord[0].Set(0.0F, 0.041015625F);
	menuBarTexcoord[1].Set(0.0F, 0.0F);
	menuBarTexcoord[2].Set(s, 0.0F);
	menuBarTexcoord[3].Set(s, 0.041015625F);
	
	if (GetWidgetState() & kWidgetHilited)
	{
		SetVertexCount(8);
		
		menuBarVertex[4].Set(hiliteRange.min, 0.0F);
		menuBarVertex[5].Set(hiliteRange.min, h);
		menuBarVertex[6].Set(hiliteRange.max, h);
		menuBarVertex[7].Set(hiliteRange.max, 0.0F);
		
		menuBarTexcoord[4].Set(0.0F, 0.041015625F);
		menuBarTexcoord[5].Set(0.0F, 0.0F);
		menuBarTexcoord[6].Set(0.0F, 0.0F);
		menuBarTexcoord[7].Set(0.0F, 0.041015625F);
	}
	else
	{
		SetVertexCount(4);
	}
	
	const ColorRGBA& color = RenderableWidget::GetWidgetColor();
	for (machine a = 0; a < 4; a++) menuBarColor[a] = color;
	for (machine a = 4; a < 8; a++) menuBarColor[a] = hiliteColor;
}

void MenuBarWidget::HandleMouseEvent(const PanelMouseEventData *eventData)
{
	EventType eventType = eventData->eventType;
	if ((eventType == kEventMouseDown) || (eventType == kEventMouseMoved))
	{
		if (WidgetContainsPoint(eventData->mousePosition))
		{
			float x1 = 0.0F;
			PulldownMenuWidget *widget = menuList.First();
			while (widget)
			{
				float width = widget->GetFormattedTextWidth();
				float x2 = x1 + width + menuSpacing;
				
				if (eventData->mousePosition.x < x2)
				{
					hiliteRange.Set(x1, x2);
					SetWidgetState(GetWidgetState() | kWidgetHilited);
					SetBuildFlag();
					break;
				}
				
				x1 = x2;
				widget = widget->Next();
			}
			
			if (widget)
			{
				Menu *menu = TheInterfaceMgr->GetActiveMenu();
				if ((!menu) || (widget != menu->GetPulldownMenu()))
				{
					const Point3D& position = GetWorldPosition();
					Menu *menu = widget->DisplayMenu(Point3D(position.x + x1, position.y + GetWidgetSize().y, 0.0F));
					menu->SetObserver(&menuObserver);
					menu->SetMenuBar(this);
				}
			}
		}
	}
}

bool MenuBarWidget::HandleKeyboardEvent(const KeyboardEventData *eventData)
{
	PulldownMenuWidget *widget = menuList.First();
	while (widget)
	{
		if (widget->PulldownMenuWidget::HandleKeyboardEvent(eventData)) return (true);
		widget = widget->Next();
	}
	
	return (false);
}

void MenuBarWidget::HandleMenuEvent(Widget *widget, const WidgetEventData *eventData)
{
	SetWidgetState(GetWidgetState() & ~kWidgetHilited);
	
	if (eventData->eventType == kEventWidgetSelect)
	{
		const MenuEventData *menuEventData = static_cast<const MenuEventData *>(eventData);
		menuEventData->menuItemWidget->Activate();
	}
}


MenuFrameWidget::MenuFrameWidget() :
		RenderableWidget(kWidgetMenuFrame, kRenderIndexedTriangles),
		diffuseAttribute(kAttributeMutable)
{
	SetDefaultColorType(kWidgetColorBackground);
	RenderableWidget::SetWidgetColor(TheInterfaceMgr->GetInterfaceColor(kInterfaceColorMenuBack));
}

MenuFrameWidget::MenuFrameWidget(const Vector2D& size) :
		RenderableWidget(kWidgetMenuFrame, kRenderIndexedTriangles, size),
		diffuseAttribute(kAttributeMutable)
{
	SetDefaultColorType(kWidgetColorBackground);
	RenderableWidget::SetWidgetColor(TheInterfaceMgr->GetInterfaceColor(kInterfaceColorMenuBack));
}

MenuFrameWidget::~MenuFrameWidget()
{
}

void MenuFrameWidget::Preprocess(void)
{
	RenderableWidget::Preprocess();
	
	attributeList.Append(&diffuseAttribute);
	SetMaterialAttributeList(&attributeList);
	SetMaterialObjectPointer(TheInterfaceMgr->GetMaterialObjectPointer());
	
	SetVertexCount(25);
	SetAttributeArray(kArrayVertex, frameVertex);
	SetAttributeArray(kArrayTexture0, frameTexcoord);
	SetTriangleArray(18, frameTriangle);
	
	float s = Fmin(GetWidgetSize().x - 8.0F, 56.0F) * 0.001953125F + 0.7578125F;
	float t = 0.4296875F - Fmin(GetWidgetSize().y - 8.0F, 56.0F) * 0.001953125F;
	
	frameTexcoord[0].Set(0.734375F, 0.4453125F);
	frameTexcoord[1].Set(0.7578125F, 0.4453125F);
	frameTexcoord[2].Set(s, 0.4453125F);
	frameTexcoord[3].Set(0.734375F, 0.4296875F);
	frameTexcoord[4].Set(0.7578125F, 0.4296875F);
	frameTexcoord[5].Set(s, 0.4296875F);
	frameTexcoord[6].Set(0.734375F, t);
	frameTexcoord[7].Set(0.7578125F, t);
	frameTexcoord[8].Set(s, t);
	
	frameTexcoord[9].Set(0.8671875F, 0.4453125F);
	frameTexcoord[10].Set(0.890625F, 0.4453125F);
	frameTexcoord[11].Set(0.8671875F, 0.4296875F);
	frameTexcoord[12].Set(0.890625F, 0.4296875F);
	frameTexcoord[13].Set(0.8671875F, t);
	frameTexcoord[14].Set(0.890625F, t);
	
	frameTexcoord[15].Set(0.734375F, 0.3203125F);
	frameTexcoord[16].Set(0.7578125F, 0.3203125F);
	frameTexcoord[17].Set(s, 0.3203125F);
	frameTexcoord[18].Set(0.734375F, 0.2890625F);
	frameTexcoord[19].Set(0.7578125F, 0.2890625F);
	frameTexcoord[20].Set(s, 0.2890625F);
	
	frameTexcoord[21].Set(0.8671875F, 0.3203125F);
	frameTexcoord[22].Set(0.890625F, 0.3203125F);
	frameTexcoord[23].Set(0.8671875F, 0.2890625F);
	frameTexcoord[24].Set(0.890625F, 0.2890625F);
}

void MenuFrameWidget::Build(void)
{
	float width = GetWidgetSize().x;
	float height = GetWidgetSize().y;
	
	frameVertex[0].Set(-8.0F, -4.0F);
	frameVertex[1].Set(4.0F, -4.0F);
	frameVertex[2].Set(width - 4.0F, -4.0F);
	frameVertex[3].Set(-8.0F, 4.0F);
	frameVertex[4].Set(4.0F, 4.0F);
	frameVertex[5].Set(width - 4.0F, 4.0F);
	frameVertex[6].Set(-8.0F, height - 4.0F);
	frameVertex[7].Set(4.0F, height - 4.0F);
	frameVertex[8].Set(width - 4.0F, height - 4.0F);
	
	frameVertex[9].Set(width - 4.0F, -4.0F);
	frameVertex[10].Set(width + 8.0F, -4.0F);
	frameVertex[11].Set(width - 4.0F, 4.0F);
	frameVertex[12].Set(width + 8.0F, 4.0F);
	frameVertex[13].Set(width - 4.0F, height - 4.0F);
	frameVertex[14].Set(width + 8.0F, height - 4.0F);
	
	frameVertex[15].Set(-8.0F, height - 4.0F);
	frameVertex[16].Set(4.0F, height - 4.0F);
	frameVertex[17].Set(width - 4.0F, height - 4.0F);
	frameVertex[18].Set(-8.0F, height + 12.0F);
	frameVertex[19].Set(4.0F, height + 12.0F);
	frameVertex[20].Set(width - 4.0F, height + 12.0F);
	
	frameVertex[21].Set(width - 4.0F, height - 4.0F);
	frameVertex[22].Set(width + 8.0F, height - 4.0F);
	frameVertex[23].Set(width - 4.0F, height + 12.0F);
	frameVertex[24].Set(width + 8.0F, height + 12.0F);
	
	diffuseAttribute.SetDiffuseColor(GetWidgetColor());
}


Menu::Menu(int32 style, const List<MenuItemWidget> *list, int32 selection, float minWidth, float spacing, const char *font) :
		RootWidget(kWidgetMenu),
		quadWidget(Vector2D(minWidth, spacing))
{
	itemList = list;
	
	menuStyle = style;
	itemSpacing = spacing;
	minMenuWidth = minWidth;
	fontName = font;
	
	itemPadding = (menuStyle == kMenuPopup) ? 2.0F : 10.0F;
	menuPadding = 2.0F;
	
	initialSelection = selection;
	openTime = 0;
}

Menu::~Menu()
{
	Widget *widget = GetFirstSubnode();
	while (widget)
	{
		Widget *next = widget->Next();
		if (widget->GetWidgetType() == kWidgetMenuItem) widget->UpdatableTree<Widget>::Detach();
		widget = next;
	}
}

void Menu::SetSelection(int32 selection)
{
	itemSelection = selection;
	if (selection >= 0)
	{
		quadWidget.SetWidgetPosition(Point3D(menuPadding, (float) selection * itemSpacing + menuPadding, 0.0F));
		quadWidget.Invalidate();
		quadWidget.Show();
	}
	else
	{
		quadWidget.Hide();
	}
}

void Menu::Preprocess(void)
{
	AutoReleaseFont font(fontName);
	
	float width = 0.0F;
	float height = menuPadding;
	
	int32 count = 0;
	MenuItemWidget *widget = itemList->First();
	while (widget)
	{
		float w = 0.0F;
		ColorRGBA color(0.0F, 0.0F, 0.0F, (widget->Enabled()) ? 1.0F : 0.5F);
		
		TextWidget *textWidget = widget->GetTextWidget();
		if (textWidget)
		{
			textWidget->SetFont(fontName);
			textWidget->SetWidgetColor(color);
			w = font->GetTextWidth(textWidget->GetText());
			textWidget->SetWidgetSize(Vector2D(w, itemSpacing));
		}
		
		TextWidget *shortcutWidget = widget->GetShortcutWidget();
		if (shortcutWidget)
		{
			shortcutWidget->SetFont(fontName);
			shortcutWidget->SetWidgetColor(color);
			float x = font->GetTextWidth(shortcutWidget->GetText());
			shortcutWidget->SetWidgetSize(Vector2D(x, itemSpacing));
			w += x + 24.0F;
		}
		
		TextWidget *bulletWidget = widget->GetBulletWidget();
		if (bulletWidget)
		{
			bulletWidget->SetFont(fontName);
			bulletWidget->SetWidgetColor(color);
			bulletWidget->SetWidgetSize(Vector2D(itemPadding - 4.0F, itemSpacing));
		}
		
		width = Fmax(width, w);
		
		widget->SetWidgetPosition(Point3D(menuPadding, height, 0.0F));
		AddSubnode(widget);
		
		count++;
		height += itemSpacing;
		
		widget = widget->Next();
	}
	
	width = Fmax(width + (itemPadding + menuPadding) * 2.0F, minMenuWidth);
	height += menuPadding;
	itemCount = count;
	
	float x = width - (itemPadding + menuPadding * 2.0F);
	widget = itemList->First();
	while (widget)
	{
		TextWidget *textWidget = widget->GetTextWidget();
		if (textWidget)
		{
			textWidget->SetWidgetPosition(Point3D(itemPadding, 0.0F, 0.0F));
		}
		
		TextWidget *shortcutWidget = widget->GetShortcutWidget();
		if (shortcutWidget)
		{
			shortcutWidget->SetWidgetPosition(Point3D(x - shortcutWidget->GetWidgetSize().x, 0.0F, 0.0F));
		}
		
		TextWidget *bulletWidget = widget->GetBulletWidget();
		if (bulletWidget)
		{
			bulletWidget->SetWidgetPosition(Point3D(0.0F, 0.0F, 0.0F));
		}
		
		LineWidget *lineWidget = widget->GetLineWidget();
		if (lineWidget)
		{
			lineWidget->SetWidgetPosition(Point3D(-menuPadding, PositiveFloor(itemSpacing * 0.5F), 0.0F));
			lineWidget->SetWidgetSize(Vector2D(width, 1.0F));
		}
		
		widget = widget->Next();
	}
	
	Vector2D size(width, height);
	SetWidgetSize(size);
	
	quadWidget.SetWidgetSize(Vector2D(size.x - menuPadding * 2.0F, itemSpacing));
	quadWidget.SetWidgetColor(ColorRGBA(TheInterfaceMgr->GetInterfaceColor(kInterfaceColorHilite).GetColorRGB() * 0.75F, 1.0F));
	AddFirstSubnode(&quadWidget);
	SetSelection(initialSelection);
	
	frameWidget.SetWidgetSize(size);
	AddFirstSubnode(&frameWidget);
	
	RootWidget::Preprocess();
}

void Menu::HandleMouseEvent(const PanelMouseEventData *eventData)
{
	EventType eventType = eventData->eventType;
	if ((eventType == kEventMouseDown) || (eventType == kEventRightMouseDown) || (eventType == kEventMiddleMouseDown))
	{
		if (!WidgetContainsPoint(eventData->mousePosition))
		{
			MenuEventData menuEventData(kEventWidgetCancel);
			PostEvent(&menuEventData);
			delete this;
			return;
		}
	}
	else if ((eventType != kEventMouseUp) && (eventType != kEventMouseMoved))
	{
		return;
	}
	
	int32 selection = -1;
	MenuItemWidget *widget = nullptr;
	
	const Point3D& position = eventData->mousePosition;
	if ((position.x >= menuPadding) && (position.x <= GetWidgetSize().x - menuPadding))
	{
		selection = (int32) Floor((position.y - menuPadding) / itemSpacing);
		if ((unsigned_int32) selection < (unsigned_int32) itemCount) widget = (*itemList)[selection];
		else selection = -1;
	}
	
	if ((widget) && (!widget->Enabled())) widget = nullptr;
	
	if (itemSelection != selection)
	{
		initialSelection = kWidgetValueNone;
		SetSelection((widget) ? selection : -1);
	}
	
	if (eventType == kEventMouseDown)
	{
		initialSelection = kWidgetValueNone;
		openTime = kMinMenuOpenTime;
	}
	else if (eventType == kEventMouseUp)
	{
		if (selection < 0)
		{
			if (menuStyle != kMenuPulldown)
			{
				MenuEventData menuEventData(kEventWidgetCancel);
				PostEvent(&menuEventData);
				delete this;
			}
		}
		else if ((widget) && (selection != initialSelection) && (openTime >= kMinMenuOpenTime))
		{
			if (menuStyle != kMenuContextual)
			{
				MenuEventData menuEventData(kEventWidgetSelect, selection, widget);
				PostEvent(&menuEventData);
			}
			else
			{
				widget->Activate();
			}
			
			delete this;
		}
	}
}

bool Menu::HandleKeyboardEvent(const KeyboardEventData *eventData)
{
	if ((eventData->eventType == kEventKeyDown) && (eventData->keyCode == kKeyCodeEscape))
	{
		MenuEventData menuEventData(kEventWidgetCancel);
		PostEvent(&menuEventData);
		delete this;
		return (true);
	}
	
	return (false);
}

void Menu::TrackTask(WidgetPart widgetPart, const Point3D& mousePosition)
{
	openTime += TheTimeMgr->GetSystemDeltaTime();
}

// ZYURVUR
