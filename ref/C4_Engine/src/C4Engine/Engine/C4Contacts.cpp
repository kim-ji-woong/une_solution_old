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
#include "C4Physics.h"
#include "C4World.h"
#include "C4Manipulator.h"
#include "C4Configuration.h"


using namespace C4;


const char C4::kConnectorKeyBody1[] = "%Body1";
const char C4::kConnectorKeyBody2[] = "%Body2";


namespace C4
{
	template <> Heap Memory<Contact>::heap("Contact", 65536, kHeapMutexless);
	template class Memory<Contact>;
}


Contact::Contact(ContactType type, Body *body1, Body *body2, unsigned_int32 mask) : GraphEdge<Body, Contact>(body1, body2)
{
	contactType = type;
	enabledFlag = true;
	
	notificationMask = mask;
	contactParam = 1.0F;
	
	ResetConstraintImpulses();
}

Contact::~Contact()
{
	#if C4DIAGNOSTICS
	
		delete contactPointRenderable.GetTarget();
		delete contactVectorRenderable.GetTarget();
	
	#endif
}

Contact *Contact::Construct(Unpacker& data, unsigned_int32 unpackFlags, Body *nullBody)
{
	switch (data.GetType())
	{
		case kContactRigidBody:
			
			return (new RigidBodyContact(nullBody));
		
		case kContactGeometry:
			
			return (new GeometryContact(nullBody));
		
		case kContactJoint:
			
			return (new JointContact(nullBody));
		
		case kContactStaticJoint:
			
			return (new StaticJointContact(nullBody));
	}
	
	return (nullptr);
}

void Contact::PackType(Packer& data) const
{
	data << contactType;
}

void Contact::Pack(Packer& data, unsigned_int32 packFlags) const
{
	data << ChunkHeader('EFLG', 4);
	data << enabledFlag;
	
	const Body *body = GetStartElement();
	if (body->GetBodyType() == kBodyRigid)
	{
		const Node *node = static_cast<const RigidBodyController *>(body)->GetTargetNode();
		if (node->LinkedNodePackable(packFlags))
		{
			data << ChunkHeader('STRT', 4);
			data << node->GetNodeIndex();
		}
	}
	
	body = GetFinishElement();
	if (body->GetBodyType() == kBodyRigid)
	{
		const Node *node = static_cast<const RigidBodyController *>(body)->GetTargetNode();
		if (node->LinkedNodePackable(packFlags))
		{
			data << ChunkHeader('FNSH', 4);
			data << node->GetNodeIndex();
		}
	}
	
	data << TerminatorChunk;
}

void Contact::Unpack(Unpacker& data, unsigned_int32 unpackFlags) 
{
	UnpackChunkList<Contact>(data, unpackFlags); 
} 
 
bool Contact::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{ 
	switch (chunkHeader->chunkType)
	{
		case 'EFLG':
			 
			data >> enabledFlag;
			return (true);
		
		case 'STRT': 
		{
			int32	nodeIndex;
			
			data >> nodeIndex;
			data.AddNodeLink(nodeIndex, &StartBodyLinkProc, this);
			return (true);
		}
		
		case 'FNSH':
		{
			int32	nodeIndex;
			
			data >> nodeIndex;
			data.AddNodeLink(nodeIndex, &FinishBodyLinkProc, this);
			return (true);
		}
	}
	
	return (false);
}

void Contact::StartBodyLinkProc(Node *node, void *cookie)
{
	Contact *contact = static_cast<Contact *>(cookie);
	contact->SetStartElement(static_cast<RigidBodyController *>(node->GetController()));
}

void Contact::FinishBodyLinkProc(Node *node, void *cookie)
{
	Contact *contact = static_cast<Contact *>(cookie);
	contact->SetFinishElement(static_cast<RigidBodyController *>(node->GetController()));
}

bool Contact::NonpersistentFinishNode(void) const
{
	return (false);
}

float Contact::CalculateConstraintImpulse(const RigidBodyController *rigidBody1, const RigidBodyController *rigidBody2, const Jacobian& jacobian1, const Jacobian& jacobian2, float velocityBoost)
{
	const Vector3D& linearVelocity1 = rigidBody1->GetLinearVelocity();
	const Vector3D& angularVelocity1 = rigidBody1->GetAngularVelocity();
	const Vector3D& linearVelocity2 = rigidBody2->GetLinearVelocity();
	const Vector3D& angularVelocity2 = rigidBody2->GetAngularVelocity();
	
	float inverseMass1 = rigidBody1->GetInverseBodyMass();
	const InertiaTensor& inverseInertiaTensor1 = rigidBody1->GetWorldInverseInertiaTensor();
	float inverseMass2 = rigidBody2->GetInverseBodyMass();
	const InertiaTensor& inverseInertiaTensor2 = rigidBody2->GetWorldInverseInertiaTensor();
	
	return (-(jacobian1.linear * linearVelocity1 + jacobian1.angular * angularVelocity1 + jacobian2.linear * linearVelocity2 + jacobian2.angular * angularVelocity2 + velocityBoost) / (jacobian1.linear * jacobian1.linear * inverseMass1 + jacobian1.angular * (inverseInertiaTensor1 * jacobian1.angular) + jacobian2.linear * jacobian2.linear * inverseMass2 + jacobian2.angular * (inverseInertiaTensor2 * jacobian2.angular)));
}

float Contact::CalculateConstraintImpulse(const RigidBodyController *rigidBody1, const RigidBodyController *rigidBody2, const Jacobian& jacobian1, const Jacobian& jacobian2)
{
	const Vector3D& linearVelocity1 = rigidBody1->GetLinearVelocity();
	const Vector3D& angularVelocity1 = rigidBody1->GetAngularVelocity();
	const Vector3D& linearVelocity2 = rigidBody2->GetLinearVelocity();
	const Vector3D& angularVelocity2 = rigidBody2->GetAngularVelocity();
	
	float inverseMass1 = rigidBody1->GetInverseBodyMass();
	const InertiaTensor& inverseInertiaTensor1 = rigidBody1->GetWorldInverseInertiaTensor();
	float inverseMass2 = rigidBody2->GetInverseBodyMass();
	const InertiaTensor& inverseInertiaTensor2 = rigidBody2->GetWorldInverseInertiaTensor();
	
	return (-(jacobian1.linear * linearVelocity1 + jacobian1.angular * angularVelocity1 + jacobian2.linear * linearVelocity2 + jacobian2.angular * angularVelocity2) / (jacobian1.linear * jacobian1.linear * inverseMass1 + jacobian1.angular * (inverseInertiaTensor1 * jacobian1.angular) + jacobian2.linear * jacobian2.linear * inverseMass2 + jacobian2.angular * (inverseInertiaTensor2 * jacobian2.angular)));
}

float Contact::CalculateConstraintImpulse(const RigidBodyController *rigidBody, const Jacobian& jacobian, float velocityBoost)
{
	const Vector3D& linearVelocity = rigidBody->GetLinearVelocity();
	const Vector3D& angularVelocity = rigidBody->GetAngularVelocity();
	
	float inverseMass = rigidBody->GetInverseBodyMass();
	const InertiaTensor& inverseInertiaTensor = rigidBody->GetWorldInverseInertiaTensor();
	
	return (-(jacobian.linear * linearVelocity + jacobian.angular * angularVelocity + velocityBoost) / (jacobian.linear * jacobian.linear * inverseMass + jacobian.angular * (inverseInertiaTensor * jacobian.angular)));
}

float Contact::CalculateConstraintImpulse(const RigidBodyController *rigidBody, const Jacobian& jacobian)
{
	const Vector3D& linearVelocity = rigidBody->GetLinearVelocity();
	const Vector3D& angularVelocity = rigidBody->GetAngularVelocity();
	
	float inverseMass = rigidBody->GetInverseBodyMass();
	const InertiaTensor& inverseInertiaTensor = rigidBody->GetWorldInverseInertiaTensor();
	
	return (-(jacobian.linear * linearVelocity + jacobian.angular * angularVelocity) / (jacobian.linear * jacobian.linear * inverseMass + jacobian.angular * (inverseInertiaTensor * jacobian.angular)));
}

float Contact::CalculateLinearConstraintImpulse(const RigidBodyController *rigidBody, const Vector3D& linearJacobian, float velocityBoost)
{
	const Vector3D& linearVelocity = rigidBody->GetLinearVelocity();
	float inverseMass = rigidBody->GetInverseBodyMass();
	
	return (-(linearJacobian * linearVelocity + velocityBoost) / (linearJacobian * linearJacobian * inverseMass));
}

float Contact::CalculateLinearConstraintImpulse(const RigidBodyController *rigidBody, const Vector3D& linearJacobian)
{
	const Vector3D& linearVelocity = rigidBody->GetLinearVelocity();
	float inverseMass = rigidBody->GetInverseBodyMass();
	
	return (-(linearJacobian * linearVelocity) / (linearJacobian * linearJacobian * inverseMass));
}

float Contact::CalculateAngularConstraintImpulse(const RigidBodyController *rigidBody, const Vector3D& angularJacobian, float velocityBoost)
{
	const Vector3D& angularVelocity = rigidBody->GetAngularVelocity();
	const InertiaTensor& inverseInertiaTensor = rigidBody->GetWorldInverseInertiaTensor();
	
	return (-(angularJacobian * angularVelocity + velocityBoost) / (angularJacobian * (inverseInertiaTensor * angularJacobian)));
}

float Contact::CalculateAngularConstraintImpulse(const RigidBodyController *rigidBody, const Vector3D& angularJacobian)
{
	const Vector3D& angularVelocity = rigidBody->GetAngularVelocity();
	const InertiaTensor& inverseInertiaTensor = rigidBody->GetWorldInverseInertiaTensor();
	
	return (-(angularJacobian * angularVelocity) / (angularJacobian * (inverseInertiaTensor * angularJacobian)));
}

float Contact::AccumulateConstraintImpulse(int32 index, float multiplier, const Range<float>& range)
{
	float m = (cumulativeImpulse[index] += multiplier);
	m = Clamp(m, range.min, range.max);
	multiplier = m - appliedImpulse[index];
	appliedImpulse[index] = m;
	return (multiplier);
}

float Contact::AccumulateConstraintImpulse(int32 index, float multiplier)
{
	float m = (cumulativeImpulse[index] += multiplier);
	multiplier = m - appliedImpulse[index];
	appliedImpulse[index] = m;
	return (multiplier);
}

void Contact::ApplyConstraints(void)
{
}


CollisionContact::CollisionContact(ContactType type, Body *body1, Body *body2, unsigned_int32 mask) : Contact(type, body1, body2, mask)
{
	cachedSimplexVertexCount = 0;
}

CollisionContact::~CollisionContact()
{
}

void CollisionContact::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Contact::Pack(data, packFlags);
	
	int32 count = cachedSimplexVertexCount;
	data << ChunkHeader('SPLX', 4 + count * (sizeof(Point3D) * 2));
	data << count;
	
	for (machine a = 0; a < count; a++)
	{
		data << cachedAlphaVertex[a];
		data << cachedBetaVertex[a];
	}
	
	count = subcontactCount;
	data << ChunkHeader('SBCT', 4 + count * (sizeof(Point3D) * 2 + sizeof(Vector3D) * 3 + 4));
	data << count;
	
	for (machine a = 0; a < count; a++)
	{
		data << subcontact[a].activeStep;
		data << subcontact[a].triangleIndex;
		data << subcontact[a].alphaPosition;
		data << subcontact[a].betaPosition;
		data << subcontact[a].bodyNormal;
		data << subcontact[a].bodyTangent[0];
		data << subcontact[a].bodyTangent[1];
	}
	
	data << TerminatorChunk;
}

void CollisionContact::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Contact::Unpack(data, unpackFlags);
	UnpackChunkList<CollisionContact>(data, unpackFlags);
}

bool CollisionContact::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'SPLX':
		{
			int32	count;
			
			data >> count;
			cachedSimplexVertexCount = count;
			
			for (machine a = 0; a < count; a++)
			{
				data >> cachedAlphaVertex[a];
				data >> cachedBetaVertex[a];
			}
			
			return (true);
		}
		
		case 'SBCT':
		{
			int32	count;
			
			data >> count;
			subcontactCount = count;
			
			for (machine a = 0; a < count; a++)
			{
				data >> subcontact[a].activeStep;
				data >> subcontact[a].triangleIndex;
				data >> subcontact[a].alphaPosition;
				data >> subcontact[a].betaPosition;
				data >> subcontact[a].bodyNormal;
				data >> subcontact[a].bodyTangent[0];
				data >> subcontact[a].bodyTangent[1];
			}
			
			return (true);
		}
	}
	
	return (false);
}


RigidBodyContact::RigidBodyContact(RigidBodyController *body1, RigidBodyController *body2, const Shape *shape1, const Shape *shape2, unsigned_int32 signature1, unsigned_int32 signature2, const IntersectionData *intersectionData) : CollisionContact(kContactRigidBody, body1, body2, kContactNotificationOutgoing | kContactNotificationIncoming)
{
	startShape = shape1;
	finishShape = shape2;
	startSignature = signature1;
	finishSignature = signature2;
	SetContactParam(intersectionData->contactParam);
	
	subcontactCount = 1;
	
	subcontact[0].activeStep = body1->GetPhysicsController()->GetSimulationStep();
	subcontact[0].triangleIndex = 0;
	
	subcontact[0].alphaPosition = AdjugateTransform(body1->GetFinalWorldTransform(), intersectionData->alphaContact);
	subcontact[0].betaPosition = AdjugateTransform(body2->GetFinalWorldTransform(), intersectionData->betaContact);
	subcontact[0].bodyNormal = intersectionData->contactNormal * body1->GetFinalWorldTransform();
	
	subcontact[0].bodyTangent[0] = Math::CreateUnitPerpendicular(subcontact[0].bodyNormal);
	subcontact[0].bodyTangent[1] = subcontact[0].bodyNormal % subcontact[0].bodyTangent[0];
	
	if ((body1->GetRigidBodyFlags() | body2->GetRigidBodyFlags()) & kRigidBodyDisabledContact) Disable();
	
	#if C4DIAGNOSTICS
	
		const Node *node = body1->GetTargetNode();
		World *world = node->GetWorld();
		
		if (world->GetDiagnosticFlags() & kDiagnosticContacts)
		{
			ColorRGBA color(1.0F, 1.0F, 1.0F);
			
			unsigned_int32 index = Math::Random(3);
			color[index] = Math::RandomFloat(0.5F) + 0.5F;
			color[IncMod<3>(index)] = Math::RandomFloat(0.5F) + 0.5F;
			
			ContactRenderable *renderable = new ContactVectorRenderable(subcontact, color);
			renderable->SetTransformable(node);
			contactVectorRenderable = renderable;
			world->AddContactRenderable(renderable);
			
			renderable = new ContactPointRenderable(subcontact, color);
			renderable->SetTransformable(body2->GetTargetNode());
			contactPointRenderable = renderable;
			world->AddContactRenderable(renderable);
		}
	
	#endif
}

RigidBodyContact::RigidBodyContact(Body *nullBody) : CollisionContact(kContactRigidBody, nullBody, nullBody)
{
}

RigidBodyContact::~RigidBodyContact()
{
}

void RigidBodyContact::Pack(Packer& data, unsigned_int32 packFlags) const
{
	CollisionContact::Pack(data, packFlags);
	
	data << ChunkHeader('SIGN', 8);
	data << startSignature;
	data << finishSignature;
	
	data << TerminatorChunk;
}

void RigidBodyContact::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	CollisionContact::Unpack(data, unpackFlags);
	UnpackChunkList<RigidBodyContact>(data, unpackFlags);
}

bool RigidBodyContact::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'SIGN':
			
			data >> startSignature;
			data >> finishSignature;
			return (true);
	}
	
	return (false);
}

bool RigidBodyContact::NonpersistentFinishNode(void) const
{
	return ((static_cast<const RigidBodyController *>(GetFinishElement())->GetTargetNode()->GetNodeFlags() & kNodeNonpersistent) != 0);
}

void RigidBodyContact::UpdateContact(const IntersectionData *intersectionData)
{
	Vector3D	v[kMaxSubcontactCount];
	float		delta[kMaxSubcontactCount];
	
	RigidBodyController *rigidBody1 = static_cast<RigidBodyController *>(GetStartElement());
	RigidBodyController *rigidBody2 = static_cast<RigidBodyController *>(GetFinishElement());
	
	Point3D alphaPosition = AdjugateTransform(rigidBody1->GetFinalWorldTransform(), intersectionData->alphaContact);
	
	for (machine a = 0; a < subcontactCount; a++)
	{
		v[a] = subcontact[a].alphaPosition - alphaPosition;
		delta[a] = SquaredMag(v[a]);
	}
	
	machine index = 0;
	float minDelta = 0.0F;
	if (subcontactCount > 0)
	{
		minDelta = delta[0];
		for (machine a = 1; a < subcontactCount; a++)
		{
			if (delta[a] < minDelta)
			{
				minDelta = delta[a];
				index = a;
			}
		}
	}
	
	if (minDelta > kMaxSubcontactSquaredDelta)
	{
		index = subcontactCount;
		if (index == kMaxSubcontactCount)
		{
			float m1 = Magnitude(v[0] % v[1]);
			float m2 = Magnitude(v[1] % v[2]);
			float m3 = Magnitude(v[2] % v[3]);
			float m4 = Magnitude(v[3] % v[0]);
			float area1 = m2 + m3;
			float area2 = m3 + m4;
			float area3 = m4 + m1;
			float area4 = m1 + m2;
			
			index = 0;
			
			if (area2 > area1)
			{
				area1 = area2;
				index = 1;
			}
			
			if (area3 > area1)
			{
				area1 = area3;
				index = 2;
			}
			
			if (area4 > area1) index = 3;
		}
		else
		{
			subcontactCount = index + 1;
		}
	}
	
	subcontact[index].activeStep = rigidBody1->GetPhysicsController()->GetSimulationStep();
	subcontact[index].triangleIndex = 0;
	
	subcontact[index].alphaPosition = alphaPosition;
	subcontact[index].betaPosition = AdjugateTransform(rigidBody2->GetFinalWorldTransform(), intersectionData->betaContact);
	subcontact[index].bodyNormal = intersectionData->contactNormal * rigidBody1->GetFinalWorldTransform();
	
	subcontact[index].bodyTangent[0] = Math::CreateUnitPerpendicular(subcontact[index].bodyNormal);
	subcontact[index].bodyTangent[1] = subcontact[index].bodyNormal % subcontact[index].bodyTangent[0];
	
	ResetConstraintImpulses();
	
	#if C4DIAGNOSTICS
	
		ContactRenderable *renderable = contactVectorRenderable;
		if (renderable) renderable->UpdateContact(subcontactCount, subcontact);
		
		renderable = contactPointRenderable;
		if (renderable) renderable->UpdateContact(subcontactCount, subcontact);
	
	#endif
}

void RigidBodyContact::ApplyConstraints(void)
{
	RigidBodyController *rigidBody1 = static_cast<RigidBodyController *>(GetStartElement());
	RigidBodyController *rigidBody2 = static_cast<RigidBodyController *>(GetFinishElement());
	
	const Transform4D& transform1 = rigidBody1->GetFinalWorldTransform();
	const Transform4D& transform2 = rigidBody2->GetFinalWorldTransform();
	
	Point3D cm1 = transform1 * rigidBody1->GetCenterOfMass();
	Point3D cm2 = transform2 * rigidBody2->GetCenterOfMass();
	
	int32 simulationStep = rigidBody1->GetPhysicsController()->GetSimulationStep();
	
	for (machine a = 0; a < subcontactCount;)
	{
		Point3D alphaWorldPosition = transform1 * subcontact[a].alphaPosition;
		Point3D betaWorldPosition = transform2 * subcontact[a].betaPosition;
		Vector3D deltaWorldPosition = betaWorldPosition - alphaWorldPosition;
		Vector3D worldNormal = transform1 * subcontact[a].bodyNormal;
		float separation = deltaWorldPosition * worldNormal;
		
		machine constraintIndex = a * 3;
		
		if (SquaredMag(deltaWorldPosition) - separation * separation < kMaxSubcontactSquaredTangentialSeparation)
		{
			if (separation < -kContactEpsilon)
			{
				subcontact[a].activeStep = simulationStep;
				
				Jacobian separationJacobian1(-worldNormal, worldNormal % (betaWorldPosition - cm1));
				Jacobian separationJacobian2(worldNormal, (betaWorldPosition - cm2) % worldNormal);
				
				float velocityBoost = FminZero(separation - kContactEpsilon) * kContactStabilizeFactor;
				float separationImpulse = CalculateConstraintImpulse(rigidBody1, rigidBody2, separationJacobian1, separationJacobian2, velocityBoost);
				separationImpulse = AccumulateConstraintImpulse(constraintIndex, separationImpulse, Range<float>(0.0F, K::infinity));
				rigidBody1->ApplyVelocityCorrection(separationJacobian1, separationImpulse);
				rigidBody2->ApplyVelocityCorrection(separationJacobian2, separationImpulse);
				
				float frictionCoefficient = Fmin(rigidBody1->GetFrictionCoefficient(), rigidBody2->GetFrictionCoefficient());
				if (frictionCoefficient > 0.0F)
				{
					Vector3D tangent1 = transform1 * subcontact[a].bodyTangent[0];
					Vector3D tangent2 = transform1 * subcontact[a].bodyTangent[1];
					
					Vector3D v1 = alphaWorldPosition - cm1;
					Vector3D v2 = betaWorldPosition - cm2;
					Jacobian frictionJacobianX1(tangent1, v1 % tangent1);
					Jacobian frictionJacobianX2(-tangent1, tangent1 % v2);
					Jacobian frictionJacobianY1(tangent2, v1 % tangent2);
					Jacobian frictionJacobianY2(-tangent2, tangent2 % v2);
					
					float frictionImpulseX = CalculateConstraintImpulse(rigidBody1, rigidBody2, frictionJacobianX1, frictionJacobianX2);
					float frictionImpulseY = CalculateConstraintImpulse(rigidBody1, rigidBody2, frictionJacobianY1, frictionJacobianY2);
					
					float m = Sqrt(frictionImpulseX * frictionImpulseX + frictionImpulseY * frictionImpulseY);
					if (m > 0.0F)
					{
						float frictionLimit = FmaxZero(cumulativeImpulse[constraintIndex]) * frictionCoefficient;
						m = Fmin(m, frictionLimit) / m;
						
						frictionImpulseX = AccumulateConstraintImpulse(constraintIndex + 1, frictionImpulseX * m);
						frictionImpulseY = AccumulateConstraintImpulse(constraintIndex + 2, frictionImpulseY * m);
						rigidBody1->ApplyVelocityCorrection(frictionJacobianX1, frictionImpulseX);
						rigidBody1->ApplyVelocityCorrection(frictionJacobianY1, frictionImpulseY);
						rigidBody2->ApplyVelocityCorrection(frictionJacobianX2, frictionImpulseX);
						rigidBody2->ApplyVelocityCorrection(frictionJacobianY2, frictionImpulseY);
					}
				}
				
				a++;
				continue;
			}
		}
		
		int32 inactiveStepCount = simulationStep - subcontact[a].activeStep;
		if (inactiveStepCount >= 4)
		{
			int32 count = subcontactCount - 1;
			if (count != 0)
			{
				subcontactCount = count;
				for (machine b = a; b < subcontactCount; b++) subcontact[b] = subcontact[b + 1];
				
				#if C4DIAGNOSTICS
				
					ContactRenderable *renderable = contactVectorRenderable;
					if (renderable) renderable->UpdateContact(count, subcontact);
					
					renderable = contactPointRenderable;
					if (renderable) renderable->UpdateContact(count, subcontact);
				
				#endif
				
				continue;
			}
			else
			{
				delete this;
				return;
			}
		}
		
		a++;
	}
}

void RigidBodyContact::GetWorldContactPosition(const RigidBodyController *rigidBody, Point3D *position, Vector3D *normal) const
{
	if (GetStartElement() == rigidBody)
	{
		*position = static_cast<RigidBodyController *>(GetFinishElement())->GetFinalWorldTransform() * subcontact[0].betaPosition;
		*normal = rigidBody->GetFinalWorldTransform() * subcontact[0].bodyNormal;
	}
	else
	{
		const Transform4D& worldTransform = static_cast<RigidBodyController *>(GetStartElement())->GetFinalWorldTransform();
		*position = worldTransform * subcontact[0].alphaPosition;
		*normal = worldTransform * -subcontact[0].bodyNormal;
	}
}


GeometryContact::GeometryContact(Geometry *geometry, RigidBodyController *body, const IntersectionData *intersectionData, unsigned_int32 signature) : CollisionContact(kContactGeometry, body, body->GetPhysicsController()->GetNullBody(), kContactNotificationOutgoing)
{
	contactGeometry = geometry;
	contactSignature = signature;
	SetContactParam(intersectionData->contactParam);
	
	subcontactCount = 1;
	
	subcontact[0].activeStep = body->GetPhysicsController()->GetSimulationStep();
	subcontact[0].triangleIndex = intersectionData->triangleIndex;
	
	subcontact[0].alphaPosition = AdjugateTransform(body->GetFinalWorldTransform(), intersectionData->alphaContact);
	subcontact[0].betaPosition = geometry->GetInverseWorldTransform() * intersectionData->betaContact;
	subcontact[0].bodyNormal = intersectionData->contactNormal * body->GetFinalWorldTransform();
	
	subcontact[0].bodyTangent[0] = Math::CreateUnitPerpendicular(subcontact[0].bodyNormal);
	subcontact[0].bodyTangent[1] = subcontact[0].bodyNormal % subcontact[0].bodyTangent[0];
	
	normalVelocityMagnitude = FmaxZero(body->GetLinearVelocity() * intersectionData->contactNormal);
	
	if (body->GetRigidBodyFlags() & kRigidBodyDisabledContact) Disable();
	
	#if C4DIAGNOSTICS
	
		const Node *node = body->GetTargetNode();
		World *world = node->GetWorld();
		
		if (world->GetDiagnosticFlags() & kDiagnosticContacts)
		{
			ColorRGBA color(1.0F, 1.0F, 1.0F);
			
			unsigned_int32 index = Math::Random(3);
			color[index] = Math::RandomFloat(0.5F) + 0.5F;
			color[IncMod<3>(index)] = Math::RandomFloat(0.5F) + 0.5F;
			
			ContactRenderable *renderable = new ContactVectorRenderable(subcontact, color);
			renderable->SetTransformable(node);
			contactVectorRenderable = renderable;
			world->AddContactRenderable(renderable);
			
			renderable = new ContactPointRenderable(subcontact, color);
			renderable->SetTransformable(geometry);
			contactPointRenderable = renderable;
			world->AddContactRenderable(renderable);
		}
	
	#endif
}

GeometryContact::GeometryContact(Body *nullBody) : CollisionContact(kContactGeometry, nullBody, nullBody)
{
}

GeometryContact::~GeometryContact()
{
}

void GeometryContact::Pack(Packer& data, unsigned_int32 packFlags) const
{
	CollisionContact::Pack(data, packFlags);
	
	if (contactGeometry->LinkedNodePackable(packFlags))
	{
		data << ChunkHeader('GEOM', 4);
		data << contactGeometry->GetNodeIndex();
	}
	
	data << ChunkHeader('SIGN', 4);
	data << contactSignature;
	
	data << ChunkHeader('NVMG', 4);
	data << normalVelocityMagnitude;
	
	data << TerminatorChunk;
}

void GeometryContact::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	CollisionContact::Unpack(data, unpackFlags);
	UnpackChunkList<GeometryContact>(data, unpackFlags);
}

bool GeometryContact::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'GEOM':
		{
			int32	nodeIndex;
			
			data >> nodeIndex;
			data.AddNodeLink(nodeIndex, &GeometryLinkProc, this);
			return (true);
		}
		
		case 'SIGN':
			
			data >> contactSignature;
			return (true);
		
		case 'NVMG':
			
			data >> normalVelocityMagnitude;
			return (true);
	}
	
	return (false);
}

void GeometryContact::GeometryLinkProc(Node *node, void *cookie)
{
	GeometryContact *geometryContact = static_cast<GeometryContact *>(cookie);
	geometryContact->contactGeometry = static_cast<Geometry *>(node);
}

bool GeometryContact::NonpersistentFinishNode(void) const
{
	return ((contactGeometry->GetNodeFlags() & kNodeNonpersistent) != 0);
}

void GeometryContact::UpdateContact(const IntersectionData *intersectionData)
{
	Vector3D	v[kMaxSubcontactCount];
	float		delta[kMaxSubcontactCount];
	
	RigidBodyController *rigidBody = static_cast<RigidBodyController *>(GetStartElement());
	Point3D alphaPosition = AdjugateTransform(rigidBody->GetFinalWorldTransform(), intersectionData->alphaContact);
	
	for (machine a = 0; a < subcontactCount; a++)
	{
		v[a] = subcontact[a].alphaPosition - alphaPosition;
		delta[a] = SquaredMag(v[a]);
	}
	
	machine index = 0;
	float minDelta = 0.0F;
	if (subcontactCount > 0)
	{
		minDelta = delta[0];
		for (machine a = 1; a < subcontactCount; a++)
		{
			if (delta[a] < minDelta)
			{
				minDelta = delta[a];
				index = a;
			}
		}
	}
	
	if (minDelta > kMaxSubcontactSquaredDelta)
	{
		index = subcontactCount;
		if (index == kMaxSubcontactCount)
		{
			float m1 = Magnitude(v[0] % v[1]);
			float m2 = Magnitude(v[1] % v[2]);
			float m3 = Magnitude(v[2] % v[3]);
			float m4 = Magnitude(v[3] % v[0]);
			float area1 = m2 + m3;
			float area2 = m3 + m4;
			float area3 = m4 + m1;
			float area4 = m1 + m2;
			
			index = 0;
			
			if (area2 > area1)
			{
				area1 = area2;
				index = 1;
			}
			
			if (area3 > area1)
			{
				area1 = area3;
				index = 2;
			}
			
			if (area4 > area1) index = 3;
		}
		else
		{
			subcontactCount = index + 1;
		}
	}
	
	subcontact[index].activeStep = rigidBody->GetPhysicsController()->GetSimulationStep();
	subcontact[index].triangleIndex = intersectionData->triangleIndex;
	
	subcontact[index].alphaPosition = alphaPosition;
	subcontact[index].betaPosition = contactGeometry->GetInverseWorldTransform() * intersectionData->betaContact;
	subcontact[index].bodyNormal = intersectionData->contactNormal * rigidBody->GetFinalWorldTransform();
	
	subcontact[index].bodyTangent[0] = Math::CreateUnitPerpendicular(subcontact[index].bodyNormal);
	subcontact[index].bodyTangent[1] = subcontact[index].bodyNormal % subcontact[index].bodyTangent[0];
	
	normalVelocityMagnitude = FmaxZero(rigidBody->GetLinearVelocity() * intersectionData->contactNormal);
	
	ResetConstraintImpulses();
	
	#if C4DIAGNOSTICS
	
		ContactRenderable *renderable = contactVectorRenderable;
		if (renderable) renderable->UpdateContact(subcontactCount, subcontact);
		
		renderable = contactPointRenderable;
		if (renderable) renderable->UpdateContact(subcontactCount, subcontact);
	
	#endif
}

void GeometryContact::ApplyConstraints(void)
{
	RigidBodyController *rigidBody = static_cast<RigidBodyController *>(GetStartElement());
	const Transform4D& transform = rigidBody->GetFinalWorldTransform();
	Point3D cm = transform * rigidBody->GetCenterOfMass();
	
	int32 simulationStep = rigidBody->GetPhysicsController()->GetSimulationStep();
	
	for (machine a = 0; a < subcontactCount;)
	{
		Point3D rigidBodyWorldPosition = transform * subcontact[a].alphaPosition;
		Point3D geometryWorldPosition = contactGeometry->GetWorldTransform() * subcontact[a].betaPosition;
		Vector3D deltaWorldPosition = geometryWorldPosition - rigidBodyWorldPosition;
		Vector3D worldNormal = transform * subcontact[a].bodyNormal;
		
		float separation = deltaWorldPosition * worldNormal;
		if (SquaredMag(deltaWorldPosition) - separation * separation < kMaxSubcontactSquaredTangentialSeparation)
		{
			if (separation < -kContactEpsilon)
			{
				subcontact[a].activeStep = simulationStep;
				
				Jacobian separationJacobian(-worldNormal, worldNormal % (geometryWorldPosition - cm));
				float velocityBoost = FminZero(separation - kContactEpsilon) * kContactStabilizeFactor - normalVelocityMagnitude * rigidBody->GetRestitutionCoefficient();
				float separationImpulse = CalculateConstraintImpulse(rigidBody, separationJacobian, velocityBoost);
				
				machine constraintIndex = a * 3;
				separationImpulse = AccumulateConstraintImpulse(constraintIndex, separationImpulse, Range<float>(0.0F, K::infinity));
				rigidBody->ApplyVelocityCorrection(separationJacobian, separationImpulse);
				
				float frictionCoefficient = rigidBody->GetFrictionCoefficient();
				if (frictionCoefficient > 0.0F)
				{
					Vector3D tangent1 = transform * subcontact[a].bodyTangent[0];
					Vector3D tangent2 = transform * subcontact[a].bodyTangent[1];
					
					Vector3D v = rigidBodyWorldPosition - cm;
					Jacobian frictionJacobianX(tangent1, v % tangent1);
					Jacobian frictionJacobianY(tangent2, v % tangent2);
					
					float frictionImpulseX = CalculateConstraintImpulse(rigidBody, frictionJacobianX);
					float frictionImpulseY = CalculateConstraintImpulse(rigidBody, frictionJacobianY);
					
					float m = Sqrt(frictionImpulseX * frictionImpulseX + frictionImpulseY * frictionImpulseY);
					if (m > 0.0F)
					{
						float frictionLimit = FmaxZero(cumulativeImpulse[constraintIndex]) * frictionCoefficient;
						m = Fmin(m, frictionLimit) / m;
						
						frictionImpulseX = AccumulateConstraintImpulse(constraintIndex + 1, frictionImpulseX * m);
						frictionImpulseY = AccumulateConstraintImpulse(constraintIndex + 2, frictionImpulseY * m);
						rigidBody->ApplyVelocityCorrection(frictionJacobianX, frictionImpulseX);
						rigidBody->ApplyVelocityCorrection(frictionJacobianY, frictionImpulseY);
					}
				}
				
				a++;
				continue;
			}
		}
		
		int32 inactiveStepCount = simulationStep - subcontact[a].activeStep;
		if (inactiveStepCount >= 4)
		{
			int32 count = subcontactCount - 1;
			if (count != 0)
			{
				subcontactCount = count;
				for (machine b = a; b < subcontactCount; b++) subcontact[b] = subcontact[b + 1];
				
				#if C4DIAGNOSTICS
				
					ContactRenderable *renderable = contactVectorRenderable;
					if (renderable) renderable->UpdateContact(count, subcontact);
					
					renderable = contactPointRenderable;
					if (renderable) renderable->UpdateContact(count, subcontact);
				
				#endif
				
				continue;
			}
			else
			{
				delete this;
				return;
			}
		}
		
		a++;
	}
}


JointContact::JointContact(Joint *joint, RigidBodyController *body1, RigidBodyController *body2) : Contact(kContactJoint, body1, body2)
{
	contactJoint = joint;
}

JointContact::JointContact(ContactType type, Joint *joint, Body *body1, Body *body2) : Contact(type, body1, body2)
{
	contactJoint = joint;
}

JointContact::JointContact(Body *nullBody) : Contact(kContactJoint, nullBody, nullBody)
{
}

JointContact::JointContact(ContactType type, Body *nullBody) : Contact(type, nullBody, nullBody)
{
}

JointContact::~JointContact()
{
}

void JointContact::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Contact::Pack(data, packFlags);
	
	if (contactJoint->LinkedNodePackable(packFlags))
	{
		data << ChunkHeader('JONT', 4);
		data << contactJoint->GetNodeIndex();
	}
	
	data << TerminatorChunk;
}

void JointContact::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Contact::Unpack(data, unpackFlags);
	UnpackChunkList<JointContact>(data, unpackFlags);
}

bool JointContact::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'JONT':
		{
			int32	nodeIndex;
			
			data >> nodeIndex;
			data.AddNodeLink(nodeIndex, &JointLinkProc, this);
			return (true);
		}
	}
	
	return (false);
}

void JointContact::JointLinkProc(Node *node, void *cookie)
{
	JointContact *jointContact = static_cast<JointContact *>(cookie);
	jointContact->contactJoint = static_cast<Joint *>(node);
}

void JointContact::ApplyConstraints(void)
{
	contactJoint->ApplyConstraints(this);
}


StaticJointContact::StaticJointContact(Joint *joint, RigidBodyController *body) : JointContact(kContactStaticJoint, joint, body, joint->GetNullBody())
{
}

StaticJointContact::StaticJointContact(Body *nullBody) : JointContact(kContactStaticJoint, nullBody)
{
}

StaticJointContact::~StaticJointContact()
{
}

void StaticJointContact::ApplyConstraints(void)
{
	GetContactJoint()->ApplyStaticContraints(this);
}


JointObject::JointObject(JointType type) : Object(kObjectJoint)
{
	jointType = type;
	
	jointFlags = 0;
	breakingForce = 0.0F;
}

JointObject::~JointObject()
{
}

JointObject *JointObject::Construct(Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (data.GetType())
	{
		case kJointSpherical:
			
			return (new SphericalJointObject);
		
		case kJointUniversal:
			
			return (new UniversalJointObject);
		
		case kJointDiscal:
			
			return (new DiscalJointObject);
		
		case kJointCylindrical:
			
			return (new CylindricalJointObject);
		
		case kJointRevolute:
			
			return (new RevoluteJointObject);
		
		case kJointPrismatic:
			
			return (new PrismaticJointObject);
	}
	
	return (nullptr);
}

void JointObject::PackType(Packer& data) const
{
	Object::PackType(data);
	data << jointType;
}

void JointObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	data << ChunkHeader('FLAG', 4);
	data << jointFlags;
	
	data << ChunkHeader('BFRC', 4);
	data << breakingForce;
	
	data << TerminatorChunk;
}

void JointObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	UnpackChunkList<JointObject>(data, unpackFlags);
}

bool JointObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'FLAG':
			
			data >> jointFlags;
			return (true);
		
		case 'BFRC':
			
			data >> breakingForce;
			return (true);
	}
	
	return (false);
}

int32 JointObject::GetCategoryCount(void) const
{
	return (1);
}

Type JointObject::GetCategoryType(int32 index, const char **title) const
{
	if (index == 0)
	{
		*title = TheInterfaceMgr->GetStringTable()->GetString(StringID(kObjectJoint));
		return (kObjectJoint);
	}
	
	return (0);
}

int32 JointObject::GetCategorySettingCount(Type category) const
{
	if (category == kObjectJoint) return (3);
	return (0);
}

Setting *JointObject::GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const
{
	if (category == kObjectJoint)
	{
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		
		if (index == 0)
		{
			const char *title = table->GetString(StringID(kObjectJoint, 'JONT'));
			return (new HeadingSetting(kObjectJoint, title));
		}
		
		if (index == 1)
		{
			const char *title = table->GetString(StringID(kObjectJoint, 'JONT', 'BRAK'));
			return (new BooleanSetting('BRAK', ((jointFlags & kJointBreakable) != 0), title));
		}
		
		if (index == 2)
		{
			const char *title = table->GetString(StringID(kObjectJoint, 'JONT', 'BFRC'));
			return (new TextSetting('BFRC', breakingForce, title));
		}
	}
	
	return (nullptr);
}

void JointObject::SetCategorySetting(Type category, const Setting *setting)
{
	if (category == kObjectJoint)
	{
		Type identifier = setting->GetSettingIdentifier();
		
		if (identifier == 'BRAK')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) jointFlags |= kJointBreakable;
			else jointFlags &= ~kJointBreakable;
		}
		else if (identifier == 'BFRC')
		{
			breakingForce = Fmax(Text::StringToFloat(static_cast<const TextSetting *>(setting)->GetText()), 0.0F);
		}
	}
}


SphericalJointObject::SphericalJointObject() : JointObject(kJointSpherical)
{
}

SphericalJointObject::~SphericalJointObject()
{
}


UniversalJointObject::UniversalJointObject() : JointObject(kJointUniversal)
{
}

UniversalJointObject::~UniversalJointObject()
{
}


DiscalJointObject::DiscalJointObject() : JointObject(kJointDiscal)
{
}

DiscalJointObject::~DiscalJointObject()
{
}


RevoluteJointObject::RevoluteJointObject() : JointObject(kJointRevolute)
{
}

RevoluteJointObject::~RevoluteJointObject()
{
}


CylindricalJointObject::CylindricalJointObject() : JointObject(kJointCylindrical)
{
}

CylindricalJointObject::~CylindricalJointObject()
{
}


PrismaticJointObject::PrismaticJointObject() : JointObject(kJointPrismatic)
{
}

PrismaticJointObject::~PrismaticJointObject()
{
}


Joint::Joint(JointType type) : Node(kNodeJoint)
{
	jointType = type;
}

Joint::Joint(const Joint& joint) : Node(joint)
{
	jointType = joint.jointType;
}

Joint::~Joint()
{
	JointContact *contact = jointContact;
	delete contact;
}

Joint *Joint::Construct(Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (data.GetType())
	{
		case kJointSpherical:
			
			return (new SphericalJoint(0));
		
		case kJointUniversal:
			
			return (new UniversalJoint(0));
		
		case kJointDiscal:
			
			return (new DiscalJoint(0));
		
		case kJointCylindrical:
			
			return (new CylindricalJoint(0));
		
		case kJointRevolute:
			
			return (new RevoluteJoint(0));
		
		case kJointPrismatic:
			
			return (new PrismaticJoint(0));
	}
	
	return (nullptr);
}

void Joint::PackType(Packer& data) const
{
	Node::PackType(data);
	data << jointType;
}

void Joint::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Node::Pack(data, packFlags);
	
	data << TerminatorChunk;
}

void Joint::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Node::Unpack(data, unpackFlags);
	UnpackChunkList<Joint>(data, unpackFlags);
}

bool Joint::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	#if C4LEGACY
	
		switch (chunkHeader->chunkType)
		{
			case '1LNK':
			{
				int32	nodeIndex;
				
				data >> nodeIndex;
				data.AddNodeLink(nodeIndex, &FirstRigidBodyLinkProc, this);
				return (true);
			}
			
			case '2LNK':
			{
				int32	nodeIndex;
				
				data >> nodeIndex;
				data.AddNodeLink(nodeIndex, &SecondRigidBodyLinkProc, this);
				return (true);
			}
		}
	
	#endif
	
	return (false);
}

#if C4LEGACY

	void Joint::FirstRigidBodyLinkProc(Node *node, void *cookie)
	{
		Joint *joint = static_cast<Joint *>(cookie);
		joint->SetFirstConnectedRigidBody(node);
	}
	
	void Joint::SecondRigidBodyLinkProc(Node *node, void *cookie)
	{
		Joint *joint = static_cast<Joint *>(cookie);
		joint->SetSecondConnectedRigidBody(node);
	}

#endif

int32 Joint::GetInternalConnectorCount(void) const
{
	return (2);
}

const char *Joint::GetInternalConnectorKey(int32 index) const
{
	if (index == 0) return (kConnectorKeyBody1);
	else if (index == 1) return (kConnectorKeyBody2);
	return (nullptr);
}

void Joint::SetFirstConnectedRigidBody(Node *node)
{
	if (node)
	{
		Hub *hub = GetHub();
		if (hub)
		{
			Connector *connector = hub->FindOutgoingConnector(kConnectorKeyBody1);
			if (connector)
			{
				connector->SetConnectorTarget(node);
				return;
			}
		}
		
		AddConnector(kConnectorKeyBody1, node);
	}
	else
	{
		RemoveConnector(kConnectorKeyBody1);
	}
}

void Joint::SetSecondConnectedRigidBody(Node *node)
{
	if (node)
	{
		Hub *hub = GetHub();
		if (hub)
		{
			Connector *connector = hub->FindOutgoingConnector(kConnectorKeyBody2);
			if (connector)
			{
				connector->SetConnectorTarget(node);
				return;
			}
		}
		
		AddConnector(kConnectorKeyBody2, node);
	}
	else
	{
		RemoveConnector(kConnectorKeyBody2);
	}
}

void Joint::Preprocess(void)
{
	Node::Preprocess();
	
	if ((!GetManipulator()) && (GetNullBody()))
	{
		JointContact *contact = jointContact;
		delete contact;
		
		const Node *node1 = GetFirstConnectedRigidBody();
		const Node *node2 = GetSecondConnectedRigidBody();
		
		RigidBodyController *rigidBody1 = (node1) ? FindRigidBody(node1, &jointPosition[0], &jointDirection[0]) : nullptr;
		RigidBodyController *rigidBody2 = (node2) ? FindRigidBody(node2, &jointPosition[1], &jointDirection[1]) : nullptr;
		
		if (rigidBody1)
		{
			jointDistance[0] = Magnitude(jointPosition[0] - rigidBody1->GetTargetNode()->GetInverseWorldTransform() * GetWorldPosition());
			
			if (rigidBody2)
			{
				jointDistance[1] = Magnitude(jointPosition[1] - rigidBody2->GetTargetNode()->GetInverseWorldTransform() * GetWorldPosition());
				
				jointContact = new JointContact(this, rigidBody1, rigidBody2);
			}
			else
			{
				jointContact = new StaticJointContact(this, rigidBody1);
			}
		}
		else if (rigidBody2)
		{
			jointPosition[0] = jointPosition[1];
			jointDirection[0] = jointDirection[1];
			jointDistance[0] = Magnitude(jointPosition[0] - rigidBody2->GetTargetNode()->GetInverseWorldTransform() * GetWorldPosition());
			
			jointContact = new StaticJointContact(this, rigidBody2);
		}
	}
}

RigidBodyController *Joint::FindRigidBody(const Node *node, Point3D *position, Vector3D *direction) const
{
	Controller *controller = node->GetController();
	if ((controller) && (controller->GetBaseControllerType() == kControllerRigidBody))
	{
		*position = node->GetInverseWorldTransform() * GetWorldPosition();
		*direction = node->GetInverseWorldTransform() * GetWorldTransform()[2];
		return (static_cast<RigidBodyController *>(controller));
	}
	
	Point3D pos = node->GetNodePosition();
	Vector3D dir = node->GetNodeTransform()[2];
	for (;;)
	{
		node = node->GetSuperNode();
		if (node->GetNodeType() == kNodeZone) break;
		
		controller = node->GetController();
		if ((controller) && (controller->GetBaseControllerType() == kControllerRigidBody))
		{
			*position = pos;
			*direction = dir;
			return (static_cast<RigidBodyController *>(controller));
		}
		
		const Transform4D& transform = node->GetNodeTransform();
		pos = transform * pos;
		dir = transform * dir;
	}
	
	return (nullptr);
}

Body *Joint::GetNullBody(void) const
{
	const PhysicsNode *physicsNode = GetWorld()->GetRootNode()->GetPhysicsNode();
	if (physicsNode)
	{
		PhysicsController *controller = static_cast<PhysicsController *>(physicsNode->GetController());
		if (controller) return (controller->GetNullBody());
	}
	
	return (nullptr);
}


SphericalJoint::SphericalJoint(int) : Joint(kJointSpherical)
{
}

SphericalJoint::SphericalJoint() : Joint(kJointSpherical)
{
	SetNewObject(new SphericalJointObject);
}

SphericalJoint::SphericalJoint(const SphericalJoint& sphericalJoint) : Joint(sphericalJoint)
{
}

SphericalJoint::~SphericalJoint()
{
}

Node *SphericalJoint::Replicate(void) const
{
	return (new SphericalJoint(*this));
}

void SphericalJoint::ApplyConstraints(Contact *contact) const
{
	RigidBodyController *rigidBody1 = static_cast<RigidBodyController *>(contact->GetStartElement());
	RigidBodyController *rigidBody2 = static_cast<RigidBodyController *>(contact->GetFinishElement());
	const Transform4D& worldTransform1 = rigidBody1->GetFinalWorldTransform();
	const Transform4D& worldTransform2 = rigidBody2->GetFinalWorldTransform();
	
	Vector3D cm1 = worldTransform1 * rigidBody1->GetCenterOfMass();
	Vector3D r1 = worldTransform1 * (GetJointPosition(0) - rigidBody1->GetCenterOfMass());
	Vector3D p1 = cm1 + r1;
	
	Vector3D cm2 = worldTransform2 * rigidBody2->GetCenterOfMass();
	Vector3D r2 = worldTransform2 * (GetJointPosition(1) - rigidBody2->GetCenterOfMass());
	Vector3D p2 = cm2 + r2;
	
	Vector3D dp = p2 - p1;
	float d = SquaredMag(dp);
	if (d > K::min_float)
	{
		float f = InverseSqrt(d);
		
		Jacobian distanceJacobian1(dp * -f, (dp % r1) * f);
		Jacobian distanceJacobian2(dp * f, (r2 % dp) * f);
		
		float velocityBoost = (d * f) * kContactStabilizeFactor;
		float distanceImpulse = Contact::CalculateConstraintImpulse(rigidBody1, rigidBody2, distanceJacobian1, distanceJacobian2, velocityBoost);
		distanceImpulse = contact->AccumulateConstraintImpulse(0, distanceImpulse);
		rigidBody1->ApplyVelocityCorrection(distanceJacobian1, distanceImpulse);
		rigidBody2->ApplyVelocityCorrection(distanceJacobian2, distanceImpulse);
	}
}

void SphericalJoint::ApplyStaticContraints(Contact *contact) const
{
	RigidBodyController *rigidBody = static_cast<RigidBodyController *>(contact->GetStartElement());
	const Transform4D& worldTransform = rigidBody->GetFinalWorldTransform();
	
	Vector3D cm = worldTransform * rigidBody->GetCenterOfMass() - GetWorldPosition();
	Vector3D r = worldTransform * (GetJointPosition(0) - rigidBody->GetCenterOfMass());
	Vector3D p = cm + r;
	
	float d = SquaredMag(p);
	if (d > K::min_float)
	{
		float f = InverseSqrt(d);
		
		Jacobian distanceJacobian(p * f, (r % cm) * f);
		float velocityBoost = (d * f - GetJointDistance(0)) * kContactStabilizeFactor;
		float distanceImpulse = Contact::CalculateConstraintImpulse(rigidBody, distanceJacobian, velocityBoost);
		distanceImpulse = contact->AccumulateConstraintImpulse(1, distanceImpulse);
		rigidBody->ApplyVelocityCorrection(distanceJacobian, distanceImpulse);
	}
}


UniversalJoint::UniversalJoint(int) : Joint(kJointUniversal)
{
}

UniversalJoint::UniversalJoint() : Joint(kJointUniversal)
{
	SetNewObject(new UniversalJointObject);
}

UniversalJoint::UniversalJoint(const UniversalJoint& universalJoint) : Joint(universalJoint)
{
}

UniversalJoint::~UniversalJoint()
{
}

Node *UniversalJoint::Replicate(void) const
{
	return (new UniversalJoint(*this));
}

void UniversalJoint::ApplyConstraints(Contact *contact) const
{
	RigidBodyController *rigidBody1 = static_cast<RigidBodyController *>(contact->GetStartElement());
	RigidBodyController *rigidBody2 = static_cast<RigidBodyController *>(contact->GetFinishElement());
	const Transform4D& worldTransform1 = rigidBody1->GetFinalWorldTransform();
	const Transform4D& worldTransform2 = rigidBody2->GetFinalWorldTransform();
	
	Vector3D cm1 = worldTransform1 * rigidBody1->GetCenterOfMass();
	Vector3D r1 = worldTransform1 * (GetJointPosition(0) - rigidBody1->GetCenterOfMass());
	Vector3D p1 = cm1 + r1;
	
	Vector3D cm2 = worldTransform2 * rigidBody2->GetCenterOfMass();
	Vector3D r2 = worldTransform2 * (GetJointPosition(1) - rigidBody2->GetCenterOfMass());
	Vector3D p2 = cm2 + r2;
	
	Vector3D spinJacobian1 = r1 * InverseMag(r1);
	float spinImpulse1 = Contact::CalculateAngularConstraintImpulse(rigidBody1, spinJacobian1);
	spinImpulse1 = contact->AccumulateConstraintImpulse(0, spinImpulse1);
	rigidBody1->ApplyAngularVelocityCorrection(spinJacobian1, spinImpulse1);
	
	Vector3D spinJacobian2 = r2 * InverseMag(r2);
	float spinImpulse2 = Contact::CalculateAngularConstraintImpulse(rigidBody2, spinJacobian2);
	spinImpulse2 = contact->AccumulateConstraintImpulse(1, spinImpulse2);
	rigidBody2->ApplyAngularVelocityCorrection(spinJacobian2, spinImpulse2);
	
	Vector3D dp = p2 - p1;
	float d = SquaredMag(dp);
	if (d > K::min_float)
	{
		float f = InverseSqrt(d);
		
		Jacobian distanceJacobian1(dp * -f, (dp % r1) * f);
		Jacobian distanceJacobian2(dp * f, (r2 % dp) * f);
		
		float velocityBoost = (d * f) * kContactStabilizeFactor;
		float distanceImpulse = Contact::CalculateConstraintImpulse(rigidBody1, rigidBody2, distanceJacobian1, distanceJacobian2, velocityBoost);
		distanceImpulse = contact->AccumulateConstraintImpulse(2, distanceImpulse);
		rigidBody1->ApplyVelocityCorrection(distanceJacobian1, distanceImpulse);
		rigidBody2->ApplyVelocityCorrection(distanceJacobian2, distanceImpulse);
	}
}

void UniversalJoint::ApplyStaticContraints(Contact *contact) const
{
	RigidBodyController *rigidBody = static_cast<RigidBodyController *>(contact->GetStartElement());
	const Transform4D& worldTransform = rigidBody->GetFinalWorldTransform();
	
	Vector3D cm = worldTransform * rigidBody->GetCenterOfMass() - GetWorldPosition();
	Vector3D r = worldTransform * (GetJointPosition(0) - rigidBody->GetCenterOfMass());
	Vector3D p = cm + r;
	
	Vector3D spinJacobian = r * InverseMag(r);
	float spinImpulse = Contact::CalculateAngularConstraintImpulse(rigidBody, spinJacobian);
	spinImpulse = contact->AccumulateConstraintImpulse(0, spinImpulse);
	rigidBody->ApplyAngularVelocityCorrection(spinJacobian, spinImpulse);
	
	float d = SquaredMag(p);
	if (d > K::min_float)
	{
		float f = InverseSqrt(d);
		
		Jacobian distanceJacobian(p * f, (r % cm) * f);
		float velocityBoost = (d * f - GetJointDistance(0)) * kContactStabilizeFactor;
		float distanceImpulse = Contact::CalculateConstraintImpulse(rigidBody, distanceJacobian, velocityBoost);
		distanceImpulse = contact->AccumulateConstraintImpulse(1, distanceImpulse);
		rigidBody->ApplyVelocityCorrection(distanceJacobian, distanceImpulse);
	}
}


DiscalJoint::DiscalJoint(int) : Joint(kJointDiscal)
{
}

DiscalJoint::DiscalJoint() : Joint(kJointDiscal)
{
	SetNewObject(new DiscalJointObject);
}

DiscalJoint::DiscalJoint(const DiscalJoint& discalJoint) : Joint(discalJoint)
{
}

DiscalJoint::~DiscalJoint()
{
}

Node *DiscalJoint::Replicate(void) const
{
	return (new DiscalJoint(*this));
}

void DiscalJoint::ApplyConstraints(Contact *contact) const
{
	RigidBodyController *rigidBody1 = static_cast<RigidBodyController *>(contact->GetStartElement());
	RigidBodyController *rigidBody2 = static_cast<RigidBodyController *>(contact->GetFinishElement());
	const Transform4D& worldTransform1 = rigidBody1->GetFinalWorldTransform();
	const Transform4D& worldTransform2 = rigidBody2->GetFinalWorldTransform();
	
	Vector3D cm1 = worldTransform1 * rigidBody1->GetCenterOfMass();
	Vector3D r1 = worldTransform1 * (GetJointPosition(0) - rigidBody1->GetCenterOfMass());
	Vector3D p1 = cm1 + r1;
	
	Vector3D cm2 = worldTransform2 * rigidBody2->GetCenterOfMass();
	Vector3D r2 = worldTransform2 * (GetJointPosition(1) - rigidBody2->GetCenterOfMass());
	Vector3D p2 = cm2 + r2;
	
	const Vector3D& axisJacobian1 = worldTransform1 * GetJointDirection(0);
	float axisImpulse1 = Contact::CalculateLinearConstraintImpulse(rigidBody1, axisJacobian1);
	axisImpulse1 = contact->AccumulateConstraintImpulse(0, axisImpulse1);
	rigidBody1->ApplyLinearVelocityCorrection(axisJacobian1, axisImpulse1);
	
	const Vector3D& axisJacobian2 = worldTransform2 * GetJointDirection(1);
	float axisImpulse2 = Contact::CalculateLinearConstraintImpulse(rigidBody2, axisJacobian2);
	axisImpulse2 = contact->AccumulateConstraintImpulse(1, axisImpulse2);
	rigidBody2->ApplyLinearVelocityCorrection(axisJacobian2, axisImpulse2);
	
	Vector3D dp = p2 - p1;
	float d = SquaredMag(dp);
	if (d > K::min_float)
	{
		float f = InverseSqrt(d);
		
		Jacobian distanceJacobian1(dp * -f, (dp % r1) * f);
		Jacobian distanceJacobian2(dp * f, (r2 % dp) * f);
		
		float velocityBoost = (d * f) * kContactStabilizeFactor;
		float distanceImpulse = Contact::CalculateConstraintImpulse(rigidBody1, rigidBody2, distanceJacobian1, distanceJacobian2, velocityBoost);
		distanceImpulse = contact->AccumulateConstraintImpulse(2, distanceImpulse);
		rigidBody1->ApplyVelocityCorrection(distanceJacobian1, distanceImpulse);
		rigidBody2->ApplyVelocityCorrection(distanceJacobian2, distanceImpulse);
	}
}

void DiscalJoint::ApplyStaticContraints(Contact *contact) const
{
	RigidBodyController *rigidBody = static_cast<RigidBodyController *>(contact->GetStartElement());
	const Transform4D& worldTransform = rigidBody->GetFinalWorldTransform();
	
	Vector3D cm = worldTransform * rigidBody->GetCenterOfMass() - GetWorldPosition();
	Vector3D r = worldTransform * (GetJointPosition(0) - rigidBody->GetCenterOfMass());
	Vector3D p = cm + r;
	
	const Vector3D& axisJacobian = GetWorldTransform()[2];
	float axisImpulse = Contact::CalculateLinearConstraintImpulse(rigidBody, axisJacobian);
	axisImpulse = contact->AccumulateConstraintImpulse(0, axisImpulse);
	rigidBody->ApplyLinearVelocityCorrection(axisJacobian, axisImpulse);
	
	float d = SquaredMag(p);
	if (d > K::min_float)
	{
		float f = InverseSqrt(d);
		
		Jacobian distanceJacobian(p * f, (r % cm) * f);
		float velocityBoost = (d * f - GetJointDistance(0)) * kContactStabilizeFactor;
		float distanceImpulse = Contact::CalculateConstraintImpulse(rigidBody, distanceJacobian, velocityBoost);
		distanceImpulse = contact->AccumulateConstraintImpulse(1, distanceImpulse);
		rigidBody->ApplyVelocityCorrection(distanceJacobian, distanceImpulse);
	}
}


RevoluteJoint::RevoluteJoint(int) : Joint(kJointRevolute)
{
}

RevoluteJoint::RevoluteJoint() : Joint(kJointRevolute)
{
	SetNewObject(new RevoluteJointObject);
}

RevoluteJoint::RevoluteJoint(const RevoluteJoint& revoluteJoint) : Joint(revoluteJoint)
{
}

RevoluteJoint::~RevoluteJoint()
{
}

Node *RevoluteJoint::Replicate(void) const
{
	return (new RevoluteJoint(*this));
}

void RevoluteJoint::ApplyConstraints(Contact *contact) const
{
	RigidBodyController *rigidBody1 = static_cast<RigidBodyController *>(contact->GetStartElement());
	RigidBodyController *rigidBody2 = static_cast<RigidBodyController *>(contact->GetFinishElement());
	const Transform4D& worldTransform1 = rigidBody1->GetFinalWorldTransform();
	const Transform4D& worldTransform2 = rigidBody2->GetFinalWorldTransform();
	
	Vector3D cm1 = worldTransform1 * rigidBody1->GetCenterOfMass();
	Vector3D r1 = worldTransform1 * (GetJointPosition(0) - rigidBody1->GetCenterOfMass());
	Vector3D p1 = cm1 + r1;
	
	Vector3D cm2 = worldTransform2 * rigidBody2->GetCenterOfMass();
	Vector3D r2 = worldTransform2 * (GetJointPosition(1) - rigidBody2->GetCenterOfMass());
	Vector3D p2 = cm2 + r2;
	
	const Vector3D& axisJacobian1 = worldTransform1 * GetJointDirection(0);
	float axisImpulse1 = Contact::CalculateLinearConstraintImpulse(rigidBody1, axisJacobian1);
	axisImpulse1 = contact->AccumulateConstraintImpulse(0, axisImpulse1);
	rigidBody1->ApplyLinearVelocityCorrection(axisJacobian1, axisImpulse1);
	
	const Vector3D& axisJacobian2 = worldTransform2 * GetJointDirection(1);
	float axisImpulse2 = Contact::CalculateLinearConstraintImpulse(rigidBody2, axisJacobian2);
	axisImpulse2 = contact->AccumulateConstraintImpulse(1, axisImpulse2);
	rigidBody2->ApplyLinearVelocityCorrection(axisJacobian2, axisImpulse2);
	
	Vector3D spinJacobian1 = r1 * InverseMag(r1);
	//float velocityBoost = Acos(axisJacobian1 * axisJacobian2) * kContactStabilizeFactor;
	float spinImpulse1 = Contact::CalculateAngularConstraintImpulse(rigidBody1, spinJacobian1);
	spinImpulse1 = contact->AccumulateConstraintImpulse(2, spinImpulse1);
	rigidBody1->ApplyAngularVelocityCorrection(spinJacobian1, spinImpulse1);
	
	Vector3D spinJacobian2 = r2 * InverseMag(r2);
	float spinImpulse2 = Contact::CalculateAngularConstraintImpulse(rigidBody2, spinJacobian2);
	spinImpulse2 = contact->AccumulateConstraintImpulse(3, spinImpulse2);
	rigidBody2->ApplyAngularVelocityCorrection(spinJacobian2, spinImpulse2);
	
	Vector3D dp = p2 - p1;
	float d = SquaredMag(dp);
	if (d > K::min_float)
	{
		float f = InverseSqrt(d);
		
		Jacobian distanceJacobian1(dp * -f, (dp % r1) * f);
		Jacobian distanceJacobian2(dp * f, (r2 % dp) * f);
		
		float velocityBoost = (d * f) * kContactStabilizeFactor;
		float distanceImpulse = Contact::CalculateConstraintImpulse(rigidBody1, rigidBody2, distanceJacobian1, distanceJacobian2, velocityBoost);
		distanceImpulse = contact->AccumulateConstraintImpulse(4, distanceImpulse);
		rigidBody1->ApplyVelocityCorrection(distanceJacobian1, distanceImpulse);
		rigidBody2->ApplyVelocityCorrection(distanceJacobian2, distanceImpulse);
	}
}

void RevoluteJoint::ApplyStaticContraints(Contact *contact) const
{
	RigidBodyController *rigidBody = static_cast<RigidBodyController *>(contact->GetStartElement());
	const Transform4D& worldTransform = rigidBody->GetFinalWorldTransform();
	
	Vector3D cm = worldTransform * rigidBody->GetCenterOfMass() - GetWorldPosition();
	Vector3D r = worldTransform * (GetJointPosition(0) - rigidBody->GetCenterOfMass());
	Vector3D p = cm + r;
	
	const Vector3D& axisJacobian = GetWorldTransform()[2];
	float axisImpulse = Contact::CalculateLinearConstraintImpulse(rigidBody, axisJacobian);
	axisImpulse = contact->AccumulateConstraintImpulse(0, axisImpulse);
	rigidBody->ApplyLinearVelocityCorrection(axisJacobian, axisImpulse);
	
	Vector3D spinJacobian = r * InverseMag(r);
	float spinImpulse = Contact::CalculateAngularConstraintImpulse(rigidBody, spinJacobian);
	spinImpulse = contact->AccumulateConstraintImpulse(1, spinImpulse);
	rigidBody->ApplyAngularVelocityCorrection(spinJacobian, spinImpulse);
	
	float d = SquaredMag(p);
	if (d > K::min_float)
	{
		float f = InverseSqrt(d);
		
		Jacobian distanceJacobian(p * f, (r % cm) * f);
		float velocityBoost = (d * f - GetJointDistance(0)) * kContactStabilizeFactor;
		float distanceImpulse = Contact::CalculateConstraintImpulse(rigidBody, distanceJacobian, velocityBoost);
		distanceImpulse = contact->AccumulateConstraintImpulse(2, distanceImpulse);
		rigidBody->ApplyVelocityCorrection(distanceJacobian, distanceImpulse);
	}
}


CylindricalJoint::CylindricalJoint(int) : Joint(kJointCylindrical)
{
}

CylindricalJoint::CylindricalJoint() : Joint(kJointCylindrical)
{
	SetNewObject(new CylindricalJointObject);
}

CylindricalJoint::CylindricalJoint(const CylindricalJoint& cylindricalJoint) : Joint(cylindricalJoint)
{
}

CylindricalJoint::~CylindricalJoint()
{
}

Node *CylindricalJoint::Replicate(void) const
{
	return (new CylindricalJoint(*this));
}

void CylindricalJoint::ApplyConstraints(Contact *contact) const
{
}

void CylindricalJoint::ApplyStaticContraints(Contact *contact) const
{
}


PrismaticJoint::PrismaticJoint(int) : Joint(kJointPrismatic)
{
}

PrismaticJoint::PrismaticJoint() : Joint(kJointPrismatic)
{
	SetNewObject(new PrismaticJointObject);
}

PrismaticJoint::PrismaticJoint(const PrismaticJoint& prismaticJoint) : Joint(prismaticJoint)
{
}

PrismaticJoint::~PrismaticJoint()
{
}

Node *PrismaticJoint::Replicate(void) const
{
	return (new PrismaticJoint(*this));
}

void PrismaticJoint::ApplyConstraints(Contact *contact) const
{
}

void PrismaticJoint::ApplyStaticContraints(Contact *contact) const
{
}

// ZYURVUR
