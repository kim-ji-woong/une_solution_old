#include "StdAfx.h"

#include <string>
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
	Crane::Crane( int nID )
	{
		m_nID = nID;

		m_nTextID = -1;
		m_nLOD = 1;
		m_bVisible = false;
		m_bfirst = true;
	}

	Crane::~Crane( void )
	{
	}

	bool Crane::GetVisible()
	{
		return m_bVisible;
	}

	int Crane::CreateCrane( System::String^ body, System::String^ line, System::String^ pin )
	{
		m_szCraneName = System::String::Format("Crane {0}", m_nID);

		m_szBodyName = body;
		m_szLineName = line;
		m_szPinName = pin;

		char buf[4096];
		wchar_t * t = ToWcharArray(m_szBodyName);
		WideToMulti(buf, t, CP_ACP);
		std::string szBodyName = std::string(buf);
		delete[] t;	

		wchar_t * t2 = ToWcharArray(m_szLineName);
		WideToMulti(buf, t2, CP_ACP);
		std::string szLineName = std::string(buf);
		delete[] t2;	

		wchar_t * t3 = ToWcharArray(m_szPinName);
		WideToMulti(buf, t3, CP_ACP);
		std::string szPinName = std::string(buf);
		delete[] t3;	


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

			USceneNode * pNode2 = pMgr->FindSceneNode(szLineName);

			pNode2->GetPosition(vec);
			
		}

		wchar_t * t4 = ToWcharArray(m_szCraneName);
		WideToMulti(buf, t4 ,CP_ACP);
		std::string szText = std::string(buf);
		delete[] t4;	
		// AddText
		UBaseView* pView = GetBaseView();
		m_nTextID = pView->AddTextPOI2(szText, 0.0f, 2.5f, 0.0f, false);

		return m_nID;
	}

	void Crane::SetInitLocation( float x, float y, float z )
	{
		char buf[4096];
		wchar_t * t = ToWcharArray(m_szBodyName);
		WideToMulti(buf, t, CP_ACP);
		std::string szBodyName = std::string(buf);
		delete[] t;	

		wchar_t * t2 = ToWcharArray(m_szLineName);
		WideToMulti(buf, t2, CP_ACP);
		std::string szLineName = std::string(buf);
		delete[] t2;	

		wchar_t * t3 = ToWcharArray(m_szPinName);
		WideToMulti(buf, t3, CP_ACP);
		std::string szPinName = std::string(buf);
		delete[] t3;	

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

			UBaseView* pView = GetBaseView();
			pView->MoveTextPOI2(m_nTextID, x, y + 2.5f, z);

			USceneNode * pNode2 = pMgr->FindSceneNode(szLineName);
			
			pNode2->GetPosition(vec);
			y = vec.y;
			//z = vec.z;
			pNode2->SetPosition(UnE::Math::Vector3(x, y, z));
			
			m_initZlocation = vec.z;

			USceneNode * pNode3 = pMgr->FindSceneNode(szPinName);
			pNode3->GetPosition(vec);
			y = vec.y;
			//z = vec.z;
			pNode3->SetPosition(UnE::Math::Vector3(x, y, z));
		}

		
	}
	
	float Crane::GetHookLocation()
	{
		char buf[512];
		wchar_t * t2 = ToWcharArray(m_szLineName);
		WideToMulti(buf, t2, CP_ACP);
		std::string szLineName = std::string(buf);
		delete[] t2;

		UBaseModel* pModel = UDB::GetBaseModel(hMainWnd);
		if (pModel != NULL)
		{			
			USceneNodeManager* pMgr = pModel->GetSecneManager();			
			USceneNode * pNode2 = pMgr->FindSceneNode(szLineName);

			UnE::Math::Vector3 vec;
			pNode2->GetPosition(vec);
			float z = m_initZlocation - vec.z;
			return z;
		}
		return -100.0f;
	}

	System::Collections::ArrayList^ Crane::GetHookBound()
	{
		System::Collections::ArrayList^ arBound = gcnew System::Collections::ArrayList();

		char buf[4096];
		wchar_t * t2 = ToWcharArray(m_szLineName);
		WideToMulti(buf, t2, CP_ACP);
		std::string szLineName = std::string(buf);
		delete[] t2;

		float y = 0.0f;
		float y2 = 0.0f;
		UBaseModel* pModel = UDB::GetBaseModel(hMainWnd);
		if (pModel != NULL)
		{
			USceneNodeManager* pMgr = pModel->GetSecneManager();

			USceneNode * pNode2 = pMgr->FindSceneNode(szLineName);


			UnE::Math::AxisAlignedBox aabb;
			pNode2->GetBoundBox(aabb);

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

	int Crane::SetHookLocation(float zz)
	{
		
		if (zz < -9)
			zz = -9;
		if (zz > 9)
			zz = 9;

		char buf[4096];

		wchar_t * t2 = ToWcharArray(m_szLineName);
		WideToMulti(buf, t2, CP_ACP);
		std::string szLineName = std::string(buf);
		delete[] t2;

		UBaseModel* pModel = UDB::GetBaseModel(hMainWnd);
		if (pModel != NULL)
		{
			USceneNodeManager* pMgr = pModel->GetSecneManager();

			UnE::Math::Vector3 vec;	

			USceneNode * pNode2 = pMgr->FindSceneNode(szLineName);
			
			pNode2->GetPosition(vec);

			if (m_bfirst == true)
			{
				m_bfirst = false;
				m_initZlocation = vec.z;
			}
			float x = vec.x; 
			float y = vec.y;
			float z = m_initZlocation - zz;
			pNode2->SetPosition(UnE::Math::Vector3(x, y, z));
		}
		return 0;
	}

	float Crane::GetLocation()
	{
		char buf[512];
		wchar_t * t = ToWcharArray(m_szBodyName);
		WideToMulti(buf, t, CP_ACP);
		std::string szBodyName = std::string(buf);
		delete[] t;

		float y = 0.0f;
		float z = 0.0f;
		UBaseModel* pModel = UDB::GetBaseModel(hMainWnd);
		if (pModel != NULL)
		{
			USceneNodeManager* pMgr = pModel->GetSecneManager();
			USceneNode * pNode = pMgr->FindSceneNode(szBodyName);
			UnE::Math::Vector3 vec;
			pNode->GetPosition(vec);
			return vec.x;
		}
		return -100;
	}

	int Crane::SetLocation( float x )
	{
		char buf[512];
		wchar_t * t = ToWcharArray(m_szBodyName);
		WideToMulti(buf, t, CP_ACP);
		std::string szBodyName = std::string(buf);
		delete[] t;	

		float y = 0.0f;
		float z = 0.0f;
		UBaseModel* pModel = UDB::GetBaseModel(hMainWnd);	
		if( pModel != NULL)
		{
			USceneNodeManager* pMgr = pModel->GetSecneManager();
			USceneNode * pNode = pMgr->FindSceneNode(szBodyName);
			UnE::Math::Vector3 vec;
			pNode->GetPosition(vec);

			UBaseView* pView = GetBaseView();
			pView->MoveTextPOI2(m_nTextID, x, vec.y + 2.5f, vec.z);

			SetLocation( x, vec.y, vec.z); 

			
		}
		return 0;
	}

	void Crane::SetLocation( float x, float y, float z )
	{
		char buf[4096];
		wchar_t * t = ToWcharArray(m_szBodyName);
		WideToMulti(buf, t, CP_ACP);
		std::string szBodyName = std::string(buf);
		delete[] t;	

		wchar_t * t2 = ToWcharArray(m_szLineName);
		WideToMulti(buf, t2, CP_ACP);
		std::string szLineName = std::string(buf);
		delete[] t2;	

		wchar_t * t3 = ToWcharArray(m_szPinName);
		WideToMulti(buf, t3, CP_ACP);
		std::string szPinName = std::string(buf);
		delete[] t3;	

		UBaseModel* pModel = UDB::GetBaseModel(hMainWnd);	
		if( pModel != NULL)
		{
			USceneNodeManager* pMgr = pModel->GetSecneManager();
			USceneNode * pNode = pMgr->FindSceneNode(szBodyName);

			//pNode->ShowBoundingBox(true);

			UnE::Math::Vector3 vec;
			pNode->GetPosition(vec);
			y = vec.y;
			z = vec.z;
			pNode->SetPosition(UnE::Math::Vector3(x, y, z));

			USceneNode * pNode2 = pMgr->FindSceneNode(szLineName);
			//pNode2->ShowBoundingBox(true);
			pNode2->GetPosition(vec);
			y = vec.y;
			z = vec.z;
			pNode2->SetPosition(UnE::Math::Vector3(x, y, z));

			USceneNode * pNode3 = pMgr->FindSceneNode(szPinName);
			pNode3->GetPosition(vec);
			y = vec.y;
			z = vec.z;
			pNode3->SetPosition(UnE::Math::Vector3(x, y, z));	

			pNode->UpdateBound();
			//pNode3->ShowBoundingBox(true);
		}		
	}


	bool Crane::Select()
	{
		return false;
	}

	void Crane::ClearSelect()
	{
	}

	void Crane::Delete()
	{
	}

	void Crane::OnVisible( bool bVisible )
	{
		m_bVisible = bVisible;

		SetLOD(m_nLOD);
	}

	void Crane::SetLOD( int nLOD )
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

		wchar_t * t2 = ToWcharArray(m_szLineName);
		WideToMulti(buf, t2, CP_ACP);
		std::string szLineName = std::string(buf);
		delete[] t2;	

		wchar_t * t3 = ToWcharArray(m_szPinName);
		WideToMulti(buf, t3, CP_ACP);
		std::string szPinName = std::string(buf);
		delete[] t3;	

		USceneNode * pNode = pMgr->FindSceneNode(szBodyName);
		USceneNode * pNode2 = pMgr->FindSceneNode(szLineName);
		USceneNode * pNode3 = pMgr->FindSceneNode(szPinName);
			

		switch( nLOD )
		{
		case 3:
			//pView->ShowIconPOI(m_nIconID, false);
			pView->ShowTextPOI2(m_nTextID, false);
			pNode->SetVisible(false);
			pNode2->SetVisible(false);
			pNode3->SetVisible(false);
			break;
		case 2:
			//pView->ShowIconPOI(m_nIconID, m_bVisible);
			pView->ShowTextPOI2(m_nTextID, m_bVisible);	
			pNode->SetVisible(m_bVisible);
			pNode2->SetVisible(m_bVisible);
			pNode3->SetVisible(m_bVisible);
			break;
		case 1:	
			//pView->ShowIconPOI(m_nIconID, m_bVisible);
			pView->ShowTextPOI2(m_nTextID, false);
			pNode->SetVisible(m_bVisible);
			pNode2->SetVisible(m_bVisible);
			pNode3->SetVisible(m_bVisible);
			break;
		}	
		pView->RenderOneFrame();	
	}

	System::Collections::ArrayList^ Crane::GetBound()
	{
		System::Collections::ArrayList^ arBound = gcnew System::Collections::ArrayList();

		char buf[4096];
		wchar_t * t = ToWcharArray(m_szBodyName);
		WideToMulti(buf, t, CP_ACP);
		std::string szBodyName = std::string(buf);
		delete[] t;

		wchar_t * t2 = ToWcharArray(m_szLineName);
		WideToMulti(buf, t2, CP_ACP);
		std::string szLineName = std::string(buf);
		delete[] t2;

		wchar_t * t3 = ToWcharArray(m_szPinName);
		WideToMulti(buf, t3, CP_ACP);
		std::string szPinName = std::string(buf);
		delete[] t3;

		float y = 0.0f;
		float y2 = 0.0f;
		UBaseModel* pModel = UDB::GetBaseModel(hMainWnd);
		if (pModel != NULL)
		{
			USceneNodeManager* pMgr = pModel->GetSecneManager();
			USceneNode * pNode = pMgr->FindSceneNode(szBodyName);


			USceneNode * pNode2 = pMgr->FindSceneNode(szLineName);
			
			USceneNode * pNode3 = pMgr->FindSceneNode(szPinName);
			

			UnE::Math::AxisAlignedBox aabb;
			pNode->GetBoundBox(aabb);

			UnE::Math::AxisAlignedBox aabb2;
			pNode2->GetBoundBox(aabb2);
			
			UnE::Math::AxisAlignedBox aabb3;
			pNode3->GetBoundBox(aabb3);

			aabb.merge(aabb2);
			aabb.merge(aabb3);

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


	// CraneManager
	//////////////////////////////////////////////////////////////////////////
	Crane^ CraneManager::GetCrane( int idx )
	{
		if( idx == 0)
			return m_Crane1;
		if( idx == 1)
			return m_Crane2;
		return nullptr;
	}

	CraneManager::CraneManager()
	{
		CreateCrane();
	}

	CraneManager::~CraneManager()
	{
	}

	void CraneManager::CreateCrane()
	{
		UnE::Core::UBaseView * pView = UDB::GetBaseView(hMainWnd);
		if( pView != NULL)
		{
			char buf[512];
			sprintf(buf, "%d#mesh_44#134", hMainWnd);
			std::string szBodyName1 = std::string(buf);
			std::string szBodyName = pView->CloneSceneNode(szBodyName1, 0.0f, 0.0f, 0.0f);

			sprintf(buf, "%d#mesh_62#188", hMainWnd);
			std::string szLineName1 = std::string(buf);
			std::string szLineName = pView->CloneSceneNode(szLineName1, 0.0f,  0.0f,0.0f);


			sprintf(buf, "%d#mesh_51#155", hMainWnd);
			std::string szPinName1 = std::string(buf);
			std::string szPinName = pView->CloneSceneNode(szPinName1, 0.0f,  0.0f,0.0f);

			std::wstring szC1 = MultiToWide(szBodyName1.c_str(), szBodyName1.size(), CP_ACP);
			System::String^ szCraBodyName = ToSystemString((wchar_t*)szC1.c_str());

			std::wstring szC2 = MultiToWide(szLineName1.c_str(), szLineName1.size(), CP_ACP);
			System::String^ szCraLineName = ToSystemString((wchar_t*)szC2.c_str());

			std::wstring szC3 = MultiToWide(szPinName1.c_str(), szPinName1.size(), CP_ACP);
			System::String^ szCraPinName = ToSystemString((wchar_t*)szC3.c_str());

			std::wstring szC4 = MultiToWide(szBodyName.c_str(), szBodyName.size(), CP_ACP);
			System::String^ szCraBodyName2 = ToSystemString((wchar_t*)szC4.c_str());

			std::wstring szC5 = MultiToWide(szLineName.c_str(), szLineName.size(), CP_ACP);
			System::String^ szCraLineName2 = ToSystemString((wchar_t*)szC5.c_str());

			std::wstring szC6 = MultiToWide(szPinName.c_str(), szPinName.size(), CP_ACP);
			System::String^ szCraPinName2 = ToSystemString((wchar_t*)szC6.c_str());

			UBaseModel* pModel = UDB::GetBaseModel(hMainWnd);	
			USceneNodeManager* pMgr = pModel->GetSecneManager();
			USceneNode * pNode = pMgr->FindSceneNode(szBodyName1);

			UnE::Math::Vector3 vec;
			pNode->GetPosition(vec);
			float y = vec.y;

			m_Crane1 = gcnew Crane(0);
			m_Crane1->CreateCrane(szCraBodyName, szCraLineName, szCraPinName);
			
			m_Crane2 = gcnew Crane(1);
			m_Crane2->CreateCrane(szCraBodyName2, szCraLineName2, szCraPinName2);
			m_Crane2->SetInitLocation(50.0f, y, -11.0f);
			
		}

	}
}
