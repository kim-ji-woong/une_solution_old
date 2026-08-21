/*
-----------------------------------------------------------------------------
This source file is part of OGRE
(Object-oriented Graphics Rendering Engine)
For the latest info, see http://www.ogre3d.org/

Copyright (c) 2000-2012 Torus Knot Software Ltd

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
-----------------------------------------------------------------------------
*/
#include "stdafx.h"

#include "UMatrix4.h"
#include "UMath.h"



namespace UnE
{
	namespace Math
	{
#ifndef DOTNET
		const Matrix4 Matrix4::ZERO(
			0, 0, 0, 0,
			0, 0, 0, 0,
			0, 0, 0, 0,
			0, 0, 0, 0 );
	
		const Matrix4 Matrix4::ZEROAFFINE(
		0, 0, 0, 0,
		0, 0, 0, 0,
		0, 0, 0, 0,
		0, 0, 0, 1 );

		const Matrix4 Matrix4::IDENTITY(
			1, 0, 0, 0,
			0, 1, 0, 0,
			0, 0, 1, 0,
			0, 0, 0, 1 );

		const Matrix4 Matrix4::CLIPSPACE2DTOIMAGESPACE(
			0.5,    0,  0, 0.5, 
			  0, -0.5,  0, 0.5, 
			  0,    0,  1,   0,
			  0,    0,  0,   1);
#endif
		//-----------------------------------------------------------------------
		static Real MINOR(REF_CONST(Matrix4) m, const size_t r0, const size_t r1, const size_t r2, 
									const size_t c0, const size_t c1, const size_t c2)
		{
			return 
				IDX(OF(m,m),r0,c0) * (IDX(OF(m,m),r1,c1) * IDX(OF(m,m),r2,c2) - IDX(OF(m,m),r2,c1) * IDX(OF(m,m),r1,c2)) -
				IDX(OF(m,m),r0,c1) * (IDX(OF(m,m),r1,c0) * IDX(OF(m,m),r2,c2) - IDX(OF(m,m),r2,c0) * IDX(OF(m,m),r1,c2)) +
				IDX(OF(m,m),r0,c2) * (IDX(OF(m,m),r1,c0) * IDX(OF(m,m),r2,c1) - IDX(OF(m,m),r2,c0) * IDX(OF(m,m),r1,c1));
		}
		//-----------------------------------------------------------------------
		INSTANCE(Matrix4) Matrix4::adjoint() CONSTF
		{
			return dnonlynew Matrix4( MINOR(THIS_OBJ, 1, 2, 3, 1, 2, 3),
				-MINOR(THIS_OBJ, 0, 2, 3, 1, 2, 3),
				MINOR(THIS_OBJ, 0, 1, 3, 1, 2, 3),
				-MINOR(THIS_OBJ, 0, 1, 2, 1, 2, 3),

				-MINOR(THIS_OBJ, 1, 2, 3, 0, 2, 3),
				MINOR(THIS_OBJ, 0, 2, 3, 0, 2, 3),
				-MINOR(THIS_OBJ, 0, 1, 3, 0, 2, 3),
				MINOR(THIS_OBJ, 0, 1, 2, 0, 2, 3),

				MINOR(THIS_OBJ, 1, 2, 3, 0, 1, 3),
				-MINOR(THIS_OBJ, 0, 2, 3, 0, 1, 3),
				MINOR(THIS_OBJ, 0, 1, 3, 0, 1, 3),
				-MINOR(THIS_OBJ, 0, 1, 2, 0, 1, 3),

				-MINOR(THIS_OBJ, 1, 2, 3, 0, 1, 2),
				MINOR(THIS_OBJ, 0, 2, 3, 0, 1, 2),
				-MINOR(THIS_OBJ, 0, 1, 3, 0, 1, 2),
				MINOR(THIS_OBJ, 0, 1, 2, 0, 1, 2));
		}
		//-----------------------------------------------------------------------
		Real Matrix4::determinant() CONSTF
		{
			return IDX(m,0,0) * MINOR(THIS_OBJ, 1, 2, 3, 1, 2, 3) -
				IDX(m,0,1) * MINOR(THIS_OBJ, 1, 2, 3, 0, 2, 3) +
				IDX(m,0,2) * MINOR(THIS_OBJ, 1, 2, 3, 0, 1, 3) -
				IDX(m,0,3) * MINOR(THIS_OBJ, 1, 2, 3, 0, 1, 2);
		}
		//-----------------------------------------------------------------------
		INSTANCE(Matrix4) Matrix4::inverse() CONSTF
		{
			Real m00 = IDX(m,0,0), m01 = IDX(m,0,1), m02 = IDX(m,0,2), m03 = IDX(m,0,3);
			Real m10 = IDX(m,1,0), m11 = IDX(m,1,1), m12 = IDX(m,1,2), m13 = IDX(m,1,3);
			Real m20 = IDX(m,2,0), m21 = IDX(m,2,1), m22 = IDX(m,2,2), m23 = IDX(m,2,3);
			Real m30 = IDX(m,3,0), m31 = IDX(m,3,1), m32 = IDX(m,3,2), m33 = IDX(m,3,3);

			Real v0 = m20 * m31 - m21 * m30;
			Real v1 = m20 * m32 - m22 * m30;
			Real v2 = m20 * m33 - m23 * m30;
			Real v3 = m21 * m32 - m22 * m31;
			Real v4 = m21 * m33 - m23 * m31;
			Real v5 = m22 * m33 - m23 * m32;

			Real t00 = + (v5 * m11 - v4 * m12 + v3 * m13);
			Real t10 = - (v5 * m10 - v2 * m12 + v1 * m13);
			Real t20 = + (v4 * m10 - v2 * m11 + v0 * m13);
			Real t30 = - (v3 * m10 - v1 * m11 + v0 * m12);

			Real invDet = 1 / (t00 * m00 + t10 * m01 + t20 * m02 + t30 * m03);

			Real d00 = t00 * invDet;
			Real d10 = t10 * invDet;
			Real d20 = t20 * invDet;
			Real d30 = t30 * invDet;

			Real d01 = - (v5 * m01 - v4 * m02 + v3 * m03) * invDet;
			Real d11 = + (v5 * m00 - v2 * m02 + v1 * m03) * invDet;
			Real d21 = - (v4 * m00 - v2 * m01 + v0 * m03) * invDet;
			Real d31 = + (v3 * m00 - v1 * m01 + v0 * m02) * invDet;

			v0 = m10 * m31 - m11 * m30;
			v1 = m10 * m32 - m12 * m30;
			v2 = m10 * m33 - m13 * m30;
			v3 = m11 * m32 - m12 * m31;
			v4 = m11 * m33 - m13 * m31;
			v5 = m12 * m33 - m13 * m32;

			Real d02 = + (v5 * m01 - v4 * m02 + v3 * m03) * invDet;
			Real d12 = - (v5 * m00 - v2 * m02 + v1 * m03) * invDet;
			Real d22 = + (v4 * m00 - v2 * m01 + v0 * m03) * invDet;
			Real d32 = - (v3 * m00 - v1 * m01 + v0 * m02) * invDet;

			v0 = m21 * m10 - m20 * m11;
			v1 = m22 * m10 - m20 * m12;
			v2 = m23 * m10 - m20 * m13;
			v3 = m22 * m11 - m21 * m12;
			v4 = m23 * m11 - m21 * m13;
			v5 = m23 * m12 - m22 * m13;

			Real d03 = - (v5 * m01 - v4 * m02 + v3 * m03) * invDet;
			Real d13 = + (v5 * m00 - v2 * m02 + v1 * m03) * invDet;
			Real d23 = - (v4 * m00 - v2 * m01 + v0 * m03) * invDet;
			Real d33 = + (v3 * m00 - v1 * m01 + v0 * m02) * invDet;

			return dnonlynew Matrix4(
				d00, d01, d02, d03,
				d10, d11, d12, d13,
				d20, d21, d22, d23,
				d30, d31, d32, d33);
		}
		//-----------------------------------------------------------------------
		INSTANCE(Matrix4) Matrix4::inverseAffine(void) CONSTF
		{
			assert(isAffine());

			Real m10 = IDX(m,1,0), m11 = IDX(m,1,1), m12 = IDX(m,1,2);
			Real m20 = IDX(m,2,0), m21 = IDX(m,2,1), m22 = IDX(m,2,2);

			Real t00 = m22 * m11 - m21 * m12;
			Real t10 = m20 * m12 - m22 * m10;
			Real t20 = m21 * m10 - m20 * m11;

			Real m00 = IDX(m,0,0), m01 = IDX(m,0,1), m02 = IDX(m,0,2);

			Real invDet = 1 / (m00 * t00 + m01 * t10 + m02 * t20);

			t00 *= invDet; t10 *= invDet; t20 *= invDet;

			m00 *= invDet; m01 *= invDet; m02 *= invDet;

			Real r00 = t00;
			Real r01 = m02 * m21 - m01 * m22;
			Real r02 = m01 * m12 - m02 * m11;

			Real r10 = t10;
			Real r11 = m00 * m22 - m02 * m20;
			Real r12 = m02 * m10 - m00 * m12;

			Real r20 = t20;
			Real r21 = m01 * m20 - m00 * m21;
			Real r22 = m00 * m11 - m01 * m10;

			Real m03 = IDX(m,0,3), m13 = IDX(m,1,3), m23 = IDX(m,2,3);

			Real r03 = - (r00 * m03 + r01 * m13 + r02 * m23);
			Real r13 = - (r10 * m03 + r11 * m13 + r12 * m23);
			Real r23 = - (r20 * m03 + r21 * m13 + r22 * m23);

			return dnonlynew Matrix4(
				r00, r01, r02, r03,
				r10, r11, r12, r13,
				r20, r21, r22, r23,
				  0,   0,   0,   1);
		}
		//-----------------------------------------------------------------------
		void Matrix4::makeTransform(REF_CONST(Vector3) position, REF_CONST(Vector3) scale, REF_CONST(Quaternion) orientation)
		{
			// Ordering:
			//    1. Scale
			//    2. Rotate
			//    3. Translate

			INSTANCE(Matrix3) rot3x3 = dnonlynew Matrix3();
			OF(orientation,ToRotationMatrix(rot3x3));

			// Set up final matrix with scale, rotation and translation
			IDX(m,0,0) = OF(scale,x) * IDX(rot3x3,0,0); IDX(m,0,1) = OF(scale,y) * IDX(rot3x3,0,1); IDX(m,0,2) = OF(scale,z) * IDX(rot3x3,0,2); IDX(m,0,3) = OF(position,x);
			IDX(m,1,0) = OF(scale,x) * IDX(rot3x3,1,0); IDX(m,1,1) = OF(scale,y) * IDX(rot3x3,1,1); IDX(m,1,2) = OF(scale,z) * IDX(rot3x3,1,2); IDX(m,1,3) = OF(position,y);
			IDX(m,2,0) = OF(scale,x) * IDX(rot3x3,2,0); IDX(m,2,1) = OF(scale,y) * IDX(rot3x3,2,1); IDX(m,2,2) = OF(scale,z) * IDX(rot3x3,2,2); IDX(m,2,3) = OF(position,z);

			// No projection term
			IDX(m,3,0) = 0; IDX(m,3,1) = 0; IDX(m,3,2) = 0; IDX(m,3,3) = 1;
		}
		//-----------------------------------------------------------------------
		void Matrix4::makeInverseTransform(REF_CONST(Vector3) position, REF_CONST(Vector3) scale, REF_CONST(Quaternion) orientation)
		{
			// Invert the parameters
			INSTANCE(Vector3) invTranslate = -position;
			INSTANCE(Vector3) invScale = dnonlynew Vector3(1 / OF(scale,x), 1 / OF(scale,y), 1 / OF(scale,z));
			INSTANCE(Quaternion) invRot = OF(orientation,Inverse());

			// Because we're inverting, order is translation, rotation, scale
			// So make translation relative to scale & rotation
			invTranslate = invRot * invTranslate; // rotate
			invTranslate *= invScale; // scale

			// Next, make a 3x3 rotation matrix
			INSTANCE(Matrix3) rot3x3 = dnonlynew Matrix3();
			OF(invRot,ToRotationMatrix(rot3x3));

			// Set up final matrix with scale, rotation and translation
			IDX(m,0,0) = OF(invScale,x) * IDX(rot3x3,0,0); IDX(m,0,1) = OF(invScale,x) * IDX(rot3x3,0,1); IDX(m,0,2) = OF(invScale,x) * IDX(rot3x3,0,2); IDX(m,0,3) = OF(invTranslate,x);
			IDX(m,1,0) = OF(invScale,y) * IDX(rot3x3,1,0); IDX(m,1,1) = OF(invScale,y) * IDX(rot3x3,1,1); IDX(m,1,2) = OF(invScale,y) * IDX(rot3x3,1,2); IDX(m,1,3) = OF(invTranslate,y);
			IDX(m,2,0) = OF(invScale,z) * IDX(rot3x3,2,0); IDX(m,2,1) = OF(invScale,z) * IDX(rot3x3,2,1); IDX(m,2,2) = OF(invScale,z) * IDX(rot3x3,2,2); IDX(m,2,3) = OF(invTranslate,z);		

			// No projection term
			IDX(m,3,0) = 0; IDX(m,3,1) = 0; IDX(m,3,2) = 0; IDX(m,3,3) = 1;
		}
		//-----------------------------------------------------------------------
		void Matrix4::decomposition(REF(Vector3) position, REF(Vector3) scale, REF(Quaternion) orientation) CONSTF
		{
			assert(isAffine());

			INSTANCE(Matrix3) m3x3 = dnonlynew Matrix3();
			extract3x3Matrix(m3x3);

			INSTANCE(Matrix3) matQ = dnonlynew Matrix3();
			INSTANCE(Vector3) vecU = dnonlynew Vector3();
			OF( m3x3, QDUDecomposition( matQ, scale, vecU ));

			orientation = dnonlynew Quaternion( matQ );
			position = dnonlynew Vector3( IDX(m,0,3), IDX(m,1,3), IDX(m,2,3) );
		}	




		void Matrix4::operator=( REF_CONST(Matrix3) mat3 )
		{
			IDX(m,0,0) = IDX(mat3,0,0); IDX(m,0,1) = IDX(mat3,0,1); IDX(m,0,2) = IDX(mat3,0,2);
			IDX(m,1,0) = IDX(mat3,1,0); IDX(m,1,1) = IDX(mat3,1,1); IDX(m,1,2) = IDX(mat3,1,2);
			IDX(m,2,0) = IDX(mat3,2,0); IDX(m,2,1) = IDX(mat3,2,1); IDX(m,2,2) = IDX(mat3,2,2);
		}

#ifdef DOTNET		
	
		inline INSTANCE(Vector4) Matrix4::operator * (REF_CONST(Vector4) v, REF_CONST(Matrix4) mat)
		{
			return dnonlynew Vector4(
				OF(v,x)*IDX(mat,0,0) + OF(v,y)*IDX(mat,1,0) + OF(v,z)*IDX(mat,2,0) + OF(v,w)*IDX(mat,3,0),
				OF(v,x)*IDX(mat,0,1) + OF(v,y)*IDX(mat,1,1) + OF(v,z)*IDX(mat,2,1) + OF(v,w)*IDX(mat,3,1),
				OF(v,x)*IDX(mat,0,2) + OF(v,y)*IDX(mat,1,2) + OF(v,z)*IDX(mat,2,2) + OF(v,w)*IDX(mat,3,2),
				OF(v,x)*IDX(mat,0,3) + OF(v,y)*IDX(mat,1,3) + OF(v,z)*IDX(mat,2,3) + OF(v,w)*IDX(mat,3,3)
				);
		}

		INSTANCE(Matrix4) Matrix4::operator * ( REF_CONST(Matrix4) m, Real scalar) CONSTF
		{
			return dnonlynew Matrix4(
				scalar*IDX(m,0,0), scalar*IDX(m,0,1), scalar*IDX(m,0,2), scalar*IDX(m,0,3),
				scalar*IDX(m,1,0), scalar*IDX(m,1,1), scalar*IDX(m,1,2), scalar*IDX(m,1,3),
				scalar*IDX(m,2,0), scalar*IDX(m,2,1), scalar*IDX(m,2,2), scalar*IDX(m,2,3),
				scalar*IDX(m,3,0), scalar*IDX(m,3,1), scalar*IDX(m,3,2), scalar*IDX(m,3,3));
		}

		INSTANCE(Plane) Matrix4::operator*( REF_CONST(Matrix4) m, REF_CONST(Plane) p) CONSTF
		{
			INSTANCE(Plane) ret = dnonlynew Plane();
			INSTANCE(Matrix4) invTrans = OF(OF(m, inverse()),transpose());
			INSTANCE(Vector4) v4 = dnonlynew Vector4( OF(OF(p,normal),x), OF(OF(p,normal),y), OF(OF(p,normal),z), OF(p,d) );
			v4 = invTrans * v4;
			OF(OF(ret,normal),x) = OF(v4,x); 
			OF(OF(ret,normal),y) = OF(v4,y); 
			OF(OF(ret,normal),z) = OF(v4,z);
			OF(ret,d) = OF(v4,w) / OF( OF(ret,normal) , normalise());
			return ret;
		}

		INSTANCE(Vector4) Matrix4::operator*( REF_CONST(Matrix4) m, REF_CONST(Vector4) v) CONSTF
		{
			return dnonlynew Vector4(
				IDX(m,0,0) * OF(v,x) + IDX(m,0,1) * OF(v,y) + IDX(m,0,2) * OF(v,z) + IDX(m,0,3) * OF(v,w),
				IDX(m,1,0) * OF(v,x) + IDX(m,1,1) * OF(v,y) + IDX(m,1,2) * OF(v,z) + IDX(m,1,3) * OF(v,w),
				IDX(m,2,0) * OF(v,x) + IDX(m,2,1) * OF(v,y) + IDX(m,2,2) * OF(v,z) + IDX(m,2,3) * OF(v,w),
				IDX(m,3,0) * OF(v,x) + IDX(m,3,1) * OF(v,y) + IDX(m,3,2) * OF(v,z) + IDX(m,3,3) * OF(v,w)
				);
		}

		INSTANCE(Vector3) Matrix4::operator*( REF_CONST(Matrix4) m, REF_CONST(Vector3) v ) CONSTF
		{
			INSTANCE(Vector3) r = dnonlynew Vector3();

			Real fInvW = 1.0f / ( IDX(m,3,0) * OF(v,x) + IDX(m,3,1) * OF(v,y) + IDX(m,3,2) * OF(v,z) + IDX(m,3,3) );

			OF(r,x) = ( IDX(m,0,0) * OF(v,x) + IDX(m,0,1) * OF(v,y) + IDX(m,0,2) * OF(v,z) + IDX(m,0,3) ) * fInvW;
			OF(r,y) = ( IDX(m,1,0) * OF(v,x) + IDX(m,1,1) * OF(v,y) + IDX(m,1,2) * OF(v,z) + IDX(m,1,3) ) * fInvW;
			OF(r,z) = ( IDX(m,2,0) * OF(v,x) + IDX(m,2,1) * OF(v,y) + IDX(m,2,2) * OF(v,z) + IDX(m,2,3) ) * fInvW;

			return r;
		}

		INSTANCE(Matrix4) Matrix4::operator*( REF_CONST(Matrix4) m, REF_CONST(Matrix4) m2) CONSTF 
		{
			return OF(m , concatenate( m2 ));
		}

		bool Matrix4::operator!=( REF_CONST(Matrix4) m, REF_CONST(Matrix4) m2 ) CONSTF
		{
			if( 
				IDX(m,0,0) != IDX(m2,0,0) || IDX(m,0,1) != IDX(m2,0,1) || IDX(m,0,2) != IDX(m2,0,2) || IDX(m,0,3) != IDX(m2,0,3) ||
				IDX(m,1,0) != IDX(m2,1,0) || IDX(m,1,1) != IDX(m2,1,1) || IDX(m,1,2) != IDX(m2,1,2) || IDX(m,1,3) != IDX(m2,1,3) ||
				IDX(m,2,0) != IDX(m2,2,0) || IDX(m,2,1) != IDX(m2,2,1) || IDX(m,2,2) != IDX(m2,2,2) || IDX(m,2,3) != IDX(m2,2,3) ||
				IDX(m,3,0) != IDX(m2,3,0) || IDX(m,3,1) != IDX(m2,3,1) || IDX(m,3,2) != IDX(m2,3,2) || IDX(m,3,3) != IDX(m2,3,3) )
				return true;
			return false;
		}

		bool Matrix4::operator==( REF_CONST(Matrix4) m, REF_CONST(Matrix4) m2 ) CONSTF
		{
			if( 
				IDX(m,0,0) != IDX(m2,0,0) || IDX(m,0,1) != IDX(m2,0,1) || IDX(m,0,2) != IDX(m2,0,2) || IDX(m,0,3) != IDX(m2,0,3) ||
				IDX(m,1,0) != IDX(m2,1,0) || IDX(m,1,1) != IDX(m2,1,1) || IDX(m,1,2) != IDX(m2,1,2) || IDX(m,1,3) != IDX(m2,1,3) ||
				IDX(m,2,0) != IDX(m2,2,0) || IDX(m,2,1) != IDX(m2,2,1) || IDX(m,2,2) != IDX(m2,2,2) || IDX(m,2,3) != IDX(m2,2,3) ||
				IDX(m,3,0) != IDX(m2,3,0) || IDX(m,3,1) != IDX(m2,3,1) || IDX(m,3,2) != IDX(m2,3,2) || IDX(m,3,3) != IDX(m2,3,3) )
				return false;
			return true;
		}

		INSTANCE(Matrix4) Matrix4::operator-( REF_CONST(Matrix4) m, REF_CONST(Matrix4) m2 ) CONSTF
		{
			INSTANCE(Matrix4) r = dnonlynew Matrix4();
			IDX(r,0,0) = IDX(m,0,0) - IDX(m2,0,0);
			IDX(r,0,1) = IDX(m,0,1) - IDX(m2,0,1);
			IDX(r,0,2) = IDX(m,0,2) - IDX(m2,0,2);
			IDX(r,0,3) = IDX(m,0,3) - IDX(m2,0,3);

			IDX(r,1,0) = IDX(m,1,0) - IDX(m2,1,0);
			IDX(r,1,1) = IDX(m,1,1) - IDX(m2,1,1);
			IDX(r,1,2) = IDX(m,1,2) - IDX(m2,1,2);
			IDX(r,1,3) = IDX(m,1,3) - IDX(m2,1,3);

			IDX(r,2,0) = IDX(m,2,0) - IDX(m2,2,0);
			IDX(r,2,1) = IDX(m,2,1) - IDX(m2,2,1);
			IDX(r,2,2) = IDX(m,2,2) - IDX(m2,2,2);
			IDX(r,2,3) = IDX(m,2,3) - IDX(m2,2,3);

			IDX(r,3,0) = IDX(m,3,0) - IDX(m2,3,0);
			IDX(r,3,1) = IDX(m,3,1) - IDX(m2,3,1);
			IDX(r,3,2) = IDX(m,3,2) - IDX(m2,3,2);
			IDX(r,3,3) = IDX(m,3,3) - IDX(m2,3,3);

			return r;
		}

		INSTANCE(Matrix4) Matrix4::operator+( REF_CONST(Matrix4) m, REF_CONST(Matrix4) m2 ) CONSTF
		{
			INSTANCE(Matrix4) r = dnonlynew Matrix4();

			IDX(r,0,0) = IDX(m,0,0) + IDX(m2,0,0);
			IDX(r,0,1) = IDX(m,0,1) + IDX(m2,0,1);
			IDX(r,0,2) = IDX(m,0,2) + IDX(m2,0,2);
			IDX(r,0,3) = IDX(m,0,3) + IDX(m2,0,3);

			IDX(r,1,0) = IDX(m,1,0) + IDX(m2,1,0);
			IDX(r,1,1) = IDX(m,1,1) + IDX(m2,1,1);
			IDX(r,1,2) = IDX(m,1,2) + IDX(m2,1,2);
			IDX(r,1,3) = IDX(m,1,3) + IDX(m2,1,3);

			IDX(r,2,0) = IDX(m,2,0) + IDX(m2,2,0);
			IDX(r,2,1) = IDX(m,2,1) + IDX(m2,2,1);
			IDX(r,2,2) = IDX(m,2,2) + IDX(m2,2,2);
			IDX(r,2,3) = IDX(m,2,3) + IDX(m2,2,3);

			IDX(r,3,0) = IDX(m,3,0) + IDX(m2,3,0);
			IDX(r,3,1) = IDX(m,3,1) + IDX(m2,3,1);
			IDX(r,3,2) = IDX(m,3,2) + IDX(m2,3,2);
			IDX(r,3,3) = IDX(m,3,3) + IDX(m2,3,3);

			return r;
		}
#else
		
		INSTANCE(Matrix4) Matrix4::operator*( Real scalar) CONSTF
		{
			return dnonlynew Matrix4(
				scalar*IDX(m,0,0), scalar*IDX(m,0,1), scalar*IDX(m,0,2), scalar*IDX(m,0,3),
				scalar*IDX(m,1,0), scalar*IDX(m,1,1), scalar*IDX(m,1,2), scalar*IDX(m,1,3),
				scalar*IDX(m,2,0), scalar*IDX(m,2,1), scalar*IDX(m,2,2), scalar*IDX(m,2,3),
				scalar*IDX(m,3,0), scalar*IDX(m,3,1), scalar*IDX(m,3,2), scalar*IDX(m,3,3));
		}

		INSTANCE(Plane) Matrix4::operator*( REF_CONST(Plane) p) CONSTF
		{
			INSTANCE(Plane) ret = dnonlynew Plane();
			INSTANCE(Matrix4) invTrans = OF(inverse(),transpose());
			INSTANCE(Vector4) v4 = dnonlynew Vector4( OF(OF(p,normal),x), OF(OF(p,normal),y), OF(OF(p,normal),z), OF(p,d) );
			v4 = invTrans * v4;
			OF(OF(ret,normal),x) = OF(v4,x); 
			OF(OF(ret,normal),y) = OF(v4,y); 
			OF(OF(ret,normal),z) = OF(v4,z);
			OF(ret,d) = OF(v4,w) / OF( OF(ret,normal) , normalise());
			return ret;
		}

		INSTANCE(Vector4) Matrix4::operator*( REF_CONST(Vector4) v) CONSTF
		{
			return dnonlynew Vector4(
				IDX(m,0,0) * OF(v,x) + IDX(m,0,1) * OF(v,y) + IDX(m,0,2) * OF(v,z) + IDX(m,0,3) * OF(v,w),
				IDX(m,1,0) * OF(v,x) + IDX(m,1,1) * OF(v,y) + IDX(m,1,2) * OF(v,z) + IDX(m,1,3) * OF(v,w),
				IDX(m,2,0) * OF(v,x) + IDX(m,2,1) * OF(v,y) + IDX(m,2,2) * OF(v,z) + IDX(m,2,3) * OF(v,w),
				IDX(m,3,0) * OF(v,x) + IDX(m,3,1) * OF(v,y) + IDX(m,3,2) * OF(v,z) + IDX(m,3,3) * OF(v,w)
				);
		}

		INSTANCE(Vector3) Matrix4::operator*( REF_CONST(Vector3) v ) CONSTF
		{
			INSTANCE(Vector3) r = dnonlynew Vector3();

			Real fInvW = 1.0f / ( IDX(m,3,0) * OF(v,x) + IDX(m,3,1) * OF(v,y) + IDX(m,3,2) * OF(v,z) + IDX(m,3,3) );

			OF(r,x) = ( IDX(m,0,0) * OF(v,x) + IDX(m,0,1) * OF(v,y) + IDX(m,0,2) * OF(v,z) + IDX(m,0,3) ) * fInvW;
			OF(r,y) = ( IDX(m,1,0) * OF(v,x) + IDX(m,1,1) * OF(v,y) + IDX(m,1,2) * OF(v,z) + IDX(m,1,3) ) * fInvW;
			OF(r,z) = ( IDX(m,2,0) * OF(v,x) + IDX(m,2,1) * OF(v,y) + IDX(m,2,2) * OF(v,z) + IDX(m,2,3) ) * fInvW;

			return r;
		}
						
		INSTANCE(Matrix4) Matrix4::operator*( REF_CONST(Matrix4) m2) CONSTF 
		{
			return concatenate( m2 );
		}

		bool Matrix4::operator!=( REF_CONST(Matrix4) m2 ) CONSTF
		{
			if( 
				IDX(m,0,0) != IDX(m2,0,0) || IDX(m,0,1) != IDX(m2,0,1) || IDX(m,0,2) != IDX(m2,0,2) || IDX(m,0,3) != IDX(m2,0,3) ||
				IDX(m,1,0) != IDX(m2,1,0) || IDX(m,1,1) != IDX(m2,1,1) || IDX(m,1,2) != IDX(m2,1,2) || IDX(m,1,3) != IDX(m2,1,3) ||
				IDX(m,2,0) != IDX(m2,2,0) || IDX(m,2,1) != IDX(m2,2,1) || IDX(m,2,2) != IDX(m2,2,2) || IDX(m,2,3) != IDX(m2,2,3) ||
				IDX(m,3,0) != IDX(m2,3,0) || IDX(m,3,1) != IDX(m2,3,1) || IDX(m,3,2) != IDX(m2,3,2) || IDX(m,3,3) != IDX(m2,3,3) )
				return true;
			return false;
		}

		bool Matrix4::operator==( REF_CONST(Matrix4) m2 ) CONSTF
		{
			if( 
				IDX(m,0,0) != IDX(m2,0,0) || IDX(m,0,1) != IDX(m2,0,1) || IDX(m,0,2) != IDX(m2,0,2) || IDX(m,0,3) != IDX(m2,0,3) ||
				IDX(m,1,0) != IDX(m2,1,0) || IDX(m,1,1) != IDX(m2,1,1) || IDX(m,1,2) != IDX(m2,1,2) || IDX(m,1,3) != IDX(m2,1,3) ||
				IDX(m,2,0) != IDX(m2,2,0) || IDX(m,2,1) != IDX(m2,2,1) || IDX(m,2,2) != IDX(m2,2,2) || IDX(m,2,3) != IDX(m2,2,3) ||
				IDX(m,3,0) != IDX(m2,3,0) || IDX(m,3,1) != IDX(m2,3,1) || IDX(m,3,2) != IDX(m2,3,2) || IDX(m,3,3) != IDX(m2,3,3) )
				return false;
			return true;
		}

		INSTANCE(Matrix4) Matrix4::operator-( REF_CONST(Matrix4) m2 ) CONSTF
		{
			INSTANCE(Matrix4) r = dnonlynew Matrix4();
			IDX(r,0,0) = IDX(m,0,0) - IDX(m2,0,0);
			IDX(r,0,1) = IDX(m,0,1) - IDX(m2,0,1);
			IDX(r,0,2) = IDX(m,0,2) - IDX(m2,0,2);
			IDX(r,0,3) = IDX(m,0,3) - IDX(m2,0,3);

			IDX(r,1,0) = IDX(m,1,0) - IDX(m2,1,0);
			IDX(r,1,1) = IDX(m,1,1) - IDX(m2,1,1);
			IDX(r,1,2) = IDX(m,1,2) - IDX(m2,1,2);
			IDX(r,1,3) = IDX(m,1,3) - IDX(m2,1,3);

			IDX(r,2,0) = IDX(m,2,0) - IDX(m2,2,0);
			IDX(r,2,1) = IDX(m,2,1) - IDX(m2,2,1);
			IDX(r,2,2) = IDX(m,2,2) - IDX(m2,2,2);
			IDX(r,2,3) = IDX(m,2,3) - IDX(m2,2,3);

			IDX(r,3,0) = IDX(m,3,0) - IDX(m2,3,0);
			IDX(r,3,1) = IDX(m,3,1) - IDX(m2,3,1);
			IDX(r,3,2) = IDX(m,3,2) - IDX(m2,3,2);
			IDX(r,3,3) = IDX(m,3,3) - IDX(m2,3,3);

			return r;
		}

		INSTANCE(Matrix4) Matrix4::operator+( REF_CONST(Matrix4) m2 ) CONSTF
		{
			INSTANCE(Matrix4) r = dnonlynew Matrix4();

			IDX(r,0,0) = IDX(m,0,0) + IDX(m2,0,0);
			IDX(r,0,1) = IDX(m,0,1) + IDX(m2,0,1);
			IDX(r,0,2) = IDX(m,0,2) + IDX(m2,0,2);
			IDX(r,0,3) = IDX(m,0,3) + IDX(m2,0,3);

			IDX(r,1,0) = IDX(m,1,0) + IDX(m2,1,0);
			IDX(r,1,1) = IDX(m,1,1) + IDX(m2,1,1);
			IDX(r,1,2) = IDX(m,1,2) + IDX(m2,1,2);
			IDX(r,1,3) = IDX(m,1,3) + IDX(m2,1,3);

			IDX(r,2,0) = IDX(m,2,0) + IDX(m2,2,0);
			IDX(r,2,1) = IDX(m,2,1) + IDX(m2,2,1);
			IDX(r,2,2) = IDX(m,2,2) + IDX(m2,2,2);
			IDX(r,2,3) = IDX(m,2,3) + IDX(m2,2,3);

			IDX(r,3,0) = IDX(m,3,0) + IDX(m2,3,0);
			IDX(r,3,1) = IDX(m,3,1) + IDX(m2,3,1);
			IDX(r,3,2) = IDX(m,3,2) + IDX(m2,3,2);
			IDX(r,3,3) = IDX(m,3,3) + IDX(m2,3,3);

			return r;
		}
#endif

	}
}

