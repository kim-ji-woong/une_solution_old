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


#ifndef C4ZoneManipulators_h
#define C4ZoneManipulators_h


#include "C4EditorManipulators.h"
#include "C4Zones.h"


namespace C4
{
	class ZoneManipulator : public EditorManipulator
	{
		friend class EditorManipulator;
		
		private:
			
			Vector4D						zoneSizeVector;
			AutoRelease<MaterialObject>		zoneMaterial;
			DiffuseAttribute				zoneDiffuseColor;
			TextureMapAttribute				zoneTextureMap;
			Renderable						zoneRenderable;
			
			static ZoneManipulator *Construct(Zone *zone);
		
		protected:
			
			ZoneManipulator(Zone *zone);
			
			const Vector4D& GetZoneSize(void) const
			{
				return (zoneSizeVector);
			}
			
			void SetZoneSize(float x, float y, float z)
			{
				zoneSizeVector.GetVector3D().Set(x, y, z);
			}
			
			Renderable *GetZoneRenderable(void)
			{
				return (&zoneRenderable);
			}
			
			const Renderable *GetZoneRenderable(void) const
			{
				return (&zoneRenderable);
			}
		
		public:
			
			~ZoneManipulator();
			
			Zone *GetTargetNode(void) const
			{
				return (static_cast<Zone *>(EditorManipulator::GetTargetNode()));
			}
			
			ZoneObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			const char *GetDefaultNodeName(void) const;
			
			void SetTarget(bool target);
			
			void Invalidate(void);
			
			void Select(void);
			void Unselect(void);
			
			void HandleDelete(bool undoable);
			
			bool Pick(const Ray *ray, PickData *data) const;
			bool RegionPick(const Region *region) const;
			
			void Update(void);
			void Render(const ManipulatorRenderData *renderData);
	};
	
	
	class InfiniteZoneManipulator : public ZoneManipulator
	{
		private:
			
			Box3D		originalZoneBox;
			
			Point3D		boxVertex[48];
			Point2D		boxTexcoord[48];
			
			bool CalculateNodeSphere(BoundingSphere *sphere) const;
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			InfiniteZoneManipulator(InfiniteZone *infinite);
			~InfiniteZoneManipulator();
			
			InfiniteZone *GetTargetNode(void) const
			{ 
				return (static_cast<InfiniteZone *>(EditorManipulator::GetTargetNode()));
			} 
			 
			InfiniteZoneObject *GetObject(void) const 
			{
				return (static_cast<InfiniteZoneObject *>(GetTargetNode()->GetObject())); 
			}
			
			Box3D CalculateNodeBoundingBox(void) const;
			 
			void HandleSizeUpdate(int32 count, const float *size);
			
			void BeginResize(const ManipulatorResizeData *resizeData);
			bool Resize(const ManipulatorResizeData *resizeData); 
			
			void Update(void);
	};
	
	
	class BoxZoneManipulator : public ZoneManipulator
	{
		private:
			
			Point2D		boxTexcoord[48];
			
			bool CalculateNodeSphere(BoundingSphere *sphere) const;
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			BoxZoneManipulator(BoxZone *box);
			~BoxZoneManipulator();
			
			BoxZone *GetTargetNode(void) const
			{
				return (static_cast<BoxZone *>(EditorManipulator::GetTargetNode()));
			}
			
			BoxZoneObject *GetObject(void) const
			{
				return (static_cast<BoxZoneObject *>(GetTargetNode()->GetObject()));
			}
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Resize(const ManipulatorResizeData *resizeData);
			void Update(void);
	};
	
	
	class CylinderZoneManipulator : public ZoneManipulator
	{
		private:
			
			Point2D		cylinderTexcoord[192];
			
			bool CalculateNodeSphere(BoundingSphere *sphere) const;
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			CylinderZoneManipulator(CylinderZone *cylinder);
			~CylinderZoneManipulator();
			
			CylinderZone *GetTargetNode(void) const
			{
				return (static_cast<CylinderZone *>(EditorManipulator::GetTargetNode()));
			}
			
			CylinderZoneObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Resize(const ManipulatorResizeData *resizeData);
			void Update(void);
	};
	
	
	class DomeZoneManipulator : public ZoneManipulator
	{
		private:
			
			static const ConstPoint3D	domeVertex[128];
			static const ConstVector4D	domeTangent[128];
			
			Point2D		domeTexcoord[128];
			
			bool CalculateNodeSphere(BoundingSphere *sphere) const;
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			DomeZoneManipulator(DomeZone *dome);
			~DomeZoneManipulator();
			
			DomeZone *GetTargetNode(void) const
			{
				return (static_cast<DomeZone *>(EditorManipulator::GetTargetNode()));
			}
			
			DomeZoneObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Resize(const ManipulatorResizeData *resizeData);
			void Update(void);
	};
	
	
	class PolygonZoneManipulator : public ZoneManipulator
	{
		private:
			
			Point3D		originalVertexPosition;
			
			Point3D		polygonVertex[kMaxZoneVertexCount * 12];
			Vector4D	polygonTangent[kMaxZoneVertexCount * 12];
			Point2D		polygonTexcoord[kMaxZoneVertexCount * 12];
			
			bool CalculateNodeSphere(BoundingSphere *sphere) const;
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
			
			static Point3D ConstrainVertex(const Point3D& original, const Point3D& current, const Point3D& v1, const Point3D& v2);
		
		public:
			
			PolygonZoneManipulator(PolygonZone *polygon);
			~PolygonZoneManipulator();
			
			PolygonZone *GetTargetNode(void) const
			{
				return (static_cast<PolygonZone *>(EditorManipulator::GetTargetNode()));
			}
			
			PolygonZoneObject *GetObject(void) const
			{
				return (static_cast<PolygonZoneObject *>(GetTargetNode()->GetObject()));
			}
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			void BeginResize(const ManipulatorResizeData *resizeData);
			bool Resize(const ManipulatorResizeData *resizeData);
			
			bool Pick(const Ray *ray, PickData *data) const;
			
			void Update(void);
	};
}


#endif

// ZYURVUR
