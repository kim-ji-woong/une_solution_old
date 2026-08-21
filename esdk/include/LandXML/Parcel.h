#pragma once
#include <vector>
#include <string>
#include <map>
#include "Geometry/GVertex.h"

namespace UnE
{
	namespace LX
	{
		class CoordGeom
		{
		public:
			CoordGeom();
			virtual ~CoordGeom();

		public:
			void SetStartRef(std::wstring strRef);
			void SetEndRef(std::wstring strRef);

			const std::wstring& GetStartRef() const;
			const std::wstring& GetEndRef() const;

			void AddVertex(Geometry::Vertex3D vertex);
			bool InsertVertex(int nIndex, Geometry::Vertex3D vertex);
			int GetVertexCount() const;
			const Geometry::Vertex3D* GetVertex(int nIndex) const;
			bool RemoveVertex(int nIndex);
			void RemoveAllVertex();

			bool ReadPointList3D(wchar_t* strPoints);

		public:
			static bool GetPoint3DNextIndex(wchar_t* strPoints, unsigned int& rBeginIndex, unsigned int& rEndIndex);

		protected:
			std::wstring m_strStartRef;
			std::wstring m_strEndRef;
			std::vector<Geometry::Vertex3D> m_vecVertices;
		};

		class Parcel
		{
		public:
			Parcel(void);
			virtual ~Parcel(void);

		public:
			Parcel(const Parcel& rhs);
			void operator= (const Parcel& rhs);

		public:
			void SetParcelName(std::wstring strFaceName);
			const std::wstring& GetParcelName() const;

			void AddCoord(CoordGeom* pCoord);
			bool InsertCoord(int nIndex, CoordGeom* pCoord);
			int GetCoordCount() const;
			const CoordGeom* GetCoord(int nIndex) const;
			bool RemoveCoord(int nIndex);
			void RemoveAllCoord();

			void SetAttrib(std::wstring strAttrName, std::wstring strAttrValue);
			bool GetAttrib(std::wstring strAttrName, std::wstring& strAttrValue);

			int GetAttribCount() const;
			bool GetAttrib(int nIndex, std::wstring& strAttrName, std::wstring& strAttrValue);
			bool RemoveAttrib(int nIndex);
			bool RemoveAttrib(std::wstring strAttrName);
			void RemoveAllAttrib();

		private:
			void Copy(const Parcel& rhs);
			void FreeMemory();

		private:
			std::wstring m_strParcelName;
			std::vector<CoordGeom*> m_vecCoords;
			std::map<std::wstring, std::wstring> m_mapAttr;

			int* m_pRefCount;
		};

		class Parcels
		{
		public:
			Parcels();
			virtual ~Parcels();

		public:
			Parcels(const Parcels& rhs);
			void operator= (const Parcels& rhs);

		public:
			void AddParcel(Parcel* pParcel);
			bool InsertParcel(int nIndex, Parcel* pParcel);
			int GetParcelCount() const;
			const Parcel* GetParcel(int nIndex) const;
			bool RemoveParcel(int nIndex);
			void RemoveAllParcel(bool freeMemory = false);

		private:
			void Copy(const Parcels& rhs);
			void FreeMemory();

		private:
			std::vector<Parcel*> m_vecParcels;

			int* m_pRefCount;
		};
	}
}
