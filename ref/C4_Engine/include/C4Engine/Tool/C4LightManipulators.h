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


#ifndef C4LightManipulators_h
#define C4LightManipulators_h


#include "C4EditorManipulators.h"
#include "C4VolumeManipulators.h"
#include "C4Lights.h"


namespace C4
{
	class LightManipulator : public EditorManipulator
	{
		friend class EditorManipulator;
		
		private:
			
			static Manipulator *Construct(Light *light);
		
		public:
			
			LightManipulator(Light *light, const char *iconName);
			~LightManipulator();
			
			Light *GetTargetNode(void) const
			{
				return (static_cast<Light *>(EditorManipulator::GetTargetNode()));
			}
			
			const char *GetDefaultNodeName(void) const;
			
			void Preprocess(void);
			void Invalidate(void);
	};
	
	
	class InfiniteLightManipulator : public LightManipulator
	{
		private:
			
			List<Attribute>			directionAttributeList;
			DiffuseAttribute		directionDiffuseColor;
			TextureMapAttribute		directionTextureMap;
			Renderable				directionRenderable;
			Vector4D				directionSizeVector;
			
			static const ConstPoint3D	directionVertex[4];
			static const ConstPoint2D	directionTexcoord[4];
			static const float			directionRadius[4];
		
		public:
			
			InfiniteLightManipulator(InfiniteLight *infiniteLight);
			~InfiniteLightManipulator();
			
			InfiniteLight *GetTargetNode(void) const
			{
				return (static_cast<InfiniteLight *>(EditorManipulator::GetTargetNode()));
			}
			
			InfiniteLightObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			void Render(const ManipulatorRenderData *renderData);
	};
	
	
	class PointLightManipulator : public LightManipulator, public SphereVolumeManipulator
	{
		private:
			
			bool CalculateNodeSphere(BoundingSphere *sphere) const;
			
			int32 GetHandleTable(Point3D *handle) const;
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			
			PointLightManipulator(PointLight *pointLight);
			~PointLightManipulator();
			
			PointLight *GetTargetNode(void) const
			{
				return (static_cast<PointLight *>(EditorManipulator::GetTargetNode()));
			}
			
			PointLightObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			void Select(void);
			void Unselect(void);
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Resize(const ManipulatorResizeData *resizeData);
			
			void Update(void);
			void Render(const ManipulatorRenderData *renderData); 
	};
	 
	 
	class SpotLightManipulator : public LightManipulator, public ProjectionVolumeManipulator 
	{
		private: 
			
			bool CalculateNodeSphere(BoundingSphere *sphere) const;
			
			int32 GetHandleTable(Point3D *handle) const; 
			void GetHandleData(int32 index, ManipulatorHandleData *handleData) const;
		
		public:
			 
			SpotLightManipulator(SpotLight *spotLight);
			~SpotLightManipulator();
			
			SpotLight *GetTargetNode(void) const
			{
				return (static_cast<SpotLight *>(EditorManipulator::GetTargetNode()));
			}
			
			SpotLightObject *GetObject(void) const
			{
				return (GetTargetNode()->GetObject());
			}
			
			void Select(void);
			void Unselect(void);
			
			Box3D CalculateNodeBoundingBox(void) const;
			
			bool Resize(const ManipulatorResizeData *resizeData);
			
			void Update(void);
			void Render(const ManipulatorRenderData *renderData);
	};
}


#endif

// ZYURVUR
