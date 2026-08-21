#include "StdAfx.h"

#include <string>
#include "MovingEquipment.h"
#include "Crane.h"


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


extern int hMainWnd;
extern wchar_t* ToWcharArray(System::String^ str);
extern System::String^ ToSystemString(wchar_t* str);
extern int WideToMulti(char* pszDst, const wchar_t* pwzIn, UINT uCodepage);
extern std::wstring MultiToWide( const char* str, int len, UINT CodaPage );

namespace Core
{
	extern UBaseView * GetBaseView();
	
	// WorkerLayer
	//////////////////////////////////////////////////////////////////////////
	EquipmentLayer::EquipmentLayer( int nLayerID )
		: Layer(nLayerID, false)
	{
		m_nType = 5;
	}

	EquipmentLayer::~EquipmentLayer()
	{
	}
	void EquipmentLayer::Add( int nObjID )
	{
		throw gcnew System::NotImplementedException();
	}

	void EquipmentLayer::Add( int nObjID , int nType)
	{
		if(nType == 0)
		{
			CraneManager^ mgr = CraneManager::Instance;
			Crane^ worker = mgr->GetCrane(nObjID);
			if( worker != nullptr && !m_ObjList->Contains(worker))
				m_ObjList->Add(worker);		
		}
		if(nType == 1)
		{
			MovingEquipmentManager^ mgr = MovingEquipmentManager::Instance;
			MovingEquipment^ worker = mgr->GetEquipment(nObjID);
			if( worker != nullptr && !m_ObjList->Contains(worker))
				m_ObjList->Add(worker);	
		}		
	}
	void EquipmentLayer::Remove( int nObjID )
	{
		throw gcnew System::NotImplementedException();
	}
	void EquipmentLayer::Remove( int nObjID , int nType)
	{
		if(nType == 0)
		{
			CraneManager^ mgr = CraneManager::Instance;
			Crane^ worker = mgr->GetCrane(nObjID);
			if( worker != nullptr && !m_ObjList->Contains(worker))
				m_ObjList->Remove(worker);		
		}
		if(nType == 1)
		{
			MovingEquipmentManager^ mgr = MovingEquipmentManager::Instance;
			MovingEquipment^ worker = mgr->GetEquipment(nObjID);
			if( worker != nullptr && !m_ObjList->Contains(worker))
				m_ObjList->Remove(worker);	
		}
	}

	void EquipmentLayer::SetVisible( bool bShow )
	{
		if( Parent != nullptr)
		{
			int nCount  = m_ObjList->Count;
			for( int i = 0; i < nCount; i++)
			{
				System::Object^ obj = m_ObjList[i];
				if( obj->GetType() == Crane::typeid)
				{
					Crane^ worker = (Crane^)obj;
					worker->OnVisible(bShow);
				}
				else
				{
					MovingEquipment^ worker = (MovingEquipment^)obj;
					worker->OnVisible(bShow);
				}				
			}			
		}
	}

	void EquipmentLayer::SetLOD( int nLevel )
	{
		if( Parent != nullptr)
		{
			int nCount  = m_ObjList->Count;
			for( int i = 0; i < nCount; i++)
			{
				System::Object^ obj = m_ObjList[i];
				if( obj->GetType() == Crane::typeid)
				{
					Crane^ worker = (Crane^)obj;
					worker->SetLOD(nLevel);
				}
				else
				{
					MovingEquipment^ worker = (MovingEquipment^)obj;
					worker->SetLOD(nLevel);
				}
			}			
		}
	}

	//////////////////////////////////////////////////////////////////////////


	MovingEquipment::MovingEquipment( int nID )
	{
		m_nID = nID;
		m_nTextID = -1;
		m_nLOD = 1;
		m_bVisible = false;


		m_fMinValue = 0;
		m_fMaxValue = 10;
	}

	MovingEquipment::~MovingEquipment( void )
	{
	}

	bool MovingEquipment::GetVisible()
	{
		return m_bVisible;
	}

	System::Collections::ArrayList^ MovingEquipment::GetBound()
	{
		System::Collections::ArrayList^ arBound = gcnew System::Collections::ArrayList();

		char buf[4096];
		wchar_t * t = ToWcharArray(m_szBodyName);
		WideToMulti(buf, t, CP_ACP);
		std::string szBodyName = std::string(buf);
		delete[] t;

		float y = 0.0f;
		float y2 = 0.0f;
		UBaseModel* pModel = UDB::GetBaseModel(hMainWnd);
		if (pModel != NULL)
		{
			USceneNodeManager* pMgr = pModel->GetSecneManager();
			USceneNode * pNode = pMgr->FindSceneNode(szBodyName);

			UnE::Math::AxisAlignedBox aabb;
			pNode->GetBoundBox(aabb);

			const UnE::Math::Vector3 * pAllCorner = aabb.getAllCorners();
			if (pAllCorner != NULL)
			{
				for (int i = 0; i < 8; i++)
				{
					Position3D^ pos1 = gcnew Position3D(pAllCorner[i].x, pAllCorner[i].y, pAllCorner[i].z);
					arBound->Add(pos1);
				}
			}
						
		}
		return arBound;
	}

	int MovingEquipment::Create( System::String^ body )
	{
		m_szBodyName = body;
		m_szEquipName = System::String::Format("Equip {0}", m_nID); 

		char buf[4096];
		wchar_t * t = ToWcharArray(m_szBodyName);
		WideToMulti(buf, t, CP_ACP);
		std::string szBodyName = std::string(buf);
		delete[] t;	

		float y = 0.0f;
		float y2 = 0.0f;
		UBaseModel* pModel = UDB::GetBaseModel(hMainWnd);	
		if( pModel != NULL)
		{
			USceneNodeManager* pMgr = pModel->GetSecneManager();
			USceneNode * pNode = pMgr->FindSceneNode(szBodyName);
			
			UnE::Math::Vector3 vec;
			pNode->GetPosition(vec);
			y = vec.y;
			
		}

		wchar_t * t4 = ToWcharArray(m_szEquipName);
		WideToMulti(buf, t4 ,CP_ACP);
		std::string szText = std::string(buf);
		delete[] t4;	
		// AddText
		UBaseView* pView = GetBaseView();
		m_nTextID = pView->AddTextPOI2(szText, 0.0f, 2.5f, 0.0f, false);

		return m_nID;
	}

	void MovingEquipment::SetInitLocation( float x, float y, float z )
	{
		char buf[4096];
		wchar_t * t = ToWcharArray(m_szBodyName);
		WideToMulti(buf, t, CP_ACP);
		std::string szBodyName = std::string(buf);
		delete[] t;	
			

		UBaseModel* pModel = UDB::GetBaseModel(hMainWnd);	
		if( pModel != NULL)
		{
			USceneNodeManager* pMgr = pModel->GetSecneManager();
			USceneNode * pNode = pMgr->FindSceneNode(szBodyName);


			UnE::Math::Vector3 vec;
			pNode->GetPosition(vec);
			y = vec.y;
			//z = vec.z;
			pNode->SetPosition(UnE::Math::Vector3(x, y, z));
			
		}
	}

	float MovingEquipment::GetLocation()
	{
		char buf[512];
		wchar_t * t = ToWcharArray(m_szBodyName);
		WideToMulti(buf, t, CP_ACP);
		std::string szBodyName = std::string(buf);
		delete[] t;

		UBaseModel* pModel = UDB::GetBaseModel(hMainWnd);
		if (pModel != NULL)
		{
			USceneNodeManager* pMgr = pModel->GetSecneManager();
			USceneNode * pNode = pMgr->FindSceneNode(szBodyName);
			UnE::Math::Vector3 vec;
			pNode->GetPosition(vec);
			float z = -69.5f + vec.z;
			z = z * -1.0f;	
			return z;
		}
		return -100.0f;
	}

	int MovingEquipment::SetLocation( float z )
	{
		char buf[1024];
		wchar_t * t = ToWcharArray(m_szBodyName);
		WideToMulti(buf, t, CP_ACP);
		std::string szBodyName = std::string(buf);
		delete[] t;	

		UBaseModel* pModel = UDB::GetBaseModel(hMainWnd);	
		if( pModel != NULL)
		{
			USceneNodeManager* pMgr = pModel->GetSecneManager();
			USceneNode * pNode = pMgr->FindSceneNode(szBodyName);
			UnE::Math::Vector3 vec;
			pNode->GetPosition(vec);
			if( z > m_fMaxValue)
			{
				z = m_fMaxValue;
			}
			else if( z < m_fMinValue)
			{
				z = m_fMinValue;
			}

			SetLocation( vec.x, vec.y, -69.5f + z); 


		}
		return 0;
	}

	void MovingEquipment::SetLocation( float x, float y, float z )
	{
		char buf[4096];
		wchar_t * t = ToWcharArray(m_szBodyName);
		WideToMulti(buf, t, CP_ACP);
		std::string szBodyName = std::string(buf);
		delete[] t;		

		UBaseModel* pModel = UDB::GetBaseModel(hMainWnd);	
		if( pModel != NULL)
		{
			USceneNodeManager* pMgr = pModel->GetSecneManager();
			USceneNode * pNode = pMgr->FindSceneNode(szBodyName);

			pNode->SetPosition(UnE::Math::Vector3(x, y, z));		
		}

		UBaseView* pView = GetBaseView();
		pView->MoveTextPOI2(m_nTextID, x, y + 3.0f, z);
	}


	bool MovingEquipment::Select()
	{
		return false;
	}

	void MovingEquipment::ClearSelect()
	{
	}

	void MovingEquipment::Delete()
	{
	}

	void MovingEquipment::OnVisible( bool bVisible )
	{
		m_bVisible = bVisible;

		SetLOD(m_nLOD);
	}

	void MovingEquipment::SetLOD( int nLOD )
	{
		m_nLOD = nLOD;

		UBaseView * pView = GetBaseView();
		if( pView == NULL)
			return;

		UBaseModel* pModel = UDB::GetBaseModel(hMainWnd);	
		USceneNodeManager* pMgr = pModel->GetSecneManager();
		
		char buf[4096];
		wchar_t * t = ToWcharArray(m_szBodyName);
		WideToMulti(buf, t, CP_ACP);
		std::string szBodyName = std::string(buf);
		delete[] t;	


		USceneNode * pNode = pMgr->FindSceneNode(szBodyName);		

		switch( nLOD )
		{
		case 3:
			//pView->ShowIconPOI(m_nIconID, false);
			pView->ShowTextPOI2(m_nTextID, false);
			pNode->SetVisible(false);		
			break;
		case 2:
			//pView->ShowIconPOI(m_nIconID, m_bVisible);
			pView->ShowTextPOI2(m_nTextID, m_bVisible);	
			pNode->SetVisible(m_bVisible);			
			break;
		case 1:	
			//pView->ShowIconPOI(m_nIconID, m_bVisible);
			pView->ShowTextPOI2(m_nTextID, false);
			pNode->SetVisible(m_bVisible);			
			break;
		}	
		pView->RenderOneFrame();
	}

	void MovingEquipment::SetMaxValue( float maxValue )
	{
		m_fMaxValue = maxValue;
	}

	float MovingEquipment::GetMaxValue()
	{
		return m_fMaxValue;
	}

	void MovingEquipment::SetMinValue( float minValue )
	{
		m_fMinValue;
	}

	float MovingEquipment::GetMinValue()
	{
		return m_fMinValue;
	}

	// CraneManager
	//////////////////////////////////////////////////////////////////////////
	MovingEquipment^ MovingEquipmentManager::GetEquipment( int idx )
	{
		return m_Equipment;
	}

	MovingEquipmentManager::MovingEquipmentManager()
	{
		CreateMovingEquipment();
	}

	MovingEquipmentManager::~MovingEquipmentManager()
	{
	}

	void MovingEquipmentManager::CreateMovingEquipment()
	{
		UnE::Core::UBaseView * pView = UDB::GetBaseView(hMainWnd);
		if( pView != NULL)
		{
			std::string szNodeName = std::string("MovingEquip");
			std::wstring szC6 = MultiToWide(szNodeName.c_str(), szNodeName.size(), CP_ACP);
			System::String^ szCraPinName2 = ToSystemString((wchar_t*)szC6.c_str());

			UBaseModel* pModel = UDB::GetBaseModel(hMainWnd);	
			USceneNodeManager* pMgr = pModel->GetSecneManager();
			USceneNode * pNode = pMgr->FindSceneNode(szNodeName);

			UnE::Math::Vector3 vec;
			pNode->GetPosition(vec);
			float y = vec.y;
		
			m_Equipment = gcnew MovingEquipment(0);
			m_Equipment->Create(szCraPinName2);


		}
	}
}
