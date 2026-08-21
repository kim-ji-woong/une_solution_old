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


#ifndef C4Logo_h
#define C4Logo_h


#include "C4Interface.h"


namespace C4
{
	class Sound;
	
	
	enum
	{
		kWidgetSequence		= 'SEQU'
	};
	
	
	class SequenceWidget : public ImageWidget
	{
		private:
			
			SequenceResource	*sequenceResource;
			ResourceLoader		sequenceLoader;
			
			UnsignedFixed		framePosition;
			unsigned_int32		displayFrame;
			Rect				displayRect;
			
			Color4C				*sequenceImage;
			Color4C				*tempImage;
			unsigned_int32		*frameOffsetTable;
			
			SequenceHeader		sequenceHeader;
			TextureHeader		textureHeader;
			
			void ClearRect(int32 left, int32 top, int32 right, int32 bottom) const;
		
		public:
			
			C4API SequenceWidget(const Vector2D& size, const char *name);
			C4API ~SequenceWidget();
			
			void Preprocess(void);
			void Move(void);
			
			void HandleMouseEvent(const PanelMouseEventData *eventData);
	};
	
	
	class LogoWindow : public Window, public Completable<LogoWindow>
	{
		private:
			
			SequenceWidget		*logoWidget;
			Sound				*logoSound;
			
			bool				completeFlag;
			ColorRGBA			desktopColor;
			
			static void SoundComplete(Sound *sound, void *cookie);
		
		public:
			
			C4API LogoWindow();
			C4API ~LogoWindow();
			
			void Move(void);
			bool HandleKeyboardEvent(const KeyboardEventData *eventData);
			void HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData);
			void Close(void);
	};
}


#endif

// ZYURVUR
