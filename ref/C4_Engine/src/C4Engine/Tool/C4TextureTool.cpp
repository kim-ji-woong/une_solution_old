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


#include "C4TextureTool.h"
#include "C4TextureViewer.h"
#include "C4TextureGenerator.h"
#include "C4TerrainPalette.h"
#include "C4World.h"


using namespace C4;


TextureTool *C4::TheTextureTool = nullptr;


C4::Plugin *ConstructPlugin(void)
{
	return (new TextureTool);
}


TextureTool::TextureTool() :
		Singleton<TextureTool>(TheTextureTool),
		stringTable("TextureTool/strings"),
		
		textureCommandObserver(this, &TextureTool::HandleTextureCommand),
		textureCommand("texture", &textureCommandObserver),
		textureMenuItem(stringTable.GetString(StringID('VIEW', 'MCMD')), WidgetObserver<TextureTool>(this, &TextureTool::HandleOpenTextureMenuItem)),
		
		importTextureCommandObserver(this, &TextureTool::HandleImportTextureCommand),
		importTextureCommand("itexture", &importTextureCommandObserver),
		importTextureMenuItem(stringTable.GetString(StringID('IMPT', 'MCMD')), WidgetObserver<TextureTool>(this, &TextureTool::HandleImportTextureMenuItem)),
		
		terrainPaletteCommandObserver(this, &TextureTool::HandleTerrainPaletteCommand),
		terrainPaletteCommand("texpal", &terrainPaletteCommandObserver),
		terrainPaletteMenuItem(stringTable.GetString(StringID('TPAL', 'MCMD')), WidgetObserver<TextureTool>(this, &TextureTool::HandleTerrainPaletteMenuItem)),
		
		generateTexturesCommandObserver(this, &TextureTool::HandleGenerateTexturesCommand),
		generateTexturesCommand("gentex", &generateTexturesCommandObserver),
		generateTexturesMenuItem(stringTable.GetString(StringID('TGEN', 'MCMD')), WidgetObserver<TextureTool>(this, &TextureTool::HandleGenerateTexturesMenuItem))
{
	TheEngine->AddCommand(&textureCommand);
	TheEngine->AddCommand(&importTextureCommand);
	TheEngine->AddCommand(&terrainPaletteCommand);
	TheEngine->AddCommand(&generateTexturesCommand);
	
	ThePluginMgr->AddToolMenuItem(&textureMenuItem);
	ThePluginMgr->AddToolMenuItem(&importTextureMenuItem);
	ThePluginMgr->AddToolMenuItem(&terrainPaletteMenuItem);
	ThePluginMgr->AddToolMenuItem(&generateTexturesMenuItem);
}

TextureTool::~TextureTool()
{
	FilePicker *picker = texturePicker;
	delete picker;
	
	picker = targaPicker;
	delete picker;
	
	AmbientTextureGenerator::windowList.Purge();
	ImportTextureWindow::windowList.Purge();
	TextureWindow::windowList.Purge();
	
	delete TheTerrainPaletteWindow;
	delete TheTextureGeneratorWindow;
}

void TextureTool::TexturePicked(FilePicker *picker, void *cookie)
{
	ResourceName	name;
	
	if (picker) name = picker->GetResourceName();
	else name = static_cast<const char *>(cookie);
	
	ResourceResult result = TextureWindow::Open(name);
	if (result != kResourceOkay)
	{
		const StringTable *table = TheTextureTool->GetStringTable();
		String<kMaxCommandLength> output(table->GetString(StringID('VIEW', 'NRES')));
		output += name;
		Engine::Report(output);
	}
}

void TextureTool::HandleOpenTextureMenuItem(Widget *widget, const WidgetEventData *eventData)
{
	FilePicker *picker = texturePicker;
	if (picker)
	{
		TheInterfaceMgr->SetActiveWindow(picker);
	}
	else
	{
		const char *title = stringTable.GetString(StringID('VIEW', 'OPEN'));
		
		picker = new TexturePicker('TXTR', title, TheResourceMgr->GetGenericCatalog(), TextureResource::GetDescriptor());
		picker->SetCompletionProc(&TexturePicked);
		
		texturePicker = picker;
		TheInterfaceMgr->AddWidget(picker);
	}
}
 
void TextureTool::HandleTextureCommand(Command *command, const char *text)
{ 
	if (*text != 0) 
	{ 
		ResourceName	name;
		 
		Text::ReadString(text, name, kMaxResourceNameLength);
		TexturePicked(nullptr, &name[0]);
	}
	else 
	{
		HandleOpenTextureMenuItem(nullptr, nullptr);
	}
} 

void TextureTool::ImportTexturePicked(FilePicker *picker, void *cookie)
{
	if (picker)
	{
		ResourceName name(picker->GetFileName());
		name[Text::GetResourceNameLength(name)] = 0;
		ImportTextureWindow::Open(name);
	}
	else
	{
		ImportTextureWindow::Open(static_cast<const char *>(cookie));
	}
}

void TextureTool::HandleImportTextureMenuItem(Widget *widget, const WidgetEventData *eventData)
{
	FilePicker *picker = targaPicker;
	if (picker)
	{
		TheInterfaceMgr->SetActiveWindow(picker);
	}
	else
	{
		const char *title = stringTable.GetString(StringID('IMPT', 'OPEN'));
		
		picker = new FilePicker('TIMP', title, ThePluginMgr->GetImportCatalog(), TargaResource::GetDescriptor());
		picker->SetCompletionProc(&ImportTexturePicked);
		
		targaPicker = picker;
		TheInterfaceMgr->AddWidget(picker);
	}
}

void TextureTool::HandleImportTextureCommand(Command *command, const char *text)
{
	if (*text != 0)
	{
		String<kMaxResourceNameLength>	name;
		
		text += Text::ReadString(text, name, kMaxResourceNameLength);
		text += Text::GetWhitespaceLength(text);
		
		if (*text == 0)
		{
			ImportTexturePicked(nullptr, &name[0]);
		}
		else
		{
			TextureImporter importer(name);
			importer.ProcessCommandLine(text);
			importer.ImportTexture(name);
		}
	}
	else
	{
		HandleImportTextureMenuItem(nullptr, nullptr);
	}
}

void TextureTool::HandleTerrainPaletteMenuItem(Widget *widget, const WidgetEventData *eventData)
{
	HandleTerrainPaletteCommand(nullptr, nullptr);
}

void TextureTool::HandleTerrainPaletteCommand(Command *command, const char *text)
{
	ResourceName			outputName;
	Array<ResourceName>		inputNameArray;
	
	outputName[0] = 0;
	if (text)
	{
		for (;;)
		{
			ResourceName	param;
			
			text += Text::GetWhitespaceLength(text);
			if (*text == 0) break;
			
			text += Text::ReadString(text, param, kMaxResourceNameLength);
			
			if (param == "-o")
			{
				text += Text::GetWhitespaceLength(text);
				text += Text::ReadString(text, outputName, kMaxResourceNameLength);
			}
			else
			{
				inputNameArray.AddElement(param);
			}
		}
	}
	
	if ((outputName[0] == 0) || (inputNameArray.GetElementCount() == 0))
	{
		TerrainPaletteWindow::Open();
		return;
	}
	
	TerrainPaletteWindow::GenerateTerrainPalette(outputName, inputNameArray);
}

void TextureTool::HandleGenerateTexturesMenuItem(Widget *widget, const WidgetEventData *eventData)
{
	HandleGenerateTexturesCommand(nullptr, nullptr);
}

void TextureTool::HandleGenerateTexturesCommand(Command *command, const char *text)
{
	World *world = TheWorldMgr->GetWorld();
	if (world) TextureGeneratorWindow::Open();
}

// ZYURVUR
