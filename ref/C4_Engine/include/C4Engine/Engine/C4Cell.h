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


#ifndef C4Cell_h
#define C4Cell_h


//# \component	World Manager
//# \prefix		WorldMgr/


#include "C4Bounding.h"


namespace C4
{
	class Site;
	class CellGraph;
	
	
	class C4_API Bond : public GraphEdge<Site, Bond>, public Memory<Bond>
	{
		public:
			
			Bond(Site *start, Site *finish);
	};
	
	
	class C4_API Site : public GraphElement<Site, Bond>
	{
		private:
			
			int32		cellIndex;
			Box3D		worldBoundingBox;
		
		public:
			
			Site(int32 index = 0)
			{
				cellIndex = index;
			}
			
			int32 GetCellIndex(void) const
			{
				return (cellIndex);
			}
			
			const Box3D& GetWorldBoundingBox(void) const
			{
				return (worldBoundingBox);
			}
			
			void SetWorldBoundingBox(const Box3D& box)
			{
				worldBoundingBox = box;
			}
			
			void SetWorldBoundingBox(const Point3D& pmin, const Point3D& pmax)
			{
				worldBoundingBox.min = pmin;
				worldBoundingBox.max = pmax;
			}
			
			#if C4SIMD
			
				void SetWorldBoundingBox(float4 pmin, float4 pmax)
				{
					SimdStore3D(pmin, &worldBoundingBox.min.x);
					SimdStore3D(pmax, &worldBoundingBox.max.x);
				}
			
			#endif
	};
	
	
	inline Bond::Bond(Site *start, Site *finish) : GraphEdge<Site, Bond>(start, finish)
	{
	}
	
	
	class C4_API Cell : public Site, public Memory<Cell>
	{
		friend class CellGraph;
		
		private:
			
			Cell	*subcell[4];
			
			Cell();
		
		public:
			
			Cell(CellGraph *graph, Cell *superCell, int32 index);
			~Cell();
	};
	
	
	class C4_API CellGraph : public Graph<Site, Bond>
	{
		private:
			
			Site			*superSite;
			
			float			cellSize;
			float			inverseCellSize; 
			float			rootCellSize;
			 
			Integer2D		maxCellCoord; 
			int32			levelCount; 
			
			Cell			rootCell; 
			
			Cell *UpdateCell(Cell *superCell, int32 i, int32 j, float cellSize);
		
		public: 
			
			CellGraph(Site *site);
			~CellGraph();
			 
			void Activate(const Box3D& box, float size);
			
			void AddSite(Site *site);
			void RemoveSite(Site *site);
	};
	
	
	class C4_API CellGraphSite : public CellGraph, public Site
	{
		public:
			
			CellGraphSite();
			~CellGraphSite();
	};
}


#endif

// ZYURVUR
