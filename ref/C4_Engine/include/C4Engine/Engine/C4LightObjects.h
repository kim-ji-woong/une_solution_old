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


#ifndef C4LightObjects_h
#define C4LightObjects_h


//# \component	Graphics Manager
//# \prefix		GraphicsMgr/


#include "C4Objects.h"
#include "C4Bounding.h"
#include "C4Textures.h"


namespace C4
{
	typedef Type	LightType;
	
	
	//# \enum	LightType
	
	enum
	{
		kLightInfinite				= 'INFT',	//## An infinite light.
		kLightDepth					= 'DPTH',	//## An infinite light with a shadow map.
		kLightLandscape				= 'LAND',	//## An infinite light with a large-scale multi-resolution shadow map.
		kLightPoint					= 'PONT',	//## A point light.
		kLightCube					= 'CUBE',	//## A point light with a projected cube shadow texture.
		kLightSpot					= 'SPOT'	//## A spot light with a projected 2D shadow texture.
	};
	
	
	//# \enum	LightFlags
	
	enum
	{
		kLightStatic				= 1 << 0,	//## Light is static (does not move).
		kLightShadowInhibit			= 1 << 1,	//## Light does not cast shadows.
		kLightGenerator				= 1 << 2,	//## Light generates a projected shadow.
		kLightExternalZone			= 1 << 5,	//## The light's position may temporarily be outside its owning zone.
		kLightConfined				= 1 << 6,	//## The light is confined to a small sphere (point light only).
		kLightInstanceShadowSpace	= 1 << 7	//## The light should use the shadow space connected by an instance super node.
	};
	
	
	enum
	{
		kMaxShadowSectionCount	= 4
	};
	
	const float kInverseMaxShadowSectionCount = 1.0F / (float) kMaxShadowSectionCount;
	
	
	class Texture;
	
	
	struct LightShadowData
	{
		Vector3D		shadowSize;
		Vector3D		inverseShadowSize;
		Point3D			shadowPosition;
		
		float			texelSize;
		
		Antivector4D	nearPlane;
		Antivector4D	farPlane;
		Antivector4D	sectionPlane;
		
		Point3D			sectionPolygon[4];
	};
	
	
	//# \class	LightObject		Encapsulates data pertaining to a light source.
	//
	//# The $LightObject$ class encapsulates data pertaining to a light source.
	//
	//# \def	class LightObject : public Object
	//
	//# \ctor	LightObject(LightType type, LightType base, const ColorRGB& color);
	//
	//# The constructor has protected access. The $LightObject$ class can only exist as the base class for another class.
	//
	//# \param	type	The type of the light source. See below for a list of possible types.
	//# \param	base	The base type of the light source. This should be $kLightInfinite$ or $kLightPoint$.
	//# \param	color	The color of light emitted by the light source.
	//
	//# \desc
	//# 
	//# \table	LightType
	//
	//# \base	WorldMgr/Object		A $LightObject$ is an object that can be shared by multiple light nodes.
	//
	//# \also	$@WorldMgr/Light@$
	
	
	//# \function	LightObject::GetLightType		Returns the specific type of a light.
	//
	//# \proto	LightType GetLightType(void) const;
	//
	//# \desc
	//# The $GetLightType$ function returns the specific light type, which may be one of the following values.
	//
	//# \table	LightType 
	//
	//# All of the light types are divided into two categories, and the general category that a light object 
	//# falls into can be determined by calling the $@LightObject::GetBaseLightType@$ function. 
	// 
	//# \also	$@LightObject::GetBaseLightType@$
	 
	
	//# \function	LightObject::GetBaseLightType		Returns the base type of a light.
	//
	//# \proto	LightType GetBaseLightType(void) const; 
	//
	//# \desc
	//# All of the light types are divided into two categories, lights that are directional and have infinite
	//# range and lights that are localized and have finite range. The $GetBaseLightType$ function returns the base 
	//# light type, which can only be $kLightInfinite$ or $kLightPoint$. This represents which general category
	//# the light object falls into. The more specific type of light can be determined by calling the
	//# $@LightObject::GetLightType@$ function.
	//# 
	//# The $@InfiniteLightObject@$ and $@DepthLightObject@$ classes have the $kLightInfinite$ base type.
	//# The $@PointLightObject@$, $@CubeLightObject@$, and $@SpotLightObject@$ classes have the $kLightPoint$ base type.
	//
	//# \also	$@LightObject::GetLightType@$
	
	
	//# \function	LightObject::GetLightColor		Returns the light color.
	//
	//# \proto	const ColorRGB& GetLightColor(void) const;
	//
	//# \desc
	//# The $GetLightColor$ function returns the light color.
	//
	//# \also	$@Math/ColorRGB@$
	//# \also	$@LightObject::SetLightColor@$
	
	
	//# \function	LightObject::SetLightColor		Sets the light color.
	//
	//# \proto	void SetLightColor(const ColorRGB& color);
	//
	//# \param	color	The new light color.
	//
	//# \desc
	//# The $SetLightColor$ function sets the light color. The light color may be changed at any time,
	//# and doing so has immediate effect.
	//# 
	//# To turn a light off, the $kNodeDisabled$ flag should be set for the $@WorldMgr/Light@$ node by
	//# calling the $@WorldMgr/Node::SetNodeFlags@$ function. This results in much better performance than
	//# setting the light's color to black.
	//
	//# \also	$@Math/ColorRGB@$
	//# \also	$@LightObject::GetLightColor@$
	
	
	//# \function	LightObject::GetLightFlags		Returns the light flags.
	//
	//# \proto	unsigned_int32 GetLightFlags(void) const;
	//
	//# \desc
	//# The $GetLightFlags$ function returns the light flags, which can be a combination (through logical OR) of the following values.
	//
	//# \table	LightFlags
	//
	//# \also	$@LightObject::SetLightFlags@$
	
	
	//# \function	LightObject::SetLightFlags		Sets the light flags.
	//
	//# \proto	void SetLightFlags(unsigned_int32 flags);
	//
	//# \param	flags	The new light flags.
	//
	//# \desc
	//# The $SetLightFlags$ function sets the light flags to the value specified by the $flags$ parameter,
	//# which can be a combination (through logical OR) of the following values.
	//
	//# \table	LightFlags
	//
	//# The $kLightGenerator$ flag only pertains to depth lights, cube lights, and spot lights. It indicates
	//# that a projected texture should be generated for a light during the development process.
	//# 
	//# The $kLightExternalZone$ flag should be set when the light source is a subnode of another node that
	//# may be in a different zone. When this flag is set, the light node will independently determine which
	//# zone it's in instead of using the owning zone in the node hierarchy.
	//
	//# \also	$@LightObject::GetLightFlags@$
	
	
	class LightObject : public Object
	{
		friend class Object;
		
		private:
			
			LightType			lightType;
			LightType			baseLightType;
			
			ColorRGB			lightColor;
			unsigned_int32		lightFlags;
			int32				minDetailLevel;
			
			static LightObject *Construct(Unpacker& data, unsigned_int32 unpackFlags);
		
		protected:
			
			LightObject(LightType type, LightType base);
			LightObject(LightType type, LightType base, const ColorRGB& color);
			LightObject(const LightObject& lightObject);
			virtual ~LightObject();
		
		public:
			
			LightType GetLightType(void) const
			{
				return (lightType);
			}
			
			LightType GetBaseLightType(void) const
			{
				return (baseLightType);
			}
			
			const ColorRGB& GetLightColor(void) const
			{
				return (lightColor);
			}
			
			void SetLightColor(const ColorRGB& color)
			{
				lightColor = color;
			}
			
			unsigned_int32 GetLightFlags(void) const
			{
				return (lightFlags);
			}
			
			void SetLightFlags(unsigned_int32 flags)
			{
				lightFlags = flags;
			}
			
			int32 GetMinDetailLevel(void) const
			{
				return (minDetailLevel);
			}
			
			void SetMinDetailLevel(int32 level)
			{
				minDetailLevel = level;
			}
			
			void PackType(Packer& data) const;
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetCategoryCount(void) const;
			Type GetCategoryType(int32 index, const char **title) const;
			int32 GetCategorySettingCount(Type category) const;
			Setting *GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const;
			void SetCategorySetting(Type category, const Setting *setting);
	};
	
	
	//# \class	InfiniteLightObject		Encapsulates data pertaining to an infinite light source.
	//
	//# The $InfiniteLightObject$ class encapsulates data pertaining to an infinite light source.
	//
	//# \def	class InfiniteLightObject : public LightObject
	//
	//# \ctor	InfiniteLightObject(const ColorRGB& color);
	//
	//# \param	color	The color of light emitted by the light source.
	//
	//# \desc
	//
	//# \base	LightObject		An $InfiniteLightObject$ is a specific type of $LightObject$.
	//
	//# \also	$@WorldMgr/InfiniteLight@$
	
	
	class InfiniteLightObject : public LightObject
	{
		friend class LightObject;
		
		private:
			
			InfiniteLightObject();
			
			Object *Replicate(void) const override;
		
		protected:
			
			InfiniteLightObject(LightType type);
			InfiniteLightObject(LightType type, const ColorRGB& color);
			InfiniteLightObject(const InfiniteLightObject& infiniteLightObject);
			~InfiniteLightObject();
		
		public:
			
			InfiniteLightObject(const ColorRGB& color);
	};
	
	
	//# \class	DepthLightObject		Encapsulates data pertaining to an infinite light source having a depth-based shadow map.
	//
	//# The $DepthLightObject$ class encapsulates data pertaining to an infinite light source having a depth-based shadow map.
	//
	//# \def	class DepthLightObject : public InfiniteLightObject
	//
	//# \ctor	DepthLightObject(const ColorRGB& color);
	//
	//# \param	color	The color of light emitted by the light source.
	//
	//# \desc
	//
	//# \base	InfiniteLightObject		A $DepthLightObject$ is a specific type of $InfiniteLightObject$.
	//
	//# \also	$@WorldMgr/DepthLight@$
	
	
	class DepthLightObject : public InfiniteLightObject
	{
		friend class LightObject;
		
		private:
			
			DepthLightObject();
			
			Object *Replicate(void) const override;
		
		protected:
			
			DepthLightObject(LightType type);
			DepthLightObject(LightType type, const ColorRGB& color);
			DepthLightObject(const DepthLightObject& depthLightObject);
			~DepthLightObject();
		
		public:
			
			DepthLightObject(const ColorRGB& color);
	};
	
	
	//# \class	LandscapeLightObject		Encapsulates data pertaining to an infinite light source having a large-scale multi-resolution shadow map.
	//
	//# The $LandscapeLightObject$ class encapsulates data pertaining to an infinite light source having a large-scale multi-resolution shadow map.
	//
	//# \def	class LandscapeLightObject : public InfiniteLightObject
	//
	//# \ctor	LandscapeLightObject(const ColorRGB& color);
	//
	//# \param	color	The color of light emitted by the light source.
	//
	//# \desc
	//
	//# \base	DepthLightObject		A $LandscapeLightObject$ is a specific type of $DepthLightObject$.
	//
	//# \also	$@WorldMgr/LandscapeLight@$
	
	
	class LandscapeLightObject : public DepthLightObject
	{
		friend class LightObject;
		
		private:
			
			Range<float>		sectionRange[kMaxShadowSectionCount];
			
			LandscapeLightObject();
			LandscapeLightObject(const LandscapeLightObject& landscapeLightObject);
			~LandscapeLightObject();
			
			Object *Replicate(void) const override;
		
		public:
			
			LandscapeLightObject(const ColorRGB& color);
			
			Range<float> *GetSectionRangeArray(void)
			{
				return (sectionRange);
			}
			
			const Range<float> *GetSectionRangeArray(void) const
			{
				return (sectionRange);
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetCategorySettingCount(Type category) const;
			Setting *GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const;
			void SetCategorySetting(Type category, const Setting *setting);
	};
	
	
	//# \class	PointLightObject		Encapsulates data pertaining to a point light source.
	//
	//# The $PointLightObject$ class encapsulates data pertaining to a point light source.
	//
	//# \def	class PointLightObject : public LightObject
	//
	//# \ctor	PointLightObject(const ColorRGB& color, float range);
	//
	//# \param	color	The color of light emitted by the light source.
	//# \param	range	The spherical range of the light source.
	//
	//# \desc
	//
	//# \base	LightObject		A $PointLightObject$ is a specific type of $LightObject$.
	//
	//# \also	$@WorldMgr/PointLight@$
	
	
	//# \function	PointLightObject::GetLightRange		Returns the spherical range of the light source.
	//
	//# \proto	float GetLightRange(void) const;
	//
	//# \desc
	//# The $GetLightRange$ function returns the range of the point light source. The light attenuation
	//# is zero at this distance from the light source, and no illumination is applied outside this range.
	//
	//# \also	$@PointLightObject::SetLightRange@$
	
	
	//# \function	PointLightObject::SetLightRange		Sets the spherical range of the light source.
	//
	//# \proto	void SetLightRange(float range);
	//
	//# \param	range	The spherical range of the light source.
	//
	//# \desc
	//# The $SetLightRange$ function sets the range of the point light source. The light attenuation
	//# is zero at this distance from the light source, and no illumination is applied outside this range.
	//
	//# \also	$@PointLightObject::GetLightRange@$
	
	
	class PointLightObject : public LightObject
	{
		friend class LightObject;
		
		private:
			
			float		lightRange;
			float		confinementRadius;
			
			PointLightObject();
			
			Object *Replicate(void) const override;
		
		protected:
			
			PointLightObject(LightType type);
			PointLightObject(LightType type, const ColorRGB& color, float range);
			PointLightObject(const PointLightObject& pointLightObject);
			~PointLightObject();
		
		public:
			
			PointLightObject(const ColorRGB& color, float range);
			
			float GetLightRange(void) const
			{
				return (lightRange);
			}
			
			void SetLightRange(float range)
			{
				lightRange = range;
			}
			
			float GetConfinementRadius(void) const
			{
				return (confinementRadius);
			}
			
			void SetConfinementRadius(float radius)
			{
				confinementRadius = radius;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetObjectSize(float *size) const;
			void SetObjectSize(const float *size);
	};
	
	
	//# \class	ShadowLightObject		Encapsulates data pertaining to a point light source having a projected shadow texture map.
	//
	//# The $PointLightObject$ class encapsulates data pertaining to a point light source having a projected shadow texture map.
	//
	//# \def	class ShadowLightObject : public PointLightObject
	//
	//# \ctor	ShadowLightObject(LightType type, const ColorRGB& color, float range, const char *name);
	//
	//# The constructor has protected access. The $ShadowLightObject$ class can only exist as the base class for another class.
	//
	//# \param	type	The type of light source. This must be either $kLightCube$ or $kLightSpot$.
	//# \param	color	The color of light emitted by the light source.
	//# \param	range	The spherical range of the light source.
	//# \param	name	The name of the projected shadow texture map.
	//
	//# \desc
	//
	//# \base	PointLightObject		A $ShadowLightObject$ is a specific type of $PointLightObject$.
	//
	//# \also	$@CubeLightObject@$
	//# \also	$@SpotLightObject@$
	
	
	//# \function	ShadowLightObject::GetTextureSize		Returns the size of the projected shadow texture map.
	//
	//# \proto	int32 GetTextureSize(void) const;
	//
	//# \desc
	//
	//# \also	$@ShadowLightObject::SetTextureSize@$
	//# \also	$@ShadowLightObject::GetShadowMap@$
	//# \also	$@ShadowLightObject::GetShadowName@$
	
	
	//# \function	ShadowLightObject::SetTextureSize		Sets the size of the projected shadow texture map.
	//
	//# \proto	void SetTextureSize(int32 size);
	//
	//# \param	size	The new texture size.
	//
	//# \desc
	//
	//# \also	$@ShadowLightObject::GetTextureSize@$
	
	
	//# \function	ShadowLightObject::GetShadowMap		Returns a pointer to the $@Texture@$ object for the projected shadow.
	//
	//# \proto	Texture *const& GetShadowMap(void) const;
	//
	//# \desc
	//
	//# \also	$@ShadowLightObject::GetShadowName@$
	//# \also	$@ShadowLightObject::GetTextureSize@$
	
	
	//# \function	ShadowLightObject::GetShadowName		Returns the name of the projected shadow texture map.
	//
	//# \proto	const ResourceName& GetShadowName(void) const;
	//
	//# \desc
	//
	//# \also	$@ShadowLightObject::GetShadowMap@$
	//# \also	$@ShadowLightObject::GetTextureSize@$
	
	
	class ShadowLightObject : public PointLightObject
	{
		private:
			
			Texture				*shadowMap;
			
			int32				textureSize;
			TextureFormat		textureFormat;
			ResourceName		shadowName;
		
		protected:
			
			ShadowLightObject(LightType type);
			ShadowLightObject(LightType type, const ColorRGB& color, float range, const char *name);
			ShadowLightObject(const ShadowLightObject& shadowLightObject);
			~ShadowLightObject();
		
		public:
			
			Texture *const& GetShadowMap(void) const
			{
				return (shadowMap);
			}
			
			int32 GetTextureSize(void) const
			{
				return (textureSize);
			}
			
			void SetTextureSize(int32 size)
			{
				textureSize = size;
			}
			
			TextureFormat GetTextureFormat(void) const
			{
				return (textureFormat);
			}
			
			void SetTextureFormat(TextureFormat format)
			{
				textureFormat = format;
			}
			
			const ResourceName& GetShadowName(void) const
			{
				return (shadowName);
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			void *BeginSettingsUnpack(void);
			
			int32 GetCategorySettingCount(Type category) const;
			Setting *GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const;
			void SetCategorySetting(Type category, const Setting *setting);
			
			virtual void SetShadowMap(const char *name);
	};
	
	
	//# \class	CubeLightObject		Encapsulates data pertaining to a point light source having a projected cube texture map.
	//
	//# The $CubeLightObject$ class encapsulates data pertaining to a point light source having a projected cube texture map.
	//
	//# \def	class CubeLightObject : public ShadowLightObject
	//
	//# \ctor	CubeLightObject(const ColorRGB& color, float range, const char *name);
	//
	//# \param	color	The color of light emitted by the light source.
	//# \param	range	The spherical range of the light source.
	//# \param	name	The name of the projected shadow texture map.
	//
	//# \desc
	//
	//# \base	ShadowLightObject		A $CubeLightObject$ is a specific type of $ShadowLightObject$.
	//
	//# \also	$@WorldMgr/CubeLight@$
	
	
	class CubeLightObject : public ShadowLightObject
	{
		friend class LightObject;
		
		private:
			
			CubeLightObject();
			CubeLightObject(const CubeLightObject& cubeLightObject);
			~CubeLightObject();
			
			Object *Replicate(void) const override;
		
		public:
			
			CubeLightObject(const ColorRGB& color, float range, const char *name);
	};
	
	
	//# \class	SpotLightObject		Encapsulates data pertaining to a spot light source having a projected 2D texture map.
	//
	//# The $SpotLightObject$ class encapsulates data pertaining to a spot light source having a projected 2D texture map.
	//
	//# \def	class SpotLightObject : public ShadowLightObject
	//
	//# \ctor	SpotLightObject(const ColorRGB& color, float range, float apex, const char *name);
	//
	//# \param	color	The color of light emitted by the light source.
	//# \param	range	The spherical range of the light source.
	//# \param	apex	The tangent of half the apex angle for the spot light. This determines the light's angle of illumination.
	//# \param	name	The name of the projected shadow texture map.
	//
	//# \desc
	//
	//# \base	ShadowLightObject		A $SpotLightObject$ is a specific type of $ShadowLightObject$.
	//
	//# \also	$@WorldMgr/SpotLight@$
	
	
	//# \function	SpotLightObject::GetApexTangent		Returns the tangent of half the apex angle for a spot light.
	//
	//# \proto	float GetApexTangent(void) const;
	//
	//# \desc
	//
	//# \also	$@SpotLightObject::SetApexTangent@$
	
	
	//# \function	SpotLightObject::SetApexTangent		Sets the tangent of half the apex angle for a spot light.
	//
	//# \proto	void SetApexTangent(float apex);
	//
	//# \param	apex	The tangent of half the apex angle for the spot light.
	//
	//# \desc
	//
	//# \also	$@SpotLightObject::GetApexTangent@$
	
	
	class SpotLightObject : public ShadowLightObject
	{
		friend class LightObject;
		
		private:
			
			float		apexTangent;
			float		aspectRatio;
			
			SpotLightObject();
			SpotLightObject(const SpotLightObject& spotLightObject);
			~SpotLightObject();
			
			Object *Replicate(void) const override;
			
			void CalculateAspectRatio(void);
		
		public:
			
			SpotLightObject(const ColorRGB& color, float range, float apex, const char *name);
			
			float GetApexTangent(void) const
			{
				return (apexTangent);
			}
			
			void SetApexTangent(float apex)
			{
				apexTangent = apex;
			}
			
			float GetAspectRatio(void) const
			{
				return (aspectRatio);
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetObjectSize(float *size) const;
			void SetObjectSize(const float *size);
			
			void SetShadowMap(const char *name);
	};
}


#endif

// ZYURVUR
