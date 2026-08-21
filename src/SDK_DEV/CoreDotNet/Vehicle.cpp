#include "StdAfx.h"
#include "Vehicle.h"


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
using namespace System::Collections::Generic;


extern int hMainWnd;
extern wchar_t* ToWcharArray(System::String^ str);
extern System::String^ ToSystemString(wchar_t* str);
extern int WideToMulti(char* pszDst, const wchar_t* pwzIn, UINT uCodepage);
extern std::wstring MultiToWide( const char* str, int len, UINT CodaPage );

namespace Core
{
	

	extern UBaseView * GetBaseView();
	

	char * bufName [] = {
		"%d#mesh_60#182",
		"%d#mesh_50#152",
		"%d#mesh_53#161",
		"%d#mesh_56#170",
		"%d#mesh_63#191",
		"%d#mesh_64#194",
		"%d#mesh_65#197",
		"%d#mesh_66#200",
		"%d#mesh_67#203"
	};

	
	// VehicleLayer
	//////////////////////////////////////////////////////////////////////////
	VehicleLayer::VehicleLayer( int nLayerID )
		: Layer(nLayerID, false)
	{
		m_nType = 4;
		
	}

	VehicleLayer::~VehicleLayer()
	{
	}

	void VehicleLayer::Add( int nObjID )
	{
		VehicleManager^ mgr = VehicleManager::Instance;
		Vehicle^ vehicle = mgr->GetVehicle(nObjID);
		if( vehicle != nullptr && !m_ObjList->Contains(vehicle))
			m_ObjList->Add(vehicle);		
	}

	void VehicleLayer::Remove( int nObjID )
	{
		VehicleManager^ mgr = VehicleManager::Instance;
		Vehicle^ vehicle = mgr->GetVehicle(nObjID);
		if( vehicle != nullptr && m_ObjList->Contains(vehicle)) 
			m_ObjList->Remove(vehicle);		
	}

	void VehicleLayer::SetVisible( bool bShow )
	{
		if( Parent != nullptr)
		{
			int nCount  = m_ObjList->Count;
			for( int i = 0; i < nCount; i++)
			{
				Vehicle^ vehicle = (Vehicle^)m_ObjList[i];	
				vehicle->OnVisible(bShow);
			}			
		}
	}

	void VehicleLayer::SetLOD( int nLevel )
	{
		if( Parent != nullptr)
		{
			int nCount  = m_ObjList->Count;
			for( int i = 0; i < nCount; i++)
			{
				Vehicle^ vehicle = (Vehicle^)m_ObjList[i];	
				vehicle->SetLOD(nLevel);
			}			
		}
	}

	//
	// Vehicle
	//////////////////////////////////////////////////////////////////////////

	Vehicle::~Vehicle(void)
	{
	}

	Vehicle::Vehicle( System::String^ szName, VehicleType nType, float nWidth, float nLength, float nHeight )
	{
		m_szName = szName;
		m_nIconID = -1;
		m_nTextID = -1;

		m_nLOD = 1;
		m_bVisible = false;

		m_nWidth = nWidth;
		m_nLength = nLength;
		m_nHight = nHeight;
		m_nType = nType;

		m_arScens = gcnew System::Collections::ArrayList();
		m_arVecs = gcnew System::Collections::ArrayList();

		m_bFirstLocation = true;
	}

	int Vehicle::CreateVehicle( System::String^ szPath)
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

		int nID = UDB::GetNextCookie();
		m_nVehicleID = nID;
		// Create Mesh
		// AddIcon
		m_nIconID = pView->AddIconPOI(szTextPath, 0.0f, 0.2f, 0.0f, false);
		
		wchar_t * t2 = ToWcharArray(m_szName);
		WideToMulti(buf, t2, CP_ACP);
		std::string szText = std::string(buf);
		delete[] t2;	
		// AddText
		m_nTextID = pView->AddTextPOI2(szText, 0.0f, 10.5f, 0.0f, false);
		

		UBaseModel* pModel = UDB::GetBaseModel(hMainWnd);	
		USceneNodeManager* pMgr = pModel->GetSecneManager();
		// AddScenNode		
		//if( m_nType == VehicleType::FORKLIFT)
		//{			
		//	char buf[512];
		//
		//	UnE::Math::Vector3 vec2;	
		//	UnE::Math::Vector3 vec3;
		//	for(int i = 0 ; i < 9 ; i++)
		//	{
		//		sprintf(buf, bufName[i], hMainWnd);
		//		std::string szName1 = std::string(buf);

		//		if( i == 0)
		//		{
		//			std::string szName11 = pView->CloneSceneNode(szName1, 0.0f, 0.0f, 0.0f);

		//			std::wstring szC1 = MultiToWide(szName11.c_str(), szName11.size(), CP_ACP);
		//			System::String^ szSceneName = ToSystemString((wchar_t*)szC1.c_str());
		//			m_arScens->Add(szSceneName);

		//			USceneNode * pNode = pMgr->FindSceneNode(szName11);
		//			pNode->GetPosition(vec3);	
		//			pNode->ShowBoundingBox(false);
		//			//pNode->SetPosition(UnE::Math::Vector3(0.0f, 0.0f, 0.0f));
		//		}
		//		else
		//		{
		//			System::String^ szParentName = (System::String^)m_arScens[0];
		//			wchar_t * t3 = ToWcharArray(szParentName);
		//			WideToMulti(buf, t3, CP_ACP);
		//			std::string strParentName = std::string(buf);
		//			delete[] t3;

		//			std::string szName11 = pView->CloneSceneNode(szName1, strParentName, 0.0f, 0.0f, 0.0f);

		//			std::wstring szC1 = MultiToWide(szName11.c_str(), szName11.size(), CP_ACP);
		//			System::String^ szSceneName = ToSystemString((wchar_t*)szC1.c_str());
		//			m_arScens->Add(szSceneName);

		//			USceneNode * pNode = pMgr->FindSceneNode(szName11);
		//			pNode->GetPosition(vec2);
		//			pNode->ShowBoundingBox(false);
		//			UnE::Math::Vector3 vDir = vec3 - vec2;
		//			KeyValuePair<float, float>^ m = gcnew KeyValuePair<float, float>(vDir.x, vDir.z);

		//			m_arVecs->Add(m);
		//		}
		//	}			
		//}
		//else
		{
			char buf[512];
			sprintf(buf, "%d#vehicle#%d", hMainWnd, nID);
			std::string szBodyName1 = std::string(buf);
			pView->CreateSceneNode(szBodyName1, m_nLength, m_nWidth, m_nHight, 0.0f, 0.0f, 0.0f, true);
			
			std::wstring szC1 = MultiToWide(szBodyName1.c_str(), szBodyName1.size(), CP_ACP);
			m_szSceneName = ToSystemString((wchar_t*)szC1.c_str());
		}

	
		std::string strSceneName = "";
		/*if( m_nType == VehicleType::FORKLIFT)
		{
			System::String^ szSceneName = (System::String^)m_arScens[0];
			wchar_t * t3 = ToWcharArray(szSceneName);
			WideToMulti(buf, t3, CP_ACP);
			strSceneName = std::string(buf);
			delete[] t3;
		}
		else*/
		{
			//move scene
			wchar_t * t3 = ToWcharArray(m_szSceneName);
			WideToMulti(buf, t3, CP_ACP);
			strSceneName = std::string(buf);
			delete[] t3;
		}	
		USceneNode * pNode = pMgr->FindSceneNode(strSceneName);
		UnE::Math::Vector3 vec3;
		pNode->GetPosition(vec3);
		pNode->SetPosition(UnE::Math::Vector3(0, vec3.y, 0));
		
		// visble false;
		VehicleManager::Instance->AddVehicle(this);		
		return m_nVehicleID;
	}

	void Vehicle::SetLocation( float x, float y, float z )
	{
		UBaseView * pView = GetBaseView();
		if( pView == NULL)
			return;
		
		if( m_nIconID < 0 || m_nTextID < 0)
			return;

		char buf[4096];
		std::string strSceneName = "";
		/*if( m_nType == VehicleType::FORKLIFT)
		{
		System::String^ szSceneName = (System::String^)m_arScens[0];
		wchar_t * t3 = ToWcharArray(szSceneName);
		WideToMulti(buf, t3, CP_ACP);
		strSceneName = std::string(buf);
		delete[] t3;
		}
		else*/
		{
			//move scene
			wchar_t * t3 = ToWcharArray(m_szSceneName);
			WideToMulti(buf, t3, CP_ACP);
			strSceneName = std::string(buf);
			delete[] t3;
		}

		UBaseModel* pModel = UDB::GetBaseModel(hMainWnd);	
		USceneNodeManager* pMgr = pModel->GetSecneManager();
		USceneNode * pNode = pMgr->FindSceneNode(strSceneName);
		
		UnE::Math::Vector3 vec3;
		pNode->GetPosition(vec3);
		
		
		pNode->SetPosition(UnE::Math::Vector3(x, vec3.y, -z));
		
		//move icon;
		pView->MoveIconPOI(m_nIconID, x, y+m_nHight, -z);

		//move text;
		pView->MoveTextPOI2(m_nTextID, x, y+ 10.5f+m_nHight, -z);

		if( m_bFirstLocation == true)
		{
			m_bFirstLocation = false;
		}
		else
		{
			UnE::Math::Vector3 vec1 = vec3;	
			UnE::Math::Vector3 vec2 = UnE::Math::Vector3(x, vec3.y, -z);	
			UnE::Math::Vector3 vDir = vec2 - vec1;
			vDir.normalise();
			pNode->SetHeading(vDir);
		}
	}

	bool Vehicle::Select()
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

	void Vehicle::ClearSelect()
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

	void Vehicle::Delete()
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
		char buf[4096];
		wchar_t * t3 = ToWcharArray(m_szSceneName);
		WideToMulti(buf, t3, CP_ACP);
		std::string szSceneName = std::string(buf);
		delete[] t3;	

		UBaseModel* pModel = UDB::GetBaseModel(hMainWnd);	
		if( pModel != NULL)
		{
			USceneNodeManager* pMgr = pModel->GetSecneManager();
			USceneNode * pNode = pMgr->GetRootSceneNode()->RemoveChild(szSceneName);
			if (pNode != NULL)
				delete pNode;
		}

		pView->RemoveSceneNode(szSceneName);

		m_nIconID = -1;
		m_nTextID = -1;
		m_szSceneName = gcnew System::String("");
	}

	void Vehicle::SetLOD(int nLOD)
	{
		m_nLOD = nLOD;

		UBaseView * pView = GetBaseView();
		if( pView == NULL)
			return;

		if( m_nIconID < 0 || m_nTextID < 0)
			return;

		//if( m_nType == VehicleType::FORKLIFT)
		//{	
		//	char buf[4096];
		//	UBaseModel* pModel = UDB::GetBaseModel(hMainWnd);	
		//	USceneNodeManager* pMgr = pModel->GetSecneManager();

		//	System::String^ szSceneName = (System::String^)m_arScens[0];
		//	wchar_t * t3 = ToWcharArray(szSceneName);
		//	WideToMulti(buf, t3, CP_ACP);
		//	std::string strSceneName = std::string(buf);
		//	delete[] t3;
		//	USceneNode * pNode = pMgr->FindSceneNode(strSceneName);

		//	switch( nLOD )
		//	{
		//	case 3:
		//		pView->ShowIconPOI(m_nIconID, false);
		//		pView->ShowTextPOI(m_nTextID, false);		

		//		
		//		pNode->SetVisible(false);
		//		//}	
		//		break;
		//	case 2:
		//		pView->ShowIconPOI(m_nIconID, m_bVisible);
		//		pView->ShowTextPOI(m_nTextID, m_bVisible);	
		//		/*for(int i = 0 ; i < m_arScens->Count ; i++)
		//		{
		//			System::String^ szSceneName = (System::String^)m_arScens[i];
		//			wchar_t * t3 = ToWcharArray(szSceneName);
		//			WideToMulti(buf, t3, CP_ACP);
		//			std::string strSceneName = std::string(buf);
		//			delete[] t3;*/

		//			//USceneNode * pNode = pMgr->FindSceneNode(strSceneName);
		//			pNode->SetVisible(m_bVisible);
		//		//}	
		//		break;
		//	case 1:	
		//		pView->ShowIconPOI(m_nIconID, m_bVisible);
		//		pView->ShowTextPOI(m_nTextID, false);
		//		/*for(int i = 0 ; i < m_arScens->Count ; i++)
		//		{
		//			System::String^ szSceneName = (System::String^)m_arScens[i];
		//			wchar_t * t3 = ToWcharArray(szSceneName);
		//			WideToMulti(buf, t3, CP_ACP);
		//			std::string strSceneName = std::string(buf);
		//			delete[] t3;*/

		//			//USceneNode * pNode = pMgr->FindSceneNode(strSceneName);
		//			pNode->SetVisible(m_bVisible);
		//		//}	
		//		break;
		//	}
		//}
		//else
		{
			char buf[4096];
			wchar_t * t3 = ToWcharArray(m_szSceneName);
			WideToMulti(buf, t3, CP_ACP);
			std::string szSceneName = std::string(buf);
			delete[] t3;	

			UBaseModel* pModel = UDB::GetBaseModel(hMainWnd);	
			USceneNodeManager* pMgr = pModel->GetSecneManager();
			USceneNode * pNode = pMgr->FindSceneNode(szSceneName);

			switch( nLOD )
			{
			case 3:
				pView->ShowIconPOI(m_nIconID, false);
				pView->ShowTextPOI2(m_nTextID, false);
				pNode->SetVisible(false);
				break;
			case 2:
				pView->ShowIconPOI(m_nIconID, m_bVisible);
				pView->ShowTextPOI2(m_nTextID, m_bVisible);	
				pNode->SetVisible(m_bVisible);
				break;
			case 1:	
				pView->ShowIconPOI(m_nIconID, m_bVisible);
				pView->ShowTextPOI2(m_nTextID, false);
				pNode->SetVisible(m_bVisible);
				break;
			}	
		}		
		pView->RenderOneFrame();		
	}

	void Vehicle::OnVisible( bool bVisible )
	{
		m_bVisible = bVisible;

		SetLOD(m_nLOD);
	}

	//////////////////////////////////////////////////////////////////////////
	// VehicleManager
	//////////////////////////////////////////////////////////////////////////
	VehicleManager::VehicleManager()
	{
		this->m_VehicleList = gcnew System::Collections::ArrayList();
		
		UBaseModel* pModel = UDB::GetBaseModel(hMainWnd);	
		USceneNodeManager* pMgr = pModel->GetSecneManager();
		char buf[512];
		for (int i = 0; i < 9; i++)
		{
			sprintf(buf, bufName[i], hMainWnd);
			std::string szName1 = std::string(buf);
			USceneNode * pNode = pMgr->FindSceneNode(szName1);
			if (pNode != NULL)
			{
				pNode->SetVisible(false);
			}	
		}
	}

	Vehicle^ VehicleManager::GetVehicle( int nID )
	{
		int nCount  = m_VehicleList->Count;
		for( int i = 0; i < nCount; i++)
		{
			Vehicle^ node = (Vehicle^)m_VehicleList[i];	
			if( node->VehicleID == nID)
				return node;
		}
		return nullptr;
	}

	void VehicleManager::AddVehicle( Vehicle^ vehicle )
	{
		if( !m_VehicleList->Contains( vehicle ))
		{			
			m_VehicleList->Add(vehicle);
		}
	}

	void VehicleManager::RemoveVehicle( Vehicle^ vehicle )
	{
		if( vehicle != nullptr)
			m_VehicleList->Remove(vehicle);
	}

	void VehicleManager::RemoveVehicle( int nID )
	{
		Vehicle^ vehicle = GetVehicle(nID);
		if( vehicle != nullptr)
			m_VehicleList->Remove(vehicle);
	}
}