#include "stdafx.h"
#include "_POI.h"
#include "POI.h"
#include <algorithm>
#include <codecvt>

using namespace VectorGraphics;

namespace FireSafetyManager
{
	static const double HALF_PI	= 1.57079632679489661923;
	static const double _PI = 3.14159265358979323846;
	static const double _3HALF_PI = 4.71238898038468985769;
	static const double _2PI = 6.28318530717958647692;

	POIIcon* POIType::m_pDefaultIcon = 0;

	POIType::POIType()
	{
		m_nID = -1;
		m_strTypeName = L"";
		m_strCode = L"";
		m_pIcon = 0;
	}

	POIType::POIType(int nID, const std::wstring& strName, const std::wstring& strCode, POIIcon* pIcon)
	{
		m_nID = nID;
		m_strTypeName = strName;
		m_strCode = strCode;
		m_pIcon = pIcon;
	}

	POIType::~POIType()
	{
		delete m_pIcon;
	}

	int POIType::GetID()
	{
		return m_nID;
	}

	const std::wstring& POIType::GetName()
	{
		return m_strTypeName;
	}

	const std::wstring& POIType::GetCode()
	{
		return m_strCode;
	}

	POIIcon* POIType::GetIcon()
	{
		return m_pIcon;
	}

	static std::wstring UTF8ToANSI(const char *utf8str)
	{
		std::wstring_convert<std::codecvt_utf8<wchar_t>> wconv;
		std::wstring wstr = wconv.from_bytes(utf8str);
		return wstr;
	}

	static int GetStringLength(unsigned char* bytes, int& rIndex)
	{
		int add = 0;
		int multiply = 1;

		while (bytes[rIndex] >= 0x80)
		{
			add = add + multiply * (int)(bytes[rIndex++] - 0x80);
			multiply *= 0x80;
		}

		return add + multiply * (int)bytes[rIndex++];
	}

	static std::wstring ReadString(unsigned char* bytes, int& rIndex)
	{
		int len = GetStringLength(bytes, rIndex);

		char temp = bytes[rIndex + len];
		bytes[rIndex + len] = 0;

		std::wstring str = UTF8ToANSI((const char*)&bytes[rIndex]);
		bytes[rIndex + len] = temp;
		rIndex += len;

		return str;
	}

	static VertexList* ReadLine(unsigned char* bytes, int& rIndex, std::vector<VertexList*>& vecBoundaries, VertexList* pCurrentList, double dTolerance)
	{
		int DoubleSize = (int)sizeof(double);
		double beginX, beginY, endX, endY;

		memcpy(&beginX, &bytes[rIndex], DoubleSize);
		rIndex += DoubleSize;
		memcpy(&beginY, &bytes[rIndex], DoubleSize);
		rIndex += DoubleSize;
		memcpy(&endX, &bytes[rIndex], DoubleSize);
		rIndex += DoubleSize;
		memcpy(&endY, &bytes[rIndex], DoubleSize);
		rIndex += DoubleSize;

		Vertex2D vBegin(beginX, beginY);
		Vertex2D vEnd(endX, endY);

		if (pCurrentList != 0)
		{
			std::list<Vertex2D>::iterator iter = pCurrentList->Vertices.end();

			if (pCurrentList->Vertices.begin() != iter)
			{
				iter--;
				Vertex2D& rLast = *iter;

				if (rLast.GetDistance(vBegin) < dTolerance)
				{
					pCurrentList->Vertices.push_back(vEnd);
					return pCurrentList;
				}
			}
		}

		VertexList* pVertexList = new VertexList();
		pVertexList->Vertices.push_back(vBegin);
		pVertexList->Vertices.push_back(vEnd);

		vecBoundaries.push_back(pVertexList);
		return pVertexList;
	}

	static VertexList* ReadArc(unsigned char* bytes, int& rIndex, std::vector<VertexList*>& vecBoundaries, VertexList* pCurrentList, double dTolerance)
	{
		int DoubleSize = (int)sizeof(double);
		int BoolSize = (int)sizeof(bool);
		double centerX, centerY, radius, beginAngle, arcAngle;
		bool isClockwise;

		memcpy(&centerX, &bytes[rIndex], DoubleSize);
		rIndex += DoubleSize;
		memcpy(&centerY, &bytes[rIndex], DoubleSize);
		rIndex += DoubleSize;
		memcpy(&radius, &bytes[rIndex], DoubleSize);
		rIndex += DoubleSize;
		memcpy(&beginAngle, &bytes[rIndex], DoubleSize);
		rIndex += DoubleSize;
		memcpy(&arcAngle, &bytes[rIndex], DoubleSize);
		rIndex += DoubleSize;
		memcpy(&isClockwise, &bytes[rIndex], BoolSize);
		rIndex += BoolSize;

		double dBeginX = centerX + cos(beginAngle) * radius;
		double dBeginY = centerY + sin(beginAngle) * radius;
		Vertex2D vBegin(dBeginX, dBeginY);

		VertexList* pVertexList = 0;

		if (pCurrentList != 0)
		{
			std::list<Vertex2D>::iterator iter = pCurrentList->Vertices.end();

			if (pCurrentList->Vertices.begin() != iter)
			{
				iter--;
				Vertex2D& rLast = *iter;

				if (rLast.GetDistance(vBegin) < dTolerance)
				{
					pVertexList = pCurrentList;
				}
			}
		}

		if (pVertexList == 0)
		{
			pVertexList = new VertexList();
			pVertexList->Vertices.push_back(vBegin);
			vecBoundaries.push_back(pVertexList);
		}

		int nVertexCount = (int)(100 * arcAngle / _2PI);

		if (nVertexCount < 0)
			nVertexCount = -nVertexCount;

		if (nVertexCount < 2)
			return pVertexList;

		double dAngle = arcAngle / (nVertexCount - 1);

		for (int i = 1; i < nVertexCount; i++)
		{
			double dTheta = isClockwise ? beginAngle - dAngle * i : beginAngle + dAngle * i;
			double x = centerX + cos(dTheta) * radius;
			double y = centerY + sin(dTheta) * radius;

			pVertexList->Vertices.push_back(Vertex2D(x, y));
		}

		return pVertexList;
	}

	static Vertex2D GetEArcVertex(Vertex2D& vTL, Vertex2D& vBL, Vertex2D& vBR, Vertex2D& vCenter, Vertex2D& vLeft, Vertex2D& vRight, Vertex2D& vTop, Vertex2D& vBottom, double a, double b, double dAngle, double dTolerance)
	{
		if (dAngle < 0.0)
		{
			dAngle += _2PI;
		}
		else if (dAngle > _2PI)
		{
			dAngle -= _2PI;
		}

		if (dAngle <= dTolerance || dAngle >= _2PI - dTolerance)
			return vRight;
		else if (dAngle >= HALF_PI - dTolerance && dAngle <= HALF_PI + dTolerance)
			return vTop;
		else if (dAngle >= _PI - dTolerance && dAngle <= _PI + dTolerance)
			return vLeft;
		else if (dAngle >= _3HALF_PI - dTolerance && dAngle <= _3HALF_PI + dTolerance)
			return vBottom;

		double dLengthX, dLengthY;

		if (dAngle < HALF_PI)
		{
			double dTanData = tan(dAngle);

			dLengthX = sqrt(1.0 / (1.0 / a / a + dTanData * dTanData / b / b));
			dLengthY = sqrt(1.0 / (1.0 / a / a / dTanData / dTanData + 1.0 / b / b));
		}
		else if (dAngle < _PI)
		{
			double dTanData = tan(_PI - dAngle);

			dLengthX = -sqrt(1.0 / (1.0 / a / a + dTanData * dTanData / b / b));
			dLengthY = sqrt(1.0 / (1.0 / a / a / dTanData / dTanData + 1.0 / b / b));
		}
		else if (dAngle < _3HALF_PI)
		{
			double dTanData = tan(dAngle - _PI);

			dLengthX = -sqrt(1.0 / (1.0 / a / a + dTanData * dTanData / b / b));
			dLengthY = -sqrt(1.0 / (1.0 / a / a / dTanData / dTanData + 1.0 / b / b));
		}
		else
		{
			double dTanData = tan(_2PI - dAngle);

			dLengthX = sqrt(1.0 / (1.0 / a / a + dTanData * dTanData / b / b));
			dLengthY = -sqrt(1.0 / (1.0 / a / a / dTanData / dTanData + 1.0 / b / b));
		}

		double x = vCenter.x + (vRight.x - vCenter.x) * dLengthX / a + (vTop.x - vCenter.x) * dLengthY / b;
		double y = vCenter.y + (vRight.y - vCenter.y) * dLengthX / a + (vTop.y - vCenter.y) * dLengthY / b;
		return Vertex2D(x, y);
	}

	static double ValidAngle(double angle)
	{
		if (angle < 0.0)
		{
			int nCount = (int)(-angle / _2PI);
			angle += _2PI * (nCount + 1);
		}
		else if (angle > _2PI)
		{
			int nCount = (int)(angle / _2PI);
			angle -= _2PI * nCount;
		}

		return angle;
	}

	static VertexList* ReadEArc(unsigned char* bytes, int& rIndex, std::vector<VertexList*>& vecBoundaries, VertexList* pCurrentList, double dTolerance)
	{
		int DoubleSize = (int)sizeof(double);
		int BoolSize = (int)sizeof(bool);
		double tlX, tlY, blX, blY, brX, brY, beginAngle, earcAngle;
		bool isClockwise;

		memcpy(&tlX, &bytes[rIndex], DoubleSize);
		rIndex += DoubleSize;
		memcpy(&tlY, &bytes[rIndex], DoubleSize);
		rIndex += DoubleSize;
		memcpy(&blX, &bytes[rIndex], DoubleSize);
		rIndex += DoubleSize;
		memcpy(&blY, &bytes[rIndex], DoubleSize);
		rIndex += DoubleSize;
		memcpy(&brX, &bytes[rIndex], DoubleSize);
		rIndex += DoubleSize;
		memcpy(&brY, &bytes[rIndex], DoubleSize);
		rIndex += DoubleSize;
		memcpy(&beginAngle, &bytes[rIndex], DoubleSize);
		rIndex += DoubleSize;
		memcpy(&earcAngle, &bytes[rIndex], DoubleSize);
		rIndex += DoubleSize;
		memcpy(&isClockwise, &bytes[rIndex], BoolSize);
		rIndex += BoolSize;

		Vertex2D vTL(tlX, tlY), vBL(blX, blY), vBR(brX, brY);
		Vertex2D vCenter = (vTL + vBR) / 2;
		Vertex2D vLeft = (vTL + vBL) / 2;
		Vertex2D vBottom = (vBL + vBR) / 2;
		Vertex2D vRight = vLeft + vBR - vBL;
		Vertex2D vTop = vBottom + vTL - vBL;

		double a = vBL.GetDistance(vBR);
		double b = vTL.GetDistance(vBL);

		beginAngle = ValidAngle(beginAngle);
		Vertex2D vBegin = GetEArcVertex(vTL, vBL, vBR, vCenter, vLeft, vRight, vTop, vBottom, a, b, beginAngle, dTolerance);

		VertexList* pVertexList = 0;

		if (pCurrentList != 0)
		{
			std::list<Vertex2D>::iterator iter = pCurrentList->Vertices.end();

			if (pCurrentList->Vertices.begin() != iter)
			{
				iter--;
				Vertex2D& rLast = *iter;

				if (rLast.GetDistance(vBegin) < dTolerance)
				{
					pVertexList = pCurrentList;
				}
			}
		}

		if (pVertexList == 0)
		{
			pVertexList = new VertexList();
			pVertexList->Vertices.push_back(vBegin);
			vecBoundaries.push_back(pVertexList);
		}

		int nVertexCount = (int)(100 * earcAngle / _2PI);

		if (nVertexCount < 0)
			nVertexCount = -nVertexCount;

		if (nVertexCount < 2)
			return pVertexList;

		double dAngle = earcAngle / (nVertexCount - 1);

		for (int i = 1; i < nVertexCount; i++)
		{
			double dTheta = isClockwise ? beginAngle - dAngle * i : beginAngle + dAngle * i;
			Vertex2D vertex = GetEArcVertex(vTL, vBL, vBR, vCenter, vLeft, vRight, vTop, vBottom, a, b, dTheta, dTolerance);

			pVertexList->Vertices.push_back(vertex);
		}

		return pVertexList;
	}

	static void ReadBoundary(std::vector<VertexList*>& vecBoundaries, unsigned char* bytes, int& rIndex, double dTolerance)
	{
		int IntSize = (int)sizeof(int);

		int nPathCount, nDrawingType;
		memcpy(&nPathCount, &bytes[rIndex], IntSize);

		rIndex += IntSize;
		VertexList* pVertexList = 0;

		for (int i = 0; i < nPathCount; i++)
		{
			memcpy(&nDrawingType, &bytes[rIndex], IntSize);
			rIndex += IntSize;

			// Line
			if (nDrawingType == 1)
			{
				pVertexList = ReadLine(bytes, rIndex, vecBoundaries, pVertexList, dTolerance);
			}
			// Arc
			else if (nDrawingType == 2)
			{
				pVertexList = ReadArc(bytes, rIndex, vecBoundaries, pVertexList, dTolerance);
			}
			// EArc
			else if (nDrawingType == 3)
			{
				pVertexList = ReadEArc(bytes, rIndex, vecBoundaries, pVertexList, dTolerance);
			}
		}
	}

	static void ReadText(std::vector<POIText>& vecTexts, unsigned char* bytes, int& rIndex)
	{
		int IntSize = (int)sizeof(int);

		int nTextCount;
		memcpy(&nTextCount, &bytes[rIndex], IntSize);

		rIndex += IntSize;

		int DoubleSize = (int)sizeof(double);
		double x, y, angle;

		int FloatSize = (int)sizeof(float);
		float fontSize;


		for (int i = 0; i < nTextCount; i++)
		{
			std::wstring strText = ReadString(bytes, rIndex);

			memcpy(&x, &bytes[rIndex], DoubleSize);
			rIndex += DoubleSize;
			memcpy(&y, &bytes[rIndex], DoubleSize);
			rIndex += DoubleSize;
			memcpy(&fontSize, &bytes[rIndex], FloatSize);
			rIndex += FloatSize;
			memcpy(&angle, &bytes[rIndex], DoubleSize);
			rIndex += DoubleSize;

			POIText data(strText, Vertex2D(x + fontSize / 5, y + fontSize * 0.4), fontSize * 2, angle);
			vecTexts.push_back(data);
		}
	}

	bool POIType::LoadPOIIcon(const std::string& strPath)
	{
		FILE* fp;
		fopen_s(&fp, strPath.c_str(), "rb");

		if (fp != 0)
		{
			fseek(fp, 0, SEEK_END);
			int nFileSize = ftell(fp);
			fseek(fp, 0, SEEK_SET);

			unsigned char* bytes = new unsigned char[nFileSize + 1];
			fread(bytes, sizeof(unsigned char), nFileSize, fp);

			fclose(fp);

			int nIndex = 0;
			std::wstring strPOIName = ReadString(bytes, nIndex);

			double dTolerance = 0.1;

			std::vector<VertexList*> vecEdgeBoundaries;
			ReadBoundary(vecEdgeBoundaries, bytes, nIndex, dTolerance);

			int IntSize = (int)sizeof(int);

			int nPolygonCount;
			memcpy(&nPolygonCount, &bytes[nIndex], IntSize);
			nIndex += IntSize;

			std::vector<VertexList*> vecFillBoundaries;

			for (int i = 0; i < nPolygonCount; i++)
			{
			//if (nPolygonCount > 0)
				ReadBoundary(vecFillBoundaries, bytes, nIndex, dTolerance);
			}

			std::vector<POIText> vecTexts;
			ReadText(vecTexts, bytes, nIndex);

			delete[] bytes;

			m_pIcon = new POIIcon();

			int nEdgeCount = (int)vecEdgeBoundaries.size();
			int nFillCount = (int)vecFillBoundaries.size();
			int nTextCount = (int)vecTexts.size();

			for (int i = 0; i < nEdgeCount; i++)
			{
				m_pIcon->AddBoundaryEdge(vecEdgeBoundaries[i]);
			}

			for (int i = 0; i < nFillCount; i++)
			{
				m_pIcon->AddFillEdge(vecFillBoundaries[i]);
			}

			for (int i = 0; i < nTextCount; i++)
			{
				m_pIcon->AddText(vecTexts[i]);
			}

			m_pIcon->Done();
		}
		else
			return false;

		return true;
	}

	POIIcon* POIType::GetDefaultIcon()
	{
		if (m_pDefaultIcon != 0)
			return m_pDefaultIcon;

		MakeDefaultIcon();
		return m_pDefaultIcon;
	}

	void POIType::MakeDefaultIcon()
	{
		double dIconSize = 100;
		double dRadius = dIconSize / 2;
		double centerX = dRadius;
		double centerY = dRadius;

		int nVertexCount = 100;
		double dAngle = _2PI / nVertexCount;

		POIIcon* icon = new POIIcon();
		VertexList* polygon = new VertexList();

		for (int i = 0; i < nVertexCount; i++)
		{
			double dTheta = dAngle * i;
			double x = centerX + cos(dTheta) * dRadius;
			double y = centerY + sin(dTheta) * dRadius;

			polygon->Vertices.push_back(Vertex2D(x, y));
		}

		icon->AddFillEdge(polygon);
		icon->Done();

		m_pDefaultIcon = icon;
	}

	POI::POI()
	{
		m_nID = -1;
		m_strName = L"";
		m_dHeight = 0.0;
		m_dAngle = 0.0;
		m_pType = 0;
	}

	// dAngle : Degree
	POI::POI(int nID, const std::wstring& strName, const Vertex2D& vPos, double dHeight, double dAngle, POIType* pType)
	{
		m_nID = nID;
		m_strName = strName;
		m_vPos = vPos;
		m_dHeight = dHeight;
		m_dAngle = dAngle;
		m_pType = pType;
	}

	int POI::GetID()
	{
		return m_nID;
	}

	const std::wstring& POI::GetName()
	{
		return m_strName;
	}

	const Vertex2D& POI::GetPosition()
	{
		return m_vPos;
	}

	double POI::GetHeight()
	{
		return m_dHeight;
	}

	POIType* POI::GetPOIType()
	{
		return m_pType;
	}

	double POI::GetAngle()
	{
		return m_dAngle;
	}
}
