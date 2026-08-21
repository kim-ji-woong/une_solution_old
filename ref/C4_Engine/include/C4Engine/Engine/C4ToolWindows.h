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


#ifndef C4ToolWindows_h
#define C4ToolWindows_h


#include "C4Interface.h"
#include "C4Graphics.h"
#include "C4Shaders.h"
#include "C4Network.h"


namespace C4
{
	enum
	{
		kWidgetGraph			= 'GRPH',
		kWidgetFrameBuffer		= 'FBUF'
	};
	
	
	class C4_API GraphWidget : public RenderableWidget
	{
		private:
			
			int32				valueCount;
			Point3D				*vertexArray;
			
			List<Attribute>		attributeList;
			DiffuseAttribute	diffuseAttribute;
		
		public:
			
			GraphWidget(const Vector2D& size, int32 count, const ColorRGBA& color);
			~GraphWidget();
			
			void Preprocess(void);
			
			void AddValue(float value);
	};
	
	
	class C4_API RateWindow : public Window, public Singleton<RateWindow>
	{
		private:
			
			enum
			{
				kRateMeasureFrameCount = 48
			};
			
			TextWidget		*rateText;
			
			int32			frameCount;
			int32			timeTable[kRateMeasureFrameCount];
			
			GraphWidget		rateGraph;
			GraphWidget		renderGraph;
			
			RateWindow();
			
		public:
		
			~RateWindow();
			
			static void Open(void);
			
			void Preprocess(void);
			void Move(void);
			
			bool HandleKeyboardEvent(const KeyboardEventData *eventData);
	};
	
	
	class C4_API StatsWindow : public Window, public Singleton<StatsWindow>
	{
		private:
			
			enum
			{
				kRenderStatDirectVerts,
				kRenderStatDirectPrims,
				kRenderStatDirectCmds,
				kRenderStatShadowVerts,
				kRenderStatShadowPrims,
				kRenderStatShadowCmds,
				kRenderStatStencilVerts,
				kRenderStatStencilPrims,
				kRenderStatStencilCmds,
				kRenderStatVelocityVerts,
				kRenderStatVelocityPrims,
				kRenderStatVelocityCmds,
				kRenderStatDistortionVerts,
				kRenderStatDistortionPrims,
				kRenderStatDistortionCmds,
				kRenderStatTotalVerts,
				kRenderStatTotalPrims,
				kRenderStatTotalCmds,
				kRenderStatTextureCount,
				kRenderStatTextureMemory,
				kRenderStatVertexBufferCount,
				kRenderStatVertexBufferMemory,
				kRenderStatCount
			}; 
			
			enum 
			{ 
				kWorldStatLightCount, 
				kWorldStatGeometryCount,
				kWorldStatTerrainCount, 
				kWorldStatWaterCount,
				kWorldStatImpostorCount,
				kWorldStatDepthShadowCount,
				kWorldStatStencilShadowCount, 
				kWorldStatShadowSections,
				kWorldStatStencilClears,
				kWorldStatDirectPortals,
				kWorldStatRemotePortals, 
				kWorldStatOcclusionRegions,
				kWorldStatPlayingSources,
				kWorldStatEngagedSources,
				kWorldStatRunningScripts,
				kWorldStatCount
			};
			
			enum
			{
				kPhysicsStatRigidBodyCount,
				kPhysicsStatClothMoveCount,
				kPhysicsStatClothUpdateCount,
				kPhysicsStatWaterMoveCount,
				kPhysicsStatWaterUpdateCount,
				kPhysicsStatBuoyancyCount,
				kPhysicsStatGeometryIntersections,
				kPhysicsStatRigidBodyIntersections,
				kPhysicsStatCount
			};
			
			MultipaneWidget		*paneWidget;
			Widget				*renderGroup;
			Widget				*worldGroup;
			Widget				*physicsGroup;
			
			TextWidget			*renderStatText[kRenderStatCount];
			TextWidget			*worldStatText[kWorldStatCount];
			TextWidget			*physicsStatText[kPhysicsStatCount];
			
			StatsWindow();
		
		public:
			
			~StatsWindow();
			
			static void Open(void);
			
			void Preprocess(void);
			void Move(void);
			
			bool HandleKeyboardEvent(const KeyboardEventData *eventData);
			void HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData);
	};
	
	
	class C4_API FrameBufferWidget : public RenderableWidget
	{
		private:
			
			List<Attribute>		attributeList;
			ShaderAttribute		shaderAttribute;
			
			Point3D				vertexArray[16];
			Point2D				texcoordArray[16];
		
		public:
			
			FrameBufferWidget(const Vector2D& size);
			~FrameBufferWidget();
			
			void Preprocess(void);
			void Build(void);
	};
	
	
	class C4_API ShadowMapWindow : public Window, public Singleton<ShadowMapWindow>
	{
		private:
			
			FrameBufferWidget		frameBufferWidget;
			
			ShadowMapWindow();
		
		public:
			
			~ShadowMapWindow();
			
			static void Open(void);
			
			bool HandleKeyboardEvent(const KeyboardEventData *eventData);
	};
	
	
	class C4_API NetworkWindow : public Window, public Singleton<NetworkWindow>
	{
		private:
			
			TextWidget		*connectionText;
			TextWidget		*chatRateText;
			
			GraphWidget		*incomingGraph[kPacketTypeCount];
			GraphWidget		*outgoingGraph[kPacketTypeCount];
			
			NetworkWindow();
			
		public:
			
			~NetworkWindow();
			
			static void Open(void);
			
			void Preprocess(void);
			void Move(void);
			
			bool HandleKeyboardEvent(const KeyboardEventData *eventData);
	};
	
	
	class C4_API ExtensionsWindow : public Window, public Singleton<ExtensionsWindow>
	{
		private:
			
			PushButtonWidget		*okayButton;
			PushButtonWidget		*cancelButton;
			ListWidget				*extensionsList;
			
			CheckWidget				*enableBox[kGraphicsExtensionCount];
			
			#if C4WINDOWS || C4LINUX
			
				CheckWidget			*windowSystemEnableBox[kWindowSystemExtensionCount];
			
			#endif
			
			ExtensionsWindow();
		
		public:
			
			~ExtensionsWindow();
			
			static void Open(void);
			
			void Preprocess(void);
			
			bool HandleKeyboardEvent(const KeyboardEventData *eventData);
			void HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData);
	};
	
	
	class C4_API ConsoleWindow : public Window, public Singleton<ConsoleWindow>
	{
		private:
			
			enum
			{
				kConsoleLineCount				= 17,
				kConsoleHistoryCount			= 256,
				kConsoleCommandHistoryCount		= 8
			};
			
			bool						dirtyFlag;
			
			TextWidget					*textWidget;
			EditTextWidget				*commandLine;
			ScrollWidget				*scrollWidget;
			
			Reporter					reporter;
			
			unsigned_int32				commandHistoryCount;
			unsigned_int32				commandHistoryStart;
			unsigned_int32				commandHistoryOffset;
			String<kMaxCommandLength>	commandHistory[kConsoleCommandHistoryCount];
			
			ConsoleWindow();
			
			static void ConsoleProc(void *cookie);
			static void Report(const char *text, unsigned_int32 flags, void *cookie);
			
			void UpdateDisplayLine(void)
			{
				textWidget->SetFirstRenderLine(scrollWidget->GetValue());
			}
		
		public:
			
			~ConsoleWindow();
			
			static void New(void);
			
			void Preprocess(void);
			void EnterForeground(void);
			void Close(void);
			
			bool HandleKeyboardEvent(const KeyboardEventData *eventData);
			void HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData);
			
			void Render(List<Renderable> *renderList);
			
			void AddText(const char *text = nullptr);
	};
	
	
	C4_API extern RateWindow *TheRateWindow;
	C4_API extern StatsWindow *TheStatsWindow;
	C4_API extern NetworkWindow *TheNetworkWindow;
	C4_API extern ShadowMapWindow *TheShadowMapWindow;
	C4_API extern ExtensionsWindow *TheExtensionsWindow;
	C4_API extern ConsoleWindow *TheConsoleWindow;
}


#endif

// ZYURVUR
