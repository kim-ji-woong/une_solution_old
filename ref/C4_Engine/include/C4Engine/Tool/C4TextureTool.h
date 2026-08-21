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


#ifndef C4TextureTool_h
#define C4TextureTool_h


#include "C4Plugins.h"


#ifdef C4TEXTURE

	#define C4TEXTUREAPI C4MODULEEXPORT
	
	
	extern "C"
	{
		C4MODULEEXPORT C4::Plugin *ConstructPlugin(void);
	}

#else

	#define C4TEXTUREAPI C4MODULEIMPORT

#endif


namespace C4
{
	enum TextureImportResult
	{
		kTextureImportOkay,
		kTextureImportFileNotFound,
		kTextureImportCreateFailed,
		kTextureImportBadFileFormat,
		kTextureImportBadTextureSize
	};
	
	
	class AmbientSpace;
	class DepthLight;
	struct CollisionData;
	
	
	class TextureTool : public Plugin, public Singleton<TextureTool>
	{
		private:
			
			StringTable						stringTable;
			
			CommandObserver<TextureTool>	textureCommandObserver;
			Command							textureCommand;
			MenuItemWidget					textureMenuItem;
			
			CommandObserver<TextureTool>	importTextureCommandObserver;
			Command							importTextureCommand;
			MenuItemWidget					importTextureMenuItem;
			
			CommandObserver<TextureTool>	terrainPaletteCommandObserver;
			Command							terrainPaletteCommand;
			MenuItemWidget					terrainPaletteMenuItem;
			
			CommandObserver<TextureTool>	generateTexturesCommandObserver;
			Command							generateTexturesCommand;
			MenuItemWidget					generateTexturesMenuItem;
			
			Link<FilePicker>				texturePicker;
			Link<FilePicker>				targaPicker;
			
			static void TexturePicked(FilePicker *picker, void *cookie);
			void HandleOpenTextureMenuItem(Widget *widget, const WidgetEventData *eventData);
			void HandleTextureCommand(Command *command, const char *text);
			
			static void ImportTexturePicked(FilePicker *picker, void *cookie);
			void HandleImportTextureMenuItem(Widget *widget, const WidgetEventData *eventData);
			void HandleImportTextureCommand(Command *command, const char *text);
			
			void HandleTerrainPaletteMenuItem(Widget *widget, const WidgetEventData *eventData);
			void HandleTerrainPaletteCommand(Command *command, const char *text);
			
			void HandleGenerateTexturesMenuItem(Widget *widget, const WidgetEventData *eventData);
			void HandleGenerateTexturesCommand(Command *command, const char *text);
		
		public:
			
			TextureTool();
			~TextureTool();
			
			const StringTable *GetStringTable(void) const
			{
				return (&stringTable);
			}
			
			static void GenerateTextures(World *world, unsigned_int32 flags);
	};
	
	
	C4TEXTUREAPI extern TextureTool *TheTextureTool;
}


#endif

// ZYURVUR
