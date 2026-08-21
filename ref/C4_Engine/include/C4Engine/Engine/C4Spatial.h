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


#ifndef C4Spatial_h
#define C4Spatial_h


//# \component	Utility Library
//# \prefix		Utilities/


#include "C4Memory.h"


namespace C4
{
	enum
	{
		kQuadrantX		= 1 << 0,
		kQuadrantY		= 1 << 1
	};
	
	
	enum
	{
		kOctantX		= 1 << 0,
		kOctantY		= 1 << 1,
		kOctantZ		= 1 << 2
	};
	
	
	template <class type> class Bintree
	{
		private:
			
			Bintree		*subnode[2];
		
		protected:
			
			Bintree();
		
		public:
			
			virtual ~Bintree();
			
			type *GetSubnode(int32 index) const
			{
				return (static_cast<type *>(subnode[index]));
			}
			
			void SetSubnode(int32 index, Bintree *node)
			{
				subnode[index] = node;
			}
			
			virtual void Purge(void);
	};
	
	
	template <class type> Bintree<type>::Bintree()
	{
		subnode[0] = nullptr;
		subnode[1] = nullptr;
	}
	
	template <class type> Bintree<type>::~Bintree()
	{
		delete subnode[0];
		delete subnode[1];
	}
	
	template <class type> void Bintree<type>::Purge(void)
	{
		delete subnode[0];
		delete subnode[1];
		subnode[0] = nullptr;
		subnode[1] = nullptr;
	}
	
	
	template <class type> class Quadtree
	{
		private:
			
			Quadtree	*subnode[4];
		
		protected:
			
			Quadtree();
		
		public:
			
			virtual ~Quadtree();
			
			type *GetSubnode(int32 quadrant) const
			{
				return (static_cast<type *>(subnode[quadrant]));
			}
			
			void SetSubnode(int32 quadrant, Quadtree *node)
			{
				subnode[quadrant] = node;
			}
			
			virtual void Purge(void); 
	};
	 
	 
	template <class type> Quadtree<type>::Quadtree() 
	{
		for (machine a = 0; a < 4; a++) subnode[a] = nullptr; 
	}
	
	template <class type> Quadtree<type>::~Quadtree()
	{ 
		for (machine a = 0; a < 4; a++) delete subnode[a];
	}
	
	template <class type> void Quadtree<type>::Purge(void) 
	{
		for (machine a = 0; a < 4; a++)
		{
			delete subnode[a];
			subnode[a] = nullptr;
		}
	}
	
	
	template <class type> class Octree
	{
		private:
			
			Octree		*subnode[8];
		
		protected:
			
			Octree();
		
		public:
			
			virtual ~Octree();
			
			type *GetSubnode(int32 octant) const
			{
				return (static_cast<type *>(subnode[octant]));
			}
			
			void SetSubnode(int32 octant, Octree *node)
			{
				subnode[octant] = node;
			}
			
			virtual void Purge(void);
	};
	
	
	template <class type> Octree<type>::Octree()
	{
		for (machine a = 0; a < 8; a++) subnode[a] = nullptr;
	}
	
	template <class type> Octree<type>::~Octree()
	{
		for (machine a = 0; a < 8; a++) delete subnode[a];
	}
	
	template <class type> void Octree<type>::Purge(void)
	{
		for (machine a = 0; a < 8; a++)
		{
			delete subnode[a];
			subnode[a] = nullptr;
		}
	}
}


#endif

// ZYURVUR
