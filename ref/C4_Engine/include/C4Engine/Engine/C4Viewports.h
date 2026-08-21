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


#ifndef C4Viewports_h
#define C4Viewports_h


//# \component	Interface Manager
//# \prefix		InterfaceMgr/


#include "C4Interface.h"
#include "C4Cameras.h"


namespace C4
{
	enum
	{
		kWidgetOrthoViewport	= 'OVPT',
		kWidgetFrustumViewport	= 'FVPT',
		kWidgetWorldViewport	= 'WRLD'
	};
	
	
	//# \class	ViewportWidget		The base class for interface widgets that display 3D viewports.
	//
	//# The $ViewportWidget$ class is the base class for interface widgets that display 3D viewports.
	//
	//# \def	class ViewportWidget : public RenderableWidget
	//
	//# \ctor	ViewportWidget(ViewportType type, const Vector2D& size);
	//
	//# The $ViewportWidget$ constructor has protected access. The $ViewportWidget$ class can only
	//# exist as the base class for a more specific type of viewport.
	//
	//# \param	type		The type of the viewport.
	//# \param	size		The size of the viewport, in pixels.
	//
	//# \desc
	//# 
	//
	//# \base	RenderableWidget	All rendered interface widgets are subclasses of $RenderableWidget$.
	//
	//# \also	$@OrthoViewportWidget@$
	//# \also	$@FrustumViewportWidget@$
	
	
	class C4_API ViewportWidget : public RenderableWidget
	{
		public:
			
			typedef void MouseEventProc(const MouseEventData *, ViewportWidget *, void *);
			typedef void TrackTaskProc(const Point3D&, ViewportWidget *, void *);
			typedef void RenderProc(List<Renderable> *, ViewportWidget *, void *);
		
		private:
			
			int32					viewportIndex;
			Camera					*viewportCamera;
			
			MouseEventProc			*mouseEventProc;
			void					*mouseEventCookie;
			
			TrackTaskProc			*trackTaskProc;
			void					*trackTaskCookie;
			
			RenderProc				*renderProc;
			void					*renderCookie;
			
			RenderProc				*overlayProc;
			void					*overlayCookie;
			
			List<Attribute>			attributeList;
			TextureMapAttribute		textureMapAttribute;
			
			Point2D					viewportVertex[4];
			Point2D					viewportTexcoord[4];
			
			bool					textureValidFlag;
			TextureHeader			textureHeader;
			
			void Initialize(void);
		
		protected:
			
			ViewportWidget(WidgetType type, Camera *camera);
			ViewportWidget(WidgetType type, Camera *camera, const Vector2D& size);
			ViewportWidget(const ViewportWidget& viewportWidget, Camera *camera);
			
			virtual void SetGraphicsCamera(void) = 0;
		
		public:
			
			~ViewportWidget();
			
			int32 GetViewportIndex(void) const
			{
				return (viewportIndex);
			}
			
			void SetViewportIndex(int32 index)
			{
				viewportIndex = index;
			} 
			
			Camera *GetViewportCamera(void) 
			{ 
				return (viewportCamera); 
			}
			 
			const Camera *GetViewportCamera(void) const
			{
				return (viewportCamera);
			} 
			
			void SetMouseEventProc(MouseEventProc *proc, void *cookie = nullptr)
			{
				mouseEventProc = proc; 
				mouseEventCookie = cookie;
			}
			
			void SetTrackTaskProc(TrackTaskProc *proc, void *cookie = nullptr)
			{
				trackTaskProc = proc;
				trackTaskCookie = cookie;
			}
			
			void SetRenderProc(RenderProc *proc, void *cookie = nullptr)
			{
				renderProc = proc;
				renderCookie = cookie;
			}
			
			void SetOverlayProc(RenderProc *proc, void *cookie = nullptr)
			{
				overlayProc = proc;
				overlayCookie = cookie;
			}
			
			void InvalidateTexture(void)
			{
				textureValidFlag = false;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			void SetWidgetSize(const Vector2D& size);
			void Preprocess(void);
			void Build(void);
			
			void AllocateTexture(void);
			void DeallocateTexture(void);
			
			void Render(List<Renderable> *renderList);
			
			void HandleMouseEvent(const PanelMouseEventData *eventData);
			void TrackTask(WidgetPart widgetPart, const Point3D& mousePosition);
	};
	
	
	//# \class	OrthoViewportWidget		The interface widget that displays an orthographic 3D viewport.
	//
	//# The $OrthoViewportWidget$ class represents an interface widget that displays an orthographic 3D viewport.
	//
	//# \def	class OrthoViewportWidget : public ViewportWidget
	//
	//# \ctor	OrthoViewportWidget(const Vector2D& size, const Vector2D& scale);
	//
	//# \param	size		The size of the viewport, in pixels.
	//# \param	scale		The orthographic camera scale.
	//
	//# \desc
	//# The $OrthoViewportWidget$ class displays a viewport with an orthographic camera.
	//
	//# \base	ViewportWidget		An $OrthoViewportWidget$ is a specific type of viewport.
	//
	//# \also	$@FrustumViewportWidget@$
	
	
	class OrthoViewportWidget : public ViewportWidget
	{
		friend class WidgetReg<OrthoViewportWidget>;
		
		private:
			
			OrthoCamera		orthoCamera;
			Vector2D		orthoScale;
			
			OrthoViewportWidget();
			OrthoViewportWidget(const OrthoViewportWidget& orthoViewportWidget);
			
			C4API Widget *Replicate(void) const override;
			
			void SetGraphicsCamera(void);
		
		public:
			
			C4API OrthoViewportWidget(const Vector2D& size, const Vector2D& scale);
			C4API ~OrthoViewportWidget();
			
			OrthoCamera *GetViewportCamera(void)
			{
				return (&orthoCamera);
			}
			
			const OrthoCamera *GetViewportCamera(void) const
			{
				return (&orthoCamera);
			}
			
			const Vector2D& GetOrthoScale(void) const
			{
				return (orthoScale);
			}
			
			void SetOrthoScale(const Vector2D& scale)
			{
				orthoScale = scale;
			}
			
			C4API void Pack(Packer& data, unsigned_int32 packFlags) const;
			C4API void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
	};
	
	
	//# \class	FrustumViewportWidget	The interface widget that displays a frustum 3D viewport.
	//
	//# The $FrustumViewportWidget$ class represents an interface widget that displays a frustum 3D viewport.
	//
	//# \def	class FrustumViewportWidget : public ViewportWidget
	//
	//# \ctor	FrustumViewportWidget(const Vector2D& size, float focalLength);
	//
	//# \param	size			The size of the viewport, in pixels.
	//# \param	focalLength		The focal length of the frustum camera.
	//
	//# \desc
	//# The $FrustumViewportWidget$ class displays a viewport with a frustum camera.
	//
	//# \base	ViewportWidget		A $FrustumViewportWidget$ is a specific type of viewport.
	//
	//# \also	$@OrthoViewportWidget@$
	
	
	class C4_API FrustumViewportWidget : public ViewportWidget
	{
		friend class WidgetReg<FrustumViewportWidget>;
		
		private:
			
			FrustumCamera	frustumCamera;
			
			float			cameraAzimuth;
			float			cameraAltitude;
			
			FrustumViewportWidget();
			
			Widget *Replicate(void) const override;
			
			void SetGraphicsCamera(void);
		
		protected:
			
			FrustumViewportWidget(WidgetType type);
			FrustumViewportWidget(WidgetType type, const Vector2D& size, float focalLength);
			FrustumViewportWidget(const FrustumViewportWidget& frustumViewportWidget);
		
		public:
			
			FrustumViewportWidget(const Vector2D& size, float focalLength);
			~FrustumViewportWidget();
			
			FrustumCamera *GetViewportCamera(void)
			{
				return (&frustumCamera);
			}
			
			const FrustumCamera *GetViewportCamera(void) const
			{
				return (&frustumCamera);
			}
			
			float GetCameraAzimuth(void) const
			{
				return (cameraAzimuth);
			}
			
			float GetCameraAltitude(void) const
			{
				return (cameraAltitude);
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			void SetCameraTransform(float azm, float alt, const Point3D& position);
			void SetCameraPosition(const Point3D& position);
	};
	
	
	//# \class	WorldViewportWidget		The interface widget that renders a world in a 3D viewport.
	//
	//# The $WorldViewportWidget$ class represents an interface widget that renders a world in a 3D viewport.
	//
	//# \def	class WorldViewportWidget : public FrustumViewportWidget
	//
	//# \ctor	WorldViewportWidget(const Vector2D& size, float focalLength);
	//
	//# \param	size			The size of the viewport, in pixels.
	//# \param	focalLength		The focal length of the frustum camera.
	//
	//# \desc
	//# The $WorldViewportWidget$ class renders a world into a viewport with a frustum camera.
	//
	//# \base	FrustumViewportWidget		A $WorldViewportWidget$ is a specific type of frustum viewport.
	
	
	class WorldViewportWidget : public FrustumViewportWidget
	{
		friend class WidgetReg<WorldViewportWidget>;
		
		private:
			
			World		*viewportWorld;
			
			Point3D		cameraTarget;
			float		cameraDistance;
			float		maxCameraDistance;
			
			bool		trackFlag;
			Point3D		previousPosition;
			
			WorldViewportWidget();
			
			C4API Widget *Replicate(void) const override;
			
			static void ViewportHandleMouseEvent(const MouseEventData *eventData, ViewportWidget *viewport, void *cookie);
			static void ViewportRender(List<Renderable> *renderList, ViewportWidget *viewport, void *cookie);
		
		public:
			
			C4API WorldViewportWidget(const Vector2D& size, float focalLength);
			C4API ~WorldViewportWidget();
			
			World *GetViewportWorld(void) const
			{
				return (viewportWorld);
			}
			
			void Preprocess(void);
			
			C4API void EnableCameraOrbit(const Point3D& target, float distance);
			C4API void DisableCameraOrbit(void);
			
			C4API void SetCameraAngles(float azm, float alt);
			
			C4API void LoadWorld(const char *name);
			C4API void UnloadWorld();
	};
	
	
	class Grid : public Renderable
	{
		private:
			
			struct GridVertex
			{
				Point2D		position;
				Color4C		color;
			};
			
			unsigned_int32		gridFlags;
			
			float				gridLineSpacing;
			int32				majorLineInterval;
			
			Point2D				boundingBoxMin;
			Point2D				boundingBoxMax;
			
			Color4C				minorLineColor;
			Color4C				majorLineColor;
			Color4C				axisLineColor;
			Color4C				boundingBoxColor;
			
			GridVertex			*gridStorage;
			int32				gridStorageCount;
			
			VertexBuffer				dynamicVertexBuffer;
			VertexBufferObserver<Grid>	dynamicVertexBufferObserver;
			
			void FillDynamicVertexBuffer(VertexBuffer *vertexBuffer);
		
		public:
			
			enum
			{
				kGridShowBoundingBox	= 1 << 0
			};
			
			C4API Grid();
			C4API ~Grid();
			
			unsigned_int32 GetGridFlags(void) const
			{
				return (gridFlags);
			}
			
			void SetGridFlags(unsigned_int32 flags)
			{
				gridFlags = flags;
			}
			
			float GetGridLineSpacing(void) const
			{
				return (gridLineSpacing);
			}
			
			void SetGridLineSpacing(float spacing)
			{
				gridLineSpacing = spacing;
			}
			
			int32 GetMajorLineInterval(void) const
			{
				return (majorLineInterval);
			}
			
			void SetMajorLineInterval(int32 interval)
			{
				majorLineInterval = interval;
			}
			
			void SetBoundingBox(const Vector2D& min, const Vector2D& max)
			{
				boundingBoxMin = min;
				boundingBoxMax = max;
			}
			
			void SetMinorLineColor(const ColorRGB& color)
			{
				minorLineColor.Set((int32) (color.red * 255.0F), (int32) (color.green * 255.0F), (int32) (color.blue * 255.0F), 0xFF);
			}
			
			void SetMajorLineColor(const ColorRGB& color)
			{
				majorLineColor.Set((int32) (color.red * 255.0F), (int32) (color.green * 255.0F), (int32) (color.blue * 255.0F), 0xFF);
			}
			
			void SetAxisLineColor(const ColorRGB& color)
			{
				axisLineColor.Set((int32) (color.red * 255.0F), (int32) (color.green * 255.0F), (int32) (color.blue * 255.0F), 0xFF);
			}
			
			void SetBoundingBoxColor(const ColorRGB& color)
			{
				boundingBoxColor.Set((int32) (color.red * 255.0F), (int32) (color.green * 255.0F), (int32) (color.blue * 255.0F), 0xFF);
			}
			
			C4API void Build(const Point2D& min, const Point2D& max, float scale);
	};
	
	
	class DragRect : public Renderable
	{
		private:
			
			List<Attribute>			attributeList;
			DiffuseAttribute		diffuseColor;
			
			Point3D					rectVertex[16];
		
		public:
			
			C4API DragRect(const ColorRGBA& color);
			C4API ~DragRect();
			
			C4API void Build(const Point2D& p1, const Point2D& p2, float scale);
			C4API void Build(const Point2D& p1, const Point2D& p2, const Vector3D& dx, const Vector3D& dy, float scale);
	};
}


#endif

// ZYURVUR
