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


#include "C4Emitters.h"


using namespace C4;


EmitterObject::EmitterObject(EmitterType type, Volume *volume) :
		Object(kObjectEmitter),
		VolumeObject(volume)
{
	emitterType = type;
	emitterFlags = 0;
}

EmitterObject::~EmitterObject()
{
}

EmitterObject *EmitterObject::Construct(Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (data.GetType())
	{
		case kEmitterBox:
			
			return (new BoxEmitterObject);
		
		case kEmitterCylinder:
			
			return (new CylinderEmitterObject);
		
		case kEmitterSphere:
			
			return (new SphereEmitterObject);
	}
	
	return (nullptr);
}

void EmitterObject::PackType(Packer& data) const
{
	Object::PackType(data);
	data << emitterType;
}

void EmitterObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	data << ChunkHeader('DATA', 4);
	data << emitterFlags;
	
	data << TerminatorChunk;
	
	PackVolume(data, packFlags);
}

void EmitterObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	UnpackChunkList<EmitterObject>(data, unpackFlags);
	UnpackVolume(data, unpackFlags);
}

bool EmitterObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'DATA':
			
			data >> emitterFlags;
			return (true);
	}
	
	return (false);
}

int32 EmitterObject::GetObjectSize(float *size) const
{
	return (GetVolumeObjectSize(size));
}

void EmitterObject::SetObjectSize(const float *size)
{
	SetVolumeObjectSize(size);
}


BoxEmitterObject::BoxEmitterObject() : EmitterObject(kEmitterBox, this)
{
}

BoxEmitterObject::BoxEmitterObject(const Vector3D& size) :
		EmitterObject(kEmitterBox, this),
		BoxVolume(size)
{
}

BoxEmitterObject::~BoxEmitterObject()
{
}

float BoxEmitterObject::GetEmitterRadius(void) const
{
	const Vector3D& boxSize = GetBoxSize();
	return (Fmax(boxSize.x, boxSize.y) * 0.5F); 
}
 
float BoxEmitterObject::GetEmitterSurfaceArea(void) const 
{ 
	const Vector3D& boxSize = GetBoxSize();
	return (boxSize.x * boxSize.y); 
}

Point3D BoxEmitterObject::GetVolumeEmissionPoint(void) const
{ 
	const Vector3D& boxSize = GetBoxSize();
	return (Point3D(Math::RandomFloat(boxSize.x), Math::RandomFloat(boxSize.y), Math::RandomFloat(boxSize.z)));
}
 
Point3D BoxEmitterObject::GetTopSurfaceEmissionPoint(void) const
{
	const Vector3D& boxSize = GetBoxSize();
	return (Point3D(Math::RandomFloat(boxSize.x), Math::RandomFloat(boxSize.y), boxSize.z));
}

Point3D BoxEmitterObject::GetBottomSurfaceEmissionPoint(void) const
{
	const Vector3D& boxSize = GetBoxSize();
	return (Point3D(Math::RandomFloat(boxSize.x), Math::RandomFloat(boxSize.y), 0.0F));
}


CylinderEmitterObject::CylinderEmitterObject() : EmitterObject(kEmitterCylinder, this)
{
}

CylinderEmitterObject::CylinderEmitterObject(const Vector2D& size, float height) :
		EmitterObject(kEmitterCylinder, this),
		CylinderVolume(size, height)
{
}

CylinderEmitterObject::~CylinderEmitterObject()
{
}

float CylinderEmitterObject::GetEmitterRadius(void) const
{
	const Vector2D& cylinderSize = GetCylinderSize();
	return (Fmax(cylinderSize.x, cylinderSize.y));
}

float CylinderEmitterObject::GetEmitterSurfaceArea(void) const
{
	const Vector2D& cylinderSize = GetCylinderSize();
	return (K::pi * cylinderSize.x * cylinderSize.y);
}

Point3D CylinderEmitterObject::GetVolumeEmissionPoint(void) const
{
	Vector2D t = CosSin(Math::RandomFloat(K::two_pi));
	float r = Sqrt(Math::RandomFloat(1.0F));
	
	const Vector2D& cylinderSize = GetCylinderSize();
	return (Point3D(t.x * cylinderSize.x * r, t.y * cylinderSize.y * r, Math::RandomFloat(GetCylinderHeight())));
}

Point3D CylinderEmitterObject::GetTopSurfaceEmissionPoint(void) const
{
	Vector2D t = CosSin(Math::RandomFloat(K::two_pi));
	float r = Sqrt(Math::RandomFloat(1.0F));
	
	const Vector2D& cylinderSize = GetCylinderSize();
	return (Point3D(t.x * cylinderSize.x * r, t.y * cylinderSize.y * r, GetCylinderHeight()));
}

Point3D CylinderEmitterObject::GetBottomSurfaceEmissionPoint(void) const
{
	Vector2D t = CosSin(Math::RandomFloat(K::two_pi));
	float r = Sqrt(Math::RandomFloat(1.0F));
	
	const Vector2D& cylinderSize = GetCylinderSize();
	return (Point3D(t.x * cylinderSize.x * r, t.y * cylinderSize.y * r, 0.0F));
}


SphereEmitterObject::SphereEmitterObject() : EmitterObject(kEmitterSphere, this)
{
}

SphereEmitterObject::SphereEmitterObject(const Vector3D& size) :
		EmitterObject(kEmitterSphere, this),
		SphereVolume(size)
{
}

SphereEmitterObject::~SphereEmitterObject()
{
}

float SphereEmitterObject::GetEmitterRadius(void) const
{
	const Vector3D& sphereSize = GetSphereSize();
	return (Fmax(sphereSize.x, sphereSize.y, sphereSize.z));
}

float SphereEmitterObject::GetEmitterSurfaceArea(void) const
{
	const Vector3D& sphereSize = GetSphereSize();
	return (K::pi * sphereSize.x * sphereSize.y);
}

Point3D SphereEmitterObject::GetVolumeEmissionPoint(void) const
{
	Vector3D v = Math::RandomUnitVector();
	float r = Sqrt(Math::RandomFloat(1.0F));
	
	const Vector3D& sphereSize = GetSphereSize();
	return (Point3D(sphereSize.x * v.x * r, sphereSize.y * v.y * r, sphereSize.z * v.z * r));
}

Point3D SphereEmitterObject::GetTopSurfaceEmissionPoint(void) const
{
	Vector3D v = Math::RandomUnitVector();
	v.z = Fabs(v.z);
	
	const Vector3D& sphereSize = GetSphereSize();
	return (Point3D(sphereSize.x * v.x, sphereSize.y * v.y, sphereSize.z * v.z));
}

Point3D SphereEmitterObject::GetBottomSurfaceEmissionPoint(void) const
{
	Vector3D v = Math::RandomUnitVector();
	v.z = Fnabs(v.z);
	
	const Vector3D& sphereSize = GetSphereSize();
	return (Point3D(sphereSize.x * v.x, sphereSize.y * v.y, sphereSize.z * v.z));
}


Emitter::Emitter(EmitterType type) : Node(kNodeEmitter)
{
	emitterType = type;
}

Emitter::Emitter(const Emitter& emitter) : Node(emitter)
{
	emitterType = emitter.emitterType;
}

Emitter::~Emitter()
{
}

Emitter *Emitter::Construct(Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (data.GetType())
	{
		case kEmitterBox:
			
			return (new BoxEmitter);
		
		case kEmitterCylinder:
			
			return (new CylinderEmitter);
		
		case kEmitterSphere:
			
			return (new SphereEmitter);
	}
	
	return (nullptr);
}

void Emitter::PackType(Packer& data) const
{
	Node::PackType(data);
	data << emitterType;
}


BoxEmitter::BoxEmitter() : Emitter(kEmitterBox)
{
}

BoxEmitter::BoxEmitter(const Vector3D& size) : Emitter(kEmitterBox)
{
	SetNewObject(new BoxEmitterObject(size));
}

BoxEmitter::BoxEmitter(const BoxEmitter& boxEmitter) : Emitter(boxEmitter)
{
}

BoxEmitter::~BoxEmitter()
{
}

Node *BoxEmitter::Replicate(void) const
{
	return (new BoxEmitter(*this));
}


CylinderEmitter::CylinderEmitter() : Emitter(kEmitterCylinder)
{
}

CylinderEmitter::CylinderEmitter(const Vector2D& size, float height) : Emitter(kEmitterCylinder)
{
	SetNewObject(new CylinderEmitterObject(size, height));
}

CylinderEmitter::CylinderEmitter(const CylinderEmitter& cylinderEmitter) : Emitter(cylinderEmitter)
{
}

CylinderEmitter::~CylinderEmitter()
{
}

Node *CylinderEmitter::Replicate(void) const
{
	return (new CylinderEmitter(*this));
}


SphereEmitter::SphereEmitter() : Emitter(kEmitterSphere)
{
}

SphereEmitter::SphereEmitter(const Vector3D& size) : Emitter(kEmitterSphere)
{
	SetNewObject(new SphereEmitterObject(size));
}

SphereEmitter::SphereEmitter(const SphereEmitter& sphereEmitter) : Emitter(sphereEmitter)
{
}

SphereEmitter::~SphereEmitter()
{
}

Node *SphereEmitter::Replicate(void) const
{
	return (new SphereEmitter(*this));
}

// ZYURVUR
