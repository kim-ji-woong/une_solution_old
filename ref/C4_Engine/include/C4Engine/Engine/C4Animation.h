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


#ifndef C4Animation_h
#define C4Animation_h


//# \component	World Manager
//# \prefix		WorldMgr/


#include "C4Resources.h"
#include "C4Construction.h"
#include "C4Time.h"


namespace C4
{
	//# \tree	Animator
	//
	//# \node	MergeAnimator
	//# \node	BlendAnimator
	//# \node	FrameAnimator
	
	
	typedef Type	AnimatorType;
	typedef Type	TrackType;
	typedef Type	CueType;
	
	
	enum
	{
		kAnimatorMerge		= 'MERG',
		kAnimatorBlend		= 'BLND',
		kAnimatorFrame		= 'FRAM'
	};
	
	
	enum
	{
		kTrackTransform		= 'XFRM',
		kTrackCue			= 'CUE '
	};
	
	
	class Model;
	
	
	struct TransformFrameData
	{
		Matrix3D		transform;
		Point3D			position;
	};
	
	
	struct TransformTrackHeader
	{
		int32			frameCount;
		float			frameDuration;
		
		const TransformFrameData *GetFrameData(void) const
		{
			return (reinterpret_cast<const TransformFrameData *>(this + 1));
		}
	};
	
	
	struct CueData
	{
		CueType			cueType;
		float			cueTime;
	};
	
	
	struct CueTrackHeader
	{
		int32			cueCount;
		CueData			cueData[1];
	};
	
	
	struct AnimationHeader
	{
		struct TrackData
		{
			TrackType		trackType;
			unsigned_int32	trackOffset;
		};
		
		struct HashData
		{
			unsigned_int32	hashValue;
			int32			nodeIndex;
		};
		
		struct BucketData
		{
			unsigned_int16	bucketSize;
			unsigned_int16	bucketOffset;
		};
		
		struct HashTable
		{
			int32			bucketCount;
			BucketData		bucketData[1]; 
			
			const HashData *GetHashData(void) const 
			{ 
				return (reinterpret_cast<const HashData *>(bucketData + bucketCount)); 
			}
		}; 
		
		int32			nodeCount;
		unsigned_int32	hashTableOffset;
		 
		int32			trackCount;
		TrackData		trackData[1];
		
		const HashTable *GetNodeHashTable(void) const 
		{
			return (reinterpret_cast<const HashTable *>(reinterpret_cast<const char *>(this) + hashTableOffset));
		}
		
		const void *GetTrackHeader(int32 index) const
		{
			return (reinterpret_cast<const char *>(this) + trackData[index].trackOffset);
		}
	};
	
	
	class AnimationResource : public Resource<AnimationResource>
	{
		friend class Resource<AnimationResource>;
		
		private:
			
			static C4API ResourceDescriptor		descriptor;
			
			~AnimationResource();
			
			void Preprocess(void);
		
		public:
			
			C4API AnimationResource(const char *name, ResourceCatalog *catalog);
			
			int32 GetEndian(void) const
			{
				return (static_cast<const int32 *>(GetData())[0]);
			}
			
			const AnimationHeader *GetAnimationHeader(void) const
			{
				return (reinterpret_cast<const AnimationHeader *>(&reinterpret_cast<const int32 *>(GetData())[1]));
			}
	};
	
	
	struct AnimatorTransform
	{
		Quaternion		rotation;
		Point3D			position;
	};
	
	
	//# \class	Animator		The base class for all model animation classes.
	//
	//# The $Animator$ class is the base class for all model animation classes.
	//
	//# \def	class Animator : public UpdatableTree<Animator>, public Packable, public Constructable<Animator>
	//
	//# \ctor	Animator(AnimatorType type, Model *model, Node *node = nullptr);
	//
	//# \param	type		The animator type.
	//# \param	model		The model to which the animator applies.
	//# \param	node		The root of the subtree within the model that is affected by the animator. If this is $nullptr$, then the root is the same as the $model$ parameter.
	//
	//# \desc
	//#
	//
	//# \base	Utilities/UpdatableTree<Animator>	Used internally by the World Manager.
	//# \base	ResourceMgr/Packable				Animators can be packed for storage in resources.
	//# \base	System/Constructable<Animator>		New animator subclasses may be defined by an application, and a constructor
	//#												function can be installed using the $Constructable$ class.
	//
	//# \also	$@MergeAnimator@$
	//# \also	$@BlendAnimator@$
	//# \also	$@FrameAnimator@$
	
	
	//# \function	Animator::GetAnimatorType		Returns the animator type.
	//
	//# \proto	AnimatorType GetAnimatorType(void) const;
	//
	//# \desc
	//
	
	
	//# \function	Animator::GetTargetModel		Returns the model to which an animator applies.
	//
	//# \proto	Model *GetTargetModel(void) const;
	//
	//# \desc
	//
	//# \also	$@Animator::GetTargetNode@$
	//# \also	$@Animator::SetTargetNode@$
	
	
	//# \function	Animator::GetTargetNode			Returns the root of the subtree with the model that is affected by an animator.
	//
	//# \proto	Node *GetTargetNode(void) const;
	//
	//# \desc
	//
	//# \also	$@Animator::SetTargetNode@$
	//# \also	$@Animator::GetTargetModel@$
	
	
	//# \function	Animator::SetTargetNode			Sets the root of the subtree with the model that is affected by an animator.
	//
	//# \proto	void SetTargetNode(Node *node);
	//
	//# \param	node	The new target node. This must be in the subtree rooted at the model node to which the animator applies.
	//
	//# \desc
	//
	//# \also	$@Animator::GetTargetNode@$
	//# \also	$@Animator::GetTargetModel@$
	
	
	//# \function	Animator::GetWeightInterpolator		Returns the interpolator used to modify an animator's weight.
	//
	//# \proto	Interpolator *GetWeightInterpolator(void);
	//# \proto	const Interpolator *GetWeightInterpolator(void) const;
	//
	//# \desc
	//
	//# \also	$@TimeMgr/Interpolator@$
	
	
	//# \function	Animator::Preprocess		Called to allow an animator to perform any necessary preprocessing.
	//
	//# \proto	void Preprocess(void);
	//
	//# \desc
	//
	
	
	//# \function	Animator::Configure			Called when an animator is affected by a change in the animation tree for a model.
	//
	//# \proto	void Configure(void);
	//
	//# \desc
	//
	
	
	//# \function	Animator::Move				Called each frame to allow the animator to calculate its output.
	//
	//# \proto	void Move(void);
	//
	//# \desc
	//
	
	
	class Animator : public UpdatableTree<Animator>, public Packable, public Constructable<Animator>
	{
		public:
			
			enum
			{
				kUpdateOutput			= 1 << 0,
				
				kInitialUpdateFlags		= kUpdateOutput,
				kPropagatedUpdateFlags	= kUpdateOutput
			};
		
		private:
			
			AnimatorType				animatorType;
			
			Model						*targetModel;
			Node						*targetNode;
			
			int32						animatorNodeStart;
			int32						animatorNodeCount;
			
			unsigned_int32				storageSize;
			char						*outputStorage;
			
			AnimatorTransform			**outputTable;
			AnimatorTransform			*transformTable;
			void						*animatorData;
			
			Interpolator				weightInterpolator;
			
			static Animator *Construct(Unpacker& data, unsigned_int32 unpackFlags = 0);
		
		protected:
			
			C4API Animator(AnimatorType type);
			C4API Animator(AnimatorType type, Model *model, Node *node = nullptr);
			
			void *GetAnimatorData(void) const
			{
				return (animatorData);
			}
			
			C4API void AllocateStorage(int32 nodeStart, int32 nodeCount, int32 transformCount, unsigned_int32 dataSize = 0);
		
		public:
			
			C4API virtual ~Animator();
			
			AnimatorType GetAnimatorType(void) const
			{
				return (animatorType);
			}
			
			Model *GetTargetModel(void) const
			{
				return (targetModel);
			}
			
			Node *GetTargetNode(void) const
			{
				return (targetNode);
			}
			
			void SetTargetNode(Node *node)
			{
				targetNode = node;
				Invalidate();
			}
			
			int32 GetNodeStart(void) const
			{
				return (animatorNodeStart);
			}
			
			int32 GetNodeCount(void) const
			{
				return (animatorNodeCount);
			}
			
			AnimatorTransform **GetOutputTable(void) const
			{
				return (outputTable);
			}
			
			AnimatorTransform *GetTransformTable(void) const
			{
				return (transformTable);
			}
			
			Interpolator *GetWeightInterpolator(void)
			{
				return (&weightInterpolator);
			}
			
			const Interpolator *GetWeightInterpolator(void) const
			{
				return (&weightInterpolator);
			}
			
			void AddNewSubnode(Animator *animator)
			{
				AddSubnode(animator);
				animator->Preprocess();
			}
			
			C4API static Animator *New(AnimatorType type);
			
			C4API void PackType(Packer& data) const;
			C4API void Pack(Packer& data, unsigned_int32 packFlags) const;
			C4API void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			C4API void SetTargetModel(Model *model, Node *node = nullptr);
			C4API int32 GetNodeIndex(const Node *target) const;
			
			C4API void Update(void);
			C4API void Premove(void);
			
			C4API virtual void Preprocess(void);
			C4API virtual void Configure(void);
			C4API virtual void Move(void);
	};
	
	
	//# \class	MergeAnimator		An animator that merges outputs from its subnodes.
	//
	//# The $MergeAnimator$ class is an animator that merges outputs from its subnodes.
	//
	//# \def	class MergeAnimator : public Animator
	//
	//# \ctor	MergeAnimator(Model *model, Node *node = nullptr);
	//
	//# \param	model		The model to which the animator applies.
	//# \param	node		The root of the subtree within the model that is affected by the animator. If this is $nullptr$, then the root is the same as the $model$ parameter.
	//
	//# \desc
	//#
	//
	//# \base	Animator	The $MergeAnimator$ class is a special type of animator.
	//
	//# \also	$@BlendAnimator@$
	
	
	class MergeAnimator : public Animator
	{
		public:
			
			C4API MergeAnimator();
			C4API MergeAnimator(Model *model, Node *node = nullptr);
			C4API ~MergeAnimator();
			
			C4API void Configure(void);
	};
	
	
	//# \class	BlendAnimator		An animator that blends outputs from its subnodes.
	//
	//# The $BlendAnimator$ class is an animator that blends outputs from its subnodes.
	//
	//# \def	class BlendAnimator : public Animator
	//
	//# \ctor	BlendAnimator(Model *model, Node *node = nullptr);
	//
	//# \param	model		The model to which the animator applies.
	//# \param	node		The root of the subtree within the model that is affected by the animator. If this is $nullptr$, then the root is the same as the $model$ parameter.
	//
	//# \desc
	//#
	//
	//# \base	Animator	The $BlendAnimator$ class is a special type of animator.
	//
	//# \also	$@MergeAnimator@$
	
	
	class BlendAnimator : public Animator
	{
		private:
			
			int32		blendStart;
			int32		blendCount;
		
		public:
			
			C4API BlendAnimator();
			C4API BlendAnimator(Model *model, Node *node = nullptr);
			C4API ~BlendAnimator();
			
			C4API void Configure(void);
			C4API void Move(void);
	};
	
	
	//# \class	FrameAnimator		An animator that outputs transforms from frame data in an animation resource.
	//
	//# The $FrameAnimator$ class is an animator that outputs transforms from frame data in an animation resource.
	//
	//# \def	class FrameAnimator : public Animator, public ExclusiveObservable<FrameAnimator, CueType>
	//
	//# \ctor	FrameAnimator(Model *model, Node *node = nullptr);
	//
	//# \param	model		The model to which the animator applies.
	//# \param	node		The root of the subtree within the model that is affected by the animator. If this is $nullptr$, then the root is the same as the $model$ parameter.
	//
	//# \desc
	//#
	//
	//# \base	Animator												The $FrameAnimator$ class is a special type of animator.
	//# \base	Utilities/ExclusiveObservable<FrameAnimator, CueType>	A $FrameAnimator$ object can be observed for animation cue events.
	
	
	//# \function	FrameAnimator::SetAnimation		Sets the animation resource from which the animator generates its output.
	//
	//# \proto	void SetAnimation(const char *name);
	//
	//# \param	name	The name of the animation resource.
	//
	//# \desc
	//# The $SetAnimation$ function sets the current animation for a frame animator to the animation stored
	//# in the resource specified by the $name$ parameter. Animations are typically stored in a subfolder
	//# having the model resource's name, and the $name$ parameter should include this. For example, to
	//# apply an animation named "walk" to a model named "monster", the $name$ parameter should point
	//# to the string "monster/walk". (A forward slash should be used on all platforms.)
	
	
	//# \function	FrameAnimator::GetFrameInterpolator		Returns the interpolator used to play animation frames.
	//
	//# \proto	Interpolator *GetFrameInterpolator(void);
	//# \proto	const Interpolator *GetFrameInterpolator(void) const;
	//
	//# \desc
	//#
	//
	//# \also	$@TimeMgr/Interpolator@$
	
	
	class FrameAnimator : public Animator, public ExclusiveObservable<FrameAnimator, CueType>
	{
		friend class Animator;
		
		private:
			
			AnimationResource					*animationResource;
			const AnimationHeader				*animationHeader;
			const TransformTrackHeader			*transformTrackHeader;
			const CueTrackHeader				*cueTrackHeader;
			
			float								animationDuration;
			float								animationFrame;
			
			float								frameFrequency;
			Interpolator						frameInterpolator;
			
			void GenerateNodeRemapTable(void);
			void ExecuteAnimationFrame(float frame);
		
		public:
			
			C4API FrameAnimator();
			C4API FrameAnimator(Model *model, Node *node = nullptr);
			C4API ~FrameAnimator();
			
			const AnimationHeader *GetAnimationHeader(void) const
			{
				return (animationHeader);
			}
			
			const TransformTrackHeader *GetTransformTrackHeader(void) const
			{
				return (transformTrackHeader);
			}
			
			const CueTrackHeader *GetCueTrackHeader(void) const
			{
				return (cueTrackHeader);
			}
			
			float GetAnimationDuration(void) const
			{
				return (animationDuration);
			}
			
			float GetFrameFrequency(void) const
			{
				return (frameFrequency);
			}
			
			Interpolator *GetFrameInterpolator(void)
			{
				return (&frameInterpolator);
			}
			
			const Interpolator *GetFrameInterpolator(void) const
			{
				return (&frameInterpolator);
			}
			
			C4API void Pack(Packer& data, unsigned_int32 packFlags) const;
			C4API void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			C4API void Configure(void);
			C4API void Move(void);
			
			C4API void SetAnimationHeader(const AnimationHeader *header);
			C4API void SetAnimation(const char *name);
	};
	
	
	template <class observerType> class FrameAnimatorObserver : public ExclusiveObserver<observerType, FrameAnimator>
	{
		public:
			
			FrameAnimatorObserver(observerType *observer, void (observerType::*callback)(FrameAnimator *, CueType)) : ExclusiveObserver<observerType, FrameAnimator>(observer, callback)
			{
			}
	};
}


#endif

// ZYURVUR
