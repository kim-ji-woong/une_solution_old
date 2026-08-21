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


#ifndef C4CameraObjects_h
#define C4CameraObjects_h


//# \component	Graphics Manager
//# \prefix		GraphicsMgr/


#include "C4Objects.h"


namespace C4
{
	typedef Type	CameraType;
	
	
	//# \enum	CameraType
	
	enum
	{
		kCameraOrtho		= 'ORTH',	//## Orthographic camera.
		kCameraFrustum		= 'FRUS',	//## Frustum (perspective) camera.
		kCameraRemote		= 'REMO'	//## Remote camera.
	};
	
	
	//# \enum	CameraFlags
	
	enum
	{
		kCameraExternalZone		= 1 << 0	//## The camera's position may temporarily be outside its owning zone.
	};
	
	
	//# \enum	ClearFlags
	
	enum
	{
		kClearColorBuffer		= 1 << 0,	//## Clear the color buffer.
		kClearDepthBuffer		= 1 << 1,	//## Clear the depth buffer.
		kClearStencilBuffer		= 1 << 2	//## Clear the stencil buffer.
	};
	
	
	//# \enum	FrustumFlags
	
	enum
	{
		kFrustumInfinite		= 1 << 0,	//## Frustum has an infinite far plane.
		kFrustumOblique			= 1 << 1	//## Frustum has an oblique near plane.
	};
	
	
	enum ProjectionResult
	{
		kProjectionFull,
		kProjectionPartial,
		kProjectionEmpty
	};
	
	
	struct ProjectionRect
	{
		float		left;
		float		right;
		float		bottom;
		float		top;
		
		ProjectionRect& Set(float l, float r, float b, float t)
		{
			left = l;
			right = r;
			bottom = b;
			top = t;
			return (*this);
		}
		
		ProjectionRect& operator |=(const ProjectionRect& rect)
		{
			left = Fmin(left, rect.left);
			right = Fmax(right, rect.right);
			bottom = Fmin(bottom, rect.bottom);
			top = Fmax(top, rect.top);
			return (*this);
		}
	};
	
	
	//# \class	CameraObject		Encapsulates data pertaining to a camera.
	//
	//# The $CameraObject$ class encapsulates data pertaining to a camera.
	//
	//# \def	class CameraObject : public Object
	//
	//# \ctor	CameraObject(CameraType type);
	//
	//# The constructor has protected access. The $CameraObject$ class can only exist as the base class for another class.
	//
	//# \param	type	The type of the camera. See below for a list of possible types.
	//
	//# \desc
	//# 
	//
	//# \table	CameraType 
	// 
	//# \base	WorldMgr/Object		A $CameraObject$ is an object that can be shared by multiple camera nodes. 
	//
	//# \also	$@WorldMgr/Camera@$ 
	
	
	//# \function	CameraObject::GetCameraType		Returns the type of a camera.
	// 
	//# \proto	CameraType GetCameraType(void) const;
	//
	//# \desc
	//# The $GetCameraType$ function returns the camera type, which may be one of the following values or may 
	//# be a type defined by a derived class.
	//
	//# \table	CameraType
	
	
	//# \function	CameraObject::GetViewRect		Returns the view rectangle.
	//
	//# \proto	const Rect& GetViewRect(void) const;
	//
	//# \desc
	//
	//# \also	$@CameraObject::SetViewRect@$
	
	
	//# \function	CameraObject::SetViewRect		Sets the view rectangle.
	//
	//# \proto	void SetViewRect(const Rect& rect);
	//
	//# \param	rect	The new view rectangle.
	//
	//# \desc
	//
	//# \also	$@CameraObject::GetViewRect@$
	
	
	//# \function	CameraObject::GetNearDepth		Returns the near plane depth.
	//
	//# \proto	const float& GetNearDepth(void) const;
	//
	//# \desc
	//
	//# \also	$@CameraObject::SetNearDepth@$
	
	
	//# \function	CameraObject::SetNearDepth		Sets the near plane depth.
	//
	//# \proto	void SetNearDepth(float depth);
	//
	//# \param	depth	The new near plane depth.
	//
	//# \desc
	//
	//# \also	$@CameraObject::GetNearDepth@$
	
	
	//# \function	CameraObject::GetFarDepth		Returns the far plane depth.
	//
	//# \proto	const float& GetFarDepth(void) const;
	//
	//# \desc
	//
	//# \also	$@CameraObject::SetFarDepth@$
	
	
	//# \function	CameraObject::SetFarDepth		Sets the far plane depth.
	//
	//# \proto	void SetFarDepth(float depth);
	//
	//# \param	depth	The new far plane depth.
	//
	//# \desc
	//
	//# \also	$@CameraObject::GetFarDepth@$
	
	
	//# \function	CameraObject::GetClearFlags		Returns the clear flags.
	//
	//# \proto	unsigned_int32 GetClearFlags(void) const;
	//
	//# \desc
	//
	//# \table	ClearFlags
	//
	//# \also	$@CameraObject::SetClearFlags@$
	
	
	//# \function	CameraObject::SetClearFlags		Sets the clear flags.
	//
	//# \proto	void SetClearFlags(unsigned_int32 flags);
	//
	//# \param	flags	The new clear flags.
	//
	//# \desc
	//
	//# \table	ClearFlags
	//
	//# \also	$@CameraObject::GetClearFlags@$
	
	
	//# \function	CameraObject::GetClearColor		Returns the clear color.
	//
	//# \proto	const ColorRGBA& GetClearColor(void) const;
	//
	//# \desc
	//
	//# \also	$@CameraObject::SetClearColor@$
	
	
	//# \function	CameraObject::SetClearColor		Sets the clear color.
	//
	//# \proto	void SetClearColor(const ColorRGBA& color);
	//
	//# \param	color	The new clear color.
	//
	//# \desc
	//
	//# \also	$@CameraObject::GetClearColor@$
	
	
	class CameraObject : public Object
	{
		friend class Object;
		
		private:
			
			CameraType			cameraType;
			
			Rect				viewRect;
			float				nearDepth;
			float				farDepth;
			
			unsigned_int32		cameraFlags;
			unsigned_int32		clearFlags;
			ColorRGBA			clearColor;
			
			static CameraObject *Construct(Unpacker& data, unsigned_int32 unpackFlags);
		
		protected:
			
			CameraObject(CameraType type);
			virtual ~CameraObject();
		
		public:
			
			CameraType GetCameraType(void) const
			{
				return (cameraType);
			}
			
			const Rect& GetViewRect(void) const
			{
				return (viewRect);
			}
			
			void SetViewRect(const Rect& rect)
			{
				viewRect = rect;
			}
			
			const float& GetNearDepth(void) const
			{
				return (nearDepth);
			}
			
			void SetNearDepth(float depth)
			{
				nearDepth = depth;
			}
			
			const float& GetFarDepth(void) const
			{
				return (farDepth);
			}
			
			void SetFarDepth(float depth)
			{
				farDepth = depth;
			}
			
			unsigned_int32 GetCameraFlags(void) const
			{
				return (cameraFlags);
			}
			
			void SetCameraFlags(unsigned_int32 flags)
			{
				cameraFlags = flags;
			}
			
			unsigned_int32 GetClearFlags(void) const
			{
				return (clearFlags);
			}
			
			void SetClearFlags(unsigned_int32 flags)
			{
				clearFlags = flags;
			}
			
			const ColorRGBA& GetClearColor(void) const
			{
				return (clearColor);
			}
			
			void SetClearColor(const ColorRGBA& color)
			{
				clearColor = color;
			}
			
			void PackType(Packer& data) const;
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetCategoryCount(void) const;
			Type GetCategoryType(int32 index, const char **title) const;
			int32 GetCategorySettingCount(Type category) const;
			Setting *GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const;
			void SetCategorySetting(Type category, const Setting *setting);
			
			virtual void Activate(void) const = 0;
	};
	
	
	//# \class	OrthoCameraObject		Encapsulates data pertaining to a camera using an orthographic projection.
	//
	//# The $OrthoCameraObject$ class encapsulates data pertaining to a camera using an orthographic projection.
	//
	//# \def	class OrthoCameraObject : public CameraObject
	//
	//# \ctor	OrthoCameraObject();
	//
	//# \desc
	//# 
	//# \base	CameraObject	An $OrthoCameraObject$ is a specific type of $CameraObject$.
	//
	//# \also	$@WorldMgr/OrthoCamera@$
	
	
	//# \function	OrthoCameraObject::GetOrthoLeft		Returns the left edge of the orthographic view.
	//
	//# \proto	float GetOrthoRectLeft(void) const;
	//
	//# \desc
	//
	//# \also	$@OrthoCameraObject::GetOrthoRight@$
	//# \also	$@OrthoCameraObject::GetOrthoTop@$
	//# \also	$@OrthoCameraObject::GetOrthoBottom@$
	//# \also	$@OrthoCameraObject::SetOrthoRect@$
	
	
	//# \function	OrthoCameraObject::GetOrthoRight	Returns the right edge of the orthographic view.
	//
	//# \proto	float GetOrthoRight(void) const;
	//
	//# \desc
	//
	//# \also	$@OrthoCameraObject::GetOrthoLeft@$
	//# \also	$@OrthoCameraObject::GetOrthoTop@$
	//# \also	$@OrthoCameraObject::GetOrthoBottom@$
	//# \also	$@OrthoCameraObject::SetOrthoRect@$
	
	
	//# \function	OrthoCameraObject::GetOrthoTop		Returns the top edge of the orthographic view.
	//
	//# \proto	float GetOrthoTop(void) const;
	//
	//# \desc
	//
	//# \also	$@OrthoCameraObject::GetOrthoLeft@$
	//# \also	$@OrthoCameraObject::GetOrthoRight@$
	//# \also	$@OrthoCameraObject::GetOrthoBottom@$
	//# \also	$@OrthoCameraObject::SetOrthoRect@$
	
	
	//# \function	OrthoCameraObject::GetOrthoBottom	Returns the bottom edge of the orthographic view.
	//
	//# \proto	float GetOrthoBottom(void) const;
	//
	//# \desc
	//
	//# \also	$@OrthoCameraObject::GetOrthoLeft@$
	//# \also	$@OrthoCameraObject::GetOrthoRight@$
	//# \also	$@OrthoCameraObject::GetOrthoTop@$
	//# \also	$@OrthoCameraObject::SetOrthoRect@$
	
	
	//# \function	OrthoCameraObject::SetOrthoRect		Sets the edges of the orthographic view.
	//
	//# \proto	void SetOrthoRect(float left, float right, float top, float bottom);
	//
	//# \param	left	The left edge of the orthographic view.
	//# \param	right	The right edge of the orthographic view.
	//# \param	top		The top edge of the orthographic view.
	//# \param	bottom	The bottom edge of the orthographic view.
	//
	//# \desc
	//
	//# \also	$@OrthoCameraObject::GetOrthoLeft@$
	//# \also	$@OrthoCameraObject::GetOrthoRight@$
	//# \also	$@OrthoCameraObject::GetOrthoTop@$
	//# \also	$@OrthoCameraObject::GetOrthoBottom@$
	
	
	class OrthoCameraObject : public CameraObject
	{
		friend class CameraObject;
		
		private:
			
			float		orthoLeft;
			float		orthoRight;
			float		orthoTop;
			float		orthoBottom;
			
			~OrthoCameraObject();
		
		public:
			
			C4API OrthoCameraObject();
			
			float GetOrthoRectLeft(void) const
			{
				return (orthoLeft);
			}
			
			float GetOrthoRectRight(void) const
			{
				return (orthoRight);
			}
			
			float GetOrthoRectTop(void) const
			{
				return (orthoTop);
			}
			
			float GetOrthoRectBottom(void) const
			{
				return (orthoBottom);
			}
			
			void SetOrthoRect(float left, float right, float top, float bottom)
			{
				orthoLeft = left;
				orthoRight = right;
				orthoTop = top;
				orthoBottom = bottom;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			void Activate(void) const;
	};
	
	
	//# \class	FrustumCameraObject		Encapsulates data pertaining to a camera using a perspective projection.
	//
	//# The $FrustumCameraObject$ class encapsulates data pertaining to a camera using a perspective projection.
	//
	//# \def	class FrustumCameraObject : public CameraObject
	//
	//#	\ctor	FrustumCameraObject(CameraType type);
	//# \ctor	FrustumCameraObject(CameraType type, float focal, float aspect);
	//# \ctor	FrustumCameraObject(float focal, float aspect);
	//
	//# \param	type		The camera type. This is only used when constructing a $FrustumCameraObject$ as
	//#						a base class for another camera object type.
	//# \param	focal		The focal length of the perspective projection. This determines the field of view.
	//# \param	aspect		The aspect ratio of the projection. This is normally the ratio between the height
	//#						and the width of the viewport.
	//
	//# \desc
	//# 
	//
	//# \base	CameraObject	A $FrustumCameraObject$ is a specific type of $CameraObject$.
	//
	//# \also	$@WorldMgr/FrustumCamera@$
	
	
	//# \function	FrustumCameraObject::GetFrustumFlags		Returns the frustum flags.
	//
	//# \proto	unsigned_int32 GetFrustumFlags(void) const;
	//
	//# \desc
	//# The $GetFrustumFlags$ function returns the frustum flags for the camera.
	//# The frustum flags can be a combination (through logical OR) of the following values.
	//
	//# \table	FrustumFlags
	//
	//# \also	$@FrustumCameraObject::SetFrustumFlags@$
	
	
	//# \function	FrustumCameraObject::SetFrustumFlags		Sets the frustum flags.
	//
	//# \proto	void SetFrustumFlags(unsigned_int32 flags);
	//
	//# \param	flags	The new frustum flags.
	//
	//# \desc
	//# The $SetFrustumFlags$ function sets the frustum flags for the camera to those specified by the $flags$ parameter.
	//# The frustum flags can be a combination (through logical OR) of the following values.
	//
	//# \table	FrustumFlags
	//
	//# \also	$@FrustumCameraObject::GetFrustumFlags@$
	
	
	//# \function	FrustumCameraObject::GetFocalLength		Returns the focal length.
	//
	//# \proto	float GetFocalLength(void) const;
	//
	//# \desc
	//# The $GetFocalLength$ function returns the focal length for the camera.
	//# The focal length <i>e</i> is related to the horizontal field of view (FOV) through the equation
	//#
	//# <i>e</i> = 1 / tan(FOV / 2).
	//
	//# \also	$@FrustumCameraObject::SetFocalLength@$
	
	
	//# \function	FrustumCameraObject::SetFocalLength		Sets the focal length.
	//
	//# \proto	void SetFocalLength(float focal);
	//
	//# \param	focal	The new focal length.
	//
	//# \desc
	//# The $SetFocalLength$ function sets the focal length for the camera to the value specified by the $focal$ parameter.
	//# The focal length <i>e</i> is related to the horizontal field of view (FOV) through the equation
	//#
	//# <i>e</i> = 1 / tan(FOV / 2).
	//
	//# \also	$@FrustumCameraObject::GetFocalLength@$
	
	
	//# \function	FrustumCameraObject::GetAspectRatio		Returns the focal length.
	//
	//# \proto	float GetAspectRatio(void) const;
	//
	//# \desc
	//# The $GetAspectRatio$ function returns the aspect ratio for the camera.
	//# The aspect ratio is given by the height of the viewport divided by its width.
	//
	//# \also	$@FrustumCameraObject::SetAspectRatio@$
	
	
	//# \function	FrustumCameraObject::SetAspectRatio		Sets the focal length.
	//
	//# \proto	void SetAspectRatio(float aspect);
	//
	//# \param	aspect	The new aspect ratio.
	//
	//# \desc
	//# The $SetAspectRatio$ function sets the aspect ratio for the camera to the value given by the $aspect$ parameter.
	//# The aspect ratio is given by the height of the viewport divided by its width.
	//
	//# \also	$@FrustumCameraObject::GetAspectRatio@$
	
	
	class FrustumCameraObject : public CameraObject
	{
		friend class CameraObject;
		
		private:
			
			unsigned_int32	frustumFlags;
			
			float			focalLength;
			float			aspectRatio;
			
			FrustumCameraObject();
		
		protected:
			
			FrustumCameraObject(CameraType type);
			FrustumCameraObject(CameraType type, float focal, float aspect);
			~FrustumCameraObject();
		
		public:
			
			C4API FrustumCameraObject(float focal, float aspect);
			
			unsigned_int32 GetFrustumFlags(void) const
			{
				return (frustumFlags);
			}
			
			void SetFrustumFlags(unsigned_int32 flags)
			{
				frustumFlags = flags;
			}
			
			float GetFocalLength(void) const
			{
				return (focalLength);
			}
			
			void SetFocalLength(float focal)
			{
				focalLength = focal;
			}
			
			float GetAspectRatio(void) const
			{
				return (aspectRatio);
			}
			
			void SetAspectRatio(float aspect)
			{
				aspectRatio = aspect;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetObjectSize(float *size) const;
			void SetObjectSize(const float *size);
			
			void Activate(void) const;
			
			ProjectionResult ProjectSphere(const Point3D& center, float radius, ProjectionRect *rect) const;
	};
	
	
	//# \class	RemoteCameraObject		Encapsulates data pertaining to a camera having a remote transform.
	//
	//# The $FrustumCameraObject$ class encapsulates data pertaining to a camera having a remote transform.
	//# Such cameras are used for rendering recursive view effects such as mirrors or portals through which
	//# a remote area of the world can be viewed.
	//
	//# \def	class RemoteCameraObject : public FrustumCameraObject
	//
	//# \ctor	RemoteCameraObject(float focal, float aspect, const Transform4D& transform, const Vector4D& clipPlane);
	//
	//# \param	focal		The focal length of the perspective projection. This determines the field of view.
	//# \param	aspect		The aspect ratio of the projection. This is normally the ratio between the height
	//#						and the width of the viewport.
	//# \param	transform	The world-space remote transform. This is applied to the camera's world transform.
	//# \param	clipPlane	The world-space geometric clipping plane.
	//
	//# \desc
	//# 
	//# \base	FrustumCameraObject		An $RemoteCameraObject$ is a specific type of $FrustumCameraObject$, and thus always uses a perspective projection.
	//
	//# \also	$@WorldMgr/RemoteCamera@$
	
	
	//# \function	RemoteCameraObject::GetRemoteTransform		Returns the remote transform.
	//
	//# \proto	const Transform4D& GetRemoteTransform(void) const;
	//
	//# \desc
	//
	//# \also	$@RemoteCameraObject::SetRemoteTransform@$
	//# \also	$@RemoteCameraObject::GetRemoteDeterminant@$
	
	
	//# \function	RemoteCameraObject::SetRemoteTransform		Sets the remote transform.
	//
	//# \proto	void SetRemoteTransform(const Transform4D& transform);
	//
	//# \param	transform	The remote transform.
	//
	//# \desc
	//
	//# \also	$@RemoteCameraObject::GetRemoteTransform@$
	//# \also	$@RemoteCameraObject::GetRemoteDeterminant@$
	
	
	//# \function	RemoteCameraObject::GetRemoteDeterminant	Returns a value indicating whether the remote transform contains a reflection.
	//
	//# \proto	float GetRemoteDeterminant(void) const;
	//
	//# \desc
	//
	//# \also	$@RemoteCameraObject::GetRemoteTransform@$
	//# \also	$@RemoteCameraObject::SetRemoteTransform@$
	
	
	//# \function	RemoteCameraObject::GetRemoteClipPlane		Returns the geometric clipping plane for the remote camera.
	//
	//# \proto	const Antivector4D& GetRemoteClipPlane(void) const;
	//
	//# \desc
	//
	//# \also	$@RemoteCameraObject::SetRemoteClipPlane@$
	
	
	//# \function	RemoteCameraObject::SetRemoteClipPlane		Sets the geometric clipping plane for the remote camera.
	//
	//# \proto	void SetRemoteClipPlane(const Antivector4D& clipPlane);
	//
	//# \param	clipPlane	The clipping plane.
	//
	//# \desc
	//
	//# \also	$@RemoteCameraObject::GetRemoteClipPlane@$
	
	
	class RemoteCameraObject : public FrustumCameraObject
	{
		friend class CameraObject;
		
		private:
			
			Transform4D			remoteTransform;
			Transform4D			inverseRemoteTransform;
			float				remoteDeterminant;
			
			Antivector4D		remoteClipPlane;
			ProjectionRect		frustumBoundary;
			
			RemoteCameraObject();
			~RemoteCameraObject();
		
		public:
			
			RemoteCameraObject(float focal, float aspect, const Transform4D& transform, const Antivector4D& clipPlane);
			
			const Transform4D& GetRemoteTransform(void) const
			{
				return (remoteTransform);
			}
			
			const Transform4D& GetInverseRemoteTransform(void) const
			{
				return (inverseRemoteTransform);
			}
			
			float GetRemoteDeterminant(void) const
			{
				return (remoteDeterminant);
			}
			
			const Antivector4D& GetRemoteClipPlane(void) const
			{
				return (remoteClipPlane);
			}
			
			void SetRemoteClipPlane(const Antivector4D& clipPlane)
			{
				remoteClipPlane = -(clipPlane * inverseRemoteTransform);
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			void SetRemoteTransform(const Transform4D& transform);
			void SetFrustumBoundary(float left, float right, float top, float bottom);
			
			void Activate(void) const;
	};
}


#endif

// ZYURVUR
