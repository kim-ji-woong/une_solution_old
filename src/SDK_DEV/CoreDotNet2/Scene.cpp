#include "StdAfx.h"
#include "Scene.h"

#include <map>
#include <utility>

#include <atlcoll.h>
using namespace ATL;

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

using namespace UnE::Core;


//////////////////////////////////////////////////////////////////////////
// Local function

extern wchar_t* ToWcharArray(System::String^ str);


extern System::String^ ToSystemString(wchar_t* str);



namespace Core
{
	void ShowBoundingBox(Scene^ parent, bool bShow)
	{
		int nCount  = parent->Childs->Count;
		for( int i = 0; i < nCount; i++)
		{
			Scene^ node = (Scene^)parent->Childs[i];	
			ShowBoundingBox(node, bShow);
			node->ShowBoundingBox(bShow);
		}		
	}

	void ShowNode(Scene^ parent, bool bShow)
	{
		int nCount  = parent->Childs->Count;
		for( int i = 0; i < nCount; i++)
		{
			Scene^ node = (Scene^)parent->Childs[i];	
			ShowNode(node, bShow);
			node->SetVisible(bShow);
		}		
	}

	void AddNode( SceneManager^ scManager, UnE::Core::USceneNode* pNode, Scene^ parent, System::Collections::ArrayList^ arNodeList)
	{
		USES_CONVERSION;

		bool bVisible = pNode->GetVisible();
		std::string szName = pNode->SceneName();
		std::string szAName = pNode->GetAliasName();
		
		Scene^ scene = gcnew Scene();
		scene->Tag((int)pNode);
		scene->Name = ToSystemString(A2W(szName.c_str()));
		scene->AliasName = ToSystemString(A2W(szAName.c_str()));
		scene->Visible = bVisible;
		scene->Parent = parent;
		
		if( parent != nullptr)
			parent->AddChild(scene);


		scene->CoreSceneManager = scManager;
		arNodeList->Add(scene);

		int nCount  = pNode->GetNumChilds();
		for( int i = 0; i < nCount; i++)
		{
			UnE::Core::USceneNode* pUNode = (UnE::Core::USceneNode*)pNode->ChildAt(i);	
			AddNode(scManager, pUNode, scene, arNodeList);
		}
		
	}

	//////////////////////////////////////////////////////////////////////////
	// Scene
	Scene::Scene(void)
	{
		m_ChildList = gcnew System::Collections::ArrayList();
	}

	void Scene::AddChild( Scene^ child )
	{
		m_ChildList->Add(child);
	}

	void Scene::RemoveChild( Scene^ child )
	{
		m_ChildList->Remove(child);
	}

	void Scene::ShowBoundingBox( bool bShow )
	{
		m_bShowBounds = bShow;
		UnE::Core::USceneNode* pNode = ( UnE::Core::USceneNode*)m_Tag;
		if( pNode != NULL)
			pNode->ShowBoundingBox(bShow);		 
	}

	void Scene::Zoom(bool bRedraw)
	{
		CoreSceneManager->ParentView->ZoomObject(Name);
		CoreSceneManager->ParentView->UpdateWindow();
	}

	void Scene::SetVisible( bool bShow )
	{
		m_bVisible = bShow;
		UnE::Core::USceneNode* pNode = ( UnE::Core::USceneNode*)m_Tag;
		if( pNode != NULL)
		{
			if( m_bVisible == true)
			{
				pNode->ShowBoundingBox(m_bShowBounds);
			}
			else
			{
				pNode->ShowBoundingBox(false);
			}
			

			pNode->SetVisible(m_bVisible);

		}
		
	}

	void Scene::ShowBoundingBoxAll( bool bShow )
	{
		int nCount  = Childs->Count;
		for( int i = 0; i < nCount; i++)
		{
			Scene^ node = (Scene^)Childs[i];	
			Core::ShowBoundingBox(node, bShow);
			node->ShowBoundingBox(bShow);
		}
	}

	void Scene::SetVisibleAll( bool bShow )
	{
		int nCount  = Childs->Count;
		for( int i = 0; i < nCount; i++)
		{
			Scene^ node = (Scene^)Childs[i];	
			ShowNode(node, bShow);
			node->SetVisible(bShow);
		}
	}

	Position3D^ Scene::GetPosition()
	{
		UnE::Core::USceneNode * pNode = (UnE::Core::USceneNode*)m_Tag;
		if( pNode != NULL)
		{
			UnE::Math::Vector3 vPos;
			pNode->GetPosition(vPos);
			Position3D^ pos = gcnew Position3D(vPos.x, vPos.y, vPos.z);
			return pos;
		}
		return nullptr;
	}

	void Scene::SetPosition( Position3D^ pos )
	{
		UnE::Core::USceneNode * pNode = (UnE::Core::USceneNode*)m_Tag;
		if( pNode != NULL)
		{
			UnE::Math::Vector3 vPos(pos->X, pos->Y, pos->Z);
			pNode->SetPosition(vPos);			
		}
	}

	bool Scene::IsChildNode( System::String^ szName )
	{
		int nCount  = Childs->Count;
		for( int i = 0; i < nCount; i++)
		{		
			Scene^ node = (Scene^)Childs[i];
			if( node->Name == szName)
				return true;			
		}
		return false;
	}

	Scene^ Scene::GetChild( System::String^ szName )
	{
		int nCount  = Childs->Count;
		for( int i = 0; i < nCount; i++)
		{		
			Scene^ node = (Scene^)Childs[i];
			if( node->Name == szName)
				return node;			
		}
		return nullptr;
	}

	Position3D^ Scene::GetMinimum()
	{
		UnE::Core::USceneNode* pNode = ( UnE::Core::USceneNode*)m_Tag;
		if( pNode != NULL)
		{
			UnE::Math::AxisAlignedBox bounding;
			pNode->GetBoundBox(bounding);
			UnE::Math::Vector3& vec = bounding.getMinimum();
			Position3D^ pos = gcnew Position3D(vec.x, vec.y, vec.z);
			return pos;
		}
		return nullptr;		
	}

	Position3D^ Scene::GetMaximum()
	{
		UnE::Core::USceneNode* pNode = ( UnE::Core::USceneNode*)m_Tag;
		if( pNode != NULL)
		{
			UnE::Math::AxisAlignedBox bounding;
			pNode->GetBoundBox(bounding);
			UnE::Math::Vector3& vec = bounding.getMaximum();
				Position3D^ pos = gcnew Position3D(vec.x, vec.y, vec.z);
			return pos;
		}
		return nullptr;	
	}


	//////////////////////////////////////////////////////////////////////////
	// Scene Manager 
	SceneManager::SceneManager(BaseView^ view)
	{
		m_View = view;

		m_ChildList = gcnew System::Collections::ArrayList();
	}

	void SceneManager::UpdateData()
	{
		int hWnd = m_View->WindowHandle;
		UnE::Core::UBaseModel* pModel = UnE::Core::UDB::GetBaseModel(hWnd);
		if( pModel != NULL)
		{

			UnE::Core::USceneNode * pNode = (UnE::Core::USceneNode*)pModel->GetSecneManager()->GetRootSceneNode();
			if (pNode != NULL)
			{
				m_ChildList->Clear();

				AddNode(this, pNode, nullptr, m_ChildList);
			}			
		}
	}
	
	void SceneManager::ShowBoundingBoxAll( bool bShow )
	{
		int nCount  = Childs->Count;
		for( int i = 0; i < nCount; i++)
		{
			Scene^ node = (Scene^)Childs[i];	
			ShowBoundingBox(node, bShow);
			node->ShowBoundingBox(bShow);
		}
	}

	void SceneManager::SetVisibleAll( bool bShow )
	{
		int nCount  = Childs->Count;
		for( int i = 0; i < nCount; i++)
		{
			Scene^ node = (Scene^)Childs[i];	
			ShowNode(node, bShow);
			node->SetVisible(bShow);
		}
	}

	Scene^ SceneManager::FindSceneNode( System::String^ szName )
	{
		int nCount  = Childs->Count;
		for( int i = 0; i < nCount; i++)
		{			
			Scene^ node = (Scene^)Childs[i];
			if( node->Name == szName)
				return node;			
		}
		return nullptr;
	}

	Scene^ SceneManager::FindSceneNodeByAliasName( System::String^ szName )
	{
		int nCount  = Childs->Count;
		for( int i = 0; i < nCount; i++)
		{			
			Scene^ node = (Scene^)Childs[i];
			if( node->AliasName == szName)
				return node;			
		}
		return nullptr;
	}

}
