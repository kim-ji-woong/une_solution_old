//=============================================================
//
// C4 Engine version 2.9.1
// Copyright 1999-2012, by Terathon Software LLC
//
// This copy is licensed to the following:
//
//     Registered user: Soo Ki Kim
//     Maximum number of users: 1
//     License #C4T0035002
//
// License is granted under terms of the license agreement
// entered by the registed user.
//
// Unauthorized redistribution of source code is strictly
// prohibited. Violators will be prosecuted.
//
//=============================================================


#ifndef C4Shadows_h
#define C4Shadows_h


#include "C4Renderable.h"


namespace C4
{
	class Light;
	class InfiniteLight;
	class PointLight;
	class Geometry;
	
	
	class StencilData : public StencilShadow
	{
		friend class Geometry;
		
		private:
			
			Geometry	*shadowGeometry;
			char		*shadowStorage;
		
		public:
			
			StencilData(Geometry *geometry);
			~StencilData();
			
			Geometry *GetGeometry(void) const
			{
				return (shadowGeometry);
			}
			
			bool *Activate(bool front = false);
			void Deactivate(void);
			
			void CalculateInfiniteShadowBounds(const InfiniteLight *light);
			void CalculatePointShadowBounds(const PointLight *light);
	};
	
	
	class StencilVolume : public StencilData, public ListElement<StencilVolume>, public LinkTarget<StencilVolume>
	{
		private:
			
			Light		*targetLight;
			
			int32		extrusionDetailLevel;
			int32		endcapDetailLevel;
		
		public:
			
			StencilVolume(Geometry *geometry, Light *light, Link<StencilVolume> *link);
			~StencilVolume();
			
			Light *GetLight(void) const
			{
				return (targetLight);
			}
			
			int32 GetExtrusionDetailLevel(void) const
			{
				return (extrusionDetailLevel);
			}
			
			void SetExtrusionDetailLevel(int32 level)
			{
				extrusionDetailLevel = level;
			}
			
			int32 GetEndcapDetailLevel(void) const
			{
				return (endcapDetailLevel);
			}
			
			void SetEndcapDetailLevel(int32 level)
			{
				endcapDetailLevel = level;
			}
	};
}


#endif

// ZYURVUR
