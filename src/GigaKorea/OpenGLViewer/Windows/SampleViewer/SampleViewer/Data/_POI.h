#pragma once
#include <vector>
#include <string>
#include "Vertex2D.h"

namespace VectorGraphics
{
	class POIIcon;
}

namespace FireSafetyManager
{
	class POIType
	{
	public:
		POIType();
		POIType(int nID, const std::wstring& strName, const std::wstring& strCode, VectorGraphics::POIIcon* pIcon);
		virtual ~POIType();

	public:
		int GetID();
		const std::wstring& GetName();
		const std::wstring& GetCode();
		VectorGraphics::POIIcon* GetIcon();
		bool LoadPOIIcon(const std::string& strPath);

	public:
		static VectorGraphics::POIIcon* GetDefaultIcon();

	private:
		static void MakeDefaultIcon();

	private:
		int m_nID;
		std::wstring m_strTypeName;
		std::wstring m_strCode;
		VectorGraphics::POIIcon* m_pIcon;

	private:
		static VectorGraphics::POIIcon* m_pDefaultIcon;
	};

	class POI
	{
	public:
		POI();
		// dAngle : Degree
		POI(int nID, const std::wstring& strName, const VectorGraphics::Vertex2D& vPos, double dHeight, double dAngle, POIType* pType);

	public:
		int GetID();
		const std::wstring& GetName();
		const VectorGraphics::Vertex2D& GetPosition();
		double GetHeight();
		POIType* GetPOIType();
		// Degree
		double GetAngle();
		
	private:
		int m_nID;
		POIType* m_pType;
		std::wstring m_strName;
		VectorGraphics::Vertex2D m_vPos;
		double m_dHeight;
		// Degree
		double m_dAngle;
	};
}
