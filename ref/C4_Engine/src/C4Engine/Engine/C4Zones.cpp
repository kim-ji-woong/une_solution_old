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


#include "C4Zones.h"
#include "C4Manipulator.h"
#include "C4Configuration.h"


using namespace C4;


const char C4::kConnectorKeyFog[]			= "%Fog";
const char C4::kConnectorKeyAcoustics[]		= "%Acoustics";
const char C4::kConnectorKeyAmbient[]		= "%Ambient";


ZoneObject::ZoneObject(ZoneType type) : Object(kObjectZone)
{
	zoneType = type;
	zoneFlags = 0;
	
	ambientLight.Set(1.0F, 1.0F, 1.0F);
	
	environmentMap = nullptr;
	environmentName[0] = 0;
}

ZoneObject::~ZoneObject()
{
	if (environmentMap) environmentMap->Release();
}

ZoneObject *ZoneObject::Construct(Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (data.GetType())
	{
		case kZoneInfinite:
			
			return (new InfiniteZoneObject);
		
		case kZoneBox:
			
			return (new BoxZoneObject);
		
		case kZoneCylinder:
			
			return (new CylinderZoneObject);
		
		case kZoneDome:
			
			return (new DomeZoneObject);
		
		case kZonePolygon:
			
			return (new PolygonZoneObject);
	}
	
	return (nullptr);
}

void ZoneObject::PackType(Packer& data) const
{
	Object::PackType(data);
	data << zoneType;
}

void ZoneObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	PackHandle handle = data.BeginChunk('DATA');
	data << zoneFlags;
	data << ambientLight;
	data << environmentName;
	data.EndChunk(handle);
	
	data << TerminatorChunk;
}

void ZoneObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	UnpackChunkList<ZoneObject>(data, unpackFlags);
}

bool ZoneObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'DATA':
			
			data >> zoneFlags;
			data >> ambientLight;
			
			data >> environmentName;
			SetEnvironmentMap(environmentName);
			return (true);
	}
	
	return (false);
}

void *ZoneObject::BeginSettingsUnpack(void)
{
	if (environmentMap)
	{
		environmentMap->Release();
		environmentMap = nullptr; 
	}
	 
	return (nullptr); 
} 

int32 ZoneObject::GetCategoryCount(void) const 
{
	return (1);
}
 
Type ZoneObject::GetCategoryType(int32 index, const char **title) const
{
	if (index == 0)
	{ 
		*title = TheInterfaceMgr->GetStringTable()->GetString(StringID(kObjectZone));
		return (kObjectZone);
	}
	
	return (0);
}

int32 ZoneObject::GetCategorySettingCount(Type category) const
{
	if (category == kObjectZone) return (5);
	return (0);
}

Setting *ZoneObject::GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const
{
	if (category == kObjectZone)
	{
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		
		if (index == 0)
		{
			const char *title = table->GetString(StringID(kObjectZone, 'ZONE'));
			return (new HeadingSetting(kObjectZone, title));
		}
		
		if (index == 1)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const char *title = table->GetString(StringID(kObjectZone, 'ZONE', 'TRAN'));
			return (new BooleanSetting('TRAN', ((zoneFlags & kZoneTransition) != 0), title));
		}
		
		if (index == 2)
		{
			const char *title = table->GetString(StringID(kObjectZone, 'ZONE', 'SKYB'));
			return (new BooleanSetting('SKYB', ((zoneFlags & kZoneRenderSkybox) != 0), title));
		}
		
		if (index == 3)
		{
			const char *title = table->GetString(StringID(kObjectZone, 'ZONE', 'AMBT'));
			const char *picker = table->GetString(StringID(kObjectZone, 'ZONE', 'CPCK'));
			return (new ColorSetting('AMBT', ambientLight, title, picker));
		}
		
		if (index == 4)
		{
			const char *title = table->GetString(StringID(kObjectZone, 'ZONE', 'ENVR'));
			const char *picker = table->GetString(StringID(kObjectZone, 'ZONE', 'TPCK'));
			return (new ResourceSetting('ENVR', environmentName, title, picker, TextureResource::GetDescriptor()));
		}
	}
	
	return (nullptr);
}

void ZoneObject::SetCategorySetting(Type category, const Setting *setting)
{
	if (category == kObjectZone)
	{
		Type identifier = setting->GetSettingIdentifier();
		
		if (identifier == 'TRAN')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) zoneFlags |= kZoneTransition;
			else zoneFlags &= ~kZoneTransition;
		}
		else if (identifier == 'SKYB')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) zoneFlags |= kZoneRenderSkybox;
			else zoneFlags &= ~kZoneRenderSkybox;
		}
		else if (identifier == 'AMBT')
		{
			ambientLight = static_cast<const ColorSetting *>(setting)->GetColor();
		}
		else if (identifier == 'ENVR')
		{
			environmentName = static_cast<const ResourceSetting *>(setting)->GetResourceName();
		}
	}
}

void ZoneObject::SetEnvironmentMap(const char *name)
{
	Texture *texture = environmentMap;
	
	environmentName = name;
	if (name[0] == 0) name = "C4/environment";
	
	environmentMap = Texture::Get(name);
	if (!environmentMap) environmentMap = Texture::Get("C4/environment");
	
	if (texture) texture->Release();
}

bool ZoneObject::CalculateOppositePlane(const Vector3D& direction, Antivector4D *plane) const
{
	return (false);
}


InfiniteZoneObject::InfiniteZoneObject() : ZoneObject(kZoneInfinite)
{
	zoneBox.min.Set(0.0F, 0.0F, 0.0F);
	zoneBox.max.Set(1.0F, 1.0F, 1.0F);
}

InfiniteZoneObject::InfiniteZoneObject(const Box3D& box) : ZoneObject(kZoneInfinite)
{
	zoneBox = box;
}

InfiniteZoneObject::~InfiniteZoneObject()
{
}

void InfiniteZoneObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	ZoneObject::Pack(data, packFlags);
	
	data << ChunkHeader('ZBOX', sizeof(Point3D) * 2);
	data << zoneBox.min;
	data << zoneBox.max;
	
	data << TerminatorChunk;
}

void InfiniteZoneObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	ZoneObject::Unpack(data, unpackFlags);
	
	#if C4LEGACY
	
		if (data.GetVersion() > 40)
		{
	
	#endif
	
			UnpackChunkList<InfiniteZoneObject>(data, unpackFlags);
	
	#if C4LEGACY
	
		}
	
	#endif
}

bool InfiniteZoneObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'ZBOX':
			
			data >> zoneBox.min;
			data >> zoneBox.max;
			return (true);
	}
	
	return (false);
}

int32 InfiniteZoneObject::GetObjectSize(float *size) const
{
	size[0] = zoneBox.min.x;
	size[1] = zoneBox.min.y;
	size[2] = zoneBox.min.z;
	size[3] = zoneBox.max.x;
	size[4] = zoneBox.max.y;
	size[5] = zoneBox.max.z;
	return (6);
}

void InfiniteZoneObject::SetObjectSize(const float *size)
{
	zoneBox.min.Set(size[0], size[1], size[2]);
	zoneBox.max.Set(size[3], size[4], size[5]);
}

bool InfiniteZoneObject::ExteriorSphere(const Point3D& center, float radius) const
{
	return (false);
}

bool InfiniteZoneObject::InteriorSphere(const Point3D& center, float radius) const
{
	return (true);
}

bool InfiniteZoneObject::ExteriorSweptSphere(const Point3D& p1, const Point3D& p2, float radius) const
{
	return (false);
}

bool InfiniteZoneObject::InteriorSweptSphere(const Point3D& p1, const Point3D& p2, float radius) const
{
	return (true);
}

bool InfiniteZoneObject::IntersectRay(const Ray *ray, float *param) const
{
	return (true);
}


BoxZoneObject::BoxZoneObject() : ZoneObject(kZoneBox)
{
}

BoxZoneObject::BoxZoneObject(const Vector3D& size) : ZoneObject(kZoneBox)
{
	boxSize = size;
}

BoxZoneObject::~BoxZoneObject()
{
}

void BoxZoneObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	ZoneObject::Pack(data, packFlags);
	
	data << ChunkHeader('SIZE', sizeof(Vector3D));
	data << boxSize;
	
	data << TerminatorChunk;
}

void BoxZoneObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	ZoneObject::Unpack(data, unpackFlags);
	UnpackChunkList<BoxZoneObject>(data, unpackFlags);
}

bool BoxZoneObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'SIZE':
			
			data >> boxSize;
			return (true);
	}
	
	return (false);
}

int32 BoxZoneObject::GetObjectSize(float *size) const
{
	size[0] = boxSize.x;
	size[1] = boxSize.y;
	size[2] = boxSize.z;
	return (3);
}

void BoxZoneObject::SetObjectSize(const float *size)
{
	boxSize.Set(size[0], size[1], size[2]);
}

bool BoxZoneObject::ExteriorSphere(const Point3D& center, float radius) const
{
	if ((center.x < -radius) || (center.x > boxSize.x + radius)) return (true);
	if ((center.y < -radius) || (center.y > boxSize.y + radius)) return (true);
	if ((center.z < -radius) || (center.z > boxSize.z + radius)) return (true);
	return (false);
}

bool BoxZoneObject::InteriorSphere(const Point3D& center, float radius) const
{
	if ((center.x < radius) || (center.x > boxSize.x - radius)) return (false);
	if ((center.y < radius) || (center.y > boxSize.y - radius)) return (false);
	if ((center.z < radius) || (center.z > boxSize.z - radius)) return (false);
	return (true);
}

bool BoxZoneObject::ExteriorSweptSphere(const Point3D& p1, const Point3D& p2, float radius) const
{
	if ((!BoxZoneObject::ExteriorSphere(p1, radius)) || (!BoxZoneObject::ExteriorSphere(p2, radius))) return (false);
	
	Point3D q1 = p1;
	Point3D q2 = p2;
	
	float d1 = q1.z + radius;
	float d2 = q2.z + radius;
	
	if (d1 < 0.0F)
	{
		if (d2 < 0.0F) return (true);
		
		float dz = q1.z - q2.z;
		if (Fabs(dz) > K::min_float)
		{
			float t = d1 / dz;
			q1 = q1 + (q2 - q1) * t;
		}
	}
	else if (d2 < 0.0F)
	{
		float dz = q1.z - q2.z;
		if (Fabs(dz) > K::min_float)
		{
			float t = d1 / dz;
			q2 = q1 + (q2 - q1) * t;
		}
	}
	
	float sz = boxSize.z + radius;
	d1 = sz - q1.z;
	d2 = sz - q2.z;
	
	if (d1 < 0.0F)
	{
		if (d2 < 0.0F) return (true);
		
		float dz = q2.z - q1.z;
		if (Fabs(dz) > K::min_float)
		{
			float t = d1 / dz;
			q1 = q1 + (q2 - q1) * t;
		}
	}
	else if (d2 < 0.0F)
	{
		float dz = q2.z - q1.z;
		if (Fabs(dz) > K::min_float)
		{
			float t = d1 / dz;
			q2 = q1 + (q2 - q1) * t;
		}
	}
	
	d1 = q1.y + radius;
	d2 = q2.y + radius;
	
	if (d1 < 0.0F)
	{
		if (d2 < 0.0F) return (true);
		
		float dy = q1.y - q2.y;
		if (Fabs(dy) > K::min_float)
		{
			float t = d1 / dy;
			q1 = q1 + (q2 - q1) * t;
		}
	}
	else if (d2 < 0.0F)
	{
		float dy = q1.y - q2.y;
		if (Fabs(dy) > K::min_float)
		{
			float t = d1 / dy;
			q2 = q1 + (q2 - q1) * t;
		}
	}
	
	float sy = boxSize.y + radius;
	d1 = sy - q1.y;
	d2 = sy - q2.y;
	
	if (d1 < 0.0F)
	{
		if (d2 < 0.0F) return (true);
		
		float dy = q2.y - q1.y;
		if (Fabs(dy) > K::min_float)
		{
			float t = d1 / dy;
			q1 = q1 + (q2 - q1) * t;
		}
	}
	else if (d2 < 0.0F)
	{
		float dy = q2.y - q1.y;
		if (Fabs(dy) > K::min_float)
		{
			float t = d1 / dy;
			q2 = q1 + (q2 - q1) * t;
		}
	}
	
	d1 = q1.x + radius;
	d2 = q2.x + radius;
	
	if (d1 < 0.0F)
	{
		if (d2 < 0.0F) return (true);
		
		float dx = q1.x - q2.x;
		if (Fabs(dx) > K::min_float)
		{
			float t = d1 / dx;
			q1 = q1 + (q2 - q1) * t;
		}
	}
	else if (d2 < 0.0F)
	{
		float dx = q1.x - q2.x;
		if (Fabs(dx) > K::min_float)
		{
			float t = d1 / dx;
			q2 = q1 + (q2 - q1) * t;
		}
	}
	
	float sx = boxSize.x + radius;
	d1 = sx - q1.x;
	d2 = sx - q2.x;
	
	if (d1 < 0.0F)
	{
		if (d2 < 0.0F) return (true);
		
		float dx = q2.x - q1.x;
		if (Fabs(dx) > K::min_float)
		{
			float t = d1 / dx;
			q1 = q1 + (q2 - q1) * t;
		}
	}
	else if (d2 < 0.0F)
	{
		float dx = q2.x - q1.x;
		if (Fabs(dx) > K::min_float)
		{
			float t = d1 / dx;
			q2 = q1 + (q2 - q1) * t;
		}
	}
	
	return (false);
}

bool BoxZoneObject::InteriorSweptSphere(const Point3D& p1, const Point3D& p2, float radius) const
{
	return ((BoxZoneObject::InteriorSphere(p1, radius)) && (BoxZoneObject::InteriorSphere(p2, radius)));
}

bool BoxZoneObject::IntersectRay(const Ray *ray, float *param) const
{
	float qx = ray->origin.x;
	float qy = ray->origin.y;
	float qz = ray->origin.z;
	float vx = ray->direction.x;
	float vy = ray->direction.y;
	float vz = ray->direction.z;
	float radius = ray->radius;
	
	if (Fabs(vx) > K::min_float)
	{
		float t = ((vx > 0.0F) ? -qx : boxSize.x - qx) / vx;
		if (t < ray->tmax)
		{
			float y = qy + vy * t;
			float z = qz + vz * t;
			if ((y > -radius) && (y < boxSize.y + radius) && (z > -radius) && (z < boxSize.z + radius))
			{
				if ((y < radius) || (y > boxSize.y - radius) || (z < radius) || (z > boxSize.z - radius))
				{
					*param = t;
					return (true);
				}
			}
		}
	}
	
	if (Fabs(vy) > K::min_float)
	{
		float t = ((vy > 0.0F) ? -qy : boxSize.y - qy) / vy;
		if (t < ray->tmax)
		{
			float x = qx + vx * t;
			float z = qz + vz * t;
			if ((x > -radius) && (x < boxSize.x + radius) && (z > -radius) && (z < boxSize.z + radius))
			{
				if ((x < radius) || (x > boxSize.x - radius) || (z < radius) || (z > boxSize.z - radius))
				{
					*param = t;
					return (true);
				}
			}
		}
	}
	
	if (Fabs(vz) > K::min_float)
	{
		float t = ((vz > 0.0F) ? -qz : boxSize.z - qz) / vz;
		if (t < ray->tmax)
		{
			float x = qx + vx * t;
			float y = qy + vy * t;
			if ((x > -radius) && (x < boxSize.x + radius) && (y > -radius) && (y < boxSize.y + radius))
			{
				if ((x < radius) || (x > boxSize.x - radius) || (y < radius) || (y > boxSize.y - radius))
				{
					*param = t;
					return (true);
				}
			}
		}
	}
	
	return (false);
}

bool BoxZoneObject::CalculateOppositePlane(const Vector3D& direction, Antivector4D *plane) const
{
	float dx = direction.x;
	float dy = direction.y;
	float dz = direction.z;
	
	float m = InverseSqrt(dx * dx + dy * dy + dz * dz);
	dx *= m;
	dy *= m;
	dz *= m;
	
	float x = (dx > 0.0F) ? boxSize.x : 0.0F;
	float y = (dy > 0.0F) ? boxSize.y : 0.0F;
	float z = (dz > 0.0F) ? boxSize.z : 0.0F;
	
	plane->Set(-dx, -dy, -dz, dx * x + dy * y + dz * z);
	return (true);
}


CylinderZoneObject::CylinderZoneObject() : ZoneObject(kZoneCylinder)
{
}

CylinderZoneObject::CylinderZoneObject(const Vector2D& size, float height) : ZoneObject(kZoneCylinder)
{
	cylinderSize = size;
	cylinderHeight = height;
	ratioXY = size.x / size.y;
}

CylinderZoneObject::~CylinderZoneObject()
{
}

void CylinderZoneObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	ZoneObject::Pack(data, packFlags);
	
	data << ChunkHeader('SIZE', sizeof(Vector2D) + 4);
	data << cylinderSize;
	data << cylinderHeight;
	
	data << TerminatorChunk;
}

void CylinderZoneObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	ZoneObject::Unpack(data, unpackFlags);
	UnpackChunkList<CylinderZoneObject>(data, unpackFlags);
}

bool CylinderZoneObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'SIZE':
			
			data >> cylinderSize;
			data >> cylinderHeight;
			ratioXY = cylinderSize.x / cylinderSize.y;
			return (true);
	}
	
	return (false);
}

int32 CylinderZoneObject::GetObjectSize(float *size) const
{
	size[0] = cylinderSize.x;
	size[1] = cylinderSize.y;
	size[2] = cylinderHeight;
	return (3);
}

void CylinderZoneObject::SetObjectSize(const float *size)
{
	cylinderSize.Set(size[0], size[1]);
	cylinderHeight = size[2];
	ratioXY = cylinderSize.x / cylinderSize.y;
}

bool CylinderZoneObject::ExteriorSphere(const Point3D& center, float radius) const
{
	if ((center.z < -radius) || (center.z > cylinderHeight + radius)) return (true);
	
	float rx = cylinderSize.x + radius;
	float ry = cylinderSize.y + radius;
	float m = rx / ry;
	
	return (center.x * center.x + m * m * center.y * center.y > rx * rx);
}

bool CylinderZoneObject::InteriorSphere(const Point3D& center, float radius) const
{
	if ((center.z < radius) || (center.z > cylinderHeight - radius)) return (false);
	
	float rx = cylinderSize.x - radius;
	if (rx <= 0.0F) return (false);
	
	float ry = cylinderSize.y - radius;
	if (ry <= 0.0F) return (false);
	
	float m = rx / ry;
	if (center.x * center.x + m * m * center.y * center.y > rx * rx) return (false);
	return (true);
}

bool CylinderZoneObject::ExteriorSweptSphere(const Point3D& p1, const Point3D& p2, float radius) const
{
	if ((!CylinderZoneObject::ExteriorSphere(p1, radius)) || (!CylinderZoneObject::ExteriorSphere(p2, radius))) return (false);
	
	float sx = p1.x;
	float sy = p1.y;
	float dx = p2.x - sx;
	float dy = p2.y - sy;
	
	float rx = cylinderSize.x + radius;
	float ry = cylinderSize.y + radius;
	float m2 = rx * rx / (ry * ry);
	
	float a = dx * dx + m2 * dy * dy;
	if (Fabs(a) > K::min_float)
	{
		float b = -(sx * dx + m2 * sy * dy);
		float d = b * b - a * (sx * sx + m2 * sy * sy - rx * rx);
		if (d < K::min_float) return (true);
		
		a = 1.0F / a;
		d = Sqrt(d);
		float t1 = (b - d) * a;
		float t2 = (b + d) * a;
		
		if ((t1 < 0.0F) && (t2 < 0.0F)) return (true);
		if ((t1 > 1.0F) && (t2 > 1.0F)) return (true);
		
		float sz = p1.z;
		float dz = p2.z - sz;
		
		float z1 = sz + dz * t1;
		float z2 = sz + dz * t2;
		if ((z1 < -radius) && (z2 < -radius)) return (true);
		
		float height = cylinderHeight + radius;
		if ((z1 > height) && (z2 > height)) return (true);
		
		return (false);
	}
	
	float z1 = p1.z;
	float z2 = p2.z;
	if ((z1 < -radius) && (z2 < -radius)) return (true);
	
	float height = cylinderHeight + radius;
	if ((z1 > height) && (z2 > height)) return (true);
	
	return (sx * sx + m2 * sy * sy > rx * rx);
}

bool CylinderZoneObject::InteriorSweptSphere(const Point3D& p1, const Point3D& p2, float radius) const
{
	return ((CylinderZoneObject::InteriorSphere(p1, radius)) && (CylinderZoneObject::InteriorSphere(p2, radius)));
}

bool CylinderZoneObject::IntersectRay(const Ray *ray, float *param) const
{
	float qx = ray->origin.x;
	float qy = ray->origin.y;
	float qz = ray->origin.z;
	float vx = ray->direction.x;
	float vy = ray->direction.y;
	float vz = ray->direction.z;
	float radius = ray->radius;
	
	float r = cylinderSize.x + radius;
	float m2 = ratioXY * ratioXY;
	
	float a = vx * vx + vy * vy * m2;
	float b = qx * vx + qy * vy * m2;
	float c = qx * qx + qy * qy * m2 - r * r;
	float d = b * b - a * c;
	
	if (d > K::min_float)
	{
		float t = -(b + Sqrt(d)) / a;
		if (t < ray->tmax)
		{
			float z = qz + vz * t;
			if ((z > -radius) && (z < cylinderHeight + radius))
			{
				*param = t;
				return (true);
			}
		}
	}
	
	if (Fabs(vz) > K::min_float)
	{
		float t = ((vz > 0.0F) ? -qz : cylinderHeight - qz) / vz;
		if (t < ray->tmax)
		{
			float ri = cylinderSize.x - radius;
			float x = qx + vx * t;
			float y = qy + vy * t;
			if ((x * x + y * y * m2 < r * r) && (x * x + y * y * m2 > ri * ri))
			{
				*param = t;
				return (true);
			}
		}
	}
	
	return (false);
}

bool CylinderZoneObject::CalculateOppositePlane(const Vector3D& direction, Antivector4D *plane) const
{
	float dx = direction.x;
	float dy = direction.y;
	float dz = direction.z;
	
	float m = InverseSqrt(dx * dx + dy * dy + dz * dz);
	dx *= m;
	dy *= m;
	dz *= m;
	
	float x = dx * cylinderSize.x;
	float y = dy * cylinderSize.y;
	
	float w = dx * x + dy * y;
	if (dz > 0.0F) w += dz * cylinderHeight;
	
	plane->Set(-dx, -dy, -dz, w);
	return (true);
}


DomeZoneObject::DomeZoneObject() : ZoneObject(kZoneDome)
{
}

DomeZoneObject::DomeZoneObject(const Vector3D& size) : ZoneObject(kZoneDome)
{
	domeSize = size;
	ratioXY = size.x / size.y;
	ratioXZ = size.x / size.z;
}

DomeZoneObject::~DomeZoneObject()
{
}

void DomeZoneObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	ZoneObject::Pack(data, packFlags);
	
	data << ChunkHeader('SIZE', sizeof(Vector3D));
	data << domeSize;
	
	data << TerminatorChunk;
}

void DomeZoneObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	ZoneObject::Unpack(data, unpackFlags);
	UnpackChunkList<DomeZoneObject>(data, unpackFlags);
}

bool DomeZoneObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'SIZE':
			
			data >> domeSize;
			ratioXY = domeSize.x / domeSize.y;
			ratioXZ = domeSize.x / domeSize.z;
			return (true);
	}
	
	return (false);
}

int32 DomeZoneObject::GetObjectSize(float *size) const
{
	size[0] = domeSize.x;
	size[1] = domeSize.y;
	size[2] = domeSize.z;
	return (3);
}

void DomeZoneObject::SetObjectSize(const float *size)
{
	domeSize.x = size[0];
	domeSize.y = size[1];
	domeSize.z = size[2];
	ratioXY = domeSize.x / domeSize.y;
	ratioXZ = domeSize.x / domeSize.z;
}

bool DomeZoneObject::ExteriorSphere(const Point3D& center, float radius) const
{
	if (center.z < -radius) return (true);
	
	float rx = domeSize.x + radius;
	float ry = domeSize.y + radius;
	float rz = domeSize.z + radius;
	float m = rx / ry;
	float n = rx / rz;
	
	return (center.x * center.x + m * m * center.y * center.y + n * n * center.z * center.z > rx * rx);
}

bool DomeZoneObject::InteriorSphere(const Point3D& center, float radius) const
{
	if (center.z < radius) return (false);
	
	float rx = domeSize.x - radius;
	if (rx <= 0.0F) return (false);
	
	float ry = domeSize.y - radius;
	if (ry <= 0.0F) return (false);
	
	float rz = domeSize.z - radius;
	if (rz <= 0.0F) return (false);
	
	float m = rx / ry;
	float n = rx / rz;
	if (center.x * center.x + m * m * center.y * center.y + n * n * center.z * center.z > rx * rx) return (false);
	return (true);
}

bool DomeZoneObject::ExteriorSweptSphere(const Point3D& p1, const Point3D& p2, float radius) const
{
	if ((!DomeZoneObject::ExteriorSphere(p1, radius)) || (!DomeZoneObject::ExteriorSphere(p2, radius))) return (false);
	
	float sz = p1.z;
	if ((sz < -radius) && (p2.z < -radius)) return (true);
	
	float sx = p1.x;
	float sy = p1.y;
	float dx = p2.x - sx;
	float dy = p2.y - sy;
	float dz = p2.z - sz;
	
	float rx = domeSize.x + radius;
	float ry = domeSize.y + radius;
	float rz = domeSize.y + radius;
	float m2 = rx * rx / (ry * ry);
	float n2 = rx * rx / (rz * rz);
	
	float a = dx * dx + m2 * dy * dy + n2 * dz * dz;
	float b = -(sx * dx + m2 * sy * dy + n2 * sz * dz);
	float d = b * b - a * (sx * sx + m2 * sy * sy + n2 * sz * sz - rx * rx);
	if (d < K::min_float) return (true);
	
	a = 1.0F / a;
	d = Sqrt(d);
	float t1 = (b - d) * a;
	float t2 = (b + d) * a;
	
	if ((t1 < 0.0F) && (t2 < 0.0F)) return (true);
	if ((t1 > 1.0F) && (t2 > 1.0F)) return (true);
	
	float z1 = sz + dz * t1;
	float z2 = sz + dz * t2;
	if ((z1 < -radius) && (z2 < -radius)) return (true);
	
	return (false);
}

bool DomeZoneObject::InteriorSweptSphere(const Point3D& p1, const Point3D& p2, float radius) const
{
	return ((DomeZoneObject::InteriorSphere(p1, radius)) && (DomeZoneObject::InteriorSphere(p2, radius)));
}

bool DomeZoneObject::IntersectRay(const Ray *ray, float *param) const
{
	float qx = ray->origin.x;
	float qy = ray->origin.y;
	float qz = ray->origin.z;
	float vx = ray->direction.x;
	float vy = ray->direction.y;
	float vz = ray->direction.z;
	float radius = ray->radius;
	
	float r = domeSize.x + radius;
	float m2 = ratioXY * ratioXY;
	float n2 = ratioXZ * ratioXZ;
	
	float a = vx * vx + vy * vy * m2 + vz * vz * n2;
	float b = qx * vx + qy * vy * m2 + qz * vz * n2;
	float c = qx * qx + qy * qy * m2 + qz * qz * n2 - r * r;
	float d = b * b - a * c;
	
	if (d > K::min_float)
	{
		float t = -(b + Sqrt(d)) / a;
		if (t < ray->tmax)
		{
			float z = qz + vz * t;
			if (z > -radius)
			{
				*param = t;
				return (true);
			}
		}
	}
	
	if (vz > K::min_float)
	{
		float t = -qz / vz;
		if (t < ray->tmax)
		{
			float ri = domeSize.x - radius;
			float x = qx + vx * t;
			float y = qy + vy * t;
			if ((x * x + y * y * m2 < r * r) && (x * x + y * y * m2 > ri * ri))
			{
				*param = t;
				return (true);
			}
		}
	}
	
	return (false);
}

bool DomeZoneObject::CalculateOppositePlane(const Vector3D& direction, Antivector4D *plane) const
{
	float dx = direction.x;
	float dy = direction.y;
	float dz = direction.z;
	
	float m = InverseSqrt(dx * dx + dy * dy + dz * dz);
	dx *= m;
	dy *= m;
	dz *= m;
	
	float x = dx * domeSize.x;
	float y = dy * domeSize.y;
	
	float w = dx * x + dy * y;
	if (dz > 0.0F) w += dz * dz * domeSize.z;
	
	plane->Set(-dx, -dy, -dz, w);
	return (true);
}


PolygonZoneObject::PolygonZoneObject() : ZoneObject(kZonePolygon)
{
}

PolygonZoneObject::PolygonZoneObject(const Vector2D& size, float height) : ZoneObject(kZonePolygon)
{
	SetPolygonSize(size, height);
}

PolygonZoneObject::~PolygonZoneObject()
{
}

void PolygonZoneObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	ZoneObject::Pack(data, packFlags);
	
	data << ChunkHeader('HITE', 4);
	data << polygonHeight;
	
	data << ChunkHeader('VERT', 4 + vertexCount * sizeof(Point3D));
	data << vertexCount;
	data.WriteArray(vertexCount, polygonVertex);
	
	data << TerminatorChunk;
}

void PolygonZoneObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	ZoneObject::Unpack(data, unpackFlags);
	UnpackChunkList<PolygonZoneObject>(data, unpackFlags);
}

bool PolygonZoneObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'HITE':
			
			data >> polygonHeight;
			return (true);
		
		case 'VERT':
			
			data >> vertexCount;
			data.ReadArray(vertexCount, polygonVertex);
			return (true);
	}
	
	return (false);
}

void PolygonZoneObject::SetPolygonSize(const Vector2D& size, float height)
{
	polygonHeight = height;
	vertexCount = 4;
	
	float x = size.x;
	float y = size.y;
	
	polygonVertex[0].Set(0.0F, 0.0F, 0.0F);
	polygonVertex[1].Set(x, 0.0F, 0.0F);
	polygonVertex[2].Set(x, y, 0.0F);
	polygonVertex[3].Set(0.0F, y, 0.0F);
}

int32 PolygonZoneObject::GetObjectSize(float *size) const
{
	size[0] = polygonHeight;
	return (1);
}

void PolygonZoneObject::SetObjectSize(const float *size)
{
	polygonHeight = size[0];
}

bool PolygonZoneObject::ExteriorSphere(const Point3D& center, float radius) const
{
	if ((center.z < -radius) || (center.z > polygonHeight + radius)) return (true);
	
	int32 count = vertexCount;
	const Point3D *v1 = &polygonVertex[count - 1];
	for (machine a = 0; a < count; a++)
	{
		const Point3D *v2 = &polygonVertex[a];
		
		float nx = v2->y - v1->y;
		float ny = v1->x - v2->x;
		float m = InverseSqrt(nx * nx + ny * ny);
		nx *= m;
		ny *= m;
		
		if ((center.x - v1->x) * nx + (center.y - v1->y) * ny > radius) return (true);
		
		v1 = v2;
	}
	
	return (false);
}

bool PolygonZoneObject::InteriorSphere(const Point3D& center, float radius) const
{
	if ((center.z < radius) || (center.z > polygonHeight - radius)) return (false);
	
	int32 count = vertexCount;
	const Point3D *v1 = &polygonVertex[count - 1];
	for (machine a = 0; a < count; a++)
	{
		const Point3D *v2 = &polygonVertex[a];
		
		float nx = v1->y - v2->y;
		float ny = v2->x - v1->x;
		float m = InverseSqrt(nx * nx + ny * ny);
		nx *= m;
		ny *= m;
		
		if ((center.x - v1->x) * nx + (center.y - v1->y) * ny < radius) return (false);
		
		v1 = v2;
	}
	
	return (true);
}

bool PolygonZoneObject::ExteriorSweptSphere(const Point3D& p1, const Point3D& p2, float radius) const
{
	if ((!PolygonZoneObject::ExteriorSphere(p1, radius)) || (!PolygonZoneObject::ExteriorSphere(p2, radius))) return (false);
	
	Point3D q1 = p1;
	Point3D q2 = p2;
	
	float d1 = q1.z + radius;
	float d2 = q2.z + radius;
	
	if (d1 < 0.0F)
	{
		if (d2 < 0.0F) return (true);
		
		float dz = q1.z - q2.z;
		if (Fabs(dz) > K::min_float)
		{
			float t = d1 / dz;
			q1 = q1 + (q2 - q1) * t;
		}
	}
	else if (d2 < 0.0F)
	{
		float dz = q1.z - q2.z;
		if (Fabs(dz) > K::min_float)
		{
			float t = d1 / dz;
			q2 = q1 + (q2 - q1) * t;
		}
	}
	
	float sz = polygonHeight + radius;
	d1 = sz - q1.z;
	d2 = sz - q2.z;
	
	if (d1 < 0.0F)
	{
		if (d2 < 0.0F) return (true);
		
		float dz = q2.z - q1.z;
		if (Fabs(dz) > K::min_float)
		{
			float t = d1 / dz;
			q1 = q1 + (q2 - q1) * t;
		}
	}
	else if (d2 < 0.0F)
	{
		float dz = q2.z - q1.z;
		if (Fabs(dz) > K::min_float)
		{
			float t = d1 / dz;
			q2 = q1 + (q2 - q1) * t;
		}
	}
	
	int32 count = vertexCount;
	const Point3D *v1 = &polygonVertex[count - 1];
	for (machine a = 0; a < count; a++)
	{
		const Point3D *v2 = &polygonVertex[a];
		
		float nx = v1->y - v2->y;
		float ny = v2->x - v1->x;
		float m = InverseSqrt(nx * nx + ny * ny);
		nx *= m;
		ny *= m;
		
		float w = radius - nx * v1->x - ny * v1->y;
		d1 = q1.x * nx + q1.y * ny + w;
		d2 = q2.x * nx + q2.y * ny + w;
		
		if (d1 < 0.0F)
		{
			if (d2 < 0.0F) return (true);
			
			float dq = d1 - d2;
			if (Fabs(dq) > K::min_float)
			{
				float t = d1 / dq;
				q1 = q1 + (q2 - q1) * t;
			}
		}
		else if (d2 < 0.0F)
		{
			float dq = d1 - d2;
			if (Fabs(dq) > K::min_float)
			{
				float t = d1 / dq;
				q2 = q1 + (q2 - q1) * t;
			}
		}
		
		v1 = v2;
	}
	
	return (false);
}

bool PolygonZoneObject::InteriorSweptSphere(const Point3D& p1, const Point3D& p2, float radius) const
{
	return ((PolygonZoneObject::InteriorSphere(p1, radius)) && (PolygonZoneObject::InteriorSphere(p2, radius)));
}

bool PolygonZoneObject::IntersectRay(const Ray *ray, float *param) const
{
	Point3D q1 = ray->origin + ray->direction * ray->tmin;
	Point3D q2 = ray->origin + ray->direction * ray->tmax;
	if ((!PolygonZoneObject::ExteriorSphere(q1, ray->radius)) && (!PolygonZoneObject::ExteriorSphere(q2, ray->radius))) return (false);
	
	bool clip = false;
	float d1 = q1.z + ray->radius;
	float d2 = q2.z + ray->radius;
	
	if (d1 < 0.0F)
	{
		if (d2 < 0.0F) return (false);
		
		float dz = q1.z - q2.z;
		if (Fabs(dz) > K::min_float)
		{
			float t = d1 / dz;
			q1 = q1 + (q2 - q1) * t;
			clip = true;
		}
	}
	else if (d2 < 0.0F)
	{
		float dz = q1.z - q2.z;
		if (Fabs(dz) > K::min_float)
		{
			float t = d1 / dz;
			q2 = q1 + (q2 - q1) * t;
		}
	}
	
	float sz = polygonHeight + ray->radius;
	d1 = sz - q1.z;
	d2 = sz - q2.z;
	
	if (d1 < 0.0F)
	{
		if (d2 < 0.0F) return (false);
		
		float dz = q2.z - q1.z;
		if (Fabs(dz) > K::min_float)
		{
			float t = d1 / dz;
			q1 = q1 + (q2 - q1) * t;
			clip = true;
		}
	}
	else if (d2 < 0.0F)
	{
		float dz = q2.z - q1.z;
		if (Fabs(dz) > K::min_float)
		{
			float t = d1 / dz;
			q2 = q1 + (q2 - q1) * t;
		}
	}
	
	int32 count = vertexCount;
	const Point3D *v1 = &polygonVertex[count - 1];
	for (machine a = 0; a < count; a++)
	{
		const Point3D *v2 = &polygonVertex[a];
		
		float nx = v1->y - v2->y;
		float ny = v2->x - v1->x;
		float m = InverseSqrt(nx * nx + ny * ny);
		nx *= m;
		ny *= m;
		
		float w = ray->radius - nx * v1->x - ny * v1->y;
		d1 = q1.x * nx + q1.y * ny + w;
		d2 = q2.x * nx + q2.y * ny + w;
		
		if (d1 < 0.0F)
		{
			if (d2 < 0.0F) return (false);
			
			float dq = nx * (q2.x - q1.x) + ny * (q2.y - q1.y) + w;
			if (Fabs(dq) > K::min_float)
			{
				float t = d1 / dq;
				q1 = q1 + (q1 - q2) * t;
				clip = true;
			}
		}
		else if (d2 < 0.0F)
		{
			float dq = nx * (q2.x - q1.x) + ny * (q2.y - q1.y) + w;
			if (Fabs(dq) > K::min_float)
			{
				float t = d1 / dq;
				q2 = q1 + (q1 - q2) * t;
			}
		}
		
		v1 = v2;
	}
	
	*param = (((clip) ? q1 : q2) - ray->origin) * ray->direction;
	return (true);
}

bool PolygonZoneObject::CalculateOppositePlane(const Vector3D& direction, Antivector4D *plane) const
{
	const Point3D	*q;
	
	float dx = direction.x;
	float dy = direction.y;
	float dz = direction.z;
	
	float m = InverseSqrt(dx * dx + dy * dy + dz * dz);
	dx *= m;
	dy *= m;
	dz *= m;
	
	float f = K::minus_infinity;
	int32 count = vertexCount;
	
	if (dz < 0.0F)
	{
		for (machine a = 0; a < count; a++)
		{
			const Point3D& p = polygonVertex[a];
			float d = p.x * dx + p.y * dy + p.z * dz;
			if (d > f)
			{
				q = &p;
				f = d;
			}
		}
		
		plane->Set(-dx, -dy, -dz, dx * q->x + dy * q->y + dz * q->z);
	}
	else
	{
		float h = polygonHeight;
		for (machine a = 0; a < count; a++)
		{
			const Point3D& p = polygonVertex[a];
			float d = p.x * dx + p.y * dy + (p.z + h) * dz;
			if (d > f)
			{
				q = &p;
				f = d;
			}
		}
		
		plane->Set(-dx, -dy, -dz, dx * q->x + dy * q->y + dz * (q->z + h));
	}
	
	return (true);
}


C4::Zone::Zone(ZoneType type) :
		Node(kNodeZone),
		visibilityGraph(this)
{
	zoneType = type;
	
	exclusionMask = 0;
	transitionMapping = this;
	
	connectedFogSpace = nullptr;
	connectedAcousticsSpace = nullptr;
}

C4::Zone::Zone(const Zone& zone) :
		Node(zone),
		visibilityGraph(this)
{
	zoneType = zone.zoneType;
	
	exclusionMask = 0;
	transitionMapping = this;
	
	connectedFogSpace = nullptr;
	connectedAcousticsSpace = nullptr;
}

C4::Zone::~Zone()
{
	subzoneList.RemoveAll();
	portalList.RemoveAll();
	occlusionPortalList.RemoveAll();
	occlusionSpaceList.RemoveAll();
	fogSpaceList.RemoveAll();
	markerList.RemoveAll();
	instanceList.RemoveAll();
	
	lightRegionList.RemoveAll();
	sourceRegionList.RemoveAll();
}

C4::Zone *C4::Zone::Construct(Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (data.GetType())
	{
		case kZoneInfinite:
			
			return (new InfiniteZone);
		
		case kZoneBox:
			
			return (new BoxZone);
		
		case kZoneCylinder:
			
			return (new CylinderZone);
		
		case kZoneDome:
			
			return (new DomeZone);
		
		case kZonePolygon:
			
			return (new PolygonZone);
	}
	
	return (nullptr);
}

void C4::Zone::PackType(Packer& data) const
{
	Node::PackType(data);
	data << zoneType;
}

void C4::Zone::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Node::Pack(data, packFlags);
	
	const Node *physicsNode = physicsNodeLink;
	if ((physicsNode) && (physicsNode->LinkedNodePackable(packFlags)))
	{
		data << ChunkHeader('PLNK', 4);
		data << physicsNode->GetNodeIndex();
	}
	
	data << TerminatorChunk;
}

void C4::Zone::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Node::Unpack(data, unpackFlags);
	UnpackChunkList<Zone>(data, unpackFlags);
}

bool C4::Zone::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		#if C4LEGACY
		
			case 'FLNK':
			{
				int32	fogSpaceIndex;
				
				data >> fogSpaceIndex;
				data.AddNodeLink(fogSpaceIndex, &FogSpaceLinkProc, this);
				return (true);
			}
			
			case 'SLNK':
			{
				int32	acousticsSpaceIndex;
				
				data >> acousticsSpaceIndex;
				data.AddNodeLink(acousticsSpaceIndex, &AcousticsSpaceLinkProc, this);
				return (true);
			}
			
			case 'ALNK':
			{
				int32	ambientSpaceIndex;
				
				data >> ambientSpaceIndex;
				data.AddNodeLink(ambientSpaceIndex, &AmbientSpaceLinkProc, this);
				return (true);
			}
		
		#endif
		
		case 'PLNK':
		{
			int32	physicsNodeIndex;
			
			data >> physicsNodeIndex;
			data.AddNodeLink(physicsNodeIndex, &PhysicsNodeLinkProc, this);
			return (true);
		}
	}
	
	return (false);
}

#if C4LEGACY

	void C4::Zone::FogSpaceLinkProc(Node *node, void *cookie)
	{
		Zone *zone = static_cast<Zone *>(cookie);
		zone->SetConnectedFogSpace(static_cast<FogSpace *>(node));
	}
	
	void C4::Zone::AcousticsSpaceLinkProc(Node *node, void *cookie)
	{
		Zone *zone = static_cast<Zone *>(cookie);
		zone->SetConnectedAcousticsSpace(static_cast<AcousticsSpace *>(node));
	}
	
	void C4::Zone::AmbientSpaceLinkProc(Node *node, void *cookie)
	{
		Zone *zone = static_cast<Zone *>(cookie);
		zone->SetConnectedAmbientSpace(static_cast<AmbientSpace *>(node));
	}

#endif

void C4::Zone::PhysicsNodeLinkProc(Node *node, void *cookie)
{
	Zone *zone = static_cast<Zone *>(cookie);
	zone->physicsNodeLink = node;
}

int32 C4::Zone::GetInternalConnectorCount(void) const
{
	return (3);
}

const char *C4::Zone::GetInternalConnectorKey(int32 index) const
{
	if (index == 0) return (kConnectorKeyFog);
	else if (index == 1) return (kConnectorKeyAcoustics);
	else if (index == 2) return (kConnectorKeyAmbient);
	return (nullptr);
}

void C4::Zone::ProcessInternalConnectors(void)
{
	connectedFogSpace = static_cast<FogSpace *>(GetConnectedNode(kConnectorKeyFog));
	connectedAcousticsSpace = static_cast<AcousticsSpace *>(GetConnectedNode(kConnectorKeyAcoustics));
	
	const AmbientSpace *ambientSpace = GetConnectedAmbientSpace();
	if (ambientSpace)
	{
		ambientEnvironment.ambientShaderType = kShaderAmbientSpace;
		ambientEnvironment.ambientSpaceObject = ambientSpace->GetObject();
		ambientEnvironment.ambientSpaceTransformable = ambientSpace;
	}
	else
	{
		ambientEnvironment.ambientShaderType = kShaderAmbient;
		ambientEnvironment.ambientSpaceObject = nullptr;
		ambientEnvironment.ambientSpaceTransformable = nullptr;
	}
}

bool C4::Zone::ValidConnectedNode(const ConnectorKey& key, const Node *node) const
{
	if (key == kConnectorKeyFog)
	{
		if (node->GetNodeType() == kNodeSpace) return (static_cast<const Space *>(node)->GetSpaceType() == kSpaceFog);
		return (false);
	}
	
	if (key == kConnectorKeyAcoustics)
	{
		if (node->GetNodeType() == kNodeSpace) return (static_cast<const Space *>(node)->GetSpaceType() == kSpaceAcoustics);
		return (false);
	}
	
	if (key == kConnectorKeyAmbient)
	{
		if (node->GetNodeType() == kNodeSpace) return (static_cast<const Space *>(node)->GetSpaceType() == kSpaceAmbient);
		return (false);
	}
	
	return (Node::ValidConnectedNode(key, node));
}

void C4::Zone::SetConnectedFogSpace(FogSpace *fogSpace)
{
	connectedFogSpace = fogSpace;
	
	if (fogSpace)
	{
		Hub *hub = GetHub();
		if (hub)
		{
			Connector *connector = hub->FindOutgoingConnector(kConnectorKeyFog);
			if (connector)
			{
				connector->SetConnectorTarget(fogSpace);
				return;
			}
		}
		
		AddConnector(kConnectorKeyFog, fogSpace);
	}
	else
	{
		RemoveConnector(kConnectorKeyFog);
	}
}

void C4::Zone::SetConnectedAcousticsSpace(AcousticsSpace *acousticsSpace)
{
	connectedAcousticsSpace = acousticsSpace;
	
	if (acousticsSpace)
	{
		Hub *hub = GetHub();
		if (hub)
		{
			Connector *connector = hub->FindOutgoingConnector(kConnectorKeyAcoustics);
			if (connector)
			{
				connector->SetConnectorTarget(acousticsSpace);
				return;
			}
		}
		
		AddConnector(kConnectorKeyAcoustics, acousticsSpace);
	}
	else
	{
		RemoveConnector(kConnectorKeyAcoustics);
	}
}

AmbientSpace *C4::Zone::GetConnectedAmbientSpace(void) const
{
	Node *node = GetConnectedNode(kConnectorKeyAmbient);
	if (node) return (static_cast<AmbientSpace *>(node));
	return (nullptr);
}

void C4::Zone::SetConnectedAmbientSpace(AmbientSpace *ambientSpace)
{
	if (ambientSpace)
	{
		ambientEnvironment.ambientSpaceObject = ambientSpace->GetObject();
		ambientEnvironment.ambientSpaceTransformable = ambientSpace;
		
		Hub *hub = GetHub();
		if (hub)
		{
			Connector *connector = hub->FindOutgoingConnector(kConnectorKeyAmbient);
			if (connector)
			{
				connector->SetConnectorTarget(ambientSpace);
				return;
			}
		}
		
		AddConnector(kConnectorKeyAmbient, ambientSpace);
	}
	else
	{
		ambientEnvironment.ambientSpaceObject = nullptr;
		ambientEnvironment.ambientSpaceTransformable = nullptr;
		RemoveConnector(kConnectorKeyAmbient);
	}
}

void C4::Zone::Preprocess(void)
{
	if (!GetManipulator())
	{
		const Box3D& box = GetWorldBoundingBox();
		float size = Fmax(box.max.x - box.min.x, box.max.y - box.min.y);
		
		if (size >= 256.0F)
		{
			visibilityGraph.Activate(box, 64.0F);
			triggerGraph.Activate(box, 64.0F);
			fieldGraph.Activate(box, 64.0F);
		}
		else if (size >= 128.0F)
		{
			visibilityGraph.Activate(box, 32.0F);
			triggerGraph.Activate(box, 32.0F);
			fieldGraph.Activate(box, 32.0F);
		}
		else if (size >= 64.0F)
		{
			visibilityGraph.Activate(box, 16.0F);
			triggerGraph.Activate(box, 16.0F);
			fieldGraph.Activate(box, 16.0F);
		}
		else if (size >= 32.0F)
		{
			visibilityGraph.Activate(box, 8.0F);
		}
		else if (size >= 16.0F)
		{
			visibilityGraph.Activate(box, 4.0F);
		}
	}
	
	const ZoneObject *object = GetObject();
	ambientEnvironment.ambientLightColor = &object->GetAmbientLight();
	ambientEnvironment.environmentMap = &object->GetEnvironmentMap();
	
	Node::Preprocess();
	ProcessTransitions();
}

void C4::Zone::Neutralize(void)
{
	ListElement<Zone>::Detach();
	Node::Neutralize();
}

void C4::Zone::EnterZone(Zone *zone)
{
	zone->AddSubzone(this);
}

void C4::Zone::InvalidateLightRegions(void) const
{
	for (;;)
	{
		const LightRegion *lightRegion = GetFirstLightRegion();
		if (!lightRegion) break;
		
		lightRegion->GetLight()->InvalidateLightRegions();
	}
}

void C4::Zone::InvalidateSourceRegions(void) const
{
	const SourceRegion *sourceRegion = GetFirstSourceRegion();
	while (sourceRegion)
	{
		sourceRegion->GetSource()->InvalidateSourceRegions();
		sourceRegion = sourceRegion->GetNextSourceRegion();
	}
}

void C4::Zone::AddTransition(Zone *zone)
{
	Bond *bond = zoneSite.GetFirstOutgoingEdge();
	while (bond)
	{
		if (bond->GetFinishElement() == zone) return;
		bond = bond->GetNextOutgoingEdge();
	}
	
	new Bond(&zoneSite, zone);
}

void C4::Zone::ProcessTransitions(void)
{
	const ZoneObject *object = GetObject();
	if (object)
	{
		PurgeIncomingEdges();
		
		if (object->GetZoneFlags() & kZoneTransition)
		{
			Zone *superZone = GetOwningZone();
			
			int32 count = 0;
			const Portal *portal = portalList.First();
			while (portal)
			{
				if (portal->GetPortalType() == kPortalDirect)
				{
					Zone *zone = portal->GetConnectedZone();
					if (zone)
					{
						zone->AddTransition(this);
						if (zone == superZone) transitionMapping = superZone;
						
						if (count == 0)
						{
							ambientEnvironment.ambientLightColor = &zone->GetObject()->GetAmbientLight();
							ambientEnvironment.gradientPortal[0] = portal;
						}
						else if (count == 1)
						{
							ambientEnvironment.gradientLightColor = &zone->GetObject()->GetAmbientLight();
							ambientEnvironment.gradientPortal[1] = portal;
						}
						
						count++;
					}
				}
				
				portal = portal->Next();
			}
			
			if (ambientEnvironment.ambientShaderType != kShaderAmbientSpace) ambientEnvironment.ambientShaderType = (count == 2) ? kShaderAmbientGradient : kShaderAmbient;
		}
	}
}


InfiniteZone::InfiniteZone() : Zone(kZoneInfinite)
{
	auxiliaryObject = nullptr;
	
	SetActiveUpdateFlags(GetActiveUpdateFlags() | kUpdatePostTransform);
	SetWorldBoundingBox(Zero3D, Zero3D);
}

InfiniteZone::InfiniteZone(const InfiniteZone& infiniteZone) : Zone(infiniteZone)
{
	auxiliaryObject = infiniteZone.auxiliaryObject;
	if (auxiliaryObject) auxiliaryObject->Retain();
	
	SetActiveUpdateFlags(GetActiveUpdateFlags() | kUpdatePostTransform);
	SetWorldBoundingBox(Zero3D, Zero3D);
}

InfiniteZone::~InfiniteZone()
{
	if (auxiliaryObject) auxiliaryObject->Release();
}

Node *InfiniteZone::Replicate(void) const
{
	return (new InfiniteZone(*this));
}

void InfiniteZone::Prepack(List<Object> *linkList) const
{
	Zone::Prepack(linkList);
	if (auxiliaryObject) linkList->Append(auxiliaryObject);
}

void InfiniteZone::Pack(Packer& data, unsigned_int32 packFlags) const
{
	Zone::Pack(data, packFlags);
	
	if ((auxiliaryObject) && (!(packFlags & kPackSettings)))
	{
		data << ChunkHeader('AUXO', 4);
		data << auxiliaryObject->GetObjectIndex();
	}
	
	data << TerminatorChunk;
}

void InfiniteZone::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	Zone::Unpack(data, unpackFlags);
	UnpackChunkList<InfiniteZone>(data, unpackFlags);
}

bool InfiniteZone::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'AUXO':
		{
			int32	objectIndex;
			
			data >> objectIndex;
			data.AddObjectLink(objectIndex, &AuxiliaryObjectLinkProc, this);
			return (true);
		}
	}
	
	return (false);
}

void InfiniteZone::AuxiliaryObjectLinkProc(Object *object, void *cookie)
{
	InfiniteZone *infiniteZone = static_cast<InfiniteZone *>(cookie);
	infiniteZone->SetAuxiliaryObject(object);
}

void InfiniteZone::SetAuxiliaryObject(Object *object)
{
	if (auxiliaryObject != object)
	{
		if (auxiliaryObject) auxiliaryObject->Release();
		if (object) object->Retain();
		auxiliaryObject = object;
	}
}

bool InfiniteZone::CalculateBoundingBox(Box3D *box) const
{
	*box = GetObject()->GetZoneBox();
	return (true);
}

void InfiniteZone::Preprocess(void)
{
	SetWorldBoundingBox(GetObject()->GetZoneBox());
	Zone::Preprocess();
}


BoxZone::BoxZone() : Zone(kZoneBox)
{
}

BoxZone::BoxZone(const Vector3D& size) : Zone(kZoneBox)
{
	SetNewObject(new BoxZoneObject(size));
}

BoxZone::BoxZone(const BoxZone& boxZone) : Zone(boxZone)
{
}

BoxZone::~BoxZone()
{
}

Node *BoxZone::Replicate(void) const
{
	return (new BoxZone(*this));
}

bool BoxZone::CalculateBoundingBox(Box3D *box) const
{
	box->min.Set(0.0F, 0.0F, 0.0F);
	box->max = GetObject()->GetBoxSize();
	return (true);
}

void BoxZone::Preprocess(void)
{
	Zone::Preprocess();
	
	SetVisibilityProc(&BoxVisible);
}


CylinderZone::CylinderZone() : Zone(kZoneCylinder)
{
}

CylinderZone::CylinderZone(const Vector2D& size, float height) : Zone(kZoneCylinder)
{
	SetNewObject(new CylinderZoneObject(size, height));
}

CylinderZone::CylinderZone(const CylinderZone& cylinderZone) : Zone(cylinderZone)
{
}

CylinderZone::~CylinderZone()
{
}

Node *CylinderZone::Replicate(void) const
{
	return (new CylinderZone(*this));
}

bool CylinderZone::CalculateBoundingBox(Box3D *box) const
{
	const CylinderZoneObject *object = GetObject();
	const Vector2D& cylinderSize = object->GetCylinderSize();
	
	box->min.Set(-cylinderSize.x, -cylinderSize.y, 0.0F);
	box->max.Set(cylinderSize.x, cylinderSize.y, object->GetCylinderHeight());
	return (true);
}

void CylinderZone::Preprocess(void)
{
	Zone::Preprocess();
	
	SetVisibilityProc(&BoxVisible);
}


DomeZone::DomeZone() : Zone(kZoneDome)
{
}

DomeZone::DomeZone(const Vector3D& size) : Zone(kZoneDome)
{
	SetNewObject(new DomeZoneObject(size));
}

DomeZone::DomeZone(const DomeZone& domeZone) : Zone(domeZone)
{
}

DomeZone::~DomeZone()
{
}

Node *DomeZone::Replicate(void) const
{
	return (new DomeZone(*this));
}

bool DomeZone::CalculateBoundingBox(Box3D *box) const
{
	const DomeZoneObject *object = GetObject();
	const Vector3D& domeSize = object->GetDomeSize();
	
	box->min.Set(-domeSize.x, -domeSize.y, 0.0F);
	box->max = domeSize;
	return (true);
}

void DomeZone::Preprocess(void)
{
	Zone::Preprocess();
	
	SetVisibilityProc(&BoxVisible);
}


PolygonZone::PolygonZone() : Zone(kZonePolygon)
{
}

PolygonZone::PolygonZone(const Vector2D& size, float height) : Zone(kZonePolygon)
{
	SetNewObject(new PolygonZoneObject(size, height));
}

PolygonZone::PolygonZone(const PolygonZone& polygonZone) : Zone(polygonZone)
{
}

PolygonZone::~PolygonZone()
{
}

Node *PolygonZone::Replicate(void) const
{
	return (new PolygonZone(*this));
}

bool PolygonZone::CalculateBoundingBox(Box3D *box) const
{
	PolygonZoneObject *object = GetObject();
	
	const Point3D *vertex = object->GetVertexArray();
	float xmin = vertex->x;
	float ymin = vertex->y;
	float xmax = xmin;
	float ymax = ymin;
	
	int32 count = object->GetVertexCount();
	for (machine a = 1; a < count; a++)
	{
		const Point3D& p = vertex[a];
		float x = p.x;
		float y = p.y;
		
		xmin = Fmin(xmin, x);
		xmax = Fmax(xmax, x);
		ymin = Fmin(ymin, y);
		ymax = Fmax(ymax, y);
	}
	
	box->min.Set(xmin, ymin, 0.0F);
	box->max.Set(xmax, ymax, object->GetPolygonHeight());
	return (true);
}

void PolygonZone::Preprocess(void)
{
	Zone::Preprocess();
	
	SetVisibilityProc(&BoxVisible);
}

// ZYURVUR
