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


#include "C4Logo.h"
#include "C4Sound.h"
#include "C4Time.h"


using namespace C4;


SequenceWidget::SequenceWidget(const Vector2D& size, const char *name) : ImageWidget(kWidgetSequence, size)
{
	framePosition = 0;
	displayFrame = 0xFFFFFFFF;
	
	sequenceImage = nullptr;
	frameOffsetTable = nullptr;
	sequenceResource = nullptr;
	
	SequenceResource *resource = SequenceResource::Get(name, kResourceDeferLoad);
	if (resource)
	{
		sequenceResource = resource;
		
		if (resource->OpenLoader(&sequenceLoader) == kResourceOkay)
		{
			resource->LoadSequenceHeader(&sequenceLoader, &sequenceHeader);
			frameOffsetTable = new unsigned_int32[sequenceHeader.frameCount + 1];
			resource->LoadFrameOffsetTable(&sequenceLoader, &sequenceHeader, frameOffsetTable);
			
			int32 sequenceWidth = sequenceHeader.sequenceWidth;
			int32 sequenceHeight = sequenceHeader.sequenceHeight;
			displayRect.Set(sequenceWidth, sequenceHeight, 0, 0);
			
			textureHeader.textureType = kTextureRectangle;
			textureHeader.textureFlags = 0;
			textureHeader.colorSemantic = kTextureSemanticNone;
			textureHeader.alphaSemantic = kTextureSemanticNone;
			textureHeader.imageFormat = kTextureRGBA8;
			textureHeader.imageWidth = sequenceWidth;
			textureHeader.imageHeight = sequenceHeight;
			textureHeader.imageDepth = 1;
			textureHeader.wrapMode[0] = kTextureClamp;
			textureHeader.wrapMode[1] = kTextureClamp;
			textureHeader.wrapMode[2] = kTextureClamp;
			textureHeader.mipmapCount = 1;
			textureHeader.mipmapDataOffset = 0;
			
			int32 pixelCount = sequenceWidth * sequenceHeight;
			sequenceImage = new Color4C[pixelCount * 2];
			tempImage = sequenceImage + pixelCount;
			MemoryMgr::ClearMemory(sequenceImage, pixelCount * sizeof(Color4C));
			
			float w = (float) sequenceWidth;
			float h = (float) sequenceHeight;
			
			float imageWidth = size.x;
			float imageHeight = imageWidth * h / w;
			SetWidgetSize(Vector2D(imageWidth, imageHeight));
			
			SetTexture(0, &textureHeader, sequenceImage);
			
			SetVertexTexcoord2D(0, Point2D(0.0F, h));
			SetVertexTexcoord2D(1, Point2D(0.0F, 0.0F));
			SetVertexTexcoord2D(2, Point2D(w, h));
			SetVertexTexcoord2D(3, Point2D(w, 0.0F));
		}
	}
}

SequenceWidget::~SequenceWidget()
{
	delete[] sequenceImage;
	delete[] frameOffsetTable;
	
	if (sequenceResource) sequenceResource->Release();
}

void SequenceWidget::Preprocess(void)
{
	ImageWidget::Preprocess();
	
	RootWidget *root = GetRootWidget();
	if (root) root->AddMovingWidget(this);
}

void SequenceWidget::ClearRect(int32 left, int32 top, int32 right, int32 bottom) const
{
	int32 width = right - left;
	int32 height = bottom - top;
	
	if ((width > 0) && (height > 0))
	{
		Color4C *destin = sequenceImage + (top * sequenceHeader.sequenceWidth + left);
		
		machine j = 0;
		do
		{
			machine i = 0;
			do
			{
				destin[i].ClearMaxAlpha();
			} while (++i < width);
			 
			destin += sequenceHeader.sequenceWidth;
		} while (++j < height); 
	} 
} 

void SequenceWidget::Move(void) 
{
	if (sequenceResource)
	{
		framePosition += TheTimeMgr->GetDeltaTime() * sequenceHeader.frameRate / 1000; 
		unsigned_int32 frame = Min(framePosition >> 16, sequenceHeader.frameCount - 1);
		
		if (frame != displayFrame)
		{ 
			unsigned_int8	*frameData;
			
			displayFrame = frame;
			sequenceResource->LoadFrameData(&sequenceLoader, frame, frameOffsetTable, &frameData);
			
			const Rect *rect = reinterpret_cast<Rect *>(frameData);
			ClearRect(displayRect.left, displayRect.top, displayRect.right, rect->top);
			ClearRect(displayRect.left, rect->bottom, displayRect.right, displayRect.bottom);
			ClearRect(displayRect.left, displayRect.top, rect->left, displayRect.bottom);
			ClearRect(rect->right, displayRect.top, displayRect.right, displayRect.bottom);
			
			const unsigned_int32 *frameSize = reinterpret_cast<const unsigned_int32 *>(rect + 1);
			Comp::DecompressData(frameData + sizeof(Rect) + 4, *frameSize, tempImage);
			
			const Color4C *source = tempImage;
			Color4C *destin = sequenceImage + (rect->top * sequenceHeader.sequenceWidth + rect->left);
			
			int32 width = rect->Width();
			int32 height = rect->Height();
			
			for (machine j = 0; j < height; j++)
			{
				for (machine i = 0; i < width; i++) destin[i] = source[i];
				
				source += width;
				destin += sequenceHeader.sequenceWidth;
			}
			
			GetTexture()->Update(displayRect | *rect);
			displayRect = *rect;
			
			delete[] frameData;
		}
	}
}

void SequenceWidget::HandleMouseEvent(const PanelMouseEventData *eventData)
{
	EventType eventType = eventData->eventType;
	if ((eventType == kEventMouseDown) || (eventType == kEventRightMouseDown) || (eventType == kEventMiddleMouseDown)) Activate();
}


LogoWindow::LogoWindow() : Window(Vector2D(800.0F, 560.0F), nullptr, kWindowPlain | kWindowFullVertical)
{
	completeFlag = false;
	
	logoWidget = new SequenceWidget(GetWidgetSize(), "C4/logo");
	AddSubnode(logoWidget);
	
	desktopColor = TheInterfaceMgr->GetInterfaceColor(kInterfaceColorDesktop);
	TheEngine->GetVariable("desktopColor")->SetValue("000000FF");
	
	logoSound = new Sound;
	logoSound->Load("C4/logo");
	logoSound->SetSoundFlags(kSoundPersistent);
	logoSound->SetCompletionProc(&SoundComplete, this);
	logoSound->Delay(1);
}

LogoWindow::~LogoWindow()
{
	char	string[9];
	
	logoSound->Release();
	
	desktopColor.GetHexString(string);
	TheEngine->GetVariable("desktopColor")->SetValue(string);
}

void LogoWindow::SoundComplete(Sound *sound, void *cookie)
{
	static_cast<LogoWindow *>(cookie)->completeFlag = true;
}

void LogoWindow::Move(void)
{
	if (completeFlag)
	{
		Close();
		return;
	}
	
	Window::Move();
}

bool LogoWindow::HandleKeyboardEvent(const KeyboardEventData *eventData)
{
	if (eventData->eventType == kEventKeyDown)
	{
		Close();
		return (true);
	}
	
	return (Window::HandleKeyboardEvent(eventData));
}

void LogoWindow::HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetActivate) Close();
}

void LogoWindow::Close(void)
{
	CallCompletionProc();
	Window::Close();
}

// ZYURVUR
