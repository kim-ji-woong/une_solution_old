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


#ifndef C4Models_h
#define C4Models_h


//# \component	World Manager
//# \prefix		WorldMgr/


#include "C4Geometries.h"
#include "C4Controller.h"
#include "C4Animation.h"
#include "C4Markers.h"


namespace C4
{
	typedef Type	ModelType;
	
	
	enum
	{
		kModelUnknown		= 0,
		kModelGeneric		= 1
	};
	
	
	//# \enum	ModelRegistrationFlags
	
	enum
	{
		kModelPrecache		= 1 << 0,		//## Precache the model resource.
		kModelPrivate		= 1 << 1		//## Do not display the model type in the World Editor.
	};
	
	
	enum
	{
		kControllerSkin			= 'SKIN',
		kControllerAnimation	= 'ANIM'
	};
	
	
	enum
	{
		kFunctionPlayAnimation	= 'PLAY',
		kFunctionStopAnimation	= 'STOP'
	};
	
	
	class Model;
	
	
	class C4_API ModelResource : public Resource<ModelResource>
	{
		friend class Resource<ModelResource>;
		
		private:
			
			static ResourceDescriptor		descriptor;
			
			~ModelResource();
		
		public:
			
			ModelResource(const char *name, ResourceCatalog *catalog);
	};
	
	
	//# \class	Bone	Represents a skeletal component used by a skinnable mesh.
	//
	//# The $Bone$ class represents a skeletal component used by a skinnable mesh.
	//
	//# \def	class Bone : public Node
	//
	//# \ctor	Bone(unsigned_int32 hash = 0);
	//
	//# \param	hash	The hash value for the name of the bone node.
	//
	//# \desc
	//# The $Bone$ class represents a single bone in a skeleton used by a skinnable mesh. An entire skeleton
	//# is typically composed of many bones arranged in a transform hierarchy.
	//
	//# \base	Node		A $Bone$ node is a scene graph node.
	//
	//# \also	$@Model@$
	
	
	//# \function	Bone::GetModelTransform		Returns the transform from bone space to model space.
	//
	//# \proto	const Transform4D& GetModelTransform(void) const;
	//
	//# \desc
	//# The $GetModelTransform$ function returns the transform that maps points in the object space of the bone to
	//# the object space of the model to which the bone belongs. This transform is valid only after the bone has been
	//# preprocessed and updated. The model node itself can be retrieved by calling the $@Bone::GetSkeletonRoot@$ function.
	//
	//# \also	$@Bone::GetSkeletonRoot@$
	
	
	//# \function	Bone::GetSkeletonRoot		Returns the root node of the model to which a bone belongs.
	//
	//# \proto	Node *GetSkeletonRoot(void) const; 
	//
	//# \desc 
	//# The $GetSkeletonRoot$ function returns a pointer to the model node to which a bone belongs. The return value is 
	//# only valid after the bone has been preprocessed. If there is no model node above the bone in the transform hierarchy, 
	//# then the return value is $nullptr$.
	// 
	//# \also	$@Bone::GetModelTransform@$
	
	
	class C4_API Bone : public Node 
	{
		private:
			
			Node			*skeletonRoot; 
			
			Transform4D		modelTransform;
			Box3D			boundingBox;
			
			Bone(const Bone& bone);
			
			Node *Replicate(void) const override;
			
			void CalculatePostTransform(void) override;
			bool CalculateBoundingBox(Box3D *box) const override;
			bool CalculateBoundingSphere(BoundingSphere *sphere) const override;
		
		public:
			
			Bone();
			~Bone();
			
			Node *GetSkeletonRoot(void) const
			{
				return (skeletonRoot);
			}
			
			const Transform4D& GetModelTransform(void) const
			{
				return (modelTransform);
			}
			
			const Box3D& GetBoundingBox(void) const
			{
				return (boundingBox);
			}
			
			void SetBoundingBox(const Box3D& box)
			{
				boundingBox = box;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			void Preprocess(void);
	};
	
	
	//# \class	SkinController		Controls a skinnable mesh.
	//
	//# The $SkinController$ class controls a skinnable mesh.
	//
	//# \def	class SkinController : public Controller
	//
	//# \ctor	SkinController();
	//
	//# \desc
	//# The $SkinController$ class represents the controller that is assigned to skinnable meshes. It is responsible
	//# for calculating new vertex positions whenever the skeleton to which the mesh is attached moves.
	//
	//# \base	Controller/Controller		A $SkinController$ is a specific type of controller.
	//
	//# \also	$@Bone@$
	//# \also	$@Model@$
	
	
	class C4_API SkinController : public Controller
	{
		private:
			
			struct DynamicVertex
			{
				Point3D		vertex;
				Point3D		previous;
				Vector3D	normal;
				Vector4D	tangent;
			};
			
			char				*skinStorage;
			
			int32				skinBoneCount;
			Bone				**skinBoneTable;
			Transform4D			*transformTable;
			
			ArrayBundle			skinVertexArray[2];
			ArrayBundle			skinNormalArray;
			ArrayBundle			skinTangentArray;
			ArrayBundle			skinPlaneArray;
			
			Box3D				skinBoundingBox;
			
			unsigned_int8		vertexParity;
			bool				motionBlurFlag;
			bool				handednessFlag;
			
			BatchJob			skinUpdateJob;

			VertexBuffer							dynamicVertexBuffer;
			VertexBufferObserver<SkinController>	dynamicVertexBufferObserver;

			SkinController(const SkinController& skinController);
			
			Controller *Replicate(void) const override;
			
			void CalculateBoneBoundingBoxes(void) const;
			
			static void CalculatePostTransform(MeshGeometry *meshGeometry);
			
			static void SkinUpdateJob(Job *job, void *cookie);
			static void FinalizeUpdate(Job *job, void *cookie);
			
			void FillDynamicVertexBuffer(VertexBuffer *vertexBuffer);
		
		public:
			
			SkinController();
			~SkinController();
			
			MeshGeometry *GetTargetNode(void) const
			{
				return (static_cast<MeshGeometry *>(Controller::GetTargetNode()));
			}
			
			static bool ValidNode(const Node *node);
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			void Preprocess(void);
			void Neutralize(void);
			void StopMotion(void);
			void Update(void);
			
			void SetDetailLevel(int32 level);
	};
	
	
	//# \class	ModelRegistration		Contains information about an application-defined model type.
	//
	//# The $ModelRegistration$ class contains information about an application-defined model type.
	//
	//# \def	class ModelRegistration : public MapElement<ModelRegistration>
	//
	//# \ctor	ModelRegistration(ModelType type, const char *name, const char *path, unsigned_int32 flags = 0, ControllerType contType = 0);
	//
	//# \param	type		The model type.
	//# \param	name		The model name.
	//# \param	path		The resource name for the model.
	//# \param	flags		Flags pertaining to the model type.
	//# \param	contType	The model's default controller type.
	//
	//# \desc
	//# The $ModelRegistration$ class is used to register an application-defined model type so that
	//# instances of the model can easily be cloned and so that they can be placed in a world using
	//# the World Editor. The act of instantiating an $ModelRegistration$ object automatically registers
	//# the corresponding model type. The model type is unregistered when the $ModelRegistration$ object
	//# is destroyed.
	//# 
	//# Each model type must have a unique 32-bit identifier which is usually a four-character code.
	//# This identifier is specified in the $type$ parameter in the model registration, and is later
	//# passed to the $@Model::Get@$ function to create instances of the specific type of model.
	//# 
	//# The $name$ parameter specifies the human-readable model name that is displayed in the World Editor.
	//# If the $kModelPrivate$ flag is specified in the $flags$ parameter, then the $name$ parameter may
	//# be $nullptr$.
	//# 
	//# The $path$ parameter specifies the name of the model resource corresponding to the model type.
	//# 
	//# The $flags$ parameter is optional and assigns special properties to the model registration.
	//# It can be a combination (through logical OR) of the following values.
	//
	//# \table	ModelRegistrationFlags
	//
	//# If the $kModelPrivate$ flag is specified, then the model type cannot be placed in a world using
	//# the World Editor, but the $@Model::Get@$ function still produces instances of the model.
	//# 
	//# The $contType$ parameter is optional and identifies the type of controller that should be
	//# automatically assigned to a model of the registration's type when it is placed in a world using
	//# the World Editor. The type of controller specified does not need to be registered using the
	//# $@Controller/ControllerRegistration@$ class, but the controller won't be accessible in the World
	//# Editor if it's not registered. The default value of 0 means that no controller is assigned.
	//
	//# \base	Utilities/MapElement<ModelRegistration>		Used internally by the World Manager.
	//
	//# \also	$@Model@$
	
	
	//# \function	ModelRegistration::GetModelType		Returns the registered model type.
	//
	//# \proto	ModelType GetModelType(void) const;
	//
	//# \desc
	//# The $GetKey$ function returns the unique 32-bit identifier associated with a model type.
	//
	//# \also	$@ModelRegistration::GetModelName@$
	
	
	//# \function	ModelRegistration::GetModelFlags		Returns the model registration flags.
	//
	//# \proto	unsigned_int32 GetModelFlags(void) const;
	//
	//# \desc
	//# The $GetModelFlags$ function returns the flags that were assigned to the model type
	//# when the model registration was created. The flags can be a combination (through logical
	//# OR) of the following values.
	//
	//# \table	ModelRegistrationFlags
	//
	//# \also	$@ModelRegistration::GetModelType@$
	//# \also	$@ModelRegistration::GetModelName@$
	
	
	//# \function	ModelRegistration::GetModelName		Returns the model name.
	//
	//# \proto	const char *GetModelName(void) const;
	//
	//# \desc
	//# The $GetModelName$ function returns the human-readable model name for a particular model registration.
	//# The model name is established when the model registration is constructed.
	//
	//# \also	$@ModelRegistration::GetResourceName@$
	//# \also	$@ModelRegistration::GetModelType@$
	
	
	//# \function	ModelRegistration::GetResourceName		Returns the name of the model resource.
	//
	//# \proto	const char *GetResourceName(void) const;
	//
	//# \desc
	//# The $GetResourceName$ function returns the resource name corresponding to a model's data.
	//# The resource name is established when the model registration is constructed.
	//
	//# \also	$@ModelRegistration::GetModelName@$
	//# \also	$@ModelRegistration::GetModelType@$
	
	
	class C4_API ModelRegistration : public Registration<Model, ModelRegistration>
	{
		private:
			
			unsigned_int32		modelFlags;
			
			const char			*modelName;
			const char			*resourceName;
			
			ControllerType		controllerType;
			
			Model				*prototypeModel;
			List<Model>			cloneList;
			
			void LoadPrototype(void);
		
		public:
			
			ModelRegistration(ModelType type, const char *name, const char *rsrcName, unsigned_int32 flags = 0, ControllerType contType = 0);
			~ModelRegistration();
			
			ModelType GetModelType(void) const
			{
				return (GetRegistrableType());
			}
			
			unsigned_int32 GetModelFlags(void) const
			{
				return (modelFlags);
			}
			
			const char *GetModelName(void) const
			{
				return (modelName);
			}
			
			const char *GetResourceName(void) const
			{
				return (resourceName);
			}
			
			ControllerType GetControllerType(void) const
			{
				return (controllerType);
			}
			
			Model *Construct(void) const;
			
			void Reload(void);
			
			Model *Clone(Model *model = nullptr);
			void Retire(Model *model);
	};
	
	
	//# \class	Model		The base class for all animatable models.
	//
	//# The $Model$ class is the base class for all animatable models.
	//
	//# \def	class Model : public Node, public ListElement<Model>, public Registrable<Model, ModelRegistration>
	//
	//# \ctor	Model(ModelType type = kModelUnknown);
	//
	//# \param	type	The model type.
	//
	//# \desc
	//# The $Model$ class serves as the base class for all animatable model nodes. A $Model$ instance
	//# is not normally constructed directly, but is created by calling the $@Model::Get@$ function or
	//# by constructing an instance of the $@GenericModel@$ class. The $@Model::Get@$ function should
	//# be used to create instances of models whose type has been registered with the $@ModelRegistration@$
	//# class. A $@GenericModel@$ instance should be constructed to explicitly create a model without
	//# a registered type.
	//
	//# \base	Node											A $Model$ node is a scene graph node.
	//# \base	Utilities/ListElement<Model>					Used internally by the World Manager.
	//# \base	System/Registrable<Model, ModelRegistration>	Custom model types can be registered with the engine.
	//
	//# \also	$@ModelRegistration@$
	//# \also	$@GenericModel@$
	//# \also	$@Bone@$
	
	
	//# \function	Model::Get		Returns a new instances of a particular type of model.
	//
	//# \proto	static Model *Get(ModelType type);
	//
	//# \desc
	//# The $Get$ function is used to create new instances of the type of model specified by the
	//# $type$ parameter. This function will always clone an existing model of the same type if
	//# possible, and it will load the model's model resource if no instances have been created
	//# yet and the model has not been precached.
	//# 
	//# In order to create new model instances with this function, the model type corresponding
	//# to the value of the $type$ parameter must have previously been registered through the
	//# instantiation of a $@ModelRegistration@$ object. If there is no registration matching the
	//# $type$ parameter, then the return value is $nullptr$.
	//#
	//# If the model type corresponding to the value of the $type$ parameter has been registered,
	//# but the model resource named in the registration does not exist, then a generic model is
	//# loaded, and a pointer to a clone of it is returned by the $Get$ function.
	//
	//# \also	$@ModelRegistration@$
	
	
	//# \function	Model::GetModelType		Returns the model type.
	//
	//# \proto	ModelType GetModelType(void) const;
	//
	//# \desc
	//# The $GetModelType$ function returns the type of a model node. If the model was created
	//# using the $@Model::Get@$ function, then the returned type is the same value as the type
	//# used to create the model. If the model was created as a $@GenericModel@$ node, then the
	//# returned type is $kModelGeneric$.
	//
	//# \also	$@Model::Get@$
	//# \also	$@GenericModel@$
	
	
	//# \function	Model::GetRootAnimator		Returns the root animator assigned to a model.
	//
	//# \proto	Animator *GetRootAnimator(void) const;
	//
	//# \desc
	//
	//# \also	$@Model::SetRootAnimator@$
	//# \also	$@Model::Animate@$
	//# \also	$@Animator@$
	
	
	//# \function	Model::SetRootAnimator		Sets the root animator assigned to a model.
	//
	//# \proto	void SetRootAnimator(Animator *animator);
	//
	//# \param	animator	The new root animator.
	//
	//# \desc
	//
	//# \also	$@Model::GetRootAnimator@$
	//# \also	$@Model::Animate@$
	//# \also	$@Animator@$
	
	
	//# \function	Model::FindNode		Finds a node having a specific name.
	//
	//# \proto	Node *FindNode(const char *name) const;
	//
	//# \param	name	The name of the node to search for. This is case sensitive.
	//
	//# \desc
	//# When a model is first preprocessed, it creates a table of pointers to its subnodes for
	//# quick access. The $FindNode$ function searches this table for a node having the name
	//# specified by the $name$ parameter. If the node is found, then a pointer to it is returned.
	//# Otherwise, the return value is $nullptr$. If more than one node has the specified name,
	//# then a pointer to one of the nodes will be returned, but which one is undefined.
	//
	//# \also	$@Node::GetNodeName@$
	//# \also	$@Node::SetNodeName@$
	
	
	//# \function	Model::Animate			Runs the animators assigned to a model.
	//
	//# \proto	void Animate(void);
	//
	//# \desc
	//
	//# \also	$@Model::GetRootAnimator@$
	//# \also	$@Model::SetRootAnimator@$
	//# \also	$@Animator@$
	
	
	class C4_API Model : public Node, public ListElement<Model>, public Registrable<Model, ModelRegistration>
	{
		friend class Node;
		
		private:
			
			enum
			{
				kModelLoaded	= 1 << 0
			};
			
			enum
			{
				kModelHashBucketCount = 32
			};
			
			struct HashBucket
			{
				unsigned_int16		count;
				unsigned_int16		start;
			};
			
			ModelType		modelType;
			unsigned_int32	modelState;
			
			Animator		*rootAnimator;
			
			Node			**modelHashTable;
			Node			**animatedNodeTable;
			
			int32			animatedNodeCount;
			HashBucket		hashBucket[kModelHashBucketCount];
			
			Node *Replicate(void) const override;
			
			static Model *Construct(Unpacker& data, unsigned_int32 unpackFlags);
			
			void ExecuteAnimationFrame(float frame);
		
		protected:
			
			Model(const Model& model);
		
		public:
			
			Model(ModelType type = kModelUnknown);
			virtual ~Model();
			
			ModelType GetModelType(void) const
			{
				return (modelType);
			}
			
			Animator *GetRootAnimator(void) const
			{
				return (rootAnimator);
			}
			
			Node *const *GetAnimatedNodeTable(void) const
			{
				return (animatedNodeTable);
			}
			
			int32 GetAnimatedNodeCount(void) const
			{
				return (animatedNodeCount);
			}
			
			Node *FindNode(const char *name) const
			{
				return (FindNode(Text::GetTextHash(name)));
			}
			
			void PackType(Packer& data) const;
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			void Preprocess(void);
			void Neutralize(void);
			
			Node *FindNode(unsigned_int32 hash) const;
			int32 GetAnimationIndex(const Node *node) const;
			
			void SetRootAnimator(Animator *animator);
			void Animate(void);
			
			virtual void Load(World *world);
			virtual void Unload(void);
			
			static Model *New(const char *name, ModelType type = kModelUnknown, unsigned_int32 unpackFlags = 0);
			static Model *Get(ModelType type);
	};
	
	
	//# \class	GenericModel	Represents a generic model node in a world.
	//
	//# The $GenericModel$ class represents a generic model node in a world.
	//
	//# \def	class GenericModel : public Model, public ListElement<GenericModel>
	//
	//# \ctor	GenericModel(const char *name);
	//
	//# \param	name	The name of the model resource.
	//
	//# \desc
	//# The $GenericModel$ class serves as the root node for a generic model in a world.
	//# An instance of $GenericModel$ should be constructed to explicitly create a model that
	//# does not have a type that was previously registered through the $@ModelRegistration@$ class.
	//#
	//# When a $GenericModel$ node is created, the model resource specified by the $name$
	//# parameter is loaded as a subtree of the $GenericModel$ node. If another $GenericModel$ node
	//# already exists for the same name, then the resource is not reloaded, but a clone of the existing
	//# model is created to share the same object data.
	//#
	//# If no model resource matching the $name$ parameter can be found, then no subnodes are created
	//# beneath the $GenericModel$ node.
	//
	//# \base	Model									A $GenericModel$ node is a specific type of model.
	//# \base	Utilities/ListElement<GenericModel>		Used internally by the World Manager.
	//
	//# \also	$@ModelRegistration@$
	//# \also	$@Model::Get@$
	
	
	//# \function	GenericModel::GetModelName		Returns the model resource name.
	//
	//# \proto	const ResourceName& GetModelName(void) const;
	//
	//# \desc
	//# The $GetModelName$ function returns the name of the model resource used to create the generic model node.
	//
	//# \also	$@Model::GetModelType@$
	
	
	class C4_API GenericModel : public Model, public ListElement<GenericModel>
	{
		friend class Model;
		
		private:
			
			ResourceName		modelName;
			
			GenericModel();
			GenericModel(const GenericModel& genericModel);
			
			Node *Replicate(void) const override;
		
		public:
			
			GenericModel(const char *name);
			~GenericModel();
			
			const ResourceName& GetModelName(void) const
			{
				return (modelName);
			}
			
			void SetModelName(const char *name)
			{
				modelName = name;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetCategoryCount(void) const;
			Type GetCategoryType(int32 index, const char **title) const;
			int32 GetCategorySettingCount(Type category) const;
			Setting *GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const;
			void SetCategorySetting(Type category, const Setting *setting);
			
			void Load(World *world);
			void Unload(void);
	};
	
	
	//# \class	AnimationController		Plays animations for a model node.
	//
	//# The $AnimationController$ class plays animations for a model node.
	//
	//# \def	class AnimationController : public Controller
	//
	//# \ctor	AnimationController();
	//
	//# \desc
	//# 
	//
	//# \base	Controller/Controller		A $AnimationController$ is a specific type of controller.
	//
	//# \also	$@Model@$
	
	
	class C4_API AnimationController : public Controller
	{
		private:
			
			unsigned_int32		animationMode;
			ResourceName		animationName;
			
			FrameAnimator		*frameAnimator;
			
			AnimationController(const AnimationController& animationController);
			
			Controller *Replicate(void) const override;
		
		public:
			
			enum
			{
				kAnimationMessageState
			};
			
			AnimationController();
			~AnimationController();
			
			Model *GetTargetNode(void) const
			{
				return (static_cast<Model *>(Controller::GetTargetNode()));
			}
			
			FrameAnimator *GetFrameAnimator(void) const
			{
				return (frameAnimator);
			}
			
			static void RegisterFunctions(ControllerRegistration *registration);
			static bool ValidNode(const Node *node);
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			void *BeginSettingsUnpack(void);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			ControllerMessage *ConstructMessage(ControllerMessageType type) const;
			void ReceiveMessage(const ControllerMessage *message);
			void SendInitialStateMessages(Player *player) const;
			
			void Preprocess(void);
			void Move(void);
			
			void PlayAnimation(const char *name, unsigned_int32 mode);
			void StopAnimation(void);
	};
	
	
	class C4_API AnimationStateMessage : public ControllerMessage
	{
		friend class AnimationController;
		
		private:
			
			unsigned_int32		animationMode;
			float				animatorValue;
			ResourceName		animationName;
			
			AnimationStateMessage(int32 controllerIndex);
		
		public:
			
			AnimationStateMessage(int32 controllerIndex, const char *name, float value, unsigned_int32 mode);
			~AnimationStateMessage();
			
			unsigned_int32 GetAnimationMode(void) const
			{
				return (animationMode);
			}
			
			float GetAnimatorValue(void) const
			{
				return (animatorValue);
			}
			
			const char *GetAnimationName(void) const
			{
				return (animationName);
			}
			
			void Compress(Compressor& data) const;
			bool Decompress(Decompressor& data);
	};
	
	
	class C4_API PlayAnimationFunction : public Function
	{
		private:
			
			unsigned_int32		animationMode;
			ResourceName		animationName;
			
			PlayAnimationFunction(const PlayAnimationFunction& playAnimationFunction);
			
			Function *Replicate(void) const override;
		
		public:
			
			PlayAnimationFunction();
			~PlayAnimationFunction();
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			void Compress(Compressor& data) const;
			bool Decompress(Decompressor& data);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			bool OverridesFunction(const Function *function) const;
			void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};
	
	
	class C4_API StopAnimationFunction : public Function
	{
		private:
			
			StopAnimationFunction(const StopAnimationFunction& stopAnimationFunction);
			
			Function *Replicate(void) const override;
		
		public:
			
			StopAnimationFunction();
			~StopAnimationFunction();
			
			bool OverridesFunction(const Function *function) const;
			void Execute(Controller *controller, FunctionMethod *method, const ScriptState *state);
	};
}


#endif

// ZYURVUR
