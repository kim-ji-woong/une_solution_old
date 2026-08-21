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


#ifndef C4Attributes_h
#define C4Attributes_h


//# \component	Graphics Manager
//# \prefix		GraphicsMgr/


#include "C4Textures.h"


namespace C4
{
	//# \tree	Attribute
	//
	//# \node	DiffuseAttribute
	//# \node	SpecularAttribute
	//# \node	EmissionAttribute
	//# \node	EnvironmentAttribute
	//# \node	ReflectionAttribute
	//# \node	RefractionAttribute
	//# \node	MicrofacetAttribute
	//# \node	MapAttribute
	//# \sub
	//#		\node	TextureMapAttribute
	//#		\node	NormalMapAttribute
	//#		\node	HorizonMapAttribute
	//#		\node	GlossMapAttribute
	//#		\node	EmissionMapAttribute
	//#		\node	OpacityMapAttribute
	//#		\node	EnvironmentMapAttribute
	//# \end
	//# \node	ShaderAttribute
	
	
	typedef Type	AttributeType;
	
	
	//# \enum	AttributeType
	
	enum
	{
		kAttributeReference			= 'REFR',
		kAttributeDiffuse			= 'DIFF',		//## Diffuse reflection color.
		kAttributeSpecular			= 'SPEC',		//## Specular reflection color and power.
		kAttributeEmission			= 'EMIS',		//## Constant emission color.
		kAttributeEnvironment		= 'ENVR',		//## Environment map reflection color.
		kAttributeReflection		= 'RFLC',		//## Reflection buffer parameters.
		kAttributeRefraction		= 'RFRC',		//## Refraction buffer parameters.
		kAttributeMicrofacet		= 'MFCT',		//## Microfacet reflection parameters.
		kAttributeTextureMap		= 'TEXT',		//## Diffuse texture map.
		kAttributeNormalMap			= 'BUMP',		//## Bump (normal) map.
		kAttributeHorizonMap		= 'HRZN',		//## Horizon map.
		kAttributeGlossMap			= 'GLOS',		//## Specular gloss map.
		kAttributeEmissionMap		= 'EMAP',		//## Emission map.
		kAttributeOpacityMap		= 'OPAC',		//## Opacity map.
		kAttributeEnvironmentMap	= 'ENVM',		//## Environment map override.
		kAttributeDeltaDepth		= 'DLTA',
		kAttributePaint				= 'PANT',
		kAttributeFire				= 'FIRE',
		kAttributeShader			= 'SHDR'		//## Shader graph.
	};
	
	
	//# \enum	AttributeFlags
	
	enum
	{
		kAttributeMutable				= 1 << 0	//## The constant data in the attribute, such as a color, is mutable. This means that the data is loaded as a shader parameter instead of being inlined as a literal constant.
	};
	
	
	//# \enum	HorizonFlags
	
	enum
	{
		kHorizonExcludeInfiniteLight	= 1 << 0,	//## Do not render horizon map for lights with the infinite base type.
		kHorizonExcludePointLight		= 1 << 1	//## Do not render horizon map for lights with the point base type.
	};
	
	
	//# \class	Attribute	The base class for all material attributes.
	//
	//# The $Attribute$ class is the base class for all material attributes.
	//
	//# \def	class Attribute : public ListElement<Attribute>, public Packable
	//
	//# \ctor	Attribute(AttributeType type);
	//
	//# The constructor has protected access. The $Attribute$ class can only exist as the base class for another class.
	//
	//# \desc
	//# The $Attribute$ class is the base class for all material attributes. A list of material attributes is supplied to
	//# the $@RenderSegment@$ class by either attaching a material with the $@RenderSegment::SetMaterialObjectPointer@$ function
	//# or by setting an auxiliary attribute list using the $@RenderSegment::SetMaterialAttributeList@$ function. These
	//# material attributes describe to the Graphics Manager how a renderable object should be shaded.
	//# 
	//# A $@WorldMgr/Geometry@$ node can have a $@MaterialObject@$ object attached to it that holds a list of
	//# material attributes. In this case, the list of material attributes is automatically applied to the $@Renderable@$
	//# base class of the $@WorldMgr/Geometry@$ node when it is preprocessed.
	//
	//# \base	Utilities/ListElement<Attribute>	Attributes are stored in a list by the $@MaterialObject@$ class. 
	//#												The $@Renderable@$ class also uses lists of attributes.
	//# \base	ResourceMgr/Packable				Attributes can be packed for storage in resources. 
	// 
	//# \also	$@Renderable@$ 
	//# \also	$@MaterialObject@$
	//# \also	$@WorldMgr/Geometry@$ 
	
	
	//# \function	Attribute::GetAttributeType		Returns the type of an attribute.
	// 
	//# \proto	AttributeType GetAttributeType(void) const;
	//
	//# \desc
	//# The $GetAttributeType$ function returns the type of an attribute, which can be one of the following values. 
	//
	//# \table	AttributeType
	
	
	class Attribute : public ListElement<Attribute>, public Packable
	{
		friend class MaterialObject;
		
		private:
			
			AttributeType	attributeType;
			unsigned_int32	attributeFlags;
			
			virtual Attribute *Replicate(void) const = 0;
			
			static Attribute *Construct(Unpacker& data, unsigned_int32 unpackFlags);
		
		protected:
			
			Attribute(AttributeType type, unsigned_int32 flags = 0);
			Attribute(const Attribute& attribute);
		
		public:
			
			virtual ~Attribute();
			
			AttributeType GetAttributeType(void) const
			{
				return (attributeType);
			}
			
			unsigned_int32 GetAttributeFlags(void) const
			{
				return (attributeFlags);
			}
			
			void SetAttributeFlags(unsigned_int32 flags)
			{
				attributeFlags = flags;
			}
			
			Attribute *Clone(void) const
			{
				return (Replicate());
			}
			
			void PackType(Packer& data) const;
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			virtual bool operator ==(const Attribute& attribute) const;
			
			virtual void SetAttributeColor(const ColorRGBA& color);
	};
	
	
	class ReferenceAttribute : public Attribute
	{
		private:
			
			const Attribute		*attributeReference;
			
			ReferenceAttribute(const ReferenceAttribute& referenceAttribute);
			
			Attribute *Replicate(void) const override;
		
		public:
			
			C4API ReferenceAttribute();
			C4API explicit ReferenceAttribute(const Attribute *attribute);
			C4API ~ReferenceAttribute();
			
			const Attribute *GetReference(void) const
			{
				return (attributeReference);
			}
			
			void SetReference(const Attribute *attribute)
			{
				attributeReference = attribute;
			}
			
			bool operator ==(const Attribute& attribute) const;
	};
	
	
	//# \class	DiffuseAttribute		Material attribute for a diffuse color.
	//
	//# The $DiffuseAttribute$ class represents the material attribute for a diffuse color.
	//
	//# \def	class DiffuseAttribute : public Attribute
	//
	//# \ctor	explicit DiffuseAttribute(const ColorRGBA& color);
	//
	//# \param	color	The initial diffuse color.
	//
	//# \desc
	//# The $DiffuseAttribute$ class represents the material attribute for a diffuse color.
	//# The diffuse color is applied during both the ambient rendering pass and each lighting pass.
	//
	//# \base	Attribute	A $DiffuseAttribute$ is a specific type of $Attribute$.
	//
	//# \also	$@Math/ColorRGBA@$
	
	
	//# \function	DiffuseAttribute::GetDiffuseColor		Returns the diffuse color.
	//
	//# \proto	const ColorRGBA& GetDiffuseColor(void) const;
	//
	//# \desc
	//# The $GetDiffuseColor$ function returns the diffuse color stored in a diffuse material attribute.
	//
	//# \also	$@DiffuseAttribute::SetDiffuseColor@$
	//# \also	$@DiffuseAttribute::SetDiffuseAlpha@$
	
	
	//# \function	DiffuseAttribute::SetDiffuseColor		Sets the diffuse color.
	//
	//# \proto	void SetDiffuseColor(const ColorRGBA& color);
	//
	//# \param	color	The new diffuse color.
	//
	//# \desc
	//# The $SetDiffuseColor$ attribute sets the diffuse color stored in a diffuse material attribute to the value given by the $color$ parameter.
	//# A renderable object to which the affected attribute applies will subsequently be rendered using the new color.
	//
	//# \also	$@DiffuseAttribute::SetDiffuseAlpha@$
	//# \also	$@DiffuseAttribute::GetDiffuseColor@$
	
	
	//# \function	DiffuseAttribute::SetDiffuseAlpha		Sets the alpha component of the diffuse color.
	//
	//# \proto	void SetDiffuseAlpha(float alpha);
	//
	//# \param	alpha	The new diffuse alpha component.
	//
	//# \desc
	//# The $SetDiffuseAlpha$ attribute sets the alpha component of the diffuse color stored in a diffuse material attribute to the value given by the $alpha$ parameter.
	//# The red, green, and blue components are not changed.
	//# A renderable object to which the affected attribute applies will subsequently be rendered using the new color.
	//
	//# \also	$@DiffuseAttribute::SetDiffuseColor@$
	//# \also	$@DiffuseAttribute::GetDiffuseColor@$
	
	
	class DiffuseAttribute : public Attribute
	{
		private:
			
			ColorRGBA		diffuseColor;
			
			Attribute *Replicate(void) const override;
		
		public:
			
			C4API explicit DiffuseAttribute(unsigned_int32 flags = 0);
			C4API explicit DiffuseAttribute(const ColorRGBA& color, unsigned_int32 flags = 0);
			C4API DiffuseAttribute(const DiffuseAttribute& diffuseAttribute);
			C4API ~DiffuseAttribute();
			
			const ColorRGBA& GetDiffuseColor(void) const
			{
				return (diffuseColor);
			}
			
			void SetDiffuseColor(const ColorRGBA& color)
			{
				diffuseColor = color;
			}
			
			void SetDiffuseAlpha(float alpha)
			{
				diffuseColor.alpha = alpha;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			bool operator ==(const Attribute& attribute) const;
			
			void SetAttributeColor(const ColorRGBA& color);
	};
	
	
	//# \class	SpecularAttribute		Material attribute for a specular color.
	//
	//# The $SpecularAttribute$ class represents the material attribute for a specular color.
	//
	//# \def	class SpecularAttribute : public Attribute
	//
	//# \ctor	SpecularAttribute(const ColorRGBA& color, float exponent);
	//
	//# \param	color		The initial specular color.
	//# \param	exponent	The initial specular exponent.
	//
	//# \desc
	//# The $SpecularAttribute$ class represents the material attribute for a specular color.
	//# The specular color each lighting pass, but does not contribute to the ambient pass.
	//# The presence of a specular material attribute determines whether a specular term is
	//# calculated when an object is rendered.
	//
	//# \base	Attribute	A $SpecularAttribute$ is a specific type of $Attribute$.
	//
	//# \also	$@Math/ColorRGBA@$
	
	
	//# \function	SpecularAttribute::GetSpecularColor		Returns the specular color.
	//
	//# \proto	const ColorRGBA& GetSpecularColor(void) const;
	//
	//# \desc
	//# The $GetSpecularColor$ function returns the specular color stored in a specular material attribute.
	//
	//# \also	$@SpecularAttribute::SetSpecularColor@$
	//# \also	$@SpecularAttribute::GetSpecularExponent@$
	//# \also	$@SpecularAttribute::SetSpecularExponent@$
	
	
	//# \function	SpecularAttribute::SetSpecularColor		Sets the specular color.
	//
	//# \proto	void SetSpecularColor(const ColorRGBA& color);
	//
	//# \param	color	The new specular color. The alpha component is ignored.
	//
	//# \desc
	//# The $SetSpecularColor$ attribute sets the specular color stored in a specular material attribute to the value given by the $color$ parameter.
	//# A renderable object to which the affected attribute applies will subsequently be rendered using the new color.
	//# The alpha component of the specular color does not participate in shading calculations.
	//
	//# \also	$@SpecularAttribute::GetSpecularColor@$
	//# \also	$@SpecularAttribute::GetSpecularExponent@$
	//# \also	$@SpecularAttribute::SetSpecularExponent@$
	
	
	//# \function	SpecularAttribute::GetSpecularExponent		Returns the specular exponent.
	//
	//# \proto	const float& GetSpecularExponent(void) const;
	//
	//# \desc
	//# The $GetSpecularExponent$ function returns the specular exponent stored in a specular material attribute.
	//
	//# \also	$@SpecularAttribute::SetSpecularExponent@$
	//# \also	$@SpecularAttribute::GetSpecularColor@$
	//# \also	$@SpecularAttribute::SetSpecularColor@$
	
	
	//# \function	SpecularAttribute::SetSpecularExponent		Sets the specular exponent.
	//
	//# \proto	void SetSpecularExponent(float exponent);
	//
	//# \param	exponent	The new specular exponent.
	//
	//# \desc
	//# The $SetSpecularExponent$ function sets the specular exponent stored in a specular material attribute to the value given by the $exponent$ parameter.
	//# A renderable object to which the affected attribute applies will <i>not</i> automatically be rendered using the new exponent.
	//# The change will only take effect once the $@Renderable::InvalidateShaderData@$ function has been called for the affected object.
	
	
	class SpecularAttribute : public Attribute
	{
		private:
			
			ColorRGBA		specularColor;
			float			specularExponent;
			
			Attribute *Replicate(void) const override;
		
		public:
			
			C4API explicit SpecularAttribute(unsigned_int32 flags = 0);
			C4API SpecularAttribute(const ColorRGBA& color, float exponent, unsigned_int32 flags = 0);
			C4API SpecularAttribute(const SpecularAttribute& specularAttribute);
			C4API ~SpecularAttribute();
			
			const ColorRGBA& GetSpecularColor(void) const
			{
				return (specularColor);
			}
			
			void SetSpecularColor(const ColorRGBA& color)
			{
				specularColor = color;
			}
			
			const float& GetSpecularExponent(void) const
			{
				return (specularExponent);
			}
			
			void SetSpecularExponent(float exponent)
			{
				specularExponent = exponent;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			bool operator ==(const Attribute& attribute) const;
			
			void SetAttributeColor(const ColorRGBA& color);
	};
	
	
	//# \class	EmissionAttribute		Material attribute for a emission color.
	//
	//# The $EmissionAttribute$ class represents the material attribute for a emission color.
	//
	//# \def	class EmissionAttribute : public Attribute
	//
	//# \ctor	explicit EmissionAttribute(const ColorRGBA& color);
	//
	//# \param	color	The initial emission color.
	//
	//# \desc
	//# The $EmissionAttribute$ class represents the material attribute for a emission color.
	//# The emission color is applied during the ambient rendering pass only.
	//
	//# \base	Attribute	An $EmissionAttribute$ is a specific type of $Attribute$.
	//
	//# \also	$@Math/ColorRGBA@$
	
	
	//# \function	EmissionAttribute::GetEmissionColor		Returns the emission color.
	//
	//# \proto	const ColorRGBA& GetEmissionColor(void) const;
	//
	//# \desc
	//# The $GetEmissionColor$ function returns the emission color stored in a emission material attribute.
	//
	//# \also	$@EmissionAttribute::SetEmissionColor@$
	
	
	//# \function	EmissionAttribute::SetEmissionColor		Sets the emission color.
	//
	//# \proto	void SetEmissionColor(const ColorRGBA& color);
	//
	//# \param	color	The new emission color.
	//
	//# \desc
	//# The $SetEmissionColor$ attribute sets the emission color stored in a emission material attribute to the value given by the $color$ parameter.
	//# A renderable object to which the affected attribute applies will subsequently be rendered using the new color.
	//
	//# \also	$@EmissionAttribute::GetEmissionColor@$
	
	
	class EmissionAttribute : public Attribute
	{
		private:
			
			ColorRGBA		emissionColor;
			
			Attribute *Replicate(void) const override;
		
		public:
			
			C4API explicit EmissionAttribute(unsigned_int32 flags = 0);
			C4API explicit EmissionAttribute(const ColorRGBA& color, unsigned_int32 flags = 0);
			C4API EmissionAttribute(const EmissionAttribute& emissionAttribute);
			C4API ~EmissionAttribute();
			
			const ColorRGBA& GetEmissionColor(void) const
			{
				return (emissionColor);
			}
			
			void SetEmissionColor(const ColorRGBA& color)
			{
				emissionColor = color;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			bool operator ==(const Attribute& attribute) const;
			
			void SetAttributeColor(const ColorRGBA& color);
	};
	
	
	//# \class	EnvironmentAttribute		Material attribute for an environment map color.
	//
	//# The $EnvironmentAttribute$ class represents the material attribute for an environment map color.
	//
	//# \def	class EnvironmentAttribute : public Attribute
	//
	//# \ctor	explicit EnvironmentAttribute(const ColorRGBA& color);
	//
	//# \param	color	The initial environment color.
	//
	//# \desc
	//# The $EnvironmentAttribute$ class represents the material attribute for an environment map color.
	//# The environment color is applied during the ambient rendering pass only.
	//
	//# \base	Attribute	An $EnvironmentAttribute$ is a specific type of $Attribute$.
	//
	//# \also	$@Math/ColorRGBA@$
	
	
	//# \function	EnvironmentAttribute::GetEnvironmentColor		Returns the environment color.
	//
	//# \proto	const ColorRGBA& GetEnvironmentColor(void) const;
	//
	//# \desc
	//# The $GetEnvironmentColor$ function returns the environment color stored in an environment material attribute.
	//
	//# \also	$@EnvironmentAttribute::SetEnvironmentColor@$
	
	
	//# \function	EnvironmentAttribute::SetEnvironmentColor		Sets the environment color.
	//
	//# \proto	void SetEnvironmentColor(const ColorRGBA& color);
	//
	//# \param	color	The new environment color.
	//
	//# \desc
	//# The $SetEnvironmentColor$ attribute sets the environment color stored in an environment material attribute to the value given by the $color$ parameter.
	//# A renderable object to which the affected attribute applies will subsequently be rendered using the new color.
	//
	//# \also	$@EnvironmentAttribute::GetEnvironmentColor@$
	
	
	class EnvironmentAttribute : public Attribute
	{
		private:
			
			ColorRGBA		environmentColor;
			
			Attribute *Replicate(void) const override;
		
		public:
			
			C4API explicit EnvironmentAttribute(unsigned_int32 flags = 0);
			C4API explicit EnvironmentAttribute(const ColorRGBA& color, unsigned_int32 flags = 0);
			C4API EnvironmentAttribute(const EnvironmentAttribute& environmentAttribute);
			C4API ~EnvironmentAttribute();
			
			const ColorRGBA& GetEnvironmentColor(void) const
			{
				return (environmentColor);
			}
			
			void SetEnvironmentColor(const ColorRGBA& color)
			{
				environmentColor = color;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			bool operator ==(const Attribute& attribute) const;
			
			void SetAttributeColor(const ColorRGBA& color);
	};
	
	
	//# \class	ReflectionAttribute		Material attribute for a reflection color.
	//
	//# The $ReflectionAttribute$ class represents the material attribute for a reflection color.
	//
	//# \def	class ReflectionAttribute : public Attribute
	//
	//# \ctor	ReflectionAttribute(const ColorRGBA& color, float normalReflect, float scale);
	//
	//# \param	color			The initial reflection color.
	//# \param	normalReflect	The normal incidence reflectivity.
	//# \param	scale			The warp offset scale.
	//
	//# \desc
	//# 
	//
	//# \base	Attribute	A $ReflectionAttribute$ is a specific type of $Attribute$.
	//
	//# \also	$@Math/ColorRGBA@$
	
	
	//# \function	ReflectionAttribute::GetReflectionColor		Sets the reflection color.
	//
	//# \proto	const ColorRGBA& GetReflectionColor(void) const;
	//
	//# \desc
	//# The $GetReflectionColor$ function returns the reflection color stored in a reflection material attribute.
	//
	//# \also	$@ReflectionAttribute::SetReflectionColor@$
	
	
	//# \function	ReflectionAttribute::SetReflectionColor		Sets the reflection color.
	//
	//# \proto	void SetReflectionColor(const ColorRGBA& color);
	//
	//# \param	color	The new reflection color.
	//
	//# \desc
	//# The $SetReflectionColor$ attribute sets the reflection color stored in a reflection material attribute to the value given by the $color$ parameter.
	//# A renderable object to which the affected attribute applies will subsequently be rendered using the new color.
	//
	//# \also	$@ReflectionAttribute::GetReflectionColor@$
	
	
	class ReflectionAttribute : public Attribute
	{
		public:
			
			struct ReflectionParams
			{
				float		normalIncidenceReflectivity;
				float		reflectionOffsetScale;
			};
		
		private:
			
			ColorRGBA			reflectionColor;
			ReflectionParams	reflectionParams;
			
			Attribute *Replicate(void) const override;
		
		public:
			
			C4API explicit ReflectionAttribute(unsigned_int32 flags = 0);
			C4API ReflectionAttribute(const ColorRGBA& color, float factor, float scale, unsigned_int32 flags = 0);
			C4API ReflectionAttribute(const ReflectionAttribute& reflectionAttribute);
			C4API ~ReflectionAttribute();
			
			const ColorRGBA& GetReflectionColor(void) const
			{
				return (reflectionColor);
			}
			
			void SetReflectionColor(const ColorRGBA& color)
			{
				reflectionColor = color;
			}
			
			const ReflectionParams *GetReflectionParams(void) const
			{
				return (&reflectionParams);
			}
			
			void SetNormalIncidenceReflectivity(float normalReflect)
			{
				reflectionParams.normalIncidenceReflectivity = normalReflect;
			}
			
			void SetReflectionOffsetScale(float scale)
			{
				reflectionParams.reflectionOffsetScale = scale;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			bool operator ==(const Attribute& attribute) const;
			
			void SetAttributeColor(const ColorRGBA& color);
	};
	
	
	//# \class	RefractionAttribute		Material attribute for a refraction color.
	//
	//# The $RefractionAttribute$ class represents the material attribute for a refraction color.
	//
	//# \def	class RefractionAttribute : public Attribute
	//
	//# \ctor	RefractionAttribute(const ColorRGBA& color, float scale);
	//
	//# \param	color		The initial refraction color.
	//# \param	scale		The warp offset scale.
	//
	//# \desc
	//# 
	//
	//# \base	Attribute	A $RefractionAttribute$ is a specific type of $Attribute$.
	//
	//# \also	$@Math/ColorRGBA@$
	
	
	//# \function	RefractionAttribute::GetRefractionColor		Sets the refraction color.
	//
	//# \proto	const ColorRGBA& GetRefractionColor(void) const;
	//
	//# \desc
	//# The $GetRefractionColor$ function returns the refraction color stored in a refraction material attribute.
	//
	//# \also	$@RefractionAttribute::SetRefractionColor@$
	
	
	//# \function	RefractionAttribute::SetRefractionColor		Sets the refraction color.
	//
	//# \proto	void SetRefractionColor(const ColorRGBA& color);
	//
	//# \param	color	The new refraction color.
	//
	//# \desc
	//# The $SetRefractionColor$ attribute sets the refraction color stored in a refraction material attribute to the value given by the $color$ parameter.
	//# A renderable object to which the affected attribute applies will subsequently be rendered using the new color.
	//
	//# \also	$@RefractionAttribute::GetRefractionColor@$
	
	
	class RefractionAttribute : public Attribute
	{
		public:
			
			struct RefractionParams
			{
				float		refractionOffsetScale;
			};
		
		private:
			
			ColorRGBA			refractionColor;
			RefractionParams	refractionParams;
			
			Attribute *Replicate(void) const override;
		
		public:
			
			C4API explicit RefractionAttribute(unsigned_int32 flags = 0);
			C4API RefractionAttribute(const ColorRGBA& color, float scale, unsigned_int32 flags = 0);
			C4API RefractionAttribute(const RefractionAttribute& refractionAttribute);
			C4API ~RefractionAttribute();
			
			const ColorRGBA& GetRefractionColor(void) const
			{
				return (refractionColor);
			}
			
			void SetRefractionColor(const ColorRGBA& color)
			{
				refractionColor = color;
			}
			
			const RefractionParams *GetRefractionParams(void) const
			{
				return (&refractionParams);
			}
			
			void SetRefractionOffsetScale(float scale)
			{
				refractionParams.refractionOffsetScale = scale;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			bool operator ==(const Attribute& attribute) const;
			
			void SetAttributeColor(const ColorRGBA& color);
	};
	
	
	//# \class	MicrofacetAttribute		Material attribute for microfacet shading.
	//
	//# The $MicrofacetAttribute$ class represents the material attribute for microfacet shading.
	//
	//# \def	class MicrofacetAttribute : public Attribute
	//
	//# \ctor	MicrofacetAttribute(const ColorRGBA& color, const Vector2D& slope, float reflectivity);
	//
	//# \param	color			The microfacet color.
	//# \param	slope			The average microfacet slope in the two tangent directions.
	//# \param	reflectivity	The microfacet reflectivity.
	//
	//# \desc
	//# 
	//
	//# \base	Attribute	A $MicrofacetAttribute$ is a specific type of $Attribute$.
	//
	//# \also	$@Math/ColorRGBA@$
	
	
	class MicrofacetAttribute : public Attribute
	{
		public:
			
			enum
			{
				kMicrofacetTextureSize = 32
			};
			
			class MicrofacetTexture : public Shared, public LinkTarget<MicrofacetTexture>
			{
				private:
					
					Texture		*texture;
					ColorRGB	microfacetColor;
				
				protected:
					
					MicrofacetTexture(const ColorRGB& color);
					~MicrofacetTexture();
					
					void SetTexture(const TextureHeader *header, const void *image)
					{
						texture = Texture::Get(header, image);
					}
				
				public:
					
					const ColorRGB& GetMicrofacetColor(void) const
					{
						return (microfacetColor);
					}
					
					const Texture *GetTextureObject(void) const
					{
						return (texture);
					}
			};
			
			class IsotropicMicrofacetTexture : public MicrofacetTexture, public HashTableElement<IsotropicMicrofacetTexture>
			{
				public:
					
					struct KeyType
					{
						ColorRGB	color;
						float		slope;
					};
				
				private:
					
					KeyType		textureKey;
					Color4C		textureImage[kMicrofacetTextureSize * kMicrofacetTextureSize];
				
				public:
					
					IsotropicMicrofacetTexture(const ColorRGB& color, float slope, float threshold);
					~IsotropicMicrofacetTexture();
					
					const KeyType& GetKey(void) const
					{
						return (textureKey);
					}
					
					static unsigned_int32 Hash(const KeyType& key)
					{
						return (MaxZero((unsigned_int32) (key.slope * 100.0F + (key.color.red + key.color.green + key.color.blue) * 16.0F)));
					}
			};
			
			class AnisotropicMicrofacetTexture : public MicrofacetTexture, public HashTableElement<AnisotropicMicrofacetTexture>
			{
				public:
					
					struct KeyType
					{
						ColorRGB	color;
						Vector2D	slope;
					};
				
				private:
					
					KeyType		textureKey;
					Color4C		textureImage[kMicrofacetTextureSize * kMicrofacetTextureSize * kMicrofacetTextureSize];
				
				public:
					
					AnisotropicMicrofacetTexture(const ColorRGB& color, const Vector2D& slope, float threshold);
					~AnisotropicMicrofacetTexture();
					
					const KeyType& GetKey(void) const
					{
						return (textureKey);
					}
					
					static unsigned_int32 Hash(const KeyType& key)
					{
						return (MaxZero((unsigned_int32) ((key.slope.x + key.slope.y) * 50.0F + (key.color.red + key.color.green + key.color.blue) * 16.0F)));
					}
			};
			
			struct MicrofacetParams
			{
				ColorRGBA							microfacetColor;
				Vector2D							microfacetSlope;
				mutable float						microfacetThreshold;
				mutable Link<MicrofacetTexture>		microfacetTexture;
				
				void Invalidate(void)
				{
					MicrofacetTexture *texture = microfacetTexture;
					if (texture)
					{
						texture->Release();
						microfacetTexture = nullptr;
					}
				}
			};
		
		private:
			
			float				microfacetReflectivity;
			MicrofacetParams	microfacetParams;
			
			static const TextureHeader isotropicMicrofacetTextureHeader;
			static const TextureHeader anisotropicMicrofacetTextureHeader;
			
			static HashTable<IsotropicMicrofacetTexture>	*isotropicHashTable;
			static HashTable<AnisotropicMicrofacetTexture>	*anisotropicHashTable;
			static char										isotropicHashTableStorage[sizeof(HashTable<IsotropicMicrofacetTexture>)];
			static char										anisotropicHashTableStorage[sizeof(HashTable<AnisotropicMicrofacetTexture>)];
			
			Attribute *Replicate(void) const override;
			
			static float CalculateThreshold(const MicrofacetParams *params);
			static ColorRGB CalculateRefractionIndex(const ColorRGB& color);
			static float CalculateFresnelTerm(float L_dot_H, float refractionIndex);
		
		public:
			
			C4API explicit MicrofacetAttribute(unsigned_int32 flags = 0);
			C4API MicrofacetAttribute(const ColorRGBA& color, const Vector2D& slope, float reflectivity, unsigned_int32 flags = 0);
			C4API MicrofacetAttribute(const MicrofacetAttribute& microfacetAttribute);
			C4API ~MicrofacetAttribute();
			
			const MicrofacetParams *GetMicrofacetParams(void) const
			{
				return (&microfacetParams);
			}
			
			void SetMicrofacetColor(const ColorRGBA& color)
			{
				microfacetParams.microfacetColor = color;
				microfacetParams.Invalidate();
			}
			
			void SetMicrofacetSlope(const Vector2D& slope)
			{
				microfacetParams.microfacetSlope = slope;
				microfacetParams.Invalidate();
			}
			
			float GetMicrofacetReflectivity(void) const
			{
				return (microfacetReflectivity);
			}
			
			void SetMicrofacetReflectivity(float reflectivity)
			{
				microfacetReflectivity = reflectivity;
			}
			
			static void Initialize(void);
			static void Terminate(void);
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			bool operator ==(const Attribute& attribute) const;
			
			void SetAttributeColor(const ColorRGBA& color);
			
			static const Texture *GetTextureObject(const MicrofacetParams *params);
	};
	
	
	inline bool operator ==(const MicrofacetAttribute::IsotropicMicrofacetTexture::KeyType& x, const MicrofacetAttribute::IsotropicMicrofacetTexture::KeyType& y)
	{
		return ((x.color == y.color) && (x.slope == y.slope));
	}
	
	inline bool operator ==(const MicrofacetAttribute::AnisotropicMicrofacetTexture::KeyType& x, const MicrofacetAttribute::AnisotropicMicrofacetTexture::KeyType& y)
	{
		return ((x.color == y.color) && (x.slope == y.slope));
	}
	
	
	//# \class	MapAttribute		Material attribute base class for texture maps.
	//
	//# The $MapAttribute$ class is the base class for attributes that use a texture map.
	//
	//# \def	class MapAttribute : public Attribute
	//
	//# \ctor	MapAttribute(AttributeType type, const char *name);
	//# \ctor	MapAttribute(AttributeType type, Texture *texture);
	//# \ctor	MapAttribute(AttributeType type, const TextureHeader *header, const void *image = nullptr);
	//
	//# The constructors have protected access. The $MapAttribute$ class can only exist as the base class
	//# for other material attribute classes that use texture maps.
	//
	//# \param	type		The attribute type. This must be the type of a subclass that inherits from $MapAttribute$.
	//# \param	name		The name of the texture map to load.
	//# \param	texture		The texture object to use. Specifying this parameter increments the reference count of the texture object.
	//# \param	header		A texture header from which to construct a new texture object.
	//# \param	image		A pointer to a texture image that is used if the texture header does not specify an offset to an image.
	//
	//# \desc
	//# The $MapAttribute$ class encapsulates information about a texture map for other material attributes.
	//# If a $MapAttribute$ object is constructed by passing the $name$ parameter, then the texture map
	//# is loaded through the Resource Manager. If a $MapAttribute$ object is constructed by passing the
	//# $header$ and $image$ parameters, then a new texture map is constructed using the information in
	//# the $@TextureHeader@$ structure. If the $image$ parameter is not $nullptr$, then the texture image
	//# is read from the location to which it points; otherwise, the texture header specifies the location
	//# of the image.
	//
	//# \base	Attribute	A $MapAttribute$ is a specific type of $Attribute$.
	//
	//# \also	$@TextureMapAttribute@$
	//# \also	$@NormalMapAttribute@$
	//# \also	$@GlossMapAttribute@$
	//# \also	$@EmissionMapAttribute@$
	
	
	//# \function	MapAttribute::GetTextureName		Returns the name of the texture map.
	//
	//# \proto	const ResourceName& GetTextureName(void) const;
	//
	//# \desc
	//# The $GetTextureName$ function returns the name of the texture map used by the attribute.
	//# If the texture map does not have a name (because it was not loaded from a resource),
	//# then the name is the empty string.
	//
	//# \also	$@MapAttribute::GetTexture@$
	//# \also	$@MapAttribute::SetTexture@$
	
	
	//# \function	MapAttribute::GetTexture		Returns the texture map object.
	//
	//# \proto	Texture *const& GetTexture(void) const;
	//
	//# \desc
	//# The $GetTexture$ function returns a pointer to the texture map object used by the attribute.
	//
	//# \also	$@MapAttribute::SetTexture@$
	//# \also	$@MapAttribute::GetTextureName@$
	
	
	//# \function	MapAttribute::SetTexture		Sets the texture map object.
	//
	//# \proto	void SetTexture(const char *name);
	//# \proto	void SetTexture(Texture *texture);
	//# \proto	void SetTexture(const TextureHeader *header, const void *image = nullptr);
	//
	//# \param	name		The name of the texture map to load.
	//# \param	texture		The texture object to use. Specifying this parameter increments the reference count of the texture object.
	//# \param	header		A texture header from which to construct a new texture object.
	//# \param	image		A pointer to a texture image that is used if the texture header does not specify an offset to an image.
	//
	//# \desc
	//# The $SetTexture$ function sets the texture map object used by the attribute.
	//# If the $name$ parameter is passed to this function, then the texture map
	//# is loaded through the Resource Manager. If the $header$ and $image$ parameters are used,
	//# then a new texture map is constructed using the information in the $@TextureHeader@$ structure.
	//# If the $image$ parameter is not $nullptr$, then the texture image is read from the location
	//# to which it points; otherwise, the texture header specifies the location of the image.
	//
	//# \also	$@MapAttribute::GetTexture@$
	
	
	class MapAttribute : public Attribute
	{
		private:
			
			ResourceName	textureName;
			Texture			*textureObject;
			
			int32			texcoordIndex;
		
		protected:
			
			MapAttribute(AttributeType type);
			MapAttribute(AttributeType type, const char *name);
			MapAttribute(AttributeType type, Texture *texture);
			MapAttribute(AttributeType type, const TextureHeader *header, const void *image = nullptr);
			MapAttribute(AttributeType type, const char *name, const TextureHeader *header, const void *image = nullptr);
			MapAttribute(const MapAttribute& mapAttribute);
		
		public:
			
			C4API ~MapAttribute();
			
			const ResourceName& GetTextureName(void) const
			{
				return (textureName);
			}
			
			Texture *GetTexture(void) const
			{
				return (textureObject);
			}
			
			int32 GetTexcoordIndex(void) const
			{
				return (texcoordIndex);
			}
			
			void SetTexcoordIndex(int32 index)
			{
				texcoordIndex = index;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			void *BeginSettingsUnpack(void);
			
			bool operator ==(const Attribute& attribute) const;
			
			C4API void SetTexture(const char *name);
			C4API void SetTexture(Texture *texture);
			C4API void SetTexture(const TextureHeader *header, const void *image = nullptr);
	};
	
	
	//# \class	TextureMapAttribute		Material attribute for a diffuse texture map.
	//
	//# The $TextureMapAttribute$ class represents the material attribute for a diffuse texture map.
	//
	//# \def	class TextureMapAttribute : public MapAttribute
	//
	//# \ctor	explicit TextureMapAttribute(const char *name);
	//# \ctor	explicit TextureMapAttribute(Texture *texture);
	//# \ctor	TextureMapAttribute(const TextureHeader *header, const void *image = nullptr);
	//
	//# \param	name		The name of the texture map to load.
	//# \param	texture		The texture object to use.
	//# \param	header		A texture header from which to construct a new texture object.
	//# \param	image		A pointer to a texture image that is used if the texture header does not specify an offset to an image.
	//
	//# \desc
	//# The $TextureMapAttribute$ class represents the material attribute for a diffuse texture map.
	//# The diffuse texture map is applied during both the ambient rendering pass and each lighting pass.
	//# 
	//# See the $@MapAttribute@$ class for a description of the differences among the various constructors.
	//
	//# \base	MapAttribute	All attributes using a texture map are subclasses of $MapAttribute$.
	
	
	class TextureMapAttribute : public MapAttribute
	{
		private:
			
			Attribute *Replicate(void) const override;
		
		public:
			
			C4API TextureMapAttribute();
			C4API explicit TextureMapAttribute(const char *name);
			C4API explicit TextureMapAttribute(Texture *texture);
			C4API TextureMapAttribute(const TextureHeader *header, const void *image = nullptr);
			C4API TextureMapAttribute(const char *name, const TextureHeader *header, const void *image = nullptr);
			C4API TextureMapAttribute(const TextureMapAttribute& textureMapAttribute);
			C4API ~TextureMapAttribute();
	};
	
	
	//# \class	NormalMapAttribute		Material attribute for a normal map.
	//
	//# The $NormalMapAttribute$ class represents the material attribute for a normal map.
	//
	//# \def	class NormalMapAttribute : public MapAttribute
	//
	//# \ctor	explicit NormalMapAttribute(const char *name);
	//# \ctor	explicit NormalMapAttribute(Texture *texture);
	//# \ctor	NormalMapAttribute(const TextureHeader *header, const void *image = nullptr);
	//
	//# \param	name		The name of the texture map to load.
	//# \param	texture		The texture object to use.
	//# \param	header		A texture header from which to construct a new texture object.
	//# \param	image		A pointer to a texture image that is used if the texture header does not specify an offset to an image.
	//
	//# \desc
	//# The $NormalMapAttribute$ class represents the material attribute for a normal map.
	//# The normal map is applied during each lighting pass and is also used for environment-mapped bump mapping.
	//# 
	//# See the $@MapAttribute@$ class for a description of the differences among the various constructors.
	//
	//# \base	MapAttribute	All attributes using a texture map are subclasses of $MapAttribute$.
	
	
	class NormalMapAttribute : public MapAttribute
	{
		private:
			
			float	parallaxScale;
			
			Attribute *Replicate(void) const override;
		
		public:
			
			C4API NormalMapAttribute();
			C4API explicit NormalMapAttribute(const char *name);
			C4API explicit NormalMapAttribute(Texture *texture);
			C4API NormalMapAttribute(const TextureHeader *header, const void *image = nullptr);
			C4API NormalMapAttribute(const NormalMapAttribute& normalMapAttribute);
			C4API ~NormalMapAttribute();
			
			const float& GetParallaxScale(void) const
			{
				return (parallaxScale);
			}
			
			void SetParallaxScale(float scale)
			{
				parallaxScale = scale;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			void *BeginSettingsUnpack(void);
			
			bool operator ==(const Attribute& attribute) const;
	};
	
	
	//# \class	HorizonMapAttribute		Material attribute for a horizon map.
	//
	//# The $HorizonMapAttribute$ class represents the material attribute for a horizon map.
	//
	//# \def	class HorizonMapAttribute : public MapAttribute
	//
	//# \ctor	explicit HorizonMapAttribute(const char *name);
	//# \ctor	explicit HorizonMapAttribute(Texture *texture);
	//# \ctor	HorizonMapAttribute(const TextureHeader *header, const void *image = nullptr);
	//
	//# \param	name		The name of the texture map to load.
	//# \param	texture		The texture object to use.
	//# \param	header		A texture header from which to construct a new texture object.
	//# \param	image		A pointer to a texture image that is used if the texture header does not specify an offset to an image.
	//
	//# \desc
	//# The $HorizonMapAttribute$ class represents the material attribute for a horizon map.
	//# Two specially-computed horizon maps are required for horizon mapping to work properly.
	//# 
	//# See the $@MapAttribute@$ class for a description of the differences among the various constructors.
	//
	//# \base	MapAttribute	All attributes using a texture map are subclasses of $MapAttribute$.
	
	
	class HorizonMapAttribute : public MapAttribute
	{
		private:
			
			unsigned_int32		horizonFlags;
			
			Attribute *Replicate(void) const override;
		
		public:
			
			C4API HorizonMapAttribute();
			C4API explicit HorizonMapAttribute(const char *name);
			C4API explicit HorizonMapAttribute(Texture *texture);
			C4API HorizonMapAttribute(const TextureHeader *header, const void *image = nullptr);
			C4API HorizonMapAttribute(const HorizonMapAttribute& horizonMapAttribute);
			C4API ~HorizonMapAttribute();
			
			unsigned_int32 GetHorizonFlags(void) const
			{
				return (horizonFlags);
			}
			
			void SetHorizonFlags(unsigned_int32 flags)
			{
				horizonFlags = flags;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			void *BeginSettingsUnpack(void);
			
			bool operator ==(const Attribute& attribute) const;
	};
	
	
	//# \class	GlossMapAttribute		Material attribute for a gloss map.
	//
	//# The $GlossMapAttribute$ class represents the material attribute for a gloss map.
	//
	//# \def	class GlossMapAttribute : public MapAttribute
	//
	//# \ctor	explicit GlossMapAttribute(const char *name);
	//# \ctor	explicit GlossMapAttribute(Texture *texture);
	//# \ctor	GlossMapAttribute(const TextureHeader *header, const void *image = nullptr);
	//
	//# \param	name		The name of the texture map to load.
	//# \param	texture		The texture object to use.
	//# \param	header		A texture header from which to construct a new texture object.
	//# \param	image		A pointer to a texture image that is used if the texture header does not specify an offset to an image.
	//
	//# \desc
	//# The $GlossMapAttribute$ class represents the material attribute for a gloss map.
	//# The gloss map modulates specular reflection and environment mapping.
	//# 
	//# See the $@MapAttribute@$ class for a description of the differences among the various constructors.
	//
	//# \base	MapAttribute	All attributes using a texture map are subclasses of $MapAttribute$.
	
	
	class GlossMapAttribute : public MapAttribute
	{
		private:
			
			Attribute *Replicate(void) const override;
		
		public:
			
			C4API GlossMapAttribute();
			C4API explicit GlossMapAttribute(const char *name);
			C4API explicit GlossMapAttribute(Texture *texture);
			C4API GlossMapAttribute(const TextureHeader *header, const void *image = nullptr);
			C4API GlossMapAttribute(const GlossMapAttribute& glossMapAttribute);
			C4API ~GlossMapAttribute();
	};
	
	
	//# \class	EmissionMapAttribute		Material attribute for an emission map.
	//
	//# The $EmissionMapAttribute$ class represents the material attribute for an emission map.
	//
	//# \def	class EmissionMapAttribute : public MapAttribute
	//
	//# \ctor	explicit EmissionMapAttribute(const char *name);
	//# \ctor	explicit EmissionMapAttribute(Texture *texture);
	//# \ctor	EmissionMapAttribute(const TextureHeader *header, const void *image = nullptr);
	//
	//# \param	name		The name of the texture map to load.
	//# \param	texture		The texture object to use.
	//# \param	header		A texture header from which to construct a new texture object.
	//# \param	image		A pointer to a texture image that is used if the texture header does not specify an offset to an image.
	//
	//# \desc
	//# The $EmissionMapAttribute$ class represents the material attribute for an emission map.
	//# The emission map is applied only during the ambient rendering pass.
	//# 
	//# See the $@MapAttribute@$ class for a description of the differences among the various constructors.
	//
	//# \base	MapAttribute	All attributes using a texture map are subclasses of $MapAttribute$.
	
	
	class EmissionMapAttribute : public MapAttribute
	{
		private:
			
			Attribute *Replicate(void) const override;
		
		public:
			
			C4API EmissionMapAttribute();
			C4API explicit EmissionMapAttribute(const char *name);
			C4API explicit EmissionMapAttribute(Texture *texture);
			C4API EmissionMapAttribute(const TextureHeader *header, const void *image = nullptr);
			C4API EmissionMapAttribute(const EmissionMapAttribute& emissionMapAttribute);
			C4API ~EmissionMapAttribute();
	};
	
	
	//# \class	OpacityMapAttribute		Material attribute for an opacity map.
	//
	//# The $OpacityMapAttribute$ class represents the material attribute for an opacity map.
	//
	//# \def	class OpacityMapAttribute : public MapAttribute
	//
	//# \ctor	explicit OpacityMapAttribute(const char *name);
	//# \ctor	explicit OpacityMapAttribute(Texture *texture);
	//# \ctor	OpacityMapAttribute(const TextureHeader *header, const void *image = nullptr);
	//
	//# \param	name		The name of the texture map to load.
	//# \param	texture		The texture object to use.
	//# \param	header		A texture header from which to construct a new texture object.
	//# \param	image		A pointer to a texture image that is used if the texture header does not specify an offset to an image.
	//
	//# \desc
	//# The $OpacityMapAttribute$ class represents the material attribute for an opacity map.
	//# The opacity map is applied only during the ambient rendering pass.
	//# 
	//# See the $@MapAttribute@$ class for a description of the differences among the various constructors.
	//
	//# \base	MapAttribute	All attributes using a texture map are subclasses of $MapAttribute$.
	
	
	class OpacityMapAttribute : public MapAttribute
	{
		private:
			
			Attribute *Replicate(void) const override;
		
		public:
			
			C4API OpacityMapAttribute();
			C4API explicit OpacityMapAttribute(const char *name);
			C4API explicit OpacityMapAttribute(Texture *texture);
			C4API OpacityMapAttribute(const TextureHeader *header, const void *image = nullptr);
			C4API OpacityMapAttribute(const OpacityMapAttribute& opacityMapAttribute);
			C4API ~OpacityMapAttribute();
	};
	
	
	//# \class	EnvironmentMapAttribute		Material attribute for an environment map.
	//
	//# The $EnvironmentMapAttribute$ class represents the material attribute for an environment map.
	//
	//# \def	class EnvironmentMapAttribute : public MapAttribute
	//
	//# \ctor	explicit EnvironmentMapAttribute(const char *name);
	//# \ctor	explicit EnvironmentMapAttribute(Texture *texture);
	//# \ctor	EnvironmentMapAttribute(const TextureHeader *header, const void *image = nullptr);
	//
	//# \param	name		The name of the texture map to load.
	//# \param	texture		The texture object to use.
	//# \param	header		A texture header from which to construct a new texture object.
	//# \param	image		A pointer to a texture image that is used if the texture header does not specify an offset to an image.
	//
	//# \desc
	//# The $EnvironmentMapAttribute$ class represents the material attribute for an environment map.
	//# If this attribute appears in a renderable object's attribute list, then it overrides any external environment map.
	//# The environment map is applied only during the ambient rendering pass.
	//# 
	//# See the $@MapAttribute@$ class for a description of the differences among the various constructors.
	//
	//# \base	MapAttribute	All attributes using a texture map are subclasses of $MapAttribute$.
	
	
	class EnvironmentMapAttribute : public MapAttribute
	{
		private:
			
			Attribute *Replicate(void) const override;
		
		public:
			
			C4API EnvironmentMapAttribute();
			C4API explicit EnvironmentMapAttribute(const char *name);
			C4API explicit EnvironmentMapAttribute(Texture *texture);
			C4API EnvironmentMapAttribute(const TextureHeader *header, const void *image = nullptr);
			C4API EnvironmentMapAttribute(const EnvironmentMapAttribute& environmentMapAttribute);
			C4API ~EnvironmentMapAttribute();
	};
	
	
	class DeltaDepthAttribute : public Attribute
	{
		private:
			
			float		deltaScale;
			
			Attribute *Replicate(void) const override;
		
		public:
			
			C4API DeltaDepthAttribute();
			C4API explicit DeltaDepthAttribute(float scale);
			C4API DeltaDepthAttribute(const DeltaDepthAttribute& deltaDepthAttribute);
			C4API ~DeltaDepthAttribute();
			
			float GetDeltaScale(void) const
			{
				return (deltaScale);
			}
			
			void SetDeltaScale(float scale)
			{
				deltaScale = scale;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			bool operator ==(const Attribute& attribute) const;
	};
	
	
	class FireAttribute : public MapAttribute
	{
		public:
			
			struct FireParams
			{
				float		fireIntensity;
				Vector2D	noiseVelocity[3];
			};
		
		private:
			
			FireParams		fireParams;
			
			Attribute *Replicate(void) const override;
		
		public:
			
			C4API FireAttribute();
			C4API FireAttribute(float intensity, const Vector2D& velocity1, const Vector2D& velocity2, const Vector2D& velocity3);
			C4API FireAttribute(const FireAttribute& fireAttribute);
			C4API ~FireAttribute();
			
			const FireParams *GetFireParams(void) const
			{
				return (&fireParams);
			}
			
			void SetFireIntensity(float intensity)
			{
				fireParams.fireIntensity = intensity;
			}
			
			void SetFireSpeed(int32 speed)
			{
				CalculateNoiseVelocities(speed, fireParams.noiseVelocity);
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			bool operator ==(const Attribute& attribute) const;
			
			C4API static void CalculateNoiseVelocities(int32 speed, Vector2D *velocity);
	};
}


#endif

// ZYURVUR
