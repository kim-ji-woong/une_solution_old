#pragma once

#include "BaseView.h"

using namespace System;

namespace Core
{
	ref class SceneManager;

	public ref class Scene
	{
	protected:
		System::String^ m_szName;
		System::String^ m_szAliasName;
		bool			m_bVisible;
		
		Scene^			m_Parent;
		bool		    m_bShowBounds;
		int				m_Tag;		
		SceneManager^	m_Manager;

		System::Collections::ArrayList^ m_ChildList;
	public:
		Scene(void);
		
		void Tag(int val) { m_Tag = val; }

		property SceneManager^ CoreSceneManager
		{
			SceneManager^ get() { return m_Manager; }
			void set(SceneManager^ val) { m_Manager = val; }
		}

		property System::String^ AliasName
		{
			System::String^ get() { return m_szAliasName; }
			void set(System::String^ val) { m_szAliasName = val; }
		}


		property System::String^ Name
		{
			System::String^ get() { return m_szName; }
			void set(System::String^ val) { m_szName = val; }
		}

		property Scene^ Parent
		{
			Scene^ get() { return m_Parent; }
			void set(Scene^ val) { m_Parent = val; }
		}

		property bool ShowBound
		{
			bool get() { return m_bShowBounds; }
			void set(bool bVal) 
			{
				m_bShowBounds = bVal;
				ShowBoundingBox(m_bShowBounds);
			}
		}

		property bool Visible
		{
			bool get() { return m_bVisible; }
			void set(bool bVal)
			{
				m_bVisible = bVal; 
				SetVisible(m_bVisible);
			}
		}

		property System::Collections::ArrayList^  Childs
		{
			System::Collections::ArrayList^ get() { return m_ChildList; }
		}
		void SetVisible(bool bShow);

		void ShowBoundingBox(bool bShow);
		
		void AddChild(Scene^ child);

		void RemoveChild(Scene^ child);

		void Zoom(bool bRedraw);

		void ShowBoundingBoxAll(bool bShow);

		void SetVisibleAll(bool bShow);

		Position3D^ GetPosition();

		void SetPosition(Position3D^ pos);

		bool IsChildNode(System::String^ szName);

		Scene^ GetChild(System::String^ szName);

		Position3D^ GetMinimum();

		Position3D^ GetMaximum();
	};

	public ref class SceneManager
	{
	protected:
		Core::BaseView^ m_View;
		System::Collections::ArrayList^ m_ChildList;

	public:
		property System::Collections::ArrayList^  Childs
		{
			System::Collections::ArrayList^ get() { return m_ChildList; }
		}

		property Core::BaseView^ ParentView
		{
			Core::BaseView^ get() { return m_View ; }
		}

		
		SceneManager(BaseView^ view);

		void UpdateData();

		void ShowBoundingBoxAll(bool bShow);

		void SetVisibleAll(bool bShow);

		Scene^ FindSceneNode(System::String^ szName);

		Scene^ FindSceneNodeByAliasName(System::String^ szName);
	};
}

