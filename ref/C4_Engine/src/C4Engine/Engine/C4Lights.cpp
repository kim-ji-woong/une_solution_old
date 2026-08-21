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
#include "C4Manipulator.h"
#include "C4Configuration.h"


using namespace C4;


const char C4::kConnectorKeyShadow[] = "%Shadow";


Light::Light(LightType type, LightType base) : Node(kNodeLight)
{
	lightType = type;
	baseLightType = base;
	
	connectedShadowSpace = nullptr;
	rootRegion = nullptr;
	
	SetActiveUpdateFlags((GetActiveUpdateFlags() & ~kUpdateBoundingSphere) | kUpdatePostBounding);
}

Light::Light(const Light& light) : Node(light)
{
	lightType = light.lightType;
	baseLightType = light.baseLightType;
	
	connectedShadowSpace = nullptr;
	rootRegion = nullptr;
	
	SetActiveUpdateFlags((GetActiveUpdateFlags() & ~kUpdateBoundingSphere) | kUpdatePostBounding);
}

Light::~Light()
{
	delete rootRegion;
}

Light *Light::Construct(Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (data.GetType())
	{
		case kLightInfinite:
			
			return (new InfiniteLight);
		
		case kLightDepth:
			
			return (new DepthLight);
		
		case kLightLandscape:
			
			return (new LandscapeLight);
		
		case kLightPoint:
			
			return (new PointLight);
		
		case kLightCube:
			
			return (new CubeLight);
		
		case kLightSpot:
			
			return (new SpotLight);
	}
	
	return (nullptr);
}

void Light::PackType(Packer& data) const
{
	Node::PackType(data);
	data << lightType;
}

void Light::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Node::Pack(data, packFlags);
	
	const Node *exclude = exclusionNode;
	if ((exclude) && (exclude->LinkedNodePackable(packFlags)))
	{
		data << ChunkHeader('EXCL', 4);
		data << exclude->GetNodeIndex();
	}
	
	data << TerminatorChunk;
}

void Light::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Node::Unpack(data, unpackFlags);
	UnpackChunkList<Light>(data, unpackFlags);
}

bool Light::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		#if C4LEGACY
		 
			case 'LINK':
			{ 
				int32	nodeIndex; 
				 
				data >> nodeIndex;
				data.AddNodeLink(nodeIndex, &ShadowSpaceLinkProc, this); 
				return (true);
			}
		
		#endif 
		
		case 'EXCL':
		{
			int32	excludeIndex; 
			
			data >> excludeIndex;
			data.AddNodeLink(excludeIndex, &ExcludeLinkProc, this);
			return (true);
		}
	}
	
	return (false);
}

#if C4LEGACY

	void Light::ShadowSpaceLinkProc(Node *node, void *cookie)
	{
		Light *light = static_cast<Light *>(cookie);
		light->SetConnectedShadowSpace(static_cast<ShadowSpace *>(node));
	}

#endif

void Light::ExcludeLinkProc(Node *node, void *cookie)
{
	Light *light = static_cast<Light *>(cookie);
	light->exclusionNode = node;
}

int32 Light::GetCategorySettingCount(Type category) const
{
	int32 count = Node::GetCategorySettingCount(category);
	if (category == 'NODE') count += 2;
	return (count);
}

Setting *Light::GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const
{
	if (category == 'NODE')
	{
		int32 count = Node::GetCategorySettingCount(category);
		if (index >= count)
		{
			const StringTable *table = TheInterfaceMgr->GetStringTable();
			
			if (index == count)
			{
				const char *title = table->GetString(StringID('NODE', kNodeLight));
				return (new HeadingSetting(kNodeLight, title));
			}
			
			if (index == count + 1)
			{
				const char *title = table->GetString(StringID('NODE', kNodeLight, 'SHAR'));
				return (new BooleanSetting('SHAR', ((GetNodeFlags() & kNodeUnsharedObject) != 0), title));
			}
			
			return (nullptr);
		}
	}
	
	return (Node::GetCategorySetting(category, index, flags));
}

void Light::SetCategorySetting(Type category, const Setting *setting)
{
	if (category == 'NODE')
	{
		Type identifier = setting->GetSettingIdentifier();
		
		if (identifier == 'SHAR')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) SetNodeFlags(GetNodeFlags() | kNodeUnsharedObject);
			else SetNodeFlags(GetNodeFlags() & ~kNodeUnsharedObject);
		}
		else
		{
			Node::SetCategorySetting('NODE', setting);
		}
	}
}

int32 Light::GetInternalConnectorCount(void) const
{
	return (1);
}

const char *Light::GetInternalConnectorKey(int32 index) const
{
	if (index == 0) return (kConnectorKeyShadow);
	return (nullptr);
}

void Light::ProcessInternalConnectors(void)
{
	Node *node = GetConnectedNode(kConnectorKeyShadow);
	if (!node)
	{
		if (GetObject()->GetLightFlags() & kLightInstanceShadowSpace)
		{
			const Node *super = GetSuperNode();
			do
			{
				if (super->GetNodeType() == kNodeInstance)
				{
					node = super->GetConnectedNode(kConnectorKeyShadow);
					break;
				}
				
				super = super->GetSuperNode();
			} while (super);
		}
	}
	
	connectedShadowSpace = static_cast<ShadowSpace *>(node);
}

bool Light::ValidConnectedNode(const ConnectorKey& key, const Node *node) const
{
	if (key == kConnectorKeyShadow)
	{
		if (node->GetNodeType() == kNodeSpace) return (static_cast<const Space *>(node)->GetSpaceType() == kSpaceShadow);
		return (false);
	}
	
	return (Node::ValidConnectedNode(key, node));
}

void Light::SetConnectedShadowSpace(ShadowSpace *shadowSpace)
{
	connectedShadowSpace = shadowSpace;
	
	if (shadowSpace)
	{
		Hub *hub = GetHub();
		if (hub)
		{
			Connector *connector = hub->FindOutgoingConnector(kConnectorKeyShadow);
			if (connector)
			{
				connector->SetConnectorTarget(shadowSpace);
				return;
			}
		}
		
		AddConnector(kConnectorKeyShadow, shadowSpace);
	}
	else
	{
		RemoveConnector(kConnectorKeyShadow);
	}
}

void Light::Neutralize(void)
{
	delete rootRegion;
	rootRegion = nullptr;
	
	Node::Neutralize();
}

void Light::InvalidateLightRegions(void)
{
	delete rootRegion;
	rootRegion = nullptr;
	
	Invalidate();
}

void Light::CalculateBoundaryPolygons(LightRegion *region) const
{
}


InfiniteLight::InfiniteLight() : Light(kLightInfinite, kLightInfinite)
{
}

InfiniteLight::InfiniteLight(LightType type) : Light(type, kLightInfinite)
{
}

InfiniteLight::InfiniteLight(const ColorRGB& color) : Light(kLightInfinite, kLightInfinite)
{
	SetNewObject(new InfiniteLightObject(color));
}

InfiniteLight::InfiniteLight(const InfiniteLight& infiniteLight) : Light(infiniteLight)
{
}

InfiniteLight::~InfiniteLight()
{
}

Node *InfiniteLight::Replicate(void) const
{
	return (new InfiniteLight(*this));
}

void InfiniteLight::CalculateIllumination(LightRegion *region)
{
	Zone *zone = region->GetZone();
	zone->SetExclusionMask(1);
	
	const Vector3D& lightDirection = GetWorldTransform()[2];
	
	unsigned_int32 portalFlagsMask = kPortalLightInhibit;
	if ((GetObject()->GetLightFlags() & (kLightStatic | kLightConfined)) != 0) portalFlagsMask |= kPortalStaticLightInhibit;
	
	const Portal *portal = zone->GetFirstPortal();
	while (portal)
	{
		if ((portal->Enabled()) || ((!region->GetSuperNode()) && (zone->GetObject()->GetZoneFlags() & kZoneTransition)))
		{
			PortalType type = portal->GetPortalType();
			const PortalObject *portalObject = portal->GetObject();
			unsigned_int32 portalFlags = portalObject->GetPortalFlags();
				
			if ((type == kPortalDirect) || ((type == kPortalRemote) && (portalFlags & kPortalAllowRemoteLight)))
			{
				if ((portalFlags & portalFlagsMask) == 0)
				{
					Zone *connectedZone = portal->GetConnectedZone();
					if ((connectedZone) && (connectedZone->GetExclusionMask() == 0))
					{
						if ((portal->GetWorldPlane() ^ lightDirection) > 0.0F)
						{
							Point3D		temp[2][kMaxPortalVertexCount];
							
							int32 vertexCount = portalObject->GetVertexCount();
							const Point3D *vertex = portal->GetWorldVertexArray();
							
							int32 planeCount = region->GetPlaneCount();
							if (planeCount > 0)
							{
								int32 count = Min(planeCount, kMaxPortalVertexCount - vertexCount);
								if (count > 0)
								{
									const Antivector4D *plane = region->GetPlaneArray();
									for (machine a = 0; a < count; a++)
									{
										int8	location[kMaxPortalVertexCount];
										
										Point3D *result = temp[a & 1];
										vertexCount = Math::ClipPolygonAgainstPlane(vertexCount, vertex, plane[a], location, result);
										if (vertexCount == 0) goto nextPortal;
										vertex = result;
									}
								}
							}
							
							LightRegion *newRegion = new LightRegion(this, connectedZone);
							newRegion->SetIlluminatedPortal(portal);
							connectedZone->AddLightRegion(newRegion);
							
							newRegion->SetPlaneCount(vertexCount);
							Antivector4D *plane = newRegion->GetPlaneArray();
							
							newRegion->SetBoundaryPolygonCount(1);
							newRegion->SetBoundaryPolygonVertexCount(0, vertexCount);
							Point3D *boundaryVertex = newRegion->GetBoundaryPolygonVertexArray();
							
							const Point3D *p1 = &vertex[vertexCount - 1];
							for (machine a = 0; a < vertexCount; a++)
							{
								const Point3D *p2 = &vertex[a];
								boundaryVertex[a] = *p2;
								
								Antivector3D normal = ((*p1 - *p2) % lightDirection).Normalize();
								plane->Set(normal, *p1);
								p1 = p2;
								plane++;
							}
							
							region->AddSubnode(newRegion);
							CalculateIllumination(newRegion);
						}
					}
				}
			}
		}
		
		nextPortal:
		portal = portal->Next();
	}
	
	const Bond *bond = zone->GetZoneSite()->GetFirstOutgoingEdge();
	while (bond)
	{
		Zone *bondZone = static_cast<Zone *>(bond->GetFinishElement());
		if (bondZone->GetExclusionMask() == 0)
		{
			if (bondZone->Visible(region))
			{
				LightRegion *newRegion = new LightRegion(this, bondZone, region);
				bondZone->AddLightRegion(newRegion);
				
				region->AddSubnode(newRegion);
				CalculateIllumination(newRegion);
			}
		}
		
		bond = bond->GetNextOutgoingEdge();
	}
	
	zone->SetExclusionMask(0);
}

void InfiniteLight::CalculatePostBounding(void)
{
	Zone	*zone;
	
	delete rootRegion;
	rootRegion = nullptr;
	
	const InfiniteLightObject *object = GetObject();
	
	if (!(object->GetLightFlags() & kLightExternalZone)) zone = GetOwningZone();
	else zone = GetWorld()->FindZone(GetWorldPosition());
	
	if (zone)
	{
		LightRegion *region = new LightRegion(this, zone);
		zone->AddLightRegion(region);
		region->SetPlaneCount(0);
		
		rootRegion = region;
		CalculateIllumination(region);
	}
}

void InfiniteLight::CalculateBoundaryPolygons(LightRegion *region) const
{
	if (region->GetBoundaryPolygonCount() != 0)
	{
		Antivector4D	oppositePlane;
		
		Vector3D lightDirection = -GetWorldTransform()[2];
		
		const Zone *zone = region->GetZone();
		const Transform4D& transform = zone->GetInverseWorldTransform();
		if (zone->GetObject()->CalculateOppositePlane(transform * lightDirection, &oppositePlane))
		{
			oppositePlane = oppositePlane * transform;
			
			int32 portalVertexCount = region->GetBoundaryPolygonVertexCount(0);
			Point3D *oppositeVertex = region->GetBoundaryPolygonVertexArray();
			const Point3D *portalVertex = oppositeVertex;
			
			oppositeVertex += portalVertexCount;
			region->SetBoundaryPolygonVertexCount(1, portalVertexCount);
			
			float f = 1.0F / (oppositePlane ^ lightDirection);
			for (machine a = 0; a < portalVertexCount; a++)
			{
				const Point3D& p = portalVertex[a];
				float t = (oppositePlane ^ p) * f;
				oppositeVertex[a] = p - lightDirection * t;
			}
			
			const Point3D *p1 = &portalVertex[portalVertexCount - 1];
			const Point3D *q1 = &oppositeVertex[portalVertexCount - 1];
			
			Point3D *vertex = oppositeVertex + portalVertexCount;
			for (machine a = 0; a < portalVertexCount; a++)
			{
				region->SetBoundaryPolygonVertexCount(a + 2, 4);
				
				const Point3D *p2 = &portalVertex[a];
				const Point3D *q2 = &oppositeVertex[a];
				
				vertex[0] = *p1;
				vertex[1] = *p2;
				vertex[2] = *q2;
				vertex[3] = *q1;
				
				vertex += 4;
				p1 = p2;
				q1 = q2;
			}
			
			region->SetBoundaryPolygonCount(portalVertexCount + 2);
		}
	}
}


DepthLight::DepthLight() : InfiniteLight(kLightDepth)
{
}

DepthLight::DepthLight(LightType type) : InfiniteLight(type)
{
}

DepthLight::DepthLight(const DepthLight& depthLight) : InfiniteLight(depthLight)
{
}

DepthLight::DepthLight(const ColorRGB& color) : InfiniteLight(kLightDepth)
{
	SetNewObject(new DepthLightObject(color));
}

DepthLight::~DepthLight()
{
}

Node *DepthLight::Replicate(void) const
{
	return (new DepthLight(*this));
}

const LightShadowData *DepthLight::CalculateShadowData(const FrustumCamera *camera)
{
	const ShadowSpace *shadowSpace = GetConnectedShadowSpace();
	if (shadowSpace)
	{
		const Transform4D& lightTransform = GetInverseWorldTransform();
		const Transform4D& shadowTransform = shadowSpace->GetWorldTransform();
		const Vector3D& size = shadowSpace->GetObject()->GetBoxSize();
		
		Point3D p0 = lightTransform * shadowTransform.GetTranslation();
		Vector3D x = lightTransform * shadowTransform[0] * size.x;
		Vector3D y = lightTransform * shadowTransform[1] * size.y;
		Vector3D z = lightTransform * shadowTransform[2] * size.z;
		
		Point3D p1 = p0 + x;
		Point3D p2 = p1 + y;
		Point3D p3 = p0 + y;
		Point3D p4 = p0 + z;
		Point3D p5 = p1 + z;
		Point3D p6 = p2 + z;
		Point3D p7 = p3 + z;
		
		float xmin = Fmin(Fmin(p0.x, p1.x, p2.x, p3.x), Fmin(p4.x, p5.x, p6.x, p7.x));
		float xmax = Fmax(Fmax(p0.x, p1.x, p2.x, p3.x), Fmax(p4.x, p5.x, p6.x, p7.x));
		float ymin = Fmin(Fmin(p0.y, p1.y, p2.y, p3.y), Fmin(p4.y, p5.y, p6.y, p7.y));
		float ymax = Fmax(Fmax(p0.y, p1.y, p2.y, p3.y), Fmax(p4.y, p5.y, p6.y, p7.y));
		float zmin = Fmin(Fmin(p0.z, p1.z, p2.z, p3.z), Fmin(p4.z, p5.z, p6.z, p7.z));
		float zmax = Fmax(Fmax(p0.z, p1.z, p2.z, p3.z), Fmax(p4.z, p5.z, p6.z, p7.z));
		
		shadowData.shadowSize.Set(xmax - xmin, ymax - ymin, zmax - zmin);
		shadowData.inverseShadowSize.Set(1.0F / shadowData.shadowSize.x, 1.0F / shadowData.shadowSize.y, 1.0F / shadowData.shadowSize.z);
		shadowData.shadowPosition.Set((xmin + xmax) * 0.5F, (ymin + ymax) * 0.5F, zmax);
	}
	else
	{
		const BoundingSphere *sphere = GetOwningZone()->GetBoundingSphere();
		if (sphere)
		{
			float radius = sphere->GetRadius();
			float diameter = radius * 2.0F;
			float inverseDiameter = 1.0F / diameter;
			
			shadowData.shadowSize.Set(diameter, diameter, diameter);
			shadowData.inverseShadowSize.Set(inverseDiameter, inverseDiameter, inverseDiameter);
			
			Point3D center = GetInverseWorldTransform() * sphere->GetCenter();
			shadowData.shadowPosition.Set(center.x, center.y, center.z + radius);
		}
		else
		{
			shadowData.shadowSize.Set(1.0F, 1.0F, 1.0F);
			shadowData.inverseShadowSize.Set(1.0F, 1.0F, 1.0F);
			shadowData.shadowPosition.Set(0.0F, 0.0F, 0.0F);
		}
	}
	
	shadowData.texelSize = Fmax(shadowData.shadowSize.x, shadowData.shadowSize.y) / (float) TheGraphicsMgr->GetDynamicShadowMapSize();
	return (&shadowData);
}


LandscapeLight::LandscapeLight() : DepthLight(kLightLandscape)
{
}

LandscapeLight::LandscapeLight(const LandscapeLight& landscapeLight) : DepthLight(landscapeLight)
{
}

LandscapeLight::LandscapeLight(const ColorRGB& color) : DepthLight(kLightLandscape)
{
	SetNewObject(new LandscapeLightObject(color));
}

LandscapeLight::~LandscapeLight()
{
}

Node *LandscapeLight::Replicate(void) const
{
	return (new LandscapeLight(*this));
}

const LightShadowData *LandscapeLight::CalculateShadowData(const FrustumCamera *camera)
{
	const LightShadowData *depthLightData = DepthLight::CalculateShadowData(camera);
	
	const Transform4D& cameraTransform = camera->GetWorldTransform();
	const Transform4D& worldTransform = GetWorldTransform();
	const Transform4D& lightTransform = GetInverseWorldTransform();
	
	Vector3D right = lightTransform * cameraTransform[0];
	Vector3D down = lightTransform * cameraTransform[1];
	Vector3D view = lightTransform * cameraTransform[2];
	Point3D p0 = lightTransform * camera->GetWorldPosition();
	
	const FrustumCameraObject *cameraObject = camera->GetObject();
	float inverseFocal = 1.0F / cameraObject->GetFocalLength();
	float aspect = cameraObject->GetAspectRatio();
	
	float inverseShadowMapSize = 1.0F / (float) TheGraphicsMgr->GetDynamicShadowMapSize();
	
	const Range<float> *sectionRange = GetObject()->GetSectionRangeArray();
	
	for (machine a = 0; a < kMaxShadowSectionCount; a++)
	{
		float e1 = sectionRange[a].min;
		float e2 = sectionRange[a].max;
		
		Point3D center1 = p0 + view * e1;
		Point3D center2 = p0 + view * e2;
		Vector3D v1 = right * (e1 * inverseFocal);
		Vector3D w1 = down * (e1 * aspect * inverseFocal);
		Vector3D v2 = right * (e2 * inverseFocal);
		Vector3D w2 = down * (e2 * aspect * inverseFocal);
		
		Point3D p1 = center1 + v1 + w1;
		Point3D p2 = center1 + v1 - w1;
		Point3D p3 = center1 - v1 + w1;
		Point3D p4 = center1 - v1 - w1;
		
		shadowData[a].sectionPolygon[0] = worldTransform * p1;
		shadowData[a].sectionPolygon[1] = worldTransform * p2;
		shadowData[a].sectionPolygon[2] = worldTransform * p3;
		shadowData[a].sectionPolygon[3] = worldTransform * p4;
		
		Point3D q1 = center2 + v2 + w2;
		Point3D q2 = center2 + v2 - w2;
		Point3D q3 = center2 - v2 + w2;
		Point3D q4 = center2 - v2 - w2;
		
		float xmin = Fmin(Fmin(p1.x, p2.x, p3.x, p4.x), Fmin(q1.x, q2.x, q3.x, q4.x));
		float xmax = Fmax(Fmax(p1.x, p2.x, p3.x, p4.x), Fmax(q1.x, q2.x, q3.x, q4.x));
		float ymin = Fmin(Fmin(p1.y, p2.y, p3.y, p4.y), Fmin(q1.y, q2.y, q3.y, q4.y));
		float ymax = Fmax(Fmax(p1.y, p2.y, p3.y, p4.y), Fmax(q1.y, q2.y, q3.y, q4.y));
		float zmin = Fmin(Fmin(p1.z, p2.z, p3.z, p4.z), Fmin(q1.z, q2.z, q3.z, q4.z));
		float zmax = Fmax(Fmax(p1.z, p2.z, p3.z, p4.z), Fmax(q1.z, q2.z, q3.z, q4.z));
		
		zmax = Fmax(zmax, depthLightData->shadowPosition.z);
		float dz = zmax - zmin;
		
		float size = PositiveCeil(Magnitude(p1 - q4));
		shadowData[a].shadowSize.Set(size, size, dz);
		
		float inverseSize = 1.0F / size;
		shadowData[a].inverseShadowSize.Set(inverseSize, inverseSize, 1.0F / dz);
		
		float texel = size * inverseShadowMapSize;
		float inverseTexel = 1.0F / texel;
		shadowData[a].texelSize = texel;
		
		float x = (xmin + xmax) * 0.5F;
		float y = (ymin + ymax) * 0.5F;
		x = Floor(x * inverseTexel + 0.5F) * texel;
		y = Floor(y * inverseTexel + 0.5F) * texel;
		
		shadowData[a].shadowPosition.Set(x, y, zmax);
		shadowData[a].nearPlane.Set(cameraTransform[2], worldTransform * center1);
		shadowData[a].farPlane.Set(-cameraTransform[2], worldTransform * center2);
		
		if (a < kMaxShadowSectionCount - 1)
		{
			float nextStart = sectionRange[a + 1].min;
			shadowData[a].sectionPlane.Set(cameraTransform[2] / (e2 - nextStart), worldTransform * (p0 + view * nextStart));
		}
		else
		{
			shadowData[a].sectionPlane.Set(0.0F, 0.0F, 0.0F, 0.0F);
		}
	}
	
	return (shadowData);
}


PointLight::PointLight() : Light(kLightPoint, kLightPoint)
{
}

PointLight::PointLight(LightType type) : Light(type, kLightPoint)
{
}

PointLight::PointLight(const ColorRGB& color, float range) : Light(kLightPoint, kLightPoint)
{
	SetNewObject(new PointLightObject(color, range));
}

PointLight::PointLight(const PointLight& pointLight) : Light(pointLight)
{
}

PointLight::~PointLight()
{
}

Node *PointLight::Replicate(void) const
{
	return (new PointLight(*this));
}

void PointLight::CalculateIllumination(LightRegion *region)
{
	Zone *zone = region->GetZone();
	zone->SetExclusionMask(1);
	
	const Point3D& lightPosition = GetWorldPosition();
	const PointLightObject *lightObject = GetObject();
	float lightRange = lightObject->GetLightRange();
	unsigned_int32 lightFlags = lightObject->GetLightFlags();
	if (lightFlags & kLightConfined) lightRange += lightObject->GetConfinementRadius();
	
	if ((zone->GetFirstSubzone()) || (!zone->GetObject()->InteriorSphere(zone->GetInverseWorldTransform() * lightPosition, lightRange)))
	{
		unsigned_int32 portalFlagsMask = kPortalLightInhibit;
		if ((lightFlags & (kLightStatic | kLightConfined)) != 0) portalFlagsMask |= kPortalStaticLightInhibit;
		
		const Portal *portal = zone->GetFirstPortal();
		while (portal)
		{
			if ((portal->Enabled()) || ((!region->GetSuperNode()) && (zone->GetObject()->GetZoneFlags() & kZoneTransition)))
			{
				PortalType type = portal->GetPortalType();
				const PortalObject *portalObject = portal->GetObject();
				unsigned_int32 portalFlags = portalObject->GetPortalFlags();
				
				if ((type == kPortalDirect) || ((type == kPortalRemote) && (portalFlags & kPortalAllowRemoteLight)))
				{
					if ((portalFlags & portalFlagsMask) == 0)
					{
						Zone *connectedZone = portal->GetConnectedZone();
						if ((connectedZone) && (connectedZone->GetExclusionMask() == 0))
						{
							float distance = portal->GetWorldPlane() ^ lightPosition;
							if (Fabs(distance) < lightRange)
							{
								const BoundingSphere *sphere = portal->GetBoundingSphere();
								float r = sphere->GetRadius() + lightRange;
								if (SquaredMag(sphere->GetCenter() - lightPosition) < r * r)
								{
									LightRegion		*newRegion;
									Point3D			temp[2][kMaxPortalVertexCount];
									
									int32 vertexCount = portalObject->GetVertexCount();
									const Point3D *vertex = portal->GetWorldVertexArray();
									
									if (distance > kMinPortalClipDistance)
									{
										int32 planeCount = region->GetPlaneCount();
										if (planeCount > 0)
										{
											int32 count = Min(planeCount, kMaxPortalVertexCount - vertexCount);
											if (count > 0)
											{
												const Antivector4D *plane = region->GetPlaneArray();
												for (machine a = 0; a < count; a++)
												{
													int8	location[kMaxPortalVertexCount];
													
													Point3D *result = temp[a & 1];
													vertexCount = Math::ClipPolygonAgainstPlane(vertexCount, vertex, plane[a], location, result);
													if (vertexCount == 0) goto nextPortal;
													vertex = result;
												}
											}
										}
										
										newRegion = new LightRegion(this, connectedZone);
										
										newRegion->SetPlaneCount(vertexCount);
										Antivector4D *plane = newRegion->GetPlaneArray();
										
										newRegion->SetBoundaryPolygonCount(1);
										newRegion->SetBoundaryPolygonVertexCount(0, vertexCount);
										Point3D *boundaryVertex = newRegion->GetBoundaryPolygonVertexArray();
										
										Vector3D v1 = vertex[vertexCount - 1] - lightPosition;
										for (machine a = 0; a < vertexCount; a++)
										{
											const Point3D& p = vertex[a];
											boundaryVertex[a] = p;
											
											Vector3D v2 = p - lightPosition;
											Antivector3D normal = (v2 % v1).Normalize();
											
											if (!(lightFlags & kLightConfined))
											{
												plane->Set(normal, lightPosition);
											}
											else
											{
												float d = Math::DistancePointToLine(lightPosition, p, v2 - v1);
												float cr = lightObject->GetConfinementRadius();
												if (d > cr)
												{
													Vector3D offset = normal * (d * cr * InverseSqrt(d * d - cr * cr));
													normal = ((v2 - offset) % (v1 - offset)).Normalize();
													plane->Set(normal, lightPosition + offset);
												}
												else
												{
													plane->Set(normal, lightPosition);
												}
											}
											
											v1 = v2;
											plane++;
										}
									}
									else
									{
										const Vector3D& normal = portal->GetWorldPlane().GetAntivector3D();
										if (Math::PointInConvexPolygon(lightPosition, vertexCount, vertex, normal) == kPolygonExterior) goto nextPortal;
										
										newRegion = new LightRegion(this, connectedZone, region);
									}
									
									region->AddSubnode(newRegion);
									connectedZone->AddLightRegion(newRegion);
									
									newRegion->SetIlluminatedPortal(portal);
									CalculateIllumination(newRegion);
								}
							}
						}
					}
				}
			}
			
			nextPortal:
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
				float r = sphere->GetRadius() + lightRange;
				if ((SquaredMag(sphere->GetCenter() - lightPosition) < r * r) && (bondZone->Visible(region)))
				{
					LightRegion *newRegion = new LightRegion(this, bondZone, region);
					bondZone->AddLightRegion(newRegion);
					
					region->AddSubnode(newRegion);
					CalculateIllumination(newRegion);
				}
			}
		}
		
		bond = bond->GetNextOutgoingEdge();
	}
	
	zone->SetExclusionMask(0);
}

void PointLight::CalculatePostBounding(void)
{
	Zone	*zone;
	
	const PointLightObject *object = GetObject();
	unsigned_int32 lightFlags = object->GetLightFlags();
	
	if (rootRegion)
	{
		if (lightFlags & kLightConfined) return;
		
		delete rootRegion;
		rootRegion = nullptr;
	}
	
	if (!(lightFlags & kLightExternalZone)) zone = GetOwningZone();
	else zone = GetWorld()->FindZone(GetWorldPosition());
	
	if (zone)
	{
		LightRegion *region = new LightRegion(this, zone);
		zone->AddLightRegion(region);
		region->SetPlaneCount(0);
		
		rootRegion = region;
		CalculateIllumination(region);
	}
}

void PointLight::CalculateBoundaryPolygons(LightRegion *region) const
{
	if (region->GetBoundaryPolygonCount() != 0)
	{
		Antivector4D	oppositePlane;
		
		const Point3D& lightPosition = GetWorldPosition();
		
		const Portal *portal = region->GetIlluminatedPortal();
		const Vector3D& direction = portal->GetWorldPlane().GetAntivector3D();
		
		const PointLightObject *object = GetObject();
		float range = object->GetLightRange();
		unsigned_int32 lightFlags = object->GetLightFlags();
		if (lightFlags & kLightConfined) range += object->GetConfinementRadius();
		
		Antivector4D backPlane(direction, range - lightPosition * direction);
		
		const Zone *zone = region->GetZone();
		const Transform4D& zoneTransform = zone->GetInverseWorldTransform();
		if (zone->GetObject()->CalculateOppositePlane(-(zoneTransform * direction), &oppositePlane))
		{
			oppositePlane = oppositePlane * zoneTransform;
			if ((oppositePlane ^ lightPosition) < range) backPlane = oppositePlane;
		}
		
		int32 portalVertexCount = region->GetBoundaryPolygonVertexCount(0);
		Point3D *oppositeVertex = region->GetBoundaryPolygonVertexArray();
		const Point3D *portalVertex = oppositeVertex;
		
		oppositeVertex += portalVertexCount;
		region->SetBoundaryPolygonVertexCount(1, portalVertexCount);
		
		const Antivector4D *lateralPlane1 = region->GetPlaneArray();
		const Antivector4D *lateralPlane2 = lateralPlane1 + (portalVertexCount - 1);
		
		for (machine a = portalVertexCount - 1; a >= 0; a--)
		{
			Vector3D dp = lateralPlane1->GetAntivector3D() % lateralPlane2->GetAntivector3D();
			const Point3D& p = portalVertex[a];
			float t = (backPlane ^ p) / (backPlane ^ dp);
			oppositeVertex[a] = p - dp * t;
			
			lateralPlane1 = lateralPlane2;
			lateralPlane2--;
		}
		
		const Point3D *p1 = &portalVertex[portalVertexCount - 1];
		const Point3D *q1 = &oppositeVertex[portalVertexCount - 1];
		
		Point3D *vertex = oppositeVertex + portalVertexCount;
		for (machine a = 0; a < portalVertexCount; a++)
		{
			region->SetBoundaryPolygonVertexCount(a + 2, 4);
			
			const Point3D *p2 = &portalVertex[a];
			const Point3D *q2 = &oppositeVertex[a];
			
			vertex[0] = *p1;
			vertex[1] = *p2;
			vertex[2] = *q2;
			vertex[3] = *q1;
			
			vertex += 4;
			p1 = p2;
			q1 = q2;
		}
		
		region->SetBoundaryPolygonCount(portalVertexCount + 2);
	}
}


CubeLight::CubeLight() : PointLight(kLightCube)
{
}

CubeLight::CubeLight(const ColorRGB& color, float range, const char *name) : PointLight(kLightCube)
{
	SetNewObject(new CubeLightObject(color, range, name));
}

CubeLight::CubeLight(const CubeLight& cubeLight) : PointLight(cubeLight)
{
}

CubeLight::~CubeLight()
{
}

Node *CubeLight::Replicate(void) const
{
	return (new CubeLight(*this));
}


SpotLight::SpotLight() : PointLight(kLightSpot)
{
}

SpotLight::SpotLight(const ColorRGB& color, float range, float apex, const char *name) : PointLight(kLightSpot)
{
	SetNewObject(new SpotLightObject(color, range, apex, name));
}

SpotLight::SpotLight(const SpotLight& spotLight) : PointLight(spotLight)
{
}

SpotLight::~SpotLight()
{
}

Node *SpotLight::Replicate(void) const
{
	return (new SpotLight(*this));
}

void SpotLight::CalculatePostBounding(void)
{
	Zone	*zone;
	
	const SpotLightObject *object = GetObject();
	unsigned_int32 lightFlags = object->GetLightFlags();
	
	if (rootRegion)
	{
		if (lightFlags & kLightConfined) return;
		
		delete rootRegion;
		rootRegion = nullptr;
	}
	
	if (!(object->GetLightFlags() & kLightExternalZone)) zone = GetOwningZone();
	else zone = GetWorld()->FindZone(GetWorldPosition());
	
	if (zone)
	{
		LightRegion *region = new LightRegion(this, zone);
		zone->AddLightRegion(region);
		
		region->SetPlaneCount(5);
		Antivector4D *plane = region->GetPlaneArray();
		
		float e = object->GetApexTangent();
		float a = object->GetAspectRatio();
		float r1 = InverseSqrt(e * e + 1.0F);
		float r2 = InverseSqrt(e * e + a * a);
		
		float w = (lightFlags & kLightConfined) ? object->GetConfinementRadius() : 0.0F;
		
		plane[0].Set(e * r1, 0.0F, r1, w);
		plane[1].Set(0.0F, e * r2, a * r2, w);
		plane[2].Set(-e * r1, 0.0F, r1, w);
		plane[3].Set(0.0F, -e * r2, a * r2, w);
		
		const Transform4D& transform = GetInverseWorldTransform();
		for (machine i = 0; i < 4; i++) plane[i] = plane[i] * transform;
		
		const Vector3D& direction = GetWorldTransform()[2];
		plane[4].Set(direction, w - (direction * GetWorldPosition()));
		
		rootRegion = region;
		CalculateIllumination(region);
	}
}

void SpotLight::CalculateBoundaryPolygons(LightRegion *region) const
{
	if (region->GetSuperNode())
	{
		PointLight::CalculateBoundaryPolygons(region);
	}
	else
	{
		Antivector4D	oppositePlane;
		
		const Transform4D& lightTransform = GetWorldTransform();
		const Point3D& lightPosition = GetWorldPosition();
		
		const SpotLightObject *object = GetObject();
		float range = object->GetLightRange();
		unsigned_int32 lightFlags = object->GetLightFlags();
		if (lightFlags & kLightConfined) range += object->GetConfinementRadius();
		
		Antivector4D backPlane(-lightTransform[2], lightPosition * lightTransform[2] + range);
		
		const Zone *zone = region->GetZone();
		const Transform4D& zoneTransform = zone->GetInverseWorldTransform();
		if (zone->GetObject()->CalculateOppositePlane(zoneTransform * lightTransform[2], &oppositePlane))
		{
			oppositePlane = oppositePlane * zoneTransform;
			if ((oppositePlane ^ lightPosition) < range) backPlane = oppositePlane;
		}
		
		Point3D *vertex = region->GetBoundaryPolygonVertexArray();
		const Antivector4D *lateralPlane1 = region->GetPlaneArray();
		const Antivector4D *lateralPlane2 = lateralPlane1 + 3;
		
		if (!(lightFlags & kLightConfined))
		{
			region->SetBoundaryPolygonVertexCount(0, 4);
			
			for (machine a = 3; a >= 0; a--)
			{
				Vector3D dp = lateralPlane1->GetAntivector3D() % lateralPlane2->GetAntivector3D();
				float t = (backPlane ^ lightPosition) / (backPlane ^ dp);
				vertex[a] = lightPosition - dp * t;
				
				lateralPlane1 = lateralPlane2;
				lateralPlane2--;
			}
			
			region->SetBoundaryPolygonVertexCount(1, 3);
			vertex[4] = lightPosition;
			vertex[5] = vertex[0];
			vertex[6] = vertex[1];
			
			region->SetBoundaryPolygonVertexCount(2, 3);
			vertex[7] = lightPosition;
			vertex[8] = vertex[1];
			vertex[9] = vertex[2];
			
			region->SetBoundaryPolygonVertexCount(3, 3);
			vertex[10] = lightPosition;
			vertex[11] = vertex[2];
			vertex[12] = vertex[3];
			
			region->SetBoundaryPolygonVertexCount(4, 3);
			vertex[13] = lightPosition;
			vertex[14] = vertex[3];
			vertex[15] = vertex[0];
			
			region->SetBoundaryPolygonCount(5);
		}
		else
		{
			const Antivector4D& frontPlane = lateralPlane1[4];
			
			float e = object->GetApexTangent();
			float r = object->GetConfinementRadius();
			Point3D q = lightPosition - lightTransform[2] * (r * Sqrt(1.0F + e * e));
			
			region->SetBoundaryPolygonVertexCount(0, 4);
			region->SetBoundaryPolygonVertexCount(1, 4);
			
			for (machine a = 3; a >= 0; a--)
			{
				Vector3D dp = lateralPlane1->GetAntivector3D() % lateralPlane2->GetAntivector3D();
				vertex[a] = q - dp * ((frontPlane ^ q) / (frontPlane ^ dp));
				vertex[a + 4] = q - dp * ((backPlane ^ q) / (backPlane ^ dp));
				
				lateralPlane1 = lateralPlane2;
				lateralPlane2--;
			}
			
			const Point3D *p1 = vertex + 3;
			const Point3D *q1 = vertex + 7;
			Point3D *lateralVertex = vertex + 8;
			
			for (machine a = 0; a < 4; a++)
			{
				region->SetBoundaryPolygonVertexCount(a + 2, 4);
				
				const Point3D *p2 = vertex + a;
				const Point3D *q2 = vertex + (a + 4);
				
				lateralVertex[0] = *p1;
				lateralVertex[1] = *p2;
				lateralVertex[2] = *q2;
				lateralVertex[3] = *q1;
				
				lateralVertex += 4;
				p1 = p2;
				q1 = q2;
			}
			
			region->SetBoundaryPolygonCount(6);
		}
	}
}

// ZYURVUR
