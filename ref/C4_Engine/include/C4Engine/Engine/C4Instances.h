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


#ifndef C4Instances_h
#define C4Instances_h


//# \component	World Manager
//# \prefix		WorldMgr/


#include "C4Node.h"
#include "C4Modifiers.h"


namespace C4
{
	//# \class	Instance		Represents an instanced world node in a world.
	//
	//# The $Instance$ class represents an instanced world node in a world.
	//
	//# \def	class Instance : public Node, public ListElement<Instance>
	//
	//# \ctor	Instance(const char *name);
	//
	//# \param	name		The name of the instanced world.
	//
	//# \desc
	//# The $Instance$ class represents an instance node from which another world
	//# (the instanced world) can be expanded.
	//
	//# \base	Node								An instance is a type of node.
	//# \base	Utilities/ListElement<Instance>		Used internally by the World Manager.
	
	
	//# \function	Instance::GetWorldName		Returns the name of an instanced world.
	//
	//# \proto	const ResourceName& GetWorldName(void) const;
	//
	//# \desc
	//# The $GetWorldName$ function returns the name of the world resource referenced by an instance node.
	//
	//# \also	$@Instance::SetWorldName@$
	
	
	//# \function	Instance::SetWorldName		Sets the name of an instanced world.
	//
	//# \proto	void SetWorldName(const char *name);
	//
	//# \param	name	The name of the instanced world.
	//
	//# \desc
	//# The $SetWorldName$ function sets the name of the world resource referenced by an instance node
	//# to that specified by the $name$ parameter.
	//
	//# \also	$@Instance::GetWorldName@$
	
	
	//# \function	Instance::GetFirstModifier	Returns the first modifier for an instanced world.
	//
	//# \proto	Modifier *GetFirstModifier(void) const;
	//
	//# \desc
	//# The $GetFirstModifier$ function returns the first modifier attached to an instanced world.
	//# If the instance has no modifiers, then the return value is $nullptr$.
	//
	//# \also	$@Instance::AddModifier@$
	//# \also	$@Modifier@$
	
	
	//# \function	Instance::AddModifier		Adds a modifier to an instanced world.
	//
	//# \proto	void AddModifier(Modifier *modifier);
	//
	//# \param	modifier	The modifier to add to the instance.
	//
	//# \desc
	//# The $AddModifier$ function adds the modifier specified by the $modifier$ parameter to an
	//# instanced world.
	//
	//# \also	$@Instance::GetFirstModifier@$
	//# \also	$@Modifier@$
	
	
	class Instance : public Node, public ListElement<Instance>
	{
		friend class Node;
		
		private:
			
			List<Modifier>		modifierList;
			ResourceName		worldName;
			
			Instance();
			Instance(const Instance& instance);
			
			Node *Replicate(void) const override;
			
			void ExtractNodes(Node *rigidBodyNode, unsigned_int32 worldFlags);
			static bool ModifierCloneFilter(const Node *node, void *cookie);
		
		public:
			
			C4API Instance(const char *name);
			C4API ~Instance(); 
			
			using ListElement<Instance>::Previous; 
			using ListElement<Instance>::Next; 
			 
			Modifier *GetFirstModifier(void) const
			{ 
				return (modifierList.First());
			}
			
			void AddModifier(Modifier *modifier) 
			{
				modifierList.Append(modifier);
			}
			 
			void PurgeModifiers(void)
			{
				modifierList.Purge();
			}
			
			const ResourceName& GetWorldName(void) const
			{
				return (worldName);
			}
			
			void SetWorldName(const char *name)
			{
				worldName = name;
			}
			
			void Prepack(List<Object> *linkList) const;
			void Pack(Packer& data, unsigned_int32 packFlags) const;
			void Unpack(Unpacker& data, unsigned_int32 unpackFlags);
			bool UnpackChunk(const ChunkHeader *chunkHeader, Unpacker& data, unsigned_int32 unpackFlags);
			void *BeginSettingsUnpack(void);
			
			int32 GetCategoryCount(void) const;
			Type GetCategoryType(int32 index, const char **title) const;
			int32 GetCategorySettingCount(Type category) const;
			Setting *GetCategorySetting(Type category, int32 index, unsigned_int32 flags) const;
			void SetCategorySetting(Type category, const Setting *setting);
			
			int32 GetInternalConnectorCount(void) const;
			const char *GetInternalConnectorKey(int32 index) const;
			bool ValidConnectedNode(const ConnectorKey& key, const Node *node) const;
			
			C4API bool Expand(World *world);
			C4API void Collapse(void);
			
			void Preprocess(void);
			void Neutralize(void);
			
			void EnterZone(Zone *zone);
	};
}


#endif

// ZYURVUR
