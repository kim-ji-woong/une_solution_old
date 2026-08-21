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


#ifndef C4ExtraEffects_h
#define C4ExtraEffects_h


//# \component	Extras Plugin
//# \prefix		ExtrasPlugin/


#include "C4ExtrasBase.h"
#include "C4Effects.h"


namespace C4
{
	enum
	{
		kEffectShockwave		= 'SHCK',
		kEffectShell			= 'SHEL'
	};
	
	
	//# \class	ShockwaveEffect		Represents a shockwave effect node in a world.
	//
	//# The $ShockwaveEffect$ class represents a shockwave effect node in a world.
	//
	//# \def	class ShockwaveEffect : public Effect
	//
	//# \ctor	ShockwaveEffect(const char *textureName, float radius, float width, float speed);
	//
	//# \param	textureName		The name of the distortion texture to use for the effect.
	//# \param	radius			The maximum radius to which the effect will grow in size.
	//# \param	width			The width of the shockwave ring, the difference between the outer radius and inner radius.
	//# \param	speed			The speed at which the shockwave radius increases, in units per millisecond.
	//
	//# \desc
	//# The $ShockwaveEffect$ class produces the effect of an expanding shockwave ring that is rendered into
	//# the distortion buffer. The shockwave begins with a radius of zero and expands at the rate specified by
	//# the $speed$ parameter until the radius exceeds the maximum size given by the $radius$ parameter.
	//# When the maximum radius is reached, the shockwave effect is automatically destroyed.
	//#
	//# The texture map specified by the $textureName$ parameter should contain a distortion pattern in the red and
	//# green channels. The shockwave effect uses the center pixel of the texture for the entire inner edge of the
	//# expanding ring, and the boundary of an inscribed circle in the texture image corresponds to the outer edge
	//# of the ring.
	//
	//# \base	EffectMgr/Effect		A shockwave effect is a specific type of effect.
	
	
	class C4EXTRASAPI ShockwaveEffect : public Effect
	{
		friend class EffectReg<ShockwaveEffect>;
		
		private:
			
			float						shockRadius;
			float						maxShockRadius;
			float						shockWidth;
			float						shockSpeed;
			
			List<Attribute>				attributeList;
			TextureMapAttribute			textureMap;
			
			Point2D						shockVertex[130];
			static const ConstPoint2D	shockTexcoord[130];
			
			ShockwaveEffect();
			ShockwaveEffect(const ShockwaveEffect& shockwaveEffect);
			
			Node *Replicate(void) const override;
			
			bool CalculateBoundingSphere(BoundingSphere *sphere) const override;
		
		public:
			
			ShockwaveEffect(const char *textureName, float radius, float width, float speed);
			~ShockwaveEffect();
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			void Preprocess(void);
			
			void Move(void);
			void Render(const Camera *camera, List<Renderable> *effectList);
	};
	
	
	//# \class	ShellEffect		Represents a shell effect node in a world.
	//
	//# The $ShellEffect$ class represents a shell effect node in a world.
	//
	//# \def	class ShellEffect : public Effect
	//
	//# \ctor	ShellEffect(Geometry *geometry, float size, const ColorRGBA& color);
	//
	//# \param	geometry	The geometry for which the shell effect is created.
	//# \param	size		The distance by which the shell is extruded from the geometry.
	//# \param	color		The color of the shell effect.
	//
	//# \desc
	//# 
	//
	//# \base	EffectMgr/Effect	A shell effect is a specific type of effect. 
	 
	 
	class C4EXTRASAPI ShellEffect : public Effect
	{ 
		friend class EffectReg<ShellEffect>;
		
		private:
			 
			Geometry			*shellGeometry;
			
			Vector4D			scaleVector;
			 
			List<Attribute>		attributeList;
			DiffuseAttribute	diffuseAttribute;
			
			ShellEffect();
			ShellEffect(const ShellEffect& shellEffect);
			
			Node *Replicate(void) const override;
			
			void CalculatePostBounding(void) override;
		
		public:
			
			ShellEffect(Geometry *geometry, float size, const ColorRGBA& color);
			~ShellEffect();
			
			const Geometry *GetShellGeometry(void) const
			{
				return (shellGeometry);
			}
			
			float GetShellSize(void) const
			{
				return (scaleVector.x);
			}
			
			void SetShellSize(float size)
			{
				scaleVector.x = size;
			}
			
			const ColorRGBA& GetShellColor(void) const
			{
				return (diffuseAttribute.GetDiffuseColor());
			}
			
			void SetShellColor(const ColorRGBA& color)
			{
				diffuseAttribute.SetDiffuseColor(color);
			}
			
			void Preprocess(void);
			
			void Render(const Camera *camera, List<Renderable> *effectList);
	};
}


#endif

// ZYURVUR
