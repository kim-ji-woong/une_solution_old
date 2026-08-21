#pragma once
#include "EasyXML/EasyXML2.h"
#include "Units.h"
#include "CgPoint.h"
#include "Parcel.h"

namespace UnE
{
	namespace LX
	{
		class XmlLoader
		{
		public:
			XmlLoader(void);
			virtual ~XmlLoader(void);

		public:
			bool Open(wchar_t* strPath);
			
			Units& GetUnits();
			Parcels& GetParcels();
			CgPoints& GetCgPoints();

		private:
			bool LoadUnits();

			bool LoadParcels();
			bool LoadParcel(DWORD_PTR dwNode);
			bool LoadParcels(DWORD_PTR dwNode, Parcel* pParcel);
			bool LoadParcel(DWORD_PTR dwNode, Parcel* pParcel);
			CoordGeom* LoadCoordGeom(DWORD_PTR dwNode);
			CoordGeom* LoadIrregularLine(DWORD_PTR dwNode);
			bool ReadPointRef(DWORD_PTR dwNode, wchar_t* strValue);
			bool ReadPointList3D(DWORD_PTR dwNode, CoordGeom* pCoord);
			
			bool LoadCgPoints();
			CgPoint* LoadCgPoint(DWORD_PTR dwNode);

		private:
			EasyXML2 m_xml;
			Units m_units;
			Parcels m_parcels;
			CgPoints m_cgPoints;
		};
	}
}
