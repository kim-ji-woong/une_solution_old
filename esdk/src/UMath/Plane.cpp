#include "stdafx.h"
#include "UPlane.h"
#include "UMatrix3.h"
#include "UAxisAlignedBox.h"


namespace UnE
{
	namespace Math
	{
		//-----------------------------------------------------------------------
		Plane::Plane ()
		{
			normal = dnonlynew Vector3(Vector3::ZERO);
			d = 0.0;
		}
		//-----------------------------------------------------------------------
		Plane::Plane (REF_CONST(Plane) rhs)
		{
			normal = dnonlynew Vector3(OF(rhs,normal));
			d = OF(rhs,d);
		}
		//-----------------------------------------------------------------------
		Plane::Plane (REF_CONST(Vector3) rkNormal, Real fConstant)
		{
			normal = dnonlynew Vector3(rkNormal);
			d = -fConstant;
		}
		//---------------------------------------------------------------------
		Plane::Plane (Real a, Real b, Real c, Real _d)
			: d(_d)
		{
			normal = dnonlynew Vector3(a,b,c);
		}
		//-----------------------------------------------------------------------
		Plane::Plane (REF_CONST(Vector3) rkNormal, REF_CONST(Vector3) rkPoint)
		{
			normal = dnonlynew Vector3();
			redefine(rkNormal, rkPoint);
		}
		//-----------------------------------------------------------------------
		Plane::Plane (REF_CONST(Vector3) rkPoint0, REF_CONST(Vector3) rkPoint1, REF_CONST(Vector3) rkPoint2)
		{
			normal = dnonlynew Vector3();
			redefine(rkPoint0, rkPoint1, rkPoint2);
		}
		//-----------------------------------------------------------------------
		Real Plane::getDistance (REF_CONST(Vector3) rkPoint) CONSTF
		{
			return OF(normal,dotProduct(rkPoint)) + d;
		}
		//-----------------------------------------------------------------------
		Side Plane::getSide (REF_CONST(Vector3) rkPoint) CONSTF
		{
			Real fDistance = getDistance(rkPoint);

			if ( fDistance < 0.0 )
				return ENUM_OF(Side,NEGATIVE_SIDE);

			if ( fDistance > 0.0 )
				return ENUM_OF(Side,POSITIVE_SIDE);

			return ENUM_OF(Side, NO_SIDE);
		}


		//-----------------------------------------------------------------------
		Side Plane::getSide (REF_CONST(AxisAlignedBox) box) CONSTF
		{
			if (OF(box,isNull())) 
				return ENUM_OF(Side,NO_SIDE);
			if (OF(box,isInfinite()))
				return ENUM_OF(Side,BOTH_SIDE);

			return getSide(OF(box,getCenter()), OF(box,getHalfSize()));
		}
		//-----------------------------------------------------------------------
		Side Plane::getSide (REF_CONST(Vector3) centre, REF_CONST(Vector3) halfSize) CONSTF
		{
			// Calculate the distance between box centre and the plane
			Real dist = getDistance(centre);

			// Calculate the maximise allows absolute distance for
			// the distance between box centre and plane
			Real maxAbsDist = OF(normal,absDotProduct(halfSize));

			if (dist < -maxAbsDist)
				return ENUM_OF(Side, NEGATIVE_SIDE);

			if (dist > +maxAbsDist)
				return ENUM_OF(Side, POSITIVE_SIDE);

			return ENUM_OF(Side, BOTH_SIDE);
		}
		//-----------------------------------------------------------------------
		void Plane::redefine(REF_CONST(Vector3) rkPoint0, REF_CONST(Vector3) rkPoint1, REF_CONST(Vector3) rkPoint2)
		{
			INSTANCE(Vector3) kEdge1 = rkPoint1 - rkPoint0;
			INSTANCE(Vector3) kEdge2 = rkPoint2 - rkPoint0;
			normal = OF(kEdge1,crossProduct(kEdge2));
			OF(normal,normalise());
			d = -OF(normal,dotProduct(rkPoint0));
		}
		//-----------------------------------------------------------------------
		void Plane::redefine(REF_CONST(Vector3) rkNormal, REF_CONST(Vector3) rkPoint)
		{
			normal = dnonlynew Vector3(rkNormal);
			d = -OF(rkNormal,dotProduct(rkPoint));
		}
		//-----------------------------------------------------------------------
		INSTANCE(Vector3) Plane::projectVector(REF_CONST(Vector3) p) CONSTF
		{
			// We know plane normal is unit length, so use simple method
			INSTANCE(Matrix3) xform = dnonlynew Matrix3();
			IDX(xform,0,0) = 1.0f - OF(normal,x) * OF(normal,x);
			IDX(xform,0,1) = -OF(normal,x) * OF(normal,y);
			IDX(xform,0,2) = -OF(normal,x) * OF(normal,z);
			IDX(xform,1,0) = -OF(normal,y) * OF(normal,x);
			IDX(xform,1,1) = 1.0f - OF(normal,y) * OF(normal,y);
			IDX(xform,1,2) = -OF(normal,y) * OF(normal,z);
			IDX(xform,2,0) = -OF(normal,z) * OF(normal,x);
			IDX(xform,2,1) = -OF(normal,z) * OF(normal,y);
			IDX(xform,2,2) = 1.0f - OF(normal,z) * OF(normal,z);
			return xform * p;
		}
		//-----------------------------------------------------------------------
		Real Plane::normalise(void)
		{
			Real fLength = OF(normal,length());

			// Will also work for zero-sized vectors, but will change nothing
			// We're not using epsilons because we don't need to.
			// Read http://OF(www,ogre3d).org/forums/viewtopic.php?f=4&t=61259
			if ( fLength > Real(0.0f) )
			{
				Real fInvLength = 1.0f / fLength;
				normal *= fInvLength;
				d *= fInvLength;
			}

			return fLength;
		}

#ifndef DOTNET
		//-----------------------------------------------------------------------
		std::ostream& operator<< (std::ostream& o, REF_CONST(Plane) p)
		{
			o << "Plane(normal=" << OF(p,normal) << ", d=" << OF(p,d) << ")";
			return o;
		}
#endif
	}
} // namespace UnE
