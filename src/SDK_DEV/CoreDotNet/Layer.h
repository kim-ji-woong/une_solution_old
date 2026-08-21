#pragma once

#include "BaseView.h"

using namespace System;

namespace Core
{
	ref class LayerManager;
	public ref class Layer : UnE::View::Content::ILayer
	{
	protected:
		Layer(){}
		LayerManager^ m_Parent;

	protected:
		System::Collections::ArrayList^ m_ObjList;
		bool m_bVisible;
		int m_nID;
		bool m_bText;
		int m_nType;

		int m_nLod;

		float m_nShortDist;
		float m_nLongDist;
	public:
		virtual ~Layer(){}
		Layer(int nId, bool bText);
		Layer(int nId, bool bText, float nHideLODDist, float nShowLODDist);

		property int Type
		{
			int get() { return m_nType; }
		}
		
		property bool IsText
		{
			bool get() { return m_bText; }
		}

		property int ID
		{
			int get() { return m_nID; }
		}
		property Core::LayerManager^ Parent
		{
			Core::LayerManager^ get() { return m_Parent ; }
			void set(Core::LayerManager^ val) { m_Parent = val; }
		}

		virtual property System::Collections::ArrayList^  Objects
		{
			System::Collections::ArrayList^ get() { return m_ObjList; }
		}

		virtual void Add(int nObjID);
		virtual void Remove(int nObjID);

		virtual void SetVisible(bool bShow);

		virtual void SetLOD(int nLevel);
	};
	
	public ref class LayerManager : UnE::View::Content::ILayerManager
	{
	private:
		LayerManager(){}
		
	protected:
		UnE::View::Content::IBaseView^ m_View;
		System::Collections::ArrayList^ m_LayerList;

	public:
		property System::Collections::ArrayList^  Layers
		{
			System::Collections::ArrayList^ get() { return m_LayerList; }
		}

		property UnE::View::Content::IBaseView^ ParentView
		{
			UnE::View::Content::IBaseView^ get() { return m_View; }
		}
		
		LayerManager(UnE::View::Content::IBaseView^ view);

		virtual UnE::View::Content::ILayer^ GetLayer(int nID);

		void AddLayer(Layer^ layer);
		void AddLayer(int nID, bool bText);
		void AddLayer(int nID, bool bText, float nHideLODDist, float nShowLODDist);
		void RemoveLayer(Layer^ layer);
		void RemoveLayer(int nID);

		void ShowAllLayer();
		virtual void ShowLayer(int nID);

		void HideAllLayer();
		virtual void HideLayer(int nID);

		virtual void RemoveLayerChild(int nObjID);

	};
}

