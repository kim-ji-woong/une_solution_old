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


#include "C4World.h"
#include "C4WaterTools.h"
#include "C4EditorSupport.h"


using namespace C4;


namespace
{
	const ConstColorRGBA kBlockSelectedColor = {0.1875F, 0.625F, 0.75F, 1.0F};
	const ConstColorRGBA kBlockUnselectedColor = {0.09375F, 0.3125F, 0.375F, 1.0F};
}


ResourceDescriptor WaterResource::descriptor("wat");


WaterResource::WaterResource(const char *name, ResourceCatalog *catalog) : Resource<WaterResource>(name, catalog)
{
}

WaterResource::~WaterResource()
{
}

void WaterResource::Preprocess(void)
{
	int32 *data = static_cast<int32 *>(GetData());
	if (data[0] != 1)
	{
		Reverse(&data[1]);
		Reverse(&data[2]);
		
		int32 blockCount = data[2];
		data += 3;
		
		for (machine a = 0; a < blockCount; a++)
		{
			data += 16;
			
			Reverse(&data[0]);
			int32 waterCount = data[0];
			data++;
			
			for (machine b = 0; b < waterCount; b++)
			{
				Reverse(&data[0]);
				Reverse(&data[1]);
				Reverse(&data[2]);
				
				int32 vertexCount = data[2] * 2;
				data += 3;
				
				for (machine c = 0; c < vertexCount; c++) Reverse(&data[c]);
				data += vertexCount;
			}
		}
	}
}


WaterBlockManipulator::WaterBlockManipulator(WaterBlock *block) :
		EditorManipulator(block, "WorldEditor/node/WaterBlock"),
		blockDiffuseColor(kBlockUnselectedColor, kAttributeMutable),
		blockRenderable(kRenderIndexedTriangles, kRenderDepthTest)
{
	SetManipulatorFlags(kManipulatorLockedController);
	
	blockAttributeList.Append(&blockDiffuseColor);
	blockRenderable.SetMaterialAttributeList(&blockAttributeList);
	blockRenderable.SetShaderFlags(kShaderAmbientEffect | kShaderScaleVertex | kShaderOffsetVertex);
	blockRenderable.SetRenderParameterPointer(&blockSizeVector);
	blockRenderable.SetTransformable(block);
	blockRenderable.SetVertexCount(kManipulatorBoxVertexCount);
	blockRenderable.SetAttributeArray(kArrayVertex, &manipulatorBoxVertex[0]);
	blockRenderable.SetAttributeArray(kArrayOffset, &manipulatorBoxOffset[0]);
	blockRenderable.SetTriangleArray(kManipulatorBoxTriangleCount, manipulatorBoxTriangle);
}

WaterBlockManipulator::~WaterBlockManipulator()
{
}

const char *WaterBlockManipulator::GetDefaultNodeName(void) const
{
	return (TheWorldEditor->GetStringTable()->GetString(StringID('NAME', kNodeWaterBlock)));
}

int32 WaterBlockManipulator::GetHandleTable(Point3D *handle) const
{
	handle[0] = Zero3D + Vector3D(GetTargetNode()->GetBlockBoxSize(), 0.0F);
	return (1);
}

void WaterBlockManipulator::GetHandleData(int32 index, ManipulatorHandleData *handleData) const
{
	handleData->handleFlags = kManipulatorHandlePositiveX | kManipulatorHandlePositiveY | kManipulatorHandlePositiveZ;
	handleData->oppositeIndex = kHandleOrigin;
}

void WaterBlockManipulator::Select(void) 
{
	EditorManipulator::Select(); 
	blockDiffuseColor.SetDiffuseColor(kBlockSelectedColor); 
} 

void WaterBlockManipulator::Unselect(void) 
{
	EditorManipulator::Unselect();
	blockDiffuseColor.SetDiffuseColor(kBlockUnselectedColor);
} 

bool WaterBlockManipulator::CalculateNodeSphere(BoundingSphere *sphere) const
{
	const WaterBlock *block = GetTargetNode(); 
	Vector2D boxSize = block->GetBlockBoxSize() * 0.5F;
	
	sphere->SetCenter(Zero3D + Vector3D(boxSize, 0.0F));
	sphere->SetRadius(Magnitude(boxSize));
	return (true);
}

Box3D WaterBlockManipulator::CalculateNodeBoundingBox(void) const
{
	const WaterBlock *block = GetTargetNode();
	const Vector3D& boxSize = block->GetBlockBoxSize();
	return (Box3D(Zero3D, Zero3D + boxSize));
}

bool WaterBlockManipulator::Pick(const Ray *ray, PickData *data) const
{
	float r = (ray->radius != 0.0F) ? ray->radius : Editor::kFrustumRenderScale;
	float r2 = r * r * 16.0F;
	
	Vector3D boxSize = GetTargetNode()->GetBlockBoxSize();
	
	if (PickLineSegment(ray, Zero3D, Point3D(boxSize.x, 0.0F, 0.0F), r2, &data->rayParam)) return (true);
	if (PickLineSegment(ray, Point3D(boxSize.x, 0.0F, 0.0F), Point3D(boxSize.x, boxSize.y, 0.0F), r2, &data->rayParam)) return (true);
	if (PickLineSegment(ray, Point3D(boxSize.x, boxSize.y, 0.0F), Point3D(0.0F, boxSize.y, 0.0F), r2, &data->rayParam)) return (true);
	if (PickLineSegment(ray, Point3D(0.0F, boxSize.y, 0.0F), Zero3D, r2, &data->rayParam)) return (true);
	if (PickLineSegment(ray, Point3D(0.0F, 0.0F, boxSize.z), Point3D(boxSize.x, 0.0F, boxSize.z), r2, &data->rayParam)) return (true);
	if (PickLineSegment(ray, Point3D(boxSize.x, 0.0F, boxSize.z), Zero3D + boxSize, r2, &data->rayParam)) return (true);
	if (PickLineSegment(ray, Zero3D + boxSize, Point3D(0.0F, boxSize.y, boxSize.z), r2, &data->rayParam)) return (true);
	if (PickLineSegment(ray, Point3D(0.0F, boxSize.y, boxSize.z), Point3D(0.0F, 0.0F, boxSize.z), r2, &data->rayParam)) return (true);
	if (PickLineSegment(ray, Zero3D, Point3D(0.0F, 0.0F, boxSize.z), r2, &data->rayParam)) return (true);
	if (PickLineSegment(ray, Point3D(boxSize.x, 0.0F, 0.0F), Point3D(boxSize.x, 0.0F, boxSize.z), r2, &data->rayParam)) return (true);
	if (PickLineSegment(ray, Point3D(boxSize.x, boxSize.y, 0.0F), Zero3D + boxSize, r2, &data->rayParam)) return (true);
	if (PickLineSegment(ray, Point3D(0.0F, boxSize.y, 0.0F), Point3D(0.0F, boxSize.y, boxSize.z), r2, &data->rayParam)) return (true);
	
	return (false);
}

bool WaterBlockManipulator::RegionPick(const Region *region) const
{
	Vector3D boxSize = GetTargetNode()->GetBlockBoxSize();
	
	if (RegionPickLineSegment(region, Zero3D, Point3D(boxSize.x, 0.0F, 0.0F))) return (true);
	if (RegionPickLineSegment(region, Point3D(boxSize.x, 0.0F, 0.0F), Point3D(boxSize.x, boxSize.y, 0.0F))) return (true);
	if (RegionPickLineSegment(region, Point3D(boxSize.x, boxSize.y, 0.0F), Point3D(0.0F, boxSize.y, 0.0F))) return (true);
	if (RegionPickLineSegment(region, Point3D(0.0F, boxSize.y, 0.0F), Zero3D)) return (true);
	if (RegionPickLineSegment(region, Point3D(0.0F, 0.0F, boxSize.z), Point3D(boxSize.x, 0.0F, boxSize.z))) return (true);
	if (RegionPickLineSegment(region, Point3D(boxSize.x, 0.0F, boxSize.z), Zero3D + boxSize)) return (true);
	if (RegionPickLineSegment(region, Zero3D + boxSize, Point3D(0.0F, boxSize.y, boxSize.z))) return (true);
	if (RegionPickLineSegment(region, Point3D(0.0F, boxSize.y, boxSize.z), Point3D(0.0F, 0.0F, boxSize.z))) return (true);
	if (RegionPickLineSegment(region, Zero3D, Point3D(0.0F, 0.0F, boxSize.z))) return (true);
	if (RegionPickLineSegment(region, Point3D(boxSize.x, 0.0F, 0.0F), Point3D(boxSize.x, 0.0F, boxSize.z))) return (true);
	if (RegionPickLineSegment(region, Point3D(boxSize.x, boxSize.y, 0.0F), Zero3D + boxSize)) return (true);
	if (RegionPickLineSegment(region, Point3D(0.0F, boxSize.y, 0.0F), Point3D(0.0F, boxSize.y, boxSize.z))) return (true);
	
	return (false);
}

bool WaterBlockManipulator::Resize(const ManipulatorResizeData *resizeData)
{
	float	newScale;
	
	WaterBlock *block = GetTargetNode();
	
	float oldScale = GetOriginalSize()[0];
	const Integer2D& size = block->GetBlockSize();
	int32 dimension = block->GetObject()->GetWaterFieldDimension();
	Vector2D oldSize((float) (size.x * dimension) * oldScale, (float) (size.y * dimension) * oldScale);
	
	const Vector3D& delta = resizeData->resizeDelta;
	Vector2D newSize(Fmax(oldSize.x + delta.x, kSizeEpsilon), Fmax(oldSize.y + delta.y, kSizeEpsilon));
	
	float ax = Fabs(newSize.x - oldSize.x);
	float ay = Fabs(newSize.y - oldSize.y);
	
	if (ax >= ay) newScale = oldScale * (newSize.x / oldSize.x);
	else newScale = oldScale * (newSize.y / oldSize.y);
	
	if (newScale != oldScale) {}//RescaleBlock(newScale);	// [HACK]
	return (false);
}

void WaterBlockManipulator::Render(const ManipulatorRenderData *renderData)
{
	List<Renderable> *renderList = renderData->manipulatorList;
	if (renderList)
	{
		const WaterBlock *block = GetTargetNode();
		const Vector2D& size = block->GetBlockBoxSize();
		blockSizeVector.Set(size.x, size.y, 0.0F, renderData->viewportScale * 2.0F);
		renderList->Append(&blockRenderable);
	}
	
	EditorManipulator::Render(renderData);
}


WaterGeometryManipulator::WaterGeometryManipulator(WaterGeometry *water) : GeometryManipulator(water)
{
	SetManipulatorFlags(kManipulatorLockedTransform | kManipulatorLockedController);
}

WaterGeometryManipulator::~WaterGeometryManipulator()
{
}

const char *WaterGeometryManipulator::GetDefaultNodeName(void) const
{
	return (TheWorldEditor->GetStringTable()->GetString(StringID('NAME', kGeometryWater)));
}


HorizonWaterGeometryManipulator::HorizonWaterGeometryManipulator(HorizonWaterGeometry *water) : GeometryManipulator(water)
{
	SetManipulatorFlags(kManipulatorLockedTransform | kManipulatorLockedController);
}

HorizonWaterGeometryManipulator::~HorizonWaterGeometryManipulator()
{
}

const char *HorizonWaterGeometryManipulator::GetDefaultNodeName(void) const
{
	return (TheWorldEditor->GetStringTable()->GetString(StringID('NAME', kGeometryHorizonWater)));
}


WaterPage::WaterPage() :
		EditorPage(kEditorPageWater, "WorldEditor/water/Water"),
		waterButtonObserver(this, &WaterPage::HandleWaterButtonEvent),
		menuButtonObserver(this, &WaterPage::HandleMenuButtonEvent)
{
	currentTool = -1;
}

WaterPage::~WaterPage()
{
}

void WaterPage::Preprocess(void)
{
	EditorPage::Preprocess();
	
	waterButton = static_cast<IconButtonWidget *>(FindWidget("Block"));
	waterButton->SetObserver(&waterButtonObserver);
	
	menuButton = static_cast<IconButtonWidget *>(FindWidget("Menu"));
	menuButton->SetObserver(&menuButtonObserver);
	
	const StringTable *table = TheWorldEditor->GetStringTable();
	
	MenuItemWidget *widget = new MenuItemWidget(table->GetString(StringID('PAGE', kEditorPageWater, 'GLND')), WidgetObserver<WaterPage>(this, &WaterPage::HandleGenerateLandHeightMenuItemEvent));
	waterMenuItem[kWaterMenuGenerateLandHeight] = widget;
	waterMenuItemList.Append(widget);
	
	widget = new MenuItemWidget(table->GetString(StringID('PAGE', kEditorPageWater, 'RLND')), WidgetObserver<WaterPage>(this, &WaterPage::HandleRemoveLandHeightMenuItemEvent));
	waterMenuItem[kWaterMenuRemoveLandHeight] = widget;
	waterMenuItemList.Append(widget);
	
	waterMenuItemList.Append(new MenuItemWidget(kLineSolid));
	
	widget = new MenuItemWidget(table->GetString(StringID('PAGE', kEditorPageWater, 'RBLD')), WidgetObserver<WaterPage>(this, &WaterPage::HandleRebuildWaterBlockMenuItemEvent));
	waterMenuItem[kWaterMenuRebuildWaterBlock] = widget;
	waterMenuItemList.Append(widget);
	
	waterMenuItemList.Append(new MenuItemWidget(kLineSolid));
	
	widget = new MenuItemWidget(table->GetString(StringID('PAGE', kEditorPageWater, 'IWAV')), WidgetObserver<WaterPage>(this, &WaterPage::HandleImportWaveDataMenuItemEvent));
	waterMenuItem[kWaterMenuImportWaveData] = widget;
	waterMenuItemList.Append(widget);
	
	widget = new MenuItemWidget(table->GetString(StringID('PAGE', kEditorPageWater, 'RWAV')), WidgetObserver<WaterPage>(this, &WaterPage::HandleRemoveWaveDataMenuItemEvent));
	waterMenuItem[kWaterMenuRemoveWaveData] = widget;
	waterMenuItemList.Append(widget);
}

void WaterPage::HandleWaterButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetChange)
	{
		GetEditor()->SetCurrentTool(this, widget);
	}
}

void WaterPage::HandleMenuButtonEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetActivate)
	{
		int32 geometryCount = 0;
		int32 blockCount = 0;
		
		const NodeReference *reference = GetEditor()->GetFirstSelectedNode();
		while (reference)
		{
			const Node *node = reference->GetNode();
			NodeType type = node->GetNodeType();
			
			if (type == kNodeGeometry)
			{
				const Geometry *geometry = static_cast<const Geometry *>(node);
				geometryCount += GenerateLandElevationWindow::ValidGeometry(geometry);
			}
			else if (type == kNodeWaterBlock)
			{
				if (node->GetFirstSubnode()) blockCount++;
			}
			
			reference = reference->Next();
		}
		
		if (geometryCount != 0)
		{
			waterMenuItem[kWaterMenuGenerateLandHeight]->Enable();
			waterMenuItem[kWaterMenuRemoveLandHeight]->Enable();
		}
		else
		{
			waterMenuItem[kWaterMenuGenerateLandHeight]->Disable();
			waterMenuItem[kWaterMenuRemoveLandHeight]->Disable();
		}
		
		if (blockCount == 1) waterMenuItem[kWaterMenuRebuildWaterBlock]->Enable();
		else waterMenuItem[kWaterMenuRebuildWaterBlock]->Disable();
		
		Menu *menu = new Menu(kMenuContextual, &waterMenuItemList);
		menu->SetWidgetPosition(menuButton->GetWorldPosition() + Vector3D(25.0F, 0.0F, 0.0F));
		TheInterfaceMgr->SetActiveMenu(menu);
	}
}

void WaterPage::HandleGenerateLandHeightMenuItemEvent(Widget *menuItem, const WidgetEventData *eventData)
{
	Editor *editor = GetEditor();
	editor->AddSubwindow(new GenerateLandElevationWindow(editor));
}

void WaterPage::HandleRemoveLandHeightMenuItemEvent(Widget *menuItem, const WidgetEventData *eventData)
{
	Editor *editor = GetEditor();
	editor->AddUndoData(new GeometryUndoData(editor->GetSelectionList(), kGeometryWater));
	
	const NodeReference *reference = editor->GetFirstSelectedNode();
	while (reference)
	{
		Node *node = reference->GetNode();
		if (node->GetNodeType() == kNodeGeometry)
		{
			Geometry *geometry = static_cast<Geometry *>(node);
			if (geometry->GetGeometryType() == kGeometryWater)
			{
				const GeometryObject *object = geometry->GetObject();
				
				int32 levelCount = object->GetGeometryLevelCount();
				for (machine level = 0; level < levelCount; level++)
				{
					GeometryLevel *geometryLevel = object->GetGeometryLevel(level);
					Point3D *land = geometryLevel->GetArray<Point3D>(kArrayPosition1);
					if (land)
					{
						ArrayDescriptor		desc;
						GeometryLevel		tempLevel;
						
						tempLevel.CopyGeometryLevel(geometryLevel);
						
						desc.identifier = kArrayPosition1;
						desc.elementCount = 0;
						desc.elementSize = sizeof(Point3D);
						desc.componentCount = 3;
						
						geometryLevel->AllocateStorage(&tempLevel, 1, &desc);
						geometry->SetAttributeArray(kArrayPosition1, (float *) nullptr);
						
						editor->InvalidateGeometry(geometry);
					}
				}
			}
		}
		
		reference = reference->Next();
	}
}

void WaterPage::HandleRebuildWaterBlockMenuItemEvent(Widget *menuItem, const WidgetEventData *eventData)
{
	const NodeReference *reference = GetEditor()->GetFirstSelectedNode();
	while (reference)
	{
		Node *node = reference->GetNode();
		if ((node->GetNodeType() == kNodeWaterBlock) && (node->GetFirstSubnode()))
		{
			WaterBlock *block = static_cast<WaterBlock *>(node);
			GetEditor()->AddSubwindow(new BuildWaterWindow(GetEditor(), block));
			break;
		}
		
		reference = reference->Next();
	}
}

void WaterPage::HandleImportWaveDataMenuItemEvent(Widget *menuItem, const WidgetEventData *eventData)
{
	const char *title = TheWorldEditor->GetStringTable()->GetString(StringID('PAGE', kEditorPageWater, 'PICK'));
	FilePicker *picker = new FilePicker('IWAV', title, TheResourceMgr->GetGenericCatalog(), WaterResource::GetDescriptor());
	picker->SetCompletionProc(&WaterPicked, this);
	GetEditor()->AddSubwindow(picker);
}

void WaterPage::WaterPicked(FilePicker *picker, void *cookie)
{
	WaterResource *resource = WaterResource::Get(picker->GetResourceName());
	if (resource)
	{
		const WaterPage *waterPage = static_cast<WaterPage *>(cookie);
		Editor *editor = waterPage->GetEditor();
		editor->SetWorldUnsavedFlag();
		editor->InvalidateAllViewports();
		
		const int32 *waveData = static_cast<int32 *>(resource->GetData());
		int32 blockCount = waveData[2];
		waveData += 3;
		
		for (machine a = 0; a < blockCount; a++)
		{
			const char *name = reinterpret_cast<const char *>(waveData);
			const Node *node = editor->FindNode(name);
			const WaterBlock *block = ((node) && (node->GetNodeType() == kNodeWaterBlock)) ? static_cast<const WaterBlock *>(node) : nullptr;
			
			int32 waterCount = waveData[16];
			waveData += 17;
			
			for (machine b = 0; b < waterCount; b++)
			{
				Integer2D coord(waveData[0], waveData[1]);
				int32 vertexCount = waveData[2];
				waveData += 3;
				
				if (block)
				{
					WaterGeometry *water = block->GetWaterGeometry(coord.x, coord.y);
					if ((water) && (water->GetWaterVertexCount() == vertexCount))
					{
						const float		*elevation[2];
						
						elevation[0] = reinterpret_cast<const float *>(waveData);
						elevation[1] = elevation[0] + vertexCount;
						water->LoadWaterElevation(elevation);
					}
				}
				
				waveData += vertexCount * 2;
			}
		}
		
		resource->Release();
	}
}

void WaterPage::HandleRemoveWaveDataMenuItemEvent(Widget *menuItem, const WidgetEventData *eventData)
{
	Editor *editor = GetEditor();
	editor->SetWorldUnsavedFlag();
	editor->InvalidateAllViewports();
	
	Node *root = editor->GetRootNode();
	Node *node = root->GetFirstSubnode();
	while (node)
	{
		if (node->GetNodeType() == kNodeWaterBlock)
		{
			Node *subnode = node->GetFirstSubnode();
			while (subnode)
			{
				if (subnode->GetNodeType() == kNodeGeometry)
				{
					Geometry *geometry = static_cast<Geometry *>(subnode);
					if (geometry->GetGeometryType() == kGeometryWater)
					{
						WaterGeometry *water = static_cast<WaterGeometry *>(geometry);
						water->ClearWaterElevation();
					}
				}
				
				subnode = subnode->Next();
			}
			
			node = root->GetNextLevelNode(node);
			continue;
		}
		
		node = root->GetNextNode(node);
	}
}

void WaterPage::Engage(Editor *editor, void *cookie)
{
	Widget *widget = static_cast<Widget *>(cookie);
	
	if (widget == waterButton)
	{
		currentTool = 0;
		waterButton->SetValue(1);
	}
	
	editor->SetCurrentCursor(TheWorldEditor->GetEditorCursor(kEditorCursorCross));
}

void WaterPage::Disengage(Editor *editor, void *cookie)
{
	if (currentTool != -1)
	{
		waterButton->SetValue(0);
		currentTool = -1;
	}
}

bool WaterPage::BeginTool(Editor *editor, EditorTrackData *trackData)
{
	if (currentTool == 0)
	{
		if (trackData->viewportType == kEditorViewportOrtho)
		{
			WaterBlock *block = new WaterBlock(Integer2D(4, 4), 32, 0.0F, 16384.0F, Range<float>(-16.0F, 2.0F));
			editor->InitNewNode(trackData, block);
			return (true);
		}
	}
	
	return (false);
}

bool WaterPage::TrackTool(Editor *editor, EditorTrackData *trackData)
{
	editor->AutoScroll(trackData);
	
	if (currentTool == 0)
	{
		Point2D anchor = trackData->snappedAnchorPosition;
		float dx = trackData->snappedCurrentPosition.x - anchor.x;
		float dy = anchor.y - trackData->snappedCurrentPosition.y;
		
		float ax = Fabs(dx);
		float ay = Fabs(dy);
		
		ax = ay = Fmax(ax, ay);
		dx = (dx < 0.0F) ? -ax : ax;
		dy = (dy < 0.0F) ? -ax : ax;
		
		if (dx != trackData->currentSize.x)
		{
			trackData->currentSize.Set(dx, dy);
			
			unsigned_int32 editorFlags = editor->GetEditorObject()->GetEditorFlags();
			if (editorFlags & kEditorDrawFromCenter)
			{
				anchor.x -= ax;
				anchor.y += ay;
				dx = ax * 2.0F;
			}
			else
			{
				if (dx < 0.0F)
				{
					anchor.x += dx;
					dx = -dx;
				}
				
				if (dy < 0.0F) anchor.y -= dy;
			}
			
			WaterBlock *block = static_cast<WaterBlock *>(trackData->trackNode);
			block->SetNodePosition(editor->GetTargetSpacePosition(trackData, anchor));
			
			WaterBlockObject *object = block->GetObject();
			object->SetWaterFieldScale(dx / (float) (object->GetWaterFieldDimension() * block->GetBlockSize().x));
			
			editor->InvalidateNode(block);
		}
		
		return (dx != 0.0F);
	}
	
	return (true);
}

bool WaterPage::EndTool(Editor *editor, EditorTrackData *trackData)
{
	bool result = TrackTool(editor, trackData);
	if (currentTool == 0)
	{
		editor->CommitNewNode(trackData, result);
		if (result) editor->AddSubwindow(new BuildWaterWindow(editor, static_cast<WaterBlock *>(trackData->trackNode)));
	}
	
	return (true);
}

void WaterPage::ExportWater(const World *world, const ResourceName& resourceName)
{
	List<NodeReference>		blockList;
	
	int32 blockCount = 0;
	Node *root = world->GetRootNode();
	
	Node *node = root->GetFirstSubnode();
	while (node)
	{
		if (node->GetNodeType() == kNodeWaterBlock)
		{
			const Controller *controller = node->GetController();
			if ((controller) && (controller->GetControllerType() == kControllerWater))
			{
				const char *name = node->GetNodeName();
				if (name)
				{
					blockList.Append(new NodeReference(node));
					blockCount++;
				}
			}
			
			node = root->GetNextLevelNode(node);
			continue;
		}
		
		node = root->GetNextNode(node);
	}
	
	if (blockCount != 0)
	{
		ResourcePath	path;
		File			waterFile;
		
		const NodeReference *reference = blockList.First();
		do
		{
			const WaterBlock *block = static_cast<WaterBlock *>(reference->GetNode());
			WaterController *controller = static_cast<WaterController *>(block->GetController());
			
			Node *node = block->GetFirstSubnode();
			while (node)
			{
				if (node->GetNodeType() == kNodeGeometry)
				{
					Geometry *geometry = static_cast<Geometry *>(node);
					if (geometry->GetGeometryType() == kGeometryWater)
					{
						WaterGeometry *water = static_cast<WaterGeometry *>(geometry);
						controller->SetDecayingWaterState(water);
					}
				}
				
				node = node->Next();
			}
			
			do
			{
				controller->Advance(0);
			} while (controller->GetFirstDecayingWaterGeometry());
			
			reference = reference->Next();
		} while (reference);
		
		TheResourceMgr->GetGenericCatalog()->GetResourcePath(WaterResource::GetDescriptor(), resourceName, world->GetResourceLocation(), &path);
		TheResourceMgr->CreateDirectoryPath(path);
		
		if (waterFile.Open(path, kFileCreate) == kFileOkay)
		{
			int32 endian = 1;
			int32 version = 1;
			
			waterFile.Write(&endian, 4);
			waterFile.Write(&version, 4);
			waterFile.Write(&blockCount, 4);
			
			const NodeReference *reference = blockList.First();
			do
			{
				const WaterBlock *block = static_cast<WaterBlock *>(reference->GetNode());
				
				String<63> name(block->GetNodeName());
				for (machine a = name.Length() + 1; a < 64; a++) name[a] = 0;
				waterFile.Write(&name, 64);
				
				int32 waterCount = 0;
				const Node *node = block->GetFirstSubnode();
				while (node)
				{
					if ((node->GetNodeType() == kNodeGeometry) && (static_cast<const Geometry *>(node)->GetGeometryType() == kGeometryWater)) waterCount++;
					node = node->Next();
				}
				
				waterFile.Write(&waterCount, 4);
				
				const WaterController *controller = static_cast<WaterController *>(block->GetController());
				unsigned_int32 parity = controller->GetMoveParity();
				
				node = block->GetFirstSubnode();
				while (node)
				{
					if (node->GetNodeType() == kNodeGeometry)
					{
						const Geometry *geometry = static_cast<const Geometry *>(node);
						if (geometry->GetGeometryType() == kGeometryWater)
						{
							const WaterGeometry *water = static_cast<const WaterGeometry *>(geometry);
							
							const Integer2D& coord = water->GetObject()->GetGeometryCoord();
							waterFile.Write(&coord, sizeof(Integer2D));
							
							int32 vertexCount = water->GetWaterVertexCount();
							waterFile.Write(&vertexCount, 4);
							
							float *const (& elevation)[2] = water->GetWaterElevation();
							waterFile.Write(elevation[parity], vertexCount * 4);
							waterFile.Write(elevation[parity ^ 1], vertexCount * 4);
						}
					}
					
					node = node->Next();
				}
				
				reference = reference->Next();
			} while (reference);
		}
	}
}


WaterRebuildUndoData::WaterRebuildUndoData(WaterBlock *block) : UndoData(kUndoWaterRebuild)
{
	blockNode = block;
	blockSize = block->GetBlockSize();
	
	const WaterBlockObject *object = block->GetObject();
	waterFieldScale = object->GetWaterFieldScale();
	waterHorizonDistance = object->GetWaterHorizonDistance();
}

WaterRebuildUndoData::~WaterRebuildUndoData()
{
}

void WaterRebuildUndoData::Undo(Editor *editor)
{
	for (;;)
	{
		Node *node = blockNode->GetFirstSubnode();
		if (!node) break;
		
		editor->DeleteNode(node);
	}
	
	blockNode->SetBlockSize(blockSize);
	
	WaterBlockObject *object = blockNode->GetObject();
	object->SetWaterFieldScale(waterFieldScale);
	object->SetWaterHorizonDistance(waterHorizonDistance);
	
	editor->InvalidateNode(blockNode);
	blockNode->Preprocess();
}


WaterBuilder::WaterBuilder(const WaterBlock *block)
{
	const WaterBlockObject *object = block->GetObject();
	waterSize = block->GetBlockSize() * object->GetWaterFieldDimension();
	waterHorizonDistance = object->GetWaterHorizonDistance();
	
	for (machine a = 0; a < 4; a++) horizonFlag[a] = false;
	
	const Node *node = block->GetFirstSubnode();
	while (node)
	{
		if (node->GetNodeType() == kNodeGeometry)
		{
			const Geometry *geometry = static_cast<const Geometry *>(node);
			if (geometry->GetGeometryType() == kGeometryHorizonWater)
			{
				const HorizonWaterGeometry *water = static_cast<const HorizonWaterGeometry *>(geometry);
				horizonFlag[water->GetObject()->GetWaterDirection()] = true;
			}
		}
		
		node = node->Next();
	}
}

WaterBuilder::~WaterBuilder()
{
}

int32 WaterBuilder::GetSettingCount(void) const
{
	return (9);
}

Setting *WaterBuilder::GetSetting(int32 index) const
{
	const StringTable *table = TheWorldEditor->GetStringTable();
	
	if (index == 0)
	{
		const char *title = table->GetString(StringID('WBLD', 'GEOM'));
		return (new HeadingSetting('GEOM', title));
	}
	
	if (index == 1)
	{
		const char *title = table->GetString(StringID('WBLD', 'GEOM', 'XSIZ'));
		return (new TextSetting('XSIZ', Text::IntegerToString(waterSize.x), title, 4, &EditTextWidget::NumberFilter));
	}
	
	if (index == 2)
	{
		const char *title = table->GetString(StringID('WBLD', 'GEOM', 'YSIZ'));
		return (new TextSetting('YSIZ', Text::IntegerToString(waterSize.y), title, 4, &EditTextWidget::NumberFilter));
	}
	
	if (index == 3)
	{
		const char *title = table->GetString(StringID('WBLD', 'GEOM', 'HRZN'));
		return (new TextSetting('HRZN', waterHorizonDistance, title));
	}
	
	if (index == 4)
	{
		const char *title = table->GetString(StringID('WBLD', 'HFLG'));
		return (new HeadingSetting('HFLG', title));
	}
	
	if (index == 5)
	{
		const char *title = table->GetString(StringID('WBLD', 'HFLG', 'EAST'));
		return (new BooleanSetting('EAST', horizonFlag[kWaterDirectionEast], title));
	}
	
	if (index == 6)
	{
		const char *title = table->GetString(StringID('WBLD', 'HFLG', 'WEST'));
		return (new BooleanSetting('WEST', horizonFlag[kWaterDirectionWest], title));
	}
	
	if (index == 7)
	{
		const char *title = table->GetString(StringID('WBLD', 'HFLG', 'NRTH'));
		return (new BooleanSetting('NRTH', horizonFlag[kWaterDirectionNorth], title));
	}
	
	if (index == 8)
	{
		const char *title = table->GetString(StringID('WBLD', 'HFLG', 'SOTH'));
		return (new BooleanSetting('SOTH', horizonFlag[kWaterDirectionSouth], title));
	}
	
	return (nullptr);
}

void WaterBuilder::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'XSIZ')
	{
		waterSize.x = Max(Text::StringToInteger(static_cast<const TextSetting *>(setting)->GetText()), 1);
	}
	else if (identifier == 'YSIZ')
	{
		waterSize.y = Max(Text::StringToInteger(static_cast<const TextSetting *>(setting)->GetText()), 1);
	}
	else if (identifier == 'HRZN')
	{
		waterHorizonDistance = Fmax(Text::StringToFloat(static_cast<const TextSetting *>(setting)->GetText()), 256.0F);
	}
	else if (identifier == 'EAST')
	{
		horizonFlag[kWaterDirectionEast] = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
	}
	else if (identifier == 'WEST')
	{
		horizonFlag[kWaterDirectionWest] = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
	}
	else if (identifier == 'NRTH')
	{
		horizonFlag[kWaterDirectionNorth] = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
	}
	else if (identifier == 'SOTH')
	{
		horizonFlag[kWaterDirectionSouth] = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
	}
}

void WaterBuilder::BuildWater(Job *job, WaterBlock *block)
{
	WaterBlockObject *object = block->GetObject();
	int32 dimension = object->GetWaterFieldDimension();
	
	int32 blockSizeX = (waterSize.x + dimension - 1) / dimension;
	int32 blockSizeY = (waterSize.y + dimension - 1) / dimension;
	
	float dx = object->GetWaterFieldScale() * (float) (block->GetBlockSize().x * dimension);
	float dy = object->GetWaterFieldScale() * (float) (block->GetBlockSize().y * dimension);
	
	float xscale = dx / (float) (blockSizeX * dimension);
	float yscale = dy / (float) (blockSizeY * dimension);
	
	float s1 = Fmax(xscale, yscale);
	float s2 = Fmin(xscale, yscale);
	float scale = object->GetWaterFieldScale();
	object->SetWaterFieldScale((Fabs(scale - s1) < Fabs(scale - s2)) ? s1 : s2);
	
	block->SetBlockSize(Integer2D(blockSizeX, blockSizeY));
	object->SetWaterHorizonDistance(waterHorizonDistance);
}


BuildWaterWindow::BuildWaterWindow(Editor *editor, WaterBlock *block) : Window("WorldEditor/water/Build")
{
	worldEditor = editor;
	blockNode = block;
	
	waterBuilder = new WaterBuilder(block);
}

BuildWaterWindow::~BuildWaterWindow()
{
	delete waterBuilder;
}

void BuildWaterWindow::Preprocess(void)
{
	Window::Preprocess();
	
	buildButton = static_cast<PushButtonWidget *>(FindWidget("Build"));
	cancelButton = static_cast<PushButtonWidget *>(FindWidget("Cancel"));
	
	configurationWidget = static_cast<ConfigurationWidget *>(FindWidget("Config"));
	configurationWidget->BuildConfiguration(waterBuilder);
}

bool BuildWaterWindow::HandleKeyboardEvent(const KeyboardEventData *eventData)
{
	if (eventData->eventType == kEventKeyDown)
	{
		unsigned_int32 code = eventData->keyCode;
		
		if (code == kKeyCodeReturn)
		{
			buildButton->Activate();
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

void BuildWaterWindow::HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData)
{
	EventType eventType = eventData->eventType;
	
	if (eventType == kEventWidgetActivate)
	{
		if (widget == buildButton)
		{
			List<NodeReference>		deletedList;
			
			Node *node = blockNode->GetFirstSubnode();
			if (node)
			{
				do
				{
					deletedList.Append(new NodeReference(node));
					node = node->Next();
				} while (node);
				
				worldEditor->AddUndoData(new DeleteUndoData(&deletedList));
				
				WaterRebuildUndoData *undoData = new WaterRebuildUndoData(blockNode);
				undoData->SetCoupledFlag(true);
				worldEditor->AddUndoData(undoData);
				
				for (;;)
				{
					Node *node = blockNode->GetFirstSubnode();
					if (!node) break;
					
					worldEditor->DeleteNode(node, true);
				}
			}
			
			configurationWidget->CommitConfiguration(waterBuilder);
			
			WaterProgressWindow *window = new WaterProgressWindow(worldEditor, blockNode, waterBuilder);
			worldEditor->AddSubwindow(window);
			
			waterBuilder = nullptr;
			Close();
		}
		else if (widget == cancelButton)
		{
			if (!blockNode->GetFirstSubnode())
			{
				worldEditor->DeleteNode(blockNode);
				worldEditor->DeleteLastUndoData();
			}
			
			Close();
		}
	}
}


WaterProgressWindow::WaterProgressWindow(Editor *editor, WaterBlock *block, WaterBuilder *builder) :
		Window("WorldEditor/water/Progress"),
		waterJob(&WaterJob, this)
{
	worldEditor = editor;
	blockNode = block;
	waterBuilder = builder;
	
	buildSuccess = false;
	TheJobMgr->SubmitJob(&waterJob);
}

WaterProgressWindow::~WaterProgressWindow()
{
	delete waterBuilder;
}

void WaterProgressWindow::WaterJob(Job *job, void *cookie)
{
	WaterProgressWindow *window = static_cast<WaterProgressWindow *>(cookie);
	
	window->waterBuilder->BuildWater(job, window->blockNode);

	const Integer2D& size = window->blockNode->GetBlockSize();
	job->SetJobMagnitude(size.x * size.y + 4);
	int32 progress = 0;
	
	for (machine j = 0; j < size.y; j++)
	{
		for (machine i = 0; i < size.x; i++)
		{
			WaterGeometry *geometry = new WaterGeometry(window->blockNode, Integer2D(i, j));
			geometry->GetObject()->Build(geometry);
			window->geometryList.Append(geometry);
			
			if (job->Cancelled()) return;
			job->SetJobProgress(++progress);
		}
	}
	
	const bool *horizonFlag = window->waterBuilder->GetHorizonFlagArray();
	for (machine a = 0; a < 4; a++)
	{
		if (horizonFlag[a])
		{
			HorizonWaterGeometry *geometry = new HorizonWaterGeometry(window->blockNode, a);
			geometry->GetObject()->Build(geometry);
			window->geometryList.Append(geometry);
		}
		
		if (job->Cancelled()) return;
		job->SetJobProgress(++progress);
	}
	
	window->buildSuccess = true;
}

void WaterProgressWindow::Preprocess(void)
{
	Window::Preprocess();
	
	stopButton = static_cast<PushButtonWidget *>(FindWidget("Stop"));
	progressBar = static_cast<ProgressWidget *>(FindWidget("Progress"));
}

void WaterProgressWindow::Move(void)
{
	Window::Move();
	
	if (waterJob.Complete())
	{
		if (buildSuccess)
		{
			MaterialObject *materialObject = worldEditor->GetSelectedMaterial()->GetMaterialObject();
			for (;;)
			{
				Node *node = geometryList.First();
				if (!node) break;
				
				Geometry *geometry = static_cast<Geometry *>(node);
				geometryList.Remove(geometry);
				
				blockNode->AddSubnode(geometry);
				
				EditorManipulator::Install(worldEditor, geometry);
				Editor::GetManipulator(geometry)->InvalidateGraph();
				geometry->SetMaterialObject(0, materialObject);
			}
			
			blockNode->Preprocess();
			
			worldEditor->PostEvent(GizmoEditorEvent(kEditorEventGizmoTargetInvalidated, blockNode));
			worldEditor->InvalidateAllViewports();
		}
		else
		{
			worldEditor->DeleteNode(blockNode);
			worldEditor->DeleteLastUndoData();
		}
		
		Close();
	}
	else
	{
		progressBar->SetProgress(waterJob.GetJobProgress(), waterJob.GetJobMagnitude());
	}
}

bool WaterProgressWindow::HandleKeyboardEvent(const KeyboardEventData *eventData)
{
	if (eventData->eventType == kEventKeyDown)
	{
		if (eventData->keyCode == kKeyCodeEscape)
		{
			stopButton->Activate();
			return (true);
		}
	}
	
	return (Window::HandleKeyboardEvent(eventData));
}

void WaterProgressWindow::HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData)
{
	if ((eventData->eventType == kEventWidgetActivate) && (widget == stopButton)) TheJobMgr->CancelJob(&waterJob);
}


GenerateLandElevationWindow::GenerateLandElevationWindow(Editor *editor) : Window("WorldEditor/LandElevation")
{
	worldEditor = editor;
	
	jobCount = 0;
	jobTable = nullptr;
}

GenerateLandElevationWindow::~GenerateLandElevationWindow()
{
	int32 count = jobCount;
	for (machine a = count - 1; a >= 0; a--) delete jobTable[a];
	delete[] jobTable;
	
	const NodeReference *reference = worldEditor->GetFirstSelectedNode();
	while (reference)
	{
		Node *node = reference->GetNode();
		if (node->GetNodeType() == kNodeGeometry)
		{
			Geometry *geometry = static_cast<Geometry *>(node);
			if (ValidGeometry(geometry)) worldEditor->InvalidateGeometry(geometry);
		}
		
		reference = reference->Next();
	}
}

GenerateLandElevationWindow::LandElevationJob::LandElevationJob(GenerateLandElevationWindow *window, ExecuteProc *execProc, void *cookie) : Job(execProc, cookie)
{
	jobWindow = window;
}

void GenerateLandElevationWindow::Preprocess(void)
{
	Window::Preprocess();
	
	stopButton = static_cast<PushButtonWidget *>(FindWidget("Stop"));
	progressBar = static_cast<ProgressWidget *>(FindWidget("Progress"));
	
	StartJob();
}

void GenerateLandElevationWindow::Move(void)
{
	Window::Move();
	
	int32 count = jobCount;
	if (count > 0)
	{
		int32 progress = 0;
		
		for (machine a = 0; a < count; a++) progress += jobTable[a]->Complete();
		progressBar->SetValue(progress);
		
		if (progress == count) Close();
	}
}

bool GenerateLandElevationWindow::ValidGeometry(const Geometry *geometry)
{
	return ((geometry->GetGeometryType() == kGeometryWater) && (static_cast<const WaterGeometry *>(geometry)->GetBlockNode()));
}

void GenerateLandElevationWindow::StartJob(void)
{
	worldEditor->AddUndoData(new GeometryUndoData(worldEditor->GetSelectionList(), kGeometryWater));
	
	int32 count = 0;
	const NodeReference *reference = worldEditor->GetFirstSelectedNode();
	while (reference)
	{
		const Node *node = reference->GetNode();
		if ((node->GetNodeType() == kNodeGeometry) && (node->GetObject()->GetReferenceCount() == 1))
		{
			count += ValidGeometry(static_cast<const Geometry *>(node));
		}
		
		reference = reference->Next();
	}
	
	jobCount = count;
	jobTable = new Job *[count];
	
	count = 0;
	reference = worldEditor->GetFirstSelectedNode();
	while (reference)
	{
		Node *node = reference->GetNode();
		if ((node->GetNodeType() == kNodeGeometry) && (node->GetObject()->GetReferenceCount() == 1))
		{
			Geometry *geometry = static_cast<Geometry *>(node);
			if (ValidGeometry(geometry))
			{
				Job *job = new LandElevationJob(this, &GenerateLandElevationJob, geometry);
				jobTable[count++] = job;
				TheJobMgr->SubmitJob(job);
			}
		}
		
		reference = reference->Next();
	}
	
	progressBar->SetMaxValue(count);
}

bool GenerateLandElevationWindow::DetectCollision(const Node *root, Ray *ray, PickData *pickData)
{
	bool result = false;
	
	if (root->Enabled())
	{
		float	t1, t2;
		
		EditorManipulator *manipulator = Editor::GetManipulator(root);
		const BoundingSphere *sphere = manipulator->GetTreeSphere();
		
		if ((sphere) && (Math::IntersectRayAndSphere(ray, sphere->GetCenter(), sphere->GetRadius(), &t1, &t2)))
		{
			if (root->GetNodeType() == kNodeGeometry)
			{
				const Geometry *geometry = static_cast<const Geometry *>(root);
				GeometryType type = geometry->GetGeometryType();
				if ((type != kGeometryWater) && (type != kGeometryHorizonWater) && (!(geometry->GetObject()->GetGeometryFlags() & kGeometryInvisible)))
				{
					sphere = manipulator->GetNodeSphere();
					if ((sphere) && (Math::IntersectRayAndSphere(ray, sphere->GetCenter(), sphere->GetRadius(), &t1, &t2)))
					{
						Ray		nodeRay;
						
						const Transform4D& transform = root->GetInverseWorldTransform();
						nodeRay.origin = transform * ray->origin;
						nodeRay.direction = transform * ray->direction;
						nodeRay.radius = 0.0F;
						nodeRay.tmin = Fmax(ray->tmin, t1);
						nodeRay.tmax = Fmin(ray->tmax, t2);
						
						if (manipulator->Pick(&nodeRay, pickData))
						{
							pickData->pickPoint = root->GetWorldTransform() * pickData->pickPoint;
							pickData->pickNormal = pickData->pickNormal * transform;
							
							ray->tmax = pickData->rayParam;
							result = true;
						}
					}
				}
			}
			
			Node *node = root->GetFirstSubnode();
			while (node)
			{
				result |= DetectCollision(node, ray, pickData);
				node = node->Next();
			}
		}
	}
	
	return (result);
}

void GenerateLandElevationWindow::GenerateLandElevationJob(Job *job, void *cookie)
{
	Ray		ray;
	
	GenerateLandElevationWindow *window = static_cast<LandElevationJob *>(job)->GetJobWindow();
	const Node *rootNode = window->worldEditor->GetRootNode();
	
	ray.radius = 0.0F;
	ray.tmin = 0.0F;
	
	const Geometry *geometry = static_cast<Geometry *>(cookie);
	const WaterBlockObject *blockObject = static_cast<const WaterGeometry *>(geometry)->GetBlockNode()->GetObject();
	
	const GeometryObject *object = geometry->GetObject();
	GeometryLevel *geometryLevel = object->GetGeometryLevel(0);
	int32 vertexCount = geometryLevel->GetVertexCount();
	
	Point3D *restrict land = geometryLevel->GetArray<Point3D>(kArrayPosition1);
	if (!land)
	{
		ArrayDescriptor		desc;
		GeometryLevel		tempLevel;
		
		window->jobLock.AcquireExclusive();
		
		tempLevel.CopyGeometryLevel(geometryLevel);
		
		desc.identifier = kArrayPosition1;
		desc.elementCount = vertexCount;
		desc.elementSize = sizeof(Point3D);
		desc.componentCount = 3;
		
		geometryLevel->AllocateStorage(&tempLevel, 1, &desc);
		land = geometryLevel->GetArray<Point3D>(kArrayPosition1);
		MemoryMgr::ClearMemory(land, vertexCount * sizeof(Point3D));
		
		window->jobLock.ReleaseExclusive();
	}
	
	int32 dimension = blockObject->GetWaterFieldDimension();
	int32 row = dimension + 3;
	float *elevation = new float[row * row];
	
	float scale = blockObject->GetWaterFieldScale();
	const Range<float>& elevationRange = blockObject->GetLandElevationRange();
	float testElevation = blockObject->GetLandTestElevation();
	
	const Integer2D& geometryCoord = static_cast<const WaterGeometryObject *>(object)->GetGeometryCoord();
	const Transform4D& transform = geometry->GetWorldTransform();
	
	window->jobLock.AcquireShared();
	
	for (machine j = -1; j < dimension + 2; j++)
	{
		float y = (float) (geometryCoord.y * dimension + j) * scale;
		float *h = &elevation[(j + 1) * row + 1];
		
		for (machine i = -1; i < dimension + 2; i++)
		{
			PickData	pickData;
			
			float x = (float) (geometryCoord.x * dimension + i) * scale;
			Point3D p = transform.GetTranslation() + (transform[0] * x + transform[1] * y);
			
			ray.origin = p;
			ray.direction.Set(0.0F, 0.0F, testElevation);
			ray.tmax = 1.0F;
			
			float z = testElevation;
			if (DetectCollision(rootNode, &ray, &pickData)) z *= ray.tmax;
			
			ray.origin.z = p.z + z;
			ray.direction.z = elevationRange.min - z;
			ray.tmax = 1.0F;
			
			if (DetectCollision(rootNode, &ray, &pickData)) h[i] = Fmin(ray.origin.z - p.z + ray.tmax * ray.direction.z, elevationRange.max);
			else h[i] = elevationRange.min;
		}
	}
	
	window->jobLock.ReleaseShared();
	
	for (machine j = 0; j <= dimension; j++)
	{
		const float *h = &elevation[(j + 1) * row + 1];
		
		for (machine i = 0; i <= dimension; i++)
		{
			float dx = h[1] - h[-1];
			float dy = h[row] - h[-row];
			
			float r = dx * dx + dy * dy;
			if (r > K::min_float)
			{
				r = InverseSqrt(r);
				land->Set(dx * r, dy * r, h[0]);
			}
			else
			{
				land->Set(0.0F, 0.0F, h[0]);
			}
			
			land++;
			h++;
		}
	}
	
	delete[] elevation;
}

bool GenerateLandElevationWindow::HandleKeyboardEvent(const KeyboardEventData *eventData)
{
	if (eventData->eventType == kEventKeyDown)
	{
		if (eventData->keyCode == kKeyCodeEscape)
		{
			stopButton->Activate();
			return (true);
		}
	}
	
	return (Window::HandleKeyboardEvent(eventData));
}

void GenerateLandElevationWindow::HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetActivate)
	{
		if (widget == stopButton)
		{
			TheJobMgr->CancelJobArray(jobCount, jobTable);
			Close();
		}
	}
}

// ZYURVUR
