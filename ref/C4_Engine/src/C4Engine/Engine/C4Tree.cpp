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


#include "C4Tree.h"


using namespace C4;


TreeBase::~TreeBase()
{
	PurgeSubtree();
	if (superNode) superNode->RemoveSubnode(this);
}

int32 TreeBase::GetSubnodeCount(void) const
{
	machine count = 0;
	const TreeBase *subnode = firstSubnode;
	while (subnode)
	{
		count++;
		subnode = subnode->nextNode;
	}
	
	return ((int32) count);
}

int32 TreeBase::GetNodeIndex(void) const
{
	machine index = 0;
	
	const TreeBase *element = this;
	for (;;)
	{
		element = element->Previous();
		if (!element) break;
		index++;
	}
	
	return ((int32) index);
}

TreeBase *TreeBase::GetRootNode(void)
{
	TreeBase *root = this;
	for (;;)
	{
		TreeBase *node = root->superNode;
		if (!node) break;
		root = node;
	}
	
	return (root);
}

const TreeBase *TreeBase::GetRootNode(void) const
{
	const TreeBase *root = this;
	for (;;)
	{
		const TreeBase *node = root->superNode;
		if (!node) break;
		root = node;
	}
	
	return (root);
}

bool TreeBase::Successor(const TreeBase *node) const
{
	TreeBase *super = node->superNode;
	while (super)
	{
		if (super == this) return (true);
		super = super->superNode;
	}
	
	return (false);
}

TreeBase *TreeBase::GetLeftmostNode(void)
{
	TreeBase *node = this;
	for (;;)
	{
		TreeBase *subnode = node->firstSubnode;
		if (!subnode) break;
		node = subnode;
	}
	
	return (node);
}

const TreeBase *TreeBase::GetLeftmostNode(void) const
{
	const TreeBase *node = this;
	for (;;)
	{
		const TreeBase *subnode = node->firstSubnode;
		if (!subnode) break;
		node = subnode;
	}
	
	return (node); 
}
 
TreeBase *TreeBase::GetRightmostNode(void) 
{ 
	TreeBase *node = this;
	for (;;) 
	{
		TreeBase *subnode = node->lastSubnode;
		if (!subnode) break;
		node = subnode; 
	}
	
	return (node);
} 

const TreeBase *TreeBase::GetRightmostNode(void) const
{
	const TreeBase *node = this;
	for (;;)
	{
		const TreeBase *subnode = node->lastSubnode;
		if (!subnode) break;
		node = subnode;
	}
	
	return (node);
}

TreeBase *TreeBase::GetNextNode(const TreeBase *node) const
{
	TreeBase *n = node->GetFirstSubnode();
	if (!n)
	{
		for (;;)
		{
			if (node == this) break;
			
			n = node->nextNode;
			if (n) break;
			
			node = node->superNode;
		}
	}

	return (n);
}

TreeBase *TreeBase::GetPreviousNode(const TreeBase *node)
{
	if (node == this) return (nullptr);
	
	TreeBase *n = node->prevNode;
	if (!n) return (node->superNode);
	return (n->GetRightmostNode());
}

const TreeBase *TreeBase::GetPreviousNode(const TreeBase *node) const
{
	if (node == this) return (nullptr);
	
	const TreeBase *n = node->prevNode;
	if (!n) return (node->superNode);
	return (n->GetRightmostNode());
}

TreeBase *TreeBase::GetNextLevelNode(const TreeBase *node) const
{
	TreeBase *n = nullptr;
	for (;;)
	{
		if (node == this) break;
		
		n = node->Next();
		if (n) break;
		
		node = node->superNode;
	}
	
	return (n);
}

TreeBase *TreeBase::GetPreviousLevelNode(const TreeBase *node) const
{
	TreeBase *n = nullptr;
	for (;;)
	{
		if (node == this) break;
		
		n = node->Previous();
		if (n) break;
		
		node = node->superNode;
	}
	
	return (n);
}

void TreeBase::MoveSubtree(TreeBase *super)
{
	for (;;)
	{
		TreeBase *node = GetFirstSubnode();
		if (!node) break;
		super->AddSubnode(node);
	}
}

void TreeBase::RemoveSubtree(void)
{
	TreeBase *subnode = firstSubnode;
	while (subnode)
	{
		TreeBase *next = subnode->nextNode;
		subnode->prevNode = nullptr;
		subnode->nextNode = nullptr;
		subnode->superNode = nullptr;
		subnode = next;
	}
	
	firstSubnode = nullptr;
	lastSubnode = nullptr;
}

void TreeBase::PurgeSubtree(void)
{
	while (firstSubnode) delete firstSubnode;
}

void TreeBase::AddSubnode(TreeBase *node)
{
	TreeBase *tree = node->superNode;
	if (tree)
	{
		TreeBase *prev = node->prevNode;
		TreeBase *next = node->nextNode;
		
		if (prev)
		{
			prev->nextNode = next;
			node->prevNode = nullptr;
		}
		
		if (next)
		{
			next->prevNode = prev;
			node->nextNode = nullptr;
		}
		
		if (tree->firstSubnode == node) tree->firstSubnode = next;
		if (tree->lastSubnode == node) tree->lastSubnode = prev;
	}
	
	if (lastSubnode)
	{
		lastSubnode->nextNode = node;
		node->prevNode = lastSubnode;
		lastSubnode = node;
	}
	else
	{
		firstSubnode = lastSubnode = node;
	}
	
	node->superNode = this;
}

void TreeBase::AddFirstSubnode(TreeBase *node)
{
	TreeBase *tree = node->superNode;
	if (tree)
	{
		TreeBase *prev = node->prevNode;
		TreeBase *next = node->nextNode;
		
		if (prev)
		{
			prev->nextNode = next;
			node->prevNode = nullptr;
		}
		
		if (next)
		{
			next->prevNode = prev;
			node->nextNode = nullptr;
		}
		
		if (tree->firstSubnode == node) tree->firstSubnode = next;
		if (tree->lastSubnode == node) tree->lastSubnode = prev;
	}
	
	if (firstSubnode)
	{
		firstSubnode->prevNode = node;
		node->nextNode = firstSubnode;
		firstSubnode = node;
	}
	else
	{
		firstSubnode = lastSubnode = node;
	}
	
	node->superNode = this;
}

void TreeBase::AddSubnodeBefore(TreeBase *node, TreeBase *before)
{
	TreeBase *tree = node->superNode;
	if (tree)
	{
		TreeBase *prev = node->prevNode;
		TreeBase *next = node->nextNode;
		if (prev) prev->nextNode = next;
		if (next) next->prevNode = prev;
		if (tree->firstSubnode == node) tree->firstSubnode = next;
		if (tree->lastSubnode == node) tree->lastSubnode = prev;
	}
	
	node->nextNode = before;
	TreeBase *after = before->prevNode;
	node->prevNode = after;
	
	if (after) after->nextNode = node;
	else firstSubnode = node;
	
	before->prevNode = node;
	node->superNode = this;
}

void TreeBase::AddSubnodeAfter(TreeBase *node, TreeBase *after)
{
	TreeBase *tree = node->superNode;
	if (tree)
	{
		TreeBase *prev = node->prevNode;
		TreeBase *next = node->nextNode;
		if (prev) prev->nextNode = next;
		if (next) next->prevNode = prev;
		if (tree->firstSubnode == node) tree->firstSubnode = next;
		if (tree->lastSubnode == node) tree->lastSubnode = prev;
	}
	
	node->prevNode = after;
	TreeBase *before = after->nextNode;
	node->nextNode = before;
	
	if (before) before->prevNode = node;
	else lastSubnode = node;
	
	after->nextNode = node;
	node->superNode = this;
}

void TreeBase::RemoveSubnode(TreeBase *subnode)
{
	TreeBase *prev = subnode->prevNode;
	TreeBase *next = subnode->nextNode;
	if (prev) prev->nextNode = next;
	if (next) next->prevNode = prev;
	
	if (firstSubnode == subnode) firstSubnode = next;
	if (lastSubnode == subnode) lastSubnode = prev;
	
	subnode->prevNode = nullptr;
	subnode->nextNode = nullptr;
	subnode->superNode = nullptr;
}

void TreeBase::Detach(void)
{
	if (superNode) superNode->RemoveSubnode(this);
}

// ZYURVUR
