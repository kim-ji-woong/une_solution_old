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


#ifndef C4Effects_h
#define C4Effects_h


//# \component	Effect Manager
//# \prefix		EffectMgr/


#include "C4Node.h"
#include "C4Renderable.h"
#include "C4Attributes.h"
#include "C4Paths.h"


namespace C4
{
	typedef Type	EffectType;
	
	
	enum
	{
		kObjectEffect		= 'EFCT'
	};
	
	
	//# \enum	EffectType
	
	enum
	{
		kEffectParticleSystem	= 'PART',		//## A particle system effect.
		kEffectMarking			= 'MARK',		//## A surface marking effect.
		kEffectQuad				= 'QUAD',		//## A generic billboarded quad effect.
		kEffectFlare			= 'FLAR',		//## A fractional-occlusion flare effect.
		kEffectBeam				= 'BEAM',		//## A polyboard beam effect.
		kEffectTube				= 'TUBE',		//## A polyboard tube effect.
		kEffectBolt				= 'BOLT',		//## A lightning bolt effect.
		kEffectFire				= 'FIRE',		//## A procedural fire effect.
		kEffectPanel			= 'PANL'		//## An interface panel effect.
	};
	
	
	//#	\enum	EffectList
	
	enum
	{
		kEffectListLight,			//## Fully lit effects. These are rendered during the ambient pass and lighting passes.
		kEffectListOpaque,			//## Opaque effects. These are rendered after the final lighting pass and before any transparent effects.
		kEffectListTransparent,		//## Transparent effects. These are rendered after all opaque effects and are sorted back to front.
		kEffectListFrontmost,		//## Frontmost effects. These are render after all transparent effects and are not sorted.
		kEffectListOcclusion,		//## Occlusion queries. These are rendered after the ambient pass and before the first lighting pass.
		kEffectListDistortion,		//## Distortion effects. These are rendered into the distortion buffer for post-processing.
		kEffectListVelocity,
		kEffectListCover,
		kEffectListCount
	};
	
	
	//# \enum	EffectFlags
	
	enum
	{
		kEffectStatic			= 1 << 0		//## The effect is always completely contained in its owning zone.
	};
	
	
	//# \enum	MarkingFlags
	
	enum
	{
		kMarkingLight			= 1 << 0,		//## The marking receives full lighting.
		kMarkingBlendLight		= 1 << 1,		//## Alpha blending is applied to a lighted marking (valid only if the $kMarkingLight$ flag is set).
		kMarkingDepthWrite		= 1 << 2,		//## The marking writes to the depth buffer (valid only if the $kMarkingLight$ flag is set).
		kMarkingTwoSided		= 1 << 3,		//## The marking is rendered two-sided (valid only if the $kMarkingLight$ flag is <i>not</i> set).
		kMarkingClipRange		= 1 << 4		//## The minimum and maximum depths to which the marking is clipped are given by the $clip$ field of the $@MarkingData@$ structure.
	};
	
	
	//# \enum	QuadFlags
	
	enum
	{
		kQuadInfinite			= 1 << 0,		//## The quad is rendered at infinity.
		kQuadSoftDepth			= 1 << 1		//## The quad fades out as it gets close to scene geometry to avoid depth-testing artifacts.
	};
	
	
	class Effect;
	class Camera;
	class PointLight;
	class MarkingList;
	
	
	//# \class	EffectRegistration		Manages internal registration information for a custom effect type.
	//
	//# The $EffectRegistration$ class manages internal registration information for a custom effect type.
	//
	//# \def	class EffectRegistration : public Registration<Effect, EffectRegistration>
	//
	//# \ctor	EffectRegistration(EffectType type, const char *name);
	//
	//# \param	type		The effect type.
	//# \param	name		The effect name. 
	//
	//# \desc 
	//# The $EffectRegistration$ class is abstract and serves as the common base class for the template class 
	//# $@EffectReg@$. A custom effect is registered with the engine by instantiating an object of type 
	//# $EffectReg<classType>$, where $classType$ is the type of the effect subclass being registered.
	// 
	//# \base	System/Registration<Effect, EffectRegistration>		An effect registration is a specific type of registration object.
	//
	//# \also	$@EffectReg@$
	//# \also	$@Effect@$ 
	
	
	//# \function	EffectRegistration::GetEffectType		Returns the registered effect type.
	// 
	//# \proto	EffectType GetEffectType(void) const;
	//
	//# \desc
	//# The $GetEffectType$ function returns the effect type for a particular effect registration.
	//# The effect type is established when the effect registration is constructed.
	//
	//# \also	$@EffectRegistration::GetEffectName@$
	
	
	//# \function	EffectRegistration::GetEffectName		Returns the human-readable effect name.
	//
	//# \proto	const char *GetEffectName(void) const;
	//
	//# \desc
	//# The $GetEffectName$ function returns the human-readable effect name for a particular effect registration.
	//# The effect name is established when the effect registration is constructed.
	//
	//# \also	$@EffectRegistration::GetEffectType@$
	
	
	class EffectRegistration : public Registration<Effect, EffectRegistration>
	{
		private:
			
			const char		*effectName;
		
		public:
			
			C4API EffectRegistration(EffectType type, const char *name);
			C4API ~EffectRegistration();
			
			EffectType GetEffectType(void) const
			{
				return (GetRegistrableType());
			}
			
			const char *GetEffectName(void) const
			{
				return (effectName);
			}
	};
	
	
	//# \class	EffectReg	 Represents a custom effect type.
	//
	//# The $EffectReg$ class represents a custom effect type.
	//
	//# \def	template <class classType> class EffectReg : public EffectRegistration
	//
	//# \tparam	classType	The custom effect class.
	//
	//# \ctor	EffectReg(EffectType type, const char *name);
	//
	//# \param	type		The effect type.
	//# \param	name		The effect name.
	//
	//# \desc
	//# The $EffectReg$ template class is used to advertise the existence of a custom effect type.
	//# The Effect Manager uses an effect registration to construct a custom effect. The act of instantiating an
	//# $EffectReg$ object automatically registers the corresponding effect type. The effect type is unregistered
	//# when the $EffectReg$ object is destroyed.
	//# 
	//# No more than one effect registration should be created for each distinct effect type.
	//
	//# \base	EffectRegistration		All specific effect registration classes share the common base class $EffectRegistration$.
	//
	//# \also	$@Effect@$
	
	
	template <class classType> class EffectReg : public EffectRegistration
	{
		public:
			
			EffectReg(EffectType type, const char *name) : EffectRegistration(type, name)
			{
			}
			
			Effect *Construct(void) const
			{
				return (new classType);
			}
	};
	
	
	//# \class	EffectObject		Encapsulates data pertaining to a special effect.
	//
	//# The $EffectObject$ class encapsulates data pertaining to a special effect.
	//
	//# \def	class EffectObject : public Object
	//
	//# \ctor	EffectObject(EffectType type);
	//
	//# The constructor has protected access. The $EffectObject$ class can only exist as the base class for another class.
	//
	//# \param	type	The type of the effect. See below for a list of possible types.
	//
	//# \desc
	//# 
	//# \table	EffectType
	//
	//# \base	WorldMgr/Object		An $EffectObject$ is an object that can be shared by multiple light nodes.
	//
	//# \also	$@Effect@$
	
	
	//# \function	EffectObject::GetEffectType		Returns the specific type of an effect.
	//
	//# \proto	EffectType GetEffectType(void) const;
	//
	//# \desc
	//# The $GetEffectType$ function returns the specific effect type, which may be one of the following values
	//# or an application-defined type.
	//
	//# \table	EffectType
	
	
	class EffectObject : public Object
	{
		friend class WorldMgr;
		
		private:
			
			EffectType		effectType;
			
			static EffectObject *Construct(Unpacker& data, unsigned_int32 unpackFlags);
		
		protected:
			
			C4API EffectObject(EffectType type);
			C4API ~EffectObject();
		
		public:
			
			EffectType GetEffectType(void) const
			{
				return (effectType);
			}
			
			C4API void PackType(Packer& data) const;
	};
	
	
	//# \class	Effect		Represents a special effect node in a world.
	//
	//# The $Effect$ class represents a special effect node in a world.
	//
	//# \def	class Effect : public RenderableNode, public ListElement<Effect>, public Registrable<Effect, EffectRegistration>
	//
	//# \ctor	Effect(EffectType type, RenderType renderType, unsigned_int32 renderState = 0);
	//
	//# The constructor has protected access. The $Effect$ class can only exist as the base class for a more specific type of effect.
	//
	//# \param	type			The effect type.
	//# \param	renderType		The render type passed to the $Renderable$ base class.
	//# \param	renderState		The render state passed to the $Renderable$ base class.
	//
	//# \desc
	//#
	//
	//# \base	WorldMgr/RenderableNode							An $Effect$ node is a renderable scene graph node.
	//# \base	Utilities/ListElement<Effect>					Used internally by the World Manager.
	//# \base	System/Registrable<Effect, EffectRegistration>	Custom effect types can be registered with the engine.
	
	
	//# \function	Effect::GetEffectType		Returns the effect type.
	//
	//# \proto	EffectType GetEffectType(void) const;
	//
	//# \desc
	//# The $GetEffectType$ function returns the specific effect type, which may be one of the following values
	//# or an application-defined type.
	//
	//# \table	EffectType
	
	
	//# \function	Effect::GetEffectListIndex		Returns the effect list index.
	//
	//# \proto	unsigned_int32 GetEffectListIndex(void) const;
	//
	//# \desc
	//# The $GetEffectListIndex$ function returns the index of the render list in which the effect is placed when
	//# it is visible. The list index can be one of the following values.
	//
	//# \table	EffectList
	//
	//# By default, an effect is placed in the $kEffectListTransparent$ list, but some effect subclasses change this value.
	//
	//# \also	$@Effect::SetEffectListIndex@$
	
	
	//# \function	Effect::SetEffectListIndex		Sets the effect list index.
	//
	//# \proto	void SetEffectListIndex(unsigned_int32 index);
	//
	//# \param	index	The effect list index. This may be one of the values listed below.
	//
	//# \desc
	//# The $SetEffectListIndex$ function sets the index of the render list in which the effect is placed when
	//# it is visible. The $index$ parameter can be one of the following values.
	//
	//# \table	EffectList
	//
	//# By default, an effect is placed in the $kEffectListTransparent$ list, but some effect subclasses change this value.
	//
	//# \also	$@Effect::GetEffectListIndex@$
	
	
	//# \function	Effect::SetDistortionState		Sets the state necessary for rendering into the distortion buffer.
	//
	//# \proto	void SetDistortionState(void);
	//
	//# \desc
	//# The $SetDistortionState$ function sets all of the state necessary for rendering an effect into the distortion
	//# buffer. It sets the effect list to $kEffectListDistortion$, sets the $kShaderDistortion$ shader flag, and sets
	//# the blend state to $kBlendAccumulate$. This function should be called by any subclass of $Effect$ that renders
	//# into the distortion buffer.
	//
	//# \also	$@Effect::SetEffectListIndex@$
	
	
	//# \function	Effect::Move		Called once per frame to move an effect.
	//
	//# \proto	virtual void Move(void);
	//
	//# \desc
	//# The $Move$ function is called once per frame to allow an effect to perform any necessary movement.
	//# An effect may safely delete itself inside this function if desired.
	//# 
	//# Any type of processing that only needs to be done when the effect is known to be visible should be
	//# postponed until the $@Effect::Render@$ function is called. This saves computation when the effect
	//# is not visible or is occluded.
	//
	//# \also	$@Effect::Render@$
	
	
	//# \function	Effect::Render		Called when an effect should be rendered.
	//
	//# \proto	virtual void Render(const Camera *camera, List<Renderable> *effectList) = 0;
	//
	//# \param	camera		The camera for which the effect is being rendered.
	//# \param	effectList	An array of render lists to which the effect should add its renderables.
	//
	//# \desc
	//# The $Render$ function is called when the World Manager has determined that an effect needs to be rendered.
	//# This function is only called for an effect node that is enabled and has already passed the visibility
	//# and occlusion tests. (An effect can be disabled by setting the $kNodeDisabled$ flag with the
	//# $@WorldMgr/Node::SetNodeFlags@$ function.)
	//# 
	//# When the $Render$ function is called, an $Effect$ node should use the $@Utilities/List::Append@$ function
	//# to add itself and any additional renderable objects to one or more of the lists in the array specified by
	//# the $effectList$ parameter. This array should be indexed using the following values.
	//
	//# \table	EffectList
	//
	//# For example, to add an effect renderable to the transparent effect list, the $Render$ function should
	//# make the following call.
	//
	//# \code	effectList[kEffectListTransparent].Append(this);
	//
	//# \special
	//# Note that the $Render$ function can be called multiple times during the same frame for an effect that
	//# is visible from multiple cameras. The $Render$ function should not perform any iterative movement under
	//# the assumption that the $Render$ function is called only once. Instead, this type of computation should
	//# be peformed in the $@Effect::Move@$ function.
	//
	//# \also	$@Utilities/List@$
	//# \also	$@GraphicsMgr/Renderable@$
	//# \also	$@Effect::Move@$
	//# \also	$@WorldMgr/Node::SetVisibilityProc@$
	//# \also	$@WorldMgr/Node::SetOcclusionProc@$
	
	
	class C4_API Effect : public RenderableNode, public ListElement<Effect>, public Registrable<Effect, EffectRegistration>
	{
		friend class Node;
		
		private:
			
			EffectType			effectType;
			unsigned_int32		effectFlags;
			unsigned_int32		effectListIndex;
			
			static Effect *Construct(Unpacker& data, unsigned_int32 unpackFlags = 0);
		
		protected:
			
			Effect(EffectType type, RenderType renderType, unsigned_int32 renderState = 0);
			Effect(const Effect& effect);
			
			void CalculatePostBounding(void) override;
			void BondAffectedZones(Zone *zone, const Point3D& center, float radius);
		
		public:
			
			virtual ~Effect();
			
			using ListElement<Effect>::Previous;
			using ListElement<Effect>::Next;
			
			EffectType GetEffectType(void) const
			{
				return (effectType);
			}
			
			EffectObject *GetObject(void) const
			{
				return (static_cast<EffectObject *>(Node::GetObject()));
			}
			
			unsigned_int32 GetEffectFlags(void) const
			{
				return (effectFlags);
			}
			
			void SetEffectFlags(unsigned_int32 flags)
			{
				effectFlags = flags;
			}
			
			unsigned_int32 GetEffectListIndex(void) const
			{
				return (effectListIndex);
			}
			
			void SetEffectListIndex(unsigned_int32 index)
			{
				effectListIndex = index;
			}
			
			void SetDistortionState(void)
			{
				SetAmbientBlendState(kBlendAccumulate);
				SetEffectListIndex(kEffectListDistortion);
				SetShaderFlags(GetShaderFlags() | (kShaderAmbientEffect | kShaderDistortion));
			}
			
			static Effect *New(EffectType type);
			
			void PackType(Packer& data) const;
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			void Preprocess(void);
			void Neutralize(void);
			void EnterZone(Zone *zone);
			void AddEffectBond(Zone *zone);
			
			virtual void UpdateEffectGeometry(void);
			
			virtual void Move(void);
			virtual void Render(const Camera *camera, List<Renderable> *effectList);
	};
	
	
	//# \struct	MarkingData		Defines the parameters of a surface marking effect.
	//
	//# The $MarkingData$ structure defines the parameters of a surface marking effect.
	//
	//# \ctor	MarkingData(unsigned_int32 flags = 0, MarkingList *list = nullptr);
	//# \ctor	MarkingData(const Vector2D& scale, const Vector2D& offset, unsigned_int32 flags = 0, MarkingList *list = nullptr);
	//
	//# \param	flags		The marking effect flags.
	//# \param	list		A pointer to the $@MarkingList@$ object into which each $@MarkingEffect@$ is inserted.
	//# \param	scale		A scale to apply to the (<i>s</i>, <i>t</i>) texture coordinates used by the marking.
	//# \param	offset		An offset to apply to the (<i>s</i>, <i>t</i>) texture coordinates used by the marking.
	//
	//# \data	MarkingData
	//
	//# \desc
	//# The $MarkingData$ structure holds information that is passed to the constructor for the $@MarkingEffect@$ class.
	//# The $markingFlags$ member can currently be zero or the following value.
	//
	//# \table	MarkingFlags
	//
	//# If the $list$ parameter is not $nullptr$, then each $@MarkingEffect@$ node created during a call to the $@MarkingEffect::New@$
	//# function is added to the $@MarkingList@$ object (which can be a subclass of $MarkingList$).
	//
	//# If the $scale$ and $offset$ parameters are specified, then the $texcoordScale$ and $texcoordOffset$ members of
	//# the $MarkingData$ structure are initialized to those values. Otherwise, the scale is initialized to (1, 1), and the
	//# offset is initialized to (0, 0) so that the entire texture map is used by the marking.
	//
	//# \also	$@MarkingEffect@$
	
	
	//# \member		MarkingData
	
	struct MarkingData
	{
		unsigned_int32	markingFlags;			//## The marking effect flags. This is set to zero by the $MarkingData$ constructors.
		MarkingList		*markingList;			//## A pointer to a $@MarkingList@$ (or a subclass) to which marking effects are added.
		
		Point3D			center;					//## The world-space center of the surface marking.
		Vector3D		normal;					//## The world-space normal direction of the surface marking. This vector must have unit length.
		Vector3D		tangent;				//## The world-space tangent direction of the surface marking. (This determines texture orientation.) This vector does not have to be unit length.
		float			radius;					//## The radius of the surface marking.
		Range<float>	clip;					//## The range of depths, with respect to the center and normal direction, to which the marking is clipped if the $kMarkingClipRange$ flag is specified. If $kMarkingClipRange$ is not specified, then the range is [&minus;<i>r</i>,&nbsp;<i>r</i>], where <i>r</i> is the radius of the marking.
		
		Vector2D		texcoordScale;			//## The (<i>s</i>, <i>t</i>) scale to apply to the texture coordinates of the marking. This is set to (1, 1) by the default constructor.
		Vector2D		texcoordOffset;			//## The (<i>s</i>, <i>t</i>) offset to apply to the texture coordinates of the marking. This is set to (0, 0) by the default constructor.
		
		union
		{
			const char		*textureName;		//## A pointer to the name of the texture map used by the surface marking. This occupies the same space as $materialObject$ and should not be specified if the $kMarkingLight$ is set in the $markingFlags$ field.
			MaterialObject	*materialObject;	//## A pointer to the material object used by the surface marking. This occupies the same space as $textureName$ and should only be specified if the $kMarkingLight$ is set in the $markingFlags$ field.
		};
		
		ColorRGBA		color;					//## The marking color. Ignored if a material object containing a $DiffuseAttribute$ is specified.
		int32			lifeTime;				//## The time for which the surface marking is rendered. If this is set to -1, then the marking is rendered for exactly one frame.
		
		MarkingData(unsigned_int32 flags = 0, MarkingList *list = nullptr)
		{
			markingFlags = flags;
			markingList = list;
			
			texcoordScale.Set(1.0F, 1.0F);
			texcoordOffset.Set(0.0F, 0.0F);
		}
		
		MarkingData(const Vector2D& scale, const Vector2D& offset, unsigned_int32 flags = 0, MarkingList *list = nullptr)
		{
			markingFlags = flags;
			markingList = list;
			
			texcoordScale = scale;
			texcoordOffset = offset;
		}
	};
	
	
	//# \class	MarkingEffect		Represents a surface marking node in a world.
	//
	//# The $MarkingEffect$ class represents a surface marking node in a world.
	//
	//# \def	class MarkingEffect : public Effect, public ListElement<MarkingEffect>, public Memory<MarkingEffect>
	//
	//# \ctor	MarkingEffect(const Geometry *geometry, const MarkingData *data);
	//
	//# \param	geometry	The geometry to which the marking is to be applied.
	//# \param	data		A pointer to a $@MarkingData@$ data structure defining the marking's parameters.
	//
	//# \desc
	//# The $MarkingEffect$ class represents a surface marking node that is associated with a single geometry node.
	//#
	//# Surface markings are normally generated for all of the geometries intersecting a particular location by calling
	//# the $@MarkingEffect::New@$ function, but it is also possible to construct a $MarkingEffect$ directly. If the
	//# $MarkingEffect$ constructor is explicitly called (by using the $new$ operator), then the calling code should
	//# subsequently call the $@MarkingEffect::Nonempty@$ function to determine whether any triangles were generated.
	//# If the marking effect is not empty, then it should be added to the scene as a subnode of the geometry node
	//# specified by the $geometry$ parameter by calling the $@Node::AddNewSubnode@$ function. If the $MarkingEffect::Nonempty$
	//# function returns $false$, then the marking effect should simply be deleted.
	//
	//# \base	Effect									A $MarkingEffect$ node is a specific type of $Effect$.
	//# \base	Utilities/ListElement<MarkingEffect>	Each $MarkingEffect$ node belonging to a single surface marking can be stored in a $@MarkingList@$ object.
	//# \base	MemoryMgr/Memory<MarkingEffect>			Storage for marking effects is allocated in a dedicated heap for speed.
	//
	//# \also	$@MarkingData@$
	//# \also	$@MarkingList@$
	
	
	//# \function	MarkingEffect::New		Creates a set of marking effects at a particular location.
	//
	//# \proto	static void New(World *world, const MarkingData *data);
	//
	//# \param	world	The world in which the marking effects should be applied.
	//# \param	data	A pointer to a $@MarkingData@$ structure describing the marking.
	//
	//# \desc
	//# The $New$ function creates a set of marking effects at a particular location using the parameters specified in the
	//# $@MarkingData@$ structure pointed to by the $data$ parameter. The $New$ function finds all enabled geometry nodes that
	//# intersect the marking's bounds, excludes those having either the $kGeometryMarkingInhibit$ or $kGeometryInvisible$ flag set,
	//# creates a new marking effect for each geometry, and adds each nonempty marking effect to the world.
	//#
	//# If the $markingList$ field of the $@MarkingData@$ structure points to a $@MarkingList@$ object, then each nonempty
	//# marking effect created by the $New$ function is added to this list so that it's possible to track all pieces of the
	//# complete surface marking.
	//
	//# \also	$@MarkingData@$
	//# \also	$@MarkingList@$
	
	
	//# \function	MarkingEffect::Nonempty		Returns a boolean value indicating whether a surface marking contains any triangles.
	//
	//# \proto	bool Nonempty(void) const;
	//
	//# \desc
	//# The $Nonempty$ function returns a boolean value indicating whether a surface marking contains any triangles. If the return
	//# value is $true$, then at least one triangle was generated for the surface marking. Otherwise, the surface marking is empty,
	//# and it should be deleted. This function is intended to be used when a surface marking is constructed directly&mdash;it is
	//# not necessary when the $@MarkingEffect::New@$ function is called.
	
	
	class MarkingEffect : public Effect, public ListElement<MarkingEffect>, public Memory<MarkingEffect>
	{
		friend class Effect;
		
		private:
			
			enum
			{
				kMaxSmallMarkingVertexCount		= 64
			};
			
			struct ClippingData
			{
				int32			geometryVertexCount;
				int32			maxMarkingVertexCount;
				
				Antivector4D	leftPlane;
				Antivector4D	rightPlane;
				Antivector4D	bottomPlane;
				Antivector4D	topPlane;
				Antivector4D	frontPlane;
				Antivector4D	backPlane;
			};
			
			unsigned_int32			markingFlags;
			float					markingAlpha;
			
			float					markingDepthOffset;
			
			int32					markingLifeTime;
			int32					markingFadeTime;
			int32					markingKillTime;
			int32					markingInvisibleTime;
			
			Point3D					effectPosition;
			float					effectRadius;
			
			int32					markingVertexCount;
			int32					markingTriangleCount;
			
			MaterialObject			*materialObject;
			List<Attribute>			attributeList;
			DiffuseAttribute		diffuseColor;
			TextureMapAttribute		textureMap;
			
			char					*largeArrayStorage;
			Point3D					*vertexArray;
			Vector3D				*normalArray;
			ColorRGBA				*colorArray;
			Vector3D				*tangentArray;
			Point2D					*texcoordArray;
			Triangle				*triangleArray;
			
			Point3D					smallVertexArray[kMaxSmallMarkingVertexCount];
			Vector3D				smallNormalArray[kMaxSmallMarkingVertexCount];
			ColorRGBA				smallColorArray[kMaxSmallMarkingVertexCount];
			Vector3D				smallTangentArray[kMaxSmallMarkingVertexCount];
			Point2D					smallTexcoordArray[kMaxSmallMarkingVertexCount];
			Triangle				smallTriangleArray[kMaxSmallMarkingVertexCount * 3];
			
			MarkingEffect();
			
			static void MaterialObjectLinkProc(Object *object, void *cookie);
			
			bool CalculateBoundingBox(Box3D *box) const override;
			bool CalculateBoundingSphere(BoundingSphere *sphere) const override;
			
			static ProximityResult MarkGeometry(Node *node, const Point3D& center, float radius, void *cookie);
			
			void AllocateLargeArrays(int32 vertexCount, int32 triangleCount);
			bool AddPolygon(int32 vertexCount, const Point3D *vertex, const Vector3D *normal, ClippingData *clippingData);
			static int32 ClipPolygonAgainstPlane(const Antivector4D& plane, int32 vertexCount, const Point3D *vertex, const Vector3D *normal, Point3D *newVertex, Vector3D *newNormal);
		
		public:
			
			C4API MarkingEffect(const Geometry *geometry, const MarkingData *data);
			C4API ~MarkingEffect();
			
			bool Nonempty(void) const
			{
				return (markingVertexCount > 0);
			}
			
			const char *GetTextureName(void) const
			{
				return (textureMap.GetTextureName());
			}
			
			void Prepack(List<Object> *linkList) const;
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			void *BeginSettingsUnpack(void);
			
			void Preprocess(void);
			
			void Move(void);
			void Render(const Camera *camera, List<Renderable> *effectList);
			
			C4API static void New(const World *world, const MarkingData *data);
	};
	
	
	//# \class	MarkingList		Contains a list of marking effects.
	//
	//# The $MarkingList$ class contains a list of marking effects.
	//
	//# \def	class MarkingList : public List<MarkingEffect>
	//
	//# \ctor	MarkingList();
	//
	//# \desc
	//# The $MarkingList$ class is used to contain a list of the marking effects created during a single call to the
	//# $@MarkingEffect::New@$ function. When a new surface marking is created, it can be split over multiple geometries,
	//# and each part gets its own $@MarkingEffect@$ node. If a pointer to a $MarkingList$ object (or a subclass object)
	//# is specified in the $@MarkingData@$ structure, then all of the $MarkingEffect$ nodes are added to that list.
	//#
	//# When the last marking effect in the list is destroyed because its lifetime has expired, the $@MarkingList::HandleDestruction@$
	//# function is called. This function should be implemented by a custom subclass of the $MarkingList$ class to perform
	//# any action required when the marking completely disappears from the scene. The $HandleDestruction$ function is not
	//# called if no marking effects are created in the first place during a call to the $MarkingEffect::New$ function.
	//#
	//# Destroying a $MarkingList$ object effectively removes the entire surface marking from the scene.
	//
	//# \base	Utilities/List<MarkingEffect>		A $QuadEffectObject$ is an object that can be shared by multiple quad effect nodes.
	//
	//# \also	$@MarkingEffect@$
	//# \also	$@MarkingData@$
	
	
	//# \function	MarkingList::HandleDestruction		Called when all parts of a surface marking have been destroyed.
	//
	//# \proto	virtual void HandleDestruction(void);
	//
	//# \desc
	//# The $HandleDestruction$ function is called when the last marking effect contained in a $MarkingList$ object is destroyed
	//# because its lifetime has expired. This function should be implemented by a custom subclass of the $MarkingList$ class to
	//# perform any action required when the marking completely disappears from the scene. The $HandleDestruction$ function is not
	//# called if no marking effects are created in the first place during a call to the $@MarkingEffect::New@$ function.
	//
	//# \also	$@MarkingEffect@$
	//# \also	$@MarkingData@$
	
	
	class MarkingList : public List<MarkingEffect>
	{
		public:
			
			virtual void HandleDestruction(void);
	};
	
	
	//# \class	QuadEffectObject		Encapsulates data pertaining to a generic billboarded quad effect.
	//
	//# The $QuadEffectObject$ class encapsulates data pertaining to a generic billboarded quad effect.
	//
	//# \def	class QuadEffectObject : public EffectObject
	//
	//# \ctor	QuadEffectObject(float radius, const ColorRGBA& color, const char *textureName);
	//
	//# \param	radius			The radius of the beam.
	//# \param	color			The color of the beam.
	//# \param	textureName		The name of the quad texture.
	//
	//# \desc
	//#
	//
	//# \base	EffectObject		A $QuadEffectObject$ is an object that can be shared by multiple quad effect nodes.
	//
	//# \also	$@QuadEffect@$
	
	
	class QuadEffectObject : public EffectObject
	{
		friend class EffectObject;
		
		private:
			
			unsigned_int32		quadFlags;
			float				quadRadius;
			ColorRGBA			quadColor;
			unsigned_int32		quadBlendState;
			float				quadDeltaScale;
			ResourceName		quadTextureName;
			
			QuadEffectObject();
			~QuadEffectObject();
		
		public:
			
			QuadEffectObject(float radius, const ColorRGBA& color, const char *textureName);
			
			unsigned_int32 GetQuadFlags(void) const
			{
				return (quadFlags);
			}
			
			void SetQuadFlags(unsigned_int32 flags)
			{
				quadFlags = flags;
			}
			
			float GetQuadRadius(void) const
			{
				return (quadRadius);
			}
			
			void SetQuadRadius(float radius)
			{
				quadRadius = radius;
			}
			
			const ColorRGBA& GetQuadColor(void) const
			{
				return (quadColor);
			}
			
			void SetQuadColor(const ColorRGBA& color)
			{
				quadColor = color;
			}
			
			void SetQuadAlpha(float alpha)
			{
				quadColor.alpha = alpha;
			}
			
			unsigned_int32 GetQuadBlendState(void) const
			{
				return (quadBlendState);
			}
			
			void SetQuadBlendState(unsigned_int32 blendState)
			{
				quadBlendState = blendState;
			}
			
			float GetQuadSoftDepthScale(void) const
			{
				return (quadDeltaScale);
			}
			
			void SetQuadSoftDepthScale(float scale)
			{
				quadDeltaScale = scale;
			}
			
			const ResourceName& GetQuadTextureName(void) const
			{
				return (quadTextureName);
			}
			
			void SetQuadTextureName(const char *name)
			{
				quadTextureName = name;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetCategoryCount(void) const;
			Type GetCategoryType(int32 index, const char **title) const;
			int32 GetCategorySettingCount(Type category) const;
			Setting *GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const;
			void SetCategorySetting(Type category, const Setting *setting);
			
			int32 GetObjectSize(float *size) const;
			void SetObjectSize(const float *size);
	};
	
	
	//# \class	QuadEffect		Represents a generic billboarded quad effect node in a world.
	//
	//# The $QuadEffect$ class represents a generic billboarded quad effect node in a world.
	//
	//# \def	class QuadEffect : public Effect
	//
	//# \ctor	QuadEffect(float radius, const ColorRGBA& color, const char *textureName);
	//
	//# \param	radius			The radius of the beam.
	//# \param	color			The color of the beam.
	//# \param	textureName		The name of the quad texture.
	//
	//# \desc
	//#
	//
	//# \base	Effect		A quad effect is a specific type of effect.
	//
	//# \also	$@QuadEffectObject@$
	
	
	class QuadEffect : public Effect
	{
		friend class Effect;
		
		private:
			
			int32					quadOrientation;
			
			Point3D					quadVertex[4];
			Point2D					quadTexcoord[4];
			Vector2D				quadBillboard[4];
			
			List<Attribute>			attributeList;
			DiffuseAttribute		diffuseAttribute;
			TextureMapAttribute		textureMapAttribute;
			DeltaDepthAttribute		deltaDepthAttribute;
			
			QuadEffect();
			QuadEffect(const QuadEffect& quadEffect);
			
			Node *Replicate(void) const override;
			
			void CalculatePostTransform(void) override;
			bool CalculateBoundingBox(Box3D *box) const override;
			bool CalculateBoundingSphere(BoundingSphere *sphere) const override;
			
			static bool DirectionVisible(const Node *node, const Region *region);
			static bool DirectionOccluded(const Node *node, const Region *region);
		
		public:
			
			C4API QuadEffect(float radius, const ColorRGBA& color, const char *textureName);
			C4API ~QuadEffect();
			
			QuadEffectObject *GetObject(void) const
			{
				return (static_cast<QuadEffectObject *>(Node::GetObject()));
			}
			
			int32 GetQuadOrientation(void) const
			{
				return (quadOrientation);
			}
			
			void SetQuadOrientation(int32 orientation)
			{
				quadOrientation = orientation;
				QuadEffect::UpdateEffectGeometry();
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			void Preprocess(void);
			void ProcessObjectSettings(void);
			C4API void UpdateEffectGeometry(void);
			
			void Render(const Camera *camera, List<Renderable> *effectList);
	};
	
	
	//# \class	FlareEffectObject		Encapsulates data pertaining to a fractional-occlusion flare effect.
	//
	//# The $FlareEffectObject$ class encapsulates data pertaining to a fractional-occlusion flare effect.
	//
	//# \def	class FlareEffectObject : public EffectObject
	//
	//# \ctor	FlareEffectObject(float flare, float occlusion, float rotation, const char *textureName);
	//
	//# \param	flare			The radius of the flare.
	//# \param	occlusion		The radius of the occlusion query.
	//# \param	rotation		The occlusion rotation radius.
	//# \param	textureName		The name of the flare texture.
	//
	//# \desc
	//# 
	//# \base	EffectObject		A $FlareEffectObject$ is an object that can be shared by multiple flare effect nodes.
	//
	//# \also	$@FlareEffect@$
	
	
	//# \function	FlareEffectObject::GetFlareRadius		Returns the flare radius.
	//
	//# \proto	float GetFlareRadius(void) const;
	//
	//# \desc
	//
	//# \also	$@FlareEffectObject::SetFlareRadius@$
	//# \also	$@FlareEffectObject::GetOcclusionRadius@$
	//# \also	$@FlareEffectObject::SetOcclusionRadius@$
	//# \also	$@FlareEffectObject::GetRotationRadius@$
	//# \also	$@FlareEffectObject::SetRotationRadius@$
	
	
	//# \function	FlareEffectObject::SetFlareRadius		Sets the flare radius.
	//
	//# \proto	void SetFlareRadius(float radius);
	//
	//# \param	radius		The new flare radius.
	//
	//# \desc
	//
	//# \also	$@FlareEffectObject::GetFlareRadius@$
	//# \also	$@FlareEffectObject::GetOcclusionRadius@$
	//# \also	$@FlareEffectObject::SetOcclusionRadius@$
	//# \also	$@FlareEffectObject::GetRotationRadius@$
	//# \also	$@FlareEffectObject::SetRotationRadius@$
	
	
	//# \function	FlareEffectObject::GetOcclusionRadius		Returns the occlusion query radius.
	//
	//# \proto	float GetOcclusionRadius(void) const;
	//
	//# \desc
	//
	//# \also	$@FlareEffectObject::SetOcclusionRadius@$
	//# \also	$@FlareEffectObject::GetRotationRadius@$
	//# \also	$@FlareEffectObject::SetRotationRadius@$
	//# \also	$@FlareEffectObject::GetFlareRadius@$
	//# \also	$@FlareEffectObject::SetFlareRadius@$
	
	
	//# \function	FlareEffectObject::SetOcclusionRadius		Sets the occlusion query radius.
	//
	//# \proto	void SetOcclusionRadius(float radius);
	//
	//# \param	radius		The new occlusion query radius.
	//
	//# \desc
	//
	//# \also	$@FlareEffectObject::GetOcclusionRadius@$
	//# \also	$@FlareEffectObject::GetRotationRadius@$
	//# \also	$@FlareEffectObject::SetRotationRadius@$
	//# \also	$@FlareEffectObject::GetFlareRadius@$
	//# \also	$@FlareEffectObject::SetFlareRadius@$
	
	
	//# \function	FlareEffectObject::GetRotationRadius		Returns the occlusion rotation radius.
	//
	//# \proto	float GetRotationRadius(void) const;
	//
	//# \desc
	//
	//# \also	$@FlareEffectObject::SetRotationRadius@$
	//# \also	$@FlareEffectObject::GetOcclusionRadius@$
	//# \also	$@FlareEffectObject::SetOcclusionRadius@$
	//# \also	$@FlareEffectObject::GetFlareRadius@$
	//# \also	$@FlareEffectObject::SetFlareRadius@$
	
	
	//# \function	FlareEffectObject::SetRotationRadius		Sets the occlusion rotation radius.
	//
	//# \proto	void SetRotationRadius(float radius);
	//
	//# \param	radius		The new occlusion rotation radius.
	//
	//# \desc
	//
	//# \also	$@FlareEffectObject::GetRotationRadius@$
	//# \also	$@FlareEffectObject::GetOcclusionRadius@$
	//# \also	$@FlareEffectObject::SetOcclusionRadius@$
	//# \also	$@FlareEffectObject::GetFlareRadius@$
	//# \also	$@FlareEffectObject::SetFlareRadius@$
	
	
	//# \function	FlareEffectObject::GetFlareTextureName		Returns the name of the flare texture.
	//
	//# \proto	const ResourceName& GetFlareTextureName(void) const;
	//
	//# \desc
	//
	//# \also	$@FlareEffectObject::SetFlareTextureName@$
	
	
	//# \function	FlareEffectObject::SetFlareTextureName		Sets the name of the flare texture.
	//
	//# \proto	void SetFlareTextureName(const char *name);
	//
	//# \param	name	The name of the flare texture.
	//
	//# \desc
	//
	//# \also	$@FlareEffectObject::GetFlareTextureName@$
	
	
	class FlareEffectObject : public EffectObject
	{
		friend class EffectObject;
		
		private:
			
			float			flareRadius;
			float			occlusionRadius;
			float			rotationRadius;
			
			ColorRGB		flareColor;
			ResourceName	flareTextureName;
			
			FlareEffectObject();
			~FlareEffectObject();
		
		public:
			
			FlareEffectObject(float flare, float occlusion, float rotation, const char *textureName);
			
			float GetFlareRadius(void) const
			{
				return (flareRadius);
			}
			
			void SetFlareRadius(float radius)
			{
				flareRadius = radius;
			}
			
			float GetOcclusionRadius(void) const
			{
				return (occlusionRadius);
			}
			
			void SetOcclusionRadius(float radius)
			{
				occlusionRadius = radius;
			}
			
			float GetRotationRadius(void) const
			{
				return (rotationRadius);
			}
			
			void SetRotationRadius(float radius)
			{
				rotationRadius = radius;
			}
			
			const ColorRGB& GetFlareColor(void) const
			{
				return (flareColor);
			}
			
			void SetFlareColor(const ColorRGB& color)
			{
				flareColor = color;
			}
			
			const ResourceName& GetFlareTextureName(void) const
			{
				return (flareTextureName);
			}
			
			void SetFlareTextureName(const char *name)
			{
				flareTextureName = name;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetCategoryCount(void) const;
			Type GetCategoryType(int32 index, const char **title) const;
			int32 GetCategorySettingCount(Type category) const;
			Setting *GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const;
			void SetCategorySetting(Type category, const Setting *setting);
			
			int32 GetObjectSize(float *size) const;
			void SetObjectSize(const float *size);
	};
	
	
	//# \class	FlareEffect		Represents a fractional-occlusion flare effect node in a world.
	//
	//# The $FlareEffect$ class represents a fractional-occlusion flare effect node in a world.
	//
	//# \def	class FlareEffect : public Effect
	//
	//# \ctor	FlareEffect(float flare, float occlusion, float rotation, const char *textureName);
	//
	//# \param	flare			The radius of the flare.
	//# \param	occlusion		The radius of the occlusion query.
	//# \param	rotation		The occlusion rotation radius.
	//# \param	textureName		The name of the flare texture.
	//
	//# \desc
	//#
	//
	//# \base	Effect		A flare effect is a specific type of effect.
	//
	//# \also	$@FlareEffectObject@$
	
	
	class FlareEffect : public Effect
	{
		friend class Effect;
		
		private:
			
			float					inverseWidth;
			
			Point3D					flareVertex[4];
			Point2D					flareTexcoord[4];
			Point3D					occlusionVertex[4];
			
			List<Attribute>			attributeList;
			DiffuseAttribute		diffuseAttribute;
			TextureMapAttribute		textureMap;
			
			Renderable				occlusionRenderable;
			OcclusionQuery			occlusionQuery;
			
			FlareEffect();
			FlareEffect(const FlareEffect& flareEffect);
			
			Node *Replicate(void) const override;
			
			bool CalculateBoundingBox(Box3D *box) const override;
			bool CalculateBoundingSphere(BoundingSphere *sphere) const override;
			
			static void RenderFlare(OcclusionQuery *query, List<Renderable> *renderList, void *cookie);
		
		public:
			
			C4API FlareEffect(float flare, float occlusion, float rotation, const char *textureName);
			C4API ~FlareEffect();
			
			FlareEffectObject *GetObject(void) const
			{
				return (static_cast<FlareEffectObject *>(Node::GetObject()));
			}
			
			void Preprocess(void);
			void ProcessObjectSettings(void);
			
			void Render(const Camera *camera, List<Renderable> *effectList);
	};
	
	
	//# \class	BeamEffectObject		Encapsulates data pertaining to a polyboard beam effect.
	//
	//# The $BeamEffectObject$ class encapsulates data pertaining to a polyboard beam effect.
	//
	//# \def	class BeamEffectObject : public EffectObject
	//
	//# \ctor	BeamEffectObject(float radius, float height, const ColorRGBA& color, const char *textureName = nullptr);
	//
	//# \param	radius			The radius of the beam.
	//# \param	height			The height (or length) of the beam.
	//# \param	color			The color of the beam.
	//# \param	textureName		The name of the texture applied to the beam.
	//
	//# \desc
	//# 
	//# \base	EffectObject		A $BeamEffectObject$ is an object that can be shared by multiple beam effect nodes.
	//
	//# \also	$@BeamEffect@$
	
	
	class BeamEffectObject : public EffectObject
	{
		friend class EffectObject;
		
		private:
			
			float			beamRadius;
			float			beamHeight;
			
			ColorRGBA		beamColor;
			ResourceName	beamTextureName;
			float			texcoordScale;
			
			BeamEffectObject();
			~BeamEffectObject();
		
		public:
			
			BeamEffectObject(float radius, float height, const ColorRGBA& color, const char *textureName = nullptr);
			
			float GetBeamRadius(void) const
			{
				return (beamRadius);
			}
			
			void SetBeamRadius(float radius)
			{
				beamRadius = radius;
			}
			
			float GetBeamHeight(void) const
			{
				return (beamHeight);
			}
			
			void SetBeamHeight(float height)
			{
				beamHeight = height;
			}
			
			const ColorRGBA& GetBeamColor(void) const
			{
				return (beamColor);
			}
			
			void SetBeamColor(const ColorRGBA& color)
			{
				beamColor = color;
			}
			
			const ResourceName& GetBeamTextureName(void) const
			{
				return (beamTextureName);
			}
			
			void SetBeamTextureName(const char *name)
			{
				beamTextureName = name;
			}
			
			float GetTexcoordScale(void) const
			{
				return (texcoordScale);
			}
			
			void SetTexcoordScale(float scale)
			{
				texcoordScale = scale;
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
			
			int32 GetObjectSize(float *size) const;
			void SetObjectSize(const float *size);
	};
	
	
	//# \class	BeamEffect		Represents a polyboard beam effect node in a world.
	//
	//# The $BeamEffect$ class represents a polyboard beam effect node in a world.
	//
	//# \def	class BeamEffect : public Effect
	//
	//# \ctor	BeamEffect(float radius, float height, const ColorRGBA& color, const char *textureName = nullptr);
	//
	//# \param	radius			The radius of the beam.
	//# \param	height			The height (or length) of the beam.
	//# \param	color			The color of the beam.
	//# \param	textureName		The name of the texture applied to the beam.
	//
	//# \desc
	//#
	//
	//# \base	Effect		A beam effect is a specific type of effect.
	//
	//# \also	$@BeamEffectObject@$
	
	
	class BeamEffect : public Effect
	{
		friend class Effect;
		
		private:
			
			Point3D					vertexArray[4];
			Vector4D				tangentArray[4];
			Point2D					texcoordArray[4];
			
			List<Attribute>			attributeList;
			DiffuseAttribute		diffuseColor;
			TextureMapAttribute		textureMap;
			
			BeamEffect();
			BeamEffect(const BeamEffect& beamEffect);
			
			Node *Replicate(void) const override;
			
			bool CalculateBoundingBox(Box3D *box) const override;
			bool CalculateBoundingSphere(BoundingSphere *sphere) const override;
		
		public:
			
			C4API BeamEffect(float radius, float height, const ColorRGBA& color, const char *textureName = nullptr);
			C4API ~BeamEffect();
			
			BeamEffectObject *GetObject(void) const
			{
				return (static_cast<BeamEffectObject *>(Node::GetObject()));
			}
			
			void Preprocess(void);
			void ProcessObjectSettings(void);
			C4API void UpdateEffectGeometry(void);
	};
	
	
	//# \class	TubeEffectObject		Encapsulates data pertaining to a polyboard tube effect.
	//
	//# The $TubeEffectObject$ class encapsulates data pertaining to a polyboard tube effect.
	//
	//# \def	class TubeEffectObject : public EffectObject
	//
	//# \ctor	TubeEffectObject(const Path *path, float radius, const ColorRGBA& color, const char *textureName = nullptr);
	//
	//# \param	path			The path along which the tube is created.
	//# \param	radius			The radius of the tube.
	//# \param	color			The color of the tube.
	//# \param	textureName		The name of the texture applied to the tube.
	//
	//# \desc
	//# 
	//# \base	EffectObject		A $TubeEffectObject$ is an object that can be shared by multiple tube effect nodes.
	//
	//# \also	$@TubeEffect@$
	
	
	class TubeEffectObject : public EffectObject
	{
		friend class EffectObject;
		
		private:
			
			float			tubeRadius;
			
			Path			tubePath;
			Box3D			pathBoundingBox;
			
			ColorRGBA		tubeColor;
			ResourceName	tubeTextureName;
			float			texcoordScale;
			int32			maxSubdiv;
			
			int32			tubeVertexCount;
			char			*tubeStorage;
			Point3D			*tubeVertexArray;
			Vector4D		*tubeTangentArray;
			Point2D			*tubeTexcoordArray;
		
		protected:
			
			TubeEffectObject(EffectType type = kEffectTube);
			TubeEffectObject(EffectType type, const Path *path, float radius, const ColorRGBA& color, const char *textureName = nullptr);
			~TubeEffectObject();
			
			Point3D *GetVertexArray(void)
			{
				return (tubeVertexArray);
			}
			
			Vector4D *GetTangentArray(void)
			{
				return (tubeTangentArray);
			}
			
			Point2D *GetTexcoordArray(void)
			{
				return (tubeTexcoordArray);
			}
			
			void AllocateStorage(int32 vertexCount);
		
		public:
			
			TubeEffectObject(const Path *path, float radius, const ColorRGBA& color, const char *textureName = nullptr);
			
			float GetTubeRadius(void) const
			{
				return (tubeRadius);
			}
			
			void SetTubeRadius(float radius)
			{
				tubeRadius = radius;
			}
			
			Path *GetTubePath(void)
			{
				return (&tubePath);
			}
			
			const Path *GetTubePath(void) const
			{
				return (&tubePath);
			}
			
			const Box3D& GetPathBoundingBox(void) const
			{
				return (pathBoundingBox);
			}
			
			const ColorRGBA& GetTubeColor(void) const
			{
				return (tubeColor);
			}
			
			void SetTubeColor(const ColorRGBA& color)
			{
				tubeColor = color;
			}
			
			const ResourceName& GetTubeTextureName(void) const
			{
				return (tubeTextureName);
			}
			
			void SetTubeTextureName(const char *name)
			{
				tubeTextureName = name;
			}
			
			float GetTexcoordScale(void) const
			{
				return (texcoordScale);
			}
			
			void SetTexcoordScale(float scale)
			{
				texcoordScale = scale;
			}
			
			int32 GetMaxSubdiv(void) const
			{
				return (maxSubdiv);
			}
			
			void SetMaxSubdiv(int32 subdiv)
			{
				maxSubdiv = subdiv;
			}
			
			int32 GetVertexCount(void) const
			{
				return (tubeVertexCount);
			}
			
			const Point3D *GetVertexArray(void) const
			{
				return (tubeVertexArray);
			}
			
			const Vector4D *GetTangentArray(void) const
			{
				return (tubeTangentArray);
			}
			
			const Point2D *GetTexcoordArray(void) const
			{
				return (tubeTexcoordArray);
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
			
			int32 GetObjectSize(float *size) const;
			void SetObjectSize(const float *size);
			
			C4API void SetTubePath(const Path *path);
			C4API virtual void Build(void);
	};
	
	
	//# \class	TubeEffect		Represents a polyboard tube effect node in a world.
	//
	//# The $TubeEffect$ class represents a polyboard tube effect node in a world.
	//
	//# \def	class TubeEffect : public Effect
	//
	//# \ctor	TubeEffect(const Path *path, float radius, const ColorRGBA& color, const char *textureName = nullptr);
	//
	//# \param	path			The path along which the tube is created.
	//# \param	radius			The radius of the tube.
	//# \param	color			The color of the tube.
	//# \param	textureName		The name of the texture applied to the tube.
	//
	//# \desc
	//#
	//
	//# \base	Effect		A tube effect is a specific type of effect.
	//
	//# \also	$@TubeEffectObject@$
	
	
	class TubeEffect : public Effect
	{
		friend class Effect;
		
		private:
			
			List<Attribute>			attributeList;
			DiffuseAttribute		diffuseColor;
			TextureMapAttribute		textureMap;
			
			Node *Replicate(void) const override;
			
			#if C4LEGACY
			
				static void PathLinkProc(Node *node, void *cookie);
			
			#endif
			
			bool CalculateBoundingBox(Box3D *box) const override;
			bool CalculateBoundingSphere(BoundingSphere *sphere) const override;
		
		protected:
			
			TubeEffect(EffectType type = kEffectTube);
			TubeEffect(EffectType type, const Path *path, float radius, const ColorRGBA& color, const char *textureName = nullptr);
			TubeEffect(const TubeEffect& tubeEffect);
		
		public:
			
			C4API TubeEffect(const Path *path, float radius, const ColorRGBA& color, const char *textureName = nullptr);
			C4API ~TubeEffect();
			
			TubeEffectObject *GetObject(void) const
			{
				return (static_cast<TubeEffectObject *>(Node::GetObject()));
			}
			
			void SetTubeColor(const ColorRGBA& color)
			{
				diffuseColor.SetDiffuseColor(color);
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetInternalConnectorCount(void) const;
			const char *GetInternalConnectorKey(int32 index) const;
			bool ValidConnectedNode(const ConnectorKey& key, const Node *node) const;
			C4API PathMarker *GetConnectedPathMarker(void) const;
			C4API void SetConnectedPathMarker(PathMarker *marker);
			
			void Preprocess(void);
			void ProcessObjectSettings(void);
	};
	
	
	class BoltEffectObject : public TubeEffectObject
	{
		friend class EffectObject;
		
		private:
			
			float			maxPathDeviation;
			
			int32			branchingDepth;
			int32			branchCount;
			float			branchRadiusScale;
			Range<float>	branchLengthRange;
			
			BoltEffectObject();
			~BoltEffectObject();
			
			void BuildBranch(const Path *path, float radius, float deviation, int32 zdiv, Point3D *vertex, Vector4D *tangent, Point2D *texcoord);
		
		public:
			
			BoltEffectObject(const Path *path, float radius, float deviation, const ColorRGBA& color, const char *textureName = nullptr);
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetCategorySettingCount(Type category) const;
			Setting *GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const;
			void SetCategorySetting(Type category, const Setting *setting);
			
			C4API void Build(void);
	};
	
	
	class BoltEffect : public TubeEffect
	{
		friend class Effect;
		
		private:
			
			BoltEffect();
			BoltEffect(const BoltEffect& boltEffect);
			
			Node *Replicate(void) const override;
		
		public:
			
			C4API BoltEffect(const Path *path, float radius, float deviation, const ColorRGBA& color, const char *textureName = nullptr);
			C4API ~BoltEffect();
			
			BoltEffectObject *GetObject(void) const
			{
				return (static_cast<BoltEffectObject *>(Node::GetObject()));
			}
	};
	
	
	//# \class	FireEffectObject		Encapsulates data pertaining to a procedural fire effect.
	//
	//# The $FireEffectObject$ class encapsulates data pertaining to a procedural fire effect.
	//
	//# \def	class FireEffectObject : public EffectObject
	//
	//# \ctor	FireEffectObject(float radius, float height, float intensity, int32 speed, const char *textureName);
	//
	//# \param	radius			The radius of the fire.
	//# \param	height			The height of the fire.
	//# \param	intensity		The fire animation intensity.
	//# \param	speed			The fire animation speed.
	//# \param	textureName		The name of the fire texture.
	//
	//# \desc
	//# 
	//# \base	EffectObject		A $FireEffectObject$ is an object that can be shared by multiple fire effect nodes.
	//
	//# \also	$@FireEffect@$
	
	
	//# \function	FireEffectObject::GetFireRadius		Returns the fire radius.
	//
	//# \proto	float GetFireRadius(void) const;
	//
	//# \desc
	//
	//# \also	$@FireEffectObject::SetFireRadius@$
	//# \also	$@FireEffectObject::GetFireHeight@$
	//# \also	$@FireEffectObject::SetFireHeight@$
	
	
	//# \function	FireEffectObject::SetFireRadius		Sets the fire radius.
	//
	//# \proto	void SetFireRadius(float radius);
	//
	//# \param	radius		The new fire radius.
	//
	//# \desc
	//
	//# \also	$@FireEffectObject::GetFireRadius@$
	//# \also	$@FireEffectObject::GetFireHeight@$
	//# \also	$@FireEffectObject::SetFireHeight@$
	
	
	//# \function	FireEffectObject::GetFireHeight		Returns the fire height.
	//
	//# \proto	float GetFireHeight(void) const;
	//
	//# \desc
	//
	//# \also	$@FireEffectObject::SetFireHeight@$
	//# \also	$@FireEffectObject::GetFireRadius@$
	//# \also	$@FireEffectObject::SetFireRadius@$
	
	
	//# \function	FireEffectObject::SetFireHeight		Sets the fire height.
	//
	//# \proto	void SetFireHeight(float height);
	//
	//# \param	height		The new fire height.
	//
	//# \desc
	//
	//# \also	$@FireEffectObject::GetFireHeight@$
	//# \also	$@FireEffectObject::GetFireRadius@$
	//# \also	$@FireEffectObject::SetFireRadius@$
	
	
	//# \function	FireEffectObject::GetFireIntensity		Returns the fire animation intensity.
	//
	//# \proto	float GetFireIntensity(void) const;
	//
	//# \desc
	//
	//# \also	$@FireEffectObject::SetFireIntensity@$
	//# \also	$@FireEffectObject::GetFireSpeed@$
	//# \also	$@FireEffectObject::SetFireSpeed@$
	
	
	//# \function	FireEffectObject::SetFireIntensity		Sets the fire animation intensity.
	//
	//# \proto	void SetFireIntensity(float intensity);
	//
	//# \param	intensity	The new fire animation intensity.
	//
	//# \desc
	//
	//# \also	$@FireEffectObject::GetFireIntensity@$
	//# \also	$@FireEffectObject::GetFireSpeed@$
	//# \also	$@FireEffectObject::SetFireSpeed@$
	
	
	//# \function	FireEffectObject::GetFireSpeed		Returns the fire animation speed.
	//
	//# \proto	float GetFireSpeed(void) const;
	//
	//# \desc
	//
	//# \also	$@FireEffectObject::SetFireSpeed@$
	//# \also	$@FireEffectObject::GetFireIntensity@$
	//# \also	$@FireEffectObject::SetFireIntensity@$
	
	
	//# \function	FireEffectObject::SetFireSpeed		Sets the fire animation speed.
	//
	//# \proto	void SetFireSpeed(int32 speed);
	//
	//# \param	speed		The new fire animation speed. This must be a value between 0 and 24, inclusive.
	//
	//# \desc
	//
	//# \also	$@FireEffectObject::GetFireSpeed@$
	//# \also	$@FireEffectObject::GetFireIntensity@$
	//# \also	$@FireEffectObject::SetFireIntensity@$
	
	
	//# \function	FireEffectObject::GetFireTextureName		Returns the name of the fire texture.
	//
	//# \proto	const ResourceName& GetFireTextureName(void) const;
	//
	//# \desc
	//
	//# \also	$@FireEffectObject::SetFireTextureName@$
	
	
	//# \function	FireEffectObject::SetFireTextureName		Sets the name of the fire texture.
	//
	//# \proto	void SetFireTextureName(const char *name);
	//
	//# \param	name	The name of the fire texture.
	//
	//# \desc
	//
	//# \also	$@FireEffectObject::GetFireTextureName@$
	
	
	class FireEffectObject : public EffectObject
	{
		friend class EffectObject;
		
		private:
			
			float			fireRadius;
			float			fireHeight;
			
			float			fireIntensity;
			int32			fireSpeed;
			
			ResourceName	fireTextureName;
			
			FireEffectObject();
			~FireEffectObject();
		
		public:
			
			FireEffectObject(float radius, float height, float intensity, int32 speed, const char *textureName);
			
			float GetFireRadius(void) const
			{
				return (fireRadius);
			}
			
			void SetFireRadius(float radius)
			{
				fireRadius = radius;
			}
			
			float GetFireHeight(void) const
			{
				return (fireHeight);
			}
			
			void SetFireHeight(float height)
			{
				fireHeight = height;
			}
			
			float GetFireIntensity(void) const
			{
				return (fireIntensity);
			}
			
			void SetFireIntensity(float intensity)
			{
				fireIntensity = intensity;
			}
			
			int32 GetFireSpeed(void) const
			{
				return (fireSpeed);
			}
			
			void SetFireSpeed(int32 speed)
			{
				fireSpeed = speed;
			}
			
			const ResourceName& GetFireTextureName(void) const
			{
				return (fireTextureName);
			}
			
			void SetFireTextureName(const char *name)
			{
				fireTextureName = name;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetCategoryCount(void) const;
			Type GetCategoryType(int32 index, const char **title) const;
			int32 GetCategorySettingCount(Type category) const;
			Setting *GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const;
			void SetCategorySetting(Type category, const Setting *setting);
			
			int32 GetObjectSize(float *size) const;
			void SetObjectSize(const float *size);
	};
	
	
	//# \class	FireEffect		Represents a procedural fire effect node in a world.
	//
	//# The $FireEffect$ class represents a procedural fire effect node in a world.
	//
	//# \def	class FireEffect : public Effect
	//
	//# \ctor	FireEffect(float radius, float height, float intensity, int32 speed, const char *textureName);
	//
	//# \param	radius			The radius of the fire.
	//# \param	height			The height of the fire.
	//# \param	intensity		The fire animation intensity.
	//# \param	speed			The fire animation speed.
	//# \param	textureName		The name of the fire texture.
	//
	//# \desc
	//#
	//
	//# \base	Effect		A fire effect is a specific type of effect.
	//
	//# \also	$@FireEffectObject@$
	
	
	class FireEffect : public Effect
	{
		friend class Effect;
		
		private:
			
			Point3D					vertexArray[4];
			Vector4D				texcoordArray[4];
			
			List<Attribute>			attributeList;
			TextureMapAttribute		textureMap;
			FireAttribute			fireAttribute;
			
			FireEffect();
			FireEffect(const FireEffect& fireEffect);
			
			Node *Replicate(void) const override;
			
			bool CalculateBoundingBox(Box3D *box) const override;
			bool CalculateBoundingSphere(BoundingSphere *sphere) const override;
		
		public:
			
			C4API FireEffect(float radius, float height, float intensity, int32 speed, const char *textureName);
			C4API ~FireEffect();
			
			FireEffectObject *GetObject(void) const
			{
				return (static_cast<FireEffectObject *>(Node::GetObject()));
			}
			
			void Preprocess(void);
			void ProcessObjectSettings(void);
			
			void Render(const Camera *camera, List<Renderable> *effectList);
	};
}


#endif

// ZYURVUR
