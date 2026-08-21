#pragma once

#include "GeometryAPI.h"

#ifndef GP_POINT2D
#define GP_DIR2D(Vertex2DObj)	gp_Dir2d(OF(Vertex2DObj, x), OF(Vertex2DObj, y))
#define GP_POINT2D(Vertex2DObj) gp_Pnt2d(OF(Vertex2DObj, x), OF(Vertex2DObj, y))
#define GP_POINT3D(Vertex3DObj) gp_Pnt(OF(Vertex3DObj, x), OF(Vertex3DObj, y), OF(Vertex3DObj, z))
#endif

namespace UnE
{
	namespace Geometry
	{
		template <class Real>
		//GEOMETRY_DECLARE_EXPORT_CLASS(Vertex3) ABSTRACT
		TEMPLATE_DECLARE_CLASS(Vertex3) ABSTRACT
		{
		public:
			Vertex3()
			{
				x = y = z = 0.0f;
			}

			Vertex3(Real x, Real y, Real z)
			{
				SetVertex(x, y, z);
			}

		public:
			void SetVertex(Real x, Real y, Real z)
			{
				this->x = x;
				this->y = y;
				this->z = z;
			}

			void CopyFrom(REF_CONST(Vertex3) rhs)
			{
				this->x = OF(rhs, x);
				this->y = OF(rhs, y);
				this->z = OF(rhs, z);
			}

		public:
			virtual Real GetDistance(REF_CONST(Vertex3) rVertex) CONST = 0;

		public:
			Real x, y, z;
		};

		template <class Real>
		//GEOMETRY_DECLARE_EXPORT_CLASS(Vertex2) ABSTRACT
		TEMPLATE_DECLARE_CLASS(Vertex2) ABSTRACT
		{
		public:
			Vertex2()
			{
				x = y = 0.0f;
			}

			Vertex2(Real x, Real y)
			{
				SetVertex(x, y);
			}

		public:
			void SetVertex(Real x, Real y)
			{
				this->x = x;
				this->y = y;
			}

			void CopyFrom(REF_CONST(Vertex2) rhs)
			{
				this->x = OF(rhs, x);
				this->y = OF(rhs, y);
			}

		public:
			virtual Real GetDistance(REF_CONST(Vertex2) rVertex) CONST = 0;

		public:
			Real x, y;
		};

		GEOMETRY_EXPORT_CLASS(Vertex3D) : PUBLIC Vertex3<double>
		{
		public:
			Vertex3D();
			Vertex3D(Vertex3DRefConst rhs);
			Vertex3D(double x, double y, double z);
			virtual ~Vertex3D(void);

		public:
#ifdef DOTNET
			//static bool operator== (Vertex3D^ op1, Vertex3D^ op2);
			//static bool operator!= (Vertex3D^ op1, Vertex3D^ op2);
			static Vertex3D^ operator+ (Vertex3D^ op1, Vertex3D^ op2);
			static Vertex3D^ operator- (Vertex3D^ op1, Vertex3D^ op2);
			static Vertex3D^ operator* (Vertex3D^ op, double data);
			static Vertex3D^ operator/ (Vertex3D^ op, double data);
#else
			//bool operator== (const Vertex3D& rhs) const;
			//bool operator!= (const Vertex3D& rhs) const;
			Vertex3D operator+ (const Vertex3D& rhs) const;
			Vertex3D operator- (const Vertex3D& rhs) const;
			Vertex3D operator* (double data) const;
			Vertex3D operator/ (double data) const;
#endif

			// v1과 v2를 지나는 직선을 기준으로 현재의 버텍스와 대칭되는 객체를 만들어 리턴한다.
			// v1과 v2가 동일한 좌표일 경우 false를 리턴한다.
			bool Mirror(REF_CONST(Vertex3D) v1, REF_CONST(Vertex3D) v2, OUT CBR(INSTANCE(Vertex3D)) rResult) CONST;
			// v1, v2, v3를 지나는 평면을 기준으로 현재의 버텍스와 대칭되는 객체를 만들어 리턴한다.
			// v1, v2, v3가 평면을 구성하지 못할 경우 false를 리턴한다.
			bool Mirror(REF_CONST(Vertex3D) v1, REF_CONST(Vertex3D) v2, REF_CONST(Vertex3D) v3, OUT CBR(INSTANCE(Vertex3D)) rResult) CONST;

		public:
			virtual double GetDistance(REF_CONST(Vertex3<double>) rVertex) CONST override;
		};

		GEOMETRY_EXPORT_CLASS(Vertex3F) : PUBLIC Vertex3<float>
		{
		public:
			Vertex3F();
			Vertex3F(Vertex3FRefConst rhs);
			Vertex3F(float x, float y, float z);
			virtual ~Vertex3F(void);

		public:
#ifdef DOTNET
			//static bool operator== (Vertex3F^ op1, Vertex3F^ op2);
			//static bool operator!= (Vertex3F^ op1, Vertex3F^ op2);
			static Vertex3F^ operator+ (Vertex3F^ op1, Vertex3F^ op2);
			static Vertex3F^ operator- (Vertex3F^ op1, Vertex3F^ op2);
			static Vertex3F^ operator* (Vertex3F^ op, float data);
			static Vertex3F^ operator/ (Vertex3F^ op, float data);
#else
			//bool operator== (const Vertex3F& rhs) const;
			//bool operator!= (const Vertex3F& rhs) const;
			Vertex3F operator+ (const Vertex3F& rhs) const;
			Vertex3F operator- (const Vertex3F& rhs) const;
			Vertex3F operator* (float data) const;
			Vertex3F operator/ (float data) const;
#endif
			// v1과 v2를 지나는 직선을 기준으로 현재의 버텍스와 대칭되는 객체를 만들어 리턴한다.
			// v1과 v2가 동일한 좌표일 경우 false를 리턴한다.
			bool Mirror(REF_CONST(Vertex3F) v1, REF_CONST(Vertex3F) v2, OUT CBR(INSTANCE(Vertex3F)) rResult) CONST;
			// v1, v2, v3를 지나는 평면을 기준으로 현재의 버텍스와 대칭되는 객체를 만들어 리턴한다.
			// v1, v2, v3가 평면을 구성하지 못할 경우 false를 리턴한다.
			bool Mirror(REF_CONST(Vertex3F) v1, REF_CONST(Vertex3F) v2, REF_CONST(Vertex3F) v3, OUT CBR(INSTANCE(Vertex3F)) rResult) CONST;

		public:
			virtual float GetDistance(REF_CONST(Vertex3<float>) rVertex) CONST override;
		};

		GEOMETRY_EXPORT_CLASS(Vertex2D) : PUBLIC Vertex2<double>
		{
		public:
			Vertex2D();
			Vertex2D(Vertex2DRefConst rhs);
			Vertex2D(double x, double y);
			virtual ~Vertex2D(void);

		public:
#ifdef DOTNET
			//static bool operator== (Vertex2D^ op1, Vertex2D^ op2);
			//static bool operator!= (Vertex2D^ op1, Vertex2D^ op2);
			static Vertex2D^ operator+ (Vertex2D^ op1, Vertex2D^ op2);
			static Vertex2D^ operator- (Vertex2D^ op1, Vertex2D^ op2);
			static Vertex2D^ operator* (Vertex2D^ op, double data);
			static Vertex2D^ operator/ (Vertex2D^ op, double data);
#else
			//bool operator== (const Vertex2D& rhs) const;
			//bool operator!= (const Vertex2D& rhs) const;
			Vertex2D operator+ (const Vertex2D& rhs) const;
			Vertex2D operator- (const Vertex2D& rhs) const;
			Vertex2D operator* (double data) const;
			Vertex2D operator/ (double data) const;
#endif

			// v1과 v2를 지나는 직선을 기준으로 현재의 버텍스와 대칭되는 객체를 만들어 리턴한다.
			// v1과 v2가 동일한 좌표일 경우 false를 리턴한다.
			bool Mirror(REF_CONST(Vertex2D) v1, REF_CONST(Vertex2D) v2, OUT CBR(INSTANCE(Vertex2D)) rResult) CONST;

		public:
			virtual double GetDistance(REF_CONST(Vertex2<double>) rVertex) CONST override;
		};

		GEOMETRY_EXPORT_CLASS(Vertex2F) : PUBLIC Vertex2<float>
		{
		public:
			Vertex2F();
			Vertex2F(Vertex2FRefConst rhs);
			Vertex2F(float x, float y);
			virtual ~Vertex2F(void);

		public:
#ifdef DOTNET
			//static bool operator== (Vertex2F^ op1, Vertex2F^ op2);
			//static bool operator!= (Vertex2F^ op1, Vertex2F^ op2);
			static Vertex2F^ operator+ (Vertex2F^ op1, Vertex2F^ op2);
			static Vertex2F^ operator- (Vertex2F^ op1, Vertex2F^ op2);
			static Vertex2F^ operator* (Vertex2F^ op, float data);
			static Vertex2F^ operator/ (Vertex2F^ op, float data);
#else
			//bool operator== (const Vertex2F& rhs) const;
			//bool operator!= (const Vertex2F& rhs) const;
			Vertex2F operator+ (const Vertex2F& rhs) const;
			Vertex2F operator- (const Vertex2F& rhs) const;
			Vertex2F operator* (float data) const;
			Vertex2F operator/ (float data) const;
#endif

			// v1과 v2를 지나는 직선을 기준으로 현재의 버텍스와 대칭되는 객체를 만들어 리턴한다.
			// v1과 v2가 동일한 좌표일 경우 false를 리턴한다.
			bool Mirror(REF_CONST(Vertex2F) v1, REF_CONST(Vertex2F) v2, OUT CBR(INSTANCE(Vertex2F)) rResult) CONST;

		public:
			virtual float GetDistance(REF_CONST(Vertex2<float>) rVertex) CONST override;
		};
	}
}
