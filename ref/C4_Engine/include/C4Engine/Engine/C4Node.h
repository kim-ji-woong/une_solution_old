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


#ifndef C4Node_h
#define C4Node_h


//# \component	World Manager
//# \prefix		WorldMgr/


#include "C4Cell.h"
#include "C4Resources.h"
#include "C4Properties.h"
#include "C4Objects.h"
#include "C4Connector.h"
#include "C4Renderable.h"


namespace C4
{
	//# \tree	Node
	//
	//# \node	Camera
	//# \sub
	//#		\node	OrthoCamera
	//#		\node	FrustumCamera
	//#		\sub
	//#			\node	RemoteCamera
	//#			\node	OrientedCamera
	//#			\sub
	//#				\node	SpectatorCamera
	//#				\node	BenchmarkCamera
	//#			\end
	//#		\end
	//# \end
	//
	//# \node	Light
	//# \sub
	//#		\node	InfiniteLight
	//#		\sub
	//#			\node	DepthLight
	//#			\node	LandscapeLight
	//#		\end
	//#		\node	PointLight
	//#		\sub
	//#			\node	CubeLight
	//#			\node	SpotLight
	//#		\end
	//# \end
	//
	//# \node	Source
	//# \sub
	//#		\node	AmbientSource
	//#		\node	OmniSource
	//#		\sub
	//#			\node	DirectedSource
	//#		\end
	//# \end
	//
	//# \node	Geometry
	//# \sub
	//#		\node	MeshGeometry
	//#		\node	PrimitiveGeometry
	//#		\sub
	//#			\node	PlateGeometry
	//#			\node	DiskGeometry
	//#			\node	HoleGeometry
	//#			\node	AnnulusGeometry
	//#			\node	BoxGeometry
	//#			\node	PyramidGeometry
	//#			\node	CylinderGeometry
	//#			\node	ConeGeometry
	//#			\node	SphereGeometry
	//#			\node	DomeGeometry
	//#			\node	TorusGeometry
	//#			\node	PathPrimitiveGeometry
	//#			\sub
	//#				\node	TubeGeometry
	//#				\node	ExtrusionGeometry
	//#				\node	RevolutionGeometry
	//#			\end
	//#			\node	PhysicsMgr/ClothGeometry
	//#			\node	PhysicsMgr/WaterGeometry
	//#		\end
	//# \end
	//
	//# \node	Instance
	//
	//# \node	Model
	//# \sub
	//#		\node	GenericModel
	//# \end
	//
	//# \node	Bone
	//
	//# \node	Marker
	//# \sub
	//#		\node	LocatorMarker
	//#		\node	ConnectionMarker
	//#		\node	CubeMarker
	//# \end
	//
	//# \node	Trigger
	//# \sub 
	//#		\node	BoxTrigger
	//#		\node	CylinderTrigger 
	//#		\node	SphereTrigger 
	//# \end 
	//
	//# \node	EffectMgr/Effect 
	//# \sub
	//#		\node	EffectMgr/ParticleSystem
	//#		\sub
	//#			\node	EffectMgr/PointParticleSystem 
	//#			\node	EffectMgr/InfinitePointParticleSystem
	//#			\node	EffectMgr/LineParticleSystem
	//#			\node	EffectMgr/QuadParticleSystem
	//#			\node	EffectMgr/FireParticleSystem 
	//#			\node	EffectMgr/PolyboardParticleSystem
	//#			\node	EffectMgr/BlobParticleSystem
	//#		\end
	//#		\node	EffectMgr/MarkingEffect
	//#		\node	EffectMgr/QuadEffect
	//#		\node	EffectMgr/FlareEffect
	//#		\node	EffectMgr/BeamEffect
	//#		\node	EffectMgr/TubeEffect
	//#		\node	EffectMgr/FireEffect
	//#		\node	EffectMgr/PanelEffect
	//#		\node	ExtrasPlugin/ShockwaveEffect
	//#		\node	ExtrasPlugin/ShellEffect
	//# \end
	//
	//# \node	EffectMgr/Emitter
	//# \sub
	//#		\node	EffectMgr/BoxEmitter
	//#		\node	EffectMgr/CylinderEmitter
	//#		\node	EffectMgr/SphereEmitter
	//# \end
	//
	//# \node	Space
	//#	\sub
	//#		\node	FogSpace
	//#		\node	ShadowSpace
	//#		\node	AmbientSpace
	//#		\node	AcousticsSpace
	//#		\node	OcclusionSpace
	//#		\node	PaintSpace
	//#	\end
	//
	//# \node	Portal
	//# \sub
	//#		\node	DirectPortal
	//#		\node	RemotePortal
	//#		\node	OcclusionPortal
	//# \end
	//
	//# \node	Zone
	//# \sub
	//#		\node	InfiniteZone
	//#		\node	BoxZone
	//#		\node	CylinderZone
	//#		\node	DomeZone
	//#		\node	PolygonZone
	//# \end
	//
	//# \node	Skybox
	//# \node	Impostor
	//
	//# \node	PhysicsMgr/PhysicsNode
	//# \node	PhysicsMgr/Shape
	//# \sub
	//#		\node	PhysicsMgr/BoxShape
	//#		\node	PhysicsMgr/PyramidShape
	//#		\node	PhysicsMgr/CylinderShape
	//#		\node	PhysicsMgr/ConeShape
	//#		\node	PhysicsMgr/SphereShape
	//#		\node	PhysicsMgr/DomeShape
	//#		\node	PhysicsMgr/CapsuleShape
	//#		\node	PhysicsMgr/TruncatedPyramidShape
	//#		\node	PhysicsMgr/TruncatedConeShape
	//#		\node	PhysicsMgr/TruncatedDomeShape
	//# \end
	//# \node	PhysicsMgr/Joint
	//# \sub
	//#		\node	PhysicsMgr/SphericalJoint
	//#		\node	PhysicsMgr/UniversalJoint
	//#		\node	PhysicsMgr/DiscalJoint
	//#		\node	PhysicsMgr/RevoluteJoint
	//#		\node	PhysicsMgr/CylindricalJoint
	//#		\node	PhysicsMgr/PrismaticJoint
	//# \end
	//# \node	PhysicsMgr/Field
	//# \sub
	//#		\node	PhysicsMgr/BoxField
	//#		\node	PhysicsMgr/CylinderField
	//#		\node	PhysicsMgr/SphereField
	//# \end
	
	
	typedef int32 CollisionState;
	typedef int32 ProximityResult;
	
	
	//# \enum	NodeType
	
	enum
	{
		kNodeGroup			= 0,			//## Group node.
		kNodeCamera			= 'CAMR',		//## Camera node.
		kNodeLight			= 'LITE',		//## Light node.
		kNodeSource			= 'SORC',		//## Sound source node.
		kNodeGeometry		= 'GEOM',		//## Geometry node.
		kNodeInstance		= 'INST',		//## Instanced world node.
		kNodeModel			= 'MODL',		//## Model (character, projectile, etc.) node.
		kNodeBone			= 'BONE',		//## Bone used in skeletal animation node.
		kNodeMarker			= 'MARK',		//## Marker node.
		kNodeTrigger		= 'TRIG',		//## Trigger node.
		kNodeEffect			= 'EFCT',		//## Effect node.
		kNodeEmitter		= 'EMIT',		//## Emitter node.
		kNodeSpace			= 'SPAC',		//## Space node.
		kNodePortal			= 'PORT',		//## Portal node.
		kNodeZone			= 'ZONE',		//## Zone node.
		kNodeSkybox			= 'SKYB',		//## Skybox node.
		kNodeImpostor		= 'IPST',		//## Impostor node.
		kNodePhysics		= 'PHYS',		//## Physics node.
		kNodeShape			= 'SHAP',		//## Physics shape node.
		kNodeJoint			= 'JONT',		//## Physics joint node.
		kNodeField			= 'FELD'		//## Physics field node.
	};
	
	
	//# \enum	NodeFlags
	
	enum
	{
		kNodeNonpersistent			= 1 << 0,		//## The node is skipped during world serialization.
		kNodeDisabled				= 1 << 1,		//## The node is disabled (applies to most node types).
		kNodeCloneInhibit			= 1 << 2,		//## The node is skipped during a cloning operation.
		kNodeAnimateInhibit			= 1 << 3,		//## The node is not animated when attached to a model.
		kNodeUnsharedObject			= 1 << 5,		//## The node's object should be replicated instead of shared when the node is replicated.
		kNodeDirectEnableOnly		= 1 << 6,		//## The node can only be enabled or disabled by applying such an operation directly to the node, and not to a node above it in the scene graph.
		kNodeDynamicVisibility		= 1 << 28,
		kNodeVisibilitySite			= 1 << 29,
		kNodeIsolatedVisibility		= 1 << 30,
		kNodeExternalVisibility		= 1 << 31,
		kNodeFlagsMask				= 0x0000FFFF
	};
	
	
	enum
	{
		kWorldVersion				= 44
	};
	
	
	//# \enum	CollisionKind
	
	enum
	{
		kCollisionRigidBody			= 1 << 0,		//## Any type of rigid body.
		kCollisionCharacter			= 1 << 1,		//## A rigid body that represents a character.
		kCollisionProjectile		= 1 << 2,		//## A rigid body that represents a projectile.
		kCollisionVehicle			= 1 << 3,		//## A rigid body that represents a vehicle.
		
		kCollisionCamera			= 1 << 8,		//## A type of camera.
		kCollisionInteraction		= 1 << 9,
		
		kCollisionSightPath			= 1 << 13,		//## When used in an exclusion mask, does not obstruct sight. 
		kCollisionSoundPath			= 1 << 15,		//## When used in an exclusion mask, does not obstruct sound.
		
		kCollisionBaseKind			= 1 << 16,		//## First application-defined collision kind.
		
		kCollisionExcludeAll		= 0xFFFFFFFF	//## When used as a collision exclusion mask, this value prevents collisions with everything.
	};
	
	
	class Node;
	class World;
	class Controller;
	class Manipulator;
	class Geometry;
	class Marker;
	class Zone;
	class Region;
	class Shape;
	class RigidBodyController;
	
	
	//# \struct	CollisionPoint		Contains basic information about a collision point.
	//
	//# The $CollisionPoint$ structure contains basic information about a collision point.
	//
	//# \def	struct CollisionPoint
	//
	//# \data	CollisionPoint
	//
	//# \also	$@CollisionData@$
	//# \also	$@World::DetectCollision@$
	//# \also	$@World::QueryCollision@$
	
	
	//# \member		CollisionPoint
	
	struct CollisionPoint
	{
		float			param;			//## The fractional distance between the beginning and ending positions where the collision occurred.
		Point3D			position;		//## The world-space point at which the collision occurred.
		Vector3D		normal;			//## The world-space normal at the point of collision.
	};
	
	
	//# \struct	CollisionData		Contains extended information about a collision.
	//
	//# The $CollisionData$ structure contains extended information about a collision.
	//
	//# \def	struct CollisionData : CollisionPoint
	//
	//# \data	CollisionData
	//
	//# \desc
	//# The $CollisionData$ structure is used to return information about a collision when using the
	//# $@World::DetectCollision@$ and $@World::QueryCollision@$ functions.
	//
	//# \base	CollisionPoint		The $CollisionData$ structure extends the $CollisionPoint$ structure.
	//
	//# \also	$@World::DetectCollision@$
	//# \also	$@World::QueryCollision@$
	
	
	//# \member		CollisionData
	
	struct CollisionData : CollisionPoint
	{
		union
		{
			Geometry				*geometry;		//## The geometry node with which the collision occurred. This is valid when the $@World::DetectCollision@$ function returns $true$ or the $@World::QueryCollision@$ function returns $kCollisionStateGeometry$.
			RigidBodyController		*rigidBody;		//## The rigid body with which the collision occurred. This is valid only if the $@World::QueryCollision@$ function was called and it returned $kCollisionStateRigidBody$.
		};
		
		union
		{
			unsigned_int32			triangleIndex;	//## The index of the mesh triangle where the collision occurred. This is valid when the $@World::DetectCollision@$ function returns $true$ or the $@World::QueryCollision@$ function returns $kCollisionStateGeometry$.
			const Shape				*shape;			//## The shape with which the collision occurred. This is valid only if the $@World::QueryCollision@$ function was called and it returned $kCollisionStateRigidBody$.
		};
		
		Zone						*zone;			//## The zone in which the collision occurred. This is only valid if the collision occurred with a geometry, not a rigid body.
	};
	
	
	struct WorldHeader
	{
		int32		endian;
		int32		version;
		int32		controllerCount;
		int32		objectCount;
		int32		nodeCount;
		int32		offsetCount;
	};
	
	void Reverse(WorldHeader *wh);
	
	
	//# \class	Node	The base class for all elements of a scene graph.
	//
	//# Every node that belongs to a scene graph is a subclass of the $Node$ class.
	//
	//# \def	class Node : public Transformable, public UpdatableTree<Node>, public LinkTarget<Node>,
	//# \def2	public Packable, public Configurable, public Constructable<Node>
	//
	//# \ctor	Node(NodeType type = kNodeGroup);
	//
	//# \desc
	//# The $Node$ class provides the base functionality for all members of the scene graph representing a world.
	//# Most nodes are represented by subclasses of the $Node$ class such as $@Geometry@$ or $@Light@$.
	//# When the $Node$ class itself appears in a world, it simply acts as a grouping mechanism and has
	//# the $kNodeGroup$ type.
	//
	//# \base	Utilities/Transformable			Holds the object-to-world transform for a node.
	//# \base	Utilities/UpdatableTree<Node>	Nodes are stored in a hierachical updatable tree.
	//# \base	Utilities/LinkTarget<Node>		Used internally by the World Manager.
	//# \base	ResourceMgr/Packable			Nodes can be packed for storage in resources.
	//# \base	InterfaceMgr/Configurable		Nodes can define configurable parameters that are exposed
	//#											as user interface widgets in the World Editor.
	//# \base	System/Constructable<Object>	New node subclasses may be defined by an application, and a constructor
	//#											function can be installed using the $Constructable$ class.
	//
	//# \also	$@Object@$
	//# \also	$@Controller/Controller@$
	
	
	//# \function	Node::GetNodeType		Returns the type of a node.
	//
	//# \proto	NodeType GetNodeType(void) const;
	//
	//# \desc
	//# The $GetNodeType$ function returns the type of a node. The following table lists the built-in types
	//# that can be returned. Additional types may be defined by the application.
	//
	//# \table	NodeType
	
	
	//# \function	Node::GetNodeFlags		Returns the node flags.
	//
	//# \proto	unsigned_int32 GetNodeFlags(void) const;
	//
	//# \desc
	//# The $GetNodeFlags$ function returns the node flags, which can be a combination (through logical OR) of the following bit flags.
	//
	//# \table	NodeFlags
	//
	//# \also	$@Node::SetNodeFlags@$
	
	
	//# \function	Node::SetNodeFlags		Sets the node flags.
	//
	//# \proto	void SetNodeFlags(unsigned_int32 flags);
	//
	//# \param	flags	The new node flags.
	//
	//# \desc
	//# The $SetNodeFlags$ function sets the node flags to the value given by the $flags$ parameter, which can be
	//# a combination (through logical OR) of the following bit flags.
	//
	//# \table	NodeFlags
	//
	//# \also	$@Node::GetNodeFlags@$
	
	
	//# \function	Node::GetWorld		Returns the world to which a node belongs.
	//
	//# \proto	World *GetWorld(void) const;
	//
	//# \desc
	//# The $GetWorld$ function returns a pointer to the $@World@$ object to which the node belongs.
	//# A node's world pointer is automatically set when a node is added to a tree with the
	//# $@Node::AddNewSubnode@$ function.
	//
	//# \also	$@Node::SetWorld@$
	//# \also	$@Node::AddNewSubnode@$
	//# \also	$@World@$
	
	
	//# \function	Node::SetWorld		Sets the world to which a node belongs.
	//
	//# \proto	void SetWorld(World *world);
	//
	//# \param	world	A pointer to the world that owns the node.
	//
	//# \desc
	//# The $SetWorld$ function sets the $@World@$ object to which a node belongs.
	//# A node's world pointer is automatically set when a node is added to a tree with the
	//# $@Node::AddNewSubnode@$ function.
	//
	//# \also	$@Node::GetWorld@$
	//# \also	$@Node::AddNewSubnode@$
	//# \also	$@World@$
	
	
	//# \div
	//# \function	Node::GetOwningZone		Returns the most immediate zone containing a node.
	//
	//# \proto	Zone *GetOwningZone(void) const;
	//
	//# \desc
	//# The $GetOwningZone$ function returns the most immediate zone containing a node. If the node is not inside
	//# a zone, then the return value is $nullptr$. If the node is a zone itself, then the containing zone is returned,
	//# not the zone for which this function is called.
	//
	//# \also	$@Zone@$
	
	
	//# \function	Node::AddNewSubnode		Adds a newly created subnode to a node.
	//
	//# \proto	void AddNewSubnode(Node *node);
	//
	//# \param	node	A pointer to the subnode to add.
	//
	//# \desc
	//# The $AddNewSubnode$ function add the subnode specified by the $node$ parameter to a node
	//# and subsequently preprocesses the subnode. Calling $AddNewSubnode$ is equivalent to calling
	//# $@Utilities/Tree::AddSubnode@$ for the same node object and then calling $@Node::Preprocess@$
	//# for the node specified by $node$.
	//
	//# \also	$@Utilities/Tree::AddSubnode@$
	//# \also	$@Node::Preprocess@$
	
	
	//# \div
	//# \function	Node::Enable		Enables a node tree.
	//
	//# \proto	void Enable(void);
	//
	//# \desc
	//# The $Enable$ function enables a node and all of its subnodes. Calling the $Enable$ function is
	//# equivalent to clearing the $kNodeDisabled$ flag for each node in the tree rooted at the
	//# node for which it is called.
	//#
	//# If any subnode in the tree has the $kNodeDirectEnableOnly$ flag set, then that subnode is not
	//# affected by the $Enable$ function. The root node for which the $Enable$ function is called is
	//# always enabled regardless of the $kNodeDirectEnableOnly$ flag.
	//
	//# \also	$@Node::Disable@$
	//# \also	$@Node::GetNodeFlags@$
	//# \also	$@Node::SetNodeFlags@$
	
	
	//# \function	Node::Disable		Disables a node tree.
	//
	//# \proto	void Disable(void);
	//
	//# \desc
	//# The $Disable$ function disables a node and all of its subnodes. Calling the $Disable$ function is
	//# equivalent to setting the $kNodeDisabled$ flag for each node in the tree rooted at the
	//# node for which it is called.
	//#
	//# If any subnode in the tree has the $kNodeDirectEnableOnly$ flag set, then that subnode is not
	//# affected by the $Disable$ function. The root node for which the $Disable$ function is called is
	//# always disabled regardless of the $kNodeDirectEnableOnly$ flag.
	//
	//# \also	$@Node::Enable@$
	//# \also	$@Node::GetNodeFlags@$
	//# \also	$@Node::SetNodeFlags@$
	
	
	//# \div
	//# \function	Node::GetObject		Returns a node's object.
	//
	//# \proto	Object *GetObject(void) const;
	//
	//# \desc
	//# The $GetObject$ function returns the primary object referenced by a node. If the node does not reference an
	//# object, then the return value is $nullptr$.
	//# 
	//# Objects hold data that can be shared among multiple nodes for the purposes of instancing. An object's reference
	//# count represents the number of instances of that object in a scene.
	//
	//# \also	$@Node::SetObject@$
	//# \also	$@Object@$
	
	
	//# \function	Node::SetObject		Sets a node's object.
	//
	//# \proto	void SetObject(Object *object);
	//
	//# \param	object		A pointer to the object to which the node should refer. This can be $nullptr$.
	//
	//# \desc
	//# The $SetObject$ function sets the primary object referenced by a node. If the node previously referenced a
	//# different object, then that reference is released, causing the destruction of the old object if its reference
	//# count reaches zero. The reference count of the new object (if $object$ is not $nullptr$) is incremented by one.
	//
	//# \also	$@Node::GetObject@$
	//# \also	$@Object@$
	
	
	//# \function	Node::GetController		Returns a node's controller.
	//
	//# \proto	Controller *GetController(void) const;
	//
	//# \desc
	//# The $GetController$ function returns the $@Controller/Controller@$ assigned to a node. If the node does not
	//# have a controller, then the return value is $nullptr$.
	//
	//# \also	$@Controller/Controller@$
	//# \also	$@Node::SetController@$
	
	
	//# \function	Node::SetController		Sets a node's controller.
	//
	//# \proto	void SetController(Controller *controller);
	//
	//# \param	controller		A pointer to the controller. This can be $nullptr$.
	//
	//# \desc
	//# The $SetController$ function assigns a controller to a node. If the node already has a different controller
	//# assigned to it, then that controller is removed and becomes unassigned, but it is not destroyed.
	//
	//# \also	$@Controller/Controller@$
	//# \also	$@Node::GetController@$
	
	
	//# \div
	//# \function	Node::GetNodeTransform		Returns a node's local transform.
	//
	//# \proto	const Transform4D& GetNodeTransform(void) const;
	//
	//# \desc
	//# The $GetNodeTransform$ function returns a node's local transform. This transform represents the change
	//# in coordinates between a node's local coordinate system (object space) and the coordinate system of its
	//# immediate parent node. To retrieve the transform from object space to world space, call the
	//# $@Utilities/Transformable::GetWorldTransform@$ function.
	//
	//# \also	$@Node::SetNodeTransform@$
	//# \also	$@Node::GetNodePosition@$
	//# \also	$@Utilities/Transformable::GetWorldTransform@$
	//# \also	$@Math/Transform4D@$
	
	
	//# \function	Node::SetNodeTransform		Sets a node's local transform.
	//
	//# \proto	void SetNodeTransform(const Transform4D& transform);
	//# \proto	void SetNodeTransform(const Matrix3D& matrix, const Point3D& position);
	//# \proto	void SetNodeTransform(const Vector3D& c1, const Vector3D& c2, const Vector3D& c3, const Point3D& c4);
	//
	//# \param	transform	The new local transform.
	//# \param	matrix		The new upper-left 3&times;3 portion of the local transform.
	//# \param	position	The new local position.
	//# \param	c1			The first column of the 4D transform.
	//# \param	c2			The second column of the 4D transform.
	//# \param	c3			The third column of the 4D transform.
	//# \param	c4			The fourth column of the 4D transform.
	//
	//# \desc
	//# The $SetNodeTransform$ function sets a node's local transform. This transform represents the change
	//# in coordinates between a node's local coordinate system (object space) and the coordinate system of its
	//# immediate parent node.
	//# 
	//# After the node transform has been set, the world transform returned by the
	//# $@Utilities/Transformable::GetWorldTransform@$ function is not valid until the node is updated. To cause
	//# a node to be updated, the $@Utilities/UpdatableTree::Invalidate@$ function should be called after altering
	//# its transform. The node will then be updated the next time the World Manager processes the scene containing
	//# it or when the $@Utilities/UpdatableTree::Update@$ function is explicitly called for the node.
	//
	//# \special
	//# The $SetNodeTransform$ function should not be called for any node under the control of the physics simulation
	//# through a $@PhysicsMgr/RigidBodyController@$. Instead, the $@PhysicsMgr/RigidBodyController::SetRigidBodyTransform@$
	//# function should be called.
	//
	//# \also	$@Node::GetNodeTransform@$
	//# \also	$@Node::SetNodeMatrix3D@$
	//# \also	$@Node::SetNodePosition@$
	//# \also	$@Utilities/Transformable::GetWorldTransform@$
	//# \also	$@Math/Transform4D@$
	
	
	//# \function	Node::SetNodeMatrix3D		Sets the upper-left 3&times;3 portion of a node's local transform.
	//
	//# \proto	void SetNodeMatrix3D(const Matrix3D& matrix);
	//# \proto	void SetNodeMatrix3D(const Vector3D& c1, const Vector3D& c2, const Vector3D& c3);
	//
	//# \param	matrix		The new upper-left 3&times;3 portion of the local transform.
	//# \param	c1			The first column of the 3&times;3 matrix.
	//# \param	c2			The second column of the 3&times;3 matrix.
	//# \param	c3			The third column of the 3&times;3 matrix.
	//
	//# \desc
	//# The $SetNodeMatrix3D$ function sets the upper-left 3&times;3 portion of a node's local transform without
	//# effecting the translation portion in the fourth column of the matrix. As with the $@Node::SetNodeTransform@$
	//# function, the world transform is not valid until the node is updated.
	//
	//# \special
	//# The $SetNodeMatrix3D$ function should not be called for any node under the control of the physics simulation
	//# through a $@PhysicsMgr/RigidBodyController@$. Instead, the $@PhysicsMgr/RigidBodyController::SetRigidBodyMatrix3D@$
	//# function should be called.
	//
	//# \also	$@Node::SetNodeTransform@$
	//# \also	$@Node::SetNodePosition@$
	
	
	//# \function	Node::GetNodePosition		Returns a node's local position.
	//
	//# \proto	const Point3D& GetNodePosition(void) const;
	//
	//# \desc
	//# The $GetNodePosition$ function returns a node's local position. This position represents the origin of the
	//# node in the coordinate system of the node's immediate parent node. To retrieve the world-space position,
	//# call the $@Utilities/Transformable::GetWorldPosition@$ function.
	//
	//# \also	$@Node::SetNodePosition@$
	//# \also	$@Node::GetNodeTransform@$
	//# \also	$@Utilities/Transformable::GetWorldPosition@$
	//# \also	$@Math/Point3D@$
	
	
	//# \function	Node::SetNodePosition		Sets a node's local position.
	//
	//# \proto	void SetNodePosition(const Point3D& position);
	//
	//# \param	position	The new local position.
	//
	//# \desc
	//# The $SetNodePosition$ function sets a node's local position without affecting the rest of the node's transform.
	//# As with the $@Node::SetNodeTransform@$ function, the world transform is not valid until the node is updated.
	//
	//# \special
	//# The $SetNodePosition$ function should not be called for any node under the control of the physics simulation
	//# through a $@PhysicsMgr/RigidBodyController@$. Instead, the $@PhysicsMgr/RigidBodyController::SetRigidBodyPosition@$
	//# function should be called.
	//
	//# \also	$@Node::GetNodePosition@$
	//# \also	$@Node::SetNodeTransform@$
	//# \also	$@Node::SetNodeMatrix3D@$
	
	
	//# \function	Node::StopMotion		Resets the motion information stored for a node so that motion blur is correctly stopped.
	//
	//# \proto	void StopMotion(void);
	//
	//# \desc
	//# The $StopMotion$ function should be called when a node that has been in motion stops. This function ensures that
	//# the previous transforms used for rendering motion blur are reset so that motion blur does not continue to be
	//# applied to a stopped object. When the $StopMotion$ function is called, it affects the node for which it is called
	//# and its entire tree of subnodes.
	//# 
	//# The $@Utilities/UpdatableTree::Invalidate@$ function should always be called for the same node during the same
	//# frame when the $StopMotion$ function is called. Failing to do this will result in motion blur artifacts.
	
	
	//# \div
	//# \function	Node::GetBoundingSphere		Returns a node's world-space bounding sphere.
	//
	//# \proto	BoundingSphere *const& GetBoundingSphere(void) const;
	//
	//# \desc
	//# The $GetBoundingSphere$ function returns a node's world-space bounding sphere. If the node does not have
	//# a bounding sphere, then the return value is a reference to a location containing $nullptr$.
	//
	//# \also	$@Node::Visible@$
	//# \also	$@Node::Occluded@$
	
	
	//# \function	Node::Visible		Determines whether a node is visible within a given region.
	//
	//# \proto	bool Visible(const Region *region) const;
	//
	//# \param	region		The region for which the node should be tested for visibility.
	//
	//# \desc
	//# The $Visible$ function calls a node's currently installed visibility procedure to determine whether
	//# the node is visible within the region specified by the $region$ parameter. This function is normally
	//# only called from within the World Manager. The return value is $true$ if the node is visible, and
	//# $false$ otherwise.
	//# 
	//# By default, a node's visibility procedure tests the node's bounding sphere against the planes of the
	//# given region. A different visibility procedure can be installed by calling the
	//# $@Node::SetVisibilityProc@$ function.
	//
	//# \also	$@Node::SetVisibilityProc@$
	//# \also	$@Node::Occluded@$
	//# \also	$@Region@$
	
	
	//# \function	Node::Occluded		Determines whether a node is occluded by any occlusion region.
	//
	//# \proto	bool Occluded(const Region *region) const;
	//
	//# \param	region		The first region in a list of occlusion regions for which the node should be tested for occlusion. This cannot be $nullptr$.
	//
	//# \desc
	//# The $Occluded$ function calls a node's currently installed occlusion procedure to determine whether
	//# the node is occluded within any of the regions in the list whose first member is specified by the
	//# $region$ parameter. This function is normally only called from within the World Manager. The return
	//# value is $true$ if the node is occluded, and $false$ otherwise.
	//# 
	//# By default, a node's occlusion procedure tests the node's bounding sphere against the planes of each
	//# region in the list. A different occlusion procedure can be installed by calling the
	//# $@Node::SetOcclusionProc@$ function.
	//
	//# \also	$@Node::SetOcclusionProc@$
	//# \also	$@Node::Visible@$
	//# \also	$@Region@$
	
	
	//# \function	Node::SetVisibilityProc		Sets the function that handles visibility testing for a node.
	//
	//# \proto	void SetVisibilityProc(VisibilityProc *proc);
	//
	//# \param	proc		A pointer to the function that performs the visibility test.
	//
	//# \desc
	//# The $SetVisibilityProc$ function installs the procedure that is called when visibility testing
	//# is needed for a node. The $VisibilityProc$ type is defined as follows.
	//
	//# \code	typedef bool VisibilityProc(const Node *, const Region *);
	//
	//# When the visibility procedure is called, it can use whatever means is appropriate to determine whether
	//# the node is visible within the given region. This is normally accomplished by calling one or more of
	//# the following member functions of the $@Region@$ class.
	//
	//# $@Region::PolygonVisible@$
	//# $@Region::SphereVisible@$
	//# $@Region::EllipsoidVisible@$
	//# $@Region::BoxVisible@$
	//# $@Region::CylinderVisible@$
	//# 
	//# By default, the $Node::SphereVisible$ function is installed as a node's visibility procedure. This function
	//# passes the node's bounding sphere to the $@Region::SphereVisible@$ function to determine whether the node
	//# is visible.
	//# 
	//# The $Node::AlwaysVisible$ function may be installed as the visibility procedure to force a node to be
	//# visible all the time.
	//# 
	//# If a custom visibility procedure is installed using the $SetVisibilityProc$, then a custom occlusion
	//# procedure should also be installed using the $@Node::SetOcclusionProc@$ function if occlusion portals are in use.
	//
	//# \also	$@Node::Visible@$
	//# \also	$@Node::SetOcclusionProc@$
	//# \also	$@Region@$
	
	
	//# \function	Node::SetOcclusionProc		Sets the function that handles occlusion testing for a node.
	//
	//# \proto	void SetOcclusionProc(OcclusionProc *proc);
	//
	//# \param	proc		A pointer to the function that performs the occlusion test.
	//
	//# \desc
	//# The $SetOcclusionProc$ function installs the procedure that is called when occlusion testing
	//# is needed for a node. The $OcclusionProc$ type is defined as follows.
	//
	//# \code	typedef bool OcclusionProc(const Node *, const Region *);
	//
	//# When the occlusion procedure is called, it can use whatever means is appropriate to determine whether
	//# the node is occluded within any of the regions in the list beginning with the region passed in. This is
	//# normally accomplished by calling one or more of the following member functions of the $@Region@$ class.
	//
	//# $@Region::PolygonOccluded@$
	//# $@Region::SphereOccluded@$
	//# $@Region::EllipsoidOccluded@$
	//# $@Region::BoxOccluded@$
	//# $@Region::CylinderOccluded@$
	//# 
	//# By default, the $Node::SphereOccluded$ function is installed as a node's occlusion procedure. This function
	//# passes the node's bounding sphere to the $@Region::SphereOccluded@$ function for each region in the list to
	//# determine whether the node is visible.
	//# 
	//# When the occlusion procedure function is called, the first region, passed as the second parameter, is
	//# guarenteed not to be $nullptr$. The $@Utilities/ListElement::Next@$ function should be used to
	//# iterate over all of the regions in the list.
	//# 
	//# The $Node::NeverOccluded$ function may be installed as the occlusion procedure to force a node to be
	//# unoccluded all the time.
	//# 
	//# If a custom occlusion procedure is installed using the $SetOcclusionProc$, then a custom visibility
	//# procedure should also be installed using the $@Node::SetVisibilityProc@$ function.
	//
	//# \also	$@Node::Occluded@$
	//# \also	$@Node::SetVisibilityProc@$
	//# \also	$@Region@$
	
	
	//# \div
	//# \function	Node::GetHub		Returns the hub attached to a node.
	//
	//# \proto	Hub *GetHub(void) const;
	//
	//# \desc
	//# The $GetHub$ function returns a pointer to the hub attached to a node. A hub exists for a node whenever the node has
	//# any outgoing or incoming connector. If a node has no hub, then the $GetHub$ function returns $nullptr$.
	//#
	//# To iterate over the connectors for a node, the member functions of the $@Utilities/GraphElement@$ base
	//# class can be used. For example, to iterate over all outgoing connectors, call the $@Utilities/GraphElement::GetFirstOutgoingEdge@$
	//# function to retrieve the first connector, and then call $@Utilities/GraphEdge::GetNextOutgoingEdge@$ function for the connector
	//# until $nullptr$ is returned.
	//
	//# \also	$@Hub@$
	//# \also	$@Connector@$
	//# \also	$@Node::AddConnector@$
	//# \also	$@Node::RemoveConnector@$
	//# \also	$@Node::GetConnectedNode@$
	//# \also	$@Node::SetConnectedNode@$
	
	
	//# \function	Node::AddConnector		Adds a node connection.
	//
	//# \proto	Connector *AddConnector(const char *key, Node *node = nullptr);
	//
	//# \param	key		The key value for the connector. This is a string up to 15 bytes in length, not counting the null terminator.
	//# \param	node	The initial target node for the connector.
	//
	//# \desc
	//# The $AddConnector$ function attaches a new $@Connector@$ object to a node. The $key$ parameter specifies a
	//# unique identifier for the connector that is normally used to assign some kind of meaning to the node that
	//# it connects to. The key value is used by the $@Node::GetConnectedNode@$ function to retrieve the node that
	//# is connected through a particular connector. The key value should be unique among all connectors attached to
	//# the same node. If a node has two or more connectors with the same key, then it is undefined which connector
	//# will be returned by searches for a connector with that key.
	//#
	//# If necessary, a hub is created for the node for which the $AddConnector$ function is called. The new connector becomes
	//# the last outgoing edge for the node's hub. If the $node$ parameter is not $nullptr$, then a hub is also created for
	//# the target node, if necessary, and the new connector becomes the last incoming edge for the target node's hub.
	//#
	//# If the $node$ parameter is $nullptr$, then the new connector's start and finish nodes are both set to the hub attached
	//# to the node for which the $AddConnector$ function is called.
	//
	//# \also	$@Hub@$
	//# \also	$@Connector@$
	//# \also	$@Node::GetHub@$
	//# \also	$@Node::RemoveConnector@$
	//# \also	$@Node::GetConnectedNode@$
	//# \also	$@Node::SetConnectedNode@$
	
	
	//# \function	Node::RemoveConnector	Removes a node connector.
	//
	//# \proto	bool RemoveConnector(const char *key);
	//
	//# \param	key		The key value for the connector. This is a string up to 15 bytes in length, not counting the null terminator.
	//
	//# \desc
	//# The $RemoveConnector$ function removes an existing $@Connector@$ object from a node. The $key$ parameter specifies the
	//# unique identifier for the connector that is to be removed. If a connector with this key exists, then it is deleted,
	//# and the $RemoveConnector$ function returns $true$. The no such connector exists, then the $RemoveConnector$ function
	//# performs no action and returns $false$.
	//# 
	//# If the node's hub has no connectors remaining after the connector specified by the $key$ parameter is deleted, then
	//# the hub is also deleted.
	//
	//# \also	$@Hub@$
	//# \also	$@Connector@$
	//# \also	$@Node::GetHub@$
	//# \also	$@Node::RemoveConnector@$
	//# \also	$@Node::GetConnectedNode@$
	//# \also	$@Node::SetConnectedNode@$
	
	
	//# \function	Node::GetConnectedNode		Returns the connected node with a particular key.
	//
	//# \proto	Node *GetConnectedNode(const char *key) const;
	//
	//# \param	key		The key value of the connector.
	//
	//# \desc
	//# The $GetConnectedNode$ function searches for a connector having a key matching the $key$ parameter and,
	//# if such a connector is found, returns the node to which it connects. If there is no connector with the
	//# matching key, or the connector exists but is not connected to another node, then the return value is $nullptr$.
	//
	//# \also	$@Node::SetConnectedNode@$
	//# \also	$@Node::AddConnector@$
	//# \also	$@Node::RemoveConnector@$
	//# \also	$@Node::GetHub@$
	
	
	//# \function	Node::SetConnectedNode		Sets the connected node with a particular key.
	//
	//# \proto	bool SetConnectedNode(const char *key, Node *node) const;
	//
	//# \param	key		The key value of the connector.
	//# \param	node	The node to which the connector should be linked. This may be $nullptr$.
	//
	//# \desc
	//# The $SetConnectedNode$ function searches for a connector having a key matching the $key$ parameter and,
	//# if such a connector is found, connects it to the node specified by the $node$ parameter and returns $true$.
	//# If there is no connector with the matching key, then this function returns $false$.
	//#
	//# If the connector exists, and the $node$ parameter is $nullptr$, then the target of the connector is set
	//# to the node for which the $SetConnectedNode$ function is called. That is, unconnected connectors loop
	//# back to their starting points.
	//
	//# \also	$@Node::GetConnectedNode@$
	//# \also	$@Node::GetFirstConnector@$
	//# \also	$@Node::AddConnector@$
	//# \also	$@Node::RemoveConnector@$
	
	
	//# \div
	//# \function	Node::GetProperty		Returns the property of a given type that is attached to a node.
	//
	//# \proto	Property *GetProperty(PropertyType type) const;
	//
	//# \param	type	The property type.
	//
	//# \desc
	//# The $GetProperty$ function returns the property attached to a node having the type specified
	//# by the $type$ parameter. If no such property exists, then the return value is $nullptr$.
	//
	//# \also	$@Node::GetFirstProperty@$
	//# \also	$@Node::AddProperty@$
	//# \also	$@Property@$
	
	
	//# \function	Node::GetFirstProperty		Returns the first property directly attached to a node.
	//
	//# \proto	Property *GetFirstProperty(void) const;
	//
	//# \desc
	//# The $GetFirstProperty$ function returns the first property directly attached to a node. All of the
	//# properties directly attached to a node can be iterated by repeatedly calling the $@Utilities/ListElement::Next@$
	//# function on the returned pointer. If no properties are directly attached to a node, then the return
	//# value is $nullptr$.
	//
	//# \also	$@Node::GetProperty@$
	//# \also	$@Node::AddProperty@$
	//# \also	$@Property@$
	
	
	//# \function	Node::AddProperty		Attaches a property directly to a node.
	//
	//# \proto	void AddProperty(Property *property);
	//
	//# \param	property	The property to attach.
	//
	//# \desc
	//# The $AddProperty$ function attaches the property specified by the $property$ parameter directly to a node.
	//# A property can be attached to only one node at a time, so the property is removed from any other node to
	//# which it may have previously been attached.
	//
	//# \also	$@Node::GetFirstProperty@$
	//# \also	$@Node::GetProperty@$
	//# \also	$@Property@$
	
	
	//# \function	Node::GetPropertyObject		Returns the property object attached to a node.
	//
	//# \proto	PropertyObject *GetPropertyObject(void) const;
	//
	//# \desc
	//# The $GetPropertyObject$ function returns a pointer to the property object attached to a node.
	//# If there is no property object attached to a node, then the return value is $nullptr$.
	//# A node does not have a property object by default.
	//
	//# \also	$@Node::SetPropertyObject@$
	//# \also	$@Node::GetSharedProperty@$
	//# \also	$@PropertyObject@$
	
	
	//# \function	Node::SetPropertyObject		Attaches a property object to a node.
	//
	//# \proto	void SetPropertyObject(PropertyObject *object);
	//
	//# \param	object		The property object to attach.
	//
	//# \desc
	//# The $SetPropertyObject$ function attaches the property object specified by the $object$ parameter to a node.
	//# If $object$ is $nullptr$, then the node does not have a property object after this function is called.
	//# Otherwise, the reference count of the property object is incremented, and the new property is attached to the node.
	//# The reference count of any property object previously attached to the node is decremented, and the old property
	//# object is deleted if its reference count reaches zero.
	//
	//# \also	$@Node::SetPropertyObject@$
	//# \also	$@Node::GetSharedProperty@$
	//# \also	$@PropertyObject@$
	
	
	//# \function	Node::GetSharedProperty		Returns the shared property of a given type that is stored in a node's property object.
	//
	//# \proto	Property *GetSharedProperty(PropertyType type) const;
	//
	//# \param	type	The property type.
	//
	//# \desc
	//# The $GetSharedProperty$ function returns the property stored in a node's property object having the type specified
	//# by the $type$ parameter. If no such property exists or there is no property object attached to the node, then the
	//# return value is $nullptr$.
	//
	//# \also	$@Node::GetPropertyObject@$
	//# \also	$@Node::SetPropertyObject@$
	//# \also	$@PropertyObject@$
	
	
	//# \function	Node::GetNodeName		Returns the name of a node.
	//
	//# \proto	const char *GetNodeName(void) const;
	//
	//# \desc
	//# The $GetNodeName$ function returns a pointer to the name of a node. If the node does not have a name,
	//# then the return value is $nullptr$. (The name itself is stored in a property attached to the node.)
	//
	//# \also	$@Node::SetNodeName@$
	
	
	//# \function	Node::SetNodeName		Sets the name of a node.
	//
	//# \proto	void SetNodeName(const char *name);
	//
	//# \param	name	The new node name. This cannot be $nullptr$.
	//
	//# \desc
	//# The $SetNodeName$ function sets the name of a node to the string specified by the $name$ parameter.
	//# If the node did not previously have a name, then a new property is created in which to store the name,
	//# and that property is attached to the node. There is no practical limit to the length of a node name.
	//#
	//# To remove the name from a node, use the $@Node::GetProperty@$ function to get the property having type
	//# $kPropertyName$, and delete it. Do not call $SetNodeName$ with an empty string or $nullptr$.
	//
	//# \also	$@Node::GetNodeName@$
	//# \also	$@Node::GetProperty@$
	
	
	//# \div
	//# \function	Node::Clone		Clones a node hierarchy.
	//
	//# \proto	Node *Clone(void) const;
	//
	//# \desc
	//# The $Clone$ function duplicates a node hierarchy rooted at the node for which this function is called and
	//# returns the root of the duplicate node tree. The objects referenced by the nodes in the tree are not duplicated,
	//# but are referenced by the duplicate nodes.
	
	
	//# \div
	//# \function	Node::Preprocess		Performs any preprocessing that a node needs to do before being used in a world.
	//
	//# \proto	virtual void Preprocess(void);
	//
	//# \desc
	//# The $Preprocess$ function performs any preprocessing that a node needs to do before being used in a world.
	//# Whenever a node is added to a scene, it should subsequently be preprocessed. Calling the $@Node::AddNewSubnode@$
	//# function to add a node to a scene is equivalent to calling $@Utilities/Tree::AddSubnode@$ and following it with
	//# a call to $Preprocess$.
	//# 
	//# Whenever a subclass implements an override for the $Preprocess$ function, it should always call the
	//# $Preprocess$ function of its direct base class first.
	//
	//# \also	$@Node::AddNewSubnode@$
	
	
	//# \function	Node::EnterZone		Called when a node enters a zone.
	//
	//# \proto	virtual void EnterZone(Zone *zone);
	//
	//# \param	zone	The zone being entered.
	//
	//# \desc
	//# The $EnterZone$ function is called by the engine when a node is placed into a new zone. Unless the node is
	//# initially being added to the scene, a call to this function will be preceded by a call to the $@Node::ExitZone@$
	//# function for the zone that the node was previously in.
	//
	//# \also	$@Node::ExitZone@$
	//# \also	$@Zone@$
	
	
	//# \function	Node::ExitZone		Called when a node exits a zone.
	//
	//# \proto	virtual void ExitZone(Zone *zone);
	//
	//# \param	zone	The zone being exited.
	//
	//# \desc
	//# The $ExitZone$ function is called by the engine when a node is removed from a zone. Unless the node is being
	//# destroyed, a call to this function will be followed by a call to the $@Node::EnterZone@$ function for the
	//# new zone that the node is being placed in.
	//
	//# \also	$@Node::EnterZone@$
	//# \also	$@Zone@$

	
	class C4_API Node : public Transformable, public UpdatableTree<Node>, public Site, public LinkTarget<Node>, public Packable, public Configurable, public Constructable<Node>
	{
		friend class Hub;
		
		public:
			
			typedef bool CloneFilterProc(const Node *, void *);
			
			enum
			{
				kUpdateTransform		= 1 << 0,
				kUpdatePostTransform	= 1 << 1,
				kUpdateBoundingSphere	= 1 << 2,
				kUpdateVisibility		= 1 << 3,
				kUpdatePostBounding		= 1 << 4,
				
				kInitialUpdateFlags		= kUpdateTransform | kUpdateBoundingSphere,
				kPropagatedUpdateFlags	= kUpdateBoundingSphere
			};
			
		private:
			
			typedef bool VisibilityProc(const Node *, const Region *);
			typedef bool OcclusionProc(const Node *, const Region *);
			
			struct ConnectorCloneData
			{
				Connector		*connector;
				int32			linkIndex;
			};
			
			NodeType				nodeType;
			unsigned_int32			nodeFlags;
			unsigned_int32			nodeHash;
			unsigned_int32			nodeStamp;
			
			World					*nodeWorld;
			Manipulator				*nodeManipulator;
			Controller				*nodeController;
			Object					*nodeObject;
			Hub						*nodeHub;
			
			Map<Property>			propertyMap;
			PropertyObject			*propertyObject;
			
			Transform4D				nodeTransform;
			Transform4D				previousWorldTransform;
			
			BoundingSphere			boundingSphere;
			BoundingSphere			*boundingSpherePointer;
			
			VisibilityProc			*visibilityProc;
			OcclusionProc			*occlusionProc;
			
			union
			{
				mutable int32		nodeIndex;
				mutable int32		superIndex;
			};
			
			int32					objectIndex;
			
			Node *CloneNode(CloneFilterProc *filterProc = &DefaultCloneFilter, void *filterCookie = nullptr) const;
			Node *CloneNode(const Node *root, Node **nodeTable, Array<ConnectorCloneData> *connectorArray, CloneFilterProc *filterProc = &DefaultCloneFilter, void *filterCookie = nullptr) const;
			
			static void ConnectorLinkProc(Node *node, void *cookie);
			static void PropertyObjectLinkProc(Object *object, void *cookie);
			
			void PrepackNodeObjects(List<Object> *linkList) const;
			static Object **LoadOriginalObjects(const ResourceName& name, World *previousWorld, int32 newObjectCount, int32 *originalObjectCount, int32 *totalObjectCount);
			static Node *LoadNodeTable(Unpacker& unpacker, unsigned_int32 unpackFlags, int32 nodeCount, int32 objectCount, Object **objectTable);
		
		protected:
			
			Node(const Node& node);
			
			#if C4LEGACY
			
				void SetNodeHash(unsigned_int32 hash)
				{
					nodeHash = hash;
				}
			
			#endif
			
			void SetNewObject(Object *object)
			{
				nodeObject = object;
			}
			
			void SetBoundingSphere(const Point3D& center, float radius)
			{
				boundingSphere.SetCenter(center);
				boundingSphere.SetRadius(radius);
				boundingSpherePointer = &boundingSphere;
			}
			
			virtual void CalculateWorldTransform(void);
			virtual void CalculatePostTransform(void);
			virtual void CalculateVisibility(void);
			virtual void CalculatePostBounding(void);
		
		public:
			
			Node(NodeType type = kNodeGroup);
			virtual ~Node();
			
			using UpdatableTree<Node>::Previous;
			using UpdatableTree<Node>::Next;
			using UpdatableTree<Node>::GetPreviousNode;
			using UpdatableTree<Node>::GetNextNode;
			using UpdatableTree<Node>::Detach;
			
			static Node *Construct(Unpacker& data, unsigned_int32 unpackFlags);
			
			NodeType GetNodeType(void) const
			{
				return (nodeType);
			}
			
			unsigned_int32 GetNodeFlags(void) const
			{
				return (nodeFlags);
			}
			
			void SetNodeFlags(unsigned_int32 flags)
			{
				nodeFlags = flags;
			}
			
			bool Enabled(void) const
			{
				return ((nodeFlags & kNodeDisabled) == 0);
			}
			
			unsigned_int32 GetNodeHash(void) const
			{
				return (nodeHash);
			}
			
			unsigned_int32 GetNodeStamp(void) const
			{
				return (nodeStamp);
			}
			
			void SetNodeStamp(unsigned_int32 stamp)
			{
				nodeStamp = stamp;
			}
			
			World *GetWorld(void) const
			{
				return (nodeWorld);
			}
			
			void SetWorld(World *world)
			{
				nodeWorld = world;
			}
			
			void AddNewSubnode(Node *node)
			{
				AddSubnode(node);
				node->Preprocess();
			}
			
			Manipulator *GetManipulator(void) const
			{
				return (nodeManipulator);
			}
			
			void SetManipulator(Manipulator *manipulator)
			{
				nodeManipulator = manipulator;
			}
			
			Controller *GetController(void) const
			{
				return (nodeController);
			}
			
			Object *GetObject(void) const
			{
				return (nodeObject);
			}
			
			Hub *GetHub(void) const
			{
				return (nodeHub);
			}
			
			int32 GetPropertyCount(void) const
			{
				return (propertyMap.GetElementCount());
			}
			
			Property *GetProperty(PropertyType type) const
			{
				return (propertyMap.Find(type));
			}
			
			Property *GetFirstProperty(void) const
			{
				return (propertyMap.First());
			}
			
			void AddProperty(Property *property)
			{
				propertyMap.Insert(property);
			}
			
			PropertyObject *GetPropertyObject(void) const
			{
				return (propertyObject);
			}
			
			Property *GetSharedProperty(PropertyType type) const
			{
				return ((propertyObject) ? propertyObject->GetProperty(type) : nullptr);
			}
			
			const Transform4D& GetNodeTransform(void) const
			{
				return (nodeTransform);
			}
			
			void SetNodeTransform(const Transform4D& transform)
			{
				nodeTransform = transform;
			}
			
			void SetNodeTransform(const Matrix3D& matrix, const Point3D& position)
			{
				nodeTransform.Set(matrix, position);
			}
			
			void SetNodeTransform(const Vector3D& c1, const Vector3D& c2, const Vector3D& c3, const Point3D& c4)
			{
				nodeTransform.Set(c1, c2, c3, c4);
			}
			
			void SetNodeMatrix3D(const Matrix3D& matrix)
			{
				nodeTransform.SetMatrix3D(matrix);
			}
			
			void SetNodeMatrix3D(const Transform4D& transform)
			{
				nodeTransform.SetMatrix3D(transform);
			}
			
			void SetNodeMatrix3D(const Vector3D& c1, const Vector3D& c2, const Vector3D& c3)
			{
				nodeTransform.SetMatrix3D(c1, c2, c3);
			}
			
			const Point3D& GetNodePosition(void) const
			{
				return (nodeTransform.GetTranslation());
			}
			
			void SetNodePosition(const Point3D& position)
			{
				nodeTransform.SetTranslation(position);
			}
			
			const Transform4D& GetPreviousWorldTransform(void) const
			{
				return (previousWorldTransform);
			}
			
			void SetPreviousWorldTransform(const Transform4D& transform)
			{
				previousWorldTransform = transform;
			}
			
			BoundingSphere *const& GetBoundingSphere(void) const
			{
				return (boundingSpherePointer);
			}
			
			const Point3D *GetBoundingSphereCenterPointer(void) const
			{
				return (&boundingSphere.GetCenter());
			}
			
			void SetVisibilityProc(VisibilityProc *proc)
			{
				visibilityProc = proc;
			}
			
			void SetOcclusionProc(OcclusionProc *proc)
			{
				occlusionProc = proc;
			}
			
			bool Visible(const Region *region) const
			{
				return ((*visibilityProc)(this, region));
			}
			
			bool Occluded(const Region *region) const
			{
				return ((*occlusionProc)(this, region));
			}
			
			int32 GetNodeIndex(void) const
			{
				return (nodeIndex);
			}
			
			void InvalidateNodeIndex(void)
			{
				nodeIndex = -1;
			}
			
			virtual Node *Replicate(void) const;
			
			static bool DefaultCloneFilter(const Node *node, void *cookie = nullptr);
			Node *Clone(CloneFilterProc *filterProc = &DefaultCloneFilter, void *filterCookie = nullptr) const;
			void CloneSubtree(Node *root) const;
			
			bool LinkedNodePackable(unsigned_int32 packFlags) const;
			
			void PackType(Packer& data) const;
			void Prepack(List<Object> *linkList) const;
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			void *BeginSettingsUnpack(void);
			void EndSettingsUnpack(void *cookie);
			
			int32 GetCategoryCount(void) const;
			Type GetCategoryType(int32 index, const char **title) const;
			int32 GetCategorySettingCount(Type category) const;
			Setting *GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const;
			void SetCategorySetting(Type category, const Setting *setting);
			
			void Invalidate(void);
			void StopMotion(void);
			void Update(void);
			
			void InitTransform(void);
			void UpdateTransform(void);
			void UpdatePostTransform(void);
			void UpdateBoundingSphere(void);
			void UpdateVisibility(void);
			void UpdatePostBounding(void);
			
			void Enable(void);
			void Disable(void);
			
			void SetPersistent(void);
			void SetNonpersistent(void);
			
			void SetObject(Object *object);
			void SetController(Controller *controller);
			void SetPropertyObject(PropertyObject *object);
			
			Node *GetConnectedNode(const char *key) const;
			bool SetConnectedNode(const char *key, Node *node) const;
			void AddConnector(const char *key, Node *node = nullptr);
			bool RemoveConnector(const char *key);
			
			virtual int32 GetInternalConnectorCount(void) const;
			virtual const char *GetInternalConnectorKey(int32 index) const;
			virtual void ProcessInternalConnectors(void);
			virtual bool ValidConnectedNode(const ConnectorKey& key, const Node *node) const;
			
			const char *GetNodeName(void) const;
			void SetNodeName(const char *name);
			
			void BondVisibility(void);
			void BreakVisibility(void);
			void TransferVisibility(void);
			
			virtual void Preprocess(void);
			virtual void Neutralize(void);
			virtual void ProcessObjectSettings(void);
			
			virtual void EnterZone(Zone *zone);
			virtual void ExitZone(Zone *zone);
			
			Zone *GetOwningZone(void) const;
			
			virtual bool CalculateBoundingBox(Box3D *box) const;
			virtual bool CalculateBoundingSphere(BoundingSphere *sphere) const;
			
			static bool AlwaysVisible(const Node *node, const Region *region);
			static bool NeverOccluded(const Node *node, const Region *region);
			
			static bool BoxVisible(const Node *node, const Region *region);
			static bool BoxOccluded(const Node *node, const Region *region);
			static bool SphereVisible(const Node *node, const Region *region);
			static bool SphereOccluded(const Node *node, const Region *region);
			
			FileResult PackTree(File *file, unsigned_int32 packFlags = 0) const;
			void PackTree(Package *package, unsigned_int32 packFlags = 0) const;
			static Node *UnpackTree(const void *data, unsigned_int32 unpackFlags = 0);
			
			FileResult PackDeltaTree(File *file, const ResourceName& originalName) const;
			static Node *UnpackDeltaTree(const void *data, ResourceName& originalName, World *previousWorld = nullptr);
	};
	
	
	//# \class	RenderableNode		The base class for renderable scene graph nodes.
	//
	//# Every directly-renderable node in a scene graph is a subclass of the $RenderableNode$ class.
	//
	//# \def	class RenderableNode : public Node, public Renderable
	//
	//# \ctor	RenderableNode(NodeType type, RenderType renderType, unsigned_int32 renderState = 0);
	//
	//# \param	type			The node type passed to the $Node$ base class.
	//# \param	renderType		The render type for the $Renderable$ base class.
	//# \param	renderState		The render state for the $Renderable$ base class.
	//
	//# \desc
	//# The $RenderableNode$ class serves as the base class for scene graph nodes that can be directly rendered,
	//# such as geometries and effects.
	//
	//# \base	Node						A $RenderableNode$ is a special type of node.
	//# \base	GraphicsMgr/Renderable		Holds rendering information for the node.
	//
	//# \also	$@Geometry@$
	//# \also	$@Effect@$
	
	
	class C4_API RenderableNode : public Node, public Renderable
	{
		protected:
			
			void CalculatePostTransform(void) override;
			
			RenderableNode(NodeType type, RenderType renderType, unsigned_int32 renderState = 0);
			RenderableNode(const RenderableNode& renderableNode);
		
		public:
			
			~RenderableNode();
			
			void Neutralize(void);
	};
}


#endif

// ZYURVUR
