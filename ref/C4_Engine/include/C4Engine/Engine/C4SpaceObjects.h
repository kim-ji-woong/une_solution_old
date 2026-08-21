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


#ifndef C4SpaceObjects_h
#define C4SpaceObjects_h


//# \component	World Manager
//# \prefix		WorldMgr/


#include "C4Objects.h"
#include "C4Volumes.h"
#include "C4Textures.h"


namespace C4
{
	typedef Type	SpaceType;
	typedef Type	FogFunction;
	
	
	//# \enum	SpaceType
	
	enum
	{
		kSpaceFog				= 'FOG ',		//## A fog space.
		kSpaceShadow			= 'SHAD',		//## A shadow space.
		kSpaceAmbient			= 'AMBT',		//## An ambient space.
		kSpaceAcoustics			= 'ACST',		//## An acoustics space.
		kSpaceOcclusion			= 'OCCL',		//## An occlusion space.
		kSpacePaint				= 'PANT'		//## A paint space.
	};
	
	
	//# \enum	FogFlags
	
	enum
	{
		kFogOcclusionInhibit	= 1 << 0		//## Do not use distance occlusion inside the fog volume.
	};
	
	
	//# \enum	FogFunction
	
	enum
	{
		kFogFunctionConstant	= 'CNST',		//## Use constant fog density.
		kFogFunctionLinear		= 'LINR'		//## Use linearly increasing fog density.
	};
	
	
	//# \enum	AmbientSpaceFlags
	
	enum
	{
		kAmbientSpaceGenerator	= 1 << 0		//## This ambient space generates a texture map.
	};
	
	
	class Texture;
	
	
	//# \class	SpaceObject		Encapsulates data pertaining to a space.
	//
	//# The $SpaceObject$ class encapsulates data pertaining to a space.
	//
	//# \def	class SpaceObject : public Object, public VolumeObject
	//
	//# \ctor	SpaceObject(SpaceType type, Volume *volume);
	//
	//# The constructor has protected access. The $SpaceObject$ class can only exist as the base class for another class.
	//
	//# \param	type		The type of the space. See below for a list of possible types.
	//# \param	volume		A pointer to the generic volume object representing the space.
	//
	//# \desc
	//# 
	//# \table	SpaceType
	//
	//# \base		Object			A $SpaceObject$ is an object that can be shared by multiple space nodes.
	//# \privbase	VolumeObject	Used internally by the engine for generic volume objects.
	//
	//# \also	$@Space@$
	
	
	//# \function	SpaceObject::GetSpaceType		Returns the specific type of a space.
	//
	//# \proto	SpaceType GetSpaceType(void) const;
	//
	//# \desc
	//# The $GetSpaceType$ function returns the specific space type, which may be one of the following values.
	//
	//# \table	SpaceType
	
	
	class SpaceObject : public Object, public VolumeObject
	{
		friend class Object;
		
		private:
			
			SpaceType	spaceType;
			
			static SpaceObject *Construct(Unpacker& data, unsigned_int32 unpackFlags); 
		
		protected: 
			 
			SpaceObject(SpaceType type, Volume *volume); 
			~SpaceObject();
		 
		public:
			
			SpaceType GetSpaceType(void) const
			{ 
				return (spaceType);
			}
			
			void PackType(Packer& data) const; 
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetObjectSize(float *size) const;
			void SetObjectSize(const float *size);
	};
	
	
	//# \class	FogSpaceObject		Encapsulates data pertaining to a fog space.
	//
	//# The $FogSpaceObject$ class encapsulates data pertaining to a fog space.
	//
	//# \def	class FogSpaceObject : public SpaceObject, public PlateVolume
	//
	//# \ctor	FogSpaceObject(const Vector2D& size);
	//
	//# \param	size	The size of the plate.
	//
	//# \desc
	// 
	//# \base	SpaceObject		A $FogSpaceObject$ is an object that can be shared by multiple fog space nodes.
	//# \base	PlateVolume		A $FogSpaceObject$ is represented by a generic plate volume.
	//
	//# \also	$@FogSpace@$
	
	
	//# \function	FogSpaceObject::GetFogFlags		Returns the fog flags.
	//
	//# \proto	unsigned_int32 GetFogFlags(void) const;
	//
	//# \desc
	//# The $GetFogFlags$ function returns the fog flags, which can be a combination (through logical OR) of the following values.
	//
	//# \table	FogFlags
	//
	//# \also	$@FogSpaceObject::SetFogFlags@$
	
	
	//# \function	FogSpaceObject::SetFogFlags		Sets the fog flags.
	//
	//# \proto	void SetFogFlags(unsigned_int32 flags);
	//
	//# \param	flags	The new fog flags.
	//
	//# \desc
	//# The $SetFogFlags$ function sets the fog flags to the value specified by the $flags$ parameter,
	//# which can be a combination (through logical OR) of the following values.
	//
	//# \table	FogFlags
	//
	//# \also	$@FogSpaceObject::GetFogFlags@$
	
	
	//# \function	FogSpaceObject::GetFogColor		Returns the fog color.
	//
	//# \proto	const ColorRGBA& GetFogColor(void) const;
	//
	//# \desc
	//# The $GetFogColor$ function returns the fog color. The alpha channel of the color is not used.
	//
	//# \also	$@FogSpaceObject::SetFogColor@$
	
	
	//# \function	FogSpaceObject::SetFogColor		Sets the fog color.
	//
	//# \proto	void SetFogColor(const ColorRGBA& color);
	//
	//# \param	color	The new fog color.
	//
	//# \desc
	//# The $SetFogColor$ function sets the fog color to that specified by the $color$ parameter.
	//# The alpha channel of the color is not used and should be set to 1.0.
	//
	//# \also	$@FogSpaceObject::GetFogColor@$
	
	
	//# \function	FogSpaceObject::GetFogDensity		Returns the fog density.
	//
	//# \proto	float GetFogDensity(void) const;
	//
	//# \desc
	//# The $GetFogDensity$ function returns the fog density. See the $@FogSpaceObject::SetFogFunction@$ function for
	//# a description of how the density is used in calculating fog effects.
	//
	//# \also	$@FogSpaceObject::SetFogDensity@$
	
	
	//# \function	FogSpaceObject::SetFogDensity		Sets the fog density.
	//
	//# \proto	void SetFogDensity(float density);
	//
	//# \param	density		The new fog density.
	//
	//# \desc
	//# The $SetFogDensity$ function sets the fog density to the value specified by the $density$ parameter. See the
	//# $@FogSpaceObject::SetFogFunction@$ function for a description of how the density is used in calculating fog effects.
	//
	//# \also	$@FogSpaceObject::GetFogDensity@$
	
	
	//# \function	FogSpaceObject::GetFogFunction		Returns the fog function.
	//
	//# \proto	FogFunction GetFogFunction(void) const;
	//
	//# \desc
	//# The $GetFogFunction$ function returns the fog function, which can be one of the following values.
	//
	//# \table	FogFunction
	//
	//# See the $@FogSpaceObject::SetFogFunction@$ function for a description of how fog functions are used in calculating fog effects.
	//
	//# \also	$@FogSpaceObject::SetFogFunction@$
	
	
	//# \function	FogSpaceObject::SetFogFunction		Sets the fog function.
	//
	//# \proto	void SetFogFunction(FogFunction function);
	//
	//# \param	function	The new fog function.
	//
	//# \desc
	//# The $SetFogFunction$ function sets the fog function to that specified by the $function$ parameter,
	//# which can be one of the following values.
	//
	//# \table	FogFunction
	//
	//# If the fog function is $kFogFunctionConstant$, then the density of the fog is constant everywhere beneath the fog plane.
	//# (This density is specified using the $@FogSpaceObject::SetFogDensity@$ function.) If the fog function is $kFogFunctionLinear$,
	//# then the fog density is given by <i>&rho;z</i>, where <i>&rho;</i> is the fog density, and <i>z</i> is the distance beneath
	//# the fog plane.
	//
	//# \also	$@FogSpaceObject::GetFogFunction@$
	//# \also	$@FogSpaceObject::SetFogDensity@$
	
	
	class FogSpaceObject : public SpaceObject, public PlateVolume
	{
		friend class SpaceObject;
		
		private:
			
			unsigned_int32		fogFlags;
			ColorRGBA			fogColor;
			float				fogDensity;
			FogFunction			fogFunction;
			
			unsigned_int32		perspectiveExclusionMask;
			
			FogSpaceObject();
			~FogSpaceObject();
		
		public:
			
			FogSpaceObject(const Vector2D& size);
			
			unsigned_int32 GetFogFlags(void) const
			{
				return (fogFlags);
			}
			
			void SetFogFlags(unsigned_int32 flags)
			{
				fogFlags = flags;
			}
			
			const ColorRGBA& GetFogColor(void) const
			{
				return (fogColor);
			}
			
			void SetFogColor(const ColorRGBA& color)
			{
				fogColor = color;
			}
			
			float GetFogDensity(void) const
			{
				return (fogDensity);
			}
			
			void SetFogDensity(float density)
			{
				fogDensity = density;
			}
			
			FogFunction GetFogFunction(void) const
			{
				return (fogFunction);
			}
			
			void SetFogFunction(FogFunction function)
			{
				fogFunction = function;
			}
			
			unsigned_int32 GetPerspectiveExclusionMask(void) const
			{
				return (perspectiveExclusionMask);
			}
			
			void SetPerspectiveExclusionMask(unsigned_int32 mask)
			{
				perspectiveExclusionMask = mask;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			void *BeginSettingsUnpack(void);
			
			int32 GetCategoryCount(void) const;
			Type GetCategoryType(int32 index, const char **title) const;
			int32 GetCategorySettingCount(Type category) const;
			Setting *GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const;
			void SetCategorySetting(Type category, const Setting *setting);
			
			float GetOcclusionValue(void) const;
	};
	
	
	//# \class	ShadowSpaceObject	Encapsulates data pertaining to a shadow space.
	//
	//# The $ShadowSpaceObject$ class encapsulates data pertaining to a shadow space.
	//
	//# \def	class ShadowSpaceObject : public SpaceObject, public BoxVolume
	//
	//# \ctor	ShadowSpaceObject(const Vector3D& size);
	//
	//# \param	size	The size of the box.
	//
	//# \desc
	// 
	//# \base	SpaceObject		A $ShadowSpaceObject$ is an object that can be shared by multiple shadow space nodes.
	//# \base	BoxVolume		A $ShadowSpaceObject$ is represented by a generic box volume.
	//
	//# \also	$@ShadowSpace@$
	
	
	class ShadowSpaceObject : public SpaceObject, public BoxVolume
	{
		friend class SpaceObject;
		
		private:
			
			ShadowSpaceObject();
			~ShadowSpaceObject();
		
		public:
			
			ShadowSpaceObject(const Vector3D& size);
	};
	
	
	//# \class	AmbientSpaceObject		Encapsulates data pertaining to an ambient space.
	//
	//# The $AmbientSpaceObject$ class encapsulates data pertaining to an ambient space.
	//
	//# \def	class AmbientSpaceObject : public SpaceObject, public BoxVolume
	//
	//# \ctor	AmbientSpaceObject(const Vector3D& size, int32 x, int32 y, int32 z, const char *name);
	//
	//# \param	size	The size of the box.
	//# \param	x		The width of the ambient texture.
	//# \param	y		The height of the ambient texture.
	//# \param	z		The depth of the ambient texture.
	//# \param	name	The name of the ambient texture.
	//
	//# \desc
	// 
	//# \base	SpaceObject		An $AmbientSpaceObject$ is an object that can be shared by multiple ambient space nodes.
	//# \base	BoxVolume		An $AmbientSpaceObject$ is represented by a generic box volume.
	//
	//# \also	$@AmbientSpace@$
	
	
	class AmbientSpaceObject : public SpaceObject, public BoxVolume
	{
		friend class SpaceObject;
		
		private:
			
			unsigned_int32	ambientSpaceFlags;
			float			samplingRadius;
			float			occlusionExponent;
			float			minAmbientValue;
			int32			textureSize[3];
			
			Texture			*ambientMap[2];
			ResourceName	ambientName;
			
			AmbientSpaceObject();
			~AmbientSpaceObject();
		
		public:
			
			enum
			{
				kMaxAmbientSpaceSize = 128
			};
			
			AmbientSpaceObject(const Vector3D& size, int32 x, int32 y, int32 z, const char *name);
			
			unsigned_int32 GetAmbientSpaceFlags(void) const
			{
				return (ambientSpaceFlags);
			}
			
			void SetAmbientSpaceFlags(unsigned_int32 flags)
			{
				ambientSpaceFlags = flags;
			}
			
			float GetSamplingRadius(void) const
			{
				return (samplingRadius);
			}
			
			void SetSamplingRadius(float radius)
			{
				samplingRadius = radius;
			}
			
			float GetOcclusionExponent(void) const
			{
				return (occlusionExponent);
			}
			
			void SetOcclusionExponent(float exponent)
			{
				occlusionExponent = exponent;
			}
			
			float GetMinAmbientValue(void) const
			{
				return (minAmbientValue);
			}
			
			void SetMinAmbientValue(float value)
			{
				minAmbientValue = value;
			}
			
			const int32 *GetTextureSize(void) const
			{
				return (textureSize);
			}
			
			void SetTextureSize(int32 x, int32 y, int32 z)
			{
				textureSize[0] = x;
				textureSize[1] = y;
				textureSize[2] = z;
			}
			
			Texture *const& GetAmbientMap(int32 index) const
			{
				return (ambientMap[index]);
			}
			
			const ResourceName& GetAmbientName(void) const
			{
				return (ambientName);
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			void *BeginSettingsUnpack(void);
			
			int32 GetCategoryCount(void) const;
			Type GetCategoryType(int32 index, const char **title) const;
			int32 GetCategorySettingCount(Type category) const;
			Setting *GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const;
			void SetCategorySetting(Type category, const Setting *setting);
			
			C4API void SetAmbientMap(const char *name);
	};
	
	
	//# \class	AcousticsSpaceObject	Encapsulates data pertaining to an acoustic space.
	//
	//# The $ShadowSpaceObject$ class encapsulates data pertaining to an acoustic space.
	//
	//# \def	class AcousticsSpaceObject : public SpaceObject, public BoxVolume
	//
	//# \ctor	AcousticsSpaceObject(const Vector3D& size);
	//
	//# \param	size	The size of the box.
	//
	//# \desc
	// 
	//# \base	SpaceObject		An $AcousticsSpaceObject$ is an object that can be shared by multiple acoustic space nodes.
	//# \base	BoxVolume		An $AcousticsSpaceObject$ is represented by a generic box volume.
	//
	//# \also	$@AcousticsSpace@$
	
	
	class AcousticsSpaceObject : public SpaceObject, public BoxVolume
	{
		friend class SpaceObject;
		
		private:
			
			float		reflectionVolume;
			float		reflectionHFVolume;
			float		reverbDecayTime;
			float		mediumHFAbsorption;
			
			AcousticsSpaceObject();
			~AcousticsSpaceObject();
		
		public:
			
			AcousticsSpaceObject(const Vector3D& size);
			
			float GetReflectionVolume(void) const
			{
				return (reflectionVolume);
			}
			
			void SetReflectionVolume(float volume)
			{
				reflectionVolume = volume;
			}
			
			float GetReflectionHFVolume(void) const
			{
				return (reflectionHFVolume);
			}
			
			void SetReflectionHFVolume(float volume)
			{
				reflectionHFVolume = volume;
			}
			
			float GetReverbDecayTime(void) const
			{
				return (reverbDecayTime);
			}
			
			void SetReverbDecayTime(float time)
			{
				reverbDecayTime = time;
			}
			
			float GetMediumHFAbsorption(void) const
			{
				return (mediumHFAbsorption);
			}
			
			void SetMediumHFAbsorption(float absorption)
			{
				mediumHFAbsorption = absorption;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetCategoryCount(void) const;
			Type GetCategoryType(int32 index, const char **title) const;
			int32 GetCategorySettingCount(Type category) const;
			Setting *GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const;
			void SetCategorySetting(Type category, const Setting *setting);
	};
	
	
	//# \class	OcclusionSpaceObject	Encapsulates data pertaining to an occlusion space.
	//
	//# The $OcclusionSpaceObject$ class encapsulates data pertaining to an occlusion space.
	//
	//# \def	class OcclusionSpaceObject : public SpaceObject, public BoxVolume
	//
	//# \ctor	OcclusionSpaceObject(const Vector3D& size);
	//
	//# \param	size	The size of the box.
	//
	//# \desc
	// 
	//# \base	SpaceObject		An $OcclusionSpaceObject$ is an object that can be shared by multiple occlusion space nodes.
	//# \base	BoxVolume		An $OcclusionSpaceObject$ is represented by a generic box volume.
	//
	//# \also	$@OcclusionSpace@$
	
	
	class OcclusionSpaceObject : public SpaceObject, public BoxVolume
	{
		friend class SpaceObject;
		
		private:
			
			OcclusionSpaceObject();
			~OcclusionSpaceObject();
		
		public:
			
			OcclusionSpaceObject(const Vector3D& size);
	};
	
	
	//# \class	PaintSpaceObject	Encapsulates data pertaining to a paint space.
	//
	//# The $PaintSpaceObject$ class encapsulates data pertaining to a paint space.
	//
	//# \def	class PaintSpaceObject : public SpaceObject, public BoxVolume
	//
	//# \ctor	PaintSpaceObject(const Vector3D& size, const Integer2D& resolution);
	//
	//# \param	size		The size of the box.
	//# \param	resolution	The resolution of the paint texture. This must be a power of two between the values of $kPaintMinResolution$ and $kPaintMaxResolution$, inclusive.
	//
	//# \desc
	// 
	//# \base	SpaceObject		A $PaintSpaceObject$ is an object that can be shared by multiple paint space nodes.
	//# \base	BoxVolume		A $PaintSpaceObject$ is represented by a generic box volume.
	//
	//# \also	$@PaintSpace@$
	
	
	class PaintSpaceObject : public SpaceObject, public BoxVolume
	{
		friend class SpaceObject;
		
		private:
			
			struct PaintImageDesc
			{
				Integer2D		paintResolution;
				int32			channelCount;
			};
			
			PaintImageDesc		imageDesc;
			
			unsigned_int8		*paintImage;
			Texture				*paintTexture;
			TextureHeader		textureHeader;
			
			int32				preprocessCount;
			
			PaintSpaceObject();
			~PaintSpaceObject();
			
			void CreatePaintTexture(void);
		
		public:
			
			PaintSpaceObject(const Vector3D& size, const Integer2D& resolution, int32 count);
			
			const Integer2D& GetPaintResolution(void) const
			{
				return (imageDesc.paintResolution);
			}
			
			void SetPaintResolution(const Integer2D& resolution)
			{
				imageDesc.paintResolution = resolution;
			}
			
			int32 GetChannelCount(void) const
			{
				return (imageDesc.channelCount);
			}
			
			void SetChannelCount(int32 count)
			{
				imageDesc.channelCount = count;
			}
			
			void *GetPaintImage(void) const
			{
				return (paintImage);
			}
			
			const Texture *const *GetPaintTexturePointer(void) const
			{
				return (&paintTexture);
			}
			
			Texture *GetPaintTexture(void) const
			{
				return (paintTexture);
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			void *BeginSettingsUnpack(void);
			
			int32 GetCategoryCount(void) const;
			Type GetCategoryType(int32 index, const char **title) const;
			int32 GetCategorySettingCount(Type category) const;
			Setting *GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const;
			void SetCategorySetting(Type category, const Setting *setting);
			
			void *BeginSettings(void);
			void EndSettings(void *cookie);
			
			void Preprocess(void);
			void Neutralize(void);
	};
}


#endif

// ZYURVUR
