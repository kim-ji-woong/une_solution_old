#ifndef __UMATH_Ray_H_
#define __UMATH_Ray_H_

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
		/** Representation of a ray in space, i.e. a line with an origin and direction. */
		UMATH_DECLARE_EXPORT_CLASS(Ray)
		{
		protected:
			INSTANCE(Vector3) mOrigin;
			INSTANCE(Vector3) mDirection;
		public:
			Ray():mOrigin(Vector3::ZERO), mDirection(Vector3::UNIT_Z) {}
			Ray(REF_CONST(Vector3) origin, REF_CONST(Vector3) direction)
				:mOrigin(origin), mDirection(direction) {}

			/** Sets the origin of the ray. */
			void setOrigin(REF_CONST(Vector3) origin) {mOrigin = origin;} 
			/** Gets the origin of the ray. */
			REF_CONST(Vector3) getOrigin(void) CONSTF {return mOrigin;} 

			/** Sets the direction of the ray. */
			void setDirection(REF_CONST(Vector3) dir) {mDirection = dir;} 
			/** Gets the direction of the ray. */
			REF_CONST(Vector3) getDirection(void) CONSTF {return mDirection;} 

			/** Gets the position of a point t units along the ray. */
			INSTANCE(Vector3) getPoint(Real t) CONSTF { 
				return dnonlynew Vector3(mOrigin + (mDirection * t));
			}
		

#ifdef DOTNET
			/** Gets the position of a point t units along the ray. */
			static INSTANCE(Vector3) operator*(REF_CONST(Ray) ray , Real t) CONSTF 
			{ 
				return OF( ray, getPoint(t) );
			}
#else
			/** Gets the position of a point t units along the ray. */
			INSTANCE(Vector3) operator*(Real t) CONSTF 
			{ 
				return getPoint(t);
			}
#endif

			/** Tests whether this ray intersects the given plane. 
			@return A pair structure where the first element indicates whether
				an intersection occurs, and if true, the second element will
				indicate the distance along the ray at which it intersects. 
				This can be converted to a point in space by calling getPoint().
			*/
			INSTANCE(STD_PAIR(bool, Real)) intersects(REF_CONST(Plane) p) CONSTF
			{
				return UMath::intersects(THIS_OBJ, p);
			}
			/** Tests whether this ray intersects the given plane bounded volume. 
			@return A pair structure where the first element indicates whether
			an intersection occurs, and if true, the second element will
			indicate the distance along the ray at which it intersects. 
			This can be converted to a point in space by calling getPoint().
			*/
			//.STD_PAIR<bool, Real> intersects(const PlaneBoundedVolume& p) const
			//{
			//	return Math::intersects(*this, p.planes, p.outside == Plane::POSITIVE_SIDE);
			//}
			/** Tests whether this ray intersects the given sphere. 
			@return A pair structure where the first element indicates whether
				an intersection occurs, and if true, the second element will
				indicate the distance along the ray at which it intersects. 
				This can be converted to a point in space by calling getPoint().
			*/
			INSTANCE(STD_PAIR(bool, Real)) intersects(REF_CONST(Sphere) s) CONSTF
			{
				return UMath::intersects(THIS_OBJ, s);
			}
			/** Tests whether this ray intersects the given box. 
			@return A pair structure where the first element indicates whether
				an intersection occurs, and if true, the second element will
				indicate the distance along the ray at which it intersects. 
				This can be converted to a point in space by calling getPoint().
			*/
			INSTANCE(STD_PAIR(bool, Real)) intersects(REF_CONST(AxisAlignedBox) box) CONSTF
			{
				return UMath::intersects(THIS_OBJ, box);
			}

		};
		/** @} */
		/** @} */
	}
}
#endif
