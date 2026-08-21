#include "stdafx.h"

#include <math.h>

#include <math.h>
#include "asm_math.h"
#include "UVector2.h"
#include "UVector3.h"
#include "UVector4.h"
#include "UQuaternion.h"
#include "URay.h"
#include "USphere.h"
#include "UAxisAlignedBox.h"
#include "UPlane.h"
#include "UMatrix4.h"
#include "UMath.h"

#ifdef DOTNET
using namespace System;
using namespace System::Collections;
using namespace System::Collections::Generic;
#endif

namespace UnE
{
	namespace Math
	{
	#ifndef DOTNET
		const Real UMath::POS_INFINITY = std::numeric_limits<Real>::infinity();
		const Real UMath::NEG_INFINITY = -std::numeric_limits<Real>::infinity();
		const Real UMath::PI = Real( 3.14159265358979323846 );
		const Real UMath::TWO_PI = Real(  2.0 * PI );
		const Real UMath::HALF_PI = Real( 0.5 * PI );
		const Real UMath::fDeg2Rad = PI / Real(180.0);
		const Real UMath::fRad2Deg = Real(180.0) / PI;
		const Real UMath::LOG2 = log(Real(2.0));

		int UMath::mTrigTableSize;
		UMath::AngleUnit UMath::msAngleUnit;

		Real  UMath::mTrigTableFactor;
		Real *UMath::mSinTable = NULL;
		Real *UMath::mTanTable = NULL;	
		
	#endif
		
		//-----------------------------------------------------------------------
		UMath::UMath( unsigned int trigTableSize )
		{
			msAngleUnit = ENUM_OF(AngleUnit, AU_DEGREE);
			mTrigTableSize = trigTableSize;
			mTrigTableFactor = mTrigTableSize / UMath::TWO_PI;

#ifdef DOTNET
			mSinTable = dnonlynew array<Real>(mTrigTableSize);
			mTanTable = dnonlynew array<Real>(mTrigTableSize);
#else
			mSinTable = (Real*)malloc(sizeof(Real)*mTrigTableSize);
			mTanTable = (Real*)malloc(sizeof(Real)*mTrigTableSize);
#endif

			buildTrigTables();
		}

		//-----------------------------------------------------------------------
		UMath::~UMath()
		{
#ifndef DOTNET
			free (mSinTable);
			free (mTanTable);
#endif
		}

		//-----------------------------------------------------------------------
		void UMath::buildTrigTables(void)
		{
			// Build trig lookup tables
			// Could get away with building only PI sized Sin table but simpler this 
			// way. Who cares, it'll ony use an extra 8k of memory anyway and I like 
			// simplicity.
			Real angle;
			for (int i = 0; i < mTrigTableSize; ++i)
			{
				angle = UMath::TWO_PI * i / mTrigTableSize;
				mSinTable[i] = sin(angle);
				mTanTable[i] = tan(angle);
			}
		}
		//-----------------------------------------------------------------------	
		Real UMath::SinTable (Real fValue)
		{
			// Convert range to index values, wrap if required
			int idx;
			if (fValue >= 0)
			{
				idx = int(fValue * mTrigTableFactor) % mTrigTableSize;
			}
			else
			{
				idx = mTrigTableSize - (int(-fValue * mTrigTableFactor) % mTrigTableSize) - 1;
			}

			return mSinTable[idx];
		}
		//-----------------------------------------------------------------------
		Real UMath::TanTable (Real fValue)
		{
			// Convert range to index values, wrap if required
			int idx = int(fValue *= mTrigTableFactor) % mTrigTableSize;
			return mTanTable[idx];
		}
		//-----------------------------------------------------------------------
		int UMath::ISign (int iValue)
		{
			return ( iValue > 0 ? +1 : ( iValue < 0 ? -1 : 0 ) );
		}
		//-----------------------------------------------------------------------
		INSTANCE(Radian) UMath::ACos (Real fValue)
		{
			if ( -1.0 < fValue )
			{
				if ( fValue < 1.0 )
					return dnonlynew Radian(acos(fValue));
				else
					return dnonlynew Radian(0.0);
			}
			else
			{
				return dnonlynew Radian(PI);
			}
		}
		//-----------------------------------------------------------------------
		INSTANCE(Radian) UMath::ASin (Real fValue)
		{
			if ( -1.0 < fValue )
			{
				if ( fValue < 1.0 )
					return dnonlynew Radian(asin(fValue));
				else
					return dnonlynew Radian(HALF_PI);
			}
			else
			{
				return  dnonlynew Radian(-HALF_PI);
			}
		}
		//-----------------------------------------------------------------------
		Real UMath::Sign (Real fValue)
		{
			if ( fValue > 0.0 )
				return 1.0;

			if ( fValue < 0.0 )
				return -1.0;

			return 0.0;
		}
		//-----------------------------------------------------------------------
		Real UMath::InvSqrt(Real fValue)
		{
			return Real(asm_rsq(fValue));
		}
		//-----------------------------------------------------------------------
		Real UMath::UnitRandom ()
		{
			return asm_rand() / asm_rand_max();
		}
	
		//-----------------------------------------------------------------------
		Real UMath::RangeRandom (Real fLow, Real fHigh)
		{
			return (fHigh-fLow)*UnitRandom() + fLow;
		}

		//-----------------------------------------------------------------------
		Real UMath::SymmetricRandom ()
		{
			return 2.0f * UnitRandom() - 1.0f;
		}

	   //-----------------------------------------------------------------------
		void UMath::setAngleUnit(UMath::AngleUnit unit)
	   {
		   msAngleUnit = unit;
	   }
	   //-----------------------------------------------------------------------
	   UMath::AngleUnit UMath::getAngleUnit(void)
	   {
		   return msAngleUnit;
	   }
		//-----------------------------------------------------------------------
		Real UMath::AngleUnitsToRadians(Real angleunits)
		{
		   if (msAngleUnit == ENUM_OF(AngleUnit,AU_DEGREE))
			   return angleunits * fDeg2Rad;
		   else
			   return angleunits;
		}

		//-----------------------------------------------------------------------
		Real UMath::RadiansToAngleUnits(Real radians)
		{
		   if (msAngleUnit == ENUM_OF(AngleUnit,AU_DEGREE))
			   return radians * fRad2Deg;
		   else
			   return radians;
		}

		//-----------------------------------------------------------------------
		Real UMath::AngleUnitsToDegrees(Real angleunits)
		{
		   if (msAngleUnit == ENUM_OF(AngleUnit,AU_RADIAN))
			   return angleunits * fRad2Deg;
		   else
			   return angleunits;
		}

		//-----------------------------------------------------------------------
		Real UMath::DegreesToAngleUnits(Real degrees)
		{
		   if (msAngleUnit == ENUM_OF(AngleUnit,AU_RADIAN))
			   return degrees * fDeg2Rad;
		   else
			   return degrees;
		}

		//-----------------------------------------------------------------------
		bool UMath::pointInTri2D(REF_CONST(Vector2) p, REF_CONST(Vector2) a, REF_CONST(Vector2) b, REF_CONST(Vector2) c)
		{
			// Winding must be consistent from all edges for point to be inside
			INSTANCE(Vector2) v1 = dnonlynew Vector2();
			INSTANCE(Vector2) v2 = dnonlynew Vector2();
			Real dot[3];
			bool zeroDot[3];

			v1 = b - a;
			v2 = p - a;

			// Note we don't care about normalisation here since sign is all we need
			// It means we don't have to worry about magnitude of cross products either
			dot[0] =   OF( v1, crossProduct(v2));
			zeroDot[0] = UMath::RealEqual(dot[0], 0.0f, float(1e-3));


			v1 = c - b;
			v2 = p - b;

			dot[1] =  OF( v1,crossProduct(v2));
			zeroDot[1] = UMath::RealEqual(dot[1], 0.0f, float(1e-3));

			// Compare signs (ignore colinear / coincident points)
			if(!zeroDot[0] && !zeroDot[1] 
			&& UMath::Sign(dot[0]) != UMath::Sign(dot[1]))
			{
				return false;
			}

			v1 = a - c;
			v2 = p - c;

			dot[2] =  OF( v1,crossProduct(v2));
			zeroDot[2] = UMath::RealEqual(dot[2], 0.0f, float(1e-3));
			// Compare signs (ignore colinear / coincident points)
			if((!zeroDot[0] && !zeroDot[2] 
				&& UMath::Sign(dot[0]) != UMath::Sign(dot[2])) ||
				(!zeroDot[1] && !zeroDot[2] 
				&& UMath::Sign(dot[1]) != UMath::Sign(dot[2])))
			{
				return false;
			}
			return true;
		}
		//-----------------------------------------------------------------------
		bool UMath::pointInTri3D(REF_CONST(Vector3) p, REF_CONST(Vector3) a, 
			REF_CONST(Vector3) b, REF_CONST(Vector3) c, REF_CONST(Vector3) normal)
		{
			// Winding must be consistent from all edges for point to be inside
			INSTANCE(Vector3) v1 = dnonlynew Vector3();
			INSTANCE(Vector3) v2 = dnonlynew Vector3();

			Real dot[3];
			bool zeroDot[3];

			v1 = b - a;
			v2 = p - a;

			// Note we don't care about normalisation here since sign is all we need
			// It means we don't have to worry about magnitude of cross products either
			dot[0] =  OF( OF( v1 , crossProduct(v2)) ,dotProduct(normal));
			zeroDot[0] = UMath::RealEqual(dot[0], 0.0f, float(1e-3));


			v1 = c - b;
			v2 = p - b;

			dot[1] = OF( OF( v1 , crossProduct(v2)) ,dotProduct(normal));
			zeroDot[1] = UMath::RealEqual(dot[1], 0.0f, float(1e-3));

			// Compare signs (ignore colinear / coincident points)
			if(!zeroDot[0] && !zeroDot[1] 
				&& UMath::Sign(dot[0]) != UMath::Sign(dot[1]))
			{
				return false;
			}

			v1 = a - c;
			v2 = p - c;

			dot[2] = OF( OF( v1 , crossProduct(v2)) ,dotProduct(normal));
			zeroDot[2] = UMath::RealEqual(dot[2], 0.0f, float(1e-3));
			// Compare signs (ignore colinear / coincident points)
			if((!zeroDot[0] && !zeroDot[2] 
				&& UMath::Sign(dot[0]) != UMath::Sign(dot[2])) ||
				(!zeroDot[1] && !zeroDot[2] 
				&& UMath::Sign(dot[1]) != UMath::Sign(dot[2])))
			{
				return false;
			}


			return true;
		}
		//-----------------------------------------------------------------------
		bool UMath::RealEqual( Real a, Real b, Real tolerance )
		{
			if (fabs(b-a) <= tolerance)
				return true;
			else
				return false;
		}

		//-----------------------------------------------------------------------
		INSTANCE(STD_PAIR(bool, Real)) UMath::intersects(REF_CONST(Ray) ray, REF_CONST(Plane) plane)
		{

			Real denom = OF( OF(plane,normal) , dotProduct(OF(ray,getDirection())));
			if (UMath::Abs(denom) < std::numeric_limits<Real>::epsilon())
			{
				// Parallel
				return dnonlynew STD_PAIR(bool, Real)(false, 0.0f);
			}
			else
			{
				Real nom = OF( OF(plane,normal), dotProduct(OF(ray,getOrigin()))) + OF(plane,d);
				Real t = -(nom/denom);
				return dnonlynew STD_PAIR(bool, Real)(t >= 0, t);
			}
		
		}
		//-----------------------------------------------------------------------
		INSTANCE(STD_PAIR(bool, Real)) UMath::intersects(REF_CONST(Ray) ray, REF_CONST(STD_VECTOR(INSTANCE(Plane))) planes, bool normalIsOutside)
		{
			
#ifdef DOTNET
			LinkedList<Plane^>^ planesList = dnonlynew LinkedList<Plane^>(planes);				
#else
			INSTANCE(STD_LIST(INSTANCE(Plane))) planesList;
			for (STD_VECTOR(INSTANCE(Plane))::const_iterator i = planes.begin(); i != planes.end(); ++i)
			{
				planesList.push_back(*i);
			}
#endif
			return intersects(ray, planesList, normalIsOutside);

		}
		//-----------------------------------------------------------------------
		INSTANCE(STD_PAIR(bool, Real)) UMath::intersects(REF_CONST(Ray) ray, REF_CONST(STD_LIST(INSTANCE(Plane))) planes, bool normalIsOutside)
		{			
#ifdef DOTNET			
			KeyValuePair<bool, Real>^  ret = dnonlynew KeyValuePair<bool,Real>(false, 0.0f);
			KeyValuePair<bool, Real>^  end = dnonlynew KeyValuePair<bool,Real>(false, 0.0f);

			System::Collections::Generic::LinkedListNode<Plane^>^ planeitend = planes->Last;
			System::Collections::Generic::LinkedListNode<Plane^>^ planeit = planes->First;
			bool allInside = true;

			// derive side
			// NB we don't pass directly since that would require Plane::Side in 
			// interface, which results in recursive includes since Math is so fundamental
			Side outside = normalIsOutside ? Side::POSITIVE_SIDE : Side::NEGATIVE_SIDE;
			
			for (  ; planeit != planeitend; planeit = planeit->Next)
			{
				REF_CONST(Plane) plane = planeit->Value;
				// is origin outside?
				if (plane->getSide(ray->getOrigin()) == outside)
				{
					allInside = false;
					// Test single plane
					INSTANCE(STD_PAIR(bool, Real)) planeRes = OF(ray,intersects(plane));
					if (planeRes->Key)
					{
						// Ok, we intersected
						bool b = true;
						// Use the most distant result since convex volume
						Real v = ((ret->Value > planeRes->Value) ? ret->Value : planeRes->Value);

						ret = dnonlynew KeyValuePair<bool,Real>(b, v);
					}
					else
					{
						ret = dnonlynew KeyValuePair<bool,Real>(false, 0.0f);
						return ret;
					}
				}
				else
				{
					INSTANCE(STD_PAIR(bool, Real)) planeRes = OF(ray,intersects(plane));
					if (planeRes->Key )
					{
						if( ! end->Key )
						{
							end = dnonlynew KeyValuePair<bool,Real>( end->Key, planeRes->Value);
						}
						else
						{
							end = dnonlynew KeyValuePair<bool,Real>( end->Key, end->Value);
						}

					}

				}
			}

			if (allInside)
			{
				// Intersecting at 0 distance since inside the volume!
				ret = dnonlynew KeyValuePair<bool,Real>(true, 0.0f);
				return ret;
			}

			if( end->Key )
			{
				if( end->Value < ret->Value )
				{
					ret = dnonlynew KeyValuePair<bool,Real>(false, ret->Value);
					return ret;
				}
			}
#else
			INSTANCE(STD_PAIR(bool, Real)) ret;
			INSTANCE(STD_PAIR(bool, Real)) end;
			STD_LIST(INSTANCE(Plane))::const_iterator planeit, planeitend;
			planeitend = planes.end();
			bool allInside = true;
			
			ret.first = false;
			ret.second = 0.0f;
			end.first = false;
			end.second = 0;


			// derive side
			// NB we don't pass directly since that would require Plane::Side in 
			// interface, which results in recursive includes since Math is so fundamental
			Side outside = normalIsOutside ? POSITIVE_SIDE : NEGATIVE_SIDE;

			for (planeit = planes.begin(); planeit != planeitend; ++planeit)
			{
				REF_CONST(Plane) plane = *planeit;
				// is origin outside?
				if (plane.getSide(ray.getOrigin()) == outside)
				{
					allInside = false;
					// Test single plane
					STD_PAIR(bool, Real) planeRes = OF(ray,intersects(plane));
					if (planeRes.first)
					{
						// Ok, we intersected
						ret.first = true;
						// Use the most distant result since convex volume
						ret.second = std::max(ret.second, planeRes.second);
					}
					else
					{
						ret.first =false;
						ret.second=0.0f;
						return ret;
					}
				}
				else
				{
					STD_PAIR(bool, Real) planeRes = 
						OF(ray,intersects(plane));
					if (planeRes.first)
					{
						if( !end.first )
						{
							end.first = true;
							end.second = planeRes.second;
						}
						else
						{
							end.second = std::min( planeRes.second, end.second );
						}

					}

				}
			}

			if (allInside)
			{
				// Intersecting at 0 distance since inside the volume!
				ret.first = true;
				ret.second = 0.0f;
				return ret;
			}

			if( end.first )
			{
				if( end.second < ret.second )
				{
					ret.first = false;
					return ret;
				}
			}
#endif
			return ret;
		}
		//-----------------------------------------------------------------------
		INSTANCE(STD_PAIR(bool, Real)) UMath::intersects(REF_CONST(Ray) ray, REF_CONST(Sphere) sphere, 
			bool discardInside)
		{
			REF_CONST(Vector3) raydir = OF(ray,getDirection());
			// Adjust ray origin relative to sphere center
			REF_CONST(Vector3) rayorig = OF(ray,getOrigin()) - OF(sphere, getCenter());
			Real radius = OF(sphere,getRadius());

			// Check origin inside first
			if (OF(rayorig,squaredLength()) <= radius*radius && discardInside)
			{
				return dnonlynew STD_PAIR(bool, Real)(true, 0.0f);
			}

			// Mmm, quadratics
			// Build coeffs which can be used with std quadratic solver
			// ie t = (-b +/- sqrt(b*b + 4ac)) / 2a
			Real a = OF(raydir,dotProduct(raydir));
			Real b = 2 * OF(rayorig,dotProduct(raydir));
			Real c = OF(rayorig,dotProduct(rayorig)) - radius*radius;

			// Calc determinant
			Real d = (b*b) - (4 * a * c);
			if (d < 0)
			{
				// No intersection
				return dnonlynew STD_PAIR(bool, Real)(false, 0.0f);
			}
			else
			{
				// BTW, if d=0 there is one intersection, if d > 0 there are 2
				// But we only want the closest one, so that's ok, just use the 
				// '-' version of the solver
				Real t = ( -b - UMath::Sqrt(d) ) / (2 * a);
				if (t < 0)
					t = ( -b + UMath::Sqrt(d) ) / (2 * a);
				return dnonlynew STD_PAIR(bool, Real)(true, t);
			}


		}
		//-----------------------------------------------------------------------
		INSTANCE(STD_PAIR(bool, Real)) UMath::intersects(REF_CONST(Ray) ray, REF_CONST(AxisAlignedBox) box)
		{
			if (OF(box,isNull())) 
				return dnonlynew STD_PAIR(bool, Real)(false, 0.0f);
			if (OF(box,isInfinite()))
				return dnonlynew STD_PAIR(bool, Real)(true, 0.0f);

			Real lowt = 0.0f;
			Real t;
			bool hit = false;
			INSTANCE(Vector3) hitpoint = dnonlynew Vector3();
			REF_CONST(Vector3) min = OF(box,getMinimum());
			REF_CONST(Vector3) max = OF(box,getMaximum());
			REF_CONST(Vector3) rayorig = OF(ray,getOrigin());
			REF_CONST(Vector3) raydir = OF(ray,getDirection());

			// Check origin inside first
			if ( rayorig > min && rayorig < max )
			{
				return dnonlynew STD_PAIR(bool, Real)(true, 0.0f);
			}

			// Check each face in turn, only check closest 3
			// Min x
			if (OF(rayorig,x) <= OF(min,x) && OF(raydir,x) > 0)
			{
				t = (OF(min,x) - OF(rayorig,x)) / OF(raydir,x);
				if (t >= 0)
				{
					// Substitute t back into ray and check bounds and dist
					hitpoint = rayorig + raydir * t;
					if (OF(hitpoint,y) >= OF(min,y) && OF(hitpoint,y) <= OF(max,y) &&
						OF(hitpoint,z) >= OF(min,z) && OF(hitpoint,z) <= OF(max,z) &&
						(!hit || t < lowt))
					{
						hit = true;
						lowt = t;
					}
				}
			}
			// Max x
			if (OF(rayorig,x) >= OF(max,x) && OF(raydir,x) < 0)
			{
				t = (OF(max,x) - OF(rayorig,x)) / OF(raydir,x);
				if (t >= 0)
				{
					// Substitute t back into ray and check bounds and dist
					hitpoint = rayorig + raydir * t;
					if (OF(hitpoint,y) >= OF(min,y) && OF(hitpoint,y) <= OF(max,y) &&
						OF(hitpoint,z) >= OF(min,z) && OF(hitpoint,z) <= OF(max,z) &&
						(!hit || t < lowt))
					{
						hit = true;
						lowt = t;
					}
				}
			}
			// Min y
			if (OF(rayorig,y) <= OF(min,y) && OF(raydir,y) > 0)
			{
				t = (OF(min,y) - OF(rayorig,y)) / OF(raydir,y);
				if (t >= 0)
				{
					// Substitute t back into ray and check bounds and dist
					hitpoint = rayorig + raydir * t;
					if (OF(hitpoint,x) >= OF(min,x) && OF(hitpoint,x) <= OF(max,x) &&
						OF(hitpoint,z) >= OF(min,z) && OF(hitpoint,z) <= OF(max,z) &&
						(!hit || t < lowt))
					{
						hit = true;
						lowt = t;
					}
				}
			}
			// Max y
			if (OF(rayorig,y) >= OF(max,y) && OF(raydir,y) < 0)
			{
				t = (OF(max,y) - OF(rayorig,y)) / OF(raydir,y);
				if (t >= 0)
				{
					// Substitute t back into ray and check bounds and dist
					hitpoint = rayorig + raydir * t;
					if (OF(hitpoint,x) >= OF(min,x) && OF(hitpoint,x) <= OF(max,x) &&
						OF(hitpoint,z) >= OF(min,z) && OF(hitpoint,z) <= OF(max,z) &&
						(!hit || t < lowt))
					{
						hit = true;
						lowt = t;
					}
				}
			}
			// Min z
			if (OF(rayorig,z) <= OF(min,z) && OF(raydir,z) > 0)
			{
				t = (OF(min,z) - OF(rayorig,z)) / OF(raydir,z);
				if (t >= 0)
				{
					// Substitute t back into ray and check bounds and dist
					hitpoint = rayorig + raydir * t;
					if (OF(hitpoint,x) >= OF(min,x) && OF(hitpoint,x) <= OF(max,x) &&
						OF(hitpoint,y) >= OF(min,y) && OF(hitpoint,y) <= OF(max,y) &&
						(!hit || t < lowt))
					{
						hit = true;
						lowt = t;
					}
				}
			}
			// Max z
			if (OF(rayorig,z) >= OF(max,z) && OF(raydir,z) < 0)
			{
				t = (OF(max,z) - OF(rayorig,z)) / OF(raydir,z);
				if (t >= 0)
				{
					// Substitute t back into ray and check bounds and dist
					hitpoint = rayorig + raydir * t;
					if (OF(hitpoint,x) >= OF(min,x) && OF(hitpoint,x) <= OF(max,x) &&
						OF(hitpoint,y) >= OF(min,y) && OF(hitpoint,y) <= OF(max,y) &&
						(!hit || t < lowt))
					{
						hit = true;
						lowt = t;
					}
				}
			}

			return dnonlynew STD_PAIR(bool, Real)(hit, lowt);

		} 
		//-----------------------------------------------------------------------
		bool UMath::intersects(REF_CONST(Ray) ray, REF_CONST(AxisAlignedBox) box,
			Real* d1, Real* d2)
		{
			if (OF(box,isNull()))
				return false;

			if (OF(box,isInfinite()))
			{
				if (d1) *d1 = 0;
				if (d2) *d2 = UMath::POS_INFINITY;
				return true;
			}

			REF_CONST(Vector3) min = OF(box,getMinimum());
			REF_CONST(Vector3) max = OF(box,getMaximum());
			REF_CONST(Vector3) rayorig = OF(ray,getOrigin());
			REF_CONST(Vector3) raydir = OF(ray,getDirection());

			INSTANCE(Vector3) absDir = dnonlynew Vector3();
			absDir[0] = UMath::Abs(raydir[0]);
			absDir[1] = UMath::Abs(raydir[1]);
			absDir[2] = UMath::Abs(raydir[2]);

			// Sort the axis, ensure check minimise floating error axis first
			int imax = 0, imid = 1, imin = 2;
			if (absDir[0] < absDir[2])
			{
				imax = 2;
				imin = 0;
			}
			if (absDir[1] < absDir[imin])
			{
				imid = imin;
				imin = 1;
			}
			else if (absDir[1] > absDir[imax])
			{
				imid = imax;
				imax = 1;
			}

			Real start = 0, end = UMath::POS_INFINITY;

	#define _CALC_AXIS(i)                                       \
		do {                                                    \
			Real denom = 1 / raydir[i];                         \
			Real newstart = (min[i] - rayorig[i]) * denom;      \
			Real newend = (max[i] - rayorig[i]) * denom;        \
			if (newstart > newend) std::swap(newstart, newend); \
			if (newstart > end || newend < start) return false; \
			if (newstart > start) start = newstart;             \
			if (newend < end) end = newend;                     \
		} while(0)

			// Check each axis in turn

			_CALC_AXIS(imax);

			if (absDir[imid] < std::numeric_limits<Real>::epsilon())
			{
				// Parallel with middle and minimise axis, check bounds only
				if (rayorig[imid] < min[imid] || rayorig[imid] > max[imid] ||
					rayorig[imin] < min[imin] || rayorig[imin] > max[imin])
					return false;
			}
			else
			{
				_CALC_AXIS(imid);

				if (absDir[imin] < std::numeric_limits<Real>::epsilon())
				{
					// Parallel with minimise axis, check bounds only
					if (rayorig[imin] < min[imin] || rayorig[imin] > max[imin])
						return false;
				}
				else
				{
					_CALC_AXIS(imin);
				}
			}
	#undef _CALC_AXIS

			if (d1) *d1 = start;
			if (d2) *d2 = end;

			return true;
		}
		//-----------------------------------------------------------------------
		INSTANCE(STD_PAIR(bool, Real)) UMath::intersects(REF_CONST(Ray) ray, REF_CONST(Vector3) a,
			REF_CONST(Vector3) b, REF_CONST(Vector3) c, REF_CONST(Vector3) normal,
			bool positiveSide, bool negativeSide)
		{
			//
			// Calculate intersection with plane.
			//
			Real t;
			{
				Real denom = OF(normal,dotProduct(OF(ray,getDirection())));

				// Check intersect side
				if (denom > + std::numeric_limits<Real>::epsilon())
				{
					if (!negativeSide)
						return dnonlynew STD_PAIR(bool, Real)(false, 0.0f);
				}
				else if (denom < - std::numeric_limits<Real>::epsilon())
				{
					if (!positiveSide)
						return dnonlynew STD_PAIR(bool, Real)(false, 0.0f);
				}
				else
				{
					// Parallel or triangle area is close to zero when
					// the plane normal not normalised.
					return dnonlynew STD_PAIR(bool, Real)(false, 0.0f);
				}

				t = OF( normal,dotProduct(a - OF(ray,getOrigin()))) / denom;

				if (t < 0)
				{
					// Intersection is behind origin
					return dnonlynew STD_PAIR(bool, Real)(false, 0.0f);
				}
			}

			//
			// Calculate the largest area projection plane in X, Y or Z.
			//
			size_t i0, i1;
			{
				Real n0 = UMath::Abs(normal[0]);
				Real n1 = UMath::Abs(normal[1]);
				Real n2 = UMath::Abs(normal[2]);

				i0 = 1; i1 = 2;
				if (n1 > n2)
				{
					if (n1 > n0) i0 = 0;
				}
				else
				{
					if (n2 > n0) i1 = 0;
				}
			}

			//
			// Check the intersection point is inside the triangle.
			//
			{
				Real u1 = b[i0] - a[i0];
				Real v1 = b[i1] - a[i1];
				Real u2 = c[i0] - a[i0];
				Real v2 = c[i1] - a[i1];
				Real u0 = t * OF(ray,getDirection())[i0] + OF(ray,getOrigin())[i0] - a[i0];
				Real v0 = t * OF(ray,getDirection())[i1] + OF(ray,getOrigin())[i1] - a[i1];

				Real alpha = u0 * v2 - u2 * v0;
				Real beta  = u1 * v0 - u0 * v1;
				Real area  = u1 * v2 - u2 * v1;

				// epsilon to avoid float precision error
				const Real EPSILON = 1e-6f;

				Real tolerance = - EPSILON * area;

				if (area > 0)
				{
					if (alpha < tolerance || beta < tolerance || alpha+beta > area-tolerance)
						return dnonlynew STD_PAIR(bool, Real)(false, 0.0f);
				}
				else
				{
					if (alpha > tolerance || beta > tolerance || alpha+beta < area-tolerance)
						return dnonlynew STD_PAIR(bool, Real)(false, 0.0f);
				}
			}

			return dnonlynew STD_PAIR(bool, Real)(true, t);
		}
		//-----------------------------------------------------------------------
		INSTANCE(STD_PAIR(bool, Real)) UMath::intersects(REF_CONST(Ray) ray, REF_CONST(Vector3) a,
			REF_CONST(Vector3) b, REF_CONST(Vector3) c,
			bool positiveSide, bool negativeSide)
		{
			INSTANCE(Vector3) normal = calculateBasicFaceNormalWithoutNormalize(a, b, c);
			return intersects(ray, a, b, c, normal, positiveSide, negativeSide);
		}
		//-----------------------------------------------------------------------
		bool UMath::intersects(REF_CONST(Sphere) sphere, REF_CONST(AxisAlignedBox) box)
		{
			if (OF(box,isNull())) return false;
			if (OF(box,isInfinite())) return true;

			// Use splitting planes
			REF_CONST(Vector3) center = OF(sphere,getCenter());
			Real radius = OF(sphere,getRadius());
			REF_CONST(Vector3) min = OF(box,getMinimum());
			REF_CONST(Vector3) max = OF(box,getMaximum());


#ifdef DOTNET
			Real s, d = 0;
			for (int i = 0; i < 3; ++i)
			{
				if( center[i] < min[i])
				{
					s = center[i] - min[i];
					d += s * s; 
				}
				else if(center[i] > max[i])
				{
					s = center[i] - max[i];
					d += s * s; 
				}
			}
#else
			// Arvo's algorithm
			Real s, d = 0;
			for (int i = 0; i < 3; ++i)
			{
				if (OF(center,ptr())[i] < OF(min,ptr())[i])
				{
					s = OF(center,ptr())[i] - OF(min,ptr())[i];
					d += s * s; 
				}
				else if(OF(center,ptr())[i] > OF(max,ptr())[i])
				{
					s = OF(center,ptr())[i] - OF(max,ptr())[i];
					d += s * s; 
				}
			}
#endif
			return d <= radius * radius;

		}
		//-----------------------------------------------------------------------
		bool UMath::intersects(REF_CONST(Plane) plane, REF_CONST(AxisAlignedBox) box)
		{
			return (OF(plane,getSide(box)) == ENUM_OF(Side,BOTH_SIDE));
		}
		//-----------------------------------------------------------------------
		bool UMath::intersects(REF_CONST(Sphere) sphere, REF_CONST(Plane) plane)
		{
			return (
				UMath::Abs(OF(plane,getDistance(OF(sphere,getCenter()))))
				<= OF(sphere,getRadius()) );
		}
	

		//-----------------------------------------------------------------------
		INSTANCE(Vector3) UMath::calculateTangentSpaceVector(
			REF_CONST(Vector3) position1, REF_CONST(Vector3) position2, REF_CONST(Vector3) position3,
			Real u1, Real v1, Real u2, Real v2, Real u3, Real v3)
		{
			//side0 is the vector along one side of the triangle of vertices passed in, 
			//and side1 is the vector along another side. Taking the cross product of these returns the normal.
			INSTANCE(Vector3) side0 = position1 - position2;
			INSTANCE(Vector3) side1 = position3 - position1;
			//Calculate face normal
			INSTANCE(Vector3) normal = OF( side1 ,crossProduct(side0));
			OF(normal,normalise());
			//Now we use a formula to calculate the tangent. 
			Real deltaV0 = v1 - v2;
			Real deltaV1 = v3 - v1;
			INSTANCE(Vector3) tangent = deltaV1 * side0 - deltaV0 * side1;
			OF(tangent,normalise());
			//Calculate binormal
			Real deltaU0 = u1 - u2;
			Real deltaU1 = u3 - u1;
			INSTANCE(Vector3) binormal = deltaU1 * side0 - deltaU0 * side1;
			OF(binormal,normalise());
			//Now, we take the cross product of the tangents to get a vector which 
			//should point in the same direction as our normal calculated above. 
			//If it points in the opposite direction (the dot product between the normals is less than zero), 
			//then we need to reverse the s and t tangents. 
			//This is because the triangle has been mirrored when going from tangent space to object space.
			//reverse tangents if necessary
			INSTANCE(Vector3) tangentCross = OF(tangent,crossProduct(binormal));
			if (OF(tangentCross,dotProduct(normal)) < 0.0f)
			{
				tangent = -tangent;
				binormal = -binormal;
			}

			return tangent;

		}
		//-----------------------------------------------------------------------
		INSTANCE(Matrix4) UMath::buildReflectionMatrix(REF_CONST(Plane) p)
		{
			return dnonlynew Matrix4(
				-2 * OF(OF(p,normal),x) * OF(OF(p,normal),x) + 1,   -2 * OF(OF(p,normal),x) * OF(OF(p,normal),y),       -2 * OF(OF(p,normal),x) * OF(OF(p,normal),z),       -2 * OF(OF(p,normal),x) * OF(p,d), 
				-2 * OF(OF(p,normal),y) * OF(OF(p,normal),x),       -2 * OF(OF(p,normal),y) * OF(OF(p,normal),y) + 1,   -2 * OF(OF(p,normal),y) * OF(OF(p,normal),z),       -2 * OF(OF(p,normal),y) * OF(p,d), 
				-2 * OF(OF(p,normal),z) * OF(OF(p,normal),x),       -2 * OF(OF(p,normal),z) * OF(OF(p,normal),y),       -2 * OF(OF(p,normal),z) * OF(OF(p,normal),z) + 1,   -2 * OF(OF(p,normal),z) * OF(p,d), 
				0,                                  0,                                  0,                                  1);
		}
		//-----------------------------------------------------------------------
		INSTANCE(Vector4) UMath::calculateFaceNormal(REF_CONST(Vector3) v1, REF_CONST(Vector3) v2, REF_CONST(Vector3) v3)
		{
			INSTANCE(Vector3) normal = calculateBasicFaceNormal(v1, v2, v3);
			// Now set up the w (distance of tri from origin
			return dnonlynew Vector4(OF(normal,x), OF(normal,y), OF(normal,z), -(OF(normal,dotProduct(v1))));
		}
		//-----------------------------------------------------------------------
		INSTANCE(Vector3) UMath::calculateBasicFaceNormal(REF_CONST(Vector3) v1, REF_CONST(Vector3) v2, REF_CONST(Vector3) v3)
		{
			INSTANCE(Vector3) normal = OF( (v2 - v1) , crossProduct(v3 - v1) );
			OF(normal,normalise());
			return normal;
		}
		//-----------------------------------------------------------------------
		INSTANCE(Vector4) UMath::calculateFaceNormalWithoutNormalize(REF_CONST(Vector3) v1, REF_CONST(Vector3) v2, REF_CONST(Vector3) v3)
		{
			INSTANCE(Vector3) normal = calculateBasicFaceNormalWithoutNormalize(v1, v2, v3);
			// Now set up the w (distance of tri from origin)
			return dnonlynew Vector4(OF(normal,x), OF(normal,y), OF(normal,z), -(OF(normal,dotProduct(v1))));
		}
		//-----------------------------------------------------------------------
		INSTANCE(Vector3) UMath::calculateBasicFaceNormalWithoutNormalize(REF_CONST(Vector3) v1, REF_CONST(Vector3) v2, REF_CONST(Vector3) v3)
		{
			INSTANCE(Vector3) normal = OF( (v2 - v1) , crossProduct(v3 - v1));
			return normal;
		}
		//-----------------------------------------------------------------------
		Real UMath::gaussianDistribution(Real x, Real offset, Real scale)
		{
			Real nom = UMath::Exp(
				-UMath::Sqr(x - offset) / (2 * UMath::Sqr(scale)));
			Real denom = scale * UMath::Sqrt(2 * UMath::PI);

			return nom / denom;

		}
		//---------------------------------------------------------------------
		INSTANCE(Matrix4) UMath::makeViewMatrix(REF_CONST(Vector3) position, REF_CONST(Quaternion) orientation)
		{
			INSTANCE(Matrix4) mat = dnonlynew Matrix4(Matrix3::IDENTITY);
			return makeViewMatrix(position, orientation, mat);
		}
		//---------------------------------------------------------------------
		INSTANCE(Matrix4) UMath::makeViewMatrix(REF_CONST(Vector3) position, REF_CONST(Quaternion) orientation, REF(Matrix4) reflectMatrix)
		{
			{
				INSTANCE(Matrix4) viewMatrix = dnonlynew Matrix4();

				// View matrix is:
				//
				//  [ Lx  Uy  Dz  Tx  ]
				//  [ Lx  Uy  Dz  Ty  ]
				//  [ Lx  Uy  Dz  Tz  ]
				//  [ 0   0   0   1   ]
				//
				// Where T = -(Transposed(Rot) * Pos)

				// This is most efficiently done using 3x3 Matrices
				INSTANCE(Matrix3) rot = dnonlynew Matrix3();
				OF(orientation,ToRotationMatrix(rot));

				// Make the translation relative to new axes
				INSTANCE(Matrix3) rotT = OF(rot,Transpose());
				INSTANCE(Vector3) trans = -rotT * position;

				// Make final matrix
				viewMatrix = dnonlynew Matrix4(Matrix3::IDENTITY);
				viewMatrix = dnonlynew Matrix4(rotT); // fills upper 3x3
				IDX(viewMatrix,0,3) = OF(trans,x);
				IDX(viewMatrix,1,3) = OF(trans,y);
				IDX(viewMatrix,2,3) = OF(trans,z);

				// Deal with reflections
				viewMatrix = viewMatrix * reflectMatrix;
				return viewMatrix;
			}
		}
		//---------------------------------------------------------------------
		//INSTANCE(Matrix4) UMath::makeViewMatrix(REF_CONST(Vector3) position, REF_CONST(Quaternion) orientation, PTR_CONST(Matrix4) reflectMatrix)
		//{
		//	INSTANCE(Matrix4) viewMatrix = dnonlynew Matrix4();

		//	// View matrix is:
		//	//
		//	//  [ Lx  Uy  Dz  Tx  ]
		//	//  [ Lx  Uy  Dz  Ty  ]
		//	//  [ Lx  Uy  Dz  Tz  ]
		//	//  [ 0   0   0   1   ]
		//	//
		//	// Where T = -(Transposed(Rot) * Pos)

		//	// This is most efficiently done using 3x3 Matrices
		//	INSTANCE(Matrix3) rot = dnonlynew Matrix3();
		//	OF(orientation,ToRotationMatrix(rot));

		//	// Make the translation relative to new axes
		//	INSTANCE(Matrix3) rotT = OF(rot,Transpose());
		//	INSTANCE(Vector3) trans = -rotT * position;

		//	// Make final matrix
		//	viewMatrix = dnonlynew Matrix4(Matrix4::IDENTITY);
		//	viewMatrix = rotT; // fills upper 3x3
		//	IDX(viewMatrix,0,3) = OF(trans,x);
		//	IDX(viewMatrix,1,3) = OF(trans,y);
		//	IDX(viewMatrix,2,3) = OF(trans,z);

		//	// Deal with reflections
		//	if (reflectMatrix)
		//	{
		//		viewMatrix = viewMatrix * (*reflectMatrix);
		//	}

		//	return viewMatrix;

		//}
		//---------------------------------------------------------------------
		Real UMath::boundingRadiusFromAABB(REF_CONST(AxisAlignedBox) aabb)
		{
			INSTANCE(Vector3) max = dnonlynew Vector3(OF(aabb,getMaximum()));
			INSTANCE(Vector3) min = OF(aabb,getMinimum());

			INSTANCE(Vector3) magnitude = max;
			OF(magnitude,makeCeil(-max));
			OF(magnitude,makeCeil(min));
			OF(magnitude,makeCeil(-min));

			return OF(magnitude,length());
		}

	}
}
