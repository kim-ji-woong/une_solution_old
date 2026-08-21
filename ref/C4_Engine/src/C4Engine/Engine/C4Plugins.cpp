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


#include "C4Plugins.h"
#include "C4ToolWindows.h"


using namespace C4;


#if C4PLAYSTATION3 //[ PS3

			// -- PlayStation 3 code hidden --

#endif //]


PluginMgr *C4::ThePluginMgr = nullptr;


namespace C4
{
	template <> PluginMgr Manager<PluginMgr>::managerObject(0);
	template <> PluginMgr **Manager<PluginMgr>::managerPointer = &ThePluginMgr;
}


ResourceDescriptor TargaResource::descriptor("tga");


TargaResource::TargaResource(const char *name, ResourceCatalog *catalog) : Resource<TargaResource>(name, catalog)
{
}

TargaResource::~TargaResource()
{
}

void TargaResource::Preprocess(void)
{
	#if C4BIGENDIAN
	
		TargaHeader *header = static_cast<TargaHeader *>(GetData());
		
		Reverse(&header->xOffset);
		Reverse(&header->yOffset);
		Reverse(&header->width);
		Reverse(&header->height);
	
	#endif
}


Plugin::Plugin()
{
}

Plugin::~Plugin()
{
}

#if C4DEBUG

	void Plugin::DebugPlugin(void)
	{
	}

#elif C4OPTIMIZED
	
	void Plugin::OptimizedPlugin(void)
	{
	}

#elif C4SERVER
	
	void Plugin::ServerPlugin(void)
	{
	}

#endif

void Plugin::PluginTask(void)
{
}


PluginModule::PluginModule() : Module(kModulePlugin)
{
	modulePlugin = nullptr;
}

PluginModule::~PluginModule()
{
}

EngineResult PluginModule::Load(const char *name)
{
	EngineResult result = Module::Load(name);
	if (result != kEngineOkay)
		return (result);
	
	#if !C4GAMECONSOLE
	
		ConstructProc *constructor = (ConstructProc *) GetFunctionAddress("ConstructPlugin");
		if (!constructor) return (kEngineModuleConstructMissing);
	 
	#else
	 
		ConstructProc *constructor = &ConstructPlugin; 
	 
	#endif
	 
	Plugin *plugin = (*constructor)();
	if (!plugin)
		return (kEngineModuleInitFailed);
	
	modulePlugin = plugin; 
	return (kEngineOkay);
}

 
PluginMgr::PluginMgr(int)
{
}

PluginMgr::~PluginMgr()
{
}

EngineResult PluginMgr::Construct(void)
{
	importCatalog.Construct("Import/");
	exportCatalog.Construct("Export/");
	
	BuildToolsMenu();
	
	Engine::Report("Plugin Manager", kReportLog | kReportHeading);
	Engine::Report("<table cellspacing=\"0\" cellpadding=\"0\">\r\n", kReportLog);
	
	LoadPlugins("");
	
	Engine::Report("</table>\r\n", kReportLog);
	
	return (kEngineOkay);
}

void PluginMgr::Destruct(void)
{
	pluginList.Purge();
	
	exportCatalog.Destruct();
	importCatalog.Destruct();
}

void PluginMgr::LoadPlugins(const char *directory)
{
	#if !C4GAMECONSOLE
	
		List<FileReference>		fileList;
		
		TheEngine->GetPluginList(directory, &fileList);
		FileReference *reference = fileList.First();
		while (reference)
		{
			if (!(reference->GetFlags() & kFileDirectory))
			{
				String<kMaxFileNameLength> moduleName(directory);
				if (directory[0] != 0) moduleName += '/';
				moduleName += reference->GetName();
				moduleName[Text::GetResourceNameLength(moduleName)] = 0;
				
				Engine::Report("<tr><th>", kReportLog);
				Engine::Report(moduleName, kReportLog);
				Engine::Report("</th><td>\r\n", kReportLog);
				
				PluginModule *pluginModule = new PluginModule;
				EngineResult result = pluginModule->Load(moduleName);

				if (result == kEngineOkay) 
					pluginList.Append(pluginModule);
				else 
					delete pluginModule;
				
				Engine::LogResult(result);
				Engine::Report("</td></tr>\r\n", kReportLog);
			}
			
			reference = reference->Next();
		}
		
		reference = fileList.First();
		while (reference)
		{
			if (reference->GetFlags() & kFileDirectory)
			{
				String<kMaxFileNameLength> directoryName(directory);
				LoadPlugins((directoryName += '/') += reference->GetName());
			}
			
			reference = reference->Next();
		}
	
	#else
	
		PluginModule *pluginModule = new PluginModule;
		pluginModule->Load("Extras");
		pluginList.Append(pluginModule);
	
	#endif
}

void PluginMgr::PurgePlugins(void)
{
	PluginModule *plugin = pluginList.Last();
	while (plugin)
	{
		delete plugin->GetPlugin();
		plugin = plugin->Previous();
	}
}

void PluginMgr::PluginTask(void)
{
	PluginModule *plugin = pluginList.First();
	while (plugin)
	{
		plugin->GetPlugin()->PluginTask();
		plugin = plugin->Next();
	}
}

void PluginMgr::BuildToolsMenu(void)
{
	const StringTable *table = TheInterfaceMgr->GetStringTable();
	
	PulldownMenuWidget *toolsMenu = TheInterfaceMgr->GetToolsMenu();
	
	topMenuItem = nullptr;
	bottomMenuItem = new MenuItemWidget(kLineSolid);
	toolsMenu->AppendMenuItem(bottomMenuItem);
	
	toolsMenu->AppendMenuItem(new MenuItemWidget(table->GetString(StringID('MENU', 'TOOL', 'CONS')), WidgetObserver<PluginMgr>(this, &PluginMgr::HandleConsoleMenuItem)));
	toolsMenu->AppendMenuItem(new MenuItemWidget(table->GetString(StringID('MENU', 'TOOL', 'RATE')), WidgetObserver<PluginMgr>(this, &PluginMgr::HandleRateMenuItem)));
	toolsMenu->AppendMenuItem(new MenuItemWidget(table->GetString(StringID('MENU', 'TOOL', 'STAT')), WidgetObserver<PluginMgr>(this, &PluginMgr::HandleStatsMenuItem)));
	toolsMenu->AppendMenuItem(new MenuItemWidget(table->GetString(StringID('MENU', 'TOOL', 'NTWK')), WidgetObserver<PluginMgr>(this, &PluginMgr::HandleNetworkMenuItem)));
	toolsMenu->AppendMenuItem(new MenuItemWidget(kLineSolid));
	toolsMenu->AppendMenuItem(new MenuItemWidget(table->GetString(StringID('MENU', 'TOOL', 'SHOT')), WidgetObserver<PluginMgr>(this, &PluginMgr::HandleScreenshotMenuItem), Shortcut('\\')));
	toolsMenu->AppendMenuItem(new MenuItemWidget(kLineSolid));
	toolsMenu->AppendMenuItem(new MenuItemWidget(table->GetString(StringID('MENU', 'TOOL', 'QUIT')), WidgetObserver<PluginMgr>(this, &PluginMgr::HandleQuitMenuItem), Shortcut('Q')));
}

void PluginMgr::AddToolMenuItem(MenuItemWidget *widget, bool end)
{
	PulldownMenuWidget *toolsMenu = TheInterfaceMgr->GetToolsMenu();
	
	if (end)
	{
		toolsMenu->InsertMenuItemBefore(widget, bottomMenuItem);
	}
	else if (topMenuItem)
	{
		toolsMenu->InsertMenuItemAfter(widget, topMenuItem);
		topMenuItem = widget;
	}
	else
	{
		topMenuItem = widget;
		toolsMenu->PrependMenuItem(widget);
	}
}

void PluginMgr::HandleConsoleMenuItem(Widget *widget, const WidgetEventData *eventData)
{
	TheInterfaceMgr->SetActiveWindow(TheConsoleWindow);
}

void PluginMgr::HandleRateMenuItem(Widget *widget, const WidgetEventData *eventData)
{
	TheEngine->HandleRateCommand(nullptr, nullptr);
}

void PluginMgr::HandleStatsMenuItem(Widget *widget, const WidgetEventData *eventData)
{
	TheEngine->HandleStatCommand(nullptr, nullptr);
}

void PluginMgr::HandleNetworkMenuItem(Widget *widget, const WidgetEventData *eventData)
{
	TheEngine->HandleNetCommand(nullptr, nullptr);
}

void PluginMgr::HandleScreenshotMenuItem(Widget *widget, const WidgetEventData *eventData)
{
	TheEngine->HandleShotCommand(nullptr, "C4_shot_#");
}

void PluginMgr::HandleQuitMenuItem(Widget *widget, const WidgetEventData *eventData)
{
	TheEngine->HandleQuitCommand(nullptr, nullptr);
}

// ZYURVUR
