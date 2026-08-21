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
#include "C4Cameras.h"


using namespace C4;


namespace C4
{
	template <> Heap Memory<Region>::heap("Region", 16384, kHeapMutexless);
	template class Memory<Region>;
}


C4::Region::Region()
{
}

C4::Region::~Region()
{
}

bool C4::Region::BoxVisible(const Box3D& box) const
{
	#if C4SIMD
	
		register const float4 sign_bit = SimdGetNegativeZero();
		float4 min = SimdLoadUnaligned(&box.min.x);
		float4 max = SimdLoadUnaligned(&box.max.x);
		
		register const float4 half = SimdLoadConstant<0x3F000000>();
		float4 center = SimdMul(SimdAdd(min, max), half);
		float4 size = SimdMul(SimdSub(max, min), half);
		
		int32 count = planeCount;
		for (machine a = 0; a < count; a++)
		{
			float4 plane = SimdLoadUnaligned(&regionPlane[a].x);
			
			float4 f = SimdOr(SimdMul(plane, size), sign_bit);
			f = SimdAddScalar(SimdAddScalar(f, SimdSmearY(f)), SimdSmearZ(f));
			
			if (SimdCmpltScalar(SimdPlaneWedgePoint3D(plane, center), f)) return (false);
		}
	
	#else
	
		Point3D center = box.GetCenter();
		Vector3D size = box.GetSize() * 0.5F;
		
		int32 count = planeCount;
		for (machine a = 0; a < count; a++)
		{
			const Antivector4D& plane = regionPlane[a];
			float reff = Fnabs(plane.x * size.x) + Fnabs(plane.y * size.y) + Fnabs(plane.z * size.z);
			if ((plane ^ center) < reff) return (false);
		}
	
	#endif
	
	return (true);
}

bool C4::Region::BoxVisible(const Point3D& center, const Vector3D *axis) const
{
	#if C4SIMD
	
		register const float4 sign_bit = SimdGetNegativeZero();
		float4 c = SimdLoadUnaligned(&center.x);
		
		int32 count = planeCount;
		for (machine a = 0; a < count; a++)
		{
			float4 plane = SimdLoadUnaligned(&regionPlane[a].x);
			
			float4 a1 = SimdDot3D(plane, SimdLoadUnaligned(&axis[0].x));
			float4 a2 = SimdDot3D(plane, SimdLoadUnaligned(&axis[1].x));
			float4 a3 = SimdDot3D(plane, SimdLoadUnaligned(&axis[2].x));
			
			a1 = SimdOr(sign_bit, a1);
			a2 = SimdOr(sign_bit, a2);
			a3 = SimdOr(sign_bit, a3);
			
			a1 = SimdAddScalar(a1, a2);
			a1 = SimdAddScalar(a1, a3);
			
			if (SimdCmpltScalar(SimdPlaneWedgePoint3D(plane, c), a1)) return (false);
		}
	
	#else
	
		int32 count = planeCount;
		for (machine a = 0; a < count; a++)
		{
			const Antivector4D& plane = regionPlane[a];
			float reff = Fnabs(plane ^ axis[0]) + Fnabs(plane ^ axis[1]) + Fnabs(plane ^ axis[2]);
			if ((plane ^ center) < reff) return (false);
		}
	
	#endif
	
	return (true); 
}
 
bool C4::Region::BoxVisible(const Point3D& center, const Vector3D& size) const 
{ 
	#if C4SIMD
	 
		register const float4 sign_bit = SimdGetNegativeZero();
		float4 c = SimdLoadUnaligned(&center.x);
		float4 s = SimdLoadUnaligned(&size.x);
		 
		int32 count = planeCount;
		for (machine a = 0; a < count; a++)
		{
			float4 plane = SimdLoadUnaligned(&regionPlane[a].x); 
			
			float4 f = SimdOr(SimdMul(plane, s), sign_bit);
			f = SimdAddScalar(SimdAddScalar(f, SimdSmearY(f)), SimdSmearZ(f));
			
			if (SimdCmpltScalar(SimdPlaneWedgePoint3D(plane, c), f)) return (false);
		}
	
	#else
	
		int32 count = planeCount;
		for (machine a = 0; a < count; a++)
		{
			const Antivector4D& plane = regionPlane[a];
			float reff = Fnabs(plane.x * size.x) + Fnabs(plane.y * size.y) + Fnabs(plane.z * size.z);
			if ((plane ^ center) < reff) return (false);
		}
	
	#endif
	
	return (true);
}

bool C4::Region::DirectionVisible(const Vector3D& direction, float radius) const
{
	radius = -radius;
	int32 count = planeCount;
	for (machine a = 0; a < count; a++)
	{
		if ((regionPlane[a] ^ direction) < radius) return (false);
	}
	
	return (true);
}

bool C4::Region::PolygonVisible(int32 vertexCount, const Point3D *vertex) const
{
	#if C4SIMD
	
		register const float4 zero = SimdGetZero();
		
		int32 count = planeCount;
		for (machine a = 0; a < count; a++)
		{
			float4 plane = SimdLoadUnaligned(&regionPlane[a].x);
			for (machine b = 0; b < vertexCount; b++)
			{
				float4 d = SimdPlaneWedgePoint3D(plane, SimdLoadUnaligned(&vertex[b].x));
				if (SimdCmpgtScalar(d, zero)) goto next;
			}
			
			return (false);
			next:;
		}
	
	#else
	
		int32 count = planeCount;
		for (machine a = 0; a < count; a++)
		{
			const Antivector4D& plane = regionPlane[a];
			for (machine b = 0; b < vertexCount; b++)
			{
				if ((plane ^ vertex[b]) > 0.0F) goto next;
			}
			
			return (false);
			next:;
		}
	
	#endif
	
	return (true);
}

bool C4::Region::SphereVisible(const Point3D& center, float radius) const
{
	#if C4SIMD
	
		float4 r = SimdXor(SimdLoadScalar(&radius), SimdGetNegativeZero());
		float4 c = SimdLoadUnaligned(&center.x);
		
		int32 count = planeCount;
		for (machine a = 0; a < count; a++)
		{
			float4 plane = SimdLoadUnaligned(&regionPlane[a].x);
			float4 d = SimdPlaneWedgePoint3D(plane, c);
			if (SimdCmpltScalar(d, r)) return (false);
		}
	
	#else
	
		radius = -radius;
		int32 count = planeCount;
		for (machine a = 0; a < count; a++)
		{
			if ((regionPlane[a] ^ center) < radius) return (false);
		}
	
	#endif
	
	return (true);
}

bool C4::Region::EllipsoidVisible(const Point3D& center, const Vector3D *axis) const
{
	#if C4SIMD
	
		float4 c = SimdLoadUnaligned(&center.x);
		
		int32 count = planeCount;
		for (machine a = 0; a < count; a++)
		{
			float4 plane = SimdLoadUnaligned(&regionPlane[a].x);
			float4 R_dot_N = SimdDot3D(plane, SimdLoadUnaligned(&axis[0].x));
			float4 S_dot_N = SimdDot3D(plane, SimdLoadUnaligned(&axis[1].x));
			float4 T_dot_N = SimdDot3D(plane, SimdLoadUnaligned(&axis[2].x));
			float4 d2 = SimdMulScalar(R_dot_N, R_dot_N);
			d2 = SimdMaddScalar(S_dot_N, S_dot_N, d2);
			d2 = SimdMaddScalar(T_dot_N, T_dot_N, d2);
			
			float4 reff = SimdNegate(SimdSqrtScalar(d2));
			if (SimdCmpltScalar(SimdPlaneWedgePoint3D(plane, c), reff)) return (false);
		}
	
	#else
	
		int32 count = planeCount;
		for (machine a = 0; a < count; a++)
		{
			const Antivector4D& plane = regionPlane[a];
			float R_dot_N = plane ^ axis[0];
			float S_dot_N = plane ^ axis[1];
			float T_dot_N = plane ^ axis[2];
			float reff = -Sqrt(R_dot_N * R_dot_N + S_dot_N * S_dot_N + T_dot_N * T_dot_N);
			if ((plane ^ center) < reff) return (false);
		}
	
	#endif
	
	return (true);
}

bool C4::Region::CylinderVisible(const Point3D& p1, const Point3D& p2, float radius) const
{
	#if C4SIMD
	
		float4 q1 = SimdLoadUnaligned(&p1.x);
		float4 q2 = SimdLoadUnaligned(&p2.x);
		
		float4 dp = SimdSub(q2, q1);
		float4 m = SimdInverseSqrtScalar(SimdDot3D(dp, dp));
		dp = SimdMul(dp, SimdSmearX(m));
		
		register const float4 zero = SimdGetZero();
		register const float4 one = SimdLoadConstant<0x3F800000>();
		
		float4 r = SimdLoadScalar(&radius);
		
		int32 count = planeCount;
		for (machine a = 0; a < count; a++)
		{
			float4 plane = SimdLoadUnaligned(&regionPlane[a].x);
			
			float4 d1 = SimdDot3D(plane, q1);
			float4 d2 = SimdDot3D(plane, q2);
			
			float4 s = SimdDot3D(plane, dp);
			float4 reff = SimdMulScalar(r, SimdSqrtScalar(SimdMaxScalar(SimdNmsubScalar(s, s, one), zero)));
			
			float4 f = SimdSubScalar(SimdNegate(SimdSmearW(plane)), reff);
			if (SimdCmpltScalar(d1, f))
			{
				if (SimdCmpltScalar(d2, f)) return (false);
				
				float4 t = SimdDivScalar(SimdSubScalar(f, d1), SimdSubScalar(d2, d1));
				q1 = SimdMadd(SimdSmearX(t), SimdSub(q2, q1), q1);
			}
			else if (SimdCmpltScalar(d2, f))
			{
				float4 t = SimdDivScalar(SimdSubScalar(f, d1), SimdSubScalar(d2, d1));
				q2 = SimdMadd(SimdSmearX(t), SimdSub(q2, q1), q1);
			}
		}
	
	#else
	
		Point3D q1 = p1;
		Point3D q2 = p2;
		
		Vector3D dp = q2 - q1;
		dp.Normalize();
		
		int32 count = planeCount;
		for (machine a = 0; a < count; a++)
		{
			const Antivector4D& plane = regionPlane[a];
			
			float d1 = plane ^ q1.GetVector3D();
			float d2 = plane ^ q2.GetVector3D();
			
			float s = plane ^ dp;
			float reff = radius * Sqrt(FmaxZero(1.0F - s * s));
			
			float f = -plane.w - reff;
			if (d1 < f)
			{
				if (d2 < f) return (false);
				
				float t = (f - d1) / (d2 - d1);
				q1 += t * (q2 - q1);
			}
			else if (d2 < f)
			{
				float t = (f - d1) / (d2 - d1);
				q2 = q1 + t * (q2 - q1);
			}
		}
	
	#endif
	
	return (true);
}

bool C4::Region::BoxOccluded(const Box3D& box) const
{
	#if C4SIMD
	
		register const float4 sign_bit = SimdGetNegativeZero();
		float4 min = SimdLoadUnaligned(&box.min.x);
		float4 max = SimdLoadUnaligned(&box.max.x);
		
		register const float4 half = SimdLoadConstant<0x3F000000>();
		float4 center = SimdMul(SimdAdd(min, max), half);
		float4 size = SimdMul(SimdSub(max, min), half);
		
		int32 count = planeCount;
		for (machine a = 0; a < count; a++)
		{
			float4 plane = SimdLoadUnaligned(&regionPlane[a].x);
			
			float4 f = SimdAndc(SimdMul(plane, size), sign_bit);
			f = SimdAddScalar(SimdAddScalar(f, SimdSmearY(f)), SimdSmearZ(f));
			
			if (SimdCmpltScalar(SimdPlaneWedgePoint3D(plane, center), f)) return (false);
		}
	
	#else
	
		Point3D center = box.GetCenter();
		Vector3D size = box.GetSize() * 0.5F;
		
		int32 count = planeCount;
		for (machine a = 0; a < count; a++)
		{
			const Antivector4D& plane = regionPlane[a];
			float reff = Fabs(plane.x * size.x) + Fabs(plane.y * size.y) + Fabs(plane.z * size.z);
			if ((plane ^ center) < reff) return (false);
		}
	
	#endif
	
	return (true);
}

bool C4::Region::BoxOccluded(const Point3D& center, const Vector3D *axis) const
{
	#if C4SIMD
	
		register const float4 sign_bit = SimdGetNegativeZero();
		float4 c = SimdLoadUnaligned(&center.x);
		
		int32 count = planeCount;
		for (machine a = 0; a < count; a++)
		{
			float4 plane = SimdLoadUnaligned(&regionPlane[a].x);
			
			float4 a1 = SimdDot3D(plane, SimdLoadUnaligned(&axis[0].x));
			float4 a2 = SimdDot3D(plane, SimdLoadUnaligned(&axis[1].x));
			float4 a3 = SimdDot3D(plane, SimdLoadUnaligned(&axis[2].x));
			
			a1 = SimdAndc(a1, sign_bit);
			a2 = SimdAndc(a2, sign_bit);
			a3 = SimdAndc(a3, sign_bit);
			
			a1 = SimdAddScalar(a1, a2);
			a1 = SimdAddScalar(a1, a3);
			
			if (SimdCmpltScalar(SimdPlaneWedgePoint3D(plane, c), a1)) return (false);
		}
	
	#else
	
		int32 count = planeCount;
		for (machine a = 0; a < count; a++)
		{
			const Antivector4D& plane = regionPlane[a];
			float reff = Fabs(plane ^ axis[0]) + Fabs(plane ^ axis[1]) + Fabs(plane ^ axis[2]);
			if ((plane ^ center) < reff) return (false);
		}
	
	#endif
	
	return (true);
}

bool C4::Region::BoxOccluded(const Point3D& center, const Vector3D& size) const
{
	#if C4SIMD
	
		register const float4 sign_bit = SimdGetNegativeZero();
		float4 c = SimdLoadUnaligned(&center.x);
		float4 s = SimdLoadUnaligned(&size.x);
		
		int32 count = planeCount;
		for (machine a = 0; a < count; a++)
		{
			float4 plane = SimdLoadUnaligned(&regionPlane[a].x);
			
			float4 f = SimdAndc(SimdMul(plane, s), sign_bit);
			f = SimdAddScalar(SimdAddScalar(f, SimdSmearY(f)), SimdSmearZ(f));
			
			if (SimdCmpltScalar(SimdPlaneWedgePoint3D(plane, c), f)) return (false);
		}
	
	#else
	
		int32 count = planeCount;
		for (machine a = 0; a < count; a++)
		{
			const Antivector4D& plane = regionPlane[a];
			float reff = Fabs(plane.x * size.x) + Fabs(plane.y * size.y) + Fabs(plane.z * size.z);
			if ((plane ^ center) < reff) return (false);
		}
	
	#endif
	
	return (true);
}

bool C4::Region::DirectionOccluded(const Vector3D& direction, float radius) const
{
	int32 count = planeCount;
	for (machine a = 0; a < count; a++)
	{
		if ((regionPlane[a] ^ direction) < radius) return (false);
	}
	
	return (true);
}

bool C4::Region::PolygonOccluded(int32 vertexCount, const Point3D *vertex) const
{
	#if C4SIMD
	
		register const float4 zero = SimdGetZero();
		
		int32 count = planeCount;
		for (machine a = 0; a < count; a++)
		{
			float4 plane = SimdLoadUnaligned(&regionPlane[a].x);
			for (machine b = 0; b < vertexCount; b++)
			{
				float4 d = SimdPlaneWedgePoint3D(plane, SimdLoadUnaligned(&vertex[b].x));
				if (SimdCmpltScalar(d, zero)) return (false);
			}
		}
	
	#else
	
		int32 count = planeCount;
		for (machine a = 0; a < count; a++)
		{
			const Antivector4D& plane = regionPlane[a];
			for (machine b = 0; b < vertexCount; b++)
			{
				if ((plane ^ vertex[b]) < 0.0F) return (false);
			}
		}
	
	#endif
	
	return (true);
}

bool C4::Region::SphereOccluded(const Point3D& center, float radius) const
{
	#if C4SIMD
	
		float4 r = SimdLoadScalar(&radius);
		float4 c = SimdLoadUnaligned(&center.x);
		
		int32 count = planeCount;
		for (machine a = 0; a < count; a++)
		{
			float4 plane = SimdLoadUnaligned(&regionPlane[a].x);
			if (SimdCmpltScalar(SimdPlaneWedgePoint3D(plane, c), r)) return (false);
		}
	
	#else
	
		int32 count = planeCount;
		for (machine a = 0; a < count; a++)
		{
			if ((regionPlane[a] ^ center) < radius) return (false);
		}
	
	#endif
	
	return (true);
}

bool C4::Region::EllipsoidOccluded(const Point3D& center, const Vector3D *axis) const
{
	#if C4SIMD
	
		float4 c = SimdLoadUnaligned(&center.x);
		
		int32 count = planeCount;
		for (machine a = 0; a < count; a++)
		{
			float4 plane = SimdLoadUnaligned(&regionPlane[a].x);
			float4 R_dot_N = SimdDot3D(plane, SimdLoadUnaligned(&axis[0].x));
			float4 S_dot_N = SimdDot3D(plane, SimdLoadUnaligned(&axis[1].x));
			float4 T_dot_N = SimdDot3D(plane, SimdLoadUnaligned(&axis[2].x));
			float4 d2 = SimdMul(R_dot_N, R_dot_N);
			d2 = SimdMadd(S_dot_N, S_dot_N, d2);
			d2 = SimdMadd(T_dot_N, T_dot_N, d2);
			
			float4 reff = SimdSqrt(d2);
			if (SimdCmpltScalar(SimdPlaneWedgePoint3D(plane, c), reff)) return (false);
		}
	
	#else
	
		int32 count = planeCount;
		for (machine a = 0; a < count; a++)
		{
			const Antivector4D& plane = regionPlane[a];
			float R_dot_N = plane ^ axis[0];
			float S_dot_N = plane ^ axis[1];
			float T_dot_N = plane ^ axis[2];
			float reff = Sqrt(R_dot_N * R_dot_N + S_dot_N * S_dot_N + T_dot_N * T_dot_N);
			if ((plane ^ center) < reff) return (false);
		}
	
	#endif
	
	return (true);
}

bool C4::Region::CylinderOccluded(const Point3D& p1, const Point3D& p2, float radius) const
{
	#if C4SIMD
	
		float4 q1 = SimdLoadUnaligned(&p1.x);
		float4 q2 = SimdLoadUnaligned(&p2.x);
		
		float4 dp = SimdSub(q2, q1);
		float4 m = SimdInverseSqrtScalar(SimdDot3D(dp, dp));
		dp = SimdMul(dp, SimdSmearX(m));
		
		register const float4 zero = SimdGetZero();
		register const float4 one = SimdLoadConstant<0x3F800000>();
		
		float4 r = SimdLoadScalar(&radius);
		
		int32 count = planeCount;
		for (machine a = 0; a < count; a++)
		{
			float4 plane = SimdLoadUnaligned(&regionPlane[a].x);
			
			float4 s = SimdDot3D(plane, dp);
			float4 reff = SimdMulScalar(r, SimdSqrtScalar(SimdMaxScalar(SimdNmsub(s, s, one), zero)));
			
			if ((SimdCmpltScalar(SimdPlaneWedgePoint3D(plane, q1), reff)) || (SimdCmpltScalar(SimdPlaneWedgePoint3D(plane, q2), reff))) return (false);
		}
	
	#else
	
		Vector3D dp = p2 - p1;
		dp.Normalize();
		
		int32 count = planeCount;
		for (machine a = 0; a < count; a++)
		{
			const Antivector4D& plane = regionPlane[a];
			
			float s = plane ^ dp;
			float reff = radius * Sqrt(FmaxZero(1.0F - s * s));
			if (((plane ^ p1) < reff) || ((plane ^ p2) < reff)) return (false);
		}
	
	#endif
	
	return (true);
}


ShadowRegion::ShadowRegion()
{
	regionLight = nullptr;
}

ShadowRegion::~ShadowRegion()
{
}


ZoneRegion::ZoneRegion(Zone *zone)
{
	regionZone = zone;
}

ZoneRegion::ZoneRegion(Zone *zone, const ZoneRegion *region)
{
	regionZone = zone;
	
	int32 count = region->GetPlaneCount();
	SetPlaneCount(count);
	
	const Antivector4D *p = region->GetPlaneArray();
	Antivector4D *plane = GetPlaneArray();
	for (machine a = 0; a < count; a++) plane[a] = p[a];
}

ZoneRegion::~ZoneRegion()
{
}


CameraRegion::CameraRegion(const Camera *camera, Zone *zone) :
		ZoneRegion(zone),
		impostorArray(0, 256)
{
	regionCamera = camera;
	
	nonlateralPlaneCount = 0;
	portalExcludePlaneCount = 0;
	shadowRegionFlags = 0;
}

CameraRegion::CameraRegion(const Camera *camera, Zone *zone, const CameraRegion *region) :
		ZoneRegion(zone, region),
		impostorArray(0, 256)
{
	regionCamera = camera;
	
	nonlateralPlaneCount = region->nonlateralPlaneCount;
	portalExcludePlaneCount = region->portalExcludePlaneCount;
	shadowRegionFlags = region->shadowRegionFlags;
}

CameraRegion::~CameraRegion()
{
	geometryList.RemoveAll();
	
	Zone *zone = GetZone();
	if (zone)
	{
		World *world = zone->GetWorld();
		for (;;)
		{
			Effect *effect = effectList.First();
			if (!effect) break;
			
			world->AddEffect(effect);
		}
	}
}

void CameraRegion::SetFrustumPortalPlanes(int32 vertexCount, const Point3D *vertex, const Antivector4D& portalPlane)
{
	const Point3D& cameraPosition = regionCamera->GetWorldPosition();
	Antivector4D *plane = GetPlaneArray();
	
	const Point3D *p1 = &vertex[vertexCount - 1];
	for (machine a = 0; a < vertexCount; a++)
	{
		const Point3D *p2 = &vertex[a];
		plane->SetStandard(cameraPosition, *p2, *p1);
		plane++;
		p1 = p2;
	}
	
	plane[0] = -portalPlane;
	
	SetPlaneCount(vertexCount + 1);
	nonlateralPlaneCount = 1;
	shadowRegionFlags = 0x01;
}

void CameraRegion::SetFrustumOcclusionPortalPlanes(int32 vertexCount, const Point3D *vertex, int32 frontPlaneCount, const Antivector4D *frontPlane)
{
	float	distance[2][4];
	
	const float kCullEpsilon = kBoundaryEpsilon * 2.0F;
	
	const Point3D& cameraPosition = regionCamera->GetWorldPosition();
	const Vector3D *frustumNormal = &static_cast<const FrustumCamera *>(regionCamera)->GetFrustumPlaneNormal(0);
	
	Vector3D v1 = vertex[vertexCount - 1] - cameraPosition;
	for (machine k = 0; k < 4; k++) distance[0][k] = frustumNormal[k] * v1;
	
	int32 planeCount = 0;
	Antivector4D *plane = GetPlaneArray();
	
	unsigned_int32 p1 = 0;
	for (machine a = 0; a < vertexCount; a++)
	{
		Vector3D v2 = vertex[a] - cameraPosition;
		Antivector3D occluderNormal = (v2 % v1).Normalize();
		
		bool cull = false;
		unsigned_int32 p2 = p1 ^ 1;
		for (machine k = 0; k < 4; k++)
		{
			const Vector3D& n = frustumNormal[k];
			float d = n * v2;
			distance[p2][k] = d;
			
			if (Fmax(d, distance[p1][k]) < kCullEpsilon)
			{
				cull |= (occluderNormal * n > 0.0F);
			}
		}
		
		if (!cull)
		{
			plane->Set(occluderNormal, cameraPosition);
			planeCount++;
			plane++;
		}
		
		v1 = v2;
		p1 ^= 1;
	}
	
	for (machine a = 0; a < frontPlaneCount; a++) plane[a] = frontPlane[a];
	
	SetPlaneCount(planeCount + frontPlaneCount);
	nonlateralPlaneCount = frontPlaneCount;
	shadowRegionFlags = 0x01;
}

void CameraRegion::SetOrthoPortalPlanes(int32 vertexCount, const Point3D *vertex, const Antivector4D& portalPlane)
{
	const Vector3D& viewDirection = regionCamera->GetWorldTransform()[2];
	Antivector4D *plane = GetPlaneArray();
	
	const Point3D *v1 = &vertex[vertexCount - 1];
	for (machine a = 0; a < vertexCount; a++)
	{
		const Point3D *v2 = &vertex[a];
		Antivector3D occluderNormal = ((*v2 - *v1) % viewDirection).Normalize();
		plane->Set(occluderNormal, *v1);
		plane++;
		v1 = v2;
	}
	
	plane[0] = -portalPlane;
	
	SetPlaneCount(vertexCount + 1);
	nonlateralPlaneCount = 1;
	shadowRegionFlags = 0x01;
}

void CameraRegion::SetOrthoPortalPlanes(int32 vertexCount, const Point3D *vertex, const CameraRegion *rootRegion)
{
	const Vector3D& viewDirection = regionCamera->GetWorldTransform()[2];
	Antivector4D *plane = GetPlaneArray();
	
	const Point3D *v1 = &vertex[vertexCount - 1];
	for (machine a = 0; a < vertexCount; a++)
	{
		const Point3D *v2 = &vertex[a];
		Antivector3D normal = ((*v2 - *v1) % viewDirection).Normalize();
		plane->Set(normal, *v1);
		plane++;
		v1 = v2;
	}
	
	int32 planeCount = rootRegion->GetPlaneCount();
	plane[0] = rootRegion->GetPlaneArray()[planeCount - 2];
	plane[1] = rootRegion->GetPlaneArray()[planeCount - 1];
	
	SetPlaneCount(vertexCount + 2);
	nonlateralPlaneCount = 2;
	shadowRegionFlags = 0x01;
}

bool CameraRegion::AddOppositePlane(void)
{
	Antivector4D	plane;
	
	const Zone *zone = GetZone();
	if (!(zone->GetObject()->GetZoneFlags() & kZoneTransition))
	{
		Region *region = GetFirstSubnode();
		while (region)
		{
			static_cast<CameraRegion *>(region)->AddOppositePlane();
			region = region->Next();
		}
	}
	else
	{
		float distance = 0.0F;
		const Antivector4D *oppositePlane = nullptr;
		const Point3D& cameraPosition = regionCamera->GetWorldPosition();
		
		Region *region = GetFirstSubnode();
		while (region)
		{
			if (static_cast<CameraRegion *>(region)->AddOppositePlane())
			{
				const Antivector4D& p = region->GetPlaneArray()[region->GetPlaneCount() - 1];
				float d = p ^ cameraPosition;
				if (d > distance)
				{
					distance = d;
					oppositePlane = &p;
				}
			}
			
			region = region->Next();
		}
		
		if (oppositePlane)
		{
			int32 count = GetPlaneCount();
			if (count < kMaxRegionPlaneCount)
			{
				GetPlaneArray()[count] = *oppositePlane;
				
				SetPlaneCount(count + 1);
				shadowRegionFlags |= 1 << nonlateralPlaneCount++;
			}
			
			return (true);
		}
		
		region = GetSuperNode();
		if (region)
		{
			Antivector4D	superPlane;
			
			const Zone *superZone = static_cast<ZoneRegion *>(region)->GetZone();
			const Transform4D& superTransform = superZone->GetInverseWorldTransform();
			if (!superZone->GetObject()->CalculateOppositePlane(superTransform * regionCamera->GetWorldTransform()[2], &superPlane)) return (false);
			superPlane = superPlane * superTransform;
			
			const Transform4D& transform = zone->GetInverseWorldTransform();
			if (!zone->GetObject()->CalculateOppositePlane(transform * regionCamera->GetWorldTransform()[2], &plane)) return (false);
			plane = plane * transform;
			
			int32 count = GetPlaneCount();
			if (count < kMaxRegionPlaneCount)
			{
				GetPlaneArray()[count] = ((superPlane ^ cameraPosition) > (plane ^ cameraPosition)) ? superPlane : plane;
				
				SetPlaneCount(count + 1);
				shadowRegionFlags |= 1 << nonlateralPlaneCount++;
			}
			
			return (true);
		}
	}
	
	const Transform4D& transform = zone->GetInverseWorldTransform();
	if (zone->GetObject()->CalculateOppositePlane(transform * regionCamera->GetWorldTransform()[2], &plane))
	{
		int32 count = GetPlaneCount();
		if (count < kMaxRegionPlaneCount)
		{
			GetPlaneArray()[count] = plane * transform;
			
			SetPlaneCount(count + 1);
			shadowRegionFlags |= 1 << nonlateralPlaneCount++;
		}
		
		return (true);
	}
	
	return (false);
}

bool CameraRegion::ContainsInfiniteLight(const Vector3D& direction) const
{
	int32 count = GetPlaneCount() - nonlateralPlaneCount;
	const Antivector4D *plane = GetPlaneArray();
	
	for (machine a = 0; a < count; a++)
	{
		if ((plane[a] ^ direction) < 0.0F) return (false);
	}
	
	return (true);
}

bool CameraRegion::ContainsPointLight(const Point3D& position) const
{
	int32 count = GetPlaneCount() - nonlateralPlaneCount;
	const Antivector4D *plane = GetPlaneArray();
	
	for (machine a = 0; a < count; a++)
	{
		if ((plane[a] ^ position) < 0.0F) return (false);
	}
	
	return (true);
}


LightRegion::LightRegion(Light *light, Zone *zone) : ZoneRegion(zone)
{
	regionLight = light;
	illuminatedPortal = nullptr;
	
	lightRegionFlags = 0;
	boundaryPolygonCount = 0;
}

LightRegion::LightRegion(Light *light, Zone *zone, const LightRegion *region) : ZoneRegion(zone, region)
{
	regionLight = light;
	illuminatedPortal = nullptr;
	
	lightRegionFlags = 0;
	boundaryPolygonCount = 0;
}

LightRegion::~LightRegion()
{
}


SourceRegion::SourceRegion(Source *source, Zone *zone) : ZoneRegion(zone)
{
	regionSource = source;
	primaryRegion = this;
	
	permeatedPortal = nullptr;
	permeatedPosition = source->GetWorldPosition();
	permeatedPathLength = 0.0F;
	
	audiblePosition = source->GetWorldPosition();
	audiblePathLength = 0.0F;
	
	SetPlaneCount(0);
}

SourceRegion::SourceRegion(Source *source, Zone *zone, SourceRegion *region) : ZoneRegion(zone, region)
{
	regionSource = source;
	primaryRegion = region;
	audibleSubregion = nullptr;
	
	permeatedPosition = region->permeatedPosition;
	permeatedPathLength = region->permeatedPathLength;
}

SourceRegion::~SourceRegion()
{
}


#if C4DIAGNOSTICS

	RegionRenderable::RegionRenderable(const Region *region, const Point3D& referencePoint, float size) :
			Renderable(kRenderIndexedTriangles, kRenderDepthTest | kRenderDepthInhibit),
			diffuseColor(ColorRGBA(0.0F, 0.125F, 0.0F, 0.0F), kAttributeMutable)
	{
		wireframeColor.Set(0.0F, 1.0F, 0.0F, 1.0F);
		
		attributeList.Append(&diffuseColor);
		SetMaterialAttributeList(&attributeList);
		SetShaderFlags(kShaderAmbientEffect);
		SetAmbientBlendState(kBlendAccumulate);
		SetWireframeColorPointer(&wireframeColor);
		
		int32 vertexCount = 0;
		int32 triangleCount = 0;
		
		int32 planeCount = region->GetPlaneCount();
		const Antivector4D *plane = region->GetPlaneArray();
		for (machine a = 0; a < planeCount; a++)
		{
			Point3D		vertex[2][kMaxRegionPlaneCount + 3];
			int8		location[kMaxRegionPlaneCount + 3];
			
			const Vector3D& normal = plane[a].GetAntivector3D();
			Point3D p = referencePoint - normal * (plane[a] ^ referencePoint);
			Vector3D u = Math::CreatePerpendicular(normal);
			u *= size * InverseMag(u);
			Vector3D v = u % normal;
			
			vertex[0][0] = p + u + v;
			vertex[0][1] = p - u + v;
			vertex[0][2] = p - u - v;
			vertex[0][3] = p + u - v;
			
			int32 count = 4;
			int32 parity = 0;
			
			for (machine b = 0; b < planeCount; b++)
			{
				if (b != a)
				{
					count = Math::ClipPolygonAgainstPlane(count, vertex[parity], plane[b], location, vertex[parity ^ 1]);
					if (count == 0) goto next;
					parity ^= 1;
				}
			}
			
			for (machine b = 0; b < count; b++) vertexArray[vertexCount + b] = vertex[parity][b];
			triangleCount += Math::TriangulatePolygon(count, &vertexArray[vertexCount], -normal, &triangleArray[triangleCount], vertexCount);
			vertexCount += count;
			
			next:;
		}
		
		SetVertexCount(vertexCount);
		SetAttributeArray(kArrayVertex, vertexArray);
		SetTriangleArray(triangleCount, triangleArray);
	}
	
	RegionRenderable::~RegionRenderable()
	{
	}

#endif

// ZYURVUR
