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


#ifndef C4TextureViewer_h
#define C4TextureViewer_h


#include "C4TextureImporter.h"


namespace C4
{
	class TextureWindow : public Window, public ListElement<TextureWindow>
	{
		friend class TextureTool;
		
		private:
			
			int32						mipmapLevel;
			
			ImageWidget					*textureImage[6];
			
			CheckWidget					*flipBox;
			CheckWidget					*blendBox;
			PopupMenuWidget				*backgroundMenu;
			
			QuadWidget					*colorBackground;
			ImageWidget					*checkerBackground;
			
			ResourceName				resourceName;
			TextureResource				*textureResource;
			const TextureHeader			*textureHeader;
			
			static List<TextureWindow>	windowList;
			
			void SetImagePosition(void);
		
		public:
			
			TextureWindow(const char *name, TextureResource *resource, const TextureHeader *header);
			~TextureWindow();
			
			const char *GetResourceName(void) const
			{
				return (resourceName);
			}
			
			static ResourceResult Open(const char *name);
			
			void Preprocess(void);
			
			bool HandleKeyboardEvent(const KeyboardEventData *eventData);
			void HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData);
	};
}


#endif

// ZYURVUR
