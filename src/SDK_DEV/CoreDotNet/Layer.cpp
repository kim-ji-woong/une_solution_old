#include "StdAfx.h"
#include "Layer.h"
#include "BaseView.h"

namespace Core
{
	Layer::Layer(int id, bool bText)
	{
		m_nID = id;
		m_ObjList = gcnew System::Collections::ArrayList();
		m_Parent = nullptr;
		m_bText = bText;
		if( bText == true)
			m_nType = 2;
		else
			m_nType = 1;
	}	

	Layer::Layer(int id, bool bText, float a, float b)
	{
		m_nID = id;
		m_ObjList = gcnew System::Collections::ArrayList();
		m_Parent = nullptr;
		m_bText = bText;
		
		m_nType = 3;

		m_nShortDist = a;
		m_nLongDist = b;
	}
	
	void Layer::Add( int nObjID )
	{
		if(!m_ObjList->Contains(nObjID))
			m_ObjList->Add(nObjID);		
	}

	void Layer::Remove( int nObjID )
	{
		if(m_ObjList->Contains(nObjID))
			m_ObjList->Remove(nObjID);		
	}

	void Layer::SetVisible( bool bShow )
	{
		if( Parent != nullptr)
		{
			int nCount  = m_ObjList->Count;
			for( int i = 0; i < nCount; i++)
			{
				int id = (int)m_ObjList[i];	
				if( m_nType == 2)
					m_Parent->ParentView->ShowNames(id, bShow);
				else if (m_nType == 1)
					m_Parent->ParentView->ShowIconPOI(id, bShow);
				else if (m_nType == 3)
				{
					if (bShow == true)
						m_Parent->ParentView->SetTextPOILOD(id, bShow, m_nShortDist);
					else
						m_Parent->ParentView->SetTextPOILOD(id, bShow, m_nLongDist);

				}
			}			
		}
	}

	void Layer::SetLOD( int nLevel )
	{
		m_nLod = nLevel;
	}

	//////////////////////////////////////////////////////////////////////////
	LayerManager::LayerManager(UnE::View::Content::IBaseView^ view)
	{
		view->LayerManager = this;
		m_View = view;
		this->m_LayerList = gcnew System::Collections::ArrayList();

	}

	void LayerManager::AddLayer( Layer^ layer )
	{
		if( !m_LayerList->Contains( layer ))
		{
			layer->Parent = this;
			m_LayerList->Add(layer);
		}
	}

	void LayerManager::AddLayer( int nID , bool bText)
	{
		int nCount  = m_LayerList->Count;
		for( int i = 0; i < nCount; i++)
		{
			Layer^ node = (Layer^)m_LayerList[i];	
			if( node->ID == nID)
				return;
		}

		Layer^ layer = gcnew Layer(nID, bText);
		layer->Parent = this;
		m_LayerList->Add(layer);
	}

	void LayerManager::AddLayer(int nID, bool bText, float nHideLODDist, float nShowLODDist)
	{
		int nCount = m_LayerList->Count;
		for (int i = 0; i < nCount; i++)
		{
			Layer^ node = (Layer^)m_LayerList[i];
			if (node->ID == nID)
				return;
		}

		Layer^ layer = gcnew Layer(nID, bText, nHideLODDist, nShowLODDist);
		layer->Parent = this;
		m_LayerList->Add(layer);
	}

	void LayerManager::RemoveLayer( Layer^ layer )
	{
		if( layer != nullptr)
			m_LayerList->Remove(layer);
	}

	void LayerManager::RemoveLayer( int nID )
	{
		UnE::View::Content::ILayer^ layer = GetLayer(nID);
		if( layer != nullptr)
			m_LayerList->Remove(layer);
	}

	void LayerManager::ShowAllLayer()
	{
		int nCount  = m_LayerList->Count;
		for( int i = 0; i < nCount; i++)
		{
			Layer^ node = (Layer^)m_LayerList[i];	
			node->SetVisible(true);
			
		}
	}

	void LayerManager::ShowLayer( int nID )
	{
		UnE::View::Content::ILayer^ layer = GetLayer(nID);
		if( layer != nullptr)
			layer->SetVisible(true);
	}

	void LayerManager::HideAllLayer()
	{
		int nCount  = m_LayerList->Count;
		for( int i = 0; i < nCount; i++)
		{
			Layer^ node = (Layer^)m_LayerList[i];	
			node->SetVisible(false);
		}
	}

	void LayerManager::HideLayer( int nID )
	{
		UnE::View::Content::ILayer^ layer = GetLayer(nID);
		if( layer != nullptr)
			layer->SetVisible(false);
	}

	void LayerManager::RemoveLayerChild( int nObjID )
	{
		int nCount  = m_LayerList->Count;
		for( int i = 0; i < nCount; i++)
		{
			Layer^ node = (Layer^)m_LayerList[i];	
			node->Objects->Remove(nObjID);			
		}
	}

	UnE::View::Content::ILayer^ LayerManager::GetLayer(int nID)
	{
		int nCount  = m_LayerList->Count;
		for( int i = 0; i < nCount; i++)
		{
			Layer^ node = (Layer^)m_LayerList[i];	
			if( node->ID == nID)
				return node;
		}
		return nullptr;
	}

}
