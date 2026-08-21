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


#include "C4Cameras.h"
#include "C4Input.h"
#include "C4Engine.h"
#include "C4World.h"


using namespace C4;


namespace
{
	const float kSurfaceGuardEpsilon	= 0.002F;
	const float kSlideMagnitudeEpsilon	= 0.001F;
	const float kSlideTransferEpsilon	= 0.001F;
	const float kNegSlideProbeDepth		= -0.02F;
}


Camera::Camera(CameraType type) : Node(kNodeCamera)
{
	cameraType = type;
	
	SetActiveUpdateFlags(GetActiveUpdateFlags() & ~kUpdateBoundingSphere);
}

Camera::Camera(const Camera& camera) : Node(camera)
{
	cameraType = camera.cameraType;
	
	SetActiveUpdateFlags(GetActiveUpdateFlags() & ~kUpdateBoundingSphere);
}

Camera::~Camera()
{
}

Camera *Camera::Construct(Unpacker& data, unsigned_int32 unpackFlags)
{
	switch (data.GetType())
	{
		case kCameraOrtho:
			
			return (new OrthoCamera(true));
		
		case kCameraFrustum:
			
			return (new FrustumCamera);
		
		case kCameraRemote:
			
			return (new RemoteCamera);
	}
	
	return (nullptr);
}

void Camera::PackType(Packer& data) const
{
	Node::PackType(data);
	data << cameraType;
}

bool Camera::CalculateBoundingBox(Box3D *box) const
{
	if (GetManipulator())
	{
		box->min.Set(-0.5F, -0.5F, -0.5F);
		box->max.Set(0.5F, 0.5F, 0.5F);
		return (true);
	}
	
	return (false);
}

bool Camera::CalculateBoundingSphere(BoundingSphere *sphere) const
{
	if (GetManipulator())
	{
		sphere->SetCenter(0.0F, 0.0F, 0.0F);
		sphere->SetRadius(1.0F);
		return (true);
	}
	
	return (false);
}

void Camera::LookAtPoint(const Point3D& point)
{
	Vector3D view = (point - GetNodePosition()).Normalize();
	
	float x = view.x;
	float y = view.y;
	float f = InverseSqrt(x * x + y * y);
	Vector3D right(y * f, -x * f, 0.0F);
	
	Vector3D down = view % right;
	SetNodeMatrix3D(right, down, view);
}

void Camera::LookAtPoint(const Point3D& point, const Vector3D& up)
{
	Vector3D view = (point - GetNodePosition()).Normalize(); 
	Vector3D right = (view % up).Normalize();
	Vector3D down = view % right; 
	SetNodeMatrix3D(right, down, view); 
} 

void Camera::LookInDirection(const Vector3D& direction) 
{
	float x = direction.x;
	float y = direction.y;
	float f = InverseSqrt(x * x + y * y); 
	Vector3D right(y * f, -x * f, 0.0F);
	
	Vector3D down = direction % right;
	SetNodeMatrix3D(right, down, direction); 
}

void Camera::LookInDirection(const Vector3D& direction, const Vector3D& up)
{
	Vector3D right = (direction % up).Normalize();
	Vector3D down = direction % right;
	SetNodeMatrix3D(right, down, direction);
}

void Camera::Move(void)
{
}


OrthoCamera::OrthoCamera(bool) : Camera(kCameraOrtho)
{
}

OrthoCamera::OrthoCamera() : Camera(kCameraOrtho)
{
	SetNewObject(new OrthoCameraObject);
}

OrthoCamera::OrthoCamera(const OrthoCamera& orthoCamera) : Camera(orthoCamera)
{
}

OrthoCamera::~OrthoCamera()
{
}

Node *OrthoCamera::Replicate(void) const
{
	return (new OrthoCamera(*this));
}

void OrthoCamera::CastRay(float x, float y, Ray *ray) const
{
	const OrthoCameraObject *object = static_cast<OrthoCameraObject *>(GetObject());
	
	float left = object->GetOrthoRectLeft();
	float right = object->GetOrthoRectRight();
	float top = object->GetOrthoRectTop();
	float bottom = object->GetOrthoRectBottom();
	
	ray->origin.Set(left + x * (right - left), top + y * (bottom - top), 0.0F);
	ray->direction.Set(0.0F, 0.0F, 1.0F);
}

bool OrthoCamera::BoxVisible(const Point3D& center, const Vector3D *axis) const
{
	const OrthoCameraObject *object = static_cast<OrthoCameraObject *>(GetObject());
	
	Vector3D q = center - GetWorldPosition();
	
	const Vector3D& nx = GetWorldTransform()[0];
	float reff = Fabs(axis[0] * nx) + Fabs(axis[1] * nx) + Fabs(axis[2] * nx);
	
	float d = nx * q;
	if ((object->GetOrthoRectLeft() - d > reff) || (d - object->GetOrthoRectRight() > reff)) return (false);
	
	const Vector3D& ny = GetWorldTransform()[1];
	reff = Fabs(axis[0] * ny) + Fabs(axis[1] * ny) + Fabs(axis[2] * ny);
	
	d = ny * q;
	if ((object->GetOrthoRectTop() - d > reff) || (d - object->GetOrthoRectBottom() > reff)) return (false);
	
	const Vector3D& nz = GetWorldTransform()[2];
	reff = Fabs(axis[0] * nz) + Fabs(axis[1] * nz) + Fabs(axis[2] * nz);
	
	d = nz * q;
	if ((object->GetNearDepth() - d > reff) || (d - object->GetFarDepth() > reff)) return (false);
	
	return (true);
}

bool OrthoCamera::SphereVisible(const Point3D& center, float radius) const
{
	const OrthoCameraObject *object = static_cast<OrthoCameraObject *>(GetObject());
	
	Vector3D q = center - GetWorldPosition();
	
	float d = GetWorldTransform()[0] * q;
	if ((object->GetOrthoRectLeft() - d > radius) || (d - object->GetOrthoRectRight() > radius)) return (false);
	
	d = GetWorldTransform()[1] * q;
	if ((object->GetOrthoRectTop() - d > radius) || (d - object->GetOrthoRectBottom() > radius)) return (false);
	
	d = GetWorldTransform()[2] * q;
	if ((object->GetNearDepth() - d > radius) || (d - object->GetFarDepth() > radius)) return (false);
	
	return (true);
}

void OrthoCamera::CalculateOrthoCameraRegion(CameraRegion *region) const
{
	region->SetPlaneCount(6);
	region->SetNonlateralPlaneCount(2);
	region->SetShadowRegionFlags(0x01);
	
	const OrthoCameraObject *object = GetObject();
	
	Antivector4D *plane = region->GetPlaneArray();
	const Point3D& position = GetWorldPosition();
	
	const Vector3D& right = GetWorldTransform()[0];
	float d = right * position;
	
	plane[0].Set(right, -d - object->GetOrthoRectLeft());
	plane[1].Set(-right, d + object->GetOrthoRectRight());
	
	const Vector3D& down = GetWorldTransform()[1];
	d = down * position;
	
	plane[2].Set(down, -d - object->GetOrthoRectTop());
	plane[3].Set(-down, d + object->GetOrthoRectBottom());
	
	const Vector3D& view = GetWorldTransform()[2];
	d = view * position;
	
	plane[4].Set(view, -d - object->GetNearDepth());
	plane[5].Set(-view, d + object->GetFarDepth());
}


FrustumCamera::FrustumCamera() : Camera(kCameraFrustum)
{
	SetActiveUpdateFlags(GetActiveUpdateFlags() | kUpdatePostTransform);
}

FrustumCamera::FrustumCamera(CameraType type) : Camera(type)
{
	SetActiveUpdateFlags(GetActiveUpdateFlags() | kUpdatePostTransform);
}

FrustumCamera::FrustumCamera(float focal, float aspect) : Camera(kCameraFrustum)
{
	SetNewObject(new FrustumCameraObject(focal, aspect));
	SetActiveUpdateFlags(GetActiveUpdateFlags() | kUpdatePostTransform);
}

FrustumCamera::FrustumCamera(const FrustumCamera& frustumCamera) : Camera(frustumCamera)
{
	SetActiveUpdateFlags(GetActiveUpdateFlags() | kUpdatePostTransform);
}

FrustumCamera::~FrustumCamera()
{
}

Node *FrustumCamera::Replicate(void) const
{
	return (new FrustumCamera(*this));
}

void FrustumCamera::CalculatePostTransform(void)
{
	const FrustumCameraObject *object = GetObject();
	
	const Transform4D& transform = GetWorldTransform();
	const Vector3D& rightDirection = transform[0];
	const Vector3D& downDirection = transform[1];
	const Vector3D& viewDirection = transform[2];
	
	float e = object->GetFocalLength();
	float a = object->GetAspectRatio();
	
	float d = a * a + 1.0F;
	sineHalfField = Sqrt(d / (e * e + d));
	
	float g1 = InverseSqrt(e * e + 1.0F);
	float g2 = InverseSqrt(e * e + a * a);
	
	frustumPlaneNormal[0] = (viewDirection + rightDirection * e) * g1;
	frustumPlaneNormal[1] = (viewDirection - rightDirection * e) * g1;
	frustumPlaneNormal[2] = (viewDirection * a + downDirection * e) * g2;
	frustumPlaneNormal[3] = (viewDirection * a - downDirection * e) * g2;
	
	float n = object->GetNearDepth();
	float f = object->GetFarDepth();
	e = n / e;
	a *= e;
	
	frustumVertex[0] = transform * Point3D(e, -a, n);
	frustumVertex[1] = transform * Point3D(-e, -a, n);
	frustumVertex[2] = transform * Point3D(-e, a, n);
	frustumVertex[3] = transform * Point3D(e, a, n);
	
	nearPlaneCenter = transform.GetTranslation() + transform[2] * n;
	
	float m = f / n;
	e *= m;
	a *= m;
	
	frustumVertex[4] = transform * Point3D(e, -a, f);
	frustumVertex[5] = transform * Point3D(-e, -a, f);
	frustumVertex[6] = transform * Point3D(-e, a, f);
	frustumVertex[7] = transform * Point3D(e, a, f);
}

void FrustumCamera::CastRay(float x, float y, Ray *ray) const
{
	const FrustumCameraObject *object = GetObject();
	
	ray->origin.Set(0.0F, 0.0F, 0.0F);
	ray->direction.Set(2.0F * x - 1.0F, (2.0F * y - 1.0F) * object->GetAspectRatio(), object->GetFocalLength());
}

bool FrustumCamera::BoxVisible(const Point3D& center, const Vector3D *axis) const
{
	const FrustumCameraObject *object = GetObject();
	
	Vector3D q = center - GetWorldPosition();
	const Vector3D& n = GetWorldTransform()[2];
	float reff = Fabs(axis[0] * n) + Fabs(axis[1] * n) + Fabs(axis[2] * n);
	
	float d = n * q;
	if (object->GetNearDepth() - d > reff) return (false);
	
	for (machine a = 0; a < 4; a++)
	{
		const Vector3D& normal = frustumPlaneNormal[a];
		reff = Fnabs(axis[0] * normal) + Fnabs(axis[1] * normal) + Fnabs(axis[2] * normal);
		if (normal * q < reff) return (false);
	}
	
	return (true);
}

bool FrustumCamera::DirectionVisible(const Vector3D& direction, float radius) const
{
	#if C4SIMD
	
		float4 r = SimdXor(SimdLoadScalar(&radius), SimdGetNegativeZero());
		float4 d = SimdLoadUnaligned(&direction.x);
		
		for (machine a = 0; a < 4; a++)
		{
			float4 plane = SimdLoadUnaligned(&frustumPlaneNormal[a].x);
			if (SimdCmpltScalar(SimdDot3D(plane, d), r)) return (false);
		}
	
	#else
	
		radius = -radius;
		for (machine a = 0; a < 4; a++)
		{
			if (frustumPlaneNormal[a] * direction < radius) return (false);
		}
	
	#endif
	
	return (true);
}

bool FrustumCamera::SphereVisible(const Point3D& center, float radius) const
{
	const FrustumCameraObject *object = GetObject();
	
	#if C4SIMD
	
		float4 r = SimdXor(SimdLoadScalar(&radius), SimdGetNegativeZero());
		float4 q = SimdSub(SimdLoadUnaligned(&center.x), SimdLoadUnaligned(&GetWorldPosition().x));
		
		float4 d = SimdDot3D(SimdLoadUnaligned(&GetWorldTransform()[2].x), q);
		d = SimdSubScalar(d, SimdLoadScalar(&object->GetNearDepth()));
		if (SimdCmpltScalar(d, r)) return (false);
		
		for (machine a = 0; a < 4; a++)
		{
			float4 plane = SimdLoadUnaligned(&frustumPlaneNormal[a].x);
			if (SimdCmpltScalar(SimdDot3D(plane, q), r)) return (false);
		}
	
	#else
	
		Vector3D q = center - GetWorldPosition();
		float d = GetWorldTransform()[2] * q;
		radius = -radius;
		
		if (d - object->GetNearDepth() < radius) return (false);
		for (machine a = 0; a < 4; a++)
		{
			if (frustumPlaneNormal[a] * q < radius) return (false);
		}
	
	#endif
	
	return (true);
}

void FrustumCamera::CalculateFrustumCameraRegion(CameraRegion *region) const
{
	region->SetPlaneCount(5);
	region->SetNonlateralPlaneCount(1);
	region->SetShadowRegionFlags(0x01);
	
	Antivector4D *plane = region->GetPlaneArray();
	const Point3D& position = GetWorldPosition();
	
	for (machine a = 0; a < 4; a++) plane[a].Set(frustumPlaneNormal[a], position);
	
	const Vector3D& direction = GetWorldTransform()[2];
	plane[4].Set(direction, -(direction * position) - GetObject()->GetNearDepth());
}


RemoteCamera::RemoteCamera() : FrustumCamera(kCameraRemote)
{
}

RemoteCamera::RemoteCamera(float focal, float aspect, const Transform4D& transform, const Antivector4D& clipPlane) : FrustumCamera(kCameraRemote)
{
	SetNewObject(new RemoteCameraObject(focal, aspect, transform, clipPlane));
}

RemoteCamera::RemoteCamera(const RemoteCamera& remoteCamera) : FrustumCamera(remoteCamera)
{
}

RemoteCamera::~RemoteCamera()
{
}

Node *RemoteCamera::Replicate(void) const
{
	return (new RemoteCamera(*this));
}

void RemoteCamera::CalculateWorldTransform(void)
{
	RemoteCameraObject *object = static_cast<RemoteCameraObject *>(GetObject());
	const Transform4D& remoteTransform = object->GetRemoteTransform();
	
	Node *node = GetSuperNode();
	if (node) SetWorldTransform(remoteTransform * node->GetWorldTransform() * GetNodeTransform());
	else SetWorldTransform(remoteTransform * GetNodeTransform());
}

void RemoteCamera::CalculatePostTransform(void)
{
	FrustumCamera::CalculatePostTransform();
	
	if (GetObject()->GetRemoteDeterminant() < 0.0F)
	{
		Point3D *vertex = GetFrustumVertexArray();
		
		Point3D t1 = vertex[1];
		vertex[1] = vertex[3];
		vertex[3] = t1;
		
		Point3D t2 = vertex[5];
		vertex[5] = vertex[7];
		vertex[7] = t2;
	}
}

void RemoteCamera::CalculateRemoteCameraRegion(int32 vertexCount, const Point3D *vertex, CameraRegion *region)
{
	RemoteCameraObject *object = GetObject();
	
	float xmin = K::infinity;
	float xmax = K::minus_infinity;
	float ymin = K::infinity;
	float ymax = K::minus_infinity;
	
	float focal = object->GetFocalLength();
	const Transform4D& remoteTransform = object->GetRemoteTransform();
	const Transform4D& cameraTransform = GetInverseWorldTransform();
	
	remoteVertexCount = vertexCount;
	remoteCenter.Set(0.0F, 0.0F, 0.0F);
	
	for (machine a = 0; a < vertexCount; a++)
	{
		remoteVertex[a] = remoteTransform * vertex[a];
		remoteCenter += remoteVertex[a];
		
		Point3D v = cameraTransform * remoteVertex[a];
		float g = focal / v.z;
		float x = v.x * g;
		float y = v.y * g;
		
		xmin = Fmin(xmin, x);
		xmax = Fmax(xmax, x);
		ymin = Fmin(ymin, y);
		ymax = Fmax(ymax, y);
	}
	
	object->SetFrustumBoundary(xmin, xmax, ymin, ymax);
	remoteCenter /= (float) vertexCount;
	
	Antivector4D *plane = region->GetPlaneArray();
	const Point3D& cameraPosition = GetWorldPosition();
	float m = object->GetRemoteDeterminant();
	
	Vector3D v1 = remoteVertex[vertexCount - 1] - cameraPosition;
	for (machine a = 0; a < vertexCount; a++)
	{
		Vector3D v2 = remoteVertex[a] - cameraPosition;
		Antivector3D normal = v2 % v1;
		normal *= InverseMag(normal) * m;
		plane->Set(normal, cameraPosition);
		
		v1 = v2;
		plane++;
	}
	
	const Vector3D& cameraDirection = GetWorldTransform()[2];
	plane[0].Set(cameraDirection, -(cameraDirection * cameraPosition) - object->GetNearDepth());
	plane[1] = object->GetRemoteClipPlane();
	
	region->SetPlaneCount(vertexCount + 2);
	region->SetNonlateralPlaneCount(2);
	region->SetPortalExcludePlaneCount(1);
	region->SetShadowRegionFlags(0x01);
}


OrientedCamera::OrientedCamera(float focal, float aspect) : FrustumCamera(focal, aspect)
{
	cameraAzimuth = 0.0F;
	cameraAltitude = 0.0F;
}

OrientedCamera::~OrientedCamera()
{
}

void OrientedCamera::Move(void)
{
	Vector3D	v;
	
	v.y = 0.0F;
	CosSin(cameraAltitude, &v.x, &v.z);
	v.RotateAboutZ(cameraAzimuth);
	LookInDirection(v);
}


SpectatorCamera::SpectatorCamera(float focal, float aspect, float radius) : OrientedCamera(focal, aspect)
{
	spectatorFlags = 0;
	
	spectatorRadius = radius;
	spectatorSpeed = 0.0F;
	
	collisionOffset.Set(0.0F, 0.0F, 0.0F);
}

SpectatorCamera::~SpectatorCamera()
{
}

Vector3D SpectatorCamera::CalculateGuardOffset(unsigned_int32 flags) const
{
	Vector3D	totalOffset;
	
	if (flags & kSpectatorSlide1)
	{
		if (flags & kSpectatorSlide2)
		{
			float d = slideNormal[0] * slideNormal[1];
			if (d < 0.0F)
			{
				Vector3D n1 = slideNormal[0] - ProjectOnto(slideNormal[0], slideNormal[1]);
				Vector3D n2 = slideNormal[1] - ProjectOnto(slideNormal[1], slideNormal[0]);
				Vector3D offset = n1 + n2;
				
				float m = SquaredMag(offset);
				if (m > kSlideMagnitudeEpsilon) totalOffset = offset * (K::sqrt_2 * kSurfaceGuardEpsilon * InverseSqrt(m));
				else totalOffset = slideNormal[0] * kSurfaceGuardEpsilon;
			}
			else
			{
				Vector3D offset = slideNormal[0] + slideNormal[1];
				float m = K::sqrt_2 - (K::sqrt_2 - 1.0F) * d;
				totalOffset = offset * (kSurfaceGuardEpsilon * m * InverseMag(offset));
			}
		}
		else
		{
			totalOffset = slideNormal[0] * kSurfaceGuardEpsilon;
		}
	}
	else
	{
		totalOffset = slideNormal[1] * kSurfaceGuardEpsilon;
	}
	
	return (totalOffset);
}

bool SpectatorCamera::TestSlide(unsigned_int32 flags, int32 index) const
{
	CollisionData	data;
	
	const Vector3D& normal = slideNormal[index];
	float radius = spectatorRadius;
	
	Point3D position = GetNodePosition() + collisionOffset + normal * radius;
	Vector3D delta = normal * (kNegSlideProbeDepth - radius);
	
	if (index == 0)
	{
		if (flags & kSpectatorSlide2) delta -= ProjectOnto(delta, slideNormal[1]);
	}
	else
	{
		if (flags & kSpectatorSlide1) delta -= ProjectOnto(delta, slideNormal[0]);
	}
	
	return (GetWorld()->DetectCollision(position, position + delta, spectatorRadius, kCollisionCamera, &data));
}

void SpectatorCamera::Move(void)
{
	unsigned_int32 flags = spectatorFlags;
	if ((flags & kSpectatorSlide1) && (!TestSlide(flags, 0))) flags &= ~kSpectatorSlide1;
	if ((flags & kSpectatorSlide2) && (!TestSlide(flags, 1))) flags &= ~kSpectatorSlide2;
	
	if ((flags & kSpectatorMoveMask) != 0)
	{
		CollisionData	data;
		Vector3D		guardOffset;
		Vector3D		guardDelta;
		
		const Vector3D& view = GetNodeTransform()[2];
		const Vector3D& right = GetNodeTransform()[0];
		
		Vector3D delta(0.0F, 0.0F, 0.0F);
		
		float t = TheTimeMgr->GetFloatDeltaTime();
		spectatorSpeed = Fmin(spectatorSpeed + t * 1.0e-5F, 0.05F);
		t *= spectatorSpeed;
		
		if (flags & kSpectatorMoveForward) delta += view * t;
		if (flags & kSpectatorMoveBackward) delta -= view * t;
		if (flags & kSpectatorMoveRight) delta += right * t;
		if (flags & kSpectatorMoveLeft) delta -= right * t;
		if (flags & kSpectatorMoveUp) delta.z += t;
		if (flags & kSpectatorMoveDown) delta.z -= t;
		
		if ((flags & (kSpectatorSlide1 | kSpectatorSlide2)) != 0)
		{
			guardOffset = CalculateGuardOffset(flags);
			guardDelta = guardOffset - collisionOffset;
			
			bool slide1 = ((flags & kSpectatorSlide1) && (delta * slideNormal[0] < 0.0F));
			bool slide2 = ((flags & kSpectatorSlide2) && (delta * slideNormal[1] < 0.0F));
			
			if (slide1)
			{
				if (slide2)
				{
					Vector3D planeNormal = slideNormal[0] % slideNormal[1];
					float m = SquaredMag(planeNormal);
					if (m > kSlideMagnitudeEpsilon) delta = planeNormal * (delta * planeNormal / m);
				}
				else
				{
					delta -= ProjectOnto(delta, slideNormal[0]);
				}
			}
			else if (slide2)
			{
				delta -= ProjectOnto(delta, slideNormal[1]);
			}
		}
		else
		{
			guardOffset.Set(0.0F, 0.0F, 0.0F);
			guardDelta = -collisionOffset;
		}
		
		const Point3D& p1 = GetNodePosition();
		Point3D p2 = p1 + delta + guardOffset;
		if (GetWorld()->DetectCollision(p1 + collisionOffset, p2, spectatorRadius, kCollisionCamera, &data))
		{
			float mag = Magnitude(delta);
			const Vector3D& normal = data.normal;
			float t = FmaxZero(data.param);
			
			if (flags & kSpectatorSlide1)
			{
				if ((normal * slideNormal[0] > kSlideTransferEpsilon) && (delta * slideNormal[0] > mag * kSlideTransferEpsilon))
				{
					slideNormal[0] = normal;
				}
				else if (flags & kSpectatorSlide2)
				{
					if ((normal * slideNormal[1] > kSlideTransferEpsilon) && (delta * slideNormal[1] > mag * kSlideTransferEpsilon))
					{
						slideNormal[1] = normal;
					}
					else
					{
						t = 0.0F;
					}
				}
				else
				{
					flags |= kSpectatorSlide2;
					slideNormal[1] = normal;
				}
			}
			else
			{
				if ((flags & kSpectatorSlide2) && (normal * slideNormal[1] > kSlideTransferEpsilon) && (delta * slideNormal[1] > mag * kSlideTransferEpsilon))
				{
					slideNormal[1] = normal;
				}
				else
				{
					flags |= kSpectatorSlide1;
					slideNormal[0] = normal;
				}
			}
			
			SetNodePosition(p1 + delta * t);
			collisionOffset += guardDelta * t;
		}
		else
		{
			SetNodePosition(p1 + delta);
			collisionOffset = guardOffset;
		}
	}
	else
	{
		spectatorSpeed = 0.0F;
	}
	
	spectatorFlags = flags;
	
	float azimuth = GetCameraAzimuth();
	azimuth += TheInputMgr->GetMouseDeltaX();
	if (azimuth < -K::pi) azimuth += K::two_pi;
	else if (azimuth > K::pi) azimuth -= K::two_pi;
	SetCameraAzimuth(azimuth);
	
	float altitude = GetCameraAltitude();
	altitude += TheInputMgr->GetMouseDeltaY();
	if (altitude < -1.45F) altitude = -1.45F;
	else if (altitude > 1.45F) altitude = 1.45F;
	SetCameraAltitude(altitude);
	
	OrientedCamera::Move();
}


BenchmarkCamera::BenchmarkCamera(float focal, float aspect) :
		OrientedCamera(focal, aspect),
		completionTask(&EndBenchmark, this)
{
	currentMarker = nullptr;
}

BenchmarkCamera::~BenchmarkCamera()
{
}

void BenchmarkCamera::Move(void)
{
	if (currentMarker)
	{
		SetCameraAzimuth((float) angleIndex * K::two_pi * K::one_over_256);
		
		angleIndex += (angleIndex < 0) ? 32 : 1;
		if (angleIndex > 0)
		{
			frameCount++;
			totalTime += TheTimeMgr->GetSystemDeltaTime();
		}
		
		if (angleIndex > 256) SetCurrentMarker(currentMarker->Next());
		
		OrientedCamera::Move();
	}
	else
	{
		TheTimeMgr->AddTask(&completionTask);
	}
}

void BenchmarkCamera::SetCurrentMarker(Marker *marker)
{
	currentMarker = marker;
	if (marker)
	{
		angleIndex = -256;
		marker->AddSubnode(this);
	}
}

void BenchmarkCamera::BeginBenchmark(World *world, Marker *marker)
{
	if (marker)
	{
		benchmarkWorld = world;
		worldCamera = world->GetCamera();
		world->SetCamera(this);
		
		SetCurrentMarker(marker);
		
		frameCount = 0;
		totalTime = 0;
	}
}

void BenchmarkCamera::EndBenchmark(DeferredTask *task, void *cookie)
{
	BenchmarkCamera *camera = static_cast<BenchmarkCamera *>(cookie);
	camera->GetWorld()->SetCamera(camera->worldCamera);
	
	String<159> text("<b>Benchmark:</b> ");
	text += TheDisplayMgr->GetDisplayWidth();
	text += "&nbsp;&times;&nbsp;";
	text += TheDisplayMgr->GetDisplayHeight();
	text += "&nbsp;&nbsp;<span style=\"background-color: #22EE66;\">&nbsp;";
	
	String<15> fps(Text::FloatToString((float) camera->frameCount / (float) camera->totalTime * 1000.0F));
	text += fps;
	text += " fps&nbsp;</span><br/><br/>\r\n";
	
	Engine::Report(text, kReportLog);
	Engine::Report(fps);
	
	delete camera;
}

// ZYURVUR
