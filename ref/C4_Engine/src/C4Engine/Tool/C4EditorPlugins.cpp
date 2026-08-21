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


#include "C4WorldEditor.h"


using namespace C4;


EditorPlugin::EditorPlugin()
{
	TheWorldEditor->RegisterEditorPlugin(this);
}

EditorPlugin::~EditorPlugin()
{
}

void EditorPlugin::Initialize(EditorObject *editorObject)
{
}

void EditorPlugin::Preprocess(Editor *editor)
{
}


SceneImportPlugin::SceneImportPlugin()
{
	TheWorldEditor->RegisterSceneImportPlugin(this);
}

SceneImportPlugin::~SceneImportPlugin()
{
}


SceneExportPlugin::SceneExportPlugin()
{
	TheWorldEditor->RegisterSceneExportPlugin(this);
}

SceneExportPlugin::~SceneExportPlugin()
{
}


SceneImportPicker::SceneImportPicker(const char *title, SceneImportType type, const char *subdir, const char *panel) : FilePicker('IMPT', title, ThePluginMgr->GetImportCatalog(), nullptr, nullptr, 0, (panel) ? panel : "WorldEditor/ImportScene")
{
	importType = type;
	if (subdir) importSubdir = subdir;
	else importSubdir[0] = 0;
}

SceneImportPicker::~SceneImportPicker()
{
}

SceneImportPlugin *SceneImportPicker::GetSceneImportPlugin(void) const
{
	return ((*TheWorldEditor->GetSceneImportPluginList())[importPluginMenu->GetSelection()]);
}

void SceneImportPicker::Preprocess(void)
{
	FilePicker::Preprocess();
	
	importPluginMenu = static_cast<PopupMenuWidget *>(FindWidget("Plugin"));
	
	const SceneImportPlugin *defaultPlugin = TheWorldEditor->GetSceneImportPluginList()->First();
	if (defaultPlugin)
	{
		const SceneImportPlugin *plugin = defaultPlugin;
		do
		{
			importPluginMenu->AppendMenuItem(new MenuItemWidget(plugin->GetPluginName()));
			plugin = plugin->Next();
		} while (plugin);
		
		importPluginMenu->SetSelection(0);
		
		SetResourceDescriptor(defaultPlugin->GetImportResourceDescriptor(importType));
		if (importSubdir[0] != 0) SetCurrentDirectory(ResourcePath(ThePluginMgr->GetImportCatalog()->GetRootPath()) += importSubdir);
	}
	else
	{
		importPluginMenu->Disable();
	}
}

void SceneImportPicker::HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData)
{
	if ((eventData->eventType == kEventWidgetChange) && (widget == importPluginMenu))
	{
		const SceneImportPlugin *plugin = GetSceneImportPlugin();
		SetResourceDescriptor(plugin->GetImportResourceDescriptor(importType));
		if (importSubdir[0] != 0) SetCurrentDirectory(ResourcePath(ThePluginMgr->GetImportCatalog()->GetRootPath()) += importSubdir);
	}
	else
	{
		FilePicker::HandleWidgetEvent(widget, eventData);
	}
}

 
SceneExportPicker::SceneExportPicker(const char *title, const char *subdir, const char *panel) : FilePicker('EXPT', title, ThePluginMgr->GetExportCatalog(), nullptr, nullptr, kFilePickerSave, (panel) ? panel : "WorldEditor/ExportScene")
{ 
	if (subdir) exportSubdir = subdir; 
	else exportSubdir[0] = 0; 
}
 
SceneExportPicker::~SceneExportPicker()
{
}
 
SceneExportPlugin *SceneExportPicker::GetSceneExportPlugin(void) const
{
	return ((*TheWorldEditor->GetSceneExportPluginList())[exportPluginMenu->GetSelection()]);
} 

void SceneExportPicker::Preprocess(void)
{
	FilePicker::Preprocess();
	
	exportPluginMenu = static_cast<PopupMenuWidget *>(FindWidget("Plugin"));
	
	const SceneExportPlugin *defaultPlugin = TheWorldEditor->GetSceneExportPluginList()->First();
	if (defaultPlugin)
	{
		const SceneExportPlugin *plugin = defaultPlugin;
		do
		{
			exportPluginMenu->AppendMenuItem(new MenuItemWidget(plugin->GetPluginName()));
			plugin = plugin->Next();
		} while (plugin);
		
		exportPluginMenu->SetSelection(0);
		
		SetResourceDescriptor(defaultPlugin->GetExportResourceDescriptor());
		if (exportSubdir[0] != 0) SetCurrentDirectory(ResourcePath(ThePluginMgr->GetExportCatalog()->GetRootPath()) += exportSubdir);
	}
	else
	{
		exportPluginMenu->Disable();
	}
}

void SceneExportPicker::HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData)
{
	if ((eventData->eventType == kEventWidgetActivate) && (widget == exportPluginMenu))
	{
		const SceneExportPlugin *plugin = GetSceneExportPlugin();
		SetResourceDescriptor(plugin->GetExportResourceDescriptor());
		if (exportSubdir[0] != 0) SetCurrentDirectory(ResourcePath(ThePluginMgr->GetImportCatalog()->GetRootPath()) += exportSubdir);
	}
	else
	{
		FilePicker::HandleWidgetEvent(widget, eventData);
	}
}

// ZYURVUR
