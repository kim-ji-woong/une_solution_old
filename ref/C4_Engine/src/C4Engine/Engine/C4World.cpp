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


#include "C4World.h"
#include "C4Instances.h"
#include "C4Particles.h"
#include "C4Forces.h"
#include "C4Physics.h"
#include "C4Skybox.h"
#include "C4Scripts.h"
#include "C4Panels.h"
#include "C4Terrain.h"
#include "C4Water.h"
#include "C4Shaders.h"


using namespace C4;


namespace
{
	const float kCameraNearDepth			= 0.1F;
	const float kCameraFarDepth				= 500.0F;
	const float kNearClipEpsilon			= 0.01F;
	const float kCameraLightClipEpsilon		= 0.01F;
	const float kDetailEpsilon				= 0.01F;
	const float kDiagnosticRegionSize		= 64.0F;
	
	
	enum
	{
		kMaxCollisionEdgeCount		= 32,
		kMaxRemoteRecursionCount	= 3,
		kMaxCameraRecursionCount	= 3
	};
	
	
	enum
	{
		kPortalGroupReflection,
		kPortalGroupRefraction,
		kPortalGroupRemote,
		kPortalGroupCamera,
		kPortalGroupCount
	};
	
	
	enum
	{
		kWorldUnfogEnable		= 1 << 0
	};
}


WorldMgr *C4::TheWorldMgr = nullptr;


namespace C4
{
	template <> WorldMgr Manager<WorldMgr>::managerObject(0);
	template <> WorldMgr **Manager<WorldMgr>::managerPointer = &TheWorldMgr;
	
	template <> Heap C4::Memory<PortalData>::heap("PortalData", 4096, kHeapMutexless);
	
	
	struct CollisionParams
	{
		const RigidBodyController	*excludedRigidBody;
		const Point3D				*colliderPosition[2];
		float						colliderRadius;
		Box3D						colliderBox;
		unsigned_int32				collisionKind;
	};
	
	
	struct ProximityParams
	{
		World::ProximityProc		*proximityProc;
		void						*proximityCookie;
		const Point3D				*proximityCenter;
		float						proximityRadius;
		Box3D						proximityBox;
	};
	
	
	struct InteractionData
	{
		float			param;
		Point3D			position;
		Node			*interaction;
	};
	
	
	struct ControllerData
	{
		Controller		*controller;
		
		int32			prevControllerIndex;
		int32			nextControllerIndex;
	};
	
	
	struct ShadowRenderData
	{
		const Node			*excludeNode; 
		const LightRegion	*lightRegion;
		 
		List<Region>		shadowRegionList; 
		Region				nearClipRegion; 
	};
	 
	
	class PortalData : public MapElement<PortalData>, public Memory<PortalData>
	{
		private: 
			
			Portal		*targetPortal;
			Zone		*originZone;
			 
			int32		portalVertexCount;
			Point3D		portalVertex[kMaxPortalVertexCount];
		
		public:
			
			typedef Portal *KeyType;
			
			PortalData(Portal *portal, Zone *zone, int32 vertexCount, const Point3D *vertex);
			~PortalData();
			
			KeyType GetKey(void) const
			{
				return (targetPortal);
			}
			
			Portal *GetPortal(void) const
			{
				return (targetPortal);
			}
			
			Zone *GetOriginZone(void) const
			{
				return (originZone);
			}
			
			int32 GetVertexCount(void) const
			{
				return (portalVertexCount);
			}
			
			const Point3D *GetVertexArray(void) const
			{
				return (portalVertex);
			}
	};
	
	
	class QueryThreadData
	{
		public:
			
			int32					threadFlag;
			Array<Geometry *, 64>	geometryArray;
			
			QueryThreadData(int32 flag);
			~QueryThreadData();
			
			bool AddGeometry(Geometry *geometry);
			void ResetGeometryArray(void);
	};
	
	
	struct WorldContext
	{
		const FrustumCamera		*renderCamera;
		
		Skybox					*skyboxNode;
		bool					skyboxFlag;
		
		mutable bool			shadowMapKeepFlag;
		unsigned_int32			shadowInhibitMask;
		
		unsigned_int32			perspectiveFlags;
		int32					cameraMinDetailLevel;
		float					cameraDetailBias;
		const CameraRegion		*shadowReceiveRegion;
		
		List<Region>			occlusionList;
		Map<PortalData>			portalGroup[kPortalGroupCount];
		
		const FogSpace			**fogSpacePtr;
		List<Region>			unfoggedList;
		Region					unfoggedRegion;
		
		WorldContext(const FrustumCamera *camera)
		{
			renderCamera = camera;
			skyboxFlag = false;
			shadowMapKeepFlag = false;
			unfoggedRegion.SetPlaneCount(1);
		}
	};
}


ResourceDescriptor WorldResource::descriptor("wld");
ResourceDescriptor SaveResource::descriptor("sav");


#if C4DIAGNOSTICS

	List<Renderable> World::lightRegionRenderList;
	Renderable World::lightRegionRenderable(kRenderLineLoop);
	List<Attribute> World::lightRegionAttributeList;
	DiffuseAttribute World::lightRegionDiffuseColor(ColorRGBA(1.0F, 1.0F, 0.0F, 1.0F));
	
	List<Renderable> World::sourcePathRenderList;
	Renderable World::sourcePathRenderable(kRenderLines);
	List<Attribute> World::sourcePathAttributeList;
	DiffuseAttribute World::sourcePathDiffuseColor(ColorRGBA(0.0F, 0.5F, 1.0F, 1.0F));
	Point3D World::sourcePathVertex[2];

#endif


PortalData::PortalData(Portal *portal, Zone *zone, int32 vertexCount, const Point3D *vertex)
{
	targetPortal = portal;
	originZone = zone;
	
	portalVertexCount = vertexCount;
	for (machine a = 0; a < vertexCount; a++) portalVertex[a] = vertex[a];
}

PortalData::~PortalData()
{
}


QueryThreadData::QueryThreadData(int32 flag)
{
	threadFlag = flag;
}

QueryThreadData::~QueryThreadData()
{
}

bool QueryThreadData::AddGeometry(Geometry *geometry)
{
	int32 flag = threadFlag;
	
	volatile int32 *geometryFlags = geometry->GetQueryThreadFlags();
	if (*geometryFlags & flag) return (false);
	
	AtomicOr(geometryFlags, flag);
	geometryArray.AddElement(geometry);
	return (true);
}

void QueryThreadData::ResetGeometryArray(void)
{
	int32 mask = ~threadFlag;
	
	int32 count = geometryArray.GetElementCount();
	for (machine a = 0; a < count; a++) AtomicAnd(geometryArray[a]->GetQueryThreadFlags(), mask);
	
	geometryArray.Purge();
}


WorldResource::WorldResource(const char *name, ResourceCatalog *catalog) : Resource<WorldResource>(name, catalog)
{
}

WorldResource::~WorldResource()
{
}

int32 WorldResource::GetControllerCount(void) const
{
	const int32 *data = static_cast<const int32 *>(GetData());
	
	int32 controllerCount = data[2];
	if (data[0] != 1) Reverse(&controllerCount);
	
	return (controllerCount);
}

ResourceResult WorldResource::LoadObjectOffsetTable(ResourceLoader *loader, WorldHeader *worldHeader, int32 **offsetTable) const
{
	ResourceResult result = loader->Read(worldHeader, 0, sizeof(WorldHeader));
	if (result != kResourceOkay) return (result);
	
	int32 endian = worldHeader->endian;
	if (endian != 1) Reverse(worldHeader);
	
	int32 offsetCount = worldHeader->offsetCount;
	int32 *table = new int32[offsetCount];
	
	result = loader->Read(table, sizeof(WorldHeader), offsetCount * 4);
	if (result == kResourceOkay)
	{
		if (endian != 1) for (machine a = 0; a < offsetCount; a++) Reverse(&table[a]);
		*offsetTable = table;
		return (kResourceOkay);
	}
	
	delete[] table;
	return (result);
}

ResourceResult WorldResource::LoadAllObjects(ResourceLoader *loader, const WorldHeader *header, const int32 *offsetTable, char **objectData) const
{
	int32 start = offsetTable[0];
	unsigned_int32 size = offsetTable[header->offsetCount - 1] - start;
	char *data = new char[size];
	
	ResourceResult result = loader->Read(data, start, size);
	if (result == kResourceOkay)
	{
		*objectData = data;
		return (kResourceOkay);
	}
	
	delete[] data;
	return (result);
}

ResourceResult WorldResource::LoadObject(ResourceLoader *loader, int32 index, const int32 *offsetTable, char **objectData) const
{
	int32 start = offsetTable[index];
	unsigned_int32 size = offsetTable[index + 1] - start;
	char *data = new char[size];
	
	ResourceResult result = loader->Read(data, start, size);
	if (result == kResourceOkay)
	{
		*objectData = data;
		return (kResourceOkay);
	}
	
	delete[] data;
	return (result);
}


SaveResource::SaveResource(const char *name, ResourceCatalog *catalog) : Resource<SaveResource>(name, catalog)
{
}

SaveResource::~SaveResource()
{
}

int32 SaveResource::GetControllerCount(void) const
{
	const int32 *data = static_cast<const int32 *>(GetData());
	
	int32 controllerCount = data[2];
	if (data[0] != 1) Reverse(&controllerCount);
	
	return (controllerCount);
}


InstancedWorldData::InstancedWorldData(unsigned_int32 hash, Node *node)
{
	worldHash = hash;
	prototypeCopy = node;
}

InstancedWorldData::~InstancedWorldData()
{
	delete prototypeCopy;
}


GenericModelData::GenericModelData(unsigned_int32 hash, GenericModel *model)
{
	modelHash = hash;
	modelList.Append(model);
}

GenericModelData::~GenericModelData()
{
}


Interactor::Interactor()
{
}

Interactor::~Interactor()
{
}

void Interactor::SetInteractionProbe(const Point3D& p1, const Point3D& p2)
{
	interactionPoint[0] = p1;
	interactionPoint[1] = p2;
}

void Interactor::HandleInteractionEvent(InteractionEventType type, Node *node, const Point3D *position)
{
	switch (type)
	{
		case kInteractionEventEngage:
			
			interactionNode = node;
			interactionPosition = *position;
			break;
		
		case kInteractionEventDisengage:
			
			interactionNode = nullptr;
			break;
		
		case kInteractionEventTrack:
			
			interactionPosition = *position;
			break;
	}
}

void Interactor::DetectInteraction(const World *world)
{
	InteractionData		data;
	
	data.param = 1.0F;
	if (world->DetectInteraction(interactionPoint[0], interactionPoint[1], &data))
	{
		Node *node = data.interaction;
		Point3D p = node->GetInverseWorldTransform() * data.position;
		
		if (node == interactionNode)
		{
			HandleInteractionEvent(kInteractionEventTrack, node, &p);
		}
		else
		{
			if (interactionNode) HandleInteractionEvent(kInteractionEventDisengage, interactionNode);
			HandleInteractionEvent(kInteractionEventEngage, node, &p);
		}
	}
	else
	{
		if (interactionNode) HandleInteractionEvent(kInteractionEventDisengage, interactionNode);
	}
}


World::World(const char *name, unsigned_int32 flags) : updateObservable(this)
{
	worldName = name;
	worldFlags = flags;
	rootNode = nullptr;
}

World::World(Node *root, unsigned_int32 flags) : updateObservable(this)
{
	worldName[0] = 0;
	worldFlags = flags;
	rootNode = root;
}

World::~World()
{
	engagedSourceList.RemoveAll();
	
	activeTriggerList[0].RemoveAll();
	activeTriggerList[1].RemoveAll();
	
	controllerList[0].RemoveAll();
	controllerList[1].RemoveAll();
	
	delete rootNode;
	SetCamera(nullptr);
	
	delete previousWorld.GetTarget();
}

WorldResult World::Preprocess(void)
{
	if (!rootNode)
	{
		int32	controllerCount;
		
		if (worldFlags & kWorldRestore)
		{
			SaveResource *resource = SaveResource::Get(worldName, 0, TheResourceMgr->GetSaveCatalog());
			if (resource)
			{
				rootNode = Node::UnpackDeltaTree(resource->GetData(), worldName, previousWorld);
				controllerCount = resource->GetControllerCount();
				resource->Release();
			}
		}
		else
		{
			WorldResource *resource = WorldResource::Get(worldName, 0, nullptr, &resourceLocation);
			if (resource)
			{
				rootNode = Node::UnpackTree(resource->GetData());
				controllerCount = resource->GetControllerCount();
				resource->Release();
			}
		}
		
		if (!rootNode) return (kWorldLoadFailed);
		
		ExpandInstancedWorlds(rootNode);
		
		staticControllerCount = controllerCount;
		controllerArray.SetElementCount(controllerCount);

		for (machine a = 0; a < controllerCount; a++)
			controllerArray[a].controller = nullptr;
		
		#if C4PLAYSTATION3
		
			ProgramResource::Load(worldName);
		
		#endif
	}
	else
	{
		staticControllerCount = 0;
	}
	
	SetCamera(nullptr);
	
	worldFlags &= ~kWorldRestore;
	worldPerspective = 0;
	
	shaderTime = 0.0F;
	velocityNormalizationTime = TheWorldMgr->GetDefaultVelocityNormalizationTime();
	
	finalColorScale[0].Set(1.0F, 1.0F, 1.0F, 1.0F);
	finalColorBias.Set(0.0F, 0.0F, 0.0F, 0.0F);
	
	renderWidth = TheDisplayMgr->GetDisplayWidth();
	renderHeight = TheDisplayMgr->GetDisplayHeight();
	
	geometryRenderStamp = 0xFFFFFFFF;
	shadowRenderStamp = 0xFFFFFFFF;
	impostorRenderStamp = 0xFFFFFFFF;
	triggerActivateStamp = 0xFFFFFFFF;
	
	controllerParity = 0;
	effectParity = 0;
	sourceParity = 0;
	triggerParity = 0;
	
	firstFreeControllerIndex = kControllerUnassigned;
	lastFreeControllerIndex = kControllerUnassigned;
	
	for (machine a = 0; a < kWorldCounterCount; a++) worldCounter[a] = 0;
	
	#if C4DIAGNOSTICS
	
		diagnosticFlags = 0;
	
	#endif
	
	ProcessWorldProperties();
	
	rootNode->SetWorld(this);
	rootNode->InitTransform();
	rootNode->Preprocess();
	rootNode->Update();
	
	return (kWorldOkay);
}

void World::ProcessWorldProperties(void)
{
	worldSkybox = nullptr;
	clearColor = nullptr;
	
	Node *node = rootNode->GetFirstSubnode();
	while (node)
	{
		if (node->GetNodeType() == kNodeSkybox)
		{
			worldSkybox = static_cast<Skybox *>(node);
			break;
		}
		
		node = node->Next();
	}
	
	const Property *property = rootNode->GetFirstProperty();
	while (property)
	{
		PropertyType type = property->GetPropertyType();
		if (type == kPropertyClear)
		{
			worldFlags |= kWorldClearColor;
			clearColor = &static_cast<const ClearProperty *>(property)->GetClearColor();
		}
		
		property = property->Next();
	}
}

void World::ExpandInstancedWorlds(Node *root, int32 depth)
{
	Node *node = root->GetFirstSubnode();
	while (node)
	{
		if (node->GetNodeType() == kNodeInstance)
		{
			Node *next = root->GetNextLevelNode(node);
			
			if (static_cast<Instance *>(node)->Expand(this))
			{
				if (depth < kWorldMaxInstanceDepth) ExpandInstancedWorlds(node, depth + 1);
			}
			
			node = next;
			continue;
		}
		
		node = root->GetNextNode(node);
	}
}

Node *World::NewInstancedWorld(const char *name, Node::CloneFilterProc *filterProc, void *filterCookie)
{
	unsigned_int32 hash = Text::GetTextHash(name);
	InstancedWorldData *data = instancedWorldDataMap.Find(hash);
	if (data) return (data->GetPrototypeCopy()->Clone(filterProc, filterCookie));
	
	WorldResource *resource = WorldResource::Get(name);
	if (!resource) return (nullptr);
	
	Node *node = Node::UnpackTree(resource->GetData(), kUnpackNonpersistent | kUnpackExternal);
	resource->Release();
	
	data = new InstancedWorldData(hash, node);
	instancedWorldDataMap.Insert(data);
	return (node->Clone(filterProc, filterCookie));
}

Node *World::NewGenericModel(const char *name, GenericModel *model)
{
	unsigned_int32 hash = Text::GetTextHash(name);
	GenericModelData *data = genericModelDataMap.Find(hash);
	if (data)
	{
		Node *node = data->GetGenericModel();
		data->AddGenericModel(model);
		if (node) return (node->Clone());
	}
	
	ModelResource *resource = ModelResource::Get(name);
	if (!resource) return (nullptr);
	
	Node *node = Node::UnpackTree(resource->GetData(), kUnpackNonpersistent | kUnpackExternal);
	resource->Release();
	
	if (!data) genericModelDataMap.Insert(new GenericModelData(hash, model));
	return (node);
}

ImpostorSystem *World::GetImpostorSystem(MaterialObject *material, const float *clipData)
{
	ImpostorSystem *system = impostorSystemMap.Find(material);
	if (system) return (system);

	system = new ImpostorSystem(material, clipData);
	impostorSystemMap.Insert(system);
	return (system);
}

void World::AddController(Controller *controller)
{
	unsigned_int32 flags = controller->GetControllerFlags();
	if (!(flags & kControllerAsleep)) controller->Wake();
	
	if (!(flags & kControllerLocal))
	{
		int32 count = controllerArray.GetElementCount();
		int32 index = controller->GetControllerIndex();
		
		if (index == kControllerUnassigned)
		{
			index = NewControllerIndex();
			controller->SetControllerIndex(index);
		}
		else if (index >= count)
		{
			controllerArray.SetElementCount(index + 1);
			for (machine a = count; a < index; a++) controllerArray[a].controller = nullptr;
		}
		
		controllerArray[index].controller = controller;
	}
}

void World::RemoveController(Controller *controller)
{
	int32 index = controller->GetControllerIndex();
	if ((unsigned_int32) index < (unsigned_int32) controllerArray.GetElementCount())
	{
		ControllerData *data = &controllerArray[index];
		data->controller = nullptr;
		
		if (index >= staticControllerCount)
		{
			int32 last = lastFreeControllerIndex;
			if (last != kControllerUnassigned)
			{
				lastFreeControllerIndex = index;
				data->prevControllerIndex = last;
				data->nextControllerIndex = kControllerUnassigned;
				controllerArray[last].nextControllerIndex = index;
			}
			else
			{
				firstFreeControllerIndex = index;
				lastFreeControllerIndex = index;
				data->prevControllerIndex = kControllerUnassigned;
				data->nextControllerIndex = kControllerUnassigned;
			}
		}
	}
	
	List<Controller> *list = controller->GetOwningList();
	if (list) list->Remove(controller);
}

void World::WakeController(Controller *controller)
{
	if (!controller->GetOwningList())
	{
		if (controller->GetControllerType() == kControllerPhysics)
		{
			physicsControllerList.Append(controller);
		}
		else
		{
			if (!(controller->GetControllerFlags() & kControllerMoveInhibit)) controllerList[controllerParity].Append(controller);
		}
	}
}

void World::SleepController(Controller *controller)
{
	List<Controller> *list = controller->GetOwningList();
	if (list) list->Remove(controller);
}

Controller *World::GetController(int32 index) const
{
	if ((unsigned_int32) index < (unsigned_int32) controllerArray.GetElementCount()) return (controllerArray[index].controller);
	return (nullptr);
}

int32 World::NewControllerIndex(void)
{
	int32 first = firstFreeControllerIndex;
	if (first != kControllerUnassigned)
	{
		int32 next = controllerArray[first].nextControllerIndex;
		firstFreeControllerIndex = next;
		
		if (next != kControllerUnassigned) controllerArray[next].prevControllerIndex = kControllerUnassigned;
		else lastFreeControllerIndex = kControllerUnassigned;
		
		return (first);
	}
	
	int32 count = controllerArray.GetElementCount();
	controllerArray.SetElementCount(count + 1);
	controllerArray[count].controller = nullptr;
	return (count);
}

ControllerMessage *World::ConstructControllerMessage(ControllerMessageType controllerMessageType, int32 controllerIndex, Decompressor& data, void *world)
{
	Controller *controller = static_cast<World *>(world)->GetController(controllerIndex);
	if (controller) return (controller->ConstructMessage(controllerMessageType));
	
	return (nullptr);
}

void World::ReceiveControllerMessage(const ControllerMessage *message, void *world)
{
	Controller *controller = static_cast<World *>(world)->GetController(message->GetControllerIndex());
	if (controller)
	{
		unsigned_int32 flags = message->GetMessageFlags();
		if ((flags & (kMessageDestroyer | kMessageJournaled)) == kMessageJournaled)
		{
			ControllerMessage *journaledMessage = controller->GetFirstJournaledMessage();
			while (journaledMessage)
			{
				ControllerMessage *next = journaledMessage->Next();
				if (message->OverridesMessage(journaledMessage)) delete journaledMessage;
				journaledMessage = next;
			}
			
			controller->AddJournaledMessage(const_cast<ControllerMessage *>(message));
		}
		
		if (!message->HandleControllerMessage(controller)) controller->ReceiveMessage(message);
	}
}

void World::SetCameraClearParams(CameraObject *object) const
{
	if (worldFlags & kWorldClearColor)
	{
		if (clearColor) object->SetClearColor(*clearColor);
		object->SetClearFlags(kClearColorBuffer | kClearDepthBuffer | kClearStencilBuffer);
	}
	else
	{
		object->SetClearFlags(kClearDepthBuffer | kClearStencilBuffer);
	}
}

void World::SetCamera(FrustumCamera *camera)
{
	currentCamera = camera;
	listenerZone = nullptr;
	
	unsigned_int32 flags = worldFlags;
	if (camera)
	{
		camera->SetWorld(this);
		if (!(flags & kWorldListenerInhibit)) TheSoundMgr->SetListenerTransformable(camera);
		
		FrustumCameraObject *object = camera->GetObject();
		SetCameraClearParams(object);
		
		object->SetFrustumFlags(kFrustumInfinite);
		object->SetNearDepth(kCameraNearDepth);
		object->SetFarDepth(kCameraFarDepth);
	}
	else
	{
		if (!(flags & kWorldListenerInhibit)) TheSoundMgr->SetListenerTransformable(nullptr);
	}
}

C4::Zone *World::FindZone(Zone *root, const Point3D& position, Zone **transition)
{
	const ZoneObject *object = root->GetObject();
	if (!object->ExteriorSphere(root->GetInverseWorldTransform() * position, 0.0F))
	{
		if (object->GetZoneFlags() & kZoneTransition)
		{
			*transition = root;
			return (nullptr);
		}
		
		Zone *subzone = root->GetFirstSubzone();
		while (subzone)
		{
			Zone *zone = FindZone(subzone, position, transition);
			if (zone) return (zone);
			
			subzone = subzone->Next();
		}
		
		return (root);
	}
	
	return (nullptr);
}

C4::Zone *World::FindZone(const Point3D& position, bool remapTransition) const
{
	Zone *transition = nullptr;
	Zone *zone = FindZone(GetRootNode(), position, &transition);
	
	if ((transition) && (zone == transition->GetOwningZone()))
	{
		zone = (remapTransition) ? transition->GetTransitionMapping() : transition;
	}
	
	return (zone);
}

bool World::DetectGeometryCollision(Geometry *geometry, const CollisionParams *collisionParams, CollisionData *collisionData, QueryThreadData *threadData)
{
	const GeometryObject *object = geometry->GetObject();
	if ((object->GetCollisionExclusionMask() & collisionParams->collisionKind) == 0)
	{
		if (threadData->AddGeometry(geometry))
		{
			GeometryHitData		geometryHitData;
			
			const Transform4D& inverseTransform = geometry->GetInverseWorldTransform();
			if (object->DetectCollision(inverseTransform * *collisionParams->colliderPosition[0], inverseTransform * *collisionParams->colliderPosition[1], collisionParams->colliderRadius, &geometryHitData))
			{
				float t = geometryHitData.param;
				if (t < collisionData->param)
				{
					collisionData->param = t;
					collisionData->position = geometry->GetWorldTransform() * geometryHitData.position;
					collisionData->normal = geometryHitData.normal * inverseTransform;
					collisionData->geometry = geometry;
					collisionData->triangleIndex = geometryHitData.triangleIndex;
					return (true);
				}
			}
		}
	}
	
	return (false);
}

bool World::DetectNodeCollision(Node *node, const CollisionParams *collisionParams, CollisionData *collisionData, QueryThreadData *threadData)
{
	if (node->Enabled())
	{
		bool result = false;
		
		if (node->GetNodeType() == kNodeGeometry)
		{
			result = DetectGeometryCollision(static_cast<Geometry *>(node), collisionParams, collisionData, threadData);
		}
		
		const Bond *bond = node->GetFirstOutgoingEdge();
		while (bond)
		{
			Site *site = bond->GetFinishElement();
			if (site->GetWorldBoundingBox().Intersection(collisionParams->colliderBox))
			{
				result |= DetectNodeCollision(static_cast<Node *>(site), collisionParams, collisionData, threadData);
			}
			
			bond = bond->GetNextOutgoingEdge();
		}
		
		return (result);
	}
	
	return (false);
}

bool World::DetectCellCollision(const Site *cell, const CollisionParams *collisionParams, CollisionData *collisionData, QueryThreadData *threadData)
{
	bool result = false;
	
	const Bond *bond = cell->GetFirstOutgoingEdge();
	while (bond)
	{
		Site *site = bond->GetFinishElement();
		if (site->GetWorldBoundingBox().Intersection(collisionParams->colliderBox))
		{
			if (site->GetCellIndex() < 0)
			{
				Node *node = static_cast<Node *>(site);
				NodeType type = node->GetNodeType();
				if ((type & 0x00FFFFFF) != 0x00424C4B)
				{
					result |= DetectNodeCollision(node, collisionParams, collisionData, threadData);
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
								if (subnode->GetWorldBoundingBox().Intersection(collisionParams->colliderBox))
								{
									result |= DetectGeometryCollision(static_cast<Geometry *>(subnode), collisionParams, collisionData, threadData);
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
				result |= DetectCellCollision(site, collisionParams, collisionData, threadData);
			}
		}
		
		bond = bond->GetNextOutgoingEdge();
	}
	
	return (result);
}

bool World::DetectZoneCollision(Zone *zone, const CollisionParams *collisionParams, CollisionData *collisionData, QueryThreadData *threadData)
{
	const Point3D& p1 = *collisionParams->colliderPosition[0];
	const Transform4D& transform = zone->GetInverseWorldTransform();
	if (!zone->GetObject()->ExteriorSweptSphere(transform * p1, transform * (p1 + (*collisionParams->colliderPosition[1] - p1) * collisionData->param), collisionParams->colliderRadius))
	{
		bool result = false;
		
		if (DetectCellCollision(zone, collisionParams, collisionData, threadData))
		{
			collisionData->zone = zone;
			result = true;
		}
		
		Zone *subzone = zone->GetFirstSubzone();
		while (subzone)
		{
			result |= DetectZoneCollision(subzone, collisionParams, collisionData, threadData);
			subzone = subzone->Next();
		}
		
		return (result);
	}
	
	return (false);
}

bool World::DetectCollision(const Point3D& p1, const Point3D& p2, float radius, unsigned_int32 kind, CollisionData *collisionData, int32 threadIndex) const
{
	if (p1 != p2)
	{
		CollisionParams		collisionParams;
		
		#if C4SIMD
		
			float4 r = SimdLoadSmearScalar(&radius);
			float4 q1 = SimdLoadUnaligned(&p1.x);
			float4 q2 = SimdLoadUnaligned(&p2.x);
			collisionParams.colliderBox.Set(SimdSub(SimdMin(q1, q2), r), SimdAdd(SimdMax(q1, q2), r));
		
		#else
		
			collisionParams.colliderBox.min.Set(Fmin(p1.x, p2.x) - radius, Fmin(p1.y, p2.y) - radius, Fmin(p1.z, p2.z) - radius);
			collisionParams.colliderBox.max.Set(Fmax(p1.x, p2.x) + radius, Fmax(p1.y, p2.y) + radius, Fmax(p1.z, p2.z) + radius);
		
		#endif
		
		collisionParams.colliderPosition[0] = &p1;
		collisionParams.colliderPosition[1] = &p2;
		collisionParams.colliderRadius = radius;
		collisionParams.collisionKind = kind;
		
		QueryThreadData queryThreadData(1 << threadIndex);
		bool result = false;
		
		collisionData->param = 1.0F;
		if (DetectZoneCollision(GetRootNode(), &collisionParams, collisionData, &queryThreadData))
		{
			collisionData->normal.Normalize();
			collisionData->position -= collisionData->normal * radius;
			result = true;
		}
		
		queryThreadData.ResetGeometryArray();
		return (result);
	}
	
	return (false);
}

const AcousticsProperty *World::DetectObstruction(const Point3D& position) const
{
	CollisionData	collisionData;
	
	const Point3D& listenerPosition = currentCamera->GetWorldPosition();
	if (DetectCollision(position, listenerPosition, 0.0F, kCollisionSoundPath, &collisionData))
	{
		return (static_cast<const AcousticsProperty *>(collisionData.geometry->GetProperty(kPropertyAcoustics)));
	}
	
	return (nullptr);
}

CollisionState World::QueryNodeCollision(Node *node, const CollisionParams *collisionParams, CollisionData *collisionData, QueryThreadData *threadData)
{
	if (node->Enabled())
	{
		CollisionState result = kCollisionStateNone;
		
		Controller *controller = node->GetController();
		if ((controller) && (controller->GetBaseControllerType() == kControllerRigidBody))
		{
			RigidBodyController *rigidBody = static_cast<RigidBodyController *>(controller);
			if (((rigidBody->GetCollisionExclusionMask() & collisionParams->collisionKind) == 0) && (rigidBody != collisionParams->excludedRigidBody))
			{
				BodyHitData		bodyHitData;
				
				if (rigidBody->DetectSegmentIntersection(*collisionParams->colliderPosition[0], *collisionParams->colliderPosition[1], collisionParams->colliderRadius, &bodyHitData))
				{
					float t = bodyHitData.param;
					if (t < collisionData->param)
					{
						result = kCollisionStateRigidBody;
						
						collisionData->param = t;
						collisionData->position = bodyHitData.position;
						collisionData->normal = bodyHitData.normal;
						collisionData->rigidBody = rigidBody;
						collisionData->shape = bodyHitData.shape;
					}
				}
			}
		}
		else if (node->GetNodeType() == kNodeGeometry)
		{
			if (DetectGeometryCollision(static_cast<Geometry *>(node), collisionParams, collisionData, threadData)) result = kCollisionStateGeometry;
		}
		
		const Bond *bond = node->GetFirstOutgoingEdge();
		while (bond)
		{
			Site *site = bond->GetFinishElement();
			if (site->GetWorldBoundingBox().Intersection(collisionParams->colliderBox))
			{
				CollisionState state = QueryNodeCollision(static_cast<Node *>(site), collisionParams, collisionData, threadData);
				if (state != kCollisionStateNone) result = state;
			}
			
			bond = bond->GetNextOutgoingEdge();
		}
		
		return (result);
	}
	
	return (kCollisionStateNone);
}

CollisionState World::QueryCellCollision(const Site *cell, const CollisionParams *collisionParams, CollisionData *collisionData, QueryThreadData *threadData)
{
	CollisionState result = kCollisionStateNone;
	
	const Bond *bond = cell->GetFirstOutgoingEdge();
	while (bond)
	{
		Site *site = bond->GetFinishElement();
		if (site->GetWorldBoundingBox().Intersection(collisionParams->colliderBox))
		{
			if (site->GetCellIndex() < 0)
			{
				Node *node = static_cast<Node *>(site);
				NodeType type = node->GetNodeType();
				if ((type & 0x00FFFFFF) != 0x00424C4B)
				{
					CollisionState state = QueryNodeCollision(node, collisionParams, collisionData, threadData);
					if (state != kCollisionStateNone) result = state;
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
								if (subnode->GetWorldBoundingBox().Intersection(collisionParams->colliderBox))
								{
									if (DetectGeometryCollision(static_cast<Geometry *>(subnode), collisionParams, collisionData, threadData)) result = kCollisionStateGeometry;
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
				CollisionState state = QueryCellCollision(site, collisionParams, collisionData, threadData);
				if (state != kCollisionStateNone) result = state;
			}
		}
		
		bond = bond->GetNextOutgoingEdge();
	}
	
	return (result);
}

CollisionState World::QueryZoneCollision(Zone *zone, const CollisionParams *collisionParams, CollisionData *collisionData, QueryThreadData *threadData)
{
	const Point3D& p1 = *collisionParams->colliderPosition[0];
	const Transform4D& transform = zone->GetInverseWorldTransform();
	if (!zone->GetObject()->ExteriorSweptSphere(transform * p1, transform * (p1 + (*collisionParams->colliderPosition[1] - p1) * collisionData->param), collisionParams->colliderRadius))
	{
		CollisionState result = kCollisionStateNone;
		
		CollisionState state = QueryCellCollision(zone, collisionParams, collisionData, threadData);
		if (state != kCollisionStateNone)
		{
			collisionData->zone = zone;
			result = state;
		}
		
		Zone *subzone = zone->GetFirstSubzone();
		while (subzone)
		{
			state = QueryZoneCollision(subzone, collisionParams, collisionData, threadData);
			if (state != kCollisionStateNone) result = state;
			
			subzone = subzone->Next();
		}
		
		return (result);
	}
	
	return (kCollisionStateNone);
}

CollisionState World::QueryCollision(const Point3D& p1, const Point3D& p2, float radius, unsigned_int32 kind, CollisionData *collisionData, const RigidBodyController *excludeBody, int32 threadIndex) const
{
	if (p1 != p2)
	{
		CollisionParams		collisionParams;
		
		#if C4SIMD
		
			float4 r = SimdLoadSmearScalar(&radius);
			float4 q1 = SimdLoadUnaligned(&p1.x);
			float4 q2 = SimdLoadUnaligned(&p2.x);
			collisionParams.colliderBox.Set(SimdSub(SimdMin(q1, q2), r), SimdAdd(SimdMax(q1, q2), r));
		
		#else
		
			collisionParams.colliderBox.min.Set(Fmin(p1.x, p2.x) - radius, Fmin(p1.y, p2.y) - radius, Fmin(p1.z, p2.z) - radius);
			collisionParams.colliderBox.max.Set(Fmax(p1.x, p2.x) + radius, Fmax(p1.y, p2.y) + radius, Fmax(p1.z, p2.z) + radius);
		
		#endif
		
		collisionParams.excludedRigidBody = excludeBody;
		collisionParams.colliderPosition[0] = &p1;
		collisionParams.colliderPosition[1] = &p2;
		collisionParams.colliderRadius = radius;
		collisionParams.collisionKind = kind;
		
		QueryThreadData queryThreadData(1 << threadIndex);
		CollisionState result = kCollisionStateNone;
		
		collisionData->param = 1.0F;
		CollisionState state = QueryZoneCollision(GetRootNode(), &collisionParams, collisionData, &queryThreadData);
		if (state != kCollisionStateNone)
		{
			collisionData->normal.Normalize();
			collisionData->position -= collisionData->normal * radius;
			result = state;
		}
		
		queryThreadData.ResetGeometryArray();
		return (result);
	}
	
	return (kCollisionStateNone);
}

ProximityResult World::QueryNodeProximity(Node *node, const ProximityParams *proximityParams)
{
	if (node->Enabled())
	{
		if (node->GetNodeType() == kNodeGeometry)
		{
			ProximityResult result = (*proximityParams->proximityProc)(node, *proximityParams->proximityCenter, proximityParams->proximityRadius, proximityParams->proximityCookie);
			if (result == kProximityStop) return (kProximityStop);
			if (result == kProximitySkipSuccessors) goto end;
		}
		else
		{
			Controller *controller = node->GetController();
			if ((controller) && (controller->GetBaseControllerType() == kControllerRigidBody))
			{
				ProximityResult result = (*proximityParams->proximityProc)(node, *proximityParams->proximityCenter, proximityParams->proximityRadius, proximityParams->proximityCookie);
				if (result == kProximityStop) return (kProximityStop);
				if (result == kProximitySkipSuccessors) goto end;
			}
		}
		
		const Bond *bond = node->GetFirstOutgoingEdge();
		while (bond)
		{
			const Bond *next = bond->GetNextOutgoingEdge();
			
			Site *site = bond->GetFinishElement();
			if (site->GetWorldBoundingBox().Intersection(proximityParams->proximityBox))
			{
				if (QueryNodeProximity(static_cast<Node *>(site), proximityParams) == kProximityStop) return (kProximityStop);
			}
			
			bond = next;
		}
	}
	
	end:
	return (kProximityContinue);
}

ProximityResult World::QueryCellProximity(const Site *cell, const ProximityParams *proximityParams)
{
	const Bond *bond = cell->GetFirstOutgoingEdge();
	while (bond)
	{
		const Bond *next = bond->GetNextOutgoingEdge();
		
		Site *site = bond->GetFinishElement();
		if (site->GetWorldBoundingBox().Intersection(proximityParams->proximityBox))
		{
			if (site->GetCellIndex() < 0)
			{
				Node *node = static_cast<Node *>(site);
				NodeType type = node->GetNodeType();
				if ((type & 0x00FFFFFF) != 0x00424C4B)
				{
					if (QueryNodeProximity(node, proximityParams) == kProximityStop) return (kProximityStop);
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
								if (subnode->GetWorldBoundingBox().Intersection(proximityParams->proximityBox))
								{
									ProximityResult result = (*proximityParams->proximityProc)(subnode, *proximityParams->proximityCenter, proximityParams->proximityRadius, proximityParams->proximityCookie);
									if (result == kProximityStop) return (kProximityStop);
									if (result == kProximitySkipSuccessors) goto nextTerrain;
								}
								
								subnode = node->GetNextNode(subnode);
								continue;
							}
							
							nextTerrain:
							subnode = node->GetNextLevelNode(subnode);
						}
					}
				}
			}
			else
			{
				if (QueryCellProximity(site, proximityParams) == kProximityStop) return (kProximityStop);
			}
		}
		
		bond = next;
	}
	
	return (kProximityContinue);
}

ProximityResult World::QueryZoneProximity(Zone *zone, const ProximityParams *proximityParams)
{
	const Transform4D& transform = zone->GetInverseWorldTransform();
	if (!zone->GetObject()->ExteriorSphere(transform * *proximityParams->proximityCenter, proximityParams->proximityRadius))
	{
		if (QueryCellProximity(zone, proximityParams) == kProximityStop) return (kProximityStop);
		
		Zone *subzone = zone->GetFirstSubzone();
		while (subzone)
		{
			if (QueryZoneProximity(subzone, proximityParams) == kProximityStop) return (kProximityStop);
			subzone = subzone->Next();
		}
	}
	
	return (kProximityContinue);
}

void World::QueryProximity(const Point3D& center, float radius, ProximityProc *proc, void *cookie) const
{
	ProximityParams		proximityParams;
	
	#if C4SIMD
	
		float4 r = SimdLoadSmearScalar(&radius);
		float4 p = SimdLoadUnaligned(&center.x);
		proximityParams.proximityBox.Set(SimdSub(p, r), SimdAdd(p, r));
	
	#else
	
		proximityParams.proximityBox.min.Set(center.x - radius, center.y - radius, center.z - radius);
		proximityParams.proximityBox.max.Set(center.x + radius, center.y + radius, center.z + radius);
	
	#endif
	
	proximityParams.proximityProc = proc;
	proximityParams.proximityCookie = cookie;
	proximityParams.proximityCenter = &center;
	proximityParams.proximityRadius = radius;
	
	QueryZoneProximity(GetRootNode(), &proximityParams);
}

bool World::DetectNodeInteraction(Node *node, const Box3D& box, const Point3D& p1, const Point3D& p2, InteractionData *interactionData, QueryThreadData *threadData)
{
	if (node->Enabled())
	{
		bool result = false;
		
		if (node->GetNodeType() == kNodeGeometry)
		{
			Geometry *geometry = static_cast<Geometry *>(node);
			
			const GeometryObject *object = geometry->GetObject();
			if (!(object->GetCollisionExclusionMask() & kCollisionInteraction))
			{
				const Property *property = geometry->GetProperty(kPropertyInteraction);
				if ((property) && (!(property->GetPropertyFlags() & kPropertyDisabled)))
				{
					if (threadData->AddGeometry(geometry))
					{
						GeometryHitData		geometryHitData;
						
						const Transform4D& inverseTransform = geometry->GetInverseWorldTransform();
						if (object->DetectCollision(inverseTransform * p1, inverseTransform * p2, 0.0F, &geometryHitData))
						{
							float t = geometryHitData.param;
							if (t < interactionData->param)
							{
								interactionData->param = t;
								interactionData->position = geometry->GetWorldTransform() * geometryHitData.position;
								interactionData->interaction = geometry;
								result = true;
							}
						}
					}
				}
			}
		}
		
		const Bond *bond = node->GetFirstOutgoingEdge();
		while (bond)
		{
			Site *site = bond->GetFinishElement();
			if (site->GetWorldBoundingBox().Intersection(box))
			{
				result |= DetectNodeInteraction(static_cast<Node *>(site), box, p1, p2, interactionData, threadData);
			}
			
			bond = bond->GetNextOutgoingEdge();
		}
		
		return (result);
	}
	
	return (false);
}

bool World::DetectCellInteraction(const Site *cell, const Box3D& box, const Point3D& p1, const Point3D& p2, InteractionData *interactionData, QueryThreadData *threadData)
{
	bool result = false;
	
	const Bond *bond = cell->GetFirstOutgoingEdge();
	while (bond)
	{
		Site *site = bond->GetFinishElement();
		if (site->GetWorldBoundingBox().Intersection(box))
		{
			if (site->GetCellIndex() < 0) result |= DetectNodeInteraction(static_cast<Node *>(site), box, p1, p2, interactionData, threadData);
			else result |= DetectCellInteraction(site, box, p1, p2, interactionData, threadData);
		}
		
		bond = bond->GetNextOutgoingEdge();
	}
	
	return (result);
}

bool World::DetectPanelEffectInteraction(const Zone *zone, const Box3D& box, const Point3D& p1, const Point3D& p2, InteractionData *interactionData)
{
	bool result = false;
	
	Bond *bond = zone->GetEffectSite()->GetFirstOutgoingEdge();
	while (bond)
	{
		Node *node = static_cast<Node *>(bond->GetFinishElement());
		if ((node->Enabled()) && (node->GetWorldBoundingBox().Intersection(box)))
		{
			Effect *effect = static_cast<Effect *>(node);
			if (effect->GetEffectType() == kEffectPanel)
			{
				const Property *property = effect->GetProperty(kPropertyInteraction);
				if ((property) && (!(property->GetPropertyFlags() & kPropertyDisabled)))
				{
					CollisionPoint		collisionPoint;
					
					const Transform4D& inverseTransform = effect->GetInverseWorldTransform();
					if (static_cast<PanelEffect *>(effect)->DetectCollision(inverseTransform * p1, inverseTransform * p2, &collisionPoint))
					{
						float t = collisionPoint.param;
						if (t < interactionData->param)
						{
							interactionData->param = t;
							interactionData->position = p1 + (p2 - p1) * t;
							interactionData->interaction = effect;
							result = true;
						}
					}
				}
			}
		}
		
		bond = bond->GetNextOutgoingEdge();
	}
	
	return (result);
}

bool World::DetectZoneInteraction(const Zone *zone, const Box3D& box, const Point3D& p1, const Point3D& p2, InteractionData *interactionData, QueryThreadData *threadData)
{
	const Transform4D& transform = zone->GetInverseWorldTransform();
	if (!zone->GetObject()->ExteriorSweptSphere(transform * p1, transform * (p1 + (p2 - p1) * interactionData->param), 0.0F))
	{
		bool result = DetectCellInteraction(zone, box, p1, p2, interactionData, threadData);
		result |= DetectPanelEffectInteraction(zone, box, p1, p2, interactionData);
		
		const Zone *subzone = zone->GetFirstSubzone();
		while (subzone)
		{
			result |= DetectZoneInteraction(subzone, box, p1, p2, interactionData, threadData);
			subzone = subzone->Next();
		}
		
		return (result);
	}
	
	return (false);
}

bool World::DetectInteraction(const Point3D& p1, const Point3D& p2, InteractionData *interactionData) const
{
	Box3D	box;
	
	#if C4SIMD
	
		float4 q1 = SimdLoadUnaligned(&p1.x);
		float4 q2 = SimdLoadUnaligned(&p2.x);
		box.Set(SimdMin(q1, q2), SimdMax(q1, q2));
	
	#else
	
		box.min.Set(Fmin(p1.x, p2.x), Fmin(p1.y, p2.y), Fmin(p1.z, p2.z));
		box.max.Set(Fmax(p1.x, p2.x), Fmax(p1.y, p2.y), Fmax(p1.z, p2.z));
	
	#endif
	
	QueryThreadData threadData(1 << JobMgr::kMaxWorkerThreadCount);
	bool result = DetectZoneInteraction(GetRootNode(), box, p1, p2, interactionData, &threadData);
	
	threadData.ResetGeometryArray();
	return (result);
}

void World::ActivateCellTriggers(Site *cell, const Box3D& box, const Point3D& p1, const Point3D& p2, float radius, Node *activator)
{
	const Bond *bond = cell->GetFirstOutgoingEdge();
	while (bond)
	{
		const Bond *next = bond->GetNextOutgoingEdge();
		
		Site *site = bond->GetFinishElement();
		if (site->GetWorldBoundingBox().Intersection(box))
		{
			if (site->GetCellIndex() < 0)
			{
				Trigger *trigger = static_cast<Trigger *>(site);
				
				unsigned_int32 stamp = triggerActivateStamp;
				if (trigger->GetNodeStamp() != stamp)
				{
					trigger->SetNodeStamp(stamp);
					
					if ((trigger->Enabled()) && (!trigger->ListElement<Trigger>::GetOwningList()))
					{
						const Transform4D& transform = trigger->GetInverseWorldTransform();
						const TriggerObject *object = trigger->GetObject();
						
						if (object->IntersectSegment(transform * p1, transform * p2, radius))
						{
							unsigned_int32 triggerFlags = object->GetTriggerFlags();
							if (triggerFlags & kTriggerActivateDisable) trigger->SetNodeFlags(trigger->GetNodeFlags() | kNodeDisabled);
							else if (!(triggerFlags & kTriggerContinuouslyActivated)) activeTriggerList[triggerParity ^ 1].Append(trigger);
							
							trigger->Activate(activator);
						}
					}
				}
			}
			else
			{
				ActivateCellTriggers(site, box, p1, p2, radius, activator);
			}
		}
		
		bond = next;
	}
}

void World::ActivateZoneTriggers(Zone *zone, const Point3D& p1, const Point3D& p2, float radius, Node *activator)
{
	const Transform4D& zoneTransform = zone->GetInverseWorldTransform();
	if (!zone->GetObject()->ExteriorSweptSphere(zoneTransform * p1, zoneTransform * p2, radius))
	{
		Box3D	box;
		
		#if C4SIMD
		
			float4 r = SimdLoadSmearScalar(&radius);
			float4 q1 = SimdLoadUnaligned(&p1.x);
			float4 q2 = SimdLoadUnaligned(&p2.x);
			box.Set(SimdSub(SimdMin(q1, q2), r), SimdAdd(SimdMax(q1, q2), r));
		
		#else
		
			box.min.Set(Fmin(p1.x, p2.x) - radius, Fmin(p1.y, p2.y) - radius, Fmin(p1.z, p2.z) - radius);
			box.max.Set(Fmax(p1.x, p2.x) + radius, Fmax(p1.y, p2.y) + radius, Fmax(p1.z, p2.z) + radius);
		
		#endif
		
		ActivateCellTriggers(zone->GetTriggerSite(), box, p1, p2, radius, activator);
		
		Zone *subzone = zone->GetFirstSubzone();
		while (subzone)
		{
			ActivateZoneTriggers(subzone, p1, p2, radius, activator);
			subzone = subzone->Next();
		}
	}
}

void World::ActivateTriggers(const Point3D& p1, const Point3D& p2, float radius, Node *activator)
{
	triggerActivateStamp++;
	ActivateZoneTriggers(GetRootNode(), p1, p2, radius, activator);
	
	List<Trigger> *triggerList = &activeTriggerList[triggerParity];
	Trigger *trigger = triggerList->First();
	while (trigger)
	{
		Trigger *next = trigger->Next();
		
		if (trigger->Enabled())
		{
			const Transform4D& transform = trigger->GetInverseWorldTransform();
			if (trigger->GetObject()->IntersectSegment(transform * p1, transform * p2, radius))
			{
				activeTriggerList[triggerParity ^ 1].Append(trigger);
			}
		}
		else
		{
			triggerList->Remove(trigger);
		}
		
		trigger = next;
	}
}

RigidBodyStatus World::HandleNewRigidBodyContact(RigidBodyController *rigidBody, const RigidBodyContact *contact, RigidBodyController *contactBody)
{
	return (kRigidBodyUnchanged);
}

RigidBodyStatus World::HandleNewGeometryContact(RigidBodyController *rigidBody, const GeometryContact *contact)
{
	return (kRigidBodyUnchanged);
}

void World::HandleWaterSubmergence(RigidBodyController *rigidBody)
{
}

void World::MoveControllers(unsigned_int32 parity)
{
	List<Controller> *currentList = &controllerList[parity];
	List<Controller> *nextList = &controllerList[parity ^ 1];
	
	for (;;)
	{
		Controller *controller = currentList->First();
		if (!controller) break;
		
		nextList->Append(controller);
		controller->Move();
	}
	
	Controller *controller = physicsControllerList.First();
	if (controller) static_cast<PhysicsController *>(controller)->PhysicsController::Move();
}

void World::MoveEffects(unsigned_int32 parity)
{
	List<Effect> *currentList = &activeEffectList[parity];
	List<Effect> *nextList = &activeEffectList[parity ^ 1];
	
	for (;;)
	{
		Effect *effect = currentList->First();
		if (!effect) break;
		
		nextList->Append(effect);
		effect->Move();
	}
}

void World::MoveSources(unsigned_int32 parity)
{
	List<Source> *currentList = &playingSourceList[parity];
	List<Source> *nextList = &playingSourceList[parity ^ 1];
	for (;;)
	{
		Source *source = currentList->First();
		if (!source) break;
		
		worldCounter[kWorldCounterPlayingSource]++;
		
		nextList->Append(source);
		source->Move();
	}
	
	Source *source = engagedSourceList.First();
	while (source)
	{
		worldCounter[kWorldCounterEngagedSource]++;
		
		Source *next = source->Next();
		source->Move();
		source = next;
	}
	
	worldCounter[kWorldCounterPlayingSource] += worldCounter[kWorldCounterEngagedSource];
}

void World::Move(void)
{

	for (machine a = 0; a < kWorldCounterCount; a++)
		worldCounter[a] = 0;
	
	unsigned_int8 parity = controllerParity;
	MoveControllers(parity);
	controllerParity = parity ^ 1;
	
	parity = effectParity;
	MoveEffects(parity);
	effectParity = parity ^ 1;
	
	parity = sourceParity;
	MoveSources(parity);
	sourceParity = parity ^ 1;
	
	parity = triggerParity;
	List<Trigger> *triggerList = &activeTriggerList[parity];
	for (;;)
	{
		Trigger *trigger = triggerList->First();
		if (!trigger) break;
		
		triggerList->Remove(trigger);
		trigger->Deactivate();
	}
	
	triggerParity = parity ^ 1;
}

void World::Update(void)
{
	rootNode->Update();
	
	FrustumCamera *camera = currentCamera;
	if (camera)
	{
		if (!(worldFlags & kWorldViewport))
		{
			FrustumCameraObject *object = camera->GetObject();
			object->SetViewRect(Rect(0, 0, renderWidth, renderHeight));
			object->SetAspectRatio((float) renderHeight / (float) renderWidth);
		}
		
		camera->Move();
		camera->Invalidate();
		camera->Update();
		
		cameraZone = FindZone(camera->GetWorldPosition(), true);
	}

	updateObservable.PostEvent();
}

void World::Interact(void)
{
	Interactor *interactor = interactorList.First();
	while (interactor)
	{
		interactor->DetectInteraction(this);
		interactor = interactor->Next();
	}
}

void World::Listen(void)
{
	const FrustumCamera *camera = currentCamera;
	if (camera)
	{
		const Point3D& listenerPosition = camera->GetWorldPosition();
		Zone *zone = cameraZone;
		
		if (!(worldFlags & kWorldListenerInhibit))
		{
			if (zone != listenerZone)
			{
				listenerZone = zone;
				const AcousticsSpace *acousticsSpace = zone->GetConnectedAcousticsSpace();
				TheSoundMgr->SetListenerRoom((acousticsSpace) ? acousticsSpace->GetSoundRoom() : nullptr);
			}
			
			Source *source = engagedSourceList.First();
			while (source)
			{
				static_cast<OmniSource *>(source)->BeginUpdate();
				source = source->Next();
			}
			
			SourceRegion *sourceRegion = zone->GetFirstSourceRegion();
			while (sourceRegion)
			{
				OmniSource *omniSource = static_cast<OmniSource *>(sourceRegion->GetSource());
				
				const Point3D& sourcePosition = sourceRegion->GetPermeatedPosition();
				if (Magnitude(sourcePosition - listenerPosition) + sourceRegion->GetPermeatedPathLength() < omniSource->GetSourceRange())
				{
					unsigned_int32 state = omniSource->sourceState;
					if (!(state & kSourceEngaged))
					{
						engagedSourceList.Append(omniSource);
						omniSource->BeginUpdate();
					}
					
					omniSource->sourceState = state | kSourceAudible;
					omniSource->AddPlayRegion(sourceRegion, listenerPosition);
				}
				
				sourceRegion = sourceRegion->GetNextSourceRegion();
			}
			
			source = engagedSourceList.First();
			while (source)
			{
				Source *next = source->Next();
				
				unsigned_int32 state = source->sourceState;
				if (state & kSourceAudible)
				{
					if (!(state & kSourceEngaged))
					{
						if (!source->Engage()) playingSourceList[sourceParity].Append(source);
					}
					else
					{
						static_cast<OmniSource *>(source)->EndUpdate();
					}
				}
				else
				{
					source->Disengage();
					playingSourceList[sourceParity].Append(source);
				}
				
				source = next;
			}
		}
	}
}

#if C4DIAGNOSTICS

	void World::RenderSourcePaths(Zone *zone, const Transform4D& listenerTransform)
	{
		const SourceRegion *sourceRegion = zone->GetFirstSourceRegion();
		while (sourceRegion)
		{
			const Source *source = sourceRegion->GetSource();
			if (source->sourceState & kSourceEngaged)
			{
				const SourceRegion *region = sourceRegion->GetPrimaryRegion();
				
				sourcePathVertex[0] = listenerTransform.GetTranslation() + listenerTransform[2];
				sourcePathVertex[1] = region->GetAudiblePosition();
				TheGraphicsMgr->DrawRenderList(&sourcePathRenderList);
				
				const SourceRegion *superRegion = region->GetSuperNode();
				while (superRegion)
				{
					const SourceRegion *nextRegion = superRegion->GetSuperNode();
					if (superRegion->GetAudibleSubregion() == region)
					{
						sourcePathVertex[0] = region->GetAudiblePosition();
						sourcePathVertex[1] = superRegion->GetAudiblePosition();
						TheGraphicsMgr->DrawRenderList(&sourcePathRenderList);
						
						region = superRegion;
					}
					
					superRegion = nextRegion;
				}
			}
			
			sourceRegion = sourceRegion->GetNextSourceRegion();
		}
	}

#endif

bool World::NodeExcluded(const Node *node, const Node *exclude)
{
	do
	{
		if (node == exclude) return (true);
		
		NodeType type = node->GetNodeType();
		if ((type != kNodeGeometry) && (type != kNodeBone)) break;
		
		node = node->GetSuperNode();
	} while (node);
	
	return (false);
}

bool World::WorldBoundingBoxVisible(const Box3D& box, const Region *region, const List<Region> *occlusionList)
{
	if (!region->BoxVisible(box)) return (false);
	
	const Region *occluder = occlusionList->First();
	while (occluder)
	{
		if (occluder->BoxOccluded(box)) return (false);
		occluder = occluder->Next();
	}
	
	return (true);
}

bool World::ShadowNodeVisible(const Node *node, const List<Region> *shadowRegionList)
{
	const Region *region = shadowRegionList->First();
	while (region)
	{
		if (node->Visible(region)) return (true);
		region = region->Next();
	}
	
	return (false);
}

bool World::ShadowCellVisible(const Site *cell, const List<Region> *shadowRegionList)
{
	const Region *region = shadowRegionList->First();
	while (region)
	{
		if (region->BoxVisible(cell->GetWorldBoundingBox())) return (true);
		region = region->Next();
	}
	
	return (false);
}

void World::SetNodeFogState(const WorldContext *worldContext, const Node *node, Renderable *renderable)
{
	unsigned_int32 renderableFlags = renderable->GetRenderableFlags() & ~kRenderableUnfog;
	
	const Region *region = worldContext->unfoggedList.First();
	if ((region) && (node->Occluded(region))) renderableFlags |= kRenderableUnfog;
	
	renderable->SetRenderableFlags(renderableFlags);
}

void World::ProcessGeometry(const WorldContext *worldContext, Geometry *geometry)
{
	const GeometryObject *object = geometry->GetObject();
	int32 geometryLevelCount = object->GetGeometryLevelCount();
	if ((geometryLevelCount > 1) || (object->GetGeometryFlags() & kGeometryShaderDetailEnable))
	{
		int32 minLevel = Max(geometry->GetMinDetailLevel(), worldContext->cameraMinDetailLevel);
		
		const BoundingSphere *sphere = geometry->GetBoundingSphere();
		const Point3D& center = sphere->GetCenter();
		
		const FrustumCamera *camera = worldContext->renderCamera;
		Vector3D direction = center - camera->GetWorldPosition();
		float d = Magnitude(direction);
		if ((d > kDetailEpsilon) && (camera->GetWorldTransform()[2] * direction > 0.0F))
		{
			float focalLength = static_cast<FrustumCameraObject *>(camera->Node::GetObject())->GetFocalLength();
			float r = sphere->GetRadius() * focalLength / d;
			float t = worldContext->cameraDetailBias - Log(r);
			
			int32 level = Min(Max((int32) (t - 1.5F + object->GetGeometryDetailBias()), minLevel), geometryLevelCount - 1);
			if (geometry->GetDetailLevel() != level) geometry->SetDetailLevel(level);
			
			if (object->GetGeometryFlags() & kGeometryShaderDetailEnable)
			{
				float u = t + 1.0F + object->GetShaderDetailBias();
				geometry->SetShaderDetailLevel(Max((int32) u, minLevel));
				geometry->SetShaderDetailParameter(FmaxZero(Fmin(1.0F - u, 1.0F)));
			}
		}
		else
		{
			if (geometry->GetDetailLevel() != 0) geometry->SetDetailLevel(0);
			
			geometry->SetShaderDetailLevel(0);
			geometry->SetShaderDetailParameter(1.0F);
		}
	}
	
	Controller *controller = geometry->GetController();
	if ((controller) && (controller->GetControllerFlags() & kControllerUpdate)) controller->Update();
}

void World::UpdateGeometry(Geometry *geometry)
{
	if (geometry->GetNodeStamp() != geometryRenderStamp)
	{
		ProcessGeometry(currentWorldContext, geometry);
	}
}

void World::RenderEffects(const WorldContext *worldContext, CameraRegion *cameraRegion)
{
	Bond *bond = cameraRegion->GetZone()->GetEffectSite()->GetFirstOutgoingEdge();
	while (bond)
	{
		Bond *next = bond->GetNextOutgoingEdge();
		
		Node *node = static_cast<Node *>(bond->GetFinishElement());
		if ((node->Enabled()) && (NodeVisible(node, cameraRegion, &worldContext->occlusionList)))
		{
			Effect *effect = static_cast<Effect *>(node);
			if (!effect->ListElement<Renderable>::GetOwningList())
			{
				SetNodeFogState(worldContext, effect, effect);
				effect->Render(cameraRegion->GetCamera(), &renderStageList[kRenderStageFirstEffect]);
				if ((effect->GetEffectListIndex() == kEffectListLight) && (effect->ListElement<Renderable>::GetOwningList())) cameraRegion->AddEffect(effect);
			}
		}
		
		bond = next;
	}
}

void World::RenderAmbientGeometry(const WorldContext *worldContext, Geometry *geometry, CameraRegion *cameraRegion)
{
	unsigned_int32 flags = geometry->GetObject()->GetGeometryFlags();
	if ((!(flags & kGeometryInvisible)) && ((geometry->GetPerspectiveExclusionMask() & worldContext->perspectiveFlags) == 0))
	{
		unsigned_int32 renderStamp = geometryRenderStamp;
		if (geometry->GetNodeStamp() != renderStamp)
		{
			geometry->SetNodeStamp(renderStamp);
			worldCounter[kWorldCounterGeometry]++;
			
			ProcessGeometry(worldContext, geometry);
			SetNodeFogState(worldContext, geometry, geometry);
			
			if ((flags & (kGeometryAmbientOnly | kGeometryRenderEffectPass)) == 0) cameraRegion->AddGeometry(geometry);
			renderStageList[geometry->GetGeometryRenderStage()].Append(geometry);
		}
	}
}

void World::RenderAmbientNode(const WorldContext *worldContext, Node *node, CameraRegion *cameraRegion)
{
	NodeType type = node->GetNodeType();
	if ((type != kNodeZone) && (node->Enabled()) && (NodeVisible(node, cameraRegion, &worldContext->occlusionList)))
	{
		if ((type & 0x00FFFFFF) != 0x00424C4B)
		{
			if (type == kNodeGeometry)
			{
				RenderAmbientGeometry(worldContext, static_cast<Geometry *>(node), cameraRegion);
			}
			else if (type == kNodeImpostor)
			{
				Impostor *impostor = static_cast<Impostor *>(node);
				
				float distance = SquaredMag(impostor->GetWorldPosition().GetVector2D() - worldContext->renderCamera->GetWorldPosition().GetVector2D());
				if (distance > impostor->GetSquaredRenderDistance())
				{
					unsigned_int32 renderStamp = impostorRenderStamp;
					if (impostor->GetNodeStamp() != renderStamp)
					{
						impostor->SetNodeStamp(renderStamp);
						worldCounter[kWorldCounterImpostor]++;
						
						cameraRegion->AddImpostor(impostor);
						impostor->Render();
					}
					
					if (distance > impostor->GetSquaredGeometryDistance()) return;
				}
			}
			
			const Bond *bond = node->GetFirstOutgoingEdge();
			while (bond)
			{
				RenderAmbientNode(worldContext, static_cast<Node *>(bond->GetFinishElement()), cameraRegion);
				bond = bond->GetNextOutgoingEdge();
			}
		}
		else
		{
			if (type == kNodeTerrainBlock)
			{
				const FrustumCamera *camera = static_cast<const FrustumCamera *>(cameraRegion->GetCamera());
				float inverseFocal = 1.0F / camera->GetObject()->GetFocalLength();
				
				Node *subnode = node->GetFirstSubnode();
				while (subnode)
				{
					if (subnode->GetNodeType() == kNodeGeometry)
					{
						TerrainGeometry *terrain = static_cast<TerrainGeometry *>(subnode);
						if (WorldBoundingBoxVisible(terrain->GetWorldBoundingBox(), cameraRegion, &worldContext->occlusionList))
						{
							unsigned_int32 renderStamp = geometryRenderStamp;
							if (terrain->GetNodeStamp() != renderStamp)
							{
								terrain->SetNodeStamp(renderStamp);
								
								const TerrainGeometryObject *object = terrain->GetObject();
								int32 level = object->GetDetailLevel();
								
								if (level <= worldContext->cameraMinDetailLevel)
								{
									if ((!(object->GetGeometryFlags() & kGeometryInvisible)) && ((terrain->GetPerspectiveExclusionMask() & worldContext->perspectiveFlags) == 0))
									{
										worldCounter[kWorldCounterTerrain]++;
										
										SetNodeFogState(worldContext, terrain, terrain);
										cameraRegion->AddGeometry(terrain);
										renderStageList[kRenderStageDefault].Append(terrain);
										
										if (level != 0) terrainList.Append(static_cast<TerrainLevelGeometry *>(terrain));
									}
									
									subnode = node->GetNextLevelNode(terrain);
									continue;
								}
								else
								{
									Vector3D direction = terrain->GetWorldCenter() - camera->GetWorldPosition();
									float d = Magnitude(direction) * inverseFocal;
									if ((d > terrain->GetRenderDistance()) && (camera->GetWorldTransform()[2] * direction > 0.0F))
									{
										if ((!(object->GetGeometryFlags() & kGeometryInvisible)) && ((terrain->GetPerspectiveExclusionMask() & worldContext->perspectiveFlags) == 0))
										{
											worldCounter[kWorldCounterTerrain]++;
											
											SetNodeFogState(worldContext, terrain, terrain);
											cameraRegion->AddGeometry(terrain);
											terrainList.Append(static_cast<TerrainLevelGeometry *>(terrain));
											renderStageList[kRenderStageDefault].Append(terrain);
										}
										
										subnode = node->GetNextLevelNode(terrain);
										continue;
									}
								}
							}
							
							subnode = node->GetNextNode(subnode);
							continue;
						}
					}
					
					subnode = node->GetNextLevelNode(subnode);
				}
			}
			else
			{
				const Camera *camera = cameraRegion->GetCamera();
				
				Bond *bond = node->GetFirstOutgoingEdge();
				while (bond)
				{
					Geometry *geometry = static_cast<Geometry *>(bond->GetFinishElement());
					if (WorldBoundingBoxVisible(geometry->GetWorldBoundingBox(), cameraRegion, &worldContext->occlusionList))
					{
						unsigned_int32 renderStamp = geometryRenderStamp;
						if (geometry->GetNodeStamp() != renderStamp)
						{
							geometry->SetNodeStamp(renderStamp);
							
							if ((geometry->GetPerspectiveExclusionMask() & worldContext->perspectiveFlags) == 0)
							{
								worldCounter[kWorldCounterWater]++;
								
								SetNodeFogState(worldContext, geometry, geometry);
								if ((geometry->GetObject()->GetGeometryFlags() & (kGeometryAmbientOnly | kGeometryRenderEffectPass)) == 0) cameraRegion->AddGeometry(geometry);
								renderStageList[geometry->GetGeometryRenderStage()].Append(geometry);
								
								if (geometry->GetGeometryType() == kGeometryWater)
								{
									float d = SquaredMag(geometry->GetBoundingSphere()->GetCenter() - camera->GetWorldPosition());
									static_cast<WaterGeometry *>(geometry)->UpdateWater(d);
								}
							}
						}
					}
					else
					{
						if (geometry->GetGeometryType() == kGeometryWater)
						{
							float d = SquaredMag(geometry->GetBoundingSphere()->GetCenter() - camera->GetWorldPosition());
							static_cast<WaterGeometry *>(geometry)->UpdateInvisibleWater(d);
						}
					}
					
					bond = bond->GetNextOutgoingEdge();
				}
			}
		}
	}
}

void World::RenderAmbientCell(const WorldContext *worldContext, const Site *cell, CameraRegion *cameraRegion)
{
	if (WorldBoundingBoxVisible(cell->GetWorldBoundingBox(), cameraRegion, &worldContext->occlusionList))
	{
		const Bond *bond = cell->GetFirstOutgoingEdge();
		while (bond)
		{
			Site *site = bond->GetFinishElement();
			if (site->GetCellIndex() < 0) RenderAmbientNode(worldContext, static_cast<Node *>(site), cameraRegion);
			else RenderAmbientCell(worldContext, site, cameraRegion);
			
			bond = bond->GetNextOutgoingEdge();
		}
	}
}

void World::RenderAmbientRegion(WorldContext *worldContext, CameraRegion *rootRegion)
{
	const Zone *zone = rootRegion->GetZone();
	if (zone->GetObject()->GetZoneFlags() & kZoneRenderSkybox) worldContext->skyboxFlag = true;
	
	Region *unfoggedRegion = worldContext->unfoggedList.First();
	if ((unfoggedRegion) && (!zone->GetFirstFogSpace()) && (!zone->GetConnectedFogSpace())) worldContext->unfoggedList.Remove(unfoggedRegion);
	
	bool transition = ((zone->GetObject()->GetZoneFlags() & kZoneTransition) != 0);
	if ((!transition) || (NodeVisible(zone, rootRegion, &worldContext->occlusionList)))
	{
		const Bond *bond = zone->GetFirstOutgoingEdge();
		while (bond)
		{
			Site *site = bond->GetFinishElement();
			if (site->GetCellIndex() < 0) RenderAmbientNode(worldContext, static_cast<Node *>(site), rootRegion);
			else RenderAmbientCell(worldContext, site, rootRegion);
			
			bond = bond->GetNextOutgoingEdge();
		}
	}
	
	RenderEffects(worldContext, rootRegion);
	
	if (unfoggedRegion) worldContext->unfoggedList.Append(unfoggedRegion);
	
	CameraRegion *subregion = rootRegion->GetFirstSubnode();
	while (subregion)
	{
		RenderAmbientRegion(worldContext, subregion);
		subregion = subregion->Tree<CameraRegion>::Next();
	}
}

void World::RenderInfiniteShadowVolume(Geometry *geometry, InfiniteLight *light, StencilMode stencilMode)
{
	int32 detailLevel = geometry->GetDetailLevel();
	const GeometryObject *geometryObject = geometry->GetObject();
	const GeometryLevel *geometryLevel = geometryObject->GetGeometryLevel(detailLevel);
	
	int32 edgeCount = geometryLevel->GetArrayDescriptor(kArrayEdge)->elementCount;
	if ((stencilMode == kStencilFail) || (edgeCount != 0))
	{
		StencilData *stencilData = geometry->GetStencilData();
		StencilVolume *stencilVolume = nullptr;
		
		bool buildExtrusion = true;
		bool buildEndcaps = (stencilMode == kStencilFail);
		
		unsigned_int32 lightFlags = light->GetObject()->GetLightFlags();
		if ((lightFlags & kLightStatic) && (!(geometryObject->GetGeometryFlags() & kGeometryDynamic)))
		{
			Link<StencilVolume> *stencilVolumeLink = geometry->GetStaticStencilVolume(light);
			if (stencilVolumeLink)
			{
				stencilVolume = *stencilVolumeLink;
				if (stencilVolume)
				{
					buildExtrusion = (stencilVolume->GetExtrusionDetailLevel() != detailLevel);
					if (buildEndcaps) buildEndcaps = (stencilVolume->GetEndcapDetailLevel() != detailLevel);
				}
				else
				{
					stencilVolume = new StencilVolume(geometry, light, stencilVolumeLink);
				}
				
				stencilData = stencilVolume;
			}
		}
		
		if (buildExtrusion) stencilData->CalculateInfiniteShadowBounds(light);
		
		const Vector4D& objectLightPosition = TheGraphicsMgr->SetGeometryTransformable(geometry->GetTransformable());
		if (TheGraphicsMgr->ActivateShadowBounds(stencilData))
		{
			if (buildExtrusion)
			{
				geometry->CalculateInfiniteShadowFrontArray(objectLightPosition.GetVector3D());
				
				const Point3D *vertex = static_cast<Point3D *>(geometry->GetArrayBundle(kArrayVertex)->pointer);
				const Edge *edge = geometryLevel->GetArray<Edge>(kArrayEdge);
				
				const bool *front = geometry->GetShadowFrontArray();
				Vector4D *restrict extrusionVertex = stencilData->GetExtrusionVertexArray();
				
				int32 extrusionEdgeCount = 0;
				for (machine a = 0; a < edgeCount; a++)
				{
					bool f1 = front[edge->faceIndex[0]];
					bool f2 = front[edge->faceIndex[1]];
					if (f1 ^ f2)
					{
						const Point3D& p1 = vertex[edge->vertexIndex[0]];
						const Point3D& p2 = vertex[edge->vertexIndex[1]];
						
						if (f1)
						{
							extrusionVertex[0] = p2;
							extrusionVertex[1] = p1;
						}
						else
						{
							extrusionVertex[0] = p1;
							extrusionVertex[1] = p2;
						}
						
						extrusionVertex[2].Set(0.0F, 0.0F, 0.0F, 0.0F);
						
						extrusionEdgeCount++;
						extrusionVertex += 3;
					}
					
					edge++;
				}
				
				stencilData->SetExtrusionEdgeCount(extrusionEdgeCount);
				if (stencilVolume) stencilVolume->SetExtrusionDetailLevel(detailLevel);
			}
			else
			{
				if (buildEndcaps) geometry->CalculateInfiniteShadowFrontArray(objectLightPosition.GetVector3D());
			}
			
			if (stencilData->GetExtrusionEdgeCount() != 0)
			{
				TheGraphicsMgr->DrawStencilShadow(stencilData, kStencilInfiniteExtrusion, stencilMode);
				
				if (stencilMode == kStencilFail)
				{
					if (buildEndcaps)
					{
						int32 frontEndcapTriangleCount = 0;
						Triangle *restrict frontEndcapTriangleArray = stencilData->GetFrontEndcapTriangleArray();
						
						int32 faceCount = geometryLevel->GetFaceCount();
						const Triangle *triangle = geometryLevel->GetArray<Triangle>(kArrayFace);
						
						const bool *front = geometry->GetShadowFrontArray();
						const unsigned_int16 *planeIndex = geometryLevel->GetArray<unsigned_int16>(kArrayPlaneIndex);
						if (planeIndex)
						{
							for (machine a = 0; a < faceCount; a++)
							{
								if (front[*planeIndex])
								{
									frontEndcapTriangleArray[frontEndcapTriangleCount] = *triangle;
									frontEndcapTriangleCount++;
								}
								
								triangle++;
								planeIndex++;
							}
						}
						else
						{
							for (machine a = 0; a < faceCount; a++)
							{
								if (front[a])
								{
									frontEndcapTriangleArray[frontEndcapTriangleCount] = *triangle;
									frontEndcapTriangleCount++;
								}
								
								triangle++;
							}
						}
						
						stencilData->SetFrontEndcapTriangleCount(frontEndcapTriangleCount);
						if (stencilVolume) stencilVolume->SetEndcapDetailLevel(detailLevel);
					}
					
					if (stencilData->GetFrontEndcapTriangleCount() != 0)
					{
						int32 vertexCount = geometryLevel->GetVertexCount();
						const Point3D *vertex = static_cast<Point3D *>(geometry->GetArrayBundle(kArrayVertex)->pointer);
						
						stencilData->SetGeometryVertexArray(vertexCount, vertex);
						TheGraphicsMgr->DrawStencilShadow(stencilData, kStencilEndcapIdentity, kStencilFail);
					}
				}
			}
		}
	}
}

void World::RenderInfiniteShadowNode(const WorldContext *worldContext, Node *node, const ShadowRenderData *renderData)
{
	NodeType type = node->GetNodeType();
	if ((type != kNodeZone) && (node->Enabled()) && (node->Visible(renderData->lightRegion)))
	{
		if (type == kNodeGeometry)
		{
			Geometry *geometry = static_cast<Geometry *>(node);
			const GeometryObject *geometryObject = geometry->GetObject();
			if ((geometryObject->GetGeometryFlags() & worldContext->shadowInhibitMask) == 0)
			{
				unsigned_int32 shadowStamp = shadowRenderStamp;
				if (geometry->GetShadowStamp() != shadowStamp)
				{
					geometry->SetShadowStamp(shadowStamp);
					
					if (ShadowNodeVisible(geometry, &renderData->shadowRegionList))
					{
						unsigned_int32 mask = geometry->GetPerspectiveExclusionMask();
						unsigned_int32 perspective = worldContext->perspectiveFlags;
						
						if (((mask >> 16) & perspective) == 0)
						{
							worldCounter[kWorldCounterStencilShadow]++;
							
							StencilMode stencilMode = ((mask & perspective) == 0) ? kStencilPass : kStencilFail;
							
							unsigned_int32 renderStamp = geometryRenderStamp;
							if (geometry->GetNodeStamp() != renderStamp)
							{
								geometry->SetNodeStamp(renderStamp);
								
								if (mask & kPerspectiveDirect) stencilMode = kStencilFail;
								ProcessGeometry(worldContext, geometry);
								FinishWorldBatch();
							}
							
							if ((stencilMode == kStencilPass) && (geometry->Visible(&renderData->nearClipRegion))) stencilMode = kStencilFail;
							RenderInfiniteShadowVolume(geometry, static_cast<InfiniteLight *>(renderData->lightRegion->GetLight()), stencilMode);
						}
					}
				}
			}
		}
		
		const Bond *bond = node->GetFirstOutgoingEdge();
		while (bond)
		{
			Node *node = static_cast<Node *>(bond->GetFinishElement());
			if (node != renderData->excludeNode) RenderInfiniteShadowNode(worldContext, node, renderData);
			
			bond = bond->GetNextOutgoingEdge();
		}
	}
}

void World::RenderInfiniteShadowCell(const WorldContext *worldContext, const Site *cell, const ShadowRenderData *renderData)
{
	if ((renderData->lightRegion->BoxVisible(cell->GetWorldBoundingBox())) && (ShadowCellVisible(cell, &renderData->shadowRegionList)))
	{
		const Bond *bond = cell->GetFirstOutgoingEdge();
		while (bond)
		{
			Site *site = bond->GetFinishElement();
			if (site->GetCellIndex() < 0)
			{
				Node *node = static_cast<Node *>(site);
				if (node != renderData->excludeNode) RenderInfiniteShadowNode(worldContext, node, renderData);
			}
			else
			{
				RenderInfiniteShadowCell(worldContext, site, renderData);
			}
			
			bond = bond->GetNextOutgoingEdge();
		}
	}
}

void World::CalculateInfiniteNearClipRegion(const FrustumCamera *camera, const Vector3D& lightDirection, Region *nearClipRegion)
{
	const FrustumCameraObject *cameraObject = camera->GetObject();
	Antivector4D *plane = nearClipRegion->GetPlaneArray();
	
	if ((cameraObject->GetCameraType() != kCameraRemote) || (!(cameraObject->GetFrustumFlags() & kFrustumOblique)))
	{
		const Point3D *vertex = camera->GetFrustumVertexArray();
		const Point3D& center = camera->GetNearPlaneCenter();
		
		const Transform4D& transform = camera->GetWorldTransform();
		
		float lz = transform[2] * lightDirection;
		if (lz > kNearClipEpsilon)
		{
			nearClipRegion->SetPlaneCount(5);
			plane[0].Set(transform[2], center);
			
			const Point3D *v1 = &vertex[3];
			for (machine a = 0; a < 4; a++)
			{
				const Point3D *v2 = &vertex[a];
				Vector3D normal = ((*v2 - *v1) % lightDirection).Normalize();
				plane[a + 1].Set(normal, *v1);
				v1 = v2;
			}
		}
		else if (lz < -kNearClipEpsilon)
		{
			nearClipRegion->SetPlaneCount(5);
			plane[0].Set(-transform[2], center);
			
			const Point3D *v1 = &vertex[3];
			for (machine a = 0; a < 4; a++)
			{
				const Point3D *v2 = &vertex[a];
				Vector3D normal = ((*v1 - *v2) % lightDirection).Normalize();
				plane[a + 1].Set(normal, *v1);
				v1 = v2;
			}
		}
		else
		{
			nearClipRegion->SetPlaneCount(2);
			plane[0].Set(transform[2], center);
			plane[1] = -plane[0];
		}
	}
	else
	{
		const RemoteCamera *remoteCamera = static_cast<const RemoteCamera *>(camera);
		int32 vertexCount = remoteCamera->GetRemoteVertexCount();
		const Point3D *vertex = remoteCamera->GetRemoteVertexArray();
		
		const RemoteCameraObject *remoteCameraObject = remoteCamera->GetObject();
		const Antivector4D& clipPlane = remoteCameraObject->GetRemoteClipPlane();
		float m = remoteCameraObject->GetRemoteDeterminant();
		
		float ld = clipPlane ^ lightDirection;
		if (ld > kNearClipEpsilon)
		{
			nearClipRegion->SetPlaneCount(vertexCount + 1);
			plane[0] = clipPlane;
			
			const Point3D *v1 = &vertex[vertexCount - 1];
			for (machine a = 0; a < vertexCount; a++)
			{
				const Point3D *v2 = &vertex[a];
				Vector3D normal = (*v2 - *v1) % lightDirection;
				normal *= InverseMag(normal) * m;
				plane[a + 1].Set(normal, *v1);
				v1 = v2;
			}
		}
		else if (ld < -kNearClipEpsilon)
		{
			nearClipRegion->SetPlaneCount(vertexCount + 1);
			plane[0] = -clipPlane;
			
			const Point3D *v1 = &vertex[vertexCount - 1];
			for (machine a = 0; a < vertexCount; a++)
			{
				const Point3D *v2 = &vertex[a];
				Vector3D normal = (*v1 - *v2) % lightDirection;
				normal *= InverseMag(normal) * m;
				plane[a + 1].Set(normal, *v1);
				v1 = v2;
			}
		}
		else
		{
			nearClipRegion->SetPlaneCount(2);
			plane[0] = clipPlane;
			plane[1] = -clipPlane;
		}
	}
}

void World::CalculateInfiniteShadowRegion(const CameraRegion *cameraRegion, const Vector3D& lightDirection, ShadowRegion *shadowRegion)
{
	int32 cameraPlaneCount = cameraRegion->GetPlaneCount();
	int32 nonlateralPlaneCount = cameraRegion->GetNonlateralPlaneCount();
	int32 lateralPlaneCount = cameraPlaneCount - nonlateralPlaneCount;
	
	const Camera *camera = cameraRegion->GetCamera();
	const Point3D& cameraPosition = camera->GetWorldPosition();
	
	const Antivector4D *cameraPlane = cameraRegion->GetPlaneArray();
	Antivector4D *shadowPlane = shadowRegion->GetPlaneArray();
	
	if (cameraRegion->ContainsInfiniteLight(lightDirection))
	{
		shadowPlane[0].Set(camera->GetWorldTransform()[2], cameraPosition);
		
		for (machine a = 0; a < lateralPlaneCount; a++) shadowPlane[a + 1] = cameraPlane[a];
		shadowRegion->SetPlaneCount(lateralPlaneCount + 1);
	}
	else
	{
		bool	frontArray[kMaxPortalVertexCount];
		
		for (machine a = 0; a < lateralPlaneCount; a++) frontArray[a] = ((cameraPlane[a] ^ lightDirection) > 0.0F);
		
		int32 shadowPlaneCount = 0;
		
		const Antivector4D *plane1 = &cameraPlane[lateralPlaneCount - 1];
		bool front1 = frontArray[lateralPlaneCount - 1];
		
		for (machine a = 0; a < lateralPlaneCount; a++)
		{
			const Antivector4D *plane2 = &cameraPlane[a];
			bool front2 = frontArray[a];
			
			if (front2)
			{
				shadowPlane[shadowPlaneCount++] = *plane2;
				
				if (!front1)
				{
					Antivector4D& newPlane = shadowPlane[shadowPlaneCount++];
					newPlane = Bivector4D(*plane1, *plane2) ^ lightDirection;
					newPlane.Standardize();
				}
			}
			else
			{
				if (front1)
				{
					Antivector4D& newPlane = shadowPlane[shadowPlaneCount++];
					newPlane = Bivector4D(*plane2, *plane1) ^ lightDirection;
					newPlane.Standardize();
				}
			}
			
			plane1 = plane2;
			front1 = front2;
		}
		
		unsigned_int32 flags = cameraRegion->GetShadowRegionFlags();
		for (machine a = 0; a < nonlateralPlaneCount; a++)
		{
			if (flags & 1)
			{
				const Antivector4D& plane = cameraPlane[lateralPlaneCount + a];
				bool front = ((plane ^ lightDirection) > 0.0F);
				
				if (front)
				{
					shadowPlane[shadowPlaneCount] = plane;
					if (++shadowPlaneCount == kMaxRegionPlaneCount) break;
					
					for (machine b = 0; b < lateralPlaneCount; b++)
					{
						if (!frontArray[b])
						{
							Antivector4D& newPlane = shadowPlane[shadowPlaneCount];
							newPlane = Bivector4D(cameraPlane[b], plane) ^ lightDirection;
							newPlane.Standardize();
							
							if (++shadowPlaneCount == kMaxRegionPlaneCount) goto full;
						}
					}
				}
				else
				{
					for (machine b = 0; b < lateralPlaneCount; b++)
					{
						if (frontArray[b])
						{
							Antivector4D& newPlane = shadowPlane[shadowPlaneCount];
							newPlane = Bivector4D(plane, cameraPlane[b]) ^ lightDirection;
							newPlane.Standardize();
							
							if (++shadowPlaneCount == kMaxRegionPlaneCount) goto full;
						}
					}
				}
			}
			
			flags >>= 1;
		}
		
		full:
		shadowRegion->SetPlaneCount(shadowPlaneCount);
	}
}

void World::RenderInfiniteLight(WorldContext *worldContext, InfiniteLight *light)
{
	List<Reference<LightRegion> >	referenceList;
	
	worldContext->shadowInhibitMask = kGeometryShadowInhibit;
	
	const LightShadowData *shadowData = nullptr;
	const Vector3D& lightDirection = light->GetWorldTransform()[2];
	
	LightType lightType = light->GetLightType();
	if (lightType == kLightDepth)
	{
		worldContext->shadowInhibitMask |= kGeometryRenderShadowMap;
		
		DepthLight *depthLight = static_cast<DepthLight *>(light);
		shadowData = depthLight->CalculateShadowData(worldContext->renderCamera);
		
		if (!worldContext->shadowMapKeepFlag)
		{
			ShadowRegion	shadowRegion;
			
			TheGraphicsMgr->BeginShadowMap();
			
			const CameraRegion *receiveRegion = worldContext->shadowReceiveRegion;
			int32 nonlateralPlaneCount = receiveRegion->GetNonlateralPlaneCount();
			int32 planeCount = receiveRegion->GetPlaneCount() - nonlateralPlaneCount;
			nonlateralPlaneCount = Min(nonlateralPlaneCount, 1);	// Only keep near plane
			
			CameraRegion sectionRegion(worldContext->renderCamera);
			const Antivector4D *planeArray = receiveRegion->GetPlaneArray();
			Antivector4D *sectionPlaneArray = sectionRegion.GetPlaneArray();
			for (machine b = 0; b < planeCount; b++) *sectionPlaneArray++ = planeArray[b];
			
			unsigned_int32 shadowRegionFlags = receiveRegion->GetShadowRegionFlags() & nonlateralPlaneCount;
			int32 shadowNonlateralPlaneCount = nonlateralPlaneCount;
			
			const ShadowSpace *shadowSpace = light->GetConnectedShadowSpace();
			if (shadowSpace)
			{
				*sectionPlaneArray++ = shadowSpace->GetInverseWorldTransform().GetRow(2);
				shadowRegionFlags = (shadowRegionFlags << 1) | 0x01;
				shadowNonlateralPlaneCount++;
			}
			
			planeArray += planeCount;
			for (machine b = 0; b < nonlateralPlaneCount; b++) *sectionPlaneArray++ = planeArray[b];
			
			sectionRegion.SetPlaneCount(planeCount + shadowNonlateralPlaneCount);
			sectionRegion.SetNonlateralPlaneCount(shadowNonlateralPlaneCount);
			sectionRegion.SetShadowRegionFlags(shadowRegionFlags);
			
			CalculateInfiniteShadowRegion(&sectionRegion, lightDirection, &shadowRegion);
			RenderShadowMap(worldContext, depthLight, 0, shadowData, &shadowRegion);
			worldCounter[kWorldCounterShadowSection]++;
			
			TheGraphicsMgr->EndShadowMap();
		}
	}
	else if (lightType == kLightLandscape)
	{
		worldContext->shadowInhibitMask |= kGeometryRenderShadowMap;
		
		LandscapeLight *landscapeLight = static_cast<LandscapeLight *>(light);
		shadowData = landscapeLight->CalculateShadowData(worldContext->renderCamera);
		
		if (!worldContext->shadowMapKeepFlag)
		{
			ShadowRegion	shadowRegion;
			
			TheGraphicsMgr->BeginShadowMap();
			
			const CameraRegion *receiveRegion = worldContext->shadowReceiveRegion;
			int32 planeCount = receiveRegion->GetPlaneCount() - receiveRegion->GetNonlateralPlaneCount();
			const Antivector4D *planeArray = receiveRegion->GetPlaneArray();
			
			const ShadowSpace *shadowSpace = light->GetConnectedShadowSpace();
			
			for (machine a = 0; a < kMaxShadowSectionCount; a++)
			{
				if (a > 0)
				{
					const Region *region = worldContext->occlusionList.First();
					while (region)
					{
						for (;;)
						{
							if (region->PolygonOccluded(4, shadowData[a].sectionPolygon))
							{
								if (++a < kMaxShadowSectionCount)
								{
									region = worldContext->occlusionList.First();
									continue;
								}
								
								goto end;
							}
							
							break;
						}
						
						region = region->Next();
					}
				}
				
				CameraRegion sectionRegion(worldContext->renderCamera);
				Antivector4D *sectionPlaneArray = sectionRegion.GetPlaneArray();
				for (machine b = 0; b < planeCount; b++) *sectionPlaneArray++ = planeArray[b];
				
				int32 nonlateralPlaneCount = 1 + (a < kMaxShadowSectionCount - 1);
				if (shadowSpace)
				{
					*sectionPlaneArray++ = shadowSpace->GetInverseWorldTransform().GetRow(2);
					nonlateralPlaneCount++;
				}
				
				*sectionPlaneArray++ = shadowData[a].nearPlane;
				*sectionPlaneArray++ = shadowData[a].farPlane;
				
				sectionRegion.SetPlaneCount(planeCount + nonlateralPlaneCount);
				sectionRegion.SetNonlateralPlaneCount(nonlateralPlaneCount);
				sectionRegion.SetShadowRegionFlags(~0);
				
				CalculateInfiniteShadowRegion(&sectionRegion, lightDirection, &shadowRegion);
				RenderShadowMap(worldContext, landscapeLight, a, &shadowData[a], &shadowRegion);
				worldCounter[kWorldCounterShadowSection]++;
			}
			
			end:
			TheGraphicsMgr->EndShadowMap();
		}
	}
	
	const InfiniteLightObject *lightObject = light->GetObject();
	TheGraphicsMgr->SetLight(lightObject, light, shadowData);
	
	Reference<LightRegion> *lightRegionReference = lightRegionList.First();
	while (lightRegionReference)
	{
		Reference<LightRegion> *next = lightRegionReference->Next();
		
		LightRegion *lightRegion = lightRegionReference->GetTarget();
		if (lightRegion->GetLight() == light) referenceList.Append(lightRegionReference);
		
		lightRegionReference = next;
	}
	
	const Node *excludeNode = light->GetExclusionNode();
	if (!(lightObject->GetLightFlags() & kLightShadowInhibit))
	{
		ShadowRenderData	renderData;
		
		renderData.excludeNode = excludeNode;
		CalculateInfiniteNearClipRegion(worldContext->renderCamera, lightDirection, &renderData.nearClipRegion);
		
		lightRegionReference = referenceList.First();
		while (lightRegionReference)
		{
			LightRegion *lightRegion = lightRegionReference->GetTarget();
			do
			{
				CameraRegion *cameraRegion = lightRegion->GetZone()->GetFirstCameraRegion();
				while (cameraRegion)
				{
					ShadowRegion *shadowRegion = cameraRegion->GetShadowRegion();
					renderData.shadowRegionList.Append(shadowRegion);
					
					if (shadowRegion->GetLight() != light)
					{
						shadowRegion->SetLight(light);
						CalculateInfiniteShadowRegion(cameraRegion, lightDirection, shadowRegion);
						
						#if C4DIAGNOSTICS
						
							if (diagnosticFlags & kDiagnosticShadowRegions)
							{
								shadowRegionDiagnosticList.Append(new RegionRenderable(shadowRegion, worldContext->renderCamera->GetWorldPosition(), kDiagnosticRegionSize));
							}
						
						#endif
					}
					
					cameraRegion = cameraRegion->GetNextCameraRegion();
				}
				
				lightRegion = lightRegion->GetSuperNode();
			} while (lightRegion);
			
			lightRegionReference = lightRegionReference->Next();
		}
		
		TheGraphicsMgr->BeginStencilShadow();
		shadowRenderStamp++;
		
		lightRegionReference = referenceList.First();
		while (lightRegionReference)
		{
			LightRegion *lightRegion = lightRegionReference->GetTarget();
			do
			{
				unsigned_int32 flags = lightRegion->GetLightRegionFlags();
				if (!(flags & kLightRegionShadowsRendered))
				{
					lightRegion->SetLightRegionFlags(flags | kLightRegionShadowsRendered);
					renderData.lightRegion = lightRegion;
					
					const Bond *bond = lightRegion->GetZone()->GetFirstOutgoingEdge();
					while (bond)
					{
						Site *site = bond->GetFinishElement();
						if (site->GetCellIndex() < 0)
						{
							Node *node = static_cast<Node *>(site);
							if (node != excludeNode) RenderInfiniteShadowNode(worldContext, node, &renderData);
						}
						else
						{
							RenderInfiniteShadowCell(worldContext, site, &renderData);
						}
						
						bond = bond->GetNextOutgoingEdge();
					}
				}
				
				lightRegion = lightRegion->GetSuperNode();
			} while (lightRegion);
			
			lightRegionReference = lightRegionReference->Next();
		}
		
		TheGraphicsMgr->EndStencilShadow();
		renderData.shadowRegionList.RemoveAll();
	}
	
	lightRegionReference = referenceList.First();
	while (lightRegionReference)
	{
		LightRegion *lightRegion = lightRegionReference->GetTarget();
		
		const CameraRegion *cameraRegion = lightRegion->GetZone()->GetFirstCameraRegion();
		while (cameraRegion)
		{
			Geometry *geometry = cameraRegion->GetFirstGeometry();
			while (geometry)
			{
				if ((!excludeNode) || (!NodeExcluded(geometry, excludeNode)))
				{
					if (geometry->Visible(lightRegion)) renderStageList[kRenderStageDefault].Append(geometry);
				}
				
				geometry = geometry->Next();
			}
			
			int32 impostorCount = cameraRegion->GetImpostorCount();
			for (machine a = 0; a < impostorCount; a++)
			{
				Impostor *impostor = cameraRegion->GetImpostor(a);
				if (impostor->Visible(lightRegion)) impostor->Render();
			}
			
			Effect *effect = cameraRegion->GetFirstEffect();
			while (effect)
			{
				if (effect->Visible(lightRegion)) renderStageList[kRenderStageDefault].Append(effect);
				effect = effect->Next();
			}
			
			cameraRegion = cameraRegion->GetNextCameraRegion();
		}
		
		lightRegionReference = lightRegionReference->Next();
	}
	
	TheGraphicsMgr->GroupLightRenderList(&renderStageList[kRenderStageDefault]);
	
	ImpostorSystem *system = impostorSystemMap.First();
	while (system)
	{
		system->RenderSystem(&renderStageList[kRenderStageDefault]);
		system = system->Next();
	}
	
	TheGraphicsMgr->DrawRenderList(&renderStageList[kRenderStageDefault]);
	renderStageList[kRenderStageDefault].RemoveAll();
}

void World::RenderPointShadowVolume(Geometry *geometry, PointLight *light, StencilMode stencilMode)
{
	int32 detailLevel = geometry->GetDetailLevel();
	const GeometryObject *geometryObject = geometry->GetObject();
	const GeometryLevel *geometryLevel = geometryObject->GetGeometryLevel(detailLevel);
	
	int32 edgeCount = geometryLevel->GetArrayDescriptor(kArrayEdge)->elementCount;
	if ((stencilMode == kStencilFail) || (edgeCount != 0))
	{
		StencilData *stencilData = geometry->GetStencilData();
		StencilVolume *stencilVolume = nullptr;
		
		bool buildExtrusion = true;
		bool buildEndcaps = (stencilMode == kStencilFail);
		
		unsigned_int32 lightFlags = light->GetObject()->GetLightFlags();
		if ((lightFlags & kLightStatic) && (!(geometryObject->GetGeometryFlags() & kGeometryDynamic)))
		{
			Link<StencilVolume> *stencilVolumeLink = geometry->GetStaticStencilVolume(light);
			if (stencilVolumeLink)
			{
				stencilVolume = *stencilVolumeLink;
				if (stencilVolume)
				{
					buildExtrusion = (stencilVolume->GetExtrusionDetailLevel() != detailLevel);
					if (buildEndcaps) buildEndcaps = (stencilVolume->GetEndcapDetailLevel() != detailLevel);
				}
				else
				{
					stencilVolume = new StencilVolume(geometry, light, stencilVolumeLink);
				}
				
				stencilData = stencilVolume;
			}
		}
		
		if (buildExtrusion) stencilData->CalculatePointShadowBounds(light);
		
		const Vector4D& objectLightPosition = TheGraphicsMgr->SetGeometryTransformable(geometry->GetTransformable());
		if (TheGraphicsMgr->ActivateShadowBounds(stencilData))
		{
			if (buildExtrusion)
			{
				geometry->CalculatePointShadowFrontArray(objectLightPosition.GetPoint3D());
				
				const Point3D *vertex = static_cast<Point3D *>(geometry->GetArrayBundle(kArrayVertex)->pointer);
				const Edge *edge = geometryLevel->GetArray<Edge>(kArrayEdge);
				
				const bool *front = geometry->GetShadowFrontArray();
				Vector4D *restrict extrusionVertex = stencilData->GetExtrusionVertexArray();
				
				int32 extrusionEdgeCount = 0;
				for (machine a = 0; a < edgeCount; a++)
				{
					bool f1 = front[edge->faceIndex[0]];
					bool f2 = front[edge->faceIndex[1]];
					if (f1 ^ f2)
					{
						const Point3D& p1 = vertex[edge->vertexIndex[0]];
						const Point3D& p2 = vertex[edge->vertexIndex[1]];
						
						if (f1)
						{
							extrusionVertex[0] = p2;
							extrusionVertex[1] = p1;
							extrusionVertex[2].Set(p1.x, p1.y, p1.z, 0.0F);
							extrusionVertex[3].Set(p2.x, p2.y, p2.z, 0.0F);
						}
						else
						{
							extrusionVertex[0] = p1;
							extrusionVertex[1] = p2;
							extrusionVertex[2].Set(p2.x, p2.y, p2.z, 0.0F);
							extrusionVertex[3].Set(p1.x, p1.y, p1.z, 0.0F);
						}
						
						extrusionEdgeCount++;
						extrusionVertex += 4;
					}
					
					edge++;
				}
				
				stencilData->SetExtrusionEdgeCount(extrusionEdgeCount);
				if (stencilVolume) stencilVolume->SetExtrusionDetailLevel(detailLevel);
			}
			else
			{
				if (buildEndcaps) geometry->CalculatePointShadowFrontArray(objectLightPosition.GetPoint3D());
			}
			
			if (stencilData->GetExtrusionEdgeCount() != 0)
			{
				TheGraphicsMgr->DrawStencilShadow(stencilData, kStencilPointExtrusion, stencilMode);
				
				if (stencilMode == kStencilFail)
				{
					if (buildEndcaps)
					{
						int32 faceCount = geometryLevel->GetFaceCount();
						const Triangle *triangle = geometryLevel->GetArray<Triangle>(kArrayFace);
						
						int32 frontEndcapTriangleCount = 0;
						int32 backEndcapTriangleCount = 0;
						
						Triangle *restrict frontEndcapTriangleArray = stencilData->GetFrontEndcapTriangleArray();
						Triangle *restrict backEndcapTriangleArray = frontEndcapTriangleArray + faceCount;
						
						const bool *front = geometry->GetShadowFrontArray();
						const unsigned_int16 *planeIndex = geometryLevel->GetArray<unsigned_int16>(kArrayPlaneIndex);
						if (planeIndex)
						{
							for (machine a = 0; a < faceCount; a++)
							{
								if (front[*planeIndex])
								{
									frontEndcapTriangleArray[frontEndcapTriangleCount] = *triangle;
									frontEndcapTriangleCount++;
								}
								else
								{
									*--backEndcapTriangleArray = *triangle;
									backEndcapTriangleCount++;
								}
								
								triangle++;
								planeIndex++;
							}
						}
						else
						{
							for (machine a = 0; a < faceCount; a++)
							{
								if (front[a])
								{
									frontEndcapTriangleArray[frontEndcapTriangleCount] = *triangle;
									frontEndcapTriangleCount++;
								}
								else
								{
									*--backEndcapTriangleArray = *triangle;
									backEndcapTriangleCount++;
								}
								
								triangle++;
							}
						}
						
						stencilData->SetFrontEndcapTriangleCount(frontEndcapTriangleCount);
						stencilData->SetBackEndcapTriangleCount(backEndcapTriangleCount);
						stencilData->SetBackEndcapTriangleArray(backEndcapTriangleArray);
						if (stencilVolume) stencilVolume->SetEndcapDetailLevel(detailLevel);
					}
					
					int32 vertexCount = geometryLevel->GetVertexCount();
					const Point3D *vertex = static_cast<Point3D *>(geometry->GetArrayBundle(kArrayVertex)->pointer);
					
					if (stencilData->GetFrontEndcapTriangleCount() != 0)
					{
						stencilData->SetGeometryVertexArray(vertexCount, vertex);
						TheGraphicsMgr->DrawStencilShadow(stencilData, kStencilEndcapIdentity, kStencilFail);
					}
					
					if (stencilData->GetBackEndcapTriangleCount() != 0)
					{
						stencilData->SetGeometryVertexArray(vertexCount, vertex);
						TheGraphicsMgr->DrawStencilShadow(stencilData, kStencilEndcapProjection, kStencilFail);
					}
				}
			}
		}
	}
}

void World::RenderPointShadowNode(const WorldContext *worldContext, Node *node, const ShadowRenderData *renderData)
{
	NodeType type = node->GetNodeType();
	if ((type != kNodeZone) && (node->Enabled()) && (node->Visible(renderData->lightRegion)))
	{
		if (type == kNodeGeometry)
		{
			Geometry *geometry = static_cast<Geometry *>(node);
			const GeometryObject *geometryObject = geometry->GetObject();
			if ((geometryObject->GetGeometryFlags() & worldContext->shadowInhibitMask) == 0)
			{
				PointLight *light = static_cast<PointLight *>(renderData->lightRegion->GetLight());
				const Point3D& lightPosition = light->GetWorldPosition();
				float lightRange = light->GetObject()->GetLightRange();
				
				const BoundingSphere *sphere = geometry->GetBoundingSphere();
				float radius = sphere->GetRadius();
				
				Vector3D axis = sphere->GetCenter() - lightPosition;
				float d = radius + lightRange;
				float dist2 = SquaredMag(axis);
				if (dist2 < d * d)
				{
					if (!geometryObject->ExteriorSphere(geometry->GetInverseWorldTransform() * lightPosition, lightRange))
					{
						if (ShadowNodeVisible(geometry, &renderData->shadowRegionList))
						{
							unsigned_int32 mask = geometry->GetPerspectiveExclusionMask();
							unsigned_int32 perspective = worldContext->perspectiveFlags;
							
							if (((mask >> 16) & perspective) == 0)
							{
								worldCounter[kWorldCounterStencilShadow]++;
								
								StencilMode stencilMode = ((mask & perspective) == 0) ? kStencilPass : kStencilFail;
								
								unsigned_int32 renderStamp = geometryRenderStamp;
								if (geometry->GetNodeStamp() != renderStamp)
								{
									geometry->SetNodeStamp(renderStamp);
									
									if (mask & kPerspectiveDirect) stencilMode = kStencilFail;
									ProcessGeometry(worldContext, geometry);
									FinishWorldBatch();
								}
								
								if ((stencilMode == kStencilPass) && (geometry->Visible(&renderData->nearClipRegion))) stencilMode = kStencilFail;
								RenderPointShadowVolume(geometry, light, stencilMode);
							}
						}
					}
				}
			}
		}
		
		const Bond *bond = node->GetFirstOutgoingEdge();
		while (bond)
		{
			Node *node = static_cast<Node *>(bond->GetFinishElement());
			if (node != renderData->excludeNode) RenderPointShadowNode(worldContext, node, renderData);
			
			bond = bond->GetNextOutgoingEdge();
		}
	}
}

void World::RenderPointShadowCell(const WorldContext *worldContext, const Site *cell, const ShadowRenderData *renderData)
{
	if ((renderData->lightRegion->BoxVisible(cell->GetWorldBoundingBox())) && (ShadowCellVisible(cell, &renderData->shadowRegionList)))
	{
		const Bond *bond = cell->GetFirstOutgoingEdge();
		while (bond)
		{
			Site *site = bond->GetFinishElement();
			if (site->GetCellIndex() < 0)
			{
				Node *node = static_cast<Node *>(site);
				if (node != renderData->excludeNode) RenderPointShadowNode(worldContext, node, renderData);
			}
			else
			{
				RenderPointShadowCell(worldContext, site, renderData);
			}
			
			bond = bond->GetNextOutgoingEdge();
		}
	}
}

void World::CalculatePointNearClipRegion(const FrustumCamera *camera, const Point3D& lightPosition, Region *nearClipRegion)
{
	const FrustumCameraObject *cameraObject = camera->GetObject();
	Antivector4D *plane = nearClipRegion->GetPlaneArray();
	
	if ((cameraObject->GetCameraType() != kCameraRemote) || (!(cameraObject->GetFrustumFlags() & kFrustumOblique)))
	{
		const Point3D *vertex = camera->GetFrustumVertexArray();
		const Point3D& center = camera->GetNearPlaneCenter();
		
		Vector3D backNormal = (center - lightPosition).Normalize();
		plane[0].Set(backNormal, lightPosition);
		
		const Transform4D& transform = camera->GetWorldTransform();
		Antivector4D nearPlane(transform[2], center);
		
		float lz = nearPlane ^ lightPosition;
		if (lz > kNearClipEpsilon)
		{
			nearClipRegion->SetPlaneCount(6);
			plane[1] = nearPlane;
			
			const Point3D *v1 = &vertex[3];
			for (machine a = 0; a < 4; a++)
			{
				const Point3D *v2 = &vertex[a];
				Vector3D normal = ((*v2 - *v1) % (lightPosition - *v1)).Normalize();
				plane[a + 2].Set(normal, *v1);
				v1 = v2;
			}
		}
		else if (lz < -kNearClipEpsilon)
		{
			nearClipRegion->SetPlaneCount(6);
			plane[1] = -nearPlane;
			
			const Point3D *v1 = &vertex[3];
			for (machine a = 0; a < 4; a++)
			{
				const Point3D *v2 = &vertex[a];
				Vector3D normal = ((*v1 - *v2) % (lightPosition - *v1)).Normalize();
				plane[a + 2].Set(normal, *v1);
				v1 = v2;
			}
		}
		else
		{
			nearClipRegion->SetPlaneCount(3);
			plane[1] = nearPlane;
			plane[2] = -nearPlane;
		}
	}
	else
	{
		const RemoteCamera *remoteCamera = static_cast<const RemoteCamera *>(camera);
		int32 vertexCount = remoteCamera->GetRemoteVertexCount();
		const Point3D *vertex = remoteCamera->GetRemoteVertexArray();
		const Point3D& center = remoteCamera->GetRemoteCenter();
		
		const RemoteCameraObject *remoteCameraObject = remoteCamera->GetObject();
		const Antivector4D& clipPlane = remoteCameraObject->GetRemoteClipPlane();
		float m = remoteCameraObject->GetRemoteDeterminant();
		
		Vector3D backNormal = (center - lightPosition).Normalize();
		plane[0].Set(backNormal, lightPosition);
		
		float ld = clipPlane ^ lightPosition;
		if (ld > kNearClipEpsilon)
		{
			nearClipRegion->SetPlaneCount(vertexCount + 2);
			plane[1] = clipPlane;
			
			const Point3D *v1 = &vertex[vertexCount - 1];
			for (machine a = 0; a < vertexCount; a++)
			{
				const Point3D *v2 = &vertex[a];
				Vector3D normal = (*v2 - *v1) % (lightPosition - *v1);
				normal *= InverseMag(normal) * m;
				plane[a + 2].Set(normal, *v1);
				v1 = v2;
			}
		}
		else if (ld < -kNearClipEpsilon)
		{
			nearClipRegion->SetPlaneCount(vertexCount + 2);
			plane[1] = -clipPlane;
			
			const Point3D *v1 = &vertex[vertexCount - 1];
			for (machine a = 0; a < vertexCount; a++)
			{
				const Point3D *v2 = &vertex[a];
				Vector3D normal = (*v1 - *v2) % (lightPosition - *v1);
				normal *= InverseMag(normal) * m;
				plane[a + 2].Set(normal, *v1);
				v1 = v2;
			}
		}
		else
		{
			nearClipRegion->SetPlaneCount(3);
			plane[1] = clipPlane;
			plane[2] = -clipPlane;
		}
	}
}

void World::CalculatePointShadowRegion(const CameraRegion *cameraRegion, const Point3D& lightPosition, ShadowRegion *shadowRegion)
{
	int32 cameraPlaneCount = cameraRegion->GetPlaneCount();
	int32 nonlateralPlaneCount = cameraRegion->GetNonlateralPlaneCount();
	int32 lateralPlaneCount = cameraPlaneCount - nonlateralPlaneCount;
	
	const Camera *camera = cameraRegion->GetCamera();
	const Point3D& cameraPosition = camera->GetWorldPosition();
	
	const Antivector4D *cameraPlane = cameraRegion->GetPlaneArray();
	Antivector4D *shadowPlane = shadowRegion->GetPlaneArray();
	
	if (cameraRegion->ContainsPointLight(lightPosition))
	{
		shadowPlane[0].Set(camera->GetWorldTransform()[2], cameraPosition);
		
		for (machine a = 0; a < lateralPlaneCount; a++) shadowPlane[a + 1] = cameraPlane[a];
		shadowRegion->SetPlaneCount(lateralPlaneCount + 1);
	}
	else
	{
		bool	frontArray[kMaxPortalVertexCount];
		
		for (machine a = 0; a < lateralPlaneCount; a++) frontArray[a] = ((cameraPlane[a] ^ lightPosition) > 0.0F);
		
		int32 shadowPlaneCount = 1;
		Antivector3D backPlaneNormal(0.0F, 0.0F, 0.0F);
		
		const Antivector4D *plane1 = &cameraPlane[lateralPlaneCount - 1];
		bool front1 = frontArray[lateralPlaneCount - 1];
		
		for (machine a = 0; a < lateralPlaneCount; a++)
		{
			const Antivector4D *plane2 = &cameraPlane[a];
			bool front2 = frontArray[a];
			
			if (front2)
			{
				shadowPlane[shadowPlaneCount++] = *plane2;
				
				if (!front1)
				{
					Antivector4D& newPlane = shadowPlane[shadowPlaneCount++];
					newPlane = Bivector4D(*plane1, *plane2) ^ lightPosition;
					newPlane.Standardize();
				}
			}
			else
			{
				backPlaneNormal += plane2->GetAntivector3D();
				
				if (front1)
				{
					Antivector4D& newPlane = shadowPlane[shadowPlaneCount++];
					newPlane = Bivector4D(*plane2, *plane1) ^ lightPosition;
					newPlane.Standardize();
				}
			}
			
			plane1 = plane2;
			front1 = front2;
		}
		
		backPlaneNormal.Normalize();
		shadowPlane[0].Set(backPlaneNormal, lightPosition);
		
		unsigned_int32 flags = cameraRegion->GetShadowRegionFlags();
		for (machine a = 0; a < nonlateralPlaneCount; a++)
		{
			if (flags & 1)
			{
				const Antivector4D& plane = cameraPlane[lateralPlaneCount + a];
				bool front = ((plane ^ lightPosition) > 0.0F);
				
				if (front)
				{
					shadowPlane[shadowPlaneCount] = plane;
					if (++shadowPlaneCount == kMaxRegionPlaneCount) break;
					
					for (machine b = 0; b < lateralPlaneCount; b++)
					{
						if (!frontArray[b])
						{
							Antivector4D& newPlane = shadowPlane[shadowPlaneCount];
							newPlane = Bivector4D(cameraPlane[b], plane) ^ lightPosition;
							newPlane.Standardize();
							
							if (++shadowPlaneCount == kMaxRegionPlaneCount) goto full;
						}
					}
				}
				else
				{
					for (machine b = 0; b < lateralPlaneCount; b++)
					{
						if (frontArray[b])
						{
							Antivector4D& newPlane = shadowPlane[shadowPlaneCount];
							newPlane = Bivector4D(plane, cameraPlane[b]) ^ lightPosition;
							newPlane.Standardize();
							
							if (++shadowPlaneCount == kMaxRegionPlaneCount) goto full;
						}
					}
				}
			}
			
			flags >>= 1;
		}
		
		full:
		shadowRegion->SetPlaneCount(shadowPlaneCount);
	}
}

void World::RenderPointLight(WorldContext *worldContext, PointLight *light)
{
	const PointLightObject *lightObject = light->GetObject();
	if (!TheGraphicsMgr->SetLight(lightObject, light))
	{
		Reference<LightRegion> *lightRegionReference = lightRegionList.First();
		while (lightRegionReference)
		{
			Reference<LightRegion> *nextLightRegionReference = lightRegionReference->Next();
			if (lightRegionReference->GetTarget()->GetLight() == light) delete lightRegionReference;
			lightRegionReference = nextLightRegionReference;
		}
		
		return;
	}
	
	List<Reference<LightRegion> >	referenceList;
	
	Reference<LightRegion> *lightRegionReference = lightRegionList.First();
	while (lightRegionReference)
	{
		Reference<LightRegion> *next = lightRegionReference->Next();
		
		LightRegion *lightRegion = lightRegionReference->GetTarget();
		if (lightRegion->GetLight() == light) referenceList.Append(lightRegionReference);
		
		lightRegionReference = next;
	}
	
	const Point3D& lightPosition = light->GetWorldPosition();
	float lightRange = lightObject->GetLightRange();
	
	const Node *excludeNode = light->GetExclusionNode();
	if (!(lightObject->GetLightFlags() & kLightShadowInhibit))
	{
		ShadowRenderData	renderData;
		
		worldContext->shadowInhibitMask = kGeometryShadowInhibit;
		if (light->GetLightType() == kLightCube) worldContext->shadowInhibitMask |= kGeometryCubeLightInhibit;
		
		renderData.excludeNode = excludeNode;
		CalculatePointNearClipRegion(worldContext->renderCamera, lightPosition, &renderData.nearClipRegion);
		
		lightRegionReference = referenceList.First();
		while (lightRegionReference)
		{
			LightRegion *lightRegion = lightRegionReference->GetTarget();
			do
			{
				CameraRegion *cameraRegion = lightRegion->GetZone()->GetFirstCameraRegion();
				while (cameraRegion)
				{
					ShadowRegion *shadowRegion = cameraRegion->GetShadowRegion();
					renderData.shadowRegionList.Append(shadowRegion);
					
					if (shadowRegion->GetLight() != light)
					{
						shadowRegion->SetLight(light);
						CalculatePointShadowRegion(cameraRegion, lightPosition, shadowRegion);
						
						#if C4DIAGNOSTICS
						
							if (diagnosticFlags & kDiagnosticShadowRegions)
							{
								shadowRegionDiagnosticList.Append(new RegionRenderable(shadowRegion, worldContext->renderCamera->GetWorldPosition(), kDiagnosticRegionSize));
							}
						
						#endif
					}
					
					cameraRegion = cameraRegion->GetNextCameraRegion();
				}
				
				lightRegion = lightRegion->GetSuperNode();
			} while (lightRegion);
			
			lightRegionReference = lightRegionReference->Next();
		}
		
		TheGraphicsMgr->BeginStencilShadow();
		shadowRenderStamp++;
		
		lightRegionReference = referenceList.First();
		while (lightRegionReference)
		{
			LightRegion *lightRegion = lightRegionReference->GetTarget();
			do
			{
				unsigned_int32 flags = lightRegion->GetLightRegionFlags();
				if (!(flags & kLightRegionShadowsRendered))
				{
					lightRegion->SetLightRegionFlags(flags | kLightRegionShadowsRendered);
					renderData.lightRegion = lightRegion;
					
					const Bond *bond = lightRegion->GetZone()->GetFirstOutgoingEdge();
					while (bond)
					{
						Site *site = bond->GetFinishElement();
						if (site->GetCellIndex() < 0)
						{
							Node *node = static_cast<Node *>(site);
							if (node != excludeNode) RenderPointShadowNode(worldContext, node, &renderData);
						}
						else
						{
							RenderPointShadowCell(worldContext, site, &renderData);
						}
						
						bond = bond->GetNextOutgoingEdge();
					}
				}
				
				lightRegion = lightRegion->GetSuperNode();
			} while (lightRegion);
			
			lightRegionReference = lightRegionReference->Next();
		}
		
		TheGraphicsMgr->EndStencilShadow();
		renderData.shadowRegionList.RemoveAll();
	}
	
	lightRegionReference = referenceList.First();
	while (lightRegionReference)
	{
		LightRegion *lightRegion = lightRegionReference->GetTarget();
		
		const CameraRegion *cameraRegion = lightRegion->GetZone()->GetFirstCameraRegion();
		while (cameraRegion)
		{
			Geometry *geometry = cameraRegion->GetFirstGeometry();
			while (geometry)
			{
				if ((!excludeNode) || (!NodeExcluded(geometry, excludeNode)))
				{
					const BoundingSphere *sphere = geometry->GetBoundingSphere();
					float d = sphere->GetRadius() + lightRange;
					if (SquaredMag(sphere->GetCenter() - lightPosition) < d * d)
					{
						if (!geometry->GetObject()->ExteriorSphere(geometry->GetInverseWorldTransform() * lightPosition, lightRange))
						{
							if (geometry->Visible(lightRegion)) renderStageList[kRenderStageDefault].Append(geometry);
						}
					}
				}
				
				geometry = geometry->Next();
			}
			
			int32 impostorCount = cameraRegion->GetImpostorCount();
			for (machine a = 0; a < impostorCount; a++)
			{
				Impostor *impostor = cameraRegion->GetImpostor(a);
				if (impostor->Visible(lightRegion)) impostor->Render();
			}
			
			Effect *effect = cameraRegion->GetFirstEffect();
			while (effect)
			{
				const BoundingSphere *sphere = effect->GetBoundingSphere();
				float d = sphere->GetRadius() + lightRange;
				if (SquaredMag(sphere->GetCenter() - lightPosition) < d * d)
				{
					if (effect->Visible(lightRegion)) renderStageList[kRenderStageDefault].Append(effect);
				}
				
				effect = effect->Next();
			}
			
			cameraRegion = cameraRegion->GetNextCameraRegion();
		}
		
		lightRegionReference = lightRegionReference->Next();
	}
	
	TheGraphicsMgr->GroupLightRenderList(&renderStageList[kRenderStageDefault]);
	
	ImpostorSystem *system = impostorSystemMap.First();
	while (system)
	{
		system->RenderSystem(&renderStageList[kRenderStageDefault]);
		system = system->Next();
	}
	
	TheGraphicsMgr->DrawRenderList(&renderStageList[kRenderStageDefault]);
	renderStageList[kRenderStageDefault].RemoveAll();
}

bool World::PointLightVisible(const Light *light, CameraRegion *rootRegion, const List<Region> *occlusionList)
{
	const PointLightObject *object = static_cast<const PointLightObject *>(light->GetObject());
	const Point3D& center = light->GetWorldPosition();
	float radius = object->GetLightRange();
	
	if (!rootRegion->SphereVisible(center, radius)) return (false);
	
	const Region *region = occlusionList->First();
	while (region)
	{
		if (region->SphereOccluded(center, radius)) return (false);
		region = region->Next();
	}
	
	return (true);
}

bool World::LightVisibleInTransition(const CameraRegion *cameraRegion, const LightRegion *lightRegion)
{
	const CameraRegion *cameraSuperRegion = cameraRegion->GetSuperNode();
	if (cameraSuperRegion)
	{
		const LightRegion *lightSuperRegion = lightRegion->GetSuperNode();
		if (lightSuperRegion)
		{
			const Zone *lightSuperZone = lightSuperRegion->GetZone();
			const Zone *cameraSuperZone = cameraSuperRegion->GetZone();
			
			const Portal *portal = cameraRegion->GetZone()->GetFirstPortal();
			while (portal)
			{
				if (portal->GetConnectedZone() == lightSuperZone)
				{
					if ((!portal->Enabled()) && (cameraSuperZone != lightSuperZone)) return (false);
					break;
				}
				
				portal = portal->Next();
			}
		}
	}
	
	return (true);
}

bool World::ClipLightRegion(const LightRegion *lightRegion, const CameraRegion *cameraRegion)
{
	int32 polygonCount = lightRegion->GetBoundaryPolygonCount();
	if (polygonCount < 4) return (true);
	
	if ((!cameraRegion->GetSuperNode()) && (lightRegion->SphereVisible(cameraRegion->GetCamera()->GetWorldPosition(), kCameraLightClipEpsilon))) return (true);
	
	const Point3D *polygonVertex = lightRegion->GetBoundaryPolygonVertexArray();
	
	int32 planeCount = cameraRegion->GetPlaneCount();
	const Antivector4D *plane = cameraRegion->GetPlaneArray();
	
	for (machine polygonIndex = 0; polygonIndex < polygonCount; polygonIndex++)
	{
		Point3D		temp[2][kMaxPortalVertexCount + kMaxRegionPlaneCount];
		
		int32 vertexCount = lightRegion->GetBoundaryPolygonVertexCount(polygonIndex);
		const Point3D *vertex = polygonVertex;
		polygonVertex += vertexCount;
		
		for (machine planeIndex = 0; planeIndex < planeCount; planeIndex++)
		{
			int8	location[kMaxPortalVertexCount + kMaxRegionPlaneCount];
			
			Point3D *result = temp[planeIndex & 1];
			vertexCount = Math::ClipPolygonAgainstPlane(vertexCount, vertex, plane[planeIndex], location, result);
			if (vertexCount == 0) goto nextPolygon;
			vertex = result;
		}
		
		return (true);
		
		nextPolygon:;
	}
	
	return (false);
}

void World::CollectLightRegions(const WorldContext *worldContext, CameraRegion *rootRegion)
{
	const Zone *zone = rootRegion->GetZone();
	bool transition = ((zone->GetObject()->GetZoneFlags() & kZoneTransition) != 0);
	
	LightRegion *lightRegion = zone->GetFirstLightRegion();
	while (lightRegion)
	{
		const Light *light = lightRegion->GetLight();
		if (light->Enabled())
		{
			if ((light->GetBaseLightType() == kLightInfinite) || (PointLightVisible(light, rootRegion, &worldContext->occlusionList)))
			{
				if ((!transition) || (LightVisibleInTransition(rootRegion, lightRegion)))
				{
					unsigned_int32 flags = lightRegion->GetLightRegionFlags();
					if (!(flags & kLightRegionBoundaryCalculated))
					{
						lightRegion->SetLightRegionFlags(flags | kLightRegionBoundaryCalculated);
						light->CalculateBoundaryPolygons(lightRegion);
					}
					
					if (ClipLightRegion(lightRegion, rootRegion))
					{
						LightRegion *region = lightRegion;
						do
						{
							region->SetLightRegionFlags(region->GetLightRegionFlags() & ~kLightRegionShadowsRendered);
							region = region->GetSuperNode();
						} while (region);
						
						lightRegionList.Append(new RegionReference<LightRegion>(lightRegion));
						
						#if C4DIAGNOSTICS
						
							if (diagnosticFlags & kDiagnosticLightRegions)
							{
								int32 polygonCount = lightRegion->GetBoundaryPolygonCount();
								const Point3D *vertex = lightRegion->GetBoundaryPolygonVertexArray();
								
								for (machine polygonIndex = 0; polygonIndex < polygonCount; polygonIndex++)
								{
									int32 vertexCount = lightRegion->GetBoundaryPolygonVertexCount(polygonIndex);
									lightRegionRenderable.SetAttributeArray(kArrayVertex, vertex);
									lightRegionRenderable.SetVertexCount(vertexCount);
									vertex += vertexCount;
									
									TheGraphicsMgr->DrawRenderList(&lightRegionRenderList);
								}
							}
						
						#endif
					}
				}
			}
		}
		
		lightRegion = lightRegion->GetNextLightRegion();
	}
	
	CameraRegion *subregion = rootRegion->GetFirstSubnode();
	while (subregion)
	{
		CollectLightRegions(worldContext, subregion);
		subregion = subregion->Tree<CameraRegion>::Next();
	}
}

bool World::ProcessFogSpace(WorldContext *worldContext, const FogSpace *fogSpace, CameraRegion *rootRegion)
{
	const FogSpaceObject *object = fogSpace->GetObject();
	if ((object->GetPerspectiveExclusionMask() & worldContext->perspectiveFlags) == 0)
	{
		const FrustumCamera *camera = worldContext->renderCamera;
		const Point3D& cameraPosition = camera->GetWorldPosition();
		const Vector3D& cameraDirection = camera->GetWorldTransform()[2];
		
		Antivector4D fogPlane = fogSpace->GetInverseWorldTransform().GetRow(2);
		
		float F_dot_C = fogPlane ^ cameraPosition;
		if (F_dot_C < 0.0F)
		{
			*worldContext->fogSpacePtr = fogSpace;
			
			if (!(object->GetFogFlags() & kFogOcclusionInhibit))
			{
				Region *newRegion = new Region;
				
				if (object->GetFogFunction() == kFogFunctionConstant)
				{
					float d = object->GetOcclusionValue();
					Antivector4D *planeArray = newRegion->GetPlaneArray();
					planeArray[0].Set(cameraDirection, -(cameraPosition * cameraDirection) - d);
					planeArray[1] = -fogPlane;
					newRegion->SetPlaneCount(2);
				}
				else
				{
					float d = F_dot_C + Sqrt(F_dot_C * F_dot_C * 0.25F + object->GetOcclusionValue());
					const Vector3D& planeNormal = fogPlane.GetAntivector3D();
					newRegion->GetPlaneArray()[0].Set(-planeNormal, cameraPosition * planeNormal - d);
					newRegion->SetPlaneCount(1);
				}
				
				worldContext->occlusionList.Append(newRegion);
			}
		}
		else if ((fogPlane ^ cameraDirection) < camera->GetSineHalfField())
		{
			if (NodeVisible(fogSpace, rootRegion, &worldContext->occlusionList))
			{
				*worldContext->fogSpacePtr = fogSpace;
				worldContext->unfoggedRegion.GetPlaneArray()[0] = fogPlane;
				worldContext->unfoggedList.Append(&worldContext->unfoggedRegion);
				
				if (!(object->GetFogFlags() & kFogOcclusionInhibit))
				{
					Region *newRegion = new Region;
					
					float d = object->GetOcclusionValue();
					if (object->GetFogFunction() != kFogFunctionConstant) d = Sqrt(d);
					newRegion->GetPlaneArray()[0].Set(-fogPlane.GetAntivector3D(), -fogPlane.w - d);
					newRegion->SetPlaneCount(1);
					
					worldContext->occlusionList.Append(newRegion);
				}
			}
		}
		
		return (true);
	}
	
	return (false);
}

void World::ProcessPortal(WorldContext *worldContext, Portal *portal, CameraRegion *rootRegion)
{
	if (portal->Enabled())
	{
		const PortalObject *portalObject = portal->GetObject();
		if ((portalObject->GetPerspectiveExclusionMask() & worldContext->perspectiveFlags) == 0)
		{
			const BoundingSphere *sphere = portal->GetBoundingSphere();
			if (rootRegion->SphereVisible(sphere->GetCenter(), sphere->GetRadius()))
			{
				const FrustumCamera *camera = worldContext->renderCamera;
				const Point3D& cameraPosition = camera->GetWorldPosition();
				
				const Antivector4D& portalPlane = portal->GetWorldPlane();
				float distance = portalPlane ^ cameraPosition;
				if ((distance >= 0.0F) && ((portalPlane ^ camera->GetWorldTransform()[2]) < camera->GetSineHalfField()))
				{
					Point3D				tempVertex[2][kMaxPortalVertexCount];
					MapReservation		reservation;
					
					int32 vertexCount = portalObject->GetVertexCount();
					const Point3D *vertex = portal->GetWorldVertexArray();
					
					if (distance > kMinPortalClipDistance)
					{
						int32 planeCount = rootRegion->GetPlaneCount() - rootRegion->GetPortalExcludePlaneCount();
						int32 count = Min(planeCount, kMaxPortalVertexCount - vertexCount);
						
						const Antivector4D *plane = rootRegion->GetPlaneArray();
						for (machine a = 0; a < count; a++)
						{
							int8	location[kMaxPortalVertexCount];
							
							Point3D *result = tempVertex[a & 1];
							vertexCount = Math::ClipPolygonAgainstPlane(vertexCount, vertex, plane[a], location, result);
							if (vertexCount == 0) return;
							vertex = result;
						}
						
						const Region *region = worldContext->occlusionList.First();
						while (region)
						{
							if (region->PolygonOccluded(vertexCount, vertex)) return;
							region = region->Next();
						}
					}
					else
					{
						if (Math::PointInConvexPolygon(cameraPosition, vertexCount, vertex, portalPlane.GetAntivector3D()) == kPolygonExterior) return;
						
						vertexCount = 4;
						vertex = camera->GetFrustumVertexArray();
					}
					
					PortalType portalType = portal->GetPortalType();
					if (portalType == kPortalDirect)
					{
						Zone *connectedZone = portal->GetConnectedZone();
						if ((connectedZone) && (connectedZone->GetExclusionMask() == 0))
						{
							CameraRegion *newRegion = new CameraRegion(camera, connectedZone);
							newRegion->SetFrustumPortalPlanes(vertexCount, vertex, portalPlane);
							rootRegion->AddSubnode(newRegion);
							
							const FogSpace **fsp = worldContext->fogSpacePtr;
							if (portalObject->GetPortalFlags() & kPortalFogInhibit) worldContext->fogSpacePtr = nullptr;
							ProcessCameraRegion(worldContext, newRegion);
							worldContext->fogSpacePtr = fsp;
							
							worldCounter[kWorldCounterDirectPortal]++;
						}
					}
					else if (portalType == kPortalRemote)
					{
						if (remoteRecursionCount < kMaxRemoteRecursionCount)
						{
							PortalBuffer buffer = static_cast<const RemotePortalObject *>(portalObject)->GetPortalBuffer();
							if (buffer == kPortalBufferReflection)
							{
								Map<PortalData> *portalMap = &worldContext->portalGroup[kPortalGroupReflection];
								if (portalMap->Reserve(portal, &reservation)) portalMap->Insert(new PortalData(portal, rootRegion->GetZone(), vertexCount, vertex), &reservation);
							}
							else if (buffer == kPortalBufferRefraction)
							{
								Map<PortalData> *portalMap = &worldContext->portalGroup[kPortalGroupRefraction];
								if (portalMap->Reserve(portal, &reservation)) portalMap->Insert(new PortalData(portal, rootRegion->GetZone(), vertexCount, vertex), &reservation);
							}
							else
							{
								Map<PortalData> *portalMap = &worldContext->portalGroup[kPortalGroupRemote];
								if (portalMap->Reserve(portal, &reservation)) portalMap->Insert(new PortalData(portal, rootRegion->GetZone(), vertexCount, vertex), &reservation);
							}
						}
					}
					else if (portalType == kPortalCamera)
					{
						if (cameraRecursionCount < kMaxCameraRecursionCount)
						{
							Map<PortalData> *portalMap = &worldContext->portalGroup[kPortalGroupCamera];
							if (portalMap->Reserve(portal, &reservation)) portalMap->Insert(new PortalData(portal, rootRegion->GetZone(), vertexCount, vertex), &reservation);
						}
					}
				}
			}
		}
	}
}

void World::ProcessCameraRegion(WorldContext *worldContext, CameraRegion *rootRegion)
{
	Zone *zone = rootRegion->GetZone();
	zone->AddCameraRegion(rootRegion);
	zone->SetExclusionMask(1);
	
	if ((worldContext->fogSpacePtr) && (!*worldContext->fogSpacePtr))
	{
		const FogSpace *fogSpace = zone->GetFirstFogSpace();
		while (fogSpace)
		{
			if ((fogSpace->Enabled()) && (ProcessFogSpace(worldContext, fogSpace, rootRegion))) break;
			fogSpace = fogSpace->Next();
		}
		
		if (!fogSpace)
		{
			fogSpace = zone->GetConnectedFogSpace();
			if ((fogSpace) && (fogSpace->Enabled())) ProcessFogSpace(worldContext, fogSpace, rootRegion);
		}
	}
	
	const OcclusionPortal *occlusionPortal = static_cast<OcclusionPortal *>(zone->GetFirstOcclusionPortal());
	while (occlusionPortal)
	{
		if ((occlusionPortal->Enabled()) && ((occlusionPortal->GetObject()->GetPerspectiveExclusionMask() & worldContext->perspectiveFlags) == 0))
		{
			const BoundingSphere *sphere = occlusionPortal->GetBoundingSphere();
			if (rootRegion->SphereVisible(sphere->GetCenter(), sphere->GetRadius()))
			{
				CameraRegion *newRegion = occlusionPortal->CalculateFrustumOcclusionRegion(worldContext->renderCamera, zone);
				if (newRegion)
				{
					worldContext->occlusionList.Append(newRegion);
					worldCounter[kWorldCounterOcclusionRegion]++;
				}
			}
		}
		
		occlusionPortal = static_cast<OcclusionPortal *>(occlusionPortal->Next());
	}
	
	const OcclusionSpace *occlusionSpace = zone->GetFirstOcclusionSpace();
	while (occlusionSpace)
	{
		if ((occlusionSpace->Enabled()) && (NodeVisible(occlusionSpace, rootRegion, &worldContext->occlusionList)))
		{
			CameraRegion *newRegion = occlusionSpace->CalculateFrustumOcclusionRegion(worldContext->renderCamera, zone);
			if (newRegion)
			{
				worldContext->occlusionList.Append(newRegion);
				worldCounter[kWorldCounterOcclusionRegion]++;
			}
		}
		
		occlusionSpace = occlusionSpace->Next();
	}
	
	Portal *portal = zone->GetFirstPortal();
	while (portal)
	{
		ProcessPortal(worldContext, portal, rootRegion);
		portal = portal->Next();
	}
	
	const Bond *bond = zone->GetZoneSite()->GetFirstOutgoingEdge();
	while (bond)
	{
		Zone *bondZone = static_cast<Zone *>(bond->GetFinishElement());
		if (bondZone->GetExclusionMask() == 0)
		{
			CameraRegion *newRegion = new CameraRegion(worldContext->renderCamera, bondZone, rootRegion);
			rootRegion->AddSubnode(newRegion);
			
			ProcessCameraRegion(worldContext, newRegion);
		}
		
		bond = bond->GetNextOutgoingEdge();
	}
	
	zone->SetExclusionMask(0);
}

void World::RenderIndirectPortals(const WorldContext *worldContext)
{
	const PortalData *portalData = worldContext->portalGroup[kPortalGroupCamera].First();
	while (portalData)
	{
		const CameraPortal *cameraPortal = static_cast<const CameraPortal *>(portalData->GetPortal());
		FrustumCamera *targetCamera = cameraPortal->GetTargetCamera();
		if (targetCamera)
		{
			Zone	*targetZone;
			
			const CameraPortalObject *portalObject = cameraPortal->GetObject();
			int32 width = portalObject->GetViewportWidth();
			int32 height = portalObject->GetViewportHeight();
			
			if (width > renderWidth)
			{
				height = Max(renderWidth * height / width, 32);
				width = renderWidth;
			}
			
			if (height > renderHeight)
			{
				width = Max(renderHeight * width / height, 32);
				height = renderHeight;
			}
			
			int32 prevRenderWidth = renderWidth;
			int32 prevRenderHeight = renderHeight;
			renderWidth = width;
			renderHeight = height;
			
			int32 displayHeight = TheDisplayMgr->GetDisplayHeight();
			Rect viewportRect(0, displayHeight - height, width, displayHeight);
			cameraPortal->CallRenderSizeProc(width, height);
			
			FrustumCameraObject *cameraObject = targetCamera->GetObject();
			cameraObject->SetAspectRatio((float) height / (float) width);
			cameraObject->SetViewRect(viewportRect);
			SetCameraClearParams(cameraObject);
			
			if (!(cameraObject->GetCameraFlags() & kCameraExternalZone)) targetZone = targetCamera->GetOwningZone();
			else targetZone = FindZone(targetCamera->GetWorldPosition(), true);
			
			CameraRegion cameraRegion(targetCamera, targetZone);
			targetCamera->CalculateFrustumCameraRegion(&cameraRegion);
			
			WorldContext portalContext(targetCamera);
			portalContext.skyboxNode = worldContext->skyboxNode;
			portalContext.perspectiveFlags = kPerspectiveCameraWidget;
			portalContext.cameraMinDetailLevel = Max(worldContext->cameraMinDetailLevel, portalObject->GetMinDetailLevel());
			portalContext.cameraDetailBias = worldContext->cameraDetailBias + portalObject->GetDetailLevelBias();
			portalContext.shadowReceiveRegion = &cameraRegion;
			
			cameraRecursionCount++;
			
			RenderCamera(&portalContext, &cameraRegion, kRenderTargetPrimary);
			TheGraphicsMgr->CopyRenderTarget(cameraPortal->GetCameraTexture(), viewportRect);
			
			cameraRecursionCount--;
			
			renderWidth = prevRenderWidth;
			renderHeight = prevRenderHeight;
		}
		
		portalData = portalData->Next();
	}
	
	portalData = worldContext->portalGroup[kPortalGroupReflection].First();
	while (portalData)
	{
		RemotePortal *remotePortal = static_cast<RemotePortal *>(portalData->GetPortal());
		RenderRemoteCamera(worldContext, remotePortal, kRenderTargetReflection, kPerspectiveReflection, portalData);
		portalData = portalData->Next();
	}
	
	portalData = worldContext->portalGroup[kPortalGroupRefraction].First();
	while (portalData)
	{
		RemotePortal *remotePortal = static_cast<RemotePortal *>(portalData->GetPortal());
		RenderRemoteCamera(worldContext, remotePortal, kRenderTargetRefraction, kPerspectiveRefraction, portalData);
		portalData = portalData->Next();
	}
	
	portalData = worldContext->portalGroup[kPortalGroupRemote].First();
	while (portalData)
	{
		RemotePortal *remotePortal = static_cast<RemotePortal *>(portalData->GetPortal());
		RenderRemoteCamera(worldContext, remotePortal, kRenderTargetPrimary, kPerspectiveRemotePortal, portalData);
		portalData = portalData->Next();
	}
}

void World::RenderRemoteCamera(const WorldContext *worldContext, RemotePortal *remotePortal, RenderTargetType target, unsigned_int32 perspectiveFlag, const PortalData *portalData)
{
	Zone *remoteZone = remotePortal->GetConnectedZone();
	if (!remoteZone) remoteZone = portalData->GetOriginZone();
	
	int32 vertexCount = portalData->GetVertexCount();
	const Point3D *vertex = portalData->GetVertexArray();
	
	Transform4D remoteTransform = remotePortal->GetWorldTransform() * remotePortal->GetRemoteTransform() * remotePortal->GetInverseWorldTransform();
	
	const FrustumCamera *camera = worldContext->renderCamera;
	const FrustumCameraObject *cameraObject = camera->GetObject();
	
	const RemotePortalObject *portalObject = remotePortal->GetObject();
	const Antivector4D& plane = remotePortal->GetWorldPlane();
	
	RemoteCamera remoteCamera(cameraObject->GetFocalLength() * portalObject->GetFocalLengthMultiplier(), cameraObject->GetAspectRatio(), remoteTransform, Antivector4D(plane.GetAntivector3D(), plane.w - portalObject->GetPortalPlaneOffset()));
	RemoteCameraObject *remoteCameraObject = static_cast<RemoteCameraObject *>(remoteCamera.GetObject());
	
	unsigned_int32 portalFlags = portalObject->GetPortalFlags();
	if (portalFlags & kPortalObliqueFrustum) remoteCameraObject->SetFrustumFlags(kFrustumInfinite | kFrustumOblique);
	else remoteCameraObject->SetFrustumFlags(kFrustumInfinite);
	
	SetCameraClearParams(remoteCameraObject);
	if (portalFlags & kPortalOverrideClearColor) remoteCameraObject->SetClearColor(portalObject->GetPortalClearColor());
	remoteCameraObject->SetNearDepth(cameraObject->GetNearDepth());
	remoteCameraObject->SetFarDepth(cameraObject->GetFarDepth());
	remoteCameraObject->SetViewRect(cameraObject->GetViewRect());
	
	remoteCamera.SetNodeTransform(camera->GetWorldTransform());
	remoteCamera.Invalidate();
	remoteCamera.Update();
	
	const Transform4D& previousCameraWorldTransform = remotePortal->GetPreviousCameraWorldTransform();
	if (previousCameraWorldTransform(3,3) != 0.0F) remoteCamera.SetPreviousWorldTransform(previousCameraWorldTransform);
	remotePortal->SetPreviousCameraWorldTransform(remoteCamera.GetWorldTransform());
	
	WorldContext portalContext(&remoteCamera);
	portalContext.skyboxNode = (!(portalFlags & kPortalSkyboxInhibit)) ? worldContext->skyboxNode : nullptr;
	portalContext.perspectiveFlags = (worldContext->perspectiveFlags & ~kPerspectiveDirect) | perspectiveFlag;
	portalContext.cameraMinDetailLevel = Max(worldContext->cameraMinDetailLevel, portalObject->GetMinDetailLevel());
	portalContext.cameraDetailBias = worldContext->cameraDetailBias + portalObject->GetDetailLevelBias();
	
	remoteRecursionCount++;
	unsigned_int32 nodeFlags = remotePortal->GetNodeFlags();
	if (!(portalFlags & kPortalRecursive)) remotePortal->SetNodeFlags(nodeFlags | kNodeDisabled);
	
	CameraRegion cameraRegion(&remoteCamera, remoteZone);
	CameraRegion receiveRegion(&remoteCamera, remoteZone);
	remoteCamera.CalculateRemoteCameraRegion(vertexCount, vertex, &cameraRegion);
	
	bool keepFlag = ((perspectiveFlag == kPerspectiveRefraction) && (!(portalFlags & kPortalSeparateShadowMap)));
	if (keepFlag)
	{
		remoteCamera.CalculateFrustumCameraRegion(&receiveRegion);
		portalContext.shadowReceiveRegion = &receiveRegion;
	}
	else
	{
		portalContext.shadowReceiveRegion = &cameraRegion;
	}
	
	if (!(portalFlags & kPortalDistant)) RenderCamera(&portalContext, &cameraRegion, target);
	else RenderDistantCamera(&portalContext, &cameraRegion, target);
	
	worldContext->shadowMapKeepFlag = keepFlag;
	
	remoteRecursionCount--;
	remotePortal->SetNodeFlags(nodeFlags);
	
	worldCounter[kWorldCounterRemotePortal]++;
}

void World::RenderCamera(WorldContext *worldContext, CameraRegion *cameraRegion, RenderTargetType target)
{
	#if !C4SERVER
	
		currentWorldContext = worldContext;
		
		const FogSpace *cameraFogSpace = nullptr;
		worldContext->fogSpacePtr = &cameraFogSpace;
		
		ProcessCameraRegion(worldContext, cameraRegion);
		RenderIndirectPortals(worldContext);
		
		geometryRenderStamp++;
		impostorRenderStamp++;
		
		TheGraphicsMgr->SetRenderTarget(target);
		TheGraphicsMgr->SetCamera(worldContext->renderCamera->GetObject(), worldContext->renderCamera);
		// comment by skkim - test		
		//if (cameraFogSpace)
		//	TheGraphicsMgr->SetFogSpace(cameraFogSpace->GetObject(), cameraFogSpace);
		
		RenderAmbientRegion(worldContext, cameraRegion);
		
		TerrainLevelGeometry *terrain = terrainList.First();
		while (terrain)
		{
			terrain->UpdateBorderState();
			terrain = terrain->Next();
		}
		
		ImpostorSystem *system = impostorSystemMap.First();
		while (system)
		{
			system->RenderSystem(&renderStageList[kRenderStageImpostor]);
			system = system->Next();
		}
		
		unsigned_int32 structureFlags = kStructureRenderVelocity | kStructureRenderDepth | kStructureRenderGradient;
		
		if (worldFlags & kWorldClearColor)
			structureFlags |= kStructureClearBuffer;
		
		if (worldContext->skyboxFlag)
		{
			Skybox *skybox = worldContext->skyboxNode;
			if ((skybox) && (skybox->Render(worldContext->renderCamera, &renderStageList[kRenderStageDefault]))) structureFlags |= kStructureClearBuffer;
		}
		
		FinishWorldBatch();
		
		bool structurePass = false;
		if (worldContext->perspectiveFlags == kPerspectiveDirect)
		{
			if (worldFlags & kWorldMotionBlurInhibit) structureFlags &= ~kStructureRenderVelocity;
			else if (worldFlags & kWorldZeroBackgroundVelocity) structureFlags |= kStructureZeroBackgroundVelocity;
			
			float velocityScale = velocityNormalizationTime / Fmax(TheTimeMgr->GetSystemFloatDeltaTime(), 1.0F);
			
			if (TheGraphicsMgr->BeginStructureRendering(worldContext->renderCamera->GetPreviousWorldTransform(), structureFlags, velocityScale))
			{
				TheGraphicsMgr->DrawStructureList(&renderStageList[kRenderStageCover]);
				
				TheGraphicsMgr->GroupAmbientRenderList(&renderStageList[kRenderStageDefault]);
				TheGraphicsMgr->DrawStructureList(&renderStageList[kRenderStageDefault]);
				
				TheGraphicsMgr->GroupAmbientRenderList(&renderStageList[kRenderStageAlphaTest]);
				TheGraphicsMgr->DrawStructureList(&renderStageList[kRenderStageAlphaTest]);
				
				TheGraphicsMgr->DrawStructureList(&renderStageList[kRenderStageImpostor]);
				TheGraphicsMgr->DrawStructureList(&renderStageList[kRenderStageEffectVelocity]);
				
				TheGraphicsMgr->EndStructureRendering();
				structurePass = true;
			}
		}
		
		if (structurePass)
		{
			TheGraphicsMgr->DrawRenderList(&renderStageList[kRenderStageCover]);
			TheGraphicsMgr->DrawRenderList(&renderStageList[kRenderStageDefault]);
			TheGraphicsMgr->DrawRenderList(&renderStageList[kRenderStageAlphaTest]);
			TheGraphicsMgr->DrawRenderList(&renderStageList[kRenderStageImpostor]);
			
			TheGraphicsMgr->GroupAmbientRenderList(&renderStageList[kRenderStageDecal]);
			TheGraphicsMgr->DrawRenderList(&renderStageList[kRenderStageDecal]);
			
			BlobParticleSystem::FinishBatches(&renderStageList[kRenderStageFirstEffect]);
			TheGraphicsMgr->DrawRenderList(&renderStageList[kRenderStageEffectLight]);
		}
		else
		{
			TheGraphicsMgr->DrawRenderList(&renderStageList[kRenderStageCover]);
			
			TheGraphicsMgr->GroupAmbientRenderList(&renderStageList[kRenderStageDefault]);
			TheGraphicsMgr->DrawRenderList(&renderStageList[kRenderStageDefault]);
			
			TheGraphicsMgr->GroupAmbientRenderList(&renderStageList[kRenderStageAlphaTest]);
			TheGraphicsMgr->DrawRenderList(&renderStageList[kRenderStageAlphaTest]);
			
			TheGraphicsMgr->DrawRenderList(&renderStageList[kRenderStageImpostor]);
			
			TheGraphicsMgr->GroupAmbientRenderList(&renderStageList[kRenderStageDecal]);
			TheGraphicsMgr->DrawRenderList(&renderStageList[kRenderStageDecal]);
			
			BlobParticleSystem::FinishBatches(&renderStageList[kRenderStageFirstEffect]);
			TheGraphicsMgr->DrawRenderList(&renderStageList[kRenderStageEffectLight]);
		}
		
		renderStageList[kRenderStageDefault].RemoveAll();
		renderStageList[kRenderStageAlphaTest].RemoveAll();
		renderStageList[kRenderStageImpostor].RemoveAll();
		renderStageList[kRenderStageCover].RemoveAll();
		renderStageList[kRenderStageDecal].RemoveAll();
		
		if (!(worldFlags & kWorldAmbientOnly))
		{
			TheGraphicsMgr->DrawRenderList(&renderStageList[kRenderStageEffectOcclusion]);
			
			CollectLightRegions(worldContext, cameraRegion);
			cameraRegion->AddOppositePlane();
			
			int32 lightDetailLevel = TheWorldMgr->GetLightDetailLevel();
			for (;;)
			{
				Reference<LightRegion> *lightRegionReference = lightRegionList.First();
				if (!lightRegionReference) break;
				
				LightRegion *lightRegion = lightRegionReference->GetTarget();
				Light *light = lightRegion->GetLight();
				
				if ((light->Enabled()) && (light->GetObject()->GetMinDetailLevel() <= lightDetailLevel))
				{
					worldCounter[kWorldCounterLight]++;
					
					if (light->GetBaseLightType() == kLightInfinite)
					{
						InfiniteLight *infiniteLight = static_cast<InfiniteLight *>(light);
						RenderInfiniteLight(worldContext, infiniteLight);
					}
					else
					{
						PointLight *pointLight = static_cast<PointLight *>(light);
						RenderPointLight(worldContext, pointLight);
					}
				}
				else
				{
					do
					{
						Reference<LightRegion> *nextLightRegionReference = lightRegionReference->Next();
						
						if (lightRegionReference->GetTarget()->GetLight() == light)
							delete lightRegionReference;
						
						lightRegionReference = nextLightRegionReference;
					} while (lightRegionReference);
				}
			}
			
			TheGraphicsMgr->SetAmbient();
			
			TheGraphicsMgr->DrawRenderList(&renderStageList[kRenderStageEffectOpaque]);
			TheGraphicsMgr->DrawRenderList(&renderStageList[kRenderStageEffectVelocity]);
			TheGraphicsMgr->DrawRenderList(&renderStageList[kRenderStageEffectCover]);
			
			TheGraphicsMgr->SortRenderList(&renderStageList[kRenderStageEffectTransparent]);
			TheGraphicsMgr->DrawRenderList(&renderStageList[kRenderStageEffectTransparent]);
			TheGraphicsMgr->DrawRenderList(&renderStageList[kRenderStageEffectFrontmost]);
			
			TheGraphicsMgr->ProcessOcclusionQueries();
		}
		
		TheGraphicsMgr->SetFogSpace(nullptr, nullptr);
		
		if ((!renderStageList[kRenderStageEffectDistortion].Empty()) && (worldContext->perspectiveFlags == kPerspectiveDirect))
		{
			if (TheGraphicsMgr->BeginDistortionRendering())
			{
				TheGraphicsMgr->DrawDistortionList(&renderStageList[kRenderStageEffectDistortion]);
				TheGraphicsMgr->EndDistortionRendering();
			}
		}
		
		for (machine a = kRenderStageFirstEffect; a <= kRenderStageLastEffect; a++)
			renderStageList[a].RemoveAll();
		
		#if C4DIAGNOSTICS
		
			if (!shadowRegionDiagnosticList.Empty())
			{
				TheGraphicsMgr->DrawRenderList(&shadowRegionDiagnosticList);
				TheGraphicsMgr->DrawWireframe(kWireframeTwoSided | kWireframeColor, &shadowRegionDiagnosticList);
			}
			
			if (diagnosticFlags & kDiagnosticSourcePaths) 
				RenderSourcePaths(cameraRegion->GetZone(), TheSoundMgr->GetListenerTransformable()->GetWorldTransform());
			
			if (!rigidBodyDiagnosticList.Empty()) TheGraphicsMgr->DrawRenderList(&rigidBodyDiagnosticList);
			if (!contactDiagnosticList.Empty()) TheGraphicsMgr->DrawRenderList(&contactDiagnosticList);
		
		#endif
		
		terrainList.RemoveAll();
	
	#endif
}

void World::RenderDistantCamera(WorldContext *worldContext, CameraRegion *cameraRegion, RenderTargetType target)
{
	#if !C4SERVER
	
		currentWorldContext = worldContext;
		
		TheGraphicsMgr->SetRenderTarget(target);
		TheGraphicsMgr->SetCamera(worldContext->renderCamera->GetObject(), worldContext->renderCamera);
		
		const Zone *zone = cameraRegion->GetZone();
		if (zone->GetObject()->GetZoneFlags() & kZoneRenderSkybox)
		{
			Skybox *skybox = worldContext->skyboxNode;
			if (skybox)
			{
				const FogSpace *cameraFogSpace = nullptr;
				worldContext->fogSpacePtr = &cameraFogSpace;
				
				const FogSpace *fogSpace = zone->GetFirstFogSpace();
				while (fogSpace)
				{
					if ((fogSpace->Enabled()) && (ProcessFogSpace(worldContext, fogSpace, cameraRegion))) break;
					fogSpace = fogSpace->Next();
				}
				
				if (!fogSpace)
				{
					fogSpace = zone->GetConnectedFogSpace();
					if ((fogSpace) && (fogSpace->Enabled())) ProcessFogSpace(worldContext, fogSpace, cameraRegion);
				}
				
				if (cameraFogSpace) TheGraphicsMgr->SetFogSpace(cameraFogSpace->GetObject(), cameraFogSpace);
				
				skybox->Render(worldContext->renderCamera, &renderStageList[kRenderStageDefault]);
				TheGraphicsMgr->DrawRenderList(&renderStageList[kRenderStageDefault]);
				renderStageList[kRenderStageDefault].RemoveAll();
				
				TheGraphicsMgr->SetFogSpace(nullptr, nullptr);
			}
		}
	
	#endif
}

void World::BeginRendering(void)
{
	if (worldFlags & kWorldPostColorMatrix) TheGraphicsMgr->SetFinalColorTransform(finalColorScale[0], finalColorScale[1], finalColorScale[2], finalColorBias);
	else TheGraphicsMgr->SetFinalColorTransform(finalColorScale[0], finalColorBias);
	
	TheGraphicsMgr->SetShaderTime(shaderTime, TheTimeMgr->GetFloatDeltaTime());
}

void World::EndRendering(void)
{
	TheGraphicsMgr->SetRenderTarget(kRenderTargetDisplay);
	
	shaderTime = PositiveFrac((shaderTime + TheTimeMgr->GetFloatDeltaTime()) * kInverseShaderTimePeriod) * kShaderTimePeriod;
	
	#if C4DIAGNOSTICS
	
		diagnosticFlags &= ~kDiagnosticShadowRegions;
	
	#endif
}

void World::SetFinalColorTransform(const ColorRGBA& scale, const ColorRGBA& bias)
{
	finalColorScale[0] = scale;
	finalColorBias = bias;
	worldFlags &= ~kWorldPostColorMatrix;
}

void World::SetFinalColorTransform(const ColorRGBA& red, const ColorRGBA& green, const ColorRGBA& blue, const ColorRGBA& bias)
{
	finalColorScale[0] = red;
	finalColorScale[1] = green;
	finalColorScale[2] = blue;
	finalColorBias = bias;
	worldFlags |= kWorldPostColorMatrix;
}

void World::Render(void)
{
	const FrustumCamera *camera = currentCamera;
	if (camera)
	{
		ImpostorSystem *system = impostorSystemMap.First();
		while (system)
		{
			system->Build();
			system = system->Next();
		}
		
		remoteRecursionCount = 0;
		cameraRecursionCount = 0;
		
		WorldContext worldContext(camera);
		CameraRegion rootRegion(camera, cameraZone);
		camera->CalculateFrustumCameraRegion(&rootRegion);

		Skybox *skybox = worldSkybox;
		worldContext.skyboxNode = ((skybox) && (skybox->Enabled())) ? skybox : nullptr;
				
		worldContext.perspectiveFlags = kPerspectiveDirect | worldPerspective;
		
		worldContext.cameraMinDetailLevel = 0;
		worldContext.cameraDetailBias = 0.0F;
		worldContext.shadowReceiveRegion = &rootRegion;
		
		RenderCamera(&worldContext, &rootRegion, kRenderTargetPrimary);
	}
}

void World::RenderShadowMapNode(const WorldContext *worldContext, Node *node, const Region *cameraRegion, const Region *shadowRegion, const List<Region> *occlusionList)
{
	NodeType type = node->GetNodeType();
	if ((type != kNodeZone) && (node->Enabled()) && (node->Visible(cameraRegion)) && (NodeVisible(node, shadowRegion, occlusionList)))
	{
		if ((type & 0x00FFFFFF) != 0x00424C4B)
		{
			if (type == kNodeGeometry)
			{
				Geometry *geometry = static_cast<Geometry *>(node);
				if (!geometry->ListElement<Renderable>::GetOwningList())
				{
					unsigned_int32 flags = geometry->GetObject()->GetGeometryFlags();
					if ((flags & kGeometryRenderShadowMap) && ((geometry->GetShadowPerspectiveExclusionMask() & worldContext->perspectiveFlags) == 0))
					{
						worldCounter[kWorldCounterDepthShadow]++;
						
						unsigned_int32 renderStamp = geometryRenderStamp;
						if (geometry->GetNodeStamp() != renderStamp)
						{
							geometry->SetNodeStamp(renderStamp);
							ProcessGeometry(worldContext, geometry);
						}
						
						renderStageList[kRenderStageDefault].Append(geometry);
					}
				}
			}
			else if (type == kNodeImpostor)
			{
				Impostor *impostor = static_cast<Impostor *>(node);
					
				float distance = SquaredMag(impostor->GetWorldPosition().GetVector2D() - worldContext->renderCamera->GetWorldPosition().GetVector2D());
				if (distance > impostor->GetSquaredRenderDistance())
				{
					unsigned_int32 renderStamp = impostorRenderStamp;
					if (impostor->GetNodeStamp() != renderStamp)
					{
						impostor->SetNodeStamp(renderStamp);
						impostor->Render();
					}
					
					if (distance > impostor->GetSquaredGeometryDistance()) return;
				}
			}
			
			const Bond *bond = node->GetFirstOutgoingEdge();
			while (bond)
			{
				RenderShadowMapNode(worldContext, static_cast<Node *>(bond->GetFinishElement()), cameraRegion, shadowRegion, occlusionList);
				bond = bond->GetNextOutgoingEdge();
			}
		}
		else if (type == kNodeTerrainBlock)
		{
			const FrustumCamera *camera = worldContext->renderCamera;
			float inverseFocal = 1.0F / camera->GetObject()->GetFocalLength();
			
			Node *subnode = node->GetFirstSubnode();
			while (subnode)
			{
				if (subnode->GetNodeType() == kNodeGeometry)
				{
					TerrainGeometry *terrain = static_cast<TerrainGeometry *>(subnode);
					const Box3D& box = terrain->GetWorldBoundingBox();
					if ((cameraRegion->BoxVisible(box)) && (WorldBoundingBoxVisible(box, shadowRegion, occlusionList)))
					{
						int32 level = terrain->GetObject()->GetDetailLevel();
						if (level <= worldContext->cameraMinDetailLevel)
						{
							if ((terrain->GetShadowPerspectiveExclusionMask() & worldContext->perspectiveFlags) == 0)
							{
								worldCounter[kWorldCounterDepthShadow]++;
								renderStageList[kRenderStageDefault].Append(terrain);
								
								if (level != 0)
								{
									TerrainLevelGeometry *terrainLevel = static_cast<TerrainLevelGeometry *>(terrain);
									if (!terrainLevel->Rendering()) terrainLevel->UpdateBorderState();
								}
							}
							
							subnode = node->GetNextLevelNode(terrain);
							continue;
						}
						else
						{
							Vector3D direction = terrain->GetWorldCenter() - camera->GetWorldPosition();
							float d = Magnitude(direction) * inverseFocal;
							if ((d > terrain->GetRenderDistance()) && (camera->GetWorldTransform()[2] * direction > 0.0F))
							{
								if ((terrain->GetShadowPerspectiveExclusionMask() & worldContext->perspectiveFlags) == 0)
								{
									worldCounter[kWorldCounterDepthShadow]++;
									renderStageList[kRenderStageDefault].Append(terrain);
									
									TerrainLevelGeometry *terrainLevel = static_cast<TerrainLevelGeometry *>(terrain);
									if (!terrainLevel->Rendering()) terrainLevel->UpdateBorderState();
								}
								
								subnode = node->GetNextLevelNode(terrain);
								continue;
							}
						}
						
						subnode = node->GetNextNode(subnode);
						continue;
					}
				}
				
				subnode = node->GetNextLevelNode(subnode);
			}
		}
	}
}

void World::RenderShadowMapCell(const WorldContext *worldContext, const Site *cell, const Region *cameraRegion, const Region *shadowRegion, const List<Region> *occlusionList)
{
	const Box3D& box = cell->GetWorldBoundingBox();
	if ((WorldBoundingBoxVisible(box, cameraRegion, occlusionList))  && (shadowRegion->BoxVisible(box)))
	{
		const Bond *bond = cell->GetFirstOutgoingEdge();
		while (bond)
		{
			Site *site = bond->GetFinishElement();
			if (site->GetCellIndex() < 0) RenderShadowMapNode(worldContext, static_cast<Node *>(site), cameraRegion, shadowRegion, occlusionList);
			else RenderShadowMapCell(worldContext, site, cameraRegion, shadowRegion, occlusionList);
			
			bond = bond->GetNextOutgoingEdge();
		}
	}
}

void World::RenderShadowMapRegion(const WorldContext *worldContext, const CameraRegion *cameraRegion, const Region *shadowRegion, const List<Region> *occlusionList)
{
	const Zone *zone = cameraRegion->GetZone();
	if ((zone->Visible(cameraRegion)) && (NodeVisible(zone, shadowRegion, occlusionList)))
	{
		const Bond *bond = zone->GetFirstOutgoingEdge();
		while (bond)
		{
			Site *site = bond->GetFinishElement();
			if (site->GetCellIndex() < 0) RenderShadowMapNode(worldContext, static_cast<Node *>(site), cameraRegion, shadowRegion, occlusionList);
			else RenderShadowMapCell(worldContext, site, cameraRegion, shadowRegion, occlusionList);
			
			bond = bond->GetNextOutgoingEdge();
		}
	}
	
	const CameraRegion *subregion = cameraRegion->GetFirstSubnode();
	while (subregion)
	{
		RenderShadowMapRegion(worldContext, subregion, shadowRegion, occlusionList);
		subregion = subregion->Tree<CameraRegion>::Next();
	}
}

void World::ProcessShadowMapRegion(const WorldContext *worldContext, const OrthoCamera *camera, CameraRegion *rootRegion, const Region *shadowRegion, List<Region> *occlusionList)
{
	const Point3D& cameraPosition = camera->GetWorldPosition();
	
	Zone *zone = rootRegion->GetZone();
	zone->SetExclusionMask(1);
	
	const Portal *portal = zone->GetFirstPortal();
	while (portal)
	{
		if ((portal->Enabled()) && (portal->GetPortalType() == kPortalDirect))
		{
			const PortalObject *object = portal->GetObject();
			if (!(object->GetPortalFlags() & kPortalShadowMapInhibit))
			{
				Zone *connectedZone = portal->GetConnectedZone();
				if ((connectedZone) && (connectedZone->GetExclusionMask() == 0))
				{
					const Antivector4D& portalPlane = portal->GetWorldPlane();
					float distance = portalPlane ^ cameraPosition;
					if ((distance >= 0.0F) && ((portalPlane ^ camera->GetWorldTransform()[2]) < 0.0F))
					{
						const BoundingSphere *sphere = portal->GetBoundingSphere();
						if ((rootRegion->SphereVisible(sphere->GetCenter(), sphere->GetRadius())) && (shadowRegion->SphereVisible(sphere->GetCenter(), sphere->GetRadius())))
						{
							Point3D		temp[2][kMaxPortalVertexCount];
							
							int32 vertexCount = object->GetVertexCount();
							const Point3D *vertex = portal->GetWorldVertexArray();
							
							int32 count = Min(rootRegion->GetPlaneCount(), kMaxPortalVertexCount - vertexCount);
							const Antivector4D *plane = rootRegion->GetPlaneArray();
							for (machine a = 0; a < count; a++)
							{
								int8	location[kMaxPortalVertexCount];
								
								Point3D *result = temp[a & 1];
								vertexCount = Math::ClipPolygonAgainstPlane(vertexCount, vertex, plane[a], location, result);
								if (vertexCount == 0) goto nextPortal;
								vertex = result;
							}
							
							CameraRegion *newRegion = new CameraRegion(camera, connectedZone);
							newRegion->SetOrthoPortalPlanes(vertexCount, vertex, rootRegion);
							
							rootRegion->AddSubnode(newRegion);
							ProcessShadowMapRegion(worldContext, camera, newRegion, shadowRegion, occlusionList);
						}
					}
				}
			}
		}
		
		nextPortal:
		portal = portal->Next();
	}
	
	const Portal *occlusionPortal = zone->GetFirstOcclusionPortal();
	while (occlusionPortal)
	{
		if ((occlusionPortal->Enabled()) && ((occlusionPortal->GetObject()->GetPerspectiveExclusionMask() & worldContext->perspectiveFlags) == 0))
		{
			const BoundingSphere *sphere = occlusionPortal->GetBoundingSphere();
			if ((rootRegion->SphereVisible(sphere->GetCenter(), sphere->GetRadius())) && (shadowRegion->SphereVisible(sphere->GetCenter(), sphere->GetRadius())))
			{
				const Antivector4D& portalPlane = occlusionPortal->GetWorldPlane();
				float distance = portalPlane ^ camera->GetWorldPosition();
				if ((distance > 0.0F) && ((portalPlane ^ camera->GetWorldTransform()[2]) < 0.0F))
				{
					CameraRegion *newRegion = new CameraRegion(camera, zone);
					newRegion->SetOrthoPortalPlanes(occlusionPortal->GetObject()->GetVertexCount(), occlusionPortal->GetWorldVertexArray(), portalPlane);
					occlusionList->Append(newRegion);
				}
			}
		}
		
		occlusionPortal = occlusionPortal->Next();
	}
	
	const OcclusionSpace *occlusionSpace = zone->GetFirstOcclusionSpace();
	while (occlusionSpace)
	{
		if ((occlusionSpace->Enabled()) && (NodeVisible(occlusionSpace, rootRegion, occlusionList)) && (NodeVisible(occlusionSpace, shadowRegion, occlusionList)))
		{
			CameraRegion *newRegion = occlusionSpace->CalculateOrthoOcclusionRegion(camera, zone);
			if (newRegion) occlusionList->Append(newRegion);
		}
		
		occlusionSpace = occlusionSpace->Next();
	}
	
	const Bond *bond = zone->GetZoneSite()->GetFirstOutgoingEdge();
	while (bond)
	{
		Zone *bondZone = static_cast<Zone *>(bond->GetFinishElement());
		if (bondZone->GetExclusionMask() == 0)
		{
			if ((bondZone->Visible(rootRegion)) && (bondZone->Visible(shadowRegion)))
			{
				CameraRegion *newRegion = new CameraRegion(camera, bondZone, rootRegion);
				rootRegion->AddSubnode(newRegion);
				ProcessShadowMapRegion(worldContext, camera, newRegion, shadowRegion, occlusionList);
			}
		}
		
		bond = bond->GetNextOutgoingEdge();
	}
	
	zone->SetExclusionMask(0);
}

void World::RenderShadowMap(const WorldContext *worldContext, DepthLight *depthLight, int32 sectionIndex, const LightShadowData *shadowData, const Region *shadowRegion)
{
	OrthoCamera		orthoCamera;
	List<Region>	occlusionList;
	
	impostorRenderStamp++;
	
	orthoCamera.SetWorld(this);
	OrthoCameraObject *cameraObject = orthoCamera.GetObject();
	
	int32 shadowMapSize = TheGraphicsMgr->GetDynamicShadowMapSize();
	int32 y = shadowMapSize * (kMaxShadowSectionCount - 1 - sectionIndex);
	
	cameraObject->SetViewRect(Rect(0, y, shadowMapSize, y + shadowMapSize));
	cameraObject->SetNearDepth(0.0F);
	cameraObject->SetFarDepth(shadowData->shadowSize.z);
	
	float w = shadowData->shadowSize.x * 0.5F;
	float h = shadowData->shadowSize.y * 0.5F;
	cameraObject->SetOrthoRect(-w, w, -h, h);
	
	orthoCamera.SetNodeMatrix3D(K::minus_x_unit, K::y_unit, K::minus_z_unit);
	orthoCamera.SetNodePosition(Point3D(shadowData->shadowPosition.x, shadowData->shadowPosition.y, shadowData->shadowPosition.z + shadowData->texelSize * 2.0F));
	depthLight->AddNewSubnode(&orthoCamera);
	orthoCamera.Update();
	
	CameraRegion rootRegion(&orthoCamera, depthLight->GetOwningZone());
	orthoCamera.CalculateOrthoCameraRegion(&rootRegion);
	
	TheGraphicsMgr->SetCamera(cameraObject, &orthoCamera, ~0, false);
	
	ProcessShadowMapRegion(worldContext, &orthoCamera, &rootRegion, shadowRegion, &occlusionList);
	RenderShadowMapRegion(worldContext, &rootRegion, shadowRegion, &occlusionList);
	
	ImpostorSystem *system = impostorSystemMap.First();
	while (system)
	{
		system->RenderSystem(&renderStageList[kRenderStageDefault]);
		system = system->Next();
	}
	
	FinishWorldBatch();
	
	TheGraphicsMgr->DrawShadowMapList(&renderStageList[kRenderStageDefault]);
	renderStageList[kRenderStageDefault].RemoveAll();
}


WorldMgr::WorldMgr(int) :
		objectConstructor(&ConstructObject),
		controllerStateSender(&SendControllerState, this),
		displayEventHandler(&HandleDisplayEvent, this),
		lightDetailLevelObserver(this, &WorldMgr::HandleLightDetailLevelEvent)
{
}

WorldMgr::~WorldMgr()
{
}

EngineResult WorldMgr::Construct(void)
{
	currentWorld = nullptr;
	
	worldConstructorProc = nullptr;
	Object::InstallConstructor(&objectConstructor);
	TheMessageMgr->InstallStateSender(&controllerStateSender);
	TheDisplayMgr->InstallDisplayEventHandler(&displayEventHandler);
	
	unsigned_int32 speed = TheGraphicsMgr->GetCapabilities()->hardwareSpeed;
	TheEngine->InitVariable("lightDetailLevel", (speed >= 2) ? "2" : ((speed >= 1) ? "1" : "0"), kVariablePermanent, &lightDetailLevelObserver);
	
	defaultVelocityNormalizationTime = 8.33333F;
	
	Controller::RegisterStandardControllers();
	Property::RegisterStandardProperties();
	Modifier::RegisterStandardModifiers();
	Mutator::RegisterStandardMutators();
	Process::RegisterStandardProcesses();
	Widget::RegisterStandardWidgets();
	Method::RegisterStandardMethods();
	Force::RegisterStandardForces();
	
	#if C4DIAGNOSTICS
	
		World::lightRegionRenderList.Append(&World::lightRegionRenderable);
		World::lightRegionRenderable.SetRenderableFlags(kRenderableFogInhibit);
		World::lightRegionRenderable.SetShaderFlags(kShaderAmbientEffect);
		World::lightRegionAttributeList.Append(&World::lightRegionDiffuseColor);
		World::lightRegionRenderable.SetMaterialAttributeList(&World::lightRegionAttributeList);
		
		World::sourcePathRenderList.Append(&World::sourcePathRenderable);
		World::sourcePathRenderable.SetRenderableFlags(kRenderableFogInhibit);
		World::sourcePathRenderable.SetShaderFlags(kShaderAmbientEffect);
		World::sourcePathAttributeList.Append(&World::sourcePathDiffuseColor);
		World::sourcePathRenderable.SetMaterialAttributeList(&World::sourcePathAttributeList);
		World::sourcePathRenderable.SetVertexCount(2);
		World::sourcePathRenderable.SetAttributeArray(kArrayVertex, World::sourcePathVertex);
	
	#endif
	
	return (kEngineOkay);
}

void WorldMgr::Destruct(void)
{
	displayEventHandler.Detach();
	controllerStateSender.Detach();
	Object::RemoveConstructor(&objectConstructor);
	
	TheResourceMgr->FlushCache(AnimationResource::GetDescriptor());
}

Object *WorldMgr::ConstructObject(Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (data.GetType())
	{
		case kObjectProperty:
			
			return (new PropertyObject);
		
		case kObjectMaterial:
		
		#if C4LEGACY
		
			case 'EMAT':
		
		#endif
			
			return (new MaterialObject);
		
		case kObjectScript:
			
			return (new ScriptObject);
		
		case kObjectTerrainBlock:
		
		#if C4LEGACY
		
			case 'BLCK':
		
		#endif
			
			return (new TerrainBlockObject);
		
		case kObjectWaterBlock:
			
			return (new WaterBlockObject);
		
		case kObjectGeometry:
			
			return (GeometryObject::Construct(++data, unpackFlags));
		
		case kObjectSource:
			
			return (SourceObject::Construct(++data, unpackFlags));
		
		case kObjectPortal:
			
			return (PortalObject::Construct(++data, unpackFlags));
		
		case kObjectZone:
			
			return (ZoneObject::Construct(++data, unpackFlags));
		
		case kObjectTrigger:
			
			return (TriggerObject::Construct(++data, unpackFlags));
		
		case kObjectEffect:
			
			return (EffectObject::Construct(++data, unpackFlags));
		
		case kObjectEmitter:
			
			return (EmitterObject::Construct(++data, unpackFlags));
		
		case kObjectShape:
			
			return (ShapeObject::Construct(++data, unpackFlags));
		
		case kObjectJoint:
			
			return (JointObject::Construct(++data, unpackFlags));
		
		case kObjectField:
			
			return (FieldObject::Construct(++data, unpackFlags));
	}
	
	return (nullptr);
}

void WorldMgr::SendControllerState(Player *to, void *cookie)
{
	World *world = static_cast<WorldMgr *>(cookie)->GetWorld();
	if (world)
	{
		machine count = world->GetControllerArraySize();
		for (machine index = 0; index < count; index++)
		{
			const Controller *controller = world->GetController(index);
			if (controller) controller->SendInitialStateMessages(to);
		}
	}
}

void WorldMgr::HandleDisplayEvent(const DisplayEventData *eventData, void *cookie)
{
	if (eventData->eventType == kEventDisplayChange)
	{
		WorldMgr *worldMgr = static_cast<WorldMgr *>(cookie);
		
		World *world = worldMgr->currentWorld;
		if (world)
		{
			int32 width = TheDisplayMgr->GetDisplayWidth();
			int32 height = TheDisplayMgr->GetDisplayHeight();
			world->SetRenderSize(width, height);
		}
	}
}

void WorldMgr::HandleLightDetailLevelEvent(Variable *variable)
{
	lightDetailLevel = MaxZero(Min(variable->GetIntegerValue(), 2));
}

WorldResult WorldMgr::InitWorld(World *world)
{
	currentWorld = nullptr;
	
	if (world->Preprocess() != kWorldOkay)
	{
		delete world;
		return (kWorldLoadFailed);
	}
	
	TheMessageMgr->SetControllerMessageProcs(&World::ConstructControllerMessage, &World::ReceiveControllerMessage, world);
	TheTimeMgr->ResetTime();
	
	currentWorld = world;
	return (kWorldOkay);
}

WorldResult WorldMgr::LoadWorld(const char *name)
{
	if (currentWorld) UnloadWorld();
	
	World *world = (worldConstructorProc) ? (*worldConstructorProc)(name, worldConstructorCookie) : new World(name);
	return (InitWorld(world));
}

void WorldMgr::UnloadWorld(void)
{
	TheMessageMgr->SetControllerMessageProcs(nullptr, nullptr);
	
	delete currentWorld;
	currentWorld = nullptr;
	
	VertexProgram::Flush();
	FragmentProgram::Flush();
	MaterialObject::ReleaseMaterialCache();
}

void WorldMgr::SaveDeltaWorld(const char *name)
{
	const World *world = currentWorld;
	if (world)
	{
		File			file;
		ResourcePath	path;
		
		TheResourceMgr->GetSaveCatalog()->GetResourcePath(SaveResource::GetDescriptor(), name, &path);
		if ((FileMgr::CreateDirectoryPath(path) == kFileOkay) && (file.Open(path, kFileCreate) == kFileOkay))
		{
			world->GetRootNode()->PackDeltaTree(&file, world->GetWorldName());
		}
	}
}

WorldResult WorldMgr::RestoreDeltaWorld(const char *name)
{
	World *world = (worldConstructorProc) ? (*worldConstructorProc)(name, worldConstructorCookie) : new World(name);
	world->SetWorldFlags(world->GetWorldFlags() | kWorldRestore);
	world->previousWorld = currentWorld;
	return (InitWorld(world));
}

void WorldMgr::Move(void)
{
	World *world = currentWorld;
	if (world)
	{
		if (!(world->GetWorldFlags() & kWorldPaused))
		{
			world->Move();
			world->Update();
			world->Interact();
		}
		else
		{
			for (machine a = 0; a < kWorldCounterRenderCount; a++)
				world->worldCounter[a] = 0;
			world->Update();
		}
		
		world->GetRootNode()->Update();
		world->Listen();
	}
}

void WorldMgr::Render(void)
{
	World *world = currentWorld;
	if (world)
	{
		world->BeginRendering();
		world->Render();
		world->EndRendering();
	}
}

// ZYURVUR
