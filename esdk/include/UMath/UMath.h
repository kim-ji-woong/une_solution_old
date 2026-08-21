#ifndef __UMATH_MATH_H__
#define __UMATH_MATH_H__

#include "UMathAPI.h"



namespace UnE
{
	namespace Math
	{
		UMATH_DECLARE_EXPORT_CLASS(Radian)
		{
		protected:
			Real mRad;

		public:
			Radian () : mRad(0) {  }
			Radian ( Real r ) : mRad(r) {  }
			Radian (  REF_CONST(Degree) d );
			Radian (  REF_CONST(Radian) r ){ mRad = OF(r, mRad); }
			REF(Radian) operator = ( const Real& f ) { mRad = f; return THIS_OBJ; }
			REF(Radian) operator = ( REF_CONST(Radian) r ) { mRad = OF(r,mRad); return THIS_OBJ; }
			REF(Radian) operator = ( REF_CONST(Degree) d );

			Real valueDegrees() CONSTF; // see bottom of this file
			Real valueRadians() CONSTF { return mRad; }
			Real valueAngleUnits() CONSTF;


#ifdef DOTNET // C++ CLI
			Radian( Radian% r){ mRad = r.mRad; }
			void operator=(Radian% r){ mRad = r.mRad; }

			static INSTANCE(Radian)  operator - (REF_CONST(Radian) r1) CONSTF { return dnonlynew Radian(- OF(r1,mRad)); }
			static REF_CONST(Radian) operator + (REF_CONST(Radian) r1) CONSTF { return r1; }

			static INSTANCE(Radian) operator + ( REF_CONST(Radian) r1, REF_CONST(Radian) r ) CONSTF 
			{ 
				return dnonlynew Radian ( OF(r1,mRad) + OF(r,mRad) );
			}
			static INSTANCE(Radian) operator + ( REF_CONST(Radian) r1, REF_CONST(Degree) d ) CONSTF;
			static INSTANCE(Radian) operator - ( REF_CONST(Radian) r1, REF_CONST(Radian) r ) CONSTF
			{
				return dnonlynew Radian ( OF(r1,mRad) - OF(r,mRad) );
			}
			static INSTANCE(Radian) operator - ( REF_CONST(Radian) r1, REF_CONST(Degree) d ) CONSTF;

			static INSTANCE(Radian) operator * ( REF_CONST(Radian) r1, Real f ) CONSTF
			{
				return dnonlynew Radian ( OF(r1,mRad) * f );
			}
			static INSTANCE(Radian) operator * ( Real a, REF_CONST(Radian) b )
			{
				return dnonlynew Radian ( a * OF(b,valueRadians()) );
			}
			static INSTANCE(Radian) operator * ( REF_CONST(Radian) r1, REF_CONST(Radian) f ) CONSTF 
			{
				return dnonlynew Radian ( OF(r1,mRad) * OF(f,mRad) ); 
			}
			static INSTANCE(Radian) operator / ( REF_CONST(Radian) r1, Real f ) CONSTF 
			{
				return dnonlynew Radian ( OF(r1,mRad) / f ); 
			}
			static INSTANCE(Radian) operator / ( Real a, REF_CONST(Radian) b )
			{
				return dnonlynew Radian ( a / OF(b,valueRadians()) );
			}
			static REF(Radian) operator += ( REF_CONST(Radian) r1, REF_CONST(Radian) r ) { OF(r1,mRad) += OF(r,mRad); return r1; }
			static REF(Radian) operator += ( REF_CONST(Radian) r1, REF_CONST(Degree) d );			
			static REF(Radian) operator -= ( REF_CONST(Radian) r1, REF_CONST(Radian) r ) { OF(r1,mRad) -= OF(r,mRad); return r1; }
			static REF(Radian) operator -= ( REF_CONST(Radian) r1, REF_CONST(Degree) d );			
			static REF(Radian) operator *= ( REF_CONST(Radian) r1, Real f ) { OF(r1,mRad) *= f; return r1; }					
			static REF(Radian) operator /= ( REF_CONST(Radian) r1, Real f ) { OF(r1,mRad) /= f; return r1; }

			static bool operator <  ( REF_CONST(Radian) r1, REF_CONST(Radian) r ) CONSTF { return OF(r1,mRad) <  OF(r,mRad); }
			static bool operator <= ( REF_CONST(Radian) r1, REF_CONST(Radian) r ) CONSTF { return OF(r1,mRad) <= OF(r,mRad); }
			static bool operator == ( REF_CONST(Radian) r1, REF_CONST(Radian) r ) CONSTF { return OF(r1,mRad) == OF(r,mRad); }
			static bool operator != ( REF_CONST(Radian) r1, REF_CONST(Radian) r ) CONSTF { return OF(r1,mRad) != OF(r,mRad); }
			static bool operator >= ( REF_CONST(Radian) r1, REF_CONST(Radian) r ) CONSTF { return OF(r1,mRad) >= OF(r,mRad); }
			static bool operator >  ( REF_CONST(Radian) r1, REF_CONST(Radian) r ) CONSTF { return OF(r1,mRad) >  OF(r,mRad); }

#else   // C++ Native
			REF_CONST(Radian) operator + () CONSTF { return THIS_OBJ; }
			INSTANCE(Radian) operator + ( REF_CONST(Radian) r ) CONSTF { return dnonlynew Radian ( mRad + OF(r,mRad) ); }
			INSTANCE(Radian) operator + ( REF_CONST(Degree) d ) CONSTF;
			REF(Radian) operator += ( REF_CONST(Radian) r ) { mRad += OF(r,mRad); return THIS_OBJ; }
			REF(Radian) operator += ( REF_CONST(Degree) d );
			INSTANCE(Radian) operator - () CONSTF { return dnonlynew Radian(-mRad); }
			INSTANCE(Radian) operator - ( REF_CONST(Radian) r ) CONSTF { return dnonlynew Radian ( mRad - OF(r,mRad) ); }
			INSTANCE(Radian) operator - ( REF_CONST(Degree) d ) CONSTF;
			REF(Radian) operator -= ( REF_CONST(Radian) r ) { mRad -= OF(r,mRad); return THIS_OBJ; }
			REF(Radian) operator -= ( REF_CONST(Degree) d );
			INSTANCE(Radian) operator * ( Real f ) CONSTF { return dnonlynew Radian ( mRad * f ); }
			INSTANCE(Radian) operator * ( REF_CONST(Radian) f ) CONSTF { return dnonlynew Radian ( mRad * OF(f,mRad) ); }
			REF(Radian) operator *= ( Real f ) { mRad *= f; return THIS_OBJ; }
			INSTANCE(Radian) operator / ( Real f ) CONSTF { return dnonlynew Radian ( mRad / f ); }
			REF(Radian) operator /= ( Real f ) { mRad /= f; return THIS_OBJ; }

			bool operator <  ( REF_CONST(Radian) r ) CONSTF { return mRad <  OF(r,mRad); }
			bool operator <= ( REF_CONST(Radian) r ) CONSTF { return mRad <= OF(r,mRad); }
			bool operator == ( REF_CONST(Radian) r ) CONSTF { return mRad == OF(r,mRad); }
			bool operator != ( REF_CONST(Radian) r ) CONSTF { return mRad != OF(r,mRad); }
			bool operator >= ( REF_CONST(Radian) r ) CONSTF { return mRad >= OF(r,mRad); }
			bool operator >  ( REF_CONST(Radian) r ) CONSTF { return mRad >  OF(r,mRad); }

			inline UMATH_API FRIEND std::ostream& operator <<( std::ostream& o, REF_CONST(Radian) v )
			{
				o << "Radian(" << OF(v,valueRadians()) << ")";
				return o;
			}
#endif
		};

		/** Wrapper class which indicates a given angle value is in Degrees.
		@remarks
			Degree values are interchangeable with Radian values, and conversions
			will be done automatically between them.
		*/
		UMATH_DECLARE_EXPORT_CLASS(Degree)
		{
		protected:
			Real mDeg; // if you get an error here - make sure to define/typedef 'Real' first

		public:
			explicit Degree () : mDeg(0) {}
			explicit Degree ( Real d ) : mDeg(d) {}
			Degree ( REF_CONST(Radian) r ) : mDeg(OF(r,valueDegrees())) {}
			Degree ( REF_CONST(Degree) d ){ mDeg = OF(d, mDeg); }
			REF(Degree)			operator = ( const Real& f ) { mDeg = f; return THIS_OBJ; }
			REF(Degree)			operator = ( REF_CONST(Degree) d ) { mDeg = OF(d,mDeg); return THIS_OBJ; }
			REF(Degree)			operator = ( REF_CONST(Radian) r ) { mDeg = OF(r,valueDegrees()); return THIS_OBJ; }

			Real valueDegrees() CONSTF { return mDeg; }
			Real valueRadians() CONSTF; // see bottom of this file
			Real valueAngleUnits() CONSTF;


#ifdef DOTNET
			Degree( Degree% d)
			{
				mDeg = d.mDeg;
			}
			void operator=(Degree% d) {
				mDeg = d.mDeg;
			}
			static REF_CONST(Degree) operator + (REF_CONST(Degree) d1) CONSTF { return d1; }
			static INSTANCE(Degree)	 operator + ( REF_CONST(Degree) d1, REF_CONST(Degree) d ) CONSTF 
			{
				return dnonlynew Degree ( OF(d1,mDeg)  + OF(d,mDeg) );
			}
			static INSTANCE(Degree)	 operator + ( REF_CONST(Degree) d1, REF_CONST(Radian) r ) CONSTF
			{
				return dnonlynew Degree ( OF(d1,mDeg) + OF(r,valueDegrees()) ); 
			}
			static REF(Degree)		 operator += ( REF_CONST(Degree) d1, REF_CONST(Degree) d ) 
			{ 
				OF(d1,mDeg) += OF(d,mDeg); 
				return d1;
			}
			static REF(Degree)		 operator += ( REF_CONST(Degree) d1,REF_CONST(Radian) r ) 
			{
				OF(d1,mDeg) += OF(r,valueDegrees()); return d1; 
			}
			static INSTANCE(Degree)	 operator - (REF_CONST(Degree) d1) CONSTF 
			{ 
				return dnonlynew Degree(-OF(d1,mDeg)); 
			}
			static INSTANCE(Degree)	 operator - ( REF_CONST(Degree) d1, REF_CONST(Degree) d ) CONSTF 
			{
				return dnonlynew Degree ( OF(d1,mDeg) - OF(d,mDeg) ); 
			}
			static INSTANCE(Degree)	 operator - ( REF_CONST(Degree) d1, REF_CONST(Radian) r ) CONSTF 
			{ 
				return dnonlynew Degree ( OF(d1,mDeg) - OF(r,valueDegrees()) );
			}
			static REF(Degree)		 operator -= ( REF_CONST(Degree) d1, REF_CONST(Degree) d )
			{
				OF(d1,mDeg) -= OF(d,mDeg); return d1; 
			}
			static REF(Degree)		 operator -= ( REF_CONST(Degree) d1, REF_CONST(Radian) r )
			{
				OF(d1,mDeg) -= OF(r,valueDegrees()); return d1;
			}
			static INSTANCE(Degree)	 operator * ( REF_CONST(Degree) d1, Real f ) CONSTF 
			{
				return dnonlynew Degree ( OF(d1,mDeg) * f );
			}
			static INSTANCE(Degree)	 operator * ( REF_CONST(Degree) d1, REF_CONST(Degree) f ) CONSTF 
			{
				return dnonlynew Degree ( OF(d1,mDeg) * OF(f,mDeg));
			}
			static REF(Degree)		 operator *= ( REF_CONST(Degree) d1, Real f ) 
			{ 
				OF(d1,mDeg) *= f; return d1; 
			}
			static INSTANCE(Degree)	 operator / ( REF_CONST(Degree) d1, Real f ) CONSTF 
			{ 
				return dnonlynew Degree ( OF(d1,mDeg) / f ); 
			}						
			static REF(Degree)		 operator /= ( REF_CONST(Degree) d1, Real f )
			{ 
				OF(d1,mDeg) /= f;
				return d1; 
			}
			static INSTANCE(Degree) operator * ( Real a, REF_CONST(Degree) b )
			{
				return dnonlynew Degree ( a * OF(b,valueDegrees()) );
			}
			static INSTANCE(Degree) operator / ( Real a, REF_CONST(Degree) b )
			{
				return dnonlynew Degree ( a / OF(b,valueDegrees()) );
			}
			static bool operator <  ( REF_CONST(Degree) d1, REF_CONST(Degree) d ) CONSTF { return OF(d1,mDeg) <  OF(d,mDeg); }
			static bool operator <= ( REF_CONST(Degree) d1, REF_CONST(Degree) d ) CONSTF { return OF(d1,mDeg) <= OF(d,mDeg); }
			static bool operator == ( REF_CONST(Degree) d1, REF_CONST(Degree) d ) CONSTF { return OF(d1,mDeg) == OF(d,mDeg); }
			static bool operator != ( REF_CONST(Degree) d1, REF_CONST(Degree) d ) CONSTF { return OF(d1,mDeg) != OF(d,mDeg); }
			static bool operator >= ( REF_CONST(Degree) d1, REF_CONST(Degree) d ) CONSTF { return OF(d1,mDeg) >= OF(d,mDeg); }
			static bool operator >  ( REF_CONST(Degree) d1, REF_CONST(Degree) d ) CONSTF { return OF(d1,mDeg) >  OF(d,mDeg); }
#else  // native
			REF_CONST(Degree)	operator + () CONSTF { return THIS_OBJ; }
			INSTANCE(Degree)	operator + ( REF_CONST(Degree) d ) CONSTF { return dnonlynew Degree ( mDeg + OF(d,mDeg) ); }
			INSTANCE(Degree)	operator + ( REF_CONST(Radian) r ) CONSTF { return dnonlynew Degree ( mDeg + OF(r,valueDegrees()) ); }
			REF(Degree)			operator += ( REF_CONST(Degree) d ) { mDeg += OF(d,mDeg); return THIS_OBJ; }
			REF(Degree)			operator += ( REF_CONST(Radian) r ) { mDeg += OF(r,valueDegrees()); return THIS_OBJ; }
			INSTANCE(Degree)	operator - () CONSTF { return dnonlynew Degree(-mDeg); }
			INSTANCE(Degree)	operator - ( REF_CONST(Degree) d ) CONSTF { return dnonlynew Degree ( mDeg - OF(d,mDeg) ); }
			INSTANCE(Degree)	operator - ( REF_CONST(Radian) r ) CONSTF { return dnonlynew Degree ( mDeg - OF(r,valueDegrees()) ); }
			REF(Degree)			operator -= ( REF_CONST(Degree) d ) { mDeg -= OF(d,mDeg); return THIS_OBJ; }
			REF(Degree)			operator -= ( REF_CONST(Radian) r ) { mDeg -= OF(r,valueDegrees()); return THIS_OBJ; }
			INSTANCE(Degree)	operator * ( Real f ) CONSTF { return dnonlynew Degree ( mDeg * f ); }
			INSTANCE(Degree)	operator * ( REF_CONST(Degree) f ) CONSTF { return dnonlynew Degree ( mDeg * OF(f,mDeg)); }
			REF(Degree)			operator *= ( Real f ) { mDeg *= f; return THIS_OBJ; }
			INSTANCE(Degree)	operator / ( Real f ) CONSTF { return dnonlynew Degree ( mDeg / f ); }
			REF(Degree)			operator /= ( Real f ) { mDeg /= f; return THIS_OBJ; }

			bool operator <  ( REF_CONST(Degree) d ) CONSTF { return mDeg <  OF(d,mDeg); }
			bool operator <= ( REF_CONST(Degree) d ) CONSTF { return mDeg <= OF(d,mDeg); }
			bool operator == ( REF_CONST(Degree) d ) CONSTF { return mDeg == OF(d,mDeg); }
			bool operator != ( REF_CONST(Degree) d ) CONSTF { return mDeg != OF(d,mDeg); }
			bool operator >= ( REF_CONST(Degree) d ) CONSTF { return mDeg >= OF(d,mDeg); }
			bool operator >  ( REF_CONST(Degree) d ) CONSTF { return mDeg >  OF(d,mDeg); }

			inline UMATH_API FRIEND std::ostream& operator <<( std::ostream& o, REF_CONST(Degree) v )
			{
				o << "Degree(" << OF(v,valueDegrees()) << ")";
				return o;
			}
#endif
		};

		/** Wrapper class which identifies a value as the currently default angle 
			type, as defined by Math::setAngleUnit.
		@remarks
			Angle values will be automatically converted between radians and degrees,
			as appropriate.
		*/
		UMATH_DECLARE_EXPORT_CLASS(Angle)
		{
			Real mAngle;
		public:
			explicit Angle ( Real angle ) : mAngle(angle) {}


#ifdef DOTNET
			USR_CONV operator Radian(CONV_TYPE(Angle,a)) CONSTF;			
			USR_CONV operator Degree(CONV_TYPE(Angle,a)) CONSTF;
#else
			operator Radian() CONSTF;
			operator Degree() CONSTF;
#endif
			//INSTANCE(Radian) operator Radian() CONSTF;
			//INSTANCE(Radian)operator Degree() CONSTF;
		};



		// these functions could not be defined within the class definition of class
		// Radian because they required class Degree to be defined
		inline Radian::Radian ( REF_CONST(Degree) d ) : mRad(OF(d,valueRadians())) { }
		inline REF(Radian) Radian::operator = ( REF_CONST(Degree) d ) {
			mRad = OF(d,valueRadians()); return THIS_OBJ;
		}

#ifdef DOTNET
		inline INSTANCE(Radian) Radian::operator + (REF_CONST(Radian) r1, REF_CONST(Degree) d ) CONSTF
		{
			return dnonlynew Radian ( OF(r1,mRad) + OF(d,valueRadians()) );
		}
		inline REF(Radian) Radian::operator += (REF_CONST(Radian) r1, REF_CONST(Degree) d )
		{
			OF(r1,mRad) += OF(d,valueRadians());
			return r1;
		}
		inline INSTANCE(Radian) Radian::operator - (REF_CONST(Radian) r1, REF_CONST(Degree) d ) CONSTF
		{
			return dnonlynew Radian (  OF(r1,mRad) -OF(d,valueRadians()) );
		}
		inline REF(Radian) Radian::operator -= (REF_CONST(Radian) r1, REF_CONST(Degree) d )
		{
			OF(r1,mRad) -= OF(d,valueRadians());
			return r1;
		}
#else
		inline INSTANCE(Radian) Radian::operator + (REF_CONST(Degree) d ) CONSTF
		{
			return dnonlynew Radian ( mRad + OF(d,valueRadians()) );
		}
		inline REF(Radian) Radian::operator += (REF_CONST(Degree) d )
		{
			mRad += OF(d,valueRadians());
			return THIS_OBJ;
		}
		inline INSTANCE(Radian) Radian::operator - (REF_CONST(Degree) d ) CONSTF
		{
			return dnonlynew Radian (  mRad -OF(d,valueRadians()) );
		}
		inline REF(Radian) Radian::operator -= (REF_CONST(Degree) d )
		{
			mRad -= OF(d,valueRadians());
			return THIS_OBJ;
		}
#endif
		

		/** Class to provide access to common mathematical functions.
			@remarks
				Most of the maths functions are aliased versions of the C runtime
				library functions. They are aliased here to provide future
				optimisation opportunities, either from faster RTLs or custom
				math approximations.
			@note
				<br>This is based on MgcMath.h from
				<a href="http://www.geometrictools.com/">Wild Magic</a>.
		*/
		UMATH_DECLARE_EXPORT_CLASS(UMath)
		{
	   public:
		   /** The angular units used by the API. This functionality is now deprecated in favor
			   of discreet angular unit types ( see Degree and Radian above ). The only place
			   this functionality is actually still used is when parsing files. Search for
			   usage of the Angle class for those instances
		   */
		   ENUM_CLASS AngleUnit
		   {
			   AU_DEGREE,
			   AU_RADIAN
		   };

		protected:
		   

			/** Private function to build trig tables.
			*/
			void buildTrigTables();

			static Real SinTable (Real fValue);
			static Real TanTable (Real fValue);
		public:
			/** Default constructor.
				@param
					trigTableSize Optional parameter to set the size of the
					tables used to implement Sin, Cos, Tan
			*/
			UMath(){ UMath(4096); }
			UMath(unsigned int trigTableSize);

			/** Default destructor.
			*/
			~UMath();

			static inline int IAbs (int iValue) { return ( iValue >= 0 ? iValue : -iValue ); }
			static inline int ICeil (float fValue) { return int(ceil(fValue)); }
			static inline int IFloor (float fValue) { return int(floor(fValue)); }
			static int ISign (int iValue);

			/** Absolute value function
				@param
					fValue The value whose absolute value will be returned.
			*/
			static inline Real Abs (Real fValue) { return Real(fabs(fValue)); }

			/** Absolute value function
				@param
					fValue The value, in degrees, whose absolute value will be returned.
			 */
			static inline INSTANCE(Degree) Abs (REF_CONST(Degree) dValue) { return dnonlynew Degree(fabs(OF(dValue,valueDegrees()))); }

			/** Absolute value function
				@param
					fValue The value, in radians, whose absolute value will be returned.
			 */
			static inline INSTANCE(Radian) Abs (REF_CONST(Radian) rValue) { return dnonlynew Radian(fabs(OF(rValue,valueRadians()))); }

			/** Arc cosine function
				@param
					fValue The value whose arc cosine will be returned.
			 */
			static INSTANCE(Radian) ACos (Real fValue);

			/** Arc sine function
				@param
					fValue The value whose arc sine will be returned.
			 */
			static INSTANCE(Radian) ASin (Real fValue);

			/** Arc tangent function
				@param
					fValue The value whose arc tangent will be returned.
			 */
			static inline INSTANCE(Radian) ATan (Real fValue) { return dnonlynew Radian(atan(fValue)); }

			/** Arc tangent between two values function
				@param
					fY The first value to calculate the arc tangent with.
				@param
					fX The second value to calculate the arc tangent with.
			 */
			static inline INSTANCE(Radian) ATan2 (Real fY, Real fX) { return dnonlynew Radian(atan2(fY,fX)); }

			/** Ceiling function
				Returns the smallest following integer. (example: Ceil(1.1) = 2)

				@param
					fValue The value to round up to the nearest integer.
			 */
			static inline Real Ceil (Real fValue) { return Real(ceil(fValue)); }
			static inline bool isNaN(Real f)
			{
				// std::isnan() is C99, not supported by all compilers
				// However NaN always fails this next test, no other number does.
				return f != f;
			}

			/** Cosine function.
				@param
					fValue Angle in radians
				@param
					useTables If true, uses lookup tables rather than
					calculation - faster but less accurate.
			*/
			static inline Real Cos (REF_CONST(Radian) fValue)
			{
				return Cos(fValue, false);
			}
			static inline Real Cos (REF_CONST(Radian) fValue, bool useTables)
			{
				return (!useTables) ? Real(cos(OF(fValue,valueRadians()))) : SinTable(OF(fValue,valueRadians()) + HALF_PI);
			}
			/** Cosine function.
				@param
					fValue Angle in radians
				@param
					useTables If true, uses lookup tables rather than
					calculation - faster but less accurate.
			*/
			static inline Real Cos (Real fValue)
			{
				return Cos(fValue, false);
			}
			static inline Real Cos (Real fValue, bool useTables)
			{
				return (!useTables) ? Real(cos(fValue)) : SinTable(fValue + HALF_PI);
			}

			static inline Real Exp (Real fValue) { return Real(exp(fValue)); }

			/** Floor function
				Returns the largest previous integer. (example: Floor(1.9) = 1)
		 
				@param
					fValue The value to round down to the nearest integer.
			 */
			static inline Real Floor (Real fValue) { return Real(floor(fValue)); }

			static inline Real Log (Real fValue) { return Real(log(fValue)); }

			/// Stored value of log(2) for frequent use
			static CONST Real LOG2;

			static inline Real Log2 (Real fValue) { return Real(log(fValue)/LOG2); }

			static inline Real LogN (Real base, Real fValue) { return Real(log(fValue)/log(base)); }

			static inline Real Pow (Real fBase, Real fExponent) { return Real(pow(fBase,fExponent)); }

			static Real Sign (Real fValue);
			static inline INSTANCE(Radian) Sign ( REF_CONST(Radian) rValue )
			{
				return dnonlynew Radian(Sign(OF(rValue,valueRadians())));
			}
			static inline INSTANCE(Degree) Sign ( REF_CONST(Degree) dValue )
			{
				return dnonlynew Degree(Sign(OF(dValue,valueDegrees())));
			}

			/** Sine function.
				@param
					fValue Angle in radians
				@param
					useTables If true, uses lookup tables rather than
					calculation - faster but less accurate.
			*/
			static inline Real Sin (REF_CONST(Radian) fValue)
			{
				return Sin(fValue, false);
			}
			static inline Real Sin (REF_CONST(Radian) fValue, bool useTables)
			{
				return (!useTables) ? Real(sin(OF(fValue,valueRadians()))) : SinTable(OF(fValue,valueRadians()));
			}

			/** Sine function.
				@param
					fValue Angle in radians
				@param
					useTables If true, uses lookup tables rather than
					calculation - faster but less accurate.
			*/
			static inline Real Sin (Real fValue)
			{
				return Sin(fValue, false);
			}
			static inline Real Sin (Real fValue, bool useTables)
			{
				return (!useTables) ? Real(sin(fValue)) : SinTable(fValue);
			}

			/** Squared function.
				@param
					fValue The value to be squared (fValue^2)
			*/
			static inline Real Sqr (Real fValue) { return fValue*fValue; }

			/** Square root function.
				@param
					fValue The value whose square root will be calculated.
			 */
			static inline Real Sqrt (Real fValue) { return Real(sqrt(fValue)); }

			/** Square root function.
				@param
					fValue The value, in radians, whose square root will be calculated.
				@return
					The square root of the angle in radians.
			 */
			static inline INSTANCE(Radian) Sqrt (REF_CONST(Radian) fValue) { return dnonlynew Radian(sqrt(OF(fValue,valueRadians()))); }

			/** Square root function.
				@param
					fValue The value, in degrees, whose square root will be calculated.
				@return
					The square root of the angle in degrees.
			 */
			static inline INSTANCE(Degree) Sqrt (REF_CONST(Degree) fValue) { return dnonlynew Degree(sqrt(OF(fValue,valueDegrees()))); }

			/** Inverse square root i.e. 1 / Sqrt(x), good for vector
				normalisation.
				@param
					fValue The value whose inverse square root will be calculated.
			*/
			static Real InvSqrt (Real fValue);

			/** Generate a random number of unit length.
				@return
					A random number in the range from [0,1].
			*/
			static Real UnitRandom ();

			/** Generate a random number within the range provided.
				@param
					fLow The lower bound of the range.
				@param
					fHigh The upper bound of the range.
				@return
					A random number in the range from [fLow,fHigh].
			 */
			static Real RangeRandom (Real fLow, Real fHigh);

			/** Generate a random number in the range [-1,1].
				@return
					A random number in the range from [-1,1].
			 */
			static Real SymmetricRandom ();

			/** Tangent function.
				@param
					fValue Angle in radians
				@param
					useTables If true, uses lookup tables rather than
					calculation - faster but less accurate.
			*/
			static inline Real Tan (REF_CONST(Radian) fValue) 
			{
				return Tan(fValue, false);
			}
			static inline Real Tan (REF_CONST(Radian) fValue, bool useTables)
			{
				return (!useTables) ? Real(tan(OF(fValue,valueRadians()))) : TanTable(OF(fValue,valueRadians()));
			}
			/** Tangent function.
				@param
					fValue Angle in radians
				@param
					useTables If true, uses lookup tables rather than
					calculation - faster but less accurate.
			*/
			static inline Real Tan (Real fValue)
			{
				return Tan(fValue, false);
			}
			static inline Real Tan (Real fValue, bool useTables)
			{
				return (!useTables) ? Real(tan(fValue)) : TanTable(fValue);
			}

			static inline Real DegreesToRadians(Real degrees) { return degrees * fDeg2Rad; }
			static inline Real RadiansToDegrees(Real radians) { return radians * fRad2Deg; }

		   /** These functions used to set the assumed angle units (radians or degrees) 
				expected when using the Angle type.
		   @par
				You can set this directly after creating a new Root, and also before/after resource creation,
				depending on whether you want the change to affect resource files.
		   */
		   static void setAngleUnit(AngleUnit unit);
		   /** Get the unit being used for angles. */
		   static AngleUnit getAngleUnit(void);

		   /** Convert from the current AngleUnit to radians. */
		   static Real AngleUnitsToRadians(Real units);
		   /** Convert from radians to the current AngleUnit . */
		   static Real RadiansToAngleUnits(Real radians);
		   /** Convert from the current AngleUnit to degrees. */
		   static Real AngleUnitsToDegrees(Real units);
		   /** Convert from degrees to the current AngleUnit. */
		   static Real DegreesToAngleUnits(Real degrees);

		   /** Checks whether a given point is inside a triangle, in a
				2-dimensional (Cartesian) space.
				@remarks
					The vertices of the triangle must be given in either
					trigonometrical (anticlockwise) or inverse trigonometrical
					(clockwise) order.
				@param
					p The point.
				@param
					a The triangle's first vertex.
				@param
					b The triangle's second vertex.
				@param
					c The triangle's third vertex.
				@return
					If the point resides in the triangle, <b>true</b> is
					returned.
				@par
					If the point is outside the triangle, <b>false</b> is
					returned.
			*/
			static bool pointInTri2D(REF_CONST(Vector2) p, REF_CONST(Vector2) a, 
				REF_CONST(Vector2) b, REF_CONST(Vector2) c);

		   /** Checks whether a given 3D point is inside a triangle.
		   @remarks
				The vertices of the triangle must be given in either
				trigonometrical (anticlockwise) or inverse trigonometrical
				(clockwise) order, and the point must be guaranteed to be in the
				same plane as the triangle
			@param
				p The point.
			@param
				a The triangle's first vertex.
			@param
				b The triangle's second vertex.
			@param
				c The triangle's third vertex.
			@param 
				normal The triangle plane's normal (passed in rather than calculated
					on demand since the caller may already have it)
			@return
				If the point resides in the triangle, <b>true</b> is
				returned.
			@par
				If the point is outside the triangle, <b>false</b> is
				returned.
			*/
			static bool pointInTri3D(REF_CONST(Vector3) p, REF_CONST(Vector3) a, 
				REF_CONST(Vector3) b, REF_CONST(Vector3) c,REF_CONST(Vector3) normal);
			
			/** Ray / plane intersection, returns boolean result and distance. */
			static INSTANCE( STD_PAIR(bool, Real)) intersects(REF_CONST(Ray) ray, REF_CONST(Plane) plane);

			/** Ray / sphere intersection, returns boolean result and distance. */
			static INSTANCE( STD_PAIR(bool, Real)) intersects(REF_CONST(Ray) ray, REF_CONST(Sphere) sphere)
			{
				return intersects(ray, sphere, true);
			}
			static INSTANCE( STD_PAIR(bool, Real)) intersects(REF_CONST(Ray) ray, REF_CONST(Sphere) sphere, bool discardInside);
		
			/** Ray / box intersection, returns boolean result and distance. */
			static INSTANCE( STD_PAIR(bool, Real)) intersects(REF_CONST(Ray) ray, REF_CONST(AxisAlignedBox) box);

			/** Ray / box intersection, returns boolean result and two intersection distance.
			@param
				ray The ray.
			@param
				box The box.
			@param
				d1 A real pointer to retrieve the near intersection distance
					from the ray origin, maybe <b>null</b> which means don't care
					about the near intersection distance.
			@param
				d2 A real pointer to retrieve the far intersection distance
					from the ray origin, maybe <b>null</b> which means don't care
					about the far intersection distance.
			@return
				If the ray is intersects the box, <b>true</b> is returned, and
				the near intersection distance is return by <i>d1</i>, the
				far intersection distance is return by <i>d2</i>. Guarantee
				<b>0</b> <= <i>d1</i> <= <i>d2</i>.
			@par
				If the ray isn't intersects the box, <b>false</b> is returned, and
				<i>d1</i> and <i>d2</i> is unmodified.
			*/
			static bool intersects(REF_CONST(Ray) ray, REF_CONST(AxisAlignedBox) box,
				Real* d1, Real* d2);

			/** Ray / triangle intersection, returns boolean result and distance.
			@param
				ray The ray.
			@param
				a The triangle's first vertex.
			@param
				b The triangle's second vertex.
			@param
				c The triangle's third vertex.
			@param 
				normal The triangle plane's normal (passed in rather than calculated
					on demand since the caller may already have it), doesn't need
					normalised since we don't care.
			@param
				positiveSide Intersect with "positive side" of the triangle
			@param
				negativeSide Intersect with "negative side" of the triangle
			@return
				If the ray is intersects the triangle, a pair of <b>true</b> and the
				distance between intersection point and ray origin returned.
			@par
				If the ray isn't intersects the triangle, a pair of <b>false</b> and
				<b>0</b> returned.
			*/

			static INSTANCE( STD_PAIR(bool, Real)) intersects(REF_CONST(Ray) ray, REF_CONST(Vector3) a,
				REF_CONST(Vector3) b, REF_CONST(Vector3) c, REF_CONST(Vector3) normal)
			{
				return intersects(ray, a, b, c, normal, true, true);
			}
			static INSTANCE( STD_PAIR(bool, Real)) intersects(REF_CONST(Ray) ray, REF_CONST(Vector3) a,
				REF_CONST(Vector3) b, REF_CONST(Vector3) c, REF_CONST(Vector3) normal,
				bool positiveSide)
			{
				return intersects(ray, a, b, c, normal, positiveSide, true);
			}

			static INSTANCE( STD_PAIR(bool, Real)) intersects(REF_CONST(Ray) ray, REF_CONST(Vector3) a,
				REF_CONST(Vector3) b, REF_CONST(Vector3) c, REF_CONST(Vector3) normal,
				bool positiveSide, bool negativeSide);

			/** Ray / triangle intersection, returns boolean result and distance.
			@param
				ray The ray.
			@param
				a The triangle's first vertex.
			@param
				b The triangle's second vertex.
			@param
				c The triangle's third vertex.
			@param
				positiveSide Intersect with "positive side" of the triangle
			@param
				negativeSide Intersect with "negative side" of the triangle
			@return
				If the ray is intersects the triangle, a pair of <b>true</b> and the
				distance between intersection point and ray origin returned.
			@par
				If the ray isn't intersects the triangle, a pair of <b>false</b> and
				<b>0</b> returned.
			*/
			static INSTANCE( STD_PAIR(bool, Real)) intersects(REF_CONST(Ray) ray, REF_CONST(Vector3) a,
				REF_CONST(Vector3) b, REF_CONST(Vector3) c)
			{
				return intersects(ray, a, b, c, true, true);
			}
			static INSTANCE( STD_PAIR(bool, Real)) intersects(REF_CONST(Ray) ray, REF_CONST(Vector3) a,
				REF_CONST(Vector3) b, REF_CONST(Vector3) c, bool positiveSide)
			{
				return intersects(ray, a, b, c, positiveSide, true);
			}

			static INSTANCE( STD_PAIR(bool, Real)) intersects(REF_CONST(Ray) ray, REF_CONST(Vector3) a,
				REF_CONST(Vector3) b, REF_CONST(Vector3) c,
				bool positiveSide, bool negativeSide);

			/** Sphere / box intersection test. */
			static bool intersects(REF_CONST(Sphere) sphere, REF_CONST(AxisAlignedBox) box);

			/** Plane / box intersection test. */
			static bool intersects(REF_CONST(Plane) plane, REF_CONST(AxisAlignedBox) box);

			/** Ray / convex plane list intersection test. 
			@param ray The ray to test with
			@param plaeList List of planes which form a convex volume
			@param normalIsOutside Does the normal point outside the volume
			*/
			static INSTANCE( STD_PAIR(bool, Real)) intersects(
				REF_CONST(Ray) ray, REF_CONST(STD_VECTOR(INSTANCE(Plane))) planeList, 
				bool normalIsOutside);
			/** Ray / convex plane list intersection test. 
			@param ray The ray to test with
			@param plaeList List of planes which form a convex volume
			@param normalIsOutside Does the normal point outside the volume
			*/
			static INSTANCE( STD_PAIR(bool, Real)) intersects(
				REF_CONST(Ray) ray, REF_CONST(STD_LIST(INSTANCE(Plane))) planeList, 
				bool normalIsOutside);

			/** Sphere / plane intersection test. 
			@remarks NB just do a plane.getDistance(sphere.getCenter()) for more detail!
			*/
			static bool intersects(REF_CONST(Sphere) sphere, REF_CONST(Plane) plane);

			/** Compare 2 reals, using tolerance for inaccuracies.
			*/
			static bool RealEqual(Real a, Real b)
			{
				return RealEqual(a,b, std::numeric_limits<Real>::epsilon());
			}

			static bool RealEqual(Real a, Real b, Real tolerance);

			/** Calculates the tangent space vector for a given set of positions / texture coords. */
			static INSTANCE(Vector3) calculateTangentSpaceVector(
				REF_CONST(Vector3) position1, REF_CONST(Vector3) position2, REF_CONST(Vector3) position3,
				Real u1, Real v1, Real u2, Real v2, Real u3, Real v3);

			/** Build a reflection matrix for the passed in plane. */
			static INSTANCE(Matrix4) buildReflectionMatrix(REF_CONST(Plane) p);
			/** Calculate a face normal, including the w component which is the offset from the origin. */
			static INSTANCE(Vector4) calculateFaceNormal(REF_CONST(Vector3) v1, REF_CONST(Vector3) v2, REF_CONST(Vector3) v3);
			/** Calculate a face normal, no w-information. */
			static INSTANCE(Vector3) calculateBasicFaceNormal(REF_CONST(Vector3) v1, REF_CONST(Vector3) v2, REF_CONST(Vector3) v3);
			/** Calculate a face normal without normalize, including the w component which is the offset from the origin. */
			static INSTANCE(Vector4) calculateFaceNormalWithoutNormalize(REF_CONST(Vector3) v1, REF_CONST(Vector3) v2, REF_CONST(Vector3) v3);
			/** Calculate a face normal without normalize, no w-information. */
			static INSTANCE(Vector3) calculateBasicFaceNormalWithoutNormalize(REF_CONST(Vector3) v1, REF_CONST(Vector3) v2, REF_CONST(Vector3) v3);

			/** Generates a value based on the Gaussian (normal) distribution function
				with the given offset and scale parameters.
			*/
			static Real gaussianDistribution(Real x)
			{
				return gaussianDistribution(x,  0.0f, 1.0f);
			}
			static Real gaussianDistribution(Real x, Real offset)
			{
				return gaussianDistribution(x, offset, 1.0f);
			}
			static Real gaussianDistribution(Real x, Real offset, Real scale);

			/** Clamp a value within an inclusive range. */
			template <typename T>
			static T Clamp(T val, T minval, T maxval)
			{
				assert (minval <= maxval && "Invalid clamp range");
				return std::max<T>(std::min<T>(val, maxval), minval);
			}


			static INSTANCE(Matrix4) makeViewMatrix(REF_CONST(Vector3) position, REF_CONST(Quaternion) orientation, REF(Matrix4) reflectMatrix);
			static INSTANCE(Matrix4) makeViewMatrix(REF_CONST(Vector3) position, REF_CONST(Quaternion) orientation);

			/** Get a bounding radius value from a bounding box. */
			static Real boundingRadiusFromAABB(REF_CONST(AxisAlignedBox) aabb);


			static CONST Real POS_INFINITY	SC_VALUE(std::numeric_limits<Real>::infinity());
			static CONST Real NEG_INFINITY	SC_VALUE(-std::numeric_limits<Real>::infinity());
			static CONST Real PI			SC_VALUE(Real( 3.14159265358979323846 ));
			static CONST Real TWO_PI		SC_VALUE(Real( 2.0 * PI ));
			static CONST Real HALF_PI		SC_VALUE(Real( 0.5 * PI ));
			static CONST Real fDeg2Rad		SC_VALUE(PI / Real(180.0));
			static CONST Real fRad2Deg		SC_VALUE(Real(180.0) / PI);
			
			
			
			
			// angle units used by the api
			static AngleUnit msAngleUnit;
			/// Size of the trig tables as determined by constructor.
			static int mTrigTableSize;
			/// Radian -> index factor value ( mTrigTableSize / 2 * PI )
			static Real mTrigTableFactor;

#ifdef DOTNET
			static array<Real>^ mSinTable = nullptr; 
			static array<Real>^ mTanTable = nullptr; 
#else
			static Real* mSinTable;
			static Real* mTanTable;
#endif

		};

		// these functions must be defined down here, because they rely on the
		// angle unit conversion functions in class Math:

		inline Real Radian::valueDegrees() CONSTF
		{
			return UMath::RadiansToDegrees ( mRad );
		}

		inline Real Radian::valueAngleUnits() CONSTF
		{
			return UMath::RadiansToAngleUnits ( mRad );
		}

		inline Real Degree::valueRadians() CONSTF
		{
			return UMath::DegreesToRadians ( mDeg );
		}

		inline Real Degree::valueAngleUnits() CONSTF
		{
			return UMath::DegreesToAngleUnits ( mDeg );
		}


		inline Angle::operator Radian(CONV_TYPE(Angle,a)) CONSTF
		{
#ifdef DOTNET
			return Radian(UMath::AngleUnitsToRadians(OF(a,mAngle)));
#else
			return Radian(UMath::AngleUnitsToRadians(mAngle));
#endif
		}

		inline Angle::operator Degree(CONV_TYPE(Angle,a)) CONSTF
		{
#ifdef DOTNET
			return Degree( UMath::AngleUnitsToDegrees(OF(a,mAngle)));
#else
			return Degree( UMath::AngleUnitsToDegrees(mAngle));
#endif
		}

#ifndef DOTNET
		inline INSTANCE(Radian) operator * ( Real a, REF_CONST(Radian) b )
		{
			return dnonlynew Radian ( a * OF(b,valueRadians()) );
		}

		inline INSTANCE(Radian) operator / ( Real a, REF_CONST(Radian) b )
		{
			return dnonlynew Radian ( a / OF(b,valueRadians()) );
		}

		inline INSTANCE(Degree) operator * ( Real a, REF_CONST(Degree) b )
		{
			return dnonlynew Degree ( a * OF(b,valueDegrees()) );
		}

		inline INSTANCE(Degree) operator / ( Real a, REF_CONST(Degree) b )
		{
			return dnonlynew Degree ( a / OF(b,valueDegrees()) );
		}
#endif
		/** @} */
		/** @} */
	}
}
#endif
