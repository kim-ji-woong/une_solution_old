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


#include "C4EditorSupport.h"
#include "C4WorldEditor.h"
#include "C4MarkerManipulators.h"
#include "C4TerrainTools.h"
#include "C4Water.h"
#include "C4World.h"


using namespace C4;


namespace
{
	const float kMinEditorGridSpacing = 0.001F;
	
	
	const TextureAlignMode alignModeTable[kTextureAlignModeCount] =
	{
		kTextureAlignNatural,
		kTextureAlignObjectPlane,
		kTextureAlignWorldPlane,
		kTextureAlignGlobalObjectPlane
	};
	
	
	enum
	{
		kTextureUndoNone,
		kTextureUndoOffset,
		kTextureUndoScale,
		kTextureUndoRotation,
		kTextureUndoReflection,
		kTextureUndoMode
	};
}


NodeType FindPage::nodeTypeTable[kNodeTypeCount] =
{
	kNodeGroup, kNodeTerrainBlock, kNodeWaterBlock, kNodeBone, kNodeCamera, kNodeEffect, kNodeEmitter, kNodeField, kNodeGeometry, kNodeImpostor, kNodeInstance,
	kNodeJoint, kNodeLight, kNodeMarker, kNodeModel, kNodePhysics, kNodePortal, kNodeShape, kNodeSkybox, kNodeSource, kNodeSpace, kNodeTrigger, kNodeZone
};


TypeWidget::TypeWidget(const Vector2D& size, const char *text, Type type) : TextWidget(size, text, "font/Normal")
{
	itemType = type;
}

TypeWidget::~TypeWidget()
{
}


EditorPage::EditorPage(PageType type, const char *panelName) : Page(panelName)
{
	pageType = type;
	worldEditor = nullptr;
	
	pageState = kWidgetHidden;
	bookIndex = 0;
	
	menuItem = nullptr;
}

EditorPage::~EditorPage()
{
}

void EditorPage::Pack(Packer& data, unsigned_int32 packFlags) const
{
	unsigned_int32 state = GetWidgetState() & (kWidgetHidden | kWidgetCollapsed);
	
	data << ChunkHeader('PGST', 4);
	data << state;
	
	const BookWidget *book = GetOwningBook();
	if (book)
	{
		data << ChunkHeader('BOOK', 4);
		data << book->ListElement<BookWidget>::GetListIndex();
	}
	
	data << TerminatorChunk;
}

void EditorPage::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	UnpackChunkList<EditorPage>(data, unpackFlags);
}

bool EditorPage::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'PGST':
			
			data >> pageState;
			return (true);
		
		case 'BOOK':
			 
			data >> bookIndex;
			return (true); 
		 
		#if C4LEGACY 
		
			case 'STAT': 
			{
				unsigned_int32	state;
				
				data >> state; 
				pageState = ((state << 1) & kWidgetHidden) | ((state << 2) & kWidgetCollapsed);
				return (true);
			}
		 
		#endif
	}
	
	return (false);
}

void EditorPage::SetWidgetState(unsigned_int32 state)
{
	if (menuItem)
	{
		if (state & kWidgetHidden) menuItem->HideBullet();
		else menuItem->ShowBullet();
	}
	
	Page::SetWidgetState(state);
}

void EditorPage::Preprocess(void)
{
	Page::Preprocess();
	
	if (pageState & kWidgetHidden) Hide();
	else if (pageState & kWidgetCollapsed) Collapse();
}

void EditorPage::HandlePageMenuItem(Widget *menuItem, const WidgetEventData *eventData)
{
	if (!Visible())
	{
		for (machine a = 0; a < kMaxEditorToolBookCount; a++)
		{
			BookWidget *book = worldEditor->GetBookWidget(a);
			if (book->Visible())
			{
				book->AppendPage(this);
				Show();
				Expand();
				book->OrganizePages();
				break;
			}
		}
	}
}


GeometriesPage::GeometriesPage() :
		EditorPage(kEditorPageGeometries, "WorldEditor/geometry/Geometries"),
		geometryButtonObserver(this, &GeometriesPage::HandleGeometryButtonEvent)
{
	currentTool = -1;
	InitialShow();
}

GeometriesPage::~GeometriesPage()
{
}

void GeometriesPage::Preprocess(void)
{
	static const char *const buttonIdentifier[kEditorGeometryCount] =
	{
		"Plate", "Disk", "Hole", "Annulus", "Box", "Pyramid", "Cylinder", "Cone",
		"Sphere", "Dome", "Torus", "TruncCone", "Tube", "Extrusion", "Revolution", "Cloth"
	};
	
	EditorPage::Preprocess();
	
	for (machine a = 0; a < kEditorGeometryCount; a++)
	{
		geometryButton[a] = static_cast<IconButtonWidget *>(FindWidget(buttonIdentifier[a]));
		geometryButton[a]->SetObserver(&geometryButtonObserver);
	}
}

void GeometriesPage::HandleGeometryButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		GetEditor()->SetCurrentTool(this, widget);
	}
}

void GeometriesPage::Engage(Editor *editor, void *cookie)
{
	Widget *widget = static_cast<Widget *>(cookie);
	
	for (machine a = 0; a < kEditorGeometryCount; a++)
	{
		if (widget == geometryButton[a])
		{
			currentTool = a;
			geometryButton[a]->SetValue(1);
			break;
		}
	}
	
	editor->SetCurrentCursor(TheWorldEditor->GetEditorCursor(kEditorCursorCross));
}

void GeometriesPage::Disengage(Editor *editor, void *cookie)
{
	if (currentTool != -1)
	{
		geometryButton[currentTool]->SetValue(0);
		currentTool = -1;
	}
}

bool GeometriesPage::BeginTool(Editor *editor, EditorTrackData *trackData)
{
	if (trackData->viewportType == kEditorViewportOrtho)
	{
		PrimitiveGeometry *geometry = nullptr;
		
		switch (currentTool)
		{
			case kEditorGeometryPlate:
				
				geometry = new PlateGeometry(Vector2D(kSizeEpsilon, kSizeEpsilon));
				geometry->GetObject()->SetGeometryFlags(kGeometryShadowInhibit);
				break;
			
			case kEditorGeometryDisk:
				
				geometry = new DiskGeometry(Vector2D(kSizeEpsilon, kSizeEpsilon));
				geometry->GetObject()->SetGeometryFlags(kGeometryShadowInhibit);
				break;
			
			case kEditorGeometryHole:
				
				geometry = new HoleGeometry(Vector2D(kSizeEpsilon, kSizeEpsilon), Vector2D(kSizeEpsilon * 0.5F, kSizeEpsilon * 0.5F));
				geometry->GetObject()->SetGeometryFlags(kGeometryShadowInhibit);
				break;
			
			case kEditorGeometryAnnulus:
				
				geometry = new AnnulusGeometry(Vector2D(kSizeEpsilon, kSizeEpsilon), Vector2D(kSizeEpsilon * 0.5F, kSizeEpsilon * 0.5F));
				geometry->GetObject()->SetGeometryFlags(kGeometryShadowInhibit);
				break;
			
			case kEditorGeometryBox:
				
				geometry = new BoxGeometry(Vector3D(kSizeEpsilon, kSizeEpsilon, kSizeEpsilon));
				break;
			
			case kEditorGeometryPyramid:
				
				geometry = new PyramidGeometry(Vector2D(kSizeEpsilon, kSizeEpsilon), kSizeEpsilon);
				break;
			
			case kEditorGeometryCylinder:
				
				geometry = new CylinderGeometry(Vector2D(kSizeEpsilon, kSizeEpsilon), kSizeEpsilon * 2.0F);
				break;
			
			case kEditorGeometryCone:
				
				geometry = new ConeGeometry(Vector2D(kSizeEpsilon, kSizeEpsilon), kSizeEpsilon * 2.0F);
				break;
			
			case kEditorGeometrySphere:
				
				geometry = new SphereGeometry(Vector3D(kSizeEpsilon, kSizeEpsilon, kSizeEpsilon));
				break;
			
			case kEditorGeometryDome:
				
				geometry = new DomeGeometry(Vector3D(kSizeEpsilon, kSizeEpsilon, kSizeEpsilon));
				break;
			
			case kEditorGeometryTorus:
				
				geometry = new TorusGeometry(Vector2D(kSizeEpsilon, kSizeEpsilon), kSizeEpsilon * 0.25F);
				break;
			
			case kEditorGeometryTruncatedCone:
				
				geometry = new TruncatedConeGeometry(Vector2D(kSizeEpsilon, kSizeEpsilon), kSizeEpsilon * 2.0F, 0.5F);
				break;
			
			case kEditorGeometryTube:
			case kEditorGeometryExtrusion:
			case kEditorGeometryRevolution:
			{
				const NodeReference *reference = editor->GetFirstSelectedNode();
				if ((reference) && (!reference->Next()))
				{
					Node *node = reference->GetNode();
					if (node->GetNodeType() == kNodeMarker)
					{
						Marker *marker = static_cast<Marker *>(node);
						if (marker->GetMarkerType() == kMarkerPath)
						{
							PathMarker *pathMarker = static_cast<PathMarker *>(marker);
							const Path *path = pathMarker->GetPath();
							
							if (currentTool == kEditorGeometryTube) geometry = new TubeGeometry(path, Vector2D(kSizeEpsilon, kSizeEpsilon));
							else if (currentTool == kEditorGeometryExtrusion) geometry = new ExtrusionGeometry(path, Vector2D(kSizeEpsilon, kSizeEpsilon), kSizeEpsilon);
							else geometry = new RevolutionGeometry(path, Vector2D(kSizeEpsilon, kSizeEpsilon), kSizeEpsilon * 2.0F);
							
							static_cast<PathPrimitiveGeometry *>(geometry)->SetConnectedPathMarker(pathMarker);
						}
					}
				}
				
				break;
			}
			
			case kEditorGeometryCloth:
				
				geometry = new ClothGeometry(Vector2D(kSizeEpsilon, kSizeEpsilon), 16, 16);
				break;
		}
		
		if (geometry)
		{
			if (!(editor->GetEditorObject()->GetEditorFlags() & kEditorCapGeometry)) geometry->GetObject()->SetPrimitiveFlags(0);
			editor->InitNewNode(trackData, geometry);
			return (true);
		}
	}
	
	return (false);
}

bool GeometriesPage::TrackTool(Editor *editor, EditorTrackData *trackData)
{
	editor->AutoScroll(trackData);
	
	Point2D anchor = trackData->snappedAnchorPosition;
	float dx = trackData->snappedCurrentPosition.x - anchor.x;
	float dy = anchor.y - trackData->snappedCurrentPosition.y;
	
	float ax = Fabs(dx);
	float ay = Fabs(dy);
	
	if (InterfaceMgr::GetShiftKey())
	{
		ax = ay = Fmax(ax, ay);
		dx = (dx < 0.0F) ? -ax : ax;
		dy = (dy < 0.0F) ? -ax : ax;
	}
	
	if ((dx != trackData->currentSize.x) || (dy != trackData->currentSize.y))
	{
		trackData->currentSize.Set(dx, dy);
		
		unsigned_int32 editorFlags = editor->GetEditorObject()->GetEditorFlags();
		
		PrimitiveGeometry *geometry = static_cast<PrimitiveGeometry *>(trackData->trackNode);
		PrimitiveType primitiveType = geometry->GetPrimitiveType();
		
		if ((primitiveType == kPrimitivePlate) || (primitiveType == kPrimitiveBox) || (primitiveType == kPrimitivePyramid) || (primitiveType == kPrimitiveExtrusion) || (primitiveType == kPrimitiveCloth))
		{
			if (editorFlags & kEditorDrawFromCenter)
			{
				anchor.x -= ax;
				anchor.y += ay;
				dx = ax * 2.0F;
				dy = ay * 2.0F;
			}
			else
			{
				if (dx < 0.0F)
				{
					anchor.x += dx;
					dx = -dx;
				}
				
				if (dy < 0.0F)
				{
					anchor.y -= dy;
					dy = -dy;
				}
			}
		}
		else
		{
			if (editorFlags & kEditorDrawFromCenter)
			{
				dx = ax;
				dy = ay;
			}
			else
			{
				anchor.x += dx * 0.5F;
				anchor.y -= dy * 0.5F;
				dx = ax * 0.5F;
				dy = ay * 0.5F;
			}
		}
		
		float sx = Fmax(dx, kSizeEpsilon);
		float sy = Fmax(dy, kSizeEpsilon);
		
		if (primitiveType != kPrimitiveTube) geometry->SetNodePosition(editor->GetTargetSpacePosition(trackData, anchor));
		
		switch (primitiveType)
		{
			case kPrimitivePlate:
			{
				const PlateGeometry *plate = static_cast<PlateGeometry *>(geometry);
				plate->GetObject()->SetPlateSize(Vector2D(sx, sy));
				break;
			}
			
			case kPrimitiveDisk:
			{
				const DiskGeometry *disk = static_cast<DiskGeometry *>(geometry);
				disk->GetObject()->SetDiskSize(Vector2D(sx, sy));
				break;
			}
			
			case kPrimitiveHole:
			{
				const HoleGeometry *hole = static_cast<HoleGeometry *>(geometry);
				HoleGeometryObject *object = hole->GetObject();
				object->SetOuterSize(Vector2D(sx, sy));
				object->SetInnerSize(Vector2D(sx * 0.5F, sy * 0.5F));
				break;
			}
			
			case kPrimitiveAnnulus:
			{
				const AnnulusGeometry *annulus = static_cast<AnnulusGeometry *>(geometry);
				AnnulusGeometryObject *object = annulus->GetObject();
				object->SetOuterSize(Vector2D(sx, sy));
				object->SetInnerSize(Vector2D(sx * 0.5F, sy * 0.5F));
				break;
			}
			
			case kPrimitiveBox:
			{
				const BoxGeometry *box = static_cast<BoxGeometry *>(geometry);
				box->GetObject()->SetBoxSize(Vector3D(sx, sy, Fmax(sx, sy)));
				break;
			}
			
			case kPrimitivePyramid:
			{
				const PyramidGeometry *cone = static_cast<PyramidGeometry *>(geometry);
				PyramidGeometryObject *object = cone->GetObject();
				object->SetPyramidSize(Vector2D(sx, sy));
				object->SetPyramidHeight(Fmax(sx, sy));
				break;
			}
			
			case kPrimitiveCylinder:
			{
				const CylinderGeometry *cylinder = static_cast<CylinderGeometry *>(geometry);
				CylinderGeometryObject *object = cylinder->GetObject();
				object->SetCylinderSize(Vector2D(sx, sy));
				object->SetCylinderHeight(Fmax(sx, sy) * 2.0F);
				break;
			}
			
			case kPrimitiveCone:
			{
				const ConeGeometry *cone = static_cast<ConeGeometry *>(geometry);
				ConeGeometryObject *object = cone->GetObject();
				object->SetConeSize(Vector2D(sx, sy));
				object->SetConeHeight(Fmax(sx, sy) * 2.0F);
				break;
			}
			
			case kPrimitiveTruncatedCone:
			{
				const TruncatedConeGeometry *cone = static_cast<TruncatedConeGeometry *>(geometry);
				TruncatedConeGeometryObject *object = cone->GetObject();
				object->SetConeSize(Vector2D(sx, sy));
				object->SetConeHeight(Fmax(sx, sy) * 2.0F);
				break;
			}
			
			case kPrimitiveSphere:
			{
				const SphereGeometry *sphere = static_cast<SphereGeometry *>(geometry);
				sphere->GetObject()->SetSphereSize(Vector3D(sx, sy, Fmax(sx, sy)));
				break;
			}
			
			case kPrimitiveDome:
			{
				const DomeGeometry *dome = static_cast<DomeGeometry *>(geometry);
				dome->GetObject()->SetDomeSize(Vector3D(sx, sy, Fmax(sx, sy)));
				break;
			}
			
			case kPrimitiveTorus:
			{
				const TorusGeometry *torus = static_cast<TorusGeometry *>(geometry);
				TorusGeometryObject *object = torus->GetObject();
				float r = Fmin(sx, sy) * 0.25F;
				object->SetPrimarySize(Vector2D(sx - r, sy - r));
				object->SetSecondaryRadius(r);
				break;
			}
			
			case kPrimitiveTube:
			{
				const TubeGeometry *tube = static_cast<TubeGeometry *>(geometry);
				tube->GetObject()->SetTubeSize(Vector2D(sx, sy));
				break;
			}
			
			case kPrimitiveExtrusion:
			{
				const ExtrusionGeometry *extrusion = static_cast<ExtrusionGeometry *>(geometry);
				ExtrusionGeometryObject *object = extrusion->GetObject();
				object->SetExtrusionSize(Vector2D(sx, sy));
				object->SetExtrusionHeight(Fmax(sx, sy));
				break;
			}
			
			case kPrimitiveRevolution:
			{
				const RevolutionGeometry *revolution = static_cast<RevolutionGeometry *>(geometry);
				RevolutionGeometryObject *object = revolution->GetObject();
				object->SetRevolutionSize(Vector2D(sx, sy));
				object->SetRevolutionHeight(Fmax(sx, sy) * 2.0F);
				break;
			}
			
			case kPrimitiveCloth:
			{
				const ClothGeometry *cloth = static_cast<ClothGeometry *>(geometry);
				cloth->GetObject()->SetClothSize(Vector2D(sx, sy));
				break;
			}
		}
		
		editor->InvalidateNode(geometry);
		editor->GetRootNode()->Update();
		editor->RebuildGeometry(geometry);
	}
	
	return ((dx != 0.0F) && (dy != 0.0F));
}

bool GeometriesPage::EndTool(Editor *editor, EditorTrackData *trackData)
{
	editor->CommitNewNode(trackData, TrackTool(editor, trackData));
	return (true);
}


CamerasPage::CamerasPage() :
		EditorPage(kEditorPageCameras, "WorldEditor/camera/Cameras"),
		cameraButtonObserver(this, &CamerasPage::HandleCameraButtonEvent)
{
	currentTool = -1;
}

CamerasPage::~CamerasPage()
{
}

void CamerasPage::Preprocess(void)
{
	static const char *const buttonIdentifier[kEditorCameraCount] =
	{
		"Frustum"
	};
	
	EditorPage::Preprocess();
	
	for (machine a = 0; a < kEditorCameraCount; a++)
	{
		cameraButton[a] = static_cast<IconButtonWidget *>(FindWidget(buttonIdentifier[a]));
		cameraButton[a]->SetObserver(&cameraButtonObserver);
	}
}

void CamerasPage::HandleCameraButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		GetEditor()->SetCurrentTool(this, widget);
	}
}

void CamerasPage::Engage(Editor *editor, void *cookie)
{
	Widget *widget = static_cast<Widget *>(cookie);
	
	for (machine a = 0; a < kEditorCameraCount; a++)
	{
		if (widget == cameraButton[a])
		{
			currentTool = a;
			cameraButton[a]->SetValue(1);
			break;
		}
	}
	
	editor->SetCurrentCursor(TheWorldEditor->GetEditorCursor(kEditorCursorCross));
}

void CamerasPage::Disengage(Editor *editor, void *cookie)
{
	if (currentTool != -1)
	{
		cameraButton[currentTool]->SetValue(0);
		currentTool = -1;
	}
}

bool CamerasPage::BeginTool(Editor *editor, EditorTrackData *trackData)
{
	if (trackData->viewportType == kEditorViewportOrtho)
	{
		Camera *camera = nullptr;
		
		switch (currentTool)
		{
			case kEditorCameraFrustum:
			{
				FrustumCamera *frustumCamera = new FrustumCamera(1.0F, 1.0F);
				frustumCamera->GetObject()->SetFrustumFlags(kFrustumInfinite);
				camera = frustumCamera;
				break;
			}
		}
		
		if (camera)
		{
			CameraObject *object = camera->GetObject();
			object->SetNearDepth(0.1F);
			object->SetFarDepth(1.0F);
			
			editor->InitNewNode(trackData, camera);
			return (true);
		}
	}
	
	return (false);
}

bool CamerasPage::TrackTool(Editor *editor, EditorTrackData *trackData)
{
	editor->AutoScroll(trackData);
	
	float dx = trackData->currentPosition.x - trackData->snappedAnchorPosition.x;
	float dy = trackData->snappedAnchorPosition.y - trackData->currentPosition.y;
	
	Camera *camera = static_cast<Camera *>(trackData->trackNode);
	CameraType cameraType = camera->GetCameraType();
	
	if (InterfaceMgr::GetShiftKey())
	{
		if (Fabs(dx) > Fabs(dy)) dy = 0.0F;
		else dx = 0.0F;
	}
	
	if ((dx != trackData->currentSize.x) || (dy != trackData->currentSize.y))
	{
		trackData->currentSize.Set(dx, dy);
		
		switch (cameraType)
		{
			case kCameraFrustum:
			{
				float r2 = dx * dx + dy * dy;
				float scale = trackData->viewportScale * 8.0F;
				if (r2 > scale * scale)
				{
					Vector3D direction = editor->GetTargetSpaceDirection(trackData, Vector3D(dx, -dy, 0.0F));
					Vector3D down = editor->GetTargetSpaceDirection(trackData, Vector3D(0.0F, 0.0F, 1.0F));
					
					direction.Normalize();
					down = (down - ProjectOnto(down, direction)).Normalize();
					
					camera->SetNodeMatrix3D(down % direction, down, direction);
				}
				
				static_cast<FrustumCameraObject *>(camera->GetObject())->SetFarDepth(Fmax(Sqrt(r2), 1.0F));
				break;
			}
		}
		
		editor->InvalidateNode(camera);
	}
	
	return (true);
}

bool CamerasPage::EndTool(Editor *editor, EditorTrackData *trackData)
{
	editor->CommitNewNode(trackData, TrackTool(editor, trackData));
	return (true);
}


LightsPage::LightsPage() :
		EditorPage(kEditorPageLights, "WorldEditor/light/Lights"),
		lightButtonObserver(this, &LightsPage::HandleLightButtonEvent)
{
	currentTool = -1;
}

LightsPage::~LightsPage()
{
}

void LightsPage::Preprocess(void)
{
	static const char *const buttonIdentifier[kEditorLightCount] =
	{
		"Point", "Cube", "Spot", "Infinite", "Depth", "Landscape"
	};
	
	EditorPage::Preprocess();
	
	for (machine a = 0; a < kEditorLightCount; a++)
	{
		lightButton[a] = static_cast<IconButtonWidget *>(FindWidget(buttonIdentifier[a]));
		lightButton[a]->SetObserver(&lightButtonObserver);
	}
}

void LightsPage::HandleLightButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		GetEditor()->SetCurrentTool(this, widget);
	}
}

void LightsPage::Engage(Editor *editor, void *cookie)
{
	Widget *widget = static_cast<Widget *>(cookie);
	
	for (machine a = 0; a < kEditorLightCount; a++)
	{
		if (widget == lightButton[a])
		{
			currentTool = a;
			lightButton[a]->SetValue(1);
			break;
		}
	}
	
	editor->SetCurrentCursor(TheWorldEditor->GetEditorCursor(kEditorCursorCross));
}

void LightsPage::Disengage(Editor *editor, void *cookie)
{
	if (currentTool != -1)
	{
		lightButton[currentTool]->SetValue(0);
		currentTool = -1;
	}
}

bool LightsPage::BeginTool(Editor *editor, EditorTrackData *trackData)
{
	Light *light = nullptr;
	
	switch (currentTool)
	{
		case kEditorLightPoint:
			
			if (trackData->viewportType == kEditorViewportOrtho) light = new PointLight(K::white->GetColorRGB(), 0.0F);
			break;
		
		case kEditorLightCube:
			
			if (trackData->viewportType == kEditorViewportOrtho) light = new CubeLight(K::white->GetColorRGB(), 0.0F, "C4/cube");
			break;
		
		case kEditorLightSpot:
			
			if (trackData->viewportType == kEditorViewportOrtho) light = new SpotLight(K::white->GetColorRGB(), 0.0F, 1.0F, "C4/spot");
			break;
		
		case kEditorLightInfinite:
			
			if ((trackData->viewportType != kEditorViewportGraph) || (trackData->superNode)) light = new InfiniteLight(K::white->GetColorRGB());
			break;
		
		case kEditorLightDepth:
			
			if ((trackData->viewportType != kEditorViewportGraph) || (trackData->superNode)) light = new DepthLight(K::white->GetColorRGB());
			break;
		
		case kEditorLightLandscape:
			
			if ((trackData->viewportType != kEditorViewportGraph) || (trackData->superNode)) light = new LandscapeLight(K::white->GetColorRGB());
			break;
	}
	
	if (light)
	{
		editor->InitNewNode(trackData, light);
		
		if (trackData->viewportType == kEditorViewportOrtho) return (true);
		editor->CommitNewNode(trackData, true);
	}
	
	return (false);
}

bool LightsPage::TrackTool(Editor *editor, EditorTrackData *trackData)
{
	editor->AutoScroll(trackData);
	
	float dx = trackData->currentPosition.x - trackData->snappedAnchorPosition.x;
	float dy = trackData->snappedAnchorPosition.y - trackData->currentPosition.y;
	
	Light *light = static_cast<Light *>(trackData->trackNode);
	LightType lightType = light->GetLightType();
	
	if ((InterfaceMgr::GetShiftKey()) && (lightType != kLightPoint) && (lightType != kLightCube))
	{
		if (Fabs(dx) > Fabs(dy)) dy = 0.0F;
		else dx = 0.0F;
	}
	
	float r2 = dx * dx + dy * dy;
	if ((dx != trackData->currentSize.x) || (dy != trackData->currentSize.y))
	{
		trackData->currentSize.Set(dx, dy);
		
		float range = Sqrt(r2);
		
		switch (lightType)
		{
			case kLightSpot:
				
				static_cast<SpotLightObject *>(light->GetObject())->SetLightRange(range);
				// no break
			
			case kLightInfinite:
			case kLightDepth:
			case kLightLandscape:
				
				if (r2 != 0.0F)
				{
					Vector3D direction = editor->GetTargetSpaceDirection(trackData, Vector3D(dx, -dy, 0.0F));
					Vector3D down = editor->GetTargetSpaceDirection(trackData, Vector3D(0.0F, 0.0F, 1.0F));
					
					direction.Normalize();
					if (lightType != kLightSpot) direction = -direction;
					down = (down - ProjectOnto(down, direction)).Normalize();
					
					light->SetNodeMatrix3D(down % direction, down, direction);
				}
				
				break;
			
			case kLightPoint:
			case kLightCube:
				
				static_cast<PointLightObject *>(light->GetObject())->SetLightRange(range);
				break;
		}
		
		editor->InvalidateNode(light);
	}
	
	return (r2 != 0.0F);
}

bool LightsPage::EndTool(Editor *editor, EditorTrackData *trackData)
{
	editor->CommitNewNode(trackData, TrackTool(editor, trackData));
	return (true);
}


SourcesPage::SourcesPage() :
		EditorPage(kEditorPageSources, "WorldEditor/source/Sources"),
		sourceButtonObserver(this, &SourcesPage::HandleSourceButtonEvent)
{
	currentTool = -1;
}

SourcesPage::~SourcesPage()
{
}

void SourcesPage::Preprocess(void)
{
	static const char *const buttonIdentifier[kEditorSourceCount] =
	{
		"Ambient", "Omni", "Directed"
	};
	
	EditorPage::Preprocess();
	
	for (machine a = 0; a < kEditorSourceCount; a++)
	{
		sourceButton[a] = static_cast<IconButtonWidget *>(FindWidget(buttonIdentifier[a]));
		sourceButton[a]->SetObserver(&sourceButtonObserver);
	}
}

void SourcesPage::HandleSourceButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		GetEditor()->SetCurrentTool(this, widget);
	}
}

void SourcesPage::Engage(Editor *editor, void *cookie)
{
	Widget *widget = static_cast<Widget *>(cookie);
	
	for (machine a = 0; a < kEditorSourceCount; a++)
	{
		if (widget == sourceButton[a])
		{
			currentTool = a;
			sourceButton[a]->SetValue(1);
			break;
		}
	}
	
	editor->SetCurrentCursor(TheWorldEditor->GetEditorCursor(kEditorCursorCross));
}

void SourcesPage::Disengage(Editor *editor, void *cookie)
{
	if (currentTool != -1)
	{
		sourceButton[currentTool]->SetValue(0);
		currentTool = -1;
	}
}

bool SourcesPage::BeginTool(Editor *editor, EditorTrackData *trackData)
{
	if (trackData->viewportType == kEditorViewportOrtho)
	{
		Source *source = nullptr;
		
		switch (currentTool)
		{
			case kEditorSourceAmbient:
				
				source = new AmbientSource(nullptr, true);
				break;
			
			case kEditorSourceOmni:
				
				source = new OmniSource("C4/missing", 0.0F, true);
				break;
			
			case kEditorSourceDirected:
				
				source = new DirectedSource("C4/missing", 0.0F, 1.0F, true);
				break;
		}
		
		if (source)
		{
			SourceObject *object = source->GetObject();
			object->SetSourceFlags(object->GetSourceFlags() | kSourceLoop);
			
			editor->InitNewNode(trackData, source);
			return (true);
		}
	}
	
	return (false);
}

bool SourcesPage::TrackTool(Editor *editor, EditorTrackData *trackData)
{
	editor->AutoScroll(trackData);
	
	Source *source = static_cast<Source *>(trackData->trackNode);
	SourceType sourceType = source->GetSourceType();
	
	if (sourceType != kSourceAmbient)
	{
		float dx = trackData->currentPosition.x - trackData->snappedAnchorPosition.x;
		float dy = trackData->snappedAnchorPosition.y - trackData->currentPosition.y;
		
		if ((InterfaceMgr::GetShiftKey()) && (sourceType != kSourceOmni))
		{
			if (Fabs(dx) > Fabs(dy)) dy = 0.0F;
			else dx = 0.0F;
		}
		
		float r2 = dx * dx + dy * dy;
		if ((dx != trackData->currentSize.x) || (dy != trackData->currentSize.y))
		{
			trackData->currentSize.Set(dx, dy);
			static_cast<OmniSourceObject *>(source->GetObject())->SetSourceRange(Sqrt(r2));
			
			if ((sourceType == kSourceDirected) && (r2 != 0.0F))
			{
				Vector3D direction = editor->GetTargetSpaceDirection(trackData, Vector3D(dx, -dy, 0.0F));
				Vector3D down = editor->GetTargetSpaceDirection(trackData, Vector3D(0.0F, 0.0F, 1.0F));
				
				direction.Normalize();
				down = (down - ProjectOnto(down, direction)).Normalize();
				
				source->SetNodeMatrix3D(down % direction, down, direction);
			}
			
			editor->InvalidateNode(source);
		}
		
		return (r2 != 0.0F);
	}
	
	return (true);
}

bool SourcesPage::EndTool(Editor *editor, EditorTrackData *trackData)
{
	editor->CommitNewNode(trackData, TrackTool(editor, trackData));
	return (true);
}


ZonesPage::ZonesPage() :
		EditorPage(kEditorPageZones, "WorldEditor/zone/Zones"),
		zoneButtonObserver(this, &ZonesPage::HandleZoneButtonEvent)
{
	currentTool = -1;
}

ZonesPage::~ZonesPage()
{
}

void ZonesPage::Preprocess(void)
{
	static const char *const zoneButtonIdentifier[kEditorZoneCount] =
	{
		"Box", "Cylinder", "Dome", "Polygon"
	};
	
	static const char *const toolButtonIdentifier[kEditorZoneToolCount] =
	{
		"Insert", "Remove"
	};
	
	EditorPage::Preprocess();
	
	for (machine a = 0; a < kEditorZoneCount; a++)
	{
		zoneButton[a] = static_cast<IconButtonWidget *>(FindWidget(zoneButtonIdentifier[a]));
		zoneButton[a]->SetObserver(&zoneButtonObserver);
	}
	
	for (machine a = 0; a < kEditorZoneToolCount; a++)
	{
		toolButton[a] = static_cast<IconButtonWidget *>(FindWidget(toolButtonIdentifier[a]));
		toolButton[a]->SetObserver(&zoneButtonObserver);
	}
}

void ZonesPage::HandleZoneButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		GetEditor()->SetCurrentTool(this, widget);
	}
}

void ZonesPage::Engage(Editor *editor, void *cookie)
{
	Widget *widget = static_cast<Widget *>(cookie);
	
	for (machine a = 0; a < kEditorZoneCount; a++)
	{
		if (widget == zoneButton[a])
		{
			currentMode = kEditorZoneModeDraw;
			currentTool = a;
			zoneButton[a]->SetValue(1);
			
			editor->SetCurrentCursor(TheWorldEditor->GetEditorCursor(kEditorCursorCross));
			return;
		}
	}
	
	for (machine a = 0; a < kEditorZoneToolCount; a++)
	{
		if (widget == toolButton[a])
		{
			currentMode = kEditorZoneModeTool;
			currentTool = a;
			toolButton[a]->SetValue(1);
			
			editor->SetRenderFlags(editor->GetRenderFlags() | kEditorRenderHandles);
			editor->SetCurrentCursor(TheWorldEditor->GetEditorCursor((a == 0) ? kEditorCursorInsert : kEditorCursorRemove));
			return;
		}
	}
}

void ZonesPage::Disengage(Editor *editor, void *cookie)
{
	if (currentTool != -1)
	{
		if (currentMode == kEditorZoneModeDraw)
		{
			zoneButton[currentTool]->SetValue(0);
		}
		else
		{
			toolButton[currentTool]->SetValue(0);
			editor->SetRenderFlags(editor->GetRenderFlags() & ~kEditorRenderHandles);
		}
		
		currentTool = -1;
	}
}

bool ZonesPage::BeginTool(Editor *editor, EditorTrackData *trackData)
{
	if (currentMode == kEditorZoneModeDraw)
	{
		if (trackData->viewportType == kEditorViewportOrtho)
		{
			Zone *zone = nullptr;
			
			switch (currentTool)
			{
				case kEditorZoneBox:
					
					zone = new BoxZone(Vector3D(0.0F, 0.0F, 0.0F));
					break;
				
				case kEditorZoneCylinder:
					
					zone = new CylinderZone(Vector2D(0.0F, 0.0F), 0.0F);
					break;
				
				case kEditorZoneDome:
					
					zone = new DomeZone(Vector3D(0.0F, 0.0F, 0.0F));
					break;
				
				case kEditorZonePolygon:
					
					zone = new PolygonZone(Vector2D(0.0F, 0.0F), 0.0F);
					break;
			}
			
			if (zone)
			{
				editor->InitNewNode(trackData, zone);
				return (true);
			}
		}
	}
	else
	{
		int32	handleIndex;
		
		Node *handleNode = editor->PickHandle(trackData, &handleIndex);
		
		if (currentTool == kEditorZoneToolInsert)
		{
			if (!handleNode)
			{
				PickData	pickData;
				
				Node *selectNode = editor->PickNode(trackData, &pickData, 1 << kEditorNodeZone);
				if (selectNode)
				{
					Zone *zone = static_cast<Zone *>(selectNode);
					if (zone->GetZoneType() == kZonePolygon)
					{
						PolygonZone *polygon = static_cast<PolygonZone *>(zone);
						PolygonZoneObject *object = polygon->GetObject();
						
						int32 vertexCount = object->GetVertexCount();
						if (vertexCount < kMaxZoneVertexCount)
						{
							int32 index = pickData.pickIndex[0];
							if (index >= 0)
							{
								editor->AddUndoData(new ZoneVertexUndoData(polygon));
								
								Point3D *vertex = object->GetVertexArray();
								for (machine a = vertexCount - 1; a >= index; a--) vertex[a + 1] = vertex[a];
								
								object->SetVertexCount(vertexCount + 1);
								vertex[index] = pickData.pickPoint;
								polygon->Invalidate();
								
								trackData->resizeData.resizeFlags = 0;
								trackData->resizeData.handleFlags = 0;
								trackData->resizeData.handleIndex = index;
								
								Editor::GetManipulator(polygon)->BeginResize(&trackData->resizeData);
								trackData->trackNode = polygon;
								return (true);
							}
						}
					}
				}
			}
		}
		else if (currentTool == kEditorZoneToolRemove)
		{
			if ((handleNode) && (handleNode->GetNodeType() == kNodeZone))
			{
				Zone *zone = static_cast<Zone *>(handleNode);
				if (zone->GetZoneType() == kZonePolygon)
				{
					PolygonZone *polygon = static_cast<PolygonZone *>(zone);
					PolygonZoneObject *object = polygon->GetObject();
					
					int32 vertexCount = object->GetVertexCount();
					if (vertexCount > 3)
					{
						editor->AddUndoData(new ZoneVertexUndoData(polygon));
						
						Point3D *vertex = object->GetVertexArray();
						for (machine a = handleIndex + 1; a < vertexCount; a++) vertex[a - 1] = vertex[a];
						
						object->SetVertexCount(vertexCount - 1);
						polygon->Invalidate();
					}
				}
			}
		}
	}
	
	return (false);
}

bool ZonesPage::TrackTool(Editor *editor, EditorTrackData *trackData)
{
	if (currentMode == kEditorZoneModeDraw)
	{
		editor->AutoScroll(trackData);
		
		Point2D anchor = trackData->snappedAnchorPosition;
		float dx = trackData->snappedCurrentPosition.x - anchor.x;
		float dy = anchor.y - trackData->snappedCurrentPosition.y;
		
		float ax = Fabs(dx);
		float ay = Fabs(dy);
		
		if (InterfaceMgr::GetShiftKey())
		{
			ax = ay = Fmax(ax, ay);
			dx = (dx < 0.0F) ? -ax : ax;
			dy = (dy < 0.0F) ? -ax : ax;
		}
		
		if ((dx != trackData->currentSize.x) || (dy != trackData->currentSize.y))
		{
			trackData->currentSize.Set(dx, dy);
			
			unsigned_int32 editorFlags = editor->GetEditorObject()->GetEditorFlags();
			
			Zone *zone = static_cast<Zone *>(trackData->trackNode);
			ZoneType zoneType = zone->GetZoneType();
			
			if ((zoneType == kZoneBox) || (zoneType == kZonePolygon))
			{
				if (editorFlags & kEditorDrawFromCenter)
				{
					anchor.x -= ax;
					anchor.y += ay;
					dx = ax * 2.0F;
					dy = ay * 2.0F;
				}
				else
				{
					if (dx < 0.0F)
					{
						anchor.x += dx;
						dx = -dx;
					}
					
					if (dy < 0.0F)
					{
						anchor.y -= dy;
						dy = -dy;
					}
				}
				
				zone->SetNodePosition(editor->GetTargetSpacePosition(trackData, anchor));
				
				if (zoneType == kZoneBox)
				{
					const BoxZone *box = static_cast<BoxZone *>(zone);
					box->GetObject()->SetBoxSize(Vector3D(dx, dy, Fmax(dx, dy)));
				}
				else
				{
					const PolygonZone *polygon = static_cast<PolygonZone *>(zone);
					polygon->GetObject()->SetPolygonSize(Vector2D(dx, dy), Fmax(dx, dy));
				}
			}
			else
			{
				if (editorFlags & kEditorDrawFromCenter)
				{
					dx = ax;
					dy = ay;
				}
				else
				{
					anchor.x += dx * 0.5F;
					anchor.y -= dy * 0.5F;
					dx = ax * 0.5F;
					dy = ay * 0.5F;
					
					zone->SetNodePosition(editor->GetTargetSpacePosition(trackData, anchor));
				}
				
				if (zoneType == kZoneCylinder)
				{
					const CylinderZone *cylinder = static_cast<CylinderZone *>(zone);
					CylinderZoneObject *object = cylinder->GetObject();
					object->SetCylinderSize(Vector2D(dx, dy));
					object->SetCylinderHeight(Fmax(dx, dy) * 2.0F);
				}
				else
				{
					const DomeZone *dome = static_cast<DomeZone *>(zone);
					dome->GetObject()->SetDomeSize(Vector3D(dx, dy, Fmax(dx, dy)));
				}
			}
			
			editor->InvalidateNode(zone);
		}
		
		return ((dx != 0.0F) && (dy != 0.0F));
	}
	
	if (currentTool == kEditorZoneToolInsert)
	{
		editor->AutoScroll(trackData);
		
		if (trackData->currentPosition != trackData->previousPosition)
		{
			PolygonZone *polygon = static_cast<PolygonZone *>(trackData->trackNode);
			
			Vector2D delta = trackData->currentPosition - trackData->anchorPosition;
			trackData->resizeData.resizeDelta = polygon->GetInverseWorldTransform() * editor->GetWorldSpaceDirection(trackData, Vector3D(delta.x, delta.y, 0.0F));
			Editor::GetManipulator(polygon)->Resize(&trackData->resizeData);
			editor->InvalidateNode(polygon);
		}
	}
	
	return (true);
}

bool ZonesPage::EndTool(Editor *editor, EditorTrackData *trackData)
{
	bool result = TrackTool(editor, trackData);
	if (currentMode == kEditorZoneModeDraw) editor->CommitNewNode(trackData, result);
	return (true);
}


PortalsPage::PortalsPage() :
		EditorPage(kEditorPagePortals, "WorldEditor/portal/Portals"),
		portalButtonObserver(this, &PortalsPage::HandlePortalButtonEvent)
{
	currentTool = -1;
}

PortalsPage::~PortalsPage()
{
}

void PortalsPage::Preprocess(void)
{
	static const char *const portalButtonIdentifier[kEditorPortalCount] =
	{
		"Direct", "Remote", "Occlusion"
	};
	
	static const char *const toolButtonIdentifier[kEditorPortalToolCount] =
	{
		"Insert", "Remove"
	};
	
	EditorPage::Preprocess();
	
	for (machine a = 0; a < kEditorPortalCount; a++)
	{
		portalButton[a] = static_cast<IconButtonWidget *>(FindWidget(portalButtonIdentifier[a]));
		portalButton[a]->SetObserver(&portalButtonObserver);
	}
	
	for (machine a = 0; a < kEditorPortalToolCount; a++)
	{
		toolButton[a] = static_cast<IconButtonWidget *>(FindWidget(toolButtonIdentifier[a]));
		toolButton[a]->SetObserver(&portalButtonObserver);
	}
}

void PortalsPage::HandlePortalButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		GetEditor()->SetCurrentTool(this, widget);
	}
}

void PortalsPage::Engage(Editor *editor, void *cookie)
{
	Widget *widget = static_cast<Widget *>(cookie);
	
	for (machine a = 0; a < kEditorPortalCount; a++)
	{
		if (widget == portalButton[a])
		{
			currentMode = kEditorPortalModeDraw;
			currentTool = a;
			portalButton[a]->SetValue(1);
			
			editor->SetCurrentCursor(TheWorldEditor->GetEditorCursor(kEditorCursorCross));
			return;
		}
	}
	
	for (machine a = 0; a < kEditorPortalToolCount; a++)
	{
		if (widget == toolButton[a])
		{
			currentMode = kEditorPortalModeTool;
			currentTool = a;
			toolButton[a]->SetValue(1);
			
			editor->SetRenderFlags(editor->GetRenderFlags() | kEditorRenderHandles);
			editor->SetCurrentCursor(TheWorldEditor->GetEditorCursor((a == 0) ? kEditorCursorInsert : kEditorCursorRemove));
			return;
		}
	}
}

void PortalsPage::Disengage(Editor *editor, void *cookie)
{
	if (currentTool != -1)
	{
		if (currentMode == kEditorPortalModeDraw)
		{
			portalButton[currentTool]->SetValue(0);
		}
		else
		{
			toolButton[currentTool]->SetValue(0);
			editor->SetRenderFlags(editor->GetRenderFlags() & ~kEditorRenderHandles);
		}
		
		currentTool = -1;
	}
}

bool PortalsPage::BeginTool(Editor *editor, EditorTrackData *trackData)
{
	if (currentMode == kEditorPortalModeDraw)
	{
		if (trackData->viewportType == kEditorViewportOrtho)
		{
			Portal *portal = nullptr;
			
			switch (currentTool)
			{
				case kEditorPortalDirect:
					
					portal = new DirectPortal(Vector2D(0.0F, 0.0F));
					portal->AddConnector(kConnectorKeyZone);
					break;
				
				case kEditorPortalRemote:
					
					portal = new RemotePortal(Vector2D(0.0F, 0.0F));
					break;
				
				case kEditorPortalOcclusion:
					
					portal = new OcclusionPortal(Vector2D(0.0F, 0.0F));
					break;
			}
			
			if (portal)
			{
				editor->InitNewNode(trackData, portal);
				return (true);
			}
		}
	}
	else
	{
		int32	handleIndex;
		
		Node *handleNode = editor->PickHandle(trackData, &handleIndex);
		
		if (currentTool == kEditorPortalToolInsert)
		{
			if (!handleNode)
			{
				PickData	pickData;
				
				Node *selectNode = editor->PickNode(trackData, &pickData, 1 << kEditorNodePortal);
				if (selectNode)
				{
					Portal *portal = static_cast<Portal *>(selectNode);
					PortalObject *object = portal->GetObject();
					
					int32 vertexCount = object->GetVertexCount();
					if (vertexCount < kMaxPortalVertexCount)
					{
						int32 index = pickData.pickIndex[0];
						if (index >= 0)
						{
							editor->AddUndoData(new PortalVertexUndoData(portal));
							
							Point3D *vertex = object->GetVertexArray();
							for (machine a = vertexCount - 1; a >= index; a--) vertex[a + 1] = vertex[a];
							
							object->SetVertexCount(vertexCount + 1);
							vertex[index] = pickData.pickPoint;
							portal->Invalidate();
							
							trackData->resizeData.resizeFlags = 0;
							trackData->resizeData.handleFlags = 0;
							trackData->resizeData.handleIndex = index;
							
							Editor::GetManipulator(portal)->BeginResize(&trackData->resizeData);
							trackData->trackNode = portal;
							return (true);
						}
					}
				}
			}
		}
		else if (currentTool == kEditorPortalToolRemove)
		{
			if ((handleNode) && (handleNode->GetNodeType() == kNodePortal))
			{
				Portal *portal = static_cast<Portal *>(handleNode);
				PortalObject *object = portal->GetObject();
				
				int32 vertexCount = object->GetVertexCount();
				if (vertexCount > 3)
				{
					editor->AddUndoData(new PortalVertexUndoData(portal));
					
					Point3D *vertex = object->GetVertexArray();
					for (machine a = handleIndex + 1; a < vertexCount; a++) vertex[a - 1] = vertex[a];
					
					object->SetVertexCount(vertexCount - 1);
					portal->Invalidate();
				}
			}
		}
	}
	
	return (false);
}

bool PortalsPage::TrackTool(Editor *editor, EditorTrackData *trackData)
{
	if (currentMode == kEditorPortalModeDraw)
	{
		editor->AutoScroll(trackData);
		
		Point2D anchor = trackData->snappedAnchorPosition;
		float dx = trackData->snappedCurrentPosition.x - anchor.x;
		float dy = anchor.y - trackData->snappedCurrentPosition.y;
		
		float ax = Fabs(dx);
		float ay = Fabs(dy);
		
		if (InterfaceMgr::GetShiftKey())
		{
			ax = ay = Fmax(ax, ay);
			dx = (dx < 0.0F) ? -ax : ax;
			dy = (dy < 0.0F) ? -ax : ax;
		}
		
		if ((dx != trackData->currentSize.x) || (dy != trackData->currentSize.y))
		{
			trackData->currentSize.Set(dx, dy);
			
			unsigned_int32 editorFlags = editor->GetEditorObject()->GetEditorFlags();
			if (editorFlags & kEditorDrawFromCenter)
			{
				anchor.x -= ax;
				anchor.y += ay;
				dx = ax * 2.0F;
				dy = ay * 2.0F;
			}
			else
			{
				if (dx < 0.0F)
				{
					anchor.x += dx;
					dx = -dx;
				}
				
				if (dy < 0.0F)
				{
					anchor.y -= dy;
					dy = -dy;
				}
			}
			
			Portal *portal = static_cast<Portal *>(trackData->trackNode);
			portal->SetNodePosition(editor->GetTargetSpacePosition(trackData, anchor));
			portal->GetObject()->SetPortalSize(Vector2D(dx, dy));
			editor->InvalidateNode(portal);
		}
		
		return ((dx != 0.0F) && (dy != 0.0F));
	}
	
	if (currentTool == kEditorPortalToolInsert)
	{
		editor->AutoScroll(trackData);
		
		if (trackData->currentPosition != trackData->previousPosition)
		{
			Portal *portal = static_cast<Portal *>(trackData->trackNode);
			
			Vector2D delta = trackData->currentPosition - trackData->anchorPosition;
			trackData->resizeData.resizeDelta = portal->GetInverseWorldTransform() * editor->GetWorldSpaceDirection(trackData, Vector3D(delta.x, delta.y, 0.0F));
			Editor::GetManipulator(portal)->Resize(&trackData->resizeData);
			editor->InvalidateNode(portal);
		}
	}
	
	return (true);
}

bool PortalsPage::EndTool(Editor *editor, EditorTrackData *trackData)
{
	bool result = TrackTool(editor, trackData);
	if (currentMode == kEditorPortalModeDraw) editor->CommitNewNode(trackData, result);
	return (true);
}


SpacesPage::SpacesPage() :
		EditorPage(kEditorPageSpaces, "WorldEditor/space/Spaces"),
		spaceButtonObserver(this, &SpacesPage::HandleSpaceButtonEvent)
{
	currentTool = -1;
}

SpacesPage::~SpacesPage()
{
}

void SpacesPage::Preprocess(void)
{
	static const char *const buttonIdentifier[kEditorSpaceCount] =
	{
		"Fog", "Shadow", "Ambient", "Acoustics", "Occlusion", "Paint"
	};
	
	EditorPage::Preprocess();
	
	for (machine a = 0; a < kEditorSpaceCount; a++)
	{
		spaceButton[a] = static_cast<IconButtonWidget *>(FindWidget(buttonIdentifier[a]));
		spaceButton[a]->SetObserver(&spaceButtonObserver);
	}
}

void SpacesPage::HandleSpaceButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		GetEditor()->SetCurrentTool(this, widget);
	}
}

void SpacesPage::Engage(Editor *editor, void *cookie)
{
	Widget *widget = static_cast<Widget *>(cookie);
	
	for (machine a = 0; a < kEditorSpaceCount; a++)
	{
		if (widget == spaceButton[a])
		{
			currentTool = a;
			spaceButton[a]->SetValue(1);
			break;
		}
	}
	
	editor->SetCurrentCursor(TheWorldEditor->GetEditorCursor(kEditorCursorCross));
}

void SpacesPage::Disengage(Editor *editor, void *cookie)
{
	if (currentTool != -1)
	{
		spaceButton[currentTool]->SetValue(0);
		currentTool = -1;
	}
}

bool SpacesPage::BeginTool(Editor *editor, EditorTrackData *trackData)
{
	if (trackData->viewportType == kEditorViewportOrtho)
	{
		Space *space = nullptr;
		
		switch (currentTool)
		{
			case kEditorSpaceFog:
				
				space = new FogSpace(Vector2D(0.0F, 0.0F));
				break;
			
			case kEditorSpaceShadow:
				
				space = new ShadowSpace(Vector3D(0.0F, 0.0F, 0.0F));
				break;
			
			case kEditorSpaceAmbient:
				
				space = new AmbientSpace(Vector3D(0.0F, 0.0F, 0.0F), 16, 16, 16, "ambient");
				break;
			
			case kEditorSpaceAcoustics:
				
				space = new AcousticsSpace(Vector3D(0.0F, 0.0F, 0.0F));
				break;
			
			case kEditorSpaceOcclusion:
				
				space = new OcclusionSpace(Vector3D(0.0F, 0.0F, 0.0F));
				break;
			
			case kEditorSpacePaint:
				
				space = new PaintSpace(Vector3D(0.0F, 0.0F, 0.0F), Integer2D(128, 128), 1);
				break;
		}
		
		if (space)
		{
			editor->InitNewNode(trackData, space);
			return (true);
		}
	}
	
	return (false);
}

bool SpacesPage::TrackTool(Editor *editor, EditorTrackData *trackData)
{
	editor->AutoScroll(trackData);
	
	Point2D anchor = trackData->snappedAnchorPosition;
	float dx = trackData->snappedCurrentPosition.x - anchor.x;
	float dy = anchor.y - trackData->snappedCurrentPosition.y;
	
	float ax = Fabs(dx);
	float ay = Fabs(dy);
	
	if (InterfaceMgr::GetShiftKey())
	{
		ax = ay = Fmax(ax, ay);
		dx = (dx < 0.0F) ? -ax : ax;
		dy = (dy < 0.0F) ? -ax : ax;
	}
	
	if ((dx != trackData->currentSize.x) || (dy != trackData->currentSize.y))
	{
		trackData->currentSize.Set(dx, dy);
		
		unsigned_int32 editorFlags = editor->GetEditorObject()->GetEditorFlags();
		if (editorFlags & kEditorDrawFromCenter)
		{
			anchor.x -= ax;
			anchor.y += ay;
			dx = ax * 2.0F;
			dy = ay * 2.0F;
		}
		else
		{
			if (dx < 0.0F)
			{
				anchor.x += dx;
				dx = -dx;
			}
			
			if (dy < 0.0F)
			{
				anchor.y -= dy;
				dy = -dy;
			}
		}
		
		Space *space = static_cast<Space *>(trackData->trackNode);
		space->SetNodePosition(editor->GetTargetSpacePosition(trackData, anchor));
		
		if (space->GetSpaceType() == kSpaceFog)
		{
			PlateVolume *plate = static_cast<PlateVolume *>(space->GetObject()->GetVolume());
			plate->SetPlateSize(Vector2D(dx, dy));
		}
		else
		{
			BoxVolume *box = static_cast<BoxVolume *>(space->GetObject()->GetVolume());
			box->SetBoxSize(Vector3D(dx, dy, Fmax(dx, dy)));
		}
		
		editor->InvalidateNode(space);
	}
	
	return ((dx != 0.0F) && (dy != 0.0F));
}

bool SpacesPage::EndTool(Editor *editor, EditorTrackData *trackData)
{
	editor->CommitNewNode(trackData, TrackTool(editor, trackData));
	return (true);
}


MarkersPage::MarkersPage() :
		EditorPage(kEditorPageMarkers, "WorldEditor/marker/Markers"),
		markerButtonObserver(this, &MarkersPage::HandleMarkerButtonEvent)
{
	currentTool = -1;
}

MarkersPage::~MarkersPage()
{
}

void MarkersPage::Preprocess(void)
{
	static const char *const buttonIdentifier[kEditorMarkerCount] =
	{
		"Locator", "Connection", "Cube"
	};
	
	EditorPage::Preprocess();
	
	for (machine a = 0; a < kEditorMarkerCount; a++)
	{
		markerButton[a] = static_cast<IconButtonWidget *>(FindWidget(buttonIdentifier[a]));
		markerButton[a]->SetObserver(&markerButtonObserver);
	}
	
	locatorList = static_cast<ListWidget *>(FindWidget("List"));
	locatorList->SetObserver(&markerButtonObserver);
	locatorList->SetWidgetUsage(locatorList->GetWidgetUsage() & ~(kWidgetKeyboardFocus | kWidgetMouseWheel));
	
	Vector2D size = locatorList->GetNaturalListItemSize();
	
	const LocatorRegistration *registration = LocatorMarker::GetFirstRegistration();
	while (registration)
	{
		locatorList->InsertSortedListItem(new TypeWidget(size, registration->GetLocatorName(), registration->GetLocatorType()));
		registration = registration->Next();
	}
}

void MarkersPage::HandleMarkerButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		GetEditor()->SetCurrentTool(this, widget);
	}
}

void MarkersPage::Engage(Editor *editor, void *cookie)
{
	Widget *widget = static_cast<Widget *>(cookie);
	
	currentTool = -1;
	for (machine a = 0; a < kEditorMarkerCount; a++)
	{
		if (widget == markerButton[a])
		{
			currentTool = a;
			markerButton[a]->SetValue(1);
			break;
		}
	}
	
	editor->SetCurrentCursor(TheWorldEditor->GetEditorCursor(kEditorCursorCross));
}

void MarkersPage::Disengage(Editor *editor, void *cookie)
{
	if (currentTool != -1)
	{
		markerButton[currentTool]->SetValue(0);
		currentTool = -1;
	}
	else
	{
		if (cookie != locatorList) locatorList->UnselectAllListItems();
	}
}

bool MarkersPage::BeginTool(Editor *editor, EditorTrackData *trackData)
{
	if ((trackData->viewportType != kEditorViewportGraph) || (trackData->superNode))
	{
		Marker *marker = nullptr;
		
		if (currentTool != -1)
		{
			switch (currentTool)
			{
				case kEditorMarkerLocator:
					
					marker = new LocatorMarker(0);
					break;
				
				case kEditorMarkerConnection:
					
					marker = new ConnectionMarker;
					break;
				
				case kEditorMarkerCube:
					
					marker = new CubeMarker("C4/environment", kTextureRGBA8, 128);
					break;
			}
		}
		else
		{
			const Widget *widget = locatorList->GetFirstSelectedListItem();
			if (widget) marker = new LocatorMarker(static_cast<const TypeWidget *>(widget)->GetItemType());
		}
		
		if (marker)
		{
			editor->InitNewNode(trackData, marker);
			
			if (trackData->viewportType == kEditorViewportOrtho) return (true);
			editor->CommitNewNode(trackData, true);
		}
	}
	
	return (false);
}

bool MarkersPage::TrackTool(Editor *editor, EditorTrackData *trackData)
{
	editor->AutoScroll(trackData);
	
	float dx = trackData->currentPosition.x - trackData->snappedAnchorPosition.x;
	float dy = trackData->snappedAnchorPosition.y - trackData->currentPosition.y;
	
	if (InterfaceMgr::GetShiftKey())
	{
		if (Fabs(dx) > Fabs(dy)) dy = 0.0F;
		else dx = 0.0F;
	}
	
	if ((dx != trackData->currentSize.x) || (dy != trackData->currentSize.y))
	{
		trackData->currentSize.Set(dx, dy);
		
		Marker *marker = static_cast<Marker *>(trackData->trackNode);
		if (marker->GetMarkerType() == kMarkerLocator)
		{
			float scale = trackData->viewportScale * 8.0F;
			if (dx * dx + dy * dy > scale * scale)
			{
				Vector3D direction = editor->GetTargetSpaceDirection(trackData, Vector3D(dx, -dy, 0.0F));
				Vector3D up = editor->GetTargetSpaceDirection(trackData, Vector3D(0.0F, 0.0F, -1.0F));
				
				direction.Normalize();
				up = (up - ProjectOnto(up, direction)).Normalize();
				
				marker->SetNodeMatrix3D(direction, up % direction, up);
				editor->InvalidateNode(marker);
			}
		}
	}
	
	return (true);
}

bool MarkersPage::EndTool(Editor *editor, EditorTrackData *trackData)
{
	editor->CommitNewNode(trackData, TrackTool(editor, trackData));
	return (true);
}


TriggersPage::TriggersPage() :
		EditorPage(kEditorPageTriggers, "WorldEditor/trigger/Triggers"),
		triggerButtonObserver(this, &TriggersPage::HandleTriggerButtonEvent)
{
	currentTool = -1;
}

TriggersPage::~TriggersPage()
{
}

void TriggersPage::Preprocess(void)
{
	static const char *const buttonIdentifier[kEditorTriggerCount] =
	{
		"Box", "Cylinder", "Sphere"
	};
	
	EditorPage::Preprocess();
	
	for (machine a = 0; a < kEditorTriggerCount; a++)
	{
		triggerButton[a] = static_cast<IconButtonWidget *>(FindWidget(buttonIdentifier[a]));
		triggerButton[a]->SetObserver(&triggerButtonObserver);
	}
}

void TriggersPage::HandleTriggerButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		GetEditor()->SetCurrentTool(this, widget);
	}
}

void TriggersPage::Engage(Editor *editor, void *cookie)
{
	Widget *widget = static_cast<Widget *>(cookie);
	
	for (machine a = 0; a < kEditorTriggerCount; a++)
	{
		if (widget == triggerButton[a])
		{
			currentTool = a;
			triggerButton[a]->SetValue(1);
			break;
		}
	}
	
	editor->SetCurrentCursor(TheWorldEditor->GetEditorCursor(kEditorCursorCross));
}

void TriggersPage::Disengage(Editor *editor, void *cookie)
{
	if (currentTool != -1)
	{
		triggerButton[currentTool]->SetValue(0);
		currentTool = -1;
	}
}

bool TriggersPage::BeginTool(Editor *editor, EditorTrackData *trackData)
{
	if (trackData->viewportType == kEditorViewportOrtho)
	{
		Trigger *trigger = nullptr;
		
		switch (currentTool)
		{
			case kEditorTriggerBox:
				
				trigger = new BoxTrigger(Vector3D(0.0F, 0.0F, 0.0F));
				break;
			
			case kEditorTriggerCylinder:
				
				trigger = new CylinderTrigger(Vector2D(0.0F, 0.0F), 0.0F);
				break;
			
			case kEditorTriggerSphere:
				
				trigger = new SphereTrigger(Vector3D(0.0F, 0.0F, 0.0F));
				break;
		}
		
		if (trigger)
		{
			editor->InitNewNode(trackData, trigger);
			return (true);
		}
	}
	
	return (false);
}

bool TriggersPage::TrackTool(Editor *editor, EditorTrackData *trackData)
{
	editor->AutoScroll(trackData);
	
	Point2D anchor = trackData->snappedAnchorPosition;
	float dx = trackData->snappedCurrentPosition.x - anchor.x;
	float dy = anchor.y - trackData->snappedCurrentPosition.y;
	
	float ax = Fabs(dx);
	float ay = Fabs(dy);
	
	if (InterfaceMgr::GetShiftKey())
	{
		ax = ay = Fmax(ax, ay);
		dx = (dx < 0.0F) ? -ax : ax;
		dy = (dy < 0.0F) ? -ax : ax;
	}
	
	if ((dx != trackData->currentSize.x) || (dy != trackData->currentSize.y))
	{
		trackData->currentSize.Set(dx, dy);
		
		unsigned_int32 editorFlags = editor->GetEditorObject()->GetEditorFlags();
		
		Trigger *trigger = static_cast<Trigger *>(trackData->trackNode);
		TriggerType triggerType = trigger->GetTriggerType();
		
		if (triggerType == kTriggerBox)
		{
			if (editorFlags & kEditorDrawFromCenter)
			{
				anchor.x -= ax;
				anchor.y += ay;
				dx = ax * 2.0F;
				dy = ay * 2.0F;
			}
			else
			{
				if (dx < 0.0F)
				{
					anchor.x += dx;
					dx = -dx;
				}
				
				if (dy < 0.0F)
				{
					anchor.y -= dy;
					dy = -dy;
				}
			}
			
			trigger->SetNodePosition(editor->GetTargetSpacePosition(trackData, anchor));
			
			const BoxTrigger *box = static_cast<BoxTrigger *>(trigger);
			box->GetObject()->SetBoxSize(Vector3D(dx, dy, Fmax(dx, dy)));
		}
		else
		{
			if (editorFlags & kEditorDrawFromCenter)
			{
				dx = ax;
				dy = ay;
			}
			else
			{
				anchor.x += dx * 0.5F;
				anchor.y -= dy * 0.5F;
				dx = ax * 0.5F;
				dy = ay * 0.5F;
				
				trigger->SetNodePosition(editor->GetTargetSpacePosition(trackData, anchor));
			}
			
			switch (triggerType)
			{
				case kTriggerCylinder:
				{
					const CylinderTrigger *cylinder = static_cast<CylinderTrigger *>(trigger);
					CylinderTriggerObject *object = cylinder->GetObject();
					object->SetCylinderSize(Vector2D(dx, dy));
					object->SetCylinderHeight(Fmax(dx, dy) * 2.0F);
					break;
				}
				
				case kTriggerSphere:
				{
					const SphereTrigger *sphere = static_cast<SphereTrigger *>(trigger);
					SphereTriggerObject *object = sphere->GetObject();
					object->SetSphereSize(Vector3D(dx, dy, Fmax(dx, dy)));
					break;
				}
			}
		}
		
		editor->InvalidateNode(trigger);
	}
	
	return ((dx != 0.0F) && (dy != 0.0F));
}

bool TriggersPage::EndTool(Editor *editor, EditorTrackData *trackData)
{
	editor->CommitNewNode(trackData, TrackTool(editor, trackData));
	return (true);
}


EffectsPage::EffectsPage() :
		EditorPage(kEditorPageEffects, "WorldEditor/effect/Effects"),
		effectButtonObserver(this, &EffectsPage::HandleEffectButtonEvent)
{
	currentTool = -1;
}

EffectsPage::~EffectsPage()
{
}

void EffectsPage::Preprocess(void)
{
	static const char *const buttonIdentifier[kEditorEffectCount] =
	{
		"Quad", "Flare", "Beam", "Tube", "Fire", "Panel"
	};
	
	EditorPage::Preprocess();
	
	for (machine a = 0; a < kEditorEffectCount; a++)
	{
		effectButton[a] = static_cast<IconButtonWidget *>(FindWidget(buttonIdentifier[a]));
		effectButton[a]->SetObserver(&effectButtonObserver);
	}
}

void EffectsPage::HandleEffectButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		GetEditor()->SetCurrentTool(this, widget);
	}
}

void EffectsPage::Engage(Editor *editor, void *cookie)
{
	Widget *widget = static_cast<Widget *>(cookie);
	
	for (machine a = 0; a < kEditorEffectCount; a++)
	{
		if (widget == effectButton[a])
		{
			currentTool = a;
			effectButton[a]->SetValue(1);
			break;
		}
	}
	
	editor->SetCurrentCursor(TheWorldEditor->GetEditorCursor(kEditorCursorCross));
}

void EffectsPage::Disengage(Editor *editor, void *cookie)
{
	if (currentTool != -1)
	{
		effectButton[currentTool]->SetValue(0);
		currentTool = -1;
	}
}

bool EffectsPage::BeginTool(Editor *editor, EditorTrackData *trackData)
{
	if (trackData->viewportType == kEditorViewportOrtho)
	{
		Effect *effect = nullptr;
		
		switch (currentTool)
		{
			case kEditorEffectQuad:
				
				effect = new QuadEffect(0.0F, K::white, "texture/LightFlare");
				break;
			
			case kEditorEffectFlare:
				
				effect = new FlareEffect(0.0F, 0.15F, 0.1F, "texture/LightFlare");
				break;
			
			case kEditorEffectBeam:
				
				effect = new BeamEffect(0.0F, 0.0F, ColorRGBA(1.0F, 1.0F, 1.0F, 1.0F));
				break;
			
			case kEditorEffectTube:
			{
				const NodeReference *reference = editor->GetFirstSelectedNode();
				if ((reference) && (!reference->Next()))
				{
					Node *node = reference->GetNode();
					if (node->GetNodeType() == kNodeMarker)
					{
						Marker *marker = static_cast<Marker *>(node);
						if (marker->GetMarkerType() == kMarkerPath)
						{
							PathMarker *pathMarker = static_cast<PathMarker *>(marker);
							const Path *path = pathMarker->GetPath();
							
							TubeEffect *tubeEffect = new TubeEffect(path, 0.0F, ColorRGBA(1.0F, 1.0F, 1.0F, 1.0F));
							tubeEffect->SetConnectedPathMarker(pathMarker);
							effect = tubeEffect;
						}
					}
				}
				
				break;
			}
			
			case kEditorEffectFire:
				
				effect = new FireEffect(0.0F, 0.0F, 0.25F, 16, "fire/Flame");
				break;
			
			case kEditorEffectPanel:
				
				effect = new PanelEffect(Vector2D(0.0F, 0.0F));
				break;
		}
		
		if (effect)
		{
			editor->InitNewNode(trackData, effect);
			return (true);
		}
	}
	
	return (false);
}

bool EffectsPage::TrackTool(Editor *editor, EditorTrackData *trackData)
{
	editor->AutoScroll(trackData);
	
	Point2D anchor = trackData->snappedAnchorPosition;
	float dx = trackData->snappedCurrentPosition.x - anchor.x;
	float dy = anchor.y - trackData->snappedCurrentPosition.y;
	
	float ax = Fabs(dx);
	float ay = Fabs(dy);
	
	Effect *effect = static_cast<Effect *>(trackData->trackNode);
	EffectType effectType = effect->GetEffectType();
	
	if (effectType == kEffectPanel)
	{
		if (InterfaceMgr::GetShiftKey())
		{
			ax = ay = Fmax(ax, ay);
			dx = (dx < 0.0F) ? -ax : ax;
			dy = (dy < 0.0F) ? -ax : ax;
		}
	}
	else
	{
		if (ax > ay)
		{
			dy = (dy > 0.0F) ? ax : -ax;
			ay = ax;
		}
		else
		{
			dx = (dx > 0.0F) ? ay : -ay;
			ax = ay;
		}
	}
	
	if ((dx != trackData->currentSize.x) || (dy != trackData->currentSize.y))
	{
		trackData->currentSize.Set(dx, dy);
		
		unsigned_int32 editorFlags = editor->GetEditorObject()->GetEditorFlags();
		if (effectType == kEffectPanel)
		{
			if (editorFlags & kEditorDrawFromCenter)
			{
				anchor.x -= ax;
				anchor.y += ay;
				dx = ax * 2.0F;
				dy = ay * 2.0F;
			}
			else
			{
				if (dx < 0.0F)
				{
					anchor.x += dx;
					dx = -dx;
				}
				
				if (dy < 0.0F)
				{
					anchor.y -= dy;
					dy = -dy;
				}
			}
			
			effect->SetNodePosition(editor->GetTargetSpacePosition(trackData, anchor));
			
			const PanelEffect *panelEffect = static_cast<PanelEffect *>(effect);
			panelEffect->GetObject()->SetExternalPanelSize(Vector2D(dx, dy));
		}
		else
		{
			if (editorFlags & kEditorDrawFromCenter)
			{
				dx = ax;
				dy = ay;
			}
			else
			{
				anchor.x += dx * 0.5F;
				anchor.y -= dy * 0.5F;
				dx = ax * 0.5F;
				dy = ay * 0.5F;
				
				effect->SetNodePosition(editor->GetTargetSpacePosition(trackData, anchor));
			}
			
			switch (effectType)
			{
				case kEffectQuad:
				{
					const QuadEffect *quadEffect = static_cast<QuadEffect *>(effect);
					QuadEffectObject *object = quadEffect->GetObject();
					object->SetQuadRadius(dx);
					break;
				}
				
				case kEffectFlare:
				{
					const FlareEffect *flareEffect = static_cast<FlareEffect *>(effect);
					FlareEffectObject *object = flareEffect->GetObject();
					object->SetFlareRadius(dx);
					break;
				}
				
				case kEffectBeam:
				{
					BeamEffect *beamEffect = static_cast<BeamEffect *>(effect);
					BeamEffectObject *object = beamEffect->GetObject();
					object->SetBeamRadius(dx);
					object->SetBeamHeight(dx * 4.0F);
					beamEffect->Preprocess();
					break;
				}
				
				case kEffectTube:
				{
					const TubeEffect *tubeEffect = static_cast<TubeEffect *>(effect);
					TubeEffectObject *object = tubeEffect->GetObject();
					object->SetTubeRadius(dx);
					break;
				}
				
				case kEffectFire:
				{
					const FireEffect *fireEffect = static_cast<FireEffect *>(effect);
					FireEffectObject *object = fireEffect->GetObject();
					object->SetFireRadius(dx);
					object->SetFireHeight(dx * 2.0F);
					break;
				}
			}
		}
		
		editor->InvalidateNode(effect);
		if (effectType == kEffectTube) static_cast<TubeEffect *>(effect)->GetObject()->Build();
	}
	
	return ((dx != 0.0F) && (dy != 0.0F));
}

bool EffectsPage::EndTool(Editor *editor, EditorTrackData *trackData)
{
	editor->CommitNewNode(trackData, TrackTool(editor, trackData));
	return (true);
}


ParticlesPage::ParticlesPage() :
		EditorPage(kEditorPageParticles, "WorldEditor/particle/Particles"),
		emitterButtonObserver(this, &ParticlesPage::HandleEmitterButtonEvent)
{
	currentTool = -1;
}

ParticlesPage::~ParticlesPage()
{
}

void ParticlesPage::Preprocess(void)
{
	static const char *const buttonIdentifier[kEditorEmitterCount] =
	{
		"Box", "Cylinder", "Sphere"
	};
	
	EditorPage::Preprocess();
	
	for (machine a = 0; a < kEditorEmitterCount; a++)
	{
		emitterButton[a] = static_cast<IconButtonWidget *>(FindWidget(buttonIdentifier[a]));
		emitterButton[a]->SetObserver(&emitterButtonObserver);
	}
	
	particleSystemList = static_cast<ListWidget *>(FindWidget("List"));
	particleSystemList->SetObserver(&emitterButtonObserver);
	particleSystemList->SetWidgetUsage(particleSystemList->GetWidgetUsage() & ~(kWidgetKeyboardFocus | kWidgetMouseWheel));
	
	Vector2D size = particleSystemList->GetNaturalListItemSize();
	
	const ParticleSystemRegistration *registration = Registrable<ParticleSystem, ParticleSystemRegistration>::GetFirstRegistration();
	while (registration)
	{
		particleSystemList->InsertSortedListItem(new TypeWidget(size, registration->GetParticleSystemName(), registration->GetParticleSystemType()));
		registration = registration->Next();
	}
}

void ParticlesPage::HandleEmitterButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		GetEditor()->SetCurrentTool(this, widget);
	}
}

void ParticlesPage::Engage(Editor *editor, void *cookie)
{
	Widget *widget = static_cast<Widget *>(cookie);
	
	currentTool = -1;
	for (machine a = 0; a < kEditorEmitterCount; a++)
	{
		if (widget == emitterButton[a])
		{
			currentTool = a;
			emitterButton[a]->SetValue(1);
			break;
		}
	}
	
	editor->SetCurrentCursor(TheWorldEditor->GetEditorCursor(kEditorCursorCross));
}

void ParticlesPage::Disengage(Editor *editor, void *cookie)
{
	if (currentTool != -1)
	{
		emitterButton[currentTool]->SetValue(0);
		currentTool = -1;
	}
	else
	{
		if (cookie != particleSystemList) particleSystemList->UnselectAllListItems();
	}
}

bool ParticlesPage::BeginTool(Editor *editor, EditorTrackData *trackData)
{
	if (currentTool != -1)
	{
		if (trackData->viewportType == kEditorViewportOrtho)
		{
			Emitter *emitter = nullptr;
			
			switch (currentTool)
			{
				case kEditorEmitterBox:
					
					emitter = new BoxEmitter(Vector3D(0.0F, 0.0F, 0.0F));
					break;
				
				case kEditorEmitterCylinder:
					
					emitter = new CylinderEmitter(Vector2D(0.0F, 0.0F), 0.0F);
					break;
				
				case kEditorEmitterSphere:
					
					emitter = new SphereEmitter(Vector3D(0.0F, 0.0F, 0.0F));
					break;
			}
			
			if (emitter)
			{
				editor->InitNewNode(trackData, emitter);
				return (true);
			}
		}
	}
	else
	{
		if ((trackData->viewportType != kEditorViewportGraph) || (trackData->superNode))
		{
			const Widget *widget = particleSystemList->GetFirstSelectedListItem();
			if (widget)
			{
				ParticleSystem *particleSystem = ParticleSystem::New(static_cast<const TypeWidget *>(widget)->GetItemType());
				editor->InitNewNode(trackData, particleSystem);
				
				if (trackData->viewportType == kEditorViewportOrtho) return (true);
				editor->CommitNewNode(trackData, true);
			}
		}
	}
	
	return (false);
}

bool ParticlesPage::TrackTool(Editor *editor, EditorTrackData *trackData)
{
	editor->AutoScroll(trackData);
	
	Point2D anchor = trackData->snappedAnchorPosition;
	
	if (trackData->trackNode->GetNodeType() == kNodeEmitter)
	{
		float dx = trackData->snappedCurrentPosition.x - anchor.x;
		float dy = anchor.y - trackData->snappedCurrentPosition.y;
		
		float ax = Fabs(dx);
		float ay = Fabs(dy);
		
		if (InterfaceMgr::GetShiftKey())
		{
			ax = ay = Fmax(ax, ay);
			dx = (dx < 0.0F) ? -ax : ax;
			dy = (dy < 0.0F) ? -ax : ax;
		}
		
		if ((dx != trackData->currentSize.x) || (dy != trackData->currentSize.y))
		{
			trackData->currentSize.Set(dx, dy);
			
			unsigned_int32 editorFlags = editor->GetEditorObject()->GetEditorFlags();
			
			Emitter *emitter = static_cast<Emitter *>(trackData->trackNode);
			EmitterType emitterType = emitter->GetEmitterType();
			
			if (emitterType == kEmitterBox)
			{
				if (editorFlags & kEditorDrawFromCenter)
				{
					anchor.x -= ax;
					anchor.y += ay;
					dx = ax * 2.0F;
					dy = ay * 2.0F;
				}
				else
				{
					if (dx < 0.0F)
					{
						anchor.x += dx;
						dx = -dx;
					}
					
					if (dy < 0.0F)
					{
						anchor.y -= dy;
						dy = -dy;
					}
				}
				
				emitter->SetNodePosition(editor->GetTargetSpacePosition(trackData, anchor));
				
				const BoxEmitter *box = static_cast<BoxEmitter *>(emitter);
				box->GetObject()->SetBoxSize(Vector3D(dx, dy, Fmax(dx, dy)));
			}
			else
			{
				if (editorFlags & kEditorDrawFromCenter)
				{
					dx = ax;
					dy = ay;
				}
				else
				{
					anchor.x += dx * 0.5F;
					anchor.y -= dy * 0.5F;
					dx = ax * 0.5F;
					dy = ay * 0.5F;
					
					emitter->SetNodePosition(editor->GetTargetSpacePosition(trackData, anchor));
				}
				
				switch (emitterType)
				{
					case kEmitterCylinder:
					{
						const CylinderEmitter *cylinder = static_cast<CylinderEmitter *>(emitter);
						CylinderEmitterObject *object = cylinder->GetObject();
						object->SetCylinderSize(Vector2D(dx, dy));
						object->SetCylinderHeight(Fmax(dx, dy) * 2.0F);
						break;
					}
					
					case kEmitterSphere:
					{
						const SphereEmitter *sphere = static_cast<SphereEmitter *>(emitter);
						SphereEmitterObject *object = sphere->GetObject();
						object->SetSphereSize(Vector3D(dx, dy, Fmax(dx, dy)));
						break;
					}
				}
			}
			
			editor->InvalidateNode(emitter);
		}
		
		return ((dx != 0.0F) && (dy != 0.0F));
	}
	
	float dx = trackData->currentPosition.x - anchor.x;
	float dy = anchor.y - trackData->currentPosition.y;
	
	if (InterfaceMgr::GetShiftKey())
	{
		if (Fabs(dx) > Fabs(dy)) dy = 0.0F;
		else dx = 0.0F;
	}
	
	if ((dx != trackData->currentSize.x) || (dy != trackData->currentSize.y))
	{
		trackData->currentSize.Set(dx, dy);
		
		if (dx * dx + dy * dy != 0.0F)
		{
			Vector3D direction = editor->GetTargetSpaceDirection(trackData, Vector3D(dx, -dy, 0.0F));
			Vector3D up = editor->GetTargetSpaceDirection(trackData, Vector3D(0.0F, 0.0F, -1.0F));
			
			direction.Normalize();
			up = (up - ProjectOnto(up, direction)).Normalize();
			
			Node *node = trackData->trackNode;
			node->SetNodeMatrix3D(direction, up % direction, up);
			editor->InvalidateNode(node);
		}
	}
	
	return (true);
}

bool ParticlesPage::EndTool(Editor *editor, EditorTrackData *trackData)
{
	editor->CommitNewNode(trackData, TrackTool(editor, trackData));
	return (true);
}


PhysicsPage::PhysicsPage() :
		EditorPage(kEditorPagePhysics, "WorldEditor/physics/Physics"),
		physicsButtonObserver(this, &PhysicsPage::HandlePhysicsButtonEvent)
{
	currentTool = -1;
}

PhysicsPage::~PhysicsPage()
{
}

void PhysicsPage::Preprocess(void)
{
	static const char *const shapeButtonIdentifier[kEditorShapeCount] =
	{
		"Box", "Pyramid", "Cylinder", "Cone", "Sphere", "Dome", "Capsule", "TruncPyramid", "TruncCone", "TruncDome"
	};
	
	static const char *const jointButtonIdentifier[kEditorJointCount] =
	{
		"Spherical", "Universal", "Discal", "Revolute", "Cylindrical", "Prismatic"
	};
	
	static const char *const fieldButtonIdentifier[kEditorFieldCount] =
	{
		"BoxField", "CylinderField", "SphereField"
	};
	
	EditorPage::Preprocess();
	
	physicsButton = static_cast<IconButtonWidget *>(FindWidget("Physics"));
	physicsButton->SetObserver(&physicsButtonObserver);
	
	for (machine a = 0; a < kEditorShapeCount; a++)
	{
		shapeButton[a] = static_cast<IconButtonWidget *>(FindWidget(shapeButtonIdentifier[a]));
		shapeButton[a]->SetObserver(&physicsButtonObserver);
	}
	
	for (machine a = 0; a < kEditorJointCount; a++)
	{
		jointButton[a] = static_cast<IconButtonWidget *>(FindWidget(jointButtonIdentifier[a]));
		jointButton[a]->SetObserver(&physicsButtonObserver);
	}
	
	for (machine a = 0; a < kEditorFieldCount; a++)
	{
		fieldButton[a] = static_cast<IconButtonWidget *>(FindWidget(fieldButtonIdentifier[a]));
		fieldButton[a]->SetObserver(&physicsButtonObserver);
	}
}

void PhysicsPage::HandlePhysicsButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		GetEditor()->SetCurrentTool(this, widget);
	}
}

void PhysicsPage::Engage(Editor *editor, void *cookie)
{
	Widget *widget = static_cast<Widget *>(cookie);
	
	if (widget == physicsButton)
	{
		currentMode = kEditorPhysicsModePhysics;
		currentTool = 0;
		physicsButton->SetValue(1);
	}
	else
	{
		for (machine a = 0; a < kEditorShapeCount; a++)
		{
			if (widget == shapeButton[a])
			{
				currentMode = kEditorPhysicsModeShape;
				currentTool = a;
				shapeButton[a]->SetValue(1);
				goto end;
			}
		}
		
		for (machine a = 0; a < kEditorJointCount; a++)
		{
			if (widget == jointButton[a])
			{
				currentMode = kEditorPhysicsModeJoint;
				currentTool = a;
				jointButton[a]->SetValue(1);
				goto end;
			}
		}
		
		for (machine a = 0; a < kEditorFieldCount; a++)
		{
			if (widget == fieldButton[a])
			{
				currentMode = kEditorPhysicsModeField;
				currentTool = a;
				fieldButton[a]->SetValue(1);
				goto end;
			}
		}
	}
	
	end:
	editor->SetCurrentCursor(TheWorldEditor->GetEditorCursor(kEditorCursorCross));
}

void PhysicsPage::Disengage(Editor *editor, void *cookie)
{
	if (currentTool != -1)
	{
		if (currentMode == kEditorPhysicsModePhysics) physicsButton->SetValue(0);
		else if (currentMode == kEditorPhysicsModeShape) shapeButton[currentTool]->SetValue(0);
		else if (currentMode == kEditorPhysicsModeJoint) jointButton[currentTool]->SetValue(0);
		else if (currentMode == kEditorPhysicsModeField) fieldButton[currentTool]->SetValue(0);
		
		currentTool = -1;
	}
}

bool PhysicsPage::BeginTool(Editor *editor, EditorTrackData *trackData)
{
	if (currentMode == kEditorPhysicsModePhysics)
	{
		if ((trackData->viewportType != kEditorViewportGraph) || (trackData->superNode))
		{
			PhysicsNode *node = new PhysicsNode;
			node->SetController(new PhysicsController);
			
			editor->InitNewNode(trackData, node);
			editor->CommitNewNode(trackData, true);
		}
	}
	else if (currentMode == kEditorPhysicsModeShape)
	{
		if (trackData->viewportType == kEditorViewportOrtho)
		{
			Shape *shape = nullptr;
			
			switch (currentTool)
			{
				case kEditorShapeBox:
					
					shape = new BoxShape(Vector3D(0.0F, 0.0F, 0.0F));
					break;
				
				case kEditorShapePyramid:
					
					shape = new PyramidShape(Vector2D(0.0F, 0.0F), 0.0F);
					break;
				
				case kEditorShapeCylinder:
					
					shape = new CylinderShape(Vector2D(0.0F, 0.0F), 0.0F);
					break;
				
				case kEditorShapeCone:
					
					shape = new ConeShape(Vector2D(0.0F, 0.0F), 0.0F);
					break;
				
				case kEditorShapeSphere:
					
					shape = new SphereShape(Vector3D(0.0F, 0.0F, 0.0F));
					break;
				
				case kEditorShapeDome:
					
					shape = new DomeShape(Vector3D(0.0F, 0.0F, 0.0F));
					break;
				
				case kEditorShapeCapsule:
					
					shape = new CapsuleShape(Vector3D(0.0F, 0.0F, 0.0F), 0.0F);
					break;
				
				case kEditorShapeTruncatedPyramid:
					
					shape = new TruncatedPyramidShape(Vector2D(0.0F, 0.0F), 0.0F, 0.5F);
					break;
				
				case kEditorShapeTruncatedCone:
					
					shape = new TruncatedConeShape(Vector2D(0.0F, 0.0F), 0.0F, 0.5F);
					break;
				
				case kEditorShapeTruncatedDome:
					
					shape = new TruncatedDomeShape(Vector2D(0.0F, 0.0F), 0.0F, 0.5F);
					break;
			}
			
			if (shape)
			{
				shape->SetNodeFlags(kNodeAnimateInhibit);
				editor->InitNewNode(trackData, shape);
				return (true);
			}
		}
	}
	else if (currentMode == kEditorPhysicsModeJoint)
	{
		if ((trackData->viewportType != kEditorViewportGraph) || (trackData->superNode))
		{
			Joint *joint = nullptr;
			
			switch (currentTool)
			{
				case kEditorJointSpherical:
					
					joint = new SphericalJoint;
					break;
				
				case kEditorJointUniversal:
					
					joint = new UniversalJoint;
					break;
				
				case kEditorJointDiscal:
					
					joint = new DiscalJoint;
					break;
				
				case kEditorJointRevolute:
					
					joint = new RevoluteJoint;
					break;
				
				case kEditorJointCylindrical:
					
					joint = new CylindricalJoint;
					break;
				
				case kEditorJointPrismatic:
					
					joint = new PrismaticJoint;
					break;
			}
			
			if (joint)
			{
				editor->InitNewNode(trackData, joint);
				
				if (trackData->viewportType == kEditorViewportOrtho) return (true);
				editor->CommitNewNode(trackData, true);
			}
		}
	}
	else if (currentMode == kEditorPhysicsModeField)
	{
		if (trackData->viewportType == kEditorViewportOrtho)
		{
			Field *field = nullptr;
			
			switch (currentTool)
			{
				case kEditorFieldBox:
					
					field = new BoxField(Vector3D(0.0F, 0.0F, 0.0F));
					break;
				
				case kEditorFieldCylinder:
					
					field = new CylinderField(Vector2D(0.0F, 0.0F), 0.0F);
					break;
				
				case kEditorFieldSphere:
					
					field = new SphereField(Vector3D(0.0F, 0.0F, 0.0F));
					break;
			}
			
			if (field)
			{
				editor->InitNewNode(trackData, field);
				return (true);
			}
		}
	}
	
	return (false);
}

bool PhysicsPage::TrackTool(Editor *editor, EditorTrackData *trackData)
{
	editor->AutoScroll(trackData);
	
	Point2D anchor = trackData->snappedAnchorPosition;
	
	float dx = trackData->snappedCurrentPosition.x - anchor.x;
	float dy = anchor.y - trackData->snappedCurrentPosition.y;
	
	if (currentMode == kEditorPhysicsModeShape)
	{
		float ax = Fabs(dx);
		float ay = Fabs(dy);
		
		if (InterfaceMgr::GetShiftKey())
		{
			ax = ay = Fmax(ax, ay);
			dx = (dx < 0.0F) ? -ax : ax;
			dy = (dy < 0.0F) ? -ax : ax;
		}
		
		if ((dx != trackData->currentSize.x) || (dy != trackData->currentSize.y))
		{
			trackData->currentSize.Set(dx, dy);
			
			unsigned_int32 editorFlags = editor->GetEditorObject()->GetEditorFlags();
			
			Shape *shape = static_cast<Shape *>(trackData->trackNode);
			ShapeType shapeType = shape->GetShapeType();
			
			if ((shapeType == kShapeBox) || (shapeType == kShapePyramid) || (shapeType == kShapeTruncatedPyramid))
			{
				if (editorFlags & kEditorDrawFromCenter)
				{
					anchor.x -= ax;
					anchor.y += ay;
					dx = ax * 2.0F;
					dy = ay * 2.0F;
				}
				else
				{
					if (dx < 0.0F)
					{
						anchor.x += dx;
						dx = -dx;
					}
					
					if (dy < 0.0F)
					{
						anchor.y -= dy;
						dy = -dy;
					}
				}
				
				shape->SetNodePosition(editor->GetTargetSpacePosition(trackData, anchor));
				
				switch (shapeType)
				{
					case kShapeBox:
					{
						const BoxShape *box = static_cast<BoxShape *>(shape);
						box->GetObject()->SetBoxSize(Vector3D(dx, dy, Fmax(dx, dy)));
						break;
					}
					
					case kShapePyramid:
					{
						const PyramidShape *pyramid = static_cast<PyramidShape *>(shape);
						PyramidShapeObject *object = pyramid->GetObject();
						object->SetPyramidSize(Vector2D(dx, dy));
						object->SetPyramidHeight(Fmax(dx, dy));
						break;
					}
					
					case kShapeTruncatedPyramid:
					{
						const TruncatedPyramidShape *truncatedPyramid = static_cast<TruncatedPyramidShape *>(shape);
						TruncatedPyramidShapeObject *object = truncatedPyramid->GetObject();
						object->SetPyramidSize(Vector2D(dx, dy));
						object->SetPyramidHeight(Fmax(dx, dy));
						break;
					}
				}
			}
			else
			{
				if (editorFlags & kEditorDrawFromCenter)
				{
					dx = ax;
					dy = ay;
				}
				else
				{
					anchor.x += dx * 0.5F;
					anchor.y -= dy * 0.5F;
					dx = ax * 0.5F;
					dy = ay * 0.5F;
					
					shape->SetNodePosition(editor->GetTargetSpacePosition(trackData, anchor));
				}
				
				switch (shapeType)
				{
					case kShapeCylinder:
					{
						const CylinderShape *cylinder = static_cast<CylinderShape *>(shape);
						CylinderShapeObject *object = cylinder->GetObject();
						object->SetCylinderSize(Vector2D(dx, dy));
						object->SetCylinderHeight(Fmax(dx, dy) * 2.0F);
						break;
					}
					
					case kShapeCone:
					{
						const ConeShape *cone = static_cast<ConeShape *>(shape);
						ConeShapeObject *object = cone->GetObject();
						object->SetConeSize(Vector2D(dx, dy));
						object->SetConeHeight(Fmax(dx, dy) * 2.0F);
						break;
					}
					
					case kShapeSphere:
					{
						const SphereShape *sphere = static_cast<SphereShape *>(shape);
						SphereShapeObject *object = sphere->GetObject();
						object->SetSphereSize(Vector3D(dx, dy, Fmax(dx, dy)));
						break;
					}
					
					case kShapeDome:
					{
						const DomeShape *dome = static_cast<DomeShape *>(shape);
						DomeShapeObject *object = dome->GetObject();
						object->SetDomeSize(Vector3D(dx, dy, Fmax(dx, dy)));
						break;
					}
					
					case kShapeCapsule:
					{
						const CapsuleShape *capsule = static_cast<CapsuleShape *>(shape);
						CapsuleShapeObject *object = capsule->GetObject();
						float m = Fmax(dx, dy);
						object->SetCapsuleSize(Vector3D(dx, dy, m));
						object->SetCapsuleHeight(m * 2.0F);
						break;
					}
					
					case kShapeTruncatedCone:
					{
						const TruncatedConeShape *cone = static_cast<TruncatedConeShape *>(shape);
						TruncatedConeShapeObject *object = cone->GetObject();
						object->SetConeSize(Vector2D(dx, dy));
						object->SetConeHeight(Fmax(dx, dy) * 2.0F);
						break;
					}
					
					case kShapeTruncatedDome:
					{
						const TruncatedDomeShape *dome = static_cast<TruncatedDomeShape *>(shape);
						TruncatedDomeShapeObject *object = dome->GetObject();
						object->SetDomeSize(Vector2D(dx, dy));
						object->SetDomeHeight(Fmax(dx, dy));
						break;
					}
				}
			}
			
			editor->InvalidateNode(shape);
		}
		
		return ((dx != 0.0F) && (dy != 0.0F));
	}
	else if (currentMode == kEditorPhysicsModeJoint)
	{
		if (InterfaceMgr::GetShiftKey())
		{
			if (Fabs(dx) > Fabs(dy)) dy = 0.0F;
			else dx = 0.0F;
		}
		
		if ((dx != trackData->currentSize.x) || (dy != trackData->currentSize.y))
		{
			trackData->currentSize.Set(dx, dy);
			
			float scale = trackData->viewportScale * 8.0F;
			if (dx * dx + dy * dy > scale * scale)
			{
				Vector3D direction = editor->GetTargetSpaceDirection(trackData, Vector3D(dx, -dy, 0.0F));
				Vector3D up = editor->GetTargetSpaceDirection(trackData, Vector3D(0.0F, 0.0F, -1.0F));
				
				direction.Normalize();
				up = (up - ProjectOnto(up, direction)).Normalize();
				
				Node *node = trackData->trackNode;
				node->SetNodeMatrix3D(direction, up % direction, up);
				editor->InvalidateNode(node);
			}
		}
		
		return (true);
	}
	else if (currentMode == kEditorPhysicsModeField)
	{
		float ax = Fabs(dx);
		float ay = Fabs(dy);
		
		if (InterfaceMgr::GetShiftKey())
		{
			ax = ay = Fmax(ax, ay);
			dx = (dx < 0.0F) ? -ax : ax;
			dy = (dy < 0.0F) ? -ax : ax;
		}
		
		if ((dx != trackData->currentSize.x) || (dy != trackData->currentSize.y))
		{
			trackData->currentSize.Set(dx, dy);
			
			unsigned_int32 editorFlags = editor->GetEditorObject()->GetEditorFlags();
			
			Field *field = static_cast<Field *>(trackData->trackNode);
			FieldType fieldType = field->GetFieldType();
			
			if (fieldType == kFieldBox)
			{
				if (editorFlags & kEditorDrawFromCenter)
				{
					anchor.x -= ax;
					anchor.y += ay;
					dx = ax * 2.0F;
					dy = ay * 2.0F;
				}
				else
				{
					if (dx < 0.0F)
					{
						anchor.x += dx;
						dx = -dx;
					}
					
					if (dy < 0.0F)
					{
						anchor.y -= dy;
						dy = -dy;
					}
				}
				
				field->SetNodePosition(editor->GetTargetSpacePosition(trackData, anchor));
				
				const BoxField *box = static_cast<BoxField *>(field);
				box->GetObject()->SetBoxSize(Vector3D(dx, dy, Fmax(dx, dy)));
			}
			else
			{
				if (editorFlags & kEditorDrawFromCenter)
				{
					dx = ax;
					dy = ay;
				}
				else
				{
					anchor.x += dx * 0.5F;
					anchor.y -= dy * 0.5F;
					dx = ax * 0.5F;
					dy = ay * 0.5F;
					
					field->SetNodePosition(editor->GetTargetSpacePosition(trackData, anchor));
				}
				
				switch (fieldType)
				{
					case kFieldCylinder:
					{
						const CylinderField *cylinder = static_cast<CylinderField *>(field);
						CylinderFieldObject *object = cylinder->GetObject();
						object->SetCylinderSize(Vector2D(dx, dy));
						object->SetCylinderHeight(Fmax(dx, dy) * 2.0F);
						break;
					}
					
					case kFieldSphere:
					{
						const SphereField *sphere = static_cast<SphereField *>(field);
						SphereFieldObject *object = sphere->GetObject();
						object->SetSphereSize(Vector3D(dx, dy, Fmax(dx, dy)));
						break;
					}
				}
			}
			
			editor->InvalidateNode(field);
		}
		
		return ((dx != 0.0F) && (dy != 0.0F));
	}
	
	return (false);
}

bool PhysicsPage::EndTool(Editor *editor, EditorTrackData *trackData)
{
	editor->CommitNewNode(trackData, TrackTool(editor, trackData));
	return (true);
}


ModelsPage::ModelsPage() :
		EditorPage(kEditorPageModels, "WorldEditor/model/Models"),
		modelButtonObserver(this, &ModelsPage::HandleModelButtonEvent)
{
	currentTool = -1;
}

ModelsPage::~ModelsPage()
{
}

void ModelsPage::Preprocess(void)
{
	EditorPage::Preprocess();
	
	modelButton = static_cast<IconButtonWidget *>(FindWidget("Model"));
	modelButton->SetObserver(&modelButtonObserver);
	
	modelList = static_cast<ListWidget *>(FindWidget("List"));
	modelList->SetObserver(&modelButtonObserver);
	modelList->SetWidgetUsage(modelList->GetWidgetUsage() & ~(kWidgetKeyboardFocus | kWidgetMouseWheel));
	
	Vector2D size = modelList->GetNaturalListItemSize();
	
	const ModelRegistration *registration = Model::GetFirstRegistration();
	while (registration)
	{
		if (!(registration->GetModelFlags() & kModelPrivate))
		{
			modelList->InsertSortedListItem(new TypeWidget(size, registration->GetModelName(), registration->GetModelType()));
		}
		
		registration = registration->Next();
	}
}

void ModelsPage::HandleModelButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		GetEditor()->SetCurrentTool(this, widget);
	}
}

void ModelsPage::Engage(Editor *editor, void *cookie)
{
	Widget *widget = static_cast<Widget *>(cookie);
	
	if (widget == modelButton)
	{
		currentTool = 0;
		modelButton->SetValue(1);
	}
	else
	{
		currentTool = -1;
	}
	
	editor->SetCurrentCursor(TheWorldEditor->GetEditorCursor(kEditorCursorCross));
}

void ModelsPage::Disengage(Editor *editor, void *cookie)
{
	if (currentTool != -1)
	{
		modelButton->SetValue(0);
		currentTool = -1;
	}
	else
	{
		if (cookie != modelList) modelList->UnselectAllListItems();
	}
}

bool ModelsPage::BeginTool(Editor *editor, EditorTrackData *trackData)
{
	if ((trackData->viewportType != kEditorViewportGraph) || (trackData->superNode))
	{
		if (currentTool != -1)
		{
			Model *model = new GenericModel("");
			editor->InitNewNode(trackData, model);
			
			if (trackData->viewportType == kEditorViewportOrtho) return (true);
			editor->CommitNewNode(trackData, true);
		}
		else
		{
			const Widget *widget = modelList->GetFirstSelectedListItem();
			if (widget)
			{
				Type modelType = static_cast<const TypeWidget *>(widget)->GetItemType();
				Model *model = new Model(modelType);
				
				const ModelRegistration *registration = Model::FindRegistration(modelType);
				if (registration)
				{
					ControllerType controllerType = registration->GetControllerType();
					if (controllerType != 0) model->SetController(Controller::New(controllerType));
				}
				
				editor->InitNewNode(trackData, model);
				editor->ExpandModel(model);
				
				if (trackData->viewportType == kEditorViewportOrtho) return (true);
				editor->CommitNewNode(trackData, true);
			}
		}
	}
	
	return (false);
}

bool ModelsPage::TrackTool(Editor *editor, EditorTrackData *trackData)
{
	editor->AutoScroll(trackData);
	
	float dx = trackData->currentPosition.x - trackData->snappedAnchorPosition.x;
	float dy = trackData->snappedAnchorPosition.y - trackData->currentPosition.y;
	
	if (InterfaceMgr::GetShiftKey())
	{
		if (Fabs(dx) > Fabs(dy)) dy = 0.0F;
		else dx = 0.0F;
	}
	
	if ((dx != trackData->currentSize.x) || (dy != trackData->currentSize.y))
	{
		trackData->currentSize.Set(dx, dy);
		
		float scale = trackData->viewportScale * 8.0F;
		if (dx * dx + dy * dy > scale * scale)
		{
			Vector3D direction = editor->GetTargetSpaceDirection(trackData, Vector3D(dx, -dy, 0.0F));
			Vector3D up = editor->GetTargetSpaceDirection(trackData, Vector3D(0.0F, 0.0F, -1.0F));
			
			direction.Normalize();
			up = (up - ProjectOnto(up, direction)).Normalize();
			
			Node *node = trackData->trackNode;
			node->SetNodeMatrix3D(direction, up % direction, up);
			editor->InvalidateNode(node);
		}
	}
	
	return (true);
}

bool ModelsPage::EndTool(Editor *editor, EditorTrackData *trackData)
{
	editor->CommitNewNode(trackData, TrackTool(editor, trackData));
	return (true);
}


WorldsPage::WorldsPage() :
		EditorPage(kEditorPageWorlds, "WorldEditor/world/Worlds"),
		editorObserver(this, &WorldsPage::HandleEditorEvent),
		worldButtonObserver(this, &WorldsPage::HandleWorldButtonEvent),
		worldListObserver(this, &WorldsPage::HandleWorldListEvent),
		menuButtonObserver(this, &WorldsPage::HandleMenuButtonEvent)
{
	currentTool = -1;
	selectPercentage = 50;
}

WorldsPage::~WorldsPage()
{
}

WorldsPage::WorldWidget::WorldWidget(const char *text) : TextWidget(text, "font/Normal")
{
}

WorldsPage::WorldWidget::~WorldWidget()
{
}

WorldsPage::SelectSomeWindow::SelectSomeWindow(unsigned_int32 percent) : Window("WorldEditor/world/SelectSome")
{
	selectPercentage = percent;
}

WorldsPage::SelectSomeWindow::~SelectSomeWindow()
{
}

void WorldsPage::SelectSomeWindow::Preprocess(void)
{
	Window::Preprocess();
	
	okayButton = static_cast<PushButtonWidget *>(FindWidget("OK"));
	cancelButton = static_cast<PushButtonWidget *>(FindWidget("Cancel"));
	
	percentBox = static_cast<EditTextWidget *>(FindWidget("Percent"));
	percentBox->SetText(Text::IntegerToString(selectPercentage));
	SetFocusWidget(percentBox);
}

bool WorldsPage::SelectSomeWindow::HandleKeyboardEvent(const KeyboardEventData *eventData)
{
	if (eventData->eventType == kEventKeyDown)
	{
		unsigned_int32 code = eventData->keyCode;
		
		if (code == kKeyCodeReturn)
		{
			okayButton->Activate();
			return (true);
		}
		else if (code == kKeyCodeEscape)
		{
			cancelButton->Activate();
			return (true);
		}
	}
	
	return (Window::HandleKeyboardEvent(eventData));
}

void WorldsPage::SelectSomeWindow::HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetActivate)
	{
		if (widget == okayButton)
		{
			unsigned_int32 percent = Text::StringToInteger(percentBox->GetText());
			if (percent > 0)
			{
				selectPercentage = Min(percent, 100);
				CallCompletionProc();
			}
			
			Close();
		}
		else if (widget == cancelButton)
		{
			Close();
		}
	}
}

void WorldsPage::Pack(Packer& data, unsigned_int32 packFlags) const
{
	EditorPage::Pack(data, packFlags);
	
	WorldWidget *widget = worldWidgetMap.First();
	while (widget)
	{
		PackHandle handle = data.BeginChunk('WRLD');
		data << widget->GetText();
		data.EndChunk(handle);
		
		widget = widget->MapElement<WorldWidget>::Next();
	}
	
	data << ChunkHeader('PCNT', 4);
	data << selectPercentage;
	
	data << TerminatorChunk;
}

void WorldsPage::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	EditorPage::Unpack(data, unpackFlags);
	UnpackChunkList<WorldsPage>(data, unpackFlags);
}

bool WorldsPage::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'WRLD':
		{
			ResourcePath	text;
			
			data >> text;
			AddWorldWidget(text);
			return (true);
		}
		
		case 'PCNT':
			
			data >> selectPercentage;
			return (true);
	}
	
	return (false);
}

void WorldsPage::Preprocess(void)
{
	EditorPage::Preprocess();
	GetEditor()->AddObserver(&editorObserver);
	
	worldButton = static_cast<IconButtonWidget *>(FindWidget("World"));
	worldButton->SetObserver(&worldButtonObserver);
	
	worldList = static_cast<ListWidget *>(FindWidget("List"));
	worldList->SetObserver(&worldListObserver);
	worldList->SetWidgetUsage(worldList->GetWidgetUsage() & ~(kWidgetKeyboardFocus | kWidgetMouseWheel));
	
	menuButton = static_cast<IconButtonWidget *>(FindWidget("Menu"));
	menuButton->SetObserver(&menuButtonObserver);
	
	const StringTable *table = TheWorldEditor->GetStringTable();
	
	worldMenuItemList.Append(new MenuItemWidget(table->GetString(StringID('PAGE', kEditorPageWorlds, 'CLEN')), WidgetObserver<WorldsPage>(this, &WorldsPage::HandleCleanupMenuItemEvent)));
	worldMenuItemList.Append(new MenuItemWidget(kLineSolid));
	
	MenuItemWidget *widget = new MenuItemWidget(table->GetString(StringID('PAGE', kEditorPageWorlds, 'SALL')), WidgetObserver<WorldsPage>(this, &WorldsPage::HandleSelectAllMenuItemEvent));
	worldMenuItem[kWorldMenuSelectAll] = widget;
	worldMenuItemList.Append(widget);
	
	widget = new MenuItemWidget(table->GetString(StringID('PAGE', kEditorPageWorlds, 'SSOM')), WidgetObserver<WorldsPage>(this, &WorldsPage::HandleSelectSomeMenuItemEvent));
	worldMenuItem[kWorldMenuSelectSome] = widget;
	worldMenuItemList.Append(widget);
	
	widget = new MenuItemWidget(table->GetString(StringID('PAGE', kEditorPageWorlds, 'REPL')), WidgetObserver<WorldsPage>(this, &WorldsPage::HandleReplaceSelectedMenuItemEvent));
	worldMenuItem[kWorldMenuReplaceSelected] = widget;
	worldMenuItemList.Append(widget);
	
	BuildWorldList();
}

void WorldsPage::AddWorldWidget(const char *text)
{
	if (text[0] != 0)
	{
		MapReservation	reservation;
		
		if (worldWidgetMap.Reserve(text, &reservation))
		{
			WorldWidget *widget = new WorldWidget(text);
			worldWidgetMap.Insert(widget, &reservation);
		}
	}
}

void WorldsPage::AddZoneWorlds(const Zone *zone)
{
	const Instance *instance = zone->GetFirstInstance();
	while (instance)
	{
		if (!(instance->GetNodeFlags() & kNodeNonpersistent)) AddWorldWidget(instance->GetWorldName());
		instance = instance->Next();
	}
	
	const Zone *subzone = zone->GetFirstSubzone();
	while (subzone)
	{
		AddZoneWorlds(subzone);
		subzone = subzone->Next();
	}
}

void WorldsPage::BuildWorldList(void)
{
	WorldWidget *widget = worldWidgetMap.First();
	while (widget)
	{
		if (widget->GetSuperNode()) worldList->RemoveListItem(widget);
		widget = widget->MapElement<WorldWidget>::Next();
	}
	
	Vector2D size = worldList->GetNaturalListItemSize();
	
	widget = worldWidgetMap.First();
	while (widget)
	{
		widget->SetWidgetSize(size);
		worldList->AppendListItem(widget);
		widget = widget->MapElement<WorldWidget>::Next();
	}
}

void WorldsPage::SelectAllZoneWorlds(Editor *editor, const Zone *zone, const char *worldName)
{
	Instance *instance = zone->GetFirstInstance();
	while (instance)
	{
		if (Text::CompareText(instance->GetWorldName(), worldName)) editor->SelectNode(instance);
		instance = instance->Next();
	}
	
	const Zone *subzone = zone->GetFirstSubzone();
	while (subzone)
	{
		SelectAllZoneWorlds(editor, subzone, worldName);
		subzone = subzone->Next();
	}
}

int32 WorldsPage::GatherZoneWorlds(Editor *editor, const Zone *zone, const char *worldName, List<NodeReference> *worldList)
{
	int32 count = 0;
	
	Instance *instance = zone->GetFirstInstance();
	while (instance)
	{
		if (Text::CompareText(instance->GetWorldName(), worldName))
		{
			worldList->Append(new NodeReference(instance));
			count++;
		}
		
		instance = instance->Next();
	}
	
	const Zone *subzone = zone->GetFirstSubzone();
	while (subzone)
	{
		count += GatherZoneWorlds(editor, subzone, worldName, worldList);
		subzone = subzone->Next();
	}
	
	return (count);
}

void WorldsPage::HandleCleanupMenuItemEvent(Widget *menuItem, const WidgetEventData *eventData)
{
	worldList->PurgeListItems();
	AddZoneWorlds(GetEditor()->GetRootNode());
	BuildWorldList();
}

void WorldsPage::HandleSelectAllMenuItemEvent(Widget *menuItem, const WidgetEventData *eventData)
{
	Editor *editor = GetEditor();
	editor->UnselectAll();
	
	const Widget *widget = worldList->GetFirstSelectedListItem();
	while (widget)
	{
		SelectAllZoneWorlds(editor, editor->GetRootNode(), static_cast<const WorldWidget *>(widget)->GetText());
		widget = worldList->GetNextSelectedListItem(widget);
	}
}

void WorldsPage::HandleSelectSomeMenuItemEvent(Widget *menuItem, const WidgetEventData *eventData)
{
	if (worldList->GetFirstSelectedListItem())
	{
		SelectSomeWindow *window = new SelectSomeWindow(selectPercentage);
		window->SetCompletionProc(&SelectSomeComplete, this);
		GetEditor()->AddSubwindow(window);
	}
}

void WorldsPage::SelectSomeComplete(SelectSomeWindow *window, void *cookie)
{
	WorldsPage *page = static_cast<WorldsPage *>(cookie);
	
	Editor *editor = page->GetEditor();
	editor->UnselectAll();
	
	const Widget *widget = page->worldList->GetFirstSelectedListItem();
	while (widget)
	{
		List<NodeReference>		worldList;
		
		const char *worldName = static_cast<const WorldWidget *>(widget)->GetText();
		int32 totalCount = GatherZoneWorlds(editor, editor->GetRootNode(), worldName, &worldList);
		
		unsigned_int32 percent = window->GetSelectPercentage();
		page->selectPercentage = percent;
		int32 selectCount = (totalCount * percent + 50) / 100;
		
		for (machine a = 0; a < selectCount; a++)
		{
			NodeReference *reference = worldList[Math::Random(totalCount)];
			editor->SelectNode(reference->GetNode());
			
			delete reference;
			totalCount--;
		}
		
		widget = page->worldList->GetNextSelectedListItem(widget);
	}
}

void WorldsPage::HandleReplaceSelectedMenuItemEvent(Widget *menuItem, const WidgetEventData *eventData)
{
	const Widget *widget = worldList->GetFirstSelectedListItem();
	if (widget)
	{
		Editor *editor = GetEditor();
		
		UndoData *undoData = new ReplaceWorldUndoData(editor->GetSelectionList());
		editor->AddUndoData(undoData);
		bool undoable = false;
		
		const char *worldName = static_cast<const WorldWidget *>(widget)->GetText();
		
		const NodeReference *reference = editor->GetFirstSelectedNode();
		while (reference)
		{
			Node *node = reference->GetNode();
			if (node->GetNodeType() == kNodeInstance)
			{
				Instance *instance = static_cast<Instance *>(node);
				instance->Collapse();
				instance->SetWorldName(worldName);
				editor->ExpandWorld(instance);
				
				undoable = true;
			}
			
			reference = reference->Next();
		}
		
		if (!undoable) delete undoData;
	}
}

void WorldsPage::HandleEditorEvent(Editor *editor, const EditorEvent& event)
{
	EditorEventType type = event.GetEventType();
	if ((type == kEditorEventNodesPasted) || (type == kEditorEventNodeInfoModified))
	{
		const NodeReference *reference = editor->GetFirstSelectedNode();
		while (reference)
		{
			const Node *node = reference->GetNode();
			if (node->GetNodeType() == kNodeInstance)
			{
				AddWorldWidget(static_cast<const Instance *>(node)->GetWorldName());
			}
			
			reference = reference->Next();
		}
		
		BuildWorldList();
	}
}

void WorldsPage::HandleWorldButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		GetEditor()->SetCurrentTool(this, widget);
	}
}

void WorldsPage::HandleWorldListEvent(Widget *widget, const WidgetEventData *eventData)
{
	EventType eventType = eventData->eventType;
	
	if (eventType == kEventWidgetActivate)
	{
		HandleSelectAllMenuItemEvent(nullptr, nullptr);
	}
	else if (eventType == kEventWidgetChange)
	{
		if (worldList->GetFirstSelectedListItem()) GetEditor()->SetCurrentTool(this, widget);
		else GetEditor()->SetCurrentTool(this, worldButton);
	}
}

void WorldsPage::HandleMenuButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetActivate)
	{
		int32 count = worldList->GetSelectedListItemCount();
		if (count > 0)
		{
			worldMenuItem[kWorldMenuSelectAll]->Enable();
			worldMenuItem[kWorldMenuSelectSome]->Enable();
			
			if (count == 1) worldMenuItem[kWorldMenuReplaceSelected]->Enable();
			else worldMenuItem[kWorldMenuReplaceSelected]->Disable();
		}
		else
		{
			for (machine a = 0; a < kWorldMenuItemCount; a++) worldMenuItem[a]->Disable();
		}
		
		Menu *menu = new Menu(kMenuContextual, &worldMenuItemList);
		menu->SetWidgetPosition(menuButton->GetWorldPosition() + Vector3D(25.0F, 0.0F, 0.0F));
		TheInterfaceMgr->SetActiveMenu(menu);
	}
}

void WorldsPage::Engage(Editor *editor, void *cookie)
{
	Widget *widget = static_cast<Widget *>(cookie);
	
	if (widget == worldButton)
	{
		currentTool = 0;
		worldButton->SetValue(1);
	}
	else
	{
		currentTool = -1;
	}
	
	editor->SetCurrentCursor(TheWorldEditor->GetEditorCursor(kEditorCursorCross));
}

void WorldsPage::Disengage(Editor *editor, void *cookie)
{
	if (currentTool != -1)
	{
		worldButton->SetValue(0);
		currentTool = -1;
	}
	else
	{
		if (cookie != worldList) worldList->UnselectAllListItems();
	}
}

bool WorldsPage::BeginTool(Editor *editor, EditorTrackData *trackData)
{
	if ((trackData->viewportType != kEditorViewportGraph) || (trackData->superNode))
	{
		Instance *instance = nullptr;
		
		if (currentTool != -1)
		{
			instance = new Instance("");
		}
		else
		{
			int32 count = worldList->GetSelectedListItemCount();
			if (count > 0)
			{
				int32 index = (count == 1) ? 0 : Math::Random(count);
				const Widget *widget = worldList->GetSelectedListItem(index);
				instance = new Instance(static_cast<const WorldWidget *>(widget)->GetText());
			}
		}
		
		if (instance)
		{
			editor->InitNewNode(trackData, instance);
			editor->ExpandWorld(instance);
			
			if (trackData->viewportType == kEditorViewportOrtho) return (true);
			editor->CommitNewNode(trackData, true);
		}
	}
	
	return (false);
}

bool WorldsPage::TrackTool(Editor *editor, EditorTrackData *trackData)
{
	editor->AutoScroll(trackData);
	
	float dx = trackData->currentPosition.x - trackData->snappedAnchorPosition.x;
	float dy = trackData->snappedAnchorPosition.y - trackData->currentPosition.y;
	
	if (InterfaceMgr::GetShiftKey())
	{
		if (Fabs(dx) > Fabs(dy)) dy = 0.0F;
		else dx = 0.0F;
	}
	
	if ((dx != trackData->currentSize.x) || (dy != trackData->currentSize.y))
	{
		trackData->currentSize.Set(dx, dy);
		
		float scale = trackData->viewportScale * 8.0F;
		if (dx * dx + dy * dy > scale * scale)
		{
			Vector3D direction = editor->GetTargetSpaceDirection(trackData, Vector3D(dx, -dy, 0.0F));
			Vector3D up = editor->GetTargetSpaceDirection(trackData, Vector3D(0.0F, 0.0F, -1.0F));
			
			direction.Normalize();
			up = (up - ProjectOnto(up, direction)).Normalize();
			
			Node *node = trackData->trackNode;
			node->SetNodeMatrix3D(direction, up % direction, up);
			editor->InvalidateNode(node);
		}
	}
	
	return (true);
}

bool WorldsPage::EndTool(Editor *editor, EditorTrackData *trackData)
{
	editor->CommitNewNode(trackData, TrackTool(editor, trackData));
	return (true);
}


SkyboxesPage::SkyboxesPage() :
		EditorPage(kEditorPageSkyboxes, "WorldEditor/skybox/Skyboxes"),
		skyboxButtonObserver(this, &SkyboxesPage::HandleSkyboxButtonEvent)
{
	currentTool = -1;
}

SkyboxesPage::~SkyboxesPage()
{
}

void SkyboxesPage::Preprocess(void)
{
	EditorPage::Preprocess();
	
	skyboxButton = static_cast<IconButtonWidget *>(FindWidget("Skybox"));
	skyboxButton->SetObserver(&skyboxButtonObserver);
}

void SkyboxesPage::HandleSkyboxButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		GetEditor()->SetCurrentTool(this, widget);
	}
}

void SkyboxesPage::Engage(Editor *editor, void *cookie)
{
	Widget *widget = static_cast<Widget *>(cookie);
	
	if (widget == skyboxButton)
	{
		currentTool = 0;
		skyboxButton->SetValue(1);
	}
	
	editor->SetCurrentCursor(TheWorldEditor->GetEditorCursor(kEditorCursorCross));
}

void SkyboxesPage::Disengage(Editor *editor, void *cookie)
{
	if (currentTool != -1)
	{
		skyboxButton->SetValue(0);
		currentTool = -1;
	}
}

bool SkyboxesPage::BeginTool(Editor *editor, EditorTrackData *trackData)
{
	if ((trackData->viewportType != kEditorViewportGraph) || (trackData->superNode))
	{
		if (currentTool == 0)
		{
			editor->InitNewNode(trackData, new Skybox);
			
			if (trackData->viewportType == kEditorViewportOrtho) return (true);
			editor->CommitNewNode(trackData, true);
		}
	}
	
	return (false);
}

bool SkyboxesPage::TrackTool(Editor *editor, EditorTrackData *trackData)
{
	editor->AutoScroll(trackData);
	return (true);
}

bool SkyboxesPage::EndTool(Editor *editor, EditorTrackData *trackData)
{
	editor->CommitNewNode(trackData, TrackTool(editor, trackData));
	return (true);
}


ImpostorsPage::ImpostorsPage() :
		EditorPage(kEditorPageImpostors, "WorldEditor/impostor/Impostors"),
		impostorButtonObserver(this, &ImpostorsPage::HandleImpostorButtonEvent)
{
	currentTool = -1;
}

ImpostorsPage::~ImpostorsPage()
{
}

void ImpostorsPage::Preprocess(void)
{
	EditorPage::Preprocess();
	
	impostorButton = static_cast<IconButtonWidget *>(FindWidget("Impostor"));
	impostorButton->SetObserver(&impostorButtonObserver);
}

void ImpostorsPage::HandleImpostorButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		GetEditor()->SetCurrentTool(this, widget);
	}
}

void ImpostorsPage::Engage(Editor *editor, void *cookie)
{
	Widget *widget = static_cast<Widget *>(cookie);
	
	if (widget == impostorButton)
	{
		currentTool = 0;
		impostorButton->SetValue(1);
	}
	
	editor->SetCurrentCursor(TheWorldEditor->GetEditorCursor(kEditorCursorCross));
}

void ImpostorsPage::Disengage(Editor *editor, void *cookie)
{
	if (currentTool != -1)
	{
		impostorButton->SetValue(0);
		currentTool = -1;
	}
}

bool ImpostorsPage::BeginTool(Editor *editor, EditorTrackData *trackData)
{
	if ((trackData->viewportType != kEditorViewportGraph) || (trackData->superNode))
	{
		if (currentTool == 0)
		{
			editor->InitNewNode(trackData, new Impostor);
			
			if (trackData->viewportType == kEditorViewportOrtho) return (true);
			editor->CommitNewNode(trackData, true);
		}
	}
	
	return (false);
}

bool ImpostorsPage::TrackTool(Editor *editor, EditorTrackData *trackData)
{
	editor->AutoScroll(trackData);
	return (true);
}

bool ImpostorsPage::EndTool(Editor *editor, EditorTrackData *trackData)
{
	editor->CommitNewNode(trackData, TrackTool(editor, trackData));
	return (true);
}


PathsPage::PathsPage() :
		EditorPage(kEditorPagePaths, "WorldEditor/path/Paths"),
		pathButtonObserver(this, &PathsPage::HandlePathButtonEvent)
{
	currentTool = -1;
}

PathsPage::~PathsPage()
{
}

void PathsPage::Preprocess(void)
{
	static const char *const buttonIdentifier[kEditorPathCount] =
	{
		"Linear", "Elliptical", "Bezier"
	};
	
	EditorPage::Preprocess();
	
	for (machine a = 0; a < kEditorPathCount; a++)
	{
		pathButton[a] = static_cast<IconButtonWidget *>(FindWidget(buttonIdentifier[a]));
		pathButton[a]->SetObserver(&pathButtonObserver);
	}
}

void PathsPage::HandlePathButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		GetEditor()->SetCurrentTool(this, widget);
	}
}

void PathsPage::Engage(Editor *editor, void *cookie)
{
	Widget *widget = static_cast<Widget *>(cookie);
	
	for (machine a = 0; a < kEditorPathCount; a++)
	{
		if (widget == pathButton[a])
		{
			currentTool = a;
			pathButton[a]->SetValue(1);
			break;
		}
	}
	
	editor->SetCurrentCursor(TheWorldEditor->GetEditorCursor(kEditorCursorCross));
}

void PathsPage::Disengage(Editor *editor, void *cookie)
{
	if (currentTool != -1)
	{
		pathButton[currentTool]->SetValue(0);
		currentTool = -1;
	}
}

bool PathsPage::SnapToBeginning(const PathMarker *marker, const EditorTrackData *trackData)
{
	if (!(trackData->currentModifierKeys & kModifierKeyCommand))
	{
		Ray			ray;
		PickData	pickData;
		
		const Transform4D& transform = marker->GetInverseWorldTransform();
		
		ray.origin = transform * trackData->worldRay.origin;
		ray.direction = transform * trackData->worldRay.direction;
		ray.radius = trackData->worldRay.radius;
		ray.tmin = trackData->worldRay.tmin;
		ray.tmax = trackData->worldRay.tmax;
		
		if (static_cast<PathManipulator *>(Editor::GetManipulator(marker))->PickControlPoint(&ray, &pickData))
		{
			return (pickData.pickIndex[0] == 0);
		}
	}
	
	return (false);
}

bool PathsPage::BeginTool(Editor *editor, EditorTrackData *trackData)
{
	if (trackData->viewportType == kEditorViewportOrtho)
	{
		PathMarker *pathMarker = nullptr;
		
		const NodeReference *reference = editor->GetFirstSelectedNode();
		if ((reference) && (!reference->Next()))
		{
			Node *node = reference->GetNode();
			if (node->GetNodeType() == kNodeMarker)
			{
				Marker *marker = static_cast<Marker *>(node);
				if (marker->GetMarkerType() == kMarkerPath) pathMarker = static_cast<PathMarker *>(marker);
			}
		}
		
		if (pathMarker)
		{
			targetPath = pathMarker;
			Path *path = pathMarker->GetPath();
			
			PathComponent *prevComponent = path->GetLastPathComponent();
			const Point3D& point = prevComponent->GetEndPosition();
			Vector3D tangent = prevComponent->GetEndTangent();
			
			Point3D position = pathMarker->GetInverseWorldTransform() * (editor->GetTargetZone()->GetWorldTransform() * editor->GetTargetSpacePosition(trackData, trackData->snappedAnchorPosition));
			Point3D snappedPosition = (SnapToBeginning(pathMarker, trackData)) ? path->GetFirstPathComponent()->GetBeginPosition() : position;
			
			PathComponent *component = nullptr;
			
			switch (currentTool)
			{
				case kEditorPathLinear:
					
					component = new LinearPathComponent(point, snappedPosition);
					break;
				
				case kEditorPathElliptical:
					
					component = new EllipticalPathComponent(point, snappedPosition, point + tangent * InverseMag(tangent));
					break;
				
				case kEditorPathBezier:
				{
					if (prevComponent->GetPathType() == kPathBezier)
					{
						BezierPathComponent *bezierComponent = static_cast<BezierPathComponent *>(prevComponent);
						const Point3D& p = bezierComponent->GetControlPoint(0);
						if (bezierComponent->GetControlPoint(3) == p)
						{
							bezierComponent->SetControlPoint(2, position);
							bezierComponent->SetControlPoint(3, position);
							
							trackData->trackNode = pathMarker;
							pathMarker->Invalidate();
							return (true);
						}
					}
					
					component = new BezierPathComponent(point, point + tangent, snappedPosition, snappedPosition);
					break;
				}
			}
			
			if (component)
			{
				editor->AddUndoData(new PathUndoData(pathMarker));
				
				path->AppendPathComponent(component);
				editor->InvalidateNode(pathMarker);
				
				trackData->trackNode = pathMarker;
				return (true);
			}
		}
		else
		{
			targetPath = nullptr;
			
			PathMarker *marker = new PathMarker(K::z_unit);
			PathComponent *component = nullptr;
			
			switch (currentTool)
			{
				case kEditorPathLinear:
					
					component = new LinearPathComponent(Zero3D, Zero3D);
					break;
				
				case kEditorPathElliptical:
					
					component = new EllipticalPathComponent(Zero3D, Zero3D, Point3D(1.0F, 0.0F, 0.0F));
					break;
				
				case kEditorPathBezier:
					
					component = new BezierPathComponent(Zero3D, Zero3D, Zero3D, Zero3D);
					break;
			}
			
			if (component)
			{
				marker->GetPath()->AppendPathComponent(component);
				editor->InitNewNode(trackData, marker);
				return (true);
			}
			
			delete marker;
		}
	}
	
	return (false);
}

bool PathsPage::TrackTool(Editor *editor, EditorTrackData *trackData)
{
	editor->AutoScroll(trackData);
	
	float dx = trackData->snappedCurrentPosition.x - trackData->snappedAnchorPosition.x;
	float dy = trackData->snappedCurrentPosition.y - trackData->snappedAnchorPosition.y;
	float ax = Fabs(dx);
	float ay = Fabs(dy);
	
	if (InterfaceMgr::GetShiftKey())
	{
		if (ax < ay * 0.5F) ax = 0.0F;
		else if (ay < ax * 0.5F) ay = 0.0F;
		else ax = ay = Fmax(ax, ay);
		
		dx = (dx < 0.0F) ? -ax : ax;
		dy = (dy < 0.0F) ? -ay : ay;
	}
	
	if ((dx != trackData->currentSize.x) || (dy != trackData->currentSize.y) || (trackData->currentModifierKeys != trackData->previousModifierKeys))
	{
		trackData->currentSize.Set(dx, dy);
		
		PathMarker *pathMarker = static_cast<PathMarker *>(trackData->trackNode);
		Path *path = pathMarker->GetPath();
		
		Point3D position = pathMarker->GetInverseWorldTransform() * (editor->GetTargetZone()->GetWorldTransform() * editor->GetTargetSpacePosition(trackData, trackData->snappedAnchorPosition + Vector2D(dx, dy)));
		Point3D snappedPosition = (SnapToBeginning(pathMarker, trackData)) ? path->GetFirstPathComponent()->GetBeginPosition() : position;
		
		PathComponent *component = path->GetLastPathComponent();
		switch (component->GetPathType())
		{
			case kPathLinear:
			{
				LinearPathComponent *linearComponent = static_cast<LinearPathComponent *>(component);
				linearComponent->SetControlPoint(1, snappedPosition);
				break;
			}
			
			case kPathElliptical:
			{
				EllipticalPathComponent *ellipticalComponent = static_cast<EllipticalPathComponent *>(component);
				ellipticalComponent->SetControlPoint(1, snappedPosition);
				break;
			}
			
			case kPathBezier:
			{
				BezierPathComponent *bezierComponent = static_cast<BezierPathComponent *>(component);
				if (targetPath) bezierComponent->SetControlPoint(2, position);
				else bezierComponent->SetControlPoint(1, position);
				break;
			}
		}
		
		editor->InvalidateNode(pathMarker);
	}
	
	return ((dx != 0.0F) || (dy != 0.0F));
}

bool PathsPage::EndTool(Editor *editor, EditorTrackData *trackData)
{
	if (!targetPath) editor->CommitNewNode(trackData, TrackTool(editor, trackData));
	return (true);
}


PlacementPage::PlacementPage() :
		EditorPage(kEditorPagePlacement, "WorldEditor/page/Placement"),
		placementWidgetObserver(this, &PlacementPage::HandlePlacementWidgetEvent),
		applyButtonObserver(this, &PlacementPage::HandleApplyButtonEvent)
{
	enableValue = 0;
	rotateValue = 0;
	tangentValue = 0;
	sinkValue = 0;
	radiusValue = 0.0F;
}

PlacementPage::~PlacementPage()
{
}

void PlacementPage::Pack(Packer& data, unsigned_int32 packFlags) const
{
	EditorPage::Pack(data, packFlags);
	
	data << ChunkHeader('ENAB', 4);
	data << enableValue;
	
	data << ChunkHeader('ROTA', 4);
	data << rotateValue;
	
	data << ChunkHeader('TANG', 4);
	data << tangentValue;
	
	data << ChunkHeader('SINK', 4);
	data << sinkValue;
	
	data << ChunkHeader('SRAD', 4);
	data << radiusValue;
	
	data << TerminatorChunk;
}

void PlacementPage::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	EditorPage::Unpack(data, unpackFlags);
	UnpackChunkList<PlacementPage>(data, unpackFlags);
}

bool PlacementPage::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'ENAB':
			
			data >> enableValue;
			return (true);
		
		case 'ROTA':
			
			data >> rotateValue;
			return (true);
		
		case 'TANG':
			
			data >> tangentValue;
			return (true);
		
		case 'SINK':
			
			data >> sinkValue;
			return (true);
		
		case 'SRAD':
			
			data >> radiusValue;
			return (true);
	}
	
	return (false);
}

void PlacementPage::Preprocess(void)
{
	EditorPage::Preprocess();
	
	enableWidget = static_cast<CheckWidget *>(FindWidget("Enable"));
	rotateWidget = static_cast<CheckWidget *>(FindWidget("Rotate"));
	tangentWidget = static_cast<CheckWidget *>(FindWidget("Tangent"));
	sinkWidget = static_cast<CheckWidget *>(FindWidget("Sink"));
	radiusWidget = static_cast<EditTextWidget *>(FindWidget("Radius"));
	applyButton = static_cast<PushButtonWidget *>(FindWidget("Apply"));
	
	enableWidget->SetObserver(&placementWidgetObserver);
	rotateWidget->SetObserver(&placementWidgetObserver);
	tangentWidget->SetObserver(&placementWidgetObserver);
	sinkWidget->SetObserver(&placementWidgetObserver);
	radiusWidget->SetObserver(&placementWidgetObserver);
	applyButton->SetObserver(&applyButtonObserver);
	
	if (enableValue != 0) enableWidget->SetValue(1);
	if (rotateValue != 0) rotateWidget->SetValue(1);
	if (tangentValue != 0) tangentWidget->SetValue(1);
	if (sinkValue != 0) sinkWidget->SetValue(1);
	
	radiusWidget->SetText(Text::FloatToString(radiusValue));
	
	GetEditor()->InstallPlacementModifier(&ModifyPlacement, this);
}

void PlacementPage::HandlePlacementWidgetEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		if (widget == enableWidget) enableValue = enableWidget->GetValue();
		else if (widget == rotateWidget) rotateValue = rotateWidget->GetValue();
		else if (widget == tangentWidget) tangentValue = tangentWidget->GetValue();
		else if (widget == sinkWidget) sinkValue = sinkWidget->GetValue();
		else if (widget == radiusWidget) radiusValue = Text::StringToFloat(radiusWidget->GetText());
	}
}

void PlacementPage::HandleApplyButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetActivate)
	{
		bool undoDataFlag = false;
		
		Editor *editor = GetEditor();
		NodeReference *reference = editor->GetFirstSelectedNode();
		while (reference)
		{
			Node *node = reference->GetNode();
			if (Editor::GetManipulator(node)->GetManipulatorFlags() & kManipulatorModifiablePlacement)
			{
				if (!undoDataFlag)
				{
					undoDataFlag = true;
					editor->AddUndoData(new MoveUndoData(editor->GetSelectionList()));
				}
				
				Transform4D transform = node->GetWorldTransform();
				ApplyRotation(editor, &transform);
				ApplySink(editor, &transform);
				
				unsigned_int32 flags = node->GetNodeFlags();
				node->SetNodeFlags(flags | kNodeDisabled);
				ApplyTangent(editor, &transform);
				node->SetNodeFlags(flags);
				
				node->SetNodeTransform(node->GetSuperNode()->GetInverseWorldTransform() * transform);
				editor->InvalidateNode(node);
			}
			
			reference = reference->Next();
		}
	}
}

void PlacementPage::ApplyRotation(const Editor *editor, Transform4D *transform) const
{
	if (rotateValue != 0)
	{
		transform->SetMatrix3D(Matrix3D().SetRotationAboutZ(Math::RandomFloat(K::two_pi)));
	}
}

void PlacementPage::ApplyTangent(const Editor *editor, Transform4D *transform, const Vector3D *normal) const
{
	if (tangentValue != 0)
	{
		CollisionData	data;
		
		if (!normal)
		{
			const Point3D& position = transform->GetTranslation();
			Point3D p1(position.x, position.y, position.z + 0.25F);
			Point3D p2(position.x, position.y, position.z - 1.0F);
			
			if (!editor->GetEditorWorld()->DetectCollision(p1, p2, 0.0F, kCollisionSightPath, &data)) return;
			normal = &data.normal;
		}
		
		Matrix3D m = transform->GetMatrix3D();
		float t = Acos(*normal * m[2]);
		if (t != 0.0F)
		{
			m = Matrix3D().SetRotationAboutAxis(t, (m[2] % *normal).Normalize()) * m;
			transform->SetMatrix3D(m);
		}
	}
}

void PlacementPage::ApplySink(const Editor *editor, Transform4D *transform) const
{
	if (sinkValue != 0)
	{
		CollisionData	data;
		
		const ConstVector2D *trig = Math::GetTrigTable();
		
		float maxParam = 0.0F;
		float maxDepth = radiusValue * 2.0F;
		
		Point3D position = transform->GetTranslation();
		const World *world = editor->GetEditorWorld();
		
		for (machine a = 0; a < 8; a++)
		{
			Vector2D cs = trig[a * 32] * radiusValue;
			Point3D p1(position.x + cs.x, position.y + cs.y, position.z);
			Point3D p2(p1.x, p1.y, p1.z - maxDepth);
			
			if (world->DetectCollision(p1, p2, 0.0F, kCollisionSightPath, &data)) maxParam = Fmax(maxParam, data.param);
		}
		
		position.z -= maxDepth * maxParam;
		transform->SetTranslation(position);
	}
}

void PlacementPage::ModifyPlacement(const Editor *editor, Transform4D *transform, const Vector3D& normal, void *cookie)
{
	const PlacementPage *page = static_cast<PlacementPage *>(cookie);
	if (page->enableValue != 0)
	{
		page->ApplyRotation(editor, transform);
		page->ApplySink(editor, transform);
		page->ApplyTangent(editor, transform, &normal);
	}
}


MaterialPage::MaterialPage() :
		EditorPage(kEditorPageMaterial, "WorldEditor/page/Material"),
		editorObserver(this, &MaterialPage::HandleEditorEvent),
		toolButtonObserver(this, &MaterialPage::HandleToolButtonEvent),
		materialWidgetObserver(this, &MaterialPage::HandleMaterialWidgetEvent)
{
	currentTool = -1;
	InitialShow();
}

MaterialPage::~MaterialPage()
{
}

void MaterialPage::Preprocess(void)
{
	static const char *const buttonIdentifier[kEditorMaterialToolCount] =
	{
		"Pickup"
	};
	
	Editor *editor = GetEditor();
	editor->AddObserver(&editorObserver);
	
	materialWidget = new MaterialWidget(Vector2D(64.0F, 64.0F), editor->GetSelectedMaterial());
	materialWidget->SetWidgetPosition(Point3D(32.0F, 7.0F, 0.0F));
	materialWidget->SetObserver(&materialWidgetObserver);
	AddSubnode(materialWidget);
	
	EditorPage::Preprocess();
	
	for (machine a = 0; a < kEditorMaterialToolCount; a++)
	{
		toolButton[a] = static_cast<IconButtonWidget *>(FindWidget(buttonIdentifier[a]));
		toolButton[a]->SetObserver(&toolButtonObserver);
	}
}

void MaterialPage::HandleEditorEvent(Editor *editor, const EditorEvent& event)
{
	EditorEventType type = event.GetEventType();
	if (type == kEditorEventMaterialSelected)
	{
		materialWidget->SetMaterial(editor->GetSelectedMaterial());
	}
	else if (type == kEditorEventMaterialModified)
	{
		const MaterialObject *materialObject = static_cast<const MaterialEditorEvent *>(&event)->GetEventMaterialObject();
		if (materialWidget->GetMaterialContainer()->GetMaterialObject() == materialObject) materialWidget->UpdatePreview();
	}
}

void MaterialPage::HandleToolButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		GetEditor()->SetCurrentTool(this, widget);
	}
}

void MaterialPage::Engage(Editor *editor, void *cookie)
{
	Widget *widget = static_cast<Widget *>(cookie);
	
	if (widget == toolButton[kEditorMaterialToolPickup])
	{
		currentTool = 0;
		toolButton[kEditorMaterialToolPickup]->SetValue(1);
		
		editor->SetCurrentCursor(TheWorldEditor->GetEditorCursor(kEditorCursorDropper));
	}
}

void MaterialPage::Disengage(Editor *editor, void *cookie)
{
	if (currentTool != -1)
	{
		toolButton[currentTool]->SetValue(0);
		currentTool = -1;
	}
}

bool MaterialPage::BeginTool(Editor *editor, EditorTrackData *trackData)
{
	const MaterialObject *materialObject = nullptr;
	
	if (trackData->viewportType != kEditorViewportGraph)
	{
		PickData	pickData;
		
		const Node *node = editor->PickNode(trackData, &pickData, 1 << kEditorNodeGeometry);
		if ((node) && (pickData.triangleIndex != kInvalidTriangleIndex))
		{
			const Geometry *geometry = static_cast<const Geometry *>(node);
			materialObject = geometry->GetTriangleMaterial(pickData.triangleIndex);
		}
	}
	else
	{
		const Node *node = Editor::GetManipulator(editor->GetRootNode())->PickGraphNode(trackData, &trackData->worldRay);
		if (node) materialObject = Editor::GetManipulator(node)->PickupMaterial();
	}
	
	if (materialObject)
	{
		MaterialContainer *container = editor->GetEditorObject()->FindMaterialContainer(materialObject);
		if (container) editor->SelectMaterial(container);
	}
	
	return (false);
}

void MaterialPage::HandleMaterialWidgetEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetActivate)
	{
		GetEditor()->OpenMaterialManager();
	}
}


SelectionMaskPage::SelectionMaskPage() :
		EditorPage(kEditorPageSelectionMask, "WorldEditor/page/SelectionMask"),
		editorObserver(this, &SelectionMaskPage::HandleEditorEvent),
		maskButtonObserver(this, &SelectionMaskPage::HandleMaskButtonEvent)
{
	InitialShow();
}

SelectionMaskPage::~SelectionMaskPage()
{
}

void SelectionMaskPage::Preprocess(void)
{
	static const char *const buttonIdentifier[kEditorNodeCount] =
	{
		"Geometry", "Camera", "Light", "Source", "Zone", "Portal",
		"Space", "Marker", "Trigger", "Effect", "Model", "Group"
	};
	
	EditorPage::Preprocess();
	
	Editor *editor = GetEditor();
	editor->AddObserver(&editorObserver);
	
	unsigned_int32 mask = editor->GetEditorObject()->GetSelectionMask();
	for (machine a = 0; a < kEditorNodeCount; a++)
	{
		maskButton[a] = static_cast<IconButtonWidget *>(FindWidget(buttonIdentifier[a]));
		maskButton[a]->SetObserver(&maskButtonObserver);
		maskButton[a]->SetValue(mask & 1);
		mask >>= 1;
	}
}

void SelectionMaskPage::HandleEditorEvent(Editor *editor, const EditorEvent& event)
{
	if (event.GetEventType() == kEditorEventNodeCreated)
	{
		int32 type = -1;
		switch (static_cast<const NodeEditorEvent *>(&event)->GetEventNode()->GetNodeType())
		{
			case kNodeGeometry:
				
				type = kEditorNodeGeometry;
				break;
			
			case kNodeCamera:
				
				type = kEditorNodeCamera;
				break;
			
			case kNodeLight:
				
				type = kEditorNodeLight;
				break;
			
			case kNodeSource:
				
				type = kEditorNodeSource;
				break;
			
			case kNodeSpace:
				
				type = kEditorNodeSpace;
				break;
			
			case kNodeZone:
				
				type = kEditorNodeZone;
				break;
			
			case kNodePortal:
				
				type = kEditorNodePortal;
				break;
			
			case kNodeMarker:
			case kNodeSkybox:
				
				type = kEditorNodeMarker;
				break;
			
			case kNodeTrigger:
				
				type = kEditorNodeTrigger;
				break;
			
			case kNodeEffect:
			case kNodeEmitter:
			case kNodeShape:
			case kNodeJoint:
			case kNodeField:
			case kNodePhysics:
				
				type = kEditorNodeEffect;
				break;
			
			case kNodeModel:
				
				type = kEditorNodeModel;
				break;
			
			case kNodeGroup:
			case kNodeTerrainBlock:
				
				type = kEditorNodeGroup;
				break;
		}
		
		if (type >= 0)
		{
			maskButton[type]->SetValue(1);
			
			EditorObject *object = editor->GetEditorObject();
			object->SetSelectionMask(object->GetSelectionMask() | (1 << type));
		}
	}
}

void SelectionMaskPage::HandleMaskButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	for (machine a = 0; a < kEditorNodeCount; a++)
	{
		if (widget == maskButton[a])
		{
			EditorObject *object = GetEditor()->GetEditorObject();
			
			unsigned_int32 mask = object->GetSelectionMask();
			
			if (InterfaceMgr::GetCommandKey())
			{
				mask |= 1 << a;
				maskButton[a]->SetValue(1);
				
				for (machine b = 0; b < kEditorNodeCount; b++)
				{
					if (b != a)
					{
						mask &= ~(1 << b);
						maskButton[b]->SetValue(0);
					}
				}
			}
			else
			{
				if (mask & (1 << a))
				{
					mask &= ~(1 << a);
					maskButton[a]->SetValue(0);
				}
				else
				{
					mask |= 1 << a;
					maskButton[a]->SetValue(1);
				}
			}
			
			object->SetSelectionMask(mask);
			break;
		}
	}
}


VisibilityPage::VisibilityPage() :
		EditorPage(kEditorPageVisibility, "WorldEditor/page/Visibility"),
		visibilityButtonObserver(this, &VisibilityPage::HandleVisibilityButtonEvent)
{
}

VisibilityPage::~VisibilityPage()
{
}

void VisibilityPage::Preprocess(void)
{
	static const char *const showIdentifier[kEditorNodeCount] =
	{
		"Geometry1", "Camera1", "Light1", "Source1", "Zone1", "Portal1",
		"Space1", "Marker1", "Trigger1", "Effect1", "Model1", "Group1"
	};
	
	static const char *const hideIdentifier[kEditorNodeCount] =
	{
		"Geometry0", "Camera0", "Light0", "Source0", "Zone0", "Portal0",
		"Space0", "Marker0", "Trigger0", "Effect0", "Model0", "Group0"
	};
	
	EditorPage::Preprocess();
	
	for (machine a = 0; a < kEditorNodeCount; a++)
	{
		showButton[a] = static_cast<IconButtonWidget *>(FindWidget(showIdentifier[a]));
		hideButton[a] = static_cast<IconButtonWidget *>(FindWidget(hideIdentifier[a]));
		
		showButton[a]->SetObserver(&visibilityButtonObserver);
		hideButton[a]->SetObserver(&visibilityButtonObserver);
	}
}

void VisibilityPage::HandleVisibilityButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	static const NodeType nodeType1[kEditorNodeCount] =
	{
		kNodeGeometry, kNodeCamera, kNodeLight, kNodeSource, kNodeZone, kNodePortal,
		kNodeSpace, kNodeMarker, kNodeTrigger, kNodeEffect, kNodeModel, kNodeGroup
	};
	
	static const NodeType nodeType2[kEditorNodeCount] =
	{
		kNodeGeometry, kNodeCamera, kNodeLight, kNodeSource, kNodeZone, kNodePortal,
		kNodeSpace, kNodeSkybox, kNodeTrigger, kNodeEmitter, kNodeModel, kNodeGroup
	};
	
	if (eventData->eventType == kEventWidgetActivate)
	{
		for (machine a = 0; a < kEditorNodeCount; a++)
		{
			if (widget == showButton[a])
			{
				NodeType type1 = nodeType1[a];
				NodeType type2 = nodeType2[a];
				
				Editor *editor = GetEditor();
				Node *root = editor->GetRootNode();
				Node *node = root->GetNextNode(root);
				while (node)
				{
					NodeType type = node->GetNodeType();
					if ((type == type1) || (type == type2)) editor->ShowNode(node);
					node = root->GetNextNode(node);
				}
				
				return;
			}
		}
		
		for (machine a = 0; a < kEditorNodeCount; a++)
		{
			if (widget == hideButton[a])
			{
				NodeType type1 = nodeType1[a];
				NodeType type2 = nodeType2[a];
				
				Editor *editor = GetEditor();
				Node *root = editor->GetRootNode();
				Node *node = root->GetNextNode(root);
				while (node)
				{
					NodeType type = node->GetNodeType();
					if ((type == type1) || (type == type2)) editor->HideNode(node);
					node = root->GetNextNode(node);
				}
				
				return;
			}
		}
	}
}


ViewportsPage::ViewportsPage() :
		EditorPage(kEditorPageViewports, "WorldEditor/page/Viewports"),
		layoutButtonObserver(this, &ViewportsPage::HandleLayoutButtonEvent),
		cameraSliderObserver(this, &ViewportsPage::HandleCameraSliderEvent)
{
	InitialShow();
}

ViewportsPage::~ViewportsPage()
{
}

void ViewportsPage::Preprocess(void)
{
	static const char *const buttonIdentifier[kEditorLayoutCount] =
	{
		"1", "4", "2H", "2V", "3L", "3R", "3T", "3B"
	};
	
	EditorPage::Preprocess();
	
	for (machine a = 0; a < kEditorLayoutCount; a++)
	{
		layoutButton[a] = static_cast<IconButtonWidget *>(FindWidget(buttonIdentifier[a]));
		layoutButton[a]->SetObserver(&layoutButtonObserver);
	}
	
	cameraSlider = static_cast<SliderWidget *>(FindWidget("Speed"));
	cameraSlider->SetObserver(&cameraSliderObserver);
	
	const EditorObject *object = GetEditor()->GetEditorObject();
	SetViewportLayout(object->GetCurrentViewportLayout());
	SetCameraSpeed(object->GetCameraSpeed());
}

void ViewportsPage::SetViewportLayout(int32 layout)
{
	for (machine a = 0; a < kEditorLayoutCount; a++) layoutButton[a]->SetValue(a == layout);
}

void ViewportsPage::SetCameraSpeed(float speed)
{
	cameraSlider->SetValue((int32) (speed * 64.0F));
}

void ViewportsPage::HandleLayoutButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		for (machine a = 0; a < kEditorLayoutCount; a++)
		{
			if (widget == layoutButton[a])
			{
				SetViewportLayout(a);
					
				Editor *editor = GetEditor();
				EditorObject *object = editor->GetEditorObject();
					
				int32 layout = object->GetCurrentViewportLayout();
				if (a != layout)
				{
					object->SetPreviousViewportLayout(layout);
					object->SetCurrentViewportLayout(a);
					editor->UpdateViewportStructures();
				}
					
				break;
			}
		}
	}
}

void ViewportsPage::HandleCameraSliderEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		GetEditor()->GetEditorObject()->SetCameraSpeed(Fmax((float) cameraSlider->GetValue() * 0.015625F, 0.00390625F));
	}
}


TransformPage::TransformPage() :
		EditorPage(kEditorPageTransform, "WorldEditor/page/Transform"),
		editorObserver(this, &TransformPage::HandleEditorEvent),
		positionTextObserver(this, &TransformPage::HandlePositionTextEvent),
		rotationTextObserver(this, &TransformPage::HandleRotationTextEvent),
		sizeTextObserver(this, &TransformPage::HandleSizeTextEvent)
{
}

TransformPage::~TransformPage()
{
}

void TransformPage::Preprocess(void)
{
	static const char *const positionIdentifier[3] =
	{
		"Xpos", "Ypos", "Zpos"
	};
	
	static const char *const rotationIdentifier[3] =
	{
		"Xrot", "Yrot", "Zrot"
	};
	
	static const char *const sizeIdentifier[kMaxObjectSizeCount] =
	{
		"Xsize", "Ysize", "Zsize", "Asize", "Bsize", "Csize"
	};
	
	
	EditorPage::Preprocess();
	GetEditor()->AddObserver(&editorObserver);
	
	for (machine a = 0; a < 3; a++)
	{
		positionTextWidget[a] = static_cast<EditTextWidget *>(FindWidget(positionIdentifier[a]));
		positionTextWidget[a]->SetObserver(&positionTextObserver);
		
		rotationTextWidget[a] = static_cast<EditTextWidget *>(FindWidget(rotationIdentifier[a]));
		rotationTextWidget[a]->SetObserver(&rotationTextObserver);
	}
	
	for (machine a = 0; a < kMaxObjectSizeCount; a++)
	{
		sizeTextWidget[a] = static_cast<EditTextWidget *>(FindWidget(sizeIdentifier[a]));
		sizeTextWidget[a]->SetObserver(&sizeTextObserver);
	}
}

void TransformPage::HandleEditorEvent(Editor *editor, const EditorEvent& event)
{
	EditorEventType type = event.GetEventType();
	
	if (type == kEditorEventGizmoMoved)
	{
		undoType = kUndoNone;
		updateFlag = true;
		
		const Node *node = static_cast<const NodeEditorEvent *>(&event)->GetEventNode();
		if (node) UpdateTransform(node);
		else ClearTransform();
	}
	else if (type == kEditorEventGizmoTargetInvalidated)
	{
		if (updateFlag)
		{
			const Node *node = static_cast<const NodeEditorEvent *>(&event)->GetEventNode();
			UpdateTransform(node);
			undoType = kUndoNone;
		}
	}
}

void TransformPage::HandlePositionTextEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		Editor *editor = GetEditor();
		
		const NodeReference *reference = editor->GetGizmoTarget();
		if (reference)
		{
			Node *node = reference->GetNode();
			
			if (undoType != kUndoMove)
			{
				undoType = kUndoMove;
				editor->AddUndoData(new MoveUndoData(node));
			}
			
			const EditTextWidget *editText = static_cast<EditTextWidget *>(widget);
			float f = Text::StringToFloat(editText->GetText());
			
			Point3D position = node->GetNodePosition();
			for (machine a = 0; a < 3; a++)
			{
				if (editText == positionTextWidget[a])
				{
					position[a] = f;
					break;
				}
			}
			
			node->SetNodePosition(position);
			
			updateFlag = false;
			editor->InvalidateNode(node);
			
			if (node->GetNodeType() == kNodeGeometry)
			{
				node->Update();
				editor->RegenerateTexcoords(static_cast<Geometry *>(node));
			}
			
			updateFlag = true;
		}
	}
}

void TransformPage::HandleRotationTextEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		Editor *editor = GetEditor();
		
		const NodeReference *reference = editor->GetGizmoTarget();
		if (reference)
		{
			Node *node = reference->GetNode();
			
			if (undoType != kUndoMove)
			{
				undoType = kUndoMove;
				editor->AddUndoData(new MoveUndoData(node));
			}
			
			float x = Text::StringToFloat(rotationTextWidget[0]->GetText()) * K::radians;
			float y = Text::StringToFloat(rotationTextWidget[1]->GetText()) * K::radians;
			float z = Text::StringToFloat(rotationTextWidget[2]->GetText()) * K::radians;
			node->SetNodeMatrix3D(Matrix3D().SetEulerAngles(x, y, z));
			
			updateFlag = false;
			editor->InvalidateNode(node);
			
			if (node->GetNodeType() == kNodeGeometry)
			{
				node->Update();
				editor->RegenerateTexcoords(static_cast<Geometry *>(node));
			}
			
			updateFlag = true;
		}
	}
}

void TransformPage::HandleSizeTextEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		Editor *editor = GetEditor();
		
		const NodeReference *reference = editor->GetGizmoTarget();
		if (reference)
		{
			Node *node = reference->GetNode();
			Object *object = node->GetObject();
			if (object)
			{
				float	objectSize[kMaxObjectSizeCount];
				
				if (undoType != kUndoSize)
				{
					undoType = kUndoSize;
					editor->AddUndoData(new SizeUndoData(node));
				}
				
				int32 count = object->GetObjectSize(objectSize);
				for (machine a = 0; a < count; a++)
				{
					const char *string = sizeTextWidget[a]->GetText();
					for (machine b = 0;; b++)
					{
						unsigned_int32 c = string[b];
						if (c == 0) break;
						
						if (c - '0' < 10U)
						{
							objectSize[a] = Text::StringToFloat(string);
							break;
						}
					}
				}
				
				updateFlag = false;
				Editor::GetManipulator(node)->HandleSizeUpdate(count, objectSize);
				updateFlag = true;
			}
		}
	}
}

void TransformPage::ClearTransform(void)
{
	for (machine a = 0; a < 3; a++)
	{
		positionTextWidget[a]->SetText(nullptr);
		positionTextWidget[a]->Disable();
		
		rotationTextWidget[a]->SetText(nullptr);
		rotationTextWidget[a]->Disable();
	}
	
	for (machine a = 0; a < kMaxObjectSizeCount; a++)
	{
		sizeTextWidget[a]->SetText(nullptr);
		sizeTextWidget[a]->Disable();
	}
}

void TransformPage::UpdateTransform(const Node *node)
{
	float	x, y, z;
	
	const Point3D& position = node->GetNodePosition();
	positionTextWidget[0]->SetText(Text::FloatToString(position.x));
	positionTextWidget[1]->SetText(Text::FloatToString(position.y));
	positionTextWidget[2]->SetText(Text::FloatToString(position.z));
	
	node->GetNodeTransform().GetEulerAngles(&x, &y, &z);
	rotationTextWidget[0]->SetText(Text::FloatToString(x * K::degrees));
	rotationTextWidget[1]->SetText(Text::FloatToString(y * K::degrees));
	rotationTextWidget[2]->SetText(Text::FloatToString(z * K::degrees));
	
	if (!(Editor::GetManipulator(node)->GetManipulatorFlags() & kManipulatorLockedTransform))
	{
		for (machine a = 0; a < 3; a++)
		{
			positionTextWidget[a]->Enable();
			positionTextWidget[a]->SetWidgetAlpha(1.0F);
			rotationTextWidget[a]->Enable();
			rotationTextWidget[a]->SetWidgetAlpha(1.0F);
		}
	}
	else
	{
		for (machine a = 0; a < 3; a++)
		{
			positionTextWidget[a]->Disable();
			positionTextWidget[a]->SetWidgetAlpha(0.5F);
			rotationTextWidget[a]->Disable();
			rotationTextWidget[a]->SetWidgetAlpha(0.5F);
		}
	}
	
	int32 count = 0;
	
	const Object *object = node->GetObject();
	if (object)
	{
		float	objectSize[kMaxObjectSizeCount];
		
		count = object->GetObjectSize(objectSize);
		for (machine a = 0; a < count; a++)
		{
			sizeTextWidget[a]->Enable();
			sizeTextWidget[a]->SetText(Text::FloatToString(objectSize[a]));
		}
	}
	
	for (machine a = count; a < kMaxObjectSizeCount; a++)
	{
		sizeTextWidget[a]->SetText(nullptr);
		sizeTextWidget[a]->Disable();
	}
}


TextureMappingPage::TextureMappingPage() :
		EditorPage(kEditorPageTextureMapping, "WorldEditor/page/TextureMapping"),
		editorObserver(this, &TextureMappingPage::HandleEditorEvent),
		toolButtonObserver(this, &TextureMappingPage::HandleToolButtonEvent),
		offsetTextObserver(this, &TextureMappingPage::HandleOffsetTextEvent),
		scaleTextObserver(this, &TextureMappingPage::HandleScaleTextEvent),
		rotationTextObserver(this, &TextureMappingPage::HandleRotationTextEvent),
		reflectionBoxObserver(this, &TextureMappingPage::HandleReflectionBoxEvent),
		modeMenuObserver(this, &TextureMappingPage::HandleModeMenuEvent)
{
	currentTool = -1;
	textureUndoType = kTextureUndoNone;
	targetGeometry = nullptr;
}

TextureMappingPage::~TextureMappingPage()
{
}

void TextureMappingPage::Preprocess(void)
{
	static const char *const toolButtonIdentifier[kEditorTextureToolCount] =
	{
		"Offset", "Rotate", "Scale"
	};
	
	static const char *const offsetIdentifier[2] =
	{
		"Soffset", "Toffset"
	};
	
	static const char *const scaleIdentifier[2] =
	{
		"Sscale", "Tscale"
	};
	
	static const char *const modeIdentifier[2] =
	{
		"Salign", "Talign"
	};
	
	EditorPage::Preprocess();
	GetEditor()->AddObserver(&editorObserver);
	
	for (machine a = 0; a < kEditorTextureToolCount; a++)
	{
		toolButton[a] = static_cast<IconButtonWidget *>(FindWidget(toolButtonIdentifier[a]));
		toolButton[a]->SetObserver(&toolButtonObserver);
	}
	
	for (machine a = 0; a < 2; a++)
	{
		offsetTextWidget[a] = static_cast<EditTextWidget *>(FindWidget(offsetIdentifier[a]));
		offsetTextWidget[a]->SetObserver(&offsetTextObserver);
		
		scaleTextWidget[a] = static_cast<EditTextWidget *>(FindWidget(scaleIdentifier[a]));
		scaleTextWidget[a]->SetObserver(&scaleTextObserver);
	}
	
	rotationTextWidget = static_cast<EditTextWidget *>(FindWidget("Rot"));
	rotationTextWidget->SetObserver(&rotationTextObserver);
	
	reflectionCheckWidget = static_cast<CheckWidget *>(FindWidget("Reflect"));
	reflectionCheckWidget->SetObserver(&rotationTextObserver);
	
	for (machine a = 0; a < 2; a++)
	{
		modePopupMenu[a] = static_cast<PopupMenuWidget *>(FindWidget(modeIdentifier[a]));
		modePopupMenu[a]->SetObserver(&modeMenuObserver);
	}
	
	planePopupMenu = static_cast<PopupMenuWidget *>(FindWidget("Plane"));
	planePopupMenu->SetObserver(&modeMenuObserver);
}

void TextureMappingPage::HandleEditorEvent(Editor *editor, const EditorEvent& event)
{
	EditorEventType type = event.GetEventType();
	if (type == kEditorEventSelectionUpdated)
	{
		textureUndoType = kTextureUndoNone;
		targetGeometry = nullptr;
		
		const NodeReference *reference = editor->GetFirstSelectedNode();
		while (reference)
		{
			Node *node = reference->GetNode();
			if (node->GetNodeType() == kNodeGeometry)
			{
				const GeometryManipulator *manipulator = static_cast<GeometryManipulator *>(Editor::GetManipulator(node));
				if (manipulator->GetSelectionType() == kEditorSelectionSurface)
				{
					if (targetGeometry)
					{
						targetGeometry = nullptr;
						break;
					}
					
					Geometry *geometry = static_cast<Geometry *>(node);
					int32 surfaceCount = geometry->GetObject()->GetSurfaceCount();
					
					int32 selectionCount = 0;
					for (machine a = 0; a < surfaceCount; a++)
					{
						if (manipulator->SurfaceSelected(a))
						{
							selectionCount++;
							targetSurfaceIndex = a;
						}
					}
					
					if (selectionCount == 1) targetGeometry = geometry;
				}
			}
			
			reference = reference->Next();
		}
		
		UpdateTextureAlignData();
	}
	else if (type == kEditorEventTexcoordModified)
	{
		if (targetGeometry == static_cast<const NodeEditorEvent *>(&event)->GetEventNode())
		{
			textureUndoType = kTextureUndoNone;
			UpdateTextureAlignData();
		}
	}
}

void TextureMappingPage::HandleToolButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		GetEditor()->SetCurrentTool(this, widget);
	}
}

void TextureMappingPage::HandleOffsetTextEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		Geometry *geometry = targetGeometry;
		Editor *editor = GetEditor();
		
		if (textureUndoType != kTextureUndoOffset)
		{
			textureUndoType = kTextureUndoOffset;
			editor->AddUndoData(new TextureUndoData(geometry));
		}
		
		const EditTextWidget *editText = static_cast<EditTextWidget *>(widget);
		float f = Text::StringToFloat(editText->GetText());
		
		const GeometryObject *object = geometry->GetObject();
		TextureAlignData *alignData = object->GetSurfaceData(targetSurfaceIndex)->textureAlignData;
		
		if (editText == offsetTextWidget[0])
		{
			float offset = f - alignData[0].alignPlane.w;
			alignData[0].alignPlane.w = f;
			
			if (alignData[0].alignMode == kTextureAlignNatural) OffsetTexcoords(object, targetSurfaceIndex, Vector2D(offset, 0.0F));
		}
		else
		{
			float offset = f - alignData[1].alignPlane.w;
			alignData[1].alignPlane.w = f;
			
			if (alignData[1].alignMode == kTextureAlignNatural) OffsetTexcoords(object, targetSurfaceIndex, Vector2D(0.0F, offset));
		}
		
		editor->RegenerateTexcoords(geometry);
	}
}

void TextureMappingPage::HandleScaleTextEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		Geometry *geometry = targetGeometry;
		Editor *editor = GetEditor();
		
		if (textureUndoType != kTextureUndoScale)
		{
			textureUndoType = kTextureUndoScale;
			editor->AddUndoData(new TextureUndoData(geometry));
		}
		
		const EditTextWidget *editText = static_cast<EditTextWidget *>(widget);
		float f = Fabs(Text::StringToFloat(editText->GetText()));
		if (f != 0.0F)
		{
			const GeometryObject *object = geometry->GetObject();
			TextureAlignData *alignData = object->GetSurfaceData(targetSurfaceIndex)->textureAlignData;
			
			if (editText == scaleTextWidget[0])
			{
				float scale = f * InverseMag(alignData[0].alignPlane.GetAntivector3D());
				alignData[0].alignPlane.GetAntivector3D() *= scale;
				
				if (alignData[0].alignMode == kTextureAlignNatural) ScaleTexcoords(object, targetSurfaceIndex, Vector2D(scale, 0.0F));
			}
			else
			{
				float scale = f * InverseMag(alignData[1].alignPlane.GetAntivector3D());
				alignData[1].alignPlane.GetAntivector3D() *= scale;
				
				if (alignData[1].alignMode == kTextureAlignNatural) ScaleTexcoords(object, targetSurfaceIndex, Vector2D(0.0F, scale));
			}
			
			editor->RegenerateTexcoords(geometry);
		}
	}
}

void TextureMappingPage::HandleRotationTextEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		Transform4D		matrix;
		float			angle;
		
		Geometry *geometry = targetGeometry;
		Editor *editor = GetEditor();
		
		if (textureUndoType != kTextureUndoRotation)
		{
			textureUndoType = kTextureUndoRotation;
			editor->AddUndoData(new TextureUndoData(geometry));
		}
		
		const EditTextWidget *editText = static_cast<EditTextWidget *>(widget);
		float f = Text::StringToFloat(editText->GetText()) * K::radians;
		
		const GeometryObject *object = geometry->GetObject();
		TextureAlignData *alignData = object->GetSurfaceData(targetSurfaceIndex)->textureAlignData;
		
		bool snat = (alignData[0].alignMode == kTextureAlignNatural);
		bool tnat = (alignData[1].alignMode == kTextureAlignNatural);
		Vector3D& snormal = alignData[0].alignPlane.GetAntivector3D();
		Vector3D& tnormal = alignData[1].alignPlane.GetAntivector3D();
		
		if ((snormal.x == 0.0F) && (tnormal.x == 0.0F))
		{
			angle = f - Atan(snormal.z, snormal.y);
			matrix.SetRotationAboutX(angle);
		}
		else if ((snormal.y == 0.0F) && (tnormal.y == 0.0F))
		{
			angle = f - Atan(-snormal.z, snormal.x);
			matrix.SetRotationAboutY(angle);
		}
		else
		{
			angle = f - Atan(snormal.y, snormal.x);
			matrix.SetRotationAboutZ(angle);
		}
		
		snormal = matrix * snormal;
		tnormal = matrix * tnormal;
		
		if (snat | tnat)
		{
			matrix.SetRotationAboutZ(angle);
			if (!snat) matrix.SetRow(0, K::x_unit);
			if (!tnat) matrix.SetRow(1, K::y_unit);
			RotateTexcoords(object, targetSurfaceIndex, matrix);
		}
		
		editor->RegenerateTexcoords(geometry);
	}
}

void TextureMappingPage::HandleReflectionBoxEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		Geometry *geometry = targetGeometry;
		Editor *editor = GetEditor();
		
		if (textureUndoType != kTextureUndoReflection)
		{
			textureUndoType = kTextureUndoReflection;
			editor->AddUndoData(new TextureUndoData(geometry));
		}
		
		const GeometryObject *object = geometry->GetObject();
		TextureAlignData *alignData = object->GetSurfaceData(targetSurfaceIndex)->textureAlignData;
		
		Antivector4D& splane = alignData[0].alignPlane;
		splane = -splane;
		
		if (alignData[0].alignMode == kTextureAlignNatural) ScaleTexcoords(object, targetSurfaceIndex, Vector2D(-1.0F, 1.0F));
		editor->RegenerateTexcoords(geometry);
		
		UpdateTextureAlignData();
	}
}

void TextureMappingPage::HandleModeMenuEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		Geometry *geometry = targetGeometry;
		Editor *editor = GetEditor();
		
		if (textureUndoType != kTextureUndoMode)
		{
			textureUndoType = kTextureUndoMode;
			editor->AddUndoData(new TextureUndoData(geometry));
		}
		
		const PopupMenuWidget *popupMenu = static_cast<PopupMenuWidget *>(widget);
		
		const GeometryObject *object = geometry->GetObject();
		TextureAlignData *alignData = object->GetSurfaceData(targetSurfaceIndex)->textureAlignData;
		
		if (popupMenu == modePopupMenu[0]) alignData[0].alignMode = alignModeTable[popupMenu->GetSelection()];
		else if (popupMenu == modePopupMenu[1]) alignData[1].alignMode = alignModeTable[popupMenu->GetSelection()];
		
		const Antivector4D& splane = alignData[0].alignPlane;
		const Antivector4D& tplane = alignData[1].alignPlane;
		
		float sx = splane.x;
		float sy = splane.y;
		float tx = tplane.x;
		float ty = tplane.y;
		
		if ((splane.x == 0.0F) && (tplane.x == 0.0F))
		{
			sx = splane.y;
			sy = splane.z;
			tx = tplane.y;
			ty = tplane.z;
		}
		else if ((splane.y == 0.0F) && (tplane.y == 0.0F))
		{
			sx = splane.x;
			sy = -splane.z;
			tx = tplane.x;
			ty = -tplane.z;
		}
		
		int32 selection = planePopupMenu->GetSelection();
		
		if (selection == 0) alignData[0].alignPlane.Set(sx, sy, 0.0F, splane.w);
		else if (selection == 1) alignData[0].alignPlane.Set(sx, 0.0F, -sy, splane.w);
		else alignData[0].alignPlane.Set(0.0F, sx, sy, splane.w);
		
		if (selection == 0) alignData[1].alignPlane.Set(tx, ty, 0.0F, tplane.w);
		else if (selection == 1) alignData[1].alignPlane.Set(tx, 0.0F, -ty, tplane.w);
		else alignData[1].alignPlane.Set(0.0F, tx, ty, tplane.w);
		
		editor->RegenerateTexcoords(geometry);
	}
}

void TextureMappingPage::Engage(Editor *editor, void *cookie)
{
	Widget *widget = static_cast<Widget *>(cookie);
	
	for (machine a = 0; a < kEditorTextureToolCount; a++)
	{
		if (widget == toolButton[a])
		{
			currentTool = a;
			toolButton[a]->SetValue(1);
			break;
		}
	}
}

void TextureMappingPage::Disengage(Editor *editor, void *cookie)
{
	if (currentTool != -1)
	{
		toolButton[currentTool]->SetValue(0);
		currentTool = -1;
	}
}

bool TextureMappingPage::BeginTool(Editor *editor, EditorTrackData *trackData)
{
	if (trackData->viewportType != kEditorViewportGraph)
	{
		PickData	pickData;
		
		Node *node = editor->PickNode(trackData, &pickData, 1 << kEditorNodeGeometry);
		if ((node) && (node->GetManipulator()->Selected()))
		{
			undoDataFlag = false;
			return (true);
		}
	}
	
	return (false);
}

bool TextureMappingPage::TrackTool(Editor *editor, EditorTrackData *trackData)
{
	if (trackData->currentModifierKeys & kModifierKeyShift)
	{
		float dx = trackData->currentPosition.x - trackData->anchorPosition.x;
		float dy = trackData->currentPosition.y - trackData->anchorPosition.y;
		
		if (Fabs(dy) > Fabs(dx)) trackData->currentPosition.x = trackData->anchorPosition.x;
		else trackData->currentPosition.y = trackData->anchorPosition.y;
	}
	
	if (trackData->currentPosition != trackData->previousPosition)
	{
		if (!undoDataFlag)
		{
			undoDataFlag = true;
			editor->AddUndoData(new TextureUndoData(editor->GetSelectionList()));
		}
		
		Vector2D delta = (trackData->currentPosition - trackData->previousPosition) * 0.125F;
		
		const NodeReference *reference = editor->GetFirstSelectedNode();
		while (reference)
		{
			Node *node = reference->GetNode();
			if (node->GetNodeType() == kNodeGeometry)
			{
				Geometry *geometry = static_cast<Geometry *>(node);
				const GeometryObject *object = static_cast<Geometry *>(node)->GetObject();
				int32 surfaceCount = object->GetSurfaceCount();
				
				const GeometryManipulator *manipulator = static_cast<GeometryManipulator *>(Editor::GetManipulator(geometry));
				int32 selectedCount = manipulator->GetSelectedSurfaceCount();
				
				for (machine a = 0; a < surfaceCount; a++)
				{
					if ((selectedCount == 0) || (manipulator->SurfaceSelected(a)))
					{
						SurfaceData *data = object->GetSurfaceData(a);
						bool snat = (data->textureAlignData[0].alignMode == kTextureAlignNatural);
						bool tnat = (data->textureAlignData[1].alignMode == kTextureAlignNatural);
						
						if (currentTool == kEditorTextureToolOffset)
						{
							data->textureAlignData[0].alignPlane.w -= delta.x;
							data->textureAlignData[1].alignPlane.w += delta.y;
							
							if (snat | tnat) OffsetTexcoords(object, a, Vector2D((snat) ? -delta.x : 0.0F, (tnat) ? delta.y : 0.0F));
						}
						else if (currentTool == kEditorTextureToolRotate)
						{
							Transform4D		matrix;
							
							float angle = delta.y;
							
							Vector3D& snormal = data->textureAlignData[0].alignPlane.GetAntivector3D();
							Vector3D& tnormal = data->textureAlignData[1].alignPlane.GetAntivector3D();
							
							if ((snormal.x == 0.0F) && (tnormal.x == 0.0F)) matrix.SetRotationAboutX(angle);
							else if ((snormal.y == 0.0F) && (tnormal.y == 0.0F)) matrix.SetRotationAboutY(angle);
							else matrix.SetRotationAboutZ(angle);
							
							snormal = matrix * snormal;
							tnormal = matrix * tnormal;
							
							if (snat | tnat)
							{
								matrix.SetRotationAboutZ(angle);
								if (!snat) matrix.SetRow(0, K::x_unit);
								if (!tnat) matrix.SetRow(1, K::y_unit);
								RotateTexcoords(object, a, matrix);
							}
						}
						else if (currentTool == kEditorTextureToolScale)
						{
							float sx = Exp(delta.x * K::ln_2);
							float sy = Exp(-delta.y * K::ln_2);
							
							data->textureAlignData[0].alignPlane.GetAntivector3D() *= sx;
							data->textureAlignData[1].alignPlane.GetAntivector3D() *= sy;
							
							if (snat | tnat) ScaleTexcoords(object, a, Vector2D((snat) ? sx : 1.0F, (tnat) ? sy : 1.0F));
						}
					}
				}
				
				editor->RegenerateTexcoords(geometry);
			}
			
			reference = reference->Next();
		}
		
		if (targetGeometry) UpdateTextureAlignData();
	}
	
	return (true);
}

bool TextureMappingPage::EndTool(Editor *editor, EditorTrackData *trackData)
{
	return (TrackTool(editor, trackData));
}

void TextureMappingPage::OffsetTexcoords(const GeometryObject *object, unsigned_int32 index, const Vector2D& offset)
{
	int32 levelCount = object->GetGeometryLevelCount();
	for (machine a = 0; a < levelCount; a++)
	{
		GeometryLevel *level = object->GetGeometryLevel(a);
		
		int32 vertexCount = level->GetVertexCount();
		Point2D *texcoord = level->GetArray<Point2D>(kArrayTexture0);
		
		const unsigned_int16 *surfaceIndex = level->GetArray<unsigned_int16>(kArraySurfaceIndex);
		if (surfaceIndex)
		{
			for (machine a = 0; a < vertexCount; a++)
			{
				if (surfaceIndex[a] == index) texcoord[a] += offset;
			}
		}
		else
		{
			for (machine a = 0; a < vertexCount; a++) texcoord[a] += offset;
		}
	}
}

void TextureMappingPage::RotateTexcoords(const GeometryObject *object, unsigned_int32 index, const Transform4D& rotation)
{
	int32 levelCount = object->GetGeometryLevelCount();
	for (machine a = 0; a < levelCount; a++)
	{
		GeometryLevel *level = object->GetGeometryLevel(a);
		
		int32 vertexCount = level->GetVertexCount();
		Point2D *texcoord = level->GetArray<Point2D>(kArrayTexture0);
		
		const unsigned_int16 *surfaceIndex = level->GetArray<unsigned_int16>(kArraySurfaceIndex);
		if (surfaceIndex)
		{
			for (machine a = 0; a < vertexCount; a++)
			{
				if (surfaceIndex[a] == index) texcoord[a] = rotation * texcoord[a];
			}
		}
		else
		{
			for (machine a = 0; a < vertexCount; a++) texcoord[a] = rotation * texcoord[a];
		}
	}
}

void TextureMappingPage::ScaleTexcoords(const GeometryObject *object, unsigned_int32 index, const Vector2D& scale)
{
	int32 levelCount = object->GetGeometryLevelCount();
	for (machine a = 0; a < levelCount; a++)
	{
		GeometryLevel *level = object->GetGeometryLevel(a);
		
		int32 vertexCount = level->GetVertexCount();
		Point2D *texcoord = level->GetArray<Point2D>(kArrayTexture0);
		
		const unsigned_int16 *surfaceIndex = level->GetArray<unsigned_int16>(kArraySurfaceIndex);
		if (surfaceIndex)
		{
			for (machine a = 0; a < vertexCount; a++)
			{
				if (surfaceIndex[a] == index) texcoord[a] &= scale;
			}
		}
		else
		{
			for (machine a = 0; a < vertexCount; a++) texcoord[a] &= scale;
		}
	}
}

void TextureMappingPage::UpdateTextureAlignData(void)
{
	if (targetGeometry)
	{
		const SurfaceData *surfaceData = targetGeometry->GetObject()->GetSurfaceData(targetSurfaceIndex);
		for (machine a = 0; a < 2; a++)
		{
			const TextureAlignData *alignData = &surfaceData->textureAlignData[a];
			
			offsetTextWidget[a]->SetText(Text::FloatToString(alignData->alignPlane.w));
			offsetTextWidget[a]->Enable();
			
			scaleTextWidget[a]->SetText(Text::FloatToString(Magnitude(alignData->alignPlane.GetAntivector3D())));
			scaleTextWidget[a]->Enable();
			
			int32 selection = 0;
			TextureAlignMode mode = alignData->alignMode;
			if (mode == kTextureAlignObjectPlane) selection = 1;
			else if (mode == kTextureAlignWorldPlane) selection = 2;
			else if (mode == kTextureAlignGlobalObjectPlane) selection = 3;
			
			modePopupMenu[a]->SetSelection(selection);
			modePopupMenu[a]->Enable();
		}
		
		const Antivector4D& splane = surfaceData->textureAlignData[0].alignPlane;
		const Antivector4D& tplane = surfaceData->textureAlignData[1].alignPlane;
		
		float sx = splane.x;
		float sy = splane.y;
		float tx = tplane.x;
		float ty = tplane.y;
		int32 selection = 0;
		
		if ((splane.y == 0.0F) && (tplane.y == 0.0F))
		{
			sx = splane.x;
			sy = -splane.z;
			tx = tplane.x;
			ty = -tplane.z;
			selection = 1;
		}
		else if ((splane.x == 0.0F) && (tplane.x == 0.0F))
		{
			sx = splane.y;
			sy = splane.z;
			tx = tplane.y;
			ty = tplane.z;
			selection = 2;
		}
		
		rotationTextWidget->SetText(Text::FloatToString(Atan(sy, sx) * K::degrees));
		rotationTextWidget->Enable();
		
		float d = sx * ty - sy * tx;
		reflectionCheckWidget->SetValue(d < 0.0F);
		reflectionCheckWidget->Enable();
		
		planePopupMenu->SetSelection(selection);
		planePopupMenu->Enable();
	}
	else
	{
		for (machine a = 0; a < 2; a++)
		{
			offsetTextWidget[a]->SetText(nullptr);
			offsetTextWidget[a]->Disable();
			
			scaleTextWidget[a]->SetText(nullptr);
			scaleTextWidget[a]->Disable();
			
			modePopupMenu[a]->SetSelection(kWidgetValueNone);
			modePopupMenu[a]->Disable();
		}
		
		rotationTextWidget->SetText(nullptr);
		rotationTextWidget->Disable();
		
		reflectionCheckWidget->SetValue(0);
		reflectionCheckWidget->Disable();
		
		planePopupMenu->SetSelection(kWidgetValueNone);
		planePopupMenu->Disable();
	}
}


InfoPage::InfoPage() :
		EditorPage(kEditorPageInfo, "WorldEditor/page/Info"),
		editorObserver(this, &InfoPage::HandleEditorEvent)
{
}

InfoPage::~InfoPage()
{
}

void InfoPage::Preprocess(void)
{
	static const char *const nodeInfoIdentifier[kNodeInfoCount] =
	{
		"Name", "Controller", "Connectors", "Properties", "Instances"
	};
	
	static const char *const geometryInfoIdentifier[kGeometryInfoCount] =
	{
		"Vertices", "Faces", "Surfaces", "Materials", "Levels"
	};
	
	EditorPage::Preprocess();
	GetEditor()->AddObserver(&editorObserver);
	
	nodeIconWidget = static_cast<ImageWidget *>(FindWidget("Icon"));
	
	geometryGroup = FindWidget("GeometryGroup");
	worldGroup = FindWidget("WorldGroup");
	typeGroup = FindWidget("TypeGroup");
	
	for (machine a = 0; a < kNodeInfoCount; a++) nodeWidget[a] = static_cast<TextWidget *>(FindWidget(nodeInfoIdentifier[a]));
	for (machine a = 0; a < kGeometryInfoCount; a++) geometryWidget[a] = static_cast<TextWidget *>(FindWidget(geometryInfoIdentifier[a]));
	
	worldWidget = static_cast<TextWidget *>(FindWidget("World"));
	typeWidget = static_cast<TextWidget *>(FindWidget("Type"));
}

void InfoPage::HandleEditorEvent(Editor *editor, const EditorEvent& event)
{
	EditorEventType type = event.GetEventType();
	
	if (type == kEditorEventGizmoMoved)
	{
		const Node *node = static_cast<const NodeEditorEvent *>(&event)->GetEventNode();
		if (node) UpdateInfo(node);
		else ClearInfo();
	}
	else if (type == kEditorEventGizmoTargetModified)
	{
		UpdateInfo(static_cast<const NodeEditorEvent *>(&event)->GetEventNode());
	}
}

void InfoPage::ClearInfo(void)
{
	nodeIconWidget->Hide();
	for (machine a = 0; a < kNodeInfoCount; a++) nodeWidget[a]->SetText(nullptr);
	
	geometryGroup->Hide();
	worldGroup->Hide();
	typeGroup->Hide();
}

void InfoPage::UpdateInfo(const Node *node)
{
	const EditorManipulator *manipulator = Editor::GetManipulator(node);
	
	nodeIconWidget->SetTexture(0, manipulator->GetIconName());
	nodeIconWidget->Show();
	
	const char *name = node->GetNodeName();
	if (name) nodeWidget[kNodeInfoName]->SetText(name);
	else nodeWidget[kNodeInfoName]->SetText(manipulator->GetDefaultNodeName());
	
	const StringTable *table = TheWorldEditor->GetStringTable();
	
	const Controller *controller = node->GetController();
	if (controller)
	{
		ControllerType controllerType = controller->GetControllerType();
		if (controllerType == kControllerGeneric)
		{
			nodeWidget[kNodeInfoController]->SetText(table->GetString(StringID('PAGE', kEditorPageInfo, 'GNRC')));
		}
		else
		{
			const ControllerRegistration *registration = Controller::FindRegistration(controllerType);
			if (registration) nodeWidget[kNodeInfoController]->SetText(registration->GetControllerName());
			else nodeWidget[kNodeInfoController]->SetText(Text::TypeToString(controllerType));
		}
	}
	else
	{
		nodeWidget[kNodeInfoController]->SetText(table->GetString(StringID('PAGE', kEditorPageInfo, 'NONE')));
	}
	
	const Hub *hub = node->GetHub();
	if (hub) nodeWidget[kNodeInfoConnectors]->SetText(String<7>(hub->GetOutgoingEdgeCount()));
	else nodeWidget[kNodeInfoConnectors]->SetText("0");
	
	nodeWidget[kNodeInfoProperties]->SetText(String<7>(node->GetPropertyCount()));
	
	const Object *object = node->GetObject();
	if (object)
	{
		// We can't just use the object's reference count here because there might be
		// nodes in the undo buffer that still refer to the the object.
		
		int32 count = 0;
		Node *root = GetEditor()->GetRootNode();
		Node *subnode = root;
		do
		{
			count += (subnode->GetObject() == object);
			subnode = root->GetNextNode(subnode);
		} while (subnode);
		
		nodeWidget[kNodeInfoInstances]->SetText(String<7>(count));
	}
	else
	{
		nodeWidget[kNodeInfoInstances]->SetText("0");
	}
	
	NodeType nodeType = node->GetNodeType();
	if (nodeType == kNodeGeometry)
	{
		const Geometry *geometry = static_cast<const Geometry *>(node);
		const GeometryObject *object = geometry->GetObject();
		const GeometryLevel *level = object->GetGeometryLevel(0);
		
		geometryWidget[kGeometryInfoVertices]->SetText(String<7>(level->GetVertexCount()));
		geometryWidget[kGeometryInfoFaces]->SetText(String<7>(level->GetFaceCount()));
		geometryWidget[kGeometryInfoSurfaces]->SetText(String<7>(object->GetSurfaceCount()));
		geometryWidget[kGeometryInfoMaterials]->SetText(String<7>(geometry->GetMaterialCount()));
		geometryWidget[kGeometryInfoLevels]->SetText(String<7>(object->GetGeometryLevelCount()));
		
		geometryGroup->Show();
		worldGroup->Hide();
		typeGroup->Hide();
	}
	else if (nodeType == kNodeMarker)
	{
		const Marker *marker = static_cast<const Marker *>(node);
		MarkerType markerType = marker->GetMarkerType();
		if (markerType == kMarkerLocator)
		{
			const LocatorMarker *locator = static_cast<const LocatorMarker *>(marker);
			LocatorType locatorType = locator->GetLocatorType();
			if (locatorType != 0) typeWidget->SetText(Text::TypeToString(locatorType));
			else typeWidget->SetText(table->GetString(StringID('PAGE', kEditorPageInfo, 'NONE')));
			
			typeGroup->Show();
			worldGroup->Hide();
		}
		
		geometryGroup->Hide();
	}
	else if (nodeType == kNodeInstance)
	{
		const Instance *instance = static_cast<const Instance *>(node);
		worldWidget->SetText(instance->GetWorldName());
		
		worldGroup->Show();
		typeGroup->Hide();
		geometryGroup->Hide();
	}
	else if (nodeType == kNodeModel)
	{
		ModelType modelType = static_cast<const Model *>(node)->GetModelType();
		const ModelRegistration *registration = Model::FindRegistration(modelType);
		if (registration) typeWidget->SetText(registration->GetModelName());
		else typeWidget->SetText(Text::TypeToString(modelType));
		
		typeGroup->Show();
		worldGroup->Hide();
		geometryGroup->Hide();
	}
}


GridPage::GridPage() :
		EditorPage(kEditorPageGrid, "WorldEditor/page/Grid"),
		gridButtonObserver(this, &GridPage::HandleGridButtonEvent),
		gridTextObserver(this, &GridPage::HandleGridTextEvent),
		gridColorObserver(this, &GridPage::HandleGridColorEvent)
{
	InitialShow();
}

GridPage::~GridPage()
{
}

void GridPage::Preprocess(void)
{
	static const char *const buttonIdentifier[kEditorGridButtonCount] =
	{
		"Grid", "Snap", "Axes", "Halve", "Double"
	};
	
	EditorPage::Preprocess();
	
	for (machine a = 0; a < kEditorGridButtonCount; a++)
	{
		gridButton[a] = static_cast<IconButtonWidget *>(FindWidget(buttonIdentifier[a]));
		gridButton[a]->SetObserver(&gridButtonObserver);
	}
	
	gridSpacingWidget = static_cast<EditTextWidget *>(FindWidget("Spacing"));
	majorLineWidget = static_cast<EditTextWidget *>(FindWidget("Major"));
	gridColorWidget = static_cast<ColorWidget *>(FindWidget("Color"));
	
	gridSpacingWidget->SetObserver(&gridTextObserver);
	majorLineWidget->SetObserver(&gridTextObserver);
	gridColorWidget->SetObserver(&gridColorObserver);
	
	const EditorObject *object = GetEditor()->GetEditorObject();
	
	unsigned_int32 flags = object->GetEditorFlags();
	if (flags & kEditorShowGridlines) gridButton[kEditorGridButtonShow]->SetValue(1);
	if (flags & kEditorSnapToGrid) gridButton[kEditorGridButtonSnap]->SetValue(1);
	if (flags & kEditorShowViewportInfo) gridButton[kEditorGridButtonAxes]->SetValue(1);
	
	gridSpacingWidget->SetText(Text::FloatToString(object->GetGridLineSpacing()));
	majorLineWidget->SetText(Text::IntegerToString(object->GetMajorLineInterval()));
	gridColorWidget->SetValue(object->GetGridColor());
}

void GridPage::HandleGridButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	Editor *editor = GetEditor();
	EditorObject *editorObject = editor->GetEditorObject();
	
	EventType eventType = eventData->eventType;
	
	if (eventType == kEventWidgetActivate)
	{
		if (widget == gridButton[kEditorGridButtonHalve])
		{
			float spacing = Fmax(editorObject->GetGridLineSpacing() * 0.5F, kMinEditorGridSpacing);
			editorObject->SetGridLineSpacing(spacing);
			gridSpacingWidget->SetText(Text::FloatToString(spacing));
			
			editor->InvalidateAllViewports();
			editor->InvalidateAllViewports();
		}
		else if (widget == gridButton[kEditorGridButtonDouble])
		{
			float spacing = editorObject->GetGridLineSpacing() * 2.0F;
			editorObject->SetGridLineSpacing(spacing);
			gridSpacingWidget->SetText(Text::FloatToString(spacing));
			
			editor->InvalidateAllViewports();
			editor->InvalidateAllViewports();
		}
	}
	else if (eventType == kEventWidgetChange)
	{
		if (widget == gridButton[kEditorGridButtonShow])
		{
			unsigned_int32 flags = editorObject->GetEditorFlags() ^ kEditorShowGridlines;
			editorObject->SetEditorFlags(flags);
			gridButton[kEditorGridButtonShow]->SetValue((flags & kEditorShowGridlines) != 0);
			editor->InvalidateAllViewports();
		}
		else if (widget == gridButton[kEditorGridButtonSnap])
		{
			unsigned_int32 flags = editorObject->GetEditorFlags() ^ kEditorSnapToGrid;
			editorObject->SetEditorFlags(flags);
			gridButton[kEditorGridButtonSnap]->SetValue((flags & kEditorSnapToGrid) != 0);
		}
		else if (widget == gridButton[kEditorGridButtonAxes])
		{
			unsigned_int32 flags = editorObject->GetEditorFlags() ^ kEditorShowViewportInfo;
			editorObject->SetEditorFlags(flags);
			gridButton[kEditorGridButtonAxes]->SetValue((flags & kEditorShowViewportInfo) != 0);
			
			if (flags & kEditorShowViewportInfo)
			{
				for (machine a = 0; a < kEditorViewportCount; a++) editor->GetViewport(a)->ShowViewportInfo();
			}
			else
			{
				for (machine a = 0; a < kEditorViewportCount; a++) editor->GetViewport(a)->HideViewportInfo();
			}
			
			editor->InvalidateAllViewports();
		}
	}
}

void GridPage::HandleGridTextEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		Editor *editor = GetEditor();
		EditorObject *editorObject = editor->GetEditorObject();
		
		if (widget == gridSpacingWidget)
		{
			editorObject->SetGridLineSpacing(Fmax(Text::StringToFloat(gridSpacingWidget->GetText()), kMinEditorGridSpacing));
			editor->InvalidateAllViewports();
			editor->InvalidateAllViewports();
		}
		else if (widget == majorLineWidget)
		{
			editorObject->SetMajorLineInterval(Max(Text::StringToInteger(majorLineWidget->GetText()), 2));
			editor->InvalidateAllViewports();
			editor->InvalidateAllViewports();
		}
	}
}

void GridPage::HandleGridColorEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		Editor *editor = GetEditor();
		EditorObject *editorObject = editor->GetEditorObject();
		
		editorObject->SetGridColor(gridColorWidget->GetValue().GetColorRGB());
		editor->InvalidateAllViewports();
		editor->InvalidateAllViewports();
	}
}


FindPage::FindPage() :
		EditorPage(kEditorPageFind, "WorldEditor/page/Find"),
		findAllButtonObserver(this, &FindPage::HandleFindAllButtonEvent),
		findNextButtonObserver(this, &FindPage::HandleFindNextButtonEvent),
		findPreviousButtonObserver(this, &FindPage::HandleFindPreviousButtonEvent)
{
	controllerTypeTable = nullptr;
}

FindPage::~FindPage()
{
	delete[] controllerTypeTable;
}

void FindPage::Preprocess(void)
{
	EditorPage::Preprocess();
	
	nameWidget = static_cast<EditTextWidget *>(FindWidget("Name"));
	typeWidget = static_cast<PopupMenuWidget *>(FindWidget("Type"));
	controllerWidget = static_cast<PopupMenuWidget *>(FindWidget("Controller"));
	
	PushButtonWidget *findAllWidget = static_cast<PushButtonWidget *>(FindWidget("All"));
	PushButtonWidget *findNextWidget = static_cast<PushButtonWidget *>(FindWidget("Next"));
	PushButtonWidget *findPreviousWidget = static_cast<PushButtonWidget *>(FindWidget("Previous"));
	
	findAllWidget->SetObserver(&findAllButtonObserver);
	findNextWidget->SetObserver(&findNextButtonObserver);
	findPreviousWidget->SetObserver(&findPreviousButtonObserver);
	
	const StringTable *table = TheWorldEditor->GetStringTable();
	typeWidget->AppendMenuItem(new MenuItemWidget(table->GetString(StringID('PAGE', 'FIND', 'GRUP'))));
	for (machine a = 1; a < kNodeTypeCount; a++) typeWidget->AppendMenuItem(new MenuItemWidget(table->GetString(StringID('PAGE', 'FIND', nodeTypeTable[a]))));
	
	int32 count = Controller::GetRegistrationCount();
	controllerTypeTable = new ControllerType[count];
	
	int32 index = 0;
	const ControllerRegistration *registration = Controller::GetFirstRegistration();
	while (registration)
	{
		const char *name = registration->GetControllerName();
		if (name)
		{
			controllerTypeTable[index++] = registration->GetControllerType();
			controllerWidget->AppendMenuItem(new MenuItemWidget(name));
		}
		
		registration = registration->Next();
	}
}

bool FindPage::MatchingNode(const Node *node) const
{
	if (node->GetNodeFlags() & kNodeNonpersistent) return (false);
	
	const char *findName = nameWidget->GetText();
	if (findName[0] != 0)
	{
		const char *nodeName = node->GetNodeName();
		if ((!nodeName) || (!Text::CompareTextCaseless(nodeName, findName))) return (false);
	}
	
	int32 index = typeWidget->GetSelection() - 1;
	if (index >= 0)
	{
		if (node->GetNodeType() != nodeTypeTable[index]) return (false);
	}
	
	index = controllerWidget->GetSelection() - 1;
	if (index >= 0)
	{
		const Controller *controller = node->GetController();
		if (index == 0)
		{
			if (controller) return (false);
		}
		else
		{
			if ((!controller) || (controller->GetControllerType() != controllerTypeTable[index - 1])) return (false);
		}
	}
	
	return (true);
}

void FindPage::HandleFindAllButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetActivate)
	{
		Editor *editor = GetEditor();
		editor->UnselectAll();
		
		Node *root = editor->GetRootNode();
		Node *node = root;
		while (node)
		{
			if (MatchingNode(node)) editor->SelectNode(node);
			node = root->GetNextNode(node);
		}
		
		editor->FrameSelectionAllViewports();
	}
}

void FindPage::HandleFindNextButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetActivate)
	{
		Editor *editor = GetEditor();
		Node *root = editor->GetRootNode();
		
		const NodeReference *reference = editor->GetFirstSelectedNode();
		Node *node = (reference) ? reference->GetNode() : nullptr;
		
		editor->UnselectAll();
		
		if (node) node = root->GetNextNode(node);
		if (!node) node = root;
		const Node *stop = node;
		
		for (;;)
		{
			if (MatchingNode(node))
			{
				editor->SelectNode(node);
				break;
			}
			
			if (node) node = root->GetNextNode(node);
			if (!node) node = root;
			if (node == stop) return;
		}
		
		editor->FrameSelectionAllViewports();
	}
}

void FindPage::HandleFindPreviousButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetActivate)
	{
		Editor *editor = GetEditor();
		Node *root = editor->GetRootNode();
		
		const NodeReference *reference = editor->GetFirstSelectedNode();
		Node *node = (reference) ? reference->GetNode() : nullptr;
		
		editor->UnselectAll();
		
		if (node) node = root->GetPreviousNode(node);
		if (!node) node = root->GetRightmostNode();
		const Node *stop = node;
		
		for (;;)
		{
			if (MatchingNode(node))
			{
				editor->SelectNode(node);
				break;
			}
			
			if (node) node = root->GetPreviousNode(node);
			if (!node) node = root->GetRightmostNode();
			if (node == stop) return;
		}
		
		editor->FrameSelectionAllViewports();
	}
}


PaintPage::PaintPage() :
		EditorPage(kEditorPagePaint, "WorldEditor/page/Paint"),
		editorObserver(this, &PaintPage::HandleEditorEvent),
		toolButtonObserver(this, &PaintPage::HandleToolButtonEvent),
		channelButtonObserver(this, &PaintPage::HandleChannelButtonEvent),
		colorObserver(this, &PaintPage::HandleColorEvent),
		checkObserver(this, &PaintPage::HandleCheckEvent),
		sliderObserver(this, &PaintPage::HandleSliderEvent),
		menuButtonObserver(this, &PaintPage::HandleMenuButtonEvent)
{
	currentTool = -1;
	targetPaintSpace = nullptr;
	
	for (machine a = 0; a < 4; a++) channelMask[a] = true;
	brushColor.Set(1.0F, 1.0F, 1.0F, 1.0F);
	
	invertValue = 0;
	stylusValue = 0;
	strengthValue = 100;
	radiusValue = 7;
	fuzzyValue = 50;
}

PaintPage::~PaintPage()
{
}

void PaintPage::Pack(Packer& data, unsigned_int32 packFlags) const
{
	EditorPage::Pack(data, packFlags);
	
	data << ChunkHeader('MASK', 16);
	for (machine a = 0; a < 4; a++) data << channelMask[a];
	
	data << ChunkHeader('BCOL', sizeof(ColorRGBA));
	data << brushColor;
	
	data << ChunkHeader('INVT', 4);
	data << invertValue;
	
	data << ChunkHeader('STYL', 4);
	data << stylusValue;
	
	data << ChunkHeader('STRE', 4);
	data << strengthValue;
	
	data << ChunkHeader('RADI', 4);
	data << radiusValue;
	
	data << ChunkHeader('FUZZ', 4);
	data << fuzzyValue;
	
	data << TerminatorChunk;
}

void PaintPage::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	EditorPage::Unpack(data, unpackFlags);
	UnpackChunkList<PaintPage>(data, unpackFlags);
}

bool PaintPage::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'MASK':
			
			for (machine a = 0; a < 4; a++) data >> channelMask[a];
			return (true);
		
		case 'BCOL':
			
			data >> brushColor;
			return (true);
		
		case 'INVT':
			
			data >> invertValue;
			return (true);
		
		case 'STYL':
			
			data >> stylusValue;
			return (true);
		
		case 'STRE':
			
			data >> strengthValue;
			return (true);
		
		case 'RADI':
			
			data >> radiusValue;
			return (true);
		
		case 'FUZZ':
			
			data >> fuzzyValue;
			return (true);
	}
	
	return (false);
}

void PaintPage::Preprocess(void)
{
	static const char *const buttonIdentifier[kEditorPaintToolCount] =
	{
		"Paint"
	};
	
	static const char *const channelIdentifier[4] =
	{
		"Red", "Green", "Blue", "Alpha"
	};
	
	EditorPage::Preprocess();
	GetEditor()->AddObserver(&editorObserver);
	
	for (machine a = 0; a < kEditorPaintToolCount; a++)
	{
		toolButton[a] = static_cast<IconButtonWidget *>(FindWidget(buttonIdentifier[a]));
		toolButton[a]->SetObserver(&toolButtonObserver);
	}
	
	for (machine a = 0; a < 4; a++)
	{
		channelButton[a] = static_cast<IconButtonWidget *>(FindWidget(channelIdentifier[a]));
		channelButton[a]->SetObserver(&channelButtonObserver);
	}
	
	colorWidget = static_cast<ColorWidget *>(FindWidget("Color"));
	invertWidget = static_cast<CheckWidget *>(FindWidget("Invert"));
	stylusWidget = static_cast<CheckWidget *>(FindWidget("Stylus"));
	imageWidget = static_cast<ImageWidget *>(FindWidget("Image"));
	
	colorWidget->SetObserver(&colorObserver);
	invertWidget->SetObserver(&checkObserver);
	stylusWidget->SetObserver(&checkObserver);
	
	strengthSlider = static_cast<SliderWidget *>(FindWidget("StrengthValue"));
	strengthText = static_cast<TextWidget *>(FindWidget("StrengthText"));
	
	radiusSlider = static_cast<SliderWidget *>(FindWidget("RadiusValue"));
	radiusText = static_cast<TextWidget *>(FindWidget("RadiusText"));
	
	fuzzySlider = static_cast<SliderWidget *>(FindWidget("FuzzyValue"));
	fuzzyText = static_cast<TextWidget *>(FindWidget("FuzzyText"));
	
	strengthSlider->SetObserver(&sliderObserver);
	radiusSlider->SetObserver(&sliderObserver);
	fuzzySlider->SetObserver(&sliderObserver);
	
	for (machine a = 0; a < 4; a++) channelButton[a]->SetValue(channelMask[a]);
	colorWidget->SetValue(brushColor);
	
	invertWidget->SetValue(invertValue);
	stylusWidget->SetValue(stylusValue);
	
	strengthSlider->SetValue(strengthValue);
	radiusSlider->SetValue(radiusValue);
	fuzzySlider->SetValue(fuzzyValue);
	
	UpdateSlider(strengthSlider);
	UpdateSlider(radiusSlider);
	UpdateSlider(fuzzySlider);
	
	menuButton = static_cast<IconButtonWidget *>(FindWidget("Menu"));
	menuButton->SetObserver(&menuButtonObserver);
	
	const StringTable *table = TheWorldEditor->GetStringTable();
	
	MenuItemWidget *widget = new MenuItemWidget(table->GetString(StringID('PAGE', kEditorPagePaint, 'ASSC')), WidgetObserver<PaintPage>(this, &PaintPage::HandleAssociatePaintSpaceMenuItemEvent));
	paintMenuItem[kPaintMenuAssociatePaintSpace] = widget;
	paintMenuItemList.Append(widget);
	
	widget = new MenuItemWidget(table->GetString(StringID('PAGE', kEditorPagePaint, 'DSSC')), WidgetObserver<PaintPage>(this, &PaintPage::HandleDissociatePaintSpaceMenuItemEvent));
	paintMenuItem[kPaintMenuDissociatePaintSpace] = widget;
	paintMenuItemList.Append(widget);
	
	paintMenuItemList.Append(new MenuItemWidget(kLineSolid));
	
	widget = new MenuItemWidget(table->GetString(StringID('PAGE', kEditorPagePaint, 'SPNT')), WidgetObserver<PaintPage>(this, &PaintPage::HandleSelectAssociatedPaintSpacesMenuItemEvent));
	paintMenuItem[kPaintMenuSelectAssociatedPaintSpaces] = widget;
	paintMenuItemList.Append(widget);
	
	widget = new MenuItemWidget(table->GetString(StringID('PAGE', kEditorPagePaint, 'SGEO')), WidgetObserver<PaintPage>(this, &PaintPage::HandleSelectAssociatedGeometriesMenuItemEvent));
	paintMenuItem[kPaintMenuSelectAssociatedGeometries] = widget;
	paintMenuItemList.Append(widget);
}

void PaintPage::UpdateSlider(SliderWidget *widget)
{
	if (widget == strengthSlider)
	{
		int32 strength = (int32) (GetBrushStrength() * 100.0F + 0.5F);
		strengthText->SetText(String<15>(strength) += '%');
	}
	else if (widget == radiusSlider)
	{
		radiusText->SetText(Text::FloatToString(GetBrushRadius()));
	}
	else if (widget == fuzzySlider)
	{
		int32 fuzzy = (int32) (GetBrushFuzziness() * 100.0F + 0.5F);
		fuzzyText->SetText(String<15>(fuzzy) += '%');
	}
}

void PaintPage::UpdateImage(void)
{
	if (painter->UpdateImage())
	{
		targetPaintSpace->GetObject()->GetPaintTexture()->Update(painter->GetPaintBounds());
		GetEditor()->InvalidateFrustumViewports();
	}
}

void PaintPage::HandleEditorEvent(Editor *editor, const EditorEvent& event)
{
	if (event.GetEventType() == kEditorEventGizmoMoved)
	{
		Node *node = static_cast<const NodeEditorEvent *>(&event)->GetEventNode();
		if ((node) && (node->GetNodeType() == kNodeSpace))
		{
			Space *space = static_cast<Space *>(node);
			if (space->GetSpaceType() == kSpacePaint)
			{
				SetTargetPaintSpace(static_cast<PaintSpace *>(space));
			}
		}
	}
}

void PaintPage::HandleToolButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		GetEditor()->SetCurrentTool(this, widget);
	}
}

void PaintPage::HandleChannelButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		IconButtonWidget *iconButtonWidget = static_cast<IconButtonWidget *>(widget);
		for (machine a = 0; a < 4; a++)
		{
			if (channelButton[a] == iconButtonWidget)
			{
				bool mask = (channelMask[a] = !channelMask[a]);
				iconButtonWidget->SetValue(mask);
				break;
			}
		}
	}
}

void PaintPage::HandleColorEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		ColorWidget *colorWidget = static_cast<ColorWidget *>(widget);
		brushColor = colorWidget->GetValue();
	}
}

void PaintPage::HandleCheckEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		CheckWidget *checkWidget = static_cast<CheckWidget *>(widget);
		int32 value = checkWidget->GetValue();
		
		if (checkWidget == invertWidget) invertValue = value;
		else if (checkWidget == stylusWidget) stylusValue = value;
	}
}

void PaintPage::HandleSliderEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		SliderWidget *sliderWidget = static_cast<SliderWidget *>(widget);
		int32 value = sliderWidget->GetValue();
		
		if (sliderWidget == strengthSlider) strengthValue = value;
		else if (sliderWidget == radiusSlider) radiusValue = value;
		else if (sliderWidget == fuzzySlider) fuzzyValue = value;
		
		UpdateSlider(static_cast<SliderWidget *>(widget));
	}
}

void PaintPage::HandleMenuButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetActivate)
	{
		int32 paintSpaceCount = 0;
		int32 geometryCount = 0;
		
		const NodeReference *reference = GetEditor()->GetFirstSelectedNode();
		while (reference)
		{
			const Node *node = reference->GetNode();
			NodeType type = node->GetNodeType();
			if (type == kNodeSpace)
			{
				if (static_cast<const Space *>(node)->GetSpaceType() == kSpacePaint) paintSpaceCount++;
			}
			else if (type == kNodeGeometry)
			{
				geometryCount++;
			}
			
			reference = reference->Next();
		}
		
		if (paintSpaceCount != 0)
		{
			paintMenuItem[kPaintMenuSelectAssociatedGeometries]->Enable();
		}
		else
		{
			paintMenuItem[kPaintMenuSelectAssociatedGeometries]->Disable();
		}
		
		if (geometryCount != 0)
		{
			if (paintSpaceCount == 1) paintMenuItem[kPaintMenuAssociatePaintSpace]->Enable();
			else paintMenuItem[kPaintMenuAssociatePaintSpace]->Disable();
			
			paintMenuItem[kPaintMenuDissociatePaintSpace]->Enable();
			paintMenuItem[kPaintMenuSelectAssociatedPaintSpaces]->Enable();
		}
		else
		{
			paintMenuItem[kPaintMenuAssociatePaintSpace]->Disable();
			paintMenuItem[kPaintMenuDissociatePaintSpace]->Disable();
			paintMenuItem[kPaintMenuSelectAssociatedPaintSpaces]->Disable();
		}
		
		Menu *menu = new Menu(kMenuContextual, &paintMenuItemList);
		menu->SetWidgetPosition(menuButton->GetWorldPosition() + Vector3D(25.0F, 0.0F, 0.0F));
		TheInterfaceMgr->SetActiveMenu(menu);
	}
}

void PaintPage::HandleAssociatePaintSpaceMenuItemEvent(Widget *menuItem, const WidgetEventData *eventData)
{
	Editor *editor = GetEditor();
	
	PaintSpace *paintSpace = nullptr;
	const NodeReference *reference = editor->GetFirstSelectedNode();
	while (reference)
	{
		Node *node = reference->GetNode();
		if (node->GetNodeType() == kNodeSpace)
		{
			Space *space = static_cast<Space *>(node);
			if (space->GetSpaceType() == kSpacePaint)
			{
				paintSpace = static_cast<PaintSpace *>(space);
				break;
			}
		}
		
		reference = reference->Next();
	}
	
	if (paintSpace)
	{
		UndoData *undoData = new AssociatePaintSpaceUndoData(editor->GetSelectionList());
		editor->AddUndoData(undoData);
		
		reference = editor->GetFirstSelectedNode();
		while (reference)
		{
			Node *node = reference->GetNode();
			if (node->GetNodeType() == kNodeGeometry)
			{
				Geometry *geometry = static_cast<Geometry *>(node);
				geometry->SetConnectedPaintSpace(paintSpace);
				geometry->InvalidateShaderData();
				
				Editor::GetManipulator(geometry)->UpdateConnectors();
			}
			
			reference = reference->Next();
		}
	}
}

void PaintPage::HandleDissociatePaintSpaceMenuItemEvent(Widget *menuItem, const WidgetEventData *eventData)
{
	Editor *editor = GetEditor();
	
	UndoData *undoData = new AssociatePaintSpaceUndoData(editor->GetSelectionList());
	editor->AddUndoData(undoData);
	
	const NodeReference *reference = editor->GetFirstSelectedNode();
	while (reference)
	{
		Node *node = reference->GetNode();
		if (node->GetNodeType() == kNodeGeometry)
		{
			Geometry *geometry = static_cast<Geometry *>(node);
			geometry->SetConnectedPaintSpace(nullptr);
			geometry->InvalidateShaderData();
			
			Editor::GetManipulator(geometry)->UpdateConnectors();
		}
		
		reference = reference->Next();
	}
}

void PaintPage::HandleSelectAssociatedPaintSpacesMenuItemEvent(Widget *menuItem, const WidgetEventData *eventData)
{
	List<NodeReference>		paintSpaceList;
	
	Editor *editor = GetEditor();
	
	const NodeReference *reference = editor->GetFirstSelectedNode();
	while (reference)
	{
		const Node *node = reference->GetNode();
		if (node->GetNodeType() == kNodeGeometry)
		{
			PaintSpace *paintSpace = static_cast<const Geometry *>(node)->GetConnectedPaintSpace();
			if (paintSpace)
			{
				const NodeReference *paintSpaceReference = paintSpaceList.First();
				while (paintSpaceReference)
				{
					if (paintSpaceReference->GetNode() == paintSpace) goto next;
					paintSpaceReference = paintSpaceReference->Next();
				}
				
				paintSpaceList.Append(new NodeReference(paintSpace));
			}
		}
		
		next:
		reference = reference->Next();
	}
	
	editor->UnselectAll();
	
	reference = paintSpaceList.First();
	while (reference)
	{
		editor->SelectNode(reference->GetNode());
		reference = reference->Next();
	}
}

void PaintPage::HandleSelectAssociatedGeometriesMenuItemEvent(Widget *menuItem, const WidgetEventData *eventData)
{
	List<NodeReference>		geometryList;
	
	Editor *editor = GetEditor();
	
	const NodeReference *reference = editor->GetFirstSelectedNode();
	while (reference)
	{
		const Node *node = reference->GetNode();
		if (node->GetNodeType() == kNodeSpace)
		{
			const Space *space = static_cast<const Space *>(node);
			if (space->GetSpaceType() == kSpacePaint)
			{
				const PaintSpace *paintSpace = static_cast<const PaintSpace *>(space);
				
				const Hub *hub = space->GetHub();
				if (hub)
				{
					const Connector *connector = hub->GetFirstIncomingEdge();
					while (connector)
					{
						Node *start = connector->GetStartElement()->GetNode();
						if (start->GetNodeType() == kNodeGeometry)
						{
							Geometry *geometry = static_cast<Geometry *>(start);
							if (geometry->GetConnectedPaintSpace() == paintSpace)
							{
								const NodeReference *geometryReference = geometryList.First();
								while (geometryReference)
								{
									if (geometryReference->GetNode() == geometry) goto next;
									geometryReference = geometryReference->Next();
								}
								
								geometryList.Append(new NodeReference(geometry));
							}
						}
						
						next:
						connector = connector->GetNextIncomingEdge();
					}
				}
			}
		}
		
		reference = reference->Next();
	}
	
	editor->UnselectAll();
	
	reference = geometryList.First();
	while (reference)
	{
		editor->SelectNode(reference->GetNode());
		reference = reference->Next();
	}
}

void PaintPage::Engage(Editor *editor, void *cookie)
{
	Widget *widget = static_cast<Widget *>(cookie);
	
	for (machine a = 0; a < kEditorPaintToolCount; a++)
	{
		if (widget == toolButton[a])
		{
			currentTool = a;
			toolButton[a]->SetValue(1);
			break;
		}
	}
	
	editor->SetCurrentCursor(TheWorldEditor->GetEditorCursor(kEditorCursorCross));
}

void PaintPage::Disengage(Editor *editor, void *cookie)
{
	if (currentTool != -1)
	{
		toolButton[currentTool]->SetValue(0);
		currentTool = -1;
	}
}

bool PaintPage::PaintPickFilter(const Node *node, const PickData *pickData, void *cookie)
{
	if ((!Editor::GetManipulator(node)->Hidden()) && (node->GetNodeType() == kNodeGeometry) && (pickData->triangleIndex != kInvalidTriangleIndex))
	{
		const Geometry *geometry = static_cast<const Geometry *>(node);
		const PaintSpace *paintSpace = static_cast<PaintPage *>(cookie)->GetTargetPaintSpace();
		if ((!paintSpace) || (geometry->GetConnectedPaintSpace() == paintSpace))
		{
			const MaterialObject *materialObject = geometry->GetTriangleMaterial(pickData->triangleIndex);
			if (materialObject)
			{
				const Attribute *attribute = materialObject->GetFirstAttribute();
				while (attribute)
				{
					AttributeType type = attribute->GetAttributeType();
					if (type == kAttributePaint)
					{
						return (true);
					}
					else if (type == kAttributeShader)
					{
						const ShaderAttribute *shaderAttribute = static_cast<const ShaderAttribute *>(attribute);
						for (machine a = 0; a < kShaderGraphCount; a++)
						{
							const ShaderGraph *graph = shaderAttribute->GetShaderGraph(a);
							const Process *process = graph->GetFirstElement();
							while (process)
							{
								if (process->GetProcessType() == kProcessPaintTexture) return (true);
								process = process->GetNextElement();
							}
						}
					}
					
					attribute = attribute->Next();
				}
			}
		}
	}
	
	return (false);
}

bool PaintPage::BeginTool(Editor *editor, EditorTrackData *trackData)
{
	SetTargetPaintSpace(nullptr);
	
	if (editor->SetTrackPickFilter(trackData, &PaintPickFilter, this))
	{
		const Geometry *geometry = static_cast<const Geometry *>(trackData->currentPickNode);
		PaintSpace *paintSpace = geometry->GetConnectedPaintSpace();
		if (paintSpace)
		{
			SetTargetPaintSpace(paintSpace);
			
			paintState.SetChannelMask(channelMask[0], channelMask[1], channelMask[2], channelMask[3]);
			
			if (invertWidget->GetValue() == 0)
			{
				paintState.SetBrushColor(brushColor);
			}
			else
			{
				float red = 1.0F - brushColor.red;
				float green = 1.0F - brushColor.green;
				float blue = 1.0F - brushColor.blue;
				float alpha = 1.0F - brushColor.alpha;
				paintState.SetBrushColor(ColorRGBA(red, green, blue, alpha));
			}
			
			paintState.SetBrushRadius(GetBrushRadius());
			paintState.SetBrushFuzziness(GetBrushFuzziness());
			
			float strength = GetBrushStrength();
			if (stylusWidget->GetValue() != 0) strength *= TheEngine->GetStylusPressure();
			paintState.SetBrushOpacity(strength);
			
			const PaintSpaceObject *object = paintSpace->GetObject();
			const Integer2D& resolution = object->GetPaintResolution();
			painter = new Painter(resolution, object->GetChannelCount(), object->GetPaintImage(), &paintState);
			
			previousPosition = (paintSpace->GetPaintEnvironment()->paintTransform * trackData->currentPickPoint).GetPoint2D();
			previousPosition.x *= (float) resolution.x;
			previousPosition.y *= (float) resolution.y;
			
			painter->BeginPainting();
			painter->DrawDot(previousPosition);
			
			UpdateImage();
			return (true);
		}
	}
	
	return (false);
}

bool PaintPage::TrackTool(Editor *editor, EditorTrackData *trackData)
{
	editor->AutoScroll(trackData);
	
	if (trackData->currentPickNode)
	{
		Point2D position = (targetPaintSpace->GetPaintEnvironment()->paintTransform * trackData->currentPickPoint).GetPoint2D();
		const Integer2D& resolution = targetPaintSpace->GetObject()->GetPaintResolution();
		position.x *= (float) resolution.x;
		position.y *= (float) resolution.y;
		
		if (SquaredMag(position - previousPosition) > K::min_float)
		{
			if (stylusWidget->GetValue() != 0)
			{
				float strength = GetBrushStrength() * TheEngine->GetStylusPressure();
				paintState.SetBrushOpacity(strength);
			}
			
			painter->DrawLine(previousPosition, position);
			previousPosition = position;
			UpdateImage();
		}
	}
	
	return (true);
}

bool PaintPage::EndTool(Editor *editor, EditorTrackData *trackData)
{
	bool result = TrackTool(editor, trackData);
	
	painter->EndPainting();
	editor->AddUndoData(new PaintUndoData(targetPaintSpace->GetObject(), painter));
	
	delete painter;
	return (result);
}

void PaintPage::SetTargetPaintSpace(PaintSpace *paintSpace)
{
	targetPaintSpace = paintSpace;
	
	if (paintSpace)
	{
		imageWidget->SetTexture(0, paintSpace->GetObject()->GetPaintTexture());
		imageWidget->Show();
	}
	else
	{
		Texture *texture = nullptr;
		imageWidget->SetTexture(0, texture);
		imageWidget->Hide();
	}
}

// ZYURVUR
