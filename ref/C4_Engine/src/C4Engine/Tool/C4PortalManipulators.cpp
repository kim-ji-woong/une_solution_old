//=============================================================
//
// C4 Engine version 2.9.1
// Copyright 1999-2012, by Terathon Software LLC
//
// This file is part of the C4 Engine and is provided under the
// terms of the license agreement entered by the registed user.
//
// Unauthorized redistribution of source code is strictly
// prohibited. Violators will be prosecuted.
//
//=============================================================


#include "C4PortalManipulators.h"
#include "C4WorldEditor.h"


using namespace C4;


namespace
{
	const ConstColorRGBA kDirectPortalColor = {0.0F, 1.0F, 0.0F, 1.0F};
	const ConstColorRGBA kRemotePortalColor = {0.5F, 0.0F, 1.0F, 1.0F};
	const ConstColorRGBA kOcclusionPortalColor = {1.0F, 0.0F, 0.0F, 1.0F};
	
	
	const TextureHeader outlineTextureHeader =
	{
		kTexture2D,
		kTextureForceHighQuality,
		kTextureSemanticEmission,
		kTextureSemanticTransparency,
		kTextureI8,
		8, 1, 1,
		{kTextureClamp, kTextureRepeat, kTextureRepeat},
		4
	};
	
	
	const unsigned_int8 outlineTextureImage[15] =
	{
		0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00,
		0x80, 0xFF, 0xFF, 0x80,
		0xC0, 0xC0,
		0xC0
	};
}


const ConstPoint2D PortalManipulator::outlineTexcoord[kMaxPortalVertexCount * 4] =
{
	{0.0F, 0.0F}, {1.0F, 0.0F}, {1.0F, 0.0F}, {0.0F, 0.0F},
	{0.0F, 0.0F}, {1.0F, 0.0F}, {1.0F, 0.0F}, {0.0F, 0.0F},
	{0.0F, 0.0F}, {1.0F, 0.0F}, {1.0F, 0.0F}, {0.0F, 0.0F},
	{0.0F, 0.0F}, {1.0F, 0.0F}, {1.0F, 0.0F}, {0.0F, 0.0F},
	{0.0F, 0.0F}, {1.0F, 0.0F}, {1.0F, 0.0F}, {0.0F, 0.0F},
	{0.0F, 0.0F}, {1.0F, 0.0F}, {1.0F, 0.0F}, {0.0F, 0.0F},
	{0.0F, 0.0F}, {1.0F, 0.0F}, {1.0F, 0.0F}, {0.0F, 0.0F},
	{0.0F, 0.0F}, {1.0F, 0.0F}, {1.0F, 0.0F}, {0.0F, 0.0F},
	{0.0F, 0.0F}, {1.0F, 0.0F}, {1.0F, 0.0F}, {0.0F, 0.0F},
	{0.0F, 0.0F}, {1.0F, 0.0F}, {1.0F, 0.0F}, {0.0F, 0.0F},
	{0.0F, 0.0F}, {1.0F, 0.0F}, {1.0F, 0.0F}, {0.0F, 0.0F},
	{0.0F, 0.0F}, {1.0F, 0.0F}, {1.0F, 0.0F}, {0.0F, 0.0F}
};


PortalManipulator::PortalManipulator(Portal *portal, const ColorRGBA& color) :
		EditorManipulator(portal, "WorldEditor/portal/Direct"),
		outlineDiffuseColor(kAttributeMutable),
		outlineTextureMap(&outlineTextureHeader, outlineTextureImage),
		outlineRenderable(kRenderQuads, kRenderDepthTest | kRenderDepthInhibit | kRenderDepthOffset),
		directionDiffuseColor(kAttributeMutable),
		directionTextureMap("WorldEditor/direction"),
		directionRenderable(kRenderQuads, kRenderDepthTest | kRenderDepthInhibit | kRenderDepthOffset)
{
	portalColor = color;
	sizeVector.Set(1.0F, 1.0F, 1.0F, 1.0F);
	Unselect();
	
	outlineAttributeList.Append(&outlineDiffuseColor);
	outlineAttributeList.Append(&outlineTextureMap);
	outlineRenderable.SetMaterialAttributeList(&outlineAttributeList);
	outlineRenderable.SetDepthOffset(0.0078125F, portal->GetBoundingSphereCenterPointer());
	outlineRenderable.SetAmbientBlendState(kBlendInterpolate);
	outlineRenderable.SetShaderFlags(kShaderAmbientEffect | kShaderVertexPolyboard | kShaderLinearPolyboard | kShaderOrthoPolyboard | kShaderScaleVertex);
	outlineRenderable.SetRenderParameterPointer(&sizeVector);
	outlineRenderable.SetTransformable(portal);
	outlineRenderable.SetAttributeArray(kArrayVertex, outlineVertex);
	outlineRenderable.SetAttributeArray(kArrayTangent, outlineTangent);
	outlineRenderable.SetAttributeArray(kArrayTexture0, &outlineTexcoord[0]);
	
	directionAttributeList.Append(&directionDiffuseColor);
	directionAttributeList.Append(&directionTextureMap);
	directionRenderable.SetMaterialAttributeList(&directionAttributeList);
	directionRenderable.SetDepthOffset(0.0078125F, portal->GetBoundingSphereCenterPointer());
	directionRenderable.SetAmbientBlendState(kBlendInterpolate);
	directionRenderable.SetTransformable(portal);
	directionRenderable.GetFirstRenderSegment()->SetMaterialState(kMaterialTwoSided);
	directionRenderable.SetAttributeArray(kArrayVertex, directionVertex);
	directionRenderable.SetAttributeArray(kArrayTexture0, directionTexcoord);
}

PortalManipulator::~PortalManipulator()
{
}

Manipulator *PortalManipulator::Construct(Portal *portal)
{
	switch (portal->GetPortalType())
	{
		case kPortalDirect:
			
			return (new DirectPortalManipulator(static_cast<DirectPortal *>(portal)));
		 
		case kPortalRemote:
			 
			return (new RemotePortalManipulator(static_cast<RemotePortal *>(portal))); 
		 
		case kPortalOcclusion:
			 
			return (new OcclusionPortalManipulator(static_cast<OcclusionPortal *>(portal)));
	}
	
	return (nullptr); 
}

const char *PortalManipulator::GetDefaultNodeName(void) const
{ 
	return (TheWorldEditor->GetStringTable()->GetString(StringID('NAME', kNodePortal, GetTargetNode()->GetPortalType())));
}

void PortalManipulator::Invalidate(void)
{
	EditorManipulator::Invalidate();
	
	Zone *zone = GetTargetNode()->GetOwningZone();
	if (zone)
	{
		zone->InvalidateLightRegions();
		zone->ProcessTransitions();
	}
}

void PortalManipulator::Select(void)
{
	EditorManipulator::Select();
	outlineDiffuseColor.SetDiffuseColor(K::white);
	directionDiffuseColor.SetDiffuseColor(portalColor);
}

void PortalManipulator::Unselect(void)
{
	EditorManipulator::Unselect();
	outlineDiffuseColor.SetDiffuseColor(portalColor);
	directionDiffuseColor.SetDiffuseColor(portalColor * 0.5F);
}

void PortalManipulator::HandleConnectorUpdate(void)
{
	EditorManipulator::HandleConnectorUpdate();
	
	Zone *zone = GetTargetNode()->GetOwningZone();
	if (zone)
	{
		zone->InvalidateLightRegions();
		zone->ProcessTransitions();
	}
}

Box3D PortalManipulator::CalculateNodeBoundingBox(void) const
{
	const PortalObject *object = GetObject();
	int32 vertexCount = object->GetVertexCount();
	const Point3D *vertex = object->GetVertexArray();
	
	Box3D box(vertex[0], vertex[0]);
	for (machine a = 1; a < vertexCount; a++) box.Union(vertex[a]);
	
	return (box);
}

int32 PortalManipulator::GetHandleTable(Point3D *handle) const
{
	const PortalObject *object = GetObject();
	int32 vertexCount = object->GetVertexCount();
	const Point3D *vertex = object->GetVertexArray();
	
	for (machine a = 0; a < vertexCount; a++) handle[a] = vertex[a];
	return (vertexCount);
}

void PortalManipulator::GetHandleData(int32 index, ManipulatorHandleData *handleData) const
{
	const PortalObject *object = GetObject();
	int32 vertexCount = object->GetVertexCount();
	const Point3D *vertex = object->GetVertexArray();
	
	int32 opposite = index;
	float distance = 0.0F;
	
	const Point3D& p = vertex[index];
	for (machine a = 0; a < vertexCount; a++) if (a != index)
	{
		float d = SquaredMag(vertex[a] - p);
		if (d > distance)
		{
			distance = d;
			opposite = (int32) a;
		}
	}
	
	handleData->handleFlags = 0;
	handleData->oppositeIndex = opposite;
}

bool PortalManipulator::Pick(const Ray *ray, PickData *data) const
{
	const PortalObject *object = GetObject();
	int32 vertexCount = object->GetVertexCount();
	const Point3D *vertex = object->GetVertexArray();
	
	float r = (ray->radius != 0.0F) ? ray->radius : Editor::kFrustumRenderScale;
	float r2 = r * r * 16.0F;
	
	const Point3D *p1 = &vertex[vertexCount - 1];
	for (machine a = 0; a < vertexCount; a++)
	{
		float	s, t;
		
		const Point3D *p2 = &vertex[a];
		Vector3D dp = *p2 - *p1;
		
		if ((Math::CalculateNearestParameters(*p1, dp, ray->origin, ray->direction, &s, &t)) && (t < ray->tmax))
		{
			Point3D q = *p1 + dp * s;
			if (SquaredMag(q - ray->origin - ray->direction * t) < r2)
			{
				float f = dp * dp + r2;
				if ((Math::SquaredDistancePointToLine(*p1, ray->origin, ray->direction) < f) && (Math::SquaredDistancePointToLine(*p2, ray->origin, ray->direction) < f))
				{
					data->rayParam = t;
					data->pickIndex[0] = a;
					data->pickPoint = q;
					return (true);
				}
			}
		}
		
		p1 = p2;
	}
	
	return (false);
}

bool PortalManipulator::RegionPick(const Region *region) const
{
	const Transform4D& worldTransform = GetTargetNode()->GetWorldTransform();
	
	const PortalObject *object = GetObject();
	int32 vertexCount = object->GetVertexCount();
	const Point3D *vertex = object->GetVertexArray();
	
	Point3D p1 = worldTransform * vertex[vertexCount - 1];
	for (machine a = 0; a < vertexCount; a++)
	{
		Point3D p2 = worldTransform * vertex[a];
		if (region->CylinderVisible(p1, p2, 0.0F)) return (true);
		p1 = p2;
	}
	
	return (false);
}

void PortalManipulator::BeginResize(const ManipulatorResizeData *resizeData)
{
	EditorManipulator::BeginResize(resizeData);
	
	const PortalObject *object = GetObject();
	const Point3D *vertex = object->GetVertexArray();
	originalVertexPosition = vertex[resizeData->handleIndex];
}

Point3D PortalManipulator::ConstrainVertex(const Point3D& original, const Point3D& current, const Point3D& v1, const Point3D& v2)
{
	float x = current.x;
	float y = current.y;
	
	Vector2D dv(v1.x - v2.x, v1.y - v2.y);
	if ((x - v2.x) * dv.y - (y - v2.y) * dv.x < 0.0F)
	{
		float dx = x - original.x;
		float dy = y - original.y;
		
		Vector3D plane(dv.y, -dv.x, dv.x * v1.y - dv.y * v1.x);
		float t = -(plane.x * original.x + plane.y * original.y + plane.z) / (plane.x * dx + plane.y * dy);
		x = original.x + t * dx;
		y = original.y + t * dy;
	}
	
	return (Point3D(x, y, 0.0F));
}

bool PortalManipulator::Resize(const ManipulatorResizeData *resizeData)
{
	if (resizeData->handleFlags == 0)
	{
		Portal *portal = GetTargetNode();
		Point3D p = portal->GetWorldTransform() * (originalVertexPosition + resizeData->resizeDelta);
		p = portal->GetInverseWorldTransform() * GetEditor()->SnapToGrid(p);
		
		PortalObject *object = GetObject();
		Point3D *vertex = object->GetVertexArray();
		
		int32 count = object->GetVertexCount();
		int32 index = resizeData->handleIndex;
		
		const Point3D *v1 = &vertex[(index != count - 1) ? index + 1 : 0];
		const Point3D *v2 = &vertex[(index != 0) ? index - 1 : count - 1];
		p = ConstrainVertex(originalVertexPosition, p, *v1, *v2);
		
		if (count > 3)
		{
			const Point3D *v3 = &vertex[(index < count - 2) ? index + 2 : index + 2 - count];
			p = ConstrainVertex(originalVertexPosition, p, *v1, *v3);
			
			v3 = &vertex[(index > 1) ? index - 2 : index - 2 + count];
			p = ConstrainVertex(originalVertexPosition, p, *v3, *v2);
		}
		
		vertex[index] = p;
	}
	
	return (false);
}

void PortalManipulator::Update(void)
{
	if (!(GetManipulatorState() & kManipulatorUpdated))
	{
		const PortalObject *object = GetObject();
		int32 vertexCount = object->GetVertexCount();
		
		outlineRenderable.SetVertexCount(vertexCount * 4);
		directionRenderable.SetVertexCount(vertexCount * 4);
		
		const Point3D *portalVertex = object->GetVertexArray();
		const Point3D *p1 = &portalVertex[vertexCount - 1];
		
		Point3D *vertex1 = outlineVertex;
		Vector4D *tangent = outlineTangent;
		Point3D *vertex2 = directionVertex;
		Point2D *texcoord = directionTexcoord;
		
		for (machine a = 0; a < vertexCount; a++)
		{
			const Point3D *p2 = &portalVertex[a];
			
			vertex1[0] = *p1;
			vertex1[1] = *p1;
			vertex1[2] = *p2;
			vertex1[3] = *p2;
			
			Vector3D dp = *p2 - *p1;
			float m = Magnitude(dp);
			dp /= m;
			
			tangent[0].Set(dp, -1.0F);
			tangent[1].Set(dp, 1.0F);
			tangent[2].Set(dp, 1.0F);
			tangent[3].Set(dp, -1.0F);
			
			vertex2[0] = *p1;
			vertex2[1].Set(p1->x, p1->y, 0.125F);
			vertex2[2].Set(p2->x, p2->y, 0.125F);
			vertex2[3] = *p2;
			
			m *= 8.0F;
			texcoord[0].Set(0.0F, 1.0F);
			texcoord[1].Set(0.0F, 0.0F);
			texcoord[2].Set(m, 0.0F);
			texcoord[3].Set(m, 1.0F);
			
			vertex1 += 4;
			tangent += 4;
			vertex2 += 4;
			texcoord += 4;
			p1 = p2;
		}
	}
	
	EditorManipulator::Update();
}

void PortalManipulator::Render(const ManipulatorRenderData *renderData)
{
	List<Renderable> *renderList = renderData->manipulatorList;
	if (renderList)
	{
		sizeVector.w = renderData->viewportScale;
		renderList->Append(&directionRenderable);
		renderList->Append(&outlineRenderable);
	}
	
	EditorManipulator::Render(renderData);
}


DirectPortalManipulator::DirectPortalManipulator(DirectPortal *portal) : PortalManipulator(portal, kDirectPortalColor)
{
}

DirectPortalManipulator::~DirectPortalManipulator()
{
}


RemotePortalManipulator::RemotePortalManipulator(RemotePortal *portal) : PortalManipulator(portal, kRemotePortalColor)
{
}

RemotePortalManipulator::~RemotePortalManipulator()
{
}


OcclusionPortalManipulator::OcclusionPortalManipulator(OcclusionPortal *portal) : PortalManipulator(portal, kOcclusionPortalColor)
{
}

OcclusionPortalManipulator::~OcclusionPortalManipulator()
{
}

// ZYURVUR
