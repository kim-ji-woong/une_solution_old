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


#ifndef C4Physics_h
#define C4Physics_h


//# \component	Physics Manager
//# \prefix		PhysicsMgr/

//# \import		C4Node.h


#include "C4Shapes.h"
#include "C4Contacts.h"
#include "C4Controller.h"

#if C4DIAGNOSTICS

	#include "C4Renderable.h"

#endif


namespace C4
{
	typedef Type	BodyType;
	typedef Type	RigidBodyType;
	
	
	enum
	{
		kControllerRigidBody	= 'BODY',
		kControllerPhysics		= 'PHYS'
	};
	
	
	enum
	{
		kFunctionSetGravity		= 'SGRV'
	};
	
	
	enum
	{
		kBodyNull				= 0,
		kBodyRigid				= 'RIGD'
	};
	
	
	enum
	{
		kRigidBodyGeneric		= 0
	};
	
	
	//# \enum	RigidBodyFlags
	
	enum
	{
		kRigidBodyKeepAwake				= 1 << 0,		//## The rigid body is never put to sleep. For performance reasons, this flag should be set only when absolutely necessary.
		kRigidBodyPartialSleep			= 1 << 1,		//## When the rigid body is put to sleep for the physics simulation, it is not put to sleep as a controller in general.
		kRigidBodyFixedOrientation		= 1 << 2,		//## The rigid body never rotates in the physics simulation, and thus always preserves its original orientation in space.
		kRigidBodyDisabledContact		= 1 << 3,		//## New collision contacts made with the rigid body should be created in the disabled state. This is useful if a collision will always result in the destruction of the rigid body.
		kRigidBodyLocalSimulation		= 1 << 4,		//## The rigid body is only simulated locally on each machine, and the server does not transmit information about the rigid body to the clients.
		kRigidBodyForceFieldInhibit		= 1 << 5		//## The rigid body is not affected by force fields. The global gravity force is still applied.
	};
	
	
	enum
	{
		kRigidBodyAsleep				= 1 << 0
	};
	
	
	//# \enum	RigidBodyStatus
	
	enum RigidBodyStatus
	{
		kRigidBodyUnchanged,		//## No change was made to the rigid body.
		kRigidBodyDestroyed,		//## The rigid body was destroyed.
		kRigidBodyDetached			//## All contacts with the rigid body were broken.
	};
	
	
	enum
	{
		kPhysicsCounterRigidBody,
		kPhysicsCounterBuoyancy,
		kPhysicsCounterGeometryIntersection,
		kPhysicsCounterShapeIntersection,
		kPhysicsCounterCount
	};
	
	
	class PhysicsController;
	class WaterBlock;
	
	#if C4DIAGNOSTICS
	
		class RigidBodyRenderable;
	
	#endif
	
	 
	struct BodyHitData : ShapeHitData
	{ 
		const Shape		*shape; 
	}; 
	
	 
	class Body : public GraphElement<Body, Contact>
	{
		friend class PhysicsController;
		 
		private:
			
			BodyType	bodyType;
		 
		protected:
			
			Body(BodyType type = kBodyNull);
		
		public:
			
			~Body();
			
			BodyType GetBodyType(void) const
			{
				return (bodyType);
			}
	};
	
	
	//# \class	RigidBodyController		Manages a rigid body in a physics simulation.
	//
	//# The $RigidBodyController$ class manages a rigid body in a physics simulation.
	//
	//# \def	class RigidBodyController : public Controller, public ListElement<RigidBodyController>, public Body
	//
	//# \ctor	RigidBodyController();
	//
	//# \desc
	//# The $RigidBodyController$ class manages a rigid body in a physics simulation.
	//
	//# \base		Controller/Controller						A $RigidBodyController$ is a specific type of controller.
	//# \base		Utilities/ListElement<RigidBodyController>	Used internally by the Physics Manager.
	//# \privbase	Body										Used internally by the Physics Manager.
	//
	//# \also	$@PhysicsController@$
	//# \also	$@Shape@$
	//# \also	$@Joint@$
	//# \also	$@Force@$
	//# \also	$@Field@$
	
	
	//# \function	RigidBodyController::GetRigidBodyFlags		Returns the rigid body flags.
	//
	//# \proto	unsigned_int32 GetRigidBodyFlags(void) const;
	//
	//# \desc
	//# The $GetRigidBodyFlags$ function returns the rigid body flags, which can be a combination
	//# (through logical OR) of the following values.
	//
	//# \table	RigidBodyFlags
	//
	//# \also	$@RigidBodyController::SetRigidBodyFlags@$
	
	
	//# \function	RigidBodyController::SetRigidBodyFlags		Sets the rigid body flags.
	//
	//# \proto	void SetRigidBodyFlags(unsigned_int32 flags);
	//
	//# \param	flags	The new rigid body flags. See below for possible values.
	//
	//# \desc
	//# The $SetRigidBodyFlags$ function sets the rigid body flags. The $flags$ parameter can be a combination
	//# (through logical OR) of the following values.
	//
	//# \table	RigidBodyFlags
	//
	//# The initial value of the rigid body flags is 0.
	//
	//# \also	$@RigidBodyController::GetRigidBodyFlags@$
	
	
	//# \function	RigidBodyController::GetGravityMultiplier	Returns the gravity multiplier for a rigid body.
	//
	//# \proto	float GetGravityMultiplier(void) const;
	//
	//# \desc
	//# The $GetGravityMultiplier$ function returns the gravity multiplier for a rigid body.
	//
	//# \also	$@RigidBodyController::SetGravityMultiplier@$
	//# \also	$@PhysicsController::GetGravityAcceleration@$
	//# \also	$@PhysicsController::SetGravityAcceleration@$
	
	
	//# \function	RigidBodyController::SetGravityMultiplier	Sets the gravity multiplier for a rigid body.
	//
	//# \proto	void SetGravityMultiplier(float multiplier);
	//
	//# \param	multiplier	The new gravity multiplier.
	//
	//# \desc
	//# The $SetGravityMultiplier$ function sets the gravity multiplier for a rigid body to the value
	//# specified by the $multiplier$ parameter. The gravity multiplier scales the force exerted on a
	//# rigid body by the global gravity set in the $@PhysicsController@$ object. A multiplier of 1.0 means
	//# that the ordinary gravity force is applied, while values higher or lower than 1.0 mean that
	//# proportionately more or less gravity is applied. If the gravity multiplier is 0.0, then no
	//# gravity is applied to the rigid body at all.
	//#
	//# The initial value of the gravity multiplier is 1.0.
	//
	//# \also	$@RigidBodyController::GetGravityMultiplier@$
	//# \also	$@PhysicsController::GetGravityAcceleration@$
	//# \also	$@PhysicsController::SetGravityAcceleration@$
	
	
	//# \function	RigidBodyController::GetDragMultiplier		Returns the drag multiplier for a rigid body.
	//
	//# \proto	float GetDragMultiplier(void) const;
	//
	//# \desc
	//# The $GetDragMultiplier$ function returns the drag multiplier for a rigid body.
	//
	//# \also	$@RigidBodyController::SetDragMultiplier@$
	//# \also	$@FluidForce@$
	//# \also	$@WindForce@$
	
	
	//# \function	RigidBodyController::SetDragMultiplier		Sets the drag multiplier for a rigid body.
	//
	//# \proto	void SetDragMultiplier(float multiplier);
	//
	//# \param	multiplier	The new drag multiplier.
	//
	//# \desc
	//# The $SetDragMultiplier$ function sets the drag multiplier for a rigid body to the value
	//# specified by the $multiplier$ parameter. The drag multiplier scales the drag force exerted
	//# on a rigid body by various force fields such as the $@FluidForce@$ and $@WindForce@$ classes.
	//# A value of 1.0 means that the ordinary drag force is applied, while values higher or lower than
	//# 1.0 mean that proportionately more or less drag is applied. If the drag multiplier is 0.0, then
	//# no drag is applied to the rigid body at all.
	//#
	//# The initial value of the drag multiplier is 1.0.
	//
	//# \also	$@RigidBodyController::GetDragMultiplier@$
	//# \also	$@FluidForce@$
	//# \also	$@WindForce@$
	
	
	//# \function	RigidBodyController::GetRestitutionCoefficient		Returns the restitution coefficient for a rigid body.
	//
	//# \proto	float GetRestitutionCoefficient(void) const;
	//
	//# \desc
	//# The $GetRestitutionCoefficient$ function returns the restitution coefficient for a rigid body.
	//
	//# \also	$@RigidBodyController::SetRestitutionCoefficient@$
	
	
	//# \function	RigidBodyController::SetRestitutionCoefficient		Sets the restitution coefficient for a rigid body.
	//
	//# \proto	void SetRestitutionCoefficient(float restitution);
	//
	//# \param	restitution		The new restitution coefficient.
	//
	//# \desc
	//# The $SetRestitutionCoefficient$ function sets the restitution coefficient for a rigid body to the
	//# value specified by the $restitution$ parameter. This value determines how much a rigid body bounces
	//# when it collides with another rigid body or static geometry, and it should be in the range [0.0,&nbsp;1.0].
	//# A value of 0.0 means that all of the rigid body's energy is lost in a collision, and a value of 1.0
	//# means that collisions produce a completely elastic response.
	//#
	//# The initial value of the restitution coefficient is 0.0.
	//
	//# \also	$@RigidBodyController::GetRestitutionCoefficient@$
	
	
	//# \function	RigidBodyController::GetFrictionCoefficient		Returns the friction coefficient for a rigid body.
	//
	//# \proto	float GetFrictionCoefficient(void) const;
	//
	//# \desc
	//# The $GetFrictionCoefficient$ function returns the friction coefficient for a rigid body.
	//
	//# \also	$@RigidBodyController::SetFrictionCoefficient@$
	
	
	//# \function	RigidBodyController::SetFrictionCoefficient		Sets the friction coefficient for a rigid body.
	//
	//# \proto	void SetFrictionCoefficient(float friction);
	//
	//# \param	friction		The new friction coefficient.
	//
	//# \desc
	//# The $SetFrictionCoefficient$ function sets the friction coefficient for a rigid body to the
	//# value specified by the $friction$ parameter. This value determines how much frictional force
	//# is exerted on a rigid body when it is in contact with another rigid body or static geometry.
	//#
	//# The initial value of the friction coefficient is 0.01.
	//
	//# \also	$@RigidBodyController::GetFrictionCoefficient@$
	
	
	//# \function	RigidBodyController::GetSleepBoxSize		Returns the sleep box size for a rigid body.
	//
	//# \proto	float GetSleepBoxSize(void) const;
	//
	//# \desc
	//# The $GetSleepBoxSize$ function returns the size of the sleep boxes used to determine when a rigid body
	//# can be put to sleep.
	//
	//# \also	$@RigidBodyController::SetSleepBoxSize@$
	
	
	//# \function	RigidBodyController::SetSleepBoxSize		Sets the sleep box size for a rigid body.
	//
	//# \proto	float SetSleepBoxSize(float size) const;
	//
	//# \param	size	The new size of the sleep box.
	//
	//# \desc
	//# The $SetSleepBoxSize$ function sets the size of the sleep boxes used to determine when a rigid body
	//# can be put to sleep. The initial value is given by the $kRigidBodySleepBoxSize$ constant. Larger values
	//# increase the tendancy for a rigid body to be put to sleep when it experiences only small motions.
	//
	//# \also	$@RigidBodyController::GetSleepBoxSize@$
	
	
	//# \div
	//# \function	RigidBodyController::GetCollisionKind		Returns the collision kind for a rigid body.
	//
	//# \proto	unsigned_int32 GetCollisionKind(void) const;
	//
	//# \desc
	//# The $GetCollisionKind$ function returns the collision kind for a rigid body.
	//#
	//# See the $@RigidBodyController::SetCollisionKind@$ function for an explanation of collision kinds.
	//
	//# \also	$@RigidBodyController::SetCollisionKind@$
	//# \also	$@RigidBodyController::GetCollisionExclusionMask@$
	//# \also	$@RigidBodyController::SetCollisionExclusionMask@$
	//# \also	$@RigidBodyController::ValidRigidBodyCollision@$
	//# \also	$@RigidBodyController::ValidGeometryCollision@$
	
	
	//# \function	RigidBodyController::SetCollisionKind		Sets the collision kind for a rigid body.
	//
	//# \proto	void SetCollisionKind(unsigned_int32 kind);
	//
	//# \param	kind	The new collision kind.
	//
	//# \desc
	//# The $SetCollisionKind$ function sets the collision kind for a rigid body. The collision kind is a 32-bit
	//# value that typically has a single bit set to 1, and the rest set to 0. However, values with more than one bit
	//# set are allowed. The following collision kinds are defined by the engine.
	//
	//# \table	CollisionKind
	//
	//# User-defined collision kinds should always be single bit values greater than or equal to $kCollisionBaseKind$.
	//# New collision kinds would typically be defined by setting the first one equal to $kCollisionBaseKind$, the second
	//# one equal to $kCollisionBaseKind << 1$, the third one equal to $kCollisionBaseKind << 2$, and so on.
	//#
	//# The initial collision kind for a rigid body is $kCollisionRigidBody$.
	//
	//# \also	$@RigidBodyController::GetCollisionKind@$
	//# \also	$@RigidBodyController::GetCollisionExclusionMask@$
	//# \also	$@RigidBodyController::SetCollisionExclusionMask@$
	//# \also	$@RigidBodyController::ValidRigidBodyCollision@$
	//# \also	$@RigidBodyController::ValidGeometryCollision@$
	
	
	//# \function	RigidBodyController::GetCollisionExclusionMask		Returns the collision exclusion mask for a rigid body.
	//
	//# \proto	unsigned_int32 GetCollisionExclusionMask(void) const;
	//
	//# \desc
	//# The $GetCollisionExclusionMask$ function returns the collision exclusion mask for a rigid body.
	//#
	//# See the $@RigidBodyController::SetCollisionExclusionMask@$ function for an explanation of collision exclusion masks.
	//
	//# \also	$@RigidBodyController::SetCollisionExclusionMask@$
	//# \also	$@RigidBodyController::GetCollisionKind@$
	//# \also	$@RigidBodyController::SetCollisionKind@$
	//# \also	$@RigidBodyController::ValidRigidBodyCollision@$
	//# \also	$@RigidBodyController::ValidGeometryCollision@$
	
	
	//# \function	RigidBodyController::SetCollisionExclusionMask		Sets the collision exclusion mask for a rigid body.
	//
	//# \proto	void GetCollisionExclusionMask(unsigned_int32 mask);
	//
	//# \param	mask	The new collision exclusion mask.
	//
	//# \desc
	//# The $SetCollisionExclusionMask$ function sets the collision exclusion mask for a rigid body to the value specified
	//# by the $mask$ parameter. The exclusion mask can be any 32-bit value that is a combination (through logical OR) of
	//# collision kind values. For any bits that are set, the default $@RigidBodyController::ValidRigidBodyCollision@$ function
	//# does not allow collisions with any rigid bodies having the corresponding collision kind.
	//#
	//# The initial collision exclusion mask is 0, meaning that all collisions are allowed.
	//
	//# \also	$@RigidBodyController::GetCollisionExclusionMask@$
	//# \also	$@RigidBodyController::GetCollisionKind@$
	//# \also	$@RigidBodyController::SetCollisionKind@$
	//# \also	$@RigidBodyController::ValidRigidBodyCollision@$
	//# \also	$@RigidBodyController::ValidGeometryCollision@$
	
	
	//# \function	RigidBodyController::GetPhysicsController		Returns the physics controller to which a rigid body belongs.
	//
	//# \proto	PhysicsController *GetPhysicsController(void) const;
	//
	//# \desc
	//# The $GetPhysicsController$ function returns the physics controller to which a rigid body belongs.
	//# Every rigid body in a world belongs to the same global physics controller. If there is no physics
	//# controller in the world, then this function returns $nullptr$.
	//
	//# \also	$@PhysicsController@$
	
	
	//# \function	RigidBodyController::GetBodyVolume		Returns the volume of a rigid body.
	//
	//# \proto	float GetBodyVolume(void) const;
	//
	//# \desc
	//# The $GetBodyVolume$ function returns the total volume occupied by all of the shapes composing a
	//# rigid body in cubic meters (m<sup>3</sup>). If any shapes overlap, then the full volume of each shape is included
	//# in the total volume for the rigid body.
	//
	//# \also	$@RigidBodyController::GetBodyMass@$
	
	
	//# \function	RigidBodyController::GetBodyMass		Returns the mass of a rigid body.
	//
	//# \proto	float GetBodyMass(void) const;
	//
	//# \desc
	//# The $GetBodyMass$ function returns the total mass of the shapes composing a rigid body in metric tons
	//# (i.e., in thousands of kilograms). If any shapes overlap, then the full mass of each shape is included
	//# in the total mass for the rigid body.
	//
	//# \also	$@RigidBodyController::GetBodyVolume@$
	//# \also	$@ShapeObject::GetShapeDensity@$
	//# \also	$@ShapeObject::SetShapeDensity@$
	
	
	//# \function	RigidBodyController::GetCenterOfMass	Returns the center of mass of a rigid body.
	//
	//# \proto	const Point3D& GetCenterOfMass(void) const;
	//
	//# \desc
	//# The $GetCenterOfMass$ function returns the center of mass of a rigid body in the coordinate space
	//# for the node to which the rigid body controller is assigned.
	//
	//# \also	$@RigidBodyController::GetBodyVolume@$
	//# \also	$@RigidBodyController::GetBodyMass@$
	
	
	//# \div
	//# \function	RigidBodyController::GetLinearVelocity		Returns the current linear velocity of a rigid body.
	//
	//# \proto	const Vector3D& GetLinearVelocity(void) const;
	//
	//# \desc
	//# The $GetLinearVelocity$ function returns the current linear velocity for a rigid body in world-space coordinates,
	//# measured in meters per second (m/s).
	//
	//# \also	$@RigidBodyController::SetLinearVelocity@$
	//# \also	$@RigidBodyController::GetAngularVelocity@$
	//# \also	$@RigidBodyController::SetAngularVelocity@$
	//# \also	$@RigidBodyController::GetOriginalLinearVelocity@$
	//# \also	$@RigidBodyController::GetOriginalAngularVelocity@$
	
	
	//# \function	RigidBodyController::SetLinearVelocity		Sets the current linear velocity of a rigid body.
	//
	//# \proto	void SetLinearVelocity(const Vector3D& velocity);
	//
	//# \param	velocity	The new linear velocity, in world-space coordinates.
	//
	//# \desc
	//# The $SetLinearVelocity$ function sets the current linear velocity for a rigid body to that specified by the
	//# $velocity$ parameter. The velocity vector is specified in world-space coordinates, and it is
	//# measured in meters per second (m/s).
	//
	//# \also	$@RigidBodyController::GetLinearVelocity@$
	//# \also	$@RigidBodyController::GetAngularVelocity@$
	//# \also	$@RigidBodyController::SetAngularVelocity@$
	//# \also	$@RigidBodyController::GetOriginalLinearVelocity@$
	//# \also	$@RigidBodyController::GetOriginalAngularVelocity@$
	
	
	//# \function	RigidBodyController::GetAngularVelocity		Returns the current angular velocity of a rigid body.
	//
	//# \proto	const Vector3D& GetAngularVelocity(void) const;
	//
	//# \desc
	//# The $GetAngularVelocity$ function returns the current angular velocity for a rigid body in world-space coordinates,
	//# measured in radians per second (rad/s).
	//
	//# \also	$@RigidBodyController::SetAngularVelocity@$
	//# \also	$@RigidBodyController::GetLinearVelocity@$
	//# \also	$@RigidBodyController::SetLinearVelocity@$
	//# \also	$@RigidBodyController::GetOriginalLinearVelocity@$
	//# \also	$@RigidBodyController::GetOriginalAngularVelocity@$
	
	
	//# \function	RigidBodyController::SetAngularVelocity		Sets the current angular velocity of a rigid body.
	//
	//# \proto	void SetAngularVelocity(const Vector3D& velocity);
	//
	//# \param	velocity	The new angular velocity, in world-space coordinates.
	//
	//# \desc
	//# The $SetAngularVelocity$ function sets the current angular velocity for a rigid body to that specified by the
	//# $velocity$ parameter. The velocity vector is specified in world-space coordinates, and it is
	//# measured in radians per second (rad/s).
	//
	//# \also	$@RigidBodyController::GetAngularVelocity@$
	//# \also	$@RigidBodyController::GetLinearVelocity@$
	//# \also	$@RigidBodyController::SetLinearVelocity@$
	//# \also	$@RigidBodyController::GetOriginalLinearVelocity@$
	//# \also	$@RigidBodyController::GetOriginalAngularVelocity@$
	
	
	//# \function	RigidBodyController::GetOriginalLinearVelocity		Returns the linear velocity that a rigid body had at the beginning of the simulation step.
	//
	//# \proto	const Vector3D& GetOriginalLinearVelocity(void) const;
	//
	//# \desc
	//# The $GetOriginalLinearVelocity$ function returns the linear velocity that a rigid body had at the beginning of the most recent simulation step,
	//# before any contact forces were applied. The velocity is returned in world-space coordinates, and it's measured in meters per second (m/s).
	//
	//# \also	$@RigidBodyController::GetOriginalAngularVelocity@$
	//# \also	$@RigidBodyController::GetLinearVelocity@$
	//# \also	$@RigidBodyController::GetAngularVelocity@$
	
	
	//# \function	RigidBodyController::GetOriginalAngularVelocity		Returns the angular velocity that a rigid body had at the beginning of the simulation step.
	//
	//# \proto	const Vector3D& GetOriginalAngularVelocity(void) const;
	//
	//# \desc
	//# The $GetOriginalAngularVelocity$ function returns the angular velocity that a rigid body had at the beginning of the most recent simulation step,
	//# before any contact forces were applied. The velocity is returned in world-space coordinates, and it's measured in radians per second (rad/s).
	//
	//# \also	$@RigidBodyController::GetOriginalLinearVelocity@$
	//# \also	$@RigidBodyController::GetLinearVelocity@$
	//# \also	$@RigidBodyController::GetAngularVelocity@$
	
	
	//# \div
	//# \function	RigidBodyController::GetExternalForce		Returns the external force acting on a rigid body.
	//
	//# \proto	const Vector3D& GetExternalForce(void) const;
	//
	//# \desc
	//# The $GetExternalForce$ function returns the external force, in world-space coordinates, acting on a rigid body's center of mass.
	//# The force is measured in kilonewtons (kN).
	//
	//# \also	$@RigidBodyController::SetExternalForce@$
	//# \also	$@RigidBodyController::GetExternalTorque@$
	//# \also	$@RigidBodyController::SetExternalTorque@$
	//# \also	$@RigidBodyController::ApplyImpulse@$
	
	
	//# \function	RigidBodyController::SetExternalForce		Sets the external force acting on a rigid body.
	//
	//# \proto	void SetExternalForce(const Vector2D& force);
	//# \proto	void SetExternalForce(const Vector3D& force);
	//
	//# \param	force		The new external force, in world-space coordinates.
	//
	//# \desc
	//# The $SetExternalForce$ function sets the external force, in world-space coordinates, acting on a rigid body's center of mass
	//# to the force specified by the $force$ parameter. The force is measured in kilonewtons (kN). If a 2D vector is specified, then
	//# the <i>z</i> coordinate of the force is zero.
	//#
	//# Once an external force is established, it is applied continuously until it is removed. The $@RigidBodyController::ApplyImpulse@$
	//# function can be used to exert a one-time instantaneous force on a rigid body.
	//# 
	//# The initial value of the external force is (0,0,0).
	//
	//# \also	$@RigidBodyController::SetExternalForce@$
	//# \also	$@RigidBodyController::GetExternalTorque@$
	//# \also	$@RigidBodyController::SetExternalTorque@$
	//# \also	$@RigidBodyController::ApplyImpulse@$
	
	
	//# \function	RigidBodyController::GetExternalTorque		Returns the external torque acting on a rigid body.
	//
	//# \proto	const Vector3D& GetExternalTorque(void) const;
	//
	//# \desc
	//# The $GetExternalTorque$ function returns the external torque, in world-space coordinates, acting on a rigid body about its center of mass.
	//# The torque is measured in kilonewtons times meters (kN&middot;m).
	//
	//# \also	$@RigidBodyController::SetExternalTorque@$
	//# \also	$@RigidBodyController::GetExternalForce@$
	//# \also	$@RigidBodyController::SetExternalForce@$
	//# \also	$@RigidBodyController::ApplyImpulse@$
	
	
	//# \function	RigidBodyController::SetExternalTorque		Sets the external torque acting on a rigid body.
	//
	//# \proto	void SetExternalTorque(const Vector3D& torque);
	//
	//# \param	torque		The new external torque, in world-space coordinates.
	//
	//# \desc
	//# The $SetExternalTorque$ function sets the external torque, in world-space coordinates, acting on a rigid body about its center of mass
	//# to the torque specified by the $torque$ parameter. The torque is measured in kilonewtons times meters (kN&middot;m).
	//#
	//# Once an external torque is established, it is applied continuously until it is removed. The $@RigidBodyController::ApplyImpulse@$
	//# function can be used to exert a one-time instantaneous torque on a rigid body.
	//# 
	//# The initial value of the external torque is (0,0,0).
	//
	//# \also	$@RigidBodyController::GetExternalTorque@$
	//# \also	$@RigidBodyController::GetExternalForce@$
	//# \also	$@RigidBodyController::SetExternalForce@$
	//# \also	$@RigidBodyController::ApplyImpulse@$
	
	
	//# \function	RigidBodyController::GetExternalLinearResistance		Returns the external resistive force acting on the linear velocity of a rigid body.
	//
	//# \proto	const Vector3D& GetExternalLinearResistance(void) const;
	//
	//# \desc
	//# The $GetExternalLinearResistance$ function returns the external resistive force, in world-space coordinates, acting on the linear
	//# velocity of a rigid body. The resistive force is measured in kilonewtons per meter-per-second (kN&middot;s&middot;m<sup>&minus;1</sup>).
	//
	//# \also	$@RigidBodyController::SetExternalLinearResistance@$
	//# \also	$@RigidBodyController::GetExternalForce@$
	//# \also	$@RigidBodyController::SetExternalForce@$
	
	
	//# \function	RigidBodyController::SetExternalLinearResistance		Sets the external resistive force acting on the linear velocity of a rigid body.
	//
	//# \proto	void SetExternalLinearResistance(const Vector2D& resistance);
	//# \proto	void SetExternalLinearResistance(const Vector3D& resistance);
	//
	//# \param	resistance		The new external resistive force, in world-space coordinates.
	//
	//# \desc
	//# The $SetExternalLinearResistance$ function sets the external resistive force, in world-space coordinates, acting on the linear
	//# velocity of a rigid body to the resistance specified by the $resistance$ parameter. The resistive force is measured in kilonewtons
	//# per meter-per-second (kN&middot;s&middot;m<sup>&minus;1</sup>). If a 2D vector is specified, then the <i>z</i> coordinate of the resistance is zero.
	//#
	//# Once an external resistance is established, it is applied continuously until it is removed. The force due to the resistance is calculated
	//# by multiplying the current linear velocity by the external linear resistance componentwise. This force is then subtracted from the total
	//# force applied to a rigid body.
	//# 
	//# The initial value of the external resistive force is (0,0,0).
	//
	//# \also	$@RigidBodyController::GetExternalLinearResistance@$
	//# \also	$@RigidBodyController::GetExternalForce@$
	//# \also	$@RigidBodyController::SetExternalForce@$
	
	
	//# \function	RigidBodyController::ApplyImpulse		Applies an impulse to a rigid body.
	//
	//# \proto	void ApplyImpulse(const Vector3D& impulse);
	//# \proto	void ApplyImpulse(const Vector3D& impulse, const Point3D& position);
	//
	//# \param	impulse		The impulse to apply, in node-space coordinates.
	//# \param	position	The node-space position to which the impulse is applied.
	//
	//# \desc
	//# The $ApplyImpulse$ function applies a one-time instantaneous impulse to a rigid body. The impulse is specified by the $impulse$
	//# parameter and is measured in kilonewtons times seconds (kN&middot;s). This function causes a force to be applied to the rigid body's
	//# center of mass during the next simulation step. A torque is also applied to the rigid body when the difference between the $position$
	//# parameter and the rigid body's center of mass is not parallel to the direction of the impulse.
	//#
	//# The $impulse$ and $position$ parameters are specified in the node-space coordinates for the target node of the rigid body controller.
	//# If the $position$ parameter is omitted, then the impulse is applied to the rigid body's center of mass.
	//#
	//# If the $ApplyImpulse$ function is called multiple times for the same rigid body between simulation steps, then the forces and
	//# torques that get applied are accumulated.
	//
	//# \also	$@RigidBodyController::GetExternalForce@$
	//# \also	$@RigidBodyController::SetExternalForce@$
	//# \also	$@RigidBodyController::GetExternalTorque@$
	//# \also	$@RigidBodyController::SetExternalTorque@$
	
	
	//# \function	RigidBodyController::SetRigidBodyTransform		Sets the node transform for a rigid body.
	//
	//# \proto	void SetRigidBodyTransform(const Transform4D& transform);
	//
	//# \param	transform		The new node transform for the rigid body.
	//
	//# \desc
	//# The $SetRigidBodyTransform$ function sets the node transform for a rigid body. This function should be called
	//# instead of the $@WorldMgr/Node::SetNodeTransform@$ function to change the node transform for any node under the control
	//# of a $RigidBodyController$.
	//#
	//# The upper-left 3&nbsp;&times;&nbsp;3 portion of the matrix specified by the $transform$ parameter must be right-handed and orthogonal.
	//
	//# \also	$@RigidBodyController::SetRigidBodyPosition@$
	//# \also	$@RigidBodyController::SetRigidBodyMatrix3D@$
	
	
	//# \function	RigidBodyController::SetRigidBodyMatrix3D		Sets the node rotation matrix for a rigid body.
	//
	//# \proto	void SetRigidBodyMatrix3D(const Matrix3D& matrix);
	//
	//# \param	matrix		The new node rotation matrix for the rigid body.
	//
	//# \desc
	//# The $SetRigidBodyMatrix3D$ function sets the node rotation matrix for a rigid body. This function should be called
	//# instead of the $@WorldMgr/Node::SetNodeMatrix3D@$ function to change the node rotation matrix for any node under the control
	//# of a $RigidBodyController$.
	//#
	//# The matrix specified by the $matrix$ parameter must be right-handed and orthogonal.
	//
	//# \also	$@RigidBodyController::SetRigidBodyTransform@$
	//# \also	$@RigidBodyController::SetRigidBodyPosition@$
	
	
	//# \function	RigidBodyController::SetRigidBodyPosition		Sets the node position for a rigid body.
	//
	//# \proto	void SetRigidBodyPosition(const Point3D& position);
	//
	//# \param	position		The new node position for the rigid body.
	//
	//# \desc
	//# The $SetRigidBodyPosition$ function sets the node position for a rigid body. This function should be called
	//# instead of the $@WorldMgr/Node::SetNodePosition@$ function to change the node position for any node under the control
	//# of a $RigidBodyController$.
	//
	//# \also	$@RigidBodyController::SetRigidBodyTransform@$
	//# \also	$@RigidBodyController::SetRigidBodyMatrix3D@$
	
	
	//# \div
	//# \function	RigidBodyController::CalculateSubmergedVolume		Calculates the submerged volume of a rigid body.
	//
	//# \proto	float CalculateSubmergedVolume(const Antivector4D& plane, Point3D *submergedCentroid) const;
	//
	//# \param	plane				The planar boundary, in node-space coordinates.
	//# \param	submergedCentroid	A pointer to the location where the submerged centroid is returned.
	//
	//# \desc
	//# The $CalculateSubmergedVolume$ function determines what portion of a rigid body lies on the positive side of
	//# the plane specified by the $plane$ parameter. For each shape composing the rigid body, the exact volume of the
	//# part lying on the positive side of the plane is calculated, and the results are summed to produce the return
	//# value for this function. The geometric centroid of the submerged volume of each shape is also calculated, and
	//# the volume-weighted sum of these centroids is returned through the $submergedCentroid$ parameter.
	
	
	//# \div
	//# \function	RigidBodyController::ValidRigidBodyCollision		Returns a boolean value indicating whether a collision with another rigid body would be valid.
	//
	//# \proto	virtual bool ValidRigidBodyCollision(const RigidBodyController *body) const;
	//
	//# \param	body	A pointer to another rigid body with which a collision might occur.
	//
	//# \desc
	//# The $ValidRigidBodyCollision$ function returns a boolean value indicating whether a collision between the rigid body for
	//# which it is called and the rigid body specified by the $body$ parameter should be considered valid. This function
	//# can be overridden in a subclass of $RigidBodyController$ to implement arbitrary collision masking. The default
	//# implementation in the base class checks whether the collision kind of the rigid body for which $ValidRigidBodyCollision$
	//# is called is excluded by the collision exclusion mask for the rigid body specified by the $body$ parameter.
	//# The function returns $true$ if a collision between the two bodies is allowed, and it returns $false$ if such a
	//# collision should never occur.
	//#
	//# When a collision might occur between two rigid bodies, the $ValidRigidBodyCollision$ function is called twice, one time
	//# for each rigid body with the other rigid body passed as the $body$ parameter.
	//
	//# \also	$@RigidBodyController::ValidGeometryCollision@$
	//# \also	$@RigidBodyController::GetCollisionKind@$
	//# \also	$@RigidBodyController::SetCollisionKind@$
	//# \also	$@RigidBodyController::GetCollisionExclusionMask@$
	//# \also	$@RigidBodyController::SetCollisionExclusionMask@$
	
	
	//# \function	RigidBodyController::ValidGeometryCollision		Returns a boolean value indicating whether a collision with a geometry node would be valid.
	//
	//# \proto	virtual bool ValidGeometryCollision(const Geometry *geometry) const;
	//
	//# \param	geometry	A pointer to a geometry node with which a collision might occur.
	//
	//# \desc
	//# The $ValidGeometryCollision$ function returns a boolean value indicating whether a collision between the rigid body for
	//# which it is called and the geometry node specified by the $geometry$ parameter should be considered valid. This function
	//# can be overridden in a subclass of $RigidBodyController$ to implement arbitrary collision masking. The default
	//# implementation in the base class checks whether the collision kind of the rigid body for which $ValidRigidBodyCollision$
	//# is called is excluded by the collision exclusion mask for the geometry object attached to the node specified by the
	//# $geometry$ parameter. The function returns $true$ if a collision with the geometry node is allowed, and it returns $false$
	//# if such a collision should never occur.
	//
	//# \also	$@RigidBodyController::ValidRigidBodyCollision@$
	//# \also	$@RigidBodyController::GetCollisionKind@$
	//# \also	$@RigidBodyController::SetCollisionKind@$
	//# \also	$@RigidBodyController::GetCollisionExclusionMask@$
	//# \also	$@RigidBodyController::SetCollisionExclusionMask@$
	//# \also	$@WorldMgr/GeometryObject::GetCollisionExclusionMask@$
	//# \also	$@WorldMgr/GeometryObject::SetCollisionExclusionMask@$
	
	
	//# \function	RigidBodyController::HandleNewRigidBodyContact		Called when a new contact is made with another rigid body.
	//
	//# \proto	virtual RigidBodyStatus HandleNewRigidBodyContact(const RigidBodyContact *contact, RigidBodyController *contactBody);
	//
	//# \param	contact			The new contact.
	//# \param	contactBody		The rigid body with which contact was made.
	//
	//# \desc
	//# The $HandleNewRigidBodyContact$ function is called by the Physics Manager when a rigid body makes a new contact with
	//# another rigid body. This function can be overridden in a subclass of $RigidBodyController$ in order to carry out a
	//# specialized response to a collision.
	//#
	//# The $contact$ parameter specifies the newly created $@RigidBodyContact@$ object, which is an edge in the contact graph maintained
	//# by the Physics Manager. The rigid body for which the $HandleNewRigidBodyContact$ function is called can be either the start node
	//# or finish node for this edge. The $contactBody$ parameter specifies the other rigid body involved in the new contact, which is
	//# always on the opposite end of the contact edge relative to the rigid body for which the $HandleNewRigidBodyContact$ function is called.
	//#
	//# When a collision occurs between two rigid bodies, the $HandleNewRigidBodyContact$ function is called once for each rigid body.
	//# The order of the two calls is not defined, so any overridden function should not depend on the $HandleNewRigidBodyContact$ function
	//# being called for the start node of the $contact$ parameter before the finish node or vice-versa.
	//#
	//# An overridden $HandleNewRigidBodyContact$ function can call the $@RigidBodyContact::GetWorldContactPosition@$ function to obtain
	//# the world-space position and normal corresponding to one of the rigid bodies involving in the contact.
	//#
	//# The $HandleNewRigidBodyContact$ function should return one of the following values.
	//
	//# \table RigidBodyStatus
	//
	//# The default implementation of the $HandleNewRigidBodyContact$ function calls the $@WorldMgr/World::HandleNewRigidBodyContact@$ function.
	//
	//# \also	$@RigidBodyController::HandleNewGeometryContact@$
	//# \also	$@WorldMgr/World::HandleNewRigidBodyContact@$
	//# \also	$@RigidBodyContact@$
	
	
	//# \function	RigidBodyController::HandleNewGeometryContact		Called when a new contact is made with a geometry node.
	//
	//# \proto	virtual RigidBodyStatus HandleNewGeometryContact(const GeometryContact *contact);
	//
	//# \param	contact			The new contact.
	//
	//# \desc
	//# The $HandleNewGeometryContact$ function is called by the Physics Manager when a rigid body makes a new contact with
	//# a geometry node. This function can be overridden in a subclass of $RigidBodyController$ in order to carry out a
	//# specialized response to a collision.
	//#
	//# The $contact$ parameter specifies the newly created $@GeometryContact@$ object, which is an edge in the contact graph maintained
	//# by the Physics Manager. The rigid body for which the $HandleNewGeometryContact$ function is called is always the start node of this
	//# edge, and a special null body is the finish node.
	//#
	//# The $HandleNewGeometryContact$ function should return one of the following values.
	//
	//# \table RigidBodyStatus
	//
	//# The default implementation of the $HandleNewGeometryContact$ function calls the $@WorldMgr/World::HandleNewGeometryContact@$ function.
	//
	//# \also	$@RigidBodyController::HandleNewRigidBodyContact@$
	//# \also	$@WorldMgr/World::HandleNewGeometryContact@$
	//# \also	$@GeometryContact@$
	
	
	class RigidBodyController : public Controller, public ListElement<RigidBodyController>, public Body, public SnapshotSender
	{
		friend class PhysicsController;
		
		private:
			
			RigidBodyType			rigidBodyType;
			unsigned_int32			rigidBodyFlags;
			unsigned_int32			rigidBodyState;
			
			float					gravityMultiplier;
			float					dragMultiplier;
			
			float					restitutionCoefficient;
			float					frictionCoefficient;
			float					rollingResistance;
			
			unsigned_int32			collisionKind;
			unsigned_int32			collisionExclusionMask;
			
			PhysicsController		*physicsController;
			const WaterBlock		*submergedWaterBlock;
			
			List<Shape>				shapeList;
			List<Shape>				internalShapeList;
			
			float					bodyVolume;
			float					bodyMass;
			float					inverseBodyMass;
			Point3D					centerOfMass;
			InertiaTensor			inertiaTensor;
			
			float					boundingRadius;
			Box3D					boundingBox;
			
			Vector3D				linearVelocity;
			Vector3D				angularVelocity;
			
			Vector3D				originalLinearVelocity;
			Vector3D				originalAngularVelocity;
			Vector3D				initialLinearVelocity;
			Vector3D				initialAngularVelocity;
			Vector3D				linearCorrection;
			Vector3D				angularCorrection;
			float					maxLinearCorrection;
			float					maxAngularCorrection;
			
			Transform4D				initialNodeTransform;
			Transform4D				finalNodeTransform;
			Transform4D				initialWorldTransform;
			Transform4D				finalWorldTransform;
			Transform4D				worldMoveTransform;
			
			Box3D					bodyCollisionBox;
			
			Point3D					worldCenterOfMass;
			InertiaTensor			worldInverseInertiaTensor;
			
			Point3D					moveCenterOfMass;
			Vector3D				moveDisplacement;
			Vector3D				moveRotationAxis;
			float					moveRotationAngle;
			
			Vector3D				externalForce;
			Vector3D				externalTorque;
			Vector3D				externalLinearResistance;
			float					externalAngularResistance;
			
			Vector3D				appliedForce;
			Vector3D				appliedTorque;
			
			Vector3D				impulseForce;
			Vector3D				impulseTorque;
			
			Vector3D				networkDelta[2];
			float					networkDecay[2];
			unsigned_int32			networkParity;
			
			int32					sleepStepCount;
			int32					maxSleepStepCount;
			
			float					sleepBoxSize;
			Box3D					centerSleepBox;
			Box3D					axisSleepBox[2];
			
			#if C4DIAGNOSTICS
			
				Link<RigidBodyRenderable>	rigidBodyRenderable;
			
			#endif
			
			C4API Controller *Replicate(void) const override;
			
			static void WaterBlockLinkProc(Node *node, void *cookie);
			
			void RecursiveWake(bool applyForces = false);
			void RecursiveKeepAwake(void);
			
			RigidBodyContact *FindOutgoingBodyContact(const RigidBodyController *rigidBody, unsigned_int32 startSignature, unsigned_int32 finishSignature) const;
			RigidBodyContact *FindIncomingBodyContact(const RigidBodyController *rigidBody, unsigned_int32 startSignature, unsigned_int32 finishSignature) const;
			bool FindGeometryContact(const Geometry *geometry, unsigned_int32 signature, GeometryContact **matchingContact) const;
			
			void AdjustDisplacement(float t);
			
			static void GeometryCollisionJob(Job *job, void *cookie);
			static void FinalizeExistingStaticGeometrySingleContact(Job *job, void *cookie);
			static void FinalizeNewStaticGeometrySingleContact(Job *job, void *cookie);
			static void FinalizeExistingStaticGeometryMultipleContact(Job *job, void *cookie);
			static void FinalizeNewStaticGeometryMultipleContact(Job *job, void *cookie);
			static void FinalizeNewDynamicGeometrySingleContact(Job *job, void *cookie);
			static void FinalizeNewDynamicGeometryMultipleContact(Job *job, void *cookie);
			
			void DetectGeometryCollision(Geometry *geometry, const Point3D& p1, const Point3D& p2);
			void DetectNodeCollision(Node *node, List<Geometry> *geometryList, const Point3D& p1, const Point3D& p2);
			void DetectCellCollision(Site *cell, List<Geometry> *geometryList, const Point3D& p1, const Point3D& p2);
			void DetectZoneCollision(Zone *zone, List<Geometry> *geometryList, const Point3D& p1, const Point3D& p2);
			void DetectWorldCollisions(void);
			
			void ApplyCellForceFields(Site *cell, const Box3D& box, unsigned_int32 fieldStamp);
			void CalculateAppliedForces(const Vector3D& gravity);
			
			void Integrate(void);
			void Preconstrain(void);
			void Constrain(void);
			void Finalize(void);
		
		protected:
			
			C4API RigidBodyController(ControllerType type);
			C4API RigidBodyController(const RigidBodyController& rigidBodyController);
			
			void SetInverseBodyMass(float inverseMass)
			{
				inverseBodyMass = inverseMass;
			}
		
		public:
			
			enum
			{
				kRigidBodyMessageSnapshot,
				kRigidBodyMessageWake,
				kRigidBodyMessageSleep,
				kRigidBodyMessageBaseCount
			};
			
			C4API RigidBodyController();
			C4API ~RigidBodyController();
			
			using ListElement<RigidBodyController>::Previous;
			using ListElement<RigidBodyController>::Next;
			
			RigidBodyType GetRigidBodyType(void) const
			{
				return (rigidBodyType);
			}
			
			void SetRigidBodyType(RigidBodyType type)
			{
				rigidBodyType = type;
			}
			
			unsigned_int32 GetRigidBodyFlags(void) const
			{
				return (rigidBodyFlags);
			}
			
			void SetRigidBodyFlags(unsigned_int32 flags)
			{
				rigidBodyFlags = flags;
			}
			
			bool RigidBodyAsleep(void) const
			{
				return ((rigidBodyState & kRigidBodyAsleep) != 0);
			}
			
			float GetGravityMultiplier(void) const
			{
				return (gravityMultiplier);
			}
			
			void SetGravityMultiplier(float multiplier)
			{
				gravityMultiplier = multiplier;
			}
			
			float GetDragMultiplier(void) const
			{
				return (dragMultiplier);
			}
			
			void SetDragMultiplier(float multiplier)
			{
				dragMultiplier = multiplier;
			}
			
			float GetRestitutionCoefficient(void) const
			{
				return (restitutionCoefficient);
			}
			
			void SetRestitutionCoefficient(float restitution)
			{
				restitutionCoefficient = restitution;
			}
			
			float GetFrictionCoefficient(void) const
			{
				return (frictionCoefficient);
			}
			
			void SetFrictionCoefficient(float friction)
			{
				frictionCoefficient = friction;
			}
			
			float GetRollingResistance(void) const
			{
				return (rollingResistance);
			}
			
			void SetRollingResistance(float resistance)
			{
				rollingResistance = resistance;
			}
			
			unsigned_int32 GetCollisionKind(void) const
			{
				return (collisionKind);
			}
			
			void SetCollisionKind(unsigned_int32 kind)
			{
				collisionKind = kind;
			}
			
			unsigned_int32 GetCollisionExclusionMask(void) const
			{
				return (collisionExclusionMask);
			}
			
			void SetCollisionExclusionMask(unsigned_int32 mask)
			{
				collisionExclusionMask = mask;
			}
			
			PhysicsController *GetPhysicsController(void) const
			{
				return (physicsController);
			}
			
			const WaterBlock *GetSubmergedWaterBlock(void) const
			{
				return (submergedWaterBlock);
			}
			
			void SetSubmergedWaterBlock(const WaterBlock *waterBlock)
			{
				submergedWaterBlock = waterBlock;
			}
			
			Shape *GetFirstShape(void) const
			{
				return (shapeList.First());
			}
			
			float GetBodyVolume(void) const
			{
				return (bodyVolume);
			}
			
			float GetBodyMass(void) const
			{
				return (bodyMass);
			}
			
			float GetInverseBodyMass(void) const
			{
				return (inverseBodyMass);
			}
			
			const Point3D& GetCenterOfMass(void) const
			{
				return (centerOfMass);
			}
			
			float GetBoundingRadius(void) const
			{
				return (boundingRadius);
			}
			
			const Vector3D& GetLinearVelocity(void) const
			{
				return (linearVelocity);
			}
			
			void SetLinearVelocity(const Vector3D& velocity)
			{
				linearVelocity = velocity;
			}
			
			const Vector3D& GetAngularVelocity(void) const
			{
				return (angularVelocity);
			}
			
			void SetAngularVelocity(const Vector3D& velocity)
			{
				angularVelocity = velocity;
			}
			
			const Vector3D& GetOriginalLinearVelocity(void) const
			{
				return (originalLinearVelocity);
			}
			
			const Vector3D& GetOriginalAngularVelocity(void) const
			{
				return (originalAngularVelocity);
			}
			
			const Transform4D& GetFinalNodeTransform(void) const
			{
				return (finalNodeTransform);
			}
			
			const Point3D& GetFinalNodePosition(void) const
			{
				return (finalNodeTransform.GetTranslation());
			}
			
			const Transform4D& GetFinalWorldTransform(void) const
			{
				return (finalWorldTransform);
			}
			
			const Point3D& GetFinalWorldPosition(void) const
			{
				return (finalWorldTransform.GetTranslation());
			}
			
			const Point3D& GetWorldCenterOfMass(void) const
			{
				return (worldCenterOfMass);
			}
			
			const InertiaTensor& GetWorldInverseInertiaTensor(void) const
			{
				return (worldInverseInertiaTensor);
			}
			
			const Vector3D& GetExternalForce(void) const
			{
				return (externalForce);
			}
			
			void SetExternalForce(const Vector2D& force)
			{
				externalForce = force;
			}
			
			void SetExternalForce(const Vector3D& force)
			{
				externalForce = force;
			}
			
			const Vector3D& GetExternalTorque(void) const
			{
				return (externalTorque);
			}
			
			void SetExternalTorque(const Vector3D& torque)
			{
				externalTorque = torque;
			}
			
			const Vector3D& GetExternalLinearResistance(void) const
			{
				return (externalLinearResistance);
			}
			
			void SetExternalLinearResistance(const Vector2D& resistance)
			{
				externalLinearResistance = resistance;
			}
			
			void SetExternalLinearResistance(const Vector3D& resistance)
			{
				externalLinearResistance = resistance;
			}
			
			float GetExternalAngularResistance(void) const
			{
				return (externalAngularResistance);
			}
			
			void SetExternalAngularResistance(float resistance)
			{
				externalAngularResistance = resistance;
			}
			
			int32 GetMaxSleepStepCount(void) const
			{
				return (maxSleepStepCount);
			}
			
			void SetMaxSleepStepCount(int32 count)
			{
				maxSleepStepCount = count;
			}
			
			float GetSleepBoxSize(void) const
			{
				return (sleepBoxSize);
			}
			
			void SetSleepBoxSize(float size)
			{
				sleepBoxSize = size;
			}
			
			C4API void Pack(Packer& data, unsigned_int32 packFlags) const;
			C4API void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			C4API int32 GetSettingCount(void) const;
			C4API Setting *GetSetting(int32 index) const;
			C4API void SetSetting(const Setting *setting);
			
			C4API void Preprocess(void);
			C4API void Neutralize(void);
			C4API void ChangeZones(Zone *zone, const Transform4D& transform);
			
			C4API void Wake(void);
			C4API void Sleep(void);
			
			C4API ControllerMessage *ConstructMessage(ControllerMessageType type) const;
			C4API void ReceiveMessage(const ControllerMessage *message);
			C4API void SendInitialStateMessages(Player *player) const;
			C4API void SendSnapshot(void);
			
			void ApplyVelocityPreconstraint(const Jacobian& jacobian, float impulse);
			void ApplyVelocityCorrection(const Jacobian& jacobian, float impulse);
			void ApplyLinearVelocityCorrection(const Vector3D& jacobian, float impulse);
			void ApplyAngularVelocityCorrection(const Vector3D& jacobian, float impulse);
			
			void RemoveLaterContacts(float param);
			
			C4API void SetRigidBodyTransform(const Transform4D& transform);
			C4API void SetRigidBodyMatrix3D(const Matrix3D& matrix);
			C4API void SetRigidBodyPosition(const Point3D& position);
			
			C4API void ApplyImpulse(const Vector3D& impulse);
			C4API void ApplyImpulse(const Vector3D& impulse, const Point3D& position);
			
			C4API bool DetectSegmentIntersection(const Point3D& p1, const Point3D& p2, float radius, BodyHitData *bodyHitData) const;
			C4API float CalculateSubmergedVolume(const Antivector4D& plane, Point3D *submergedCentroid) const;
			
			C4API virtual bool ValidRigidBodyCollision(const RigidBodyController *body) const;
			C4API virtual bool ValidGeometryCollision(const Geometry *geometry) const;
			
			C4API virtual RigidBodyStatus HandleNewRigidBodyContact(const RigidBodyContact *contact, RigidBodyController *contactBody);
			C4API virtual RigidBodyStatus HandleNewGeometryContact(const GeometryContact *contact);
			C4API virtual void HandleWaterSubmergence(void);
	};
	
	
	class RigidBodySnapshotMessage : public ControllerMessage
	{
		friend class RigidBodyController;
		
		private:
			
			Point3D			rigidBodyPosition;
			Quaternion		rigidBodyRotation;
			Vector3D		rigidBodyLinearVelocity;
			Vector3D		rigidBodyAngularVelocity;
			
			RigidBodySnapshotMessage(int32 controllerIndex);
		
		public:
			
			RigidBodySnapshotMessage(int32 controllerIndex, const Point3D& position, const Quaternion& rotation, const Vector3D& linearVelocity, const Vector3D& angularVelocity);
			~RigidBodySnapshotMessage();
			
			const Point3D& GetRigidBodyPosition(void) const
			{
				return (rigidBodyPosition);
			}
			
			const Quaternion& GetRigidBodyRotation(void) const
			{
				return (rigidBodyRotation);
			}
			
			const Vector3D& GetRigidBodyLinearVelocity(void) const
			{
				return (rigidBodyLinearVelocity);
			}
			
			const Vector3D& GetRigidBodyAngularVelocity(void) const
			{
				return (rigidBodyAngularVelocity);
			}
			
			void Compress(Compressor& data) const;
			bool Decompress(Decompressor& data);
	};
	
	
	class RigidBodySleepMessage : public ControllerMessage
	{
		friend class RigidBodyController;
		
		private:
			
			Point3D			rigidBodyPosition;
			Quaternion		rigidBodyRotation;
			
			RigidBodySleepMessage(int32 controllerIndex);
		
		public:
			
			RigidBodySleepMessage(int32 controllerIndex, const Point3D& position, const Quaternion& rotation);
			~RigidBodySleepMessage();
			
			const Point3D& GetRigidBodyPosition(void) const
			{
				return (rigidBodyPosition);
			}
			
			const Quaternion& GetRigidBodyRotation(void) const
			{
				return (rigidBodyRotation);
			}
			
			void Compress(Compressor& data) const;
			bool Decompress(Decompressor& data);
	};
	
	
	class ShapeIntersectionJob : public BatchJob, public Memory<ShapeIntersectionJob>
	{
		public:
			
			RigidBodyController		*alphaBody;
			RigidBodyController		*betaBody;
			const Shape				*alphaShape;
			const Shape				*betaShape;
			unsigned_int32			alphaIndex;
			unsigned_int32			betaIndex;
			
			RigidBodyContact		*rigidBodyContact;
			IntersectionData		intersectionData;
			
			ShapeIntersectionJob(ExecuteProc *execProc, PhysicsController *data, RigidBodyController *body1, RigidBodyController *body2, const Shape *shape1, const Shape *shape2, unsigned_int32 index1, unsigned_int32 index2);
	};
	
	
	class GeometryIntersectionJob : public BatchJob, public Memory<GeometryIntersectionJob>
	{
		public:
			
			RigidBodyController		*rigidBody;
			const Shape				*shapeNode;
			unsigned_int32			shapeIndex;
			Geometry				*geometryNode;
			
			int32					contactCount;
			GeometryContact			*geometryContact;
			IntersectionData		intersectionData[kMaxIntersectionContactCount];
			
			GeometryIntersectionJob(ExecuteProc *execProc, RigidBodyController *body, const Shape *shape, unsigned_int32 index, Geometry *geometry);
	};
	
	
	//# \class	PhysicsController		Manages the physics simulation for an entire world.
	//
	//# The $PhysicsController$ class manages the physics simulation for an entire world.
	//
	//# \def	class PhysicsController : public Controller
	//
	//# \ctor	PhysicsController();
	//
	//# \desc
	//# The $PhysicsController$ class manages the physics simulation for an entire world. A physics controller
	//# can only be assigned to a physics node, and exactly one physics node should exist in the root zone of
	//# any world using the features of the Physics Manager.
	//
	//# \base	Controller/Controller	A $PhysicsController$ is a specific type of controller.
	//
	//# \also	$@PhysicsNode@$
	//# \also	$@RigidBodyController@$
	
	
	//# \function	PhysicsController::GetMaxLinearSpeed	Returns the maximum linear speed for all rigid bodies.
	//
	//# \proto	float GetMaxLinearSpeed(void) const;
	//
	//# \desc
	//# The $GetMaxLinearSpeed$ function returns the maximum linear speed, measured in meters per second (m/s),
	//# that is allowed for all rigid bodies. If the magnitude of a rigid body's linear velocity ever exceeds
	//# this value, then is it clamped at the end of the simulation step.
	//
	//# \also	$@PhysicsController::SetMaxLinearSpeed@$
	//# \also	$@PhysicsController::GetMaxAngularSpeed@$
	//# \also	$@PhysicsController::SetMaxAngularSpeed@$
	
	
	//# \function	PhysicsController::SetMaxLinearSpeed	Sets the maximum linear speed for all rigid bodies.
	//
	//# \proto	void SetMaxLinearSpeed(float speed);
	//
	//# \param	speed	The new maximum linear speed, in meters per second (m/s).	
	//
	//# \desc
	//# The $SetMaxLinearSpeed$ function sets the maximum linear speed, measured in meters per second (m/s),
	//# that is allowed for all rigid bodies to the value specified by the $speed$ parameter. If the magnitude
	//# of a rigid body's linear velocity ever exceeds this value, then is it clamped at the end of the simulation step.
	//
	//# \also	$@PhysicsController::GetMaxLinearSpeed@$
	//# \also	$@PhysicsController::GetMaxAngularSpeed@$
	//# \also	$@PhysicsController::SetMaxAngularSpeed@$
	
	
	//# \function	PhysicsController::GetMaxAngularSpeed	Returns the maximum angular speed for all rigid bodies.
	//
	//# \proto	float GetMaxAngularSpeed(void) const;
	//
	//# \desc
	//# The $GetMaxAngularSpeed$ function returns the maximum angular speed, measured in radians per second (rad/s),
	//# that is allowed for all rigid bodies. If the magnitude of a rigid body's angular velocity ever exceeds this
	//# value, then is it clamped at the end of the simulation step.
	//
	//# \also	$@PhysicsController::SetMaxAngularSpeed@$
	//# \also	$@PhysicsController::GetMaxLinearSpeed@$
	//# \also	$@PhysicsController::SetMaxLinearSpeed@$
	
	
	//# \function	PhysicsController::SetMaxAngularSpeed	Sets the maximum angular speed for all rigid bodies.
	//
	//# \proto	void SetMaxAngularSpeed(float speed);
	//
	//# \param	speed	The new maximum angular speed, in radians per second (rad/s).	
	//
	//# \desc
	//# The $SetMaxAngularSpeed$ function sets the maximum angular speed, measured in radians per second (rad/s),
	//# that is allowed for all rigid bodies to the value specified by the $speed$ parameter. If the magnitude
	//# of a rigid body's angular velocity ever exceeds this value, then is it clamped at the end of the simulation step.
	//
	//# \also	$@PhysicsController::GetMaxAngularSpeed@$
	//# \also	$@PhysicsController::GetMaxLinearSpeed@$
	//# \also	$@PhysicsController::SetMaxLinearSpeed@$
	
	
	//# \function	PhysicsController::GetGravityAcceleration	Returns the global acceleration of gravity.
	//
	//# \proto	const Vector3D& GetGravityAcceleration(void) const;
	//
	//# \desc
	//# The $GetGravityAcceleration$ function returns the global world-space acceleration of gravity, measured
	//# in meters per second squared (m/s<sup>2</sup>).
	//
	//# \also	$@PhysicsController::SetGravityAcceleration@$
	//# \also	$@RigidBodyController::GetGravityMultiplier@$
	//# \also	$@RigidBodyController::SetGravityMultiplier@$
	
	
	//# \function	PhysicsController::SetGravityAcceleration	Sets the global acceleration of gravity.
	//
	//# \proto	void SetGravityAcceleration(const Vector3D& acceleration);
	//
	//# \param	acceleration	The new acceleration of gravity, in meters per second squared (m/s<sup>2</sup>).
	//
	//# \desc
	//# The $SetGravityAcceleration$ function sets the global world-space acceleration of gravity, measured
	//# in meters per second squared (m/s<sup>2</sup>), to the vector specified by the $acceleration$ parameter.
	//#
	//# The initial value of the gravity acceleration is (0,&nbsp;0,&nbsp;&minus;9.8) m/s<sup>2</sup>.
	//
	//# \also	$@PhysicsController::GetGravityAcceleration@$
	//# \also	$@RigidBodyController::GetGravityMultiplier@$
	//# \also	$@RigidBodyController::SetGravityMultiplier@$
	
	
	class PhysicsController : public Controller
	{
		friend class RigidBodyController;
		
		private:
			
			Graph<Body, Contact>		physicsGraph;
			Body						nullBody;
			
			int32						simulationStep;
			int32						simulationTime;
			
			unsigned_int32				rigidBodyParity;
			unsigned_int32				fieldApplicationStamp;
			
			List<RigidBodyController>	rigidBodyList[2];
			List<RigidBodyController>	sleepingList;
			
			Batch						collisionBatch;
			
			Vector3D					gravityAcceleration;
			
			float						maxLinearSpeed;
			float						maxAngularSpeed;
			
			int32						physicsCounter[kPhysicsCounterCount];
			
			static unsigned_int32 (*const simplexMinFunc[4])(const Point3D *, Point3D *);

			static unsigned_int32 CalculateZeroSimplexMinimum(const Point3D *simplex, Point3D *p);
			static unsigned_int32 CalculateOneSimplexMinimum(const Point3D *simplex, Point3D *p);
			static unsigned_int32 CalculateTwoSimplexMinimum(const Point3D *simplex, Point3D *p);
			static unsigned_int32 CalculateThreeSimplexMinimum(const Point3D *simplex, Point3D *p);
			
			static float SortRigidBodyList(List<RigidBodyController> *inputList, int32 depth, float minValue, float maxValue, int32 index, List<RigidBodyController> *outputList);
			void CollideRigidBodiesX(List<RigidBodyController> *inputList, int32 depth, float xmin, float xmax, List<RigidBodyController> *outputList);
			void CollideRigidBodiesY(List<RigidBodyController> *inputList, int32 depth, float ymin, float ymax, List<RigidBodyController> *outputList);
			void CollideRigidBodiesZ(List<RigidBodyController> *inputList, int32 depth, float zmin, float zmax, List<RigidBodyController> *outputList);
			
			void DetectBodyCollision(RigidBodyController *alphaBody, RigidBodyController *betaBody);
			
			static void ShapeCollisionJob(Job *job, void *cookie);
			static void FinalizeExistingShapeContact(Job *job, void *cookie);
			static void FinalizeNewShapeContact(Job *job, void *cookie);
		
		public:
			
			C4API PhysicsController();
			C4API ~PhysicsController();
			
			Body *GetNullBody(void)
			{
				return (&nullBody);
			}

			int32 GetSimulationStep(void) const
			{
				return (simulationStep);
			}
			
			unsigned_int32 IncrementFieldStamp(void)
			{
				return (++fieldApplicationStamp);
			}
			
			const Vector3D& GetGravityAcceleration(void) const
			{
				return (gravityAcceleration);
			}
			
			void SetGravityAcceleration(const Vector3D& acceleration)
			{
				gravityAcceleration = acceleration;
			}
			
			float GetMaxLinearSpeed(void) const
			{
				return (maxLinearSpeed);
			}
			
			void SetMaxLinearSpeed(float speed)
			{
				maxLinearSpeed = speed;
			}
			
			float GetMaxAngularSpeed(void) const
			{
				return (maxAngularSpeed);
			}
			
			void SetMaxAngularSpeed(float speed)
			{
				maxAngularSpeed = speed;
			}
			
			Body *GetFirstBody(void) const
			{
				return (physicsGraph.GetFirstElement());
			}
			
			RigidBodyController *GetFirstSleepingRigidBody(void) const
			{
				return (sleepingList.First());
			}
			
			int32 GetPhysicsCounter(int32 index) const
			{
				return (physicsCounter[index]);
			}
			
			void IncrementPhysicsCounter(int32 index)
			{
				physicsCounter[index]++;
			}

			static unsigned_int32 CalculateSimplexMinimum(int32 count, const Point3D *simplex, Point3D *p)
			{
				return ((*simplexMinFunc[count])(simplex, p));
			}
			
			static bool ValidNode(const Node *node);
			static void RegisterFunctions(ControllerRegistration *registration);
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			void AddRigidBody(RigidBodyController *rigidBody);
			void RemoveRigidBody(RigidBodyController *rigidBody);
			
			void WakeRigidBody(RigidBodyController *rigidBody);
			void SleepRigidBody(RigidBodyController *rigidBody);
			
			void Move(void);
	};
	
	
	class SetGravityFunction : public Function
	{
		private:
			
			float		gravityAcceleration;
			
			SetGravityFunction(const SetGravityFunction& setGravityFunction);
			
			Function *Replicate(void) const override;
		
		public:
			
			SetGravityFunction();
			~SetGravityFunction();
			
			float GetGravityAcceleration(void) const
			{
				return (gravityAcceleration);
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			
			void Compress(Compressor& data) const;
			bool Decompress(Decompressor& data);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};
	
	
	//# \class	PhysicsNode		Represents a physics node in a world.
	//
	//# The $PhysicsNode$ class represents a physics node in a world.
	//
	//# \def	class PhysicsNode : public Node
	//
	//# \ctor	PhysicsNode();
	//
	//# \desc
	//# The $PhysicsNode$ class represents a physics node in a world. Any world using the features of the
	//# Physics Manager should contain exactly one physics node in the root zone, and it should have a physics
	//# controller assigned to it.
	//
	//# \base	WorldMgr/Node	A $PhysicsNode$ is a scene graph node.
	//
	//# \also	$@PhysicsController@$
	
	
	class PhysicsNode : public Node
	{
		public:
			
			C4API PhysicsNode();
			C4API ~PhysicsNode();
			
			void EnterZone(Zone *zone);
	};
	
	
	#if C4DIAGNOSTICS
	
		class RigidBodyRenderable : public Renderable, public LinkTarget<RigidBodyRenderable>
		{
			private:
				
				List<Attribute>		attributeList;
				DiffuseAttribute	diffuseColor;
				
				Point3D				vertexArray[24];
				static const Line	lineArray[12];
			
			public:
				
				RigidBodyRenderable(const Box3D& box);
				~RigidBodyRenderable();
				
				void SetCollisionBox(const Box3D& box);
		};
		
		
		class ContactRenderable : public Renderable, public LinkTarget<ContactRenderable>
		{
			private:
				
				List<Attribute>			attributeList;
				DiffuseAttribute		diffuseColor;
				TextureMapAttribute		textureMap;
			
			protected:
				
				ContactRenderable(const ColorRGBA& color, const char *texture);
			
			public:
				
				~ContactRenderable();
				
				virtual void UpdateContact(int32 count, const Subcontact *subcontact) = 0;
		};
		
		
		class ContactVectorRenderable : public ContactRenderable
		{
			private:
				
				Point3D			vertexArray[Contact::kMaxSubcontactCount * 4];
				Vector4D		tangentArray[Contact::kMaxSubcontactCount * 4];
				Point2D			texcoordArray[Contact::kMaxSubcontactCount * 4];
			
			public:
				
				ContactVectorRenderable(const Subcontact *subcontact, const ColorRGBA& color);
				~ContactVectorRenderable();
				
				void UpdateContact(int32 count, const Subcontact *subcontact);
		};
		
		
		class ContactPointRenderable : public ContactRenderable
		{
			private:
			
				Point3D			vertexArray[Contact::kMaxSubcontactCount * 4];
				Vector2D		billboardArray[Contact::kMaxSubcontactCount * 4];
				Point2D			texcoordArray[Contact::kMaxSubcontactCount * 4];
				
			public:
				
				ContactPointRenderable(const Subcontact *subcontact, const ColorRGBA& color);
				~ContactPointRenderable();
				
				void UpdateContact(int32 count, const Subcontact *subcontact);
		};
	
	#endif
}


#endif

// ZYURVUR
