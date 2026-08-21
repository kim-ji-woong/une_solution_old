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


#ifndef C4Spaces_h
#define C4Spaces_h


//# \component	World Manager
//# \prefix		WorldMgr/

//# \import		C4SpaceObjects.h


#include "C4SpaceObjects.h"
#include "C4Node.h"


namespace C4
{
	struct Line;
	class SoundRoom;
	class CameraRegion;
	class FrustumCamera;
	class OrthoCamera;
	
	
	//# \class	Space	Represents a space node in a world.
	//
	//# The $Space$ class represents a space node in a world.
	//
	//# \def	class Space : public Node
	//
	//# \ctor	Space(SpaceType type);
	//
	//# The constructor has protected access. A $Space$ class can only exist as the base class for a more specific type of space.
	//
	//# \param	type	The type of the space. See below for a list of possible types.
	//
	//# \desc
	//# 
	//# \table	SpaceType
	//
	//# \base	Node	A $Space$ node is a scene graph node.
	//
	//# \also	$@SpaceObject@$
	
	
	//# \function	Space::GetSpaceType		Returns the specific type of a space.
	//
	//# \proto	SpaceType GetSpaceType(void) const;
	//
	//# \desc
	//# The $GetSpaceType$ function returns the specific space type, which may be one of the following values.
	//
	//# \table	SpaceType
	
	
	class Space : public Node
	{
		friend class Node;
		
		private:
			
			SpaceType		spaceType;
			
			static Space *Construct(Unpacker& data, unsigned_int32 unpackFlags);
			
			bool CalculateBoundingBox(Box3D *box) const override;
			bool CalculateBoundingSphere(BoundingSphere *sphere) const override;
		
		protected:
			
			Space(SpaceType type);
			Space(const Space& space);
		
		public:
			
			virtual ~Space();
			
			SpaceType GetSpaceType(void) const
			{
				return (spaceType);
			}
			
			SpaceObject *GetObject(void) const
			{
				return (static_cast<SpaceObject *>(Node::GetObject()));
			}
			
			void PackType(Packer& data) const;
	};
	
	
	//# \class	FogSpace		Represents a fog space node in a world.
	//
	//# The $FogSpace$ class represents a fog space node in a world.
	//
	//# \def	class FogSpace : public Space, public ListElement<FogSpace>
	//
	//# \ctor	FogSpace(const Vector2D& size);
	//
	//# \param	size	The size of the plane.
	//
	//# \desc
	//# 
	//
	//# \base	Space								A fog space is a specific type of space. 
	//# \base	Utilities/ListElement<FogSpace>		Used internally by the World Manager. 
	// 
	//# \also	$@FogSpaceObject@$
	 
	
	class FogSpace : public Space, public ListElement<FogSpace>
	{
		friend class Space; 
		
		private:
			
			Point3D		worldVertex[4]; 
			
			FogSpace();
			FogSpace(const FogSpace& fogSpace);
			
			Node *Replicate(void) const override;
			
			static bool FogVisible(const Node *node, const Region *region);
			static bool FogOccluded(const Node *node, const Region *region);
			
			void CalculatePostTransform(void) override;
		
		public:
			
			C4API FogSpace(const Vector2D& size);
			C4API ~FogSpace();
			
			using ListElement<FogSpace>::Previous;
			using ListElement<FogSpace>::Next;
			
			FogSpaceObject *GetObject(void) const
			{
				return (static_cast<FogSpaceObject *>(Node::GetObject()));
			}
			
			void Preprocess(void);
			void Neutralize(void);
			void EnterZone(Zone *zone);
	};
	
	
	//# \class	ShadowSpace		Represents a shadow space node in a world.
	//
	//# The $ShadowSpace$ class represents a shadow space node in a world.
	//
	//# \def	class ShadowSpace : public Space
	//
	//# \ctor	ShadowSpace(const Vector3D& size);
	//
	//# \param	size	The size of the box.
	//
	//# \desc
	//#
	//
	//# \base	Space		A shadow space is a specific type of space.
	//
	//# \also	$@ShadowSpaceObject@$
	
	
	class ShadowSpace : public Space
	{
		friend class Space;
		
		private:
			
			ShadowSpace();
			ShadowSpace(const ShadowSpace& shadowSpace);
			
			Node *Replicate(void) const override;
			
			static bool ClipSegmentToPlanes(Point3D& p1, Point3D& p2, int32 planeCount, const Antivector4D *plane);
		
		public:
			
			C4API ShadowSpace(const Vector3D& size);
			C4API ~ShadowSpace();
			
			ShadowSpaceObject *GetObject(void) const
			{
				return (static_cast<ShadowSpaceObject *>(Node::GetObject()));
			}
			
			int32 ClipToShadowBounds(int32 planeCount, const Antivector4D *plane, Point3D *vertex, Line *line, int32 baseIndex) const;
	};
	
	
	//# \class	AmbientSpace		Represents an ambient space node in a world.
	//
	//# The $AmbientSpace$ class represents an ambient space node in a world.
	//
	//# \def	class AmbientSpace : public Space
	//
	//# \ctor	AmbientSpace(const Vector3D& size, int32 x, int32 y, int32 z, const char *name);
	//
	//# \param	size	The size of the box.
	//# \param	x		The width of the ambient texture.
	//# \param	y		The height of the ambient texture.
	//# \param	z		The depth of the ambient texture.
	//# \param	name	The name of the ambient texture.
	//
	//# \desc
	//#
	//
	//# \base	Space		An ambient space is a specific type of space.
	//
	//# \also	$@AmbientSpaceObject@$
	
	
	class AmbientSpace : public Space
	{
		friend class Space;
		
		private:
			
			AmbientSpace();
			AmbientSpace(const AmbientSpace& ambientSpace);
			
			Node *Replicate(void) const override;
		
		public:
			
			C4API AmbientSpace(const Vector3D& size, int32 x, int32 y, int32 z, const char *name);
			C4API ~AmbientSpace();
			
			AmbientSpaceObject *GetObject(void) const
			{
				return (static_cast<AmbientSpaceObject *>(Node::GetObject()));
			}
			
			void EnterZone(Zone *zone);
	};
	
	
	//# \class	AcousticsSpace		Represents an acoustic space node in a world.
	//
	//# The $AcousticsSpace$ class represents an acoustic space node in a world.
	//
	//# \def	class AcousticsSpace : public Space
	//
	//# \ctor	AcousticsSpace(const Vector3D& size);
	//
	//# \param	size	The size of the box.
	//
	//# \desc
	//#
	//
	//# \base	Space		An acoustic space is a specific type of space.
	//
	//# \also	$@AcousticsSpaceObject@$
	
	
	class AcousticsSpace : public Space
	{
		friend class Space;
		
		private:
			
			SoundRoom	*soundRoom;
			
			AcousticsSpace();
			AcousticsSpace(const AcousticsSpace& acousticsSpace);
			
			Node *Replicate(void) const override;
		
		public:
			
			C4API AcousticsSpace(const Vector3D& size);
			C4API ~AcousticsSpace();
			
			AcousticsSpaceObject *GetObject(void) const
			{
				return (static_cast<AcousticsSpaceObject *>(Node::GetObject()));
			}
			
			SoundRoom *GetSoundRoom(void) const
			{
				return (soundRoom);
			}
			
			void Preprocess(void);
			void EnterZone(Zone *zone);
	};
	
	
	//# \class	OcclusionSpace		Represents an occlusion space node in a world.
	//
	//# The $OcclusionSpace$ class represents an occlusion space node in a world.
	//
	//# \def	class OcclusionSpace : public Space, public ListElement<OcclusionSpace>
	//
	//# \ctor	OcclusionSpace(const Vector3D& size);
	//
	//# \param	size	The size of the box.
	//
	//# \desc
	//#
	//
	//# \base	Space									An occlusion space is a specific type of space.
	//# \base	Utilities/ListElement<OcclusionSpace>	Used internally by the World Manager.
	//
	//# \also	$@OcclusionSpaceObject@$
	
	
	class OcclusionSpace : public Space, public ListElement<OcclusionSpace>
	{
		friend class Space;
		
		private:
			
			Point3D		worldCenter;
			Vector3D	worldAxis[3];
			
			OcclusionSpace();
			OcclusionSpace(const OcclusionSpace& occlusionSpace);
			
			Node *Replicate(void) const override;
			
			static bool BoxVisible(const Node *node, const Region *region);
			
			void CalculatePostTransform(void) override;
		
		public:
			
			C4API OcclusionSpace(const Vector3D& size);
			C4API ~OcclusionSpace();
			
			using ListElement<OcclusionSpace>::Previous;
			using ListElement<OcclusionSpace>::Next;
			
			OcclusionSpaceObject *GetObject(void) const
			{
				return (static_cast<OcclusionSpaceObject *>(Node::GetObject()));
			}
			
			void Neutralize(void);
			void EnterZone(Zone *zone);
			
			CameraRegion *CalculateFrustumOcclusionRegion(const FrustumCamera *camera, Zone *zone) const;
			CameraRegion *CalculateOrthoOcclusionRegion(const OrthoCamera *camera, Zone *zone) const;
	};
	
	
	//# \class	PaintSpace		Represents a paint space node in a world.
	//
	//# The $PaintSpace$ class represents a paint space node in a world.
	//
	//# \def	class PaintSpace : public Space
	//
	//# \ctor	PaintSpace(const Vector3D& size, const Integer2D& resolution);
	//
	//# \param	size		The size of the box.
	//# \param	resolution	The resolution of the paint texture. This must be a power of two between the values of $kPaintMinResolution$ and $kPaintMaxResolution$, inclusive.
	//
	//# \desc
	//#
	//
	//# \base	Space		A paint space is a specific type of space.
	//
	//# \also	$@PaintSpaceObject@$
	
	
	class PaintSpace : public Space
	{
		friend class Space;
		
		private:
			
			PaintEnvironment		paintEnvironment;
			
			PaintSpace();
			PaintSpace(const PaintSpace& paintSpace);
			
			Node *Replicate(void) const override;
			
			void CalculatePostTransform(void) override;
		
		public:
			
			C4API PaintSpace(const Vector3D& size, const Integer2D& resolution, int32 count);
			C4API ~PaintSpace();
			
			PaintSpaceObject *GetObject(void) const
			{
				return (static_cast<PaintSpaceObject *>(Node::GetObject()));
			}
			
			const PaintEnvironment *GetPaintEnvironment(void) const
			{
				return (&paintEnvironment);
			}
			
			void Preprocess(void);
			void Neutralize(void);
	};
}


#endif

// ZYURVUR
