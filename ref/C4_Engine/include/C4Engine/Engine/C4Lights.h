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


#ifndef C4Lights_h
#define C4Lights_h


//# \component	World Manager
//# \prefix		WorldMgr/

//# \import		C4LightObjects.h


#include "C4LightObjects.h"
#include "C4Regions.h"
#include "C4Spaces.h"


namespace C4
{
	C4API extern const char kConnectorKeyShadow[];
	
	
	class FrustumCamera;
	
	
	//# \class	Light	Represents a light node in a world.
	//
	//# The $Light$ class represents a light node in a world.
	//
	//# \def	class Light : public Node
	//
	//# \ctor	Light(LightType type, LightType base);
	//
	//# The constructor has protected access. A $Light$ class can only exist as the base class for a more specific type of light.
	//
	//# \param	type	The type of the light source. See below for a list of possible types.
	//# \param	base	The base type of the light source. This should be $kLightInfinite$ or $kLightPoint$.
	//
	//# \desc
	//# 
	//# \table	LightType
	//
	//# \base	Node	A $Light$ node is a scene graph node.
	//
	//# \also	$@GraphicsMgr/LightObject@$
	
	
	//# \function	Light::GetLightType		Returns the specific type of a light.
	//
	//# \proto	LightType GetLightType(void) const;
	//
	//# \desc
	//# The $GetLightType$ function returns the specific light type, which may be one of the following values.
	//
	//# \table	LightType
	//
	//# All of the light types are divided into two categories, and the general category that a light object
	//# falls into can be determined by calling the $@Light::GetBaseLightType@$ function.
	//
	//# \also	$@Light::GetBaseLightType@$
	
	
	//# \function	Light::GetBaseLightType		Returns the base type of a light.
	//
	//# \proto	LightType GetBaseLightType(void) const;
	//
	//# \desc
	//# The $GetBaseLightType$ function returns the base light type. See the $@GraphicsMgr/LightObject::GetBaseLightType@$
	//# function for details about the base type.
	//
	//# \also	$@Light::GetLightType@$
	
	
	class Light : public Node
	{
		friend class Node;
		
		private:
			
			LightType				lightType;
			LightType				baseLightType;
			
			ShadowSpace				*connectedShadowSpace;
			Link<Node>				exclusionNode;
			
			List<StencilVolume>		stencilVolumeList;
			
			static Light *Construct(Unpacker& data, unsigned_int32 unpackFlags);
			
			#if C4LEGACY
			
				static void ShadowSpaceLinkProc(Node *node, void *cookie);
			
			#endif
			
			static void ExcludeLinkProc(Node *node, void *cookie);
		
		protected:
			
			ZoneRegion				*rootRegion;
			
			Light(LightType type, LightType base);
			Light(const Light& light);
		 
		public:
			 
			virtual ~Light(); 
			 
			LightType GetLightType(void) const
			{ 
				return (lightType);
			}
			
			LightType GetBaseLightType(void) const 
			{
				return (baseLightType);
			}
			 
			LightObject *GetObject(void) const
			{
				return (static_cast<LightObject *>(Node::GetObject()));
			}
			
			ShadowSpace *GetConnectedShadowSpace(void) const
			{
				return (connectedShadowSpace);
			}
			
			Node *GetExclusionNode(void) const
			{
				return (exclusionNode);
			}
			
			void SetExclusionNode(Node *node)
			{
				exclusionNode = node;
			}
			
			void AddStencilVolume(StencilVolume *stencilVolume)
			{
				stencilVolumeList.Append(stencilVolume);
			}
			
			void InvalidateStaticShadowVolumes(void)
			{
				stencilVolumeList.Purge();
			}
			
			const ZoneRegion *GetRootRegion(void) const
			{
				return (rootRegion);
			}
			
			void PackType(Packer& data) const;
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetCategorySettingCount(Type category) const;
			Setting *GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const;
			void SetCategorySetting(Type category, const Setting *setting);
			
			int32 GetInternalConnectorCount(void) const;
			const char *GetInternalConnectorKey(int32 index) const;
			void ProcessInternalConnectors(void);
			bool ValidConnectedNode(const ConnectorKey& key, const Node *node) const;
			C4API void SetConnectedShadowSpace(ShadowSpace *shadowSpace);
			
			void Neutralize(void);
			
			C4API void InvalidateLightRegions(void);
			
			virtual void CalculateBoundaryPolygons(LightRegion *region) const;
	};
	
	
	//# \class	InfiniteLight	Represents an infinite light node in a world.
	//
	//# The $InfiniteLight$ class represents an infinite light node in a world.
	//
	//# \def	class InfiniteLight : public Light
	//
	//# \ctor	InfiniteLight(const ColorRGB& color);
	//
	//# \param	color	The color of light emitted by the light source.
	//
	//# \desc
	//#
	//
	//# \base	Light	An infinite light is a type of light.
	//
	//# \also	$@GraphicsMgr/InfiniteLightObject@$
	
	
	class InfiniteLight : public Light
	{
		friend class Light;
		
		private:
			
			InfiniteLight();
			
			Node *Replicate(void) const override;
			
			void CalculateIllumination(LightRegion *region);
		
		protected:
			
			InfiniteLight(LightType type);
			InfiniteLight(const InfiniteLight& infiniteLight);
			
			void CalculatePostBounding(void) override;
		
		public:
			
			C4API InfiniteLight(const ColorRGB& color);
			C4API ~InfiniteLight();
			
			InfiniteLightObject *GetObject(void) const
			{
				return (static_cast<InfiniteLightObject *>(Node::GetObject()));
			}
			
			void CalculateBoundaryPolygons(LightRegion *region) const;
	};
	
	
	//# \class	DepthLight		Represents a depth light node in a world.
	//
	//# The $DepthLight$ class represents a depth light node in a world.
	//
	//# \def	class DepthLight : public InfiniteLight
	//
	//# \ctor	DepthLight(const ColorRGB& color);
	//
	//# \param	color	The color of light emitted by the light source.
	//
	//# \desc
	//#
	//
	//# \base	InfiniteLight	A depth light is a special type of infinite light.
	//
	//# \also	$@GraphicsMgr/DepthLightObject@$
	
	
	class DepthLight : public InfiniteLight
	{
		friend class Light;
		
		private:
			
			LightShadowData		shadowData;
			
			DepthLight();
			
			Node *Replicate(void) const override;
		
		protected:
			
			DepthLight(LightType type);
			DepthLight(const DepthLight& depthLight);
		
		public:
			
			C4API DepthLight(const ColorRGB& color);
			C4API ~DepthLight();
			
			DepthLightObject *GetObject(void) const
			{
				return (static_cast<DepthLightObject *>(Node::GetObject()));
			}
			
			virtual const LightShadowData *CalculateShadowData(const FrustumCamera *camera);
	};
	
	
	//# \class	LandscapeLight		Represents a landscape light node in a world.
	//
	//# The $LandscapeLight$ class represents a landscape light node in a world.
	//
	//# \def	class LandscapeLight : public InfiniteLight
	//
	//# \ctor	LandscapeLight(const ColorRGB& color);
	//
	//# \param	color	The color of light emitted by the light source.
	//
	//# \desc
	//#
	//
	//# \base	DepthLight		A landscape light is a special type of depth light.
	//
	//# \also	$@GraphicsMgr/LandscapeLightObject@$
	
	
	class LandscapeLight : public DepthLight
	{
		friend class Light;
		
		private:
			
			LightShadowData		shadowData[kMaxShadowSectionCount];
			
			LandscapeLight();
			LandscapeLight(const LandscapeLight& landscapeLight);
			
			Node *Replicate(void) const override;
		
		public:
			
			C4API LandscapeLight(const ColorRGB& color);
			C4API ~LandscapeLight();
			
			LandscapeLightObject *GetObject(void) const
			{
				return (static_cast<LandscapeLightObject *>(Node::GetObject()));
			}
			
			const LightShadowData *CalculateShadowData(const FrustumCamera *camera);
	};
	
	
	//# \class	PointLight	Represents a point light node in a world.
	//
	//# The $PointLight$ class represents a point light node in a world.
	//
	//# \def	class PointLight : public Light
	//
	//# \ctor	PointLight(const ColorRGB& color, float range);
	//
	//# \param	color	The color of light emitted by the light source.
	//# \param	range	The spherical range of the light source.
	//
	//# \desc
	//#
	//
	//# \base	Light	A point light is a type of light.
	//
	//# \also	$@GraphicsMgr/PointLightObject@$
	
	
	class PointLight : public Light
	{
		friend class Light;
		
		private:
			
			PointLight();
			
			Node *Replicate(void) const override;
			
			void CalculatePostBounding(void) override;
		
		protected:
			
			PointLight(LightType type);
			PointLight(const PointLight& pointLight);
			
			void CalculateIllumination(LightRegion *region);
		
		public:
			
			C4API PointLight(const ColorRGB& color, float range);
			C4API ~PointLight();
			
			PointLightObject *GetObject(void) const
			{
				return (static_cast<PointLightObject *>(Node::GetObject()));
			}
			
			void CalculateBoundaryPolygons(LightRegion *region) const;
	};
	
	
	//# \class	CubeLight	Represents a cube light node in a world.
	//
	//# The $CubeLight$ class represents a cube light node in a world.
	//
	//# \def	class CubeLight : public PointLight
	//
	//# \ctor	CubeLight(const ColorRGB& color, float range, const char *name);
	//
	//# \param	color	The color of light emitted by the light source.
	//# \param	range	The spherical range of the light source.
	//# \param	name	The name of the projected shadow texture map.
	//
	//# \desc
	//#
	//
	//# \base	PointLight	A cube light is a special type of point light.
	//
	//# \also	$@GraphicsMgr/CubeLightObject@$
	
	
	class CubeLight : public PointLight
	{
		friend class Light;
		
		private:
			
			CubeLight();
			CubeLight(const CubeLight& cubeLight);
			
			Node *Replicate(void) const override;
		
		public:
			
			C4API CubeLight(const ColorRGB& color, float range, const char *name);
			C4API ~CubeLight();
			
			CubeLightObject *GetObject(void) const
			{
				return (static_cast<CubeLightObject *>(Node::GetObject()));
			}
	};
	
	
	//# \class	SpotLight	Represents a spot light node in a world.
	//
	//# The $SpotLight$ class represents a spot light node in a world.
	//
	//# \def	class SpotLight : public PointLight
	//
	//# \ctor	SpotLight(const ColorRGB& color, float range, float apex, const char *name);
	//
	//# \param	color	The color of light emitted by the light source.
	//# \param	range	The spherical range of the light source.
	//# \param	apex	The tangent of half the apex angle for the spot light. This determines the light's angle of illumination.
	//# \param	name	The name of the projected shadow texture map.
	//
	//# \desc
	//#
	//
	//# \base	PointLight	A spot light is a special type of point light.
	//
	//# \also	$@GraphicsMgr/SpotLightObject@$
	
	
	class SpotLight : public PointLight
	{
		friend class Light;
		
		private:
			
			SpotLight();
			SpotLight(const SpotLight& spotLight);
			
			Node *Replicate(void) const override;
			
			void CalculatePostBounding(void) override;
		
		public:
			
			C4API SpotLight(const ColorRGB& color, float range, float apex, const char *name);
			C4API ~SpotLight();
			
			SpotLightObject *GetObject(void) const
			{
				return (static_cast<SpotLightObject *>(Node::GetObject()));
			}
			
			void CalculateBoundaryPolygons(LightRegion *region) const;
	};
}


#endif

// ZYURVUR
