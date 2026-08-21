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


#ifndef C4Plugins_h
#define C4Plugins_h


#include "C4Input.h"
#include "C4FilePicker.h"


namespace C4
{
	struct TargaHeader
	{
		unsigned_int8		idLength;
		unsigned_int8		colorMapType;
		unsigned_int8		imageType;
		unsigned_int8		colorMapStartL;
		unsigned_int8		colorMapStartH;
		unsigned_int8		colorMapLengthL;
		unsigned_int8		colorMapLengthH;
		unsigned_int8		colorMapDepth;
		int16				xOffset;
		int16				yOffset;
		int16				width;
		int16				height;
		unsigned_int8		pixelDepth;
		unsigned_int8		imageDescriptor;
		
		const unsigned_int8 *GetPixelData(void) const
		{
			return (reinterpret_cast<const unsigned_int8 *>(this + 1) + idLength);
		};
	};
	
	
	class TargaResource : public Resource<TargaResource>
	{
		friend class Resource<TargaResource>;
		
		private:
			
			static C4API ResourceDescriptor		descriptor;
			
			~TargaResource();
			
			void Preprocess(void);
			
		public:
			
			C4API TargaResource(const char *name, ResourceCatalog *catalog);
			
			const TargaHeader *GetTargaHeader(void) const
			{
				return (static_cast<const TargaHeader *>(GetData()));
			}
	};
	
	
	class Plugin
	{
		private:
			
			#if C4DEBUG
			
				C4API virtual void DebugPlugin(void);
			
			#elif C4OPTIMIZED
				
				C4API virtual void OptimizedPlugin(void);
			
			#elif C4SERVER
				
				C4API virtual void ServerPlugin(void);
			
			#endif
		
		protected:
			
			C4API Plugin();
		
		public:
			
			C4API virtual ~Plugin();
			
			C4API virtual void PluginTask(void);
	};
	
	
	class PluginModule : public Module, public ListElement<PluginModule>
	{
		private:
			
			typedef Plugin *ConstructProc(void);
			
			Plugin		*modulePlugin;
		
		public:
			
			PluginModule();
			~PluginModule();
			
			Plugin *GetPlugin(void) const
			{ 
				return (modulePlugin);
			} 
			 
			EngineResult Load(const char *name); 
	};
	 
	
	class PluginMgr : public Manager<PluginMgr>
	{
		private: 
			
			List<PluginModule>		pluginList;
			
			MenuItemWidget			*topMenuItem; 
			MenuItemWidget			*bottomMenuItem;
			
			CatalogStorage<GenericResourceCatalog>	importCatalog;
			CatalogStorage<GenericResourceCatalog>	exportCatalog;
			
			void LoadPlugins(const char *directory);
			
			void BuildToolsMenu(void);
			
			void HandleConsoleMenuItem(Widget *widget, const WidgetEventData *eventData);
			void HandleRateMenuItem(Widget *widget, const WidgetEventData *eventData);
			void HandleStatsMenuItem(Widget *widget, const WidgetEventData *eventData);
			void HandleNetworkMenuItem(Widget *widget, const WidgetEventData *eventData);
			void HandleScreenshotMenuItem(Widget *widget, const WidgetEventData *eventData);
			void HandleQuitMenuItem(Widget *widget, const WidgetEventData *eventData);
		
		public:
			
			PluginMgr(int);
			~PluginMgr();
			
			EngineResult Construct(void);
			void Destruct(void);
			
			GenericResourceCatalog *GetImportCatalog(void)
			{
				return (importCatalog);
			}
			
			GenericResourceCatalog *GetExportCatalog(void)
			{
				return (exportCatalog);
			}
			
			C4API void AddToolMenuItem(MenuItemWidget *widget, bool end = true);
			
			void PurgePlugins(void);
			void PluginTask(void);
	};
	
	
	C4API extern PluginMgr *ThePluginMgr;
}


#endif

// ZYURVUR
