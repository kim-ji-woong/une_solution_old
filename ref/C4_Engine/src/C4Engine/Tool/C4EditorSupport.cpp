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
#include "C4TerrainTools.h"
#include "C4WaterTools.h"
#include "C4World.h"


using namespace C4;


EditorObject::EditorObject() : Object(kObjectEditor)
{
	editorFlags = kEditorShowGridlines | kEditorShowViewportInfo | kEditorSnapToGrid | kEditorCapGeometry;
	selectedMaterial = nullptr;
	selectionMask = ~0;
	bookLocation = 0x0001;
	
	gridLineSpacing = 0.1F;
	majorLineInterval = 10;
	
	Variable *var = TheEngine->InitVariable("editorGridColor", "808080");
	gridColor.SetHexString(var->GetValue());
	
	snapAngle = K::pi_over_4;
	cameraSpeed = 0.02F;
	
	targetZone = nullptr;
	
	currentViewportLayout = kEditorLayout4;
	previousViewportLayout = kEditorLayout4;
	fullViewportIndex = kEditorViewportTopLeft;
	
	SetViewportMode(kEditorViewportTopLeft, kViewportModeTop);
	SetViewportMode(kEditorViewportTopRight, kViewportModeFront);
	SetViewportMode(kEditorViewportBottomLeft, kViewportModeGraph);
	SetViewportMode(kEditorViewportBottomRight, kViewportModeFrustum);
	SetViewportMode(kEditorViewportLeft, kViewportModeTop);
	SetViewportMode(kEditorViewportRight, kViewportModeFront);
	SetViewportMode(kEditorViewportTop, kViewportModeTop);
	SetViewportMode(kEditorViewportBottom, kViewportModeRight);
	
	meshOriginSettings = kMeshOriginCenter | (kMeshOriginCenter << 8) | (kMeshOriginCenter << 16);
	
	pageList.Append(new SelectionMaskPage);
	pageList.Append(new GeometriesPage);
	pageList.Append(new CamerasPage);
	pageList.Append(new LightsPage);
	pageList.Append(new SourcesPage);
	pageList.Append(new ZonesPage);
	pageList.Append(new PortalsPage);
	pageList.Append(new SpacesPage);
	pageList.Append(new MarkersPage);
	pageList.Append(new TriggersPage);
	pageList.Append(new EffectsPage);
	pageList.Append(new ParticlesPage);
	pageList.Append(new PhysicsPage);
	pageList.Append(new ModelsPage);
	pageList.Append(new WorldsPage);
	pageList.Append(new SkyboxesPage);
	pageList.Append(new ImpostorsPage);
	pageList.Append(new PathsPage);
	pageList.Append(new TerrainPage);
	pageList.Append(new WaterPage);
	pageList.Append(new PlacementPage);
	pageList.Append(new MaterialPage);
	pageList.Append(new VisibilityPage);
	pageList.Append(viewportsPage = new ViewportsPage);
	pageList.Append(new TransformPage);
	pageList.Append(new TextureMappingPage);
	pageList.Append(new InfoPage);
	pageList.Append(new GridPage);
	pageList.Append(new FindPage);
	pageList.Append(paintPage = new PaintPage);
	
	TheWorldEditor->InitializePlugins(this);
}

EditorObject::~EditorObject()
{
}

void EditorObject::Prepack(List<Object> *linkList) const
{
	MaterialContainer *material = materialList.First();
	while (material)
	{
		MaterialObject *object = material->GetMaterialObject();
		if (object) linkList->Append(object);
		
		material = material->Next();
	}
}

void EditorObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	data << ChunkHeader('FLAG', 4);
	data << editorFlags;
	
	if (selectionMask != ~0)
	{
		data << ChunkHeader('MASK', 4);
		data << selectionMask;
	} 
	
	data << ChunkHeader('BOOK', 4); 
	data << bookLocation; 
	 
	data << ChunkHeader('GRID', 4 + sizeof(ColorRGB));
	data << gridLineSpacing; 
	data << gridColor;
	
	data << ChunkHeader('GMAJ', 4);
	data << majorLineInterval; 
	
	data << ChunkHeader('SNAP', 4);
	data << snapAngle;
	 
	data << ChunkHeader('CAMR', 4);
	data << cameraSpeed;
	
	if (targetZone)
	{
		data << ChunkHeader('TARG', 4);
		data << targetZone->GetNodeIndex();
	}
	
	data << ChunkHeader('LAYO', 12);
	data << currentViewportLayout;
	data << previousViewportLayout;
	data << fullViewportIndex;
	
	for (machine a = 0; a < kEditorViewportCount; a++)
	{
		data << ChunkHeader('VPRT', 8 + sizeof(Transform4D) + sizeof(Vector2D));
		data << (int32) a;
		data << viewportMode[a];
		data << viewportTransform[a];
		data << viewportData[a];
	}
	
	data << ChunkHeader('ORIG', 4);
	data << meshOriginSettings;
	
	const MaterialContainer *material = materialList.First();
	while (material)
	{
		PackHandle handle = data.BeginChunk('MATR');
		material->Pack(data, packFlags);
		data.EndChunk(handle);
		
		material = material->Next();
	}
	
	int32 index = 0;
	material = selectedMaterial->Previous();
	while (material)
	{
		index++;
		material = material->Previous();
	}
	
	data << ChunkHeader('CMAT', 4);
	data << index;
	
	int32 visibleCount = 0;
	
	const EditorPage *page = pageList.First();
	while (page)
	{
		PackHandle handle = data.BeginChunk('PAGE');
		data << page->GetPageType();
		page->Pack(data, packFlags);
		data.EndChunk(handle);
		
		if (page->Visible()) visibleCount++;
		
		page = page->ListElement<EditorPage>::Next();
	}
	
	if (visibleCount != 0)
	{
		data << ChunkHeader('PORD', 4 + visibleCount * 8);
		data << visibleCount;
		
		const EditorPage *page = pageList.First();
		while (page)
		{
			if (page->Visible())
			{
				data << static_cast<const EditorPage *>(page)->GetPageType();
				
				Page *prev = page->ListElement<Page>::Previous();
				if (prev) data << static_cast<const EditorPage *>(prev)->GetPageType();
				else data << int32(0);
			}
			
			page = page->ListElement<EditorPage>::Next();
		}
	}
	
	data << TerminatorChunk;
}

void EditorObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	UnpackChunkList<EditorObject>(data, unpackFlags);
}

bool EditorObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'FLAG':
			
			data >> editorFlags;
			return (true);
		
		case 'MASK':
			
			data >> selectionMask;
			return (true);
		
		case 'BOOK':
		{
			unsigned_int32	location;
			
			data >> location;
			bookLocation = Max(location, 0x0001);
			return (true);
		}
		
		case 'GRID':
			
			data >> gridLineSpacing;
			data >> gridColor;
			return (true);
		
		case 'GMAJ':
			
			data >> majorLineInterval;
			return (true);
		
		case 'SNAP':
			
			data >> snapAngle;
			return (true);
		
		case 'CAMR':
			
			data >> cameraSpeed;
			return (true);
		
		case 'TARG':
		{
			int32	zoneIndex;
			
			data >> zoneIndex;
			data.AddNodeLink(zoneIndex, &TargetZoneLinkProc, this);
			return (true);
		}
		
		case 'LAYO':
			
			data >> currentViewportLayout;
			data >> previousViewportLayout;
			data >> fullViewportIndex;
			return (true);
		
		case 'VPRT':
		{
			int32	index;
			
			data >> index;
			data >> viewportMode[index];
			data >> viewportTransform[index];
			data >> viewportData[index];
			return (true);
		}
		
		case 'ORIG':
			
			data >> meshOriginSettings;
			return (true);
		
		case 'MATR':
			
			if (unpackFlags & kUnpackEditor)
			{
				MaterialContainer *material = new MaterialContainer;
				material->Unpack(data, unpackFlags);
				materialList.Append(material);
				return (true);
			}
			
			break;
		
		case 'CMAT':
			
			if (unpackFlags & kUnpackEditor)
			{
				int32	index;
				
				data >> index;
				selectedMaterial = materialList[index];
				return (true);
			}
			
			break;
		
		case 'PAGE':
			
			if (unpackFlags & kUnpackEditor)
			{
				PageType	type;
				
				data >> type;
				
				EditorPage *page = pageList.First();
				while (page)
				{
					if (page->GetPageType() == type)
					{
						page->Unpack(data, unpackFlags);
						return (true);
					}
					
					page = page->ListElement<EditorPage>::Next();
				}
			}
			
			break;
		
		case 'PORD':
			
			if (unpackFlags & kUnpackEditor)
			{
				int32				count;
				List<EditorPage>	tempList;
								
				data >> count;
				for (machine a = 0; a < count; a++)
				{
					PageType	type, prevType;
					
					data >> type;
					data >> prevType;
					
					EditorPage *page = pageList.First();
					while (page)
					{
						if (page->GetPageType() == type)
						{
							page->prevPageType = prevType;
							tempList.Append(page);
							break;
						}
						
						page = page->ListElement<EditorPage>::Next();
					}
				}
				
				for (;;)
				{
					EditorPage *page = tempList.First();
					if (!page) break;
					
					bool progress = false;
					do
					{
						EditorPage *next = page->ListElement<EditorPage>::Next();
						
						PageType prevType = page->prevPageType;
						if (prevType != 0)
						{
							EditorPage *prevPage = pageList.First();
							while (prevPage)
							{
								if (prevPage->GetPageType() == prevType)
								{
									pageList.InsertAfter(page, prevPage);
									progress = true;
									break;
								}
								
								prevPage = prevPage->ListElement<EditorPage>::Next();
							}
						}
						else
						{
							pageList.Prepend(page);
							progress = true;
						}
						
						page = next;
					} while (page);
					
					if (!progress) pageList.Append(tempList.First());
				}
				
				return (true);
			}
			
			break;
	}
	
	return (false);
}

void EditorObject::TargetZoneLinkProc(Node *node, void *cookie)
{
	EditorObject *editorObject = static_cast<EditorObject *>(cookie);
	editorObject->targetZone = static_cast<Zone *>(node);
}

int32 EditorObject::GetSettingCount(void) const
{
	return (7);
}

Setting *EditorObject::GetSetting(int32 index) const
{
	const StringTable *table = TheWorldEditor->GetStringTable();
	
	if (index == 0)
	{
		const char *title = table->GetString(StringID('SETT', 'DRAW'));
		return (new HeadingSetting('DRAW', title));
	}
	
	if (index == 1)
	{
		int32 selection = 2;
		if (snapAngle == K::pi_over_6) selection = 1;
		else if (snapAngle == K::pi_over_6 * 0.5F) selection = 0;
		
		const char *title = table->GetString(StringID('SETT', 'ASNP'));
		MenuSetting *menu = new MenuSetting('ASNP', selection, title, 3);
		
		menu->SetMenuItemString(0, table->GetString(StringID('SETT', 'ASNP', '15DG')));
		menu->SetMenuItemString(1, table->GetString(StringID('SETT', 'ASNP', '30DG')));
		menu->SetMenuItemString(2, table->GetString(StringID('SETT', 'ASNP', '45DG')));
		
		return (menu);
	}
	
	if (index == 2)
	{
		const char *title = table->GetString(StringID('SETT', 'PAGE'));
		return (new HeadingSetting('PAGE', title));
	}
	
	if ((index >= 3) && (index <= 6))
	{
		index -= 3;
		int32 selection = (bookLocation >> (index * 4)) & 0x0F;
		
		const char *title = table->GetString(StringID('SETT', 'PAG1' + index));
		MenuSetting *menu = new MenuSetting('PAG1' + index, selection, title, 3);
		
		menu->SetMenuItemString(0, table->GetString(StringID('SETT', 'PAGE', 'NONE')));
		menu->SetMenuItemString(1, table->GetString(StringID('SETT', 'PAGE', 'LEFT')));
		menu->SetMenuItemString(2, table->GetString(StringID('SETT', 'PAGE', 'RGHT')));
		
		return (menu);
	}
	
	return (nullptr);
}

void EditorObject::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'ASNP')
	{
		int32 selection = static_cast<const MenuSetting *>(setting)->GetMenuSelection();
		
		if (selection == 0) snapAngle = K::pi_over_6 * 0.5F;
		else if (selection == 1) snapAngle = K::pi_over_6;
		else snapAngle = K::pi_over_4;
	}
	else if ((identifier >= 'PAG1') && (identifier <= 'PAG4'))
	{
		int32 selection = static_cast<const MenuSetting *>(setting)->GetMenuSelection();
		
		int32 shift = (identifier - 'PAG1') * 4;
		bookLocation = (bookLocation & ~(0x0F << shift)) | (selection << shift);
		
		if (identifier == 'PAG4') bookLocation = Max(bookLocation, 0x0001);
	}
}

void EditorObject::Preprocess(Editor *editor)
{
	if (!selectedMaterial)
	{
		MaterialContainer *material = materialList.First();
		if (!material)
		{
			MaterialObject *object = new MaterialObject;
			material = new MaterialContainer(object, "default");
			materialList.Append(material);
			object->Release();
		}
		
		selectedMaterial = material;
	}
	
	EditorPage *page = pageList.First();
	while (page)
	{
		page->SetEditor(editor);
		page = page->ListElement<EditorPage>::Next();
	}
}

void EditorObject::SetCurrentViewportLayout(int32 layout)
{
	currentViewportLayout = layout;
	viewportsPage->SetViewportLayout(layout);
}

void EditorObject::SetViewportMode(int32 index, int32 mode)
{
	viewportMode[index] = mode;
	
	switch (mode)
	{
		case kViewportModeTop:
			
			viewportTransform[index].Set(1.0F, 0.0F, 0.0F, 0.0F, 0.0F, -1.0F, 0.0F, 0.0F, 0.0F, 0.0F, -1.0F, 0.0F);
			viewportData[index].Set(0.015625F, 0.015625F);
			break;
		
		case kViewportModeBottom:
			
			viewportTransform[index].Set(-1.0F, 0.0F, 0.0F, 0.0F, 0.0F, -1.0F, 0.0F, 0.0F, 0.0F, 0.0F, 1.0F, 0.0F);
			viewportData[index].Set(0.015625F, 0.015625F);
			break;
		
		case kViewportModeFront:
			
			viewportTransform[index].Set(0.0F, 0.0F, -1.0F, 0.0F, 1.0F, 0.0F, 0.0F, 0.0F, 0.0F, -1.0F, 0.0F, 0.0F);
			viewportData[index].Set(0.015625F, 0.015625F);
			break;
		
		case kViewportModeBack:
			
			viewportTransform[index].Set(0.0F, 0.0F, 1.0F, 0.0F, -1.0F, 0.0F, 0.0F, 0.0F, 0.0F, -1.0F, 0.0F, 0.0F);
			viewportData[index].Set(0.015625F, 0.015625F);
			break;
		
		case kViewportModeRight:
			
			viewportTransform[index].Set(1.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 1.0F, 0.0F, 0.0F, -1.0F, 0.0F, 0.0F);
			viewportData[index].Set(0.015625F, 0.015625F);
			break;
		
		case kViewportModeLeft:
			
			viewportTransform[index].Set(-1.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F, -1.0F, 0.0F, 0.0F, -1.0F, 0.0F, 0.0F);
			viewportData[index].Set(0.015625F, 0.015625F);
			break;
		
		case kViewportModeFrustum:
			
			viewportTransform[index].Set(0.0F, 0.0F, -1.0F, 5.0F, 1.0F, 0.0F, 0.0F, 0.0F, 0.0F, -1.0F, 0.0F, 1.0F);
			viewportData[index].Set(K::pi, 0.0F);
			break;
		
		case kViewportModeGraph:
			
			viewportTransform[index].Set(1.0F, 0.0F, 0.0F, kGraphBoxWidth, 0.0F, 1.0F, 0.0F, 8.0F, 0.0F, 0.0F, 1.0F, 0.0F);
			viewportData[index].Set(1.0F, 1.0F);
			break;
	}
}

void EditorObject::ConfirmMaterialContainer(MaterialObject *object)
{
	MaterialContainer *container = materialList.First();
	while (container)
	{
		if (container->GetMaterialObject() == object)
		{
			container->SetUsageCount(container->GetUsageCount() + 1);
			return;
		}
		
		container = container->Next();
	}
	
	container = new MaterialContainer(object);
	container->SetUsageCount(1);
	materialList.Append(container);
}

List<MaterialContainer> *EditorObject::UpdateMaterialList(const Node *root)
{
	MaterialContainer *container = materialList.First();
	while (container)
	{
		container->SetUsageCount(0);
		container = container->Next();
	}
	
	const Node *node = root;
	do
	{
		NodeType type = node->GetNodeType();
		if (type == kNodeGeometry)
		{
			const Geometry *geometry = static_cast<const Geometry *>(node);
			int32 count = geometry->GetMaterialCount();
			for (machine a = 0; a < count; a++)
			{
				MaterialObject *object = geometry->GetMaterialObject(a);
				if (object) ConfirmMaterialContainer(object);
			}
		}
		else if (type == kNodeSkybox)
		{
			const Skybox *skybox = static_cast<const Skybox *>(node);
			MaterialObject *object = skybox->GetMaterialObject();
			if (object) ConfirmMaterialContainer(object);
		}
		else if (type == kNodeImpostor)
		{
			const Impostor *impostor = static_cast<const Impostor *>(node);
			MaterialObject *object = impostor->GetMaterialObject();
			if (object) ConfirmMaterialContainer(object);
		}
		else if (type == kNodeEffect)
		{
			const Effect *effect = static_cast<const Effect *>(node);
			if (effect->GetEffectType() == kEffectParticleSystem)
			{
				const ParticleSystem *particleSystem = static_cast<const ParticleSystem *>(effect);
				MaterialObject *object = particleSystem->GetMaterialObject();
				if (object) ConfirmMaterialContainer(object);
			}
		}
		
		if ((type != kNodeInstance) && (type != kNodeModel)) node = root->GetNextNode(node);
		else node = root->GetNextLevelNode(node);
		
	} while (node);
	
	return (&materialList);
}

MaterialContainer *EditorObject::FindMaterialContainer(const MaterialObject *materialObject) const
{
	MaterialContainer *container = materialList.First();
	while (container)
	{
		const MaterialObject *object = container->GetMaterialObject();
		if (object == materialObject) return (container);
		
		container = container->Next();
	}
	
	return (nullptr);
}

MaterialObject *EditorObject::FindMatchingMaterial(const MaterialObject *materialObject) const
{
	MaterialContainer *container = materialList.First();
	while (container)
	{
		MaterialObject *object = container->GetMaterialObject();
		if (*object == *materialObject) return (object);
		
		container = container->Next();
	}
	
	return (nullptr);
}

MaterialObject *EditorObject::FindNamedMaterial(const char *name) const
{
	MaterialContainer *container = materialList.First();
	while (container)
	{
		if (container->GetMaterialName() == name) return (container->GetMaterialObject());
		container = container->Next();
	}
	
	return (nullptr);
}

void EditorObject::ReplaceMaterial(MaterialObject *oldMaterial, MaterialObject *newMaterial, Node *root)
{
	Node *node = root->GetFirstSubnode();
	while (node)
	{
		NodeType type = node->GetNodeType();
		if (type == kNodeGeometry)
		{
			bool replaced = false;
			Geometry *geometry = static_cast<Geometry *>(node);
			int32 count = geometry->GetMaterialCount();
			for (machine a = 0; a < count; a++)
			{
				if (geometry->GetMaterialObject(a) == oldMaterial)
				{
					geometry->SetMaterialObject(a, newMaterial);
					replaced = true;
				}
			}
			
			if (replaced) geometry->InvalidateShaderData();
		}
		else if (type == kNodeSkybox)
		{
			Skybox *skybox = static_cast<Skybox *>(node);
			if (skybox->GetMaterialObject() == oldMaterial)
			{
				skybox->SetMaterialObject(newMaterial);
				skybox->InvalidateShaderData();
			}
		}
		
		node = root->GetNextNode(node);
	}
}

void EditorObject::CleanupMaterials(Node *root)
{
	if (selectedMaterial)
	{
		MaterialObject *selectedObject = selectedMaterial->GetMaterialObject();
		
		MaterialContainer *container = materialList.First();
		while (container)
		{
			MaterialContainer *next = container->Next();
			
			MaterialObject *object = container->GetMaterialObject();
			if ((object != selectedObject) && (*object == *selectedObject))
			{
				ReplaceMaterial(object, selectedObject, root);
				delete container;
			}
			
			container = next;
		}
	}
	
	MaterialContainer *originalContainer = materialList.First();
	while (originalContainer)
	{
		MaterialContainer *nextOriginal = originalContainer->Next();
		MaterialObject *originalObject = originalContainer->GetMaterialObject();
		
		MaterialContainer *duplicateContainer = nextOriginal;
		while (duplicateContainer)
		{
			MaterialContainer *nextDuplicate = duplicateContainer->Next();
			
			MaterialObject *duplicateObject = duplicateContainer->GetMaterialObject();
			if (*duplicateObject == *originalObject)
			{
				ReplaceMaterial(duplicateObject, originalObject, root);
				if (duplicateContainer == nextOriginal) nextOriginal = nextDuplicate;
				delete duplicateContainer;
			}
			
			duplicateContainer = nextDuplicate;
		}
		
		originalContainer = nextOriginal;
	}
	
	UpdateMaterialList(root);
}

void EditorObject::AddEditorPage(EditorPage *page)
{
	pageList.Append(page);
}


WorldSavePicker::WorldSavePicker() : FilePicker('WRLD', nullptr, TheResourceMgr->GetGenericCatalog(), WorldResource::GetDescriptor(), nullptr, kFilePickerSave, "WorldEditor/WorldSave")
{
}

WorldSavePicker::~WorldSavePicker()
{
}

void WorldSavePicker::Preprocess(void)
{
	FilePicker::Preprocess();
	
	stripBox = static_cast<CheckWidget *>(FindWidget("Strip"));
}


EditorSettingsWindow::EditorSettingsWindow(Editor *editor) : Window("WorldEditor/Settings")
{
	worldEditor = editor;
}

EditorSettingsWindow::~EditorSettingsWindow()
{
}

void EditorSettingsWindow::Preprocess(void)
{
	Window::Preprocess();
	
	okayButton = static_cast<PushButtonWidget *>(FindWidget("OK"));
	cancelButton = static_cast<PushButtonWidget *>(FindWidget("Cancel"));
	
	configurationWidget = static_cast<ConfigurationWidget *>(FindWidget("Config"));
	configurationWidget->BuildConfiguration(worldEditor->GetEditorObject());
	
	SetNextFocusWidget();
}

bool EditorSettingsWindow::HandleKeyboardEvent(const KeyboardEventData *eventData)
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

void EditorSettingsWindow::HandleWidgetEvent(Widget *widget, const WidgetEventData *eventData)
{
	if (eventData->eventType == kEventWidgetActivate)
	{
		if (widget == okayButton)
		{
			configurationWidget->CommitConfiguration(worldEditor->GetEditorObject());
			worldEditor->UpdateViewportStructures();
			Close();
		}
		else if (widget == cancelButton)
		{
			Close();
		}
	}
}

// ZYURVUR
