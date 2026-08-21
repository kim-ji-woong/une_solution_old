#ifndef _UMATH_Rectangle_H__
#define _UMATH_Rectangle_H__

#include "MathAPI.h"


namespace UnE
{
	namespace Math
	{

		/** \addtogroup Core
		*  @{
		*/
		/** \addtogroup General
		*  @{
		*/

		UMATH_DECLARE_EXPORT_CLASS(Rectangle)
		{
		public:
			Real left;
			Real top;
			Real right;
			Real bottom;

			inline bool inside(Real x, Real y) CONSTF { return x >= left && x <= right && y >= top && y <= bottom; }
		};

		/** Geometric intersection of two rectanglar regions.
		 *
		 * @remarks Calculates the geometric intersection of two rectangular
		 * regions.  Rectangle coordinates must be ([0-N], [0-N]), such that
		 * (0,0) is in the upper left hand corner.
		 *
		 * If the two input rectangles do not intersect, then the result will be
		 * a degenerate rectangle, i.e. left >= right or top >= bottom, or both.
		 */
		inline Rectangle intersect(REF_CONST(Rectangle) lhs, REF_CONST(Rectangle) rhs)
		{
			INSTANCE(Rectangle) r = dnonlynew Rectangle();

			OF(r,left)   = OF(lhs,left)   > OF(rhs,left)   ? OF(lhs,left)   : OF(rhs,left);
			OF(r,top)    = OF(lhs,top)    > OF(rhs,top)    ? OF(lhs,top)    : OF(rhs,top);
			OF(r,right)  = OF(lhs,right)  < OF(rhs,right)  ? OF(lhs,right)  : OF(rhs,right);
			OF(r,bottom) = OF(lhs,bottom) < OF(rhs,bottom) ? OF(lhs,bottom) : OF(rhs,bottom);

			return r;
		}
		/** @} */
		/** @} */
	}
}

#endif // _Rectangle_H__
