#ifndef __UNE_CORE_API_UPOLYGON_H_INCLUDED__
#define __UNE_CORE_API_UPOLYGON_H_INCLUDED__

#pragma once


#pragma once

#include "CoreAPI.h"
#include <vector>
#include "uobject.h"
#include "UVector3.h"


namespace UnE
{
	namespace Core
	{
		class UBaseView;
		class USpaceVolume;
		class UEffectObject;

		class CORE_API UPolygon : public UCoreObject
		{
			friend class USpaceVolume;
			
		protected:
			bool m_bFirstLoad;
			UBaseView * m_pView;
			float m_fHeight;
		
			std::vector< UnE::Math::Vector3 > m_vecPoints;
		public:
			UPolygon(UBaseView* pView);
			virtual ~UPolygon(void);

			// Add a point to the point list
			void addPoint(const UnE::Math::Vector3 &p);
			
			// Add a point to the point list
			void addPoint(Real x, Real y, Real z);

			// Change the location of an existing point in the point list
			void setPoint(unsigned int index, const UnE::Math::Vector3 &value);

			// Return the location of an existing point in the point list
			UnE::Math::Vector3 getPoint(unsigned short index);

			// Return the total number of points in the point list
			unsigned int getNumPoints(void) const;

			// Remove all points from the point list
			void clear();

			// Call this to update the hardware buffer after making changes.  
			virtual void update();

			void SetHeight(float val) { m_fHeight = val; }
			float GetHeight() { return m_fHeight; }

			void SetVisible(bool bShow);

		protected:
			virtual void CreateNode();

		};

		class CORE_API USpaceVolume : public UCoreObject 
		{
		protected:
			float m_fHeight;
			bool m_bFirstLoad;
			UBaseView * m_pView;
			UEffectObject * m_pEffect;
			
			int m_R;
			int m_G;
			int m_B;

		public:
			USpaceVolume(UBaseView* pView);
			virtual ~USpaceVolume(void);

			void CreateVolume(UPolygon* polygon, float yPos, float fHeight);

			void SetVisible(bool bShow);

			void SetColor(int r, int g, int b);
	
		};

	}
}
#endif//__UNE_CORE_API_UPOLYGON_H_INCLUDED__
