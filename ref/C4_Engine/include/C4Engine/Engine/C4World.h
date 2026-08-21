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


#ifndef C4World_h
#define C4World_h


//# \component	World Manager
//# \prefix		WorldMgr/

//# \import		C4Physics.h


#include "C4Shadows.h"
#include "C4Display.h"
#include "C4Graphics.h"
#include "C4Impostors.h"
#include "C4Zones.h"


namespace C4
{
	typedef EngineResult WorldResult;
	
	
	enum
	{
		kWorldOkay			= kEngineOkay,
		kWorldLoadFailed	= (kManagerWorld << 16) | 0x0001
	};
	
	
	enum
	{
		kWorldPaused					= 1 << 0,
		kWorldViewport					= 1 << 1,
		kWorldClearColor				= 1 << 2,
		kWorldAmbientOnly				= 1 << 3,
		kWorldMotionBlurInhibit			= 1 << 4,
		kWorldZeroBackgroundVelocity	= 1 << 5,
		kWorldPostColorMatrix			= 1 << 6,
		kWorldListenerInhibit			= 1 << 7,
		kWorldRestore					= 1 << 8
	};
	
	
	//# \enum	PerspectiveMask
	
	enum
	{
		kPerspectiveDirect				= 1 << 0,		//## The perspective of the primary camera used to render the world.
		kPerspectiveReflection			= 1 << 1,		//## The perspective of a camera used to render a reflection buffer.
		kPerspectiveRefraction			= 1 << 2,		//## The perspective of a camera used to render a refraction buffer.
		kPerspectiveCameraWidget		= 1 << 3,		//## The perspective of a camera used to render an image for a camera widget in a panel.
		kPerspectiveRemotePortal		= 1 << 4,		//## The perspective of a camera used to render through a remote portal, but into the primary buffer.
		kPerspectiveAmbientSpace		= 1 << 8,		//## The perspective of a camera used to generate an ambient space texture map.
		kShadowPerspectiveDirect		= kPerspectiveDirect << 16,
		kShadowPerspectiveReflection	= kPerspectiveReflection << 16,
		kShadowPerspectiveRefraction	= kPerspectiveRefraction << 16,
		kShadowPerspectiveCameraWidget	= kPerspectiveCameraWidget << 16,
		kShadowPerspectiveRemotePortal	= kPerspectiveRemotePortal << 16
	};
	
	
	enum
	{
		kRenderStageDefault,
		kRenderStageAlphaTest,
		kRenderStageImpostor,
		kRenderStageCover,
		kRenderStageDecal,
		kRenderStageEffectLight,
		kRenderStageEffectOpaque,
		kRenderStageEffectTransparent,
		kRenderStageEffectFrontmost,
		kRenderStageEffectOcclusion,
		kRenderStageEffectDistortion,
		kRenderStageEffectVelocity,
		kRenderStageEffectCover,
		kRenderStageCount,
		
		kRenderStageFirstEffect = kRenderStageEffectLight,
		kRenderStageLastEffect = kRenderStageEffectCover
	};
	
	
	//# \enum	CollisionState
	
	enum
	{
		kCollisionStateNone			= 0,		//## No collision occurred.
		kCollisionStateGeometry		= 1,		//## Collision occurred with world geometry.
		kCollisionStateRigidBody	= 2			//## Collision occurred with a rigid body.
	};
	
	
	//# \enum	ProximityResult
	
	enum
	{
		kProximityContinue			= 0,		//## Continue visting nodes normally.
		kProximitySkipSuccessors	= 1,		//## Do not visit any successors of the current node.
		kProximityStop				= 2			//## Stop the proximity query at the current node.
	};
	 
	
	enum 
	{ 
		kWorldMaxInstanceDepth		= 3 
	};
	 
	
	enum
	{
		kWorldCounterLight, 
		kWorldCounterGeometry,
		kWorldCounterTerrain,
		kWorldCounterWater,
		kWorldCounterImpostor, 
		kWorldCounterDepthShadow,
		kWorldCounterStencilShadow,
		kWorldCounterShadowSection,
		kWorldCounterDirectPortal,
		kWorldCounterRemotePortal,
		kWorldCounterOcclusionRegion,
		kWorldCounterPlayingSource,
		kWorldCounterEngagedSource,
		kWorldCounterRunningScript,
		kWorldCounterClothMove,
		kWorldCounterClothUpdate,
		kWorldCounterWaterMove,
		kWorldCounterWaterUpdate,
		kWorldCounterCount,
		kWorldCounterRenderCount = kWorldCounterPlayingSource
	};
	
	
	#if C4DIAGNOSTICS
	
		enum
		{
			kDiagnosticLightRegions		= 1 << 0,
			kDiagnosticShadowRegions	= 1 << 1,
			kDiagnosticSourcePaths		= 1 << 2,
			kDiagnosticRigidBodies		= 1 << 3,
			kDiagnosticContacts			= 1 << 4
		};
	
	#endif
	
	
	class Skybox;
	class OrthoCamera;
	class FrustumCamera;
	class TerrainLevelGeometry;
	class PanelEffect;
	class PortalData;
	class QueryThreadData;
	struct WorldContext;
	struct CollisionParams;
	struct ProximityParams;
	struct InteractionData;
	struct ControllerData;
	struct ShadowRenderData;
	
	
	class WorldResource : public Resource<WorldResource>
	{
		friend class Resource<WorldResource>;
		
		private:
			
			static C4API ResourceDescriptor		descriptor;
			
			~WorldResource();
		
		public:
			
			C4API WorldResource(const char *name, ResourceCatalog *catalog);
			
			int32 GetControllerCount(void) const;
			
			ResourceResult LoadObjectOffsetTable(ResourceLoader *loader, WorldHeader *worldHeader, int32 **offsetTable) const;
			ResourceResult LoadAllObjects(ResourceLoader *loader, const WorldHeader *header, const int32 *offsetTable, char **objectData) const;
			ResourceResult LoadObject(ResourceLoader *loader, int32 index, const int32 *offsetTable, char **objectData) const;
	};
	
	
	class SaveResource : public Resource<SaveResource>
	{
		friend class Resource<SaveResource>;
		
		private:
			
			static C4API ResourceDescriptor		descriptor;
			
			~SaveResource();
		
		public:
			
			C4API SaveResource(const char *name, ResourceCatalog *catalog);
			
			int32 GetControllerCount(void) const;
	};
	
	
	class WorldObservable : public Observable<WorldObservable>
	{
		private:
			
			World		*observableWorld;

		public:
			
			WorldObservable(World *world)
			{
				observableWorld = world;
			}

			World *GetWorld(void) const
			{
				return (observableWorld);
			}
	};
	
	
	class InstancedWorldData : public MapElement<InstancedWorldData>
	{
		private:
			
			unsigned_int32	worldHash;
			Node			*prototypeCopy;
		
		public:
			
			typedef unsigned_int32 KeyType;
			
			InstancedWorldData(unsigned_int32 hash, Node *node);
			~InstancedWorldData();
			
			KeyType GetKey(void) const
			{
				return (worldHash);
			}
			
			Node *GetPrototypeCopy(void) const
			{
				return (prototypeCopy);
			}
	};
	
	
	class GenericModelData : public MapElement<GenericModelData>
	{
		private:
			
			unsigned_int32			modelHash;
			List<GenericModel>		modelList;
		
		public:
			
			typedef unsigned_int32 KeyType;
			
			GenericModelData(unsigned_int32 hash, GenericModel *model);
			~GenericModelData();
			
			KeyType GetKey(void) const
			{
				return (modelHash);
			}
			
			GenericModel *GetGenericModel(void) const
			{
				return (modelList.First());
			}
			
			void AddGenericModel(GenericModel *model)
			{
				modelList.Append(model);
			}
	};
	
	
	//# \class	Interactor		Handles interaction events.
	//
	//# The $Interactor$ class handles interaction events.
	//
	//# \def	class Interactor : public ListElement<Interactor>
	//
	//# \ctor	Interactor();
	//
	//# \desc
	//#
	//
	//# \base	Utilities/ListElement<Interactor>	Used internally by the World Manager.
	//
	//# \also	$@World::AddInteractor@$
	//# \also	$@World::RemoveInteractor@$
	//# \also	$@World::Interact@$
	
	
	//# \function	Interactor::GetInteractionNode		Returns the node currently engaged in interaction.
	//
	//# \proto	Node *GetInteractionNode(void) const;
	//
	//# \desc
	//# The $GetInteractionNode$ function returns a pointer to the node that is currently engaged in interaction
	//# by a particular instance of the $@Interactor@$ class. If no node is currently engaged in interaction,
	//# then this function returns $nullptr$.
	//
	//# \also	$@Interactor::SetInteractionProbe@$
	//# \also	$@Interactor::HandleInteractionEvent@$
	
	
	//# \function	Interactor::SetInteractionProbe		Sets the line segment representing the interaction probe.
	//
	//# \proto	void SetInteractionProbe(const Point3D& p1, const Point3D& p2);
	//
	//# \param	p1		The beginning of the line segment.
	//# \param	p2		The end of the line segment.
	//
	//# \desc
	//# The $SetInteractionProbe$ sets the world-space endpoint coordinates of the probe used to test for
	//# interactive objects. This function is typically called from within a function overriding the
	//# $@World::Interact@$ function before the base class $Interact$ function is called.
	//# 
	//# When the interaction probe intersects interactive objects in a world, the $@Interactor::HandleInteractionEvent@$
	//# function is called to handle various interaction events.
	//
	//# \also	$@Interactor::GetInteractionNode@$
	//# \also	$@Interactor::HandleInteractionEvent@$
	
	
	//# \function	Interactor::HandleInteractionEvent		Called to handle an interaction event.
	//
	//# \proto	virtual void HandleInteractionEvent(InteractionEventType type, Node *node, const Point3D *position = nullptr);
	//
	//# \param	type		The type of event. See the $@Controller/Controller::HandleInteractionEvent@$ for a list of possible types.
	//# \param	node		The interactive node to which the event pertains.
	//# \param	position	The object-space position on the interactive node at which the event took place. If the $type$
	//#						parameter is $kInteractionEventDisengage$, then this parameter is $nullptr$.
	//
	//# \desc
	//# The $HandleInteractionEvent$ function is called by the World Manager when an interaction event takes place
	//# for a particular instance of the $@Interactor@$ class. If this function is overridden, it must call the base
	//# class counterpart first. An overriding function will typical cause the $@Controller/Controller::HandleInteractionEvent@$
	//# function to be called for the controller attached to the node specified by the $node$ parameter.
	//
	//# The $HandleInteractionEvent$ function only receives the $kInteractionEventEngage$, $kInteractionEventDisengage$,
	//# and $kInteractionEventTrack$ events. The remaining two event types, $kInteractionEventActivate$ and
	//# $kInteractionEventDeactivate$, should be sent directly to a controller when the user explicitly provides
	//# input with the intent to interact with an object.
	//
	//# It can be determined whether an $Interactor$ instance is currently engaged with an interactive object by calling
	//# the $@Interactor::GetInteractionNode@$ function.
	//
	//# \also	$@Interactor::SetInteractionProbe@$
	//# \also	$@Interactor::GetInteractionNode@$
	//# \also	$@Controller/Controller::HandleInteractionEvent@$
	
	
	class Interactor : public ListElement<Interactor>
	{
		private:
			
			Link<Node>	interactionNode;
			Point3D		interactionPosition;
			
			Point3D		interactionPoint[2];
		
		public:
			
			C4API Interactor();
			C4API virtual ~Interactor();
			
			Node *GetInteractionNode(void) const
			{
				return (interactionNode);
			}
			
			void SetInteractionNode(Node *node)
			{
				interactionNode = node;
			}
			
			const Point3D& GetInteractionPosition(void) const
			{
				return (interactionPosition);
			}
			
			C4API void SetInteractionProbe(const Point3D& p1, const Point3D& p2);
			C4API virtual void HandleInteractionEvent(InteractionEventType type, Node *node, const Point3D *position = nullptr);
			
			void DetectInteraction(const World *world);
	};
	
	
	//# \class	World	Encapsulates a complete world.
	//
	//# The $World$ class encapsulates a complete world.
	//
	//# \def	class World : public LinkTarget<World>
	//
	//# \ctor	World(const char *name, unsigned_int32 flags = 0);
	//# \ctor	World(Node *root, unsigned_int32 flags = 0);
	//
	//# \param	name	The name of a world resource.
	//# \param	root	A pointer to the root node of the world's scene graph.
	//# \param	flags	The initial world flags.
	//
	//# \desc
	//#
	//
	//# \base	Utilities/LinkTarget<World>		Used internally by the World Manager.
	//
	//# \also	$@Node@$
	//# \also	$@WorldMgr@$
	
	
	//# \function	World::GetWorldFlags		Returns the world flags.
	//
	//# \proto	unsigned_int32 GetWorldFlags(void) const;
	//
	//# \desc
	//
	//# \also	$@World::SetWorldFlags@$
	
	
	//# \function	World::SetWorldFlags		Sets the world flags.
	//
	//# \proto	void SetWorldFlags(unsigned_int32 flags);
	//
	//# \param	flags	The new world flags.
	//
	//# \desc
	//
	//# \also	$@World::GetWorldFlags@$
	
	
	//# \function	World::SetFinalColorTransform		Sets the color transform used in post-processing.
	//
	//# \proto	void SetFinalColorTransform(const ColorRGBA& scale, const ColorRGBA& bias);
	//# \proto	void SetFinalColorTransform(const ColorRGBA& red, const ColorRGBA& green, const ColorRGBA& blue, const ColorRGBA& bias);
	//
	//# \param	scale	The componentwise scale color.
	//# \param	bias	The componentwise bias color.
	//# \param	red		The color matrix row for the red channel.
	//# \param	green	The color matrix row for the green channel.
	//# \param	blue	The color matrix row for the blue channel.
	//
	//# \desc
	//# The $SetFinalColorTransform$ sets the color transform that is used in the final stage of
	//# post-processing when a world is rendered. If the $scale$ parameter is specified, then the red,
	//# green, and blue components of the final color are multiplied by the corresponding components of the
	//# $scale$ color. If the $red$, $green$, and $blue$ parameters are specified, then they serve as rows of
	//# a color matrix that transform the final color. The alpha channels are not included in the calculation.
	//# In both cases, the $bias$ parameter specifies a color that is added to the result of the scale or matrix transform.
	//#
	//# The channels of each color passed to the $SetFinalColorTransform$ function may be any floating-point values.
	//# By default, the post-processing color transform uses a scale color of (1,1,1,1) and a bias color of (0,0,0,0).
	
	
	//# \function	World::GetRootNode		Returns the root node of a world.
	//
	//# \proto	Zone *GetRootNode(void) const;
	//
	//# \desc
	//# The $GetRootNode$ function returns a pointer to the root node of a world. Since the root node
	//# of a world is always a zone, the return value is a pointer to a $@Zone@$ node.
	//
	//# \also	$@Zone@$
	
	
	//# \function	World::FindZone		Finds the zone containing a given point.
	//
	//# \proto	Zone *FindZone(const Point3D& position, bool remapTransition = false) const;
	//
	//# \param	position			The world-space position whose containing zone will be found.
	//# \param	remapTransition		Used internally by the World Manager. This should always be $false$.
	//
	//# \desc
	//
	//# \also	$@World::GetRootNode@$
	
	
	//# \function	World::GetCamera		Returns the current camera.
	//
	//# \proto	FrustumCamera *GetCamera(void) const;
	//
	//# \desc
	//
	//# \also	$@World::SetCamera@$
	//# \also	$@World::Render@$
	
	
	//# \function	World::SetCamera		Sets the current camera.
	//
	//# \proto	void SetCamera(FrustumCamera *camera);
	//
	//# \param	camera		A pointer to the camera.
	//
	//# \desc
	//
	//# \also	$@World::GetCamera@$
	//# \also	$@World::Render@$
	
	
	//# \function	World::GetController		Returns the controller having a specific controller index.
	//
	//# \proto	Controller *GetController(int32 index) const;
	//
	//# \param	index	The index of the controller.
	//
	//# \desc
	//# The $GetController$ function returns a pointer to the controller having the index specified by the $index$
	//# parameter. If no such controller exists, then the return value is $nullptr$.
	//
	//# \also	$@Controller/Controller@$
	
	
	//# \div
	//# \function	World::AddInteractor	Adds an interactor to a world.
	//
	//# \proto	void AddInteractor(Interactor *interactor);
	//
	//# \param	interactor		The interactor to add to the world.
	//
	//# \desc
	//# The $AddInteractor$ function adds an interactor to a world. Each interactor has an interaction probe
	//# that is tested against interactive nodes in a world when the $@World::Interact@$ function is called
	//# by the World Manager.
	//
	//# \also	$@World::RemoveInteractor@$
	//# \also	$@World::Interact@$
	//# \also	$@Interactor@$
	
	
	//# \function	World::RemoveInteractor		Removes an interactor from a world.
	//
	//# \proto	void RemoveInteractor(Interactor *interactor);
	//
	//# \param	interactor		The interactor to remove from the world.
	//
	//# \desc
	//# The $RemoveInteractor$ function removes an interactor from a world so that it is no longer tested against
	//# interactive nodes.
	//
	//# \also	$@World::AddInteractor@$
	//# \also	$@World::Interact@$
	//# \also	$@Interactor@$
	
	
	//# \function	World::Interact		Tests for interactions in a world.
	//
	//# \proto	virtual void Interact(void);
	//
	//# \desc
	//# The $Interact$ function is called each frame after all movement has completed and before any rendering
	//# takes place. It tests all of the active interaction probes and dispatches passive interaction events.
	//# This function can be overridden in order to set interaction probes before the base class counterpart is called.
	//
	//# \also	$@World::AddInteractor@$
	//# \also	$@World::RemoveInteractor@$
	//# \also	$@Interactor@$
	
	
	//# \div
	//# \function	World::SetRenderSize		Sets the size of the viewport into which the world is rendered.
	//
	//# \proto	void SetRenderSize(int32 width, int32 height);
	//
	//# \param	width		The width of the viewport.
	//# \param	height		The height of the viewport.
	//
	//# \desc
	//
	//# \also	$@World::Render@$
	
	
	//# \function	World::Render		Renders a world.
	//
	//# \proto	virtual void Render(unsigned_int32 perspective = 0);
	//
	//# \param	perspective		Additional perspective flags to apply to the rendered scene. This should normally be 0.
	//
	//# \desc
	//
	//# \also	$@World::SetCamera@$
	//# \also	$@World::SetRenderSize@$
	
	
	//# \div
	//# \function	World::DetectCollision		Detects a collision between world geometry and a swept sphere.
	//
	//# \proto	bool DetectCollision(const Point3D& p1, const Point3D& p2, float radius, unsigned_int32 kind,
	//# \proto2	CollisionData *collisionData, int32 threadIndex = JobMgr::kMaxWorkerThreadCount) const;
	//
	//# \param	p1				The beginning of the line segment in world space.
	//# \param	p2				The end of the line segment in world space.
	//# \param	radius			The radius of the sphere. This cannot be negative, but it can be zero.
	//# \param	kind			The collision kind.
	//# \param	collisionData	The returned collision data.
	//# \param	threadIndex		The index of the Job Manager worker thread that is calling this function.
	//
	//# \desc
	//# The points specified by the parameters $p1$ and $p2$, combined with the radius specified by the $radius$
	//# parameter, define a directed swept sphere. The $DetectCollision$ function detects the first collision between
	//# this swept sphere and all enabled $@Geometry@$ nodes possessing collision information. If a collision is detected,
	//# then the function returns $true$; otherwise, it returns $false$.
	//# 
	//# The $kind$ parameter can be used to invalidate certain types of collisions. When a candidate geometry is
	//# encountered in the collision detection process, its collision exclusion mask is logically ANDed with the
	//# value of the $kind$ parameter. A collision can only occur if the result of this operation is zero. The collision
	//# mask associated with a geometry can be set using the $@GeometryObject::SetCollisionExclusionMask@$ function.
	//# The collision kind can be a combination (through logical OR) of the following predefined values and application-defined values.
	//
	//# \table	CollisionKind
	//
	//# If a collision occurs, then the $@CollisionData@$ structure pointed to by the $collisionData$ parameter is
	//# filled out with information about the collision. The $param$ field of this data structure represents the
	//# fraction of the distance that the sphere traveled from $p1$ to $p2$ before the collision occurred.
	//#
	//# The $threadIndex$ parameter specifies the index of the Job Manager worker thread that has called the $DetectCollision$
	//# function. If the $DetectCollision$ function is called from the main thread, then this parameter should not be
	//# specified so that the default value is used. If the $DetectCollision$ function is called from a job, then the
	//# $threadIndex$ parameter should be set to the value returned by the $@System/Job::GetThreadIndex@$ function.
	//# The $threadIndex$ parameter must be set correctly in order for multithreaded collision detection to work properly.
	//# 
	//# The $DetectCollision$ function works by intersecting a line segment with the Minkowski sum of a sphere and
	//# arbitrary polygon meshes. The algorithm is very precise and can determine when collisions occur with the
	//# expanded faces, edges, or vertices of the mesh referenced by a geometry node. If the value of the $radius$
	//# parameter is 0.0, then the collision detection reduces to a ray intersection with faces only.
	//
	//# \also	$@CollisionData@$
	//# \also	$@World::QueryCollision@$
	//# \also	$@World::QueryProximity@$
	//# \also	$@GeometryObject::GetCollisionExclusionMask@$
	//# \also	$@GeometryObject::SetCollisionExclusionMask@$
	
	
	//# \function	World::QueryCollision		Detects whether a swept sphere collides with world geometry or rigid bodies.
	//
	//# \proto	CollisionState QueryCollision(const Point3D& p1, const Point3D& p2, float radius, unsigned_int32 kind,
	//# \proto2	CollisionData *collisionData, const RigidBodyController *excludeBody = nullptr, int32 threadIndex = JobMgr::kMaxWorkerThreadCount) const;
	//
	//# \param	p1				The beginning of the line segment in world space.
	//# \param	p2				The end of the line segment in world space.
	//# \param	radius			The radius of the sphere. This cannot be negative, but it can be zero.
	//# \param	kind			The collision kind.
	//# \param	collisionData	The returned collision data.
	//# \param	excludeBody		A rigid body that will be excluded from the query.
	//# \param	threadIndex		The index of the Job Manager worker thread that is calling this function.
	//
	//# \desc
	//# The points specified by the parameters $p1$ and $p2$, combined with the radius specified by the $radius$
	//# parameter, define a directed swept sphere. The $QueryCollision$ function detects the first collision between
	//# this swept sphere and all $@Geometry@$ nodes possessing collision information. Unlike the $@World::DetectCollision@$
	//# function, the $QueryCollision$ function also detects collisions with any $@PhysicsMgr/RigidBodyController@$ objects. If a
	//# collision is detected, then the function returns either $kCollisionStateGeometry$ or $kCollisionStateRigidBody$,
	//# depending on the type of collision; otherwise, it returns $kCollisionStateNone$.
	//# 
	//# The $kind$ parameter can be used to invalidate certain types of collisions. When a candidate geometry or rigid body
	//# is encountered in the collision detection process, its collision exclusion mask is logically ANDed with the
	//# value of the $kind$ parameter. A collision can only occur if the result of this operation is zero. The collision
	//# mask associated with a geometry can be set using the $@GeometryObject::SetCollisionExclusionMask@$ function,
	//# and the collision mask for a rigid body can be set using the $@PhysicsMgr/RigidBodyController::SetCollisionExclusionMask@$ function.
	//# The collision kind can be a combination (through logical OR) of the following predefined values and application-defined values.
	//
	//# \table	CollisionKind
	//
	//# If a collision occurs, then the $@CollisionData@$ structure pointed to by the $collisionData$ parameter is
	//# filled out with information about the collision. The $param$ field of this data structure represents the
	//# fraction of the distance that the sphere traveled from $p1$ to $p2$ before the collision occurred.
	//#
	//# If the $excludeBody$ parameter is not $nullptr$, then any potential collisions with the rigid body is specifies are ignored.
	//#
	//# The $threadIndex$ parameter specifies the index of the Job Manager worker thread that has called the $QueryCollision$
	//# function. If the $QueryCollision$ function is called from the main thread, then this parameter should not be
	//# specified so that the default value is used. If the $QueryCollision$ function is called from a job, then the
	//# $threadIndex$ parameter should be set to the value returned by the $@System/Job::GetThreadIndex@$ function.
	//# The $threadIndex$ parameter must be set correctly in order for multithreaded collision queries to work properly.
	//
	//# \also	$@CollisionData@$
	//# \also	$@World::DetectCollision@$
	//# \also	$@World::QueryProximity@$
	//# \also	$@GeometryObject::GetCollisionExclusionMask@$
	//# \also	$@GeometryObject::SetCollisionExclusionMask@$
	//# \also	$@PhysicsMgr/RigidBodyController::GetCollisionExclusionMask@$
	//# \also	$@PhysicsMgr/RigidBodyController::SetCollisionExclusionMask@$
	
	
	//# \function	World::QueryProximity		Enumerates the world geometry nodes and rigid bodies that intersect a sphere.
	//
	//# \proto	void QueryProximity(const Point3D& center, float radius, ProximityProc *proc, void *cookie) const;
	//
	//# \param	center			The center of the sphere in world space.
	//# \param	radius			The radius of the sphere. This must be positive.
	//# \param	proc			A pointer to a function that is called for each node intersecting the sphere.
	//# \param	cookie			A user-defined pointer that is passed to the callback function specified by the $proc$ parameter.
	//
	//# \desc
	//# The $QueryProximity$ function searches the world for all geometry nodes and rigid bodies having bounding volumes
	//# that intersect the sphere given by the $center$ and $radius$ parameters. For each geometry node or rigid body found,
	//# the callback function specified by the $proc$ parameter is called. The $ProximityProc$ type is defined as follows.
	//
	//# \code	typedef ProximityResult ProximityProc(Node *node, const Point3D& center, float radius, void *cookie);
	//
	//# The $node$ parameter passed to the callback function is either a geometry node or a node of any type to which a
	//# rigid body controller is attached. The $center$, $radius$, and $cookie$ parameters are the same as those passed to the
	//# $QueryProximity$ function. The callback function should return one of the following values to determine how the enumerate proceeds.
	//
	//# \table	ProximityResult
	//
	//# The callback function is allowed to delete the node passed to it or any of its subnodes, but it may not delete
	//# other nodes elsewhere in the world. If the callback function deletes the node passed into the $node$ parameter,
	//# then it must return either $kProximitySkipSuccessors$ or $kProximityStop$.
	//
	//# \also	$@World::QueryCollision@$
	//# \also	$@World::DetectCollision@$
	
	
	//# \function	World::ActivateTriggers		Activates all triggers through which a given segment passes.
	//
	//# \proto	void ActivateTriggers(const Point3D& p1, const Point3D& p2, float radius, Node *activator = nullptr);
	//
	//# \param	p1			The beginning of the line segment in world space.
	//# \param	p2			The end of the line segment in world space.
	//# \param	radius		The radius of the line segment.
	//# \param	activator	The node that is assigned to be a trigger's activator.
	//
	//# \desc
	//# The $ActivateTriggers$ function finds all trigger nodes in the world that intersect the line segment specified
	//# by the $p1$ and $p2$ parameters and activates them. If the $radius$ parameter is greater than zero, then the
	//# line segment is actually a swept sphere, and intersections are tested with its volume instead of an infinitely
	//# thin line segment.
	//#
	//# When a trigger node is activated, it searches for a controller to activate in the following order and performs
	//# exactly one of the following actions.
	//#
	//# 1. If the trigger node itself has a controller, then that controller is activated.<br />
	//# 2. If the trigger object specifies a target connector key, there is a node connected to the trigger through a connector having that key, and that node has a controller, then that controller is activated.<br />
	//# 3. If such a target node exists, but it does not have a controller, then its immediate subnodes are examined. Every controller belonging to that set of subnodes is activated.
	//
	//# \also	$@Trigger@$
	
	
	//# \div
	//# \function	World::HandleNewRigidBodyContact		Called by default when a new contact is made between two rigid bodies.
	//
	//# \proto	virtual RigidBodyStatus HandleNewRigidBodyContact(RigidBodyController *rigidBody, const RigidBodyContact *contact,
	//# \proto2	RigidBodyController *contactBody);
	//
	//# \param	rigidBody		One rigid body making contact.
	//# \param	contact			The new contact.
	//# \param	contactBody		The other rigid body making contact.
	//
	//# \desc
	//# The $HandleNewRigidBodyContact$ function is called by the $@PhysicsMgr/RigidBodyController@$ class by default when a rigid body
	//# makes a new contact with another rigid body. This function can be overridden in a subclass of $World$ in order to carry
	//# out a specialized response to a collision.
	//#
	//# The $HandleNewRigidBodyContact$ function should return one of the following values.
	//
	//# \table RigidBodyStatus
	//
	//# The default implementation of the $HandleNewRigidBodyContact$ function returns $kRigidBodyUnchanged$.
	//
	//# \also	$@World::HandleNewGeometryContact@$
	//# \also	$@PhysicsMgr/RigidBodyController::HandleNewRigidBodyContact@$
	//# \also	$@PhysicsMgr/RigidBodyContact@$
	
	
	//# \function	World::HandleNewGeometryContact		Called by default when a new contact is made between a rigid body and a geometry node.
	//
	//# \proto	virtual RigidBodyStatus HandleNewGeometryContact(RigidBodyController *rigidBody, const GeometryContact *contact);
	//
	//# \param	rigidBody		The rigid body making contact.
	//# \param	contact			The new contact.
	//
	//# \desc
	//# The $HandleNewGeometryContact$ function is called by the $@PhysicsMgr/RigidBodyController@$ class by default when a rigid body
	//# makes a new contact with a geometry node. This function can be overridden in a subclass of $World$ in order to carry
	//# out a specialized response to a collision.
	//#
	//# The $HandleNewGeometryContact$ function should return one of the following values.
	//
	//# \table RigidBodyStatus
	//
	//# The default implementation of the $HandleNewGeometryContact$ function returns $kRigidBodyUnchanged$.
	//
	//# \also	$@World::HandleNewRigidBodyContact@$
	//# \also	$@PhysicsMgr/RigidBodyController::HandleNewGeometryContact@$
	//# \also	$@PhysicsMgr/GeometryContact@$
	
	
	class World : public LinkTarget<World>
	{
		friend class WorldMgr;
		
		public:
			
			typedef ProximityResult ProximityProc(Node *, const Point3D&, float, void *);
		
		private:
			
			ResourceName					worldName;
			ResourceLocation				resourceLocation;
			
			Link<World>						previousWorld;
			Map<InstancedWorldData>			instancedWorldDataMap;
			Map<GenericModelData>			genericModelDataMap;
			
			unsigned_int32					worldFlags;
			unsigned_int32					worldPerspective;

			float							shaderTime;
			float							velocityNormalizationTime;
			
			int32							renderWidth;
			int32							renderHeight;
			
			ColorRGBA						finalColorScale[3];
			ColorRGBA						finalColorBias;
			
			Node							*rootNode;
			Skybox							*worldSkybox;
			const ColorRGBA					*clearColor;
			
			FrustumCamera					*currentCamera;
			Zone							*cameraZone;
			Zone							*listenerZone;
			
			const WorldContext				*currentWorldContext;
			
			unsigned_int32					geometryRenderStamp;
			unsigned_int32					shadowRenderStamp;
			
			int32							remoteRecursionCount;
			int32							cameraRecursionCount;
			
			List<Renderable>				renderStageList[kRenderStageCount];
						
			List<TerrainLevelGeometry>		terrainList;
			List<Reference<LightRegion> >	lightRegionList;
			
			unsigned_int32					impostorRenderStamp;
			Map<ImpostorSystem>				impostorSystemMap;
			
			unsigned_int8					controllerParity;
			unsigned_int8					effectParity;
			unsigned_int8					sourceParity;
			unsigned_int8					triggerParity;
			
			List<Effect>					activeEffectList[2];
			
			List<Source>					engagedSourceList;
			List<Source>					playingSourceList[2];
			
			unsigned_int32					triggerActivateStamp;
			List<Trigger>					activeTriggerList[2];
			
			Batch							worldBatch;
			
			WorldObservable					updateObservable;
			
			Array<ControllerData>			controllerArray;
			int32							staticControllerCount;
			int32							firstFreeControllerIndex;
			int32							lastFreeControllerIndex;
			
			List<Controller>				controllerList[2];
			List<Controller>				physicsControllerList;
			
			List<Interactor>				interactorList;
			
			int32							worldCounter[kWorldCounterCount];
			
			#if C4DIAGNOSTICS
			
				unsigned_int32				diagnosticFlags;
				List<Renderable>			shadowRegionDiagnosticList;
				List<Renderable>			rigidBodyDiagnosticList;
				List<Renderable>			contactDiagnosticList;
				
				static List<Renderable>		lightRegionRenderList;
				static Renderable			lightRegionRenderable;
				static List<Attribute>		lightRegionAttributeList;
				static DiffuseAttribute		lightRegionDiffuseColor;
				
				static List<Renderable>		sourcePathRenderList;
				static Renderable			sourcePathRenderable;
				static List<Attribute>		sourcePathAttributeList;
				static DiffuseAttribute		sourcePathDiffuseColor;
				static Point3D				sourcePathVertex[2];
			
			#endif
			
			static bool NodeVisible(const Node *node, const Region *region, const List<Region> *occlusionList)
			{
				return ((node->Visible(region)) && ((occlusionList->Empty()) || (!node->Occluded(occlusionList->First()))));
			}
			
			void SetCameraClearParams(CameraObject *object) const;
			
			static ControllerMessage *ConstructControllerMessage(ControllerMessageType controllerMessageType, int32 controllerIndex, Decompressor& data, void *world);
			static void ReceiveControllerMessage(const ControllerMessage *message, void *world);
			
			static Zone *FindZone(Zone *root, const Point3D& position, Zone **transition);
			
			static bool DetectGeometryCollision(Geometry *geometry, const CollisionParams *collisionParams, CollisionData *collisionData, QueryThreadData *threadData);
			static bool DetectNodeCollision(Node *node, const CollisionParams *collisionParams, CollisionData *collisionData, QueryThreadData *threadData);
			static bool DetectCellCollision(const Site *cell, const CollisionParams *collisionParams, CollisionData *collisionData, QueryThreadData *threadData);
			static bool DetectZoneCollision(Zone *zone, const CollisionParams *collisionParams, CollisionData *collisionData, QueryThreadData *threadData);
			
			static CollisionState QueryNodeCollision(Node *node, const CollisionParams *collisionParams, CollisionData *collisionData, QueryThreadData *threadData);
			static CollisionState QueryCellCollision(const Site *cell, const CollisionParams *collisionParams, CollisionData *collisionData, QueryThreadData *threadData);
			static CollisionState QueryZoneCollision(Zone *zone, const CollisionParams *collisionParams, CollisionData *collisionData, QueryThreadData *threadData);
			
			static ProximityResult QueryNodeProximity(Node *node, const ProximityParams *proximityParams);
			static ProximityResult QueryCellProximity(const Site *cell, const ProximityParams *proximityParams);
			static ProximityResult QueryZoneProximity(Zone *zone, const ProximityParams *proximityParams);
			
			static bool DetectNodeInteraction(Node *node, const Box3D& box, const Point3D& p1, const Point3D& p2, InteractionData *interactionData, QueryThreadData *threadData);
			static bool DetectCellInteraction(const Site *cell, const Box3D& box, const Point3D& p1, const Point3D& p2, InteractionData *interactionData, QueryThreadData *threadData);
			static bool DetectPanelEffectInteraction(const Zone *zone, const Box3D& box, const Point3D& p1, const Point3D& p2, InteractionData *interactionData);
			static bool DetectZoneInteraction(const Zone *zone, const Box3D& box, const Point3D& p1, const Point3D& p2, InteractionData *interactionData, QueryThreadData *threadData);
			
			void ActivateCellTriggers(Site *cell, const Box3D& box, const Point3D& p1, const Point3D& p2, float radius, Node *activator);
			void ActivateZoneTriggers(Zone *zone, const Point3D& p1, const Point3D& p2, float radius, Node *activator);
			
			void MoveControllers(unsigned_int32 parity);
			void MoveEffects(unsigned_int32 parity);
			void MoveSources(unsigned_int32 parity);
			
			void Listen(void);
			
			#if C4DIAGNOSTICS
			
				void RenderSourcePaths(Zone *zone, const Transform4D& listenerTransform);
			
			#endif
			
			static bool NodeExcluded(const Node *node, const Node *exclude);
			static bool WorldBoundingBoxVisible(const Box3D& box, const Region *region, const List<Region> *occlusionList);
			static bool ShadowNodeVisible(const Node *node, const List<Region> *shadowRegionList);
			static bool ShadowCellVisible(const Site *cell, const List<Region> *shadowRegionList);
			
			static void SetNodeFogState(const WorldContext *worldContext, const Node *node, Renderable *renderable);
			void ProcessGeometry(const WorldContext *worldContext, Geometry *geometry);
			
			void RenderEffects(const WorldContext *worldContext, CameraRegion *cameraRegion);
			
			void RenderAmbientGeometry(const WorldContext *worldContext, Geometry *geometry, CameraRegion *cameraRegion);
			void RenderAmbientNode(const WorldContext *worldContext, Node *node, CameraRegion *cameraRegion);
			void RenderAmbientCell(const WorldContext *worldContext, const Site *cell, CameraRegion *cameraRegion);
			void RenderAmbientRegion(WorldContext *worldContext, CameraRegion *rootRegion);
			
			void RenderInfiniteShadowVolume(Geometry *geometry, InfiniteLight *light, StencilMode stencilMode);
			void RenderInfiniteShadowNode(const WorldContext *worldContext, Node *node, const ShadowRenderData *renderData);
			void RenderInfiniteShadowCell(const WorldContext *worldContext, const Site *cell, const ShadowRenderData *renderData);
			static void CalculateInfiniteNearClipRegion(const FrustumCamera *camera, const Vector3D& lightDirection, Region *nearClipRegion);
			static void CalculateInfiniteShadowRegion(const CameraRegion *cameraRegion, const Vector3D& lightDirection, ShadowRegion *shadowRegion);
			void RenderInfiniteLight(WorldContext *worldContext, InfiniteLight *light);
			
			void RenderPointShadowVolume(Geometry *geometry, PointLight *light, StencilMode stencilMode);
			void RenderPointShadowNode(const WorldContext *worldContext, Node *node, const ShadowRenderData *renderData);
			void RenderPointShadowCell(const WorldContext *worldContext, const Site *cell, const ShadowRenderData *renderData);
			static void CalculatePointNearClipRegion(const FrustumCamera *camera, const Point3D& lightPosition, Region *nearClipRegion);
			static void CalculatePointShadowRegion(const CameraRegion *cameraRegion, const Point3D& lightPosition, ShadowRegion *shadowRegion);
			void RenderPointLight(WorldContext *worldContext, PointLight *light);
			
			static bool PointLightVisible(const Light *light, CameraRegion *rootRegion, const List<Region> *occlusionList);
			static bool LightVisibleInTransition(const CameraRegion *cameraRegion, const LightRegion *lightRegion);
			static bool ClipLightRegion(const LightRegion *lightRegion, const CameraRegion *cameraRegion);
			void CollectLightRegions(const WorldContext *worldContext, CameraRegion *rootRegion);
			
			bool ProcessFogSpace(WorldContext *worldContext, const FogSpace *fogSpace, CameraRegion *rootRegion);
			void ProcessPortal(WorldContext *worldContext, Portal *portal, CameraRegion *rootRegion);
			void ProcessCameraRegion(WorldContext *worldContext, CameraRegion *rootRegion);
			
			void RenderIndirectPortals(const WorldContext *worldContext);
			void RenderRemoteCamera(const WorldContext *worldContext, RemotePortal *remotePortal, RenderTargetType target, unsigned_int32 perspectiveFlag, const PortalData *portalData);
			void RenderCamera(WorldContext *worldContext, CameraRegion *cameraRegion, RenderTargetType target);
			void RenderDistantCamera(WorldContext *worldContext, CameraRegion *cameraRegion, RenderTargetType target);
			
			void RenderShadowMapNode(const WorldContext *worldContext, Node *node, const Region *cameraRegion, const Region *shadowRegion, const List<Region> *occlusionList);
			void RenderShadowMapCell(const WorldContext *worldContext, const Site *cell, const Region *cameraRegion, const Region *shadowRegion, const List<Region> *occlusionList);
			void RenderShadowMapRegion(const WorldContext *worldContext, const CameraRegion *cameraRegion, const Region *shadowRegion, const List<Region> *occlusionList);
			void ProcessShadowMapRegion(const WorldContext *worldContext, const OrthoCamera *camera, CameraRegion *rootRegion, const Region *shadowRegion, List<Region> *occlusionList);
			void RenderShadowMap(const WorldContext *worldContext, DepthLight *depthLight, int32 sectionIndex, const LightShadowData *shadowData, const Region *shadowRegion);
		
		public:
			
			C4API World(const char *name, unsigned_int32 flags = 0);
			C4API World(Node *root, unsigned_int32 flags = 0);
			C4API virtual ~World();
			
			const ResourceName& GetWorldName(void) const
			{
				return (worldName);
			}
			
			const ResourceLocation *GetResourceLocation(void) const
			{
				return (&resourceLocation);
			}
			
			void PurgeInstancedWorldData(void)
			{
				instancedWorldDataMap.Purge();
			}
			
			unsigned_int32 GetWorldFlags(void) const
			{
				return (worldFlags);
			}
			
			void SetWorldFlags(unsigned_int32 flags)
			{
				worldFlags = flags;
			}
			
			unsigned_int32 GetWorldPerspective(void) const
			{
				return (worldPerspective);
			}
			
			void SetWorldPerspective(unsigned_int32 perspective)
			{
				worldPerspective = perspective;
			}
			
			float GetVelocityNormalizationTime(void) const
			{
				return (velocityNormalizationTime);
			}
			
			void SetVelocityNormalizationTime(float time)
			{
				velocityNormalizationTime = time;
			}
			
			const ColorRGBA& GetFinalColorScale(int32 index = 0) const
			{
				return (finalColorScale[index]);
			}
			
			const ColorRGBA& GetFinalColorBias(void) const
			{
				return (finalColorBias);
			}
			
			Zone *GetRootNode(void) const
			{
				return (static_cast<Zone *>(rootNode));
			}
			
			FrustumCamera *GetCamera(void) const
			{
				return (currentCamera);
			}
			
			int32 GetRenderWidth(void) const
			{
				return (renderWidth);
			}
			
			int32 GetRenderHeight(void) const
			{
				return (renderHeight);
			}
			
			void SetRenderSize(int32 width, int32 height)
			{
				renderWidth = width;
				renderHeight = height;
			}
			
			void AddPlayingSource(Source *source)
			{
				playingSourceList[sourceParity].Append(source);
			}
			
			void AddEffect(Effect *effect)
			{
				activeEffectList[effectParity].Append(effect);
			}
			
			void AddInteractor(Interactor *interactor)
			{
				interactorList.Append(interactor);
			}
			
			void RemoveInteractor(Interactor *interactor)
			{
				if (interactorList.Member(interactor)) interactorList.Remove(interactor);
			}
			
			void SubmitWorldJob(BatchJob *job)
			{
				TheJobMgr->SubmitJob(job, &worldBatch);
			}
			
			void FinishWorldBatch(void)
			{
				TheJobMgr->FinishBatch(&worldBatch);
			}
			
			void AddUpdateObserver(WorldObservable::ObserverType *observer)
			{
				updateObservable.AddObserver(observer);
			}
			
			int32 GetControllerArraySize(void) const
			{
				return (controllerArray.GetElementCount());
			}
			
			unsigned_int32 GetControllerParity(void) const
			{
				return (controllerParity);
			}
			
			unsigned_int32 GetTriggerParity(void) const
			{
				return (triggerParity);
			}
			
			unsigned_int32 GetEffectParity(void) const
			{
				return (effectParity);
			}
			
			unsigned_int32 GetSourceParity(void) const
			{
				return (sourceParity);
			}
			
			#if C4DIAGNOSTICS
			
				unsigned_int32 GetDiagnosticFlags(void) const
				{
					return (diagnosticFlags);
				}
				
				void SetDiagnosticFlags(unsigned_int32 flags)
				{
					diagnosticFlags = flags;
				}
				
				void PurgeShadowDiagnosticData(void)
				{
					shadowRegionDiagnosticList.Purge();
				}
				
				void AddRigidBodyRenderable(RigidBodyRenderable *renderable)
				{
					rigidBodyDiagnosticList.Append(renderable);
				}
				
				void PurgeRigidBodyDiagnosticData(void)
				{
					rigidBodyDiagnosticList.Purge();
				}
				
				void AddContactRenderable(ContactRenderable *renderable)
				{
					contactDiagnosticList.Append(renderable);
				}
				
				void PurgeContactDiagnosticData(void)
				{
					contactDiagnosticList.Purge();
				}
			
			#endif
			
			int32 GetWorldCounter(int32 index) const
			{
				return (worldCounter[index]);
			}
			
			void IncrementWorldCounter(int32 index)
			{
				worldCounter[index]++;
			}
			
			C4API virtual WorldResult Preprocess(void);
			C4API void ProcessWorldProperties(void);
			
			C4API void ExpandInstancedWorlds(Node *root, int32 depth = 0);
			
			Node *NewInstancedWorld(const char *name, Node::CloneFilterProc *filterProc = &Node::DefaultCloneFilter, void *filterCookie = nullptr);
			Node *NewGenericModel(const char *name, GenericModel *model);
			
			ImpostorSystem *GetImpostorSystem(MaterialObject *material, const float *clipData);
			
			void AddController(Controller *controller);
			void RemoveController(Controller *controller);
			void WakeController(Controller *controller);
			static void SleepController(Controller *controller);
			
			C4API Controller *GetController(int32 index) const;
			C4API int32 NewControllerIndex(void);
			
			C4API void SetCamera(FrustumCamera *camera);
			C4API void UpdateGeometry(Geometry *geometry);
			
			C4API Zone *FindZone(const Point3D& position, bool remapTransition = false) const;
			C4API bool DetectCollision(const Point3D& p1, const Point3D& p2, float radius, unsigned_int32 kind, CollisionData *collisionData, int32 threadIndex = JobMgr::kMaxWorkerThreadCount) const;
			C4API CollisionState QueryCollision(const Point3D& p1, const Point3D& p2, float radius, unsigned_int32 kind, CollisionData *collisionData, const RigidBodyController *excludeBody = nullptr, int32 threadIndex = JobMgr::kMaxWorkerThreadCount) const;
			C4API void QueryProximity(const Point3D& center, float radius, ProximityProc *proc, void *cookie) const;
			
			const AcousticsProperty *DetectObstruction(const Point3D& position) const;
			bool DetectInteraction(const Point3D& p1, const Point3D& p2, InteractionData *interactionData) const;
			
			C4API void ActivateTriggers(const Point3D& p1, const Point3D& p2, float radius, Node *activator = nullptr);
			
			C4API virtual RigidBodyStatus HandleNewRigidBodyContact(RigidBodyController *rigidBody, const RigidBodyContact *contact, RigidBodyController *contactBody);
			C4API virtual RigidBodyStatus HandleNewGeometryContact(RigidBodyController *rigidBody, const GeometryContact *contact);
			C4API virtual void HandleWaterSubmergence(RigidBodyController *rigidBody);
			
			C4API virtual void Move(void);
			C4API virtual void Update(void);
			C4API virtual void Interact(void);
			
			C4API virtual void BeginRendering(void);
			C4API virtual void EndRendering(void);
			C4API virtual void Render(void);
			
			C4API void SetFinalColorTransform(const ColorRGBA& scale, const ColorRGBA& bias);
			C4API void SetFinalColorTransform(const ColorRGBA& red, const ColorRGBA& green, const ColorRGBA& blue, const ColorRGBA& bias);
	};
	
	
	//# \class	WorldMgr	The World Manager class.
	//
	//# \def	class WorldMgr : public Manager<WorldMgr>
	//
	//# \desc
	//# The $WorldMgr$ class encapsulates the high-level world management features of the C4 Engine.
	//# The single instance of the World Manager is constructed during an application's initialization
	//# and destroyed at termination.
	//# 
	//# The World Manager's member functions are accessed through the global pointer $TheWorldMgr$.
	//
	//# \also	$@World@$
	
	
	//# \function	WorldMgr::GetWorld		Returns the currently active world.
	//
	//# \proto	World *GetWorld(void) const;
	//
	//# \desc
	//
	//# \also	$@World@$
	//# \also	$@WorldMgr::LoadWorld@$
	//# \also	$@WorldMgr::UnloadWorld@$
	
	
	//# \function	WorldMgr::SetWorldConstructor		Installs a world class constructor function.
	//
	//# \proto	void SetWorldConstructor(WorldConstructProc *proc, void *cookie = nullptr);
	//
	//# \param	proc	A pointer to the world constructor function.
	//# \param	cookie	A pointer to user-defined data that is passed to the world constructor function.
	//
	//# \desc
	//
	//# \code	typedef World *WorldConstructProc(const char *, void *);
	//
	//# \also	$@World@$
	
	
	//# \function	WorldMgr::LoadWorld		Loads a world resource and makes it the current world.
	//
	//# \proto	WorldResult LoadWorld(const char *name);
	//
	//# \param	name	The name of the world resource to load.
	//
	//# \desc
	//
	//# \also	$@World@$
	//# \also	$@WorldMgr::UnloadWorld@$
	//# \also	$@WorldMgr::SaveDeltaWorld@$
	//# \also	$@WorldMgr::RestoreDeltaWorld@$
	
	
	//# \function	WorldMgr::UnloadWorld		Unloads the current world data.
	//
	//# \proto	void UnloadWorld(void);
	//
	//# \desc
	//
	//# \also	$@World@$
	//# \also	$@WorldMgr::LoadWorld@$
	//# \also	$@WorldMgr::SaveDeltaWorld@$
	//# \also	$@WorldMgr::RestoreDeltaWorld@$
	
	
	//# \function	WorldMgr::SaveDeltaWorld		Saves a delta file for the current world.
	//
	//# \proto	void SaveDeltaWorld(const char *name);
	//
	//# \param	name	The name of the file to save.
	//
	//# \desc
	//
	//# \also	$@World@$
	//# \also	$@WorldMgr::RestoreDeltaWorld@$
	//# \also	$@WorldMgr::LoadWorld@$
	//# \also	$@WorldMgr::UnloadWorld@$
	
	
	//# \function	WorldMgr::RestoreDeltaWorld		Restores a previously saved delta file.
	//
	//# \proto	void RestoreDeltaWorld(const char *name);
	//
	//# \param	name	The name of the file to restore.
	//
	//# \desc
	//
	//# \also	$@World@$
	//# \also	$@WorldMgr::SaveDeltaWorld@$
	//# \also	$@WorldMgr::LoadWorld@$
	//# \also	$@WorldMgr::UnloadWorld@$
	
	
	class C4_API WorldMgr : public Manager<WorldMgr>
	{
		friend class World;
		
		public:
			
			typedef World *WorldConstructProc(const char *, void *);
		
		private:
			
			World							*currentWorld;
			
			WorldConstructProc				*worldConstructorProc;
			void							*worldConstructorCookie;
			
			Constructor<Object>				objectConstructor;
			StateSender						controllerStateSender;
			DisplayEventHandler				displayEventHandler;
			VariableObserver<WorldMgr>		lightDetailLevelObserver;
			
			int32							lightDetailLevel;
			float							defaultVelocityNormalizationTime;
			
			WorldResult InitWorld(World *world);
			
			static Object *ConstructObject(Unpacker& data, unsigned_int32 unpackFlags);
			static void SendControllerState(Player *to, void *cookie);
			static void HandleDisplayEvent(const DisplayEventData *eventData, void *cookie);
			
			void HandleLightDetailLevelEvent(Variable *variable);
		
		public:
			
			WorldMgr(int);
			~WorldMgr();
			
			EngineResult Construct(void);
			void Destruct(void);
			
			World *GetWorld(void) const
			{
				return (currentWorld);
			}
			
			void SetWorldConstructor(WorldConstructProc *proc, void *cookie = nullptr)
			{
				worldConstructorProc = proc;
				worldConstructorCookie = cookie;
			}
			
			int32 GetLightDetailLevel(void) const
			{
				return (lightDetailLevel);
			}
			
			float GetDefaultVelocityNormalizationTime(void) const
			{
				return (defaultVelocityNormalizationTime);
			}
			
			void SetDefaultVelocityNormalizationTime(float time)
			{
				defaultVelocityNormalizationTime = time;
			}
			
			WorldResult LoadWorld(const char *name);
			void UnloadWorld(void);
			
			void SaveDeltaWorld(const char *name);
			WorldResult RestoreDeltaWorld(const char *name);
			
			void Move(void);
			void Render(void);
	};
	
	
	C4_API extern WorldMgr *TheWorldMgr;
}


#endif

// ZYURVUR
