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


#ifndef C4Tree_h
#define C4Tree_h


//# \component	Utility Library
//# \prefix		Utilities/


#include "C4Memory.h"


namespace C4
{
	class C4_API TreeBase
	{
		private:
			
			TreeBase		*prevNode;
			TreeBase		*nextNode;
			TreeBase		*superNode;
			TreeBase		*firstSubnode;
			TreeBase		*lastSubnode;
		
		protected:
			
			TreeBase()
			{
				prevNode = nullptr;
				nextNode = nullptr;
				superNode = nullptr;
				firstSubnode = nullptr;
				lastSubnode = nullptr;
			}
			
			virtual ~TreeBase();
			
			TreeBase *Previous(void) const
			{
				return (prevNode);
			}
			
			TreeBase *Next(void) const
			{
				return (nextNode);
			}
			
			TreeBase *GetSuperNode(void) const
			{
				return (superNode);
			}
			
			TreeBase *GetFirstSubnode(void) const
			{
				return (firstSubnode);
			}
			
			TreeBase *GetLastSubnode(void) const
			{
				return (lastSubnode);
			}
			
			TreeBase *GetRootNode(void);
			const TreeBase *GetRootNode(void) const;
			
			bool Successor(const TreeBase *node) const;
			
			TreeBase *GetLeftmostNode(void);
			const TreeBase *GetLeftmostNode(void) const;
			TreeBase *GetRightmostNode(void);
			const TreeBase *GetRightmostNode(void) const;
			
			TreeBase *GetNextNode(const TreeBase *node) const;
			TreeBase *GetPreviousNode(const TreeBase *node);
			const TreeBase *GetPreviousNode(const TreeBase *node) const;
			TreeBase *GetNextLevelNode(const TreeBase *node) const;
			TreeBase *GetPreviousLevelNode(const TreeBase *node) const;
			
			void MoveSubtree(TreeBase *super);
			
			void AddSubnode(TreeBase *node);
			void AddFirstSubnode(TreeBase *node);
			void AddSubnodeBefore(TreeBase *node, TreeBase *before);
			void AddSubnodeAfter(TreeBase *node, TreeBase *after);
			void RemoveSubnode(TreeBase *subnode);
		
		public:
			
			int32 GetSubnodeCount(void) const;
			int32 GetNodeIndex(void) const;
			
			void RemoveSubtree(void);
			void PurgeSubtree(void);
			
			virtual void Detach(void);
	};
	
	
	//# \class	Tree	The base class for objects that can be stored in a hierarchical tree.
	//
	//# Objects inherit from the $Tree$ class so that they can be stored in a hierarchical tree.
	//
	//# \def	template <class type> class Tree : public TreeBase 
	//
	//# \tparam		type	The type of the class that can be stored in a tree. This parameter should be the 
	//#						type of the class that inherits directly from the $Tree$ class. 
	// 
	//# \ctor	Tree();
	// 
	//# The constructor has protected access takes no parameters. The $Tree$ class can only exist
	//# as a base class for another class.
	//
	//# \desc 
	//# The $Tree$ class should be declared as a base class for objects that need to be stored in a hierarchical tree.
	//# The $type$ template parameter should match the class type of such objects.
	//# 
	//# When a $Tree$ object is initially created, its super node is $nullptr$ and it has no subnodes. An object having no super node 
	//# is the root of a tree. (A newly created $Tree$ object can be thought of as the root of a tree containing a single object.)
	//# In a normal tree structure, one object is created to serve as the root node. Objects are added to the tree by calling the
	//# $@Tree::AddSubnode@$ function to designate them as subnodes of other objects in the tree.
	//# 
	//# A tree is traversed in depth-first order using the $@Tree::GetNextNode@$ function. The root node of a tree is considered
	//# to be the first node visited in a traversal. Iterative calls to the $@Tree::GetNextNode@$ function visit every non-root
	//# node in a tree.
	//# 
	//# When a $Tree$ object is destroyed, all of its subnodes (and all of their subnodes, etc.) are also destroyed.
	//
	//# \privbase	TreeBase	Used internally to encapsulate common functionality that is independent
	//#							of the template parameter.
	
	
	//# \function	Tree::GetSuperNode		Returns the super node of an object.
	//
	//# \proto	type *GetSuperNode(void) const;
	//
	//# \desc
	//# The $GetSuperNode$ function returns the direct super node above an object in a tree. If the object is the root of a
	//# tree, then the return value is $nullptr$.
	//
	//# \also	$@Tree::GetRootNode@$
	//# \also	$@Tree::Successor@$
	//# \also	$@Tree::GetFirstSubnode@$
	//# \also	$@Tree::GetNextNode@$
	
	
	//# \function	Tree::GetRootNode		Returns the root node of the tree containing an object.
	//
	//# \proto	type *GetRootNode(void);
	//# \proto	const type *GetRootNode(void) const;
	//
	//# \desc
	//# The $GetRootNode$ function returns the root node of the tree containing an object. If the object is the root node
	//# of a tree, then the return value is a pointer to itself.
	//
	//# \also	$@Tree::GetSuperNode@$
	//# \also	$@Tree::Successor@$
	//# \also	$@Tree::GetFirstSubnode@$
	//# \also	$@Tree::GetNextNode@$
	
	
	//# \function	Tree::GetFirstSubnode		Returns the first subnode of an object.
	//
	//# \proto	type *GetFirstSubnode(void) const;
	//
	//# \desc
	//# The $GetFirstSubnode$ function returns the first direct subnode of an object is a tree. If the object has no subnodes,
	//# then the return value is $nullptr$.
	//# 
	//# All of the direct subnodes of an object are stored in a single linked list. To go from one subnode to the next,
	//# starting with the first, the $@ListElement::Next@$ function should be called.
	//
	//# \also	$@Tree::GetNextNode@$
	//# \also	$@Tree::GetLastSubnode@$
	//# \also	$@Tree::GetSuperNode@$
	//# \also	$@Tree::GetRootNode@$
	
	
	//# \function	Tree::GetLastSubnode		Returns the last subnode of an object.
	//
	//# \proto	type *GetLastSubnode(void) const;
	//
	//# \desc
	//# The $GetLastSubnode$ function returns the last direct subnode of an object is a tree. If the object has no subnodes,
	//# then the return value is $nullptr$.
	//# 
	//# All of the direct subnodes of an object are stored in a single linked list. To go from one subnode to the next,
	//# starting with the last, the $@ListElement::Previous@$ function should be called.
	//
	//# \also	$@Tree::GetFirstSubnode@$
	//# \also	$@Tree::GetSuperNode@$
	//# \also	$@Tree::GetRootNode@$
	
	
	//# \function	Tree::GetNextNode		Returns the next node in a traversal of a tree.
	//
	//# \proto	type *GetNextNode(const Tree<type> *node) const;
	//
	//# \param	node	A pointer to the current node in the traversal.
	//
	//# \desc
	//# To iterate through the subnode hierarchy of a tree, the $GetNextNode$ function should be repeatedly called for the root
	//# node of the tree until it returns $nullptr$. The root node is considered the first node visited in the tree. The second
	//# node is obtained by calling $GetNextNode$ with the $node$ parameter set to a pointer to the root node. The traversal
	//# occurs in depth-first order, meaning that a particular node's entire subtree is visited before the next node at the
	//# same level in the tree. The traversal is also a pre-order traversal, meaning that a particular node is visited
	//# before any of its subnodes are visited.
	//
	//# \also	$@Tree::GetFirstSubnode@$
	//# \also	$@Tree::GetNextLevelNode@$
	//# \also	$@Tree::GetSuperNode@$
	//# \also	$@Tree::GetRootNode@$
	
	
	//# \function	Tree::GetNextLevelNode		Returns the next node in a traversal of a tree that is not a subnode of the current node.
	//
	//# \proto	type *GetNextLevelNode(const Tree<type> *node) const;
	//
	//# \param	node	A pointer to the current node in the traversal.
	//
	//# \desc
	//# During iteration of the nodes in a tree, the $GetNextLevelNode$ function can be used to jump to the next subnode on the
	//# same level as the $node$ parameter, skipping its entire subtree. Node selection after skipping the subtree behaves
	//# exactly like that used by the $@Tree::GetNextNode@$ function.
	//
	//# \also	$@Tree::GetNextNode@$
	//# \also	$@Tree::GetFirstSubnode@$
	//# \also	$@Tree::GetSuperNode@$
	//# \also	$@Tree::GetRootNode@$
	
	
	//# \function	Tree::GetSubnodeCount		Returns the number of immediate subnodes of an object.
	//
	//# \proto	int32 GetSubnodeCount(void) const;
	//
	//# \desc
	//# The $GetSubnodeCount$ function returns the number of the immediate subnodes of an object.
	//
	//# \also	$@Tree::GetFirstSubnode@$
	//# \also	$@Tree::GetLastSubnode@$
	
	
	//# \function	Tree::AddSubnode		Adds a direct subnode to an object.
	//
	//# \proto	virtual void AddSubnode(Tree<type> *node);
	//
	//# \param	node	A pointer to the subnode to add.
	//
	//# \desc
	//# The $AddSubnode$ function adds the node pointed to by the $node$ parameter to an object. If the node is already a
	//# member of a tree, then it is first removed from that tree. If the node being added as a subnode has subnodes itself,
	//# then those subnodes are carried with the node into the new tree.
	//
	//# \also	$@Tree::RemoveSubnode@$
	//# \also	$@Tree::GetFirstSubnode@$
	//# \also	$@Tree::GetNextNode@$
	
	
	//# \function	Tree::RemoveSubnode		Removes a direct subnode of an object.
	//
	//# \proto	virtual void RemoveSubnode(Tree<type> *node);
	//
	//# \param	node	A pointer to the subnode to remove.
	//
	//# \desc
	//# The $RemoveSubnode$ function removes the node pointed to by the $node$ parameter from an object. The node must be
	//# an existing subnode of the object for which $RemoveSubnode$ is called. If the node has any subnodes, then those
	//# subnodes remain subnodes of the removed node.
	//
	//# \also	$@Tree::AddSubnode@$
	//# \also	$@Tree::GetFirstSubnode@$
	//# \also	$@Tree::GetNextNode@$
	
	
	//# \function	Tree::MoveSubtree		Moves all subnodes of an object to another object.
	//
	//# \proto	virtual void MoveSubtree(Tree<type> *super);
	//
	//# \param	super	The object to which the subnodes are moved. This cannot be the same object for which
	//#					the $MoveSubtree$ function is called.
	//
	//# \desc
	//# The $MoveSubtree$ function removes all of the subnodes of an object and then transfers all of them as
	//# subnodes of the object specified by the $super$ parameter.
	//
	//# \also	$@Tree::AddSubnode@$
	//# \also	$@Tree::RemoveSubnode@$
	//# \also	$@Tree::PurgeSubtree@$
	
	
	//# \function	Tree::RemoveSubtree		Removes all subnodes of an object.
	//
	//# \proto	void RemoveSubtree(void);
	//
	//# \desc
	//# The $RemoveSubtree$ function removes all of the immediate subnodes of an object.
	//
	//# \also	$@Tree::RemoveSubnode@$
	//# \also	$@Tree::PurgeSubtree@$
	
	
	//# \function	Tree::PurgeSubtree		Deletes all subnodes of an object.
	//
	//# \proto	void PurgeSubtree(void);
	//
	//# \desc
	//# The $PurgeSubtree$ function recursively deletes all of the subnodes of an object.
	//
	//# \also	$@Tree::RemoveSubtree@$
	//# \also	$@Tree::RemoveSubnode@$
	
	
	//# \function	Tree::Successor		Returns a boolean value indicating whether one node is an successor of another.
	//
	//# \proto	bool Successor(const Tree<type> *node) const;
	//
	//# \desc
	//# The $Successor$ function returns $true$ if the node specified by the $node$ parameter is a successor
	//# of the node for which the function is called. If the node is not an successor, this function returns $false$.
	//
	//# \also	$@Tree::GetSuperNode@$
	//# \also	$@Tree::GetRootNode@$
	
	
	template <class type> class Tree : public TreeBase
	{
		protected:
			
			Tree() {}
		
		public:
			
			~Tree() {}
			
			type *Previous(void) const
			{
				return (static_cast<type *>(static_cast<Tree<type> *>(TreeBase::Previous())));
			}
			
			type *Next(void) const
			{
				return (static_cast<type *>(static_cast<Tree<type> *>(TreeBase::Next())));
			}
			
			type *GetSuperNode(void) const
			{
				return (static_cast<type *>(static_cast<Tree<type> *>(TreeBase::GetSuperNode())));
			}
			
			type *GetFirstSubnode(void) const
			{
				return (static_cast<type *>(static_cast<Tree<type> *>(TreeBase::GetFirstSubnode())));
			}
			
			type *GetLastSubnode(void) const
			{
				return (static_cast<type *>(static_cast<Tree<type> *>(TreeBase::GetLastSubnode())));
			}
			
			type *GetRootNode(void)
			{
				return (static_cast<type *>(static_cast<Tree<type> *>(TreeBase::GetRootNode())));
			}
			
			const type *GetRootNode(void) const
			{
				return (static_cast<const type *>(static_cast<const Tree<type> *>(TreeBase::GetRootNode())));
			}
			
			bool Successor(const Tree<type> *node) const
			{
				return (TreeBase::Successor(node));
			}
			
			type *GetLeftmostNode(void)
			{
				return (static_cast<type *>(static_cast<Tree<type> *>(TreeBase::GetLeftmostNode())));
			}
			
			const type *GetLeftmostNode(void) const
			{
				return (static_cast<const type *>(static_cast<const Tree<type> *>(TreeBase::GetLeftmostNode())));
			}
			
			type *GetRightmostNode(void)
			{
				return (static_cast<type *>(static_cast<Tree<type> *>(TreeBase::GetRightmostNode())));
			}
			
			const type *GetRightmostNode(void) const
			{
				return (static_cast<const type *>(static_cast<const Tree<type> *>(TreeBase::GetRightmostNode())));
			}
			
			type *GetNextNode(const Tree<type> *node) const
			{
				return (static_cast<type *>(static_cast<Tree<type> *>(TreeBase::GetNextNode(node))));
			}
			
			type *GetPreviousNode(const Tree<type> *node)
			{
				return (static_cast<type *>(static_cast<Tree<type> *>(TreeBase::GetPreviousNode(node))));
			}
			
			const type *GetPreviousNode(const Tree<type> *node) const
			{
				return (static_cast<const type *>(static_cast<const Tree<type> *>(TreeBase::GetPreviousNode(node))));
			}
			
			type *GetNextLevelNode(const Tree<type> *node) const
			{
				return (static_cast<type *>(static_cast<Tree<type> *>(TreeBase::GetNextLevelNode(node))));
			}
			
			type *GetPreviousLevelNode(const Tree<type> *node) const
			{
				return (static_cast<type *>(static_cast<Tree<type> *>(TreeBase::GetPreviousLevelNode(node))));
			}
			
			void MoveSubtree(Tree<type> *super)
			{
				TreeBase::MoveSubtree(super);
			}
			
			virtual void AddSubnode(type *node);
			virtual void AddFirstSubnode(type *node);
			virtual void AddSubnodeBefore(type *node, type *before);
			virtual void AddSubnodeAfter(type *node, type *after);
			virtual void RemoveSubnode(type *node);
	};
	
	
	template <class type> void Tree<type>::AddSubnode(type *node)
	{
		TreeBase::AddSubnode(static_cast<Tree<type> *>(node));
	}
	
	template <class type> void Tree<type>::AddFirstSubnode(type *node)
	{
		TreeBase::AddFirstSubnode(static_cast<Tree<type> *>(node));
	}
	
	template <class type> void Tree<type>::AddSubnodeBefore(type *node, type *before)
	{
		TreeBase::AddSubnodeBefore(static_cast<Tree<type> *>(node), static_cast<Tree<type> *>(before));
	}
	
	template <class type> void Tree<type>::AddSubnodeAfter(type *node, type *after)
	{
		TreeBase::AddSubnodeAfter(static_cast<Tree<type> *>(node), static_cast<Tree<type> *>(after));
	}
	
	template <class type> void Tree<type>::RemoveSubnode(type *node)
	{
		TreeBase::RemoveSubnode(static_cast<Tree<type> *>(node));
	}
	
	
	//# \class	UpdatableTree	The base class for objects that can be stored in a tree and need to track update flags.
	//
	//# Objects inherit from the $UpdatableTree$ class so that they can be stored in a hierarchical tree
	//# and have update flags tracked.
	//
	//# \def	template <class type> class UpdatableTree : public Tree<type>
	//
	//# \tparam		type	The type of the class that can be stored in a tree. This parameter should be the
	//#						type of the class that inherits directly from the $UpdatableTree$ class.
	//
	//# \ctor	UpdatableTree();
	//
	//# The constructor has protected access takes no parameters. The $UpdatableTree$ class can only exist
	//# as a base class for another class.
	//
	//# \desc
	//
	//# \base	Tree<type>		An $UpdatableTree$ object is an extension of a $Tree$ object.
	
	
	//# \function	UpdatableTree::GetActiveUpdateFlags		Returns the active update flags for a node.
	//
	//# \proto	unsigned_int32 GetActiveUpdateFlags(void) const;
	//
	//# \desc
	//# 
	//
	//# \also	$@UpdatableTree::SetActiveUpdateFlags@$
	
	
	//# \function	UpdatableTree::SetActiveUpdateFlags		Sets the active update flags for a node.
	//
	//# \proto	void SetActiveUpdateFlags(unsigned_int32 flags);
	//
	//# \param	flags	The new active update flags.
	//
	//# \desc
	//# 
	//
	//# \also	$@UpdatableTree::GetActiveUpdateFlags@$
	
	
	//# \function	UpdatableTree::GetCurrentUpdateFlags	Returns the current update flags for a node.
	//
	//# \proto	unsigned_int32 GetCurrentUpdateFlags(void) const;
	//
	//# \desc
	//# 
	//
	//# \also	$@UpdatableTree::GetActiveUpdateFlags@$
	
	
	//# \function	UpdatableTree::GetSubtreeUpdateFlags		Returns the cumulative update flags for all subnodes.
	//
	//# \proto	unsigned_int32 GetSubtreeUpdateFlags(void) const;
	//
	//# \desc
	//# 
	//
	//# \also	$@UpdatableTree::GetCurrentUpdateFlags@$
	//# \also	$@UpdatableTree::GetActiveUpdateFlags@$
	
	
	//# \function	UpdatableTree::Invalidate		Invalidates a node and its subnodes.
	//
	//# \proto	virtual void Invalidate(void);
	//
	//# \desc
	//# 
	//
	//# \also	$@UpdatableTree::Update@$
	
	
	//# \function	UpdatableTree::Update		Updates a node and its subnodes.
	//
	//# \proto	virtual void Update(void);
	//
	//# \desc
	//# 
	//
	//# \also	$@UpdatableTree::Invalidate@$
	
	
	template <class type> class UpdatableTree : public Tree<type>
	{
		private:
			
			unsigned_int32	activeUpdateFlags;
			unsigned_int32	currentUpdateFlags;
			unsigned_int32	subtreeUpdateFlags;
		
		protected:
			
			UpdatableTree();
			
			void SetCurrentUpdateFlags(unsigned_int32 flags)
			{
				currentUpdateFlags = flags;
			}
			
			void SetSubtreeUpdateFlags(unsigned_int32 flags)
			{
				subtreeUpdateFlags = flags;
			}
			
			void PropagateUpdateFlags(unsigned_int32 flags);
		
		public:
			
			~UpdatableTree();
			
			unsigned_int32 GetActiveUpdateFlags(void) const
			{
				return (activeUpdateFlags);
			}
			
			unsigned_int32 GetCurrentUpdateFlags(void) const
			{
				return (currentUpdateFlags);
			}
			
			unsigned_int32 GetSubtreeUpdateFlags(void) const
			{
				return (subtreeUpdateFlags);
			}
			
			void Detach(void) override;
			
			void AddSubnode(type *node) override;
			void AddFirstSubnode(type *node) override;
			void AddSubnodeBefore(type *node, type *before) override;
			void AddSubnodeAfter(type *node, type *after) override;
			void RemoveSubnode(type *node) override;
			
			void SetActiveUpdateFlags(unsigned_int32 flags);
			
			virtual void Invalidate(void);
			virtual void Update(void);
	};
	
	template <class type> UpdatableTree<type>::UpdatableTree()
	{
		activeUpdateFlags = type::kInitialUpdateFlags;
		currentUpdateFlags = type::kInitialUpdateFlags;
		subtreeUpdateFlags = 0;
	}
	
	template <class type> UpdatableTree<type>::~UpdatableTree()
	{
		unsigned_int32 flags = activeUpdateFlags & type::kPropagatedUpdateFlags;
		if (flags != 0)
		{
			UpdatableTree *super = Tree<type>::GetSuperNode();
			if (super) super->PropagateUpdateFlags(flags);
		}
	}
	
	template <class type> void UpdatableTree<type>::PropagateUpdateFlags(unsigned_int32 flags)
	{
		UpdatableTree *node = this;
		do
		{
			if ((node->subtreeUpdateFlags & flags) == flags) break;
			
			node->currentUpdateFlags |= flags & type::kPropagatedUpdateFlags;
			node->subtreeUpdateFlags |= flags;
			
			node = node->GetSuperNode();
		} while (node);
	}
	
	template <class type> void UpdatableTree<type>::Detach(void)
	{
		UpdatableTree *super = Tree<type>::GetSuperNode();
		if (super)
		{
			unsigned_int32 flags = activeUpdateFlags & type::kPropagatedUpdateFlags;
			if (flags != 0) super->PropagateUpdateFlags(flags);
			
			Tree<type>::Detach();
		}
	}
	
	template <class type> void UpdatableTree<type>::AddSubnode(type *node)
	{
		node->UpdatableTree<type>::Detach();
		Tree<type>::AddSubnode(node);
		node->Invalidate();
	}
	
	template <class type> void UpdatableTree<type>::AddFirstSubnode(type *node)
	{
		node->UpdatableTree<type>::Detach();
		Tree<type>::AddFirstSubnode(node);
		node->Invalidate();
	}
	
	template <class type> void UpdatableTree<type>::AddSubnodeBefore(type *node, type *before)
	{
		node->UpdatableTree<type>::Detach();
		Tree<type>::AddSubnodeBefore(node, before);
		node->Invalidate();
	}
	
	template <class type> void UpdatableTree<type>::AddSubnodeAfter(type *node, type *after)
	{
		node->UpdatableTree<type>::Detach();
		Tree<type>::AddSubnodeAfter(node, after);
		node->Invalidate();
	}
	
	template <class type> void UpdatableTree<type>::RemoveSubnode(type *node)
	{
		unsigned_int32 flags = node->GetActiveUpdateFlags() & type::kPropagatedUpdateFlags;
		if (flags != 0) PropagateUpdateFlags(type::kPropagatedUpdateFlags);
		
		Tree<type>::RemoveSubnode(node);
	}
	
	template <class type> void UpdatableTree<type>::SetActiveUpdateFlags(unsigned_int32 flags)
	{
		activeUpdateFlags = flags;
		currentUpdateFlags = flags;
		
		UpdatableTree *super = Tree<type>::GetSuperNode();
		if (super) super->PropagateUpdateFlags(flags);
	}
	
	template <class type> void UpdatableTree<type>::Invalidate(void)
	{
		unsigned_int32 flags = currentUpdateFlags | activeUpdateFlags;
		currentUpdateFlags = flags;
		
		UpdatableTree *super = Tree<type>::GetSuperNode();
		if (super) super->PropagateUpdateFlags(flags | subtreeUpdateFlags);
		
		UpdatableTree *node = Tree<type>::GetFirstSubnode();
		while (node)
		{
			node->Invalidate();
			node = node->Next();
		}
	}
	
	template <class type> void UpdatableTree<type>::Update(void)
	{
		currentUpdateFlags = 0;
		
		if (subtreeUpdateFlags != 0)
		{
			subtreeUpdateFlags = 0;
			
			UpdatableTree *node = Tree<type>::GetFirstSubnode();
			while (node)
			{
				node->Update();
				node = node->Next();
			}
		}
	}
}


#endif

// ZYURVUR
