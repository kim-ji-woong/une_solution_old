#include "stdafx.h"
#include "Manager.h"
#include <algorithm>
#include "PathItem.h"
#include "ISpace.h"
#include "IWall.h"
#include "Line2D.h"
#include <vector>
#include "POI.h"
#include "PolygonBuilder.h"
#include "Polygon.h"

using namespace VectorGraphics;

namespace SpaceMaker
{
	Manager::Manager()
	{
	}


	Manager::~Manager()
	{
	}

	void Manager::AddWall(IWall* pWall)
	{
		m_walls.push_back(pWall);
	}

	void Manager::AddSpace(ISpace* pSpace)
	{
		m_spaces.push_back(pSpace);
	}

	void SetShapeItems(const Vertex2D& vBegin, const Vertex2D& vEnd, IWall* pWall, std::vector<PathItem*>& shapeCenterItems)
	{
		PathItem* item = new PathItem();
		item->SetWall(pWall);

		//if (wall.GetGridType() == Wall.GridType.Line)
			item->SetLine(new Line2D(vBegin, vEnd), 0);
		
			shapeCenterItems.push_back(item);
	}

	bool MakeCenterLineBoundary(ISpace* pSpace, std::vector<PathItem*>& shapeCenterItems)
	{
		shapeCenterItems.clear();

		Vertex2D* vNext = 0;
		//Vertex2D vTL = null;
		//Vertex2D vBR = null;

		int nWallCount = pSpace->GetWallCount();

		for (int i = 0; i < nWallCount; i++)
		{
			IWall* pWall = pSpace->GetWall(i);

			Vertex2D& vBegin1 = (Vertex2D&)pWall->GetBegin();
			Vertex2D& vEnd1 = (Vertex2D&)pWall->GetEnd();

			if (vNext == 0)
			{
				IWall* pWall2 = pSpace->GetWall(i + 1);

				const Vertex2D& vBegin2 = pWall2->GetBegin();
				const Vertex2D& vEnd2 = pWall2->GetEnd();

				if (vEnd1.GetDistance(vBegin2) < 0.1 || vEnd1.GetDistance(vEnd2) < 0.1)
				{
					SetShapeItems(vBegin1, vEnd1, pWall, shapeCenterItems);
					vNext = &vEnd1;
				}
				else
				{
					SetShapeItems(vEnd1, vBegin1, pWall, shapeCenterItems);
					vNext = &vBegin1;
				}
			}
			else
			{
				if (vNext->GetDistance(vBegin1) < 0.1)
				{
					SetShapeItems(vBegin1, vEnd1, pWall, shapeCenterItems);
					vNext = &vEnd1;
				}
				else
				{
					SetShapeItems(vEnd1, vBegin1, pWall, shapeCenterItems);
					vNext = &vBegin1;
				}
			}
		}

		return true;
	}

	double GetWallThick(int nPathIndex, std::vector<PathItem*>& items)
	{
		int nIndex = nPathIndex;

		do
		{
			PathItem* item = items[nIndex--];
			IWall* pWall = item->GetWall();

			if (pWall != 0 && pWall->GetThick() > 0.0)
				return pWall->GetThick();

			if (nIndex < 0)
				nIndex = (int)(items.size() - 1);
		} while (nIndex != nPathIndex);

		// 모든 벽의 두께가 0일 경우 기본벽 두께를 30cm로 정한다.
		return 30;
	}

	bool MakeInnerLineBoundary(std::vector<PathItem*>& shapeInnerItems, std::vector<PathItem*>& shapeCenterItems)
	{
		shapeInnerItems.clear();

		// m_shapeCenterItems에는 Arc나 EArc가 포함되어 있을수 있으므로 Arc와 EAr를 임시로 직선으로 변환시킨다.
		double dSum = 0.0;
		Vertex2D vBegin, vEnd, vFirst;
		Vertex2D vPrev;

		int nVertexCount = 0;
		int nPathCount = (int)shapeCenterItems.size();

		for (int i = 0; i<nPathCount; i++)
		{
			PathItem* path = shapeCenterItems[i];

			if (path->GetVertex(vBegin, vEnd) == false)
				return false;

			if (i == 0)
			{
				vFirst = vBegin;
				vPrev = vBegin;
				nVertexCount++;
			}

			dSum += (vEnd.x - vPrev.x) * (vEnd.y + vPrev.y);
			vPrev = vEnd;
			nVertexCount++;
		}

		if (nVertexCount < 3)
			return false;

		dSum += (vFirst.x - vPrev.x) * (vFirst.y + vPrev.y);

		bool isClockWise = dSum > 0.0;

		PathItem* prev = shapeCenterItems[0];
		double dPrevWallThick = GetWallThick(0, shapeCenterItems);
		PathItem* prevItem = 0;

		for (int i = 1; i <= nPathCount; i++)
		{
			int nIndex = i < nPathCount ? i : 0;

			PathItem* path = shapeCenterItems[nIndex];
			double dWallThick = GetWallThick(nIndex, shapeCenterItems);

			PathItem* item1 = prevItem == 0 ? prev->Offset(dPrevWallThick / 2, isClockWise) : prevItem;
			PathItem* item2 = i < nPathCount ? path->Offset(dWallThick / 2, isClockWise) : shapeInnerItems[0];

			if (i == 1)
				shapeInnerItems.push_back(item1);

			if (i < nPathCount)
				shapeInnerItems.push_back(item2);

			int nItem1Index = (int)(shapeInnerItems.size() - 2);
			int nResult = PathItem::CalcIntersection(item1, item2, shapeInnerItems, nItem1Index);

			if (nResult == 0)
				return false;

			prev = path;
			prevItem = item2;
			dPrevWallThick = dWallThick;
		}

		// 계산결과는 PathItem의 m_innerXXX에 저장되어 있는데 이 정보를 모두 m_XXX으로 옮긴다.
		for (std::vector<PathItem*>::iterator iter = shapeInnerItems.begin(); iter != shapeInnerItems.end(); iter++)
		{
			PathItem* item = *iter;
			item->InnerToCenter();
		}

		return true;
	}

	void AddPath(std::vector<Vertex2D>& vertices, PathItem* item)
	{
		//if (item.GetDrawType() == PathItem.DrawType.Line)
		{
			Vertex2D vBegin, vEnd;
			item->GetVertex(vBegin, vEnd);

			int nVertexCount = vertices.size();

			if (nVertexCount == 0)
			{
				vertices.push_back(vBegin);
				vertices.push_back(vEnd);
			}
			else
			{
				const Vertex2D& vLast = vertices[nVertexCount - 1];

				if (vBegin.GetDistance(vLast) > 0.001)
					vertices.push_back(vBegin);

				vertices.push_back(vEnd);
			}
		}
	}

	bool MakeGraphicsPath(std::vector<PathItem*>& items, std::vector<Vertex2D>& vertices)
	{
		int nItemCount = (int)items.size();

		for (int i = 0; i < nItemCount;i++)
		{
			PathItem* item = items[i];
			AddPath(vertices, item);
		}

		return true;
	}

	bool CalcSpace(ISpace* pSpace, std::map<ISpace*, std::vector<PathItem*>>& mapInnerItems)
	{
		std::vector<PathItem*> shapeCenterItems, shapeInnerItems;

		if (MakeCenterLineBoundary(pSpace, shapeCenterItems) == false)
			return false;

		if (MakeInnerLineBoundary(shapeInnerItems, shapeCenterItems) == false)
			return false;

		std::vector<Vertex2D> vecVertices;

		bool result = MakeGraphicsPath(shapeInnerItems, vecVertices);

		int nCenterItemCount = (int)shapeCenterItems.size();
		int nInnerItemCount = (int)shapeInnerItems.size();

		for (int i = 0; i < nCenterItemCount; i++)
		{
			PathItem* pItem = shapeCenterItems[i];
			delete pItem;
		}

		mapInnerItems[pSpace] = shapeInnerItems;
		/*for (int i = 0; i < nInnerItemCount; i++)
		{
			PathItem* pItem = shapeInnerItems[i];
			delete pItem;
		}*/

		int nVertexCount = (int)vecVertices.size();

		for (int i = 0; i < nVertexCount; i++)
		{
			pSpace->AddBoundaryVertex(vecVertices[i]);
		}
		
		return result;
	}

	VertexList* MakeOutsidePolygon(std::list<IWall*>& walls, std::list<IWall*>& lineWalls, int& rPolygonCount, int& rLineCount, Line2D*& lines)
	{
		PolygonBuilder polygonBuilder;

		for (std::list<IWall*>::iterator iter = walls.begin(); iter != walls.end();iter++)
		{
			IWall* pWall = *iter;

			if (pWall->GetLinkedSpaceCount() == 1)
			{
				//if (wall.GetGridType() == Wall.GridType.Line)
				{
					lineWalls.push_back(pWall);
					polygonBuilder.AddLine(pWall->GetBegin(), pWall->GetEnd());
				}
			}
		}

		VertexList* polygons = 0;
		polygonBuilder.MakePolygon(rPolygonCount, polygons, rLineCount, lines);
		return polygons;
	}

	extern bool IsInclude(const Vertex2D& rVertex, const Vertex2D& vBegin, const Vertex2D& vEnd);

	IWall* FindLineWall(std::list<IWall*>& lineWalls, const Vertex2D& v1, const Vertex2D& v2)
	{
		for (std::list<IWall*>::iterator iter = lineWalls.begin(); iter != lineWalls.end();iter++)
		{
			IWall* pWall = *iter;

			if (IsInclude(v1, pWall->GetBegin(), pWall->GetEnd()) && IsInclude(v2, pWall->GetBegin(), pWall->GetEnd()))
				return pWall;
		}

		return 0;
	}

	void MakeOutsideCenterline(VertexList& polygon, std::list<IWall*>& lineWalls, std::vector<PathItem*>& centerItems)
	{
		int nVertexCount = (int)polygon.Vertices.size();

		if (nVertexCount < 3)
			return;

		std::list<Vertex2D>::iterator iter = polygon.Vertices.end();
		iter--;

		Vertex2D* pPrev = &(*iter);
		iter = polygon.Vertices.begin();

		for (int i = 0; i < nVertexCount; i++)
		{
			const Vertex2D& vBegin = *pPrev;
			Vertex2D& vertex = *iter;
			iter++;

			IWall* pWall = FindLineWall(lineWalls, vBegin, vertex);

			if (pWall == 0)
			{
				//System.Diagnostics.Trace.WriteLine("No Wall Error");
				return;
			}

			PathItem* item = new PathItem();
			item->SetLine(new Line2D(vBegin, vertex), 0);
			item->SetWall(pWall);

			centerItems.push_back(item);

			pPrev = &vertex;
		}
	}

	bool IsClockwise(VertexList& polygon)
	{
		double dSum = 0;
		int nVertexCount = (int)polygon.Vertices.size();

		if (nVertexCount < 3)
			return false;

		std::list<Vertex2D>::iterator iter = polygon.Vertices.end();
		iter--;

		Vertex2D* prev = &(*iter);
		iter = polygon.Vertices.begin();

		for (int i = 0; i < nVertexCount; i++)
		{
			Vertex2D& vertex = *iter;

			dSum += (vertex.x - prev->x) * (vertex.y + prev->y);
			prev = &vertex;
		}

		return dSum > 0.0;
	}

	void SetOutsideBoundary(VertexList& polygon, std::vector<PathItem*>& centerItems, std::map<IWall*, std::vector<PathItem*>*>& mapWallOutsideBoundaryPath)
	{
		bool isClockWise = IsClockwise(polygon);

		int nPathCount = (int)centerItems.size();
		PathItem* prev = centerItems[0];
		double dPrevWallThick = GetWallThick(0, centerItems);
		PathItem* prevItem = 0;

		std::vector<PathItem*>* innerItems = new std::vector<PathItem*>();

		for (int i = 1; i <= nPathCount; i++)
		{
			int nIndex = i < nPathCount ? i : 0;
			PathItem* path = centerItems[nIndex];
			double dWallThick = GetWallThick(nIndex, centerItems);

			PathItem* item1 = prevItem == 0 ? prev->Offset(-dPrevWallThick / 2, isClockWise) : prevItem;
			PathItem* item2 = i < nPathCount ? path->Offset(-dWallThick / 2, isClockWise) : (*innerItems)[0];

			if (i == 1)
				innerItems->push_back(item1);

			if (i < nPathCount)
				innerItems->push_back(item2);

			int nItem1Index = (int)(innerItems->size() - 2);
			int nResult = PathItem::CalcIntersection(item1, item2, *innerItems, nItem1Index);

			if (nResult == 0)
				return;

			prev = path;
			prevItem = item2;
			dPrevWallThick = dWallThick;
		}

		int nInnerCount = (int)innerItems->size();

		for (int i = 0; i < nInnerCount;i++)
		{
			PathItem* item = (*innerItems)[i];
			item->InnerToCenter();
		}

		int nCenterCount = (int)centerItems.size();

		for (int i = 0; i < nCenterCount;i++)
		{
			PathItem* item = centerItems[i];
			IWall* pWall = item->GetWall();

			if (pWall == 0)
				continue;

			mapWallOutsideBoundaryPath[pWall] = innerItems;
		}
	}

	PathItem* GetPathItem(IWall* pWall, std::vector<PathItem*>& items)
	{
		int nItemCount = (int)items.size();

		for (int i = 0; i < nItemCount; i++)
		{
			PathItem* item = items[i];

			if (item->GetWall() == pWall)
				return item;
		}

		return 0;
	}

	Vertex2D GetNearestVertex(const Vertex2D& rVertex, const Vertex2D& vLineBegin, const Vertex2D& vLineEnd, bool noLimit);

	void GetLineNearVertex(IWall* pWall, PathItem* item, Vertex2D& vBegin, Vertex2D& vEnd)
	{
		const Vertex2D& wallBegin = pWall->GetBegin();
		const Vertex2D& wallEnd = pWall->GetEnd();

		Vertex2D begin, end;

		if (item->GetVertex(begin, end) == false)
			return;

		Vertex2D vB = GetNearestVertex(wallBegin, begin, end, true);
		Vertex2D vE = GetNearestVertex(wallEnd, begin, end, true);

		bool includeBegin = IsInclude(vB, begin, end);
		bool includeEnd = IsInclude(vE, begin, end);

		if (includeBegin && includeEnd)
		{
			double dLen1 = begin.GetDistance(vB);
			double dLen2 = begin.GetDistance(vE);

			if (dLen1 < dLen2)
			{
				vBegin.x = begin.x;
				vBegin.y = begin.y;
				vEnd.x = end.x;
				vEnd.y = end.y;
			}
			else
			{
				vBegin.x = end.x;
				vBegin.y = end.y;
				vEnd.x = begin.x;
				vEnd.y = begin.y;
			}
		}
		else if (includeBegin == false && includeEnd)
		{
			double dLen1 = vB.GetDistance(begin);
			double dLen2 = vB.GetDistance(end);

			if (dLen1 < dLen2)
			{
				vBegin.x = begin.x;
				vBegin.y = begin.y;
				vEnd.x = end.x;
				vEnd.y = end.y;
			}
			else
			{
				vBegin.x = end.x;
				vBegin.y = end.y;
				vEnd.x = begin.x;
				vEnd.y = begin.y;
			}
		}
		else if (includeEnd == false && includeBegin)
		{
			double dLen1 = vE.GetDistance(begin);
			double dLen2 = vE.GetDistance(end);

			if (dLen1 < dLen2)
			{
				vBegin.x = end.x;
				vBegin.y = end.y;
				vEnd.x = begin.x;
				vEnd.y = begin.y;
			}
			else
			{
				vBegin.x = begin.x;
				vBegin.y = begin.y;
				vEnd.x = end.x;
				vEnd.y = end.y;
			}
		}
		else// if (includeBegin == false && includeEnd == false)
		{
			double dBB = vB.GetDistance(begin);
			double dBE = vB.GetDistance(end);
			double dEB = vE.GetDistance(begin);
			double dEE = vE.GetDistance(end);

			if (dBB < dBE && dEB < dEE)
			{
				// 둘다 begin쪽에 있는 경우
				if (dBB > dEB)
				{
					vBegin.x = begin.x;
					vBegin.y = begin.y;
					vEnd.x = end.x;
					vEnd.y = end.y;
				}
				else
				{
					vBegin.x = end.x;
					vBegin.y = end.y;
					vEnd.x = begin.x;
					vEnd.y = begin.y;
				}
			}
			else if (dBB > dBE && dEB > dEE)
			{
				// 둘다 end쪽에 있는 경우
				if (dEE > dBE)
				{
					vBegin.x = begin.x;
					vBegin.y = begin.y;
					vEnd.x = end.x;
					vEnd.y = end.y;
				}
				else
				{
					vBegin.x = end.x;
					vBegin.y = end.y;
					vEnd.x = begin.x;
					vEnd.y = begin.y;
				}
			}
			else
			{
				// 두 점의 위치가 서로 다른 경우
				if (dBB < dBE)
				{
					vBegin.x = begin.x;
					vBegin.y = begin.y;
					vEnd.x = end.x;
					vEnd.y = end.y;
				}
				else
				{
					vBegin.x = end.x;
					vBegin.y = end.y;
					vEnd.x = begin.x;
					vEnd.y = begin.y;
				}
			}
		}
	}

	void GetNearVertex(IWall* pWall, std::vector<PathItem*>& items, Vertex2D& vBegin, Vertex2D& vEnd)
	{
		PathItem* item = GetPathItem(pWall, items);

		if (item == 0)
			return;

		GetLineNearVertex(pWall, item, vBegin, vEnd);
	}

	/*void GetNearVertex(IWall* pWall, std::vector<PathItem*>& items, Vertex2D& vBegin, Vertex2D& vEnd)
	{
		bool noBegin = true, noEnd = true;
		double lenBegin = 0.0, lenEnd = 0.0;

		const Vertex2D& wallBegin = pWall->GetBegin();
		const Vertex2D& wallEnd = pWall->GetEnd();

		int nItemCount = (int)items.size();

		for (int i = 0; i < nItemCount;i++)
		{
			PathItem* item = items[i];
			Vertex2D vB, vE;

			if (item->GetVertex(vB, vE) == false)
				continue;

			double len1 = wallBegin.GetDistance(vB);
			double len2 = wallEnd.GetDistance(vB);

			if (noBegin == true || len1 < lenBegin)
			{
				vBegin = vB;
				lenBegin = len1;
				noBegin = false;
			}

			if (noEnd == true || len2 < lenEnd)
			{
				vEnd = vB;
				lenEnd = len2;
				noEnd = false;
			}
		}
	}*/

	void AddVertexToPolygonBuilder(std::vector<PathItem*>& items, PolygonBuilder& polygonBuilder)
	{
		Vertex2D vBegin, vEnd, vMiddle;

		int nItemCount = (int)items.size();

		for (int i = 0; i < nItemCount;i++)
		{
			PathItem* item = items[i];

			if (item->GetVertex(vBegin, vEnd) == false)
				continue;

			polygonBuilder.AddLine(vBegin, vEnd);
		}
	}

	VertexList* MakeWallPolygons(IWall* pWall, int& rPolygonCount, int& rLineCount, Line2D*& lines, std::vector<PathItem*>& items1, std::vector<PathItem*>& items2, const Vertex2D& vBeginNear1, const Vertex2D& vBeginNear2, const Vertex2D& vEndNear1, const Vertex2D& vEndNear2)
	{
		PolygonBuilder polygonBuilder;

		AddVertexToPolygonBuilder(items1, polygonBuilder);
		AddVertexToPolygonBuilder(items2, polygonBuilder);

		const Vertex2D& vBegin = pWall->GetBegin();
		const Vertex2D& vEnd = pWall->GetEnd();

		polygonBuilder.AddLine(vBegin, vBeginNear1);
		polygonBuilder.AddLine(vBegin, vBeginNear2);
		polygonBuilder.AddLine(vEnd, vEndNear1);
		polygonBuilder.AddLine(vEnd, vEndNear2);

		VertexList* polygons = 0;
		polygonBuilder.MakePolygon(rPolygonCount, polygons, rLineCount, lines);

		return polygons;
	}

	Vertex2D GetMiddleVertex(IWall* pWall)
	{
		Vertex2D vMiddle;

		if (pWall->GetGridType() == IWall::GridType::Line)
		{
			vMiddle = (pWall->GetBegin() + pWall->GetEnd()) / 2;
		}

		return vMiddle;
	}

	bool HitTest(VertexList& rVertexList, const Vertex2D& vertex)
	{
		if (rVertexList.Vertices.size() < 3)
			return false;

		std::list<Vertex2D>::iterator iter = rVertexList.Vertices.end();
		iter--;
		Vertex2D* pPrev = &(*iter);

		Polygon polygon;

		//IsInclude()

		for (iter = rVertexList.Vertices.begin(); iter != rVertexList.Vertices.end(); iter++)
		{
			Vertex2D& rVertex = *iter;
			polygon.AddVertex(rVertex);

			// 경계에 걸쳐진 버텍스는 무시한다.
			if (IsInclude(vertex, rVertex, *pPrev))
				return false;

			pPrev = &rVertex;
		}

		return polygon.HitTest(vertex);
	}

	Vertex2D* GetLast(VertexList& polygon)
	{
		if (polygon.Vertices.size() == 0)
			return 0;

		std::list<Vertex2D>::iterator iter = polygon.Vertices.end();
		iter--;
		return &(*iter);
	}

	extern double GetAngle(const Vertex2D& v1, const Vertex2D& vCenter, const Vertex2D& v2);
	extern int IsRightSideFromLine(const Vertex2D& rVertex, const Vertex2D& vBegin, const Vertex2D& vEnd);

	Vertex2D GetNearestVertex(const Vertex2D& rVertex, const Vertex2D& vLineBegin, const Vertex2D& vLineEnd, bool noLimit)
	{
		double dLen = rVertex.GetDistance(vLineBegin);
		double dLen2 = rVertex.GetDistance(vLineEnd);

		if (dLen <= 0.001 || dLen2 <= 0.001)
			return rVertex;

		double dAngle = GetAngle(rVertex, vLineBegin, vLineEnd);
		double dH = dLen * cos(dAngle);

		Vertex2D vertex = vLineBegin.GetLinearVertex(vLineEnd, dH);

		if (noLimit || IsInclude(vertex, vLineBegin, vLineEnd))
		{
			return vertex;
		}

		return dLen < dLen2 ? vLineBegin : vLineEnd;
	}

	void ReshapeLineVertex(const Vertex2D& vLineBegin, const Vertex2D& vLineEnd, Vertex2D& vBegin1, Vertex2D& vBegin2)
	{
		Vertex2D v1 = GetNearestVertex(vBegin1, vLineBegin, vLineEnd, true);
		Vertex2D v2 = GetNearestVertex(vBegin2, vLineBegin, vLineEnd, true);

		double len1 = vLineEnd.GetDistance(vBegin1);
		double len2 = vLineEnd.GetDistance(vBegin2);
		double len3 = vLineEnd.GetDistance(vLineBegin);

		if (len1 > len2 && len1 > len3)
		{
			if (IsRightSideFromLine(vBegin1, vLineBegin, vLineEnd) == 1)
				vBegin2 = v1.GetRightVertex(vLineEnd, -v2.GetDistance(vBegin2));
			else
				vBegin2 = v1.GetRightVertex(vLineEnd, v2.GetDistance(vBegin2));
		}
		else if (len2 > len1 && len2 > len3)
		{
			if (IsRightSideFromLine(vBegin2, vLineBegin, vLineEnd) == 1)
				vBegin1 = v2.GetRightVertex(vLineEnd, -v1.GetDistance(vBegin1));
			else
				vBegin1 = v2.GetRightVertex(vLineEnd, v1.GetDistance(vBegin1));
		}
		else// if (len3 >= len1 && len3 >= len2)
		{
			if (IsRightSideFromLine(vBegin1, vLineBegin, vLineEnd) == 1)
			{
				vBegin1 = vLineBegin.GetRightVertex(vLineEnd, v1.GetDistance(vBegin1));
				vBegin2 = vLineBegin.GetRightVertex(vLineEnd, -v2.GetDistance(vBegin2));
			}
			else
			{
				vBegin1 = vLineBegin.GetRightVertex(vLineEnd, -v1.GetDistance(vBegin1));
				vBegin2 = vLineBegin.GetRightVertex(vLineEnd, v2.GetDistance(vBegin2));
			}
		}
	}

	// 벽체 외곽영역 폴리곤을 벽체의 양끝점을 기준으로 좀더 매끈하게 다듬는다.
	// (벽체 양끝을 뭉뚝하게 만든다.)
	VertexList ReshapePolygon(IWall* pWall, VertexList& polygon)
	{
		const Vertex2D& vBegin = pWall->GetBegin();
		const Vertex2D& vEnd = pWall->GetEnd();

		VertexList vertices1, vertices2;
		VertexList* vertices = &vertices1;

		int nVertexCount = (int)polygon.Vertices.size();
		int nBeginIndex = -1, nEndIndex = -1;

		std::list<Vertex2D>::iterator iter = polygon.Vertices.begin();

		for (int i = 0; i<nVertexCount; i++)
		{
			const Vertex2D& vertex = *iter;
			iter++;

			if (vBegin.GetDistance(vertex) < 0.1)
			{
				nBeginIndex = i;

				if (nEndIndex >= 0)
					break;
			}
			else if (vEnd.GetDistance(vertex) < 0.1)
			{
				nEndIndex = i;

				if (nBeginIndex >= 0)
					break;
			}
		}

		if (nBeginIndex < 0 || nEndIndex < 0)
			return polygon;

		int nIndex = nBeginIndex;

		iter = polygon.Vertices.begin();

		for (int i = 0; i < nIndex; i++)
		{
			iter++;
		}

		do
		{
			if (++nIndex >= nVertexCount)
			{
				nIndex = 0;
				iter = polygon.Vertices.begin();
			}
			else
				iter++;

			const Vertex2D& vertex = *iter;

			if (vEnd.GetDistance(vertex) < 0.1)
				vertices = &vertices2;
			else if (vBegin.GetDistance(vertex) >= 0.1)
			{
				vertices->Vertices.push_back(vertex);
			}
		}
		while (nIndex != nBeginIndex);

		int nVertexCount1 = (int)vertices1.Vertices.size();
		int nVertexCount2 = (int)vertices2.Vertices.size();

		if (nVertexCount1 < 2 || nVertexCount2 < 2)
			return polygon;

		Vertex2D& vBegin1 = *vertices1.Vertices.begin();
		Vertex2D& vEnd1 = *GetLast(vertices1);
		Vertex2D& vBegin2 = *GetLast(vertices2);
		Vertex2D& vEnd2 = *vertices2.Vertices.begin();

		//if (m_gridType == GridType.Line)
		{
			ReshapeLineVertex(vBegin, vEnd, vBegin1, vBegin2);
			ReshapeLineVertex(vEnd, vBegin, vEnd1, vEnd2);
		}
		
		VertexList newPolygon;

		for (iter = vertices1.Vertices.begin(); iter != vertices1.Vertices.end(); iter++)
		{
			newPolygon.Vertices.push_back(*iter);
		}

		for (iter = vertices2.Vertices.begin(); iter != vertices2.Vertices.end(); iter++)
		{
			newPolygon.Vertices.push_back(*iter);
		}

		return newPolygon;
	}

	void PolygonToPathItems(VertexList& polygon, std::vector<PathItem*>& items)
	{
		int nVertexCount = (int)polygon.Vertices.size();
		Vertex2D* prev = GetLast(polygon);

		std::list<Vertex2D>::iterator iter = polygon.Vertices.begin();

		for (int i = 0; i<nVertexCount; i++)
		{
			Vertex2D& vertex = *iter;
			iter++;

			PathItem* item = new PathItem();
			item->SetLine(new Line2D(*prev, vertex), 0);
			items.push_back(item);
			prev = &vertex;
		}
	}

	bool SelectBoundaryPolygon(IWall* pWall, VertexList* polygons, int nPolygonCount)
	{
		VertexList* selectedPolygon = 0;

		if (nPolygonCount == 1)
		{
			selectedPolygon = &polygons[0];
		}
		else
		{
			Vertex2D vMiddle = GetMiddleVertex(pWall);

			for (int i = 0; i < nPolygonCount;i++)
			{
				VertexList& polygon = polygons[i];

				if (HitTest(polygon, vMiddle))
				{
					selectedPolygon = &polygon;
					break;
				}
			}
		}

		if (selectedPolygon == 0)
			return false;

		VertexList newPolygon = ReshapePolygon(pWall, *selectedPolygon);

		std::vector<PathItem*> boundary;
		PolygonToPathItems(newPolygon, boundary);
		
		std::vector<Vertex2D> vecVertices;
		
		if (MakeGraphicsPath(boundary, vecVertices))
		{
			int nVertexCount = (int)vecVertices.size();

			for (int i = 0; i < nVertexCount; i++)
			{
				pWall->AddBoundaryVertex(vecVertices[i]);
			}
		}

		int nBoundaryCount = (int)boundary.size();

		for (int i = 0; i < nBoundaryCount; i++)
		{
			delete boundary[i];
		}

		return true;
	}

	bool MakeWallShape(IWall* pWall, std::map<IWall*, std::vector<PathItem*>*>& mapWallOutsideBoundaryPath, std::map<ISpace*, std::vector<PathItem*>>& mapInnerItems)
	{
		std::vector<PathItem*>* items1 = 0;
		std::vector<PathItem*>* items2 = 0;

		int nLinkedSpaceCount = pWall->GetLinkedSpaceCount();

		if (nLinkedSpaceCount == 0)
			return true;
		else if (nLinkedSpaceCount == 1)
		{
			ISpace* pSpace = pWall->GetLinkedSpace(0);
			std::map<ISpace*, std::vector<PathItem*>>::iterator iter1 = mapInnerItems.find(pSpace);
			std::map<IWall*, std::vector<PathItem*>*>::iterator iter2 = mapWallOutsideBoundaryPath.find(pWall);

			if (iter1 == mapInnerItems.end() || iter2 == mapWallOutsideBoundaryPath.end())
				return false;

			items1 = &iter1->second;
			items2 = iter2->second;
		}
		else// if (m_linkedSpaces.Count == 2)
		{
			ISpace* pSpace1 = pWall->GetLinkedSpace(0);
			ISpace* pSpace2 = pWall->GetLinkedSpace(1);

			std::map<ISpace*, std::vector<PathItem*>>::iterator iter1 = mapInnerItems.find(pSpace1);
			std::map<ISpace*, std::vector<PathItem*>>::iterator iter2 = mapInnerItems.find(pSpace2);

			if (iter1 == mapInnerItems.end() || iter2 == mapInnerItems.end())
				return false;

			items1 = &iter1->second;
			items2 = &iter2->second;
		}

		Vertex2D vBeginNear1, vBeginNear2, vEndNear1, vEndNear2;

		GetNearVertex(pWall, *items1, vBeginNear1, vEndNear1);
		GetNearVertex(pWall, *items2, vBeginNear2, vEndNear2);

		int nPolygonCount, nLineCount;
		Line2D* lines = 0;
		VertexList* polygons = MakeWallPolygons(pWall, nPolygonCount, nLineCount, lines, *items1, *items2, vBeginNear1, vBeginNear2, vEndNear1, vEndNear2);

		if (polygons == 0)
			return false;

		bool result = SelectBoundaryPolygon(pWall, polygons, nPolygonCount);

		delete[] lines;
		delete[] polygons;
		return result;
	}

	bool Manager::Calc()
	{
		std::map<ISpace*, std::vector<PathItem*>> mapInnerItems;

		for (std::list<ISpace*>::iterator iter = m_spaces.begin(); iter != m_spaces.end(); iter++)
		{
			if (CalcSpace(*iter, mapInnerItems) == false)
				return false;
		}

		return MakeOutsideWallLine(&mapInnerItems);
	}

	void DeleteInnerItems(std::map<ISpace*, std::vector<PathItem*>>& mapInnerItems)
	{
		for (std::map<ISpace*, std::vector<PathItem*>>::iterator iter = mapInnerItems.begin(); iter != mapInnerItems.end(); iter++)
		{
			std::vector<PathItem*>& vecPathItems = iter->second;
			int nItemCount = (int)vecPathItems.size();

			for (int i = 0; i < nItemCount; i++)
			{
				PathItem* item = vecPathItems[i];
				delete item;
			}
		}
	}

	bool Manager::MakeOutsideWallLine(void* arg)
	{
		std::map<ISpace*, std::vector<PathItem*>>* mapInnerItems = (std::map<ISpace*, std::vector<PathItem*>>*)arg;

		int nPolygonCount, nLineCount;
		std::list<IWall*> lineWalls;
		Line2D* lines = 0;
		VertexList* polygons = MakeOutsidePolygon(m_walls, lineWalls, nPolygonCount, nLineCount, lines);

		if (polygons == 0)
		{
			DeleteInnerItems(*mapInnerItems);
			return false;
		}

		std::map<IWall*, std::vector<PathItem*>*> mapWallOutsideBoundaryPath;

		for (int i = 0; i < nPolygonCount;i++)
		{
			VertexList& polygon = polygons[i];
			std::vector<PathItem*> centerItems;
			MakeOutsideCenterline(polygon, lineWalls, centerItems);

			SetOutsideBoundary(polygon, centerItems, mapWallOutsideBoundaryPath);
		}

		for (std::list<IWall*>::iterator iter = m_walls.begin(); iter != m_walls.end(); iter++)
		{
			IWall* pWall = *iter;
			MakeWallShape(pWall, mapWallOutsideBoundaryPath, *mapInnerItems);
		}

		DeleteInnerItems(*mapInnerItems);
		/*for (std::map<ISpace*, std::vector<PathItem*>>::iterator iter = mapInnerItems->begin(); iter != mapInnerItems->end(); iter++)
		{
			std::vector<PathItem*>& vecPathItems = iter->second;
			int nItemCount = (int)vecPathItems.size();

			for (int i = 0; i < nItemCount; i++)
			{
				PathItem* item = vecPathItems[i];
				delete item;
			}
		}*/

		for (std::map<IWall*, std::vector<PathItem*>*>::iterator iter = mapWallOutsideBoundaryPath.begin(); iter != mapWallOutsideBoundaryPath.end(); iter++)
		{
			std::vector<PathItem*>* vecPathItems = iter->second;
			int nItemCount = (int)vecPathItems->size();

			for (int i = 0; i < nItemCount; i++)
			{
				PathItem* item = (*vecPathItems)[i];
				delete item;
			}

			// 모두 같은 vector를 공유한다.
			vecPathItems->clear();
			delete vecPathItems;
			break;
		}

		return true;
	}
}
