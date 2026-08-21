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


#ifndef C4Mutators_h
#define C4Mutators_h


//# \component	Interface Manager
//# \prefix		InterfaceMgr/


#include "C4Construction.h"


namespace C4
{
	enum
	{
		kMaxMutatorKeyLength	= 15
	};
	
	
	typedef Type	MutatorType;
	typedef Type	WidgetColorType;
	
	
	typedef String<kMaxMutatorKeyLength>	MutatorKey;
	
	
	enum
	{
		kMutatorPulsate			= 'PULS',
		kMutatorRandomize		= 'RAND',
		kMutatorScroll			= 'SCRL',
		kMutatorRotate			= 'ROTR',
		kMutatorScale			= 'SCAL',
		kMutatorFade			= 'FADE',
		kMutatorTicker			= 'TICK',
		kMutatorAnimate			= 'ANIM'
	};
	
	
	//# \enum	MutatorState
	
	enum
	{
		kMutatorDisabled		= 1 << 0,		//## The mutator is disabled and will not be updated.
		kMutatorReverse			= 1 << 1,		//## The mutator should run in reverse (if supported).
		kMutatorTerminated		= 1 << 2		//## The mutator has finished running.
	};
	
	
	enum
	{
		kPulsateWaveSquare,
		kPulsateWaveTriangle,
		kPulsateWaveSine,
		kPulsateWaveCount
	};
	
	
	enum
	{
		kPulsateBlendInterpolate,
		kPulsateBlendAdd,
		kPulsateBlendMultiply,
		kPulsateBlendCount
	};
	
	
	enum
	{
		kScaleMutatorScaleTexcoords		= 1 << 0,
		kScaleMutatorFlipHorizontal		= 1 << 1,
		kScaleMutatorFlipVertical		= 1 << 2,
		kScaleMutatorRestartable		= 1 << 3
	};
	
	
	enum
	{
		kFadeMutatorFinishHide			= 1 << 0,
		kFadeMutatorRestartable			= 1 << 1
	};
	
	
	enum
	{
		kTickerMutatorLooping			= 1 << 0,
		kTickerMutatorRestartable		= 1 << 1
	};
	
	
	enum
	{
		kAnimateMutatorLooping			= 1 << 0,
		kAnimateMutatorRestartable		= 1 << 1
	};
	
	
	class Mutator;
	class Widget;
	
	
	//# \class	MutatorRegistration		Manages internal registration information for a custom mutator type. 
	//
	//# The $MutatorRegistration$ class manages internal registration information for a custom mutator type. 
	// 
	//# \def	class MutatorRegistration : public Registration<Mutator, MutatorRegistration> 
	//
	//# \ctor	MutatorRegistration(MutatorType type, const char *name); 
	//
	//# \param	type		The mutator type.
	//# \param	name		The mutator name.
	// 
	//# \desc
	//# The $MutatorRegistration$ class is abstract and serves as the common base class for the template class
	//# $@MutatorReg@$. A custom mutator is registered with the engine by instantiating an object of type
	//# $MutatorReg<classType>$, where $classType$ is the type of the mutator subclass being registered. 
	//
	//# \base	System/Registration<Mutator, MutatorRegistration>		A mutator registration is a specific type of registration object.
	//
	//# \also	$@MutatorReg@$
	//# \also	$@Mutator@$
	
	
	//# \function	MutatorRegistration::GetMutatorType		Returns the registered mutator type.
	//
	//# \proto	MutatorType GetMutatorType(void) const;
	//
	//# \desc
	//# The $GetMutatorType$ function returns the mutator type for a particular mutator registration.
	//# The mutator type is established when the mutator registration is constructed.
	//
	//# \also	$@MutatorRegistration::GetMutatorName@$
	
	
	//# \function	MutatorRegistration::GetMutatorName		Returns the human-readable mutator name.
	//
	//# \proto	const char *GetMutatorName(void) const;
	//
	//# \desc
	//# The $GetMutatorName$ function returns the human-readable mutator name for a particular mutator registration.
	//# The mutator name is established when the mutator registration is constructed.
	//
	//# \also	$@MutatorRegistration::GetMutatorType@$
	
	
	class C4_API MutatorRegistration : public Registration<Mutator, MutatorRegistration>
	{
		private:
			
			const char		*mutatorName;
		
		public:
			
			MutatorRegistration(MutatorType type, const char *name);
			~MutatorRegistration();
			
			MutatorType GetMutatorType(void) const
			{
				return (GetRegistrableType());
			}
			
			const char *GetMutatorName(void) const
			{
				return (mutatorName);
			}
			
			virtual bool ValidWidget(const Widget *widget) const = 0;
	};
	
	
	//# \class	MutatorReg	 Represents a custom mutator type.
	//
	//# The $MutatorReg$ class represents a custom mutator type.
	//
	//# \def	template <class classType> class MutatorReg : public MutatorRegistration
	//
	//# \tparam	classType	The custom mutator class.
	//
	//# \ctor	MutatorReg(MutatorType type, const char *name);
	//
	//# \param	type		The mutator type.
	//# \param	name		The mutator name.
	//
	//# \desc
	//# The $MutatorReg$ template class is used to advertise the existence of a custom mutator type.
	//# The Interface Manager uses a mutator registration to construct a custom mutator. The act of instantiating a
	//# $MutatorReg$ object automatically registers the corresponding mutator type. The mutator type is unregistered
	//# when the $MutatorReg$ object is destroyed.
	//# 
	//# No more than one mutator registration should be created for each distinct mutator type.
	//
	//# \base	MutatorRegistration		All specific mutator registration classes share the common base class $MutatorRegistration$.
	//
	//# \also	$@Mutator@$
	
	
	template <class classType> class MutatorReg : public MutatorRegistration
	{
		public:
			
			MutatorReg(MutatorType type, const char *name) : MutatorRegistration(type, name)
			{
			}
			
			Mutator *Construct(void) const
			{
				return (new classType);
			}
			
			bool ValidWidget(const Widget *widget) const
			{
				return ((GetMutatorName()) && (classType::ValidWidget(widget)));
			}
	};
	
	
	//# \class	Mutator		Used to modify a widget in some way over time.
	//
	//# The $Mutator$ class is used to modify a widget in some way over time.
	//
	//# \def	class Mutator : public ListElement<Mutator>, public Packable, public Configurable, public Registrable<Mutator, MutatorRegistration>
	//
	//# \ctor	Mutator(MutatorType type);
	//
	//# \param	type		The mutator type.
	//
	//# \desc
	//# 
	//
	//# \base	Utilities/ListElement<Mutator>						Used internally by the Interface Manager.
	//# \base	ResourceMgr/Packable								Mutators can be packed for storage in resources.
	//# \base	InterfaceMgr/Configurable							Mutators can define configurable parameters that are exposed
	//#																as user interface widgets in the Panel Editor.
	//# \base	System/Registrable<Mutator, MutatorRegistration>	Custom mutator types can be registered with the engine.
	//
	//# \also	$@MutatorReg@$
	//# \also	$@Widget@$
	//# \also	$@EffectMgr/PanelEffect@$
	//# \also	$@EffectMgr/PanelController@$
	
	
	//# \function	Mutator::GetMutatorType		Returns the mutator type.
	//
	//# \proto	MutatorType GetMutatorType(void) const;
	//
	//# \desc
	//# The $GetMutatorType$ function returns the mutator type.
	
	
	//# \function	Mutator::GetMutatorState	Returns the mutator state.
	//
	//# \proto	unsigned_int32 GetMutatorState(void) const;
	//
	//# \desc
	//# The $GetMutatorState$ function returns the mutator flags, which can be a combination (through
	//# logical OR) of the following values.
	//
	//# \table	MutatorState
	//
	//# \also	$@Mutator::SetMutatorState@$
	
	
	//# \function	Mutator::SetMutatorState	Sets the mutator state.
	//
	//# \proto	void SetMutatorState(unsigned_int32 state);
	//
	//# \param	state	The new mutator state.
	//
	//# \desc
	//# The $SetMutatorState$ function sets the mutator state to the value specified by the $state$ parameter,
	//# which can be a combination (through logical OR) of the following values.
	//
	//# \table	MutatorState
	//
	//# \also	$@Mutator::GetMutatorState@$
	
	
	//# \function	Mutator::GetMutatorKey		Returns the mutator key.
	//
	//# \proto	const MutatorKey& GetMutatorKey(void) const;
	//
	//# \desc
	//# The $GetMutatorKey$ function returns the mutator key. The mutator key is a string having up to 15
	//# single-byte characters that can be used to identify one or more mutators in a panel. The initial
	//# key for a mutator is the empty string.
	//
	//# \also	$@Mutator::SetMutatorKey@$
	
	
	//# \function	Mutator::SetMutatorKey		Sets the mutator key.
	//
	//# \proto	void SetMutatorKey(const char *key);
	//
	//# \param	key		The new mutator key. This is a string up to 15 bytes in length, not counting the null terminator.
	//
	//# \desc
	//# The $SetMutatorKey$ function sets the mutator key to the string specified by the $key$ parameter.
	//# The mutator key is a string having up to 15 single-byte characters that can be used to identify
	//# one or more mutators in a panel. The initial key for a mutator is the empty string.
	//
	//# \also	$@Mutator::GetMutatorKey@$
	
	
	class C4_API Mutator : public ListElement<Mutator>, public Packable, public Configurable, public Registrable<Mutator, MutatorRegistration>
	{
		friend class Widget;
		
		private:
			
			MutatorType		mutatorType;
			unsigned_int32	mutatorState;
			MutatorKey		mutatorKey;
			
			Widget			*targetWidget;
			
			virtual Mutator *Replicate(void) const = 0;
		
		protected:
			
			Mutator(MutatorType type);
			Mutator(const Mutator& mutator);
		
		public:
			
			virtual ~Mutator();
			
			MutatorType GetMutatorType(void) const
			{
				return (mutatorType);
			}
			
			unsigned_int32 GetMutatorState(void) const
			{
				return (mutatorState);
			}
			
			const MutatorKey& GetMutatorKey(void) const
			{
				return (mutatorKey);
			}
			
			void SetMutatorKey(const char *key)
			{
				mutatorKey = key;
			}
			
			Widget *GetTargetWidget(void) const
			{
				return (targetWidget);
			}
			
			Mutator *Clone(void) const
			{
				return (Replicate());
			}
			
			static Mutator *New(MutatorType type);
			static bool ValidWidget(const Widget *widget);
			static void RegisterStandardMutators(void);
			
			void PackType(Packer& data) const;
			 void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			void *BeginSettingsUnpack(void);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			virtual void SetMutatorState(unsigned_int32 state);
			
			virtual void Preprocess(void);
			virtual void Move(void);
			virtual void Reset(void);
	};
	
	
	class C4_API PulsateMutator : public Mutator
	{
		friend class MutatorReg<PulsateMutator>;
		
		private:
			
			ColorRGBA			pulsateColor;
			WidgetColorType		colorType;
			
			int32				pulsateWave;
			int32				pulsateBlend;
			
			float				pulsateFrequency;
			float				pulsatePhaseShift;
			float				pulsateState;
			
			static float (*const pulsateWaveFunc[kPulsateWaveCount])(float);
			static ColorRGBA (*const pulsateBlendFunc[kPulsateBlendCount])(const ColorRGBA&, const ColorRGBA&, float);
			
			PulsateMutator();
			PulsateMutator(const PulsateMutator& pulsateMutator);
			
			Mutator *Replicate(void) const override;
			
			static float SquareWaveFunc(float t);
			static float TriangleWaveFunc(float t);
			static float SineWaveFunc(float t);
			
			static ColorRGBA InterpolateBlendFunc(const ColorRGBA& c1, const ColorRGBA& c2, float t);
			static ColorRGBA AddBlendFunc(const ColorRGBA& c1, const ColorRGBA& c2, float t);
			static ColorRGBA MultiplyBlendFunc(const ColorRGBA& c1, const ColorRGBA& c2, float t);
		
		public:
			
			PulsateMutator(const ColorRGBA& color, int32 wave, float frequency, float phaseShift = 0.0F);
			~PulsateMutator();
			
			const ColorRGBA& GetPulsateColor(void) const
			{
				return (pulsateColor);
			}
			
			void SetPulsateColor(const ColorRGBA& color)
			{
				pulsateColor = color;
			}
			
			WidgetColorType GetColorType(void) const
			{
				return (colorType);
			}
			
			void SetColorType(WidgetColorType type)
			{
				colorType = type;
			}
			
			int32 GetPulsateWave(void) const
			{
				return (pulsateWave);
			}
			
			void SetPulsateWave(int32 wave)
			{
				pulsateWave = Min(MaxZero(wave), kPulsateWaveCount - 1);
			}
			
			int32 GetPulsateBlend(void) const
			{
				return (pulsateBlend);
			}
			
			void SetPulsateBlend(int32 blend)
			{
				pulsateBlend = Min(MaxZero(blend), kPulsateBlendCount - 1);
			}
			
			float GetPulsateFrequency(void) const
			{
				return (pulsateFrequency);
			}
			
			void SetPulsateFrequency(float frequency)
			{
				pulsateFrequency = frequency;
			}
			
			float GetPulsatePhaseShift(void) const
			{
				return (pulsatePhaseShift);
			}
			
			void SetPulsatePhaseShift(float phaseShift)
			{
				pulsatePhaseShift = phaseShift;
			}
			
			float GetPulsateState(void) const
			{
				return (pulsateState);
			}
			
			void SetPulsateState(float state)
			{
				pulsateState = state;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			void Move(void);
			void Reset(void);
	};
	
	
	class C4_API RandomizeMutator : public Mutator
	{
		private:
			
			RandomizeMutator(const RandomizeMutator& randomizeMutator);
			
			Mutator *Replicate(void) const override;
		
		public:
			
			RandomizeMutator();
			~RandomizeMutator();
			
			static bool ValidWidget(const Widget *widget);
			
			void Move(void);
	};
	
	
	class C4_API ScrollMutator : public Mutator
	{
		friend class MutatorReg<ScrollMutator>;
		
		private:
			
			Vector2D	scrollSpeed;
			Vector2D	scrollOffset;
			
			ScrollMutator();
			ScrollMutator(const ScrollMutator& scrollMutator);
			
			Mutator *Replicate(void) const override;
		
		public:
			
			ScrollMutator(const Vector2D& speed);
			~ScrollMutator();
			
			const Vector2D& GetScrollSpeed(void) const
			{
				return (scrollSpeed);
			}
			
			void SetScrollSpeed(const Vector2D& speed)
			{
				scrollSpeed = speed;
			}
			
			const Vector2D& GetScrollOffset(void) const
			{
				return (scrollOffset);
			}
			
			void SetScrollOffset(const Vector2D& offset)
			{
				scrollOffset = offset;
			}
			
			static bool ValidWidget(const Widget *widget);
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			void Move(void);
			void Reset(void);
	};
	
	
	class C4_API RotateMutator : public Mutator
	{
		friend class MutatorReg<RotateMutator>;
		
		private:
			
			float		rotationSpeed;
			float		rotationAngle;
			
			RotateMutator();
			RotateMutator(const RotateMutator& rotateMutator);
			
			Mutator *Replicate(void) const override;
		
		public:
			
			RotateMutator(float speed);
			~RotateMutator();
			
			float GetRotationSpeed(void) const
			{
				return (rotationSpeed);
			}
			
			void SetRotationSpeed(float speed)
			{
				rotationSpeed = speed;
			}
			
			float GetRotationAngle(void) const
			{
				return (rotationAngle);
			}
			
			void SetRotationAngle(float angle)
			{
				rotationAngle = angle;
			}
			
			static bool ValidWidget(const Widget *widget);
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			void Move(void);
			void Reset(void);
	};
	
	
	class C4_API ScaleMutator : public Mutator
	{
		friend class MutatorReg<ScaleMutator>;
		
		private:
			
			unsigned_int32	scaleMutatorFlags;
			
			Vector2D		startScale;
			Vector2D		finishScale;
			
			float			scaleTime;
			float			currentTime;
			
			ScaleMutator();
			ScaleMutator(const ScaleMutator& scaleMutator);
			
			Mutator *Replicate(void) const override;
			
			float GetInitialTime(void) const;
			void UpdateScale(float time) const;
		
		public:
			
			ScaleMutator(const Vector2D& start, const Vector2D& finish, float time);
			~ScaleMutator();
			
			unsigned_int32 GetScaleMutatorFlags(void) const
			{
				return (scaleMutatorFlags);
			}
			
			void SetScaleMutatorFlags(unsigned_int32 flags)
			{
				scaleMutatorFlags = flags;
			}
			
			const Vector2D& GetStartScale(void) const
			{
				return (startScale);
			}
			
			void SetStartScale(const Vector2D& scale)
			{
				startScale = scale;
			}
			
			const Vector2D& GetFinishScale(void) const
			{
				return (finishScale);
			}
			
			void SetFinishScale(const Vector2D& scale)
			{
				finishScale = scale;
			}
			
			float GetScaleTime(void) const
			{
				return (scaleTime);
			}
			
			void SetScaleTime(float time)
			{
				scaleTime = time;
			}
			
			float GetCurrentTime(void) const
			{
				return (currentTime);
			}
			
			void SetCurrentTime(float time)
			{
				currentTime = time;
			}
			
			static bool ValidWidget(const Widget *widget);
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			void SetMutatorState(unsigned_int32 state);
			
			void Preprocess(void);
			void Move(void);
			void Reset(void);
	};
	
	
	class C4_API FadeMutator : public Mutator
	{
		friend class MutatorReg<FadeMutator>;
		
		private:
			
			unsigned_int32		fadeMutatorFlags;
			ColorRGBA			fadeColor;
			WidgetColorType		colorType;
			
			float				fadeTime;
			float				currentTime;
			
			FadeMutator();
			FadeMutator(const FadeMutator& fadeMutator);
			
			Mutator *Replicate(void) const override;
			
			float GetInitialTime(void) const;
			void UpdateColor(float time) const;
		
		public:
			
			FadeMutator(const ColorRGBA& color, float time);
			~FadeMutator();
			
			unsigned_int32 GetFadeMutatorFlags(void) const
			{
				return (fadeMutatorFlags);
			}
			
			void SetFadeMutatorFlags(unsigned_int32 flags)
			{
				fadeMutatorFlags = flags;
			}
			
			const ColorRGBA& GetFadeColor(void) const
			{
				return (fadeColor);
			}
			
			void SetFadeColor(const ColorRGBA& color)
			{
				fadeColor = color;
			}
			
			WidgetColorType GetColorType(void) const
			{
				return (colorType);
			}
			
			void SetColorType(WidgetColorType type)
			{
				colorType = type;
			}
			
			float GetFadeTime(void) const
			{
				return (fadeTime);
			}
			
			void SetFadeTime(float time)
			{
				fadeTime = time;
			}
			
			float GetCurrentTime(void) const
			{
				return (currentTime);
			}
			
			void SetCurrentTime(float time)
			{
				currentTime = time;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			void SetMutatorState(unsigned_int32 state);
			
			void Preprocess(void);
			void Move(void);
			void Reset(void);
	};
	
	
	class C4_API TickerMutator : public Mutator
	{
		friend class MutatorReg<TickerMutator>;
		
		private:
			
			unsigned_int32	tickerMutatorFlags;
			
			float			startPosition;
			float			finishPosition;
			
			float			scrollSpeed;
			float			currentPosition;
			
			TickerMutator();
			TickerMutator(const TickerMutator& tickerMutator);
			
			Mutator *Replicate(void) const override;
			
			float GetInitialPosition(void) const;
			void UpdatePosition(float position) const;
		
		public:
			
			TickerMutator(float start, float finish, float time);
			~TickerMutator();
			
			unsigned_int32 GetTickerMutatorFlags(void) const
			{
				return (tickerMutatorFlags);
			}
			
			void SetTickerMutatorFlags(unsigned_int32 flags)
			{
				tickerMutatorFlags = flags;
			}
			
			float GetStartPosition(void) const
			{
				return (startPosition);
			}
			
			void SetStartPosition(float position)
			{
				startPosition = position;
			}
			
			float GetFinishPosition(void) const
			{
				return (finishPosition);
			}
			
			void SetFinishPosition(float position)
			{
				finishPosition = position;
			}
			
			float GetScrollSpeed(void) const
			{
				return (scrollSpeed);
			}
			
			void SetScrollSpeed(float speed)
			{
				scrollSpeed = speed;
			}
			
			float GetCurrentPosition(void) const
			{
				return (currentPosition);
			}
			
			void SetCurrentPosition(float position)
			{
				currentPosition = position;
			}
			
			static bool ValidWidget(const Widget *widget);
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			void SetMutatorState(unsigned_int32 state);
			
			void Preprocess(void);
			void Move(void);
			void Reset(void);
	};
	
	
	class C4_API AnimateMutator : public Mutator
	{
		friend class MutatorReg<AnimateMutator>;
		
		private:
			
			unsigned_int32	animateMutatorFlags;
			
			int32			frameCountX;
			int32			frameCountY;
			int32			totalFrameCount;
			
			float			frameRate;
			float			currentFrame;
			
			AnimateMutator();
			AnimateMutator(const AnimateMutator& animateMutator);
			
			Mutator *Replicate(void) const override;
			
			float GetInitialFrame(void) const;
			void UpdateFrame(float frame) const;
		
		public:
			
			AnimateMutator(int32 x, int32 y, int32 total, float rate);
			~AnimateMutator();
			
			unsigned_int32 GetAnimateMutatorFlags(void) const
			{
				return (animateMutatorFlags);
			}
			
			void SetAnimateMutatorFlags(unsigned_int32 flags)
			{
				animateMutatorFlags = flags;
			}
			
			int32 GetFrameCountX(void) const
			{
				return (frameCountX);
			}
			
			int32 GetFrameCountY(void) const
			{
				return (frameCountY);
			}
			
			int32 GetTotalFrameCount(void) const
			{
				return (totalFrameCount);
			}
			
			void SetFrameCount(int32 x, int32 y, int32 total)
			{
				frameCountX = x;
				frameCountY = y;
				totalFrameCount = total;
			}
			
			float GetFrameRate(void) const
			{
				return (frameRate);
			}
			
			void SetFrameRate(float rate)
			{
				frameRate = rate;
			}
			
			float GetCurrentFrame(void) const
			{
				return (currentFrame);
			}
			
			void SetCurrentFrame(float frame)
			{
				currentFrame = frame;
			}
			
			static bool ValidWidget(const Widget *widget);
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			void SetMutatorState(unsigned_int32 state);
			
			void Preprocess(void);
			void Move(void);
			void Reset(void);
	};
}


#endif

// ZYURVUR
