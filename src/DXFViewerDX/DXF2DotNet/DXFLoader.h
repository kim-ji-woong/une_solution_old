#pragma once

namespace Utility
{
	class FileManager;
	class Vertex3D;
}

#include <string>
#include <list>
#include "DXFTable.h"
#include "DXFLType.h"
#include <Windows.h>

namespace DXF
{
	namespace HEADER
	{
		class CHeader;
	}

	namespace TABLES
	{
		class TableManager;
	}

	namespace ENTITIES
	{
		class EntityManager;
		class Line;
		class Arc;
		class Circle;
		class Ellipse;
		class PolyLine;
		class Hatch;
		class Text;
		class MText;
		class Insert;
		class Entity;
	}

	namespace BLOCKS
	{
		class BlockManager;
		class BlockData;
	}

	namespace OBJECTS
	{
		class ObjectManager;
	}
}

namespace DXFDotNet
{
	ref class DXFControl;
	ref class LineType;
	ref class Layer;
	ref class Line;
	ref class Arc;
	ref class EArc;
	ref class PolyLine;
	ref class Hatch;
	ref class Shape;
	ref class Text;
	ref class PlotSettings;

	public ref class DXFLoader
	{
	public:
		DXFLoader(DXFControl^ ctrl, System::Collections::ArrayList^ arrLayer);
		virtual ~DXFLoader(void);

	public:
		bool Load(System::String^ strPath);

	public:
		// 도면을 열때 AutoCAD에서 마지막으로 기억된 Viewport를 사용할 것인가?
		property bool UseLastViewport
		{
			bool get() { return m_useLastViewport; }
			void set(bool value) { m_useLastViewport = value; }
		}

	public:
		// vCenter를 중점으로 가진 Arc의 시작점(vBegin)에서 끝점(vEnd) 까지의 Vertex List를 구하여 arrVertex에 집어넣는다.
		// isClockwise : Arc의 진행 방향이 시계방향인가?
		static void CreateArcVertex(System::Collections::ArrayList^ arrVertex, UnE::Geometry::Vertex2D^ vBegin, UnE::Geometry::Vertex2D^ vCenter, UnE::Geometry::Vertex2D^ vEnd, int nSliceCount, bool isClockwise);

	protected:
		void ReadDXFLType(DXF::TABLES::TableManager* pTblMgr);
		void AddLineTypeFromDXF(DXF::TABLES::LType::Entity* pEntity);
		void ReadDXFLayer(DXF::HEADER::CHeader* pHdrMgr, DXF::TABLES::TableManager* pTblMgr);
		void ReadDXFEntity(DXF::ENTITIES::EntityManager* pEntMgr, DXF::BLOCKS::BlockManager* pBlkMgr, DXF::TABLES::TableManager* pTblMgr, COLORREF* pCol, double dScale);
		UnE::Geometry::Vertex2D^ ReadDXFVPort(DXF::TABLES::TableManager* pTblMgr, double& dViewportWeight, double dScale);
		double SetUnitOfLength(DXF::HEADER::CHeader& hdrMgr);
		
		Layer^ GetLayer(System::String^ strLayerName);

		Line^ GetLineFromDXF(DXF::ENTITIES::Line* pLineSrc, double dScale, Layer^ layer);
		Arc^ GetArcFromDXF(DXF::ENTITIES::Arc* pArcSrc, double dScale, Layer^ layer);
		Arc^ GetCircleFromDXF(DXF::ENTITIES::Circle* pCircleSrc, double dScale, Layer^ layer);
		EArc^ GetEArcFromDXF(DXF::ENTITIES::Ellipse* pEllipseSrc, double dScale, Layer^ layer);
		PolyLine^ GetPolyLineFromDXF(DXF::ENTITIES::PolyLine* pPolySrc, double dScale, Layer^ layer);
		Hatch^ GetHatchFromDXF(DXF::ENTITIES::Hatch* pHatchSrc, double dScale, Layer^ layer);
		void DXFLoader::GetHatchBoundaryFromLineEdge(Hatch^ hatch, void* pLine, double dScale);
		Text^ GetTextFromDXF(DXF::ENTITIES::Text* pTextSrc, DXF::TABLES::TableManager* pTblMgr, double dScale, Layer^ layer);
		Text^ GetMTextFromDXF(DXF::ENTITIES::MText* pTextSrc, DXF::TABLES::TableManager* pTblMgr, double dScale, Layer^ layer);
		void GetEntityFromInsert(DXF::ENTITIES::Insert* pInsert, DXF::BLOCKS::BlockManager* pBlkMgr, DXF::TABLES::TableManager* pTblMgr, double dScale);
		void GetEntityFromBlockData(DXF::BLOCKS::BlockData* pBlock, const Utility::Vertex3D& rInsertPoint, const wchar_t* strBlockName, DXF::BLOCKS::BlockManager* pBlkMgr, DXF::TABLES::TableManager* pTblMgr, double dScale);

		void SetObjectColor(Shape^ obj, Layer^ layer, DXF::ENTITIES::Entity* pEntity, COLORREF* pCol);
		Shape^ FindMap(DXF::ENTITIES::Entity* pEntity);

		void CheckTextObjectArea(Text^ text, int nHorizon, int nVertical, float fTextWidth, float fTextHeight);

		double GetUnitFlag(UnitOfLength unitSrc, UnitOfLength unitTrg);
		//UnE::Geometry::Vertex2D^ CalcInitCenter(UnE::Geometry::Vertex2D^ vObjectCenter);

		// Polyline내의 Arc 중심점을 구한다.
		// v1 : Arc의 시작점
		// v2 : Arc의 끝점
		// bulge : Arc 매개변수
		// isClockWise : Arc의 진행방향이 시계방향인가?
		UnE::Geometry::Vertex2D^ GetPolylineArcCenter(UnE::Geometry::Vertex2D^ v1, UnE::Geometry::Vertex2D^ v2, double bulge, bool% isClockWise);

		void SetPlotSettings(DXF::OBJECTS::ObjectManager& objMgr, PlotSettings^ plotSettings);

	protected:
		DXFControl^ m_ctrl;
		// LineType Name, LineType
		System::Collections::Generic::Dictionary<System::String^, LineType^>^ m_dicLineType;
		// Layer Name, Layer
		System::Collections::Generic::Dictionary<System::String^, Layer^>^ m_dicLayer;
		System::Collections::ArrayList^ m_arrLayer;
		// For Hatch Objects
		// DXF::ENTITIES::Entity*, Shape 객체
		System::Collections::Generic::Dictionary<__int64, Shape^>^ m_dicShape;

		// DXF 로딩후 화면 중심점을 계산하기 위하여 DXF 객체들의 사각영역을 기억시킨다.
		UnE::Geometry::Vertex2D^ m_vObjectTL;
		UnE::Geometry::Vertex2D^ m_vObjectBL;
		UnE::Geometry::Vertex2D^ m_vObjectBR;
		bool m_isInitArea;

		// 도면을 열때 AutoCAD에서 마지막으로 기억된 Viewport를 사용할 것인가?
		bool m_useLastViewport;
	};
}
