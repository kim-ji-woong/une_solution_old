
#pragma once

using namespace System;
//using namespace Poly2Tri;

namespace Core
{
	ref class Position3D;
	ref class BaseView;
	ref class ZoneVolumeManager;

	public ref class ZonePolygon
	{
	protected:
		System::Collections::ArrayList^ m_arVertexList;
		int m_nID;
		BaseView^ m_View;
		System::String^ m_szName;
		float m_fHeight;
	public:
		ZonePolygon(BaseView^ view);
		virtual ~ZonePolygon(void);

		property float Height
		{
			float get() { return m_fHeight ; }
			void set(float value) { m_fHeight = value; }
		}

		property Core::BaseView^ ParentView
		{
			Core::BaseView^ get() { return m_View ; }
		}

		property System::String^ Name
		{
			System::String^ get() { return m_szName ; }
		}

		void ShowZonePolygon(bool bShow);
		
		void AddVertex(Position3D^ vert);
		void AddVertex( float x, float y, float z );
		Position3D^ GetVertex(int idx);

		Position3D^ GetCentroid();

		void CreatePolygon();
		void UpdatePolygon();

		
	};

	public ref class ZoneVolume
	{
	protected:
		int m_nPoiID;
		int m_nTextID;
		int m_nID;
		BaseView^ m_View;
		System::String^ m_szName;
		float m_fHeight;
		ZonePolygon^ m_Polygon;
		ZoneVolumeManager^ m_Parent;
		bool m_bShow;
	public:

		ZoneVolume(BaseView^ view, ZonePolygon^ polygon, float fHeight, System::String^ szName);
		ZoneVolume( BaseView^ view, ZonePolygon^ polygon, float fHeight , bool bIcon);
		ZoneVolume(BaseView^ view, ZonePolygon^ polygon, float fHeight, bool bIcon, System::Drawing::Color color);
		virtual ~ZoneVolume(void);

		property float Height
		{
			float get() { return m_fHeight ; }
		}

		property int ID
		{
			int get() { return m_nID ; }
		}

		property Core::BaseView^ ParentView
		{
			Core::BaseView^ get() { return m_View ; }
		}

		property Core::ZoneVolumeManager^ Parent
		{
			Core::ZoneVolumeManager^ get() { return m_Parent ; }
			void set(Core::ZoneVolumeManager^ val) { m_Parent = val; }
		}

		property System::String^ Name
		{
			System::String^ get() { return m_szName ; }
		}
		bool GetVisible();
		void SetVisible(bool bShow);

	};


	public ref class ZoneVolumeManager
	{
	protected:
		Core::BaseView^ m_View;
		System::Collections::ArrayList^ m_ChildList;
		System::Collections::Generic::SortedList< int, System::String^ >^ m_mapChild;
	public:
		property System::Collections::ArrayList^  Childs
		{
			System::Collections::ArrayList^ get() { return m_ChildList; }
		}

		property Core::BaseView^ ParentView
		{
			Core::BaseView^ get() { return m_View ; }
		}

		ZoneVolumeManager(BaseView^ view);

		void SetVisibleAll(bool bShow);
		
		ZoneVolume^ FindZoneVolume(System::String^ szName);

		ZoneVolume^ FindZoneVolume(int nID);
		ZoneVolume^ CreateZoneVolume(BaseView^ view, ZonePolygon^ polygon, float fHeight, System::String^ szName, System::String^ szText);
		ZoneVolume^ CreateZoneVolume(BaseView^ view, ZonePolygon^ polygon, float fHeight, System::String^ szName, bool bIcon);
		ZoneVolume^ CreateZoneVolume(BaseView^ view, ZonePolygon^ polygon, float fHeight, System::String^ szName);

		ZoneVolume^ CreateZoneVolume(BaseView^ view, ZonePolygon^ polygon, float fHeight, System::String^ szName, bool bIcon, System::Drawing::Color color);
		void ClearAll();
	};

};

