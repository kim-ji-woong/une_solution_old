// 기본 DLL 파일입니다.

#include "stdafx.h"

#include <map>
#include <utility>

#include <atlcoll.h>
using namespace ATL;

#include "BaseView.h"
#include "Layer.h"

#include "UMathAPI.h"
#include "UDB.h"
#include "UBaseView.h"
#include "UEntity.h"
#include "UAnimation.h"
#include "UBaseView.h"
#include "UBaseDriver.h"
#include "UBaseModel.h"
#include "UMouseOperator.h"
#include "UScene.h"

#include "Poco/AutoPtr.h"
#include "Poco/Zip/Decompress.h"
#include "Poco/Zip/ZipLocalFileHeader.h"
#include "Poco/Zip/ZipArchive.h"
#include "Poco/Path.h"
#include "Poco/File.h"
#include "Poco/Delegate.h"



using namespace UnE::Core;

using namespace System;
using namespace System::Collections;


HKEY key; 
int hMainWnd;

//////////////////////////////////////////////////////////////////////////
// Local function

wchar_t* ToWcharArray(System::String^ str)
{
	if (str == nullptr)
		return 0;

	int nLen = str->Length;
	wchar_t* wstr = new wchar_t[nLen + 1];

	array<wchar_t>^ arr = str->ToCharArray();

	for (int i=0;i<nLen;i++)
		wstr[i] = arr[i];
	wstr[nLen] = 0;

	return wstr;
}

System::String^ ToSystemString(wchar_t* str)
{
	if (str == 0)
		return nullptr;

	System::String^ _str = gcnew System::String(L"");

	for (int i=0;str[i] != 0;i++)
	{
		_str += str[i];
	}

	return _str;
}

int WideToMulti(char* pszDst, const wchar_t* pwzIn, UINT uCodepage)
{
	int nReqLen = (int)WideCharToMultiByte(uCodepage, 0, pwzIn, (int)wcslen(pwzIn), NULL, 0, NULL, NULL);
	int nLen    = (int)WideCharToMultiByte(uCodepage, 0, pwzIn, (int)wcslen(pwzIn), pszDst, nReqLen, NULL, NULL); 
	if(nLen)
		pszDst[nLen] = 0;
	return nLen;
} 


std::wstring MultiToWide( const char* str, int len, UINT CodaPage )
{
	int bufSize = MultiByteToWideChar(CodaPage, 0, str, len, NULL, 0);
	int strSize = bufSize;
	if( len == -1 )
		strSize = bufSize-1;
	if( strSize <= 0 )
		return std::wstring();
	std::vector<wchar_t> wsv(bufSize);
	if( MultiByteToWideChar(CodaPage, 0, str,len, &wsv[0], bufSize) == 0 )
		return std::wstring();
	return std::wstring(wsv.begin(), wsv.begin()+strSize);
}



//////////////////////////////////////////////////////////////////////////
// BaseEngine
void Core::Engine::Init(System::String^ szWorkPath, System::String^ szAppName)
{
	
	if( szAppName == nullptr)
	{
		szAppName = gcnew System::String("CoreApp");
	}
	char buf[4096];
	
	wchar_t * t = ToWcharArray(szAppName);
	
	std::wstring szOrName = std::wstring(t);
	

	std::wstring keyName = L"Software\\UnE\\" + szOrName;
		
	
	wchar_t * t2 = ToWcharArray(szWorkPath);


	WideToMulti(buf, t2, CP_ACP);
	std::string szWorkDir = std::string(buf);
	WideToMulti(buf, t, CP_ACP);
	std::string mAppName = std::string(buf);
	

	hInstance = System::Diagnostics::Process::GetCurrentProcess()->Handle.ToInt32();
	LONG nResult = RegOpenKeyEx(HKEY_CURRENT_USER, keyName.c_str(), 0, KEY_ALL_ACCESS  ,&key);
	if( nResult == ERROR_FILE_NOT_FOUND )
	{
		RegCreateKeyEx(HKEY_CURRENT_USER,  keyName.c_str(), 0, 0, 0, KEY_ALL_ACCESS, NULL, &key, 0);
	}
	HINSTANCE hTemp = (HINSTANCE)hInstance;
	UBaseDriver::Instance().SetClient(hTemp);	
	UBaseDriver::Instance().SetRegistry(key);
	UBaseDriver::Instance().SetDisplayMode(1280, 1024, 32);
	UBaseDriver::Instance().ChangeRenderer(eRS_OPENGL);
	//UBaseDriver::Instance().ChangeRenderer(eRS_DIRECT9);
	UBaseDriver::Instance().InitDriver(szWorkDir, mAppName);

	hMainWnd = 0;

	delete[] t;
	delete[] t2;
}

void Core::Engine::EngineDispose()
{
	
	UDB::RemoveAllOperator(true);
	UDB::RemoveAllBaseView(true);
	UBaseDriver::Instance().DisposeDriver();

	hMainWnd = 0;
}

//////////////////////////////////////////////////////////////////////////
// BaseView
Core::BaseView::BaseView(  )
{
	InitializeComponent();
	bCheckPoistion = false;
	mMode = 0;
	ResizeRedraw = false;
	DoubleBuffered = true;
	m_nViewMode = 4;
	bOrbit = false;
	bPan = false;
	mPopup = nullptr;
	mTarget = this;
	this->Text = "3DView";
	mCompType = Core::Component::None;

	m_bComponentMode = false;
	m_LayerManager = nullptr;
	//mTarget->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
	//mTarget->DoubleBuffered = true;
	this->SetStyle( System::Windows::Forms::ControlStyles::DoubleBuffer |
					System::Windows::Forms::ControlStyles::UserPaint |
					System::Windows::Forms::ControlStyles::Opaque, true);

	m_Brush = gcnew System::Drawing::SolidBrush(System::Drawing::Color::White);

	//mTarget->Paint += gcnew System::Windows::Forms::PaintEventHandler(this, &BaseView::OnPaint);
	//mTarget->SizeChanged += gcnew System::EventHandler(this, &BaseView::OnSize);
	
	
	mTarget->MouseDown += gcnew System::Windows::Forms::MouseEventHandler(this, &BaseView::OnMouseDown);
	mTarget->MouseUp += gcnew System::Windows::Forms::MouseEventHandler(this, &BaseView::OnMouseUp);
	mTarget->MouseMove += gcnew System::Windows::Forms::MouseEventHandler(this, &BaseView::OnMouseMove);
	mTarget->MouseClick += gcnew System::Windows::Forms::MouseEventHandler(this, &BaseView::OnMouseClick);		

	m_bEnableGradient = true;
}



void Core::BaseView::OnPaintBackground( System::Windows::Forms::PaintEventArgs^ pevent )
{
	UpdateWindow();
}


void Core::BaseView::OnPaint( System::Object^ sender, System::Windows::Forms::PaintEventArgs^ e )
{
	//UpdateWindow();
}


void Core::BaseView::OnPaint( System::Windows::Forms::PaintEventArgs^ e )
{
	UpdateWindow();
}

void Core::BaseView::OnSize( System::Object^ sender, System::EventArgs^ e )
{
	int width =  mTarget->Size.Width;
	int height = mTarget->Size.Height;

	if( width > 0 && height > 0)
	{
		UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
		if( pView != NULL)
		{
			pView->ChangeDisplaySize(width,  height);
		}
		//UpdateWindow();
	}
}

void Core::BaseView::AddBeams(float tx, float ty)
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		char buf[512];
		sprintf(buf, "%d#mesh_30#92", mHWND);
		pView->CloneSceneNode(std::string(buf), tx, 0.0f, ty);

		sprintf(buf, "%d#mesh_31#95", mHWND);
		pView->CloneSceneNode(std::string(buf), tx, 0.0f, ty);
	}
}

void Core::BaseView::AddCore(float tx, float ty)
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		char buf[512];
		sprintf(buf, "%d#mesh_45#137", mHWND);
		pView->CloneSceneNode(std::string(buf), tx, 0.0f, ty);
		
		sprintf(buf, "%d#mesh_59#179", mHWND);
		pView->CloneSceneNode(std::string(buf), tx, 0.0f, ty);

		sprintf(buf, "%d#mesh_46#140", mHWND);
		pView->CloneSceneNode(std::string(buf), tx, 0.0f, ty);

		sprintf(buf, "%d#mesh_47#143", mHWND);
		pView->CloneSceneNode(std::string(buf),  tx, 0.0f, ty);

	}
}


void Core::BaseView::OnMouseDown( System::Object^ sender, System::Windows::Forms::MouseEventArgs^ e )
{
	if( this->Parent != nullptr)
	{
		this->Parent->Select();
		this->Parent->Focus();
	}

	MouseOperator* pOp = (MouseOperator*)UDB::GetOperator(mHWND, UOpType::eOp_Mouse);
	if( pOp == NULL)
		return;
	CPoint pos(e->X, e->Y);
	if (e->Button == System::Windows::Forms::MouseButtons::Left)
	{
		pOp->OnLButtonDown(MK_LBUTTON, pos);
		bOrbit = true;
		bPan = false;
	}
	else if (e->Button == System::Windows::Forms::MouseButtons::Right)
	{
		pOp->OnRButtonDown(MK_RBUTTON, pos);
		bOrbit = false;
		bPan = false;
	}
	else if (e->Button == System::Windows::Forms::MouseButtons::Middle)
	{
		pOp->OnMButtonDown(MK_MBUTTON, pos);
		bOrbit = false;
		bPan = true;
	}

}

void Core::BaseView::OnMouseUp( System::Object^ sender, System::Windows::Forms::MouseEventArgs^ e )
{
	MouseOperator* pOp = (MouseOperator*)UDB::GetOperator(mHWND, UOpType::eOp_Mouse);
	if( pOp == NULL)
		return;

	bOrbit = false;
	bPan = false;
	CPoint pos(e->X, e->Y);
	if (e->Button == System::Windows::Forms::MouseButtons::Left)
	{
		pOp->OnLButtonUp(MK_LBUTTON, pos);
	}
	else if (e->Button == System::Windows::Forms::MouseButtons::Right)
	{
		pOp->SavePoint(MK_RBUTTON, pos);
	}
	else if (e->Button == System::Windows::Forms::MouseButtons::Middle)
	{
		pOp->OnMButtonUp(MK_MBUTTON, pos);
	}
}

void Core::BaseView::OnMouseMove( System::Object^ sender, System::Windows::Forms::MouseEventArgs^ e )
{
	MouseOperator* pOp = (MouseOperator*)UDB::GetOperator(mHWND, UOpType::eOp_Mouse);
	if( pOp == NULL)
		return;
	CPoint pos(e->X, e->Y);

	if (bOrbit == true)
	{
		pOp->OnMouseMove(MK_LBUTTON, pos);
	}
	else if (bPan == true)
	{
		pOp->OnMouseMove(MK_MBUTTON, pos);
	}

	UpdateWindow();
}

void Core::BaseView::OnMouseClick( System::Object^ sender, System::Windows::Forms::MouseEventArgs^ e )
{
	MouseOperator* pOp = (MouseOperator*)UDB::GetOperator(mHWND, UOpType::eOp_Mouse);
	if( pOp == NULL)
		return;
	if( bCheckPoistion == false)
		return;

	if (e->Button == System::Windows::Forms::MouseButtons::Right)
	{
		if( mPopup != nullptr)
		{
			CPoint pos(e->X, e->Y);
			pOp->SavePoint(MK_RBUTTON, pos);
			System::Drawing::Point^ pt = gcnew System::Drawing::Point(e->X, e->Y);
			System::Drawing::Point^ scPt = mTarget->PointToScreen(*pt);
			mPopup->Show(scPt->X, scPt->Y);
		}		
	}
	if (e->Button == System::Windows::Forms::MouseButtons::Left)
	{
		if (m_bComponentMode == true)
		{
			AddComponent(e->X, e->Y, mCompType);
		}
	}
}

void Core::BaseView::OnMouseWheel( long x, long y, int delta )
{
	CPoint pos(x,y);
	MouseOperator* pOp = (MouseOperator*)UDB::GetOperator(mHWND, UOpType::eOp_Mouse);
	if( pOp == NULL)
		return;

	pOp->OnMouseWheel(MK_MBUTTON, delta, pos);
	UpdateWindow();
}

void Core::BaseView::RemovePOI()
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		pView->RemovePOI();
	}
}



Core::Position3D^ Core::BaseView::AddPOI(System::String^ szPath)
{	
	MouseOperator* pOp = (MouseOperator*)UDB::GetOperator(mHWND, UOpType::eOp_Mouse);
	if( pOp == NULL)
	{
		return gcnew Position3D(1000000000, -1000000000, 1000000000);
	}

	CPoint pos = pOp->GetSavedPoint();

	char buf[4096];
	wchar_t * t = ToWcharArray(szPath);
	WideToMulti(buf, t, CP_ACP);
	std::string szTextPath = std::string(buf);
	delete[] t;	

	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		int nID = pView->AddIconPOI(szTextPath);
	}
	UnE::Math::Vector3 vPos;
	pOp->Get3DPosition(pos, vPos);

	Position3D^ pt = gcnew Position3D(vPos.x, vPos.y, vPos.z);
	return pt;

}

void Core::BaseView::RemovePOI( float x, float y, float z )
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		pView->RemovePOI(x, y, z);
	}
}

void Core::BaseView::RemovePOI( int nID )
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		pView->RemovePOI(nID);

	}
	if( m_LayerManager != nullptr)
	{
		m_LayerManager->RemoveLayerChild(nID);
	}
}


void Core::BaseView::RemoveTextPOI(int nID)
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		pView->RemoveTextPOI(nID);

	}
	if( m_LayerManager != nullptr)
	{
		m_LayerManager->RemoveLayerChild(nID);
	}
}

int Core::BaseView::AddPOI( System::String^ szPath, float x, float y, float z )
{

	char buf[4096];
	wchar_t * t = ToWcharArray(szPath);
	WideToMulti(buf, t, CP_ACP);
	std::string szTextPath = std::string(buf);
	delete[] t;	

	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		int nID = pView->AddIconPOI(szTextPath, x, y, z);
		return nID;
	}
	return -1;
}

int Core::BaseView::UpdateIcon( int nID, System::String^ szNewPath )
{

	char buf[4096];
	wchar_t * t = ToWcharArray(szNewPath);
	WideToMulti(buf, t, CP_ACP);
	std::string szTextPath = std::string(buf);
	delete[] t;	

	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		pView->UpdateIcon(nID, szTextPath);
		return nID;
	}
	return -1;
}


void Core::BaseView::OpenMesh(System::String^ strPath)
{
	char buf[4096];
	wchar_t * t = ToWcharArray(strPath);
	WideToMulti(buf, t, CP_ACP);
	std::string szName = std::string(buf);
	delete[] t;

	UnE::Core::UBaseModel * pDoc = UDB::GetBaseModel(mHWND);
	if (pDoc != NULL)
	{		
		pDoc->ReadDAE(szName);		
	}
}

void Core::BaseView::OpenMesh( System::String^ strPath , bool bDAE )
{
	char buf[4096];
	wchar_t * t = ToWcharArray(strPath);
	WideToMulti(buf, t, CP_ACP);
	std::string szName = std::string(buf);
	delete[] t;	

	UnE::Core::UBaseModel * pDoc = UDB::GetBaseModel(mHWND);
	if( pDoc != NULL)
	{
		if (bDAE == true)
			pDoc->ReadDAE(szName);
		else
			pDoc->ReadScene(szName);
	}
}

System::String^ Core::BaseView::OnSelectNode()
{
	MouseOperator* pOp = (MouseOperator*)UDB::GetOperator(mHWND, UOpType::eOp_Mouse);
	if( pOp == NULL)
		return ToSystemString(L"");

	CPoint pt = pOp->GetSavedPoint();
	UINT   nFlag = pOp->GetSavedFlags();

	pOp->OnSelectNode(nFlag, pt);

	UpdateWindow();

	USceneNode * pObj = pOp->SelectNode();
	if( pObj != NULL)
	{
		std::string szName = pObj->GetAliasName();
		USES_CONVERSION;
		return ToSystemString(A2W(szName.c_str()));
		
	}
	return ToSystemString(L"");
}

System::String^ Core::BaseView::OnPickName()
{
	MouseOperator* pOp = (MouseOperator*)UDB::GetOperator(mHWND, UOpType::eOp_Mouse);
	if( pOp == NULL)
		return ToSystemString(L"");

	CPoint pt = pOp->GetSavedPoint();
	UINT   nFlag = pOp->GetSavedFlags();

	pOp->OnSelect(nFlag, pt);
	UEntity * pObj = pOp->SelectedObject();	

	if( pObj != NULL)
	{
		std::string szName = pObj->GetName();

		UnE::Core::UObjectManager* pObjMgr = UDB::GetUDB()->GetObjectManger((HWND)(mHWND));
		UObject * pUObj = pObjMgr->GetUObject(szName);
		if( pUObj != NULL)
		{
			pOp->ClearSelect();
			std::string szAlias = pUObj->GetAlias();
			USES_CONVERSION;
			return ToSystemString(A2W(szAlias.c_str()));
		}
	}
	pOp->ClearSelect();
	return ToSystemString(L"");
}

System::String^ Core::BaseView::OnSelect()
{
	MouseOperator* pOp = (MouseOperator*)UDB::GetOperator(mHWND, UOpType::eOp_Mouse);
	if( pOp == NULL)
		return ToSystemString(L"");

	CPoint pt = pOp->GetSavedPoint();
	UINT   nFlag = pOp->GetSavedFlags();

	pOp->OnSelect(nFlag, pt);
	
	UpdateWindow();

	UEntity * pObj = pOp->SelectedObject();
	if( pObj != NULL)
	{
		std::string szName = pObj->GetName();

		UnE::Core::UObjectManager* pObjMgr = UDB::GetUDB()->GetObjectManger((HWND)(mHWND));
		UObject * pUObj = pObjMgr->GetUObject(szName);
		if( pUObj != NULL)
		{
			std::string szAlias = pUObj->GetAlias();
			USES_CONVERSION;
			return ToSystemString(A2W(szAlias.c_str()));
		}
	}
	return ToSystemString(L"");
}

void Core::BaseView::ClearSelect()
{
	MouseOperator* pOp = (MouseOperator*)UDB::GetOperator(mHWND, UOpType::eOp_Mouse);
	if( pOp == NULL)
		return;

	pOp->ClearSelect();
	UpdateWindow();
}

void Core::BaseView::OnViewFront(bool bUpdateWindow)
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		pView->SetFrontView();
		if (bUpdateWindow == true)
			UpdateWindow();
	}
}

void Core::BaseView::OnViewFront()
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if (pView != NULL)
	{
		pView->SetFrontView();
		UpdateWindow();
	}
}

void Core::BaseView::OnViewTop(bool bUpdateWindow)
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		pView->SetTopView();
		if (bUpdateWindow == true)
			UpdateWindow();
	}
}
void Core::BaseView::OnViewTop()
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if (pView != NULL)
	{
		pView->SetTopView();
		UpdateWindow();
	}
}


void Core::BaseView::OnViewLeft(bool bUpdateWindow)
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		pView->SetLeftView();
		if (bUpdateWindow == true)
			UpdateWindow();
	}
}
void Core::BaseView::OnViewLeft( )
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if (pView != NULL)
	{
		pView->SetLeftView();
		UpdateWindow();
	}
}

void Core::BaseView::OnViewRight(bool bUpdateWindow)
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		pView->SetRightView();
		if (bUpdateWindow == true)
			UpdateWindow();
	}
}

void Core::BaseView::OnViewRight( )
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if (pView != NULL)
	{
		pView->SetRightView();
		UpdateWindow();
	}
}

void Core::BaseView::SetCameraOrientaion(Core::Quaternion3D^ orient)
{
	if( orient == nullptr)
		return;

	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		pView->SetCameraOrientation(UnE::Math::Quaternion(orient->X, orient->Y, orient->Z, orient->W));
	}
}

Core::Quaternion3D^ Core::BaseView::GetCameraOrientaion()
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		UnE::Math::Quaternion vec = pView->GetCameraOrientaion();
		return gcnew Quaternion3D(vec.x, vec.y, vec.z, vec.w);
	}
	return nullptr;
}

Core::Position3D^ Core::BaseView::GetCameraDirection()
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		UnE::Math::Vector3 vec = pView->GetCameraDirection();
		return gcnew Position3D(vec.x, vec.y, vec.z);
	}
	return nullptr;
}

void Core::BaseView::SetCameraDirection(Position3D^ dir)
{
	if( dir == nullptr)
		return;

	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		pView->SetCameraDirection(UnE::Math::Vector3(dir->X, dir->Y, dir->Z));		
	}
}

void Core::BaseView::SetCameraPosition(Position3D^ pos)
{
	if( pos == nullptr)
		return;

	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		pView->SetCameraPosition(UnE::Math::Vector3(pos->X, pos->Y, pos->Z));		
	}
}

Core::Position3D^ Core::BaseView::GetCameraPosition()
{

	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		
		UnE::Math::Vector3 vec = pView->GetCameraPosition();
		return gcnew Position3D(vec.x, vec.y, vec.z);
	}
	return nullptr;
}

void Core::BaseView::OnViewFix()
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		pView->SetFrontView();
		pView->SetFixView();
		UpdateWindow();
	}
}

void Core::BaseView::OnViewFit(bool bUpdateWindow)
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		pView->SetFitView();
		if (bUpdateWindow == true)
		UpdateWindow();
	}
}
void Core::BaseView::OnViewFit( )
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if (pView != NULL)
	{
		pView->SetFitView();
		UpdateWindow();
	}
}


void Core::BaseView::OnViewHome(bool bUpdateWindow)
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		if( bMain == true)
			pView->SetHomeView(0.1f);
		else
			pView->SetFitView();

		if (bUpdateWindow == true)
		UpdateWindow();
	}
}
void Core::BaseView::OnViewHome( )
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if (pView != NULL)
	{
		if (bMain == true)
			pView->SetHomeView(0.1f);
		else
			pView->SetFitView();

		UpdateWindow();
	}
}


void Core::BaseView::OnViewRear(bool bUpdateWindow)
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		pView->SetRearView();
		UpdateWindow();
	}	
}

void Core::BaseView::OnViewRear()
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if (pView != NULL)
	{
		pView->SetRearView();
	}
}


void Core::BaseView::UpdateWindow()
{
	if( bMain == true)
	{
		UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
		if( pView != NULL)
		{
			pView->RenderScene();
		}
	}
	else
	{
		UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
		if( pView != NULL)
		{
			pView->RenderOneFrame();
		}
	}
}

bool Core::BaseView::InitBaseView()
{
	if( UBaseDriver::Instance().IsInitDriver() == false)
		return false;

	mHWND = mTarget->Handle.ToInt32();
	if( hMainWnd == 0)
	{
		bMain = true;
		hMainWnd = mHWND;
		CRect rect;
		CWnd * pWnd = CWnd::FromHandle((HWND)mHWND);
		pWnd->GetClientRect(&rect);			

		UnE::Core::MouseOperator * pOp = new UnE::Core::MouseOperator();
		UBaseView * pView = new UBaseView((HWND)mHWND);

		
		pView->EnableGraient(m_bEnableGradient);
		
		if( m_ColorBackBottom != nullptr)
		{
			int r = m_ColorBackBottom->R;
			float rf = (float)r / 255.0f;

			int g = m_ColorBackBottom->G;
			float gf = (float)g / 255.0f;

			int b = m_ColorBackBottom->B;
			float bf = b / 255.0f;

			pView->SetBackBottomColor(rf, gf, bf);
		}

		if( m_ColorBackUpper != nullptr)
		{
			byte r = m_ColorBackUpper->R;
			float rf = r / 255.0f;

			byte g = m_ColorBackUpper->G;
			float gf = g / 255.0f;

			byte b = m_ColorBackUpper->B;
			float bf = b / 255.0f;

			pView->SetBackUpperColor(rf, gf, bf);
		}

		pView->AddOperator(pOp);	
		UBaseDriver::Instance().SetDisplayMode(rect.Width()+1, rect.Height()+1, 32);
		if( pView->CreateRenderWindow(rect.Width()+1, rect.Height()+1, "main", "maincam") == true)
		{		
			pOp->SetHWnd((HWND)mHWND);
			int temp = mHWND;
			UDB::AddBaseView(pView);
			UDB::AddOperator(pOp);
			pView->LoadDefultResource();
		}	
		else
		{
			delete pView;
			delete pOp;
			return false;
		}
	}
	else
	{
		bMain = false;
		CRect rect;
		CWnd * pWnd = CWnd::FromHandle((HWND)mHWND);
		pWnd->GetClientRect(&rect);	
		UnE::Core::MouseOperator * pOp = new UnE::Core::MouseOperator();
		UBaseView * pView = new UBaseView((HWND)mHWND);
		pView->AddOperator(pOp);	
		UBaseDriver::Instance().SetDisplayMode(rect.Width()+1, rect.Height()+1, 32);
		if( pView->CreateSubWindow((HWND)hMainWnd, rect.Width()+1, rect.Height()+1, "subcam") == true)
		{		
			pOp->SetHWnd((HWND)mHWND);
			int temp = mHWND;
			UDB::AddBaseView(pView);
			UDB::AddOperator(pOp);
		}
		else
		{
			delete pView;
			delete pOp;
			return false;
		}
	}

	return true;
}

void Core::BaseView::SetMode( int nMode )
{
	mMode = nMode;
}

int Core::BaseView::AddAliasName( System::String^ orName, System::String^ alias )
{
	char buf[2048];

	//USES_CONVERSION;
	wchar_t * t = ToWcharArray(orName);
	WideToMulti(buf, t, CP_ACP);

	std::string szOrName = std::string(buf);
	delete[] t;

	wchar_t * t2 = ToWcharArray(alias);
	WideToMulti(buf, t2, CP_ACP);
	std::string szAlias = std::string(buf);
	delete[] t2;

	
	UnE::Core::UObjectManager* pObjMgr = UDB::GetUDB()->GetObjectManger((HWND)(mHWND));
	UObject * pObj = pObjMgr->GetUObjectByAlias(szOrName);
	if( pObj != NULL)
	{
		UBaseView * pView = UDB::GetBaseView(mHWND);
		if( pView != NULL)
		{
			int nID = pView->ShowObjectName(pObj, szAlias);
			return nID;
		}		
	}
	return -1;
}


void Core::BaseView::ClearAllData()
{
	UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		pView->ClearViewData();
	}
}

int Core::BaseView::AddZoneName( System::String^ groupName, float x, float y, float z )
{
	char buf[2048];
	wchar_t * t = ToWcharArray(groupName);
	WideToMulti(buf, t, CP_ACP);
	std::string szGroupName = std::string(buf);
	delete[] t;

	UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		int nID = pView->AddTextPOI(szGroupName, x, y, z);
		return nID;
	}
	return -1;
}

int Core::BaseView::AddGroupName( System::String^ groupName, float x, float y, float z )
{
	char buf[2048];
	wchar_t * t = ToWcharArray(groupName);
	WideToMulti(buf, t, CP_ACP);
	std::string szGroupName = std::string(buf);
	delete[] t;

	UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		int nID = pView->ShowZoneName(x, y, z, szGroupName);
		return nID;
	}
	return -1;
}

void Core::BaseView::ChangeViewSize( int width, int height )
{
	if( width > 0 && height > 0)
	{
		UBaseView * pView = UDB::GetBaseView(mHWND);
		if( pView != NULL)
		{
			pView->ChangeDisplaySize(width,  height);
		}
	}
}

void Core::BaseView::SetCheckPoistion( bool bCheck )
{
	bCheckPoistion = bCheck;
	if( bCheckPoistion == false)
	{
		if( mPopup != nullptr)
		{
			if( mPopup->Visible == true)
			{
				mPopup->Visible = false;
			}
		}
	}
}

Core::Position3D^ Core::BaseView::OnPosition()
{
	MouseOperator* pOp = (MouseOperator*)UDB::GetOperator(mHWND, UOpType::eOp_Mouse);
	if( pOp == NULL)
	{
		return gcnew Position3D(1000000000, -1000000000, 1000000000);
	}

	CPoint pos = pOp->GetSavedPoint();	
	UnE::Math::Vector3 vPos;
	pOp->Get3DPosition(pos, vPos);

	Position3D^ pt = gcnew Position3D(vPos.x, vPos.y, vPos.z);
	return pt;
}

std::vector<UnE::Math::Vector3> gPointList;
void Core::BaseView::AddPointToLine( float x, float y, float z )
{
	//UnE::Math::Vector3 vPos(x, y, z);
	//gPointList.push_back(vPos);
}

void Core::BaseView::UpdateLine()
{
	//UBaseView::Instance().DrawTempLine((HWND)(mHWND), gPointList);
}

void Core::BaseView::Refresh()
{
	RedrawScene();
}

void Core::BaseView::OnResize( System::EventArgs^ e )
{
	int width =  mTarget->Size.Width;
	int height = mTarget->Size.Height;

	if( width > 0 && height > 0)
	{
		UBaseView * pView = UDB::GetBaseView(mHWND);
		if( pView != NULL)
		{
			pView->ChangeDisplaySize(width,  height);
		}
	}
}

void Core::BaseView::RedrawScene()
{
	UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		pView->RenderScene();
	}
}

//void Core::BaseView::AddFire( int id, float x, float y, float z,System::String^ szName )
//{
//	wchar_t *t = ToWcharArray(szName);
//	USES_CONVERSION;
//	std::string szTextPath = W2A(t);
//	delete []t;
//
//	UBaseView * pView = UDB::GetBaseView(mHWND);
//	if( pView != NULL)
//	{
//		pView->AddFireExtinguisher(id, x, y, z, szTextPath);
//	}
//}

void Core::BaseView::OnViewOctree(bool m_bShowOctree)
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		pView->ShowOctree(m_bShowOctree);
	}
}

void Core::BaseView::OnViewWireframe()
{
	m_nViewMode = 1;
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		pView->ChangeViewMode(UnE::Core::UPolygonMode::ePM_WIREFRAME);	
	}
}

void Core::BaseView::OnViewHiddenline()
{
	m_nViewMode = 2;
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		pView->ChangeViewMode(UnE::Core::UPolygonMode::ePM_HIDDENLINE);
	}
}

void Core::BaseView::OnViewPolygon()
{
	m_nViewMode = 3;
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		pView->ChangeViewMode(UnE::Core::UPolygonMode::ePM_SHADED);
	}
}

void Core::BaseView::OnViewTextured()
{
	m_nViewMode = 4;
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		pView->ChangeViewMode(UnE::Core::UPolygonMode::ePM_TEXTURED);
	}
}

void Core::BaseView::ZoomObject( System::String^ szName )
{
	if( szName == nullptr || szName->Equals(""))
		return;

	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		char buf[4096];			
		wchar_t * t = ToWcharArray(szName);
		WideToMulti(buf, t, CP_ACP);
		std::string szAcpName = std::string(buf);
		delete [] t;
		pView->GetMouseOperator()->SetZoomObject(szAcpName);
	}
}

void Core::BaseView::ZoomObjectAnimation(System::String^ szName)
{
	if (szName == nullptr || szName->Equals(""))
		return;

	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if (pView != NULL)
	{
		char buf[4096];
		wchar_t * t = ToWcharArray(szName);
		WideToMulti(buf, t, CP_ACP);
		std::string szAcpName = std::string(buf);
		delete[] t;
		UnE::Math::Vector3 vPos;
		if(pView->GetMouseOperator()->GetObjectPoint(szAcpName, vPos))
			ZoomTargetAnimation(gcnew Position3D(vPos.x, vPos.y, vPos.z), 30.0f);
	}
}

void Core::BaseView::MouseMoveTo( int x, int y )
{
	MouseOperator* pOp = (MouseOperator*)UDB::GetOperator(mHWND, UOpType::eOp_Mouse);
	if( pOp == NULL)
		return;
	CPoint pos(x, y);

	if (bOrbit == true)
	{
		pOp->OnMouseMove(MK_LBUTTON, pos);
	}
	else if (bPan == true)
	{
		pOp->OnMouseMove(MK_MBUTTON, pos);
	}

	UpdateWindow();
}

void Core::BaseView::PerformLayout( System::Windows::Forms::Control^ affectedControl, System::String^ affectedProperty )
{
	UpdateWindow();
}

System::Drawing::Point^ Core::BaseView::Get2DPoint( Position3D^ pos )
{
	System::Drawing::Point^ result = gcnew System::Drawing::Point();
	MouseOperator* pOp = (MouseOperator*)UDB::GetOperator(mHWND, UOpType::eOp_Mouse);
	if( pOp == NULL)
		return result;

	UnE::Math::Vector3 vPos(pos->X, pos->Y, pos->Z);		
	CPoint res;
	pOp->Get2DPosition(vPos, res );

	result->X = res.x;
	result->Y = res.y;
	return result;
}

Core::Position3D^ Core::BaseView::Get3DPoint( System::Drawing::Point^ pt )
{
	MouseOperator* pOp = (MouseOperator*)UDB::GetOperator(mHWND, UOpType::eOp_Mouse);
	if( pOp == NULL)
	{
		return gcnew Position3D(1000000000, -1000000000, 1000000000);
	}
	CPoint pos(pt->X, pt->Y);
	UnE::Math::Vector3 vPos;
	pOp->Get3DPosition(pos, vPos);
	Position3D^ ptRes = gcnew Position3D(vPos.x, vPos.y, vPos.z);
	return ptRes;
}

int Core::BaseView::OnSelectPOI( int x, int y )
{
	MouseOperator* pOp = (MouseOperator*)UDB::GetOperator(mHWND, UOpType::eOp_Mouse);
	if( pOp == NULL)
	{
		return -1;
	}
	int nResult = pOp->OnSelectPOI(x, y);
	return nResult;
}

void Core::BaseView::ShowIconPOI( int nID, bool bShow )
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		pView->ShowIconPOI(nID, bShow);
	}
}

void Core::BaseView::ShowNames( int nID, bool bShow )
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		pView->ShowTextPOI(nID, bShow);
	}
}

bool Core::BaseView::SaveScreen( System::String^ path )
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
		char buf[4096];			
		wchar_t * t = ToWcharArray(path);
		WideToMulti(buf, t, CP_ACP);
		std::string szOrName = std::string(buf);

		delete []t;

		return pView->SaveScreenShot(szOrName);
	}
	return false;
}

void Core::BaseView::SelectPOI( int nIcon, bool bShow )
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{		
		pView->SelectIconPOI(nIcon, bShow);
	}
}

void Core::BaseView::ClearAllSelectedPOI()
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{		
		pView->ClearSelectedPIO();
	}
}

void Core::BaseView::SetPickSize( int nIcon, int width, int height )
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{		
		pView->SetPickSize(nIcon, width, height);
	}
}

bool Core::BaseView::IsPOISelected( int nIcon )
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{		
		return pView->IsIconPOISelected(nIcon);		
	}
	return false;
}

bool Core::BaseView::MovePOI( int nID, float x, float y, float z )
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{		
		return pView->MoveIconPOI(nID, x, y, z);		
	}
	return false;
}

//void DoAnimation(Core::BaseView^ view, Core::Position3D^ pos, float dist)
//{
//	/*Ogre::Vector3 vPosMove = (vPos - vStartPos) / 20;
//	Ogre::Vector3 vDirMove = (vDir - vStartDir) / 20;*/
//	Core::Position3D^ vStartPos = view->GetCameraPosition();
//	Core::Position3D^ vStartDir = view->GetCameraDirection();
//	Core::Position3D vPosMove((pos->X - vStartPos->X) / 20.0f, (pos->Y - vStartPos->Y) / 20.0f, (pos->Z - vStartPos->Z) / 20.0f);
//
//	UnE::Math::Vector3 vNewPos(vStartPos->X, vStartPos->Y, vStartPos->Z);
//	//Position3D^ vNewDir = vStartDir;
//
//	UnE::Core::UBaseView * pView = UDB::GetBaseView(view->WindowHandle);
//
//	for (int i = 0; i < 20; ++i)
//	{
//		vNewPos.x = vNewPos.x + vPosMove.X;
//		vNewPos.y = vNewPos.y + vPosMove.Y;
//		vNewPos.z = vNewPos.z + vPosMove.Z;
//		pView->GetMouseOperator()->TargetZoom(vNewPos, dist);
//		view->RedrawScene();
//		Sleep(30);
//	}
//	
//}

void Core::BaseView::ZoomTarget(Position3D^ pos, float dist)
{
	if (pos == nullptr)
		return;

	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{
	UnE::Math::Vector3 vec(pos->X, pos->Y, pos->Z);
	pView->GetMouseOperator()->TargetZoom(vec, dist);
	}
}

void Core::BaseView::ZoomTargetAnimation( Position3D^ pos, float dist )
{
	if( pos == nullptr)
		return;

	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if (pView != NULL)
	{
		Core::Position3D^ vStartPos = GetCameraPosition();
		Core::Position3D^ vStartDir = GetCameraDirection();
		Core::Position3D vPosMove((pos->X - vStartPos->X) / 20.0f, (pos->Y - vStartPos->Y) / 20.0f, (pos->Z - vStartPos->Z) / 20.0f);

		UnE::Math::Vector3 vNewPos(vStartPos->X, vStartPos->Y, vStartPos->Z);
		//Position3D^ vNewDir = vStartDir;

		UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);

		for (int i = 0; i < 20; ++i)
		{
			vNewPos.x = vNewPos.x + vPosMove.X;
			vNewPos.y = vNewPos.y + vPosMove.Y;
			vNewPos.z = vNewPos.z + vPosMove.Z;
			pView->GetMouseOperator()->TargetZoom(vNewPos, dist);
			
			RedrawScene();
			Sleep(30);
		}
	}
}

void Core::BaseView::EnablePOI( int nID, bool bEnable )
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{		
		//pView->EnablePOI(nID, bEnable);		
	}
}

void Core::BaseView::OnSavePt( System::Windows::Forms::MouseEventArgs^ e )
{
	MouseOperator* pOp = (MouseOperator*)UDB::GetOperator(mHWND, UOpType::eOp_Mouse);
	if( pOp == NULL)
		return;

	CPoint pos(e->X, e->Y);
	if (e->Button == System::Windows::Forms::MouseButtons::Left)
	{
		pOp->SavePoint(MK_LBUTTON, pos);
	}
	else if (e->Button == System::Windows::Forms::MouseButtons::Right)
	{
		pOp->SavePoint(MK_RBUTTON, pos);
	}
	else if (e->Button == System::Windows::Forms::MouseButtons::Middle)
	{
		pOp->SavePoint(MK_MBUTTON, pos);
	}
}

void Core::BaseView::SetIconPOISize( float width, float height )
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{		
		pView->SetIconPOISize(width, height);	
	}
}

void Core::BaseView::SetTextColor( float red, float green, float blue )
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{		
		pView->SetTextPOIColor(red, green, blue);
	}
}

void Core::BaseView::SetTextHeight(float fFontHeight)
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if (pView != NULL)
	{
		pView->SetFontHeight(fFontHeight);
	}
}

void Core::BaseView::SetTextLODDist(float fDistance)
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if (pView != NULL)
	{
		pView->SetTextLODDist(fDistance);
	}
}
void Core::BaseView::SetTextLOD(bool bLOD)
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if (pView != NULL)
	{
		pView->SetTextLOD(bLOD);
	}
}

bool Core::BaseView::IsInCamera( float x, float y, float z )
{
	MouseOperator* pOp = (MouseOperator*)UDB::GetOperator(mHWND, UOpType::eOp_Mouse);
	if( pOp == NULL)
		return false;

	BOOL bResult = pOp->CheckInFrustum(x, y, z);
	
	return (bResult == TRUE);
}

float Core::BaseView::GetPOIDistance( int nPoi )
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{		
		float fDistnce = pView->GetPOIDistance(nPoi);
		return fDistnce;
	}
	return -1.0f;
}

void Core::BaseView::CreateSceneNodes()
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if( pView != NULL)
	{		
		pView->CreateSceneNode("CarCube", 1,1,1, 0.0f, 0.0f, 0.0f, false);
		pView->CreateScenePane("ManCube", 2, 0.0f, 0.0f, 0.0f, false);

		pView->CreateSceneNode("MovingEquip", 3,8, 6, 105.0f, -2.0f, -69.5f, false);
	}
}

void Core::BaseView::CreateCompass(float fAzumith)
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if (pView != NULL)
	{
		pView->CreateCompass(fAzumith);
	}
}

void Core::BaseView::AddComponent(int x, int y, int compType)
{

	MouseOperator* pOp = (MouseOperator*)UDB::GetOperator(mHWND, UOpType::eOp_Mouse);
	if (pOp == NULL)
	{
		return;
	}

	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if (pView != NULL)
	{
		CPoint pos = CPoint(x, y);
		UnE::Math::Vector3 vPos;
		pOp->Get3DPosition(pos, vPos);

		pView->CreateTree();
	}
}

void Core::BaseView::ShowShelterPath(int nType)
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if (pView != NULL)
	{
		pView->ShowPath(nType);
	}
}

void Core::BaseView::HideAllShelter()
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if (pView != NULL)
	{
		pView->HideAllPath();
	}
}

void Core::BaseView::SetTextPOILOD(int nID, bool bToogle, float dist)
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if (pView != NULL)
	{
		pView->SetTextPOILOD(nID, bToogle, dist);
	}
}

int Core::BaseView::CheckScenePosition(System::String^ szName, int type, float value)
{
	if (szName == nullptr)
	{
		return -999;
	}

	char buf[4096];
	wchar_t * t = ToWcharArray(szName);
	WideToMulti(buf, t, CP_ACP);
	std::string mSceneName = std::string(buf);

	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if (pView != NULL)
	{
		return pView->CheckScenePosition(mSceneName, type, value);
	}
	return -999;
}

void Core::BaseView::CreateFloor(float fwidth, float fheight, float felevation, System::Drawing::Color^ tcolor, System::Drawing::Color^ bcolor, bool bEnableGradient )
{

	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if (pView != NULL)
	{
		pView->SetFloorEnableGradient(bEnableGradient);
		pView->SetFloorTopColor(tcolor->R / 255.0f, tcolor->G / 255.0f, tcolor->B / 255.0f);
		pView->SetFloorBottomColor(bcolor->R / 255.0f, bcolor->G / 255.0f, bcolor->B / 255.0f);
		pView->CreateFloor(fwidth, fheight, felevation);
	}
}

//void Core::BaseView::SetBackgroudGradient( bool bEnabled )
//{
//	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
//	if( pView != NULL)
//	{		
//		pView->EnableGraient(bEnabled);
//	}
//
//}

//void Core::BaseView::SetBackgroundUpperColor( float red, float green, float blue )
//{
//	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
//	if( pView != NULL)
//	{		
//		pView->SetBackUpperColor(red, green, blue);
//	}
//}

//void Core::BaseView::SetBackgroundBottomColor( float red, float green, float blue )
//{
//	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
//	if( pView != NULL)
//	{		
//		pView->SetBackBottomColor(red, green, blue);
//	}
//}

void Core::BaseView::EarthquakeMotion(bool earthquake)
{
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if (pView != NULL)
	{		
		pView->SetTempMaterial(earthquake);
	}
}



void Core::BaseView::CameraMovingAnimation(Position3D^ targetCampos, Position3D^ targetCamDir, Quaternion3D^ targetCamQuart)
{
	//  실제 서버 구현에는 포함되지 않고 시연에만 사용되는것으로 동의함(kjw), 동작중 센서신호를 받지 못함(skkim 2017-10-18)
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if (pView != NULL)
	{
		float fValue = 20.0f;
		//pView->SetTempMaterial(earthquake);
		Core::Position3D^ vStartPos = GetCameraPosition();
		Core::Position3D^ vStartDir = GetCameraDirection();

		Core::Position3D^ vPassPos = gcnew Core::Position3D(targetCampos->X, targetCampos->Y, targetCampos->Z);
		Core::Position3D^ vPassDir = gcnew Core::Position3D(targetCamDir->X, targetCamDir->Y, targetCamDir->Z);

		Core::Position3D vPosMove((vPassPos->X - vStartPos->X) / fValue, (vPassPos->Y - vStartPos->Y) / fValue, (vPassPos->Z - vStartPos->Z) / fValue);
		Core::Position3D^ vNewPos = gcnew Core::Position3D(vStartPos->X, vStartPos->Y, vStartPos->Z);

		Core::Position3D vDirMove((vPassDir->X - vStartDir->X) / fValue, (vPassDir->Y - vStartDir->Y) / fValue, (vPassDir->Z - vStartDir->Z) / fValue);
		Core::Position3D^ vNewDir = gcnew Core::Position3D(vStartDir->X, vStartDir->Y, vStartDir->Z);

		for (int i = 0; i < (int)fValue; ++i)
		{
			vNewPos->X = vNewPos->X + vPosMove.X;
			vNewPos->Y = vNewPos->Y + vPosMove.Y;
			vNewPos->Z = vNewPos->Z + vPosMove.Z;

			vNewDir->X = vNewDir->X + vDirMove.X;
			vNewDir->Y = vNewDir->Y + vDirMove.Y;
			vNewDir->Z = vNewDir->Z + vDirMove.Z;

			SetCameraPosition(vNewPos);
			SetCameraDirection(vNewDir);

			RedrawScene();
			Sleep(30);
		}
	}
}


#ifdef SAFE_KOREA_YH_2017

void Core::BaseView::CameraMovingAnimationViewAll(Position3D^ targetCampos, Position3D^ targetCamDir, Quaternion3D^ targetCamQuart)
{
	//  실제 서버 구현에는 포함되지 않고 시연에만 사용되는것으로 동의함(kjw), 동작중 센서신호를 받지 못함(skkim 2017-10-18)
	UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);
	if (pView != NULL)
	{
		float fValue = 20.0f;
		//pView->SetTempMaterial(earthquake);
		Core::Position3D^ vStartPos = GetCameraPosition();
		Core::Position3D^ vStartDir = GetCameraDirection();
		
		Core::Position3D^ vPassPos = gcnew Core::Position3D(901.0919f, 852.5491f, 798.0175f);
		Core::Position3D^ vPassDir = gcnew Core::Position3D((float)1.279113E-05, -0.6637159f, -0.7479848);
		
		Core::Position3D vPosMove((vPassPos->X - vStartPos->X) / fValue, (vPassPos->Y - vStartPos->Y) / fValue, (vPassPos->Z - vStartPos->Z) / fValue);
		Core::Position3D^ vNewPos = gcnew Core::Position3D(vStartPos->X, vStartPos->Y, vStartPos->Z);

		Core::Position3D vDirMove((vPassDir->X - vStartDir->X) / fValue, (vPassDir->Y - vStartDir->Y) / fValue, (vPassDir->Z - vStartDir->Z) / fValue);
		Core::Position3D^ vNewDir = gcnew Core::Position3D(vStartDir->X, vStartDir->Y, vStartDir->Z);

		//Position3D^ vNewDir = vStartDir;

		//UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);

		for (int i = 0; i < (int)fValue ; ++i)
		{
			vNewPos->X = vNewPos->X + vPosMove.X;
			vNewPos->Y = vNewPos->Y + vPosMove.Y;
			vNewPos->Z = vNewPos->Z + vPosMove.Z;

			vNewDir->X = vNewDir->X + vDirMove.X;
			vNewDir->Y = vNewDir->Y + vDirMove.Y;
			vNewDir->Z = vNewDir->Z + vDirMove.Z;

			SetCameraPosition(vNewPos);
			SetCameraDirection(vNewDir);

			RedrawScene();
			Sleep(30);
		}


		vStartPos = gcnew Core::Position3D(901.0919f, 852.5491f, 798.0175f);
		vStartDir = gcnew Core::Position3D((float)1.279113E-05, -0.6637159f, -0.7479848);

		Core::Position3D vPosMove2((targetCampos->X - vStartPos->X) / fValue, (targetCampos->Y - vStartPos->Y) / fValue, (targetCampos->Z - vStartPos->Z) / fValue);
		vNewPos = gcnew Core::Position3D(vStartPos->X, vStartPos->Y, vStartPos->Z);

		Core::Position3D vDirMove2((targetCamDir->X - vStartDir->X) / fValue, (targetCamDir->Y - vStartDir->Y) / fValue, (targetCamDir->Z - vStartDir->Z) / fValue);
		vNewDir = gcnew Core::Position3D(vStartDir->X, vStartDir->Y, vStartDir->Z);

		//Position3D^ vNewDir = vStartDir;

		//UnE::Core::UBaseView * pView = UDB::GetBaseView(mHWND);

		for (int i = 0; i < (int)fValue; ++i)
		{
			vNewPos->X = vNewPos->X + vPosMove2.X;
			vNewPos->Y = vNewPos->Y + vPosMove2.Y;
			vNewPos->Z = vNewPos->Z + vPosMove2.Z;

			vNewDir->X = vNewDir->X + vDirMove2.X;
			vNewDir->Y = vNewDir->Y + vDirMove2.Y;
			vNewDir->Z = vNewDir->Z + vDirMove2.Z;

			SetCameraPosition(vNewPos);
			SetCameraDirection(vNewDir);

			RedrawScene();
			Sleep(30);
		}

	}
}
#endif






