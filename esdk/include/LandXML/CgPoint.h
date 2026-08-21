#pragma once
#include <string>
#include <vector>
#include "Geometry/GVertex.h"

namespace UnE
{
	namespace LX
	{
		class CgPoint
		{
		public:
			enum SurveyType { MONUMENT = 0, CONTROL, SIDESHOT, BOUNDARY, NATURAL_BOUNDARY, TRAVERSE, REFRERENCE, ADMINISTRATIVE };
			enum StateType { ABANDONED = 0, DESTROYED, EXISTING, PROPOSED };

		public:
			CgPoint(void);
			virtual ~CgPoint(void);

		public:
			void SetName(std::wstring strName);
			void SetOID(std::wstring strOID);
			void SetSurveyType(SurveyType type);
			void SetSurveyType(wchar_t* strType);
			void SetStateType(StateType type);
			void SetStateType(wchar_t* strType);
			void SetVertex(Geometry::Vertex3D vertex);

			std::wstring GetName() const;
			std::wstring GetOID() const;
			SurveyType GetSurveyType() const;
			StateType GetStateType() const;
			const Geometry::Vertex3D& GetVertex() const;

			bool ReadPoint3D(wchar_t* strPoints);

		private:
			std::wstring m_strName;
			SurveyType m_survType;
			StateType m_stateType;
			std::wstring m_strOID;
			Geometry::Vertex3D m_vertex;
		};

		class CgPoints
		{
		public:
			CgPoints();
			virtual ~CgPoints();

		public:
			CgPoints(const CgPoints& rhs);
			void operator= (const CgPoints& rhs);

		public:
			void AddPoint(CgPoint* pPoint);
			bool InsertPoint(int nIndex, CgPoint* pPoint);
			int GetPointCount() const;
			const CgPoint* GetPoint(int nIndex) const;
			bool RemovePoint(int nIndex);
			void RemoveAllPoint(bool freeMemory = false);

		private:
			void FreeMemory();
			void Copy(const CgPoints& rhs);

		private:
			std::vector<CgPoint*> m_vecPoints;
			int* m_pRefCount;
		};
	}
}
