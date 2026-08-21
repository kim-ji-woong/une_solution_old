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


#include "C4Forces.h"
#include "C4Fields.h"
#include "C4Physics.h"
#include "C4Water.h"
#include "C4Configuration.h"


using namespace C4;


namespace C4
{
	template class Registrable<Force, ForceRegistration>;
}


ForceRegistration::ForceRegistration(ForceType type, const char *name) : Registration<Force, ForceRegistration>(type)
{
	forceName = name;
}

ForceRegistration::~ForceRegistration()
{
}


Force::Force(ForceType type)
{
	forceType = type;
	
	targetField = nullptr;
}

Force::Force(const Force& force)
{
	forceType = force.forceType;
	
	targetField = nullptr;
}

Force::~Force()
{
}

Force *Force::New(ForceType type)
{
	Type	data[2];
	
	data[0] = type;
	data[1] = 0;
	
	Unpacker unpacker(data);
	return (Construct(unpacker));
}

bool Force::ValidField(const Field *field)
{
	return (true);
}

void Force::RegisterStandardForces(void)
{
	const StringTable *table = TheInterfaceMgr->GetStringTable();
	
	static ForceReg<GravityForce> gravityRegistration(kForceGravity, table->GetString(StringID('FORC', kForceGravity)));
	static ForceReg<FluidForce> fluidRegistration(kForceFluid, table->GetString(StringID('FORC', kForceFluid)));
	static ForceReg<WindForce> windRegistration(kForceWind, table->GetString(StringID('FORC', kForceWind)));
}

void Force::PackType(Packer& data) const
{
	data << forceType;
}

void Force::Preprocess(void)
{
}

bool Force::ApplyForce(RigidBodyController *rigidBody, const Transform4D& worldTransform, Vector3D *force, Vector3D *torque)
{
	return (false);
}


GravityForce::GravityForce() : Force(kForceGravity)
{
	gravityAcceleration = 9.8F;
}

GravityForce::GravityForce(const GravityForce& gravityForce) : Force(gravityForce)
{
	gravityAcceleration = gravityForce.gravityAcceleration;
}

GravityForce::~GravityForce()
{
}

Force *GravityForce::Replicate(void) const
{
	return (new GravityForce(*this));
} 

void GravityForce::Pack(Packer& data, unsigned_int32 packFlags) const 
{ 
	Force::Pack(data, packFlags); 
	
	data << ChunkHeader('ACCL', 4); 
	data << gravityAcceleration;
	
	data << TerminatorChunk;
} 

void GravityForce::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Force::Unpack(data, unpackFlags); 
	UnpackChunkList<GravityForce>(data, unpackFlags);
}

bool GravityForce::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'ACCL':
			
			data >> gravityAcceleration;
			return (true);
	}
	
	return (false);
}

int32 GravityForce::GetSettingCount(void) const
{
	return (1);
}

Setting *GravityForce::GetSetting(int32 index) const
{
	const StringTable *table = TheInterfaceMgr->GetStringTable();
	
	if (index == 0)
	{
		const char *title = table->GetString(StringID('FORC', kForceGravity, 'ACCL'));
		return (new TextSetting('ACCL', gravityAcceleration, title));
	}
	
	return (nullptr);
}

void GravityForce::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'ACCL')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		gravityAcceleration = Text::StringToFloat(text);
	}
}

bool GravityForce::ApplyForce(RigidBodyController *rigidBody, const Transform4D& worldTransform, Vector3D *force, Vector3D *torque)
{
	*force = GetTargetField()->GetWorldTransform()[2] * (-gravityAcceleration * rigidBody->GetBodyMass() * rigidBody->GetGravityMultiplier());
	torque->Set(0.0F, 0.0F, 0.0F);
	return (true);
}


FluidForce::FluidForce() : Force(kForceFluid)
{
	fluidDensity = 1.0F;
	linearDrag = 0.25F;
	angularDrag = 0.1F;
	fluidCurrent.Set(0.0F, 0.0F, 0.0F);
	
	waterConnectorKey[0] = 0;
}

FluidForce::FluidForce(const FluidForce& fluidForce) : Force(fluidForce)
{
	fluidDensity = fluidForce.fluidDensity;
	linearDrag = fluidForce.linearDrag;
	angularDrag = fluidForce.angularDrag;
	fluidCurrent = fluidForce.fluidCurrent;
	
	waterConnectorKey = fluidForce.waterConnectorKey;
}

FluidForce::~FluidForce()
{
}

Force *FluidForce::Replicate(void) const
{
	return (new FluidForce(*this));
}

bool FluidForce::ValidField(const Field *field)
{
	FieldType type = field->GetFieldType();
	return ((type == kFieldBox) || (type == kFieldCylinder));
}

void FluidForce::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Force::Pack(data, packFlags);
	
	data << ChunkHeader('DENS', 4);
	data << fluidDensity;
	
	data << ChunkHeader('DRAG', 8);
	data << linearDrag;
	data << angularDrag;
	
	data << ChunkHeader('CRNT', sizeof(Vector3D));
	data << fluidCurrent;
	
	PackHandle handle = data.BeginChunk('WCON');
	data << waterConnectorKey;
	data.EndChunk(handle);
	
	data << TerminatorChunk;
}

void FluidForce::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Force::Unpack(data, unpackFlags);
	UnpackChunkList<FluidForce>(data, unpackFlags);
}

bool FluidForce::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'DENS':
			
			data >> fluidDensity;
			return (true);
		
		case 'DRAG':
			
			data >> linearDrag;
			data >> angularDrag;
			return (true);
		
		case 'CRNT':
			
			data >> fluidCurrent;
			return (true);
		
		case 'WCON':
			
			data >> waterConnectorKey;
			return (true);
		
		#if C4LEGACY
		
			case 'CKEY':
			{
				Type	key;
				
				data >> key;
				waterConnectorKey = Text::TypeToString(key);
				return (true);
			}
		
		#endif
	}
	
	return (false);
}

int32 FluidForce::GetSettingCount(void) const
{
	return (6);
}

Setting *FluidForce::GetSetting(int32 index) const
{
	const StringTable *table = TheInterfaceMgr->GetStringTable();
	
	if (index == 0)
	{
		const char *title = table->GetString(StringID('FORC', kForceFluid, 'DENS'));
		return (new TextSetting('DENS', fluidDensity, title));
	}
	
	if (index == 1)
	{
		const char *title = table->GetString(StringID('FORC', kForceFluid, 'LDRG'));
		return (new TextSetting('LDRG', linearDrag, title));
	}
	
	if (index == 2)
	{
		const char *title = table->GetString(StringID('FORC', kForceFluid, 'ADRG'));
		return (new TextSetting('ADRG', angularDrag, title));
	}
	
	if (index == 3)
	{
		const char *title = table->GetString(StringID('FORC', kForceFluid, 'SPED'));
		return (new TextSetting('SPED', Magnitude(fluidCurrent), title));
	}
	
	if (index == 4)
	{
		const char *title = table->GetString(StringID('FORC', kForceFluid, 'CDIR'));
		return (new TextSetting('CDIR', Atan(fluidCurrent.y, fluidCurrent.x) * K::degrees, title));
	}
	
	if (index == 5)
	{
		const char *title = table->GetString(StringID('FORC', kForceFluid, 'WCON'));
		return (new TextSetting('WCON', waterConnectorKey, title, kMaxConnectorKeyLength, &Connector::ConnectorKeyFilter));
	}
	
	return (nullptr);
}

void FluidForce::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'DENS')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		fluidDensity = FmaxZero(Text::StringToFloat(text));
	}
	else if (identifier == 'LDRG')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		linearDrag = FmaxZero(Text::StringToFloat(text));
	}
	else if (identifier == 'ADRG')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		angularDrag = FmaxZero(Text::StringToFloat(text));
	}
	else if (identifier == 'SPED')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		fluidCurrent.Set(FmaxZero(Text::StringToFloat(text)), 0.0F, 0.0F);
	}
	else if (identifier == 'CDIR')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		fluidCurrent.RotateAboutZ(Text::StringToFloat(text) * K::radians);
	}
	else if (identifier == 'WKEY')
	{
		waterConnectorKey = static_cast<const TextSetting *>(setting)->GetText();
	}
}

void FluidForce::Preprocess(void)
{
	waterBlock = nullptr;
	if (waterConnectorKey[0] != 0)
	{
		const Node *node = GetTargetField()->GetConnectedNode(waterConnectorKey);
		if ((node) && (node->GetNodeType() == kNodeWaterBlock) && (node->GetController())) waterBlock = static_cast<const WaterBlock *>(node);
	}
}

bool FluidForce::ApplyForce(RigidBodyController *rigidBody, const Transform4D& worldTransform, Vector3D *force, Vector3D *torque)
{
	Antivector4D	plane;
	Point3D			centroid;
	
	if (waterBlock)
	{
		const Transform4D& inverseTransform = waterBlock->GetInverseWorldTransform();
		float elevation = waterBlock->GetFilteredWaterElevation((inverseTransform * rigidBody->GetWorldCenterOfMass()).GetPoint2D());
		plane = Antivector4D(-inverseTransform(2,0), -inverseTransform(2,1), -inverseTransform(2,2), elevation - inverseTransform(2,3)) * worldTransform;
	}
	else
	{
		plane = GetTargetField()->GetSurfacePlane() * worldTransform;
	}
	
	float volume = rigidBody->CalculateSubmergedVolume(plane, &centroid);
	if (volume > 0.0F)
	{
		PhysicsController *physicsController = rigidBody->GetPhysicsController();
		physicsController->IncrementPhysicsCounter(kPhysicsCounterBuoyancy);
		
		Vector3D buoyantForce = physicsController->GetGravityAcceleration() * -(volume * fluidDensity);
		Vector3D buoyantTorque = worldTransform * (centroid - rigidBody->GetCenterOfMass()) % buoyantForce;
		
		Vector3D dv = GetTargetField()->GetWorldTransform() * fluidCurrent - rigidBody->GetLinearVelocity();
		const Vector3D& dw = rigidBody->GetAngularVelocity();
		
		volume *= rigidBody->GetDragMultiplier();
		
		*force = buoyantForce + dv * (Magnitude(dv) * linearDrag * volume);
		*torque = buoyantTorque - dw * (Magnitude(dw) * angularDrag * volume);
		
		if (waterBlock) rigidBody->SetSubmergedWaterBlock(waterBlock);
		return (true);
	}
	
	return (false);
}


WindForce::WindForce() : Force(kForceWind)
{
	windVelocity.Set(1.0F, 0.0F, 0.0F);
	windDrag = 1.0F;
}

WindForce::WindForce(const WindForce& windForce) : Force(windForce)
{
	windVelocity = windForce.windVelocity;
	windDrag = windForce.windDrag;
}

WindForce::~WindForce()
{
}

Force *WindForce::Replicate(void) const
{
	return (new WindForce(*this));
}

void WindForce::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Force::Pack(data, packFlags);
	
	data << ChunkHeader('VELO', sizeof(Vector3D));
	data << windVelocity;
	
	data << ChunkHeader('DRAG', 4);
	data << windDrag;
	
	data << TerminatorChunk;
}

void WindForce::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Force::Unpack(data, unpackFlags);
	UnpackChunkList<WindForce>(data, unpackFlags);
}

bool WindForce::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'VELO':
			
			data >> windVelocity;
			return (true);
		
		case 'DRAG':
			
			data >> windDrag;
			return (true);
	}
	
	return (false);
}

int32 WindForce::GetSettingCount(void) const
{
	return (2);
}

Setting *WindForce::GetSetting(int32 index) const
{
	const StringTable *table = TheInterfaceMgr->GetStringTable();
	
	if (index == 0)
	{
		const char *title = table->GetString(StringID('FORC', kForceWind, 'SPED'));
		return (new TextSetting('SPED', Magnitude(windVelocity), title));
	}
	
	if (index == 1)
	{
		const char *title = table->GetString(StringID('FORC', kForceWind, 'DRAG'));
		return (new TextSetting('DRAG', windDrag, title));
	}
	
	return (nullptr);
}

void WindForce::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'SPED')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		windVelocity.Set(FmaxZero(Text::StringToFloat(text)), 0.0F, 0.0F);
	}
	else if (identifier == 'DRAG')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		windDrag = FmaxZero(Text::StringToFloat(text));
	}
}

bool WindForce::ApplyForce(RigidBodyController *rigidBody, const Transform4D& worldTransform, Vector3D *force, Vector3D *torque)
{
	Vector3D dv = GetTargetField()->GetWorldTransform() * windVelocity - rigidBody->GetLinearVelocity();
	*force = dv * (Magnitude(dv) * windDrag * (rigidBody->GetBodyVolume() * rigidBody->GetDragMultiplier()));
	torque->Set(0.0F, 0.0F, 0.0F);
	return (true);
}

// ZYURVUR
