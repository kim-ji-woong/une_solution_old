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


#include "C4CameraObjects.h"
#include "C4Configuration.h"
#include "C4Graphics.h"


using namespace C4;


CameraObject::CameraObject(CameraType type) : Object(kObjectCamera)
{
	cameraType = type;
	cameraFlags = 0;
	
	clearFlags = 0;
	clearColor.Set(0.0F, 0.0F, 0.0F, 0.0F);
}

CameraObject::~CameraObject()
{
}

CameraObject *CameraObject::Construct(Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (data.GetType())
	{
		case kCameraOrtho:
			
			return (new OrthoCameraObject);
		
		case kCameraFrustum:
			
			return (new FrustumCameraObject);
		
		case kCameraRemote:
			
			return (new RemoteCameraObject);
	}
	
	return (nullptr);
}

void CameraObject::PackType(Packer& data) const
{
	Object::PackType(data);
	data << cameraType;
}

void CameraObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	data << ChunkHeader('VIEW', sizeof(Rect) + 8);
	data << viewRect;
	data << nearDepth;
	data << farDepth;
	
	data << ChunkHeader('FLAG', 4);
	data << cameraFlags;
	
	data << ChunkHeader('CLER', 4 + sizeof(ColorRGBA));
	data << clearFlags;
	data << clearColor;
	
	data << TerminatorChunk;
}

void CameraObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	UnpackChunkList<CameraObject>(data, unpackFlags);
}

bool CameraObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'VIEW':
		{
			data >> viewRect;
			data >> nearDepth;
			data >> farDepth;
			return (true);
		}
		
		case 'FLAG':
			
			data >> cameraFlags;
			return (true);
		
		case 'CLER':
			
			data >> clearFlags;
			data >> clearColor;
			return (true);
	}
	
	return (false);
}

int32 CameraObject::GetCategoryCount(void) const
{
	return (1);
}

Type CameraObject::GetCategoryType(int32 index, const char **title) const 
{
	if (index == 0) 
	{ 
		*title = TheInterfaceMgr->GetStringTable()->GetString(StringID(kObjectCamera)); 
		return (kObjectCamera);
	} 
	
	return (0);
}
 
int32 CameraObject::GetCategorySettingCount(Type category) const
{
	if (category == kObjectCamera) return (2);
	return (0); 
}

Setting *CameraObject::GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const
{
	if (category == kObjectCamera)
	{
		const StringTable *table = TheInterfaceMgr->GetStringTable();
		
		if (index == 0)
		{
			const char *title = table->GetString(StringID(kObjectCamera, 'FLAG'));
			return (new HeadingSetting('FLAG', title));
		}
		
		if (index == 1)
		{
			if (flags & kConfigurationScript) return (nullptr);
			
			const char *title = table->GetString(StringID(kObjectCamera, 'FLAG', 'EXTN'));
			return (new BooleanSetting('EXTN', ((cameraFlags & kCameraExternalZone) != 0), title));
		}
	}
	
	return (nullptr);
}

void CameraObject::SetCategorySetting(Type category, const Setting *setting)
{
	if (category == kObjectCamera)
	{
		Type identifier = setting->GetSettingIdentifier();
		
		if (identifier == 'EXTN')
		{
			bool b = static_cast<const BooleanSetting *>(setting)->GetBooleanValue();
			if (b) cameraFlags |= kCameraExternalZone;
			else cameraFlags &= ~kCameraExternalZone;
		}
	}
}


OrthoCameraObject::OrthoCameraObject() : CameraObject(kCameraOrtho)
{
}

OrthoCameraObject::~OrthoCameraObject()
{
}

void OrthoCameraObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	CameraObject::Pack(data, packFlags);
	
	data << ChunkHeader('ORTH', 16);
	data << orthoLeft;
	data << orthoRight;
	data << orthoTop;
	data << orthoBottom;
	
	data << TerminatorChunk;
}

void OrthoCameraObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	CameraObject::Unpack(data, unpackFlags);
	UnpackChunkList<OrthoCameraObject>(data, unpackFlags);
}

bool OrthoCameraObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'ORTH':
		{
			data >> orthoLeft;
			data >> orthoRight;
			data >> orthoTop;
			data >> orthoBottom;
			return (true);
		}
	}
	
	return (false);
}

void OrthoCameraObject::Activate(void) const
{
	TheGraphicsMgr->SetOrtho(GetViewRect(), orthoLeft, orthoRight, orthoTop, orthoBottom, GetNearDepth(), GetFarDepth());
}


FrustumCameraObject::FrustumCameraObject() : CameraObject(kCameraFrustum)
{
}

FrustumCameraObject::FrustumCameraObject(CameraType type) : CameraObject(type)
{
}

FrustumCameraObject::FrustumCameraObject(CameraType type, float focal, float aspect) : CameraObject(type)
{
	frustumFlags = 0;
	focalLength = focal;
	aspectRatio = aspect;
}

FrustumCameraObject::FrustumCameraObject(float focal, float aspect) : CameraObject(kCameraFrustum)
{
	frustumFlags = 0;
	focalLength = focal;
	aspectRatio = aspect;
}

FrustumCameraObject::~FrustumCameraObject()
{
}

void FrustumCameraObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	CameraObject::Pack(data, packFlags);
	
	data << ChunkHeader('FRUS', 12);
	data << frustumFlags;
	data << focalLength;
	data << aspectRatio;
	
	data << TerminatorChunk;
}

void FrustumCameraObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	CameraObject::Unpack(data, unpackFlags);
	UnpackChunkList<FrustumCameraObject>(data, unpackFlags);
}

bool FrustumCameraObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'FRUS':
		{
			data >> frustumFlags;
			data >> focalLength;
			data >> aspectRatio;
			return (true);
		}
	}
	
	return (false);
}

int32 FrustumCameraObject::GetObjectSize(float *size) const
{
	size[0] = GetFarDepth();
	size[1] = focalLength;
	return (2);
}

void FrustumCameraObject::SetObjectSize(const float *size)
{
	SetFarDepth(size[0]);
	focalLength = size[1];
}

void FrustumCameraObject::Activate(void) const
{
	TheGraphicsMgr->SetFrustum(GetViewRect(), focalLength, aspectRatio, GetNearDepth(), GetFarDepth(), frustumFlags);
}

ProjectionResult FrustumCameraObject::ProjectSphere(const Point3D& center, float radius, ProjectionRect *rect) const
{
	float cx = center.x;
	float cy = center.y;
	float cz = center.z;
	float r2 = radius * radius;
	
	float cx2 = cx * cx;
	float cy2 = cy * cy;
	float cz2 = cz * cz;
	float cxz2 = cx2 + cz2;
	if (cxz2 + cy2 > r2)
	{
		float left = -1.0F;
		float right = 1.0F;
		float bottom = -1.0F;
		float top = 1.0F;
		
		float rcz = 1.0F / cz;
		
		float dx = r2 * cx2 - cxz2 * (r2 - cz2);
		if (dx > 0.0F)
		{
			dx = Sqrt(dx);
			float ax = 1.0F / cxz2;
			float bx = radius * cx;
			
			float nx1 = (bx + dx) * ax;
			float nx2 = (bx - dx) * ax;
			
			float nz1 = (radius - nx1 * cx) * rcz;
			float nz2 = (radius - nx2 * cx) * rcz;
			
			float pz1 = cz - radius * nz1;
			float pz2 = cz - radius * nz2;
			
			if (pz1 < 0.0F)
			{
				float x = nz1 * focalLength / nx1;
				if (nx1 > 0.0F) left = Fmax(left, x);
				else right = Fmin(right, x);
			}
			
			if (pz2 < 0.0F)
			{
				float x = nz2 * focalLength / nx2;
				if (nx2 > 0.0F) left = Fmax(left, x);
				else right = Fmin(right, x);
			}
		}
		
		float cyz2 = cy2 + cz2;
		float dy = r2 * cy2 - cyz2 * (r2 - cz2);
		if (dy > 0.0F)
		{
			dy = Sqrt(dy);
			float ay = 1.0F / cyz2;
			float by = radius * cy;
			
			float ny1 = (by + dy) * ay;
			float ny2 = (by - dy) * ay;
			
			float nz1 = (radius - ny1 * cy) * rcz;
			float nz2 = (radius - ny2 * cy) * rcz;
			
			float pz1 = cz - radius * nz1;
			float pz2 = cz - radius * nz2;
			
			if (pz1 < 0.0F)
			{
				float y = nz1 * focalLength / (ny1 * aspectRatio);
				if (ny1 > 0.0F) bottom = Fmax(bottom, y);
				else top = Fmin(top, y);
			}
			
			if (pz2 < 0.0F)
			{
				float y = nz2 * focalLength / (ny2 * aspectRatio);
				if (ny2 > 0.0F) bottom = Fmax(bottom, y);
				else top = Fmin(top, y);
			}
		}
		
		if ((!(left < right)) || (!(bottom < top))) return (kProjectionEmpty);
		
		rect->left = left;
		rect->right = right;
		rect->bottom = bottom;
		rect->top = top;
		
		return (kProjectionPartial);
	}
	
	return (kProjectionFull);
}


RemoteCameraObject::RemoteCameraObject() : FrustumCameraObject(kCameraRemote)
{
}

RemoteCameraObject::RemoteCameraObject(float focal, float aspect, const Transform4D& transform, const Antivector4D& clipPlane) : FrustumCameraObject(kCameraRemote, focal, aspect)
{
	SetRemoteTransform(transform);
	SetRemoteClipPlane(clipPlane);
}

RemoteCameraObject::~RemoteCameraObject()
{
}

void RemoteCameraObject::Pack(Packer& data, unsigned_int32 packFlags) const
{
	FrustumCameraObject::Pack(data, packFlags);
	
	data << ChunkHeader('RMOT', sizeof(Transform4D));
	data << remoteTransform;
	
	data << TerminatorChunk;
}

void RemoteCameraObject::Unpack(Unpacker& data, unsigned_int32 unpackFlags)
{
	FrustumCameraObject::Unpack(data, unpackFlags);
	UnpackChunkList<RemoteCameraObject>(data, unpackFlags);
}

bool RemoteCameraObject::UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (chunkHeader->chunkType)
	{
		case 'RMOT':
		{
			Transform4D		transform;
			
			data >> transform;
			SetRemoteTransform(transform);
			return (true);
		}
	}
	
	return (false);
}

void RemoteCameraObject::SetRemoteTransform(const Transform4D& transform)
{
	remoteTransform = transform;
	inverseRemoteTransform = Inverse(transform);
	remoteDeterminant = (Determinant(transform) < 0.0F) ? -1.0F : 1.0F;
}

void RemoteCameraObject::SetFrustumBoundary(float left, float right, float top, float bottom)
{
	frustumBoundary.left = Fmax(left, -1.0F);
	frustumBoundary.right = Fmin(right, 1.0F);
	
	float a = 1.0F / GetAspectRatio();
	frustumBoundary.bottom = Fmax(bottom * -a, -1.0F);
	frustumBoundary.top = Fmin(top * -a, 1.0F);
}

void RemoteCameraObject::Activate(void) const
{
	TheGraphicsMgr->SetSubfrustum(GetViewRect(), frustumBoundary, GetFocalLength(), GetAspectRatio(), GetNearDepth(), GetFarDepth(), GetFrustumFlags(), remoteClipPlane);
}

// ZYURVUR
