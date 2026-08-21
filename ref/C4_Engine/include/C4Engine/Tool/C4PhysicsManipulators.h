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


#ifndef C4PhysicsManipulators_h
#define C4PhysicsManipulators_h


#include "C4EditorManipulators.h"
#include "C4VolumeManipulators.h"
#include "C4Physics.h"
#include "C4Fields.h"


namespace C4
{
	class PhysicsNodeManipulator : public EditorManipulator
	{
		public:
			
			PhysicsNodeManipulator(PhysicsNode *physicsNode);
			~PhysicsNodeManipulator();
			
			const char *GetDefaultNodeName(void) const;
			
			void Preprocess(void);
	};
	
	
	class ShapeManipulator : public EditorManipulator
	{
		friend class EditorManipulator;
		
		private:
			
			VolumeManipulator	*volumeManipulator;
			
			static Manipulator *Construct(Shape *shape);
		
		protected:
			
			ShapeManipulator(Shape *shape, VolumeManipulator *volume);
		
		public:
			
			~ShapeManipulator();
			
			Shape *GetTargetNode(void) const
			{
				return (static_cast<Shape *>(EditorManipulator::GetTargetNode()));
			}
			
			const char *GetDefaultNodeName(void) const;
			
			void Select(void);
			void Unselect(void);
			
			bool Pick(const Ray *ray, PickData *data) const;
			bool RegionPick(const Region *region) const;
			
			void Render(const ManipulatorRenderData *renderData);
	};
	
	
	class BoxShapeManipulator : public ShapeManipulator, public BoxVolumeManipulator
	{
		private:
			
			bool CalculateNodeSphere(BoundingSphere *sphere) const;
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			BoxShapeManipulator(BoxShape *box);
			~BoxShapeManipulator();
			
			BoxShape *GetTargetNode(void) const
			{
				return (static_cast<BoxShape *>(EditorManipulator::GetTargetNode()));
			}
			
			BoxShapeObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Resize(const ManipulatorResizeData *resizeData);
			void Update(void);
	};
	
	
	class PyramidShapeManipulator : public ShapeManipulator, public PyramidVolumeManipulator
	{
		private:
			
			bool CalculateNodeSphere(BoundingSphere *sphere) const;
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			 
			PyramidShapeManipulator(PyramidShape *pyramid);
			~PyramidShapeManipulator(); 
			 
			PyramidShape *GetTargetNode(void) const 
			{
				return (static_cast<PyramidShape *>(EditorManipulator::GetTargetNode())); 
			}
			
			PyramidShapeObject *GetObject(void) const
			{ 
				return (GetTargetNode()->GetObject());
			}
			
			Box3D CalculateNodeBoundingBox(void) const; 
			
			bool Resize(const ManipulatorResizeData *resizeData);
			void Update(void);
	};
	
	
	class CylinderShapeManipulator : public ShapeManipulator, public CylinderVolumeManipulator
	{
		private:
			
			bool CalculateNodeSphere(BoundingSphere *sphere) const;
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			CylinderShapeManipulator(CylinderShape *cylinder);
			~CylinderShapeManipulator();
			
			CylinderShape *GetTargetNode(void) const
			{
				return (static_cast<CylinderShape *>(EditorManipulator::GetTargetNode()));
			}
			
			CylinderShapeObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Resize(const ManipulatorResizeData *resizeData);
			void Update(void);
	};
	
	
	class ConeShapeManipulator : public ShapeManipulator, public ConeVolumeManipulator
	{
		private:
			
			bool CalculateNodeSphere(BoundingSphere *sphere) const;
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			ConeShapeManipulator(ConeShape *cone);
			~ConeShapeManipulator();
			
			ConeShape *GetTargetNode(void) const
			{
				return (static_cast<ConeShape *>(EditorManipulator::GetTargetNode()));
			}
			
			ConeShapeObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Resize(const ManipulatorResizeData *resizeData);
			void Update(void);
	};
	
	
	class SphereShapeManipulator : public ShapeManipulator, public SphereVolumeManipulator
	{
		private:
			
			bool CalculateNodeSphere(BoundingSphere *sphere) const;
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			SphereShapeManipulator(SphereShape *sphere);
			~SphereShapeManipulator();
			
			SphereShape *GetTargetNode(void) const
			{
				return (static_cast<SphereShape *>(EditorManipulator::GetTargetNode()));
			}
			
			SphereShapeObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Resize(const ManipulatorResizeData *resizeData);
			void Update(void);
	};
	
	
	class DomeShapeManipulator : public ShapeManipulator, public DomeVolumeManipulator
	{
		private:
			
			bool CalculateNodeSphere(BoundingSphere *sphere) const;
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			DomeShapeManipulator(DomeShape *dome);
			~DomeShapeManipulator();
			
			DomeShape *GetTargetNode(void) const
			{
				return (static_cast<DomeShape *>(EditorManipulator::GetTargetNode()));
			}
			
			DomeShapeObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Resize(const ManipulatorResizeData *resizeData);
			void Update(void);
	};
	
	
	class CapsuleShapeManipulator : public ShapeManipulator, public CapsuleVolumeManipulator
	{
		private:
			
			bool CalculateNodeSphere(BoundingSphere *sphere) const;
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			CapsuleShapeManipulator(CapsuleShape *capsule);
			~CapsuleShapeManipulator();
			
			CapsuleShape *GetTargetNode(void) const
			{
				return (static_cast<CapsuleShape *>(EditorManipulator::GetTargetNode()));
			}
			
			CapsuleShapeObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Resize(const ManipulatorResizeData *resizeData);
			void Update(void);
	};
	
	
	class TruncatedPyramidShapeManipulator : public ShapeManipulator, public TruncatedPyramidVolumeManipulator
	{
		private:
			
			bool CalculateNodeSphere(BoundingSphere *sphere) const;
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			TruncatedPyramidShapeManipulator(TruncatedPyramidShape *truncatedPyramid);
			~TruncatedPyramidShapeManipulator();
			
			TruncatedPyramidShape *GetTargetNode(void) const
			{
				return (static_cast<TruncatedPyramidShape *>(EditorManipulator::GetTargetNode()));
			}
			
			TruncatedPyramidShapeObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Resize(const ManipulatorResizeData *resizeData);
			void Update(void);
	};
	
	
	class TruncatedConeShapeManipulator : public ShapeManipulator, public TruncatedConeVolumeManipulator
	{
		private:
			
			bool CalculateNodeSphere(BoundingSphere *sphere) const;
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			TruncatedConeShapeManipulator(TruncatedConeShape *truncatedCone);
			~TruncatedConeShapeManipulator();
			
			TruncatedConeShape *GetTargetNode(void) const
			{
				return (static_cast<TruncatedConeShape *>(EditorManipulator::GetTargetNode()));
			}
			
			TruncatedConeShapeObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Resize(const ManipulatorResizeData *resizeData);
			void Update(void);
	};
	
	
	class TruncatedDomeShapeManipulator : public ShapeManipulator, public TruncatedDomeVolumeManipulator
	{
		private:
			
			bool CalculateNodeSphere(BoundingSphere *sphere) const;
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			TruncatedDomeShapeManipulator(TruncatedDomeShape *truncatedDome);
			~TruncatedDomeShapeManipulator();
			
			TruncatedDomeShape *GetTargetNode(void) const
			{
				return (static_cast<TruncatedDomeShape *>(EditorManipulator::GetTargetNode()));
			}
			
			TruncatedDomeShapeObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Resize(const ManipulatorResizeData *resizeData);
			void Update(void);
	};
	
	
	class JointManipulator : public EditorManipulator
	{
		friend class EditorManipulator;
		
		private:
			
			static Manipulator *Construct(Joint *joint);
		
		protected:
			
			JointManipulator(Joint *joint, const char *iconName);
		
		public:
			
			~JointManipulator();
			
			Joint *GetTargetNode(void) const
			{
				return (static_cast<Joint *>(EditorManipulator::GetTargetNode()));
			}
			
			void Preprocess(void);
			
			void Select(void);
			void Unselect(void);
			
			void Render(const ManipulatorRenderData *renderData);
	};
	
	
	class SphericalJointManipulator : public JointManipulator
	{
		public:
			
			SphericalJointManipulator(SphericalJoint *spherical);
			~SphericalJointManipulator();
			
			SphericalJoint *GetTargetNode(void) const
			{
				return (static_cast<SphericalJoint *>(EditorManipulator::GetTargetNode()));
			}
			
			const char *GetDefaultNodeName(void) const;
	};
	
	
	class UniversalJointManipulator : public JointManipulator
	{
		public:
			
			UniversalJointManipulator(UniversalJoint *universal);
			~UniversalJointManipulator();
			
			UniversalJoint *GetTargetNode(void) const
			{
				return (static_cast<UniversalJoint *>(EditorManipulator::GetTargetNode()));
			}
			
			const char *GetDefaultNodeName(void) const;
	};
	
	
	class DiscalJointManipulator : public JointManipulator
	{
		public:
			
			DiscalJointManipulator(DiscalJoint *discal);
			~DiscalJointManipulator();
			
			DiscalJoint *GetTargetNode(void) const
			{
				return (static_cast<DiscalJoint *>(EditorManipulator::GetTargetNode()));
			}
			
			const char *GetDefaultNodeName(void) const;
	};
	
	
	class RevoluteJointManipulator : public JointManipulator
	{
		public:
			
			RevoluteJointManipulator(RevoluteJoint *revolute);
			~RevoluteJointManipulator();
			
			RevoluteJoint *GetTargetNode(void) const
			{
				return (static_cast<RevoluteJoint *>(EditorManipulator::GetTargetNode()));
			}
			
			const char *GetDefaultNodeName(void) const;
	};
	
	
	class CylindricalJointManipulator : public JointManipulator
	{
		public:
			
			CylindricalJointManipulator(CylindricalJoint *cylindrical);
			~CylindricalJointManipulator();
			
			CylindricalJoint *GetTargetNode(void) const
			{
				return (static_cast<CylindricalJoint *>(EditorManipulator::GetTargetNode()));
			}
			
			const char *GetDefaultNodeName(void) const;
	};
	
	
	class PrismaticJointManipulator : public JointManipulator
	{
		public:
			
			PrismaticJointManipulator(PrismaticJoint *prismatic);
			~PrismaticJointManipulator();
			
			PrismaticJoint *GetTargetNode(void) const
			{
				return (static_cast<PrismaticJoint *>(EditorManipulator::GetTargetNode()));
			}
			
			const char *GetDefaultNodeName(void) const;
	};
	
	
	class FieldManipulator : public EditorManipulator
	{
		friend class EditorManipulator;
		
		private:
			
			VolumeManipulator	*volumeManipulator;
			
			static Manipulator *Construct(Field *field);
		
		protected:
			
			FieldManipulator(Field *field, VolumeManipulator *volume);
		
		public:
			
			~FieldManipulator();
			
			Field *GetTargetNode(void) const
			{
				return (static_cast<Field *>(EditorManipulator::GetTargetNode()));
			}
			
			void Select(void);
			void Unselect(void);
			
			bool Pick(const Ray *ray, PickData *data) const;
			bool RegionPick(const Region *region) const;
			
			void Render(const ManipulatorRenderData *renderData);
	};
	
	
	class BoxFieldManipulator : public FieldManipulator, public BoxVolumeManipulator
	{
		private:
			
			bool CalculateNodeSphere(BoundingSphere *sphere) const;
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			BoxFieldManipulator(BoxField *box);
			~BoxFieldManipulator();
			
			BoxField *GetTargetNode(void) const
			{
				return (static_cast<BoxField *>(EditorManipulator::GetTargetNode()));
			}
			
			BoxFieldObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			const char *GetDefaultNodeName(void) const;
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Resize(const ManipulatorResizeData *resizeData);
			void Update(void);
	};
	
	
	class CylinderFieldManipulator : public FieldManipulator, public CylinderVolumeManipulator
	{
		private:
			
			bool CalculateNodeSphere(BoundingSphere *sphere) const;
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			CylinderFieldManipulator(CylinderField *cylinder);
			~CylinderFieldManipulator();
			
			CylinderField *GetTargetNode(void) const
			{
				return (static_cast<CylinderField *>(EditorManipulator::GetTargetNode()));
			}
			
			CylinderFieldObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			const char *GetDefaultNodeName(void) const;
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Resize(const ManipulatorResizeData *resizeData);
			void Update(void);
	};
	
	
	class SphereFieldManipulator : public FieldManipulator, public SphereVolumeManipulator
	{
		private:
			
			bool CalculateNodeSphere(BoundingSphere *sphere) const;
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			SphereFieldManipulator(SphereField *sphere);
			~SphereFieldManipulator();
			
			SphereField *GetTargetNode(void) const
			{
				return (static_cast<SphereField *>(EditorManipulator::GetTargetNode()));
			}
			
			SphereFieldObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			const char *GetDefaultNodeName(void) const;
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Resize(const ManipulatorResizeData *resizeData);
			void Update(void);
	};
}


#endif

// ZYURVUR
