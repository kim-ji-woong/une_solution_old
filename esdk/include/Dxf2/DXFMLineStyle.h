#pragma once
#include "DXFObject.h"

namespace DXF
{
	namespace OBJECTS
	{
		class ObjectManager;

		class MLineStyle : public Object
		{
		public:
			class Element
			{
			public:
				double m_dOffset;
				int m_nColor;
				std::wstring m_strLineType;
			};

		public:
			MLineStyle(wchar_t* strStyleName, ObjectManager* pMgr);
			virtual ~MLineStyle(void);

		public:
			void AddElement(double dOffset, int nColor, wchar_t* strLineType);
			void SetData();
			// nColor : ACI(AutoCAD Color Index)¿¡ µû¸§
			void SetFillColor(int nColor);
			void SetAngle(double dBeginAngle, double dEndAngle);
			wchar_t* GetStyleName();

		protected:
			void Init();

		public:
			static wchar_t* GetSubClassName();

		protected:
			std::wstring m_strStyleName;
			int m_nElementSize;
			std::list<Element> m_listElement;
			int m_nFillColor;
			double m_dBeginAngle;
			double m_dEndAngle;

		protected:
			static std::wstring m_strSubClassName;
		};
	}
}
