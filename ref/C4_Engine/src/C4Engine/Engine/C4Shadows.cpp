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


#include "C4Lights.h"
#include "C4Spaces.h"


using namespace C4;


StencilData::StencilData(Geometry *geometry)
{
	shadowGeometry = geometry;
	shadowStorage = nullptr;
	shadowPolyhedron = nullptr;
}

StencilData::~StencilData()
{
	delete[] shadowStorage;
}

bool *StencilData::Activate(bool front)
{
	const GeometryObject *geometryObject = shadowGeometry->GetObject();
	const GeometryLevel *geometryLevel = geometryObject->GetGeometryLevel(0);
	
	int32 faceCount = geometryLevel->GetArrayDescriptor(kArrayFace)->elementCount;
	int32 edgeCount = geometryLevel->GetArrayDescriptor(kArrayEdge)->elementCount;
		
	unsigned_int32 size = sizeof(Polyhedron) + sizeof(Vector4D) * 4 * edgeCount + sizeof(Triangle) * faceCount;
	if (front)
	{
		int32	planeCount;
		
		if (!geometryLevel->GetWeightData())
		{
			planeCount = geometryLevel->GetArrayDescriptor(kArrayPlane)->elementCount;
			
			int32 levelCount = geometryObject->GetGeometryLevelCount();
			for (machine a = 1; a < levelCount; a++)
			{
				const GeometryLevel *level = geometryObject->GetGeometryLevel(a);
				planeCount = Max(planeCount, level->GetArrayDescriptor(kArrayPlane)->elementCount);
			}
		}
		else
		{
			planeCount = faceCount;
		}
		
		size += planeCount;
		if (geometryObject->GetGeometryFlags() & kGeometryTwoSidedPlaneArray) size += planeCount;
	}
	
	char *pointer = new char[size];
	shadowStorage = pointer;
	
	shadowPolyhedron = reinterpret_cast<Polyhedron *>(pointer);
	pointer += sizeof(Polyhedron);
	
	extrusionVertexArray = reinterpret_cast<Vector4D *>(pointer);
	pointer += sizeof(Vector4D) * 4 * edgeCount;
	
	frontEndcapTriangleArray = reinterpret_cast<Triangle *>(pointer);
	pointer += sizeof(Triangle) * faceCount;
	
	return (reinterpret_cast<bool *>(pointer));
}

void StencilData::Deactivate(void)
{
	delete[] shadowStorage;
	shadowStorage = nullptr;
}

void StencilData::CalculateInfiniteShadowBounds(const InfiniteLight *light)
{
	const ShadowSpace *shadowSpace = light->GetConnectedShadowSpace();
	if (shadowSpace)
	{
		Antivector4D	spacePlane[6];
		Polyhedron		tempPolyhedron;
		
		const Vector3D& lightDirection = light->GetWorldTransform()[2];
		
		const BoundingSphere *sphere = shadowGeometry->GetBoundingSphere();
		float r = sphere->GetRadius();
		
		Vector3D v1 = Math::CreatePerpendicular(lightDirection);
		v1 *= r * InverseMag(v1);
		Vector3D v2 = lightDirection % v1;

		Point3D p = sphere->GetCenter() + lightDirection * r;
		Vector3D w1 = v1 + v2;
		Vector3D w2 = v1 - v2;
		
		shadowPolyhedron->vertexCount = 8;
		shadowPolyhedron->edgeCount = 12;
		shadowPolyhedron->faceCount = 6;
		
		shadowPolyhedron->vertex[0] = p + w1;
		shadowPolyhedron->vertex[1] = p - w2;
		shadowPolyhedron->vertex[2] = p - w1;
		shadowPolyhedron->vertex[3] = p + w2; 
		
		Vector3D dp = lightDirection * (shadowSpace->GetBoundingSphere()->GetRadius() * 2.0F + r); 
		shadowPolyhedron->vertex[4] = shadowPolyhedron->vertex[0] - dp; 
		shadowPolyhedron->vertex[5] = shadowPolyhedron->vertex[1] - dp; 
		shadowPolyhedron->vertex[6] = shadowPolyhedron->vertex[2] - dp;
		shadowPolyhedron->vertex[7] = shadowPolyhedron->vertex[3] - dp; 
		
		shadowPolyhedron->edge[0].vertexIndex[0] = 0;
		shadowPolyhedron->edge[0].vertexIndex[1] = 1;
		shadowPolyhedron->edge[0].faceIndex[0] = 0; 
		shadowPolyhedron->edge[0].faceIndex[1] = 2;
		
		shadowPolyhedron->edge[1].vertexIndex[0] = 1;
		shadowPolyhedron->edge[1].vertexIndex[1] = 2; 
		shadowPolyhedron->edge[1].faceIndex[0] = 0;
		shadowPolyhedron->edge[1].faceIndex[1] = 3;
		
		shadowPolyhedron->edge[2].vertexIndex[0] = 2;
		shadowPolyhedron->edge[2].vertexIndex[1] = 3;
		shadowPolyhedron->edge[2].faceIndex[0] = 0;
		shadowPolyhedron->edge[2].faceIndex[1] = 4;
		
		shadowPolyhedron->edge[3].vertexIndex[0] = 3;
		shadowPolyhedron->edge[3].vertexIndex[1] = 0;
		shadowPolyhedron->edge[3].faceIndex[0] = 0;
		shadowPolyhedron->edge[3].faceIndex[1] = 5;
		
		shadowPolyhedron->edge[4].vertexIndex[0] = 4;
		shadowPolyhedron->edge[4].vertexIndex[1] = 5;
		shadowPolyhedron->edge[4].faceIndex[0] = 2;
		shadowPolyhedron->edge[4].faceIndex[1] = 1;
		
		shadowPolyhedron->edge[5].vertexIndex[0] = 5;
		shadowPolyhedron->edge[5].vertexIndex[1] = 6;
		shadowPolyhedron->edge[5].faceIndex[0] = 3;
		shadowPolyhedron->edge[5].faceIndex[1] = 1;
		
		shadowPolyhedron->edge[6].vertexIndex[0] = 6;
		shadowPolyhedron->edge[6].vertexIndex[1] = 7;
		shadowPolyhedron->edge[6].faceIndex[0] = 4;
		shadowPolyhedron->edge[6].faceIndex[1] = 1;
		
		shadowPolyhedron->edge[7].vertexIndex[0] = 7;
		shadowPolyhedron->edge[7].vertexIndex[1] = 4;
		shadowPolyhedron->edge[7].faceIndex[0] = 5;
		shadowPolyhedron->edge[7].faceIndex[1] = 1;
		
		shadowPolyhedron->edge[8].vertexIndex[0] = 0;
		shadowPolyhedron->edge[8].vertexIndex[1] = 4;
		shadowPolyhedron->edge[8].faceIndex[0] = 2;
		shadowPolyhedron->edge[8].faceIndex[1] = 5;
		
		shadowPolyhedron->edge[9].vertexIndex[0] = 1;
		shadowPolyhedron->edge[9].vertexIndex[1] = 5;
		shadowPolyhedron->edge[9].faceIndex[0] = 3;
		shadowPolyhedron->edge[9].faceIndex[1] = 2;
		
		shadowPolyhedron->edge[10].vertexIndex[0] = 2;
		shadowPolyhedron->edge[10].vertexIndex[1] = 6;
		shadowPolyhedron->edge[10].faceIndex[0] = 4;
		shadowPolyhedron->edge[10].faceIndex[1] = 3;
		
		shadowPolyhedron->edge[11].vertexIndex[0] = 3;
		shadowPolyhedron->edge[11].vertexIndex[1] = 7;
		shadowPolyhedron->edge[11].faceIndex[0] = 5;
		shadowPolyhedron->edge[11].faceIndex[1] = 4;
		
		shadowPolyhedron->face[0].edgeCount = 4;
		shadowPolyhedron->face[0].edgeIndex[0] = 0;
		shadowPolyhedron->face[0].edgeIndex[1] = 1;
		shadowPolyhedron->face[0].edgeIndex[2] = 2;
		shadowPolyhedron->face[0].edgeIndex[3] = 3;
		
		shadowPolyhedron->face[1].edgeCount = 4;
		shadowPolyhedron->face[1].edgeIndex[0] = 7;
		shadowPolyhedron->face[1].edgeIndex[1] = 6;
		shadowPolyhedron->face[1].edgeIndex[2] = 5;
		shadowPolyhedron->face[1].edgeIndex[3] = 4;
		
		shadowPolyhedron->face[2].edgeCount = 4;
		shadowPolyhedron->face[2].edgeIndex[0] = 0;
		shadowPolyhedron->face[2].edgeIndex[1] = 8;
		shadowPolyhedron->face[2].edgeIndex[2] = 4;
		shadowPolyhedron->face[2].edgeIndex[3] = 9;
		
		shadowPolyhedron->face[3].edgeCount = 4;
		shadowPolyhedron->face[3].edgeIndex[0] = 1;
		shadowPolyhedron->face[3].edgeIndex[1] = 9;
		shadowPolyhedron->face[3].edgeIndex[2] = 5;
		shadowPolyhedron->face[3].edgeIndex[3] = 10;
		
		shadowPolyhedron->face[4].edgeCount = 4;
		shadowPolyhedron->face[4].edgeIndex[0] = 2;
		shadowPolyhedron->face[4].edgeIndex[1] = 10;
		shadowPolyhedron->face[4].edgeIndex[2] = 6;
		shadowPolyhedron->face[4].edgeIndex[3] = 11;
		
		shadowPolyhedron->face[5].edgeCount = 4;
		shadowPolyhedron->face[5].edgeIndex[0] = 3;
		shadowPolyhedron->face[5].edgeIndex[1] = 11;
		shadowPolyhedron->face[5].edgeIndex[2] = 7;
		shadowPolyhedron->face[5].edgeIndex[3] = 8;
		
		const Point3D *vertex = shadowPolyhedron->vertex;
		
		shadowPolyhedron->face[0].plane.Set(lightDirection, vertex[0]);
		shadowPolyhedron->face[1].plane.Set(-lightDirection, vertex[4]);
		shadowPolyhedron->face[2].plane.SetStandard(vertex[0], vertex[4], vertex[5]);
		shadowPolyhedron->face[3].plane.SetStandard(vertex[1], vertex[5], vertex[6]);
		shadowPolyhedron->face[4].plane.SetStandard(vertex[2], vertex[6], vertex[7]);
		shadowPolyhedron->face[5].plane.SetStandard(vertex[3], vertex[7], vertex[4]);
		
		const Transform4D& transform = shadowSpace->GetWorldTransform();
		const Point3D& position = shadowSpace->GetWorldPosition();
		const Vector3D& spaceSize = shadowSpace->GetObject()->GetBoxSize();
		
		float d = transform[0] * position;
		spacePlane[0].Set(transform[0], -d);
		spacePlane[1].Set(-transform[0], d + spaceSize.x);
		
		d = transform[1] * position;
		spacePlane[2].Set(transform[1], -d);
		spacePlane[3].Set(-transform[1], d + spaceSize.y);
		
		d = transform[2] * position;
		spacePlane[4].Set(transform[2], -d);
		spacePlane[5].Set(-transform[2], d + spaceSize.z);
		
		if ((!Math::ClipPolyhedronAgainstPlane(shadowPolyhedron, spacePlane[0], &tempPolyhedron))
			|| (!Math::ClipPolyhedronAgainstPlane(&tempPolyhedron, spacePlane[1], shadowPolyhedron))
			|| (!Math::ClipPolyhedronAgainstPlane(shadowPolyhedron, spacePlane[2], &tempPolyhedron))
			|| (!Math::ClipPolyhedronAgainstPlane(&tempPolyhedron, spacePlane[3], shadowPolyhedron))
			|| (!Math::ClipPolyhedronAgainstPlane(shadowPolyhedron, spacePlane[4], &tempPolyhedron))
			|| (!Math::ClipPolyhedronAgainstPlane(&tempPolyhedron, spacePlane[5], shadowPolyhedron)))
		{
			shadowPolyhedron->edgeCount = 0;
		}
	}
	else
	{
		shadowPolyhedron->edgeCount = 0;
	}
}

void StencilData::CalculatePointShadowBounds(const PointLight *light)
{
	const BoundingSphere *sphere = shadowGeometry->GetBoundingSphere();
	
	const Point3D& lightPosition = light->GetWorldPosition();
	Vector3D dp = sphere->GetCenter() - lightPosition;
	
	float m = dp * dp;
	float r = sphere->GetRadius();
	float f = r + 0.125F;
	if (m > f * f)
	{
		float t = InverseSqrt(m);
		float d = m * t;
		float s = r * Sqrt((d - r) / (d + r));
		
		Vector3D q = dp * ((d - r) * t);
		Vector3D v1 = Math::CreatePerpendicular(dp);
		v1 *= s * InverseMag(v1);
		Vector3D v2 = v1 % dp * t;
		
		Vector3D w1 = v1 + v2;
		Vector3D w2 = v1 - v2;
		
		Vector3D d1 = q + w1;
		Vector3D d2 = q - w2;
		Vector3D d3 = q - w1;
		Vector3D d4 = q + w2;
		
		shadowPolyhedron->vertexCount = 8;
		shadowPolyhedron->edgeCount = 12;
		shadowPolyhedron->faceCount = 6;
		
		shadowPolyhedron->vertex[0] = lightPosition + d1;
		shadowPolyhedron->vertex[1] = lightPosition + d2;
		shadowPolyhedron->vertex[2] = lightPosition + d3;
		shadowPolyhedron->vertex[3] = lightPosition + d4;
		
		float f = light->GetObject()->GetLightRange() / (d - r);
		shadowPolyhedron->vertex[4] = lightPosition + d1 * f;
		shadowPolyhedron->vertex[5] = lightPosition + d2 * f;
		shadowPolyhedron->vertex[6] = lightPosition + d3 * f;
		shadowPolyhedron->vertex[7] = lightPosition + d4 * f;
		
		shadowPolyhedron->edge[0].vertexIndex[0] = 0;
		shadowPolyhedron->edge[0].vertexIndex[1] = 1;
		shadowPolyhedron->edge[0].faceIndex[0] = 0;
		shadowPolyhedron->edge[0].faceIndex[1] = 2;
		
		shadowPolyhedron->edge[1].vertexIndex[0] = 1;
		shadowPolyhedron->edge[1].vertexIndex[1] = 2;
		shadowPolyhedron->edge[1].faceIndex[0] = 0;
		shadowPolyhedron->edge[1].faceIndex[1] = 3;
		
		shadowPolyhedron->edge[2].vertexIndex[0] = 2;
		shadowPolyhedron->edge[2].vertexIndex[1] = 3;
		shadowPolyhedron->edge[2].faceIndex[0] = 0;
		shadowPolyhedron->edge[2].faceIndex[1] = 4;
		
		shadowPolyhedron->edge[3].vertexIndex[0] = 3;
		shadowPolyhedron->edge[3].vertexIndex[1] = 0;
		shadowPolyhedron->edge[3].faceIndex[0] = 0;
		shadowPolyhedron->edge[3].faceIndex[1] = 5;
		
		shadowPolyhedron->edge[4].vertexIndex[0] = 4;
		shadowPolyhedron->edge[4].vertexIndex[1] = 5;
		shadowPolyhedron->edge[4].faceIndex[0] = 2;
		shadowPolyhedron->edge[4].faceIndex[1] = 1;
		
		shadowPolyhedron->edge[5].vertexIndex[0] = 5;
		shadowPolyhedron->edge[5].vertexIndex[1] = 6;
		shadowPolyhedron->edge[5].faceIndex[0] = 3;
		shadowPolyhedron->edge[5].faceIndex[1] = 1;
		
		shadowPolyhedron->edge[6].vertexIndex[0] = 6;
		shadowPolyhedron->edge[6].vertexIndex[1] = 7;
		shadowPolyhedron->edge[6].faceIndex[0] = 4;
		shadowPolyhedron->edge[6].faceIndex[1] = 1;
		
		shadowPolyhedron->edge[7].vertexIndex[0] = 7;
		shadowPolyhedron->edge[7].vertexIndex[1] = 4;
		shadowPolyhedron->edge[7].faceIndex[0] = 5;
		shadowPolyhedron->edge[7].faceIndex[1] = 1;
		
		shadowPolyhedron->edge[8].vertexIndex[0] = 0;
		shadowPolyhedron->edge[8].vertexIndex[1] = 4;
		shadowPolyhedron->edge[8].faceIndex[0] = 2;
		shadowPolyhedron->edge[8].faceIndex[1] = 5;
		
		shadowPolyhedron->edge[9].vertexIndex[0] = 1;
		shadowPolyhedron->edge[9].vertexIndex[1] = 5;
		shadowPolyhedron->edge[9].faceIndex[0] = 3;
		shadowPolyhedron->edge[9].faceIndex[1] = 2;
		
		shadowPolyhedron->edge[10].vertexIndex[0] = 2;
		shadowPolyhedron->edge[10].vertexIndex[1] = 6;
		shadowPolyhedron->edge[10].faceIndex[0] = 4;
		shadowPolyhedron->edge[10].faceIndex[1] = 3;
		
		shadowPolyhedron->edge[11].vertexIndex[0] = 3;
		shadowPolyhedron->edge[11].vertexIndex[1] = 7;
		shadowPolyhedron->edge[11].faceIndex[0] = 5;
		shadowPolyhedron->edge[11].faceIndex[1] = 4;
		
		shadowPolyhedron->face[0].edgeCount = 4;
		shadowPolyhedron->face[0].edgeIndex[0] = 0;
		shadowPolyhedron->face[0].edgeIndex[1] = 1;
		shadowPolyhedron->face[0].edgeIndex[2] = 2;
		shadowPolyhedron->face[0].edgeIndex[3] = 3;
		
		shadowPolyhedron->face[1].edgeCount = 4;
		shadowPolyhedron->face[1].edgeIndex[0] = 7;
		shadowPolyhedron->face[1].edgeIndex[1] = 6;
		shadowPolyhedron->face[1].edgeIndex[2] = 5;
		shadowPolyhedron->face[1].edgeIndex[3] = 4;
		
		shadowPolyhedron->face[2].edgeCount = 4;
		shadowPolyhedron->face[2].edgeIndex[0] = 0;
		shadowPolyhedron->face[2].edgeIndex[1] = 8;
		shadowPolyhedron->face[2].edgeIndex[2] = 4;
		shadowPolyhedron->face[2].edgeIndex[3] = 9;
		
		shadowPolyhedron->face[3].edgeCount = 4;
		shadowPolyhedron->face[3].edgeIndex[0] = 1;
		shadowPolyhedron->face[3].edgeIndex[1] = 9;
		shadowPolyhedron->face[3].edgeIndex[2] = 5;
		shadowPolyhedron->face[3].edgeIndex[3] = 10;
		
		shadowPolyhedron->face[4].edgeCount = 4;
		shadowPolyhedron->face[4].edgeIndex[0] = 2;
		shadowPolyhedron->face[4].edgeIndex[1] = 10;
		shadowPolyhedron->face[4].edgeIndex[2] = 6;
		shadowPolyhedron->face[4].edgeIndex[3] = 11;
		
		shadowPolyhedron->face[5].edgeCount = 4;
		shadowPolyhedron->face[5].edgeIndex[0] = 3;
		shadowPolyhedron->face[5].edgeIndex[1] = 11;
		shadowPolyhedron->face[5].edgeIndex[2] = 7;
		shadowPolyhedron->face[5].edgeIndex[3] = 8;
		
		const Point3D *vertex = shadowPolyhedron->vertex;
		
		Vector3D n = dp * t;
		shadowPolyhedron->face[0].plane.Set(-n, vertex[0]);
		shadowPolyhedron->face[1].plane.Set(n, vertex[4]);
		shadowPolyhedron->face[2].plane.SetStandard(vertex[0], vertex[4], vertex[5]);
		shadowPolyhedron->face[3].plane.SetStandard(vertex[1], vertex[5], vertex[6]);
		shadowPolyhedron->face[4].plane.SetStandard(vertex[2], vertex[6], vertex[7]);
		shadowPolyhedron->face[5].plane.SetStandard(vertex[3], vertex[7], vertex[4]);
		
		const ShadowSpace *shadowSpace = light->GetConnectedShadowSpace();
		if (shadowSpace)
		{
			Antivector4D	spacePlane[6];
			Polyhedron		tempPolyhedron;
			
			const Transform4D& transform = shadowSpace->GetWorldTransform();
			const Point3D& position = shadowSpace->GetWorldPosition();
			const Vector3D& spaceSize = shadowSpace->GetObject()->GetBoxSize();
			
			float w = transform[0] * position;
			spacePlane[0].Set(transform[0], -w);
			spacePlane[1].Set(-transform[0], w + spaceSize.x);
			
			w = transform[1] * position;
			spacePlane[2].Set(transform[1], -w);
			spacePlane[3].Set(-transform[1], w + spaceSize.y);
			
			w = transform[2] * position;
			spacePlane[4].Set(transform[2], -w);
			spacePlane[5].Set(-transform[2], w + spaceSize.z);
			
			if ((!Math::ClipPolyhedronAgainstPlane(shadowPolyhedron, spacePlane[0], &tempPolyhedron))
				|| (!Math::ClipPolyhedronAgainstPlane(&tempPolyhedron, spacePlane[1], shadowPolyhedron))
				|| (!Math::ClipPolyhedronAgainstPlane(shadowPolyhedron, spacePlane[2], &tempPolyhedron))
				|| (!Math::ClipPolyhedronAgainstPlane(&tempPolyhedron, spacePlane[3], shadowPolyhedron))
				|| (!Math::ClipPolyhedronAgainstPlane(shadowPolyhedron, spacePlane[4], &tempPolyhedron))
				|| (!Math::ClipPolyhedronAgainstPlane(&tempPolyhedron, spacePlane[5], shadowPolyhedron)))
			{
				shadowPolyhedron->edgeCount = 0;
			}
		}
	}
	else
	{
		shadowPolyhedron->edgeCount = 0;
	}
}


StencilVolume::StencilVolume(Geometry *geometry, Light *light, Link<StencilVolume> *link) : StencilData(geometry)
{
	targetLight = light;
	light->AddStencilVolume(this);
	
	*link = this;
	
	extrusionDetailLevel = -1;
	endcapDetailLevel = -1;
	Activate();
}

StencilVolume::~StencilVolume()
{
}

// ZYURVUR
