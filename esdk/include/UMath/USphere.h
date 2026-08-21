#ifndef __UMATH_Sphere_H_
#define __UMATH_Sphere_H_


#include "UMathAPI.h"
#include "UVector3.h"

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
		/** A sphere primitive, mostly used for bounds checking. 
		@remarks
			A sphere in math texts is normally represented by the function
			x^2 + y^2 + z^2 = r^2 (for sphere's centered on the origin). Ogre stores spheres
			simply as a center point and a radius.
		*/
		UMATH_DECLARE_EXPORT_CLASS(Sphere)
		{
		protected:
			Real mRadius;
			INSTANCE(Vector3) mCenter;
		public:
			/** Standard constructor - creates a unit sphere around the origin.*/
			Sphere() : mRadius(1.0), mCenter(dnonlynew Vector3(Vector3::ZERO))
			{

			}
			/** Constructor allowing arbitrary spheres. 
				@param center The center point of the sphere.
				@param radius The radius of the sphere.
			*/
			Sphere(REF_CONST(Vector3) center, Real radius)
				: mRadius(radius), mCenter(dnonlynew Vector3(Vector3::ZERO)) {}

			/** Returns the radius of the sphere. */
			Real getRadius(void) CONSTF { return mRadius; }

			/** Sets the radius of the sphere. */
			void setRadius(Real radius) { mRadius = radius; }

			/** Returns the center point of the sphere. */
			REF_CONST(Vector3) getCenter(void) CONSTF { return mCenter; }

			/** Sets the center point of the sphere. */
			void setCenter(REF_CONST(Vector3) center) { mCenter = dnonlynew Vector3(center); }

			/** Returns whether or not this sphere intersects another sphere. */
			bool intersects(REF_CONST(Sphere) s) CONSTF
			{
				INSTANCE(Vector3) vec = OF(s,mCenter) - mCenter;
				Real r = OF( vec , squaredLength());
				return (  r  <= UMath::Sqr(  OF(s,mRadius) + mRadius));
			}
			/** Returns whether or not this sphere intersects a box. */
			bool intersects(REF_CONST(AxisAlignedBox) box) CONSTF
			{
				return UMath::intersects(THIS_OBJ, box);
			}
			/** Returns whether or not this sphere intersects a plane. */
			bool intersects(REF_CONST(Plane) plane) CONSTF
			{
				return UMath::intersects(THIS_OBJ, plane);
			}
			/** Returns whether or not this sphere intersects a point. */
			bool intersects(REF_CONST(Vector3) v) CONSTF
			{
				return (OF((v - mCenter),squaredLength()) <= UMath::Sqr(mRadius));
			}
			/** Merges another Sphere into the current sphere */
			void merge(REF_CONST(Sphere) oth)
			{
				INSTANCE(Vector3) vec = OF(oth,mCenter) - mCenter;
				INSTANCE(Vector3) diff =  vec;
				Real lengthSq = OF(diff,squaredLength());
				Real radiusDiff = OF(oth,getRadius()) - mRadius;
			
				// Early-out
				if (UMath::Sqr(radiusDiff) >= lengthSq) 
				{
					// One fully contains the other
					if (radiusDiff <= 0.0f) 
						return; // no change
					else 
					{
						mCenter = dnonlynew Vector3(OF(oth,mCenter));
						mRadius = OF(oth,getRadius());
						return;
					}
				}
			
				Real length = UMath::Sqrt(lengthSq);
			
				INSTANCE(Vector3) newCenter = dnonlynew Vector3();
				Real newRadius;
				if ((length + OF(oth,getRadius())) > mRadius) 
				{
					Real t = (length + radiusDiff) / (2.0f * length);
					newCenter = mCenter + diff * t;
				} 
				// otherwise, we keep our existing center
			
				newRadius = 0.5f * (length + mRadius + OF(oth,getRadius()));
			
				mCenter = newCenter;
				mRadius = newRadius;
			}
		

		};
		/** @} */
		/** @} */
	}
}

#endif

