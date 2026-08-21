#pragma once
#include "Shape.h"
#include "Vertex2D.h"
#include <string>
#include <list>

namespace VectorGraphics
{
	class __declspec(dllexport) VertexList
	{
	public:
		std::list<Vertex2D> Vertices;
	};

	class __declspec(dllexport) POIText
	{
	public:
		POIText();
		// dAngle : Degree
		POIText(const std::wstring& strText, const Vertex2D& vPos, float fFontSize, double dAngle);

	public:
		const std::wstring& GetText();
		void SetPosition(const Vertex2D& vPos);
		void SetFontName(const std::wstring& strFontName);
		// Degree
		double GetAngle();
		float GetFontSize();
		const std::wstring& GetFontName();
		const Vertex2D& GetPosition();

		void Draw(Layer* pLayer, const Vertex2D& vPos);

	private:
		std::wstring m_strText;
		float m_fFontSize;
		Vertex2D m_vPos;
		// Degree
		double m_dAngle;
		std::wstring m_strFontName;
	};

	class __declspec(dllexport) POIIcon
	{
	public:
		POIIcon();
		virtual ~POIIcon();

	public:
		void AddBoundaryEdge(VertexList* pVertexList);
		void AddFillEdge(VertexList* pVertexList);
		void AddText(const POIText& text);
		void Done();

		int GetBoundaryEdgeCount();
		VertexList* GetBoundaryEdge(int nIndex);
		int GetFillEdgeCount();
		VertexList* GetFillEdge(int nIndex);
		int GetTextCount();
		POIText* GetText(int nIndex);

		const Vertex2D& GetTL();
		const Vertex2D& GetBR();

		void Draw(Layer* pLayer, const Vertex2D& vPos);

	private:
		void SetBoundary(VertexList* pVertexList);
		void SetBoundary(POIText& text);

	private:
		std::list<VertexList*> m_boundaryEdges;
		std::list<VertexList*> m_fillEdges;
		std::list<POIText> m_texts;
		Vertex2D m_vTL, m_vBR;
		bool m_initialize;

		std::list<unsigned int*> m_indices;
		std::list<float*> m_coords;
		std::list<int> m_indexCounts;
	};

	class __declspec(dllexport) POI : public Shape
	{
	public:
		POI();
		virtual ~POI();

	public:
		void Draw();
		bool HitTest(const Vertex2D& vPos);
		bool HitTestIfPOI(const Vertex2D& vPos);

	public:
		void SetPosition(const Vertex2D& vPos);
		void SetIcon(POIIcon* pIcon);

		const Vertex2D& GetPosition();
		POIIcon* GetIcon();

	private:
		Vertex2D m_vPos, m_vTrans;
		POIIcon* m_pIcon;
		bool m_setPosition;
	};
}
