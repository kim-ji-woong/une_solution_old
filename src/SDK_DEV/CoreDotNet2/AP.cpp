#include "StdAfx.h"
#include "AP.h"

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


extern int hMainWnd;
extern wchar_t* ToWcharArray(System::String^ str);
extern System::String^ ToSystemString(wchar_t* str);
extern int WideToMulti(char* pszDst, const wchar_t* pwzIn, UINT uCodepage);
extern std::wstring MultiToWide( const char* str, int len, UINT CodaPage );



namespace Core
{



	extern UBaseView * GetBaseView();
	
	// APLayer
	//////////////////////////////////////////////////////////////////////////
	APLayer::APLayer(int nLayerID)
		: Layer(nLayerID, false)
	{
		m_nType = 3;
	}

	APLayer::~APLayer()
	{
	}

	void APLayer::Add(int nObjID)
	{
		APManager^ mgr = APManager::Instance;
		AP^ ap = mgr->GetAP(nObjID);
		if (ap != nullptr && !m_ObjList->Contains(ap))
			m_ObjList->Add(ap);
	}

	void APLayer::Remove(int nObjID)
	{
		APManager^ mgr = APManager::Instance;
		AP^ ap = mgr->GetAP(nObjID);
		if (ap != nullptr && m_ObjList->Contains(ap))
			m_ObjList->Remove(ap);
	}

	void APLayer::SetVisible(bool bShow)
	{
		if (Parent != nullptr)
		{
			int nCount = m_ObjList->Count;
			for (int i = 0; i < nCount; i++)
			{
				AP^ ap = (AP^)m_ObjList[i];
				ap->OnVisible(bShow);
			}
		}
	}

	void APLayer::SetLOD(int nLevel)
	{
		if (Parent != nullptr)
		{
			int nCount = m_ObjList->Count;
			for (int i = 0; i < nCount; i++)
			{
				AP^ ap = (AP^)m_ObjList[i];
				ap->SetLOD(nLevel);
			}
		}
	}

	//
	// AP
	//////////////////////////////////////////////////////////////////////////

	AP::~AP(void)
	{
	}

	AP::AP(System::String^ szName)
	{
		m_szName = szName;
		m_nIconID = -1;
		m_nTextID = -1;

		m_nLOD = 1;
		m_bVisible = false;
	}

	int AP::CreateAP(System::String^ szPath)
	{
		m_szIconPath = szPath;

		if( hMainWnd == 0)
			return -1;

		UBaseView * pView = GetBaseView();
		if( pView == NULL)
			return -1;

		char buf[4096];
		wchar_t * t = ToWcharArray(szPath);
		WideToMulti(buf, t, CP_ACP);
		std::string szTextPath = std::string(buf);
		delete[] t;	

		// Create Mesh
		// AddIcon
		m_nIconID = pView->AddIconPOI(szTextPath, 0.0f, 0.3f, 0.0f, false);
	

		wchar_t * t2 = ToWcharArray(m_szName);
		WideToMulti(buf, t2, CP_ACP);
		std::string szText = std::string(buf);
		delete[] t2;	
		// AddText
		m_nTextID = pView->AddTextPOI2(szText, 0.0f, 10.5f, 0.0f, false);
		pX = 0.0f, pY = 10.5f, pZ = 0.0f;

		// AddScenNode

		/*std::string szSceneName = pView->CloneSceneNode(std::string("ManCube"), 0.0f,  0.0f,0.0f, true);

		std::wstring szC1 = MultiToWide(szSceneName.c_str(), szSceneName.size(), CP_ACP);
		m_szSceneName = ToSystemString((wchar_t*)szC1.c_str());*/

		// visble false;
		APManager::Instance->AddAP(this);
		int nID = UDB::GetNextCookie();
		m_nWorkID = nID;
		return m_nWorkID;
	}

	void AP::SetLocation(float x, float y, float z)
	{
		UBaseView * pView = GetBaseView();
		if( pView == NULL)
			return;

		if( m_nIconID < 0 || m_nTextID < 0)
			return;
		//move scene
		//move icon;
		pView->MoveIconPOI(m_nIconID, x, y+0.3f, z);

		//move text;
		
		pView->MoveTextPOI2(m_nTextID, x, y + 10.5f, z);

		pX = x, pY = y +10.5f, pZ = z;

		/*char buf[4096];
		wchar_t * t3 = ToWcharArray(m_szSceneName);
		WideToMulti(buf, t3, CP_ACP);
		std::string szSceneName = std::string(buf);
		delete[] t3;	

		UBaseModel* pModel = UDB::GetBaseModel(hMainWnd);	
		if( pModel != NULL)
		{
			USceneNodeManager* pMgr = pModel->GetSecneManager();
			USceneNode * pNode = pMgr->FindSceneNode(szSceneName);
			pNode->SetPosition(UnE::Math::Vector3(x, y, z));
		}*/
	}

	bool AP::Select()
	{
		UBaseView * pView = GetBaseView();
		if( pView == NULL)
			return false;

		if( m_nIconID < 0 )
			return false;

		// Select icon
		pView->SelectIconPOI(m_nIconID, true);
		
		return true;
	}

	void AP::ClearSelect()
	{
		UBaseView * pView = GetBaseView();
		if( pView == NULL)
			return;

		if( m_nIconID < 0 )
			return;

		// Select icon
		pView->SelectIconPOI(m_nIconID, false);

		return;
	}

	void AP::Delete()
	{
		// delete icon
		UBaseView * pView = GetBaseView();
		if( pView == NULL)
			return;
		if( m_nIconID < 0 || m_nTextID < 0)
			return;

		// delete icon
		pView->RemovePOI(m_nIconID);
		
		// delete text
		pView->RemoveTextPOI2(m_nTextID);

		// delete scene
		/*char buf[4096];
		wchar_t * t3 = ToWcharArray(m_szSceneName);
		WideToMulti(buf, t3, CP_ACP);
		std::string szSceneName = std::string(buf);
		delete[] t3;	

		UBaseModel* pModel = UDB::GetBaseModel(hMainWnd);	
		if( pModel != NULL)
		{
			USceneNodeManager* pMgr = pModel->GetSecneManager();
			//USceneNode * pNode = pMgr->FindSceneNode(szSceneName);
			USceneNode * pNode =pMgr->GetRootSceneNode()->RemoveChild(szSceneName);
			if( pNode != NULL)
				delete pNode;
		}

		pView->RemoveSceneNode(szSceneName);*/

		m_nIconID = -1;
		m_nTextID = -1;
		//m_szSceneName = gcnew System::String("");
	}

	void AP::SetLOD(int nLOD)
	{
		m_nLOD = nLOD;

		if( m_nIconID < 0 || m_nTextID < 0)
			return;

		UBaseView * pView = GetBaseView();
		if( pView == NULL)
			return;

		/*char buf[4096];
		wchar_t * t3 = ToWcharArray(m_szSceneName);
		WideToMulti(buf, t3, CP_ACP);
		std::string szSceneName = std::string(buf);
		delete[] t3;	

		UBaseModel* pModel = UDB::GetBaseModel(hMainWnd);	
		USceneNodeManager* pMgr = pModel->GetSecneManager();
		USceneNode * pNode = pMgr->FindSceneNode(szSceneName);
		
		if (pNode == NULL)
			return;*/
		

		switch( nLOD )
		{
		case 4:
			pView->ShowIconPOI(m_nIconID, false);
			pView->ShowTextPOI2(m_nTextID, false);
			/*if (m_bShowNameOnly)
				pView->ShowTextPOI2(m_nTextID, false);
			else
				pView->ShowTextPOI2(m_nAccTextID, true);
			pNode->SetVisible(false);*/
			break;
		case 3:
			pView->ShowIconPOI(m_nIconID, false);
			pView->ShowTextPOI2(m_nTextID, false);
			/*if (m_bShowNameOnly)
				pView->ShowTextPOI2(m_nTextID, false);
			else
				pView->ShowTextPOI2(m_nAccTextID, false);
			pNode->SetVisible(false);*/
			break;
		case 2:
			pView->ShowIconPOI(m_nIconID, m_bVisible);
			pView->ShowTextPOI2(m_nTextID, m_bVisible);
			/*if (m_bShowNameOnly)
				pView->ShowTextPOI2(m_nTextID, m_bVisible);
			else
				pView->ShowTextPOI2(m_nAccTextID, m_bVisible);
			pNode->SetVisible(m_bVisible);*/
			break;
		case 1:	
			pView->ShowIconPOI(m_nIconID, m_bVisible);
			pView->ShowTextPOI2(m_nTextID, false);
			/*if (m_bShowNameOnly)
				pView->ShowTextPOI2(m_nTextID, false);
			else
				pView->ShowTextPOI2(m_nAccTextID, false);
			pNode->SetVisible(m_bVisible);*/
			break;
		}	
		pView->RenderOneFrame();			
	}

	void AP::OnVisible(bool bVisible)
	{
		m_bVisible = bVisible;
		
		SetLOD(m_nLOD);
	}

	//////////////////////////////////////////////////////////////////////////
	// APManager
	//////////////////////////////////////////////////////////////////////////
	APManager::APManager()
	{
		this->m_APList = gcnew System::Collections::ArrayList();
	}

	AP^ APManager::GetAP(int nID)
	{
		int nCount = m_APList->Count;
		for( int i = 0; i < nCount; i++)
		{
			AP^ node = (AP^)m_APList[i];
			if( node->WorkID == nID)
				return node;
		}
		return nullptr;
	}

	void APManager::AddAP(AP^ ap)
	{
		if (!m_APList->Contains(ap))
		{			
			m_APList->Add(ap);
		}
	}

	void APManager::RemoveAP(AP^ ap)
	{
		if (ap != nullptr)
			m_APList->Remove(ap);
	}

	void APManager::RemoveAP(int nID)
	{
		AP^ ap = GetAP(nID);
		if (ap != nullptr)
			m_APList->Remove(ap);
	}
	//////////////////////////////////////////////////////////////////////////
}


