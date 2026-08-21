#include "StdAfx.h"
#include "DXFLoader.h"
#include "DXF.h"
#include "LineType.h"
#include "Layer.h"
#include "DXFControl.h"
#include "Line.h"
#include "Arc.h"
#include "EArc.h"
#include "PolyLine.h"
#include "Hatch.h"
#include "Text.h"
#include "Block.h"
#include "PlotSettings.h"
#include "EntityFactory.h"
#include <vector>

using namespace UnE::Geometry;

BEGIN_NS(DXFDotNet)

DXFLoader::DXFLoader(DXFControl^ ctrl, System::Collections::ArrayList^ arrLayer)
{
	m_ctrl = ctrl;

	m_dicLineType = gcnew System::Collections::Generic::Dictionary<System::String^, LineType^>();
	m_dicLayer = gcnew System::Collections::Generic::Dictionary<System::String^, Layer^>();
	m_dicShape = gcnew System::Collections::Generic::Dictionary<__int64, Shape^>();

	m_arrLayer = arrLayer;

	m_vObjectTL = gcnew UnE::Geometry::Vertex2D(0.0, 0.0);
	m_vObjectBL = gcnew UnE::Geometry::Vertex2D(0.0, 0.0);
	m_vObjectBR = gcnew UnE::Geometry::Vertex2D(0.0, 0.0);
	m_isInitArea = false;
	m_useLastViewport = false;
}

DXFLoader::~DXFLoader(void)
{
}

bool DXFLoader::Load(System::String^ strPath)
{
	int nLen = strPath->Length;

	wchar_t* path = ToWcharArray(strPath);
	if (path == 0)
		return false;

	DXF::BLOCKS::BlockManager blkMgr;
	DXF::CLASSES::ClassManager clsMgr;
	DXF::ENTITIES::EntityManager entMgr;
	DXF::HEADER::CHeader hdrMgr;
	DXF::OBJECTS::ObjectManager objMgr(&blkMgr);
	DXF::TABLES::TableManager tblMgr;

	DXF::DXFManager mgr;
	mgr.SetBlockManager(&blkMgr);
	mgr.SetClassManager(&clsMgr);
	mgr.SetEntityManager(&entMgr);
	mgr.SetHeaderManager(&hdrMgr);
	mgr.SetObjectManager(&objMgr);
	mgr.SetTableManager(&tblMgr);

	bool isSuccess = mgr.OpenFile(path);

	delete [] path;

	if (!isSuccess)
		return false;

	//SetPlotSettings(objMgr, m_ctrl->PlotSettings);

	m_ctrl->Layers->Clear();
	m_ctrl->Blocks->Clear();
	//m_ctrl->InitSize();

	double dUnitFlag = SetUnitOfLength(hdrMgr);

	ReadDXFLType(&tblMgr);
	ReadDXFLayer(&hdrMgr, &tblMgr);
	// AutoCAD에서 가장 마지막으로 화면에 나타났던 Viewport를 읽어오는 부분
	// AutoCAD가 기억시킨 마지막 장면을 불러오기 위하여 읽어야 하지만, 마지막으로 기억된 화면보다는
	// 실제 DXF 객체들을 화면에 보여주는 것이 더 중요하므로 Viewport 정보는 읽지 않는다.
	double dViewportWeight = 1.0;
	UnE::Geometry::Vertex2D^ vObjectCenter = ReadDXFVPort(&tblMgr, dViewportWeight, dUnitFlag);
	ReadDXFEntity(&entMgr, &blkMgr, &tblMgr, 0, dUnitFlag);

	m_ctrl->ObjectTL = m_vObjectTL;
	m_ctrl->ObjectBR = m_vObjectBR;
	m_ctrl->ObjectCenter = vObjectCenter;

	return true;
}

void DXFLoader::SetPlotSettings(DXF::OBJECTS::ObjectManager& objMgr, PlotSettings^ plotSettings)
{
	void* pID = 0;
	DXF::OBJECTS::Object* pObject = objMgr.GetObject(pID);
	DXF::OBJECTS::Layout* pLayout = 0;

	while (pObject != 0)
	{
		if (!wcscmp(pObject->GetEntityType(), L"LAYOUT"))
		{
			pLayout = (DXF::OBJECTS::Layout*)pObject;
			break;
		}
	}

	if (pLayout == 0)
		return;

	DXF::OBJECTS::PlotSettings* pPlotSettings = pLayout->GetPlotSettings();

	if (pPlotSettings == 0)
		return;

	double dNumerator, dDenominator;
	int nPaperUnits = pPlotSettings->GetPaperUnits();
	pPlotSettings->GetPrintScale(&dNumerator, &dDenominator);

	if (dNumerator != 0.0 && dDenominator != 0.0)
		plotSettings->SetPrintScale(dNumerator, dDenominator);

	if (nPaperUnits >= 0 && nPaperUnits < (int)PlotSettings::PlotPaperUnits::TYPE_COUNT)
		plotSettings->PlotPaperUnit = (PlotSettings::PlotPaperUnits)nPaperUnits;
}
//
//UnE::Geometry::Vertex2D^ DXFLoader::CalcInitCenter(UnE::Geometry::Vertex2D^ vObjectCenter)
//{
//	UnE::Geometry::Vertex2D^ vOrigin = m_ctrl->ScreenToGlobal(0, 0);
//	UnE::Geometry::Vertex2D^ v100 = m_ctrl->ScreenToGlobal(100, 0);
//
//	int nCenterX = m_ctrl->Size.Width / 2;
//	int nCenterY = m_ctrl->Size.Height / 2;
//	UnE::Geometry::Vertex2D^ vCenter = m_ctrl->ScreenToGlobal(nCenterX, nCenterY);
//
//	double distance = vOrigin->GetDistance(v100);
//	double w = vObjectCenter->x - vCenter->x;
//	double h = vObjectCenter->y - vCenter->y;
//
//	int nMoveX = (int)(100 * w / distance);
//	int nMoveY = (int)(100 * h / distance);
//
//	UnE::Geometry::Vertex2D^ vViewportCenter = m_ctrl->GetViewportCenter();
//	Vertex2D^ vNewCenter = gcnew Vertex2D(vViewportCenter->x + nMoveX, -vViewportCenter->y + nMoveY);
//	m_ctrl->SetViewportCenter(vNewCenter);
//
//	Vertex2D^ vCurrent = m_ctrl->ScreenToGlobal(nCenterX, nCenterY);
//	return vCurrent;
//}

double DXFLoader::SetUnitOfLength(DXF::HEADER::CHeader& hdrMgr)
{
	UnitOfLength unitTrg = m_ctrl->UnitOfLength;
	UnitOfLength unitSrc = unitTrg;

	std::map<std::wstring, DXF::HEADER::CData>& rHeaderMap = hdrMgr.GetHeader();
	std::map<std::wstring, DXF::HEADER::CData>::iterator iter = rHeaderMap.find(L"INSUNITS");

	if (iter != rHeaderMap.end())
	{
		std::wstring strTag = L"";
		int nCode = 0;
		double dValue = 0.0;
		
		int nValue = iter->second.GetIntValue();

		if (nValue == 1)
			unitSrc = DXFDotNet::UnitOfLength::INCH;
		else if (nValue == 2)
			unitSrc = DXFDotNet::UnitOfLength::FEET;
		else if (nValue == 4)
			unitSrc = DXFDotNet::UnitOfLength::MILLIMETER;
		else if (nValue == 5)
			unitSrc = DXFDotNet::UnitOfLength::CENTIMETER;
		else if (nValue == 6)
			unitSrc = DXFDotNet::UnitOfLength::METER;
	}

	return GetUnitFlag(unitSrc, unitTrg);
}

double DXFLoader::GetUnitFlag(UnitOfLength unitSrc, UnitOfLength unitTrg)
{
	if (unitSrc == DXFDotNet::UnitOfLength::INCH)
	{
		if (unitTrg == DXFDotNet::UnitOfLength::INCH)
			return 1.0;
		else if (unitTrg == DXFDotNet::UnitOfLength::FEET)
			return 1.0 / 12;
		else if (unitTrg == DXFDotNet::UnitOfLength::MILLIMETER)
			return 25.4;
		else if (unitTrg == DXFDotNet::UnitOfLength::CENTIMETER)
			return 2.54;
		else if (unitTrg == DXFDotNet::UnitOfLength::METER)
			return 0.0254;
	}
	else if (unitSrc == DXFDotNet::UnitOfLength::FEET)
	{
		if (unitTrg == DXFDotNet::UnitOfLength::INCH)
			return 12.0;
		else if (unitTrg == DXFDotNet::UnitOfLength::FEET)
			return 1.0;
		else if (unitTrg == DXFDotNet::UnitOfLength::MILLIMETER)
			return 304.8;
		else if (unitTrg == DXFDotNet::UnitOfLength::CENTIMETER)
			return 30.48;
		else if (unitTrg == DXFDotNet::UnitOfLength::METER)
			return 0.3048;
	}
	else if (unitSrc == DXFDotNet::UnitOfLength::MILLIMETER)
	{
		if (unitTrg == DXFDotNet::UnitOfLength::INCH)
			return 1.0 / 25.4;
		else if (unitTrg == DXFDotNet::UnitOfLength::FEET)
			return 1.0 / 25.4 / 12;
		else if (unitTrg == DXFDotNet::UnitOfLength::MILLIMETER)
			return 1.0;
		else if (unitTrg == DXFDotNet::UnitOfLength::CENTIMETER)
			return 0.1;
		else if (unitTrg == DXFDotNet::UnitOfLength::METER)
			return 0.001;
	}
	else if (unitSrc == DXFDotNet::UnitOfLength::CENTIMETER)
	{
		if (unitTrg == DXFDotNet::UnitOfLength::INCH)
			return 1.0 / 2.54;
		else if (unitTrg == DXFDotNet::UnitOfLength::FEET)
			return 1.0 / 2.54 / 12;
		else if (unitTrg == DXFDotNet::UnitOfLength::MILLIMETER)
			return 10;
		else if (unitTrg == DXFDotNet::UnitOfLength::CENTIMETER)
			return 1.0;
		else if (unitTrg == DXFDotNet::UnitOfLength::METER)
			return 0.01;
	}
	else if (unitSrc == DXFDotNet::UnitOfLength::METER)
	{
		if (unitTrg == DXFDotNet::UnitOfLength::INCH)
			return 1.0 / 0.0254;
		else if (unitTrg == DXFDotNet::UnitOfLength::FEET)
			return 1.0 / 0.0254 / 12;
		else if (unitTrg == DXFDotNet::UnitOfLength::MILLIMETER)
			return 1000.0;
		else if (unitTrg == DXFDotNet::UnitOfLength::CENTIMETER)
			return 100.0;
		else if (unitTrg == DXFDotNet::UnitOfLength::METER)
			return 1.0;
	}

	return 1.0;
}

UnE::Geometry::Vertex2D^ DXFLoader::ReadDXFVPort(DXF::TABLES::TableManager* pTblMgr, double& dViewportWeight, double dScale)
{
	DXF::TABLES::VPort* pVPort = pTblMgr->GetVPort();
	DXF::TABLES::VPort::Entity* pEntity = pVPort->GetActiveEntity();

	if (!pEntity) 
	{
		void* pNod = 0;
		pEntity = pVPort->GetEntity(pNod);
		if (!pEntity) return nullptr;
	}

	double dCenterX, dCenterY, dHeight, dWidth;
	double dAxisX[3], dAxisY[3];

	pEntity->GetCenterPoint(&dCenterX,&dCenterY);
	pEntity->GetUCSAxisX(&dAxisX[0],&dAxisX[1],&dAxisX[2]);
	pEntity->GetUCSAxisY(&dAxisY[0],&dAxisY[1],&dAxisY[2]);
	dHeight = pEntity->GetViewportHeight();
	dWidth  = pEntity->GetViewportAspect() * dHeight;

	UnE::Geometry::Vertex2D^ v0 = gcnew UnE::Geometry::Vertex2D(0.0, 0.0);
	UnE::Geometry::Vertex2D^ vX = gcnew UnE::Geometry::Vertex2D(dAxisX[0], dAxisX[1]);
	UnE::Geometry::Vertex2D^ vY = gcnew UnE::Geometry::Vertex2D(dAxisY[0], dAxisY[1]);

	double dLen1 = v0->GetDistance(vX);
	double dLen2 = v0->GetDistance(vY);

	double dBLX = dCenterX - dWidth / 2;
	double dBLY = dCenterY - dHeight / 2;
	double dBRX = dCenterX + dWidth / 2;
	double dBRY = dBLY;
	double dTLX = dBLX;
	double dTLY = dCenterY + dHeight / 2;

	/*if (dWidth / m_ctrl->Size.Width > dHeight / m_ctrl->Size.Height)
		m_ctrl->SetViewportWeight(dWidth / m_ctrl->Size.Width);
	else
		m_ctrl->SetViewportWeight(dHeight / m_ctrl->Size.Height);*/
	if (dWidth / m_ctrl->Size.Width > dHeight / m_ctrl->Size.Height)
		dViewportWeight = dWidth / m_ctrl->Size.Width;
	else 
		dViewportWeight = dHeight / m_ctrl->Size.Height;

	return gcnew UnE::Geometry::Vertex2D(dCenterX * dScale, dCenterY * dScale);
}

void DXFLoader::ReadDXFLType(DXF::TABLES::TableManager* pTblMgr)
{
	DXF::TABLES::LType* pLType = pTblMgr->GetLType();

	void* pID = 0;
	DXF::TABLES::LType::Entity* pEntity = pLType->GetEntityFromID(pID);

	while (pEntity)
	{
		AddLineTypeFromDXF(pEntity);
		pEntity = pLType->GetEntityFromID(pID);
	}
}

void DXFLoader::AddLineTypeFromDXF(DXF::TABLES::LType::Entity* pEntity)
{
	wchar_t* wstrLineType = pEntity->GetTypeName();

	System::String^ strLineType = gcnew System::String(wstrLineType);

	bool isDash = strLineType->IndexOf(L"Dash", System::StringComparison::CurrentCultureIgnoreCase) >= 0;
	int nDotIndex = strLineType->IndexOf(L"Dot", System::StringComparison::CurrentCultureIgnoreCase);

	bool isDot = false, isDotDot = false;

	if (nDotIndex >= 0)
	{
		isDot = true;
		isDotDot = strLineType->IndexOf(L"Dot", nDotIndex + 1, System::StringComparison::CurrentCultureIgnoreCase) >= 0;
	}
		
	System::Drawing::Drawing2D::DashStyle lineStyle = System::Drawing::Drawing2D::DashStyle::Solid;

	if (isDotDot)
		lineStyle = System::Drawing::Drawing2D::DashStyle::DashDotDot;
	else if (isDash)
	{
		if (isDot)
			lineStyle = System::Drawing::Drawing2D::DashStyle::DashDot;
		else
			lineStyle = System::Drawing::Drawing2D::DashStyle::Dash;
	}
	else if (isDot)
		lineStyle = System::Drawing::Drawing2D::DashStyle::Dot;

	LineType^ lineType = gcnew LineType(m_ctrl, lineStyle, 1);
	lineType->LineTypeName = strLineType;
	m_dicLineType[strLineType] = lineType;

}

void DXFLoader::ReadDXFLayer(DXF::HEADER::CHeader* pHdrMgr, DXF::TABLES::TableManager* pTblMgr)
{
	void* pNod = 0;
	DXF::TABLES::Layer* pLayer = pTblMgr->GetLayer();
	DXF::TABLES::Layer::Entity* pEntity = pLayer->GetEntity(pNod);

	for (;pEntity;pEntity=pLayer->GetEntity(pNod))
	{
		Layer^ targetLayer = GetLayer(gcnew System::String(pEntity->GetLayerName()));
		
		if (targetLayer == nullptr) 
		{
			targetLayer = m_ctrl->GetShapeFactory()->CreateLayer((IShapeOwner^)m_ctrl);
			//targetLayer = gcnew Layer();
			targetLayer->LayerName = gcnew System::String(pEntity->GetLayerName());

			m_dicLayer[targetLayer->LayerName] = targetLayer;
			m_arrLayer->Add(targetLayer);
		}

		targetLayer->Hidden = pEntity->IsHidden();
		targetLayer->Frozen = pEntity->IsFrozen();
		targetLayer->Lock = pEntity->IsLocked();

		int nACI = pEntity->GetColor();

		if (nACI < 0)
		{
			targetLayer->Hidden = true;	// Hidden Layer
		}
		else
		{
			int nRed, nGreen, nBlue;
			if (PenWorld::ACI::ACIToRGB(nACI,&nRed,&nGreen,&nBlue))
			{
				targetLayer->LineColor = System::Drawing::Color::FromArgb(nRed, nGreen, nBlue);
			}
		}

		wchar_t* wstrLineType = pEntity->GetLineType();
		System::String^ strLineType = gcnew System::String(wstrLineType);

		if (m_dicLineType->ContainsKey(strLineType))
		{
			targetLayer->SetLineType(m_dicLineType[strLineType]);
		}
	}
}

Layer^ DXFLoader::GetLayer(System::String^ strLayerName)
{
	if (m_dicLayer->ContainsKey(strLayerName))
		return m_dicLayer[strLayerName];

	return nullptr;
}

void DXFLoader::ReadDXFEntity(DXF::ENTITIES::EntityManager* pEntMgr, DXF::BLOCKS::BlockManager* pBlkMgr, DXF::TABLES::TableManager* pTblMgr, COLORREF* pCol, double dScale)
{
	void* pNod = 0;
	DXF::ENTITIES::Entity* pEntity = pEntMgr->GetEntity(pNod);

	for (;pEntity;pEntity=pEntMgr->GetEntity(pNod))
	{
		wchar_t* strEntityType = pEntity->GetEntityType();

		Layer^ layer = GetLayer(gcnew System::String(pEntity->GetOwnLayer()));
		if (layer == nullptr)
			continue;

		Shape^ obj = nullptr;

		if (!wcscmp(strEntityType, L"ARC"))
		{
			obj = (Shape^)GetArcFromDXF((DXF::ENTITIES::Arc*)pEntity, dScale, layer);
		}
		else if (!wcscmp(strEntityType, L"CIRCLE"))
		{
			obj = (Shape^)GetCircleFromDXF((DXF::ENTITIES::Circle*)pEntity, dScale, layer);
		}
		else if (!wcscmp(strEntityType, L"ELLIPSE"))
		{
			obj = GetEArcFromDXF((DXF::ENTITIES::Ellipse*)pEntity, dScale, layer);
		}
		else if (!wcscmp(strEntityType, L"LINE"))
		{
			obj = (Shape^)GetLineFromDXF((DXF::ENTITIES::Line*)pEntity, dScale, layer);
		}
		else if (!wcscmp(strEntityType, L"LWPOLYLINE"))
		{
			obj = (Shape^)GetPolyLineFromDXF((DXF::ENTITIES::PolyLine*)pEntity, dScale, layer);
		}
		else if (!wcscmp(strEntityType, L"POLYLINE"))
		{
			obj = (Shape^)GetPolyLineFromDXF((DXF::ENTITIES::PolyLine*)pEntity, dScale, layer);
		}
		else if (!wcscmp(strEntityType, L"HATCH"))
		{
			obj = (Shape^)GetHatchFromDXF((DXF::ENTITIES::Hatch*)pEntity, dScale, layer);
		}
		else if (!wcscmp(strEntityType, L"TEXT"))
		{
			obj = (Shape^)GetTextFromDXF((DXF::ENTITIES::Text*)pEntity, pTblMgr, dScale, layer);
		}
		else if (!wcscmp(strEntityType, L"MTEXT"))
		{
			obj = (Shape^)GetMTextFromDXF((DXF::ENTITIES::MText*)pEntity, pTblMgr, dScale, layer);
		}
		else if (!wcscmp(strEntityType, L"INSERT"))
		{
			GetEntityFromInsert((DXF::ENTITIES::Insert*)pEntity, pBlkMgr, pTblMgr, dScale);
			continue;
		}

		if (obj != nullptr) 
		{
			obj->ID = pEntity->GetHandle();
			layer->Add(obj);
			
			//obj->SetLineTypeOption(Shape::ControlType::BYLAYER);

			SetObjectColor(obj, layer, pEntity, pCol);

			if (obj->GetShapeType() != Shape::ShapeType::HATCH)
			{
				// Hatch에서 사용하기 위하여 저장
				m_dicShape[(__int64)pEntity] = obj;
			}
		}
	}
}

Line^ DXFLoader::GetLineFromDXF(DXF::ENTITIES::Line* pLineSrc, double dScale, Layer^ layer)
{
	double dCoordBegin[3], dCoordEnd[3];
	pLineSrc->GetCoord(dCoordBegin, dCoordEnd);

	for (int i=0;i<3;i++)
	{
		dCoordBegin[i] *= dScale;
		dCoordEnd[i] *= dScale;
	}
		
	Line^ line = m_ctrl->GetShapeFactory()->CreateLine();
	
	line->Begin = gcnew UnE::Geometry::Vertex2D(dCoordBegin[0], dCoordBegin[1]);
	line->End = gcnew UnE::Geometry::Vertex2D(dCoordEnd[0], dCoordEnd[1]);

	// 기본 레이어는 영역 계산에서 제외
	//if (wcscmp(pLineSrc->GetOwnLayer(), L"0"))
	if (!layer->Hidden && !layer->Frozen && wcscmp(pLineSrc->GetOwnLayer(), L"0"))
	{
		if (m_isInitArea)
		{
			if (m_vObjectTL->x > line->Begin->x) m_vObjectTL->x = line->Begin->x;
			if (m_vObjectTL->x > line->End->x) m_vObjectTL->x = line->End->x;
			if (m_vObjectTL->y < line->Begin->y) m_vObjectTL->y = line->Begin->y;
			if (m_vObjectTL->y < line->End->y) m_vObjectTL->y = line->End->y;

			if (m_vObjectBR->x < line->Begin->x) m_vObjectBR->x = line->Begin->x;
			if (m_vObjectBR->x < line->End->x) m_vObjectBR->x = line->End->x;
			if (m_vObjectBR->y > line->Begin->y) m_vObjectBR->y = line->Begin->y;
			if (m_vObjectBR->y > line->End->y) m_vObjectBR->y = line->End->y;

			m_vObjectBL->x = m_vObjectTL->x;
			m_vObjectBL->y = m_vObjectBR->y;
		}
		else
		{
			m_vObjectTL->x = m_vObjectBL->x = m_vObjectBR->x = line->Begin->x;
			m_vObjectTL->y = m_vObjectBL->y = m_vObjectBR->y = line->Begin->y;

			if (m_vObjectTL->x > line->End->x) m_vObjectTL->x = line->End->x;
			if (m_vObjectTL->y < line->End->y) m_vObjectTL->y = line->End->y;

			if (m_vObjectBR->x < line->End->x) m_vObjectBR->x = line->End->x;
			if (m_vObjectBR->y > line->End->y) m_vObjectBR->y = line->End->y;

			m_vObjectBL->x = m_vObjectTL->x;
			m_vObjectBL->y = m_vObjectBR->y;

			m_isInitArea = true;
		}
	}

	return line;
}

Arc^ DXFLoader::GetArcFromDXF(DXF::ENTITIES::Arc* pArcSrc, double dScale, Layer^ layer)
{
	Arc^ arcTrg = GetCircleFromDXF(pArcSrc, dScale, layer);
	if (arcTrg == nullptr)
		return nullptr;

	double dBeginAngle, dEndAngle;
	pArcSrc->GetAngle(&dBeginAngle,&dEndAngle);

	double dAngle = dEndAngle - dBeginAngle;
	if (dAngle < 0) dAngle += 360.0;

	if (m_ctrl->DownToTop())
	{
		arcTrg->BeginAngle = dEndAngle - 360.0;
		arcTrg->ArcAngle = -dAngle;
	}
	else
	{
		arcTrg->BeginAngle = 360.0 - dEndAngle;
		arcTrg->ArcAngle = dAngle;
	}

	arcTrg->IsCircle = false;

	return arcTrg;
}

Arc^ DXFLoader::GetCircleFromDXF(DXF::ENTITIES::Circle* pCircleSrc, double dScale, Layer^ layer)
{
	double dArrCenter[3], dRadius;
	
	dRadius = pCircleSrc->GetRadius();
	if (dRadius <= 0.0) return nullptr;
	
	pCircleSrc->GetCenterPoint(&dArrCenter[0],&dArrCenter[1],&dArrCenter[2]);

	dRadius *= dScale;

	for (int i=0;i<3;i++)
		dArrCenter[i] *= dScale;

	Arc^ arcTrg = m_ctrl->GetShapeFactory()->CreateArc();

	arcTrg->Center = gcnew Vertex2D(dArrCenter[0], dArrCenter[1]);
	arcTrg->Radius = dRadius;
	arcTrg->IsCircle = true;

	// 기본 레이어는 영역 계산에서 제외
	//if (wcscmp(pCircleSrc->GetOwnLayer(), L"0"))
	if (!layer->Hidden && !layer->Frozen && wcscmp(pCircleSrc->GetOwnLayer(), L"0"))
	{
		if (m_isInitArea)
		{
			if (m_vObjectTL->x > dArrCenter[0] - dRadius) m_vObjectTL->x = dArrCenter[0] - dRadius;
			if (m_vObjectTL->y < dArrCenter[1] + dRadius) m_vObjectTL->y = dArrCenter[1] + dRadius;
			if (m_vObjectBR->x < dArrCenter[0] + dRadius) m_vObjectBR->x = dArrCenter[0] + dRadius;
			if (m_vObjectBR->y > dArrCenter[1] - dRadius) m_vObjectBR->y = dArrCenter[1] - dRadius;
		}
		else
		{
			m_vObjectTL->x = dArrCenter[0] - dRadius;
			m_vObjectTL->y = dArrCenter[1] + dRadius;
			m_vObjectBR->x = dArrCenter[0] + dRadius;
			m_vObjectBR->y = dArrCenter[1] - dRadius;

			m_isInitArea = true;
		}

		m_vObjectBL->x = m_vObjectTL->x;
		m_vObjectBL->y = m_vObjectBR->y;
	}

	return arcTrg;
}

// Degree
static double GetEllipseAngle(double dParameter, double dRatio)
{
	double dAdd = 0.0;

	if (dParameter > System::Math::PI * 2)
	{
		while (dParameter > System::Math::PI * 2)
		{
			dParameter -= System::Math::PI * 2;
			dAdd += 360.0;
		}
	}
	else if (dParameter < -System::Math::PI * 2)
	{
		while (dParameter < -System::Math::PI * 2)
		{
			dParameter += System::Math::PI * 2;
			dAdd -= 360.0;
		}
	}

	double dData = System::Math::Tan(dParameter) * dRatio;
	double dAngle = System::Math::Atan(dData) * 180 / System::Math::PI;

	// 각도 보정
	if (dParameter > System::Math::PI / 2 && dParameter <= System::Math::PI * 1.5)
		dAngle += 180.0;
	else if (dParameter > System::Math::PI * 1.5)
		dAngle += 360.0;

	return dAngle + dAdd;
}

// Degree
static void GetEllipseAngle(DXF::ENTITIES::Ellipse* pEllipseSrc, double dRatio, double& dBeginAngle, double& dEndAngle)
{
	double dBeginParameter, dEndParameter;
	pEllipseSrc->GetParameter(&dBeginParameter, &dEndParameter);

	dBeginAngle = GetEllipseAngle(dBeginParameter, dRatio);
	dEndAngle = GetEllipseAngle(dEndParameter, dRatio);
}

EArc^ DXFLoader::GetEArcFromDXF(DXF::ENTITIES::Ellipse* pEllipseSrc, double dScale, Layer^ layer)
{
	double arrCenter[3];
	double dLongRadius[3];
	// Degree
	double dBeginAngle, dEndAngle;
	
	double dRatio = pEllipseSrc->GetRatio();
	pEllipseSrc->GetLongAxisCoord(&dLongRadius[0], &dLongRadius[1], &dLongRadius[2]);
	if (dRatio == 0.0 || (dLongRadius[0] == 0.0 && dLongRadius[1] == 0.0 && dLongRadius[2] == 0.0)) return nullptr;

	GetEllipseAngle(pEllipseSrc, dRatio, dBeginAngle, dEndAngle);
	pEllipseSrc->GetCenterPoint(&arrCenter[0], &arrCenter[1], &arrCenter[2]);

	for (int i=0;i<3;i++)
	{
		arrCenter[i] *= dScale;
		dLongRadius[i] *= dScale;
	}

	Vertex2D^ vCenter = gcnew Vertex2D(arrCenter[0], arrCenter[1]);
	Vertex2D^ vMR = gcnew Vertex2D(arrCenter[0] + dLongRadius[0], arrCenter[1] + dLongRadius[1]);

	double dLen = vCenter->GetDistance(vMR);

	Vertex2D^ vTM = UnE::Geometry::Math::GetRightVertex(vCenter, vMR, dLen * dRatio);
	Vertex2D^ vML = vCenter * 2 - vMR;

	Vertex2D^ vTL = vML + (vTM - vCenter);
	Vertex2D^ vBL = vML - (vTM - vCenter);
	Vertex2D^ vBR = vMR - (vTM - vCenter);

	EArc^ eArc = m_ctrl->GetShapeFactory()->CreateEArc();

	eArc->Width = vBL->GetDistance(vBR);
	eArc->Height = vTL->GetDistance(vBL);
	eArc->TopLeft = vTL;
	eArc->BottomLeft = vBL;
	eArc->BottomRight = vBR;

	Vertex2D^ vR = gcnew Vertex2D(arrCenter[0] + 100.0, arrCenter[1] + 0.0);

	double dXAxisAngle = UnE::Geometry::Math::GetAngle(vMR, vCenter, vR);
	if (vMR->y < vCenter->y)
		dXAxisAngle = UnE::Geometry::Math::_2PI() - dXAxisAngle;

	dXAxisAngle = UnE::Geometry::Math::RadToDeg(dXAxisAngle);
	eArc->XAxisAngle = dXAxisAngle;

	if (System::Math::Abs(dBeginAngle) < UnE::Geometry::Math::HALF_TOLERANCE() && System::Math::Abs(dEndAngle - 360.0) < UnE::Geometry::Math::HALF_TOLERANCE())
	{
		// 완전한 타원
		eArc->IsEllipse = true;
	}
	else
	{
		double dAngle = dEndAngle - dBeginAngle;
		if (dAngle < 0) dAngle += 360.0;

		eArc->IsEllipse = false;

		if (m_ctrl->DownToTop())
		{
			//eArc->BeginAngle = dEndAngle - 360.0;
			eArc->BeginAngle = -dEndAngle;
			eArc->EArcAngle = dAngle;
		}
		else
		{
			eArc->BeginAngle = dBeginAngle;
			eArc->EArcAngle = dAngle;
		}
	}

	// 기본 레이어는 영역 계산에서 제외
	//if (wcscmp(pEllipseSrc->GetOwnLayer(), L"0"))
	if (!layer->Hidden && !layer->Frozen && wcscmp(pEllipseSrc->GetOwnLayer(), L"0"))
	{
		if (m_isInitArea)
		{
			if (m_vObjectTL->x > vTL->x) m_vObjectTL->x = vTL->x;
			if (m_vObjectTL->y < vTL->y) m_vObjectTL->y = vTL->y;
			if (m_vObjectBR->x < vBR->x) m_vObjectBR->x = vBR->x;
			if (m_vObjectBR->y > vBR->y) m_vObjectBR->y = vBR->y;
		}
		else
		{
			m_vObjectTL->x = vTL->x;
			m_vObjectTL->y = vTL->y;
			m_vObjectBR->x = vBR->x;
			m_vObjectBR->y = vBR->y;

			m_isInitArea = true;
		}

		m_vObjectBL->x = m_vObjectTL->x;
		m_vObjectBL->y = m_vObjectBR->y;
	}

	return eArc;
}

PolyLine^ DXFLoader::GetPolyLineFromDXF(DXF::ENTITIES::PolyLine* pPolySrc, double dScale, Layer^ layer)
{	
	void* pNod = 0;
	double dX, dY, dBulge;

	System::Collections::ArrayList^ arrVertex = gcnew System::Collections::ArrayList();
	System::Collections::ArrayList^ arrPolyVertex = gcnew System::Collections::ArrayList();

	while (pPolySrc->GetPoint(pNod, &dX, &dY, &dBulge))
	{
		//polyLine->UpdatePoint(nIndex++, dX, dY);
		Vertex3D^ vertex = gcnew Vertex3D(dX, dY, dBulge);
		arrVertex->Add(vertex);
	}

	int nVertexCount = arrVertex->Count;

	if (nVertexCount == 0)
		return nullptr;

	for (int i=0;i<nVertexCount;i++)
	{
		Vertex3D^ vCurrent = (Vertex3D^)arrVertex[i];

		if (vCurrent->z == 0.0)	// 직선 구간
		{
			arrPolyVertex->Add(gcnew Vertex2D(vCurrent->x * dScale, vCurrent->y * dScale));
		}
		else					// Arc 구간
		{
			if (i == nVertexCount - 1 && !pPolySrc->GetClosed())
				continue;

			int nNextIndex = i == nVertexCount - 1 ? 0 : i + 1;
			Vertex3D^ vNext = (Vertex3D^)arrVertex[nNextIndex];

			Vertex2D^ v1 = gcnew Vertex2D(vCurrent->x, vCurrent->y);
			Vertex2D^ v2 = gcnew Vertex2D(vNext->x, vNext->y);

			bool isClockWise;
			Vertex2D^ vCenter = GetPolylineArcCenter(v1, v2, vCurrent->z, isClockWise);

			/*Vertex2D^ v3 = (v1 + v2) / 2;

			double dLen = v1->GetDistance(v2);
			double s = dLen / 2 * vCurrent->z;

			double radius = System::Math::Abs((System::Math::Pow(dLen / 2, 2) + System::Math::Pow(s, 2)) / (2 * s));

			double dCos   = (2 * radius * radius - dLen * dLen) / 2 / radius / radius;
			if (dCos < -1.0) dCos = -1.0;
			else if (dCos > 1.0) dCos = 1.0;

			//double dTheta = acos((2 * radius * radius - dLen * dLen) / 2 / radius / radius);
			double dTheta = System::Math::Acos(dCos);
			double dAngle = (System::Math::PI - dTheta) / 2;
			dLen = radius * System::Math::Sin(dAngle);

			bool isClockWise = vCurrent->z > 0.0 ? false : true;
			Vertex2D^ vCenter = UnE::Geometry::Math::GetRightVertex(v3, v1, isClockWise ? dLen : -dLen);*/

			v1->x *= dScale;
			v1->y *= dScale;
			v2->x *= dScale;
			v2->y *= dScale;
			vCenter->x *= dScale;
			vCenter->y *= dScale;

			CreateArcVertex(arrPolyVertex, v1, vCenter, v2, 30, isClockWise);
		}
	}

	DXFDotNet::PolyLine^ polyLine = m_ctrl->GetShapeFactory()->CreatePolyLine();;

	if (pPolySrc->GetClosed())
	{
		Vertex2D^ vFirst = (Vertex2D^)arrPolyVertex[0];
		arrPolyVertex->Add(gcnew Vertex2D(vFirst->x, vFirst->y));
	}

	polyLine->SetVertex(arrPolyVertex);

	// 기본 레이어는 영역 계산에서 제외
	//if (wcscmp(pPolySrc->GetOwnLayer(), L"0"))
	if (!layer->Hidden && !layer->Frozen && wcscmp(pPolySrc->GetOwnLayer(), L"0"))
	{
		for each (Vertex2D^ vertex in arrPolyVertex)
		{
			if (m_isInitArea)
			{
				if (m_vObjectTL->x > vertex->x) m_vObjectTL->x = vertex->x;
				if (m_vObjectTL->y < vertex->y) m_vObjectTL->y = vertex->y;
				if (m_vObjectBR->x < vertex->x) m_vObjectBR->x = vertex->x;
				if (m_vObjectBR->y > vertex->y) m_vObjectBR->y = vertex->y;
			}
			else
			{
				m_vObjectTL->x = vertex->x;
				m_vObjectTL->y = vertex->y;
				m_vObjectBR->x = vertex->x;
				m_vObjectBR->y = vertex->y;

				m_isInitArea = true;
			}

			m_vObjectBL->x = m_vObjectTL->x;
			m_vObjectBL->y = m_vObjectBR->y;
		}
	}

	return polyLine;
}

// Polyline내의 Arc 중심점을 구한다.
// v1 : Arc의 시작점
// v2 : Arc의 끝점
// bulge : Arc 매개변수
// isClockWise : Arc의 진행방향이 시계방향인가?
Vertex2D^ DXFLoader::GetPolylineArcCenter(Vertex2D^ v1, Vertex2D^ v2, double bulge, bool% isClockWise)
{
	Vertex2D^ v3 = (v1 + v2) / 2;

	double dLen = v1->GetDistance(v2);
	double s = dLen / 2 * bulge;

	Vertex2D^ vM1 = UnE::Geometry::Math::GetRightVertex(v3, v1, s);
	Vertex2D^ vM2 = UnE::Geometry::Math::GetRightVertex(v3, v1, -s);

	isClockWise = bulge > 0.0 ? false : true;

	Vertex2D^ vCenter;
	Arc2D^ arc = gcnew Arc2D(v1, vM1, v2);

	if (isClockWise == arc->IsClockWise())
		vCenter = arc->GetCenter();
	else
		vCenter = v3 * 2 - arc->GetCenter();

	return vCenter;
}

// hatch의 Polygon이 시작점과 끝점이 같을 경우 끝점을 없앤다.
static void CheckHatchPolygon(Hatch^ hatch)
{
	if (hatch == nullptr)
		return;

	int nPointSize = hatch->GetPointSize();

	if (nPointSize <= 3)
		return;

	float xBegin, yBegin, xEnd, yEnd;

	if (!hatch->GetPoint(0, xBegin, yBegin))
		return;

	if (!hatch->GetPoint(nPointSize - 1, xEnd, yEnd))
		return;

	Vertex2F^ vBegin = gcnew Vertex2F(xBegin, yBegin);
	Vertex2F^ vEnd = gcnew Vertex2F(xEnd, yEnd);

	// 시작점과 끝점이 같을 경우
	if (vBegin->GetDistance(vEnd) <= UnE::Geometry::Math::HALF_TOLERANCE())
	{
		float x, y;
		System::Collections::ArrayList^ arrPoints = gcnew System::Collections::ArrayList();

		for (int i = 0; i < nPointSize - 1; i++)
		{
			if (!hatch->GetPoint(i, x, y))
				return;

			arrPoints->Add(gcnew Vertex2F(x, y));
		}

		// 끝점을 제외하고 새로 만든다.
		hatch->SetPointSize(nPointSize - 1);

		for (int i = 0; i < nPointSize - 1; i++)
		{
			Vertex2F^ vertex = (Vertex2F^)arrPoints[i];
			hatch->UpdatePoint(i, vertex->x, vertex->y);
		}
		hatch->UpdatePoint(true);
		//hatch->CalcGLBuffer();
	}
}

/*enum FillType {SOLID = -1, AR_CONC = 0, AR_HBONE = 1, AR_SAND = 2, CLAY = 3,
BRICK = 4, GRAVEL = 5, HONEY = 6, JIS_LC_20 = 7,
STEEL = 8, ANSI31 = 9
};*/
Hatch^ DXFLoader::GetHatchFromDXF(DXF::ENTITIES::Hatch* pHatchSrc, double dScale, Layer^ layer)
{
	/*PenWorld::Hatch* pHatchTrg = new PenWorld::Hatch;

	if (pHatchSrc->IsSolidType()) pHatchTrg->SetFillType(PenWorld::SOLID);
	else
	{
		std::string& strPattern = pHatchSrc->GetHatchPatternName();

		if (strPattern == "AR-CONC") pHatchTrg->SetFillType(PenWorld::AR_CONC);
		else if (strPattern == "AR-HBONE") pHatchTrg->SetFillType(PenWorld::AR_HBONE);
		else if (strPattern == "AR-SAND") pHatchTrg->SetFillType(PenWorld::AR_SAND);
		else if (strPattern == "CLAY") pHatchTrg->SetFillType(PenWorld::CLAY);
		else if (strPattern == "BRICK") pHatchTrg->SetFillType(PenWorld::BRICK);
		else if (strPattern == "GRAVEL") pHatchTrg->SetFillType(PenWorld::GRAVEL);
		else if (strPattern == "HONEY") pHatchTrg->SetFillType(PenWorld::HONEY);
		else if (strPattern == "JIS_LC_20") pHatchTrg->SetFillType(PenWorld::JIS_LC_20);
		else if (strPattern == "STEEL") pHatchTrg->SetFillType(PenWorld::STEEL);
		else if (strPattern == "ANSI31") pHatchTrg->SetFillType(PenWorld::ANSI31);
		else
		{
		delete pHatchTrg;
		return 0;
		}
	}*/

	Hatch^ hatch = m_ctrl->GetShapeFactory()->CreateHatch();

	void* pID = 0;
	DXF::ENTITIES::Hatch::BoundaryManager* pBoundaryManager;

	while (pBoundaryManager = pHatchSrc->GetBoundaryManager(pID))
	{
		std::list<DXF::ENTITIES::Hatch::Boundary*>::iterator iter = pBoundaryManager->m_list.begin();

		for (; iter != pBoundaryManager->m_list.end(); iter++)
		{
			DXF::ENTITIES::Hatch::Boundary* pBoundary = *iter;
			if (pBoundary == 0)
				continue;

			DXF::ENTITIES::Hatch::Boundary::BoundaryType type = pBoundary->GetBoundaryType();

			if (type == DXF::ENTITIES::Hatch::Boundary::POLYLINE)
			{
				DXF::ENTITIES::Hatch::PolyLineType* pPolyLine = (DXF::ENTITIES::Hatch::PolyLineType*)pBoundary;

				hatch->SetPointSize(pPolyLine->m_nPointSize);

				float tlX = 0.0f, tlY = 0.0f, brX = 0.0f, brY = 0.0f;

				for (int i = 0; i<pPolyLine->m_nPointSize; i++)
				{
					hatch->UpdatePoint(i, (float)(pPolyLine->m_pArrX[i] * dScale), (float)(pPolyLine->m_pArrY[i] * dScale));

					if (i == 0)
					{
						tlX = brX = (float)pPolyLine->m_pArrX[i];
						tlY = brY = (float)pPolyLine->m_pArrY[i];
					}
					else
					{
						if (tlX > (float)pPolyLine->m_pArrX[i])
							tlX = (float)pPolyLine->m_pArrX[i];

						if (tlY < (float)pPolyLine->m_pArrY[i])
							tlY = (float)pPolyLine->m_pArrY[i];

						if (brX < (float)pPolyLine->m_pArrX[i])
							brX = (float)pPolyLine->m_pArrX[i];

						if (brY >(float)pPolyLine->m_pArrY[i])
							brY = (float)pPolyLine->m_pArrY[i];
					}
				}
				hatch->UpdatePoint(true);				

				tlX = (float)(tlX * dScale);
				tlY = (float)(tlY * dScale);
				brX = (float)(brX * dScale);
				brY = (float)(brY * dScale);

				hatch->Center = System::Drawing::PointF((float)((tlX + brX) / 2), (float)((tlY + brY) / 2));
			}
			else if (type == DXF::ENTITIES::Hatch::Boundary::ARCEDGE)
			{
				DXF::ENTITIES::Hatch::ArcEdge* pArc = (DXF::ENTITIES::Hatch::ArcEdge*)pBoundary;
				hatch->Center = System::Drawing::PointF((float)(pArc->m_ptCenter.m_pt[0] * dScale), (float)(pArc->m_ptCenter.m_pt[1] * dScale));
			}
			else if (type == DXF::ENTITIES::Hatch::Boundary::EARCEDGE)
			{
				DXF::ENTITIES::Hatch::EArcEdge* pEArc = (DXF::ENTITIES::Hatch::EArcEdge*)pBoundary;
				hatch->Center = System::Drawing::PointF((float)(pEArc->m_dCenterPoint[0] * dScale), (float)(pEArc->m_dCenterPoint[1] * dScale));
			}
			else if (type == DXF::ENTITIES::Hatch::Boundary::LINEEDGE)
			{
				GetHatchBoundaryFromLineEdge(hatch, pBoundary, dScale);
				/*DXF::ENTITIES::Hatch::LineEdge* pLine = (DXF::ENTITIES::Hatch::LineEdge*)pBoundary;

				bool isFirst = true;
				float tlX = 0.0f, tlY = 0.0f, brX = 0.0f, brY = 0.0f;

				std::list<Utility::Vertex2D>::iterator pIter = pLine->m_listBeginPoint.begin();

				for (;pIter != pLine->m_listBeginPoint.end();pIter++)
				{
					Utility::Vertex2D& rVertex = *pIter;

					if (isFirst)
					{
						isFirst = false;
						tlX = brX = (float)rVertex.m_pt[0];
						tlY = brY = (float)rVertex.m_pt[1];
					}
					else
					{
						if (tlX > rVertex.m_pt[0])
						tlX = (float)rVertex.m_pt[0];

						if (tlY < rVertex.m_pt[1])
						tlY = (float)rVertex.m_pt[1];

						if (brX < rVertex.m_pt[0])
						brX = (float)rVertex.m_pt[0];

						if (brY > rVertex.m_pt[1])
						brY = (float)rVertex.m_pt[1];
					}
				}

				pIter = pLine->m_listEndPoint.begin();

				for (;pIter != pLine->m_listEndPoint.end();pIter++)
				{
					Utility::Vertex2D& rVertex = *pIter;

					if (isFirst)
					{
						isFirst = false;
						tlX = brX = (float)rVertex.m_pt[0];
						tlY = brY = (float)rVertex.m_pt[1];
					}
					else
					{
						if (tlX > rVertex.m_pt[0])
						tlX = (float)rVertex.m_pt[0];

						if (tlY < rVertex.m_pt[1])
						tlY = (float)rVertex.m_pt[1];

						if (brX < rVertex.m_pt[0])
						brX = (float)rVertex.m_pt[0];

						if (brY > rVertex.m_pt[1])
						brY = (float)rVertex.m_pt[1];
					}
				}

				tlX = (float)(tlX * dScale);
				tlY = (float)(tlY * dScale);
				brX = (float)(brX * dScale);
				brY = (float)(brY * dScale);

				hatch->Center = System::Drawing::PointF((tlX + brX) / 2, (tlY + brY) / 2);*/
			}
		}
	}

	CheckHatchPolygon(hatch);
	return hatch;
}

static double GetDistance(const Utility::Vertex2D& vertex1, const Utility::Vertex2D& vertex2)
{
	return sqrt((vertex1.m_pt[0] - vertex2.m_pt[0]) * (vertex1.m_pt[0] - vertex2.m_pt[0]) +
		(vertex1.m_pt[1] - vertex2.m_pt[1]) * (vertex1.m_pt[1] - vertex2.m_pt[1]));
}

void DXFLoader::GetHatchBoundaryFromLineEdge(Hatch^ hatch, void* _pLine, double dScale)
{
	DXF::ENTITIES::Hatch::LineEdge* pLine = (DXF::ENTITIES::Hatch::LineEdge*)_pLine;

	int nBeginPointSize = (int)pLine->m_listBeginPoint.size();
	int nEndPointSize = (int)pLine->m_listEndPoint.size();

	if (nBeginPointSize != nEndPointSize || nBeginPointSize == 0)
		return;

	std::vector<std::pair<Utility::Vertex2D, Utility::Vertex2D> > arrLines;
	std::vector<Utility::Vertex2D> arrVertices;
	Utility::Vertex2D *prevBeginVertex = 0, *prevEndVertex = 0;

	bool isFirst = true;
	float tlX = 0.0f, tlY = 0.0f, brX = 0.0f, brY = 0.0f;

	std::list<Utility::Vertex2D>::iterator pIterBegin = pLine->m_listBeginPoint.begin();
	std::list<Utility::Vertex2D>::iterator pIterEnd = pLine->m_listEndPoint.begin();

	for (; pIterBegin != pLine->m_listBeginPoint.end() && pIterEnd != pLine->m_listEndPoint.end(); pIterBegin++, pIterEnd++)
	{
		Utility::Vertex2D& rVertexBegin = *pIterBegin;
		Utility::Vertex2D& rVertexEnd = *pIterEnd;

		if (isFirst)
		{
			isFirst = false;
			tlX = brX = (float)rVertexBegin.m_pt[0];
			tlY = brY = (float)rVertexBegin.m_pt[1];
		}
		else
		{
			if (tlX > rVertexBegin.m_pt[0])
				tlX = (float)rVertexBegin.m_pt[0];

			if (tlY < rVertexBegin.m_pt[1])
				tlY = (float)rVertexBegin.m_pt[1];

			if (brX < rVertexBegin.m_pt[0])
				brX = (float)rVertexBegin.m_pt[0];

			if (brY > rVertexBegin.m_pt[1])
				brY = (float)rVertexBegin.m_pt[1];
		}

		if (tlX > rVertexEnd.m_pt[0])
			tlX = (float)rVertexEnd.m_pt[0];

		if (tlY < rVertexEnd.m_pt[1])
			tlY = (float)rVertexEnd.m_pt[1];

		if (brX < rVertexEnd.m_pt[0])
			brX = (float)rVertexEnd.m_pt[0];

		if (brY > rVertexEnd.m_pt[1])
			brY = (float)rVertexEnd.m_pt[1];

		if (prevBeginVertex == 0 && prevEndVertex == 0)
		{
			arrVertices.push_back(rVertexBegin);
			arrVertices.push_back(rVertexEnd);

			prevBeginVertex = &rVertexBegin;
			prevEndVertex = &rVertexEnd;
		}
		else
		{
			if (GetDistance(*prevEndVertex, rVertexBegin) <= UnE::Geometry::Math::HALF_TOLERANCE())
			{
				arrVertices.push_back(rVertexEnd);

				prevBeginVertex = prevEndVertex;
				prevEndVertex = &rVertexEnd;
			}
			else if (GetDistance(*prevEndVertex, rVertexEnd) <= UnE::Geometry::Math::HALF_TOLERANCE())
			{
				arrVertices.push_back(rVertexBegin);

				prevBeginVertex = prevEndVertex;
				prevEndVertex = &rVertexBegin;
			}
			else
			{
				std::pair<Utility::Vertex2D, Utility::Vertex2D> line;
				line.first = rVertexBegin;
				line.second = rVertexEnd;
				arrLines.push_back(line);
			}
		}
	}

	int nLineCount = arrLines.size();
	int nPrevCount = nLineCount;

	while (nLineCount > 0 && prevBeginVertex != 0 && prevEndVertex != 0)
	{
		for (int i = 0; i < nLineCount; i++)
		{
			std::pair<Utility::Vertex2D, Utility::Vertex2D>& rPair = arrLines[i];
			Utility::Vertex2D& rVertexBegin = rPair.first;
			Utility::Vertex2D& rVertexEnd = rPair.second;

			if (GetDistance(*prevEndVertex, rVertexBegin) <= UnE::Geometry::Math::HALF_TOLERANCE())
			{
				arrVertices.push_back(rVertexEnd);

				prevBeginVertex = prevEndVertex;
				prevEndVertex = &rVertexEnd;
			}
			else if (GetDistance(*prevEndVertex, rVertexEnd) <= UnE::Geometry::Math::HALF_TOLERANCE())
			{
				arrVertices.push_back(rVertexBegin);

				prevBeginVertex = prevEndVertex;
				prevEndVertex = &rVertexBegin;
			}
			else
				continue;

			nLineCount--;
			break;
		}

		if (nLineCount == nPrevCount)
			return;
	}

	int nVertexCount = arrVertices.size();

	if (nVertexCount == 0)
		return;

	hatch->SetPointSize(nVertexCount);

	for (int i = 0; i < nVertexCount; i++)
	{
		Utility::Vertex2D& rVertex = arrVertices[i];
		hatch->UpdatePoint(i, (float)rVertex.m_pt[0], (float)rVertex.m_pt[1]);
	}

	hatch->UpdatePoint(true);

	tlX = (float)(tlX * dScale);
	tlY = (float)(tlY * dScale);
	brX = (float)(brX * dScale);
	brY = (float)(brY * dScale);

	hatch->Center = System::Drawing::PointF((tlX + brX) / 2, (tlY + brY) / 2);
}

Text^ DXFLoader::GetTextFromDXF(DXF::ENTITIES::Text* pTextSrc, DXF::TABLES::TableManager* pTblMgr, double dScale, Layer^ layer)
{
	int nHorizon, nVertical;
	double dFirstAlignPoint[3], dSecondAlignPoint[3];

	pTextSrc->GetJustification(&nHorizon, &nVertical);

	double dStrHeight = pTextSrc->GetHeight();
	pTextSrc->GetFirstAlignPoint(&dFirstAlignPoint[0], &dFirstAlignPoint[1], &dFirstAlignPoint[2]);
	pTextSrc->GetSecondAlignPoint(&dSecondAlignPoint[0], &dSecondAlignPoint[1], &dSecondAlignPoint[2]);
	
	int nACI = pTextSrc->GetColorIndex();

	wchar_t* strStyleName = pTextSrc->GetStyleName();
	DXF::TABLES::Style* pStyle = pTblMgr->GetStyle();
	DXF::TABLES::Style::Entity* pStyleEntity = pStyle->GetEntity(strStyleName);

	Text^ text = m_ctrl->GetShapeFactory()->CreateText();

	if (pStyleEntity)
		text->Font = gcnew System::Drawing::Font(gcnew System::String(pStyleEntity->GetFontName()), (float)dStrHeight);
	else
		text->Font = gcnew System::Drawing::Font(gcnew System::Drawing::FontFamily(System::Drawing::Text::GenericFontFamilies::Serif), (float)dStrHeight);

	text->Title = gcnew System::String(pTextSrc->GetString());

	System::Drawing::Graphics^ g = m_ctrl->CreateGraphics();
    System::Drawing::SizeF sf_font = g->MeasureString(text->Title, text->Font);

	if (nHorizon != 0 || nVertical != 0)
	{
		double dArrPos[3];

		if (nHorizon == 0)
		{
			dArrPos[0] = dSecondAlignPoint[0];
		}
		else if (nHorizon == 1)
		{
			dArrPos[0] = dSecondAlignPoint[0] - sf_font.Width / 2;
		}
		else if (nHorizon == 3 || nHorizon == 4)
		{
			dArrPos[0] = dSecondAlignPoint[0] - sf_font.Width / 2;
			nVertical = 2;
		}
		else if (nHorizon == 5)
		{
			dArrPos[0] = dSecondAlignPoint[0] - sf_font.Width / 2;
		}
		else// if (nHorizon == 2)
		{
			dArrPos[0] = dSecondAlignPoint[0] - sf_font.Width;
		}

		if (nVertical == 0)
		{
			// Cad는 문자의 아래 부분을 기준점으로 한다.
			dArrPos[1] = dSecondAlignPoint[1] + sf_font.Height;// + dTextHeight;;
		}
		else if (nVertical == 1)
		{
			dArrPos[1] = dSecondAlignPoint[1] + sf_font.Height;
		}
		else if (nVertical == 2)
		{
			dArrPos[1] = dSecondAlignPoint[1] + sf_font.Height / 2;
		}
		else// if (nVertical == 3)
		{
			dArrPos[1] = dSecondAlignPoint[1];
		}

		if (nHorizon == 1 || nHorizon == 3 ||
			nHorizon == 4 || nHorizon == 5)	// 중간
			nHorizon = 1;

		if (nVertical == 3) nVertical = 0;
		else if (nVertical == 2) nVertical = 1;
		else nVertical = 2;

		/*System::Drawing::PointF ptPos;
		ptPos.X = (float)(dArrPos[0] * dScale);
		ptPos.Y = (float)(dArrPos[1] * dScale);

		text->Position = ptPos;*/
		UnE::Geometry::Vertex2D^ vPos = gcnew UnE::Geometry::Vertex2D();
		vPos->x = dArrPos[0] * dScale;
		vPos->y = dArrPos[1] * dScale;

		text->SetPosition(vPos);

		if (nVertical == 0)			// 위쪽 정렬
			text->VerticalAlignment = System::Drawing::StringAlignment::Near;
		else if (nVertical == 1)	// 가운데 정렬
			text->VerticalAlignment = System::Drawing::StringAlignment::Center;
		else						// 아래쪽 정렬
			text->VerticalAlignment = System::Drawing::StringAlignment::Far;

		if (nHorizon == 0)			// 왼쪽 정렬
			text->HorizontalAlignment = System::Drawing::StringAlignment::Near;
		else if (nHorizon == 1)		// 가운데 정렬
			text->HorizontalAlignment = System::Drawing::StringAlignment::Center;
		else						// 오른쪽 정렬
			text->HorizontalAlignment = System::Drawing::StringAlignment::Far;
	}
	else 
	{
		/*System::Drawing::PointF ptPos;
		ptPos.X = (float)(dFirstAlignPoint[0] * dScale);
		ptPos.Y = (float)(dFirstAlignPoint[1] * dScale);

		text->Position = ptPos;*/
		UnE::Geometry::Vertex2D^ vPos = gcnew UnE::Geometry::Vertex2D();
		vPos->x = dFirstAlignPoint[0] * dScale;
		// 수직정렬 기본값은 baseline이므로 Font 크기만큼 더한다.
		vPos->y = (dFirstAlignPoint[1] + dStrHeight) * dScale;

		text->SetPosition(vPos);

		// 아래쪽 정렬
		text->VerticalAlignment = System::Drawing::StringAlignment::Far;
		// 왼쪽 정렬
		text->HorizontalAlignment = System::Drawing::StringAlignment::Near;
	}

	text->Angle = pTextSrc->GetTextAngle();

	// 기본 레이어는 영역 계산에서 제외
	//if (wcscmp(pTextSrc->GetOwnLayer(), L"0"))
	if (!layer->Hidden && !layer->Frozen && wcscmp(pTextSrc->GetOwnLayer(), L"0"))
		CheckTextObjectArea(text, nHorizon, nVertical, sf_font.Width, sf_font.Height);

	return text;
}

// nHorizon : 0(왼쪽 정렬), 1(가운데 정렬), 2(오른쪽 정렬)
// nVertical : 0(위쪽 정렬), 1(가운데 정렬), 2(아래쪽 정렬)
void DXFLoader::CheckTextObjectArea(Text^ text, int nHorizon, int nVertical, float fTextWidth, float fTextHeight)
{
	Vertex2D^ vTL = nullptr;
	Vertex2D^ vBL = nullptr;
	Vertex2D^ vBR = nullptr;
	//Vertex2D^ vPos = gcnew Vertex2D(text->Position.X, text->Position.Y);
	Vertex2D^ vPos = text->Position;

	double dTheta = UnE::Geometry::Math::DegToRad(text->Angle);

	if (nHorizon == 0)
	{
		if (nVertical == 0)
		{
			vTL = gcnew Vertex2D(vPos->x, vPos->y);
			vBL = gcnew Vertex2D(vPos->x + fTextHeight * System::Math::Sin(dTheta), vPos->y - fTextHeight * System::Math::Cos(dTheta));
		}
		else if (nVertical == 1)
		{
			vTL = gcnew Vertex2D(vPos->x - fTextHeight / 2 * System::Math::Sin(dTheta), vPos->y + fTextHeight / 2 * System::Math::Cos(dTheta));
			vBL = vPos * 2 - vTL;
		}
		else// if (nVertical == 2)
		{
			vTL = gcnew Vertex2D(vPos->x - fTextHeight * System::Math::Sin(dTheta), vPos->y + fTextHeight * System::Math::Cos(dTheta));
			vBL = vPos;
		}
	}
	else if (nHorizon == 1)
	{
		if (nVertical == 0)
		{
			vTL = gcnew Vertex2D(vPos->x - fTextWidth / 2 * System::Math::Cos(dTheta), vPos->y - fTextWidth / 2 * System::Math::Sin(dTheta));
			vBL = UnE::Geometry::Math::GetRightVertex(vTL, vPos, -fTextHeight);
		}
		else if (nVertical == 1)
		{
			Vertex2D^ vL = gcnew Vertex2D(vPos->x - fTextWidth / 2 * System::Math::Cos(dTheta), vPos->y - fTextWidth / 2 * System::Math::Sin(dTheta));
			vTL = UnE::Geometry::Math::GetRightVertex(vL, vPos, fTextHeight / 2);
			vBL = vL * 2 - vTL;
		}
		else// if (nVertical == 2)
		{
			vBL = gcnew Vertex2D(vPos->x - fTextWidth / 2 * System::Math::Cos(dTheta), vPos->y - fTextWidth / 2 * System::Math::Sin(dTheta));
			vTL = UnE::Geometry::Math::GetRightVertex(vBL, vPos, fTextHeight);
		}
	}
	else// if (nHorizon == 2)
	{
		if (nVertical == 0)
		{
			vTL = gcnew Vertex2D(vPos->x - fTextWidth * System::Math::Cos(dTheta), vPos->y - fTextWidth * System::Math::Sin(dTheta));
			vBL = UnE::Geometry::Math::GetRightVertex(vTL, vPos, -fTextHeight);
		}
		else if (nVertical == 1)
		{
			Vertex2D^ vL = gcnew Vertex2D(vPos->x - fTextWidth * System::Math::Cos(dTheta), vPos->y - fTextWidth * System::Math::Sin(dTheta));
			vTL = UnE::Geometry::Math::GetRightVertex(vL, vPos, fTextHeight / 2);
			vBL = vL * 2 - vTL;
		}
		else// if (nVertical == 2)
		{
			vBL = gcnew Vertex2D(vPos->x - fTextWidth * System::Math::Cos(dTheta), vPos->y - fTextWidth * System::Math::Sin(dTheta));
			vTL = UnE::Geometry::Math::GetRightVertex(vBL, vPos, fTextHeight);
		}
	}

	vBR = UnE::Geometry::Math::GetRightVertex(vBL, vTL, -fTextWidth);
	array<Vertex2D^>^ arrVertex = {vTL, vBL, vBR};

	for each (Vertex2D^ vertex in arrVertex)
	{
		if (m_isInitArea)
		{
			if (m_vObjectTL->x > vertex->x) m_vObjectTL->x = vertex->x;
			if (m_vObjectTL->y < vertex->y) m_vObjectTL->y = vertex->y;
			if (m_vObjectBR->x < vertex->x) m_vObjectBR->x = vertex->x;
			if (m_vObjectBR->y > vertex->y) m_vObjectBR->y = vertex->y;
		}
		else
		{
			m_vObjectTL->x = vertex->x;
			m_vObjectTL->y = vertex->y;
			m_vObjectBR->x = vertex->x;
			m_vObjectBR->y = vertex->y;

			m_isInitArea = true;
		}

		m_vObjectBL->x = m_vObjectTL->x;
		m_vObjectBL->y = m_vObjectBR->y;
	}
}

// MText는 줄바꿈 문자로 "\\P"를 사용하는데 이를 "\r\n"으로 바꾼다.
static System::String^ ParseMText(wchar_t* strText)
{
	System::String^ strMText = gcnew System::String(strText);
	strMText = strMText->Replace(gcnew System::String(L"\\P"), gcnew System::String(L"\r\n"));

	int nIndex = strMText->IndexOf(L"{\\f");

	while (nIndex >= 0)
	{
		int nIndex1 = strMText->IndexOf(L';', nIndex + 1);
		int nIndex2 = strMText->IndexOf(L'}', nIndex + 1);

		if (nIndex1 < 0 || nIndex2 < 2 || nIndex2 <= nIndex1)
			break;

		strMText = System::String::Format(L"{0}{1}{2}", strMText->Substring(0, nIndex), strMText->Substring(nIndex1 + 1, nIndex2 - nIndex1 - 1), strMText->Substring(nIndex2 + 1));
		nIndex = strMText->IndexOf(L"{\\f");
	}

	return strMText;
}

Text^ DXFLoader::GetMTextFromDXF(DXF::ENTITIES::MText* pTextSrc, DXF::TABLES::TableManager* pTblMgr, double dScale, Layer^ layer)
{
	int nHorizon, nVertical;
	double dInsertionPoint[3];

	pTextSrc->GetAttachment(&nHorizon, &nVertical);
	double dStrHeight = pTextSrc->GetHeight();
	pTextSrc->GetInsertionPoint(&dInsertionPoint[0], &dInsertionPoint[1], &dInsertionPoint[2]);
	int nACI = pTextSrc->GetColorIndex();

	double dAreaWidth, dAreaHeight;
	pTextSrc->GetArea(&dAreaWidth, &dAreaHeight);

	double dLineSpace = pTextSrc->GetLineSpace() * dStrHeight * 0.65;

	System::String^ strMText = ParseMText(pTextSrc->GetString());
	
	const wchar_t* strStyleName = pTextSrc->GetStyleName();
	DXF::TABLES::Style* pStyle = pTblMgr->GetStyle();
	DXF::TABLES::Style::Entity* pStyleEntity = pStyle->GetEntity(strStyleName);

	Text^ text = m_ctrl->GetShapeFactory()->CreateText();

	if (pStyleEntity)
		text->Font = gcnew System::Drawing::Font(gcnew System::String(pStyleEntity->GetFontName()), (float)dStrHeight);
	else
		text->Font = gcnew System::Drawing::Font(gcnew System::Drawing::FontFamily(System::Drawing::Text::GenericFontFamilies::Serif), (float)dStrHeight);

	text->Title = strMText;

	System::Drawing::Graphics^ g = m_ctrl->CreateGraphics();
    System::Drawing::SizeF sf_font = g->MeasureString(text->Title, text->Font);
	dAreaHeight = sf_font.Height;

	/*System::Drawing::PointF ptPos;
	ptPos.X = (float)(dInsertionPoint[0] * dScale);
	ptPos.Y = (float)(dInsertionPoint[1] * dScale);

	text->Position = ptPos;*/
	Vertex2D^ vPos = gcnew Vertex2D();
	vPos->x = dInsertionPoint[0] * dScale;
	vPos->y = dInsertionPoint[1] * dScale;

	text->SetPosition(vPos);

	if (nVertical == 0)			// 위쪽 정렬
		text->VerticalAlignment = System::Drawing::StringAlignment::Near;
	else if (nVertical == 1)	// 가운데 정렬
		text->VerticalAlignment = System::Drawing::StringAlignment::Center;
	else						// 아래쪽 정렬
		text->VerticalAlignment = System::Drawing::StringAlignment::Far;

	if (nHorizon == 0)			// 왼쪽 정렬
		text->HorizontalAlignment = System::Drawing::StringAlignment::Near;
	else if (nHorizon == 1)		// 가운데 정렬
		text->HorizontalAlignment = System::Drawing::StringAlignment::Center;
	else						// 오른쪽 정렬
		text->HorizontalAlignment = System::Drawing::StringAlignment::Far;

	// MText는 Text와 달리 GetTextAngle()이 아니라 X축 방향 벡터를 통하여 회전값을 얻어온다.
	double dXAxisVectorX, dXAxisVectorY, dXAxisVectorZ;
	pTextSrc->GetAxisVector(0, dXAxisVectorX, dXAxisVectorY, dXAxisVectorZ);

	double dRad = System::Math::Acos(dXAxisVectorX);
	text->Angle = UnE::Geometry::Math::RadToDeg(dRad);

	if (!layer->Hidden && !layer->Frozen && layer->LayerName != L"0")
		CheckTextObjectArea(text, nHorizon, nVertical, (float)dAreaWidth, (float)dAreaHeight);

	return text;
}

void DXFLoader::GetEntityFromInsert(DXF::ENTITIES::Insert* pInsert, DXF::BLOCKS::BlockManager* pBlkMgr, DXF::TABLES::TableManager* pTblMgr, double dScale)
{
	if (pInsert == 0) return;

	const wchar_t* strBlockName = pInsert->GetBlockName();
	DXF::BLOCKS::BlockData* pBlock = (DXF::BLOCKS::BlockData*)pBlkMgr->GetBlockData(strBlockName);
	//if (pBlock == 0) return;

	GetEntityFromBlockData(pBlock, pInsert->GetInsertPoint(), strBlockName, pBlkMgr, pTblMgr, dScale);
	
	/*Block^ block = gcnew Block();

	const Utility::Vertex3D& rInsertPoint = pInsert->GetInsertPoint();
	block->OriginVertex = gcnew Vertex2D(rInsertPoint.m_pt[0] * dScale, rInsertPoint.m_pt[1] * dScale);

	block->Name = gcnew System::String(strBlockName);

	m_ctrl->SetCurrentBlock(block);

	void* pID = 0;
	DXF::ENTITIES::Entity* pEntity;

	while (pEntity = pBlock->GetEntity(pID))
	{
		wchar_t* strEntityType = pEntity->GetEntityType();
		Shape^ shape = nullptr;
		Layer^ layer = nullptr;
		
		// 화면에 그리지 않고 좌표 정보만 필요한가?
		//if (!m_bJustCoord)
		{
			layer = GetLayer(gcnew System::String(pEntity->GetOwnLayer()));
			if (layer == nullptr) continue;
		}

		if (!wcscmp(strEntityType, L"ARC"))
		{
			Arc^ arc = GetArcFromDXF((DXF::ENTITIES::Arc*)pEntity, dScale, layer);
			if (arc->ArcAngle < UnE::Geometry::Math::HALF_TOLERANCE())
				continue;

			shape = (Shape^)arc;
		}
		else if (!wcscmp(strEntityType, L"CIRCLE"))
		{
			Arc^ arc = GetCircleFromDXF((DXF::ENTITIES::Circle*)pEntity, dScale, layer);
			if (arc->Radius < UnE::Geometry::Math::HALF_TOLERANCE())
				continue;

			shape = (Shape^)arc;
		}
		else if (!wcscmp(strEntityType, L"ELLIPSE"))
		{
			shape = (Shape^)GetEArcFromDXF((DXF::ENTITIES::Ellipse*)pEntity, dScale, layer);
		}
		else if (!wcscmp(strEntityType, L"LINE"))
		{
			Line^ line = GetLineFromDXF((DXF::ENTITIES::Line*)pEntity, dScale, layer);
			if (line->Begin->GetDistance(line->End) < UnE::Geometry::Math::HALF_TOLERANCE())
				continue;

			shape = (Shape^)line;
		}
		else if (!wcscmp(strEntityType, L"LWPOLYLINE"))
		{
			shape = (Shape^)GetPolyLineFromDXF((DXF::ENTITIES::PolyLine*)pEntity, dScale, layer);
		}
		else if (!wcscmp(strEntityType, L"HATCH"))
		{
			shape = (Shape^)GetHatchFromDXF((DXF::ENTITIES::Hatch*)pEntity, dScale, layer);
		}
		else if (!wcscmp(strEntityType, L"TEXT"))
		{
			shape = (Shape^)GetTextFromDXF((DXF::ENTITIES::Text*)pEntity, pTblMgr, dScale, layer);
		}
		else if (!wcscmp(strEntityType, L"MTEXT"))
		{
			shape = (Shape^)GetMTextFromDXF((DXF::ENTITIES::MText*)pEntity, pTblMgr, dScale, layer);
		}
		else if (!wcscmp(strEntityType, L"INSERT"))
		{
			const wchar_t* strInsertBlockName = ((DXF::ENTITIES::Insert*)pEntity)->GetBlockName();
			const DXF::BLOCKS::BlockData* pBlkData = pBlkMgr->GetBlockData(strInsertBlockName);

			if (pBlkData != 0)
			{
			}
		}

		if (shape != nullptr)
		{
			m_dicShape[(__int64)pEntity] = shape;
		}

		// 화면에 그리지 않고 좌표 정보만 필요한가?
		//if (!m_bJustCoord)
		{
			layer->Add(shape);
		}
	}

	m_ctrl->SetCurrentBlock(nullptr);
	m_ctrl->Blocks->Add(block);*/
}

void DXFLoader::GetEntityFromBlockData(DXF::BLOCKS::BlockData* pBlock, const Utility::Vertex3D& rInsertPoint, const wchar_t* strBlockName, DXF::BLOCKS::BlockManager* pBlkMgr, DXF::TABLES::TableManager* pTblMgr, double dScale)
{
	if (pBlock == 0) return;

	Block^ block = m_ctrl->GetShapeFactory()->CreateBlock(m_ctrl);

	block->OriginVertex = gcnew Vertex2D(rInsertPoint.m_pt[0] * dScale, rInsertPoint.m_pt[1] * dScale);

	block->Name = gcnew System::String(strBlockName);

	m_ctrl->SetCurrentBlock(block);

	void* pID = 0;
	DXF::ENTITIES::Entity* pEntity;

	while (pEntity = pBlock->GetEntity(pID))
	{
		wchar_t* strEntityType = pEntity->GetEntityType();
		Shape^ shape = nullptr;
		Layer^ layer = nullptr;
		
		// 화면에 그리지 않고 좌표 정보만 필요한가?
		//if (!m_bJustCoord)
		{
			layer = GetLayer(gcnew System::String(pEntity->GetOwnLayer()));
			if (layer == nullptr) continue;
		}

		if (!wcscmp(strEntityType, L"ARC"))
		{
			Arc^ arc = GetArcFromDXF((DXF::ENTITIES::Arc*)pEntity, dScale, layer);
			if (arc->ArcAngle < UnE::Geometry::Math::HALF_TOLERANCE())
				continue;

			shape = (Shape^)arc;
		}
		else if (!wcscmp(strEntityType, L"CIRCLE"))
		{
			Arc^ arc = GetCircleFromDXF((DXF::ENTITIES::Circle*)pEntity, dScale, layer);
			if (arc->Radius < UnE::Geometry::Math::HALF_TOLERANCE())
				continue;

			shape = (Shape^)arc;
		}
		else if (!wcscmp(strEntityType, L"ELLIPSE"))
		{
			shape = (Shape^)GetEArcFromDXF((DXF::ENTITIES::Ellipse*)pEntity, dScale, layer);
		}
		else if (!wcscmp(strEntityType, L"LINE"))
		{
			Line^ line = GetLineFromDXF((DXF::ENTITIES::Line*)pEntity, dScale, layer);
			if (line->Begin->GetDistance(line->End) < UnE::Geometry::Math::HALF_TOLERANCE())
				continue;

			shape = (Shape^)line;
		}
		else if (!wcscmp(strEntityType, L"LWPOLYLINE"))
		{
			shape = (Shape^)GetPolyLineFromDXF((DXF::ENTITIES::PolyLine*)pEntity, dScale, layer);
		}
		else if (!wcscmp(strEntityType, L"POLYLINE"))
		{
			shape = (Shape^)GetPolyLineFromDXF((DXF::ENTITIES::PolyLine*)pEntity, dScale, layer);
		}
		else if (!wcscmp(strEntityType, L"HATCH"))
		{
			shape = (Shape^)GetHatchFromDXF((DXF::ENTITIES::Hatch*)pEntity, dScale, layer);
		}
		else if (!wcscmp(strEntityType, L"TEXT"))
		{
			shape = (Shape^)GetTextFromDXF((DXF::ENTITIES::Text*)pEntity, pTblMgr, dScale, layer);
		}
		else if (!wcscmp(strEntityType, L"MTEXT"))
		{
			shape = (Shape^)GetMTextFromDXF((DXF::ENTITIES::MText*)pEntity, pTblMgr, dScale, layer);
		}
		else if (!wcscmp(strEntityType, L"INSERT"))
		{
			DXF::ENTITIES::Insert* pInsert = (DXF::ENTITIES::Insert*)pEntity;

			const wchar_t* strInsertBlockName = pInsert->GetBlockName();
			const DXF::BLOCKS::BlockData* pBlkData = pBlkMgr->GetBlockData(strInsertBlockName);

			GetEntityFromBlockData((DXF::BLOCKS::BlockData*)pBlkData, pInsert->GetInsertPoint(), pInsert->GetBlockName(), pBlkMgr, pTblMgr, dScale);
			return;
		}

		if (shape != nullptr)
		{
			m_dicShape[(__int64)pEntity] = shape;
		}
		else
			continue;

		// 화면에 그리지 않고 좌표 정보만 필요한가?
		//if (!m_bJustCoord)
		{
			shape->ID = pEntity->GetHandle();
			layer->Add(shape);
		}
	}

	m_ctrl->SetCurrentBlock(nullptr);
	m_ctrl->Blocks->Add(block);
}

Shape^ DXFLoader::FindMap(DXF::ENTITIES::Entity* pEntity)
{
	__int64 nKey = (__int64)pEntity;

	if (m_dicShape->ContainsKey(nKey))
		return m_dicShape[nKey];

	return nullptr;
}

void DXFLoader::SetObjectColor(Shape^ obj, Layer^ layer, DXF::ENTITIES::Entity* pEntity, COLORREF* pCol)
{
	int nACI = pEntity->GetColorIndex();

	if (pCol != 0)
	{
		obj->SetColorOption(Shape::ControlType::BYOWN);
		obj->SetOwnColor(System::Drawing::Color::FromArgb(GetRValue(*pCol), GetGValue(*pCol), GetBValue(*pCol)));
	}
	else if (nACI != 256 && nACI > 0)
	{
		int nRed, nGreen, nBlue;
		if (PenWorld::ACI::ACIToRGB(nACI, &nRed, &nGreen, &nBlue))
		{
			System::Drawing::Color color = System::Drawing::Color::FromArgb(nRed, nGreen, nBlue);
			obj->SetColorOption(Shape::ControlType::BYOWN);
			obj->SetOwnColor(color);
		}
	}
	else
	{
		//int nACI = pEntity->GetColorIndex();
		if (nACI == 256)
		{
			obj->SetColorOption(Shape::ControlType::BYLAYER);
		}
		else if (nACI == 0)
		{
			obj->SetColorOption(Shape::ControlType::BYBLOCK);
		}
		/*else if (nACI > 0)
		{
			int nRed, nGreen, nBlue;
			if (PenWorld::ACI::ACIToRGB(nACI,&nRed,&nGreen,&nBlue))
			{
				System::Drawing::Color color = System::Drawing::Color::FromArgb(nRed, nGreen, nBlue);
				obj->SetOwnColor(color);
			}
		}*/
	}
}

// vCenter를 중점으로 가진 Arc의 시작점(vBegin)에서 끝점(vEnd) 까지의 Vertex List를 구하여 arrVertex에 집어넣는다.
// isClockwise : Arc의 진행 방향이 시계방향인가?
void DXFLoader::CreateArcVertex(System::Collections::ArrayList^ arrVertex, Vertex2D^ vBegin, Vertex2D^ vCenter, Vertex2D^ vEnd, int nSliceCount, bool isClockwise)
{
	if (nSliceCount <= 0)
		return;

	Vertex2D^ vRight = gcnew Vertex2D(vCenter->x + 100.0, vCenter->y);

	double dAngle1 = UnE::Geometry::Math::GetAngle(vBegin, vCenter, vRight);
	if (vBegin->y < vCenter->y) dAngle1 = UnE::Geometry::Math::_2PI() - dAngle1;

	double dAngle2 = UnE::Geometry::Math::GetAngle(vEnd, vCenter, vRight);
	if (vEnd->y < vCenter->y) dAngle2 = UnE::Geometry::Math::_2PI() - dAngle2;

	double delta = 0.0;

	if (isClockwise)
	{
		if (dAngle1 > dAngle2)
			delta = (dAngle1 - dAngle2) / -nSliceCount;
		else
			delta = (UnE::Geometry::Math::_2PI() - (dAngle2 - dAngle1)) / -nSliceCount;
	}
	else
	{
		if (dAngle2 > dAngle1)
			delta = (dAngle2 - dAngle1) / nSliceCount;
		else
			delta = (UnE::Geometry::Math::_2PI() - (dAngle1 - dAngle2)) / nSliceCount;
	}

	double dRadius = vCenter->GetDistance(vBegin);

	for (int i=1;i<=nSliceCount;i++)
	{
		Vertex2D^ vertex = gcnew Vertex2D();
		double dAngle = dAngle1 + delta * i;

		vertex->x = vCenter->x + dRadius * System::Math::Cos(dAngle);
		vertex->y = vCenter->y + dRadius * System::Math::Sin(dAngle);

		arrVertex->Add(vertex);
	}
}

END_NS
