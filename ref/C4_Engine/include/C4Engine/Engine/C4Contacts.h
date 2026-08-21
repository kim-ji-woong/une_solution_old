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


#ifndef C4Contacts_h
#define C4Contacts_h


//# \component	Physics Manager
//# \prefix		PhysicsMgr/


#include "C4Node.h"


namespace C4
{
	typedef Type	ContactType;
	typedef Type	JointType;
	
	
	enum
	{
		kContactRigidBody		= 'BODY',
		kContactGeometry		= 'GEOM',
		kContactJoint			= 'JONT',
		kContactStaticJoint		= 'SJNT'
	};
	
	
	enum
	{
		kObjectJoint			= 'JONT'
	};
	
	
	//# \enum	JointType
	
	enum
	{
		kJointSpherical			= 'SPHR',		//## A spherical joint.
		kJointUniversal			= 'UNIV',		//## A universal joint.
		kJointDiscal			= 'DISC',		//## A discal joint.
		kJointRevolute			= 'RVLT',		//## A revolute joint.
		kJointCylindrical		= 'CYLD',		//## A cylindrical joint.
		kJointPrismatic			= 'PRSM'		//## A prismatic joint.
	};
	
	
	//# \enum	JointFlags
	
	enum
	{
		kJointBreakable			= 1 << 0		//## The joint can be broken if enough force is applied.
	};
	
	
	enum
	{
		kContactNotificationOutgoing	= 1 << 0,
		kContactNotificationIncoming	= 1 << 1
	};
	
	
	C4API extern const char kConnectorKeyBody1[];
	C4API extern const char kConnectorKeyBody2[];
	
	
	class Body;
	class Joint;
	class RigidBodyController;
	
	#if C4DIAGNOSTICS
	
		class ContactRenderable;
	
	#endif
	
	
	struct Jacobian
	{
		Vector3D	linear;
		Vector3D	angular;
			
		Jacobian(const Vector3D& lin, const Vector3D& ang)
		{
			linear = lin;
			angular = ang;
		}
	};
	
	
	struct Subcontact
	{
		int32			activeStep;
		unsigned_int32	triangleIndex;
		
		Point3D			alphaPosition;
		Point3D			betaPosition;
		
		Vector3D		bodyNormal;
		Vector3D		bodyTangent[2];
	};
	
	
	class Contact : public GraphEdge<Body, Contact>, public Packable, public Memory<Contact> 
	{
		public: 
			 
			enum 
			{
				kMaxSubcontactCount				= 4, 
				kMaxContactConstraintCount		= 12
			};
		
		private: 
			
			ContactType			contactType;
			bool				enabledFlag;
			 
			unsigned_int32		notificationMask;
			float				contactParam;
			
			static void StartBodyLinkProc(Node *node, void *cookie);
			static void FinishBodyLinkProc(Node *node, void *cookie);
		
		protected:
			
			float				cumulativeImpulse[kMaxContactConstraintCount];
			float				appliedImpulse[kMaxContactConstraintCount];
			
			#if C4DIAGNOSTICS
			
				Link<ContactRenderable>		contactVectorRenderable;
				Link<ContactRenderable>		contactPointRenderable;
			
			#endif
			
			Contact(ContactType type, Body *body1, Body *body2, unsigned_int32 mask = 0);
			
			void SetContactParam(float param)
			{
				contactParam = param;
			}
			
			void ResetConstraintImpulses(void)
			{
				for (machine a = 0; a < kMaxContactConstraintCount; a++)
				{
					cumulativeImpulse[a] = 0.0F;
					appliedImpulse[a] = 0.0F;
				}
			}
		
		public:
			
			virtual ~Contact();
			
			ContactType GetContactType(void) const
			{
				return (contactType);
			}
			
			bool Enabled(void) const
			{
				return (enabledFlag);
			}
			
			void Enable(void)
			{
				enabledFlag = true;
			}
			
			void Disable(void)
			{
				enabledFlag = false;
			}
			
			unsigned_int32 GetNotificationMask(void) const
			{
				return (notificationMask);
			}
			
			bool GetNotificationFlag(unsigned flag)
			{
				unsigned_int32 mask = notificationMask;
				notificationMask = mask & ~flag;
				return ((mask & flag) != 0);
			}
			
			float GetContactParam(void) const
			{
				return (contactParam);
			}
			
			static Contact *Construct(Unpacker& data, unsigned_int32 unpackFlags, Body *nullBody);
			
			void PackType(Packer& data) const;
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			virtual bool NonpersistentFinishNode(void) const;
			
			static float CalculateConstraintImpulse(const RigidBodyController *rigidBody1, const RigidBodyController *rigidBody2, const Jacobian& jacobian1, const Jacobian& jacobian2, float velocityBoost);
			static float CalculateConstraintImpulse(const RigidBodyController *rigidBody1, const RigidBodyController *rigidBody2, const Jacobian& jacobian1, const Jacobian& jacobian2);
			static float CalculateConstraintImpulse(const RigidBodyController *rigidBody, const Jacobian& jacobian, float velocityBoost);
			static float CalculateConstraintImpulse(const RigidBodyController *rigidBody, const Jacobian& jacobian);
			static float CalculateLinearConstraintImpulse(const RigidBodyController *rigidBody, const Vector3D& linearJacobian, float velocityBoost);
			static float CalculateLinearConstraintImpulse(const RigidBodyController *rigidBody, const Vector3D& linearJacobian);
			static float CalculateAngularConstraintImpulse(const RigidBodyController *rigidBody, const Vector3D& angularJacobian, float velocityBoost);
			static float CalculateAngularConstraintImpulse(const RigidBodyController *rigidBody, const Vector3D& angularJacobian);
			
			float AccumulateConstraintImpulse(int32 index, float multiplier, const Range<float>& range);
			float AccumulateConstraintImpulse(int32 index, float multiplier);
			
			virtual void ApplyConstraints(void);
	};
	
	
	class CollisionContact : public Contact
	{
		friend class Shape;
		
		private:
			
			int32				cachedSimplexVertexCount;
			Point3D				cachedAlphaVertex[4];
			Point3D				cachedBetaVertex[4];
		
		protected:
			
			int32				subcontactCount;
			Subcontact			subcontact[kMaxSubcontactCount];
			
			CollisionContact(ContactType type, Body *body1, Body *body2, unsigned_int32 mask = 0);
			~CollisionContact();
		
		public:
			
			int32 GetSubcontactCount(void) const
			{
				return (subcontactCount);
			}
			
			const Subcontact *GetSubcontact(int32 index) const
			{
				return (&subcontact[index]);
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
	};
	
	
	//# \class	RigidBodyContact		Represents a contact between two rigid bodies.
	//
	//# The $RigidBodyContact$ class represents a contact between two rigid bodies.
	//
	//# \def	class RigidBodyContact : public CollisionContact
	//
	//# \desc
	//# The $RigidBodyContact$ class represents a contact between single shapes belonging to two
	//# different rigid bodies. Instances of the $RigidBodyContact$ class are only created internally
	//# by the Physics Manager.
	//#
	//# When a rigid body makes a new contact with another rigid body, the $@RigidBodyController::HandleNewRigidBodyContact@$
	//# function is called for both rigid bodies.
	//#
	//# \privbase	CollisionContact	Used internally by the Physics Manager.
	//
	//# \also	$@GeometryContact@$
	//# \also	$@RigidBodyController::HandleNewRigidBodyContact@$
	
	
	//# \function	RigidBodyContact::GetWorldContactPosition		Returns the world-space contact position and normal vector.
	//
	//# \proto	void GetWorldContactPosition(const RigidBodyController *rigidBody, Point3D *position, Vector3D *normal) const;
	//
	//# \param	rigidBody		One of the rigid bodies involved in the contact.
	//# \param	position		A pointer to a location that receives the world-space position.
	//# \param	normal			A pointer to a location that receives the world-space normal vector.
	//
	//# \desc
	//# The $GetWorldContactPosition$ function returns the world-space position and normal vector corresponding to one of the
	//# two rigid bodies involved in a rigid body contact. The $rigidBody$ parameter must be a pointer to one of the rigid bodies
	//# connected in the contact graph by the $RigidBodyContact$ object for which this function is called. It would ordinarily
	//# be set to either a pointer to the object for which the $@RigidBodyController::HandleNewRigidBodyContact@$ function has been
	//# called or the the $contactBody$ parameter passed to the $@RigidBodyController::HandleNewRigidBodyContact@$ function.
	//#
	//# The $position$ parameter should specify a location to which the world-space contact position is written. This position
	//# represents the point on the surface of the <i>other</i> rigid body that is closest to the deepest penetration between the
	//# two rigid bodies.
	//#
	//# The $normal$ parameter should specify a location to which the world-space contact normal vector is written. The normal
	//# vector always points outward with respect to the rigid body specified by the $rigidBody$ parameter.
	//# (If the $GetWorldContactPosition$ were to be called for both rigid bodies involved in the contact, the normal vectors
	//# returned would be negatives of each other.)
	//
	//# \also	$@RigidBodyController::HandleNewRigidBodyContact@$
	
	
	class RigidBodyContact : public CollisionContact
	{
		friend class Contact;
		
		private:
			
			const Shape			*startShape;
			const Shape			*finishShape;
			
			unsigned_int32		startSignature;
			unsigned_int32		finishSignature;
			
			RigidBodyContact(Body *nullBody);
		
		public:
			
			RigidBodyContact(RigidBodyController *body1, RigidBodyController *body2, const Shape *shape1, const Shape *shape2, unsigned_int32 signature1, unsigned_int32 signature2, const IntersectionData *intersectionData);
			~RigidBodyContact();
			
			const Shape *GetStartShape(void) const
			{
				return (startShape);
			}
			
			const Shape *GetFinishShape(void) const
			{
				return (finishShape);
			}
			
			unsigned_int32 GetStartSignature(void) const
			{
				return (startSignature);
			}
			
			unsigned_int32 GetFinishSignature(void) const
			{
				return (finishSignature);
			}
			
			const Point3D& GetContactPosition(void) const
			{
				return (subcontact[0].alphaPosition);
			}
			
			const Point3D& GetBetaContactPosition(void) const
			{
				return (subcontact[0].betaPosition);
			}
			
			const Vector3D& GetContactNormal(void) const
			{
				return (subcontact[0].bodyNormal);
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			bool NonpersistentFinishNode(void) const;
			
			void UpdateContact(const IntersectionData *intersectionData);
			void ApplyConstraints(void);
			
			C4API void GetWorldContactPosition(const RigidBodyController *rigidBody, Point3D *position, Vector3D *normal) const;
	};
	
	
	//# \class	GeometryContact		Represents a contact between a rigid body and a geometry.
	//
	//# The $GeometryContact$ class represents a contact between a rigid body and a geometry.
	//
	//# \def	class GeometryContact : public CollisionContact
	//
	//# \desc
	//# The $GeometryContact$ class represents a contact between a single shape belonging to a
	//# rigid bodies and a geometry node. Instances of the $GeometryContact$ class are only created
	//# internally by the Physics Manager.
	//#
	//# When a rigid body makes a new contact with a geometry node, the $@RigidBodyController::HandleNewGeometryContact@$
	//# function is called for the rigid body.
	//#
	//# \privbase	CollisionContact	Used internally by the Physics Manager.
	//
	//# \also	$@RigidBodyContact@$
	//# \also	$@RigidBodyController::HandleNewGeometryContact@$
	
	
	//# \function	GeometryContact::GetContactGeometry		Returns the geometry node involved in a contact.
	//
	//# \proto	Geometry *GetContactGeometry(void) const;
	//
	//# \desc
	//# The $GetContactGeometry$ function returns a pointer to the geometry node involved in a contact.
	
	
	class GeometryContact : public CollisionContact
	{
		friend class Contact;
		
		private:
			
			Geometry			*contactGeometry;
			unsigned_int32		contactSignature;
			
			float				normalVelocityMagnitude;
			
			GeometryContact(Body *nullBody);
			
			static void GeometryLinkProc(Node *node, void *cookie);
		
		public:
			
			GeometryContact(Geometry *geometry, RigidBodyController *body, const IntersectionData *intersectionData, unsigned_int32 signature);
			~GeometryContact();
			
			Geometry *GetContactGeometry(void) const
			{
				return (contactGeometry);
			}
			
			unsigned_int32 GetContactSignature(void) const
			{
				return (contactSignature);
			}
			
			const Point3D& GetRigidBodyContactPosition(void) const
			{
				return (subcontact[0].alphaPosition);
			}
			
			const Point3D& GetGeometryContactPosition(void) const
			{
				return (subcontact[0].betaPosition);
			}
			
			const Vector3D& GetRigidBodyContactNormal(void) const
			{
				return (subcontact[0].bodyNormal);
			}
			
			float GetNormalVelocityMagnitude(void) const
			{
				return (normalVelocityMagnitude);
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			bool NonpersistentFinishNode(void) const;
			
			void UpdateContact(const IntersectionData *intersectionData);
			void ApplyConstraints(void);
	};
	
	
	class JointContact : public Contact, public LinkTarget<JointContact>
	{
		friend class Contact;
		
		private:
			
			Joint		*contactJoint;
			
			JointContact(Body *nullBody);
			
			static void JointLinkProc(Node *node, void *cookie);
		
		protected:
			
			JointContact(ContactType type, Joint *joint, Body *body1, Body *body2);
			JointContact(ContactType type, Body *nullBody);
			
			Joint *GetContactJoint(void) const
			{
				return (contactJoint);
			}
		
		public:
			
			JointContact(Joint *joint, RigidBodyController *body1, RigidBodyController *body2);
			~JointContact();
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			void ApplyConstraints(void);
	};
	
	
	class StaticJointContact : public JointContact
	{
		friend class Contact;
		
		private:
			
			StaticJointContact(Body *nullBody);
		
		public:
			
			StaticJointContact(Joint *joint, RigidBodyController *body);
			~StaticJointContact();
			
			void ApplyConstraints(void);
	};
	
	
	//# \class	JointObject		Encapsulates data pertaining to a physics joint.
	//
	//# The $JointObject$ class encapsulates data pertaining to a physics joint.
	//
	//# \def	class JointObject : public Object
	//
	//# \ctor	JointObject(JointType type);
	//
	//# The constructor has protected access. The $JointObject$ class can only exist as the base class for another class.
	//
	//# \param	type	The type of the joint. See below for a list of possible types.
	//
	//# \desc
	//# The $JointObject$ class encapsulates data describing a joint. A joint can be used to connect a rigid body
	//# to another rigid body or to static geometry, and the type of joint determines what kind of motion is allowed
	//# for the connected rigid bodies.
	//#
	//# A joint object can be of one of the following types.
	//
	//# \table	JointType
	//
	//# \base	WorldMgr/Object		A $JointObject$ is an object that can be shared by multiple joint nodes.
	//
	//# \also	$@Joint@$
	//# \also	$@RigidBodyController@$
	
	
	//# \function	JointObject::GetJointType		Returns the specific type of a joint.
	//
	//# \proto	JointType GetJointType(void) const;
	//
	//# \desc
	//# The $GetJointType$ function returns the specific joint type, which may be one of the following values.
	//
	//# \table	JointType
	
	
	//# \function	JointObject::GetJointFlags		Returns the joint flags.
	//
	//# \proto	unsigned_int32 GetJointFlags(void) const;
	//
	//# \desc
	//# The $GetJointFlags$ function returns the joint flags, which can be a combination
	//# (through logical OR) of the following values.
	//
	//# \table	JointFlags
	//
	//# \also	$@JointObject::SetJointFlags@$
	
	
	//# \function	JointObject::SetJointFlags		Sets the joint flags.
	//
	//# \proto	void SetJointFlags(unsigned_int32 flags);
	//
	//# \param	flags	The new joint flags. See below for possible values.
	//
	//# \desc
	//# The $SetJointFlags$ function sets the joint flags. The $flags$ parameter can be a combination
	//# (through logical OR) of the following values.
	//
	//# \table	JointFlags
	//
	//# The initial value of the joint flags is 0.
	//
	//# \also	$@JointObject::GetJointFlags@$
	
	
	//# \function	JointObject::GetBreakingForce	Returns the breaking force for a joint.
	//
	//# \proto	float GetBreakingForce(void) const;
	//
	//# \desc
	//# The $GetBreakingForce$ function returns the breaking force for a joint, measured in kilonewtons (kN).
	//# The breaking force is only used if the $kJointBreakable$ flag is set for the joint.
	//
	//# \also	$@JointObject::SetBreakingForce@$
	//# \also	$@JointObject::GetJointFlags@$
	//# \also	$@JointObject::SetJointFlags@$
	
	
	//# \function	JointObject::SetBreakingForce	Sets the breaking force for a joint.
	//
	//# \proto	void SetBreakingForce(float force);
	//
	//# \param	force	The new breaking force.
	//
	//# \desc
	//# The $SetBreakingForce$ function sets the breaking force for a joint, measured in kilonewtons (kN).
	//# The breaking force is only used if the $kJointBreakable$ flag is set for the joint.
	//#
	//# The initial value of the breaking force is 0, so it should be explicitly set to a positive value at the
	//# time when a joint is made breakable. Otherwise, the joint will tend to fall apart on the first simulation step.
	//
	//# \also	$@JointObject::GetBreakingForce@$
	//# \also	$@JointObject::GetJointFlags@$
	//# \also	$@JointObject::SetJointFlags@$
	
	
	class JointObject : public Object
	{
		friend class WorldMgr;
		
		private:
			
			JointType		jointType;
			
			unsigned_int32	jointFlags;
			float			breakingForce;
			
			static JointObject *Construct(Unpacker& data, unsigned_int32 unpackFlags);
		
		protected:
			
			JointObject(JointType type);
			~JointObject();
		
		public:
			
			JointType GetJointType(void) const
			{
				return (jointType);
			}
			
			unsigned_int32 GetJointFlags(void) const
			{
				return (jointFlags);
			}
			
			void SetJointFlags(unsigned_int32 flags)
			{
				jointFlags = flags;
			}
			
			float GetBreakingForce(void) const
			{
				return (breakingForce);
			}
			
			void SetBreakingForce(float force)
			{
				breakingForce = force;
			}
			
			void PackType(Packer& data) const;
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetCategoryCount(void) const;
			Type GetCategoryType(int32 index, const char **title) const;
			int32 GetCategorySettingCount(Type category) const;
			Setting *GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const;
			void SetCategorySetting(Type category, const Setting *setting);
	};
	
	
	//# \class	SphericalJointObject	Encapsulates data pertaining to a spherical joint.
	//
	//# The $SphericalJointObject$ class encapsulates data pertaining to a spherical joint.
	//
	//# \def	class SphericalJointObject : public JointObject
	//
	//# \ctor	SphericalJointObject();
	//
	//# \desc
	//# The $SphericalJointObject$ class encapsulates a physics joint that allows rotation about all three axes.
	//# See the $@SphericalJoint@$ class for a description of the joint kinematics.
	//
	//# \base	JointObject		A $SphericalJointObject$ is an object that can be shared by multiple spherical joint nodes.
	//
	//# \also	$@SphericalJoint@$
	
	
	class SphericalJointObject : public JointObject
	{
		private:
			
			~SphericalJointObject();
		
		public:
			
			SphericalJointObject();
	};
	
	
	//# \class	UniversalJointObject	Encapsulates data pertaining to a universal joint.
	//
	//# The $UniversalJointObject$ class encapsulates data pertaining to a universal joint.
	//
	//# \def	class UniversalJointObject : public JointObject
	//
	//# \ctor	UniversalJointObject();
	//
	//# \desc
	//# The $UniversalJointObject$ class encapsulates a physics joint that allows rotation about the two non-radial axes.
	//# See the $@UniversalJoint@$ class for a description of the joint kinematics.
	//
	//# \base	JointObject		A $UniversalJointObject$ is an object that can be shared by multiple universal joint nodes.
	//
	//# \also	$@UniversalJoint@$
	
	
	class UniversalJointObject : public JointObject
	{
		private:
			
			~UniversalJointObject();
		
		public:
			
			UniversalJointObject();
	};
	
	
	//# \class	DiscalJointObject	Encapsulates data pertaining to a discal joint.
	//
	//# The $DiscalJointObject$ class encapsulates data pertaining to a discal joint.
	//
	//# \def	class DiscalJointObject : public JointObject
	//
	//# \ctor	DiscalJointObject();
	//
	//# \desc
	//# The $DiscalJointObject$ class encapsulates a physics joint that allows rotation about the radial axis and one non-radial axis.
	//# See the $@DiscalJoint@$ class for a description of the joint kinematics.
	//
	//# \base	JointObject		A $DiscalJointObject$ is an object that can be shared by multiple discal joint nodes.
	//
	//# \also	$@DiscalJoint@$
	
	
	class DiscalJointObject : public JointObject
	{
		private:
			
			~DiscalJointObject();
		
		public:
			
			DiscalJointObject();
	};
	
	
	//# \class	RevoluteJointObject		Encapsulates data pertaining to a revolute joint.
	//
	//# The $RevoluteJointObject$ class encapsulates data pertaining to a revolute joint.
	//
	//# \def	class RevoluteJointObject : public JointObject
	//
	//# \ctor	RevoluteJointObject();
	//
	//# \desc
	//# The $RevoluteJointObject$ class encapsulates a physics joint that allows rotation about a single axis.
	//# See the $@RevoluteJoint@$ class for a description of the joint kinematics.
	//
	//# \base	JointObject		A $RevoluteJointObject$ is an object that can be shared by multiple revolute joint nodes.
	//
	//# \also	$@RevoluteJoint@$
	
	
	class RevoluteJointObject : public JointObject
	{
		private:
			
			~RevoluteJointObject();
		
		public:
			
			RevoluteJointObject();
	};
	
	
	//# \class	CylindricalJointObject		Encapsulates data pertaining to a cylindrical joint.
	//
	//# The $CylindricalJointObject$ class encapsulates data pertaining to a cylindrical joint.
	//
	//# \def	class CylindricalJointObject : public JointObject
	//
	//# \ctor	CylindricalJointObject();
	//
	//# \desc
	//# The $CylindricalJointObject$ class encapsulates a physics joint that allows rotation about a single axis and translation along the same axis.
	//# See the $@CylindricalJoint@$ class for a description of the joint kinematics.
	//
	//# \base	JointObject		A $CylindricalJointObject$ is an object that can be shared by multiple cylindrical joint nodes.
	//
	//# \also	$@CylindricalJoint@$
	
	
	class CylindricalJointObject : public JointObject
	{
		private:
			
			~CylindricalJointObject();
		
		public:
			
			CylindricalJointObject();
	};
	
	
	//# \class	PrismaticJointObject	Encapsulates data pertaining to a prismatic joint.
	//
	//# The $PrismaticJointObject$ class encapsulates data pertaining to a prismatic joint.
	//
	//# \def	class PrismaticJointObject : public JointObject
	//
	//# \ctor	PrismaticJointObject();
	//
	//# \desc
	//# The $PrismaticJointObject$ class encapsulates a physics joint that allows translation along a single axis.
	//# See the $@PrismaticJoint@$ class for a description of the joint kinematics.
	//
	//# \base	JointObject		A $PrismaticJointObject$ is an object that can be shared by multiple prismatic joint nodes.
	//
	//# \also	$@PrismaticJoint@$
	
	
	class PrismaticJointObject : public JointObject
	{
		private:
			
			~PrismaticJointObject();
		
		public:
			
			PrismaticJointObject();
	};
	
	
	//# \class	Joint		Represents a joint node in a world.
	//
	//# The $Joint$ class represents a joint node in a world.
	//
	//# \def	class Joint : public Node
	//
	//# \ctor	Joint(JointType type);
	//
	//# The constructor has protected access. A $Joint$ class can only exist as the base class for a more specific type of joint.
	//
	//# \param	type	The type of the joint. See below for a list of possible types.
	//
	//# \desc
	//# The $Joint$ class represents a joint node in a world. A joint can be used to connect a rigid body
	//# to another rigid body or to static geometry, and the type of joint determines what kind of motion is allowed
	//# for the connected rigid bodies.
	//#
	//# A joint node can be of one of the following types.
	//
	//# \table	JointType
	//
	//# \base	WorldMgr/Node		A $Joint$ node is a scene graph node.
	//
	//# \also	$@JointObject@$
	//# \also	$@RigidBodyController@$
	
	
	//# \function	Joint::GetJointType		Returns the specific type of a joint.
	//
	//# \proto	JointType GetJointType(void) const;
	//
	//# \desc
	//# The $GetJointType$ function returns the specific joint type, which can be one of the following values.
	//
	//# \table	JointType
	
	
	//# \function	Joint::GetFirstRigidBodyConnector		Returns the first rigid body connected to a joint.
	//
	//# \proto	Node *GetFirstRigidBodyConnector(void) const;
	//
	//# \desc
	//# The $GetFirstRigidBodyConnector$ function returns the first rigid body connected to a joint.
	//# If there is no first rigid body connected to the joint, then the return value is $nullptr$.
	//
	//# \also	$@Joint::SetFirstRigidBodyConnector@$
	//# \also	$@Joint::GetSecondRigidBodyConnector@$
	//# \also	$@Joint::SetSecondRigidBodyConnector@$
	
	
	//# \function	Joint::SetFirstRigidBodyConnector		Sets the first rigid body connected to a joint.
	//
	//# \proto	void SetFirstRigidBodyConnector(Node *node);
	//
	//# \param	node	The node representing the first rigid body.
	//
	//# \desc
	//# The $SetFirstRigidBodyConnector$ function sets the first rigid body connected to a joint to the node specified
	//# by the $node$ parameter. This node must have a rigid body controller assigned to it in order to be affected
	//# by the joint.
	//#
	//# The first rigid body connector is initially set to $nullptr$.
	//
	//# \also	$@Joint::GetFirstRigidBodyConnector@$
	//# \also	$@Joint::GetSecondRigidBodyConnector@$
	//# \also	$@Joint::SetSecondRigidBodyConnector@$
	
	
	//# \function	Joint::GetSecondRigidBodyConnector		Returns the second rigid body connected to a joint.
	//
	//# \proto	Node *GetSecondRigidBodyConnector(void) const;
	//
	//# \desc
	//# The $GetSecondRigidBodyConnector$ function returns the second rigid body connected to a joint.
	//# If there is no second rigid body connected to the joint, then the return value is $nullptr$.
	//
	//# \also	$@Joint::SetSecondRigidBodyConnector@$
	//# \also	$@Joint::GetFirstRigidBodyConnector@$
	//# \also	$@Joint::SetFirstRigidBodyConnector@$
	
	
	//# \function	Joint::SetSecondRigidBodyConnector		Sets the second rigid body connected to a joint.
	//
	//# \proto	void SetSecondRigidBodyConnector(Node *node);
	//
	//# \param	node	The node representing the second rigid body.
	//
	//# \desc
	//# The $SetSecondRigidBodyConnector$ function sets the second rigid body connected to a joint to the node specified
	//# by the $node$ parameter. This node must have a rigid body controller assigned to it in order to be affected
	//# by the joint.
	//#
	//# If the first rigid body connector is $nullptr$, then the second rigid body connector is ignored. When connecting
	//# only one node to a joint, the first rigid body connector should always be used.
	//#
	//# The second rigid body connector is initially set to $nullptr$.
	//
	//# \also	$@Joint::GetSecondRigidBodyConnector@$
	//# \also	$@Joint::GetFirstRigidBodyConnector@$
	//# \also	$@Joint::SetFirstRigidBodyConnector@$
	
	
	class Joint : public Node
	{
		friend class Node;
		
		private:
			
			JointType			jointType;
			
			Link<JointContact>	jointContact;
			Point3D				jointPosition[2];
			Vector3D			jointDirection[2];
			float				jointDistance[2];
			
			static Joint *Construct(Unpacker& data, unsigned_int32 unpackFlags);
			
			#if C4LEGACY
			
				static void FirstRigidBodyLinkProc(Node *node, void *cookie);
				static void SecondRigidBodyLinkProc(Node *node, void *cookie);
			
			#endif
			
			RigidBodyController *FindRigidBody(const Node *node, Point3D *position, Vector3D *direction) const;
		
		protected:
			
			Joint(JointType type);
			Joint(const Joint& joint);
			
			JointContact *GetJointContact(void) const
			{
				return (jointContact);
			}
			
			const Point3D& GetJointPosition(int32 index) const
			{
				return (jointPosition[index]);
			}
			
			const Vector3D& GetJointDirection(int32 index) const
			{
				return (jointDirection[index]);
			}
			
			float GetJointDistance(int32 index) const
			{
				return (jointDistance[index]);
			}
		
		public:
			
			virtual ~Joint();
			
			JointType GetJointType(void) const
			{
				return (jointType);
			}
			
			JointObject *GetObject(void) const
			{
				return (static_cast<JointObject *>(Node::GetObject()));
			}
			
			Node *GetFirstConnectedRigidBody(void) const
			{
				return (GetConnectedNode(kConnectorKeyBody1));
			}
			
			Node *GetSecondConnectedRigidBody(void) const
			{
				return (GetConnectedNode(kConnectorKeyBody2));
			}
			
			void PackType(Packer& data) const;
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetInternalConnectorCount(void) const;
			const char *GetInternalConnectorKey(int32 index) const;
			C4API void SetFirstConnectedRigidBody(Node *node);
			C4API void SetSecondConnectedRigidBody(Node *node);
			
			void Preprocess(void);
			
			Body *GetNullBody(void) const;
			
			virtual void ApplyConstraints(Contact *contact) const = 0;
			virtual void ApplyStaticContraints(Contact *contact) const = 0;
	};
	
	
	//# \class	SphericalJoint		Represents a spherical joint node in a world.
	//
	//# The $SphericalJoint$ class represents a spherical joint node in a world.
	//
	//# \def	class SphericalJoint : public Joint
	//
	//# \ctor	SphericalJoint();
	//
	//# \desc
	//# The $SphericalJoint$ class represents a spherical joint node in a world. A spherical joint
	//# has three rotational degrees of freedom.
	//#
	//# When a spherical joint is used to connect a rigid body to static geometry, it allows rotation
	//# about any axis, but maintains a constant distance between the rigid body and the joint position.
	//#
	//# When a spherical joint is used to connect two rigid bodies, the original joint position is saved
	//# in node-space coordinates for both rigid bodies. The rigid bodies are allowed to rotate about any axis,
	//# but the two saved joint positions are required to coincide in world-space, maintaining the original
	//# distances between the rigid bodies and the original joint position.
	//
	//# \base	Joint		A spherical joint is a specific type of joint.
	//
	//# \also	$@SphericalJointObject@$
	
	
	class SphericalJoint : public Joint
	{
		friend class Joint;
		
		private:
			
			SphericalJoint(int);
			SphericalJoint(const SphericalJoint& sphericalJoint);
			
			Node *Replicate(void) const override;
		
		public:
			
			C4API SphericalJoint();
			C4API ~SphericalJoint();
			
			SphericalJointObject *GetObject(void) const
			{
				return (static_cast<SphericalJointObject *>(Node::GetObject()));
			}
			
			void ApplyConstraints(Contact *contact) const;
			void ApplyStaticContraints(Contact *contact) const;
	};
	
	
	//# \class	UniversalJoint		Represents a universal joint node in a world.
	//
	//# The $UniversalJoint$ class represents a universal joint node in a world.
	//
	//# \def	class UniversalJoint : public Joint
	//
	//# \ctor	UniversalJoint();
	//
	//# \desc
	//# The $UniversalJoint$ class represents a universal joint node in a world. A universal joint
	//# has two rotational degrees of freedom.
	//#
	//# When a universal joint is used to connect a rigid body to static geometry, it allows rotation
	//# about the two non-radial axes and maintains a constant distance between the rigid body and the joint position.
	//#
	//# When a universal joint is used to connect two rigid bodies, the original joint position is saved
	//# in node-space coordinates for both rigid bodies. The rigid bodies are allowed to rotate about the two non-radial axes,
	//# and the two saved joint positions are required to coincide in world-space, maintaining the original
	//# distances between the rigid bodies and the original joint position.
	//
	//# \base	Joint		A universal joint is a specific type of joint.
	//
	//# \also	$@UniversalJointObject@$
	
	
	class UniversalJoint : public Joint
	{
		friend class Joint;
		
		private:
			
			UniversalJoint(int);
			UniversalJoint(const UniversalJoint& universalJoint);
			
			Node *Replicate(void) const override;
		
		public:
			
			C4API UniversalJoint();
			C4API ~UniversalJoint();
			
			UniversalJointObject *GetObject(void) const
			{
				return (static_cast<UniversalJointObject *>(Node::GetObject()));
			}
			
			void ApplyConstraints(Contact *contact) const;
			void ApplyStaticContraints(Contact *contact) const;
	};
	
	
	//# \class	DiscalJoint		Represents a discal joint node in a world.
	//
	//# The $DiscalJoint$ class represents a discal joint node in a world.
	//
	//# \def	class DiscalJoint : public Joint
	//
	//# \ctor	DiscalJoint();
	//
	//# \desc
	//# The $DiscalJoint$ class represents a discal joint node in a world. A discal joint
	//# has two rotational degrees of freedom.
	//#
	//# When a discal joint is used to connect a rigid body to static geometry, it allows rotation
	//# about the joint's <i>z</i> axis and the radial axis while maintaining a constant distance between the rigid body and the joint position.
	//#
	//# When a discal joint is used to connect two rigid bodies, the original joint position and <i>z</i> axis direction are saved
	//# in node-space coordinates for both rigid bodies. The rigid bodies are allowed to rotate about the <i>z</i> axis and the radial axis,
	//# and the two saved joint positions and two saved <i>z</i> axis directions are required to coincide in world-space, maintaining the original
	//# distances and orientation between the rigid bodies and the original joint configuration.
	//
	//# \base	Joint		A discal joint is a specific type of joint.
	//
	//# \also	$@DiscalJointObject@$
	
	
	class DiscalJoint : public Joint
	{
		friend class Joint;
		
		private:
			
			DiscalJoint(int);
			DiscalJoint(const DiscalJoint& discalJoint);
			
			Node *Replicate(void) const override;
		
		public:
			
			C4API DiscalJoint();
			C4API ~DiscalJoint();
			
			DiscalJointObject *GetObject(void) const
			{
				return (static_cast<DiscalJointObject *>(Node::GetObject()));
			}
			
			void ApplyConstraints(Contact *contact) const;
			void ApplyStaticContraints(Contact *contact) const;
	};
	
	
	//# \class	RevoluteJoint		Represents a revolute joint node in a world.
	//
	//# The $RevoluteJoint$ class represents a revolute joint node in a world.
	//
	//# \def	class RevoluteJoint : public Joint
	//
	//# \ctor	RevoluteJoint();
	//
	//# \desc
	//# The $RevoluteJoint$ class represents a revolute joint node in a world. A revolute joint
	//# has one rotational degree of freedom and behaves like a hinge.
	//#
	//# When a revolute joint is used to connect a rigid body to static geometry, it allows rotation
	//# about the joint's <i>z</i> axis and maintains a constant distance between the rigid body and the joint position.
	//#
	//# When a revolute joint is used to connect two rigid bodies, the original joint position and <i>z</i> axis direction are saved
	//# in node-space coordinates for both rigid bodies. The rigid bodies are allowed to rotate about the <i>z</i> axis,
	//# and the two saved joint positions and two saved <i>z</i> axis directions are required to coincide in world-space,
	//# maintaining the original distances and orientation between the rigid bodies and the original joint configuration.
	//
	//# \base	Joint		A revolute joint is a specific type of joint.
	//
	//# \also	$@RevoluteJointObject@$
	
	
	class RevoluteJoint : public Joint
	{
		friend class Joint;
		
		private:
			
			RevoluteJoint(int);
			RevoluteJoint(const RevoluteJoint& revoluteJoint);
			
			Node *Replicate(void) const override;
		
		public:
			
			C4API RevoluteJoint();
			C4API ~RevoluteJoint();
			
			RevoluteJointObject *GetObject(void) const
			{
				return (static_cast<RevoluteJointObject *>(Node::GetObject()));
			}
			
			void ApplyConstraints(Contact *contact) const;
			void ApplyStaticContraints(Contact *contact) const;
	};
	
	
	//# \class	CylindricalJoint	Represents a cylindrical joint node in a world.
	//
	//# The $CylindricalJoint$ class represents a cylindrical joint node in a world.
	//
	//# \def	class CylindricalJoint : public Joint
	//
	//# \ctor	CylindricalJoint();
	//
	//# \desc
	//# The $CylindricalJoint$ class represents a cylindrical joint node in a world. A cylindrical joint
	//# has one translational degree of freedom and one rotational degree of freedom. It behaves like a piston
	//# that allows rotation about the translational axis.
	//#
	//# When a cylindrical joint is used to connect a rigid body to static geometry, it allows only translation along the
	//# joint's <i>z</i> axis and rotation about the joint's <i>z</i> axis.
	//#
	//# When a revolute joint is used to connect two rigid bodies, the original <i>z</i> axis direction is saved
	//# in node-space coordinates for both rigid bodies. The rigid bodies are allowed to translate along the <i>z</i> axis
	//# and rotate about the <i>z</i> axis, and the two saved <i>z</i> axis directions are required to coincide in world-space,
	//# maintaining the original orientation between the rigid bodies and the original joint configuration.
	//
	//# \base	Joint		A cylindrical joint is a specific type of joint.
	//
	//# \also	$@CylindricalJointObject@$
	
	
	class CylindricalJoint : public Joint
	{
		friend class Joint;
		
		private:
			
			CylindricalJoint(int);
			CylindricalJoint(const CylindricalJoint& cylindricalJoint);
			
			Node *Replicate(void) const override;
		
		public:
			
			C4API CylindricalJoint();
			C4API ~CylindricalJoint();
			
			CylindricalJointObject *GetObject(void) const
			{
				return (static_cast<CylindricalJointObject *>(Node::GetObject()));
			}
			
			void ApplyConstraints(Contact *contact) const;
			void ApplyStaticContraints(Contact *contact) const;
	};
	
	
	//# \class	PrismaticJoint	Represents a prismatic joint node in a world.
	//
	//# The $PrismaticJoint$ class represents a prismatic joint node in a world.
	//
	//# \def	class PrismaticJoint : public Joint
	//
	//# \ctor	PrismaticJoint();
	//
	//# \desc
	//# The $PrismaticJoint$ class represents a prismatic joint node in a world. A prismatic joint
	//# has one translational degree of freedom and behaves like a non-rotating piston.
	//#
	//# When a prismatic joint is used to connect a rigid body to static geometry, it allows only translation along the
	//# joint's <i>z</i> axis.
	//#
	//# When a prismatic joint is used to connect two rigid bodies, the original <i>z</i> axis direction is saved
	//# in node-space coordinates for both rigid bodies. The rigid bodies are allowed to translate along the <i>z</i> axis,
	//# and the two saved <i>z</i> axis directions are required to coincide in world-space, maintaining the original
	//# orientation between the rigid bodies and the original joint configuration.
	//
	//# \base	Joint		A prismatic joint is a specific type of joint.
	//
	//# \also	$@PrismaticJointObject@$
	
	
	class PrismaticJoint : public Joint
	{
		friend class Joint;
		
		private:
			
			PrismaticJoint(int);
			PrismaticJoint(const PrismaticJoint& prismaticJoint);
			
			Node *Replicate(void) const override;
		
		public:
			
			C4API PrismaticJoint();
			C4API ~PrismaticJoint();
			
			PrismaticJointObject *GetObject(void) const
			{
				return (static_cast<PrismaticJointObject *>(Node::GetObject()));
			}
			
			void ApplyConstraints(Contact *contact) const;
			void ApplyStaticContraints(Contact *contact) const;
	};
}


#endif

// ZYURVUR
