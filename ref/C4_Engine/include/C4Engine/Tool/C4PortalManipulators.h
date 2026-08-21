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


#ifndef C4PortalManipulators_h
#define C4PortalManipulators_h


#include "C4EditorManipulators.h"
#include "C4Portals.h"


namespace C4
{
	class PortalManipulator : public EditorManipulator
	{
		friend class EditorManipulator;
		
		private:
			
			ColorRGBA				portalColor;
			Vector4D				sizeVector;
			
			List<Attribute>			outlineAttributeList;
			DiffuseAttribute		outlineDiffuseColor;
			TextureMapAttribute		outlineTextureMap;
			Renderable				outlineRenderable;
			
			List<Attribute>			directionAttributeList;
			DiffuseAttribute		directionDiffuseColor;
			TextureMapAttribute		directionTextureMap;
			Renderable				directionRenderable;
			
			Point3D					originalVertexPosition;
			
			Point3D					outlineVertex[kMaxPortalVertexCount * 4];
			Vector4D				outlineTangent[kMaxPortalVertexCount * 4];
			Point3D					directionVertex[kMaxPortalVertexCount * 4];
			Point2D					directionTexcoord[kMaxPortalVertexCount * 4];
			
			static const ConstPoint2D	outlineTexcoord[kMaxPortalVertexCount * 4];
			
			static Manipulator *Construct(Portal *portal);
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
			
			static Point3D ConstrainVertex(const Point3D& original, const Point3D& current, const Point3D& v1, const Point3D& v2);
		
		protected:
			
			PortalManipulator(Portal *portal, const ColorRGBA& color);
		
		public:
			
			~PortalManipulator();
			
			Portal *GetTargetNode(void) const
			{
				return (static_cast<Portal *>(EditorManipulator::GetTargetNode()));
			}
			
			PortalObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			const char *GetDefaultNodeName(void) const;
			
			void Invalidate(void);
			
			void Select(void);
			void Unselect(void);
			
			void HandleConnectorUpdate(void);
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Pick(const Ray *ray, PickData *data) const;
			bool RegionPick(const Region *region) const;
			
			void BeginResize(const ManipulatorResizeData *resizeData);
			bool Resize(const ManipulatorResizeData *resizeData);
			
			void Update(void);
			void Render(const ManipulatorRenderData *renderData);
	};
	
	
	class DirectPortalManipulator : public PortalManipulator
	{
		public:
			
			DirectPortalManipulator(DirectPortal *portal);
			~DirectPortalManipulator();
			
			DirectPortal *GetTargetNode(void) const
			{
				return (static_cast<DirectPortal *>(EditorManipulator::GetTargetNode()));
			}
	};
	
	
	class RemotePortalManipulator : public PortalManipulator
	{
		public: 
			
			RemotePortalManipulator(RemotePortal *portal); 
			~RemotePortalManipulator(); 
			 
			RemotePortal *GetTargetNode(void) const
			{ 
				return (static_cast<RemotePortal *>(EditorManipulator::GetTargetNode()));
			}
	};
	 
	
	class OcclusionPortalManipulator : public PortalManipulator
	{
		public: 
			
			OcclusionPortalManipulator(OcclusionPortal *portal);
			~OcclusionPortalManipulator();
			
			OcclusionPortal *GetTargetNode(void) const
			{
				return (static_cast<OcclusionPortal *>(EditorManipulator::GetTargetNode()));
			}
	};
}


#endif

// ZYURVUR
