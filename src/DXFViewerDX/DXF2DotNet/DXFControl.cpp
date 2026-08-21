#include "StdAfx.h"
#include "DXFControl.h"
#include "Layer.h"
#include "DXFLoader.h"
#include "Shape.h"
#include "LineType.h"
#include "PlotSettings.h"
#include "UPrintDocument.h"


#include "Block.h"
#include "Line.h"
#include "PolyLine.h"
#include "Text.h"
#include "ShapeGroup.h"
#include "Hatch.h"
#include "EArc.h"
#include "Arc.h"

using namespace System::Drawing;
using namespace System::Collections;
using namespace UnE::Geometry;

namespace DXFDotNet
{
	void DXFControl::Init()
	{
		m_pCurrentLayer = nullptr;
		m_pCurrentBlock = nullptr;

		m_arrLayer = gcnew ArrayList();
		m_arrBlock = gcnew ArrayList();

		m_unitOfLength = DXFDotNet::UnitOfLength::MILLIMETER;

		m_isOpened = false;

		m_fHomem11 = m_fHomem12 = m_fHomem21 = m_fHomem22 = 0.0f;
		m_fHomedx = m_fHomedy = 0.0f;
		
		m_dicPens = gcnew System::Collections::Generic::Dictionary<__int64, System::Drawing::Pen^>();
		
		m_vHomeViewportTL = m_vHomeViewportBL = m_vHomeViewportBR = nullptr;
		m_dHomeViewportWeight = 0.0;

		m_nGroupItemDistance = 30;
		m_useGroupItem = false;
		m_nGroupItemMinCount = 3;

		m_useLastViewport = false;

		m_vObjectTL = nullptr;
		m_vObjectBR = nullptr;
		m_vObjectCenter = nullptr;
	}
	// Y축이 화면 아래에서 위쪽으로 증가하는 방향인가?
	bool DXFControl::DownToTop()
	{
		return true;
	}

	System::Collections::Generic::Dictionary<__int64, System::Drawing::Pen^>^ DXFControl::GetLineTypePen()
	{
		return m_dicPens;
	}

	void DXFControl::SetCurrentLayer(Layer^ layer)
	{
		m_pCurrentLayer = layer;
	}

	Layer^ DXFControl::GetCurrentLayer()
	{
		return m_pCurrentLayer;
	}

	void DXFControl::SetCurrentBlock(Block^ block)
	{
		m_pCurrentBlock = block;
	}

	Block^ DXFControl::GetCurrentBlock()
	{
		return m_pCurrentBlock;
	}

	Vertex2D^ DXFControl::ScreenToGlobal(int x, int y)
	{
		Vertex2D^ vResult = nullptr;
		return vResult;
	}

	Point DXFControl::GlobalToScreen(Vertex2D^ vertex)
	{
		Point ptResult;

		return ptResult;
	}

	void DXFControl::_Refresh()
	{

	}


	EntityFactory^ DXFControl::GetShapeFactory()
	{
		return mFactory;
	}

	void DXFControl::SetShapeFactory(EntityFactory^ factory)
	{
		mFactory = factory;
	}
}