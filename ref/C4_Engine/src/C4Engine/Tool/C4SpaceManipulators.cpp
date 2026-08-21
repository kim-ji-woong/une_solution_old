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


#include "C4SpaceManipulators.h"
#include "C4EditorSupport.h"
#include "C4WorldEditor.h"


using namespace C4;


namespace
{
	const ConstColorRGBA kSpaceInteriorColor = {0.75F, 0.75F, 0.75F, 0.75F};
	const ConstColorRGBA kSpaceOutlineColor = {1.0F, 1.0F, 1.0F, 1.0F};
}


SpaceManipulator::SpaceManipulator(Space *space, VolumeManipulator *volume, const char *iconName) : EditorManipulator(space, iconName)
{
	volumeManipulator = volume;
}

SpaceManipulator::~SpaceManipulator()
{
}

Manipulator *SpaceManipulator::Construct(Space *space)
{
	switch (space->GetSpaceType())
	{
		case kSpaceFog:
			
			return (new FogSpaceManipulator(static_cast<FogSpace *>(space)));
		
		case kSpaceShadow:
			
			return (new ShadowSpaceManipulator(static_cast<ShadowSpace *>(space)));
		
		case kSpaceAmbient:
			
			return (new AmbientSpaceManipulator(static_cast<AmbientSpace *>(space)));
		
		case kSpaceAcoustics:
			
			return (new AcousticsSpaceManipulator(static_cast<AcousticsSpace *>(space)));
		
		case kSpaceOcclusion:
			
			return (new OcclusionSpaceManipulator(static_cast<OcclusionSpace *>(space)));
		
		case kSpacePaint:
			
			return (new PaintSpaceManipulator(static_cast<PaintSpace *>(space)));
	}
	
	return (nullptr);
}

const char *SpaceManipulator::GetDefaultNodeName(void) const
{
	return (TheWorldEditor->GetStringTable()->GetString(StringID('NAME', kNodeSpace, GetTargetNode()->GetSpaceType())));
}

void SpaceManipulator::Select(void)
{
	EditorManipulator::Select();
	volumeManipulator->Select();
}

void SpaceManipulator::Unselect(void)
{
	EditorManipulator::Unselect();
	volumeManipulator->Unselect();
}

bool SpaceManipulator::Pick(const Ray *ray, PickData *data) const
{
	return (volumeManipulator->Pick(ray, data));
}

bool SpaceManipulator::RegionPick(const Region *region) const
{
	return (volumeManipulator->RegionPick(GetTargetNode()->GetWorldTransform(), region));
}

void SpaceManipulator::Render(const ManipulatorRenderData *renderData)
{
	volumeManipulator->Render(renderData);
	EditorManipulator::Render(renderData);
}


FogSpaceManipulator::FogSpaceManipulator(FogSpace *fog) :
		SpaceManipulator(fog, this, "WorldEditor/space/Fog"),
		PlateVolumeManipulator(fog, kSpaceInteriorColor, kSpaceOutlineColor, "WorldEditor/volume/Fog")
{
}

FogSpaceManipulator::~FogSpaceManipulator()
{
}

Box3D FogSpaceManipulator::CalculateNodeBoundingBox(void) const
{ 
	return (PlateVolumeManipulator::CalculateBoundingBox(GetObject()->GetPlateSize()));
} 
 
int32 FogSpaceManipulator::GetHandleTable(Point3D *handle) const 
{
	return (PlateVolumeManipulator::GetHandleTable(GetObject()->GetPlateSize(), handle)); 
}

void FogSpaceManipulator::GetHandleData(int32 index, ManipulatorHandleData *handleData) const
{ 
	PlateVolumeManipulator::GetHandleData(index, handleData);
}

bool FogSpaceManipulator::Resize(const ManipulatorResizeData *resizeData) 
{
	FogSpaceObject *object = GetObject();
	
	Vector2D newSize = object->GetPlateSize();
	const Vector2D *oldSize = reinterpret_cast<const Vector2D *>(GetOriginalSize());
	bool move = PlateVolumeManipulator::Resize(resizeData, *oldSize, newSize);
	
	object->SetPlateSize(newSize);
	return (move);
}

void FogSpaceManipulator::Update(void)
{
	if (!(GetManipulatorState() & kManipulatorUpdated)) SetPlateSize(GetObject()->GetPlateSize());
	SpaceManipulator::Update();
}


ShadowSpaceManipulator::ShadowSpaceManipulator(ShadowSpace *shadow) :
		SpaceManipulator(shadow, this, "WorldEditor/space/Shadow"),
		BoxVolumeManipulator(shadow, kSpaceInteriorColor, kSpaceOutlineColor, "WorldEditor/volume/Shadow")
{
}

ShadowSpaceManipulator::~ShadowSpaceManipulator()
{
}

Box3D ShadowSpaceManipulator::CalculateNodeBoundingBox(void) const
{
	return (BoxVolumeManipulator::CalculateBoundingBox(GetObject()->GetBoxSize()));
}

int32 ShadowSpaceManipulator::GetHandleTable(Point3D *handle) const
{
	return (BoxVolumeManipulator::GetHandleTable(GetObject()->GetBoxSize(), handle));
}

void ShadowSpaceManipulator::GetHandleData(int32 index, ManipulatorHandleData *handleData) const
{
	BoxVolumeManipulator::GetHandleData(index, handleData);
}

bool ShadowSpaceManipulator::Resize(const ManipulatorResizeData *resizeData)
{
	ShadowSpaceObject *object = GetObject();
	
	Vector3D newSize = object->GetBoxSize();
	const Vector3D *oldSize = reinterpret_cast<const Vector3D *>(GetOriginalSize());
	bool move = BoxVolumeManipulator::Resize(resizeData, *oldSize, newSize);
	
	object->SetBoxSize(newSize);
	return (move);
}

void ShadowSpaceManipulator::Update(void)
{
	if (!(GetManipulatorState() & kManipulatorUpdated)) SetBoxSize(GetObject()->GetBoxSize());
	SpaceManipulator::Update();
}


AmbientSpaceManipulator::AmbientSpaceManipulator(AmbientSpace *ambient) :
		SpaceManipulator(ambient, this, "WorldEditor/space/Ambient"),
		BoxVolumeManipulator(ambient, kSpaceInteriorColor, kSpaceOutlineColor, "WorldEditor/volume/Ambient"),
		gridDiffuseColor(ColorRGBA(1.0F, 1.0F, 0.0F, 1.0F)),
		gridRenderable(kRenderIndexedLines, kRenderDepthTest | kRenderDepthInhibit)
{
	gridAttributeList.Append(&gridDiffuseColor);
	gridRenderable.SetMaterialAttributeList(&gridAttributeList);
	gridRenderable.SetTransformable(ambient);
}

AmbientSpaceManipulator::~AmbientSpaceManipulator()
{
}

Box3D AmbientSpaceManipulator::CalculateNodeBoundingBox(void) const
{
	return (BoxVolumeManipulator::CalculateBoundingBox(GetObject()->GetBoxSize()));
}

int32 AmbientSpaceManipulator::GetHandleTable(Point3D *handle) const
{
	return (BoxVolumeManipulator::GetHandleTable(GetObject()->GetBoxSize(), handle));
}

void AmbientSpaceManipulator::GetHandleData(int32 index, ManipulatorHandleData *handleData) const
{
	BoxVolumeManipulator::GetHandleData(index, handleData);
}

bool AmbientSpaceManipulator::Resize(const ManipulatorResizeData *resizeData)
{
	AmbientSpaceObject *object = GetObject();
	
	Vector3D newSize = object->GetBoxSize();
	const Vector3D *oldSize = reinterpret_cast<const Vector3D *>(GetOriginalSize());
	bool move = BoxVolumeManipulator::Resize(resizeData, *oldSize, newSize);
	
	object->SetBoxSize(newSize);
	return (move);
}

void AmbientSpaceManipulator::Update(void)
{
	if (!(GetManipulatorState() & kManipulatorUpdated))
	{
		SetBoxSize(GetObject()->GetBoxSize());
		
		const AmbientSpaceObject *object = GetObject();
		const Vector3D& boxSize = object->GetBoxSize();
		
		const int32 *textureSize = object->GetTextureSize();
		int32 width = textureSize[0] - 1;
		int32 height = textureSize[1] - 1;
		int32 depth = textureSize[2] - 1;
		
		Point3D *vertex = gridVertex;
		Line *line = gridLine;
		unsigned_int16 base = 0;
		
		float dx = boxSize.x / (float) width;
		for (machine i = 1; i < width; i++)
		{
			float x = (float) i * dx;
			
			vertex[0].Set(x, 0.0F, 0.0F);
			vertex[1].Set(x, boxSize.y, 0.0F);
			vertex[2].Set(x, boxSize.y, boxSize.z);
			vertex[3].Set(x, 0.0F, boxSize.z);
			
			line[0].Set(base, base + 1);
			line[1].Set(base + 1, base + 2);
			line[2].Set(base + 2, base + 3);
			line[3].Set(base + 3, base);
			
			vertex += 4;
			line += 4;
			base += 4;
		}
		
		float dy = boxSize.y / (float) height;
		for (machine j = 1; j < height; j++)
		{
			float y = (float) j * dy;
			
			vertex[0].Set(0.0F, y, 0.0F);
			vertex[1].Set(boxSize.x, y, 0.0F);
			vertex[2].Set(boxSize.x, y, boxSize.z);
			vertex[3].Set(0.0F, y, boxSize.z);
			
			line[0].Set(base, base + 1);
			line[1].Set(base + 1, base + 2);
			line[2].Set(base + 2, base + 3);
			line[3].Set(base + 3, base);
			
			vertex += 4;
			line += 4;
			base += 4;
		}
		
		float dz = boxSize.z / (float) depth;
		for (machine k = 1; k < depth; k++)
		{
			float z = (float) k * dz;
			
			vertex[0].Set(0.0F, 0.0F, z);
			vertex[1].Set(boxSize.x, 0.0F, z);
			vertex[2].Set(boxSize.x, boxSize.y, z);
			vertex[3].Set(0.0F, boxSize.y, z);
			
			line[0].Set(base, base + 1);
			line[1].Set(base + 1, base + 2);
			line[2].Set(base + 2, base + 3);
			line[3].Set(base + 3, base);
			
			vertex += 4;
			line += 4;
			base += 4;
		}
	}
	
	SpaceManipulator::Update();
}

void AmbientSpaceManipulator::Render(const ManipulatorRenderData *renderData)
{
	SpaceManipulator::Render(renderData);
	
	List<Renderable> *renderList = renderData->manipulatorList;
	if (renderList)
	{
		const int32 *textureSize = GetObject()->GetTextureSize();
		int32 width = textureSize[0] - 2;
		int32 height = textureSize[1] - 2;
		int32 depth = textureSize[2] - 2;
		
		int32 count = (width + height + depth) * 4;
		gridRenderable.SetVertexCount(count);
		gridRenderable.SetLineArray(count, gridLine);
		gridRenderable.SetAttributeArray(kArrayVertex, gridVertex);
		renderList->Append(&gridRenderable);
	}
}


AcousticsSpaceManipulator::AcousticsSpaceManipulator(AcousticsSpace *acoustics) :
		SpaceManipulator(acoustics, this, "WorldEditor/space/Acoustics"),
		BoxVolumeManipulator(acoustics, kSpaceInteriorColor, kSpaceOutlineColor, "WorldEditor/volume/Acoustics")
{
}

AcousticsSpaceManipulator::~AcousticsSpaceManipulator()
{
}

Box3D AcousticsSpaceManipulator::CalculateNodeBoundingBox(void) const
{
	return (BoxVolumeManipulator::CalculateBoundingBox(GetObject()->GetBoxSize()));
}

int32 AcousticsSpaceManipulator::GetHandleTable(Point3D *handle) const
{
	return (BoxVolumeManipulator::GetHandleTable(GetObject()->GetBoxSize(), handle));
}

void AcousticsSpaceManipulator::GetHandleData(int32 index, ManipulatorHandleData *handleData) const
{
	BoxVolumeManipulator::GetHandleData(index, handleData);
}

bool AcousticsSpaceManipulator::Resize(const ManipulatorResizeData *resizeData)
{
	AcousticsSpaceObject *object = GetObject();
	
	Vector3D newSize = object->GetBoxSize();
	const Vector3D *oldSize = reinterpret_cast<const Vector3D *>(GetOriginalSize());
	bool move = BoxVolumeManipulator::Resize(resizeData, *oldSize, newSize);
	
	object->SetBoxSize(newSize);
	return (move);
}

void AcousticsSpaceManipulator::Update(void)
{
	if (!(GetManipulatorState() & kManipulatorUpdated)) SetBoxSize(GetObject()->GetBoxSize());
	SpaceManipulator::Update();
}


OcclusionSpaceManipulator::OcclusionSpaceManipulator(OcclusionSpace *occlusion) :
		SpaceManipulator(occlusion, this, "WorldEditor/space/Occlusion"),
		BoxVolumeManipulator(occlusion, ColorRGBA(0.75F, 0.0F, 0.0F, 0.75F), kSpaceOutlineColor, "WorldEditor/volume/Occlusion")
{
}

OcclusionSpaceManipulator::~OcclusionSpaceManipulator()
{
}

Box3D OcclusionSpaceManipulator::CalculateNodeBoundingBox(void) const
{
	return (BoxVolumeManipulator::CalculateBoundingBox(GetObject()->GetBoxSize()));
}

int32 OcclusionSpaceManipulator::GetHandleTable(Point3D *handle) const
{
	return (BoxVolumeManipulator::GetHandleTable(GetObject()->GetBoxSize(), handle));
}

void OcclusionSpaceManipulator::GetHandleData(int32 index, ManipulatorHandleData *handleData) const
{
	BoxVolumeManipulator::GetHandleData(index, handleData);
}

bool OcclusionSpaceManipulator::Resize(const ManipulatorResizeData *resizeData)
{
	OcclusionSpaceObject *object = GetObject();
	
	Vector3D newSize = object->GetBoxSize();
	const Vector3D *oldSize = reinterpret_cast<const Vector3D *>(GetOriginalSize());
	bool move = BoxVolumeManipulator::Resize(resizeData, *oldSize, newSize);
	
	object->SetBoxSize(newSize);
	return (move);
}

void OcclusionSpaceManipulator::Update(void)
{
	if (!(GetManipulatorState() & kManipulatorUpdated)) SetBoxSize(GetObject()->GetBoxSize());
	SpaceManipulator::Update();
}


PaintSpaceManipulator::PaintSpaceManipulator(PaintSpace *paint) :
		SpaceManipulator(paint, this, "WorldEditor/space/Paint"),
		BoxVolumeManipulator(paint, kSpaceInteriorColor, kSpaceOutlineColor, "WorldEditor/volume/Paint")
{
}

PaintSpaceManipulator::~PaintSpaceManipulator()
{
}

Box3D PaintSpaceManipulator::CalculateNodeBoundingBox(void) const
{
	return (BoxVolumeManipulator::CalculateBoundingBox(GetObject()->GetBoxSize()));
}

int32 PaintSpaceManipulator::GetHandleTable(Point3D *handle) const
{
	return (BoxVolumeManipulator::GetHandleTable(GetObject()->GetBoxSize(), handle));
}

void PaintSpaceManipulator::GetHandleData(int32 index, ManipulatorHandleData *handleData) const
{
	BoxVolumeManipulator::GetHandleData(index, handleData);
}

bool PaintSpaceManipulator::Resize(const ManipulatorResizeData *resizeData)
{
	PaintSpaceObject *object = GetObject();
	
	Vector3D newSize = object->GetBoxSize();
	const Vector3D *oldSize = reinterpret_cast<const Vector3D *>(GetOriginalSize());
	bool move = BoxVolumeManipulator::Resize(resizeData, *oldSize, newSize);
	
	object->SetBoxSize(newSize);
	return (move);
}

void PaintSpaceManipulator::Update(void)
{
	if (!(GetManipulatorState() & kManipulatorUpdated)) SetBoxSize(GetObject()->GetBoxSize());
	SpaceManipulator::Update();
}

void PaintSpaceManipulator::HandleDelete(bool undoable)
{
	EditorManipulator::HandleDelete(undoable);
	
	PaintSpace *paintSpace = GetTargetNode();
	const Hub *hub = paintSpace->GetHub();
	if (hub)
	{
		const Connector *connector = hub->GetFirstIncomingEdge();
		while (connector)
		{
			const Connector *next = connector->GetNextIncomingEdge();
			
			Node *start = connector->GetStartElement()->GetNode();
			if (start->GetNodeType() == kNodeGeometry)
			{
				Geometry *geometry = static_cast<Geometry *>(start);
				if (geometry->GetConnectedPaintSpace() == paintSpace)
				{
					geometry->SetConnectedPaintSpace(nullptr);
					geometry->InvalidateShaderData();
					
					Editor::GetManipulator(geometry)->UpdateConnectors();
					
					if (undoable) undoGeometryList.Append(new NodeReference(geometry));
				}
			}
			
			connector = next;
		}
	}
	
	PaintPage *paintPage = GetEditor()->GetEditorObject()->GetPaintPage();
	if (paintPage->GetTargetPaintSpace() == paintSpace) paintPage->SetTargetPaintSpace(nullptr);
}

void PaintSpaceManipulator::HandleUndelete(void)
{
	EditorManipulator::HandleUndelete();
	
	PaintSpace *paintSpace = GetTargetNode();
	for (;;)
	{
		const NodeReference *reference = undoGeometryList.First();
		if (!reference) break;
		
		Geometry *geometry = static_cast<Geometry *>(reference->GetNode());
		geometry->SetConnectedPaintSpace(paintSpace);
		geometry->InvalidateShaderData();
		
		delete reference;
	}
}

void PaintSpaceManipulator::HandleSettingsUpdate(void)
{
	EditorManipulator::HandleSettingsUpdate();
	
	PaintSpace *paintSpace = GetTargetNode();
	const Hub *hub = paintSpace->GetHub();
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
					geometry->InvalidateShaderData();
				}
			}
			
			connector = connector->GetNextIncomingEdge();
		}
	}
	
	PaintPage *paintPage = GetEditor()->GetEditorObject()->GetPaintPage();
	if (paintPage->GetTargetPaintSpace() == paintSpace) paintPage->SetTargetPaintSpace(nullptr);
}

// ZYURVUR
