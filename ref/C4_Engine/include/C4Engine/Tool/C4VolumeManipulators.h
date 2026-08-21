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


#ifndef C4VolumeManipulators_h
#define C4VolumeManipulators_h


#include "C4EditorManipulators.h"


namespace C4
{
	class VolumeManipulator
	{
		private:
			
			ColorRGBA				interiorColor;
			ColorRGBA				outlineColor;
			Vector4D				volumeSizeVector;
			
			List<Attribute>			interiorAttributeList;
			DiffuseAttribute		interiorDiffuseColor;
			TextureMapAttribute		interiorTextureMap;
			Renderable				interiorRenderable;
			
			List<Attribute>			outlineAttributeList;
			DiffuseAttribute		outlineDiffuseColor;
			TextureMapAttribute		outlineTextureMap;
			Renderable				outlineRenderable;
		
		protected:
			
			VolumeManipulator(Node *node, RenderType renderType, const ColorRGBA& interior, const ColorRGBA& outline, const char *textureName);
			
			void SetVolumeSize(float x, float y, float z)
			{
				volumeSizeVector.GetVector3D().Set(x, y, z);
			}
			
			static bool PolygonVisibleInRegion(int32 vertexCount, const Point3D *vertex, const Region *region);
		
		public:
			
			static const TextureHeader	outlineTextureHeader;
			static const unsigned_int8	outlineTextureImage[15];
			static const ConstPoint2D	outlineTexcoord[272];
			
			virtual ~VolumeManipulator();
			
			Renderable *GetInteriorRenderable(void)
			{
				return (&interiorRenderable);
			}
			
			Renderable *GetOutlineRenderable(void)
			{
				return (&outlineRenderable);
			}
			
			void Select(void);
			void Unselect(void);
			
			bool Pick(const Ray *ray, PickData *data) const;
			bool RegionPick(const Transform4D& worldTransform, const Region *region) const;
			
			void Render(const ManipulatorRenderData *renderData);
	};
	
	
	class PlateVolumeManipulator : public VolumeManipulator
	{
		private:
			
			static const ConstPoint3D	interiorVertex[4];
			static const ConstPoint3D	outlineVertex[16];
			static const ConstVector4D	outlineTangent[16];
			
			Point2D		interiorTexcoord[4];
		
		protected:
			
			PlateVolumeManipulator(Node *node, const ColorRGBA& interior, const ColorRGBA& outline, const char *textureName);
		
		public:
			
			~PlateVolumeManipulator();
			
			void SetPlateSize(const Vector2D& size);
			
			static void CalculateVolumeSphere(const Vector2D& size, BoundingSphere *sphere);
			static Box3D CalculateBoundingBox(const Vector2D& size);
			
			static int32 GetHandleTable(const Vector2D& size, Point3D *handle);
			static void GetHandleData(int32 index, ManipulatorHandleData *handleData);
			
			static bool Resize(const ManipulatorResizeData *resizeData, const Vector2D& oldSize, Vector2D& newSize);
	};
	
	
	class DiskVolumeManipulator : public VolumeManipulator
	{
		private:
			
			static const ConstPoint3D	interiorVertex[16];
			static const Triangle		interiorTriangle[14]; 
			static const ConstPoint3D	outlineVertex[64];
			static const ConstVector4D	outlineTangent[64]; 
			 
			Point2D		interiorTexcoord[16]; 
		
		protected: 
			
			DiskVolumeManipulator(Node *node, const ColorRGBA& interior, const ColorRGBA& outline, const char *textureName);
		
		public: 
			
			~DiskVolumeManipulator();
			
			void SetDiskSize(float radius) 
			{
				SetDiskSize(Vector2D(radius, radius));
			}
			
			void SetDiskSize(const Vector2D& size);
			
			static void CalculateVolumeSphere(float radius, BoundingSphere *sphere);
			static void CalculateVolumeSphere(const Vector2D& size, BoundingSphere *sphere);
			
			static Box3D CalculateBoundingBox(float radius);
			static Box3D CalculateBoundingBox(const Vector2D& size);
			
			static int32 GetHandleTable(float radius, Point3D *handle);
			static int32 GetHandleTable(const Vector2D& size, Point3D *handle);
			static void GetHandleData(int32 index, ManipulatorHandleData *handleData);
			
			static bool Resize(const ManipulatorResizeData *resizeData, float oldRadius, float& newRadius);
			static bool Resize(const ManipulatorResizeData *resizeData, const Vector2D& oldSize, Vector2D& newSize);
	};
	
	
	class BoxVolumeManipulator : public VolumeManipulator
	{
		private:
			
			static const ConstPoint3D	interiorVertex[24];
			
			Point2D		interiorTexcoord[24];
		
		protected:
			
			BoxVolumeManipulator(Node *node, const ColorRGBA& interior, const ColorRGBA& outline, const char *textureName);
		
		public:
			
			static const ConstPoint3D	outlineVertex[48];
			static const ConstVector4D	outlineTangent[48];
			
			~BoxVolumeManipulator();
			
			void SetBoxSize(const Vector3D& size);
			
			static void CalculateVolumeSphere(const Vector3D& size, BoundingSphere *sphere);
			static Box3D CalculateBoundingBox(const Vector3D& size);
			
			static int32 GetHandleTable(const Vector3D& size, Point3D *handle);
			static void GetHandleData(int32 index, ManipulatorHandleData *handleData);
			
			static bool Resize(const ManipulatorResizeData *resizeData, const Vector3D& oldSize, Vector3D& newSize);
	};
	
	
	class PyramidVolumeManipulator : public VolumeManipulator
	{
		private:
			
			static const ConstPoint3D	interiorVertex[16];
			static const Triangle		interiorTriangle[6];
			static const ConstPoint3D	outlineVertex[32];
			static const ConstVector4D	outlineTangent[32];
			
			Point2D		interiorTexcoord[16];
		
		protected:
			
			PyramidVolumeManipulator(Node *node, const ColorRGBA& interior, const ColorRGBA& outline, const char *textureName);
		
		public:
			
			~PyramidVolumeManipulator();
			
			void SetPyramidSize(const Vector2D& size, float height);
			
			static void CalculateVolumeSphere(const Vector2D& size, float height, BoundingSphere *sphere);
			static Box3D CalculateBoundingBox(const Vector2D& size, float height);
			
			static int32 GetHandleTable(const Vector2D& size, float height, Point3D *handle);
			static void GetHandleData(int32 index, ManipulatorHandleData *handleData);
			
			static bool Resize(const ManipulatorResizeData *resizeData, const Vector2D& oldSize, float oldHeight, Vector2D& newSize, float& newHeight);
	};
	
	
	class CylinderVolumeManipulator : public VolumeManipulator
	{
		private:
			
			static const ConstPoint3D	interiorVertex[66];
			static const Triangle		interiorTriangle[60];
			
			Point2D		interiorTexcoord[66];
		
		protected:
			
			CylinderVolumeManipulator(Node *node, const ColorRGBA& interior, const ColorRGBA& outline, const char *textureName);
		
		public:
			
			static const ConstPoint3D	outlineVertex[144];
			static const ConstVector4D	outlineTangent[144];
			
			~CylinderVolumeManipulator();
			
			void SetCylinderSize(float radius, float height)
			{
				SetCylinderSize(Vector2D(radius, radius), height);
			}
			
			void SetCylinderSize(const Vector2D& size, float height);
			
			static void CalculateVolumeSphere(float radius, float height, BoundingSphere *sphere);
			static void CalculateVolumeSphere(const Vector2D& size, float height, BoundingSphere *sphere);
			
			static Box3D CalculateBoundingBox(float radius, float height);
			static Box3D CalculateBoundingBox(const Vector2D& size, float height);
			
			static int32 GetHandleTable(float radius, float height, Point3D *handle);
			static int32 GetHandleTable(const Vector2D& size, float height, Point3D *handle);
			static void GetHandleData(int32 index, ManipulatorHandleData *handleData);
			
			static bool Resize(const ManipulatorResizeData *resizeData, float oldRadius, float oldHeight, float& newRadius, float& newHeight);
			static bool Resize(const ManipulatorResizeData *resizeData, const Vector2D& oldSize, float oldHeight, Vector2D& newSize, float& newHeight);
	};
	
	
	class ConeVolumeManipulator : public VolumeManipulator
	{
		private:
			
			static const ConstPoint3D	interiorVertex[49];
			static const Triangle		interiorTriangle[30];
			static const ConstPoint3D	outlineVertex[80];
			static const ConstVector4D	outlineTangent[80];
			
			Point2D		interiorTexcoord[49];
		
		protected:
			
			ConeVolumeManipulator(Node *node, const ColorRGBA& interior, const ColorRGBA& outline, const char *textureName);
		
		public:
			
			~ConeVolumeManipulator();
			
			void SetConeSize(float radius, float height)
			{
				SetConeSize(Vector2D(radius, radius), height);
			}
			
			void SetConeSize(const Vector2D& size, float height);
			
			static void CalculateVolumeSphere(float radius, float height, BoundingSphere *sphere);
			static void CalculateVolumeSphere(const Vector2D& size, float height, BoundingSphere *sphere);
			
			static Box3D CalculateBoundingBox(float radius, float height);
			static Box3D CalculateBoundingBox(const Vector2D& size, float height);
			
			static int32 GetHandleTable(float radius, float height, Point3D *handle);
			static int32 GetHandleTable(const Vector2D& size, float height, Point3D *handle);
			static void GetHandleData(int32 index, ManipulatorHandleData *handleData);
			
			static bool Resize(const ManipulatorResizeData *resizeData, float oldRadius, float oldHeight, float& newRadius, float& newHeight);
			static bool Resize(const ManipulatorResizeData *resizeData, const Vector2D& oldSize, float oldHeight, Vector2D& newSize, float& newHeight);
	};
	
	
	class SphereVolumeManipulator : public VolumeManipulator
	{
		private:
			
			static const ConstPoint3D	interiorVertex[151];
			static const Triangle		interiorTriangle[224];
			static const ConstPoint3D	outlineVertex[192];
			static const ConstVector4D	outlineTangent[192];
			
			Point2D		interiorTexcoord[151];
		
		protected:
			
			SphereVolumeManipulator(Node *node, const ColorRGBA& interior, const ColorRGBA& outline, const char *textureName);
		
		public:
			
			~SphereVolumeManipulator();
			
			void SetSphereSize(float radius)
			{
				SetSphereSize(Vector3D(radius, radius, radius));
			}
			
			void SetSphereSize(const Vector3D& size);
			
			static void CalculateVolumeSphere(float radius, BoundingSphere *sphere);
			static void CalculateVolumeSphere(const Vector3D& size, BoundingSphere *sphere);
			
			static Box3D CalculateBoundingBox(float radius);
			static Box3D CalculateBoundingBox(const Vector3D& size);
			
			static int32 GetHandleTable(float radius, Point3D *handle);
			static int32 GetHandleTable(const Vector3D& size, Point3D *handle);
			static void GetHandleData(int32 index, ManipulatorHandleData *handleData);
			static void GetCircularHandleData(int32 index, ManipulatorHandleData *handleData);
			
			static bool Resize(const ManipulatorResizeData *resizeData, float oldRadius, float& newRadius);
			static bool Resize(const ManipulatorResizeData *resizeData, const Vector3D& oldSize, Vector3D& newSize);
	};
	
	
	class DomeVolumeManipulator : public VolumeManipulator
	{
		private:
			
			static const ConstPoint3D	interiorVertex[100];
			static const Triangle		interiorTriangle[126];
			static const ConstPoint3D	outlineVertex[128];
			static const ConstVector4D	outlineTangent[128];
			
			Point2D		interiorTexcoord[100];
		
		protected:
			
			DomeVolumeManipulator(Node *node, const ColorRGBA& interior, const ColorRGBA& outline, const char *textureName);
		
		public:
			
			~DomeVolumeManipulator();
			
			void SetDomeSize(float radius)
			{
				SetDomeSize(Vector3D(radius, radius, radius));
			}
			
			void SetDomeSize(const Vector3D& size);
			
			static void CalculateVolumeSphere(float radius, BoundingSphere *sphere);
			static void CalculateVolumeSphere(const Vector3D& size, BoundingSphere *sphere);
			
			static Box3D CalculateBoundingBox(float radius);
			static Box3D CalculateBoundingBox(const Vector3D& size);
			
			static int32 GetHandleTable(float radius, Point3D *handle);
			static int32 GetHandleTable(const Vector3D& size, Point3D *handle);
			static void GetHandleData(int32 index, ManipulatorHandleData *handleData);
			
			static bool Resize(const ManipulatorResizeData *resizeData, float oldRadius, float& newRadius);
			static bool Resize(const ManipulatorResizeData *resizeData, const Vector3D& oldSize, Vector3D& newSize);
	};
	
	
	class CapsuleVolumeManipulator : public VolumeManipulator
	{
		private:
			
			static const ConstVector4D	prototypeInteriorVertex[168];
			static const Triangle		interiorTriangle[256];
			static const ConstVector4D	prototypeOutlineVertex[272];
			static const ConstVector4D	outlineTangent[272];
			
			Point3D		interiorVertex[168];
			Point2D		interiorTexcoord[168];
			Point3D		outlineVertex[272];
		
		protected:
			
			CapsuleVolumeManipulator(Node *node, const ColorRGBA& interior, const ColorRGBA& outline, const char *textureName);
		
		public:
			
			~CapsuleVolumeManipulator();
			
			void SetCapsuleSize(float radius, float height)
			{
				SetCapsuleSize(Vector3D(radius, radius, radius), height);
			}
			
			void SetCapsuleSize(const Vector3D& size, float height);
			
			static void CalculateVolumeSphere(float radius, float height, BoundingSphere *sphere);
			static void CalculateVolumeSphere(const Vector3D& size, float height, BoundingSphere *sphere);
			
			static Box3D CalculateBoundingBox(float radius, float height);
			static Box3D CalculateBoundingBox(const Vector3D& size, float height);
			
			static int32 GetHandleTable(float radius, float height, Point3D *handle);
			static int32 GetHandleTable(const Vector3D& size, float height, Point3D *handle);
			static void GetHandleData(int32 index, ManipulatorHandleData *handleData);
			
			static bool Resize(const ManipulatorResizeData *resizeData, float oldRadius, float oldHeight, float& newRadius, float& newHeight);
			static bool Resize(const ManipulatorResizeData *resizeData, const Vector3D& oldSize, float oldHeight, Vector3D& newSize, float& newHeight);
	};
	
	
	class TruncatedPyramidVolumeManipulator : public VolumeManipulator
	{
		private:
			
			static const ConstPoint3D	prototypeInteriorVertex[24];
			static const ConstPoint3D	prototypeOutlineVertex[48];
			static const ConstVector4D	outlineTangent[48];
			
			Point3D		interiorVertex[24];
			Point2D		interiorTexcoord[24];
			Point3D		outlineVertex[48];
		
		protected:
			
			TruncatedPyramidVolumeManipulator(Node *node, const ColorRGBA& interior, const ColorRGBA& outline, const char *textureName);
		
		public:
			
			~TruncatedPyramidVolumeManipulator();
			
			void SetTruncatedPyramidSize(const Vector2D& size, float height, float ratio);
			
			static void CalculateVolumeSphere(const Vector2D& size, float height, float ratio, BoundingSphere *sphere);
			static Box3D CalculateBoundingBox(const Vector2D& size, float height);
			
			static int32 GetHandleTable(const Vector2D& size, float height, float ratio, Point3D *handle);
			static void GetHandleData(int32 index, ManipulatorHandleData *handleData);
			
			static bool Resize(const ManipulatorResizeData *resizeData, const Vector2D& oldSize, float oldHeight, float oldRatio, Vector2D& newSize, float& newHeight, float& newRatio);
	};
	
	
	class TruncatedConeVolumeManipulator : public VolumeManipulator
	{
		private:
			
			static const ConstPoint3D	prototypeInteriorVertex[66];
			static const Triangle		interiorTriangle[60];
			static const ConstPoint3D	prototypeOutlineVertex[144];
			static const ConstVector4D	outlineTangent[144];
			
			Point3D		interiorVertex[66];
			Point2D		interiorTexcoord[66];
			Point3D		outlineVertex[144];
		
		protected:
			
			TruncatedConeVolumeManipulator(Node *node, const ColorRGBA& interior, const ColorRGBA& outline, const char *textureName);
		
		public:
			
			~TruncatedConeVolumeManipulator();
			
			void SetTruncatedConeSize(float radius, float height, float ratio)
			{
				SetTruncatedConeSize(Vector2D(radius, radius), height, ratio);
			}
			
			void SetTruncatedConeSize(const Vector2D& size, float height, float ratio);
			
			static void CalculateVolumeSphere(float radius, float height, float ratio, BoundingSphere *sphere);
			static void CalculateVolumeSphere(const Vector2D& size, float height, float ratio, BoundingSphere *sphere);
			
			static Box3D CalculateBoundingBox(float radius, float height);
			static Box3D CalculateBoundingBox(const Vector2D& size, float height);
			
			static int32 GetHandleTable(float radius, float height, float ratio, Point3D *handle);
			static int32 GetHandleTable(const Vector2D& size, float height, float ratio, Point3D *handle);
			static void GetHandleData(int32 index, ManipulatorHandleData *handleData);
			
			static bool Resize(const ManipulatorResizeData *resizeData, float oldRadius, float oldHeight, float oldRatio, float& newRadius, float& newHeight, float& newRatio);
			static bool Resize(const ManipulatorResizeData *resizeData, const Vector2D& oldSize, float oldHeight, float oldRatio, Vector2D& newSize, float& newHeight, float& newRatio);
	};
	
	
	class TruncatedDomeVolumeManipulator : public VolumeManipulator
	{
		private:
			
			static const ConstPoint3D	prototypeInteriorVertex[100];
			static const Triangle		interiorTriangle[124];
			static const ConstPoint3D	prototypeOutlineVertex[176];
			static const ConstVector4D	outlineTangent[176];
			
			Point3D		interiorVertex[100];
			Point2D		interiorTexcoord[100];
			Point3D		outlineVertex[176];
		
		protected:
			
			TruncatedDomeVolumeManipulator(Node *node, const ColorRGBA& interior, const ColorRGBA& outline, const char *textureName);
		
		public:
			
			~TruncatedDomeVolumeManipulator();
			
			void SetTruncatedDomeSize(float radius, float height, float ratio)
			{
				SetTruncatedDomeSize(Vector2D(radius, radius), height, ratio);
			}
			
			void SetTruncatedDomeSize(const Vector2D& size, float height, float ratio);
			
			static void CalculateVolumeSphere(float radius, float height, float ratio, BoundingSphere *sphere);
			static void CalculateVolumeSphere(const Vector2D& size, float height, float ratio, BoundingSphere *sphere);
			
			static Box3D CalculateBoundingBox(float radius, float height);
			static Box3D CalculateBoundingBox(const Vector2D& size, float height);
			
			static int32 GetHandleTable(float radius, float height, float ratio, Point3D *handle);
			static int32 GetHandleTable(const Vector2D& size, float height, float ratio, Point3D *handle);
			static void GetHandleData(int32 index, ManipulatorHandleData *handleData);
			
			static bool Resize(const ManipulatorResizeData *resizeData, float oldRadius, float oldHeight, float oldRatio, float& newRadius, float& newHeight, float& newRatio);
			static bool Resize(const ManipulatorResizeData *resizeData, const Vector2D& oldSize, float oldHeight, float oldRatio, Vector2D& newSize, float& newHeight, float& newRatio);
	};
	
	
	class ProjectionVolumeManipulator : public VolumeManipulator
	{
		private:
			
			static const ConstPoint3D	interiorVertex[16];
			static const Triangle		interiorTriangle[6];
			static const ConstPoint3D	outlineVertex[32];
			static const ConstVector4D	outlineTangent[32];
			
			Point2D		interiorTexcoord[16];
		
		protected:
			
			ProjectionVolumeManipulator(Node *node, const ColorRGBA& interior, const ColorRGBA& outline, const char *textureName);
		
		public:
			
			~ProjectionVolumeManipulator();
			
			void SetProjectionSize(const Vector2D& size, float height);
			
			static void CalculateVolumeSphere(const Vector2D& size, float height, BoundingSphere *sphere);
			static Box3D CalculateBoundingBox(const Vector2D& size, float height);
			
			static int32 GetHandleTable(const Vector2D& size, float height, Point3D *handle);
			static void GetHandleData(int32 index, ManipulatorHandleData *handleData);
			
			static bool Resize(const ManipulatorResizeData *resizeData, const Vector2D& oldSize, float oldHeight, Vector2D& newSize, float& newHeight);
	};
}


#endif

// ZYURVUR
