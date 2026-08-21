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


#ifndef C4MarkerManipulators_h
#define C4MarkerManipulators_h


#include "C4EditorManipulators.h"
#include "C4Instances.h"
#include "C4Paths.h"


namespace C4
{
	class MarkerManipulator : public EditorManipulator
	{
		friend class EditorManipulator;
		
		private:
			
			static Manipulator *Construct(Marker *marker);
		
		public:
			
			MarkerManipulator(Marker *marker, const char *iconName);
			~MarkerManipulator();
			
			Marker *GetTargetNode(void) const
			{
				return (static_cast<Marker *>(EditorManipulator::GetTargetNode()));
			}
			
			const char *GetDefaultNodeName(void) const;
			
			void Preprocess(void);
	};
	
	
	class LocatorMarkerManipulator : public MarkerManipulator
	{
		public:
			
			LocatorMarkerManipulator(LocatorMarker *marker);
			~LocatorMarkerManipulator();
			
			LocatorMarker *GetTargetNode(void) const
			{
				return (static_cast<LocatorMarker *>(EditorManipulator::GetTargetNode()));
			}
			
			const char *GetDefaultNodeName(void) const;
	};
	
	
	class PathManipulator : public EditorManipulator
	{
		private:
			
			char					*pathStorage;
			
			int32					pathVertexCount;
			Point3D					*pathVertex;
			Vector4D				*pathTangent;
			Point2D					*pathTexcoord;
			
			int32					tangentVertexCount;
			Point3D					*tangentVertex;
			ColorRGBA				*tangentColor;
			Vector4D				*tangentTangent;
			Point2D					*tangentTexcoord;
			
			int32					pointVertexCount;
			Point3D					*pointVertex;
			ColorRGBA				*pointColor;
			Vector2D				*pointBillboard;
			
			float					*pointSelectionArray;
			int32					maxSelectedPointCount;
			
			Vector4D				pathSizeVector;
			Vector4D				pointSizeVector;
			
			List<Attribute>			pathAttributeList;
			DiffuseAttribute		pathDiffuseColor;
			TextureMapAttribute		pathTextureMap;
			
			List<Attribute>			tangentAttributeList;
			TextureMapAttribute		tangentTextureMap;
			
			Renderable				pathRenderable;
			Renderable				tangentRenderable;
			Renderable				pointRenderable;
			
			bool CalculateNodeSphere(BoundingSphere *sphere) const;
			
			static const PathComponent *GetControlPointComponent(const Path *path, int32 *index);
			void UpdateControlPointSelection(void);
		
		public:
			
			PathManipulator(PathMarker *path);
			~PathManipulator();
			
			PathMarker *GetTargetNode(void) const
			{ 
				return (static_cast<PathMarker *>(EditorManipulator::GetTargetNode()));
			} 
			 
			bool ControlPointSelected(int32 index) const 
			{
				return ((pointSelectionArray) && (pointSelectionArray[index] > 0.0F)); 
			}
			
			const char *GetDefaultNodeName(void) const;
			 
			void Select(void);
			void Unselect(void);
			
			void SelectControlPoint(int32 index, bool selectTangent = false); 
			void UnselectControlPoint(int32 index, bool unselectTangent = false);
			void MoveSelectedControlPoints(const Vector3D& delta, bool maintainTangents = false);
			
			bool PickControlPoint(const Ray *ray, PickData *data) const;
			bool Pick(const Ray *ray, PickData *data) const;
			bool RegionPick(const Region *region) const;
			
			void Update(void);
			void Render(const ManipulatorRenderData *renderData);
	};
}


#endif

// ZYURVUR
