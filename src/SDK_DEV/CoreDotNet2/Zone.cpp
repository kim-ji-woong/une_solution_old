
#include "StdAfx.h"
#include "Zone.h"
#include "BaseView.h"

#include "UDB.h"
#include "UBaseDriver.h"
#include "UBaseView.h"
#include "UObject.h"
#include "UPolygon.h"

using namespace System;
using namespace System::Collections;
using namespace System::Collections::Generic;
using namespace System::Drawing;
using namespace Core;


extern System::String^ ToSystemString(wchar_t* str);
extern wchar_t* ToWcharArray(System::String^ str);
extern int WideToMulti(char* pszDst, const wchar_t* pwzIn, UINT uCodepage);
namespace Core
{

	UnE::Core::UPolygon * GetPolygon(System::String^ szName, int nhwnd)
	{
		UnE::Core::UObjectManager * pObjManager = UnE::Core::UDB::GetObjectManger((HWND)nhwnd);
		if (pObjManager != NULL)
		{
			//USES_CONVERSION;
			char buf[2048];
			wchar_t * t = ToWcharArray(szName);
			WideToMulti(buf, t, CP_ACP);
			std::string szOrName = std::string(buf);
			delete[] t;

			UnE::Core::UPolygon * pPolygon = (UnE::Core::UPolygon*)pObjManager->GetUObject(szOrName);
			if (pPolygon != NULL)
			{
				return pPolygon;
			}
		}
		return NULL;
	}

	ZonePolygon::ZonePolygon(BaseView^ view)
	{
		m_arVertexList = gcnew System::Collections::ArrayList();
		m_nID = -1;
		m_View = view;
		m_szName = gcnew System::String("");
	}

	ZonePolygon::~ZonePolygon(void)
	{
	}

	void ZonePolygon::ShowZonePolygon(bool bShow)
	{
		if (m_View != nullptr && m_nID != -1)
		{
			int mHWND = m_View->WindowHandle;
			UnE::Core::UPolygon * pPolygon = GetPolygon(m_szName, mHWND);
			if (pPolygon != NULL)
			{
				pPolygon->SetVisible(bShow);
			}
		}
	}

	void ZonePolygon::AddVertex(Position3D^ vert)
	{
		if (m_arVertexList->Count > 0)
		{
			Position3D^ pos = (Position3D^)m_arVertexList[0];
			if (pos->Equals(vert))
				return;
		}
		m_arVertexList->Add(vert);
	}

	void ZonePolygon::AddVertex(float x, float y, float z)
	{
		Position3D^ vert = gcnew Position3D(x, y, z);
		if (m_arVertexList->Count > 0)
		{
			Position3D^ pos = (Position3D^)m_arVertexList[0];
			if (pos->Equals(vert))
				return;
		}
		m_arVertexList->Add(vert);
	}

	Position3D^ ZonePolygon::GetVertex(int idx)
	{
		if (idx < 0 || idx >= m_arVertexList->Count)
			return nullptr;

		return (Position3D^)m_arVertexList[idx];
	}

	void ZonePolygon::UpdatePolygon()
	{
		int mHWND = m_View->WindowHandle;

		UnE::Core::UPolygon * pPolygon = GetPolygon(m_szName, mHWND);
		if (pPolygon != NULL)
		{
			int nCount = m_arVertexList->Count;
			pPolygon->clear();
			for (int i = 0; i < nCount; i++)
			{
				Position3D^ pos = (Position3D^)m_arVertexList[i];
				pPolygon->addPoint(pos->X, pos->Y, pos->Z);
			}
			pPolygon->update();
		}
	}

	void ZonePolygon::CreatePolygon()
	{
		int mHWND = m_View->WindowHandle;
		UnE::Core::UBaseView * pView = UnE::Core::UDB::GetBaseView(mHWND);
		if (pView != NULL)
		{
			UnE::Core::UPolygon * pUPolygon = new UnE::Core::UPolygon(pView);
			pUPolygon->SetHeight(m_fHeight);
			m_nID = pUPolygon->GetID();
			std::string szName = pUPolygon->GetName();
			USES_CONVERSION;
			m_szName = ToSystemString(A2W(szName.c_str()));

			int nCount = m_arVertexList->Count;
			for (int i = 0; i < nCount; i++)
			{
				Position3D^ pos = (Position3D^)m_arVertexList[i];
				pUPolygon->addPoint(pos->X, pos->Y, pos->Z);
			}

			//pUPolygon->update();
		}
	}

	Position3D^ ZonePolygon::GetCentroid()
	{
		Position3D^ result = gcnew Position3D(0.0f, 0.0f, 0.0f);
		int nCount = m_arVertexList->Count;
		if (nCount == 0)
			return result;

		for (int i = 0; i < nCount; i++)
		{
			Position3D^ pos = (Position3D^)m_arVertexList[i];
			result->X += pos->X;
			result->Y += pos->Y;
			result->Z += pos->Z;
		}
		result->X /= nCount;
		result->Y /= nCount;
		result->Z /= nCount;
		return result;
	}

	//////////////////////////////////////////////////////////////////////////
	// Zone Volume

	UnE::Core::USpaceVolume * GetSpaceVolume(System::String^ szName, int nhwnd)
	{
		UnE::Core::UObjectManager * pObjManager = UnE::Core::UDB::GetObjectManger((HWND)nhwnd);
		if (pObjManager != NULL)
		{
			//USES_CONVERSION;
			//wchar_t * t = ToWcharArray(szName);
			//std::string szOrName = std::string(W2A(t));

			char buf[2048];
			wchar_t * t = ToWcharArray(szName);
			WideToMulti(buf, t, CP_ACP);
			std::string szOrName = std::string(buf);
			delete[] t;

			UnE::Core::USpaceVolume * pPolygon = (UnE::Core::USpaceVolume*)pObjManager->GetUObject(szOrName);
			if (pPolygon != NULL)
			{
				return pPolygon;
			}
		}
		return NULL;
	}
	ZoneVolume::ZoneVolume(BaseView^ view, ZonePolygon^ polygon, float fHeight, System::String^ szText)
	{
		m_bShow = false;
		m_Parent = nullptr;
		m_View = view;
		m_Polygon = polygon;
		m_fHeight = fHeight;
		m_nPoiID = -1;
		m_nTextID = -1;
		int mHWND = m_View->WindowHandle;
		UnE::Core::UBaseView * pView = UnE::Core::UDB::GetBaseView(mHWND);
		UnE::Core::UPolygon * pPolygon = GetPolygon(polygon->Name, mHWND);
		if (pView != NULL && pPolygon != NULL)
		{
			UnE::Core::USpaceVolume * pVolume = new UnE::Core::USpaceVolume(pView);

			m_nID = pVolume->GetID();
			std::string szName = pVolume->GetName();
			USES_CONVERSION;
			m_szName = ToSystemString(A2W(szName.c_str()));


			try
			{
				pVolume->CreateVolume(pPolygon, pPolygon->GetHeight(), fHeight);
				Position3D^ pos = polygon->GetCentroid();
				pos->Y = fHeight + 1.0f;

				if (szText != nullptr)
				{
					char buf[4096];
					wchar_t * t = ToWcharArray(szText);
					WideToMulti(buf, t, CP_ACP);
					std::string szTextPath = std::string(buf);
					delete[] t;

					m_nTextID = pView->AddTextPOI2(szTextPath, pos->X, pos->Y, pos->Z, false);
					pView->MoveTextPOI2(m_nTextID, pos->X, pos->Y, pos->Z);
				}
				else
				{
					m_nTextID = -1;
				}
			}
			catch (System::AccessViolationException^ e)
			{
			}
		}
	}
	ZoneVolume::ZoneVolume(BaseView^ view, ZonePolygon^ polygon, float fHeight, bool bIcon, System::Drawing::Color color)
	{
		m_bShow = false;
		m_Parent = nullptr;
		m_View = view;
		m_Polygon = polygon;
		m_fHeight = fHeight;
		m_nPoiID = -1;
		m_nTextID = -1;
		int mHWND = m_View->WindowHandle;
		UnE::Core::UBaseView * pView = UnE::Core::UDB::GetBaseView(mHWND);
		UnE::Core::UPolygon * pPolygon = GetPolygon(polygon->Name, mHWND);
		if (pView != NULL && pPolygon != NULL)
		{
			UnE::Core::USpaceVolume * pVolume = new UnE::Core::USpaceVolume(pView);
			pVolume->SetColor(color.R / 255, color.G / 255, color.B / 255);
			m_nID = pVolume->GetID();
			std::string szName = pVolume->GetName();
			USES_CONVERSION;
			m_szName = ToSystemString(A2W(szName.c_str()));


			try
			{
				pVolume->CreateVolume(pPolygon, pPolygon->GetHeight(), fHeight );
				Position3D^ pos = polygon->GetCentroid();
				pos->Y = fHeight + 1.0f;

				m_nPoiID = -1;

			}
			catch (System::AccessViolationException^ e)
			{
			}

		}
	}
	ZoneVolume::ZoneVolume(BaseView^ view, ZonePolygon^ polygon, float fHeight, bool bIcon)
	{
		m_bShow = false;
		m_Parent = nullptr;
		m_View = view;
		m_Polygon = polygon;
		m_fHeight = fHeight;
		m_nPoiID = -1;
		m_nTextID = -1;
		int mHWND = m_View->WindowHandle;
		UnE::Core::UBaseView * pView = UnE::Core::UDB::GetBaseView(mHWND);
		UnE::Core::UPolygon * pPolygon = GetPolygon(polygon->Name, mHWND);
		if (pView != NULL && pPolygon != NULL)
		{
			UnE::Core::USpaceVolume * pVolume = new UnE::Core::USpaceVolume(pView);

			m_nID = pVolume->GetID();
			std::string szName = pVolume->GetName();
			USES_CONVERSION;
			m_szName = ToSystemString(A2W(szName.c_str()));


			try
			{
				pVolume->CreateVolume(pPolygon, pPolygon->GetHeight(), fHeight);
				Position3D^ pos = polygon->GetCentroid();
				pos->Y = fHeight + 1.0f;

				if (bIcon == true)
				{
					std::string szPath = UnE::Core::UBaseDriver::Instance().GetEngineWorkDir();
					szPath = szPath + "Media\\icons\\È­Àç.ico";

					m_nPoiID = pView->AddIconPOI(szPath, pos->X, pos->Y, pos->Z, 64.0f, 64.0f, false);
				}
				else
				{
					m_nPoiID = -1;
				}

			}
			catch (System::AccessViolationException^ e)
			{
			}

		}
	}

	ZoneVolume::~ZoneVolume(void)
	{

	}
	bool ZoneVolume::GetVisible()
	{
		return m_bShow;
	}
	void ZoneVolume::SetVisible(bool bShow)
	{
		m_bShow = bShow;
		if (m_View != nullptr && m_szName != nullptr)
		{
			int mHWND = m_View->WindowHandle;
			UnE::Core::USpaceVolume * pVolume = GetSpaceVolume(m_szName, mHWND);
			if (pVolume != NULL)
			{
				pVolume->SetVisible(bShow);
			}


			UnE::Core::UBaseView * pView = UnE::Core::UDB::GetBaseView(mHWND);
			if (pView != NULL && m_nPoiID != -1)
			{
				pView->ShowIconPOI(m_nPoiID, bShow);
			}
			if (pView != NULL && m_nTextID != -1)
			{
				pView->ShowTextPOI2(m_nTextID, bShow);
			}


		}
	}

	//////////////////////////////////////////////////////////////////////////
	// ZoneVolume Manager

	ZoneVolumeManager::ZoneVolumeManager(BaseView^ view)
	{
		m_View = view;

		m_ChildList = gcnew System::Collections::ArrayList();

		m_mapChild = gcnew System::Collections::Generic::SortedList<int, System::String^ >();
	}

	void ZoneVolumeManager::SetVisibleAll(bool bShow)
	{
		int nCount = Childs->Count;
		for (int i = 0; i < nCount; i++)
		{
			ZoneVolume^ zone = (ZoneVolume^)Childs[i];
			zone->SetVisible(bShow);
		}
	}

	ZoneVolume^ ZoneVolumeManager::FindZoneVolume(System::String^ szName)
	{
		if (m_mapChild->ContainsValue(szName))
		{
			for each(System::Collections::Generic::KeyValuePair<int, System::String^> pair in m_mapChild)
			{
				if (pair.Value == szName)
				{
					return FindZoneVolume(pair.Key);
				}

			}
		}
		return nullptr;
	}

	ZoneVolume^ ZoneVolumeManager::FindZoneVolume(int nID)
	{
		int nCount = Childs->Count;
		for (int i = 0; i < nCount; i++)
		{
			ZoneVolume^ zone = (ZoneVolume^)Childs[i];
			if (zone->ID == nID)
			{
				return zone;
			}
		}
		return nullptr;
	}
	ZoneVolume^ ZoneVolumeManager::CreateZoneVolume(BaseView^ view, ZonePolygon^ polygon, float fHeight, System::String^ szName, System::String^ szText)
	{
		if (!m_mapChild->ContainsValue(szName))
		{
			ZoneVolume^ zoneVolume = gcnew ZoneVolume(view, polygon, fHeight, szText);
			//zoneVolume->Name = szName;
			zoneVolume->Parent = this;
			m_ChildList->Add(zoneVolume);

			m_mapChild->Add(zoneVolume->ID, szName);

			return zoneVolume;
		}
		return nullptr;
	}

	ZoneVolume^ ZoneVolumeManager::CreateZoneVolume(BaseView^ view, ZonePolygon^ polygon, float fHeight, System::String^ szName, bool bIcon)
	{
		if (!m_mapChild->ContainsValue(szName))
		{
			ZoneVolume^ zoneVolume = gcnew ZoneVolume(view, polygon, fHeight, bIcon);
			zoneVolume->Parent = this;
			//zoneVolume->Name = szName;
			m_ChildList->Add(zoneVolume);

			m_mapChild->Add(zoneVolume->ID, szName);

			return zoneVolume;
		}
		return nullptr;
	}

	ZoneVolume^ ZoneVolumeManager::CreateZoneVolume(BaseView^ view, ZonePolygon^ polygon, float fHeight, System::String^ szName, bool bIcon, System::Drawing::Color color)
	{
		if (!m_mapChild->ContainsValue(szName))
		{
			ZoneVolume^ zoneVolume = gcnew ZoneVolume(view, polygon, fHeight, bIcon, color);
			zoneVolume->Parent = this;
			//zoneVolume->Name = szName;
			m_ChildList->Add(zoneVolume);

			m_mapChild->Add(zoneVolume->ID, szName);

			return zoneVolume;
		}
		return nullptr;
	}

	ZoneVolume^ ZoneVolumeManager::CreateZoneVolume(BaseView^ view, ZonePolygon^ polygon, float fHeight, System::String^ szName)
	{
		if (!m_mapChild->ContainsValue(szName))
		{
			ZoneVolume^ zoneVolume = gcnew ZoneVolume(view, polygon, fHeight, true);
			zoneVolume->Parent = this;
			//zoneVolume->Name = szName;
			m_ChildList->Add(zoneVolume);

			m_mapChild->Add(zoneVolume->ID, szName);

			return zoneVolume;
		}
		return nullptr;
	}


	void ZoneVolumeManager::ClearAll()
	{
		m_mapChild->Clear();
	}


}
