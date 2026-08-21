// SampleViewer.cpp : 응용 프로그램에 대한 진입점을 정의합니다.
//

#include "stdafx.h"
#include "SampleViewer.h"
#include "VectorCtrl.h"
#include "Line.h"
#include "Layer.h"
#include "Polyline.h"
#include "Polygon.h"
#include "DBManager.h"
#include "Data/Project.h"
#include "Data/Level.h"
#include "Data/Space.h"
#include "Data/Wall.h"
#include "Data/Window.h"
#include "Data/Door.h"
#include "Data/_POI.h"
#include "Data/Column.h"
#include "Data/AlertArea.h"
#include "Manager.h"
#include "POI.h"
#include "resource.h"
#include <map>

#define MAX_LOADSTRING 100

using namespace VectorGraphics;
using namespace FireSafetyManager;

// 전역 변수:
HINSTANCE hInst;								// 현재 인스턴스입니다.
TCHAR szTitle[MAX_LOADSTRING];					// 제목 표시줄 텍스트입니다.
TCHAR szWindowClass[MAX_LOADSTRING];			// 기본 창 클래스 이름입니다.

// 이 코드 모듈에 들어 있는 함수의 정방향 선언입니다.
ATOM				MyRegisterClass(HINSTANCE hInstance);
BOOL				InitInstance(HINSTANCE, int);
LRESULT CALLBACK	WndProc(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK	About(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK	LayerDlgProc(HWND, UINT, WPARAM, LPARAM);

VectorCtrl* vectorCtrl = 0;

int APIENTRY _tWinMain(_In_ HINSTANCE hInstance,
                     _In_opt_ HINSTANCE hPrevInstance,
                     _In_ LPTSTR    lpCmdLine,
                     _In_ int       nCmdShow)
{
	UNREFERENCED_PARAMETER(hPrevInstance);
	UNREFERENCED_PARAMETER(lpCmdLine);

 	// TODO: 여기에 코드를 입력합니다.
	MSG msg;
	HACCEL hAccelTable;

	// 전역 문자열을 초기화합니다.
	LoadString(hInstance, IDS_APP_TITLE, szTitle, MAX_LOADSTRING);
	LoadString(hInstance, IDC_SAMPLEVIEWER, szWindowClass, MAX_LOADSTRING);
	MyRegisterClass(hInstance);

	// 응용 프로그램 초기화를 수행합니다.
	if (!InitInstance (hInstance, nCmdShow))
	{
		return FALSE;
	}

	hAccelTable = LoadAccelerators(hInstance, MAKEINTRESOURCE(IDC_SAMPLEVIEWER));

	// 기본 메시지 루프입니다.
	while (GetMessage(&msg, NULL, 0, 0))
	{
		if (!TranslateAccelerator(msg.hwnd, hAccelTable, &msg))
		{
			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}
	}

	return (int) msg.wParam;
}



//
//  함수: MyRegisterClass()
//
//  목적: 창 클래스를 등록합니다.
//
ATOM MyRegisterClass(HINSTANCE hInstance)
{
	WNDCLASSEX wcex;

	wcex.cbSize = sizeof(WNDCLASSEX);

	wcex.style			= CS_HREDRAW | CS_VREDRAW;
	wcex.lpfnWndProc	= WndProc;
	wcex.cbClsExtra		= 0;
	wcex.cbWndExtra		= 0;
	wcex.hInstance		= hInstance;
	wcex.hIcon			= LoadIcon(hInstance, MAKEINTRESOURCE(IDI_SAMPLEVIEWER));
	wcex.hCursor		= LoadCursor(NULL, IDC_ARROW);
	wcex.hbrBackground	= (HBRUSH)(COLOR_WINDOW+1);
	wcex.lpszMenuName	= MAKEINTRESOURCE(IDC_SAMPLEVIEWER);
	wcex.lpszClassName	= szWindowClass;
	wcex.hIconSm		= LoadIcon(wcex.hInstance, MAKEINTRESOURCE(IDI_SMALL));

	return RegisterClassEx(&wcex);
}

Layer* selectedLayer = 0;
Layer* oldLayer = 0;
Shape* selectedShape = 0;
int StatusBarHeight = 50;

Layer* AddLayer(std::wstring strName, COLORREF col)
{
	Layer* layer = new Layer(strName);
	layer->SetColor(col);
	vectorCtrl->AddLayer(layer);

	return layer;
}

Layer* GetLayer(std::wstring strLayerName)
{
	int nLayerCount = vectorCtrl->GetLayerCount();

	for (int i = 0; i < nLayerCount; i++)
	{
		Layer* layer = vectorCtrl->GetLayer(i);

		if (layer != 0 && layer->GetLayerName() == strLayerName)
			return layer;
	}

	return 0;
}

const wchar_t* WallCenterLineLayerName = L"WallCenterLine";
const wchar_t* WallBoundaryLayerName = L"WallBoundary";
const wchar_t* SpaceLayerName = L"Space";
const wchar_t* DoorLayerName = L"Door";
const wchar_t* WindowLayerName = L"Window";
const wchar_t* POILayerName = L"POI";
const wchar_t* ColumnLayerName = L"Column";
const wchar_t* AlertAreaLayerName = L"AlertArea";

void MakeLayers()
{
	AddLayer(WallBoundaryLayerName, RGB(255, 255, 0));
	AddLayer(WallCenterLineLayerName, RGB(255, 0, 0));
	AddLayer(SpaceLayerName, RGB(0, 255, 0));
	AddLayer(DoorLayerName, RGB(0, 0, 255));
	AddLayer(WindowLayerName, RGB(0, 0, 255));
	AddLayer(POILayerName, RGB(255, 0, 0));
	AddLayer(ColumnLayerName, RGB(255, 0, 0));
	AddLayer(AlertAreaLayerName, RGB(255, 0, 255));
}

void SetBoundary(const Vertex2D& rVertex, Vertex2D*& vTL, Vertex2D*& vBR)
{
	if (vTL == 0)
	{
		vTL = new Vertex2D(rVertex);
		vBR = new Vertex2D(rVertex);
	}
	else
	{
		if (vTL->x > rVertex.x)
			vTL->x = rVertex.x;
		if (vTL->y < rVertex.y)
			vTL->y = rVertex.y;

		if (vBR->x < rVertex.x)
			vBR->x = rVertex.x;
		if (vBR->y > rVertex.y)
			vBR->y = rVertex.y;
	}
}

void AddDoor(Wall* pWall, int nDoorCount)
{
	Layer* doorLayer = GetLayer(DoorLayerName);

	if (doorLayer != 0)
	{
		for (int i = 0; i < nDoorCount; i++)
		{
			Door* pDoor = pWall->GetDoor(i);

			if (pDoor != 0)
			{
				std::vector<VertexList*> edges;
				int nEdgeCount = pDoor->CalcBoundary(edges);

				for (int j = 0; j < nEdgeCount; j++)
				{
					VertexList* vertices = edges[j];
					VectorGraphics::Polyline* polyline = new VectorGraphics::Polyline();

					for (std::list<Vertex2D>::iterator iter = vertices->Vertices.begin(); iter != vertices->Vertices.end(); iter++)
					{
						polyline->AddVertex(*iter);
					}

					doorLayer->AddShape(polyline);
					delete vertices;
				}
			}
		}
	}
}

void AddWindow(Wall* pWall, int nWindowCount)
{
	Layer* windowLayer = GetLayer(WindowLayerName);

	if (windowLayer != 0)
	{
		for (int i = 0; i < nWindowCount; i++)
		{
			Window* pWindow = pWall->GetWindow(i);

			if (pWindow != 0)
			{
				std::vector<VertexList*> edges;
				int nEdgeCount = pWindow->CalcBoundary(edges);

				for (int j = 0; j < nEdgeCount; j++)
				{
					VertexList* vertices = edges[j];
					VectorGraphics::Polyline* polyline = new VectorGraphics::Polyline();

					for (std::list<Vertex2D>::iterator iter = vertices->Vertices.begin(); iter != vertices->Vertices.end(); iter++)
					{
						polyline->AddVertex(*iter);
					}

					polyline->SetClosed(true);
					windowLayer->AddShape(polyline);
					delete vertices;
				}
			}
		}
	}
}

void AddWall(Level* pLevel, int nWallCount, Vertex2D*& vTL, Vertex2D*& vBR)
{
	Layer* wallCenterLayer = GetLayer(WallCenterLineLayerName);
	Layer* wallBoundaryLayer = GetLayer(WallBoundaryLayerName);

	if (wallCenterLayer != 0 && wallBoundaryLayer != 0)
	{
		for (int i = 0; i < nWallCount; i++)
		{
			Wall* pWall = pLevel->GetWall(i);

			if (pWall != 0)
			{
				int nVertexCount = pWall->GetBoundaryVertexCount();

				if (nVertexCount > 0)
				{
					Line* line = new Line(pWall->GetBegin(), pWall->GetEnd());
					wallCenterLayer->AddShape(line);

					VectorGraphics::Polygon* polygon = new VectorGraphics::Polygon();

					for (int j = 0; j < nVertexCount; j++)
					{
						Vertex2D* pVertex = pWall->GetBoundaryVertex(j);

						if (pVertex != 0)
						{
							polygon->AddVertex(*pVertex);

							if (nVertexCount > 2)
								SetBoundary(*pVertex, vTL, vBR);
						}
					}

					polygon->Done();
					polygon->SetDrawingMode(VectorGraphics::Polygon::DrawMode::Boundary);
					wallBoundaryLayer->AddShape(polygon);

					int nDoorCount = pWall->GetDoorCount();
					int nWindowCount = pWall->GetWindowCount();

					if (nDoorCount > 0)
						AddDoor(pWall, nDoorCount);

					if (nWindowCount > 0)
						AddWindow(pWall, nWindowCount);
				}
				else
				{
					Line* line = new Line(pWall->GetBegin(), pWall->GetEnd());
					wallCenterLayer->AddShape(line);
				}
			}
		}
	}
}

// 버텍스 중복 체크
bool CheckVertex(std::vector<Vertex2D*>& vertices, Vertex2D& rVertex)
{
	int nVertexCount = (int)vertices.size();

	for (int i = 1; i < nVertexCount; i++)
	{
		Vertex2D* begin = vertices[i - 1];
		Vertex2D* end = vertices[i];

		double d1 = begin->GetDistance(rVertex);
		double d2 = end->GetDistance(rVertex);
		double d3 = begin->GetDistance(*end);

		if (d1 + d2 <= d3 + 0.001)
			return false;
	}

	return true;
}

void AddSpace(Level* pLevel, int nSpaceCount, Vertex2D*& vTL, Vertex2D*& vBR)
{
	Layer* spaceLayer = GetLayer(SpaceLayerName);

	if (spaceLayer != 0)
	{
		for (int i = 0; i < nSpaceCount; i++)
		{
			Space* pSpace = pLevel->GetSpace(i);

			if (pSpace != 0)
			{
				VectorGraphics::Polygon* polygon = new VectorGraphics::Polygon();
				int nVertexCount = pSpace->GetBoundaryVertexCount();

				std::vector<Vertex2D*> vertices;
				
				for (int j = 0; j < nVertexCount; j++)
				{
					Vertex2D* pVertex = pSpace->GetBoundaryVertex(j);

					if (pVertex != 0)
					{
						if (CheckVertex(vertices, *pVertex) == false)
							continue;

						vertices.push_back(pVertex);
					}
				}
				
				int nVertexCount2 = (int)vertices.size();

				for (int j = 0; j < nVertexCount2; j++)
				{
					polygon->AddVertex(*vertices[j]);
					SetBoundary(*vertices[j], vTL, vBR);
				}

				/*for (int j = 0; j < nVertexCount; j++)
				{
					Vertex2D* pVertex = pSpace->GetBoundaryVertex(j);

					if (pVertex != 0)
					{
						polygon->AddVertex(*pVertex);
						SetBoundary(*pVertex, vTL, vBR);
					}
				}*/

				polygon->Done();
				polygon->SetDrawingMode(VectorGraphics::Polygon::DrawMode::Boundary);
				spaceLayer->AddShape(polygon);
			}
		}
	}
}

void AddPOI(Level* pLevel)
{
	Layer* poiLayer = GetLayer(POILayerName);

	if (poiLayer != 0)
	{
		int nPOICount = pLevel->GetPOICount();

		for (int i = 0; i < nPOICount; i++)
		{
			FireSafetyManager::POI* _poi = pLevel->GetPOI(i);

			if (_poi != 0)
			{
				POIType* poiType = _poi->GetPOIType();
				VectorGraphics::POI* poi = 0;

				if (poiType != 0)
				{
					POIIcon* poiIcon = poiType->GetIcon();

					if (poiIcon != 0)
					{
						poi = new VectorGraphics::POI();
						poi->SetIcon(poiIcon);
					}
				}

				if (poi == 0)
				{
					poi = new VectorGraphics::POI();
					poi->SetIcon(POIType::GetDefaultIcon());
				}

				poi->SetPosition(_poi->GetPosition());
				poiLayer->AddShape(poi);
			}
		}
	}
}

void AddColumn(Level* pLevel)
{
	Layer* columnLayer = GetLayer(ColumnLayerName);

	if (columnLayer != 0)
	{
		int nColumnCount = pLevel->GetColumnCount();

		for (int i = 0; i < nColumnCount; i++)
		{
			FireSafetyManager::Column* pColumn = pLevel->GetColumn(i);

			if (pColumn != 0)
			{
				VectorGraphics::Polygon* polygon = new VectorGraphics::Polygon();
				Column::ColumnType columnType = pColumn->GetColumnType();

				Vertex2D *vBoundaryTL = 0, *vBoundaryBR = 0;
				std::vector<Vertex2D> vertices;

				if (columnType == Column::ColumnType::RectType)
				{
					Vertex2D vTL, vBL, vBR;
					pColumn->GetRect(vTL, vBL, vBR);

					Vertex2D vTR = vBR - vBL + vTL;

					vertices.push_back(vTL);
					vertices.push_back(vBL);
					vertices.push_back(vBR);
					vertices.push_back(vTR);
				}
				else if (columnType == Column::ColumnType::CircleType)
				{
					Vertex2D vCenter;
					double dRadius;
					pColumn->GetCircle(vCenter, dRadius);

					int nSlice = 100;
					double dSlice = 3.141592653 * 2 / nSlice;

					for (int i = 0; i < nSlice; i++)
					{
						double dAngle = i * dSlice;
						double x = vCenter.x + dRadius * cos(dAngle);
						double y = vCenter.y + dRadius * sin(dAngle);

						vertices.push_back(Vertex2D(x, y));
					}
				}
				else
					continue;

				int nVertexCount = (int)vertices.size();

				for (int j = 0; j < nVertexCount; j++)
				{
					polygon->AddVertex(vertices[j]);
					SetBoundary(vertices[j], vBoundaryTL, vBoundaryBR);
				}

				polygon->Done();
				polygon->SetDrawingMode(VectorGraphics::Polygon::DrawMode::Boundary);
				columnLayer->AddShape(polygon);
			}
		}
	}
}

void AddAlertArea(Level* pLevel)
{
	Layer* alertAreaLayer = GetLayer(AlertAreaLayerName);

	if (alertAreaLayer != 0)
	{
		int nAreaCount = pLevel->GetAlertAreaCount();
		Vertex2D *vBoundaryTL = 0, *vBoundaryBR = 0;

		for (int i = 0; i < nAreaCount; i++)
		{
			FireSafetyManager::AlertArea* pArea = pLevel->GetAlertArea(i);

			if (pArea != 0)
			{
				VectorGraphics::Polygon* polygon = new VectorGraphics::Polygon();
				int nVertexCount = pArea->GetBoundaryVertexCount();
				
				for (int j = 0; j < nVertexCount; j++)
				{
					Vertex2D* pVertex = pArea->GetBoundaryVertex(j);

					if (pVertex != 0)
					{
						polygon->AddVertex(*pVertex);
						SetBoundary(*pVertex, vBoundaryTL, vBoundaryBR);
					}
				}

				polygon->Done();
				polygon->SetDrawingMode(VectorGraphics::Polygon::DrawMode::Fill);
				alertAreaLayer->AddShape(polygon);
			}
		}
	}
}

void AddLevel(Level* pLevel)
{
	SpaceMaker::Manager mgr;

	int nSpaceCount = pLevel->GetSpaceCount();
	int nWallCount = pLevel->GetWallCount();

	for (int i = 0; i < nSpaceCount; i++)
	{
		Space* pSpace = pLevel->GetSpace(i);

		if (pSpace != 0)
			mgr.AddSpace(pSpace);
	}
	
	for (int i = 0; i < nWallCount; i++)
	{
		Wall* pWall = pLevel->GetWall(i);

		if (pWall != 0)
			mgr.AddWall(pWall);
	}

	//if (mgr.Calc())
	{
		Vertex2D* vTL = 0;
		Vertex2D* vBR = 0;

		AddWall(pLevel, nWallCount, vTL, vBR);
		AddSpace(pLevel, nSpaceCount, vTL, vBR);
		AddPOI(pLevel);
		AddColumn(pLevel);
		AddAlertArea(pLevel);

		if (vTL != 0 && vBR != 0)
		{
			int nWidth, nHeight;
			vectorCtrl->GetScreenSize(nWidth, nHeight);

			Vertex2D _vTL, _vBR;
			vectorCtrl->ScreenToGlobal(0, 0, &_vTL);
			vectorCtrl->ScreenToGlobal(nWidth, nHeight, &_vBR);

			double dObjectWidth = vBR->x - vTL->x;
			double dObjectHeight = vTL->y - vBR->y;
			double dScreenWidth = _vBR.x - _vTL.x;
			double dScreenHeight = _vTL.y - _vBR.y;

			double dViewportWeight = 1.0;

			if (dObjectWidth / dObjectHeight < dScreenWidth / dScreenHeight)
			{
				dViewportWeight = dObjectHeight / 0.85 / dScreenHeight;
			}
			else
			{
				dViewportWeight = dObjectWidth / 0.85 / dScreenWidth;
			}

			vectorCtrl->SetViewportWeight(dViewportWeight);

			Vertex2D vCenter = (*vTL + *vBR) / 2;
			vectorCtrl->SetViewportCenter(vCenter.x, vCenter.y);

			delete vTL;
			delete vBR;
		}
	}
}

void AddProject(Project* project)
{
	int nLevelCount = project->GetLevelCount();
	int nLevelIndex = 0;

	if (nLevelCount > 0)
	{
		// Sample로 하나의 Level만 사용
		Level* pLevel = project->GetLevel(nLevelIndex);

		if (pLevel != 0)
			AddLevel(pLevel);
	}

	for (int i = 0; i < nLevelCount; i++)
	{
		if (i == nLevelIndex)
			continue;

		// 예제에서는 사용하지 않으니 메모리 해제한다.
		Level* pLevel = project->GetLevel(i);
		delete pLevel;
	}
}

Project* currentProject = 0;

void MakeGeometry(DBManager& dbMgr)
{
	vectorCtrl->Clear();

	MakeLayers();

	std::map<int, POIType*>& mapPOITypes = dbMgr.GetPOITypes();
	int nProjectCount = dbMgr.GetProjectCount();

	if (nProjectCount > 0)
	{
		// Sample로 하나의 Project만 사용
		Project* project = dbMgr.GetProject(0);
		currentProject = project;

		if (project != 0)
			AddProject(project);
	}
}

//
//   함수: InitInstance(HINSTANCE, int)
//
//   목적: 인스턴스 핸들을 저장하고 주 창을 만듭니다.
//
//   설명:
//
//        이 함수를 통해 인스턴스 핸들을 전역 변수에 저장하고
//        주 프로그램 창을 만든 다음 표시합니다.
//
BOOL InitInstance(HINSTANCE hInstance, int nCmdShow)
{
   HWND hWnd;

   hInst = hInstance; // 인스턴스 핸들을 전역 변수에 저장합니다.

   hWnd = CreateWindow(szWindowClass, szTitle, WS_OVERLAPPEDWINDOW,
      CW_USEDEFAULT, 0, CW_USEDEFAULT, 0, NULL, NULL, hInstance, NULL);

   if (!hWnd)
   {
      return FALSE;
   }

   RECT rect;
   GetClientRect(hWnd, &rect);

   vectorCtrl = new VectorCtrl();
   vectorCtrl->OnCreate(rect.right - rect.left, rect.bottom - rect.top - StatusBarHeight);

   DBManager dbMgr;
   
   if (dbMgr.LoadDB("../../../sample.db", "../../../POI"))
   {
	   MakeGeometry(dbMgr);
   }

   HWND hDlg = CreateDialog(hInst, MAKEINTRESOURCE(IDD_DIALOG_Layer), hWnd, LayerDlgProc);
   ShowWindow(hDlg, SW_SHOW);

   selectedLayer = new Layer(L"선택 레이어");
   selectedLayer->SetColor(RGB(0, 255, 0));
   vectorCtrl->AddLayer(selectedLayer);

   ShowWindow(hWnd, nCmdShow);
   UpdateWindow(hWnd);

   return TRUE;
}

void DrawMouseVertex(HWND hWnd, int x, int y)
{
	RECT rect;
	HDC hdc = ::GetDC(hWnd);
	GetClientRect(hWnd, &rect);

	Vertex2D vMouse;
	vectorCtrl->ScreenToGlobal(x, y, &vMouse);

	_locale_t locale = _get_current_locale();

	wchar_t str[256];
	_swprintf_l(str, 256, L"좌표 : %.1lf, %.1lf", locale, vMouse.x, vMouse.y);
	TextOut(hdc, 10, rect.bottom - 34, str, wcslen(str));

	if (currentProject != 0)
	{
		AnchorNode* pAnchorNode = currentProject->GetAnchorNode();

		if (pAnchorNode != 0)
		{
			Vertex2D vGlobal = pAnchorNode->LocalToGlobal(vMouse.x, vMouse.y);
			_swprintf_l(str, 256, L"Global 좌표 : %.1lf, %.1lf", locale, vGlobal.x, vGlobal.y);
			TextOut(hdc, 300, rect.bottom - 34, str, wcslen(str));
		}
	}

	InvalidateRect(hWnd, &rect, FALSE);
	ReleaseDC(hWnd, hdc);
}

bool IsPolygonType(Shape* pShape)
{
	std::string strClassName = typeid(*pShape).name();

	if (strClassName == "class VectorGraphics::Polygon")
		return true;

	return false;
}

//
//  함수: WndProc(HWND, UINT, WPARAM, LPARAM)
//
//  목적:  주 창의 메시지를 처리합니다.
//
//  WM_COMMAND	- 응용 프로그램 메뉴를 처리합니다.
//  WM_PAINT	- 주 창을 그립니다.
//  WM_DESTROY	- 종료 메시지를 게시하고 반환합니다.
//
//
LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	int wmId, wmEvent;
	PAINTSTRUCT ps;
	HDC hdc;

	switch (message)
	{
	case WM_COMMAND:
		wmId    = LOWORD(wParam);
		wmEvent = HIWORD(wParam);
		// 메뉴 선택을 구문 분석합니다.
		switch (wmId)
		{
		case IDM_ABOUT:
			DialogBox(hInst, MAKEINTRESOURCE(IDD_ABOUTBOX), hWnd, About);
			break;
		case IDM_EXIT:
			DestroyWindow(hWnd);
			break;
		default:
			return DefWindowProc(hWnd, message, wParam, lParam);
		}
		break;
	case WM_PAINT:
	{
		hdc = BeginPaint(hWnd, &ps);
		// TODO: 여기에 그리기 코드를 추가합니다.
		HDC hdcWindow = GetDC(hWnd);
		vectorCtrl->OnDraw(hdcWindow);

		ReleaseDC(hWnd, hdcWindow);
		EndPaint(hWnd, &ps);
	}
		break;
	case WM_SIZE:
	{
		RECT rect;

		if (GetClientRect(hWnd, &rect))
		{
			vectorCtrl->OnSize(rect.right - rect.left, rect.bottom - rect.top - StatusBarHeight);
			InvalidateRect(hWnd, NULL, TRUE);
		}
	}
		break;
	case WM_LBUTTONDOWN:
	{
		Vertex2D vertex;
		vectorCtrl->ScreenToGlobal(LOWORD(lParam), HIWORD(lParam), &vertex);

		// POI를 먼저 HitTest 한다.
		Shape* pShape = vectorCtrl->HitTestPOI(vertex);

		// POI가 선택되지 않았으면 다른 객체들을 HitTest 한다.
		if (pShape == 0)
			pShape = vectorCtrl->HitTestExceptPOI(vertex);

		if (pShape == 0)
		{
			if (selectedShape != 0)
			{
				selectedLayer->RemoveShape(selectedShape);
				oldLayer->AddShape(selectedShape);

				if (IsPolygonType(selectedShape))
				{
					VectorGraphics::Polygon* polygon = (VectorGraphics::Polygon*)selectedShape;
					polygon->SetDrawingMode(VectorGraphics::Polygon::DrawMode::Boundary);
				}

				oldLayer = 0;
				selectedShape = 0;
				InvalidateRect(hWnd, NULL, FALSE);
			}
		}
		else
		{
			if (selectedShape != 0 && selectedShape != pShape)
			{
				if (IsPolygonType(selectedShape))
				{
					VectorGraphics::Polygon* polygon = (VectorGraphics::Polygon*)selectedShape;
					polygon->SetDrawingMode(VectorGraphics::Polygon::DrawMode::Boundary);
				}

				selectedLayer->RemoveShape(selectedShape);
				oldLayer->AddShape(selectedShape);
			}

			if (selectedShape != pShape)
			{
				selectedShape = pShape;
				oldLayer = selectedShape->GetLayer();
				oldLayer->RemoveShape(selectedShape);
				selectedLayer->AddShape(selectedShape);

				if (IsPolygonType(selectedShape))
				{
					VectorGraphics::Polygon* polygon = (VectorGraphics::Polygon*)selectedShape;
					polygon->SetDrawingMode(VectorGraphics::Polygon::DrawMode::Fill);
				}

				InvalidateRect(hWnd, NULL, FALSE);
			}
		}
	}
		break;
	case WM_MBUTTONDOWN:
		vectorCtrl->MouseDown(LOWORD(lParam), HIWORD(lParam), VectorCtrl::MouseType::MBUTTON);
		return 0;
	case WM_MBUTTONUP:
		vectorCtrl->MouseUp(LOWORD(lParam), HIWORD(lParam), VectorCtrl::MouseType::MBUTTON);
		return 0;
	case WM_MOUSEMOVE:
	{
		int x = LOWORD(lParam);
		int y = HIWORD(lParam);
		vectorCtrl->MouseMove(x, y);
		DrawMouseVertex(hWnd, x, y);
	}
		return 0;
	case WM_MOUSEWHEEL:
	{
		short zDelta = GET_WHEEL_DELTA_WPARAM(wParam);

		POINT pt;
		pt.x = LOWORD(lParam);
		pt.y = HIWORD(lParam);

		ScreenToClient(hWnd, &pt);

		double dWeight = vectorCtrl->GetViewportWeight();

		if (zDelta < 0)
		{
			dWeight += dWeight * 0.1;
		}
		else
		{
			dWeight -= dWeight * 0.1;
		}

		vectorCtrl->Zoom(pt.x, pt.y, dWeight);
		DrawMouseVertex(hWnd, pt.x, pt.y);
	}
		return 0;
	case WM_DESTROY:
		PostQuitMessage(0);
		break;
	default:
		return DefWindowProc(hWnd, message, wParam, lParam);
	}
	return 0;
}

// 정보 대화 상자의 메시지 처리기입니다.
INT_PTR CALLBACK About(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
	UNREFERENCED_PARAMETER(lParam);
	switch (message)
	{
	case WM_INITDIALOG:
		return (INT_PTR)TRUE;

	case WM_COMMAND:
		if (LOWORD(wParam) == IDOK || LOWORD(wParam) == IDCANCEL)
		{
			EndDialog(hDlg, LOWORD(wParam));
			return (INT_PTR)TRUE;
		}
		break;
	}
	return (INT_PTR)FALSE;
}

std::map<HWND, Layer*> mapLayers;

void SetChecked(HWND hDlg, int nID, bool isChecked, std::wstring strLayerName)
{
	HWND hWnd = GetDlgItem(hDlg, nID);

	if (isChecked)
		SendMessage(hWnd, BM_SETCHECK, BST_CHECKED, 0);
	else
		SendMessage(hWnd, BM_SETCHECK, BST_UNCHECKED, 0);

	Layer* pLayer = GetLayer(strLayerName);

	if (hWnd != 0 && pLayer != 0)
		mapLayers[hWnd] = pLayer;
}

void AllChecked(HWND hDlg)
{
	SetChecked(hDlg, IDC_CHECK_WallCenterLine, true, L"WallCenterLine");
	SetChecked(hDlg, IDC_CHECK_WallBoundary, true, L"WallBoundary");
	SetChecked(hDlg, IDC_CHECK_Space, true, L"Space");
	SetChecked(hDlg, IDC_CHECK_Door, true, L"Door");
	SetChecked(hDlg, IDC_CHECK_Window, true, L"Window");
	SetChecked(hDlg, IDC_CHECK_POI, true, L"POI");
	SetChecked(hDlg, IDC_CHECK_COLUMN, true, L"Column");
	SetChecked(hDlg, IDC_CHECK_ALERT_AREA, true, L"AlertArea");
}

Layer* GetCheckBoxLayer(HWND hWnd)
{
	std::map<HWND, Layer*>::iterator iter = mapLayers.find(hWnd);

	if (iter != mapLayers.end())
		return iter->second;

	return 0;
}

void ToggleLayer(Layer* pLayer)
{
	if (pLayer->GetVisible())
		pLayer->SetVisible(false);
	else
		pLayer->SetVisible(true);
}

// 레이어 대화 상자의 메시지 처리기
INT_PTR CALLBACK LayerDlgProc(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
	UNREFERENCED_PARAMETER(lParam);
	switch (message)
	{
	case WM_INITDIALOG:
		AllChecked(hDlg);
		return (INT_PTR)TRUE;

	case WM_COMMAND:
	{
		Layer* pLayer = GetCheckBoxLayer((HWND)lParam);

		if (pLayer != 0)
		{
			ToggleLayer(pLayer);
			HWND hWnd = GetParent(hDlg);

			if (hWnd)
				InvalidateRect(hWnd, NULL, FALSE);
		}
	}
		break;
	}
	return (INT_PTR)FALSE;
}