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


#ifndef C4GeometryManipulators_h
#define C4GeometryManipulators_h


#include "C4EditorManipulators.h"
#include "C4Cloth.h"


namespace C4
{
	class GeometryManipulator : public EditorManipulator
	{
		friend class EditorManipulator;
		
		private:
			
			ColorRGBA			geometryColor;
			
			bool				*selectedSurfaceArray;
			Triangle			*selectedFaceArray;
			int32				maxSelectedFaceCount;
			
			int32				selectionDetailLevel;
			List<Attribute>		selectionAttributeList;
			DiffuseAttribute	selectionDiffuseColor;
			Renderable			selectionRenderable;
			
			static Manipulator *Construct(Geometry *geometry);
		
		public:
			
			GeometryManipulator(Geometry *geometry);
			~GeometryManipulator();
			
			Geometry *GetTargetNode(void) const
			{
				return (static_cast<Geometry *>(EditorManipulator::GetTargetNode()));
			}
			
			bool SurfaceSelected(unsigned_int32 index) const
			{
				return ((selectedSurfaceArray) && (selectedSurfaceArray[index]));
			}
			
			const char *GetDefaultNodeName(void) const;
			
			void Invalidate(void);
			
			void Select(void);
			void Unselect(void);
			
			void SelectSurface(unsigned_int32 index);
			void UnselectSurface(unsigned_int32 index);
			
			int32 GetSelectedSurfaceCount(void) const;
			void UpdateSurfaceSelection(void);
			
			void HandleSizeUpdate(int32 count, const float *size);
			
			bool MaterialSettable(void) const;
			const MaterialObject *PickupMaterial(void) const;
			void SetMaterial(MaterialObject *materialObject);
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Pick(const Ray *ray, PickData *data) const;
			bool RegionPick(const Region *region) const;
			
			void Render(const ManipulatorRenderData *renderData);
	};
	
	
	class MeshGeometryManipulator : public GeometryManipulator
	{
		private:
			
			Box3D	originalBounds;
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			MeshGeometryManipulator(MeshGeometry *mesh);
			~MeshGeometryManipulator();
			
			MeshGeometry *GetTargetNode(void) const
			{
				return (static_cast<MeshGeometry *>(EditorManipulator::GetTargetNode()));
			}
			
			MeshGeometryObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			void BeginResize(const ManipulatorResizeData *resizeData);
			bool Resize(const ManipulatorResizeData *resizeData);
	};
	
	
	class PlateGeometryManipulator : public GeometryManipulator 
	{
		private: 
			 
			int32 GetHandleTable(Point3D *handle) const; 
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		 
		public:
			
			PlateGeometryManipulator(PlateGeometry *plate);
			~PlateGeometryManipulator(); 
			
			PlateGeometry *GetTargetNode(void) const
			{
				return (static_cast<PlateGeometry *>(EditorManipulator::GetTargetNode())); 
			}
			
			PlateGeometryObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			bool Pick(const Ray *ray, PickData *data) const;
			bool Resize(const ManipulatorResizeData *resizeData);
	};
	
	
	class DiskGeometryManipulator : public GeometryManipulator
	{
		private:
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			DiskGeometryManipulator(DiskGeometry *disk);
			~DiskGeometryManipulator();
			
			DiskGeometry *GetTargetNode(void) const
			{
				return (static_cast<DiskGeometry *>(EditorManipulator::GetTargetNode()));
			}
			
			DiskGeometryObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			bool Pick(const Ray *ray, PickData *data) const;
			bool Resize(const ManipulatorResizeData *resizeData);
	};
	
	
	class HoleGeometryManipulator : public GeometryManipulator
	{
		private:
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			HoleGeometryManipulator(HoleGeometry *disk);
			~HoleGeometryManipulator();
			
			HoleGeometry *GetTargetNode(void) const
			{
				return (static_cast<HoleGeometry *>(EditorManipulator::GetTargetNode()));
			}
			
			HoleGeometryObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			bool Pick(const Ray *ray, PickData *data) const;
			bool Resize(const ManipulatorResizeData *resizeData);
	};
	
	
	class AnnulusGeometryManipulator : public GeometryManipulator
	{
		private:
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			AnnulusGeometryManipulator(AnnulusGeometry *annulus);
			~AnnulusGeometryManipulator();
			
			AnnulusGeometry *GetTargetNode(void) const
			{
				return (static_cast<AnnulusGeometry *>(EditorManipulator::GetTargetNode()));
			}
			
			AnnulusGeometryObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			bool Pick(const Ray *ray, PickData *data) const;
			bool Resize(const ManipulatorResizeData *resizeData);
	};
	
	
	class BoxGeometryManipulator : public GeometryManipulator
	{
		private:
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			BoxGeometryManipulator(BoxGeometry *box);
			~BoxGeometryManipulator();
			
			BoxGeometry *GetTargetNode(void) const
			{
				return (static_cast<BoxGeometry *>(EditorManipulator::GetTargetNode()));
			}
			
			BoxGeometryObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			bool Pick(const Ray *ray, PickData *data) const;
			bool Resize(const ManipulatorResizeData *resizeData);
	};
	
	
	class PyramidGeometryManipulator : public GeometryManipulator
	{
		private:
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			PyramidGeometryManipulator(PyramidGeometry *pyramid);
			~PyramidGeometryManipulator();
			
			PyramidGeometry *GetTargetNode(void) const
			{
				return (static_cast<PyramidGeometry *>(EditorManipulator::GetTargetNode()));
			}
			
			PyramidGeometryObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			bool Resize(const ManipulatorResizeData *resizeData);
	};
	
	
	class CylinderGeometryManipulator : public GeometryManipulator
	{
		private:
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			CylinderGeometryManipulator(CylinderGeometry *cylinder);
			~CylinderGeometryManipulator();
			
			CylinderGeometry *GetTargetNode(void) const
			{
				return (static_cast<CylinderGeometry *>(EditorManipulator::GetTargetNode()));
			}
			
			CylinderGeometryObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			bool Pick(const Ray *ray, PickData *data) const;
			bool Resize(const ManipulatorResizeData *resizeData);
	};
	
	
	class ConeGeometryManipulator : public GeometryManipulator
	{
		private:
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			ConeGeometryManipulator(ConeGeometry *cone);
			~ConeGeometryManipulator();
			
			ConeGeometry *GetTargetNode(void) const
			{
				return (static_cast<ConeGeometry *>(EditorManipulator::GetTargetNode()));
			}
			
			ConeGeometryObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			bool Resize(const ManipulatorResizeData *resizeData);
	};
	
	
	class TruncatedConeGeometryManipulator : public GeometryManipulator
	{
		private:
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			TruncatedConeGeometryManipulator(TruncatedConeGeometry *truncatedCone);
			~TruncatedConeGeometryManipulator();
			
			TruncatedConeGeometry *GetTargetNode(void) const
			{
				return (static_cast<TruncatedConeGeometry *>(EditorManipulator::GetTargetNode()));
			}
			
			TruncatedConeGeometryObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			bool Resize(const ManipulatorResizeData *resizeData);
	};
	
	
	class SphereGeometryManipulator : public GeometryManipulator
	{
		private:
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			SphereGeometryManipulator(SphereGeometry *sphere);
			~SphereGeometryManipulator();
			
			SphereGeometry *GetTargetNode(void) const
			{
				return (static_cast<SphereGeometry *>(EditorManipulator::GetTargetNode()));
			}
			
			SphereGeometryObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			bool Resize(const ManipulatorResizeData *resizeData);
	};
	
	
	class DomeGeometryManipulator : public GeometryManipulator
	{
		private:
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			DomeGeometryManipulator(DomeGeometry *dome);
			~DomeGeometryManipulator();
			
			DomeGeometry *GetTargetNode(void) const
			{
				return (static_cast<DomeGeometry *>(EditorManipulator::GetTargetNode()));
			}
			
			DomeGeometryObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			bool Resize(const ManipulatorResizeData *resizeData);
	};
	
	
	class TorusGeometryManipulator : public GeometryManipulator
	{
		private:
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			TorusGeometryManipulator(TorusGeometry *torus);
			~TorusGeometryManipulator();
			
			TorusGeometry *GetTargetNode(void) const
			{
				return (static_cast<TorusGeometry *>(EditorManipulator::GetTargetNode()));
			}
			
			TorusGeometryObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			bool Resize(const ManipulatorResizeData *resizeData);
	};
	
	
	class TubeGeometryManipulator : public GeometryManipulator
	{
		private:
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			TubeGeometryManipulator(TubeGeometry *tube);
			~TubeGeometryManipulator();
			
			TubeGeometry *GetTargetNode(void) const
			{
				return (static_cast<TubeGeometry *>(EditorManipulator::GetTargetNode()));
			}
			
			TubeGeometryObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			bool Resize(const ManipulatorResizeData *resizeData);
	};
	
	
	class ExtrusionGeometryManipulator : public GeometryManipulator
	{
		private:
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			ExtrusionGeometryManipulator(ExtrusionGeometry *extrusion);
			~ExtrusionGeometryManipulator();
			
			ExtrusionGeometry *GetTargetNode(void) const
			{
				return (static_cast<ExtrusionGeometry *>(EditorManipulator::GetTargetNode()));
			}
			
			ExtrusionGeometryObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			bool Resize(const ManipulatorResizeData *resizeData);
	};
	
	
	class RevolutionGeometryManipulator : public GeometryManipulator
	{
		private:
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			RevolutionGeometryManipulator(RevolutionGeometry *revolution);
			~RevolutionGeometryManipulator();
			
			RevolutionGeometry *GetTargetNode(void) const
			{
				return (static_cast<RevolutionGeometry *>(EditorManipulator::GetTargetNode()));
			}
			
			RevolutionGeometryObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			bool Resize(const ManipulatorResizeData *resizeData);
	};
	
	
	class ClothGeometryManipulator : public GeometryManipulator
	{
		private:
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			ClothGeometryManipulator(ClothGeometry *cloth);
			~ClothGeometryManipulator();
			
			ClothGeometry *GetTargetNode(void) const
			{
				return (static_cast<ClothGeometry *>(EditorManipulator::GetTargetNode()));
			}
			
			ClothGeometryObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			bool Pick(const Ray *ray, PickData *data) const;
			bool Resize(const ManipulatorResizeData *resizeData);
	};
}


#endif

// ZYURVUR
