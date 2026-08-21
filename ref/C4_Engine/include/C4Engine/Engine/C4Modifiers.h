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


#ifndef C4Modifiers_h
#define C4Modifiers_h


//# \component	World Manager
//# \prefix		WorldMgr/


#include "C4Resources.h"
#include "C4Construction.h"
#include "C4Configurable.h"


namespace C4
{
	typedef Type	ModifierType;
	
	
	enum
	{
		kModifierAugmentInstance		= 'AGMT',
		kModifierWakeController			= 'WAKE',
		kModifierSleepController		= 'SLEP',
		kModifierConnectInstance		= 'CONN',
		kModifierMoveConnector			= 'MCON',
		kModifierDeleteNode				= 'DELT',
		kModifierReplaceMaterial		= 'MATL',
		kModifierRemovePhysics			= 'RPHY',
		kModifierRemoveLights			= 'RLIT',
		kModifierRemoveSources			= 'RSRC'
	};
	
	
	enum
	{
		kMaxModifierNodeNameLength	= 15
	};
	
	
	typedef String<kMaxModifierNodeNameLength> ModifierNodeName;
	
	
	class Modifier;
	class Instance;
	
	
	//# \class	ModifierRegistration	Manages internal registration information for a custom modifier type.
	//
	//# The $ModifierRegistration$ class manages internal registration information for a custom modifier type.
	//
	//# \def	class ModifierRegistration : public Registration<Modifier, ModifierRegistration>
	//
	//# \ctor	ModifierRegistration(ModifierType type, const char *name);
	//
	//# \param	type	The modifier type.
	//# \param	name	The modifier name.
	//
	//# \desc
	//# The $ModifierRegistration$ class is abstract and serves as the common base class for the template class
	//# $@ModifierReg@$. A custom modifier is registered with the engine by instantiating an object of type
	//# $ModifierReg<classType>$, where $classType$ is the type of the modifier subclass being registered.
	//
	//# \base	System/Registration<Modifier, ModifierRegistration>		A modifier registration is a specific type of registration object.
	//
	//# \also	$@ModifierReg@$
	//# \also	$@Modifier@$
	
	
	//# \function	ModifierRegistration::GetModifierType		Returns the registered modifier type.
	//
	//# \proto	ModifierType GetModifierType(void) const;
	//
	//# \desc
	//# The $GetModifierType$ function returns the modifier type for a particular modifier registration.
	//# The modifier type is established when the modifier registration is constructed.
	//
	//# \also	$@ModifierRegistration::GetModifierName@$
	
	
	//# \function	ModifierRegistration::GetModifierName		Returns the human-readable modifier name.
	//
	//# \proto	const char *GetModifierName(void) const;
	//
	//# \desc
	//# The $GetModifierName$ function returns the human-readable modifier name for a particular modifier registration.
	//# The modifier name is established when the modifier registration is constructed.
	//
	//# \also	$@ModifierRegistration::GetModifierType@$
	
	
	class C4_API ModifierRegistration : public Registration<Modifier, ModifierRegistration>
	{
		private:
			
			const char		*modifierName;
		
		protected:
			
			ModifierRegistration(ModifierType type, const char *name);
		
		public:
			 
			~ModifierRegistration();
			 
			ModifierType GetModifierType(void) const 
			{ 
				return (GetRegistrableType());
			} 
			
			const char *GetModifierName(void) const
			{
				return (modifierName); 
			}
			
			virtual bool ValidInstance(const Instance *instance) const = 0;
	}; 
	
	
	//# \class	ModifierReg		 Represents a custom modifier type.
	//
	//# The $ModifierReg$ class represents a custom modifier type.
	//
	//# \def	template <class classType> class ModifierReg : public ModifierRegistration
	//
	//# \tparam	classType	The custom modifier class.
	//
	//# \ctor	ModifierReg(ModifierType type, const char *name);
	//
	//# \param	type	The modifier type.
	//# \param	name	The modifier name.
	//
	//# \desc
	//# The $ModifierReg$ template class is used to advertise the existence of a custom modifier type.
	//# The World Manager uses a modifier registration to construct a custom modifier, and the World Editor
	//# examines a modifier registration to determine what type of instance a custom modifier can be assigned to.
	//# The act of instantiating a $ModifierReg$ object automatically registers the corresponding modifier
	//# type. The modifier type is unregistered when the $ModifierReg$ object is destroyed.
	//# 
	//# No more than one modifier registration should be created for each distinct modifier type.
	//
	//# \base	ModifierRegistration	All specific modifier registration classes share the common base class $ModifierRegistration$.
	//
	//# \also	$@Modifier@$
	
	
	template <class classType> class ModifierReg : public ModifierRegistration
	{
		public:
			
			ModifierReg(ModifierType type, const char *name) : ModifierRegistration(type, name)
			{
			}
			
			Modifier *Construct(void) const
			{
				return (new classType);
			}
			
			bool ValidInstance(const Instance *instance) const
			{
				return ((GetModifierName()) && (classType::ValidInstance(instance)));
			}
	};
	
	
	//# \class	Modifier	The base class for all modifier objects.
	//
	//# Every modifier that can be attached to a scene graph node is a subclass of the $Modifier$ class.
	//
	//# \def	class Modifier : public ListElement<Modifier>, public Packable,
	//# \def2	public Configurable, public Registrable<Modifier, ModifierRegistration>
	//
	//# \ctor	Modifier(ModifierType type);
	//
	//# \param	type	The modifier type.
	//
	//# \desc
	//# The $Modifier$ class is an object attached to an instance node that causes the instanced world to be modified in
	//# some way when it is loaded into the scene. An application may define its own custom modifiers, and they become visible in the
	//# World Editor.
	//# 
	//# A custom modifier type is defined by creating a subclass of the $Modifier$ class. For the modifier
	//# type to be visible in the World Editor, it is also necessary to construct an associated $@ModifierReg@$ object.
	//# 
	//# A custom modifier can expose its data to the World Editor by implementing the functions of the
	//# $@InterfaceMgr/Configurable@$ base class.
	//
	//# \base	Utilities/ListElement<Modifier>						Modifiers are stored in a list attached to an instance node.
	//# \base	ResourceMgr/Packable								Modifiers can be packed for storage in resources.
	//# \base	InterfaceMgr/Configurable							Modifiers can define configurable parameters that are exposed
	//#																as user interface widgets in the World Editor.
	//# \base	System/Registrable<Modifier, ModifierRegistration>	Custom modifier types can be registered with the engine.
	//
	//# \also	$@Instance::GetFirstModifier@$
	//# \also	$@Instance::AddModifier@$
	//# \also	$@ModifierReg@$
	
	
	//# \function	Modifier::GetModifierType		Returns the modifier type.
	//
	//# \proto	ModifierType GetModifierType(void) const;
	//
	//# \desc
	//# The $GetModifierType$ function returns the modifier type.
	//
	//# \also	$@ModifierReg@$
	
	
	//# \function	Modifier::ValidInstance		Returns a boolean value indicating whether the modifier can be assigned to a particular instance node.
	//
	//# \proto	static bool ValidInstance(const Instance *instance);
	//
	//# \param	instance	The instance node to be tested for validity.
	//
	//# \desc
	//# The $ValidInstance$ function can be redefined by modifier subclasses. Its implementation should examine the
	//# instance node pointed to by the $instance$ parameter and return $true$ if the modifier type can be used with the node.
	//# If the modifier type cannot be used, the $ValidInstance$ function should return $false$. If the $ValidInstance$
	//# function is not redefined for a registered subclass of the $Modifier$ class, then that modifier type
	//# can be assigned to any instance node.
	
	
	//# \function	Modifier::Apply			Applies a modifier to an instanced world.
	//
	//# \proto	virtual void Apply(World *world, Instance *instance);
	//
	//# \param	world		The main world inside which the instanced world has been expanded.
	//# \param	instance	The instance node to which the modifier is attached.
	//
	//# \desc
	//# The $Apply$ function is called for each modifier attached to an instance node immediately after the instanced
	//# world is loaded. The $Apply$ function should be overridden by subclasses of the $@Modifier@$ class, and it can
	//# make any changes to the subnodes of the instance node specified by the $instance$ parameter that are necessary
	//# to implement the modifier's functionality.
	//#
	//# Note that the instanced world will be preprocessed after all modifiers have been applied. The $Apply$ function
	//# itself should not call the $@Node::Preprocess@$ function for any new nodes that it creates, and it should not
	//# call the $@Node::AddNewSubnode@$ function to add nodes to the instanced world because that function calls the
	//# $Preprocess$ function. (The $AddSubnode$ function should be called instead.)
	//#
	//# A modifier should not make changes to any $@Object@$ classes attached to a node because they are shared
	//# among all copies of the instanced world. Any changes made to these objects would affect all instances
	//# and not just the one to which the modifier is applied.
	//#
	//# The $Apply$ function may not delete the instance node specified by the $instance$ parameter.
	//#
	//# The default implementation of the $Apply$ function performs no action.
	//
	//# \also	$@Modifier::KeepNode@$
	
	
	//# \function	Modifier::KeepNode		Returns a boolean value indicating whether a node should be included in a particular copy of an instanced world.
	//
	//# \proto	virtual bool KeepNode(const Node *node) const;
	//
	//# \param	node		A pointer to the node that should be tested for inclusion.
	//
	//# \desc
	//# The $KeepNode$ function is called for each modifier attached to an instance node every time a new copy of
	//# the instanced world is created. This function is called for every subnode of the original, unmodified copy of the
	//# instanced world, and it should return $true$ if that subnode should be copied into the new instance or $false$
	//# if that subnode and its entire subtree should be skipped so that they don't appear in the new instance.
	//#
	//# The default implementation of the $KeepNode$ function always returns $true$ so that the entire instanced world
	//# is always copied into the new instance.
	//
	//# \also	$@Modifier::Apply@$
	
	
	class C4_API Modifier : public ListElement<Modifier>, public Packable, public Configurable, public Registrable<Modifier, ModifierRegistration>
	{
		friend class Node;
		
		private:
			
			ModifierType		modifierType;
			
			virtual Modifier *Replicate(void) const;
		
		protected:
			
			Modifier(const Modifier& modifier);
		
		public:
			
			typedef ModifierType KeyType;
			
			Modifier(ModifierType type);
			virtual ~Modifier();
			
			ModifierType GetModifierType(void) const
			{
				return (modifierType);
			}
			
			Modifier *Clone(void) const
			{
				return (Replicate());
			}
			
			static Modifier *New(ModifierType type);
			static bool ValidInstance(const Instance *instance);
			static void RegisterStandardModifiers(void);
			
			void PackType(Packer& data) const;
			
			virtual void Apply(World *world, Instance *instance);
			virtual bool KeepNode(const Node *node) const;
	};
	
	
	class C4_API AugmentInstanceModifier : public Modifier
	{
		friend class ModifierReg<AugmentInstanceModifier>;
		
		private:
			
			ResourceName	worldName;
			
			AugmentInstanceModifier();
			AugmentInstanceModifier(const AugmentInstanceModifier& augmentInstanceModifier);
			
			Modifier *Replicate(void) const override;
		
		public:
			
			AugmentInstanceModifier(const char *name);
			~AugmentInstanceModifier();
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			void Apply(World *world, Instance *instance);
	};
	
	
	class C4_API WakeControllerModifier : public Modifier
	{
		private:
			
			WakeControllerModifier(const WakeControllerModifier& wakeControllerModifier);
			
			Modifier *Replicate(void) const override;
		
		public:
			
			WakeControllerModifier();
			~WakeControllerModifier();
			
			void Apply(World *world, Instance *instance);
	};
	
	
	class C4_API SleepControllerModifier : public Modifier
	{
		private:
			
			SleepControllerModifier(const SleepControllerModifier& sleepControllerModifier);
			
			Modifier *Replicate(void) const override;
		
		public:
			
			SleepControllerModifier();
			~SleepControllerModifier();
			
			void Apply(World *world, Instance *instance);
	};
	
	
	class C4_API ConnectInstanceModifier : public Modifier
	{
		friend class ModifierReg<ConnectInstanceModifier>;
		
		private:
			
			ConnectorKey		connectorKey;
			ModifierNodeName	targetNodeName;
			
			ConnectInstanceModifier();
			ConnectInstanceModifier(const ConnectInstanceModifier& connectInstanceModifier);
			
			Modifier *Replicate(void) const override;
		
		public:
			
			ConnectInstanceModifier(const char *key, const char *name);
			~ConnectInstanceModifier();
			
			const ConnectorKey& GetConnectorKey(void) const
			{
				return (connectorKey);
			}
			
			void SetConnectorKey(const ConnectorKey& key)
			{
				connectorKey = key;
			}
			
			const ModifierNodeName& GetTargetNodeName(void) const
			{
				return (targetNodeName);
			}
			
			void SetTargetNodeName(const char *name)
			{
				targetNodeName = name;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			void Apply(World *world, Instance *instance);
	};
	
	
	class C4_API MoveConnectorModifier : public Modifier
	{
		friend class ModifierReg<MoveConnectorModifier>;
		
		private:
			
			ConnectorKey		connectorKey;
			ModifierNodeName	targetNodeName;
			
			MoveConnectorModifier();
			MoveConnectorModifier(const MoveConnectorModifier& moveConnectorModifier);
			
			Modifier *Replicate(void) const override;
		
		public:
			
			MoveConnectorModifier(const char *key, const char *name);
			~MoveConnectorModifier();
			
			const ConnectorKey& GetConnectorKey(void) const
			{
				return (connectorKey);
			}
			
			void SetConnectorKey(const ConnectorKey& key)
			{
				connectorKey = key;
			}
			
			const ModifierNodeName& GetTargetNodeName(void) const
			{
				return (targetNodeName);
			}
			
			void SetTargetNodeName(const char *name)
			{
				targetNodeName = name;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			void Apply(World *world, Instance *instance);
	};
	
	
	class C4_API DeleteNodeModifier : public Modifier
	{
		friend class ModifierReg<DeleteNodeModifier>;
		
		private:
			
			unsigned_int32		nodeHash;
			ModifierNodeName	nodeName;
			
			DeleteNodeModifier();
			DeleteNodeModifier(const DeleteNodeModifier& deleteNodeModifier);
			
			Modifier *Replicate(void) const override;
		
		public:
			
			DeleteNodeModifier(const char *name);
			~DeleteNodeModifier();
			
			const ModifierNodeName& GetNodeName(void) const
			{
				return (nodeName);
			}
			
			void SetNodeName(const char *name)
			{
				nodeName = name;
			}
			
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			bool KeepNode(const Node *node) const;
	};
	
	
	class C4_API ReplaceMaterialModifier : public Modifier
	{
		friend class ModifierReg<ReplaceMaterialModifier>;
		
		private:
			
			unsigned_int32		nodeHash;
			ModifierNodeName	nodeName;
			
			MaterialObject		*materialObject;
			
			ReplaceMaterialModifier();
			ReplaceMaterialModifier(const ReplaceMaterialModifier& replaceMaterialModifier);
			
			Modifier *Replicate(void) const override;
			
			static void MaterialObjectLinkProc(Object *object, void *cookie);
		
		public:
			
			ReplaceMaterialModifier(const char *name);
			~ReplaceMaterialModifier();
			
			const ModifierNodeName& GetNodeName(void) const
			{
				return (nodeName);
			}
			
			void SetNodeName(const char *name)
			{
				nodeName = name;
			}
			
			MaterialObject *GetMaterialObject(void) const
			{
				return (materialObject);
			}
			
			void Prepack(List<Object> *linkList) const;
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			void *BeginSettingsUnpack(void);
			
			int32 GetSettingCount(void) const;
			Setting *GetSetting(int32 index) const;
			void SetSetting(const Setting *setting);
			
			void SetMaterialObject(MaterialObject *object);
			
			void Apply(World *world, Instance *instance);
	};
	
	
	class C4_API RemovePhysicsModifier : public Modifier
	{
		private:
			
			RemovePhysicsModifier(const RemovePhysicsModifier& removePhysicsModifier);
			
			Modifier *Replicate(void) const override;
		
		public:
			
			RemovePhysicsModifier();
			~RemovePhysicsModifier();
			
			void Apply(World *world, Instance *instance);
			bool KeepNode(const Node *node) const;
	};
	
	
	class C4_API RemoveLightsModifier : public Modifier
	{
		private:
			
			RemoveLightsModifier(const RemoveLightsModifier& removeLightsModifier);
			
			Modifier *Replicate(void) const override;
		
		public:
			
			RemoveLightsModifier();
			~RemoveLightsModifier();
			
			bool KeepNode(const Node *node) const;
	};
	
	
	class C4_API RemoveSourcesModifier : public Modifier
	{
		private:
			
			RemoveSourcesModifier(const RemoveSourcesModifier& removeSourcesModifier);
			
			Modifier *Replicate(void) const override;
		
		public:
			
			RemoveSourcesModifier();
			~RemoveSourcesModifier();
			
			bool KeepNode(const Node *node) const;
	};
}


#endif

// ZYURVUR
