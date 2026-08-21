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
#include "C4Particles.h"
#include "C4Panels.h"
#include "C4Manipulator.h"


using namespace C4;


namespace
{
	const float kMarkingEpsilon = 0.25F;
	
	
	const TextureHeader beamTextureHeader =
	{
		kTexture2D,
		kTextureForceHighQuality,
		kTextureSemanticEmission,
		kTextureSemanticNone,
		kTextureL8,
		16, 1, 1,
		{kTextureClamp, kTextureClamp, kTextureClamp},
		1
	};
	
	
	const unsigned_int8 beamTextureImage[16] =
	{
		0x00, 0x03, 0x09, 0x0F, 0x19, 0x27, 0x3F, 0x6F, 0xFF, 0x6F, 0x3F, 0x27, 0x19, 0x0F, 0x09, 0x03
	};
}


namespace C4
{
	template class Registrable<Effect, EffectRegistration>;
	
	template <> Heap Memory<MarkingEffect>::heap("MarkingEffect", kHeapMutexless);
	template class Memory<MarkingEffect>;
}


EffectRegistration::EffectRegistration(EffectType type, const char *name) : Registration<Effect, EffectRegistration>(type)
{
	effectName = name;
}

EffectRegistration::~EffectRegistration()
{
}


EffectObject::EffectObject(EffectType type) : Object(kObjectEffect)
{
	effectType = type;
}

EffectObject::~EffectObject()
{
}

EffectObject *EffectObject::Construct(Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (data.GetType())
	{
		case kEffectQuad:
			
			return (new QuadEffectObject);
		
		case kEffectFlare:
			
			return (new FlareEffectObject);
		
		case kEffectBeam:
			
			return (new BeamEffectObject);
		
		case kEffectTube:
			
			return (new TubeEffectObject);
		
		case kEffectBolt:
			
			return (new BoltEffectObject);
		
		case kEffectFire:
			
			return (new FireEffectObject);
		
		case kEffectPanel:
			
			return (new PanelEffectObject);
	}
	
	return (nullptr);
}

void EffectObject::PackType(Packer& data) const
{
	Object::PackType(data);
	data << effectType; 
}
 
 
Effect::Effect(EffectType type, RenderType renderType, unsigned_int32 renderState) : RenderableNode(kNodeEffect, renderType, renderState) 
{
	effectType = type; 
	effectFlags = 0;
	effectListIndex = kEffectListTransparent;
	SetActiveUpdateFlags(GetActiveUpdateFlags() | kUpdatePostBounding);
} 

Effect::Effect(const Effect& effect) : RenderableNode(effect)
{
	effectType = effect.effectType; 
	effectFlags = effect.effectFlags;
	effectListIndex = effect.effectListIndex;
	SetActiveUpdateFlags(GetActiveUpdateFlags() | kUpdatePostBounding);
}

Effect::~Effect()
{
}

Effect *Effect::Construct(Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (data.GetType())
	{
		case kEffectParticleSystem:
			
			return (Registrable<ParticleSystem, ParticleSystemRegistration>::Construct(++data, unpackFlags));
		
		case kEffectMarking:
			
			return (new MarkingEffect);
		
		case kEffectQuad:
			
			return (new QuadEffect);
		
		case kEffectFlare:
			
			return (new FlareEffect);
		
		case kEffectBeam:
			
			return (new BeamEffect);
		
		case kEffectTube:
			
			return (new TubeEffect);
		
		case kEffectBolt:
			
			return (new BoltEffect);
		
		case kEffectFire:
			
			return (new FireEffect);
		
		case kEffectPanel:
			
			return (new PanelEffect);
	}
	
	return (Registrable<Effect, EffectRegistration>::Construct(data, unpackFlags));
}

Effect *Effect::New(EffectType type)
{
	Type	data[2];
	
	data[0] = type;
	data[1] = 0;
	
	Unpacker unpacker(data);
	return (Construct(unpacker));
}

void Effect::PackType(Packer& data) const
{
	RenderableNode::PackType(data);
	data << effectType;
}

void Effect::Pack(Packer& data, unsigned_int32 packFlags) const
{
	RenderableNode::Pack(data, packFlags);
	
	data << ChunkHeader('FLAG', 4);
	data << effectFlags;
	
	data << TerminatorChunk;
}

void Effect::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	RenderableNode::Unpack(data, unpackFlags);
	UnpackChunkList<Effect>(data, unpackFlags);
}

bool Effect::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'FLAG':
			
			data >> effectFlags;
			return (true);
	}
	
	return (false);
}

void Effect::Preprocess(void)
{
	RenderableNode::Preprocess();
	
	World *world = GetWorld();
	if (world) world->AddEffect(this);
}

void Effect::Neutralize(void)
{
	ListElement<Effect>::Detach();
	RenderableNode::Neutralize();
}

void Effect::EnterZone(Zone *zone)
{
	if (effectListIndex == kEffectListLight)
	{
		const AmbientEnvironment *environment = zone->GetAmbientEnvironment();
		if (*environment->environmentMap) InvalidateAmbientShaderData();
		SetAmbientEnvironment(environment);
	}
}

void Effect::AddEffectBond(Zone *zone)
{
	Site *site = zone->GetEffectSite();
	
	Bond *bond = GetFirstIncomingEdge();
	while (bond)
	{
		if (bond->GetStartElement() == site) return;
		bond = bond->GetNextIncomingEdge();
	}
	
	new Bond(site, this);
}

void Effect::CalculatePostBounding(void)
{
	PurgeIncomingEdges();
	
	Zone *zone = GetOwningZone();
	if (zone)
	{
		new Bond(zone->GetEffectSite(), this);
		
		if (!(effectFlags & kEffectStatic))
		{
			const BoundingSphere *sphere = GetBoundingSphere();
			if (sphere)
			{
				BondAffectedZones(zone, sphere->GetCenter(), sphere->GetRadius());
				
				Bond *bond = GetFirstIncomingEdge();
				while (bond)
				{
					Zone::GetEffectSiteZone(bond->GetStartElement())->SetExclusionMask(0);
					bond = bond->GetNextIncomingEdge();
				}
			}
		}
	}
}

void Effect::BondAffectedZones(Zone *zone, const Point3D& center, float radius)
{
	zone->SetExclusionMask(1);
	
	if ((zone->GetFirstSubzone()) || (!zone->GetObject()->InteriorSphere(zone->GetInverseWorldTransform() * center, radius)))
	{
		const Portal *portal = zone->GetFirstPortal();
		while (portal)
		{
			if ((portal->Enabled()) && (portal->GetPortalType() == kPortalDirect))
			{
				Zone *connectedZone = portal->GetConnectedZone();
				if ((connectedZone) && (connectedZone->GetExclusionMask() == 0))
				{
					if ((portal->GetWorldPlane() ^ center) < radius)
					{
						new Bond(connectedZone->GetEffectSite(), this);
						BondAffectedZones(connectedZone, center, radius);
					}
				}
			}
			
			portal = portal->Next();
		}
	}
	
	const Bond *bond = zone->GetZoneSite()->GetFirstOutgoingEdge();
	while (bond)
	{
		Zone *bondZone = static_cast<Zone *>(bond->GetFinishElement());
		if (bondZone->GetExclusionMask() == 0)
		{
			const BoundingSphere *sphere = bondZone->GetBoundingSphere();
			if (sphere)
			{
				float r = radius + sphere->GetRadius();
				if (SquaredMag(center - sphere->GetCenter()) < r * r)
				{
					new Bond(bondZone->GetEffectSite(), this);
					BondAffectedZones(bondZone, center, radius);
				}
			}
		}
		
		bond = bond->GetNextOutgoingEdge();
	}
}

void Effect::UpdateEffectGeometry(void)
{
}

void Effect::Move(void)
{
}

void Effect::Render(const Camera *camera, List<Renderable> *effectList)
{
	effectList[effectListIndex].Append(this);
}


MarkingEffect::MarkingEffect() : Effect(kEffectMarking, kRenderIndexedTriangles, kRenderDepthTest | kRenderDepthInhibit | kRenderDepthOffset)
{
	SetNodeFlags(kNodeCloneInhibit | kNodeAnimateInhibit);
	
	largeArrayStorage = nullptr;
	materialObject = nullptr;
}

MarkingEffect::MarkingEffect(const Geometry *geometry, const MarkingData *data) :
		Effect(kEffectMarking, kRenderIndexedTriangles, kRenderDepthTest | kRenderDepthInhibit | kRenderDepthOffset),
		diffuseColor(data->color, kAttributeMutable)
{
	Point3D			markingCenter;
	Vector3D		markingNormal;
	Vector3D		markingTangent;
	ClippingData	clippingData;
	
	SetNodeFlags(kNodeCloneInhibit | kNodeAnimateInhibit);
	
	largeArrayStorage = nullptr;
	materialObject = nullptr;
	
	markingFlags = data->markingFlags;
	markingAlpha = data->color.alpha;
	
	int32 life = data->lifeTime;
	if (life >= 0)
	{
		markingLifeTime = life;
		markingFadeTime = Max(markingLifeTime * 3 / 4, 1);
		markingKillTime = markingFadeTime / 8;
	}
	else
	{
		markingLifeTime = TheTimeMgr->GetDeltaTime() + 1;
		markingFadeTime = 0;
		markingKillTime = 0;
	}
	
	markingDepthOffset = Math::RandomFloat(0.015625F) + 0.0078125F;
	
	MarkingList *markingList = data->markingList;
	if (markingList) markingList->Append(this);
	
	float radius = data->radius;
	float zmin = -radius;
	float zmax = radius;
	
	if (markingFlags & kMarkingClipRange)
	{
		zmin = data->clip.min;
		zmax = data->clip.max;
	}
	
	effectPosition = geometry->GetInverseWorldTransform() * data->center;
	effectRadius = Fmax(Fabs(zmin), Fabs(zmax));
	effectRadius = Sqrt(effectRadius * effectRadius + radius * radius * 2.0F);
	
	const Transformable *transformable = geometry->GetTransformable();
	if (transformable)
	{
		const Transform4D& transform = transformable->GetWorldTransform();
		const Transform4D& inverse = transformable->GetInverseWorldTransform();
		
		markingCenter = inverse * data->center;
		markingNormal = data->normal * transform;
		markingTangent = inverse * data->tangent;
	}
	else
	{
		markingCenter = data->center;
		markingNormal = data->normal;
		markingTangent = data->tangent;
	}
	
	vertexArray = smallVertexArray;
	normalArray = smallNormalArray;
	colorArray = smallColorArray;
	tangentArray = smallTangentArray;
	texcoordArray = smallTexcoordArray;
	triangleArray = smallTriangleArray;
	
	Antivector3D up = (markingNormal % markingTangent).Normalize();
	Antivector3D right = (up % markingNormal).Normalize();
	
	float d = markingCenter * right;
	clippingData.leftPlane.Set(right, radius - d);
	clippingData.rightPlane.Set(-right, radius + d);
	
	d = markingCenter * up;
	clippingData.bottomPlane.Set(up, radius - d);
	clippingData.topPlane.Set(-up, radius + d);
	
	d = markingCenter * markingNormal;
	clippingData.backPlane.Set(markingNormal, -zmin - d);
	clippingData.frontPlane.Set(-markingNormal, zmax + d);
	
	markingVertexCount = 0;
	markingTriangleCount = 0;
	clippingData.maxMarkingVertexCount = kMaxSmallMarkingVertexCount;
	
	const GeometryObject *object = geometry->GetObject();
	const GeometryLevel *geometryLevel = object->GetGeometryLevel(0);
	int32 vertexCount = geometryLevel->GetVertexCount();
	clippingData.geometryVertexCount = vertexCount;
	
	const Point3D *vertex = geometryLevel->GetArray<Point3D>(kArrayVertex);
	const Vector3D *normal = geometryLevel->GetArray<Vector3D>(kArrayNormal);
	
	int32 triangleCount = geometryLevel->GetFaceCount();
	const Triangle *triangle = geometryLevel->GetArray<Triangle>(kArrayFace);
	
	for (machine a = 0; a < triangleCount; a++)
	{
		int32 i1 = triangle->index[0];
		int32 i2 = triangle->index[1];
		int32 i3 = triangle->index[2];
		
		const Point3D& v1 = vertex[i1];
		const Point3D& v2 = vertex[i2];
		const Point3D& v3 = vertex[i3];
		
		Vector3D cross = (v2 - v1) % (v3 - v1);
		if (markingNormal * cross * InverseMag(cross) > kMarkingEpsilon)
		{
			Point3D		tempVertex[2][9];
			Vector3D	tempNormal[2][9];
			
			tempVertex[0][0] = v1;
			tempVertex[0][1] = v2;
			tempVertex[0][2] = v3;
			
			tempNormal[0][0] = normal[i1];
			tempNormal[0][1] = normal[i2];
			tempNormal[0][2] = normal[i3];
			
			int32 count = ClipPolygonAgainstPlane(clippingData.leftPlane, 3, tempVertex[0], tempNormal[0], tempVertex[1], tempNormal[1]);
			if (count != 0)
			{
				count = ClipPolygonAgainstPlane(clippingData.rightPlane, count, tempVertex[1], tempNormal[1], tempVertex[0], tempNormal[0]);
				if (count != 0)
				{
					count = ClipPolygonAgainstPlane(clippingData.bottomPlane, count, tempVertex[0], tempNormal[0], tempVertex[1], tempNormal[1]);
					if (count != 0)
					{
						count = ClipPolygonAgainstPlane(clippingData.topPlane, count, tempVertex[1], tempNormal[1], tempVertex[0], tempNormal[0]);
						if (count != 0)
						{
							count = ClipPolygonAgainstPlane(clippingData.backPlane, count, tempVertex[0], tempNormal[0], tempVertex[1], tempNormal[1]);
							if (count != 0)
							{
								count = ClipPolygonAgainstPlane(clippingData.frontPlane, count, tempVertex[1], tempNormal[1], tempVertex[0], tempNormal[0]);
								if ((count != 0) && (!AddPolygon(count, tempVertex[0], tempNormal[0], &clippingData))) break;
							}
						}
					}
				}
			}
		}
		
		triangle++;
	}
	
	if (markingVertexCount > 0)
	{
		float f = 0.5F / radius;
		Vector3D sdir = clippingData.leftPlane.GetAntivector3D() * (data->texcoordScale.x * f);
		Vector3D tdir = clippingData.bottomPlane.GetAntivector3D() * (data->texcoordScale.y * f);
		float ds = data->texcoordScale.x * 0.5F + data->texcoordOffset.x;
		float dt = data->texcoordScale.y * 0.5F + data->texcoordOffset.y;
		
		for (machine a = 0; a < markingVertexCount; a++)
		{
			Vector3D v = vertexArray[a] - markingCenter;
			float s = v * sdir + ds;
			float t = v * tdir + dt;
			texcoordArray[a].Set(s, t);
		}
		
		if (markingFlags & kMarkingLight)
		{
			MaterialObject *material = data->materialObject;
			if (material) material->Retain();
			materialObject = material;
		}
		else
		{
			textureMap.SetTexture(data->textureName);
		}
	}
	else
	{
		markingLifeTime = 0;
	}
}

MarkingEffect::~MarkingEffect()
{
	if (materialObject) materialObject->Release();
	delete[] largeArrayStorage;
	
	List<MarkingEffect> *list = ListElement<MarkingEffect>::GetOwningList();
	if (list)
	{
		list->Remove(this);
		if (list->Empty()) static_cast<MarkingList *>(list)->HandleDestruction();
	}
}

void MarkingEffect::Prepack(List<Object> *linkList) const
{
	Effect::Prepack(linkList);
	if (materialObject) linkList->Append(materialObject);
}

void MarkingEffect::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Effect::Pack(data, packFlags);
	
	data << ChunkHeader('FLAG', 4);
	data << markingFlags;
	
	data << ChunkHeader('ALFA', 4);
	data << markingAlpha;
	
	data << ChunkHeader('DOFF', 4);
	data << markingDepthOffset;
	
	data << ChunkHeader('TIME', 12);
	data << markingLifeTime;
	data << markingFadeTime;
	data << markingKillTime;
	
	data << ChunkHeader('SPHR', sizeof(Point3D) + 4);
	data << effectPosition;
	data << effectRadius;
	
	PackHandle handle = data.BeginChunk('DIFF');
	diffuseColor.Pack(data, packFlags);
	data.EndChunk(handle);
	
	if (markingFlags & kMarkingLight)
	{
		if (materialObject)
		{
			data << ChunkHeader('MATL', 4);
			data << materialObject->GetObjectIndex();
		}
	}
	else
	{
		PackHandle handle = data.BeginChunk('TXTR');
		textureMap.Pack(data, packFlags);
		data.EndChunk(handle);
	}
		
	int32 vertexCount = markingVertexCount;
	int32 triangleCount = markingTriangleCount;
	
	data << ChunkHeader('GEOM', 8);
	data << vertexCount;
	data << triangleCount;
	
	if (triangleCount & 1)
	{
		triangleArray[triangleCount].Set(0, 0, 0);
		triangleCount++;
	}
	
	data << ChunkHeader('VERT', vertexCount * sizeof(Point3D));
	data.WriteArray(vertexCount, vertexArray);
	
	data << ChunkHeader('TEX0', vertexCount * sizeof(Point2D));
	data.WriteArray(vertexCount, texcoordArray);
	
	data << ChunkHeader('FACE', triangleCount * sizeof(Triangle));
	data.WriteArray(triangleCount, triangleArray);
	
	if (markingFlags & kMarkingLight)
	{
		data << ChunkHeader('FRAM', vertexCount * sizeof(Vector3D) * 2);
		data.WriteArray(vertexCount, normalArray);
		data.WriteArray(vertexCount, tangentArray);
	}
	else
	{
		data << ChunkHeader('COLR', vertexCount * sizeof(ColorRGBA));
		data.WriteArray(vertexCount, colorArray);
	}
	
	data << TerminatorChunk;
}

void MarkingEffect::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Effect::Unpack(data, unpackFlags);
	UnpackChunkList<MarkingEffect>(data, unpackFlags);
}

bool MarkingEffect::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'FLAG':
			
			data >> markingFlags;
			return (true);
		
		case 'ALFA':
			
			data >> markingAlpha;
			return (true);
		
		case 'DOFF':
			
			data >> markingDepthOffset;
			return (true);
		
		case 'TIME':
			
			data >> markingLifeTime;
			data >> markingFadeTime;
			data >> markingKillTime;
			return (true);
		
		case 'SPHR':
			
			data >> effectPosition;
			data >> effectRadius;
			return (true);
		
		case 'DIFF':
			
			diffuseColor.Unpack(data, unpackFlags);
			return (true);
		
		case 'MATL':
		{
			int32	objectIndex;
			
			data >> objectIndex;
			data.AddObjectLink(objectIndex, &MaterialObjectLinkProc, this);
			return (true);
		}
		
		case 'TXTR':
			
			textureMap.Unpack(data, unpackFlags);
			return (true);
		
		case 'GEOM':
			
			data >> markingVertexCount;
			data >> markingTriangleCount;
			
			if (markingVertexCount > kMaxSmallMarkingVertexCount)
			{
				AllocateLargeArrays(markingVertexCount, (markingTriangleCount + 1) & ~1);
			}
			else
			{
				vertexArray = smallVertexArray;
				normalArray = smallNormalArray;
				colorArray = smallColorArray;
				tangentArray = smallTangentArray;
				texcoordArray = smallTexcoordArray;
				triangleArray = smallTriangleArray;
			}

			return (true);
		
		case 'VERT':
			
			data.ReadArray(markingVertexCount, vertexArray);
			return (true);
		
		case 'TEX0':
			
			data.ReadArray(markingVertexCount, texcoordArray);
			return (true);
		
		case 'FACE':
			
			data.ReadArray((markingTriangleCount + 1) & ~1, triangleArray);
			return (true);
		
		case 'FRAM':
			
			data.ReadArray(markingVertexCount, normalArray);
			data.ReadArray(markingVertexCount, tangentArray);
			return (true);
		
		case 'COLR':
			
			data.ReadArray(markingVertexCount, colorArray);
			return (true);
	}
	
	return (false);
}

void *MarkingEffect::BeginSettingsUnpack(void)
{
	diffuseColor.BeginSettingsUnpack();
	textureMap.BeginSettingsUnpack();
	
	delete[] largeArrayStorage;
	largeArrayStorage = nullptr;
	
	return (Effect::BeginSettingsUnpack());
}

void MarkingEffect::MaterialObjectLinkProc(Object *object, void *cookie)
{
	static_cast<MarkingEffect *>(cookie)->materialObject = static_cast<MaterialObject *>(object);
	object->Retain();
}

bool MarkingEffect::CalculateBoundingBox(Box3D *box) const
{
	const Point3D& p = effectPosition;
	float r = effectRadius;
	
	box->min.Set(p.x - r, p.y - r, p.z - r);
	box->max.Set(p.x + r, p.y + r, p.z + r);
	
	return (true);
}

bool MarkingEffect::CalculateBoundingSphere(BoundingSphere *sphere) const
{
	sphere->SetCenter(effectPosition);
	sphere->SetRadius(effectRadius);
	return (true);
}

void MarkingEffect::Preprocess(void)
{
	SetEffectListIndex((markingFlags & kMarkingLight) ? kEffectListLight : kEffectListOpaque);
	Effect::Preprocess();
	
	const Node *node = GetSuperNode();
	if (node->GetNodeType() == kNodeGeometry) SetTransformable(static_cast<const Geometry *>(node)->GetTransformable());
	
	SetDepthOffset(markingDepthOffset, GetBoundingSphereCenterPointer());
	
	SetVertexCount(markingVertexCount);
	SetAttributeArray(kArrayVertex, vertexArray);
	SetAttributeArray(kArrayTexture0, texcoordArray);
	SetTriangleArray(markingTriangleCount, triangleArray);
	
	RenderSegment *segment = GetFirstRenderSegment();
	
	attributeList.Append(&diffuseColor);
	segment->SetMaterialAttributeList(&attributeList);
	
	if (markingFlags & kMarkingLight)
	{
		if (markingFlags & kMarkingDepthWrite) SetRenderState(GetRenderState() & ~kRenderDepthInhibit);
		
		if (markingFlags & kMarkingBlendLight)
		{
			SetAmbientBlendState(kBlendInterpolate | kBlendAlphaPreserve);
			SetLightBlendState(BlendState(kBlendSourceAlpha, kBlendOne));
		}
		
		segment->SetMaterialObjectPointer(&materialObject);
		
		SetAttributeArray(kArrayNormal, normalArray);
		SetAttributeArray(kArrayTangent, tangentArray);
	}
	else
	{
		SetRenderState(GetRenderState() | kRenderAlphaTest);
		SetAmbientBlendState(kBlendInterpolate | kBlendAlphaPreserve);
		if (markingFlags & kMarkingTwoSided) segment->SetMaterialState(kMaterialTwoSided);
		
		SetAttributeArray(kArrayColor0, colorArray);
		attributeList.Append(&textureMap);
	}
	
	markingInvisibleTime = -1;
}

void MarkingEffect::New(const World *world, const MarkingData *data)
{
	world->QueryProximity(data->center, data->radius, &MarkGeometry, const_cast<MarkingData *>(data));
}

ProximityResult MarkingEffect::MarkGeometry(Node *node, const Point3D& center, float radius, void *cookie)
{
	if (node->GetNodeType() == kNodeGeometry)
	{
		Geometry *geometry = static_cast<Geometry *>(node);
		
		const GeometryObject *object = geometry->GetObject();
		if (!(object->GetGeometryFlags() & (kGeometryInvisible | kGeometryMarkingInhibit)))
		{
			MarkingEffect *marking = new MarkingEffect(geometry, static_cast<MarkingData *>(cookie));
			if (marking->Nonempty()) geometry->AddNewSubnode(marking);
			else delete marking;
		}
	}
	
	return (kProximityContinue);
}

void MarkingEffect::AllocateLargeArrays(int32 vertexCount, int32 triangleCount)
{
	if (markingFlags & kMarkingLight)
	{
		char *array = new char[vertexCount * (sizeof(Point3D) + sizeof(Vector3D) * 2 + sizeof(Point2D)) + triangleCount * sizeof(Triangle)];
		largeArrayStorage = array;
		
		vertexArray = reinterpret_cast<Point3D *>(array);
		normalArray = reinterpret_cast<Vector3D *>(vertexArray + vertexCount);
		tangentArray = normalArray + vertexCount;
		texcoordArray = reinterpret_cast<Point2D *>(tangentArray + vertexCount);
		triangleArray = reinterpret_cast<Triangle *>(texcoordArray + vertexCount);
	}
	else
	{
		char *array = new char[vertexCount * (sizeof(Point3D) + sizeof(ColorRGBA) + sizeof(Point2D)) + triangleCount * sizeof(Triangle)];
		largeArrayStorage = array;
		
		vertexArray = reinterpret_cast<Point3D *>(array);
		colorArray = reinterpret_cast<ColorRGBA *>(vertexArray + vertexCount);
		texcoordArray = reinterpret_cast<Point2D *>(colorArray + vertexCount);
		triangleArray = reinterpret_cast<Triangle *>(texcoordArray + vertexCount);
	}
}

bool MarkingEffect::AddPolygon(int32 vertexCount, const Point3D *vertex, const Vector3D *normal, ClippingData *clippingData)
{
	int32 count = markingVertexCount;
	if (count + vertexCount >= clippingData->maxMarkingVertexCount)
	{
		if (clippingData->maxMarkingVertexCount != kMaxSmallMarkingVertexCount) return (false);
		
		int32 newMaxCount = clippingData->geometryVertexCount * 4;
		clippingData->maxMarkingVertexCount = newMaxCount;
		AllocateLargeArrays(newMaxCount, newMaxCount * 3);
		
		if (markingFlags & kMarkingLight)
		{
			MemoryMgr::CopyMemory(smallVertexArray, vertexArray, count * sizeof(Point3D));
			MemoryMgr::CopyMemory(smallNormalArray, normalArray, count * sizeof(Vector3D));
			MemoryMgr::CopyMemory(smallTangentArray, tangentArray, count * sizeof(Vector3D));
			MemoryMgr::CopyMemory(smallTexcoordArray, texcoordArray, count * sizeof(Point2D));
			MemoryMgr::CopyMemory(smallTriangleArray, triangleArray, markingTriangleCount * sizeof(Triangle));
		}
		else
		{
			MemoryMgr::CopyMemory(smallVertexArray, vertexArray, count * sizeof(Point3D));
			MemoryMgr::CopyMemory(smallColorArray, colorArray, count * sizeof(ColorRGBA));
			MemoryMgr::CopyMemory(smallTexcoordArray, texcoordArray, count * sizeof(Point2D));
			MemoryMgr::CopyMemory(smallTriangleArray, triangleArray, markingTriangleCount * sizeof(Triangle));
		}
	}
	
	Triangle *triangle = triangleArray + markingTriangleCount;
	markingTriangleCount += vertexCount - 2;
	for (machine a = 2; a < vertexCount; a++)
	{
		triangle->Set(count, count + a - 1, count + a);
		triangle++;
	}
	
	if (markingFlags & kMarkingLight)
	{
		const Vector3D& tang = clippingData->leftPlane.GetAntivector3D();
		for (machine a = 0; a < vertexCount; a++)
		{
			vertexArray[count] = vertex[a];
			Vector3D nrml = normal[a] * InverseMag(normal[a]);
			normalArray[count] = nrml;
			tangentArray[count] = (tang - nrml * (nrml * tang)).Normalize();
			count++;
		}
	}
	else
	{
		float f = 1.0F / (1.0F - kMarkingEpsilon);
		const Vector3D& markingNormal = clippingData->backPlane.GetAntivector3D();
		for (machine a = 0; a < vertexCount; a++)
		{
			vertexArray[count] = vertex[a];
			const Vector3D& nrml = normal[a];
			colorArray[count].Set(1.0F, 1.0F, 1.0F, FmaxZero((markingNormal * nrml * InverseMag(nrml) - kMarkingEpsilon) * f));
			count++;
		}
	}
	
	markingVertexCount = count;
	return (true);
}

int32 MarkingEffect::ClipPolygonAgainstPlane(const Antivector4D& plane, int32 vertexCount, const Point3D *vertex, const Vector3D *normal, Point3D *newVertex, Vector3D *newNormal)
{
	bool	negative[9];
	
	int32 negativeCount = 0;
	for (machine a = 0; a < vertexCount; a++)
	{
		bool neg = ((plane ^ vertex[a]) < 0.0F);
		negative[a] = neg;
		negativeCount += neg;
	}
	
	if (negativeCount == vertexCount) return (0);
	
	int32 count = 0;
	int32 previous = vertexCount - 1;
	for (machine index = 0; index < vertexCount; index++)
	{
		if (negative[index])
		{
			if (!negative[previous])
			{
				const Point3D& v1 = vertex[previous];
				const Point3D& v2 = vertex[index];
				float t = (plane ^ v1) / (plane ^ (v1 - v2));
				newVertex[count] = v1 * (1.0F - t) + v2 * t;
				
				const Vector3D& n1 = normal[previous];
				const Vector3D& n2 = normal[index];
				newNormal[count] = n1 * (1.0F - t) + n2 * t;
				count++;
			}
		}
		else
		{
			if (negative[previous])
			{
				const Point3D& v1 = vertex[index];
				const Point3D& v2 = vertex[previous];
				float t = (plane ^ v1) / (plane ^ (v1 - v2));
				newVertex[count] = v1 * (1.0F - t) + v2 * t;
				
				const Vector3D& n1 = normal[index];
				const Vector3D& n2 = normal[previous];
				newNormal[count] = n1 * (1.0F - t) + n2 * t;
				count++;
			}
			
			newVertex[count] = vertex[index];
			newNormal[count] = normal[index];
			count++;
		}
		
		previous = index;
	}
	
	return (count);
}

void MarkingEffect::Move(void)
{
	int32 life = markingLifeTime;
	int32 fade = markingFadeTime;
	int32 invisible = markingInvisibleTime;
	
	if ((life <= 0) || ((life < fade) && (invisible > markingKillTime)))
	{
		delete this;
	}
	else
	{
		int32 dt = TheTimeMgr->GetDeltaTime();
		markingLifeTime = life - dt;
		
		if (invisible >= 0) markingInvisibleTime = invisible + dt;
		else markingInvisibleTime = 0;
	}
}

void MarkingEffect::Render(const Camera *camera, List<Renderable> *effectList)
{
	int32 life = markingLifeTime;
	if (life != 0)
	{
		markingInvisibleTime = -1;
		
		int32 fade = markingFadeTime;
		if (life < fade)
		{
			float alpha = markingAlpha;
			float ratio = (float) life / (float) fade;
			
			if (!(markingFlags & kMarkingLight)) alpha *= ratio;
			else alpha *= ratio * 0.5F + 0.5F;
			
			diffuseColor.SetDiffuseAlpha(alpha);
		}
		
		effectList[GetEffectListIndex()].Append(this);
	}
}


void MarkingList::HandleDestruction(void)
{
}


QuadEffectObject::QuadEffectObject() : EffectObject(kEffectQuad)
{
	quadFlags = 0;
	quadBlendState = BlendState(kBlendSourceAlpha, kBlendOne);
	quadDeltaScale = 1.0F;
}

QuadEffectObject::QuadEffectObject(float radius, const ColorRGBA& color, const char *textureName) : EffectObject(kEffectQuad)
{
	quadRadius = radius;
	quadColor = color;
	quadTextureName = textureName;
	
	quadFlags = 0;
	quadBlendState = BlendState(kBlendSourceAlpha, kBlendOne);
	quadDeltaScale = 1.0F;
}

QuadEffectObject::~QuadEffectObject()
{
}

void QuadEffectObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	EffectObject::Pack(data, packFlags);
	
	data << ChunkHeader('FLAG', 4);
	data << quadFlags;
	
	PackHandle handle = data.BeginChunk('DATA');
	data << quadRadius;
	data << quadColor;
	data << quadTextureName;
	data.EndChunk(handle);
	
	data << ChunkHeader('BLND', 4);
	data << quadBlendState;
	
	data << ChunkHeader('DSCL', 4);
	data << quadDeltaScale;
	
	data << TerminatorChunk;
}

void QuadEffectObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	EffectObject::Unpack(data, unpackFlags);
	UnpackChunkList<QuadEffectObject>(data, unpackFlags);
}

bool QuadEffectObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'FLAG':
			
			data >> quadFlags;
			return (true);
		
		case 'DATA':
			
			data >> quadRadius;
			data >> quadColor;
			data >> quadTextureName;
			return (true);
		
		case 'BLND':
			
			data >> quadBlendState;
			return (true);
		
		case 'DSCL':
			
			data >> quadDeltaScale;
			return (true);
	}
	
	return (false);
}

int32 QuadEffectObject::GetCategoryCount(void) const
{
	return (1);
}

Type QuadEffectObject::GetCategoryType(int32 index, const char **title) const
{
	if (index == 0)
	{
		*title = TheInterfaceMgr->GetStringTable()->GetString(StringID(kEffectQuad));
		return (kEffectQuad);
	}
	
	return (0);
}

int32 QuadEffectObject::GetCategorySettingCount(Type category) const
{
	if (category == kEffectQuad) return (7);
	return (0);
}

Setting *QuadEffectObject::GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const
{
	if (category == kEffectQuad)
	{
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		
		if (index == 0)
		{
			const char *title = table->GetString(StringID(kEffectQuad, 'QUAD'));
			return (new HeadingSetting(kEffectQuad, title));
		}
		
		if (index == 1)
		{
			const char *title = table->GetString(StringID(kEffectQuad, 'QUAD', 'COLR'));
			const char *picker = table->GetString(StringID(kEffectQuad, 'QUAD', 'CPCK'));
			return (new ColorSetting('COLR', quadColor, title, picker, kColorPickerAlpha));
		}
		
		if (index == 2)
		{
			const char *title = table->GetString(StringID(kEffectQuad, 'QUAD', 'TNAM'));
			const char *picker = table->GetString(StringID(kEffectQuad, 'QUAD', 'TPCK'));
			return (new ResourceSetting('TNAM', quadTextureName, title, picker, TextureResource::GetDescriptor()));
		}
		
		if (index == 3)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			int32 selection = 0;
			if (quadBlendState == BlendState(kBlendSourceAlpha, kBlendOne)) selection = 1;
			else if (quadBlendState == kBlendInterpolate) selection = 2;
			else if (quadBlendState == BlendState(kBlendOne, kBlendInvSourceAlpha)) selection = 3;
			else if (quadBlendState == kBlendReplace) selection = 4;
			
			const char *title = table->GetString(StringID(kEffectQuad, 'QUAD', 'BLND'));
			MenuSetting *menu = new MenuSetting('BLND', selection, title, 5);
			
			menu->SetMenuItemString(0, table->GetString(StringID(kEffectQuad, 'QUAD', 'BLND', 'ADD ')));
			menu->SetMenuItemString(1, table->GetString(StringID(kEffectQuad, 'QUAD', 'BLND', 'ADDA')));
			menu->SetMenuItemString(2, table->GetString(StringID(kEffectQuad, 'QUAD', 'BLND', 'TERP')));
			menu->SetMenuItemString(3, table->GetString(StringID(kEffectQuad, 'QUAD', 'BLND', 'PREM')));
			menu->SetMenuItemString(4, table->GetString(StringID(kEffectQuad, 'QUAD', 'BLND', 'REPL')));
			
			return (menu);
		}
		
		if (index == 4)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const char *title = table->GetString(StringID(kEffectQuad, 'QUAD', 'INFT'));
			return (new BooleanSetting('INFT', ((quadFlags & kQuadInfinite) != 0), title));
		}
		
		if (index == 5)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const char *title = table->GetString(StringID(kEffectQuad, 'QUAD', 'SOFT'));
			return (new BooleanSetting('SOFT', ((quadFlags & kQuadSoftDepth) != 0), title));
		}
		
		if (index == 6)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const char *title = table->GetString(StringID(kEffectQuad, 'QUAD', 'DSCL'));
			return (new TextSetting('DSCL', quadDeltaScale, title));
		}
	}
	
	return (nullptr);
}

void QuadEffectObject::SetCategorySetting(Type category, const Setting *setting)
{
	if (category == kEffectQuad)
	{
		Type identifier = setting->GetSettingIdentifier();
		
		if (identifier == 'COLR')
		{
			quadColor = static_cast<const ColorSetting *>(setting)->GetColor();
		}
		else if (identifier == 'TNAM')
		{
			quadTextureName = static_cast<const ResourceSetting *>(setting)->GetResourceName();
		}
		else if (identifier == 'BLND')
		{
			static const unsigned_int32 stateTable[5] = 
			{
				kBlendAccumulate, BlendState(kBlendSourceAlpha, kBlendOne), kBlendInterpolate, BlendState(kBlendOne, kBlendInvSourceAlpha), kBlendReplace
			};
			
			quadBlendState = stateTable[static_cast<const MenuSetting *>(setting)->GetMenuSelection()];
		}
		else if (identifier == 'INFT')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) quadFlags |= kQuadInfinite;
			else quadFlags &= ~kQuadInfinite;
		}
		else if (identifier == 'SOFT')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) quadFlags |= kQuadSoftDepth;
			else quadFlags &= ~kQuadSoftDepth;
		}
		else if (identifier == 'DSCL')
		{
			quadDeltaScale = FmaxZero(Text::StringToFloat(static_cast<const TextSetting *>(setting)->GetText()));
		}
	}
}

int32 QuadEffectObject::GetObjectSize(float *size) const
{
	size[0] = quadRadius;
	return (1);
}

void QuadEffectObject::SetObjectSize(const float *size)
{
	quadRadius = size[0];
}


QuadEffect::QuadEffect() :
		Effect(kEffectQuad, kRenderTriangleStrip, kRenderDepthTest | kRenderDepthInhibit),
		diffuseAttribute(kAttributeMutable)
{
	quadOrientation = 0;
}

QuadEffect::QuadEffect(float radius, const ColorRGBA& color, const char *textureName) :
		Effect(kEffectQuad, kRenderTriangleStrip, kRenderDepthTest | kRenderDepthInhibit),
		diffuseAttribute(kAttributeMutable)
{
	SetNewObject(new QuadEffectObject(radius, color, textureName));
	quadOrientation = 0;
}

QuadEffect::QuadEffect(const QuadEffect& quadEffect) :
		Effect(quadEffect),
		diffuseAttribute(kAttributeMutable)
{
	quadOrientation = quadEffect.quadOrientation;
}

QuadEffect::~QuadEffect()
{
}

Node *QuadEffect::Replicate(void) const
{
	return (new QuadEffect(*this));
}

void QuadEffect::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Effect::Pack(data, packFlags);
	
	data << ChunkHeader('ORNT', 4);
	data << quadOrientation;
	
	data << TerminatorChunk;
}

void QuadEffect::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Effect::Unpack(data, unpackFlags);
	
	#if C4LEGACY
	
		if (data.GetVersion() >= 26) UnpackChunkList<QuadEffect>(data, unpackFlags);
	
	#else
	
		UnpackChunkList<QuadEffect>(data, unpackFlags);
	
	#endif
}

bool QuadEffect::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'ORNT':
			
			data >> quadOrientation;
			return (true);
	}
	
	return (false);
}

void QuadEffect::CalculatePostTransform(void)
{
	const Vector3D& direction = GetWorldTransform()[2];
	for (machine a = 0; a < 4; a++) quadVertex[a] = direction;
}

bool QuadEffect::CalculateBoundingBox(Box3D *box) const
{
	float r = GetObject()->GetQuadRadius();
	box->min.Set(-r, -r, -r);
	box->max.Set(r, r, r);
	return (true);
}

bool QuadEffect::CalculateBoundingSphere(BoundingSphere *sphere) const
{
	sphere->SetCenter(0.0F, 0.0F, 0.0F);
	sphere->SetRadius(GetObject()->GetQuadRadius());
	return (true);
}

bool QuadEffect::DirectionVisible(const Node *node, const Region *region)
{
	const QuadEffectObject *object = static_cast<const QuadEffect *>(node)->GetObject();
	return (region->DirectionVisible(node->GetWorldTransform()[2], object->GetQuadRadius() * 0.015625F));
}

bool QuadEffect::DirectionOccluded(const Node *node, const Region *region)
{
	const QuadEffectObject *object = static_cast<const QuadEffect *>(node)->GetObject();
	
	float radius = object->GetQuadRadius() * 0.015625F;
	const Vector3D& direction = node->GetWorldTransform()[2];
	
	do
	{
		if (region->DirectionOccluded(direction, radius)) return (true);
		region = region->Next();
	} while (region);
	
	return (false);
}

void QuadEffect::Preprocess(void)
{
	Effect::Preprocess();
	
	SetVertexCount(4);
	SetAttributeArray(kArrayVertex, quadVertex);
	SetAttributeArray(kArrayTexture0, quadTexcoord);
	
	if (!(GetObject()->GetQuadFlags() & kQuadInfinite))
	{
		SetTransformable(this);
		SetTransparentPosition(&GetWorldPosition());
		SetShaderFlags(kShaderAmbientEffect | kShaderVertexBillboard);
		SetActiveUpdateFlags(GetActiveUpdateFlags() & ~Node::kUpdatePostTransform);
		
		SetVisibilityProc(&SphereVisible);
		SetOcclusionProc(&SphereOccluded);
	}
	else
	{
		SetAttributeArray(kArrayBillboard, quadBillboard);
		SetShaderFlags(kShaderAmbientEffect | kShaderVertexInfinite | kShaderVertexBillboard);
		SetActiveUpdateFlags(GetActiveUpdateFlags() | Node::kUpdatePostTransform);
		SetVisibilityProc(&DirectionVisible);
		SetOcclusionProc(&DirectionOccluded);
	}
	
	quadTexcoord[0].Set(0.0F, 0.0F);
	quadTexcoord[1].Set(1.0F, 0.0F);
	quadTexcoord[2].Set(0.0F, 1.0F);
	quadTexcoord[3].Set(1.0F, 1.0F);
	
	attributeList.Append(&diffuseAttribute);
	attributeList.Append(&textureMapAttribute);
	
	RenderSegment *segment = GetFirstRenderSegment();
	segment->SetMaterialAttributeList(&attributeList);
	segment->SetMaterialState(kMaterialTwoSided);
	
	QuadEffect::ProcessObjectSettings();
	QuadEffect::UpdateEffectGeometry();
}

void QuadEffect::ProcessObjectSettings(void)
{
	const QuadEffectObject *object = GetObject();
	SetAmbientBlendState(object->GetQuadBlendState());
	
	textureMapAttribute.SetTexture(object->GetQuadTextureName());
	
	if (object->GetQuadFlags() & kQuadSoftDepth)
	{
		deltaDepthAttribute.SetDeltaScale(object->GetQuadSoftDepthScale());
		attributeList.Append(&deltaDepthAttribute);
	}
	else
	{
		deltaDepthAttribute.Detach();
	}
	
	InvalidateShaderData();
}

void QuadEffect::UpdateEffectGeometry(void)
{
	const QuadEffectObject *object = GetObject();
	
	float r = object->GetQuadRadius();
	int32 angle = quadOrientation;
	
	if (!(object->GetQuadFlags() & kQuadInfinite))
	{
		Vector2D cs = Math::GetTrigTable()[angle] * r;
		float u = cs.y + cs.x;
		float v = cs.y - cs.x;
		
		quadVertex[0].Set(v, u, 0.0F);
		quadVertex[1].Set(u, -v, 0.0F);
		quadVertex[2].Set(-u, v, 0.0F);
		quadVertex[3].Set(-v, -u, 0.0F);
	}
	else
	{
		r *= 0.015625F;
		Vector2D cs = Math::GetTrigTable()[angle] * r;
		float u = cs.y + cs.x;
		float v = cs.y - cs.x;
		
		quadBillboard[0].Set(v, u);
		quadBillboard[1].Set(u, -v);
		quadBillboard[2].Set(-u, v);
		quadBillboard[3].Set(-v, -u);
	}
}

void QuadEffect::Render(const Camera *camera, List<Renderable> *effectList)
{
	diffuseAttribute.SetDiffuseColor(GetObject()->GetQuadColor());
	effectList[GetEffectListIndex()].Append(this);
}


FlareEffectObject::FlareEffectObject() : EffectObject(kEffectFlare)
{
}

FlareEffectObject::FlareEffectObject(float flare, float occlusion, float rotation, const char *textureName) : EffectObject(kEffectFlare)
{
	flareRadius = flare;
	occlusionRadius = occlusion;
	rotationRadius = rotation;
	
	flareColor.Set(1.0F, 1.0F, 1.0F);
	flareTextureName = textureName;
}

FlareEffectObject::~FlareEffectObject()
{
}

void FlareEffectObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	EffectObject::Pack(data, packFlags);
	
	PackHandle handle = data.BeginChunk('DATA');
	data << flareRadius;
	data << occlusionRadius;
	data << rotationRadius;
	data << flareColor;
	data << flareTextureName;
	data.EndChunk(handle);
	
	data << TerminatorChunk;
}

void FlareEffectObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	EffectObject::Unpack(data, unpackFlags);
	UnpackChunkList<FlareEffectObject>(data, unpackFlags);
}

bool FlareEffectObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'DATA':
			
			data >> flareRadius;
			data >> occlusionRadius;
			data >> rotationRadius;
			data >> flareColor;
			data >> flareTextureName;
			return (true);
	}
	
	return (false);
}

int32 FlareEffectObject::GetCategoryCount(void) const
{
	return (1);
}

Type FlareEffectObject::GetCategoryType(int32 index, const char **title) const
{
	if (index == 0)
	{
		*title = TheInterfaceMgr->GetStringTable()->GetString(StringID(kEffectFlare));
		return (kEffectFlare);
	}
	
	return (0);
}

int32 FlareEffectObject::GetCategorySettingCount(Type category) const
{
	if (category == kEffectFlare) return (6);
	return (0);
}

Setting *FlareEffectObject::GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const
{
	if (category == kEffectFlare)
	{
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		
		if (index == 0)
		{
			const char *title = table->GetString(StringID(kEffectFlare, 'FLAR'));
			return (new HeadingSetting(kEffectFlare, title));
		}
		
		if (index == 1)
		{
			const char *title = table->GetString(StringID(kEffectFlare, 'FLAR', 'COLR'));
			const char *picker = table->GetString(StringID(kEffectFlare, 'FLAR', 'CPCK'));
			return (new ColorSetting('COLR', flareColor, title, picker));
		}
		
		if (index == 2)
		{
			const char *title = table->GetString(StringID(kEffectFlare, 'FLAR', 'TNAM'));
			const char *picker = table->GetString(StringID(kEffectFlare, 'FLAR', 'TPCK'));
			return (new ResourceSetting('TNAM', flareTextureName, title, picker, TextureResource::GetDescriptor()));
		}
		
		if (index == 3)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const char *title = table->GetString(StringID(kEffectFlare, 'FLAR', 'OCCL'));
			return (new TextSetting('OCCL', occlusionRadius, title));
		}
		
		if (index == 4)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const char *title = table->GetString(StringID(kEffectFlare, 'FLAR', 'ROTA'));
			return (new TextSetting('ROTA', rotationRadius, title));
		}
	}
	
	return (nullptr);
}

void FlareEffectObject::SetCategorySetting(Type category, const Setting *setting)
{
	if (category == kEffectFlare)
	{
		Type identifier = setting->GetSettingIdentifier();
		
		if (identifier == 'COLR')
		{
			flareColor = static_cast<const ColorSetting *>(setting)->GetColor();
		}
		else if (identifier == 'TNAM')
		{
			flareTextureName = static_cast<const ResourceSetting *>(setting)->GetResourceName();
		}
		else if (identifier == 'OCCL')
		{
			occlusionRadius = Text::StringToFloat(static_cast<const TextSetting *>(setting)->GetText());
		}
		else if (identifier == 'ROTA')
		{
			rotationRadius = Text::StringToFloat(static_cast<const TextSetting *>(setting)->GetText());
		}
	}
}

int32 FlareEffectObject::GetObjectSize(float *size) const
{
	size[0] = flareRadius;
	return (1);
}

void FlareEffectObject::SetObjectSize(const float *size)
{
	flareRadius = size[0];
}


FlareEffect::FlareEffect() :
		Effect(kEffectFlare, kRenderTriangleStrip),
		diffuseAttribute(kAttributeMutable),
		occlusionRenderable(kRenderTriangleStrip, kRenderDepthTest | kRenderColorInhibit | kRenderDepthInhibit),
		occlusionQuery(&RenderFlare, this)
{
}

FlareEffect::FlareEffect(float flare, float occlusion, float rotation, const char *textureName) :
		Effect(kEffectFlare, kRenderTriangleStrip),
		diffuseAttribute(kAttributeMutable),
		occlusionRenderable(kRenderTriangleStrip, kRenderDepthTest | kRenderColorInhibit | kRenderDepthInhibit),
		occlusionQuery(&RenderFlare, this)
{
	SetNewObject(new FlareEffectObject(flare, occlusion, rotation, textureName));
}

FlareEffect::FlareEffect(const FlareEffect& flareEffect) :
		Effect(flareEffect),
		diffuseAttribute(kAttributeMutable),
		occlusionRenderable(kRenderTriangleStrip, kRenderDepthTest | kRenderColorInhibit | kRenderDepthInhibit),
		occlusionQuery(&RenderFlare, this)
{
}

FlareEffect::~FlareEffect()
{
}

Node *FlareEffect::Replicate(void) const
{
	return (new FlareEffect(*this));
}

bool FlareEffect::CalculateBoundingBox(Box3D *box) const
{
	float r = GetObject()->GetFlareRadius();
	box->min.Set(-r, -r, -r);
	box->max.Set(r, r, r);
	return (true);
}

bool FlareEffect::CalculateBoundingSphere(BoundingSphere *sphere) const
{
	sphere->SetCenter(0.0F, 0.0F, 0.0F);
	sphere->SetRadius(GetObject()->GetFlareRadius());
	return (true);
}

void FlareEffect::Preprocess(void)
{
	Effect::Preprocess();
	
	SetShaderFlags(kShaderAmbientEffect);
	SetAmbientBlendState(BlendState(kBlendSourceAlpha, kBlendOne));
	
	flareTexcoord[0].Set(0.0F, 1.0F);
	flareTexcoord[1].Set(1.0F, 1.0F);
	flareTexcoord[2].Set(0.0F, 0.0F);
	flareTexcoord[3].Set(1.0F, 0.0F);
	
	SetVertexCount(4);
	SetAttributeArray(kArrayVertex, flareVertex);
	SetAttributeArray(kArrayTexture0, flareTexcoord);
	
	attributeList.Append(&diffuseAttribute);
	attributeList.Append(&textureMap);
	
	RenderSegment *segment = GetFirstRenderSegment();
	segment->SetMaterialAttributeList(&attributeList);
	segment->SetMaterialState(kMaterialTwoSided);
	
	occlusionRenderable.SetVertexCount(4);
	occlusionRenderable.SetAttributeArray(kArrayVertex, occlusionVertex);
	occlusionRenderable.SetShaderFlags(kShaderAmbientEffect);
	occlusionRenderable.SetOcclusionQuery(&occlusionQuery);
	occlusionRenderable.GetFirstRenderSegment()->SetMaterialState(kMaterialTwoSided);
	
	ProcessObjectSettings();
}

void FlareEffect::ProcessObjectSettings(void)
{
	textureMap.SetTexture(GetObject()->GetFlareTextureName());
	InvalidateShaderData();
}

void FlareEffect::Render(const Camera *camera, List<Renderable> *effectList)
{
	const FlareEffectObject *object = GetObject();
	
	const Vector3D& right = camera->GetWorldTransform()[0];
	const Vector3D& down = camera->GetWorldTransform()[1];
	
	float r = object->GetFlareRadius();
	Vector3D v1 = (right + down) * r;
	Vector3D v2 = (right - down) * r;
	
	const Point3D& p = GetWorldPosition();
	flareVertex[0] = p - v1;
	flareVertex[1] = p - v2;
	flareVertex[2] = p + v2;
	flareVertex[3] = p + v1;
	
	Vector3D offset = camera->GetWorldPosition() - p;
	Point3D q = p + offset * (object->GetRotationRadius() * InverseMag(offset));
	
	float d = (q - camera->GetWorldPosition()) * camera->GetWorldTransform()[2];
	float e = static_cast<FrustumCameraObject *>(camera->Node::GetObject())->GetFocalLength();
	float w = Fmax(object->GetOcclusionRadius() * e / d, 0.002F);
	inverseWidth = 1.0F / w;
	
	float s = w * d / (e * r);
	v1 *= s;
	v2 *= s;
	
	occlusionVertex[0] = q - v1;
	occlusionVertex[1] = q - v2;
	occlusionVertex[2] = q + v2;
	occlusionVertex[3] = q + v1;
	
	effectList[kEffectListOcclusion].Append(&occlusionRenderable);
}

void FlareEffect::RenderFlare(OcclusionQuery *query, List<Renderable> *renderList, void *cookie)
{
	FlareEffect *flareEffect = static_cast<FlareEffect *>(cookie);
	
	float w = flareEffect->inverseWidth;
	float intensity = Fmin(query->GetUnoccludedArea() * w * w, 1.0F);
	
	const ColorRGB& flareColor = flareEffect->GetObject()->GetFlareColor();
	ColorRGBA color(flareColor.red * intensity, flareColor.green * intensity, flareColor.blue * intensity, 1.0F);
	flareEffect->diffuseAttribute.SetDiffuseColor(color);
	
	renderList->Append(flareEffect);
}


BeamEffectObject::BeamEffectObject() : EffectObject(kEffectBeam)
{
	texcoordScale = 1.0F;
	beamTextureName[0] = 0;
}

BeamEffectObject::BeamEffectObject(float radius, float height, const ColorRGBA& color, const char *textureName) : EffectObject(kEffectBeam)
{
	beamRadius = radius;
	beamHeight = height;
	beamColor = color;
	
	texcoordScale = 1.0F;
	if (textureName) beamTextureName = textureName;
	else beamTextureName[0] = 0;
}

BeamEffectObject::~BeamEffectObject()
{
}

void BeamEffectObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	EffectObject::Pack(data, packFlags);
	
	data << ChunkHeader('SIZE', 8);
	data << beamRadius;
	data << beamHeight;
	
	data << ChunkHeader('COLR', sizeof(ColorRGBA));
	data << beamColor;
	
	if (beamTextureName[0] != 0)
	{
		data << ChunkHeader('TSCL', 4);
		data << texcoordScale;
		
		PackHandle handle = data.BeginChunk('TXTR');
		data << beamTextureName;
		data.EndChunk(handle);
	}
	
	data << TerminatorChunk;
}

void BeamEffectObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	EffectObject::Unpack(data, unpackFlags);
	UnpackChunkList<BeamEffectObject>(data, unpackFlags);
}

bool BeamEffectObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'SIZE':
			
			data >> beamRadius;
			data >> beamHeight;
			return (true);
		
		case 'COLR':
			
			data >> beamColor;
			return (true);
		
		case 'TSCL':
			
			data >> texcoordScale;
			return (true);
		
		case 'TXTR':
			
			data >> beamTextureName;
			return (true);
	}
	
	return (false);
}

void *BeamEffectObject::BeginSettingsUnpack(void)
{
	beamTextureName[0] = 0;
	return (EffectObject::BeginSettingsUnpack());
}

int32 BeamEffectObject::GetCategoryCount(void) const
{
	return (1);
}

Type BeamEffectObject::GetCategoryType(int32 index, const char **title) const
{
	if (index == 0)
	{
		*title = TheInterfaceMgr->GetStringTable()->GetString(StringID(kEffectBeam));
		return (kEffectBeam);
	}
	
	return (0);
}

int32 BeamEffectObject::GetCategorySettingCount(Type category) const
{
	if (category == kEffectBeam) return (4);
	return (0);
}

Setting *BeamEffectObject::GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const
{
	if (category == kEffectBeam)
	{
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		
		if (index == 0)
		{
			const char *title = table->GetString(StringID(kEffectBeam, 'BEAM'));
			return (new HeadingSetting(kEffectBeam, title));
		}
		
		if (index == 1)
		{
			const char *title = table->GetString(StringID(kEffectBeam, 'BEAM', 'COLR'));
			const char *picker = table->GetString(StringID(kEffectBeam, 'BEAM', 'CPCK'));
			return (new ColorSetting('COLR', beamColor, title, picker, kColorPickerAlpha));
		}
		
		if (index == 2)
		{
			const char *title = table->GetString(StringID(kEffectBeam, 'BEAM', 'TNAM'));
			const char *picker = table->GetString(StringID(kEffectBeam, 'BEAM', 'TPCK'));
			return (new ResourceSetting('TNAM', beamTextureName, title, picker, TextureResource::GetDescriptor()));
		}
		
		if (index == 3)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const char *title = table->GetString(StringID(kEffectBeam, 'BEAM', 'TSCL'));
			return (new TextSetting('TSCL', texcoordScale, title));
		}
	}
	
	return (nullptr);
}

void BeamEffectObject::SetCategorySetting(Type category, const Setting *setting)
{
	if (category == kEffectBeam)
	{
		Type identifier = setting->GetSettingIdentifier();
		
		if (identifier == 'COLR')
		{
			beamColor = static_cast<const ColorSetting *>(setting)->GetColor();
		}
		else if (identifier == 'TNAM')
		{
			beamTextureName = static_cast<const ResourceSetting *>(setting)->GetResourceName();
		}
		else if (identifier == 'TSCL')
		{
			texcoordScale = Text::StringToFloat(static_cast<const TextSetting *>(setting)->GetText());
		}
	}
}

int32 BeamEffectObject::GetObjectSize(float *size) const
{
	size[0] = beamRadius;
	size[1] = beamHeight;
	return (2);
}

void BeamEffectObject::SetObjectSize(const float *size)
{
	beamRadius = size[0];
	beamHeight = size[1];
}


BeamEffect::BeamEffect() : Effect(kEffectBeam, kRenderTriangleStrip, kRenderDepthTest | kRenderDepthInhibit)
{
}

BeamEffect::BeamEffect(float radius, float height, const ColorRGBA& color, const char *textureName) : Effect(kEffectBeam, kRenderTriangleStrip, kRenderDepthTest | kRenderDepthInhibit)
{
	SetNewObject(new BeamEffectObject(radius, height, color, textureName));
}

BeamEffect::BeamEffect(const BeamEffect& beamEffect) : Effect(beamEffect)
{
}

BeamEffect::~BeamEffect()
{
}

Node *BeamEffect::Replicate(void) const
{
	return (new BeamEffect(*this));
}

bool BeamEffect::CalculateBoundingBox(Box3D *box) const
{
	const BeamEffectObject *object = GetObject();
	float r = object->GetBeamRadius();
	float h = object->GetBeamHeight();
	
	box->min.Set(-r, -r, 0.0F);
	box->max.Set(r, r, h);
	return (true);
}

bool BeamEffect::CalculateBoundingSphere(BoundingSphere *sphere) const
{
	const BeamEffectObject *object = GetObject();
	float r = object->GetBeamRadius();
	float h = object->GetBeamHeight() * 0.5F;
	
	sphere->SetCenter(0.0F, 0.0F, h);
	sphere->SetRadius(Sqrt(h * h + r * r));
	return (true);
}

void BeamEffect::Preprocess(void)
{
	Effect::Preprocess();
	
	SetTransformable(this);
	SetTransparentPosition(&GetWorldPosition());
	SetShaderFlags(kShaderAmbientEffect | kShaderVertexPolyboard | kShaderLinearPolyboard);
	SetAmbientBlendState(kBlendAccumulate);
	
	SetVertexCount(4);
	SetAttributeArray(kArrayVertex, vertexArray);
	SetAttributeArray(kArrayTangent, tangentArray);
	SetAttributeArray(kArrayTexture0, texcoordArray);
	
	attributeList.Append(&diffuseColor);
	attributeList.Append(&textureMap);
	SetMaterialAttributeList(&attributeList);
	
	BeamEffect::ProcessObjectSettings();
	BeamEffect::UpdateEffectGeometry();
}

void BeamEffect::ProcessObjectSettings(void)
{
	const BeamEffectObject *object = GetObject();
	
	diffuseColor.SetDiffuseColor(object->GetBeamColor());
	
	const char *textureName = object->GetBeamTextureName();
	if (textureName[0] != 0) textureMap.SetTexture(textureName);
	else textureMap.SetTexture(&beamTextureHeader, beamTextureImage);
	
	InvalidateShaderData();
}

void BeamEffect::UpdateEffectGeometry(void)
{
	const BeamEffectObject *object = GetObject();
	
	float height = object->GetBeamHeight();
	vertexArray[0].Set(0.0F, 0.0F, 0.0F);
	vertexArray[1].Set(0.0F, 0.0F, 0.0F);
	vertexArray[2].Set(0.0F, 0.0F, height);
	vertexArray[3].Set(0.0F, 0.0F, height);
	
	float radius = object->GetBeamRadius();
	tangentArray[0].Set(0.0F, 0.0F, 1.0F, -radius);
	tangentArray[1].Set(0.0F, 0.0F, 1.0F, radius);
	tangentArray[2].Set(0.0F, 0.0F, 1.0F, -radius);
	tangentArray[3].Set(0.0F, 0.0F, 1.0F, radius);
	
	float scale = object->GetTexcoordScale();
	texcoordArray[0].Set(0.0F, 0.0F);
	texcoordArray[1].Set(1.0F, 0.0F);
	texcoordArray[2].Set(0.0F, scale);
	texcoordArray[3].Set(1.0F, scale);
}


TubeEffectObject::TubeEffectObject(EffectType type) : EffectObject(type)
{
	texcoordScale = 1.0F;
	maxSubdiv = 16;
	
	tubeTextureName[0] = 0;
	tubeStorage = nullptr;
}

TubeEffectObject::TubeEffectObject(const Path *path, float radius, const ColorRGBA& color, const char *textureName) :
		EffectObject(kEffectTube),
		tubePath(*path)
{
	tubeRadius = radius;
	tubePath.GetBoundingBox(&pathBoundingBox);
	
	tubeColor = color;
	
	if (textureName) tubeTextureName = textureName;
	else tubeTextureName[0] = 0;
	
	texcoordScale = 1.0F;
	maxSubdiv = 16;
	
	tubeStorage = nullptr;
}

TubeEffectObject::TubeEffectObject(EffectType type, const Path *path, float radius, const ColorRGBA& color, const char *textureName) :
		EffectObject(type),
		tubePath(*path)
{
	tubeRadius = radius;
	tubePath.GetBoundingBox(&pathBoundingBox);
	
	tubeColor = color;
	
	if (textureName) tubeTextureName = textureName;
	else tubeTextureName[0] = 0;
	
	texcoordScale = 1.0F;
	maxSubdiv = 16;
	
	tubeStorage = nullptr;
}

TubeEffectObject::~TubeEffectObject()
{
	delete[] tubeStorage;
}

void TubeEffectObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	EffectObject::Pack(data, packFlags);
	
	data << ChunkHeader('RADI', 4);
	data << tubeRadius;
	
	PackHandle handle = data.BeginChunk('PATH');
	tubePath.Pack(data, packFlags);
	data.EndChunk(handle);
	
	data << ChunkHeader('COLR', sizeof(ColorRGBA));
	data << tubeColor;
	
	data << ChunkHeader('SBDV', 4);
	data << maxSubdiv;
	
	if (tubeTextureName[0] != 0)
	{
		data << ChunkHeader('TSCL', 4);
		data << texcoordScale;
		
		PackHandle handle = data.BeginChunk('TXTR');
		data << tubeTextureName;
		data.EndChunk(handle);
	}
	
	if (tubeStorage)
	{
		int32 count = tubeVertexCount;
		
		data << ChunkHeader('VERT', 4 + count * (sizeof(Point3D) + sizeof(Vector4D) + sizeof(Point2D)));
		data << count;
		
		data.WriteArray(count, tubeVertexArray);
		data.WriteArray(count, tubeTangentArray);
		data.WriteArray(count, tubeTexcoordArray);
	}
	
	data << TerminatorChunk;
}

void TubeEffectObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	EffectObject::Unpack(data, unpackFlags);
	UnpackChunkList<TubeEffectObject>(data, unpackFlags);
}

bool TubeEffectObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'RADI':
			
			data >> tubeRadius;
			return (true);
		
		case 'PATH':
			
			tubePath.Unpack(data, unpackFlags);
			tubePath.GetBoundingBox(&pathBoundingBox);
			return (true);
		
		case 'COLR':
			
			data >> tubeColor;
			return (true);
		
		case 'SBDV':
			
			data >> maxSubdiv;
			return (true);
		
		case 'TSCL':
			
			data >> texcoordScale;
			return (true);
		
		case 'TXTR':
			
			data >> tubeTextureName;
			return (true);
		
		case 'VERT':
		{
			int32	count;
			
			data >> count;
			AllocateStorage(count);
			
			data.ReadArray(count, tubeVertexArray);
			data.ReadArray(count, tubeTangentArray);
			data.ReadArray(count, tubeTexcoordArray);
		}
	}
	
	return (false);
}

void *TubeEffectObject::BeginSettingsUnpack(void)
{
	tubePath.BeginSettingsUnpack();
	tubeTextureName[0] = 0;
	
	delete[] tubeStorage;
	tubeStorage = nullptr;
	
	return (EffectObject::BeginSettingsUnpack());
}

int32 TubeEffectObject::GetCategoryCount(void) const
{
	return (1);
}

Type TubeEffectObject::GetCategoryType(int32 index, const char **title) const
{
	if (index == 0)
	{
		*title = TheInterfaceMgr->GetStringTable()->GetString(StringID(kEffectTube));
		return (kEffectTube);
	}
	
	return (0);
}

int32 TubeEffectObject::GetCategorySettingCount(Type category) const
{
	if (category == kEffectTube) return (5);
	return (0);
}

Setting *TubeEffectObject::GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const
{
	if (category == kEffectTube)
	{
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		
		if (index == 0)
		{
			const char *title = table->GetString(StringID(kEffectTube, 'TUBE'));
			return (new HeadingSetting(kEffectTube, title));
		}
		
		if (index == 1)
		{
			const char *title = table->GetString(StringID(kEffectTube, 'TUBE', 'COLR'));
			const char *picker = table->GetString(StringID(kEffectTube, 'TUBE', 'CPCK'));
			return (new ColorSetting('COLR', tubeColor, title, picker, kColorPickerAlpha));
		}
		
		if (index == 2)
		{
			const char *title = table->GetString(StringID(kEffectTube, 'TUBE', 'TNAM'));
			const char *picker = table->GetString(StringID(kEffectTube, 'TUBE', 'TPCK'));
			return (new ResourceSetting('TNAM', tubeTextureName, title, picker, TextureResource::GetDescriptor()));
		}
		
		if (index == 3)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const char *title = table->GetString(StringID(kEffectTube, 'TUBE', 'TSCL'));
			return (new TextSetting('TSCL', texcoordScale, title));
		}
		
		if (index == 4)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const char *title = table->GetString(StringID(kEffectTube, 'TUBE', 'SBDV'));
			return (new TextSetting('SBDV', Text::IntegerToString(maxSubdiv), title, 2, &EditTextWidget::NumberFilter));
		}
	}
	
	return (nullptr);
}

void TubeEffectObject::SetCategorySetting(Type category, const Setting *setting)
{
	if (category == kEffectTube)
	{
		Type identifier = setting->GetSettingIdentifier();
		
		if (identifier == 'COLR')
		{
			tubeColor = static_cast<const ColorSetting *>(setting)->GetColor();
		}
		else if (identifier == 'TNAM')
		{
			tubeTextureName = static_cast<const ResourceSetting *>(setting)->GetResourceName();
		}
		else if (identifier == 'TSCL')
		{
			texcoordScale = Text::StringToFloat(static_cast<const TextSetting *>(setting)->GetText());
		}
		else if (identifier == 'SBDV')
		{
			maxSubdiv = Max(Text::StringToInteger(static_cast<const TextSetting *>(setting)->GetText()), 1);
		}
	}
}

int32 TubeEffectObject::GetObjectSize(float *size) const
{
	size[0] = tubeRadius;
	return (1);
}

void TubeEffectObject::SetObjectSize(const float *size)
{
	tubeRadius = size[0];
}

void TubeEffectObject::SetTubePath(const Path *path)
{
	tubePath = *path;
	tubePath.GetBoundingBox(&pathBoundingBox);
}

void TubeEffectObject::AllocateStorage(int32 vertexCount)
{
	if ((!tubeStorage) || (tubeVertexCount != vertexCount))
	{
		delete[] tubeStorage;
		
		tubeVertexCount = vertexCount;
		tubeStorage = new char[vertexCount * (sizeof(Point3D) + sizeof(Vector4D) + sizeof(Point2D))];
		tubeVertexArray = reinterpret_cast<Point3D *>(tubeStorage);
		tubeTangentArray = reinterpret_cast<Vector4D *>(tubeVertexArray + vertexCount);
		tubeTexcoordArray = reinterpret_cast<Point2D *>(tubeTangentArray + vertexCount);
	}
}

void TubeEffectObject::Build(void)
{
	int32 zdiv = Max(maxSubdiv, 1);
	float dz = 1.0F / (float) zdiv;
	
	int32 vertexCount = 2;
	float totalLength = 0.0F;
	
	const PathComponent *component = tubePath.GetFirstPathComponent();
	const Point3D& pathBeginPosition = component->GetBeginPosition();
	
	do
	{
		if (component->GetPathType() == kPathLinear)
		{
			vertexCount += 2;
			
			const LinearPathComponent *linearComponent = static_cast<const LinearPathComponent *>(component);
			totalLength += Magnitude(linearComponent->GetControlPoint(1) - linearComponent->GetControlPoint(0));
		}
		else
		{
			vertexCount += zdiv * 2;
			
			Point3D p1 = component->GetBeginPosition();
			for (machine j = 1; j <= zdiv; j++)
			{
				Point3D p2 = component->GetPosition((float) j * dz);
				totalLength += Magnitude(p2 - p1);
				p1 = p2;
			}
		}
		
		component = component->Next();
	} while (component);
	
	bool closed = (tubePath.GetLastPathComponent()->GetEndPosition() == pathBeginPosition);
	
	AllocateStorage(vertexCount);
	Point3D *vertex = tubeVertexArray;
	Vector4D *tangent = tubeTangentArray;
	Point2D *texcoord = tubeTexcoordArray;
	
	float radius = tubeRadius;
	float invLength = 1.0F / totalLength;
	float tex = 0.0F;
	
	component = tubePath.GetFirstPathComponent();
	for (;;)
	{
		if (component->GetPathType() == kPathLinear)
		{
			const Point3D& beginPosition = component->GetBeginPosition();
			Vector3D beginTangent = component->GetBeginTangent().Normalize();
			
			vertex[0] = beginPosition;
			vertex[1] = beginPosition;
			tangent[0].Set(beginTangent, -radius);
			tangent[1].Set(beginTangent, radius);
			texcoord[0].Set(0.0F, tex * texcoordScale);
			texcoord[1].Set(1.0F, tex * texcoordScale);
			
			const LinearPathComponent *linearComponent = static_cast<const LinearPathComponent *>(component);
			tex += Magnitude(linearComponent->GetControlPoint(1) - linearComponent->GetControlPoint(0)) * invLength;
			
			vertex += 2;
			tangent += 2;
			texcoord += 2;
		}
		else
		{
			Point3D p1 = component->GetBeginPosition();
			Vector3D tang = component->GetBeginTangent().Normalize();
			
			vertex[0] = p1;
			vertex[1] = p1;
			tangent[0].Set(tang, -radius);
			tangent[1].Set(tang, radius);
			texcoord[0].Set(0.0F, tex * texcoordScale);
			texcoord[1].Set(1.0F, tex * texcoordScale);
			
			vertex += 2;
			tangent += 2;
			texcoord += 2;
			
			for (machine j = 1; j < zdiv; j++)
			{
				float t = (float) j * dz;
				Point3D p2 = component->GetPosition(t);
				tang = component->GetTangent(t).Normalize();
				
				tex += Magnitude(p2 - p1) * invLength;
				p1 = p2;
				
				vertex[0] = p2;
				vertex[1] = p2;
				tangent[0].Set(tang, -radius);
				tangent[1].Set(tang, radius);
				texcoord[0].Set(0.0F, tex * texcoordScale);
				texcoord[1].Set(1.0F, tex * texcoordScale);
				
				vertex += 2;
				tangent += 2;
				texcoord += 2;
			}
			
			tex += Magnitude(component->GetEndPosition() - p1) * invLength;
		}
		
		const PathComponent *nextComponent = component->Next();
		if (!nextComponent)
		{
			const Point3D	*endPosition;
			Vector3D		endTangent;
			
			if (closed)
			{
				endPosition = &pathBeginPosition;
				endTangent = tubePath.GetFirstPathComponent()->GetBeginTangent();
			}
			else
			{
				endPosition = &component->GetEndPosition();
				endTangent = component->GetEndTangent();
			}
			
			endTangent.Normalize();
			
			vertex[0] = *endPosition;
			vertex[1] = *endPosition;
			tangent[0].Set(endTangent, -radius);
			tangent[1].Set(endTangent, radius);
			texcoord[0].Set(0.0F, texcoordScale);
			texcoord[1].Set(1.0F, texcoordScale);
			
			break;
		}
		
		component = nextComponent;
	}
}


TubeEffect::TubeEffect(EffectType type) : Effect(type, kRenderTriangleStrip, kRenderDepthTest | kRenderDepthInhibit)
{
}

TubeEffect::TubeEffect(const Path *path, float radius, const ColorRGBA& color, const char *textureName) : Effect(kEffectTube, kRenderTriangleStrip, kRenderDepthTest | kRenderDepthInhibit)
{
	SetNewObject(new TubeEffectObject(path, radius, color, textureName));
}

TubeEffect::TubeEffect(EffectType type, const Path *path, float radius, const ColorRGBA& color, const char *textureName) : Effect(type, kRenderTriangleStrip, kRenderDepthTest | kRenderDepthInhibit)
{
}

TubeEffect::TubeEffect(const TubeEffect& tubeEffect) : Effect(tubeEffect)
{
}

TubeEffect::~TubeEffect()
{
}

Node *TubeEffect::Replicate(void) const
{
	return (new TubeEffect(*this));
}

void TubeEffect::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Effect::Pack(data, packFlags);
	
	data << TerminatorChunk;
}

void TubeEffect::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Effect::Unpack(data, unpackFlags);
	UnpackChunkList<TubeEffect>(data, unpackFlags);
}

bool TubeEffect::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	#if C4LEGACY
	
		if (chunkHeader->chunkType == 'LINK')
		{
			int32	nodeIndex;
			
			data >> nodeIndex;
			data.AddNodeLink(nodeIndex, &PathLinkProc, this);
			return (true);
		}
	
	#endif
	
	return (false);
}

#if C4LEGACY

	void TubeEffect::PathLinkProc(Node *node, void *cookie)
	{
		TubeEffect *tubeEffect = static_cast<TubeEffect *>(cookie);
		tubeEffect->SetConnectedPathMarker(static_cast<PathMarker *>(node));
	}

#endif

bool TubeEffect::CalculateBoundingBox(Box3D *box) const
{
	*box = GetObject()->GetPathBoundingBox();
	return (true);
}

bool TubeEffect::CalculateBoundingSphere(BoundingSphere *sphere) const
{
	const TubeEffectObject *object = GetObject();
	const Box3D& bounds = object->GetPathBoundingBox();
	
	sphere->SetCenter((bounds.min + bounds.max) * 0.5F);
	sphere->SetRadius(Magnitude(bounds.max - bounds.min) * 0.5F + object->GetTubeRadius());
	return (true);
}

int32 TubeEffect::GetInternalConnectorCount(void) const
{
	return (1);
}

const char *TubeEffect::GetInternalConnectorKey(int32 index) const
{
	if (index == 0) return (kConnectorKeyPath);
	return (nullptr);
}

bool TubeEffect::ValidConnectedNode(const ConnectorKey& key, const Node *node) const
{
	if (key == kConnectorKeyPath)
	{
		if (node->GetNodeType() == kNodeMarker) return (static_cast<const Marker *>(node)->GetMarkerType() == kMarkerPath);
		return (false);
	}
	
	return (Effect::ValidConnectedNode(key, node));
}

PathMarker *TubeEffect::GetConnectedPathMarker(void) const
{
	Node *node = GetConnectedNode(kConnectorKeyPath);
	if (node) return (static_cast<PathMarker *>(node));
	return (nullptr);
}

void TubeEffect::SetConnectedPathMarker(PathMarker *marker)
{
	if (marker)
	{
		Hub *hub = GetHub();
		if (hub)
		{
			Connector *connector = hub->FindOutgoingConnector(kConnectorKeyPath);
			if (connector)
			{
				connector->SetConnectorTarget(marker);
				return;
			}
		}
		
		AddConnector(kConnectorKeyPath, marker);
	}
	else
	{
		RemoveConnector(kConnectorKeyPath);
	}
}

void TubeEffect::Preprocess(void)
{
	Effect::Preprocess();
	
	SetTransformable(this);
	SetTransparentPosition(&GetWorldPosition());
	SetShaderFlags(kShaderAmbientEffect | kShaderVertexPolyboard);
	SetAmbientBlendState(kBlendAccumulate);
	
	const TubeEffectObject *object = GetObject();
	
	SetVertexCount(object->GetVertexCount());
	SetAttributeArray(kArrayVertex, object->GetVertexArray());
	SetAttributeArray(kArrayTangent, object->GetTangentArray());
	SetAttributeArray(kArrayTexture0, object->GetTexcoordArray());
	
	attributeList.Append(&diffuseColor);
	attributeList.Append(&textureMap);
	SetMaterialAttributeList(&attributeList);
	
	TubeEffect::ProcessObjectSettings();
}

void TubeEffect::ProcessObjectSettings(void)
{
	const TubeEffectObject *object = GetObject();
	
	diffuseColor.SetDiffuseColor(object->GetTubeColor());
	
	const char *textureName = object->GetTubeTextureName();
	if (textureName[0] != 0) textureMap.SetTexture(textureName);
	else textureMap.SetTexture(&beamTextureHeader, beamTextureImage);
	
	InvalidateShaderData();
}


BoltEffectObject::BoltEffectObject() : TubeEffectObject(kEffectBolt)
{
}

BoltEffectObject::BoltEffectObject(const Path *path, float radius, float deviation, const ColorRGBA& color, const char *textureName) : TubeEffectObject(kEffectBolt, path, radius, color, textureName)
{
	maxPathDeviation = deviation;
	
	branchingDepth = 0;
	branchCount = 2;
	branchRadiusScale = 0.5F;
	branchLengthRange.min = 1.0F;
	branchLengthRange.max = 2.0F;
}

BoltEffectObject::~BoltEffectObject()
{
}

void BoltEffectObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	TubeEffectObject::Pack(data, packFlags);
	
	data << ChunkHeader('DEVI', 4);
	data << maxPathDeviation;
	
	data << ChunkHeader('BRCH', 12 + sizeof(Range<float>));
	data << branchingDepth;
	data << branchCount;
	data << branchRadiusScale;
	data << branchLengthRange;
	
	data << TerminatorChunk;
}

void BoltEffectObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	TubeEffectObject::Unpack(data, unpackFlags);
	UnpackChunkList<BoltEffectObject>(data, unpackFlags);
}

bool BoltEffectObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'DEVI':
			
			data >> maxPathDeviation;
			return (true);
		
		case 'BRCH':
			
			data >> branchingDepth;
			data >> branchCount;
			data >> branchRadiusScale;
			data >> branchLengthRange;
			return (true);
	}
	
	return (false);
}

int32 BoltEffectObject::GetCategorySettingCount(Type category) const
{
	int32 count = TubeEffectObject::GetCategorySettingCount(category);
	if (category == kEffectTube) count += 7;
	return (count);
}

Setting *BoltEffectObject::GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const
{
	if (category == kEffectTube)
	{
		int32 count = TubeEffectObject::GetCategorySettingCount(kEffectTube);
		if (index >= count)
		{
			const StringTable *table = TheInterfaceMgr->GetStringTable();
			
			if (index == count)
			{
				const char *title = table->GetString(StringID(kEffectBolt, 'BOLT'));
				return (new HeadingSetting(kEffectBolt, title));
			}
			
			if (index == count + 1)
			{
				const char *title = table->GetString(StringID(kEffectBolt, 'BOLT', 'DEVI'));
				return (new TextSetting('DEVI', maxPathDeviation, title));
			}
			
			if (index == count + 2)
			{
				const char *title = table->GetString(StringID(kEffectBolt, 'BOLT', 'DPTH'));
				return (new IntegerSetting('DPTH', branchingDepth, title, 0, 4, 1));
			}
			
			if (index == count + 3)
			{
				const char *title = table->GetString(StringID(kEffectBolt, 'BOLT', 'BRCH'));
				return (new IntegerSetting('BRCH', branchCount, title, 1, 4, 1));
			}
			
			if (index == count + 4)
			{
				const char *title = table->GetString(StringID(kEffectBolt, 'BOLT', 'SCAL'));
				return (new TextSetting('SCAL', branchRadiusScale, title));
			}
			
			if (index == count + 5)
			{
				const char *title = table->GetString(StringID(kEffectBolt, 'BOLT', 'MINL'));
				return (new TextSetting('MINL', branchLengthRange.min, title));
			}
			
			if (index == count + 6)
			{
				const char *title = table->GetString(StringID(kEffectBolt, 'BOLT', 'MAXL'));
				return (new TextSetting('MAXL', branchLengthRange.max, title));
			}
		}
	}
	
	return (TubeEffectObject::GetCategorySetting(category, index, flags));
}

void BoltEffectObject::SetCategorySetting(Type category, const Setting *setting)
{
	if (category == kEffectTube)
	{
		Type identifier = setting->GetSettingIdentifier();
		
		if (identifier == 'DEVI')
		{
			maxPathDeviation = Text::StringToFloat(static_cast<const TextSetting *>(setting)->GetText());
		}
		else if (identifier == 'DPTH')
		{
			branchingDepth = static_cast<const IntegerSetting *>(setting)->GetIntegerValue();
		}
		else if (identifier == 'BRCH')
		{
			branchCount = static_cast<const IntegerSetting *>(setting)->GetIntegerValue();
		}
		else if (identifier == 'SCAL')
		{
			branchRadiusScale = Text::StringToFloat(static_cast<const TextSetting *>(setting)->GetText());
		}
		else if (identifier == 'MINL')
		{
			branchLengthRange.min = Text::StringToFloat(static_cast<const TextSetting *>(setting)->GetText());
		}
		else if (identifier == 'MAXL')
		{
			branchLengthRange.max = Text::StringToFloat(static_cast<const TextSetting *>(setting)->GetText());
		}
		else
		{
			TubeEffectObject::SetCategorySetting(kEffectTube, setting);
		}
	}
}

void BoltEffectObject::BuildBranch(const Path *path, float radius, float deviation, int32 zdiv, Point3D *vertex, Vector4D *tangent, Point2D *texcoord)
{
	float dz = 1.0F / (float) zdiv;
	const ConstVector2D *trig = Math::GetTrigTable();
	
	const PathComponent *component = GetTubePath()->GetFirstPathComponent();
	for (;;)
	{
		Point3D p1 = component->GetBeginPosition();
		Vector3D tang = component->GetBeginTangent().Normalize();
		
		vertex[0] = p1;
		vertex[1] = p1;
		tangent[0].Set(tang, -radius);
		tangent[1].Set(tang, radius);
		texcoord[0].Set(0.0F, 0.0F);
		texcoord[1].Set(1.0F, 0.0F);
		
		vertex += 2;
		tangent += 2;
		texcoord += 2;
		
		for (machine j = 1; j < zdiv; j++)
		{
			float t = (float) j * dz;
			Point3D p2 = component->GetPosition(t);
			tang = component->GetTangent(t).Normalize();
			Vector3D v1 = Math::CreateUnitPerpendicular(tang);
			Vector3D v2 = tang % v1;
			
			float m = deviation * t * (1.0F - t) * Math::RandomFloat(1.0F);
			const Vector2D& cs = trig[Math::Random(256)];
			p2 += v1 * (cs.x * m) + v2 * (cs.y * m);
			p1 = p2;
			
			vertex[0] = p2;
			vertex[1] = p2;
			tangent[0].Set(tang, -radius);
			tangent[1].Set(tang, radius);
			texcoord[0].Set(0.0F, 0.0F);
			texcoord[1].Set(1.0F, 0.0F);
			
			vertex += 2;
			tangent += 2;
			texcoord += 2;
		}
		
		const PathComponent *nextComponent = component->Next();
		if (!nextComponent)
		{
			const Point3D& endPosition = component->GetEndPosition();
			Vector3D endTangent = component->GetEndTangent().Normalize();
			
			vertex[0] = endPosition;
			vertex[1] = endPosition;
			tangent[0].Set(endTangent, -radius);
			tangent[1].Set(endTangent, radius);
			texcoord[0].Set(0.0F, 0.0F);
			texcoord[1].Set(1.0F, 0.0F);
			
			break;
		}
		
		component = nextComponent;
	}
}

void BoltEffectObject::Build(void)
{
	int32 zdiv = Max(GetMaxSubdiv(), 1);
	
	int32 vertexCount = 2 + GetTubePath()->GetPathComponentCount() * zdiv * 2;
	
	AllocateStorage(vertexCount);
	Point3D *vertex = GetVertexArray();
	Vector4D *tangent = GetTangentArray();
	Point2D *texcoord = GetTexcoordArray();
	
	BuildBranch(GetTubePath(), GetTubeRadius(), maxPathDeviation * 4.0F, zdiv, vertex, tangent, texcoord);
}


BoltEffect::BoltEffect() : TubeEffect(kEffectBolt)
{
}

BoltEffect::BoltEffect(const Path *path, float radius, float deviation, const ColorRGBA& color, const char *textureName) : TubeEffect(kEffectBolt, path, radius, color, textureName)
{
	SetNewObject(new BoltEffectObject(path, radius, deviation, color, textureName));
}

BoltEffect::BoltEffect(const BoltEffect& boltEffect) : TubeEffect(boltEffect)
{
}

BoltEffect::~BoltEffect()
{
}

Node *BoltEffect::Replicate(void) const
{
	return (new BoltEffect(*this));
}


FireEffectObject::FireEffectObject() : EffectObject(kEffectFire)
{
}

FireEffectObject::FireEffectObject(float radius, float height, float intensity, int32 speed, const char *textureName) : EffectObject(kEffectFire)
{
	fireRadius = radius;
	fireHeight = height;
	
	fireIntensity = intensity;
	fireSpeed = speed;
	
	fireTextureName = textureName;
}

FireEffectObject::~FireEffectObject()
{
}

void FireEffectObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	EffectObject::Pack(data, packFlags);
	
	data << ChunkHeader('DATA', 8);
	data << fireRadius;
	data << fireHeight;
	
	data << ChunkHeader('PARM', 8);
	data << fireIntensity;
	data << fireSpeed;
	
	PackHandle handle = data.BeginChunk('TXTR');
	data << fireTextureName;
	data.EndChunk(handle);
	
	data << TerminatorChunk;
}

void FireEffectObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	EffectObject::Unpack(data, unpackFlags);
	UnpackChunkList<FireEffectObject>(data, unpackFlags);
}

bool FireEffectObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'DATA':
			
			data >> fireRadius;
			data >> fireHeight;
			return (true);
		
		case 'PARM':
			
			data >> fireIntensity;
			data >> fireSpeed;
			return (true);
		
		case 'TXTR':
			
			data >> fireTextureName;
			return (true);
	}
	
	return (false);
}

int32 FireEffectObject::GetCategoryCount(void) const
{
	return (1);
}

Type FireEffectObject::GetCategoryType(int32 index, const char **title) const
{
	if (index == 0)
	{
		*title = TheInterfaceMgr->GetStringTable()->GetString(StringID(kEffectFire));
		return (kEffectFire);
	}
	
	return (0);
}

int32 FireEffectObject::GetCategorySettingCount(Type category) const
{
	if (category == kEffectFire) return (4);
	return (0);
}

Setting *FireEffectObject::GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const
{
	if (category == kEffectFire)
	{
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		
		if (index == 0)
		{
			const char *title = table->GetString(StringID(kEffectFire, 'FIRE'));
			return (new HeadingSetting(kEffectFire, title));
		}
		
		if (index == 1)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const char *title = table->GetString(StringID(kEffectFire, 'FIRE', 'TNAM'));
			const char *picker = table->GetString(StringID(kEffectFire, 'FIRE', 'PICK'));
			return (new ResourceSetting('TNAM', fireTextureName, title, picker, TextureResource::GetDescriptor()));
		}
		
		if (index == 2)
		{
			const char *title = table->GetString(StringID(kEffectFire, 'FIRE', 'INTS'));
			return (new IntegerSetting('INTS', Max((int32) (fireIntensity * 100.0F + 0.5F), 1), title, 1, 60, 1));
		}
		
		if (index == 3)
		{
			const char *title = table->GetString(StringID(kEffectFire, 'FIRE', 'SPED'));
			return (new IntegerSetting('SPED', fireSpeed + 1, title, 1, 25, 1));
		}
	}
	
	return (nullptr);
}

void FireEffectObject::SetCategorySetting(Type category, const Setting *setting)
{
	if (category == kEffectFire)
	{
		Type identifier = setting->GetSettingIdentifier();
		
		if (identifier == 'TNAM')
		{
			fireTextureName = static_cast<const ResourceSetting *>(setting)->GetResourceName();
		}
		else if (identifier == 'INTS')
		{
			fireIntensity = (float) static_cast<const IntegerSetting *>(setting)->GetIntegerValue() * 0.01F;
		}
		else if (identifier == 'SPED')
		{
			fireSpeed = static_cast<const IntegerSetting *>(setting)->GetIntegerValue() - 1;
		}
	}
}

int32 FireEffectObject::GetObjectSize(float *size) const
{
	size[0] = fireRadius;
	size[1] = fireHeight;
	return (2);
}

void FireEffectObject::SetObjectSize(const float *size)
{
	fireRadius = size[0];
	fireHeight = size[1];
}


FireEffect::FireEffect() : Effect(kEffectFire, kRenderTriangleStrip, kRenderDepthTest | kRenderDepthInhibit | kRenderAlphaTest)
{
}

FireEffect::FireEffect(float radius, float height, float intensity, int32 speed, const char *textureName) : Effect(kEffectFire, kRenderTriangleStrip, kRenderDepthTest | kRenderDepthInhibit | kRenderAlphaTest)
{
	SetNewObject(new FireEffectObject(radius, height, intensity, speed, textureName));
}

FireEffect::FireEffect(const FireEffect& fireEffect) : Effect(fireEffect)
{
}

FireEffect::~FireEffect()
{
}

Node *FireEffect::Replicate(void) const
{
	return (new FireEffect(*this));
}

bool FireEffect::CalculateBoundingBox(Box3D *box) const
{
	const FireEffectObject *object = GetObject();
	float r = object->GetFireRadius();
	float h = object->GetFireHeight();
	
	box->min.Set(-r, -r, 0.0F);
	box->max.Set(r, r, h);
	return (true);
}

bool FireEffect::CalculateBoundingSphere(BoundingSphere *sphere) const
{
	const FireEffectObject *object = GetObject();
	float r = object->GetFireRadius();
	float h = object->GetFireHeight() * 0.5F;
	
	sphere->SetCenter(0.0F, 0.0F, h);
	sphere->SetRadius(Sqrt(h * h + r * r));
	return (true);
}

void FireEffect::Preprocess(void)
{
	Effect::Preprocess();
	
	SetTransformable(this);
	SetTransparentPosition(&GetWorldPosition());
	SetShaderFlags(kShaderAmbientEffect);
	SetAmbientBlendState(kBlendAccumulate);
	
	float dx = Math::RandomFloat(1.0F);
	float dy = Math::RandomFloat(1.0F);
	
	texcoordArray[0].Set(0.0F, 1.0F, dx, dy);
	texcoordArray[1].Set(0.0F, 0.0F, dx, dy);
	texcoordArray[2].Set(1.0F, 1.0F, dx, dy);
	texcoordArray[3].Set(1.0F, 0.0F, dx, dy);
	
	SetVertexCount(4);
	SetAttributeArray(kArrayVertex, vertexArray);
	SetAttributeArray(kArrayTexture0, texcoordArray);
	
	textureMap.SetTexture(GetObject()->GetFireTextureName());
	
	attributeList.Append(&textureMap);
	attributeList.Append(&fireAttribute);
	SetMaterialAttributeList(&attributeList);
	
	FireEffect::ProcessObjectSettings();
}

void FireEffect::ProcessObjectSettings(void)
{
	const FireEffectObject *object = GetObject();
	
	fireAttribute.SetFireIntensity(object->GetFireIntensity());
	fireAttribute.SetFireSpeed(object->GetFireSpeed());
}

void FireEffect::Render(const Camera *camera, List<Renderable> *effectList)
{
	Point3D cameraPosition = GetInverseWorldTransform() * camera->GetWorldPosition();
	
	float x = -cameraPosition.y;
	float y = cameraPosition.x;
	float r = x * x + y * y;
	if (r > K::min_float)
	{
		const FireEffectObject *object = GetObject();
		
		r = InverseSqrt(r) * object->GetFireRadius();
		x *= r;
		y *= r;
		
		float h = object->GetFireHeight();
		
		vertexArray[0].Set(-x, -y, h);
		vertexArray[1].Set(-x, -y, 0.0F);
		vertexArray[2].Set(x, y, h);
		vertexArray[3].Set(x, y, 0.0F);
		
		effectList[kEffectListTransparent].Append(this);
	}
}

// ZYURVUR
