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


#include "C4PhysicsManipulators.h"
#include "C4WorldEditor.h"


using namespace C4;


namespace
{
	const ConstColorRGBA kShapeInteriorColor = {0.0F, 1.0F, 0.375F, 0.75F};
	const ConstColorRGBA kShapeOutlineColor = {1.0F, 1.0F, 1.0F, 1.0F};
	
	const ConstColorRGBA kFieldInteriorColor = {0.0F, 0.75F, 1.0F, 0.75F};
	const ConstColorRGBA kFieldOutlineColor = {1.0F, 1.0F, 1.0F, 1.0F};
}


PhysicsNodeManipulator::PhysicsNodeManipulator(PhysicsNode *physicsNode) : EditorManipulator(physicsNode, "WorldEditor/physics/Physics2")
{
	SetManipulatorFlags(kManipulatorLockedController);
}

PhysicsNodeManipulator::~PhysicsNodeManipulator()
{
}

const char *PhysicsNodeManipulator::GetDefaultNodeName(void) const
{
	return (TheWorldEditor->GetStringTable()->GetString(StringID('NAME', kNodePhysics, kNodePhysics)));
}

void PhysicsNodeManipulator::Preprocess(void)
{
	SetManipulatorState(GetManipulatorState() | kManipulatorShowIcon);
	EditorManipulator::Preprocess();
}


ShapeManipulator::ShapeManipulator(Shape *shape, VolumeManipulator *volume) : EditorManipulator(shape, "WorldEditor/physics/Box")
{
	volumeManipulator = volume;
}

ShapeManipulator::~ShapeManipulator()
{
}

Manipulator *ShapeManipulator::Construct(Shape *shape)
{
	switch (shape->GetShapeType())
	{
		case kShapeBox:
			
			return (new BoxShapeManipulator(static_cast<BoxShape *>(shape)));
		
		case kShapePyramid:
			
			return (new PyramidShapeManipulator(static_cast<PyramidShape *>(shape)));
		
		case kShapeCylinder:
			
			return (new CylinderShapeManipulator(static_cast<CylinderShape *>(shape)));
		
		case kShapeCone:
			
			return (new ConeShapeManipulator(static_cast<ConeShape *>(shape)));
		
		case kShapeSphere:
			
			return (new SphereShapeManipulator(static_cast<SphereShape *>(shape)));
		
		case kShapeDome:
			
			return (new DomeShapeManipulator(static_cast<DomeShape *>(shape)));
		
		case kShapeCapsule:
			
			return (new CapsuleShapeManipulator(static_cast<CapsuleShape *>(shape)));
		
		case kShapeTruncatedPyramid:
			
			return (new TruncatedPyramidShapeManipulator(static_cast<TruncatedPyramidShape *>(shape)));
		
		case kShapeTruncatedCone:
			
			return (new TruncatedConeShapeManipulator(static_cast<TruncatedConeShape *>(shape)));
		
		case kShapeTruncatedDome:
			
			return (new TruncatedDomeShapeManipulator(static_cast<TruncatedDomeShape *>(shape)));
	}
	
	return (nullptr);
}

const char *ShapeManipulator::GetDefaultNodeName(void) const
{
	return (TheWorldEditor->GetStringTable()->GetString(StringID('NAME', 'PHYS', GetTargetNode()->GetShapeType())));
}

void ShapeManipulator::Select(void)
{ 
	EditorManipulator::Select();
	volumeManipulator->Select(); 
} 
 
void ShapeManipulator::Unselect(void)
{ 
	EditorManipulator::Unselect();
	volumeManipulator->Unselect();
}
 
bool ShapeManipulator::Pick(const Ray *ray, PickData *data) const
{
	return (volumeManipulator->Pick(ray, data));
} 

bool ShapeManipulator::RegionPick(const Region *region) const
{
	return (volumeManipulator->RegionPick(GetTargetNode()->GetWorldTransform(), region));
}

void ShapeManipulator::Render(const ManipulatorRenderData *renderData)
{
	volumeManipulator->Render(renderData);
	EditorManipulator::Render(renderData);
}


BoxShapeManipulator::BoxShapeManipulator(BoxShape *box) :
		ShapeManipulator(box, this),
		BoxVolumeManipulator(box, kShapeInteriorColor, kShapeOutlineColor, "WorldEditor/volume/Shape")
{
}

BoxShapeManipulator::~BoxShapeManipulator()
{
}

bool BoxShapeManipulator::CalculateNodeSphere(BoundingSphere *sphere) const
{
	BoxVolumeManipulator::CalculateVolumeSphere(GetObject()->GetBoxSize(), sphere);
	return (true);
}

Box3D BoxShapeManipulator::CalculateNodeBoundingBox(void) const
{
	return (BoxVolumeManipulator::CalculateBoundingBox(GetObject()->GetBoxSize()));
}

int32 BoxShapeManipulator::GetHandleTable(Point3D *handle) const
{
	return (BoxVolumeManipulator::GetHandleTable(GetObject()->GetBoxSize(), handle));
}

void BoxShapeManipulator::GetHandleData(int32 index, ManipulatorHandleData *handleData) const
{
	BoxVolumeManipulator::GetHandleData(index, handleData);
}

bool BoxShapeManipulator::Resize(const ManipulatorResizeData *resizeData)
{
	BoxShapeObject *object = GetObject();
	
	Vector3D newSize = object->GetBoxSize();
	const Vector3D *oldSize = reinterpret_cast<const Vector3D *>(GetOriginalSize());
	bool move = BoxVolumeManipulator::Resize(resizeData, *oldSize, newSize);
	
	object->SetBoxSize(newSize);
	return (move);
}

void BoxShapeManipulator::Update(void)
{
	if (!(GetManipulatorState() & kManipulatorUpdated)) SetBoxSize(GetObject()->GetBoxSize());
	ShapeManipulator::Update();
}


PyramidShapeManipulator::PyramidShapeManipulator(PyramidShape *pyramid) :
		ShapeManipulator(pyramid, this),
		PyramidVolumeManipulator(pyramid, kShapeInteriorColor, kShapeOutlineColor, "WorldEditor/volume/Shape")
{
}

PyramidShapeManipulator::~PyramidShapeManipulator()
{
}

bool PyramidShapeManipulator::CalculateNodeSphere(BoundingSphere *sphere) const
{
	const PyramidShapeObject *object = GetObject();
	PyramidVolumeManipulator::CalculateVolumeSphere(object->GetPyramidSize(), object->GetPyramidHeight(), sphere);
	return (true);
}

Box3D PyramidShapeManipulator::CalculateNodeBoundingBox(void) const
{
	const PyramidShapeObject *object = GetObject();
	return (PyramidVolumeManipulator::CalculateBoundingBox(object->GetPyramidSize(), object->GetPyramidHeight()));
}

int32 PyramidShapeManipulator::GetHandleTable(Point3D *handle) const
{
	const PyramidShapeObject *object = GetObject();
	return (PyramidVolumeManipulator::GetHandleTable(object->GetPyramidSize(), object->GetPyramidHeight(), handle));
}

void PyramidShapeManipulator::GetHandleData(int32 index, ManipulatorHandleData *handleData) const
{
	PyramidVolumeManipulator::GetHandleData(index, handleData);
}

bool PyramidShapeManipulator::Resize(const ManipulatorResizeData *resizeData)
{
	PyramidShapeObject *object = GetObject();
	
	Vector2D newSize = object->GetPyramidSize();
	float newHeight = object->GetPyramidHeight();
	const Vector2D *oldSize = reinterpret_cast<const Vector2D *>(GetOriginalSize());
	float oldHeight = GetOriginalSize()[2];
	bool move = PyramidVolumeManipulator::Resize(resizeData, *oldSize, oldHeight, newSize, newHeight);
	
	object->SetPyramidSize(newSize);
	object->SetPyramidHeight(newHeight);
	return (move);
}

void PyramidShapeManipulator::Update(void)
{
	if (!(GetManipulatorState() & kManipulatorUpdated))
	{
		const PyramidShapeObject *object = GetObject();
		SetPyramidSize(object->GetPyramidSize(), object->GetPyramidHeight());
	}
	
	ShapeManipulator::Update();
}


CylinderShapeManipulator::CylinderShapeManipulator(CylinderShape *cylinder) :
		ShapeManipulator(cylinder, this),
		CylinderVolumeManipulator(cylinder, kShapeInteriorColor, kShapeOutlineColor, "WorldEditor/volume/Shape")
{
}

CylinderShapeManipulator::~CylinderShapeManipulator()
{
}

bool CylinderShapeManipulator::CalculateNodeSphere(BoundingSphere *sphere) const
{
	const CylinderShapeObject *object = GetObject();
	CylinderVolumeManipulator::CalculateVolumeSphere(object->GetCylinderSize(), object->GetCylinderHeight(), sphere);
	return (true);
}

Box3D CylinderShapeManipulator::CalculateNodeBoundingBox(void) const
{
	const CylinderShapeObject *object = GetObject();
	return (CylinderVolumeManipulator::CalculateBoundingBox(object->GetCylinderSize(), object->GetCylinderHeight()));
}

int32 CylinderShapeManipulator::GetHandleTable(Point3D *handle) const
{
	const CylinderShapeObject *object = GetObject();
	return (CylinderVolumeManipulator::GetHandleTable(object->GetCylinderSize(), object->GetCylinderHeight(), handle));
}

void CylinderShapeManipulator::GetHandleData(int32 index, ManipulatorHandleData *handleData) const
{
	CylinderVolumeManipulator::GetHandleData(index, handleData);
}

bool CylinderShapeManipulator::Resize(const ManipulatorResizeData *resizeData)
{
	CylinderShapeObject *object = GetObject();
	
	Vector2D newSize = object->GetCylinderSize();
	float newHeight = object->GetCylinderHeight();
	const Vector2D *oldSize = reinterpret_cast<const Vector2D *>(GetOriginalSize());
	float oldHeight = GetOriginalSize()[2];
	bool move = CylinderVolumeManipulator::Resize(resizeData, *oldSize, oldHeight, newSize, newHeight);
	
	object->SetCylinderSize(newSize);
	object->SetCylinderHeight(newHeight);
	return (move);
}

void CylinderShapeManipulator::Update(void)
{
	if (!(GetManipulatorState() & kManipulatorUpdated))
	{
		const CylinderShapeObject *object = GetObject();
		SetCylinderSize(object->GetCylinderSize(), object->GetCylinderHeight());
	}
	
	ShapeManipulator::Update();
}


ConeShapeManipulator::ConeShapeManipulator(ConeShape *cone) :
		ShapeManipulator(cone, this),
		ConeVolumeManipulator(cone, kShapeInteriorColor, kShapeOutlineColor, "WorldEditor/volume/Shape")
{
}

ConeShapeManipulator::~ConeShapeManipulator()
{
}

bool ConeShapeManipulator::CalculateNodeSphere(BoundingSphere *sphere) const
{
	const ConeShapeObject *object = GetObject();
	ConeVolumeManipulator::CalculateVolumeSphere(object->GetConeSize(), object->GetConeHeight(), sphere);
	return (true);
}

Box3D ConeShapeManipulator::CalculateNodeBoundingBox(void) const
{
	const ConeShapeObject *object = GetObject();
	return (ConeVolumeManipulator::CalculateBoundingBox(object->GetConeSize(), object->GetConeHeight()));
}

int32 ConeShapeManipulator::GetHandleTable(Point3D *handle) const
{
	const ConeShapeObject *object = GetObject();
	return (ConeVolumeManipulator::GetHandleTable(object->GetConeSize(), object->GetConeHeight(), handle));
}

void ConeShapeManipulator::GetHandleData(int32 index, ManipulatorHandleData *handleData) const
{
	ConeVolumeManipulator::GetHandleData(index, handleData);
}

bool ConeShapeManipulator::Resize(const ManipulatorResizeData *resizeData)
{
	ConeShapeObject *object = GetObject();
	
	Vector2D newSize = object->GetConeSize();
	float newHeight = object->GetConeHeight();
	const Vector2D *oldSize = reinterpret_cast<const Vector2D *>(GetOriginalSize());
	float oldHeight = GetOriginalSize()[2];
	bool move = ConeVolumeManipulator::Resize(resizeData, *oldSize, oldHeight, newSize, newHeight);
	
	object->SetConeSize(newSize);
	object->SetConeHeight(newHeight);
	return (move);
}

void ConeShapeManipulator::Update(void)
{
	if (!(GetManipulatorState() & kManipulatorUpdated))
	{
		const ConeShapeObject *object = GetObject();
		SetConeSize(object->GetConeSize(), object->GetConeHeight());
	}
	
	ShapeManipulator::Update();
}


SphereShapeManipulator::SphereShapeManipulator(SphereShape *sphere) :
		ShapeManipulator(sphere, this),
		SphereVolumeManipulator(sphere, kShapeInteriorColor, kShapeOutlineColor, "WorldEditor/volume/Shape")
{
}

SphereShapeManipulator::~SphereShapeManipulator()
{
}

bool SphereShapeManipulator::CalculateNodeSphere(BoundingSphere *sphere) const
{
	SphereVolumeManipulator::CalculateVolumeSphere(GetObject()->GetSphereSize(), sphere);
	return (true);
}

Box3D SphereShapeManipulator::CalculateNodeBoundingBox(void) const
{
	return (SphereVolumeManipulator::CalculateBoundingBox(GetObject()->GetSphereSize()));
}

int32 SphereShapeManipulator::GetHandleTable(Point3D *handle) const
{
	return (SphereVolumeManipulator::GetHandleTable(GetObject()->GetSphereSize(), handle));
}

void SphereShapeManipulator::GetHandleData(int32 index, ManipulatorHandleData *handleData) const
{
	SphereVolumeManipulator::GetHandleData(index, handleData);
}

bool SphereShapeManipulator::Resize(const ManipulatorResizeData *resizeData)
{
	SphereShapeObject *object = GetObject();
	
	Vector3D newSize = object->GetSphereSize();
	const Vector3D *oldSize = reinterpret_cast<const Vector3D *>(GetOriginalSize());
	bool move = SphereVolumeManipulator::Resize(resizeData, *oldSize, newSize);
	
	object->SetSphereSize(newSize);
	return (move);
}

void SphereShapeManipulator::Update(void)
{
	if (!(GetManipulatorState() & kManipulatorUpdated)) SetSphereSize(GetObject()->GetSphereSize());
	ShapeManipulator::Update();
}


DomeShapeManipulator::DomeShapeManipulator(DomeShape *dome) :
		ShapeManipulator(dome, this),
		DomeVolumeManipulator(dome, kShapeInteriorColor, kShapeOutlineColor, "WorldEditor/volume/Shape")
{
}

DomeShapeManipulator::~DomeShapeManipulator()
{
}

bool DomeShapeManipulator::CalculateNodeSphere(BoundingSphere *sphere) const
{
	DomeVolumeManipulator::CalculateVolumeSphere(GetObject()->GetDomeSize(), sphere);
	return (true);
}

Box3D DomeShapeManipulator::CalculateNodeBoundingBox(void) const
{
	return (DomeVolumeManipulator::CalculateBoundingBox(GetObject()->GetDomeSize()));
}

int32 DomeShapeManipulator::GetHandleTable(Point3D *handle) const
{
	return (DomeVolumeManipulator::GetHandleTable(GetObject()->GetDomeSize(), handle));
}

void DomeShapeManipulator::GetHandleData(int32 index, ManipulatorHandleData *handleData) const
{
	DomeVolumeManipulator::GetHandleData(index, handleData);
}

bool DomeShapeManipulator::Resize(const ManipulatorResizeData *resizeData)
{
	DomeShapeObject *object = GetObject();
	
	Vector3D newSize = object->GetDomeSize();
	const Vector3D *oldSize = reinterpret_cast<const Vector3D *>(GetOriginalSize());
	bool move = DomeVolumeManipulator::Resize(resizeData, *oldSize, newSize);
	
	object->SetDomeSize(newSize);
	return (move);
}

void DomeShapeManipulator::Update(void)
{
	if (!(GetManipulatorState() & kManipulatorUpdated)) SetDomeSize(GetObject()->GetDomeSize());
	ShapeManipulator::Update();
}


CapsuleShapeManipulator::CapsuleShapeManipulator(CapsuleShape *capsule) :
		ShapeManipulator(capsule, this),
		CapsuleVolumeManipulator(capsule, kShapeInteriorColor, kShapeOutlineColor, "WorldEditor/volume/Shape")
{
}

CapsuleShapeManipulator::~CapsuleShapeManipulator()
{
}

bool CapsuleShapeManipulator::CalculateNodeSphere(BoundingSphere *sphere) const
{
	const CapsuleShapeObject *object = GetObject();
	CapsuleVolumeManipulator::CalculateVolumeSphere(object->GetCapsuleSize(), object->GetCapsuleHeight(), sphere);
	return (true);
}

Box3D CapsuleShapeManipulator::CalculateNodeBoundingBox(void) const
{
	const CapsuleShapeObject *object = GetObject();
	return (CapsuleVolumeManipulator::CalculateBoundingBox(object->GetCapsuleSize(), object->GetCapsuleHeight()));
}

int32 CapsuleShapeManipulator::GetHandleTable(Point3D *handle) const
{
	const CapsuleShapeObject *object = GetObject();
	return (CapsuleVolumeManipulator::GetHandleTable(object->GetCapsuleSize(), object->GetCapsuleHeight(), handle));
}

void CapsuleShapeManipulator::GetHandleData(int32 index, ManipulatorHandleData *handleData) const
{
	CapsuleVolumeManipulator::GetHandleData(index, handleData);
}

bool CapsuleShapeManipulator::Resize(const ManipulatorResizeData *resizeData)
{
	CapsuleShapeObject *object = GetObject();
	
	Vector3D newSize = object->GetCapsuleSize();
	float newHeight = object->GetCapsuleHeight();
	const Vector3D *oldSize = reinterpret_cast<const Vector3D *>(GetOriginalSize());
	float oldHeight = GetOriginalSize()[3];
	bool move = CapsuleVolumeManipulator::Resize(resizeData, *oldSize, oldHeight, newSize, newHeight);
	
	object->SetCapsuleSize(newSize);
	object->SetCapsuleHeight(newHeight);
	return (move);
}

void CapsuleShapeManipulator::Update(void)
{
	if (!(GetManipulatorState() & kManipulatorUpdated))
	{
		const CapsuleShapeObject *object = GetObject();
		SetCapsuleSize(object->GetCapsuleSize(), object->GetCapsuleHeight());
	}
	
	ShapeManipulator::Update();
}


TruncatedPyramidShapeManipulator::TruncatedPyramidShapeManipulator(TruncatedPyramidShape *truncatedPyramid) :
		ShapeManipulator(truncatedPyramid, this),
		TruncatedPyramidVolumeManipulator(truncatedPyramid, kShapeInteriorColor, kShapeOutlineColor, "WorldEditor/volume/Shape")
{
}

TruncatedPyramidShapeManipulator::~TruncatedPyramidShapeManipulator()
{
}

bool TruncatedPyramidShapeManipulator::CalculateNodeSphere(BoundingSphere *sphere) const
{
	const TruncatedPyramidShapeObject *object = GetObject();
	TruncatedPyramidVolumeManipulator::CalculateVolumeSphere(object->GetPyramidSize(), object->GetPyramidHeight(), object->GetPyramidRatio(), sphere);
	return (true);
}

Box3D TruncatedPyramidShapeManipulator::CalculateNodeBoundingBox(void) const
{
	const TruncatedPyramidShapeObject *object = GetObject();
	return (TruncatedPyramidVolumeManipulator::CalculateBoundingBox(object->GetPyramidSize(), object->GetPyramidHeight()));
}

int32 TruncatedPyramidShapeManipulator::GetHandleTable(Point3D *handle) const
{
	const TruncatedPyramidShapeObject *object = GetObject();
	return (TruncatedPyramidVolumeManipulator::GetHandleTable(object->GetPyramidSize(), object->GetPyramidHeight(), object->GetPyramidRatio(), handle));
}

void TruncatedPyramidShapeManipulator::GetHandleData(int32 index, ManipulatorHandleData *handleData) const
{
	TruncatedPyramidVolumeManipulator::GetHandleData(index, handleData);
}

bool TruncatedPyramidShapeManipulator::Resize(const ManipulatorResizeData *resizeData)
{
	TruncatedPyramidShapeObject *object = GetObject();
	
	Vector2D newSize = object->GetPyramidSize();
	float newHeight = object->GetPyramidHeight();
	float newRatio = object->GetPyramidRatio();
	const Vector2D *oldSize = reinterpret_cast<const Vector2D *>(GetOriginalSize());
	float oldHeight = GetOriginalSize()[2];
	float oldRatio = GetOriginalSize()[3];
	bool move = TruncatedPyramidVolumeManipulator::Resize(resizeData, *oldSize, oldHeight, oldRatio, newSize, newHeight, newRatio);
	
	object->SetPyramidSize(newSize);
	object->SetPyramidHeight(newHeight);
	object->SetPyramidRatio(newRatio);
	return (move);
}

void TruncatedPyramidShapeManipulator::Update(void)
{
	if (!(GetManipulatorState() & kManipulatorUpdated))
	{
		const TruncatedPyramidShapeObject *object = GetObject();
		SetTruncatedPyramidSize(object->GetPyramidSize(), object->GetPyramidHeight(), object->GetPyramidRatio());
	}
	
	ShapeManipulator::Update();
}


TruncatedConeShapeManipulator::TruncatedConeShapeManipulator(TruncatedConeShape *truncatedCone) :
		ShapeManipulator(truncatedCone, this),
		TruncatedConeVolumeManipulator(truncatedCone, kShapeInteriorColor, kShapeOutlineColor, "WorldEditor/volume/Shape")
{
}

TruncatedConeShapeManipulator::~TruncatedConeShapeManipulator()
{
}

bool TruncatedConeShapeManipulator::CalculateNodeSphere(BoundingSphere *sphere) const
{
	const TruncatedConeShapeObject *object = GetObject();
	TruncatedConeVolumeManipulator::CalculateVolumeSphere(object->GetConeSize(), object->GetConeHeight(), object->GetConeRatio(), sphere);
	return (true);
}

Box3D TruncatedConeShapeManipulator::CalculateNodeBoundingBox(void) const
{
	const TruncatedConeShapeObject *object = GetObject();
	return (TruncatedConeVolumeManipulator::CalculateBoundingBox(object->GetConeSize(), object->GetConeHeight()));
}

int32 TruncatedConeShapeManipulator::GetHandleTable(Point3D *handle) const
{
	const TruncatedConeShapeObject *object = GetObject();
	return (TruncatedConeVolumeManipulator::GetHandleTable(object->GetConeSize(), object->GetConeHeight(), object->GetConeRatio(), handle));
}

void TruncatedConeShapeManipulator::GetHandleData(int32 index, ManipulatorHandleData *handleData) const
{
	TruncatedConeVolumeManipulator::GetHandleData(index, handleData);
}

bool TruncatedConeShapeManipulator::Resize(const ManipulatorResizeData *resizeData)
{
	TruncatedConeShapeObject *object = GetObject();
	
	Vector2D newSize = object->GetConeSize();
	float newHeight = object->GetConeHeight();
	float newRatio = object->GetConeRatio();
	const Vector2D *oldSize = reinterpret_cast<const Vector2D *>(GetOriginalSize());
	float oldHeight = GetOriginalSize()[2];
	float oldRatio = GetOriginalSize()[3];
	bool move = TruncatedConeVolumeManipulator::Resize(resizeData, *oldSize, oldHeight, oldRatio, newSize, newHeight, newRatio);
	
	object->SetConeSize(newSize);
	object->SetConeHeight(newHeight);
	object->SetConeRatio(newRatio);
	return (move);
}

void TruncatedConeShapeManipulator::Update(void)
{
	if (!(GetManipulatorState() & kManipulatorUpdated))
	{
		const TruncatedConeShapeObject *object = GetObject();
		SetTruncatedConeSize(object->GetConeSize(), object->GetConeHeight(), object->GetConeRatio());
	}
	
	ShapeManipulator::Update();
}


TruncatedDomeShapeManipulator::TruncatedDomeShapeManipulator(TruncatedDomeShape *truncatedDome) :
		ShapeManipulator(truncatedDome, this),
		TruncatedDomeVolumeManipulator(truncatedDome, kShapeInteriorColor, kShapeOutlineColor, "WorldEditor/volume/Shape")
{
}

TruncatedDomeShapeManipulator::~TruncatedDomeShapeManipulator()
{
}

bool TruncatedDomeShapeManipulator::CalculateNodeSphere(BoundingSphere *sphere) const
{
	const TruncatedDomeShapeObject *object = GetObject();
	TruncatedDomeVolumeManipulator::CalculateVolumeSphere(object->GetDomeSize().GetVector2D(), object->GetDomeHeight(), object->GetDomeRatio(), sphere);
	return (true);
}

Box3D TruncatedDomeShapeManipulator::CalculateNodeBoundingBox(void) const
{
	const TruncatedDomeShapeObject *object = GetObject();
	return (TruncatedDomeVolumeManipulator::CalculateBoundingBox(object->GetDomeSize().GetVector2D(), object->GetDomeHeight()));
}

int32 TruncatedDomeShapeManipulator::GetHandleTable(Point3D *handle) const
{
	const TruncatedDomeShapeObject *object = GetObject();
	return (TruncatedDomeVolumeManipulator::GetHandleTable(object->GetDomeSize().GetVector2D(), object->GetDomeHeight(), object->GetDomeRatio(), handle));
}

void TruncatedDomeShapeManipulator::GetHandleData(int32 index, ManipulatorHandleData *handleData) const
{
	TruncatedDomeVolumeManipulator::GetHandleData(index, handleData);
}

bool TruncatedDomeShapeManipulator::Resize(const ManipulatorResizeData *resizeData)
{
	TruncatedDomeShapeObject *object = GetObject();
	
	Vector2D newSize = object->GetDomeSize().GetVector2D();
	float newHeight = object->GetDomeHeight();
	float newRatio = object->GetDomeRatio();
	const Vector2D *oldSize = reinterpret_cast<const Vector2D *>(GetOriginalSize());
	float oldHeight = GetOriginalSize()[2];
	float oldRatio = GetOriginalSize()[3];
	bool move = TruncatedDomeVolumeManipulator::Resize(resizeData, *oldSize, oldHeight, oldRatio, newSize, newHeight, newRatio);
	
	object->SetDomeSize(newSize);
	object->SetDomeHeight(newHeight);
	object->SetDomeRatio(newRatio);
	return (move);
}

void TruncatedDomeShapeManipulator::Update(void)
{
	if (!(GetManipulatorState() & kManipulatorUpdated))
	{
		const TruncatedDomeShapeObject *object = GetObject();
		SetTruncatedDomeSize(object->GetDomeSize().GetVector2D(), object->GetDomeHeight(), object->GetDomeRatio());
	}
	
	ShapeManipulator::Update();
}


JointManipulator::JointManipulator(Joint *joint, const char *iconName) : EditorManipulator(joint, iconName)
{
}

JointManipulator::~JointManipulator()
{
}

Manipulator *JointManipulator::Construct(Joint *joint)
{
	switch (joint->GetJointType())
	{
		case kJointSpherical:
			
			return (new SphericalJointManipulator(static_cast<SphericalJoint *>(joint)));
		
		case kJointUniversal:
			
			return (new UniversalJointManipulator(static_cast<UniversalJoint *>(joint)));
		
		case kJointDiscal:
			
			return (new DiscalJointManipulator(static_cast<DiscalJoint *>(joint)));
		
		case kJointRevolute:
			
			return (new RevoluteJointManipulator(static_cast<RevoluteJoint *>(joint)));
		
		case kJointCylindrical:
			
			return (new CylindricalJointManipulator(static_cast<CylindricalJoint *>(joint)));
		
		case kJointPrismatic:
			
			return (new PrismaticJointManipulator(static_cast<PrismaticJoint *>(joint)));
	}
	
	return (nullptr);
}

void JointManipulator::Preprocess(void)
{
	EditorManipulator::Preprocess();
	SetManipulatorState(GetManipulatorState() | kManipulatorShowIcon);
}

void JointManipulator::Select(void)
{
	EditorManipulator::Select();
}

void JointManipulator::Unselect(void)
{
	EditorManipulator::Unselect();
}

void JointManipulator::Render(const ManipulatorRenderData *renderData)
{
	EditorManipulator::Render(renderData);
}


SphericalJointManipulator::SphericalJointManipulator(SphericalJoint *spherical) : JointManipulator(spherical, "WorldEditor/physics/Spherical")
{
}

SphericalJointManipulator::~SphericalJointManipulator()
{
}

const char *SphericalJointManipulator::GetDefaultNodeName(void) const
{
	return (TheWorldEditor->GetStringTable()->GetString(StringID('NAME', 'PHYS', 'SPHJ')));
}


UniversalJointManipulator::UniversalJointManipulator(UniversalJoint *universal) : JointManipulator(universal, "WorldEditor/physics/Universal")
{
}

UniversalJointManipulator::~UniversalJointManipulator()
{
}

const char *UniversalJointManipulator::GetDefaultNodeName(void) const
{
	return (TheWorldEditor->GetStringTable()->GetString(StringID('NAME', 'PHYS', 'UNIJ')));
}


DiscalJointManipulator::DiscalJointManipulator(DiscalJoint *discal) : JointManipulator(discal, "WorldEditor/physics/Discal")
{
}

DiscalJointManipulator::~DiscalJointManipulator()
{
}

const char *DiscalJointManipulator::GetDefaultNodeName(void) const
{
	return (TheWorldEditor->GetStringTable()->GetString(StringID('NAME', 'PHYS', 'DISJ')));
}


RevoluteJointManipulator::RevoluteJointManipulator(RevoluteJoint *revolute) : JointManipulator(revolute, "WorldEditor/physics/Revolute")
{
}

RevoluteJointManipulator::~RevoluteJointManipulator()
{
}

const char *RevoluteJointManipulator::GetDefaultNodeName(void) const
{
	return (TheWorldEditor->GetStringTable()->GetString(StringID('NAME', 'PHYS', 'REVJ')));
}


CylindricalJointManipulator::CylindricalJointManipulator(CylindricalJoint *cylindrical) : JointManipulator(cylindrical, "WorldEditor/physics/Cylindrical")
{
}

CylindricalJointManipulator::~CylindricalJointManipulator()
{
}

const char *CylindricalJointManipulator::GetDefaultNodeName(void) const
{
	return (TheWorldEditor->GetStringTable()->GetString(StringID('NAME', 'PHYS', 'CYLJ')));
}


PrismaticJointManipulator::PrismaticJointManipulator(PrismaticJoint *prismatic) : JointManipulator(prismatic, "WorldEditor/physics/Prismatic")
{
}

PrismaticJointManipulator::~PrismaticJointManipulator()
{
}

const char *PrismaticJointManipulator::GetDefaultNodeName(void) const
{
	return (TheWorldEditor->GetStringTable()->GetString(StringID('NAME', 'PHYS', 'PRSJ')));
}


FieldManipulator::FieldManipulator(Field *field, VolumeManipulator *volume) : EditorManipulator(field, "WorldEditor/physics/BoxField")
{
	volumeManipulator = volume;
}

FieldManipulator::~FieldManipulator()
{
}

Manipulator *FieldManipulator::Construct(Field *field)
{
	switch (field->GetFieldType())
	{
		case kFieldBox:
			
			return (new BoxFieldManipulator(static_cast<BoxField *>(field)));
		
		case kFieldCylinder:
			
			return (new CylinderFieldManipulator(static_cast<CylinderField *>(field)));
		
		case kFieldSphere:
			
			return (new SphereFieldManipulator(static_cast<SphereField *>(field)));
	}
	
	return (nullptr);
}

void FieldManipulator::Select(void)
{
	EditorManipulator::Select();
	volumeManipulator->Select();
}

void FieldManipulator::Unselect(void)
{
	EditorManipulator::Unselect();
	volumeManipulator->Unselect();
}

bool FieldManipulator::Pick(const Ray *ray, PickData *data) const
{
	return (volumeManipulator->Pick(ray, data));
}

bool FieldManipulator::RegionPick(const Region *region) const
{
	return (volumeManipulator->RegionPick(GetTargetNode()->GetWorldTransform(), region));
}

void FieldManipulator::Render(const ManipulatorRenderData *renderData)
{
	volumeManipulator->Render(renderData);
	EditorManipulator::Render(renderData);
}


BoxFieldManipulator::BoxFieldManipulator(BoxField *box) :
		FieldManipulator(box, this),
		BoxVolumeManipulator(box, kFieldInteriorColor, kFieldOutlineColor, "WorldEditor/volume/Field")
{
}

BoxFieldManipulator::~BoxFieldManipulator()
{
}

const char *BoxFieldManipulator::GetDefaultNodeName(void) const
{
	return (TheWorldEditor->GetStringTable()->GetString(StringID('NAME', 'PHYS', 'BOXF')));
}

bool BoxFieldManipulator::CalculateNodeSphere(BoundingSphere *sphere) const
{
	BoxVolumeManipulator::CalculateVolumeSphere(GetObject()->GetBoxSize(), sphere);
	return (true);
}

Box3D BoxFieldManipulator::CalculateNodeBoundingBox(void) const
{
	return (BoxVolumeManipulator::CalculateBoundingBox(GetObject()->GetBoxSize()));
}

int32 BoxFieldManipulator::GetHandleTable(Point3D *handle) const
{
	return (BoxVolumeManipulator::GetHandleTable(GetObject()->GetBoxSize(), handle));
}

void BoxFieldManipulator::GetHandleData(int32 index, ManipulatorHandleData *handleData) const
{
	BoxVolumeManipulator::GetHandleData(index, handleData);
}

bool BoxFieldManipulator::Resize(const ManipulatorResizeData *resizeData)
{
	BoxFieldObject *object = GetObject();
	
	Vector3D newSize = object->GetBoxSize();
	const Vector3D *oldSize = reinterpret_cast<const Vector3D *>(GetOriginalSize());
	bool move = BoxVolumeManipulator::Resize(resizeData, *oldSize, newSize);
	
	object->SetBoxSize(newSize);
	return (move);
}

void BoxFieldManipulator::Update(void)
{
	if (!(GetManipulatorState() & kManipulatorUpdated)) SetBoxSize(GetObject()->GetBoxSize());
	FieldManipulator::Update();
}


CylinderFieldManipulator::CylinderFieldManipulator(CylinderField *cylinder) :
		FieldManipulator(cylinder, this),
		CylinderVolumeManipulator(cylinder, kFieldInteriorColor, kFieldOutlineColor, "WorldEditor/volume/Field")
{
}

CylinderFieldManipulator::~CylinderFieldManipulator()
{
}

const char *CylinderFieldManipulator::GetDefaultNodeName(void) const
{
	return (TheWorldEditor->GetStringTable()->GetString(StringID('NAME', 'PHYS', 'CYLF')));
}

bool CylinderFieldManipulator::CalculateNodeSphere(BoundingSphere *sphere) const
{
	const CylinderFieldObject *object = GetObject();
	CylinderVolumeManipulator::CalculateVolumeSphere(object->GetCylinderSize(), object->GetCylinderHeight(), sphere);
	return (true);
}

Box3D CylinderFieldManipulator::CalculateNodeBoundingBox(void) const
{
	const CylinderFieldObject *object = GetObject();
	return (CylinderVolumeManipulator::CalculateBoundingBox(object->GetCylinderSize(), object->GetCylinderHeight()));
}

int32 CylinderFieldManipulator::GetHandleTable(Point3D *handle) const
{
	const CylinderFieldObject *object = GetObject();
	return (CylinderVolumeManipulator::GetHandleTable(object->GetCylinderSize(), object->GetCylinderHeight(), handle));
}

void CylinderFieldManipulator::GetHandleData(int32 index, ManipulatorHandleData *handleData) const
{
	CylinderVolumeManipulator::GetHandleData(index, handleData);
}

bool CylinderFieldManipulator::Resize(const ManipulatorResizeData *resizeData)
{
	CylinderFieldObject *object = GetObject();
	
	Vector2D newSize = object->GetCylinderSize();
	float newHeight = object->GetCylinderHeight();
	const Vector2D *oldSize = reinterpret_cast<const Vector2D *>(GetOriginalSize());
	float oldHeight = GetOriginalSize()[2];
	bool move = CylinderVolumeManipulator::Resize(resizeData, *oldSize, oldHeight, newSize, newHeight);
	
	object->SetCylinderSize(newSize);
	object->SetCylinderHeight(newHeight);
	return (move);
}

void CylinderFieldManipulator::Update(void)
{
	if (!(GetManipulatorState() & kManipulatorUpdated))
	{
		const CylinderFieldObject *object = GetObject();
		SetCylinderSize(object->GetCylinderSize(), object->GetCylinderHeight());
	}
	
	FieldManipulator::Update();
}


SphereFieldManipulator::SphereFieldManipulator(SphereField *sphere) :
		FieldManipulator(sphere, this),
		SphereVolumeManipulator(sphere, kFieldInteriorColor, kFieldOutlineColor, "WorldEditor/volume/Field")
{
}

SphereFieldManipulator::~SphereFieldManipulator()
{
}

const char *SphereFieldManipulator::GetDefaultNodeName(void) const
{
	return (TheWorldEditor->GetStringTable()->GetString(StringID('NAME', 'PHYS', 'SPHF')));
}

bool SphereFieldManipulator::CalculateNodeSphere(BoundingSphere *sphere) const
{
	SphereVolumeManipulator::CalculateVolumeSphere(GetObject()->GetSphereSize(), sphere);
	return (true);
}

Box3D SphereFieldManipulator::CalculateNodeBoundingBox(void) const
{
	return (SphereFieldManipulator::CalculateBoundingBox(GetObject()->GetSphereSize()));
}

int32 SphereFieldManipulator::GetHandleTable(Point3D *handle) const
{
	return (SphereVolumeManipulator::GetHandleTable(GetObject()->GetSphereSize(), handle));
}

void SphereFieldManipulator::GetHandleData(int32 index, ManipulatorHandleData *handleData) const
{
	SphereVolumeManipulator::GetHandleData(index, handleData);
}

bool SphereFieldManipulator::Resize(const ManipulatorResizeData *resizeData)
{
	SphereFieldObject *object = GetObject();
	
	Vector3D newSize = object->GetSphereSize();
	const Vector3D *oldSize = reinterpret_cast<const Vector3D *>(GetOriginalSize());
	bool move = SphereVolumeManipulator::Resize(resizeData, *oldSize, newSize);
	
	object->SetSphereSize(newSize);
	return (move);
}

void SphereFieldManipulator::Update(void)
{
	if (!(GetManipulatorState() & kManipulatorUpdated)) SetSphereSize(GetObject()->GetSphereSize());
	FieldManipulator::Update();
}

// ZYURVUR
