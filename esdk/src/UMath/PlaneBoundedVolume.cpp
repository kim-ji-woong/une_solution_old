#include "stdafx.h"
#include "UPlaneBoundedVolume.h"

namespace UnE
{
	namespace Math
	{
		PlaneBoundedVolume::PlaneBoundedVolume() 
			:outside(Side::NEGATIVE_SIDE) 
		{

		}
		/** Constructor, determines which side is deemed to be 'outside' */
		PlaneBoundedVolume::PlaneBoundedVolume(UnE::Math::Side theOutside) 
			: outside(theOutside) 
		{

		}
		PlaneBoundedVolume::PlaneBoundedVolume(const PlaneBoundedVolume& rhs)
		{
			planes = rhs.planes;
			outside = rhs.outside;
		}
	}
}