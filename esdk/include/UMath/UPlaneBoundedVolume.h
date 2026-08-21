#ifndef __UNE_MATH_PLANEBOUNDEDVOLUME_H_INCLUDED__
#define __UNE_MATH_PLANEBOUNDEDVOLUME_H_INCLUDED__

#pragma once

#include "UMathAPI.h"

#include "UAxisAlignedBox.h"
#include "USphere.h"
#include "UMath.h"
#include "UPlane.h"

namespace UnE
{
	namespace Math
	{
		/** \addtogroup Core
		*  @{
		*/
		/** \addtogroup Math
		*  @{
		*/
		/** Represents a convex volume bounded by planes.
		*/
		UMATH_DECLARE_EXPORT_CLASS(PlaneBoundedVolume)
		{
		public:
			typedef std::vector<UnE::Math::Plane> PlaneList;
			/// Publicly accessible plane list, you can modify this direct
			PlaneList planes;
			UnE::Math::Side outside;

			PlaneBoundedVolume();
			/** Constructor, determines which side is deemed to be 'outside' */
			PlaneBoundedVolume(UnE::Math::Side theOutside);
			PlaneBoundedVolume(const PlaneBoundedVolume& rhs);

			virtual ~PlaneBoundedVolume(){}

			/** Intersection test with AABB
			@remarks May return false positives but will never miss an intersection.
			*/
			inline bool intersects(const AxisAlignedBox& box) const
			{
				if (box.isNull()) return false;
				if (box.isInfinite()) return true;

				// Get centre of the box
				UnE::Math::Vector3 centre = box.getCenter();
				// Get the half-size of the box
				UnE::Math::Vector3 halfSize = box.getHalfSize();
			
				PlaneList::const_iterator i, iend;
				iend = planes.end();
				for (i = planes.begin(); i != iend; ++i)
				{
					const Plane& plane = *i;

					Side side = plane.getSide(centre, halfSize);
					if (side == outside)
					{
						// Found a splitting plane therefore return not intersecting
						return false;
					}
				}

				// couldn't find a splitting plane, assume intersecting
				return true;

			}
			/** Intersection test with Sphere
			@remarks May return false positives but will never miss an intersection.
			*/
			inline bool intersects(const Sphere& sphere) const
			{
				PlaneList::const_iterator i, iend;
				iend = planes.end();
				for (i = planes.begin(); i != iend; ++i)
				{
					const Plane& plane = *i;

					// Test which side of the plane the sphere is
					Real d = plane.getDistance(sphere.getCenter());
					// Negate d if planes point inwards
					if (outside == Side::NEGATIVE_SIDE) d = -d;

					if ( (d - sphere.getRadius()) > 0)
						return false;
				}

				return true;

			}

			/** Intersection test with a Ray
			@return std::pair of hit (bool) and distance
			@remarks May return false positives but will never miss an intersection.
			*/
			inline std::pair<bool, Real> intersects(const Ray& ray)
			{
				return UMath::intersects(ray, planes, outside == Side::POSITIVE_SIDE);
			}

		};

		typedef std::vector<PlaneBoundedVolume> PlaneBoundedVolumeList;

		/** @} */
		/** @} */


	} // namespace Core
}// namespace UnE

#endif

