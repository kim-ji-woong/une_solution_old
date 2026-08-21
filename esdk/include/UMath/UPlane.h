#ifndef __UMATH_Plane_H__
#define __UMATH_Plane_H__

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
		/** Defines a plane in 3D space.
			@remarks
				A plane is defined in 3D space by the equation
				Ax + By + Cz + D = 0
			@par
				This equates to a vector (the normal of the plane, whose x, y
				and z components equate to the coefficients A, B and C
				respectively), and a constant (D) which is the distance along
				the normal you have to go to move the plane back to the origin.
		 */
		ENUM_CLASS Side
		{
			NO_SIDE,
				POSITIVE_SIDE,
				NEGATIVE_SIDE,
				BOTH_SIDE
		};

		UMATH_DECLARE_EXPORT_CLASS(Plane)
		{
		public:

			INSTANCE(Vector3) normal;
			Real d;

			
			/** Default constructor - sets everything to 0.
			*/
			Plane ();
			Plane (REF_CONST(Plane) rhs);
			/** Construct a plane through a normal, and a distance to move the plane along the normal.*/
			Plane (REF_CONST(Vector3) rkNormal, Real fConstant);
			/** Construct a plane using the 4 constants directly **/
			Plane (Real a, Real b, Real c, Real d);
			Plane (REF_CONST(Vector3) rkNormal, REF_CONST(Vector3) rkPoint);
			Plane (REF_CONST(Vector3) rkPoint0, REF_CONST(Vector3) rkPoint1,
				REF_CONST(Vector3) rkPoint2);

			/** The "positive side" of the plane is the half space to which the
				plane normal points. The "negative side" is the other half
				space. The flag "no side" indicates the plane itself.
			*/
			

			Side getSide (REF_CONST(Vector3) rkPoint) CONSTF;

			/**
			Returns the side where the alignedBox is. The flag BOTH_SIDE indicates an intersecting box.
			One corner ON the plane is sufficient to consider the box and the plane intersecting.
			*/
			Side getSide (REF_CONST(AxisAlignedBox) rkBox) CONSTF;

			/** Returns which side of the plane that the given box lies on.
				The box is defined as centre/half-size pairs for effectively.
			@param centre The centre of the box.
			@param halfSize The half-size of the box.
			@return
				POSITIVE_SIDE if the box complete lies on the "positive side" of the plane,
				NEGATIVE_SIDE if the box complete lies on the "negative side" of the plane,
				and BOTH_SIDE if the box intersects the plane.
			*/
			Side getSide (REF_CONST(Vector3) centre, REF_CONST(Vector3) halfSize) CONSTF;

			/** This is a pseudodistance. The sign of the return value is
				positive if the point is on the positive side of the plane,
				negative if the point is on the negative side, and zero if the
				point is on the plane.
				@par
				The absolute value of the return value is the true distance only
				when the plane normal is a unit length vector.
			*/
			Real getDistance (REF_CONST(Vector3) rkPoint) CONSTF;

			/** Redefine this plane based on 3 points. */
			void redefine(REF_CONST(Vector3) rkPoint0, REF_CONST(Vector3) rkPoint1,
				REF_CONST(Vector3) rkPoint2);

			/** Redefine this plane based on a normal and a point. */
			void redefine(REF_CONST(Vector3) rkNormal, REF_CONST(Vector3) rkPoint);

			/** Project a vector onto the plane. 
			@remarks This gives you the element of the input vector that is perpendicular 
				to the normal of the plane. You can get the element which is parallel
				to the normal of the plane by subtracting the result of this method
				from the original vector, since parallel + perpendicular = original.
			@param v The input vector
			*/
			INSTANCE(Vector3) projectVector(REF_CONST(Vector3) v) CONSTF;

			/** Normalises the plane.
				@remarks
					This method normalises the plane's normal and the length scale of d
					is as well.
				@note
					This function will not crash for zero-sized vectors, but there
					will be no changes made to their components.
				@return The previous length of the plane's normal.
			*/
			Real normalise(void);

			
			/// Comparison operator
			bool operator==(REF_CONST(Plane) rhs) CONSTF
			{
				return ( OF(rhs,d) == d &&  OF(rhs,normal) == normal);
			}
			bool operator!=(REF_CONST(Plane) rhs) CONSTF
			{
				return ( OF(rhs,d) != d || OF(rhs, normal) != normal);
			}
#ifndef DOTNET
			UMATH_API FRIEND std::ostream& operator<< (std::ostream& o, REF_CONST(Plane) p);
#endif
		};

		typedef STD_VECTOR(INSTANCE(Plane)) PlaneList;
		/** @} */
		/** @} */
	}
} // namespace UnE

#endif
