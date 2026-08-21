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


#include "C4ToolWindows.h"
#include "C4AudioCapture.h"
#include "C4Input.h"
#include "C4World.h"


using namespace C4;


RateWindow *C4::TheRateWindow = nullptr;
StatsWindow *C4::TheStatsWindow = nullptr;
ShadowMapWindow *C4::TheShadowMapWindow = nullptr;
NetworkWindow *C4::TheNetworkWindow = nullptr;
ExtensionsWindow *C4::TheExtensionsWindow = nullptr;
ConsoleWindow *C4::TheConsoleWindow = nullptr;


GraphWidget::GraphWidget(const Vector2D& size, int32 count, const ColorRGBA& color) :
		RenderableWidget(kWidgetGraph, kRenderQuads, size),
		diffuseAttribute(color)
{
	valueCount = count;
	vertexArray = new Point3D[count * 4];
}

GraphWidget::~GraphWidget()
{
	delete[] vertexArray;
}

void GraphWidget::Preprocess(void)
{
	RenderableWidget::Preprocess();
	
	attributeList.Append(&diffuseAttribute);
	SetMaterialAttributeList(&attributeList);
	
	int32 count = valueCount;
	SetVertexCount(count * 4);
	SetAttributeArray(kArrayVertex, vertexArray);
	
	float x1 = 0.0F;
	float x2 = 1.0F;
	float h = GetWidgetSize().y;
	
	Point3D *vertex = vertexArray;
	for (machine a = 0; a < count; a++)
	{
		vertex[0].Set(x1, h, 0.0F);
		vertex[1].Set(x1, h, 0.0F);
		vertex[2].Set(x2, h, 0.0F);
		vertex[3].Set(x2, h, 0.0F);
		vertex += 4;
		
		x1 = x2;
		x2 = x2 + 1.0F;
	}
}

void GraphWidget::AddValue(float value)
{
	for (machine a = 1; a < valueCount; a++)
	{
		int32 index = a * 4;
		float y = vertexArray[index].y;
		vertexArray[index - 4].y = y;
		vertexArray[index - 1].y = y;
	}
	
	value = GetWidgetSize().y - value;
	int32 index = valueCount * 4 - 4;
	vertexArray[index].y = value;
	vertexArray[index + 3].y = value;
}


RateWindow::RateWindow() :
		Window("C4/Rate"),
		Singleton<RateWindow>(TheRateWindow),
		rateGraph(Vector2D((float) kRateMeasureFrameCount, 30.0F), kRateMeasureFrameCount, ColorRGBA(0.0F, 1.0F, 0.0F, 1.0F)),
		renderGraph(Vector2D((float) kRateMeasureFrameCount, 30.0F), kRateMeasureFrameCount, ColorRGBA(1.0F, 1.0F, 0.0F, 1.0F))
{
	SetWidgetPosition(Point3D(TheInterfaceMgr->GetDesktopSize().x - GetWidgetSize().x - 10.0F, 26.0F, 0.0F));
	
	rateGraph.SetWidgetPosition(Point3D(7.0F, 22.0F, 0.0F));
	AddSubnode(&rateGraph);
	
	if (TheGraphicsMgr->GetCapabilities()->extensionFlag[kExtensionTimerQuery])
	{
		renderGraph.SetWidgetPosition(Point3D(6.0F, 76.0F, 0.0F));
		AddSubnode(&renderGraph);
	}
}

RateWindow::~RateWindow()
{
}

void RateWindow::Open(void)
{
	if (TheRateWindow) TheInterfaceMgr->SetActiveWindow(TheRateWindow);
	else TheInterfaceMgr->AddWidget(new RateWindow); 
}
 
void RateWindow::Preprocess(void) 
{ 
	Window::Preprocess();
	 
	rateText = static_cast<TextWidget *>(FindWidget("FPS"));
	
	frameCount = 0;
	int32 time = TheTimeMgr->GetSystemAbsoluteTime(); 
	for (machine a = 0; a < kRateMeasureFrameCount; a++) timeTable[a] = time;
}

void RateWindow::Move(void) 
{
	unsigned_int32 time = TheTimeMgr->GetSystemAbsoluteTime();
	for (machine a = 0; a < kRateMeasureFrameCount - 1; a++) timeTable[a] = timeTable[a + 1];
	timeTable[kRateMeasureFrameCount - 1] = time;
	
	if (frameCount >= kRateMeasureFrameCount)
	{
		int32 totalTime = time - timeTable[0];
		if (totalTime != 0)
		{
			static char s[5] = "00.0";
			static char t[5] = " 000";
			
			int32 fps = (kRateMeasureFrameCount - 1) * 10000 / totalTime;
			if (fps < 1000)
			{
				s[0] = (char) (fps / 100 % 10 + 48);
				s[1] = (char) (fps / 10 % 10 + 48);
				s[3] = (char) (fps % 10 + 48);
				rateText->SetText(s);
			}
			else
			{
				if (fps < 10000) t[0] = ' ';
				else t[0] = (char) (fps / 10000 % 10 + 48);
				t[1] = (char) (fps / 1000 % 10 + 48);
				t[2] = (char) (fps / 100 % 10 + 48);
				t[3] = (char) (fps / 10 % 10 + 48);
				rateText->SetText(t);
			}
		}
	}
	else
	{
		frameCount++;
	}
	
	int32 dt = timeTable[kRateMeasureFrameCount - 1] - timeTable[kRateMeasureFrameCount - 2];
	rateGraph.AddValue((float) (Min(dt, 60) >> 1));
	
	time = (unsigned_int32) (TheGraphicsMgr->GetRenderingTime() / 1000000);
	renderGraph.AddValue((float) (Min(time, 60) >> 1));
	
	TheGraphicsMgr->SetDiagnosticFlags(TheGraphicsMgr->GetDiagnosticFlags() | kDiagnosticTimer);
}

bool RateWindow::HandleKeyboardEvent(const KeyboardEventData *eventData)
{
	if (eventData->eventType == kEventKeyCommand)
	{
		if (eventData->keyCode == 'W')
		{
			Close();
			return (true);
		}
	}
	
	return (Window::HandleKeyboardEvent(eventData));
}


StatsWindow::StatsWindow() :
		Window("C4/Stats"),
		Singleton<StatsWindow>(TheStatsWindow)
{
	const Vector2D& desktopSize = TheInterfaceMgr->GetDesktopSize();
	SetWidgetPosition(Point3D(desktopSize.x - GetWidgetSize().x - 10.0F, desktopSize.y - GetWidgetSize().y - 10.0F, 0.0F));
}

StatsWindow::~StatsWindow()
{
}

void StatsWindow::Open(void)
{
	if (TheStatsWindow) TheInterfaceMgr->SetActiveWindow(TheStatsWindow);
	else TheInterfaceMgr->AddWidget(new StatsWindow);
}

void StatsWindow::Preprocess(void)
{
	static const char *const renderKey[kRenderStatCount] =
	{
		"RenderVerts", "RenderPrims", "RenderCmds", "ShadowVerts", "ShadowPrims", "ShadowCmds", "StencilVerts", "StencilPrims", "StencilCmds",
		"VelocityVerts", "VelocityPrims", "VelocityCmds", "DistortVerts", "DistortPrims", "DistortCmds", "TotalVerts", "TotalPrims", "TotalCmds",
		"Textures", "TextureMem", "VBOs", "VertexMem"
	};
	
	static const char *const worldKey[kWorldStatCount] =
	{
		"Lights", "Geometries", "Terrain", "Water", "Impostors", "Shadows", "Stencils", "Sections", "Clears",
		"DirectPortals", "RemotePortals", "Occlusion", "PlayingSources", "EngagedSources", "Scripts"
	};
	
	static const char *const physicsKey[kPhysicsStatCount] =
	{
		"RigidBodies", "ClothMove", "ClothUpdate", "WaterMove", "WaterUpdate", "Buoyancy", "GeomIntersect", "BodyIntersect"
	};
	
	Window::Preprocess();
	
	paneWidget = static_cast<MultipaneWidget *>(FindWidget("Pane"));
	renderGroup = FindWidget("Render");
	worldGroup = FindWidget("World");
	physicsGroup = FindWidget("Physics");
	
	for (machine a = 0; a < kRenderStatCount; a++) renderStatText[a] = static_cast<TextWidget *>(FindWidget(renderKey[a]));
	for (machine a = 0; a < kWorldStatCount; a++) worldStatText[a] = static_cast<TextWidget *>(FindWidget(worldKey[a]));
	for (machine a = 0; a < kPhysicsStatCount; a++) physicsStatText[a] = static_cast<TextWidget *>(FindWidget(physicsKey[a]));
}

void StatsWindow::Move(void)
{
	int32 pane = paneWidget->GetSelection();
	if (pane == 0)
	{
		int32 vertexTotal = 0;
		int32 primitiveTotal = 0;
		int32 commandTotal = 0;
		
		int32 vertexCount = TheGraphicsMgr->GetGraphicsCounter(kGraphicsCounterDirectVertices);
		int32 primitiveCount = TheGraphicsMgr->GetGraphicsCounter(kGraphicsCounterDirectPrimitives);
		int32 commandCount = TheGraphicsMgr->GetGraphicsCounter(kGraphicsCounterDirectCommands);
		
		vertexTotal += vertexCount;
		primitiveTotal += primitiveCount;
		commandTotal += commandCount;
		
		renderStatText[kRenderStatDirectVerts]->SetText(String<7>(vertexCount));
		renderStatText[kRenderStatDirectPrims]->SetText(String<7>(primitiveCount));
		renderStatText[kRenderStatDirectCmds]->SetText(String<7>(commandCount));
		
		vertexCount = TheGraphicsMgr->GetGraphicsCounter(kGraphicsCounterShadowVertices);
		primitiveCount = TheGraphicsMgr->GetGraphicsCounter(kGraphicsCounterShadowPrimitives);
		commandCount = TheGraphicsMgr->GetGraphicsCounter(kGraphicsCounterShadowCommands);
		
		vertexTotal += vertexCount;
		primitiveTotal += primitiveCount;
		commandTotal += commandCount;
		
		renderStatText[kRenderStatShadowVerts]->SetText(String<7>(vertexCount));
		renderStatText[kRenderStatShadowPrims]->SetText(String<7>(primitiveCount));
		renderStatText[kRenderStatShadowCmds]->SetText(String<7>(commandCount));
		
		vertexCount = TheGraphicsMgr->GetGraphicsCounter(kGraphicsCounterStencilVertices);
		primitiveCount = TheGraphicsMgr->GetGraphicsCounter(kGraphicsCounterStencilPrimitives);
		commandCount = TheGraphicsMgr->GetGraphicsCounter(kGraphicsCounterStencilCommands);
		
		vertexTotal += vertexCount;
		primitiveTotal += primitiveCount;
		commandTotal += commandCount;
		
		renderStatText[kRenderStatStencilVerts]->SetText(String<7>(vertexCount));
		renderStatText[kRenderStatStencilPrims]->SetText(String<7>(primitiveCount));
		renderStatText[kRenderStatStencilCmds]->SetText(String<7>(commandCount));
		
		vertexCount = TheGraphicsMgr->GetGraphicsCounter(kGraphicsCounterVelocityVertices);
		primitiveCount = TheGraphicsMgr->GetGraphicsCounter(kGraphicsCounterVelocityPrimitives);
		commandCount = TheGraphicsMgr->GetGraphicsCounter(kGraphicsCounterVelocityCommands);
		
		vertexTotal += vertexCount;
		primitiveTotal += primitiveCount;
		commandTotal += commandCount;
		
		renderStatText[kRenderStatVelocityVerts]->SetText(String<7>(vertexCount));
		renderStatText[kRenderStatVelocityPrims]->SetText(String<7>(primitiveCount));
		renderStatText[kRenderStatVelocityCmds]->SetText(String<7>(commandCount));
		
		vertexCount = TheGraphicsMgr->GetGraphicsCounter(kGraphicsCounterDistortionVertices);
		primitiveCount = TheGraphicsMgr->GetGraphicsCounter(kGraphicsCounterDistortionPrimitives);
		commandCount = TheGraphicsMgr->GetGraphicsCounter(kGraphicsCounterDistortionCommands);
		
		vertexTotal += vertexCount;
		primitiveTotal += primitiveCount;
		commandTotal += commandCount;
		
		renderStatText[kRenderStatDistortionVerts]->SetText(String<7>(vertexCount));
		renderStatText[kRenderStatDistortionPrims]->SetText(String<7>(primitiveCount));
		renderStatText[kRenderStatDistortionCmds]->SetText(String<7>(commandCount));
		
		renderStatText[kRenderStatTotalVerts]->SetText(String<7>(vertexTotal));
		renderStatText[kRenderStatTotalPrims]->SetText(String<7>(primitiveTotal));
		renderStatText[kRenderStatTotalCmds]->SetText(String<7>(commandTotal));
		
		renderStatText[kRenderStatTextureCount]->SetText(String<7>(Texture::GetTotalTextureCount()));
		renderStatText[kRenderStatTextureMemory]->SetText(String<7>((Texture::GetTotalTextureMemory() + 0x03FF) >> 10));
		renderStatText[kRenderStatVertexBufferCount]->SetText(String<7>(VertexBuffer::GetTotalVertexBufferCount()));
		renderStatText[kRenderStatVertexBufferMemory]->SetText(String<7>((VertexBuffer::GetTotalVertexBufferMemory() + 0x03FF) >> 10));
	}
	else if (pane == 1)
	{
		worldStatText[kWorldStatStencilClears]->SetText(String<7>(TheGraphicsMgr->GetGraphicsCounter(kGraphicsCounterStencilClears)));
		
		const World *world = TheWorldMgr->GetWorld();
		if (world)
		{
			worldStatText[kWorldStatLightCount]->SetText(String<7>(world->GetWorldCounter(kWorldCounterLight)));
			worldStatText[kWorldStatGeometryCount]->SetText(String<7>(world->GetWorldCounter(kWorldCounterGeometry)));
			worldStatText[kWorldStatTerrainCount]->SetText(String<7>(world->GetWorldCounter(kWorldCounterTerrain)));
			worldStatText[kWorldStatWaterCount]->SetText(String<7>(world->GetWorldCounter(kWorldCounterWater)));
			worldStatText[kWorldStatImpostorCount]->SetText(String<7>(world->GetWorldCounter(kWorldCounterImpostor)));
			worldStatText[kWorldStatDepthShadowCount]->SetText(String<7>(world->GetWorldCounter(kWorldCounterDepthShadow)));
			worldStatText[kWorldStatStencilShadowCount]->SetText(String<7>(world->GetWorldCounter(kWorldCounterStencilShadow)));
			worldStatText[kWorldStatShadowSections]->SetText(String<7>(world->GetWorldCounter(kWorldCounterShadowSection)));
			worldStatText[kWorldStatDirectPortals]->SetText(String<7>(world->GetWorldCounter(kWorldCounterDirectPortal)));
			worldStatText[kWorldStatRemotePortals]->SetText(String<7>(world->GetWorldCounter(kWorldCounterRemotePortal)));
			worldStatText[kWorldStatOcclusionRegions]->SetText(String<7>(world->GetWorldCounter(kWorldCounterOcclusionRegion)));
			worldStatText[kWorldStatPlayingSources]->SetText(String<7>(world->GetWorldCounter(kWorldCounterPlayingSource)));
			worldStatText[kWorldStatEngagedSources]->SetText(String<7>(world->GetWorldCounter(kWorldCounterEngagedSource)));
			worldStatText[kWorldStatRunningScripts]->SetText(String<7>(world->GetWorldCounter(kWorldCounterRunningScript)));
		}
		else
		{
			worldStatText[kWorldStatLightCount]->SetText("0");
			worldStatText[kWorldStatGeometryCount]->SetText("0");
			worldStatText[kWorldStatTerrainCount]->SetText("0");
			worldStatText[kWorldStatWaterCount]->SetText("0");
			worldStatText[kWorldStatImpostorCount]->SetText("0");
			worldStatText[kWorldStatDepthShadowCount]->SetText("0");
			worldStatText[kWorldStatStencilShadowCount]->SetText("0");
			worldStatText[kWorldStatShadowSections]->SetText("0");
			worldStatText[kWorldStatDirectPortals]->SetText("0");
			worldStatText[kWorldStatRemotePortals]->SetText("0");
			worldStatText[kWorldStatOcclusionRegions]->SetText("0");
			worldStatText[kWorldStatPlayingSources]->SetText("0");
			worldStatText[kWorldStatEngagedSources]->SetText("0");
			worldStatText[kWorldStatRunningScripts]->SetText("0");
		}
	}
	else if (pane == 2)
	{
		const World *world = TheWorldMgr->GetWorld();
		if (world)
		{
			int32 rigidBodyTotal = 0;
			int32 buoyancyTotal = 0;
			int32 geometryIntersectionTotal = 0;
			int32 rigidBodyIntersectionTotal = 0;
			
			const PhysicsNode *node = world->GetRootNode()->GetPhysicsNode();
			if (node)
			{
				const Controller *controller = node->GetController();
				if ((controller) && (controller->GetControllerType() == kControllerPhysics))
				{
					const PhysicsController *physicsController = static_cast<const PhysicsController *>(controller);
					rigidBodyTotal = physicsController->GetPhysicsCounter(kPhysicsCounterRigidBody);
					buoyancyTotal = physicsController->GetPhysicsCounter(kPhysicsCounterBuoyancy);
					geometryIntersectionTotal = physicsController->GetPhysicsCounter(kPhysicsCounterGeometryIntersection);
					rigidBodyIntersectionTotal = physicsController->GetPhysicsCounter(kPhysicsCounterShapeIntersection);
				}
			}
			
			physicsStatText[kPhysicsStatRigidBodyCount]->SetText(String<7>(rigidBodyTotal));
			physicsStatText[kPhysicsStatBuoyancyCount]->SetText(String<7>(buoyancyTotal));
			physicsStatText[kPhysicsStatGeometryIntersections]->SetText(String<7>(geometryIntersectionTotal));
			physicsStatText[kPhysicsStatRigidBodyIntersections]->SetText(String<7>(rigidBodyIntersectionTotal));
			
			physicsStatText[kPhysicsStatClothMoveCount]->SetText(String<7>(world->GetWorldCounter(kWorldCounterClothMove)));
			physicsStatText[kPhysicsStatClothUpdateCount]->SetText(String<7>(world->GetWorldCounter(kWorldCounterClothUpdate)));
			physicsStatText[kPhysicsStatWaterMoveCount]->SetText(String<7>(world->GetWorldCounter(kWorldCounterWaterMove)));
			physicsStatText[kPhysicsStatWaterUpdateCount]->SetText(String<7>(world->GetWorldCounter(kWorldCounterWaterUpdate)));
		}
		else
		{
			physicsStatText[kPhysicsStatRigidBodyCount]->SetText("0");
			physicsStatText[kPhysicsStatBuoyancyCount]->SetText("0");
			physicsStatText[kPhysicsStatGeometryIntersections]->SetText("0");
			physicsStatText[kPhysicsStatRigidBodyIntersections]->SetText("0");
			physicsStatText[kPhysicsStatClothMoveCount]->SetText("0");
			physicsStatText[kPhysicsStatClothUpdateCount]->SetText("0");
			physicsStatText[kPhysicsStatWaterMoveCount]->SetText("0");
			physicsStatText[kPhysicsStatWaterUpdateCount]->SetText("0");
		}
	}
}

bool StatsWindow::HandleKeyboardEvent(const KeyboardEventData *eventData)
{
	if (eventData->eventType == kEventKeyCommand)
	{
		if (eventData->keyCode == 'W')
		{
			Close();
			return (true);
		}
	}
	
	return (Window::HandleKeyboardEvent(eventData));
}

void StatsWindow::HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		if (widget == paneWidget)
		{
			int32 pane = paneWidget->GetSelection();
			if (pane == 0)
			{
				renderGroup->Show();
				worldGroup->Hide();
				physicsGroup->Hide();
			}
			else if (pane == 1)
			{
				renderGroup->Hide();
				worldGroup->Show();
				physicsGroup->Hide();
			}
			else if (pane == 2)
			{
				renderGroup->Hide();
				worldGroup->Hide();
				physicsGroup->Show();
			}
		}
	}
}


FrameBufferWidget::FrameBufferWidget(const Vector2D& size) : RenderableWidget(kWidgetFrameBuffer, kRenderQuads, size)
{
}

FrameBufferWidget::~FrameBufferWidget()
{
}

void FrameBufferWidget::Preprocess(void)
{
	RenderableWidget::Preprocess();
	
	SetAmbientBlendState(kBlendReplace);
	
	SetVertexCount(16);
	SetAttributeArray(kArrayVertex, vertexArray);
	SetAttributeArray(kArrayTexture0, texcoordArray);
	
	attributeList.Append(&shaderAttribute);
	SetMaterialAttributeList(&attributeList);
	
	ShaderGraph *shaderGraph = shaderAttribute.GetShaderGraph(kShaderGraphAmbient);
	
	Process *process1 = new FrameBufferProcess;
	Process *process2 = new AmbientOutputProcess;
	shaderGraph->AddElement(process1);
	shaderGraph->AddElement(process2);
	new Route(process1, process2, 0);
	
	texcoordArray[0].Set(0.0F, 0.25F);
	texcoordArray[1].Set(0.0F, 0.0F);
	texcoordArray[2].Set(1.0F, 0.0F);
	texcoordArray[3].Set(1.0F, 0.25F);
	
	texcoordArray[4].Set(0.0F, 0.5F);
	texcoordArray[5].Set(0.0F, 0.25F);
	texcoordArray[6].Set(1.0F, 0.25F);
	texcoordArray[7].Set(1.0F, 0.5F);
	
	texcoordArray[8].Set(0.0F, 0.75F);
	texcoordArray[9].Set(0.0F, 0.5F);
	texcoordArray[10].Set(1.0F, 0.5F);
	texcoordArray[11].Set(1.0F, 0.75F);
	
	texcoordArray[12].Set(0.0F, 1.0F);
	texcoordArray[13].Set(0.0F, 0.75F);
	texcoordArray[14].Set(1.0F, 0.75F);
	texcoordArray[15].Set(1.0F, 1.0F);
}

void FrameBufferWidget::Build(void)
{
	float w = GetWidgetSize().x;
	float h = GetWidgetSize().y * 0.25F;
	
	vertexArray[0].Set(0.0F, 0.0F, 0.0F);
	vertexArray[1].Set(0.0F, h, 0.0F);
	vertexArray[2].Set(w, h, 0.0F);
	vertexArray[3].Set(w, 0.0F, 0.0F);
	
	vertexArray[4].Set(0.0F, h, 0.0F);
	vertexArray[5].Set(0.0F, h * 2.0F, 0.0F);
	vertexArray[6].Set(w, h * 2.0F, 0.0F);
	vertexArray[7].Set(w, h, 0.0F);
	
	vertexArray[8].Set(0.0F, h * 2.0F, 0.0F);
	vertexArray[9].Set(0.0F, h * 3.0F, 0.0F);
	vertexArray[10].Set(w, h * 3.0F, 0.0F);
	vertexArray[11].Set(w, h * 2.0F, 0.0F);
	
	vertexArray[12].Set(0.0F, h * 3.0F, 0.0F);
	vertexArray[13].Set(0.0F, h * 4.0F, 0.0F);
	vertexArray[14].Set(w, h * 4.0F, 0.0F);
	vertexArray[15].Set(w, h * 3.0F, 0.0F);
}


ShadowMapWindow::ShadowMapWindow() :
		Window(Vector2D(200.0F, 800.0F), "Shadow Map", kWindowCloseBox | kWindowPassive),
		Singleton<ShadowMapWindow>(TheShadowMapWindow),
		frameBufferWidget(Vector2D(200.0F, 800.0F))
{
	SetWidgetPosition(Point3D(8.0F, 26.0F, 0.0F));
	
	AddSubnode(&frameBufferWidget);
}

ShadowMapWindow::~ShadowMapWindow()
{
}

void ShadowMapWindow::Open(void)
{
	if (TheShadowMapWindow) TheInterfaceMgr->SetActiveWindow(TheShadowMapWindow);
	else TheInterfaceMgr->AddWidget(new ShadowMapWindow);
}

bool ShadowMapWindow::HandleKeyboardEvent(const KeyboardEventData *eventData)
{
	if (eventData->eventType == kEventKeyCommand)
	{
		if (eventData->keyCode == 'W')
		{
			Close();
			return (true);
		}
	}
	
	return (Window::HandleKeyboardEvent(eventData));
}


NetworkWindow::NetworkWindow() :
		Window("C4/Network"),
		Singleton<NetworkWindow>(TheNetworkWindow)
{
	SetWidgetPosition(Point3D(8.0F, TheInterfaceMgr->GetDesktopSize().y - GetWidgetSize().y - 10.0F, 0.0F));
}

NetworkWindow::~NetworkWindow()
{
}

void NetworkWindow::Open(void)
{
	if (TheNetworkWindow) TheInterfaceMgr->SetActiveWindow(TheNetworkWindow);
	else TheInterfaceMgr->AddWidget(new NetworkWindow);
}

void NetworkWindow::Preprocess(void)
{
	Window::Preprocess();
	
	connectionText = static_cast<TextWidget *>(FindWidget("Connect"));
	chatRateText = static_cast<TextWidget *>(FindWidget("Chat"));
	
	char inKey[4] = "IN0";
	char outKey[5] = "OUT0";
	
	for (machine a = 0; a < kPacketTypeCount; a++)
	{
		inKey[2] = outKey[3] = (char) (a + 48);
		
		const Widget *inWidget = FindWidget(inKey);
		const Widget *outWidget = FindWidget(outKey);
		
		GraphWidget *graph = new GraphWidget(inWidget->GetWidgetSize(), 32, ColorRGBA(0.5F, 1.0F, 0.0F, 1.0F));
		graph->SetWidgetPosition(inWidget->GetWidgetPosition());
		incomingGraph[a] = graph;
		AddNewSubnode(graph);
		
		graph = new GraphWidget(outWidget->GetWidgetSize(), 32, ColorRGBA(1.0F, 0.5F, 0.0F, 1.0F));
		graph->SetWidgetPosition(outWidget->GetWidgetPosition());
		outgoingGraph[a] = graph;
		AddNewSubnode(graph);
	}
}

void NetworkWindow::Move(void)
{
	connectionText->SetText(String<7>(TheNetworkMgr->GetConnectionCount()));
	chatRateText->SetText(String<7>(TheAudioCaptureMgr->GetChatReceiveRate()));
	
	for (machine a = 0; a < kPacketTypeCount; a++)
	{
		incomingGraph[a]->AddValue((float) Min(TheNetworkMgr->GetIncomingPacketCounter(a), 32));
		outgoingGraph[a]->AddValue((float) Min(TheNetworkMgr->GetOutgoingPacketCounter(a), 32));
	}
}

bool NetworkWindow::HandleKeyboardEvent(const KeyboardEventData *eventData)
{
	if (eventData->eventType == kEventKeyCommand)
	{
		if (eventData->keyCode == 'W')
		{
			Close();
			return (true);
		}
	}
	
	return (Window::HandleKeyboardEvent(eventData));
}


ExtensionsWindow::ExtensionsWindow() :
		Window("C4/Extensions"),
		Singleton<ExtensionsWindow>(TheExtensionsWindow)
{
}

ExtensionsWindow::~ExtensionsWindow()
{
}

void ExtensionsWindow::Open(void)
{
	if (TheExtensionsWindow) TheInterfaceMgr->SetActiveWindow(TheExtensionsWindow);
	else TheInterfaceMgr->AddWidget(new ExtensionsWindow);
}

void ExtensionsWindow::Preprocess(void)
{
	Window::Preprocess();
	
	okayButton = static_cast<PushButtonWidget *>(FindWidget("OK"));
	cancelButton = static_cast<PushButtonWidget *>(FindWidget("Cancel"));
	
	extensionsList = static_cast<ListWidget *>(FindWidget("List"));
	float width = extensionsList->GetWidgetSize().x - 16.0F;
	
	const GraphicsExtensionData *extensionData = GraphicsMgr::GetExtensionData();
	for (machine a = 0; a < kGraphicsExtensionCount; a++)
	{
		enableBox[a] = new CheckWidget(Vector2D(width, 16.0F), extensionData->name1, "font/Gui");
		extensionsList->AppendListItem(enableBox[a]);
		if (extensionData->enabled) enableBox[a]->SetValue(1);
		if (!extensionData->required) enableBox[a]->Enable();
		
		extensionData++;
	}
	
	#if C4WINDOWS || C4LINUX
	
		const WindowSystemExtensionData *windowSystemExtensionData = GraphicsMgr::GetWindowSystemExtensionData();
		for (machine a = 0; a < kWindowSystemExtensionCount; a++)
		{
			windowSystemEnableBox[a] = new CheckWidget(Vector2D(width, 16.0F), windowSystemExtensionData->name, "font/Gui");
			extensionsList->AppendListItem(windowSystemEnableBox[a]);
			if (windowSystemExtensionData->enabled) windowSystemEnableBox[a]->SetValue(1);
			windowSystemEnableBox[a]->Enable();
			
			windowSystemExtensionData++;
		}
	
	#endif
	
	SetFocusWidget(extensionsList);
}

bool ExtensionsWindow::HandleKeyboardEvent(const KeyboardEventData *eventData)
{
	if (eventData->eventType == kEventKeyDown)
	{
		unsigned_int32 code = eventData->keyCode;
		
		if (code == kKeyCodeEscape)
		{
			Close();
			return (true);
		}
		else if (code == kKeyCodeReturn)
		{
			okayButton->Activate();
			return (true);
		}
	}
	
	return (Window::HandleKeyboardEvent(eventData));
}

void ExtensionsWindow::HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetActivate)
	{
		if (widget == okayButton)
		{
			const GraphicsExtensionData *data = GraphicsMgr::GetExtensionData();
			for (machine a = 0; a < kGraphicsExtensionCount; a++)
			{
				data->enabled = (enableBox[a]->GetValue() != 0);
				data++;
			}
			
			#if C4WINDOWS|| C4LINUX
			
				const WindowSystemExtensionData *windowSystemExtensionData = GraphicsMgr::GetWindowSystemExtensionData();
				for (machine a = 0; a < kWindowSystemExtensionCount; a++)
				{
					windowSystemExtensionData->enabled = (windowSystemEnableBox[a]->GetValue() != 0);
					windowSystemExtensionData++;
				}
			
			#endif
			
			GraphicsMgr::Delete();
			GraphicsMgr::New();
			
			Close();
		}
		else if (widget == cancelButton)
		{
			Close();
		}
	}
}


ConsoleWindow::ConsoleWindow() :
		Window("C4/Console"),
		Singleton<ConsoleWindow>(TheConsoleWindow),
		reporter(&Report, this)
{
	SetWidgetPosition(Point3D(8.0F, 26.0F, 0.0F));
	Hide();
	
	TheInputMgr->SetConsoleProc(&ConsoleProc, this);
	Engine::InstallReporter(&reporter);
	
	commandHistoryCount = 0;
	commandHistoryStart = 0;
	commandHistoryOffset = 0;
	
	dirtyFlag = true;
}

ConsoleWindow::~ConsoleWindow()
{
	TheInputMgr->SetConsoleProc(nullptr);
}

void ConsoleWindow::New(void)
{
	if (!TheConsoleWindow) TheInterfaceMgr->AddWidget(new ConsoleWindow);
}

void ConsoleWindow::ConsoleProc(void *cookie)
{
	TheInterfaceMgr->SetActiveWindow(static_cast<ConsoleWindow *>(cookie));
}

void ConsoleWindow::Report(const char *text, unsigned_int32 flags, void *cookie)
{
	if (!(flags & kReportLog)) static_cast<ConsoleWindow *>(cookie)->AddText(text);
}

void ConsoleWindow::Preprocess(void)
{
	Window::Preprocess();
	
	textWidget = static_cast<TextWidget *>(FindWidget("Text"));
	commandLine = static_cast<EditTextWidget *>(FindWidget("Edit"));
	scrollWidget = static_cast<ScrollWidget *>(FindWidget("Scroll"));
	
	textWidget->SetTextFormatExclusionMask(0);
	textWidget->SetRenderLineCount(kConsoleLineCount);
	
	scrollWidget->SetPageDistance(kConsoleLineCount - 1);
	scrollWidget->SetValue(kConsoleHistoryCount - kConsoleLineCount);
	
	#if C4DEBUG
	
		textWidget->SetText((String<63>("[+UND][#FFF]C4 Engine[-UND]\n[#FF8]Version ") += C4VERSION) += " [#AA5](Debug)\n");
	
	#else
	
		textWidget->SetText((String<63>("[+UND][#FFF]C4 Engine[-UND]\n[#FF8]Version ") += C4VERSION) += '\n');
	
	#endif
}

void ConsoleWindow::EnterForeground(void)
{
	Window::EnterForeground();
	
	if (!Visible())
	{
		Show();
		SetFocusWidget(commandLine);
		commandLine->SelectAll();
	}
}

void ConsoleWindow::Close(void)
{
	Hide();
	TheInterfaceMgr->GetStrip()->HideEmpty();
}

bool ConsoleWindow::HandleKeyboardEvent(const KeyboardEventData *eventData)
{
	EventType eventType = eventData->eventType;
	if (eventType == kEventKeyDown)
	{
		unsigned_int32 code = eventData->keyCode;
		
		if (code == kKeyCodeReturn)
		{
			const char *text = commandLine->GetText();
			if (text[0] != 0)
			{
				unsigned_int32 start = commandHistoryStart;
				commandHistory[start] = text;
				commandHistoryCount = Min(commandHistoryCount + 1, kConsoleCommandHistoryCount - 1);
				commandHistoryStart = (start + 1) & (kConsoleCommandHistoryCount - 1);
				commandHistoryOffset = 0;
				
				TheEngine->ExecuteText(text);
				commandLine->SetText(nullptr);
			}

			return (true);
		}
		else if (code == kKeyCodeEscape)
		{
			Close();
			return (true);
		}
		else if (code == kKeyCodeUpArrow)
		{
			unsigned_int32 offset = commandHistoryOffset;
			if (offset < commandHistoryCount)
			{
				unsigned_int32 start = commandHistoryStart;
				if (++offset == 1) commandHistory[start] = commandLine->GetText();
				commandHistoryOffset = offset;
				
				unsigned_int32 position = (start - offset) & (kConsoleCommandHistoryCount - 1);
				commandLine->SetText(commandHistory[position]);
				commandLine->SetSelection(kMaxCommandLength, kMaxCommandLength);
			}
		}
		else if (code == kKeyCodeDownArrow)
		{
			unsigned_int32 offset = commandHistoryOffset;
			if (offset > 0)
			{
				commandHistoryOffset = --offset;
				
				unsigned_int32 position = (commandHistoryStart - offset) & (kConsoleCommandHistoryCount - 1);
				commandLine->SetText(commandHistory[position]);
				commandLine->SetSelection(kMaxCommandLength, kMaxCommandLength);
			}
		}
		else if (code == kKeyCodePageUp)
		{
			int32 value = scrollWidget->GetValue();
			if (value > 0)
			{
				scrollWidget->SetValue(MaxZero(value - scrollWidget->GetPageDistance()));
				UpdateDisplayLine();
			}
		}
		else if (code == kKeyCodePageDown)
		{
			int32 value = scrollWidget->GetValue();
			int32 maxValue = scrollWidget->GetMaxValue();
			if (value < maxValue)
			{
				scrollWidget->SetValue(Min(value + scrollWidget->GetPageDistance(), maxValue));
				UpdateDisplayLine();
			}
		}
	}
	else if (eventType == kEventKeyCommand)
	{
		if (eventData->keyCode == 'W')
		{
			Close();
			return (true);
		}
	}
	
	return (Window::HandleKeyboardEvent(eventData));
}

void ConsoleWindow::HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		if (widget == scrollWidget) UpdateDisplayLine();
	}
}

void ConsoleWindow::Render(List<Renderable> *renderList)
{
	if (dirtyFlag)
	{
		dirtyFlag = false;
		
		textWidget->SplitLines();
		int32 count = textWidget->GetLineCount();
		if (count > kConsoleHistoryCount)
		{
			String<> string(textWidget->GetText() + textWidget->GetLineEnd(count - kConsoleHistoryCount - 1));
			textWidget->SetText(string);
			
			count = kConsoleHistoryCount;
		}
		
		count = MaxZero(count - kConsoleLineCount);
		scrollWidget->SetMaxValue(count);
		scrollWidget->SetValue(count);
		UpdateDisplayLine();
	}
	
	Window::Render(renderList);
}

void ConsoleWindow::AddText(const char *text)
{
	const char *history = textWidget->GetText();
	if (text) textWidget->SetText((String<>(history) += "\n[INIT][LEFT]") += text);
	else textWidget->SetText(String<>(history) += '\n');
	
	dirtyFlag = true;
}

// ZYURVUR
