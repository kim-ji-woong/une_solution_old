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


#include "C4Simulation.h"
#include "C4Forces.h"
#include "C4Water.h"
#include "C4World.h"
#include "C4Terrain.h"
#include "C4Configuration.h"


using namespace C4;


namespace C4
{
	template <> Heap Memory<ShapeIntersectionJob>::heap("ShapeIntersectionJob", 65536, kHeapMutexless);
	template class Memory<ShapeIntersectionJob>;
	
	template <> Heap Memory<GeometryIntersectionJob>::heap("GeometryIntersectionJob", 65536, kHeapMutexless);
	template class Memory<GeometryIntersectionJob>;
}


unsigned_int32 (*const PhysicsController::simplexMinFunc[4])(const Point3D *, Point3D *) =
{
	&CalculateZeroSimplexMinimum, &CalculateOneSimplexMinimum, &CalculateTwoSimplexMinimum, &CalculateThreeSimplexMinimum
};


#if C4DIAGNOSTICS

	const C4::Line RigidBodyRenderable::lineArray[12] =
	{
		{{0, 1}}, {{2, 3}}, {{0, 2}}, {{1, 3}}, {{4, 5}}, {{6, 7}}, {{4, 6}}, {{5, 7}}, {{0, 4}}, {{1, 5}}, {{2, 6}}, {{3, 7}}
	};

#endif


Body::Body(BodyType type)
{
	bodyType = type;
}

Body::~Body()
{
}


RigidBodyController::RigidBodyController() :
		Controller(kControllerRigidBody),
		Body(kBodyRigid)
{
	SetBaseControllerType(kControllerRigidBody);
	SetControllerFlags(kControllerMoveInhibit);
	
	rigidBodyType = kRigidBodyGeneric;
	rigidBodyFlags = 0;
	rigidBodyState = 0;
	
	gravityMultiplier = 1.0F;
	dragMultiplier = 1.0F;
	
	restitutionCoefficient = 0.0F;
	frictionCoefficient = 0.01F;
	rollingResistance = 0.0F;
	
	collisionKind = kCollisionRigidBody;
	collisionExclusionMask = 0;
	
	maxSleepStepCount = kRigidBodySleepStepCount;
	sleepBoxSize = kRigidBodySleepBoxSize;
	
	physicsController = nullptr;
	submergedWaterBlock = nullptr;
	bodyVolume = 0.0F;
}

RigidBodyController::RigidBodyController(ControllerType type) :
		Controller(type),
		Body(kBodyRigid)
{
	SetBaseControllerType(kControllerRigidBody);
	
	rigidBodyType = kRigidBodyGeneric;
	rigidBodyFlags = 0;
	rigidBodyState = 0;
	
	gravityMultiplier = 1.0F;
	dragMultiplier = 1.0F;
	
	restitutionCoefficient = 0.0F;
	frictionCoefficient = 0.01F;
	rollingResistance = 0.0F;
	
	collisionKind = kCollisionRigidBody;
	collisionExclusionMask = 0;
	
	maxSleepStepCount = kRigidBodySleepStepCount;
	sleepBoxSize = kRigidBodySleepBoxSize;
	
	physicsController = nullptr;
	submergedWaterBlock = nullptr;
	bodyVolume = 0.0F; 
}
 
RigidBodyController::RigidBodyController(const RigidBodyController& rigidBodyController) : 
		Controller(rigidBodyController), 
		Body(kBodyRigid)
{ 
	rigidBodyType = rigidBodyController.rigidBodyType;
	rigidBodyFlags = rigidBodyController.rigidBodyFlags;
	rigidBodyState = rigidBodyController.rigidBodyState;
	 
	gravityMultiplier = rigidBodyController.gravityMultiplier;
	dragMultiplier = rigidBodyController.dragMultiplier;
	
	restitutionCoefficient = rigidBodyController.restitutionCoefficient; 
	frictionCoefficient = rigidBodyController.frictionCoefficient;
	rollingResistance = rigidBodyController.rollingResistance;
	
	collisionKind = rigidBodyController.collisionKind;
	collisionExclusionMask = rigidBodyController.collisionExclusionMask;
	
	maxSleepStepCount = rigidBodyController.maxSleepStepCount;
	sleepBoxSize = rigidBodyController.sleepBoxSize;
	
	physicsController = nullptr;
	submergedWaterBlock = nullptr;
	bodyVolume = 0.0F;
}

RigidBodyController::~RigidBodyController()
{
	shapeList.RemoveAll();
	internalShapeList.RemoveAll();
	
	#if C4DIAGNOSTICS
	
		delete rigidBodyRenderable.GetTarget();
	
	#endif
}

Controller *RigidBodyController::Replicate(void) const
{
	return (new RigidBodyController(*this));
}

void RigidBodyController::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Controller::Pack(data, packFlags);
	
	data << ChunkHeader('FLAG', 4);
	data << rigidBodyFlags;
	
	data << ChunkHeader('STAT', 4);
	data << rigidBodyState;
	
	data << ChunkHeader('GRAV', 4);
	data << gravityMultiplier;
	
	data << ChunkHeader('DRAG', 4);
	data << dragMultiplier;
	
	data << ChunkHeader('REST', 4);
	data << restitutionCoefficient;
	
	data << ChunkHeader('FRIC', 4);
	data << frictionCoefficient;
	
	data << ChunkHeader('ROLL', 4);
	data << rollingResistance;
	
	data << ChunkHeader('COLL', 8);
	data << collisionKind;
	data << collisionExclusionMask;
	
	data << ChunkHeader('SNAP', 4);
	data << GetSnapshotPeriod();
	
	data << ChunkHeader('MSLP', 4);
	data << maxSleepStepCount;
	
	data << ChunkHeader('SBSZ', 4);
	data << sleepBoxSize;
	
	if (bodyVolume != 0.0F)
	{
		data << ChunkHeader('VOLU', 4);
		data << bodyVolume;
		
		data << ChunkHeader('MASS', 8 + sizeof(Point3D) + sizeof(InertiaTensor));
		data << bodyMass;
		data << inverseBodyMass;
		data << centerOfMass;
		data << inertiaTensor;
		
		data << ChunkHeader('VELO', sizeof(Vector3D) * 2);
		data << linearVelocity;
		data << angularVelocity;
		
		data << ChunkHeader('XFRM', sizeof(Transform4D) * 2);
		data << initialNodeTransform;
		data << finalNodeTransform;
		
		data << ChunkHeader('MOVE', sizeof(Point3D) + sizeof(Vector3D) * 2 + 4);
		data << moveCenterOfMass;
		data << moveDisplacement;
		data << moveRotationAxis;
		data << moveRotationAngle;
		
		data << ChunkHeader('EXTN', sizeof(Vector3D) * 2);
		data << externalForce;
		data << externalTorque;
		
		data << ChunkHeader('LRES', sizeof(Vector3D));
		data << externalLinearResistance;
		
		data << ChunkHeader('ARES', sizeof(Vector3D));
		data << externalAngularResistance;
		
		data << ChunkHeader('IMPL', sizeof(Vector3D) * 2);
		data << impulseForce;
		data << impulseTorque;
		
		data << ChunkHeader('SLEP', 4);
		data << sleepStepCount;
		
		data << ChunkHeader('SLBX', sizeof(Point3D) * 6);
		data << centerSleepBox.min;
		data << centerSleepBox.max;
		data << axisSleepBox[0].min;
		data << axisSleepBox[0].max;
		data << axisSleepBox[1].min;
		data << axisSleepBox[1].max;
		
		if ((submergedWaterBlock) && (submergedWaterBlock->LinkedNodePackable(packFlags)))
		{
			data << ChunkHeader('WBLK', 4);
			data << submergedWaterBlock->GetNodeIndex();
		}
	}
	
	data << TerminatorChunk;
}

void RigidBodyController::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Controller::Unpack(data, unpackFlags);
	UnpackChunkList<RigidBodyController>(data, unpackFlags);
}

bool RigidBodyController::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'FLAG':
			
			data >> rigidBodyFlags;
			return (true);
		
		case 'STAT':
			
			data >> rigidBodyState;
			return (true);
		
		case 'GRAV':
			
			data >> gravityMultiplier;
			return (true);
		
		case 'DRAG':
			
			data >> dragMultiplier;
			return (true);
		
		case 'REST':
			
			data >> restitutionCoefficient;
			return (true);
		
		case 'FRIC':
			
			data >> frictionCoefficient;
			return (true);
		
		case 'ROLL':
			
			data >> rollingResistance;
			return (true);
		
		case 'COLL':
			
			data >> collisionKind;
			data >> collisionExclusionMask;
			return (true);
		
		case 'SNAP':
		{
			int32	period;
			
			data >> period;
			SetSnapshotPeriod(period);
			return (true);
		}
		
		case 'MSLP':
			
			data >> maxSleepStepCount;
			return (true);
		
		case 'SBSZ':
			
			data >> sleepBoxSize;
			return (true);
		
		case 'VOLU':
			
			data >> bodyVolume;
			return (true);
		
		case 'MASS':
			
			data >> bodyMass;
			data >> inverseBodyMass;
			data >> centerOfMass;
			data >> inertiaTensor;
			return (true);
		
		case 'VELO':
			
			data >> linearVelocity;
			data >> angularVelocity;
			return (true);
		
		case 'XFRM':
			
			data >> initialNodeTransform;
			data >> finalNodeTransform;
			return (true);
		
		case 'MOVE':
			
			data >> moveCenterOfMass;
			data >> moveDisplacement;
			data >> moveRotationAxis;
			data >> moveRotationAngle;
			return (true);
		
		case 'EXTN':
			
			data >> externalForce;
			data >> externalTorque;
			return (true);
		
		case 'LRES':
			
			data >> externalLinearResistance;
			return (true);
		
		case 'ARES':
			
			data >> externalAngularResistance;
			return (true);
		
		case 'IMPL':
			
			data >> impulseForce;
			data >> impulseTorque;
			return (true);
		
		case 'SLEP':
			
			data >> sleepStepCount;
			return (true);
		
		case 'SLBX':
			
			data >> centerSleepBox.min;
			data >> centerSleepBox.max;
			data >> axisSleepBox[0].min;
			data >> axisSleepBox[0].max;
			data >> axisSleepBox[1].min;
			data >> axisSleepBox[1].max;
			return (true);
		
		case 'WBLK':
		{
			int32	nodeIndex;
			
			data >> nodeIndex;
			data.AddNodeLink(nodeIndex, &WaterBlockLinkProc, this);
			return (true);
		}
	}
	
	return (false);
}

void RigidBodyController::WaterBlockLinkProc(Node *node, void *cookie)
{
	RigidBodyController *rigidBody = static_cast<RigidBodyController *>(cookie);
	rigidBody->submergedWaterBlock = static_cast<WaterBlock *>(node);
}

int32 RigidBodyController::GetSettingCount(void) const
{
	return (11);
}

Setting *RigidBodyController::GetSetting(int32 index) const
{
	const StringTable *table = TheInterfaceMgr->GetStringTable();
	
	if (index == 0)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerRigidBody, 'FLAG'));
		return (new HeadingSetting('FLAG', title));
	}
	
	if (index == 1)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerRigidBody, 'FLAG', 'SLEP'));
		return (new BooleanSetting('SLEP', Asleep(), title));
	}
	
	if (index == 2)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerRigidBody, 'FLAG', 'IFRC'));
		return (new BooleanSetting('IFRC', ((rigidBodyFlags & kRigidBodyForceFieldInhibit) != 0), title));
	}
	
	if (index == 3)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerRigidBody, 'PROP'));
		return (new HeadingSetting('PROP', title));
	}
	
	if (index == 4)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerRigidBody, 'PROP', 'GRAV'));
		return (new TextSetting('GRAV', gravityMultiplier, title));
	}
	
	if (index == 5)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerRigidBody, 'PROP', 'DRAG'));
		return (new TextSetting('DRAG', dragMultiplier, title));
	}
	
	if (index == 6)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerRigidBody, 'PROP', 'REST'));
		return (new FloatSetting('REST', restitutionCoefficient, title, 0.0F, 1.0F, 0.05F));
	}
	
	if (index == 7)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerRigidBody, 'PROP', 'FRIC'));
		return (new TextSetting('FRIC', frictionCoefficient, title));
	}
	
	if (index == 8)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerRigidBody, 'PROP', 'ROLL'));
		return (new TextSetting('ROLL', rollingResistance, title));
	}
	
	if (index == 9)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerRigidBody, 'NTWK'));
		return (new HeadingSetting('NTWK', title));
	}
	
	if (index == 10)
	{
		const char *title = table->GetString(StringID('CTRL', kControllerRigidBody, 'NTWK', 'SNAP'));
		return (new TextSetting('SNAP', Text::IntegerToString(GetSnapshotPeriod()), title, 3, &EditTextWidget::NumberFilter));
	}
	
	return (nullptr);
}

void RigidBodyController::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'SLEP')
	{
		unsigned_int32 flags = GetControllerFlags();
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) SetControllerFlags(flags | kControllerAsleep);
		else SetControllerFlags(flags & ~kControllerAsleep);
	}
	else if (identifier == 'IFRC')
	{
		bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
		if (b) rigidBodyFlags |= kRigidBodyForceFieldInhibit;
		else rigidBodyFlags &= ~kRigidBodyForceFieldInhibit;
	}
	else if (identifier == 'GRAV')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		gravityMultiplier = FmaxZero(Text::StringToFloat(text));
	}
	else if (identifier == 'DRAG')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		dragMultiplier = FmaxZero(Text::StringToFloat(text));
	}
	else if (identifier == 'REST')
	{
		restitutionCoefficient = static_cast<const FloatSetting *>(setting)->GetFloatValue();
	}
	else if (identifier == 'FRIC')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		frictionCoefficient = FmaxZero(Text::StringToFloat(text));
	}
	else if (identifier == 'ROLL')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		rollingResistance = FmaxZero(Text::StringToFloat(text));
	}
	else if (identifier == 'SNAP')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		SetSnapshotPeriod(Max(Text::StringToInteger(text), 1));
	}
}

void RigidBodyController::Preprocess(void)
{
	Node *node = GetTargetNode();
	node->SetNodeFlags(node->GetNodeFlags() | (kNodeDynamicVisibility | kNodeVisibilitySite));
	
	if (!node->GetManipulator())
	{
		if (Asleep()) rigidBodyState |= kRigidBodyAsleep;
		
		Node *subnode = node;
		do
		{
			if (subnode->GetNodeType() == kNodeGeometry)
			{
				GeometryObject *object = static_cast<Geometry *>(subnode)->GetObject();
				object->SetGeometryFlags(object->GetGeometryFlags() | kGeometryDynamic);
			}
			
			subnode = node->GetNextNode(subnode);
		} while (subnode);
		
		subnode = node->GetFirstSubnode();
		while (subnode)
		{
			if (subnode->GetNodeType() == kNodeShape)
			{
				Shape *shape = static_cast<Shape *>(subnode);
				shapeList.Append(shape);
			}
			
			subnode = subnode->Next();
		}
		
		if (bodyVolume == 0.0F)
		{
			bodyMass = 0.0F;
			centerOfMass.Set(0.0F, 0.0F, 0.0F);
			
			Shape *shape = shapeList.First();
			while (shape)
			{
				const ShapeObject *shapeObject = shape->GetObject();
				
				float volume = shapeObject->CalculateVolume();
				float density = shapeObject->GetShapeDensity();
				float mass = volume * density;
				bodyVolume += volume * NonzeroFsgn(density);
				bodyMass += mass;
				
				Point3D center = shapeObject->CalculateCenterOfMass();
				centerOfMass += shape->GetNodeTransform() * center * mass;
				
				shape = shape->Next();
			}
			
			bodyMass = Fmax(bodyMass, kMinRigidBodyMass);
			inverseBodyMass = 1.0F / bodyMass;
			centerOfMass *= inverseBodyMass;
			
			inertiaTensor.Set(0.0F, 0.0F, 0.0F, 0.0F, 0.0F, 0.0F);
			
			shape = shapeList.First();
			while (shape)
			{
				Shape *next = shape->Next();
				
				const ShapeObject *shapeObject = shape->GetObject();
				if (shapeObject->GetShapeFlags() & kShapeCollisionInhibit) internalShapeList.Append(shape);
				
				InertiaTensor inertia = shapeObject->CalculateInertiaTensor();
				const Transform4D& transform = shape->GetNodeTransform();
				
				inertia = Rotate(inertia, transform);
				inertia = Translate(inertia, centerOfMass - transform * shapeObject->CalculateCenterOfMass(), shapeObject->CalculateMass());
				inertiaTensor += inertia;
				
				shape = next;
			}
			
			linearVelocity.Set(0.0F, 0.0F, 0.0F);
			angularVelocity.Set(0.0F, 0.0F, 0.0F);
			
			moveDisplacement.Set(0.0F, 0.0F, 0.0F);
			moveRotationAxis.Set(0.0F, 0.0F, 1.0F);
			moveRotationAngle = 0.0F;
			
			initialNodeTransform = node->GetNodeTransform();
			finalNodeTransform = node->GetNodeTransform();
			
			externalForce.Set(0.0F, 0.0F, 0.0F);
			externalTorque.Set(0.0F, 0.0F, 0.0F);
			externalLinearResistance.Set(0.0F, 0.0F, 0.0F);
			externalAngularResistance = 0.0F;
			
			impulseForce.Set(0.0F, 0.0F, 0.0F);
			impulseTorque.Set(0.0F, 0.0F, 0.0F);
			
			sleepStepCount = 0;
			centerSleepBox.min = initialNodeTransform * centerOfMass;
			centerSleepBox.max = centerSleepBox.min;
			axisSleepBox[0].min = centerSleepBox.min + initialNodeTransform[0];
			axisSleepBox[0].max = axisSleepBox[0].min;
			axisSleepBox[1].min = centerSleepBox.min + initialNodeTransform[1];
			axisSleepBox[1].max = axisSleepBox[1].min;
		}
		
		Shape *shape = shapeList.First();
		if (shape)
		{
			const World *world = node->GetWorld();
			if (world)
			{
				PhysicsNode *physicsNode = world->GetRootNode()->GetPhysicsNode();
				if (physicsNode)
				{
					Controller *controller = physicsNode->GetController();
					if ((controller) && (controller->GetControllerType() == kControllerPhysics))
					{
						physicsController = static_cast<PhysicsController *>(controller);
						physicsController->AddRigidBody(this);
						
						node->SetActiveUpdateFlags(node->GetActiveUpdateFlags() | Node::kUpdateVisibility);
					}
				}
			}
			
			shape->CalculateBoundingBox(&boundingBox);
			boundingBox.Transform(shape->GetNodeTransform());
			
			for (;;)
			{
				Box3D	shapeBounds;
				
				shape = shape->Next();
				if (!shape) break;
				
				shape->CalculateBoundingBox(&shapeBounds);
				shapeBounds.Transform(shape->GetNodeTransform());
				boundingBox.Union(shapeBounds);
			}
			
			Vector3D p1 = boundingBox.min - centerOfMass;
			Vector3D p2 = boundingBox.max - centerOfMass;
			
			float x = Fmax(Fabs(p1.x), Fabs(p2.x));
			float y = Fmax(Fabs(p1.y), Fabs(p2.y));
			float z = Fmax(Fabs(p1.z), Fabs(p2.z));
			boundingRadius = Sqrt(x * x + y * y + z * z);
			
			Transform4D worldTransform = node->GetSuperNode()->GetWorldTransform() * node->GetNodeTransform();
			initialWorldTransform = worldTransform;
			finalWorldTransform = worldTransform;
			
			bodyCollisionBox = Transform(boundingBox, worldTransform);
			moveCenterOfMass = node->GetNodeTransform() * centerOfMass;
		}
		else
		{
			boundingBox.min.Set(0.0F, 0.0F, 0.0F);
			boundingBox.max.Set(0.0F, 0.0F, 0.0F);
			boundingRadius = 0.0F;
		}
		
		worldMoveTransform.SetIdentity();
		
		networkDelta[0].Set(0.0F, 0.0F, 0.0F);
		networkDelta[1].Set(0.0F, 0.0F, 0.0F);
		networkDecay[0] = 0.0F;
		networkDecay[1] = 0.0F;
		networkParity = 0;
	}
	
	if (collisionKind == 0) collisionKind = kCollisionRigidBody;
	
	Controller::Preprocess();
}

void RigidBodyController::Neutralize(void)
{
	if (physicsController)
	{
		physicsController->RemoveRigidBody(this);
		physicsController = nullptr;
	}
	
	Controller::Neutralize();
}

void RigidBodyController::ChangeZones(Zone *zone, const Transform4D& transform)
{
	initialNodeTransform = transform * initialNodeTransform;
	finalNodeTransform = transform * finalNodeTransform;
	
	moveCenterOfMass = transform * moveCenterOfMass;
	moveDisplacement = transform * moveDisplacement;
	moveRotationAxis = transform * moveRotationAxis;
}

void RigidBodyController::Wake(void)
{
	Controller::Wake();
	
	if ((physicsController) && (shapeList.First()))
	{
		sleepStepCount = 0;
		physicsController->WakeRigidBody(this);
		
		if ((!(rigidBodyFlags & kRigidBodyLocalSimulation)) && (TheMessageMgr->Server()))
		{
			TheMessageMgr->AddSnapshotSender(this);
			TheMessageMgr->SendMessageClients(ControllerMessage(kRigidBodyMessageWake, GetControllerIndex()));
		}
		
		Node *node = GetTargetNode();
		Node *subnode = node;
		do
		{
			NodeType type = subnode->GetNodeType();
			if (type == kNodeGeometry)
			{
				Geometry *geometry = static_cast<Geometry *>(subnode);
				if (geometry->GetGeometryType() == kGeometryMesh) geometry->SetRenderableFlags(geometry->GetRenderableFlags() | kRenderableMotionBlurGradient);
			}
			
			subnode = node->GetNextNode(subnode);
		} while (subnode);
	}
}

void RigidBodyController::Sleep(void)
{
	SetRigidBodyTransform(finalNodeTransform);
	
	bodyCollisionBox = Transform(boundingBox, finalWorldTransform);
	linearVelocity.Set(0.0F, 0.0F, 0.0F);
	angularVelocity.Set(0.0F, 0.0F, 0.0F);
	
	if (physicsController)
	{
		physicsController->SleepRigidBody(this);
		
		if ((!(rigidBodyFlags & kRigidBodyLocalSimulation)) && (TheMessageMgr->Server()))
		{
			TheMessageMgr->RemoveSnapshotSender(this);
			TheMessageMgr->SendMessageClients(RigidBodySleepMessage(GetControllerIndex(), finalNodeTransform.GetTranslation(), Quaternion().SetRotationMatrix(finalNodeTransform)));
		}
	}
	
	if (!(rigidBodyFlags & kRigidBodyPartialSleep)) Controller::Sleep();
	
	Node *node = GetTargetNode();
	node->Invalidate();
	
	Node *subnode = node;
	do
	{
		NodeType type = subnode->GetNodeType();
		if (type == kNodeGeometry)
		{
			Geometry *geometry = static_cast<Geometry *>(subnode);
			if (geometry->GetGeometryType() == kGeometryMesh) geometry->SetRenderableFlags(geometry->GetRenderableFlags() & ~kRenderableMotionBlurGradient);
		}
		
		subnode = node->GetNextNode(subnode);
	} while (subnode);
	
	#if C4DIAGNOSTICS
	
		delete rigidBodyRenderable.GetTarget();
	
	#endif
}

void RigidBodyController::RecursiveWake(bool applyForces)
{
	if (TheMessageMgr->Server())
	{
		ExpediteSnapshot();
		
		if (RigidBodyAsleep())
		{
			Wake();
			
			if (applyForces)
			{
				CalculateAppliedForces(physicsController->gravityAcceleration);
				DetectWorldCollisions();
			}
			
			const Contact *contact = GetFirstOutgoingEdge();
			while (contact)
			{
				Body *body = contact->GetFinishElement();
				if (body->GetBodyType() == kBodyRigid) static_cast<RigidBodyController *>(body)->RecursiveWake(applyForces);
				
				contact = contact->GetNextOutgoingEdge();
			}
			
			contact = GetFirstIncomingEdge();
			while (contact)
			{
				Body *body = contact->GetStartElement();
				if (body->GetBodyType() == kBodyRigid) static_cast<RigidBodyController *>(body)->RecursiveWake(applyForces);
				
				contact = contact->GetNextIncomingEdge();
			}
		}
		else
		{
			sleepStepCount = 0;
		}
	}
}

void RigidBodyController::RecursiveKeepAwake(void)
{
	sleepStepCount = Min(sleepStepCount, maxSleepStepCount - 1);
	
	const Contact *contact = GetFirstOutgoingEdge();
	while (contact)
	{
		Body *body = contact->GetFinishElement();
		if (body->GetBodyType() == kBodyRigid)
		{
			RigidBodyController *rigidBody = static_cast<RigidBodyController *>(body);
			if (rigidBody->sleepStepCount >= rigidBody->maxSleepStepCount) rigidBody->RecursiveKeepAwake();
		}
		
		contact = contact->GetNextOutgoingEdge();
	}
	
	contact = GetFirstIncomingEdge();
	while (contact)
	{
		Body *body = contact->GetStartElement();
		if (body->GetBodyType() == kBodyRigid)
		{
			RigidBodyController *rigidBody = static_cast<RigidBodyController *>(body);
			if (rigidBody->sleepStepCount >= rigidBody->maxSleepStepCount) rigidBody->RecursiveKeepAwake();
		}
		
		contact = contact->GetNextIncomingEdge();
	}
}

ControllerMessage *RigidBodyController::ConstructMessage(ControllerMessageType type) const
{
	switch (type)
	{
		case kRigidBodyMessageSnapshot:
			
			return (new RigidBodySnapshotMessage(GetControllerIndex()));
		
		case kRigidBodyMessageWake:
			
			return (new ControllerMessage(kRigidBodyMessageWake, GetControllerIndex()));
		
		case kRigidBodyMessageSleep:
			
			return (new RigidBodySleepMessage(GetControllerIndex()));
	}
	
	return (Controller::ConstructMessage(type));
}

void RigidBodyController::ReceiveMessage(const ControllerMessage *message)
{
	switch (message->GetControllerMessageType())
	{
		case kRigidBodyMessageSnapshot:
		{
			const RigidBodySnapshotMessage *m = static_cast<const RigidBodySnapshotMessage *>(message);
			
			Transform4D serverTransform(m->GetRigidBodyRotation().GetRotationMatrix(), m->GetRigidBodyPosition());
			//Transform4D differenceTransform = serverTransform * Adjugate(finalWorldTransform);
			
			unsigned_int32 parity = networkParity;
			networkDelta[parity] = serverTransform.GetTranslation() - finalWorldTransform.GetTranslation();
			networkDecay[parity] = 1.0F;
			
			initialWorldTransform.SetMatrix3D(serverTransform);
			initialWorldTransform[3] += networkDelta[parity];
			finalWorldTransform = serverTransform;
			
			networkParity = parity ^ 1;
			
			Node *node = GetTargetNode();
			const Node *super = node->GetSuperNode();
			
			const Transform4D& inverseTransform = super->GetInverseWorldTransform();
			initialNodeTransform = inverseTransform * initialWorldTransform;
			finalNodeTransform = inverseTransform * finalWorldTransform;
			
			linearVelocity = m->GetRigidBodyLinearVelocity();
			angularVelocity = m->GetRigidBodyAngularVelocity();
			
			moveCenterOfMass = initialNodeTransform * centerOfMass;
			moveDisplacement = inverseTransform * (linearVelocity * kTimeStep);
			
			float w = SquaredMag(angularVelocity);
			if (w > K::min_float)
			{
				float r = InverseSqrt(w);
				moveRotationAxis = (angularVelocity * r) * super->GetWorldTransform();
				moveRotationAngle = w * r * kTimeStep;
			}
			else
			{
				moveRotationAxis.Set(0.0F, 0.0F, 1.0F);
				moveRotationAngle = 0.0F;
			}
			
			break;
		}
		
		case kRigidBodyMessageWake:
			
			Wake();
			break;
		
		case kRigidBodyMessageSleep:
		{
			const RigidBodySleepMessage *m = static_cast<const RigidBodySleepMessage *>(message);
			
			finalNodeTransform.Set(m->GetRigidBodyRotation().GetRotationMatrix(), m->GetRigidBodyPosition());
			Sleep();
			break;
		}
		
		default:
			
			Controller::ReceiveMessage(message);
			break;
	}
}

void RigidBodyController::SendInitialStateMessages(Player *player) const
{
	Quaternion	rotation;
	
	const Point3D& position = finalNodeTransform.GetTranslation();
	rotation.SetRotationMatrix(finalNodeTransform);
	
	if (RigidBodyAsleep())
	{
		player->SendMessage(RigidBodySleepMessage(GetControllerIndex(), position, rotation));
	}
	else
	{
		player->SendMessage(ControllerMessage(kRigidBodyMessageWake, GetControllerIndex()));
		player->SendMessage(RigidBodySnapshotMessage(GetControllerIndex(), position, rotation, linearVelocity, angularVelocity));
	}
}

void RigidBodyController::SendSnapshot(void)
{
	RigidBodySnapshotMessage message(GetControllerIndex(), finalWorldTransform.GetTranslation(), Quaternion().SetRotationMatrix(finalWorldTransform), linearVelocity, angularVelocity);
	message.SetMessageFlags(kMessageUnreliable);
	TheMessageMgr->SendMessageClients(message);
}

RigidBodyContact *RigidBodyController::FindOutgoingBodyContact(const RigidBodyController *rigidBody, unsigned_int32 startSignature, unsigned_int32 finishSignature) const
{
	Contact *contact = GetFirstOutgoingEdge();
	while (contact)
	{
		if ((contact->GetFinishElement() == rigidBody) && (contact->GetContactType() == kContactRigidBody))
		{
			RigidBodyContact *bodyContact = static_cast<RigidBodyContact *>(contact);
			if ((bodyContact->GetStartSignature() == startSignature) && (bodyContact->GetFinishSignature() == finishSignature)) return (bodyContact);
		}
		
		contact = contact->GetNextOutgoingEdge();
	}
	
	return (nullptr);
}

RigidBodyContact *RigidBodyController::FindIncomingBodyContact(const RigidBodyController *rigidBody, unsigned_int32 startSignature, unsigned_int32 finishSignature) const
{
	Contact *contact = GetFirstIncomingEdge();
	while (contact)
	{
		if ((contact->GetStartElement() == rigidBody) && (contact->GetContactType() == kContactRigidBody))
		{
			RigidBodyContact *bodyContact = static_cast<RigidBodyContact *>(contact);
			if ((bodyContact->GetStartSignature() == startSignature) && (bodyContact->GetFinishSignature() == finishSignature)) return (bodyContact);
		}
		
		contact = contact->GetNextIncomingEdge();
	}
	
	return (nullptr);
}

bool RigidBodyController::FindGeometryContact(const Geometry *geometry, unsigned_int32 signature, GeometryContact **matchingContact) const
{
	bool result = false;
	
	Contact *contact = GetFirstOutgoingEdge();
	while (contact)
	{
		if (contact->GetContactType() == kContactGeometry)
		{
			GeometryContact *geometryContact = static_cast<GeometryContact *>(contact);
			if (geometryContact->GetContactGeometry() == geometry)
			{
				if (geometryContact->GetContactSignature() == signature)
				{
					*matchingContact = geometryContact;
					return (true);
				}
				
				result = true;
			}
		}
		
		contact = contact->GetNextOutgoingEdge();
	}
	
	*matchingContact = nullptr;
	return (result);
}

inline void RigidBodyController::AdjustDisplacement(float t)
{
	finalWorldTransform[3] -= moveDisplacement * (1.0F - t);
	moveDisplacement *= t;
	moveCenterOfMass = worldCenterOfMass + moveDisplacement;
}

void RigidBodyController::GeometryCollisionJob(Job *job, void *cookie)
{
	GeometryIntersectionJob *geometryJob = static_cast<GeometryIntersectionJob *>(job);
	if( geometryJob == NULL)
		return;

	const Geometry *geometry = geometryJob->geometryNode;
	const GeometryObject *object = geometry->GetObject();
	
	if (geometryJob->rigidBody->FindGeometryContact(geometry, geometryJob->shapeIndex, &geometryJob->geometryContact))
	{
		if (object->GetConvexPrimitiveFlag())
		{
			if (geometryJob->shapeNode->StaticIntersectPrimitive(geometry, geometryJob->geometryContact, &geometryJob->intersectionData[0]))
			{
				if (geometryJob->geometryContact) geometryJob->SetFinalizeProc(&FinalizeExistingStaticGeometrySingleContact);
				else geometryJob->SetFinalizeProc(&FinalizeNewStaticGeometrySingleContact);
			}
		}
		else if (object->GetGeometryFlags() & kGeometryConvexHull)
		{
			if (geometryJob->shapeNode->StaticIntersectConvexHull(geometry, geometryJob->geometryContact, &geometryJob->intersectionData[0]))
			{
				if (geometryJob->geometryContact) geometryJob->SetFinalizeProc(&FinalizeExistingStaticGeometrySingleContact);
				else geometryJob->SetFinalizeProc(&FinalizeNewStaticGeometrySingleContact);
			}
		}
		else
		{
			GeometryContact *geometryContact = geometryJob->geometryContact;
			if (geometryContact)
			{
				unsigned_int32	triangleIndex[Contact::kMaxSubcontactCount];
				
				int32 triangleCount = geometryContact->GetSubcontactCount();
				for (machine a = 0; a < triangleCount; a++) triangleIndex[a] = geometryContact->GetSubcontact(a)->triangleIndex;
				
				int32 contactCount = geometryJob->shapeNode->MixedIntersectGeometry(geometry, geometryJob->rigidBody->moveDisplacement, triangleCount, triangleIndex, geometryJob->intersectionData);
				if (contactCount != 0)
				{
					geometryJob->contactCount = contactCount;
					geometryJob->SetFinalizeProc(&FinalizeExistingStaticGeometryMultipleContact);
				}
			}
			else
			{
				int32 contactCount = geometryJob->shapeNode->StaticIntersectGeometry(geometry, geometryJob->intersectionData);
				if (contactCount != 0)
				{
					geometryJob->contactCount = contactCount;
					geometryJob->SetFinalizeProc(&FinalizeNewStaticGeometryMultipleContact);
				}
			}
		}
	}
	else
	{
		if (object->GetConvexPrimitiveFlag())
		{
			if (geometryJob->shapeNode->DynamicIntersectPrimitive(geometry, geometryJob->rigidBody->moveDisplacement, &geometryJob->intersectionData[0]))
			{
				geometryJob->SetFinalizeProc(&FinalizeNewDynamicGeometrySingleContact);
			}
		}
		else if (object->GetGeometryFlags() & kGeometryConvexHull)
		{
			if (geometryJob->shapeNode->DynamicIntersectConvexHull(geometry, geometryJob->rigidBody->moveDisplacement, &geometryJob->intersectionData[0]))
			{
				geometryJob->SetFinalizeProc(&FinalizeNewDynamicGeometrySingleContact);
			}
		}
		else
		{
			int32 contactCount = geometryJob->shapeNode->DynamicIntersectGeometry(geometry, geometryJob->rigidBody->moveDisplacement, geometryJob->intersectionData);
			if (contactCount != 0)
			{
				geometryJob->contactCount = contactCount;
				geometryJob->SetFinalizeProc(&FinalizeNewDynamicGeometryMultipleContact);
			}
		}
	}
}

void RigidBodyController::FinalizeExistingStaticGeometrySingleContact(Job *job, void *cookie)
{
	const GeometryIntersectionJob *geometryJob = static_cast<GeometryIntersectionJob *>(job);
	geometryJob->geometryContact->UpdateContact(&geometryJob->intersectionData[0]);
}

void RigidBodyController::FinalizeNewStaticGeometrySingleContact(Job *job, void *cookie)
{
	const GeometryIntersectionJob *geometryJob = static_cast<GeometryIntersectionJob *>(job);
	new GeometryContact(geometryJob->geometryNode, geometryJob->rigidBody, &geometryJob->intersectionData[0], geometryJob->shapeIndex);
}

void RigidBodyController::FinalizeExistingStaticGeometryMultipleContact(Job *job, void *cookie)
{
	const GeometryIntersectionJob *geometryJob = static_cast<GeometryIntersectionJob *>(job);
	for (machine a = 0; a < geometryJob->contactCount; a++) geometryJob->geometryContact->UpdateContact(&geometryJob->intersectionData[a]);
}

void RigidBodyController::FinalizeNewStaticGeometryMultipleContact(Job *job, void *cookie)
{
	const GeometryIntersectionJob *geometryJob = static_cast<GeometryIntersectionJob *>(job);
	GeometryContact *geometryContact = new GeometryContact(geometryJob->geometryNode, geometryJob->rigidBody, geometryJob->intersectionData, geometryJob->shapeIndex);
	for (machine a = 1; a < geometryJob->contactCount; a++) geometryContact->UpdateContact(&geometryJob->intersectionData[a]);
}

void RigidBodyController::FinalizeNewDynamicGeometrySingleContact(Job *job, void *cookie)
{
	const GeometryIntersectionJob *geometryJob = static_cast<GeometryIntersectionJob *>(job);
	
	float param = geometryJob->intersectionData[0].contactParam;
	if (param < 1.0F)
	{
		geometryJob->rigidBody->AdjustDisplacement(param);
		geometryJob->rigidBody->RemoveLaterContacts(param);
	}
	
	new GeometryContact(geometryJob->geometryNode, geometryJob->rigidBody, &geometryJob->intersectionData[0], geometryJob->shapeIndex);
}

void RigidBodyController::FinalizeNewDynamicGeometryMultipleContact(Job *job, void *cookie)
{
	const GeometryIntersectionJob *geometryJob = static_cast<GeometryIntersectionJob *>(job);
	
	float param = geometryJob->intersectionData[0].contactParam;
	for (machine a = 1; a < geometryJob->contactCount; a++) param = Fmin(param, geometryJob->intersectionData[a].contactParam);
	
	if (param < 1.0F)
	{
		geometryJob->rigidBody->AdjustDisplacement(param);
		geometryJob->rigidBody->RemoveLaterContacts(param);
	}
	
	GeometryContact *geometryContact = new GeometryContact(geometryJob->geometryNode, geometryJob->rigidBody, geometryJob->intersectionData, geometryJob->shapeIndex);
	for (machine a = 1; a < geometryJob->contactCount; a++) geometryContact->UpdateContact(&geometryJob->intersectionData[a]);
}

void RigidBodyController::DetectGeometryCollision(Geometry *geometry, const Point3D& p1, const Point3D& p2)
{
	const GeometryObject *object = geometry->GetObject();
	if ((object->GetCollisionOctree()) && (ValidGeometryCollision(geometry)))
	{
		const Transform4D& inverseTransform = geometry->GetInverseWorldTransform();
		Point3D q1 = inverseTransform * p1;
		Point3D q2 = inverseTransform * p2;
		
		if (!object->ExteriorSweptSphere(q1, q2, boundingRadius))
		{
			unsigned_int32 shapeIndex = 0;
			Shape *shape = shapeList.First();
			while (shape)
			{
				physicsController->IncrementPhysicsCounter(kPhysicsCounterGeometryIntersection);
				
				GeometryIntersectionJob *job = new GeometryIntersectionJob(&GeometryCollisionJob, this, shape, shapeIndex, geometry);
				TheJobMgr->SubmitJob(job, &physicsController->collisionBatch);
				
				shapeIndex++;
				shape = shape->Next();
			}
		}
	}
}

void RigidBodyController::DetectNodeCollision(Node *node, List<Geometry> *geometryList, const Point3D& p1, const Point3D& p2)
{
	if (node->Enabled())
	{
		if (node->GetNodeType() == kNodeGeometry)
		{
			Geometry *geometry = static_cast<Geometry *>(node);
			if (!geometry->ListElement<Geometry>::GetOwningList())
			{
				geometryList->Append(geometry);
				DetectGeometryCollision(geometry, p1, p2);
			}
		}
		
		const Bond *bond = node->GetFirstOutgoingEdge();
		while (bond)
		{
			Site *site = bond->GetFinishElement();
			if (site->GetWorldBoundingBox().Intersection(bodyCollisionBox))
			{
				DetectNodeCollision(static_cast<Node *>(site), geometryList, p1, p2);
			}
			
			bond = bond->GetNextOutgoingEdge();
		}
	}
}

void RigidBodyController::DetectCellCollision(Site *cell, List<Geometry> *geometryList, const Point3D& p1, const Point3D& p2)
{
	const Bond *bond = cell->GetFirstOutgoingEdge();
	while (bond)
	{
		Site *site = bond->GetFinishElement();
		if (site->GetWorldBoundingBox().Intersection(bodyCollisionBox))
		{
			if (site->GetCellIndex() < 0)
			{
				Node *node = static_cast<Node *>(site);
				NodeType type = node->GetNodeType();
				if ((type & 0x00FFFFFF) != 0x00424C4B)
				{
					DetectNodeCollision(node, geometryList, p1, p2);
				}
				else
				{
					if ((type == kNodeTerrainBlock) && (node->Enabled()))
					{
						Node *subnode = node->GetFirstSubnode();
						while (subnode)
						{
							if (subnode->GetNodeType() == kNodeGeometry)
							{
								if (subnode->GetWorldBoundingBox().Intersection(bodyCollisionBox))
								{
									DetectGeometryCollision(static_cast<Geometry *>(subnode), p1, p2);
								}
								
								subnode = node->GetNextNode(subnode);
								continue;
							}
							
							subnode = node->GetNextLevelNode(subnode);
						}
					}
				}
			}
			else
			{
				DetectCellCollision(site, geometryList, p1, p2);
			}
		}
		
		bond = bond->GetNextOutgoingEdge();
	}
}

void RigidBodyController::DetectZoneCollision(Zone *zone, List<Geometry> *geometryList, const Point3D& p1, const Point3D& p2)
{
	const Transform4D& transform = zone->GetInverseWorldTransform();
	if (!zone->GetObject()->ExteriorSweptSphere(transform * p1, transform * p2, boundingRadius))
	{
		DetectCellCollision(zone, geometryList, p1, p2);
		
		Zone *subzone = zone->GetFirstSubzone();
		while (subzone)
		{
			DetectCellCollision(subzone, geometryList, p1, p2);
			subzone = subzone->Next();
		}
	}
}

void RigidBodyController::DetectWorldCollisions(void)
{
	List<Geometry>		geometryList;
	
	initialLinearVelocity = linearVelocity;
	initialAngularVelocity = angularVelocity;
	
	linearCorrection.Set(0.0F, 0.0F, 0.0F);
	angularCorrection.Set(0.0F, 0.0F, 0.0F);
	
	bodyCollisionBox = Union(Transform(boundingBox, initialWorldTransform), Transform(boundingBox, finalWorldTransform));
	
	#if C4DIAGNOSTICS
	
		World *world = GetTargetNode()->GetWorld();
		if (world->GetDiagnosticFlags() & kDiagnosticRigidBodies)
		{
			RigidBodyRenderable *renderable = rigidBodyRenderable;
			if (renderable)
			{
				renderable->SetCollisionBox(bodyCollisionBox);
			}
			else
			{
				renderable = new RigidBodyRenderable(bodyCollisionBox);
				rigidBodyRenderable = renderable;
				world->AddRigidBodyRenderable(renderable);
			}
		}
	
	#endif
	
	Transform4D inverseMoveTransform = Adjugate(worldMoveTransform);
	
	Shape *shape = shapeList.First();
	while (shape)
	{
		shape->CalculateCollisionBox(initialWorldTransform, finalWorldTransform);
		shape->SetInverseMoveTransform(inverseMoveTransform);
		
		shape = shape->Next();
	}
	
	if (inverseBodyMass > K::min_float)
	{
		const Node *node = GetTargetNode();
		Zone *zone = node->GetOwningZone();
		
		if (!zone->GetFirstSubzone())
		{
			const ZoneObject *object = zone->GetObject();
			if (!(object->GetZoneFlags() & kZoneTransition))
			{
				const Transform4D& transform = zone->GetInverseWorldTransform();
				if (object->InteriorSweptSphere(transform * worldCenterOfMass, transform * moveCenterOfMass, boundingRadius))
				{
					DetectCellCollision(zone, &geometryList, worldCenterOfMass, moveCenterOfMass);
					goto end;
				}
			}
		}
		
		DetectZoneCollision(node->GetWorld()->GetRootNode(), &geometryList, worldCenterOfMass, moveCenterOfMass);
	}
	
	end:
	geometryList.RemoveAll();
}

void RigidBodyController::ApplyCellForceFields(Site *cell, const Box3D& box, unsigned_int32 fieldStamp)
{
	const Bond *bond = cell->GetFirstOutgoingEdge();
	while (bond)
	{
		Site *site = bond->GetFinishElement();
		if (site->GetWorldBoundingBox().Intersection(box))
		{
			if (site->GetCellIndex() < 0)
			{
				Field *field = static_cast<Field *>(site);
				
				unsigned_int32 stamp = fieldStamp;
				if (field->GetNodeStamp() != stamp)
				{
					field->SetNodeStamp(stamp);
					
					if ((field->Enabled()) && (!field->GetObject()->ExteriorSphere(field->GetInverseWorldTransform() * worldCenterOfMass, boundingRadius)))
					{
						Vector3D	force, torque;
						
						if (field->GetForce()->ApplyForce(this, initialWorldTransform, &force, &torque))
						{
							appliedForce += force;
							appliedTorque += torque;
						}
					}
				}
			}
			else
			{
				ApplyCellForceFields(site, box, fieldStamp);
			}
		}
		
		bond = bond->GetNextOutgoingEdge();
	}
}

void RigidBodyController::CalculateAppliedForces(const Vector3D& gravity)
{
	originalLinearVelocity = linearVelocity;
	originalAngularVelocity = angularVelocity;
	
	Node *node = GetTargetNode();
	const Node *super = node->GetSuperNode();
	initialWorldTransform = super->GetWorldTransform() * finalNodeTransform;
	worldCenterOfMass = initialWorldTransform * centerOfMass;
	
	appliedForce = gravity * (bodyMass * gravityMultiplier) + externalForce - (linearVelocity & externalLinearResistance) + initialWorldTransform * impulseForce;
	appliedTorque = externalTorque + initialWorldTransform * impulseTorque;
	
	impulseForce.Set(0.0F, 0.0F, 0.0F);
	impulseTorque.Set(0.0F, 0.0F, 0.0F);
	
	const WaterBlock *waterBlock = submergedWaterBlock;
	submergedWaterBlock = nullptr;
	
	if (!(rigidBodyFlags & kRigidBodyForceFieldInhibit))
	{
		unsigned_int32 fieldStamp = physicsController->IncrementFieldStamp();
		ApplyCellForceFields(node->GetOwningZone()->GetFieldSite(), Transform(boundingBox, initialWorldTransform), fieldStamp);
	}
	
	if (rigidBodyFlags & kRigidBodyFixedOrientation) worldInverseInertiaTensor.Set(0.0F, 0.0F, 0.0F);
	else worldInverseInertiaTensor = Inverse(Rotate(inertiaTensor, initialWorldTransform));
	
	linearVelocity += appliedForce * (inverseBodyMass * kTimeStep);
	angularVelocity += (worldInverseInertiaTensor * appliedTorque - angularVelocity * (rollingResistance + externalAngularResistance)) * kTimeStep;
	
	Integrate();
	
	if ((submergedWaterBlock != waterBlock) && (submergedWaterBlock)) HandleWaterSubmergence();
}

void RigidBodyController::Integrate(void)
{
	moveDisplacement = linearVelocity * kTimeStep;
	moveCenterOfMass = worldCenterOfMass + moveDisplacement;
	
	float w = SquaredMag(angularVelocity);
	if (w > K::min_float)
	{
		Matrix3D	rotation;
		
		float r = InverseSqrt(w);
		moveRotationAxis = angularVelocity * r;
		moveRotationAngle = w * r * kTimeStep;
		rotation.SetRotationAboutAxis(moveRotationAngle, moveRotationAxis);
		
		worldMoveTransform.Set(rotation, moveCenterOfMass - rotation * worldCenterOfMass);
		finalWorldTransform = worldMoveTransform * initialWorldTransform;
	}
	else
	{
		moveRotationAxis.Set(0.0F, 0.0F, 1.0F);
		moveRotationAngle = 0.0F;
		
		worldMoveTransform.SetDisplacement(moveDisplacement);
		finalWorldTransform.Set(initialWorldTransform[0], initialWorldTransform[1], initialWorldTransform[2], initialWorldTransform.GetTranslation() + moveDisplacement);
	}
}

void RigidBodyController::Constrain(void)
{
	Contact *contact = GetFirstOutgoingEdge();
	while (contact)
	{
		Contact *next = contact->GetNextOutgoingEdge();
		if (contact->Enabled()) contact->ApplyConstraints();
		contact = next;
	}
}

void RigidBodyController::Finalize(void)
{
	const Node *super = GetTargetNode()->GetSuperNode();
	worldMoveTransform = super->GetInverseWorldTransform() * worldMoveTransform * super->GetWorldTransform();
	
	initialNodeTransform = finalNodeTransform;
	finalNodeTransform = worldMoveTransform * initialNodeTransform;
	
	moveCenterOfMass = initialNodeTransform * centerOfMass;
	moveDisplacement = super->GetInverseWorldTransform() * moveDisplacement;
	moveRotationAxis = moveRotationAxis * super->GetWorldTransform();
	
	Contact *contact = GetFirstOutgoingEdge();
	while (contact)
	{
		if (contact->GetNotificationFlag(kContactNotificationOutgoing))
		{
			RigidBodyStatus		status;
			
			if (contact->GetContactType() == kContactRigidBody)
			{
				RigidBodyContact *rigidBodyContact = static_cast<RigidBodyContact *>(contact);
				status = HandleNewRigidBodyContact(rigidBodyContact, static_cast<RigidBodyController *>(rigidBodyContact->GetFinishElement()));
			}
			else
			{
				status = HandleNewGeometryContact(static_cast<GeometryContact *>(contact));
			}
			
			if (status != kRigidBodyUnchanged)
			{
				if (status == kRigidBodyDestroyed) return;
				break;
			}
		}
		
		contact = contact->GetNextOutgoingEdge();
	}
	
	contact = GetFirstIncomingEdge();
	while (contact)
	{
		if (contact->GetNotificationFlag(kContactNotificationIncoming))
		{
			RigidBodyContact *rigidBodyContact = static_cast<RigidBodyContact *>(contact);
			RigidBodyStatus status = HandleNewRigidBodyContact(rigidBodyContact, static_cast<RigidBodyController *>(rigidBodyContact->GetStartElement()));
			
			if (status != kRigidBodyUnchanged)
			{
				if (status == kRigidBodyDestroyed) return;
				break;
			}
		}
		
		contact = contact->GetNextIncomingEdge();
	}
	
	float maxLinearSpeed = physicsController->GetMaxLinearSpeed();
	float linearSpeed = Magnitude(linearVelocity);
	if (linearSpeed > maxLinearSpeed) linearVelocity *= maxLinearSpeed / linearSpeed;
	
	float maxAngularSpeed = physicsController->GetMaxAngularSpeed();
	float angularSpeed = Magnitude(angularVelocity);
	if (angularSpeed > maxAngularSpeed) angularVelocity *= maxAngularSpeed / angularSpeed;
	
	if (!(rigidBodyFlags & kRigidBodyKeepAwake))
	{
		#if C4SIMD
		
			float	maxBoxSize;
			
			float4 c1 = SimdLoad(&finalNodeTransform(0,0));
			float4 c2 = SimdLoad(&finalNodeTransform(0,0), 4);
			
			float4 centerPoint = SimdTransformPoint3D(c1, c2, SimdLoad(&finalNodeTransform(0,0), 8), SimdLoad(&finalNodeTransform(0,0), 12), SimdLoadUnaligned(&centerOfMass.x));
			float4 centerDiff = centerSleepBox.IncludePoint(centerPoint);
			centerDiff = SimdMax(SimdMax(SimdSmearX(centerDiff), SimdSmearY(centerDiff)), SimdSmearZ(centerDiff));
			
			float4 axisPoint1 = SimdAdd(centerPoint, c1);
			float4 axisDiff1 = axisSleepBox[0].IncludePoint(axisPoint1);
			axisDiff1 = SimdMax(SimdMax(SimdSmearX(axisDiff1), SimdSmearY(axisDiff1)), SimdSmearZ(axisDiff1));
			
			float4 axisPoint2 = SimdAdd(centerPoint, c2);
			float4 axisDiff2 = axisSleepBox[1].IncludePoint(axisPoint2);
			axisDiff2 = SimdMax(SimdMax(SimdSmearX(axisDiff2), SimdSmearY(axisDiff2)), SimdSmearZ(axisDiff2));
			
			SimdStoreX(SimdMax(SimdMul(SimdMax(axisDiff1, axisDiff2), SimdLoadConstant<0x3F000000>()), centerDiff), &maxBoxSize);
		
		#else
		
			Point3D centerPoint = finalNodeTransform * centerOfMass;
			Vector3D centerDiff = centerSleepBox.IncludePoint(centerPoint);
			float centerBoxSize = Fmax(centerDiff.x, centerDiff.y, centerDiff.z);
			
			Point3D axisPoint1 = centerPoint + finalNodeTransform[0];
			Vector3D axisDiff1 = axisSleepBox[0].IncludePoint(axisPoint1);
			float axisBoxSize1 = Fmax(axisDiff1.x, axisDiff1.y, axisDiff1.z);
			
			Point3D axisPoint2 = centerPoint + finalNodeTransform[1];
			Vector3D axisDiff2 = axisSleepBox[1].IncludePoint(axisPoint2);
			float axisBoxSize2 = Fmax(axisDiff2.x, axisDiff2.y, axisDiff2.z);
			
			float maxBoxSize = Fmax(Fmax(axisBoxSize1, axisBoxSize2) * 0.5F, centerBoxSize);
		
		#endif
		
		if (maxBoxSize > sleepBoxSize)
		{
			#if C4SIMD
			
				SimdStore3D(centerPoint, &centerSleepBox.min.x);
				SimdStore3D(centerPoint, &centerSleepBox.max.x);
				SimdStore3D(axisPoint1, &axisSleepBox[0].min.x);
				SimdStore3D(axisPoint1, &axisSleepBox[0].max.x);
				SimdStore3D(axisPoint2, &axisSleepBox[1].min.x);
				SimdStore3D(axisPoint2, &axisSleepBox[1].max.x);
			
			#else
			
				centerSleepBox.min = centerPoint;
				centerSleepBox.max = centerPoint;
				axisSleepBox[0].min = axisPoint1;
				axisSleepBox[0].max = axisPoint1;
				axisSleepBox[1].min = axisPoint2;
				axisSleepBox[1].max = axisPoint2;
			
			#endif
			
			sleepStepCount = 0;
		}
		else
		{
			sleepStepCount++;
		}
	}
}

void RigidBodyController::ApplyVelocityPreconstraint(const Jacobian& jacobian, float impulse)
{
	linearVelocity += jacobian.linear * (impulse * inverseBodyMass);
	angularVelocity += worldInverseInertiaTensor * (jacobian.angular * impulse);
}

void RigidBodyController::ApplyVelocityCorrection(const Jacobian& jacobian, float impulse)
{
	Vector3D linear = jacobian.linear * (impulse * inverseBodyMass);
	maxLinearCorrection = Fmax(maxLinearCorrection, SquaredMag(linear));
	linearCorrection += linear;
	linearVelocity = initialLinearVelocity + linearCorrection;
	
	Vector3D angular = worldInverseInertiaTensor * (jacobian.angular * impulse);
	maxAngularCorrection = Fmax(maxAngularCorrection, SquaredMag(angular));
	angularCorrection += angular;
	angularVelocity = initialAngularVelocity + angularCorrection;
}

void RigidBodyController::ApplyLinearVelocityCorrection(const Vector3D& jacobian, float impulse)
{
	Vector3D linear = jacobian * (impulse * inverseBodyMass);
	maxLinearCorrection = Fmax(maxLinearCorrection, SquaredMag(linear));
	linearCorrection += linear;
	linearVelocity = initialLinearVelocity + linearCorrection;
}

void RigidBodyController::ApplyAngularVelocityCorrection(const Vector3D& jacobian, float impulse)
{
	Vector3D angular = worldInverseInertiaTensor * (jacobian * impulse);
	maxAngularCorrection = Fmax(maxAngularCorrection, SquaredMag(angular));
	angularCorrection += angular;
	angularVelocity = initialAngularVelocity + angularCorrection;
}

void RigidBodyController::RemoveLaterContacts(float param)
{
	Contact *contact = GetFirstOutgoingEdge();
	while (contact)
	{
		Contact *next = contact->GetNextOutgoingEdge();
		if ((contact->GetNotificationMask() != 0) && (contact->GetContactParam() > param)) delete contact;
		contact = next;
	}
	
	contact = GetFirstIncomingEdge();
	while (contact)
	{
		Contact *next = contact->GetNextIncomingEdge();
		if ((contact->GetNotificationMask() != 0) && (contact->GetContactParam() > param)) delete contact;
		contact = next;
	}
}

void RigidBodyController::SetRigidBodyTransform(const Transform4D& transform)
{
	Node *node = GetTargetNode();
	node->SetNodeTransform(transform);
	node->StopMotion();
	
	initialNodeTransform = transform;
	finalNodeTransform = transform;
	
	initialWorldTransform = node->GetSuperNode()->GetWorldTransform() * transform;
	finalWorldTransform = initialWorldTransform;
	
	moveCenterOfMass = transform * centerOfMass;
	moveDisplacement.Set(0.0F, 0.0F, 0.0F);
	moveRotationAngle = 0.0F;
}

void RigidBodyController::SetRigidBodyMatrix3D(const Matrix3D& matrix)
{
	Node *node = GetTargetNode();
	node->SetNodeMatrix3D(matrix);
	
	initialNodeTransform.SetMatrix3D(matrix);
	finalNodeTransform.SetMatrix3D(matrix);
}

void RigidBodyController::SetRigidBodyPosition(const Point3D& position)
{
	Node *node = GetTargetNode();
	node->SetNodePosition(position);
	node->StopMotion();
	
	initialNodeTransform.SetTranslation(position);
	finalNodeTransform.SetTranslation(position);
	
	initialWorldTransform = node->GetSuperNode()->GetWorldTransform() * initialNodeTransform;
	finalWorldTransform = initialWorldTransform;
	
	moveCenterOfMass = initialNodeTransform * centerOfMass;
	moveDisplacement.Set(0.0F, 0.0F, 0.0F);
	moveRotationAngle = 0.0F;
}

void RigidBodyController::ApplyImpulse(const Vector3D& impulse)
{
	impulseForce += impulse * kInverseTimeStep;
	
	RecursiveWake();
}

void RigidBodyController::ApplyImpulse(const Vector3D& impulse, const Point3D& position)
{
	impulseForce += impulse * kInverseTimeStep;
	impulseTorque += ((position - centerOfMass) % impulse) * kInverseTimeStep;
	
	RecursiveWake();
}

bool RigidBodyController::DetectSegmentIntersection(const Point3D& p1, const Point3D& p2, float radius, BodyHitData *bodyHitData) const
{
	bool intersection = false;
	float tmax = 1.0F;
	
	const Shape *shape = shapeList.First();
	while (shape)
	{
		ShapeHitData	shapeHitData;
		
		const Transform4D& inverseShapeTransform = shape->GetInverseWorldTransform();
		if (shape->GetObject()->DetectSegmentIntersection(inverseShapeTransform * p1, inverseShapeTransform * p2, radius, &shapeHitData))
		{
			float t = shapeHitData.param;
			if ((!intersection) || (t < tmax))
			{
				intersection = true;
				tmax = t;
				
				bodyHitData->param = t;
				bodyHitData->position = shape->GetWorldTransform() * shapeHitData.position;
				bodyHitData->normal = shapeHitData.normal * inverseShapeTransform;
				bodyHitData->shape = shape;
			}
		}
		
		shape = shape->Next();
	}
	
	return (intersection);
}

float RigidBodyController::CalculateSubmergedVolume(const Antivector4D& plane, Point3D *submergedCentroid) const
{
	Point3D		centroid;
	
	float submergedVolume = 0.0F;
	submergedCentroid->Set(0.0F, 0.0F, 0.0F);
	
	const Shape *shape = shapeList.First();
	while (shape)
	{
		const ShapeObject *object = shape->GetObject();
		const Transform4D& shapeTransform = shape->GetNodeTransform();
		
		float volume = object->CalculateSubmergedVolume(plane * shapeTransform, &centroid);
		if (volume > 0.0F)
		{
			submergedVolume += volume;
			*submergedCentroid += shapeTransform * centroid * volume;
		}
		
		shape = shape->Next();
	}
	
	shape = internalShapeList.First();
	while (shape)
	{
		const ShapeObject *object = shape->GetObject();
		const Transform4D& shapeTransform = shape->GetNodeTransform();
		
		float volume = object->CalculateSubmergedVolume(plane * shapeTransform, &centroid);
		if (volume > 0.0F)
		{
			volume *= NonzeroFsgn(object->GetShapeDensity());
			
			submergedVolume += volume;
			*submergedCentroid += shapeTransform * centroid * volume;
		}
		
		shape = shape->Next();
	}
	
	*submergedCentroid /= submergedVolume;
	return (submergedVolume);
}

bool RigidBodyController::ValidRigidBodyCollision(const RigidBodyController *body) const
{
	return ((collisionKind & body->GetCollisionExclusionMask()) == 0);
}

bool RigidBodyController::ValidGeometryCollision(const Geometry *geometry) const
{
	return ((collisionKind & geometry->GetObject()->GetCollisionExclusionMask()) == 0);
}

RigidBodyStatus RigidBodyController::HandleNewRigidBodyContact(const RigidBodyContact *contact, RigidBodyController *contactBody)
{
	return (GetTargetNode()->GetWorld()->HandleNewRigidBodyContact(this, contact, contactBody));
}

RigidBodyStatus RigidBodyController::HandleNewGeometryContact(const GeometryContact *contact)
{
	return (GetTargetNode()->GetWorld()->HandleNewGeometryContact(this, contact));
}

void RigidBodyController::HandleWaterSubmergence(void)
{
	GetTargetNode()->GetWorld()->HandleWaterSubmergence(this);
}


RigidBodySnapshotMessage::RigidBodySnapshotMessage(int32 controllerIndex) : ControllerMessage(RigidBodyController::kRigidBodyMessageSnapshot, controllerIndex)
{
}

RigidBodySnapshotMessage::RigidBodySnapshotMessage(int32 controllerIndex, const Point3D& position, const Quaternion& rotation, const Vector3D& linearVelocity, const Vector3D& angularVelocity) : ControllerMessage(RigidBodyController::kRigidBodyMessageSnapshot, controllerIndex)
{
	rigidBodyPosition = position;
	rigidBodyRotation = rotation;
	rigidBodyLinearVelocity = linearVelocity;
	rigidBodyAngularVelocity = angularVelocity;
}

RigidBodySnapshotMessage::~RigidBodySnapshotMessage()
{
}

void RigidBodySnapshotMessage::Compress(Compressor& data) const
{
	ControllerMessage::Compress(data);
	
	data << rigidBodyPosition;
	data << rigidBodyRotation;
	data << rigidBodyLinearVelocity;
	data << rigidBodyAngularVelocity;
}

bool RigidBodySnapshotMessage::Decompress(Decompressor& data)
{
	if (ControllerMessage::Decompress(data))
	{
		data >> rigidBodyPosition;
		data >> rigidBodyRotation;
		data >> rigidBodyLinearVelocity;
		data >> rigidBodyAngularVelocity;
		return (true);
	}
	
	return (false);
}


RigidBodySleepMessage::RigidBodySleepMessage(int32 controllerIndex) : ControllerMessage(RigidBodyController::kRigidBodyMessageSleep, controllerIndex)
{
}

RigidBodySleepMessage::RigidBodySleepMessage(int32 controllerIndex, const Point3D& position, const Quaternion& rotation) : ControllerMessage(RigidBodyController::kRigidBodyMessageSleep, controllerIndex)
{
	rigidBodyPosition = position;
	rigidBodyRotation = rotation;
}

RigidBodySleepMessage::~RigidBodySleepMessage()
{
}

void RigidBodySleepMessage::Compress(Compressor& data) const
{
	ControllerMessage::Compress(data);
	
	data << rigidBodyPosition;
	data << rigidBodyRotation;
}

bool RigidBodySleepMessage::Decompress(Decompressor& data)
{
	if (ControllerMessage::Decompress(data))
	{
		data >> rigidBodyPosition;
		data >> rigidBodyRotation;
		return (true);
	}
	
	return (false);
}


ShapeIntersectionJob::ShapeIntersectionJob(ExecuteProc *execProc, PhysicsController *data, RigidBodyController *body1, RigidBodyController *body2, const Shape *shape1, const Shape *shape2, unsigned_int32 index1, unsigned_int32 index2) : BatchJob(execProc, data, kJobNonpersistent)
{
	alphaBody = body1;
	betaBody = body2;
	alphaShape = shape1;
	betaShape = shape2;
	alphaIndex = index1;
	betaIndex = index2;
}


GeometryIntersectionJob::GeometryIntersectionJob(ExecuteProc *execProc, RigidBodyController *body, const Shape *shape, unsigned_int32 index, Geometry *geometry) : BatchJob(execProc, nullptr, kJobNonpersistent)
{
	rigidBody = body;
	shapeNode = shape;
	shapeIndex = index;
	geometryNode = geometry;
}


PhysicsController::PhysicsController() : Controller(kControllerPhysics)
{
	physicsGraph.AddElement(&nullBody);
	
	simulationStep = 0;
	simulationTime = kPhysicsTimeStep;
	rigidBodyParity = 0;
	fieldApplicationStamp = 0xFFFFFFFF;
	
	gravityAcceleration.Set(0.0F, 0.0F, -9.8F);
	
	maxLinearSpeed = kDefaultMaxLinearSpeed;
	maxAngularSpeed = kDefaultMaxAngularSpeed;
	
	for (machine a = 0; a < kPhysicsCounterCount; a++) physicsCounter[a] = 0;
}

PhysicsController::~PhysicsController()
{
}

bool PhysicsController::ValidNode(const Node *node)
{
	return (node->GetNodeType() == kNodePhysics);
}

void PhysicsController::RegisterFunctions(ControllerRegistration *registration)
{
	const StringTable *table = TheInterfaceMgr->GetStringTable();
	
	static FunctionReg<SetGravityFunction> setGravityRegistration(registration, kFunctionSetGravity, table->GetString(StringID('CTRL', kControllerPhysics, kFunctionSetGravity)), kFunctionRemote | kFunctionJournaled);
}

void PhysicsController::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Controller::Pack(data, packFlags);
	
	data << ChunkHeader('STEP', 4);
	data << simulationStep;
	
	data << ChunkHeader('GRAV', sizeof(Vector3D));
	data << gravityAcceleration;
	
	data << ChunkHeader('MAXS', 8);
	data << maxLinearSpeed;
	data << maxAngularSpeed;
	
	if (!(packFlags & kPackSettings))
	{
		const Body *body = physicsGraph.GetFirstElement();
		while (body)
		{
			if ((body->GetBodyType() != kBodyRigid) || (!(static_cast<const RigidBodyController *>(body)->GetTargetNode()->GetNodeFlags() & kNodeNonpersistent)))
			{
				const Contact *contact = body->GetFirstOutgoingEdge();
				while (contact)
				{
					if (!contact->NonpersistentFinishNode())
					{
						PackHandle handle = data.BeginChunk('CTAC');
						contact->PackType(data);
						contact->Pack(data, packFlags);
						data.EndChunk(handle);
					}
					
					contact = contact->GetNextOutgoingEdge();
				}
			}
			
			body = body->GetNextElement();
		}
	}
	
	data << TerminatorChunk;
}

void PhysicsController::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Controller::Unpack(data, unpackFlags);
	UnpackChunkList<PhysicsController>(data, unpackFlags);
}

bool PhysicsController::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'STEP':
			
			data >> simulationStep;
			return (true);
		
		case 'GRAV':
			
			data >> gravityAcceleration;
			return (true);
		
		case 'MAXS':
			
			data >> maxLinearSpeed;
			data >> maxAngularSpeed;
			return (true);
		
		case 'CTAC':
		{
			Contact *contact = Contact::Construct(data, unpackFlags, &nullBody);
			if (contact)
			{
				contact->Unpack(++data, unpackFlags);
				return (true);
			}
			
			break;
		}
	}
	
	return (false);
}

int32 PhysicsController::GetSettingCount(void) const
{
	return (1);
}

Setting *PhysicsController::GetSetting(int32 index) const
{
	if (index == 0)
	{
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		const char *title = table->GetString(StringID('CTRL', kControllerPhysics, 'GRAV'));
		return (new TextSetting('GRAV', -gravityAcceleration.z, title));
	}
	
	return (nullptr);
}

void PhysicsController::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'GRAV')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		gravityAcceleration.z = -Text::StringToFloat(text);
	}
}

void PhysicsController::AddRigidBody(RigidBodyController *rigidBody)
{
	physicsGraph.AddElement(rigidBody);
	
	if ((rigidBody->RigidBodyAsleep()) && (rigidBody->GetTargetNode()->Enabled())) sleepingList.Append(rigidBody);
}

void PhysicsController::RemoveRigidBody(RigidBodyController *rigidBody)
{
	physicsGraph.RemoveElement(rigidBody);
}

void PhysicsController::WakeRigidBody(RigidBodyController *rigidBody)
{
	const List<RigidBodyController> *list = rigidBody->ListElement<RigidBodyController>::GetOwningList();
	if ((!list) || (list == &sleepingList)) rigidBodyList[rigidBodyParity].Append(rigidBody);
	
	rigidBody->rigidBodyState &= ~kRigidBodyAsleep;
}

void PhysicsController::SleepRigidBody(RigidBodyController *rigidBody)
{
	if (rigidBody->GetTargetNode()->Enabled()) sleepingList.Append(rigidBody);
	else rigidBody->ListElement<RigidBodyController>::Detach();
	
	rigidBody->rigidBodyState |= kRigidBodyAsleep;
}

unsigned_int32 PhysicsController::CalculateZeroSimplexMinimum(const Point3D *simplex, Point3D *p)
{
	*p = simplex[0];
	return (0x0001);
}

unsigned_int32 PhysicsController::CalculateOneSimplexMinimum(const Point3D *simplex, Point3D *p)
{
	#if C4SIMD
	
		float4 q1 = SimdLoadUnaligned(&simplex[0].x);
		float4 q2 = SimdLoadUnaligned(&simplex[1].x);
		float4 v = SimdSub(q2, q1);
		
		register const float4 zero = SimdGetZero();
		
		if (!SimdCmpltScalar(SimdDot3D(v, q1), zero))
		{
			SimdStore3D(q1, &p->x);
			return (0x0001);
		}
		
		if (!SimdCmpgtScalar(SimdDot3D(v, q2), zero))
		{
			SimdStore3D(q2, &p->x);
			return (0x0002);
		}
		
		float4 f = SimdDiv(SimdProjectOnto3D(q1, v), SimdSmearX(SimdDot3D(v, v)));
		SimdStore3D(SimdSub(q1, f), &p->x);
		return (0x0003);
	
	#else
	
		const Point3D& q1 = simplex[0];
		const Point3D& q2 = simplex[1];
		Vector3D v = q2 - q1;
		
		if (!(v * q1 < 0.0F))
		{
			// Origin is in Voronoi region for vertex q1.
			
			*p = q1;
			return (0x0001);
		}
		
		if (!(v * q2 > 0.0F))
		{
			// Origin is in Voronoi region for vertex q2.
			
			*p = q2;
			return (0x0002);
		}
		
		// Origin is in Voronoi region for line segment interior.
		
		*p = q1 - ProjectOnto(q1, v) / SquaredMag(v);
		return (0x0003);
	
	#endif
}

unsigned_int32 PhysicsController::CalculateTwoSimplexMinimum(const Point3D *simplex, Point3D *p)
{
	#if C4SIMD
	
		float4 q1 = SimdLoadUnaligned(&simplex[0].x);
		float4 q2 = SimdLoadUnaligned(&simplex[1].x);
		float4 q3 = SimdLoadUnaligned(&simplex[2].x);
		float4 v1 = SimdSub(q2, q1);
		float4 v2 = SimdSub(q3, q2);
		float4 v3 = SimdSub(q1, q3);
		
		register const float4 zero = SimdGetZero();
		
		bool m1 = !SimdCmpltScalar(SimdDot3D(v1, q1), zero);
		bool p1 = !SimdCmpgtScalar(SimdDot3D(v1, q2), zero);
		bool m2 = !SimdCmpltScalar(SimdDot3D(v2, q2), zero);
		bool p2 = !SimdCmpgtScalar(SimdDot3D(v2, q3), zero);
		bool m3 = !SimdCmpltScalar(SimdDot3D(v3, q3), zero);
		bool p3 = !SimdCmpgtScalar(SimdDot3D(v3, q1), zero);
		
		if (m1 & p3)
		{
			SimdStore3D(q1, &p->x);
			return (0x0001);
		}
		
		if (m2 & p1)
		{
			SimdStore3D(q2, &p->x);
			return (0x0002);
		}
		
		if (m3 & p2)
		{
			SimdStore3D(q3, &p->x);
			return (0x0004);
		}
		
		float4 n = SimdCross3D(v3, v1);
		bool d1 = !SimdCmpltScalar(SimdDot3D(SimdCross3D(n, v1), q1), zero);
		bool d2 = !SimdCmpltScalar(SimdDot3D(SimdCross3D(n, v2), q2), zero);
		bool d3 = !SimdCmpltScalar(SimdDot3D(SimdCross3D(n, v3), q3), zero);
		
		if ((d1) && (!m1) && (!p1))
		{
			float4 f = SimdDiv(SimdProjectOnto3D(q1, v1), SimdSmearX(SimdDot3D(v1, v1)));
			SimdStore3D(SimdSub(q1, f), &p->x);
			return (0x0003);
		}
		
		if ((d2) && (!m2) && (!p2))
		{
			float4 f = SimdDiv(SimdProjectOnto3D(q2, v2), SimdSmearX(SimdDot3D(v2, v2)));
			SimdStore3D(SimdSub(q2, f), &p->x);
			return (0x0006);
		}
		
		if ((d3) && (!m3) && (!p3))
		{
			float4 f = SimdDiv(SimdProjectOnto3D(q3, v3), SimdSmearX(SimdDot3D(v3, v3)));
			SimdStore3D(SimdSub(q3, f), &p->x);
			return (0x0005);
		}
		
		float4 f = SimdDiv(SimdProjectOnto3D(q1, n), SimdSmearX(SimdDot3D(n, n)));
		SimdStore3D(f, &p->x);
		return (0x0007);
	
	#else
	
		const Point3D& q1 = simplex[0];
		const Point3D& q2 = simplex[1];
		const Point3D& q3 = simplex[2];
		Vector3D v1 = q2 - q1;
		Vector3D v2 = q3 - q2;
		Vector3D v3 = q1 - q3;
		
		bool m1 = !(v1 * q1 < 0.0F);
		bool p1 = !(v1 * q2 > 0.0F);
		bool m2 = !(v2 * q2 < 0.0F);
		bool p2 = !(v2 * q3 > 0.0F);
		bool m3 = !(v3 * q3 < 0.0F);
		bool p3 = !(v3 * q1 > 0.0F);
		
		if (m1 & p3)
		{
			// Origin is in Voronoi region for vertex q1.
			
			*p = q1;
			return (0x0001);
		}
		
		if (m2 & p1)
		{
			// Origin is in Voronoi region for vertex q2.
			
			*p = q2;
			return (0x0002);
		}
		
		if (m3 & p2)
		{
			// Origin is in Voronoi region for vertex q3.
			
			*p = q3;
			return (0x0004);
		}
		
		Vector3D n = v3 % v1;
		bool d1 = !((n % v1) * q1 < 0.0F);
		bool d2 = !((n % v2) * q2 < 0.0F);
		bool d3 = !((n % v3) * q3 < 0.0F);
		
		if ((d1) && (!m1) && (!p1))
		{
			// Origin is in Voronoi region for edge v1.
			
			*p = q1 - ProjectOnto(q1, v1) / SquaredMag(v1);
			return (0x0003);
		}
		
		if ((d2) && (!m2) && (!p2))
		{
			// Origin is in Voronoi region for edge v2.
			
			*p = q2 - ProjectOnto(q2, v2) / SquaredMag(v2);
			return (0x0006);
		}
		
		if ((d3) && (!m3) && (!p3))
		{
			// Origin is in Voronoi region for edge v3.
			
			*p = q3 - ProjectOnto(q3, v3) / SquaredMag(v3);
			return (0x0005);
		}
		
		// Origin is in Voronoi region for triangle interior.
		
		*p = ProjectOnto(q1, n) / SquaredMag(n);
		return (0x0007);
	
	#endif
}

unsigned_int32 PhysicsController::CalculateThreeSimplexMinimum(const Point3D *simplex, Point3D *p)
{
	#if C4SIMD
	
		float4 q1 = SimdLoadUnaligned(&simplex[0].x);
		float4 q2 = SimdLoadUnaligned(&simplex[1].x);
		float4 q3 = SimdLoadUnaligned(&simplex[2].x);
		float4 q4 = SimdLoadUnaligned(&simplex[3].x);
		float4 v12 = SimdSub(q2, q1);
		float4 v23 = SimdSub(q3, q2);
		float4 v31 = SimdSub(q1, q3);
		float4 v14 = SimdSub(q4, q1);
		float4 v24 = SimdSub(q4, q2);
		float4 v34 = SimdSub(q4, q3);
		
		register const float4 zero = SimdGetZero();
		
		bool m12 = !SimdCmpltScalar(SimdDot3D(v12, q1), zero);
		bool p12 = !SimdCmpgtScalar(SimdDot3D(v12, q2), zero);
		bool m23 = !SimdCmpltScalar(SimdDot3D(v23, q2), zero);
		bool p23 = !SimdCmpgtScalar(SimdDot3D(v23, q3), zero);
		bool m31 = !SimdCmpltScalar(SimdDot3D(v31, q3), zero);
		bool p31 = !SimdCmpgtScalar(SimdDot3D(v31, q1), zero);
		bool m14 = !SimdCmpltScalar(SimdDot3D(v14, q1), zero);
		bool p14 = !SimdCmpgtScalar(SimdDot3D(v14, q4), zero);
		bool m24 = !SimdCmpltScalar(SimdDot3D(v24, q2), zero);
		bool p24 = !SimdCmpgtScalar(SimdDot3D(v24, q4), zero);
		bool m34 = !SimdCmpltScalar(SimdDot3D(v34, q3), zero);
		bool p34 = !SimdCmpgtScalar(SimdDot3D(v34, q4), zero);
		
		if (m12 & p31 & m14)
		{
			SimdStore3D(q1, &p->x);
			return (0x0001);
		}
		
		if (m23 & p12 & m24)
		{
			SimdStore3D(q2, &p->x);
			return (0x0002);
		}
		
		if (m31 & p23 & m34)
		{
			SimdStore3D(q3, &p->x);
			return (0x0004);
		}
		
		if (p14 & p24 & p34)
		{
			SimdStore3D(q4, &p->x);
			return (0x0008);
		}
		
		float4 n1 = SimdCross3D(v12, v31);
		float4 n2 = SimdCross3D(v12, v14);
		float4 n3 = SimdCross3D(v23, v24);
		float4 n4 = SimdCross3D(v31, v34);
		
		bool d11 = !SimdCmpltScalar(SimdDot3D(SimdCross3D(v12, n1), q1), zero);
		bool d12 = !SimdCmpltScalar(SimdDot3D(SimdCross3D(v23, n1), q2), zero);
		bool d13 = !SimdCmpltScalar(SimdDot3D(SimdCross3D(v31, n1), q3), zero);
		bool d21 = !SimdCmpltScalar(SimdDot3D(SimdCross3D(n2, v12), q1), zero);
		bool d22 = !SimdCmpltScalar(SimdDot3D(SimdCross3D(n2, v24), q2), zero);
		bool d24 = !SimdCmpltScalar(SimdDot3D(SimdCross3D(v14, n2), q4), zero);
		bool d32 = !SimdCmpltScalar(SimdDot3D(SimdCross3D(n3, v23), q2), zero);
		bool d33 = !SimdCmpltScalar(SimdDot3D(SimdCross3D(n3, v34), q3), zero);
		bool d34 = !SimdCmpltScalar(SimdDot3D(SimdCross3D(v24, n3), q4), zero);
		bool d43 = !SimdCmpltScalar(SimdDot3D(SimdCross3D(n4, v31), q3), zero);
		bool d41 = !SimdCmpltScalar(SimdDot3D(SimdCross3D(n4, v14), q1), zero);
		bool d44 = !SimdCmpltScalar(SimdDot3D(SimdCross3D(v34, n4), q4), zero);
		
		if ((!m12) && (!p12) && (d11) && (d21))
		{
			float4 f = SimdDiv(SimdProjectOnto3D(q1, v12), SimdSmearX(SimdDot3D(v12, v12)));
			SimdStore3D(SimdSub(q1, f), &p->x);
			return (0x0003);
		}
		
		if ((!m23) && (!p23) && (d12) && (d32))
		{
			float4 f = SimdDiv(SimdProjectOnto3D(q2, v23), SimdSmearX(SimdDot3D(v23, v23)));
			SimdStore3D(SimdSub(q2, f), &p->x);
			return (0x0006);
		}
		
		if ((!m31) && (!p31) && (d13) && (d43))
		{
			float4 f = SimdDiv(SimdProjectOnto3D(q3, v31), SimdSmearX(SimdDot3D(v31, v31)));
			SimdStore3D(SimdSub(q3, f), &p->x);
			return (0x0005);
		}
		
		if ((!m14) && (!p14) && (d24) && (d41))
		{
			float4 f = SimdDiv(SimdProjectOnto3D(q1, v14), SimdSmearX(SimdDot3D(v14, v14)));
			SimdStore3D(SimdSub(q1, f), &p->x);
			return (0x0009);
		}
		
		if ((!m24) && (!p24) && (d22) && (d34))
		{
			float4 f = SimdDiv(SimdProjectOnto3D(q2, v24), SimdSmearX(SimdDot3D(v24, v24)));
			SimdStore3D(SimdSub(q2, f), &p->x);
			return (0x000A);
		}
		
		if ((!m34) && (!p34) && (d33) && (d44))
		{
			float4 f = SimdDiv(SimdProjectOnto3D(q3, v34), SimdSmearX(SimdDot3D(v34, v34)));
			SimdStore3D(SimdSub(q3, f), &p->x);
			return (0x000C);
		}
		
		bool f1 = !SimdCmpltScalar(SimdMulScalar(SimdDot3D(n1, q1), SimdDot3D(n1, v14)), zero);
		bool f2 = !SimdCmpgtScalar(SimdMulScalar(SimdDot3D(n2, q2), SimdDot3D(n2, v31)), zero);
		bool f3 = !SimdCmpgtScalar(SimdMulScalar(SimdDot3D(n3, q3), SimdDot3D(n3, v12)), zero);
		bool f4 = !SimdCmpgtScalar(SimdMulScalar(SimdDot3D(n4, q4), SimdDot3D(n4, v23)), zero);
		
		if ((f1) && (!d11) && (!d12) && (!d13))
		{
			float4 f = SimdDiv(SimdProjectOnto3D(q1, n1), SimdSmearX(SimdDot3D(n1, n1)));
			SimdStore3D(f, &p->x);
			return (0x0007);
		}
		
		if ((f2) && (!d21) && (!d22) && (!d24))
		{
			float4 f = SimdDiv(SimdProjectOnto3D(q2, n2), SimdSmearX(SimdDot3D(n2, n2)));
			SimdStore3D(f, &p->x);
			return (0x000B);
		}
		
		if ((f3) && (!d32) && (!d33) && (!d34))
		{
			float4 f = SimdDiv(SimdProjectOnto3D(q3, n3), SimdSmearX(SimdDot3D(n3, n3)));
			SimdStore3D(f, &p->x);
			return (0x000E);
		}
		
		if ((f4) && (!d43) && (!d41) && (!d44))
		{
			float4 f = SimdDiv(SimdProjectOnto3D(q4, n4), SimdSmearX(SimdDot3D(n4, n4)));
			SimdStore3D(f, &p->x);
			return (0x000D);
		}
		
		SimdStore3D(zero, &p->x);
		return (0x000F);
	
	#else
	
		const Point3D& q1 = simplex[0];
		const Point3D& q2 = simplex[1];
		const Point3D& q3 = simplex[2];
		const Point3D& q4 = simplex[3];
		Vector3D v12 = q2 - q1;
		Vector3D v23 = q3 - q2;
		Vector3D v31 = q1 - q3;
		Vector3D v14 = q4 - q1;
		Vector3D v24 = q4 - q2;
		Vector3D v34 = q4 - q3;
		
		bool m12 = !(v12 * q1 < 0.0F);
		bool p12 = !(v12 * q2 > 0.0F);
		bool m23 = !(v23 * q2 < 0.0F);
		bool p23 = !(v23 * q3 > 0.0F);
		bool m31 = !(v31 * q3 < 0.0F);
		bool p31 = !(v31 * q1 > 0.0F);
		bool m14 = !(v14 * q1 < 0.0F);
		bool p14 = !(v14 * q4 > 0.0F);
		bool m24 = !(v24 * q2 < 0.0F);
		bool p24 = !(v24 * q4 > 0.0F);
		bool m34 = !(v34 * q3 < 0.0F);
		bool p34 = !(v34 * q4 > 0.0F);
		
		if (m12 & p31 & m14)
		{
			// Origin is in Voronoi region for vertex q1.
			
			*p = q1;
			return (0x0001);
		}
		
		if (m23 & p12 & m24)
		{
			// Origin is in Voronoi region for vertex q2.
			
			*p = q2;
			return (0x0002);
		}
		
		if (m31 & p23 & m34)
		{
			// Origin is in Voronoi region for vertex q3.
			
			*p = q3;
			return (0x0004);
		}
		
		if (p14 & p24 & p34)
		{
			// Origin is in Voronoi region for vertex q4.
			
			*p = q4;
			return (0x0008);
		}
		
		Vector3D n1 = v12 % v31;
		Vector3D n2 = v12 % v14;
		Vector3D n3 = v23 % v24;
		Vector3D n4 = v31 % v34;
		
		bool d11 = !((v12 % n1) * q1 < 0.0F);
		bool d12 = !((v23 % n1) * q2 < 0.0F);
		bool d13 = !((v31 % n1) * q3 < 0.0F);
		bool d21 = !((n2 % v12) * q1 < 0.0F);
		bool d22 = !((n2 % v24) * q2 < 0.0F);
		bool d24 = !((v14 % n2) * q4 < 0.0F);
		bool d32 = !((n3 % v23) * q2 < 0.0F);
		bool d33 = !((n3 % v34) * q3 < 0.0F);
		bool d34 = !((v24 % n3) * q4 < 0.0F);
		bool d43 = !((n4 % v31) * q3 < 0.0F);
		bool d41 = !((n4 % v14) * q1 < 0.0F);
		bool d44 = !((v34 % n4) * q4 < 0.0F);
		
		if ((!m12) && (!p12) && (d11) && (d21))
		{
			// Origin is in Voronoi region for edge v12.
			
			*p = q1 - ProjectOnto(q1, v12) / SquaredMag(v12);
			return (0x0003);
		}
		
		if ((!m23) && (!p23) && (d12) && (d32))
		{
			// Origin is in Voronoi region for edge v23.
			
			*p = q2 - ProjectOnto(q2, v23) / SquaredMag(v23);
			return (0x0006);
		}
		
		if ((!m31) && (!p31) && (d13) && (d43))
		{
			// Origin is in Voronoi region for edge v31.
			
			*p = q3 - ProjectOnto(q3, v31) / SquaredMag(v31);
			return (0x0005);
		}
		
		if ((!m14) && (!p14) && (d24) && (d41))
		{
			// Origin is in Voronoi region for edge v14.
			
			*p = q1 - ProjectOnto(q1, v14) / SquaredMag(v14);
			return (0x0009);
		}
		
		if ((!m24) && (!p24) && (d22) && (d34))
		{
			// Origin is in Voronoi region for edge v24.
			
			*p = q2 - ProjectOnto(q2, v24) / SquaredMag(v24);
			return (0x000A);
		}
		
		if ((!m34) && (!p34) && (d33) && (d44))
		{
			// Origin is in Voronoi region for edge v34.
			
			*p = q3 - ProjectOnto(q3, v34) / SquaredMag(v34);
			return (0x000C);
		}
		
		bool f1 = !((n1 * q1) * (n1 * v14) < 0.0F);
		bool f2 = !((n2 * q2) * (n2 * v31) > 0.0F);
		bool f3 = !((n3 * q3) * (n3 * v12) > 0.0F);
		bool f4 = !((n4 * q4) * (n4 * v23) > 0.0F);
		
		if ((f1) && (!d11) && (!d12) && (!d13))
		{
			// Origin is in Voronoi region for face f1.
			
			*p = ProjectOnto(q1, n1) / SquaredMag(n1);
			return (0x0007);
		}
		
		if ((f2) && (!d21) && (!d22) && (!d24))
		{
			// Origin is in Voronoi region for face f2.
			
			*p = ProjectOnto(q2, n2) / SquaredMag(n2);
			return (0x000B);
		}
		
		if ((f3) && (!d32) && (!d33) && (!d34))
		{
			// Origin is in Voronoi region for face f3.
			
			*p = ProjectOnto(q3, n3) / SquaredMag(n3);
			return (0x000E);
		}
		
		if ((f4) && (!d43) && (!d41) && (!d44))
		{
			// Origin is in Voronoi region for face f4.
			
			*p = ProjectOnto(q4, n4) / SquaredMag(n4);
			return (0x000D);
		}
		
		// Origin is in tetrahedron interior.
		
		p->Set(0.0F, 0.0F, 0.0F);
		return (0x000F);
	
	#endif
}

float PhysicsController::SortRigidBodyList(List<RigidBodyController> *inputList, int32 depth, float minValue, float maxValue, int32 index, List<RigidBodyController> *outputList)
{
	RigidBodyController *rigidBody = inputList->First();
	if (rigidBody == inputList->Last())
	{
		outputList->Append(rigidBody);
		return (0.0F);
	}
	
	float distance = maxValue - minValue;
	if ((distance < kCollisionSweepEpsilon) || (--depth == 0))
	{
		do
		{
			RigidBodyController *next = rigidBody->Next();
			outputList->Append(rigidBody);
			rigidBody = next;
		} while (rigidBody);
		
		return (distance);
	}
	
	List<RigidBodyController>	lowerList;
	
	float maxLower = minValue;
	float minHigher = maxValue;
	float center = (minValue + maxValue) * 0.5F;
	
	do
	{
		RigidBodyController *next = rigidBody->Next();
		float v = rigidBody->bodyCollisionBox.min[index];
		if (v < center)
		{
			maxLower = Fmax(maxLower, v);
			lowerList.Append(rigidBody);
		}
		else
		{
			minHigher = Fmin(minHigher, v);
		}
		
		rigidBody = next;
	} while (rigidBody);
	
	distance = SortRigidBodyList(&lowerList, depth, minValue, maxLower, index, outputList);
	distance = Fmax(SortRigidBodyList(inputList, depth, minHigher, maxValue, index, outputList), distance);
	return (distance);
}

void PhysicsController::CollideRigidBodiesX(List<RigidBodyController> *inputList, int32 depth, float xmin, float xmax, List<RigidBodyController> *outputList)
{
	List<RigidBodyController>	sortedList;
	
	float overlap = SortRigidBodyList(inputList, depth, xmin, xmax, 0, &sortedList);
	
	RigidBodyController *lowerBody = sortedList.First();
	if (lowerBody)
	{
		for (;;)
		{
			RigidBodyController *higherBody = lowerBody->Next();
			if (!higherBody)
			{
				outputList->Append(lowerBody);
				break;
			}
			
			xmax = lowerBody->bodyCollisionBox.max.x;
			if (higherBody->bodyCollisionBox.min.x - overlap < xmax)
			{
				List<RigidBodyController>	clusterList;
				
				float y = lowerBody->bodyCollisionBox.min.y;
				float ymin = y;
				float ymax = y;
				
				clusterList.Append(lowerBody);
				do
				{
					lowerBody = higherBody;
					higherBody = higherBody->Next();
					clusterList.Append(lowerBody);
					
					y = lowerBody->bodyCollisionBox.min.y;
					ymin = Fmin(ymin, y);
					ymax = Fmax(ymax, y);
					
					xmax = Fmax(xmax, lowerBody->bodyCollisionBox.max.x);
				} while ((higherBody) && (higherBody->bodyCollisionBox.min.x - overlap < xmax));
				
				CollideRigidBodiesY(&clusterList, depth, ymin, ymax, outputList);
				if (!higherBody) break;
			}
			else
			{
				outputList->Append(lowerBody);
			}
			
			lowerBody = higherBody;
		}
	}
}

void PhysicsController::CollideRigidBodiesY(List<RigidBodyController> *inputList, int32 depth, float ymin, float ymax, List<RigidBodyController> *outputList)
{
	List<RigidBodyController>	sortedList;
	
	float overlap = SortRigidBodyList(inputList, depth, ymin, ymax, 1, &sortedList);
	
	RigidBodyController *lowerBody = sortedList.First();
	for (;;)
	{
		RigidBodyController *higherBody = lowerBody->Next();
		if (!higherBody)
		{
			outputList->Append(lowerBody);
			break;
		}
		
		ymax = lowerBody->bodyCollisionBox.max.y;
		if (higherBody->bodyCollisionBox.min.y - overlap < ymax)
		{
			List<RigidBodyController>	clusterList;
			
			float z = lowerBody->bodyCollisionBox.min.z;
			float zmin = z;
			float zmax = z;
			
			clusterList.Append(lowerBody);
			do
			{
				lowerBody = higherBody;
				higherBody = higherBody->Next();
				clusterList.Append(lowerBody);
				
				z = lowerBody->bodyCollisionBox.min.z;
				zmin = Fmin(zmin, z);
				zmax = Fmax(zmax, z);
				
				ymax = Fmax(ymax, lowerBody->bodyCollisionBox.max.y);
			} while ((higherBody) && (higherBody->bodyCollisionBox.min.y - overlap < ymax));
			
			CollideRigidBodiesZ(&clusterList, depth, zmin, zmax, outputList);
			if (!higherBody) break;
		}
		else
		{
			outputList->Append(lowerBody);
		}
		
		lowerBody = higherBody;
	}
}

void PhysicsController::CollideRigidBodiesZ(List<RigidBodyController> *inputList, int32 depth, float zmin, float zmax, List<RigidBodyController> *outputList)
{
	List<RigidBodyController>	sortedList;
	
	float overlap = SortRigidBodyList(inputList, depth, zmin, zmax, 2, &sortedList);
	
	RigidBodyController *lowerBody = sortedList.First();
	for (;;)
	{
		RigidBodyController *higherBody = lowerBody->Next();
		if (!higherBody)
		{
			outputList->Append(lowerBody);
			break;
		}
		
		zmax = lowerBody->bodyCollisionBox.max.z;
		if (higherBody->bodyCollisionBox.min.z - overlap < zmax)
		{
			List<RigidBodyController>	clusterList;
			
			int32 bodyCount = 1;
			int32 sleepingCount = lowerBody->RigidBodyAsleep();
			clusterList.Append(lowerBody);
			
			do
			{
				lowerBody = higherBody;
				higherBody = higherBody->Next();
				
				bodyCount++;
				sleepingCount += lowerBody->RigidBodyAsleep();
				clusterList.Append(lowerBody);
				
				zmax = Fmax(zmax, lowerBody->bodyCollisionBox.max.z);
			} while ((higherBody) && (higherBody->bodyCollisionBox.min.z - overlap < zmax));
			
			if (sleepingCount < bodyCount)
			{
				for (;;)
				{
					RigidBodyController *alphaBody = clusterList.First();
					outputList->Append(alphaBody);
					
					RigidBodyController *betaBody = clusterList.First();
					if (!betaBody) break;
					
					do
					{
						if (alphaBody->bodyCollisionBox.Intersection(betaBody->bodyCollisionBox))
						{
							if ((!alphaBody->RigidBodyAsleep()) || (!betaBody->RigidBodyAsleep()))
							{
								if ((alphaBody->ValidRigidBodyCollision(betaBody)) && (betaBody->ValidRigidBodyCollision(alphaBody))) DetectBodyCollision(alphaBody, betaBody);
							}
						}
						
						betaBody = betaBody->Next();
					} while ((betaBody) && (betaBody->bodyCollisionBox.min.z - overlap < alphaBody->bodyCollisionBox.max.z));
				}
			}
			else
			{
				RigidBodyController *rigidBody = clusterList.First();
				do
				{
					RigidBodyController *next = rigidBody->Next();
					outputList->Append(rigidBody);
					rigidBody = next;
				} while (rigidBody);
			}
			
			if (!higherBody) break;
		}
		else
		{
			outputList->Append(lowerBody);
		}
		
		lowerBody = higherBody;
	}
}

void PhysicsController::DetectBodyCollision(RigidBodyController *alphaBody, RigidBodyController *betaBody)
{
	unsigned_int32 alphaIndex = 0;
	const Shape *alphaShape = alphaBody->shapeList.First();
	while (alphaShape)
	{
		const Box3D& alphaBox = alphaShape->GetCollisionBox();
		
		unsigned_int32 betaIndex = 0;
		const Shape *betaShape = betaBody->shapeList.First();
		while (betaShape)
		{
			const Box3D& betaBox = betaShape->GetCollisionBox();
			if (alphaBox.Intersection(betaBox))
			{
				physicsCounter[kPhysicsCounterShapeIntersection]++;
				
				ShapeIntersectionJob *job = new ShapeIntersectionJob(&ShapeCollisionJob, this, alphaBody, betaBody, alphaShape, betaShape, alphaIndex, betaIndex);
				TheJobMgr->SubmitJob(job, &collisionBatch);
			}
			
			betaIndex++;
			betaShape = betaShape->Next();
		}
		
		alphaIndex++;
		alphaShape = alphaShape->Next();
	}
}

void PhysicsController::ShapeCollisionJob(Job *job, void *cookie)
{
	ShapeIntersectionJob *shapeJob = static_cast<ShapeIntersectionJob *>(job);
	
	RigidBodyContact *alphaContact = shapeJob->alphaBody->FindOutgoingBodyContact(shapeJob->betaBody, shapeJob->alphaIndex, shapeJob->betaIndex);
	if (alphaContact)
	{
		if (shapeJob->alphaShape->StaticIntersectShape(shapeJob->betaShape, alphaContact, &shapeJob->intersectionData))
		{
			shapeJob->rigidBodyContact = alphaContact;
			shapeJob->SetFinalizeProc(&FinalizeExistingShapeContact);
		}
	}
	else
	{
		RigidBodyContact *betaContact = shapeJob->alphaBody->FindIncomingBodyContact(shapeJob->betaBody, shapeJob->betaIndex, shapeJob->alphaIndex);
		if (betaContact)
		{
			if (shapeJob->betaShape->StaticIntersectShape(shapeJob->alphaShape, betaContact, &shapeJob->intersectionData))
			{
				shapeJob->rigidBodyContact = betaContact;
				shapeJob->SetFinalizeProc(&FinalizeExistingShapeContact);
			}
		}
		else
		{
			if (shapeJob->alphaShape->DynamicIntersectShape(shapeJob->betaShape, shapeJob->alphaBody->moveDisplacement, shapeJob->betaBody->moveDisplacement, &shapeJob->intersectionData))
			{
				shapeJob->SetFinalizeProc(&FinalizeNewShapeContact);
			}
		}
	}
}

void PhysicsController::FinalizeExistingShapeContact(Job *job, void *cookie)
{
	const ShapeIntersectionJob *shapeJob = static_cast<ShapeIntersectionJob *>(job);
	shapeJob->rigidBodyContact->UpdateContact(&shapeJob->intersectionData);
}

void PhysicsController::FinalizeNewShapeContact(Job *job, void *cookie)
{
	const ShapeIntersectionJob *shapeJob = static_cast<ShapeIntersectionJob *>(job);
	
	float param = shapeJob->intersectionData.contactParam;
	if (param < 1.0F)
	{
		shapeJob->alphaBody->AdjustDisplacement(param);
		shapeJob->betaBody->AdjustDisplacement(param);
		
		shapeJob->betaBody->RemoveLaterContacts(param);
		shapeJob->alphaBody->RemoveLaterContacts(param);
	}
	
	shapeJob->alphaBody->RecursiveWake(true);
	shapeJob->betaBody->RecursiveWake(true);
	
	new RigidBodyContact(shapeJob->alphaBody, shapeJob->betaBody, shapeJob->alphaShape, shapeJob->betaShape, shapeJob->alphaIndex, shapeJob->betaIndex, &shapeJob->intersectionData);
}

void PhysicsController::Move(void)
{
	int32 time = simulationTime + TheTimeMgr->GetDeltaTime();
	int32 stepCount = time / kPhysicsTimeStep;
	time -= stepCount * kPhysicsTimeStep;
	stepCount = Min(stepCount, kMaxPhysicsStepCount);
	simulationTime = time;
	
	for (machine a = 0; a < stepCount; a++)
	{
		List<RigidBodyController>	bodyList[2];
		List<RigidBodyController>	constrainList;
		
		for (machine b = kPhysicsCounterRigidBody + 1; b < kPhysicsCounterCount; b++) physicsCounter[b] = 0;
		
		int32 parity = rigidBodyParity;
		rigidBodyParity = parity ^ 1;
		
		int32 bodyCount = 0;
		float xmin = K::infinity;
		float xmax = K::minus_infinity;
		
		List<RigidBodyController> *currentList = &rigidBodyList[parity];
		for (;;)
		{
			RigidBodyController *rigidBody = currentList->First();
			if (!rigidBody) break;
			
			rigidBody->CalculateAppliedForces(gravityAcceleration);
			rigidBody->DetectWorldCollisions();
			
			float x = rigidBody->bodyCollisionBox.min.x;
			xmin = Fmin(xmin, x);
			xmax = Fmax(xmax, x);
			
			bodyCount++;
			bodyList[0].Append(rigidBody);
		}
		
		if (bodyCount == 0) break;
		
		for (;;)
		{
			RigidBodyController *rigidBody = sleepingList.First();
			if (!rigidBody) break;
			
			float x = rigidBody->bodyCollisionBox.min.x;
			xmin = Fmin(xmin, x);
			xmax = Fmax(xmax, x);
			
			bodyCount++;
			bodyList[0].Append(rigidBody);
		}
		
		CollideRigidBodiesX(&bodyList[0], Max(33 - Cntlz(bodyCount), 8), xmin, xmax, &bodyList[1]);
		TheJobMgr->FinishBatch(&collisionBatch);
		
		for (machine iteration = 0; iteration < kMaxConstraintIterationCount; iteration++)
		{
			RigidBodyController *rigidBody = bodyList[1].First();
			if (!rigidBody) break;
			
			do
			{
				RigidBodyController *next = rigidBody->Next();
				
				if ((rigidBody->GetFirstIncomingEdge()) || (rigidBody->GetFirstOutgoingEdge()))
				{
					rigidBody->maxLinearCorrection = 0.0F;
					rigidBody->maxAngularCorrection = 0.0F;
				}
				else
				{
					constrainList.Append(rigidBody);
				}
				
				rigidBody = next;
			} while (rigidBody);
			
			rigidBody = bodyList[1].First();
			while (rigidBody)
			{
				if (!rigidBody->RigidBodyAsleep()) rigidBody->Constrain();
				rigidBody = rigidBody->Next();
			}
			
			rigidBody = bodyList[1].First();
			while (rigidBody)
			{
				RigidBodyController *next = rigidBody->Next();
				
				if (!rigidBody->RigidBodyAsleep())
				{
					rigidBody->Integrate();
					//if (Fmax(rigidBody->maxLinearCorrection, rigidBody->maxAngularCorrection) < 1.0e-6F) constrainList.Append(rigidBody);
				}
				
				rigidBody = next;
			}
		}
		
		for (;;)
		{
			RigidBodyController *rigidBody = constrainList.First();
			if (!rigidBody) break;
			
			bodyList[1].Append(rigidBody);
		}
		
		List<RigidBodyController> *nextList = &rigidBodyList[parity ^ 1];
		for (;;)
		{
			RigidBodyController *rigidBody = bodyList[1].First();
			if (!rigidBody) break;
			
			if (!rigidBody->RigidBodyAsleep())
			{
				nextList->Append(rigidBody);
				rigidBody->Finalize();
			}
			else
			{
				sleepingList.Append(rigidBody);
			}
		}
		
		if (TheMessageMgr->Server())
		{
			RigidBodyController *rigidBody = nextList->First();
			while (rigidBody)
			{
				if (rigidBody->sleepStepCount < rigidBody->maxSleepStepCount) rigidBody->RecursiveKeepAwake();
				rigidBody = rigidBody->Next();
			}
			
			rigidBody = nextList->First();
			while (rigidBody)
			{
				RigidBodyController *next = rigidBody->Next();
				if (rigidBody->sleepStepCount >= rigidBody->maxSleepStepCount) rigidBody->Sleep();
				rigidBody = next;
			}
		}
		
		simulationStep++;
	}
	
	List<RigidBodyController> *moveList = &rigidBodyList[rigidBodyParity];
	
	float t = (float) time * kInversePhysicsTimeStep;
	float decay = TheTimeMgr->GetFloatDeltaTime() * 0.01F;
	
	int32 bodyCount = 0;
	RigidBodyController *rigidBody = moveList->First();
	while (rigidBody)
	{
		Matrix3D	rotation;
		
		const Point3D& cm = rigidBody->moveCenterOfMass;
		rotation.SetRotationAboutAxis(rigidBody->moveRotationAngle * t, rigidBody->moveRotationAxis);
		Transform4D transform(rotation, cm - rotation * cm + rigidBody->moveDisplacement * t);
		
		Vector3D delta = rigidBody->networkDelta[0] * rigidBody->networkDecay[0] + rigidBody->networkDelta[1] * rigidBody->networkDecay[1];
		rigidBody->networkDecay[0] = FmaxZero(rigidBody->networkDecay[0] - decay);
		rigidBody->networkDecay[1] = FmaxZero(rigidBody->networkDecay[1] - decay);
		
		Node *node = rigidBody->GetTargetNode();
		node->SetNodeTransform(transform * rigidBody->initialNodeTransform);
		node->SetNodePosition(node->GetNodePosition() - node->GetSuperNode()->GetInverseWorldTransform() * delta);
		node->Invalidate();
		
		bodyCount++;
		rigidBody = rigidBody->Next();
	}
	
	physicsCounter[kPhysicsCounterRigidBody] = bodyCount;
}


SetGravityFunction::SetGravityFunction() : Function(kFunctionSetGravity, kControllerPhysics)
{
	gravityAcceleration = 9.8F;
}

SetGravityFunction::SetGravityFunction(const SetGravityFunction& setGravityFunction) : Function(setGravityFunction)
{
	gravityAcceleration = setGravityFunction.gravityAcceleration;
}

SetGravityFunction::~SetGravityFunction()
{
}

Function *SetGravityFunction::Replicate(void) const
{
	return (new SetGravityFunction(*this));
}

void SetGravityFunction::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Function::Pack(data, packFlags);
	
	data << gravityAcceleration;
}

void SetGravityFunction::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Function::Unpack(data, unpackFlags);
	
	data >> gravityAcceleration;
}

void SetGravityFunction::Compress(Compressor& data) const
{
	Function::Compress(data);
	
	data << gravityAcceleration;
}

bool SetGravityFunction::Decompress(Decompressor& data)
{
	if (Function::Decompress(data))
	{
		data >> gravityAcceleration;
		return (gravityAcceleration >= 0.0F);
	}
	
	return (false);
}

int32 SetGravityFunction::GetSettingCount(void) const
{
	return (1);
}

Setting *SetGravityFunction::GetSetting(int32 index) const
{
	if (index == 0)
	{
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		const char *title = table->GetString(StringID('CTRL', kControllerPhysics, kFunctionSetGravity, 'GRAV'));
		return (new TextSetting('GRAV', gravityAcceleration, title));
	}
	
	return (nullptr);
}

void SetGravityFunction::SetSetting(const Setting *setting)
{
	Type identifier = setting->GetSettingIdentifier();
	
	if (identifier == 'GRAV')
	{
		const char *text = static_cast<const TextSetting *>(setting)->GetText();
		gravityAcceleration = FmaxZero(Text::StringToFloat(text));
	}
}

void SetGravityFunction::Execute(Controller *controller, FunctionMethod *method, const ScriptState *state)
{
	static_cast<PhysicsController *>(controller)->SetGravityAcceleration(Vector3D(0.0F, 0.0F, -gravityAcceleration));
	
	CallCompletionProc();
}


PhysicsNode::PhysicsNode() : Node(kNodePhysics)
{
}

PhysicsNode::~PhysicsNode()
{
}

void PhysicsNode::EnterZone(Zone *zone)
{
	if ((!zone->GetSuperNode()) && (!zone->GetPhysicsNode())) zone->SetPhysicsNode(this);
}


#if C4DIAGNOSTICS

	RigidBodyRenderable::RigidBodyRenderable(const Box3D& box) :
			Renderable(kRenderIndexedLines, kRenderDepthTest | kRenderDepthInhibit),
			diffuseColor(ColorRGBA(0.25F, 1.0F, 0.25F, 1.0F))
	{
		attributeList.Append(&diffuseColor);
		SetMaterialAttributeList(&attributeList);
		
		SetVertexCount(8);
		SetAttributeArray(kArrayVertex, vertexArray);
		SetLineArray(12, lineArray);
		
		SetCollisionBox(box);
	}
	
	RigidBodyRenderable::~RigidBodyRenderable()
	{
	}
	
	void RigidBodyRenderable::SetCollisionBox(const Box3D& box)
	{
		vertexArray[0] = box.min;
		vertexArray[1].Set(box.max.x, box.min.y, box.min.z);
		vertexArray[2].Set(box.min.x, box.max.y, box.min.z);
		vertexArray[3].Set(box.max.x, box.max.y, box.min.z);
		vertexArray[4].Set(box.min.x, box.min.y, box.max.z);
		vertexArray[5].Set(box.max.x, box.min.y, box.max.z);
		vertexArray[6].Set(box.min.x, box.max.y, box.max.z);
		vertexArray[7].Set(box.max.x, box.max.y, box.max.z);
	}
	
	
	ContactRenderable::ContactRenderable(const ColorRGBA& color, const char *texture) :
			Renderable(kRenderQuads),
			diffuseColor(color),
			textureMap(texture)
	{
		attributeList.Append(&diffuseColor);
		attributeList.Append(&textureMap);
		SetMaterialAttributeList(&attributeList);
		
		SetAmbientBlendState(kBlendInterpolate);
	}
	
	ContactRenderable::~ContactRenderable()
	{
	}
	
	
	ContactVectorRenderable::ContactVectorRenderable(const Subcontact *subcontact, const ColorRGBA& color) : ContactRenderable(color, "C4/vector")
	{
		SetAttributeArray(kArrayVertex, vertexArray);
		SetAttributeArray(kArrayTangent, tangentArray);
		SetAttributeArray(kArrayTexture0, texcoordArray);
		
		SetShaderFlags(kShaderVertexPolyboard);
		
		ContactVectorRenderable::UpdateContact(1, subcontact);
	}
	
	ContactVectorRenderable::~ContactVectorRenderable()
	{
	}
	
	void ContactVectorRenderable::UpdateContact(int32 count, const Subcontact *subcontact)
	{
		// Texture map is 32 x 256 pixels.
		// Quad is 0.02 x 0.16 meters.
		
		SetVertexCount(count * 4);
		
		Point3D *vertex = vertexArray;
		Vector4D *tangent = tangentArray;
		Point2D *texcoord = texcoordArray;
		
		for (machine a = 0; a < count; a++)
		{
			vertex[0] = subcontact->alphaPosition - subcontact->bodyNormal * 0.01F;
			vertex[1] = vertex[0];
			vertex[2] = subcontact->alphaPosition + subcontact->bodyNormal * 0.15F;
			vertex[3] = vertex[2];
			
			tangent[0].Set(subcontact->bodyNormal, -0.01F);
			tangent[1].Set(subcontact->bodyNormal, 0.01F);
			tangent[2].Set(subcontact->bodyNormal, 0.01F);
			tangent[3].Set(subcontact->bodyNormal, -0.01F);
			
			texcoord[0].Set(0.0F, 0.0F);
			texcoord[1].Set(1.0F, 0.0F);
			texcoord[2].Set(1.0F, 1.0F);
			texcoord[3].Set(0.0F, 1.0F);
			
			subcontact++;
			vertex += 4;
			tangent += 4;
			texcoord += 4;
		}
	}


	ContactPointRenderable::ContactPointRenderable(const Subcontact *subcontact, const ColorRGBA& color) : ContactRenderable(color, "C4/contact")
	{
		SetAttributeArray(kArrayVertex, vertexArray);
		SetAttributeArray(kArrayBillboard, billboardArray);
		SetAttributeArray(kArrayTexture0, texcoordArray);
		
		SetShaderFlags(kShaderVertexBillboard);
		
		ContactPointRenderable::UpdateContact(1, subcontact);
	}
	
	ContactPointRenderable::~ContactPointRenderable()
	{
	}
	
	void ContactPointRenderable::UpdateContact(int32 count, const Subcontact *subcontact)
	{
		SetVertexCount(count * 4);
		
		Point3D *vertex = vertexArray;
		Vector2D *billboard = billboardArray;
		Point2D *texcoord = texcoordArray;
		
		for (machine a = 0; a < count; a++)
		{
			vertex[0] = subcontact->betaPosition;
			vertex[1] = subcontact->betaPosition;
			vertex[2] = subcontact->betaPosition;
			vertex[3] = subcontact->betaPosition;
			
			billboard[0].Set(-0.02F, 0.02F);
			billboard[1].Set(0.02F, 0.02F);
			billboard[2].Set(0.02F, -0.02F);
			billboard[3].Set(-0.02F, -0.02F);
			
			texcoord[0].Set(0.0F, 0.0F);
			texcoord[1].Set(1.0F, 0.0F);
			texcoord[2].Set(1.0F, 1.0F);
			texcoord[3].Set(0.0F, 1.0F);
			
			subcontact++;
			vertex += 4;
			billboard += 4;
			texcoord += 4;
		}
	}

#endif

// ZYURVUR
