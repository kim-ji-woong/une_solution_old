#include "StdAfx.h"
#include "Worker.h"

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



	UBaseView * GetBaseView()
	{
		if (hMainWnd == 0)
			return NULL;

		UBaseView * pView = UDB::GetBaseView(hMainWnd);
		return pView;
	}

	// WorkerLayer
	//////////////////////////////////////////////////////////////////////////
	WorkerLayer::WorkerLayer(int nLayerID)
		: Layer(nLayerID, false)
	{
		m_nType = 3;
	}

	WorkerLayer::~WorkerLayer()
	{
	}

	void WorkerLayer::Add(int nObjID)
	{
		WorkerManager^ mgr = WorkerManager::Instance;
		Worker^ worker = mgr->GetWorker(nObjID);
		if (worker != nullptr && !m_ObjList->Contains(worker))
			m_ObjList->Add(worker);
	}

	void WorkerLayer::Remove(int nObjID)
	{
		WorkerManager^ mgr = WorkerManager::Instance;
		Worker^ worker = mgr->GetWorker(nObjID);
		if (worker != nullptr && m_ObjList->Contains(worker))
			m_ObjList->Remove(worker);
	}

	void WorkerLayer::SetVisible(bool bShow)
	{
		if (Parent != nullptr)
		{
			int nCount = m_ObjList->Count;
			for (int i = 0; i < nCount; i++)
			{
				Worker^ worker = (Worker^)m_ObjList[i];
				worker->OnVisible(bShow);
			}
		}
	}

	void WorkerLayer::SetLOD(int nLevel)
	{
		if (Parent != nullptr)
		{
			int nCount = m_ObjList->Count;
			for (int i = 0; i < nCount; i++)
			{
				Worker^ worker = (Worker^)m_ObjList[i];
				worker->SetLOD(nLevel);
			}
		}
	}

	//
	// Worker
	//////////////////////////////////////////////////////////////////////////

	Worker::~Worker(void)
	{
	}

	Worker::Worker(System::String^ szName)
	{
		m_szName = szName;
		m_nIconID = -1;
		m_nTextID = -1;

		m_nLOD = 1;
		m_bVisible = false;
		m_bShowNameOnly = true;
		m_nAccTextID = -1;
	}

	void Worker::ClearSetAccidentText()
	{
		m_szAccText = nullptr;
		if (m_nAccTextID > 0)
		{
			if (m_bShowNameOnly == false)
			{
				ToggleText(true);
			}
			UBaseView * pView = GetBaseView();
			if (pView == NULL)
				return;

			pView->RemoveTextPOI2(m_nAccTextID);
			m_nAccTextID = -1;
		}
			
	}

	void Worker::SetAccidentText(System::String^ szText)
	{
		m_szAccText = nullptr;

		if (szText == nullptr)
			return;

		UBaseView * pView = GetBaseView();
		if (pView == NULL)
			return;

		if (m_nAccTextID > 0)
		{			
			UBaseView * pView = GetBaseView();
			if (pView == NULL)
				return;

			pView->RemoveTextPOI2(m_nAccTextID);
			m_nAccTextID = -1;
		}


		m_szAccText = m_szName + " - " + szText;

		char buf[4096];
		wchar_t * t = ToWcharArray(m_szAccText);
		WideToMulti(buf, t, CP_ACP);
		std::string szTextPath = std::string(buf);
		delete[] t;
		
		m_nAccTextID = pView->AddTextPOI2(szTextPath, pX, pY, pZ, false);
		pView->MoveTextPOI2(m_nAccTextID, pX, pY, pZ);
	}

	void Worker::ToggleText(bool bNameOnly)
	{
		if (m_nAccTextID == -1)
			return;

		m_bShowNameOnly = bNameOnly;
		if (m_bShowNameOnly)
		{
			UBaseView * pView = GetBaseView();
			if (pView == NULL)
				return;
			pView->ShowTextPOI2(m_nAccTextID, false);
			switch (m_nLOD)
			{
			case 4:
				pView->ShowTextPOI2(m_nTextID, false);
			case 3:
				pView->ShowTextPOI2(m_nTextID, false);
				break;
			case 2:
				pView->ShowTextPOI2(m_nTextID, m_bVisible);
				break;
			case 1:
				pView->ShowTextPOI2(m_nTextID, false);
				break;
			}
		}
		else
		{
			UBaseView * pView = GetBaseView();
			if (pView == NULL)
				return;
			pView->ShowTextPOI2(m_nTextID, false);
			switch (m_nLOD)
			{
			case 4:
				pView->ShowTextPOI2(m_nAccTextID, true);
			case 3:
				pView->ShowTextPOI2(m_nAccTextID, false);
				break;
			case 2:
				pView->ShowTextPOI2(m_nAccTextID, m_bVisible);
				break;
			case 1:
				pView->ShowTextPOI2(m_nAccTextID, false);
				break;
			}
		}
	}

	
	int Worker::CreateWorker( System::String^ szPath)
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

		std::string szSceneName = pView->CloneSceneNode(std::string("ManCube"), 0.0f,  0.0f,0.0f, true);

		std::wstring szC1 = MultiToWide(szSceneName.c_str(), szSceneName.size(), CP_ACP);
		m_szSceneName = ToSystemString((wchar_t*)szC1.c_str());

		// visble false;
		WorkerManager::Instance->AddWorker(this);
		int nID = UDB::GetNextCookie();
		m_nWorkID = nID;
		return m_nWorkID;
	}

	void Worker::SetLocation( float x, float y, float z )
	{
		UBaseView * pView = GetBaseView();
		if( pView == NULL)
			return;

		if( m_nIconID < 0 || m_nTextID < 0)
			return;
		//move scene
		//move icon;
		pView->MoveIconPOI(m_nIconID, x, y+0.3, z);

		//move text;
		
		pView->MoveTextPOI2(m_nTextID, x, y + 10.5f, z);

		pX = x, pY = y +10.5f, pZ = z;

		if (m_nAccTextID > 0)
			pView->MoveTextPOI2(m_nAccTextID, x, y + 10.5f, z);

		char buf[4096];
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
		}
	}

	bool Worker::Select()
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

	void Worker::ClearSelect()
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

	void Worker::Delete()
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

		if (m_nAccTextID > 0)
			pView->RemoveTextPOI2(m_nAccTextID);

		// delete scene
		char buf[4096];
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

		pView->RemoveSceneNode(szSceneName);

		m_nIconID = -1;
		m_nTextID = -1;
		m_szSceneName = gcnew System::String("");
	}

	void Worker::SetLOD(int nLOD)
	{
		m_nLOD = nLOD;

		if( m_nIconID < 0 || m_nTextID < 0)
			return;

		UBaseView * pView = GetBaseView();
		if( pView == NULL)
			return;

		char buf[4096];
		wchar_t * t3 = ToWcharArray(m_szSceneName);
		WideToMulti(buf, t3, CP_ACP);
		std::string szSceneName = std::string(buf);
		delete[] t3;	

		UBaseModel* pModel = UDB::GetBaseModel(hMainWnd);	
		USceneNodeManager* pMgr = pModel->GetSecneManager();
		USceneNode * pNode = pMgr->FindSceneNode(szSceneName);
		
		if (pNode == NULL)
			return;
		

		switch( nLOD )
		{
		case 4:
			pView->ShowIconPOI(m_nIconID, false);
			if (m_bShowNameOnly)
				pView->ShowTextPOI2(m_nTextID, false);
			else
				pView->ShowTextPOI2(m_nAccTextID, true);
			pNode->SetVisible(false);
		case 3:
			pView->ShowIconPOI(m_nIconID, false);
			if (m_bShowNameOnly)
				pView->ShowTextPOI2(m_nTextID, false);
			else
				pView->ShowTextPOI2(m_nAccTextID, false);
			pNode->SetVisible(false);
			break;
		case 2:
			pView->ShowIconPOI(m_nIconID, m_bVisible);
			if (m_bShowNameOnly)
				pView->ShowTextPOI2(m_nTextID, m_bVisible);
			else
				pView->ShowTextPOI2(m_nAccTextID, m_bVisible);
			pNode->SetVisible(m_bVisible);
			break;
		case 1:	
			pView->ShowIconPOI(m_nIconID, m_bVisible);
			if (m_bShowNameOnly)
				pView->ShowTextPOI2(m_nTextID, false);
			else
				pView->ShowTextPOI2(m_nAccTextID, false);
			pNode->SetVisible(m_bVisible);
			break;
		}	
		pView->RenderOneFrame();			
	}

	void Worker::OnVisible( bool bVisible )
	{
		m_bVisible = bVisible;
		
		SetLOD(m_nLOD);
	}

	//////////////////////////////////////////////////////////////////////////
	// WorkerManager
	//////////////////////////////////////////////////////////////////////////
	WorkerManager::WorkerManager()
	{
		this->m_WorkList = gcnew System::Collections::ArrayList();
	}

	Worker^ WorkerManager::GetWorker( int nID )
	{
		int nCount  = m_WorkList->Count;
		for( int i = 0; i < nCount; i++)
		{
			Worker^ node = (Worker^)m_WorkList[i];	
			if( node->WorkID == nID)
				return node;
		}
		return nullptr;
	}

	void WorkerManager::AddWorker( Worker^ worker )
	{
		if( !m_WorkList->Contains( worker ))
		{			
			m_WorkList->Add(worker);
		}
	}

	void WorkerManager::RemoveWorker( Worker^ worker )
	{
		if( worker != nullptr)
			m_WorkList->Remove(worker);
	}

	void WorkerManager::RemoveWorker( int nID )
	{
		Worker^ worker = GetWorker(nID);
		if( worker != nullptr)
			m_WorkList->Remove(worker);
	}
	//////////////////////////////////////////////////////////////////////////
}


