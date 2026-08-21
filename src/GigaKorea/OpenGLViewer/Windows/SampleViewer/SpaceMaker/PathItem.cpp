#include "stdafx.h"
#include "PathItem.h"
#include "IWall.h"
#include "Line2D.h"

using namespace VectorGraphics;

namespace SpaceMaker
{
	static const double HALF_PI = 1.57079632679489661923;
	static const double _PI = 3.14159265358979323846;
	static const double _3HALF_PI = 4.71238898038468985769;
	static const double _2PI = 6.28318530717958647692;

	PathItem::PathItem()
	{
		m_pLine = 0;
		m_pInnerLine = 0;
		m_innerPass = false;
		m_pWall = 0;
	}


	PathItem::~PathItem()
	{
		delete m_pLine;
		delete m_pInnerLine;
	}

	void PathItem::SetLine(Line2D* line, VectorGraphics::Vertex2D* vBegin)
	{
		if (vBegin == 0)
		{
			m_pLine = new Line2D(line->GetVertex(true), line->GetVertex(false));
		}
		else
		{
			Vertex2D v1 = line->GetVertex(true);
			Vertex2D v2 = line->GetVertex(false);

			double len1 = v1.GetDistance(*vBegin);
			double len2 = v2.GetDistance(*vBegin);

			if (len1 < len2)
			{
				m_pLine = new Line2D(v1, v2);
			}
			else
			{
				m_pLine = new Line2D(v2, v1);
			}
		}
	}

	bool PathItem::GetVertex(VectorGraphics::Vertex2D& vBegin, VectorGraphics::Vertex2D& vEnd)
	{
		if (m_pLine != 0)
		{
			vBegin = m_pLine->GetVertex(true);
			vEnd = m_pLine->GetVertex(false);
			return true;
		}

		return false;
	}

	PathItem* PathItem::Offset(double offset, bool isClockwise)
	{
		PathItem* item = 0;

		if (m_pLine != 0)
		{
			if (isClockwise == false)
				offset = -offset;

			Vertex2D vBegin = m_pLine->GetVertex(true).GetRightVertex(m_pLine->GetVertex(false), -offset);
			Vertex2D vEnd = m_pLine->GetVertex(false).GetRightVertex(m_pLine->GetVertex(true), offset);

			item = new PathItem();
			item->SetLine(new Line2D(vBegin, vEnd), 0);
		}

		if (item != 0)
			item->m_pWall = m_pWall;

		return item;
	}

	int PathItem::CalcIntersection(PathItem* item1, PathItem* item2, std::vector<PathItem*>& items, int nItem1Index)
	{
		int nIndex = nItem1Index;
		int nResult = 0;

		PathItem* itemOrigin1 = item1;
		PathItem* itemOrigin2 = item2;
		int nItem2Index = 0;

		while (item1 != 0)
		{
			while (item1->m_innerPass)
			{
				nIndex--;

				if (nIndex < 0)
					nIndex = (int)items.size() - 1;

				if (nIndex == nItem1Index)
				{
					//System.Diagnostics.Trace.WriteLine("교차점을 찾을수 없음");
					return 0;
				}

				item1 = items[nIndex];
			}

			//if (item1.m_drawType == DrawType.Line)
			{
				//if (item2.m_drawType == DrawType.Line)
					nResult = CalcIntersectionLineToLine(item1, item2);
				//else if (item2.m_drawType == DrawType.Arc || item2.m_drawType == DrawType.EArc)
				//	nResult = CalcIntersectionLineToEArc(item1, item2);
			}
			
			if (nResult == 1)
				break;
			else if (nResult == 0)
				continue;
			else if (nResult == -1)
			{
				//if (item1.m_drawType == DrawType.Line && item2.m_drawType == DrawType.Line)
				{
					// 두 직선이 한점에서 만나면서 일직선을 이루어야 하는데, 벽체의 두께가 서로 달라서 평행하게 되어버린 경우
					item1->m_pInnerLine = new Line2D(*item1->m_pLine);

					// 두 벽체 사이에 임시 PathItem을 하나 끼워넣는다.
					PathItem* itemTemp = new PathItem();
					itemTemp->SetLine(new Line2D(item1->m_pLine->GetVertex(false), item2->m_pLine->GetVertex(true)), 0);
					itemTemp->m_pInnerLine = new Line2D(*itemTemp->m_pLine);

					int nIndex = (int)(items.size() - 1);
					items.insert(items.begin() + nIndex, itemTemp);
					return 2;
				}

				//System.Diagnostics.Trace.WriteLine("교차점을 찾을수 없음");
				return 0;
			}
			else if (nResult == -2)
			{
				if (itemOrigin2 == items[0])
				{
					do
					{
						nItem2Index++;

						if (nItem2Index >= (int)items.size() || items[nItem2Index] == itemOrigin1)
						{
							//System.Diagnostics.Trace.WriteLine("교차점을 찾을수 없음");
							return 0;
						}

						item2 = items[nItem2Index];
					} while (item2->m_innerPass == false);
				}
				else
					break;
			}
		}

		return 1;
	}

	double GetAngle(const Vertex2D& v1, const Vertex2D& vCenter, const Vertex2D& v2)
	{
		// 코사인 제2법칙
		// C²= A²+ B²- 2ABcosΘ
		double a = vCenter.GetDistance(v1);
		double b = v2.GetDistance(vCenter);
		double c = v2.GetDistance(v1);

		double cosData = (a * a + b * b - c * c) / 2 / a / b;
		if (cosData < -1.0) cosData = -1.0;
		else if (cosData > 1.0) cosData = 1.0;

		return acos(cosData);
	}

	// rLine과 만나지 않으면 0을 리턴한다.
	// 교차점이 하나만 존재할 경우 rVertex1에만 값이 담겨지며 1이 리턴된다.
	// 교차점이 두 개 존재할 경우 rVertex1과 rVertex2에 각각 값이 담겨진다.
	int IntersectLineToLine(Line2D& rLine1, Line2D& rLine2, Vertex2D& rVertex1, Vertex2D& rVertex2)
	{
		double dTolerance = 0.001;

		const Vertex2D& vBegin1 = rLine1.GetVertex(true);
		const Vertex2D& vEnd1 = rLine1.GetVertex(false);
		const Vertex2D& vBegin2 = rLine2.GetVertex(true);
		const Vertex2D& vEnd2 = rLine2.GetVertex(false);

		double dLen1 = vBegin1.GetDistance(vEnd1);
		double dLen2 = vBegin2.GetDistance(vEnd2);

		// Line1이 한 점일 경우
		if (dLen1 < dTolerance)
		{
			if (dLen2 < dTolerance)
			{
				if (vBegin1.GetDistance(vBegin2) < dTolerance)
				{
					rVertex1 = vBegin1;
					return 1;
				}
			}
			else
			{
				double dLen3 = vBegin1.GetDistance(vBegin2);
				double dLen4 = vBegin1.GetDistance(vEnd2);

				if (dLen3 < dTolerance || dLen4 < dTolerance)
				{
					rVertex1 = vBegin1;
					return 1;
				}

				double dAngle = GetAngle(vBegin2, vBegin1, vEnd2);

				//if (rLine2.m_lineType) == ENUM_OF(LineType, SEGMENT))
				{
					if (_PI - dAngle < dTolerance)
					{
						rVertex1 = vBegin1;
						return 1;
					}
				}
			}

			return 0;
		}
		// rLine2가 한 점일 경우
		else if (dLen2 < dTolerance)
		{
			double dLen3 = vBegin2.GetDistance(vBegin1);
			double dLen4 = vBegin2.GetDistance(vEnd1);

			if (dLen3 < dTolerance || dLen4 < dTolerance)
			{
				rVertex1 = vBegin2;
				return 1;
			}

			double dAngle = GetAngle(vBegin1, vBegin2, vEnd1);

			//if (rLine1.m_lineType == ENUM_OF(LineType, SEGMENT))
			{
				if (_PI - dAngle < dTolerance)
				{
					rVertex1 = vBegin2;
					return 1;
				}
			}

			return 0;
		}

		// rLine1 : y = (a1)x + b1
		// rLine2 ; y = (a2)x + b2
		// x = constant 형태의 직선일 경우
		// 첫번째 직선의 x값 : c1
		// 두번째 직선의 x값 : c2
		double a[2], b[2], c[2] = { 0.0, 0.0 };
		int i, nIndex1, nIndex2;
		bool bXEq[2] = { false, false };	// x = const 형태의 방정식인가?
		double x, y;
		Vertex2D vArr[4] = { vBegin1, vEnd1, vBegin2, vEnd2 };

		for (i = 0; i<2; i++)
		{
			nIndex1 = i * 2;
			nIndex2 = nIndex1 + 1;

			if (fabs(vArr[nIndex1].x - vArr[nIndex2].x) <= dTolerance)
			{
				a[i] = b[i] = 0.0;
				c[i] = vArr[nIndex1].x;
				bXEq[i] = true;
			}
			else if (fabs(vArr[nIndex1].y - vArr[nIndex2].y) <= dTolerance)
			{
				a[i] = 0.0;
				b[i] = vArr[nIndex1].y;
			}
			else
			{
				a[i] = (vArr[nIndex2].y - vArr[nIndex1].y) / (vArr[nIndex2].x - vArr[nIndex1].x);
				b[i] = vArr[nIndex2].y - (vArr[nIndex2].y - vArr[nIndex1].y) * vArr[nIndex2].x / (vArr[nIndex2].x - vArr[nIndex1].x);
			}
		}

		if (bXEq[0] && bXEq[1])
		{
			if (fabs(c[0] - c[1]) > dTolerance)
				return 0;

			double dBig1 = vBegin1.y, dSmall1 = vEnd1.y;
			double dBig2 = vBegin2.y, dSmall2 = vEnd2.y;

			if (dBig1 < vEnd1.y)
			{
				dBig1 = vEnd1.y;
				dSmall1 = vBegin1.y;
			}
			if (dBig2 < vEnd2.y)
			{
				dBig2 = vEnd2.y;
				dSmall2 = vBegin2.y;
			}

			if ((dBig1 < dSmall2 && fabs(dBig1 - dSmall2) > dTolerance) || (dBig2 < dSmall1 && fabs(dBig2 - dSmall1) > dTolerance))
				return 0;
			else if (fabs(dBig1 - dSmall2) <= dTolerance)
			{
				rVertex1.x = c[0];
				rVertex1.y = dBig1;
				return 1;
			}
			else if (fabs(dBig2 - dSmall1) <= dTolerance)
				//else if (dBig2 == dSmall2)
			{
				rVertex1.x = c[0];
				rVertex1.y = dBig2;
				return 1;
			}
			else if (dBig1 > dSmall2)
			{
				if (dBig1 <= dBig2) rVertex1.y = dBig1;
				else rVertex1.y = dBig2;
				if (dSmall1 < dSmall2) rVertex2.y = dSmall2;
				else rVertex2.y = dSmall1;

				rVertex1.x = rVertex2.x = c[0];
				return -1;
			}
			else //if (dBig2 > dSmall1)
			{
				if (dBig2 <= dBig1) rVertex1.y = dBig2;
				else rVertex1.y = dBig1;
				if (dSmall2 < dSmall1) rVertex2.y = dSmall1;
				else rVertex2.y = dSmall2;

				rVertex1.x = rVertex2.x = c[0];
				return -1;
			}
		}
		else if (bXEq[0])
		{
			x = c[0];
			y = a[1] * x + b[1];
		}
		else if (bXEq[1])
		{
			x = c[1];
			y = a[0] * x + b[0];
		}
		else
		{
			//if (a[0] == a[1])
			if (fabs(a[0] - a[1]) <= dTolerance)
			{
				//if (b[0] != b[1]) return 0;
				if (fabs(b[0] - b[1]) > dTolerance) return 0;

				double dBig1 = vBegin1.x, dSmall1 = vEnd1.x;
				double dBig2 = vBegin2.x, dSmall2 = vEnd2.x;

				if (dBig1 < vEnd1.x)
				{
					dBig1 = vEnd1.x;
					dSmall1 = vBegin1.x;
				}
				if (dBig2 < vEnd2.x)
				{
					dBig2 = vEnd2.x;
					dSmall2 = vBegin2.x;
				}

				if ((dBig1 < dSmall2 && fabs(dBig1 - dSmall2) > dTolerance) || (dBig2 < dSmall1 && fabs(dBig2 - dSmall1) > dTolerance)) return 0;
				//if (dBig1 < dSmall2 || dBig2 < dSmall1) return 0;
				else if (fabs(dBig1 - dSmall2) <= dTolerance)
					//else if (dBig1 == dSmall2)
				{
					rVertex1.x = dBig1;
					rVertex1.y = a[0] * dBig1 + b[0];
					return 1;
				}
				else if (fabs(dBig2 - dSmall1) <= dTolerance)
					//else if (dBig2 == dSmall2)
				{
					rVertex1.x = dBig2;
					rVertex1.y = a[0] * dBig2 + b[0];
					return 1;
				}
				else if (dBig1 > dSmall2)
				{
					if (dBig1 <= dBig2) rVertex1.x = dBig1;
					else rVertex1.x = dBig2;
					if (dSmall1 < dSmall2) rVertex2.x = dSmall2;
					else rVertex2.x = dSmall1;

					rVertex1.y = a[0] * rVertex1.x + b[0];
					rVertex2.y = a[0] * rVertex2.x + b[0];
					return -1;
				}
				else //if (dBig2 > dSmall1)
				{
					if (dBig2 <= dBig1) rVertex1.x = dBig2;
					else rVertex1.x = dBig1;
					if (dSmall2 < dSmall1) rVertex2.x = dSmall1;
					else rVertex2.x = dSmall2;

					rVertex1.y = a[0] * rVertex1.x + b[0];
					rVertex2.y = a[0] * rVertex2.x + b[0];
					return -1;
				}
			}
			else
			{
				x = (b[1] - b[0]) / (a[0] - a[1]);
				y = a[0] * x + b[0];
			}
		}

		if (vBegin1.x > vEnd1.x)
		{
			if (vEnd1.x > x && fabs(vEnd1.x - x) > dTolerance) return 0;
			if (x > vBegin1.x && fabs(x - vBegin1.x) > dTolerance) return 0;
		}
		else
		{
			if (vBegin1.x > x && fabs(vBegin1.x - x) > dTolerance) return 0;
			if (x > vEnd1.x && fabs(x - vEnd1.x) > dTolerance) return 0;
		}
		if (vBegin1.y > vEnd1.y)
		{
			if (vEnd1.y > y && fabs(vEnd1.y - y) > dTolerance) return 0;
			if (y > vBegin1.y && fabs(y - vBegin1.y) > dTolerance) return 0;
		}
		else
		{
			if (vBegin1.y > y && fabs(vBegin1.y - y) > dTolerance) return 0;
			if (y > vEnd1.y && fabs(y - vEnd1.y) > dTolerance) return 0;
		}

		if (vBegin2.x > vEnd2.x)
		{
			if (vEnd2.x > x && fabs(vEnd2.x - x) > dTolerance) return 0;
			if (x > vBegin2.x && fabs(x - vBegin2.x) > dTolerance) return 0;
		}
		else
		{
			if (vBegin2.x > x && fabs(vBegin2.x - x) > dTolerance) return 0;
			if (x > vEnd2.x && fabs(x - vEnd2.x) > dTolerance) return 0;
		}
		if (vBegin2.y > vEnd2.y)
		{
			if (vEnd2.y > y && fabs(vEnd2.y - y) > dTolerance) return 0;
			if (y > vBegin2.y && fabs(y - vBegin2.y) > dTolerance) return 0;
		}
		else
		{
			if (vBegin2.y > y && fabs(vBegin2.y - y) > dTolerance) return 0;
			if (y > vEnd2.y && fabs(y - vEnd2.y) > dTolerance) return 0;
		}

		rVertex1.x = x;
		rVertex1.y = y;
		return 1;
	}

	int PathItem::CalcIntersectionLineToLine(PathItem* item1, PathItem* item2)
	{
		Line2D* itemLine1 = item1->m_pLine;
		Line2D* itemLine2 = item2->m_pLine;

		if (item1->m_pInnerLine != 0)
			itemLine1 = item1->m_pInnerLine;

		if (item2->m_pInnerLine != 0)
			itemLine2 = item2->m_pInnerLine;

		if (itemLine1 == 0 || itemLine2 == 0)
			return -1;

		Vertex2D v1, v2;
		int nResult = IntersectLineToLine(*itemLine1, *itemLine2, v1, v2);

		if (nResult == 2)
		{
			//System.Diagnostics.Trace.WriteLine("Error");
			return -1;
		}
		else if (nResult == 0)
		{
			// 두 직선이 만나지 않을 경우 각각의 직선을 연장시켜 만나는 점을 찾는다.
			const Vertex2D& vBegin1 = itemLine1->GetVertex(true);
			const Vertex2D& vEnd1 = itemLine1->GetVertex(false);
			Vertex2D vTempEnd1 = vBegin1.GetLinearVertex(vEnd1, 100000);
			const Vertex2D& vBegin2 = itemLine2->GetVertex(true);
			const Vertex2D& vEnd2 = itemLine2->GetVertex(false);
			Vertex2D vTempBegin2 = vEnd2.GetLinearVertex(vBegin2, 100000);

			Line2D line1(vBegin1, vTempEnd1/*, Line2D.LineType.HALF_LINE_BEGIN_2_END*/);
			Line2D line2(vTempBegin2, vEnd2/*, Line2D.LineType.HALF_LINE_END_2_BEGIN*/);

			nResult = IntersectLineToLine(line1, line2, v1, v2);

			if (nResult == 0)
			{
				//System.Diagnostics.Trace.WriteLine("Error");
				return -1;
			}
		}

		if (item1->m_pInnerLine == 0)
		{
			item1->m_pInnerLine = new Line2D(itemLine1->GetVertex(true), v1);
		}
		else
		{
			item1->m_pInnerLine->SetVertex(v1, false);
		}

		item2->m_pInnerLine = new Line2D(v1, itemLine2->GetVertex(false));
		return 1;
	}

	void PathItem::InnerToCenter()
	{
		//if (m_drawType == DrawType.Line)
		{
			m_pLine = m_pInnerLine;
			m_pInnerLine = 0;
		}
	}

	void PathItem::SetWall(IWall* pWall)
	{
		m_pWall = pWall;
	}

	IWall* PathItem::GetWall()
	{
		return m_pWall;
	}
}
