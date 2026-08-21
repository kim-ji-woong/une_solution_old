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


#include "C4Viewports.h"
#include "C4World.h"


using namespace C4;


ViewportWidget::ViewportWidget(WidgetType type, Camera *camera) : RenderableWidget(type, kRenderTriangleStrip)
{
	viewportIndex = 0;
	viewportCamera = camera;
	
	Initialize();
}

ViewportWidget::ViewportWidget(WidgetType type, Camera *camera, const Vector2D& size) : RenderableWidget(type, kRenderTriangleStrip, size)
{
	viewportIndex = 0;
	viewportCamera = camera;
	
	Initialize();
}

ViewportWidget::ViewportWidget(const ViewportWidget& viewportWidget, Camera *camera) : RenderableWidget(viewportWidget)
{
	viewportIndex = viewportWidget.viewportIndex;
	viewportCamera = camera;
	
	Initialize();
}

ViewportWidget::~ViewportWidget()
{
}

void ViewportWidget::Initialize(void)
{
	mouseEventProc = nullptr;
	trackTaskProc = nullptr;
	renderProc = nullptr;
	overlayProc = nullptr;
	
	textureValidFlag = false;
	
	SetWidgetUsage(kWidgetMouseWheel | kWidgetMultiaxisMouse);
}

void ViewportWidget::Pack(Packer& data, unsigned_int32 packFlags) const
{
	RenderableWidget::Pack(data, packFlags);
	
	data << TerminatorChunk;
}

void ViewportWidget::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	RenderableWidget::Unpack(data, unpackFlags);
	UnpackChunkList<ViewportWidget>(data, unpackFlags);
}

bool ViewportWidget::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	return (false);
}

void ViewportWidget::SetWidgetSize(const Vector2D& size)
{
	RenderableWidget::SetWidgetSize(size);
	
	if (!GetManipulator()) DeallocateTexture();
}

void ViewportWidget::Preprocess(void)
{
	RenderableWidget::Preprocess();
	
	SetAmbientBlendState(kBlendReplace);
	
	attributeList.Append(&textureMapAttribute);
	SetMaterialAttributeList(&attributeList);
	
	SetVertexCount(4);
	SetAttributeArray(kArrayVertex, viewportVertex);
	SetAttributeArray(kArrayTexture0, viewportTexcoord);
	
	if (GetManipulator())
	{
		textureMapAttribute.SetTexture("C4/checker");
		textureValidFlag = true;
	}
}

void ViewportWidget::Build(void)
{
	float w = GetWidgetSize().x;
	float h = GetWidgetSize().y;
	
	viewportVertex[0].Set(0.0F, 0.0F);
	viewportVertex[1].Set(0.0F, h);
	viewportVertex[2].Set(w, 0.0F);
	viewportVertex[3].Set(w, h);
	 
	if (GetManipulator())
	{ 
		w *= 0.03125F; 
		h *= 0.03125F; 
	}
	 
	viewportTexcoord[0].Set(0.0F, h);
	viewportTexcoord[1].Set(0.0F, 0.0F);
	viewportTexcoord[2].Set(w, h);
	viewportTexcoord[3].Set(w, 0.0F); 
}

void ViewportWidget::AllocateTexture(void)
{ 
	textureMapAttribute.SetTexture(nullptr, nullptr);
	
	textureHeader.textureType = kTextureRectangle;
	textureHeader.textureFlags = kTextureRenderTarget;
	textureHeader.colorSemantic = kTextureSemanticNone;
	textureHeader.alphaSemantic = kTextureSemanticNone;
	textureHeader.imageFormat = kTextureRGBA8;
	textureHeader.imageWidth = (int32) GetWidgetSize().x;
	textureHeader.imageHeight = (int32) GetWidgetSize().y;
	textureHeader.imageDepth = 1;
	textureHeader.wrapMode[0] = kTextureClamp;
	textureHeader.wrapMode[1] = kTextureClamp;
	textureHeader.wrapMode[2] = kTextureClamp;
	textureHeader.mipmapCount = 1;
	textureHeader.mipmapDataOffset = 0;
	textureHeader.auxiliaryDataSize = 0;
	textureHeader.auxiliaryDataOffset = 0;
	
	textureMapAttribute.SetTexture(&textureHeader);
	textureValidFlag = false;
	
	InvalidateShaderData();
}

void ViewportWidget::DeallocateTexture(void)
{
	textureMapAttribute.SetTexture(nullptr, nullptr);
	textureValidFlag = false;
	
	InvalidateShaderData();
}

void ViewportWidget::Render(List<Renderable> *renderList)
{
	const CameraObject		*graphicsCameraObject;
	const Transformable		*graphicsCameraTransformable;
	
	bool active = false;
	
	if (renderProc)
	{
		if (!textureValidFlag)
		{
			active = true;
			
			if (!renderList->Empty())
			{
				TheGraphicsMgr->DrawRenderList(renderList);
				renderList->RemoveAll();
			}
			
			graphicsCameraObject = TheGraphicsMgr->GetCameraObject();
			graphicsCameraTransformable = TheGraphicsMgr->GetCameraTransformable();
			SetGraphicsCamera();
			
			(*renderProc)(renderList, this, renderCookie);
			
			if (!renderList->Empty())
			{
				TheGraphicsMgr->DrawRenderList(renderList);
				renderList->RemoveAll();
			}
			
			Texture *texture = textureMapAttribute.GetTexture();
			if (texture)
			{
				const Point3D& p = GetWorldPosition();
				float w = GetWidgetSize().x;
				float h = GetWidgetSize().y;
				
				TheGraphicsMgr->CopyRenderTarget(texture, Rect((int32) p.x, (int32) p.y, (int32) (p.x + w), (int32) (p.y + h)));
				textureValidFlag = true;
			}
		}
		else
		{
			RenderableWidget::Render(renderList);
		}
	}
	else
	{
		RenderableWidget::Render(renderList);
	}
	
	if (overlayProc)
	{
		if (!active)
		{
			active = true;
			
			if (!renderList->Empty())
			{
				TheGraphicsMgr->DrawRenderList(renderList);
				renderList->RemoveAll();
			}
			
			graphicsCameraObject = TheGraphicsMgr->GetCameraObject();
			graphicsCameraTransformable = TheGraphicsMgr->GetCameraTransformable();
			SetGraphicsCamera();
		}
		
		(*overlayProc)(renderList, this, overlayCookie);
		
		if (!renderList->Empty())
		{
			TheGraphicsMgr->DrawRenderList(renderList);
			renderList->RemoveAll();
		}
	}
	
	if (active) TheGraphicsMgr->SetCamera(graphicsCameraObject, graphicsCameraTransformable);
}

void ViewportWidget::HandleMouseEvent(const PanelMouseEventData *eventData)
{
	if (mouseEventProc) (*mouseEventProc)(eventData, this, mouseEventCookie);
}

void ViewportWidget::TrackTask(WidgetPart widgetPart, const Point3D& mousePosition)
{
	if (trackTaskProc) (*trackTaskProc)(mousePosition, this, trackTaskCookie);
}


OrthoViewportWidget::OrthoViewportWidget() : ViewportWidget(kWidgetOrthoViewport, &orthoCamera)
{
	orthoScale.Set(1.0F, 1.0F);
}

OrthoViewportWidget::OrthoViewportWidget(const Vector2D& size, const Vector2D& scale) : ViewportWidget(kWidgetOrthoViewport, &orthoCamera, size)
{
	orthoScale = scale;
}

OrthoViewportWidget::OrthoViewportWidget(const OrthoViewportWidget& orthoViewportWidget) : ViewportWidget(orthoViewportWidget, &orthoCamera)
{
	orthoScale = orthoViewportWidget.orthoScale;
}

OrthoViewportWidget::~OrthoViewportWidget()
{
}

Widget *OrthoViewportWidget::Replicate(void) const
{
	return (new OrthoViewportWidget(*this));
}

void OrthoViewportWidget::Pack(Packer& data, unsigned_int32 packFlags) const
{
	ViewportWidget::Pack(data, packFlags);
	
	data << TerminatorChunk;
}

void OrthoViewportWidget::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	ViewportWidget::Unpack(data, unpackFlags);
	UnpackChunkList<OrthoViewportWidget>(data, unpackFlags);
}

bool OrthoViewportWidget::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	return (false);
}

void OrthoViewportWidget::SetGraphicsCamera(void)
{
	const Point3D& p = GetWorldPosition();
	float w = GetWidgetSize().x;
	float h = GetWidgetSize().y;
	
	OrthoCameraObject *object = orthoCamera.GetObject();
	object->SetViewRect(Rect((int32) p.x, (int32) p.y, (int32) (p.x + w), (int32) (p.y + h)));
	
	float x = PositiveFloor(w * 0.5F) * orthoScale.x;
	float y = PositiveFloor(h * 0.5F) * orthoScale.y;
	object->SetOrthoRect(-x, w * orthoScale.x - x, -y, h * orthoScale.y - y);
	
	orthoCamera.Invalidate();
	orthoCamera.Update();
	
	TheGraphicsMgr->SetCamera(object, &orthoCamera);
}


FrustumViewportWidget::FrustumViewportWidget() :
		ViewportWidget(kWidgetFrustumViewport, &frustumCamera),
		frustumCamera(2.0F, 1.0F)
{
	cameraAzimuth = 0.0F;
	cameraAltitude = 0.0F;
}

FrustumViewportWidget::FrustumViewportWidget(const Vector2D& size, float focalLength) :
		ViewportWidget(kWidgetFrustumViewport, &frustumCamera, size),
		frustumCamera(focalLength, 1.0F)
{
	cameraAzimuth = 0.0F;
	cameraAltitude = 0.0F;
}

FrustumViewportWidget::FrustumViewportWidget(WidgetType type) :
		ViewportWidget(type, &frustumCamera),
		frustumCamera(2.0F, 1.0F)
{
	cameraAzimuth = 0.0F;
	cameraAltitude = 0.0F;
}

FrustumViewportWidget::FrustumViewportWidget(WidgetType type, const Vector2D& size, float focalLength) :
		ViewportWidget(kWidgetFrustumViewport, &frustumCamera, size),
		frustumCamera(focalLength, 1.0F)
{
	cameraAzimuth = 0.0F;
	cameraAltitude = 0.0F;
}

FrustumViewportWidget::FrustumViewportWidget(const FrustumViewportWidget& frustumViewportWidget) :
		ViewportWidget(frustumViewportWidget, &frustumCamera),
		frustumCamera(frustumViewportWidget.frustumCamera.GetObject()->GetFocalLength(), 1.0F)
{
	cameraAzimuth = 0.0F;
	cameraAltitude = 0.0F;
}

FrustumViewportWidget::~FrustumViewportWidget()
{
}

Widget *FrustumViewportWidget::Replicate(void) const
{
	return (new FrustumViewportWidget(*this));
}

void FrustumViewportWidget::Pack(Packer& data, unsigned_int32 packFlags) const
{
	ViewportWidget::Pack(data, packFlags);
	
	data << TerminatorChunk;
}

void FrustumViewportWidget::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	ViewportWidget::Unpack(data, unpackFlags);
	UnpackChunkList<FrustumViewportWidget>(data, unpackFlags);
}

bool FrustumViewportWidget::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	return (false);
}

void FrustumViewportWidget::SetGraphicsCamera(void)
{
	const Point3D& p = GetWorldPosition();
	float w = GetWidgetSize().x;
	float h = GetWidgetSize().y;
	
	FrustumCameraObject *object = frustumCamera.GetObject();
	object->SetViewRect(Rect((int32) p.x, (int32) p.y, (int32) (p.x + w), (int32) (p.y + h)));
	object->SetAspectRatio(h / w);
	
	frustumCamera.Invalidate();
	frustumCamera.Update();
	
	TheGraphicsMgr->SetCamera(object, &frustumCamera);
}

void FrustumViewportWidget::SetCameraTransform(float azm, float alt, const Point3D& position)
{
	cameraAzimuth = azm;
	cameraAltitude = alt;
	
	Vector2D t = CosSin(azm);
	Vector2D p = CosSin(alt);
	Vector3D view(t.x * p.x, t.y * p.x, p.y);
	Vector3D right(t.y, -t.x, 0.0F);
	Vector3D down = view % right;
	
	frustumCamera.SetNodeTransform(Transform4D(right, down, view, position));
	InvalidateTexture();
}

void FrustumViewportWidget::SetCameraPosition(const Point3D& position)
{
	frustumCamera.SetNodePosition(position);
	InvalidateTexture();
}


WorldViewportWidget::WorldViewportWidget() : FrustumViewportWidget(kWidgetWorldViewport)
{
	viewportWorld = nullptr;
}

WorldViewportWidget::WorldViewportWidget(const Vector2D& size, float focalLength) : FrustumViewportWidget(kWidgetWorldViewport, size, focalLength)
{
	viewportWorld = nullptr;
	
	cameraDistance = 1.0F;
	cameraTarget.Set(0.0F, 0.0F, 0.0F);
}

WorldViewportWidget::~WorldViewportWidget()
{
	delete viewportWorld;
}

Widget *WorldViewportWidget::Replicate(void) const
{
	return (new WorldViewportWidget(*this));
}

void WorldViewportWidget::Preprocess(void)
{
	FrustumViewportWidget::Preprocess();
	
	SetRenderProc(&ViewportRender, this);
}

void WorldViewportWidget::EnableCameraOrbit(const Point3D& target, float distance)
{
	cameraTarget = target;
	cameraDistance = distance;
	maxCameraDistance = distance;
	
	trackFlag = false;
	SetMouseEventProc(&ViewportHandleMouseEvent, this);
}

void WorldViewportWidget::DisableCameraOrbit(void)
{
	SetMouseEventProc(nullptr);
}

void WorldViewportWidget::SetCameraAngles(float azm, float alt)
{
	Vector2D t = CosSin(azm);
	Vector2D p = CosSin(alt) * cameraDistance;
	SetCameraTransform(azm, alt, cameraTarget + Vector3D(-t.x * p.x, -t.y * p.x, -p.y));
}

void WorldViewportWidget::LoadWorld(const char *name)
{
	// Don't delete the previous world until after the new world is loaded so that any
	// resources used by both worlds don't get unloaded and immediately loaded again.
	
	World *previousWorld = viewportWorld;
	
	viewportWorld = new World(name, kWorldViewport | kWorldClearColor | kWorldMotionBlurInhibit | kWorldListenerInhibit);
	if (viewportWorld->Preprocess() == kWorldOkay)
	{
		viewportWorld->SetRenderSize((int32) GetWidgetSize().x, (int32) GetWidgetSize().y);
		
		FrustumCamera *camera = GetViewportCamera();
		camera->GetObject()->SetClearColor(ColorRGBA(0.0F, 0.125F, 0.1F, 0.0F));
		viewportWorld->SetCamera(camera);
	}
	else
	{
		delete viewportWorld;
		viewportWorld = nullptr;
	}
	
	delete previousWorld;
}

void WorldViewportWidget::UnloadWorld(void)
{
	delete viewportWorld;
	viewportWorld = nullptr;
}

void WorldViewportWidget::ViewportHandleMouseEvent(const MouseEventData *eventData, ViewportWidget *viewport, void *cookie)
{
	WorldViewportWidget *worldViewport = static_cast<WorldViewportWidget *>(cookie);
	
	EventType eventType = eventData->eventType;
	if (eventType == kEventMouseDown)
	{
		worldViewport->trackFlag = true;
		worldViewport->previousPosition = eventData->mousePosition;
	}
	else if (eventType == kEventMouseMoved)
	{
		if (worldViewport->trackFlag)
		{
			float azm = worldViewport->GetCameraAzimuth() + (worldViewport->previousPosition.x - eventData->mousePosition.x) * 0.03125F;
			if (azm < -K::pi) azm += K::two_pi;
			else if (azm > K::pi) azm -= K::two_pi;
			
			float alt = worldViewport->GetCameraAltitude() + (worldViewport->previousPosition.y - eventData->mousePosition.y) * 0.03125F;
			alt = Clamp(alt, -K::pi_over_2, K::pi_over_2);
			
			worldViewport->previousPosition = eventData->mousePosition;
			worldViewport->SetCameraAngles(azm, alt);
		}
	}
	else if (eventType == kEventMouseWheel)
	{
		float maxDistance = worldViewport->maxCameraDistance;
		float distance = worldViewport->cameraDistance - eventData->mousePosition.y * (maxDistance * 0.0625F);
		worldViewport->cameraDistance = Fmin(Fmax(distance, maxDistance * 0.125F), maxDistance);
		worldViewport->SetCameraAngles(worldViewport->GetCameraAzimuth(), worldViewport->GetCameraAltitude());
	}
	else
	{
		worldViewport->trackFlag = false;
	}
}

void WorldViewportWidget::ViewportRender(List<Renderable> *renderList, ViewportWidget *viewport, void *cookie)
{
	WorldViewportWidget *worldViewport = static_cast<WorldViewportWidget *>(cookie);
	
	World *world = worldViewport->viewportWorld;
	if (world)
	{
		unsigned_int32 mask = TheGraphicsMgr->GetTargetDisableMask();
		TheGraphicsMgr->SetTargetDisableMask(mask | ((1 << kRenderTargetReflection) | (1 << kRenderTargetRefraction)));
		
		world->Update();
		world->BeginRendering();
		world->Render();
		world->EndRendering();
		
		TheGraphicsMgr->SetTargetDisableMask(mask);
	}
}


Grid::Grid() :
		Renderable(kRenderLines),
		dynamicVertexBuffer(kVertexBufferAttribute | kVertexBufferDynamic),
		dynamicVertexBufferObserver(this, &Grid::FillDynamicVertexBuffer)
{
	gridFlags = 0;
	gridLineSpacing = 0.1F;
	majorLineInterval = 10;
	
	axisLineColor.Set(0x80, 0x80, 0x80, 0xFF);
	majorLineColor.Set(0x40, 0x40, 0x40, 0xFF);
	minorLineColor.Set(0x40, 0x40, 0x40, 0xFF);
	boundingBoxColor.Set(0x00, 0x00, 0x00, 0xFF);
	
	gridStorage = nullptr;
	gridStorageCount = 0;
	
	SetShaderFlags(kShaderAmbientEffect);
	SetAmbientBlendState(kBlendReplace | kBlendAlphaPreserve);
	
	SetDynamicArrayFlags((1 << kArrayVertex) | (1 << kArrayColor0));
	SetVertexBuffer(kVertexBufferDynamicArray, &dynamicVertexBuffer);
}

Grid::~Grid()
{
	delete[] gridStorage;
}

void Grid::Build(const Point2D& min, const Point2D& max, float scale)
{
	float offset = scale * 0.5F;
	scale = 1.0F / scale;
	
	int32 count = 12;
	float spacing = gridLineSpacing;
	if (spacing * scale >= 4.0F)
	{
		int32 imin = (int32) Ceil(min.x / spacing);
		int32 imax = (int32) Floor(max.x / spacing);
		int32 jmin = (int32) Ceil(min.y / spacing);
		int32 jmax = (int32) Floor(max.y / spacing);
		
		count += (imax - imin + jmax - jmin + 2) * 2;
	}
	else
	{
		spacing *= (float) majorLineInterval;
		if (spacing * scale >= 4.0F)
		{
			int32 imin = (int32) Ceil(min.x / spacing);
			int32 imax = (int32) Floor(max.x / spacing);
			int32 jmin = (int32) Ceil(min.y / spacing);
			int32 jmax = (int32) Floor(max.y / spacing);
			
			count += (imax - imin + jmax - jmin + 2) * 2;
		}
	}
	
	count = (count + 63) & ~63;
	if (count > gridStorageCount)
	{
		delete[] gridStorage;
		gridStorageCount = count;
		gridStorage = new GridVertex[count];
	}
	
	dynamicVertexBuffer.Initialize(count * sizeof(GridVertex), sizeof(GridVertex));
	
	GridVertex *vertex = gridStorage;
	SetAttributeArray(kArrayVertex, &vertex[0].position);
	SetAttributeOffset(kArrayVertex, 0);
	SetAttributeArray(kArrayColor0, &vertex[0].position.x + 3, 1);
	SetAttributeOffset(kArrayColor0, sizeof(Point2D));
	
	float xmin = min.x + offset;
	float xmax = max.x + offset;
	float ymin = min.y + offset;
	float ymax = max.y + offset;
	
	spacing = gridLineSpacing;
	if (spacing * scale >= 4.0F)
	{
		int32 imin = (int32) Ceil(min.x / spacing);
		int32 imax = (int32) Floor(max.x / spacing);
		
		for (machine i = imin; i <= imax; i++)
		{
			if (i % majorLineInterval != 0)
			{
				float x = (float) i * spacing + offset;
				
				vertex[0].position.Set(x, ymin);
				vertex[1].position.Set(x, ymax);
				vertex[0].color = minorLineColor;
				vertex[1].color = minorLineColor;
				vertex += 2;
			}
		}
		
		int32 jmin = (int32) Ceil(min.y / spacing);
		int32 jmax = (int32) Floor(max.y / spacing);
		
		for (machine j = jmin; j <= jmax; j++)
		{
			if (j % majorLineInterval != 0)
			{
				float y = (float) j * spacing + offset;
				
				vertex[0].position.Set(xmin, y);
				vertex[1].position.Set(xmax, y);
				vertex[0].color = minorLineColor;
				vertex[1].color = minorLineColor;
				vertex += 2;
			}
		}
	}
	
	spacing *= (float) majorLineInterval;
	if (spacing * scale >= 4.0F)
	{
		int32 imin = (int32) Ceil(min.x / spacing);
		int32 imax = (int32) Floor(max.x / spacing);
		
		for (machine i = imin; i <= imax; i++)
		{
			if (i != 0)
			{
				float x = (float) i * spacing + offset;
				
				vertex[0].position.Set(x, ymin);
				vertex[1].position.Set(x, ymax);
				vertex[0].color = majorLineColor;
				vertex[1].color = majorLineColor;
				vertex += 2;
			}
		}
		
		int32 jmin = (int32) Ceil(min.y / spacing);
		int32 jmax = (int32) Floor(max.y / spacing);
		
		for (machine j = jmin; j <= jmax; j++)
		{
			if (j != 0)
			{
				float y = (float) j * spacing + offset;
				
				vertex[0].position.Set(xmin, y);
				vertex[1].position.Set(xmax, y);
				vertex[0].color = majorLineColor;
				vertex[1].color = majorLineColor;
				vertex += 2;
			}
		}
	}
	
	vertex[0].position.Set(offset, ymin);
	vertex[1].position.Set(offset, ymax);
	vertex[0].color = axisLineColor;
	vertex[1].color = axisLineColor;
	
	vertex[2].position.Set(xmin, offset);
	vertex[3].position.Set(xmax, offset);
	vertex[2].color = axisLineColor;
	vertex[3].color = axisLineColor;
	
	vertex += 4;

	if (gridFlags & kGridShowBoundingBox)
	{
		vertex[0].position.Set(boundingBoxMin.x + offset, boundingBoxMin.y + offset);
		vertex[1].position.Set(boundingBoxMin.x + offset, boundingBoxMax.y + offset);
		vertex[2].position.Set(boundingBoxMin.x + offset, boundingBoxMax.y + offset);
		vertex[3].position.Set(boundingBoxMax.x + offset, boundingBoxMax.y + offset);
		vertex[4].position.Set(boundingBoxMax.x + offset, boundingBoxMax.y + offset);
		vertex[5].position.Set(boundingBoxMax.x + offset, boundingBoxMin.y + offset);
		vertex[6].position.Set(boundingBoxMax.x + offset, boundingBoxMin.y + offset);
		vertex[7].position.Set(boundingBoxMin.x + offset, boundingBoxMin.y + offset);
		
		for (machine a = 0; a < 8; a++) vertex[a].color = boundingBoxColor;
		vertex += 8;
	}
	
	SetVertexCount(vertex - gridStorage);
	
	if (dynamicVertexBuffer.Active())
	{
		dynamicVertexBuffer.SetObserver(&dynamicVertexBufferObserver);
		FillDynamicVertexBuffer(&dynamicVertexBuffer);
	}
}

void Grid::FillDynamicVertexBuffer(VertexBuffer *vertexBuffer)
{
	vertexBuffer->UpdateBuffer(0, GetVertexCount() * sizeof(GridVertex), gridStorage);
}


DragRect::DragRect(const ColorRGBA& color) :
		Renderable(kRenderQuads),
		diffuseColor(color)
{
	SetVertexCount(16);
	SetAttributeArray(kArrayVertex, rectVertex);
	
	SetShaderFlags(kShaderAmbientEffect);
	SetAmbientBlendState(kBlendInterpolate | kBlendAlphaPreserve);
	GetFirstRenderSegment()->SetMaterialState(kMaterialTwoSided);
	
	attributeList.Append(&diffuseColor);
	SetMaterialAttributeList(&attributeList);
}

DragRect::~DragRect()
{
}

void DragRect::Build(const Point2D& p1, const Point2D& p2, float scale)
{
	float x1 = Fmin(p1.x, p2.x);
	float x2 = Fmax(p1.x, p2.x);
	float y1 = Fmin(p1.y, p2.y);
	float y2 = Fmax(p1.y, p2.y);
	
	rectVertex[0].Set(x1, y1, 0.0F);
	rectVertex[1].Set(x1, y1 + scale, 0.0F);
	rectVertex[2].Set(x2 + scale, y1 + scale, 0.0F);
	rectVertex[3].Set(x2 + scale, y1, 0.0F);
	
	rectVertex[4].Set(x1, y2, 0.0F);
	rectVertex[5].Set(x1, y2 + scale, 0.0F);
	rectVertex[6].Set(x2 + scale, y2 + scale, 0.0F);
	rectVertex[7].Set(x2 + scale, y2, 0.0F);
	
	rectVertex[8].Set(x1, y1 + scale, 0.0F);
	rectVertex[9].Set(x1, y2, 0.0F);
	rectVertex[10].Set(x1 + scale, y2, 0.0F);
	rectVertex[11].Set(x1 + scale, y1, 0.0F);
	
	rectVertex[12].Set(x2, y1 + scale, 0.0F);
	rectVertex[13].Set(x2, y2, 0.0F);
	rectVertex[14].Set(x2 + scale, y2, 0.0F);
	rectVertex[15].Set(x2 + scale, y1, 0.0F);
}

void DragRect::Build(const Point2D& p1, const Point2D& p2, const Vector3D& dx, const Vector3D& dy, float scale)
{
	float x1 = Fmin(p1.x, p2.x);
	float x2 = Fmax(p1.x, p2.x);
	float y1 = Fmin(p1.y, p2.y);
	float y2 = Fmax(p1.y, p2.y);
	
	rectVertex[0] = dx * x1 + dy * y1;
	rectVertex[1] = dx * x1 + dy * (y1 + scale);
	rectVertex[2] = dx * (x2 + scale) + dy * (y1 + scale);
	rectVertex[3] = dx * (x2 + scale) + dy * y1;
	
	rectVertex[4] = dx * x1 + dy * y2;
	rectVertex[5] = dx * x1 + dy * (y2 + scale);
	rectVertex[6] = dx * (x2 + scale) + dy * (y2 + scale);
	rectVertex[7] = dx * (x2 + scale) + dy * y2;
	
	rectVertex[8] = dx * x1 + dy * (y1 + scale);
	rectVertex[9] = dx * x1 + dy * y2;
	rectVertex[10] = dx * (x1 + scale) + dy * y2;
	rectVertex[11] = dx * (x1 + scale) + dy * y1;
	
	rectVertex[12] = dx * x2 + dy * (y1 + scale);
	rectVertex[13] = dx * x2 + dy * y2;
	rectVertex[14] = dx * (x2 + scale) + dy * y2;
	rectVertex[15] = dx * (x2 + scale) + dy * y1;
}

// ZYURVUR
